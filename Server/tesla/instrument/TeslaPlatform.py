# 
# TeslaPlatform.py
# instrument.Platform.TeslaPlatform
#
# Adds the Tip Stripper control to the base Platform
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

# Oct 19 2006 - tip strip detection changes - RL

import os

import tesla.config
from tesla.exception import TeslaException
from tesla.hardware.config import HardwareConfiguration, LoadSettings, GetHardwareData
from tesla.hardware.ThetaAxis import ThetaAxis
from tesla.hardware.TipStripper import TipStripper
from tesla.hardware.LidSensor import LidSensor
from tesla.hardware.LidSensor import HydraulicSensor                #CWJ Add
from tesla.hardware.LidSensor import getHydraulicSensorConfigData   #CWJ Add
from tesla.hardware.LidSensor import BeaconDriver                   #CWJ Add
from tesla.hardware.LidSensor import getBeaconDriverConfigData      #CWJ Add
from tesla.instrument.Platform import Platform, PlatformError
import tesla.logger

from tesla.hardware.Axis import AxisError

from ipl.task.future import *

import time
from datetime import datetime # added by shabnam 
from tesla.types.ReferencePoint import ReferencePoint

from tesla.PgmLog import PgmLog    # 2011-11-29 sp -- programming logging

import SimpleStep.SimpleStep

# ---------------------------------------------------------------------------

class TeslaPlatformError(PlatformError):
    """Exception for errors related to Tesla Platform operation"""
    pass

# ---------------------------------------------------------------------------

class Tip:
    """Structure used to record data for tip geometry"""
    
    def __init__( self, offset, pickupPosition, prePickupPosition, \
                  stripPosition, strippedPosition):
        self.m_TravelOffset = offset
        self.m_PickupPosition = pickupPosition
        self.m_PrePickupPosition = prePickupPosition
        self.m_StripPosition = stripPosition
        self.m_StrippedPosition = strippedPosition
        
# ---------------------------------------------------------------------------

class TeslaPlatform (Platform):
    """Adds the Tip Stripper control to the base Platform,
        thereby encompassing all mechanical components of the Tesla instrument"""
    SectionName                 = "Platform"
    TipStripperSectionName      = "TipStripper"
    LidSensorSectionName        = "LidSensor"
    HydraulicSensorSectionName  = "HydraulicSensor"      #CWJ Add
    BeaconDriverSectionName     = "BeaconDriver"         #CWJ Add
    # Configuration data
    #
    TravelPositionLabel         = 'z_travelposition_mm'    
    TravelOffsetNoTipLabel      = 'z_traveloffset_notip_mm'    
    TravelOffset1mLTipLabel     = 'z_traveloffset_1mltip_mm'    
    TravelOffset5mLTipLabel     = 'z_traveloffset_5mltip_mm'    
    PickupTipPowerProfileLabel  = 'z_pickuptip_powerprofile'
    PickupTipIdlePowerProfileLabel  = 'z_pickuptip_idlepowerprofile'
    StripTipPowerProfileLabel   = 'z_striptip_powerprofile'
    PickupTipVelocityProfileLabel = 'z_pickuptip_velocityprofile'
    StripTipVelocityProfileLabel = 'z_striptip_velocityprofile'
    PrePickupPosition1mLTipLabel = 'z_prepickupposition_1mltip_mm'    
    PrePickupPosition5mLTipLabel = 'z_prepickupposition_5mltip_mm'    
    PickupPosition1mLTipLabel   = 'z_pickupposition_1mltip_mm'    
    PickupPosition5mLTipLabel   = 'z_pickupposition_5mltip_mm'    
    PostPickupPositionLabel     = 'z_postpickupposition_mm'    
    StripPosition1mLTipLabel    = 'z_stripposition_1mltip_mm'    
    StripPosition5mLTipLabel    = 'z_stripposition_5mltip_mm'    
    StrippedPosition1mLTipLabel = 'z_strippedposition_1mltip_mm'    
    StrippedPosition5mLTipLabel = 'z_strippedposition_5mltip_mm' 
    TipStripperSensorDelayLabel = "tipstripper_sensor_delay"   

    configData =            {
                            Platform.CarouselThetaOffsetLabel   : '-18',
                            Platform.RobotThetaOffsetLabel      : '36',
                            Platform.NbrSectorsLabel            : '4',
                            Platform.SectorSizeLabel            : '90',
                            TravelPositionLabel                 : '111',
                            TravelOffsetNoTipLabel              : '0',
                            TravelOffset1mLTipLabel             : '-92.5',   
                            TravelOffset5mLTipLabel             : '-110.75',
                            PrePickupPosition1mLTipLabel        : '100',
                            PrePickupPosition5mLTipLabel        : '100',
                            PickupPosition1mLTipLabel           : '143',
                            PickupPosition5mLTipLabel           : '147',
                            PostPickupPositionLabel             : '10',
                            StripPosition1mLTipLabel            : '117',
                            StripPosition5mLTipLabel            : '133',
                            StrippedPosition1mLTipLabel         : '100',
                            StrippedPosition5mLTipLabel         : '100',
                            PickupTipPowerProfileLabel          : '170, 170, 0',
                            PickupTipIdlePowerProfileLabel      : '200, 200, 0',
                            PickupTipVelocityProfileLabel       : '500, 20000, 20',
                            StripTipPowerProfileLabel           : '170, 170, 0',
                            StripTipVelocityProfileLabel        : '500, 20000, 20',

                            TipStripperSensorDelayLabel: '1.0'
                            }

    # Mnemonics for reference points
    #
    ParticleVial_Label  =  'particlevial_degrees'
    CocktailVial_Label  =  'cocktailvial_degrees'
    AntibodyVial_Label  =  'antibodyvial_degrees'
    Sample_Label        =  'samplevial_14ml_degrees'
    Tip1_1ml_Label      =  'tip_1ml_position1_degrees'
    Tip2_1ml_Label      =  'tip_1ml_position2_degrees'
    Tip3_1ml_Label      =  'tip_1ml_position3_degrees'
    Tip4_5ml_Label      =  'tip_5ml_position4_degrees'
    Tip5_5ml_Label      =  'tip_5ml_position5_degrees'
    StripTip1_1ml_Label =  'striptip_1ml_position1_degrees'
    StripTip2_1ml_Label =  'striptip_1ml_position2_degrees'
    StripTip3_1ml_Label =  'striptip_1ml_position3_degrees'
    StripTip4_5ml_Label =  'striptip_5ml_position4_degrees'
    StripTip5_5ml_Label =  'striptip_5ml_position5_degrees'
    LysisBuffer_Label   =  'lysisbuffervial_50ml_degrees'        
    Separation_Label    =  'separationvial_14ml_degrees'
    Waste_Label         =  'wastevial_50ml_degrees'
    BulkLabel           =  'bccm_bulkcontainer_degrees'    # NB: special case. Not on the carousel
    CalibrationJigSlotLabel = 'calibrationjigslot_degrees'
    CalibrationJigZAxisFeatureLabel = 'calibrationjigzaxisfeature_degrees'
    CalibrationJigSlot2Label = 'calibrationjigslot2_degrees'
    CalibrationJigZAxisFeature2Label = 'calibrationjigzaxisfeature2_degrees'

    SamplevialDistance= 'samplevialdistance'
    SeparationvialDistance= 'separationvialdistance'
    WastevialDistance='wastevialdistance'
    LysisbuffervialDistance='lysisbuffervialdistance'
    CocktailvialDistance='cocktailvialdistance'
    ParticlevialDistance='particlevialdistance'
    AntibodyvialDistance='antibodyvialdistance'
    Position1Distance='position1distance'
    Position2Distance='position2distance'
    Position3Distance='position3distance'
    Position4Distance='position4distance'
    Position5Distance='position5distance'

    referencePointCalibrationDistanceNames = {
                        Sample_Label:SamplevialDistance,
                        Separation_Label:SeparationvialDistance,
                        Waste_Label:WastevialDistance,
                        LysisBuffer_Label:LysisbuffervialDistance,
                        CocktailVial_Label:CocktailvialDistance,
                        ParticleVial_Label:ParticlevialDistance,
                        AntibodyVial_Label:AntibodyvialDistance,
                        Tip1_1ml_Label:Position1Distance,
                        Tip2_1ml_Label:Position2Distance,
                        Tip3_1ml_Label:Position3Distance,
                        Tip4_5ml_Label:Position4Distance,
                        Tip5_5ml_Label:Position5Distance}

    SamplevialToSampleDeg='samplevialtosampledeg'
    SeparationvialToSampleDeg='separationvialtosampledeg'
    WastevialToSampleDeg='wastevialtosampledeg'
    LysisbuffervialToSampleDeg='lysisbuffervialtosampledeg'
    CocktailvialToSampleDeg='cocktailvialtosampledeg'
    ParticlevialToSampleDeg='particlevialtosampledeg'
    AntibodyvialToSampleDeg='antibodyvialtosampledeg'
    Position1ToSampleDeg='position1tosampledeg'
    Position2ToSampleDeg='position2tosampledeg'
    Position3ToSampleDeg='position3tosampledeg'
    Position4ToSampleDeg='position4tosampledeg'
    Position5ToSampleDeg='position5tosampledeg'
    
    referencePointCalibrationToSampleDegNames ={
                        Sample_Label:SamplevialToSampleDeg,
                        Separation_Label:SeparationvialToSampleDeg,
                        Waste_Label:WastevialToSampleDeg,
                        LysisBuffer_Label:LysisbuffervialToSampleDeg,
                        CocktailVial_Label:CocktailvialToSampleDeg,
                        ParticleVial_Label:ParticlevialToSampleDeg,
                        AntibodyVial_Label:AntibodyvialToSampleDeg,
                        Tip1_1ml_Label:Position1ToSampleDeg,
                        Tip2_1ml_Label:Position2ToSampleDeg,
                        Tip3_1ml_Label:Position3ToSampleDeg,
                        Tip4_5ml_Label:Position4ToSampleDeg,
                        Tip5_5ml_Label:Position5ToSampleDeg}
                                            
    referencePointCalibrationList = [
                          Sample_Label,
                          Separation_Label,
                          Waste_Label,
                          LysisBuffer_Label,
                          CocktailVial_Label,
                          ParticleVial_Label,
                          AntibodyVial_Label,
                          Tip1_1ml_Label,
                          Tip2_1ml_Label,
                          Tip3_1ml_Label,
                          Tip4_5ml_Label,
                          Tip5_5ml_Label
                          ]
    referencePointCalibrationDistances ={Sample_Label:0, #ReferencePoint.samplevialDistance, 
                     Separation_Label:0, #ReferencePoint.separationvialDistance, 
                     Waste_Label:0, #ReferencePoint.wastevialDistance, 
                     LysisBuffer_Label:0, #ReferencePoint.lysisbuffervialDistance, 
                     CocktailVial_Label:0, #ReferencePoint.cocktailvialDistance, 
                     ParticleVial_Label:0, #ReferencePoint.particlevialDistance,
                     AntibodyVial_Label:0, #ReferencePoint.antibodyvialDistance, 
                     Tip1_1ml_Label:0, #ReferencePoint.position1Distance, 
                     Tip2_1ml_Label:0, #ReferencePoint.position2Distance, 
                     Tip3_1ml_Label:0, #ReferencePoint.position3Distance, 
                     Tip4_5ml_Label:0, #ReferencePoint.position4Distance,
                     Tip5_5ml_Label:0, #ReferencePoint.position5Distance
                     }
    referencePointCalibrationToSampleDegs ={Sample_Label:0, #ReferencePoint.samplevialToSampleDeg, 
                     Separation_Label:0, #ReferencePoint.separationvialToSampleDeg, 
                     Waste_Label:0, #ReferencePoint.wastevialToSampleDeg, 
                     LysisBuffer_Label:0, #ReferencePoint.lysisbuffervialToSampleDeg, 
                     CocktailVial_Label:0, #ReferencePoint.cocktailvialToSampleDeg, 
                     ParticleVial_Label:0, #ReferencePoint.particlevialToSampleDeg,
                     AntibodyVial_Label:0, #ReferencePoint.antibodyvialToSampleDeg, 
                     Tip1_1ml_Label:0, #ReferencePoint.position1ToSampleDeg, 
                     Tip2_1ml_Label:0, #ReferencePoint.position2ToSampleDeg, 
                     Tip3_1ml_Label:0, #ReferencePoint.position3ToSampleDeg, 
                     Tip4_5ml_Label:0, #ReferencePoint.position4ToSampleDeg,
                     Tip5_5ml_Label:0, #ReferencePoint.position5ToSampleDeg
                     }
    referencePointData =    {    
                            ParticleVial_Label  : '-24,      2,    47.2,  -76.8',
                            CocktailVial_Label  : '-24,    -4.5,   47.2,  -84',
                            AntibodyVial_Label  : '-24,    -11.75, 47.2,  -91',
                            Sample_Label        : '-22.55, -45,    45.75,-123.5',
                            Tip1_1ml_Label      : '-14,    -26.6,  45.2, -110.7',
                            Tip2_1ml_Label      : '-14,    -31.95, 45.2, -101.65',
                            Tip3_1ml_Label      : '-14,    -37.3,  37.2, -107.7',
                            Tip4_5ml_Label      : '-22,    -23.55, 37.2, -102.35',
                            Tip5_5ml_Label      : '-22,    -32.55, 45.2,  -97',
                            StripTip1_1ml_Label : '-14,    -26.6',
                            StripTip2_1ml_Label : '-14,    -31.95',
                            StripTip3_1ml_Label : '-14,    -37.3',
                            StripTip4_5ml_Label : '-22,    -23.55',
                            StripTip5_5ml_Label : '-22,    -32.55',
                            LysisBuffer_Label   : '-12.5,  -12,    35.5,  -81',
                            Separation_Label    : '-7.1,   -75.5,  30.3, -135',
                            Waste_Label         : '11,     -62,    11,    -62',    
                            BulkLabel           : '89',
                            CalibrationJigSlotLabel : '-29,10',
                            CalibrationJigZAxisFeatureLabel : '-29,-20',
                            CalibrationJigSlot2Label : '86, -90',
                            CalibrationJigZAxisFeature2Label : '86, -112.5',
                            }

    # Initialise this in first __init__ call
    tipReferenceMap = {}
    tipStripReferenceMap = {}

    # ---------------------------------------------------------------------------
    
    def __init__( self, name ):
        """Load in the configuration data for the Platform."""
        # global gHardwareData  # 2012-01-04 sp removed based on changes to function

        referenceSettings = TeslaPlatform.referencePointData.copy()
        referenceConfigData = GetHardwareData().Section ('ReferencePoints')
        LoadSettings (referenceSettings, referenceConfigData)
        
        Platform.__init__(self, name, TeslaPlatform.configData, referenceSettings)

        # Set up the tip stripper (which uses the carousel stepper card)
        tipStripperSettings = GetHardwareData().Section (TeslaPlatform.TipStripperSectionName)
        firmWareVersion = self.carousel._ThetaAxis().GetFirmwareVersion()
        #firmWareVersion = "109:012:079:328:001"
        #firmWareVersion = "300:012:008:328:001"
        try:
            self.__SS_XY_MICRO_LC = tesla.config.SS_XY_MICRO_LC
            version = int(firmWareVersion.split(":")[0])
            if version>=300:
                self.__SS_XY_MICRO_LC = 1
            else:
                self.__SS_XY_MICRO_LC = 0
            #print "TeslaPlatform.__SS_XY_MICRO_LC "+str(self.__SS_XY_MICRO_LC) + ", " + version
        except:
            pass

        if self.debugFlag: print tipStripperSettings
        self.tipStripper = TipStripper(TeslaPlatform.TipStripperSectionName, 
                            self.carousel._ThetaAxis().m_Card, tipStripperSettings)


        # Set up the lid sensors (which uses the carousel stepper card)
        lidSensorSettings = GetHardwareData().Section (TeslaPlatform.LidSensorSectionName)
        print lidSensorSettings
        self.lidSensor = LidSensor(TeslaPlatform.LidSensorSectionName, 
                            self.carousel._ThetaAxis().m_Card, lidSensorSettings)
        
        # Finally, complete initialisation of tip references
        #if len(TeslaPlatform.tipReferenceMap.keys()) == 0:
        self.__InitialiseTipReference()

        self.__m_CurrentTip       = None
        self.__m_CurrentTipSector = None
        self.__m_CurrentTipID     = None

        self.__logger = tesla.logger.Logger(logName = tesla.config.LOG_PATH)
        if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
            self.ExtLogger = SimpleStep.SimpleStep.GetSSExtLoggerInstance()

            
        self.__logger.logInfo( "TeslaPlatform.__SS_XY_MICRO_LC=%d"%(self.__SS_XY_MICRO_LC))

        # 2011-11-29 sp -- added logging
        self.svrLog = PgmLog( 'svrLog' )
        self.logPrefix = 'TP'

    def AttatchHydraulicSensor(self, PumpCard):           #CWJ Add
        # Set up the Hydraulic Sensor                     #CWJ Add
        HydraulicSensorSettings = GetHardwareData().Section (TeslaPlatform.HydraulicSensorSectionName)
        settings  = getHydraulicSensorConfigData(HydraulicSensorSettings)
        bIgnore   = int( settings[HydraulicSensor.IgnoreSensorLabel] ) == 1
        port      = int( settings[HydraulicSensor.SensorPortLabel] )
        cardprefix= settings[HydraulicSensor.CardAddressLabel]
        
        cardlist = [
                      self.robot.getZAxis().m_Card, 
                      self.robot.getThetaAxis().m_Card,
                      self.carousel._ThetaAxis().m_Card,
                      PumpCard
                   ]
                   
        pumpcard = None
        for card in cardlist :
            if cardprefix == card.prefix:
               pumpcard = card

        self.HydraulicSensor = HydraulicSensor( TeslaPlatform.HydraulicSensorSectionName,
                                                pumpcard,
                                                port, 
                                                bIgnore,
                                                HydraulicSensorSettings );      

    def AttatchBeaconDriver(self, PumpCard):              #CWJ Add
        # Set up the Beacon Driver                        #CWJ Add
        BeaconDriverSettings = GetHardwareData().Section (TeslaPlatform.BeaconDriverSectionName)
        settings  = getBeaconDriverConfigData(BeaconDriverSettings)
        port      = int( settings[BeaconDriver.BeaconPortLabel] )
        channel   = int( settings[BeaconDriver.BeaconChannelLabel] )
        evt1      = int( settings[BeaconDriver.BeaconEvent1TimeLabel] )
        evt2      = int( settings[BeaconDriver.BeaconEvent2TimeLabel] )
        evt3      = int( settings[BeaconDriver.BeaconEvent3TimeLabel] )
        cardprefix= settings[BeaconDriver.CardAddressLabel]
        
        cardlist = [
                      self.robot.getZAxis().m_Card, 
                      self.robot.getThetaAxis().m_Card,
                      self.carousel._ThetaAxis().m_Card,
                      PumpCard
                   ]
                   
        pumpcard = None
        for card in cardlist :
            if cardprefix == card.prefix:
               pumpcard = card

        self.BeaconDriver = BeaconDriver( pumpcard, port, channel, evt1, evt2, evt3);
    
    def _serviceMethods(self):
        '''Return a list of methods for the service interface'''
        return ['Initialise', 'MoveTo', 'MoveToPosition', 'MoveToSectorAndTip',
                'PickupTip', 'StripTip', 'powerDownCarousel','SmartPickupTip'
                ]


    def Initialise(self):
        """Perform initialisation of the Platform components"""
        # Disengage the stripper before initialising the platform
        self.tipStripper.Disengage()

        # Set the Z travel position to zero during initialise
        # (will cause problems if tip is still present otherwise!!)
        self.robot.SetZTravelPosition (None)
        Platform.Initialise (self)
        self.robot.SetZTravelPosition (float(self._m_Settings[TeslaPlatform.TravelPositionLabel]))
        #    03/14/07 - tip strip only if necessary code -RL
        self.StripTip()
        self.__m_CurrentTip = None

        self.__tipStripperSensorDelay = float (self._m_Settings[TeslaPlatform.TipStripperSensorDelayLabel])
        print "self.__tipStripperSensorDelay",self.__tipStripperSensorDelay

   
    def MoveCarouselToSafePosition(self):
        '''Move the carousel to a safe position (away from the opto for easy
        removal)'''
        self.carousel.MoveToSafePosition()

    def powerDownCarousel( self ):
        '''Powers down the carousel so it can move freely'''
        return self.carousel._ThetaAxis().removePower()

   
    def MoveTo(self, sector, referencePoint, finalZPosition):
        """Move the dispenser to a given position:
            - Raise the robot arm to travel position.
            - Move the carousel and robot to the given referencePoint.
            - Lower the robot so that the current tip is at the given finalZPosition."""
        self.MoveToPosition (sector, referencePoint)
        #self.__logger.logDebug(  "@@@@@@@@@@@@@@@@@@@@@@@@@@@ SetZPosition %f" % finalZPosition)
        
        # db added 2016/02/10 --- BEGIN SUBROUTINE --- for Automated Pump Calibration/Verification (DCV)
        """This subroutine sets the robot arm's z-height to the defined value when the os environment
        variable ROBO_DCVJIG_ZT is defined and the target referencePoint is the bccm container.
            ROBO_DCVJIG_ZT: a user env var with 2 semi-colon-separated parameters: <z-height>;<theta-offset>
            <z-height>: a value [10-148] that determines the robot arm height (default: ~115)
            <theta-offset>: a value that determines the theta offset (recommended: ~147, default: 0). """
        if ("ROBO_DCVJIG_ZT" in os.environ) and (referencePoint == Platform.BulkLabel): # check if conditions are met
            calZPosition = float(os.environ["ROBO_DCVJIG_ZT"].split(';')[0]) # grab custom z-height value (defined in ROBO_DCVJIG_ZT[0])
            self.robot.SetZPosition (calZPosition) # set the robot arm's z-height to custom z-height value
        else:
        # db added 2016/02/10 --- END SUBROUTINE ---
            self.robot.SetZPosition (finalZPosition)
        

    def DelayMoveTo(self, sector, referencePoint, finalZPosition): #CWJ Add
        """Move the dispenser to a given position:
            - Raise the robot arm to travel position.
            - Move the carousel and robot to the given referencePoint.
            - Lower the robot so that the current tip is at the given finalZPosition."""
        self.DelayMoveToPosition (sector, referencePoint)
        #self.__logger.logDebug(  "@@@@@@@@@@@@@@@@@@@@@@@@@@@ SetZPosition %f" % finalZPosition)
        self.robot.SetZPosition (finalZPosition)

    def PickupTip(self, sector, tipNbr):
        """Pickup a tip from the given position.
            - Check that the given tipNbr is valid
            - Move to pick up the tip
            - Engage the tip
            - Update the travel position
            It is assumed that there is no tip currently in place"""
        
        funcReference = __name__ + '.PickupTip'          # 2011-11-29 sp -- added logging
        if self.CurrentTipID() == (sector, tipNbr):
            # Tip is already loaded. Ignore call
            self.svrLog.logDebug('', self.logPrefix, funcReference, "Tip is already loaded. Ignore call")   # 2011-12-13 sp -- added logging
            pass

        else:
            if self.__m_CurrentTip != None:
                self.svrLog.logError('', self.logPrefix, funcReference, "Cannot pick up a tip with another one already present!")   # 2011-12-13 sp -- added logging
                raise TeslaPlatformError ("Cannot pick up a tip with another one already present!")

            if not tipNbr in TeslaPlatform.tipReferenceMap.keys():
                self.svrLog.logError('', self.logPrefix, funcReference, "Tip %d is not defined." % (tipNbr))   # 2011-12-13 sp -- added logging
                raise TeslaPlatformError ("Tip %d is not defined." % (tipNbr))

            referencePoint = TeslaPlatform.tipReferenceMap[tipNbr][0]
            tipData = TeslaPlatform.tipReferenceMap[tipNbr][1]
            
            self.svrLog.logDebug('', self.logPrefix, funcReference, "start tip pickup; sector=%d |tip=%d | ref_point=%s" %
                    (sector, tipNbr, referencePoint) )   # 2011-12-13 sp -- added logging
            try:
                # Down
                print '\n#### PickUp Tip down ####\n'
                self.MoveToPosition( sector, referencePoint)        
                # 2012-01-30 sp -- replace environment variable with configuration variable
                #if os.environ.has_key('SS_CHATTER'):
                if tesla.config.SS_CHATTER == 1:
                   data = self.robot.moveToHomeAndCheck()
                   self.__logger.logInfo( "moveToHomeAndCheck Before TipPickup down (To Start Point:%d, To Home:%d)"%(data[0],data[1]))               
                   self.svrLog.logInfo('', self.logPrefix, funcReference, "moveToHomeAndCheck Before TipPickup down (To Start Point:%d, To Home:%d)"%(data[0],data[1]))   # 2011-11-29 sp -- added logging
                else:
                    self.robot.moveToHomeAndCheck() 
                self.__SetPickupTipState( True )

                zAxis = self.robot.Z()

                zAxis.ResetStepSpeedProfile()

                self.robot.SetZPosition( tipData.m_PrePickupPosition )
        
                idleProfile =  self._m_Settings[TeslaPlatform.PickupTipIdlePowerProfileLabel] 
                zAxis.SetIdlePowerFromString( idleProfile )

                powerProfile = self._m_Settings[TeslaPlatform.PickupTipPowerProfileLabel] 
                zAxis.SetPowerFromString( powerProfile )

                velocityProfile = self._m_Settings[TeslaPlatform.PickupTipVelocityProfileLabel]
                zAxis.SetStepSpeedProfileFromString(velocityProfile)

                self.robot.SetZPosition( tipData.m_PickupPosition )
                
                zAxis.ResetStepSpeedProfile()
                

                # Note the current tip
                #
                self.__m_CurrentTipSector = sector
                self.__m_CurrentTipID = tipNbr
                self.__m_CurrentTip = TeslaPlatform.tipReferenceMap[tipNbr]

            finally:
                self.__SetPickupTipState (False)

        # On successful pick up, Home, and move to travel plane
        #
        print '\n#### PickUp Tip Success! ####\n'        
        #raw_input('Press <Enter> to Start!!!') #add by shabnam
        # 2012-01-30 sp -- replace environment variable with configuration variable
        #if os.environ.has_key('SS_LEGACY'):
        if tesla.config.SS_LEGACY == 1:
           self.robot.SetZPosition( float( self._m_Settings[TeslaPlatform.PostPickupPositionLabel] ) )
        else:
           self.robot.SetZPositionWithoutMotion( float( self._m_Settings[TeslaPlatform.PostPickupPositionLabel] ) )
        zAxis.ResetPower()
        zAxis.ResetIdlePower()

        # 2012-01-30 sp -- replace environment variable with configuration variable
        #if os.environ.has_key('SS_CHATTER'):
        if tesla.config.SS_CHATTER == 1:
            data = self.robot.moveToHomeAndCheck()
            self.__logger.logInfo( "moveToHomeAndCheck After TipPickup up (To Start Point:%d, To Home:%d)"%(data[0],data[1]))               
            self.svrLog.logInfo('', self.logPrefix, funcReference, "moveToHomeAndCheck After TipPickup up (To Start Point:%d, To Home:%d)"%(data[0],data[1]))   # 2011-11-29 sp -- added logging
        else:
           self.robot.moveToHomeAndCheck() 

        self.robot.PrepareZForTravel()            

    def SmartPickupTip(self, sector, tipNbr):
        if self.CurrentTipID() == (sector, tipNbr):
            print  "@@@@@@@@@@@@@@@@@@@@@@@@@@@ SmartPickupTip ignore call %s " % (self.CurrentTipID(),)
            # Tip is already loaded. Ignore call
            pass

        else:
            print  "@@@@@@@@@@@@@@@@@@@@@@@@@@@ SmartPickupTip doing stuff"
            #otherwise strip and pickup
            if self.__m_CurrentTip != None:
                print  "@@@@@@@@@@@@@@@@@@@@@@@@@@@ tip not none"
                self.StripTip()
            print  "@@@@@@@@@@@@@@@@@@@@@@@@@@@ do normal pickup"
            print "Tip %d is ." % (tipNbr)
            print "sector %d is ." % (sector)
            self.PickupTip(sector, tipNbr)
            print  "@@@@@@@@@@@@@@@@@@@@@@@@@@@ SmartPickupTip done!!!"


    def HomeZAxis(self):
        '''Home *just* the Z axis.'''
        self.robot.HomeZ()


    def StripTip(self, boDelay=False):
        """Strip the current tip.
            - Check that the given tipNbr is valid
            - Move Z-Axis to home and check for home
            - Move to the current tip's original pickup point
            - Move Z-Axis to the strip position
            - Engage the tip stripper
            - Set high power for strip action
            - Move Z-Axis up to stripped position
            - Home the Z axis
            - Disengage the Tip Stripper
            - Move the Z-Axis to the travel position (without tip)
            It is assumed that there is a tip currently in place"""
        if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
            self.ExtLogger.SetCmdListLog("", 'X0')
            self.ExtLogger.SetCmdListLog("---->> Start Tesla.StripTip()", 'X0')
        funcReference = __name__ + '.StripTip'          # 2011-11-29 sp -- added logging
        self.svrLog.logDebug('X', self.logPrefix, funcReference, 'Start stripTip, axis=X0')   # 2011-11-29 sp -- added logging

        self.__logger.logInfo( "StripTip boDelay=%s"%(boDelay))               
        self.svrLog.logInfo('', self.logPrefix, funcReference, "StripTip boDelay=%s"%(boDelay))
        
        if self.__m_CurrentTip == None:
            # No tip present. Ignore call.
            pass

        else:
            isStripperArmFailed = True
            try:
                tipData = self.__m_CurrentTip[1]
                pickupReferencePoint = TeslaPlatform.tipReferenceMap[self.__m_CurrentTipID][0]
                stripReferencePoint = TeslaPlatform.tipStripReferenceMap[self.__m_CurrentTipID]

                # Move z axis to home
                # 2012-01-30 sp -- replace environment variable with configuration variable
                #if os.environ.has_key('SS_CHATTER'):              #CWJ Add
                if tesla.config.SS_CHATTER == 1:              #CWJ Add
                   data = self.robot.moveToHomeAndCheck_debug()   #CWJ Add
                else:                                             #CWJ Add
                   self.robot.moveToHomeAndCheck() 
                   #raw_input('Press <Enter> to Start!!! u r in striptip def after move to home')   # add by shabnam

                # Initially move to the pickup point before lowering the tip.
                # This gives a better insert position
                if boDelay:
                    self.DelayMoveTo(self.__m_CurrentTipSector, pickupReferencePoint, tipData.m_StripPosition)
                else:
                    self.MoveTo(self.__m_CurrentTipSector, pickupReferencePoint, tipData.m_StripPosition)
                #raw_input('Press <Enter> to Start!!! u are after self.moveTo...... ') #add by shabnam
                # Now move to the strip reference point, for better meshing with the stripper
                # (NB: We're talking fractions of a mm here)
                (rTheta, cTheta) = self._ObtainRendezvous(self.__m_CurrentTipSector, stripReferencePoint, 0)

                future = Future( self.tipStripper.Engage )
                self.robot.Theta().SetTheta(rTheta)
                print "...Move to strip psn theta = ", rTheta
                #raw_input('Press <Enter> to Start!!! u are in strip Tip def after theta ') #add by shabnam
                future()
                
                # Oct 19 2006 - tip strip detection changes - RL
                #stripper can't be both home and limit
                print "self.__tipStripperSensorDelay",self.__tipStripperSensorDelay
                time.sleep(self.__tipStripperSensorDelay) #commented by shabnam

                tipStripWarningMsg = "Tip strip could fail because stripper arm didn't come out all the way."
                tipStripFailedMsg = "Tip strip failed because stripper arm didn't come out all the way."

                if tesla.config.SS_FORCE_EMULATION == 1:
                    isStripperArmFailed = False
                elif self.__SS_XY_MICRO_LC == 1: #2019-04-09 new logic for new board without i27
                    #Michael: Error messages (when I26==1, even though tip stripper is extended out) will be displayed/logged depending on SS_EnableStripArmPositionCheck. 
                    #tipStripper.getHomeStatus(self): checks I26
                    i26 = self.tipStripper.getHomeStatus()
                    msg = 'SS_XY_MICRO_LC->I26 =' + i26
                    self.__logger.logDebug(msg)
                    isStripperArmFailed = int(i26) == 1 #ERROR case!
                    ###isStripperArmFailed = True #force emulation tip strip fail
                    if isStripperArmFailed: 
                        boRaiseError = tesla.config.SS_ENABLE_STRIP_ARM_POSITION_CHECK == 1
                        if boRaiseError:
                            msg = tipStripFailedMsg
                        else:
                            msg = tipStripWarningMsg

                        print ('\n### Strip ARM Failed with SSXYMicroLC ####\n')
                        self.__logger.logDebug('### Strip ARM Failed with SSXYMicroLC ####')
                        self.__logger.logDebug(msg)
                        self.svrLog.logError('', self.logPrefix, funcReference, msg)   
                        if( tesla.config.SS_EXT_LOGGER == 1 ):  
                            self.ExtLogger.SetCmdListLog(msg, self.tipStripper.m_Card.prefix)
                            self.ExtLogger.CheckSystemAvail()
                            if boRaiseError:
                                self.ExtLogger.DumpHistory()
                        if boRaiseError:
                            raise AxisError ('Tip strip failed')
                        else:
                            isStripperArmFailed = False
                            
                else: #else stick with old logic (before 2019-04-09)

                    isStripperArmFailed = self.tipStripper.getHomeStatus() == self.tipStripper.getLimitStatus()

                    if isStripperArmFailed:
                        # 2012-01-30 sp -- replace environment variable with configuration variable
                        #if os.environ.has_key('SS_FORCE_EMULATION'):
                        if tesla.config.SS_FORCE_EMULATION == 1:
                            isStripperArmFailed = False
                        else: 
                            # 2012-01-30 sp -- replace environment variable with configuration variable
                            #if os.environ.has_key('SS_OLDPCB'):
                            if tesla.config.SS_OLDPCB == 1:
                                # 
                                #
                                # No Longer support this block- 2019 Aug CJ
                                #
                                #
                                self.__logger.logDebug(  "Tip strip could fail because stripper arm didn't come out all the way.")
                                self.svrLog.logError('', self.logPrefix, funcReference, "Tip strip could fail because stripper arm didn't come out all the way.")   # 2011-11-29 sp -- added logging
                                if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                                    self.ExtLogger.SetCmdListLog("Tip strip could fail because stripper arm didn't come out all the way.", self.tipStripper.m_Card.prefix)
                                    self.ExtLogger.CheckSystemAvail()
                                    # self.ExtLogger.DumpHistory()
                                #raise AxisError ('Tip strip failed')
                                isStripperArmFailed = False #REMOVE this if raise error
                           
                                # raw_input('\n##### Tip Stripper Arm Failed!!!! #####\a')
                           
                            else:   
                                print ('\n### New PCB Strip ARM ####\n')

                                boRaiseError = tesla.config.SS_ENABLE_STRIP_ARM_POSITION_CHECK == 1

                                self.__logger.logDebug(  "Tip strip failed because stripper arm didn't come out all the way.")
                                self.svrLog.logError('', self.logPrefix, funcReference, "Tip strip failed because stripper arm didn't come out all the way.")   # 2011-11-29 sp -- added logging
                                if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                                    self.ExtLogger.SetCmdListLog("Tip strip failed because stripper arm didn't come out all the way.", self.tipStripper.m_Card.prefix)
                                    self.ExtLogger.CheckSystemAvail()
                                    self.ExtLogger.DumpHistory()
                                
                                if boRaiseError:
                                    self.__logger.logDebug('### SS_ENABLE_STRIP_ARM_POSITION_CHECK = 1 ####')      

                                    raise AxisError ('Tip strip failed')

                                    # isStripperArmFailed = False #REMOVE this if raise error
                                else:
                                    self.__logger.logDebug('### SS_ENABLE_STRIP_ARM_POSITION_CHECK = 0 ####')                                                                
                                    
                                    #raise AxisError ('Tip strip failed')
                                    
                                    isStripperArmFailed = False #REMOVE this if raise error

                                # raw_input('\n##### Tip Stripper Arm Failed!!!! #####\a')

                
                # Commence the actual strip                
                self.__SetStripTipState(True)
                self.robot.SetZPosition(tipData.m_StrippedPosition, 1)

                # Update the current tip
                #comment for tip strip failure detection
                #self.__m_CurrentTipID = None
                #self.__m_CurrentTip = None
                #self.robot.SetZTravelPosition(float(self._m_Settings[TeslaPlatform.TravelPositionLabel]))
            finally:
                # Ensure that the tip is stripped by disengaging *after* homing
                self.__SetStripTipState(False)
                                
                #comment for tip strip failure detection
                #future = Future( self.robot.moveToHomeAndCheck )
                #self.tipStripper.Disengage()
                #future()
                # Oct 19 2006 - tip strip detection changes - RL
                #uncomment for tip strip failure detection

                try:
                    if not isStripperArmFailed:
                        # 2012-01-30 sp -- replace environment variable with configuration variable
                        #if os.environ.has_key('SS_CHATTER'):
                        if tesla.config.SS_CHATTER == 1:
                            #self.robot.moveToHomeAndCheck_debug(True)
                            future = Future( self.robot.moveToHomeAndCheck_debug, True ) 
                        else:
                            #self.robot.moveToHomeAndCheck(True)
                            future = Future( self.robot.moveToHomeAndCheck, True ) # added by shabnam 
                        self.tipStripper.Disengage() # added by shabnam
                        future() # added by shabnam 
                        #raw_input('Press <Enter> to Start!!! u are in strip Tip after home 636') #add by shabnam                                                        
                        # Update the current tip
                        self.__m_CurrentTipID = None
                        self.__m_CurrentTip = None
                finally:
                    #self.tipStripper.Disengage()
                    #t2 = datetime.now() #added by shabnam                                        
                    #raw_input('Press <Enter> to Start!!! u are in strip Tip after disengage') #add by shabnam
                    print datetime.now()
                                        
                    # 2012-01-30 sp -- replace environment variable with configuration variable
                    #if os.environ.has_key('SS_CHATTER'):
                    if tesla.config.SS_CHATTER == 1:
                       data = self.robot.moveToHomeAndCheck(False)
                       self.__logger.logInfo( "moveToHomeAndCheck After StripTip (To Start Point:%d, To Home:%d)"%(data[0],data[1]))               
                       self.svrLog.logInfo('', self.logPrefix, funcReference, "moveToHomeAndCheck After StripTip (To Start Point:%d, To Home:%d)"%(data[0],data[1]))   # 2011-11-29 sp -- added logging
                    else:
                       self.robot.moveToHomeAndCheck(False)
                       #raw_input('Press <Enter> to Start!!! u are in strip Tip after home and check') #add by shabnam                                           

        # On successful strip, move to travel plane
        self.robot.PrepareZForTravel( 2 )            

        if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
            self.ExtLogger.SetCmdListLog("---->> Finish Tesla.StripTip()", 'X0')
            self.ExtLogger.SetCmdListLog("", 'X0')
        self.svrLog.logDebug('X', self.logPrefix, funcReference, 'Completed stripTip, axis=X0')   # 2011-11-29 sp -- added logging


    def CurrentTipID (self):
        """Return the sector and storage position of the currently loaded tip.
            If not tip is currently loaded, the storage position will be None, but the sector for the last tip used is retained."""
        return (self.__m_CurrentTipSector, self.__m_CurrentTipID)
        

    def CurrentTipLength (self):
        """Return the length of the currently loaded tip."""
        if self.__m_CurrentTip == None:
            return 0
        else:
            return (-1 * self.__m_CurrentTip[1].m_TravelOffset)
        
    # ---------------------------------------------------------------------------
    #   Private methods: accessed via exception handlers
    # ---------------------------------------------------------------------------

    def __InitialiseTipReference (self):
        """Initialise the Tesla Platform tip references"""
        tipData1ml = Tip(float(self._m_Settings[TeslaPlatform.TravelOffset1mLTipLabel]),
                         float(self._m_Settings[TeslaPlatform.PickupPosition1mLTipLabel]) + self.RobotZOffset(),
                         float(self._m_Settings[TeslaPlatform.PrePickupPosition1mLTipLabel]) + self.RobotZOffset(),
                         float(self._m_Settings[TeslaPlatform.StripPosition1mLTipLabel]) + self.RobotZOffset(),
                         float(self._m_Settings[TeslaPlatform.StrippedPosition1mLTipLabel]) + self.RobotZOffset() )
        
        tipData5ml = Tip(float(self._m_Settings[TeslaPlatform.TravelOffset5mLTipLabel]),
                         float(self._m_Settings[TeslaPlatform.PickupPosition5mLTipLabel]) + self.RobotZOffset(),
                         float(self._m_Settings[TeslaPlatform.PrePickupPosition5mLTipLabel]) + self.RobotZOffset(),
                         float(self._m_Settings[TeslaPlatform.StripPosition5mLTipLabel]) + self.RobotZOffset(),
                         float(self._m_Settings[TeslaPlatform.StrippedPosition5mLTipLabel]) + self.RobotZOffset())

        TeslaPlatform.tipReferenceMap[1] = (TeslaPlatform.Tip1_1ml_Label, tipData1ml)       
        TeslaPlatform.tipReferenceMap[2] = (TeslaPlatform.Tip2_1ml_Label, tipData1ml)       
        TeslaPlatform.tipReferenceMap[3] = (TeslaPlatform.Tip3_1ml_Label, tipData1ml)       
        TeslaPlatform.tipReferenceMap[4] = (TeslaPlatform.Tip4_5ml_Label, tipData5ml)       
        TeslaPlatform.tipReferenceMap[5] = (TeslaPlatform.Tip5_5ml_Label, tipData5ml)       
                                                       
        TeslaPlatform.tipStripReferenceMap[1] = TeslaPlatform.StripTip1_1ml_Label      
        TeslaPlatform.tipStripReferenceMap[2] = TeslaPlatform.StripTip2_1ml_Label      
        TeslaPlatform.tipStripReferenceMap[3] = TeslaPlatform.StripTip3_1ml_Label      
        TeslaPlatform.tipStripReferenceMap[4] = TeslaPlatform.StripTip4_5ml_Label      
        TeslaPlatform.tipStripReferenceMap[5] = TeslaPlatform.StripTip5_5ml_Label      
                                                       
   
    def __SetPickupTipState (self,  isTrue):
        """Set the power profile in the Z-Axis to 'high', so that a tip may be picked up."""

        # Is the power setting defined? Ignore the call if it is not
        #
        powerProfile = None

        if isTrue and TeslaPlatform.PickupTipPowerProfileLabel in self._m_Settings.keys():
            powerProfile = self._m_Settings[TeslaPlatform.PickupTipPowerProfileLabel]

        velocityProfile = None

        if isTrue and TeslaPlatform.PickupTipVelocityProfileLabel in self._m_Settings.keys():
            velocityProfile = self._m_Settings[TeslaPlatform.PickupTipVelocityProfileLabel]

        self.__SetZPowerAndSpeed (powerProfile, velocityProfile)
            

    def __SetStripTipState (self, isTrue):
        """Set or reset the power and speed profile in the Z-Axis to strip or standard settings."""
        powerProfile = None

        if isTrue and TeslaPlatform.StripTipPowerProfileLabel in self._m_Settings.keys():
            powerProfile = self._m_Settings[TeslaPlatform.StripTipPowerProfileLabel]

        velocityProfile = None

        if isTrue and TeslaPlatform.StripTipVelocityProfileLabel in self._m_Settings.keys():
            velocityProfile = self._m_Settings[TeslaPlatform.StripTipVelocityProfileLabel]

        # Next two lines are debug code
        self.__SetZPowerAndSpeed (powerProfile, velocityProfile)
            

    def __SetZPowerAndSpeed (self, powerProfile = None, velocityProfile = None):
        """Set or reset the power and speed profile in the Z-Axis to pickup or standard settings."""
        # Either set or reset the power for the Z-Axis 
        zAxis = self.robot.Z()
        
        if powerProfile != None:
            zAxis.SetPowerFromString(powerProfile)
        else:
            zAxis.ResetPower()

        if velocityProfile != None:
            zAxis.SetStepSpeedProfileFromString(velocityProfile)
        else:
            zAxis.ResetStepSpeedProfile()

    
    def MoveToSectorAndTip(self, sector, tipNbr):
        '''Move to a specific sector and tip number. Note that this moves both 
        the carousel and the theta axis.
        This is mainly for testing purposes.'''
        if not tipNbr in TeslaPlatform.tipReferenceMap.keys():
            raise TeslaPlatformError ("Tip %d is not defined." % (tipNbr))

        referencePoint = TeslaPlatform.tipReferenceMap[tipNbr][0]
        self.MoveToPosition(sector, referencePoint)


    def getTipStripper(self):
        '''Return a reference to the TeslaPlatform's tip stripper instance.
        This is mainly for testing purposes.'''
        return self.tipStripper
    

    #GetInstrumentAxisStatusSet support methods
    def getCarouselAxis(self):
        return self.carousel._ThetaAxis()

# eof
