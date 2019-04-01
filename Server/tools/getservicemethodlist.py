# getservicemethodlist.py
# Starts a ProxyServer to the RoboSep service server on the localhost,
# gets the service function list and outputs a csv file compatible with the
# Invetech "generic" service software.

# Usage:
#          'python getservicemethodlist.py > tesla_service.csv'


from xmlrpclib import ServerProxy
from sys import exit
import re


# Open up service interface
s = ServerProxy( "http://localhost:8000" )
try:
    s.ping()
except:
    print "Cannot ping RoboSep service server - exiting..."
    exit( -1 )

# Build re pattern to remove more than one consecutive space (gets rid of newlines)
removeNewlinesAndMultipleSpaces = re.compile( r'\s\s+', re.DOTALL )

removeClassName = re.compile( r'.+\:\s' )

previousObject = ""

for obj,doc,func,args in s.getServiceFunctionList():
    # If it is a new object put a blank line
    if obj != previousObject:
        print
    previousObject = obj

    # Remove newlines from documentation
    doc = removeNewlinesAndMultipleSpaces.sub( r' ', doc )

    # Remove double quotes from documentation
    doc = doc.replace( '"', '' )

    # Make a copy of the object name without the class name before it
    objWithoutClassName = removeClassName.sub( r'', obj )

    args = args[1:]

    # Generate the command string
    if len( args ) > 0:
        command = obj + ' ' + func + ' ' + ' '.join( args ) + ';'
    else:
        command = obj + ' ' + func + ';'

    # Build a list of fields for this record with double quotes around
    # the documentation. The arguments are separated by spaces
    fields = ['Calibration', objWithoutClassName, '', '', '', '', '',
              '"' + doc + '"', command, '', 'S']

    # output the fields for this record separated by commas (csv format)
    print ','.join( fields )


# blank line
print

################ SECOND SET #################
# This time we need to replace "Calibration" with "Scripting and replace all
# args with "&", and put the args in column 10 separated by semicolons
for obj,doc,func,args in s.getServiceFunctionList():
    # If it is a new object put a blank line
    if obj != previousObject:
        print
    previousObject = obj

    # Remove newlines from documentation
    doc = removeNewlinesAndMultipleSpaces.sub( r' ', doc )

    # Remove double quotes from documentation
    doc = doc.replace( '"', '' )

    # Make a copy of the object name without the class name before it
    objWithoutClassName = removeClassName.sub( r'', obj )

    args = args[1:]

    # Generate the command string
    if len( args ) > 0:
        ampersands = '&' * len( args )
        command = obj + ' ' + func + ' ' + ' '.join( ampersands ) + ';'
    else:
        command = obj + ' ' + func + ';'

    # We need to put underscore in the description for each argument
    uscores = args[:]

    # Put semicolons after each arg for column 10
    # Also add the underscores to the end of the description
    for i in range( len( args ) ):
        args[i] = args[i] + ";"
        uscores[i] = "; ___"

    # Build a list of fields for this record with double quotes around
    # the documentation. The arguments are separated by spaces
    fields = ['Scripting', objWithoutClassName, '', '', '', '', '',
              '"' + doc + ''.join( uscores ) + '"', command, ''.join( args ), 'S']

    # output the fields for this record separated by commas (csv format)
    print ','.join( fields )







