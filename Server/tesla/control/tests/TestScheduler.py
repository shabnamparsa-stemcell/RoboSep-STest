# 
# TestScheduler.py
#
# Unit tests for tesla.control.Scheduler
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

import tesla.config
import tesla.logger

from tesla.exception import TeslaException

from tesla.types.Command import Command, CommandFactory
from tesla.types.Protocol import Protocol, ProtocolException
from tesla.types.sample import Sample

from tesla.control.Schedule import Schedule
from tesla.control.ProtocolManager import ProtocolManager
from tesla.control.Scheduler import SchedulerBlock, SampleScheduler, SchedulerException

# -----------------------------------------------------------------------------
# Configuration data for the tests
PROTOCOL_PATH = tesla.config.PROTOCOL_DIR

# -----------------------------------------------------------------------------

class TestSchedulerBlock(unittest.TestCase):

    def setUp(self):
	self.open = 10
	self.used = 120
	self.free = 30
	self.block = SchedulerBlock(self.open, self.used, self.free)

    def test_blockMembers(self):
	self.failUnless(self.block.m_OpenPeriod == self.open, 'Testing m_OpenPeriod')
	self.failUnless(self.block.m_UsedPeriod == self.used, 'Testing m_UsedPeriod')
	self.failUnless(self.block.m_FreePeriod == self.free, 'Testing m_FreePeriod')

    def test_defaultStartTime(self):
	self.failUnless(self.block.m_StartTime == 0, 'Testing m_StartTime')

    def test_repr(self):
	s = self.block
	self.failUnless(s, 'Testing SchedulerBlock.__repr__()')

    def test_getPeriods(self):
	periods = self.block.getPeriods()
	self.failUnless(periods == (self.open, self.used, self.free), 'Testing getPeriods()')

    def test_getStartTime(self):
	self.failUnless(self.block.getStartTime() == 0, 'Testing getStartTime() #1')
	newStart = 1967
	self.block.m_StartTime = newStart
	self.failUnless(self.block.getStartTime() == newStart, 'Testing getStartTime() #2')

    def test_setFreePeriod(self):
	freeTime = 2004
	self.block.setFreePeriod(freeTime)
	self.failUnless(self.block.m_FreePeriod == freeTime, 'Testing setFreePeriod()')

    def test_setOpenPeriod(self):
	openTime = 1066
	self.block.setOpenPeriod(openTime)
	self.failUnless(self.block.m_OpenPeriod == openTime, 'Testing setOpenPeriod()')


# -----------------------------------------------------------------------------

class SchedulerTestCase(unittest.TestCase):
    # This test case is used to set up the following testcases and to provide
    # some support methods to simplify the testing

    def setUp(self):
	self.pm = ProtocolManager(PROTOCOL_PATH)
	self.logger = tesla.logger.Logger(logName = tesla.config.LOG_PATH)	
	self.sched = SampleScheduler(self.pm, self.logger)

    def getProtocol(self, index = 0):
	# Return a single protocol (the first one?) for testing purposes
	protocolList = self.pm.getProtocols()
	self.failIf(len(protocolList) == 0, 'Ensuring we have protocols for testing')
	return protocolList[index]

    def createSampleList(self, numSamples, protocol, volume):
	# Create a list of samples (of length numSamples) with a certain 
	# protocol and sample volume
	samples = []
	for i in range(1, numSamples + 1):
	    samples.append(Sample(i, "Test sample %d" % (i), protocol.ID, volume, i))
	return samples
    
# -----------------------------------------------------------------------------

class TestScheduler(SchedulerTestCase):

    def test_initialDuration(self):
	# The duration for a newly constructed schedule should be zero
	self.failUnless(self.sched.getDuration() == 0, 'Test initial duration')

    def test_initialSchedule(self):
	# The schedule for a newly constructed schedule should be None
	self.failUnless(self.sched.getSchedule() == None, 'Test initial schedule')

    def test_findSample(self):
	protocol = self.getProtocol()
	samples = self.createSampleList(4, protocol, 5000)
	# Test that we can find a sample we know we have
	sample = SampleScheduler.findSample(samples, 3)
	# Test that we can't find a sample we don't have!
	self.assertRaises(SchedulerException, SampleScheduler.findSample, samples, 42)
	# Test that we handle multiple samples with the sample ID
	samples.append(Sample(2, "Test sample repeat %d" % (2), protocol.ID, 2500, 2))
	self.assertRaises(SchedulerException, SampleScheduler.findSample, samples, 2)

    def test_getBlockList(self):
	# Test that we can get a valid block list for a sample
	protocol = self.getProtocol()
	sample = Sample(1, 'Test sample', protocol.ID, 5000, 1)
	self.sched.appendBlocks([sample,])
	blockList = self.sched.getBlockList(sample)
	self.failUnless(type(blockList) == type([]), 'Testing block list type')

	# This is the number of blocks we should have
	protocol = self.pm.getProtocol(sample.protocolID)
	cmdCount = len(protocol.getCommands())
	self.failUnless(cmdCount > 0, 'Expected schedulable commands for this test')	
	self.failUnless(len(blockList) == cmdCount, 'Number of blocks test')

    def test_schedule(self):
	# Schedule a single sample and then test the outcome
	protocol = self.getProtocol()		
	sample = Sample(42, 'Test sample', protocol.ID, 5000, 1)
	status = self.sched.schedule([sample,])

	self.failUnless(status, 'Testing scheduling success')
	self.failUnless(self.sched.getDuration() > 0, 'Test scheduled duration')	
	self.failUnless(self.sched.getSchedule() != None, 'Test schedule')
	self.failUnless(isinstance(self.sched.getSchedule(), Schedule), 'Test schedule instance')

    def test_multiSampleSchedule(self):
	# Schedule a set of four samples and then test the outcome
	protocol = self.getProtocol()
	numSamples = 3
	samples = self.createSampleList(numSamples, protocol, 1000)
	status = self.sched.schedule(samples)
	self.failUnless(status, 'Testing scheduling success')
	self.failUnless(self.sched.getDuration() > 0, 'Test scheduled duration')	
	self.failUnless(self.sched.getSchedule() != None, 'Test schedule')
	self.failUnless(isinstance(self.sched.getSchedule(), Schedule), 'Test schedule instance')

    def test_tooManySamples(self):
	# Try to induce the scheduler to fail by scheduling more samples than
	# the carousel can hold :)
	protocol = self.getProtocol()
	numSamples = 13
	self.assertRaises(TeslaException, self.createSampleList, numSamples, protocol, 1000)

    def test_failedScheduling(self):
        # At the moment, we can't schedule 4 of these. Try anyway and make sure
        # that an exception is raised
        NUM_SAMPLES = 4
	protocol = self.pm.findProtocolsByRegexp('Positive')[0]
	samples = self.createSampleList(NUM_SAMPLES, protocol, 725)
	self.assertRaises(SchedulerException, self.sched.schedule, samples)

    def test_durations(self):
	# Schedule a sample & store the calculated duration
	# Then schedule a set of samples
	# Now go back and reschedule the first sample -- do we get the same
	# duration? We should!
	protocol = self.getProtocol()
	sampleSet1 = self.createSampleList(1, protocol, 1000)
	status = self.sched.schedule(sampleSet1)
	firstDuration = self.sched.getDuration()
	sampleSet2 = self.createSampleList(4, protocol, 1000)
	status = self.sched.schedule(sampleSet2)
	status = self.sched.schedule(sampleSet1)
	secondDuration = self.sched.getDuration()
	self.failUnless(firstDuration == secondDuration, 'Testing repeated scheduling durations')	   

    def test_reset(self):
	# Set up a schedule, reset it and make sure our values have reset
	protocol = self.getProtocol()		
	sample = Sample(67, 'Another sample', protocol.ID, 750, 2)
	status = self.sched.schedule([sample,])

	self.failUnless(status, 'Testing scheduling success before reset')
	self.failUnless(self.sched.getDuration() > 0, 'Test scheduled duration')	
	self.failUnless(self.sched.getSchedule() != None, 'Test schedule')
	
	self.sched.reset()
	self.failUnless(self.sched.getDuration() == 0, 'Test reset duration')	
	self.failUnless(self.sched.getSchedule() == None, 'Test reset schedule')

    def test_getCommandDuration(self):
	cmdTypeList = Command.LEGAL_COMMANDS
	for cmdType in cmdTypeList:
	    cmd = CommandFactory(1, "%sCommand" % cmdType, "%s test cmd" % (cmdType))
	    if cmd.isWaitType():
		self.assertRaises(SchedulerException, self.sched.getCommandTimes, cmd)
	    else:
		open, used, free = self.sched.getCommandTimes(cmd)
		self.failUnless(open + used + free > 0, "Testing %s duration" % (cmdType))

    def test_getCommandTypeTime(self):
        cmdTypeList = Command.LEGAL_COMMANDS
        for cmdType in cmdTypeList:
	    cmd = CommandFactory(1, "%sCommand" % cmdType, "%s test cmd" % (cmdType))
	    if cmd.isWaitType():
		self.assertRaises(SchedulerException, self.sched.getCommandTypeTimes, cmdType)
            else:
                timeInfo = self.sched.getCommandTypeTimes(cmdType)
                self.failUnless(len(timeInfo) == 3, "Testing time info for %s" % (cmdType))

    def test_badCommandTypeTime(self):
        # We should have a Scheduler exception raised for commands that aren't in 
        # the command times map
        cmdType = 'NONSENSE_COMMAND'
        self.assertRaises(SchedulerException, self.sched.getCommandTypeTimes, cmdType)        

# -----------------------------------------------------------------------------

class TestSchedulerDuration(SchedulerTestCase):
    # Separate test case class for testing the duration of schedules

    def setUp(self):
        SchedulerTestCase.setUp(self)
        self.protocol = self.pm.findProtocolsByRegexp('Graeme test protocol')[0:1][0]
  
    def createSampleList(self, numSamples = 1):
        # Return a list of samples
        samples = []
        for i in range(1, numSamples + 1):
            id = i * 11                     # Arbitrary sample ID
            volume = i * 250                # Sample volume
            samples.append(Sample(id, 'Another sample', self.protocol.ID, volume, i))
        return samples
  
    def test_protocol(self):
        # Make sure we have a protocol to work with
        self.failIf(self.protocol == [], 'Ensuring we have a protocol to test with')

    def test_scheduling(self):
        for numSamples, expectedDuration in ((1, 1462), (2, 2144), (3, 2991),):
            sampleList = self.createSampleList(numSamples)
            status = self.sched.schedule(sampleList)
            self.failUnless(status)
            duration = self.sched.getDuration()
            return
            self.failUnless(duration == expectedDuration, \
                    "Testing scheduler duration, expected %d, got %d" % (expectedDuration, duration))

    def test_padding(self):
        sampleList = self.createSampleList(1)
        status = self.sched.schedule(sampleList)
        self.failUnless(status)
        padding = 123
        duration1 = self.sched.getDuration()
        duration2 = self.sched.getDuration(padding)
        self.failUnless(duration1 + padding == duration2, 'Testing duration padding')


# -----------------------------------------------------------------------------

class TestSchedulerThroughput(SchedulerTestCase):
   
    def test_throughput(self):
	testProtocol = self.getProtocol(1)
	samples = self.createSampleList(1, testProtocol, 5000)
	status = self.sched.schedule(samples)
	self.failUnless(status, 'Testing scheduling success for throughput')
	self.failUnless(self.sched.getDuration() > 0, 'Test scheduled duration')	
	self.failUnless(self.sched.getSchedule() != None, 'Test schedule')

    def generateTestForProtocol(self, protocolPattern, numSamples = 1):
	protocol = self.pm.findProtocolsByRegexp(protocolPattern)[0]
	samples = self.createSampleList(numSamples, protocol, 1250)
	status = self.sched.schedule(samples)
	# self.sched.dumpBlocks()		# Some debugging assistance
	self.failUnless(status, "Testing scheduling success (%s)" % (protocolPattern))
	self.failUnless(self.sched.getDuration() > 0, "Test scheduled duration (%s)" % (protocolPattern))
	self.failUnless(self.sched.getSchedule() != None, "Test schedule (%s)" % (protocolPattern))
	
    def test_withoutWaitCommands(self):
	# Test scheduling multiple samples using a protocol that doesn't
	# contain any Wait commands (Incubate or Separate)
	# In theory, this should happily schedule for 1 - 4 samples
	self.generateTestForProtocol('Shutdown', 4)
	
    def test_fewCommands(self):
	# Test scheduling multiple samples using a protocol that doesn't
	# have many commands in it
	# In theory, this should happily schedule for 1 - 4 samples
	self.generateTestForProtocol('Prime', 4)

    def test_singleCommandProtocol(self):
	# We have a protocol with a single command in it. If we can't
	# schedule multiple samples with this protocols, we have REAL
	# problems! :)
	self.generateTestForProtocol('Home all axes', 4)
	

# -----------------------------------------------------------------------------
 
if __name__ == '__main__':
    unittest.main()

# eof
