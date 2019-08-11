# 
# Schedule.py
# tesla.control.Schedule
#
# Schedule object: created by a Scheduler instance, executed by a Dispatcher
#                  instance
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

class WorkflowContainer(object):
    '''Container for holding information we need to execute a workflow at a 
    specific time (for a specific sample).'''

    def __init__(self, sampleID, startTime_secs, workflowCall):
        '''For the specified sample ID, store the workflow command (as a 
        string) that we        need to execute it (by the dispatcher) at the 
        specified start time, which is relative to T = 0.'''
        self.__sampleID = sampleID
        self.__startTime = startTime_secs
        self.__workflowCall = workflowCall

    def __str__(self):
        '''Simple string representation'''
        return "ID = %d, Start @ %d secs, workflow = %s" % \
                (self.__sampleID, self.__startTime, self.__workflowCall)

    def getWorkflowAndTime(self):
        '''Return the workflow call and it's start time'''
        return (self.__workflowCall, self.__startTime)

    def getStartTime(self):
        return self.__startTime


    def setStartTime( self, startTime ):
        self.__startTime = startTime

    def getSampleID(self):
        '''Return the sample ID associated with this workflow'''
        return self.__sampleID

    #def __cmp__(self, otherWorkflow):
    def __lt__(self, otherWorkflow):
        '''Let's us sort WorkflowContainer objects, which we do based on start time.'''
        return self.__startTime < otherWorkflow.__startTime

# -----------------------------------------------------------------------------

class Schedule(object):
    '''This class represents all of the information required to execute the 
    commands associated with each of the samples in a sample list.
    Schedule instances are created by tesla.control.Scheduler and used by
    tesla.control.Dispatcher.'''
    
    def __init__(self):
        '''Simple constructor to initialise members. Populate the instance by
        calling addWorkflow().'''
        self.__workflows = []
        self.__changed = False

    def __str__(self):
        '''A simple string representation of the Schedule object.'''
        s = ''
        for wf in self.__workflows:
            s += ("[%s]," % (str(wf)))
        if s != '':
            s = s[:-1]            # Strip off any trailing ,
        return s

    def __len__(self):
        '''Length of the schedule == # workflows.'''
        return len(self.__workflows)

    def addWorkflow(self, sampleID, startTime, cmd):
        '''Add the information on when to schedule a sample's workflow to the
        Schedule instance. Three parameters: the sample ID, the time to start
        the workflow command and a string representation of the workflow
        command that we want to execute.'''
        self.__workflows.append(WorkflowContainer(sampleID, startTime, cmd))
        self.__changed = True

    def getWorkflows(self):
        '''Return a list of WorkflowContainer objects, allowing us to execute
        commands at specific times.'''
        if self.__changed:
            self.__workflows.sort()   # Added one or more items? Resort our list
            self.__changed = False
        return self.__workflows

#    def reverseWorkflowSchedule( self ):
#        '''Reverses the current schedule and reevaluates the start times'''
#        self.__workflows.sort()
#        self.__workflows.reverse()
#        maxStartTime = self.__workflows[0].getStartTime()
#        for i in range( len( self.__workflows ) ):
#            print "Changing", self.__workflows[i].getStartTime(), "to", maxStartTime - self.__workflows[i].getStartTime()
#            self.__workflows[i].setStartTime( maxStartTime - self.__workflows[i].getStartTime() )
        


        

# eof

