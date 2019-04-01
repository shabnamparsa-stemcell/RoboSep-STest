# 
# ThetaAxis.py
# Tesla.Hardware.ThetaAxis
#
# Drives a stepper card (via SimpleStep module) as a rotor
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

# ---------------------------------------------------------------------------

from SimpleStep.SimpleStep import SimpleStep
from tesla.hardware.config import HardwareError, LoadSettings
from tesla.hardware.Axis import Axis, AxisError

import os # db added 2016/02/10 -- for checking os environment variables

# ---------------------------------------------------------------------------
class ThetaError (AxisError):
    """Defines exceptions associated with ThetaAxis."""
    pass

# ---------------------------------------------------------------------------
class ThetaAxis (Axis):
    """Class to drive a SimpleStep stepper card as a rotor (ie may move in theta axis only)"""

    # The set of configuration labels used by ThetaAxis
    # (NB: use lower case, since configParser converts items to lowercase)
    #
    MotorStepsPerDegreeLabel = "calibration_stepsperdegree"
    HomingThetaLabel = "homingtheta_degrees"
    MinThetaLabel = "minimumangle_degrees"    
    MaxThetaLabel = "maximumangle_degrees"
    
    PI_DEG = 180.0              # Standard angular measure (theta units are degrees)
    TWOPI_DEG = 2.0 * PI_DEG
        
    MandatoryValues = ( MotorStepsPerDegreeLabel,)
    
    DefaultSettings = {   HomingThetaLabel: '0.0',
                        MinThetaLabel: '%d' % (0),
                        MaxThetaLabel: '%d' % (TWOPI_DEG),
                        }

    #-------------------------------------------------------------------------
    def __init__ (self, name, card, configData = {}, moreSettings = {}):
        """Refer to stepper card being controlled"""

        # Set up subclass (with all defaults 
        #
        settings = ThetaAxis.DefaultSettings.copy()
        settings.update (moreSettings)
        LoadSettings (settings, configData, ThetaAxis.MandatoryValues)

        # Determine step offset, so that minStep >= 0.
        #
        stepOffset = 0
        minSteps = float(settings[ThetaAxis.MinThetaLabel]) * float(settings[ThetaAxis.MotorStepsPerDegreeLabel])
        if minSteps < 0:
            stepOffset = -minSteps

        maxSteps = stepOffset + float(settings[ThetaAxis.MaxThetaLabel]) * float(settings[ThetaAxis.MotorStepsPerDegreeLabel])
            
        # Before initialising subclass, obtain maxSteps)
        #
        Axis.__init__(self, name, card, maxSteps, configData, settings)

        self.SetHomeStep (stepOffset)


    def _serviceMethods(self):
        '''Return a list of methods for the service interface'''
        return ['End', 'GetFirmwareVersion', 'Home', 'HomingPower', 
                'HomingStepSpeedProfile', 'IdlePower', 'IncrementTheta', 
                'MaxTheta', 'MinTheta', 'Power', 'SetHomingPower', 
                'SetHomingStepSpeedProfile', 'SetIdlePower',
                'SetPower', 'SetStepSpeedProfile', 'SetTheta',
                'StepSpeedProfile', 'StepsPerDegree', 'Theta', 'removePower', 'applyPower',
                ]
        

    def Home(self):
        """Homing (override)"""
        # Put on record
        # NB: we don't necessarily know the distance to home, but assuming the calculation
        #   probably causes less error than not.
	if self.debug: print self, "Axis::Home"
        if self.m_IsHomed:
            stepsToHome = self.HomeStep() - self.Step()
        else:
            stepsToHome = self.MaxStep()
            
        # Repeat home action until successful (it *should* only take one action, but...!)
        try:
            self.m_IsHomed = False
            while not self.m_IsHomed:
                self._Home()
                self.m_IsHomed = self._IsAtHome()
                self.m_Tracker.NoteHomeAction (stepsToHome, self.m_IsHomed)
                stepsToHome = 0
        finally:
            self._CompleteHoming()
        self.SetPositionTracking(self.HomeStep())


    #-------------------------------------------------------------------------
    def End(self):
        """Move to maximum limit """
        self.SetTheta(self.MaxTheta())

    #-------------------------------------------------------------------------
    def StepsPerDegree(self):
        """Obtain setting for the number of steps per degree"""
        return float (self.m_Settings[ThetaAxis.MotorStepsPerDegreeLabel])

    #-------------------------------------------------------------------------
    def HomeTheta(self):
        """Obtain home theta for rotor"""
        return float (self.m_Settings[ThetaAxis.HomingThetaLabel])

    #-------------------------------------------------------------------------
    def MinTheta(self):
        """Return the minimum permitted value of theta"""
        return float (self.m_Settings[ThetaAxis.MinThetaLabel])

    #-------------------------------------------------------------------------
    def MaxTheta(self):
        """Return the maximum permitted value of theta"""
        return float (self.m_Settings[ThetaAxis.MaxThetaLabel])

    #-------------------------------------------------------------------------
    def Theta(self):
        """Obtain the current theta value"""
        return (self.Step() - self.HomeStep())/self.StepsPerDegree()

    #-------------------------------------------------------------------------
    def SetTheta (self, theta, bNonBlocking = False):
        """Move carousel to absolute theta value.
            theta: the new setting (in degrees)"""

        if "ROBO_DCVJIG_ZT" not in os.environ: # db added 2016/02/10 -- do not validate step when ROBO_DCVJIG_ZT is set
            self.__ValidateTheta (theta)
        
        # Now move to the required step
        #
        self.SetStep(theta * self.StepsPerDegree() + self.HomeStep(), bNonBlocking, 0)


    #-------------------------------------------------------------------------
    def IncrementTheta (self, thetaInc):
        """Move carousel to relative theta value.
            thetaInc: the change in setting (in degrees)"""

        self.__ValidateTheta (self.Theta() + thetaInc)
        
        # Now increment to the required step
        #
        self.IncrementStep(thetaInc * self.StepsPerDegree(), False, False)

    #-------------------------------------------------------------------------
    #   Private methods
    #-------------------------------------------------------------------------

    #-------------------------------------------------------------------------

#    def _IsAtHome (self):
#        """Check whether axis is at home"""
#        # Check that we are in opto range (Disable if emulator is in use!!)
#        result = True
#        if self.m_CheckingHome and not self.m_Card.usingEmulator:
#            try:
#                self._ProcessCmdList (['RN-10', 'RY-1'])
#                result = False
#            except:
#                # This is what we expect
#                pass
#        return result            


    def __ValidateTheta (self, theta):
        """Check that theta is withing the valid range.
            Throw an exception if not"""

        # Ensure that (min <= theta < max)
        #
        maxTheta = self.MaxTheta()
        minTheta = self.MinTheta()
        
        if theta > maxTheta or theta < minTheta:
            raise ThetaError ('SetTheta: theta =%d cannot be mapped to valid range (%d - %d)' % (theta, minTheta, maxTheta))

        
    def _CompleteHoming (self):
        """Put settings back to normal mode"""
        # Set the home step, and set the idle power
        ResetCmds = self._ZeroCmd(-10) + \
                    self._PowerCmd(self.IdlePower()) + \
                    self._SpeedCmd(self.StepSpeedProfile())
        self._ProcessCmdList(ResetCmds)

# EOF
