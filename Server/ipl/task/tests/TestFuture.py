# File: T (Python 2.3)

import unittest
import time
from ipl.task.future import Future

class PlughException(Exception):
    pass


def foo():
    pass


def doSomething(delay = 0):
    time.sleep(delay)
    return delay


def profile(func, *args):
    start = time.time()
    dummy = func(*args)
    timeDiff = round(time.time() - start)
    return timeDiff

badCounter = 0
badLimit = 30

def badFunc():
    global badCounter
    while badCounter < badLimit:
        badCounter += 1
        time.sleep(1)
    raise PlughException("where's the pirate?")
    badCounter += badLimit


class TestFuture(unittest.TestCase):
    
    def setUp(self):
        pass

    
    def testProfileFunction(self):
        myTime = 5
        delay = profile(doSomething, myTime)
        self.assertTrue(delay >= myTime, 'Testing profile()')

    
    def testNoDelay(self):
        A = Future(foo)
        self.assertTrue(A.isDone(), 'Testing no delay')

    
    def testDelay(self):
        start = time.time()
        delay = 10
        A = Future(doSomething, delay)
        self.assertFalse(A.isDone(), 'Testing start of call to Future()')
        result = A()
        timeDiff = round(time.time() - start)
        self.assertTrue(result == delay, 'Testing Future() result')
        self.assertTrue(timeDiff >= delay, 'Testing length of call to Future()')

    
    def testErrorsInside(self):
        A = Future(badFunc)
        self.assertRaises(PlughException, A)


if __name__ == '__main__':
    unittest.main()

