# 
# LinearAxisTest.py
#
# Unit tests for LinearAxis module
# 
# Copyright (c) Invetech Operations Pty Ltd, 2003
# 495 Blackburn Rd
# Mt Waverley, Vic, Australia.
# Phone (+61 3) 9211 7700
# Fax   (+61 3) 9211 7701
# 
# The copyright to the computer program(s) herein is the
# property of Invetech Operations Pty Ltd, Australia.
# The program(s) may be used and/or copied only with
# the written permission of Invetech Operations Pty Ltd
# or in accordance with the terms and conditions
# stipulated in the agreement/contract under which
# the program(s) have been supplied.
# 
# Notes: Using Python 2.3.3 (final)

# ---------------------------------------------------------------------------

from tesla.hardware.Axis import AxisError
from tesla.hardware.AxisTracker import AxisTracker
from tesla.hardware.LinearAxis import *
import unittest
from SimpleStep.SimpleStep import SimpleStep
from tesla.hardware.config import HardwareConfiguration, gHardwareData

print '<=========== START OF UNITTEST ============>\n'
print 'We will need the SimpleStep card BoardID!!'
EMULATION = True

configData = gHardwareData.Section ('Robot_ZAxis')

TOLERANCE = 6

name = "TestLinearAxis"    

# ---------------------------------------------------------------------------

class LinearTestOK(unittest.TestCase):
    """Check that linear axis calls operate correctly"""

    m_Card = SimpleStep ('X0', 'COM1', EMULATION)
    m_Axis = LinearAxis (name, m_Card, configData)
    tracker = m_Axis.m_Tracker
    
    def testCreation(self):
        """Create LinearAxis"""
        self.failUnless (self.m_Axis.m_Card == self.m_Card)
        self.failUnless (LinearTestOK.tracker.name == name)
        
    def testHomePosition(self):
        """Can we obtain home posiition?"""
        self.assertAlmostEqual (float(configData[LinearAxis.HomingPositionLabel]), self.m_Axis.HomePosition(), TOLERANCE)

    def testHoming(self):
        """Determine whether axis needs homing, and whether it can be homed"""
        self.m_Axis.Home()
        self.failUnless (self.m_Axis.IsHomed())
        self.assertAlmostEqual (self.m_Axis.Position(), self.m_Axis.HomePosition(), TOLERANCE)
   
    def testHomeTracking(self):
        """Determine that a homing action is tracked"""
        nbrMoves = LinearTestOK.tracker.totalMovements
        self.m_Axis.Home()
        self.m_Axis.Home()
        self.failUnless (nbrMoves != LinearTestOK.tracker.totalMovements)
   
    def testPosition(self):
        """Can position be selected between 0-max?"""
        minPosition = self.m_Axis.MinPosition()
        maxPosition = self.m_Axis.MaxPosition()
        samplePosition = (maxPosition+minPosition)/2.0
        nbrMoves = LinearTestOK.tracker.totalMovements
        self.m_Axis.SetPosition (samplePosition)
        self.assertAlmostEqual (self.m_Axis.Position(), samplePosition, TOLERANCE)
        self.m_Axis.SetPosition (maxPosition)
        self.assertAlmostEqual (self.m_Axis.Position(), maxPosition, TOLERANCE)
        self.m_Axis.SetPosition (minPosition)
        self.failUnless (nbrMoves != LinearTestOK.tracker.totalMovements)
        self.assertAlmostEqual (self.m_Axis.Position(), minPosition, TOLERANCE)
    
    def testIncrementPosition (self):
        """Can axis be moved by given increment"""
        #TBD ???
        pass



#--------------------------------------------------------------------------
    
class LinearTestFail(unittest.TestCase):
    """Check that LinearAxis calls handle improper input"""
    
    m_Card = SimpleStep('Y0', 'COM2', EMULATION)

    def setUp (self):
        self.m_xRotor = LinearAxis (name, self.m_Card, configData) # do NOT home this!

        
    def testValidatePosition (self):
        """Validation should fail if not homed"""
        posn = (self.m_xRotor.MinPosition()+self.m_xRotor.MinPosition())/2.0
        self.assertEquals (self.m_xRotor.IsHomed(), False, "Axis should not be homed")
        self.assertRaises (AxisError, self.m_xRotor.Position)
        self.assertRaises (AxisError, self.m_xRotor.SetPosition, posn)

    def testPositionLimits (self):
        """Position may not be set outside configured limits"""
        self.m_xRotor.Home()
        self.assertAlmostEquals (float(configData[LinearAxis.MinPositionLabel]), self.m_xRotor.MinPosition(), TOLERANCE)
        self.assertAlmostEquals (float(configData[LinearAxis.MaxPositionLabel]), self.m_xRotor.MaxPosition(), TOLERANCE)
        self.m_xRotor.SetPosition (self.m_xRotor.MinPosition())
        self.m_xRotor.SetPosition (self.m_xRotor.MaxPosition())
        self.assertRaises (AxisError, self.m_xRotor.SetPosition, self.m_xRotor.MinPosition() - 0.1)
        self.assertRaises (AxisError, self.m_xRotor.SetPosition, self.m_xRotor.MaxPosition() + 0.1)
            
            
#--------------------------------------------------------------------
#    Invoke test:
#
if __name__ == '__main__':
    unittest.main()
            
# EOF
