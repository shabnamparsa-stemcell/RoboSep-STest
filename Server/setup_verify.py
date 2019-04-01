# 
# setup.py
# Disutils tools for creating an exe version of RoboSepLauncher.py
# Python assets
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
# Notes:
#
# To build the executable:
#	python setup.py py2exe
#

from distutils.core import setup
import py2exe

setup(
    console = [ 
        { 
            "script": "recover.py", 
            "icon_resources": [(1, "..\shared\images\Launcher.ico")] 
        } ,
    ],
    options = { "py2exe" : {"includes": "encodings", "packages": "encodings",} },
)


# eof
