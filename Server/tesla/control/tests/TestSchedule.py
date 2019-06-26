# 
# TestSchedule.py
#
# Unit tests for tesla.control.Schedule
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
from tesla.control.Schedule import WorkflowContainer, Schedule

# -----------------------------------------------------------------------------

class Test_WorkflowContainer(unittest.TestCase):

    def setUp(self):
        self.sampleID = 1
        self.workflowCall = 'Incubate(300)'
        self.startTime = 120
        self.wf1 = WorkflowContainer(self.sampleID, self.startTime, self.workflowCall)
        self.wf2 = WorkflowContainer(1, 75, 'Incubate(500)')
        self.wf3 = WorkflowContainer(1, 360, 'Incubate(600)')

        self.workflows = [self.wf1, self.wf2, self.wf3]

    def test_str(self):
        self.assertTrue(str(self.wf1), 'Testing WorkflowContainer __str__()')

    def test_getWorkflowContainerAndTime(self):
        workflowCall, startTime = self.wf1.getWorkflowAndTime()
        self.assertTrue(workflowCall == self.workflowCall, 'Testing Workflow call string')
        self.assertTrue(startTime == self.startTime, 'Testing Workflow start time')

    def test_getSampleID(self):
        id = self.wf1.getSampleID()
        self.assertTrue(id == self.sampleID, 'Testing sample ID')

    def test_cmp(self):
        self.assertTrue(self.wf2 < self.wf1, 'Testing _cmp_')
        unsortedList = self.workflows[:]
        self.workflows.sort()
        self.assertTrue(unsortedList != self.workflows, 'Testing sorted WorkflowContainer list')
        for i in range(1, len(self.workflows)):
            cmd, timeA = self.workflows[i - 1].getWorkflowAndTime()
            cmd, timeB = self.workflows[i].getWorkflowAndTime()
            self.assertTrue(timeB > timeA, 'Testing __cmp__ in sorted list')
            
# -----------------------------------------------------------------------------

class TestSchedule(unittest.TestCase):
   
    def setUp(self):
        self.sched = Schedule()
        self.sched.addWorkflow(1, 120, 'Incubate(300)')

    def test_len(self):
        self.assertTrue(len(self.sched) == 1, 'Testing schedule length')

    def test_str(self):
        self.assertTrue(str(self.sched), 'Testing Schedule __str__()')
        
# -----------------------------------------------------------------------------
 
if __name__ == '__main__':
    unittest.main()

# eof
