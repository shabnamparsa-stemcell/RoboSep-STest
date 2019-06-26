# 
# sample.py
# tesla.types.sample.py
#
# Class to capture sample information, as passed to us from clients
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

#from types import IntType, FloatType

from ipl.utils.validation import validateNumber
from tesla.exception import TeslaException
import tesla.config

# -----------------------------------------------------------------------------

class Sample:
    '''Class to capture sample information, as passed to us from clients.
    Note this class is really just a glorified data structure. - bdr'''

    counter = 0                # Counts the number of samples created so far

    def __init__(self, sampleID, sampleLabel, protocolID, volume_uL, initialQuadrant,
                cocktailLabel = [], particleLabel = [], lysisLabel= '', 
                antibodyLabel = [], sample1Label = [], sample2Label = '',
        bufferLabel = ''):
        '''Create a sample with a sample ID, a label (unique descriptor for the
        sample), the sample volume (in uL), the quadrant that the sample
        protocol should initially start processing in (some protocols will span
        multiple quadrants) and four reagent labels (for reagent tracking)
        The class members, as used by the interface clients are:

        int        ID
        string        label
        int        protocolID
        double        volume (in uL)
        int        initialQuadrant
        string        cocktailLabel
        string        particleLabel
        string        lysisLabel
        string        antibodyLabel
        string        sample1Label      # bdr99
        string        sample2Label      # bdr99
        string  bufferLabel       # Sunny
        '''

        self.ID = sampleID
        self.label = sampleLabel
        self.protocolID = protocolID
    # 2013-01-22 -- sp  added support for multiple quadrants for reagent vials
        #self.cocktailLabel = []
        #self.cocktailLabel.append( cocktailLabel )
        #self.particleLabel = []
        #self.particleLabel.append( particleLabel )
        #self.antibodyLabel = []
        #self.antibodyLabel.append( antibodyLabel )

        self.cocktailLabel = cocktailLabel 
        self.particleLabel = particleLabel 
        self.antibodyLabel = antibodyLabel 

        print("Sample particleLabel ",self.particleLabel)
        print("Sample cocktailLabel ",self.cocktailLabel)
        print("Sample antibodyLabel ",self.antibodyLabel)
        
        self.lysisLabel = lysisLabel
        self.sample1Label = sample1Label    # bdr99
        self.sample2Label = sample2Label    # bdr99
        self.bufferLabel = bufferLabel      # sunny
        
        print("Sample label ",self.sample1Label)

        self.verifySampleVolume(volume_uL)
        self.volume = volume_uL
        self.volume_uL = volume_uL          # An alias for historical purposes

        self.verifyQuadrant(initialQuadrant)
        self.initialQuadrant = initialQuadrant

        self.cocktailFilledVolume = 0 
        self.particleFilledVolume = 0 
        self.antibodyFilledVolume = 0 

        self.__status = 0                    # Undefined status at the start
        self.__etc = ''
        Sample.counter += 1                    # Increment the sample count

    def __str__(self):
        """A nice string representation of the Sample object."""
        return "ID = %d ('%s'). Protocol ID = %d. Vol = %0.2f uL" % \
                (self.ID, self.label, self.protocolID, self.volume)

    def setStatus(self, status):
        '''Set the sample status; this is used internally by the sample tracker'''
        self.__status = status

    def getStatus(self):
        '''Get the sample status'''
        return self.__status

    def verifySampleVolume(self, volume_uL):
        '''Verify that the sample volume is sensible. Throws a TeslaException
        if the value is not numeric or is out of range.'''
        if type(volume_uL) not in [int, float]:
            raise TeslaException('Invalid type for sample volume')
        elif not validateNumber(volume_uL, 0, tesla.config.DEFAULT_WORKING_VOLUME_uL):
            raise TeslaException('Sample volume is out of range')

    def verifyQuadrant(self, quadrantNumber):
        '''Verify that the specified quadrant is valid. Throws a TeslaException
        if it is not.'''
        if not validateNumber(quadrantNumber, 1, tesla.config.NUM_QUADRANTS):
            raise TeslaException('Invalid quadrant number')


# eof

