# Python bytecode 2.3 (62011)
# Embedded file name: SimpleStep\tests\TestSimpleStep.py
# Compiled at: 2004-08-27 07:38:35
# Decompiled by https://python-decompiler.com
import unittest
from SimpleStep.SimpleStep import SimpleStep, SimpleStepError

class TestSimpleStep(unittest.TestCase):
    __module__ = __name__

    def setUp(self):
        self.port = 'COM1'
        self.card = SimpleStep('D1', port=self.port, useEmulator=True)

    def test_parseAddress(self):
        self.failUnless(self.card.board == 'D', 'Board parsed')
        self.failUnless(self.card.address == 1, 'Address parsed')

    def test_PollUntilReady(self):
        packet = self.card.pollUntilReady()
        self.failUnless(packet == '>', 'Poll until ready')

    def test_isMotorCommand(self):
        self.failUnless(self.card.isMotorCommand('M'), 'M is motor command')
        self.failUnless(self.card.isMotorCommand('R'), 'R is motor command')
        self.failIf(self.card.isMotorCommand('z'), 'z is not a motor command')

    def test_getStatus(self):
        status = self.card.getStatus()
        self.failUnless(status == '>', 'getStatus()')

    def test_isPresent(self):
        self.failUnless(self.card.isPresent(), 'isPresent()')

    def test_getPort(self):
        self.failUnless(self.card.getPort() == self.port, 'getPort()')

    def test_getBoardInfo(self):
        pass


unittest.main()