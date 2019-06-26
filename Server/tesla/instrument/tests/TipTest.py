# 
# TipTest.py
# Simple functional test for tip pick up and stripping
#
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

import sys, time

from ipl.utils.validation import validateNumber
import tesla.config
from tesla.instrument.TeslaPlatform import TeslaPlatform

# -----------------------------------------------------------------------------
# If called from the command line, run a simple functional test to demonstrate
# the tip pickup and stripping by iterating through each quadrant and tip

if __name__ == '__main__':

    NUM_TIPS = 5

    platform = TeslaPlatform()
    
    startQ = 1
    endQ = platform.NbrSectors()
    startTip = 4
    endTip = NUM_TIPS
    nbrIterations = -1
    
    try:
        argc = len (sys.argv)
        """
        for i in range (1, argc):
            arg = sys.argv[i]
            if arg[0] == 'S':
            elif arg[0] == 'T':
            elif arg[0] == 'I':
            else:
                raise Exception, "Invalid parameter: '%s'" % (arg)
        startQ = int(sys.argv[1])
        endQ = int(sys.argv[2])
        for quad in (startQ, endQ):
            if not validateNumber(quad, 1, tesla.config.NUM_QUADRANTS):
            raise ValueError, "Quadrant #%d is invalid" % quad

        startTip = int(sys.argv[3])
        endTip = int(sys.argv[4])
        for tip in (startTip, endTip):
            if not validateNumber(tip, 1, NUM_TIPS):
            raise ValueError, "Tip #%d is invalid" % tip
        """
    except Exception as msg:
        print(msg)
        print("Usage: python TipTest.py [Sa[:b]] [Ta[:b]] In")
        print("       where: S = sector range a, b. 1:4 is the default")
        print("              T = tip range a, b. 1:5 is the default")
        print("              I = number of iterations. n = -1 (indefinite) is the default")
        sys.exit(1)
        
    print("Telsa tip pickup/stripping functional test")
    print("Start quadrant = %d, end quadrant = %d" % (startQ, endQ))
    print("Start tip = %d, end tip = %d" % (startTip, endTip))
    
    platform.Initialise()

    print("Testing tip pickup & stripping")

    trial = 0

    try:
        while trial != nbrIterations:
            trial += 1
            print("Trial # %d\n(Press ^C to break into program)" % (trial))

            for sector in range(startQ, endQ + 1):
                for tipNum in range(startTip, endTip + 1):
                    print("Moving to sector #%d, tip #%d" % (sector, tipNum))            
                    platform.PickupTip(sector, tipNum)
                    platform.StripTip()

    except KeyboardInterrupt:
        print("Break during trial #%d. Press 'Enter' to continue" % (trial))
        sys.stdin.readline()
    
# eof
