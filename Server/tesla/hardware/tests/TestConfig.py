# 
# TestConfig.py
#
# Unit tests for the hardware configuration class
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
from tesla.hardware.config import HardwareConfiguration

class TestConfig(unittest.TestCase):
  
    def setUp(self):
        self.hc = HardwareConfiguration('hw_config.dat')
        
    def testSections(self):
        sections = self.hc.cfg.sections()
        self.failUnless('theta' in sections, 'Testing sections')
        self.failUnless(self.hc.cfg.has_section('z'), "Has a 'z' section")
    
    def testDefaults(self):
        # At the moment, the h/w config should have no defaults?
        defaults = self.hc.cfg.defaults()
        self.failUnless(defaults == {}, 'Empty defaults test')
  
    def testValues(self):
        options = self.hc.cfg.options('z')
        self.failUnless('home' in options, 'Testing options')
        self.failUnless(self.hc.cfg.has_option('carousel', 'power'), 'Has a power option')

if __name__ == '__main__':
    unittest.main()

# eof
