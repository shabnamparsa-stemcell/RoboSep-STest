# 
# procheck.py
#
# Check a RoboSep protocol and ensure that it has legal syntax (or at least 
# sufficiently legal to be read in to the RoboSep instrument server app)
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

import sys
from tesla.types.Protocol import Protocol, ProtocolException

if __name__ == '__main__':

    if len(sys.argv) < 2:
        print("Syntax: %s protocol_file" % (sys.argv[0]))
        sys.exit(1)
    else:
        for file in sys.argv[1:]:
            print("\nChecking protocol syntax in %s" % (file))
            try:
                p = Protocol(file)
                print("Protocol definition okay")
            except ProtocolException as msg:
                print("Error: %s" % (msg))

# eof

