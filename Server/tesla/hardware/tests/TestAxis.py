# 
# TestAxis.py
#
# Unit tests for the base Axis class
# 
# Copyright (c) Invetech Pty Ltd, 2003
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
from tesla.hardware.Axis import Axis 

# bdr BOARD_ADDRESS = 'D1'

BOARD_ADDRESS = 'X1'

class TestDevice(unittest.TestCase):

    def setUp(self):
        self.axis = Axis(BOARD_ADDRESS, emulation = True)

    def test_name(self):
        self.assertTrue(self.axis.name, 'Name is defined')

    def test_address(self):
        self.assertTrue(self.axis.boardAddress == BOARD_ADDRESS, 'Board address')

unittest.main()

# eof
