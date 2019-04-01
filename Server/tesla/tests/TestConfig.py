# 
# TestConfig.py
# Unit tests for the tesla.config module
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

# Don't refactor these two lines, there is a reason for the double import
# Have a look at TestConfig.test_protocolTypeID()
import tesla.config
from tesla.config import *

# ---------------------------------------------------------------------------

class TestConfig(unittest.TestCase):
   
    def test_platform(self):
	self.failUnless(PLATFORM, 'platform test')
    
    def test_gateway(self):
	self.failUnless(GATEWAY_HOST, 'Gateway host test')
	self.failUnless(type(GATEWAY_HOST) == type(''), 'Test gateway string')
	self.failUnless(type(GATEWAY_PORT) == type(80), 'Test gateway port')
	self.failUnless(GATEWAY_PORT > 0, 'Gateway port validity test')
	
    def test_diskSpace(self):
	self.failUnless(MIN_SAFE_DISK_SPACE > 0, 'Min. disk space test')
    
    def test_logLevel(self):
	self.failUnless(LOG_LEVEL, 'log level test')

    def __dirTest(self, dir):
	'''Returns true if the directory exists'''
	return os.path.isdir(dir)

    def test_directories(self):
	for dir in [BASE_DIR, PROTOCOL_DIR, LOG_DIR, SAMPLE_REPORT_DIR]:
	    self.failUnless(self.__dirTest(dir), "Testing %s directory" % (dir))

    def test_quadrants(self):
	self.failUnless(type(NUM_QUADRANTS) == type(1), 'Testing quadrant type')
	self.failUnless(NUM_QUADRANTS > 0, 'Testing valid quadrants limit')

    def test_protocolTypeID(self):
	protocols = []
	for entry in dir(tesla.config):
            value = eval("tesla.config.%s" % (entry))
            if entry.find('_PROTOCOL') > 0:
		protocols.append(value)
		self.failUnless(type(value) == type(''), "Testing type of %s" % (value))
	# We should have 5 types of protocol defined: positive separation, 
        # negative separation, shutdown, testing and maintenance
	self.failUnless(len(protocols) == 5, 'Testing number of protocol types')
    
   
# ---------------------------------------------------------------------------

if __name__ == '__main__':
    unittest.main()

# eof
