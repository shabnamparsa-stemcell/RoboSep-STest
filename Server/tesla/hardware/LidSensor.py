# 
# LidSensor.py
# tesla.Hardware.LidSensor
#
# Abstract class driving the lid sensors (via SimpleStep module)
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

from SimpleStep.SimpleStep import SimpleStep, SimpleStepError
from tesla.hardware.config import HardwareError, LoadSettings
from tesla.hardware.Device import Device

import os

from tesla.PgmLog import PgmLog    # 2011-11-29 sp -- programming logging
import tesla.config         # 2012-01-30 sp -- replace environment variables with configuration file settings

# ---------------------------------------------------------------------------

class LidSensor(Device):
    """Class to drive a SimpleStep stepper card for reading the lid sensor"""

    # The set of configuration labels used by LidSensor
    # (NB: use lower case, since configParser converts items to lowercase)
    #

    SensorPortLabel = "sensorport"
    IgnoreLidSensorLabel = "ignorelidsensor"
    
    DefaultSettings = { SensorPortLabel: '4',
                        IgnoreLidSensorLabel: '0'
                        }

   # -------------------------------------------------------------------------
    
    def __init__ (self, name, card, configData = {}, moreSettings = {}):
        '''Refer to stepper card being controlled.
        name: name of the device
        card: the SimpleStep instance through which card is to be driven.
        configData: a dictionary of configuration settings, read in from a file
        moreDefaults: default settings, passed in from a derived class
        '''
        Device.__init__(self, name = name)
       
        # Load the settings
        settings = LidSensor.DefaultSettings.copy()
        settings.update( moreSettings )
        LoadSettings( settings, configData )
#        self.ValidateSettings()            

        # Initialise variables to the defaults
        self.m_Name = name
        self.m_Settings = settings
        self.m_card = card

        # 2011-11-29 sp -- added logging
        self.svrLog = PgmLog( 'svrLog' )
        self.logPrefix = 'LS'

        self.__m_sensorPort = int( settings[LidSensor.SensorPortLabel] )
        self.__m_bIgnoreLidSensor = int( settings[LidSensor.IgnoreLidSensorLabel] ) == 1
        #print '######################Set ignore lid sensor',self.__m_bIgnoreLidSensor
        
    def _serviceMethods(self):
        '''Return a list of methods for the service interface'''
        return []


    # --- methods ---

    def getSensorState( self ):
        """Gets the state of lid sensor"""
        if self.debug: print(self, "LidSensor::getSensorState")
        command = 'I' + str(self.__m_sensorPort)
        if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
            self.m_card.ExtLogger.SetCmdListLog("sendAndCheck()[%s] called from LidSensor.getSensorState()"%self.m_card.prefix, self.m_card.prefix);
        funcReference = __name__ + '.getSensorState'          # 2011-11-29 sp -- added logging
        result = self.m_card.sendAndCheck( command ) 
        result = int(result[0])
        if self.debug: print("LidSensor result =", result)
        self.svrLog.logDebug('', self.logPrefix, funcReference, "LidSensor %s, state=%d" % (self.m_card.prefix, result))   # 2011-11-29 sp -- added logging
        return result == 1

    def isLidClosed( self ):
        '''Returns True if the lid is closed.'''

        #print '#####################Get ignore lid sensor',self.__m_bIgnoreLidSensor

        return self.__m_bIgnoreLidSensor or not self.getSensorState() 
        
    def SetIgnoreLidSensor(self, sw):
        if sw > 0 :
           self.__m_bIgnoreLidSensor = True;
        else:
           self.__m_bIgnoreLidSensor = False; 
    
    def GetIgnoreLidSensor(self):
        return self.__m_bIgnoreLidSensor;
        
    def GetFirmwareVersion(self):
        '''Return the firmware version for the card attached to this axis.'''
        boardInfo = ''
        # Grab the last three digits from the string (the rest *seem* to 
        # just be zeros
        for param in [0, 1, 4, 5,9]:
            if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                self.m_Card.ExtLogger.SetLog("getBoardInfo()[%s] from LidSensor.GetFirmwareVersion()"%self.m_Card.prefix, self.m_Card.prefix)
            funcReference = __name__ + '.GetFirmwareVersion'          # 2011-11-29 sp -- added logging
            self.svrLog.logVerbose('', self.logPrefix, funcReference, "getBoardInfo()[%s] from LidSensor.GetFirmwareVersion()"%self.m_Card.prefix)   # 2011-11-29 sp -- added logging
            boardInfo += self.m_card.getBoardInfo(param)[-3:] + ':'
        # Cut off that trailing (& redundant) colon
        return boardInfo[:-1]

class HydraulicSensor(Device):                      #CWJ Add
      SensorPortLabel     = "sensorport"
      IgnoreSensorLabel   = "ignoresensor"
      CardAddressLabel    = "steppercardaddress"
      
      DefaultSettings = { 
                          SensorPortLabel   : '4',
                          IgnoreSensorLabel : '0',
                          CardAddressLabel  : 'Y1'
                        }

      def __init__ (self, name, card, port, bIgnore, settings):
          Device.__init__(self, name = name)
          
          self.m_Name     = name
          self.m_Settings = settings
          self.m_card     = card
          
          self.__m_sensorPort = port
          self.__m_bIgnoreSensor = bIgnore

          # 2011-11-29 sp -- added logging
          self.svrLog = PgmLog( 'svrLog' )
          self.logPrefix = 'HS'

      def getHydraulicSensorState( self ):                              #CWJ Add
                
          command = 'I' + str(self.__m_sensorPort)
          if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
             self.m_card.ExtLogger.SetCmdListLog("sendAndCheck()[%s] called from HydraulicSensor.getHydraulicSensorState()"%self.m_card.prefix, self.m_card.prefix);
          funcReference = __name__ + '.getHydraulicSensorState'          # 2011-11-29 sp -- added logging
          self.svrLog.logVerbose('', self.logPrefix, funcReference, "sendAndCheck()[%s] called from HydraulicSensor.getHydraulicSensorState()"%self.m_card.prefix)   # 2011-11-29 sp -- added logging
  
          # 2012-01-30 sp -- replace environment variable with configuration variable
          #if os.environ.has_key('SS_FORCE_EMULATION'):
          if tesla.config.SS_FORCE_EMULATION == 1:
             result = 0 
          else:
             result = self.m_card.sendAndCheck( command ) 
             result = int(result[0])
          
          if self.debug: print("Hydraulic result =", result)

          return result == 1

      def isLowHydraulicLevel(self):                                    #CWJ Add
          return not self.getHydraulicSensorState() or self.__m_bIgnoreSensor

      def SetIgnoreHydraulicSensor(self, sw):                           #CWJ Add
          if sw > 0 :
             self.__m_bIgnoreSensor = True;
          else:
             self.__m_bIgnoreSensor = False; 

      def GetIgnoreHydraulicSensor(self):                               #CWJ Add
          return self.__m_bIgnoreSensor;

def getHydraulicSensorConfigData(configData = {}, moreSettings = {}):   #CWJ Add
    # Load the settings
    settings = HydraulicSensor.DefaultSettings.copy()
    settings.update( moreSettings )
    LoadSettings( settings, configData )
          
    return settings

beaconDrv = None;

class BeaconDriver(Device):                                              #CWJ Add
      BeaconChannelLabel    = "beacon_channel" 
      BeaconPortLabel       = "beacon_port"
      BeaconEvent1TimeLabel = "beacon_event1_time"
      BeaconEvent2TimeLabel = "beacon_event2_time"
      BeaconEvent3TimeLabel = "beacon_event3_time"
      CardAddressLabel      = "steppercardaddress"
      
      DefaultSettings = { 
                          BeaconChannelLabel    : '1',
                          BeaconPortLabel       : '1',
                          BeaconEvent1TimeLabel : '50',
                          BeaconEvent2TimeLabel : '400',
                          BeaconEvent3TimeLabel : '500',                                                  
                          CardAddressLabel      : 'X1'
                        }

      def __init__ (self, Card, Port, Channel, Event1, Event2, Event3):
          self.m_card     = Card;
          self.m_port     = Port;
          self.m_channel  = Channel;
          self.m_event1   = Event1;
          self.m_event2   = Event2;
          self.m_event3   = Event3;
          beaconDrv       = self;
                                        
      def TurnOFFBeacon( self ):
          print(" \n>>> # Beacon OFF #\n");
          command = '#fD' + str(self.m_channel);
   
          if tesla.config.SS_FORCE_EMULATION == 0:
             self.m_card.sendAndCheck(command);
   
          if( tesla.config.SS_EXT_LOGGER == 1 ):  
             self.m_card.ExtLogger.SetCmdListLog("sendAndCheck()[%s] called from BeaconDriver.TurnOFF()"%self.m_card.prefix, self.m_card.prefix);
          
      def TurnONBeacon( self, evt):
          print("\n >>># Beacon ON : Event %d # \n"%evt);
          command = '#fE' + str(self.m_channel) + ',' + str(self.m_port) + ',';
          
          if   (evt == 1):
              command = command + str(self.m_event1) + ',' + str(self.m_event1);
          elif (evt == 2):    
              command = command + str(self.m_event2) + ',' + str(self.m_event2);
          elif (evt == 3):
              command = command + str(self.m_event3) + ',' + str(self.m_event3);
          else: 
              command = command + str(self.m_event1) + ',' + str(self.m_event1);

          if tesla.config.SS_FORCE_EMULATION == 0:
             self.m_card.sendAndCheck(command);

          if( tesla.config.SS_EXT_LOGGER == 1 ):  
             self.m_card.ExtLogger.SetCmdListLog("sendAndCheck()[%s] called from BeaconDriver.TrunON() : Event %d "%(self.m_card.prefix, evt), self.m_card.prefix);

          
def getBeaconDriverConfigData(configData = {}, moreSettings = {}):   #CWJ Add
    settings = BeaconDriver.DefaultSettings.copy()
    settings.update( moreSettings )
    LoadSettings( settings, configData )
        
    return settings
    
def getBeaconDriverInstance():
    global beaconDrv
    return beaconDrv;
    
    

# EOF

