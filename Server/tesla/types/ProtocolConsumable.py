# 
# ProtocolConsumable.py
# tesla.types.ProtocolConsumable
#
# Provides a user-level object defining the consumables required in a specific
# quadrant for a sample and protocol
# Samples/protocols requiring more than one quadrant would have a set of these
# objects, one for each quadrant
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

from ipl.utils.validation import validateNumber

from tesla.exception import TeslaException
from tesla.config import NUM_QUADRANTS
from functools import reduce

# -----------------------------------------------------------------------------

NOT_NEEDED = -1                            # Indicates the consumable is not needed
EMPTY = 0                                # Indicates an empty consumable is needed

# -----------------------------------------------------------------------------

class ProtocolConsumable:
    '''Provides a user-level object defining the consumables required in a 
    specific quadrant for a sample and protocol.
    Samples/protocols requiring more than one quadrant would have a set of 
    these objects, one for each quadrant.
    '''

    # ver388 - added dummy sampleVolume - bdr
    def __init__(self, quadrant, cocktailVolume_uL  = NOT_NEEDED,
                    particleVolume_uL               = NOT_NEEDED, 
                    lysisVolume_uL                  = NOT_NEEDED, 
                    antibodyVolume_uL               = NOT_NEEDED,
                    bulkBufferVolume_uL             = NOT_NEEDED,
                    wasteVesselRequired             = False,
                    separationVesselRequired        = False,
                    sampleVesselRequired            = False,
                    sampleVesselVolumeRequired      = False,
                    tipBoxRequired                  = False):
        '''Construct the consumable object
        int quadrant                        Quadrant for these consumables
        double cocktailVolume_uL        Volume (in uL) of cocktail
        double particleVolume_uL        Volume (in uL) of particles/beads
        double lysisVolume_uL                Volume (in uL) of lysis buffer
        double antibodyVolume_uL        Volume (in uL) of antibody cocktail
        double bulkBufferVolume_uL      Volume of bulk buffer required.
        bool wasteVesselRequired        Do we need a waste vessel
        bool separationVesselRequired   Do we need a separation vessel
        bool sampleVesselRequired       Do we need a sample vessel
        bool tipBoxRequired             Do we need a tip box for this quadrant

        Setting the volume to NOT_NEEDED (-1) indicates that the consumable
        is not needed.
        Setting the volume to EMPTY (0) indicates that just the empty 
        consumable is needed.
        By default, none of the containers are needed except for the waste
        container, which can (should!) be empty.
        '''
        if not validateNumber(quadrant, 1, NUM_QUADRANTS):
            raise TeslaException("Invalid quadrant number (%d)" % (quadrant))
        self.quadrant           = quadrant
        self.cocktailVolume     = float(cocktailVolume_uL)
        self.particleVolume     = float(particleVolume_uL)
        self.lysisVolume        = float(lysisVolume_uL)
        self.antibodyVolume     = float(antibodyVolume_uL)
        self.bulkBufferVolume   = float(bulkBufferVolume_uL)

        self.wasteVesselRequired       = wasteVesselRequired
        self.separationVesselRequired  = separationVesselRequired 
        self.sampleVesselRequired      = sampleVesselRequired
        self.sampleVesselVolumeRequired = sampleVesselVolumeRequired 
        self.tipBoxRequired            = tipBoxRequired

    def __volumeValue(self, volume):
        '''Returns a string of the volume or EMPTY or NOT_NEEDED'''
        volumeDict = {EMPTY: 'Empty', NOT_NEEDED: 'Not needed'}
        return "[%s]" % (volumeDict.get(volume, "%0.2f uL" % (volume)))

    def __repr__(self):
        '''String representation of the protocol consumables'''
        volumes = [self.__volumeValue(x) for x in [
                                self.cocktailVolume, self.particleVolume, 
                                self.lysisVolume, self.antibodyVolume, 
                                self.bulkBufferVolume]]
        volumeCat = reduce(lambda x,y: x + y, volumes)
        flags = ['[' + str(x)[:1] + ']' for x in [   self.wasteVesselRequired,
                                            self.separationVesselRequired,
                                            self.sampleVesselRequired,
                                            self.tipBoxRequired]]
        flagsCat = reduce(lambda x,y: x + ' ' + y, flags) 
        return "%d %s %s" % (self.quadrant, volumeCat, flagsCat)

    def getPrettyString(self):
        '''Return a pretty formatted string representation of the 
        ProtocolConsumable object (nicer anyway than the truncated version
        from __repr__.'''
        return """
               Quadrant = %d
        Cocktail volume = %s    
        Particle volume = %s
        Antibody volume = %s
           Lysis volume = %s
            BCCM volume = %s      
      Waste tube needed = %s
 Separation tube needed = %s
     Sample tube needed = %s
         Tip box needed = %s""" % (self.quadrant, 
                self.__volumeValue(self.cocktailVolume),
                self.__volumeValue(self.particleVolume),
                self.__volumeValue(self.antibodyVolume),
                self.__volumeValue(self.lysisVolume),                
                self.__volumeValue(self.bulkBufferVolume),
                str(self.wasteVesselRequired)[0],
                str(self.separationVesselRequired)[0],
                str(self.sampleVesselRequired)[0],
                str(self.tipBoxRequired)[0],
                )
# eof

