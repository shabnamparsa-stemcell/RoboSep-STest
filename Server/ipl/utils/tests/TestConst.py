# File: T (Python 2.3)

import unittest
from ipl.utils import const

class TestConst(unittest.TestCase):
    
    def test_creation(self):
        const.foo = 1
        self.assertTrue(const.foo == 1, 'Constant set')

    
    def test_resetting(self):
        const.foo = 42
        flag = 0
        
        try:
            const.foo = 67
        except ConstError:
            msg = None
            print('>>>>', msg)
            flag = 1

        self.assertTrue(flag, 'Resetting the constant')

    
    def test_del(self):
        flag = 0
        const.foo = 32
        
        try:
            del const.foo
        except:
            flag = 1

        self.assertTrue(flag, 'Deleting the constant')

    
    def test_unknownConstant(self):
        flag = 0
        
        try:
            del const.bar
        except NameError:
            flag = 1

        self.assertTrue(flag, 'Removing unknown constant')


unittest.main()
