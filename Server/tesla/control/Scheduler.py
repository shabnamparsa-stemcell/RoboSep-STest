# 
# Scheduler.py
# tesla.control.Scheduler
#
# Schedule samples and protocols in the Tesla instrument controller
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

import os
import time
from datetime import datetime
from xmlrpc.client import DateTime

from ipl.scheduler.Scheduler import TimeBlock
from ipl.scheduler.Scheduler import Scheduler as IPL_Scheduler

import tesla.config
from tesla.exception import TeslaException
from tesla.control.Schedule import Schedule
from tesla.types.Command import Command, CommandException

from tesla.PgmLog import PgmLog    # 2011-11-23 sp -- programming logging


from tesla.instrument.Instrument import Instrument

from tesla.types.XmlRpcProtocolCommand import XmlRpcProtocolCommand

# -----------------------------------------------------------------------------

class SchedulerException(TeslaException):
    '''Tesla scheduling exception class'''
    pass

# -----------------------------------------------------------------------------

class SchedulerBlock(TimeBlock):
    '''This is an enhanced version of TimeBlock with a nicer constructor and 
    some helper functions that hide us from the internals (in case they
    change).
    This is for use by the SampleScheduler class (and perhaps unit testing).'''
    
    def __init__(self, openPeriod, usedPeriod, freePeriod, startTime = 0):
        '''Construct a block with the three schedule periods and an optional
        start time (which isn't used?).'''
        TimeBlock.__init__(self)
        for number in [openPeriod, usedPeriod, freePeriod, startTime]:
            # Verify that we're not trying to work with negative time
            if number < 0:
                raise SchedulerException("SB: Time value (%d) is invalid for SchedulerBlock" % (number))
        self.m_OpenPeriod = openPeriod
        self.m_UsedPeriod = usedPeriod
        self.m_FreePeriod = freePeriod
        self.m_StartTime  = startTime

    def __repr__(self):
        '''String representation of the SchedulerBlock object, to aid debugging.'''
        return "(%d, %d, %d, %d)" % (self.m_OpenPeriod, self.m_UsedPeriod, self.m_FreePeriod, self.m_StartTime)

    def getPeriods(self):
        '''Return a tuple of (open, used and free) times '''
        return (self.m_OpenPeriod, self.m_UsedPeriod, self.m_FreePeriod)

    def getStartTime(self):
        '''Return the start time (in seconds)'''
        return self.m_StartTime

    def getFreePeriod(self ):
        '''Get the free period (in seconds)'''
        return self.m_FreePeriod

    def getOpenPeriod( self ):
        '''Get the open period (in seconds)'''
        return self.m_OpenPeriod

    def getUsedPeriod( self ):
        '''Get the used period (in seconds)'''
        return self.m_UsedPeriod

    def setFreePeriod(self, period_secs):
        '''Set the free period (in seconds)'''
        self.m_FreePeriod = period_secs

    def setOpenPeriod(self, period_secs):
        '''Set the open period (in seconds)'''
        self.m_OpenPeriod = period_secs

    def setStartTime( self, time_secs ):
        '''Set the start time (in seconds)'''
        self.m_StartTime = time_secs

# -----------------------------------------------------------------------------

class SampleScheduler(object):
    '''Adapter that takes the generic scheduler component and makes it
    easy to use for sample/protocol scheduling.

    To use this class:

        pm = ProtocolManager()
        logger = tesla.logger.Logger(...)
        scheduler = SampleScheduler(pm, logger)

        scheduler.schedule(sampleList)
        sampleSchedule = scheduler.getSchedule()
        timeToCompletion = scheduler.getETC()

        print "Processing should be complete by %s" % (timeToCompletion)
        dispatcher.processSchedule(sampleSchedule)
    '''

    # schedulerTimes is a map that defines the open, used and free time for 
    # each of our schedulable actions.
    #
    # Notes: * This is temporary code for getting the CDP up and running.
    #             * These values were obtained from AED (email, 15 March 2004)
    #             * We're not using the sample information at the moment
    
    # open, used, free
    schedulerTimes = {        'Transport'        : (  0,  28,  0),
                        'Mix'                : (  0,  28,  0),
                        'TopUpVial'        : (  0,  35,  0),
                        'ResuspendVial'        : (  0,  35,  0),
                        'Flush'                : (  0,  10,  0),
                        'Prime'                : (  0,  25,  0),
                        'HomeAll'        : (  0,  30,  0),
                        'Park'            : (  0,  10,  0),
                        'Demo'                : (  0, 200,  0),
                        'PumpLife'        : (  0, 200,  0),
                        'MixTrans'      : (  0, 200,  0),
                        'TopUpMixTrans' : (  0, 200,  0),
                        'ResusMixSepTrans'    : (  0, 200,  0),
                        'ResusMix'    : (  0, 200,  0),
                        'TopUpTrans' : (  0, 200,  0),
                        'TopUpTransSepTrans' : (  0, 200,  0),
                        'TopUpMixTransSepTrans' : (  0, 200,  0),
                    }
    skipWarningList = ['Transport','Mix','TopUpVial','ResuspendVial',
                       'MixTrans','TopUpMixTrans','ResusMixSepTrans','ResusMix',
                       'TopUpTrans','TopUpTransSepTrans','TopUpMixTransSepTrans']
    

    transportTimesSmallTip = ()
    transportTimesLargeTip = ()
    mixTimes = ()
    endTimeSpan = None
    endTimeSpanThreshold = None

    schedulerEndTimeSpan1 = None
    schedulerEndTimeSpan2 = None

    numberOfTransportsPerFlush = 1000

    noTipStripTimeDiscount = 10
    noTipPickUpTimeDiscount = 10

    # This is the delay (our first open time) at the start of the schedule to
    # ensure that there is sufficient flexibility in the schedule

  
    MAX_ITERATIONS = 1000            # Maximum number of iterations in a schedule calc
    NUM_SCHEDULES  = 1                    # Number of schedules calculated internally
    SEARCH_SPACE_MULTIPLIER = 1            # Failed? Increase the search space by this much & try again



   
    def __init__(self, protocolManager, logger, instrument=None):
        '''Constructor for the sample/protocol scheduler. Two parameters: 
            - an instance of the protocol manager
            - an instance of the Tesla event/error logger

        Note that we pass in a Logger instance, but only for recording 
        information to help us with tracing errors and debugging. Any critical
        error still needs to throw an exception or return a value that indicates
        failure (eg. schedule() returns True or False, depending on the status
        of the scheduling effort).
            To enable debugging, set the TESLA_SCHEDULER_DEBUG environment 
            variable, as defined in the tesla.config module.
            '''

        self.debug = tesla.config.SCHEDULER_DEBUG        
        self.scheduler = IPL_Scheduler()            
        self.protocolMgr = protocolManager  # A protocol manager instance
        self.logger = logger                    # A tesla.logger instance
        self.instrument = instrument

        # 2011-11-25 sp -- added logging
        self.svrLog = PgmLog( 'svrLog' )
        self.logPrefix = 'SH'

        # Get the scheduler times for each workflow command
        self.getSchedulerTimes(tesla.config.CMD_TIME_PATH)
        self.reset()                            # Create & reset other members

    def getNumberOfTransportsPerFlush( self ):
        return self.numberOfTransportsPerFlush
    

    def getSchedulerTimes(self, timeFile = None):
        '''Return a map of the scheduler times for each workflow command. If 
        timeFile is defined, we try to read in the data from that file (which
        contains a Pythonic map definition; otherwise, we use the default times
        defined in the tesla.control.schedulerTimes module.'''
        funcReference = __name__ + '.getSchedulerTimes'  # 2011-11-24 sp -- added logging
        timesSet = False
        if timeFile and os.path.exists(timeFile):
            try:
                safespace = {}
                exec(compile(open(timeFile, "rb").read(), timeFile, 'exec'), safespace)
                times = safespace['times']
                transportTimesSmallTip = safespace['transportTimesSmallTip']        
                transportTimesLargeTip = safespace['transportTimesLargeTip']
                mixTimes = safespace['mixTimes']
                schedulerEndTimeSpan1 = safespace['schedulerEndTimeSpan1']
                schedulerEndTimeSpan2 = safespace['schedulerEndTimeSpan2']
                self.numberOfTransportsPerFlush = safespace['numberOfTransportsPerFlush']
                self.noTipStripTimeDiscount = safespace['noTipStripTimeDiscount']
                self.noTipPickUpTimeDiscount = safespace['noTipPickUpTimeDiscount']
                timesSet = True
                self.logger.logInfo("SS: Import user-defined command times from %s" % (timeFile))
                self.svrLog.logID('', self.logPrefix, funcReference, "Import user-defined command times from %s" % (timeFile) )    # 2011-11-24 sp -- added logging
            except Exception as msg:
                self.logger.logWarning("SS: Unable to import user-defined command times from %s: %s; using defaults" % \
                    (timeFile, msg))
                self.svrLog.logWarning('', self.logPrefix, funcReference, "Unable to import user-defined command times from %s |mag=%s; using defaults" % \
                    (timeFile, msg) )    # 2011-11-24 sp -- added logging

        if not timesSet:
            # No success? Then use our in-built defaults for the various times
            times = SampleScheduler.schedulerTimes
            # These next set of values are lifted from cmd_times.ini as of 6 July 2004
            transportTimesSmallTip = (12, 45), (180, 65), (181, 55), (454, 75), (455, 45), (1000, 45)
            transportTimesLargeTip = (100, 45), (3000, 45), (3001, 45), (5000, 45)
            mixTimes = (100, 40), (3600, 65)
            schedulerEndTimeSpan1 = 600
            schedulerEndTimeSpan2 = 1200
        else:
            # Let's make sure we have all of the required keys (if we imported
            # our scheduler times from a timeFile)
            for cmd in SampleScheduler.schedulerTimes:
                if cmd not in times:
                    if not cmd in SampleScheduler.skipWarningList:
                        self.logger.logWarning("%s is missing scheduler times for the %s command" %\
                            (timeFile, cmd))
                        self.svrLog.logWarning('', self.logPrefix, funcReference, "Missing scheduler times in file=%s |command=%s" %\
                            (timeFile, cmd) )    # 2011-11-24 sp -- added logging
                    times[cmd] = SampleScheduler.schedulerTimes[cmd]
        
        self.schedulerTimes = times
        self.transportTimesSmallTip = transportTimesSmallTip
        self.transportTimesLargeTip = transportTimesLargeTip                    
        self.mixTimes = mixTimes
        self.schedulerEndTimeSpan1 = schedulerEndTimeSpan1
        self.schedulerEndTimeSpan2 = schedulerEndTimeSpan2

    def reset(self):
        '''Reset the scheduler and associated members.'''

        self.scheduler.Reset()
        #self.scheduler.SetNbrSchedules(self.NUM_SCHEDULES)
        self.scheduler.SetMaxIterations(self.MAX_ITERATIONS)
        self.__duration_secs = 0            # Last calculated schedule duration
        self.__schedule = None                    # Last calculated schedule
        self.__batchIDList = []                    # Our list of batch IDs for scheduling
        self.__blockIDs = {}                    # Dictionary of block IDs for samples

    def getSchedule(self):
        '''Returns a Schedule object, suitable for dispatching to the hardware
        layer. Returns None if there is no schedule.'''
        return self.__schedule

    def getETC(self, padding = 0):
        '''Return the estimated time to completion of the last schedule. The
        time format is modified ISO 8601, as documented in tesla.interface.
        Returns the time *now* if there is no schedule (ie. duration = 0).
            If padding is set to a positive integer, then this is added to the ETC.
        '''
        # The ETC is actually the time now + the scheduled duration + any padding
        funcReference = __name__ + '.getETC'    # 2011-11-24 sp -- added
        if padding > 0:
            self.logger.logDebug("SS: ETC padding = %d secs" % (padding))
            self.svrLog.logInfo('S', self.logPrefix, funcReference, "ETC padding (secs)=%d" % (padding) )    # 2011-11-24 sp -- added logging


        timeStamp = datetime.fromtimestamp(time.time() + self.getDuration(padding))

        isoTimeForXMLRPC = timeStamp.isoformat().replace('-','')
        decimalPointPos = isoTimeForXMLRPC.find( '.' )
        if decimalPointPos > 0:                
            isoTimeForXMLRPC = isoTimeForXMLRPC[:decimalPointPos]

        # A line of debugging to help with TT #473
        self.logger.logDebug("SS: getETC() ISO time = %s" % (isoTimeForXMLRPC))
        self.svrLog.logInfo('S', self.logPrefix, funcReference, "getETC() ISO time = %s" % (isoTimeForXMLRPC) )    # 2011-11-24 sp -- added logging
        return DateTime(isoTimeForXMLRPC)


    def getDuration(self, padding = 0):
        '''Return the duration of our schedule (in seconds).
        If padding is set to a positive integer, then this is added to the 
        duration.'''
        return self.__duration_secs + max(0, padding)

    def schedule(self, sampleList, sharedSectorsTranslation = []):
        '''Schedule the protocol commands associated with samples in our 
        sample list.
        Returns True if the scheduling is successful, False otherwise.'''

        funcReference = __name__ + '.schedule'  # 2011-11-24 sp -- added
        self.reset()
        self.endTimeSpanThreshold = self.schedulerEndTimeSpan1
        self.appendBlocks(sampleList)
        self.logger.logDebug("SS: new calculateTimes() called")
        self.svrLog.logInfo('S', self.logPrefix, funcReference, " new calculateTimes() called" )    # 2011-11-24 sp -- added logging
        schedulingSuccess = self.scheduler.CalculateTimes()
        self.logger.logDebug("SS: new calculateTimes() finished")
        self.svrLog.logDebug('', self.logPrefix, funcReference, " new calculateTimes() finished" )    # 2011-11-24 sp -- added logging
        
        if not schedulingSuccess:

            # If it fails, increase the open times at the start for each protocol
            self.logger.logWarning("SS: Attempt 1: Unable to schedule samples %s" % \
                (SampleScheduler.getSampleInfo(sampleList)))
            self.svrLog.logWarning('', self.logPrefix, funcReference, "Attempt 1: Unable to schedule samples %s" % \
                (SampleScheduler.getSampleInfo(sampleList)) )    # 2011-11-24 sp -- added logging

            self.reset()
            self.scheduler.SetMaxIterations( self.MAX_ITERATIONS * 2 )
            self.endTimeSpanThreshold = self.schedulerEndTimeSpan2
            self.appendBlocks(sampleList)
            schedulingSuccess = self.scheduler.CalculateTimes()

            if not schedulingSuccess:
            # Failed again? Then abort the scheduling effort
                msg = "Failed to schedule samples %s" % (SampleScheduler.getSampleInfo(sampleList))
                self.logger.logError("SS: Attempt 2: %s" % msg)
                self.svrLog.logError('', self.logPrefix, funcReference, "Attempt 2: %s" % msg )    # 2011-11-24 sp -- added logging
                raise SchedulerException(msg)
                return schedulingSuccess


        # Successful scheduling!

        # The schedule is turned back around in __getWorkflowSchedule(...)

        self.__schedule = self.__getWorkflowSchedule(sampleList, sharedSectorsTranslation)
  
        # Calculate the duration (from which we will determine the ETC)
        # Then create the scheduled list of "workflow containers" that we 
        # will dispatch down to the instrument hardware layer for execution
        self.__duration_secs = self.calculateDuration()
        
        self.logger.logInfo("SS: Scheduled %d samples (%s). Duration = %d secs." % \
                            (len(sampleList), SampleScheduler.getSampleInfo(sampleList), 
                             self.__duration_secs))
        self.svrLog.logID('', self.logPrefix, funcReference, "Scheduled samples=%d |list=%s |Duration(secs)=%d" % \
                            (len(sampleList), SampleScheduler.getSampleInfo(sampleList), 
                             self.__duration_secs) )    # 2011-11-24 sp -- added logging
        return schedulingSuccess
  
    # --- Scheduling helper functions -----------------------------------------

    def appendBlocks(self, sampleList):
        '''For each sample in the sampleList parameter, unravel it's list of
        blocks for scheduling and append them to the scheduler's block list,
        in readiness for scheduling.'''
        funcReference = __name__ + '.appendBlocks'      # 2011-12-09 sp -- added logging

        self.xmlRpcProtocols = []

        for sample in sampleList:
            # Our batch ID will be the sample ID, we store our batch IDs and
            # then also store the block IDs for each batch ID (in a dictionary)
            # with the batch ID as the key
            
            batchID = sample.ID
            self.__batchIDList.append(batchID)

            blockList = self.getBlockList(sample)

            self.xmlRpcProtocols.append(self.lastXmlRpcProtocolCommands)

            self.__blockIDs[batchID] = []
            for block in blockList:
                blockID = self.scheduler.AppendBlock(batchID, block)
                if blockID == -1:
                    self.svrLog.logWarning('', self.logPrefix, funcReference, "AppendBlock() failed; batchID=%d, block=%d" % \
                        (batchID, block))    # 2011-12-09 sp -- added logging
                    raise SchedulerException("SS: AppendBlock() failed; batchID=%d, block=%d" % \
                        (batchID, block))
                else:
                    self.__blockIDs[batchID].append(blockID)


    def getVialCapacity(self, vial):
        if not self.instrument == None:
            #print "\n\n-------------------getVialCapacity"
            sector = vial[0]
            vialName = vial[1]
            #print "----------------",sector,vialName
            container = self.instrument.ContainerAt (sector, vialName)
            #print "----------------",container.getMaxVolume()
            return container.getMaxVolume()
        return 50000


    def getMixVolAndTime(self, cmd, stage_num, sample_vol, vial, vial_volume_tracker):
        (calc_vol, isRelative, prop ) = cmd.getCalcVolAndRelativeAndProportion(stage_num,sample_vol)

        #for absolute volume, cmd.calculateVolume(sample.volume) is still good
        #for relative volume if cmd.relative then vol = cmd.proportion * vial_tracker[dest vial]
        mix_vol = calc_vol
        if isRelative:
            mix_vol = prop * vial_volume_tracker.get(vial,0)
        mix_usedCT = self.__getInterpolatedValue( mix_vol, self.mixTimes )

        return (mix_vol, mix_usedCT)
    
    def getTransVolAndTime(self, cmd, stage_num, sample_vol, src_vial, dest_vial, vial_volume_tracker):
        (calc_vol, isRelative, prop ) = cmd.getCalcVolAndRelativeAndProportion(stage_num,sample_vol)

        trans_vol = calc_vol
        if src_vial[1] in (Instrument.LysisLabel,Instrument.SampleLabel,Instrument.SupernatentLabel):
            trans_vol = min(vial_volume_tracker.get(src_vial,0), trans_vol) #adjust transport volume to what is possible
        vial_volume_tracker[src_vial] = vial_volume_tracker.get(src_vial,0) - trans_vol  #update dic with new volume src
        vial_volume_tracker[dest_vial] = vial_volume_tracker.get(dest_vial,0) + trans_vol  #update dic with new volume src

        if src_vial[1] in (Instrument.LysisLabel,Instrument.BCCMLabel,\
                                              Instrument.SampleLabel,Instrument.SupernatentLabel):
            trans_usedCT = self.__getInterpolatedValue( trans_vol, self.transportTimesLargeTip )
        else:
            trans_usedCT = self.__getInterpolatedValue( trans_vol, self.transportTimesSmallTip )

        return (trans_vol, trans_usedCT)


    def getTopUpVolToAddAndTime(self, cmd, stage_num, sample_vol, work_vol, src_vial, dest_vial, vial_volume_tracker):
        (calc_vol, isRelative, prop ) = cmd.getCalcVolAndRelativeAndProportion(stage_num,sample_vol)

        #COPIED code from Command.py
        if isRelative:
            workVolume = calc_vol + sample_vol
            workVolume = min(10000,workVolume)
        else:
            workVolume = work_vol
        topupVolume = min(self.getVialCapacity(dest_vial), workVolume)
        volumeToAdd = topupVolume - vial_volume_tracker.get(dest_vial,0)
        topup_usedCT = self.__getInterpolatedValue( volumeToAdd, self.transportTimesLargeTip ) 
        vial_volume_tracker[src_vial] = vial_volume_tracker.get(src_vial,0) - volumeToAdd      #TODO: didn't check if enough  
        vial_volume_tracker[dest_vial] = vial_volume_tracker.get(dest_vial,0) + volumeToAdd
        
        return (volumeToAdd, topup_usedCT)

                        
    
    def getBlockList(self, sample):
        '''Get a list of ScheduleBlock objects for the protocol associated with
        this Sample object.'''
        funcReference = __name__ + '.getBlockList'      # 2011-12-09 sp -- added logging
        blockList = []
        openTime = 0

        self.lastXmlRpcProtocolCommands = []

        # Retrieve the protocol associated with the sample
        protocol = self.protocolMgr.getProtocol(sample.protocolID)
        if protocol:
            tmpCmd = protocol.getCommands()[0] #hack: assume protocol have at least 1 command, should make the call static 
            consumables = protocol.getVolumes(sample.volume, protocol.type)
            sector = sample.initialQuadrant
            
            sample_vial_string = 'TPC0%d06' % (sector,)
            vial_volume_tracker = { tmpCmd._parseVialInfo(sample_vial_string):max(0, sample.volume)}
            for pc in consumables:
                index = (sector + pc.quadrant) - 1
                lysis_vial_string = 'TPC0%d02' % (index,)
                vial_volume_tracker[tmpCmd._parseVialInfo(lysis_vial_string)] = max(0, pc.lysisVolume)  #TPC0102
                
            for cmd in protocol.getCommands():
                seq = cmd.seq
                #print ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> >>>>>>>>>>>>>>>>>  100"
                if cmd.isWaitType():
                    incubationTime = cmd.minPeriod
                    blockTime = incubationTime
                    block = SchedulerBlock(openTime, 0, blockTime)
                    blockList.append(block)
                    xmlRpcCmd = XmlRpcProtocolCommand(cmd.type,blockTime,cmd.label)
                    self.lastXmlRpcProtocolCommands.append(xmlRpcCmd)
                    self.logger.logInfo( "Schedule Wait Type ID=%d (%d %d %d)" % (seq,openTime,0,blockTime))
                    # Finally set the open time for the next block
                    openTime = cmd.maxPeriod - cmd.minPeriod
                #else: # Is volume type
                elif cmd.isVolumeType():
                    (openCT, usedCT, freeCT) = self.getCommandTimes(cmd)
                    xmlRpcCmd_time = usedCT
         #                   print cmd, self.getCommandTimes(cmd)
                         # it could be a mix or transport which means we need to add the extended time
                        # depending on the volume and tip size

                    #RL Feb26 2015 Bug fix for wrong sample volume used for interpolation when same protocol used more than once
                    #fix is to do update transport volume with correct sample volume
                    #vol = cmd.calculateVolume(sample.volume)
                    if cmd.isTransportType():
                        (vol, usedCT) = self.getTransVolAndTime(cmd, 1, sample.volume, cmd.srcVial, cmd.destVial, vial_volume_tracker)
                        self.logger.logInfo( "Schedule Transport Type ID=%d (%d %d %d) vol=%d" % (seq,openTime, usedCT, freeCT,vol))
                        block = SchedulerBlock(openTime, usedCT, freeCT)
                        xmlRpcCmd_time = usedCT
                    #RL Feb26 2015 Bug fix mixTimes NOT USED AT ALL!!!
                    elif cmd.isMixType():
                        (vol, usedCT) = self.getMixVolAndTime(cmd, 1, sample.volume, cmd.srcVial, vial_volume_tracker)
                        self.logger.logInfo( "Schedule Mix Type ID=%d (%d %d %d) vol=%d" % (seq,openTime, usedCT, freeCT,vol))
                        block = SchedulerBlock(openTime, usedCT, freeCT)
                        xmlRpcCmd_time = usedCT
                    elif cmd.isTopUpType() or cmd.isResuspendType():
                        (volumeToAdd, usedCT) = self.getTopUpVolToAddAndTime(cmd, 1, sample.volume, cmd.getWorkingVolume(sample, protocol),
                                                                             cmd.srcVial, cmd.destVial, vial_volume_tracker)
                        
                        if cmd.isTopUpType():
                            self.logger.logInfo( "Schedule TopUp Type ID=%d (%d %d %d) volumeToAdd=%d" % (seq,openTime, usedCT, freeCT,volumeToAdd))
                        else:
                            self.logger.logInfo( "Schedule Resuspend Type ID=%d (%d %d %d) volumeToAdd=%d" % (seq,openTime, usedCT, freeCT,volumeToAdd))
                        block = SchedulerBlock(openTime, usedCT, freeCT)
                        xmlRpcCmd_time = usedCT

                    elif cmd.isMixTransType():
                        (mix_vol, mix_usedCT) = self.getMixVolAndTime(cmd, 1, sample.volume, cmd.srcVial, vial_volume_tracker)
                        mix_usedCT = mix_usedCT - self.noTipStripTimeDiscount 

                        (trans_vol, trans_usedCT) = self.getTransVolAndTime(cmd, 2, sample.volume, cmd.srcVial, cmd.destVial, vial_volume_tracker)
                        trans_usedCT = trans_usedCT - self.noTipPickUpTimeDiscount

                        self.logger.logInfo( "Schedule MixTrans Type ID=%d (%d %d %d) mix-strip=%d trans-pick=%d trans_vol=%d" % \
                                             (seq,openTime, mix_usedCT+trans_usedCT, freeCT,mix_usedCT,trans_usedCT,trans_vol))
                        block = SchedulerBlock(openTime, mix_usedCT+trans_usedCT, freeCT)
                        xmlRpcCmd_time = mix_usedCT+trans_usedCT

                    elif cmd.isTopUpMixTransType() or cmd.isResusMixSepTransType() or cmd.isTopUpMixTransSepTransType():
                        (volumeToAdd, topup_usedCT) = self.getTopUpVolToAddAndTime(cmd, 1, sample.volume, cmd.getWorkingVolume(sample, protocol),
                                                                             cmd.srcVial,cmd.destVial, vial_volume_tracker)
                        topup_usedCT = topup_usedCT - self.noTipStripTimeDiscount

                        (mix_vol, mix_usedCT) = self.getMixVolAndTime(cmd, 2, sample.volume, cmd.destVial, vial_volume_tracker)
                        mix_usedCT = mix_usedCT - self.noTipPickUpTimeDiscount - self.noTipStripTimeDiscount

                        (trans_vol, trans_usedCT) = self.getTransVolAndTime(cmd, 3, sample.volume, cmd.srcVial2, cmd.destVial2, vial_volume_tracker)
                        trans_usedCT = trans_usedCT - self.noTipPickUpTimeDiscount

                        wait_time = 0
                        trans2_usedCT = 0
                        trans2_vol = 0
                        if cmd.isTopUpMixTransType():
                            cmd_string = "TopUpMixTrans"
                        elif cmd.isResusMixSepTransType():
                            cmd_string = "ResusMixSepTrans"
                            wait_time = cmd.duration
                        else: #cmd.isTopUpMixTransSepTransType()
                            trans_usedCT = trans_usedCT - self.noTipStripTimeDiscount
                            cmd_string = "TopUpMixTransSepTrans"
                            wait_time = cmd.duration
                            (trans2_vol, trans2_usedCT) = self.getTransVolAndTime(cmd, 4, sample.volume, cmd.srcVial3, cmd.destVial3, vial_volume_tracker)
                            trans2_usedCT = trans2_usedCT - self.noTipPickUpTimeDiscount
                            
                            
                        self.logger.logInfo( "Schedule %s Type ID=%d (%d %d %d) top-strip=%d mix-strip-pick=%d trans-pick=%d trans2-pick=%d volumeToAdd=%d mix_vol=%d trans_vol=%d trans2_vol=%d" % \
                                             (cmd_string,seq,openTime, topup_usedCT + mix_usedCT + trans_usedCT + trans2_usedCT + wait_time, freeCT,topup_usedCT,mix_usedCT,trans_usedCT,trans2_usedCT,
                                              volumeToAdd,mix_vol,trans_vol,trans2_vol))
                        block = SchedulerBlock(openTime, topup_usedCT + mix_usedCT + trans_usedCT +trans2_usedCT+ wait_time, freeCT)
                        xmlRpcCmd_time = topup_usedCT + mix_usedCT + trans_usedCT +trans2_usedCT+ wait_time
                    elif cmd.isTopUpTransType() or cmd.isTopUpTransSepTransType():
                        (volumeToAdd, topup_usedCT) = self.getTopUpVolToAddAndTime(cmd, 1, sample.volume, cmd.getWorkingVolume(sample, protocol),
                                                                             cmd.srcVial,cmd.destVial, vial_volume_tracker)
                        topup_usedCT = topup_usedCT - self.noTipStripTimeDiscount

                        (trans_vol, trans_usedCT) = self.getTransVolAndTime(cmd, 2, sample.volume, cmd.srcVial2, cmd.destVial2, vial_volume_tracker)
                        trans_usedCT = trans_usedCT - self.noTipPickUpTimeDiscount

                        cmd_string = "TopUpTrans"

                        wait_time = 0
                        trans2_usedCT = 0
                        trans2_vol = 0
                        if cmd.isTopUpTransSepTransType():
                            trans_usedCT = trans_usedCT - self.noTipStripTimeDiscount
                            cmd_string = "TopUpTransSepTrans"
                            wait_time = cmd.duration
                            (trans2_vol, trans2_usedCT) = self.getTransVolAndTime(cmd, 3, sample.volume, cmd.srcVial3, cmd.destVial3, vial_volume_tracker)
                            trans2_usedCT = trans2_usedCT - self.noTipPickUpTimeDiscount
                            
                            
                        self.logger.logInfo( "Schedule %s Type ID=%d (%d %d %d) top-strip=%d trans-pick=%d trans2-pick=%d volumeToAdd=%d trans_vol=%d trans2_vol=%d" % \
                                             (cmd_string,seq,openTime, topup_usedCT + trans_usedCT + trans2_usedCT + wait_time, freeCT,topup_usedCT, trans_usedCT, trans2_usedCT,
                                              volumeToAdd,trans_vol,trans2_vol))
                        block = SchedulerBlock(openTime, topup_usedCT + trans_usedCT + trans2_usedCT + wait_time, freeCT)
                        xmlRpcCmd_time = topup_usedCT + trans_usedCT + trans2_usedCT + wait_time
                    elif cmd.isResusMixType():
                        (volumeToAdd, topup_usedCT) = self.getTopUpVolToAddAndTime(cmd, 1, sample.volume, cmd.getWorkingVolume(sample, protocol),
                                                                             cmd.srcVial,cmd.destVial, vial_volume_tracker)
                        topup_usedCT = topup_usedCT - self.noTipStripTimeDiscount

                        (mix_vol, mix_usedCT) = self.getMixVolAndTime(cmd, 2, sample.volume, cmd.destVial, vial_volume_tracker)
                        mix_usedCT = mix_usedCT - self.noTipPickUpTimeDiscount 
                        
                        self.logger.logInfo( "Schedule ResusMix Type ID=%d (%d %d %d) resus-strip=%d mix-pick=%d volumeToAdd=%d mix_vol=%d " % \
                                             (seq,openTime, topup_usedCT + mix_usedCT, freeCT,topup_usedCT,mix_usedCT,
                                              volumeToAdd,mix_vol))
                        block = SchedulerBlock(openTime, topup_usedCT + mix_usedCT, freeCT)
                        xmlRpcCmd_time = topup_usedCT + mix_usedCT

                    else:
                        self.logger.logInfo( "Schedule unaccounted Type ID=%d (%d %d %d) use default" % \
                                             (seq,openTime, usedCT, freeCT))
                        block = SchedulerBlock(openTime, usedCT, freeCT)
                        xmlRpcCmd_time = usedCT
                        



                    blockList.append(block)
                    xmlRpcCmd = XmlRpcProtocolCommand(cmd.type,xmlRpcCmd_time,cmd.label)
                    self.lastXmlRpcProtocolCommands.append(xmlRpcCmd)
                    openTime = cmd.extensionTime
                else:
                    (openCT, usedCT, freeCT) = self.getCommandTimes(cmd)
                    block = SchedulerBlock(openTime, usedCT, freeCT)
                    blockList.append(block)
                    xmlRpcCmd = XmlRpcProtocolCommand(cmd.type,usedCT,cmd.label)
                    self.lastXmlRpcProtocolCommands.append(xmlRpcCmd)
                    openTime = cmd.extensionTime


            # Now reverse the schedule for the purposes of putting the "end within" time as the first open time.
            # The mechanism for this is to reverse the order of the list then make the free and open times line up
            # again by advancing each open time one step and retarding each free time one step.

            # As it is programmed by forward iteration, this is done here by retarding the open time before the
            # list reversal, then doing the reversal, then retarding the free time.


            # Retard the open time
            for i in range( len( blockList ) - 1 ):
                blockList[i].setOpenPeriod( blockList[i+1].getOpenPeriod() )

            blockList[-1].setOpenPeriod( self.endTimeSpanThreshold )

            # Reverse the list
            blockList.reverse()

            # Retard the free time
            for i in range( len( blockList ) - 1 ):
                blockList[i].setFreePeriod( blockList[i+1].getFreePeriod() )

            blockList[-1].setFreePeriod( 0 )

    #            if self.debug:
    #                for i in blockList:
    #                    print i

        else:
            # Protocol == None
            # We have a serious disconnect with the client if we can't find
            # the protocol associated with this sample
            self.svrLog.logWarning('', self.logPrefix, funcReference, " Can't find protocol ID (%d) for sample ID %d" % \
                (sample.protocolID, sample.ID))    # 2011-11-24 sp -- added logging
            raise SchedulerException("SS: Can't find protocol ID (%d) for sample ID %d" % \
                (sample.protocolID, sample.ID))

        return blockList

    def getCommandTimes(self, command, sample = None):
        '''Return a tuple of (open, used, free) times that it will take the
        specified command/workflow to execute with the current sample. This
        is what we feed into our ScheduleBlock for this command.
        Note that we dont use the sample parameter (yet).'''
        return self.getCommandTypeTimes(command.type)

    def getCommandTypeTimes(self, commandType):
        '''Return a tuple of (open, used, free) times that it will take the 
        specified command to execute.'''
        # This next line is to accomodate legacy code (we used to look up a type
        # such as 'Transport', but the type is now 'TransportCommand'. Rather 
        # than changing everything here, let's just trim the type back to the old
        # style
        funcReference = __name__ + '.getCommandTypeTimes'      # 2011-12-09 sp -- added logging

        commandType = commandType.split('Command')[0]
        if commandType not in self.schedulerTimes:
            self.svrLog.logWarning('', self.logPrefix, funcReference, "Invalid type (%s) for duration calc" % \
                (commandType))    # 2011-11-24 sp -- added logging
            raise SchedulerException("SS: Invalid type (%s) for duration calc" % \
                (commandType))
        else:
            return self.schedulerTimes[commandType]

    def getEndTimeSpan( self ):
        '''Gets the end time span for the currently schduled samples'''
        return self.endTimeSpan

    def getEndTimeSpanThreshold( self ):
        '''Returns the end time span threshold'''
        return self.schedulerEndTimeSpan1

    def calculateDuration(self):
        '''For the scheduled block list, calculate and return it's duration
        (which is in seconds, relative to T = 0).'''
        funcReference = __name__ + '.calculateDuration'      # 2011-12-09 sp -- added logging
        durations = [0,]
        # For each batch ID (ie. each sample), find the last block. It's length
        # plus its start time is the duration of that batch.
        for batchID in self.__batchIDList:
            lastBlockID = self.__blockIDs[batchID][-1]
            block = SchedulerBlock(0, 0, 0)
            haveBlock = self.scheduler.GetBlock(batchID, lastBlockID, block)
            if haveBlock:
                start = block.getStartTime()
                usedTime = block.getPeriods()[1]
                freeTime = block.getPeriods()[2]
                durations.append(start + usedTime + freeTime)
            else:
                self.svrLog.logWarning('', self.logPrefix, funcReference, "Unable to find last block for batch ID %d" % \
                    (batchID))    # 2011-12-09 sp -- added logging
                raise SchedulerException("SS: Unable to find last block for batch ID %d" % \
                    (batchID))

        durations.sort()
        self.__duration_secs = durations[-1]
        return self.__duration_secs

    def __createWorkflowCall(self, sampleList, batchID, blockID, sharedSectorsTranslation = []):
        '''From the specified batchID and blockID, create a workflow call (as a
        string) that we will turn into a workflow call into the hardware
        layer.'''
        funcReference = __name__ + '.__createWorkflowCall'      # 2011-12-09 sp -- added logging

        sample = SampleScheduler.findSample(sampleList, batchID)
        protocol = self.protocolMgr.getProtocol(sample.protocolID)

        # This sample has a list of commands associated with it's protocol
        # The index into the command list is the current block ID minus
        # the first block ID for this sample (which we have)
        try:
            cmds = protocol.getCommands()
            index = blockID - self.__blockIDs[batchID][0]
            cmd = cmds[index]

            if len(sharedSectorsTranslation) == 4:
                if cmd.isTransportType() or cmd.isMixType() or cmd.isMixTransType(): # mk/db added 2016-02-19
                    vial = cmd.srcVial[0]
                    sector = (vial + sample.initialQuadrant) - 1
                    if cmd.srcVial[1] in (Instrument.AntibodyLabel, \
                                            Instrument.CocktailLabel, \
                                            Instrument.BeadLabel):
                        if sharedSectorsTranslation[sector-1]!=0:
                            newVial = sharedSectorsTranslation[sector-1]+1-sample.initialQuadrant
                            cmd.srcVial=(newVial,cmd.srcVial[1])
                            print(cmd.__class__.__name__,"convert",sector,cmd.srcVial[0]+sample.initialQuadrant-1)

            # Now create the workflow call (that we'll eval in the Dispatcher)
            cmdCall = cmd.createCall(sample, protocol)
        except CommandException as msg:
            # Unable to create the command call, probably because of bad input
            # data; ie. a bad protocol definition
            # This is serious, so let's bail
            self.svrLog.logWarning('', self.logPrefix, funcReference, "Unable to create workflow call: %s" % (msg))    # 2011-12-09 sp -- added logging
            raise SchedulerException("SS: Unable to create workflow call: %s" % (msg))
        except IndexError as msg:
            # This shouldn't happen any more (famous last words)
            self.svrLog.logWarning('', self.logPrefix, funcReference, "Index error in __createWorkflowCall: %s" % (msg))    # 2011-12-09 sp -- added logging
            raise SchedulerException("SS: Index error in __createWorkflowCall: %s" % (msg))

        return cmdCall

    def __getInterpolatedValue( self, volume, coords ):
        '''Returns the interpolated value y, given an x value (volume) and a list of points
        describing the curve'''
#        if self.debug: print volume, coords

        # First off we need to handle out of range volumes
        if volume <= coords[0][0]:
            returnVal = int( coords[0][1] )
        elif volume >= coords[-1][0]:
            returnVal = int( coords[-1][1] )
        else:
            i = 0
            for (x,y) in coords:
                if volume <= x:
                    break
                i += 1

            #print ('\n\n ^^^^^^^^^^^^^ __getInterpolatedValue v=%d i=%d \n'%(volume,i));

            # now we have i as the index to the lower of the coords we need to interpolate between
            x1,y1 = coords[i-1]
            x2,y2 = coords[i]
            #print ('\n\n ^^^^^^^^^^^^^ __getInterpolatedValue %d %d %d %d \n'%(x1,y1,x2,y2));

            returnVal = int( ( volume - x1 ) * ( y2 - y1 ) / ( x2 - x1 ) + y1 )

        #print ('\n\n ^^^^^^^^^^^^^ __getInterpolatedValue v=%d ret float=%f \n'%(volume, returnVal));
#        if self.debug: print volume, returnVal
        return returnVal


    def __getWorkflowSchedule(self, sampleList, sharedSectorsTranslation = []):
        '''Create the dispatchable container of times and corresponding
        workflow calls for each sample. Returns the Schedule object.'''
        funcReference = __name__ + '.__getWorkflowSchedule'      # 2011-12-09 sp -- added logging

        startTimes = {}
        
        
        # First off, reverse the order of start times to make the schedule forward pointing again.
        # Then subtract the used time of each block,
        if self.debug:
            print("Working out finish time")
        finishTimeList = []
        for batchID in self.__batchIDList:
            lastBlockID = self.__blockIDs[batchID][-1]
            block = SchedulerBlock(0, 0, 0)
            haveBlock = self.scheduler.GetBlock(batchID, lastBlockID, block)
            if not haveBlock:
                self.svrLog.logWarning('', self.logPrefix, funcReference, "Unable to find last block for batch ID %d" % \
                      (batchID))    # 2011-12-09 sp -- added logging
                raise SchedulerException("SS: Unable to find last block for batch ID %d" % \
                      (batchID))
            finishTimeList.append( block.getStartTime() )

        print("finishTimeList =", finishTimeList)

        finishTimeList.sort()
        finishTime = finishTimeList[-1]
        if self.debug:
            print("Finish time is", finishTime)

        for batchID in self.__batchIDList:
            startTimes[batchID] = []
            for blockID in self.__blockIDs[batchID]:
                block = SchedulerBlock(0, 0, 0)
                haveBlock = self.scheduler.GetBlock(batchID, blockID, block)
#                print "Batch %d, block %d -"%(batchID,blockID), block.getPeriods(), block.getStartTime()

                if not haveBlock:
                    self.svrLog.logWarning('', self.logPrefix, funcReference, "Unable to block ID %d for batch ID %d" % \
                        (blockID,batchID))    # 2011-12-09 sp -- added logging
                    raise SchedulerException("SS: Unable to block ID %d for batch ID %d" % \
                        (blockID,batchID))
                startTimes[batchID].append( finishTime - block.getStartTime() - block.getUsedPeriod() )
            startTimes[batchID].sort()
            
        # now shift all the start times forward in time enough to make them all non-negative 
        firstStartTimeList = []
        for batchID in self.__batchIDList:
            firstStartTimeList.append( startTimes[batchID][0] )

        firstStartTimeList.sort()
        firstStartTime = firstStartTimeList[0]
       
        for batchID in self.__batchIDList:
            for i in range( len( startTimes[batchID] ) ):
                startTimes[batchID][i] = startTimes[batchID][i] - firstStartTime 

        # Now actually generate the workflows.
        # The end time span is also calculated in this phase
        self.endTimeList = []
        s = Schedule()
        for batchID in self.__batchIDList:
            i = 0
            for blockID in self.__blockIDs[batchID]:
                block = SchedulerBlock(0, 0, 0)
                haveBlock = self.scheduler.GetBlock(batchID, blockID, block)
                if haveBlock:
                    startTime = startTimes[batchID][i]
                else:
                    self.svrLog.logWarning('', self.logPrefix, funcReference, "Unable to find block ID %d for batch ID %d" % \
                        (blockID, batchID))    # 2011-12-09 sp -- added logging
                    raise SchedulerException("SS: Unable to find block ID %d for batch ID %d" % \
                        (blockID, batchID))
                workflowCall = self.__createWorkflowCall(sampleList, batchID, blockID, sharedSectorsTranslation)
    #                if self.debug:
    #                    print "addWorkflow", (batchID, startTime, workflowCall)
                s.addWorkflow(batchID, startTime, workflowCall)
                i = i + 1
            self.endTimeList.append( startTime + block.getUsedPeriod() + block.getFreePeriod() )

        if self.debug:
            print("End times =", self.endTimeList)

        self.endTimeList.sort()
        self.endTimeSpan = self.endTimeList[-1] - self.endTimeList[0]

        if self.debug:
            print("End time span =", self.endTimeSpan)
        
        return s

    # --- support methods for debugging etc. ---

    def dumpBlocks(self):
        '''Dump (to stdout) the list of blocks that we want to schedule. This is
        really just for debugging purposes.'''
        funcReference = __name__ + '.dumpBlocks'      # 2011-12-09 sp -- added logging

        for batchID in list(self.__blockIDs.keys()):
            print('~' * 75)
            print("Batch ID (= sample ID) = %d" % (batchID))

            for blockID in self.__blockIDs[batchID]:
                block = SchedulerBlock(0, 0, 0)
                haveBlock = self.scheduler.GetBlock(batchID, blockID, block)
                if not haveBlock:
                    # This really should not happen
                    self.svrLog.logWarning('xx', self.logPrefix, funcReference, "Could not find block for %d/%d" % \
                        (batchID, blockID))    # 2011-12-09 sp -- added logging; not used within, logging not tested
                    raise SchedulerException("SS: Could not find block for %d/%d" % \
                        (batchID, blockID))

                print("%2d: %12s\tST = %-5d" % (blockID, block.getPeriods(), block.getStartTime()))
        print('=' * 75)

    # --- set up our static methods etc ---

    def getSampleInfo(sampleList):
        '''For the supplied sample list, create a nice string for debugging info.'''
        sampleInfo = ''
        if len(sampleList) > 0:
            for sample in sampleList:
                sampleInfo += "[#%s='%s'], " % (sample.ID, sample.label)
            # Trim off that last ', ' string
            sampleInfo = sampleInfo.strip(', ')
        return sampleInfo

    def findSample(sampleList, batchID):
        '''Return the sample that corresponds to the batch ID (which should be
        the same as our sample ID. Throws an exception if we can't find one, as
        this is a serious error.
        Note -- this is a static method!'''
        funcReference = __name__ + '.findSample'      # 2011-12-09 sp -- added logging
        # 2011-12-09 sp -- added logging
        svrLog = PgmLog( 'svrLog' )
        logPrefix = 'SH'

        hits = [sample for sample in sampleList if sample.ID == batchID]
        numHits = len(hits)
        if numHits == 0:
            svrLog.logWarning('', logPrefix, funcReference, "Could not find sample for batchID = %d" % (batchID))    # 2011-12-09 sp -- added logging
            raise SchedulerException("SS: Could not find sample for batchID = %d" % (batchID))
        elif numHits > 1:
            # This should *never* happen!
            svrLog.logWarning('', logPrefix, funcReference, "Found %d samples with batchID = %d" % \
                (numHits, batchID))    # 2011-12-09 sp -- added logging
            raise SchedulerException("SS: Found %d samples with batchID = %d" % \
                (numHits, batchID))
        else:
            sample = hits[0]

        return sample


    def getProtocolCommandList(self, protocolIdx):
        '''cmd = []
        test = XmlRpcProtocolCommand("A",1,"A")
        cmd.append(test)
        test = XmlRpcProtocolCommand("B",2,"B")
        cmd.append(test)
        test = XmlRpcProtocolCommand("C",3,"C")
        cmd.append(test)
        return cmd
        '''
        #print '====================================== getProtocolCommandList CC3',len(self.xmlRpcProtocols)
        #print '====================================== getProtocolCommandList CC4',len(self.xmlRpcProtocols[0]),protocolIdx
        #for c in self.xmlRpcProtocols[protocolIdx]:
        #    print c
        #print '====================================== getProtocolCommandList DD'
        
        return self.xmlRpcProtocols[protocolIdx]
        


    # Set up our static methods
    getSampleInfo = staticmethod(getSampleInfo)
    findSample = staticmethod(findSample)

# eof

