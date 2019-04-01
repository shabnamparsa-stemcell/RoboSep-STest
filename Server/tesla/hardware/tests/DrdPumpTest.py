# 
# DrdPumpTest.py
#
# Unit tests for DrdPump module
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

import unittest
from tesla.hardware.DrdPump import DrdPump, DrdPumpError
from tesla.hardware.Axis import AxisError
from SimpleStep.SimpleStep import SimpleStep
from tesla.hardware.config import HardwareConfiguration

print '<=========== START OF UNITTEST ============>\n'
print 'We will need the SimpleStep card BoardID!!'
EMULATION = True

hwcfg = HardwareConfiguration('C:\projects\hardware.ini')

configData = hwcfg.Section ('DrdPump')


TOLERANCE = 1


# ---------------------------------------------------------------------------

class DrdPumpOK(unittest.TestCase):
    """Check that DrdPump calls operate correctly"""

# bdr  m_Card = SimpleStep ('D1', 'COM1', EMULATION)
    m_Card = SimpleStep ('X1', 'COM1', EMULATION)

    def setUp (self):
        self.m_Pump = DrdPump (DrdPumpOK.m_Card, configData)
        
    def testCreation(self):
        """Create DrdPump"""
        self.assertEqual (self.m_Pump.m_Card, self.m_Card)

    def testValveSetting(self):
        """Check valve setting """
        self.assertEqual (self.m_Pump._ValvePosition(), None)
        self.m_Pump._SetValvePosition (1)
        self.assertEqual (self.m_Pump._ValvePosition(), 1)
        self.m_Pump._SetValvePosition (2)
        self.assertEqual (self.m_Pump._ValvePosition(), 2)
        self.assertRaises(DrdPumpError, self.m_Pump._SetValvePosition, 0)
        self.assertRaises(DrdPumpError, self.m_Pump._SetValvePosition, 3)
        
    def testHoming(self):
        """Check homing """
        self.assertRaises (DrdPumpError, self.m_Pump.Home)
        self.m_Pump._SetValvePosition (1)
        self.m_Pump.Home()
        
    def testDualPistonModeSetting(self):
        """Check dual Piston Mode """
        self.assertRaises (DrdPumpError, self.m_Pump.UseDualPistonMode)
        self.m_Pump._SetValvePosition (1)
        self.assertRaises (AxisError, self.m_Pump.UseDualPistonMode)
        self.m_Pump.Home()
        self.m_Pump.UseDualPistonMode()
        self.assertEqual (self.m_Pump.IsUsingDualPistonMode(), True)
        self.assertEqual (self.m_Pump.IsUsingSinglePistonMode(), False)
        
    def testSinglePistonModeSetting(self):
        """Check single Piston Mode """
        self.assertRaises (DrdPumpError, self.m_Pump.UseSinglePistonMode)
        self.m_Pump._SetValvePosition (1)
        self.assertRaises (AxisError, self.m_Pump.UseSinglePistonMode)
        self.m_Pump.Home()
        self.m_Pump.UseSinglePistonMode()
        self.assertEqual (self.m_Pump.IsUsingDualPistonMode(), False)
        self.assertEqual (self.m_Pump.IsUsingSinglePistonMode(), True)
        
    def testDualPistonModeVolume(self):
        """Check volume of dual Piston Mode """
        self.m_Pump._SetValvePosition (1)
        self.m_Pump.Home()
        self.m_Pump.UseDualPistonMode()
        self.assertAlmostEqual (self.m_Pump.Volume(), 0.00, 2)
        
        testVolume = 100.00
        self.m_Pump._SetVolume(testVolume)
        self.assertAlmostEqual (self.m_Pump.Volume(), testVolume, 2)
        
        overVolume = self.m_Pump.MaxVolume()+ 0.2
        self.assertRaises (DrdPumpError, self.m_Pump._SetVolume, overVolume)
        
    def testSinglePistonModeVolume(self):
        """Check volume of single Piston Mode """
        self.m_Pump._SetValvePosition (1)
        self.m_Pump.Home()
        self.m_Pump.UseSinglePistonMode()
        self.assertAlmostEqual (self.m_Pump.Volume(), 0.00, 2)
        
        testVolume = 1000.00
        self.m_Pump._SetVolume(testVolume)
        self.assertAlmostEqual (self.m_Pump.Volume(), testVolume, 2)
        
        overVolume = self.m_Pump.MaxVolume()+ 2.0
        self.assertRaises (DrdPumpError, self.m_Pump._SetVolume, overVolume)
        

#--------------------------------------------------------------------------
    
#--------------------------------------------------------------------
#    Invoke test:
#
if __name__ == '__main__':
    unittest.main()
            
# EOF
