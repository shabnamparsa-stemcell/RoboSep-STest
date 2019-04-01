# 
# Device.py
# tesla.Hardware.Device
#
# Abstract class for handling devices
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

from Component import Component

# -----------------------------------------------------------------------------

class Device(Component):
    '''Base class for low-level hardware devices that captures all the 
    common functionality.'''

    # Our list of registered devices
    deviceList = []

    # Our (shared) logging resource
    logger = None

    def __init__( self, name ):
        '''Construct the device with an identifying name.'''
        Component.__init__(self, name)
	Device.registerDevice(self)

    def __str__(self):
	'''Simple string representation of the device.'''
	return "Device: %s" % (self.getName())

    def reset():
        '''Reset the device list'''
        Device.deviceList = []
  
    def registerDevice(deviceInstance):
	'''Register our device with the Device list. Note that this is a 
	static method.'''
	Device.deviceList.append(deviceInstance)

    def registerLogger(loggerInstance):
	'''Register an instance of a Tesla logger with the devices. Note that
	this is a static method.'''
	# Ideally, we would check to ensure that the loggerInstance had the
	# various methods that we need.
	Device.logger = loggerInstance

    # Define our static methods
    reset = staticmethod(reset)
    registerLogger = staticmethod(registerLogger)
    registerDevice = staticmethod(registerDevice)

# eof

