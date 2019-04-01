# 
# startserver.py
# Starts a ProxyServer to the RoboSep server on the localhost
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
# Starts a ProxyServer to the RoboSep server on the localhost
# This script should be called with the command 'python -i startserver.py'
# so that it falls out to the promt when it is finished
#
# Usage:
#          Execute the associated batch script 'startserver.bat'

from xmlrpclib import ServerProxy
from sys import exit

s = ServerProxy( "http://localhost:8000" )
try:
    s.ping()
except:
    print "Cannot ping RoboSep server - exiting..."
    exit( -1 )

print "Connected to RoboSep server. The ServerProxy object is s."
print "Hit CTRL-Z ENTER to exit."
print ""

# eof

