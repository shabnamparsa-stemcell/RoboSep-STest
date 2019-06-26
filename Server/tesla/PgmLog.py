import os.path
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
#
# -----------------------------------------------------------------------------
#!/usr/bin/env python

import os
import platform
import time
import datetime
import logging
import logging.handlers
import tesla.config
from tesla.exception import TeslaException
from configparser import *
from tesla.DebuggerWindow import *     # 2011-11-24 sp -- program logging


def getTimeStampPrefixString():
    ''' Generates unique timestamp prefix for naming log files
        it is unlikely that more than one file is generated within 60s of one another
        but if so, additional code will need to be implemented elsewhere '''
    timeStampString = time.strftime( "%Y%m%d_%H%M_" )
    return timeStampString

def getTimeStamp():
    ''' returns the date and time with millisecond precision '''
    currentTime = datetime.datetime.now()
    timestamp = currentTime.strftime( '%y-%m-%d_%H:%M:%S' ) + '.%03d' % (currentTime.microsecond//1000)
    return timestamp


# -----------------------------------------------------------------------------
# Handler for log messages
# extracts parameters from PgmLog class on initialization, makes a copy of it and 
# uses the local copy to setup and operate the handler
# When a log message with log-levels greater than or equal to 'flushLevel' is received,
# upto the 'capacity' number of recently stored messages with lower log-levels will also be logged
# Otherwise, only message with reportLevel are written to the log file
# When the number of lines in the log file exceeds maxFileLength, a new log file is used to continue log output.

class pgmLogMessageHandler(logging.Handler):

    
    def __init__(self, pgmLog):
        ''' extracts and creates a local copy of the settings in pgmLog '''
        logging.Handler.__init__(self)
        self.records = []
        self.capacity = pgmLog.capacity
        self.flushLevel = pgmLog.flushLevel
        self.reportLevel = pgmLog.reportLevel
        self.logLevel = pgmLog.logLevel
        self.index = 0
        self.sysLogID = pgmLog.sysLogID
        self.filePath = pgmLog.filePath
        self.fileSuffix = pgmLog.fileSuffix
        self.fileExtension = '.stlog'
        self.maxFileLength = pgmLog.maxFileLength
        self.fileCount = 0
        self.fileNameCount = 0
        # sets up and defines the base file handler to output messages
        self.logHandler = ''
        self.newFileHandler()


    def initLogFile(self, fileHandler):
        ''' initialize the file handler and outputs the header for naming the columns in the log file
            if this is a continuation from a previous log file, include a line that indicates
            which log file this current file is a continuation from '''
        self.logHandler = fileHandler
        self.logHandler.stream.write( 'TimeStamp\tSystem\tLevel\tDescription\tReference\n' )
        if( self.fileCount > 1 ):
            continueString = '%s\t%s\tInfo\t%s\t%s\n' % ( getTimeStamp(), self.sysLogID, 'logging continued from file, %s' % self.prevFile, __name__+'.initLogFile')
            self.logHandler.stream.write( continueString )
        # keep track of the previous fileName
        self.prevFile = self.fileName


    def closePreviousLog(self, fileName):
        ''' if a previous log file is opened, add a line to the current log file to indciate that 
            logging will continue in the new file called (fileName) and close this file '''
        if( self.fileCount > 1 ):
            continueString = '%s\t%s\tInfo\t%s\t%s\n' % ( getTimeStamp(), self.sysLogID, 'logging continued in file, %s' % fileName, __name__+'.closePreviousLog')
            self.logHandler.stream.write( continueString )
            self.logHandler.close()
        # keep track of the current fileName
        self.fileName = fileName


    def newFileHandler(self):
        ''' Gets a new base file handler to store the output messages.
            Resets the msgCounter for the number of messages/lines that are written to the file.
            Use the current time to serve as the prefix for the fileName and append it to the suffix.
            If the file already exist, (e.g. new file created after current file reached max. line count),
            a 2 digit number is appended to the file name to make it unique'''
        self.msgCounter = 1
        timeStamp = getTimeStampPrefixString()
        fileName = '%s\\%s%s%s' % ( self.filePath, timeStamp, self.fileSuffix, self.fileExtension  )
        self.fileCount = self.fileCount + 1
        if( os.path.exists( fileName ) ):
            self.fileNameCount = self.fileNameCount + 1
            fileName = '%s\\%s%s%02d%s' % ( self.filePath, timeStamp, self.fileSuffix, self.fileNameCount, self.fileExtension )
        else:
            self.fileNameCount = 0
        # if there is a previous log file from this current program run, close it before creating a new one
        self.closePreviousLog( fileName )
        # creates a new base file handler and if not successful, use the standard output stream
        try:
            fileMode = 'w'
            fileHandler = logging.FileHandler( filename=fileName, mode=fileMode )
            fileHandler.setLevel( self.logLevel )
            fileHandler.setFormatter( logging.Formatter( '%(message)s' ) )
            print(( "Created log file: %s." % fileName ))
        except IOError as msg:
            # Problem creating or opening the file?
            # set log level above critical such that no logging is performed
            # resets file counter
            self.fileCount = 0
            self.logLevel = PgmLog.NOLOG
            fileHandler = logging.StreamHandler()       # defaults handler to standard output
            print( '************************************' )
            print(( '*** PgmLogException: %s' % msg ))
            print( '*** No logging to specified file!' )
            print( '************************************' )
            print()
        # initializes the log file with header and other info
        self.initLogFile(fileHandler)


    def flush(self):
        ''' Custom handler for flushing the messages. '''
        self.logFlush()


    def emit(self, record):
        ''' Custom handler for handling messages recieved. 
            Only message at or higher than the defined logLevel reaches this point.'''
        if record.levelno >= self.flushLevel:
            # if the record is at or above the flushlevel, output and flush each of the items stored 
            # in the logged buffer, empty the buffer, and then output and flush the current record
            for rec in self.records:
                self.logHandler.emit(rec)
                self.logFlush()
            del( self.records[ 0 : self.index ] )
            self.index = 0
            self.logHandler.emit(record)
            self.logFlush()
        elif record.levelno >= self.reportLevel:
            # if the record is at or above the reportLevel, output and flush the record
            self.logHandler.emit(record)
            self.logFlush()
        else:
                # otherwise add message to history queue, and increment the index counter 
            self.records.append(record)
            if( self.index == self.capacity ):
                self.records.pop(0)
            else:
                self.index = self.index + 1


    def logFlush(self):
        ''' Custom flush utility. Calls the base handler to flush the output buffer.
            Increments the number of lines that have been written to the log file and
            creates a new file if the maxFileLength lines is exceeded. '''
        self.logHandler.flush()
        if( self.msgCounter >= self.maxFileLength ):
            self.newFileHandler()
        else:
            self.msgCounter = self.msgCounter + 1


# -----------------------------------------------------------------------------
# Class for creating initiating a log file

class PgmLog:
    NOLOG           = logging.CRITICAL + logging.CRITICAL
    CRITICAL        = logging.CRITICAL
    ERROR           = logging.ERROR
    WARNING         = logging.WARNING
    ID              = int( ( logging.INFO + logging.WARNING ) / 2 )
    INFO            = logging.INFO
    DEBUG           = logging.DEBUG
    VERBOSE         = int( ( logging.DEBUG + logging.NOTSET ) / 2 )
    NOTSET          = logging.NOTSET

    version         = '5.0'
    capacity        = 100
    maxFileLength   = 10000
    flushLevel      = ERROR
    reportLevel     = WARNING
    logLevel        = DEBUG
    printLogLevel   = NOLOG
    debuggerLogLevel = NOLOG

    def __init__(self, logName='' ):
        ''' resets the logName to null or retrieves the logger associated with the logName '''
        self.logger = ''
        self.logName = logName
        self.logPrefix = 'PGM'
        self.configLog = ''
        self.sysLogID = 'log'

        if( self.logName != '' ):
            self.logger = logging.getLogger( self.logName )
            if( self.logger != '' and hasattr(self.logger, 'sysLogID')):
                self.sysLogID = self.logger.sysLogID
        pass


    def create(self, logName='', filePath='', sysLogID='', capacity=-1, logLevel=-1, reportLevel=-1, flushLevel=-1, printLogLevel=-1, debuggerLogLevel=-1  ):
        ''' if logName (the name of the logger) is specified, creates a new logfile
            in the filePath directory with the fileSuffix appended to the fileName (timeStamp generated).
            sysLogID = name the logger is referenced by
            capacity = the number of lines to buffer recent messages
            logLevel = the messages to buffer
            flushLevel = the message level which causes recent buffered messages to be flushed.
            reportLevel = the messages to output to the file.
            printLogLevel = the messages that gets printed to stdout. '''
        if( logName != '' ):
            self.logName = logName
            self.setLoggingLevels( logLevel, reportLevel, flushLevel, printLogLevel, debuggerLogLevel )
            if( filePath != '' ):
                self.filePath = filePath
            if( sysLogID != '' ):
                self.sysLogID = sysLogID
            if( capacity >= 0 ):
                self.capacity = capacity
            self.fileSuffix = self.sysLogID

            logging.addLevelName( self.ID, 'ID' )
            logging.addLevelName( self.VERBOSE, 'VERBOSE' )

            # create custom handler that will handle the log messages and 
            # sets the level for what messages will be served by the handler
            pgmLogHandler = pgmLogMessageHandler( pgmLog=self )
            pgmLogHandler.setLevel(self.logLevel)

            # gets the logger and attached the custom handler to it 
            self.logger = logging.getLogger( self.logName )
            self.logger.addHandler( pgmLogHandler )
            self.setLoggerLevels( sysLogID=self.sysLogID, logLevel=self.logLevel, printLogLevel=self.printLogLevel, debuggerLogLevel=self.debuggerLogLevel )
            # logs any messages that are generated during the creation process
            if( self.configLog != '' ):
                self.logger.log( self.configLogID, self.configLog )
            self.logger.log( self.INFO, self.formatMessage( self.INFO, 'Info_S', self.logPrefix, __name__ + '.create',
                    'maxFileLength=%d |reportLevel=%d |flushLevel=%d |logLevel=%d |bufferCapacity=%d' % ( self.maxFileLength, 
                    self.reportLevel, self.flushLevel,self.logLevel, self.capacity) ) )


    def close(self ):
        ''' resets the logName to null or retrieves the logger associated with the logName '''
        if( self.logName != '' ):
            handlers = logging._handlers.copy()
            self.logger = logging.getLogger( self.logName )
            for hdlr in handlers:
                self.logger.removeHandler( hdlr )
                hdlr.close()


    def getLogConfigurations(self, configFile, sysLogID=''):
        ''' use defaults unless it can retrieve settings from configuration file '''
        if( sysLogID != '' ):
            self.sysLogID = sysLogID
        cfg = ConfigParser()
        # if configuration file exists, extract the settings
        if( os.path.exists( configFile)):
            cfg.read( configFile )
            try:
                # get settings from configuration file
                maxFileLength   = int( cfg.get( 'log_linecount', 'maxFileLength' ) )
                capacity        = int( cfg.get( 'log_linecount', 'bufferCapacity' ) )
                logLevel        = int( cfg.get( 'log_levels', 'bufferLevel' ) )
                flushLevel      = int( cfg.get( 'log_levels', 'flushLevel' ) )
                reportLevel     = int( cfg.get( 'log_levels', 'reportLevel' ) )
                printLogLevel   = int( cfg.get( 'log_levels', 'printLevel' ) )
                debuggerLogLevel = int( cfg.get( 'log_levels', 'debuggerLevel' ) )
                # set the self-variables for the class
                self.maxFileLength = maxFileLength
                self.capacity   = capacity
                self.setLoggingLevels( logLevel, reportLevel, flushLevel, printLogLevel, debuggerLogLevel )
                # generate log message of success, report file and version number
                self.configLogID = self.VERBOSE
                self.configLog = self.formatMessage( self.VERBOSE, 'Vbs', self.logPrefix, __name__ + '.getLogConfigurations',
                    'Using log configuration=%s | version=%s' % (configFile, cfg.get( 'log_properties', 'version' ) ) )
            except Exception as msg:
                self.configLogID = self.WARNING
                self.configLog = self.formatMessage( self.WARNING, 'Warn', self.logPrefix, __name__ + '.getLogConfigurations',
                    'Error reading from configuration file [%s]...Default settings used: %s' % (configFile, msg))
            print(( self.configLog ))
        else:
            self.configLogID = self.WARNING
            self.configLog = self.formatMessage( self.WARNING, 'Warn', self.logPrefix, __name__ + '.getLogConfigurations',
                'Error opening configuration file [%s]...Default settings used.' % configFile)


    def createPathIfNotExist(self, fileName):
        '''Private method: Ensure that the path for the specified filename
        exists. If not,        create it.'''
        path = os.path.dirname(fileName)
        if( path != '' ):
            status = os.path.exists(path)
            if not status:
                # Path doesn't exist? Create it
                os.makedirs(path)


    def formatMessage( self, msgLevel, logClass, deviceName, functionReference, description ):
        ''' Format the paramters into a tab-delimited string in preparation for saving.
            If msglevel is higher than or equal to the printLogLevel, the message is also printed on the standard output. '''
        formattedMessage = ( '%s\t%s\t%s\t%s\t%s|%s' % ( getTimeStamp(), self.sysLogID,
                    logClass, description, deviceName, functionReference ) )
        if( self.logger != '' ):
            if( hasattr(self.logger, 'printLogLevel') and self.logger.printLogLevel <= msgLevel ):
                print(( 'LOG: %s' % formattedMessage ))
            if( tesla.config.SS_DEBUGGER_LOG == 1 ):
                if( hasattr(self.logger, 'debuggerLogLevel') and self.logger.debuggerLogLevel <= msgLevel ):
                    ssLogDebugger = tesla.DebuggerWindow.GetSSTracerInstance()
                    ssLogDebugger.logMessage( msgLevel, formattedMessage)
        return( formattedMessage )


    def setLoggingLevels(self, logLevel=-1, reportLevel=-1, flushLevel=-1, printLogLevel=-1, debuggerLogLevel=-1 ):
        ''' sets the condition to enable or disable log message printing to standard output '''
        if( logLevel >= 0 ):
            self.logLevel = logLevel
        if( reportLevel >= 0 ):
            self.reportLevel = reportLevel
        if( flushLevel >= 0 ):
            self.flushLevel = flushLevel
        if( printLogLevel >= 0 ):
            self.printLogLevel = printLogLevel
        if( debuggerLogLevel >= 0 ):
            self.debuggerLogLevel = debuggerLogLevel

    def setLoggerLevels(self, sysLogID='log', logLevel=-1, printLogLevel=-1, debuggerLogLevel=-1 ):
        ''' sets the condition to enable or disable which messages are logged to a file
            and/or printed to standard output '''
        if( logLevel >= 0 ):
            self.logLevel = logLevel
        if( printLogLevel >= 0 ):
            self.printLogLevel = printLogLevel
        if( debuggerLogLevel >= 0 ):
            self.debuggerLogLevel = debuggerLogLevel
        if( self.logger != '' ):
            self.logger.debuggerLogLevel = self.debuggerLogLevel
            self.logger.printLogLevel = self.printLogLevel
            self.logger.setLevel( self.logLevel )
            self.logger.sysLogID = sysLogID


    def logSystemInfo( self ):
        '''Log some key information about the system.'''
        funcReference = __name__ + '.logSystemInfo'
        self.logPost( '', self.logPrefix, funcReference, 'Node=%s' % platform.node() )
        self.logPost( '', self.logPrefix, funcReference, "Server Version=%s" % tesla.config.SOFTWARE_VERSION )


    ''' Forwards the different message types to the logging utility. '''
    def logVerbose( self, logClass, deviceName, functionReference, description ):
        logClassID = 'Vbs'
        if( logClass != '' ):
            logClassID = logClassID + '_' + logClass
        message = self.formatMessage( self.VERBOSE, logClassID, deviceName, functionReference, description )
        self.logger.log( self.VERBOSE, message )

    def logDebug( self, logClass, deviceName, functionReference, description ):
        logClassID = 'Dbg'
        if( logClass != '' ):
            logClassID = logClassID + '_' + logClass
        message = self.formatMessage( self.DEBUG, logClassID, deviceName, functionReference, description )
        self.logger.debug( message )

    def logInfo( self, logClass, deviceName, functionReference, description ):
        logClassID = 'Info'
        if( logClass != '' ):
            logClassID = logClassID + '_' + logClass
        message = self.formatMessage( self.INFO, logClassID, deviceName, functionReference, description )
        self.logger.info( message )

    def logID( self, logClass, deviceName, functionReference, description ):
        logClassID = 'ID'
        if( logClass != '' ):
            logClassID = logClassID + '_' + logClass
        message = self.formatMessage( self.ID, logClassID, deviceName, functionReference, description )
        self.logger.log( self.ID, message )

    def logWarning( self, logClass, deviceName, functionReference, description ):
        logClassID = 'Warn'
        if( logClass != '' ):
            logClassID = logClassID + '_' + logClass
        message = self.formatMessage( self.WARNING, logClassID, deviceName, functionReference, description )
        self.logger.warning( message )

    def logError( self, logClass, deviceName, functionReference, description ):
        logClassID = 'Err'
        if( logClass != '' ):
            logClassID = logClassID + '_' + logClass
        message = self.formatMessage( self.ERROR, logClassID, deviceName, functionReference, description )
        self.logger.error( message )

    def logCritical( self, logClass, deviceName, functionReference, description ):
        logClassID = 'Critical'
        if( logClass != '' ):
            logClassID = logClassID + '_' + logClass
        message = self.formatMessage( self.CRITICAL, logClassID, deviceName, functionReference, description )
        self.logger.critical( message )

# -----------------------------------------------------------------------------
# test code

def dummyLog():
    ''' recalls log by its name and start logging events '''
    import random
    svrLog = PgmLog( 'application', 'svr' )
    extLog = PgmLog( 'abc', 'ext' )
    for rep in range( 0, 5 ):
        offset = 100 * rep
        # generate random number of messages before next log event
        for i in range(offset, offset + random.randrange( 0, 100 ) ):
            svrLog.logDebug('_msg', 'app_svr', 'main.pgmlog', 'Logging low level events ' + str(i))
            extLog.logDebug('', 'app_ext', 'main.pgmlog', 'Logging ext low level events ' + str(i))
        if( rep == 3 ):
            extLog.setLoggerLevels(svrLog.INFO, svrLog.VERBOSE)
        svrLog.logError('_next', 'app_svr', 'tesla.pgmlog', 'Something went wrong in cycle: %d' % rep )
        extLog.logInfo('', 'msg_ext', 'main.pgmlog', 'Start logging previous events for cycle: %d' % rep )


def main():
    ''' sets up logging facilities '''
    svrLog = PgmLog( 'application', 'svr' )
    svrLog.getLogConfigurations('loggingConfigTest.ini')
    svrLog.create( 'application', '.\\', 'svr.stlog', 'svr', 10, PgmLog.DEBUG, PgmLog.WARNING, PgmLog.ERROR )
    
    extLog = PgmLog( 'abc', 'ext' )
    extLog.getLogConfigurations('loggingConfigTest.ini')
    extLog.create( 'abc', '.\\', 'ext.stlog', 'ext', 10, PgmLog.DEBUG, PgmLog.INFO, PgmLog.ERROR )

    dummyLog()
    pass


if __name__ == '__main__':
    main()
