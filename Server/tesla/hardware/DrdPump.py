# 
# DrdPump.py
# Tesla.Hardware.DrdPump
#
# Drives a stepper card for a DRD pump
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
#  04/11/05 - added park pump to power off motor - bdr
#  12/06/05 - added code for new IVEK pump - bdr
#  03/03/06 - added universal pump settings - RL 1
# 
# from hardware.ini...
#
#   [Pump]
#   singlepistonmode_standarddispenseflowrate_ulpersec = 2000
#   genericaspirationflowrate_ulpersec = 2000
#   dualpistonmode_calibration_ulperstep = 0.1728
#   maxvolume_singlepistonmode_ul = 3600
#   dualpistonmode_start_step = 0
#   dualpistonmode_standardaspirationflowrate_ulpersec = 285
#   valvepositionwhenhoming = 1
#   singlepistonmode_start_step = 1850
#   usinghomingovershootcorrection = 0
#   maxvolume_dualpistonmode_ul = 265
#   steppercardport = COM1
#   singlepistonmode_end_step = 4700
#   dualpistonmode_standarddispenseflowrate_ulpersec = 285
#   numberofhomingovershootcorrectiontries = 40
#   standardpowerprofile = 140,100,0
#   halfstepexponent = 0
#   backlashsteps = 150
#   homingpowerprofile = 140,100,0
#   hydraulicbottlelevel = 398576
#   genericdispenseflowrate_ulpersec = 2000
#   valvepositionwhenparked = 2
#   numberofprimingcycles = 2
#   steppercardvalveport = 0
#   singlepistonmode_calibration_ulperstep = 1.33
#   standardvelocityprofile = 42000, 45400, 10        (421, 1883 ,10)
#   homingvelocityprofile = 42000, 45400, 10        (421, 1883 ,10)
#   idlepowerprofile = 140, 50, 0
#   dualpistonmode_end_step = 1650
#   steppercardaddress = D2
#   valvepositionwhenaccessingtip = 2
#   singlepistonmode_standardaspirationflowrate_ulpersec = 2000
#   homestep = 0

from SimpleStep.SimpleStep import SimpleStep
from tesla.hardware.config import HardwareError, LoadSettings
from tesla.hardware.Axis import *

import os                                   #CWJ Add
from ipl.utils.wait import wait_msecs       #CWJ Add
import tesla.config         # 2012-01-30 sp -- replace environment variables with configuration file settings
# ---------------------------------------------------------------------------

class DRDPumpError(AxisError):
    """ Error associated with DRD pump handling"""
    pass

# ---------------------------------------------------------------------------

class PistonModeSettings(object):

    """Records settings for a particular piston mode"""

    def __init__ (self, ulPerStep, startStep, endStep, defaultAspirationRate, defaultDispenseRate, speedProfileB):
        """Store valid range, and conversion factor for a particular piston mode"""
        self.m_ulPerStep = ulPerStep
        self.m_StartStep = startStep
        self.m_EndStep = endStep
        self.m_DefaultAspirationRate = defaultAspirationRate
        self.m_DefaultDispenseRate = defaultDispenseRate
        self.m_MaxVolume = (self.m_EndStep - self.m_StartStep) * self.m_ulPerStep

        #print "!!!!!!!!!!!!!!!!!!!!!!!!!!",self.m_MaxVolume
        #print "!!!!!!!!!!!!!!!!!!!!!!!!!!",self.m_StartStep,self.m_EndStep,self.m_ulPerStep

        #RL 1
        self.m_SpeedProfileB = speedProfileB

    def MaxVolume(self):
        """Returns the maximum volume (in uL) available in this piston mode."""
        return self.m_MaxVolume


    def CalculatedStep(self, volume, tolerance = 1):
        """Returns the step represented by the given aspirated volume in this mode.
           The tolerance is used to apply a clip to the result, so that small overruns 
           of valid range are treated as rounding errors."""
        step = int((volume / self.m_ulPerStep) + self.m_StartStep + 0.5)

        # Apply clipping, if within tolerance
        if step < self.m_StartStep and step + tolerance >= self.m_StartStep:
            step = self.m_StartStep
        elif step > self.m_EndStep and step - tolerance <= self.m_EndStep:
            step = self.m_EndStep
        #print step #shabnam
        return step


    def CalculatedVolume(self, step):
        """Returns the displacement volume represented by the given step in this mode."""
        return (step - self.m_StartStep)*self.m_ulPerStep
        #print (step - self.m_StartStep)*self.m_ulPerStep #shabnam

    # def CalculatedSpeedProfile(self, ulPerSec):
    #     """Returns the speed profile to use for a given displacement rate in this mode.
    #         NB: The formula used was provided by a juju man, and the resulting (B,E,S) profile bears no resemblance to steps/sec!"""
    #     # Since we appear to be performing an incantation, magic numbers are OK
    #     #
    #     hz = ulPerSec / self.m_ulPerStep
    #     E =  46250 - (1.0/hz - 0.000069789)/0.0000005425437   # Like I said, juju and chicken entrails!!
    #     B = E
    #     if hz > 1300:
    #         B = E-1600
    #     S=8            
    #     return (B,E,S)
    # 
    # 
    # bdr
    # Returns the speed profile to use for a given displacement rate in this mode.
    # stepsPerSec as defined in hardware.ini as:
    #   dualPistonMode = 2000 uL/s / 0.1728 uL/step = 11574 steps/s
    #   singlePistonMode = 2000 uL/s / 1.33 uL/step =    1503 steps/s
    # 
    #      E_dual = 46250 - ((1000000.0 / 11574) - 69.789) / 0.5425437 = 46250 - 30.62        = 46219
    #    E_single = 46250 - ((1000000.0 / 1503) - 69.789) / 0.5425437 = 46250 - 1097.7 = 45152
    # 
    # so...for dual flow rates of 2000 ul/s B,E,S = (44619, 46219, 8)  
    # 
    # StepsPerSec = 1000000.0 / (((46250-E) * 0.54254) + 69.789)
    # StandardVelocityProfile (42000,45400,10) translates to: (421sps, 1883sps, 10)
    # Changes made by suggestion from SimpleStep for new board to now operate
    #       in steps per second instead of a time base
    #     
    # def CalculatedSpeedProfile(self, ulPerSec):
    # E = ulPerSec / self.m_ulPerStep
    # B = 600
    # if B > E:
    #    B = E
    #
    # bdr - convert secPerStep to E,B sec/step
    # def CalcStepsPerSecProfile(self, B, E):
    # 
    #     Esec = 1000000.0 / (((46250 - E) * 0.54254) + 69.789)
    #     Bsec = 1000000.0 / (((46250 - B) * 0.54254) + 69.789)
    #     return (Bsec,Esec)
    # bdr

    def CalculatedSpeedProfile(self, ulPerSec):
        """Returns the speed profile to use for a given displacement rate in this mode."""
        E = ulPerSec / self.m_ulPerStep
        
        #RL 1
        #B = 600
        B = self.m_SpeedProfileB
        
        if B > E:
            B = E - E/3
        S=8
        #S=5

        #print "B %d E %d =(%f/%f) S %d" % (B,E,ulPerSec,self.m_ulPerStep,S)
        #print "ulpersec %d = " %ulPerSec #shabnam
        #raw_input('Press <Enter> to Start!!! u r in speed calculator') #shabnam
        return (B,E,S)
    

    def InRange(self, step):
        """Returns whether the step is within the current range."""
        return step in range (self.m_StartStep, self.m_EndStep+1)




# ---------------------------------------------------------------------------

class DRDPump(Axis):
    """Class to drive the SimpleStep stepper card on a DRD pump."""

    # The set of configuration labels used by LinearAxis
    # (NB: use lower case, since configParser converts items to lowercase)
    #
    GenericAspirationFlowRateUlPerSecLabel      = 'genericaspirationflowrate_ulpersec'
    GenericDispenseFlowRateUlPerSecLabel        = 'genericdispenseflowrate_ulpersec'
    DualPistonModeStartLabel                            = 'dualpistonmode_start_step'
    DualPistonModeEndLabel                            = 'dualpistonmode_end_step'
    DualPistonModeVolumePerStepLabel            = 'dualpistonmode_calibration_ulperstep'
    DualPistonModeAspirationRateUlPerSecLabel   = 'dualpistonmode_standardaspirationflowrate_ulpersec'
    DualPistonModeDispenseRateUlPerSecLabel     = 'dualpistonmode_standarddispenseflowrate_ulpersec'
    SinglePistonModeStartLabel                        = 'singlepistonmode_start_step'
    SinglePistonModeEndLabel                        = 'singlepistonmode_end_step'
    SinglePistonModeVolumePerStepLabel                = 'singlepistonmode_calibration_ulperstep'
    SinglePistonModeAspirationRateUlPerSecLabel = 'singlepistonmode_standardaspirationflowrate_ulpersec'
    SinglePistonModeDispenseRateUlPerSecLabel        = 'singlepistonmode_standarddispenseflowrate_ulpersec'
    ValveHomingPositionLabel                            = 'valvepositionwhenhoming'            
    ValveParkingPositionLabel                            = 'valvepositionwhenparked'
    BacklashStepLabel                           = 'backlashsteps'
    ValvePortLabel                              = 'steppercardvalveport'
    StandardPowerProfileLabel                   = 'standardpowerprofile'
      
    #RL 1
    DRDPump_CalculatedSpeedProfile_B_Label      = 'drdpump_calculatedspeedprofile_b'
    DRDPump_Home_Command_Label                  = 'drdpump_home_command'
    
    #CWJ Add - for H error
    HomeStopPosLabel                            = 'prehome_pos_ct'
    HomeDeltaPosLabel                           = 'home_delta_pos_ct'
    HomeIncPosLabel                             = 'home_inc_pos_ct'
    
    m_Startup_Home                              = True

    MandatoryValues =  ()
    
    DefaultSettings =  { GenericAspirationFlowRateUlPerSecLabel : '750',
                         GenericDispenseFlowRateUlPerSecLabel : '750',
                         DualPistonModeStartLabel : '0',
                         DualPistonModeEndLabel : '1650',
                         DualPistonModeVolumePerStepLabel : '0.1728',
                         DualPistonModeAspirationRateUlPerSecLabel : '100',
                         DualPistonModeDispenseRateUlPerSecLabel : '100',
                         SinglePistonModeStartLabel : '1850',
                         SinglePistonModeEndLabel : '4700',
                         SinglePistonModeVolumePerStepLabel : '1.33',
                         SinglePistonModeAspirationRateUlPerSecLabel : '750',
                         SinglePistonModeDispenseRateUlPerSecLabel : '750',
                         Axis.MotorHomingStepVelocityLabel : '421, 1883, 8', 
                         ValveHomingPositionLabel : '2',
                         ValveParkingPositionLabel : '2',
                         BacklashStepLabel : '150',
                         ValvePortLabel : '0',
                         DRDPump_CalculatedSpeedProfile_B_Label : '600',
                         DRDPump_Home_Command_Label : 'H0',
                         StandardPowerProfileLabel : '140,100,0',
                         HomeStopPosLabel  : '500',
                         HomeDeltaPosLabel : '20',
                         HomeIncPosLabel   : '5'
                         }
    
    # IVEK removed: DualPistonModeEndLabel : '1650',
    # IVEK removed: DualPistonModeVolumePerStepLabel : '0.1728',
    # IVEK removed: DualPistonModeAspirationRateUlPerSecLabel : '100',
    # IVEK removed: DualPistonModeDispenseRateUlPerSecLabel : '100',
    # IVEK removed: SinglePistonModeStartLabel : '1850',
    # IVEK removed: SinglePistonModeEndLabel : '4700',
    # IVEK removed: SinglePistonModeVolumePerStepLabel : '1.33',
    # bdr - Axis.MotorHomingStepVelocityLabel : '421, 1883, 8', 
    # bdr - Axis.MotorHomingStepVelocityLabel : '42000, 44874, 8', 
    #
    # kc - changed DualPistonModeEndLabel from 1650 to 4000 for IVEK pump
    # kc - changed DualPistonModeVolumePerStepLabel from 1 to 0.125 for IVEK pump
    # kc - changed DualPistonModeAspirationRateUlPerSecLabel from 750 to 1000 for IVEK pump
    # kc - changed DualPistonModeDispenseRateUlPerSecLabel from 750 to 1000 for IVEK pump
    # kc - changed SinglePistonModeStartLabel from 1850 to 0 for IVEK pump
    # kc - changed SinglePistonModeEndLabel from 4700 to 40000 for IVEK pump
    # kc - changed SinglePistonModeVolumePerStepLabel from 1 to 0.125 for IVEK pump
    # RL - added two new param
    # CWJ - added three new param [HomeStopPosLabel , HomeDeltaPosLabel, HomeIncPosLabel ]        
    

    #-------------------------------------------------------------------------

    def __init__(self, name, card, configData = {}, moreSettings = {}):
        """Driver for DRD pump.
        name: the name to use when tracking use
        card: the card associated with the pump
        configData: a dictionary of configuration values with which to override 
                    the default settings.
        moreSettings: Refer to stepper card being controlled
                """

        # Set up subclass (with all defaults 
        settings =  DRDPump.DefaultSettings.copy()
        settings.update (moreSettings)
        LoadSettings (settings, configData, DRDPump.MandatoryValues)

        self.__m_ValvePosition = None        
        self.__m_ValveHomingPosition = int (settings[DRDPump.ValveHomingPositionLabel])        
        self.__m_ValveParkingPosition = int (settings[DRDPump.ValveParkingPositionLabel])        
        self.__m_ValvePort = int (settings[DRDPump.ValvePortLabel])
        self.__m_Backlash = int (settings[DRDPump.BacklashStepLabel])
        
        
        #RL 1 new param speed profile
        self.__m_DualMode = PistonModeSettings (float (settings[DRDPump.DualPistonModeVolumePerStepLabel]), \
                                                int (settings[DRDPump.DualPistonModeStartLabel]), \
                                                int (settings[DRDPump.DualPistonModeEndLabel]), \
                                                int (settings[DRDPump.DualPistonModeAspirationRateUlPerSecLabel]), \
                                                int (settings[DRDPump.DualPistonModeDispenseRateUlPerSecLabel]), \
                                                int (settings[DRDPump.DRDPump_CalculatedSpeedProfile_B_Label]))
        
        self.__m_SingleMode = PistonModeSettings (float (settings[DRDPump.SinglePistonModeVolumePerStepLabel]), \
                                                  int (settings[DRDPump.SinglePistonModeStartLabel]), \
                                                  int (settings[DRDPump.SinglePistonModeEndLabel]), \
                                                  int (settings[DRDPump.SinglePistonModeAspirationRateUlPerSecLabel]), \
                                                  int (settings[DRDPump.SinglePistonModeDispenseRateUlPerSecLabel]), \
                                                  int (settings[DRDPump.DRDPump_CalculatedSpeedProfile_B_Label]))
        
        self.m_CurrentMode = self.__m_DualMode

        # get calc'd speed profile for aspiration and dispense...
        self.__m_GenericAspirationRateProfile = "%d, %d, %d" % self.__m_SingleMode.CalculatedSpeedProfile (int (settings[DRDPump.GenericAspirationFlowRateUlPerSecLabel]))
        self.__m_GenericDispenseRateProfile = "%d, %d, %d" % self.__m_SingleMode.CalculatedSpeedProfile (int (settings[DRDPump.GenericDispenseFlowRateUlPerSecLabel]))
        
        # Before initialising subclass, obtain maxSteps)
        maxSteps = float(settings[DRDPump.SinglePistonModeEndLabel])
        Axis.__init__ (self, name, card, maxSteps, configData, settings)

        # Disable home check for pump: it doesn't work!!
        self.m_CheckingHome = False

        #CWJ
        self.m_HomeStopPos   = int(settings[self.HomeStopPosLabel]);
        self.m_HomeDeltaPos  = int(settings[self.HomeDeltaPosLabel]);
        self.m_HomeIncPos    = int(settings[self.HomeIncPosLabel]);
        
        #RL 1
        self.m_HomeCommand = settings[DRDPump.DRDPump_Home_Command_Label]
        #print "Universal Pump Debug Home Command =", {'H0': 'DRD', 'H3':'IVEK'}[settings[DRDPump.DRDPump_Home_Command_Label]]
        #print "Universal Pump Debug B =", {'600': 'DRD', '500': 'IVEK'}[settings[DRDPump.DRDPump_CalculatedSpeedProfile_B_Label]]
        #print "Universal Pump Debug DualPistonModeEnd =", {'1650': 'DRD', '4000': 'IVEK'}[settings[DRDPump.DualPistonModeEndLabel]]
        #print "Universal Pump Debug DualPistonModeVolumePerStep =", {'0.1728': 'DRD', '0.125': 'IVEK'}[settings[DRDPump.DualPistonModeVolumePerStepLabel]]
        #print "Universal Pump Debug DualPistonModeAspirationRateUlPerSec =", {'285': 'DRD', '1000': 'IVEK'}[settings[DRDPump.DualPistonModeAspirationRateUlPerSecLabel]]
        #print "Universal Pump Debug DualPistonModeDispenseRateUlPerSec =", {'285': 'DRD', '1000': 'IVEK'}[settings[DRDPump.DualPistonModeDispenseRateUlPerSecLabel]]
        #print "Universal Pump Debug SinglePistonModeStart =", {'1850': 'DRD', '0': 'IVEK'}[settings[DRDPump.SinglePistonModeStartLabel]]
        #print "Universal Pump Debug SinglePistonModeEnd =", {'4700': 'DRD', '44000': 'IVEK'}[settings[DRDPump.SinglePistonModeEndLabel]]
        #print "Universal Pump Debug SinglePistonModeVolumePerStep =", {'1.33': 'DRD', '0.125': 'IVEK'}[settings[DRDPump.SinglePistonModeVolumePerStepLabel]]

        self._ProcessCmdList(['o@LE','P' + str(settings['standardpowerprofile'])])


    def _serviceMethods(self):
        '''Return a list of methods for the service interface'''
        return ['End', 'GetFirmwareVersion', 'Home', 'IncrementVolume',
                'MaxVolume', 'ParkValve', 'Power', 'SetHomingPower', 
                'SetHomingStepSpeedProfile', 'SetIdlePower', 'SetPower', 
                'SetStepSpeedProfile', 'SetVolume', 'StepSpeedProfile', 
                'UseDualPistonMode', 'UseSinglePistonMode', 'Volume',
                ]


    def UseDualPistonMode(self):
        """Positions pump to start of dual piston mode.
            Throws an exception if valve has not been set.
            NB: Backlash is not taken into account"""
        self._ValidateValve()
        self._UsePistonMode( self.__m_DualMode, True )


    def IsUsingDualPistonMode(self):
        """Indicates whether piston is currently in dual piston range """
        return self.m_CurrentMode == self.__m_DualMode


    def MaxDualPistonModeVolume(self):
        """Returns the maximum volume (in ul) available when in dual piston mode."""
        return self.__m_DualMode.MaxVolume()


    def DualPistonModeUlPerStep(self):
        """Returns the ul conversion for dual piston mode."""
        return self.__m_DualMode.m_ulPerStep


    def UseSinglePistonMode(self):
        """Positions pump to start of single piston mode.
            Throws an exception if valve has not been set.
            NB: Backlash is not taken into account"""
        self._ValidateValve()
        self._UsePistonMode( self.__m_SingleMode, True )

    def UseSinglePistonModeWithoutStartMove( self ):
        """Positions pump to start of single piston mode.
            Throws an exception if valve has not been set.
            NB: Backlash is not taken into account"""
        self._ValidateValve()
        self._UsePistonMode( self.__m_SingleMode, False )

    def IsUsingSinglePistonMode(self):
        """Indicates whether piston is currently in single piston range """
        return self.m_CurrentMode == self.__m_SingleMode


    def MaxSinglePistonModeVolume(self):
        """Returns the maximum volume (in ul) available when in Single piston mode."""
        return self.__m_SingleMode.MaxVolume()


    def SinglePistonModeUlPerStep(self):
        """Returns the ul conversion for single piston mode."""
        return self.__m_SingleMode.m_ulPerStep


    def MaxVolume(self):
        """Returns the maximum volume (in ul) available in current piston mode.
            NB: the backlash volume is *not* taken into account."""
        return self.m_CurrentMode.MaxVolume()


    def FullStrokeVolume(self):
        """Returns the estimated volume displaced (in ul) by a full stroke (ie across both pistons).
        NB: the point at which the piston modes actually switch is not known. It is assumed to be in
        the middle of the mid-range"""
        stepsInMidRange = (self.__m_SingleMode.m_StartStep - self.__m_DualMode.m_EndStep)/2
        dualModeVolume = self.__m_DualMode.CalculatedVolume(self.__m_DualMode.m_EndStep + stepsInMidRange)
        singleModeVolume = self.__m_SingleMode.CalculatedVolume(self.__m_SingleMode.m_EndStep + stepsInMidRange)
        return dualModeVolume + singleModeVolume


    def PartialStrokeVolume( self, steps ):
        """Returns the estimated volume displaced (in ul) by a partial stroke (ie optionally
        across both pistons). NB: the point at which the piston modes actually switch is not
        known. It is assumed to be in the middle of the mid-range"""
        averageMidPoint = ( self.__m_SingleMode.m_StartStep + self.__m_DualMode.m_EndStep ) / 2.

        if steps <= averageMidPoint:
            return self.__m_DualMode.CalculatedVolume( steps )
        else:
            return self.__m_DualMode.CalculatedVolume( averageMidPoint ) + \
                   self.__m_SingleMode.CalculatedVolume( steps - averageMidPoint )

    def Volume(self):
        """Returns the volume (in ul) that is currently aspirated."""
        return self._CalculatedVolume (self.Step())


    def SetVolume(self, newVolume):
        """Aspirate/Dispense to the given volume (in ul).
            A DRDPump exception is thrown if maximum volume of current piston mode is exceeded.
            NB: backlash is not taken into account. This must be catered for by the caller"""
        newStep = self._CalculatedStep(newVolume)
        if not self.m_CurrentMode.InRange (newStep):
            raise DRDPumpError ('A total aspiration of %f ul exceeds the current range (%f ul)' % (newVolume, self.MaxVolume()))

        self._ValidateValve()                                
        self.SetStep (newStep, False, 0)


    def IncrementVolume(self, volumeInc):
        """Aspirate/Dispense a given volume (in ul)."""
        newVolume = self.Volume() + volumeInc
        self.SetVolume (newVolume)


    def SetFlowRate(self, newVolumeRate):
        """Set the flow rate."""
        #raw_input('Press <Enter> to Start!!! u r in set flow') #shabnam
        #print "volume rate %d  = " % newVolumeRate #shabnam
        (b,e,s) = self.m_CurrentMode.CalculatedSpeedProfile (newVolumeRate)
        self.SetStepSpeedProfile (b,e,s)


    def UseGenericAspirationFlowRate(self):
        """Reset the flow rate to the generic aspiration rate, regardless of piston mode."""
        self.SetStepSpeedProfileFromString (self.__m_GenericAspirationRateProfile)


    def UseGenericDispenseFlowRate(self):
        """Reset the flow rate to the generic dispense rate, regardless of piston mode."""
        #self.SetStepSpeedProfileFromString (self.__m_GenericAspirationRateProfile)
        self.SetStepSpeedProfileFromString (self.__m_GenericDispenseRateProfile)


    def UseDefaultAspirationFlowRate(self):
        """Reset the flow rate to the default aspiration rate for the current piston mode."""
        self.SetFlowRate(self.m_CurrentMode.m_DefaultAspirationRate)


    def UseDefaultDispenseFlowRate(self):
        """Reset the flow rate to the default dispense rate for the current piston mode."""
        self.SetFlowRate(self.m_CurrentMode.m_DefaultDispenseRate)


    def ParkValve(self):
        """Aspirate/Dispense a given volume (in ul)."""
        self._SetValvePosition (self.__m_ValveParkingPosition)


    def BacklashSteps(self):
        """The number of steps used in offsetting backlash."""
        return self.__m_Backlash


    def ParkPump(self):
        """Send low power (25%) cmd. - bdr"""
        cmds = self._PowerOffCmd()
        #self._ProcessCmdList(cmds)

    def IncrementStep( self, stepInc, bNonBlocking = False, bDummy = 0 ):                     #CWJ Add for DryRun 
        """Move motor by given step increment"""                                  #CWJ Add
        # 2012-01-30 sp -- replace environment variable with configuration variable
        #if os.environ.has_key('SS_DRYRUN'):                                       #CWJ Add
        if tesla.config.SS_DRYRUN == 1:                                       #CWJ Add
           print('\n### DrdPump in DryRun Mode! ###\n')                            #CWJ Add 
           iStepInc = int(stepInc)                                                #CWJ Add
           if iStepInc == 0:                                                      #CWJ Add
              # Ignore zero increment case: it can cause bad status error in card #CWJ Add
              return                                                              #CWJ Add
           else:                                                                  #CWJ Add
              # Validate new value before setting it                              #CWJ Add
              stepPosition = self.Step() + iStepInc                               #CWJ Add
              self._Validate(stepPosition)                                        #CWJ Add
                                                                                  #CWJ Add
              # Record the move in the device tracker and then move               #CWJ Add
              self.m_Tracker.NoteSteps (stepInc)                                  #CWJ Add
              self.SetPositionTracking(stepPosition)                              #CWJ Add
              wait_msecs(1000)                                                    #CWJ Add
        else:                                                                     #CWJ Add
          self.__class__.__base__.IncrementStep(self, stepInc, bNonBlocking, bDummy)      #CWJ Add
            
    # --- Protected methods ---

    def _PowerOnCmd(self):
        """Return the power on command for DRD Pump."""
        return ["P1"]


    def _PowerOffCmd(self):
        """Return the power off command for DRD Pump."""
        return ["P0"]


    def _PowerCmd(self):
        """Return the power command for DRD Pump
            (differs from simple stepper cards assumed by Axis)."""
        return self._PowerOnCmd()


    def _HomingPowerCmd(self):
        """Return the homing power command for DRD Pump
            (differs from simple stepper cards assumed by Axis)."""
        return self._PowerOnCmd()


    # bdr
    # cmds = ['F'] + \
    #        self._SpeedCmd(self.HomingStepSpeedProfile()) + \
    #        self._PowerOnCmd() + \
    #        ['N+1'] + \
    #        self._PowerOffCmd() + \
    #        self._SpeedCmd(self.StepSpeedProfile()) + \
    #        ['W100']
    # bdr


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
            
    def _Home(self, bSkipTrip = False):
        """Override the standard homing method."""
        self._SetValvePosition(self.__m_ValveHomingPosition)

        #RL 1
        home_command = self.m_HomeCommand
        pump_home_command = 'N%s1%s'%(self.m_Settings[Axis.HomingDirectionLabel], self.m_Settings[Axis.HomeLimitSensorLabel])
        pump_home_commandz = pump_home_command+'z'
        #cmds = ['H0'] + \
        # 2012-01-30 sp -- replace environment variable with configuration variable
        #if os.environ.has_key('SS_LEGACY'):
        if tesla.config.SS_LEGACY == 1:
           cmds = [home_command] + \
                  self._SpeedCmd(self.HomingStepSpeedProfile()) + \
                  ['N+1'] + \
                  self._SpeedCmd(self.StepSpeedProfile()) + \
                  ['W100']
        elif tesla.config.SS_CUSTOM_HOME == 1:
           if (self.m_Startup_Home == True):
              print('\n>>>> Custom Homing for Startup pump\n\n');
              if( tesla.config.SS_EXT_LOGGER == 1 ): 
                 self.m_Card.ExtLogger.SetCmdListLog('>>>> Custom Homing for Startup pump',self.m_Card.prefix);              
              self.m_Startup_Home = False
              type = '[' + 'Startup_Pump' + ']'
              cmds = self._GetConfigData(type)
              if cmds == []:
                 cmds = [home_command] + \
                        self._SpeedCmd(self.HomingStepSpeedProfile()) + \
                        [pump_home_command] + ['W300'] + ['CHKPOS'] + \
                        self._SpeedCmd(self.StepSpeedProfile()) + \
                        ['W100']
           else:                 
              if( tesla.config.SS_EXT_LOGGER == 1 ): 
                 self.m_Card.ExtLogger.SetCmdListLog('>>>> Custom Homing ',self.m_Card.prefix);
              type = '[' + self.m_Name + ']'
              cmds = self._GetConfigData(type)
              if cmds == []:
                 cmds = [home_command] + \
                        self._SpeedCmd(self.HomingStepSpeedProfile()) + \
                        [pump_home_command] + ['W300'] + ['CHKPOS'] + \
                        self._SpeedCmd(self.StepSpeedProfile()) + \
                        ['W100']
        else:
           if (self.m_Startup_Home == True):
                self.m_Startup_Home = False
                if not tesla.config.SS_USE460_PUMPHOMING: 
                   if( tesla.config.SS_EXT_LOGGER == 1 ): 
                      self.m_Card.ExtLogger.SetCmdListLog('>>>> Startup pump Homing',self.m_Card.prefix);
                   cmds = [self._GetPowerHomingPump()] + \
                          self._SpeedCmd(self.HomingStepSpeedProfile()) + \
                          ['z'] + [home_command] + \
                          [pump_home_commandz] + ['W300'] + ['A0'] + ['CHKPOS'] + \
                          self._SpeedCmd(self.StepSpeedProfile()) + \
                          [self._GetPowerStandardPump()] + \
                          ['W100']
                else:
                   if( tesla.config.SS_EXT_LOGGER == 1 ): 
                      self.m_Card.ExtLogger.SetCmdListLog('>>>> Startup pump Homing - Disabled',self.m_Card.prefix);
                   cmds = ['z'] + [home_command] + \
                          self._SpeedCmd(self.HomingStepSpeedProfile()) + \
                          [pump_home_command] + ['W300'] + ['CHKPOS'] + \
                          self._SpeedCmd(self.StepSpeedProfile()) + \
                          ['W100']
           else:      
                if not tesla.config.SS_USE460_PUMPHOMING: 
                   self.m_Card.setPumpHomePowerCommand( self._GetPowerHomingPump() ) ;
                   self.m_Card.setPumpStandardPowerCommand( self._GetPowerStandardPump() ) ;
                   self.m_Card.setPumpHomeStepSpeedCommand( self._SpeedCmd(self.HomingStepSpeedProfile()));
                   self.m_Card.setPumpStandardStepSpeedCommand( self._SpeedCmd(self.StepSpeedProfile()));
                   self.m_Card.setPumpHomeCommand( home_command );
                   self.m_Card.setPumpPositions(self.m_HomeStopPos, self.m_HomeDeltaPos, self.m_HomeIncPos);
                   if (bSkipTrip == True):
                      cmds = ['PUMPHOMESKIPTRIP'] + ['W300']  + ['CHKPOS']
                   else: 
                      cmds = ['PUMPHOME'] + ['W300']  + ['CHKPOS']
                else:
                   if( tesla.config.SS_EXT_LOGGER == 1 ): 
                      self.m_Card.ExtLogger.SetCmdListLog('>>>> Use N+1 for Piston position reset',self.m_Card.prefix);
                   cmds = [home_command] + \
                          self._SpeedCmd(self.HomingStepSpeedProfile()) + \
                          [pump_home_command] + ['W300'] + ['CHKPOS'] + \
                          self._SpeedCmd(self.StepSpeedProfile()) + \
                          ['W100']

        self._ProcessCmdList(cmds)

    def _IsAtHome (self):
        """Check whether axis is at home"""
        return True


    def _CompleteHoming(self):
        """Override the standard end homing method."""
        self.ParkValve()


    def _Move(self, stepCmd, bDummy1 = False, bDummy2= 0):
        """Override the standard move method."""
        self._ValidateValve()
        self._ProcessCmdList (stepCmd)


    def _SetValvePosition(self, position):
        """Move valve to given position. position should be either 1 or 2"""
        
        if position not in (1, 2):
            raise DRDPumpError("Valve position %d is invalid" % (position))

        if self.__m_ValvePosition == position:
            # No change. Ignore
            return
        else:
            # Set new position
            self.__m_ValvePosition = position
            if position == 1:
                self.m_Card.turnOutputOff(self.__m_ValvePort)
            else:
                self.m_Card.turnOutputOn(self.__m_ValvePort)


    def _ValvePosition(self):
        """Obtain current valve position.
            Returns None if valve has not been set"""
        return self.__m_ValvePosition


    def _ValidateValve(self):
        """Obtain current valve position.
            Returns None if valve has not been set"""
        if self._ValvePosition() == None:
            raise DRDPumpError ("Set valve position before operating piston")


    def _BacklashVolume(self):
        """Returns the backlash volume for the current piston mode setting."""
        return self.__m_Backlash * self.m_CurrentMode.m_ulPerStep


    def _CalculatedVolume(self, step):
        """Returns the aspirated volume represented by the given step.
            NB: the backlash volume is *not* taken into account. """
        return self.m_CurrentMode.CalculatedVolume (step)


    def _CalculatedStep(self, volume):
        """Returns the step represented by the given aspirated volume.
            NB: the backlash volume is *not* taken into account. """
        return self.m_CurrentMode.CalculatedStep (volume)


    def _UsePistonMode( self, pistonMode, bMoveToStart ):
        """Positions pump to start of given piston mode."""
        self.m_CurrentMode = pistonMode
        self.UseDefaultDispenseFlowRate ()
        if bMoveToStart:
            self.SetStep( self.m_CurrentMode.m_StartStep, False, 0 )

    def _GetPowerStandardPump(self):
        return ('P%d,%d,%d'%(self.m_Max,self.m_Hold,self.m_Decay));

    def _GetPowerHomingPump(self):
        return ('P%d,%d,%d'%(self.m_HomingMax, self.m_HomingHold, self.m_HomingDecay));        
# EOF
