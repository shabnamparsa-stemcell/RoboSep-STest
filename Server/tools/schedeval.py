# 
# schedeval.py
#
# A tool for evaluating the scheduling feasibility of protocols.
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

import tesla.config
import tesla.logger
from tesla.types.ClientProtocol import ClientProtocol
from tesla.control.ProtocolManager import ProtocolManager
from tesla.control.Scheduler import SampleScheduler, SchedulerException
from tesla.types.sample import Sample

# -----------------------------------------------------------------------------

class SchedulerEvaluator(object):
    
    def __init__(self):
	self.pm = ProtocolManager(tesla.config.PROTOCOL_DIR)
	self.logger = tesla.logger.Logger(logName = tesla.config.LOG_PATH)
	self.scheduler = SampleScheduler(self.pm, self.logger)
	# self.protocols = [x for x in self.pm.getProtocolsForClient() if x.protocolClass in [ClientProtocol.POSITIVE, ClientProtocol.NEGATIVE]]
	self.protocols = self.pm.getProtocolsForClient()
	
    def inputNumber(self, prompt = '', minNum = 0, maxNum = 9999):
	num = int(raw_input(prompt))
	if num < minNum:
	    num = minNum
	elif num > maxNum:
	    num = maxNum
	return num

    def displayProtocols(self):
	i = 0
	for prot in self.protocols:
	    print "    %d\t%s" % (i, prot)
	    i += 1

    def selectSamples(self):
	print
	numSamples = self.inputNumber('Number of samples to schedule: ', 1, 4)
	sampleList = []	
	volume = 1000
	self.displayProtocols()
	for i in range(1, numSamples + 1):
	    protNum = self.inputNumber("Select protocol for sample %d: " % (i))
	    protID = self.protocols[protNum].ID
	    sampleList.append(Sample(i, "Sample %d" % (i), protID, volume, i))
	return sampleList

    def run(self):
	samples = self.selectSamples()
	try:
	    status = self.scheduler.schedule(samples)
	    self.schedule = self.scheduler.getSchedule()
	    etc = self.scheduler.getETC()
	    duration = self.scheduler.calculateDuration()

	except SchedulerException, msg:
	    print "Scheduler exception: %s" % (msg)
	    self.schedule = None		# Force the schedule to be null

	if self.schedule == None:
	    print "\nFAILED: No schedule could be determined"
	else:
	    print "\nScheduled! ETC = %s, duration = %d secs" % (etc, duration)
	    
	    for wf in self.schedule.getWorkflows():
		cmd, time = wf.getWorkflowAndTime()
		print "T = %4d\t%s" % (time, cmd[:60])

# -----------------------------------------------------------------------------

if __name__ == '__main__':
    se = SchedulerEvaluator()
    se.run()

# eof

