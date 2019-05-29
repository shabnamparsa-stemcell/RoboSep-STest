# File: a (Python 2.3)

from ipl.fsm import FSM_Exception
from ipl.fsm.simple import SimpleFSM

class ActionFSM(SimpleFSM):
    '''A finite state machine that supports actions on entry or exit.'''
    
    def __init__(self, transitionTable, entryTable = { }, exitTable = { }):
        SimpleFSM.__init__(self, transitionTable)
        self.entryActions = entryTable
        self.exitActions = exitTable

    
    def triggerAction(self, state, entry = True):
        if entry:
            actionTable = self.entryActions
        else:
            actionTable = self.exitActions
        if actionTable.has_key(state):
            apply(actionTable[state])
        

    
    def changeState(self, newState):
        '''Change to a new state.'''
        self.triggerAction(self.state, entry = False)
        SimpleFSM.changeState(self, newState)
        self.triggerAction(self.state, entry = True)

    state = property(SimpleFSM.getState, changeState, doc = 'State property')

