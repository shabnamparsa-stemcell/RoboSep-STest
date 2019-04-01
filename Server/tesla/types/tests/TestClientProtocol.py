# 
# TestClientProtocol.py
#
# Unit tests for the tesla.types.ClientProtocol module
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
from tesla.types.ClientProtocol import ClientProtocol, ClientProtocolException
from tesla.config import DEFAULT_WORKING_VOLUME_uL 

# ---------------------------------------------------------------------------

class TestClientProtocol(unittest.TestCase):
  
    def testDefault(self):
	p = ClientProtocol(ClientProtocol.POSITIVE, 'CD18', 250, 10000)
	self.failUnless(p)

    def test_protocolTypes(self):
	# Test the legal protocol types that we can use to create 
	# ClientProtocol instances
	for protocolType in ClientProtocol.LEGAL_CLASSES:
	    # These creations should all succeed
	    cp = ClientProtocol(protocolType, "P-%s" % (protocolType), 250, 10000)
	# But this one should fail
	BOGUS_PROTOCOL = 'Pnau'
	self.assertRaises(ClientProtocolException, ClientProtocol, BOGUS_PROTOCOL,
			    'BAD PROTOCOL', 250, 10000)

    def testBadVolumes(self):
	self.assertRaises(ClientProtocolException, ClientProtocol, \
		ClientProtocol.NEGATIVE, 'CD8', 5000, 400)

    def testTooLargeVolume(self):
	bigVol = DEFAULT_WORKING_VOLUME_uL + 1
	self.assertRaises(ClientProtocolException, ClientProtocol, \
		ClientProtocol.NEGATIVE, 'CD42', 5000, bigVol)

    def testBadSepClass(self):
	# The separation class should be 0, 1 or 2 (+ve, -ve or whole blood)
	# So the creation of this Protocol instance should raise an exception
	self.assertRaises(ClientProtocolException, ClientProtocol, \
		'Nonexistent protocol', 'CD4', 250, 10000)

    def testQuadrants(self):
	quads = [1, 2, 3, 4]
	for quadCount in quads:
	   p = ClientProtocol(ClientProtocol.POSITIVE, "CD%02d" % (quadCount), \
		   250, 10000, numQuadrants = quadCount)
	   self.failUnless(p.numQuadrants == quadCount, 'Testing # quadrants')

        # For a SHUTDOWN protocol, we should be able to specify zero quadrants
        p = ClientProtocol(ClientProtocol.SHUTDOWN, "Shutdown", 0, 0, 0)
        self.failUnless(p)


    def testBadQuadrants(self):
	badQuads = [-1, 5, 42]
	for quadCount in badQuads:
	    self.assertRaises(ClientProtocolException, ClientProtocol, \
		    ClientProtocol.POSITIVE, "P-%d" % (quadCount), 250, 10000, quadCount)

    def testLabelClash(self):
	label = 'R2D2'
	p1 = ClientProtocol(ClientProtocol.POSITIVE, label, 250, 10000)

	maxVol = 7500
	p2 = ClientProtocol(ClientProtocol.POSITIVE, label, 500, maxVolume_uL = maxVol)
	self.failUnless(p2.maxVol == maxVol, 'Testing updated protocol')

# -----------------------------------------------------------------------------

if __name__ == '__main__':
    unittest.main()

# eof
