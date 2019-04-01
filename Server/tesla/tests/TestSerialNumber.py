# 
# TestSerialNum.py
# Unit tests for the instrument serial number class
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

from tesla.serialnum import InstrumentSerialNumber
from tesla.exception import TeslaException

# ---------------------------------------------------------------------------

class TestSerialNum(unittest.TestCase):
  
    def setUp(self):
        self.serial = InstrumentSerialNumber()
    
    def test_serial(self):
        # Verify that we can read a serial number from the default file
        self.failUnless(self.serial, 'Serial number is defined')
        self.failUnless(repr(self.serial), 'Serial number representation test')

    def test_missingSerialNumFile(self):
        # Try to read a serial number from a non-existent file
        InstrumentSerialNumber.reset()
        self.assertRaises(TeslaException, InstrumentSerialNumber, 'foo')

    def test_badSerialNumFile(self):
        # Try to read a serial number from a file that doesn't contain a 
        # serial number
        InstrumentSerialNumber.reset()
        self.assertRaises(TeslaException, InstrumentSerialNumber, 'c:\\winnt\\welcome.exe')

# -----------------------------------------------------------------------------

if __name__ == '__main__':
    unittest.main()

# eof
