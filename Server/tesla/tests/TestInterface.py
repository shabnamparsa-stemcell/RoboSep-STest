# 
# TestInterface.py
#
# Unit tests for the Tesla instrument interface
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

from tesla.config import GATEWAY_HOST, GATEWAY_PORT
from tesla.interface import InstrumentInterface
from tesla.control.Centre import Centre
from tesla.types.sample import Sample

import tesla.hardware.Device
import tesla.instrument.Subsystem

# ---------------------------------------------------------------------------

class TestInstrumentInterface(unittest.TestCase):
   
    def setUp(self):
	self.host = "http://%s:%d" % (GATEWAY_HOST, GATEWAY_PORT + 10)
	controller = Centre()
        self.iface = InstrumentInterface(self.host, controller)

    def tearDown(self):
        tesla.hardware.Device.Device.reset()
        tesla.instrument.Subsystem.Subsystem.reset()

    def testServerInfo(self):
	info = self.iface.getServerInfo()
        NUM_INFO_FIELDS = 5         # Number of info fields that we are expecting
	self.failUnless(len(info) == NUM_INFO_FIELDS, 'Testing length of info list')

	(addr, ifaceVersion, uptime, swVersion, serialNum) = info
	self.failUnless(addr == self.host, 'Testing hostname')
	self.failUnless(uptime > 0, 'Testing uptime')

    def testState(self):
	instrumentState = self.iface.getInstrumentState()
	self.failUnless(instrumentState == 'Start state', 'Testing default instrument state')
   
    def testDefaultSubscriberList(self):
	self.failUnless(self.iface.getSubscriberList() == [], 'Testing no subscribers')

    def testInvalidSubscriber(self):
	# This address should never be an XML-RPC server (by default, it's an HTTP server)
	bogusAddr = 'http://localhost:80'
	self.failIf(self.iface.subscribe(bogusAddr), 'Testing invalid subscriber')

    def testValidSubscriber(self):
	# TBD - We need to set up an XML-RPC server for this
	self.failUnless(1)

    def testInvalidUnsubscription(self):
	bogusAddr = 'http://localhost:80'
	self.failIf(self.iface.unsubscribe(bogusAddr), 'Testing invalid unsubscription')

    def testValidUnsubscription(self):
	# TBD - We need to set up an XML-RPC server for this
	self.failUnless(1)

    def testProtocols(self):
	protocolList = self.iface.getProtocols()
	# self.failUnless(len(protocolList) > 0, 'Testing protocol list')

    def testConsumables(self):
	protocolList = self.iface.getProtocols()
	if len(protocolList):
	    protocol = protocolList[0]
	    sampleVolume = 5000
	    consumablesList = self.iface.calculateConsumables(protocol.ID, sampleVolume)
	    # self.failUnless(len(consumablesList) > 0, 'Testing consumables')

    def test_convertSamples(self):
	ID = 12
	data = [ {'ID':str(ID), 'label':'test_a', 'protocolID':'123', 'volume_uL':'450','initialQuadrant':1},
		    {'ID':'22', 'label':'test_b', 'protocolID':'567', 'volume_uL':'5000','initialQuadrant':2},
		]
	samples = self.iface._convertSamples(data)
	self.failUnless(type(samples) == type([]), 'Testing converted sample container type')
	self.failUnless(len(samples) == len(data), 'Testing for right number of samples')
	self.failUnless(isinstance(samples[0], Sample), 'Testing sample class')
	self.failUnless(samples[0].ID == ID, 'Testing sample ID')

    def test_convertFullSample(self):
        ID = 1967
        label = 'MySample'
        volume = 1234
        quadrant = 3
        cocktailLabel = 'Martini'; particleLabel = 'Beads'
        lysisLabel = 'Lyse this'; antibodyLabel = 'Relenza'
        
        data = [ {  'ID'              : str(ID), 
                    'label'           : label, 
                    'protocolID'      : '123',
                    'volume_uL'       : str(volume), 
                    'initialQuadrant' : quadrant,
                    'cocktailLabel'   : cocktailLabel,
                    'particleLabel'   : particleLabel,
                    'lysisLabel'      : lysisLabel,
                    'antibodyLabel'   : antibodyLabel,
                    'sample1Label'    : sample1Label,		 # bdr99
                    'sample2Label'    : sample2Label,		 # bdr99
                }, ]

        NUM_SAMPLES = len(data)
    	samples = self.iface._convertSamples(data)
        self.failUnless(len(samples) == NUM_SAMPLES, 'Ensuring we have one converted sample')
        sample = samples[0]
        self.failUnless(sample.ID == ID, 'Testing sample ID')
        self.failUnless(sample.volume == volume, 'Testing sample volume')
        self.failUnless(sample.initialQuadrant == quadrant, 'Testing initial quadrant')
        for var in ['label', 'cocktailLabel', 'particleLabel', 'lysisLabel', 'antibodyLabel']:
            test = eval("%s == sample.%s" % (var, var))
            expected = eval(var)
            self.failUnless(test, "Testing %s value (should be '%s')" % (var, expected))

        self.failUnless(Sample.counter == NUM_SAMPLES, 'Testing number of samples')


    def test_getServiceFunctionList(self):
        data = self.iface.getServiceFunctionList()
        self.failUnless(type(data) == type([]), 'Testing type of service function list')
        self.failIf(len(data) == 0, 'Testing that we have service functions')
        for entry in data:
            self.failUnless(type(entry) == type([]), 'Testing type of service function entry')
            self.failUnless(len(entry) == 4, 'Testing number of items in service function entry')
            print entry


    def test_execute(self):
        # NOTE: you have to be in the SERVICE state for execute() to work and
        #       there is no easy way to do that directly via the interface
        #       without using an XML-RPC client
        #       This means we should probably test at a different level
        pass


# -----------------------------------------------------------------------------

if __name__ == '__main__':
    unittest.main()

# eof
