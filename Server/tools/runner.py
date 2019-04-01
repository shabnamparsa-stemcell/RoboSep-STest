# 
# runner.py
#
# Unit test runner for the tesla.types unit tests
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

import os, sys, re
import unittest
from ipl.utils.path import path


if __name__ == '__main__':
    testPath = path(os.getcwd())
    sys.path.append(testPath)
    testFiles = [os.path.basename(x) for x in testPath.files('Test*.py')]
    for fileName in testFiles:
        fileName = os.path.basename(fileName)
        moduleName = fileName[:-3]
        importCmd = "from %s import *" % (moduleName,)
        print "Importing unit tests from %s" % (moduleName)
        exec(importCmd)
        
    unittest.main()

# eof
