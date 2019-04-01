# 
# ThetaAxisTest.py
#
# Unit tests for ThetaAxis module
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
from tesla.hardware.ThetaAxis import *
import unittest
from SimpleStep.SimpleStep import SimpleStep
from tesla.hardware.config import HardwareConfiguration, gHardwareData

print '<=========== START OF UNITTEST ============>\n'
print 'We will need the SimpleStep card BoardID!!'
EMULATION = True

configData = gHardwareData.Section ('Carousel')

TOLERANCE = 1

name = "TestThetaAxis"    


# ---------------------------------------------------------------------------

class ThetaTestOK(unittest.TestCase):
    """Check that Theta calls operate correctly"""

    def setUp (self):
        self.m_Card = SimpleStep ('X0', 'COM1', EMULATION)
        self.m_Rotor = ThetaAxis (name, self.m_Card, configData)
    
    def testCreation(self):
        """Create Theta"""
        self.assertEqual (self.m_Rotor.m_Card, self.m_Card)
        
    def testHomeTheta(self):
        """Can we obtain home theta?"""
        self.assertAlmostEqual (float(configData[ThetaAxis.HomingThetaLabel]), self.m_Rotor.HomeTheta(), TOLERANCE)

    def testHoming(self):
        """Determine whether rotor needs homing, and whether it can be homed"""
        self.assertEqual (self.m_Rotor.IsHomed(), False)
        self.m_Rotor.Home()
        self.assertEqual (self.m_Rotor.IsHomed(), True)
        self.assertAlmostEqual (self.m_Rotor.Theta(), self.m_Rotor.HomeTheta(), TOLERANCE)
   
    def testTheta(self):
        """Can theta be selected between 0-360?"""
        self.m_Rotor.Home()
        minTheta = self.m_Rotor.MinTheta()
        maxTheta = self.m_Rotor.MaxTheta()-0.001
        sampleTheta = ThetaAxis.PI_DEG/4
        self.m_Rotor.SetTheta (sampleTheta)
        self.assertAlmostEqual (self.m_Rotor.Theta(), sampleTheta, TOLERANCE)
        self.m_Rotor.SetTheta (maxTheta)
        self.assertAlmostEqual (self.m_Rotor.Theta(), maxTheta, TOLERANCE)
        self.m_Rotor.SetTheta (minTheta)
        self.assertAlmostEqual (self.m_Rotor.Theta(), minTheta, TOLERANCE)
    
    def testThetaWrap(self):
        """Does theta wrap at 0 and 360?"""
        inc = 0.0001
        self.m_Rotor.Home()
        self.m_Rotor.SetTheta (0.0)
        self.assertAlmostEqual (self.m_Rotor.Theta(), 0.0, TOLERANCE)
        self.m_Rotor.SetTheta (ThetaAxis.TWOPI_DEG)
        self.assertAlmostEqual (self.m_Rotor.Theta(), 0.0, TOLERANCE)
        self.m_Rotor.SetTheta (ThetaAxis.TWOPI_DEG+inc)
        self.assertAlmostEqual (self.m_Rotor.Theta(), inc, TOLERANCE)
        self.m_Rotor.SetTheta (-inc)
        self.assertAlmostEqual (self.m_Rotor.Theta(), ThetaAxis.TWOPI_DEG-inc, TOLERANCE)
    
    def testIncrementTheta (self):
        """Can rotor be moved by given increment"""
        #TBD ???
        pass

    def testStepSpeedProfile (self):
        """Can step speed profile be modified."""
        self.m_Rotor.ResetStepSpeedProfile()
        (B, E, S) = self.m_Rotor.StepSpeedProfile()
        profile = '%d, %d, %d' % (B, E, S)
        self.assertEquals(configData[Axis.MotorStepVelocityLabel], profile)

        (newB, newE, newS) = (B+1, E+1, S+1)
        self.m_Rotor.SetStepSpeedProfile (newB, newE, newS)
        
        (newerB, newerE, newerS) = self.m_Rotor.StepSpeedProfile()
        self.assertEquals(newB, newerB)
        self.assertEquals(newE, newerE)
        self.assertEquals(newS, newerS)
        
        self.m_Rotor.ResetStepSpeedProfile()
        (B, E, S) = self.m_Rotor.StepSpeedProfile()
        profile = '%d, %d, %d' % (B, E, S)
        self.assertEquals(configData[Axis.MotorStepVelocityLabel], profile)


    def testHomingStepSpeedProfile (self):
        """Can homing step speed profile be modified."""
        self.m_Rotor.ResetHomingStepSpeedProfile()
        (B, E, S) = self.m_Rotor.HomingStepSpeedProfile()
        profile = '%d, %d, %d' % (B, E, S)
        self.assertEquals(configData[Axis.MotorHomingStepVelocityLabel], profile)

        (newB, newE, newS) = (B+1, E+1, S+1)
        self.m_Rotor.SetHomingStepSpeedProfile (newB, newE, newS)
        
        (newerB, newerE, newerS) = self.m_Rotor.HomingStepSpeedProfile()
        self.assertEquals(newB, newerB)
        self.assertEquals(newE, newerE)
        self.assertEquals(newS, newerS)
        
        self.m_Rotor.ResetHomingStepSpeedProfile()
        (B, E, S) = self.m_Rotor.HomingStepSpeedProfile()
        profile = '%d, %d, %d' % (B, E, S)
        self.assertEquals(configData[Axis.MotorHomingStepVelocityLabel], profile)


#--------------------------------------------------------------------------
    
class ThetaTestFail(unittest.TestCase):
    """Check that Theta calls handle improper input"""
    
    m_Card = SimpleStep('Y0', 'COM2', EMULATION)
    m_Config = configData

    def setUp (self):
        self.m_Config[ThetaAxis.MinThetaLabel] = '0.0'
        self.m_Config[ThetaAxis.MaxThetaLabel] = '%f' % (ThetaAxis.PI_DEG/2.0)
        self.m_xRotor = ThetaAxis (name, self.m_Card, self.m_Config) # do NOT home this!

        
    def testValidateTheta (self):
        """Validation should fail if not homed"""
        self.assertEquals (self.m_xRotor.IsHomed(), False, "Rotor should not be homed")
        self.assertRaises (AxisError, self.m_xRotor.Theta)
        self.assertRaises (AxisError, self.m_xRotor.SetTheta, ThetaAxis.PI_DEG/4.0)

    def testThetaLimits (self):
        """Theta may not be set outside configured limits"""
        self.m_xRotor.Home()
        self.assertAlmostEquals (float(self.m_Config[ThetaAxis.MinThetaLabel]), self.m_xRotor.MinTheta(), TOLERANCE)
        self.assertAlmostEquals (float(self.m_Config[ThetaAxis.MaxThetaLabel]), self.m_xRotor.MaxTheta(), TOLERANCE)
        self.m_xRotor.SetTheta (self.m_xRotor.MinTheta())
        self.m_xRotor.SetTheta (self.m_xRotor.MaxTheta())
        self.assertRaises (AxisError, self.m_xRotor.SetTheta, self.m_xRotor.MinTheta() - 0.1)
        self.assertRaises (AxisError, self.m_xRotor.SetTheta, self.m_xRotor.MaxTheta() + 0.1)
            
            
#--------------------------------------------------------------------
#    Invoke test:
#
if __name__ == '__main__':
    unittest.main()
            
# EOF
