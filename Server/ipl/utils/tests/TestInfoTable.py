# File: T (Python 2.3)

import unittest
from ipl.utils.InfoTable import InfoTable

class TestInfoTable(unittest.TestCase):
    
    def setUp(self):
        self.statusTable = InfoTable('test_table.txt')

    
    def test_getCode(self):
        msg = 'January'
        code = self.statusTable.getCode(msg)
        self.failUnless(code == 'MONTH1', 'getCode() test')

    
    def test_spaces(self):
        msg = 'April'
        self.failUnless(self.statusTable.getCode(msg) == 'MONTH4', 'getCode() test')

    
    def test_getMessage(self):
        self.failUnless(self.statusTable.getMessage('MONTH1') == 'January', 'getMessage() test')

    
    def test_getBadMessage(self):
        self.assertRaises(KeyError, self.statusTable.getMessage, 'BOGUS')

    
    def test_emptyCode(self):
        code = self.statusTable.getCode('Nothing')
        self.failUnless(code == '', 'Testing empty code')

    
    def test_getBadCode(self):
        self.assertRaises(LookupError, self.statusTable.getCode, 'I w0u1d BE surprIZed IF this wAs A legit msg!')

    
    def test_getCodes(self):
        codes = self.statusTable.getCodes()
        self.failUnless(type(codes) == type([]), 'Testing codes type')
        self.failUnless(len(codes) > 0, 'Do we have codes?')

    
    def test_bogusTable(self):
        SILLY_PATH = 'imaginary_table.foo'
        self.assertRaises(IOError, InfoTable, SILLY_PATH)


if __name__ == '__main__':
    unittest.main()

