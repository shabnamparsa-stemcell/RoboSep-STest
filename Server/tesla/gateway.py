# 
# gateway.py
#
# Tesla XML-RPC gateway between the instrument controller and client apps
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
  
import xmlrpc.server
import socket
import time

import tesla.config
from tesla.exception import TeslaException
from tesla.interface import InstrumentInterface

# -----------------------------------------------------------------------------

class Gateway(object):
    '''The Tesla instrument & service XML-RPC gateway.
    See tesla.controller.Controller() for an example of how to use the gateway.
    '''
    
    def __init__(self, controlCentre, host = tesla.config.GATEWAY_HOST, 
        controlPort = tesla.config.GATEWAY_PORT, logging = False):
        '''Construct the XML-RPC gateway with an instrument control centre 
        instance, an optional host and optional port. Turning logging on 
        enables the standard SimpleXMLRPCServer requests logging.
        '''
        self.__runFlag = False
        self.controlCentre = controlCentre

        hostsToTry = [host,]
        if host != 'localhost':
            hostsToTry.append('localhost')

        for targetHost in hostsToTry:
            successFlag = True            
            gatewayAddress = "http://%s:%d" % (targetHost, controlPort)
            try:
                self.controlServer = xmlrpc.server.SimpleXMLRPCServer((targetHost, controlPort), 
                                        logRequests = logging)
                # Register the various methods for the instrument control server
                self.controlServer.register_instance(InstrumentInterface(gatewayAddress, controlCentre))
                self.controlServer.register_introspection_functions()
                self.controlServer.register_function(self.halt)
                # If we got to here, we succeeded so break out
                break
            except socket.error as msg:
                # Got here because we couldn't bind to the specified gateway address
                print("WARNING: Unable to bind %s (%s)" % (gatewayAddress, msg))
                successFlag = False
        if not successFlag:
            raise TeslaException("XML-RPC server socket error = %s (address = %s)" % \
                    (msg, gatewayAddress))

                
    def isRunning(self):
        '''Returns True if the gateway is running.'''
        return self.__runFlag

    def run(self):
        '''Start the gateway server running (accepting requests from clients).'''
        self.__runFlag = True
        try:
            while self.__runFlag:
                self.controlServer.handle_request()
                currentState = self.controlCentre.getState()
                # If the control centre has shutdown or turned off, shutdown
                # the XML-RPC interface
                if currentState in ['SHUTDOWN', 'OFF']:
                    self.halt()
        except KeyboardInterrupt:
            # Caught an interrupt, can we exit more gracefully than just
            # dropping out of the handle_request() loop?
            pass
        except Exception as e:
            raise TeslaException("Caught unexpected error: %s" % (e))
        # A server has cleared the run flag, so close the servers
        self.close()


    def close(self):
        '''Close the XML-RPC servers.'''
        if self.isRunning():
            self.halt()
        try:
           self.controlServer.server_close()
        except socket.error:
            # Ignore any socket errors associated with closing the server
            pass                
        

    # --- XML-RPC methods that are available to all XML-RPC interfaces --------

    def halt(self):
        '''Halt the XML-RPC servers.
        IN: Nothing
        OUT: (bool) [always true]
        '''
        self.__runFlag = False
        return True

# eof
