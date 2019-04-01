# config.py
# tesla.hardware.config.py
#
# Class to read the hardware configuration (from the "soft setup" file)
# 
# Copyright (c) Invetech Pty Ltd, 2004
# 495 Blackburn Rd
# Mt Waverley, Vic, Australia.
# Phone (+61 3) 9211 7700
# Fax m  (+61 3) 9211 7701
# 
# The copyright to the computer program(s) herein is the
# property of Invetech Pty Ltd, Australia.
# The program(s) may be used and/or copied only with
# the written permission of Invetech Pty Ltd
# or in accordance with the terms and conditions
# stipulated in the agreement/contract under which
# the program(s) have been supplied.
#

import os
import re

import time
from datetime import datetime

from ipl.utils.file import createDir
from ipl.utils.path import path

from ConfigParser import *
from shutil import copyfile

import tesla.logger
import tesla.config
from tesla.exception import TeslaException


# ---------------------------------------------------------------------------

class HardwareError(TeslaException):
    """General exception for errors in hardware"""
    pass

# ---------------------------------------------------------------------------

class ConfigurationException(TeslaException):
    '''A convenience wrapper around the various ConfigParser errors
    that can occur'''
    pass

# ---------------------------------------------------------------------------

class HardwareConfiguration:
    '''Access to the hardware configuration file
    The configuration file consists of sections, led by a "[section]" header 
    and followed by "name: value" entries, with continuations in the style of
    RFC 822; "name=value" is also accepted. Note that leading whitespace is
    removed from values. The optional values can contain format strings which
    refer to other values in the same section, or values in a special DEFAULT 
    section. Additional defaults can be provided on initialization and retrieval. 
    Lines beginning with "#" or ";" are ignored and may be used to provide 
    comments. 

    To access the configuration data (or to set new values), work with the
    HardwareConfiguration.cfg member directly. 

    For more information, see Section 5.14 "ConfigParser" in the Python Library
    Reference.
    '''												
       
    def __init__(self, configFile, mainFile=True):
        if mainFile:
            '''Constructor; can specify an alternative configuration file to the
            one listed in the top level Tesla config.py file'''

            # Initialise the logger
            self.logger = tesla.logger.Logger(logName = tesla.config.LOG_PATH)
            self.LoadFile(configFile)

        else:
            '''Constructor for misc file'''
            if not os.path.exists( configFile):
                self.CreateDefaultHardwareMiscFile(configFile)   
            try:
                self.LoadFile(configFile)
            except:
                self.CreateDefaultHardwareMiscFile(configFile)
                self.LoadFile(configFile)
            
    def CreateDefaultHardwareMiscFile(self, configFile):
        cfg = ConfigParser()
        cfg.add_section('Pump')
        cfg.set('Pump','hydraulicbottlelevel',str(0))
    
        try:
            temp = configFile + '.tmp'
            fp = open(temp, 'w')
            cfg.write(fp)
            fp.close()

            copyfile(temp,configFile)
            
        except IOError, msg:
            raise ConfigurationException(msg)  

    def LoadFile(self, configFile):
        
        # Read in the file
        self.configFile = configFile
        cfg = ConfigParser()
        try:
            cfg.read(self.configFile)
        except MissingSectionHeaderError, msg:
            raise ConfigurationException(msg)
        except ParsingError, msg:
            raise ConfigurationException(msg)

        # convert the config data to a dictionary
        #
        self.m_Sections = {}
        for sectionKey in cfg.sections():
            section = {}
            for (cfgID, value) in cfg.items (sectionKey):
                section[cfgID] = value
                
            self.m_Sections[sectionKey] = section

        #lidSensorSettings = self.Section ('LidSensor')
        #print '--------------------roy2-----------------'
        #print lidSensorSettings
        #print '--------------------roy2-----------------'


    def SectionList(self):
        """Obtain a list of the sections read in"""
        return self.m_Sections.keys()
        
    def SectionExists(self, sectionKey):
        """Determine whether an given section exists"""
        return sectionKey in self.m_Sections.keys()
        
    def Section(self, sectionKey):
        """Obtain the item dictionary of the given section.
            An empty dictionary is returned if section does not exist"""
        if self.SectionExists (sectionKey):
            return self.m_Sections[sectionKey]
        else:
            return {}
        
    def ItemExists(self, sectionKey, itemKey):
        """Determine whether a given item exists in a given section"""
        section = self.Section(sectionKey)
        return itemKey in section.keys()
        
    def Item(self, sectionKey, itemKey):
        """Obtain a specific item in a given section.
            Returns an empty string if item does not exist"""
        section = self.Section(sectionKey)

        if itemKey in section.keys():
            return section[itemKey]
        else:
            return ''

    def writeItem( self, sectionKey, itemKey, value ):
        """Set a value to a specific section and key. If the item does not
        exist it will be created. The section must already exist. New settings
        are not saved to disk until the write method is called."""
        section = self.Section( sectionKey )
        section[itemKey] = str( value )

    def backupConfig(self, fileName):
        """Called from write() - renames all files in the config backup folder by effectively
        advancing their trailing numeral by one and then write the most recent config file 
        as hardware.ini.1."""
        print "backupConfig start"
		
        # create the directory if necessary
        backupDir = tesla.config.CFG_BACKUP_DIR        
        createDir(backupDir)
        
        stamp = datetime.fromtimestamp(time.time()).isoformat()[:-7]
        ext = "." + stamp.replace('-','').replace(':','')

        #stamp = datetime.fromtimestamp(time.time()).isoformat().replace('-','')
        #ext = "." + stamp.replace(':','')
        
        # copy this file to dir with .1 extension added
        copyfile(tesla.config.HW_CFG_PATH, tesla.config.HW_BACK_PATH + ext)
        print "backupConfig exit", ext
        
    def SaveIniAsFactory(self):                                       #RL Add
        copyfile(tesla.config.HW_CFG_PATH, tesla.config.HW_CFG_PATH + ".factory")
        print '0000000000000000000000000000000000000000'

    def RestoreIniWithFactory(self):                                           #RL Add
        copyfile(tesla.config.HW_CFG_PATH + ".factory",tesla.config.HW_CFG_PATH)
        print '111111111111111111111111111'

# 2012-03-05 RL -- added
    def LoadSelectedHardwareINI(self,path):                                       #RL Add
        copyfile(path,tesla.config.HW_CFG_PATH)
# 2012-03-05 RL -- modified
#    def write(self, newFile = None):
    def write(self, backup=False, newFile = None):
        '''Write the hardware configuration back out to disk. If newFile is
        specified, write to that file; otherwise, write to the file that we
        read from at construction-time.'''

        # Create an empty config object
        print "bdr in config.py - writing", newFile, self.configFile
        cfg = ConfigParser()

        # Populate the config with the sections data
        for sectionKey in self.m_Sections.keys():
            # Create a section
            cfg.add_section(sectionKey)
            for itemKey in self.m_Sections[sectionKey].keys():
                # Add the item to the section
                cfg.set(sectionKey,itemKey,self.m_Sections[sectionKey][itemKey])
    
        try:
            if newFile:
                fileName = newFile
            else:
                fileName = self.configFile
                # backupFileName = fileName + '.bak'
                # copyfile( fileName, backupFileName )
# 2012-03-05 RL -- added backup checking
            if backup:
                self.backupConfig( fileName )

# 2013-07-23 RL use temp fle then copy
            temp = fileName + '.tmp'
            fp = open(temp, 'w')
            cfg.write(fp)
            fp.close()

            copyfile(temp,fileName)
            
        except IOError, msg:
            raise ConfigurationException(msg)        

#-------------------------------------------------------------------------

knownMissingSettings = []

def LoadSettings (settings, configData, mandatoryList = ()):
    """Static method for reading in settings obtained from a section in HardwareConfiguration.
        settings: a dictionary of existing settings. Default settings may already be given.
        configData: a dictionary of configuration values to be used.
        mandatoryList:  a list of mandatory values that must be present in the configData.
                        A ConfigurationException is thrown if any mandatory items are missing
        """
    
    debugFlag = tesla.config.HW_DEBUG

    # Check for (and load if present) mandatory configurations. Throw a settings exception if not present
    #
    missingComponents = []
    for key in mandatoryList:
        if key not in configData.keys():
            # Mark as missing if key is not already present
            if key not in settings.keys():
                missingComponents.append(key)
        else:
            settings[key] = configData[key]

    if len (missingComponents) > 0:
        errMsg = "The following components are mandatory: %s" % (missingComponents,)
        if debugFlag: print errMsg
        raise ConfigurationException (errMsg)
        
    missingList = []
    for key in settings.keys():
        if key in configData.keys():
            # Copy the configuration value                
            settings[key] = configData[key]
        else:
            # Note missing value, and use the hard coded default (which is already present)
            missingList.append (key)

    if len(missingList) != 0 and missingList not in knownMissingSettings:
        msg = "The following defaults were not passed in configData: %s" % (missingList,)
        if debugFlag: print msg
        knownMissingSettings.append(missingList)
        

def ReloadHardwareData (iniFilePath = tesla.config.HW_CFG_PATH, miscFilePath = tesla.config.HW_MISC_PATH):
    global gHardwareData
    global gHardwareMisc
    #gHardwareData = 0
    #gHardwareData = HardwareConfiguration (iniFilePath)
    gHardwareData.LoadFile(iniFilePath)
    gHardwareMisc.LoadFile(miscFilePath)

    
def GetHardwareData():
    global gHardwareData
    return gHardwareData

# ---------------------------------------------------------------------------
# Singleton instance 
#
gHardwareData = HardwareConfiguration (tesla.config.HW_CFG_PATH)
gHardwareMisc = HardwareConfiguration (tesla.config.HW_MISC_PATH, False)
# eof

