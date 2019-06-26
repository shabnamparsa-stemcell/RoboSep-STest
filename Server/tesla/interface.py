# interface.py
# The RoboSep instrument interface that is used by the XML-RPC gateway
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
#    03/24/05 - added parkarm(0 and islidclosed() - bdr
#    04/11/05 - added parkpump to deenergize pump - bdr
#    03/23/06 - uniform multi sample - RL
#    03/29/06 - pause command - RL
#    09/07/07 - added usebuffertip transport option - bdr
#  GetInstrumentAxisStatusSet
# ---------------------------------------------------------------------------

import sys, time, re
import xmlrpc.client

from ipl.utils import flattenSequence

import tesla.config
from tesla.config import WARNING_LEVEL, ERROR_LEVEL
import tesla.logger
from tesla.serialnum import InstrumentSerialNumber
from tesla.types.sample import Sample
from tesla.control.Centre import ControlException

import inspect
from tesla.hardware.Device import Device
from tesla.instrument.Subsystem import Subsystem
from tesla.hardware.config import GetHardwareData

import RoboTrace              #CWJ Add
import time

from tesla.PgmLog import PgmLog     # 2011-11-23 sp -- programming logging
import os                           # 2011-11-23 sp -- programming logging

import time

# -----------------------------------------------------------------------------
# Constants that should not need to change too often (ie. not soft-setups)

# Our XML-RPC interface version string: Major release, minor release, revision
#   Major release: 0 = Pre-CDP, 1 = CDP, 2 = Beta, 3 = PPU, 4 = Production
#   Minor release: 1 = Development, 2 = Testing, 3 = Release
#        Revision: Revisions pertinent to the current software release

INTERFACE_VERSION = '2.1.1'


Z_HEIGHT_KEY = "z_height"
Z_HOME_KEY = "z_home"
Z_LIMIT_KEY = "z_limit"
THETA_DEG_KEY = "theta_deg"
THETA_HOME_KEY = "theta_home"
THETA_LIMIT_KEY = "theta_limit"
CAROUSEL_DEG_KEY = "carousel_deg"
CAROUSEL_HOME_KEY = "carousel_home"
CAROUSEL_LIMIT_KEY = "carousel_limit"
PUMP_VOL_KEY = "pump_vol"
PUMP_HOME_KEY = "pump_home"
PUMP_LIMIT_KEY = "pump_limit"
STRIPPER_ENGAGED_KEY = "stripper_eng"
STRIPPER_EXT_KEY = "stripper_ext"
STRIPPER_RET_KEY = "stripper_ret"
LOW_BUFFER_LVL_KEY = "low_buffer_lvl"

# -----------------------------------------------------------------------------

class InstrumentInterface(object):
    '''The published RoboSep instrument XML-RPC interface.
    
    Note that to work with the .NET XML-RPC library (http://www.xml-rpc.net/),
    DateTime values are passed out in a modified ISO 8601 format which is
    YYYYMMDDTHH:MM::SS (eg. 20040309T16:34:13).

    Note that any exception caught here has already been logged. We should only
    be logging new exceptions that we are responsible for; eg. handling bad
    input data from client.
    ControlException exceptions (and other exceptions that bubble up from lower 
    levels) should not cause the gateway to die; but to simply trigger an 
    XML-RPC exception on the client side.

    Note that there is one method that is available through this interface
    that are shared with the service interface, and defined in gateway.py:

        string halt()
    '''

    def __init__(self, gatewayURL, controlCentre, interfaceVersion = INTERFACE_VERSION):
        """Load in the configuration data for the pump."""
        self.m_hardwareConfig = GetHardwareData()

        '''Interface constructor, called by the Python application.
        NOTE: This method is not accessible from the XML-RPC interface!

        gatewayURL is the URL (including port number) for the XML-RPC server,
        and is reported back as part of getServerInfo().
            An example is: http://localhost:8000

        controlCentre is an instance of tesla.control.centre and is the
        "brains" of the control layer.

        interfaceVersion is a version number for the interface revision
        '''
        self.__startTime = self.__getNow()          # Record the time we started up
        self.__gatewayURL = gatewayURL                # Store the gateway we're tied to
        self.controlCentre = controlCentre
        self.version = interfaceVersion
        self.logger = tesla.logger.Logger(logName = tesla.config.LOG_PATH)
        self.subscribers = {}                        # Initialise our subscribers

        # 2011-11-24 -- sp
        self.svrLog = PgmLog( 'svrLog' )
        self.logPrefix = 'IF'

        # Get the instrument serial number
        self.serialNumber = InstrumentSerialNumber().getSerial()

        # Register our reporter callback with the control centre so that we can
        # receive error & status notifications
        self.controlCentre.registerReporter(self.__reporter)
        self.__needToInit = True                # Need to init the instrument?

        # Do the inspection of Devices and Subsystems
        self.__serviceFunctionList = []         # Holds a list of all service functions
        self.__deviceAndSubsystemList = []      # Holds a list of all registered Devices and Subsystems
        self.__generateServiceFunctionList()

        # Now log useful information
        self.logger.logInfo("Server interface: %s" % (self.__gatewayURL))
        self.logger.logInfo("Instrument serial number: %s" % (self.serialNumber))

        # 2011-11-24 sp -- added logging
        funcReference = __name__ + '.__init__'
        self.svrLog.logID('', self.logPrefix, funcReference, 'Url=%s |SN=%s' % (self.__gatewayURL, self.serialNumber))

    # --- General instrument/interface queries --------------------------------

    def ping(self):
        '''Are we there? This is for basic keep-alive testing.
        IN: Nothing
        OUT: (boolean) True
        '''
        return True


    def getServerInfo(self):
        '''Returns a list of server information.
        IN: Nothing
        OUT: A list of (string) gateway URL, (string) interface version,
             (string) uptime in seconds, (string) instrument version and
             (string) instrument serial number
        '''
        uptime = round(self.__getNow() - self.__startTime)
        return [self.__gatewayURL, self.version, str(uptime), 
            tesla.config.SOFTWARE_VERSION, self.serialNumber]


    def getInstrumentState(self):
        '''Returns the current instrument state.
        IN: Nothing
        OUT: (string) Instrument state
        '''
        return str(self.controlCentre.getState())


    def getInstrumentStatus(self):
        '''Returns a list of the instrument status fields - work in progress
        IN: Nothing
        OUT: A list of (string)
        '''        
        # Status: Not used. This method should be reviewed, as it appears to
        #         now be redundant.
        return ['N/A',]


    def getHostConfiguration(self):
        '''Return a map of host configuration information; for debug/diagnostic 
        purposes
        IN: Nothing
        OUT: A list of (string) host configuration information.
        '''
        configInfo = {        'Python EXE'      : sys.executable,
                        'Python version'  : sys.version,
                        'Windows version' : sys.getwindowsversion(),
        }
        return configInfo

    def getContainerVolumes(self):
        '''Returns configured volumes for containers used by the instrument'''
        return self.controlCentre.getContainerVolumes()

    def getErrorLog(self, numEntries = 10, entryOffset = 0):
        '''Returns a list of entries (as strings) from the instrument error log. 
        IN: (int) numEntries, specifies how many entries to return.
            (int) entryOffset, specifies which entry to start at. 0-indexed. 
            Defaults to 0.
        OUT: A list of (string) error entries
        '''
        # Status: complete for CDP
        return self.logger.getEntries(numEntries, entryOffset)


    # --- Event administration ------------------------------------------------

    def getSubscriberList(self):
        '''Returns a list of event server subscriptions.
        IN: Nothing
        OUT: A list of (string) subscribers
        '''
        # Status: complete for CDP
        return list(self.subscribers.keys())


    def testSubscriber(self, subscriber):
        '''Test the connection to the subscriber by pinging it
        IN: (string) subscriber URL 
        OUT: (boolean) success flag
        '''
        try:
            return subscriber.ping()
        except:
            return False                            


    def subscribe(self, serverURL):
        '''Subscribe an XML-RPC server to call events on. (overridding the base
        class subscribe() method).

        The XML-RPC server has to have the following methods:
        * ping(void)
        * reportStatus(string status)
        * reportError(int level, string message)
        * reportETC(string ETC)

        The ETC string that we pass to reportETC() should be in modified 
        ISO-8601 format; eg. 20040309T16:34:13

        The first time a client subscribes, the instrument will be
        initialised (moved to the INIT state), which initialises
        robotic axes, fluidics, etc.

        IN: Nothing
        OUT: (boolean) True (always)
        '''

        funcReference = __name__ + '.subscribe'  # 2011-11-24 sp -- added logging
        subscriberAlive = False
        if serverURL not in self.subscribers:
            if not self.__verifySubscriber(serverURL):
                # For whatever reason, the server URL is invalid or badly
                # formed; eg. it's using the same port as we use for the 
                # service interface
                self.logger.logInfo("II: %s in an invalid URL (reserved port?)" % (serverURL))
                self.svrLog.logInfo('', self.logPrefix, funcReference, 'Invalid (reserved?) url=%s' % serverURL)  # 2011-11-24 sp -- added logging
            else:
                # Bring up a connection to the subscriber        
                s = xmlrpc.client.ServerProxy(serverURL, verbose = tesla.config.XMLRPC_DEBUG)
                subscriberAlive = self.testSubscriber(s)
                if subscriberAlive:
                    self.subscribers[serverURL] = s
                    self.logger.logInfo("II: Subscribed to %s (status = %s)" % \
                                (serverURL, subscriberAlive))
                    self.svrLog.logInfo('', self.logPrefix, funcReference, "url=%s |status=%s" % \
                                (serverURL, subscriberAlive))  # 2011-11-24 sp -- added logging

                    # Do we need to initialise the instrument?
                    # We do this the first time a client subscribes
                    if self.__needToInit:
                        self.logger.logInfo("II: Initiated instrument initialisation")
                        self.svrLog.logInfo('', self.logPrefix, funcReference, "to call initialiseInstrument")  # 2011-11-24 sp -- added logging
                        self.controlCentre.initialiseInstrument()
                        self.__needToInit = False
                else:
                    # We hit an error trying to subscribe: log the error
                    self.logger.logWarning("II: Unable to subscribe to %s" % (serverURL))
                    self.svrLog.logWarning('', self.logPrefix, funcReference, "Unable to subscribe to url=%s" % serverURL)  # 2011-11-24 sp -- added logging
        else:
            self.logger.logInfo("II: %s tried to resubscribe" % (serverURL))
            self.svrLog.logInfo('', self.logPrefix, funcReference, "Try to resubscribe to url=%s" % serverURL)  # 2011-11-24 sp -- added logging
        return subscriberAlive


    def unsubscribe(self, serverURL):
        '''Unsubscribe the specified server.
        IN: (string) serverURL
        OUT: (boolean) unsubscription success flag, false indicates that the
             server wasn't subscribed
        '''
        status = serverURL in self.subscribers
        if status:
            del self.subscribers[serverURL]
            self.logger.logInfo("Unsubscribed %s" % (serverURL))
            funcReference = __name__ + '.unsubscribe'  # 2011-11-24 sp -- added logging
            # 2011-12 sp -- message not tested
            self.svrLog.logInfo('xx', self.logPrefix, funcReference, "url=%s" % serverURL)  # 2011-11-24 sp -- added logging
        return status


    # --- Methods & events related to cell separation runs --------------------


    def getCustomNames(self):
        return self.controlCentre.getCustomNames()

    def getResultChecks(self):
        return self.controlCentre.getResultChecks()
    
    # RL - uniform multi sample -  03/23/06
    #def getMultipleSamples(self):
    #    return self.controlCentre.getMultipleSamples()
    
    def getProtocols(self):
        '''Return a list of all known separation protocols.
        IN: Nothing
        OUT: A list of ClientProtocol objects
        '''
        return self.controlCentre.getProtocols()

    def calculateConsumables(self, protocolID, sampleVolume):
        '''Return a map of consumables needed for the specified protocol and 
        sample volume.
        IN: protocolID = (int) unique protocol ID, obtained from getProtocols()
            sampleVolume = (double) sammple volume (in uL)
        OUT: A list of ProtocolConsumable objects
        '''
        return self.controlCentre.calculateConsumables(protocolID, sampleVolume)

    def scheduleRun(self, samples):
        '''Schedule 1 - 4 samples to run. Stores the ETC and batch object.
        IN: samples = A list of 1 - 4 Sample objects
        OUT: (ISO 8601) Estimated Time *of* Completion, aka "ETC"
        '''
        # Turn the samples list passed into a Python list of Sample objects
        # Add the sample list to the sample tracker
        # Schedule the samples, store the batch ID and ETC
        # Store the ETC in the sample tracker and return it here
        self.logger.logInfo("interface::scheduleRun( samples = %s )" % (samples))
        funcReference = __name__ + '.scheduleRun'  # 2011-11-24 sp -- added logging
        self.svrLog.logID('', self.logPrefix, funcReference, "%s" % samples)  # 2011-11-24 sp -- added logging

        return self.controlCentre.scheduleRun(self._convertSamples(samples))

    def startRun(self, samples, userID = 'unknown',
                 isSharing = False, sharedSectorsTranslation = [], sharedSectorsProtocolIndex = []):
        '''Start the scheduled run.
        IN: samples = A list of 1 - 4 Sample objects
            userID = (string) optional user/operator ID        
        OUT: (ISO 8601) Estimated Time *of* Completion, aka "ETC"
        '''
        try:
            etc = self.controlCentre.startRun(self._convertSamples(samples), userID,
                                              isSharing,sharedSectorsTranslation,sharedSectorsProtocolIndex)
            #self._reportETC(etc)
        except ControlException as msg:
            # This exception should have already been logged and reported to
            # the host. Clean up nicely by getting the ETC from the control 
            # layer; in the case, ETC should be set to *now*
            etc = self.controlCentre.getETC()
        return etc

    def pauseRun(self):
        '''Pause a currently-executing run. The run can then be resumed
        by calling the resumeRun() method, but the validity of the
        chemistries can not be guaranteed (for example, cells may have
        dried out or been separated for too long).
        IN: Nothing
        OUT: (boolean) flag, not used
        '''
        return self.controlCentre.pauseRun()

    def resumeRun(self):
        '''Resume a paused run. This will ONLY work if the
        pauseRun() method has been callled.
        IN: Nothing
        OUT: (boolean) status, true if the run could be resumed
        '''
        result = self.controlCentre.resumeRun()
        #stupid thing blocks for some reason
        return result

    def parkArm(self):
        '''All axes are sent back to safe positions, etc.)...bdr
        IN: Nothing
        OUT: (boolean) flag, not used
        '''
        return self.controlCentre.parkArm()

    def parkPump(self):
        '''Put pump valve in deenergized position..bdr
        IN: Nothing
        OUT: (boolean) flag, not used
        '''
        return self.controlCentre.parkPump()

    def haltRun(self):
        '''Halt a currently-executing run. The run is stopped as
        gracefully as possible (all axes are sent back to safe
        positions, etc.).
        IN: Nothing
        OUT: (boolean) flag, not used
        '''
        return self.controlCentre.haltRun()

    def abortRun(self):
        '''Abort/eStop a currently-executing run. There is NO graceful
        termination of the current run; the instrument is simply STOPPED.
        IN: Nothing
        OUT: (boolean) flag, not used
        '''
        return self.controlCentre.abortRun()

    def reloadProtocols(self):
        return self.controlCentre.reloadProtocols()


    # --- Misc support methods ------------------------------------------------

    def GetInstrumentAxisStatusSet(self, combo):                                       #RL Add

        if combo > 100: #combo over 100 is not suported yet, just send back the last full set
            combo = 100
        if combo > 6: #combo over 6 is not suported yet, just send back the full set
            combo = 100

        if combo <= 0 : #combo shouldn't be < 0
            return '';
        elif combo == 1: #z
            (zHeight, zHome, zLimit) = self.controlCentre.GetZHardwareInfo()
            str = 'COMBO=%d, %s=%f %s=%s,%s=%s ' % \
            (combo, Z_HEIGHT_KEY, zHeight, Z_HOME_KEY,zHome,Z_LIMIT_KEY,zLimit)
            return str
        elif combo == 2: #theta
            (thetaDeg, thetaHome, thetaLimit) = self.controlCentre.GetThetaHardwareInfo()
            str = 'COMBO=%d, %s=%f %s=%s,%s=%s ' % \
            (combo, THETA_DEG_KEY, thetaDeg, THETA_HOME_KEY,thetaHome,THETA_LIMIT_KEY,thetaLimit)
            return str
        elif combo == 3: #carousel
            (carouselDeg, carouselHome, carouselLimit) = self.controlCentre.GetCarouselHardwareInfo()
            str = 'COMBO=%d, %s=%f %s=%s,%s=%s ' % \
            (combo, CAROUSEL_DEG_KEY, carouselDeg, CAROUSEL_HOME_KEY,carouselHome,CAROUSEL_LIMIT_KEY,carouselLimit)
            return str
        elif combo == 4: #pump
            (pumpVol, pumpHome, pumpLimit) = self.controlCentre.GetPumpHardwareInfo()
            str = 'COMBO=%d, %s=%f %s=%s,%s=%s ' % \
            (combo, PUMP_VOL_KEY, pumpVol, PUMP_HOME_KEY,pumpHome, PUMP_LIMIT_KEY,pumpLimit)
            return str
        elif combo == 5: #stripper arm
            (stripIsEngaged, stripHome, stripLimit) = self.controlCentre.GetTipStripperHardwareInfo()
            str = 'COMBO=%d, %s=%s %s=%s [%s] %s=%s [%s] ' % \
            (combo, STRIPPER_ENGAGED_KEY,stripIsEngaged, \
             STRIPPER_EXT_KEY,int(stripHome)==0, stripHome,\
             STRIPPER_RET_KEY,int(stripLimit)==0, stripLimit)
            return str
        elif combo == 6: #buffer
            isBufLow = self.isHydroFluidLow()
            str = 'COMBO=%d, %s=%s ' % \
            (combo, LOW_BUFFER_LVL_KEY,isBufLow)
            return str
        elif combo == 100: #all
            #sec = (int)(time.time())
            sec = int(round(time.time() * 1000))
            
            #print '****************start ', sec
            isBufLow = self.isHydroFluidLow()
            #print '****************Hydro ', int(round(time.time() * 1000))
            (zHeight, zHome, zLimit) = self.controlCentre.GetZHardwareInfo()
            #print '****************Z ', int(round(time.time() * 1000))
            (thetaDeg, thetaHome, thetaLimit) = self.controlCentre.GetThetaHardwareInfo()
            #print '****************theta ', int(round(time.time() * 1000))
            (carouselDeg, carouselHome, carouselLimit) = self.controlCentre.GetCarouselHardwareInfo()
            #print '****************carousel ', int(round(time.time() * 1000))
            (pumpVol, pumpHome, pumpLimit) = self.controlCentre.GetPumpHardwareInfo()
            #print '****************pump ', int(round(time.time() * 1000))
            (stripIsEngaged, stripHome, stripLimit) = self.controlCentre.GetTipStripperHardwareInfo()
            #print '****************stripper arm ', int(round(time.time() * 1000))

            str = 'COMBO=%d, %s=%f %s=%s,%s=%s, %s=%f %s=%s,%s=%s, %s=%f %s=%s,%s=%s, %s=%f %s=%s,%s=%s, %s=%s %s=%s [%s] %s=%s [%s] %s=%s ' % \
            (combo,\
             Z_HEIGHT_KEY, zHeight, Z_HOME_KEY,zHome,Z_LIMIT_KEY,zLimit, \
             THETA_DEG_KEY, thetaDeg, THETA_HOME_KEY,thetaHome,THETA_LIMIT_KEY,thetaLimit, \
             CAROUSEL_DEG_KEY, carouselDeg, CAROUSEL_HOME_KEY,carouselHome,CAROUSEL_LIMIT_KEY,carouselLimit, \
             PUMP_VOL_KEY, pumpVol, PUMP_HOME_KEY,pumpHome, PUMP_LIMIT_KEY,pumpLimit, \
             STRIPPER_ENGAGED_KEY,stripIsEngaged, \
             STRIPPER_EXT_KEY,int(stripHome)==0, stripHome,\
             STRIPPER_RET_KEY,int(stripLimit)==0, stripLimit, \
             LOW_BUFFER_LVL_KEY,isBufLow)
            #print '****************end ', int(round(time.time() * 1000))

            #print '>>> %s <<<' % str
            print('****************COMBO 100 @@@', int(round(time.time() * 1000)-sec))

            return str

        return '' #shouldn't get here!

    def SaveIniAsFactory(self):                                       #RL Add
        self.m_hardwareConfig.SaveIniAsFactory()
        return True

    def RestoreIniWithFactory(self):                                           #RL Add
        self.m_hardwareConfig.RestoreIniWithFactory()
        self.controlCentre.reInitInstrument()
        self.m_hardwareConfig = GetHardwareData()
        return True

    def shutdown(self, powerDown = True):
        '''Move the instrument to the SHUTTINGDOWN (and then the SHUTDOWN) state.
        IN: (boolean) if true, power down the instrument
        OUT: (boolean) True, not used
        '''
        self.controlCentre.shutdownInstrument(powerDown)
        return True
        
    def turnOff(self):
        '''Move the instrument to its powered off state
        IN: Nothing
        OUT: (boolean) True, not used
        '''
        self.controlCentre.shutdownInstrument(powerDown = False)
        return True


    def isLidClosed( self ):
        '''Returns true if the lid is closed
        IN: Nothing
        OUT: (boolean) True if lid is closed'''
        print("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! lid close")
        return self.controlCentre.isLidClosed()
        
    def SetIgnoreLidSensor(self, sw):                                       #CWJ Add
        print('\n>>> SetIgnoreLidSensor: %d \n'%sw)
        self.controlCentre.SetIgnoreLidSensor(sw);
        self.m_hardwareConfig.writeItem( 'LidSensor','ignorelidsensor' ,sw ) #TODO: move this to lid sensor
        self.m_hardwareConfig.write()
        rtn = 0;                                          # Return Integer
        return rtn        
    
    def GetIgnoreLidSensor(self):                                           #CWJ Add
        rtn = self.controlCentre.GetIgnoreLidSensor();
        return rtn

    def SetTimedStart(self,sw):                                       #RL Add
        print('\n>>> SetTimedStart: %d \n'%sw)
        self.m_hardwareConfig.writeItem( 'ConsoleConfig','timedstart' ,sw )
        self.m_hardwareConfig.write()
        rtn = 0;                                          # Return Integer
        return rtn
    
# 2012-03-05 RL -- added
    def LoadSelectedHardwareINI(self,path):                                       #RL Add
        print('\n>>> LoadSelectedHardwareINI: %s \n'%path)
        self.m_hardwareConfig.LoadSelectedHardwareINI(path)
        self.controlCentre.reInitInstrument()
        self.m_hardwareConfig = GetHardwareData()
        rtn = 0;                                          # Return Integer
        return rtn

    def GetTimedStart(self):                                           #RL Add
        settings = self.m_hardwareConfig.Section ('ConsoleConfig')
        rtn = int( settings['timedstart'] )  == 1
        return rtn

    def SetIgnoreHydraulicSensor(self, sw):                                 #CWJ
        print('\n>>> SetIgnoreHydraulicSensor: %d \n'%sw)
        self.controlCentre.SetIgnoreHydraulicSensor(sw);
        self.m_hardwareConfig.writeItem( 'HydraulicSensor','ignoresensor' ,sw ) #TODO: move this to lid sensor
        self.m_hardwareConfig.write()
        rtn = 0;
        return rtn
        
    def GetIgnoreHydraulicSensor(self):                                     #CWJ
        rtn = self.controlCentre.GetIgnoreHydraulicSensor();
        return rtn
    
    def GetCurrentID(self):                                         #CWJ
        CamMgr = RoboTrace.GetRoboCamMgrInstance();
        rtn = CamMgr.GetCurrentProtocolID()
#        print '\n>>> GetCurrentID: %d\n'%rtn
        return rtn
        
    def GetCurrentSeq(self):                                         #CWJ
        CamMgr = RoboTrace.GetRoboCamMgrInstance();
        rtn = CamMgr.GetCurrentProtocolSeq()
#        print '\n>>> GetCurrentSeq: %d\n'%rtn
        return rtn
        
    def GetCurrentSec(self):                                         #CWJ
        CamMgr = RoboTrace.GetRoboCamMgrInstance();
        rtn = CamMgr.GetCurrentSec()
#        print '\n>>> GetCurrentSec: %d\n'%rtn
        return rtn

    def isPauseCommand( self ):
        return self.controlCentre.isPauseCommand()
        
    def getPauseCommandCaption( self ):
        return self.controlCentre.getPauseCommandCaption()

    def isHydroFluidLow( self ):
        '''Returns true if the hydraulic fluid is low
        IN: Nothing
        OUT: (boolean) True if hydraulic fluid is low'''
        if self.GetIgnoreHydraulicSensor() == True :
            return self.controlCentre.isHydroFluidLow()
        else :
            if self.controlCentre.isHydroFluidLow():
                self.setHydroFluidFull() #setting this writes into ini so don't do it if you dont have to
            return self.controlCentre.isLowHydraulicLevel()
            
    def AttachBarcode(self):
        self.controlCentre.AttachBarcodeReader();
                
    def DetachBarcode(self):
        self.controlCentre.DetachBarcodeReader();

    def turnONBeacon(self, evt):
        self.controlCentre.TurnONBeacon(evt);
    
    def turnOFFBeacon(self):
        self.controlCentre.TurnOFFBeacon();
                                       
    def setHydroFluidFull( self ):
        '''Sets the hydraulic fluid to full capacity
        IN: Nothing
        OUT: (boolean) True, not used'''
        self.controlCentre.setHydroFluidFull()
        return True

    def getEndTimeSpan( self ):
        '''Returns the span between end times of the last calculated schedule
        IN: Nothing
        OUT: (int) end time span in seconds'''
        return self.controlCentre.getEndTimeSpan()

    def getEndTimeSpanThreshold( self ):
        '''Returns the end time span threshold
        IN: Nothing
        OUT: (int) end time span threshold in seconds'''
        return self.controlCentre.getEndTimeSpanThreshold()

    # ---- Service methods ----------------------------------------------------

    def enterServiceState( self ): 
        '''Changes the instrument state from IDLE to SERVICE
        IN: Nothing
        OUT: (boolean) flag on success of state change
        '''
        self.DetachBarcode();
        return self.controlCentre.enterServiceState()
    
    def exitServiceState( self ):
        '''Changes the instrument state to SERVICE to IDLE
        IN: Nothing
        OUT: (boolean) flag on success of state change
        '''
        self.AttachBarcode();
        return self.controlCentre.exitServiceState()

    def execute( self, objectName, method, arguments ):
        '''Execute command given by the arguments
        IN: objectName = (string) the name of the hardware object (eg. device)
            method = (string) the name of the method to execute on the object
            arguments = (string) arguments to pass to the method
        OUT: (string) results of method call
        '''
        currentState = self.controlCentre.getState()
        funcReference = __name__ + '.execute'  # 2011-11-24 sp -- added logging
        # 2011-12 sp -- messages in function not tested
        if currentState == 'SERVICE':
            #special case for resetting the instrument
            if objectName == "Centre" and method == "reInitInstrument":
                self.controlCentre.reInitInstrument()
                self.__generateServiceFunctionList()
                return ""
            # Find object in list of Devices and Subsystems
            #print "number of obj", len(self.__deviceAndSubsystemList)
            for obj in self.__deviceAndSubsystemList:
                if objectName == "%s" % (obj):
                    #print "matched", obj, id(obj)
                    break
            else:
                # No match for requested hardware object
                self.logger.logError("II: No match for %s object for service call" % (objectName))
                self.svrLog.logError('xx', self.logPrefix, funcReference, "No match for requested hardware object=%s" % objectName)  # 2011-11-24 sp -- added logging

                self._reportError(WARNING_LEVEL, 'TEC1410', objectName)
                return ""

            # Build up text representation of function we want to call
            # once we scope it it the correct Device or Subsystem
            fnCall = "self." + method + "(" + arguments.replace( ' ', ',' ) + ")" 
            self.logger.logDebug("II: Service execute() call: %s" % (fnCall))
            self.svrLog.logVerbose('svc', self.logPrefix, funcReference, "to call execute(%s)" % fnCall)  # 2011-11-24 sp -- added logging

            # Execute that function call on the object
            returnVal = None            
            try:
                returnVal = obj.execute( fnCall )
            except AttributeError as msg:
                self.logger.logWarning("II: %s is an invalid call (%s)" % (fnCall, msg))
                self.svrLog.logWarning('xx', self.logPrefix, funcReference, "Invalid: called %s |msg=%s)" % (fnCall, msg))  # 2011-11-24 sp -- added logging
                self._reportError(WARNING_LEVEL, 'TEC1411', fnCall)
            if returnVal == None:            
                return ""
            else:
                return str( returnVal )
        else:
            # We are in the wrong state; report the error and return an
            # empty string
            self.logger.logError("II: Can't execute() in %s state" % (currentState))
            self.svrLog.logError('xx', self.logPrefix, funcReference, "Can't execute() in %s state" % (currentState))  # 2011-11-24 sp -- added logging
            self._reportError(WARNING_LEVEL, 'TEC1400', currentState)
            return ""
        

    def getServiceFunctionList( self ):
        '''Return all the information the client needs for each function
        in the function list.
        IN: Nothing
        OUT: A list of lists of strings
        '''
        returnList = []
        for funcName, funcInfo in self.__serviceFunctionList:
            # If there is no doc then funcInfo.__doc__ is None so we need to change this to
            # an empty string for the xmlrpc return
            documentation = funcInfo.__doc__
            if None == documentation:
                documentation = ""
            returnList.append( ["%s"%funcInfo.__self__, documentation,\
                                funcName, inspect.getargspec(funcInfo)[0]] )

        return returnList

#    Description of function and comments added 2013-01-16 -- sp
#    This routine initializes the barcode scanner: generally called at the start of the program
#    This routine returns a value of 1 if the barcode reader has successfully initialized
#    otherwise it will return a value of -1.
    def InitScanVialBarcode( self, freeaxis ):                    #CWJ
        print ('\n *** Recv InitScanVialBarcode() Command! \n');
        rtn = self.controlCentre.InitScanVialBarcode( freeaxis )  # 2012-02-01 -- sp added support
        rtn = 1
        return rtn        

#    Description of function and comments added 2013-01-16 -- sp
#    This routine returns a value of 0 if the barcode of the given quadrant/vial can be read
#    otherwise it will return the scan code which indicates the vials that fail.
#    If the quadrant value is -1, it will reset the stored values of all quadrants and vials
    def ScanVialBarcodeAt(self, Quadrant, Vial):        #CWJ
        print(('\n *** Recv ScanVialAt Quadrant: %d, Vial: %d\n'%(Quadrant, Vial)));
        rtn = self.controlCentre.ScanVialBarcodeAt(Quadrant, Vial)  # 2012-02-01 -- sp added support
        return rtn;

#    Descripton of function and comments added 2014-08-01 -- twong
#    This routine aborts the barcode scanner
    def AbortScanVialBarcode(self):
        print ('\n *** Recv AbortScanVialBarcode() Command! \n')
        self.controlCentre.AbortScanVialBarcode()
        return True;
    
#    Descripton of function and comments added 2014-08-07 -- twong
#    This routine indicates whether the barcode scanning is done
#    This routine returns True if the barcode scanning is done
#    Otherwise it will return False
    def IsBarcodeScanningDone(self):
        print ('\n *** Recv IsBarcodeScanningDone() command! \n')
        rtn = self.controlCentre.IsBarcodeScanningDone()
        return rtn;

#    Description of function and comments added 2013-01-16 -- sp
#    return the string representing the bar code that had been read at the given location
#    no scanning is performed by this routine
    def GetVialBarcodeAt( self, Quadrant, Vial):        #CWJ
        print(('\n *** Recv GetVialBarcodeAt Quadrant: %d, Vial: %d\n'%(Quadrant, Vial)));
        rtn = self.controlCentre.GetVialBarcodeAt(Quadrant, Vial)  # 2012-02-01 -- sp added support
#        rtn = 'Scanning';   # Moving, Error, Barcode(Real Barcode)
        return rtn;


#    Description of function and comments added 2013-09-25 -- Sunny
#    return the string representing the full path xml file name for the End Of Run report
    def GetEndOfRunXMLFullFileName( self ):        
        print ('\n *** Recv GetEndOfRunXMLFileName() Command! \n');
        rtn = self.controlCentre.GetEndOfRunXMLFullFileName() 
        return rtn;



    def __generateServiceFunctionList( self ):
        '''Generates a flat list of functions for all device objects and
        subsystem objects.'''

        # Add all Devices and Subsystems to the list
        self.__deviceAndSubsystemList = []
        self.__deviceAndSubsystemList.extend(Device.deviceList)
        self.__deviceAndSubsystemList.extend( Subsystem.instanceList )

        # Inspect each class in the Device and Subsystem list
        # and add all the relevant info to the function list
        # ready for the client
        self.__serviceFunctionList = []
        for obj in self.__deviceAndSubsystemList:
            classList = obj.getServiceMethods()
            self.__serviceFunctionList.extend(classList)


    # --- Non-public run-related events ---------------------------------------
   
    def _reportETC(self, etc = None):
        '''Trigger an event reporting the ETC to all subscribers.
        This is not intended to be called across the interface.'''
        if etc == None:
            etc = self.controlCentre.getETC()
        for subscriber in list(self.subscribers.values()):
            try:
                subscriber.reportETC(etc)
            except Exception as msg:
                self.logger.logWarning("II: reportETC() exception: %s" % (msg))
                funcReference = __name__ + '._reportETC'  # 2011-11-24 sp -- added logging
                # 2011-12 sp -- message not tested
                self.svrLog.logWarning('xx', self.logPrefix, funcReference, "exception=%s" % msg)  # 2011-11-24 sp -- added logging

  
    def _reportStatus(self, state, statusCode, *args):
        '''Trigger an event reporting the current instrument status to all
        subscribers.
        This is not intended to be called across the interface.'''
            # Note: this could be rationalised with reportError(), given the high
        #       level of overlap (a nice-to-have)
        paramCount = len(args)
        params = [str(x) for x in args]
        for subscriber in list(self.subscribers.values()):
            subscriber.reportStatus(state, statusCode, paramCount, params)


    def _reportError(self, errorLevel, errorCode, *args):
        '''Trigger an event reporting a critical error to all subscribers.
        This is not intended to be called across the interface.'''
        paramCount = len(args)
        params = [str(x) for x in args]
        for subscriber in list(self.subscribers.values()):
            subscriber.reportError(errorLevel, errorCode, paramCount, params)


    # ---- Private helper methods ---------------------------------------------

    def __getNow(self):
        '''Private method: Returns the current time in epoch seconds'''
        return time.time()

    def __verifySubscriber(self, serverURL):
        '''Verify that the URL provided for subscription to an interface is
        okay; in particular, that we are not binding to the service port (on
        this machine).
        An example of a decent server URL is: 
           http://localhost:8001/InstrumentControlEventSink
        '''
        verified = False
        urlMatch = re.compile("^http://(.*):(\d+)(/.*){0,1}").search(serverURL)
        verified = urlMatch            
        # hostname = urlMatch.group(1)
        # port = urlMatch.group(2)
        return verified

    def _convertSamples(self, samples):
        '''Convert a list of sample dictionaries into a returned list of Sample
        objects. - bdr'''
        converts = []
        for s in samples:
            sample = Sample(int(s['ID']), s['label'], int(s['protocolID']), 
                                float(s['volume_uL']), int(s['initialQuadrant']),
                                s.get('cocktailLabel', []), 
                                s.get('particleLabel', []),
                                s.get('lysisLabel', ''), 
                                s.get('antibodyLabel', []),
                                s.get('sample1Label', []),        # bdr99
                                s.get('sample2Label', ''),        # bdr99
                                s.get('bufferLabel', '')     # sunny
                            )
            
            self.logger.logInfo("interface::_convertSamples - sample = %s" % (sample))            
            funcReference = __name__ + '._convertSamples'  # 2011-11-24 sp -- added logging
            self.svrLog.logID('', self.logPrefix, funcReference, "%s" % sample)  # 2011-11-24 sp -- added logging
            converts.append(sample)
        return converts

    def __reporter(self, reportType, field1, field2, *args):
        '''Private method that we use as a callback into the control centre for
        reporting status or error messages to us. We then call the 
        reportStatus() or reportError() methods accordingly. 
        The reportType parameter determines which one we call; by having a 
        reportType parameter, we can expand this in the future if we want. 
        It also means that we don't have to register multiple callbacks to 
        handle different types of reports.
        field1 and field2 are required parameters for the called reporting
        methods. We can then feed in as many other arguments as we want.
        '''
        funcReference = __name__ + '.__reporter'  # 2011-11-24 sp -- added logging
        try:
            func = getattr(self, "_report%s" % reportType)
            func(field1, field2, *args)
        except AttributeError:
            self.logger.logError("II: Invalid report type: %s" % (reportType))
            self.svrLog.logError('', self.logPrefix, funcReference, "Invalid report type=%s" % reportType)  # 2011-11-24 sp -- added logging
        except Exception as msg:
            if( field1 == 'SHUTDOWN' ):      # 2011-12-02 sp -- normal operation so report information (not error condition)
                self.svrLog.logDebug('', self.logPrefix, funcReference, "Client has completed shutdown and is no longer available: %s" % msg)
            else:                            # 2011-12-02 sp -- report error otherwise
                self.logger.logError("II: __reporter(): %s" % (msg))     
                self.svrLog.logError('', self.logPrefix, funcReference, "exception=%s" % msg)  # 2011-11-24 sp -- added logging




    def SetBarcodeRescanOffset(self, offset):
        print('\n>>> SetBarcodeRescanOffset: %f \n'%offset)
        self.controlCentre.SetBarcodeRescanOffset(offset);
        self.m_hardwareConfig.writeItem( 'Barcode_reader','barcode_rescan_offset_degrees' ,offset )
        self.m_hardwareConfig.write()
        rtn = 0                                          # Return Integer
        return rtn        
        
        
    def GetBarcodeRescanOffset(self): 
        print('\n>>> GetBarcodeRescanOffset: %f \n'%self.controlCentre.GetBarcodeRescanOffset())                                          
        return self.controlCentre.GetBarcodeRescanOffset();

    def SetBarcodeThetaOffset(self, offset):
        print('\n>>> SetBarcodeThetaOffset: %f \n'%offset)
        self.controlCentre.SetBarcodeThetaOffset(offset);
        self.m_hardwareConfig.writeItem( 'Barcode_reader','barcode_offset_degrees' ,offset )
        self.m_hardwareConfig.write()
        rtn = 0                                          # Return Integer
        return rtn        
        
        
    def GetBarcodeThetaOffset(self): 
        print('\n>>> GetBarcodeThetaOffset: %f \n'%self.controlCentre.GetBarcodeThetaOffset())                                          
        return self.controlCentre.GetBarcodeThetaOffset();

    def getProtocolCommandList(self, protocolIdx):
        return self.controlCentre.getProtocolCommandList(protocolIdx)
    
    def isSharingComboValid(self, sampleList, userID = 'undefined',
                 isSharing = False, sharedSectorsTranslation = [], sharedSectorsProtocolIndex = []):
        return self.controlCentre.isSharingComboValid(self._convertSamples(sampleList), userID,
                 isSharing, sharedSectorsTranslation, sharedSectorsProtocolIndex)
        
# eof

