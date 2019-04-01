# 
# TestInstrument.py
#
# Unit tests for tesla.instrument.Instrument
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

import tesla.config
from tesla.instrument.Instrument import Instrument
from tesla.types.tube import Tube


# -----------------------------------------------------------------------------

class TestInstrument(unittest.TestCase):
   
    def setUp(self):
	self.inst = Instrument('Instrument')
        self.numQuadrants = tesla.config.NUM_QUADRANTS

    
    def test_getBulkBufferDeadVolume(self):
        deadVolume = self.inst.getBulkBufferDeadVolume()
        self.failUnless(deadVolume >= 0, 'Testing bulk buffer dead volume')

    def test_bulkBottle(self):
        bottle = self.inst.BulkBottle()
        self.failUnless(bottle, 'Testing that we have a bottle')
        self.failUnless(isinstance(bottle, Tube), 'Testing bulk bottle is a Tube')
       
    def test_getContainers(self):
        containers = self.inst.getContainers()
        self.failUnless(isinstance(containers, dict), 'Testing container map')
        quadrantRange = range(0, self.numQuadrants + 1)
        for quadrantNumber in containers.keys():
            self.failUnless(quadrantNumber in quadrantRange, \
                    "Testing quadrant number %d for containers" % (quadrantNumber))
                


# -----------------------------------------------------------------------------
 
if __name__ == '__main__':
    unittest.main()

# eof
