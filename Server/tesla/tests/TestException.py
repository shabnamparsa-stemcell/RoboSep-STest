# 
# TestException.py
#
# Unit tests for the tesla.exception module
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
from tesla.exception import TeslaException

class TestException(unittest.TestCase):
  
    def raiseMe(self):
        raise TeslaException('Test exception')
    
    def test_raise(self):
        self.assertRaises(TeslaException, self.raiseMe)
        
if __name__ == '__main__':
    unittest.main()

# eof
