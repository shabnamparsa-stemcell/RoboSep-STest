# 
# Protocol.py
# tesla.types.Protocol.py
#
# Protocol type for the Tesla instrument control software
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
#    01/06/06 - changed getVolumes() pass list - IAT (bdr)
#    03/23/06 - uniform multi sample - RL
#    03/29/06 - Short Description for Sample Volume Dialog - RL
# 
# 

import copy
import os
import datetime
import handyxml
import xml.dom.Element

from ipl.utils import simpleHash
from ipl.utils.validation import validateNumber

import tesla.config
from tesla.hardware.config import gHardwareData, LoadSettings
from tesla.exception import TeslaException
from tesla.types.Command import Command, EndOfProtocolCommand, CommandFactory, CommandException 
from tesla.types.Command import TopUpTransSepTransCommand,  TopUpMixTransSepTransCommand, ResusMixSepTransCommand,ResusMixCommand
from tesla.types.Command import TopUpTransCommand,TopUpTransSepTransCommand,TopUpMixTransCommand,TopUpMixTransSepTransCommand,MixTransCommand
from tesla.types.ProtocolConsumable import ProtocolConsumable, EMPTY, NOT_NEEDED
from tesla.instrument.Instrument import Instrument

#KC
#when prints == 1 debug print statements will be active for dead volumes and custom names
#when prints == 0 print statements will be inactive




prints = 1






# -----------------------------------------------------------------------------

class ProtocolException(TeslaException):
    '''Exceptions related to processing protocols'''
    pass

# ----------------------------------------------------------------------------

class Protocol(object):
    '''The instrument protocol class (not to be confused with the simpler
    ClientProtocol class, which is used for transporting client-visible
    protocol information across the XML-RPC interface.
    '''

    DEFAULT_SAMPLE_THRESHOLD = 4000
    DEFAULT_LOW_WORKING_VOLUME = 5000

 
    # Dead volumes of vessels that the protocol might use
    __deadVolumeData =  {   Instrument.DeadVolumeAntibodyLabel     : '0',
                            Instrument.DeadVolumeCocktailLabel     : '0',
                            Instrument.DeadVolumeBeadLabel         : '0',
                            Instrument.DeadVolume14mlLabel         : '0',
                            Instrument.DeadVolume50mlLabel         : '0' }

    
    #subfor_transseptrans_onlyif_sep_is_lessthan_seconds = 60
    #apply_cmd_sub_logic = 1
    __cmdSubLogicData =  {   Instrument.SubForTransSepTransOnlyIfSepIsLessThanSecondsLabel    : '60',
                             Instrument.ApplyCommandSubstitutionLogicLabel                    : '1'}

 
    def __init__(self, fileName, *args, **kw):
        ''''Constructor for the Protocol class; the filename specifies an
        XML file describing the protocol.'''
        self.__sourceFile = fileName        # Store the filename for debugging
        self.__createDefaults()             # Create default values for members
        self.importFromXML(fileName)
        self.filename = fileName.encode('utf-8')


        self.ApplyCommandSubstitutionLogic = 1
        if kw.has_key(Instrument.ApplyCommandSubstitutionLogicLabel):
            self.ApplyCommandSubstitutionLogic = kw[Instrument.ApplyCommandSubstitutionLogicLabel]

        self.SubForTransSepTransOnlyIfSepIsLessThanSeconds = 60
        if kw.has_key(Instrument.SubForTransSepTransOnlyIfSepIsLessThanSecondsLabel):
            self.SubForTransSepTransOnlyIfSepIsLessThanSeconds = kw[Instrument.SubForTransSepTransOnlyIfSepIsLessThanSecondsLabel]

        #print 'TEST',self.ApplyCommandSubstitutionLogic,self.SubForTransSepTransOnlyIfSepIsLessThanSeconds
            
        #do cmd sub here!!
        if not int(self.ApplyCommandSubstitutionLogic) == 0:
            self.__do_cmd_sub__()

        #2019-04-09 RL insert extra no op end command here so pause/err msg with last command will show
        self.__append_no_op_cmd()

    def __append_no_op_cmd(self):
        #make sure no op cmd is only added once by checking last command label to see if match 'End of Run'
        no_op_cmd_label = 'End of Run'
        lastCmd = self.__cmds[len(self.__cmds)-1]
        if lastCmd.isEndOfProtocolType() and lastCmd.label==no_op_cmd_label:
            return
        else:
            newCmd = EndOfProtocolCommand(len(self.__cmds)+1, 'EndOfProtocolCommand', no_op_cmd_label)
            self.__cmds.append(newCmd)
        

    def __do_cmd_sub__(self):
        debug_print = False
        transSepTransLimit = int(self.SubForTransSepTransOnlyIfSepIsLessThanSeconds)
        #find last topup/resus
        for i, cmd in list(enumerate(self.__cmds))[::-1]:
            if cmd.includesTopUpOrResuspendInType():   #find last topup/resuspend

                #short circuit if found buffer tip use after this command
                #IF (a Transport command that enabled "Use Buffer Tip") OR
                #   (a Transport command that is from/to Buffer bottle) is found, 
                noMerge = False
                tmpCmdIdx = i + 1
                while tmpCmdIdx < len(self.__cmds):
                    if self.__cmds[tmpCmdIdx].isTransportType():
                        transCmd = self.__cmds[tmpCmdIdx]
                        srcVial = transCmd.srcVial[1]
                        destVial = transCmd.destVial[1]
                        #print "PARAMS", transCmd.useBufferTip, srcVial, destVial
                        if transCmd.useBufferTip or srcVial == Instrument.BCCMLabel or destVial == Instrument.BCCMLabel:
                            #print "FOUND BUFFER TIP USE, SKIP MERGE"
                            #print "^^^^^^^^NO MERGE   ",self.filename
                            noMerge = True
                            break
                    tmpCmdIdx =  tmpCmdIdx + 1
                if noMerge:
                    break

                
                if cmd.isTopUpOrResuspendMergeCandidateType(): #see if it is candidate
                    print i, cmd

                    nxtCmd = None
                    nxtNxtCmd = None
                    nxtNxtNxtCmd = None
                    nxtNxtNxtNxtCmd = None
                    if len(self.__cmds)>i+1:
                        nxtCmd = self.__cmds[i+1]
                    if len(self.__cmds)>i+2:
                        nxtNxtCmd = self.__cmds[i+2]
                    if len(self.__cmds)>i+3:
                        nxtNxtNxtCmd = self.__cmds[i+3]
                    if len(self.__cmds)>i+4:
                        nxtNxtNxtNxtCmd = self.__cmds[i+4]

                    if cmd.isTopUpType(): #can become TopUpMixTrans, TopUpMixTransSepTrans, TopUpTrans, TopUpTransSepTrans
                        if nxtCmd is not None and nxtNxtCmd is not None and nxtNxtNxtCmd is not None and nxtNxtNxtNxtCmd is not None and\
                           nxtCmd.isMixType() and nxtNxtCmd.isTransportType() and \
                           nxtNxtNxtCmd.isSeparateType() and nxtNxtNxtCmd.minPeriod <= transSepTransLimit and nxtNxtNxtNxtCmd.isTransportType():
                            cmdTopUp = cmd
                            cmdMix = nxtCmd
                            cmdTrans = nxtNxtCmd
                            cmdSep = nxtNxtNxtCmd
                            cmdTrans2 = nxtNxtNxtNxtCmd
                            newCmd = self.mergeTopUpWithMixWithTransWithSepWithTrans(i+1,cmdTopUp, cmdMix, cmdTrans, cmdSep,cmdTrans2)
                            if not newCmd == None:
                                debug_print = True
                                #replace old commands
                                self.__cmds[i] = newCmd
                                #self.__cmds.remove(cmd)
                                self.__cmds.remove(nxtCmd)
                                self.__cmds.remove(nxtNxtCmd)
                                self.__cmds.remove(nxtNxtNxtCmd)
                                self.__cmds.remove(nxtNxtNxtNxtCmd)
                        elif nxtCmd is not None and nxtNxtCmd is not None and nxtCmd.isMixType() and nxtNxtCmd.isTransportType():
                            cmdTopUp = cmd
                            cmdMix = nxtCmd
                            cmdTrans = nxtNxtCmd
                            newCmd = self.mergeTopUpWithMixWithTrans(i+1,cmdTopUp, cmdMix, cmdTrans)
                            if not newCmd == None:
                                debug_print = True
                                #replace old commands
                                self.__cmds[i] = newCmd
                                #self.__cmds.remove(cmd)
                                self.__cmds.remove(nxtCmd)
                                self.__cmds.remove(nxtNxtCmd)

                        elif nxtCmd is not None and nxtNxtCmd is not None and nxtNxtNxtCmd is not None and \
                              nxtCmd.isTransportType() and nxtNxtCmd.isSeparateType() and \
                              nxtNxtCmd.minPeriod <= transSepTransLimit and nxtNxtNxtCmd.isTransportType():
                            cmdTopUp = cmd
                            cmdTrans = nxtCmd
                            cmdSep = nxtNxtCmd
                            cmdTrans2 = nxtNxtNxtCmd
                            newCmd = self.mergeTopUpWithTransWithSepWithTrans(i+1,cmdTopUp, cmdTrans, cmdSep, cmdTrans2)
                            if not newCmd == None:
                                debug_print = True
                                #replace old commands
                                self.__cmds[i] = newCmd
                                #self.__cmds.remove(cmd)
                                self.__cmds.remove(nxtCmd)
                                self.__cmds.remove(nxtNxtCmd)
                                self.__cmds.remove(nxtNxtNxtCmd)
                                
                        elif nxtCmd is not None and  nxtCmd.isTransportType():
                            cmdTopUp = cmd
                            cmdTrans = nxtCmd
                            newCmd = self.mergeTopUpWithTrans(i+1,cmdTopUp, cmdTrans)
                            if not newCmd == None:
                                debug_print = True
                                #replace old commands
                                self.__cmds[i] = newCmd
                                #self.__cmds.remove(cmd)
                                self.__cmds.remove(nxtCmd)
                            
                        
                    elif cmd.isResuspendType(): #can become ResusMix, ResusMixSepTrans
                        if nxtCmd is not None and nxtNxtCmd is not None and nxtNxtNxtCmd is not None and \
                              nxtCmd.isMixType() and nxtNxtCmd.isSeparateType() and \
                              nxtNxtCmd.minPeriod <= transSepTransLimit and nxtNxtNxtCmd.isTransportType():
                            cmdResus = cmd
                            cmdMix = nxtCmd
                            cmdSep = nxtNxtCmd
                            cmdTrans = nxtNxtNxtCmd
                            newCmd = self.mergeResusWithMixWithSepWithTrans(i+1,cmdResus, cmdMix, cmdSep, cmdTrans)
                            if not newCmd == None:
                                debug_print = True
                                #replace old commands
                                self.__cmds[i] = newCmd
                                #self.__cmds.remove(cmd)
                                self.__cmds.remove(nxtCmd)
                                self.__cmds.remove(nxtNxtCmd)
                                self.__cmds.remove(nxtNxtNxtCmd)
                        elif nxtCmd is not None and  nxtCmd.isMixType():
                            cmdResus = cmd
                            cmdMix = nxtCmd
                            newCmd = self.mergeResusWithMix(i+1,cmdResus, cmdMix)
                            if not newCmd == None:
                                debug_print = True
                                #replace old commands
                                self.__cmds[i] = newCmd
                                #self.__cmds.remove(cmd)
                                self.__cmds.remove(nxtCmd)
                                  
                    elif cmd.isTopUpTransType() or cmd.isTopUpMixTransType() or cmd.isResusMixType():#can add SepTrans to the back
                        if nxtCmd is not None and nxtNxtCmd is not None and \
                              nxtCmd.isSeparateType() and nxtCmd.minPeriod <= transSepTransLimit and nxtNxtCmd.isTransportType():
                            cmdSep = nxtCmd
                            cmdTrans = nxtNxtCmd
                            if cmd.isTopUpTransType():
                                cmdTopUpTrans = cmd
                                newCmd = self.mergeTopUpTransWithSepWithTrans(i+1,cmdTopUpTrans, cmdSep, cmdTrans)
                            elif cmd.isTopUpMixTransType():
                                cmdTopUpMixTrans = cmd
                                newCmd = self.mergeTopUpMixTransWithSepWithTrans(i+1,cmdTopUpMixTrans, cmdSep, cmdTrans)
                            else:
                                cmdResusMix = cmd
                                newCmd = self.mergeResusMixWithSepWithTrans(i+1,cmdResusMix, cmdSep, cmdTrans)
                                        
                            if not newCmd == None:
                                debug_print = True
                                #replace old commands
                                self.__cmds[i] = newCmd
                                #self.__cmds.remove(cmd)
                                self.__cmds.remove(nxtCmd)
                                self.__cmds.remove(nxtNxtCmd)
                                            
                                            
                                        
                        
                break                                   #break either way after looking at last topup/resuspend

        #iterate forward and look for mix trans
        haventCheckLastCmd = True
        while haventCheckLastCmd:
            for i, cmd in list(enumerate(self.__cmds)):
                if cmd.isMixType():
                    if len(self.__cmds)>i+1:
                        nxtCmd = self.__cmds[i+1]
                        if nxtCmd.isTransportType():
                            cmdMix = cmd
                            cmdTrans = nxtCmd
                            newCmd = self.mergeMixWithTrans(i+1, cmdMix, cmdTrans)
                                        
                            if not newCmd == None:
                                debug_print = True
                                #replace old commands
                                self.__cmds[i] = newCmd
                                #self.__cmds.remove(cmd)
                                self.__cmds.remove(nxtCmd)
                                break
                            
                if i+1 >= len(self.__cmds):
                    haventCheckLastCmd = False
        
        #need to fix all seq number here!!
        for i, cmd in list(enumerate(self.__cmds)):
            cmd.seq = i+1
            
        debug_print = False
        if debug_print:
            print "^^^^^^^^MERGE   ",self.filename
            for i, cmd in list(enumerate(self.__cmds)):
                print "^^^^^^^^   ",i, cmd

    def mergeTopUpTransWithSepWithTrans(self, seq, cmdTopUpTrans, cmdSep, cmdTrans):
        newCmd = None
        #check cmdTopUpTrans dest2 == cmdTrans src
        if cmdTopUpTrans.destVial2 == cmdTrans.srcVial:
            newLabel = "%s, %s, %s" % (cmdTopUpTrans.label, cmdSep.label, cmdTrans.label)
            newCmd = TopUpTransSepTransCommand(seq, 'TopUpTransSepTransCommand', newLabel)
            newCmd.copyVolumeCommand(cmdTopUpTrans)
            newCmd.duration = cmdSep.minPeriod
            newCmd.numOfStages = 3
            newCmd.srcVial3 = cmdTrans.srcVial		
            newCmd.destVial3 = cmdTrans.destVial
            newCmd.relative3 = cmdTrans.relative            
            newCmd.proportion3 = cmdTrans.proportion
            newCmd.absVolume3 = cmdTrans.absVolume
            

        return newCmd

    
    def mergeTopUpMixTransWithSepWithTrans(self, seq, cmdTopUpMixTrans, cmdSep, cmdTrans):
        newCmd = None
        #check cmdTopUpMixTrans dest2 == cmdTrans src
        if cmdTopUpMixTrans.destVial2 == cmdTrans.srcVial:
            newLabel = "%s, %s, %s" % (cmdTopUpMixTrans.label, cmdSep.label, cmdTrans.label)
            newCmd = TopUpMixTransSepTransCommand(seq, 'TopUpMixTransSepTransCommand', newLabel)
            newCmd.copyVolumeCommand(cmdTopUpMixTrans)
            newCmd.mixCycles = cmdTopUpMixTrans.mixCycles
            newCmd.tipTubeBottomGap = cmdTopUpMixTrans.tipTubeBottomGap
            newCmd.duration = cmdSep.minPeriod
            newCmd.numOfStages = 4
            newCmd.srcVial3 = cmdTrans.srcVial		
            newCmd.destVial3 = cmdTrans.destVial
            newCmd.relative4 = cmdTrans.relative            
            newCmd.proportion4 = cmdTrans.proportion
            newCmd.absVolume4 = cmdTrans.absVolume

        return newCmd
    
    def mergeResusMixWithSepWithTrans(self, seq, cmdResusMix, cmdSep, cmdTrans):
        newCmd = None
        #check cmdResusMix dest == cmdTrans src
        if cmdResusMix.destVial == cmdTrans.srcVial:
            newLabel = "%s, %s, %s" % (cmdResusMix.label, cmdSep.label, cmdTrans.label)
            newCmd = ResusMixSepTransCommand(seq, 'ResusMixSepTransCommand', newLabel)
            newCmd.copyVolumeCommand(cmdResusMix)
            newCmd.mixCycles = cmdResusMix.mixCycles
            newCmd.tipTubeBottomGap = cmdResusMix.tipTubeBottomGap
            newCmd.duration = cmdSep.minPeriod
            newCmd.numOfStages = 3
            newCmd.srcVial2 = cmdTrans.srcVial		
            newCmd.destVial2 = cmdTrans.destVial
            newCmd.relative3 = cmdTrans.relative            
            newCmd.proportion3 = cmdTrans.proportion
            newCmd.absVolume3 = cmdTrans.absVolume

        return newCmd

    def mergeResusWithMixWithSepWithTrans(self, seq, cmdResus, cmdMix, cmdSep, cmdTrans):
        newCmd = None
        if cmdResus.destVial == cmdMix.srcVial and cmdResus.destVial == cmdTrans.srcVial:
            newLabel = "%s, %s, %s, %s" % (cmdResus.label, cmdMix.label, cmdSep.label, cmdTrans.label)
            newCmd = ResusMixSepTransCommand(seq, 'ResusMixSepTransCommand', newLabel)
            newCmd.copyVolumeCommand(cmdResus)
            newCmd.mixCycles = cmdMix.mixCycles
            newCmd.tipTubeBottomGap = cmdMix.tipTubeBottomGap
            newCmd.relative2 = cmdMix.relative            
            newCmd.proportion2 = cmdMix.proportion
            newCmd.absVolume2 = cmdMix.absVolume
            newCmd.duration = cmdSep.minPeriod
            newCmd.numOfStages = 3
            newCmd.srcVial2 = cmdTrans.srcVial		
            newCmd.destVial2 = cmdTrans.destVial
            newCmd.relative3 = cmdTrans.relative            
            newCmd.proportion3 = cmdTrans.proportion
            newCmd.absVolume3 = cmdTrans.absVolume

        return newCmd
    def mergeResusWithMix(self, seq, cmdResus, cmdMix):
        newCmd = None
        if cmdResus.destVial == cmdMix.srcVial:
            newLabel = "%s, %s" % (cmdResus.label, cmdMix.label)
            newCmd = ResusMixCommand(seq, 'ResusMixCommand', newLabel)
            newCmd.copyVolumeCommand(cmdResus)
            newCmd.mixCycles = cmdMix.mixCycles
            newCmd.tipTubeBottomGap = cmdMix.tipTubeBottomGap
            newCmd.relative2 = cmdMix.relative            
            newCmd.proportion2 = cmdMix.proportion
            newCmd.absVolume2 = cmdMix.absVolume
            newCmd.numOfStages = 2

        return newCmd
    def mergeMixWithTrans(self, seq, cmdMix, cmdTrans):
        newCmd = None
        if cmdMix.srcVial == cmdTrans.srcVial:
            newLabel = "%s, %s" % (cmdMix.label, cmdTrans.label)
            newCmd = MixTransCommand(seq, 'MixTransCommand', newLabel)
            newCmd.copyVolumeCommand(cmdMix)
            newCmd.mixCycles = cmdMix.mixCycles
            newCmd.tipTubeBottomGap = cmdMix.tipTubeBottomGap
            newCmd.relative2 = cmdTrans.relative            
            newCmd.proportion2 = cmdTrans.proportion
            newCmd.absVolume2 = cmdTrans.absVolume
            newCmd.numOfStages = 2
            newCmd.srcVial = cmdTrans.srcVial 
            newCmd.destVial = cmdTrans.destVial 
            newCmd.freeAirDispense = cmdTrans.freeAirDispense
            newCmd.useBufferTip = cmdTrans.useBufferTip

        return newCmd

      
    def mergeTopUpWithTransWithSepWithTrans(self, seq, cmdTopUp, cmdTrans, cmdSep, cmdTrans2):
        newCmd = None
        if cmdTopUp.destVial == cmdTrans.srcVial and  cmdTrans.destVial == cmdTrans2.srcVial:
            newLabel = "%s, %s, %s, %s" % (cmdTopUp.label, cmdTrans.label, cmdSep.label, cmdTrans2.label)
            newCmd = TopUpTransSepTransCommand(seq, 'TopUpTransSepTransCommand', newLabel)
            newCmd.copyVolumeCommand(cmdTopUp)
            newCmd.relative2 = cmdTrans.relative            
            newCmd.proportion2 = cmdTrans.proportion
            newCmd.absVolume2 = cmdTrans.absVolume
            newCmd.numOfStages = 3
            newCmd.srcVial2 = cmdTrans.srcVial		
            newCmd.destVial2 = cmdTrans.destVial
            newCmd.duration = cmdSep.minPeriod
            newCmd.relative3 = cmdTrans2.relative            
            newCmd.proportion3 = cmdTrans2.proportion
            newCmd.absVolume3 = cmdTrans2.absVolume
            newCmd.srcVial3 = cmdTrans2.srcVial		
            newCmd.destVial3 = cmdTrans2.destVial

        return newCmd
    
    def mergeTopUpWithTrans(self, seq, cmdTopUp, cmdTrans):
        newCmd = None
        if cmdTopUp.destVial == cmdTrans.srcVial:
            newLabel = "%s, %s" % (cmdTopUp.label, cmdTrans.label)
            newCmd = TopUpTransCommand(seq, 'TopUpTransCommand', newLabel)
            newCmd.copyVolumeCommand(cmdTopUp)
            newCmd.relative2 = cmdTrans.relative            
            newCmd.proportion2 = cmdTrans.proportion
            newCmd.absVolume2 = cmdTrans.absVolume
            newCmd.numOfStages = 2
            newCmd.srcVial2 = cmdTrans.srcVial		
            newCmd.destVial2 = cmdTrans.destVial

        return newCmd

    def mergeTopUpWithMixWithTransWithSepWithTrans(self, seq, cmdTopUp, cmdMix, cmdTrans, cmdSep, cmdTrans2):
        newCmd = None
        if cmdTopUp.destVial == cmdMix.srcVial and cmdTopUp.destVial == cmdTrans.srcVial and  cmdTrans.destVial == cmdTrans2.srcVial:
            newLabel = "%s, %s, %s, %s, %s" % (cmdTopUp.label, cmdMix.label, cmdTrans.label, cmdSep.label, cmdTrans2.label)
            newCmd = TopUpMixTransSepTransCommand(seq, 'TopUpMixTransSepTransCommand', newLabel)
            newCmd.copyVolumeCommand(cmdTopUp)
            newCmd.mixCycles = cmdMix.mixCycles
            newCmd.tipTubeBottomGap = cmdMix.tipTubeBottomGap
            newCmd.relative2 = cmdMix.relative            
            newCmd.proportion2 = cmdMix.proportion
            newCmd.absVolume2 = cmdMix.absVolume
            newCmd.numOfStages = 4
            newCmd.srcVial2 = cmdTrans.srcVial		
            newCmd.destVial2 = cmdTrans.destVial
            newCmd.relative3 = cmdTrans.relative            
            newCmd.proportion3 = cmdTrans.proportion
            newCmd.absVolume3 = cmdTrans.absVolume
            newCmd.duration = cmdSep.minPeriod
            newCmd.relative4 = cmdTrans2.relative            
            newCmd.proportion4 = cmdTrans2.proportion
            newCmd.absVolume4 = cmdTrans2.absVolume
            newCmd.srcVial3 = cmdTrans2.srcVial		
            newCmd.destVial3 = cmdTrans2.destVial
            

        return newCmd
    
    def mergeTopUpWithMixWithTrans(self, seq, cmdTopUp, cmdMix, cmdTrans):
        newCmd = None
        if cmdTopUp.destVial == cmdTrans.srcVial and cmdTopUp.destVial == cmdMix.srcVial:
            newLabel = "%s, %s, %s" % (cmdTopUp.label, cmdMix.label, cmdTrans.label)
            newCmd = TopUpMixTransCommand(seq, 'TopUpMixTransCommand', newLabel)
            newCmd.copyVolumeCommand(cmdTopUp)
            newCmd.mixCycles = cmdMix.mixCycles
            newCmd.tipTubeBottomGap = cmdMix.tipTubeBottomGap
            newCmd.relative2 = cmdMix.relative            
            newCmd.proportion2 = cmdMix.proportion
            newCmd.absVolume2 = cmdMix.absVolume
            newCmd.numOfStages = 3
            newCmd.srcVial2 = cmdTrans.srcVial		
            newCmd.destVial2 = cmdTrans.destVial
            newCmd.relative3 = cmdTrans.relative            
            newCmd.proportion3 = cmdTrans.proportion
            newCmd.absVolume3 = cmdTrans.absVolume

        return newCmd
    
    def __repr__(self):
        '''Basic string representation of the Protocol class.'''
        return "'%s' protocol (%s)" % (self.label, self.ID)


    def __createDefaults(self):
        # Header defaults
        self.__ID = ''
        self.__label = 'Undefined protocol'
        self.description = 'Undefined description'
        self.version = '1.0'                        # Protocol version
        self.created = self.__makeCreationTime()    # Date/time created
        self.modified = self.created                # Date/time modified
        self.author = 'Not defined'                 # Protocol authoa
        self.type = tesla.config.TEST_PROTOCOL      # Protocol type
        self.protocolNumber ='00000-0000'

        # Chemistry constraint defaults
        self.__numQuadrants = 1                     # Number of quadrants used
        self.minVol = 0                             # Min & max sample volumes
        self.maxVol = tesla.config.DEFAULT_WORKING_VOLUME_uL       
        self.sampleThreshold = self.DEFAULT_SAMPLE_THRESHOLD 
        self.lowWorkingVolume = self.DEFAULT_LOW_WORKING_VOLUME
        self.highWorkingVolume = self.maxVol

        self.__cmds = []                            # Command defaults
        self.__customNames = [] #CR

        self.__resultChecks = []
        self.__maxLysisVol = [-1,-1,-1,-1,-1]
        
        # RL - uniform multi sample -  03/23/06
        self.__multipleSamples = []

        self.__features = []

    # --- "getter" methods ----------------------------------------------------

    def matches(self, ID = None, label = None):
        '''Returns True if this protocol has the specified ID or label? False
        otherwise.'''
        if ID:
            return self.ID == ID
        else:
            return self.label == label


    def getCommand(self, sequenceNumber):
        '''Return the specific command from our list of protocol commands.'''
        if sequenceNumber < 1 or sequenceNumber > self.numCmds:
            raise ProtocolException, "PR: %d commands in protocol (tried to get %d)" % \
                    (self.numCmds, sequenceNumber)
        return copy.deepcopy(self.__cmds[sequenceNumber - 1])


    def getCommands(self):
        '''Return a copy of the list of protocol commands'''
        return copy.deepcopy(self.__cmds)
    
    def getFeatures(self):
        return copy.deepcopy(self.__features)
    def findFeature(self,name):
        for feature in self.__features:
            if feature.name == name:
                return copy.deepcopy(feature)
        return None

    #CR - added function
    def getCustomNames(self):
        return copy.deepcopy(self.__customNames)
    
    def getResultChecks(self):
        return copy.deepcopy(self.__resultChecks)

    
    # RL - uniform multi sample -  03/23/06
    def getMultipleSamples(self):
        return copy.deepcopy(self.__multipleSamples)



    # --- XML methods ---------------------------------------------------------

    def importFromXML(self, fileName):
        '''Import the protocol definition from the appropriate XML file.'''
        if not os.path.isfile(fileName):
            raise ProtocolException, "PR: Unable to find file (%s)" % (fileName)

        try:
            xmlData = handyxml.xml(fileName,False)
        except Exception, msg:
            raise ProtocolException, "PR: XML parse error (%s) in %s" % (msg, fileName)

        if xmlData.node.getAttribute(0) != u'http://www.w3.org/2001/XMLSchema-instance':
            # We've hit an old-style protocol file???
            raise ProtocolException, "PR: %s is not a valid protocol file" % (fileName)
        
        try:
            self.type = xmlData.getAttribute('type')
            
            if prints == 1: print "protocol.py, self.type = %s" %self.type #CR - debug check

            self.parseHeaderNode(xmlData.header[0])
            self.parseConstraintsNode(xmlData.constraints[0])
            self.parseCommandsNode(xmlData.commands[0])
            
            #CR
            for i in range(self.numQuadrants):
                try:
                    self.parseCustomNames(xmlData.customNames[i])
                except StandardError:
                    #raise ProtocolException, "CR: parse custom names"
                    for j in range(8-(len(self.__customNames) % 8)):
                        self.__customNames.append("")
                        
            for i in range(self.numQuadrants):
                try:
                    self.parseResultChecks(xmlData.resultVialChecks[i])
                except StandardError:
                    #raise ProtocolException, "CR: parse custom names"
                    for j in range(8-(len(self.__resultChecks) % 8)):
                        self.__resultChecks.append(False)

            #for i in self.__resultChecks:
            #    print i, type(i)
            
            # RL - uniform multi sample -  03/23/06
            # if exists, MUST have info for all quadrants
            try:
                self.parseMultipleSamples(xmlData.multipleSamples[0])
            except StandardError:
                self.__multipleSamples = [ u"false" for i in range(tesla.config.NUM_QUADRANTS) ]
            
        except AttributeError, msg:
            raise ProtocolException, "PR: XML protocol error (%s) in %s" % (msg, fileName)
        except StandardError, msg:
            raise ProtocolException, "PR: Unexpected error (%s) in %s" % (msg, fileName)

#CR - added function
    def parseCustomNames(self, node):
        '''Parse the custom vial names from the supplied XML node'''
        self.__customNames.append(node.bufferBottle)
        self.__customNames.append(node.wasteTube)
        self.__customNames.append(node.lysisBufferTube)
        self.__customNames.append(node.selectionCocktailVial)
        self.__customNames.append(node.magneticParticleVial)
        self.__customNames.append(node.antibodyCocktailVial)
        self.__customNames.append(node.sampleTube)
        self.__customNames.append(node.separationTube)
        
    def parseResultChecks(self, node):
        self.__resultChecks.append(node.bufferBottle=="true")
        self.__resultChecks.append(node.wasteTube=="true")
        self.__resultChecks.append(node.lysisBufferTube=="true")
        self.__resultChecks.append(node.selectionCocktailVial=="true")
        self.__resultChecks.append(node.magneticParticleVial=="true")
        self.__resultChecks.append(node.antibodyCocktailVial=="true")
        self.__resultChecks.append(node.sampleTube=="true")
        self.__resultChecks.append(node.separationTube=="true")
        
    # RL - uniform multi sample -  03/23/06
    def parseMultipleSamples(self, node):
        '''Parse multiple samples info from supplied XML node'''
        self.__multipleSamples.append(node.Q1)
        self.__multipleSamples.append(node.Q2)
        self.__multipleSamples.append(node.Q3)
        self.__multipleSamples.append(node.Q4)

    def parseHeaderNode(self, node):
        '''Parse the header members from the supplied XML node.'''
        # Mandatory fields
        self.__label = node.label.encode('utf-8').strip()

        # RL - Short Description for Sample Volume Dialog -  03/29/06
        try:
            self.description = node.description.encode('utf-8')
        except AttributeError:
            self.description = "No Description"
            
        self.__ID    = simpleHash(self.label)
        self.version = node.version        
        self.author  = node.author[0].name.encode('utf-8')        
        self.created = node.date[0].created        
        # Optional field: modification date
        try:
            self.modified = node.date[0].modified
        except AttributeError:
            self.modified = self.created
        try:
            self.protocolNumber = node.protocolNum1+"-"+node.protocolNum2
        except AttributeError:
            self.protocolNumber = '00000-0000'

    def parseConstraintsNode(self, node):
        '''Parse the chemistry constraint members from the supplied XML node.'''
        # Mandatory fields
        self.__numQuadrants = int(node.quadrants[0].number)
        if self.numQuadrants not in range(0, tesla.config.NUM_QUADRANTS + 1):
            raise AttributeError, "Invalid # quadrants (%d)" % (self.numQuadrants)

        # Optional fields: sample volume min/max
        try:
            self.minVol = int(node.sampleVolume[0].min_uL)
            self.maxVol = int(node.sampleVolume[0].max_uL)
            
            if self.minVol > self.maxVol:
                # Wrong way around? Swap them...
                self.minVol, self.maxVol = self.maxVol, self.minVol
        except AttributeError:
            self.minVol = 0
            self.maxVol = 0
            
        # Optional fields: working volume
        try:
            self.sampleThreshold = int(node.workingVolume[0].sampleThreshold_uL)
            self.lowWorkingVolume = int(node.workingVolume[0].lowVolume_uL)
            self.highWorkingVolume = int(node.workingVolume[0].highVolume_uL)

            if self.lowWorkingVolume > self.highWorkingVolume:
                # Wrong way around? Swap them...
                self.lowWorkingVolume, self.highWorkingVolume = self.highWorkingVolume, self.lowWorkingVolume

            # Ensure that we can't have a low working volume that is less than our
            # sample threshold (this would not make sense)
            if self.lowWorkingVolume < self.sampleThreshold:
                self.sampleThreshold = self.lowWorkingVolume

            # Ensure that the max volume is within range of the working volume
            if self.maxVol > self.highWorkingVolume:
                self.maxVol = self.highWorkingVolume
        except AttributeError:
            self.sampleThreshold = 0
            self.lowWorkingVolume = 0
            self.highWorkingVolume = 0
            
        # Optional fields: feature switches 
        try:
            features = [child for child in node.featuresSwitches[0].childNodes if child.nodeType == node.ELEMENT_NODE]
            numFeatures = len(features)
            
            for feature in features:
                featureName = feature.getAttribute('name')
                featureDesc = feature.getAttribute('desc')
                featureInputType = feature.getAttribute('inputType')
                featureInputData = feature.getAttribute('inputData')
                self.__features.append(Feature(featureName,featureDesc,featureInputType,featureInputData))

                
        except AttributeError:
            pass

    def parseCommandsNode(self, node):
        '''Parse the protocol commands from the supplied XML node.'''
        numCmds = int(node.number)
        # Let's filter out any text nodes
        elements = [child for child in node.childNodes if child.nodeType == node.ELEMENT_NODE]
        numElements = len(elements)
        if numElements != numCmds:
            raise AttributeError, "# command elements (%d) != # commands (%d)" % \
                    (numElements, numCmds)
        # Now create each command and append it to the list of commands
        for element in elements:
            seqNum = int(element.getAttribute('seq'))
            cmdType = element.nodeName
            label = element.getAttribute('label')
#            extensionTime = int(element.getAttribute('extensionTime'))
            self.__cmds.append(CommandFactory(seqNum, cmdType, label, element))
            label = label.encode('utf-8')

    # --- Support methods -----------------------------------------------------
  
    def __makeCreationTime(self):
        '''Define a creation time (in ISO 8601 format)'''
        return datetime.datetime.today().isoformat()[:-7].replace('-','')

  
    # --- Instance properties (all read-only) ---------------------------------
    
    ID = property(lambda self: self.__ID, doc = 'Protocol ID')
    label = property(lambda self: self.__label, doc = 'Protocol label')								
    cmds = property(getCommands, doc = 'Return copy of protocol commands list')
    numCmds = property(lambda self: len(self.__cmds), doc = 'Number of protocol commands')
    numQuadrants = property(lambda self: self.__numQuadrants, doc = 'Number of used quadrants')						
    features = property(getFeatures, doc = 'Return copy of protocol features list')

    # For testing
    sourceFile = property(lambda self: self.__sourceFile, doc = 'Protocol XML source file')


    # --- Volume and volume tracking methods ----------------------------------

    def getWorkingVolume(self, sampleVolume_uL):
        '''For the specified sample volume (in uL), return the working
        volume (uL) as defined for this protocol.'''
        if sampleVolume_uL < self.sampleThreshold:
            return self.lowWorkingVolume
        else:
            return self.highWorkingVolume    


    def __updateTracking(self, srcVial, destVial, tiprackSpecified, tiprack, volume):# CR
        '''Updates the tracking table using the specified source and
        destination vials and volume (in uL).'''
        # Extract quadrants and vessel labels
        if srcVial == ():
            srcQuad=0
            srcVessel=''
        else:
            srcQuad, srcVessel = srcVial

        if destVial == ():
            destQuad=0
            destVessel=''
        else:
            destQuad, destVessel = destVial

        if prints == 1: print "BDR -- In UPD_TRACK for Src Quad %d, Dest Quad %d Vessel %s" % (srcQuad, destQuad, destVessel) #bdr
        
        ## TRACK TIP USAGE
        
        #Is a tiprack specified?
        if tiprackSpecified == True:
            self.__tipboxRequired[tiprack] = True
        else:
            # Do we need to use a tip from initial quadrant ?
            if srcVessel in [   Instrument.BCCMLabel,
                                Instrument.SampleLabel,
                                Instrument.SupernatentLabel,
                                Instrument.WasteLabel,
                                Instrument.LysisLabel ]:
                # These use initial quadrant tips
                self.__tipboxRequired[1] = True
        
            # Do we need to use a tip outside of the initial quadrant ?
            if srcVessel in [   Instrument.CocktailLabel,
                                Instrument.BeadLabel,
                                Instrument.AntibodyLabel ]:
                # These reagents always use co-located tip
                self.__tipboxRequired[srcQuad] = True

        ## TRACK VOLUMES FOR LOADED REAGENTS
        # Are we using a pre-loaded reagent ?
        if srcVessel in [   Instrument.CocktailLabel,
                            Instrument.BeadLabel,
                            Instrument.LysisLabel,
                            Instrument.AntibodyLabel ]:
            # A reagent is being aspirated, update volume required
            if self.__tracking[srcQuad][srcVessel] == NOT_NEEDED:
                self.__tracking[srcQuad][srcVessel] = EMPTY
            self.__tracking[srcQuad][srcVessel] += volume
            if srcVessel == Instrument.LysisLabel:
                if self.__maxLysisVol[srcQuad] < 0:
                    self.__maxLysisVol[srcQuad] = volume
                else:
                    self.__maxLysisVol[srcQuad] += volume
                    
        ## TRACK VOLUME OF BCCM REQUIRED
        # Are we using BCCM ?
        if srcVessel == Instrument.BCCMLabel:
            # keep track of BCCM required
            if self.__tracking[1][srcVessel] == NOT_NEEDED:
                self.__tracking[1][srcVessel] = EMPTY
            self.__tracking[1][srcVessel] += volume
            
    
        ## TRACK USE OF VESSELS IN QUADRANTS (NO VOLUMES)
        # We only need to know if these vessels are required
        if srcVessel in [   Instrument.SampleLabel, 
                            Instrument.SupernatentLabel,
                            Instrument.WasteLabel]:
            # we dont care about volume, only that it is needed
            self.__tracking[srcQuad][srcVessel] = EMPTY
         
        # We only need to know if these vessels are required
        if destVessel in [  Instrument.SampleLabel, 
                            Instrument.SupernatentLabel,
                            Instrument.LysisLabel,
                            Instrument.WasteLabel]:
            # we dont care about volume, only that it is needed
            self.__tracking[destQuad][destVessel] = EMPTY
            if destVessel == Instrument.LysisLabel:
                if prints == 1: print "BDR -- UPDATETRACK: Lysis tube needed in quad %d- !!! - set to EMPTY %d" % (destQuad, self.__tracking[destQuad][Instrument.LysisLabel])
                if self.__maxLysisVol[destQuad] == NOT_NEEDED:
                    self.__maxLysisVol[destQuad] = EMPTY
                    
        # If we use any of the reagent vessels, assume that a prime will
        # happen at the start and force a waste vial to be required
        # At the moment, assume that a waste vial is required per quadrant
        if srcVessel in [   Instrument.BCCMLabel,
                            Instrument.CocktailLabel,
                            Instrument.BeadLabel,
                            Instrument.LysisLabel,
                            Instrument.AntibodyLabel ]:
            self.__tracking[destQuad][Instrument.WasteLabel] = EMPTY

    #IAT - added protocolType to GetVolumes' parameters
    # protocolType is used to determine if protocol type is "whole blood positive"
    # Since whole blood positive is the only protocol type that adds dead volume to volumes,
    # we need to test each protocol sent to getVolumes to determine if dead volumes need to 
    # be added, or not

        
    def getVolumes(self, sampleVolume_uL, protocolType):
        '''Returns a list of ProtocolConsumable objects, holding volume 
        information. Each ProtocolConsumable object contains the volumes 
        required for a RELATIVE quadrant on the instrument; you would make
        this absolute by referring it back to a Sample object's initialQuadrant
        field.
        The sampleVolume_uL parameter specifies the sample volume (in uL) to
        do the various volume calculations with.'''
        consumableInfo = []         
        
        
        vessels = [ Instrument.CocktailLabel,
                    Instrument.BeadLabel,
                    Instrument.LysisLabel,
                    Instrument.AntibodyLabel,
                    Instrument.WasteLabel,
                    Instrument.BCCMLabel,
                    Instrument.SupernatentLabel,
                    Instrument.SampleLabel ]

        if prints == 1: print "\nBDR --In GET_VOLUMES"  #bdr
        if prints == 1: print "Protocol Type: %s in getVolumes" % (protocolType)   
                 
        isBloodPositive = 0                                 #IAT
        if (protocolType == "WholeBloodPositive"):          #IAT
               isBloodPositive = 1                          #IAT

        self.__maxLysisVol = [-1,-1,-1,-1,-1]
         
        if prints == 1: print "BDR -- GET_VOLS: Starting - set Lysis tube volume to not needed (-1)" # bdr
        
        # Create our tracking list, where we track the volumes needed for each
        # tube in each quadrant
        # Each quadrant can track the volume in (or need of) the vessels
        # contained in that quadrant.
        # Bulk media fluid is tracked per quadrant and the totalled at the end.
        self.__tracking = [{},]
        self.__tipboxRequired = [False,]
        quadrantRange = range(1, tesla.config.NUM_QUADRANTS + 1)
        for _ in quadrantRange:
            vesselDict = {}
            for vessel in vessels:
                vesselDict[vessel] = NOT_NEEDED
            self.__tracking.append(vesselDict)
            self.__tipboxRequired.append(False)           
        
        # Run through each command and update the appropriate vessel details
        for cmd in self.__cmds:
            if prints == 1: print "cmd.type = ",cmd.type  
            if cmd.type == 'TransportCommand':
                # Update tracking
                self.__updateTracking(cmd.srcVial,cmd.destVial,cmd.tiprackSpecified, cmd.tiprack,cmd.calculateVolume(sampleVolume_uL))#CR

            if cmd.type == 'MixCommand':
                # Update tracking (No volume, just track need for tube)
                self.__updateTracking(cmd.srcVial, cmd.srcVial,cmd.tiprackSpecified, cmd.tiprack,volume = 0)#CR
               
            elif cmd.type == 'TopUpVialCommand' or cmd.type == 'ResuspendVialCommand' or cmd.type == 'ResusMixCommand':
                # swap vials if there is a src vial but no dst vial
                fromVial = cmd.srcVial
                toVial = cmd.destVial
                if toVial == () and fromVial != ():
                    toVial = fromVial
                    fromVial = ()
                # Are we using BCCM ?
                if fromVial == ():
                    fromVial = (1,Instrument.BCCMLabel)
                volume = self.getWorkingVolume(sampleVolume_uL)
                # Update tracking
                self.__updateTracking(fromVial,toVial,cmd.tiprackSpecified, cmd.tiprack,volume)#CR
                
            elif cmd.type == 'FlushCommand' or cmd.type == 'PrimeCommand':
                dummySrc = ()       # Doesn't use any consumables for source
                volume = 0          
                # Update tracking (No source required)
                self.__updateTracking(dummySrc,cmd.destVial,cmd.tiprackSpecified, cmd.tiprack,volume)#CR
            elif cmd.type == 'MixTransCommand':
                self.__updateTracking(cmd.srcVial,cmd.destVial,cmd.tiprackSpecified, cmd.tiprack,cmd.calculateVolume2(sampleVolume_uL))
            elif cmd.type == 'TopUpMixTransCommand'or cmd.type == 'ResusMixSepTransCommand' or \
                 cmd.type == 'TopUpTransCommand':
                volume = self.getWorkingVolume(sampleVolume_uL)
                if cmd.type == 'TopUpTransCommand':
                    trans_vol = cmd.calculateVolume2(sampleVolume_uL)
                else:
                    trans_vol = cmd.calculateVolume3(sampleVolume_uL)
                #update topup/resus tracker
                #self.__updateTracking((1,Instrument.BCCMLabel),cmd.srcVial,cmd.tiprackSpecified, cmd.tiprack,volume)
                self.__updateTracking(cmd.srcVial,cmd.destVial,cmd.tiprackSpecified, cmd.tiprack,volume)
                #update transport  tracker
                self.__updateTracking(cmd.destVial,cmd.destVial2,cmd.tiprackSpecified, cmd.tiprack,trans_vol)
            elif cmd.type == 'TopUpTransSepTransCommand' or cmd.type == 'TopUpMixTransSepTransCommand':
                volume = self.getWorkingVolume(sampleVolume_uL)                #update topup/resus tracker
                if cmd.type == 'TopUpTransSepTransCommand':
                    trans_vol = cmd.calculateVolume2(sampleVolume_uL)
                    trans_vol2 = cmd.calculateVolume3(sampleVolume_uL)
                else:
                    trans_vol = cmd.calculateVolume3(sampleVolume_uL)
                    trans_vol2 = cmd.calculateVolume4(sampleVolume_uL)
                self.__updateTracking(cmd.srcVial,cmd.destVial,cmd.tiprackSpecified, cmd.tiprack,volume)
                #update transport  tracker
                self.__updateTracking(cmd.destVial,cmd.destVial2,cmd.tiprackSpecified, cmd.tiprack,trans_vol)
                self.__updateTracking(cmd.destVial2,cmd.destVial3,cmd.tiprackSpecified, cmd.tiprack,trans_vol2)
                

        ## Load the dead volume data for the vessels
        currentDeadVolume = self.__deadVolumeData.copy()
        instrumentConfigData = gHardwareData.Section ('Tesla')
        LoadSettings (currentDeadVolume, instrumentConfigData)

        for q in quadrantRange:
            required = False

            # We always need at least one quadrant returned,
            # even if it has nothing required
            if q == 1:
                required = True
            
            if prints == 1: print "\nBDR -- GETVOLS: Checking dead vols for quad %d" % (q) # bdr
            
            # Process this quadrant
            for vessel in vessels:
                if self.__tracking[q][vessel] == EMPTY or self.__tracking[q][vessel] > 0:
                    required = True
            
            # Is there anything required from this quadrant ?
            if required:
                cocktailVol = self.__tracking[q][Instrument.CocktailLabel]
                if cocktailVol >= 0:
                    # Add dead volume
                    if prints == 1: print "Cocktail Vol            : %f" % (cocktailVol)           
                    cocktailVol += float(currentDeadVolume[Instrument.DeadVolumeCocktailLabel]) 
                    if prints == 1: print "Cocktail Vol + Dead Vol : %f" % (cocktailVol)

                particleVol = self.__tracking[q][Instrument.BeadLabel]
                if particleVol >= 0:
                    # Add dead volume
                    if prints == 1: print "Particle Vol            : %f" % (particleVol)                     
                    particleVol += float(currentDeadVolume[Instrument.DeadVolumeBeadLabel])
                    if prints == 1: print "Particle Vol + Dead Vol : %f" % (particleVol)

		## lysisVol can get zeroed if tube is reused in some protocols so we hold the
                ## initial value as startLysisVol for reporting to GUI - bdr 
                lysisVol = self.__tracking[q][Instrument.LysisLabel]
                
                if lysisVol < self.__maxLysisVol[q]:    # bdr nov 2007
                    lysisVol = self.__maxLysisVol[q]

                if lysisVol >= 0:
                    # Add dead volume
                    if prints == 1: print "Lysis Vol              : %f" % (lysisVol)                       
                    lysisVol += float(currentDeadVolume[Instrument.DeadVolume50mlLabel ])                    
                    if prints == 1: print "Lysis Vol + Dead Vol   : %f" % (lysisVol)


                antibodyVol = self.__tracking[q][Instrument.AntibodyLabel]
                if antibodyVol >= 0:
                    # Add dead volume
                    if prints == 1: print "Anti Vol              : %f" % (antibodyVol)  
                    antibodyVol += float(currentDeadVolume[Instrument.DeadVolumeAntibodyLabel])
                    if prints == 1: print "Anti Vol + Dead Vol   : %f" % (antibodyVol)

                BCCMVol = self.__tracking[q][Instrument.BCCMLabel]

                pc = ProtocolConsumable(
                        quadrant = q, 
                        cocktailVolume_uL   = cocktailVol,
                        particleVolume_uL   = particleVol,
                        lysisVolume_uL      = lysisVol,
                        antibodyVolume_uL   = antibodyVol,
                        bulkBufferVolume_uL = BCCMVol,
                        wasteVesselRequired      = self.__tracking[q][Instrument.WasteLabel] >= 0,
                        separationVesselRequired = self.__tracking[q][Instrument.SupernatentLabel] >= 0,
                        sampleVesselRequired     = self.__tracking[q][Instrument.SampleLabel] >= 0,
                        sampleVesselVolumeRequired     = self.getMultipleSamples()[q-1] == u"true",
                        tipBoxRequired           = self.__tipboxRequired[q]
                        )
                if prints == 1: print "BDR -- GETVOLS: Final lysisVol appended to consumableInfo[] is %d for quad %d" % (lysisVol,q) # bdr
                consumableInfo.append(pc)
        if prints == 1: print "getVolumes done\n\n"
        
        return consumableInfo
  

class Feature(object):
    def __init__(self, name, desc, inputType, inputData):
	self.name = name
	self.desc = desc 
	self.inputType = inputType
	self.inputData = inputData
# eof

