# 
# TestSampleTracker.py
#
# Unit tests for tesla.control.SampleTracker.py
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

import os
import time
from datetime import datetime

from tesla.types.sample import Sample
from tesla.types.ClientProtocol import ClientProtocol
from tesla.control.SampleTracker import *

# -----------------------------------------------------------------------------

class TestSampleTracker(unittest.TestCase):
   
    # Dummy Protocol Consumable Class
    class PC:
        def __init__(self):
            pass

    # Dummy sample object, containing protocol consumable objects - bdr
    class DummySample:
        def __init__(self, sampleID, sampleLabel, protocolID, volume_uL, initialQuadrant,
		cocktailLabel = 'None', particleLabel = 'None', lysisLabel= 'None', antibodyLabel = 'None',
		cocktailVolume = 0, particleVolume = 0, lysisVolume= 0, antibodyVolume = 0):
            self.ID = sampleID
	    self.label = sampleLabel
	    self.protocolID = protocolID
            self.protocolLabel = 'Protocol_'+str(protocolID)

	    self.cocktailLabel = cocktailLabel
	    self.particleLabel = particleLabel
	    self.lysisLabel    = lysisLabel
	    self.antibodyLabel = antibodyLabel
#		self.sample1Label  = 'None'  # bdr99
#		self.sample2Label  = 'None'  # bdr99

            self.consumables = []
            for index in range (1,3):
                pc = TestSampleTracker.PC()
                pc.quadrant = index
	        pc.cocktailVolume = cocktailVolume
	        pc.particleVolume = particleVolume
	        pc.lysisVolume    = lysisVolume
	        pc.antibodyVolume = antibodyVolume
                pc.bulkBufferVolume   = (cocktailVolume + particleVolume + lysisVolume + antibodyVolume) * 10
	        pc.wasteVesselRequired        = index == 1
                pc.separationVesselRequired   = True
                pc.sampleVesselRequired       = index == 1
                pc.tipBoxRequired             = True

                self.consumables.append(pc)
	    
	    self.volume = volume_uL
	    self.initialQuadrant = initialQuadrant

    def setUp(self):
	self.st = SampleTracker()
        self.timeNow = time.time()
        self.startTime = datetime.fromtimestamp(self.timeNow)
        self.endTime = datetime.fromtimestamp(self.timeNow + 10000)

    def testEmptyTracker(self):
	self.failUnless(self.st.samples == None, 'Testing empty tracker - sample set')
	self.failIf(self.st.hasStarted(), 'Testing empty tracker - initial state')

    def addSamples(self):
        samples = []
        for index in [1,3]:
            thisSample = TestSampleTracker.DummySample(   
                                        index, 'Sample'+str(index), 5-index, 
                                        200*index-100, index,
                                        'Cocktail'+str(index), 'Particle'+str(index), 
                                        'Lysis'+str(index), 'Antibody'+str(index),
                                        300*index-300, 
                                        400*index-400, 
                                        500*index-500, 
                                        600*index-600,)
            samples.append(thisSample)
	self.st.addSamples(samples)
	self.failIf(self.st.samples == None, 'Testing Add samples')
	
    def testStartSchedule(self):
        self.st.resetTracker()
	self.addSamples()
        self.st.scheduleStart(str(self.endTime),self.startTime,'OperatorXYZ')
	self.failUnless(self.st.hasStarted(), 'Testing schedule running')

    def testCompleteSchedule(self):
        self.st.resetTracker()
        self.addSamples()
        self.st.scheduleStart(str(self.endTime),self.startTime,'OperatorXYZ')
        self.st.scheduleCompleted(self.endTime)
	self.failUnless(self.st.samples == None, 'Testing finished tracker - sample set')
	self.failIf(self.st.hasStarted(), 'Testing finished tracker - final state')

        # check if report file created
        self.failUnless(os.path.exists(self.st.getFileName(self.endTime)),'Testing existance of report')


    def testAbortSchedule(self):
        self.st.resetTracker()
        self.addSamples()
        self.st.scheduleStart(str(self.endTime),self.startTime,'OperatorYYY')
        self.st.scheduleAborted("User request",self.endTime)
	self.failUnless(self.st.samples == None, 'Testing finished tracker - sample set')
	self.failIf(self.st.hasStarted(), 'Testing finished tracker - final state')

        # check if report file created
        self.failUnless(os.path.exists(self.st.getFileName(self.endTime)),'Testing existance of report')
        
# -----------------------------------------------------------------------------
 
if __name__ == '__main__':
    unittest.main()

# eof





if __name__ == "__main__":
    # Run inbuilt test

    st = SampleTracker()
    print 'Today\'s Path = ', st.getFileName()
    print 'Path for [2005/7/22 18:03:59.0]= ', st.getFileName(
                    datetime(2005, 7, 22, 18, 03, 59, 000))

    st.resetTracker()
    

    st.resetTracker()
    st.addSamples(samples[1:2])
    etc = "22 Aug 2004 19:04:12"
    st.scheduleStart(etc)
    time.sleep(1.5)
    st.scheduleAborted("User request")
# eof

