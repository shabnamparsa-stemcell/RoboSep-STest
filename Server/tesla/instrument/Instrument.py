# Instrument.py
# tesla.instrument.Instrument
#
# Encapsulates the level 1 workflows of the Tesla instrument
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
# Notes:
# 	 03/24/05 - added parkarm - bdr
#    04/11/05 - added parkpump - bdr
#    05/09/05 - lysis 'deadVolume' now 5 mL - bdr
#    05/29/05 - added parkCarousel (line 784) - bdr
#    03/02/06 - Ian's small tip mix fix plus my fix (TipToMixWith)- RL
#    03/20/06 - Small vial mix assumes vial has 1mL regardless - RL
#    03/21/06 - transport from buffer bottle is always from the bottom - RL
#    03/21/06 - relative multi tiprack fix - RL
#    03/27/06 - move arm slowly for buffer bottle - RL
#    03/29/06 - pause command - RL
#    09/05/07 - usebuffertip command added - bdr
#    09/20/13 - modify return code of ScanVialBarcodeAt function
#    10/16/13 - modify barcode method to rescan moving in one direction - sp
#    10/31/13 - add Hardware.ini vaules([Barcode_reader]max_count=3;max_rescan=10)-CJ

import math

from tesla.exception import TeslaException
import tesla.logger
import tesla.config
from tesla.hardware.config import gHardwareData, LoadSettings
from tesla.hardware.Device import Device
from tesla.hardware.AxisTracker import AxisTracker
from tesla.instrument.TeslaPlatform import TeslaPlatform
from tesla.instrument.TeslaPump import TeslaPump
from tesla.types.VolumeProfile import *
from tesla.types.tube import *
from tesla.types.TubeGeometry import TubeGeometry
from tesla.types.VolumeProfile import *
from tesla.instrument.Subsystem import Subsystem
from tesla.instrument.Calibration import Calibration
from tesla.hardware.BarCodeReader import BarCodeReader      # 2012-01031 sp -- added barcode reader

import Queue
import threading

from ipl.task.future import *
from ipl.utils.wait import wait_msecs

import os       #CWJ Downstroke Pause Test

from tesla.PgmLog import PgmLog    # 2011-11-25 sp -- programming logging
from ConfigParser import *         # 2012-02-01 sp -- configuration from ini file
import tesla.DebuggerWindow          # 2012-04-10 sp

from datetime import datetime #added by shabnam

import RoboTrace              	#CWJ Add
import time		  	#CWJ Add
# ---------------------------------------------------------------------------

class InstrumentError(TeslaException):
    """Exception for errors related to invalid instrument settings."""
    pass

# ---------------------------------------------------------------------------

class Instrument(Subsystem):
    """Class to control the instrument hardware to execute the Tesla instrument
    level 1 workflows."""

    # Reagent types (may need to be mapped to tubes)
    AntibodyLabel       = "AntibodyVial"
    CocktailLabel       = "CocktailVial"
    BeadLabel           = "ParticleVial"
    SampleLabel         = "SampleVial"
    SupernatentLabel    = "SeparationVial"
    WasteLabel          = "WasteVial"
    LysisLabel          = "LysisVial"
    BCCMLabel           = "BCCMContainer"
    TeslaLabel          = "Tesla"

    # A label of all reagents on the carousel (not BCCM)
    ReagentLabelSet = (AntibodyLabel, CocktailLabel, BeadLabel, SampleLabel,
			SupernatentLabel, WasteLabel, LysisLabel)

    # Configuration data    
    Tip1mlCapacityLabel = "capacity_1mltip_ul"
    Tip5mlCapacityLabel = "capacity_5mltip_ul"
    AspirationDepthLabel        = "aspirationdepthundermeniscus_mm"
    WickingDispenseDepthLabel   = "wickingdispensedepthundermeniscus_mm"
    MixingIterationsLabel       = "numberofmixingcycles"
    MixingVolumePercentageLabel = "mixingvolumepercentage"
    MixingVolumeMaxLabel    = "mixingvolumemaximum_ul"
    MixingPreMixAirSlugLabel    = "mixingpremixairslug_ul"
    MixingPauseLabel        = "mixingpause_msec"
    MixingPistonSettlingDelayLabel = "mixingpistonsettlingdelay_msec"
    MixingDispenseOffsetLabel = "mixingdispenseoffset_mm"
    MixingTip5mlLabel       = "mixingtipposition_5ml"
    MixingTip1mlLabel       = "mixingtipposition_1ml"
    
    BaseAntibodyLabel       = "baseoffset_antibodyvial_mm"
    BaseCocktailLabel       = "baseoffset_cocktailvial_mm"
    BaseBeadLabel           = "baseoffset_beadvial_mm"
    Base14mlLabel           = "baseoffset_14mlvial_mm" 
    Base50mlLabel           = "baseoffset_50mlvial_mm" 
    BaseBCCMLabel           = "baseoffset_bulkcontainer_mm"
    DeadVolumeAntibodyLabel = "deadvolume_antibodyvial_ul"
    DeadVolumeCocktailLabel = "deadvolume_cocktailvial_ul"
    DeadVolumeBeadLabel     = "deadvolume_beadvial_ul"
    DeadVolume14mlLabel     = "deadvolume_14mlvial_ul" 
    DeadVolume50mlLabel     = "deadvolume_50mlvial_ul" 
    DeadVolumeBCCMLabel     = "deadvolume_bulkcontainer_ul"
    MinTipBaseGapLabel      = "minimumallowedvialbasetotipclearance_mm"
    VolumeAfterTopUpLabel   = "volumeaftertopup_ul"
    Min5mlTipVolumeLabel     = "minimumallowed5mltipvolume_ul"
    WickingExtractVelocityProfileLabel = "wickingextractvelocityprofile"
    AspirateWickingExtractOffsetLabel  = "wickingextractoffset_aspirate_mm"
    AspirateReagentWickingExtractOffsetLabel  = "wickingextractoffset_aspirate_reagent_mm"
    AspirateBCCMWickingExtractOffsetLabel  = "wickingextractoffset_aspirate_bccm_mm"
    DispenseWickingExtractOffsetLabel  = "wickingextractoffset_dispense_mm"
    MixWickingExtractOffsetLabel   = "wickingextractoffset_mix_mm"
    TransportFlushEnabledLabel = "transportflushenabled"
    
    #VolumeProfile Labels definition
    HemisphericalFullHeight14mlLabel = "hemisphericalprofilefullvolume_14mlvial"
    CylindricalFullVolume14mlLabel = "cylindricalprofilefullvolume_14mlvial"
    CylindricalFullHeight14mlLabel = "cylindricalprofilefullheight_14mlvial"
    ConicalFullHeight50mlLabel = "conicalprofilefullheight_50mlvial"
    ConicalFullVolume50mlLabel = "conicalprofilefullvolume_50mlvial"
    CylindricalFullHeight50mlLabel = "cylindricalprofilefullheight_50mlvial"
    CylindricalFullVolume50mlLabel = "cylindricalprofilefullvolume_50mlvial"
    CylindricalFullVolumeReagentlLabel = "cylindricalprofilefullvolume_reagentvial"
    CylindricalFullHeightReagentlLabel = "cylindricalprofilefullheight_reagentvial"
    DeadVolumeProfileBulkLabel = "deadvolumeprofiledeadvolume_bulkbottle"
    CylindricalFullVolume1BulkLabel = "cylindricalprofilefullvolume1_bulkbottle"
    CylindricalFullHeight1BulkLabel = "cylindricalprofilefullheight1_bulkbottle"
    TruncatedConicalFullHeightBulkLabel = "truncatedconicalprofilefullheight_bulkbottle"
    TruncatedConicalFullVolumeBulkLabel = "truncatedconicalprofilefullvolume_bulkbottle"
    ConicalFullHeightBulkLabel = "conicalprofilefullheight_bulkbottle"
    ConicalFullVolumeBulkLabel = "conicalprofilefullvolume_bulkbottle"
    CylindricalFullHeight2BulkLabel = "cylindricalprofilefullheight2_bulkbottle"
    CylindricalFullVolume2BulkLabel = "'cylindricalprofilefullvolume2_bulkbottle"

    ThetaParkPositionLabel = "thetaparkposition_degrees"
    ZParkPositionLabel = "zparkposition_mm"

    MixingVolumeFixedSmallVialVolumeLabel = "mixingvolume_fixedsmallvialvolume"
    MixingVolumeFixedSmallVialMixVolumeLabel = "mixingvolume_fixedsmallvialmixvolume"

    SubForTransSepTransOnlyIfSepIsLessThanSecondsLabel = "subfor_transseptrans_onlyif_sep_is_lessthan_seconds"
    ApplyCommandSubstitutionLogicLabel = "apply_cmd_sub_logic"

    Filled2mlVialVolumelLabel = "filledvialvolume_2ml_uL"

    BCCMContainerName           = "bccm_containername"
    WasteVialName          = "waste_vialname"
    LysisVialName          = "lysis_vialname"
    CocktailVialName       = "cocktail_vialname"
    BeadVialName          = "particle_vialname"
    AntibodyVialName       = "antibody_vialname"
    SampleVialName         = "sample_vialname"
    SupernatentVialName    = "separation_vialname"
    

    # changed DeadVolume50mlLabel to 500 from 100 ml - bdr

    __configData =  {
                    BaseBCCMLabel               : '231.55',
                    BaseAntibodyLabel           : '203.5',
                    BaseCocktailLabel           : '203.5',
                    BaseBeadLabel               : '184.5',
                    Base14mlLabel               : '230.0',
                    Base50mlLabel               : '246.0',
                    DeadVolumeBCCMLabel         : '100',
                    DeadVolumeAntibodyLabel     : '100',
                    DeadVolumeCocktailLabel     : '100',
                    DeadVolumeBeadLabel         : '100',
                    DeadVolume14mlLabel         : '100',
                    DeadVolume50mlLabel         : '500',
                    Tip1mlCapacityLabel         : '1100',
                    Tip5mlCapacityLabel         : '5000',
                    AspirationDepthLabel        : '3',
                    WickingDispenseDepthLabel   : '3',
                    MinTipBaseGapLabel          : '2',
                    MixingIterationsLabel       : '3',
                    MixingVolumePercentageLabel : '50',
                    MixingVolumeMaxLabel        : '3600',
                    MixingPreMixAirSlugLabel    : '200',
                    MixingPauseLabel            : '0',
                    MixingPistonSettlingDelayLabel : '100',
                    MixingDispenseOffsetLabel   : '0',
                    MixingTip5mlLabel           : '5',
                    MixingTip1mlLabel           : '3',
                    VolumeAfterTopUpLabel       : '10000',
                    Min5mlTipVolumeLabel        : '100',
                    WickingExtractVelocityProfileLabel : '2000, 2510, 5',
                    AspirateWickingExtractOffsetLabel   : '10',
                    AspirateReagentWickingExtractOffsetLabel   : '10',
                    AspirateBCCMWickingExtractOffsetLabel   : '50',
                    DispenseWickingExtractOffsetLabel   : '10',
                    MixWickingExtractOffsetLabel: '30',
                    TransportFlushEnabledLabel: '1',
                    ThetaParkPositionLabel: '130',
                    ZParkPositionLabel: '100',

                    MixingVolumeFixedSmallVialVolumeLabel: '600',
                    MixingVolumeFixedSmallVialMixVolumeLabel: '1100',
                    
                    HemisphericalFullHeight14mlLabel : '1000',
                    CylindricalFullVolume14mlLabel : '13000',
                    CylindricalFullHeight14mlLabel : '85',
                    ConicalFullHeight50mlLabel : '18',
                    ConicalFullVolume50mlLabel : '5000',
                    CylindricalFullHeight50mlLabel : '95',
                    CylindricalFullVolume50mlLabel : '53000',
                    CylindricalFullVolumeReagentlLabel : '2250',
                    CylindricalFullHeightReagentlLabel : '43',
                    DeadVolumeProfileBulkLabel : '15000',
                    CylindricalFullVolume1BulkLabel : '300000',
                    CylindricalFullHeight1BulkLabel : '52',
                    TruncatedConicalFullHeightBulkLabel : '11',
                    TruncatedConicalFullVolumeBulkLabel : '5000',
                    ConicalFullHeightBulkLabel : '978',
                    ConicalFullVolumeBulkLabel : '3960',
                    CylindricalFullHeight2BulkLabel : '41',
                    CylindricalFullVolume2BulkLabel : '48000',

                    Filled2mlVialVolumelLabel : '1100',

                    BCCMContainerName : 'Buffer Bottle',
                    WasteVialName : 'Waste Tube',
                    LysisVialName: 'Lysis Buffer Tube',
                    CocktailVialName : 'Cocktail Vial (Square)',
                    BeadVialName : 'Magnetic Particle Vial (Triangle)',
                    AntibodyVialName : 'Antibody Vial (Circle)',
                    SampleVialName : 'Sample Tube',
                    SupernatentVialName : 'Separation Tube'

                    }
    
    # Map reagent names to actual tube positions (if names vary):
    __reagentPositionMap =  {
                            BeadLabel        : TeslaPlatform.ParticleVial_Label,
                            CocktailLabel    : TeslaPlatform.CocktailVial_Label,
                            AntibodyLabel    : TeslaPlatform.AntibodyVial_Label,
                            SampleLabel      : TeslaPlatform.Sample_Label, 
                            SupernatentLabel : TeslaPlatform.Separation_Label, 
                            WasteLabel       : TeslaPlatform.Waste_Label, 
                            LysisLabel       : TeslaPlatform.LysisBuffer_Label, 
                            BCCMLabel        : TeslaPlatform.BulkLabel,
                            }

    # Tip capacity
    __tipCapacityMap =  {
                        1 : Tip1mlCapacityLabel,
                        2 : Tip1mlCapacityLabel,
                        3 : Tip1mlCapacityLabel,
                        4 : Tip5mlCapacityLabel,
                        5 : Tip5mlCapacityLabel,
                        None : 0,
                        }

    # Tip usage for transport workflow
    # NOTE (GJC): I'm assuming the magic numbers here relate back to the
    #             __tipCapacityMap above?
    __transportTipUsageMap = {
                                CocktailLabel    : 1,
                                BeadLabel        : 2,
				                AntibodyLabel	 : 3,
                                BCCMLabel        : 4,
                                SampleLabel      : 5,
                                SupernatentLabel : 5,
				                LysisLabel	     : 5,
                                WasteLabel       : 5,       # should not be needed
                             }

    # List tubes for which a given tip is is not allowed entry
    tips_All = range(1,6)
    tips_1ml = (1, 2, 3)
    tips_5ml = (4, 5)
    
    __forbiddenTipList =     {
                                # WasteLabel : tips_All,
                                CocktailLabel : tips_5ml,
				AntibodyLabel : tips_5ml,
                                }

    __m_state = 'RESET'

    bBarcodeInit = False;
    
    barcodeScanningThread = None
    
    abortScanning = False

    # ---------------------------------------------------------------------------
    
    def __init__( self, name ):
        """Set up the instrument components:
            - the TeslaPlatform, which handles mechanical actions.
            - the TeslaPump, which handles fluidics
        """
        global gHardwareData
        Subsystem.__init__( self, name )

        # Enable logging to the global Tesla event/error logger
        # Then register the logger with our Device class (for logging at the
        # device level)
        self.__logger = tesla.logger.Logger(logName = tesla.config.LOG_PATH)
        Device.registerLogger(self.__logger)

        # 2011-11-25 sp -- added logging
        self.svrLog = PgmLog( 'svrLog' )
        self.logPrefix = 'IN'
        
        # Set the tracker database for all components
        AxisTracker.dbFilePath = tesla.config.COMPONENT_DB_PATH

        # Alias the Incubate() method to Separate() for protocols
#	self.Separate = self.Incubate #CWJ Mod

        self._m_Settings = Instrument.__configData.copy()
        instrumentConfigData = gHardwareData.Section (Instrument.TeslaLabel)
        LoadSettings (self._m_Settings, instrumentConfigData)

        self.__m_Platform = TeslaPlatform( "TeslaPlatform" )
        self.__nbrSectors = self.__m_Platform.NbrSectors()
        self.__sectorRange = range(1, self.__nbrSectors + 1)
        
        self.__m_Pump = TeslaPump( "TeslaPump" )
        self.__m_Containers = self.InitialiseContainers()

        card = self.__m_Pump.GetPumpCard()                              #CWJ Add
        self.__m_Platform.AttatchHydraulicSensor( card )                #CWJ Add
        
        card = self.__m_Pump.GetPumpCard()                              #CWJ Add
        self.__m_Platform.AttatchBeaconDriver( card )                   #CWJ Add

        self.__m_thetaParkPosition = \
            float( self._m_Settings[Instrument.ThetaParkPositionLabel] )
        self.__m_zParkPosition = \
            float( self._m_Settings[Instrument.ZParkPositionLabel] )
        
        self.__m_RootQuadrantSampleId = [None,None,None,None]
        self.__mixingFraction = float (self._m_Settings[Instrument.MixingVolumePercentageLabel])/100.
        self.__mixingAirSlugVolume = float (self._m_Settings[Instrument.MixingPreMixAirSlugLabel])
        self.__maxMixingVolume = float (self._m_Settings[Instrument.MixingVolumeMaxLabel]) - self.__mixingAirSlugVolume # Note adjustment
        self.__nbrMixes = int (self._m_Settings[Instrument.MixingIterationsLabel])
        self.__mixingPause = int (self._m_Settings[Instrument.MixingPauseLabel])
        self.__mixingPistonSettlingDelay = int( self._m_Settings[Instrument.MixingPistonSettlingDelayLabel] )
        self.__mixingDispenseOffset = float (self._m_Settings[Instrument.MixingDispenseOffsetLabel])
        self.__aspirateReagentWickingExtractOffset = \
            float( self._m_Settings[Instrument.AspirateReagentWickingExtractOffsetLabel] )
        self.__aspirateBCCMWickingExtractOffset = \
            float( self._m_Settings[Instrument.AspirateBCCMWickingExtractOffsetLabel] )
        self.__aspirateWickingExtractOffset = float (self._m_Settings[Instrument.AspirateWickingExtractOffsetLabel])
        self.__dispenseWickingExtractOffset = float (self._m_Settings[Instrument.DispenseWickingExtractOffsetLabel])
        self.__mixWickingExtractOffset = float (self._m_Settings[Instrument.MixWickingExtractOffsetLabel])
        self.__topupVolume = int (self._m_Settings[Instrument.VolumeAfterTopUpLabel])
        self.__min5mlTipVolume = int (self._m_Settings[Instrument.Min5mlTipVolumeLabel])

        self.__transportFlushEnabled = int( self._m_Settings[Instrument.TransportFlushEnabledLabel] ) != 0

        self.__m_isPause = False
        self.__m_pauseCaption = ""

        self.__mixingFixedSmallVialVolume = float (self._m_Settings[Instrument.MixingVolumeFixedSmallVialVolumeLabel])
        self.__mixingFixedSmallVialMixVolume = float (self._m_Settings[Instrument.MixingVolumeFixedSmallVialMixVolumeLabel])

        self.__filled2mlVialVolume = float(self._m_Settings[Instrument.Filled2mlVialVolumelLabel])

        # 2012-04-10 sp -- added barcode support
        if( tesla.config.SS_DEBUGGER_LOG == 1 ):
            ssLogDebugger = tesla.DebuggerWindow.GetSSTracerInstance()
            ssLogDebugger.setInstrument( self, self.__m_Platform )
#        debugHandler = tesla.DebugHandler.GetSSDebugHandlerInstance()
#        debugHandler.setInstrument( self, self.__m_Platform )

        self.OpenBarcodePort()
#        self.DetachBarcode();
#        self.AttachBarcode();
                
    # --- The level 1 workflow methods ----------------------------------------
   
    def HomeAll(self, id = None, **kw):
	'''Home all axes.
	id is a unique identifier (eg. sample ID) for tracking the caller.
	kw is a dictionary of optional parameters & values.'''
        
        print '===============HomeAll Start'
        
        self.__m_Platform.BeaconDriver.TurnONBeacon(3);   #CWJ Add
        time.sleep(3);                                    #CWJ Add
        self.__m_Platform.BeaconDriver.TurnOFFBeacon();   #CWJ Add

        # Nov 7, 2013 Sunny
        # Update seq for maintenance protocol        
        seq = -1
        if kw.has_key('seq'):
            seq = - kw['seq']
                
      	CamMgr = RoboTrace.GetRoboCamMgrInstance();
        CamMgr.SetProcStep(1);
        CamMgr.SetCurrentProtocolID(-1)
        CamMgr.SetCurrentProtocolSeq(seq)
        CamMgr.SetCurrentSec(-1)

        self.__m_Platform.Initialise()
        self.__m_Pump.Initialise( False )

        self.__m_Platform.BeaconDriver.TurnOFFBeacon();   #CWJ Add
        print '===============HomeAll End'
    


    #IAT - added values for tiprack (1) and tiprackSpecified (false).  
    #These values MUST be present for the command to function correctly.
    #CR - added tiprackSpecified and tiprack variables to input

 

    # -------------------------------------------------------------------------
    # --- TRANSPORT -----------------------------------------------------------
    # -------------------------------------------------------------------------
    
    def Transport(self, destVial, srcVial, volume_ul, usingFreeAirDispense=False, usingBufferTip=False, id = None, tiprackSpecified = False, tiprack = 1, skipTipStrip = False, skipPickupTip = False, **kw):

        '''Transport a given volume of fluid (in uL) from srcVial to destVial. If
        usingFreeAirDispense is True, we will do a free air dispense for all
        dispenses involved in this transport workflow.
        id is a unique identifier (eg. sample ID) for tracking the caller.
        kw is a dictionary of optional parameters & values.'''
        #print '--------------------------roy-a0---',srcVial.getLabel(),destVial.getLabel()
        #print datetime.now() # shabnam 
        funcReference = __name__ + '.Transport'  # 2011-11-25 sp -- added logging
        if kw.has_key('seq'):   #CWJ Mod
            seq = kw['seq']      #CWJ Mod
            CamMgr = RoboTrace.GetRoboCamMgrInstance();
            CamMgr.SetProcStep(2);        	   
            CamMgr.SetCurrentProtocolSeq(seq)
            CamMgr.SetCurrentProtocolID(id)
        if kw.has_key('initialQuadrant'): #CWJ Mod
           qdr = kw['initialQuadrant']
           CamMgr = RoboTrace.GetRoboCamMgrInstance()
           CamMgr.SetCurrentSec(qdr)
        
        if volume_ul <= 0:
            zeroVolMsg = "ID = %s: No transport (volume = %d uL)" % (id, volume_ul)
            self.__logger.logDebug(zeroVolMsg)
            self.svrLog.logInfo('', self.logPrefix, funcReference, 'ID=%s |No transport, volume=%d' % (id, volume_ul))   # 2011-11-25 sp -- added logging
            self.Trace(zeroVolMsg)
            return			    # Nothing to move! Do nothing
        
        # Check vial volume levels
        # Case 1: we want to transport more than is in the source vial
        volume_ul = Instrument.GetVolumeToTransport(srcVial,volume_ul,id,self.__logger,self.svrLog,self.logPrefix,funcReference,True)

        if (destVial.getVolume() + volume_ul > destVial.getMaxVolume()):
            # Case 2: we want to transport more than can fit in the destination vial
            self.svrLog.logError('', self.logPrefix, funcReference, 'Volume will overfill destination, ID=%s |Transport vol=%d |dest=%s |current dest vol=%d' % \
                    (id, volume_ul, srcVial.getLabel(), srcVial.getVolume()))   # 2011-11-25 sp -- added logging
            raise InstrumentError ("IL: (ID=%s) Volume being transported (%d uL) will overfill %s (currently at %d uL)." % \
                (id, volume_ul, destVial.getLabel(), destVial.getVolume()))

        (srcSector, srcLocation) = self.__LocationOf(srcVial)
        (destSector, destLocation) = self.__LocationOf(destVial)

        # bdr - test tiprack specified
        if tiprackSpecified == True:
            print "Specified TipRack of %d" % (tiprack)
        else :
            print "Tip rack was not specified"


        # RL - transport from buffer bottle is always from the bottom - 03/21/06
        #      (dead) mL - 10mL
        if srcVial.getLabel() == Instrument.BCCMLabel:
            srcVial.setVolume(float(self._m_Settings[Instrument.DeadVolumeBCCMLabel]) - 10000)
            print "transport from bccm", srcVial.getVolume()
            
        # Set up a debug message for tracing transported volumes
        transportMsg = "ID = %s: Transporting %duL from %d:%s (%duL) to %d:%s (%duL)" % \
                (id, volume_ul, srcSector, srcVial.getLabel(), srcVial.getVolume(),
                    destSector, destVial.getLabel(), destVial.getVolume())
        self.__logger.logDebug(transportMsg)
        self.svrLog.logInfo('', self.logPrefix, funcReference, transportMsg )   # 2011-11-25 sp -- added logging
        self.Trace(transportMsg)

        try:        
            # Select tip for transport
            (chosenSector, chosenTip) = self.__TipToUseFor (srcVial)
            (chosenSector2, chosenTip2) = self.__TipToUseFor (destVial)

            #2017_05_01 should use small tip if destination is reagent vial
            if chosenTip2 == 1 or chosenTip2 == 2 or chosenTip2==3:
                transportMsg = "Destination Vial requires small tip for transport"
                print transportMsg
                self.__logger.logDebug(transportMsg)
                self.svrLog.logInfo('', self.logPrefix, funcReference, transportMsg )
                chosenSector = chosenSector2
                chosenTip = chosenTip2
                
            
            # bdr - override normal tip on useBufferTip set
            print "   BDR - Chosen tip  %d" % (chosenTip)
            if chosenTip in Instrument.tips_5ml and usingBufferTip == True:
                if chosenTip == 5:
                    chosenTip = 4
                else:
                    chosenTip = 5
                
                print "   BDR - new Chosen tip  %d" % (chosenTip)

            # RL - relative multi tiprack fix -03/21/06
            if kw.has_key('initialQuadrant'):
                chosenSector = kw['initialQuadrant']

            #Is tiprack specified?
            if tiprackSpecified == True:
                chosenSector += tiprack - 1

            if chosenTip in Instrument.tips_5ml and volume_ul < self.__min5mlTipVolume:
                # Note problem, and ignore transport
                self.__logger.logInfo("IL: (ID=%s) Cannot accurately transport %duL using a 5mL tip. Ignoring action." % \
                            (id, volume_ul))
                self.svrLog.logInfo('', self.logPrefix, funcReference, 'Cannot transport accurately using 5ml tip, ID=%s |volume=%d' % \
                            (id, volume_ul))   # 2011-11-25 sp -- added logging
                return

            if self.__TipIsForbiddenToEnter (srcVial, chosenTip):
                self.svrLog.logError('', self.logPrefix, funcReference, 'Cannot transport with chosen tip, ID=%s |tip=%d |vial=%s' % \
                            (id, chosenTip, srcVial.getLabel()))   # 2011-11-25 sp -- added logging
                raise InstrumentError ("IL: (ID=%s) Chosen transport tip (%d) may not aspirate from %s" % \
                    (id, chosenTip, srcVial.getLabel()))

            # If there is a root sector associated with this sample use that sector,
            # otherwise make the association to the source (tip) sector.
            for rootQuadrant in self.__sectorRange:
                if self.__m_RootQuadrantSampleId[rootQuadrant-1] == id:
                    break
            else:
                self.__m_RootQuadrantSampleId[srcSector-1] = id
                if srcSector != 0:
                    rootQuadrant = srcSector
                else:
                    rootQuadrant = destSector
                
            # Now do the actual flush using the evaluated flush sector
            if self.__transportFlushEnabled:
                self.Flush(self.ContainerAt(rootQuadrant, Instrument.WasteLabel), id = id)
            
            # Update flush sector to current sample
#            if srcSector != 0:
#                self.__m_FlushSector = srcSector
            
            if chosenSector == 0:
                # This may happen if transporting from bulk, use the destination sector instead
                if destSector == 0:
                    self.svrLog.logError('', self.logPrefix, funcReference, "Can't select tip for sector 0, ID=%s" % id)   # 2011-11-25 sp -- added logging
                    raise InstrumentError ("IL: (ID=%s) Can't select tip for sector 0" % (id))
                else:
                    chosenSector = destSector

            tipCapacity = self.__TipCapacityOf (chosenTip)
            if not skipPickupTip:
                self.__UseTip (chosenSector, chosenTip)
            else:
                self.__logger.logDebug("IL: SKIP tip pickup, ignore chosen tip")
                if self.__m_Platform.CurrentTipID()[1] == None:
                    self.__logger.logDebug("IL: no tip detected, pickup chosen tip")
                    self.__UseTip (chosenSector, chosenTip)
                    
                

            # Proceed with transportation (ensure that volume in each transport
            # is about even and that we cleanly handly transporting a zero 
            # volume)
            nbrTransports = int (math.ceil(volume_ul / tipCapacity))
            if nbrTransports > 0:
                volumePerTransport = int(math.ceil(float(volume_ul) / nbrTransports)) # Round up here: it avoids 1ul leftovers!
                volumeRemaining = volume_ul
            else:
                self.__logger.logDebug("IL: No transport from %s to %s? Volume = %d, tip capacity = %d" % \
                                        (srcVial, destVial, volume_ul, tipCapacity))
                self.svrLog.logInfo('S', self.logPrefix, funcReference, "No transport performed, src=%s |dest=%s | volume=%d |tip capacity=%d" % \
                                        (srcVial, destVial, volume_ul, tipCapacity))   # 2011-11-25 sp -- added logging
                volumePerTransport = 0
                volumeRemaining = 0

            while volumeRemaining > 0:
                # Transport the required volume between vial in lots
                volumeToTransport = min (volumeRemaining, volumePerTransport)
                volumeAspirated = self._Aspirate (volumeToTransport, srcVial, tipCapacity)
                
                #self.__logger.logDebug( "@@@@@@@@@@@@@@@@@@@@@@@@@@@ _Dispense %f %s %f" % (volumeAspirated, destVial, usingFreeAirDispense))
                volumeDispensed = self._Dispense (volumeAspirated, destVial, usingFreeAirDispense)
                volumeRemaining = volumeRemaining - volumeToTransport

        finally:
            # Strip tip and park valve
            self.__m_Pump.Initialise( True )
            self.__m_Pump.ParkValve()
            # 2012-01-30 sp -- replace environment variable with configuration variable
            #if os.environ.has_key('SS_CHATTER'):
            if tesla.config.SS_CHATTER == 1:
                data = self.__m_Platform.robot.moveToHomeAndCheck()
                self.__logger.logInfo( "moveToHomeAndCheck Before StripTip (To Start Point:%d, To Home:%d)"%(data[0],data[1]))
                self.svrLog.logInfo('', self.logPrefix, funcReference, "moveToHomeAndCheck Before StripTip, (To Start Point=%d |To Home=%d)"%(data[0],data[1]))   # 2011-11-25 sp -- added logging
            else:
               self.__m_Platform.robot.moveToHomeAndCheck() 

            
            if not skipTipStrip:
                #    May/04/2009 - 50 mL TipStrip Method Changes - CWJ 
                if ( destVial.getLabel() == self.LysisLabel ) :
                    print '\nCWJ - Lysis Vial Delay TipStip\n'
                    self.__m_Platform.StripTip(True) 
                else :                
                    self.__m_Platform.StripTip() #    03/14/07 - tip strip only if necessary code -RL
            else:
                self.__logger.logDebug("IL: SKIP tip strip")
	
    def GetVolumeToTransport(srcVial,volume_ul,id,logger,svrLog,logPrefix,funcReference, doLogging = False):
        if (srcVial.getVolume() < volume_ul):
            # Case 1: we want to transport more than is in the source vial
            warningMsg = "IL: (ID=%s) Volume being transported (%d uL) exceeds amount in %s (%d uL)." % \
                    (id, volume_ul, srcVial.getLabel(), srcVial.getVolume())
            # raise InstrumentError (warningMsg)
            if doLogging:
                logger.logWarning(warningMsg)
                svrLog.logWarning('', logPrefix, funcReference, 'Exceeded volume in source, ID=%s |Transport vol=%d |src=%s |src vol=%d' % \
                        (id, volume_ul, srcVial.getLabel(), srcVial.getVolume()))   # 2011-11-25 sp -- added logging
            # To address the problem of trying to transport more than we might have
            # in the vial, only transport what is in there
            new_volume_ul = srcVial.getVolume()
            if doLogging:
                logger.logInfo("IL: (ID=%s) Setting transport volume to %d uL" % (id, new_volume_ul))
                svrLog.logInfo('', logPrefix, funcReference, 'ID=%s |Transport volume=%d' % (id, new_volume_ul))   # 2011-11-25 sp -- added logging
            return new_volume_ul
        else:
            return volume_ul

    def Mix(self, vial, tiprackSpecified, tiprack, mixCycles, tipTubeBottomGap, isRelative, volume_uL, id = None, skipTipStrip = False, skipPickupTip = False, **kw):#CR
        '''Mix fluid in the given vial.	id is a unique identifier (eg. sample ID) for tracking the caller.
	       kw is a dictionary of optional parameters & values.'''

        funcReference = __name__ + '.Mix'  # 2011-11-25 sp -- added logging
        CamMgr = RoboTrace.GetRoboCamMgrInstance();
        CamMgr.SetProcStep(3);
        CamMgr.SetCurrentProtocolID(id)
        if kw.has_key('seq'):   #CWJ Mod
           seq = kw['seq']      #CWJ Mod
           CamMgr = RoboTrace.GetRoboCamMgrInstance()
	   CamMgr.SetCurrentProtocolSeq(seq)

        if kw.has_key('initialQuadrant'): #CWJ Mod
           qdr = kw['initialQuadrant']
           CamMgr = RoboTrace.GetRoboCamMgrInstance()
           CamMgr.SetCurrentSec(qdr)

        try:
            # Obtain the required mixing tip
            (chosenSector, chosenTip) = self.__TipToMixWith (vial)
            
            # RL - relative multi tiprack fix -03/21/06
            if kw.has_key('initialQuadrant'):
                chosenSector = kw['initialQuadrant']

            #Is tiprack specified?
            if tiprackSpecified == True:
                chosenSector += tiprack - 1

            tipCapacity = self.__TipCapacityOf (chosenTip)

            #RL Small vial mix assumes vial has smallVialMixing_VialVolume regardless 03/20/06
            #   vial volume determines the height of asp/disp

            #RL NOW use __mixingFixedSmallVialVolume and __mixingFractionSmallVial 05/24/06

            tmp_volume = vial.getVolume()
            mixFraction = self.__mixingFraction

            
            #unspeicified small vial mix
            if volume_uL<=0 and chosenTip in Instrument.tips_1ml:
                vial.setVolume(self.__mixingFixedSmallVialVolume)
            #absolute
            elif volume_uL > 0 and not bool(isRelative):
                vial.setVolume(tipTubeBottomGap)
                mixFraction = 0

            mixVolume = vial.getVolume() * mixFraction
            #relative 
            if volume_uL > 0 and bool(isRelative):
                #volume_uL for relative is proportion
                mixVolume = volume_uL*tmp_volume
                mixFraction = -1
            mixVolume = min (tipCapacity, mixVolume, self.__maxMixingVolume)


            #print "Mix vial.getVolume() %d mixFraction %f  MixVolume %f " % (vial.getVolume(), mixFraction, mixVolume)
            print "Mix TipTubeBottomGap %d" % (vial.getVolume()-mixVolume)
            

            # Prior to mixing, obtain an air slug ...
            future = Future( self.__m_Pump.AspiratePreMixAirSlug, self.__mixingAirSlugVolume )
            # ... while we pick up the tip
            if not skipPickupTip:
                self.__UseTip (chosenSector, chosenTip)
            else:
                self.__logger.logDebug("IL: SKIP tip pickup, ignore chosen tip")
                if self.__m_Platform.CurrentTipID()[1] == None:
                    self.__logger.logDebug("IL: no tip detected, pickup chosen tip")
                    self.__UseTip (chosenSector, chosenTip)
            future()
            #print "------------------------------------ROY v =",mixVolume," v2=", vial.getVolume()
            # Move to start aspirating initial cycle
            
            self.MoveToAspirate (vial, mixVolume)
            zAxis = self.__m_Platform.robot.Z()

            # Determine the aspiration and dispense heights to use when mixing.
            # (Dispense height is offset from aspiration height, after clipping)
            aspirationHeight = zAxis.Position()
            dispenseHeight = self.__ClipHeight( aspirationHeight+self.__mixingDispenseOffset, \
                                                vial, self.__m_Platform.CurrentTipLength() )

            #unspeicified small vial mix
            if volume_uL<=0 and chosenTip in Instrument.tips_1ml:
                vial.setVolume(tmp_volume)
                mixVolume = self.__mixingFixedSmallVialMixVolume
            #absolute
            elif volume_uL > 0 and not bool(isRelative):
                vial.setVolume(tmp_volume)
                mixVolume = volume_uL

            print "Mix volume reset vial.getVolume() %d " % (vial.getVolume())
            print "Mix tipCapacity %d mixVolume %d aspirationHeight %d dispenseHeight %d" % (tipCapacity, mixVolume, aspirationHeight, dispenseHeight)

            MixLoop = self.__nbrMixes

            self.svrLog.logInfo('', self.logPrefix, funcReference, 'start mix; sector=%d |tip=%d |vial=%s |mixVolume=%d |cycles=%d' %
                                (chosenSector, chosenTip, vial, mixVolume, MixLoop))   # 2011-11-25 sp -- added logging

            if mixCycles != -1:
                MixLoop = mixCycles
            for i in range (0, MixLoop):
                volumeAspirated = self.__m_Pump.AspirateMix (mixVolume, i)
                wait_msecs (self.__mixingPause)
                zAxis.SetPosition (dispenseHeight)
                self.__m_Pump.DispenseMix ()
                zAxis.SetPosition (aspirationHeight)

            wait_msecs( self.__mixingPistonSettlingDelay )

            # Dispense the air slug. This should ensure that all fluid is dispensed
            self.__m_Pump.DispensePreMixAirSlug (self.__mixingAirSlugVolume)

            # Now move out of the tube, using the wicking extract
            future = Future( self.__WickingExtract, self.__mixWickingExtractOffset )
            # finished with valve and tip
            self.__m_Pump.Initialise( True )
            self.__m_Pump.ParkValve()
            future()

        finally:
            # 2012-01-30 sp -- replace environment variable with configuration variable
            #if os.environ.has_key('SS_CHATTER'):
            if tesla.config.SS_CHATTER == 1:
                data = self.__m_Platform.robot.moveToHomeAndCheck()
                self.__logger.logInfo( "moveToHomeAndCheck Before StripTip (To Start Point:%d, To Home:%d)"%(data[0],data[1]))
                self.svrLog.logInfo('', self.logPrefix, funcReference, "moveToHomeAndCheck Before StripTip, (To Start Point=%d |To Home=%d)"%(data[0],data[1]))   # 2011-11-25 sp -- added logging
            else:
               self.__m_Platform.robot.moveToHomeAndCheck() 
            if not skipTipStrip:
                self.__m_Platform.StripTip()
            else:
                self.__logger.logDebug("IL: SKIP tip strip")


    def Incubate(self, period_secs, id = None, **kw):
        '''Incubate for a specified number of seconds.
	id is a unique identifier (eg. sample ID) for tracking the caller.
	kw is a dictionary of optional parameters & values.'''
        funcReference = __name__ + '.Incubate'  # 2011-11-25 sp -- added logging
        cmdName = 'Incubate'
        ProcStep = 4
        self.WaitCommandHelper(funcReference, cmdName, ProcStep, period_secs, id, **kw)
        

    def Separate(self, period_secs, id = None, **kw):         #CWJ MOD
        '''Separate for a specified number of seconds.        #CWJ Mod
        id is a unique identifier (eg. sample ID) for tracking the caller.
        kw is a dictionary of optional parameters & values.'''

        funcReference = __name__ + '.Separate'  # 2011-11-25 sp -- added logging
        cmdName = 'Separate'
        ProcStep = 10
        self.WaitCommandHelper(funcReference, cmdName, ProcStep, period_secs, id, **kw)

    def EndOfProtocol(self, period_secs, id = None, **kw):
        '''Incubate for a specified number of seconds.
	id is a unique identifier (eg. sample ID) for tracking the caller.
	kw is a dictionary of optional parameters & values.'''
        funcReference = __name__ + '.Incubate'  # 2011-11-25 sp -- added logging
        cmdName = 'EndOfProtocol'
        ProcStep = 18
        self.WaitCommandHelper(funcReference, cmdName, ProcStep, period_secs, id, **kw)

    def WaitCommandHelper(self, funcReference, cmdName, ProcStep, period_secs, id = None, **kw):
    
        self.svrLog.logDebug('', self.logPrefix, funcReference, 'start %s; sample=%s |time=%d' % (cmdName, id, period_secs))   # 2011-11-25 sp -- added logging

        CamMgr = RoboTrace.GetRoboCamMgrInstance();
        CamMgr.SetProcStep(ProcStep);
        CamMgr.SetCurrentProtocolID(id)
        if kw.has_key('seq'):   #CWJ Mod
            seq = kw['seq']      #CWJ Mod
            CamMgr.SetCurrentProtocolSeq(seq)
	time.sleep(4)		#CWJ Add


    # RL - pause command - 03/29/06
    def Pause(self, period_secs, id = None, **kw):
	'''kw is a dictionary of optional parameters & values.'''
        print '''Pause here until user resumes... don't know how though... =('''
        funcReference = __name__ + '.Pause'  # 2011-11-25 sp -- added logging
        self.svrLog.logDebug('', self.logPrefix, funcReference, 'start Pause; sample=%s |time=%d' % (id, period_secs))   # 2011-11-25 sp -- added logging

        CamMgr = RoboTrace.GetRoboCamMgrInstance();
        CamMgr.SetProcStep(9);

        self.__m_isPause = True
        if kw.has_key('label'):
            self.__m_pauseCaption = kw['label']


    def TopUpVial(self, destVial, tiprackSpecified, tiprack, srcVial = None, workVolume = 0, usingFreeAirDispense = True,
		    id = None, skipTipStrip = False, skipPickupTip = False, **kw):
        '''Top up the given vial from the given src. By default (srcVial = None), 
	the vial is topped up from the BCCM.
	id is a unique identifier (eg. sample ID) for tracking the caller.
	kw is a dictionary of optional parameters & values.'''

        funcReference = __name__ + '.TopUpVial'  # 2011-11-25 sp -- added logging
        CamMgr = RoboTrace.GetRoboCamMgrInstance();
        CamMgr.SetProcStep(5);
	CamMgr.SetCurrentProtocolID(id)
        if kw.has_key('seq'):   #CWJ Mod
           seq = kw['seq']      #CWJ Mod
           CamMgr = RoboTrace.GetRoboCamMgrInstance()
	   CamMgr.SetCurrentProtocolSeq(seq)
        if kw.has_key('initialQuadrant'): #CWJ Mod
           qdr = kw['initialQuadrant']
           CamMgr = RoboTrace.GetRoboCamMgrInstance()
           CamMgr.SetCurrentSec(qdr)

        initialQuadrant = kw['initialQuadrant']
        # Use the BCCM, if no source vial is specified.
        fromVial = srcVial
        if fromVial == None:
            fromVial = self.BulkBottle()

        # Ensure that the top up volume does not exceed certain limits
        volumeToAdd = Instrument.GetVolumeToAddInTopUp(destVial,workVolume)
        
        ## def Transport(self, destVial, srcVial, volume_ul, usingFreeAirDispense=False, usingBufferTip=False, id = None, tiprackSpecified = False, tiprack = 1, **kw):  
        if volumeToAdd > 0:
            self.svrLog.logInfo('', self.logPrefix, funcReference, "starting TopUp, id=%s |vial=%s |volume=%d" \
                    % (str(id), destVial, volumeToAdd))   # 2011-11-25 sp -- added logging
            if kw.has_key( 'initialQuadrant' ):
                self.Transport(destVial, fromVial, volumeToAdd, usingFreeAirDispense, False, id, tiprackSpecified, tiprack, skipTipStrip = skipTipStrip, skipPickupTip = skipPickupTip, initialQuadrant = kw['initialQuadrant'] ) #CR
            else:
                self.Transport(destVial, fromVial, volumeToAdd, usingFreeAirDispense, False, id, tiprackSpecified, tiprack, skipTipStrip = skipTipStrip, skipPickupTip = skipPickupTip ) #CR
        else:
            # If we have zero (or a negative) volume, skip the transport
            self.__logger.logInfo("IL: Skipping TopUp for ID = %s (requested volume = %d uL)" \
                    % (str(id), volumeToAdd))
            self.svrLog.logInfo('', self.logPrefix, funcReference, "Skipping TopUp, ID=%s |volume=%d" \
                    % (str(id), volumeToAdd))   # 2011-11-25 sp -- added logging

    def GetVolumeToAddInTopUp(destVial, workVolume):
        # Ensure that the top up volume does not exceed certain limits
        topupVolume = min(destVial.getMaxVolume(), workVolume)
        volumeToAdd = topupVolume - destVial.getVolume()
        return volumeToAdd

    def ResuspendVial(self, destVial, tiprackSpecified, tiprack, srcVial = None, workVolume = 0, usingFreeAirDispense = True, 
			id = None, skipTipStrip = False, skipPickupTip = False, **kw):#CR
        '''Resuspend the given vial. This is essentially the same as TopUpVial, 
	except that the vial is assumed to be empty.
	id is a unique identifier (eg. sample ID) for tracking the caller.
	kw is a dictionary of optional parameters & values.'''

        funcReference = __name__ + '.ResuspendVial'  # 2011-11-25 sp -- added logging
        self.svrLog.logDebug('', self.logPrefix, funcReference, 'start resuspend')   # 2011-11-25 sp -- added logging
        CamMgr = RoboTrace.GetRoboCamMgrInstance();
        CamMgr.SetProcStep(6);
	CamMgr.SetCurrentProtocolID(id)
	CamMgr.SetBlockUpdate(True)
        if kw.has_key('seq'):   #CWJ Mod
           seq = kw['seq']      #CWJ Mod
           CamMgr = RoboTrace.GetRoboCamMgrInstance()
 	   CamMgr.SetCurrentProtocolSeq(seq)
        if kw.has_key('initialQuadrant'): #CWJ Mod
           qdr = kw['initialQuadrant']
           CamMgr = RoboTrace.GetRoboCamMgrInstance()
           CamMgr.SetCurrentSec(qdr)

        initialQuadrant = kw['initialQuadrant']
        if kw.has_key( 'initialQuadrant' ):
            self.TopUpVial( destVial, tiprackSpecified, tiprack, srcVial, workVolume, usingFreeAirDispense, \
                            id = id, skipTipStrip = skipTipStrip, skipPickupTip = skipPickupTip, initialQuadrant = kw['initialQuadrant'] )#CR
        else:
            self.TopUpVial( destVial, tiprackSpecified, tiprack, srcVial, workVolume, usingFreeAirDispense, \
                            id = id, skipTipStrip = skipTipStrip, skipPickupTip = skipPickupTip)#CR
        CamMgr = RoboTrace.GetRoboCamMgrInstance()
	CamMgr.SetBlockUpdate(False)


    def MixTrans(self, destVial, srcVial, mixCycles, tipTubeBottomGap, isRelative, mix_volume_uL,
                 trans_volume_uL, usingFreeAirDispense=False, usingBufferTip=False,
                 id = None, seq = 1, initialQuadrant = 1, tiprackSpecified = False, tiprack = 1, **kw):

        funcReference = __name__ + '.MixTrans'  
        self.svrLog.logDebug('', self.logPrefix, funcReference, 'MixTrans')   
        CamMgr = RoboTrace.GetRoboCamMgrInstance();
        CamMgr.SetupForCommand(id,'MixTrans',**kw)
        
        self.Mix(srcVial, tiprackSpecified, tiprack, mixCycles, tipTubeBottomGap, isRelative, mix_volume_uL, id, skipTipStrip = True, seq=seq, initialQuadrant=initialQuadrant)
        self.Transport(destVial, srcVial, trans_volume_uL, usingFreeAirDispense, usingBufferTip, id, tiprackSpecified, tiprack, seq=seq, skipPickupTip = True, initialQuadrant=initialQuadrant)
        self.__m_Platform.StripTip()
    
    def TopUpMixTrans(self, destVial2, destVial, srcVial, workVolume, mixCycles, tipTubeBottomGap, isRelative, mix_volume_uL,
                 trans_volume_uL, usingFreeAirDispense=True, usingBufferTip=False, 
                 id = None, seq = 1, initialQuadrant = 1, tiprackSpecified = False, tiprack = 1, **kw):

        
        funcReference = __name__ + '.TopUpMixTrans'  
        self.svrLog.logDebug('', self.logPrefix, funcReference, 'TopUpMixTrans')   
        CamMgr = RoboTrace.GetRoboCamMgrInstance();
        CamMgr.SetupForCommand(id,'TopUpMixTrans',**kw)

        self.TopUpVial(destVial, tiprackSpecified, tiprack, srcVial, workVolume, True, id, skipTipStrip = True, seq=seq, initialQuadrant=initialQuadrant)
        self.Mix(destVial, tiprackSpecified, tiprack, mixCycles, tipTubeBottomGap, isRelative, mix_volume_uL, id, skipTipStrip = True, skipPickupTip = True, seq=seq, initialQuadrant=initialQuadrant)
        self.Transport(destVial2, destVial, trans_volume_uL, usingFreeAirDispense, usingBufferTip, id, tiprackSpecified, tiprack, seq=seq, skipPickupTip = True, initialQuadrant=initialQuadrant)
        self.__m_Platform.StripTip()

        
    def ResusMixSepTrans(self, destVial2, destVial, srcVial, workVolume, mixCycles, tipTubeBottomGap, isRelative, mix_volume_uL,
                 trans_volume_uL, usingFreeAirDispense=True, usingBufferTip=False, duration=0,
                 id = None, seq = 1, initialQuadrant = 1, tiprackSpecified = False, tiprack = 1, **kw):

        
        funcReference = __name__ + '.ResusMixSepTrans'  
        self.svrLog.logDebug('', self.logPrefix, funcReference, 'ResusMixSepTrans')   
        CamMgr = RoboTrace.GetRoboCamMgrInstance();
        CamMgr.SetupForCommand(id,'ResusMixSepTrans',**kw)
	
        self.ResuspendVial(destVial, tiprackSpecified, tiprack, srcVial, workVolume, True,id, skipTipStrip = True, seq=seq, initialQuadrant=initialQuadrant)
        self.Mix(destVial, tiprackSpecified, tiprack, mixCycles, tipTubeBottomGap, isRelative, mix_volume_uL, id, skipTipStrip = True, skipPickupTip = True, seq=seq, initialQuadrant=initialQuadrant)
        #self.Separate(duration, id=id, seq=seq) #function is a no-op, have to sleep myself          
        print "ResusMixSepTrans sleep",duration
        time.sleep(duration);
        
        self.Transport(destVial2, destVial, trans_volume_uL, usingFreeAirDispense, usingBufferTip, id, tiprackSpecified, tiprack, seq=seq, skipPickupTip = True, initialQuadrant=initialQuadrant)
        self.__m_Platform.StripTip()
    
    
    def ResusMix(self, destVial, srcVial, workVolume, mixCycles, tipTubeBottomGap, isRelative, mix_volume_uL,
                 id = None, seq = 1, initialQuadrant = 1, tiprackSpecified = False, tiprack = 1, **kw):

        
        funcReference = __name__ + '.ResusMix'  
        self.svrLog.logDebug('', self.logPrefix, funcReference, 'ResusMix')   
        CamMgr = RoboTrace.GetRoboCamMgrInstance();
        CamMgr.SetupForCommand(id,'ResusMix',**kw)
	
        self.ResuspendVial(destVial, tiprackSpecified, tiprack, srcVial, workVolume, True,id, skipTipStrip = True, seq=seq,
                           initialQuadrant=initialQuadrant)
        self.Mix(destVial, tiprackSpecified, tiprack, mixCycles, tipTubeBottomGap, isRelative, mix_volume_uL, id,
                 skipPickupTip = True, seq=seq, initialQuadrant=initialQuadrant)
        self.__m_Platform.StripTip()   


    def TopUpTrans(self, destVial2, destVial, srcVial, workVolume, 
                 trans_volume_uL, usingFreeAirDispense=True, usingBufferTip=False,
                 id = None, seq = 1, initialQuadrant = 1, tiprackSpecified = False, tiprack = 1, **kw):

        
        funcReference = __name__ + '.TopUpTrans'  
        self.svrLog.logDebug('', self.logPrefix, funcReference, 'TopUpTrans')   
        CamMgr = RoboTrace.GetRoboCamMgrInstance();
        CamMgr.SetupForCommand(id,'TopUpTrans',**kw)
	
        self.TopUpVial(destVial, tiprackSpecified, tiprack, srcVial, workVolume, True, id, skipTipStrip = True, seq=seq, initialQuadrant=initialQuadrant)
        self.Transport(destVial2, destVial, trans_volume_uL, usingFreeAirDispense, usingBufferTip, id, tiprackSpecified, tiprack, seq=seq, skipPickupTip = True, initialQuadrant=initialQuadrant)
        self.__m_Platform.StripTip()

    def TopUpTransSepTrans(self, destVial3, destVial2, destVial, srcVial, workVolume, 
                 trans_volume_uL, trans_volume_uL2, usingFreeAirDispense=True, usingBufferTip=False, duration=0,
                 id = None, seq = 1, initialQuadrant = 1, tiprackSpecified = False, tiprack = 1, **kw):

        
        funcReference = __name__ + '.TopUpTransSepTrans'  
        self.svrLog.logDebug('', self.logPrefix, funcReference, 'TopUpTransSepTrans')   
        CamMgr = RoboTrace.GetRoboCamMgrInstance();
        CamMgr.SetupForCommand(id,'TopUpTransSepTrans',**kw)
	
        self.TopUpVial(destVial, tiprackSpecified, tiprack, srcVial, workVolume, True, id, skipTipStrip = True, seq=seq, initialQuadrant=initialQuadrant)
        self.Transport(destVial2, destVial, trans_volume_uL, usingFreeAirDispense, usingBufferTip, id, tiprackSpecified, tiprack, seq=seq, skipTipStrip = True, skipPickupTip = True, initialQuadrant=initialQuadrant)
        #self.Separate(duration, id=id, seq=seq) #function is a no-op, have to sleep myself          
        print "TopUpTransSepTrans sleep",duration
        time.sleep(duration);
        self.Transport(destVial3, destVial2, trans_volume_uL2, usingFreeAirDispense, usingBufferTip, id, tiprackSpecified, tiprack, seq=seq, skipPickupTip = True, initialQuadrant=initialQuadrant)
        self.__m_Platform.StripTip()

    def TopUpMixTransSepTrans(self, destVial3, destVial2, destVial, srcVial, workVolume, 
                 trans_volume_uL, trans_volume_uL2, mixCycles, tipTubeBottomGap, isRelative, mix_volume_uL,
                usingFreeAirDispense=True, usingBufferTip=False, duration=0,
                 id = None, seq = 1, initialQuadrant = 1, tiprackSpecified = False, tiprack = 1, **kw):

        
        funcReference = __name__ + '.TopUpMixTransSepTrans'  
        self.svrLog.logDebug('', self.logPrefix, funcReference, 'TopUpMixTransSepTrans')   
        CamMgr = RoboTrace.GetRoboCamMgrInstance();
        CamMgr.SetupForCommand(id,'TopUpMixTransSepTrans',**kw)
	
        self.TopUpVial(destVial, tiprackSpecified, tiprack, srcVial, workVolume, True, id, skipTipStrip = True, seq=seq, initialQuadrant=initialQuadrant)
        self.Mix(destVial, tiprackSpecified, tiprack, mixCycles, tipTubeBottomGap, isRelative, mix_volume_uL, id, skipTipStrip = True, skipPickupTip = True, seq=seq, initialQuadrant=initialQuadrant)
        self.Transport(destVial2, destVial, trans_volume_uL, usingFreeAirDispense, usingBufferTip, id, tiprackSpecified, tiprack, seq=seq, skipTipStrip = True, skipPickupTip = True, initialQuadrant=initialQuadrant)
        #self.Separate(duration, id=id, seq=seq) #function is a no-op, have to sleep myself          
        print "TopUpMixTransSepTrans sleep",duration
        time.sleep(duration);
        self.Transport(destVial3, destVial2, trans_volume_uL2, usingFreeAirDispense, usingBufferTip, id, tiprackSpecified, tiprack, seq=seq, skipPickupTip = True, initialQuadrant=initialQuadrant)
        self.__m_Platform.StripTip()

        
    
    def Flush(self, wasteVial, homeAtEnd = True, id = None, **kw):
        '''Flush out the hydraulic line to the given waste vial.
	If homeAtEnd is true, the Z axis is homed at the end of the flush.
	id is a unique identifier (eg. sample ID) for tracking the caller.
	kw is a dictionary of optional parameters & values.'''

        funcReference = __name__ + '.Flush'  # 2011-11-25 sp -- added logging
        self.svrLog.logInfo('', self.logPrefix, funcReference, 'start flush')   # 2011-11-25 sp -- added logging

        # Nov 7, 2013 Sunny
        # Update seq for maintenance protocol       
        seq = -3
        if kw.has_key('seq'):
            seq = - kw['seq']
        
        CamMgr = RoboTrace.GetRoboCamMgrInstance();
        CamMgr.SetProcStep(7);
        CamMgr.SetCurrentProtocolID(-1)
        CamMgr.SetCurrentProtocolSeq(seq)
        CamMgr.SetCurrentSec(-1)

        # Strip the current tip


        self.__m_Platform.StripTip()
        
        # Move to the top of the given waste bottle (use a free air dispense)
        # while we do the aspirate

        future = Future( self.__m_Pump.FlushAspirate )
        self.MoveToDispense(wasteVial, 0, True)
        future()

        # Commence flush and then optionally home the Z axis

        volumeDispensed = self.__m_Pump.FlushDispense()
        wasteVial.addVolume(volumeDispensed)
        print '\n### wasteVial: %d Flushed! ###\n'%volumeDispensed
        # Aspirate airslug while raising robot

        
        future = Future( self.__m_Pump.FlushAspirateAirSlug )
        self.__m_Platform.HomeZAxis()
        future()
        

    def Prime(self, wasteVial, homeAtEnd = True, id = None, **kw):
        '''Prime the hydraulic line, using the given waste vial.
	         If homeAtEnd is true, the Z axis is homed at the end of the prime.
	         id is a unique identifier (eg. sample ID) for tracking the caller.
	         kw is a dictionary of optional parameters & values.'''
        # Strip the current tip

        funcReference = __name__ + '.Prime'  # 2011-11-25 sp -- added logging
        self.svrLog.logInfo('', self.logPrefix, funcReference, 'start prime')   # 2011-11-25 sp -- added logging

        # Nov 7, 2013 Sunny
        # Update seq for maintenance protocol
        seq = -2
        if kw.has_key('seq'):
            seq = - kw['seq']
            
        CamMgr = RoboTrace.GetRoboCamMgrInstance();
        CamMgr.SetProcStep(8);
        CamMgr.SetCurrentProtocolID(-1)
        CamMgr.SetCurrentProtocolSeq(seq)
        CamMgr.SetCurrentSec(-1)

        self.__m_Platform.StripTip()
        
        # Move to the top of the given waste bottle (use a free air dispense)
        
        self.MoveToDispense(wasteVial, 0, True)
        
        # Commence prime and then optionally home the Z axis
        
        volumeDispensed = self.__m_Pump.Prime()
        wasteVial.addVolume(volumeDispensed)

        if homeAtEnd:
            self.__m_Platform.HomeZAxis()

        
    def Demo(self, iterations = 1, id = None, **kw):
	'''Run a demo mode for the instrument. You can specify a number of
	iterations.'''

  
	volume_uL = 0
	for i in range(iterations):
	    self.__m_Platform.Initialise()
            for sector in self.__sectorRange:
		vial1 = self.ContainerAt(sector, Instrument.SampleLabel)
		vial2 = self.ContainerAt(sector, Instrument.WasteLabel)
		for tipNum in (1, 4):
		    self.__m_Platform.PickupTip(sector, tipNum)
		    self.MoveToAspirate(vial1, volume_uL)
		    self.MoveToDispense(vial2, volume_uL)
		    self.__m_Platform.StripTip()
                   			
		    
    def PumpLife(self, iterations = 1, id = None, **kw):
        volume_uL = 0
	for i in range(iterations):
	    self.__m_Platform.Initialise()
	    self.__m_Pump.setHydroFluidFull()
	    wastevial = self.ContainerAt(1, Instrument.WasteLabel)
	    bccmvial = self.ContainerAt(0, Instrument.BCCMLabel)
	    bccmvial.setVolume(75000)
	    self.Prime(wastevial,homeAtEnd = False)
            for sector in self.__sectorRange:
		vial1 = self.ContainerAt(sector, Instrument.SampleLabel)
		vial2 = self.ContainerAt(sector, Instrument.WasteLabel)
		vial3 = self.ContainerAt(sector, Instrument.SupernatentLabel)
		vial4 = self.ContainerAt(sector, Instrument.LysisLabel)
		for tipNum in (1, 4):
		    self.__m_Platform.PickupTip(sector, tipNum)
		    self.MoveToAspirate(vial1, volume_uL)
		    self.MoveToDispense(vial2, volume_uL)
		    self.__m_Platform.StripTip()
		self.Transport(vial1,bccmvial,10000)
		self.Mix(vial1,False,1,1,0,0,0)
		self.Transport(vial3,vial1,7000)
		self.Mix(vial3,False,1,1,0,0,0)
		self.Transport(vial4,vial1,3000)
		self.Mix(vial4,False,1,1,0,0,0)
		self.Transport(bccmvial,vial3,7000)
		self.Transport(bccmvial,vial4,3000)
            
    def Demo2(self, iterations = 1, id = None, **kw):
        volume_uL = 0
	for i in range(iterations):
	    self.__m_Platform.Initialise()
	    self.__m_Pump.setHydroFluidFull()
	    wastevial = self.ContainerAt(1, Instrument.WasteLabel)
	    bccmvial = self.ContainerAt(0, Instrument.BCCMLabel)
	    bccmvial.setVolume(75000)
	    self.Prime(wastevial,homeAtEnd = False)
            for sector in self.__sectorRange:
		vial1 = self.ContainerAt(sector, Instrument.SampleLabel)
		vial2 = self.ContainerAt(sector, Instrument.WasteLabel)
		vial3 = self.ContainerAt(sector, Instrument.SupernatentLabel)
		vial4 = self.ContainerAt(sector, Instrument.LysisLabel)
		for tipNum in (1, 4):
		    self.__m_Platform.PickupTip(sector, tipNum)
		    self.MoveToAspirate(vial3, volume_uL)
		    self.MoveToDispense(vial2, volume_uL)
		    self.__m_Platform.StripTip()
		vial1.setVolume(10000)
		vial4.setVolume(20000)
		self.Transport(vial4,vial1,10000)
		self.Transport(vial1,vial4,10000)
            self.Transport(bccmvial,wastevial,18000)

    def Demo3(self, iterations = 1, id = None, **kw):
	'''Run a demo mode for the instrument. You can specify a number of
	iterations.'''
	volume_uL = 0
	for i in range(iterations):
	    self.__m_Platform.Initialise()
            for sector in self.__sectorRange:
		vial1 = self.ContainerAt(sector, Instrument.SampleLabel)
		vial2 = self.ContainerAt(sector, Instrument.WasteLabel)
		for tipNum in (1, 4, 5):
		    self.__m_Platform.PickupTip(sector, tipNum)
		    self.MoveToAspirate(vial1, volume_uL)
		    self.__m_Platform.StripTip()
    def Demo4(self, iterations = 1, id = None, **kw):
        volume_uL = 0
	for i in range(iterations):
	    self.__m_Platform.Initialise()
	    self.__m_Pump.setHydroFluidFull()
	    wastevial = self.ContainerAt(1, Instrument.WasteLabel)
	    bccmvial = self.ContainerAt(0, Instrument.BCCMLabel)
	    bccmvial.setVolume(75000)
	    #self.Prime(wastevial,homeAtEnd = False)
            for sector in self.__sectorRange:
		vial1 = self.ContainerAt(sector, Instrument.SampleLabel)
		vial2 = self.ContainerAt(sector, Instrument.WasteLabel)
		vial3 = self.ContainerAt(sector, Instrument.SupernatentLabel)
		vial4 = self.ContainerAt(sector, Instrument.LysisLabel)
		vial1.setVolume(400)
	
		self.Mix(vial1,False,1,1,0,0,0)
		vial1.setVolume(3000)
		self.Mix(vial1,False,1,1,0,0,0)
		vial1.setVolume(4000)
		self.Mix(vial1,False,1,1,0,0,0)
		vial1.setVolume(5000)
		self.Mix(vial1,False,1,1,0,0,0)
		vial1.setVolume(6000)
		self.Mix(vial1,False,1,1,0,0,0)
		vial1.setVolume(7000)
		self.Mix(vial1,False,1,1,0,0,0)

            #self.Transport(bccmvial,wastevial,18000)
    def Park(self, id = None, **kw):
        '''Moves the robot to a safe place for powerdown'''
        self.__m_Platform.robot.SetTheta( self.__m_thetaParkPosition )
        self.__m_Platform.robot.SetZPosition( self.__m_zParkPosition )


    # --- Other instrument-level actions that are not level 1 workflows -------
    # --- lysis is not currently aspirated....bdr
    def _Aspirate (self, volume, vial, tipCapacity):
        """Aspirate the given volume from the given vial using the given tip"""

        funcReference = __name__ + '._Aspirate'  # 2011-11-25 sp -- added logging
        
        msg = "Aspirate:  Moving to Aspirate vial = %s, volume = %f" % ((vial.getLabel()), volume)
        print msg
        
        self.MoveToAspirate(vial, volume)
        # 2012-01-30 sp -- replace environment variable with configuration variable
        #if os.environ.has_key('SS_ASPIRATEDOWN'):
        if tesla.config.SS_ASPIRATEDOWN == 1:
            raw_input('\n####MoveToAspirate done!\n\a')
        
        volumeAspirated = self.__m_Pump.Aspirate(volume, tipCapacity)
        
        msg = "Aspirate:  Volume actually aspirated = %f" % (volumeAspirated)
        print msg
        self.svrLog.logInfo('', self.logPrefix, funcReference, 'start Aspirate, vial=%s |volume=%d' % (vial, volumeAspirated))   # 2011-11-25 sp -- added logging

        # 2012-01-30 sp -- replace environment variable with configuration variable
        #if os.environ.has_key('SS_ASPIRATEDOWN'):
        if tesla.config.SS_ASPIRATEDOWN == 1:
            raw_input('\n####Pump done!\n\a')

        vial.removeVolume(volumeAspirated)
        if vial.getLabel() in ( self.AntibodyLabel, self.CocktailLabel, self.BeadLabel ):
            self.__WickingExtract(self.__aspirateReagentWickingExtractOffset)
        elif vial.getLabel() in (self.BCCMLabel,) :
            # RL - move arm slowly for buffer bottle - 03/27/06
            print "BCCM wicking extract dist", self.__aspirateBCCMWickingExtractOffset
            self.__WickingExtract(self.__aspirateBCCMWickingExtractOffset)
        else:
            self.__WickingExtract(self.__aspirateWickingExtractOffset)

        return volumeAspirated


    def _Dispense (self, volume, vial, usingFreeAirDispense = False):
        '''Dispense the given volume to the given vial using the given tip.
        NB: The given volume is assumed to be the volume aspirated previously.
	Uses a free air dispense if usingFreeAirDispense = True.'''

        funcReference = __name__ + '._Dispense'  # 2011-11-25 sp -- added logging
        self.svrLog.logInfo('', self.logPrefix, funcReference, 'start Dispense, vial=%s |volume=%d' % (vial, volume))   # 2011-11-25 sp -- added logging
        
        self.MoveToDispense(vial, volume, usingFreeAirDispense)
    
        # 2012-01-30 sp -- replace environment variable with configuration variable
        #if os.environ.has_key('SS_DISPENSEDOWN'):
        if tesla.config.SS_DISPENSEDOWN == 1:
            raw_input('\n####MoveToDispse done!\n\a')
        
        self.__m_Pump.Dispense()   # Pump will dispense whatever was aspirated
        
        # 2012-01-30 sp -- replace environment variable with configuration variable
        #if os.environ.has_key('SS_DISPENSEDOWN'):
        if tesla.config.SS_DISPENSEDOWN == 1:
            raw_input('\n####Pump done!\n\a')

        vial.addVolume(volume)

        # There's no point doing a wicking extract if the tip isn't in the fluid!!
        if not usingFreeAirDispense:   
            self.__WickingExtract(self.__dispenseWickingExtractOffset)

        
        return volume


    def MoveToAspirate(self, vial, volume_ul):
        """Move to aspirate a specific volume from the given vial."""
        if self.__TipIsForbiddenToEnter (vial):
            raise InstrumentError("Current tip (%d) may not aspirate from %s" % \
		    (self.__m_Platform.CurrentTipID()[1], vial.getLabel()))
        
        # Move to the vial and aspirate
        (sector, location) = self.__LocationOf(vial)
        height = self.__CalculateAspirationHeightFor(vial, volume_ul, self.__m_Platform.CurrentTipLength())
        msg = "Aspirate:  Moving to Aspirate vial = %s, volume = %f height =%d" % ((vial.getLabel()), volume_ul,height)
        
        #msg = "Aspirate:  Moving to Aspirate vial = %s, volume = %f" % ((vial.getLabel()), volume)
        #print "------------------------------------ROY2"
        print msg
        
        self.__m_Platform.MoveTo(sector, location, height)
        CamMgr = RoboTrace.GetRoboCamMgrInstance();		#CWJ Add
	CamMgr.SetCurrentSec(sector)				#CWJ Add
	

    def MoveToDispense(self, vial, volume_uL, usingFreeAirDispense = False):
        """Move to dispense a volume (in uL) into the given vial. The dispense can be
	free air if usingFreeAirDispense is True.
	"""
        # Move to the vial and dispense
        (sector, location) = self.__LocationOf(vial)
        
        height = self.__CalculateDispenseHeightFor(vial, volume_uL, \
            self.__m_Platform.CurrentTipLength(), usingFreeAirDispense)
        #self.__logger.logDebug( "@@@@@@@@@@@@@@@@@@@@@@@@@@@ MoveTo %f " % height)

        self.__m_Platform.MoveTo(sector, location, height)     #CWJ Add
        CamMgr = RoboTrace.GetRoboCamMgrInstance();	       #CWJ Add	
	CamMgr.SetCurrentSec(sector)		               #CWJ Add

    # --- Other support methods -------------------------------------------------------

    
    # 2012-01-31 sp -- added barcode reader 
    def MoveToReadBarCode(self, sector, vialNumber):
        # Move to the vial and aspirate
        funcReference = __name__ + '.MoveToReadBarCode'
        barcode = None
        if( vialNumber < 4 ):
            location = Instrument.__reagentPositionMap[ self.ReagentLabelSet[ vialNumber - 1 ] ]
            self.svrLog.logDebug('', self.logPrefix, funcReference, 'move to read barcode at, sector=%d |vialNumber=%d |location=%s' % (sector, vialNumber, location))   # 2011-11-25 sp -- added logging
            self.__m_Platform.MoveToBarCodePosition(sector, location, self.barCodeThetaOffset)
            barcode = self.barcodeReader.getBarCode()
            count       = 1
            rescan      = 0
            newPosition = self.barCodeThetaOffset
            maxRescan   = self.maxRescan; #CJ MOD 10/31/2013
            
            print '#maxRescan %d\n'%maxRescan
            print '#maxCount  %d\n'%self.maxCount
            
            while( ((barcode == 'NR') or (barcode == None))
                                and count < self.maxCount ):  #CJ MOD 10/31/2013
                count = count + 1
                if( count == self.maxCount < (maxRescan - 1) ):  # move back and rescan #CJ MOD 10/31/2013
                    count = 1
                    rescan = rescan + 1
                    newPosition = self.barCodeThetaOffset
                    
                newPosition = newPosition + self.barCodeRescanOffset
                self.__m_Platform.MoveToBarCodePosition(sector, location, newPosition )
                barcode = self.barcodeReader.getBarCode()
            self.svrLog.logDebug('', self.logPrefix, funcReference, 'number of times to scan barcode, rescan=%d |attempts=%d: ' % (rescan, count) )

#            if( barcode == None ):
#                self.svrLog.logDebug('', self.logPrefix, funcReference, 'unable to detect, shift position by 1 degrre')
#                self.__m_Platform.MoveToBarCodePosition(sector, location, self.barCodeThetaOffset + self.barCodeRescanOffset )
#                barcode = self.barcodeReader.getBarCode()
#                if( barcode == None ):
#                    self.svrLog.logDebug('', self.logPrefix, funcReference, 'unable to detect, shift position by -1 degrre')
#                    self.__m_Platform.MoveToBarCodePosition(sector, location, self.barCodeThetaOffset - self.barCodeRescanOffset )
#                    barcode = self.barcodeReader.getBarCode()

        return( barcode )

    def MoveToReadBarCodePosition(self, sector, vialNumber):
        # Move to the vial and aspirate
        funcReference = __name__ + '.MoveToReadBarCode'
        barcode = None
        
        if( vialNumber < 4 ):
            location = Instrument.__reagentPositionMap[ self.ReagentLabelSet[ vialNumber - 1 ] ]
            self.svrLog.logDebug('', self.logPrefix, funcReference, 'move to read barcode at, sector=%d |vialNumber=%d |location=%s' % (sector, vialNumber, location))   # 2011-11-25 sp -- added logging
            self.__m_Platform.MoveToBarCodePosition(sector, location, self.barCodeThetaOffset)
            count       = 1
            rescan      = 0
            newPosition = self.barCodeThetaOffset
            maxRescan   = self.maxRescan; #CJ MOD 10/31/2013
            
            print '#maxRescan %d\n'%maxRescan
            print '#maxCount  %d\n'%self.maxCount
            
            self.svrLog.logDebug('', self.logPrefix, funcReference, 'number of times to scan barcode, rescan=%d |attempts=%d: ' % (rescan, count) )

#            if( barcode == None ):
#                self.svrLog.logDebug('', self.logPrefix, funcReference, 'unable to detect, shift position by 1 degrre')
#                self.__m_Platform.MoveToBarCodePosition(sector, location, self.barCodeThetaOffset + self.barCodeRescanOffset )
#                barcode = self.barcodeReader.getBarCode()
#                if( barcode == None ):
#                    self.svrLog.logDebug('', self.logPrefix, funcReference, 'unable to detect, shift position by -1 degrre')
#                    self.__m_Platform.MoveToBarCodePosition(sector, location, self.barCodeThetaOffset - self.barCodeRescanOffset )
#                    barcode = self.barcodeReader.getBarCode()

        return( barcode )

    # 2013-09-20 sunny -- added
    #     Quadrant:  |   4   |    3   |   2    |   1   |
    #     Vial:      00000WWW|00000XXX|00000YYY|00000ZZZ    eg. WWW, left to right => Quadrant 4, Vial 3, 2, 1
    #
    #     eg. Quadrant 2, Vial 3 fails, the scan return code is   00000000|00000000|00000100|00000000  => 0000 0400 hex
    #     eg. Quadrant 2, Vial 1&3 fail, the scan return code is  00000000|00000000|00000101|00000000  => 0000 0500 hex
    #
    def ScanReturnCode(self, quadrant, vial):
        funcReference = __name__ + '.ScanReturnCode'  # added logging
        shift=0
        temp=0
        if (quadrant == 1):
            shift=0
        elif (quadrant == 2):
            shift=8
        elif (quadrant == 3):
            shift=16
        elif (quadrant == 4):
            shift=24
        else:
            shift=0
        if (vial == 1):
            temp=1
        elif (vial == 2):
            temp=2
        elif (vial == 3):
            temp=4
        ret = temp<<shift
        return ret
    
# 2012-01-03 sp -- added initialization from configuration file
#    def getIniConfigurations(self, configFile):
#        ''' use defaults unless it can retrieve settings from configuration file '''
#        funcReference = __name__ + '.getLogConfigurations'
#
#        cfg = ConfigParser()
#        # if configuration file exists, extract the settings
#        if( os.path.exists( configFile)):
#            cfg.read( configFile )
#            try:
#                # get settings from configuration file
#                self.barcodePort            = cfg.get( 'barcode_reader', 'port' ) 
#                self.barcodeBaudrate        = int( cfg.get( 'barcode_reader', 'baudrate' ) )
#                self.barcodePollDuration    = int( cfg.get( 'barcode_reader', 'pollDuration' ) )
#                self.barcodeNumRetries      = int( cfg.get( 'barcode_reader', 'numRetries' ) )
#                self.barcodeUseEmulation    = int( cfg.get( 'barcode_reader', 'useEmulation' ) )
#                # generate log message of success, report file and version number
#                self.svrLog.logVerbose('', self.logPrefix, funcReference, \
#                    'Using log configuration for barcode reader: port=%s | baudrate=%d | pollDuration=%d| numRetries=%d' % (self.barcodePort, self.barcodeBaudrate, self.barcodePollDuration, self.barcodeNumRetries) )
#            except Exception, msg:
#                self.svrLog.logWarning('', self.logPrefix, funcReference, \
#                    'Error reading from configuration file [%s]...Default settings used: %s' % (configFile, msg) )
#        else:
#            self.svrLog.logWarning('', self.logPrefix, funcReference, \
#                    'Error reading from configuration file [%s]...Default settings used: %s' % (configFile, msg) )
#


    def OpenBarcodePort( self ):
        funcReference = __name__ + '.OpenBarcode'  # 2012-03-30 sp -- added logging
        
        barcodeSettings = gHardwareData.Section('Barcode_reader')
        comPort = barcodeSettings['port']
        baudrate = int( barcodeSettings['baudrate'] )
        pollDuration = int( barcodeSettings['poll_duration'] )
        numRetries = int( barcodeSettings['num_retries'] )
        useEmulation = int( barcodeSettings['use_emulation'] )
        #barcodeCalibration = gHardwareData.Section('Calibration')
        devicename = barcodeSettings['devicename'];
        self.maxRescan = int(barcodeSettings['max_rescan'])
        self.maxCount  = int(barcodeSettings['max_count'])
                       
        commandSettings  = gHardwareData.Section(devicename)
        controlchar      = commandSettings['control_char_in_hex'].strip();
        if (controlchar.__len__()!= 0):
           consep           = chr(int(controlchar[2:],16));
           # print "$$$$$$ consep is %s" %consep #YW
           triggerCmd  = commandSettings['trigger_cmd'].replace(controlchar,consep);
           # print "$$$$$$ triggerCmd after replacement is %s" %triggerCmd #YW
           updateCmd   = commandSettings['update_cmd'].replace(controlchar,consep);
           initCmd     = commandSettings['init_cmd'].replace(controlchar,consep);
           initCmd2    = commandSettings['init_cmd2'].replace(controlchar,consep);           
           initCmd3    = commandSettings['init_cmd3'].replace(controlchar,consep);                      
           initCmd4    = commandSettings['init_cmd4'].replace(controlchar,consep);                                 
           initCmd5    = commandSettings['init_cmd5'].replace(controlchar,consep);                                            
           if (gHardwareData.ItemExists(devicename, 'pre_cmd')):
               preCmd = commandSettings['pre_cmd'].replace(controlchar, consep);
           else:
               preCmd = ""                                            
        else:           
           triggerCmd  = commandSettings['trigger_cmd'];
           # print "$$$$$$ triggerCmd without replacement is %s" %triggerCmd #YW 
           updateCmd   = commandSettings['update_cmd'];
           initCmd     = commandSettings['init_cmd'];
           initCmd2    = commandSettings['init_cmd2'];           
           initCmd3    = commandSettings['init_cmd3'];                      
           initCmd4    = commandSettings['init_cmd4'];                                 
           initCmd5    = commandSettings['init_cmd5'];                                            
           if (gHardwareData.ItemExists(devicename, 'pre_cmd')):
               preCmd = commandSettings['pre_cmd'].replace(controlchar, consep);
           else:
               preCmd = ""                                              
        
        initCmds = [ initCmd, initCmd2, initCmd3, initCmd4, initCmd5 ];
        
        self.barCodeThetaOffset = float( barcodeSettings['barcode_offset_degrees'] )
        self.barCodeRescanOffset = float( barcodeSettings['barcode_rescan_offset_degrees'] )
        self.barcodeReader = BarCodeReader(comPort, baudrate, pollDuration, numRetries, triggerCmd, updateCmd, initCmds, preCmd, useEmulation);

        self.barcodes = [ ["-", "-", "-"], ["-", "-", "-"], ["-", "-", "-"], ["-", "-", "-"] ]
        self.barcodesStatus = [ ["-", "-", "-"], ["-", "-", "-"], ["-", "-", "-"], ["-", "-", "-"] ]
        if( useEmulation == 1 ):
            self.svrLog.logInfo('', self.logPrefix, funcReference, 'Barcode reader initialized, using emulation')
            rtn = 1
        else:
            if( self.barcodeReader.comPortActive ):
                self.svrLog.logInfo('', self.logPrefix, funcReference, 'Barcode reader initialized')
                rtn = 1
            else:
                self.svrLog.logError('', self.logPrefix, funcReference, 'Barcode reader failed to initialized')
                rtn = -1
                
    def CloseBarcodePort(self):
        self.barcodeReader.close();
                        
    def AttachBarcode( self ):
        self.barcodeReader.AttachBarcodeReader();
        
    def DetachBarcode( self ):
        self.barcodeReader.DetachBarcodeReader();
            
    # 2012-03-30 sp -- added barcode reader
    def InitScanVialBarcode( self, freeaxis ):     
        print ('>> Enter InitScanVialBarcode[%s]\n' % freeaxis);
        if (freeaxis == True):
            self.CleanUp();
            self.__m_Platform.powerDownCarousel();

        self.bBarcodeInit = True
        rtn = 1;
        return rtn
    
    def AbortScanVialBarcode( self ):
        print('>> Aborting')
        self.abortScanning = True
    
    def IsBarcodeScanningDone( self ):
        print ('>> IsBarcodeScanningDone')
        if (self.barcodeScanningThread == None or self.barcodeScanningThread.isAlive()):
            return False;
        else:
            return True;
    
    def __barcodeScanningThread( self, q, Quadrant, Vial):
        funcReference = __name__ + '.ScanVialBarcodeAt'  # 2011-11-25 sp -- added logging
        temp = 0
        # reset barcodes if the quadrant value is -1

        if (self.bBarcodeInit == True):
            self.__m_Platform.BeaconDriver.TurnONBeacon(3);    #CWJ Add            
            time.sleep(3);
            self.__m_Platform.BeaconDriver.TurnOFFBeacon();    #CWJ Add     
            self.__m_Platform.carousel.Home();
            time.sleep(3);
           
        self.bBarcodeInit = False;
        self.__m_Platform.BeaconDriver.TurnOFFBeacon();      #CWJ Add     
        
        if( Quadrant == -1 ):
            self.barcodes = [ ["-", "-", "-"], ["-", "-", "-"], ["-", "-", "-"], ["-", "-", "-"] ]
            self.barcodesStatus = [ ["-", "-", "-"], ["-", "-", "-"], ["-", "-", "-"], ["-", "-", "-"] ]
            self.svrLog.logInfo('', self.logPrefix, funcReference, 'Quad=%d |Vial=%d | All barcodes reset' % (Quadrant, Vial))   # 2011-11-25 sp -- added logging
        else:
            if( Quadrant == 0 ):
                quadrants = [ 1, 2, 3, 4 ]
            else:
                quadrants = [ Quadrant ]
            if( Vial == 0 ):
                vials = [1, 2, 3]
            else:
                vials = [ Vial ]

            # 2013-10-16 sp
            # added timer for scanning process and changed method of rescanning vials
            #  - in one direction by maximum number of rescan offset steps defined in configuration file
            startTime = time.time()
            for q in quadrants:
                if (self.abortScanning == True):
                    break
                for v in vials:
                    if (self.abortScanning == True):
                        break
                    barcode = self.MoveToReadBarCode( q, v )
                    if ( barcode == None) or ( barcode == 'NR') :
                        if (barcode == None):
                            self.svrLog.logInfo('', self.logPrefix, funcReference, 'Quadrant=%d |Vial=%d | Barcode=ScanTimeout' % (q, v))
                            self.__logger.logInfo('BCR: Quadrant=%d |Vial=%d | Barcode=ScanTimeout' % (q, v))
                            self.barcodesStatus[ q-1 ][ v-1 ] = 'ScanTimeout'
                        if (barcode == 'NR'):
                            self.svrLog.logInfo('', self.logPrefix, funcReference, 'Quadrant=%d |Vial=%d | Barcode=NR' % (q, v))
                            self.__logger.logInfo('BCR: Quadrant=%d |Vial=%d | Barcode=NR' % (q, v))                               
                            self.barcodesStatus[ q-1 ][ v-1 ] = 'NR'
                                                          
                        barcode = self.barcodeReader.noneDetected_response
                    if( barcode == self.barcodeReader.noneDetected_response or barcode == self.barcodeReader.error_response):
                        temp = self.ScanReturnCode( q, v)
                        
                    self.barcodes[ q-1 ][ v-1 ] = barcode
            
            if (self.abortScanning == False):
                endTime = time.time()
                self.svrLog.logDebug('', self.logPrefix, funcReference, 'Q parameter=%d |V parameter=%d | Time taken=%s' % (Quadrant, Vial, (endTime - startTime)))   # 2011-11-25 sp -- added logging
            else:
                self.svrLog.logDebug('', self.logPrefix, funcReference, "Scanning is aborted" )
                print("Scanning is aborted")
        
    
    # 2012-01-31 sp -- added barcode reader
    # 2013-09-20 sunny -- add the return code that indicates the specific vials that have the error during barcode acquisition
    #                     return code = 0 indicates there are no errors
    # 2014-08-07 twong -- move the code into barcodeScanningThread
    def ScanVialBarcodeAt(self, Quadrant, Vial):
        if (self.barcodeScanningThread == None or not self.barcodeScanningThread.isAlive()):
            self.abortScanning = False        
            q = Queue.Queue()
            self.barcodeScanningThread = threading.Thread(target=self.__barcodeScanningThread, args = (q, Quadrant, Vial))
            self.barcodeScanningThread.start()
            return 0
        return 1
    
    # 2015-Dec-15 CJ -- DebugView ScanVialBarcodeAt no longer available, this function will support DebugView.
    def ScanVialBarcodeAtForCheckup(self, Quadrant, Vial):        
        funcReference = __name__ + '.ScanVialBarcodeAt'  # 2011-11-25 sp -- added logging
        rtn = 0     # default return value on success
        temp = 0
        # reset barcodes if the quadrant value is -1

        if (self.bBarcodeInit == True):
           self.__m_Platform.BeaconDriver.TurnONBeacon(3);    #CWJ Add            
           time.sleep(3);
           self.__m_Platform.BeaconDriver.TurnOFFBeacon();    #CWJ Add     
           self.__m_Platform.carousel.Home();

        self.bBarcodeInit = False;
        self.__m_Platform.BeaconDriver.TurnOFFBeacon();      #CWJ Add     
        
        if( Quadrant == -1 ):
            self.barcodes = [ ["-", "-", "-"], ["-", "-", "-"], ["-", "-", "-"], ["-", "-", "-"] ]
            self.barcodesStatus = [ ["-", "-", "-"], ["-", "-", "-"], ["-", "-", "-"], ["-", "-", "-"] ]
            self.svrLog.logInfo('', self.logPrefix, funcReference, 'Quad=%d |Vial=%d | All barcodes reset' % (Quadrant, Vial))   # 2011-11-25 sp -- added logging
        else:
            if( Quadrant == 0 ):
                quadrants = [ 1, 2, 3, 4 ]
            else:
                quadrants = [ Quadrant ]
            if( Vial == 0 ):
                vials = [1, 2, 3]
            else:
                vials = [ Vial ]

            # 2013-10-16 sp
            # added timer for scanning process and changed method of rescanning vials
            #  - in one direction by maximum number of rescan offset steps defined in configuration file
            startTime = time.time()
            for q in quadrants:
                if( Vial == -1 ):
                    self.barcodes[ q-1 ] = ["-", "-", "-"]
                    self.barcodesStatus[ q-1 ] = ["-", "-", "-"]                    
                else:
                    for v in vials:
                        barcode = self.MoveToReadBarCode( q, v )
                        if ( barcode == None) or ( barcode == 'NR') :
                            if (barcode == None):
                               self.svrLog.logInfo('', self.logPrefix, funcReference, 'Quadrant=%d |Vial=%d | Barcode=ScanTimeout' % (q, v))
                               self.__logger.logInfo('BCR: Quadrant=%d |Vial=%d | Barcode=ScanTimeout' % (q, v))
                               self.barcodesStatus[ q-1 ][ v-1 ] = 'ScanTimeout'
                            if (barcode == 'NR'):
                               self.svrLog.logInfo('', self.logPrefix, funcReference, 'Quadrant=%d |Vial=%d | Barcode=NR' % (q, v))
                               self.__logger.logInfo('BCR: Quadrant=%d |Vial=%d | Barcode=NR' % (q, v))                               
                               self.barcodesStatus[ q-1 ][ v-1 ] = 'NR'
                                                              
                            barcode = self.barcodeReader.noneDetected_response
                        if( barcode == self.barcodeReader.noneDetected_response or barcode == self.barcodeReader.error_response):
                            temp = self.ScanReturnCode( q, v)
                            rtn = rtn | temp
                        self.barcodes[ q-1 ][ v-1 ] = barcode

            endTime = time.time()
            self.svrLog.logDebug('', self.logPrefix, funcReference, 'Q parameter=%d |V parameter=%d | Time taken=%s' % (Quadrant, Vial, (endTime - startTime)))   # 2011-11-25 sp -- added logging

        return rtn;

    def GotoVialBarcode(self, Quadrant, Vial):        
        funcReference = __name__ + '.ScanVialBarcodeAt'  # 2011-11-25 sp -- added logging
        rtn = 0     # default return value on success
        temp = 0
        # reset barcodes if the quadrant value is -1

        if (self.bBarcodeInit == True):
           self.__m_Platform.BeaconDriver.TurnONBeacon(3);    #CWJ Add            
           time.sleep(3);
           self.__m_Platform.BeaconDriver.TurnOFFBeacon();    #CWJ Add     
           self.__m_Platform.carousel.Home();
           time.sleep(3);
           
        self.bBarcodeInit = False;
        self.__m_Platform.BeaconDriver.TurnOFFBeacon();      #CWJ Add     
        
        if( Quadrant == -1 ):
            self.barcodes = [ ["-", "-", "-"], ["-", "-", "-"], ["-", "-", "-"], ["-", "-", "-"] ]
            self.barcodesStatus = [ ["-", "-", "-"], ["-", "-", "-"], ["-", "-", "-"], ["-", "-", "-"] ]
            self.svrLog.logInfo('', self.logPrefix, funcReference, 'Quad=%d |Vial=%d | All barcodes reset' % (Quadrant, Vial))   # 2011-11-25 sp -- added logging
        else:
            if( Quadrant == 0 ):
                quadrants = [ 1, 2, 3, 4 ]
            else:
                quadrants = [ Quadrant ]
            if( Vial == 0 ):
                vials = [1, 2, 3]
            else:
                vials = [ Vial ]

            # 2013-10-16 sp
            # added timer for scanning process and changed method of rescanning vials
            #  - in one direction by maximum number of rescan offset steps defined in configuration file
            startTime = time.time()
            for q in quadrants:
                if( Vial == -1 ):
                    self.barcodes[ q-1 ] = ["-", "-", "-"]
                    self.barcodesStatus[ q-1 ] = ["-", "-", "-"]                    
                else:
                    for v in vials:
                        barcode = self.MoveToReadBarCodePosition( q, v )
                        rtn = rtn | temp
                        self.svrLog.logInfo('', self.logPrefix, funcReference, 'Quadrant=%d |Vial=%d ' % (q, v))   # 2011-11-25 sp -- added logging
            endTime = time.time()
            self.svrLog.logDebug('', self.logPrefix, funcReference, 'Q parameter=%d |V parameter=%d | Time taken=%s' % (Quadrant, Vial, (endTime - startTime)))   # 2011-11-25 sp -- added logging

        return rtn;
   
    # 2012-01-31 sp -- added barcode reader
    def GetVialBarcodeAt( self, Quadrant, Vial):      
        rtn = self.barcodes[ Quadrant-1 ][ Vial-1 ]
        return rtn;

    def GetVialBarcodeStatusAt( self, Quadrant, Vial):       
        rtn = self.barcodesStatus[ Quadrant-1 ][ Vial-1 ]
        return rtn;


    def Initialise(self):
        """Initialise the high-level instrument components."""
        self.__m_Platform.Initialise()
        self.__m_Pump.Initialise( False )


    def CleanUp(self):
	'''Cleaning up at the end of a run. This (currently) means that we
	move the carousel a short distance from home so that the carousel tab
	is clear of the opto (for easy removal of the carousel)
	'''
	self.__m_Platform.MoveCarouselToSafePosition()


    def getHardwareSetting(self, settingKey):
        '''Return the value of the specified setting key.'''
        if self._m_Settings.has_key(settingKey):
            return self._m_Settings[settingKey]
        else:
            raise InstrumentError, "IL: Can not find setting = %s" % (settingKey)


    def ClearRootQuadrantMapping(self):
        """Clears the root quadrant to sample ID mapping.
        This should be called prior to running a batch"""
        self.__m_RootQuadrantSampleId = [None,None,None,None]
        

    # --- Container reagent handling ------------------------------------------
    def makeCustomSizeReagentVialGeometry(self, reagentName, FullVolume, FullHeight):

        if FullVolume <= 0 or FullHeight <=0:
            return None

        #Cylindrical_FullHeight = float(self._m_Settings[Instrument.CylindricalFullHeightReagentlLabel])
        #Cylindrical_FullVolume = float(self._m_Settings[Instrument.CylindricalFullVolumeReagentlLabel])
        #Cylindrical_K = float(Cylindrical_FullHeight/Cylindrical_FullVolume)

        #NewCylindrical_FullHeight = float(Cylindrical_K * customSize)
        
        #profile_NewReagentVial = (CylindricalProfile (NewCylindrical_FullHeight, float(customSize)), ) #The conical base is ~1ul, and is currently ignored!!
        profile_NewReagentVial = (CylindricalProfile (float(FullHeight), float(FullVolume)), ) #The conical base is ~1ul, and is currently ignored!!

        zOffset = self.__m_Platform.RobotZOffset()
        
        if reagentName==Instrument.AntibodyLabel:
            AntibodyGeo = TubeGeometry (float(self._m_Settings[Instrument.BaseAntibodyLabel]) + zOffset, float(self._m_Settings[Instrument.DeadVolumeAntibodyLabel]), profile_NewReagentVial)
            return  AntibodyGeo
        elif reagentName==Instrument.CocktailLabel:
            CocktailGeo = TubeGeometry (float(self._m_Settings[Instrument.BaseCocktailLabel]) + zOffset, float(self._m_Settings[Instrument.DeadVolumeCocktailLabel]), profile_NewReagentVial)
            return CocktailGeo
        elif reagentName==Instrument.BeadLabel:
            BeadGeo = TubeGeometry (float(self._m_Settings[Instrument.BaseBeadLabel])     + zOffset, float(self._m_Settings[Instrument.DeadVolumeBeadLabel])    , profile_NewReagentVial)
            return BeadGeo
        else:
            return None
    def InitialiseContainers (self):
        '''Set up the standard containers on the instrument on startup. (They 
	are initially assumed to be empty).
	Returns a map of the tubes, with the map key being the sector number 
	and the map value being a map of tubes for that sector. The sector tube
	map has keys corresponding to tube name and each map value is the 
	corresponding Tube instance.'''

        # Standard Tesla tube volume profiles, based on data in the Tesla IDD 
	# (document 8013). The raw data in IDD is represented as a cumulative 
	# total of the values given here.
        #print "The Full Volume of Hemisphere Volume Profile is %s" % self._m_Settings[Instrument.HemisphericalFullHeight14mlLabel]
        
        profile_14mlVial = (HemisphericalProfile (float(self._m_Settings[Instrument.HemisphericalFullHeight14mlLabel])), CylindricalProfile (float(self._m_Settings[Instrument.CylindricalFullHeight14mlLabel]), float(self._m_Settings[Instrument.CylindricalFullVolume14mlLabel])),)
        profile_50mlVial = (ConicalProfile (float(self._m_Settings[Instrument.ConicalFullHeight50mlLabel]), float(self._m_Settings[Instrument.ConicalFullVolume50mlLabel])), CylindricalProfile (float(self._m_Settings[Instrument.CylindricalFullHeight50mlLabel]), float(self._m_Settings[Instrument.CylindricalFullVolume50mlLabel])),)
        profile_ReagentVial = (CylindricalProfile (float(self._m_Settings[Instrument.CylindricalFullHeightReagentlLabel]), float(self._m_Settings[Instrument.CylindricalFullVolumeReagentlLabel])), ) #The conical base is ~1ul, and is currently ignored!!
#        profile_BeadVial = (ConicalProfile (24, 900), )
        profile_BulkBottle = (DeadVolumeProfile (float(self._m_Settings[Instrument.DeadVolumeProfileBulkLabel])), CylindricalProfile (float(self._m_Settings[Instrument.CylindricalFullHeight1BulkLabel]), float(self._m_Settings[Instrument.CylindricalFullVolume1BulkLabel])), TruncatedConicalProfile (float(self._m_Settings[Instrument.TruncatedConicalFullHeightBulkLabel]), float(self._m_Settings[Instrument.TruncatedConicalFullVolumeBulkLabel]), ConicalProfile (float(self._m_Settings[Instrument.ConicalFullHeightBulkLabel]), float(self._m_Settings[Instrument.ConicalFullVolumeBulkLabel])), False), CylindricalProfile (float(self._m_Settings[Instrument.CylindricalFullHeight2BulkLabel]), float(self._m_Settings[Instrument.CylindricalFullVolume2BulkLabel])),)
        
        # Define the tubes present in each sector
        # Apply Z axis offset to all tube bases
        zOffset = self.__m_Platform.RobotZOffset()
        
        tubeList =  {
                    Instrument.AntibodyLabel        : TubeGeometry (float(self._m_Settings[Instrument.BaseAntibodyLabel]) + zOffset, float(self._m_Settings[Instrument.DeadVolumeAntibodyLabel]), profile_ReagentVial),
                    Instrument.CocktailLabel        : TubeGeometry (float(self._m_Settings[Instrument.BaseCocktailLabel]) + zOffset, float(self._m_Settings[Instrument.DeadVolumeCocktailLabel]), profile_ReagentVial),
                    Instrument.BeadLabel            : TubeGeometry (float(self._m_Settings[Instrument.BaseBeadLabel])     + zOffset, float(self._m_Settings[Instrument.DeadVolumeBeadLabel])    , profile_ReagentVial),
                    Instrument.SampleLabel          : TubeGeometry (float(self._m_Settings[Instrument.Base14mlLabel])     + zOffset, float(self._m_Settings[Instrument.DeadVolume14mlLabel])    , profile_14mlVial),
                    Instrument.SupernatentLabel     : TubeGeometry (float(self._m_Settings[Instrument.Base14mlLabel])     + zOffset, float(self._m_Settings[Instrument.DeadVolume14mlLabel])    , profile_14mlVial),
                    Instrument.LysisLabel           : TubeGeometry (float(self._m_Settings[Instrument.Base50mlLabel])     + zOffset, float(self._m_Settings[Instrument.DeadVolume50mlLabel])    , profile_50mlVial),
                    Instrument.WasteLabel           : TubeGeometry (float(self._m_Settings[Instrument.Base50mlLabel])     + zOffset, float(self._m_Settings[Instrument.DeadVolume50mlLabel])    , profile_50mlVial),
                    }                                                                                              

        # Set up the tubes in each sector
        fullTubeMap = {}
        for sector in self.__sectorRange:
            sectorTubes = {}
            for name in tubeList.keys():
		tube = Tube (sector, name, tubeList[name])
                sectorTubes[name] = tube
            fullTubeMap[sector] = sectorTubes

        # Finally, insert an entry for the bulk bottle
        # Sector 0 is reserved for items accessible to the robot that aren't on
	# the carousel
	bccmTube = Tube (0, Instrument.BCCMLabel, TubeGeometry (float(self._m_Settings[Instrument.BaseBCCMLabel]), 
			    float(self._m_Settings[Instrument.DeadVolumeBCCMLabel]), profile_BulkBottle))
        fullTubeMap[0] = {Instrument.BCCMLabel : bccmTube, }

        self.__m_Containers = fullTubeMap

        wasteVial = self.ContainerAt(1, self.WasteLabel)
        print "@#$@#$@#$@#$@#$@#$ InitialiseContainers",str(wasteVial)
        
        return fullTubeMap


    def getContainers(self):
        '''Return a map of all of the containers. This is mainly for unit
        testing purposes.'''
        return self.__m_Containers


    def setLocalCopyOfState( self, newState ):
        '''Sets a copy of the current state in this object'''
        self.__m_state = newState

    def ContainerAt (self, sector, name):
        '''Return the Container instance at the given position.
        sector : the sector to look up (1-4 on the carousel, 0 is not on the carousel)
        name: mnemonic associated with the container'''
        return self.ReplaceContainerAt(sector, name, None)
        
    def ReplaceContainerAt (self, sector, name, newContainer):

        # Short circuit container testing in service state
        if self.__m_state == 'SERVICE':
            return self.__m_Containers[sector][name]
        
        if not sector in self.__m_Containers.keys():
	    self.Trace(str(self.__m_Containers.keys()))
            raise InstrumentError ("sector %d is not in range (0-%d)" % (sector, self.__m_Platform.NbrSectors()))
        elif not name in self.__m_Containers[sector].keys():
            raise InstrumentError ("%s is not present in sector %d" % (name, sector))

        if not newContainer == None:
            self.__m_Containers[sector][name] = newContainer
        
	return self.__m_Containers[sector][name]



    def BulkBottle (self):
        '''Return the Container instance for the bulk bottle.'''
        return self.ContainerAt (0, Instrument.BCCMLabel)


    def getBulkBufferDeadVolume(self):
        '''Returns the dead volume of the bulk buffer container'''
        return float(self._m_Settings[Instrument.DeadVolumeBCCMLabel])


    def setContainerVolume(self, sector, containerName, volume_uL):
        '''Set the fluid volume (in uL) in a container, specified by sector and
        name, as per ContainerAt(). Doesn't return anything.'''
        container = self.ContainerAt(sector, containerName)
        container.setVolume(volume_uL)
        self.__logger.logDebug("IL: Setting %d:%s volume to %d uL" % \
            (sector, containerName, volume_uL))
        funcReference = __name__ + '.setContainerVolume'  # 2011-11-25 sp -- added logging
        self.svrLog.logInfo('S', self.logPrefix, funcReference, "setting volume, sector=%d |container=%s |volume=%d" % \
            (sector, containerName, volume_uL))   # 2011-11-25 sp -- added logging
	
    def setContainerVolumePercentage(self, sector, containerName, percentageLevel):
	'''Set the fluid volume as a percentage of maximum volume.
	percentageLevel is a value between 0 and 1.'''
	container = self.ContainerAt(sector, containerName)
	volume_uL = container.getMaxVolume() * percentageLevel
	self.setContainerVolume(sector, containerName, volume_uL)

    def getContainerVolume(self, sector, containerName):
	'''Return the volume (in uL) of the container at a location, specified
	by sector and container name.'''
	container = self.ContainerAt(sector, containerName)
	return container.getVolume()

    def emptyReagentContainers(self):
	'''Set the volume of the reagent containers to zero.'''	
        for sector in self.__sectorRange:
	    for containerLabel in (Instrument.AntibodyLabel, Instrument.CocktailLabel,
				    Instrument.BeadLabel, Instrument.LysisLabel,
				    Instrument.SampleLabel, Instrument.SupernatentLabel):
		self.setContainerVolume(sector, containerLabel, 0)

    def emptyWasteContainers(self):
	'''Set the volume of the waste containers to zero.'''
        for sector in self.__sectorRange:
	    self.setContainerVolume(sector, Instrument.WasteLabel, 0)


    # --- Access to the hardware components -----------------------------------
    
    # --- bdr
    def parkArm(self, id = None, **kw): 
        '''Moves the robot to a safe place for powerdown'''
        self.__m_Platform.robot.SetTheta( self.__m_thetaParkPosition )
        self.__m_Platform.robot.SetZPosition( self.__m_zParkPosition )
    	self.__m_Platform.MoveCarouselToSafePosition()
        self.__m_Platform.powerDownCarousel()
        return True

    def parkPump( self ):
        '''Parks pump valve to idle (ie non-energised) position.'''
        self.__m_Pump.ParkPump()
        return True

	# ver338
    def parkCarousel( self ):
        '''Parks carousel in safe position to clear opto.'''
    	self.__m_Platform.MoveCarouselToSafePosition()
        return True

    # --- bdr

    def SetIgnoreLidSensor(self, sw):                                       #CWJ
        self.__m_Platform.lidSensor.SetIgnoreLidSensor(sw);
        
    def GetIgnoreLidSensor(self):                                           #CWJ
        return self.__m_Platform.lidSensor.GetIgnoreLidSensor();
                
    def SetIgnoreHydraulicSensor(self, sw):                                 #CWJ
        self.__m_Platform.HydraulicSensor.SetIgnoreHydraulicSensor(sw);
        
    def GetIgnoreHydraulicSensor(self):                                     #CWJ
        return self.__m_Platform.HydraulicSensor.GetIgnoreHydraulicSensor();

    def isLowHydraulicLevel( self ):
        '''Returns True if the lid is closed, False otherwise'''            #CWJ
        rtn = self.__m_Platform.HydraulicSensor.isLowHydraulicLevel()
        if rtn == True:
            warningMsg = "*** Hydro Fluid is LOW! ***"
            self.__logger.logWarning(warningMsg)
            funcReference = __name__ + '.isLowHydraulicLevel'  # 2011-11-25 sp -- added logging
            self.svrLog.logWarning('', self.logPrefix, funcReference, warningMsg)   # 2011-11-25 sp -- added logging
        return rtn

    def TurnOFFBeacon( self ):                                              #CWJ
        self.__m_Platform.BeaconDriver.TurnOFFBeacon();
        
    def TurnONBeacon(self, evt):                                            #CWJ
        self.__m_Platform.BeaconDriver.TurnONBeacon(evt);            

    def isLidClosed( self ):
        '''Returns True if the lid is closed, False otherwise'''
        return self.__m_Platform.lidSensor.isLidClosed()

    def powerDownCarousel( self ):
        '''Powers down the carousel stepper so it can spin freely'''
        return self.__m_Platform.powerDownCarousel()

    
    def homePlatform( self):
        '''Initialises (homes) platform components'''
        return self.__m_Platform.Initialise()


    def isHydroFluidLow( self ):
        '''Returns true if the hydraulic fluid is low'''
        return self.__m_Pump.isHydroFluidLow()

    def setHydroFluidFull( self ):
        '''Sets the hydraulic fluid to full capacity'''
        self.__m_Pump.setHydroFluidFull()

    
    # RL - pause command - 03/29/06
    def isPauseCommand( self ):
        return self.__m_isPause
    def getPauseCommandCaption( self ):
        return self.__m_pauseCaption
    def resetPauseCommand( self ):
        self.__m_isPause = False
        self.__m_pauseCaption = ""

    # --- Access to the hardware components -----------------------------------

    def getPlatform(self):
	'''Return the Tesla platform instance, primarily for testing purposes.'''
	return self.__m_Platform


    def getPump(self):
	'''Return the DRD pump instance, primarily for testing purposes.'''
	return self.__m_Pump


    # ---------------------------------------------------------------------------
    #   Private methods: 
    # ---------------------------------------------------------------------------

    def _serviceMethods(self):
        '''Return a list of methods for the service interface'''
        return ['CleanUp', 'ContainerAt', 'Flush', 'HomeAll', 'Initialise',
                'Mix', 'MoveToAspirate', 'MoveToDispense', 'Park', 'Prime',
                'ResuspendVial', 'TopUpVial', 'Transport', 'emptyReagentContainers',
                'emptyWasteContainers', 'getContainerVolume', 'setContainerVolume',
                'setContainerVolumePercentage', 'MoveToBarcodeOffset', 'MoveToBarcodeOffsetForVial',
                'MixTrans', 'TopUpMixTrans', 'ResusMixSepTrans','ResusMix',
                'TopUpTrans', 'TopUpTransSepTrans', 'TopUpMixTransSepTrans'
                ]

    def __LocationOf(self, vial):
        """ Determine the sector and location reference for the given vial."""
        location = vial.getLabel()
        
        if location in Instrument.__reagentPositionMap.keys():
            # Override label
            location = Instrument.__reagentPositionMap[location]

        return (vial.getSector(), location)
    
    def _LocationOf(self, vial):          ##2015-Nov-4 CWJ Add to support service program
        """ Determine the sector and location reference for the given vial."""
        (sector,location) = self.__LocationOf(vial)
        return (sector,location);
    
    def __TipToUseFor(self, vial):
        """Determine what tip to use for a given vial."""
        if not vial.getLabel() in Instrument.__transportTipUsageMap.keys():
            raise InstrumentError ("%s vial has no designated tip!" % (vial.getLabel()))
        return (vial.getSector(), Instrument.__transportTipUsageMap[vial.getLabel()])
        
    # IAT - This function was changed due to unexpected tip selection during mix commands. 
    def __TipToMixWith(self, vial):
        '''Determine what tip to use when mixing the fluid currently in the given vial.
	Returns a tuple of sector and tip.'''
        nominalMixVolume = min (vial.getVolume() * self.__mixingFraction, self.__maxMixingVolume)
        
        if nominalMixVolume <= 5000:
            mixTip = int(Instrument.__transportTipUsageMap[vial.getLabel()])
        else: 
            raise InstrumentError ("Mix volume: %d too high." %(nominalMixVolume))
            
        if self.__TipIsForbiddenToEnter(vial, mixTip):
            raise InstrumentError ("Incorrect tip selection.")
        
	return (vial.getSector(), mixTip)
  
    
    def __TipCapacityOf(self, tipId):
        '''Determine the tip capacity (in uL) for the specified tip ID.'''
        if not tipId in self.__tipCapacityMap.keys():
            raise InstrumentError ("Tip %d is undefined!" % (tipId))
            
        return float (self._m_Settings[self.__tipCapacityMap[tipId]])
        
    
    def __UseTip(self, sector, chosenTip):
        """Strip the current tip and pick up the designated one, if it is not already in use"""
        if (sector, chosenTip) != self.__m_Platform.CurrentTipID():
            self.__m_Platform.StripTip()
            self.__m_Platform.PickupTip(sector, chosenTip)
        
    
    def __CalculateAspirationHeightFor(self, vial, volumeToTransport_uL, tipLength):
        """Determine the height at which to place the robot in preparation to aspirate."""
        print "### CWJ -  Label %s, basePos : %d, tipLen :%d"%( vial.getLabel(), vial.getBasePosition(), tipLength) 
        print "bdr - __CalculateAspirationHeightFor basepsn, tiplen, meniscus, asp_depthlbl", vial.getBasePosition(),tipLength , vial.getMeniscusAfterRemoving(volumeToTransport_uL) ,float(self._m_Settings[Instrument.AspirationDepthLabel])
        aspirationHeight = vial.getBasePosition() - tipLength - vial.getMeniscusAfterRemoving(volumeToTransport_uL) + float(self._m_Settings[Instrument.AspirationDepthLabel])
        print "bdr - __CalculateAspirationHeightFor height", aspirationHeight
        return self.__ClipHeight (aspirationHeight, vial, tipLength, True)
        
    
    def __CalculateDispenseHeightFor(self, vial, volumeToTransport_uL, tipLength, \
					usingFreeAirDispense):
        """Determine the height at which to place the robot in preparation to dispense."""
        dispenseHeight = 0
        
        if usingFreeAirDispense or self.__TipIsForbiddenToEnter(vial):
            dispenseHeight = vial.getBasePosition() - tipLength - vial.getMaxHeight()            
        else:     
            dispenseHeight = vial.getBasePosition() - tipLength - \
                   				   vial.getMeniscusAfterAdding(volumeToTransport_uL) + \
				                     float(self._m_Settings[Instrument.WickingDispenseDepthLabel])                        

        #self.__logger.logDebug( "@@@@@@@@@@@@@@@@@@@@@@@@@@@ __CalculateDispenseHeightFor %f %f %f %f %f " % (usingFreeAirDispense, \
        #                      vial.getBasePosition() , tipLength , vial.getMaxHeight(),\
        #                        self.__ClipHeight (dispenseHeight, vial, tipLength)))
        
        return self.__ClipHeight (dispenseHeight, vial, tipLength)
    
    
    def __TipIsForbiddenToEnter(self, vial, tipId = None):
        """Determine whether the current tip (or the one given) is allowed to enter a given vial."""
        if tipId == None:
            tipId = self.__m_Platform.CurrentTipID()[1]
        if vial.getLabel() in self.__forbiddenTipList.keys():
            result = tipId in self.__forbiddenTipList[vial.getLabel()]
	else:
	    result = False
        return result
    
    
    def __ClipHeight(self, height, vial, tipLength, isAspiration = False):
        """Ensure that the given height does not cause tip to get too close to the base"""
        
        if (vial.getLabel() == 'SeparationVial') and (isAspiration == True) and (tesla.config.SS_DEBUGGER_LOG == 1):
            print '\n[SeparationVial at Aspiration (Debug version)]'
            self.__logger.logDebug('DI: [ SeparationVial at Aspiration ]')            
            ssLogDebugger = tesla.DebuggerWindow.GetSSTracerInstance()
            maxHeight = vial.getBasePosition() - tipLength - ( float(self._m_Settings[Instrument.MinTipBaseGapLabel]) + ssLogDebugger.GetAspiHeight() )
        else:                       
            maxHeight = vial.getBasePosition() - tipLength - float(self._m_Settings[Instrument.MinTipBaseGapLabel])
        
        return min(height, maxHeight)
    
    
    def __WickingExtract(self, offset):
        """Perform a wicking extract by a given offset."""
        zAxis = self.__m_Platform.robot.Z()
        try:
            # Move up by the given offset, at the wicking speed
            #
            zAxis.SetStepSpeedProfileFromString(self._m_Settings[Instrument.WickingExtractVelocityProfileLabel])
            zAxis.IncrementPosition(-offset)
        finally:
            # Ensure that speed profile is reset
            #
            zAxis.ResetStepSpeedProfile()

    def SetBarcodeRescanOffset(self, offset):                                       
        self.barCodeRescanOffset = offset; 
        
    def GetBarcodeRescanOffset(self):                                           
        return self.barCodeRescanOffset;
    
    def SetBarcodeThetaOffset(self, offset):
        self.barCodeThetaOffset = offset; 
        
    def GetBarcodeThetaOffset(self):                                           
        return self.barCodeThetaOffset;

    def MoveToBarcodeOffset(self):
        self.__m_Platform.carousel.SetTheta(self.barCodeThetaOffset)

    def MoveToBarcodeOffsetForVial(self, vial):
        (sector, location) = self._LocationOf(vial) 
        self.__m_Platform.MoveToBarCodePosition(sector, location, self.barCodeThetaOffset)

    def GetFilled2mlVialVolume(self):
        return self.__filled2mlVialVolume

    def GetDefaultVialName(self, label):
        Labels = [Instrument.BCCMLabel,
           Instrument.WasteLabel,
           Instrument.LysisLabel,
           Instrument.CocktailLabel,
           Instrument.BeadLabel,
           Instrument.AntibodyLabel,
           Instrument.SampleLabel,
           Instrument.SupernatentLabel
           ]
        DefaultNames = [self._m_Settings[Instrument.BCCMContainerName],
                    self._m_Settings[Instrument.WasteVialName],
                    self._m_Settings[Instrument.LysisVialName],
                    self._m_Settings[Instrument.CocktailVialName],
                    self._m_Settings[Instrument.BeadVialName],
                    self._m_Settings[Instrument.AntibodyVialName],
                    self._m_Settings[Instrument.SampleVialName],
                    self._m_Settings[Instrument.SupernatentVialName]
                        ]
        result = label
        try:
            idx = Labels.index(label)
            result = DefaultNames[idx]
        except:
            self.logger.logDebug("CC: GetDefaultVialName: label not in list="+str(label))    

        return result


    
    # Set up our static methods
    GetVolumeToAddInTopUp = staticmethod(GetVolumeToAddInTopUp)
    GetVolumeToTransport = staticmethod(GetVolumeToTransport)
# eof
