# File: r (Python 2.3)

import os
import unittest
from ipl.utils.file import *
UNITTEST_RE = 'Test[A-z_0-9]*.py$'

def regressionTest():
    files = findMatchingFiles(UNITTEST_RE, '.')
    if len(files):
        
        filenameToModuleName = lambda f: str(f[2:-3])
        moduleNames = list(map(filenameToModuleName, files))
        modules = list(map(__import__, moduleNames))
        load = unittest.defaultTestLoader.loadTestsFromModule
        return unittest.TestSuite(list(map(load, modules)))
    

if __name__ == '__main__':
    unittest.main(defaultTest = 'regressionTest')

