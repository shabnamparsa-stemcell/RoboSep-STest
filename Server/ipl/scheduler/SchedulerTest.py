# File: S (Python 2.3)

__doc__ = 'Scheduler Unit Test '
import unittest
from . import Scheduler
print('This suite tests interface call validity only.')
print('To exercise the Scheduler algorithm, use the CPPUNIT suite and the spreadsheet')
testProtocolA = ((10000, 40, 0, 0), (0, 31, 0, 0), (1200, 31, 0, 0), (1200, 105, 300, 0),
                 (0, 31, 0, 0), (280, 31, 0, 0), (280, 31, 0, 0), (900, 285, 0, 0), (0, 285, 900, 0),
                 (0, 31, 0, 0), (280, 31, 0, 0), (280, 31, 0, 0), (900, 105, 0, 0), (0, 105, 480, 0),
                 (0, 31, 0, 0), (280, 31, 0, 0), (280, 31, 0, 0), (900, 105, 0, 0), (0, 105, 480, 0),
                 (0, 31, 0, 0), (280, 31, 0, 0), (280, 31, 0, 0), (900, 105, 0, 0), (0, 105, 300, 0),
                 (0, 31, 0, 0), (280, 31, 0, 0), (280, 31, 0, 0), (900, 105, 300, 0),
                 (0, 31, 0, 0), (280, 31, 0, 0), (280, 31, 0, 0))


testProtocolCantDo4WithDefaultSettings = ((600, 0, 0, -1), (0, 31, 240, -1), (226, 0, 0, -1), (0, 41, 0, -1),
                                          (226, 31, 0, -1),(0, 31, 0, -1), (151, 41, 0, -1), (151, 58, 2, -1),
                                          (226, 0, 0, -1), (226, 87, 0, -1), (0, 50, 0, -1),
                                          (226, 41, 0, -1), (226, 56, 3, -1), (226, 0, 0, -1), (0, 26, 0, -1))

testProtocol19054w2ml = ((600, 0, 0, -1), (0, 31, 600, -1), (226, 0, 0, -1), (0, 56, 0, -1),
                        (226, 31, 300, -1), (226, 0, 0, -1), (0, 30, 0, -1), (0, 50, 600, -1),
                      (226, 0, 0, -1), (0, 30, 0, -1), (0, 31, 0, -1))

testProtocol18054highpurityw2ml = ((600, 0, 0, -1), (120, 58, 300, -1), (300, 0, 0, -1), (300, 88, 0, -1),
                                (120, 58, 300, -1), (300, 0, 0, -1), (0, 41, 0, -1), (300, 58, 0, -1),
                                (120, 58, 300, -1), (300, 0, 0, -1), (0, 88, 0, -1), (300, 48, 480, -1),
                                (300, 0, 0, -1), (0, 30, 0, -1), (0, 48, 720, -1), (300, 0, 0, -1),
                                (0, 30, 0, -1), (0, 33, 0, -1))
                                          
testProtocol19054w8ml = ((600, 0, 0, -1), (0, 58, 600, -1), (226, 0, 0, -1), (0, 88, 0, -1),
                        (226, 30, 300, -1), (226, 0, 0, -1), (0, 41, 0, -1), (0, 46, 600, -1),
                      (226, 0, 0, -1), (0, 41, 0, -1), (0, 33, 0, -1))

testProtocol18054highpurityw8ml = ((600, 0, 0, -1), (120, 58, 300, -1), (300, 0, 0, -1), (300, 88, 0, -1),
                                (120, 58, 300, -1), (300, 0, 0, -1), (0, 41, 0, -1), (300, 58, 0, -1),
                                (120, 58, 300, -1), (300, 0, 0, -1), (0, 88, 0, -1), (300, 30, 480, -1),
                                (300, 0, 0, -1), (0, 41, 0, -1), (0, 50, 720, -1), (300, 0, 0, -1),
                                (0, 41, 0, -1), (0, 29, 0, -1))


                                          
resultProtocol19054w8mlx4 = """001_000 0 600 0 0
001_001 0 0 58 600
002_000 58 600 0 0
002_001 58 0 58 600
003_000 116 600 0 0
003_001 116 0 58 600
004_000 174 600 0 0
004_001 174 0 58 600
001_002 658 226 0 0
001_003 658 0 88 0
002_002 746 226 0 0
002_003 746 0 88 0
001_004 834 226 30 300
003_002 864 226 0 0
003_003 864 0 88 0
004_002 952 226 0 0
004_003 952 0 88 0
002_004 1040 226 30 300
003_004 1070 226 30 300
004_004 1100 226 30 300
001_005 1164 226 0 0
001_006 1164 0 41 0
001_007 1205 0 46 600
002_005 1370 226 0 0
002_006 1370 0 41 0
002_007 1411 0 46 600
003_005 1457 226 0 0
003_006 1457 0 41 0
003_007 1498 0 46 600
004_005 1544 226 0 0
004_006 1544 0 41 0
004_007 1585 0 46 600
001_008 1851 226 0 0
001_009 1851 0 41 0
001_010 1892 0 33 0
002_008 2057 226 0 0
002_009 2057 0 41 0
002_010 2098 0 33 0
003_008 2144 226 0 0
003_009 2144 0 41 0
003_010 2185 0 33 0
004_008 2231 226 0 0
004_009 2231 0 41 0
004_010 2272 0 33 0
"""

resultProtocol18054highpurityw8mlx4 = """001_000 0 600 0 0
001_001 0 120 58 300
002_000 58 600 0 0
002_001 58 120 58 300
003_000 58 600 0 0
004_000 58 600 0 0
003_001 116 120 58 300
004_001 174 120 58 300
001_002 358 300 0 0
001_003 358 300 88 0
001_004 446 120 58 300
002_002 446 300 0 0
002_003 504 300 88 0
003_002 592 300 0 0
003_003 592 300 88 0
004_002 592 300 0 0
002_004 680 120 58 300
003_004 738 120 58 300
004_003 796 300 88 0
001_005 884 300 0 0
001_006 884 0 41 0
004_004 925 120 58 300
001_007 983 300 58 0
002_005 1041 300 0 0
002_006 1041 0 41 0
001_008 1082 120 58 300
002_007 1140 300 58 0
003_005 1198 300 0 0
003_006 1198 0 41 0
002_008 1239 120 58 300
003_007 1297 300 58 0
004_005 1355 300 0 0
004_006 1355 0 41 0
003_008 1396 120 58 300
004_007 1454 300 58 0
001_009 1512 300 0 0
001_010 1512 0 88 0
004_008 1600 120 58 300
002_009 1658 300 0 0
002_010 1658 0 88 0
001_011 1746 300 30 480
002_011 1776 300 30 480
003_009 1806 300 0 0
003_010 1806 0 88 0
003_011 1894 300 30 480
004_009 1958 300 0 0
004_010 1958 0 88 0
004_011 2046 300 30 480
001_012 2256 300 0 0
001_013 2256 0 41 0
001_014 2297 0 50 720
002_012 2347 300 0 0
002_013 2347 0 41 0
002_014 2388 0 50 720
003_012 2438 300 0 0
003_013 2438 0 41 0
003_014 2479 0 50 720
004_012 2556 300 0 0
004_013 2556 0 41 0
004_014 2597 0 50 720
001_015 3067 300 0 0
001_016 3067 0 41 0
001_017 3108 0 29 0
002_015 3158 300 0 0
002_016 3158 0 41 0
002_017 3199 0 29 0
003_015 3249 300 0 0
003_016 3249 0 41 0
003_017 3290 0 29 0
004_015 3367 300 0 0
004_016 3367 0 41 0
004_017 3408 0 29 0
"""



testProtocolImpossible1of2 = ((300, 31, 240, -1), (0, 240, 0, -1))
testProtocolImpossible2of2 = ((300, 31, 0, -1), (10, 240, 0, -1))

testProtocolBacktrack1of2 = ((300, 31, 240, -1), (0, 31, 0, -1))
testProtocolBacktrack2of2 = ((300, 31, 0, -1), (10, 240, 0, -1))

testProtocolSimple1of2 = ((300, 31, 50, -1), (100, 31, 0, -1))
testProtocolSimple2of2 = ((300, 31, 20, -1), (10, 31, 0, -1))

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

    def checkBlocks(self, scheduler, batchID):
        checkBlock = Scheduler.TimeBlock()
        lastBlockEndTime = 0
        index = 0
        while scheduler.GetBlock(batchID, index, checkBlock) != 0:
            latestAllowedStartTime = lastBlockEndTime + checkBlock.m_OpenPeriod
            self.assertTrue(checkBlock.m_StartTime >= lastBlockEndTime)
            self.assertTrue(checkBlock.m_StartTime <= latestAllowedStartTime)
            lastBlockEndTime = checkBlock.m_StartTime + checkBlock.m_UsedPeriod + checkBlock.m_FreePeriod
            index = index + 1

    def checkAllProtocolsFinishedWithin10min(self, scheduler):
        result = ""
        lastBlockDictionary={}
        bathcIDs = scheduler.GetBatchIDs()
        #Collect last block time
        for batchID in bathcIDs:
            index = 0
            finishedTime = 0
            block = Scheduler.TimeBlock()
            while scheduler.GetBlock(batchID, index, block) != 0:
                finishedTime = block.m_StartTime + block.m_UsedPeriod + block.m_FreePeriod
                lastBlockDictionary[batchID] = finishedTime
                index = index + 1


        #find min and max time
        minTime = -1
        maxTime = 0
        for batchID in bathcIDs:
            tmpTime = lastBlockDictionary[batchID]
            print("BatchID %d -> time: %d" % (batchID,tmpTime))
            if minTime == -1:
                minTime = tmpTime
            if tmpTime < minTime:
                minTime = tmpTime
            if tmpTime > maxTime:
                maxTime = tmpTime

        
        print("maxTime %d - minTime: %d = %d < 600" % (maxTime,minTime,maxTime-minTime))
        self.assertTrue(maxTime-minTime < 600, "Protocol batch should end within 10mins of each other")

    def makeBlockCopy(self, block):
        tmpBlock = Scheduler.TimeBlock()
        tmpBlock.m_OpenPeriod = block.m_OpenPeriod
        tmpBlock.m_UsedPeriod = block.m_UsedPeriod
        tmpBlock.m_FreePeriod = block.m_FreePeriod
        tmpBlock.m_StartTime  = block.m_StartTime
        return tmpBlock
        

    def printOrderBlocks(self, scheduler):
        result = ""
        blockDictionary={}
        bathcIDs = scheduler.GetBatchIDs()
        for batchID in bathcIDs:
            index = 0
            block = Scheduler.TimeBlock()
            while scheduler.GetBlock(batchID, index, block) != 0:
                tmpBlock = self.makeBlockCopy(block)
                key1 = "%03d" % (batchID,)
                key2 = "%03d" % (index,)
                blockDictionary[key1+"_"+key2] = tmpBlock
                #print( str(batchID)+"_"+str(index) + " " + str(tmpBlock.m_StartTime) + " " + \
                #        str(tmpBlock.m_OpenPeriod)+ " " + str(tmpBlock.m_UsedPeriod) + " " + str(tmpBlock.m_FreePeriod) + '\n')
                
                index = index + 1


        def cmp_items(a, b):
            if a[1].m_StartTime > b[1].m_StartTime:
                return 1
            elif a[1].m_StartTime == b[1].m_StartTime and a[0] == b[0]:
                return 0
            elif a[1].m_StartTime == b[1].m_StartTime and a[0] > b[0]:
                return 1
            else:
                return -1        
        def takeStart(elem):
            return "%08d_%s" % (elem[1].m_StartTime,elem[0])
        
        newList = blockDictionary.items()
        #newList.sort(lambda a,b: a[1].m_StartTime-b[1].m_StartTime)
        #sorted(newList, cmp=cmp_items)
        newList = sorted(blockDictionary.items(), key=takeStart)
        #newList.sort(cmp_items)

        for key, block in newList:
            start = block.m_StartTime
            openTime = block.m_OpenPeriod
            usedTime = block.m_UsedPeriod
            freeTime = block.m_FreePeriod
            result = result + str(key)+ " " + str(start) + " " + \
                        str(openTime)+ " " + str(usedTime) + " " + str(freeTime) + '\n'
        return result


    def testScheduleFailed1(self):
        blockList = BuildBlockList(testProtocolCantDo4WithDefaultSettings)
        checkBlock = Scheduler.TimeBlock()
        s = Scheduler.Scheduler()

        #can do 3
        for block in blockList:
            for batchID in range(3):
                s.AppendBlock(batchID, block)
        self.assertTrue(s.CalculateTimes(), "Should be able to schedule 3")
        
        #cant do 4
        for block in blockList:
            s.AppendBlock(3, block)
        self.assertFalse(s.CalculateTimes(), "Should NOT be able to schedule 4 given defaults")


    def testScheduleFailed2(self):
        blockList = BuildBlockList(testProtocolImpossible1of2)
        blockList2 = BuildBlockList(testProtocolImpossible1of2)
        checkBlock = Scheduler.TimeBlock()
        s = Scheduler.Scheduler()
        for block in blockList:
            s.AppendBlock(1, block)
        for block in blockList2:
            s.AppendBlock(2, block)
        self.assertFalse(s.CalculateTimes(), "Should NOT be able to schedule impossible")
        
    def testScheduleBacktrack1(self):
        blockList = BuildBlockList(testProtocolBacktrack1of2)
        blockList2 = BuildBlockList(testProtocolBacktrack2of2)
        checkBlock = Scheduler.TimeBlock()
        s = Scheduler.Scheduler()
        for block in blockList:
            s.AppendBlock(1, block)
        for block in blockList2:
            s.AppendBlock(2, block)
        self.assertTrue(s.CalculateTimes(), "Should be able to schedule with backtrack")
        self.checkBlocks(s,1)
        self.checkBlocks(s,2)


    def testScheduleSimple1(self):
        blockList = BuildBlockList(testProtocolSimple1of2)
        blockList2 = BuildBlockList(testProtocolSimple2of2)
        checkBlock = Scheduler.TimeBlock()
        s = Scheduler.Scheduler()
        for block in blockList:
            s.AppendBlock(1, block)
        for block in blockList2:
            s.AppendBlock(2, block)
        self.assertTrue(s.CalculateTimes(), "Should be able to schedule with simple")
        self.checkBlocks(s,1)
        self.checkBlocks(s,2)

    def testProtocol19054w8mlx4(self):
        blockList = BuildBlockList(testProtocol19054w8ml)
        checkBlock = Scheduler.TimeBlock()
        s = Scheduler.Scheduler()
        for block in blockList:
            s.AppendBlock(1, block)
            s.AppendBlock(2, block)
            s.AppendBlock(3, block)
            s.AppendBlock(4, block)
        self.assertTrue(s.CalculateTimes(), "Should be able to schedule 4 19054 with 8ml")
        self.checkBlocks(s,1)
        self.checkBlocks(s,2)
        self.checkBlocks(s,3)
        self.checkBlocks(s,4)
        result = self.printOrderBlocks(s) == resultProtocol19054w8mlx4
        #self.assertTrue(result, "Should have same result as resultProtocol19054w8mlx4")
        self.checkAllProtocolsFinishedWithin10min(s)
        


    def testProtocol18054highpurityw8mlx4(self):
        blockList = BuildBlockList(testProtocol18054highpurityw8ml)
        checkBlock = Scheduler.TimeBlock()
        s = Scheduler.Scheduler()
        for block in blockList:
            s.AppendBlock(3, block)
            s.AppendBlock(2, block)
            s.AppendBlock(1, block)
            s.AppendBlock(4, block)
        self.assert_(s.CalculateTimes(), "Should be able to schedule 4 18054highpurity with 8ml")
        self.checkBlocks(s,1)
        self.checkBlocks(s,2)
        self.checkBlocks(s,3)
        self.checkBlocks(s,4)
        tmpPrint = self.printOrderBlocks(s)
        print(tmpPrint)
        result = tmpPrint == resultProtocol18054highpurityw8mlx4
        #self.assert_(result, "Should have same result as resultProtocol18054highpurityw8mlx4")
        self.checkAllProtocolsFinishedWithin10min(s)
        


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

