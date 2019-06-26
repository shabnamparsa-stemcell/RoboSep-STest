# File: T (Python 2.3)

import unittest
from ipl.utils.validation import *

class TestValidation(unittest.TestCase):
    
    def test_swap(self):
        self.assertTrue(swap(1, 2) == (2, 1), 'Testing swapping two numbers')

    
    def test_validateNumber(self):
        self.assertTrue(validateNumber(10, 9, 11), 'Testing valid number and limits')

    
    def test_validateWithBadRange(self):
        self.assertTrue(validateNumber(42, 67, 13), 'Testing valid number with bad range')

    
    def test_validateBadNumber(self):
        self.assertFalse(validateNumber(99, 42, 67), 'Testing invalid number')

    
    def test_validateRealNumber(self):
        self.assertTrue(validateNumber(3.1415000000000002, 3.0, 3.2000000000000002), 'Testing floating point number')


if __name__ == '__main__':
    unittest.main()

