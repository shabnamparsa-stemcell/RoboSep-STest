//----------------------------------------------------------------------------
// ProtocolCommand
//
// Invetech Pty Ltd
// Victoria, 3149, Australia
// Phone:   (+61) 3 9211 7700
// Fax:     (+61) 3 9211 7701
//
// The copyright to the computer program(s) herein is the property of 
// Invetech Pty Ltd, Australia.
// The program(s) may be used and/or copied only with the written permission 
// of Invetech Pty Ltd or in accordance with the terms and conditions 
// stipulated in the agreement/contract under which the program(s)
// have been supplied.
// 
// Copyright © 2004. All Rights Reserved.
//
//     03/29/06 - pause command - RL (look for PauseCommand)
//----------------------------------------------------------------------------
//
// 2011-09-06 sp 
//     - provide status to indicate if volume levels are correct
//
//----------------------------------------------------------------------------
using System;

using Tesla.Common.ProtocolCommand;
using Tesla.Common.Separator;

namespace Tesla.ProtocolEditorModel
{	
	/// <summary>
	/// Summary description for ProtocolCommand.
	/// </summary>
	public abstract class ProtocolCommand : IProtocolCommand
	{
		public enum CommandItem
		{			
			CommandSequenceNumber,
			CommandLabel,
			CommandSubtype,
/*CR*/		CommandSummary,
            // added 2011-09-06 sp
            // provide status to indicate if the volume levels are correct
            CommandCheckStatus,
		}

		private SubCommand	mySubtype;
		private uint			mySequenceNumber;
		private string		myLabel;
		private uint		myExtensionTime;
/*CR*/	private string      mySummary;
        // added 2011-09-06 sp
        // provide status to indicate if the volume levels are correct
        private VolumeCheck myCommandStatus;
        private string myCommandStatusStr;

		#region Construction/Destruction

		public ProtocolCommand()
		{
			myLabel = string.Empty;
			SetProtocolCommandSubtype();
		}

		private void SetProtocolCommandSubtype()
		{
			if (null != this as ProtocolHomeAllCommand)
			{
				mySubtype = SubCommand.HomeAllCommand;
			}
			else if (null != this as ProtocolDemoCommand)
			{
				mySubtype = SubCommand.DemoCommand;
			}
			else if (null != this as ProtocolPumpLifeCommand)
			{
				mySubtype = SubCommand.PumpLifeCommand;
			}
			else if (null != this as ProtocolIncubateCommand)
			{
				mySubtype = SubCommand.IncubateCommand;
			}
			else if (null != this as ProtocolSeparateCommand)
			{
				mySubtype = SubCommand.SeparateCommand;
			}
			else if (null != this as ProtocolTransportCommand)
			{
				mySubtype = SubCommand.TransportCommand;
			}
			else if (null != this as ProtocolMixCommand)
			{
				mySubtype = SubCommand.MixCommand;
			}
			else if (null != this as ProtocolTopUpVialCommand)
			{
				mySubtype = SubCommand.TopUpVialCommand;
			}
			else if (null != this as ProtocolResuspendVialCommand)
			{
				mySubtype = SubCommand.ResuspendVialCommand;
			}
			else if (null != this as ProtocolFlushCommand)
			{
				mySubtype = SubCommand.FlushCommand;
			}
			else if (null != this as ProtocolPrimeCommand)
			{
				mySubtype = SubCommand.PrimeCommand;
			}
			else if (null != this as ProtocolPauseCommand)
			{
				mySubtype = SubCommand.PauseCommand;
            }
            else if (null != this as ProtocolTopUpMixTransSepTransCommand)
            {
                mySubtype = SubCommand.TopUpMixTransSepTransCommand;
            }
            else if (null != this as ProtocolTopUpMixTransCommand)
            {
                mySubtype = SubCommand.TopUpMixTransCommand;
            }
            else if (null != this as ProtocolTopUpTransSepTransCommand)
            {
                mySubtype = SubCommand.TopUpTransSepTransCommand;
            }
            else if (null != this as ProtocolTopUpTransCommand)
            {
                mySubtype = SubCommand.TopUpTransCommand;
            }
            else if (null != this as ProtocolResusMixSepTransCommand)
            {
                mySubtype = SubCommand.ResusMixSepTransCommand;
            }
            else if (null != this as ProtocolResusMixCommand)
            {
                mySubtype = SubCommand.ResusMixCommand;
            }
            else if (null != this as ProtocolMixTransCommand)
            {
                mySubtype = SubCommand.MixTransCommand;
            }
		}

		#endregion Construction/Destruction

		#region ICommand Members

		public SubCommand CommandSubtype
		{
			get
			{				
				return mySubtype;
			}
		}

		public uint CommandSequenceNumber
		{
			get
			{
				return mySequenceNumber;
			}
			set
			{
				mySequenceNumber = value;
			}
		}
									
// IAT - added switch statement: used to display name instead of variable:
//		 eg. "TopUpVialCommand" would now display as "Top Up Vial"
/*CR*/	public string CommandSummary
		{
			get
			{
				string subtypeToDisplay = "";
				bool longNameFlag = false;
				switch(mySubtype)
				{
					case SubCommand.DemoCommand:
						subtypeToDisplay = "Demo";
						break;
					case SubCommand.PumpLifeCommand:
						subtypeToDisplay = "PumpLife";
						break;
					case SubCommand.FlushCommand:
						subtypeToDisplay = "Flush";
						break;
					case SubCommand.HomeAllCommand:
						subtypeToDisplay = "Home All";
						break;
					case SubCommand.IncubateCommand:
						subtypeToDisplay = "Incubate";
						break;
					case SubCommand.MixCommand:
						subtypeToDisplay = "Mix";
						break;
					case SubCommand.PrimeCommand:
						subtypeToDisplay = "Prime";
						break;
					case SubCommand.ResuspendVialCommand:
						subtypeToDisplay = "Resuspend Vial";
						longNameFlag = true;
						break;
					case SubCommand.SeparateCommand:
						subtypeToDisplay = "Separate";
						break;
					case SubCommand.TopUpVialCommand:
						subtypeToDisplay = "Top Up Vial";
						longNameFlag = true;
						break;
					case SubCommand.TransportCommand:
						subtypeToDisplay = "Transport";
                        break;
                    case SubCommand.PauseCommand:
                        subtypeToDisplay = "Pause";
                        break;
                    case SubCommand.TopUpMixTransSepTransCommand:
                        subtypeToDisplay = "TopUpMixTransSepTrans";
                        longNameFlag = true;
                        break;
                    case SubCommand.TopUpMixTransCommand:
                        subtypeToDisplay = "TopUpMixTrans";
                        longNameFlag = true;
                        break;
                    case SubCommand.TopUpTransSepTransCommand:
                        subtypeToDisplay = "TopUpTransSepTrans";
                        longNameFlag = true;
                        break;
                    case SubCommand.TopUpTransCommand:
                        subtypeToDisplay = "TopUpTrans";
                        longNameFlag = true;
                        break;
                    case SubCommand.ResusMixSepTransCommand:
                        subtypeToDisplay = "ResusMixSepTrans";
                        longNameFlag = true;
                        break;
                    case SubCommand.ResusMixCommand:
                        subtypeToDisplay = "ResusMix";
                        longNameFlag = true;
                        break;
                    case SubCommand.MixTransCommand:
                        subtypeToDisplay = "MixTrans";
                        longNameFlag = true;
                        break;
						// if not of above types, must be an error.
					default:
						subtypeToDisplay = "ERROR";
						break;
				}//switch

				if (longNameFlag)
					mySummary = mySequenceNumber+"\t"+subtypeToDisplay+"\t"+myLabel;
				else
					mySummary = mySequenceNumber+"\t"+subtypeToDisplay+"\t\t"+myLabel;

				return mySummary;
			}
		}

		public string CommandLabel
		{
			get
			{
				return myLabel;
			}
			set
			{
				myLabel = value;
			}
		}

		public uint CommandExtensionTime
		{
			get
			{
				return myExtensionTime;
			}
			set
			{
				myExtensionTime = value;
			}
		}

        // added 2011-09-06 sp
        // provide status to indicate if the volume levels are correct
        public VolumeCheck CommandCheckStatus
        {
            get
            {
                return myCommandStatus;
            }
            set
            {
                myCommandStatus = value;
            }
        }

        // added 2011-09-06 sp
        // provide status to indicate if the volume levels are correct
        public string CommandStatus
        {
            get
            {
                return myCommandStatusStr;
            }
            set
            {
                myCommandStatusStr = value;
            }
        }

        public virtual VolumeError validateCommandAndUpdateCommandStatus(VolumeLimits volumeLimits, int sampleVolumeMin, int sampleVolumeMax)
        {
            return VolumeError.NO_ERROR;
        }
        #endregion		
	}


    
}
