# 
# TestProtocol.py
#
# Unit tests for the tesla.types.Protocol class
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
import handyxml

import tesla.config
from tesla.exception import TeslaException
from tesla.types.Command import *
from tesla.types.Protocol import Protocol, ProtocolException
from tesla.types.ProtocolConsumable import ProtocolConsumable, EMPTY, NOT_NEEDED
from tesla.types.sample import Sample

# -------------------------------------------------------------------------
# Support function to allow us to generate XML nodes straight from a string
# (rather than from a file)

from xml.dom.ext.reader import PyExpat

def xmlFromString(xmldata):
    """ Parse some XML from a string.
        The return value is the parsed XML as DOM nodes.
    """
    doc = PyExpat.Reader().fromString(xmldata)
    parsedxml = handyxml.HandyXmlWrapper(doc.documentElement)
    return parsedxml

# -------------------------------------------------------------------------

class TestProtocol(unittest.TestCase):

    def setUp(self):
        testFile = 'test_maintenance.xml'
        self.fileName = os.path.join(tesla.config.PROTOCOL_DIR, testFile)
        self.p = Protocol(fileName = self.fileName)

    def test_file(self):
        self.failUnless(self.p, 'Testing file existence and Protocol instance creation')
        self.failUnless(self.fileName == self.p.sourceFile, 'Testing filename')

    def test_nonExistentFile(self):
        # This XML file should not exist and we should catch an exception
        absentFile = 'groundeffect.xml'
        self.assertRaises(ProtocolException, Protocol, absentFile)

    def test_ID(self):
        self.failUnless(self.p.ID)
        self.failUnless(type(self.p.ID) == int, 'Testing ID type (should be int)')

    def test_type(self):
        self.failUnless(self.p.type == 'Maintenance', 'Testing type of protocol')

    def test_parseHeaderNode(self):
        label = 'My header test protocol'
        version = '1999'
        created = '2004-01-07'
        modified = '2004-12-25'
        author = 'Banjo Patterson'

        headerData = \
        """ <header label="%s" version="%s">
                <date created="%s" modified="%s" />
                <author name="%s" />
            </header>
        """ % (label, version, created, modified, author,)
        
        headerNode = xmlFromString(headerData)
        self.p.parseHeaderNode(headerNode)
        self.failUnless(self.p.label == label, 'Testing label field')
        self.failUnless(self.p.version == version, 'Testing version field')
        self.failUnless(self.p.created == created, 'Testing created field')
        self.failUnless(self.p.modified == modified, 'Testing modified field')
        self.failUnless(self.p.author == author, 'Testing author field')
       

    def createHeaderString(self, label):
        # create a header string with the supplied label
        version = '1999'
        created = '2004-01-07'
        modified = '2004-12-25'
        author = 'Banjo Patterson'

        return \
        """ <header label="%s" version="%s">
                <date created="%s" modified="%s" />
                <author name="%s" />
            </header>
        """ % (label, version, created, modified, author,)
        

    def test_parseHeaderNode(self):
        label = 'My header test protocol'
        headerData = self.createHeaderString(label)
        headerNode = xmlFromString(headerData)
        self.p.parseHeaderNode(headerNode)
        self.failUnless(self.p.label == label, 'Testing label field')
        self.failUnless(self.p.version, 'Testing version field')
        self.failUnless(self.p.created, 'Testing created field')
        self.failUnless(self.p.modified, 'Testing modified field')
        self.failUnless(self.p.author, 'Testing author field')       


    def test_labelsWithSpaces(self):
        # Labels with leading/trailing 
        realLabel = 'foobar'
        for label in (' ' + realLabel, realLabel, realLabel + ' '):
            headerData = self.createHeaderString(label)
            headerNode = xmlFromString(headerData)
            self.p.parseHeaderNode(headerNode)
            self.failUnless(self.p.label == realLabel, 'Testing trimmed label field')            
    

    def test_parseConstraintsNode_SeparationConstraints(self):
        quads = 1
        minVol, maxVol = 1000, 10000
        threshold = 2500
        lowVol, highVol = 5000, 10000
        
        data = \
        """
        <constraints>
            <quadrants number="%d" />
            <sampleVolume min_uL="%d" max_uL="%d" />
            <workingVolume sampleThreshold_uL="%d" lowVolume_uL="%d" highVolume_uL="%d" />
        </constraints>
        """ % (quads, minVol, maxVol, threshold, lowVol, highVol)
        
        node = xmlFromString(data)
        self.p.parseConstraintsNode(node)
        self.failUnless(self.p.numQuadrants == quads, 'Testing number of quadrants')
        self.failUnless(self.p.minVol == minVol, 'Testing min sample volume')
        self.failUnless(self.p.maxVol == maxVol, 'Testing max sample volume')
        self.failUnless(self.p.sampleThreshold == threshold, 'Testing sample threshold')
        self.failUnless(self.p.lowWorkingVolume == lowVol, 'Testing low working volume')
        self.failUnless(self.p.highWorkingVolume == highVol, 'Testing high working volume')        

    def test_parseConstraintsNode_MaintenanceConstraints(self):
        quads = 0
        data = \
        """
        <constraints>
            <quadrants number="%d" />
        </constraints>
        """ % (quads)
        node = xmlFromString(data)
        self.p.parseConstraintsNode(node)
        self.failUnless(self.p.numQuadrants == quads, 'Testing number of quadrants')
        # All these should reset to zero
        self.failUnless(self.p.minVol == 0, 'Testing min sample volume')
        self.failUnless(self.p.maxVol == 0, 'Testing max sample volume')
        self.failUnless(self.p.sampleThreshold == 0, 'Testing sample threshold')
        self.failUnless(self.p.lowWorkingVolume == 0, 'Testing low working volume')
        self.failUnless(self.p.highWorkingVolume == 0, 'Testing high working volume')        
        

    def test_parseConstraintsNode_SwappedMinMaxVolumes(self):
        minVol, maxVol = 9000, 500
        data = \
        """
        <constraints>
            <quadrants number="1" />
            <sampleVolume min_uL="%d" max_uL="%d" />
            <workingVolume sampleThreshold_uL="1000" lowVolume_uL="5000" highVolume_uL="9000" />
        </constraints>
        """ % (minVol, maxVol)
        
        node = xmlFromString(data)
        self.p.parseConstraintsNode(node)
        self.failUnless(self.p.minVol == maxVol, 'Testing swapped min sample volume')
        self.failUnless(self.p.maxVol == minVol, 'Testing swapped max sample volume')


    def test_parseConstraintsNode_SwappedWorkingVolumes(self):
        lowVol, highVol = 9000, 5000
        data = \
        """
        <constraints>
            <quadrants number="1" />
            <sampleVolume min_uL="400" max_uL="800" />
            <workingVolume sampleThreshold_uL="1000" lowVolume_uL="%d" highVolume_uL="%d" />
        </constraints>
        """ % (lowVol, highVol)
        
        node = xmlFromString(data)
        self.p.parseConstraintsNode(node)
        self.failUnless(self.p.lowWorkingVolume == highVol, 'Testing swapped low working volume')
        self.failUnless(self.p.highWorkingVolume == lowVol, 'Testing swapped high working volume')


    def test_parseConstraintsNode_InvalidLowVolume(self):
        threshold = 1000
        lowVol, highVol = 500, 8000
        data = \
        """
        <constraints>
            <quadrants number="1" />
            <sampleVolume min_uL="400" max_uL="800" />
            <workingVolume sampleThreshold_uL="%d" lowVolume_uL="%d" highVolume_uL="%d" />
        </constraints>
        """ % (threshold, lowVol, highVol)
        
        node = xmlFromString(data)
        self.p.parseConstraintsNode(node)
        self.failUnless(self.p.sampleThreshold == lowVol , 'Testing new sample threshold')
        self.failUnless(self.p.lowWorkingVolume == lowVol, 'Testing low working volume')
        self.failUnless(self.p.highWorkingVolume == highVol, 'Testing high working volume')


    def test_parseConstraintsNode_InvalidMaxVolume(self):
        maxVol = 12000
        threshold = 1000
        lowVol, highVol = 5000, 8000
        data = \
        """
        <constraints>
            <quadrants number="1" />
            <sampleVolume min_uL="400" max_uL="%d" />
            <workingVolume sampleThreshold_uL="%d" lowVolume_uL="%d" highVolume_uL="%d" />
        </constraints>
        """ % (maxVol, threshold, lowVol, highVol)
        
        node = xmlFromString(data)
        self.p.parseConstraintsNode(node)
        self.failUnless(self.p.maxVol == highVol, 'Testing new max volume')


    def test_parseConstraintsNode_BadQuadrants(self):
        quads = 42
        data = \
        """
        <constraints>
            <quadrants number="%d" />
        </constraints>
        """ % (quads)
        node = xmlFromString(data)
        self.assertRaises(AttributeError, self.p.parseConstraintsNode, node)


    def test_commands(self):
        cmds1 = self.p.getCommands()
        cmds2 = self.p.getCommands()

        self.failUnless(len(cmds1) > 1)
        self.failUnless(len(cmds1) == len(cmds2))
        cmds2.pop()
        self.failUnless(len(cmds1) == len(cmds2) + 1, 'Testing decoupled lists')


    def test_command(self):
        for i in [1, 2]:
            # Expect two commands
            cmd = self.p.getCommand(i)
            self.failUnless(isinstance(cmd, Command), 'Testing Command instance')
        # Shouldn't be able to get commands outside of the first two
        self.assertRaises(ProtocolException, self.p.getCommand, 0)
        self.assertRaises(ProtocolException, self.p.getCommand, 99)


    def test_types(self):
        # Check the type of key members
        intType = type(1)
        self.failUnless(type(self.p.numQuadrants) == intType)
        self.failUnless(type(self.p.minVol) == intType)
        self.failUnless(type(self.p.maxVol) == intType)
        self.failUnless(type(self.p.sampleThreshold) == intType)
        self.failUnless(type(self.p.lowWorkingVolume) == intType)
        self.failUnless(type(self.p.highWorkingVolume) == intType)

        self.failUnless(isinstance(self.p.cmds, list), 'Testing commands class')


    def test_getVolumes(self):
        baseName = 'test_OneLiner.xml'        
        fileName = os.path.join(tesla.config.PROTOCOL_DIR, baseName)
        pro = Protocol(fileName)
        self.failUnless(pro)
          
        volume = 1000
        vols = pro.getVolumes(volume)
        self.failUnless(type(vols) == list)
        self.failUnless(len(vols) == pro.numQuadrants, 'Expect just one quadrant of consumables')
        pcs = vols[0]

        self.failUnless(isinstance(pcs, ProtocolConsumable))
        self.failUnless(pcs.quadrant == 1, 'Expect to be in the first quadrant')

        self.failUnless(pcs.tipBoxRequired, 'We need a tip box')
        self.failUnless(pcs.sampleVesselRequired, 'We need a sample vessel')
        self.failIf(pcs.separationVesselRequired, "We don't need a separation vessel")
        self.failUnless(pcs.cocktailVolume > 0, 'We need cocktail')
        self.failUnless(pcs.bulkBufferVolume == NOT_NEEDED, "We don't need BCCM")
        self.failUnless(pcs.particleVolume == NOT_NEEDED, "We don't need particles")
        self.failUnless(pcs.antibodyVolume == NOT_NEEDED, "We don't need antibody")
        self.failUnless(pcs.lysisVolume == NOT_NEEDED, "We don't need lysis")


    def test_getVolumes2(self):
        baseName = 'TestTransport.xml'
        '''        
        fileName = os.path.join(tesla.config.PROTOCOL_DIR, baseName)
        pro = Protocol(fileName)
        self.failUnless(pro)
          
        volume = 2000
        vols = pro.getVolumes(volume)
        self.failUnless(type(vols) == list)
        pcs = vols[0]
        
        self.failUnless(isinstance(pcs, ProtocolConsumable))
        self.failUnless(pcs.quadrant == 1, 'Expect to be in the first quadrant')

        self.failUnless(pcs.tipBoxRequired, 'We need a tip box')
        self.failUnless(pcs.sampleVesselRequired, 'We need a sample vessel')
        self.failUnless(pcs.separationVesselRequired, 'We need a separation vessel')
        self.failUnless(pcs.wasteVesselRequired, 'We need a waste vessel')
        
        self.failUnless(pcs.cocktailVolume > 0, 'We need cocktail')
        self.failUnless(pcs.bulkBufferVolume > 0, "We need BCCM")
        self.failUnless(pcs.particleVolume == NOT_NEEDED, "We don't need particles")
        self.failUnless(pcs.antibodyVolume == NOT_NEEDED, "We don't need antibody")
        self.failUnless(pcs.lysisVolume == NOT_NEEDED, "We don't need lysis")

        # print pcs.getPrettyString()
        '''
        
    def test_theLot(self):
        '''Test the protocol (and commands) using our test protocol.'''      
        baseName = 'test_protocol.xml'
        fileName = os.path.join(tesla.config.PROTOCOL_DIR, baseName)
        pro = Protocol(fileName)
        self.failUnless(pro)

        sample = Sample(42, 'Test sample', pro.ID, 1250, 1)
        self.failUnless(sample, 'Testing sample creation')

        cmds = pro.getCommands()
        self.failUnless(len(cmds) == pro.numCmds, 'Testing number of commands')

        expectedSeq = 1
        for cmd in cmds:
            self.failUnless(cmd.isLegal(), "Testing that %s command is legal" % (cmd))
            self.failUnless(cmd.label)
            self.failUnless(type(cmd.seq) == int, 'Testing sequence number type')
            self.failUnless(cmd.seq == expectedSeq)
            expectedSeq += 1

            if cmd.isMixType():
                pass
                
            elif cmd.isServiceType():
                if isinstance(cmd, DemoCommand):
                    self.failUnless(type(cmd.iterations) == int)
                    self.failUnless(cmd.iterations > 0)
                    
            elif cmd.isTransportType():
                self.failUnless(cmd.freeAirDispense in [True, False])
                self.failUnless(cmd.useBufferTip in [True, False])
                self.failUnless(cmd.srcVial != None)
                self.failUnless(cmd.destVial != None)
                if cmd.relative:
                    self.failUnless(cmd.proportion > 0.0)
                else:
                    self.failUnless(cmd.absVolume >= 0)
                
            elif cmd.isVolumeType():
                self.failUnless(cmd.srcVial != None)
                self.failUnless(cmd.destVial != None)
                
            elif cmd.isWaitType():
                self.failUnless(cmd.minPeriod >= 0)
                
            else:
                self.fail("%s is an unknown command type" % (cmd))
            
            call = cmd.createCall(sample, pro)
            self.failUnless(cmd)
            #  Note -- should put more testing in here...
 

# -----------------------------------------------------------------------------

if __name__ == '__main__':
    unittest.main()

# eof
