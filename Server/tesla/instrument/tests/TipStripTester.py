# 
# TipStripTester.py
# Simple functional test for tip stripping (mainly for assessing the power
# profile needed for good stripping)
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

from tesla.instrument.TeslaPlatform import TeslaPlatform

# -----------------------------------------------------------------------------
# If called from the command line, pick up a tip and then strip it
# We do this for Quadrant 1, tips 1 and 4

if __name__ == '__main__':

    sector = 1
    startTip = 1
    endTip = 4
    
    platform = TeslaPlatform()
    platform.Initialise()

    for tipNum in (startTip, endTip):
	print "Moving to sector #%d, tip #%d" % (sector, tipNum)	    	
	platform.PickupTip(sector, tipNum)
	platform.StripTip()
    
# eof
