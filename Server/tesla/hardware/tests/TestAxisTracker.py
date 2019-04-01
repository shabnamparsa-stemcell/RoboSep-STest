# 
# TestAxisTracker.py
#
# Unit tests for the base AxisTracker class
# 
# Copyright (c) Invetech Pty Ltd, 2003
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
# ---------------------------------------------------------------------------

import unittest
import os
from datetime import date

from tesla.hardware.AxisTracker import AxisTracker, Report 

class TestAxisTracker(unittest.TestCase):

    initialSetup = True
    instanceCount = 0
    
    def setUp(self):
        TestAxisTracker.instanceCount += 1
        self.dbPath = ".\TestAxisTracker.dbm"
        self.axisName = 'testAxis'
        self.maxStep = 10000
        self.nbrMoves = 42
        self.nbrMegaSteps = 2
        self.nbrSteps = 20000
        self.nbrOverflowSteps = 2000000
        self.nbrHomes = 10
        self.nbrFails = 2
        AxisTracker.dbFilePath = self.dbPath
        self.axisTracker = AxisTracker(self.axisName, self.maxStep)

        if TestAxisTracker.initialSetup:
            self.axisTracker.totalMovements = self.nbrMoves
            self.axisTracker.totalMegaSteps = self.nbrMegaSteps
            self.axisTracker.totalSteps = self.nbrSteps
            self.axisTracker.totalHomes = self.nbrHomes
            self.axisTracker.totalHomeFails = self.nbrFails
            TestAxisTracker.initialSetup = False

        else:            
            self.nbrMoves = self.axisTracker.totalMovements
            self.nbrMegaSteps = self.axisTracker.totalMegaSteps
            self.nbrSteps = self.axisTracker.totalSteps
            self.nbrHomes = self.axisTracker.totalHomes
            self.nbrFails = self.axisTracker.totalHomeFails

    def __del__ (self):
        TestAxisTracker.instanceCount -= 1

        if TestAxisTracker.instanceCount == 0:
            from tesla.hardware.AxisTracker import AxisTracker, Report 
            print 'Final result:'
            Report()
            os.remove (AxisTracker.dbFilePath)

        
    def tearDown(self):
        """ Update the movements """
        self.nbrMoves = self.axisTracker.totalMovements
        self.nbrMegaSteps = self.axisTracker.totalMegaSteps
        self.nbrSteps = self.axisTracker.totalSteps
        self.nbrHomes = self.axisTracker.totalHomes
        self.nbrFails = self.axisTracker.totalHomeFails
        self.axisTracker = None
        

    def test_init(self):
        """Test that creation loads the attributes as expected."""
        self.failUnless(self.axisTracker.maxStep == self.maxStep)
        self.failUnless(self.axisTracker.totalMovements == self.nbrMoves)
        self.failUnless(self.axisTracker.totalMegaSteps == self.nbrMegaSteps)
        self.failUnless(self.axisTracker.totalSteps == self.nbrSteps)
        self.failUnless(self.axisTracker.totalHomeFails == self.nbrFails)
        self.failUnless(self.axisTracker.totalHomes == self.nbrHomes)
        self.failUnless(self.axisTracker.commissionDate == date.today().toordinal())
        
    def test_noteSteps(self):
        """Test that noting steps updates total steps and movements."""
        nbrMoves = 10
        steps = 9
        for i in range (0, nbrMoves):
            self.axisTracker.NoteSteps(steps)

        self.failUnless(self.axisTracker.totalSteps == self.nbrSteps + steps*nbrMoves, "Each step is noted")
        self.failUnless(self.axisTracker.totalMovements == self.nbrMoves + nbrMoves, "Each move is noted")

    def test_noteStepsNeg(self):
        """Test that noting steps takes the absolute value of a negative increment."""
        nbrMoves = 10
        steps = -9
        for i in range (0, nbrMoves):
            self.axisTracker.NoteSteps(steps)

        self.failUnless(self.axisTracker.totalSteps == self.nbrSteps + abs(steps*nbrMoves), "Each step is noted")
        self.failUnless(self.axisTracker.totalMovements == self.nbrMoves + nbrMoves, "Each move is noted")

    def test_overflow1(self):
        """Test that overflow occurs correctly."""
        unMegaStep = AxisTracker.stepsPerMegaStep - 1 - self.nbrSteps
        self.axisTracker.NoteSteps(unMegaStep)
        self.failUnless(self.axisTracker.totalSteps == self.nbrSteps + unMegaStep, "Doesn't quite trigger overflow")
        self.failUnless(self.axisTracker.totalMegaSteps == self.nbrMegaSteps, "No overflow")
        self.failUnless(self.axisTracker.totalMovements == self.nbrMoves + 1, "Each move is noted")

        self.axisTracker.NoteSteps(1)
        self.failUnless(self.axisTracker.totalSteps == 0, "Triggered overflow")
        self.failUnless(self.axisTracker.totalMegaSteps == self.nbrMegaSteps + 1, "Overflow caught")
        self.failUnless(self.axisTracker.totalMovements == self.nbrMoves + 2, "Each move is noted")
        
    def test_overflow2(self):
        """Test that overflow occurs correctly with large steps."""
        megaStep = AxisTracker.stepsPerMegaStep
        self.axisTracker.NoteSteps(megaStep)
        self.failUnless(self.axisTracker.totalSteps == self.nbrSteps, "Triggered overflow")
        self.failUnless(self.axisTracker.totalMegaSteps == self.nbrMegaSteps + 1, "Overflow caught")
        self.failUnless(self.axisTracker.totalMovements == self.nbrMoves + 1, "Each move is noted")

        # To be sure...        
        self.axisTracker.NoteSteps(megaStep)
        self.failUnless(self.axisTracker.totalSteps == self.nbrSteps, "Triggered overflow")
        self.failUnless(self.axisTracker.totalMegaSteps == self.nbrMegaSteps + 2, "Overflow caught")
        self.failUnless(self.axisTracker.totalMovements == self.nbrMoves + 2, "Each move is noted")

    def test_NbrFullTraverses(self):
        """Test that the expected number of traverses is provided."""
        nbrTraverses = int((float(self.nbrMegaSteps)/self.maxStep)*AxisTracker.stepsPerMegaStep + self.nbrSteps/self.maxStep)
        self.failUnless(self.axisTracker.NbrFullTraverses() == nbrTraverses, "Each step is noted")

    def test_NoteHomeActionOK(self):
        """Test Noting of successful home action."""
        nbrSteps = 100
        self.axisTracker.NoteHomeAction (nbrSteps, True)
        self.failUnless(self.axisTracker.totalHomes == self.nbrHomes+1, "Homing action noted")
        self.failUnless(self.axisTracker.totalHomeFails == self.nbrFails, "Successful home noted")
        self.failUnless(self.axisTracker.totalSteps == self.nbrSteps + nbrSteps, "Homing steps noted")
        self.failUnless(self.axisTracker.totalMovements == self.nbrMoves + 1, "Movement noted")

    def test_NoteHomeActionNotOK(self):
        """Test Noting of unsuccessful home action."""
        nbrSteps = 100
        self.axisTracker.NoteHomeAction (nbrSteps, False)
        self.failUnless(self.axisTracker.totalHomes == self.nbrHomes+1, "Homing action noted")
        self.failUnless(self.axisTracker.totalHomeFails == self.nbrFails+1, "Successful home noted")
        self.failUnless(self.axisTracker.totalSteps == self.nbrSteps + nbrSteps, "Homing steps noted")
        self.failUnless(self.axisTracker.totalMovements == self.nbrMoves + 1, "Movement noted")

if __name__ == '__main__':
    unittest.main()

# eof
