# 
# TipStripper.py
# Tesla.Hardware.TipStripper
#
# Drives a stepper card (via SimpleStep module) to control the Tip Stripper
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
from tesla.hardware.config import HardwareError, LoadSettings
from tesla.hardware.Device import Device
from tesla.hardware.AxisTracker import AxisTracker

import os       #CWJ 
from tesla.PgmLog import PgmLog    # 2011-11-29 sp -- programming logging
import tesla.config         # 2012-01-30 sp -- replace environment variables with configuration file settings
import time

# ---------------------------------------------------------------------------

class TipStripper(Device):
    """Class to drive the Tip Stripper via a SimpleStep stepper card (ie may 
    move between two positions only)."""
    
    # The set of configuration labels used by TipStripper
    # (NB: use lower case, since configParser converts items to lowercase)
    EngagePortLabel = "steppercardengageport"
    DisengagePortLabel = "steppercarddisengageport"
    TraverseTimeLabel = 'maxtraversetime_msec'

    DefaultValues = {EngagePortLabel : '24',
                     DisengagePortLabel : '25',
                     TraverseTimeLabel : '800' }
    
    #-------------------------------------------------------------------------
    def __init__ (self, name, card, configData = {}, moreSettings = {}):
        """Refer to stepper card being controlled"""
        Device.__init__(self, name)

        # Override default settings with specific configurations
        settings = TipStripper.DefaultValues.copy()
        settings.update (moreSettings)
        LoadSettings (settings, configData)
        self.m_Card = card

        self.__m_Tracker = AxisTracker(name, 1)   # Tip stripper is taken as a stepper motor with a 1 step traverse
        
        # 2011-11-29 sp -- added logging
        self.svrLog = PgmLog( 'svrLog' )
        self.logPrefix = 'TS'

        self.__m_EngagePort = int (settings[TipStripper.EngagePortLabel])
        self.__m_DisengagePort = int (settings[TipStripper.DisengagePortLabel])
        self.__m_TraverseTime = int (settings[TipStripper.TraverseTimeLabel])
        self.Disengage()

    def _serviceMethods(self):
        '''Return a list of methods for the service interface'''
        return ['Disengage', 'Engage', 'IsEngaged', ]

    #-------------------------------------------------------------------------
    def Engage(self):
        """Engage Tip Stripper"""
        # 2012-01-30 sp -- replace environment variable with configuration variable
        #if os.environ.has_key('SS_DISABLE_STRIP'):
        if tesla.config.SS_DISABLE_STRIP == 1:
           print('\n>>>> Tip Stripper Disabled! <<<<\n')
           pass           
        else:
           self.__Traverse(self.__m_EngagePort, self.__m_DisengagePort)
        funcReference = __name__ + '.Engage'          # 2011-11-29 sp -- added logging, not accessed within program, logging not tested
        self.svrLog.logDebug('', self.logPrefix, funcReference, "Stripper arm engaged; ports on=%d |off=%d" % (self.__m_EngagePort, self.__m_DisengagePort))   # 2011-11-29 sp -- added logging
        self.__m_IsEngaged = True

    #-------------------------------------------------------------------------
    def Disengage(self):
        """Disengage Tip Stripper"""
        # 2012-01-30 sp -- replace environment variable with configuration variable
        #if os.environ.has_key('SS_DISABLE_STRIP'):
        if tesla.config.SS_DISABLE_STRIP == 1:
           print('\n>>>> Tip Stripper Disabled! <<<<\n')
           pass
        else:   
           self.__Traverse(self.__m_DisengagePort, self.__m_EngagePort)
        funcReference = __name__ + '.Disengage'          # 2011-11-29 sp -- added logging, not accessed within program, logging not tested
        self.svrLog.logDebug('', self.logPrefix, funcReference, "Stripper arm disengaged; ports on=%d |off=%d" % (self.__m_EngagePort, self.__m_DisengagePort))   # 2011-11-29 sp -- added logging
        self.__m_IsEngaged = False

    #-------------------------------------------------------------------------
    def IsEngaged(self):
        """Determine current engagement state of Tip Stripper"""
        return self.__m_IsEngaged

    #-------------------------------------------------------------------------
    def PortsUsed(self):
        """The port being used to communicate with Tip Stripper.
            Returns (Engage,Disengage) ports"""
        return (self.__m_EngagePort, self.__m_DisengagePort)

    def getEngageDriveStatus(self):
        """Returns the engage digital output status for the tip stripper"""
        if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
            self.m_Card.ExtLogger.SetCmdListLog("sendAndCheck()[%s] called from TipStripper.getEngageDriveStatus()"%self.m_Card.prefix, self.m_Card.prefix);
        funcReference = __name__ + '.getEngageDriveStatus'          # 2011-11-29 sp -- added logging, not accessed within program, logging not tested
        self.svrLog.logDebug('xx', self.logPrefix, funcReference, "sendAndCheck()[%s] called from TipStripper.getEngageDriveStatus()"% self.m_Card.prefix)   # 2011-11-29 sp -- added logging
        return self.m_Card.sendAndCheck( 'I24' )[0]

    def getDisengageDriveStatus(self):
        """Returns the disengage digital output status for the tip stripper"""
        if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
            self.m_Card.ExtLogger.SetCmdListLog("sendAndCheck()[%s] called from TipStripper.getDisengageDriveStatus()"%self.m_Card.prefix, self.m_Card.prefix);
        funcReference = __name__ + '.getDisengageDriveStatus'          # 2011-11-29 sp -- added logging, not accessed within program, logging not tested
        self.svrLog.logDebug('xx', self.logPrefix, funcReference, "sendAndCheck()[%s] called from TipStripper.getDisengageDriveStatus()"% self.m_Card.prefix)   # 2011-11-29 sp -- added logging
        return self.m_Card.sendAndCheck( 'I25' )[0]

    def getHomeStatus(self):
        """Returns the home digital input status for the tip stripper"""
        if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
            self.m_Card.ExtLogger.SetCmdListLog("sendAndCheck()[%s] called from TipStripper.getHomeStatus()"%self.m_Card.prefix, self.m_Card.prefix);
        funcReference = __name__ + '.getHomeStatus'          # 2011-11-29 sp -- added logging
        self.svrLog.logDebug('X', self.logPrefix, funcReference, "sendAndCheck()[%s] called from TipStripper.getHomeStatus()"% self.m_Card.prefix)   # 2011-11-29 sp -- added logging
        return self.m_Card.sendAndCheck( 'I26' )[0]

    def getLimitStatus(self):
        """Returns the limit digital input status for the tip stripper"""
        if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
            self.m_Card.ExtLogger.SetCmdListLog("sendAndCheck()[%s] called from TipStripper.getLimitStatus()"%self.m_Card.prefix, self.m_Card.prefix);
        funcReference = __name__ + '.getLimitStatus'          # 2011-11-29 sp -- added logging
        self.svrLog.logDebug('X', self.logPrefix, funcReference, "sendAndCheck()[%s] called from TipStripper.getLimitStatus()"%self.m_Card.prefix)   # 2011-11-29 sp -- added logging
        return self.m_Card.sendAndCheck( 'I27' )[0]

    def getDeviceUsage( self ):
        """Returns the number of full traverses of the axis"""
        return self.__m_Tracker.NbrFullTraverses()
        
    #GetInstrumentAxisStatusSet support methods
    def GetHardwareInfo(self):
        #print '****************stripper arm GetHardwareInfo ', int(round(time.time() * 1000))
        isEngaged = self.IsEngaged()
        #print '****************isEngaged ', int(round(time.time() * 1000))
        homeStatus = self.getHomeStatus()
        #print '****************home status ', int(round(time.time() * 1000))
        limitStatus = self.getLimitStatus()
        #print '****************limit status ', int(round(time.time() * 1000))
        return (isEngaged, homeStatus, limitStatus)

    #-------------------------------------------------------------------------
    #   Private methods
    #-------------------------------------------------------------------------

    #-------------------------------------------------------------------------
    def __Traverse(self, onPort, offPort):
        """Traverse Tip Stripper betwen engaged and diengaged positions"""
        self.m_Card.turnOutputOff (offPort) # Ensure this port is off
        self.m_Card.turnOutputOn (onPort)   # This will start the traverse
        waitCmd = ('W%d' % (self.__m_TraverseTime),)
        self.m_Card.processCmds(waitCmd)    # Wait to ensure that traverse is completed in a reasonable time
        self.m_Card.turnOutputOff (onPort)  # Switch off port to prevent indefinite motor run if opto fails

        # Record the traverse as a single step
        #
        self.__m_Tracker.NoteSteps (1)

# EOF
