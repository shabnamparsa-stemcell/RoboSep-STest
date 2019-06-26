# 
# Centre.py
# tesla.control.Centre
#
# The control centre of the instrument controller. This is where protocols,
# samples, schedules and hardware come together to process cell separations
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
# Note:
#    03/24/05 - added parkarm and islidclosed - bdr
#    04/11/05 - added parkpump to deenergize pump - bdr
#    04/11/05 - added parkpump to power down proc - bdr
#    05/29/05 - added parkCarousel to power down - bdr
#    01/06/06 - changes to start run - bdr (iat)
#    03/21/06 - transport from buffer bottle is always from the bottom - RL
#    03/23/06 - uniform multi sample - RL
#    03/29/06 - pause command - RL
#

import string, sys, traceback
import itertools

import win32api, win32con, win32security            # Used by __executeFullShutdown()
from ipl.win32.security import AdjustPrivilege

from ipl.fsm.simple import SimpleFSM, FSM_Exception
from ipl.task.future import Future
from ipl.utils.file import getFreeDiskSpace
from ipl.utils.InfoTable import InfoTable
import ipl.utils.string
import time

import tesla.config
from tesla.config import WARNING_LEVEL, ERROR_LEVEL
import tesla.logger
from tesla.exception import TeslaException
from tesla.types.Command import VolumeCommand
from tesla.control.Scheduler import SampleScheduler, SchedulerException
from tesla.control.Dispatcher import Dispatcher, DispatcherException
from tesla.control.ProtocolManager import ProtocolManager, ProtocolException
from tesla.instrument.Instrument import Instrument
from tesla.instrument.Calibration import Calibration
from tesla.control.SampleTracker import SampleTracker
from tesla.hardware.config import gHardwareData, LoadSettings, ReloadHardwareData
from tesla.types.tube import *

from tesla.PgmLog import PgmLog     # 2011-11-28 sp -- programming logging
import tesla.DebuggerWindow          # 2012-04-10 sp
import os                           # 2011-11-28 sp -- programming logging
import fnmatch
from datetime import date

import RoboTrace              #CWJ Add
import SimpleStep.SimpleStep  #CWJ Add
import os, sys, glob          #CWJ Add 
import zipfile
import re
# -----------------------------------------------------------------------------

class ControlException(TeslaException):
    '''Control centre exceptions'''
    pass

# -----------------------------------------------------------------------------

class ContainerVolumes:
    '''Container volume details for the instrument'''

    def __init__(self, bulkBufferDeadVolume_uL):
        '''Constructor for Container Volumes'''
        self.bulkBufferDeadVolume = bulkBufferDeadVolume_uL

# -----------------------------------------------------------------------------

class Centre(object):
    '''The control centre takes samples, associated with protocols, schedules
    them and then works with the Dispatcher to have the schedule executed by
    the hardware.
    '''
    
    def __init__(self, protocolPath = '.', reportCallback = None):
        '''Control Centre object constructor. You can specify an optional path
        to a path of protocol files and a reporting callback for sending
        reports on status and errors back to the calling object.'''
        # Initialise the logger
        self.logger = tesla.logger.Logger(logName = tesla.config.LOG_PATH)

        # 2011-11-28 sp -- added logging
        self.svrLog = PgmLog( 'svrLog' )
        self.logPrefix = 'CT'

        # Build the instrument state machine
        self.__buildFSM()                   

        # Read in our status codes
        self.statusTable = InfoTable(tesla.config.STATUS_CODES_PATH)
        self.errorTable = InfoTable(tesla.config.ERROR_CODES_PATH)

        # Initialise the callback for errors/status reports
        self.__reportCallback = reportCallback

        self.protocolPath = protocolPath
        
        # Initialise the instrument
        # skip homing because init sequence does it later
        self.reInitInstrument(False)


    def reInitInstrument(self, doHome = True):
        ReloadHardwareData()
        tesla.hardware.Device.Device.reset()
        tesla.instrument.Subsystem.Subsystem.reset()
        
        # Initialise the instrument
        self.instrument = Instrument("Instrument")
        self.__updateConfiguration()
        
        # Now initialise the rest of our control layer components
        self.pm          = ProtocolManager(self.protocolPath, self.__reporter)
        self.scheduler   = SampleScheduler(self.pm, self.logger, self.instrument)
        self.__schedule  = None              # Our current schedule
        self.dispatcher  = Dispatcher(self.instrument, self.logger, self.__reporter)
        self.tracker     = SampleTracker()
        self.calibration = Calibration("Calibration", self.instrument)


        # dispatcher needs to know how many transports per flush so we need
        # to copy it over from the Scheduler
        self.dispatcher.setNumberOfTransportsPerFlush( self.scheduler.getNumberOfTransportsPerFlush() )
        
        # Do we have the minimum set of protocols ?
        self.checkRequiredProtocols()

        # needed to init axis positions for status
        if doHome:
            self.instrument.HomeAll()



    def __updateConfiguration(self):
        '''If values in the instrument hardware.ini are different to those in
        tesla.config, override the ones in tesla.config.'''
        funcReference = __name__ + '.__updateConfiguration'      # 2011-11-28 sp -- added logging
        # Let's make sure the maximum volume settings are the same
        hwVolumeLimit = int(self.instrument.getHardwareSetting('volumeaftertopup_ul'))

        if tesla.config.DEFAULT_WORKING_VOLUME_uL != hwVolumeLimit:
            tesla.config.DEFAULT_WORKING_VOLUME_uL = hwVolumeLimit
            self.logger.logInfo("CC: Resetting control layer default working volume to %d uL" % \
                                (hwVolumeLimit))
            self.svrLog.logInfo('', self.logPrefix, funcReference, "CC: Resetting control layer default working volume to %d uL" % \
                                (hwVolumeLimit))       # 2011-11-28 sp -- added logging

    # --- starting up the instrument ---
    
    def resetInstrument(self):
        '''Reset the instrument at start-up. Does not move any robotic axes.'''
        funcReference = __name__ + '.resetInstrument'      # 2011-11-28 sp -- added logging

        if not self.__assertState(SimpleFSM.START_STATE):
            state = self.getState()
            self.__reportError(ERROR_LEVEL, 'TEC1200', state)
            self.svrLog.logError('', self.logPrefix, funcReference, 'Error: TEC1200 Not in START state (actually in %s)' % state)       # 2011-11-28 sp -- added logging
            raise ControlException("CC: Not in START state (actually in %s)" % state)
        
        self.checkDiskSpace()               # Check that disk space is okay
        self.setInitialVolumeLevels()       # Set up the initial volume levels  
        self.__changeState('RESET')         # Move to RESET, ready for INIT

    def __initialiseSequence(self):
        '''Called by initialiseInstrument(), this method handles the low-level
        hardware calls for initialisation, as well as reporting status as we
        progress during the initialisation and state changes.'''
        funcReference = __name__ + '.__initialiseSequence'      # 2011-11-28 sp -- added logging

        try:
            self.instrument.TurnONBeacon(3);                    #CWJ Add
            time.sleep(3);                                      #CWJ Add
            self.instrument.TurnOFFBeacon();                    #CWJ Add
            self.instrument.Initialise()
            self.__changeState('IDLE')                          # We are now ready to process requests
            self.instrument.TurnOFFBeacon();                    #CWJ Add
            time.sleep(3);
            SplashMgr = RoboTrace.GetRoboSplashMgrInstance();            
            SplashMgr.TerminateSplash();
            
        except Exception as msg:
            # Clean up on any error that may have bubbled up from beneath 
            # And let's force the exceptions into a string format (just in case)
            self.instrument.TurnONBeacon(1);                    #CWJ Add: Catch System Crash!            
            self.svrLog.logError('', self.logPrefix, funcReference, 'Error: TEC1210(%s)' % str(msg))       # 2011-11-28 sp -- added logging
            self.__reportError(ERROR_LEVEL, 'TEC1210', str(msg))
            # And stay in the INIT state

    def initialiseInstrument(self):
        '''Client connection established. Initialise robotic axes, etc.'''
        self.__changeState('INIT')
        _ = Future(self.__initialiseSequence)

    def getContainerVolumes(self):
        '''Returns container volume details'''
        return ContainerVolumes(
            float(self.instrument.getBulkBufferDeadVolume()))
    
    # --- "power down" the instrument ---

    def shutdownInstrument(self, powerDown = True):
        '''Put the instrument into a 'shutdown' state'''
        # If we are shutting down from the RESET state, the hardware will
        # be in an unknown configuration, so don't touch it
        
        CamMgr = RoboTrace.GetRoboCamMgrInstance();
        CamMgr.TerminateRoboCam();
        
        state = self.getState()
        funcReference = __name__ + '.shutdownInstrument'      # 2011-11-28 sp -- added logging

        if state in ['SHUTDOWN', 'OFF']:
            # If we get called after we have already shutdown, ignore it
            self.logger.logInfo("Already shutdown (in %s state), skipping this step" % (state))
            self.svrLog.logInfo('', self.logPrefix, funcReference, "Already shutdown (in %s state), skipping this step" % (state))       # 2011-11-28 sp -- added logging
            return
        
        needsHome = state != 'RESET'
        self.__schedule = None
        self.__doRun('SHUTTINGDOWN','SHUTDOWN', doHomeCmd = needsHome, doPrimeCmd = False)

        # Wait for the run to complete
        while self.getState() != 'SHUTDOWN':
            time.sleep(1)

        # ver338        
        self.parkPump();
        self.parkPump()
        self.parkArm()
        self.parkCarousel()

        # Wait a sec before new state change request
        time.sleep(1)

        # Now switch the PC off (unless this option has been overridden)
        if powerDown:
            self.__changeState('OFF')
            if tesla.config.FULL_SHUTDOWN:
                self.logger.logInfo('Executing full system shutdown')
                self.svrLog.logInfo('', self.logPrefix, funcReference, 'Executing full system shutdown')       # 2011-11-28 sp -- added logging
                
                if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                    Dmp = SimpleStep.SimpleStep.GetSSExtLoggerInstance()  #CWJ Add
                    Dmp.Dump()                                            #CWJ Add
                    Dmp.DumpBackUpHistory()                               #CWJ Add

                # 2011-11-24 -- sp
                timeStarted = tesla.config.TIME_START;
                timeCompleted = tesla.PgmLog.getTimeStampPrefixString()
                svrlog = tesla.config.LOG_PATH + 'robosep.log'
                uilog  = tesla.config.LOG_PATH + 'robosep_ui.log'

                filesToCompress = [ tesla.config.LOG_PATH, 
                                    tesla.config.CAL_LOG_PATH,
                                    tesla.config.LOG_DIR + '\\robosep_ui.log'
                                  ];
    
                for file in os.listdir( tesla.config.LOG_DIR ):
                    if fnmatch.fnmatch( file, '*_*_robosep_ui.log'):
                        workFile =  tesla.config.LOG_DIR + '\\' + file;
                        filesToCompress.append( workFile )
                
                if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                   filesToCompress.append( tesla.config.LOG_DIR + '\\SSE.trm' );                
                
                zipFileName = os.path.join( tesla.config.LOG_DIR, '%slog.zip' % timeStarted )

                prvlogpath = os.path.join( tesla.config.LOG_DIR, '*.log.*' );
                for prvlogpath in glob.glob(prvlogpath):
                    if os.path.isfile(prvlogpath):
                        filesToCompress.append(prvlogpath);

                stlogpath  = os.path.join( tesla.config.LOG_DIR, '*.stlog' );
                for stlogs in glob.glob(stlogpath):
                    if os.path.isfile(stlogs):
                        filesToCompress.append(stlogs);
                
                sselogpath = os.path.join( tesla.config.LOG_DIR, 'SSE*.err' );
                for sselogs in glob.glob(sselogpath):
                    if os.path.isfile(sselogs):
                        filesToCompress.append(sselogs);

                print(( '\nCreating compressed file ' + zipFileName ))
                zFile = zipfile.ZipFile( zipFileName, "w" )
                for name in filesToCompress:
                    zFile.write(name, os.path.basename(name), zipfile.ZIP_DEFLATED)
                zFile.close()
                zFile = zipfile.ZipFile(zipFileName, "r")
                zipFileError = zFile.testzip()                

                if( zipFileError == None ):
                    totalFileSize = 0
                    totalCompressSize = 0
                    self.svrLog.close()                    
                    for info in zFile.infolist():
          ##              print( info.filename, info.file_size, info.compress_size )
                        totalFileSize += info.file_size
                        totalCompressSize += info.compress_size
                    print(( zipFileName, "contains", len(zFile.namelist()), "files;", totalFileSize, "bytes; compressed to", int(totalCompressSize*100/totalFileSize ), "%" ))
                    print ( 'No errors encountered during compression, Original files are being removed.' )
                    
                    roboSepLogPath = os.path.join( tesla.config.LOG_DIR, 'robosep.log.*' );
                    for filePath in glob.glob(roboSepLogPath):
                        if os.path.isfile(filePath):
                            os.remove(filePath);
            
                    sselogpath = os.path.join( tesla.config.LOG_DIR, 'SSE*.err' );
                    for sselogs in glob.glob(sselogpath):
                        if os.path.isfile(sselogs):
                              os.remove(sselogs);
            
                    roboSepUiLogPath = os.path.join( tesla.config.LOG_DIR, '*_*_robosep_ui.log' );
                    for filePath in glob.glob(roboSepUiLogPath):
                        if os.path.isfile(filePath):
                              os.remove(filePath);
                            
                    stlogpath = os.path.join( tesla.config.LOG_DIR, '*.stlog' );
                    for stlogs in glob.glob(stlogpath):
                        if os.path.isfile(stlogs):
                            try:
                              os.remove(stlogs);
                            except:
                              pass;  
                tesla.config.FULL_SHUTDOWN_IN_PROGRESS = True;
                self.__executeFullShutdown()
        else:
            self.logger.logInfo('Power down on shutdown disabled')
            self.svrLog.logInfo('', self.logPrefix, funcReference, 'Power down on shutdown disabled')       # 2011-11-28 sp -- added logging

    def __executeFullShutdown(self):
        '''Execute a full shutdown of the PC'''
        # Note: on Win XP, we could just call shutdown.exe, but this won't work 
        #       on development machines that are running Windows 2000 
        _ = AdjustPrivilege(win32security.SE_SHUTDOWN_NAME)
        win32api.ExitWindowsEx(win32con.EWX_SHUTDOWN)
    

    # --- methods handling protocols ---

    #CR - added function
    def getCustomNames(self):
        return self.pm.getCustomNames()
    
    def getResultChecks(self):
        return self.pm.getResultChecks()

    # RL - uniform multi sample -  03/23/06
    #def getMultipleSamples(self):
    #    return self.pm.getMultipleSamples()

    def isPauseCommand( self ):
        return self.instrument.isPauseCommand()
        
    def getPauseCommandCaption( self ):
        return self.instrument.getPauseCommandCaption()
    
    def getProtocols(self):
        '''Return a list of client protocols'''
        return self.pm.getProtocolsForClient()

    def calculateConsumables(self, protocolID, sampleVolume):
        '''Return a map of consumables needed for the specified protocol and 
        sample volume.'''
        funcReference = __name__ + '.calculateConsumables'      # 2011-11-28 sp -- added logging

        try:
            info = self.pm.getConsumableInformation(protocolID, sampleVolume)
        except ProtocolException as msg:
            # In case of error, log the error and return an empty map 
            self.svrLog.logWarning('', self.logPrefix, funcReference, 'TEC1240 [%s]' % msg )       # 2011-11-28 sp -- added logging
            self.__reportError(WARNING_LEVEL, 'TEC1240', msg)
            info = {}
        except Exception as msg:
            # If we hit this, we have an unexpected error; log it
            self.svrLog.logWarning('', self.logPrefix, funcReference, 'TEC1241 [%s]' % msg )       # 2011-11-28 sp -- added logging
            self.__reportError(WARNING_LEVEL, 'TEC1241', msg)
            info = {}
        return info

    def reloadProtocols(self):
        return self.pm.reloadProtocols()

    # --- scheduling, starting and stopping runs ---

    def scheduleRun(self, sampleList, sharedSectorsTranslation = []):
        '''Schedule the samples, returning an estimated time to completion in
        a nicely formatted ISO 8601 format.'''

        samples = self.filterSamples(sampleList)

        funcReference = __name__ + '.scheduleRun'      # 2011-11-28 sp -- added logging

        # Log the sample information
        numSamples = len(samples)
        sampleCounter = itertools.count(1)
        for sample in samples:                
            protocol = self.pm.getProtocol(sample.protocolID)
            
            sampleInfo = "%s (%s) V = %0.2fuL" % (sample.ID, sample.label, sample.volume)
            self.logger.logInfo("CC: Scheduling sample #%d of %d: '%s' [using %s]" % \
                                (next(sampleCounter), numSamples, sampleInfo, protocol.label))

        self.svrLog.logID('', self.logPrefix, funcReference, "Scheduling [%s] |[%s]" % \
                                (protocol.label, sampleInfo ))       # 2011-11-28 sp -- added logging
        
        
        self.__schedule = None          # Force the schedule to be null by default
        
        try:
            self.scheduler.schedule(samples, sharedSectorsTranslation)
            print('\n\tLeave schedule\n')
        except SchedulerException as msg:
            self.svrLog.logWarning('', self.logPrefix, funcReference, 'TEC1222 schedule' )      # 2011-11-28 sp -- added logging
            self.__reportError(WARNING_LEVEL, 'TEC1222')
        try:
            self.__schedule = self.scheduler.getSchedule()    
            print('\n\tLeave getschedule\n')            
        except SchedulerException as msg:
            self.svrLog.logWarning('', self.logPrefix, funcReference, 'TEC1220 getSchedule[%s]' % msg )       # 2011-11-28 sp -- added logging
            self.__reportError(WARNING_LEVEL, 'TEC1220', msg)
        return self.scheduler.getETC()

    def noSchedule(self):
        '''Return True if we have no schedule, False otherwise; a nice bit of
        inverted logic! This is really just for use by startRun().
        The __schedule member should be either set to None (if there is no
        valid schedule at the moment) or a SampleSchedule object.'''
        return self.__schedule == None 
   
    def getETC(self):
        '''Return the current ETC (estimated time to completion).
        Note that you will get an ETC of *now* if the scheduler fails to 
        schedule the samples.'''
        return self.scheduler.getETC()
  
    def __determineWasteVialQuadrant(self, sampleList):
        '''Returns the quadrant number for the waste vial, if required. If the
        waste vial isn't needed or can't be determined, None is returned.'''
        wasteQuads = []                 # List of quadrants holding a waste vial
        for sample in sampleList:

            pcList = self.calculateConsumables(sample.protocolID, sample.volume)
            for pc in pcList:
                if pc.wasteVesselRequired:
                    wasteQuads.append(sample.initialQuadrant) 
        wasteQuads.sort()
        if wasteQuads == []:
            return None
        else:
            return wasteQuads[0]            # Use the first of the quadrants
  
        
    def updateTubeSizeToHandleCustVials(self, samples):
            funcReference = __name__ + '.updateTubeSizeToHandleCustVials'      

            #2017-04-29 RL
            #update tube sizes to handle custom reagent vial volumes 
            self.logger.logInfo("CC: update fulltubemap to handle custom reagent vial volumes")
            self.svrLog.logInfo('', self.logPrefix, funcReference, "update fulltubemap to handle custom reagent vial volumes" )
            self.instrument.InitialiseContainers()
            samplesWithVolumes = samples[:]
            initQ = 1
            for sample in samplesWithVolumes:
                protocol = self.pm.getProtocol(ID=sample.protocolID)

                for feat in protocol.getFeatures():
                    self.logger.logInfo("CC: DEBUG: feature name = %s" % (feat.name))

                #use defaults from hardware.ini
                Filled2mlVialVolume = self.instrument.GetFilled2mlVialVolume()        
                sample.cocktailFilledVolume = Filled2mlVialVolume 
                sample.particleFilledVolume = Filled2mlVialVolume 
                sample.antibodyFilledVolume = Filled2mlVialVolume
                
                customVialSizeFeature = protocol.findFeature("RSSCustVial")
                if not customVialSizeFeature == None:

                    vialSize = 0
                    vialProfileVol = 0
                    vialProfileHeight = 0
                    
                    inputData = re.findall('[-+]?\d+', customVialSizeFeature.inputData)
                    inputDataLength = len(inputData)
                    if inputDataLength>0:
                        vialSize = int(inputData[0])
                    if inputDataLength>1:
                        vialProfileVol = int(inputData[1])
                    if inputDataLength>2:
                        vialProfileHeight = int(inputData[2])
                        
                    if vialSize > 0 and vialProfileVol > 0 and vialProfileHeight >0:
                        self.logger.logInfo("CC: DEBUG: RSSCustVial filledVialVolume_uL=%d profileVol_uL=%d profileHeight_mm=%d id=%s initq=%d" % (vialSize,vialProfileVol,vialProfileHeight,sample.protocolID,sample.initialQuadrant))

                        sample.cocktailFilledVolume = vialSize 
                        sample.particleFilledVolume = vialSize 
                        sample.antibodyFilledVolume = vialSize 

                        #loop protocol.numQuadrants and update reagent vial sizes
                        for i in range(protocol.numQuadrants):
                            sector = sample.initialQuadrant+i
                            self.logger.logInfo("CC: DEBUG: replace quad=%d" % (sector))
                            sector = sample.initialQuadrant+i
                            
                            name = self.instrument.AntibodyLabel
                            newVial = self.instrument.makeCustomSizeReagentVialGeometry(name, vialProfileVol, vialProfileHeight)
                            self.instrument.ReplaceContainerAt(sector,name,Tube (sector,name , newVial))
                            
                            name = self.instrument.CocktailLabel
                            newVial = self.instrument.makeCustomSizeReagentVialGeometry(name, vialProfileVol, vialProfileHeight)
                            self.instrument.ReplaceContainerAt(sector,name,Tube (sector,name , newVial))
                            
                            name = self.instrument.BeadLabel
                            newVial = self.instrument.makeCustomSizeReagentVialGeometry(name, vialProfileVol, vialProfileHeight)
                            self.instrument.ReplaceContainerAt(sector,name,Tube (sector,name , newVial))
                            


    def CheckSampleReagentRequiredVolumeAgainstFilledVolume(self, samples):
        #error message should use custom names if available
        
        #Q? X vial requires Y uL but vial only contains Z uL
        errMsg = "Q%d %s requires %duL but vial only contains %duL"
        
        # Create a copy of the samples that are being processed
        samplesWithVolumes = samples[:]
        for sample in samplesWithVolumes:
            tmpQuadrants = len(sample.consumables)
            for j in range(tmpQuadrants):
                section = sample.initialQuadrant+j
                qIdx = j
                if sample.consumables[j].particleVolume > sample.particleFilledVolume:
                    vialName = self.GetVialName(sample.protocol,qIdx,Instrument.BeadLabel)
                    return errMsg % (section, vialName, sample.consumables[j].particleVolume, sample.particleFilledVolume)
                if sample.consumables[j].cocktailVolume > sample.cocktailFilledVolume:
                    vialName = self.GetVialName(sample.protocol,qIdx,Instrument.CocktailLabel)
                    return errMsg % (section, vialName, sample.consumables[j].cocktailVolume, sample.cocktailFilledVolume)
                if sample.consumables[j].antibodyVolume > sample.antibodyFilledVolume:
                    vialName = self.GetVialName(sample.protocol,qIdx,Instrument.AntibodyLabel)
                    return errMsg % (section, vialName, sample.consumables[j].antibodyVolume, sample.antibodyFilledVolume)
        return ""
            
    def GetVialName(self, protocol, qIdx, label):
        customNames = []
        try:
            customNames = protocol.getCustomNames()
        except:
            self.logger.logDebug("CC: GetVialName: failed to get custom name from protocol")    
            

        #customNames is list of string of 8*len(sample.consumables)
        Labels = [Instrument.BCCMLabel,
           Instrument.WasteLabel,
           Instrument.LysisLabel,
           Instrument.CocktailLabel,
           Instrument.BeadLabel,
           Instrument.AntibodyLabel,
           Instrument.SampleLabel,
           Instrument.SupernatentLabel
           ]
        DefaultNames = [self.instrument.GetDefaultVialName(Instrument.BCCMLabel),
                    self.instrument.GetDefaultVialName(Instrument.WasteLabel),
                    self.instrument.GetDefaultVialName(Instrument.LysisLabel),
                    self.instrument.GetDefaultVialName(Instrument.CocktailLabel),
                    self.instrument.GetDefaultVialName(Instrument.BeadLabel),
                    self.instrument.GetDefaultVialName(Instrument.AntibodyLabel),
                    self.instrument.GetDefaultVialName(Instrument.SampleLabel),
                    self.instrument.GetDefaultVialName(Instrument.SupernatentLabel)
                        ]
        
        try:
            label_idx = Labels.index(label)
            idx = qIdx*8+ label_idx
            
            #print "ZZZZZZZZZZZZZZZ",label_idx,idx, qIdx,len(customNames),customNames
            if idx >= len(customNames):
                self.logger.logDebug("CC: GetVialName: customNames array size too small idx="+str(idx)+" len(customNames)="+str(len(customNames)))
                return DefaultNames[label_idx]
            else:
                vialName = customNames[idx]
                if vialName=='':
                    return DefaultNames[label_idx]
                else:
                    return vialName
        except:
            self.logger.logDebug("CC: GetVialName: label not in list="+str(label))    
            return label

        
    def SetupVolumeForStartRun(self, samples, isSharing, sharedSectorsTranslation, sharedSectorsProtocolIndex):
                # Create a copy of the samples that are being processed
                samplesWithVolumes = samples[:]
                # Populate the samples with consumable information
                for sample in samplesWithVolumes:
                    protocol = self.pm.getProtocol(ID=sample.protocolID)
                    sample.protocol = protocol

                    #Test code here!!!

        
                    
                    #Test code here!!!
                    
                    sample.protocolLabel = protocol.label
                    
                    sample.consumables = protocol.getVolumes(sample.volume, protocol.type)
                    #if sharing AND not leeching
                    if isSharing and len(sharedSectorsTranslation) == 4 and sharedSectorsTranslation[sample.initialQuadrant-1]==0:
                        for i in range(4):
                            #Q(i+1) is sharing with this quadrant
                            if sample.initialQuadrant == sharedSectorsTranslation[i]:
                                tmpSample = samplesWithVolumes[sharedSectorsProtocolIndex[i]]
                                tmpProtocol = self.pm.getProtocol(ID=tmpSample.protocolID)
                                tmpConsumables = tmpProtocol.getVolumes(tmpSample.volume, tmpProtocol.type)
                                tmpQuadrants = len(tmpConsumables)
                                for j in range(tmpQuadrants):
                                    A = tmpConsumables[j].particleVolume
                                    B = tmpConsumables[j].cocktailVolume
                                    C = tmpConsumables[j].antibodyVolume
                                    if A>0:
                                        sample.consumables[j].particleVolume+= A-100
                                    if B>0:
                                        sample.consumables[j].cocktailVolume+= B-100
                                    if C>0:
                                        sample.consumables[j].antibodyVolume+= C-100
                    elif isSharing:
                        tmpQuadrants = len(sample.consumables)
                        for j in range(tmpQuadrants):
                             sample.consumables[j].particleVolume= 0
                             sample.consumables[j].cocktailVolume= 0
                             sample.consumables[j].antibodyVolume= 0
                        
                    for j in range(len(sample.consumables)):
                        self.logger.logDebug("CC: Sharing adjusted volume: particleVolume="+str(sample.consumables[j].particleVolume)+
                                            " cocktailVolume="+str(sample.consumables[j].cocktailVolume)+
                                            " antibodyVolume="+str(sample.consumables[j].antibodyVolume))                
                    #sample.consumables = protocol.getVolumes(sample.volume) - (3.3.9) bdr
                return samplesWithVolumes
                    
    def startRun(self, sampleList, userID = 'undefined',
                 isSharing = False, sharedSectorsTranslation = [], sharedSectorsProtocolIndex = []):
        '''Start the scheduled run for the specified samples. If a run hasn't
        been scheduled yet, schedule it, then start the run.
        An optional user/operator ID can be supplied.
        Returns the ETC.'''

        self.logger.logInfo("CC: startRun() for %s user" % (userID,))
        funcReference = __name__ + '.startRun'      # 2011-11-28 sp -- added logging
        self.svrLog.logID('', self.logPrefix, funcReference, "userID=%s" % (userID))       # 2011-11-28 sp -- added logging

        state = self.getState()
        etc = ''
        self.instrument.ClearRootQuadrantMapping()
   
        # Nov 7, 2013 Sunny
        # Reset seq so that it starts from 0
        CamMgr = RoboTrace.GetRoboCamMgrInstance()
        CamMgr.SetCurrentProtocolSeq(0)
        
        if not self.__assertState('IDLE'):
            self.__reportError(ERROR_LEVEL, 'TEC1202', state)
            self.svrLog.logError('', self.logPrefix, funcReference, 'TEC1202: CC: Not in IDLE state (actually in %s)' % (state))       # 2011-11-28 sp -- added logging
            raise ControlException("CC: Not in IDLE state (actually in %s)" % (state))
        else:
            samples = self.filterSamples(sampleList)
            
            # If we have an empty schedule, schedule the samples
            if self.noSchedule():
                self.logger.logDebug('CC: No schedule before starting run')
                self.svrLog.logInfo('S', self.logPrefix, funcReference, 'CC: No schedule before starting run')       # 2011-11-28 sp -- added logging
                self.__schedule = self.scheduler.getSchedule()                        
                if self.noSchedule():                    
                    # Still no schedule? This indicates a serious problem
                    self.__reportError(ERROR_LEVEL, 'TEC1221')
                    self.logger.logDebug("CC: Can't schedule, abandoning startRun()")
                    self.svrLog.logErrror('', self.logPrefix, funcReference, "CC: Can't schedule, abandoning startRun()")       # 2011-11-28 sp -- added logging
                    raise ControlException('CC: Empty schedule for samples after 2 tries?')
                else:
                    self.logger.logDebug("CC: Scheduled before run; ETC = %s" % (etc))              
                    self.svrLog.logDebug('', self.logPrefix, funcReference, "CC: Scheduled before run; ETC = %s" % (etc))       # 2011-11-28 sp -- added logging
            else:
                # Get an updated ETC from the scheduler
                etc = self.scheduler.getETC()
                self.logger.logInfo("CC: startRun() ETC = %s" % (etc))
                self.svrLog.logInfo('', self.logPrefix, funcReference, "ETC = %s" % (etc))       # 2011-11-28 sp -- added logging

            self.updateTubeSizeToHandleCustVials(samples)      


            # Now set the volume levels and set the waste vial
            self.setVolumeLevels(samples)

            wasteQuadrant = self.__determineWasteVialQuadrant(samples)
            if wasteQuadrant:
                self.dispatcher.setWasteVialLocation(wasteQuadrant)
            else:
                #need to refresh the waste vial location whether one is needed in the protocol or not
                #otherwise it will just use the waste vial in the previous run with stale volume info
                self.dispatcher.setWasteVialLocation(1)  

            # Now start either a shutdown or a 'normal' run
            if self.__shutdownRequested(samples):
                # If this is a shutdown protocol, starting shutting down
                self.logger.logDebug('CC: About to run shutdown protocol')
                self.svrLog.logInfo('S', self.logPrefix, funcReference, 'CC: About to run shutdown protocol')       # 2011-11-28 sp -- added logging
                self.shutdownInstrument()
            else:
                #sharing info
                self.logger.logInfo("CC: startRun() sharingInfo "+str(isSharing) + " " \
                                    +str(sharedSectorsTranslation)+ " "+ str(sharedSectorsProtocolIndex) )
                self.svrLog.logInfo('', self.logPrefix, funcReference, "sharingInfo "+str(isSharing) + " " \
                                    +str(sharedSectorsTranslation)+ " "+ str(sharedSectorsProtocolIndex))       # 2011-11-28 sp -- added logging
                
                # Set up the sample tracker
                self.tracker.resetTracker()
                                    
                samplesWithVolumes = self.SetupVolumeForStartRun(samples,isSharing, sharedSectorsTranslation, sharedSectorsProtocolIndex)
                    
                # 2013-01-16 -- sp  add code to overwrite vial IDs if they exist
                #if tesla.config.OVERWRITE_WITH_BARCODES == 1:
                #    samples = self.overWriteBarcodedReagentID( samples )

                # Sharing reagent prep 
                self.logger.logDebug("CC: isSharing "+str(isSharing))
                self.svrLog.logInfo('S', self.logPrefix, funcReference, "isSharing "+str(isSharing))       # 2011-11-28 sp -- added logging
                self.setVolumeLevels(samplesWithVolumes)
                
                #rework schedule for sharing
                if isSharing:
                    self.scheduleRun(samplesWithVolumes,sharedSectorsTranslation)
                else:
                    #or just update it with correct sample IDs
                    self.scheduleRun(samplesWithVolumes)
                    
                
                # Add the samples to the report & mark the report as "in progress"
                self.tracker.addSamples(samplesWithVolumes)
                self.tracker.scheduleStart(etc,operator=userID)

                # Now work out if we need to insert HomeAllAxes and Prime/Flush
                # commands to the start of the run
                needsHome = False
                needsPrime = False
                for sample in sampleList:
                    if self.pm.isSeparationProtocol(sample.protocolID):
                        # Bingo. Update the ETC to accomodate the time taken
                        # for the additional commands as well
                        needsHome = True
                        needsPrime = True
                        padding = self.scheduler.getCommandTypeTimes('HomeAll')[1] + \
                                    self.scheduler.getCommandTypeTimes('Prime')[1] + \
                                    self.scheduler.getCommandTypeTimes('Flush')[1]                                    
                        etc = self.scheduler.getETC(padding)
                        break
                # Start the 'normal' run

                self.__doRun('RUNNING', 'IDLE', needsHome, needsPrime)
                print("StartRun let go of control")

        return etc

    def pauseRun(self):
        '''Pause the currently executing run. Returns True if it could be 
        done safely.'''
        print("!!!!!!!!!!!!!!!!!!!!!!!!!!!RECV PAUSERUN MSG",self.getState())
        if self.getState() == 'RUNNING': 
            return self.dispatcher.pause()
        else:
            return False

    def resumeRun(self):
        '''Resume a paused run. Returns True if the run could be resumed.'''
        print('\n >>> resumeRun \n')
        if self.getState() == 'PAUSED' or self.getState() == 'PAUSECOMMAND':
            return self.dispatcher.resume()
        else:
            return False

    def parkArm(self):
        '''Park moves all hardware back to a safe state.
        Returns True if the run could be halted...bdr '''
        self.logger.logDebug("bdr: centre.parkArm") 
        funcReference = __name__ + '.parkArm'      # 2011-11-28 sp -- added logging
        self.svrLog.logInfo('', self.logPrefix, funcReference, "start parkArm")       # 2011-11-28 sp -- added logging
        return self.instrument.parkArm()

    # ver338
    def parkCarousel(self):
        '''Park moves carousel a safe state past opto 
        Returns True if the run could be halted...bdr '''
        self.logger.logDebug("bdr: centre.parkCarousel") 
        funcReference = __name__ + '.parkCarousel'      # 2011-11-28 sp -- added logging
        self.svrLog.logInfo('', self.logPrefix, funcReference, "start parkCarousel")       # 2011-11-28 sp -- added logging
        return self.instrument.parkCarousel()


    # 2012-02-01 -- sp added support
    def InitScanVialBarcode( self, freeaxis ):
        rtn = self.instrument.InitScanVialBarcode( freeaxis )
        return rtn
    
    # 2014-08-01 -- twong added support
    def AbortScanVialBarcode( self ):
        self.instrument.AbortScanVialBarcode()
        
    # 2014-08-87 -- twong added support
    def IsBarcodeScanningDone( self ):
        rtn = self.instrument.IsBarcodeScanningDone()
        return rtn;

    # 2012-02-01 -- sp added support
    def ScanVialBarcodeAt(self, Quadrant, Vial):
        rtn = self.instrument.ScanVialBarcodeAt(Quadrant, Vial)
        return rtn;

    # 2012-02-01 -- sp added support
    def GetVialBarcodeAt( self, Quadrant, Vial):
        rtn = self.instrument.GetVialBarcodeAt(Quadrant, Vial)
        return rtn;
    
    def GetVialBarcodeStatusAt( self, Quadrant, Vial):
        rtn = self.instrument.GetVialBarcodeStatusAt(Quadrant, Vial)
        return rtn;

    def AttachBarcodeReader(self):
        self.instrument.AttachBarcode();
        
    def DetachBarcodeReader(self):
        self.instrument.DetachBarcode();

    def parkPump(self):
        '''Park puts the pump valve in de-energized mode.
        Returns True if the valve could be parked...bdr '''
        self.logger.logDebug("bdr: centre.parkPump") 
        funcReference = __name__ + '.parkPump'      # 2011-11-28 sp -- added logging
        self.svrLog.logInfo('', self.logPrefix, funcReference, "start parkPump")       # 2011-11-28 sp -- added logging
        return self.instrument.parkPump()

    def haltRun(self):
        '''Halt the current run. Waits until the current action is complete,
        halts the run and then moves all hardware back to a safe state.
        Returns True if the run could be halted.'''
        print('\n >>> haltRun \n')
        return self.dispatcher.halt()

    def abortRun(self):
        '''Abort (or ESTOP) the current run. Hardware is left in an "as-is" 
        state. Returns True if the run could be halted.'''
        print('\n >>> abortRun \n')
        return self.dispatcher.estop()

    def isLidClosed( self ):
        '''Returns true if the lid is closed - bdr'''
        return self.instrument.isLidClosed()
        
    def SetIgnoreLidSensor(self, sw):                     #CWJ
        self.instrument.SetIgnoreLidSensor(sw);
    
    def GetIgnoreLidSensor(self):                         #CWJ
        return self.instrument.GetIgnoreLidSensor();

    def SetIgnoreHydraulicSensor(self, sw):               #CWJ
        self.instrument.SetIgnoreHydraulicSensor(sw);
        
    def GetIgnoreHydraulicSensor(self):                   #CWJ
        return self.instrument.GetIgnoreHydraulicSensor();

    def isLowHydraulicLevel( self ):                      #CWJ
        '''Returns true if the hydraulic fluid is low'''
        return self.instrument.isLowHydraulicLevel()

    def TurnOFFBeacon( self ):                            #CWJ
        self.instrument.TurnOFFBeacon();
        
    def TurnONBeacon(self, evt):                          #CWJ
        self.instrument.TurnONBeacon(evt);
        
    def isHydroFluidLow( self ):
        '''Returns true if the hydraulic fluid is low'''
        return self.instrument.isHydroFluidLow()

    def setHydroFluidFull( self ):
        '''Sets the hydraulic fluid to full capacity'''
        self.instrument.setHydroFluidFull()

    def getEndTimeSpan( self ):
        '''Returns the span between end times of the last calculated schedule'''
        return self.scheduler.getEndTimeSpan()

    def getEndTimeSpanThreshold( self ):
        '''Returns the end time span threshold'''
        return self.scheduler.getEndTimeSpanThreshold()

    def __shutdownRequested(self, samples):
        '''Returns True if a shutdown has been requested (one of the samples
        has been assigned a shutdown protocol). Note that expect there to
        be only one sample in this case; any more doesn't make sense.'''
        for sample in samples:
            protocol = self.pm.getProtocol(sample.protocolID)
            if protocol.type == tesla.config.SHUTDOWN_PROTOCOL:
                request = True
                numSamples = len(samples)
                if numSamples != 1:
                    self.__reportError(ERROR_LEVEL, 'TEC1250', numSamples)
                break
        else:
            request = False
        return request

    def __doRun(self, startState, endState, doHomeCmd = True, doPrimeCmd = True):
        '''Start the run going. If doHomeCmd and/or doPrimeCmd is set, the 
        appropriate commands will be executed first.'''
        
        self.__changeState(startState)
        try:
            self.dispatcher.processSchedule(schedule     = self.__schedule, 
                                            activeState  = startState,
                                            pauseState   = 'PAUSED',
                                            stoppedState = 'HALTED',
                                            estopState   = 'ESTOP',
                                            endState     = endState,
                                            needsHome    = doHomeCmd,
                                            needsPrime   = doPrimeCmd)
            # The dispatcher will spin out a thread to do the schedule
            # processing, leaving us to return to other work
            # When the dispatcher is finished, it will report a new
            # state back to us -- we catch this in __reporter() and then
            # change state appropriately

        except Exception as msg:
            self.instrument.TurnONBeacon(1);         #CWJ Add: Catch System Crash!
            self.__reportError(ERROR_LEVEL, 'TEC1230', msg)
            self.__changeState(endState)
            raise ControlException("CC: startRun() failed: %s" % (msg))

    # --- state-related methods ---

    def __buildFSM(self):
        '''Private method: build up the instrument state machine'''
        # NOTE: If you add a new state, make sure that it has an entry in the
        #       status code table file (statusConfig.txt), otherwise bad things
        #       could happen when you try to report the new state back to clients
        stateTable = {}  
        stateTable[SimpleFSM.START_STATE] = ['RESET']
        stateTable['RESET']               = ['INIT', 'SHUTTINGDOWN', 'OFF']
        stateTable['INIT']                = ['IDLE', 'SERVICE']
        stateTable['IDLE']                = ['RUNNING', 'SHUTTINGDOWN', 'SERVICE']
        stateTable['RUNNING']             = ['IDLE', 'PAUSED', 'HALTED', 'PAUSECOMMAND']
        stateTable['PAUSED']              = ['RUNNING', 'SHUTTINGDOWN', 'HALTED']
        stateTable['HALTED']              = ['IDLE', 'SHUTTINGDOWN']
        stateTable['SHUTTINGDOWN']        = ['PAUSED', 'SHUTDOWN', 'HALTED']
        stateTable['SHUTDOWN']            = ['OFF']
        stateTable['SERVICE']             = ['IDLE']
        stateTable['PAUSECOMMAND']        = ['RUNNING', 'SHUTTINGDOWN', 'HALTED']
        self.__fsm = SimpleFSM(stateTable)
        self.__fsm.addGlobalState('ESTOP')
        self.__fsm.addTransition('ESTOP', 'SHUTDOWN')
        self.__fsm.addTransition('OFF', SimpleFSM.END_STATE)

    def __assertState(self, state):
        '''Verify that we are in the right state for the current action.'''
        return self.__fsm.state == state

    def getState(self):
        '''Return the current instrument state'''
        return self.__fsm.state

    def enterServiceState( self ):
        '''Changes the instrument state from INIT or IDLE to SERVICE'''
        status = False      # By default, assume that we will fail :)
        funcReference = __name__ + '.enterServiceState'      # 2011-11-28 sp -- added logging

        try:
            self.__changeState( 'SERVICE' )
            status = True
        except FSM_Exception as msg:
            self.logger.logWarning("CC: Invalid state change: %s" % (msg))
            self.svrLog.logWarning('', self.logPrefix, funcReference, "CC: Invalid state change: %s" % (msg))       # 2011-11-28 sp -- added logging
        return status

    def exitServiceState( self ):
        '''Changes the instrument state to SERVICE to IDLE'''
        if self.getState() != 'SERVICE':
            return False
        else:
            self.__changeState( 'IDLE' )
            return True
        
   
    def __changeState(self, newState):
        '''Change state and report the new state'''
        # Note that assigning to the FSM state property causes it to change
        currentState = self.getState()
        self.__fsm.state = newState
        self.logger.logDebug("CC: State change: %s -> %s" % (currentState, newState))
        funcReference = __name__ + '.__changeState'      # 2011-11-28 sp -- added logging
        self.svrLog.logInfo('', self.logPrefix, funcReference, "From %s -> %s" % (currentState, newState))       # 2011-11-28 sp -- added logging

        try:
            newStateStatusCode = self.statusTable.getCode(newState)
            self.instrument.setLocalCopyOfState( newState )
            self.__reportStatus(newStateStatusCode)
        except:
            # If we get here, it most likely means that the state doesn't have
            # an entry in the status code config file
            self.svrLog.logWarning('', self.logPrefix, funcReference, "TEC1203 setLocalCopyOfState[%s]" % (newState))       # 2011-11-28 sp -- added logging
            self.__reportError(WARNING_LEVEL, 'TEC1203', newState)

        # make it easy for the user to spin the carousel for loading
        # we will home the axis at the start of a protocol
        if newState == 'IDLE':
            self.instrument.powerDownCarousel()

        if( tesla.config.SS_DEBUGGER_LOG == 1 ):
            ssLogDebugger = tesla.DebuggerWindow.GetSSTracerInstance()
            if( newState in [ 'IDLE', 'PAUSED', 'SERVICE' ] ):
                ssLogDebugger.EnableDebugExecuteButton()
            else:
                # ssLogDebugger.DisableDebugExecuteButton()
                pass                                        #2016-07-08 CWJ EZD Test  

        # If we get to HALTED, drop back to IDLE
        if newState == 'HALTED':
            self.__changeState('IDLE')


    # --- reporter-related methods ---

    def registerReporter(self, reportCallback):
        '''Register a function/method that we can use as a callback for 
        reporting status, error or state-change messages.'''
        self.__reportCallback = reportCallback

    def expandMsg(self, table, lookupCode, *args):
        '''Parse the arguments for a code, look up the InfoTable for the 
        message and expand it with the rest of the arguments.
        Returns the expanded message as a string.'''

        # Find the message that corresponds to this code
        try:
            msg = table.getMessage(lookupCode)
        except Exception as errMsg:
            # This is not good: it means our table is corrupt or out of date?
            return "LOOKUP FAILURE for code = %s: %s" % (lookupCode, errMsg)
        
        # Fill in the various parameters to get a complete string
        try:
            expandedMsg = ipl.utils.string.expandCSharpString(msg, args)
        except Exception as errMsg:
            # This isn't good -- we screwed up with the number of arguments 
            # we passed in?
            # However it happened, log the problem and create a string that we
            # can use until the coding problem can be fixed
            expandedMsg = "%s:%s" % (msg, str(args))
            self.logger.logWarning("CC: expandMsg failed (%s) for %s [code = %s]" % \
                    (errMsg, expandedMsg, lookupCode))
            funcReference = __name__ + '.expandMsg'      # 2011-11-28 sp -- added logging
            self.svrLog.logWarning('', self.logPrefix, funcReference, "CC: expandMsg failed (%s) for %s [code = %s]" % \
                    (errMsg, expandedMsg, lookupCode))       # 2011-11-28 sp -- added logging


        return expandedMsg

    def __reportStatus(self, statusCode, *args):
        '''Report status to our report callback function.'''
        logMsg = self.expandMsg(self.statusTable, statusCode, *args)
        self.logger.logInfo(logMsg)
        funcReference = __name__ + '.__reportStatus'      # 2011-11-28 sp -- added logging
        self.svrLog.logID('', self.logPrefix, funcReference, logMsg)       # 2011-11-28 sp -- added logging
#        self.pauseRun()

        if self.__reportCallback != None:
            self.__reportCallback('Status', self.getState(), statusCode, *args)
        
    def __reportError(self, level, errorCode, *args):
        '''Log the error and then (if configured) report an error message to 
        our report callback function.
        An exception traceback is also logged (at DEBUG level) in the log file.'''
        # Expand the message and log at the appropriate level        
        logMsg = self.expandMsg(self.errorTable, errorCode, *args)
        funcReference = __name__ + '.__reportError'      # 2011-11-28 sp -- added logging

        if level == WARNING_LEVEL:
            self.logger.logWarning(logMsg)
            self.svrLog.logWarning('', self.logPrefix, funcReference, logMsg)       # 2011-11-28 sp -- added logging
        else:
            # We are reporting at the ERROR_LEVEL
            # If we got to here, then it's grim. Let's log the exception 
            # traceback to help with debugging
            limit = None
            errType, value, tb = sys.exc_info()
            tbList = traceback.format_tb(tb, limit) + traceback.format_exception_only(errType, value)            
            if tb != None and tbList[1] != "KeyError: u'startRun'\n":
                # If there's traceback data, log the exception traceback
                # We don't do this if it's the bug noted in TT#485 or if there
                # is no exception traceback available
                tracebackMsg = "Error traceback:\n" + "%-20s %s" % (string.join(tbList[:-1], ""), tbList[-1])
                self.logger.logDebug(tracebackMsg)                      
                self.svrLog.logInfo('S', self.logPrefix, funcReference, tracebackMsg)       # 2011-11-28 sp -- added logging
            self.logger.logError(logMsg)
            self.svrLog.logError('', self.logPrefix, funcReference, logMsg)       # 2011-11-28 sp -- added logging
        # Do the callback (if a handler is registered)
        if self.__reportCallback != None:
            self.__reportCallback('Error', level, errorCode, *args)


    def __reporter(self, reportType, field1, field2, *args):
        '''Private method that we use as a callback into the Dispatcher for
        reporting status or error messages back to us. We then handle these
        messages appropriately.
        The type determines which one we call; by having a type parameter, we
        can expand this in the future if we want. It also means that we don't
        have to register multiple callbacks to handle different types of
        reports.'''
        assert reportType in Dispatcher.MESSAGE_TYPES
        funcReference = __name__ + '.__reporter'      # 2011-11-28 sp -- added logging
        if reportType == Dispatcher.STATE_MSG:
            # Received a state transition message; change state
            transition = field1         # We don't use field2
            self.logger.logDebug("Received %s transition via callback" % transition)
            self.svrLog.logInfo('S', self.logPrefix, funcReference, "Received %s transition via callback" % transition)       # 2011-11-28 sp -- added logging

            # IF we had a schedule running
            if self.getState() == 'RUNNING':
                # AND we are receiving an IDLE state change
                if transition == 'IDLE':
                    # THEN we have just completed the schedule successfully
                    if self.tracker.hasStarted():
                        # Finalise the sample tracker reporting
                        self.tracker.scheduleCompleted()
                # OR we are recieving a HALT
                if transition == 'HALTED':
                    # THEN the schedule was aborted
                    if self.tracker.hasStarted():
                        # Finalise the sample tracker reporting
                        self.tracker.scheduleAborted('Dispatcher HALTED')
            self.__changeState(transition)

        elif reportType == Dispatcher.STATUS_MSG:
            # Incoming status reports don't have field1 set
            statusCode = field2 
            self.__reportStatus(statusCode, *args)

        elif reportType == Dispatcher.ERROR_MSG:
            self.__reportError(field1, field2, *args)
            
            # IF we had a schedule running
            if self.getState() == 'RUNNING':
                # An error will cause the schedule to fail
                if self.tracker.hasStarted():
                    # Finalise the sample tracker reporting
                    self.tracker.scheduleAborted('Dispatcher Error' + str(field2))
        else:
            # Odd -- should this ever happen?
            self.logger.logWarning("Received unexpected report type: %s" % (reportType))
            self.svrLog.logWarning('', self.logPrefix, funcReference, "Received unexpected report type: %s" % (reportType))       # 2011-11-28 sp -- added logging
            if self.__reportCallback != None:
                self.__reportCallback(reportType, field1, field2, *args)

    # --- Other (semi-private) supporting methods ---

    def filterSamples(self, samples):
        '''Take a list of samples that the client wants to schedule and/or
        run. We return a list of valid samples that can be run.
        
        A scenario where the input list and returned list may not be the same
        is when the list contains samples that use a mixture of separation and
        maintenance protocols; we would only want to run a single maintenance 
        protocol. In this case, we will just return the first sample using a
        maintenance protocol.'''
        
        # Separate out our maintenance and separation protocol samples
        maintSamples = [s for s in samples if not self.pm.isSeparationProtocol(s.protocolID)]
        sepSamples = [s for s in samples if self.pm.isSeparationProtocol(s.protocolID)]
        numMaintSamples = len(maintSamples)
        numSepSamples = len(sepSamples)
        
        if numMaintSamples > 1:
            # More than one maintenance protocol? Take the first sample
            luckySample = maintSamples[0]
            filteredSamples = [luckySample,]
            self.__reportStatus('TSC1100', luckySample.ID)
        elif numMaintSamples > 0 and numSepSamples > 0:
            # Mix of maintenance and separation protocols? Take the first
            # maintenance protocol and dump the rest
            luckySample = maintSamples[0]
            filteredSamples = [luckySample,]
            self.__reportStatus('TSC1101', luckySample.ID)          
        else:
            # Note: We could expand this with additional checks (such as
            # checking where more than two samples using 2-quadrant protocols
            # are in the list: we wouldn't have enough quadrants to support
            filteredSamples = samples[:]        
            
        return filteredSamples


    # 2013-01-16 -- sp  overwrite reagent lot IDs if there are scanned values present
    def overWriteBarcodedReagentID(self, samples):
        '''Take a list of samples in the system and replace the reagent ids 
        if there are scanned value present in memory.'''
        funcReference = __name__ + '.WriteBarcodedReagentID'      # 2011-11-28 sp -- added logging
        
        barcodedSamples = samples[ : ]
        for i in range( len( barcodedSamples ) ):
            initialQuadrant = barcodedSamples[i].initialQuadrant
            for j in range( len( barcodedSamples[i].consumables )):
                barcode = self.GetVialBarcodeAt( initialQuadrant + j, 1)
                if( len( barcodedSamples[i].antibodyLabel) <= j ):
                    barcodedSamples[i].antibodyLabel.append( barcode )
                else:
                    if( barcode != '-' ):
                        barcodedSamples[i].antibodyLabel[j] = barcode 

                barcode = self.GetVialBarcodeAt( initialQuadrant + j, 2)
                if( len( barcodedSamples[i].cocktailLabel) <= j ):
                    barcodedSamples[i].cocktailLabel.append( barcode )
                else:
                    if( barcode != '-' ):
                        barcodedSamples[i].cocktailLabel[j] = barcode 

                barcode = self.GetVialBarcodeAt( initialQuadrant + j, 3)
                if( len( barcodedSamples[i].particleLabel) <= j ):
                    barcodedSamples[i].particleLabel.append( barcode )
                else:
                    if( barcode != '-' ):
                        barcodedSamples[i].particleLabel[j] = barcode 

        self.svrLog.logDebug('', self.logPrefix, funcReference, "Number of samples=%d" % len( barcodedSamples ))       # 2011-11-28 sp -- added logging
        return barcodedSamples


    def setInitialVolumeLevels(self):
        '''Set up the initial vial volumes onboard the instrument.
        This:
            * Empties the waste container
            * Marks all reagent containers as being empty
            * Marks the BCCM container as 50% full
        Returns nothing.'''
        self.instrument.emptyWasteContainers()
        self.instrument.emptyReagentContainers()
        
        # RL - transport from buffer bottle is always from the bottom -  03/21/06
        #BCCM_LEVEL = 0.51
        BCCM_LEVEL = 0.10
        self.instrument.setContainerVolumePercentage(0, Instrument.BCCMLabel, BCCM_LEVEL)

    def calculateVolumeLevels(self, samples):
        '''Calculate the volumes needed for all containers involved with 
        processing the samples in the supplied samples list.
        Returns a dictionary of levels, keyed by sector and container name:
            volume = levels[sector][containerName]
        '''
        # Note: we could refactor this code and the Protocol Manager's 
        # getConsumableInformation() methods, as they can be significantly 
        # cleaned up with a small amount of effort
        levels = {}
        for sector in range(1, tesla.config.NUM_QUADRANTS + 1):
            levels[sector] = {}
            for container in Instrument.ReagentLabelSet:
                levels[sector][container] = 0
        for sample in samples:
            sector = sample.initialQuadrant
            if 'consumables' in dir(sample):
                consumables = sample.consumables
            else:
                consumables = self.calculateConsumables(sample.protocolID, sample.volume)
                                   
            levels[sector][Instrument.SampleLabel] = max(0, sample.volume) # bdr99

            for pc in consumables:
                index = (sector + pc.quadrant) - 1
                # RL - uniform multi sample -  03/23/06
                if index > 1 and pc.sampleVesselVolumeRequired:
                    levels[index][Instrument.SampleLabel] = max(0, sample.volume)
                levels[index][Instrument.AntibodyLabel] = max(0, pc.antibodyVolume)
                levels[index][Instrument.CocktailLabel] = max(0, pc.cocktailVolume)
                levels[index][Instrument.BeadLabel]     = max(0, pc.particleVolume)
                levels[index][Instrument.LysisLabel]    = max(0, pc.lysisVolume)
                
                print('\n#CWJ - Sample volume %d'%levels[index][Instrument.SampleLabel])
                print('#CWJ - Lysis volume %d\n\n'%levels[index][Instrument.LysisLabel])
        return levels

    def setVolumeLevels(self, samples):
        '''For the specified set of samples, set up the various vial volume
        levels.
        We set the volume levels to be like they are at startup (ie. waste 
        empty, reagents empty, BCCM prefilled to a suitable level). We then set 
        the required reagent levels to the required levels.
        Returns nothing.'''

        self.setInitialVolumeLevels()
        levels = self.calculateVolumeLevels(samples)
        for sector in list(levels.keys()):
            for container in list(levels[sector].keys()):
                volume_uL = levels[sector][container]
                self.instrument.setContainerVolume(sector, container, volume_uL)
                
    def checkDiskSpace(self):
        '''Check to ensure that we have sufficient free space to work in. 
        Returns True if there is is sufficient space, False otherwise.'''
        workPath = tesla.config.BASE_DIR
        freeSpace_bytes = getFreeDiskSpace(workPath)
        spaceOkay = freeSpace_bytes > tesla.config.MIN_SAFE_DISK_SPACE
        if not spaceOkay:
            self.__reportError(ERROR_LEVEL, 'TEC1101', freeSpace_bytes)
            raise ControlException("CC: %d bytes: insufficient disk space" % \
                    (freeSpace_bytes))
        return spaceOkay

    def checkRequiredProtocols(self):
        '''Check that we have any protocols deemed essential to operation.
        At this stage, the only essential protocol is at least one shutdown 
        protocol.'''
        shutdownProtocols = self.pm.findProtocolsByType(tesla.config.SHUTDOWN_PROTOCOL)
        if len(shutdownProtocols) == 0:
            self.__reportError(ERROR_LEVEL, 'TEC1210', 'Need at least one shutdown protocol')
            raise ControlException("CC: Need at least one shutdown protocol")

    #GetInstrumentAxisStatusSet support methods
    def GetZHardwareInfo(self):
        return self.instrument.getPlatform().getRobot().GetZHardwareInfo()
    def GetThetaHardwareInfo(self):
        return self.instrument.getPlatform().getRobot().GetThetaHardwareInfo()
    def GetCarouselHardwareInfo(self):
        return self.instrument.getPlatform().getCarousel().GetHardwareInfo()
    def GetPumpHardwareInfo(self):
        return self.instrument.getPump().GetHardwareInfo()
    def GetTipStripperHardwareInfo(self):
        return self.instrument.getPlatform().getTipStripper().GetHardwareInfo()

    
    def SetBarcodeRescanOffset(self, offset):                                       
        self.instrument.SetBarcodeRescanOffset(offset);
        
    def GetBarcodeRescanOffset(self):                                           
        return self.instrument.GetBarcodeRescanOffset();

    def GetEndOfRunXMLFullFileName(self):                 #For EOR by Sunny
        return self.tracker.GetReportXMLFullFileName();

    def SetBarcodeThetaOffset(self, offset):                                       
        self.instrument.SetBarcodeThetaOffset(offset);
        
    def GetBarcodeThetaOffset(self):                                           
        return self.instrument.GetBarcodeThetaOffset();

    def getProtocolCommandList(self, protocolIdx):
        return self.scheduler.getProtocolCommandList(protocolIdx)
    
    def isSharingComboValid(self, sampleList, userID = 'undefined',
                 isSharing = False, sharedSectorsTranslation = [], sharedSectorsProtocolIndex = []):
        self.logger.logInfo("CC: isSharingComboValid() for %s user" % (userID,))
        funcReference = __name__ + '.isSharingComboValid'      
        self.svrLog.logID('', self.logPrefix, funcReference, "userID=%s" % (userID))

        samples = self.filterSamples(sampleList)
        self.updateTubeSizeToHandleCustVials(samples)
        samplesWithVolumes = self.SetupVolumeForStartRun(samples,isSharing, sharedSectorsTranslation, sharedSectorsProtocolIndex)

        volumeCheckErrMessage = self.CheckSampleReagentRequiredVolumeAgainstFilledVolume(samplesWithVolumes)
        if volumeCheckErrMessage != "":
            self.logger.logWarning("CC: isSharingComboValid found volume check error: %s" % str(volumeCheckErrMessage))
            return volumeCheckErrMessage
        
        #attempt to set the volume with sharing info
        #catch exception here and send message back to client
        try:
            self.setVolumeLevels(samplesWithVolumes)
        except Exception as msg:
            self.logger.logWarning("CC: isSharingComboValid found error: %s" % str(msg))
            return str(msg)
        
        return ""
# eof

