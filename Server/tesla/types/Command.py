# 
# Command.py
# tesla.types.Command.py
#
# Protocol command type for the Tesla instrument control software
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
#    03/29/06 - pause command - RL

from ipl.utils.validation import validateNumber
from ipl.utils.InfoTable import InfoTable

import tesla.config
import tesla.logger #CR
from tesla.exception import TeslaException
from tesla.instrument.Instrument import Instrument

# -----------------------------------------------------------------------------

class CommandException(TeslaException):
    '''Exception for problems when creating Command objects, particularly
    from parsing bad data.'''
    pass

# -----------------------------------------------------------------------------

def CommandFactory(sequenceNumber, type, label, xmlNode = None):
    '''Simple factory for the Command class.
    If you provide an invalid type parameter, this function will throw a 
    standard NameError exception.
    If the xmlNode is defined, then we parse the xml node to get addition 
    parameters for the command.'''

    creationString = "%s(%d, '%s', '%s')" % (type, sequenceNumber, type, label)
    cmd = eval(creationString)
    if xmlNode:
        cmd.parseXML(xmlNode)
    return cmd

# -----------------------------------------------------------------------------

class Command(object):

    LEGAL_COMMANDS = [  'Transport', 'Mix', 'Incubate', 'Separate', 'TopUpVial',
                        'ResuspendVial', 'Flush', 'Prime', 'HomeAll', 'Park', 
                        'Demo', 'Pause','PumpLife','MixTrans', 'TopUpMixTrans' , 'ResusMixSepTrans','ResusMix',
                        'TopUpTrans', 'TopUpTransSepTrans', 'TopUpMixTransSepTrans', 'EndOfProtocol']
   
    workingVols = {}

    vialTable = InfoTable(tesla.config.VIAL_CODES_PATH)
   
    def __init__(self, sequenceNumber, type, label):
        '''Construct a base command instance with a sequence number (specifying
        where this command sits in the command sequence in the protocol), a
        type (one of those defined in Command.LEGAL_COMMANDS), and a human
        readable label.'''
        self.seq = sequenceNumber
        if not Command.isLegalCommandType(type):
            pass
        self.type = type 
        self.label = label
        self.executionTime = 0
        self.extensionTime = 0

    def getWorkingVolume(self, sample, protocol):
        '''For a specific sample and protocol, calculate and return the working
        volume. The result is cached so we don't have to recalculate this all
        the time.'''
        key = "%s:%s" % (sample, protocol)
        if key in Command.workingVols:
            return Command.workingVols[key]
        else:
            workingVol = protocol.getWorkingVolume(sample.volume)
            Command.workingVols[key] = workingVol
            return workingVol
        

    def createCall(self, sample = None, protocol = None):
        '''Returns a string that is ready to call on the Instrument object to
        initiate the execution of a level 1 workflow. This means that all of
        the arguments for the call are fully evaluated.
        This particular method in the parent Command class is abstract. Don't
        use it :)'''
        raise NotImplementedError("Abstract method; not used for %s:%s" % \
                (self.type, self.label))


    def parseXML(self, xmlNode):
        '''Abstract method for parsing XML into command data.'''
        raise NotImplementedError("Abstract method; not used for %s:%s" % \
                (self.type, self.label))


    def isLegal(self):
        '''Instance version of the static method for testing if this is a
        legal command.'''
        return Command.isLegalCommandType(self.type)

    def isServiceType(self):
        '''Returns True if this is a service command.'''
        return isinstance(self, ServiceCommand)

    def isWaitType(self):
        '''Returns True if this a wait command.'''
        return isinstance(self, WaitCommand)
    def isSeparateType(self):
        return isinstance(self, SeparateCommand)
    def isEndOfProtocolType(self):
        return isinstance(self, EndOfProtocolCommand)

    def isVolumeType(self):
        '''Return True if this is a volume command.'''
        return isinstance(self, VolumeCommand)

    def isTransportType(self):
        '''Return True if this is a Transport command.'''
        return isinstance(self, TransportCommand) 

    def isMixType(self):
        '''Return True if this is a Mix command.'''
        return isinstance(self, MixCommand)
    
    def isTopUpType(self):
        '''Return True if this is a TopUpVialCommand.'''
        return isinstance(self, TopUpVialCommand)
    
    def isResuspendType(self):
        '''Return True if this is a ResuspendVialCommand.'''
        return isinstance(self, ResuspendVialCommand)

    def isMixTransType(self):
        return isinstance(self, MixTransCommand)
    def isTopUpMixTransType(self):
        return isinstance(self, TopUpMixTransCommand)
    def isResusMixSepTransType(self):
        return isinstance(self, ResusMixSepTransCommand)
    def isResusMixType(self):
        return isinstance(self, ResusMixCommand)
    def isTopUpTransType(self):
        return isinstance(self, TopUpTransCommand)
    def isTopUpTransSepTransType(self):
        return isinstance(self, TopUpTransSepTransCommand)
    def isTopUpMixTransSepTransType(self):
        return isinstance(self, TopUpMixTransSepTransCommand)
    def includesTopUpOrResuspendInType(self):
        return self.isTopUpType() or self.isResuspendType() or \
                self.isTopUpMixTransType() or self.isResusMixSepTransType() or self.isResusMixType() or \
                self.isTopUpTransType() or self.isTopUpTransSepTransType() or self.isTopUpMixTransSepTransType()
    def isTopUpOrResuspendMergeCandidateType(self):
        return self.isTopUpType() or self.isResuspendType() or \
                self.isTopUpMixTransType() or self.isResusMixType() or \
                self.isTopUpTransType() 

    def _parseVialInfo(self, vialString):
        '''Parse the vial string and return a tuple of quadrant and the vial name.'''
        if vialString:
            try:
                quadrant = int(vialString[3:5])
                vialName = self.vialTable.getMessage(vialString)
            except Exception as msg:
                raise CommandException("Error (%s) parsing vial data [%s]" % (msg, vialString))
            return (quadrant, vialName)
        else:
            return ()

    # --- Set up our static method(s) -----------------------------------------

    def isLegalCommandType(cmdType):
        '''Is the command a legal one? This is a static method.'''
        cmdType = cmdType.split('Command')[0]
        return cmdType in Command.LEGAL_COMMANDS

    def isWaitCommand(cmdType):
        '''Returns True if the command is just a 'wait'-type command, such as
        an Incubate or Separate command'''
        cmdType = cmdType.split('Command')[0]
        return cmdType in ['Incubate', 'Separate', 'EndOfProtocol']
    
    isLegalCommandType = staticmethod(isLegalCommandType)
    isWaitCommand = staticmethod(isWaitCommand)    
    

# ----------------------------------------------------------------------------

class ServiceCommand(Command):
    '''Commands related to servicing the instrument.'''
    
    def parseXML(self, xmlNode):
        '''Does nothing in the base ServiceCommand class.'''
        pass


class ParkCommand(ServiceCommand):
    '''A command to park the Z-Theta robot'''

    def createCall(self, sample, protocol):
        return "Park(id=%d, seq=%d)" % (sample.ID, self.seq)


class HomeAllCommand(ServiceCommand):
    '''A command to home all axes.'''

    def createCall(self, sample, protocol):
        return "HomeAll(id=%d, seq=%d)" % (sample.ID, self.seq)


class DemoCommand(ServiceCommand):
    '''A command to run some form of demo mode.'''
    
    def __init__(self, sequenceNumber, type, label):
        ServiceCommand.__init__(self, sequenceNumber, type, label)
        self.iterations = 1
        self.sef        = 0;  
    def parseXML(self, xmlNode):
        '''Sets the number of times we repeat the Demo command.'''
        self.iterations = int(xmlNode.getAttribute('iterations'))
        self.seq        = int(xmlNode.getAttribute('seq'));

    def createCall(self, sample, protocol):
        if (self.seq == 1):
            return "Demo(iterations=%d, id=%d, seq=%d)" % \
                (self.iterations, sample.ID, self.seq)
        elif (self.seq == 2):
            return "Demo5mL(iterations=%d, id=%d, seq=%d)" % \
                (self.iterations, sample.ID, self.seq)
        elif (self.seq == 3):
            return "Demo1mL(iterations=%d, id=%d, seq=%d)" % \
                (self.iterations, sample.ID, self.seq)

class PumpLifeCommand(ServiceCommand):
    '''A command to run some form of demo mode.'''
    
    def __init__(self, sequenceNumber, type, label):
        ServiceCommand.__init__(self, sequenceNumber, type, label)
        self.iterations = 1
    
    def parseXML(self, xmlNode):
        '''Sets the number of times we repeat the Demo command.'''
        self.iterations = int(xmlNode.getAttribute('iterations'))

    def createCall(self, sample, protocol):
        return "PumpLife(iterations=%d, id=%d, seq=%d)" % \
                (self.iterations, sample.ID, self.seq)
# ----------------------------------------------------------------------------

class WaitCommand(Command):
    '''A command that involves the sample waiting for a while; eg. Incubate
    or Separate.'''
   
    def __init__(self, sequenceNumber, type, label):
        Command.__init__(self, sequenceNumber, type, label)
        self.minPeriod = 0
        self.maxPeriod = 0

    def parseXML(self, xmlNode):
        '''Parse out the wait duration (in seconds) from the XML data.'''
        duration = 0
        self.extensionTime = int(xmlNode.getAttribute('extensionTime'))
        for child in xmlNode.childNodes:
            if child.nodeName == 'processingTime':
                duration = int(child.getAttribute('duration'))
                break
        self.setPeriods( duration, duration + self.extensionTime )

    def setPeriods(self, minPeriod_secs, maxPeriod_secs):
        '''Set the minimum and maximum periods that we can wait for (in 
        seconds).'''
        self.minPeriod = minPeriod_secs
        self.maxPeriod = maxPeriod_secs
        if self.minPeriod > self.maxPeriod:
            self.minPeriod, self.maxPeriod = self.maxPeriod, self.minPeriod

    def createCall(self, sample, protocol):
        cmdType = self.type.split('Command')[0]
        return "%s(%d, id=%d, seq=%d)" % (cmdType, self.minPeriod, sample.ID, self.seq)


class IncubateCommand(WaitCommand):
    '''Incubate command'''
    pass


class SeparateCommand(WaitCommand):
    '''Separate  command'''
    pass

class EndOfProtocolCommand(WaitCommand):
    pass

# RL - pause command - 03/29/06
class PauseCommand(WaitCommand):
    '''A command to pause robot'''
    '''
    def __init__(self, sequenceNumber, type, label):
        WaitCommand.__init__(self, sequenceNumber, type, label)
        self.caption = ""
    
    def parseXML(self, xmlNode):
        WaitCommand.parseXML(self, xmlNode)
        self.caption = string(xmlNode.getAttribute('label'))'''
    
    def createCall(self, sample, protocol):
        cmdType = self.type.split('Command')[0]
        return "%s(%d, id=%d, seq=%d, label='%s')" % (cmdType, self.minPeriod, sample.ID, self.seq, self.label)
        
    pass
    

# ----------------------------------------------------------------------------

class VolumeCommand(Command):
    '''A command for handling volumes. All volumes are in uL. If a vial is not 
    used, set it to None.'''

    def __init__(self, sequenceNumber, type, label):
        Command.__init__(self, sequenceNumber, type, label)
        # Vial members
        self.srcVial = None                # These will be tuples of sector # and tube name
        self.destVial = None
        self.srcVial2 = None                # These will be tuples of sector # and tube name
        self.destVial2 = None
        self.srcVial3 = None                # These will be tuples of sector # and tube name
        self.destVial3 = None
        # Volume-related members -- all volumes are in uL (microlitres)
        self.workingVolume = 0                # The working volume for this command
        self.relative = False            # Relative or absolute volumes?
        self.proportion = 0.0           # Relative volume proportion
        self.absVolume = 0              # Absolute volume value
        self.volume = 0                 # Volume used by an instance of this command
        self.tiprackSpecified = False        # if not to use default tip rack #1
        self.tiprack = 1                                # id of tip rack to use

        self.numOfStages = 1
        self.relative2 = False            # Relative or absolute volumes?
        self.proportion2 = 0.0           # Relative volume proportion
        self.absVolume2 = 0              # Absolute volume value
        self.relative3 = False            # Relative or absolute volumes?
        self.proportion3 = 0.0           # Relative volume proportion
        self.absVolume3 = 0              # Absolute volume value
        self.relative4 = False            # Relative or absolute volumes?
        self.proportion4 = 0.0           # Relative volume proportion
        self.absVolume4 = 0              # Absolute volume value
        self.relative5 = False            # Relative or absolute volumes?
        self.proportion5 = 0.0           # Relative volume proportion
        self.absVolume5 = 0              # Absolute volume value
      

    def parseXML(self, xmlNode):
        '''Extract the vial information etc. from the XML data.'''
        self.extensionTime = int(xmlNode.getAttribute('extensionTime'))
        self.freeAirDispense = xmlNode.getAttribute('freeAirDispense') == 'true'
        self.useBufferTip = xmlNode.getAttribute('useBufferTip') == 'true'
        
        if xmlNode.hasAttribute('numOfStages'):
            self.numOfStages = int(xmlNode.getAttribute('numOfStages'))    
        
        # CR
        try:
            self.tiprack = int(xmlNode.getAttribute('tipRack'))
            self.tiprackSpecified = True
        except Exception:
            self.tiprack = 1;
        if xmlNode.hasAttribute('workingVolume_uL'):
            self.workingVolume = int(xmlNode.getAttribute('workingVolume_uL'))
        for child in xmlNode.childNodes:
            if child.nodeName == 'vials':
                src = child.getAttribute('src')
                dest = child.getAttribute('dest')
                self.setVials(src, dest)
            elif child.nodeName == 'vials2':
                src = child.getAttribute('src')
                dest = child.getAttribute('dest')
                self.setVials2(src, dest)
            elif child.nodeName == 'vials3':
                src = child.getAttribute('src')
                dest = child.getAttribute('dest')
                self.setVials3(src, dest)
            elif child.nodeName == 'absoluteVolume':
                self.relative = False
                self.absVolume = int(child.getAttribute('value_uL'))
            elif child.nodeName == 'relativeVolume':
                self.relative = True
                self.proportion = float(child.getAttribute('proportion'))
            elif child.nodeName == 'absoluteVolume2' and self.numOfStages>=2:
                self.relative2 = False
                self.absVolume2 = int(child.getAttribute('value_uL'))
            elif child.nodeName == 'relativeVolume2' and self.numOfStages>=2:
                self.relative2 = True
                self.proportion2 = float(child.getAttribute('proportion'))
            elif child.nodeName == 'absoluteVolume3' and self.numOfStages>=3:
                self.relative3 = False
                self.absVolume3 = int(child.getAttribute('value_uL'))
            elif child.nodeName == 'relativeVolume3' and self.numOfStages>=3:
                self.relative3 = True
                self.proportion3 = float(child.getAttribute('proportion'))
            elif child.nodeName == 'absoluteVolume4' and self.numOfStages>=4:
                self.relative4 = False
                self.absVolume4 = int(child.getAttribute('value_uL'))
            elif child.nodeName == 'relativeVolume4' and self.numOfStages>=4:
                self.relative4 = True
                self.proportion4 = float(child.getAttribute('proportion'))
            elif child.nodeName == 'absoluteVolume5' and self.numOfStages>=5:
                self.relative5 = False
                self.absVolume5 = int(child.getAttribute('value_uL'))
            elif child.nodeName == 'relativeVolume5' and self.numOfStages>=5:
                self.relative5 = True
                self.proportion5 = float(child.getAttribute('proportion'))

        # Now make sure that we have sensible values... 
        if self.relative and self.proportion < 0.0:
            raise CommandException("Relative volume < 0 (%0.2f)" % (self.proportion))
        elif self.absVolume < 0:
            raise CommandException("Absolute volume < 0 (%0d)" % (self.absVolume))
        if self.relative2 and self.proportion2 < 0.0:
            raise CommandException("Relative volume 2 < 0 (%0.2f)" % (self.proportion2))
        elif self.absVolume2 < 0:
            raise CommandException("Absolute volume 2 < 0 (%0d)" % (self.absVolume2))
        if self.relative3 and self.proportion3 < 0.0:
            raise CommandException("Relative volume 3 < 0 (%0.2f)" % (self.proportion3))
        elif self.absVolume3 < 0:
            raise CommandException("Absolute volume 3 < 0 (%0d)" % (self.absVolume3))
        if self.relative4 and self.proportion4 < 0.0:
            raise CommandException("Relative volume 4 < 0 (%0.2f)" % (self.proportion4))
        elif self.absVolume4 < 0:
            raise CommandException("Absolute volume 4 < 0 (%0d)" % (self.absVolume4))
        if self.relative5 and self.proportion5 < 0.0:
            raise CommandException("Relative volume 5 < 0 (%0.2f)" % (self.proportion5))
        elif self.absVolume5 < 0:
            raise CommandException("Absolute volume 5 < 0 (%0d)" % (self.absVolume5))


    def copyVolumeCommand(self, oldCmd):
        self.srcVial = oldCmd.srcVial                
        self.destVial = oldCmd.destVial
        self.srcVial2 = oldCmd.srcVial2                
        self.destVial2 = oldCmd.destVial2
        self.srcVial3 = oldCmd.srcVial3                
        self.destVial3 = oldCmd.destVial3

        self.workingVolume = oldCmd.workingVolume                
        self.relative = oldCmd.relative            
        self.proportion = oldCmd.proportion           
        self.absVolume = oldCmd.absVolume              
        self.volume = oldCmd.volume                 
        self.tiprackSpecified = oldCmd.tiprackSpecified        
        self.tiprack = oldCmd.tiprack
        self.extensionTime = oldCmd.extensionTime
        self.freeAirDispense = oldCmd.freeAirDispense
        self.useBufferTip = oldCmd.useBufferTip
        

        self.numOfStages = oldCmd.numOfStages
        self.relative2 = oldCmd.relative2            
        self.proportion2 = oldCmd.proportion2           
        self.absVolume2 = oldCmd.absVolume2              
        self.relative3 = oldCmd.relative3            
        self.proportion3 = oldCmd.proportion3           
        self.absVolume3 = oldCmd.absVolume3              
        self.relative4 = oldCmd.relative4            
        self.proportion4 = oldCmd.proportion4           
        self.absVolume4 = oldCmd.absVolume4              
        self.relative5 = oldCmd.relative5            
        self.proportion5 = oldCmd.proportion5           
        self.absVolume5 = oldCmd.absVolume5              
        

    def getContainerString(self, sample, vial):
        '''Return a string of what is needed to eval an Instrument container 
        instance with this command given a specific sample and vial tuple (from 
        __createContainer above).'''
        if vial == ():
            vialString = None
        else:
            try:
                sector = (vial[0] + sample.initialQuadrant) - 1
                vialName = vial[1]
                
                # With some containers, we should ignore the sector as they
                # are off-board (ie. sector 0); eg. the BCCM
                if vialName in [Instrument.BCCMLabel,]:
                    sector = 0
                vialString = "self.instrument.ContainerAt(%d, '%s')" % (sector, vialName)
            except Exception as msg:
                raise CommandException("CO: Vial must be (quadrant, name), not %s (%s)" % \
                        (vial, msg))
        return vialString
    

    def setVials(self, srcVial, destVial):
        '''Set up the vial tuples that we will use to call up Container 
        objects that we will pass as a workflow parameter.'''
        self.srcVial = self._parseVialInfo(srcVial)
        self.destVial = self._parseVialInfo(destVial)
        
    def setVials2(self, srcVial, destVial):
        '''Set up the vial tuples that we will use to call up Container 
        objects that we will pass as a workflow parameter.'''
        self.srcVial2 = self._parseVialInfo(srcVial)
        self.destVial2 = self._parseVialInfo(destVial)
        
    def setVials3(self, srcVial, destVial):
        '''Set up the vial tuples that we will use to call up Container 
        objects that we will pass as a workflow parameter.'''
        self.srcVial3 = self._parseVialInfo(srcVial)
        self.destVial3 = self._parseVialInfo(destVial)


    def calculateVolume_helper(self, isRelative, tmpProportion, tmpAbsVolume, sampleVolume_uL = 0):
        '''Calculate the volume that we need for this command, which could be
        absolute or could be relative to the sample volume.'''

        if isRelative:
            # Relative volume (to the sample volume)
            volume = int(round(tmpProportion * sampleVolume_uL))
        else:
            # An absolute volume, as defined in the protocol
            volume = tmpAbsVolume

        return volume

    def calculateVolume(self, sampleVolume_uL = 0):
        return self.calculateVolume_helper(self.relative,self.proportion,self.absVolume,sampleVolume_uL)
    def calculateVolume2(self, sampleVolume_uL = 0):
        if self.numOfStages>=2:
            return self.calculateVolume_helper(self.relative2,self.proportion2,self.absVolume2,sampleVolume_uL)
        else:
            return 0;
    def calculateVolume3(self, sampleVolume_uL = 0):
        if self.numOfStages>=3:
            return self.calculateVolume_helper(self.relative3,self.proportion3,self.absVolume3,sampleVolume_uL)
        else:
            return 0;
    def calculateVolume4(self, sampleVolume_uL = 0):
        if self.numOfStages>=4:
            return self.calculateVolume_helper(self.relative4,self.proportion4,self.absVolume4,sampleVolume_uL)
        else:
            return 0;
    def calculateVolume5(self, sampleVolume_uL = 0):
        if self.numOfStages>=5:
            return self.calculateVolume_helper(self.relative5,self.proportion5,self.absVolume5,sampleVolume_uL)
        else:
            return 0;

    def getCalcVolAndRelativeAndProportion(self, stage_num, sampleVolume_uL = 0):
        if stage_num == 1:
            vol = self.calculateVolume(sampleVolume_uL)
            isRelative = self.relative
            prop = self.proportion
        elif stage_num == 2:
            vol = self.calculateVolume2(sampleVolume_uL)
            isRelative = self.relative2
            prop = self.proportion2
        elif stage_num == 3:
            vol = self.calculateVolume3(sampleVolume_uL)
            isRelative = self.relative3
            prop = self.proportion3
        elif stage_num == 4:
            vol = self.calculateVolume4(sampleVolume_uL)
            isRelative = self.relative4
            prop = self.proportion4
        elif stage_num == 5:
            vol = self.calculateVolume5(sampleVolume_uL)
            isRelative = self.relative5
            prop = self.proportion5
        else:
            raise CommandException("CO: getMixVolAndTime() failed, stage_num=%d seq=%d" % (stage_num, self.seq))
        return (vol,isRelative,prop)

# -----------------------------------------------------------------------------

class TransportCommand(VolumeCommand):
    '''Transport (fluid) command'''

    def __init__(self, sequenceNumber, type, label):
        '''Constructor for the Transport command. Sets up a freeAirDispense
        member, which is False by default.'''
        super(TransportCommand, self).__init__(sequenceNumber, type, label)
        # Default for transports is that we don't do a free air dispense
        self.freeAirDispense = False
        self.useBufferTip = False

    def parseXML(self, xmlNode):
        '''Parse out the volume data and then the freeAirDispense flag.'''
        VolumeCommand.parseXML(self, xmlNode)
        try:
            useBufferFlag = xmlNode.volume[0].useBufferTip.lower() == 'true'
            self.setUseBufferTip(useBufferFlag)
        except AttributeError:
            # No usebuffertip flag defined? Keep going
            pass
        try:
            freeAirFlag = xmlNode.volume[0].freeair.lower() == 'true'
            self.setFreeAirDispense(freeAirFlag)
        except AttributeError:
            # No free air dispense flag defined? Keep going
            pass
        
    def setFreeAirDispense(self, usingFreeAirDispense = False):
        '''Set the freeAirDispense member (to True or False).'''
        self.freeAirDispense = usingFreeAirDispense

    def setUseBufferTip(self, usingBufferTip = False):
        '''Set the useBufferTip member (to True or False).'''
        self.useBufferTip = usingBufferTip
    
    def createCall(self, sample, protocol):
        '''Create the workflow call.'''
        srcVial = self.getContainerString(sample, self.srcVial)
        destVial = self.getContainerString(sample, self.destVial)
        volume_uL = self.calculateVolume(sample.volume)
        workVolume = self.getWorkingVolume(sample, protocol)
        initialQuadrant = sample.initialQuadrant
        return "Transport(destVial=%s, srcVial=%s, volume_ul=%0.2f, workVolume=%0.2f, usingFreeAirDispense=%s, usingBufferTip=%s, id=%d, seq=%d, tiprackSpecified=%s, tiprack=%d, initialQuadrant=%d)" % \
                (destVial, srcVial, volume_uL, workVolume, self.freeAirDispense, self.useBufferTip, sample.ID, self.seq, self.tiprackSpecified, self.tiprack, initialQuadrant) #CR - added tiprack stuff

class MixCommand(VolumeCommand):
    '''Mix (fluid) command'''
    def __init__(self, sequenceNumber, type, label):
        VolumeCommand.__init__(self, sequenceNumber, type, label)
        self.mixCycles = -1
        self.tipTubeBottomGap = -1
        
    def createCall(self, sample, protocol):
        vial = self.getContainerString(sample, self.srcVial)
        #volume_uL = self.calculateVolume(self.volume)   #?? values get overridden anyway and param should be sample volume not self volume...
        if self.relative:
            volume_uL = self.proportion
        else:
            volume_uL = self.absVolume
        workVolume = self.getWorkingVolume(sample, protocol)
        initialQuadrant = sample.initialQuadrant

        call = "Mix(vial=%s, workVolume=%0.2f, id=%d, seq=%d, tiprackSpecified=%s, tiprack=%d, initialQuadrant=%d, mixCycles=%d, tipTubeBottomGap=%d, isRelative=%d, volume_uL=%0.2f )" % \
                 (vial, workVolume, sample.ID, self.seq, self.tiprackSpecified, self.tiprack, initialQuadrant, self.mixCycles, self.tipTubeBottomGap, self.relative, volume_uL)#CR - added tiprack stuff
        return call

    def parseXML(self, xmlNode):
        '''Parse out the volume data and then the freeAirDispense flag.'''
        VolumeCommand.parseXML(self, xmlNode)
        
        if xmlNode.hasAttribute('mixCycles'):
            self.mixCycles = int(xmlNode.getAttribute('mixCycles'))
        if xmlNode.hasAttribute('tipTubeBottomGap'):
            self.tipTubeBottomGap = int(xmlNode.getAttribute('tipTubeBottomGap'))


class TopUpVialCommand(VolumeCommand):
    '''Top up vial (with fluid) command'''
    
    def createCall(self, sample, protocol):
        srcVial = self.getContainerString(sample, self.srcVial)
        destVial = self.getContainerString(sample, self.destVial)
        # If the two vials have been accidently mis-specified, swap them
        # We should never have an undefined destVial and a specified srcVial,
        # where is it topping up to???
        if destVial == None and srcVial != None:
            destVial = srcVial
            srcVial = None
        if self.relative:
            workVolume = self.calculateVolume(sample.volume) + sample.volume
            workVolume = min(10000,workVolume)
        else:
            workVolume = self.getWorkingVolume(sample, protocol)
        initialQuadrant = sample.initialQuadrant
        return "TopUpVial(destVial=%s, srcVial=%s, workVolume=%0.2f, id=%d, seq=%d, tiprackSpecified=%s, tiprack=%d, initialQuadrant=%d)" % \
                (destVial, srcVial, workVolume, sample.ID, self.seq, self.tiprackSpecified, self.tiprack, initialQuadrant) #CR - added tiprack stuff



class ResuspendVialCommand(VolumeCommand):
    '''Resuspend vial (normally with BCCM) command'''
    
    def createCall(self, sample, protocol):
        srcVial = self.getContainerString(sample, self.srcVial)
        destVial = self.getContainerString(sample, self.destVial)
        # If the two vials have been accidently mis-specified, swap them
        # We should never have an undefined destVial and a specified srcVial,
        # what is it resuspending into???
        if destVial == None and srcVial != None:
            destVial = srcVial
            srcVial = None
        if self.relative:
            workVolume = self.calculateVolume(sample.volume) + sample.volume
            workVolume = min(10000,workVolume)
        else:
            workVolume = self.getWorkingVolume(sample, protocol)            
        initialQuadrant = sample.initialQuadrant
        return "ResuspendVial(destVial=%s, srcVial=%s, workVolume=%0.2f, id=%d, seq=%d, tiprackSpecified=%s, tiprack=%d, initialQuadrant=%d)" % \
                (destVial, srcVial, workVolume, sample.ID, self.seq, self.tiprackSpecified, self.tiprack, initialQuadrant) #CR - added tiprack stuff

class MixTransCommand(VolumeCommand):
    def __init__(self, sequenceNumber, type, label):
        VolumeCommand.__init__(self, sequenceNumber, type, label)
        self.mixCycles = -1
        self.tipTubeBottomGap = -1
        self.freeAirDispense = False
        self.useBufferTip = False
        
    def createCall(self, sample, protocol):
        srcVial = self.getContainerString(sample, self.srcVial)
        destVial = self.getContainerString(sample, self.destVial)
        if self.relative:
            mix_volume_uL = self.proportion
        else:
            mix_volume_uL = self.absVolume

        trans_volume_uL = self.calculateVolume2(sample.volume)  #!!!! stage dependent  #from transport
        
        initialQuadrant = sample.initialQuadrant

        call = "MixTrans(destVial=%s, srcVial=%s, id=%d, seq=%d, tiprackSpecified=%s, tiprack=%d, initialQuadrant=%d, \
        mixCycles=%d, tipTubeBottomGap=%d, isRelative=%d, mix_volume_uL=%0.2f, \
        trans_volume_uL=%0.2f, usingFreeAirDispense=%s, usingBufferTip=%s)" % \
                 (destVial, srcVial, sample.ID, self.seq, self.tiprackSpecified, self.tiprack, initialQuadrant,
                  self.mixCycles, self.tipTubeBottomGap, self.relative, mix_volume_uL,
                  trans_volume_uL, self.freeAirDispense, self.useBufferTip)


        return call

    def parseXML(self, xmlNode):
        '''Parse out the volume data and then the freeAirDispense flag.'''
        VolumeCommand.parseXML(self, xmlNode)
        
        if xmlNode.hasAttribute('mixCycles'):
            self.mixCycles = int(xmlNode.getAttribute('mixCycles'))
        if xmlNode.hasAttribute('tipTubeBottomGap'):
            self.tipTubeBottomGap = int(xmlNode.getAttribute('tipTubeBottomGap'))
        try:
            useBufferFlag = xmlNode.volume[0].useBufferTip.lower() == 'true'
            self.setUseBufferTip(useBufferFlag)
        except AttributeError:
            # No usebuffertip flag defined? Keep going
            pass
        try:
            freeAirFlag = xmlNode.volume[0].freeair.lower() == 'true'
            self.setFreeAirDispense(freeAirFlag)
        except AttributeError:
            # No free air dispense flag defined? Keep going
            pass
    def setFreeAirDispense(self, usingFreeAirDispense = False):
        '''Set the freeAirDispense member (to True or False).'''
        self.freeAirDispense = usingFreeAirDispense

    def setUseBufferTip(self, usingBufferTip = False):
        '''Set the useBufferTip member (to True or False).'''
        self.useBufferTip = usingBufferTip

class TopUpMixTransCommand(VolumeCommand):
    def __init__(self, sequenceNumber, type, label):
        VolumeCommand.__init__(self, sequenceNumber, type, label)
        self.mixCycles = -1
        self.tipTubeBottomGap = -1
        self.freeAirDispense = False
        self.useBufferTip = False
        
    def createCall(self, sample, protocol):
        #bufferVial =  self.getContainerString(sample,self._parseVialInfo(TPC0001)) #assume fix buffer  #dont bother just use default
        srcVial = self.getContainerString(sample, self.srcVial)
        destVial = self.getContainerString(sample, self.destVial)
        destVial2 = self.getContainerString(sample, self.destVial2)
        if self.relative:
            workVolume = self.calculateVolume(sample.volume) + sample.volume
            workVolume = min(10000,workVolume)
        else:
            workVolume = self.getWorkingVolume(sample, protocol)
        
        if self.relative2:
            mix_volume_uL = self.proportion2
        else:
            mix_volume_uL = self.absVolume2

        trans_volume_uL = self.calculateVolume3(sample.volume)  #!!!! stage dependent  #from transport
        
        initialQuadrant = sample.initialQuadrant

        call = "TopUpMixTrans(destVial=%s, srcVial=%s, workVolume=%0.2f, id=%d, seq=%d, tiprackSpecified=%s, tiprack=%d, initialQuadrant=%d, \
        mixCycles=%d, tipTubeBottomGap=%d, isRelative=%d, mix_volume_uL=%0.2f, \
        trans_volume_uL=%0.2f, usingFreeAirDispense=%s, usingBufferTip=%s, destVial2=%s)" % \
                 (destVial, srcVial, workVolume, sample.ID, self.seq, self.tiprackSpecified, self.tiprack, initialQuadrant,
                  self.mixCycles, self.tipTubeBottomGap, self.relative2, mix_volume_uL,
                  trans_volume_uL, self.freeAirDispense, self.useBufferTip,destVial2)


        return call

    def parseXML(self, xmlNode):
        '''Parse out the volume data and then the freeAirDispense flag.'''
        VolumeCommand.parseXML(self, xmlNode)
        
        if xmlNode.hasAttribute('mixCycles'):
            self.mixCycles = int(xmlNode.getAttribute('mixCycles'))
        if xmlNode.hasAttribute('tipTubeBottomGap'):
            self.tipTubeBottomGap = int(xmlNode.getAttribute('tipTubeBottomGap'))
        try:
            useBufferFlag = xmlNode.volume[0].useBufferTip.lower() == 'true'
            self.setUseBufferTip(useBufferFlag)
        except AttributeError:
            # No usebuffertip flag defined? Keep going
            pass
        try:
            freeAirFlag = xmlNode.volume[0].freeair.lower() == 'true'
            self.setFreeAirDispense(freeAirFlag)
        except AttributeError:
            # No free air dispense flag defined? Keep going
            pass
    def setFreeAirDispense(self, usingFreeAirDispense = False):
        '''Set the freeAirDispense member (to True or False).'''
        self.freeAirDispense = usingFreeAirDispense

    def setUseBufferTip(self, usingBufferTip = False):
        '''Set the useBufferTip member (to True or False).'''
        self.useBufferTip = usingBufferTip

class ResusMixSepTransCommand(VolumeCommand):
    def __init__(self, sequenceNumber, type, label):
        VolumeCommand.__init__(self, sequenceNumber, type, label)
        self.mixCycles = -1
        self.tipTubeBottomGap = -1
        self.freeAirDispense = False
        self.useBufferTip = False
        self.duration = 0
        
    def createCall(self, sample, protocol):
        #bufferVial =  self.getContainerString(sample,self._parseVialInfo(TPC0001)) #assume fix buffer  #dont bother just use default
        srcVial = self.getContainerString(sample, self.srcVial)
        destVial = self.getContainerString(sample, self.destVial)
        destVial2 = self.getContainerString(sample, self.destVial2)
        if self.relative:
            workVolume = self.calculateVolume(sample.volume) + sample.volume
            workVolume = min(10000,workVolume)
        else:
            workVolume = self.getWorkingVolume(sample, protocol)
        
        if self.relative2:
            mix_volume_uL = self.proportion2
        else:
            mix_volume_uL = self.absVolume2

        trans_volume_uL = self.calculateVolume3(sample.volume)  #!!!! stage dependent  #from transport
        
        initialQuadrant = sample.initialQuadrant

        call = "ResusMixSepTrans(destVial=%s, srcVial=%s, workVolume=%0.2f, id=%d, seq=%d, tiprackSpecified=%s, tiprack=%d, initialQuadrant=%d, \
        mixCycles=%d, tipTubeBottomGap=%d, isRelative=%d, mix_volume_uL=%0.2f, \
        trans_volume_uL=%0.2f, usingFreeAirDispense=%s, usingBufferTip=%s, duration=%d, destVial2=%s)" % \
                 (destVial, srcVial, workVolume, sample.ID, self.seq, self.tiprackSpecified, self.tiprack, initialQuadrant,
                  self.mixCycles, self.tipTubeBottomGap, self.relative2, mix_volume_uL,
                  trans_volume_uL, self.freeAirDispense, self.useBufferTip, self.duration,destVial2)


        return call

    def parseXML(self, xmlNode):
        '''Parse out the volume data and then the freeAirDispense flag.'''
        VolumeCommand.parseXML(self, xmlNode)
        
        if xmlNode.hasAttribute('mixCycles'):
            self.mixCycles = int(xmlNode.getAttribute('mixCycles'))
        if xmlNode.hasAttribute('tipTubeBottomGap'):
            self.tipTubeBottomGap = int(xmlNode.getAttribute('tipTubeBottomGap'))
        if xmlNode.hasAttribute('duration'):
            self.duration = int(xmlNode.getAttribute('duration'))
        try:
            useBufferFlag = xmlNode.volume[0].useBufferTip.lower() == 'true'
            self.setUseBufferTip(useBufferFlag)
        except AttributeError:
            # No usebuffertip flag defined? Keep going
            pass
        try:
            freeAirFlag = xmlNode.volume[0].freeair.lower() == 'true'
            self.setFreeAirDispense(freeAirFlag)
        except AttributeError:
            # No free air dispense flag defined? Keep going
            pass
    def setFreeAirDispense(self, usingFreeAirDispense = False):
        '''Set the freeAirDispense member (to True or False).'''
        self.freeAirDispense = usingFreeAirDispense

    def setUseBufferTip(self, usingBufferTip = False):
        '''Set the useBufferTip member (to True or False).'''
        self.useBufferTip = usingBufferTip


class ResusMixCommand(VolumeCommand):
    def __init__(self, sequenceNumber, type, label):
        VolumeCommand.__init__(self, sequenceNumber, type, label)
        self.mixCycles = -1
        self.tipTubeBottomGap = -1
        
    def createCall(self, sample, protocol):
        #bufferVial =  self.getContainerString(sample,self._parseVialInfo(TPC0001)) #assume fix buffer  #dont bother just use default
        srcVial = self.getContainerString(sample, self.srcVial)
        destVial = self.getContainerString(sample, self.destVial)
        if self.relative:
            workVolume = self.calculateVolume(sample.volume) + sample.volume
            workVolume = min(10000,workVolume)
        else:
            workVolume = self.getWorkingVolume(sample, protocol)
        
        if self.relative2:
            mix_volume_uL = self.proportion2
        else:
            mix_volume_uL = self.absVolume2
        
        initialQuadrant = sample.initialQuadrant

        call = "ResusMix(destVial=%s, srcVial=%s, workVolume=%0.2f, id=%d, seq=%d, tiprackSpecified=%s, tiprack=%d, initialQuadrant=%d, \
        mixCycles=%d, tipTubeBottomGap=%d, isRelative=%d, mix_volume_uL=%0.2f)" % \
                 (destVial, srcVial, workVolume, sample.ID, self.seq, self.tiprackSpecified, self.tiprack, initialQuadrant,
                  self.mixCycles, self.tipTubeBottomGap, self.relative2, mix_volume_uL)


        return call

    def parseXML(self, xmlNode):
        '''Parse out the volume data and then the freeAirDispense flag.'''
        VolumeCommand.parseXML(self, xmlNode)
        
        if xmlNode.hasAttribute('mixCycles'):
            self.mixCycles = int(xmlNode.getAttribute('mixCycles'))
        if xmlNode.hasAttribute('tipTubeBottomGap'):
            self.tipTubeBottomGap = int(xmlNode.getAttribute('tipTubeBottomGap'))

class TopUpTransCommand(VolumeCommand):
    def __init__(self, sequenceNumber, type, label):
        VolumeCommand.__init__(self, sequenceNumber, type, label)
        self.freeAirDispense = False
        self.useBufferTip = False
        
    def createCall(self, sample, protocol):
        #bufferVial =  self.getContainerString(sample,self._parseVialInfo(TPC0001)) #assume fix buffer  #dont bother just use default
        srcVial = self.getContainerString(sample, self.srcVial)
        destVial = self.getContainerString(sample, self.destVial)
        destVial2 = self.getContainerString(sample, self.destVial2)
        if self.relative:
            workVolume = self.calculateVolume(sample.volume) + sample.volume
            workVolume = min(10000,workVolume)
        else:
            workVolume = self.getWorkingVolume(sample, protocol)

        trans_volume_uL = self.calculateVolume2(sample.volume)  #!!!! stage dependent  #from transport
        
        initialQuadrant = sample.initialQuadrant

        call = "TopUpTrans(destVial=%s, srcVial=%s, workVolume=%0.2f, id=%d, seq=%d, tiprackSpecified=%s, tiprack=%d, initialQuadrant=%d, \
        trans_volume_uL=%0.2f, usingFreeAirDispense=%s, usingBufferTip=%s, destVial2=%s)" % \
                 (destVial, srcVial, workVolume, sample.ID, self.seq, self.tiprackSpecified, self.tiprack, initialQuadrant,
                  trans_volume_uL, self.freeAirDispense, self.useBufferTip,destVial2)


        return call

    def parseXML(self, xmlNode):
        '''Parse out the volume data and then the freeAirDispense flag.'''
        VolumeCommand.parseXML(self, xmlNode)
        
        try:
            useBufferFlag = xmlNode.volume[0].useBufferTip.lower() == 'true'
            self.setUseBufferTip(useBufferFlag)
        except AttributeError:
            # No usebuffertip flag defined? Keep going
            pass
        try:
            freeAirFlag = xmlNode.volume[0].freeair.lower() == 'true'
            self.setFreeAirDispense(freeAirFlag)
        except AttributeError:
            # No free air dispense flag defined? Keep going
            pass
    def setFreeAirDispense(self, usingFreeAirDispense = False):
        '''Set the freeAirDispense member (to True or False).'''
        self.freeAirDispense = usingFreeAirDispense

    def setUseBufferTip(self, usingBufferTip = False):
        '''Set the useBufferTip member (to True or False).'''
        self.useBufferTip = usingBufferTip

class TopUpTransSepTransCommand(VolumeCommand):
    def __init__(self, sequenceNumber, type, label):
        VolumeCommand.__init__(self, sequenceNumber, type, label)
        self.freeAirDispense = False
        self.useBufferTip = False
        self.duration = 0

    
    def createCall(self, sample, protocol):
        #bufferVial =  self.getContainerString(sample,self._parseVialInfo(TPC0001)) #assume fix buffer  #dont bother just use default
        srcVial = self.getContainerString(sample, self.srcVial)
        destVial = self.getContainerString(sample, self.destVial)
        destVial2 = self.getContainerString(sample, self.destVial2)
        destVial3 = self.getContainerString(sample, self.destVial3)
        if self.relative:
            workVolume = self.calculateVolume(sample.volume) + sample.volume
            workVolume = min(10000,workVolume)
        else:
            workVolume = self.getWorkingVolume(sample, protocol)

        trans_volume_uL = self.calculateVolume2(sample.volume)  #!!!! stage dependent  #from transport
        trans_volume_uL2 = self.calculateVolume3(sample.volume) 
        initialQuadrant = sample.initialQuadrant

        call = "TopUpTransSepTrans(destVial=%s, srcVial=%s, workVolume=%0.2f, id=%d, seq=%d, tiprackSpecified=%s, tiprack=%d, initialQuadrant=%d, \
        trans_volume_uL=%0.2f, usingFreeAirDispense=%s, usingBufferTip=%s, trans_volume_uL2=%0.2f, destVial2=%s, destVial3=%s, duration=%d )" % \
                 (destVial, srcVial, workVolume, sample.ID, self.seq, self.tiprackSpecified, self.tiprack, initialQuadrant,
                  trans_volume_uL, self.freeAirDispense, self.useBufferTip, trans_volume_uL2, destVial2, destVial3, self.duration)


        return call

    def parseXML(self, xmlNode):
        '''Parse out the volume data and then the freeAirDispense flag.'''
        VolumeCommand.parseXML(self, xmlNode)
        
        if xmlNode.hasAttribute('duration'):
            self.duration = int(xmlNode.getAttribute('duration'))
        
        try:
            useBufferFlag = xmlNode.volume[0].useBufferTip.lower() == 'true'
            self.setUseBufferTip(useBufferFlag)
        except AttributeError:
            # No usebuffertip flag defined? Keep going
            pass
        try:
            freeAirFlag = xmlNode.volume[0].freeair.lower() == 'true'
            self.setFreeAirDispense(freeAirFlag)
        except AttributeError:
            # No free air dispense flag defined? Keep going
            pass
    def setFreeAirDispense(self, usingFreeAirDispense = False):
        '''Set the freeAirDispense member (to True or False).'''
        self.freeAirDispense = usingFreeAirDispense

    def setUseBufferTip(self, usingBufferTip = False):
        '''Set the useBufferTip member (to True or False).'''
        self.useBufferTip = usingBufferTip

class TopUpMixTransSepTransCommand(VolumeCommand):
    def __init__(self, sequenceNumber, type, label):
        VolumeCommand.__init__(self, sequenceNumber, type, label)
        self.freeAirDispense = False
        self.useBufferTip = False
        self.duration = 0
        self.mixCycles = -1
        self.tipTubeBottomGap = -1

    
    def createCall(self, sample, protocol):
        #bufferVial =  self.getContainerString(sample,self._parseVialInfo(TPC0001)) #assume fix buffer  #dont bother just use default
        srcVial = self.getContainerString(sample, self.srcVial)
        destVial = self.getContainerString(sample, self.destVial)
        destVial2 = self.getContainerString(sample, self.destVial2)
        destVial3 = self.getContainerString(sample, self.destVial3)
        if self.relative:
            workVolume = self.calculateVolume(sample.volume) + sample.volume
            workVolume = min(10000,workVolume)
        else:
            workVolume = self.getWorkingVolume(sample, protocol)

        if self.relative2:
            mix_volume_uL = self.proportion2
        else:
            mix_volume_uL = self.absVolume2
        
        trans_volume_uL = self.calculateVolume3(sample.volume)  #!!!! stage dependent  #from transport
        trans_volume_uL2 = self.calculateVolume4(sample.volume) 
        initialQuadrant = sample.initialQuadrant

        call = "TopUpMixTransSepTrans(destVial=%s, srcVial=%s, workVolume=%0.2f, id=%d, seq=%d, tiprackSpecified=%s, tiprack=%d, initialQuadrant=%d, \
        trans_volume_uL=%0.2f, usingFreeAirDispense=%s, usingBufferTip=%s, trans_volume_uL2=%0.2f, destVial2=%s, destVial3=%s, duration=%d, \
        mixCycles=%d, tipTubeBottomGap=%d, isRelative=%d, mix_volume_uL=%0.2f)" % \
                 (destVial, srcVial, workVolume, sample.ID, self.seq, self.tiprackSpecified, self.tiprack, initialQuadrant,
                  trans_volume_uL, self.freeAirDispense, self.useBufferTip, trans_volume_uL2, destVial2, destVial3, self.duration,
                  self.mixCycles, self.tipTubeBottomGap, self.relative2, mix_volume_uL)


        return call

    def parseXML(self, xmlNode):
        '''Parse out the volume data and then the freeAirDispense flag.'''
        VolumeCommand.parseXML(self, xmlNode)
        if xmlNode.hasAttribute('mixCycles'):
            self.mixCycles = int(xmlNode.getAttribute('mixCycles'))
        if xmlNode.hasAttribute('tipTubeBottomGap'):
            self.tipTubeBottomGap = int(xmlNode.getAttribute('tipTubeBottomGap'))
        if xmlNode.hasAttribute('duration'):
            self.duration = int(xmlNode.getAttribute('duration'))
        
        try:
            useBufferFlag = xmlNode.volume[0].useBufferTip.lower() == 'true'
            self.setUseBufferTip(useBufferFlag)
        except AttributeError:
            # No usebuffertip flag defined? Keep going
            pass
        try:
            freeAirFlag = xmlNode.volume[0].freeair.lower() == 'true'
            self.setFreeAirDispense(freeAirFlag)
        except AttributeError:
            # No free air dispense flag defined? Keep going
            pass
    def setFreeAirDispense(self, usingFreeAirDispense = False):
        '''Set the freeAirDispense member (to True or False).'''
        self.freeAirDispense = usingFreeAirDispense

    def setUseBufferTip(self, usingBufferTip = False):
        '''Set the useBufferTip member (to True or False).'''
        self.useBufferTip = usingBufferTip

# -----------------------------------------------------------------------------

class VolumeMaintenanceCommand(VolumeCommand):
    '''A VolumeCommand child class for maintenance-type workflows.'''
    
    def __init__(self, sequenceNumber, type, label):
        VolumeCommand.__init__(self, sequenceNumber, type, label)
        self.home = False

    def parseXML(self, xmlNode):
        '''Parse the XML data to see if we should set the flag here for "home"
        or "no home".'''
        VolumeCommand.parseXML(self, xmlNode)
        for child in xmlNode.childNodes:
            if child.nodeName == 'flags':
                self.home = child.getAttribute('home') == 'true'


class FlushCommand(VolumeMaintenanceCommand):
    '''Fluidics flush command'''

    def createCall(self, sample, protocol):
        wasteVial = self.getContainerString(sample, self.destVial)
        
    # Nov 7, 2013 Sunny
    # Add seq for maintenance protocol
        return "Flush(wasteVial=%s, homeAtEnd = %s, id=%d, seq=%d)" % \
                (wasteVial, self.home, sample.ID, self.seq)
   
   
class PrimeCommand(VolumeMaintenanceCommand):
    '''Fluidics prime command'''

    def createCall(self, sample, protocol):
        wasteVial = self.getContainerString(sample, self.destVial)
        
    # Nov 7, 2013 Sunny
    # Add seq for maintenance protocol
        return "Prime(wasteVial=%s, homeAtEnd = %s, id=%d, seq=%d)" % \
                (wasteVial, self.home, sample.ID, self.seq)

   
# eof

