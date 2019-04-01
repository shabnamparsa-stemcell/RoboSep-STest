# 
# TestCommand.py
#
# Unit tests for the tesla.types.Command class
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

from tesla.types.Command import *
from tesla.types.sample import Sample
from tesla.types.Protocol import Protocol
from tesla.instrument.Instrument import Instrument

# -------------------------------------------------------------------------

class TestCommand(unittest.TestCase):

    def test_members(self):
	seq = 1
	cmdType = 'Mix'
	label = 'Test mix'
    	cmd = Command(seq, cmdType, label)
	self.failUnless(cmd.seq == seq, 'Testing seq member')
	self.failUnless(cmd.type == cmdType, 'Testing type member')
	self.failUnless(cmd.label == label, 'Testing label member')

    def test_isLegalCommandType(self):
	for cmdType in Command.LEGAL_COMMANDS:
	    cmd = Command(1, cmdType, "Command = %s" % (cmdType))
	    self.failUnless(Command.isLegalCommandType(cmdType), "Testing that '%s' is legal" % (cmdType))
	    self.failUnless(cmd.isLegal(), 'Testing isLegal()')
	
    def test_illegalCommand(self):
	self.failIf(Command.isLegalCommandType('ILLEGAL'), 'Illegal command test')

    def test_isWaitCommand(self):
	for cmdType in ['Incubate', 'Separate']:
	    cmd = Command(2, cmdType, "Command = %s" % (cmdType))
	    self.failUnless(Command.isWaitCommand(cmdType), "Testing '%s' isWaitCommand()" % (cmdType))
	    self.failIf(cmd.isWaitType(), 'Testing that plain command is not isWaitType()')
	cmd1 = Command(4, 'Mix', 'Another test')
	self.failIf(Command.isWaitCommand(cmd1.type), 'Testing non-wait command')
	cmd2 = WaitCommand(5, 'Incubate', 'Cooking...')
	self.failUnless(cmd2.isWaitType(), 'Testing isWaitType()')

    def test_isTransportType(self):
	for cmdType in ['Volume']:
	    cmd = Command(67, cmdType, "Command = %s" % (cmdType))
	    self.failIf(cmd.isTransportType(), 'Testing that plain command is not isTransportType()')
	cmd = TransportCommand(68, 'Transport', 'Another test')
	self.failUnless(cmd.isTransportType(), 'Testing isTransportType()')

    def test_freeAirDispense(self):
	cmd = TransportCommand(1066, 'Transport', 'Free air dispense transport')
	self.failIf(cmd.freeAirDispense, 'Testing no free air dispense')
	cmd.setFreeAirDispense(True)
	self.failUnless(cmd.freeAirDispense, 'Testing free air dispense flag')

    def test_useBufferTip(self):
	cmd = TransportCommand(1066, 'Transport', 'Use buffer tip transport')
	self.failIf(cmd.useBufferTip, 'Testing no use buffer tip')
	cmd.setUseBufferTip(True)
	self.failUnless(cmd.useBufferTip, 'Testing use buffer tip flag')

    def test_isVolumeType(self):
	for cmdType in ['Mix']:
	    cmd = Command(13, cmdType, "Command = %s" % (cmdType))
	    self.failIf(cmd.isVolumeType(), 'Testing that plain command is not isVolumeType()')	    
	cmd = VolumeCommand(14, 'Mix', 'Another test')
	self.failUnless(cmd.isVolumeType(), 'Testing isVolumeType()')

    def test_abstractMethods(self):
	# The base class has at least one abstract method -- make sure they
	# can't be directly called
	cmd = Command(99, 'Incubate', 'A test command')
	self.assertRaises(NotImplementedError, cmd.createCall)

# -----------------------------------------------------------------------------

class TestHomeAllCommand(unittest.TestCase):

    def setUp(self):
	self.seq = 2
    
    def test_homeAll(self):
	cmd = HomeAllCommand(self.seq, 'HomeAll', 'Homing all axes')
	self.failUnless(cmd.seq == self.seq)
	self.failUnless(isinstance(cmd, HomeAllCommand), 'Testing HomeAll instance')

    def test_factory(self):
	cmd = CommandFactory(self.seq, 'HomeAllCommand', 'Homing all axes')
	self.failUnless(cmd.seq == self.seq)
	self.failUnless(isinstance(cmd, HomeAllCommand), 'Testing HomeAll instance')

    def test_isServiceType(self):
	cmd = CommandFactory(123, 'HomeAllCommand', 'Homing all axes')
	self.failUnless(cmd.isServiceType(), 'Testing isServiceType()')


# -----------------------------------------------------------------------------

class Test_DemoCommand(unittest.TestCase):

    def setUp(self):
	self.seq = 4
	self.cmd = CommandFactory(self.seq, 'DemoCommand', 'Demo mode')

    def test_factory(self):
	self.failUnless(self.cmd.seq == self.seq)
	self.failUnless(isinstance(self.cmd, DemoCommand), 'Testing Demo from the factory')

    def test_isServiceType(self):
	self.failUnless(self.cmd.isServiceType(), 'Testing isServiceType()')
	
    def test_iterations(self):
	self.failUnless(self.cmd.iterations == 1, 'Testing default # iterations')

# -----------------------------------------------------------------------------

class TestWaitCommand(unittest.TestCase):

    def test_periods(self):
	cmd = WaitCommand(1, 'Incubate', 'Incubating')
	min = 300
	max = 420
	cmd.setPeriods(min, max)
	self.failUnless(cmd.minPeriod == min, 'Testing minimum wait period')
	self.failUnless(cmd.maxPeriod == max, 'Testing maximum wait period')

    def XXX_test_createCall(self):
        #
        # XXX - reactivate this        
        #
	cmd = WaitCommand(1, 'Incubate', 'Incubating')
        testFile = 'Generic Positive Selection protocol.xml'
	protocol = Protocol(fileName = os.path.join(tesla.config.PROTOCOL_DIR, testFile))
	sample = Sample(42, 'Test sample', protocol.ID, 5000, 3)
	cmd.createCall(sample, protocol)

# -----------------------------------------------------------------------------

class MockSample:
    def __init__(self):
	self.ID = 42
	self.initialQuadrant = 1
        self.volume = 1000


class TestTopUpVial(unittest.TestCase):

    def setUp(self):
	self.cmd = TopUpVialCommand(3, 'TopUpVial', 'Topping up')
    
    def test_setVials(self):
	self.cmd.setVials('TPC0103','TPC0107')
	self.failUnless(self.cmd.srcVial == (1, Instrument.CocktailLabel), 'Testing src vial')
	self.failUnless(self.cmd.destVial == (1, Instrument.SupernatentLabel), 'Testing dest vial')	

    def test_noSrcVial(self):
	self.cmd.setVials(None,'TPC0107')
	self.failUnless(self.cmd.srcVial == (), 'Testing empty src vial')
	self.failUnless(self.cmd.destVial == (1, Instrument.SupernatentLabel), 'Testing dest vial')	
	
    def XXX_test_createCall(self):
        # XXX - reactivate this
        #
	# Test vials specified properly and those that have been accidently switched
	for vial1, vial2 in [(None, 'TPC0107'), ('TPC0107', None)]:
	    self.cmd.setVials(vial1, vial2)
            testFile = 'Generic Positive Selection protocol.xml'
	    protocol = Protocol(fileName = os.path.join(tesla.config.PROTOCOL_DIR, testFile))
	    sample = Sample(42, 'Test sample', protocol.ID, 5000, 3)
            cmdCall = self.cmd.createCall(sample, protocol)
	    expected = "TopUpVial(destVial=self.instrument.ContainerAt(3, 'supernatentvial'), srcVial=None, workVolume=8500.00, id=42, seq=3)"
	    self.failUnless(cmdCall == expected, 'Testing swapped vials')

# -----------------------------------------------------------------------------

class TestVolumeCommand(unittest.TestCase):

    def setUp(self):
	self.cmd = VolumeCommand(2, 'Transport', 'Moving fluid')

    def test_getContainerString(self):
        data = (    ((2, 'WasteVial'), "self.instrument.ContainerAt(2, 'WasteVial')"),
                    ((3, 'BCCMContainer'), "self.instrument.ContainerAt(0, 'BCCMContainer')"))
        sample = MockSample() 
        for vial, expected in data:
            containerStr = self.cmd.getContainerString(sample, vial)
            self.failUnless(containerStr == expected, "Expected %s, got %s" % (expected, containerStr))


    def test_relativeVolumeInfo(self):
        # XXX
        pass

    def test_absoluteVolumeInfo(self):
        # XXX
        pass

    def test_vials(self):
	srcVial = 'TPC0105'             # Antibody vial, quadrant 1
	destVial = 'TPC0201'             # Waste vial, quadrant 2
	self.cmd.setVials(srcVial, destVial)
	self.failUnless(self.cmd.srcVial == (1, 'AntibodyVial'))
	self.failUnless(self.cmd.destVial == (2, 'WasteVial'))

    def test_NoVials(self):
	srcVial = None
	destVial = None
	self.cmd.setVials(srcVial, destVial)
	self.failUnless(self.cmd.srcVial == (), 'Null srcVial test')
	self.failUnless(self.cmd.destVial == (), 'Null destVial test')
   
    def test_badVialInput1(self):
	srcVial = 'VOODOO'
	destVial = None
	self.assertRaises(CommandException, self.cmd.setVials, srcVial, destVial)

    def test_badVialInput2(self):
	srcVial = '1,2,3'
	destVial = None
	self.assertRaises(CommandException, self.cmd.setVials, srcVial, destVial)

    def test_badQuadrants(self):
	srcVial = '5,1'
	destVial = None
	self.assertRaises(CommandException, self.cmd.setVials, srcVial, destVial)
	srcVial = None
	destVial = '9,1'
	self.assertRaises(CommandException, self.cmd.setVials, srcVial, destVial)
	
    def test_badVialNumber(self):
	srcVial = '1,0'
	destVial = None
	self.assertRaises(CommandException, self.cmd.setVials, srcVial, destVial)
	srcVial = '99,0'
	destVial = None
	self.assertRaises(CommandException, self.cmd.setVials, srcVial, destVial)

# -----------------------------------------------------------------------------

class TestCommandFactory(unittest.TestCase):
    
    def test_creation(self):
	vanillaCmd = CommandFactory(1, 'Command', 'A vanilla command')
	self.failUnless(isinstance(vanillaCmd, Command), 'Testing Command creation')

    def test_specificCreation(self):
	seq = 0
	for type in Command.LEGAL_COMMANDS:
	    seq += 1
	    cmd = CommandFactory(seq, "%sCommand" % (type), "%s command" % (type))
	    self.failUnless(isinstance(cmd, Command), "Testing %sCommand creation" % (type))

    def test_invalidCreation(self):
	# We should not be able to create Command instances with an invalid type
	type = 'INVALID'
	self.assertRaises(NameError, CommandFactory, 42, type, "%s command" % (type))

    def test_parseXML(self):
	# TBD - we need to turn this data:
	#
	# <command seq='3' type='Incubate' label='Incubate Cocktail and Sample'>
	#     <time min='720' max='1080'/>
	# </command>
	#
	# Into an Element node (from HandyXML), for example:
	# <Element Node at bfc788: Name='command' with 3 attributes and 3 children>
	# 
	waitData = None
	cmd = CommandFactory(1, 'WaitCommand', 'Waiting', waitData)
	self.failUnless(cmd)

	

# -----------------------------------------------------------------------------

if __name__ == '__main__':
    unittest.main()

# eof
