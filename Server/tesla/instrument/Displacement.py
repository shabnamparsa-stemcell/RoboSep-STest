# 
# Displacement.py
# instrument.Displacement
#
# Provides the level3 aspiration and dispense workflow data with which to drive the DRD pump in the Tesla instrument.
# 
# Copyright (c) Invetech Operations Pty Ltd, 2003
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
#
#KC - Made changes for IVEK pump
#  03/03/06 - added universal pump settings - RL 1
#

# ---------------------------------------------------------------------------

from tesla.exception import TeslaException
from tesla.hardware.config import LoadSettings
from tesla.hardware.DrdPump import DRDPump

# ---------------------------------------------------------------------------

class DisplacementError(TeslaException):
    """Exception for errors related to invalid pump settings."""
    pass

# ---------------------------------------------------------------------------

class PistonMode:
    """Contains generic requests for piston mode."""

    # ---------------------------------------------------------------------------
    def __init__ (self, pump):
        """Note the pump."""
        self.m_Pump = pump
                
    # ---------------------------------------------------------------------------
    def DefaultPumpSpeed (self):
        """Obtain the default pump speed profile for piston mode."""
        pass

    # ---------------------------------------------------------------------------
    def MaxVolume (self):
        """Obtain the maximum volume for piston mode."""
        pass
   
    # ---------------------------------------------------------------------------
    def UlPerStep (self):
        """Obtain the step conversion factor for piston mode."""
        pass
   
# ---------------------------------------------------------------------------

class SinglePistonMode(PistonMode):
    """Contains generic requests for single piston mode."""

    # ---------------------------------------------------------------------------
    def __init__ (self, pump):
        """Note the pump."""
        PistonMode.__init__ (self, pump)
                
    # ---------------------------------------------------------------------------
    def DefaultPumpSpeed (self):
        """Obtain the default pump speed profile for single piston mode."""
        return self.m_Pump.SinglePistonModeStepSpeedProfile()

    # ---------------------------------------------------------------------------
    def MaxVolume (self):
        """Obtain the maximum volume for single piston mode."""
        return self.m_Pump.MaxSinglePistonModeVolume()
   
    # ---------------------------------------------------------------------------
    def UlPerStep (self):
        """Obtain the step conversion factor for single piston mode."""
        return self.m_Pump.SinglePistonModeUlPerStep()
   
# ---------------------------------------------------------------------------

class DualPistonMode(PistonMode):
    """Contains generic requests for dual piston mode."""

    # ---------------------------------------------------------------------------
    def __init__ (self, pump):
        """Note the pump."""
        PistonMode.__init__ (self, pump)
                
    # ---------------------------------------------------------------------------
    def DefaultPumpSpeed (self):
        """Obtain the default pump speed profile for dual piston mode."""
        return self.m_Pump.DualPistonModeStepSpeedProfile()

    # ---------------------------------------------------------------------------
    def MaxVolume (self):
        """Obtain the maximum volume for dual piston mode."""
        return self.m_Pump.MaxDualPistonModeVolume()
   
    # ---------------------------------------------------------------------------
    def UlPerStep (self):
        """Obtain the step conversion factor for dual piston mode."""
        return self.m_Pump.DualPistonModeUlPerStep()
   
# ---------------------------------------------------------------------------

class Displacement:
    """Contains standard techniques and parameters used in volume displacement."""
    BacklashStepsPerSecLabel    = 'backlashflowrate_stepspersec'
    BacklashStepsLabel          = 'backlash_steps'
    DisplacementUlPerSecLabel   = 'flowrate_ulpersec'
    ScalingLabel                = 'compensation_scalingfactor'
    OffsetLabel                 = 'compensation_offset'

    SmallTipVolumeLimitLabel       = 'smalltipvolumelimit'
    LargeTipVolumeLimitLabel       = 'largetipvolumelimit'
    UsableVolumeLabel              = 'usablevolume'

    __mandatorySettings =   (
                            DisplacementUlPerSecLabel,
                            )
    __configData =  {
                    BacklashStepsLabel      : '150',
                    BacklashStepsPerSecLabel: '500',
                    ScalingLabel            : '1.0',
                    OffsetLabel             : '0.0',
                    LargeTipVolumeLimitLabel: '5000',
                    SmallTipVolumeLimitLabel: '1100',
                    UsableVolumeLabel: '3000'
                    }
    # IVEK: changed UsableVolumeLabel from 3000 to 5000 - kc

    # ---------------------------------------------------------------------------
    def __init__ (self, pistonMode, configData, moreSettings = {}):
        """Load in the configuration data for the aspiration technique."""
        settings = Displacement.__configData.copy()
        settings.update (moreSettings)
        LoadSettings (settings, configData, Displacement.__mandatorySettings)

        self.m_PistonMode = pistonMode
        self.m_Scaling = float (settings[Displacement.ScalingLabel])
        self.m_Offset = float (settings[Displacement.OffsetLabel])
        self.m_BacklashSteps = int(settings[Displacement.BacklashStepsLabel])
        self.m_BacklashUlPerSec = int(settings[Displacement.BacklashStepsPerSecLabel]) * self.UlPerStep()
        
        self.m_DisplacementUlPerSec = int(settings[Displacement.DisplacementUlPerSecLabel])
        
        self.m_smallTipVolumeLimit = int(settings[Displacement.SmallTipVolumeLimitLabel])
        self.m_largeTipVolumeLimit = int(settings[Displacement.LargeTipVolumeLimitLabel])
        self.m_usableVolume = int(settings[Displacement.UsableVolumeLabel])
        
        #RL 1
        #print "Universal Pump Debug m_usableVolume =", {'3000': 'DRD', '5500':'IVEK'}[settings[Displacement.UsableVolumeLabel]]


                
    # ---------------------------------------------------------------------------
    def VolumeToDisplace (self, volume):
        """Obtain the actual  volume to displace, given the required volume."""
        return (self.m_Scaling*volume) + self.m_Offset

    # ---------------------------------------------------------------------------
    def BacklashVolume (self):
        """Obtain the backlash volume used"""

        return self.m_BacklashSteps * self.UlPerStep()

    # ---------------------------------------------------------------------------
    def DefaultPumpSpeed (self):
        """Obtain the default pump speed profile for piston mode."""
        return self.m_PistonMode.DefaultPumpSpeed()

    # ---------------------------------------------------------------------------
    def UsableVolume (self):
        """Return the volume that is effectively usable."""
        return self.m_usableVolume

    # ---------------------------------------------------------------------------
    def CanHandleVolumeSmallTip( self, volume ):
        """Return whether the displacement parameters are usable for a given volume."""
        #return self.VolumeToDisplace(volume) <= self.UsableVolume()
        print("volume = %d small tip limit = %d" % (volume, self.m_smallTipVolumeLimit))
        #print volume <= self.m_smallTipVolumeLimit
        return volume <= self.m_smallTipVolumeLimit

    # ---------------------------------------------------------------------------
    def CanHandleVolumeLargeTip( self, volume ):
        """Return whether the displacement parameters are usable for a given volume."""
        #return self.VolumeToDisplace(volume) <= self.UsableVolume()
        print("volume = %d large tip limit = %d" % (volume, self.m_largeTipVolumeLimit))
        #print volume <= self.m_largeTipVolumeLimit
        return volume <= self.m_largeTipVolumeLimit

    # ---------------------------------------------------------------------------
    def UlPerStep (self):
        """Obtain the step conversion factor for piston mode."""
        return self.m_PistonMode.UlPerStep()

# ---------------------------------------------------------------------------

class Aspiration (Displacement):
    """Contains standard techniques and parameters used in aspiration."""
    OverAspirationMultipleLabel = 'overaspirationmultiple'
    OverAspirationVolumeUlLabel = 'overaspirationvolume_ul'

    __mandatorySettings =   ()
    __configData =  {
                    OverAspirationMultipleLabel : '0',
                    OverAspirationVolumeUlLabel   : '0',
                    }

    # ---------------------------------------------------------------------------
    def __init__ (self, pistonMode, configData):
        """Load in the configuration data for the aspiration technique."""
        settings = Aspiration.__configData.copy()
        LoadSettings (settings, configData, Aspiration.__mandatorySettings)

        Displacement.__init__ (self, pistonMode, configData, settings)

        self.m_OverAspirationVolumeUl = float (settings[Aspiration.OverAspirationVolumeUlLabel])
        self.m_OverAspirationMultiple = float (settings[Aspiration.OverAspirationMultipleLabel])
                
    # ---------------------------------------------------------------------------
    def VolumeToDisplace (self, volume):
        """Obtain the actual displace volume to aspire, given the required volume."""
        overAspirateVolume = volume + self.m_OverAspirationVolumeUl * self.m_OverAspirationMultiple
        return Displacement.VolumeToDisplace (self, overAspirateVolume)

# ---------------------------------------------------------------------------

class SingleModeAspiration (Aspiration):
    """Contains aspiration specifics for single piston mode aspiration."""

    # ---------------------------------------------------------------------------
    def __init__ (self, pump, configData):
        """Load in the configuration data for the aspiration technique."""
        Aspiration.__init__ (self, SinglePistonMode (pump), configData)
                
# ---------------------------------------------------------------------------

class DualModeAspiration (Aspiration):
    """Contains aspiration specifics for dual piston mode aspiration."""

    # ---------------------------------------------------------------------------
    def __init__ (self, pump, configData):
        """Load in the configuration data for the aspiration technique."""
        Aspiration.__init__ (self, DualPistonMode (pump), configData)
                
# ---------------------------------------------------------------------------

class FlushAspiration (SingleModeAspiration):
    """Contains aspiration specifics for flushing aspiration."""

    FlushStepsLabel   = 'flushsteps'

    __mandatorySettings =   (
                            FlushStepsLabel,
                            )
    
    # ---------------------------------------------------------------------------
    def __init__ (self, pump, configData):
        """Load in the configuration data for the flush aspiration technique."""
        settings = {}
        LoadSettings (settings, configData, FlushAspiration.__mandatorySettings)
        self.m_FlushSteps = float (settings[FlushAspiration.FlushStepsLabel])
        SingleModeAspiration.__init__ (self, pump, configData)
                
# ---------------------------------------------------------------------------

class AirSlugAspiration (DualModeAspiration):
    """Contains aspiration specifics for air slug aspiration."""

    AirSlugVolumeUlLabel   = 'airslugvolume_ul'

    __mandatorySettings =   (
                            AirSlugVolumeUlLabel,
                            )
    
    # ---------------------------------------------------------------------------
    def __init__ (self, pump, configData):
        """Load in the configuration data for the flush aspiration technique."""
        settings = {}
        LoadSettings (settings, configData, AirSlugAspiration.__mandatorySettings)
        self.m_FlushVolume = float (settings[AirSlugAspiration.AirSlugVolumeUlLabel])
        DualModeAspiration.__init__ (self, pump, configData)
                
# ---------------------------------------------------------------------------

class Dispense (Displacement):
    """Contains standard techniques and parameters used in dispense."""

    # ---------------------------------------------------------------------------
    def __init__ (self, pistonMode, configData):
        """Load in the configuration data for the dispense technique."""
        Displacement.__init__ (self, pistonMode, configData)

# ---------------------------------------------------------------------------

class SingleModeDispense (Dispense):
    """Contains specifics for single piston mode dispense."""

    # ---------------------------------------------------------------------------
    def __init__ (self, pump, configData):
        """Load in the configuration data for the aspiration technique."""
        Dispense.__init__ (self, SinglePistonMode (pump), configData)
                
# ---------------------------------------------------------------------------

class DualModeDispense (Dispense):
    """Contains  specifics for dual piston mode dispense."""

    # ---------------------------------------------------------------------------
    def __init__ (self, pump, configData):
        """Load in the configuration data for the aspiration technique."""
        Dispense.__init__ (self, DualPistonMode (pump), configData)
                
# ---------------------------------------------------------------------------

class FlushDispense (SingleModeDispense):
    """Contains  specifics for flush dispense (Actually just a Mnemonic for SingleModeDispense)."""
    pass
                
# ---------------------------------------------------------------------------

class AirSlugDispense (DualModeDispense):
    """Contains  specifics for air slug dispense (Actually just a Mnemonic for DualModeDispense)."""
    pass
                
#EOF
