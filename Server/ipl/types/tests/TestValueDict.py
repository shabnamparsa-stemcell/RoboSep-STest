# File: T (Python 2.3)

import unittest
from ipl.types.ValueDict import ValueDict

class TestValueDict(unittest.TestCase):
    
    def setUp(self):
        self.dict = ValueDict({
            'Graeme': 'Male',
            'Robert': 'Male',
            'Diane': 'Female',
            'Nathan': 'Male',
            'Tara': 'Female',
            'Alyssa': 'Female',
            'Raeleen': 'Female' })

    
    def test_getFirstKey(self):
        self.assertTrue(self.dict.getFirstKey('Male') == 'Graeme', 'Testing getFirstKey()')

    
    def test_getFirstKey_forNoValue(self):
        self.assertTrue(self.dict.getFirstKey('Neuter') == None, 'Testing no value')

    
    def test_getKeys(self):
        expected = [
            'Diane',
            'Tara',
            'Alyssa',
            'Raeleen']
        found = self.dict.getKeys('Female')
        expected.sort()
        self.assertTrue(found == expected, 'Testing keys that match our value')

    
    def test_getKeys_forNoValue(self):
        self.assertTrue(self.dict.getKeys('Neuter') == [], 'Testing no values')


if __name__ == '__main__':
    unittest.main()

