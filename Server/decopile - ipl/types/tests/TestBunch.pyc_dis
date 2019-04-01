# File: T (Python 2.3)

import unittest
from ipl.types.Bunch import Bunch

def accessBadMember():
    data = Bunch(a = 1, b = 2, c = 3)
    return data.d


class TestBunch(unittest.TestCase):
    
    def testBunch(self):
        bearName = 'Edward'
        myYear = 1066
        b = Bunch(bear = bearName, year = myYear)
        self.failUnless(b.bear == bearName)
        self.failUnless(b.year == myYear)

    
    def testBadMember(self):
        self.assertRaises(AttributeError, accessBadMember)

    
    def testAssignment(self):
        myFruit = 'Loganberry'
        food = Bunch(yoghurt = 'Raspberry', cheese = 'Brie')
        food.fruit = myFruit
        self.failUnless(food.fruit == myFruit, 'Testing assignment')


if __name__ == '__main__':
    unittest.main()

