# Calibration.py
# tesla.instrument.Calibration
#
# Class to aid in hardware configuration settings during calibration
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
# 06/02/08 logging for calibration added - bdr

from ipl.utils.wait import wait_msecs
from tesla.exception import TeslaException
import tesla.config
from tesla.hardware.config import gHardwareData, LoadSettings, ReloadHardwareData, GetHardwareData
from tesla.hardware.Device import Device
from tesla.instrument.Subsystem import Subsystem
from tesla.types.ReferencePoint import ReferencePoint

import math
import logging
import logging.handlers
import tesla.AdvMath
from tesla.instrument.TeslaPlatform import TeslaPlatform

from tesla.PgmLog import PgmLog    # 2011-11-28 sp -- programming logging
import os                          # 2011-11-28 sp -- programming logging


# logging limits
ROLLOVER_SIZE = 104857600L  # roll the file over @ 100MB
MAX_BACKUPS = 9999




# ---------------------------------------------------------------------------

class Calibration(Subsystem):
    """Class to aid in hardware configuration settings during calibration"""

    # Configuration data    
    PlatformLabel = "Platform"
    ReferencePointsLabel = "ReferencePoints"
    CalibrationLabel = "Calibration"
    CalibrationJigSlotLabel = "calibrationjigslot_degrees"
    Tesla = "Tesla"


    # Inputs to the hardware settings calculations
    
    ParticleVialOffsetLabel = "particlevialoffset_degrees"
    CocktailVialOffsetLabel = "cocktailvialoffset_degrees"
    AntibodyVialOffsetLabel = "antibodyvialoffset_degrees"
    Tip1OffsetLabel = "tip1offset_degrees"
    Tip2OffsetLabel = "tip2offset_degrees"
    Tip3OffsetLabel = "tip3offset_degrees"
    Tip4OffsetLabel = "tip4offset_degrees"
    Tip5OffsetLabel = "tip5offset_degrees"
    SampleVialOffsetLabel = "samplevialoffset_degrees"
    SeparationVialOffsetLabel = "separationvialoffset_degrees"
    LysisVialOffsetLabel = "lysisvialoffset_degrees"
    WasteVialOffsetLabel = "wastevialoffset_degrees"
    BaseOffsetAntibodyVialLabel = "calbaseoffset_antibodyvial_mm"
    BaseOffsetCoctailVialLabel = "calbaseoffset_cocktailvial_mm"
    BaseOffsetBeadVialLabel = "calbaseoffset_beadvial_mm"
    BaseOffset14mlVialLabel = "calbaseoffset_14mlvial_mm"
    BaseOffset50mlVialLabel  = "calbaseoffset_50mlvial_mm"
    CalZPrePickup1mlLabel  = "calz_prepickupposition_1mltip_mm"
    CalZPickup1mlLabel= "calz_pickupposition_1mltip_mm"
    CalZStripped1mlLabel = "calz_strippedpositionoffset_1mltip_mm"
    CalZPrePickup5mlLabel = "calz_prepickupposition_5mltip_mm"
    CalZPickup5mlLabel = "calz_pickupposition_5mltip_mm"
    CalZStripped5mlLabel = "calz_strippedpositionoffset_5mltip_mm"

    TravelOffsetNoTipLabel = 'z_traveloffset_notip_mm'    
    TravelOffset1mLTipLabel = 'z_traveloffset_1mltip_mm'    
    TravelOffset5mLTipLabel = 'z_traveloffset_5mltip_mm'
    MinTipBaseGapLabel = "minimumallowedvialbasetotipclearance_mm"

    CarouselThetaOffsetLabel = 'carouselreferencepointoffset_degrees'

    #NEW Calibration constants
    ThetaToRealDegLabel = 'thetatorealdeg'
    ArmLengthLabel = 'armlength_mm'
    ToolToJigZLabel = 'tooltojigz_mm'
    Tip1mlStripToolZLabel = '1mlstriptoolz_mm'
    Tip5mlStripToolZLabel = '5mlstriptoolz_mm'
    BccmToolZLabel = 'bccmtoolz_mm'

    __configData = {
        CalibrationJigSlotLabel      : '-24',
        ParticleVialOffsetLabel      : '-10',
        CocktailVialOffsetLabel      : '-10',
        AntibodyVialOffsetLabel      : '-10',
        Tip1OffsetLabel              : '-10',
        Tip2OffsetLabel              : '-10',
        Tip3OffsetLabel              : '-10',
        Tip4OffsetLabel              : '-10',
        Tip5OffsetLabel              : '-10',
        SampleVialOffsetLabel        : '-10',
        SeparationVialOffsetLabel    : '-10',
        LysisVialOffsetLabel         : '-10',
        WasteVialOffsetLabel         : '-10',        
        BaseOffsetAntibodyVialLabel  : '-10',
        BaseOffsetCoctailVialLabel   : '-10',
        BaseOffsetBeadVialLabel      : '-10',
        BaseOffset14mlVialLabel      : '-10',
        BaseOffset50mlVialLabel      : '-10',
        CalZPrePickup1mlLabel        : '-10',
        CalZPickup1mlLabel           : '-10',
        CalZStripped1mlLabel         : '-5',
        CalZPrePickup5mlLabel        : '-10',
        CalZPickup5mlLabel           : '-10',
        CalZStripped5mlLabel         : '-6',

        TravelOffsetNoTipLabel       : '0',
        TravelOffset1mLTipLabel      : '-92.5',
        TravelOffset5mLTipLabel      : '-110.75',
        MinTipBaseGapLabel           : '2'
        }
    

    # ---------------------------------------------------------------------------
    
    def __init__( self, name, instrument ):
        """Initialise the Calibration class"""
        global gHardwareData
        Subsystem.__init__( self, name )

        self.initLogging()  
        self.m_settings = Calibration.__configData.copy()

        # 2011-11-28 -- sp
        self.svrLog = PgmLog( 'svrLog' )
        self.logPrefix = ''
        
        #RL extra settings for new z cal
        platformConfigData = gHardwareData.Section ( Calibration.PlatformLabel )
        LoadSettings( self.m_settings, platformConfigData )
        
        teslaConfigData = gHardwareData.Section ( Calibration.Tesla )
        LoadSettings( self.m_settings, teslaConfigData )

        # Get the [Calibration] settings
        calibrationConfigData = gHardwareData.Section ( Calibration.CalibrationLabel )
        LoadSettings( self.m_settings, calibrationConfigData )

        # Create a local reference to the hardware config object.
        # This attribute will be used to manipulate the hardware config
        self.m_hardwareConfig = GetHardwareData() 

        self.m_instrument = instrument
        self.m_platform = self.m_instrument.getPlatform()
        self.m_zAxis = self.m_platform.robot.getZAxis()
        self.m_thetaAxis = self.m_platform.robot.getThetaAxis()
        self.m_carouselAxis = self.m_platform.carousel

        # intermediate variables for calculation of settings
        self.m_jigZHeight = None
        self.m_1mlTipAngle = None
        self.m_5mlTipAngle = None
        self.m_sampleVialAngle = None
        self.m_reagentVialAngle = None
        self.m_lysisVialAngle = None
        self.m_separationVialAngle = None
        self.m_wasteVialAngle = None
        self.m_jigCarouselAngle1 = None
        self.m_jigCarouselAngle2 = None
        self.m_jigCarouselAngle1_v2 = None
        self.m_jigCarouselAngle2_v2 = None
        self.m_1mlStripTipAngle = None
        self.m_5mlStripTipAngle = None
        self.m_bccmAngle = None
        self.m_stripZHeight1ml = None
        self.m_stripZHeight5ml = None
        self.m_bccmZHeight = None

        self.m_smallVialTipHeight = None
        self.m_sampleTubeTipHeight = None
       
        self.m_sampleVialRef = None
        self.m_sampleVial2Ref = None
        self.m_1mlTipStripRef = None
        self.m_5mlTipStripRef = None
        self.m_bccmRef = None
        
        #to fix mid script file write problem
        self.m_tip_1ml_position1_degrees = None
        self.m_tip_1ml_position2_degrees = None
        self.m_tip_1ml_position3_degrees = None
        self.m_tip_5ml_position4_degrees = None
        self.m_tip_5ml_position5_degrees = None
        self.m_antibodyvial_degrees = None
        self.m_cocktailvial_degrees = None
        self.m_particlevial_degrees = None
        self.m_samplevial_14ml_degrees = None
        self.m_separationvial_14ml_degrees = None
        self.m_smallvial_tip_height = None
        self.m_14ml_tube_tip_height = None
        self.m_50ml_tube_tip_height = None

        self.m_barcode_offset_degrees = None

        self.m_particleVialOffset = self.m_settings[self.ParticleVialOffsetLabel]
        self.m_cocktailVialOffset = self.m_settings[self.CocktailVialOffsetLabel]
        self.m_antibodyVialOffset = self.m_settings[self.AntibodyVialOffsetLabel]

        self.m_tip1ThetaOffset = self.m_settings[self.Tip1OffsetLabel]
        self.m_tip2ThetaOffset = self.m_settings[self.Tip2OffsetLabel]
        self.m_tip3ThetaOffset = self.m_settings[self.Tip3OffsetLabel]
        self.m_tip4ThetaOffset = self.m_settings[self.Tip4OffsetLabel]
        self.m_tip5ThetaOffset = self.m_settings[self.Tip5OffsetLabel]

        self.m_sampleVialOffset = self.m_settings[self.SampleVialOffsetLabel]
        self.m_separationVialOffset = self.m_settings[self.SeparationVialOffsetLabel]
        self.m_lysisVialOffset = self.m_settings[self.LysisVialOffsetLabel]
        self.m_wasteVialOffset = self.m_settings[self.WasteVialOffsetLabel]

        self.m_baseOffsetAntibodyVial = self.m_settings[self.BaseOffsetAntibodyVialLabel] 
        self.m_baseOffsetCocktailVial = self.m_settings[self.BaseOffsetCoctailVialLabel]
        self.m_baseOffsetBeadVial = self.m_settings[self.BaseOffsetBeadVialLabel]
        self.m_baseOffset14mlVial = self.m_settings[self.BaseOffset14mlVialLabel]
        self.m_baseOffset50mlVial = self.m_settings[self.BaseOffset50mlVialLabel]
        self.m_calZPrePickup1ml = self.m_settings[self.CalZPrePickup1mlLabel]
        self.m_calZPickup1ml = self.m_settings[self.CalZPickup1mlLabel]
        self.m_calZStrippedOffset1ml = self.m_settings[self.CalZStripped1mlLabel]
        self.m_calZPrePickup5ml = self.m_settings[self.CalZPrePickup5mlLabel]
        self.m_calZPickup5ml = self.m_settings[self.CalZPickup5mlLabel]
        self.m_calZStrippedOffset5ml = self.m_settings[self.CalZStripped5mlLabel]

        
        self.m_travelOffsetNoTipLabel   = self.m_settings[self.TravelOffsetNoTipLabel]
        self.m_travelOffset1mLTipLabel  = self.m_settings[self.TravelOffset1mLTipLabel]
        self.m_travelOffset5mLTipLabel  = self.m_settings[self.TravelOffset5mLTipLabel]
        self.m_minTipBaseGapLabel = self.m_settings[self.MinTipBaseGapLabel]
        
        self.m_centerline =  float(self.m_hardwareConfig.Item( self.m_platform.SectionName,
                                        self.m_platform.CarouselThetaOffsetLabel))
        self.thetaToRealDeg =1
        self.armLength = 0
        self.toolToJigZ = 0
        self.tip1mlStripToolZ = 0
        self.tip5mlStripToolZ = 0
        self.bccmToolZ = 0

        
    # ---------------------------------------------------------------------------

    ### Start calibration method
    
    def startCalibration( self ):
        """Resets the calibration class internal variables"""
        self.logger.info("Starting Calibration Procedure")
        funcReference = __name__ + '.startCalibration'              # 2011-11-28 sp -- added logging
        self.svrLog.logID('xx', self.logPrefix, funcReference, 'startCalibration')    # 2011-11-28 sp -- added logging; logging not tested
        self.m_jigZHeight = None
        self.m_1mlTipAngle = None
        self.m_5mlTipAngle = None
        self.m_sampleVialAngle = None
        self.m_reagentVialAngle = None
        self.m_lysisVialAngle = None
        self.m_separationVialAngle = None
        self.m_wasteVialAngle = None
        self.m_jigCarouselAngle1 = None
        self.m_jigCarouselAngle2 = None
        self.m_jigCarouselAngle1_v2 = None
        self.m_jigCarouselAngle2_v2 = None
        self.m_1mlStripTipAngle = None
        self.m_5mlStripTipAngle = None
        self.m_bccmAngle = None
        self.m_stripZHeight1ml = None
        self.m_stripZHeight5ml = None
        self.m_bccmHeight = None

        self.m_smallVialTipHeight = None
        self.m_sampleTubeTipHeight = None
        
        self.m_sampleVialRef = None
        self.m_sampleVial2Ref = None
        self.m_1mlTipStripRef = None
        self.m_5mlTipStripRef = None
        self.m_bccmRef = None


        #to fix mid script file write problem
        self.m_tip_1ml_position1_degrees = None
        self.m_tip_1ml_position2_degrees = None
        self.m_tip_1ml_position3_degrees = None
        self.m_tip_5ml_position4_degrees = None
        self.m_tip_5ml_position5_degrees = None
        self.m_antibodyvial_degrees = None
        self.m_cocktailvial_degrees = None
        self.m_particlevial_degrees = None
        self.m_samplevial_14ml_degrees = None
        self.m_separationvial_14ml_degrees = None
        self.m_smallvial_tip_height = None
        self.m_14ml_tube_tip_height = None
        self.m_50ml_tube_tip_height = None

        self.m_barcode_offset_degrees = None


    def initLogging( self ):
        """Creates a log specifically for calibration"""
        self.logger = logging.getLogger('robosep.calibration')
        self.logger.setLevel(logging.INFO)
        hdl = logging.handlers.RotatingFileHandler(tesla.config.CAL_LOG_PATH,
              mode = 'a', maxBytes = ROLLOVER_SIZE, 
              backupCount = MAX_BACKUPS)
        hdl.setLevel(logging.DEBUG)
        formatter = logging.Formatter('%(asctime)s:\t %(message)s','%B %d %H:%M:%S')
        hdl.setFormatter(formatter)
        self.logger.addHandler(hdl)

        
        
    ### Robot z calibration to jig
    
    def calibrateZHeights( self ):
        """Stores the current z height to temp var m_jigZHeight"""
        # Get the current z position
        self.m_jigZHeight = self.m_zAxis.Position()        
        print '#####!!!!!! Called calibrateZHeight %f### '%self.m_jigZHeight
        
    def calibrateSetSmallVialTipHeight (self):
        """Calculates tiphead Z for small vials var m_jigZHeight - about 192mm (travel -92mm, gap 2mm)"""
        # height = self.m_zAxis.Position() + self.sampleRefToCarouselZ + self.maxCarouselToVialZ
        # print "calibrateSetSmallVialTipHeight height=", height
        # print "calibrateSetSmallVialTipHeight zaxispsn = ", self.m_zAxis.Position()
        
        height = self.m_zAxis.Position() - float(self.m_travelOffset1mLTipLabel) + float(self.m_minTipBaseGapLabel)

        self.m_hardwareConfig.writeItem( self.m_instrument.TeslaLabel,
                                        self.m_instrument.BaseAntibodyLabel,
                                        height )
        self.m_hardwareConfig.writeItem( self.m_instrument.TeslaLabel,
                                        self.m_instrument.BaseCocktailLabel,
                                        height )
        self.m_hardwareConfig.writeItem( self.m_instrument.TeslaLabel,
                                        self.m_instrument.BaseBeadLabel,
                                        height )
        
    def calibrateSet14mLTubeTipHeight (self):
        height = self.m_zAxis.Position() - float(self.m_travelOffset5mLTipLabel) + float(self.m_minTipBaseGapLabel)
        print "calibrateSet14mLTubeTipHeight", height
        self.m_hardwareConfig.writeItem( self.m_instrument.TeslaLabel,
                                        self.m_instrument.Base14mlLabel,
                                        height )
    def calibrateSet50mLTubeTipHeight (self):
        height = self.m_zAxis.Position() - float(self.m_travelOffset5mLTipLabel) + float(self.m_minTipBaseGapLabel)
        print "calibrateSet50mLTubeTipHeight", height
        self.m_hardwareConfig.writeItem( self.m_instrument.TeslaLabel,
                                        self.m_instrument.Base50mlLabel,
                                        height )
        
    def calibrateReverseEngJigHeight (self):
        tmpJigHeight1 = self.m_sampleTubeTipHeight - float(self.m_travelOffset5mLTipLabel) + float(self.m_minTipBaseGapLabel) - float(self.m_baseOffset14mlVial)
        tmpJigHeight2 = self.m_smallVialTipHeight - float(self.m_travelOffset1mLTipLabel)  + float(self.m_minTipBaseGapLabel) - float(self.m_baseOffsetCocktailVial)
        tmpJigHeightAVG = (tmpJigHeight1 + tmpJigHeight2) / 2
        print "New Z Cal!!!",self.m_sampleTubeTipHeight , float(self.m_baseOffset14mlVial)
        print "New Z Cal!!!",self.m_smallVialTipHeight , float(self.m_baseOffsetCocktailVial)
        print "New Z Cal!!!",tmpJigHeight1, tmpJigHeight2, \
              " ==>", tmpJigHeightAVG
        #don't need this any more
        #self.m_jigZHeight = tmpJigHeightAVG

    ### Robot theta calibration to jig lines 

    def calibrate1mlTipAngle( self ):
        """Store the current theta angle to temp var m_1mlTipAngle"""
        self.m_1mlTipAngle = self.m_thetaAxis.Theta()

    def calibrate5mlTipAngle( self ):
        """Store the current theta angle to temp var m_5mlTipAngle"""
        self.m_5mlTipAngle = self.m_thetaAxis.Theta()

    def calibrateSampleVialAngle( self ):
        """Store the current theta angle to temp var m_sampleVialAngle"""
        self.m_sampleVialAngle = self.m_thetaAxis.Theta()

    def calibrateReagentVialAngle( self ):
        """Store the current theta angle to temp var m_reagentVialAngle"""
        self.m_reagentVialAngle = self.m_thetaAxis.Theta()

    def calibrateLysisVialAngle( self ):
        """Store the current theta angle to temp var m_lysisVialAngle"""
        self.m_lysisVialAngle = self.m_thetaAxis.Theta()

    def calibrateSeparationVialAngle( self ):
        """Store the current theta angle to temp var m_separationVialAngle"""
        self.m_separationVialAngle = self.m_thetaAxis.Theta()

    def calibrateWasteVialAngle( self ):
        """Store the current theta angle to temp var m_wasteVialAngle"""
        self.m_wasteVialAngle = self.m_thetaAxis.Theta()


    ### Carousel calibration
    
    def calibrateCarouselAngle1( self ):
        """Store the current carousel angle to temp var m_jigCarouselAngle1"""        
        self.m_jigCarouselAngle1 = self.m_carouselAxis.Theta()

    def calibrateCarouselAngle2( self ):
        """Store the current carousel angle to temp var m_jigCarouselAngle2"""
        self.m_jigCarouselAngle2 = self.m_carouselAxis.Theta()

    def calibrateCarouselAngle1_v2( self ):
        self.m_jigCarouselAngle1_v2 = self.m_carouselAxis.Theta()
    
    def calibrateCarouselAngle2_v2( self ):
        self.m_jigCarouselAngle2_v2 = self.m_carouselAxis.Theta()

    ### Robot theta calibration to BCCM
        
    def calibrateBCCMPosition( self ):
        """Store the current theta and z positions to temp vars m_bccmAngle
        and m_bccmHeight"""
        self.m_bccmAngle = self.m_thetaAxis.Theta()
        self.m_bccmZHeight = self.m_zAxis.Position()

    ### Robot theta to tip stripper calibration

    def calibrate1mlTipStripPosition( self ):
        """Store the current theta and z positions to temp vars
        m_1mlStripTipAngle and m_stripZHeight1ml"""
        self.m_1mlStripTipAngle = self.m_thetaAxis.Theta()
        self.m_stripZHeight1ml = self.m_zAxis.Position()

    def calibrate5mlTipStripPosition( self ):
        """Store the current theta and z positions to temp vars
        m_5mlStripTipAngle and m_stripZHeight5ml"""
        self.m_5mlStripTipAngle = self.m_thetaAxis.Theta()
        self.m_stripZHeight5ml = self.m_zAxis.Position()

    #to fix mid script file write problem
    ### set temp variables for adjustment scripts

    def getTupleString( self ):
        return self.convertTupleToString(( self.m_thetaAxis.Theta(), self.getCarouselThetaOffset() ))
    def calibrate1mlPosition1Degrees( self ):
        self.m_tip_1ml_position1_degrees = self.getTupleString()
    def calibrate1mlPosition2Degrees( self ):
        self.m_tip_1ml_position2_degrees = self.getTupleString()
    def calibrate1mlPosition3Degrees( self ):
        self.m_tip_1ml_position3_degrees = self.getTupleString()
    def calibrate5mlPosition4Degrees( self ):
        self.m_tip_5ml_position4_degrees = self.getTupleString()
    def calibrate5mlPosition5Degrees( self ):
        self.m_tip_5ml_position5_degrees = self.getTupleString()
    def calibrateAntibodyVialDegrees( self ):
        self.m_antibodyvial_degrees = self.getTupleString()
    def calibrateCocktailVialDegrees( self ):
        self.m_cocktailvial_degrees = self.getTupleString()
    def calibrateParticleVialDegrees( self ):
        self.m_particlevial_degrees = self.getTupleString()
    def calibrateSampleVialDegrees( self ):
        self.m_samplevial_14ml_degrees = self.getTupleString()
    def calibrateSeparationVialDegrees( self ):
        self.m_separationvial_14ml_degrees = self.getTupleString()
    def calibrateSmallVialTipHeight( self ):
        self.m_smallvial_tip_height = self.m_zAxis.Position() - float(self.m_travelOffset1mLTipLabel) + float(self.m_minTipBaseGapLabel)
    def calibrate14mLTubeTipHeight( self ):
        self.m_14ml_tube_tip_height = self.m_zAxis.Position() - float(self.m_travelOffset5mLTipLabel) + float(self.m_minTipBaseGapLabel)
    def calibrate50mLTubeTipHeight( self ):
        self.m_50ml_tube_tip_height = self.m_zAxis.Position() - float(self.m_travelOffset5mLTipLabel) + float(self.m_minTipBaseGapLabel)

    def calibrateBarcode( self ):
        self.m_barcode_offset_degrees = self.m_carouselAxis.Theta() #self.getCarouselThetaOffset()
    def calibrateBarcodeToVial( self, vial ):
        (sector, location) = self.m_instrument._LocationOf(vial) 
        (rTheta, cTheta) = self.m_platform._ObtainRendezvous (sector, location, 0)
        self.m_barcode_offset_degrees = self.m_carouselAxis.Theta() - cTheta
        if self.m_barcode_offset_degrees > 360:
            self.m_barcode_offset_degrees -=360
        if self.m_barcode_offset_degrees < -360:
            self.m_barcode_offset_degrees +=360
        print "----------calibrateBarcodeToVial:",self.m_barcode_offset_degrees,self.m_carouselAxis.Theta(),cTheta
    
    def ContainerAt (self, sector, name):
        return self.m_instrument.ContainerAt(sector,name)

    ### Service helper methods

    def wait( self, delay_ms ):
        """Waits for delay_ms milliseconds"""
        wait_msecs( delay_ms )

    def setCalibration( self, section, key, value ):
        """Generic set calibration value method"""
        self.m_hardwareConfig.writeItem( section, key, value )

    def setCalibrationAngles( self, section, key ):
        """Generic set calibration value method. Stores 'robot_theta, carousel_theta' pair,
        using the current axis positions."""
        s = self.convertTupleToString(( self.m_thetaAxis.Theta(), self.getCarouselThetaOffset() ))
        self.m_hardwareConfig.writeItem( section, key, s )

    def setCalibrationZ( self, section, key ):
        """Generic set calibration value method. Stores current z axis position."""
        s = self.m_zAxis.Position()
        self.m_hardwareConfig.writeItem( section, key, s )
        
    def getCalibration( self, section, key ):
        """Genertic get calibration value method"""
        return self.m_hardwareConfig.Item( section, key )

    def getCarouselThetaOffset( self ):
        '''Get the carousel theta offset'''
        return ( self.m_carouselAxis.Theta() \
                 - float( self.m_hardwareConfig.Item( self.m_platform.SectionName, \
                                                      self.m_platform.CarouselThetaOffsetLabel ) ) \
                 + 90.0 )

    def setCalibrationReferencePoint( self, key, theta, carousel ):
        """Generic set calibration value method. Stores 'robot_theta, carousel_theta' pair,
        using the current axis positions."""
        s = self.convertTupleToString(( theta, carousel ))
        print "setCalibrationReferencePoint:",key,theta,carousel
        self.m_hardwareConfig.writeItem( Calibration.ReferencePointsLabel, key, s )
    
    ### End calibration method

    def endCalibration( self ):
        '''Do final calibration calculations (if required) and write the
        results into the hardware configuration file.'''
        
        print '#### Enter endCalibration ####'
        # Work out centreline of jig and store to hardware config
        if None != self.m_jigCarouselAngle1 and None != self.m_jigCarouselAngle2:
            centreline = ( self.m_jigCarouselAngle1 + self.m_jigCarouselAngle2 ) / 2. + 90.
            self.m_hardwareConfig.writeItem( self.m_platform.SectionName,
                                        self.m_platform.CarouselThetaOffsetLabel,
                                        centreline )
            print '#### calibrate CarouselThetaOffsetLabel v1  %f ####' % (centreline )

        if None != self.m_jigCarouselAngle1_v2 and None != self.m_jigCarouselAngle2_v2:
            centreline = ( self.m_jigCarouselAngle1_v2 + self.m_jigCarouselAngle2_v2 ) / 2. + 90 +90.
            self.m_hardwareConfig.writeItem( self.m_platform.SectionName,
                                        self.m_platform.CarouselThetaOffsetLabel,
                                        centreline )
            print '#### calibrate CarouselThetaOffsetLabel v2 %f ####' % (centreline )

        # Store the reference points to hardware config
        if None != self.m_reagentVialAngle:
            s = self.convertTupleToString(( self.m_reagentVialAngle, float( self.m_particleVialOffset )))
            self.m_hardwareConfig.writeItem( self.ReferencePointsLabel,
                                        self.m_platform.ParticleVial_Label, s )
            s = self.convertTupleToString(( self.m_reagentVialAngle, float( self.m_cocktailVialOffset )))
            self.m_hardwareConfig.writeItem( self.ReferencePointsLabel,
                                        self.m_platform.CocktailVial_Label, s )
            s = self.convertTupleToString(( self.m_reagentVialAngle, float( self.m_antibodyVialOffset )))
            self.m_hardwareConfig.writeItem( self.ReferencePointsLabel,
                                        self.m_platform.AntibodyVial_Label, s )

        if None != self.m_1mlTipAngle: 
            s = self.convertTupleToString(( self.m_1mlTipAngle, float( self.m_tip1ThetaOffset )))
            self.m_hardwareConfig.writeItem( self.ReferencePointsLabel,
                                        self.m_platform.Tip1_1ml_Label, s )
            s = self.convertTupleToString(( self.m_1mlTipAngle, float( self.m_tip2ThetaOffset )))
            self.m_hardwareConfig.writeItem( self.ReferencePointsLabel,
                                        self.m_platform.Tip2_1ml_Label, s )
            s = self.convertTupleToString(( self.m_1mlTipAngle, float( self.m_tip3ThetaOffset )))
            self.m_hardwareConfig.writeItem( self.ReferencePointsLabel,
                                        self.m_platform.Tip3_1ml_Label, s )
            
        if None != self.m_5mlTipAngle: 
            s = self.convertTupleToString(( self.m_5mlTipAngle, float( self.m_tip4ThetaOffset )))
            self.m_hardwareConfig.writeItem( self.ReferencePointsLabel,
                                        self.m_platform.Tip4_5ml_Label, s )
            s = self.convertTupleToString(( self.m_5mlTipAngle, float( self.m_tip5ThetaOffset )))
            self.m_hardwareConfig.writeItem( self.ReferencePointsLabel,
                                        self.m_platform.Tip5_5ml_Label, s )

        if None != self.m_1mlStripTipAngle: 
            print '\t###Enter 1mL Strip cal!'
            s = self.convertTupleToString(( self.m_1mlStripTipAngle, float( self.m_tip1ThetaOffset )))
            self.m_hardwareConfig.writeItem( self.ReferencePointsLabel,
                                        self.m_platform.StripTip1_1ml_Label, s )
            s = self.convertTupleToString(( self.m_1mlStripTipAngle, float( self.m_tip2ThetaOffset )))
            self.m_hardwareConfig.writeItem( self.ReferencePointsLabel,
                                        self.m_platform.StripTip2_1ml_Label, s )
            s = self.convertTupleToString(( self.m_1mlStripTipAngle, float( self.m_tip3ThetaOffset )))
            self.m_hardwareConfig.writeItem( self.ReferencePointsLabel,
                                        self.m_platform.StripTip3_1ml_Label, s )
            height = float( self.m_stripZHeight1ml )
            self.m_hardwareConfig.writeItem( self.m_platform.SectionName,
                                        self.m_platform.StripPosition1mLTipLabel,
                                        height )
            height = self.m_stripZHeight1ml + float( self.m_calZStrippedOffset1ml )
            self.m_hardwareConfig.writeItem( self.m_platform.SectionName,
                                        self.m_platform.StrippedPosition1mLTipLabel,
                                        height )

        if None != self.m_5mlStripTipAngle: 
            print '\t###Enter 5mL Strip cal!'        
            s = self.convertTupleToString(( self.m_5mlStripTipAngle, float( self.m_tip4ThetaOffset )))
            self.m_hardwareConfig.writeItem( self.ReferencePointsLabel,
                                        self.m_platform.StripTip4_5ml_Label, s )
            s = self.convertTupleToString(( self.m_5mlStripTipAngle, float( self.m_tip5ThetaOffset )))
            self.m_hardwareConfig.writeItem( self.ReferencePointsLabel,
                                        self.m_platform.StripTip5_5ml_Label, s )
            height = float( self.m_stripZHeight5ml )
            self.m_hardwareConfig.writeItem( self.m_platform.SectionName,
                                        self.m_platform.StripPosition5mLTipLabel,
                                        height )
            height = self.m_stripZHeight5ml + float( self.m_calZStrippedOffset5ml )
            self.m_hardwareConfig.writeItem( self.m_platform.SectionName,
                                        self.m_platform.StrippedPosition5mLTipLabel,
                                        height )

        if None != self.m_sampleVialAngle:
            s = self.convertTupleToString(( self.m_sampleVialAngle, float( self.m_sampleVialOffset )))
            self.m_hardwareConfig.writeItem( self.ReferencePointsLabel,
                                        self.m_platform.Sample_Label, s )
            
        if None != self.m_lysisVialAngle:
            s = self.convertTupleToString(( self.m_lysisVialAngle, float( self.m_lysisVialOffset )))
            self.m_hardwareConfig.writeItem( self.ReferencePointsLabel,
                                        self.m_platform.LysisBuffer_Label, s )

        if None != self.m_separationVialAngle:
            s = self.convertTupleToString(( self.m_separationVialAngle, float( self.m_separationVialOffset )))
            self.m_hardwareConfig.writeItem( self.ReferencePointsLabel,
                                        self.m_platform.Separation_Label, s )

        if None != self.m_wasteVialAngle:
            s = self.convertTupleToString(( self.m_wasteVialAngle, float( self.m_wasteVialOffset )))
            self.m_hardwareConfig.writeItem( self.ReferencePointsLabel,
                                        self.m_platform.Waste_Label, s )

        if None != self.m_bccmAngle:
            self.m_hardwareConfig.writeItem( self.ReferencePointsLabel,
                                        self.m_platform.BulkLabel,
                                        self.m_bccmAngle )

            height = float( self.m_bccmZHeight ) - \
                     float( self.m_platform._m_Settings[self.m_platform.TravelOffset5mLTipLabel] )
            self.m_hardwareConfig.writeItem( self.m_instrument.TeslaLabel,
                                        self.m_instrument.BaseBCCMLabel,
                                        height )

        # Calculate all Z heights from the jig height
        if None != self.m_jigZHeight:
            print "\t\tEnter m_jigZHeight!!!"

            # antibody vial base
            # new height = self.m_zAxis.Position() + self.sampleRefToCarouselZ + self.maxCarouselToVialZ
            # print "endCalibration height=", height
            # print "endCalibration  zaxispsn = ", self.m_zAxis.Position()
            
            height = float(self.m_baseOffsetAntibodyVial) + self.m_jigZHeight
            self.m_hardwareConfig.writeItem( self.m_instrument.TeslaLabel,
                                        self.m_instrument.BaseAntibodyLabel,
                                        height )

            # cocktail vial base
            height = float(self.m_baseOffsetCocktailVial) + self.m_jigZHeight
            self.m_hardwareConfig.writeItem( self.m_instrument.TeslaLabel,
                                        self.m_instrument.BaseCocktailLabel,
                                        height )
        
            # bead vial base
            height = float(self.m_baseOffsetBeadVial) + self.m_jigZHeight
            self.m_hardwareConfig.writeItem( self.m_instrument.TeslaLabel,
                                        self.m_instrument.BaseBeadLabel,
                                        height )
            
            # 14 ml tube base
            height = float(self.m_baseOffset14mlVial) + self.m_jigZHeight
            self.m_hardwareConfig.writeItem( self.m_instrument.TeslaLabel,
                                        self.m_instrument.Base14mlLabel,
                                        height )

            # 50 ml tube base
            height = float(self.m_baseOffset50mlVial) + self.m_jigZHeight
            self.m_hardwareConfig.writeItem( self.m_instrument.TeslaLabel,
                                        self.m_instrument.Base50mlLabel,
                                        height )

            # 1 ml prepickup
            print '\t###Enter 1mL pickup cal!'                                
            height = float(self.m_calZPrePickup1ml) + self.m_jigZHeight
            self.m_hardwareConfig.writeItem( self.m_platform.SectionName,
                                        self.m_platform.PrePickupPosition1mLTipLabel,
                                        height )

            # 5 ml prepickup
            print '\t###Enter 5mL pickup cal!'                    
            height = float(self.m_calZPrePickup5ml) + self.m_jigZHeight
            self.m_hardwareConfig.writeItem( self.m_platform.SectionName,
                                        self.m_platform.PrePickupPosition5mLTipLabel,
                                        height )

            # 1 ml pickup
            height = float(self.m_calZPickup1ml) + self.m_jigZHeight
            self.m_hardwareConfig.writeItem( self.m_platform.SectionName,
                                        self.m_platform.PickupPosition1mLTipLabel,
                                        height )

            # 5 ml prepickup
            height = float(self.m_calZPickup5ml) + self.m_jigZHeight
            self.m_hardwareConfig.writeItem( self.m_platform.SectionName,
                                        self.m_platform.PickupPosition5mLTipLabel,
                                        height )
        

        #to fix mid script file write problem
        if None != self.m_tip_1ml_position1_degrees:
            self.m_hardwareConfig.writeItem( 'ReferencePoints', 'tip_1ml_position1_degrees', self.m_tip_1ml_position1_degrees )
        if None != self.m_tip_1ml_position2_degrees:
            self.m_hardwareConfig.writeItem( 'ReferencePoints', 'tip_1ml_position2_degrees', self.m_tip_1ml_position2_degrees )
        if None != self.m_tip_1ml_position3_degrees:
            self.m_hardwareConfig.writeItem( 'ReferencePoints', 'tip_1ml_position3_degrees', self.m_tip_1ml_position3_degrees )
        if None != self.m_tip_5ml_position4_degrees:
            self.m_hardwareConfig.writeItem( 'ReferencePoints', 'tip_5ml_position4_degrees', self.m_tip_5ml_position4_degrees )
        if None != self.m_tip_5ml_position5_degrees:
            self.m_hardwareConfig.writeItem( 'ReferencePoints', 'tip_5ml_position5_degrees', self.m_tip_5ml_position5_degrees )

        if None != self.m_antibodyvial_degrees:
            self.m_hardwareConfig.writeItem( 'ReferencePoints', 'antibodyvial_degrees', self.m_antibodyvial_degrees )
        if None != self.m_cocktailvial_degrees:
            self.m_hardwareConfig.writeItem( 'ReferencePoints', 'cocktailvial_degrees', self.m_cocktailvial_degrees )
        if None != self.m_particlevial_degrees:
            self.m_hardwareConfig.writeItem( 'ReferencePoints', 'particlevial_degrees', self.m_particlevial_degrees )
        if None != self.m_samplevial_14ml_degrees:
            self.m_hardwareConfig.writeItem( 'ReferencePoints', 'samplevial_14ml_degrees', self.m_samplevial_14ml_degrees )
        if None != self.m_separationvial_14ml_degrees:
            self.m_hardwareConfig.writeItem( 'ReferencePoints', 'separationvial_14ml_degrees', self.m_separationvial_14ml_degrees )

        if None != self.m_barcode_offset_degrees:
            print '#### m_barcode_offset_degrees ####', self.m_barcode_offset_degrees
            self.m_hardwareConfig.writeItem( 'Barcode_reader', 'barcode_offset_degrees', self.m_barcode_offset_degrees )

        if None != self.m_smallvial_tip_height:
            height = self.m_smallvial_tip_height
            self.m_hardwareConfig.writeItem( self.m_instrument.TeslaLabel,
                                            self.m_instrument.BaseAntibodyLabel,
                                            height )
            self.m_hardwareConfig.writeItem( self.m_instrument.TeslaLabel,
                                            self.m_instrument.BaseCocktailLabel,
                                            height )
            self.m_hardwareConfig.writeItem( self.m_instrument.TeslaLabel,
                                            self.m_instrument.BaseBeadLabel,
                                            height )
        if None != self.m_14ml_tube_tip_height:
            height = self.m_14ml_tube_tip_height
            self.m_hardwareConfig.writeItem( self.m_instrument.TeslaLabel,
                                            self.m_instrument.Base14mlLabel,
                                            height )
        if None != self.m_50ml_tube_tip_height:
            height = self.m_50ml_tube_tip_height
            self.m_hardwareConfig.writeItem( self.m_instrument.TeslaLabel,
                                            self.m_instrument.Base50mlLabel,
                                            height )

        # Dump the hardware config to file and reload the settings
# 2012-03-05 RL -- added True
        self.m_hardwareConfig.write(True)
        ReloadHardwareData()
        print '#### Leave endCalibration ####'












    # -----------------------------------------------------------------------------------------------------
    # -------------- Auto-Calibration support methods -----------------------------------------------------
    # -----------------------------------------------------------------------------------------------------

    def convertTupleToString(self, val):
        '''Convert a tuple to a string, with an empty tuple returning an empty
        string, instead of '()'.
        Returns a string.'''
        return str(val).strip('()')


    # -----------------------------------------------------------------------------------------------------
    # Auto-Cal coordinate registration methods
    # called from Service ServiceNewCal.cpp to register current robot position once it has been jogged
    # to the proper position for the Q1 Sample 1 position within the calibration tool well - just records
    # this as a reference point - doesn't do any calculations
    #
    # ReferencePoint(theta, carousel, r, z) is a class in tesla\types\ReferencePoint.py
    # -----------------------------------------------------------------------------------------------------

    # record theta (correct to realDeg), carousel (correct by any carousel offset) r, z)       
    def calibrateSetSample1 (self):
        self.logReferencePsn('Sample1 Reference: calibrateSetSample1') 
        self.m_sampleVialRef = ReferencePoint(self.m_thetaAxis.Theta(), self.m_carouselAxis.Theta(),
                                              0, self.m_zAxis.Position())
        """self.m_sampleVialRef = ReferencePoint(-23.1149425, -11.62758, 115.5, 140)"""

    def calibrateSetSample2 (self):
        self.logReferencePsn("Sample2 Reference: calibrateSetSample2")
        self.m_sampleVial2Ref = ReferencePoint(self.m_thetaAxis.Theta(), self.m_carouselAxis.Theta(),
                                            0, self.m_zAxis.Position())
        """self.m_sampleVial2Ref = ReferencePoint(79.1839, -96.0343, 115.5, 140)"""
    
    def calibrateSet1mlTipStrip (self):
        self.logReferencePsn("1ml Tip Reference: calibrateSet1mlTipStrip")
        self.m_1mlTipStripRef = ReferencePoint(self.m_thetaAxis.Theta(), self.m_carouselAxis.Theta(),
                                            0, self.m_zAxis.Position())
        """self.m_1mlTipStripRef = ReferencePoint(-11.0459770115,0,0,63)"""
   
    def calibrateSet5mlTipStrip (self):
        self.logReferencePsn("5ml Tip Reference: calibrateSet1mlTipStrip")
        self.m_5mlTipStripRef = ReferencePoint(self.m_thetaAxis.Theta(), self.m_carouselAxis.Theta(),
                                            0, self.m_zAxis.Position())
        """self.m_5mlTipStripRef = ReferencePoint(-22.0038314176,0,0,66)"""
    
    def calibrateSetBCCM (self):
        self.logReferencePsn("BCCM Reference: calibrateSetBCCM")
        self.m_bccmRef = ReferencePoint(self.m_thetaAxis.Theta(), self.m_carouselAxis.Theta(),
                                            0, self.m_zAxis.Position())
        """self.m_bccmRef = ReferencePoint(140,0,0,76) theta, carousel, distance, z"""

    def logReferencePsn(self, label):
        msg = '%s theta = %9.2f *self.thetaToRealDeg, carousel = %9.2f, z = %7.3f' % \
              (label, self.m_thetaAxis.Theta(), self.m_carouselAxis.Theta(), self.m_zAxis.Position())
        self.logger.info(msg)     
        funcReference = __name__ + '.logReferencePsn'              # 2011-11-28 sp -- added logging
        self.svrLog.logInfo('xx', self.logPrefix, funcReference, msg)    # 2011-11-28 sp -- added logging; logging not tested
        #msg = label, 'theta = ', self.m_thetaAxis.Theta(), 'carousel = ', \
        #      self.m_carouselAxis.Theta(), 'z = ', self.m_zAxis.Position()
        #self.logger.info(msg)






    # -----------------------------------------------------------------------------------------------------
    # Calibration routine called from Auto-Cal in Service to set positions of all tubes, vials and tips and
    # z positions of vial and tube bases and tip strip coordinates
    # NOTE: due to a change in the arm drive gear the angles used within the server are not true angles so
    # theta angles from calibration adjustments must be converted to real degrees before use
    #
    # Older manual calibration calculates its Z heights from the calibration jig height offset which ist
    # typically 1.27 mm - autocal must convert its Z heights to agree with this jig height offset as well
    # -----------------------------------------------------------------------------------------------------
    
    def calibrateCalcTipsAndVials (self):
        #MUST load constants first - these come from hardware.ini file
        self.logger.info('CalibrateCalcTipsAndVials() - final auto-calibration calculations...')
        funcReference = __name__ + '.calibrateCalcTipsAndVials'              # 2011-11-28 sp -- added logging
        self.svrLog.logInfo('xx', self.logPrefix, funcReference, 'CalibrateCalcTipsAndVials() - final auto-calibration calculations...')    # 2011-11-28 sp -- added logging; logging not tested
        self.LoadCalibrationConstants()

        # use Q1 Sample 1 and 2 sample tube reference points (equidistant +/- from the line joining the
        # carousel hub center and the robot arm post center) - default positions for S1 and S2 reference
        # theta,carousel positions during autocal are -24,-11 and 79,-96 degrees
        if None != self.m_sampleVialRef and None != self.m_sampleVial2Ref:
            # arm is calibrated in cylindrical coordinates - z, theta and r - theta angle and radius must
            # be converted using real degrees before use - factor is thetaToRealDeg (1.52 from hdwr.ini)
            self.m_sampleVialRef.theta = self.m_sampleVialRef.theta / self.thetaToRealDeg
            self.m_sampleVial2Ref.theta = self.m_sampleVial2Ref.theta / self.thetaToRealDeg 
            self.m_sampleVialRef.r = TeslaPlatform.referencePointCalibrationDistances[TeslaPlatform.Sample_Label]
            self.m_sampleVial2Ref.r = TeslaPlatform.referencePointCalibrationDistances[TeslaPlatform.Sample_Label]

            # use average sample tube cyl coords to calculate final carousel and arm coords for all
            # tip, tube and vial locations on the carousel
            self.logger.info('CalibrateCalcTipsAndVials() - Generating vial theta,carousel angles')
            self.svrLog.logInfo('xx', self.logPrefix, funcReference, 'CalibrateCalcTipsAndVials() - Generating vial theta,carousel angles')    # 2011-11-28 sp -- added logging; logging not tested
            self.GenerateReferencePoints(self.m_sampleVialRef, self.m_sampleVial2Ref, 0)
                                    
            # use average sample tube z to calculate final tube and vial z positions and for the tiprack
            # tip pickup positions
            self.logger.info('CalibrateCalcTipsAndVials() - Generating vial Z positions ')
            self.svrLog.logInfo('xx', self.logPrefix, funcReference, 'CalibrateCalcTipsAndVials() - Generating vial Z positions ')    # 2011-11-28 sp -- added logging; logging not tested
            self.GenerateZPositions((self.m_sampleVialRef.z+self.m_sampleVial2Ref.z)/2)


        # generate tip strip positions for 5 and 1 ml tips from reference positions supplied by auto-cal
        if None != self.m_1mlTipStripRef and None != self.m_5mlTipStripRef:
            s = self.convertTupleToString(( self.m_1mlTipStripRef.theta, 0))
            self.m_hardwareConfig.writeItem( self.ReferencePointsLabel,
                                        self.m_platform.StripTip1_1ml_Label, s )
            s = self.convertTupleToString(( self.m_1mlTipStripRef.theta, 0))
            self.m_hardwareConfig.writeItem( self.ReferencePointsLabel,
                                        self.m_platform.StripTip2_1ml_Label, s )
            s = self.convertTupleToString(( self.m_1mlTipStripRef.theta, 0))
            self.m_hardwareConfig.writeItem( self.ReferencePointsLabel,
                                        self.m_platform.StripTip3_1ml_Label, s )
            
            height = float( self.m_1mlTipStripRef.z ) + self.tip1mlStripToolZ
            self.logger.info('CalibrateCalcTipsAndVials() - Write 1mlTip Strip height = %7.3f' % height)
            self.svrLog.logInfo('xx', self.logPrefix, funcReference, 'CalibrateCalcTipsAndVials() - Write 1mlTip Strip height = %7.3f' % height)    # 2011-11-28 sp -- added logging; logging not tested
            self.m_hardwareConfig.writeItem( self.m_platform.SectionName,
                                        self.m_platform.StripPosition1mLTipLabel,
                                        height )
            height = self.m_1mlTipStripRef.z + float( self.m_calZStrippedOffset1ml ) + self.tip1mlStripToolZ
            self.logger.info('CalibrateCalcTipsAndVials() - Write 1mlTip Stripped height = %7.3f' % height)
            self.svrLog.logInfo('xx', self.logPrefix, funcReference, 'CalibrateCalcTipsAndVials() - Write 1mlTip Stripped height = %7.3f' % height)    # 2011-11-28 sp -- added logging; logging not tested
            self.m_hardwareConfig.writeItem( self.m_platform.SectionName,
                                        self.m_platform.StrippedPosition1mLTipLabel,
                                        height )
 
            s = self.convertTupleToString(( self.m_5mlTipStripRef.theta, 0))
            self.m_hardwareConfig.writeItem( self.ReferencePointsLabel,
                                        self.m_platform.StripTip4_5ml_Label, s )
            s = self.convertTupleToString(( self.m_5mlTipStripRef.theta, 0))
            self.m_hardwareConfig.writeItem( self.ReferencePointsLabel,
                                        self.m_platform.StripTip5_5ml_Label, s )
            height = float( self.m_5mlTipStripRef.z ) + self.tip5mlStripToolZ
            self.logger.info('CalibrateCalcTipsAndVials() - Write 5mlTip Strip height = %7.3f' % height)
            self.svrLog.logInfo('xx', self.logPrefix, funcReference, 'CalibrateCalcTipsAndVials() - Write 5mlTip Strip height = %7.3f' % height)    # 2011-11-28 sp -- added logging; logging not tested
            self.m_hardwareConfig.writeItem( self.m_platform.SectionName,
                                        self.m_platform.StripPosition5mLTipLabel,
                                        height )
            height = self.m_5mlTipStripRef.z + float( self.m_calZStrippedOffset5ml ) + self.tip5mlStripToolZ
            self.logger.info('CalibrateCalcTipsAndVials() - Write 5mlTip Stripped height = %7.3f' % height)
            self.svrLog.logInfo('xx', self.logPrefix, funcReference, 'CalibrateCalcTipsAndVials() - Write 5mlTip Strip height = %7.3f' % height)    # 2011-11-28 sp -- added logging; logging not tested
            self.m_hardwareConfig.writeItem( self.m_platform.SectionName,
                                        self.m_platform.StrippedPosition5mLTipLabel,
                                        height )
            
        if None != self.m_bccmRef:
            self.logger.info('CalibrateCalcTipsAndVials() - Write Buffer Bottle theta = %9.2f' % self.m_bccmRef.theta)
            self.svrLog.logInfo('xx', self.logPrefix, funcReference, 'CalibrateCalcTipsAndVials() - Write Buffer Bottle theta = %9.2f' % self.m_bccmRef.theta)    # 2011-11-28 sp -- added logging; logging not tested
            self.m_hardwareConfig.writeItem( self.ReferencePointsLabel,
                                        self.m_platform.BulkLabel,
                                        self.m_bccmRef.theta )

            height = float( self.m_bccmRef.z ) + self.bccmToolZ 
            self.logger.info('CalibrateCalcTipsAndVials() - Write Buffer Bottle height = %7.3f' % height)
            self.svrLog.logInfo('xx', self.logPrefix, funcReference, 'CalibrateCalcTipsAndVials() - Write Buffer Bottle height = %7.3f' % height)    # 2011-11-28 sp -- added logging; logging not tested
            self.m_hardwareConfig.writeItem( self.m_instrument.TeslaLabel,
                                        self.m_instrument.BaseBCCMLabel,
                                        height )
            
        self.logger.info('CalibrateCalcTipsAndVials() - hardware.ini file written and reloaded')
        self.svrLog.logInfo('xx', self.logPrefix, funcReference, 'CalibrateCalcTipsAndVials() - hardware.ini file written and reloaded')    # 2011-11-28 sp -- added logging; logging not tested
# 2012-03-05 RL -- added True
        self.m_hardwareConfig.write(True)
        ReloadHardwareData()

        self.startCalibration();
             
    # calc all vials reference coords given Q1 Sample Vial 1 and 2 refs, (a2bDeg = 0)
    def GenerateReferencePoints(self, a, b, a2bDeg):
        self.m_centerline = 0;
        centreline = self.m_centerline
        #centreline = -a.carousel + 90. - ReferencePoint.carouselOffset - ReferencePoint.carouselOffset2
        #centreline = ( self.m_jigCarouselAngle1 + self.m_jigCarouselAngle2 ) / 2. + 90.

        # calc arm ref angles for all vials given sample 1 and 2 references
        allTheta = self.FindThetaLines(a, b, a2bDeg)

        # calc carousel ref angles for all vials given sample 1 and 2 references
        allCarousel = self.FindCarouselLines(a, b, a2bDeg)
        
        for i,vial in enumerate(TeslaPlatform.referencePointCalibrationList):
            theta = allTheta[i]
            carousel = allCarousel[i] - centreline + 90
            self.setCalibrationReferencePoint(vial, theta, carousel)
            
        # write carousel reference points
        self.m_hardwareConfig.writeItem( self.m_platform.SectionName,
                                        self.m_platform.CarouselThetaOffsetLabel,
                                        centreline )

            
    # called by calibrateCalcTipsAndVials method to calc z psns for all tubs and vials
    # Z position must be corrected for distance from base of vial to new autocal jig and
    # for the old manual jig since legacy code still uses z with this as a built in offset.
    
    def GenerateZPositions(self,toolZ):
        # antibody vial z = vial offset from tiphead flange + corrected tiphead flange psn
        # baseoffset_antibodyvial_mm = calbaseoffset_antibodyvial_mm = 58.0 + toolZ = 135.5 + toolToJigZ = -1.27 = 192.23
        height = float(self.m_baseOffsetAntibodyVial) + toolZ + self.toolToJigZ
        self.m_hardwareConfig.writeItem( self.m_instrument.TeslaLabel, self.m_instrument.BaseAntibodyLabel, height)

        # cocktail vial z
        # baseoffset_cocktailvial_mm = calbaseoffset_cocktailvial_mm = 58.0 + toolZ = 135.5 + toolToJigZ = -1.27 = 192.23
        height = float(self.m_baseOffsetCocktailVial) + toolZ + self.toolToJigZ
        self.m_hardwareConfig.writeItem( self.m_instrument.TeslaLabel, self.m_instrument.BaseCocktailLabel, height)
        
        # bead vial z
        # baseoffset_beadvial_mm = calbaseoffset_beadvial_mm = 58.0 + toolZ = 135.5 + toolToJigZ = -1.27 = 192.23
        height = float(self.m_baseOffsetBeadVial) + toolZ + self.toolToJigZ
        self.m_hardwareConfig.writeItem( self.m_instrument.TeslaLabel, self.m_instrument.BaseBeadLabel, height)
            
        # 14 ml tube z = 14mlbase + (corrected toolZ)
        # baseoffset_14mlvial_mm = calbaseoffset_14mlvial_mm = 81.2 + toolZ = 135.5 + toolToJigZ = -1.27 = 215.43
        height = float(self.m_baseOffset14mlVial) + toolZ + self.toolToJigZ
        self.m_hardwareConfig.writeItem( self.m_instrument.TeslaLabel, self.m_instrument.Base14mlLabel, height)

        # 50 ml tube z = 50mlbase + (corrected toolZ)
        # baseoffset_50mlvial_mm = calbaseoffset_50mlvial_mm = 97.55 + toolZ = 135.5 + toolToJigZ = -1.27 = 231.78
        height = float(self.m_baseOffset50mlVial) + toolZ + self.toolToJigZ
        self.m_hardwareConfig.writeItem( self.m_instrument.TeslaLabel, self.m_instrument.Base50mlLabel, height)  

    
    def CalcTotalDistance(self, a, b, a2bDeg):
        # get arm angle between sample 1 and 2 (typically S1 = -24,-11 and S2 = 79,-96 deg)
        # so theta angle between S1 and S2 is abs(-24 - 79) = 102 deg (simple subtraction)
        thetaDiff = tesla.AdvMath.GetThetaDiff(a, b)

        # get carousel angle difference between sample 1 and 2  (abs(-11 - -96) = 85 deg)
        carouselDiff = tesla.AdvMath.GetCarouselDiff(a, b, a2bDeg)

        # for carousel vectors a.r (hub to sample1) and b.r (hub to sample2) adjacent to angle
        # 'carouselDiff', get the 3rd leg c of the sample1-hub-sample2 triangle 
        armThirdLeg = tesla.AdvMath.CosLawSAS(a.r, carouselDiff, b.r)
    
        # given arm length (post to sample1) adjacent to theta angle 'thetaDiff', get the 3rd leg c
        # of the sample1-armPost-sample2 triangle using cosine law
        armThirdLeg2 = tesla.AdvMath.CosLawSAS(self.armLength, thetaDiff, self.armLength)
        diff = armThirdLeg - armThirdLeg2
        if diff < 0.01:
            diff=0
        #armThirdLeg=armThirdLeg2
        armTheta1 = (180 - math.fabs(thetaDiff)) / 2
        armTheta2 = armTheta1
        shortR = a.r
        if a.r > b.r:
            shortR = b.r
        longR = a.r
        if a.r == shortR:
            longR = b.r
            
        carouselTheta2 = tesla.AdvMath.SinLawSSA(armThirdLeg, shortR, carouselDiff)
        carouselTheta1 = 180 - carouselTheta2 - carouselDiff

        totalDistance =tesla.AdvMath.CosLawSAS(self.armLength, armTheta2 + carouselTheta2,longR)
        totalDistance2 =tesla.AdvMath.CosLawSAS(self.armLength, 360 - armTheta1 - carouselTheta1, shortR)
        #totalDistance=totalDistance2
        return totalDistance


    # calc vector from arm post to all vials on carousel given Sample 1 and 2 reference angles
    # a2bDeg is 0
    def FindThetaLines(self, a, b, a2bDeg):
        allTheta=[]
        totalDistance = self.CalcTotalDistance(a, b, a2bDeg)
        for i,vial in enumerate(TeslaPlatform.referencePointCalibrationList):
            allTheta = allTheta+[
                tesla.AdvMath.GetThetaFromR(vial,
                    TeslaPlatform.referencePointCalibrationDistances[vial],
                    self.armLength, totalDistance, a.r, a.theta,
                    0, self.thetaToRealDeg)
               ]
        return allTheta

    def FindCarouselLines(self, a, b, a2bDeg):
        allCarousel=[]
        totalDistance = self.CalcTotalDistance(a, b,a2bDeg);
        for i,vial in enumerate(TeslaPlatform.referencePointCalibrationList):
            allCarousel = allCarousel+[
                tesla.AdvMath.GetCarouselFromR(vial,
                    TeslaPlatform.referencePointCalibrationDistances[vial],
                    self.armLength, totalDistance, a.r, a.carousel,
                    TeslaPlatform.referencePointCalibrationToSampleDegs[vial]+
                    0) #ReferencePoint.carouselOffset2
               ]
        return allCarousel

    def LoadCalibrationConstants(self):
        #print "1"
        #print self.m_hardwareConfig.Item( Calibration.CalibrationLabel,Calibration.ThetaToRealDegLabel), \
        #      self.m_hardwareConfig.Item( Calibration.CalibrationLabel,Calibration.ArmLengthLabel)
        
        self.thetaToRealDeg = float(self.m_hardwareConfig.Item( Calibration.CalibrationLabel,
                                                                        Calibration.ThetaToRealDegLabel))  
        #print "2" 
        self.armLength =float(self.m_hardwareConfig.Item( Calibration.CalibrationLabel,
                                                          Calibration.ArmLengthLabel))  
        #print "3"    
        self.toolToJigZ =float(self.m_hardwareConfig.Item( Calibration.CalibrationLabel,
                                                          Calibration.ToolToJigZLabel)) 
        #print "4"    
        self.tip1mlStripToolZ =float(self.m_hardwareConfig.Item( Calibration.CalibrationLabel,
                                                          Calibration.Tip1mlStripToolZLabel))  
        #print "5"     
        self.tip5mlStripToolZ =float(self.m_hardwareConfig.Item( Calibration.CalibrationLabel,
                                                          Calibration.Tip5mlStripToolZLabel)) 
        #print "6"      
        self.bccmToolZ =float(self.m_hardwareConfig.Item( Calibration.CalibrationLabel,
                                                          Calibration.BccmToolZLabel))
        
        self.logger.info('Load alibration Constants from hardware.ini file:\n')
        self.logger.info('  thetaToRealDeg = %f,'           % self.thetaToRealDeg)
        self.logger.info('  robot arm length = %f mm,'      % self.armLength)
        self.logger.info('  autoCal tool to Jig Z = %f mm,' % self.toolToJigZ)
        self.logger.info('  1 ml tip stripArm Z = %f mm,'   % self.tip1mlStripToolZ)
        self.logger.info('  5 ml tip stripArm Z = %f mm,'   % self.tip1mlStripToolZ)
        self.logger.info('  bufferBottle tool Z = %f mm'    % self.bccmToolZ)

        self.logger.info('Load Calibration Reference Points  from hardware.ini file:')

        funcReference = __name__ + '.LoadCalibrationConstants'              # 2011-11-28 sp -- added logging; logging not tested
        self.svrLog.logInfo('xx', self.logPrefix, funcReference, 'Load alibration Constants from hardware.ini file:')    # 2011-11-28 sp -- added logging
        self.svrLog.logInfo('xx', self.logPrefix, funcReference, '  thetaToRealDeg = %f,'           % self.thetaToRealDeg)    # 2011-11-28 sp -- added logging
        self.svrLog.logInfo('xx', self.logPrefix, funcReference, '  robot arm length = %f mm,'      % self.armLength)    # 2011-11-28 sp -- added logging
        self.svrLog.logInfo('xx', self.logPrefix, funcReference, '  autoCal tool to Jig Z = %f mm,' % self.toolToJigZ)    # 2011-11-28 sp -- added logging
        self.svrLog.logInfo('xx', self.logPrefix, funcReference, '  1 ml tip stripArm Z = %f mm,'   % self.tip1mlStripToolZ)    # 2011-11-28 sp -- added logging
        self.svrLog.logInfo('xx', self.logPrefix, funcReference, '  5 ml tip stripArm Z = %f mm,'   % self.tip1mlStripToolZ)    # 2011-11-28 sp -- added logging
        self.svrLog.logInfo('xx', self.logPrefix, funcReference, '  bufferBottle tool Z = %f mm'    % self.bccmToolZ)    # 2011-11-28 sp -- added logging
        self.svrLog.logInfo('xx', self.logPrefix, funcReference, 'Load Calibration Reference Points  from hardware.ini file:')    # 2011-11-28 sp -- added logging

        for i,vial in enumerate(TeslaPlatform.referencePointCalibrationList):
            TeslaPlatform.referencePointCalibrationDistances[vial]=  \
                                        float(self.m_hardwareConfig.Item( Calibration.CalibrationLabel,  
                                        TeslaPlatform.referencePointCalibrationDistanceNames[vial]))  
            TeslaPlatform.referencePointCalibrationToSampleDegs[vial]=  \
                                        float(self.m_hardwareConfig.Item( Calibration.CalibrationLabel,  
                                        TeslaPlatform.referencePointCalibrationToSampleDegNames[vial]))           
            self.logger.info('%s = %f deg,' % TeslaPlatform.referencePointCalibrationDistances[vial])
            self.logger.info('%s = %f deg,' % TeslaPlatform.referencePointCalibrationToSampleDegs[vial])
            self.svrLog.logInfo('xx', self.logPrefix, funcReference, '%s = %f deg,' % TeslaPlatform.referencePointCalibrationDistances[vial])    # 2011-11-28 sp -- added logging; logging not tested
            self.svrLog.logInfo('xx', self.logPrefix, funcReference, '%s = %f deg,' % TeslaPlatform.referencePointCalibrationToSampleDegs[vial])    # 2011-11-28 sp -- added logging; logging not tested
# eof
