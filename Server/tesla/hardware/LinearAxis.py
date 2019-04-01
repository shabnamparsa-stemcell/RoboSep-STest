# 
# LinearAxis.py
# tesla.hardware.LinearAxis
#
# Drives a stepper card (via SimpleStep module) along a linear strut
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
# Notes: Using Python 2.3.3 (final)

# ---------------------------------------------------------------------------

from SimpleStep.SimpleStep import SimpleStep
from tesla.hardware.config import HardwareError, LoadSettings
from tesla.hardware.Axis import Axis

# ---------------------------------------------------------------------------

# I'd like these as constants, when I get round to it. NB: positions are in mm
#

class LinearAxis (Axis):
    """Class to drive a SimpleStep stepper card as a linear strut (ie may move in a straight line)"""

    # The set of configuration labels used by LinearAxis
    # (NB: use lower case, since configParser converts items to lowercase)
    #
    MotorStepsPerMmLabel = "calibration_stepspermm"
    HomingPositionLabel = "homingposition_mm"
    MinPositionLabel = "minimumposition_mm"    
    MaxPositionLabel = "maximumposition_mm"
    
    MandatoryValues = (MotorStepsPerMmLabel, MaxPositionLabel,)
    
    DefaultSettings = {   HomingPositionLabel: '0.0',
                        MinPositionLabel: '%d' % (0)
                        }

    #-------------------------------------------------------------------------
    def __init__ (self, name, card, configData = {}, moreSettings = {}):
        """Refer to stepper card being controlled"""

        # Set up subclass (with all defaults 
        #
        settings = LinearAxis.DefaultSettings.copy()
        settings.update (moreSettings)
        LoadSettings (settings, configData, LinearAxis.MandatoryValues)

        # Before initialising subclass, obtain maxSteps
        #
        maxSteps = float(settings[LinearAxis.MaxPositionLabel]) * float(settings[LinearAxis.MotorStepsPerMmLabel])
        
        Axis.__init__(self, name, card, maxSteps, configData, settings)


    def _serviceMethods(self):
        '''Return a list of methods for the service interface'''
        return ['End', 'GetFirmwareVersion', 'Home', 'HomingPower', 
                'HomingStepSpeedProfile', 'IdlePower', 'IncrementPosition', 
                'MaxPosition', 'MinPosition', 'Position', 'Power',
                'SetHomingPower', 'SetHomingStepSpeedProfile', 'SetIdlePower',
                'SetPosition', 'SetPower', 'SetStepSpeedProfile',
                'StepSpeedProfile', 'StepsPerMm', 'removePower', 'applyPower',
                ]


    #-------------------------------------------------------------------------
    def End(self):
        """Move to maximum limit """
        self.SetPosition(self.MaxPosition())

    #-------------------------------------------------------------------------
    def StepsPerMm(self):
        """Obtain setting for the number of steps per mm"""
        return float (self.m_Settings[LinearAxis.MotorStepsPerMmLabel])

    #-------------------------------------------------------------------------
    def HomePosition(self):
        """Obtain home Position"""
        return float (self.m_Settings[LinearAxis.HomingPositionLabel])

    #-------------------------------------------------------------------------
    def MinPosition(self):
        """Return the minimum permitted Position"""
        return float (self.m_Settings[LinearAxis.MinPositionLabel])

    #-------------------------------------------------------------------------
    def MaxPosition(self):
        """Return the maximum permitted Position"""
        return float (self.m_Settings[LinearAxis.MaxPositionLabel])

    #-------------------------------------------------------------------------
    def Position(self):
        """Obtain the current Position"""
        return self.Step()/self.StepsPerMm()

    #-------------------------------------------------------------------------
    def SetPosition (self, position, tipStrip = 0):
        """Move to absolute Position.
            position: the new setting (in mm)"""

        # Now set to the required step
        #
        # print (position * self.StepsPerMm())
        self.SetStep(position * self.StepsPerMm(), False, tipStrip)

    #-------------------------------------------------------------------------
    def SetPositionWithoutMotion (self, position):                    #CWJ Add            
        """Move to absolute Position.                                 #CWJ Add            
            position: the new setting (in mm)"""                      #CWJ Add            
                                                                      #CWJ Add            
        # Now set to the required step                                #CWJ Add            
        #                                                             #CWJ Add                
        # print (position * self.StepsPerMm())                        #CWJ Add            
        self.SetStepWithoutMotion(position * self.StepsPerMm())       #CWJ Add            

    #-------------------------------------------------------------------------
    def IncrementPosition (self, increment):
        """Move by relative amount.
            increment: the relative amount by which to move (in mm)"""

        # Now increment by the required step
        #
        self.IncrementStep(increment * self.StepsPerMm(), False, False)


# EOF
