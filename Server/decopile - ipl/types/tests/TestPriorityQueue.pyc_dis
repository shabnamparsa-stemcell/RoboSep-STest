# File: T (Python 2.3)

import unittest
from ipl.types.PriorityQueue import PriorityQueue

class TestPriorityQueue(unittest.TestCase):
    
    def test_emptyQueue(self):
        q = PriorityQueue()
        self.failUnless(isinstance(q, PriorityQueue), 'Testing PriorityQueue class')
        self.failUnless(len(q) == 0, 'Testing empty queue length')

    
    def test_queue(self):
        q = PriorityQueue()
        q.insert('two', 2)
        q.insert('seven', 7)
        q.insert('nine', 9)
        q.insert('one', 1)
        q.insert('three', 3)
        expected = [
            'nine',
            'seven',
            'three',
            'two',
            'one']
        for number in expected:
            result = q.pop()
            self.failUnless(result == number, 'Expecting %s == %s' % (result, number))
        
        self.failUnless(len(q) == 0, 'Testing fully popped queue')


if __name__ == '__main__':
    unittest.main()

