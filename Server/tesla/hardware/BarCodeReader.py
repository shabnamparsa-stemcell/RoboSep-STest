#----------------------------------------------------------------------------
# BarCodeReader.py
#
# The copyright to the computer program(s) herein is the property of
# StemCell Technologies.
#
# The program(s) may be used and/or copied only with the written permission
# of StemCell Technologies or in accordance with the terms and conditions
# stipulated in the agreement/contract under which the program(s)
# have been supplied.
#
# Copyright (c) 2011. All Rights Reserved.
#
#==============================================================================
# Revisions:
#
#   2011-08-22  SP  Code extracted from RoboSepV46Xroot (SourceSafe)
#   2011-08-22  SP  Modified to work with Python 3.0+ compatibility and standalone apps
#                       - explicit definition of byte encoded strings
#                       - remove dependency of ipl.utils.wait pre-compiled code
#
#----------------------------------------------------------------------------


import serial
import SimpleStep.SerialComms

from tesla.PgmLog import PgmLog    # 2011-11-23 sp -- programming logging

from ipl.utils.wait import wait_msecs

class BarCodeReader:
    __module__ = __name__
    __doc__ = 'Interface for the barcode reader\n'

    emulator_response = "EMULATOR_DEFAULT"
    error_response = "DEVICE_IO_ERROR"
    noneDetected_response = "NONE_DETECTED"
    numRetries = 2
    pollDuration = 1000

    def __init__(self, port = 'COM253', baudrate = 115200, pollDuration=-1, numRetries=-1, triggerCmd = "", updateCmd = "", initCmds = "" , preCmd = "" , useEmulator = False):
        """Construct an instance with the serial port and baudrate"""

        self.port = port
        self.baudrate= baudrate
        self.setPollDuration(pollDuration)
        self.setNumRetries(numRetries)
        self.useEmulator = useEmulator
        self.comPortActive = False
        self.bcReader = None
        self.logPrefix = 'BC'

        self.triggerCmd = triggerCmd;
        self.updateCmd  = updateCmd;
        self.initCmds   = initCmds;
        self.preCmd = preCmd;
        
        funcReference = __name__ + '.__init__'
        self.svrLog = PgmLog( 'svrLog' )
        self.svrLog.logVerbose('', self.logPrefix, funcReference, 'starting' )
        if( not useEmulator ):
            try:
                self.bcReader = SimpleStep.SerialComms.SerialComms( self.port, self.baudrate )
                barCode = self.bcReader.sendAndCheck( self.updateCmd, self.pollDuration )
                if ( barCode == None ) or ( barCode == 'NR' ):
                    self.comPortActive = True
                    self.svrLog.logInfo('', self.logPrefix, funcReference, "completed init; Port=%s" % self.port )
                    if (self.initCmds[0].__len__()!=0) :
                        self.bcReader.sendAndCheck( self.initCmds[0], self.pollDuration );
                    if (self.initCmds[1].__len__()!=0) :
                        self.bcReader.sendAndCheck( self.initCmds[1], self.pollDuration );
                    if (self.initCmds[2].__len__()!=0) :
                        self.bcReader.sendAndCheck( self.initCmds[2], self.pollDuration );
                    if (self.initCmds[3].__len__()!=0) :
                        self.bcReader.sendAndCheck( self.initCmds[3], self.pollDuration );
                    if (self.initCmds[4].__len__()!=0) :
                        self.bcReader.sendAndCheck( self.initCmds[4], self.pollDuration );                                                                                                
                else:
                    self.svrLog.logError('', self.logPrefix, funcReference, "Error opening; Port=%s, Code=%s" % ( self.port, barCode) )
                                        
            except serial.serialutil.SerialException as args:
                self.bcReader = None
                self.comPortActive = False
                if (str(args).find('Access is denied') >= 0):
                    errorMsg = ("Can't access port %s (%s). Locked by another resource?" % (port,args))
                else:
                    errorMsg = args
                self.svrLog.logError('', self.logPrefix, funcReference, "Error opening; Port=%s, Msg=%s" %
                        (self.port, errorMsg ) )
        else:
            self.comPortActive = True
                    

    def close(self):
        if( self.bcReader != None ):
            self.bcReader.close()
        self.bcReader = None
        self.comPortActive = False

    def AttachBarcodeReader(self):
        if (not self.useEmulator):
            try:
              self.bcReader.open();
            except:
              pass;
        print('>> Barcode Reader: Attached done!');            
    
    def DetachBarcodeReader(self):
        if (not self.useEmulator):
            try:
              self.bcReader.close();
            except:
              pass;  
        print('>> Barcode Reader: Dettached done!');
        
    def setPollDuration(self, value):
        """sets a new time in milliseconds for the pollDuration if the value is greater than zero"""
        if( value > 0 ):
            self.pollDuration = value


    def setNumRetries(self, value):
        """sets the number of retries to value if the value is greater than zero"""
        if( value > 0 ):
            self.numRetries = value

    def getBarCode(self):
        """Send data and then check for a response"""
        if( self.useEmulator ):
            barcode = self.emulator_response;
        elif( self.comPortActive and self.bcReader != None ):
            try:
                retry = 0
                barcode = 'NR';                
                print(('\n----- Enter getBarCode ------ %d\n'%self.numRetries));
                
                if(self.preCmd != ""):
                    preCmdResp = None;
                    while(preCmdResp == None):
                        self.bcReader.sendAndCheck( self.initCmds[0], self.pollDuration );
                        #preCmdResp = self.bcReader.sendAndCheck(self.preCmd, self.pollDuration);
                        preCmdResp = self.bcReader.sendAndCheckWithout(self.preCmd, self.pollDuration);
                        print(('>>> Set BCR Pre Command %s  %d %s!!!\n'%(self.preCmd,self.pollDuration,preCmdResp) ));   
                    
                while (retry < self.numRetries) :
                      barcode = 'NR';
                      print(('>>> Set BCR Trigger Command %s  %d!!!\n'%(self.triggerCmd,self.pollDuration) ));                      
                      barcode = self.bcReader.sendAndCheck(self.triggerCmd, self.pollDuration)
                      if (barcode != 'NR'): 
                         break;
                      elif (barcode == None):
                         self.bcReader.sendAndCheck(self.updateCmd, self.pollDuration)
                         print(('<<< Set BCR Update Command %s  %d!!!\n'%(self.updateCmd,self.pollDuration) ));
                         retry += 1;                      
                      else:
                         self.bcReader.sendAndCheck(self.updateCmd, self.pollDuration)
                         print(('<<< Set BCR Update Command %s  %d!!!\n'%(self.updateCmd,self.pollDuration) ));
                         retry += 1;

            except serial.serialutil.SerialException as errorMsg:
                print( errorMsg )
                self.comPortActive = False
                self.bcReader = None
                barcode = self.error_response
        else:
            barcode = self.error_response
        print(('----- Leave getBarCode %s------\n'%barcode));            
        return barcode


    def wait_msecs(self, value):
        if( not self.useEmulator ):
            self.bcReader.wait_msecs(value)


if (__name__ == '__main__'):
    initCmds = [" \x16M" , "DECWIN1;DECLFT0;DECRGT42;DECBOT64;DECTOP20;SHWNRD1;TRGSTO500." , "\x16M" , "SHWNRD1;TRGSTO500." , ""]
    bcr = BarCodeReader(pollDuration=200, numRetries=-1, useEmulator = False, triggerCmd= "\x16T", updateCmd="\x16U", initCmds=initCmds, preCmd="\x16M\rIMGSUP." )
    bcr.wait_msecs(1000)                    # time to wait for the initialization to complete before sending read commands
    import time
    startTime = time.time()
    for i in range(1, 10):
        barcode = bcr.getBarCode()
        print(( i, barcode )) # trigger: poll scanner
    endTime = time.time()
    print(( "\nTime taken = %f" % (endTime - startTime) ))


