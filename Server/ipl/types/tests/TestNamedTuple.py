# File: T (Python 2.3)

import unittest
from ipl.types.NamedTuple import NamedTuple, NamedTupleMetaclass

class SampleTuple(tuple, metaclass=NamedTupleMetaclass):
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
        self.assertTrue(sample.batchID == batchID)
        self.assertTrue(sample.sampleVolume == volume)
        self.assertTrue(sample.patientID == patientID)

    
    def testBadName(self):
        sample = SampleTuple([
            'B_1969',
            1250,
            'Willow'])
        self.assertRaises(AttributeError, callBadMember, sample)


if __name__ == '__main__':
    unittest.main()

