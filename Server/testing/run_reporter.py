# 
# run_reporter.py
# Verification tool for reporting on Tesla sample processing runs
# 
# Copyright (c) Invetech Pty Ltd, 2004
# 495 Blackburn Rd
# Mt Waverley, Vic, Australia.
# Phone (+61 3) 9211 7700
# Fax   (+61 3) 9211 7701
# 
# The copyright to the computer program(s) herein is the
# property of Invetech Pty Ltd, Australia.
# The program(s) may be used and/or copied only with
# the written permission of Invetech Pty Ltd
# or in accordance with the terms and conditions
# stipulated in the agreement/contract under which
# the program(s) have been supplied.
# 

import re
import sys
import datetime

# -----------------------------------------------------------------------------

class TeslaRunException(Exception):
    '''An exception generated by the TeslaRunReporter, usually caused by a 
    file I/O problem or *really* bad data.'''
    pass

# -----------------------------------------------------------------------------
# These are our regular expressions that we are interested in

SYSTEM_START = 'INFO Error log initialised'
SYSTEM_END   = 'INFO Instrument shutdown$'
RUN_START    = 'CC: startRun() called'
# RUN_START    = 'INFO CC: startRun\(\) ETC ='
RUN_END      = 'DEBUG DI: __endOfRun\(\)'
DATETIME     = '^(\d{4}-\d{2}-\d{2}) (\d{2}:\d{2}:\d{2},\d+)\s'

SEQUENCE     = "sequence event\s+(\d+) = \((\d+), ['\"](\w+)\((.*\sid=(\d+))\)['\"]\)$"
DISPATCH     = "DEBUG DI: (\w+)\(.*, id=(\d+)*\)$"

# -----------------------------------------------------------------------------

class Sequence(object):
    '''A container for sequenced events.'''
    
    def __init__(self, sequenceNum, scheduledTime, cmd, params, id):
        '''Constructor with scheduled time (in secs), the command name (eg. 
        Transport), the command parameters and an ID.'''
        self.seqNum = sequenceNum
        self.scheduled = scheduledTime
        self.cmd = cmd
        self.params = params
        self.id = id

    def __str__(self):
        '''Return a string representation. At the moment, we don't return the
        params (for the sake of brevity).'''
        return "%d: ID = %s: %s @ Tsched = %d" % \
                (self.seqNum, self.id, self.cmd, self.scheduled)

    def getTSV(self):
        return "%d\t%s\t%d" % (self.seqNum, self.cmd, self.scheduled)

# -----------------------------------------------------------------------------

class SequenceContainer(object):
    '''A container for holding Sequence objects.'''
    
    def __init__(self, *sequences):
        '''Create the container. If we are passed Sequence objects, we add
        them.'''
        self.sequences = {}
        for seq in sequences:
            self.add(seq)

    def __str__(self):
        '''Return a string representation of the container.'''
        return "Sequence container: %d sequences" % (len(self))

    def __len__(self):
        '''Return the length of the sequence container.'''
        return len(list(self.sequences.values()))

    def add(self, sequence):
        '''Add a sequence.'''
        self.sequences[sequence.seqNum] = sequence

    def getIDs(self):
        '''Return all the IDs in the container.'''
        ids = {}
        for id in [x.id for x in list(self.sequences.values())]:
            ids[id] = 1
        ids = list(ids.keys())
        ids.sort()
        return ids

    def getByID(self, id):
        '''Return a list of all Sequences with this ID.'''
        return [self.sequences[x] for x in list(self.sequences.keys()) if self.sequences[x].id == id]

    def hasEvent(self, sequenceNum):
        '''Returns true if we have already have stored this sequence, as
        specified by the sequence number.'''
        return sequenceNum in self.sequences

# -----------------------------------------------------------------------------

class TeslaRunReporter:
    '''Class that generates an Excel spreadsheet of reliability and 
    verification data from a Tesla RoboSep log file.'''

    OUTPUT_EXTENSION = '.xls'
    
    def __init__(self, logfileName):
        '''Construct a TeslaRunReporter instance with the log file name.'''
        # Read in the file
        self.logfileName = logfileName
        try:
            f = open(logfileName, 'r')
            fileData = f.readlines()
            f.close()
            self.data = [x.strip() for x in fileData]
        except IOError as msg:
            raise TeslaRunException("Error reading log file: %s" % (msg))
            self.data = []
        # Now build up our regular expression objects
        self.re = {}
        for pattern in (SYSTEM_START, SYSTEM_END, RUN_START, RUN_END, DATETIME,
                            SEQUENCE, DISPATCH):
            self.re[pattern] = re.compile(pattern)

    def filterOnPattern(self, startPattern, endPattern, myList, discardCrap = False):
        '''Return a list of lists, where each list is defined by the start
        and end patterns.'''
        hits = []
        foundStart = False; foundEnd = False
        collection = []
        for entry in myList:
            if startPattern.search(entry):
                if (foundStart and not foundEnd) or (not foundStart and len(collection) > 1):
                    if not discardCrap:
                        hits.append(collection)
                    collection = []
                collection.append(entry)                    
                foundStart = True
            elif endPattern.search(entry):
                if (foundStart or not discardCrap):
                    collection.append(entry)
                    hits.append(collection)
                collection = []
                foundStart = False
                foundEnd = True
            else:
                collection.append(entry)
        if len(collection) > 0:
            if not foundStart and not discardCrap:
                hits.append(collection)
        return hits

    def filterLogs(self):
        '''How many logs are in this file (where a log is defined as the
        entries generated for a "power cycle" of the instrument software)?"
        Return a list of list of entries, one per log.'''
        return self.filterOnPattern(self.re[SYSTEM_START], self.re[SYSTEM_END], self.data, False)

    def filterSampleRuns(self, log):
        '''How many runs are in this log? Find them and then separate each one
        into a separate block of data. Return a list of the runs.'''
        return self.filterOnPattern(self.re[RUN_START], self.re[RUN_END], log, False)

    def processRun(self, runData):
        '''Process the data in the run list.'''
        events = SequenceContainer()
        t0 = None
        for entry in runData:
            seqMatch = self.re[SEQUENCE].search(entry)
            if seqMatch:
                seqNum = int(seqMatch.group(1))
                scheduledTime = int(seqMatch.group(2))
                cmd = seqMatch.group(3)
                params = seqMatch.group(4)
                id = seqMatch.group(5)
                if events.hasEvent(seqNum):
                    raise TeslaRunException("Sequence number %d is duplicated in %s!" % \
                            (seqNum, entry))
                else:
                    seq = Sequence(seqNum, scheduledTime, cmd, params, id)
                    events.add(seq)
            else:
                dispatchMatch = self.re[DISPATCH].search(entry)
                if dispatchMatch:
                    dispatchTime = self.getEpochTime(entry)
                    if t0 == None:
                        t0 = dispatchTime
                    print(dispatchTime - t0, dispatchMatch.group(1), dispatchMatch.group(2))

                    
        ids = events.getIDs()
        for id in ids:
            print("-" * 78)
            print("ID = %s" % (id))
            for event in events.getByID(id):
                print(event.getTSV())
            print()

    def getEpochTime(self, line):
        '''For the supplied time, return epoch time.'''
        date, time = self.getDateAndTime(line)
        elems = [int(x) for x in time[0:8].split(':')]
        t = elems[0] * 3600 + elems[1] * 60 + elems[2]
        return t

    def getDateAndTime(self, line):
        '''For the supplied line, return a tuple of date and time.'''
        date = time = 'N/A'
        matches = self.re[DATETIME].search(line)
        if matches:
            date = matches.group(1)
            time = matches.group(2)
        return (date, time)

    def getMetadata(self):
        '''Extract the metadata from the log file. Returns a dictionary of
        information.'''
        metadata = {}
        metadata['start date'], metadata['start time'] = self.getDateAndTime(self.data[0])
        metadata['end date'], metadata['end time'] = self.getDateAndTime(self.data[1])
        return metadata

    def run(self, output = None):
        '''Run the analysis and generate the report. By default, the output
        filename will be the logfile name with a '.xls' extension.'''
        if output == None:
            output = self.logfileName + self.OUTPUT_EXTENSION
        metadata = self.getMetadata()

        for log in self.filterLogs():
            sampleRuns = self.filterSampleRuns(log)
            for run in sampleRuns:
                self.processRun(run)
                
# -----------------------------------------------------------------------------

if __name__ == '__main__':
    try:
        filename = sys.argv[1]
        outputName = filename + '.xls'
        reporter = TeslaRunReporter(filename)
        reporter.run(output = outputName)
    except IndexError:
        print("\n\tSyntax: %s logfile" % (sys.argv[0]))
    
# eof

