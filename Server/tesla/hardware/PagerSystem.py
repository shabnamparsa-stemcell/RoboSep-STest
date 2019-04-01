import os
import time
import datetime
from ipl.utils.wait import wait_msecs
from tesla.hardware.config import gHardwareData, LoadSettings
from tesla.serialnum import InstrumentSerialNumber
class PagerSystemError(StandardError):
    __module__ = __name__
    __doc__ = 'Exception for errors related to talking to the PagerSystem card(s)'

class PagerSystem:
    __module__ = __name__
    __doc__ = 'Class to drive a PagerSystem. Information about the \n   '
    openPorts = {}
    SYSTEM_ERROR = 'Err->'
    PAGE_SUCCESSFUL = 'PAGED'
    
    SERIAL_PAUSE = 500
    
    PagerSystemLabel          = "PagerSystem"

    PortLabel           = "port"
    PagerIdLabel           = "pagerid" 
    SystemIdLabel           = "systemid" 
    __configData =  {
                    PortLabel               : 'COM3',
                    PagerIdLabel           : '99',
                    SystemIdLabel           : '0'
                    }
    #SYSID
    #SINPn
    #PULL
    

    def __init__(self, port = None, useEmulator = False):
        global gHardwareData
        self._m_Settings = PagerSystem.__configData.copy()
        instrumentConfigData = gHardwareData.Section (PagerSystem.PagerSystemLabel)
        LoadSettings (self._m_Settings, instrumentConfigData)

        if port == None:
            port = self._m_Settings[PagerSystem.PortLabel]

        self.pagerID = self._m_Settings[PagerSystem.PagerIdLabel]
        self.systemID = self._m_Settings[PagerSystem.SystemIdLabel]

        self.serialNumber = InstrumentSerialNumber().getSerial()
        
        self.DEBUG = True #os.environ.has_key('ROBO_DEBUG')
        self.INFO = (self.DEBUG or os.environ.has_key('ROBO_INFO'))
        if os.environ.has_key('ROBO_FORCE_EMULATION'):
            if self.INFO:
                print 'PagerSystem emulation forced'
            useEmulator = True
        self.usingEmulator = useEmulator
        if self.INFO:
            print ('Emulator status = %d' % useEmulator)
            
        if useEmulator:
            self.port = EmulatedPort(port)
            if self.INFO:
                print "WARNING: emulation mode in use! Hardware won't operate!!"
        else:
            self.port = None
            if PagerSystem.openPorts.has_key(port):
                self.port = PagerSystem.openPorts[port]
            else:
                import serial
                #import SerialComms
                from SimpleStep.SerialComms import SerialComms
                try:
                    self.port = SerialComms(port,9600)
                except serial.serialutil.SerialException, args:
                    if (str(args).find('Access is denied') >= 0):
                        errorMsg = ("Can't access port %s (%s). Locked by another resource?" % (port,
                         args))
                    else:
                        errorMsg = args
                    print "PagerSystemError: %s" % (errorMsg)
                PagerSystem.openPorts[port] = self.port
            if self.port == None or not self.reset():
                print "PagerSystemError: Could not talk to %s pager system" % (self.port)
            self.setEcho(False)
            self.setRePage(5)
            self.setTime(time.localtime()[3],time.localtime()[4])
            


    def isPresent(self):
        """Is the board present and okay? Returns true if so"""
        idx = self.process('').find(PagerSystem.SYSTEM_ERROR)
        return (idx==0)

    def reset(self):
        return self.isPresent();

    def getPort(self):
        """Return the comms port that we are attached to"""
        if self.port == None:
            return 'None'
        return self.port.portstr

    def getInfo(self):
        info = self.process('STAT',2000) #+ self.process('READ')
        return info

    #SETT
    #PAGE
    #RPG
    def setEcho(self, isEchoOn):
        if isEchoOn:
            self.process('ECHO1')
        else:
            self.process('ECHO0')

    
    def setRePage(self, min):
        return self.process('RPG,'+str(min))

    
    def page(self, msg,pagerID=None,sysID=None):

        if pagerID == None:
            pagerID=self.pagerID   
 
        if sysID == None:
            sysID=self.systemID

        msg = 'Robo-%s: %s' %(self.serialNumber, msg)
        
        result = self.process('PAGE,'+str(pagerID)+','+str(sysID)+','+msg)
        if result.find(PagerSystem.PAGE_SUCCESSFUL)!=0:
            print "PagerSystemError: Page failed. (%s)" % (msg)
        return result.find(PagerSystem.PAGE_SUCCESSFUL)==0   

    #hr in 24hr clk
    def setTime(self,hr,min):
        return self.process('SETT,'+str(hr)+','+str(min)) 
        
    def process(self, data, delay = SERIAL_PAUSE):
        """Wait until the PagerSystem is ready, then send data to the card
        and return a tuple of payload data and the status."""
        if self.DEBUG:
            print ('PagerSystem(%s): [%s] ->' % (self.getPort(),data))
        if self.port == None:
            return ''
        try:
            payload = self.port.sendAndCheck(data,delay)
        except Exception, errMsg:
            print "PagerSystemError: process failed. (%s)" % (errMsg)
            payload = None
        returnValue = payload
        if self.DEBUG:
            print ('process returnValue = [%s]' % returnValue)
        if returnValue==None:
            return ''
        return returnValue

    


