# 
# TestSample.py
#
# Unit tests for the tesla.types.sample module
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
from tesla.types.sample import Sample
from tesla.exception import TeslaException

class TestSample(unittest.TestCase):
  
    def setUp(self):
        self.protocolID = 1234
        self.volume = 5000
        self.sampleID = 0xd00d
        self.label = 'Sample 16'
        self.quadrant = 3
        self.sample = Sample(self.sampleID, self.label, self.protocolID, self.volume, \
                self.quadrant)
   
    def test_str(self):
        self.assertTrue(str(self.sample) == "ID = 53261 ('Sample 16'). Protocol ID = 1234. Vol = 5000.00 uL", 'Testing Sample.__str__()')
        
    def test_protocolID(self):
        self.assertTrue(self.sample.protocolID == self.protocolID, 'Protocol ID')

    def test_volume(self):
        # There are three ways to get the volume (for historical purposes)
        self.assertTrue(self.sample.volume == self.volume, 'Get sample volume #1')
        self.assertTrue(self.sample.volume_uL == self.volume, 'Get sample volume #2')

    def test_sampleID(self):
        self.assertTrue(self.sample.ID == self.sampleID, 'Sample ID')

    def test_quadrant(self):
        self.assertTrue(self.sample.initialQuadrant == self.quadrant, 'Initial quadrant')

    def test_invalidQuadrants(self):
        for quad in [0, 5, 42]:
            self.assertRaises(TeslaException, Sample, 1, 'Bad quad', 42, 1000, quad)

    def test_invalidVolumes(self):
        # Not a numeric type for the sample volume
        self.assertRaises(TeslaException, Sample, 1, 'Bad 1', 42, 'blah', 1)
        # A negative volume
        self.assertRaises(TeslaException, Sample, 2, 'Bad 2', 42, -1, 1)
        # A volume that is way too high
        self.assertRaises(TeslaException, Sample, 3, 'Bad 3', 42, 65000, 1)

# -----------------------------------------------------------------------------

if __name__ == '__main__':        
    unittest.main()

# eof
