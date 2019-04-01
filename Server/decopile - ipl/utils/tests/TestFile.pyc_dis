# File: T (Python 2.3)

import unittest
from ipl.utils.file import *

class TestFile(unittest.TestCase):
    
    def setUp(self):
        self.target = '.\\TestFile.py'

    
    def test_getFileList(self):
        files = getFileList('.')
        self.failUnless(self.target in files, 'Test getFileList()')

    
    def test_getFileListWithExtension(self):
        fileList1 = getFileList('.', 'py')
        fileList2 = getFileList('.', 'F00')
        self.failUnless(self.target in fileList1, 'Test getFileList() with extension mask')
        self.failIf(self.target in fileList2, 'Test getFileList() with silly extension')

    
    def test_findMatchingDirs(self):
        dirs = findMatchingDirs('system32', 'c:\\winnt')
        self.failUnless(type(dirs) == type([]), 'findMatchingDirs: test return type')
        self.failUnless(len(dirs) > 0, 'findMatchingDirs: have directories')

    
    def test_findMatchingFiles(self):
        files = findMatchingFiles('co.*\\.sys', 'c:\\')
        self.failUnless(type(files) == type([]), 'findMatchingFiles: test return type')
        self.failUnless(len(files) == 1, 'findMatchingFiles: have file')
        self.failUnless(files[0].lower().find('config.sys') >= 0, 'findMatchingFiles: right file')


if __name__ == '__main__':
    unittest.main()

