# File: T (Python 2.3)

import unittest
from ipl.utils import *

class TestUtils(unittest.TestCase):
    
    def test_simpleHash(self):
        h1 = simpleHash('Mary')
        h2 = simpleHash('Merry')
        self.assertTrue(type(h1) == type(1), 'Testing hash return type')
        self.assertTrue(h1 >= 0, 'Testing hash return value')
        self.assertFalse(h1 == h2, 'Testing for collision')

    
    def test_hexHash(self):
        h1 = hexHash('Phone: 92117700')
        h2 = hexHash('Phone: 92117770')
        self.assertTrue(type(h1) == type(''), 'Testing hex hash return type')
        self.assertFalse(h1 == h2, 'Testing for hexHash collision')

    
    def test_isIterable(self):
        a = [
            1,
            2,
            3,
            4]
        self.assertTrue(isIterable(a), 'Testing isIterable on list')
        b = (9, 7, 6, 5, 4)
        self.assertTrue(isIterable(b), 'Testing isIterable on tuple')
        c = {
            1: 2,
            2: 'a',
            3: (3, 4),
            4: 'asdas' }
        self.assertTrue(isIterable(c), 'Testing isIterable on dictionary')
        d = 1
        self.assertFalse(isIterable(d), 'Testing isIterable on integer')
        e = 'hello'
        self.assertTrue(isIterable(e), 'Testing isIterable on string')

    
    def test_isStringLike(self):
        a = (1, 2, 3, 4)
        b = 'heliosphan'
        self.assertFalse(isStringLike(a), 'Testing isStringLike on list')
        self.assertTrue(isStringLike(b), 'Testing isStringLike on string')

    
    def test_isScalar(self):
        good = ('a', 9, True, 3.1415000000000002, 'Rjd2')
        for item in good:
            self.assertTrue(isScalar(item), 'Testing isScalar on %s' % item)
        
        bad = ((1, 2, 3), [
            'a',
            'b',
            'c'], {
            1: 1,
            2: 4,
            3: 9 })
        for item in bad:
            self.assertFalse(isScalar(item), 'Testing isScalar on %s' % str(item))
        

    
    def test_flattenSequence(self):
        a = (1, 2, (3, 4), 5)
        flat = flattenSequence(a)
        self.assertTrue(flat == [
            1,
            2,
            3,
            4,
            5], 'Testing flattenSequence')
        b = [
            (1, 2),
            'maynard',
            [
                3,
                [
                    4,
                    [
                        5]]],
            (6, 'lilith'),
            7]
        reallyFlat = flattenSequence(b)
        self.assertTrue(reallyFlat == [
            1,
            2,
            'maynard',
            3,
            4,
            5,
            6,
            'lilith',
            7], 'Testing flattenSequence')
        c = 'ptolemy'
        self.assertTrue(flattenSequence(c) == [
            'ptolemy'], 'Testing flattenSequence on string')
        d = None
        self.assertTrue(flattenSequence(d) == [
            None], 'Testing flattenSequence on None')


if __name__ == '__main__':
    unittest.main()

