# File: S (Python 2.3)
import copy

class TimeBlock(object):
    
    def __init__(self):
        self.m_OpenPeriod = 0
        self.m_UsedPeriod = 0
        self.m_FreePeriod = 0
        self.m_StartTime  = -1


    def __repr__(self):
        '''String representation of the SchedulerBlock object, to aid debugging.'''
        return "%d %d %d %d" % (self.m_OpenPeriod, self.m_UsedPeriod, self.m_FreePeriod, self.m_StartTime)

class BatchState(object):

    PathAvailable = 0
    PathUsed      = 1
    PathWaiting   = 2

    def __init__(self, batchID, step, startAfter, startBefore, pathState):
        self.m_BatachID = batchID
        self.m_Step = step  #blockID
        self.m_StartAfter = startAfter
        self.m_StartBefore  = startBefore
        self.m_PathState  = pathState
        
class ScheduleState(object):
    
    def __init__(self, batchStateList):
        self.m_BatchStateDictionary = {}
        for batchState in batchStateList:
            self.m_BatchStateDictionary[batchState.m_BatachID] = batchState
        
    pass

class Scheduler(object):
    def __init__(self):
        self.Reset()

    #return Block ID (just use index for now)    
    def AppendBlock(self, batchID, block):
        block.m_StartTime  = -1
        if batchID < 0:
            return -1
        blockList = []
        if batchID in self.__BlocklistDictionary:
            blockList = self.__BlocklistDictionary[batchID]
        blockList.append(block)

        #print "################## AppendBlock ",block.m_OpenPeriod,block.m_UsedPeriod,block.m_FreePeriod

        #update dictionary in case batchID is new
        self.__BlocklistDictionary[batchID] = blockList
        
        return len(blockList) - 1
    
    #Returns True if Successful
    def CalculateTimes(self):

        #should track level of recursion
        #!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! TODO !!!!!!!!!!!!!!!!!!!!!!!!!!!!
        

        #initialize schedule state
        batchStateList = []
        for batchID, blockList in list(self.__BlocklistDictionary.items()):
            if blockList:
                batchState = BatchState(batchID,0,0,blockList[0].m_OpenPeriod,BatchState.PathAvailable)
                batchStateList.append(batchState)
        
        scheduleState = ScheduleState(batchStateList)
        return self.DoNextStep(0, scheduleState)

    #Returns True if Successful
    def DoNextStep(self, currentTime, scheduleState):

        if len(scheduleState.m_BatchStateDictionary) == 0:
            print("################## DoNextStep completed!")
            return True
            
        
        #check to see if scheduleState is still valid
        #!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! TODO !!!!!!!!!!!!!!!!!!!!!!!!!!!!

        #create a list of batchID in the order on which step I should perform next
        # PathAvailable < PathWaiting(smaller#) < PathWaiting(larger#)... Don't include PathUsed
        #for batchID, batchState in scheduleState.items():
        #batchIDOrderList = sorted(scheduleState.m_BatchStateDictionary.items() ,  key=lambda x: x[1].m_StartAfter)    
        batchIDOrderList = list(scheduleState.m_BatchStateDictionary.items())
        #batchIDOrderList.sort(key=lambda a,b: a[1].m_StartAfter-b[1].m_StartAfter)
        batchIDOrderList.sort(key=lambda a: a[1].m_StartAfter)

        #print for debug
        debug = True
        if debug:
            print("################## DoNextStep time ",currentTime)
            for batchID, batchState in batchIDOrderList:
                print(batchState.m_BatachID, batchState.m_Step, "from",batchState.m_StartAfter, "to", batchState.m_StartBefore) 


        #loop through batchID to perform
        for batchID, batchState in batchIDOrderList:
            nextScheduleState = copy.deepcopy(scheduleState)
            tmpBlock = TimeBlock()
            step = nextScheduleState.m_BatchStateDictionary[batchID].m_Step
            #print "CCCCCCCCCCCC 0"
            if self.GetBlock(batchID, step,tmpBlock) != 0 :
                print("CCCCCCCCCCCC Block ",batchID, tmpBlock.m_OpenPeriod,tmpBlock.m_UsedPeriod,tmpBlock.m_FreePeriod)
                tmpTime = currentTime

                if nextScheduleState.m_BatchStateDictionary[batchID].m_StartBefore < tmpTime:
                    return False

                
                if tmpTime < nextScheduleState.m_BatchStateDictionary[batchID].m_StartAfter:
                    tmpTime = nextScheduleState.m_BatchStateDictionary[batchID].m_StartAfter

                print("CCCCCCCCCCCC SetBlock ",batchID,step, tmpTime)
                self.SetBlockStartTime(batchID, step, tmpTime)
                
                tmpTime = tmpTime + tmpBlock.m_UsedPeriod
                tmpTimeAfter = tmpTime + tmpBlock.m_FreePeriod
                nextScheduleState.m_BatchStateDictionary[batchID].m_Step = step + 1
                nextScheduleState.m_BatchStateDictionary[batchID].m_StartAfter = tmpTimeAfter

                nextOpenPeriod = 0
                if self.GetBlock(batchID, step+1,tmpBlock) != 0 : #next block if exist
                    print("CCCCCCCCCCCC NEXT Block ",batchID, tmpBlock.m_OpenPeriod,tmpBlock.m_UsedPeriod,tmpBlock.m_FreePeriod)
                    nextOpenPeriod = tmpBlock.m_OpenPeriod
                    
                nextScheduleState.m_BatchStateDictionary[batchID].m_StartBefore = tmpTimeAfter + nextOpenPeriod
                if self.DoNextStep(tmpTime,nextScheduleState):
                    return True
            else:
                #can't find block, meaning batchID is completed
                nextScheduleState.m_BatchStateDictionary.pop(batchID)
                if self.DoNextStep(currentTime,nextScheduleState):
                    return True
        

        
        return False


    #return 0 if failed
    def GetBlock(self, batchID, blockID, tmpBlock):
        block = self.GetBlockHelper(batchID, blockID)
        if block is None:
            return 0
        tmpBlock.m_FreePeriod = block.m_FreePeriod
        tmpBlock.m_UsedPeriod = block.m_UsedPeriod
        tmpBlock.m_OpenPeriod = block.m_OpenPeriod
        tmpBlock.m_StartTime = block.m_StartTime
        #print "################## GetBlock ",block.m_OpenPeriod,block.m_UsedPeriod,block.m_FreePeriod
        return 1
            

    def SetBlockStartTime(self, batchID, blockID, currentTime):
        block = self.GetBlockHelper(batchID, blockID)
        #print "SetBlockStartTime",block,hex(id(block))
        if block is None:
            return
        block.m_StartTime = currentTime
        #print "SetBlockStartTime2",block.m_StartTime

    def GetBlockHelper(self, batchID, blockID):
        if batchID < 0 or batchID not in self.__BlocklistDictionary:
            return None
        blockList = self.__BlocklistDictionary[batchID]
        if blockID < 0 or len(blockList)<=blockID:
            return None
        #print "GetBlockHelper",blockList[blockID],hex(id(blockList[blockID]))
        return blockList[blockID]
    
    def Reset(self):
        self.__MaxIterations = 1
        #BatchID -> Block List
        self.__BlocklistDictionary = {}

    def MaxIterations(self):
        return self.__MaxIterations

    def SetMaxIterations(self, i):
        self.__MaxIterations = i



    



