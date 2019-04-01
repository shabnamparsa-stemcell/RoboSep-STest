# 
# simple_client.py
#
# A simple client for testing the Tesla XML-RPC gateway (instrument controller)
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
import msvcrt
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

CLIENT_HOST = 'localhost'		# The host and port we will use
CLIENT_PORT = 8765

WAIT_TIME = 5		    # Time (in secs) that we wait for a state change

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
	self.controlSrv.socket.setblocking(0)		# 0 = non-blocking
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

class SimpleClient:
    '''A simple client for testing the Tesla XML-RPC interface. With this
    client, you should be able to start the instrument up, get a list of 
    protocols, schedule some samples and kick off a run.'''

    def __init__(self, hostURL, monitorURL, serviceURL, debug = True):
	self.hostURL = hostURL
	self.monitorURL = monitorURL
	self.serviceURL = serviceURL
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
	serverInfo = self.controlSrv.getServerInfo()
	print "  Gateway address:", serverInfo[0]
	print "   Server version:", serverInfo[1]
	print "    Server uptime:", serverInfo[2]
	print " Software version:", serverInfo[3]
	print "    Serial number:", serverInfo[4]
	
	hostInfo = self.controlSrv.getHostConfiguration()
	for key in hostInfo.keys():
	    print "%17s: %s" % (key, hostInfo[key])

	print  
	print "Instrument status:", self.controlSrv.getInstrumentStatus()
	print " Instrument state:", self.controlSrv.getInstrumentState()
	print "        Error log:"
	for entry in self.controlSrv.getErrorLog():
	    print "\t\t" + entry
	print 
       
	subscribers = self.controlSrv.getSubscriberList()
	print "      Subscribers:", 
	if len(subscribers):
	    print subscribers
	else:
	    print "None"
	print

    def getProtocols(self):
	'''Return the list of protocols'''
	return self.controlSrv.getProtocols()

    def waitForState(self, state = 'IDLE'):
	'''Wait until we reach the specified state.'''
	if self.debug: print 'Waiting for the %s state...' % (state)
	while True:
	    currentState = self.controlSrv.getInstrumentState()
	    if currentState in ['HALTED', 'ESTOP', 'SHUTDOWN', state]:
		if self.debug: print "Breaking on %s" % (currentState)
		break
	    else:
		time.sleep(WAIT_TIME)
	return currentState
	
    def waitForIdle(self):
	'''Wait until we reach the IDLE state.'''
	return self.waitForState('IDLE')

    def reportETC(self, etc):
	'''Report the ETC for the schedule or run, if the debug flag is on.'''
	if self.debug: 
	    if self.monitor.errorFlag.isSet():
		print 'ETC not available (failed schedule)'
	    else:
		print 'ETC =', etc

    def runSamples(self, sampleList):
	'''Start a run, using a set of samples, and then wait for it to 
	finish (ie. return to IDLE).'''
	state = self.waitForIdle()
	if state == 'IDLE':
	    if self.debug: print 'Now running samples.'	
	    try:
		self.monitor.errorFlag.clear()	    # Clear the error flag
		etc = self.controlSrv.startRun(sampleList)
		self.reportETC(etc)
	    except Exception, msg:
		if self.debug: print 'ERROR ===>', msg
	    if not self.monitor.errorFlag.isSet():
		# No error, then wait for us to return to the 'idle' state
		time.sleep(WAIT_TIME)
		state = self.waitForIdle()
	    else:
		# Otherwise, clear the error flag and return
		self.monitor.errorFlag.clear()
            print "\aRun complete"              # Ring a bell to alert the user
	return state
  
    def scheduleSamples(self, numSamples, protocolName, sampleVolume = 1000):
	'''Set up some samples and schedule them.'''
	# Pick the first protocol
	protocol = [p for p in self.getProtocols() if p['label'] == protocolName][0]

        if sampleVolume < protocol['minVol'] or sampleVolume > protocol['maxVol']:
            print "ERROR: volume (%d) is out of range (%d -> %d)" % (sampleVolume, protocol['minVol'], protocol['maxVol'])
            return []
        
	pID = protocol['ID']
	cons = self.controlSrv.calculateConsumables(pID, sampleVolume)

	samples = []
	for i in range(1, numSamples + 1):
            try:
                sample = Sample(i, "Sample %d" % (i), protocol['ID'], sampleVolume, i)
                samples.append(sample)
            except TeslaException, msg:
                print "ERROR:", msg
                return []

	try:
	    self.monitor.errorFlag.clear()	# Clear the error flag
	    etc = self.controlSrv.scheduleRun(samples)
	    self.reportETC(etc)
	except Exception, msg:
	    if self.debug: print 'ERROR --->', msg
	if not self.monitor.errorFlag.isSet():
	    # Successful? Let the operator know
	    print "Samples scheduled... with protocol: %s" % (protocolName)
	else:
	    # Otherwise, clear the error flag
	    self.monitor.errorFlag.clear()
	return samples

# -----------------------------------------------------------------------------

def pickAProtocol(protocols):
    '''Print out protocol information from the server and get the user to pick
    one of the choices.
    Returns a protocol label or 'Q' (for quit).'''
    print "\n        Protocols:"
    i = 0
    for protocol in protocols:
	print "\t[%d]\t%s\t%s" % (i, hex(protocol['ID']), protocol['label'])
	i += 1
    print "\tor [Q]uit\n"
    try:
	index = int(raw_input('Protocol number: '))
	p = protocols[index]
	print "Selected [%d] = protocol: %s\n" % (index, p['label'])
	return p['label']
    except:
	return 'Q'

def pickNumSamples():
    '''Return a number between 1 and 4 for the number of samples to schedule/run'''
    N = 4
    while True:
	print "How many samples do you want to run (1 - %d): " % (N),
	try:
	    key = int(msvcrt.getche())
	except:
	    pass
	if key in range(1, N + 1):
	    break
    print
    return key

def askForVolume():
    '''Return the sample volume, as a number between 0 and 10000 uL'''
    try:
        volume = int(raw_input('Sample volume (in uL): '))
    except:
        volume = 1000
        print "Error! Reverting to volume = %d" % (volume)
    return volume

# -----------------------------------------------------------------------------

if __name__ == '__main__':
    # Enable us to load packages from the zip archive (if it's there)
    sys.path.append(tesla.config.TESLA_ARCHIVE)
    
    hostURL = "http://%s:%d" % (GATEWAY_HOST, GATEWAY_PORT)
    monitorURL = "http://%s:%d" % (CLIENT_HOST, CLIENT_PORT)
    serviceURL = hostURL
    
    client = SimpleClient(hostURL, monitorURL, serviceURL)
    protocols = client.getProtocols()

    # Loop until the user presses 'Q' when prompted for a protocol
    while True:
	client.waitForIdle()
	protocolPick = pickAProtocol(protocols)
	if protocolPick == 'Q': break
	numSamples = pickNumSamples()
        sampleVolume = askForVolume()
    
	samples = client.scheduleSamples(numSamples, protocolPick, sampleVolume)
        if len(samples) > 0:
            state = client.runSamples(samples)
            if state != 'IDLE':
                "State = %s; terminating..." % (state)
                break

    client.halt()
	
# eof

