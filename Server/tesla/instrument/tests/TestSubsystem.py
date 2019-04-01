# 
# TestSubsystem.py
# tesla.instrument.Subsystem
#
# Unit tests for the tesla.controller module
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
from tesla.instrument.Subsystem import Subsystem

# ---------------------------------------------------------------------------

# Test the Subsystem class
class TestSubsystem(unittest.TestCase):
   
    ssName = 'SS1'
    ssValue = 7
    class mySubsystem(Subsystem):
        def __init__(self,name,count):
            Subsystem.__init__(self, name)
            self._count = count
        def getCount(self):
            return self._count

    def setUp(self):
	self.subsys = TestSubsystem.mySubsystem(
                        TestSubsystem.ssName,
                        TestSubsystem.ssValue)

    def test_subsystem(self):
	self.failUnless(self.subsys)
    
    def test_execute(self):
        rv=0
        for obj in Subsystem.instanceList:
            if "Subsystem: %s" % TestSubsystem.ssName == "%s" % obj:
                rv += obj.execute("self.getCount()")
        self.failUnless(rv == TestSubsystem.ssValue)

# ---------------------------------------------------------------------------

if __name__ == '__main__':
    unittest.main()

# eof
