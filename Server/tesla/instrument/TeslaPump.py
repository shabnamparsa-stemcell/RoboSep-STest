# 
# TeslaPump.py
# tesla.instrument.TeslaPump
#
# Provides the level3 workflows required to drive the DRD pump in the Tesla 
# instrument.
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

import math
import os       #CWJ Add

from SimpleStep.SimpleStep import SimpleStep

from tesla.exception import TeslaException
from tesla.hardware.config import HardwareConfiguration, gHardwareData, LoadSettings, gHardwareMisc
from tesla.hardware.DrdPump import DRDPump
from tesla.instrument.Displacement import Displacement, SingleModeAspiration, \
                    DualModeAspiration, FlushAspiration, AirSlugAspiration, \
                    SingleModeDispense, DualModeDispense, FlushDispense, AirSlugDispense
from tesla.instrument.Subsystem import Subsystem
import tesla.config         # 2012-01-30 sp -- replace environment variables with configuration file settings

from tesla.hardware.config import GetHardwareData
#KC
#when prints == 1 debug print statements will be active for IVEK pump changes
#when prints == 0 print statements will be inactive

prints = 1

# ---------------------------------------------------------------------------

class TeslaPumpError(TeslaException):
    """Exception for errors related to invalid pump settings."""
    pass

# ---------------------------------------------------------------------------

class TeslaPump(Subsystem):
    """Class to drive the Tesla pump"""
    SectionName = "Pump"

    cMixingID = -1    
    cFlushingID = -2    

    #   Config data
    #
    PortLabel               = 'steppercardport'
    BoardAddressLabel       = 'steppercardaddress'
    ValveTipPositionLabel   = 'valvepositionwhenaccessingtip'
    MaxVolumeSingleModeLabel= 'maxvolume_singlepistonmode_ul'
    MaxVolumeDualModeLabel  = 'maxvolume_dualpistonmode_ul'
    Aspiration_1Label       = 'aspiration_1'
    Aspiration_2Label       = 'aspiration_2'
    Aspiration_3Label       = 'aspiration_3'
    Aspiration_4Label       = 'aspiration_4'
    Aspiration_MixLabel     = 'aspiration_mix'
    Dispense_1Label         = 'dispense_1'
    Dispense_2Label         = 'dispense_2'
    Dispense_3Label         = 'dispense_3'
    Dispense_4Label         = 'dispense_4'
    Dispense_MixLabel       = 'dispense_mix'
    PrimeIterationsLabel    = 'numberofprimingcycles'
    HydraulicBottleLevelLabel = 'hydraulicbottlelevel'
    HydraulicBottleThresholdLabel = 'hydraulicbottlethreshold'
    HydraulicBottleCapacityLabel = 'hydraulicbottlecapacity'
    
    __configData =  {
                    ValveTipPositionLabel   : '1',
                    #Aspiration_1Label       : '200, ',
                    #Aspiration_2Label       : '454,',
                    #Aspiration_3Label       : '1000, 5000',
                    #Dispense_1Label         : '200,',
                    #Dispense_2Label         : '454,',
                    #Dispense_3Label         : '1100, 5000',
                    MaxVolumeDualModeLabel   : '265',                        
                    MaxVolumeSingleModeLabel : '3600',                        
                    PrimeIterationsLabel    : '2',
                    HydraulicBottleCapacityLabel   : '500000',
                    HydraulicBottleThresholdLabel   : '100000',
                    HydraulicBottleLevelLabel   : '500000',
                   }
    
    __mandatorySettings =   (
                            PortLabel,
                            BoardAddressLabel
                            )
    #
    # IVEK removed: MaxVolumeSingleModeLabel  : '3600', - bdr - KC - changed to 5000
    # IVEK removed: MaxVolumeDualModeLabel  : '265',    - bdr - KC -changed to 500
    
    # ---------------------------------------------------------------------------
    def __init__( self, name ):
        """Load in the configuration data for the pump."""
        global gHardwareData
        global gHardwareMisc

        Subsystem.__init__( self, name )

        self._m_Settings = TeslaPump.__configData.copy()
        pumpSettings =  gHardwareData.Section(TeslaPump.SectionName)
        LoadSettings (self._m_Settings, pumpSettings, TeslaPump.__mandatorySettings)

        #OVERWRITING hydraulicbottlelevel WITH MISC FILE'
        miscPumpSettings =  gHardwareMisc.Section(TeslaPump.SectionName)
        LoadSettings (self._m_Settings, miscPumpSettings)
        
        # Initialise the pump inteerface
        #
        pCard = SimpleStep (self._m_Settings[TeslaPump.BoardAddressLabel], self._m_Settings[TeslaPump.PortLabel])
        self.__m_Pump = DRDPump (TeslaPump.SectionName, pCard, pumpSettings)

        # Set up aspiration techniques
        #
        self.__m_Aspiration1 = DualModeAspiration (self.__m_Pump, gHardwareData.Section('Aspiration_1'))
        self.__m_Aspiration2 = SingleModeAspiration (self.__m_Pump, gHardwareData.Section('Aspiration_2'))
        self.__m_Aspiration3 = SingleModeAspiration (self.__m_Pump, gHardwareData.Section('Aspiration_3'))
        self.__m_Aspiration4 = SingleModeAspiration (self.__m_Pump, gHardwareData.Section('Aspiration_4'))
        self.__m_AspirationMix = SingleModeAspiration (self.__m_Pump, gHardwareData.Section('Aspiration_Mix'))
        self.__m_AspirationFlush = FlushAspiration (self.__m_Pump, gHardwareData.Section('Aspiration_Flush'))
        self.__m_AspirationAirSlug = AirSlugAspiration (self.__m_Pump, gHardwareData.Section('Aspiration_AirSlug'))

        # Set up dispense techniques
        #
        self.__m_Dispense1 = DualModeDispense (self.__m_Pump, gHardwareData.Section('Dispense_1'))
        self.__m_Dispense2 = SingleModeDispense (self.__m_Pump, gHardwareData.Section('Dispense_2'))
        self.__m_Dispense3 = SingleModeDispense (self.__m_Pump, gHardwareData.Section('Dispense_3'))
        self.__m_Dispense4 = SingleModeDispense (self.__m_Pump, gHardwareData.Section('Dispense_4'))
        self.__m_DispenseMix = SingleModeDispense (self.__m_Pump, gHardwareData.Section('Dispense_Mix'))
        self.__m_DispenseFlush = FlushDispense (self.__m_Pump, gHardwareData.Section('Dispense_Flush'))
        self.__m_DispenseAirSlug = AirSlugDispense (self.__m_Pump, gHardwareData.Section('Dispense_AirSlug'))
        
        self.__m_ValveTipSetting = int (self._m_Settings[TeslaPump.ValveTipPositionLabel])        
        self.__m_ValveReservoirSetting = (self.__m_ValveTipSetting) % 2 + 1
        self.__m_MaxVolumeSingleMode = int (self._m_Settings[TeslaPump.MaxVolumeSingleModeLabel])
        self.__m_MaxVolumeDualMode = int (self._m_Settings[TeslaPump.MaxVolumeDualModeLabel])
        self.__m_NbrPrimingStrokes = int (self._m_Settings[TeslaPump.PrimeIterationsLabel])

        self.__MakeReadyForAspiration()        

        self.m_hardwareConfig = GetHardwareData()


        self.m_hardwareConfigMisc = gHardwareMisc
        
        self.__m_hydraulicLevel = int (self._m_Settings[TeslaPump.HydraulicBottleLevelLabel])
        print 'self.__m_hydraulicLevel -----------------', self.__m_hydraulicLevel
        
        #RL 1
        #print "Universal Pump Debug  MaxVolumeDualMode =", {'265': 'DRD', '500': 'IVEK'}[self._m_Settings[TeslaPump.MaxVolumeDualModeLabel]]
        #print "Universal Pump Debug  MaxVolumeSingleMode =", {'3600': 'DRD', '5500': 'IVEK'}[self._m_Settings[TeslaPump.MaxVolumeSingleModeLabel]]

    
    def GetPumpCard(self):                        #CWJ Add
        return self.__m_Pump.m_Card               #CWJ Add
    def GetPumpAxis(self):
        return self.__m_Pump
        
    def _serviceMethods(self):
        '''Return a list of methods for the service interface'''
        return ['Aspirate', 'Dispense', 'Initialise', 'SetValveToReservoir',
                'SetValveToTip', 
                ]

    def useHydroVolume( self, volume ):
        '''Reduces the available hydraulic fluid by the given amount and stores in config file'''
        # 2012-01-30 sp -- replace environment variable with configuration variable
        #if os.environ.has_key('SS_DRYRUN'):                                       #CWJ Add
        if tesla.config.SS_DRYRUN == 1:                                       #CWJ Add
           pass                                                                   #CWJ Add
        else:                                                                     #CWJ Add    
           self.__m_hydraulicLevel = self.__m_hydraulicLevel - volume
           self.m_hardwareConfigMisc.writeItem( self.SectionName,
                                            TeslaPump.HydraulicBottleLevelLabel,
                                           int( self.__m_hydraulicLevel ) )
           self.m_hardwareConfigMisc.write()

    def isHydroFluidLow( self ):
        '''Returns true if the hydraulic fluid is low'''
        # 2012-01-30 sp -- replace environment variable with configuration variable
        #if os.environ.has_key('SS_DRYRUN'):                                       #CWJ Add
        if tesla.config.SS_DRYRUN == 1:                                       #CWJ Add
           return False                                                           #CWJ Add
        else:                                                                     #CWJ Add                  
           return self.__m_hydraulicLevel < int (self._m_Settings[TeslaPump.HydraulicBottleThresholdLabel])

    def setHydroFluidFull( self ):
        '''Sets the hydraulic fluid to full capacity'''
        self.__m_hydraulicLevel = int (self._m_Settings[TeslaPump.HydraulicBottleCapacityLabel])
        self.m_hardwareConfigMisc.writeItem( self.SectionName,
                                         TeslaPump.HydraulicBottleLevelLabel,
                                         int( self.__m_hydraulicLevel ) )
        self.m_hardwareConfigMisc.write()

    # ---------------------------------------------------------------------------
    def Initialise (self, bSkipTrip = False):
        """Initialise the pump"""
        self.SetValveToReservoir()
        self.__m_Pump.Home(bSkipTrip)
        self.__MakeReadyForAspiration()        

    # ---------------------------------------------------------------------------
    def SetValveToTip (self):
        """Set valve so that piston accesses dispense tip."""
        # 2012-01-30 sp -- replace environment variable with configuration variable
        #if os.environ.has_key('SS_DRYRUN'):                                       #CWJ Add
        if tesla.config.SS_DRYRUN == 1:                                       #CWJ Add
           pass
        else:
           self.__m_Pump._SetValvePosition(self.__m_ValveTipSetting)

    # ---------------------------------------------------------------------------
    def SetValveToReservoir (self):
        """Set valve so that piston accesses reservoir."""
        # 2012-01-30 sp -- replace environment variable with configuration variable
        #if os.environ.has_key('SS_DRYRUN'):                                       #CWJ Add
        if tesla.config.SS_DRYRUN == 1:                                       #CWJ Add
           pass
        else:
           self.__m_Pump._SetValvePosition(self.__m_ValveReservoirSetting)

    # ---------------------------------------------------------------------------
    def ParkValve (self):
        """Set pump valve to de-energized state."""
        self.__m_Pump.ParkValve()

    # ---------------------------------------------------------------------------
    def ParkPump (self):
        """Power off the pump motor. - bdr"""
        self.__m_Pump.ParkPump()

    # ---------------------------------------------------------------------------
    def Aspirate (self, volume, capacity):
        """Aspirate from the vial, choosing a technique according to volume, and capacity.
        Return the volume aspirated at the tip. (This may vary from given sub-transport volume)"""
        
        self.__VerifyReadinessForAspiration()

        if volume > capacity:
            raise TeslaPumpError ('volume (%dul) exceeds tip capacity (%dul)' % (volume, capacity))

        # Determine which technique to use
        # (??? I know! magic numbers!! A candidate for rebasing!!)
        #
        volumeAspirated = 0
        usingSmallTip = capacity <= 1100
        
        if usingSmallTip:
            print '\n###### Aspirate usingSmallTip !!!'
            if self.__m_Aspiration1.CanHandleVolumeSmallTip( volume ):
                ( volumeAspirated, self.__dispenseList ) = self.__Aspirate_1(volume, self.__m_Aspiration1)
                self.__nextDispenseID = 1
            elif self.__m_Aspiration2.CanHandleVolumeSmallTip( volume ):
                ( volumeAspirated, self.__dispenseList ) = self.__Aspirate_2(volume, self.__m_Aspiration2)
                self.__nextDispenseID = 2
            elif self.__m_Aspiration3.CanHandleVolumeSmallTip( volume ):
                ( volumeAspirated, self.__dispenseList ) = self.__Aspirate_3(volume, self.__m_Aspiration3)
                self.__nextDispenseID = 3
            elif self.__m_Aspiration4.CanHandleVolumeSmallTip( volume ):
                ( volumeAspirated, self.__dispenseList ) = self.__Aspirate_4(volume, self.__m_Aspiration4)
                self.__nextDispenseID = 4
            else:
                raise TeslaPumpError ('volume (%dul) exceeds all technique limits' % (volume))
        else:
            print '\n###### Aspirate not usingSmallTip !!!'
            if self.__m_Aspiration1.CanHandleVolumeLargeTip( volume ):
                ( volumeAspirated, self.__dispenseList ) = self.__Aspirate_1(volume, self.__m_Aspiration1)
                self.__nextDispenseID = 1
            elif self.__m_Aspiration2.CanHandleVolumeLargeTip( volume ):
                ( volumeAspirated, self.__dispenseList ) = self.__Aspirate_2(volume, self.__m_Aspiration2)
                self.__nextDispenseID = 2
            elif self.__m_Aspiration3.CanHandleVolumeLargeTip( volume ):
                ( volumeAspirated, self.__dispenseList ) = self.__Aspirate_3(volume, self.__m_Aspiration3)
                self.__nextDispenseID = 3
            elif self.__m_Aspiration4.CanHandleVolumeLargeTip( volume ):
                ( volumeAspirated, self.__dispenseList ) = self.__Aspirate_4(volume, self.__m_Aspiration4)
                self.__nextDispenseID = 4
            else:
                raise TeslaPumpError ('volume (%dul) exceeds all technique limits' % (volume))


        return volumeAspirated
    
    # ---------------------------------------------------------------------------
    def Dispense (self):
        """Dispense to the vial, choosing a technique according to previous aspiration.
        The volume dispensed at the tip is assumed to be the volume retruned by the previous aspiration call"""

        self.__VerifyReadinessForDispense()
        print '\n###### Dispense !!!'
        if self.__nextDispenseID == 1:
            self.__Dispense_1(self.__dispenseList, self.__m_Dispense1)
        elif self.__nextDispenseID == 2:
            self.__Dispense_2(self.__dispenseList, self.__m_Dispense2)
        elif self.__nextDispenseID == 3:
            self.__Dispense_3(self.__dispenseList, self.__m_Dispense3)
        elif self.__nextDispenseID == 4:
            self.__Dispense_4(self.__dispenseList, self.__m_Dispense4)
        else:
            raise TeslaPumpError ("Invalid dispense ID %d" % (self.__nextDispenseID))

        self.__MakeReadyForAspiration()
        
    # ---------------------------------------------------------------------------
    def Prime (self):
        """Prime the pump. Return the volume dispensed to tip"""
        volumeDispensed = 0

        try:
            # Set the pump to home position
            #
            self.__m_Pump.Home()
            self.__m_Pump.UseGenericAspirationFlowRate()

            # Now repeatedly aspirate and dispense for the full piston stroke
            # (This is low level stuff)
            #
            volumePerStroke = self.__m_Pump.FullStrokeVolume()
            print "volumePerStroke %d" % volumePerStroke
            
            for _ in range(0,self.__m_NbrPrimingStrokes):
                self.SetValveToReservoir()
                self.__m_Pump.UseGenericAspirationFlowRate()

                self.__m_Pump.SetStep (self.__m_Pump.MaxStep(), False, 0)

                self.useHydroVolume( volumePerStroke )
                self.SetValveToTip()
                self.__m_Pump.UseGenericDispenseFlowRate()
                self.__m_Pump.SetStep (0, False, 0)
                volumeDispensed += volumePerStroke

            # To complete, aspirate full stroke from HF reservoir...
            #
            self.SetValveToReservoir()
            self.__m_Pump.SetStep (self.__m_Pump.MaxStep(), False, 0)
            self.useHydroVolume( volumePerStroke )

            # ...dispense single piston mode to tip reservoir...
            #
            self.SetValveToTip()
            self.__m_Pump.UseSinglePistonMode() # Sets position to start of mode
            volumeDispensed += self.__m_Pump.MaxVolume()
            
            # ...and flush dual piston mode to HF reservoir...
            #
            self.SetValveToReservoir()
            self.__m_Pump.UseDualPistonMode() # Sets position to start of mode
            self.__MakeReadyForAspiration()

        finally:
            self.ParkValve()

        return volumeDispensed
    
    # ---------------------------------------------------------------------------
    def FlushAspirate( self ):
        """Aspirate the flush volume"""
        # Perform the flush aspirate
        #
        (volumeAspirated, self.__dispenseList) = \
            self.__Aspirate_Flush( self.__m_AspirationFlush.m_FlushSteps,
                                   self.__m_AspirationFlush )

    def FlushDispense( self ):
        """Dispense the flush volume"""
        volumeDispensed = 0

        self.__Dispense_Flush( self.__dispenseList, self.__m_DispenseFlush )

        # Determine how much was flushed out
        #
        for volume in self.__dispenseList:
            volumeDispensed += volume

        return volumeDispensed            


    def FlushAspirateAirSlug( self ):
        # Now obtain the air slug
        #
        self.__SetupAirSlug(self.__m_AspirationAirSlug, self.__m_DispenseAirSlug)
        self.__MakeReadyForAspiration()

        self.ParkValve()



    # ---------------------------------------------------------------------------
    def AspiratePreMixAirSlug (self, volume):
        """Aspirate an air slug prior to mixing.
        NB: The subsequent dispense occurs after the mix, so this is a bit different to standard aspirate/dispense cycle."""

        self.__VerifyReadinessForAspiration()
        self.SetValveToReservoir()
        self.__m_Pump.UseSinglePistonMode()
        self.SetValveToTip()

        # Aspirate the air slug. NB: Another aspiration is expected, so no backlash correction is required
        self.__m_Pump.UseDefaultAspirationFlowRate()

        if self.debug: print "self.__m_Pump.IncrementVolume( volume )"
        self.__m_Pump.IncrementVolume( volume )

        if self.debug: print "self.SetValveToReservoir()"
        self.SetValveToReservoir()
        
        if self.debug: print "self.__m_Pump.IncrementStep( self.__m_Pump.BacklashSteps() )"
        self.__m_Pump.IncrementStep( self.__m_Pump.BacklashSteps(), False, False )
        if self.debug: print "self.__m_Pump.IncrementStep( - self.__m_Pump.BacklashSteps() )"
        self.__m_Pump.IncrementStep( - self.__m_Pump.BacklashSteps(), False, False )


    # ---------------------------------------------------------------------------
    def DispensePreMixAirSlug (self, volume):
        """Dispense the air slug after mixing."""
        # Dispense the air slug. NB: this should be just after the last mixing dispense, so no backlash correction is required
        #
        self.__m_Pump.UseDefaultDispenseFlowRate()
        self.__m_Pump.IncrementVolume(-volume)
        self.SetValveToReservoir()

    # ---------------------------------------------------------------------------
    def AspirateMix (self, volume, seqNbr):
        """Aspirate the given volume using the mixing profile"""

        self.__VerifyReadinessForAspiration()
        
        # Clip mixing volume to what can be handled by piston mode
        #
        mixVolume = min (volume, self.__m_AspirationMix.m_PistonMode.MaxVolume()-self.__m_Pump.Volume())
        #print "############################ AAA %d %d %d %d" %(mixVolume,volume, self.__m_AspirationMix.m_PistonMode.MaxVolume(),self.__m_Pump.Volume())

        # Since we're not actually interested in transporting anything,
        # just aspire the volume in one stroke: with no regard for backlash or compensation
        #
        self.__dispenseList = []
        self.SetValveToTip()
        self.__m_Pump.SetFlowRate (self.__m_AspirationMix.m_DisplacementUlPerSec)
        self.__m_Pump.IncrementVolume (mixVolume)

        # Prepare to dispense
        #
        self.__dispenseList.insert (0, mixVolume)

        self.__nextDispenseID = TeslaPump.cMixingID
        
        return mixVolume
        
    # ---------------------------------------------------------------------------
    def DispenseMix (self):
        """Dispense using the mixing profile"""
        self.__VerifyReadinessForDispense()

        # Since we're not actually interested in transporting anything,
        # just dispense the volume in one stroke: with no regard for backlash or compensation
        #
        self.SetValveToTip()
        self.__m_Pump.SetFlowRate (self.__m_DispenseMix.m_DisplacementUlPerSec)
        self.__m_Pump.IncrementVolume (-self.__dispenseList[0])

        self.__MakeReadyForAspiration()

    # ---------------------------------------------------------------------------
    #   Private methods: accessed via exception handlers
    # ---------------------------------------------------------------------------

    # ---------------------------------------------------------------------------
    def __VerifyReadinessForAspiration (self):
        """Ensure that aspiration is the next operation expected."""
        if self.__nextDispenseID != None:
            raise TeslaPumpError ("Prior aspiration (%d) needs to be dispensed first!" % (self.__nextDispenseID))
        
    # ---------------------------------------------------------------------------
    def __VerifyReadinessForDispense (self):
        """Ensure that dispense is the next operation expected."""
        if self.__nextDispenseID == None:
            raise TeslaPumpError ("No prior aspiration has occurred")
        
    # ---------------------------------------------------------------------------
    def __MakeReadyForAspiration (self):
        """Indicate that aspiration is the next operation expected."""
        self.__nextDispenseID = None
        self.__dispenseList = []
        
    # ---------------------------------------------------------------------------
    def __Aspirate_1 (self, volume, aspData):
        """Aspirate from the vial using technique 1 (refer IDD for description):"""
        self.Trace ("Aspirate_1")
        dispenseList = []
        
        # Set to dual piston mode, and aspirate backlash
        #
        self.SetValveToReservoir()
        self.__m_Pump.UseDualPistonMode()
        self.__m_Pump.SetFlowRate (aspData.m_BacklashUlPerSec)
        
        backlashVolume = aspData.BacklashVolume()
        self.__m_Pump.IncrementVolume (backlashVolume)

        # Aspire the volume 
        #
        self.SetValveToTip()
        volumeBeingDisplaced = aspData.VolumeToDisplace (volume)
        self.__m_Pump.SetFlowRate (aspData.m_DisplacementUlPerSec)
        self.__m_Pump.IncrementVolume (volumeBeingDisplaced)
        
        # Prepare to dispense
        #
        self.SetValveToReservoir()
        self.__m_Pump.IncrementVolume (backlashVolume)
        self.__m_Pump.IncrementVolume (-backlashVolume)
        self.SetValveToTip()

        # For technique 1, dispense a portion of the overaspiration back
        #
        overaspiratedVolume = Displacement.VolumeToDisplace (aspData, aspData.m_OverAspirationVolumeUl) 
        self.__m_Pump.IncrementVolume (-overaspiratedVolume)

        volumeBeingDisplaced -= overaspiratedVolume

        # NB: overaspirated volume is *not* dispensed subsequently
        #
        dispenseList.insert (0, Displacement.VolumeToDisplace(aspData, volume))
        
        return (volume + overaspiratedVolume, dispenseList)

    # ---------------------------------------------------------------------------
    def __Aspirate_2 (self, volume, aspData):
        """Aspirate from the vial using technique 2"""
        self.Trace ("Aspirate_2")
        dispenseList = []
        
        # Set to single piston mode, and aspirate backlash
        #
        self.SetValveToReservoir()
        self.__m_Pump.UseSinglePistonMode()
        self.__m_Pump.SetFlowRate (aspData.m_BacklashUlPerSec)

        backlashVolume = aspData.BacklashVolume()
        self.__m_Pump.IncrementVolume (backlashVolume)

        # Aspire the volume 
        #
        self.SetValveToTip()
        volumeBeingDisplaced = aspData.VolumeToDisplace (volume)
        self.__m_Pump.SetFlowRate (aspData.m_DisplacementUlPerSec)
        self.__m_Pump.IncrementVolume (volumeBeingDisplaced)

        # Prepare to dispense
        #
        self.SetValveToReservoir()
        self.__m_Pump.IncrementVolume (backlashVolume)
        self.__m_Pump.IncrementVolume (-backlashVolume)
        self.SetValveToTip()

        # NB: overaspirated volume is *not* dispensed subsequently
        #
        dispenseList.insert (0, Displacement.VolumeToDisplace(aspData, volume))
        
        return (volume+aspData.m_OverAspirationVolumeUl, dispenseList)

    # ---------------------------------------------------------------------------
    def __Aspirate_3 (self, volume, aspData):
        """Aspirate from the vial using technique 3"""
        self.Trace ("Aspirate_3")
        dispenseList = []
        
        # Set to single piston mode, and aspirate backlash
        #
        self.SetValveToReservoir()
        self.__m_Pump.UseSinglePistonMode()
        self.__m_Pump.SetFlowRate (aspData.m_BacklashUlPerSec)

        backlashVolume = aspData.BacklashVolume()
        self.__m_Pump.IncrementVolume (backlashVolume)
        # Aspire the volume 
        #
        self.SetValveToTip()
        volumeBeingDisplaced = aspData.VolumeToDisplace (volume)
        self.__m_Pump.SetFlowRate (aspData.m_DisplacementUlPerSec)
        self.__m_Pump.IncrementVolume (volumeBeingDisplaced)

        # Prepare to dispense
        #
        self.SetValveToReservoir()
        self.__m_Pump.SetFlowRate (aspData.m_BacklashUlPerSec)
        self.__m_Pump.IncrementVolume (backlashVolume)
        self.__m_Pump.IncrementVolume (-backlashVolume)
        self.SetValveToTip()

        # NB: overaspirated volume is *not* dispensed subsequently
        #
        dispenseList.insert (0, Displacement.VolumeToDisplace(aspData, volume))
        
        return (volume+aspData.m_OverAspirationVolumeUl, dispenseList)

    # ---------------------------------------------------------------------------
    def __Aspirate_4 (self, volume, aspData):
        """Aspirate from the vial using technique 4"""
        self.Trace ("Aspirate_4")
        dispenseList = []
        
        # Set to single piston mode.
        #
        self.SetValveToReservoir()
        self.__m_Pump.UseSinglePistonMode()
        backlashVolume = aspData.BacklashVolume()
        
        # Divide volume aspirated each cycle into equal amounts.
        # (This ensures that each volume > technique 2 volume: worst case 3001/2 = 1500.5 > 454.)
        #
        volumeBeingDisplaced = aspData.VolumeToDisplace (volume) # NB: Assume there is no aspiration ???
        nbrCycles = int (math.ceil(volumeBeingDisplaced / aspData.UsableVolume()))
        if prints == 1:
            print "KC - TeslaPump.py, Aspiration 4 being used:" #KC: for debugging
            print "nbrCycles = %d" %(nbrCycles) #KC: for debugging
            print "volumeBeingDisplaced = %d" %(volumeBeingDisplaced) #KC: for debugging
            print "aspData.UsableVolume() = %d" %(aspData.UsableVolume()) #KC: for debugging
        
        volumePerCycle = volumeBeingDisplaced / nbrCycles
        volumeLeft = volumeBeingDisplaced
        print 'Aspirate before While VolPerCycle: %d, VolLeft %d'%(volumePerCycle,volumeLeft)
        while volumeLeft > 0:
            # Initialise pump to 0 setting, accounting for backlash
            #

            self.SetValveToReservoir()

            self.__m_Pump.UseDefaultDispenseFlowRate()

            self.__m_Pump.SetVolume (0) # Combine stroke and backlash

            self.__m_Pump.SetFlowRate (aspData.m_BacklashUlPerSec)

            self.__m_Pump.IncrementVolume (backlashVolume)
            # Aspire the sub volume
            #
            volumePerCycle = min (volumeLeft, volumePerCycle)
            volumeLeft = volumeLeft - volumePerCycle
            

            self.SetValveToTip()


            self.__m_Pump.SetFlowRate (aspData.m_DisplacementUlPerSec)


            self.__m_Pump.IncrementVolume (volumePerCycle)

            dispenseList.insert (0, volumePerCycle)
            print 'Aspirate before in while VolPerCycle: %d, VolLeft %d'%(volumePerCycle,volumeLeft)
        # Prepare to dispense
        #

        self.SetValveToReservoir()

        self.__m_Pump.IncrementVolume (backlashVolume)

        self.__m_Pump.IncrementVolume (-backlashVolume)

        self.SetValveToTip()
        print '\nVolume in Tip = %f'% self.__m_Pump.Volume()        
        return (volume, dispenseList)

    # ---------------------------------------------------------------------------
    def __Aspirate_Flush( self, steps, aspData ):
        """Aspirate from the reservoir when flushing"""
        self.Trace ("Aspirate_Flush")
        dispenseList = []
        
        # Set to single piston mode without move to starting position
        #
        self.SetValveToReservoir()
        self.__m_Pump.UseSinglePistonModeWithoutStartMove()
        self.__m_Pump.SetFlowRate( aspData.m_DisplacementUlPerSec )
        
        # Aspirate the volume 
        #
        volume = self.__m_Pump.PartialStrokeVolume( steps )
        self.__m_Pump.SetStep( steps, False, 0 )

        # Prepare to dispense
        #
#        self.__m_Pump.SetFlowRate (aspData.m_BacklashUlPerSec)
#        self.__m_Pump.IncrementVolume (backlashVolume)
#        self.__m_Pump.IncrementVolume (-backlashVolume)

#        dispenseList.insert( 0, volumeBeingDisplaced )
        return (volume, dispenseList)

    # ---------------------------------------------------------------------------
    def __Dispense_1 (self, dispenseList, dispData):
        """Dispense to the vial using technique 1"""
        self.Trace ("Dispense_1")
        
        # The assumption is that dual piston mode has already been specified by prior aspiration
        #
        if not self.__m_Pump.IsUsingDualPistonMode():
            raise TeslaPumpError ("Dispense 1: expecting pump to be in dual piston mode.")
        
        # Dispense the volume 
        #
        self.SetValveToTip()
        volumeBeingDisplaced = dispenseList[0]
        self.__m_Pump.SetFlowRate (dispData.m_DisplacementUlPerSec)
        self.__m_Pump.IncrementVolume (-volumeBeingDisplaced)

    # ---------------------------------------------------------------------------
    def __Dispense_2 (self, dispenseList, dispData):
        """Dispense to the vial using technique 2"""
        self.Trace ("Dispense_2")
        
        # The assumption is that single piston mode has already been specified by prior aspiration
        #
        if not self.__m_Pump.IsUsingSinglePistonMode():
            raise TeslaPumpError ("Dispense 2: expecting pump to be in single piston mode.")
        
        # Dispense the volume 
        #
        self.SetValveToTip()
        volumeBeingDisplaced = dispenseList[0]
        self.__m_Pump.SetFlowRate (dispData.m_DisplacementUlPerSec)
        self.__m_Pump.IncrementVolume (-volumeBeingDisplaced)

    # ---------------------------------------------------------------------------
    def __Dispense_3 (self, dispenseList, dispData):
        """Dispense to the vial using technique 3"""
        self.Trace ("Dispense_3")
        
        # The assumption is that single piston mode has already been specified by prior aspiration
        #
        if not self.__m_Pump.IsUsingSinglePistonMode():
            raise TeslaPumpError ("Dispense 3: expecting pump to be in single piston mode.")
        
        # Dispense the volume 
        #
        self.SetValveToTip()
        volumeBeingDisplaced = dispenseList[0]
        self.__m_Pump.SetFlowRate (dispData.m_DisplacementUlPerSec)
        self.__m_Pump.IncrementVolume (-volumeBeingDisplaced)

    # ---------------------------------------------------------------------------
    def __Dispense_4 (self, dispenseList, dispData):
        """Dispense to the vial using technique 4"""
        self.Trace ("Dispense_4")
        
        # The assumption is that single piston mode has already been specified by prior aspiration
        #
        if not self.__m_Pump.IsUsingSinglePistonMode():
            raise TeslaPumpError ("Dispense 4: expecting pump to be in single piston mode.")

        # Prepare to dispense the necessary displaced volume
        #
        totalVolume = 0.0
        backlashVolume = dispData.BacklashVolume()

        print '\nEnter Dispense 4\n backlashVolume - %d'%backlashVolume
        print '-----------------------------------------------------------'
        print '\n============ DispenseList============================='
        print dispenseList
        print '======================================================\n'
        # Dispense in reverse order to previous aspiration
        #
        for volumePerCycle in dispenseList:
            print '\nVolume in Tip = %f'% self.__m_Pump.Volume()
            if self.__m_Pump.Volume() <= backlashVolume:
                # Prior cycle has dispensed. Reset pump for next cycle
                #                                

                self.SetValveToReservoir()

                self.__m_Pump.UseDefaultAspirationFlowRate ()

                self.__m_Pump.IncrementVolume (volumePerCycle+backlashVolume) # Combine stroke and backlash

                self.__m_Pump.SetFlowRate (dispData.m_BacklashUlPerSec)

                self.__m_Pump.IncrementVolume (-backlashVolume)
            

            self.SetValveToTip()

            self.__m_Pump.SetFlowRate (dispData.m_DisplacementUlPerSec)

            self.__m_Pump.IncrementVolume (-volumePerCycle)
            totalVolume += volumePerCycle

    # ---------------------------------------------------------------------------
    def __Dispense_Flush (self, dispenseList, dispData):
        """Dispense to the vial when flushing"""
        self.Trace ("Dispense_Flush")
        
        # The assumption is that single piston mode has already been specified by prior aspiration
        #
#        if not self.__m_Pump.IsUsingSinglePistonMode():
#            raise TeslaPumpError ("Dispense flush: expecting pump to be in single piston mode.")
        
        # Dispense the volume 
        #
        self.SetValveToTip()

        self.__m_Pump.SetFlowRate( dispData.m_DisplacementUlPerSec )

        self.__m_Pump.SetStep (0, False, 0)




    # ---------------------------------------------------------------------------
    def __SetupAirSlug (self, aspData, dispData):
        """Set up the air slug between hydraulic fluid and tip fluid."""
        # Move to Dual piston mode
        #
        self.SetValveToReservoir()
        self.__m_Pump.UseDualPistonMode()

        # Aspirate air slug volume (...you *did* raise the robot arm , didn't you?!!)
        #
        self.SetValveToTip()
        self.__m_Pump.SetFlowRate (aspData.m_DisplacementUlPerSec)
        self.__m_Pump.IncrementVolume (aspData.m_FlushVolume)

        # Reset piston
        #
        self.SetValveToReservoir()
        self.__m_Pump.SetFlowRate (dispData.m_DisplacementUlPerSec)
        self.__m_Pump.SetVolume (0)


    #GetInstrumentAxisStatusSet support methods
    def GetHardwareInfo(self):
        currentVolume = self.__m_Pump.Volume()
        hardwareHomed = self.__m_Pump.getHomeStatus()
        logicallyHomed = int(self.__m_Pump.HomeStep()) == int(self.__m_Pump.Step())
        #print '>>>>>>> %d ----- %d' % (self.__m_Pump.HomeStep(), self.__m_Pump.Step())
        return (currentVolume, hardwareHomed, logicallyHomed)

# eof
