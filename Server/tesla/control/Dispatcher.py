# 
# Dispatcher.py
# tesla.control.Dispatcher
#
# Handles dispatching of hardware events in the Tesla instrument controller
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
#    03/24/05 - added lidclosed error pause msg - bdr
#    03/29/06 - pause command - RL
#    03/14/07 - tip strip only if necessary code -RL

import itertools
import time
import threading
import re

from ipl.task.sequencer import Sequencer, SequencerException

import tesla.config
from tesla.exception import TeslaException
from tesla.control.Schedule import Schedule
from tesla.hardware.Axis import AxisError
from tesla.instrument.Instrument import InstrumentError
from tesla.instrument.Platform import Platform
from tesla.hardware.PagerSystem import PagerSystem

import RoboTrace              #CWJ Add

from tesla.PgmLog import PgmLog     # 2011-11-28 sp -- programming logging
import os                           # 2011-11-28 sp -- programming logging

# -----------------------------------------------------------------------------

class DispatcherException(TeslaException):
    '''The exception class for Dispatcher-related errors'''
    pass

# -----------------------------------------------------------------------------    

class Sequence(list):
    '''A specialisted list that allows us to sort on start time and then 
    sequence number.'''

    def getSeq(self, cmd):
        '''Return the sequence number from a command string, such as:
                Separate(0, id=1, seq=13)
        '''
        try:
            seqStr = cmd.strip(')').split('seq=')[1]
            seq = int(seqStr)
        except (IndexError, ValueError):
            seq = 0     # In case we fail, give this one priority
        return seq

    def sort(self):
        '''Custom sequence sort that sorts on time first and then the
        sequence ID that is embedded in the command string'''
        seqList = [(time, self.getSeq(cmd), cmd) for time, cmd in self]
        seqList.sort()
        self[:]= [(time, cmd) for (time, seq, cmd) in seqList]


# -----------------------------------------------------------------------------    

class Dispatcher(object):
    '''This dispatcher takes a schedule of instrument hardware-level actions 
    and executes them. It sounds so simple :)
    '''
    STATE_MSG  = 'State'    # A state-transition message type
    STATUS_MSG = 'Status'   # A status message type
    ERROR_MSG  = 'Error'    # An error message type
    
    # Our legal message types; this should be used by the reportCallback 
    # function to ensure that it's dealing with a valid message type
    MESSAGE_TYPES = [STATE_MSG, STATUS_MSG, ERROR_MSG]
 
    def __init__(self, instrument, logger, reportCallback = None):
        '''Construct a dispatcher with an Instrument object, a logger handle
        and an optional report callback (a function).
        Note that the Dispatcher reports a new state back up to the Control
        Centre (via the reportCallback). The state can be IDLE, HALTED,
        PAUSED or ESTOP.
        It is then the responsibility of the Control Centre to update the
        instrument FSM (we do not directly manipulate the instrument FSM from
        this layer).'''
        self.instrument = instrument                # The Tesla instrument object
        self.logger = logger
        self.__reportCallback = reportCallback

        # 2011-11-28 sp -- added logging
        self.svrLog = PgmLog( 'svrLog' )
        self.logPrefix = 'DP'

        self.running = False                            # Are we running?
        self.sequencer = None
        self.wasteVial = None                       # Waste vial for Prime calls
        self.debug = tesla.config.DISPATCH_DEBUG    # Internal debugging

        self.numberOfTransportsPerFlush = 1000
        
        self.pager = PagerSystem()
        print(self.pager.getInfo())


    def __onPausedHandler( self, event ):
        '''Sends the PAUSED transition request'''
        self.instrument.powerDownCarousel()
        self.pager.page('Paused.')
        
        #    03/14/07 - tip strip only if necessary code -RL
        #must pause first before moving the arm
        """try:
            self.instrument.getPlatform().StripTip()
        except AxisError, msg:          # Caught a low-level axis error
            
            errorCode = 'TSC3003' 
            errorMsg = "Manual tip strip"
            self.pager.page("Tip strip failed.")
            self.reportStatus(errorMsg, errorCode)"""
            
        self.reportTransition(self.__pauseState)

    def __preExecuteHandler( self, event ):
        '''Check the paused status of the instrument so the Sequencer knows
        if it should fire off the next action or not'''
        funcReference = __name__ + '.__preExecuteHandler'          # 2011-11-28 sp -- added logging

        if self.instrument.isPauseCommand():
            errorCode = 'TSC3003' 
            errorMsg = "Pause Command"
            errorMsg2 = self.instrument.getPauseCommandCaption()
            print("__preExecuteHandler", errorMsg)
            self.svrLog.logDebug('', self.logPrefix, funcReference, "%s [%s: %s]" % (errorCode, errorMsg, errorMsg2))   # 2011-11-28 sp -- added logging
            self.pager.page(errorMsg2)
            self.reportStatus(errorMsg, errorCode, errorMsg2)
            self.pause()
        elif not self.instrument.isLidClosed():
            errorCode = 'TSC3003' 
            errorMsg = "Lid must be closed"
            print("__preExecuteHandler", errorMsg)
            self.svrLog.logDebug('', self.logPrefix, funcReference, "%s [%s: 'Please close the lid.']" % (errorCode, errorMsg))   # 2011-11-28 sp -- added logging
            self.pager.page('Please close the lid.')
            self.reportStatus(errorMsg, errorCode)
            self.pause()
        
            
            
    def __doesActionRequirePushbackIfLate( self, event ):
        '''Returns True if the given action is one that would cause pushing
        back of the schedule if it is going to be late. Typically all actions
        except Incubate should cause pushback.'''
        if event[:len( 'Incubate' )] == 'Incubate' or \
                event[:len( 'Separate' )] == 'Separate' or \
                event[:len( 'EndOfProtocol' )] == 'EndOfProtocol' or \
                event[:len( 'Pause' )] == 'Pause' or \
                event[:len( 'Flush' )] == 'Flush':
            return False
        return True

    def __eventHandler(self, event):
        '''Handle the event that has just been dispatched to us. This is where
        the rubber meets the road...
        Note that this is running inside a Sequencer thread, so it is best to 
        call reportError(), rather than throwing an exception up.'''
        hwCall = "self.instrument.%s" % (event)
        errorHappened = True                # By default, assume an error happened
        errorMsg = 'Unknown error'        # Our default error message
        errorCode = 'TEC0101'           # Our default error code
        funcReference = __name__ + '.__eventHandler'          # 2011-11-28 sp -- added logging
        errorMsgRaw = 'Unknown error'        # Our default error message
        
        try:            
            # Report the progress (at the moment by just reporting the action)
            cmdName = event.strip("'").split("(")[0]
            self.reportStatus(cmdName, 'TSC1001')

            # Now execute the call
            self.logger.logPrintDebug("DI: %s" % (event))
            self.svrLog.logInfo('', self.logPrefix, funcReference, "%s" % (event))   # 2011-11-28 sp -- added logging
            startTime = time.time()
            exec(hwCall)

            # 2011-12-14 sp -- added reporting of id and seq ids
            idString = ''
            idStartIndex = event.find(', id=') + 2
            if( idStartIndex > 0 ):
                idIndexCount = event[ idStartIndex : ].find(',')
                idString = event[ idStartIndex : idStartIndex + idIndexCount ] + ' |'
            seqIDString = ''
            seqStartIndex = event.find(', seq=') + 2
            if( seqStartIndex > 0 ):
                seqIndexCount = event[ seqStartIndex : ].find(',')
                if( seqIndexCount < 0 ):
                    seqIndexCount = event[ seqStartIndex : ].find(')')
                seqIDString = event[ seqStartIndex : seqStartIndex + seqIndexCount ] + ' |'

            # After executing, log when it completed, except if it's a Wait class
            # command: in that case, we store the data to report later
            if cmdName in ['Incubate', 'Separate', 'Pause', 'EndOfProtocol']:
                #time.sleep(10)
                time.sleep(3) #used to be 10, don't know why, let's try 3
                duration = event.split('(')[1].split(',')[0]
                self.logger.logPrintDebug("DI: %s will complete in %s secs" % \
                                        (cmdName, duration))
                self.svrLog.logID('', self.logPrefix, funcReference, "%s%sto complete %s |duration=%s secs" % \
                                        (idString, seqIDString, cmdName, duration))   # 2011-11-28 sp -- added logging
            else:
                self.logger.logPrintDebug("DI: %s complete; duration = %0.2f secs" % \
                                        (cmdName, (time.time() - startTime)))
                self.svrLog.logID('', self.logPrefix, funcReference, "%s%scompleted %s |duration=%0.2f secs" % \
                                        (idString, seqIDString, cmdName, (time.time() - startTime)))   # 2011-11-28 sp -- added logging

            errorHappened = False        # Got to here? Then no error :)

        except AxisError as msg:          # Caught a low-level axis error
            # It's not good if this happens; usually means that a stepper card
            # has failed badly
            errorMsg = str(msg)
            errorMsgRaw = str(msg)
            errorCode = 'TEC0700'
            if str(msg) == "Tip strip failed":
                errorCode = 'TSC3003' 
                errorMsg = "Manual tip strip"
                self.pager.page("Tip strip failed.")
                self.svrLog.logError('', self.logPrefix, funcReference, "Axis Err: Tip strip failed.")   # 2011-11-28 sp -- added logging
                self.reportStatus(errorMsg, errorCode)
                self.pause()
                errorHappened = False
                CamMgr = RoboTrace.GetRoboCamMgrInstance();   #CWJ Add
                CamMgr.SetProcError();                        #CWJ Add
                self.instrument.TurnONBeacon(2);              #CWJ Add

        except (InstrumentError, TeslaException) as msg:        
                # Caught an instrument error or some other Tesla exception?
            errorMsg = str(msg)
            errorMsgRaw = str(msg)
            self.svrLog.logError('', self.logPrefix, funcReference, "Instrument Err msg=%s" % (msg))   # 2011-11-28 sp -- added logging
        except Exception as msg:                # Not good? An unanticipated error
            errorMsg = "Unexpected error: %s" % (str(msg))
            errorMsgRaw = str(msg)
            self.svrLog.logError('', self.logPrefix, funcReference, errorMsg)   # 2011-11-28 sp -- added logging
            self.pager.page(errorMsg)

        if errorHappened:
            # Log which event we were executing when the error occurred,
            # report the error and then abandon the dispatching thread
            # This will set the sequencer's "halted" flag, which will
            # move us, in __endOfRun(), to the HALTED state
            errorMsg = "DI: Error occurred during '%s'" % (event)
            self.logger.logError(errorMsg)
            self.logger.logError("DI: Error = %s" % (errorMsgRaw))
            self.svrLog.logError('', self.logPrefix, funcReference, "%s |msg=%s" % (errorMsg, errorMsgRaw))   # 2011-11-28 sp -- added logging
            self.reportError(errorMsg, errorCode)
            self.sequencer.abandonDispatching()


    def __noRun(self):
        '''Called when the sequencer has nothing to do'''
        self.running = False
        self.logger.logDebug('DI: __NoRun()')
        funcReference = __name__ + '.__noRun'          # 2011-11-28 sp -- added logging
        self.svrLog.logInfo('S', self.logPrefix, funcReference, '__NoRun()')   # 2011-11-28 sp -- added logging
        transition = self.__endState
        self.reportTransition(transition)
        

    def __endOfRun(self):
        '''Called when the Sequencer has reached the end of the run.'''
        # If we were not halted, run the instrument CleanUp method to leave
        # the system in a known state
        funcReference = __name__ + '.__endOfRun'          # 2011-11-28 sp -- added logging
        if self.sequencer.isNotHalted():
            self.logger.logDebug('DI: instrument.CleanUp() called')
            self.svrLog.logInfo('S', self.logPrefix, funcReference, 'instrument.CleanUp() called')   # 2011-11-28 sp -- added logging
            self.instrument.CleanUp()
        # Mark that we've stopped running and make the appropriate transition
        self.running = False
        self.logger.logDebug('DI: __endOfRun()')
        self.svrLog.logInfo('S', self.logPrefix, funcReference, '__endOfRun()')   # 2011-11-28 sp -- added logging
        if self.sequencer.isNotHalted():
            transition = self.__endState
        else:
            transition = self.__stoppedState
        #    03/14/07 - tip strip only if necessary code -RL
        """try:
                self.instrument.getPlatform().StripTip()
            except:
                pass"""
        self.pager.page("Protocol run completed.")
        self.reportTransition(transition)

        try:
           CamMgr = RoboTrace.GetRoboCamMgrInstance();
           CamMgr.StopCapture();
        except:
           print('\n>>> Cannot Stop Capture!\n')

    def setNumberOfTransportsPerFlush( self, number ):
        self.numberOfTransportsPerFlush = number

    def setWasteVialLocation(self, quadrant):
        '''Set the location of the waste vial for Prime calls, based on the 
        specified absolute quadrant number.'''
        if quadrant < 1 or quadrant > tesla.config.NUM_QUADRANTS:
            raise DispatcherException("Invalid quadrant number: %d" % (quadrant))
        self.wasteVial = self.instrument.ContainerAt(quadrant, self.instrument.WasteLabel)
        print("@#$@#$@#$@#$@#$@#$ setWasteVialLocation",str(self.wasteVial))


    def processSchedule(self, schedule, activeState,
                        pauseState, stoppedState, 
                        estopState, endState,
                        needsHome = True, needsPrime = True):
        '''Process the schedule of workflows for each sample at the right time.'''
        
        funcReference = __name__ + '.processSchedule'          # 2011-11-28 sp -- added logging
        CamMgr = RoboTrace.GetRoboCamMgrInstance();
        if (CamMgr.StartCapture() == True):
            VidName = CamMgr.GetVideoCaptureName();
            self.logger.logInfo('DI: Video Log File Created : %s'%VidName)
            self.svrLog.logInfo('', self.logPrefix, funcReference, 'Video Log File Created : %s'%VidName)   # 2011-11-28 sp -- added logging
        else:
            VidName = CamMgr.GetVideoCaptureName();
            self.logger.logWarning('Cannot Create Video Log File : %s'%VidName)
            self.svrLog.logWarning('', self.logPrefix, funcReference, 'Cannot Create Video Log File : %s'%VidName)   # 2011-11-28 sp -- added logging

        if self.running:
            # We should only run one Sequencer at a time
            self.svrLog.logWarning('', self.logPrefix, funcReference, "We are already running? Bailing out.")   # 2011-11-28 sp -- added logging
            raise DispatcherException("We are already running? Bailing out.")
        self.running = True

        # Keep these states for later
        self.__activeState  = activeState
        self.__pauseState   = pauseState
        self.__stoppedState = stoppedState
        self.__estopState   = estopState
        self.__endState     = endState


        # Sometimes we need to force a Home All Axes and/or Prime at the start
        if needsHome:
            self.logger.logInfo("DI: Executing HomeAllAxes command at start of run")
            self.svrLog.logID('', self.logPrefix, funcReference, "Executing HomeAllAxes command at start of run")   # 2011-11-28 sp -- added logging
            self.instrument.HomeAll()
        if needsPrime:
            if self.wasteVial:
                self.logger.logInfo("DI: Executing Prime & Flush at start of run (using %s)" % \
                                        str(self.wasteVial))
                self.svrLog.logID('', self.logPrefix, funcReference, "Executing Prime & Flush at start of run (using %s)" % \
                                        str(self.wasteVial))   # 2011-11-28 sp -- added logging
                self.instrument.Prime(self.wasteVial, homeAtEnd = False)
                self.instrument.Flush(self.wasteVial, homeAtEnd = True)
            else:
                self.logger.logWarning("DI: Waste vial not defined, can't Prime & Flush")
                self.svrLog.logWarning('', self.logPrefix, funcReference, "Waste vial not defined, can't Prime & Flush")   # 2011-11-28 sp -- added logging

        # Create the sequence, log it (for debugging) and then fire up the 
        # sequencer to execute the sequenced events
        if schedule == None:
            # If there is no schedule, just execute the __endOfRun function
            self.logger.logDebug('DI: empty sequence = no run')
            self.svrLog.logID('', self.logPrefix, funcReference, 'empty sequence = no run')   # 2011-11-28 sp -- added logging
            sequence = []
            self.sequencer = Sequencer(sequence, exitFunc = self.__noRun)

        else:
            if isinstance(schedule, Schedule):
                sequence = Dispatcher.createSequence(schedule)
            else:
                self.svrLog.logWarning( '', self.logPrefix, funcReference, "%s is not a Schedule object: can't dispatch!" % \
                        (schedule))   # 2011-11-28 sp -- added logging
                raise DispatcherException("%s is not a Schedule object: can't dispatch!" % \
                        (schedule))
        

            # Insert flushes every N transports. We keep the index of the last transport type
            # command found to prevent the last command being a flush. Flush positions are
            # stored in the list flushCommandPositions.
            nTransports = 0
            flushCommandPositions = []
            flushCommandSectors = {}
            iLastTransportCommand = 0
            recomp = re.compile("Transport|TopUpVial|Mix|ResuspendVial|MixTrans|TopUpMixTrans|ResusMixSepTrans|ResusMix|TopUpTrans|TopUpTransSepTrans|TopUpMixTransSepTrans", re.IGNORECASE )
            for i in range( len( sequence ) ):
                meth = sequence[i][1]
                print(i, meth)

                #if recomp.search(meth):
                if recomp.match(meth):
                    print("DEBUG ------------------------------ flush for ",i,meth)
                    nTransports += 1
                    iLastTransportCommand = i
                    # get sector of this transport
                
                    iLastTransportSector = re.compile("initialQuadrant=(\d)").search( meth ).group(1)

                    if nTransports >= self.numberOfTransportsPerFlush:
                        nTransports = 0
                        # mark for flush insertion
                        flushCommandPositions.append( i + 1 )
                        flushCommandSectors[i+1] = iLastTransportSector;

            # now cancel the last flush if it is after the last transport type action
            if ( len( flushCommandPositions ) > 0 and flushCommandPositions[-1] > iLastTransportCommand ):
                flushCommandPositions = flushCommandPositions[0:-2]
        
        
        


            #        print "flushCommandPositions =", flushCommandPositions
            # reverse positions so they can be inserted in the list without messing with the indices
            flushCommandPositions.reverse()

                
            # now insert the actual flushes        
            for i in flushCommandPositions:
                startTime = sequence[i-1][0]
                flushCommand = "Flush(self.instrument.ContainerAt(" + str( flushCommandSectors[i] ) + \
                               ", 'WasteVial'), id = 0, homeAtEnd = False)"
                sequence.insert( i, ( startTime + 1, flushCommand ) )

            for i in range( len( sequence ) ):
                print(i, sequence[i])


            for eventNum, event in zip(itertools.count(1), sequence):
                msg = "DI: sequence event %2d = %s" % (eventNum, event)
                self.logger.logDebug(msg)
                # 2011-12-14 sp -- added reporting of id and seq ids
                idString = ''
                idStartIndex = msg.find(', id=') + 2
                if( idStartIndex > 0 ):
                    idIndexCount = msg[ idStartIndex : ].find(',')
                    idString = msg[ idStartIndex : idStartIndex + idIndexCount ] + ' |'
                seqIDString = ''
                seqStartIndex = msg.find(', seq=') + 2
                if( seqStartIndex > 0 ):
                    seqIndexCount = msg[ seqStartIndex : ].find(',')
                    if( seqIndexCount < 0 ):
                        seqIndexCount = msg[ seqStartIndex : ].find(')')
                    seqIDString = msg[ seqStartIndex : seqStartIndex + seqIndexCount ] + ' |'
                self.svrLog.logID('', self.logPrefix, funcReference, "sequence event %2d |%s%s%s" %
                                (eventNum, idString, seqIDString, event))   # 2011-11-28 sp -- added logging

            self.sequencer = Sequencer( sequence, handler = self.__eventHandler, 
                                        exitFunc = self.__endOfRun,
                                        preExecuteHandler = self.__preExecuteHandler,
                                        onPausedHandler = self.__onPausedHandler,
                                        doesActionRequirePushbackIfLate = \
                                        self.__doesActionRequirePushbackIfLate)
            self.logger.logDebug('DI: sequencer run commencing')
            self.svrLog.logInfo('S', self.logPrefix, funcReference, 'sequencer run commencing')   # 2011-11-28 sp -- added logging



        # RL - pause command - 03/29/06
        # make sure pause command info reset before running sequence
        self.instrument.resetPauseCommand()
        
        self.logger.logInfo('DI: sequencer start run now!!!')
        self.svrLog.logID('', self.logPrefix, funcReference, 'sequencer start run now!!!')   # 2011-11-28 sp -- added logging
        self.sequencer.run()
    
    def reset(self):
        '''Reset the dispatcher. Not implemented yet (as not required) but
        left in for future generations :)'''
        return True                    # This should return success or failure
    
    
    def pause(self):
        '''Pause the run, with the option of resuming'''
      
        CamMgr = RoboTrace.GetRoboCamMgrInstance();
        CamMgr.SetProcEvent(1);
        
        print('\n>>> dispatcher pause()\n')
      
        self.logger.logInfo('DI: sequencer pause!!!')
        funcReference = __name__ + '.pause'          # 2011-11-28 sp -- added logging
        self.svrLog.logInfo('', self.logPrefix, funcReference, 'sequencer pause!!!')   # 2011-11-28 sp -- added logging

        self.sequencer.pause()
        self.instrument.TurnONBeacon(3);                    #CWJ Add        
        return True                    # This should return success or failure


    def isRunning(self):
        '''Returns True if the dispatcher is running'''
        return self.running

    def isPaused(self):
        '''Returns True if the dispatcher is paused'''
        return self.sequencer.isPaused()

    def resume(self):
        '''Resume a paused run'''
        # RL
        # make sure pause command is not active when resume (succeed or not)
        funcReference = __name__ + '.resume'          # 2011-11-28 sp -- added logging
              
        CamMgr = RoboTrace.GetRoboCamMgrInstance();
        CamMgr.SetProcEvent(2);
        print('\n>>> Dispatcher resume()\n')
        self.instrument.resetPauseCommand()
        self.instrument.TurnOFFBeacon();
        
        # check that lid is closed
        if not self.instrument.isLidClosed():
            self.reportTransition(self.__activeState)
            self.reportTransition(self.__pauseState)
            self.svrLog.logDebug('', self.logPrefix, funcReference, 'Please close the lid. (2)')   # 2011-11-28 sp -- added logging
            self.pager.page('Please close the lid. (2)')
            return False
        
        #tip strip failure check
        tipSector,tipID = self.instrument.getPlatform().CurrentTipID()
        self.svrLog.logVerbose('', self.logPrefix, funcReference, 'Tip strip sector=%s |id=%s' %(tipSector, tipID))   # 2011-11-28 sp -- added logging

        if not tipID == None:
            try:    
                #home platform before doing anything
                Platform.Initialise (self.instrument.getPlatform())
                
                self.instrument.getPlatform().StripTip()
                errorCode = 'TSC3003' 
                errorMsg = "Tip strip recovery"
                print("resume:", errorMsg)
                self.svrLog.logDebug('', self.logPrefix, funcReference, '%s [%s]' %(errorCode, errorMsg))   # 2011-11-28 sp -- added logging
                self.pager.page('Tip strip failured recovered. Please choose action.')
                self.reportStatus(errorMsg, errorCode)
            finally:
                self.reportTransition(self.__activeState)
                self.reportTransition(self.__pauseState)
                return False

              
        self.instrument.homePlatform()
        self.sequencer.resume()
        self.reportTransition(self.__activeState)
        return True                    # This should return success or failure
   
   
    def halt(self):
        '''Halt the current run.'''

        CamMgr = RoboTrace.GetRoboCamMgrInstance();
        CamMgr.SetProcEvent(3);
        
        print('\n>>> Dispatcher halt()\n')
        funcReference = __name__ + '.halt'          # 2011-11-28 sp -- added logging
        self.svrLog.logInfo('', self.logPrefix, funcReference, 'start halt process')   # 2011-11-28 sp -- added logging
        self.sequencer.halt()
        self.running = False

        self.instrument.TurnOFFBeacon();

        return True                    # This should return success or failure
   
   
    def estop(self):
        '''Force an E-Stop of the current run'''
        
        CamMgr = RoboTrace.GetRoboCamMgrInstance();
        CamMgr.SetProcEvent(4);
        
        print('\n>>> Dispatcher estop()\n')
        funcReference = __name__ + '.estop'          # 2011-11-28 sp -- added logging
        self.svrLog.logInfo('', self.logPrefix, funcReference, 'start estop process')   # 2011-11-28 sp -- added logging
        
        self.sequencer.halt()
        self.running = False

        self.reportTransition(self.__estopState)
        return True                    # This should return success or failure

    # --- reporting methods ---

    def reportTransition(self, transition):
        '''Helper function for reporting state transition requests'''
        if transition == 'PAUSED' and self.instrument.isPauseCommand():
            transition = 'PAUSECOMMAND'
        self.__reportMsg(Dispatcher.STATE_MSG, transition, None)
        
    def reportStatus(self, msg, statusCode = 'TSC1000', msg2 = None):
        '''Simple helper function for reporting status messages.
        The default status code is TSC1000 (Unspecified Dispatcher status)'''
        # We don't use field1, so just set it to None
        if msg2 == None:
            self.__reportMsg(Dispatcher.STATUS_MSG, None, statusCode, msg)
        else:
            self.__reportMsg(Dispatcher.STATUS_MSG, None, statusCode, msg, msg2)

    def reportError(self, msg, errorCode = 'TEC0101'):
        '''Simple helper function for reporting error messages.
            The default error code is TEC0101 (Unspecified Instrument hardware layer error)'''

        CamMgr = RoboTrace.GetRoboCamMgrInstance();       #CWJ Add
        CamMgr.SetProcError();                            #CWJ Add
        self.instrument.TurnONBeacon(1);                  #CWJ Add
                
        VidErrorFile = CamMgr.GetErrorVideoCaptureName();
        self.logger.logError('Error video Log was created : %s'%VidErrorFile); 
        funcReference = __name__ + '.reportError'          # 2011-11-28 sp -- added logging
        self.svrLog.logError('', self.logPrefix, funcReference, 'Error video Log was created : %s'%VidErrorFile)   # 2011-11-28 sp -- added logging
        self.__reportMsg(Dispatcher.ERROR_MSG, tesla.config.ERROR_LEVEL, errorCode, msg)


    def __reportMsg(self, type, field1, *args):
        '''Report a message (of type MESSAGE_TYPES) to the callback.
        This should be thread safe :)'''
        lck = threading.Lock()
        lck.acquire()
        if self.__reportCallback:
            self.__reportCallback(type, field1, *args)
        lck.release()

    # --- static methods ---

    def createSequence(schedule):
        '''Create (and return) a sequence from the supplied Schedule object.'''
        seq = Sequence()
        for workflow in schedule.getWorkflows():
            cmd, startTime = workflow.getWorkflowAndTime()
            seq.append((startTime, cmd))
        return seq

    createSequence = staticmethod(createSequence)

# eof

