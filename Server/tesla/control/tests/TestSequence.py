# 
# TestSequence.py
#
# Unit tests for the Sequence class in tesla.control.Dispatcher.py
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
import itertools

from tesla.control.Dispatcher import Sequence

# -----------------------------------------------------------------------------

class TestSequence(unittest.TestCase):
   
    def test_init(self):
        # Test that we have the right class
        s = Sequence()
        self.failUnless(isinstance(s, Sequence))
        self.failUnless(isinstance(s, list))


    def test_len(self):
        # Test that the lengths of the sequence lists are correct
        s1 = Sequence()
        myList = [1,2,3,4,5,6,7]
        s2 = Sequence(myList)
        self.failUnless(len(s1) == 0)
        self.failUnless(len(s2) == len(myList))


    def test_getSeq(self):
        # Verify that we correctly extract out the sequence number
        cmdSeq = 11
        cmd = "Transport(dstVial=self.instrument.ContainerAt(1, 'samplevial'), srcVial=self.instrument.ContainerAt(1, 'cocktailvial'), volume_ul=100.00, usingFreeAirDispense=False, usingBufferTip=False, id=1, seq=%d)" % (cmdSeq)
        s = Sequence()
        seq = s.getSeq(cmd)
        self.failUnless(seq == cmdSeq)


    def test_invalidSeq(self):
        # Test that we get back a sequence number of zero for a command string
        # that contains an invalid sequence number
        invalidCmd = "Incubate(120, id=1, seq=X)"
        s = Sequence()
        seq = s.getSeq(invalidCmd)
        self.failUnless(seq == 0)


    def test_failedSeq(self):
        # Test that we get back a sequence number of zero for a command string
        # that doesn't contain a sequence number
        invalidCmd = "Doofus(lala_land)"
        s = Sequence()
        seq = s.getSeq(invalidCmd)
        self.failUnless(seq == 0)


    def test_sort(self):
        # Test that sorting works on a list that is already sorted
        events = [ (0, "Transport(dstVial=self.instrument.ContainerAt(1, 'samplevial'), srcVial=self.instrument.ContainerAt(1, 'cocktailvial'), volume_ul=100.00, usingFreeAirDispense=False, usingBufferTip=False, id=1, seq=1)"),
                    (55, "Mix(vial=self.instrument.ContainerAt(1, 'samplevial'), id=1, seq=2)"),
                    (160, 'Incubate(120, id=1, seq=3)'),
                    (280, "Transport(dstVial=self.instrument.ContainerAt(1, 'samplevial'), srcVial=self.instrument.ContainerAt(1, 'beadvial'), volume_ul=50.00, usingFreeAirDispense=False, usingBufferTip=False, id=1, seq=4)"),
                    (329, "Mix(vial=self.instrument.ContainerAt(1, 'samplevial'), id=1, seq=5)"),
                    (434, 'Incubate(60, id=1, seq=6)'),
                    (494, "TopUpVial(dstVial=self.instrument.ContainerAt(1, 'samplevial'), srcVial=None, id=1, seq=7)"),
                    (716, 'Separate(0, id=1, seq=8)'),
                    (716, "Mix(vial=self.instrument.ContainerAt(1, 'samplevial'), id=1, seq=9)"),
                    (821, "Transport(dstVial=self.instrument.ContainerAt(1, 'supernatentvial'), srcVial=self.instrument.ContainerAt(1, 'samplevial'), volume_ul=10000.00, usingFreeAirDispense=False, usingBufferTip=False, id=1, seq=10)"),
                    (866, 'Separate(285, id=1, seq=11)'),
                    (1151, "Transport(dstVial=self.instrument.ContainerAt(1, 'lysisvial'), srcVial=self.instrument.ContainerAt(1, 'supernatentvial'), volume_ul=10000.00, usingFreeAirDispense=False, usingBufferTip=False, id=1, seq=12)"),
                    (1196, 'Separate(0, id=1, seq=13)'),
                    (1196, "ResuspendVial(dstVial=self.instrument.ContainerAt(1, 'supernatentvial'), srcVial=None, id=1, seq=14)"),
                    (1326, "Mix(vial=self.instrument.ContainerAt(1, 'supernatentvial'), id=1, seq=15)"),
                    (1431, "Transport(dstVial=self.instrument.ContainerAt(1, 'lysisvial'), srcVial=self.instrument.ContainerAt(1, 'supernatentvial'), volume_ul=10000.00, usingFreeAirDispense=False, usingBufferTip=False, id=1, seq=16)"),
        ]
        s = Sequence(events)
        s.sort()
        for origEvent, seqEvent in itertools.izip(events, s):
            self.failUnless(origEvent == seqEvent, "Expecting %s" % (str(origEvent)))


    def test_seqOrder(self):
        # Test that events with the same time but different sequence numbers 
        # get sorted into the correct order
        events = [  (716, 'Separate(0, id=1, seq=18)'),
                    (716, "Mix(vial=self.instrument.ContainerAt(1, 'samplevial'), id=1, seq=9)"),
        ]
        s = Sequence(events)
        s.sort()
        self.failUnless(s[1] == (716, 'Separate(0, id=1, seq=18)'))


    def test_timeOrder(self):
        # Test that events with mixed times and sequence numbers get sorted
        # into the correct order
        sortedEvents = [ (0, "Transport(dstVial=self.instrument.ContainerAt(1, 'samplevial'), srcVial=self.instrument.ContainerAt(1, 'cocktailvial'), volume_ul=100.00, usingFreeAirDispense=False, usingBufferTip=False, id=1, seq=1)"),
                    (55, "Mix(vial=self.instrument.ContainerAt(1, 'samplevial'), id=1, seq=2)"),
                    (160, 'Incubate(120, id=1, seq=3)'),
                    (494, "TopUpVial(dstVial=self.instrument.ContainerAt(1, 'samplevial'), srcVial=None, id=1, seq=7)"),
                    (716, 'Separate(0, id=1, seq=8)'),
                    (716, "Mix(vial=self.instrument.ContainerAt(1, 'samplevial'), id=1, seq=9)"),
                    (821, "Transport(dstVial=self.instrument.ContainerAt(1, 'supernatentvial'), srcVial=self.instrument.ContainerAt(1, 'samplevial'), volume_ul=10000.00, usingFreeAirDispense=False, usingBufferTip=False, id=1, seq=10)"),
                    (1326, "Mix(vial=self.instrument.ContainerAt(1, 'supernatentvial'), id=1, seq=15)"),
                    (1431, "Transport(dstVial=self.instrument.ContainerAt(1, 'lysisvial'), srcVial=self.instrument.ContainerAt(1, 'supernatentvial'), volume_ul=10000.00, usingFreeAirDispense=False, usingBufferTip=False, id=1, seq=16)"),
        ]
        mixedEvents = [
                    (1326, "Mix(vial=self.instrument.ContainerAt(1, 'supernatentvial'), id=1, seq=15)"),
                    (1431, "Transport(dstVial=self.instrument.ContainerAt(1, 'lysisvial'), srcVial=self.instrument.ContainerAt(1, 'supernatentvial'), volume_ul=10000.00, usingFreeAirDispense=False, usingBufferTip=False, id=1, seq=16)"),
                    (0, "Transport(dstVial=self.instrument.ContainerAt(1, 'samplevial'), srcVial=self.instrument.ContainerAt(1, 'cocktailvial'), volume_ul=100.00, usingFreeAirDispense=False, id=1, usingBufferTip=False, seq=1)"),
                    (716, "Mix(vial=self.instrument.ContainerAt(1, 'samplevial'), id=1, seq=9)"),
                    (55, "Mix(vial=self.instrument.ContainerAt(1, 'samplevial'), id=1, seq=2)"),                    
                    (494, "TopUpVial(dstVial=self.instrument.ContainerAt(1, 'samplevial'), srcVial=None, id=1, seq=7)"),
                    (821, "Transport(dstVial=self.instrument.ContainerAt(1, 'supernatentvial'), srcVial=self.instrument.ContainerAt(1, 'samplevial'), volume_ul=10000.00, usingFreeAirDispense=False, usingBufferTip=False, id=1, seq=10)"),
                    (716, 'Separate(0, id=1, seq=8)'),
                    (160, 'Incubate(120, id=1, seq=3)'),
        ]
        sorted = Sequence(sortedEvents)         # Our well sorted (good) list
        s = Sequence(mixedEvents)               # The mixed up list
        s.sort()
        self.failUnless(sorted == s, 'Sorted events on time has failed')

# -----------------------------------------------------------------------------
 
if __name__ == '__main__':
    unittest.main()

# eof
