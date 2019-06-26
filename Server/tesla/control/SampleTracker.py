# 
# SampleTracker.py
# tesla.control.SampleTracker
#
# Sample tracking in the Tesla instrument controller
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
from datetime import date

import tesla.config
from tesla.exception import TeslaException
from tesla.instrument.Instrument import Instrument
from tesla.serialnum import InstrumentSerialNumber

import win32api
import string

import cgi

BIN_DIR64        = 'C:\\program files (x86)\\sti\\robosep\\bin\\wkhtmltopdf.exe -s Letter '    #CWJ ADD
BIN_DIR32        = 'C:\\program files\\sti\\robosep\\bin\\wkhtmltopdf.exe -s Letter '          #CWJ ADD
DEF_NULL_CODE    = "['']"
   
def StripString(s):
    s = string.lstrip(s,"['");
    s = string.rstrip(s,"']");
    return s;   
    
# -----------------------------------------------------------------------------

class SampleTrackingException(TeslaException):
    '''Exception from sample tracking'''
    pass

# -----------------------------------------------------------------------------

class SampleTracker:
    '''Sample tracking in Telsa instrument controller.
    Note that we can only have one set of samples in the system at any time.
    '''
    
    def __init__(self, reportBaseDir = tesla.config.SAMPLE_REPORT_DIR):
        '''Initialise the sample tracker and leave it empty
        Parameters: reportBaseDir - the base directory for report generation'''
        self.reportXmlName = ""
        self.reportBaseDir = reportBaseDir
        self.resetTracker()
        if 'ProgramFiles(x86)' in os.environ:    
           self.BIN_DIR = BIN_DIR64
        else:
           self.BIN_DIR = BIN_DIR32

    def resetTracker(self):
        '''Clear all sample history out of the tracker'''
        self.samples = None
        self.scheduleStarted = False

    def addSamples(self, sampleList):
        '''Add samples to the tracker'''
        if self.scheduleStarted:
            raise SampleTrackingException("SampleTracker: Schedule already started, cannot add samples")

        self.samples = sampleList[:]

    def hasStarted(self):
        '''Returns True is a schedule is marked as started'''
        return self.scheduleStarted

    def scheduleStart(self, etc, startTime = None, operator = None):
        '''Marks a schedule as started
        startTime defaults to current system time
        operator defaults to "Unknown"'''
        if self.scheduleStarted:
            raise SampleTrackingException("SampleTracker: Schedule already started, cannot start schedule")
        if self.samples == None:
            raise SampleTrackingException("SampleTracker: No samples, cannot start schedule")

        if not startTime:
            startTime = datetime.fromtimestamp(time.time())
        if not operator:
            operator = 'Unknown'

        self.startTime = startTime
        self.etc = etc
        self.operator = operator
        self.scheduleStarted = True
    
    def scheduleCompleted(self, endTime = None):
        '''Marks a schedule as successfully complete and logs a report for all samples
        endTime defaults to current system time'''
        if not self.scheduleStarted:
            raise SampleTrackingException("SampleTracker: Schedule not started, cannot complete schedule")
        if not endTime:
            endTime = datetime.fromtimestamp(time.time())

        self.endTime = endTime
        self.reportSchedule('COMPLETED')

    def scheduleAborted(self, reason, abortTime = None):
        '''Marks a schedule as unsuccessful and logs a report for all samples
        reason should contain a description of why the schedule did not complete
        abortTime defaults to current system time'''
        if not self.scheduleStarted:
            raise SampleTrackingException("SampleTracker: Schedule not started, cannot abort schedule")
        if not abortTime:
            abortTime = datetime.fromtimestamp(time.time())

        self.endTime = abortTime
        self.reportSchedule('ABORTED because '+ reason)

    def GetReportXMLFullFileName(self):
        print("\nGetReportXMLFullFileName: %s"%(self.reportXmlName))
        return self.reportXmlName;
    
    def convertSymbols(self, inStr):
        cvtStr = cgi.escape(inStr);
        return cvtStr;

    def reportSchedule(self,finalStatus = 'Unknown'):
        '''Adds a report for all of the samples in the schedule'''
        # reports are added to the report file corresponding with the completion time.
        # to add the report based on start time use "self.startTime"
        
        #####################
        def formatReagentVolume(title, label, volume):
            '''Formats the reagent title, label and volume for use in reports'''
            if label == '':
                label = 'Unlabeled'
            
            # Format the reagent title and label
            result = "            %-26s" % (title + ":")
            result += "%-20s" % label

            # Append the volume used
            if volume > 0.0:                
                result += ' (' + str(float(volume)/1000.0) + ' ml)\n'
            else:
                result += ' (Not Used)\n'

            return result
        #####################

        #####################
        def formatVessel(title, required):
            '''Formats the flag for a vessel for use in reports'''
            # Format the reagent title and label
            result = "            %-26s" % (title + ":")

            # Append the volume used
            if required:
                result += 'Needed\n'
            else:
                result += 'Not Needed\n'

            return result
        #####################

        reportName = self.getFileName(self.endTime)
        fName,eName   = os.path.splitext(reportName)
        self.reportXmlName = fName + ".xml"
         
        reportFile = open(self.reportXmlName,"a")                
        reportFile.write("<?xml version=\"1.0\" standalone=\"yes\"?>\n")
        reportFile.write("<RunDataSet1 xmlns=\"http://tempuri.org/RunDataSet1.xsd\">")
        self.GenRunDetails(reportFile, finalStatus)
        self.GenRunSummary(reportFile)
        
        #loop
        sampleCount=0
        for sample in self.samples:
            sampleCount += 1
            self.GenRunProtocol(reportFile,sample,sampleCount)
            self.GenCommandSequences(reportFile,sample,sampleCount)
                   
        reportFile.write("\n</RunDataSet1>\n")
        reportFile.write("\n\n")
        reportFile.close()
        self.resetTracker()
        time.sleep(2)
              
              
    # --- Private methods -----------------------------------------------------

    def getFileName(self,basedOnTime=None):
        '''Return the filename for the given day'''
        if not basedOnTime:
            basedOnTime = datetime.fromtimestamp(time.time())

        # We want the directory path to be BASEDIR/yyyy/Mmm
        # ie: BASEDIR/2004/Dec
        theDate = basedOnTime #.date()
        theYear = theDate.strftime("%Y")
        theMonth = theDate.strftime("%b")
        
        # Create the report file name "Report_25Dec2004.txt"
        fileName = theDate.strftime("Report_%d%b%Y_%H_%M_%S.html")

        # full path name of report
        pathName = os.path.join(self.reportBaseDir, theYear, theMonth)

        # If the path doesn't exist, create it
        if not os.path.exists(pathName):
            try:
                os.makedirs(pathName)
            except OSError as msg:
                raise SampleTrackingException("Unable to make sample report dir (%s)" % (msg))

        # Create the full path name
        fullFileName = os.path.join(pathName, fileName)
        print(fullFileName)
        return fullFileName


        
    def getVialText(self, Vial, InitQ, customNames):

        print("BDR ------ in getVialText with srcvial=%s and initQuad=%d" % (str(Vial[1]), InitQ))  
        indx = Vial[0]-1        
        print("BDR---- using Vial[0]=%d index=%d" % (Vial[0], indx))
       
        vialName= ''
        if str(Vial[1]) == Instrument.BCCMLabel:
            if indx >= 0:
                vialName=customNames[indx*8+0]
            if vialName=='':
                vialName= 'Buffer Bottle'
        elif str(Vial[1]) == Instrument.WasteLabel:
            if indx >= 0:
                vialName=customNames[indx*8+1]
            if vialName=='':
                vialName= 'Waste Tube'
        elif str(Vial[1]) == Instrument.LysisLabel:
            if indx >= 0:
                vialName=customNames[indx*8+2]
            if vialName=='':
                vialName= 'Lysis Buffer Tube'
        elif str(Vial[1]) == Instrument.CocktailLabel:
            if indx >= 0:
                vialName=customNames[indx*8+3]
            if vialName=='':
                vialName= 'Selection Vial (Square)'
        elif str(Vial[1]) == Instrument.BeadLabel:
            if indx >= 0:
               vialName=customNames[indx*8+4]
            if vialName=='':
                vialName= 'Magnetic Particle Vial (Triangle)'
        elif str(Vial[1]) == Instrument.AntibodyLabel:
            if indx >= 0:
                vialName=customNames[indx*8+5]
            if vialName=='':
                vialName= 'Antibody Vial (Circle)'
        elif str(Vial[1]) == Instrument.SampleLabel:
            if indx >= 0:
                vialName=customNames[indx*8+6]
            if vialName=='':
                vialName= 'Sample Tube'
        elif str(Vial[1]) == Instrument.SupernatentLabel:
            if indx >= 0:
                vialName=customNames[indx*8+7]
            if vialName=='':
                vialName= 'Separation Tube'
        else:
            vialName = 'N/A'

        if indx < 1:
            indx = 0
            
        print("BDR ---- str(indx+InitQ) = %s" % (str(indx+InitQ))) 
            
        return "Q"+str(indx+InitQ)+", "+vialName #.encode('utf-8')


    def GenRunDetails(self,reportFile,finalStatus):
        instrumentID=InstrumentSerialNumber().getSerial()
        reportFile.write("\n\t<RunDetails>")
        reportFile.write("\n\t\t<operatorID>" + str(self.operator) + "</operatorID>")
        reportFile.write("\n\t\t<startDate>" +self.startTime.strftime("%Y-%m-%dT%H:%M:%S") + "</startDate>")
        reportFile.write("\n\t\t<endDate>" +self.endTime.strftime("%Y-%m-%dT%H:%M:%S") + "</endDate>")
        reportFile.write("\n\t\t<finalStatus>" +str(finalStatus) + "</finalStatus>")
        reportFile.write("\n\t\t<instrumentID>" +str(instrumentID) + "</instrumentID>")
        reportFile.write("\n\t</RunDetails>")

    def GenVialLabel(self,idx,vialVolume,vialLabelList):
        NOT_PROVIDED ="Not Provided"
        NOT_APPLICABLE ="-"    # Not Applicable
        if vialVolume<=0:
            return NOT_APPLICABLE
        elif not vialLabelList or len(vialLabelList)<=idx or vialLabelList[idx]=='':
            return NOT_PROVIDED
        else:
            return vialLabelList[idx]

    def GenSampleLabel(self,idx,vialLabelList):
        NOT_PROVIDED ="Not Provided"
        NOT_APPLICABLE ="-"    # Not Applicable
        if not vialLabelList or len(vialLabelList)<=idx or vialLabelList[idx]=='':
            return NOT_PROVIDED
        else:
            return vialLabelList[idx]


    def GenRunSummary(self,reportFile):
        NOT_PROVIDED ="Not Provided"
        NOT_APPLICABLE ="-"    # Not Applicable
        REQUIRED = "Required"
        FALCON_14ML_TUBE="14 mL Falcon Recommended - See PIS"
        FALCON_50ML_TUBE="50 mL Falcon Recommended - See PIS"
        sampleNo=""
        protocol=""
        quadrantNo=0
        sampleCount=0
        skipAdd=False;
        for i in range(4):
            if i < quadrantNo:
                continue;
            quadrantNo+=1
            
            # reset parameters
            sampleNo=""
            sampleID=""
            protocol=""
            sampleVolume_ML=""
            initQuad=0
            quadNumRequired=0
            magneticParticlesLotID=""
            selectionCocktailLotID=""
            antibodyCocktailLotID=""
            bufferLotID=""
            tipRack=""
            sampleTube=""
            separationTube=""
            negFractionTube=""
            wasteTube=""
            skipAdd=False
            
            for j in range(4):
                if j >= len(self.samples):
                    sampleNo=""
                else:
                    sample = self.samples[j]
                    if sample.initialQuadrant==quadrantNo:
                        sampleCount+=1
                        sampleNo=str(sampleCount)

                        protocol = self.convertSymbols(sample.protocol.label);
                        sampleVolume_ML="%2.3f" %(float(sample.volume)/1000.0)
                        initQuad=str(sample.initialQuadrant)
                        quadNumRequired=str(len(sample.consumables))

                        tmpQuadrants = len(sample.consumables)
                        for k in range(tmpQuadrants):
                            magneticParticlesLotID=""
                            selectionCocktailLotID=""
                            antibodyCocktailLotID=""
                            bufferLotID=""

                            magneticParticlesLotID = self.GenVialLabel(k,sample.consumables[k].particleVolume,sample.particleLabel)
                            selectionCocktailLotID = self.GenVialLabel(k,sample.consumables[k].cocktailVolume,sample.cocktailLabel)
                            antibodyCocktailLotID = self.GenVialLabel(k,sample.consumables[k].antibodyVolume,sample.antibodyLabel)

                            if sample.consumables[k].sampleVesselRequired > 0:
                                sampleID = self.GenSampleLabel(k,sample.sample1Label)
                            else:
                                sampleID = NOT_APPLICABLE
                                
                                    
                            if (k == 0):
                                if str(sample.bufferLabel) == '':
                                    if sample.consumables[k].bulkBufferVolume>0:
                                        bufferLotID=NOT_PROVIDED
                                    else:
                                        bufferLotID=NOT_APPLICABLE
                                else:
                                    bufferLotID=StripString(str(sample.bufferLabel))
                            else:
                                if sample.consumables[k].bulkBufferVolume>0:
                                    bufferLotID=NOT_PROVIDED
                                else:
                                    bufferLotID=NOT_APPLICABLE                               
                                
                            if sample.consumables[k].tipBoxRequired > 0:
                                tipRack=REQUIRED
                            else:
                                tipRack=NOT_APPLICABLE

                            if sample.consumables[k].sampleVesselRequired > 0:
                                sampleTube=REQUIRED
                            else:
                                sampleTube=NOT_APPLICABLE

                            if sample.consumables[k].separationVesselRequired > 0:
                                separationTube=REQUIRED
                            else:
                                separationTube=NOT_APPLICABLE
                                
                            if sample.consumables[k].lysisVolume>0:
                                negFractionTube=REQUIRED
                            else:
                                negFractionTube=NOT_APPLICABLE
                             
                            if sample.consumables[k].wasteVesselRequired > 0:
                                wasteTube=REQUIRED
                            else:
                                wasteTube=NOT_APPLICABLE
                                
                            if (k > 0):
                                quadrantNo+=1
                            
                            reportFile.write("\n\t<RunSummary>")
                            reportFile.write("\n\t\t<sampleNo>" +str(sampleNo) + "</sampleNo>")
                            reportFile.write("\n\t\t<sampleID>" +str(sampleID) + "</sampleID>")
                            reportFile.write("\n\t\t<protocol>" +str(protocol) + "</protocol>")
                            reportFile.write("\n\t\t<sampleVolume_ML>" +str(sampleVolume_ML) + "</sampleVolume_ML>")
                            reportFile.write("\n\t\t<magneticParticlesLotID>" +str(magneticParticlesLotID) + "</magneticParticlesLotID>")
                            reportFile.write("\n\t\t<selectionCocktailLotID>" +str(selectionCocktailLotID) + "</selectionCocktailLotID>")
                            reportFile.write("\n\t\t<antibodyCocktailLotID>" +str(antibodyCocktailLotID) + "</antibodyCocktailLotID>")
                            reportFile.write("\n\t\t<tipRack>" +str(tipRack) + "</tipRack>")
                            reportFile.write("\n\t\t<bufferLotID>" +str(bufferLotID) + "</bufferLotID>")
                            reportFile.write("\n\t\t<sampleTube>" +str(sampleTube) + "</sampleTube>")
                            reportFile.write("\n\t\t<separationTube>" +str(separationTube) + "</separationTube>")
                            reportFile.write("\n\t\t<negativeFractionTube>" +str(negFractionTube) + "</negativeFractionTube>")
                            reportFile.write("\n\t\t<wasteTube>" +str(wasteTube) + "</wasteTube>")
                            reportFile.write("\n\t\t<quadrantNo>" +str(quadrantNo) + "</quadrantNo>")
                            reportFile.write("\n\t\t<quadrantInitial>" +str(initQuad) + "</quadrantInitial>")
                            reportFile.write("\n\t\t<quadrantRequired>" +str(quadNumRequired) + "</quadrantRequired>")
                            reportFile.write("\n\t</RunSummary>")

                           # reset parameters
                            sampleVolume_ML=""
                            quadNumRequired=0
                            initQuad=0
                            magneticParticlesLotID=""
                            selectionCocktailLotID=""
                            antibodyCocktailLotID=""
                            bufferLotID=""
                            tipRack=""
                            sampleTube=""
                            separationTube=""
                            negFractionTube=""
                            wasteTube=""
                            skipAdd=True

            if (skipAdd==False):
                reportFile.write("\n\t<RunSummary>")
                reportFile.write("\n\t\t<sampleNo>" +str(sampleNo) + "</sampleNo>")
                reportFile.write("\n\t\t<sampleID>" +str(sampleID) + "</sampleID>")
                reportFile.write("\n\t\t<protocol>" +str(protocol) + "</protocol>")
                reportFile.write("\n\t\t<sampleVolume_ML>" +str(sampleVolume_ML) + "</sampleVolume_ML>")
                reportFile.write("\n\t\t<magneticParticlesLotID>" +str(magneticParticlesLotID) + "</magneticParticlesLotID>")
                reportFile.write("\n\t\t<selectionCocktailLotID>" +str(selectionCocktailLotID) + "</selectionCocktailLotID>")
                reportFile.write("\n\t\t<antibodyCocktailLotID>" +str(antibodyCocktailLotID) + "</antibodyCocktailLotID>")
                reportFile.write("\n\t\t<tipRack>" +str(tipRack) + "</tipRack>")
                reportFile.write("\n\t\t<bufferLotID>" +str(bufferLotID) + "</bufferLotID>")
                reportFile.write("\n\t\t<sampleTube>" +str(sampleTube) + "</sampleTube>")
                reportFile.write("\n\t\t<separationTube>" +str(separationTube) + "</separationTube>")
                reportFile.write("\n\t\t<negativeFractionTube>" +str(negFractionTube) + "</negativeFractionTube>")
                reportFile.write("\n\t\t<wasteTube>" +str(wasteTube) + "</wasteTube>")
                reportFile.write("\n\t\t<quadrantNo>" +str(quadrantNo) + "</quadrantNo>")
                reportFile.write("\n\t\t<quadrantInitial>" +str(initQuad) + "</quadrantInitial>")
                reportFile.write("\n\t\t<quadrantRequired>" +str(quadNumRequired) + "</quadrantRequired>")
                reportFile.write("\n\t</RunSummary>")

    def GenRunProtocol(self,reportFile,sample,sampleCount):
        reportFile.write("\n\t<RunSample>")
        reportFile.write("\n\t\t<sampleNo>" +str(sampleCount) + "</sampleNo>")
        reportFile.write("\n\t\t<quadrantNo>" +str(sample.initialQuadrant) + "</quadrantNo>")
        reportFile.write("\n\t\t<protocolName>" + self.convertSymbols(sample.protocol.label) + "</protocolName>")
        reportFile.write("\n\t\t<type>" + sample.protocol.type + "</type>")
        reportFile.write("\n\t\t<author>" + self.convertSymbols(sample.protocol.author) + "</author>")
        reportFile.write("\n\t\t<fileName>" + self.convertSymbols(sample.protocol.filename) + "</fileName>")
        YMD = str(sample.protocol.created).split('-')
        tmpDate = date(int(YMD[0]),int(YMD[1]),int(YMD[2]))        
        reportFile.write("\n\t\t<dateCreated>" +tmpDate.strftime("%Y-%m-%dT%H:%M:%S") + "</dateCreated>")
        YMD = str(sample.protocol.modified).split('-')
        tmpDate = date(int(YMD[0]),int(YMD[1]),int(YMD[2]))        
        reportFile.write("\n\t\t<dateModified>" +tmpDate.strftime("%Y-%m-%dT%H:%M:%S") + "</dateModified>")
        reportFile.write("\n\t\t<protocolNo>" +sample.protocol.protocolNumber + "</protocolNo>")
        reportFile.write("\n\t\t<version>" +str(sample.protocol.version) + "</version>")
        reportFile.write("\n\t</RunSample>")

    def GenCommandHeader(self,reportFile,seqNum, cmdType, cmdLabel):
        reportFile.write("\n\t<CommandSequence>")
        reportFile.write("\n\t\t<step>" +str(seqNum) + "</step>")
        reportFile.write("\n\t\t<command>" +cmdType + "</command>")
        reportFile.write("\n\t\t<description>" +self.convertSymbols(cmdLabel) + "</description>")
    def GenCommandFooter(self,reportFile,quadrantNo):
        reportFile.write("\n\t\t<quadrantNo>" +str(quadrantNo) + "</quadrantNo>")
        reportFile.write("\n\t</CommandSequence>")
    def GenCommandContentEmpty(self,reportFile,seqNum, cmdType, cmdLabel,quadrantNo):
        self.GenCommandHeader(reportFile,seqNum,cmdType, cmdLabel)
        reportFile.write("\n\t\t<source>" +"" + "</source>")
        reportFile.write("\n\t\t<destination>" +"" + "</destination>")
        reportFile.write("\n\t\t<volume_ML>" +"" + "</volume_ML>")
        reportFile.write("\n\t\t<tipRack>" +"" + "</tipRack>")
        self.GenCommandFooter(reportFile,quadrantNo)
    def GenCommandContentNotImplemented(self,reportFile,seqNum, cmdType, cmdLabel,quadrantNo):
        self.GenCommandHeader(reportFile,seqNum,cmdType, cmdLabel)
        reportFile.write("\n\t\t<source>" +"NOT IMPLEMENTED" +"</source>")
        reportFile.write("\n\t\t<destination>" +"" + "</destination>")
        reportFile.write("\n\t\t<volume_ML>" +"" + "</volume_ML>")
        reportFile.write("\n\t\t<tipRack>" +"" + "</tipRack>")
        self.GenCommandFooter(reportFile,quadrantNo)
    def GenCommandContent(self,reportFile,seqNum, cmdType, cmdLabel,quadrantNo,srcText,destText,volumeText,tiprackText):
        self.GenCommandHeader(reportFile,seqNum,cmdType, cmdLabel)
        if srcText != None:
            reportFile.write("\n\t\t<source>" + self.convertSymbols(srcText) + "</source>")
        if destText != None:
            reportFile.write("\n\t\t<destination>" + self.convertSymbols(destText) + "</destination>")
        if volumeText != None:
            reportFile.write("\n\t\t<volume_ML>" + volumeText + "</volume_ML>")
        if tiprackText != None:
            reportFile.write("\n\t\t<tipRack>" + tiprackText + "</tipRack>")
        self.GenCommandFooter(reportFile,quadrantNo)

    def GenCommandContentTopUpOrResuspend(self,reportFile,seqNum, cmdType, cmdLabel,quadrantNo,isRelative,proportion, \
                                          isTopUpType,protocol,sampVol,tiprackText):
        
        # reportFile.write("<td bgcolor=gray><font size=\"-2\">Buffer Bottle</font></td>")
        # reportFile.write("<td align=left valign=top><font size=\"-2\">"+self.getVialText(cmd.destVial,sample.initialQuadrant,customNames)+"</font></td>")
        if isRelative:
            #reportFile.write("<td align=left valign=top><font size=\"-2\">Relative</font></td>")
            if isTopUpType:
                volumeText = "&gt;"+"Fill up to "+"%2.3f" % (proportion*sampVol)
            else:
                volumeText = "&gt;"+"%2.3f" % (proportion*sampVol)
        else:
            #reportFile.write("<td align=left valign=top><font size=\"-2\">Threshold</font></td>") 
            if sampVol < protocol.sampleThreshold/1000:
                absVol =(protocol.lowWorkingVolume/1000)
            else:
                absVol =(protocol.highWorkingVolume/1000)
            if absVol >= 10:
                volumeText = "&lt;= 10mL"
            else:
                volumeText = "&lt;= %2.3f" % absVol
                
                    
        self.GenCommandContent(reportFile,seqNum, cmdType, cmdLabel,quadrantNo, None,None,volumeText,tiprackText)

    def GenCommandContentTransportOrMix(self,reportFile,seqNum, cmdType, cmdLabel,quadrantNo,useBufferTip,absVol,isRelative,proportion,mixCycles,isMixType,sampVol, \
                                        srcText,destText,tiprackText):
        bfrTipStr = ""
                
        if useBufferTip:
            bfrTipStr = "\rBuffer Tip"                    
        if isRelative:
            #reportFile.write("\n\t\t<volume_ML>" +"Relative" + "</volume_ML>")
            if isMixType:
                volumeText = str(mixCycles)+"*"+"%2d" % (proportion*100)+"%"
            else:
                volumeText = "&gt;"+"%2.3f" % (proportion*sampVol) + bfrTipStr                         
        else:
            #reportFile.write("<td align=left valign=top><font size=\"-2\">Absolute</font></td>")
            if absVol >= 10:
                volumeText = "&lt;= 10mL" + bfrTipStr            
            else:
                volumeText = "&gt;"+"%2.3f" % (absVol) + bfrTipStr

        self.GenCommandContent(reportFile,seqNum, cmdType, cmdLabel,quadrantNo, srcText,destText,volumeText,tiprackText)
        


    def GenCommandSequences(self,reportFile,sample,sampleCount):
        names = sample.protocol.getCustomNames()
        seqNum=0
        sampVol = sample.volume/1000
        customNames = sample.protocol.getCustomNames()
        quadrantNo = sample.initialQuadrant
        
        for cmd in sample.protocol.getCommands():
            seqNum+=1


            srcText = None
            destText = None
            volumeText = None
            tiprackText = None
 
            if cmd.isTransportType() or cmd.isMixType():
                srcText = self.getVialText(cmd.srcVial,sample.initialQuadrant,customNames)
                if cmd.isTransportType():
                    destText = self.getVialText(cmd.destVial,sample.initialQuadrant,customNames)
                else:
                    destText = ""
                      
                if cmd.tiprackSpecified:
                    tiprackText = str(cmd.tiprack)
                else:
                    tiprackText = str(cmd.srcVial[0]-1+sample.initialQuadrant)

                absVol = cmd.absVolume/1000
                useBufferTip = cmd.useBufferTip
                isRelative = cmd.relative
                proportion = cmd.proportion
                isMixType = cmd.isMixType()
                mixCycles = None
                if isMixType:
                    mixCycles = cmd.mixCycles

                self.GenCommandContentTransportOrMix(reportFile,seqNum,cmd.type.replace('Command',''),cmd.label,quadrantNo, \
                                                     useBufferTip,absVol,isRelative,proportion,mixCycles,isMixType,sampVol, \
                                        srcText,destText,tiprackText)     
        
            elif cmd.isTopUpType() or cmd.isResuspendType():
                isRelative = cmd.relative
                proportion = cmd.proportion
                isTopUpType = cmd.isTopUpType()
                protocol = sample.protocol
                if cmd.tiprackSpecified:
                    tiprackText = str(cmd.tiprack)
                else:
                    tiprackText = str(cmd.destVial[0]-1+sample.initialQuadrant)

                self.GenCommandContentTopUpOrResuspend(reportFile,seqNum,cmd.type.replace('Command',''),cmd.label,quadrantNo, \
                                                       isRelative,proportion,isTopUpType,protocol,sampVol,tiprackText)

            elif cmd.isMixTransType():
                srcText = self.getVialText(cmd.srcVial,sample.initialQuadrant,customNames) 
                destText = self.getVialText(cmd.destVial,sample.initialQuadrant,customNames)
                absVol = cmd.absVolume/1000
                isRelative = cmd.relative
                proportion = cmd.proportion
                isMixType = True
                mixCycles = cmd.mixCycles  #must have mix cycle to be here
                if cmd.tiprackSpecified:
                    tiprackText = str(cmd.tiprack)
                else:
                    tiprackText = str(cmd.destVial[0]-1+sample.initialQuadrant)
                
                useBufferTip = cmd.useBufferTip

                seqLabel = "%da" % (seqNum)
                self.GenCommandContentTransportOrMix(reportFile,seqLabel,cmd.type.replace('Command',''),cmd.label,quadrantNo, \
                                                     useBufferTip,absVol,isRelative,proportion,mixCycles,isMixType,sampVol, \
                                                    srcText,None,tiprackText)


                isMixType = False
                absVol = cmd.absVolume2/1000
                isRelative = cmd.relative2
                proportion = cmd.proportion2
                    
                seqLabel = "%db" % (seqNum)
                self.GenCommandContentTransportOrMix(reportFile,seqLabel,'Transport','',quadrantNo, \
                                                     useBufferTip,absVol,isRelative,proportion,0,isMixType,sampVol, \
                                                    srcText,destText,tiprackText)

                

            elif cmd.isTopUpMixTransType() or cmd.isResusMixSepTransType() or cmd.isResusMixType() or \
                 cmd.isTopUpTransType() or cmd.isTopUpTransSepTransType() or cmd.isTopUpMixTransSepTransType():
                
                isRelative = cmd.relative
                proportion = cmd.proportion
                isTopUpType = not (cmd.isResusMixSepTransType() or cmd.isResusMixType())
                protocol = sample.protocol
                if cmd.tiprackSpecified:
                    tiprackText = str(cmd.tiprack)
                else:
                    tiprackText = str(cmd.destVial[0]-1+sample.initialQuadrant)
                
                useBufferTip = cmd.useBufferTip

                seqLabel = "%da" % (seqNum)
                #deal with 1st part of topup or resuspend
                self.GenCommandContentTopUpOrResuspend(reportFile,seqLabel,cmd.type.replace('Command',''),cmd.label,quadrantNo, \
                                                       isRelative,proportion,isTopUpType,protocol,sampVol,tiprackText)

                
                #deal with 2nd part of mix if applicable
                if cmd.isTopUpMixTransType() or cmd.isResusMixSepTransType() or cmd.isResusMixType() or cmd.isTopUpMixTransSepTransType():
                    srcText = self.getVialText(cmd.destVial,sample.initialQuadrant,customNames) #using dest for mix src on purpose
                    absVol = cmd.absVolume2/1000
                    isRelative = cmd.relative2
                    proportion = cmd.proportion2
                    isMixType = True
                    mixCycles = cmd.mixCycles  #must have mix cycle to be here

                    seqLabel = "%db" % (seqNum)
                    self.GenCommandContentTransportOrMix(reportFile,seqLabel,'Mix','',quadrantNo, \
                                                     useBufferTip,absVol,isRelative,proportion,mixCycles,isMixType,sampVol, \
                                                    srcText,None,tiprackText)

                #short circuit if command is done
                if cmd.isResusMixType():
                    continue
            
                srcText = self.getVialText(cmd.srcVial2,sample.initialQuadrant,customNames)
                destText = self.getVialText(cmd.destVial2,sample.initialQuadrant,customNames)
                isMixType = False
                if cmd.isTopUpTransType() or cmd.isTopUpTransSepTransType():
                    absVol = cmd.absVolume2/1000
                    isRelative = cmd.relative2
                    proportion = cmd.proportion2
                    seqLabel = "%db" % (seqNum)
                else:
                    absVol = cmd.absVolume3/1000
                    isRelative = cmd.relative3
                    proportion = cmd.proportion3
                    seqLabel = "%dc" % (seqNum)
                    
                #all commands left have transport as the next part except resusmixseptrans
                if cmd.isResusMixSepTransType():
                    self.GenCommandContentEmpty(reportFile,seqLabel,'Separate','',quadrantNo)
                    seqLabel = "%dd" % (seqNum)

                #deal with transport part
                self.GenCommandContentTransportOrMix(reportFile,seqLabel,'Transport','',quadrantNo, \
                                                     useBufferTip,absVol,isRelative,proportion,0,isMixType,sampVol, \
                                                    srcText,destText,tiprackText)

                #short circuit if command is done
                if cmd.isResusMixSepTransType() or cmd.isTopUpTransType() or cmd.isTopUpMixTransType():
                    continue


                #only cmd.isTopUpTransSepTransType() or cmd.isTopUpMixTransSepTransType() are left

                srcText = self.getVialText(cmd.srcVial3,sample.initialQuadrant,customNames)
                destText = self.getVialText(cmd.destVial3,sample.initialQuadrant,customNames)
                isMixType = False
                if cmd.isTopUpTransSepTransType():
                    absVol = cmd.absVolume3/1000
                    isRelative = cmd.relative3
                    proportion = cmd.proportion3
                    seqLabel = "%dc" % (seqNum)
                    seqLabel2 = "%dd" % (seqNum)

                else:
                    absVol = cmd.absVolume4/1000
                    isRelative = cmd.relative4
                    proportion = cmd.proportion4
                    seqLabel = "%dd" % (seqNum)
                    seqLabel2 = "%de" % (seqNum)



                self.GenCommandContentEmpty(reportFile,seqLabel,'Separate','',quadrantNo)
                self.GenCommandContentTransportOrMix(reportFile,seqLabel2,'Transport','',quadrantNo, \
                                                     useBufferTip,absVol,isRelative,proportion,0,isMixType,sampVol, \
                                                    srcText,destText,tiprackText)

            else:

                self.GenCommandContentEmpty(reportFile,seqNum,cmd.type.replace('Command',''),cmd.label,quadrantNo)

                
    

# eof
