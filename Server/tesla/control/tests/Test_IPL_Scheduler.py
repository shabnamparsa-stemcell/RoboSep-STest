# 
# Test_IPL_Scheduler.py
#
# Unit tests for the IPL scheduler running mock Tesla protocols
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

import unittest
import itertools

from ipl.scheduler.Scheduler import TimeBlock, Scheduler

# -----------------------------------------------------------------------------

class Test_IPLScheduler(unittest.TestCase):
    
    def setUp(self):
        self.sched = Scheduler()
    
    def createBatches(self, numBatches):
        for batchID in range(1, numBatches + 1):
            block = TimeBlock()
            block.m_OpenPeriod = 213 
            block.m_UsedPeriod = 71 
            block.m_FreePeriod = 0
            index = self.sched.AppendBlock(batchID, block)
            self.assertTrue(index >= 0, 'Ensuring valid index')                
    
    def test_blocks(self):
        numBatches = 4
        self.createBatches(numBatches)
        calcFlag = self.sched.CalculateTimes()
        self.assertTrue(calcFlag, 'Testing scheduling success')
        
        for batchID in range(1, numBatches + 1):
            index = itertools.count(0)
            while True:
                block = TimeBlock()
                block.m_OpenPeriod = 0
                block.m_UsedPeriod = 0
                block.m_FreePeriod = 0
                getFlag = self.sched.GetBlock(batchID, next(index), block)
                if not getFlag: break

                print("%d: %d %d %d" % (batchID, block.m_OpenPeriod, 
                        block.m_UsedPeriod, block.m_FreePeriod))
    
    def test_badSchedule(self):
        numBatches = 7 
        self.createBatches(numBatches)
        calcFlag = self.sched.CalculateTimes()
        self.assertFalse(calcFlag, 'Testing scheduling failure')
        

# -----------------------------------------------------------------------------
 
if __name__ == '__main__':
    unittest.main()

# eof
