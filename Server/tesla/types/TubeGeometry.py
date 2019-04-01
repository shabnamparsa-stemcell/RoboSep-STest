# 
# TubeGeometry.py
# tesla.types.TubeGeometry
#
# Defines a tube geometry, and allows a calculation of fluid height as a function of volume
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

from tesla.exception import TeslaException
from tesla.types.VolumeProfile import *

# -----------------------------------------------------------------------------

class TubeGeometryError(TeslaException):
    pass

# -----------------------------------------------------------------------------

class TubeGeometry:
    """Define the geometric characteristics for a tube sitting in the Tesla carousel"""

    # -----------------------------------------------------------------------------
    def __init__(self, basePosition, deadVolume, tubeProfile):
        """ Define a tube geometry:
                basePosition - the position of the bottom of the tube when placed on the instrument
                deadVolume  - the amount of fluid that is not accessible
                tubeProfile - the volume/height characteristics of the tube
        """
        
        self.__m_BasePosition = basePosition
        self.__m_ProfileList = tubeProfile
        self.__deadVolume = deadVolume

    # -----------------------------------------------------------------------------
    def BasePosition (self):
        """Provide the base position (in mm)."""
        return self.__m_BasePosition
    
    # -----------------------------------------------------------------------------
    def DeadVolume (self):
        """Provide the dead volume (in ul)."""        
        return self.__deadVolume
    
    # -----------------------------------------------------------------------------
    def Capacity (self):
        """Provide the maximum tube capacity."""
        capacity = 0.0

        # Walk through the profiles
        #
        for profile in self.__m_ProfileList:
            capacity = capacity + profile.FullVolume()

        return capacity
    
    # -----------------------------------------------------------------------------
    def HeightAtCapacity (self):
        """Provide the height of the meniscus when tube is at full capacity."""
        height = 0.0

        # Walk through the profiles defining the tube until the volume is accounted for
        #
        for profile in self.__m_ProfileList:
            height = height + profile.FullHeight()

        return height
    
    # -----------------------------------------------------------------------------
    def HeightOfMeniscus (self, volume):
        """Given a volume(ul) of fluid in the tube, determine the height of the meniscus above the base position (in mm)."""
        height = 0.0
        volumeLeft = volume

        # Walk through the profiles defining the tube until the volume is accounted for
        #
        for profile in self.__m_ProfileList:
            if profile.FullVolume() < volumeLeft:
                height = height + profile.FullHeight()
                volumeLeft -= profile.FullVolume()
            else:
                height = height + profile.HeightOf (volumeLeft)
                volumeLeft = 0.0
                break

        if volumeLeft > 0.0:
            raise TubeGeometryError ("given volume (%ful) exceeds tube capacity (%ful)" % (volume, volume - volumeLeft))
                
        return height      

# eof

