# -----------------------------------------------------------------------------
#
#   PgmLog.py
#   tesla.PgmLog
#
# New program logging module (RoboSep V4.7+) for the Tesla instrument controller
# Implemented using Python Logging package with loglevels defined as:
#   Level         Numeric value
#   CRITICAL         50
#   ERROR       40
#   WARNING         30
#   INFO             20
#   DEBUG       10
#   NOTSET      0
#
# Copyright (c) StemCell Technologies Inc., 2011
# 570 West 7th Avenue, Suite 400
# Vancouver BC V5Z 1B3
# www.stemcell.com
#
# The copyright to the computer program(s) herein is the
# property of StemCell Technologies Inc.
# The program(s) may be used and/or copied only with
# the written permission of StemCell Technologies Inc.
# or in accordance with the terms and conditions
# stipulated in the agreement/contract under which
# the program(s) have been supplied.
#
# Revisions:
#   2011-11-22  SP  initial code
#   2016-02-16  db  added functionality to toggle auto dcv mode
#
# -----------------------------------------------------------------------------
#!/usr/bin/env python

import os
import platform
import time
import datetime
import tesla.config
from tesla.exception import TeslaException
from ConfigParser import *

import tesla.PgmLog
import logging
import os.path
import wx
import threading

ssTracer = None

class DebuggerWindow(wx.Frame):
    SYSTEM_READY      = '>'
    SYSTEM_BUSY       = 'b'
    SYSTEM_ABORT      = 'a'
    SYSTEM_NOT_HOMED  = 's'
    RETRIES           = 200
    SERIAL_PAUSE      = 100
    POLL_PAUSE        = 100

    DEFAULT_TEXT_COLOR = ( 0, 0, 0 )
    EXECUTE_TEXT_COLOR = ( 255, 0, 224 )
    RETURN_TEXT_COLOR = ( 0, 128, 255 )
    BREAKPOINT_TEXT_COLOR = ( 255, 0, 0 )
    BREAK_TEXT_COLOR = ( 128, 0, 255 )
    
    INVALID_POSITION = -999999999

    ZAxis = 'X0'
    ZAxisPower = 'P140,50,0'
    ZAxisSpeedBegin = 'B3000'
    ZAxisSpeedSlope = 'S5'
    ZAxisSpeedEnd = 'E15000'
    ZAxisSpeedHomeBegin = 'B1000'
    ZAxisSpeedHomeSlope = 'S5'
    ZAxisSpeedHomeEnd = 'E5000'
    ZAxisHalfStep = 'H3'
    ZAxisBackOffHome = 600
    ZAxisHome = 'N-110s'
    
    PumpAxis = 'X1'
    PumpPower = 'P150,50,0'
    PumpSpeedBegin = 'B421'
    PumpSpeedSlope = 'S10'
    PumpSpeedEnd = 'E1883'
    PumpSpeedHomeBegin = 'B421'
    PumpSpeedHomeSlope = 'S10'
    PumpSpeedHomeEnd = 'E1883'
    PumpHalfStep = 'H0'
    PumpBackOffHome = -1200
    PumpHome = 'N+110s'
    
    ThetaArm = 'Y0'
    ThetaArmPower = 'P140,100,0'
    ThetaArmSpeedBegin = 'B300'
    ThetaArmSpeedSlope = 'S7'
    ThetaArmSpeedEnd = 'E3750'
    ThetaArmSpeedHomeBegin = 'B900'
    ThetaArmSpeedHomeSlope = 'S1'
    ThetaArmSpeedHomeEnd = 'E900'
    ThetaArmHalfStep = 'H4'
    ThetaArmBackOffHome = 1200
    ThetaArmHome = 'N-110s'
    
    Carousel = 'Y1'
    CarouselPower = 'P140,50,0'
    CarouselSpeedBegin = 'B400'
    CarouselSpeedSlope = 'S5'
    CarouselSpeedEnd = 'E8000'
    CarouselSpeedHomeBegin = 'B1500'
    CarouselSpeedHomeSlope = 'S1'
    CarouselSpeedHomeEnd = 'E1500'
    CarouselHalfStep = 'H4'
    CarouselBackOffHome = 600
    CarouselHome = 'N-100s'
    CarouselHomeSwitch = 0
    
    PowerOff = 'P1,0,0'
    ZPowerOff = 'P20,20,0'
    
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
    
    def __init__(self, parent, title):
        self.svrLog = tesla.PgmLog.PgmLog( 'svrLog' )
        self.logPrefix = 'Dbg'
        self.Instrument = None
        self.platform = None
        self.robotCard = None
        self.AspiHeight = 0  
        frame = wx.Frame.__init__(self, parent, -1, title=title, size=(600,400) )
#        wx.EVT_CLOSE( self, self.OnClose )
        wx.EVT_SIZE(self, self.OnResize)

        x = y = 2       # starting points
        panel = wx.Panel( self, -1, pos=(x,y))
        
        cntlPanel = wx.Panel( self, -1, pos=(x,y), size=(580,120) )
        y = 5
        bpLabel = wx.StaticText( cntlPanel, -1, " Break point -- Highlight:", pos=(x+4, y))
        w, h = bpLabel.GetSize()
        y += h + 2
        self.bpTextCheckBox = wx.CheckBox( cntlPanel, -1, " messages containing", pos=(x+56,y+4))
        w, h = self.bpTextCheckBox.GetSize()
        self.findTxtCtrl = wx.TextCtrl( cntlPanel, -1, pos=(196, y), size=(290, h+8))
        w, h = self.findTxtCtrl.GetSize()
        self.continueButton = wx.Button( cntlPanel, -1, "Reset\nHighlight", pos=(496,y-2), size=(76,40))
#        self.continueButton = wx.Button( cntlPanel, -1, "Continue", pos=(496,y-2), size=(76,40))
        y += h + 2
        self.bpLevelCheckBox = wx.CheckBox( cntlPanel, -1, " levels at and above", pos=(x+56,y+4))
        w, h = self.bpLevelCheckBox.GetSize()
        logLevelNames = ['Verbose','Debug','Info','ID','Warning','Error']
        self.findLevelCtrl = wx.Choice( cntlPanel, -1, choices=logLevelNames, pos=(196, y), size=(290, h+8))
        self.findLevelCtrl.SetSelection( self.findLevelCtrl.FindString( 'Error' ))
        wx.EVT_CHOICE( self, self.findLevelCtrl.GetId(), self.OnLevelChange )
        self.logLevel = tesla.PgmLog.PgmLog.ERROR

        y += h + 18
        exCmdTxtLabel = wx.StaticText( cntlPanel, -1, " Execute SimpleStep commands:", pos=(x+4, y))
        w, h = exCmdTxtLabel.GetSize()
        self.ssCmdTxtCtrl = wx.TextCtrl( cntlPanel, -1, pos=(196, y), size=(290, h+8))
        self.execSSCommandButton = wx.Button( cntlPanel, -1, "Execute", pos=(496,y-2))
        wx.EVT_BUTTON( self, self.execSSCommandButton.GetId(), self.OnExecutePressed )
#        wx.EVT_BUTTON( self, self.continueButton.GetId(), self.OnContinuePressed )
        wx.EVT_BUTTON( self, self.continueButton.GetId(), self.OnResetHighlight )

        y += h + 14
        logTxtLabel = wx.StaticText( cntlPanel, -1, " Log messages:", pos=(x+4, y))
        w, h = logTxtLabel.GetSize()
        self.txtCtrl = wx.TextCtrl( self, -1, pos=(x-2,y+h+2), size=wx.Size(580,240),
                style=wx.TE_MULTILINE | wx.TE_RICH | wx.TE_READONLY | wx.TE_DONTWRAP)

        self.debugLevelCtrl = wx.Choice( cntlPanel, -1, choices=logLevelNames, pos=(90, y-8), size=(100, h+8))
        self.debugLevel = self.svrLog.logger.debuggerLogLevel
        self.debugLevelCtrl.SetSelection( self.debugLevelCtrl.FindString( self.GetDebugLevelString( self.debugLevel ) ))
        wx.EVT_CHOICE( self, self.debugLevelCtrl.GetId(), self.OnDebugLevelChange )

        self.globalSizer = wx.BoxSizer(wx.VERTICAL )
        self.globalSizer.Add(self.txtCtrl, 1, wx.EXPAND | wx.ALL, border=0 )
        panel.SetSizer(self.globalSizer)
        
        self.buttonSelection = 0
        self.logMessage = ''
        self.IsPauseTriggered = False
        self.IsDebugMode = False
        self.ExecuteReset()
        self.ContinueReset()
        self.level = 30

        self.Show(True)

    def loadBarcodeTestTable(self):
        vialIDpath           = os.path.join(tesla.config.CONFIG_DIR, 'bcrtest.txt');
        self.bcrTestOutpath       = os.path.join(tesla.config.LOG_DIR,    'bcrtestout.txt');
        self.bcrTestOutDetailpath = os.path.join(tesla.config.LOG_DIR,    'bcrtestoutdetail.txt');
        vialList = None;
        
        try:
          self.vialIDs = [ [None,None,None],[None,None,None],[None,None,None],[None,None,None] ];
          f = open( vialIDpath, 'r' );
          vialList = f.readlines();
          f.close();
          qdr  = 0;
          vial = 0;

          if len(vialList) < 12:
             print '\n>>> Cannot import %s \n'%vialIDpath;
             self.vialIDs = None;
          else:   
             idx = 0;
             for i in range(0, 4):
                 qdr  = i;
                 for k in range(0, 3): 
                     vial = k;
                     self.vialIDs[qdr][vial] = vialList[idx].strip();
                     idx = idx + 1;
        except:
          print '\n>>> Cannot open %s \n'%vialIDpath;
          self.vialIDs = None;
        
    def setInstrument(self, Instrument, platform):
        self.Instrument = Instrument
        self.platform = platform
        if( self.platform == None ):
            self.robotCard = None
        else:
            self.robotCard = self.platform.robot.m_Card

    def OnResize( self, evt):
        (width, height) = evt.GetSize()
        self.txtCtrl.SetSize( (width-18, height-160) )

    def OnClose(self, evt):
        dlg = wx.MessageDialog(self, 'Are you sure you want to close the debugger?', 'Closing...', wx.YES_NO | wx.ICON_QUESTION)
        ret = dlg.ShowModal()
        dlg.Destroy()

        if ret == wx.ID_YES:
            evt.Skip()

    def OnExecutePressed( self, evt ):
        self.IsExecuteActive = True
        self.ExecuteCommand()
        self.ExecuteReset()

    def OnContinuePressed( self, evt ):
        self.IsContinueActive = True

    def OnResetHighlight( self, evt ):
        self.SetTextColour( self.DEFAULT_TEXT_COLOR )

    def OnLevelChange( self, evt ):
        choice = self.findLevelCtrl.GetStringSelection()
        if( choice == 'Debug' ):
            self.logLevel = tesla.PgmLog.PgmLog.DEBUG
        elif( choice == 'Info' ):
            self.logLevel = tesla.PgmLog.PgmLog.INFO
        elif( choice == 'ID' ):
            self.logLevel = tesla.PgmLog.PgmLog.ID
        elif( choice == 'Warning' ):
            self.logLevel = tesla.PgmLog.PgmLog.WARNING
        elif( choice == 'Error' ):
            self.logLevel = tesla.PgmLog.PgmLog.ERROR
        else:
            self.logLevel = tesla.PgmLog.PgmLog.VERBOSE

    def OnDebugLevelChange( self, evt ):
        choice = self.debugLevelCtrl.GetStringSelection()
        if( choice == 'Debug' ):
            self.debugLevel = tesla.PgmLog.PgmLog.DEBUG
        elif( choice == 'Info' ):
            self.debugLevel = tesla.PgmLog.PgmLog.INFO
        elif( choice == 'ID' ):
            self.debugLevel = tesla.PgmLog.PgmLog.ID
        elif( choice == 'Warning' ):
            self.debugLevel = tesla.PgmLog.PgmLog.WARNING
        elif( choice == 'Error' ):
            self.debugLevel = tesla.PgmLog.PgmLog.ERROR
        else:
            self.debugLevel = tesla.PgmLog.PgmLog.VERBOSE
        self.svrLog.setLoggerLevels( debuggerLogLevel=self.debugLevel )

    def GetDebugLevelString( self, logLevel ):
        debugLevel = 'Verbose'
        if( logLevel == tesla.PgmLog.PgmLog.DEBUG ):
            debugLevel = 'Debug'
        elif( logLevel == tesla.PgmLog.PgmLog.INFO ):
            debugLevel = 'Info'
        elif( logLevel == tesla.PgmLog.PgmLog.ID ):
            debugLevel = 'ID'
        elif( logLevel == tesla.PgmLog.PgmLog.WARNING ):
            debugLevel = 'Warning'
        elif( logLevel == tesla.PgmLog.PgmLog.ERROR ):
            debugLevel = 'Error'
        return( debugLevel )
            

    def GetLogThreshold( self ):
        return( self.logLevel )

    def ExecuteCommand(self):
#        debugHandler = tesla.DebugHandler.GetSSDebugHandlerInstance()
#        debugHandler.ExecuteDebuggerCommand( self.ssCmdTxtCtrl.GetValue() )
        self.ExecuteDebuggerCommand( self.ssCmdTxtCtrl.GetValue() )

    def AppendMessage(self, msg, maxLines=100):
        numLines = self.txtCtrl.GetNumberOfLines()
        if( numLines >= maxLines ):
            self.RemoveFirstLine()
        self.txtCtrl.AppendText( msg + '\n')


    def SetTextColour(self, colour=DEFAULT_TEXT_COLOR):
        self.textColor = colour
        self.txtCtrl.SetDefaultStyle(wx.TextAttr( self.textColor ))


    def ActivatePause( self ):
#        self.continueButton.SetBackgroundColour( ( 250, 210, 210 ) )
        if( self.Instrument != None and not self.IsDebugMode ):
            self.SetTextColour( self.BREAKPOINT_TEXT_COLOR )
            self.IsPauseTriggered = True
            self.ActivateDebugMode( )

    def ActivateDebugMode( self ):
        self.IsDebugMode = True
        self.IsContinueActive = False
        self.IsPauseTriggered = False
        self.continueButton.Enable( True )
        self.execSSCommandButton.Enable( True )

    def ContinueReset( self ):
        self.IsContinueActive = True
        self.IsDebugMode = False
        self.continueButton.Disable( )
        self.execSSCommandButton.Disable( )
        self.SetTextColour( self.DEFAULT_TEXT_COLOR )

    def IsDebuggerPaused( self ):
        return self.IsDebugMode

    def IsContinueActivated( self ):
        return self.IsContinueActive

    def ExecuteReset( self ):
        self.IsExecuteActive = False

    def IsExecuteActivated( self ):
        return self.IsExecuteActive

    def GetExecuteCommand( self ):
        return self.ssCmdTxtCtrl.GetValue()

    def RemoveFirstLine(self):
        lineLength = self.txtCtrl.GetLineLength(0) + 1
        self.txtCtrl.Remove(0, lineLength)

    def SetDebugModeInUse(self):
        self.debugModeInUse = True

    def ResetDebugModeInUse(self):
        self.debugModeInUse = False

    def IsDebugModeInUse(self):
        return self.debugModeInUse

    def EnableDebugExecuteButton(self):
        self.execSSCommandButton.Enable( True )

    def DisableDebugExecuteButton(self):
        if( not self.IsDebugModeInUse() ):
            self.execSSCommandButton.Disable( )


    def logDebuggerMode(self):
        funcReference = __name__ + '.logDebuggerMode'  # 2011-11-29 -- sp
        self.SetDebugModeInUse()
        self.svrLog.logDebug('', self.logPrefix, funcReference, 'Debugger activated, system paused' )
        while( self.IsDebuggerPaused() ):
#            if( ssLogDebugger.IsExecuteActivated() ):
#                debuggerCommand = ssLogDebugger.GetExecuteCommand()
#                self.svrLog.logDebug('', self.logPrefix, funcReference,
#                            'Debugger executing: %s' % debuggerCommand )
#                ssLogDebugger.ExecuteReset()
#                self.ExecuteDebuggerCommand( debuggerCommand )
            if( self.IsContinueActivated() ):
                self.svrLog.logDebug('', self.logPrefix, funcReference,
                            'Debugger de-activated, continue initiated' )
                self.ContinueReset()
            time.sleep( 0.5 )
        self.ResetDebugModeInUse()


    def ExecuteDebuggerCommand(self, command):
        funcReference = __name__ + '.ExecuteDebuggerCommand'  # 2011-11-29 -- sp
        commandList = command.split()
        logLevel = self.svrLog.logger.debuggerLogLevel
        self.svrLog.setLoggerLevels(debuggerLogLevel=self.svrLog.VERBOSE)
        textColor = self.textColor
        self.SetTextColour( self.EXECUTE_TEXT_COLOR )
        if (len(commandList) == 0) or (commandList[0].upper() == 'HELP'):   # db added 2016-02-23
            self.svrLog.logDebug('', self.logPrefix, funcReference, 'POSSIBLE COMMANDS: ' + \
                'DCV, SS, BCR, CX, TX, ZX, ASPI')                                 # db added 2016-02-23
        else:
            if( commandList[0].upper() == 'SS'):
                self.ProcessSimpleStepCommands(commandList)
            elif( commandList[0].upper() == 'BCR'):
                self.ProcessBarCodeReaderCommand(commandList)
            elif( commandList[0].upper() == 'CX'):
                self.ProcessAxisCommand(self.Carousel, commandList[1])
            elif( commandList[0].upper() == 'TX'):
                self.ProcessAxisCommand(self.ThetaArm, commandList[1])
            elif( commandList[0].upper() == 'ZX'):
                self.ProcessAxisCommand(self.ZAxis, commandList[1])
            elif (commandList[0].upper() == 'DCV'): # db added 2016-02-16
                self.EnableAutoDCV(commandList)     # db added 2016-02-16
            elif (commandList[0].upper() == 'ASPI'):
                self.ProcessAspiHeight(commandList)
            else:
                self.svrLog.logDebug('', self.logPrefix, funcReference, \
                    'Invalid command=%s ; Enter "help" for a list of commands' % command ) # db changed 2016-02-23
        self.SetTextColour( textColor )
        self.svrLog.setLoggerLevels(debuggerLogLevel=logLevel)


    def ProcessSimpleStepCommands(self, commandList):
        for index in range( 1, len( commandList )):
            self.sendLowCmd( commandList[index] )

    def ProcessAspiHeight(self, cmdList):
        #trace = RoboTrace.GetRoboTracerInstance()
        if (cmdList[1].isdigit()):
            self.AspiHeight = int(cmdList[1])
            print ('## New Sep AspiHeight : %d'%self.AspiHeight)
        pass
        
    def ProcessAxisCommand(self, axis, command):
        if( command.lower() == 'free' ):
            self.EnableFreeMotion(axis)
        elif( command.lower() == 'stepshome' ):
            self.FindStepsHome(axis)
        elif( command.lower() == 'home' ):
            self.HomeDevice(axis)
        elif( command.lower() == 'reset' ):
            self.ResetDevice(axis)
        elif( command.lower() == 'homeswitch' ):
            self.SetHomeSwitch(axis)
            
    def EnableAutoDCV(self, commandList):   # db added 2016-02-16
        funcReference = __name__ + '.EnableAutoDCV'
        textColor = self.textColor
        if (len(commandList) > 1) and self.ValidateAutoDCVArgs(commandList[1]): # check for valid input
            os.environ['ROBO_DCVJIG_ZT'] = commandList[1]                       # set ROBO_DCVJIG_ZT
            self.SetTextColour( self.RETURN_TEXT_COLOR )
            self.svrLog.logDebug('', self.logPrefix, funcReference, \
                'OS env var [ROBO_DCVJIG_ZT] successfully set to: %s' % os.environ['ROBO_DCVJIG_ZT'])
        else:
            self.svrLog.logDebug('', self.logPrefix, funcReference, \
                'INVALID USAGE. Correct usage: "DCV <z-height>;<theta-offset>"\n' + \
                'where (10 <= z-height <= 148) and (0 <= theta-offset <= 160)')
        self.SetTextColour(textColor)
        
    def ValidateAutoDCVArgs(self, args):    # db added 2016-02-16
        funcReference = __name__ + '.ValidateAutoDCV'
        _args = args.split(';')
        if len(_args) == 2:                                     # check for two semi-colon-separated arguments
            try:                                                # convert arguments to floating point values
                zHeight = float(_args[0])
                thetaOffset = float(_args[1])
            except ValueError:
                return False
            if (zHeight >= 10) and (zHeight <= 148):            # validate legal zHeight
                if (thetaOffset >= 0) and (thetaOffset <= 160): # validate legal thetaOffset
                    return True
        return False

    def sendLowCmd( self, command ):
        funcReference = __name__ + '.sendAndCheck'  # 2011-11-29 -- sp
        textColor = self.textColor
        self.SetTextColour( self.EXECUTE_TEXT_COLOR )
        self.svrLog.logDebug('', self.logPrefix, funcReference, 'Debugger SimpleStep cmd=%s' % command )
        self.SetTextColour( self.RETURN_TEXT_COLOR )
#                msg = self.port.sendAndCheck( commandList[index], self.SERIAL_PAUSE )
        msg = self.robotCard.SendLowCmd( command, self.SERIAL_PAUSE, 0 )
        self.svrLog.logDebug('', self.logPrefix, funcReference, 'Debugger SimpleStep result=%s' % msg )
        self.SetTextColour( textColor )
        return( msg )


    def sendAndCheck( self, axis, command ):
        funcReference = __name__ + '.sendAndCheck'  # 2011-11-29 -- sp
        textColor = self.textColor
        self.SetTextColour( self.EXECUTE_TEXT_COLOR )
        self.svrLog.logDebug('', self.logPrefix, funcReference, 'Debugger SimpleStep cmd=%s' % axis+command )
        self.SetTextColour( self.RETURN_TEXT_COLOR )
        self.pollUntilReady(axis)
#                msg = self.port.sendAndCheck( commandList[index], self.SERIAL_PAUSE )
        msg = self.robotCard.SendLowCmd( (axis + command), self.SERIAL_PAUSE, 0 )
        self.pollUntilReady(axis)
        self.svrLog.logDebug('', self.logPrefix, funcReference, 'Debugger SimpleStep result=%s' % msg )
        self.SetTextColour( textColor )
        return( msg )

    def pollUntilReady(self, axis, delay = POLL_PAUSE):             #Check Axis.py
        """Polls the card for a response until it replies or the retry count
        is exceeded.
        Return the card status, or None if the card isn't responding.
        A good card status is ">" (= SYSTEM_READY). Refer to the
        SimpleStep manual for the full set of status flags that can be
        returned."""
        self.prefixResponse           = axis.lower()
        self.goodResponse             = (self.prefixResponse + self.SYSTEM_READY)
        self.busyResponse             = (self.prefixResponse + self.SYSTEM_BUSY)
        self.abortResponse            = (self.prefixResponse + self.SYSTEM_ABORT)
        self.notHomedResponse         = (self.prefixResponse + self.SYSTEM_NOT_HOMED)

        for i in range(0, self.RETRIES):                  #RETRIES = 200
            status = self.robotCard.SendLowCmd(axis, delay, 1)
            if (status != None):
                status = status.strip('\r')
            # get payload, status - if good reply exit retry loop
            if (status in (self.goodResponse, self.notHomedResponse)):
                break

            time.sleep(0.2)                  #RETRY_PAUSE = 20

    def ProcessBarCodeReaderCommand(self, command):
        funcReference = __name__ + '.ProcessBarCodeReaderCommand'  # 2011-11-29 -- sp
        success = -1
        try:
            if( command[1].lower() == 'init' ):
                self.Instrument.InitScanVialBarcode( False )  # reset all barcode values
                success = 1
            elif( command[1].lower() == 'abort' ):
                self.Instrument.AbortScanVialBarcode()
                success = 1
            elif( command[1].lower() == 'check' ):
                self.Instrument.IsBarcodeScanningDone()
                success = 1
            elif( command[1].lower() == 'close' ):
                self.Instrument.barcodeReader.close()
                success = 1

            if( command[1].lower() == 'read' ):
                quadrants = -9
                vials = -9
                if( len( command ) > 2 ):
                    if( command[2].isdigit() ):
                        quadrants = int( command[2] )
                    elif( command[2][0] in ('-', '+') ):
                        if( command[2][1:].isdigit() ):
                            quadrants = int( command[2] )
                if( len( command ) > 3 ):
                    if( command[3].isdigit() ):
                        vials = int( command[3] )
                    elif( command[3][0] in ('-', '+') ):
                        if( command[3][1:].isdigit() ):
                            vials = int( command[3] )
                    if( quadrants == 0 and vials == 0 ):
                        self.Instrument.HomeAll()

                    rtn = self.Instrument.ScanVialBarcodeAtForCheckup(quadrants, vials)
                    if( rtn != 0 ):
                        self.svrLog.logDebug('', self.logPrefix, funcReference, 'Invalid parameters specified for quadrant or vial')
                else:
                    self.svrLog.logDebug('', self.logPrefix, funcReference, 'Insufficient parameters, quadrant and vial needed')

                for q in range(4) :
                    for v in range(3) :
                        self.svrLog.logDebug('', self.logPrefix, funcReference, 'quad=%d| vial=%d| value=%s' % 
                            ( q+1, v+1, self.Instrument.GetVialBarcodeAt(q+1, v+1) ) )
                success = 1

            if( command[1].lower() == 'contread' ):
                Iter      = 1
                
                self.loadBarcodeTestTable();
                                
                if (self.vialIDs == None):
                    self.svrLog.logDebug('', self.logPrefix, funcReference, 'Invalid ''bcrtest.txt'' file.')
                    return;
                
                if( len( command ) == 3 ):
                    if( command[2].isdigit() ):
                        Iter = int( command[2] )
                    else:     
                        self.svrLog.logDebug('', self.logPrefix, funcReference, 'Insufficient parameters, iteration number needed')
                        return;
    
                    tableLotID   = None;
                    barcodeLotID = None;
                    
                    fs = open(self.bcrTestOutpath, 'w');
                    fd = open(self.bcrTestOutDetailpath, 'w');
                      
                    for i in range(0, Iter):                        
                      self.Instrument.HomeAll()
                      
                      self.Instrument.ScanVialBarcodeAtForCheckup(-1, -1)           # Clear Barcode holding array;
                      fs.write('%s -------------- BCR Cleared %s\n'%(self.GetTimeStamp(), self.Instrument.barcodes));
                      fd.write('%s -------------- BCR Cleared %s\n'%(self.GetTimeStamp(), self.Instrument.barcodes));
                                            
                      rtn = self.Instrument.ScanVialBarcodeAtForCheckup(0, 0)
                      if( rtn != 0 ):
                          self.svrLog.logDebug('', self.logPrefix, funcReference, 'Invalid parameters specified for quadrant or vial')
                          
                      self.svrLog.logDebug('', self.logPrefix, funcReference, '-------------- BCR Iteration=%d starts-------------\n'%(i+1))
                      
                      fs.write('%s -------------- BCR Iteration=%d starts-------------\n'%(self.GetTimeStamp(), i+1))
                      fd.write('%s -------------- BCR Iteration=%d starts-------------\n'%(self.GetTimeStamp(), i+1))                      
                      print('-------------- BCR Iteration=%d starts-------------\n'%(i+1));
                      
                      for q in range(0,4) :
                          for v in range(0,3) :
                              barcodeLotID = self.Instrument.GetVialBarcodeAt(q+1, v+1)
                              barcodeStatus= self.Instrument.GetVialBarcodeStatusAt(q+1, v+1)
                              tableLotID   = self.vialIDs[q][v];
                              if (tableLotID == barcodeLotID):
                                      self.svrLog.logDebug('', self.logPrefix, funcReference, 'CORRECT:   quad=%d| vial=%d| barcode=%s ' % ( q+1, v+1, barcodeLotID) )
                                      fd.write('%s CORRECT:   quad=%d| vial=%d| barcode=%s \n' % ( self.GetTimeStamp(), q+1, v+1, barcodeLotID) )                                      
                                      print('CORRECT:   quad=%d| vial=%d| barcode=%s ' % ( q+1, v+1, barcodeLotID) );
                              else:
                                      self.svrLog.logDebug('', self.logPrefix, funcReference, 'INCORRECT: quad=%d| vial=%d| barcode=%s | table=%s | status=%s' % ( q+1, v+1, barcodeLotID, tableLotID, barcodeStatus) )
                                      fs.write('%s INCORRECT: quad=%d| vial=%d| barcode=%s | table=%s | status=%s \n' % ( self.GetTimeStamp(), q+1, v+1, barcodeLotID, tableLotID, barcodeStatus) )
                                      fd.write('%s INCORRECT: quad=%d| vial=%d| barcode=%s | table=%s | status=%s \n' % ( self.GetTimeStamp(), q+1, v+1, barcodeLotID, tableLotID, barcodeStatus) )
                                      print('INCORRECT: quad=%d| vial=%d| barcode=%s | table=%s | status=%s ' % ( q+1, v+1, barcodeLotID, tableLotID, barcodeStatus) )
                                                                            
                      self.svrLog.logDebug('', self.logPrefix, funcReference, '--------------BCR Iteration finished-------------')
                      fs.write('%s --------------BCR Iteration finished-------------\n'%self.GetTimeStamp());
                      fd.write('%s --------------BCR Iteration finished-------------\n'%self.GetTimeStamp());      
                      print('--------------BCR Iteration finished-------------\n');    
                                  
                else:
                    self.svrLog.logDebug('', self.logPrefix, funcReference, 'Invalid command format')
                    fs.close();                      
                    fd.close();
                    return;
                fs.close();
                fd.close();
                success = 1

            if( command[1].lower() == 'goto' ):
                quadrants = -9
                vials = -9
                if( len( command ) > 2 ):
                    if( command[2].isdigit() ):
                        quadrants = int( command[2] )
                    elif( command[2][0] in ('-', '+') ):
                        if( command[2][1:].isdigit() ):
                            quadrants = int( command[2] )
                if( len( command ) > 3 ):
                    if( command[3].isdigit() ):
                        vials = int( command[3] )
                    elif( command[3][0] in ('-', '+') ):
                        if( command[3][1:].isdigit() ):
                            vials = int( command[3] )
                    if( quadrants == 0 and vials == 0 ):
                        self.Instrument.HomeAll()
                    rtn = self.Instrument.GotoVialBarcode(quadrants, vials)
                    if( rtn != 0 ):
                        self.svrLog.logDebug('', self.logPrefix, funcReference, 'Invalid parameters specified for quadrant or vial')
                else:
                    self.svrLog.logDebug('', self.logPrefix, funcReference, 'Insufficient parameters, quadrant and vial needed')
                success = 1
                                
        except :
            self.svrLog.logDebug('', self.logPrefix, funcReference, 'Error in command format: BCR read / goto <quadrant> <vial> / init / close ' )

        if( success == 1 ):
            self.svrLog.logDebug('', self.logPrefix, funcReference, 'command=%s completed' % command )
        else:
            self.svrLog.logDebug('', self.logPrefix, funcReference, 'command=%s| Error| Format: BCR read <quadrant> <vial> ' % command )
            

    def ResetDevice(self, axis):
        resetCmd = '!'
        if( axis == self.ZAxis ):
            self.sendAndCheck( self.ZAxis, resetCmd )
            self.sendAndCheck( self.ZAxis, self.ZAxisPower )
            self.sendAndCheck( self.ZAxis, self.ZAxisHalfStep )
            self.sendAndCheck( self.ZAxis, self.ZAxisSpeedBegin )
            self.sendAndCheck( self.ZAxis, self.ZAxisSpeedSlope )
            self.sendAndCheck( self.ZAxis, self.ZAxisSpeedEnd )
        elif( axis == self.ThetaArm ):
            self.sendAndCheck( self.ThetaArm, resetCmd )
            self.sendAndCheck( self.ThetaArm, self.ThetaArmPower )
            self.sendAndCheck( self.ThetaArm, self.ThetaArmHalfStep )
            self.sendAndCheck( self.ThetaArm, self.ThetaArmSpeedBegin )
            self.sendAndCheck( self.ThetaArm, self.ThetaArmSpeedSlope )
            self.sendAndCheck( self.ThetaArm, self.ThetaArmSpeedEnd )
        elif( axis == self.Carousel ):
            self.sendAndCheck( self.Carousel, resetCmd )
            self.sendAndCheck( self.Carousel, self.CarouselPower )
            self.sendAndCheck( self.Carousel, self.CarouselHalfStep )
            self.sendAndCheck( self.Carousel, self.CarouselSpeedBegin )
            self.sendAndCheck( self.Carousel, self.CarouselSpeedSlope )
            self.sendAndCheck( self.Carousel, self.CarouselSpeedEnd )
            

    def SetOperatingDefaults(self, axis):
        if( axis == self.ZAxis ):
            self.sendAndCheck( self.ZAxis, self.ZAxisPower )
            self.sendAndCheck( self.ZAxis, self.ZAxisHalfStep )
            self.sendAndCheck( self.ZAxis, self.ZAxisSpeedBegin )
            self.sendAndCheck( self.ZAxis, self.ZAxisSpeedSlope )
            self.sendAndCheck( self.ZAxis, self.ZAxisSpeedEnd )
        elif( axis == self.ThetaArm ):
            self.sendAndCheck( self.ThetaArm, self.ThetaArmPower )
            self.sendAndCheck( self.ThetaArm, self.ThetaArmHalfStep )
            self.sendAndCheck( self.ThetaArm, self.ThetaArmSpeedBegin )
            self.sendAndCheck( self.ThetaArm, self.ThetaArmSpeedSlope )
            self.sendAndCheck( self.ThetaArm, self.ThetaArmSpeedEnd )
        elif( axis == self.Carousel ):
            self.sendAndCheck( self.Carousel, self.CarouselPower )
            self.sendAndCheck( self.Carousel, self.CarouselHalfStep )
            self.sendAndCheck( self.Carousel, self.CarouselSpeedBegin )
            self.sendAndCheck( self.Carousel, self.CarouselSpeedSlope )
            self.sendAndCheck( self.Carousel, self.CarouselSpeedEnd )

    def HomeDevice(self, axis, mode=''):
        position = 0
        if( axis == self.ZAxis ):
            self.sendAndCheck( self.ZAxis, self.ZAxisPower )
            self.sendAndCheck( self.ZAxis, self.ZAxisSpeedHomeBegin )
            self.sendAndCheck( self.ZAxis, self.ZAxisSpeedHomeSlope )
            self.sendAndCheck( self.ZAxis, self.ZAxisSpeedHomeEnd )
            if( self.GetHomeSwStatus(axis) == 1 ):
                position = self.ZAxisBackOffHome
                self.StepBackFromHome( axis )
            self.sendAndCheck( self.ZAxis, self.ZAxisHome + mode )
        elif( axis == self.ThetaArm ):
            self.sendAndCheck( self.ThetaArm, self.ThetaArmPower )
            self.sendAndCheck( self.ThetaArm, self.ThetaArmSpeedHomeBegin )
            self.sendAndCheck( self.ThetaArm, self.ThetaArmSpeedHomeSlope )
            self.sendAndCheck( self.ThetaArm, self.ThetaArmSpeedHomeEnd )
            if( self.GetHomeSwStatus(axis) == 1 ):
                position = self.ThetaArmBackOffHome
                self.StepBackFromHome( axis )
            self.sendAndCheck( self.ThetaArm, self.ThetaArmHome + mode )
        elif( axis == self.Carousel ):
            self.sendAndCheck( self.Carousel, self.CarouselPower )
            self.sendAndCheck( self.Carousel, self.CarouselSpeedHomeBegin )
            self.sendAndCheck( self.Carousel, self.CarouselSpeedHomeSlope )
            self.sendAndCheck( self.Carousel, self.CarouselSpeedHomeEnd )
            if( self.GetHomeSwStatus(axis) == self.CarouselHomeSwitch ):
                position = self.CarouselBackOffHome
                self.StepBackFromHome( axis )
            self.sendAndCheck( self.Carousel, self.CarouselHome + mode )
        return( position )

    def StepBackFromHome( self, axis ):
        if( axis == self.ZAxis ):
            position = self.sendAndCheck( self.ZAxis, 'RN+' + str(self.ZAxisBackOffHome) )
        elif( axis == self.ThetaArm ):
            position = self.sendAndCheck( self.ThetaArm, 'RN+' + str(self.ThetaArmBackOffHome) )
        elif( axis == self.Carousel ):
            position = self.sendAndCheck( self.Carousel, 'RN+' + str(self.CarouselBackOffHome) )
        return( position )

    def SetHomeSwitch( self, axis ):
        if( axis == self.Carousel ):
            self.sendAndCheck( self.Carousel, self.CarouselPower )
            self.sendAndCheck( self.Carousel, self.CarouselSpeedHomeBegin )
            self.sendAndCheck( self.Carousel, self.CarouselSpeedHomeSlope )
            self.sendAndCheck( self.Carousel, self.CarouselSpeedHomeEnd )
            if( self.GetHomeSwStatus(axis) == 0 ):
                self.CarouselBackOffHome
                if( self.GetHomeSwStatus(axis) == 0 ):
                    self.StepBackFromHome( axis )
                    if( self.GetHomeSwStatus(axis) == 0 ):
                        self.CarouselHome = 'N-110s'
                        self.CarouselHomeSwitch = 1


    def EnableFreeMotion(self, axis):
        if( axis == self.ZAxis ):
            self.sendAndCheck( self.ZAxis, self.ZPowerOff )
        elif( axis == self.ThetaArm ):
            self.sendAndCheck( self.ThetaArm, self.PowerOff )
        elif( axis == self.Carousel ):
            self.sendAndCheck( self.Carousel, self.PowerOff )

    def GetPosition(self, axis):
        return( self.GetIntegerResponse( axis, 'm' ) )

    def GetHomeSwStatus(self, axis):
        return( self.GetIntegerResponse( axis, 'I1' ) )

    def GetIntegerResponse(self, axis, command):
        msg = self.sendAndCheck( axis, command )
        position = self.INVALID_POSITION
        msgLen = len(msg)
        if( msgLen > 3 ):
            value = msg[ 3 : (msgLen - 1) ]
            if( ( value.startswith('-') & value[1:].isdigit() ) | value.isdigit() ):
                position = int( value )
        return( position )


    def FindStepsHome( self, axis ):
        funcReference = __name__ + '.ProcessBarCodeReaderCommand'  # 2011-11-29 -- sp
        self.EnableFreeMotion(axis)
        a = raw_input('Manual operation has been selected.\nThe system can be moved freely.\n\nPress enter/return/ok to return to automatic mode...')
        backOffHome = self.HomeDevice(axis, 'z')
        position = 2147483647 - self.GetPosition( axis ) - backOffHome
        print( 'Position = ,' + str(position) + ',' )
        self.svrLog.logDebug('', self.logPrefix, funcReference, 'position=%d' % position )
        self.SetOperatingDefaults(axis)
        
    def DumpParameters(self):
        dmplist = [];
        dmplist.extend(['ZAxis=' + self.ZAxis])
        dmplist.extend(['ZAxisPower=' + self.ZAxisPower])
        dmplist.extend(['ZAxisSpeedBegin=' + self.ZAxisSpeedBegin])
        dmplist.extend(['ZAxisSpeedSlope=' + self.ZAxisSpeedSlope])
        dmplist.extend(['ZAxisSpeedEnd=' + self.ZAxisSpeedEnd])
        dmplist.extend(['ZAxisSpeedHomeBegin=' + self.ZAxisSpeedHomeBegin])
        dmplist.extend(['ZAxisSpeedHomeSlope=' + self.ZAxisSpeedHomeSlope])
        dmplist.extend(['ZAxisSpeedHomeEnd=' + self.ZAxisSpeedHomeEnd])
        dmplist.extend(['ZAxisHalfStep=' + self.ZAxisHalfStep])
        dmplist.extend(['ZAxisBackOffHome=' + str(self.ZAxisBackOffHome)])
        dmplist.extend(['ZAxisHome=' + self.ZAxisHome])
    
        dmplist.extend(['PumpAxis=' + self.PumpAxis])
        dmplist.extend(['PumpPower=' + self.PumpPower])
        dmplist.extend(['PumpSpeedBegin=' + self.PumpSpeedBegin])
        dmplist.extend(['PumpSpeedSlope=' + self.PumpSpeedSlope])
        dmplist.extend(['PumpSpeedEnd=' + self.PumpSpeedEnd])
        dmplist.extend(['PumpSpeedHomeBegin=' + self.PumpSpeedHomeBegin])
        dmplist.extend(['PumpSpeedHomeSlope=' + self.PumpSpeedHomeSlope])
        dmplist.extend(['PumpSpeedHomeEnd=' + self.PumpSpeedHomeEnd])
        dmplist.extend(['PumpHalfStep=' + self.PumpHalfStep])
        dmplist.extend(['PumpBackOffHome=' + str(self.PumpBackOffHome)])
        dmplist.extend(['PumpHome=' + self.PumpHome])
    
        dmplist.extend(['ThetaArm=' + self.ThetaArm])
        dmplist.extend(['ThetaArmPower=' + self.ThetaArmPower])
        dmplist.extend(['ThetaArmSpeedBegin=' + self.ThetaArmSpeedBegin])
        dmplist.extend(['ThetaArmSpeedSlope=' + self.ThetaArmSpeedSlope])
        dmplist.extend(['ThetaArmSpeedEnd=' + self.ThetaArmSpeedEnd])
        dmplist.extend(['ThetaArmSpeedHomeBegin=' + self.ThetaArmSpeedHomeBegin])
        dmplist.extend(['ThetaArmSpeedHomeSlope=' + self.ThetaArmSpeedHomeSlope])
        dmplist.extend(['ThetaArmSpeedHomeEnd=' + self.ThetaArmSpeedHomeEnd])
        dmplist.extend(['ThetaArmHalfStep=' + self.ThetaArmHalfStep])
        dmplist.extend(['ThetaArmBackOffHome=' + str(self.ThetaArmBackOffHome)])
        dmplist.extend(['ThetaArmHome=' + self.ThetaArmHome])
    
        dmplist.extend(['Carousel=' + self.Carousel])
        dmplist.extend(['CarouselPower=' + self.CarouselPower])
        dmplist.extend(['CarouselSpeedBegin=' + self.CarouselSpeedBegin])
        dmplist.extend(['CarouselSpeedSlope=' + self.CarouselSpeedSlope])
        dmplist.extend(['CarouselSpeedEnd=' + self.CarouselSpeedEnd])
        dmplist.extend(['CarouselSpeedHomeBegin=' + self.CarouselSpeedHomeBegin])
        dmplist.extend(['CarouselSpeedHomeSlope=' + self.CarouselSpeedHomeSlope])
        dmplist.extend(['CarouselSpeedHomeEnd=' + self.CarouselSpeedHomeEnd])
        dmplist.extend(['CarouselHalfStep=' + self.CarouselHalfStep])
        dmplist.extend(['CarouselBackOffHome=' + str(self.CarouselBackOffHome)])
        dmplist.extend(['CarouselHome=' + self.CarouselHome])
        dmplist.extend(['CarouselHomeSwitch=' + str(self.CarouselHomeSwitch)])
    
        dmplist.extend(['PowerOff=' + self.PowerOff])
        dmplist.extend(['ZPowerOff=' + self.ZPowerOff])
        
        return dmplist;

#
#
#    def OnAbout(self,e):
#        # A message dialog box with an OK button. wx.OK is a standard ID in wxWidgets.
#        dlg = wx.MessageDialog( self, "A small text editor", "About Sample Editor", wx.OK)
#        dlg.ShowModal() # Show it
#        dlg.Destroy() # finally destroy it when finished.
#
#    def OnExit(self,e):
#        self.Close(True)  # Close the frame.
#


class DebuggerApp(wx.App):
    def OnInit(self):
        frame = DebuggerWindow(None, "RoboSep Server Debugger")

        #frame = wx.Frame(None, -1, "This is a test", size=(400,300))
        frame.Show(True)
        # tell wxPython that this is our main window
        self.SetTopWindow(frame)
        # return an optional success flag
        return True


class StartGUIThread (threading.Thread ):
    def run( self ):
        global debugApp
        debugApp = DebuggerApp(0)
        debugApp.MainLoop()


def startLogDebugger():
    thrd = StartGUIThread()
    # comment the following line or set to False, if the debugger is to remain after program execution
    thrd.setDaemon(True)
    thrd.start()
    time.sleep(1)               # allow some time for components to initialize
    return thrd


def getSSDebuggerInstance():
    global debugThread

    try:
        debugThread
        if( not debugThread.isAlive() ):
           debugThread = startLogDebugger()
    except NameError:
       debugThread = startLogDebugger()

    return debugThread



def GetSSTracerInstance():
    global ssTracer
    
    if ssTracer == None:
        ssTracer = SSDebugger()
        ssTracer.LoadDebbugerParameter();
        dmpmsg = ssTracer.DumpDebbugerParameter();
        #dumpParam = open("C:\Program Files\STI\RoboSep\logs\debuggerparamdump.txt","w")
        dumpParam = open("C:\debuggerparamdump.txt","w")        
        for iter in range(0,len(dmpmsg)):
            dumpParam.write(dmpmsg[iter]+'\n')
        dumpParam.close();
    return ssTracer


class SSDebugger:
    def __init__(self):
        self.debugThread = getSSDebuggerInstance()
        self.frame = debugApp.GetTopWindow()
        self.frame.ResetDebugModeInUse()
        self.Instrument = None
        self.platform = None
        pass

    def GetDebugApp(self):
        try:
            debugApp
            if( not self.debugThread.isAlive() ):
                self.debugThread  = startLogDebugger()
                self.frame = debugApp.GetTopWindow()
                self.frame.ResetDebugModeInUse()
                self.frame.setInstrument(self.Instrument, self.platform)
        except NameError:
            print( 'NameError' )
            self.debugThread  = startLogDebugger()
            self.frame = debugApp.GetTopWindow()
            self.frame.ResetDebugModeInUse()
            self.frame.setInstrument(self.Instrument, self.platform)

        
    def setInstrument(self, Instrument, platform):
        self.GetDebugApp()
        self.Instrument = Instrument
        self.platform = platform
        self.frame.setInstrument(self.Instrument, self.platform)


    def AppendMessage(self, message, logLevel=logging.NOTSET, maxLines=100):
        self.GetDebugApp()
#        currentTime = datetime.datetime.now()
#        timestamp = currentTime.strftime( '%y-%m-%d_%H:%M:%S' ) + '.%03d' % (currentTime.microsecond//1000)
#        msg = '%s\tsvr\t%s' % ( timestamp, message )
        searchString = self.frame.findTxtCtrl.GetValue()
        if( self.IsDebuggerPaused() ):
            self.frame.AppendMessage(message, maxLines)
        else:
            activatePause = False
            if( self.frame.bpTextCheckBox.IsChecked() and searchString in message ):
                activatePause = True
            logThreshold = self.frame.GetLogThreshold()
            if( self.frame.bpLevelCheckBox.IsChecked() and logThreshold <= logLevel ):
                activatePause = True
            if( activatePause ):
                self.frame.ActivatePause()
                self.frame.AppendMessage(message, maxLines)
                self.frame.SetTextColour( self.frame.BREAK_TEXT_COLOR )
            else:
                self.frame.AppendMessage(message, maxLines)

#
#    def SetTextColour(self, colour=(0,0,0)):
#        self.frame.SetTextColour( colour )
#

    def IsDebuggerPaused( self ):
        self.GetDebugApp()
        if( self.debugThread.isAlive() ):
#            self.frame = debugApp.GetTopWindow()
            return self.frame.IsDebuggerPaused()
        else:
            return False

    def GetExecuteCommand( self ):
        self.GetDebugApp()
        return self.frame.GetExecuteCommand();

    def logMessage(self, logLevel, message=''):
        self.AppendMessage( message, logLevel )

    def SetDebugModeInUse(self):
        self.GetDebugApp()
        self.frame.SetDebugModeInUse()

    def ResetDebugModeInUse(self):
        self.GetDebugApp()
        self.frame.ResetDebugModeInUse()

    def IsDebugModeInUse(self):
        self.GetDebugApp()
        return self.frame.IsDebugModeInUse()

    def logDebuggerMode(self):
        self.GetDebugApp()
        self.frame.logDebuggerMode()

    def EnableDebugExecuteButton(self):
        self.GetDebugApp()
        self.frame.EnableDebugExecuteButton()

    def DisableDebugExecuteButton(self):
        self.GetDebugApp()
        self.frame.DisableDebugExecuteButton()

    def GetAspiHeight(self):
        return self.frame.AspiHeight;
        
    def LoadDebbugerParameter(self):
        self.frame.ZAxis                = tesla.config.ZAxis
        self.frame.ZAxisPower           = tesla.config.ZAxisPower
        self.frame.ZAxisSpeedBegin      = tesla.config.ZAxisSpeedBegin
        self.frame.ZAxisSpeedSlope      = tesla.config.ZAxisSpeedSlope
        self.frame.ZAxisSpeedEnd        = tesla.config.ZAxisSpeedEnd
        self.frame.ZAxisSpeedHomeBegin  = tesla.config.ZAxisSpeedHomeBegin
        self.frame.ZAxisSpeedHomeSlope  = tesla.config.ZAxisSpeedHomeSlope
        self.frame.ZAxisSpeedHomeEnd    = tesla.config.ZAxisSpeedHomeEnd
        self.frame.ZAxisHalfStep        = tesla.config.ZAxisHalfStep
        self.frame.ZAxisBackOffHome     = tesla.config.ZAxisBackOffHome
        self.frame.ZAxisHome            = tesla.config.ZAxisHome
    
        self.frame.PumpAxis             = tesla.config.PumpAxis
        self.frame.PumpPower            = tesla.config.PumpPower
        self.frame.PumpSpeedBegin       = tesla.config.PumpSpeedBegin
        self.frame.PumpSpeedSlope       = tesla.config.PumpSpeedSlope
        self.frame.PumpSpeedEnd         = tesla.config.PumpSpeedEnd
        self.frame.PumpSpeedHomeBegin   = tesla.config.PumpSpeedHomeBegin
        self.frame.PumpSpeedHomeSlope   = tesla.config.PumpSpeedHomeSlope
        self.frame.PumpSpeedHomeEnd     = tesla.config.PumpSpeedHomeEnd
        self.frame.PumpHalfStep         = tesla.config.PumpHalfStep
        self.frame.PumpBackOffHome      = int(tesla.config.PumpBackOffHome)
        self.frame.PumpHome             = tesla.config.PumpHome
    
        self.frame.ThetaArm             = tesla.config.ThetaArm
        self.frame.ThetaArmPower        = tesla.config.ThetaArmPower
        self.frame.ThetaArmSpeedBegin   = tesla.config.ThetaArmSpeedBegin
        self.frame.ThetaArmSpeedSlope   = tesla.config.ThetaArmSpeedSlope
        self.frame.ThetaArmSpeedEnd     = tesla.config.ThetaArmSpeedEnd
        self.frame.ThetaArmSpeedHomeBegin = tesla.config.ThetaArmSpeedHomeBegin
        self.frame.ThetaArmSpeedHomeSlope = tesla.config.ThetaArmSpeedHomeSlope
        self.frame.ThetaArmSpeedHomeEnd   = tesla.config.ThetaArmSpeedHomeEnd
        self.frame.ThetaArmHalfStep     = tesla.config.ThetaArmHalfStep
        self.frame.ThetaArmBackOffHome  = int(tesla.config.ThetaArmBackOffHome)
        self.frame.ThetaArmHome         = tesla.config.ThetaArmHome
    
        self.frame.Carousel             = tesla.config.Carousel
        self.frame.CarouselPower        = tesla.config.CarouselPower
        self.frame.CarouselSpeedBegin   = tesla.config.CarouselSpeedBegin
        self.frame.CarouselSpeedSlope   = tesla.config.CarouselSpeedSlope
        self.frame.CarouselSpeedEnd     = tesla.config.CarouselSpeedEnd
        self.frame.CarouselSpeedHomeBegin = tesla.config.CarouselSpeedHomeBegin
        self.frame.CarouselSpeedHomeSlope = tesla.config.CarouselSpeedHomeSlope
        self.frame.CarouselSpeedHomeEnd = tesla.config.CarouselSpeedHomeEnd
        self.frame.CarouselHalfStep     = tesla.config.CarouselHalfStep
        self.frame.CarouselBackOffHome  = int(tesla.config.CarouselBackOffHome)
        self.frame.CarouselHome         = tesla.config.CarouselHome
        self.frame.CarouselHomeSwitch   = tesla.config.CarouselHomeSwitch
    
        self.frame.PowerOff             = tesla.config.PowerOff
        self.frame.ZPowerOff            = tesla.config.ZPowerOff

    def DumpDebbugerParameter(self): 
        return self.frame.DumpParameters();

def logMessages(message=''):
    import random

    value = random.randint(0,10)
    logLevel = DebuggerWindow.NOTSET

    if( message == '' ):
        if( value < 4 ):
            logLevel = tesla.PgmLog.PgmLog.DEBUG
            message = 'Dbg\tThis is a debug message\tMain.test1'
        elif( value < 6 ):
            logLevel = tesla.PgmLog.PgmLog.INFO
            message = 'Info\tThis is a information message\tMain.test6'
        elif( value < 7 ):
            logLevel = tesla.PgmLog.PgmLog.ID
            message = 'ID\tThis is a ID message\tMain.test7'
        elif( value < 8 ):
            logLevel = tesla.PgmLog.PgmLog.WARNING
            message = 'Warn\tThis is a warning message\tMain.test8'
        elif( value < 9 ):
            logLevel = tesla.PgmLog.PgmLog.ERROR
            message = 'Err\tThis is a error message\tMain.test9'
        else:
            logLevel = tesla.PgmLog.PgmLog.VERBOSE
            message = 'Vbs\tThis is a verbose message\tMain.test10'

    ssLogTracer = GetSSTracerInstance()
    ssLogTracer.logMessage( logLevel, message )


if (__name__ == '__main__'):
    ssLogTracer = SSDebugger()
    ssLogTracer.AppendMessage( '' )
    while( ssLogTracer.debugThread.isAlive() ):
        logMessages()
        time.sleep(.5)
    #debugApp.MainLoop()
    time.sleep(2)
    print 'finish'


