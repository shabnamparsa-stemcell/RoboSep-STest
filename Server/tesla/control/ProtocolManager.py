# ProtocolManager.py
# tesla.control.ProtocolManager.py
#
# Protocol manager for the Tesla control software 
# 
# Copyright (c) Invetech Pty Ltd, 2004
# 495 Blackburn Rd
# Mt Waverley, Vic, Australia.
# Phone (+61 3) 9211 7700
# Fax   (+61 3) 9211 7701
# 
# The copyright to the computer program(s) herein is the
# property of Invetech Pty Ltd, Australia.
# The program(s) may be used and/or copied only with
# the written permission of Invetech Pty Ltd
# or in accordance with the terms and conditions
# stipulated in the agreement/contract under which
# the program(s) have been supplied.
#
# Notes:
# 01/06/06 - changes in 3.4.1 to getConsumableInformation - bdr
#    03/23/06 - uniform multi sample - RL
#    03/29/06 - Short Description for Sample Volume Dialog - RL
# 
#

import pickle, os
import re

import ipl.utils.file
from ipl.utils.validation import validateNumber
from ipl.types.Bunch import Bunch

import tesla.config
import tesla.logger
from tesla.exception import TeslaException
from tesla.types.Protocol import Protocol, ProtocolException
from tesla.types.ClientProtocol import ClientProtocol

import handyxml
#import xml.dom.Element

import threading

from tesla.PgmLog import PgmLog    # 2011-11-23 sp -- programming logging

from tesla.instrument.Instrument import Instrument
from configparser import *          #configuration from ini file
import RoboTrace
import time

#KC
#when prints == 1 debug print statements will be active
#when prints == 0 print statements will be inactive

prints = 0

# -----------------------------------------------------------------------------

class ProtocolException(TeslaException):
    '''Exceptions related to processing protocols'''
    pass

# -----------------------------------------------------------------------------

class ProtocolManager(object):
    '''Protocol manager; you can have multiple instances of this object but
    all of the protocols are shared across instances.'''

    FILEINFO_DB = 'profile.db'          # The pickled protocol file information
    PROTOCOL_DB = 'protocols.db'        # The pickled protocols
    CLIENT_PROTOCOL_DB = '_mru.xml'        # The client's protocol d/base
    protocols = {}                        # Our shared protocol map

    USER_CONFIG_XML_PATH = os.path.join(tesla.config.CONFIG_DIR, 'UserConfig.config')

    AllHumanLabel = "All Human"
    AllMouseLabel = "All Mouse"
    AllWholeBloodLabel = "All Whole Blood"
    AllOtherLabel = "All Other"
    AllLabel = "All"
    HumanTypeLabel = [ClientProtocol.HUMANPOSITIVE,
                      ClientProtocol.HUMANNEGATIVE]
    MouseTypeLabel = [ClientProtocol.MOUSEPOSITIVE,
                      ClientProtocol.MOUSENEGATIVE]
    WholeBloodTypeLabel = [ClientProtocol.WHOLEBLOODPOSITIVE,
                      ClientProtocol.WHOLEBLOODNEGATIVE]
    OtherTypeLabel = [ClientProtocol.POSITIVE,
                      ClientProtocol.NEGATIVE]
    AllTypeLabel = [ClientProtocol.POSITIVE,
                    ClientProtocol.NEGATIVE,
                    ClientProtocol.HUMANPOSITIVE,
                    ClientProtocol.HUMANNEGATIVE,
                    ClientProtocol.MOUSEPOSITIVE,
                    ClientProtocol.MOUSENEGATIVE,
                    ClientProtocol.WHOLEBLOODPOSITIVE,
                    ClientProtocol.WHOLEBLOODNEGATIVE]
    presetUserData = {
                AllHumanLabel           : HumanTypeLabel,
                AllMouseLabel           : MouseTypeLabel,
                AllWholeBloodLabel      : WholeBloodTypeLabel,
                AllOtherLabel           : OtherTypeLabel,
                AllLabel                : AllTypeLabel
                }
    STATUS_MSG = 'Status'   # A status message type

    def __init__(self, protocolPath = None, reportCallback = None): 
        '''Create the protocol manager instance. If the protocolPath is set,
        import protocol data from the files in that path.'''
        self.logger = tesla.logger.Logger(logName = tesla.config.LOG_PATH)
        #self.fileDBPath = os.path.join(tesla.config.PROTOCOL_DIR, self.FILEINFO_DB)
        #self.protocolDBPath = os.path.join(tesla.config.PROTOCOL_DIR, self.PROTOCOL_DB)
        self.path = protocolPath
        self.__reportCallback = reportCallback
        
        #print "###### Now in function: ProtocolManager.py _init_ ######"
        
        # 2011-11-25 sp -- added logging
        self.svrLog = PgmLog( 'svrLog' )
        self.logPrefix = 'PM'
        
        self.reloadProtocols()

    # --- methods used by the control centre (and interface) ------------------

    def reloadProtocols(self):
        
        funcReference = __name__ + '.reloadProtocols'   # 2011-11-254 sp -- added logging
        
        #print "##### Now in Function: %s ######" % funcReference
        self.getNewCmdLogicConfigurations()
        self.__usedDB = False
        self.clear()
        if self.path:
            #Load file (ie someone.udb) spec in UserConfig.config
            udbName = self.getCurrentUserUDBName()
            print(">>>>>>>>> UDBName is : %s" % udbName)
            (baseUDB, ext) = os.path.splitext(udbName)
            self.fileDBPath = os.path.join(self.path, baseUDB+"_"+self.FILEINFO_DB)
            self.protocolDBPath = os.path.join(self.path, baseUDB+"_"+self.PROTOCOL_DB)
            if udbName in ProtocolManager.presetUserData:
                #print ">>>>>> ProtocolManager.presetUserData has a key <<<<<<"
                if not os.path.isfile(self.fileDBPath) and \
                   not os.path.isfile(self.protocolDBPath):
                    self.logger.logInfo("PM: Using raw protocol files for PRESET USER")
                    count = self.__addProtocolFilesByType(ProtocolManager.presetUserData[udbName])
                    self.logger.logInfo("PM: count = %s" % (count))
                    self.svrLog.logInfo('', self.logPrefix, funcReference, 'Using raw protocol files for PRESET USER, count=%s' % (count) )    # 2011-11-254 sp -- added logging
                else:
                    self.__addProtocols(udbName)
            else:
                #print ">>>>>> ProtocolManager.presentUserData does not have a key <<<<<<"
                udbPath = os.path.join(self.path, udbName)
                #print ">>>>>> UDBPath is: %s" % udbPath
                #time.sleep(5)
                self.__addProtocols(udbPath)
        self.reportStatus("Protocols loaded!!!!");
        self.svrLog.logInfo('', self.logPrefix, funcReference, 'Protocols loaded!' )    # 2011-11-254 sp -- added logging
        return True;

    def getCurrentUserUDBName(self):
        filename = self.USER_CONFIG_XML_PATH
        udbName = "User1.udb"
        if os.path.isfile(filename):
            try:
                xmlData = handyxml.xml(filename,False)
                value = xmlData.CurrentUser[0].childNodes[0].nodeValue
                udbName = value
            except Exception as msg:
                pass
        return udbName

    def __getFilesFromUDB(self, udbPath, extensionMask = 'xml'):
        #print "####### Now in Function __getFilesFromUDB ######"
        files = []
        if os.path.isfile(udbPath):
            try:
                print("xml: "+udbPath)
                xmlData = handyxml.xml(udbPath,False)
                files = [os.path.join(self.path,node.childNodes[0].nodeValue) \
                         for node in xmlData.ProtocolFile ]
            except Exception as msg:
                print(msg)
                pass
            
        files.append(os.path.join(self.path,"home_axes.xml"))
        files.append(os.path.join(self.path,"prime.xml"))
        files.append(os.path.join(self.path,"shutdown.xml"))
        #print ">>>>> End of __getFilesFromUDB function <<<<<<"
        #time.sleep(5)
        return files

    def __addProtocolFilesByType(self, typeList, extensionMask = 'xml'):
        '''Find all the protocol files in the list and add them to the manager.'''
        funcReference = __name__ + '.__addProtocolFilesByType'   # 2011-11-254 sp -- added logging
        xmlFileList = self.__getFiles(self.path, extensionMask)
        count = 0
        newFiles = []
        for entry in xmlFileList:
            try:
                protocol = Protocol(fileName = entry, **self.protocol_args)
                tmp_type = protocol.type
                #this is stupid CR hack... from C# interface
                if tmp_type == ClientProtocol.POSITIVE or \
                                tmp_type == ClientProtocol.NEGATIVE:
                    if protocol.label.find("Human")>=0 and protocol.label.find("Positive")>=0:
                        tmp_type = ClientProtocol.HUMANPOSITIVE
                    elif protocol.label.find("Human")>=0 and protocol.label.find("Negative")>=0:
                        tmp_type = ClientProtocol.HUMANNEGATIVE
                    elif protocol.label.find("Mouse")>=0 and protocol.label.find("Positive")>=0:
                        tmp_type = ClientProtocol.MOUSEPOSITIVE
                    elif protocol.label.find("Mouse")>=0 and protocol.label.find("Negative")>=0:
                        tmp_type = ClientProtocol.MOUSENEGATIVE
                    elif protocol.label.find("Whole Blood")>=0 and protocol.label.find("Positive")>=0:
                        tmp_type = ClientProtocol.WHOLEBLOODPOSITIVE
                    elif protocol.label.find("Whole Blood")>=0 and protocol.label.find("Negative")>=0:
                                tmp_type = ClientProtocol.WHOLEBLOODNEGATIVE
                    if tmp_type in typeList or \
                               os.path.basename(entry) == 'home_axes.xml' or \
                               os.path.basename(entry) == 'prime.xml' or \
                               os.path.basename(entry) == 'shutdown.xml':
                        timeStamp = os.path.getmtime(entry)
                        newFiles.append(Bunch(name = entry, time = timeStamp))
                        self.add(protocol)
                        count += 1
                        #self.logger.logInfo("PM: Added protocol (%s %s)" % (protocol.label,protocol.type))
                        #self.svrLog.logDebug('', self.logPrefix, funcReference, 'Added protocol (%s %s)' % (protocol.label,protocol.type) )    # 2011-11-254 sp -- added logging
                        self.reportStatus("Loading "+protocol.label);
            except ProtocolException as msg:
                self.logger.logError("PM: Protocol error: %s" % (msg))
                self.svrLog.logError('', self.logPrefix, funcReference, 'Protocol error: %s' % (msg) )    # 2011-11-254 sp -- added logging
        if count > 0:
            # Store the protocols in the "pickled" database for faster retrieval
            self.__pickleProtocols()
            self.__storeFileInfo(self.fileDBPath, newFiles)
        else:
            # Curious that we didn't find any protocols, let's log this...
            self.logger.logWarning("PM: No protocols found in the protocol directory?" )
            self.svrLog.logWarning('', self.logPrefix, funcReference, 'No protocols found in the protocol directory' )    # 2011-11-254 sp -- added logging

        return count
    

    #CR - added finction
    #return 2-D array??
    def getCustomNames(self):
        allNames = []
        names = []
        for p in self.getProtocols():
            names = p.getCustomNames()
            allNames.append(names);
        return allNames

    def getResultChecks(self):
        allChecks = []
        checks = []
        for p in self.getProtocols():
            checks = p.getResultChecks()
            allChecks.append(checks);
        return allChecks
    
    # RL - uniform multi sample -  03/23/06
    #def getMultipleSamples(self):
    #    allSamples = []
    #    samples = []
    #    for p in self.getProtocols():
    #        samples = p.getMultipleSamples()
    #        allSamples.append(samples);
    #    return allSamples
        
    
    def getProtocolsForClient(self):
        '''Return the list of protocols in a suitable form for clients, ie. we
        are using the simpler ClientProtocol objects'''

        # RL - Short Description for Sample Volume Dialog -  03/29/06
        return [ClientProtocol(p.type, p.label, p.description, p.minVol, p.maxVol, p.numQuadrants) \
                for p in self.getProtocols()]
    
    
    def getConsumableInformation(self, protocolID, sampleVolume_uL):
        '''For the specified ID and a sample volume (in uL), determine
        what consumables are needed and return this as a list of
        ProtocolConsumable objects. The list is always RELATIVE, rather than
        an absolute mapping of reagent requirements to a specific carousel.'''
        protocol = self.getProtocolWithSampleCheck(protocolID, sampleVolume_uL)

        return protocol.getVolumes(sampleVolume_uL, protocol.type)


    def isSampleVolumeValidForProtocol(self, protocol, sampleVolume_uL):
        '''Check that a sample volume (in uL) is valid for the specified protocol.
        Returns True or False.
        Returns True if the protocol is not a separation protocol (as we don't
        care about the sample volume in those circumstances).'''
        if not isinstance(protocol, Protocol):
            # Assume that we have been passed in an ID rather than a protocol
            # object and try that...
            protocolID = protocol
            protocol = self.getProtocol(protocolID)
        if self.isSeparationProtocol(protocol.ID):
            return validateNumber(sampleVolume_uL, protocol.minVol, protocol.maxVol)
        else:
            return True
        
    
    # -------------------------------------------------------------------------

    def add(self, protocol):
        '''Add a protocol to the manager'''
        ProtocolManager.protocols[protocol.ID] = protocol

    def getProtocol(self, ID = None, label = None):
        '''Find a protocol by searching for it's ID or label'''
        if ID:
            return self[ID]
        else:
            for protocol in list(ProtocolManager.protocols.values()):
                if protocol.matches(ID, label):
                    hit = protocol
                    break
            else:
                hit = None
            return hit

    def findProtocolsByType(self, protocolType):
        '''Return a list of all protocols who match the specified type.'''
        return [p for p in self.getProtocols() if p.type == protocolType]

    def findProtocolsByRegexp(self, regexp):
        '''Return a list of all protocols whose labels match the regexp.'''
        pattern = re.compile(regexp)
        return [p for p in self.getProtocols() if pattern.search(p.label) != None]

    def getProtocols(self):
        '''Return the list of protocols'''
        #print(ProtocolManager.protocols.values())
        return list(ProtocolManager.protocols.values())


    def getProtocolWithSampleCheck(self, protocolID, sampleVolume_uL):
        '''Return a protocol object (as specified by it's ID), checking that
        the sample volume is appropriate.'''
        print("bdr -- Fetching protocol id = %s " % (protocolID))
        protocol = self.getProtocol(protocolID)
        if not protocol:
            raise ProtocolException("Can not find protocol ID = %d" % (protocolID))
        else:
            # Let's sanity check the sample volume to ensure it's within
            # our protocol's limits
            # We only do this for sample processing protocols

            print("bdr -- ProtocolManager - getProtWithSampChk chk sampleVolume= %0.2f" % (sampleVolume_uL))

            if not self.isSampleVolumeValidForProtocol(protocol, sampleVolume_uL):
                raise ProtocolException("Sample volume (%0.2f) is not within protocol limits (%02.f -> %0.2fuL)" \
                        % (sampleVolume_uL, protocol.minVol, protocol.maxVol))
            else:
                print("bdr -- ProtocolManager - getProtWithSampChk - return protocol ok")
                return protocol

    def isSeparationProtocol(self, ID):
        '''Returns True if this protocol is for separation.'''
        p = self.getProtocol(ID)
        return p != None and p.type in [ClientProtocol.POSITIVE,
                                        ClientProtocol.NEGATIVE,
                                        ClientProtocol.HUMANPOSITIVE,
                                        ClientProtocol.HUMANNEGATIVE,
                                        ClientProtocol.MOUSEPOSITIVE,
                                        ClientProtocol.MOUSENEGATIVE,
                                        ClientProtocol.WHOLEBLOODPOSITIVE,
                                        ClientProtocol.WHOLEBLOODNEGATIVE]#CR
 
 
    # --- Protocol file management --------------------------------------------

    def usedDB(self):
        '''Returns true if the protocol database was used, rather than importing
        XML files.'''
        return self.__usedDB

    def __getFileInfo(self, dbName):
        '''Return a list of information about files from a pickled database.
        Used to compare time stamps and file names with the current file list.'''
        funcReference = __name__ + '.__getFileInfo'   # 2011-11-254 sp -- added logging
        files = []
        pf = None
        try:
            print (">>>>>>>> File Path is: %s" % dbName)
            pf = open(dbName, 'rb')
            files = pickle.load(pf)
            pf.close()
        except EOFError:
            print(">>>>>> EOFError Occurred <<<<<<")
            RoboTrace.RoboSepMessageBox("User Profile is corrupted. Please re-configure the User Profile after the application started")
            if pf != None:
                pf.close()
            #YW: Comment out the Correction temporarily 
            #os.remove(dbName)
        except IOError as msg:
            #print ">>>>>> IOError Occurred <<<<<<"
            self.logger.logWarning("PM: Unable to read file information: %s" % (msg))            
            self.svrLog.logWarning('', self.logPrefix, funcReference, 'Unable to read file information: %s' % (msg) )    # 2011-11-254 sp -- added logging

        return files

    def __storeFileInfo(self, dbName, fileList):
        '''Store the information about each file in the database'''
        funcReference = __name__ + '.__storeFileInfo'   # 2011-11-254 sp -- added logging
        print("###### Now in ")
        try:
            pf = open(dbName, 'wb')
            pickle.dump(fileList, pf)            # Store the file info
            pf.close()

            if prints == 1: print("\nProtocolManager.py, Replacing old database with new")

        except IOError as msg:
            self.logger.logWarning("PM: Unable to store file information: %s" % (msg))
            self.svrLog.logWarning('', self.logPrefix, funcReference, 'Unable to store file information: %s' % (msg) )    # 2011-11-254 sp -- added logging

    def filesAreEqual(self, fileList1, fileList2):
        '''Returns true if the info in the two file lists are the same'''
        equalState = False                          # Assume they're not equal
        if len(fileList1) == len(fileList2):        # Are they the same size?
            fileList1.sort(key=(lambda f: f.name))
            fileList2.sort(key=(lambda f: f.name))
            for f1, f2 in zip(fileList1, fileList2):
                if f1.name != f2.name or f1.time != f2.time:
                    break
            else:
                equalState = True
        return equalState

    def isDatabaseCurrent(self, xmlFileList):
        '''What's more recent? The pickled protocol database file or some/all
        of the raw protocol XML files in xmlFileList?
        Returns True if the pickled database is fresher.'''
        # Create the list of bunches of filenames and their times
        self.__usedDB = False
        newFiles = []
        for fileName in xmlFileList:
            if os.path.exists(fileName):
                timeStamp = os.path.getmtime(fileName)
                newFiles.append(Bunch(name = fileName, time = timeStamp))
        #print ">>>>>> Going to check is the User1_profile.db exists or not <<<<<"
        #time.sleep(10)
        if os.path.exists(self.fileDBPath) and os.path.exists(self.protocolDBPath):
            # Read in the data from the protocol file database and compare
            # with the current files
            oldFiles = self.__getFileInfo(self.fileDBPath)
            self.__usedDB = self.filesAreEqual(oldFiles, newFiles)
        # Now store updated file information in the database if the database
        # was out of date
        if not self.__usedDB:
            self.__storeFileInfo(self.fileDBPath, newFiles)
        return self.__usedDB

    def __getFiles(self, path, extensionMask = 'xml'):
        '''Return a list of files from the specified path and extension mask,
        ignoring the client protocol database'''
        funcReference = __name__ + '.__getFiles'   # 2011-11-254 sp -- added logging
        try:
            files = ipl.utils.file.getFileList(path, extensionMask)
            # Ignore the client's protocol database (which is stored in an
            # XML format, and is called _mru.xml)
            files = [f for f in files if not f.count(ProtocolManager.CLIENT_PROTOCOL_DB)]
        except WindowsError as msg:
            # No protocol files? Then let's set our file list to be empty and
            # see if we can read in the database
            self.logger.logError("PM: Unable to find %s files in path (%s)? %s" % \
                (extensionMask, path, msg))
            self.svrLog.logError('', self.logPrefix, funcReference, "Unable to find %s files in path (%s)? %s" % \
                (extensionMask, path, msg) )    # 2011-11-254 sp -- added logging
            files = []

        return files

    def __addProtocols(self, path, extensionMask = 'xml'):
        '''Add protocols to the manager, from either the pre-parsed and
        processed database or from raw XML protocol definition files.'''
        self.logger.logInfo("PM: About to add protocol files. Using %s." % (path))
        funcReference = __name__ + '.__addProtocols'   # 2011-11-254 sp -- added logging
        #print "###### Now in Function: %s ######" %  funcReference
        self.svrLog.logID('', self.logPrefix, funcReference, 'File to add, path=%s' % path )    # 2011-11-254 sp -- added logging
        if path in ProtocolManager.presetUserData:
            print("__getFileInfo")
            files = [ f.name for f in self.__getFileInfo(self.fileDBPath)]
        else:
            print("__getFilesFromUDB")
            #print ">>>>>> ProtocolManager.presentUserData has no key <<<<<<<"
            #print ">>>>>> Going to call __getFilesFromUDB using path: %s after 5 secs <<<<<<" % path 
            #time.sleep(5)
            files = self.__getFilesFromUDB(path, extensionMask)       
        haveProtocols = False
        #if files == [] or self.isDatabaseCurrent(files):
        
        #print "###### Now back to function: %s ######" % funcReference 
        if self.isDatabaseCurrent(files):
            self.logger.logInfo("PM: Using protocol database (%s)" % (self.protocolDBPath))
            self.svrLog.logInfo('', self.logPrefix, funcReference, "Using protocol database=%s" % (self.protocolDBPath) )    # 2011-11-254 sp -- added logging
            protocols = self.__unpickleProtocols()
            if protocols != {}:
                for protocol in list(protocols.values()):
                    self.add(protocol)
                    #self.logger.logInfo("PM: Added protocol (%s)" % (protocol.label))
                    #self.svrLog.logDebug('', self.logPrefix, funcReference, "Added protocol (%s)" % (protocol.label) )    # 2011-11-254 sp -- added logging
                    self.reportStatus("Loading "+protocol.label);
            haveProtocols = True
        # If we don't have any protocols from the pickled database, try reading
        # them in from file
        if not haveProtocols and files != []:
            self.logger.logInfo("PM: Using raw protocol files")
            self.svrLog.logInfo('', self.logPrefix, funcReference, "Using raw protocol files" )    # 2011-11-254 sp -- added logging
            self.__addProtocolFiles(files)
        self.logger.logDebug("PM: Finished adding protocol files.")
        self.svrLog.logInfo('S', self.logPrefix, funcReference, "Finished adding protocol files" )    # 2011-11-254 sp -- added logging


    def __addProtocolFiles(self, xmlFileList):
        '''Find all the protocol files in the list and add them to the manager.'''
        funcReference = __name__ + '.__addProtocolFiles'   # 2011-11-254 sp -- added logging
        count = 0
        for entry in xmlFileList:
            try:
                
                if os.path.exists(entry):
                    protocol = Protocol(fileName = entry, **self.protocol_args)
                    self.add(protocol)
                    count += 1
                    #self.logger.logInfo("PM: Added protocol (%s)" % (protocol.label))
                    #self.svrLog.logDebug('', self.logPrefix, funcReference, "Added protocol (%s)" % (protocol.label) )    # 2011-11-254 sp -- added logging
                    self.reportStatus("Loading "+protocol.label)#.decode());
            except ProtocolException as msg:
                self.logger.logError("PM: Protocol error: %s" % (msg))
                self.svrLog.logError('', self.logPrefix, funcReference, "Protocol error: %s" % (msg) )    # 2011-11-254 sp -- added logging

        if count > 0:
            # Store the protocols in the "pickled" database for faster retrieval
            self.__pickleProtocols()
        else:
            # Curious that we didn't find any protocols, let's log this...
            self.logger.logWarning("PM: No protocols found in the protocol directory?" )
            self.svrLog.logWarning('', self.logPrefix, funcReference, "No protocols found in the protocol directory?" )    # 2011-11-254 sp -- added logging


    def __unpickleProtocols(self):
        '''Unpickle our protocols. Return a protocols map, which is empty if
        there are no pickled protocols.'''
        funcReference = __name__ + '.__unpickleProtocols'   # 2011-11-254 sp -- added logging
        try:
            pf = open(self.protocolDBPath, 'rb')
            protocols = pickle.load(pf)
            pf.close()
            if protocols == None: protocols = {}
        except IOError as msg:
            self.logger.logWarning("PM: Unable to unpickle protocols: %s" % (msg))
            self.svrLog.logWarning('', self.logPrefix, funcReference, "Unable to unpickle protocols: %s" % (msg) )    # 2011-11-254 sp -- added logging
            protocols = {}
        return protocols

    def __pickleProtocols(self):
        '''Pickle our protocols and store them to disk.'''
        funcReference = __name__ + '.__pickleProtocols'   # 2011-11-254 sp -- added logging
        try:
            pf = open(self.protocolDBPath, 'wb')
            pickle.dump(self.protocols, pf)            # Store the protocols
            pf.close()
        except IOError as msg:
            self.logger.logWarning("PM: Unable to pickle protocols: %s" % (msg))
            self.svrLog.logWarning('', self.logPrefix, funcReference, "Unable to pickle protocols: %s" % (msg) )    # 2011-11-254 sp -- added logging

    # --- Helper functions ----------------------------------------------------
    
    def getNumberOfProtocols(self):
        '''Return the the number of protocols being managed'''
        return self.__len__()
    
    def remove(self, id = None, protocol = None):
        '''Remove a protocol from the manager'''
        if id:
            del ProtocolManager.protocols[id]
        else:
            del ProtocolManager.protocols[protocol.ID]

    def replace(self, protocol):
        '''Replace an existing protocol with a new version of the protocol'''
        ProtocolManager.protocols[protocol.ID] = protocol

    def clear(self):
        '''Clear all protocols out of the manager'''
        ProtocolManager.protocols.clear()

    # --- builtins ------------------------------------------------------------

    def __contains__(self, p):
        '''Is the protocol being stored in the manager? Returns true if so'''
        return p.ID in ProtocolManager.protocols

    def __len__(self):
        '''Returns the number of protocols'''
        return len(list(ProtocolManager.protocols.keys()))

    def __getitem__(self, ID):
        '''Return a protocol from the ID key. Returns None if non-existent'''
        return ProtocolManager.protocols.get(ID, None)

    # --- reporting ------------------------------------------------------------
    
    def reportStatus(self, msg, statusCode = 'TSC1002'):
        '''Simple helper function for reporting status messages.
        The default status code is TSC1000 (Unspecified Dispatcher status)'''
        self.__reportMsg(ProtocolManager.STATUS_MSG, None, statusCode, msg)

    def __reportMsg(self, type, field1, *args):
        '''Report a message (of type MESSAGE_TYPES) to the callback.
        This should be thread safe :)'''
        lck = threading.Lock()
        lck.acquire()
        if self.__reportCallback:
            self.__reportCallback(type, field1, *args)
        lck.release()


    # 2015/09/20 RL moved variables to serverconfig from hardware ini
    #apply_cmd_sub_logic = 1  If enabled, more efficient combo commands will be substituted for 2 more actions using the same tip.
    #subfor_transseptrans_onlyif_sep_is_lessthan_seconds = 60
    def getNewCmdLogicConfigurations(self):
        funcReference = __name__ + '.getNewCmdLogicConfigurations'      # 2011-11-28 sp -- added logging
        configFile = tesla.config.SERVER_CONFIG_PATH
        cfg = ConfigParser()

        self.apply_cmd_sub_logic = 1
        self.subfor_transseptrans_onlyif_sep_is_lessthan_seconds   = 60
                
        # if configuration file exists, extract the settings
        if( os.path.exists( configFile)):
            cfg.read( configFile )
            try:
                # get settings from configuration file
                apply_cmd_sub_logic   = int( cfg.get( 'new_cmd_logic', Instrument.ApplyCommandSubstitutionLogicLabel ) )
                subfor_transseptrans_onlyif_sep_is_lessthan_seconds        = int( cfg.get( 'new_cmd_logic', Instrument.SubForTransSepTransOnlyIfSepIsLessThanSecondsLabel ) )
                
                self.apply_cmd_sub_logic = apply_cmd_sub_logic
                self.subfor_transseptrans_onlyif_sep_is_lessthan_seconds   = subfor_transseptrans_onlyif_sep_is_lessthan_seconds
            except Exception as msg:
                self.svrLog.logError('', self.logPrefix, funcReference,
                                     'Error reading from configuration file [%s]...Default settings used: %s' % (configFile, msg))
        else:
            self.svrLog.logError('', self.logPrefix, funcReference,
                                     'Error opening from configuration file [%s]...Default settings used' % (configFile))

        self.protocol_args = {
            Instrument.ApplyCommandSubstitutionLogicLabel: self.apply_cmd_sub_logic,
            Instrument.SubForTransSepTransOnlyIfSepIsLessThanSecondsLabel: self.subfor_transseptrans_onlyif_sep_is_lessthan_seconds
            }
                  


# eof

