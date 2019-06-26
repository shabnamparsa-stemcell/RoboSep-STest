# 
# TestControlCentre.py
#
# Unit tests for tesla.control.Centre.py
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
import time

import tesla.config
from tesla.control.Centre import Centre
from tesla.types.sample import Sample
from tesla.types.ClientProtocol import ClientProtocol
from tesla.control.Schedule import Schedule

# -----------------------------------------------------------------------------

class TestCentre(unittest.TestCase):
   
    def setUp(self):
        self.cc = Centre(protocolPath = tesla.config.PROTOCOL_DIR)

    def test_noSchedule(self):
        self.assertTrue(self.cc.noSchedule(), 'Testing no schedule')

    def test_reset(self):
        self.cc.resetInstrument()
        self.assertTrue(self.cc.getState() == 'RESET', 'Testing state transition to RESET')

    def test_shutdownFromReset(self):
        self.cc.resetInstrument()
        self.assertTrue(self.cc.getState() == 'RESET', 'Testing state transition to RESET')
        self.cc.shutdownInstrument(powerDown = False)
        while self.cc.dispatcher.running:
            time.sleep(0.1)
        newState = self.cc.getState()
        self.assertTrue(newState == 'SHUTDOWN', "Testing shutdown state transition (got %s)" % (newState))

    def test_getProtocols(self):
        pro = self.cc.getProtocols()
        self.assertTrue(type(pro) == type([]), 'Testing protocols container')
        self.assertTrue(len(pro) > 0, 'Testing that we have protocols')
        self.assertTrue(isinstance(pro[0], ClientProtocol))

    def test_expandMsg_Status(self):
        msg0 = self.cc.expandMsg(self.cc.statusTable, 'TSC0002')
        self.assertTrue(msg0 == 'INIT', 'Testing zero-argument status message')

        msg3 = self.cc.expandMsg(self.cc.statusTable, 'TSC9999', 9999, 'Gaiman', 1602)
        self.assertTrue(msg3 == 'Status code 9999 testing Gaiman 1602', \
                'Testing three-argument status message')

        # This one should fail
        badMsg = self.cc.expandMsg(self.cc.statusTable, 'FOOBAR')
        self.assertFalse(badMsg.find('LOOKUP FAILURE') == -1, 'Testing bad lookup code')

    def test_expandMsg_Error0(self):
        # Test a 0-parameter error message
        msg0 = self.cc.expandMsg(self.cc.errorTable, 'TEC1000')
        self.assertTrue(msg0 == 'Placeholder.', 'Testing zero-argument error message')

    def test_expandMsg_Error2(self):
        # Test a 2-parameter error message
        msg2 = self.cc.expandMsg(self.cc.errorTable, 'TEC9999', 42, 1967)
        self.assertTrue(msg2 == 'Instrument interface error testing 42 1967.', \
                'Testing two-argument error message')

    def __getProtocol(self, protocolType):
        # Return the first protocol that matches the specified type
        protocols = [p for p in self.cc.getProtocols() if p.protocolClass == protocolType]
        return protocols[0]

    def __createSamples(self, numSamples = 4, protocolType = ClientProtocol.POSITIVE):
        protocol1 = self.__getProtocol(protocolType)
        volume = 500
        samples = []
        for i in range(1, numSamples + 1):
            samples.append(Sample(i, "Sample #%d" % (i), protocol1.ID, 500.0 * i, i))
        return samples

    def test_filterForSeparation(self):
        numSamples = 4
        samples = self.__createSamples(numSamples, ClientProtocol.POSITIVE)
        self.assertTrue(len(samples) == numSamples)
        filtered = self.cc.filterSamples(samples)
        self.assertTrue(len(filtered) == 4, 'Testing filtered separation samples')

    def test_filterForMaintenance(self):
        numSamples = 4
        samples = self.__createSamples(numSamples, ClientProtocol.MAINTENANCE)
        self.assertTrue(len(samples) == numSamples)
        filtered = self.cc.filterSamples(samples)
        self.assertTrue(len(filtered) == 1, 'Testing filtered maintenance samples')

    def test_filterForMixture(self):
        # Test a mix of separation and maintenance, we should just get one
        # maintenance sample back
        samples = []
        for protocol in [ClientProtocol.POSITIVE, ClientProtocol.MAINTENANCE, \
                ClientProtocol.POSITIVE, ClientProtocol.MAINTENANCE,]:
            samples.append(self.__createSamples(1, protocol)[0])
        numSamples = 4
        samples = self.__createSamples(numSamples, ClientProtocol.MAINTENANCE)
        self.assertTrue(len(samples) == numSamples)
        filtered = self.cc.filterSamples(samples)
        self.assertTrue(len(filtered) == 1, 'Testing filtered mixed samples')


    def getSamplesAndProtocol(self, numSamples):
        # Return a sample list and a protocol, for a certain number of samples
        protocols = self.cc.getProtocols()
        selectedProtocol = protocols[1]
        volume = 500                            # An arbitrary volume (in uL)
        samples = []
        
        # Create the samples and schedule them
        for i in range(1, numSamples + 1):
            samples.append(Sample(i, "Sample #%d" % (i), selectedProtocol.ID, 500.0 * i, i))
        return (samples, selectedProtocol)
        

    def test_scheduleRun(self):
        # This is a meaty test: create a list of samples and then schedule them
        # to run. Check the schedule information we get back...
        numSamples = 2
        samples, protocol1 = self.getSamplesAndProtocol(numSamples)
        
        # Create the samples and schedule them
        etc = self.cc.scheduleRun(samples)
        self.assertFalse(self.cc.noSchedule(), 'Testing for successful schedule')
        
        # Now test the actual schedule
        schedule = self.cc.scheduler.getSchedule()
        self.assertFalse(schedule == None, 'Testing schedule')
        self.assertTrue(isinstance(schedule, Schedule), 'Testing schedule class')

        # Do we have the right number of scheduled workflows?
        realProtocol = self.cc.pm.getProtocol(protocol1.ID)
        self.assertTrue(realProtocol, 'Testing retrieval of instrument-level protocol')
        numCmds = numSamples * len(realProtocol.getCommands())
        numWorkflows = len(schedule.getWorkflows())
        self.assertTrue(numCmds == numWorkflows, 'Testing number of scheduled workflows')
        
        # Test each workflow
        for wf in schedule.getWorkflows():
            cmd, startTime = wf.getWorkflowAndTime()
            self.assertTrue(type(cmd) == type(''), 'Testing that cmd is a string')
            self.assertTrue(type(startTime) == type(1), 'Testing that startTime is a number')
            self.assertTrue(startTime >= 0, 'Testing that startTime is sensible')

    def test_badScheduleRun(self):
        # This is a test to see how the control centre handles a badly 
        # scheduled run
        numSamples = 4
        samples, protocol1 = self.getSamplesAndProtocol(numSamples)
        etc = self.cc.scheduleRun(samples)
        self.assertTrue(self.cc.noSchedule(), 'Testing for unsuccessful schedule')
        
        # Now test the actual schedule
        schedule = self.cc.scheduler.getSchedule()
        self.assertTrue(schedule == None, 'Testing for empty schedule')

    def test_repeatScheduling(self):
        # What happens when we repeat scheduling?
        numSamples = 3
        numTests = 10
        samples, protocol1 = self.getSamplesAndProtocol(numSamples)

        # Collect the durations
        durations = []
        for i in range(0, numTests):
            t0 = time.time()
            etc1 = self.cc.scheduleRun(samples)
            durations.append(time.time() - t0)
        # Then analyse them
        duration0 = durations[0]
        threshold = durations[0] * 0.25         # Deviation of 25% allowed
        outOfRangeCount = 0
        for duration in durations:
            if duration - duration0 > threshold:
                outOfRangeCount += 1
        self.assertTrue(outOfRangeCount == 0, "Testing duration of repeat schedules (%d failed)" % (outOfRangeCount))

# -----------------------------------------------------------------------------
 
if __name__ == '__main__':
    unittest.main()

# eof
