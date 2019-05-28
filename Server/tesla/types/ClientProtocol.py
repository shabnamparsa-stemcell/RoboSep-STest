# 
# ClientProtocol.py
# tesla.types.ClientProtocol
#
# Provides a client-level protocol object that captures all of the information
# that a client needs to set up a sample
# Notes:
#   * This is a read-only object as far as the client is concerned
#   * This is a subset of the InstrumentProtocol object that contains all of
#     of the information required to run a protocol (each Protocol has a
#     matching InstrumentProtocol instance)
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
#    03/29/06 - Short Description for Sample Volume Dialog - RL

from ipl.utils import simpleHash

from tesla.exception import TeslaException
import tesla.config

# -----------------------------------------------------------------------------

class ClientProtocolException(TeslaException):
    '''The exception for ClientProtocol instances. This exception is for
    handling bad data during construction of a ClientProtocol object.'''
    pass

# -----------------------------------------------------------------------------

class ClientProtocol:
    '''Define the protocol-related information needed by clients for sample
    processing
    We pass a list of ClientProtocol objects to a client. The client then 
    gives us a list of Sample objects for scheduling, with each Sample having 
    a Protocol ID
    '''

    POSITIVE    = tesla.config.POSITIVE_PROTOCOL
    NEGATIVE    = tesla.config.NEGATIVE_PROTOCOL
    HUMANPOSITIVE    = tesla.config.HUMANPOSITIVE_PROTOCOL#CR
    HUMANNEGATIVE    = tesla.config.HUMANNEGATIVE_PROTOCOL#CR
    MOUSEPOSITIVE    = tesla.config.MOUSEPOSITIVE_PROTOCOL#CR
    MOUSENEGATIVE    = tesla.config.MOUSENEGATIVE_PROTOCOL#CR
    WHOLEBLOODPOSITIVE       = tesla.config.WHOLEBLOODPOSITIVE_PROTOCOL#CR
    WHOLEBLOODNEGATIVE       = tesla.config.WHOLEBLOODNEGATIVE_PROTOCOL#CR
    MAINTENANCE = tesla.config.MAINTENANCE_PROTOCOL
    SHUTDOWN    = tesla.config.SHUTDOWN_PROTOCOL
    TEST        = tesla.config.TEST_PROTOCOL

#CR
    LEGAL_CLASSES = [POSITIVE,
                     NEGATIVE,
                     HUMANPOSITIVE,
                     HUMANNEGATIVE,
                     MOUSEPOSITIVE,
                     MOUSENEGATIVE,
                     WHOLEBLOODPOSITIVE,
                     WHOLEBLOODNEGATIVE,
                     MAINTENANCE,
                     SHUTDOWN,
                     TEST]
   
    known = {}

    # RL - Short Description for Sample Volume Dialog -  03/29/06
    def __init__(self, protocolClass, label, description,
            minVolume_uL, maxVolume_uL = tesla.config.DEFAULT_WORKING_VOLUME_uL, 
            numQuadrants = 1):
        '''Protocol initialisation, primarily designed to be called by 
        __new__(). Requires a separation class, a (unique) identifying label, 
        a minimum sample volume (in uL), a maximum sample volume (in uL), the
        number of quadrants required by the protocol.
        The class members, as used by the interface clients are:

        int        ID
        string        label
        string        description
        double        minVol
        double        maxVol
        string        protocolClass
        int        numQuadrants
        '''
        if protocolClass not in ClientProtocol.LEGAL_CLASSES:
            raise ClientProtocolException, "Invalid separation class: %s" % (protocolClass)
        self.protocolClass = protocolClass
        
        self.label = label
        self.description = description
        self.ID = simpleHash(label)

        # Let's ensure that we don't have a hash collision - just replace the
        # old instance
        # Note that we originally had some nice code in to handle this, using
        # new-style classes and the __new__ method, but the XML-RPC library
        # can not marshal new-style objects (apparently).
        ClientProtocol.known[self.ID] = self
            
        # Sanity check the volumes
        if minVolume_uL > maxVolume_uL:
            raise ClientProtocolException, 'Min volume must be less than max volume'
        elif maxVolume_uL > tesla.config.DEFAULT_WORKING_VOLUME_uL:
            raise ClientProtocolException, "Maximum volume can not exceed %0.1f uL" % \
                    (tesla.config.DEFAULT_WORKING_VOLUME_uL)
        else:
            self.minVol = float(minVolume_uL)
            self.maxVol = float(maxVolume_uL)

        # Sanity check the quadrants 
        if numQuadrants < 0 or numQuadrants > tesla.config.NUM_QUADRANTS:
            raise ClientProtocolException, "Number of quadrants must be between 0 & %d (not %d)" % \
                    (tesla.config.NUM_QUADRANTS, numQuadrants)
        self.numQuadrants = numQuadrants

    def __str__(self):
        '''Simple string representation of the protocol.'''
        return "%s [%s]" % (self.label, self.protocolClass)

# eof

