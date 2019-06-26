# 
# Platform.py
# tesla.instrument.Platform.Platform
#
# Operates a platform containing a Carousel, and a Z-Theta robot,
# and allows them to interact
#
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

from tesla.exception import TeslaException
from tesla.hardware.config import HardwareConfiguration, gHardwareData, LoadSettings
from tesla.hardware.ThetaAxis import ThetaAxis
from tesla.instrument.Carousel import Carousel
from tesla.instrument.ZThetaRobot import ZThetaRobot
from tesla.instrument.Subsystem import Subsystem
# from tesla.instrument.Calibration import Calibration

from ipl.task.future import *

from ipl.utils.wait import wait_msecs       #CWJ Add

import os # db added 2016/02/10 -- for checking os environment variables

# ---------------------------------------------------------------------------

class PlatformError(TeslaException):
    """Exception for errors related to Platform operation"""
    pass

# ---------------------------------------------------------------------------

class Platform( Subsystem ):
    """Operates a platform containing a Carousel, and a Z-Theta robot, and 
    allows them to interact."""

    # In Platform theta = 0 is the line originating in the carousel, and passing through
    # the robot axis
    #
    CarouselThetaOffsetLabel = 'carouselreferencepointoffset_degrees'    
    RobotThetaOffsetLabel = 'robotthetareferencepointoffset_degrees'
#    RobotZOffsetLabel = 'robotzreferencepointoffset_mm'
    NbrSectorsLabel = 'numberofsectorsoncarousel'
    SectorSizeLabel = 'sizeofcarouselsector_degrees'
    SectorSenseLabel = 'sectororderingsense'
    SectorOffsetLabel = 'homingsector'
    BulkLabel = 'bccm_bulkcontainer_degrees' # db added 2016/02/10 -- for ROBO_DCVJIG_ZT
    
    configData =    {
                    SectorSenseLabel : '-1',
                    SectorOffsetLabel : '1',
#                    RobotZOffsetLabel : '0.0',
                    }

    mandatoryData = (
                    NbrSectorsLabel,
                    SectorSizeLabel,
                    CarouselThetaOffsetLabel,
                    RobotThetaOffsetLabel,
                    )

    # ---------------------------------------------------------------------------
    def __init__( self, name, otherConfigData = {}, referenceData = {} ):
        """Load in the configuration data for the Platform."""
        global gHardwareData

        Subsystem.__init__( self, name )
        
        platformCfg = gHardwareData.Section('Platform')
        settings = Platform.configData.copy()
        settings.update (otherConfigData)
        LoadSettings (settings, platformCfg, Platform.mandatoryData)
        
        self._m_Settings = settings
        self.__m_CarouselThetaOffset  = float(settings[Platform.CarouselThetaOffsetLabel])  
        self.__m_RobotThetaOffset     = float(settings[Platform.RobotThetaOffsetLabel])
        self.__m_RobotZOffset         = 0. #float(settings[Platform.RobotZOffsetLabel])
        self.__m_NbrSectors           = int(settings[Platform.NbrSectorsLabel])
        self.__m_SectorOrdering       = int(settings[Platform.SectorSenseLabel])
        self.__m_SectorScale          = int(settings[Platform.SectorSizeLabel])
        self.__m_SectorOffset         = int(settings[Platform.SectorOffsetLabel])
        self.__m_RendezvousList       = {}
        
        self._LoadReferencePoints (referenceData)
        
        self.robot = ZThetaRobot( "ZThetaRobot" )
        self.carousel = Carousel( "Carousel" )


    def Initialise (self):
        """Perform initialisation of the Platform components"""
        self.robot.Home()
        self.carousel.Home()


    def NbrSectors (self):
        """Give the number of sectors on the platform"""
        return self.__m_NbrSectors


    def SetTravelPosition (self, zTravelPosition):
        """Set the position for the Robot's Z Axis when rotating the Theta axis"""
        self.robot.SetZTravelPosition(zTravelPosition)


    def TravelPosition (self):
        """Obtain the travel position for the Robot's Z Axis when rotating the Theta axis"""
        return self.robot.ZTravelPosition()


    def RobotZOffset (self):
        """Obtain the calibrated offset for the Robot Z-Axis"""
        return self.__m_RobotZOffset


    def MoveToPosition (self, sector, referencePoint, positionIndex = 0):
        """Move carousel and robot so that the robot arm is above a given position.
            sector: the sector of the carousel to access (nominally 90 degrees).
            referencePoint: a reference to the list of positions the carousel and robot need to be set at.
            index: 0, or 1: this indicates which rendezvous position to use. The first is used by default. """

        # Calculate where the components should move
        (rTheta, cTheta) = self._ObtainRendezvous (sector, referencePoint, positionIndex)
                
        # db added 2016/02/10 --- BEGIN SUBROUTINE --- for Automated Pump Calibration/Verification (DCV)
        """This subroutine adds an offset to the robot arm's target theta when the os environment
        variable ROBO_DCVJIG_ZT is defined and the target referencePoint is the bccm container.
            ROBO_DCVJIG_ZT: a user env var with 2 semi-colon-separated parameters: <z-height>;<theta-offset>
            <z-height>: a value [10-148] that determines the robot arm height (default: ~115)
            <theta-offset>: a value that determines the theta offset (recommended: ~147, default: 0). """
        if ("ROBO_DCVJIG_ZT" in os.environ) and (referencePoint == Platform.BulkLabel): # check if conditions are met
            calOffset = float(os.environ["ROBO_DCVJIG_ZT"].split(';')[1]) # grab offset (defined in ROBO_DCVJIG_ZT[1])
            rTheta = rTheta + calOffset # add the offset to bccm_bulkcontainer_degrees (defined in Config\hardware.ini)
        # db added 2016/02/10 --- END SUBROUTINE ---
        
        # Now move the components
        # Move theta non blocking
        self.robot.PrepareZForTravel()
        future = Future( self.robot.SetTheta, rTheta )

        if (cTheta != None):
            self.carousel.SetTheta(cTheta)

        future()


    # 2012-01-31 sp -- added support for barcode reader
    def MoveToBarCodePosition (self, sector, referencePoint, offset, positionIndex = 0):
        """Move carousel and robot so that the robot arm is above a given position.
            sector: the sector of the carousel to access (nominally 90 degrees).
            referencePoint: a reference to the list of positions the carousel and robot need to be set at.
            add offset for barcode reader
            index: 0, or 1: this indicates which rendezvous position to use. The first is used by default. """
        
        self.robot.moveToHomeAndCheck()
        # Calculate where the components should move
        (rTheta, cTheta) = self._ObtainRendezvous (sector, referencePoint, positionIndex)
        
        if (cTheta != None):
            cTheta = cTheta + offset
            self.carousel.SetTheta(cTheta)

    def DelayMoveToPosition (self, sector, referencePoint, positionIndex = 0):    #CWJ Add
        """Move carousel and robot so that the robot arm is above a given position.
            sector: the sector of the carousel to access (nominally 90 degrees).
            referencePoint: a reference to the list of positions the carousel and robot need to be set at.
            index: 0, or 1: this indicates which rendezvous position to use. The first is used by default. """
        
        # Calculate where the components should move
        (rTheta, cTheta) = self._ObtainRendezvous (sector, referencePoint, positionIndex)
        
        # Now move the components
        # Move theta non blocking
        
        self.robot.PrepareZForTravel()

        if (cTheta != None):
            self.carousel.SetTheta(cTheta)
        
        self.robot.SetTheta(rTheta)
        
        #future = Future( self.robot.SetTheta, rTheta )        
        #future()


    def getCarousel(self):
        '''Returns the carousel instance.'''
        return self.carousel

    def getRobot(self):
        '''Returns the Z/theta robot instance.'''
        return self.robot

    # ---------------------------------------------------------------------------
    #   Nominal 'Protected' methods
    # ---------------------------------------------------------------------------

    def _LoadReferencePoints (self, referenceData):
        """Load the intercept points for the robot and carousel.
            NB: since there are two intercept points, a set of 2x2=4 values, separated by commas, is expected
                If one point is given, then it is assumed to be for the zTheta robot only.
        """
        for key in list(referenceData.keys()):
            angleSet = referenceData[key].split(',')
            
            if len(angleSet) == 1:
                self.__m_RendezvousList[key] = ((float(angleSet[0]), None),)
            elif len(angleSet) == 2:
                self.__m_RendezvousList[key] = ((float(angleSet[0]), float(angleSet[1])),)
            elif len(angleSet) == 4:
                self.__m_RendezvousList[key] = ((float(angleSet[0]), float(angleSet[1])), (float(angleSet[2]), float(angleSet[3])))
            else:
                raise PlatformError ("Invalid reference point data for '%s' (data = '%s') should have 1, 2 or 4 numbers separated by commas." % (key, referenceData[key]))

    
    def _ObtainRendezvous (self, sector, referencePoint, index):
        """Obtain the carousel and robot theta settings needed to intersect at
           the given position on the carousel.
                Returns (rTheta, cTheta) - the required theta settings of robot and carousel respectively
               (NB: for tips, this is taken to be the position where the TipStripper may rendezvous also)
               If the reference is not on the carousel, then only the robot coordinate is returned
           """
        if sector < 0 or sector > self.__m_NbrSectors:
            raise PlatformError ("Given sector %d must be range %d - %d" % (sector, 1, self.__m_NbrSectors))

        # Validate the given point: check that it, or the string to which it refers, is in the rendezvous list
        #
        if not referencePoint in list(self.__m_RendezvousList.keys()):
            raise PlatformError ("'%s' is not a reference point." % (referencePoint))

        pointList = self.__m_RendezvousList[referencePoint]
        
        if index < 0 or index > len (pointList):
            raise PlatformError ("index = %d must be in range (0 - %d)" % (index, len (pointList)-1))
            
        (rTheta, cTheta) = self.__m_RendezvousList[referencePoint][index]
        
        rTheta = rTheta + self.__m_RobotThetaOffset
        
        if cTheta == None:
            if sector != 0:
                # Recoverable, but a logic error
                raise PlatformError ("'%s' is not on the carousel. Use sector 0." % (referencePoint))
        else:
            if sector == 0:
                raise PlatformError ("'%s' is on the carousel. Specify a sector in the range 1 - %d." % (referencePoint, self.__m_NbrSectors))

            # swap the directon of the carousel sectors
            if sector == 2:
                sector = 4
            elif sector == 4:
                sector = 2

            sectorTheta = ((self.__m_SectorOrdering *(sector-self.__m_SectorOffset)) % self.__m_NbrSectors)*self.__m_SectorScale + \
                          self.__m_CarouselThetaOffset
            cTheta = cTheta +  sectorTheta

        return (rTheta, cTheta)

# EOF
