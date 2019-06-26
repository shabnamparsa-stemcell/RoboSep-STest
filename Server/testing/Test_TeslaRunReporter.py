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
import os
import unittest
import run_reporter

# -----------------------------------------------------------------------------

class Test_SequenceContainer(unittest.TestCase):

    def setUp(self):
        self.seqcont = run_reporter.SequenceContainer()

    def test_emptySequence(self):
        self.assertTrue(len(self.seqcont) == 0, 'Testing length of empty sequence')
   
    def addSequence(self):
        self.myID = 67
        self.seqNum = 42
        self.seq = run_reporter.Sequence(self.seqNum, 100, 'My command', (), self.myID)
        self.seqcont.add(self.seq)
        
    def test_add(self):
        self.addSequence()
        self.assertTrue(len(self.seqcont) == 1, \
                'Testing length of sequence with one element')

    def test_getIDs(self):
        self.addSequence()
        ids = self.seqcont.getIDs()
        self.assertTrue(ids == [self.myID], 'Testing IDs')

    def test_getByID(self):
        self.addSequence()
        seqlist = self.seqcont.getByID(self.myID)
        self.assertTrue(seqlist == [self.seq], 'Testing getByID()')

    def test_noEvents(self):
        self.assertFalse(self.seqcont.hasEvent(0), 'We should have no events')

    def test_hasEvent(self):
        self.addSequence()
        self.assertTrue(self.seqcont.hasEvent(self.seqNum), 'Testing sequence')

# -----------------------------------------------------------------------------

class Test_TeslaRunReporter(unittest.TestCase):
    
    def setUp(self):
        filename = os.path.join('logs', 'robosep.log')
        self.run = run_reporter.TeslaRunReporter(filename)
        self.start = re.compile('AA')
        self.end = re.compile('ZZ')

    def test_filterOnPatternWithEmptyList(self):
        data = []
        results = self.run.filterOnPattern(self.start, self.end, data)
        self.assertTrue(results == [], 'Testing filter on empty list') 

    def test_filterOnPattern_1(self):
        # Testing one list
        firstEntry = 'AA starting'
        middleEntry = 'BB in the middle'
        lastEntry = 'ZZ the end'
        data = [firstEntry, middleEntry, lastEntry]
        results = self.run.filterOnPattern(self.start, self.end, data)
        self.assertTrue(len(results) == 1, 'Testing results')
        self.assertTrue(len(results[0]) == 3, 'Testing a single list')
        self.assertTrue(results[0][0] == firstEntry, 'Testing first entry')
        self.assertTrue(results[0][1] == middleEntry, 'Testing middle entry')
        self.assertTrue(results[0][2] == lastEntry, 'Testing last entry')

    def test_filterOnPattern_2(self):
        # We expect two lists with 'good' start and ends 
        data = ['AA 0', 'BB a', 'CC b', 'ZZ c', 'AA d', 'BB e', 'ZZ z']
        results = self.run.filterOnPattern(self.start, self.end, data)
        self.assertTrue(len(results) == 2, 'Testing two sets of results')
        set1 = results[0]
        set2 = results[1]
        self.assertTrue(len(set1) == 4, 'Testing set 1')
        self.assertTrue(len(set2) == 3, 'Testing set 2')
        moreResults = self.run.filterOnPattern(self.start, self.end, data, discardCrap = True)
        self.assertTrue(results == moreResults, 'Testing discardCrap')

    def test_filterOnPattern_PureJunk(self):
        # Test a list with no starts or ends
        data = ['BB b', 'CC c', 'DD d', 'EE e']
        results = self.run.filterOnPattern(self.start, self.end, data)
        self.assertTrue(results == [data], 'Testing filtering junk')

    def test_filterOnPattern_JunkAtStart(self):
        # Test a list with junk at the beginning: no start or end at the 
        # beginning of the list
        data1 = ['BB a', 'CC b', 'AA d', 'BB e', 'ZZ z']
        results1 = self.run.filterOnPattern(self.start, self.end, data1, discardCrap = True)
        results1crap = self.run.filterOnPattern(self.start, self.end, data1, discardCrap = False)
        self.assertFalse(results1 == results1crap, 'Testing discarded list != kept list')
        self.assertTrue(results1 == [['AA d', 'BB e', 'ZZ z']], 'Testing filtered list')
        self.assertTrue(results1crap == [['BB a', 'CC b'], ['AA d', 'BB e', 'ZZ z']], 'Testing crappy list')

    def test_filterOnPattern_JunkAtEnd(self):
        # Test a list with junk at the end
        data = ['AA a', 'BB b', 'ZZ c', 'EE e', 'GG g']
        results = self.run.filterOnPattern(self.start, self.end, data, discardCrap = True)
        resultsCrap = self.run.filterOnPattern(self.start, self.end, data, discardCrap = False)
        self.assertFalse(results == resultsCrap, 'Testing discarded list != kept list')
        self.assertTrue(results == [['AA a', 'BB b', 'ZZ c']], 'Testing filtered list')
        self.assertTrue(resultsCrap == [['AA a', 'BB b', 'ZZ c'], ['EE e', 'GG g']], 'Testing crappy list')

    def test_filterOnPatter_TwoStartsOneEnd(self):
        # Test a list with two starts and only one end
        # We should only be keeping the final start and end
        data2 = ['AA 0', 'BB a', 'CC b', 'AA d', 'BB e', 'ZZ z']
        results2 = self.run.filterOnPattern(self.start, self.end, data2, discardCrap = True)
        results2crap = self.run.filterOnPattern(self.start, self.end, data2, discardCrap = False)
        self.assertFalse(results2 == results2crap, 'Testing discarded list != kept list with false start')
        self.assertTrue(results2 == [['AA d', 'BB e', 'ZZ z']], 'Testing real list')
        self.assertTrue(results2crap == [['AA 0', 'BB a', 'CC b'], ['AA d', 'BB e', 'ZZ z']], 'Testing crappy list')
        
    def test_filterOnPatter_OneStartTwoEnds(self):
        # Test a list with one start and two ends
        # We should only be keeping the final start and end
        data = ['BB a', 'CC b', 'ZZ c', 'AA d', 'BB e', 'ZZ z']
        results = self.run.filterOnPattern(self.start, self.end, data, discardCrap = True)
        resultsCrap = self.run.filterOnPattern(self.start, self.end, data, discardCrap = False)
        self.assertFalse(results == resultsCrap, 'Testing discarded list != kept list with false start')
        self.assertTrue(results == [['AA d', 'BB e', 'ZZ z']], 'Testing real list')
        self.assertTrue(resultsCrap == [['BB a', 'CC b', 'ZZ c'], ['AA d', 'BB e', 'ZZ z']], 'Testing crappy list')

# -----------------------------------------------------------------------------

if __name__ == '__main__':
        unittest.main()
            
# eof

