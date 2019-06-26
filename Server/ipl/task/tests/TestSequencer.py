# File: T (Python 2.3)

import unittest
import time
from ipl.task.sequencer import Sequencer, SequencerException
from functools import reduce

class TestSequencer(unittest.TestCase):
    
    def setUp(self):
        self.counter = 0

    
    def adder(self, increment = 1):
        self.counter += increment

    
    def testDebugState(self):
        events = [
            (1, 1),
            (2, 8),
            (3, 27),
            (4, 64)]
        seq = Sequencer(events, handler = self.adder)
        self.assertFalse(seq.getDebuggingState(), 'Testing default debug state')
        seq.enableDebugging()
        self.assertTrue(seq.getDebuggingState(), 'Testing enabled debug state')
        seq.enableDebugging(False)
        self.assertFalse(seq.getDebuggingState(), 'Testing disabled debug state')

    
    def testEvents(self):
        events = [
            (1, 1),
            (2, 4),
            (3, 9),
            (4, 16)]
        self.assertTrue(self.counter == 0, 'Ensuring counter is reset')
        seq = Sequencer(events, handler = self.adder)
        seq.run()
        while seq.isRunning():
            pass
        self.assertFalse(seq.isRunning(), 'Ensure sequencer has finished')
        expectedCount = reduce(lambda x, y: x + y, [x[1] for x in events])
        self.assertTrue(expectedCount == self.counter, 'Testing event count')

    
    def lessThanTen(self, value):
        print('===>', value)
        if value > 10:
            raise ValueError
        
        self.counter += value

    
    def _testExceptionHandling(self):
        events = [
            (1, 1),
            (2, 2),
            (3, 19)]
        self.assertTrue(self.counter == 0, 'Ensuring counter is reset')
        seq = Sequencer(events, handler = self.lessThanTen)
        self.run()
        while seq.isRunning():
            pass
        self.assertFalse(seq.isRunning(), 'Ensure sequencer has finished')
        print('~~~>', self.counter)

    
    def preBreak(self, val):
        pass

    
    def breaker(self, val):
        self.counter = val
        if val == self.BREAKPOINT:
            self.seq.abandonDispatching()
        

    
    def breakerExit(self):
        self.exited = True

    
    def testBreak(self):
        self.BREAKPOINT = 400
        self.exited = False
        events = [
            (1, 1),
            (2, 4),
            (3, 9),
            (5, 25),
            (10, 100),
            (20, 400),
            (30, 900)]
        self.seq = Sequencer(events, handler = self.breaker, exitFunc = self.breakerExit, preExecuteHandler = self.preBreak)
        self.seq.run()
        while self.seq.isRunning():
            pass
        self.assertFalse(self.seq.isRunning(), 'Ensure sequencer has finished')
        self.assertTrue(self.counter == self.BREAKPOINT, 'Testing sequencer was correctly abandoned')
        self.assertTrue(self.exited, 'Testing exit func was called')

    
    def pauseHandler(self, val):
        self.letter = val
        if val == self.PAUSE_VALUE:
            self.seq.pause()
        

    
    def testPause(self):
        self.PAUSE_VALUE = 'P'
        self.letter = ''
        events = [
            (1, 'A'),
            (2, 'B'),
            (3, 'C'),
            (13, 'M'),
            (16, 'P'),
            (17, 'Q'),
            (26, 'Z')]
        self.seq = Sequencer(events, handler = self.pauseHandler)
        self.seq.run()
        while not self.seq.isPaused():
            pass
        self.assertTrue(self.letter == self.PAUSE_VALUE, 'Testing correct pause point')
        self.seq.resume()
        while self.seq.isRunning():
            pass
        self.assertFalse(self.seq.isRunning(), 'Ensure sequencer has finished')
        self.assertTrue(self.letter == 'Z', 'Testing last letter passed')


if __name__ == '__main__':
    unittest.main()

