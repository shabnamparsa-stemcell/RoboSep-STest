# 
# TestDispatcher.py
#
# Unit tests for tesla.control.Dispatcher.py
# 
# Copyright (c) Invetech Pty Ltd, 2004
# 495 Blackburn Rd
# Mt Waverley, Vic, Australia.
# Phone (+61 3) 9211 7700
# Fax   (+61 3) 9211 7701
# 
# The copyright to the computer program(s) herein is the
# property of Invetech Pty Ltd, Australia.
# The program(s) may be used and/or copied only with
# the written permission of Invetech Pty Ltd
# or in accordance with the terms and conditions
# stipulated in the agreement/contract under which
# the program(s) have been supplied.
# 

import unittest
import time, itertools, sys

import tesla.config
import tesla.logger

from tesla.control.Dispatcher import Dispatcher, DispatcherException
from tesla.control.ProtocolManager import ProtocolManager
from tesla.control.Scheduler import SampleScheduler, SchedulerException
from tesla.types.sample import Sample
from tesla.control.Schedule import Schedule

# -----------------------------------------------------------------------------

class MyInstrument(object):
    '''Dummy class for the testing of dispatching messages down to the
    Instrument class. This class allows us to test the Dispatcher without
    being connected to a 'real' instrument.'''

    def __init__(self):
        self.__cmdCounter = 0

    def isLidClosed(self):
        # We have to return True on this or we'll end up in the PAUSED state
        return True

    def getCmdCount(self):
        # How many commands have we executed?
        return self.__cmdCounter

    def foo(self, *args, **kw):
        # Our dummy method, basically any method called on an instance of 
        # MyInstrument that doesn't exist will end up using this method
        # which accepts any number of vanilla arguments and keyword arguments
        pass

    def __getattr__(self, name, *args):
        # Return a dummy method that takes any arguments and does nothing
        if name in ['HomeAll', 'Transport', 'Mix', 'Incubate', 'TopUpVial',
            'Separate', 'ResuspendVial', 'Flush', 'Prime', 'Demo', 'Park']:
            self.__cmdCounter += 1
        return self.foo
    

# -----------------------------------------------------------------------------

class TestDispatcher(unittest.TestCase):
   
    def setUp(self):
        self.pm = ProtocolManager(tesla.config.PROTOCOL_DIR)
        logger = tesla.logger.Logger(logName = tesla.config.LOG_PATH)        
        self.sched = SampleScheduler(self.pm, logger)

        self.instrument = MyInstrument()
        self.dispatcher = Dispatcher(self.instrument, logger)

    def test_init(self):
        self.assertTrue(self.dispatcher, 'Testing Dispatcher creation')

    def __createSchedule(self):
        # Create a list of samples from a specific protocol and then schedule them
        # Return the resulting Schedule object
        protocolList = self.pm.findProtocolsByRegexp('Graeme')
        self.assertFalse(len(protocolList) == 0, 'Ensuring we have enough protocols for testing')
        protocol1 = protocolList[0]

        # Create the samples and schedule them
        numSamples = 1 
        volume = 500
        samples = []
        for i in range(1, numSamples + 1):
            samples.append(Sample(i, "Sample #%d" % (i), protocol1.ID, 500.0 * i, i))
        status = self.sched.schedule(samples)

        # Testing the success of scheduling
        self.assertTrue(status, 'Testing scheduling success')
        self.assertTrue(self.sched.getDuration() > 0, 'Test scheduled duration')        

        mySchedule = self.sched.getSchedule()
        self.assertTrue(mySchedule != None, 'Test schedule')
        self.assertTrue(isinstance(mySchedule, Schedule), 'Test schedule instance')
        
        return mySchedule
        
    
    def __createSequenceAndWorkflows(self, schedule):
        # From the schedule return a tuple of events sequence and workflows
        sequence = Dispatcher.createSequence(schedule)
        workflows = schedule.getWorkflows()
        return (sequence, workflows)

    def test_createSequence(self):
        schedule = self.__createSchedule()
        sequence, workflows = self.__createSequenceAndWorkflows(schedule)
        self.assertTrue(sequence, 'Testing that we got a sequence')
        self.assertTrue(isinstance(sequence, list), 
                            'Testing that sequence is derived from the list class')
        self.assertTrue(len(sequence) > 0, 'Testing that we have events')
        self.assertTrue(len(workflows) == len(sequence), 'Testing # events')

    def test_sequenceItems(self):
        schedule = self.__createSchedule()
        sequence, workflows = self.__createSequenceAndWorkflows(schedule)
        for i in range(0, len(workflows)):
            wf, startTime = workflows[i].getWorkflowAndTime()
            eventTime, event = sequence[i]
            self.assertTrue(startTime == eventTime, "Testing sequence times (%d)" % (i))
            self.assertTrue(wf == event, "Testing sequence events (%d)" % (i))

    def test_processSchedule(self):
        schedule = self.__createSchedule()
        numCmds = len(schedule) + 1     # Add one for the HomeAll at the start
        self.dispatcher.processSchedule(schedule = schedule,
                                        activeState = 'START',
                                        pauseState =  'PAUSED',
                                        stoppedState ='HALTED',
                                        estopState =  'ESTOP',
                                        endState = 'END')
        i = itertools.count(1)
        delay = 10
        print("Be patient, this next test will take a while (hash every %s secs)" % (delay))
        while self.dispatcher.isRunning():
            time.sleep(delay)
            sys.stdout.write('#')
        print()
        cmdCount = self.instrument.getCmdCount()
        self.assertTrue(cmdCount == numCmds, "Testing # dispatched cmds; expected %d, got %d" % \
                            (numCmds, cmdCount))

    def test_badSchedule(self):
        # An invalid schedule instance should throw an exception
        schedule = 'hello world'
        self.assertRaises(DispatcherException, self.dispatcher.processSchedule,
                                        schedule = schedule,
                                        activeState = 'START',
                                        pauseState =  'PAUSED',
                                        stoppedState ='HALTED',
                                        estopState =  'ESTOP',
                                        endState = 'END')
        
# -----------------------------------------------------------------------------
 
if __name__ == '__main__':
    unittest.main()

# eof
