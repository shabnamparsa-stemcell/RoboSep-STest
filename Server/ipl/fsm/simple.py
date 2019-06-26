# File: s (Python 2.3)

from ipl.fsm import FSM_Exception

class SimpleFSM(object):
    '''
    A simple finite state machine implementation.
    States can be defined using any type that can be used as a dictionary
    key. Strings make for nicely-readable states, especially when debugging.
    Note that there is no support for actions, activities or the likes. For 
    that, have a look at ipl.fsm.harel (which is not yet written :)
    '''
    START_STATE = 'Start state'
    END_STATE = 'End state'
    
    def __init__(self, transitionTable):
        '''Create a finite state machine object with a transition table
\tthat is a dictionary where each key is a state and the associated
\tvalue is a list of legal states that we can transition to from
\tthe key state.'''
        self._SimpleFSM__transitions = transitionTable
        self._SimpleFSM__state = SimpleFSM.START_STATE

    
    def getState(self):
        '''Get the current state.'''
        return self._SimpleFSM__state

    
    def addGlobalState(self, state):
        '''Add a state that can be accessed from every other state.'''
        for startingState in list(self._SimpleFSM__transitions.keys()):
            self._SimpleFSM__transitions[startingState].append(state)
        

    
    def addTransition(self, startState, newState):
        '''Add a transition to our FSM transition table defining the
\tstarting state and the new state we transition to.'''
        if startState in self._SimpleFSM__transitions:
            self._SimpleFSM__transitions[startState].append(newState)
        else:
            self._SimpleFSM__transitions[startState] = [
                newState]

    
    def getTargetStates(self):
        '''Return the states we can move to from the current state.'''
        
        try:
            targets = self._SimpleFSM__transitions[self.getState()]
        except KeyError:
            targets = []

        return targets

    
    def changeState(self, newState):
        '''Change to a new state.'''
        targets = self.getTargetStates()
        if newState in targets:
            self._SimpleFSM__state = newState
        else:
            raise FSM_Exception("%s is not a target state for the '%s' state" % (newState, self.getState()))

    state = property(getState, changeState, doc = 'State property')

if __name__ == '__main__':
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
    fsm = SimpleFSM(table)
    print(fsm.getState())
    print(fsm.getTargetStates())
    fsm.addGlobalState('ESTOP')
    print(fsm.getTargetStates())
    fsm.changeState('POWERED_ON')
    print(fsm.getState())
    fsm.changeState('ESTOP')
    fsm.addTransition('ESTOP', SimpleFSM.END_STATE)
    fsm.changeState(SimpleFSM.END_STATE)

