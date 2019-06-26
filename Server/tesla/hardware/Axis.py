# 
# Axis.py
# tesla.Hardware.Axis
#
# Abstract class driving a stepper card (via SimpleStep module)
# Moves motor in step units, conversions (mm, degrees) to be handled by derived classes
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

import re
import os     #CWJ Add

from SimpleStep.SimpleStep import SimpleStep, SimpleStepError
from tesla.hardware.config import HardwareError, LoadSettings
from tesla.hardware.Device import Device
from tesla.hardware.AxisTracker import AxisTracker

from tesla.PgmLog import PgmLog    # 2011-11-23 sp -- programming logging
import tesla.config         # 2012-01-30 sp -- replace environment variables with configuration file settings

import tesla.DebuggerWindow          # 2012-04-10 sp

# ---------------------------------------------------------------------------

class AxisError(HardwareError):
    """General exception for errors in classes derived from Axis"""
    pass

# ---------------------------------------------------------------------------

class Axis(Device):
    """Class to drive a SimpleStep stepper card in one dimension"""

    # The set of configuration labels used by Axis
    # (NB: use lower case, since configParser converts items to lowercase)
    #
    MotorHalfStepLabel = "halfstepexponent"
    MotorPowerLabel = "standardpowerprofile"
    MotorStepVelocityLabel = "standardvelocityprofile"
    MotorHomingPowerLabel = "homingpowerprofile"
    MotorIdlePowerLabel = "idlepowerprofile"
    MotorHomingStepVelocityLabel = "homingvelocityprofile"
    MotorIdlePowerLabel = "idlepowerprofile"
    HomeStepLabel = "homestep"
    HomingOvershootOnLabel = "usinghomingovershootcorrection"
    HomingOvershootLabel = "numberofhomingovershootcorrectiontries"
    HomeBackOffStepsLabel = "homebackoffsteps"

    # 2011-10-17 sp -- added
    # 2012-01-30 sp -- replace environment variable with configuration variable
    #if os.environ.has_key('SS_MINNIE_DEV'):  # 2011-11-14 -- sp
    if tesla.config.SS_MINNIE_DEV == 1:
        PinFlagPreHomingPowerLabel = "pinflagprehomingpowerprofile"       # used predominantly by z-axis with pin-flag design
        # used predominantly by z-axis to determine discrepancy in number of steps for tip-strip
        StepsSlippedThreshold = "stepsslippedthreshold"
        HomingDirectionLabel = "homingdirection"
        HomeLimitSensorLabel = "homelimitsensor_configuration"
        HomeTimeOutLabel = "hometimeout_milliseconds"

    m_homeBackOffSteps = 2000
    
    # 2012-01-30 sp -- replace environment variable with configuration variable
    #if not os.environ.has_key('SS_MINNIE_DEV'):
    if tesla.config.SS_MINNIE_DEV == 0:
        DefaultSettings = { MotorHalfStepLabel: '4',
                        MotorPowerLabel: '140,100,0',
                        MotorStepVelocityLabel : '2200, 4000, 5',
                        MotorHomingPowerLabel: '140,100,0',
                        MotorHomingStepVelocityLabel : '2200, 4000, 5',
                        MotorIdlePowerLabel : '140, 50, 0',
                        HomeStepLabel : '0',
                        HomingOvershootOnLabel : '0',
                        HomingOvershootLabel : '40',
                        HomeBackOffStepsLabel : '2000',

                        }
    else:
        DefaultSettings = { MotorHalfStepLabel: '4',
                        MotorPowerLabel: '140,100,0',
                        MotorStepVelocityLabel : '2200, 4000, 5',
                        MotorHomingPowerLabel: '140,100,0',
                        MotorHomingStepVelocityLabel : '2200, 4000, 5',
                        MotorIdlePowerLabel : '140, 50, 0',
                        HomeStepLabel : '0',
                        HomingOvershootOnLabel : '0',
                        HomingOvershootLabel : '40',
                        HomeBackOffStepsLabel : '2000',

                        # 2011-10-17 sp -- added
                        PinFlagPreHomingPowerLabel: '20,20,0',
                        StepsSlippedThreshold: 500,
                        HomingDirectionLabel : '-',
                        HomeLimitSensorLabel : '10',
                        HomeTimeOutLabel : '10000'
                        }

   # -------------------------------------------------------------------------
    
    def __init__ (self, name, card, maxStep, configData = {}, moreSettings = {}):
        '''Refer to stepper card being controlled.
        name:
        card: the SimpleStep instance through which card is to be driven.
        maxStep: the maximum number of steps permitted (calculated by derived classes)
        configData: a dictionary of configuration settings, read in from a file
        moreDefaults: default settings, passed in from a derived class
        '''
        Device.__init__(self, name = name)

        # Load the settings
        settings  = Axis.DefaultSettings.copy()
        settings.update (moreSettings)
        LoadSettings (settings, configData)
        self.ValidateSettings()            


        # Initialise variables to the defaults
        self.m_Name = name
        self.logPrefix = name
        self.m_Settings = settings
        self.m_Card = card
        self.m_IsHomed = False     # Flag is set after successful homing, and reset on stepper loss report
        self.m_MaxStep = maxStep   # This is an imposed limit, physical travel may be greater
        self.__PositionTracking = 0        # Our internal tracking position value
        self.__PositionTracking = 0        # Our internal tracking position value

        # 2011-10-17 sp -- set reference for program log
        funcReference = __name__ + '.__init__'
        self.svrLog = PgmLog( 'svrLog' )
        self.svrLog.logVerbose('', self.logPrefix, funcReference, 'starting' )
        # 2012-01-30 sp -- replace environment variable with configuration variable
        #if os.environ.has_key('SS_MINNIE_DEV'):  # 2011-11-14 -- sp
        if tesla.config.SS_MINNIE_DEV == 1:
            # 2011-10-17 SP  added variable for using configuration file to determine homing commands to use for each axis
            self.m_InitMotorCmd = 'N%s0%s' % (self.m_Settings[Axis.HomingDirectionLabel], self.m_Settings[Axis.HomeLimitSensorLabel])
            self.m_HomeMotorCmd = 'N%s1%s' % (self.m_Settings[Axis.HomingDirectionLabel], self.m_Settings[Axis.HomeLimitSensorLabel])
            self.m_Card.setMotorCmd( self.m_InitMotorCmd, self.m_HomeMotorCmd )
            self.m_TimeOut_MilliSeconds = int( self.m_Settings[Axis.HomeTimeOutLabel] )
            self.m_Card.setHomeTimeOut( self.m_TimeOut_MilliSeconds )
            # added axis/board initialization before issuing any commands (e.g. in ResetStepSpeedProfile)
            self._ProcessCmdList ([self.m_InitMotorCmd])
            self.ResetPreHomingPower()

        self.ResetHomeStep()                            
        self.ResetHalfStep()                            
        self.ResetPower()                               
        self.ResetHomingPower()
        self.ResetIdlePower()
        self.ResetStepSpeedProfile()                    
        self.ResetHomingStepSpeedProfile()

        self.m_homeBackOffSteps = int( self.m_Settings[Axis.HomeBackOffStepsLabel] )

        # Track usage
        self.m_Tracker = AxisTracker (name, maxStep)
        self.m_CheckingHome = True        # by default

        # Log the axis and associated version of stepper card firmware
        firmWareVersion = self.GetFirmwareVersion()     # 2011-11-25 sp -- assigned to variable
        Device.logger.logInfo("Axis: %s (%s), Board & FW ver: %s" % 
                              (self.m_Name, self.m_Card.prefix, firmWareVersion))
        # 2011-11-25 sp -- add logging
        self.svrLog.logInfo('', self.logPrefix, funcReference, "completed init; Axis=%s |Firmware V%s" %
                              (self.m_Card.prefix, firmWareVersion) )
        
        self.logger = Device.logger     #CWJ Add
        if( tesla.config.SS_DEBUGGER_LOG == 1 ):
          ssLogDebugger = tesla.DebuggerWindow.GetSSTracerInstance()        
          cardAxis = card.board + str(card.address)
          param = { 
                    'Axis' :cardAxis,
                    'configData':configData,
                    'moreConfig':moreSettings
                  }
          ssLogDebugger.SetParameterFromAxis(param)                
        
    def _GetConfigData (self, type, configFile = 'C:\\Program Files\\STI\\RoboSep\\config\\homeConfig.ini'):
        import os
        values = []
        try:
          f = open(configFile)
          content = f.readlines()
          f.close()
          idx = -1
          for i in range(len(content)):
            if type in content[i]:
                idx = i + 1
                break

          if idx < len(content):
              str = content[idx].replace("\n","")
              str = str.replace(" ","")
              components = str.split(",")
              for i in range(len(components)):
                  components[i] = components[i].replace("~",",")
                  values += components[i].split("|")
        except:
            pass
        return values
        
    # --- movement methods ---

    def HomeInit( self ):
        """Homing (override)"""
        # Put on record
        # NB: we don't necessarily know the distance to home, but assuming the calculation
        #   probably causes less error than not.
        if self.debug: print(self, "Axis::Home")
        if self.m_IsHomed:
            stepsToHome = self.HomeStep() - self.Step()
        else:
            stepsToHome = self.MaxStep()

        self._PreHome()

        self.SetPositionTracking(self.HomeStep())   # 2011-12-12 sp -- move up from after try statement
        # Repeat home action until successful (it *should* only take one action, but...!)
        try:
            self.m_IsHomed = False
            while not self.m_IsHomed:
                self._Home()
                self.m_IsHomed = self._IsAtHome()
                self.m_Tracker.NoteHomeAction( stepsToHome, self.m_IsHomed )
                stepsToHome = 0
        finally:
            self._CompleteHoming()
        # self.SetPositionTracking(self.HomeStep()) # 2011-12-12 sp -- move up before try statement

    def Home( self , bSkipTrip = False):
        # 2011-10-17 sp -- set reference for program log
        funcReference = __name__ + '.Home'
        """Homing (override)"""
        # Put on record
        # NB: we don't necessarily know the distance to home, but assuming the calculation
        #   probably causes less error than not.
        if self.debug: print(self, "Axis::Home")
        if self.m_IsHomed:
            stepsToHome = self.HomeStep() - self.Step()
        else:
            stepsToHome = self.MaxStep()

        self.SetPositionTracking(self.HomeStep())   # 2011-12-12 sp -- move up from after try statement
        # Repeat home action until successful (it *should* only take one action, but...!)
        try:
            self.m_IsHomed = False
            while not self.m_IsHomed:
                if (bSkipTrip == True):
                   self._Home(bSkipTrip)                
                else:
                   self._Home()
                self.m_IsHomed = self._IsAtHome()
                self.m_Tracker.NoteHomeAction( stepsToHome, self.m_IsHomed )
                stepsToHome = 0
        finally:
            self._CompleteHoming()
        # self.SetPositionTracking(self.HomeStep()) # 2011-12-12 sp -- move up before try statement
        


    def homeIfNotHome( self , tipstrip = False):
        # 2011-10-17 sp -- set reference for program log
        funcReference = __name__ + '.homeIfNotHome'

        isHome = self._IsAtHome()
        """If the home flag is not obscured then perform a home action"""
        if not tipstrip and not isHome:
            # 2012-01-30 sp -- replace environment variable with configuration variable
            #if os.environ.has_key('SS_LEGACY'):
            if tesla.config.SS_LEGACY == 1:
                self.svrLog.logDebug('X', self.logPrefix, funcReference, "--->Chatter may occur!")   # 2011-11-29 sp -- added logging
                if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                    self.m_Card.ExtLogger.SetCmdListLog("--->Chatter may occur!", self.m_Card.prefix)
                    self.m_Card.ExtLogger.CheckSystemAvail()
                    self.m_Card.ExtLogger.DumpHistory()
            self.Home()

        if tipstrip and not isHome:
            # 2011-10-26 sp -- use previously tested code when environment variable is defined
            # 2012-01-30 sp -- replace environment variable with configuration variable
            #if not os.environ.has_key('SS_MINNIE_DEV'):
            if tesla.config.SS_MINNIE_DEV == 0:
                val = self.m_Card.FindMissingSteps()
                self.svrLog.logError('X', self.logPrefix, funcReference, "Tip strip failed! [%s steps from the %s Home sensor]"%(val,self.m_Card.prefix))   # 2011-11-29 sp -- added logging
                if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                    self.m_Card.ExtLogger.SetCmdListLog("Tip strip failed! [%s steps from the %s Home sensor]"%(val,self.m_Card.prefix), self.m_Card.prefix)
                    self.m_Card.ExtLogger.CheckSystemAvail()
                    self.m_Card.ExtLogger.DumpHistory()
                raise AxisError ('Tip strip failed')
            # 2011-10-26 sp -- use new homing function
            else:
                # postitionBeforeHome = self.m_Card.GetPositionTracking();      # using value tracked by software
                postitionBeforeHome = self.m_Card.getPosition();        # using on-board register value
                self.Home()
                val = self.m_Card.getStepsToHome()
                slippedSteps = postitionBeforeHome - val;
                self.svrLog.logDebug('S', self.logPrefix, funcReference, 'Steps slipped going home=%d' % ( slippedSteps ) )
                if( abs( slippedSteps ) > int( self.m_Settings[Axis.StepsSlippedThreshold] ) ):
                    self.svrLog.logError('', self.logPrefix, funcReference, 'Steps slipped exceeded threshold=%s' % ( self.m_Settings[Axis.StepsSlippedThreshold] ) )
                    if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                        self.m_Card.ExtLogger.SetCmdListLog("Tip strip failed! [%s steps from the %s Home sensor]"%(val,self.m_Card.prefix), self.m_Card.prefix)
                        self.m_Card.ExtLogger.CheckSystemAvail()
                        self.m_Card.ExtLogger.DumpHistory()
                    raise AxisError ('Tip strip failed')


    def End(self):
        """Move to maximum limit"""
        self.SetStep(self.MaxStep(), False, 0)


    def SetStep(self, step, bNonBlocking = False, tipStrip = 0):
        '''Move motor to a given step.'''
        # The new code uses a relative move (R command), rather than an
        # absolute move (M command) as the M command will stop early if the
        # home opto suffers a false trigger. The R command can be told to 
        # ignore the home opto during a move.
        relativeMove = step - self.GetPositionTracking()

        self.IncrementStep(relativeMove, bNonBlocking, tipStrip)

    def SetStepWithoutMotion(self, step, bNonBlocking = False):             #CWJ Add
        '''Move motor to a given step.'''                                   #CWJ Add
        # The new code uses a relative move (R command), rather than an     #CWJ Add
        # absolute move (M command) as the M command will stop early if the #CWJ Add
        # home opto suffers a false trigger. The R command can be told to   #CWJ Add
        # ignore the home opto during a move.                               #CWJ Add
        relativeMove = step - self.GetPositionTracking()                    #CWJ Add
                                                                            #CWJ Add
        self.IncrementStepWithoutMotion(relativeMove, bNonBlocking)         #CWJ Add

    def IncrementStep( self, stepInc, bNonBlocking = False, tipStrip = 0 ):
        """Move motor by given step increment"""
        iStepInc = int(stepInc)
        # 2011-10-17 sp -- set reference for program log
        funcReference = __name__ + '.IncrementStep'
        
        if iStepInc == 0:
            # Ignore zero increment case: it can cause bad status error in card
            return
        else: 
            # Validate new value before setting it
            stepPosition = self.Step() + iStepInc
            self._Validate(stepPosition)

            # Record the move in the device tracker and then move
            self.m_Tracker.NoteSteps (stepInc)
            self.SetPositionTracking(stepPosition)
            if bNonBlocking:
                self._MoveNonBlocking( self._IncrementCmd(stepInc))
            else:
                self._Move( self._IncrementCmd(stepInc), bNonBlocking, tipStrip )
        # 2011-11-14 -- sp
        self.svrLog.logDebug('', self.logPrefix, funcReference, 'Steps moved=%d |HW position=%d |SW position=%d |SW home=%s' %
                    ( iStepInc, self.m_Card.getPosition(), stepPosition, self.HomeStep() ) )
                
    def IncrementStepWithoutMotion( self, stepInc, bNonBlocking = False ):      #CWJ Add
        """Move motor by given step increment"""                                #CWJ Add
        iStepInc = int(stepInc)                                                 #CWJ Add
        # 2011-10-17 sp -- set reference for program log
        funcReference = __name__ + '.IncrementStepWithoutMotion'

        if iStepInc == 0:                                                       #CWJ Add
            # Ignore zero increment case: it can cause bad status error in card #CWJ Add
            return                                                              #CWJ Add    
        else:                                                                   #CWJ Add  
            # Validate new value before setting it                              #CWJ Add  
            stepPosition = self.Step() + iStepInc                               #CWJ Add
            self._Validate(stepPosition)                                        #CWJ Add
                                                                                #CWJ Add  
            # Record the move in the device tracker and then move               #CWJ Add  
            self.m_Tracker.NoteSteps (stepInc)                                  #CWJ Add
            self.SetPositionTracking(stepPosition)                              #CWJ Add
        # 2011-11-14 -- sp
        self.svrLog.logDebug('', self.logPrefix, funcReference, 'Steps moved=%d |HW position=%d |SW position=%d |SW home=%s' %
                    ( iStepInc, self.m_Card.getPosition(), stepPosition, self.HomeStep() ) )

    # --- support methods ---
    
    def ValidateSettings(self):
        """Override this to validate configuration settings as they are read in.
            Throw a HardwareSettingsError if a problem is encountered"""
        pass


    def GetPositionTracking(self):
        '''Return our internal tracking of the axis position.'''
        return self.__PositionTracking


    def SetPositionTracking(self, newPosition):
        '''Set the internal tracking position.'''
        self.__PositionTracking = int(newPosition)


    def GetFirmwareVersion(self):
        '''Return the firmware version for the card attached to this axis.'''
        boardInfo = ''
        # Grab the last three digits from the string (the rest *seem* to 
        # just be zeros
        funcReference = __name__ + '.GetFirmwareVersion' # 2011-11-25 sp -- added
        for param in [0, 1, 4, 5,9]:
            self.svrLog.logVerbose('X', self.logPrefix, funcReference, 'to call getBoardInfo param=%d' % param )    # 2011-11-25 sp -- added
            if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                self.m_Card.ExtLogger.SetLog("getBoardInfo()[%s] from Axis.GetFirmwareVersion()"%self.m_Card.prefix, self.m_Card.prefix)
            boardInfo += self.m_Card.getBoardInfo(param)[-3:] + ':'
        # Cut off that trailing (& redundant) colon
        return boardInfo[:-1]


    def IsHomed(self):
        """Determine whether motor needs homing"""
        return self.m_IsHomed


    def MaxStep(self):
        """Return the maximum permitted step count"""
        return self.m_MaxStep


    def Step(self):
        """Obtain the current step value"""
        step = self.GetPositionTracking()
        self._Validate(step)
        return step


    def HomeStep(self):
        """Obtain the home step value.
        By default, this is 0, but it may be reconfigured if home optos are not
        at start of travel."""
        return self.__m_HomeStep

    def HomeBackOffSteps(self):
        """Obtain number of steps to back off during home when the home
        flag is already obscured"""
        return self.m_homeBackOffSteps

    def SetHomeStep(self, homeStep):
        """Override the home step value."""
        self.__m_HomeStep = homeStep


    def ResetHomeStep(self):
        """Reset the home step value to the configured setting."""
        self.SetHomeStep (int (self.m_Settings[Axis.HomeStepLabel]))
        

    def StepSpeedProfile (self):
        """Obtain the current step speed profile."""
        return (self.m_StepSpeedBegin, self.m_StepSpeedEnd, self.m_StepSpeedSlope)


    def SetStepSpeedProfile (self, newBeginSpeed, newEndSpeed, newSlope):
        """Sets the velocity profile for subsequent moves."""
        self.m_StepSpeedBegin = newBeginSpeed
        self.m_StepSpeedEnd   = newEndSpeed
        self.m_StepSpeedSlope = newSlope

        # Update card settings immediately
        self._ProcessCmdList (self._SpeedCmd(self.StepSpeedProfile()))


    def SetStepSpeedProfileFromString (self, profile):
        """Sets the velocity profile for subsequent moves using a string as input."""
        self._SetStepSpeedProfile (self._ParseProfile (profile))


    def ResetStepSpeedProfile (self):
        """Resets the velocity profile back to the default."""
        self.SetStepSpeedProfileFromString (self.m_Settings[Axis.MotorStepVelocityLabel])


    def HomingStepSpeedProfile (self):
        """Obtain the current homing step speed profile."""
        return (self.m_HomingStepSpeedBegin, self.m_HomingStepSpeedEnd, self.m_HomingStepSpeedSlope)


    def SetHomingStepSpeedProfile (self, newBeginSpeed, newEndSpeed, newSlope):
        """Sets the velocity profile for subsequent moves."""
        self.m_HomingStepSpeedBegin = newBeginSpeed
        self.m_HomingStepSpeedEnd = newEndSpeed
        self.m_HomingStepSpeedSlope = newSlope


    def ResetHomingStepSpeedProfile (self):
        """Resets the homing velocity profile back to the default."""
        (b,e,s) = self._ParseProfile (self.m_Settings[Axis.MotorHomingStepVelocityLabel])
        self.SetHomingStepSpeedProfile (b,e,s)


    def HalfStep (self):
        """Obtain the current half step setting."""
        return self.m_HalfStep


    def SetHalfStep (self, newSetting):
        """Set the current half step setting."""
        self.m_HalfStep = newSetting


    def ResetHalfStep (self):
        """Resets the velocity profile back to the default."""
        self.SetHalfStep (int (self.m_Settings[Axis.MotorHalfStepLabel]))


    def Power (self):
        """Obtain the current power settings."""
        return (self.m_Max, self.m_Hold, self.m_Decay)


    def SetPower (self, newMax, newHold, newDecay):
        """Set the current power settings."""
        self.m_Max = newMax
        self.m_Hold = newHold
        self.m_Decay = newDecay


    def SetPowerFromString (self, profile):
        """Set the current power settings."""
        (m, h, d) = self._ParseProfile(profile)
        self.SetPower (m,h,d)

    def SetIdlePowerFromString( self, profile ):
        """Set the current idle power settings."""
        ( m, h, d ) = self._ParseProfile( profile )
        self.SetIdlePower( m, h, d )

    def ResetPower (self):
        """Resets the power back to the default."""
        # Extract the data from the default string
        (m, h, d) = self._ParseProfile(self.m_Settings[Axis.MotorPowerLabel])
        self.SetPower (m,h,d)


    def HomingPower (self):
        """Obtain the current power settings."""
        return (self.m_HomingMax, self.m_HomingHold, self.m_HomingDecay)


    def SetHomingPower (self, newMax, newHold, newDecay):
        """Set the current power settings."""
        self.m_HomingMax = newMax
        self.m_HomingHold = newHold
        self.m_HomingDecay = newDecay


    def ResetHomingPower (self):
        """Resets the power back to the default."""
        # Extract the data from the default string
        (m, h, d) = self._ParseProfile(self.m_Settings[Axis.MotorHomingPowerLabel])
        self.SetHomingPower (m, h, d)

    # 2011-10-20 sp -- power level for pre-homing
    def ResetPreHomingPower (self):
        """Resets the power back to the default."""
        # Extract the data from the default string
        (m, h, d) = self._ParseProfile(self.m_Settings[Axis.PinFlagPreHomingPowerLabel])
        self.m_PreHomingMax = m
        self.m_PreHomingHold = h
        self.m_PreHomingDecay = d

    # 2011-10-20 sp -- power level for pre-homing (for initial z-axis)
    def PreHomingPower (self):
        """Obtain the current power settings used when axis is performing a pre-home."""
        return (self.m_PreHomingMax, self.m_PreHomingHold, self.m_PreHomingDecay)


    def IdlePower (self):
        """Obtain the current power settings used when axis is stationary."""
        return (self.m_IdlePowerMax, self.m_IdlePowerHold, self.m_IdlePowerDecay)


    def SetIdlePower (self, newMax, newHold, newDecay):
        """Set the power settings to use when axis is stationary."""
        self.m_IdlePowerMax = newMax
        self.m_IdlePowerHold = newHold
        self.m_IdlePowerDecay = newDecay


    def ResetIdlePower (self):
        """Resets the idle power back to the default."""
        # Extract the data from the default string
        (m, h, d) = self._ParseProfile(self.m_Settings[Axis.MotorIdlePowerLabel])
        self.SetIdlePower (m,h,d)


    def IsValid (self, step):
        """Check whether a step setting is valid.
        step: the step setting to validate."""
        return self._Validate(step, False)

    def getStepperStatus( self ):
        """Returns the stepper card status character for this axis"""
        funcReference = __name__ + '.WaitForCompletion' # 2011-11-25 sp -- added
        self.svrLog.logVerbose('Extxx', self.logPrefix, funcReference, 'to call sendAndCheck')    # 2011-11-25 sp -- added; not accessed from within app, logging not tested
        if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
            self.m_Card.ExtLogger.SetCmdListLog("sendAndCheck()[%s] called from Axis.getStepperStatus()"%self.m_Card.prefix, self.m_Card.prefix);
        return self.m_Card.sendAndCheck( "" )[1]
 
    def getHomeStatus( self ):
        """Returns the stepper home flag state"""
        return self._IsAtHome()

    def getDeviceUsage( self ):
        """Returns the number of kilosteps performed by the axis"""
        return self.m_Tracker.NbrKilosteps()

    def removePower( self ):
        '''Turns off the power to axis'''
        cmds = self._PowerCmd( ( 100, 0, 0 ) )
        self._ProcessCmdList( cmds )
        Device.logger.logInfo("ZZZ removePower " )
        return True
    def applyPower( self ):
        cmds = self._PowerCmd(self.IdlePower())
        self._ProcessCmdList( cmds )
        Device.logger.logInfo("ZZZ applyPower " )
        return True


    #-------------------------------------------------------------------------
    #   'Protected' methods (avoid calling them unless from within a derived class)
    #   Command methods may be overridden if they vary
    #-------------------------------------------------------------------------

    def _ParseProfile (self, profileString):
        """Returns a profile tuple from the given profile string for speed or power.
            profileString should be of the form '%d, %d, %d' """
        profile = re.findall('[-+]?\d+', profileString)
        return (int(profile[0]), int(profile[1]), int(profile[2]))
        
    
    def _SetStepSpeedProfile (self, profile):
        """Sets the velocity profile (from a tuple) for subsequent moves."""
        self.SetStepSpeedProfile (profile[0], profile[1], profile[2])
        #print "profile[1] = %s" % profile[1] #shabnam

   
    def _Validate (self, step, takingAction = True):
        """Validate step setting. (NB: Do not call this directly.)
            step: the step setting to validate.
            takingAction: If set True, throw an AxisError exception if validation fails.
                           Otherwise, return result"""
        funcReference = __name__ + '._Validate'   # 2011-11-25 sp -- added
        # Determine whether motor is homed. Throw an exception if not
        msg = ''
        if not self.IsHomed():
            msg = 'Home motor before setting next position.'
        
        if "ROBO_DCVJIG_ZT" not in os.environ: # db added 2016/02/10 -- do not validate step when ROBO_DCVJIG_ZT is set
            # Ensure that step falls within the configured limits
            if (step > self.MaxStep() or step < 0):
                msg = 'Step setting exceeds given limits: 0 <= %d <= %d' % (step, self.MaxStep())

        result = (len(msg) == 0)

        if takingAction and not result:
            msg = "AX (%s): %s" % (self.m_Name, msg)
            self.svrLog.logError('', self.logPrefix, funcReference, 'AX=%s |%s' % (self.m_Name, msg) ) # 2011-11-25 sp -- added
            if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                self.m_Card.ExtLogger.SetCmdListLog(msg, self.m_Card.prefix);
                self.m_Card.ExtLogger.CheckSystemAvail()
                self.m_Card.ExtLogger.DumpHistory()
            raise AxisError (msg)
        return result        

  
    def _HalfStepCmd (self):
        """Return a command list defining the current half step"""
        return ["H%d"  % (self.HalfStep())]

 
    def _PowerCmd (self, powerProfile):
        """Return a command list defining the current power setting"""
        return ["P%d,%d,%d"  % (powerProfile[0],powerProfile[1],powerProfile[2])]


    def _SpeedCmd (self, profile):
        """Return a command list defining a speed profile setting"""
        return ["B%d" % (profile[0]), "E%d" % (profile[1]), "S%d" % (profile[2])]


    def _MoveCmd (self, step):
        """Return a command list defining the step to move to"""
        # NB: a small relative step is inserted first to get the power up.
        # This avoids a small 'kick' that has been observed
        #
        #return ["RN+1", "M%d"%(step)]
        return ["M%d" % (step)]


    def _IncrementCmd (self, increment):
        """Return a command list defining a step increment"""
        # NB: a small relative step is inserted first to get the power up.
        # This avoids a small 'kick' that has been observed
        #
        #return ["RN+1", "RN%+d"%(increment), "RN-1"]
        return ["RN%+d" % (increment)]


    def _ZeroCmd (self, zeroOffset):
        """Return a command list defining the absolute step for the current position."""
        return ["A%d" % (zeroOffset)]


    def _Move( self, moveCmd, bDummy = False, tipStrip = 0 ):
        '''Tell stepper card to move using the specified move command, which 
        dicates an absolute (M) or relative (R) movement and the steps to move 
        to (or by, for a relative move).
        "Loss of position" checking happens here.'''
        # 2011-11-25 sp -- set reference for program log
        funcReference = __name__ + '._Move'
        # 2011-11-14 -- sp
        self.svrLog.logDebug('', self.logPrefix, funcReference, 'start move command %s' % moveCmd)

        if self.debug: print(self, "Axis::_Move")
        
        # Do the move
        if tipStrip == 1:
          print('+-------------------Tip Strip _Move first move--------------')
          cmds = self._PowerCmd(self.Power()) + moveCmd + self._PowerCmd(self.IdlePower())
        elif tipStrip == 2:
          print('+-------------------Tip Strip _Move second move-------------')
          cmds = self._PowerCmd(self.Power()) + moveCmd + self._PowerCmd(self.IdlePower())
        else:  
          cmds = self._PowerCmd(self.Power()) + moveCmd + self._PowerCmd(self.IdlePower())
        
        self._ProcessCmdList(cmds)

        # Check the new position; if it's not where we want to be due to loss 
        # of position, home and try again. 
        # Note that it's assumed that the second move attempt works (brave!)
        reportedPos = self.Step()
        wantedPos = self.GetPositionTracking()
        if int(reportedPos) != wantedPos:
            Device.logger.logDebug("AX: position loss of %s (Wanted %d, reported %d)" % \
                    (self.m_Name, wantedPos, reportedPos))
            # 2011-11-25 sp -- add logging
            self.svrLog.logInfo('S', self.logPrefix, funcReference, "position lossed; home first and re-execute command; Wanted=%d |reported=%d" % \
                    (wantedPos, reportedPos) )
            self._Home()
            self._ProcessCmdList(cmds)


    def _MoveNonBlocking(self, moveCmd):
        '''Tell stepper card to move using the specified move command, which 
        dicates an absolute (M) or relative (R) movement and the steps to move 
        to (or by, for a relative move), in a non-blocking fashion.
        "Loss of position" checking happens here.'''
        if self.debug: print(self, "Axis::_MoveNonBlocking")
        
        # Do the move
        cmds = self._PowerCmd(self.Power()) 
        self._ProcessCmdList(cmds)

        cmds = moveCmd
        self._ProcessCmdList(cmds, True)
        

    def WaitForCompletion( self ):
        """Wait for an action to complete."""
        
        funcReference = __name__ + '.WaitForCompletion' # 2011-11-25 sp -- added

        if self.debug: print(self, "Axis::WaitForCompletion")

        # Wait until card has finished the move
        if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
            self.m_Card.ExtLogger.SetCmdListLog("pollUntilReady()[%s] from Axis.WaitForCompletion"%self.m_Card.prefix, self.m_Card.prefix)
        self.svrLog.logVerbose('X', self.logPrefix, funcReference, 'to call pollUntilReady')    # 2011-11-25 sp -- added, not used from within app, logging not tested
        self.m_Card.pollUntilReady()

        # Turn down the power
        cmds = self._PowerCmd(self.IdlePower())
        self._ProcessCmdList(cmds)

        # Check the new position; if it's not where we want to be due to loss 
        # of position, home and try again. 
        # Note that it's assumed that the second move attempt works (brave!)
        reportedPos = self.Step()
        wantedPos = self.GetPositionTracking()
        if int(reportedPos) != wantedPos:
            Device.logger.logDebug("AX: position loss of %s (Wanted %d, reported %d)" % \
                                   (self.m_Name, wantedPos, reportedPos))
            # 2011-11-25 sp -- add logging, not used within app, logging not tested
            self.svrLog.logInfo('Sxx', self.logPrefix, funcReference, "position loss=%s |Wanted=%d |reported=%d" % \
                    (self.m_Name, wantedPos, reportedPos) )

    def _PreHome (self):
        """Tell stepper card to home."""
        # 2011-10-17 sp -- set reference for program log
        funcReference = __name__ + '._PreHome'
        
        # 2011-10-17 sp -- use previously tested code when environment variable is defined
        # 2012-01-30 sp -- replace environment variable with configuration variable
        #if not os.environ.has_key('SS_MINNIE_DEV'):
        if tesla.config.SS_MINNIE_DEV == 0:
            cmds =  ['z'] + ['N-0'] + ['W300'] + \
                    self._HalfStepCmd() + \
                    ['P100,100,0'] + \
                    ['B3000'] + ['E15000'] + ['S5'] + \
                    ['RN+500'] + \
                    ['W100']                        # wait 100 msec         
        else:
        # 2011-10-17 sp replaced with home sensor settings from configuration file
        # changed homing logic
        # if home sensor not tripped, move safety backoffsteps margin in homing direction
        # worst case is when device is past home sensor for the z-axis, device tries
        # to overshoot limit by at most backoffsteps using reduced power settings
            cmds =  ['z'] + [self.m_InitMotorCmd] + ['W300'] + \
                    self._HalfStepCmd() + \
                    self._PowerCmd(self.PreHomingPower()) + \
                    self._SpeedCmd(self.HomingStepSpeedProfile()) + \
                    self._IncrementCmd(self.m_homeBackOffSteps) + \
                    ['W100']                        # wait 100 msec         
            self.svrLog.logDebug('', self.logPrefix, funcReference, 'cmd=%s' % cmds )
        
        self._ProcessCmdList (cmds)
        
    def _Home (self):
        """Tell stepper card to home."""
        # 2011-10-17 sp -- set reference for program log
        funcReference = __name__ + '._Home'
        
        if self.debug: print("Axis::_Home")
        # If the home flag is obscured then back off by 5000 steps
        
        # 2011-10-17 sp -- use previously tested code when environment variable is defined
        # 2012-01-30 sp -- replace environment variable with configuration variable
        #if not os.environ.has_key('SS_MINNIE_DEV'):
        if tesla.config.SS_MINNIE_DEV == 0 and tesla.config.SS_ORCA == 0:
            #if os.environ.has_key('SS_LEGACY'):
            if tesla.config.SS_LEGACY == 1:
               if self._IsAtHome():
                  cmds =  ['z'] + ['N-0'] + \
                          self._HalfStepCmd() + \
                          self._PowerCmd(self.HomingPower()) + \
                          self._SpeedCmd(self.HomingStepSpeedProfile()) + \
                          self._IncrementCmd(self.m_homeBackOffSteps) + \
                          ['W100']                        # wait 100 msec             
                  self._ProcessCmdList (cmds)            
               # Now attempt homing action        
               cmds =  ['z']                   + \
                       self._HalfStepCmd()     + \
                       self._PowerCmd(self.HomingPower())  + \
                       self._SpeedCmd(self.HomingStepSpeedProfile()) + \
                       ['N-1', 'RN-10']           + \
                       ['W100']                                                        # wait 100 msec
               self._ProcessCmdList (cmds)        
            elif tesla.config.SS_CUSTOM_HOME == 1:
                if self._IsAtHome():
                   cmds =  ['z'] + ['N-0'] + ['W300'] + ['CHKPOS'] + \
                           self._HalfStepCmd() + \
                           self._PowerCmd(self.HomingPower()) + \
                           self._SpeedCmd(self.HomingStepSpeedProfile()) + \
                           self._IncrementCmd(self.m_homeBackOffSteps) + \
                           ['W100']                        # wait 100 msec             
                   self._ProcessCmdList (cmds)            
                # Now attempt homing action    
                type = '[' + self.m_Name + ']'
                cmds =  self._GetConfigData(type)                            
                if cmds == []:
                        cmds =  ['z']                   + \
                        self._HalfStepCmd()     + \
                        self._PowerCmd(self.HomingPower())  + \
                        self._SpeedCmd(self.HomingStepSpeedProfile()) + \
                        ['N-1'] + ['W300'] + ['CHKPOS'] + \
                        ['RN-10'] + ['W100']                                            # wait 100 msec
                self._ProcessCmdList (cmds)                   
            else:
               if self._IsAtHome():
                  cmds =  ['z'] + ['N-0'] + \
                          self._HalfStepCmd() + \
                          self._PowerCmd(self.HomingPower()) + \
                          self._SpeedCmd(self.HomingStepSpeedProfile()) + \
                          self._IncrementCmd(self.m_homeBackOffSteps) + \
                          ['W100']                        # wait 100 msec             
                  self._ProcessCmdList (cmds)            
                # Now attempt homing action        

               cmds =  ['z']                   + \
                       self._HalfStepCmd()     + \
                       self._PowerCmd(self.HomingPower())  + \
                       self._SpeedCmd(self.HomingStepSpeedProfile()) + \
                       ['HOME'] + ['RN-10'] + ['W100']                                            # wait 100 msec
               self._ProcessCmdList (cmds)        
        # 2011-10-17 sp replaced with home sensor settings from configuration file
        else:
            if tesla.config.SS_CUSTOM_HOME == 1:
              if self._IsAtHome():
                 cmds =  ['z'] + [self.m_InitMotorCmd] + ['W300'] + ['CHKPOS'] + \
                         self._HalfStepCmd() + \
                         self._PowerCmd(self.HomingPower()) + \
                         self._SpeedCmd(self.HomingStepSpeedProfile()) + \
                         self._IncrementCmd(self.m_homeBackOffSteps) + \
                         ['W100']                        # wait 100 msec             
                 self._ProcessCmdList (cmds)            
              # Now attempt homing action    
              type = '[' + self.m_Name + ']'
              cmds =  self._GetConfigData(type)                            
              if cmds == []:
                      cmds =  ['z']                   + \
                      self._HalfStepCmd()     + \
                      self._PowerCmd(self.HomingPower())  + \
                      self._SpeedCmd(self.HomingStepSpeedProfile()) + \
                      [self.m_HomeMotorCmd] + ['W300'] + ['CHKPOS'] + \
                      ['RN-10'] + ['W100']                                            # wait 100 msec
              self._ProcessCmdList (cmds)                   
            else:   
              homeSettings = [] + self._HalfStepCmd() + \
                        self._PowerCmd(self.HomingPower()) + \
                        self._SpeedCmd(self.HomingStepSpeedProfile())
              if self._IsAtHome():
                  self.svrLog.logDebug('', self.logPrefix, funcReference, 'Already homed, backoff from home=%d' % self.m_homeBackOffSteps )
                  cmds =  ['z'] + [self.m_InitMotorCmd]                         # wait 100 msec
                  self._ProcessCmdList (cmds)    # As not all commands are consistenetly sent, cmds are split ans sent as 2 portions
                  cmds = homeSettings + self._IncrementCmd(self.m_homeBackOffSteps) + ['W100']                        # wait 100 msec
                  self._ProcessCmdList (cmds)
                  homeSettings = []       # 2011-12-12 sp -- no need to resend again for the homing command below
              # Now attempt homing action        
              cmds =  ['z'] + homeSettings + \
                     ['HOME'] + ['RN-10'] + ['W100']                                            # wait 100 msec
              self.svrLog.logDebug('', self.logPrefix, funcReference, 'start motion to home' )
              self._ProcessCmdList (cmds)        

              if self.m_Card.getHomingError():
                  self.svrLog.logError('', self.logPrefix, funcReference, 'Home command timed-out' )
                  raise AxisError( "AX error (%s): Home command timed-out" % self.m_Name )

        # reset step profile to standard
        # 2011-10-17 sp -- use previously tested code when environment variable is defined
        # 2012-01-30 sp -- replace environment variable with configuration variable
        #if not os.environ.has_key('SS_MINNIE_DEV'):
        if tesla.config.SS_MINNIE_DEV == 0:
            self.ResetPower()
            self.ResetStepSpeedProfile()
            cmds = self._PowerCmd( self.Power() ) + \
                   self._SpeedCmd( self.StepSpeedProfile() )
            self._ProcessCmdList( cmds )
        # 2011-10-17 sp -- remove redundancy in speed settings
        else:
            self.ResetPower()
            self._ProcessCmdList( self._PowerCmd( self.Power() ) )
            self.ResetStepSpeedProfile()

        # Now, the motor may have overshot the home sensor by an indeterminate amount.
        # We account for this as follows:
        #   - Increment the step while not looking for the home sensor
        #   - Decrement the step while looking for the home sensor.
        #       While we are still in the range that picks up the sensor, the last action equates
        #       to a relative move of 0, which generates a SimpleStepError
        #   - Whilst an error is raised, repeat the above. This effectively means that the motor
        #       will step until it leaves the home sensor range. This is the point we use as our home step
        #
        if self.m_Settings[Axis.HomingOvershootOnLabel] == '1':
            if self.debug: print("********** OVERSHOOT ON ************")
            i = 0
            overshoot = int (self.m_Settings[Axis.HomingOvershootLabel])
            inSensorRange = True
            
            while inSensorRange and i < overshoot:
                try:
                    i = i + 1
                    self._ProcessCmdList(['RN+1', 'RY-1'])
                except:
                    # Ignore exceptions: it effectively means we've moved one step forward, but not back
                    pass
            else:
                # This suggests the axis has moved out of the home sensor region
                inSensorRange = False

            if inSensorRange:
                self.svrLog.logError('', self.logPrefix, funcReference, "AX (%s): Homing is still in sensor range after %d steps.\nTry increasing the %s setting (or increase the holding power when homing." % \
                                                                            (self.m_Name, overshoot, Axis.HomingOvershootLabel) )   # 2011-11-25 sp -- added
                if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                    self.m_Card.ExtLogger.SetCmdListLog("AX (%s): Homing is still in sensor range after %d steps.\nTry increasing the %s setting (or increase the holding power when homing." % \
                                                                            (self.m_Name, overshoot, Axis.HomingOvershootLabel), self.m_Card.prefix)        
                    self.m_Card.ExtLogger.CheckSystemAvail()
                    self.m_Card.ExtLogger.DumpHistory()
                raise AxisError ("AX (%s): Homing is still in sensor range after %d steps.\nTry increasing the %s setting (or increase the holding power when homing." % \
                                    (self.m_Name, overshoot, Axis.HomingOvershootLabel))


    def _IsAtHome (self):
        """Check whether axis is at home"""
        # Check that we are in opto range (Disable if emulator is in use!!)
        funcReference = __name__ + '._IsAtHome'     # 2011-11-25 sp -- added

        if not self.m_CheckingHome or self.m_Card.usingEmulator:
            return True

        # Perform an 'I' command (without specifying a port). This will return
        # a bit mask, with bit 0 set if the flag is obscured

        if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
            self.m_Card.ExtLogger.SetCmdListLog("sendAndCheck()[%s] called from Axis._IsAtHome()"%self.m_Card.prefix,self.m_Card.prefix)
        self.svrLog.logVerbose('X', self.logPrefix, funcReference, 'to call sendAndCheck')    # 2011-11-25 sp -- added

        flags = self.m_Card.sendAndCheck( 'I' )[0]
        if self.debug: print(flags, int( flags ), int( flags ) % 2)

        # 2011-10-17 sp -- use previously tested code when environment variable is defined
        # 2012-01-30 sp -- replace environment variable with configuration variable
        #if not os.environ.has_key('SS_MINNIE_DEV'):
        if tesla.config.SS_MINNIE_DEV == 0:
            if int( flags ) % 2:
                return True
            else:
                return False
        # 2011-10-17 sp -- added section; use configuration file settings for checking home sensor
        else:
            flagNotHome  = int( flags ) % 2

            if( flagNotHome == int( self.m_HomeMotorCmd[3] ) ):
                self.svrLog.logDebug('', self.logPrefix, funcReference, 'Home detected; sensor=%d' % flagNotHome ) # 2011-12-13 sp
                return True
            else:
                self.svrLog.logDebug('', self.logPrefix, funcReference, 'Home not detected; sensor=%d' % flagNotHome ) # 2011-12-13 sp
                return False


    def _CompleteHoming (self):
        """Put settings back to normal mode"""
        # Set the home step, and set the idle power
        funcReference = __name__ + '._CompleteHoming'

        ResetCmds = self._ZeroCmd(self.HomeStep()) + \
                    self._PowerCmd(self.IdlePower()) + \
                    self._SpeedCmd(self.StepSpeedProfile())

        self._ProcessCmdList(ResetCmds)
        # 2011-11-14 -- sp
        self.svrLog.logDebug('', self.logPrefix, funcReference, 'position; HW=%d |SW=%d |SW home_ref=%s' %
                    ( self.m_Card.getPosition(), self.GetPositionTracking(), self.HomeStep() ) )


    def _ProcessCmdList( self, cmds, bNonBlocking = False ):
        """Pass command string to stepper card. Handle exceptions"""
        if self.debug: print("Axis::_ProcessCmdList - cmds, bNonBlocking")
        # 2011-11-14 -- sp
        funcReference = __name__ + '._ProcessCmdList'   
        try:
            #print "****card %s%s" % (self.m_Card.board, self.m_Card.address)
            #print "**process command list %s %s " % (cmds, bNonBlocking)
            self.m_Card.processCmds(cmds, bNonBlocking)

        except SimpleStepError as msg:
            # Catch exceptions specifically generated by the SimpleStep class
            msg = "AX (%s): %s" % (self.m_Name, msg)
            self.svrLog.logError('', self.logPrefix, funcReference, 'SimpleStep Err: AX=%s |%s' % (self.m_Name, msg) ) # 2011-11-25 sp -- added
            if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                self.m_Card.ExtLogger.SetCmdListLog(msg, self.m_Card.prefix);
                self.m_Card.ExtLogger.CheckSystemAvail()
                self.m_Card.ExtLogger.DumpHistory()
            raise AxisError(msg)
        except Exception as msg:
            if not self.m_Card.usingEmulator:
                # On the real instrument, just raise the exception back on up
                msg = "AX (%s): %s" % (self.m_Name, msg)
                self.svrLog.logError('', self.logPrefix, funcReference, 'Standard Err: AX=%s |%s' % (self.m_Name, msg) ) # 2011-11-25 sp -- added
                if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                    self.m_Card.ExtLogger.SetCmdListLog(msg, self.m_Card.prefix);
                    self.m_Card.ExtLogger.CheckSystemAvail()
                    self.m_Card.ExtLogger.DumpHistory()
                raise AxisError(msg)
            else:
                # Ignore exception(s) thrown by emulator (TT #17)
                pass

# EOF

