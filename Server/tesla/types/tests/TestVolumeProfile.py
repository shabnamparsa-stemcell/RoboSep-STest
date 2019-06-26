# 
# TestVolumeProfile.py
#
# Unit tests for the tesla.types.VolumeProfile module
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
# ---------------------------------------------------------------------------

import unittest
from tesla.types.VolumeProfile import *
from tesla.exception import TeslaException

#----------------------------------------------------------------------------
class TestDeadProfile(unittest.TestCase):
    """Unit tests for dead volume profile"""
    
    #------------------------------------------------------------------------
    def setUp(self):
        self.volume = 2000
        self.profile = DeadVolumeProfile (self.volume)
    
    #------------------------------------------------------------------------
    def testHeightOf(self):
        self.assertTrue (self.profile.HeightOf (0) == 0, "Test: height always zero.")
        self.assertTrue (self.profile.HeightOf (self.volume/2) == 0, "Test: height always zero.")
        self.assertTrue (self.profile.HeightOf (self.volume) == 0, "Test: height always zero.")
    
    #------------------------------------------------------------------------
    def testExcessCapacity(self):
        self.assertRaises (VolumeProfileError, self.profile.HeightOf, self.volume+1)
    
    #------------------------------------------------------------------------
    def testFullHeight(self):
        self.assertTrue (self.profile.FullHeight () == 0, "Test: FullHeight is zero.")
    
    #------------------------------------------------------------------------
    def testFullVolume(self):
        self.assertTrue (self.profile.FullVolume () == self.volume, "Test: FullVolume is specified profile volume.")
    
#----------------------------------------------------------------------------
class TestCylindricalProfile(unittest.TestCase):
    """Unit tests for cylindrical volume"""
    
    #------------------------------------------------------------------------
    def setUp(self):
        self.volume = 2000
        self.height = 100
        self.profile = CylindricalProfile (self.height, self.volume)
    
    #------------------------------------------------------------------------
    def testHeightOf(self):
        self.assertAlmostEqual (self.profile.HeightOf (0), 0, 3, "Test: h(0) = 0.")
        self.assertAlmostEqual (self.profile.HeightOf (self.volume), self.height, 3, "Test: h(V) = H => %f = %f." % (self.profile.HeightOf (self.volume), self.height))
        self.assertAlmostEqual (self.profile.HeightOf (self.volume/2), self.height/2, 3, "Test: interpolation - h(V/2) = H/2.")
    
    #------------------------------------------------------------------------
    def testExcessCapacity(self):
        self.assertRaises (VolumeProfileError, self.profile.HeightOf, self.volume+1)
    
    #------------------------------------------------------------------------
    def testFullHeight(self):
        self.assertTrue (self.profile.FullHeight () == self.height, "Test: FullHeight is specified profile height.")
    
    #------------------------------------------------------------------------
    def testFullVolume(self):
        self.assertTrue (self.profile.FullVolume () == self.volume, "Test: FullVolume is specified profile volume.")
    
#----------------------------------------------------------------------------
class TestConicalProfile(unittest.TestCase):
    """Unit tests for conical profile"""
    
    #------------------------------------------------------------------------
    def setUp(self):
        self.volume = 2000.0
        self.height = 100.0
        self.profile = ConicalProfile (self.height, self.volume)
        self.iProfile = ConicalProfile (self.height, self.volume, False)
    
    #------------------------------------------------------------------------
    def testHeightOf(self):
        self.assertAlmostEqual (self.profile.HeightOf (0), 0, 3, "Test: h(0) = 0.")
        self.assertAlmostEqual (self.profile.HeightOf (self.volume), self.height, 3, "Test: h(V) = H => %f = %f." % (self.profile.HeightOf (self.volume), self.height))
        self.assertAlmostEqual (self.profile.HeightOf (self.volume/8.0), self.height/2.0, 3, "Test: interpolation - h(V/8) = H/2.")
    
        self.assertAlmostEqual (self.iProfile.HeightOf (0), 0, 3, "Test: h(0) = 0.")
        self.assertAlmostEqual (self.iProfile.HeightOf (self.volume), self.height, 2, "Test: h(V) = H => %f = %f." % (self.profile.HeightOf (self.volume), self.height))
        self.assertAlmostEqual (self.iProfile.HeightOf (7.0*self.volume/8.0), self.height/2.0, 3, "Test: interpolation - h(7V/8) = H/2.")
    
    #------------------------------------------------------------------------
    def testExcessCapacity(self):
        self.assertRaises (VolumeProfileError, self.profile.HeightOf, self.volume+1)
    
    #------------------------------------------------------------------------
    def testFullHeight(self):
        self.assertTrue (self.profile.FullHeight () == self.height, "Test: FullHeight is specified profile height.")
    
    #------------------------------------------------------------------------
    def testFullVolume(self):
        self.assertTrue (self.profile.FullVolume () == self.volume, "Test: FullVolume is specified profile volume.")
        self.assertTrue (self.iProfile.FullVolume () == self.volume, "Test: FullVolume is specified profile volume.")
    
#----------------------------------------------------------------------------
class TestTruncatedConicalProfile(unittest.TestCase):
    """Unit tests for truncated conical profile"""
    
    #------------------------------------------------------------------------
    def setUp(self):
        # Use this as a comparison:
        #
        self.stdVolume = 2000.0
        self.stdHeight = 100.0
        self.stdProfile = ConicalProfile (self.stdHeight, self.stdVolume)
        self.istdProfile = ConicalProfile (self.stdHeight, self.stdVolume, False)

        # Construct a truncated cone at half height
        #
        self.tipvolume = self.stdVolume/8.0
        self.tipheight = self.stdProfile.HeightOf(self.tipvolume)
        self.tipProfile = ConicalProfile (self.tipheight, self.tipvolume)

        self.height = self.stdHeight-self.tipheight
        self.volume = self.stdVolume-self.tipvolume
        self.profile = TruncatedConicalProfile (self.height, self.volume, self.tipProfile)
        self.iProfile = TruncatedConicalProfile (self.height, self.volume, self.tipProfile, False)
    
    #------------------------------------------------------------------------
    def testHeightOf(self):
        self.assertAlmostEqual (self.profile.HeightOf (0), 0, 3, "Test: h(0) = 0.")
        self.assertAlmostEqual (self.profile.HeightOf (self.volume), self.height, 3, "Test: h(V) = H => %f = %f." % (self.profile.HeightOf (self.volume), self.height))
        testVolume = self.volume /2.0
        testHeight = self.stdProfile.HeightOf(testVolume + self.tipvolume)-self.tipheight
        self.assertAlmostEqual (self.profile.HeightOf (testVolume), testHeight, 3, "Test: interpolation - h(V/8) = H/2.")
    
        self.assertAlmostEqual (self.iProfile.HeightOf (0), 0, 3, "Test: h(0) = 0.")
        self.assertAlmostEqual (self.iProfile.HeightOf (self.volume), self.height, 2, "Test: h(V) = H => %f = %f." % (self.profile.HeightOf (self.volume), self.height))
        testVolume = self.volume /2.0
        testHeight = self.istdProfile.HeightOf(testVolume)
        self.assertAlmostEqual (self.iProfile.HeightOf (testVolume), testHeight, 3, "Test: interpolation - h(7V/8) = H/2.")
    
    #------------------------------------------------------------------------
    def testExcessCapacity(self):
        self.assertRaises (VolumeProfileError, self.profile.HeightOf, self.volume+1)
    
    #------------------------------------------------------------------------
    def testFullHeight(self):
        self.assertTrue (self.profile.FullHeight () == self.height, "Test: FullHeight is specified profile height.")
    
    #------------------------------------------------------------------------
    def testFullVolume(self):
        self.assertTrue (self.profile.FullVolume () == self.volume, "Test: FullVolume is specified profile volume.")
        self.assertTrue (self.iProfile.FullVolume () == self.volume, "Test: FullVolume is specified profile volume.")
    
#----------------------------------------------------------------------------
class TestHemisphericalProfile(unittest.TestCase):
    """Unit tests for hemispherical profile
        (TBD: hemisphere is currently treated as a cone!)"""
    
    #------------------------------------------------------------------------
    def setUp(self):
        self.volume = 2000.0
        self.profile = HemisphericalProfile (self.volume)
        self.height = self.profile.FullHeight()
    
    #------------------------------------------------------------------------
    def testHeightOf(self):
        self.assertAlmostEqual (self.profile.HeightOf (0), 0, 2, "Test: h(0) = 0.")
        self.assertAlmostEqual (self.profile.HeightOf (self.volume), self.height, 2, "Test: h(V) = H => %f = %f." % (self.profile.HeightOf (self.volume), self.height))
        halfHeightVolume = self.profile._VolumeOf(self.height/2.0)
        self.assertAlmostEqual (self.profile.HeightOf (halfHeightVolume), self.height/2.0, 2, "Test: interpolation .")
    
    #------------------------------------------------------------------------
    def testExcessCapacity(self):
        self.assertRaises (VolumeProfileError, self.profile.HeightOf, self.volume+1)
    
    #------------------------------------------------------------------------
    def testFullHeight(self):
        self.assertTrue (self.profile.FullHeight () == self.height, "Test: FullHeight is specified profile height.")
    
    #------------------------------------------------------------------------
    def testFullVolume(self):
        self.assertTrue (self.profile.FullVolume () == self.volume, "Test: FullVolume is specified profile volume.")
    
    
if __name__ == '__main__':
    unittest.main()

# eof
