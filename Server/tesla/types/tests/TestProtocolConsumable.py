# 
# TestProtocolConsumable.py
#
# Unit tests for the tesla.types.ProtocolConsumable module
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
from tesla.exception import TeslaException
from tesla.types.ProtocolConsumable import ProtocolConsumable
from tesla.types.ProtocolConsumable import NOT_NEEDED, EMPTY
from tesla.config import NUM_QUADRANTS

# ---------------------------------------------------------------------------

class TestProtocolConsumable(unittest.TestCase):
  
    def setUp(self):
        self.quadrant = 1
        self.cocktailVol = 10
        self.particleVol = 12.5
        self.lysisVol = 3450.0
        self.antibodyVol = 175.25
        self.bulkBufferVolume = 30000

        self.pc = ProtocolConsumable(self.quadrant, self.cocktailVol, 
                                        self.particleVol, self.lysisVol,
                                        self.antibodyVol, self.bulkBufferVolume,
                                        True, False, True, False)
    
    def test_members(self):
        self.failUnless(self.pc.quadrant == self.quadrant, 
                            'Testing quadrant member')
        self.failUnless(self.pc.cocktailVolume == self.cocktailVol, 
                            'Testing cocktail volume')
        self.failUnless(self.pc.particleVolume == self.particleVol, 
                            'Testing particle volume')
        self.failUnless(self.pc.lysisVolume == self.lysisVol,
                            'Testing lysis volume')
        self.failUnless(self.pc.antibodyVolume == self.antibodyVol,
                            'Testing antibody volume')
        self.failUnless(self.pc.bulkBufferVolume == self.bulkBufferVolume,
                            'Testing bulk buffer volume')
        self.failUnless(self.pc.wasteVesselRequired == True, 
                            'Testing waste vessel required')
        self.failUnless(self.pc.separationVesselRequired == False, 
                            'Testing seraration vessel required')
        self.failUnless(self.pc.sampleVesselRequired == True, 
                            'Testing sample vessel required')
        self.failUnless(self.pc.tipBoxRequired == False, 
                            'Testing tip box required')
    
    def test_emptyOrNotNeeded(self):
        pc1 = ProtocolConsumable(2, NOT_NEEDED, EMPTY, NOT_NEEDED, EMPTY)
        self.failUnless(pc1)
                
    def test_validQuadrant(self):
        con1 = ProtocolConsumable(1, EMPTY, EMPTY, EMPTY, EMPTY)
        self.failUnless(con1)
        con4 = ProtocolConsumable(NUM_QUADRANTS, EMPTY, EMPTY, EMPTY, EMPTY)
        self.failUnless(con4)        

    def test_invalidQuadrants(self):        
        self.assertRaises(TeslaException, ProtocolConsumable,
                0, 1000.0, 100.0, NOT_NEEDED, NOT_NEEDED)
        self.assertRaises(TeslaException, ProtocolConsumable,
                NUM_QUADRANTS + 1, 1000.0, 100.0, NOT_NEEDED, NOT_NEEDED)

    def test_getPrettyString(self):
        pretty = self.pc.getPrettyString()
        self.failUnless(type(pretty) == str, 'Testing pretty string type')
        self.failUnless(len(pretty) > 0)

# -----------------------------------------------------------------------------
        
if __name__ == '__main__':
    unittest.main()

# eof
