# 
# servicetest.py
#
# A simple client for testing the Tesla service functionality
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

import sys
import time
import threading
import socket, xmlrpclib, SimpleXMLRPCServer

from ipl.utils.InfoTable import InfoTable
import ipl.utils.string
from ipl.task.simplethread import SimpleThread

import tesla.config
from tesla.exception import TeslaException
from tesla.types.sample import Sample

# -----------------------------------------------------------------------------
# Constants that should not need to change too often (not soft-setups)

GATEWAY_HOST = tesla.config.GATEWAY_HOST
GATEWAY_PORT = tesla.config.GATEWAY_PORT

CLIENT_HOST = 'localhost'                # The host and port we will use
CLIENT_PORT = 8765

WAIT_TIME = 5                    # Time (in secs) that we wait for a state change

# -----------------------------------------------------------------------------

class ClientInterface:
    '''This is the interface that we subscribe the server to send back reports
    to.'''

    def __init__(self, errorFlag):
        '''Constructor with one parameter: an errorFlag Event object that 
        allows us to indicate that an error has occurred.'''
        self.errorFlag = errorFlag
        self.errorTable = InfoTable(tesla.config.ERROR_CODES_PATH)

    def ping(self):
        '''Return True if we can be pinged.'''
        return True

    def reportETC(self, etc):
        '''Report the run ETC to us (usually if it has been updated).'''
        print "CLIENT: ETC = %s" % (etc)
        return True

    def reportStatus(self, state, msg, numParams, params):
        '''Report a status message to us.'''
        print "CLIENT: STATE = %s, STATUS = %s, #PARAMS = %d, PARAMS = %s" % \
                (state, msg, numParams, params)
        return True
        
    def reportError(self, level, errorCode, numParams, params):
        '''Report an error message to us.'''        
        # Find the error message that corresponds to this code
        errorMsg = self.errorTable.getMessage(errorCode)
        self.errorFlag.set()
        try:
            logMsg = ipl.utils.string.expandCSharpString(errorMsg, params)
        except Exception, msg:
            logMsg = "CLIENT: CAUGHT EXCEPTION %s with message [%s][%s]" \
                    (str(msg), errorMsg, str(params))
        print "CLIENT: ERROR = %s [level:%d], #PARAMS = %d, PARAMS = %s" % \
                (errorCode, level, numParams, params)
        print "CLIENT: ERROR = %s" % (logMsg)

        return True

# -----------------------------------------------------------------------------

class Monitor:
    '''An XML-RPC server that is used by the instrument controller server to
    report status and error messages back to us with.'''
    
    def __init__(self, host = CLIENT_HOST, port = CLIENT_PORT, 
            logging = False):
        '''Construct the XML-RPC gateway with an optional host and optional 
        port. Turning logging on enables the standard SimpleXMLRPCServer requests 
        logging.'''
        self.errorFlag = threading.Event()
        self.controlSrv = SimpleXMLRPCServer.SimpleXMLRPCServer((host, port), logRequests = logging)
        self.controlSrv.socket.setblocking(0)                # 0 = non-blocking
        self.controlSrv.register_introspection_functions()
        self.controlSrv.register_instance(ClientInterface(self.errorFlag))

    def run(self, x, period):
        '''Start the server running.'''
        while not x.isSet():
            self.controlSrv.handle_request()
            x.wait(period)
        self.controlSrv.server_close()            

    def check(self):
        '''Check for an incoming request to handle.'''
        self.controlSrv.handle_request()

# -----------------------------------------------------------------------------

class ServiceClient:
    '''A simple client for testing the Tesla XML-RPC interface. With this
    client, you should be able to start the instrument up, get a list of 
    protocols, schedule some samples and kick off a run.'''

    def __init__(self, hostURL, monitorURL, debug = True):
        self.hostURL = hostURL
        serviceURL = hostURL
        self.monitorURL = monitorURL
        self.debug = debug
        
        # Connect to the instrument control server
        if self.debug: print "Attempting to connect to %s" % (hostURL)
        try:
            self.controlSrv = xmlrpclib.ServerProxy(hostURL)
            serverIsAlive = self.controlSrv.ping()
        except socket.error, msg:
            raise TeslaException, "Unable to connect to %s: %s" % (hostURL, msg)
        
        if serverIsAlive:
            self.monitor = Monitor()
            self.monitorThread = SimpleThread(self.monitor.run)
            self.monitorThread.start()
            self.controlSrv.subscribe(monitorURL)
            if self.debug: self.printInfo()

        # Now connect to the service server
        if self.debug: print "Attempting to connect to %s" % (serviceURL)
        try:
            self.serviceSrv = xmlrpclib.ServerProxy(serviceURL)
            serviceIsAlive = self.serviceSrv.ping()
            print "Service interface is", 
            if serviceIsAlive:
                print "up\n"
            else:
                print "down\n"
        except socket.error, msg:
            raise TeslaException, "Unable to connect to %s: %s" % (serviceURL, msg)
        
    def halt(self):
        '''Halt the client and server'''
        self.monitorThread.join()
        if self.debug: print "Halting..."
        self.controlSrv.halt()

    def printInfo(self):
        '''Print some general information from the server.'''
        print "\nInstrument state: %s\n" % (self.controlSrv.getInstrumentState())

    def waitForState(self, state = 'IDLE'):
        '''Wait until we reach the specified state.'''
        if self.debug: print 'Waiting for the %s state...' % (state)
        while True:
            currentState = self.controlSrv.getInstrumentState()
            if currentState in ['HALTED', 'ESTOP', 'SHUTDOWN', state]:
                break
            else:
                time.sleep(WAIT_TIME)
        return currentState
        
    def waitForIdle(self):
        '''Wait until we reach the IDLE state.'''
        return self.waitForState('IDLE')

    def printServiceFunctions(self, functions):
        componentEntries = {}
        for func in functions:
            (componentString, docString, method, args) = func
            component = componentString.split(': ')[1]
            # Ugly but it was quick and it works :)
            argString = str(args).replace("'",'').replace('[','').replace(']','')
            methodStr = "%s(%s)" % (method, argString)
            if not componentEntries.has_key(component):
                componentEntries[component] = [methodStr,]
            else:
                componentEntries[component].append(methodStr)
        for component in componentEntries:
            print component
            for method in componentEntries[component]:
                print "\t%s" % (method)
            print 

    def exerciseService(self, serviceCalls = []):
        '''Exercise the service interface. Pass in a list of tuples of
        service cmds to execute.'''
        self.controlSrv.enterServiceState()
        self.waitForState('SERVICE')
        print "\nNow in service state\n"

        print "Supported service functions:\n"
        functions = self.controlSrv.getServiceFunctionList()
        self.printServiceFunctions(functions)

        if serviceCalls != []:
            print "\nNow executing service calls:"
            for (objName, method, args) in serviceCalls:
                print "\nExecuting: %s.%s(%s)" % (objName, method, args)
                result = self.controlSrv.execute(objName, method, args)
                print "Result = [%s]" % (str(result))

        print "\nExiting service state\n"
        self.controlSrv.exitServiceState()
        self.waitForState()
        

# -----------------------------------------------------------------------------

if __name__ == '__main__':
    # Enable us to load packages from the zip archive (if it's there)
    sys.path.append(tesla.config.TESLA_ARCHIVE)
    
    hostURL = "http://%s:%d" % (GATEWAY_HOST, GATEWAY_PORT)
    monitorURL = "http://%s:%d" % (CLIENT_HOST, CLIENT_PORT)

    serviceCalls = [('Device: Pump', 'Home', ''),
                    ('Device: Pump', 'GetFirmwareVersion', ''), 
                    ('Device: TipStripper', 'IsEngaged', ''),
                    ('Subsystem: Carousel', 'Home', ''),
                    ('Subsystem: Carousel', 'Theta', ''),
                    ('Subsystem: Carousel', 'SetTheta', '90'),
                    ('Subsystem: Carousel', 'Theta', ''),
                    ('Subsystem: Calibration','startCalibration',''),
                    # The following ones are nonsensical but shouldn't cause
                    # an exception to be raised
                    ('Subsystem: Carousel', 'GetFirmwareVersion', ''),
                    ('Subsystem: SpaceShuttle','Launch','to_moon'),
                    ]

    client = ServiceClient(hostURL, monitorURL)
    client.waitForIdle()
    client.exerciseService(serviceCalls)
    client.halt()
        
# eof

