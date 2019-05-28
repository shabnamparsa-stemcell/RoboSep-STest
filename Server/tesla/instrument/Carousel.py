# 
# Carousel.py
# tesla.instrument.Carousel
#
# Drives the Tesla carousel subsystem
# 
# Copyright (c) Invetech Operations Pty Ltd, 2004
# 495 Blackburn Rd
# Mt Waverley, Vic, Australia.
# Phone (+61 3) 9211 7700
# Fax   (+61 3) 9211 7701
# 
# The copyright to the computer program(s) herein is the
# property of Invetech Operations Pty Ltd, Australia.
# The program(s) may be used and/or copied only with
# the written permission of Invetech Operations Pty Ltd
# or in accordance with the terms and conditions
# stipulated in the agreement/contract under which
# the program(s) have been supplied.
# 

from SimpleStep.SimpleStep import SimpleStep

from tesla.exception import TeslaException
from tesla.hardware.config import HardwareConfiguration, gHardwareData, LoadSettings
from tesla.hardware.ThetaAxis import ThetaAxis
from tesla.instrument.Subsystem import Subsystem

import os   # 2011-10-17 sp -- added
import tesla.config         # 2012-01-30 sp -- replace environment variables with configuration file settings

# -----------------------------------------------------------------------------

class CarouselError(TeslaException):
    """Exception for errors related to invalid Carousel settings
        hardware exceptions are translated"""
    pass

# -----------------------------------------------------------------------------

class Carousel(Subsystem):
    """Class to drive the Tesla Carousel"""
    
    SectionName = "Carousel"
    
    PortLabel         = 'steppercardport'
    BoardAddressLabel = 'steppercardaddress'
    # This is the offset from home for the carousel tag (for ease of clearance)
    TagOffsetLabel    = 'tag_offset'                    

    mandatorySettings = (
                        PortLabel,
                        BoardAddressLabel,
                        )

    __configData = {
                    TagOffsetLabel : 10,
                    }
    
    # ---------------------------------------------------------------------------

    def __init__( self, name ):
        """Load in the configuration data for the Carousel.
        NB: since the carousel card also drives the TipStripper, this is also 
        configured and may be accessed via the TipStripper() method.
        """
        global gHardwareData

        Subsystem.__init__( self, name )

        carConfig = Carousel.__configData.copy()
        carSettings = gHardwareData.Section(Carousel.SectionName)
        LoadSettings (carConfig, carSettings, Carousel.mandatorySettings)
        
        cCard = SimpleStep (carConfig[Carousel.BoardAddressLabel], carConfig[Carousel.PortLabel])
        self.__m_Theta = ThetaAxis (Carousel.SectionName, cCard, carSettings)
        self.__m_TagOffset = carConfig[Carousel.TagOffsetLabel]


    def _serviceMethods(self):
        '''Return a list of methods for the service interface'''
        return ['Home', 'MoveToSafePosition', 'SetTheta', 'Theta', 'End']


    # ---------------------------------------------------------------------------


    def Home (self):
        """Home carousel."""
        #home twice to compensate for case where carousel starts really close to home
        self.__m_Theta.Home()
        # 2011-10-17 sp -- remove the need to home twice in newer version of home command
        # 2012-01-30 sp -- replace environment variable with configuration variable
        #if not os.environ.has_key('SS_MINNIE_DEV'):
        if tesla.config.SS_MINNIE_DEV == 0:
            self.__m_Theta.Home()


    def Theta (self):
        """ Obtain current carousel position."""
        return self.__m_Theta.Theta()


    def SetTheta (self, theta):
        """ Move carousel to given position, as specified in rotational 
        degrees.
        Check to see which direction is shortest, and increment the angle 
        rather than move to the absolute position (this allows us to avoid
        problems sensing the home position at 360 degrees!).
        """

        # Determine which direction to move
        #
        minTheta = self.__m_Theta.MinTheta()
        maxTheta = self.__m_Theta.MaxTheta()
        currentTheta = self.__m_Theta.Theta()
        thetaInc = theta - currentTheta

        # By default, go the shorter way
        #
        if thetaInc > ThetaAxis.PI_DEG:
            thetaInc = thetaInc - ThetaAxis.TWOPI_DEG
        elif thetaInc < -ThetaAxis.PI_DEG:
            thetaInc = thetaInc + ThetaAxis.TWOPI_DEG

        #   Ensure we are within limits
        #
        while currentTheta + thetaInc > maxTheta:
            thetaInc = thetaInc - ThetaAxis.TWOPI_DEG

        while currentTheta + thetaInc < minTheta:
            thetaInc = thetaInc + ThetaAxis.TWOPI_DEG

        # Now, we can increment the angle
        #
        self.__m_Theta.IncrementTheta(thetaInc)


    def MoveToSafePosition(self):
        '''Move the carousel to a position that is clear of the home opto for
        each removal of the carousel.'''
        self.SetTheta(self.__m_TagOffset)


    #GetInstrumentAxisStatusSet support methods
    def GetHardwareInfo(self):
        currentTheta = self.__m_Theta.Theta()
        hardwareHomed = self.__m_Theta.getHomeStatus()
        logicallyHomed = int(self.__m_Theta.HomeStep()) == int(self.__m_Theta.Step())
        #print '>>>>>>> %d ----- %d' % (self.__m_Theta.HomeStep(), self.__m_Theta.Step())
        return (currentTheta, hardwareHomed, logicallyHomed)


    # ---------------------------------------------------------------------------
    #   Nominal protected methods
    # ---------------------------------------------------------------------------
    
    def _ThetaAxis (self):
        '''Return a reference to the theta axis.'''
        return self.__m_Theta


# EOF
