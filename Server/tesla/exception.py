# 
# exception.py
# tesla.exception.py
#
# Base exception class for the Tesla control software 
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

class TeslaException(StandardError):
    '''The base exception class for the Tesla instrument software. All other
    exceptions should be derived from this to pick up any specific behaviour
    that we want/need for Tesla.'''
    pass

# eof
