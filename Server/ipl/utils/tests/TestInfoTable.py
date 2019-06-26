# File: T (Python 2.3)

import unittest
from ipl.utils.InfoTable import InfoTable

class TestInfoTable(unittest.TestCase):
    
    def setUp(self):
        self.statusTable = InfoTable('test_table.txt')

    
    def test_getCode(self):
        msg = 'January'
        code = self.statusTable.getCode(msg)
        self.assertTrue(code == 'MONTH1', 'getCode() test')

    
    def test_spaces(self):
        msg = 'April'
        self.assertTrue(self.statusTable.getCode(msg) == 'MONTH4', 'getCode() test')

    
    def test_getMessage(self):
        self.assertTrue(self.statusTable.getMessage('MONTH1') == 'January', 'getMessage() test')

    
    def test_getBadMessage(self):
        self.assertRaises(KeyError, self.statusTable.getMessage, 'BOGUS')

    
    def test_emptyCode(self):
        code = self.statusTable.getCode('Nothing')
        self.assertTrue(code == '', 'Testing empty code')

    
    def test_getBadCode(self):
        self.assertRaises(LookupError, self.statusTable.getCode, 'I w0u1d BE surprIZed IF this wAs A legit msg!')

    
    def test_getCodes(self):
        codes = self.statusTable.getCodes()
        self.assertTrue(type(codes) == type([]), 'Testing codes type')
        self.assertTrue(len(codes) > 0, 'Do we have codes?')

    
    def test_bogusTable(self):
        SILLY_PATH = 'imaginary_table.foo'
        self.assertRaises(IOError, InfoTable, SILLY_PATH)


if __name__ == '__main__':
    unittest.main()

