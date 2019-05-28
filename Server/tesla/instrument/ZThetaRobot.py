# 
# ZThetaRobot.py
# instrument.ZThetaRobot.ZThetaRobot
#
# Drives the Tesla Z-Theta Robot
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
from tesla.hardware.LinearAxis import LinearAxis
from tesla.instrument.Subsystem import Subsystem

import os       #CWJ

from tesla.PgmLog import PgmLog    # 2011-11-23 sp -- program logging
import tesla.config                 # 2012-01-30 sp -- replace environment variables

# ---------------------------------------------------------------------------

class ZThetaError(TeslaException):
    """Exception for errors related to invalid Z-Theta settings hardware 
    exceptions are translated."""
    pass

# ---------------------------------------------------------------------------

class ZThetaRobot(Subsystem):
    """Class to drive the Tesla Z-Theta Robot"""
    ZAxisSectionName = "Robot_ZAxis"
    ThetaAxisSectionName = "Robot_ThetaAxis"

    #configuration data
    #
    PortLabel       = 'steppercardport'
    BoardAddressLabel = 'steppercardaddress'

    mandatoryZSettings = (
                        PortLabel,
                        BoardAddressLabel,
                        )
    
    mandatoryThetaSettings = (
                        PortLabel,
                        BoardAddressLabel,
                        )
    
    # ---------------------------------------------------------------------------
    def __init__( self, name ):
        """Load in the configuration data for the robot"""
        global gHardwareData
        # 2011-10-17 sp -- set reference for program log
        self.m_Name = name

        Subsystem.__init__( self, name )
        
        zConfig = {}
        zSettings = gHardwareData.Section(ZThetaRobot.ZAxisSectionName)
        LoadSettings (zConfig, zSettings, ZThetaRobot.mandatoryZSettings)
        zCard = SimpleStep (zConfig[ZThetaRobot.BoardAddressLabel], zConfig[ZThetaRobot.PortLabel])
        self.__m_Z = LinearAxis(ZThetaRobot.ZAxisSectionName, zCard, zSettings)

        tConfig = {}        
        tSettings = gHardwareData.Section(ZThetaRobot.ThetaAxisSectionName)
        LoadSettings (tConfig, tSettings, ZThetaRobot.mandatoryThetaSettings)
        tCard = SimpleStep (tConfig[ZThetaRobot.BoardAddressLabel], tConfig[ZThetaRobot.PortLabel])
        self.__m_Theta = ThetaAxis(ZThetaRobot.ThetaAxisSectionName, tCard, tSettings)

        # Ignore travel positions unless they are specified
        self.__m_ZTravelPosition = None
        self.__m_ThetaTravelPosition = None
    
        self.PreHomingDone = False      # CWJ Add
        self.m_Card = zCard             # CWJ Add

        # 2011-11-14 -- sp
        self.svrLog = PgmLog( 'svrLog' )
        self.logPrefix = 'ZR'

    def _serviceMethods(self):
        '''Return a list of methods for the service interface'''
        return ['Home', 'HomeTheta', 'HomeZ', 'SetTheta', 'SetZPosition', ]
    

    def getZAxis( self ):
        '''Returns a reference to the robot's Z axis'''
        return self.__m_Z
    
    def getThetaAxis (self):
        """Obtain the Theta-axis interface."""
        return self.__m_Theta

    def HomeTheta (self):
        """Home the Theta axis. Ensure that the arm is at a safe height"""
        self.PrepareZForTravel()
        self.__m_Theta.Home()

    
    def HomeZ (self):
        """Home the Z axis"""
        # 2012-01-30 sp -- replace environment variable with configuration variable
        # if os.environ.has_key('SS_LEGACY'):
        if tesla.config.SS_LEGACY == 1 or tesla.config.SS_ORCA == 1:
           self.__m_Z.Home()                   
        else:

           self.PreHomingDone = True;           # Disable z-Arm down at powerup. 0.0.1.13

           if not self.PreHomingDone :
              self.__m_Z.HomeInit()
              self.PreHomingDone = True
              print '\n<<<<< None Legacy HomeZ >>>>>\n'
           else :
              self.__m_Z.Home()           
           
    def moveToHomeAndCheck( self , tipstrip = False):
        """Move the Z axis up and check for home. Home only if not home"""
        # 2011-10-17 sp -- set reference for program log
        funcReference = __name__ + '.moveToHomeAndCheck'
        # 2011-12 sp -- log messages added
        self.svrLog.logDebug('', self.logPrefix, funcReference, 'tipstrip=%s' % tipstrip )
        data = [-1, -1]     # 2011-12-7 sp -- default dummy values to prevent error in missing return values
        
        # 2011-10-26 sp -- use previously tested code when environment variable is defined
        # otherwise bypass moving to position 0 and instead use homing command to detect loss of steps
        # 2012-01-30 sp -- replace environment variable with configuration variable
        #if not os.environ.has_key('SS_MINNIE_DEV'):
        if tesla.config.SS_MINNIE_DEV == 0 and tesla.config.SS_ORCA == 0:
            #if os.environ.has_key('SS_LEGACY'):
            if tesla.config.SS_LEGACY == 1:
               self.__m_Z.SetPosition( 0 )
               #if os.environ.has_key('SS_CHATTER'):                            #CWJ Add
               if tesla.config.SS_CHATTER == 1:                            #CWJ Add
                  data = self.m_Card.FindMissingStepsWithHoldingPowerSetting() #CWJ Add
            else:
               if not tipstrip :                          #CWJ Add
                  self.__m_Z.SetPositionWithoutMotion( 0 )#CWJ Add
                  print '\n<<<<< None Legacy >>>>>\n'
               else:                                      #CWJ Add
                  self.__m_Z.SetPosition( 0 )             #CWJ Add

        self.__m_Z.homeIfNotHome(tipstrip)

        # 2012-01-30 sp -- replace environment variable with configuration variable
        #if os.environ.has_key('SS_CHATTER'):          #CWJ Add
        if tesla.config.SS_CHATTER == 1:          #CWJ Add
           return data                                #CWJ Add

    def moveToHomeAndCheck_debug( self , tipstrip = False):
        """Move the Z axis up and check for home. Home only if not home"""
        # 2011-10-17 sp -- set reference for program log
        funcReference = __name__ + '.moveToHomeAndCheck_debug'
        self.svrLog.logDebug('', self.logPrefix, funcReference, 'tipstrip=%s' % tipstrip )

        # 2011-10-26 sp -- use previously tested code when environment variable is defined
        # otherwise bypass moving to position 0 and instead use homing command to detect loss of steps
        # 2012-01-30 sp -- replace environment variable with configuration variable
        #if not os.environ.has_key('SS_MINNIE_DEV'):
        if telsa.config.SS_MINNIE_DEV == 0 and tesla.config.SS_ORCA == 0:
            #if os.environ.has_key('SS_LEGACY'):
            if telsa.config.SS_LEGACY == 1:
               self.__m_Z.SetPosition( 0 )
            else:
               if not tipstrip :                          #CWJ Add
                  self.__m_Z.SetPositionWithoutMotion( 0 )#CWJ Add
                  print '\n<<<<< None Legacy >>>>>\n'
               else:                                      #CWJ Add
                  self.__m_Z.SetPosition( 0 )             #CWJ Add

        self.__m_Z.homeIfNotHome(tipstrip)

    def Home( self ):
        """Home all robot components. Ensure that Z axis is homed first """
        self.HomeZ()
        self.HomeTheta()


    def SetTheta (self, degrees, bNonBlocking = False):
        """Rotate arm to given position, after ensuring that it is in the 
        travel position."""
        self.PrepareZForTravel()
        self.__m_Theta.SetTheta( degrees, bNonBlocking )


    def WaitForThetaCompletion( self ):
        """Wait for completion for the non blocking move on the theta axis"""
        self.__m_Theta.WaitForCompletion()

    def SetZPosition (self, zPosition, tipStrip = 0):
        """Move the Z-axis position to the given value."""
        self.__m_Z.SetPosition (zPosition, tipStrip)

    def SetZPositionWithoutMotion (self, zPosition):        #CWJ June23,2009
        """Move the Z-axis position to the given value."""  #
        self.__m_Z.SetPositionWithoutMotion (zPosition)     #

    def SetZTravelPosition (self, zTravelPosition):
        """Define the Z-axis position to adopt prior to rotating the theta 
        axis."""
        # Ensure that result is within limits
        position = min(zTravelPosition, self.Z().MaxPosition())
        position = max(position, self.Z().MinPosition())
        self.__m_ZTravelPosition = position


    def ZTravelPosition (self):
        """Obtain the current Z-axis travel position.
        Returns 'None' if the travel position has not been specified."""
        return self.__m_ZTravelPosition


    def PrepareZForTravel(self, tipStrip = 0):
        """If a travel position is specified, then move Z axis to it."""
        if self.__m_ZTravelPosition != None:
            self.SetZPosition(self.__m_ZTravelPosition, tipStrip)


    # --- accessor functions of use for testing, debugging, etc. ---

    def Z (self):
        """Obtain the Z-axis interface."""
        return self.__m_Z


    def Theta (self):
        """Obtain the Theta-axis interface."""
        return self.__m_Theta

    def ZPosition(self):
        '''Returns the current Z position.'''
        return self.Z().Position()


    #GetInstrumentAxisStatusSet support methods
    def GetZHardwareInfo(self):
        currentZ = self.__m_Z.Position()
        hardwareHomed = self.__m_Z.getHomeStatus()
        logicallyHomed = int(self.__m_Z.HomeStep()) == int(self.__m_Z.Step())
        #print '>>>>>>> %d ----- %d' % (self.__m_Z.HomeStep(), self.__m_Z.Step())
        return (currentZ, hardwareHomed, logicallyHomed)
    def GetThetaHardwareInfo(self):
        currentTheta = self.__m_Theta.Theta()
        hardwareHomed = self.__m_Theta.getHomeStatus()
        logicallyHomed = int(self.__m_Theta.HomeStep()) == int(self.__m_Theta.Step())
        #print '>>>>>>> %d ----- %d' % (self.__m_Theta.HomeStep(), self.__m_Theta.Step())
        return (currentTheta, hardwareHomed, logicallyHomed)

# EOF
