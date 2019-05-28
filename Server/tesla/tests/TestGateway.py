# 
# TestGateway.py
#
# Unit tests for the Tesla XML-RPC gateway
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

from tesla.config import GATEWAY_HOST, GATEWAY_PORT
from tesla.gateway import Gateway
from tesla.control.Centre import Centre

# ---------------------------------------------------------------------------

class TestGateway(unittest.TestCase):
 
    def setUp(self):
        controller = Centre()
        self.gw = Gateway(controller)        

    def tearDown(self):
        self.gw.close()

    def testRunFlag(self):
        self.failIf(self.gw.isRunning(), 'Gateway should not be running')

    def testHalt(self):
        # No test yet -- we really need to set up an XML-RPC client to test 
        # halt properly
        status = self.gw.halt()
        self.failUnless(status == True, 'Testing halt status')
        self.failIf(self.gw.isRunning(), 'Testing gateway run state')



# -----------------------------------------------------------------------------

if __name__ == '__main__':
    unittest.main()

# eof
