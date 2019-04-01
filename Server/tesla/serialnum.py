# 
# serialnum.py
# tesla.serialnum
#
# Simple class to handle the instrument serial number
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

import tesla.config 
from tesla.exception import TeslaException

# -----------------------------------------------------------------------------

class InstrumentSerialNumber(object):
    '''Read-only class to retrieve the instrument serial number, which is an
    integer value.'''

    __serial = 'unspecified'
    __haveSerial = False

    def __init__(self, serialFileName = tesla.config.SERIAL_PATH):
	'''Instrument serial number constructor'''
	if not self.__haveSerial:
	    try:
		f = open(serialFileName, 'r')
		# Read in the first line and strip all whitespace (including
		# carriage returns)
                valueFromFile = f.readline().strip()
                if self.validate(valueFromFile):
		    InstrumentSerialNumber.__serial = valueFromFile
                else:
                    raise TeslaException, "Invalid serial number (%s)" % (valueFromFile)
		f.close()
		InstrumentSerialNumber.__haveSerial = True
	    except IOError, msg:
                # Any problem, throw an exception to force the developer or
                # production engineer to install a serial number file
                raise TeslaException, "Unable to read instrument serial number (%s)" % (msg)

    def __repr__(self):
        '''Return a string representation of the serial number'''
        return str(self.__serial)

    def validate(self, serialNumber):
        '''Returns True if the serial number appears to be valid :)'''
        validFlag = True
        try:
            val = int(serialNumber)
        except ValueError, msg:
            validFlag = False
        return validFlag

    # --- static methods ---

    def reset():
        '''Static method: forces the serial number class to be reset (so we 
        can re-read the file)'''
        InstrumentSerialNumber.__haveSerial = False
    reset = staticmethod(reset)

    def getSerial():
	'''Static method: returns the instrument serial number (as a string).'''
	return InstrumentSerialNumber.__serial
    getSerial = staticmethod(getSerial)

# -----------------------------------------------------------------------------

if __name__ == '__main__':
    serial = InstrumentSerialNumber()
    print "[%s]" % serial.getSerial()

# eof

