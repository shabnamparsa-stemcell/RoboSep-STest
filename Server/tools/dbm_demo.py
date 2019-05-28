# 
# dbm_demo.py
#
# Demonstrate how to use DBM databases for the component tracking
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

import anydbm
from tesla.config import COMPONENT_DB_PATH

# -----------------------------------------------------------------------------

class Device(object):
    def __init__(self, name):
        print "Constructing %s (%s)" % (name, str(self))
        self.name = name
        self.usageCount = 0
        self.__readTrackCount()

    def __del__(self):
        self.__writeTrackCount()
        print "And we have closed %s (track count = %d)" % (self.name, self.usageCount)

    def __readTrackCount(self):
        trackDB = anydbm.open(COMPONENT_DB_PATH, 'c')
        if trackDB.has_key(self.name):
            self.usageCount = int(trackDB[self.name])
        else:
            self.usageCount = 0
        trackDB.close()

    def __writeTrackCount(self):
        # This is usually called during exit and the module reference may have
        # already been cleaned up. In that case, just re-import it
        # There are more elegant solutions, but this will serve for the demo
        import anydbm
        trackDB = anydbm.open(COMPONENT_DB_PATH, 'c')
        # The dbm expects keys and values to be strings, so convert...
        trackDB[self.name] = str(self.usageCount)
        trackDB.close()
        
    def move(self, steps):
        self.usageCount += 1
        # Then do the move to the 'steps' position... 

    def home(self):
        self.move(0)

# -----------------------------------------------------------------------------

class Axis(Device):
    pass

# -----------------------------------------------------------------------------

zAxis = Axis('Z axis')
theta = Axis('Theta axis')

zAxis.home()
theta.home()

zAxis.move(500)
theta.move(100)
theta.move(2000)

theta.move(10)

zAxis.home()
theta.home()

# eof

