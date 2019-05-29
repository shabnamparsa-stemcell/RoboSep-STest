# File: T (Python 2.3)

import unittest
from ipl.utils.string import *

class TestString(unittest.TestCase):
    
    def test_expandCSharpString_plainString(self):
        plain = 'Humpty Dumpty sat on a wall'
        newPlain = expandCSharpString(plain)
        self.failUnless(plain == newPlain, 'Testing plain string')

    
    def test_expandCSharpString(self):
        data = (('1 x 1', '1'), ('3 x 3', '9'), ('9 x 12', '108'))
        input = '{0} = {1}'
        for (a, b) in data:
            newStr = expandCSharpString(input, (a, b))
            self.failUnless(newStr == '%s = %s' % (a, b), 'Testing expandCSharpString with %s' % str(data))
        

    
    def test_expandCSharpString_examples(self):
        example1 = expandCSharpString('After {0} cycles, state = {1}', (42, 'IDLE'))
        example2 = expandCSharpString('My {3} name is {2} and my {1} name is {0}', ('Cross', 'last', 'Graeme', 'first'))
        self.failUnless(example1 == 'After 42 cycles, state = IDLE')
        self.failUnless(example2 == 'My first name is Graeme and my last name is Cross')

    
    def test_expandCSharpString_swappedParameters(self):
        input = '{1} + {0} = {2}'
        newStr = expandCSharpString(input, (2, 3, 5))
        self.failUnless(newStr == '3 + 2 = 5', 'Testing swapped parameters')


if __name__ == '__main__':
    unittest.main()

