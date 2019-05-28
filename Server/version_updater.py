# 
# version_updater.py
# 
# Simple script to update the version details in the RoboSep instrument control
# server installer package
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

import sys, copy
import tesla.config

# from ipl.interfaces.perforce import Perforce

# -----------------------------------------------------------------------------
# Configurable data

VERSION_FIELDS = ['AppVerName', 'OutputBaseFilename', 
                    'VersionInfoVersion', 'VersionInfoTextVersion']

MAJOR_LABEL = {'0': 'Pre-CDP', '1': 'CDP', '2' : 'Beta', '3' : 'PPU', '4' : 'Production'}
MINOR_LABEL = {'1': 'dev', '2': 'test', '3': 'release'}

# -----------------------------------------------------------------------------

def getVersionDetails():
    '''Return a tuple of version, the info label and the info text label'''
    #   Major release: 0 = Pre-CDP, 1 = CDP, 2 = Beta, 3 = PPU, 4 = Production
    #   Minor release: 1 = Development, 2 = Testing, 3 = Release
    #        Revision: Revisions pertinent to the current software release
    version = tesla.config.SOFTWARE_VERSION
    major, minor, revision, minorRev = version.split('.')
    
    if MAJOR_LABEL.has_key(major) and MINOR_LABEL.has_key(minor):
        infoLabel = "%s %s" % (MAJOR_LABEL[major], MINOR_LABEL[minor])
    else:
        infoLabel = "Unknown release"
    infoTextLabel = infoLabel + " version"
    return (version, infoLabel, infoTextLabel)


def readInstallerFile(filename = 'robosep.iss'):
    '''Read in the contents of the specified Inno Setup installer file and 
    return the contents as a list of lines.
    Throws an exception if we can't find specific lines.'''
    # Read in the file contents
    try:
        f = open(filename, 'r')
        data = f.readlines()
        f.close()
    except IOError, msg:
        print "\nERROR: Problem reading %s: %s" % (filename, msg)
        sys.exit(-1)

    # The lines we want to change:
    #
    # AppVerName=RoboSep Server 1.1.68 (CDP dev)
    # OutputBaseFilename=RoboSepSetup-1.1.68
    # VersionInfoVersion=1.1.68
    # VersionInfoTextVersion=CDP dev version
    #
    # If we can't find these lines, complain bitterly to the user as it means that
    # the ISS file is not complete (and it definitely should be!)
    keyFields = copy.copy(VERSION_FIELDS)
    for line in data:
        fields = line.split('=')
        if fields[0] in keyFields:
            keyFields.remove(fields[0])
    if len(keyFields) > 0:
        # We didn't find all our fields? Bitch!
        print "\nERROR: The %s installer file is missing the following lines:\n" % (filename)
        for field in keyFields:
            print "\t", field
        sys.exit(-1)
    return [line.rstrip() for line in data]
   
       
def getInstallerVersion(installerFileContents):
    '''Return the version in a list of lines read in from an installer file'''
    for line in installerFileContents:
            fields = line.split('=', 1)
            if fields[0] == 'VersionInfoVersion':
                return fields[1]
    raise ValueError, "Unable to find VersionInfoVersion line in installer file"


def convertVersionString(versionStr):
    '''Convert a version string (eg. 1.2.13) to a numeric value (eg.100200013)'''
    try:
        major, minor, rev, minorRev = [int(x) for x in versionStr.split('.')]
    except ValueError, msg:
        raise ValueError, "Invalid version string (%s): %s" % (versionStr, msg)
    return (major * 10**6 + minor * 10**3 + rev)
    

def cmpVersions(version1, version2):
    '''Returns -1, 0 or 1 based on the equivalence of the two versions passed in
    (as strings)
    Note that 1.1.69 should be greater than 1.1.7'''
    v1 = convertVersionString(version1)
    v2 = convertVersionString(version2)
    return cmp(v1, v2)
    

def createNewInstallerFile(filename, versionTuple):
    '''Create a new installer file with the latest version info, *if* required'''
    installerData = readInstallerFile(filename)
    installerVersion = getInstallerVersion(installerData)
    
    newVersion = versionTuple[0]
    versionDetails = versionTuple[1]
    print
    if cmpVersions(newVersion, installerVersion) > 0:
        print "Installer file was for v%s" % (installerVersion)
        print "Creating new installer for v%s" % (newVersion)

        # XXX - p4 edit the file
                
        # Write out the file, with all the version-related lines modified
        # for the new version
        f = open(filename, 'w')        
        modified = False
        for line in installerData:
            fields = line.split('=', 1)
            if fields[0] in VERSION_FIELDS:
                if not modified:
                    f.write("AppVerName=RoboSep Server %s (%s)\n" % (newVersion, versionDetails))
                    f.write("OutputBaseFilename=RoboSepSetup-%s\n" % (newVersion))
                    f.write("VersionInfoVersion=%s\n" % (newVersion))
                    f.write("VersionInfoTextVersion=%s version\n" % (versionDetails))
                    modified = True
            else:
                f.write(line + '\n')
        f.close()
        
        # XXX - p4 submit the file
        
    else:
        print "No change: %s installer is up-to-date for v%s" % (filename, newVersion)


# -----------------------------------------------------------------------------

if __name__ == '__main__':
    versionInfo = getVersionDetails()
    createNewInstallerFile('robosep.iss', versionInfo)

# vim:sw=4:softtabstop=4:expandtab
#

