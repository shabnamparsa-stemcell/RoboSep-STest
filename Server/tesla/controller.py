# 
# controller.py
# tesla.controller
#
# The brains of the Tesla instrument controller
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

import tesla.config 
import tesla.logger
from tesla.gateway import Gateway
from tesla.control.Centre import Centre, ControlException

from tesla.PgmLog import PgmLog    # 2011-11-23 sp -- programming logging
import os                          # 2011-11-23 sp -- programming logging

# -----------------------------------------------------------------------------

class Controller:
    """The Telsa instrument controller"""
    
    def __init__(self):
        """Instrument controller constructor"""
        self.logger = tesla.logger.Logger(logName = tesla.config.LOG_PATH)
        self.controlCentre = Centre(protocolPath = tesla.config.PROTOCOL_DIR)
        self.gateway = Gateway(self.controlCentre)
        # 2011-11-24 -- sp
        self.svrLog = PgmLog( 'svrLog' )
        self.logPrefix = 'CN'
        
    def powerUpInstrument(self):
        """Power up the instrument and perform self-checks"""
        self.logger.logInfo('Instrument power-up commenced')
        # 2011-11-24 sp -- added logging
        funcReference = __name__ + '.powerUpInstrument'
        self.svrLog.logID('', self.logPrefix, funcReference, 'start powerUpInstrument')

        try:
            self.controlCentre.resetInstrument()
        except ControlException, msg:
            self.logger.logError(msg)
            # 2011-11-24 sp -- added logging
            self.svrLog.logError('', self.logPrefix, funcReference, msg)
        
    def shutdownInstrument(self, powerDown = True):
        """Shutdown the instrument"""
        self.logger.logInfo('Instrument shutdown commenced')
        # 2011-11-24 sp -- added logging
        funcReference = __name__ + '.shutdownInstrument'
        self.svrLog.logID('', self.logPrefix, funcReference, 'start shutdownInstrument')

        try:
            self.controlCentre.shutdownInstrument(powerDown)
        except ControlException, msg:
            self.logger.logError(msg)
            self.svrLog.logError('', self.logPrefix, funcReference, msg)    # 2011-11-24 sp -- added logging
        self.logger.logInfo('Instrument shutdown')
        self.svrLog.logID('', self.logPrefix, funcReference, 'completed shutdownInstrument')    # 2011-11-24 sp -- added logging

    def startGateway(self):
        """Start the gateway"""
        self.logger.logInfo('Instrument gateway started up')
        # 2011-11-24 sp -- added logging
        funcReference = __name__ + '.startGateway'
        self.svrLog.logID('', self.logPrefix, funcReference, 'startGateway')
        self.gateway.run()

# eof

