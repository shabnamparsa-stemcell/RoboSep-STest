# 
# Component.py
# tesla.hardware.Component
#
# Abstract class for common behaviour between devices and subsystems
# 
# Copyright (c) Invetech Operations Pty Ltd, 2004
# 495 Blackburn Rd
# Mt Waverley, Vic, Australia.
# Phone (+61 3) 9211 7700
# Fax   (+61 3) 9211 7701
# 
# The copyright to the computer program(s) herein is the
# property of Invetech Operations Pty Ltd, Australia.
# The program(s) may be used and/or copied only with
# the written permission of Invetech Operations Pty Ltd
# or in accordance with the terms and conditions
# stipulated in the agreement/contract under which
# the program(s) have been supplied.
# 

import inspect
import tesla.config

# -----------------------------------------------------------------------------

class Component(object):
    '''Base class for the Device and Subsystem classes.'''

    def __init__( self, name ):
        '''Construct the component with an identifying name.'''
	self.__name = name
	self.debug = tesla.config.HW_DEBUG	# Enable the H/W debug flag locally

    def _serviceMethods(self):
        '''Return a list of methods for the service interface. If a device doesn't
        override this, it gets this method by default.'''
        return []
  
    def getServiceMethods(self):
        '''Returns a list of the methods that are published to the service interface'''
        methodList = inspect.getmembers(self, inspect.ismethod)
        serviceMethods = []
        for methodEntry in methodList:
            (method, boundMethod) = methodEntry
            if method in self._serviceMethods():
                serviceMethods.append(methodEntry)
        return serviceMethods
   
    def getName(self):
	'''Return the device's identifying name.'''
	return self.__name

    def execute( self, functionWithArgs ):
        '''Executes the formatted function call string functionWithArgs on
        self.'''
        return eval( functionWithArgs )

# eof

