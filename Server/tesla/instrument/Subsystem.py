# 
# Subsystem.py
# tesla.instrument.Subsystem
#
# Base subsystem class
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

from tesla.hardware.Component import Component

# -----------------------------------------------------------------------------

class Subsystem(Component):
    '''Abstract subsystem class, this gives us common functionality across
    any of the subsystems in the tesla.instrument package.'''
    
    SectionName = 'Base Subsystem'
 
    mandatorySettings = ( )

    # Our list of registered Subsystems
    instanceList = []

    def __init__(self, name ):
        '''Initialise the subsystem'''
        Component.__init__(self, name)
        self.debugFlag = self.debug         # For historical purposes
        if self.debug:
	    self.Trace = self.PrintTrace
	else:
	    self.Trace = self.NullTrace

	Subsystem.registerInstance( self )

    # ---------------------------------------------------------------------------
    
    def __str__(self):
	'''Simple string representation of the Subsystem.'''
	return "Subsystem: %s" % (self.getName())
    
    def reset():
        '''Reset the subsystem instance list'''
        Subsystem.instanceList = []
      
    def registerInstance( instance ):
	'''Register our instance with the Instance list. Note that this is a 
	static method.'''
	Subsystem.instanceList.append( instance )
        
    # ---------------------------------------------------------------------------

    def NullTrace(self, message):
	'''Don't do anything with the message; used when we don't want debugging
	traces.'''
	pass
    
    def PrintTrace (self, message):
        '''Print a debug message'''
	print "%s TRACE: %s" % (self.SectionName, message)
        
    # ---------------------------------------------------------------------------

    # Define our static methods
    reset = staticmethod(reset)    
    registerInstance = staticmethod( registerInstance )


# eof
