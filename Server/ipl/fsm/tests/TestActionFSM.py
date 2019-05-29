# File: T (Python 2.3)

import unittest
from ipl.fsm import FSM_Exception
from ipl.fsm.actionfsm import ActionFSM

class TestActionFSM(unittest.TestCase):
    
    def setUp(self):
        table = { }
        table[ActionFSM.START_STATE] = [
            'COLD']
        table['COLD'] = [
            'WARM']
        table['WARM'] = [
            'COLD',
            'HOT']
        table['HOT'] = [
            ActionFSM.END_STATE]
        self.fsm = ActionFSM(table)
        self.counter = 0

    
    def testTransitions(self):
        states = [
            'COLD',
            'WARM',
            'COLD',
            'WARM',
            'HOT']
        for state in states:
            self.fsm.state = state
            self.failUnless(self.fsm.getState() == state, 'Changing to %s' % state)
        

    
    def foo(self):
        self.counter += 1

    
    def testActions(self):
        self.fsm.entryActions['WARM'] = self.foo
        self.failUnless(self.counter == 0)
        self.fsm.state = 'COLD'
        self.fsm.state = 'WARM'
        self.failUnless(self.counter == 1)
        self.fsm.state = 'COLD'
        self.failUnless(self.counter == 1)
        self.fsm.state = 'WARM'
        self.failUnless(self.counter == 2)
        self.fsm.state = 'HOT'
        self.failUnless(self.counter == 2)


if __name__ == '__main__':
    suite = unittest.makeSuite(TestActionFSM, 'test')
    unittest.main()

