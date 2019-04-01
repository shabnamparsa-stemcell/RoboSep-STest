#!/bin/sh
# gendoc.sh
#
# Generate HTML documentation from the Python code
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
# Note that this requires cygwin (or a similar environment) to operate
# And there is no error checking here at the moment

PYDOC="c:\\python23\\lib\\pydoc.py"

for f in *.py;
do
	NAME=`basename $f .py`
	python $PYDOC -w $NAME
done

mv *.html docs

# eof
