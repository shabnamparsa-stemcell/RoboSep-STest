# 
# TestTube.py
#
# Unit tests for the tesla.types.tube module
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
from tesla.types.tube import Tube, TubeException
from tesla.types.VolumeProfile import CylindricalProfile
from tesla.types.TubeGeometry import TubeGeometry
from tesla.exception import TeslaException

# ---------------------------------------------------------------------------

class TestTube(unittest.TestCase):
  
    def setUp(self):
        self.startVol = 42
        self.maxVol = 5000
        self.deadVol = 83
        self.sector = 1
        self.ID = '5 mL tube'
        self.tubeBase = 50
        self.tubeHeight = 50
        self.geometry = TubeGeometry(self.tubeBase, self.deadVol, 
                            (CylindricalProfile (self.tubeHeight, self.maxVol), ))
        
        self.tube = Tube(1, self.ID, self.geometry, self.startVol)
    
    def test_getLabel(self):
        self.assertTrue(self.tube.getLabel() == self.ID, 'Label test')

    def test_startVolume(self):
        self.assertTrue(self.tube.getVolume() == self.startVol, 'Initial volume test')

    def test_maxVolume(self):
        self.assertTrue(self.tube.getMaxVolume() == self.maxVol, 'Maximum volume test')

    def test_deadVolume(self):
        self.assertTrue(self.tube.getDeadVolume() == self.deadVol, 'Dead volume test')

    def test_addVolume(self):
        addedVolume = 1000
        self.tube.addVolume(addedVolume)
        newVol = self.startVol + addedVolume
        self.assertTrue(self.tube.getVolume() == newVol, 'Add volume test')

    def test_addOverVolume(self):
        # Test adding too much volume to the tube
        overVolume = self.tube.getMaxVolume() * 2
        self.assertRaises(TubeException, self.tube.addVolume, overVolume)

    def test_removeVolume(self):
        startVolume = self.tube.getVolume()
        addedVolume = 1234
        removedVolume = 1000
        self.tube.addVolume(addedVolume)
        self.tube.removeVolume(removedVolume)
        newVolume = startVolume + addedVolume - removedVolume
        self.assertTrue(self.tube.getVolume() == newVolume, 'Remove volume test')

    def test_removeOverVolume(self):
        testVolume = 1000
        # Assume we start at zero volume
        self.tube.addVolume(testVolume)
        self.tube.removeVolume(testVolume * 2)
        # We should end up with zero volume (we've aspirated everything)
        self.assertTrue(self.tube.getVolume() == 0, 'Remove too much volume')

    def test_meniscus(self):
        # Test meniscus height
        self.tube.removeVolume(self.tube.getVolume())
        self.tube.addVolume(self.tube.getMaxVolume()/2.0)
        self.assertTrue(self.tube.getMeniscus() == self.tubeHeight/2.0)

    def test_meniscusWhenAdding(self):
        # Test meniscus height check on adding
        self.tube.removeVolume(self.tube.getVolume())
        self.assertTrue(self.tube.getMeniscusAfterAdding(self.tube.getMaxVolume()) == self.tubeHeight)

    def test_meniscusWhenRemoving(self):
        # Test meniscus height check on removing
        self.tube.removeVolume(self.tube.getVolume())
        self.tube.addVolume(self.tube.getMaxVolume())
        self.assertTrue(self.tube.getMeniscusAfterRemoving(self.tube.getMaxVolume()) == 0.0)

    def test_basePosition(self):
        # Test base position
        self.assertTrue(self.tube.getBasePosition() == self.tubeBase)

    def test_tubeHeight(self):
        # Test tubeHeight
        self.assertTrue(self.tube.getMaxHeight() == self.tubeHeight)

    def test_empty(self):
        startVol = 432
        self.tube = Tube(1, self.ID, self.geometry, startVol)
        self.assertTrue(self.tube.getVolume() == startVol, 'Testing starting volume')
        self.tube.empty()
        self.assertTrue(self.tube.getVolume() == 0, 'Testing empty tube')

    def test_setVolume(self):
        startVol = 666
        self.tube = Tube(1, self.ID, self.geometry, startVol)
        for volume in (10, 100, 1000, self.maxVol):
            self.tube.setVolume(volume)
            self.assertTrue(self.tube.getVolume() == volume, "New set volume = %0.1f" % (volume))
    
    def test_bad_setVolume(self):
        startVol = 1234
        tube = Tube(1, self.ID, self.geometry, startVol)
        for volume in (-1, self.maxVol + 1):
            self.assertRaises(TubeException, tube.setVolume, volume)

    def test_str(self):
        # Test the string representation
        startVol = 2500; quad = 1
        tube = Tube(quad, self.ID, self.geometry, startVol)
        s = str(tube)
        self.assertTrue(s == "5 mL tube [Q%d]: %d.0 of 5000.0 uL" % (quad, startVol), 'Testing str()')

# -----------------------------------------------------------------------------

if __name__ == '__main__':
    unittest.main()

# eof
