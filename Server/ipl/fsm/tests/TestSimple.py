# File: T (Python 2.3)

import unittest
from ipl.fsm import FSM_Exception
from ipl.fsm.simple import SimpleFSM

class TestSimpleFSM(unittest.TestCase):
    
    def setUp(self):
        table = { }
        table[SimpleFSM.START_STATE] = [
            'POWERED_ON']
        table['POWERED_ON'] = [
            'INITIALISED']
        table['INITIALISED'] = [
            'IDLE']
        table['IDLE'] = [
            'RUNNING',
            'SHUTDOWN']
        table['RUNNING'] = [
            'IDLE',
            'PAUSED',
            'SHUTDOWN']
        table['PAUSED'] = [
            'IDLE',
            'RUNNING']
        table['SHUTDOWN'] = [
            SimpleFSM.END_STATE]
        self.fsm = SimpleFSM(table)

    
    def testState(self):
        self.assertTrue(self.fsm.getState() == SimpleFSM.START_STATE, 'Test state')

    
    def testGetTargetStates(self):
        targets = self.fsm.getTargetStates()
        self.assertTrue(targets == [
            'POWERED_ON'], 'Target test')

    
    def testEmptyTargets(self):
        self.fsm.addGlobalState('ESTOP')
        self.fsm.changeState('ESTOP')
        targets = self.fsm.getTargetStates()
        self.assertTrue(targets == [], 'Empty target test')

    
    def testAddGlobalState(self):
        self.fsm.addGlobalState('ESTOP')
        targets = self.fsm.getTargetStates()
        self.assertTrue(targets == [
            'POWERED_ON',
            'ESTOP'], 'Add target test')

    
    def testChangeState(self):
        states = [
            'POWERED_ON',
            'INITIALISED',
            'IDLE',
            'RUNNING',
            'PAUSED',
            'RUNNING',
            'IDLE',
            'SHUTDOWN']
        for state in states:
            self.fsm.changeState(state)
        
        self.assertTrue(self.fsm.getState() == states[-1], 'Change state test')

    
    def testInvalidStateChange(self):
        self.assertRaises(FSM_Exception, self.fsm.changeState, 'RUNNING')

    
    def testAddTransition(self):
        self.fsm.addGlobalState('ESTOP')
        self.fsm.addTransition('ESTOP', SimpleFSM.END_STATE)
        self.fsm.changeState('ESTOP')
        self.fsm.changeState(SimpleFSM.END_STATE)
        self.assertTrue(self.fsm.getState() == SimpleFSM.END_STATE, 'Add transition test')

    
    def testStateProperty(self):
        self.fsm.state = 'POWERED_ON'
        self.assertTrue(self.fsm.state == 'POWERED_ON')


if __name__ == '__main__':
    suite = unittest.makeSuite(TestSimpleFSM, 'test')
    unittest.main()

