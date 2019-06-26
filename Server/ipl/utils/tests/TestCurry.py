# File: T (Python 2.3)

import unittest
from ipl.utils.curry import curry

def add(x, y, z = 0):
    return x + y + z


class TestCurry(unittest.TestCase):
    
    def test_noCurry(self):
        self.assertTrue(add(1, 2, 3) == 6, 'Testing the add function')

    
    def test_curry1Param(self):
        add1 = curry(add, 1)
        self.assertTrue(add1(4, 5) == 10, 'Testing 1-parameter currying')

    
    def test_curry2Params(self):
        add2 = curry(add, 5, 6)
        self.assertTrue(add2(7) == 18, 'Testing 2-parameter currying')


if __name__ == '__main__':
    unittest.main()

