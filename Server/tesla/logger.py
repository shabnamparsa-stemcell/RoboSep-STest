# 
# logger.py
# tesla.logger
#
# Event & error logging for the Tesla instrument controller
# 
# Copyright (c) Invetech Pty Ltd, 2003
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


import os, platform 
import logging
import logging.handlers

from ipl.utils.borg import Borg
import tesla.config
from tesla.exception import TeslaException

import RoboTrace                                                        #CWJ Add

# -----------------------------------------------------------------------------

# Configuration variables for the error/event logger that should not need to 
# change
# If you find that they are, please move them across to tesla.config

LOG_ID        = 'ROBOSEP'              # Name of the log object
ROLLOVER_SIZE = 104857600L            # At 100MB, we roll the file over
MAX_BACKUPS   = 9999                        # This is a lot of log files :)
MAX_BUFFERED_ENTRIES = 1000            # How many entries do we buffer in memory?

# -----------------------------------------------------------------------------

class LoggerException(TeslaException):
    """Exception thrown when there is a logger error"""
    print "LoggerException ", str(TeslaException) 
    pass

# -----------------------------------------------------------------------------

class Logger(Borg):
    """Create a shared event, warning & error logger object"""
    
    def __init__(self, logName, logLevel = tesla.config.LOG_LEVEL, otherHandlers = []):
        """Initialise (or just copy the existing instance state & behaviour"""
        Borg.__init__(self)
        try:
            self.logger
        except AttributeError:
            self.__verifyLogPath(logName)
            self.pathName = logName
            self.logger = logging.getLogger(LOG_ID)
            self.__logBuffer = []
            self.handlers = []

            # Set up our default logging formatter object
            self.formatter = logging.Formatter('%(asctime)s %(levelname)s %(message)s')

            # Create our rotating log file handler (max backups = 9999)
            try:
                logFileHandler = logging.handlers.RotatingFileHandler(logName, 
                                    mode = 'a', maxBytes = ROLLOVER_SIZE, 
                                    backupCount = MAX_BACKUPS)
                self.addHandler(logFileHandler)
            except IOError, msg:
                # Problem creating or opening the file?
                raise LoggerException, msg

            # Roll the log file over at start-up each time
            logFileHandler.doRollover()
            
            # Add any other handlers that we have been passed in 
            for handler in otherHandlers:
                self.addHandler(handler)

            # Set the logging level and record that we are initialised
            self.logger.setLevel(logLevel)
            self.logger.info('Error log initialised')
            self.logKeyDetails()
            
    def logKeyDetails(self):
        '''Log some key information about the system.'''
        self.logger.info("Node = %s", platform.node())
        self.logger.info("RoboSep Server Version = %s", tesla.config.SOFTWARE_VERSION)
        # self.logger.info("Python version = %s", platform.python_version())

    def __str__(self):
        '''Return a string representation of the logger object.'''
        return "Tesla logger (file log at %s)" % (self.pathName)

    def addHandler(self, handler):
        '''Add a handler to the list of handlers that get log messages.'''
        self.handlers.append(handler)
        handler.setFormatter(self.formatter)        
        self.logger.addHandler(handler)
   
    def log(self, level, msg):
        '''A generic log function: log a message to all registered loggers
        at the appropriate level (defined as a string)
        This is really designed to be called internally. Use with caution.'''
        self.__storeEntry(msg)
        exec("self.logger.%s(msg)" % (level))
   
    def logDebug(self, msg):
        """Log a debug message -- this is primarily for development"""
        self.log('debug', msg)

    def logPrintDebug(self, msg):
        """Log a debug message -- this is primarily for development"""
        self.log('debug', msg)
        print "Print Debug:  " + msg

    def logInfo(self, msg):
        """Log a generic information message: not for problem reporting"""
        self.log('info', msg)
        print "Info:  " + msg

    def logWarning(self, msg):
        """Log a warning message"""
        self.log('warning', msg)
        print "Warning:  " + msg
        
        
    def logError(self, msg):
        """Log an error message"""
        self.log('error', msg)
        print "Error:  " + msg
        CamMgr = RoboTrace.GetRoboCamMgrInstance();
        CamMgr.SetProcError(True);

    def getEntries(self, numEntries = 1, entryOffset = 0):
        '''Get a certain number of entries from the event logger, from
        an optional offset (default is the start, index = 0)'''
        if numEntries < 1:
            numEntries = 1                # Return at least one entry
        if entryOffset < 0:
            entryOffset = 0                # Don't start before the first entry 
        lastEntry = entryOffset + numEntries
        return self.__logBuffer[entryOffset:lastEntry]

    # --- support methods -----------------------------------------------------

    def __verifyLogPath(self, fileName):
        '''Private method: Ensure that the path for the specified filename 
        exists. If not,        create it.'''
        path = os.path.dirname(fileName)
        status = os.path.exists(path)
        if not status:
            # Path doesn't exist? Create it
            os.makedirs(path)
        return status
        
    def __storeEntry(self, entry):
        '''Private method: store an entry in our internal buffer for later 
        recall'''
        if len(self.__logBuffer) > MAX_BUFFERED_ENTRIES:
            # Remove the first entry if we are running over our buffer limit
            self.__logBuffer.pop(0)
        self.__logBuffer.append(entry)
        
# eof

