# SimpleStep.py
#
# Revisions:
# 2011-10-17     sp      modifications to V4.6 (homing functionality and using configuration file settings for home sensor)
#

import os
from .emulator.Port import Port as EmulatedPort
from ipl.utils.wait import wait_msecs
from tesla.hardware.Device import Device

import sys

import tesla.config
import time
import datetime

import random

import msvcrt
import win32api
import win32con
import win32console
import win32gui
import win32ui

import RoboTrace

from tesla.PgmLog import PgmLog    # 2011-11-23 sp -- programming logging
import tesla.DebuggerWindow      # 2012-04-10 sp

SSExtLogger = None
TIMEOUT_LIMITS = 20

class SimpleStepExtLogger:
    LOG_TOTAL_LINES  = 6000
    DMP_TOTAL_LINES  = 6000
    port             = None
    ver              = 104
    NumOfNoneError   = 0
    MAXNONEERROR     = 4096
    writeFileMode        = 'w' #SimpleStepExtLogger.writeFileMode
    def __init__(self):
        SSExtLogger  = self
        self.Extlog  = []
        self.ExtDump = []
        # 2012-01-30 sp -- replace environment variable with configuration variable
#        self.PROC      = os.environ.has_key('SS_STEP')
#        self.STEPPOPUP = os.environ.has_key('SS_STEP_POPUP')
#        self.STEPBEEP  = os.environ.has_key('SS_STEP_BEEP')
#        self.LOGX0     = os.environ.has_key('SS_LOG_X0')
#        self.LOGX1     = os.environ.has_key('SS_LOG_X1')
#        self.LOGY0     = os.environ.has_key('SS_LOG_Y0')
#        self.LOGY1     = os.environ.has_key('SS_LOG_Y1')
#        self.CMDLST    = os.environ.has_key('SS_CMDLIST')
#        self.DUMP      = os.environ.has_key('SS_DUMP')
#        self.SILENT    = os.environ.has_key('SS_LOG_SILENT')
#        self.DMPTIME   = os.environ.has_key('SS_DUMP_DISABLETIME')

        self.PROC      = tesla.config.SS_STEP
        self.STEPPOPUP = tesla.config.SS_STEP_POPUP
        self.STEPBEEP  = tesla.config.SS_STEP_BEEP
        self.LOGX0     = tesla.config.SS_LOG_X0
        self.LOGX1     = tesla.config.SS_LOG_X1
        self.LOGY0     = tesla.config.SS_LOG_Y0
        self.LOGY1     = tesla.config.SS_LOG_Y1
        self.CMDLST    = tesla.config.SS_CMDLIST
        self.DUMP      = tesla.config.SS_DUMP
        self.SILENT    = tesla.config.SS_LOG_SILENT
        self.DMPTIME   = tesla.config.SS_DUMP_DISABLETIME


    def SetTestDebugMode(self, x0, x1, y0, y1):

        self.LOGX0    = False;
        self.LOGX1    = False;
        self.LOGY0    = False;
        self.LOGY1    = False;
        self.PROC     = False;
        self.STEPBEEP = False;
        self.CMDLST   = False;

        if x0 == True:
           self.LOGX0    = True;
           self.PROC     = True;
           self.STEPBEEP = True;
           self.CMDLST   = True;

        if x1 == True:
           self.LOGX1    = True;
           self.PROC     = True;
           self.STEPBEEP = True;
           self.CMDLST   = True;

        if y0 == True:
           self.LOGY0    = True;
           self.PROC     = True;
           self.STEPBEEP = True;
           self.CMDLST   = True;

        if y1 == True:
           self.LOGY1    = True;
           self.PROC     = True;
           self.STEPBEEP = True;
           self.CMDLST   = True;

    def SetCardVer(self,ver):
        self.ver = ver

    def PlantPort(self, port):
        if self.port == None:
                  self.port = port

    def WaitKBD(self):
        handle = win32console.GetConsoleWindow()
        win32gui.ShowWindow(handle, win32con.SW_RESTORE)
        win32gui.BringWindowToTop(handle)
        win32gui.SetActiveWindow(handle)

#        win32ui.MessageBox('Press OK Button to continue!!', 'RoboSep Simple Step Debug Mode' , win32con.MB_OK  )

        while True :
              if (self.STEPBEEP):
                  print('\a') #ASCII Beep Sound!
              print ('\nRoboSep Debug: press [Enter] or [Space] Key >')
              rtn = msvcrt.getch()
              if (ord(rtn) == 32) or (ord(rtn) == 13):
                 print('Key Accepted!\n')
                 break

        if (self.STEPPOPUP):
           win32gui.ShowWindow(handle, win32con.SW_MINIMIZE)

    def GetTimeStamp(self):
        timestamp = str(datetime.datetime.now())
        strlen = len(timestamp)

        if strlen == 26 :
           rtnTime = timestamp[0: (strlen - 3)]
        elif strlen == 19:
           rtnTime = timestamp + '.000'
        elif strlen == 20:
           rtnTime = timestamp + '000'

        return rtnTime

    def CheckSystemAvail(self):
        timestamp = self.GetTimeStamp()

        if self.ver >= 106:
           chkX0 = self.port.sendAndCheck('X0?2', 20)
           chkX1 = self.port.sendAndCheck('X1?2', 20)
           chkY0 = self.port.sendAndCheck('Y0?2', 20)
           chkY1 = self.port.sendAndCheck('Y1?2', 20)

           if chkX0 != None:
              chkX0 = chkX0.strip('\r')
           if chkX1 != None:
              chkX1 = chkX1.strip('\r')
           if chkY0 != None:
              chkY0 = chkY0.strip('\r')
           if chkY1 != None:
              chkY1 = chkY1.strip('\r')

           msg1  = 'Cards Status- %s %s %s %s'% (chkX0,chkX1,chkY0,chkY1)

           self.Extlog.append(timestamp + ': ' + msg1 +'\n')

           if self.DMPTIME :
                  timestamp = ''

           self.ExtDump.append(timestamp + ': ' + msg1+'\n')

#           if not self.SILENT :
#                  print "+" + msg1

        if self.ver >= 109 :
           errX0 = self.port.sendAndCheck('X0?3', 20)
           errX1 = self.port.sendAndCheck('X1?3', 20)
           errY0 = self.port.sendAndCheck('Y0?3', 20)
           errY1 = self.port.sendAndCheck('Y1?3', 20)

           if errX0 != None:
              errX0 = errX0.strip('\r')
           if errX1 != None:
              errX1 = errX1.strip('\r')
           if errY0 != None:
              errY0 = errY0.strip('\r')
           if errY1 != None:
              errY1 = errY1.strip('\r')

           msg2  = 'Cards Error - %s %s %s %s'% (errX0,errX1,errY0,errY1)

           self.Extlog.append(timestamp + ': ' + msg2 +'\n')

           if self.DMPTIME :
              timestamp = ''

           self.ExtDump.append(timestamp + ': ' + msg2 +'\n')

#           if not self.SILENT :
#                  print "+" + msg2

        if len(self.Extlog) >= self.LOG_TOTAL_LINES :
           self.Extlog.pop(0)
        if len(self.ExtDump) >= self.DMP_TOTAL_LINES :
           self.ExtDump.pop(0)

    def SetCmdListLog(self, data, addr):
        if self.CMDLST :

          timestamp = self.GetTimeStamp()

          self.Extlog.append(timestamp + ': '+ str(data) +'\n')

          if len(self.Extlog) >= self.LOG_TOTAL_LINES :
             self.Extlog.pop(0)

          if self.DMPTIME :
             timestamp = ''

          if self.LOGX0 :
             if addr == 'X0':
                     if not self.SILENT :
                        print("+" + data)
                     self.ExtDump.append(timestamp + ': ' + str(data) +'\n')
                     if self.PROC :
                              self.WaitKBD()
          if self.LOGX1:
             if addr == 'X1':
                     if not self.SILENT :
                        print("+" + data)
                     self.ExtDump.append(timestamp + ': ' + str(data) +'\n')
                     if self.PROC :
                              self.WaitKBD()
          if self.LOGY0:
             if addr == 'Y0':
                     if not self.SILENT :
                        print("+" + data)
                     self.ExtDump.append(timestamp + ': ' + str(data) +'\n')
                     if self.PROC :
                              self.WaitKBD()
          if self.LOGY1:
             if addr == 'Y1':
                     if not self.SILENT :
                        print("+" + data)
                     self.ExtDump.append(timestamp + ': ' + str(data) +'\n')
                     if self.PROC :
                              self.WaitKBD()

          if len(self.ExtDump) >= self.DMP_TOTAL_LINES :
             self.ExtDump.pop(0)
        else:
          self.SetLog(data, addr)

    def SetLog(self, data, addr):

        timestamp = self.GetTimeStamp()

        self.Extlog.append(timestamp + ': '+ str(data) +'\n')

        if len(self.Extlog) >= self.LOG_TOTAL_LINES :
           self.Extlog.pop(0)

        if self.CMDLST:
           pass
        else:
          if self.DMPTIME :
             timestamp = ''
          if self.LOGX0 :
             if addr == 'X0':
                     if not self.SILENT :
                        print("+" + data)
                     self.ExtDump.append(timestamp + ': '+ str(data) +'\n')
                     if self.PROC :
                              self.WaitKBD()
          if self.LOGX1:
             if addr == 'X1':
                     if not self.SILENT :
                        print("+" + data)
                     self.ExtDump.append(timestamp + ': '+ str(data) +'\n')
                     if self.PROC :
                              self.WaitKBD()

          if self.LOGY0:
             if addr == 'Y0':
                     if not self.SILENT :
                        print("+" + data)
                     self.ExtDump.append(timestamp + ': '+ str(data) +'\n')
                     if self.PROC :
                              self.WaitKBD()

          if self.LOGY1:
             if addr == 'Y1':
                     if not self.SILENT :
                        print("+" + data)
                     self.ExtDump.append(timestamp + ': '+ str(data) +'\n')
                     if self.PROC :
                              self.WaitKBD()


          if len(self.ExtDump) >= self.DMP_TOTAL_LINES :
             self.ExtDump.pop(0)

    def DumpHistory(self):
        month    = ('Dummy','Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec')
        now      = time.localtime()
        self.filename = tesla.config.LOG_DIR+'\\SSE' + ('%04d%s%02d-%02dh%02dm%02ds.err' % (now.tm_year, month[now.tm_mon], now.tm_mday, now.tm_hour, now.tm_min, now.tm_sec))
        print(self.filename)
        try:
              hFile = open(self.filename,SimpleStepExtLogger.writeFileMode)
              hFile.writelines(self.Extlog)
              hFile.close()
              print('Error Log Created!!!')
        except IOError:
              print('Cannot open/write a log file!\n')

    def DumpPollHistory(self):
        month    = ('Dummy','Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec')
        now      = time.localtime()
        self.filename = tesla.config.LOG_DIR+'\\SSE' + ('%04d%s%02d-%02dh%02dm%02ds_Poll.err' % (now.tm_year, month[now.tm_mon], now.tm_mday, now.tm_hour, now.tm_min, now.tm_sec))
        print(self.filename)
        try:
              hFile = open(self.filename,SimpleStepExtLogger.writeFileMode)
              hFile.writelines(self.Extlog)
              hFile.close()
              print('Error Log Created!!!')
        except IOError:
              print('Cannot open/write a log file!\n')

    def DumpNoneHistory(self):
        month    = ('Dummy','Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec')
        now      = time.localtime()
        self.filename = tesla.config.LOG_DIR+'\\SSE' + ('%04d%s%02d-%02dh%02dm%02ds_NONE.err' % (now.tm_year, month[now.tm_mon], now.tm_mday, now.tm_hour, now.tm_min, now.tm_sec))
        try:
              self.NumOfNoneError = self.NumOfNoneError + 1
              if self.ver >= 109 :
                 print('############# Detect [None] Error #####################')
                 ln = len(self.Extlog)
                 hFile = open(self.filename,SimpleStepExtLogger.writeFileMode)
                 hFile.writelines(self.Extlog[((ln)-50):])
                 hFile.close()
                 print('Error Log Created!!!')
                 self.NumOfNoneError = 0
              else:
                   if self.NumOfNoneError > self.MAXNONEERROR :
                      print('############# Detect [None] Error #####################')
                      ln = len(self.Extlog)
                      hFile = open(self.filename,SimpleStepExtLogger.writeFileMode)
                      hFile.writelines(self.Extlog)
                      hFile.close()
                      print('Error Log Created!!!')
                      self.NumOfNoneError = 0

        except IOError:
              print('Cannot open/write a log file!\n')

    def DumpBackUpHistory(self):
        self.filename = tesla.config.LOG_DIR+'\\SSE.trm'
        print(self.filename)
        try:
              hFile = open(self.filename,SimpleStepExtLogger.writeFileMode)
              hFile.writelines(self.Extlog)
              hFile.close()
              print('Terminate Log Created!!!')
        except IOError:
              print('Cannot open/write a log file!\n')

    def Dump(self):
        if self.DUMP :
            self.filename = tesla.config.LOG_DIR+'\\SSE.dmp'
            print(self.filename)
            try:
                hFile = open(self.filename,SimpleStepExtLogger.writeFileMode)
                hFile.writelines(self.ExtDump)
                hFile.close()
                print('SS Dump Created!!!')
            except IOError:
                print('Cannot open/write a Dump file!\n')

def GetSSExtLoggerInstance():
    global SSExtLogger

    if SSExtLogger == None:
       SSExtLogger = SimpleStepExtLogger()

    return SSExtLogger


class SimpleStepError(Exception):
    __module__ = __name__
    __doc__ = 'Exception for errors related to talking to the SimpleStep card(s)'


class SimpleStep:
    __module__ = __name__
    __doc__ = 'Class to drive a SimpleStep stepper card. Information about the \n    low-level commands can be found in the SimpleStep manual (which can be\n    downloaded from the SimpleStep website in PDF format).\n    http://www.simplestep.com/\n    '
    openPorts         = {}
    SYSTEM_READY      = '>'
    SYSTEM_BUSY       = 'b'
    SYSTEM_ABORT      = 'a'
    SYSTEM_NOT_HOMED  = 's'
    SYSTEM_BAD_H      = 'h'
    
    RETRIES           = 200

    # 2012-01-30 sp -- replace environment variable with configuration variable
    #if os.environ.has_key('SS_LEGACY'):
    # 2019-06-09 RL moved if tesla.config.SS_LEGACY to __init__ because might not be initialize yet here. AttributeError: module 'tesla.config' has no attribute 'SS_LEGACY'

    SERIAL_PAUSE      = 100
    POLL_PAUSE        = 100
    RETRY_PAUSE       = 100

    OFFLINE_PAUSE     = 1000
    ver               = 104
    dummy             = 0
    
    # 2011-10-21 sp -- variable to track homing status
    m_HomingError = True
    m_stepsToHome = 0
        
    def __init__(self, boardAddress, port = 'COM1', useEmulator = False):
        """SimpleStep constructor
        boardAddress: the address of the board; format = AN
                      where A = alphabetic character and N = numeric character
        port: the serial port that the card(s) is/are connected to
        useEmulator: boolean flag to control using the emulator

        Two levels of debugging can be turned on via environment variables:
        SS_DEBUG turns on full debugging dumps to the console
        SS_INFO turns on a small amount of useful information to the console

        Emulation can be forced (overriding the useEmulator parameter) by
        setting the SS_FORCE_EMULATION environment variable.
        """
        if tesla.config.SS_LEGACY == 1:
           SERIAL_PAUSE      = 20
           POLL_PAUSE        = 20
           RETRY_PAUSE       = 20
       
        if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
            self.ExtLogger = GetSSExtLoggerInstance()
        if( tesla.config.SS_DEBUGGER_LOG == 1 ):
            self.ssLogDebugger = tesla.DebuggerWindow.GetSSTracerInstance()

        # 2011-11-24 -- sp
        self.svrLog = PgmLog( 'svrLog' )
        funcReference = __name__ + '.__init__'  # 2011-11-29 -- sp
        
        # 2012-01-30 sp -- replace environment variable with configuration variable
        #self.DEBUG = os.environ.has_key('SS_DEBUG')
        self.DEBUG = tesla.config.SS_DEBUG
        #self.INFO = (self.DEBUG or os.environ.has_key('SS_INFO'))
        self.INFO = (self.DEBUG or tesla.config.SS_INFO)
        # 2012-01-30 sp -- replace environment variable with configuration variable
        #if os.environ.has_key('SS_FORCE_EMULATION'):
        if tesla.config.SS_FORCE_EMULATION == 1:
            if self.INFO:
                print('SimpleStep emulation forced')
            useEmulator = True
        (self.board, self.address,)   = self._SimpleStep__parseBoardAddress(boardAddress)
        self.prefix                   = ('%s%d' % (self.board, self.address))
        self.logPrefix                = ('%s%d' % (self.board, self.address))

        # 2011-11-24 -- sp
        self.svrLog.logVerbose('X', self.logPrefix, funcReference, '###### Start initializing SimpleStep Object! ######' )  # 2011-11-29 -- sp

        if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
            self.ExtLogger.SetLog('###### Start initializing SimpleStep Object! ######', self.prefix )    #CWJ MOD

        self.prefixResponse           = self.prefix.lower()
        self.goodResponse             = (self.prefixResponse + SimpleStep.SYSTEM_READY)
        self.busyResponse             = (self.prefixResponse + SimpleStep.SYSTEM_BUSY)
        self.abortResponse            = (self.prefixResponse + SimpleStep.SYSTEM_ABORT)
        self.notHomedResponse         = (self.prefixResponse + SimpleStep.SYSTEM_NOT_HOMED)
        self.usingEmulator            = useEmulator
        
        # 2011-10-21 sp -- variable to track homing status
        self.m_HomingError = False
        self.m_stepsToHome = 0

        if self.INFO:
            print(('Emulator status = %d' % useEmulator))
            print(('  Board address = %s' % boardAddress))

        if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
            self.ExtLogger.SetLog('Emulator status = %d' % useEmulator,  self.prefix)
            self.ExtLogger.SetLog('  Board address = %s' % boardAddress, self.prefix)
        
        self.svrLog.logVerbose('X', self.logPrefix, funcReference, 'Emulator status=%d |Board address=%s' % (useEmulator, boardAddress) )  # 2011-11-29 -- sp

        if useEmulator:
            self.port = EmulatedPort(port)
            if self.INFO:
                print("WARNING: emulation mode in use! Hardware won't operate!!")
            if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                self.ExtLogger.SetLog("WARNING: emulation mode in use! Hardware won't operate!!", self.prefix)
            self.svrLog.logDebug('X', self.logPrefix, funcReference, "WARNING: emulation mode in use! Hardware won't operate!!" )  # 2011-11-29 -- sp

        else:
            if port in SimpleStep.openPorts:
                self.port = SimpleStep.openPorts[port]
            else:
                import serial
                from . import SerialComms
                
                #YW: Comment out the Correction for serial port connection failure temporarily
                
                #for attempt in range (TIMEOUT_LIMITS):
                #    time.sleep(3)
                #    try:
                #        print ">>>>>> Attempt Number is: %d" % attempt
                #        self.port = SerialComms.SerialComms(port)
                #    except serial.serialutil.SerialException, args:
                #        continue
                #    else:
                #        break
                #else:
                #    print ">>>>>> Error is: %s" % args
                #    if (str(args).find('Access is denied') >= 0):
                #        errorMsg = ("Can't access port %s (%s). Locked by another resource?" % (port,args))
                #        RoboTrace.RoboSepMessageBox("Cannot Open COM Port!, Restart RoboSep!");
                #    else:
                #        errorMsg = args
                
                # YW: need to take out the if/else when above correction put in place
                try:
                    self.port = SerialComms.SerialComms(port)
                except serial.serialutil.SerialException as args:
                    
                    if (str(args).find('Access is denied') >= 0):
                        errorMsg = ("Can't access port %s (%s). Locked by another resource?" % (port,args))
                        RoboTrace.RoboSepMessageBox("Cannot Open COM Port!, Restart RoboSep!");
                    else:
                        errorMsg = args
                    
                    if self.DEBUG:
                        print(errorMsg)
                        

                    self.svrLog.logError('X', self.logPrefix, funcReference, errorMsg )  # 2011-11-29 -- sp
                    if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                        self.ExtLogger.SetLog(errorMsg, self.prefix)
#                       self.ExtLogger.CheckSystemAvail()

                        self.ExtLogger.DumpHistory()
                        
                    RoboTrace.RoboSepMessageBox("Cannot Open COM Port!")
                    raise SimpleStepError(errorMsg)
                    

                SimpleStep.openPorts[port] = self.port
                if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                    self.ExtLogger.PlantPort(self.port)
                self.svrLog.logDebug('X', self.logPrefix, funcReference, "comm port=%s" % self.port )  # 2011-11-29 -- sp

            if (not self.isPresent()):
                self.svrLog.logError('X', self.logPrefix, funcReference, "Could not talk to %s card: status = '%s'" % (self.prefix,self.getStatus()) )  # 2011-11-29 -- sp
                if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                    self.ExtLogger.SetLog("Could not talk to %s card: status = '%s'" % (self.prefix,self.getStatus()), self.prefix)
#                self.ExtLogger.CheckSystemAvail()
                    self.ExtLogger.DumpHistory()
                raise SimpleStepError("Could not talk to %s card: status = '%s'" % (self.prefix,self.getStatus()))

        # Log the axis and associated version of stepper card firmware
        Device.logger.logInfo("Stepper: Board connection %s (%s)" % (self.board, self.address))
        self.svrLog.logInfo('', self.logPrefix, funcReference, "Stepper: Board connection %s (%s)" % (self.board, self.address) )  # 2011-11-29 -- sp
        if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
            self.ExtLogger.SetLog( "Stepper: Board connection %s (%s)" % (self.board, self.address) , self.prefix)
            self.ExtLogger.SetLog('###### Finish initializing SimpleStep Object! ######', self.prefix)


    def getStatus(self):
        """Returns the status of the board"""
        (result, status,) = self.sendAndCheck('')
        return status


    def isPresent(self):
        """Is the board present and okay? Returns true if so"""
        return (self.getStatus() == SimpleStep.SYSTEM_READY)


    def getPort(self):
        """Return the comms port that we are attached to"""
        return self.port.portstr


    def getBoardInfo(self, param):    ##Check The Other modules
        """Return current board information, parameter ranges from 0 - 6"""
        msg = self.sendAndCheck(('v%d' % param))[0]

        # 2012-01-30 sp -- replace environment variable with configuration variable
        #if os.environ.has_key('SS_FORCE_EMULATION'):
        if tesla.config.SS_FORCE_EMULATION == 1:
           ver = 104
        else:
           ver = int(msg)

        if param == 0:
            self.ver = ver
            if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                self.ExtLogger.SetCardVer(self.ver)
            funcReference = __name__ + '.getBoardInfo'  # 2011-11-29 -- sp
            self.svrLog.logVerbose('X', self.logPrefix, funcReference, 'version=%d' % self.ver )  # 2011-11-29 -- sp
    
        return msg


    def getPosition(self):
        """Returns the current motor position"""
        (result, status,) = self.sendAndCheck('m')
        funcReference = __name__ + '.getPosition'  # 2011-11-29 -- sp
        try:
            pos = int(result)

            if self.DEBUG:
                print(('getPosition() = %d' % pos))

            if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                self.ExtLogger.SetLog('getPosition() = %d' % pos, self.prefix)
            self.svrLog.logVerbose('X', self.logPrefix, funcReference, 'pos=%d' % pos )  # 2011-11-29 -- sp

        except:
            if self.DEBUG:
                print(('getPosition() failed on [%s]' % result))

            self.svrLog.logError('X', self.logPrefix, funcReference, "Card %s: Can't convert result (%s) to integer" % (self.prefix,result) )  # 2011-11-29 -- sp
            if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                self.ExtLogger.SetLog( "Card %s: Can't convert result (%s) to integer" % (self.prefix,result), self.prefix )
                self.ExtLogger.CheckSystemAvail()
                self.ExtLogger.DumpHistory()
            raise SimpleStepError("Card %s: Can't convert result (%s) to integer" % (self.prefix,result))
        return pos


    def getVelocityAndSlope(self):
        """Returns a list of begin velocity, end velocity and slope settings"""
        settings = []
        for cmd in ['b', 'e', 's']:
            (result, status,) = self.sendAndCheck(cmd)
            settings.append(int(result[:-1]))

        if self.DEBUG:
            print('getVelocityAndSlope: settings =', end=' ')
            print(settings)

        funcReference = __name__ + '.getVelocityAndSlope'  # 2011-11-29 sp -- function not called from within, not tested
        self.svrLog.logDebug('Extxx', self.logPrefix, funcReference, 'settings='+ settings )  # 2011-11-29 -- sp
        if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
            self.ExtLogger.SetLog( 'getVelocityAndSlope: settings ='+ settings, self.prefix )

        return settings



    def setOutputState(self, port, state):
        """Set the state of a digital output port"""
        funcReference = __name__ + '.setOutputState'  # 2011-11-29 -- sp
        if (port < 0):
            self.svrLog.logError('X', self.logPrefix, funcReference, 'Invalid port number (%d)' % (port) )  # 2011-11-29 -- sp
            if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                self.ExtLogger.SetLog(('Card %s: Invalid port number (%d)' % (self.prefix,port)), self.prefix)
                self.ExtLogger.CheckSystemAvail()
                self.ExtLogger.DumpHistory()
            raise SimpleStepError('Card %s: Invalid port number (%d)' % (self.prefix,port))
        elif (state not in [0,1]):
            self.svrLog.logError('X', self.logPrefix, funcReference, 'State must be 0 or 1 (not %d)' % (state) )  # 2011-11-29 -- sp
            if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                self.ExtLogger.SetLog(('Card %s: State must be 0 or 1 (not %d)' % (self.prefix,state)), self.prefix)
                self.ExtLogger.CheckSystemAvail()
                self.ExtLogger.DumpHistory()
            raise SimpleStepError('Card %s: State must be 0 or 1 (not %d)' % (self.prefix,state))
        else:
            if self.DEBUG:
                print(('setOutputState() %d %d' % (port,state)))
            self.svrLog.logDebug('', self.logPrefix, funcReference, 'set Port:%d, State:%d'%(port,state) )  # 2011-11-29 -- sp
            if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                self.ExtLogger.SetCmdListLog(('setOutputState()[%s]: Port:%d, State:%d'%(self.prefix,port,state)), self.prefix)
            self.processCmds([('O%d,%d' % (port,state)),'W50'])

#            if self.DEBUG:
#                print 'setOutputState() complete\n'

    def setPumpHomePowerCommand(self, powercommand ):
        self.m_PumpHomePower = powercommand;
        
    def setPumpStandardPowerCommand(self, powercommand):
        self.m_PumpStandardPower = powercommand;
            
    def setPumpHomeStepSpeedCommand(self, speedcommand):
        self.m_PumpHomeStepSpeed = speedcommand;
        
    def setPumpStandardStepSpeedCommand(self, speedcommand):
        self.m_PumpStandardStepSpeed = speedcommand;
    
    def setPumpHomeCommand(self, homecommand):
        self.m_PumpHomeCommand = homecommand;

    def setPumpPositions(self, stop, delta, inc):
        self.m_PumpStopPos  = stop
        self.m_PumpDeltaPos = delta
        self.m_PumpIncPos   = inc

    def runBadStatushException(self):
        maxPos = 0x7FFFFFFF
        allowableStatus = [SimpleStep.SYSTEM_READY, SimpleStep.SYSTEM_NOT_HOMED];        
        pump_home_command  = self.m_HomeMotorCmd;
        pump_home_commandz = self.m_HomeMotorCmd + 'z';
        pump_zero_command  = self.m_InitMotorCmd;
        if( tesla.config.SS_EXT_LOGGER == 1 ): 
           self.ExtLogger.SetCmdListLog('---------->Starting  runBadStatushException',self.prefix);

        rtnMsg = self.sendAndCheck('?2', self.SERIAL_PAUSE);
        rtnMsg = self.sendAndCheck('?3', self.SERIAL_PAUSE);

        rtnMsg = self.sendAndCheck('v29', self.SERIAL_PAUSE);
        rtnMsg = self.sendAndCheck('v26', self.SERIAL_PAUSE);

        rtnMsg = self.sendAndCheck('m',  self.SERIAL_PAUSE)
        currentPos    = int(rtnMsg[0]) 
        movedPos      = maxPos - currentPos
        currentAbsPos = self.m_savedPumpPos - movedPos
        # cmd           = ('A%d'%(currentAbsPos))
        cmd           = 'A0'
        
        (result, status,) = self.sendAndCheck( 'z'  , self.SERIAL_PAUSE);
        if ((not (status in allowableStatus)) and (status != None)):
            Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
            if( tesla.config.SS_EXT_LOGGER == 1 ): 
               self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
               self.ExtLogger.CheckSystemAvail()
               self.ExtLogger.DumpHistory()
            raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))

        (result, status,) = self.sendAndCheck(pump_zero_command , self.SERIAL_PAUSE);
        if ((not (status in allowableStatus)) and (status != None)):
            Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
            if( tesla.config.SS_EXT_LOGGER == 1 ): 
               self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
               self.ExtLogger.CheckSystemAvail()
               self.ExtLogger.DumpHistory()
            raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))

        (result, status,) = self.sendAndCheck( cmd  , self.SERIAL_PAUSE);
        if ((not (status in allowableStatus)) and (status != None)):
            Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
            if( tesla.config.SS_EXT_LOGGER == 1 ): 
               self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
               self.ExtLogger.CheckSystemAvail()
               self.ExtLogger.DumpHistory()
            raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))

        (result, status,) = self.sendAndCheck(self.m_PumpHomePower, self.SERIAL_PAUSE);
        if ((not (status in allowableStatus)) and (status != None)):
            Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
            if( tesla.config.SS_EXT_LOGGER == 1 ): 
               self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
               self.ExtLogger.CheckSystemAvail()
               self.ExtLogger.DumpHistory()
            raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))

        (result, status,) = self.sendAndCheck(self.m_PumpHomeStepSpeed[0]);
        if ((not (status in allowableStatus)) and (status != None)):
            Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
            if( tesla.config.SS_EXT_LOGGER == 1 ): 
               self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
               self.ExtLogger.CheckSystemAvail()
               self.ExtLogger.DumpHistory()
            raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))

        (result, status,) = self.sendAndCheck(self.m_PumpHomeStepSpeed[1]);
        if ((not (status in allowableStatus)) and (status != None)):
            Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
            if( tesla.config.SS_EXT_LOGGER == 1 ): 
               self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
               self.ExtLogger.CheckSystemAvail()
               self.ExtLogger.DumpHistory()
            raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))

        (result, status,) = self.sendAndCheck(self.m_PumpHomeStepSpeed[2]);
        if ((not (status in allowableStatus)) and (status != None)):
            Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
            if( tesla.config.SS_EXT_LOGGER == 1 ): 
               self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
               self.ExtLogger.CheckSystemAvail()
               self.ExtLogger.DumpHistory()
            raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))

        rtnMsg = self.sendAndCheck('I1', self.SERIAL_PAUSE)
        tripSensor = int(rtnMsg[0])
        if (tripSensor == 1):
            try: 
              cmd = ('RN+%d'%(self.m_PumpDeltaPos));
              rtnMsg  = self.sendAndCheck( cmd , self.SERIAL_PAUSE);

              rtnMsg = self.sendAndCheck('m', self.SERIAL_PAUSE)
              excPos = int(rtnMsg[0])
              rtnMsg = self.sendAndCheck('I1', self.SERIAL_PAUSE)
              excSensor = int(rtnMsg[0])
              while ((excPos > -(self.m_PumpDeltaPos)) and ( excSensor == 0)): 
                 cmd = ('RN-%d'%(self.m_PumpIncPos));
                 rtnMsg = self.sendAndCheck( cmd, self.SERIAL_PAUSE);
                 rtnMsg = self.sendAndCheck('m', self.SERIAL_PAUSE)
                 excPos = int(rtnMsg[0])
                 rtnMsg = self.sendAndCheck('I1', self.SERIAL_PAUSE)
                 excSensor = int(rtnMsg[0])
                 
              if (excSensor == 0):
                 (result, status,) = self.sendAndCheck(pump_home_commandz, self.SERIAL_PAUSE);
                 if ((not (status in allowableStatus)) and (status != None)):
                    Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
                    if( tesla.config.SS_EXT_LOGGER == 1 ): 
                       self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
                       self.ExtLogger.CheckSystemAvail()
                       self.ExtLogger.DumpHistory()
                    raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))

                 (result, status,) = self.sendAndCheck( 'A0' , self.SERIAL_PAUSE);
                 if ((not (status in allowableStatus)) and (status != None)):
                    Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
                    if( tesla.config.SS_EXT_LOGGER == 1 ): 
                       self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
                       self.ExtLogger.CheckSystemAvail()
                       self.ExtLogger.DumpHistory()
                    raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))
                 
                 cmd = ( 'M+%d'%(currentPos) )
                 (result, status,) = self.sendAndCheck( cmd , self.SERIAL_PAUSE);         
                 if ((not (status in allowableStatus)) and (status != None)):
                    Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
                    if( tesla.config.SS_EXT_LOGGER == 1 ): 
                       self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
                       self.ExtLogger.CheckSystemAvail()
                       self.ExtLogger.DumpHistory()
                    raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))
                 if( tesla.config.SS_EXT_LOGGER == 1 ): 
                    self.ExtLogger.SetCmdListLog('<----------Pump Home failed, retry step-and-seek #XX',self.prefix);
                 return;
              if( tesla.config.SS_EXT_LOGGER == 1 ): 
                 self.ExtLogger.SetCmdListLog('<----------Finishing  runBadStatushException',self.prefix);
              return;
            except:
              if( tesla.config.SS_EXT_LOGGER == 1 ): 
                 self.ExtLogger.SetCmdListLog('<----------Pump Home failed (Try-Except Error)',self.prefix);
        else:  
            rtnMsg = self.sendAndCheck( 'RN-10', self.SERIAL_PAUSE);
            rtnMsg = self.sendAndCheck( 'A0', self.SERIAL_PAUSE);
            if( tesla.config.SS_EXT_LOGGER == 1 ): 
               self.ExtLogger.SetCmdListLog('<----------Finishing  runBadStatushException',self.prefix);
            return
        
    def setPumpHome(self, bSkipTrip = False):
        allowableStatus = [SimpleStep.SYSTEM_READY, SimpleStep.SYSTEM_NOT_HOMED];
        
        pump_home_command  = self.m_HomeMotorCmd;
        pump_home_commandz = self.m_HomeMotorCmd + 'z';
        
        if (bSkipTrip == True):
          if( tesla.config.SS_EXT_LOGGER == 1 ): 
             self.ExtLogger.SetCmdListLog('------>Starting  Non-fatal pump homing',self.prefix);
          (result, status,) = self.sendAndCheck(self.m_PumpHomeCommand, self.SERIAL_PAUSE);
          if ((not (status in allowableStatus)) and (status != None)):
              Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
              if( tesla.config.SS_EXT_LOGGER == 1 ): 
                 self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
                 self.ExtLogger.CheckSystemAvail()
                 self.ExtLogger.DumpHistory()
              raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))
          (result, status,) = self.sendAndCheck(self.m_PumpHomePower, self.SERIAL_PAUSE);
          if ((not (status in allowableStatus)) and (status != None)):
              Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
              if( tesla.config.SS_EXT_LOGGER == 1 ): 
                 self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
                 self.ExtLogger.CheckSystemAvail()
                 self.ExtLogger.DumpHistory()
              raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))
          (result, status,) = self.sendAndCheck(self.m_PumpHomeStepSpeed[0]);
          if ((not (status in allowableStatus)) and (status != None)):
              Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
              if( tesla.config.SS_EXT_LOGGER == 1 ): 
                 self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
                 self.ExtLogger.CheckSystemAvail()
                 self.ExtLogger.DumpHistory()
              raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))
          (result, status,) = self.sendAndCheck(self.m_PumpHomeStepSpeed[1]);
          if ((not (status in allowableStatus)) and (status != None)):
              Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
              if( tesla.config.SS_EXT_LOGGER == 1 ): 
                 self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
                 self.ExtLogger.CheckSystemAvail()
                 self.ExtLogger.DumpHistory()
              raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))
          (result, status,) = self.sendAndCheck(self.m_PumpHomeStepSpeed[2]);
          if ((not (status in allowableStatus)) and (status != None)):
              Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
              if( tesla.config.SS_EXT_LOGGER == 1 ): 
                 self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
                 self.ExtLogger.CheckSystemAvail()
                 self.ExtLogger.DumpHistory()
              raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))
          (result, status,) = self.sendAndCheck('M+0', self.SERIAL_PAUSE);
          # if disallowed status, raise error for Axis and report in log
          if ((not (status in allowableStatus)) and (status != None)):
              Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
              if( tesla.config.SS_EXT_LOGGER == 1 ): 
                 self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
                 self.ExtLogger.CheckSystemAvail()
                 self.ExtLogger.DumpHistory()
              raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))
          wait_msecs(300);
          (result, status,) = self.sendAndCheck('A0', self.SERIAL_PAUSE);        
          # if disallowed status, raise error for Axis and report in log
          if ((not (status in allowableStatus)) and (status != None)):
              Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
              if( tesla.config.SS_EXT_LOGGER == 1 ): 
                 self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
                 self.ExtLogger.CheckSystemAvail()
                 self.ExtLogger.DumpHistory()
              raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))
          (result, status,) = self.sendAndCheck(self.m_PumpStandardStepSpeed[0], self.SERIAL_PAUSE);
          # if disallowed status, raise error for Axis and report in log
          if ((not (status in allowableStatus)) and (status != None)):
              Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
              if( tesla.config.SS_EXT_LOGGER == 1 ): 
                 self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
                 self.ExtLogger.CheckSystemAvail()
                 self.ExtLogger.DumpHistory()
              raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))
          (result, status,) = self.sendAndCheck(self.m_PumpStandardStepSpeed[1], self.SERIAL_PAUSE);
          # if disallowed status, raise error for Axis and report in log
          if ((not (status in allowableStatus)) and (status != None)):
              Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
              if( tesla.config.SS_EXT_LOGGER == 1 ): 
                 self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
                 self.ExtLogger.CheckSystemAvail()
                 self.ExtLogger.DumpHistory()
              raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))
          (result, status,) = self.sendAndCheck(self.m_PumpStandardStepSpeed[2], self.SERIAL_PAUSE);
          # if disallowed status, raise error for Axis and report in log
          if ((not (status in allowableStatus)) and (status != None)):
              Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
              if( tesla.config.SS_EXT_LOGGER == 1 ): 
                 self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
                 self.ExtLogger.CheckSystemAvail()
                 self.ExtLogger.DumpHistory()
              raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))
          (result, status,) = self.sendAndCheck(self.m_PumpStandardPower, self.SERIAL_PAUSE);
          # if disallowed status, raise error for Axis and report in log
          if ((not (status in allowableStatus)) and (status != None)):
              Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
              if( tesla.config.SS_EXT_LOGGER == 1 ): 
                 self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
                 self.ExtLogger.CheckSystemAvail()
                 self.ExtLogger.DumpHistory()
              raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))
          if( tesla.config.SS_EXT_LOGGER == 1 ):          
              self.ExtLogger.SetCmdListLog('<------Finishing Non-fatal pump homing',self.prefix);
        else:    
          (result, status,) = self.sendAndCheck(self.m_PumpHomeCommand, self.SERIAL_PAUSE);
          if ((not (status in allowableStatus)) and (status != None)):
              Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
              if( tesla.config.SS_EXT_LOGGER == 1 ): 
                 self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
                 self.ExtLogger.CheckSystemAvail()
                 self.ExtLogger.DumpHistory()
              raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))
          (result, status,) = self.sendAndCheck('m', self.SERIAL_PAUSE)
          if ((not (status in allowableStatus)) and (status != None)):
              Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
              if( tesla.config.SS_EXT_LOGGER == 1 ): 
                 self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
                 self.ExtLogger.CheckSystemAvail()
                 self.ExtLogger.DumpHistory()
              raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))
          rtnMsg = (result, status,)
          self.m_savedPumpPos = int(rtnMsg[0])
          cmd = ('M+%s'%(self.m_PumpStopPos));
          (result, status,) = self.sendAndCheck(cmd, self.SERIAL_PAUSE);
          # if disallowed status, raise error for Axis and report in log
          if ((not (status in allowableStatus)) and (status != None)):
              Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
              if( tesla.config.SS_EXT_LOGGER == 1 ): 
                 self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
                 self.ExtLogger.CheckSystemAvail()
                 self.ExtLogger.DumpHistory()
              raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))
          (result, status,) = self.sendAndCheck(self.m_PumpHomePower, self.SERIAL_PAUSE);
          if ((not (status in allowableStatus)) and (status != None)):
              Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
              if( tesla.config.SS_EXT_LOGGER == 1 ): 
                 self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
                 self.ExtLogger.CheckSystemAvail()
                 self.ExtLogger.DumpHistory()
              raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))
          (result, status,) = self.sendAndCheck(self.m_PumpHomeStepSpeed[0]);
          if ((not (status in allowableStatus)) and (status != None)):
              Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
              if( tesla.config.SS_EXT_LOGGER == 1 ): 
                 self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
                 self.ExtLogger.CheckSystemAvail()
                 self.ExtLogger.DumpHistory()
              raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))
          (result, status,) = self.sendAndCheck(self.m_PumpHomeStepSpeed[1]);
          if ((not (status in allowableStatus)) and (status != None)):
              Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
              if( tesla.config.SS_EXT_LOGGER == 1 ): 
                 self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
                 self.ExtLogger.CheckSystemAvail()
                 self.ExtLogger.DumpHistory()
              raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))
          (result, status,) = self.sendAndCheck(self.m_PumpHomeStepSpeed[2]);
          if ((not (status in allowableStatus)) and (status != None)):
              Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
              if( tesla.config.SS_EXT_LOGGER == 1 ): 
                 self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
                 self.ExtLogger.CheckSystemAvail()
                 self.ExtLogger.DumpHistory()
              raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))
          (result, status,) = self.sendAndCheck(pump_home_commandz, self.SERIAL_PAUSE);
          # if disallowed status, raise error for Axis and report in log
          if ((not (status in allowableStatus)) and (status != None)):
              Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
              if( tesla.config.SS_EXT_LOGGER == 1 ): 
                 self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
                 self.ExtLogger.CheckSystemAvail()
                 self.ExtLogger.DumpHistory()
              raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))
          wait_msecs(300);
          (result, status,) = self.sendAndCheck('A0', self.SERIAL_PAUSE);        
          # if disallowed status, raise error for Axis and report in log
          if ((not (status in allowableStatus)) and (status != None)):
              Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
              if( tesla.config.SS_EXT_LOGGER == 1 ): 
                 self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
                 self.ExtLogger.CheckSystemAvail()
                 self.ExtLogger.DumpHistory()
              raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))
          wait_msecs(350);                                                        #Insert on 205-07-16 CJ
          if tesla.config.SS_FORCE_EMULATION == 1:
            if( tesla.config.SS_EXT_LOGGER == 1 ): 
               self.ExtLogger.SetCmdListLog(' Emulator mode: RN-10',self.prefix);
            if tesla.config.SS_BAD_H_TEST == 1:
               if( tesla.config.SS_EXT_LOGGER == 1 ): 
                  self.ExtLogger.SetLog('>>>> BAD H TEST Mode', self.prefix) 
               status = SimpleStep.SYSTEM_BAD_H
            ## if disallowed status, raise error for Axis and report in log
            #if (status == SimpleStep.SYSTEM_BAD_H):
            #  self.runBadStatushException();             
            #elif ((not (status in allowableStatus)) and (status != None)):
            #   Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
            #   self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
            #   self.ExtLogger.CheckSystemAvail()
            #   self.ExtLogger.DumpHistory()
            #   raise SimpleStepError, ("Card %s: Bad status = '%s'" % (self.prefix, status))
          else:
            (result, status,) = self.sendAndCheck('RN-10', self.SERIAL_PAUSE);        
            if tesla.config.SS_BAD_H_TEST == 1:
              if( tesla.config.SS_EXT_LOGGER == 1 ): 
                 self.ExtLogger.SetLog('>>>> BAD H TEST Mode', self.prefix) 
              status = SimpleStep.SYSTEM_BAD_H
            # if disallowed status, raise error for Axis and report in log
            if (status == SimpleStep.SYSTEM_BAD_H):
              self.runBadStatushException();             
            elif ((not (status in allowableStatus)) and (status != None)):
              Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
              if( tesla.config.SS_EXT_LOGGER == 1 ): 
                 self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
                 self.ExtLogger.CheckSystemAvail()
                 self.ExtLogger.DumpHistory()
              raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))
          (result, status,) = self.sendAndCheck('A0', self.SERIAL_PAUSE);        
          # if disallowed status, raise error for Axis and report in log
          if ((not (status in allowableStatus)) and (status != None)):
              Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
              if( tesla.config.SS_EXT_LOGGER == 1 ): 
                 self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
                 self.ExtLogger.CheckSystemAvail()
                 self.ExtLogger.DumpHistory()
              raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))
          (result, status,) = self.sendAndCheck(self.m_PumpStandardStepSpeed[0], self.SERIAL_PAUSE);
          # if disallowed status, raise error for Axis and report in log
          if ((not (status in allowableStatus)) and (status != None)):
              Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
              if( tesla.config.SS_EXT_LOGGER == 1 ): 
                 self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
                 self.ExtLogger.CheckSystemAvail()
                 self.ExtLogger.DumpHistory()
              raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))
          (result, status,) = self.sendAndCheck(self.m_PumpStandardStepSpeed[1], self.SERIAL_PAUSE);
          # if disallowed status, raise error for Axis and report in log
          if ((not (status in allowableStatus)) and (status != None)):
              Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
              if( tesla.config.SS_EXT_LOGGER == 1 ): 
                 self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
                 self.ExtLogger.CheckSystemAvail()
                 self.ExtLogger.DumpHistory()
              raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))
          (result, status,) = self.sendAndCheck(self.m_PumpStandardStepSpeed[2], self.SERIAL_PAUSE);
          # if disallowed status, raise error for Axis and report in log
          if ((not (status in allowableStatus)) and (status != None)):
              Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
              if( tesla.config.SS_EXT_LOGGER == 1 ): 
                 self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
                 self.ExtLogger.CheckSystemAvail()
                 self.ExtLogger.DumpHistory()
              raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))
          (result, status,) = self.sendAndCheck(self.m_PumpStandardPower, self.SERIAL_PAUSE);
          # if disallowed status, raise error for Axis and report in log
          if ((not (status in allowableStatus)) and (status != None)):
              Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))                
              if( tesla.config.SS_EXT_LOGGER == 1 ): 
                 self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
                 self.ExtLogger.CheckSystemAvail()
                 self.ExtLogger.DumpHistory()
              raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))
          if( tesla.config.SS_EXT_LOGGER == 1 ): 
             self.ExtLogger.SetCmdListLog('<------Finishing PUMPHOME',self.prefix);

    def turnOutputOn(self, port = 0):       #Check the Other modules DrdPump.py TipStripper.py
        """Turn a specific digital output port ON"""
        self.setOutputState(port, 1)


    def turnOutputOff(self, port = 0):      #Check the Other modules DrdPump.py TipStripper.py
        """Turn a specific digital output port OFF"""
        self.setOutputState(port, 0)


     
    def setVelocityProfile(self, B, E, S):
        """Set the motor velocity profile from the B & E velocities and the
        S slope values
        """
        #print "E%d" % E # added by shabnam
        self.processCmds([('B%d' % B), ('E%d' % E), ('S%d' % S)])


    # 2011-10-17 sp -- added variables to store homing commands that included the homing direction
    #                   and active state of home sensor
    def setMotorCmd( self, initMotorCmd, homeMotorCmd ):
        funcReference = __name__ + '.setMotorCmd'
        self.m_InitMotorCmd = initMotorCmd
        self.m_HomeMotorCmd = homeMotorCmd
        self.svrLog.logDebug('', self.logPrefix, funcReference, 'set init=%s |home=%s' %( self.m_InitMotorCmd, self.m_HomeMotorCmd ))


    # 2011-10-20 sp -- added variables to store timeout to stop continuous motion
    #                   and active state of home sensor
    def setHomeTimeOut( self, timeOut_milliseconds ):
        funcReference = __name__ + '.setHomeTimeOut'
        self.m_TimeOut_MilliSeconds = timeOut_milliseconds
        self.svrLog.logDebug('', self.logPrefix, funcReference, 'set timeOut=%d' % self.m_TimeOut_MilliSeconds )

    # 2011-10-17 sp -- added variables to store homing commands that included the homing direction
    #                   and active state of home sensor
    def getStepsToHome( self ):
        return( self.m_stepsToHome )


    # 2011-11-01 sp -- added variables to store homing commands that included the homing direction
    #                   and active state of home sensor
    def getHomingError(self):
        return( self.m_HomingError )



    def home(self):
        """Abstract (?) method for homing the axis"""
        raise NotImplementedError('Abstract method')


    def speed(self, speed_stepsPerSec, slope):
        """Abstract (?) method for setting the motor's speed"""
        raise NotImplementedError('Abstract method')


    def pollUntilReadyOrBusy(self, delay = POLL_PAUSE):
        """Polls the card for a response until it replies or the retry count
        is exceeded.
        Return the card status, or None if the card isn't responding.
        A good or busy card status is ">" or "b" (= SYSTEM_READY or SYSTEM_BUSY).
        Refer to the  SimpleStep manual for the full set of status flags that can be
        returned."""
        for _ in range(0, SimpleStep.RETRIES):                  #RETRIES = 200
            status = self.SendLowCmd(self.prefix, delay, 1)
            if (status != None):
                status = status.strip('\r')
                # get payload, status - if good reply exit retry loop
                if (status in (self.goodResponse, self.busyResponse, self.notHomedResponse)):
                    break
            wait_msecs(SimpleStep.RETRY_PAUSE)                  #RETRY_PAUSE = 20
        else:
            status = None

        if self.DEBUG:
            print(('pollUntilReadyOrBusy(): status = [%s]' % status))
        return status


    def pollUntilReady(self, delay = POLL_PAUSE):             #Check Axis.py
        """Polls the card for a response until it replies or the retry count
        is exceeded.
        Return the card status, or None if the card isn't responding.
        A good card status is ">" (= SYSTEM_READY). Refer to the
        SimpleStep manual for the full set of status flags that can be
        returned."""

        for i in range(0, SimpleStep.RETRIES):                  #RETRIES = 200
              if self.ver >= 109 :
                 whole = self.SendLowCmd(self.prefix+'?3', delay, 1)
                 if (whole != None):
                    whole = whole.strip('\r')
                    error  = whole[3:]
                    status = whole[:3]
                    # get payload, status - if good reply exit retry loop
                    if (status in (self.goodResponse, self.notHomedResponse)):
                        break
                    if status in (self.busyResponse, self.abortResponse):
                       if self.DEBUG:
                          pass
              else:
                 status = self.SendLowCmd(self.prefix, delay, 1)
                 if (status != None):
                    status = status.strip('\r')
                    # get payload, status - if good reply exit retry loop
                    if (status in (self.goodResponse, self.notHomedResponse)):
                        break
                    if status in (self.busyResponse, self.abortResponse):
                       if self.DEBUG:
                          pass

              wait_msecs(SimpleStep.RETRY_PAUSE)                  #RETRY_PAUSE = 20
        else:
              status = None
              if self.ver >= 109:
                 whole  = self.SendLowCmd(self.prefix+'?3', delay, 1)
                 whole  = whole.strip('\r')
                 error  = whole[3:]

        if self.DEBUG:
           print(('pollUntilReady(): status = [%s], TRY = %d' % (status,i+1)))

        funcReference = __name__ + '.pollUntilReady'  # 2011-11-29 -- sp
        if self.ver >= 109:
            if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
               self.ExtLogger.SetLog( ('<-- pollUntilReady(): status = [%s], error = [%s], TRY = [%d]' % (status,error,i+1)), self.prefix )
            self.svrLog.logVerbose('X', self.logPrefix, funcReference, 'status = [%s], error = [%s], TRY = [%d]' % (status,error,i+1) )  # 2011-11-29 -- sp
        else:
            if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
               self.ExtLogger.SetLog( ('<-- pollUntilReady(): status = [%s], ,TRY = [%d]' % (status,i+1)), self.prefix )
            self.svrLog.logVerbose('X', self.logPrefix, funcReference, 'status = [%s], ,TRY = [%d]' % (status,i+1) )  # 2011-11-29 -- sp

        return status


    # 2011-10-19 sp -- added timeOut to wait for previous command to complete
    def pollUntilTimeOut( self, timeOut_MilliSeconds ):
        funcReference = __name__ + '.pollUntilTimeOut'
        
        startTime = time.time()
        timeOutTime = startTime + timeOut_MilliSeconds / 1000
        while True:
            status = self.pollUntilReadyOrBusy()
            if (status != None):
                status = status.strip('\r')
            # get payload, status - if good reply exit retry loop
            if (status in (self.goodResponse)):
                completionTime = time.time() - startTime
                self.svrLog.logVerbose('S', self.logPrefix, funcReference, 'taken(s)=%f' % completionTime )
                break

            if( time.time() > timeOutTime ):
                status = (self.prefixResponse + SimpleStep.SYSTEM_ABORT + 'TimedOut')
                self.svrLog.logWarning('', self.logPrefix, funcReference, 'Timed out(ms)=%d' % timeOut_MilliSeconds )
                break
            wait_msecs(SimpleStep.RETRY_PAUSE)                  #RETRY_PAUSE = 20

        return status


    def sendAndCheck(self, data, delay = SERIAL_PAUSE):       #Check Axis.py, LidSensor.py, PagerSystem.py, TipStripper.py
        """Wait until the SimpleStep card is ready, then send data to the card
        and return a tuple of payload data and the status."""

        if self.INFO:
            print(('SS/SAC (%s): [%s] ->' % (self.prefix, data)), end=' ')

        funcReference = __name__ + '.sendAndCheck'  # 2011-11-29 -- sp
        self.svrLog.logVerbose('X', self.logPrefix, funcReference, 'SS/SAC (%s): [%s] ->' % (self.prefix, data) )  # 2011-11-29 -- sp
        if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
            self.ExtLogger.SetLog(('SS/SAC (%s): [%s] ->' % (self.prefix, data)), self.prefix)

        self.doublePollUntilReady()

        payload = self.SendLowCmd(('%s%s' % (self.prefix, data)), delay, 0)
        log_payload = payload
        
        if payload != None:
           log_payload = payload.strip('\r')
        else:
           print('SendLowCmd returns %s'%payload)

        if self.DEBUG:
            print(('sendAndCheck payload 1 = [%s]' % log_payload))

        if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
            self.ExtLogger.SetLog(('<-- sendAndCheck [%s%s] payload 1 = [%s]' % (self.prefix, data, log_payload)), self.prefix )
        self.svrLog.logVerbose('X', self.logPrefix, funcReference, '<-- data=[%s] |payload=[%s]' % (data, log_payload) )  # 2011-11-29 -- sp

        # if no reply or motor cmd replies 'ready' or 'not homed' get new reply
        if ((payload == None) or (self.isMotorCommand(data) and (payload[2] in (SimpleStep.SYSTEM_READY, SimpleStep.SYSTEM_NOT_HOMED)))):
            if self.DEBUG:
                print('sendAndCheck poll 1')

            if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                self.ExtLogger.SetLog( ('<-- sendAndCheck poll 1, payload = [%s]'%log_payload) , self.prefix)
            self.svrLog.logVerbose('X', self.logPrefix, funcReference, '<-- poll 1, payload=[%s]' % (log_payload) )  # 2011-11-29 -- sp

            payload = self.pollUntilReady()

        returnValue = (None, None)

        # analyze the reply received or exit if no reply
        if (payload != None):
            (sanityCheck, status,) = self.unpackPayload(payload)
            if (sanityCheck != self.prefixResponse):

                if self.DEBUG:
                    print('sendAndCheck, sanityCheck failed')

                self.svrLog.logError('X', self.logPrefix, funcReference, 'Sanity check (%s) failed' % (sanityCheck) )  # 2011-11-29 -- sp
                if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                    self.ExtLogger.SetLog( ('Card %s: Sanity check (%s) failed' % (self.prefix,sanityCheck) ), self.prefix)
                    self.ExtLogger.CheckSystemAvail()
                    self.ExtLogger.DumpHistory()
                raise SimpleStepError('Card %s: Sanity check (%s) failed' % (self.prefix,sanityCheck))

            # on busy or abort reply try again...
            if ((status == 'b') or (status == 'a')):
                if self.DEBUG:
                    print('sendAndCheck, busy or abort')
#                self.ExtLogger.SetLog('<-- sendAndCheck, busy or abort', self.prefix)
                if (status == 'a'):
                    self.svrLog.logError('X', self.logPrefix, funcReference, '<-- Motor Abort occurs!' )  # 2011-11-29 -- sp
                    if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                       self.ExtLogger.SetLog('<-- Motor Abort occurs!', self.prefix)
                       self.ExtLogger.CheckSystemAvail()
                       self.ExtLogger.DumpHistory()
                    self.sendAndCheck(data)

                payload = self.pollUntilReady()
                (sanityCheck, status,) = self.unpackPayload(payload)
            try:
                returnValue = (payload[3:].strip('\r'), status)
            except:
                returnValue = (payload, status)

        if self.DEBUG:
            print(('sendAndCheck returnValue = [%s][%s]' % returnValue))
#        self.ExtLogger.SetLog('<-- sendAndCheck returnValue = [%s][%s]' % returnValue, self.prefix)
        if self.INFO:
            print(('[%s]' % str(returnValue)))

#       2013-02-06 -- sp  comment lines such that program is not paused when other threads are invoked
#                       or timers are used to catch errors when in debug mode
#        if( tesla.config.SS_DEBUGGER_LOG == 1 ):
#            if( not self.ssLogDebugger.IsDebugModeInUse() ):
#                if( self.ssLogDebugger.IsDebuggerPaused() ):
#                    ssLogDebugger = tesla.DebuggerWindow.GetSSTracerInstance()
#                    ssLogDebugger.logDebuggerMode()

        return returnValue
        

    def sendAndCheckNonBlocking(self, data, delay = SERIAL_PAUSE):
        """Wait until the SimpleStep card is ready, then send data to the card
        and return a tuple of payload data and the status."""

        funcReference = __name__ + '.sendAndCheckNonBlocking'  # 2011-11-29 -- sp
        if self.INFO:
            print(('> SS/SAC (%s): [%s] ->' % (self.prefix,data)), end=' ')
        self.doublePollUntilReady()
        payload = self.SendLowCmd(('%s%s' % (self.prefix, data)), delay, 0)
        if self.DEBUG:
            print(('sendAndCheckNonBlocking payload 1 = [%s]' % payload))
        if ((payload == None) or (self.isMotorCommand(data) and (payload[2] in (SimpleStep.SYSTEM_READY, SimpleStep.SYSTEM_NOT_HOMED)))):
            if self.DEBUG:
                print('sendAndCheckNonBlocking poll 1')
            payload = self.pollUntilReadyOrBusy()

        returnValue = (None,None)

        if (payload != None):
            (sanityCheck, status,) = self.unpackPayload(payload)
            if (sanityCheck != self.prefixResponse):
                if self.DEBUG:
                    print('sendAndCheckNonBlocking, sanityCheck failed')

                self.svrLog.logError('X', self.logPrefix, funcReference, 'Sanity check (%s) failed' % (sanityCheck) )  # 2011-11-29 -- sp
                if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                    self.ExtLogger.SetLog('Card %s: Sanity check (%s) failed' % (self.prefix, sanityCheck))
                    self.ExtLogger.CheckSystemAvail()
                    self.ExtLogger.DumpHistory()
                raise SimpleStepError('Card %s: Sanity check (%s) failed' % (self.prefix, sanityCheck))
            if ((status == 'b') or (status == 'a')):
                if self.DEBUG:
                    print('sendAndCheckNonBlocking, busy or abort')
                (sanityCheck, status,) = self.unpackPayload(payload)
            try:
                returnValue = (payload[3:].strip('\r'), status)
            except:
                returnValue = (payload, status)
        if self.DEBUG:
            print(('sendAndCheck returnValue = [%s][%s]' % returnValue))
        if self.INFO:
            print(('[%s]' % str(returnValue)))
        return returnValue


    def process(self, command, bNonBlocking):
        """Process a command, some are internal, others go to the card"""
        funcReference = __name__ + '.process'   # 2011-11-29
        self.svrLog.logDebug('X', self.logPrefix, funcReference, 'cmd=%s' % command )   # 2011-11-29

        cmd = command[0]
        allowableStatus = []
        payload = command[1:]
        if (cmd == 'W'):
            pause = float(payload)
            wait_msecs(pause)
        elif (command =='CHKPOS'):                              #CWJ Add
            self.CheckPosition()                                #CWJ Add
        elif (command =='HOME'):                                #CWJ Add
            self.Init(False)                                    #CWJ Add
        elif (command =='PUMPHOME'):                            #CWJ Add
            print('\n>>>> PumpHOME \n\n')
            # self.Init(True)                                     #CWJ Add
            self.setPumpHome();
        elif (command =='PUMPHOMESKIPTRIP'):                            #CWJ Add
            print('\n>>>> Use Non-fatal pump homing \n\n')
            # self.Init(True)                                     #CWJ Add
            self.setPumpHome( True );            
        else:
            # 2011-10-24 sp -- block commands if homing error
            # 2012-01-30 sp -- replace environment variable with configuration variable
            #if( os.environ.has_key('SS_MINNIE_DEV') and self.m_HomingError and ( cmd == 'N' or cmd == 'R' or cmd == 'M') ):
            if( tesla.config.SS_MINNIE_DEV == 1 and self.m_HomingError and ( cmd == 'N' or cmd == 'R' or cmd == 'M') ):
#                Device.logger.logInfo("Stepper %s %s error: [cmd = %s][Move while homing error] "% (self.board, self.address, cmd))
#                self.ExtLogger.SetLog( ("Card %s: Move while homing error" % (self.prefix)), self.prefix )
#                self.ExtLogger.CheckSystemAvail()
#                self.ExtLogger.DumpHistory()
#                raise SimpleStepError, ("Card %s: Move while homing error" % (self.prefix))
                self.svrLog.logError('', self.logPrefix, funcReference, 'Homing error, blocking cmd=%s' % command )
                print(( "Card %s: homing error, no movements made" % (self.prefix) ))
                pass
            else:
                if bNonBlocking:
                    (result, status,) = self.sendAndCheckNonBlocking(command, self.SERIAL_PAUSE)
                    allowableStatus = [SimpleStep.SYSTEM_READY, SimpleStep.SYSTEM_BUSY, SimpleStep.SYSTEM_NOT_HOMED]
                else:
                    (result, status,) = self.sendAndCheck(command, self.SERIAL_PAUSE)
                    allowableStatus = [SimpleStep.SYSTEM_READY, SimpleStep.SYSTEM_NOT_HOMED]

                if self.DEBUG:
                    print(('process()[%s] [cmd = %s][payload = %s][result = %s][status = %s]\n' % (self.prefix, cmd, payload, result, status)))

                self.svrLog.logVerbose('X', self.logPrefix, funcReference, 'cmd=%s|payload=%s|result=%s|status=%s' % (cmd, payload, result, status) )
                
                if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                    self.ExtLogger.SetLog( ('process()[%s] [cmd = %s][payload = %s][result = %s][status = %s]' % (self.prefix, cmd, payload, result, status)), self.prefix )

                # if disallowed status, raise error for Axis and report in log
                if ((not (status in allowableStatus)) and (status != None)):
                    Device.logger.logInfo("Stepper %s %s error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (self.board, self.address, cmd, payload, result, status))
                    self.svrLog.logError('X', self.logPrefix, funcReference, "Stepper error: [cmd = %s][payload = %s] -> [result = %s][status = %s] "% (cmd, payload, result, status) )
                    if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                        self.ExtLogger.SetLog( ("Card %s: Bad status = '%s'" % (self.prefix, status)), self.prefix )
                        self.ExtLogger.CheckSystemAvail()
                        self.ExtLogger.DumpHistory()
                    raise SimpleStepError("Card %s: Bad status = '%s'" % (self.prefix, status))


    def processCmds(self, cmdList, bNonBlocking = False):
        """Process a list of commands"""

        funcReference = __name__ + '.processCmds'   # 2011-11-29
        self.svrLog.logDebug('', self.logPrefix, funcReference, '<START>  cmdList: %s' % ( str(cmdList) ) )   # 2011-11-29
#        self.ExtLogger.SetCmdListLog( '', self.prefix);
        if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
            self.ExtLogger.SetCmdListLog( ('<START>  cmdList[%s]: %s' % ( self.prefix, str(cmdList) )), self.prefix );

        i = 0
        for cmd in cmdList:
            self.process(cmd, bNonBlocking)
            i = i+1

        if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
            self.ExtLogger.SetLog(('<FINISH> cmdList[%s]: %s' % ( self.prefix, str(cmdList) )), self.prefix );
        self.svrLog.logVerbose('X', self.logPrefix, funcReference, '<FINISH>  cmdList: %s' % ( str(cmdList) ) )   # 2011-11-29


    def processFile(self, fileName, bNonBlocking = False):
        """Read in a script and process it"""
        try:
            f = open(fileName, 'r')

            while True:
                cmdLine = f.readline()
                if (not cmdLine):
                    break
                elif (cmdLine[0] != '#'):
                    strippedLine = cmdLine[:-1]
                    if strippedLine:
                        self.process(strippedLine, bNonBlocking)

            f.close()
        except Exception as msg:
            raise SimpleStepError('Card %s: processFile() error = %s' % (self.prefix, msg))


    def __parseBoardAddress(self, boardAddress):
        """Internal method to parse a board address (eg. D1) into the board
        type (eg. D) and the address (eg. 1).
        Returns a tuple of (board, address).
        """
        funcReference = __name__ + '.__parseBoardAddress'   # 2011-11-29

        if (len(boardAddress) != 2):
            self.svrLog.logError('X', str(boardAddress), funcReference, 'Invalid board address (%s)' % str(boardAddress) )   # 2011-11-29
            if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                self.ExtLogger.SetLog(('Invalid board address (%s)' % str(boardAddress)), self.prefix)
                self.ExtLogger.CheckSystemAvail()
                self.ExtLogger.DumpHistory()
            raise SimpleStepError('Invalid board address (%s)' % str(boardAddress))

        boardType = boardAddress[0]
        address = int(boardAddress[1])
        return (boardType, address)


    def isMotorCommand(self, data):
        """Returns True if the data passed in has a command that is a motor
        driving command. These commands include M (absolute move), R (relative
        move) and N (home)."""
        return ((data != None) and ((len(data) > 0) and (data[0] in ['M','N','R'])))


    def doublePollUntilReady(self, delay = POLL_PAUSE):
        """Poll the card, with a retry of the whole polling effort if we failed
        the first polling attempt.
        Returns nothing, throws an exception if we didn't succeed."""

        funcReference = __name__ + '.doublePollUntilReady'   # 2011-11-29

        for i in range(6):
            if (self.pollUntilReady() == None):
                if self.INFO:
                    print(('%s card offline? %d' % (self.prefix,i)))
                if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                    self.ExtLogger.SetLog( ('%s card offline? %d' % (self.prefix,i)), self.prefix )
                self.svrLog.logDebug('X', self.logPrefix, funcReference, '%s card offline? %d' % (self.prefix,i) )   # 2011-11-29
            else:
                return
            wait_msecs(SimpleStep.OFFLINE_PAUSE)      # OFFLINE_PAUSE = 1000
        else:                                         # Strange Code ---- CWJ
            if (self.pollUntilReady() == None):
                if self.INFO:
                    print(('%s card failed to respond' % self.prefix))
                self.svrLog.logError('X', self.logPrefix, funcReference, '%s card poll failed: Busy or not responding.' % self.prefix )   # 2011-11-29
                if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                    self.ExtLogger.SetLog(( '%s card poll failed: Busy or not responding.' % self.prefix), self.prefix )
                    self.ExtLogger.CheckSystemAvail()
                    self.ExtLogger.DumpHistory()
                raise SimpleStepError('%s card poll failed: Busy or not responding.' % self.prefix)


    def unpackPayload(self, payload):
        """Unpack the incoming payload, returning a tuple of (sanity check
        and status) where the sanity check should be the returned card
        address (in lower case) and the status should be a single character."""
        funcReference = __name__ + '.unpackPayload'   # 2011-11-29

        try:
            if (payload == None):
                sanityCheck = None
                status = None
            else:
                sanityCheck = payload[:2]
                status = payload[2:3]
        except TypeError as msg:
            if self.DEBUG:
                print(('unpackPayload() error: [%s]' % msg))
                if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                    self.ExtLogger.SetLog( ('unpackPayload() error: [%s]' % msg), self.prefix )
            
            self.svrLog.logError('X', self.logPrefix, funcReference, 'TypeError caught: %s for payload = %s (type = %s)' % (msg,payload,str(type(payload))) )   # 2011-11-29
            if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                self.ExtLogger.SetLog( ('Card %s: TypeError caught: %s for payload = %s (type = %s)' % (self.prefix, msg,payload,str(type(payload)))), self.prefix  )
                self.ExtLogger.CheckSystemAvail()
                self.ExtLogger.DumpHistory()
            raise SimpleStepError('Card %s: TypeError caught: %s for payload = %s (type = %s)' % (self.prefix, msg,payload,str(type(payload))))

        if self.DEBUG:
            print(('unpackPayload: %s %s' % (sanityCheck, status)))

#        self.ExtLogger.SetLog( ('<-- unpackPayload: %s %s' % (sanityCheck, status)), self.prefix )

        return (sanityCheck, status)

    def SendLowCmd( self, data, delay, flag ):

        msg = self.port.sendAndCheck( data, delay )
#        self.dummy = self.dummy + 1
#        if self.dummy == random.randint(10,20):
#           if (data == self.prefix):
#             print '\n### polling detect!!!\n'
#             msg = None #self.prefix+'E'
#             self.dummy = 0
#        if self.dummy >= 70:
#           self.dummy = 0


        funcReference = __name__ + '.SendLowCmd'   # 2011-11-29

        if msg == None:
            if self.ver >= 109 :
                self.svrLog.logError('X', self.logPrefix, funcReference, '<-------- ###### None Error!!! ######   SendLowCmd[%s] returns %s '%(data,msg) )   # 2011-11-29
                if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                    self.ExtLogger.SetLog( ('<-------- ###### None Error!!! ######   SendLowCmd[%s] returns %s '%(data,msg)), self.prefix  )
                    self.ExtLogger.CheckSystemAvail()
                    self.ExtLogger.DumpNoneHistory()
                return None

        (Sanity,Status) = self.unpackPayload(msg)

        if ((Status == 'E') or (Status == 'e') or \
            (Status == 'o') or (Status == 'h') or (Status == 'H') or \
            (Status == '%')):
            self.svrLog.logError('X', self.logPrefix, funcReference, '<-------- ###### Polling Error!!! ######   SendLowCmd[%s] returns %s '%(data,msg) )   # 2011-11-29
            if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                self.ExtLogger.SetLog( ('<-------- ###### Polling Error!!! ######   SendLowCmd[%s] returns %s '%(data,msg)), self.prefix  )
                self.ExtLogger.CheckSystemAvail()
                self.ExtLogger.DumpPollHistory()

        return msg

    def FindMissingSteps(self):
        # 2012-01-30 sp -- replace environment variable with configuration variable
        #if os.environ.has_key('SS_FORCE_EMULATION'):
        if tesla.config.SS_FORCE_EMULATION == 1:
           return 0
       
        # 2011-11-24 sp -- added
        funcReference = __name__ + '.FindMissingSteps'
        self.svrLog.logDebug('X', self.logPrefix, funcReference, 'start FindMissingSteps')

        if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
            self.ExtLogger.SetLog( ('---->> Start FindMissingSteps (%s)'%self.prefix), self.prefix  )
        rtnMsg = None
        # 2011-10-17 sp -- use previously tested code when environment variable is defined
        # 2012-01-30 sp -- replace environment variable with configuration variable
        #if not os.environ.has_key('SS_MINNIE_DEV'):
        if tesla.config.SS_MINNIE_DEV == 0:
            self.process('H3', False)
            self.process('P200,200,0', False)
            self.process('B750', False)
            self.process('E750', False)
            self.process('S1', False)
            self.process('N-0H', False)
            self.port.sendAndCheck(self.prefix+'TY1,1', self.POLL_PAUSE )
            self.port.sendAndCheck(self.prefix, self.POLL_PAUSE )
            if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                self.ExtLogger.SetLog( ('sendAndCheck()[%sTY1,1] from FindMissingSteps'%self.prefix), self.prefix  )
            self.svrLog.logDebug('X', self.logPrefix, funcReference, 'sendAndCheck()[%sTY1,1] from FindMissingSteps'%self.prefix)
            self.process('C-', False)
            rtnMsg = self.sendAndCheck('m')
            self.process('!',False)
            self.process('TN', False)
            self.process('N-0', False)
            if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                self.ExtLogger.SetLog( ('---->> Finish FindMissingSteps (%s)'%self.prefix), self.prefix  )
            self.svrLog.logVerbose('X', self.logPrefix, funcReference, '---->> Finish FindMissingSteps (%s)'%self.prefix)

            if ( self.ver < 109 ):
                return ( int(rtnMsg[0]) - 65536 )
            else :
                return int(rtnMsg[0])
        # 2011-10-17 sp -- added section; call homing command to get steps to home
        else:
            stepsToHome = self.Init( False )
            if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                self.ExtLogger.SetLog( ('---->> Finish FindMissingSteps (%s)'%self.prefix), self.prefix  )
            self.svrLog.logDebug('X', self.logPrefix, funcReference, 'Steps home=%d' % stepsToHome )
            return( stepsToHome )

    def FindMissingStepsWithHoldingPowerSetting(self):
        OriginPwr = [None, None, None, None, None, None, None]

        # 2012-01-30 sp -- replace environment variable with configuration variable
        #if os.environ.has_key('SS_FORCE_EMULATION'):
        if tesla.config.SS_FORCE_EMULATION == 1:
           return [0,0]

        # 2011-11-24 sp -- added
        funcReference = __name__ + '.FindMissingStepsWithHoldingPowerSetting'
        self.svrLog.logDebug('X', self.logPrefix, funcReference, 'start FindMissingStepsWithHoldingPowerSetting')
            
        if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
            self.ExtLogger.SetLog( ('---->> Start FindMissingStepsWithHoldingPowerSetting (%s)'%self.prefix), self.prefix  )
        rtnMsg  = None
        pwrMsg  = None
        diff    = 0

        pwrMsg = self.sendAndCheck('p0')
        OriginPwr[0] = int(pwrMsg[0])
        pwrMsg = self.sendAndCheck('p1')
        OriginPwr[1] = int(pwrMsg[0])
        pwrMsg = self.sendAndCheck('p2')
        OriginPwr[2] = int(pwrMsg[0])
        pwrMsg = self.sendAndCheck('p3')
        OriginPwr[3] = int(pwrMsg[0])
        pwrMsg = self.sendAndCheck('b')
        OriginPwr[4] = int(pwrMsg[0])
        pwrMsg = self.sendAndCheck('e')
        OriginPwr[5] = int(pwrMsg[0])
        pwrMsg = self.sendAndCheck('s')
        OriginPwr[6] = int(pwrMsg[0])

        cmds = ['H3'] + ['P200,200,0'] + ['B750'] + ['E750'] + ['S1'] + ['RN+500'] + ['W100']
        self.processCmds(cmds, False)

        # 2011-10-17 sp -- use previously tested code when environment variable is defined
        # 2012-01-30 sp -- replace environment variable with configuration variable
        #if not os.environ.has_key('SS_MINNIE_DEV'):
        if tesla.config.SS_MINNIE_DEV == 0:
            self.process('N-0H', False)
            self.port.sendAndCheck(self.prefix+'TY1,1', self.POLL_PAUSE )
            self.port.sendAndCheck(self.prefix, self.POLL_PAUSE )
            if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                self.ExtLogger.SetLog( ('sendAndCheck()[%sTY1,1] from FindMissingStepsWithHoldingPowerSetting'%self.prefix), self.prefix  )
            self.svrLog.logDebug('X', self.logPrefix, funcReference, 'sendAndCheck()[%sTY1,1] ' % ( self.prefix ) )
            self.process('C-', False)
            rtnMsg = self.sendAndCheck('m')
            self.process('!',False)
            self.process('TN', False)
            self.process('N-0', False)
            wait_msecs(100)

            if ( self.ver < 109 ):
                diff = ( int(rtnMsg[0]) - 65536 )
            else :
                diff = int(rtnMsg[0])
        # 2011-10-17 sp -- added section; call homing command to get steps to home
        else:
            diff = self.Init( False )

        mov = 500 + diff
        rtn = 0
        if mov > 0:
           mov = abs(mov)
           rtn = -mov
           self.process('RN-%d'%mov, False)

        if mov == 0:
           pass

        if mov < 0:
           mov = abs(mov)
           rtn = mov
           self.process('RN+%d'%mov, False)
           wait_msecs(100)

        pwr = ('P%d,%d,%d'%(OriginPwr[0],OriginPwr[1],OriginPwr[2]))
        step = ('H%d'%OriginPwr[3])
        bVel = ('B%d'%OriginPwr[4])
        eVel = ('E%d'%OriginPwr[5])
        #print "velocity+++++++++ = %s" % eVel # added by shabnam
        slop = ('S%d'%OriginPwr[6])

        cmds = [pwr] + [step] + [bVel] + [eVel] + [slop]
        self.processCmds(cmds, False)

        if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
            self.ExtLogger.SetLog( ('---->> Finish FindMissingStepsWithHoldingPowerSetting (%s)'%self.prefix), self.prefix  )
        
        # 2011-11-24 sp -- added
        self.svrLog.logDebug('X', self.logPrefix, funcReference, 'completed: rtn=%d |diff=%d' % ( rtn, diff ) )
        return [rtn,diff]

    #To Check Current Position register status
    def CheckPosition(self):
        rtnMsg = self.sendAndCheck('m')
        pos = int(rtnMsg[0])
        if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
            self.ExtLogger.SetCmdListLog( ('---->> Position After Homing (%sm): [%d]'%( self.prefix,pos)), self.prefix)
        funcReference = __name__ + '.CheckPosition'
        self.svrLog.logDebug('X', self.logPrefix, funcReference, '---->> Position After Homing (%sm): [%d]'%( self.prefix,pos))

    #FW109.51 has the init routine problem. This will be an alternative way to replace it.
    def Init(self, Pump):
        # 2011-10-17 sp -- use reference for program log
        funcReference = __name__ + '.Init'
        self.svrLog.logVerbose('X', self.logPrefix, funcReference, 'start Init, pump=%s' % Pump)
        
        # 2011-10-21 sp -- variable to track homing status
        # set to unlikely number of number of steps to home, as this is not used in previous versions
        # number will be set to actual number when newer code is executed
        stepsToHome = 999999999       
        if( self.m_HomingError ):
            return( stepsToHome )

        if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
            self.ExtLogger.SetLog( ('---->> Enter Init(%s)! '%( self.prefix)), self.prefix)

        # 2011-10-17 sp -- use previously tested code when environment variable is defined
        # 2012-01-30 sp -- replace environment variable with configuration variable
        #if not os.environ.has_key('SS_MINNIE_DEV'):
        if tesla.config.SS_MINNIE_DEV == 0:
            if (Pump == True):
               print('\n#### Pump Home!!! ####\n')
               self.process('N+1', False)
            else:
               print('\n#### Normal Home!!! ####\n')
               self.process('N-1', False)
               
            rtnMsg  = self.sendAndCheck('m')
            pos     = int(rtnMsg[0])
            rtnMsg  = self.sendAndCheck('I')
            flag    = int(rtnMsg[0])

            if (pos != 0): #if motor doesn't think it's at 0
                self.svrLog.logError('X', self.logPrefix, funcReference, '---->> Axis(%s) may occur the potential Init Error!'%self.prefix)
                if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                   self.ExtLogger.SetLog('---->> Axis(%s) may occur the potential Init Error!'%self.prefix, self.prefix)
                   self.ExtLogger.CheckSystemAvail()
                   self.ExtLogger.DumpPollHistory()
                if int( flag ) % 2: #Tripped = odd (1005 on Carousel flag), not tripped = even (1004 on carousel)
                    chk = True #not tripped
                else:
                    chk = False
                if (chk == True): #home not tripped
                    self.process('!', False)
                    if (Pump == True):
                        print('\n#### Pump Reset Pos! ####\n')
                        self.process('N+0', False)
                    else:
                        print('\n#### Normal Reset Pos! ####\n')
                    self.process('N-0', False)
                else: #chk = False , home is tripped
                    self.process('!', False) #home tripped
                    self.Init(Pump)
        # 2011-10-17 sp -- added section; use configuration file settings for homing in newer sensor design
        else:
            rtnMsg  = self.sendAndCheck('I1')
            flagNotHome  = int(rtnMsg[0])

            if( flagNotHome == int( self.m_HomeMotorCmd[3] ) ):
                self.svrLog.logVerbose('', self.logPrefix, funcReference, 'Device is already at home' )
                stepsToHome = 0
            else:
                positionBeforeHome = self.getPosition()
                # move home keeping track of the number of steps taken (i.e. not reseting position registers)
                self.svrLog.logVerbose('', self.logPrefix, funcReference, 'Home cmd=%s' %( self.m_HomeMotorCmd+'z' ))
                self.process(self.m_HomeMotorCmd+'z', False)
                # self.process(self.m_HomeMotorCmd+'z', True)
                
                if( tesla.config.SS_TIMEOUT == 1 ):
                  # own handler in waiting for home using a timed limit (value longer than what it would take)
                  if( self.pollUntilTimeOut( self.m_TimeOut_MilliSeconds ) != self.goodResponse ):
                      # send motion abort command, avoid repeated abort commands with default sendAndCheck
                      # self.sendAndCheck('*')
                      self.svrLog.logWarning('', self.logPrefix, funcReference, 'Home sensor not detected within timeout')
                      payload = self.SendLowCmd(('%s*' % self.prefix), self.SERIAL_PAUSE, 0)  # abort to stop motor
  #                    payload = self.SendLowCmd(('%s!' % self.prefix), self.SERIAL_PAUSE, 0)  # alternate method to abort motion
                      self.svrLog.logError('', self.logPrefix, funcReference, 'Motor abort command sent: confirmation=%s' % payload )
                      payload = self.pollUntilReady()
                      self.m_HomingError = True
  #                    raise SimpleStepError, "Home command timed-out"
                  # currently disabled, turned on by setting the if statement to True
                  if( False ):    # alternatively double check change of state of home sensor 
                      rtnMsg  = self.sendAndCheck('I1')
                      flagHome    = int(rtnMsg[0])
                      if( flagNotHome == flagHome ):
                          self.svrLog.logError('', self.logPrefix, funcReference, 'Home sensor not detected within timeout')
                      self.m_HomingError = True
  #                    raise SimpleStepError, "Home timed-out"

                # retrieve the number of steps taken to reach home
                rtnMsg  = self.sendAndCheck('m')
                if( self.ver >= 109 ):
                    stepsToHome = 2147483647 - int(rtnMsg[0])
                else:
                    stepsToHome = 65534 - int(rtnMsg[0])
                self.svrLog.logVerbose('S', self.logPrefix, funcReference, 'Steps from home=%d |moved=%d' %( positionBeforeHome, stepsToHome ))

            pos = stepsToHome
            rtnMsg  = self.sendAndCheck('I')        # get the status of the flags
            flag    = int(rtnMsg[0])
                    
            if (self.m_HomingError): #if motor doesn't think it's at 0 (i.e homing error)
                self.svrLog.logError('X', self.logPrefix, funcReference, '---->> Axis(%s) may occur the potential Init Error!'%self.prefix)
                if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                    self.ExtLogger.SetLog('---->> Axis(%s) may occur the potential Init Error!'%self.prefix, self.prefix)
                    self.ExtLogger.CheckSystemAvail()
                    self.ExtLogger.DumpPollHistory()

        if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
            self.ExtLogger.SetLog('---->> Leave Init(%s)! pos:%d, flag:%d'%( self.prefix, pos, flag), self.prefix)
        self.svrLog.logVerbose('X', self.logPrefix, funcReference, '---->> Leave Init(%s)! pos:%d, flag:%d'%( self.prefix, pos, flag))
        
        # 2011-10-17 sp -- added return of number of steps to home; return value ignored in previous versions
        self.m_stepsToHome = stepsToHome
        self.svrLog.logDebug('', self.logPrefix, funcReference, 'Steps home=%d' %( self.m_stepsToHome ))
        return( stepsToHome )
        
if (__name__ == '__main__'):
    import sys
    drd = SimpleStep('Y1', port='COM1')
    drd.process('F', False)
    drd.process('P1', False)
    drd.process('N+1', False)
    print('Going home')
    drd.process('M0', False)
    print('Going to end')
    drd.process('M4700', False)
    drd.process('P0', False)
