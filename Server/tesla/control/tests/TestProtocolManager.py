# 
# TestProtocolManager.py
# Unit tests for the Protocol Manager class
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

from ipl.types.Bunch import Bunch

import tesla.config
from tesla.types.Protocol import *
from tesla.types.ProtocolConsumable import ProtocolConsumable
from tesla.types.ClientProtocol import ClientProtocol, ClientProtocolException
from tesla.control.ProtocolManager import ProtocolManager

# -----------------------------------------------------------------------------

class TestProtocolManager(unittest.TestCase):
    # Test the protocol manager by making it read in protocol definition files
    
    def setUp(self):
        pm = ProtocolManager()
        # Clear data out of the protocol manager & read in protocols
        pm.clear()
        self.pm = ProtocolManager(tesla.config.PROTOCOL_DIR)

    def test_checkForFiles(self):
        self.failUnless(self.pm.getNumberOfProtocols() > 0, 'Need files for these tests')

    def test_database(self):
        if self.pm.usedDB():
            # If we used the database, delete it and create a new instance of
            # the protocol manager and ensure that it doesn't use the database
            status = os.system("del \"%s\"" % (self.pm.protocolDBPath))
            self.failUnless(status == 0, 'Unable to delete protocol database')
            newPM = ProtocolManager(tesla.config.PROTOCOL_DIR)
            self.failIf(newPM.usedDB(), 'Testing that we did not use the database')
            anotherPM = ProtocolManager(tesla.config.PROTOCOL_DIR)
            self.failUnless(anotherPM.usedDB(), 'Testing that we have used the database')

    def test_filesAreEqual(self):
        f1, f2 = [], []
        for filename, fileTime in [('a.xml', 1066), ('b.xml', 1967), ('c.xml', 1999), ('d.xml', 2001),]:
            f1.append(Bunch(name = filename, time = fileTime))
            f2.append(Bunch(name = filename, time = fileTime))
        self.failUnless(self.pm.filesAreEqual(f1, f2), 'Testing files are equal')

    def test_getProtocols(self):
        protocolList = self.pm.getProtocols()
        self.failUnless(type(protocolList) == type([]), 'Test getProtocols() return type')
        self.failUnless(len(protocolList) > 0, 'Need protocols for these tests')
        self.failUnless(isinstance(protocolList[0], Protocol), 'Testing returned protocol')

    def test_getProtocolsForClient(self):
        protocolList = self.pm.getProtocols()        
        clientList = self.pm.getProtocolsForClient()
        self.failUnless(len(clientList) > 0, 'Testing presence of client protocols')
        self.failUnless(len(protocolList) == len(clientList), 'Testing length of client list')
        self.failUnless(isinstance(clientList[0], ClientProtocol), 'Testing returned client protocol')

        # A second call to getProtocolsForClient() should NOT fail
        p2 = self.pm.getProtocolsForClient()

    def test_findProtocolsByRegexp(self):
        pattern = '.*'
        protocols = self.pm.getProtocols()
        matches = self.pm.findProtocolsByRegexp(pattern)
        self.failUnless(matches == protocols, "Testing regexp finding for %s" % (pattern))

        singleMatch = self.pm.findProtocolsByRegexp('Shutdown')
        self.failUnless(len(singleMatch) == 1, 'Testing for single regexp hit')
        
    def test_findNoProtocolsByRegexp(self):
        # We shouldn't have any matching protocols with this label :)
        matches = self.pm.findProtocolsByRegexp('VOODOO')
        self.failUnless(matches == [], 'Testing regexp finding with no results')

    def test_findProtocolsByType(self):
        protocols = self.pm.findProtocolsByType('Maintenance')
        self.failUnless(type(protocols) == type([]), 'Testing returned type')
        for p in protocols:
            self.failUnless(p.type == 'Maintenance', 'Testing protocol type')

    def test_findNoProtocolsByType(self):
        protocols = self.pm.findProtocolsByType('SILLY_TYPE')
        self.failUnless(len(protocols) == 0, 'Testing bad protocol-find type')

    def test_volumeInformation(self):
        protocol = self.pm.findProtocolsByRegexp('Generic')[0]
        # protocol = self.pm.findProtocolsByRegexp('Prime')[0]
        pcList = self.pm.getConsumableInformation(protocol.ID, 1250)
        
        numConsumables = len(pcList)
        # self.failUnless(numConsumables > 0, 'Testing that we got consumables information')
        self.failUnless(type(pcList) == type([]), 'Testing consumables types')
        
        pc = pcList[0]
        self.failUnless(isinstance(pc, ProtocolConsumable), 'Testing ProtocolConsumable type')

        protocolQuadrantsNumber = protocol.numQuadrants
        self.failUnless(protocolQuadrantsNumber in range(1,5), 'Ensuring quadrant number is okay')
        self.failUnless(numConsumables == protocolQuadrantsNumber, 'Testing # quadrants')

    def test_badVolumeInformation(self):
        protocol = self.pm.findProtocolsByRegexp('Generic')[0]
        self.assertRaises(ProtocolException, self.pm.getConsumableInformation, protocol.ID, 0)
        self.assertRaises(ProtocolException, self.pm.getConsumableInformation, protocol.ID, -1)
        self.assertRaises(ProtocolException, self.pm.getConsumableInformation, protocol.ID, 900000)

    def test_isSeparationProtocol(self):
        mainP1 = self.pm.findProtocolsByType('Maintenance')[0]
        self.failIf(self.pm.isSeparationProtocol(mainP1), \
                'Testing non-separation protocol')
        
        sepP1 = self.pm.findProtocolsByType('Positive')[0]
        sepP2 = self.pm.findProtocolsByType('Negative')

        for protocol in (sepP1,):
            self.failUnless(self.pm.isSeparationProtocol(protocol.ID), \
                    "Testing %s-type protocol" % (protocol.type))

    def test_isSampleVolumeValidForProtocol(self):
        # Test a "safe" volume
        protocol = self.pm.findProtocolsByRegexp('Generic')[0]
        self.failUnless(self.pm.isSampleVolumeValidForProtocol(protocol, 1000))

        # This should fail
        self.failIf(self.pm.isSampleVolumeValidForProtocol(protocol, -123))

        # Now test with an ID, rather than a protocol -- we should cope
        id = protocol.ID
        self.failUnless(self.pm.isSampleVolumeValidForProtocol(id, 1000))

            
    def test_isSampleVolumeValidForProtocol_forMaintenance(self):
        # This should pass for any volume
        vol = 1000
        mainP1 = self.pm.findProtocolsByType('Maintenance')[0]
        self.failUnless(self.pm.isSampleVolumeValidForProtocol(mainP1, vol))


# -----------------------------------------------------------------------------

if __name__ == '__main__':
    unittest.main()

# eof
