# File: S (Python 2.3)

__doc__ = 'Scheduler Unit Test '
import unittest
from . import Scheduler
print('This suite tests interface call validity only.')
print('To exercise the Scheduler algorithm, use the CPPUNIT suite and the spreadsheet')
testProtocolA = ((10000, 40, 0, 0), (0, 31, 0, 0), (1200, 31, 0, 0), (1200, 105, 300, 0), (0, 31, 0, 0), (280, 31, 0, 0), (280, 31, 0, 0), (900, 285, 0, 0), (0, 285, 900, 0), (0, 31, 0, 0), (280, 31, 0, 0), (280, 31, 0, 0), (900, 105, 0, 0), (0, 105, 480, 0), (0, 31, 0, 0), (280, 31, 0, 0), (280, 31, 0, 0), (900, 105, 0, 0), (0, 105, 480, 0), (0, 31, 0, 0), (280, 31, 0, 0), (280, 31, 0, 0), (900, 105, 0, 0), (0, 105, 300, 0), (0, 31, 0, 0), (280, 31, 0, 0), (280, 31, 0, 0), (900, 105, 300, 0), (0, 31, 0, 0), (280, 31, 0, 0), (280, 31, 0, 0))

def BuildBlockList(dataList):
    blockList = []
    for (openPeriod, usedPeriod, freePeriod, startTime) in dataList:
        t = Scheduler.TimeBlock()
        t.m_OpenPeriod = openPeriod
        t.m_UsedPeriod = usedPeriod
        t.m_FreePeriod = freePeriod
        blockList.append(t)
    
    return blockList


class ClassDefnCheck(unittest.TestCase):
    
    def testClasses(self):
        '''Scheduler and TimeBlock classes should be present '''
        attr = dir(Scheduler)
        self.assertNotEqual('Scheduler' in attr, 0)
        self.assertNotEqual('TimeBlock' in attr, 0)

    
    def testSchedulerConstructor(self):
        '''Scheduler constructor creates an object'''
        s = Scheduler.Scheduler()
        self.assertNotEqual('AppendBlock' in dir(s), 0)

    
    def testTimeBlockConstructor(self):
        '''TimeBlock constructor creates an object '''
        t = Scheduler.TimeBlock()
        self.assertNotEqual('m_StartTime' in dir(t), 0)



class TimeBlockCheck(unittest.TestCase):
    
    def testTimeBlock(self):
        '''TimeBlock attributes can be read/written '''
        t = Scheduler.TimeBlock()
        t.m_OpenPeriod = 1
        t.m_UsedPeriod = 2
        t.m_FreePeriod = 3
        t.m_StartTime = 4
        self.assertEqual(t.m_OpenPeriod, 1)
        self.assertEqual(t.m_UsedPeriod, 2)
        self.assertEqual(t.m_FreePeriod, 3)
        self.assertEqual(t.m_StartTime, 4)



class SchedulerCheckOK(unittest.TestCase):
    ''' Tests interfaces with good input '''
    
    def testAppendBlock(self):
        ''' AppendBlock returns an index which is >= 0, and monotonically increasing '''
        s = Scheduler.Scheduler()
        blockList = BuildBlockList(testProtocolA)
        batchID = 1
        index = -1
        for block in blockList:
            lastIndex = index
            index = s.AppendBlock(batchID, block)
            self.assertEqual(index, lastIndex + 1)
        

    
    def testGetBlock(self):
        ''' GetBlock returns the block as specified by the index '''
        blockList = BuildBlockList(testProtocolA)
        blockIndex = { }
        checkBlock = Scheduler.TimeBlock()
        s = Scheduler.Scheduler()
        batchID = 1
        for block in blockList:
            blockIndex[s.AppendBlock(batchID, block)] = block
        
        for index in list(blockIndex.keys()):
            self.assertNotEqual(s.GetBlock(batchID, index, checkBlock), 0)
            block = blockIndex[index]
            self.assertEqual(checkBlock.m_FreePeriod, block.m_FreePeriod)
            self.assertEqual(checkBlock.m_UsedPeriod, block.m_UsedPeriod)
            self.assertEqual(checkBlock.m_OpenPeriod, block.m_OpenPeriod)
        

    
    def testCalculate(self):
        ''' Calculate sets start times in a monotonic sequence '''
        blockList = BuildBlockList(testProtocolA)
        blockIndex = { }
        checkBlock = Scheduler.TimeBlock()
        s = Scheduler.Scheduler()
        batchID = 1
        for block in blockList:
            blockIndex[s.AppendBlock(batchID, block)] = block
        
        self.assertTrue(s.CalculateTimes(), 0)
        lastBlockEndTime = 0
        for index in list(blockIndex.keys()):
            self.assertNotEqual(s.GetBlock(batchID, index, checkBlock), 0)
            latestAllowedStartTime = lastBlockEndTime + checkBlock.m_OpenPeriod
            self.assertTrue(checkBlock.m_StartTime >= lastBlockEndTime)
            self.assertTrue(checkBlock.m_StartTime <= latestAllowedStartTime)
            lastBlockEndTime = checkBlock.m_StartTime + checkBlock.m_UsedPeriod + checkBlock.m_FreePeriod
        

    
    def testMaxIterations(self):
        ''' read/write MaxIterations as a property '''
        s = Scheduler.Scheduler()
        for i in range(10):
            s.SetMaxIterations(i)
            self.assertEqual(s.MaxIterations(), i)
        

    
    #def testNbrSchedules(self):
        ''' read/write NbrSchedules as a property '''
     #   s = Scheduler.Scheduler()
     #   for i in range(10):
     #       s.SetNbrSchedules(i)
     #       self.assertEqual(s.NbrSchedules(), i)
        

    
    def testNbrIterations(self):
        ''' Obtain number of iterations used in a calculation '''
        blockList = BuildBlockList(testProtocolA)
        s = Scheduler.Scheduler()
        batchID = 1
        for block in blockList:
            s.AppendBlock(batchID, block)
        
        self.assertTrue(s.NbrIterations() == 0)
        self.assertTrue(s.CalculateTimes())
        self.assertTrue(s.NbrIterations() > 0)

    
    def testResetBatchID(self):
        ''' Batch ID should be resusable after reset '''
        s = Scheduler.Scheduler()
        batchID = 1
        blockList = BuildBlockList(testProtocolA)
        blockIndex = { }
        for block in blockList:
            blockIndex[s.AppendBlock(batchID, block)] = block
        
        checkBlock = Scheduler.TimeBlock()
        self.assertNotEqual(s.GetBlock(batchID, 0, checkBlock), 0)
        s.Reset()
        self.assertEqual(s.GetBlock(batchID, 0, checkBlock), 0)

    
    #def testDelaySetting(self):
        ''' Batch start time should be delayed as required.  '''
     #   s = Scheduler.Scheduler()
     #   batchID = 1
     #   blockList = BuildBlockList(testProtocolA)
     #   blockIndex = { }
     #   for block in blockList:
     #       blockIndex[s.AppendBlock(batchID, block)] = block
        
     #   self.assertTrue(s.DelayFor(batchID) == 0)
     #   self.assertTrue(s.CalculateTimes(), 0)
     #   self.assertTrue(s.DelayFor(batchID) == 0)
     #   checkBlock = Scheduler.TimeBlock()
     #   self.assertNotEqual(s.GetBlock(batchID, 0, checkBlock), 0)
     #   self.assertTrue(checkBlock.m_StartTime == 0)
     #   delay = 100
     #   s.SetDelayFor(batchID, delay)
     #   self.assertTrue(s.DelayFor(batchID) == delay)
     #   self.assertTrue(s.CalculateTimes(), 0)
     #   self.assertNotEqual(s.GetBlock(batchID, 0, checkBlock), 0)
     #   self.assertTrue(checkBlock.m_StartTime == delay)



class SchedulerCheckBad(unittest.TestCase):
    ''' Tests interfaces with bad input '''
    
    def testAppendBlock(self):
        ''' AppendBlock requires a valid batchID '''
        s = Scheduler.Scheduler()
        block = Scheduler.TimeBlock()
        batchID = 1
        self.assertTrue(s.AppendBlock(0, block) >= 0)
        self.assertTrue(s.AppendBlock(-1, block) < 0)
        self.assertTrue(s.AppendBlock(batchID, block) >= 0)

    
    def testGetBlock(self):
        ''' GetBlock requires a valid batchID and index'''
        s = Scheduler.Scheduler()
        block = Scheduler.TimeBlock()
        self.assertTrue(s.GetBlock(0, 0, block) == 0)
        batchID = 1
        self.assertTrue(s.GetBlock(-1, 0, block) == 0)
        self.assertTrue(s.GetBlock(batchID + 1, 0, block) == 0)
        self.assertTrue(s.GetBlock(batchID, 0, block) == 0)
        index = s.AppendBlock(batchID, block)
        self.assertTrue(s.GetBlock(batchID, -1, block) == 0)
        self.assertTrue(s.GetBlock(batchID, index + 1, block) == 0)
        self.assertTrue(s.GetBlock(batchID, index, block) != 0)


if __name__ == '__main__':
    unittest.main()

