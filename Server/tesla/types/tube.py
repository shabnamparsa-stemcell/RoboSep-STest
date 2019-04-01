# 
# tube.py
# tesla.types.tube
#
# Provides a Tube object for monitoring liquid levels in the Tesla
# instrument control software
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

from tesla.exception import TeslaException
from tesla.types.TubeGeometry import TubeGeometry

# -----------------------------------------------------------------------------

class TubeException(TeslaException):
    '''Exceptions caused by tube problems; usually trying to overfill the
    tube.'''
    pass

# -----------------------------------------------------------------------------

class Tube:
    '''Define the characteristics for a tube (or any other liquid container)'''
    
    def __init__(self, sector, label, geometry, initialVolume_uL = 0):
	'''Define a tube with an identifying sector position, label, a start 
	volume and a maximum volume'''
	self.__sector = sector
	self.__label = label
	self.__geometry = geometry
	self.__volume = 0
	self.setVolume(initialVolume_uL)

    def __str__(self):
	'''A string representation of the tube'''
	return "%s [Q%d]: %0.1f of %0.1f uL" % (self.getLabel(), \
                self.getSector(), self.getVolume(), self.getMaxVolume())

    # --- volume handling methods ---

    def setVolume(self, volume_uL):
	'''Set the tube volume (in uL).'''
	if volume_uL < 0 or volume_uL > self.getMaxVolume():
	    raise TubeException, "%0.1f uL %s tube can't hold %0.1f uL" % \
		    (self.getMaxVolume(), self.__label, volume_uL)
	self.__volume = volume_uL

    def addVolume(self, volume_uL):
	'''Add a volume (in uL) to the tube'''
	if self.getMaxVolume() < (self.getVolume() + volume_uL):
	    raise TubeException, "%0.1f uL %s tube can't add %0.1f uL (already at %0.1f uL)" % \
		    (self.getMaxVolume(), self.__label, volume_uL, self.getVolume())
	else:
	    self.__volume += volume_uL

    def removeVolume(self, volume_uL):
	'''Remove a volume (in uL) from the tube'''
	if self.getVolume() < volume_uL:
	    # If we want to remove more than is there, just remove what's there
	    volume_uL = self.getVolume()
	self.__volume -= volume_uL

    def empty(self):
	'''Empty the tube'''
	self.setVolume(0)

    # --- various accessor methods ---

    def getSector(self):
	'''Return the tube sector position'''
	return self.__sector

    def getLabel(self):
	'''Return the tube label'''
	return self.__label

    def getVolume(self):
	'''Return the current tube volume (in uL)'''
	return self.__volume

    def getMeniscus(self):
	'''Return the current position of meniscus from base of tube'''
	return self.__geometry.HeightOfMeniscus (self.__volume)

    def getMeniscusAfterAdding(self, addedVolume):
	'''Return the position of meniscus after adding a given volume'''
	return self.__geometry.HeightOfMeniscus (self.__volume + addedVolume)

    def getMeniscusAfterRemoving(self, removedVolume):
	'''Return the position of meniscus after removing a given volume'''
	return self.__geometry.HeightOfMeniscus (self.__volume - removedVolume)

    def getBasePosition(self):
	'''Return the position of the tube base wrt global coordinates'''
	return self.__geometry.BasePosition ()

    def getMaxVolume(self):
	'''Return the maximum volume the tube can hold (in uL)'''
	return self.__geometry.Capacity()

    def getDeadVolume(self):
	'''Return the dead volume unavailable fo raspiration (in uL)'''
	return self.__geometry.DeadVolume()

    def getMaxHeight(self):
	'''Return the height of the meniscus at full volume'''
	return self.__geometry.HeightAtCapacity()

# eof

