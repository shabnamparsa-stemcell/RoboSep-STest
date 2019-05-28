# 
# RoboSepLauncher.py
# Script to launch the instrument control server and GUI operator console
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
# the written permission of Invetech Operations Pty Ltd
# or in accordance with the terms and conditions
# stipulated in the agreement/contract under which
# the program(s) have been supplied.
#

import threading
import sys
import time

from robosep import startRoboSepServer


import traceback, string
from _winreg import *
import os, sys, win32gui, win32con, win32api, glob    
# -----------------------------------------------------------------------------
# Configurables

TASK_PRIORITY = 'HIGH'          # The process priority (eg. NORMAL, HIGH, REALTIME)

PYTHON_PATH    = 'c:\\python23\\python.exe'
#TESLA_ARCHIVE = 'c:\\program files\\sti\\robosep\\bin\\robosep.zip'
TESLA_ARCHIVE = 'tesla'
sys.path.append(TESLA_ARCHIVE)

import tesla

BIN_DIR64        = 'C:\\program files (x86)\\sti\\robosep\\bin'   #CWJ ADD
BIN_DIR32        = 'C:\\program files\\sti\\robosep\\bin'         #CWJ ADD
BIN_DIR          = BIN_DIR32

SERVER_PATH      = 'robosep.py'
SEPARATOR_PATH   = 'Separator.exe'

SEPARATOR_CONFIG = 'Separator.exe.config'

SLEEP_TIME_SECS  = 1                # Time to sleep between server checks
TIMEOUT_LIMIT    = 20                # Timeout loop max value Original: 100

EXIT_SUCCESS = 0                # Successful exit code
EXIT_FAILURE = -1                # Exit code on failure


# -----------------------------------------------------------------------------
# Rest of our module imports

from xmlrpclib import ServerProxy
import time, socket
import types

import tesla.config

import re

from xml.dom.ext.reader import Sax2
import xml.dom.ext
import xml.dom.minidom
    
# -----------------------------------------------------------------------------

server = None


def testForApplications():
    '''Ensure that each of our applications exists'''

    # Test for the existence of each application
    for app in (SERVER_PATH, SEPARATOR_PATH):
        appPath = os.path.join(BIN_DIR, app)
        if not os.path.exists(appPath):
            consolePrint("Application installation error: can not find %s." % (appPath))
            sys.exit(EXIT_FAILURE)

def consolePrint(msg):
    '''Print a message to the console if the TESLA_QUIET_LAUNCH environment 
    variable is not set'''
    if not os.environ.has_key('QUIET_LAUNCH'):
        print msg

def serverIsRunning(serverURL):
    '''Returns True if the server is running'''
    global server
    runFlag = False
    try:
        if type(server) == types.NoneType:
            server = ServerProxy(serverURL)
        runFlag = server.ping()
    except socket.error:
        # This exception is thrown if the server is not up
        pass
    return runFlag

def getServerState():
    '''Returns the state of the instrument control server'''
    if server == None:
        raise ValueError, "Undefined server (not running yet?)"
    else:
        return server.getInstrumentState()

def startServer(serverURL):
    '''Start the server (as a separate process)'''
    consolePrint('Starting server...')
    #os.chdir(BIN_DIR)
    #print "start /b /%s %s %s" % (TASK_PRIORITY, PYTHON_PATH, SERVER_PATH)
    #status = os.system("start /b /%s %s %s" % (TASK_PRIORITY, PYTHON_PATH, SERVER_PATH))
    #successFlag = (status == 0)
    thrd = threading.Thread(target=startRoboSepServer, args=())
    thrd.start()
    time.sleep(1)
    successFlag = thrd.isAlive()
    if not successFlag:
        # indicate if the server launched ok or not
        consolePrint( 'Server could not be launched. Error number is xx. Exiting...' )        
        RoboTrace.RoboSepMessageBox('Failed to create Thread startRoboSepServer. Exiting...')        
        # sys.exit(EXIT_FAILURE)
        thrd = None;
    else:
        consolePrint( '\nServer successfully launched. Now checking for server response...' )
        
        # If we started the server, wait for it's XML-RPC interface to come up
        for attempt in range(TIMEOUT_LIMIT):
            if serverIsRunning(serverURL):
                consolePrint('\nServer is responding.')
                break
            elif serverIsRunning("http://localhost:8000"): 
                consolePrint('\nServer is responding to localhost.')
                break
            else:  
                time.sleep(SLEEP_TIME_SECS)
                consolePrint("\tWaiting %d seconds" % (attempt + 1))
        else:
            consolePrint( 'Server not responding to requests. Exiting...' )
            RoboTrace.RoboSepMessageBox('IC Server not responding to requests. Exiting...')        
            # sys.exit(EXIT_FAILURE)            
            thrd = None;
    return thrd
   
def startOperatorConsole():
    '''Now launch the separator (replacing this process); the separator is then
    responsible for launching the operator console.'''
    consolePrint('Starting user application')
    os.chdir(BIN_DIR)
    if os.environ.has_key('ROBO_MANUAL_CONSOLE'):
        return
    try:
        execCode = win32api.WinExec(SEPARATOR_PATH)
    except OSError, msg:
        consolePrint("Error launching app: %s (%s)" % (msg, execCode))
        RoboTrace.RoboSepMessageBox("Error launching app: %s (%s)" % (msg, execCode))


def readIpConfig():
    '''Returns the output of the system call ipconfig '''
    ipconfigFile = os.popen( "ipconfig", "r" )
    ipconfig = ipconfigFile.readlines()
    ipconfigFile.close()
    return ipconfig


# 2012-03-26 CWJ -- refreshHostAddress routine replaced to address network access issues
def refreshHostAddress():

    while (1):
        ipconfig = readIpConfig()
        for line in ipconfig:
            m = re.compile( "IP Address.+?(\d+\.\d+\.\d+\.\d+)" ).search( line )
            if m == None:
               m = re.compile( "IPv4 Address.+?(\d+\.\d+\.\d+\.\d+)" ).search( line )

            if m:
                hostIp = "127.0.0.1"   # m.group(1)     #Just using localhost Nov 29,2013
                tesla.config.GATEWAY_HOST = hostIp;
                break
        else:
            print "Network disconnected. Using localhost (127.0.0.1) for instrument."
            hostIp = "127.0.0.1"
            tesla.config.GATEWAY_HOST = hostIp;

        # detect unstable IP address and try again after pause if unstable
        if hostIp == "0.0.0.0":
            print "Unstable IP adress detected. Waiting 5 seconds and trying again..."
            time.sleep(5)
        else:
            break


    # print "Host IP is", hostIp

    # Read Separator.exe.config
    try:
        separatorConfigFile = open( BIN_DIR + "\\" + SEPARATOR_CONFIG )
        reader = Sax2.Reader()
        doc = reader.fromStream( separatorConfigFile )
    except IOError:
        separatorConfigFile = open( BIN_DIR + "\\" + SEPARATOR_CONFIG + ".bak" )
        reader = Sax2.Reader()
        doc = reader.fromStream( separatorConfigFile )
    except:
        separatorConfigFile.close()
        separatorConfigFile = open( BIN_DIR + "\\" + SEPARATOR_CONFIG + ".bak" )
        reader = Sax2.Reader()
        doc = reader.fromStream( separatorConfigFile )
    separatorConfigFile.close()

    #dom = xml.dom.minidom.getDOMImplementation()
    #doc = dom.createDocument(None,'configuration',None)

    try:
        # adjust host IP
        separatorNode = doc.getElementsByTagName('Separator')

        iccChildNodes = separatorNode[0].getElementsByTagName('InstrumentControlConnection')[0].childNodes

        for node in iccChildNodes:
            attributes = node.attributes
            if attributes:
                if attributes[(None,u'key')].nodeValue == 'ServerAddress':
                    oldIp = attributes[(None,u'value')].nodeValue
                    print "Replacing old host IP", oldIp, "with new host IP", hostIp, "in Separator.exe.config"
                    attributes[(None,u'value')].nodeValue = hostIp
    except:
        separatorNode = doc.createElement('Separator')
        instrumentConnectionNode = doc.createElement('InstrumentControlConnection')
        addNode = doc.createElement('add')
        addNode.setAttribute('value', hostIp)
        addNode.setAttribute('key','ServerAddress')

        instrumentConnectionNode.appendChild(addNode)
        separatorNode.appendChild(instrumentConnectionNode)
        doc.documentElement.appendChild(separatorNode)


    # Write Separator.exe.config
    try:
        print "writing Separator.exe.config"
        separatorConfigFile = open( BIN_DIR + "\\" + SEPARATOR_CONFIG, "w" )
        xml.dom.ext.PrettyPrint( doc, separatorConfigFile )
        separatorConfigFile.close()
    except:
        RoboTrace.RoboSepMessageBox('Cannot Access Separator.exe.config! Terminating...');
        sys.exit(EXIT_FAILURE)

#def refreshHostAddress():
#
#    while (1):
#        ipconfig = readIpConfig()
#        for line in ipconfig:
#            m = re.compile( "IP Address.+?(\d+\.\d+\.\d+\.\d+)" ).search( line )
#            if m == None:
#               m = re.compile( "IPv4 Address.+?(\d+\.\d+\.\d+\.\d+)" ).search( line )
#
#            if m:
#                hostIp = m.group(1)
#                break
#        else:
#            print "Network disconnected. Using localhost (127.0.0.1) for instrument."
#            hostIp = "127.0.0.1"
#
#        # detect unstable IP address and try again after pause if unstable
#        if hostIp == "0.0.0.0":
#            print "Unstable IP adress detected. Waiting 5 seconds and trying again..."
#            time.sleep(5)
#        else:
#            break
#
#
#    # print "Host IP is", hostIp
#
#    # Read Separator.exe.config
#    try:
#        separatorConfigFile = open( BIN_DIR + "\\" + SEPARATOR_CONFIG )
#        reader = Sax2.Reader()
#        doc = reader.fromStream( separatorConfigFile )
#    except IOError:
#        separatorConfigFile = open( BIN_DIR + "\\" + SEPARATOR_CONFIG + ".bak" )
#        reader = Sax2.Reader()
#        doc = reader.fromStream( separatorConfigFile )
#    except:
#        separatorConfigFile.close()
#        separatorConfigFile = open( BIN_DIR + "\\" + SEPARATOR_CONFIG + ".bak" )
#        reader = Sax2.Reader()
#        doc = reader.fromStream( separatorConfigFile )
#    separatorConfigFile.close()
#
#    #dom = xml.dom.minidom.getDOMImplementation()
#    #doc = dom.createDocument(None,'configuration',None)
#
#    try:
#        # adjust host IP
#        separatorNode = doc.getElementsByTagName('Separator')
#
#        iccChildNodes = separatorNode[0].getElementsByTagName('InstrumentControlConnection')[0].childNodes
#
#        for node in iccChildNodes:
#            attributes = node.attributes
#            if attributes:
#                if attributes[(None,u'key')].nodeValue == 'ServerAddress':
#                    oldIp = attributes[(None,u'value')].nodeValue
#                    print "Replacing old host IP", oldIp, "with new host IP", hostIp, "in Separator.exe.config"
#                    attributes[(None,u'value')].nodeValue = hostIp
#    except:
#        separatorNode = doc.createElement('Separator')
#        instrumentConnectionNode = doc.createElement('InstrumentControlConnection')
#        addNode = doc.createElement('add')
#        addNode.setAttribute('value', hostIp)
#        addNode.setAttribute('key','ServerAddress')
#
#        instrumentConnectionNode.appendChild(addNode)
#        separatorNode.appendChild(instrumentConnectionNode)
#        doc.documentElement.appendChild(separatorNode)
#
#
#    # Write Separator.exe.config
#    try:
#        print "writing Separator.exe.config"
#        separatorConfigFile = open( BIN_DIR + "\\" + SEPARATOR_CONFIG, "w" )
#        xml.dom.ext.PrettyPrint( doc, separatorConfigFile )
#        separatorConfigFile.close()
#    except:
#        RoboTrace.RoboSepMessageBox('Cannot Access Separator.exe.config! Terminating...');
#        sys.exit(EXIT_FAILURE)



# 2011-11-29 sp
# add utility for file compression
import zipfile
def compressLogFiles( zipFileName, filesToCompress, timeStarted, timeCompleted ):
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
    
    print ( '\nCreating compressed file ' + zipFileName )
    zFile = zipfile.ZipFile( zipFileName, "w" )
    for name in filesToCompress:
        zFile.write(name, os.path.basename(name), zipfile.ZIP_DEFLATED)
    zFile.close()
    zFile = zipfile.ZipFile(zipFileName, "r")
    zipFileError = zFile.testzip()
    if( zipFileError == None ):
        totalFileSize = 0
        totalCompressSize = 0
        for info in zFile.infolist():
##            print( info.filename, info.file_size, info.compress_size )
            totalFileSize += info.file_size
            totalCompressSize += info.compress_size
        print( zipFileName, "contains", len(zFile.namelist()), "files;", totalFileSize, "bytes; compressed to", int(totalCompressSize*100/totalFileSize ), "%" )
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
    else:
        print( "Errors encountered in compression; no log file cleanup was performed:", zipFileError )


# -----------------------------------------------------------------------------
# The main script


from tesla.PgmLog import PgmLog     # 2011-11-24 sp -- program logging

if __name__ == '__main__':
    
    thrd = None

    if os.environ.has_key('ProgramFiles(x86)'):    
       BIN_DIR = BIN_DIR64
       tesla.config.BASE_DIR = tesla.config.BASE_DIR64
    else:
       BIN_DIR = BIN_DIR32
       tesla.config.BASE_DIR = tesla.config.BASE_DIR32
    
    tesla.config.UpdatePathes()
    tesla.config.GetConfigEnvironment()

    import SimpleStep.SimpleStep  #CWJ Add
    import shutil                 #CWJ Add
    import RoboTrace              #CWJ Add
    
    import fnmatch
    
    SplashMgr = RoboTrace.GetRoboSplashMgrInstance();
    os.chdir(BIN_DIR)
    if not os.environ.has_key('ROBO_MANUAL_CONSOLE'):
        SplashMgr.StartSplash();

    # 2011-11-24    sp -- program logging setup
    timeStarted = tesla.PgmLog.getTimeStampPrefixString()
    tesla.config.TIME_START = timeStarted;
    
    svrLog = PgmLog( )
    svrLog.logPrefix = 'RL'     # prefix for RobosepLauncher module
    serverLogID = 'svr'
    svrLog.getLogConfigurations( tesla.config.SERVER_CONFIG_PATH, serverLogID )
    # 2012-01-30 sp -- replace environment variable with configuration variable
    #if( os.environ.has_key('SS_DEBUG') ):
    if( tesla.config.SS_DEBUG == 1 ):
        # configure to display all messages, overwrite parameter file settings
        svrLog.setLoggingLevels(logLevel=svrLog.VERBOSE, reportLevel=svrLog.VERBOSE,
                flushLevel=svrLog.VERBOSE, printLogLevel=svrLog.VERBOSE, debuggerLogLevel=svrLog.VERBOSE )
    # start logging facility
    svrLog.create( 'svrLog', tesla.config.LOG_DIR, serverLogID )
    svrLog.logInfo('', svrLog.logPrefix, 'GetConfigEnvironment', tesla.config.configEnvironmentMsg )
    # 2013-01-22 -- sp  added logging of environment variables
    svrLog.logDebug('', svrLog.logPrefix, 'GetConfigEnvironment', tesla.config.configEnvironmentSettingMsg )
    

    """from tesla.hardware.PagerSystem import PagerSystem
    
    pager = PagerSystem()
    print pager.getInfo()
    pager.setEcho(False)
    pager.setRePage(5)

    import time
    import datetime
    print pager.setTime(time.localtime()[3],time.localtime()[4])
    
    print pager.page(99,0,'RoboSep is alive')
    print pager.page(99,0,'RoboSep is alive')
    print pager.page(99,0,'RoboSep is alive1')
    print pager.page(99,0,'RoboSep is alive1')
    print pager.page(99,0,'RoboSep is alive2')
    print pager.page(99,0,'RoboSep is alive')
    print pager.page(99,0,'RoboSep is alive')
    print pager.page(99,0,'RoboSep is alive4')
    print pager.page(99,0,'RoboSep is alive')
    time.sleep(3)
    sys.exit(1)"""


    """"from tesla.instrument.Instrument import Instrument
    from tesla.instrument.Calibration import Calibration

    instrument = Instrument("Instrument")
    c = Calibration("Calibration", self.instrument)

    print 'start'
    c.calibrateSetSample1
    c.calibrateSetSample2
    c.calibrateCalcTipsAndVials
    sys.exit(1)
    print 'end'"""

    path = r'Environment'
    reg = ConnectRegistry(None, HKEY_CURRENT_USER)
    key = OpenKey(reg, path, 0, KEY_ALL_ACCESS)
    SetValueEx(key, "TESLA_FULL_SHUTDOWN", 0, REG_SZ, "1")
    
    refreshHostAddress()
    
    # 2011-11-24    sp -- program logging
    svrLog.logID( '', svrLog.logPrefix, 'RoboSepLauncher.main', 'Host=%s |Server V%s'
                    % ( tesla.config.GATEWAY_HOST, tesla.config.SOFTWARE_VERSION ) )
    
    print '\n\n ### BIN_DIR : %s'%BIN_DIR
    print '\n ### PROTOCOL_DIR : %s'%(tesla.config.PROTOCOL_DIR)
    
    try:                                                            # CWJ Add
      FullSrcName = '%s\\home_axes.xml'%BIN_DIR                     # CWJ Add
      FullDstName = '%s\\home_axes.xml'%tesla.config.PROTOCOL_DIR   # CWJ Add 
      shutil.copyfile(FullSrcName, FullDstName)                     # CWJ Add    
      FullSrcName = '%s\\prime.xml'%BIN_DIR                         # CWJ Add
      FullDstName = '%s\\prime.xml'%tesla.config.PROTOCOL_DIR       # CWJ Add
      shutil.copyfile(FullSrcName, FullDstName)                     # CWJ Add
      FullSrcName = '%s\\shutdown.xml'%BIN_DIR                      # CWJ Add
      FullDstName = '%s\\shutdown.xml'%tesla.config.PROTOCOL_DIR    # CWJ Add
      shutil.copyfile(FullSrcName, FullDstName)                     # CWJ Add
    except IOError:                                                 # CWJ Add  
      print 'Cannot copy essentail protcols!!!\n\a'                 # CWJ Add
      # 2011-11-24    sp -- program logging setup
      svrLog.logError( '', svrLog.logPrefix, 'RoboSepLauncher.main', 'Cannot copy essential protcols!' )

        
    #testForApplications()

    hostURL = "http://%s:%d" % (tesla.config.GATEWAY_HOST, tesla.config.GATEWAY_PORT)
    
    if not serverIsRunning(hostURL):
       # If the server isn't running, try to start it
       thrd = startServer(hostURL)
       if (thrd == None):
          svrLog.logError( '', svrLog.logPrefix, 'RoboSepLauncher.main', 'Server not running at %s!' % (hostURL) )
          svrLog.logID( '', svrLog.logPrefix, 'RoboSepLauncher.main', 'Closing log' )
          svrLog.close()
          timeCompleted = tesla.PgmLog.getTimeStampPrefixString()
          filesToCompress = [ tesla.config.LOG_PATH ]
          zipFileName = os.path.join( tesla.config.LOG_DIR, '%slog.zip' % timeStarted )
          compressLogFiles( zipFileName, filesToCompress, timeStarted, timeCompleted )
          sys.exit(EXIT_FAILURE)            
    else:    
        consolePrint("Server is already running")
        # 2011-11-24    sp -- program logging setup
        svrLog.logError( '', svrLog.logPrefix, 'RoboSepLauncher.main', 'Server is already running!' )

    CamMgr = RoboTrace.GetRoboCamMgrInstance();
    CamMgr.ExcuteRoboCam();
      
    if serverIsRunning(hostURL):
        # So is the server now running? If so, fire up the GUI client
        startOperatorConsole()
        pass
    else:
        consolePrint("Error: server not running at %s!" % (hostURL))
        svrLog.logError( '', svrLog.logPrefix, 'RoboSepLauncher.main', 'Server not running at %s!' % (hostURL) )
        svrLog.logID( '', svrLog.logPrefix, 'RoboSepLauncher.main', 'Closing log' )
        svrLog.close()
        CamMgr = RoboTrace.GetRoboCamMgrInstance();
        CamMgr.TerminateRoboCam();
        RoboTrace.RoboSepMessageBox("Error: server not running at %s!" % (hostURL))        

        timeCompleted = tesla.PgmLog.getTimeStampPrefixString()
        filesToCompress = [ tesla.config.LOG_PATH ]
        zipFileName = os.path.join( tesla.config.LOG_DIR, '%slog.zip' % timeStarted )
        compressLogFiles( zipFileName, filesToCompress, timeStarted, timeCompleted )

        sys.exit(EXIT_FAILURE)

    # 2012-01-30 sp -- replace environment variable with configuration variable
    #if os.environ.has_key('SS_TRACE'):
    if( tesla.config.SS_TRACE == 1 ):
      trace = RoboTrace.GetRoboTracerInstance()          #CWJ Add
      trace.SetObjfunc(thrd)                             #CWJ Add
      trace.MainLoop()                                   #CWJ Add

    if thrd != None:
        while thrd.isAlive():
            time.sleep(1)
    
    CamMgr = RoboTrace.GetRoboCamMgrInstance();
    CamMgr.TerminateRoboCam();

    SplashMgr = RoboTrace.GetRoboSplashMgrInstance();
    SplashMgr.TerminateSplash();

    if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
        Dmp = SimpleStep.SimpleStep.GetSSExtLoggerInstance()  #CWJ Add
        if tesla.config.FULL_SHUTDOWN_IN_PROGRESS == False:
           Dmp.Dump()                                         #CWJ Add
           Dmp.DumpBackUpHistory()                            #CWJ Add

    # 2011-11-24 -- sp
    svrLog.logID( '', svrLog.logPrefix, 'RoboSepLauncher.main', 'Closing log' )
    svrLog.close()

    # 2011-11-29 sp -- add compression of log files
    timeCompleted = tesla.PgmLog.getTimeStampPrefixString()

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
    
    print filesToCompress
    
    if tesla.config.FULL_SHUTDOWN_IN_PROGRESS == False:
        compressLogFiles( zipFileName, filesToCompress, timeStarted, timeCompleted )
# eof

