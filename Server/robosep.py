# 
# robosep.py
# Script to launch an instance of the Robosep instrument controller
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

import sys
import traceback, string


# -----------------------------------------------------------------------------
# Support functions

def reportImportError(msg = None):
    '''Report an error related to module importing'''
    print 'Some key modules could not be located.\nIf you installed these files ' + \
	  'manually, ensure that the PYTHONPATH environment variable is set (correctly).'
    if msg:
	print "\nImport error: %s" % (msg)
    sys.exit(1)

def testForModule(module):
    '''Returns true if the specified module can be imported, false otherwise'''
    try:
	exec("import %s" % (module))
    except ImportError:
	flag = False
    else:
	flag = True
    return flag

def testPackageAvailability():
    '''Have the key packages been installed correctly? If not, alert the user and die'''
    modules = ['ipl', 'SimpleStep', 'tesla']
    for module in modules:
	if not testForModule(module):
	    print "Error: Unable to import the '%s' module.\n" % (module)
	    reportImportError()

def setupPackageArchive(archiveName):
    '''Set up importing from an archive file'''
    sys.path.append(archiveName)
    
def reportException(msg):
    '''Report an exception with a simplified traceback.'''

    limit = None
    type, value, tb = sys.exc_info(  )
    list = traceback.format_tb(tb, limit) + traceback.format_exception_only(type, value)
    tracebackMsg = "\nError traceback:\n\n" + "%-20s %s" % (string.join(list[:-1], ""), list[-1])
    
    print "\nCAUGHT FATAL ERROR:", msg
    print tracebackMsg
    print "Please report this message to Chulwoong Jeon <CHULWOONG.JEON@STEMCELL.COM>"

def startRoboSepServer():
    #PKG_ARCHIVE = 'robosep.zip'
    
    # If we have our packages, start up the controller
    #setupPackageArchive(PKG_ARCHIVE) 
    testPackageAvailability()

    from tesla.exception import TeslaException
    from tesla.controller import Controller

    try:
	controller = Controller()
    except TeslaException, msg:
	# Handle instrument-specific exceptions that have bubbled to the top
	print "Unable to start instrument controller (%s)" % (msg)
    except OSError, msg:
        # Most likely caused by the logger being locked by another process
        print "Unable to start instrument controller (%s)" % (msg)
        print "Is another instance of the controller running?"       
    except StandardError, msg:
	# Handle any other (unexpected) error, with debug info
	reportException(msg)	    
    else:
	try:
	    controller.powerUpInstrument()
	    controller.startGateway()
	    controller.shutdownInstrument(False)
	except TeslaException, msg:
	    controller.logger.logError(msg)

# -----------------------------------------------------------------------------

if __name__ == '__main__':
    startRoboSepServer()

# eof

