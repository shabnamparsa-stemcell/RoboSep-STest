# File: T (Python 2.3)

import unittest
from ipl.types.NamedTuple import NamedTuple, NamedTupleMetaclass

class SampleTuple(tuple):
    __metaclass__ = NamedTupleMetaclass
    names = [
        'batchID',
        'sampleVolume',
        'patientID']


def callBadMember(myTuple):
    return myTuple.foo


class TestNamedTuple(unittest.TestCase):
    
    def testSampleTuple(self):
        batchID = 'B_123'
        volume = 675
        patientID = 'Dawn'
        sample = SampleTuple([
            batchID,
            volume,
            patientID])
        self.failUnless(sample.batchID == batchID)
        self.failUnless(sample.sampleVolume == volume)
        self.failUnless(sample.patientID == patientID)

    
    def testBadName(self):
        sample = SampleTuple([
            'B_1969',
            1250,
            'Willow'])
        self.assertRaises(AttributeError, callBadMember, sample)


if __name__ == '__main__':
    unittest.main()

