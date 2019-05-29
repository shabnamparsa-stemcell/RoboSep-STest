# File: T (Python 2.3)

import unittest
from ipl.types.Null import Null

class MyClass(object):
    
    def __init__(self, x):
        self.x = x

    
    def divTest(self, val):
        self.x = val / self.x

    
    def getX(self):
        return self.x

    
    def printX(self):
        print self.x



def compute(x):
    c = MyClass(x)
    
    try:
        c.divTest(12)
        return c
    except ZeroDivisionError:
        return Null()



class TestNull(unittest.TestCase):
    
    def testNull(self):
        n = Null()
        self.failUnless(isinstance(n.foobar(), Null))

    
    def testNullReturn(self):
        for i in (3.3999999999999999, 2, 1, 0):
            val = compute(i).getX()
            if i != 0:
                self.failUnless(type(val) == type(i))
                continue
            self.failUnless(isinstance(val, Null))
        


if __name__ == '__main__':
    unittest.main()

