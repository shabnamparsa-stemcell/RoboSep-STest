//----------------------------------------------------------------------------
// SeparatorGateway
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
//    03/24/05 - added public bool IsLidClosed() // bdr
//	03/29/06 - Short Description for Sample Volume Dialog - RL
//
//----------------------------------------------------------------------------
          
#define bdr9    // protocol class
#define bdr10   // is lid closed

using System;
using System.Diagnostics;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.Threading;
using System.Data;
using System.Globalization;

using Invetech.ApplicationLog;

using Tesla.Common;
using Tesla.Common.ResourceManagement;
using Tesla.Common.Separator;
using Tesla.Common.Protocol;
using Tesla.Common.OperatorConsole;

using Tesla.Separator;

namespace Tesla.OperatorConsoleControls
{
    #region Event delegates

    public delegate void SeparationProtocolTableDelegate(DataTable separationProtocols);

    public delegate void SampleTableDelegate(DataTable chosenProtocols, ProtocolMix protocolMix);

	public delegate void GuiStateChangeDelegate();

	public delegate void MessageLogTableDelegate(DataTable runLogMessages);

    #endregion Event delegates

    /// <summary>
    /// A common point of access to the ISeparator/ISeparatorEvents APIs for use by
    /// all UI objects.  
    /// </summary>
    /// <remarks>
    /// Singleton.  Strictly speaking, this class does not belong in Operator Console
    /// Controls, but since it is used by both OperatorConsole and 
    /// OperatorConsoleControls, this project was a convenient host (rather than create 
    /// a new project for one file).
    /// NOTE: because this class does not inherit from Control (and hence from 
    /// ISynchronizeInvoke), we cannot call "InvokeRequired" here to test if the current
    /// thread is the UI thread.  Therefore, we must synchronise access where required, to
    /// prevent both a UI thread call and an event thread call being active in any critical
    /// section of code in this object.
    /// </remarks>
	public class SeparatorGateway
	{
		private static SeparatorGateway theSeparatorGateway;    // Singleton instance variable

		private ISeparator				mySeparator;
		private SeparatorEventSink		myEventSink;
		private CultureInfo				myCultureInfo;

		private SeparatorState          mySeparatorState;
		private SeparatorState			myPreviousSeparatorState = SeparatorState.BatchComplete;
		private QuadrantId              myCurrentQuadrant;
		private DataTable				mySeparationProtocolsTableInfo;
		private ISeparationProtocol[]	mySeparationProtocols;

		private SortedList				myChosenProtocols;
		private DataTable               myChosenProtocolsTableInfo;
		private FluidVolume[]           mySampleVolumes = new FluidVolume[(int)QuadrantId.NUM_QUADRANTS];

		private SortedList				myProtocolConsumables;		

		private ArrayList				myReagentLotIds;

		private ProtocolMix				myProtocolMix;

		private DataTable				myRunMessagesTableInfo;

		private string					myBatchRunUserId = string.Empty;

		private FluidVolume				myBccmDeadVolume = new FluidVolume(0.0d, FluidVolumeUnit.MicroLitres);

		#region Construction/destruction

		public static SeparatorGateway GetInstance()
		{
			if (theSeparatorGateway == null)
			{
				try 
				{
					theSeparatorGateway = new SeparatorGateway();
				}
				catch
				{
					theSeparatorGateway = null;
				}
			}
			return theSeparatorGateway;
		}

		private SeparatorGateway()
		{
			// By default, we assume the user wants to select Separation protocols as the
			// initial action (not Maintenance protocols).
			myProtocolMix = ProtocolMix.SeparationOnly;

			// Initialise the local fluid volumes to a negative (invalid) volume
			for (int q=0; q < mySampleVolumes.Length; ++q)
			{
				mySampleVolumes[q].Amount = -1;
			}
		}

		/// <summary>
		/// Second-stage initialisation for SeparatorGateway.
		/// </summary>
		/// <remarks>
		/// Two-stage initialisation is used to avoid design-time problems with
		/// controls that register for run-time events via this object.
		/// </remarks>
		public void InitialiseConnection(CultureInfo cultureInfo)
		{			
			// Store the culture for later use in formatting etc.
			myCultureInfo = cultureInfo;

			// Load the console configuration from the config file
			LoadConfigurationSettings();

			// Initialise local data stores
			LocalInitialise();

			// Create a proxy for the Separator instrument
			InitialiseSeparatorConnection();

			// Register for Separator events of local interest 
			RegisterForSeparatorEvents();		
		}

		private void LocalInitialise()
		{
			// Reset any state variables that are not updated from the instrument (that is,
			// UI-specific state);
			ResetUiSpecificState();

			// Initialise space for the chosen protocols 
			myChosenProtocols = new SortedList();
			myReagentLotIds = new ArrayList();

			// Create the table structure for the list of available separation protocols
			mySeparationProtocolsTableInfo = new DataTable("SeparationProtocolsInfo");
			mySeparationProtocolsTableInfo.Columns.Add("ProtocolName", typeof(string));						
			mySeparationProtocolsTableInfo.Columns.Add("Quadrants", typeof(string));
			mySeparationProtocolsTableInfo.Columns.Add("Type", typeof(string));

			// Create the table structure and a "blank" table to list the Chosen Protocol 
			// information.
			// This will initially consist of 4 mostly-blank rows with a prompt in the
			// position of the protocol name for quadrant 1.
			// NOTE: the "SampleVolume" column is typed as 'string' so we can display
			// a blank or similiar (e.g. 'n/a') indication for protocols such as
			// maintenance protocols which do not have a sample volume.
			myChosenProtocolsTableInfo = new DataTable("ChosenProtocolsInfo");
			myChosenProtocolsTableInfo.Columns.Add("InitialQuadrant", typeof(int));
			myChosenProtocolsTableInfo.Columns.Add("ProtocolName", typeof(string));
			myChosenProtocolsTableInfo.Columns.Add("SampleVolume", typeof(string));
			myChosenProtocolsTableInfo.Columns.Add("Type", typeof(string));
			myChosenProtocolsTableInfo.Columns.Add("QuadrantCount", typeof(string));

			// Create the table structure for the run log
			myRunMessagesTableInfo = new DataTable("MessageLogInfo");
			myRunMessagesTableInfo.Columns.Add("Time", typeof(DateTime));
			myRunMessagesTableInfo.Columns.Add("Severity", typeof(string));
			myRunMessagesTableInfo.Columns.Add("Text", typeof(string));

			// Initialise the storage for protocol consumables
			myProtocolConsumables = new SortedList();			
		}

		#endregion Construction/destruction

		#region Events

		public event SeparationProtocolTableDelegate    UpdateSeparationProtocolTable;
		public event SampleTableDelegate                UpdateChosenProtocolTable;

		public event MessageLogTableDelegate			UpdateMessageLogTable;

		/// <summary>
		/// Notify the GUI that its state needs to be updated 
		/// </summary>
 		public event GuiStateChangeDelegate				UpdateGuiStateChange;


		private void NotifySeparationProtocolTableUpdate(DataTable aSeparationProtocolData)
		{
			if (UpdateSeparationProtocolTable != null)
			{
				UpdateSeparationProtocolTable(aSeparationProtocolData);
			}
		}

		private void NotifyChosenProtocolTableUpdate(ProtocolMix protocolMix)
		{
			if (UpdateChosenProtocolTable != null)
			{
				UpdateChosenProtocolTable(myChosenProtocolsTableInfo, protocolMix);
			}
		}

		/// <summary>
		/// Notify the GUI that its state needs to be updated 
		/// </summary>
 		private void NotifyGuiStateChangeUpdate()
		{
			if (UpdateGuiStateChange != null)
			{
				UpdateGuiStateChange();
			}
		}

		private void NotifyMessageLogUpdate(DataTable aLogMessageData)
		{
			if (UpdateMessageLogTable != null)
			{
				UpdateMessageLogTable(aLogMessageData);
			}
		}

		#endregion Events

		#region UI object services

		#region Properties

		public ISeparator ControlApi
		{
			get
			{
				return mySeparator;
			}
		}

		public SeparatorEventSink EventsApi
		{
			get
			{
				return myEventSink;
			}
		}

		public SeparatorState SeparatorState
		{
			get
			{
				return mySeparatorState;
			}
		}

		public int SelectedProtocolsCount
		{
			get
			{
				return myChosenProtocols.Count;
			}
		}

		public int AvailableQuadrantsCount
		{
			get
			{
				int usedQuadrants = 0;
				switch (myProtocolMix)
				{
					default:
					case ProtocolMix.ShutdownOnly:
					case ProtocolMix.MaintenanceOnly:
						// For other than Separation protocols, the carousel is considered
						// fully populated if one protocol is selected.
						usedQuadrants = (int)QuadrantId.NUM_QUADRANTS;
						break;
					case ProtocolMix.SeparationOnly:
						// All chosen protocols are Separation protocols, so total the
						// number of quadrants used by all chosen protocols.
						foreach (IProtocol protocol in myChosenProtocols.Values)
						{
							usedQuadrants += protocol.QuadrantCount;
						}
						break;
				}			
				return (int)QuadrantId.NUM_QUADRANTS - usedQuadrants;
			}
		}

		public FluidVolume TotalBufferVolume
		{
			get
			{				
				double volume = 0.0d;
				ProtocolConsumable[,] protocolConsumables = null;
				for (QuadrantId initialQuadrant = QuadrantId.Quadrant1; 
					initialQuadrant < QuadrantId.NUM_QUADRANTS;
					/* initialQuadrant is indexed in the loop according to the protocol's quadrant usage */)
				{
					int relativeQuadrant = 0;
					if (myProtocolConsumables.ContainsKey(initialQuadrant))
					{
						protocolConsumables = (ProtocolConsumable[,])myProtocolConsumables[initialQuadrant];
						for (; relativeQuadrant < protocolConsumables.GetLength(0);	++relativeQuadrant)
						{
							ProtocolConsumable quadrantBufferConsumable = 
								protocolConsumables[relativeQuadrant, (int)RelativeQuadrantLocation.QuadrantBuffer];
							if (quadrantBufferConsumable.IsRequired)
							{
								FluidVolume quadrantBufferVolume = quadrantBufferConsumable.Volume;
								// Ensure the volume is in consistent units
								quadrantBufferVolume.Unit = FluidVolumeUnit.MilliLitres;
								volume += quadrantBufferVolume.Amount;
							}
						}
					}
					else
					{
						// There was no protocol that used the current relative quadrant, so move to the next
						// quadrant.
						++relativeQuadrant;
					}
					initialQuadrant += relativeQuadrant;
				}

				// Include the BCCM dead volume in the total amount (report volume in mL)
				myBccmDeadVolume.Unit = FluidVolumeUnit.MilliLitres;
				volume += myBccmDeadVolume.Amount;

				FluidVolume bufferVolume = new FluidVolume(volume, FluidVolumeUnit.MilliLitres);
				return bufferVolume;
			}
		}

		public string BatchRunUserId
		{
			set
			{
				myBatchRunUserId = value;
			}
		}

		#endregion Properties

		public void Connect(bool reqSubscribe)
		{
			mySeparator.Connect(reqSubscribe);
		}

		public void Disconnect(bool isExit)
		{
			mySeparator.Disconnect(isExit);
		}

		/// <summary>
		/// Reset the batch run message log.
		/// </summary>
		private void ResetMessageLog()
		{			
			// In resetting the log, we add a dummy message for display purposes
			AddRunLogMessage(TraceLevel.Info, StatusCode.TSC3000, new string[]{}, true, true);
		}

		/// <summary>
		/// Add a message to the run log without copying it to the log file.
		/// </summary>
		/// <param name="severity"></param>
		/// <param name="statusMsgCode"></param>
		/// <param name="statusMessageValues"></param>
		public void AddRunLogMessage(TraceLevel severity, StatusCode statusMsgCode,
			string[] statusMessageValues)
		{
			// Add a message to the run log without resetting the log
			AddRunLogMessage(severity, statusMsgCode, statusMessageValues, false, false);
		}

		/// <summary>
		/// Add a message to the run log and optionally copy it to the log file.
		/// </summary>
		/// <param name="severity"></param>
		/// <param name="statusMsgCode"></param>
		/// <param name="statusMessageValues"></param>
		/// <param name="isAddToLogFile"></param>
		public void AddRunLogMessage(TraceLevel severity, StatusCode statusMsgCode,
			string[] statusMessageValues, bool isAddToLogFile)
		{
			// Add a message to the run log without resetting the log
			AddRunLogMessage(severity, statusMsgCode, statusMessageValues, isAddToLogFile, false);
		}

		private void AddRunLogMessage(TraceLevel severity, StatusCode statusMsgCode, 
			string[] statusMessageValues, bool isAddToLogFile, bool isLogReset)
		{
			try
			{
				if (isLogReset)
				{
					// Clear the run log entries
					myRunMessagesTableInfo.Clear();
				}

				// Set the appropriate culture, as this method may be called on a thread other than the UI thread.
				Thread.CurrentThread.CurrentCulture	= myCultureInfo;
				Thread.CurrentThread.CurrentUICulture = myCultureInfo;

				// Use the message code to lookup a localised string and populate its
				// run-time parameters (if required).
				string severityText = string.Empty;
				switch (severity)
				{
					case TraceLevel.Verbose:
						severityText = SeparatorResourceManager.GetSeparatorString(StringId.TraceLevelVerbose);
						break;
					case TraceLevel.Info:
						severityText = SeparatorResourceManager.GetSeparatorString(StringId.TraceLevelInfo);
						break;
					case TraceLevel.Warning:
						severityText = SeparatorResourceManager.GetSeparatorString(StringId.TraceLevelWarning);
						break;
					case TraceLevel.Error:
						severityText = SeparatorResourceManager.GetSeparatorString(StringId.TraceLevelError);
						break;
				}
				string msgText = string.Empty;
				string statusMessage = SeparatorResourceManager.GetStatusMessageString(statusMsgCode.ToString());
				statusMessage = string.Format(statusMessage, statusMessageValues);

				// Add the message to the on-screen run log.
				DataRow newLogItem = myRunMessagesTableInfo.Rows.Add(new object[]
				{
					DateTime.Now,
					severityText,
					statusMessage
				});
				NotifyMessageLogUpdate(myRunMessagesTableInfo);

				// Also add the message to the UI log file if required.
				if (isAddToLogFile)
				{
					LogFile.AddMessage(severity, statusMessage);
				}
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}

		public void SelectProtocol(QuadrantId sampleQuadrantId, int selectedProtocolIndex)
		{            
			// Update sample quadrant markers
			myCurrentQuadrant = sampleQuadrantId;

			// Record the fact that the user has requested selection of this protocol.
			AddRunLogMessage(TraceLevel.Info, StatusCode.TSC3007, 
				new string[]{mySeparationProtocols[selectedProtocolIndex].Label}, true);
	

			// Request the new protocol selection as the protocol for this quadrant.  
			// NOTE: we do not immediately update myChosenProtocols -- instead, we wait for
			// the update event to ensure that the system has accepted the selection request.
			mySeparator.SelectProtocol(sampleQuadrantId,
				mySeparationProtocols[selectedProtocolIndex]);
		}

		// Check that there is sufficient space for a protocol on the carousel.
		// This verification is necessary for multi-quadrant protocols.
		public void VerifyProtocolSelection(QuadrantId sampleQuadrantId, 
            int selectedProtocolIndex)
        {
            int lastQuadrant = (int)sampleQuadrantId 
                + mySeparationProtocols[selectedProtocolIndex].QuadrantCount - 1;

            // Only need to check multi-quadrant protocols
            if ( mySeparationProtocols[selectedProtocolIndex].QuadrantCount > 1 )
            {
                // Check the destination quadrants are free.
                int availableQuadrants = GetMaxProtocolQuadrants(sampleQuadrantId);
                if (availableQuadrants < mySeparationProtocols[selectedProtocolIndex].QuadrantCount)
                {
                    throw new ApplicationException("The selected protocol does not fit on the carousel");
                }
            }
        }

		/// <summary>
		/// Returns the maximum number of quadrants available for a protocol with the initial
		/// quadrant at the specified location. A protocol at quadrantID is ignored, as it will
		/// get overwritten.
		/// </summary>
		public int GetMaxProtocolQuadrants(QuadrantId quadrantID)
		{
			int freeQuadrants = 1;
			for (int quad = (int)quadrantID + 1; quad < (int)QuadrantId.NUM_QUADRANTS; quad++)
			{
				if (myChosenProtocols.Contains( (QuadrantId)quad) )
				{
					break;
				}
				else
				{
					freeQuadrants++;
				}
			}
			return freeQuadrants;
		}

		public void RemoveSelectedProtocol(QuadrantId quadrantId)
		{
			// Update sample quadrant markers
			myCurrentQuadrant = quadrantId;

			if (myChosenProtocols.Contains(quadrantId))
			{
				// Record the fact that the user has requested deselection of this protocol.
				ISeparationProtocol protocol = myChosenProtocols[quadrantId] as ISeparationProtocol;
				if (protocol != null)
				{
					AddRunLogMessage(TraceLevel.Info, StatusCode.TSC3008, new string[]{protocol.Label}, true);
				}
				// Request the protocol is removed from this quadrant.  
				// NOTE: we do not immediately update myChosenProtocols -- instead, we wait for
				// the update event to ensure that the system has accepted the deselection request.
				// Any consumables associated with the chosen protocol 
				mySeparator.DeselectProtocol(quadrantId);
			}
		}

		public void GetProtocolSampleVolumeRange(int protocolIndex,
			out FluidVolume minimumVolume, out FluidVolume maximumVolume)
		{
			ISeparationProtocol separationProtocol = mySeparationProtocols[protocolIndex];
			// NOTE: Until Separator layer is refactored to use FluidVolume and not
			// just FluidVolume.Amount, construct 'helper' FluidVolume objects before
			// assigning to the ones to return.
			FluidVolume min = new FluidVolume(separationProtocol.MinimumSampleVolume,
				FluidVolumeUnit.MicroLitres);
			FluidVolume max = new FluidVolume(separationProtocol.MaximumSampleVolume,
				FluidVolumeUnit.MicroLitres);

			// Assign the protocol's valid sample volume range
			minimumVolume = new FluidVolume(min.Amount, min.Unit);
			maximumVolume = new FluidVolume(max.Amount, max.Unit);     
		}

		// RL - Short Description for Sample Volume Dialog - 03/29/06
		public void GetProtocolDescription(int protocolIndex, out string description)
		{
			ISeparationProtocol separationProtocol = mySeparationProtocols[protocolIndex];
			description = separationProtocol.Description;
		}

		public void SetSampleVolume(QuadrantId sampleQuadrantId, FluidVolume sampleVolume)
		{
			// Update sample quadrant markers
			myCurrentQuadrant = sampleQuadrantId;

			// NOTE: this routine assumes the volume has already been verified (that is,
			// the volume is within the valid range for the selected protocol).

			// set the sample volume at the Separator (triggers recalculation of
			// the consumables required for this protocol/batch)
			// Note: we need to convert to microLitres at this point because the
			// Separator and InstrumentControl layers deal in microLitres only at this time.
			sampleVolume.Unit = FluidVolumeUnit.MicroLitres;
			mySeparator.SetSampleVolume(sampleQuadrantId, sampleVolume);   
		}

		public string FormatFluidVolume(FluidVolume fluidVolume, bool includeUnits)
		{
			string strMilliLitres = SeparatorResourceManager.GetSeparatorString(
				StringId.MilliLitres);
			string strMicroLitres = SeparatorResourceManager.GetSeparatorString(
				StringId.MicroLitres);

			string strUnit = fluidVolume.Unit == FluidVolumeUnit.MicroLitres ?
			strMicroLitres : strMilliLitres;

			// Format the fluid volume using the appropriate culture info
			string result = string.Format(myCultureInfo.NumberFormat, "{0:F3}", fluidVolume.Amount);
			result += includeUnits ? " " + strUnit : string.Empty;

			return result;
		}

		// This function is required to determine the sample volume min/max for a previously
		// added protocol.
		public void GetSelectedProtocolSampleVolumeRange(QuadrantId quadrantId,
			out FluidVolume minimumVolume, out FluidVolume maximumVolume)
		{
			minimumVolume = new FluidVolume(0, FluidVolumeUnit.MicroLitres);
			maximumVolume = new FluidVolume(0, FluidVolumeUnit.MicroLitres); 

			if (myChosenProtocols.Contains(quadrantId))
			{
				ISeparationProtocol protocol = myChosenProtocols[quadrantId] as ISeparationProtocol;
				// Note that the protocol may not have a sample volume
				if (protocol != null)
				{
					minimumVolume.Amount = protocol.MinimumSampleVolume;
					maximumVolume.Amount = protocol.MaximumSampleVolume;
				}
			}
		}

		// RL - Short Description for Sample Volume Dialog - 03/29/06
		public void GetSelectedProtocolDescription(QuadrantId quadrantId, out string description)
		{
			description ="";

			if (myChosenProtocols.Contains(quadrantId))
			{
				ISeparationProtocol protocol = myChosenProtocols[quadrantId] as ISeparationProtocol;
				// Note that the protocol may not have a sample volume
				if (protocol != null)
				{
					description = protocol.Description;
				}

			}
		}

		public void GetProtocolQuadrantBoundaries(int selectedProtocolIndexNumber,
			out QuadrantId firstQuadrant, out QuadrantId lastQuadrant)
		{
			firstQuadrant = lastQuadrant = QuadrantId.NUM_QUADRANTS;
			if (selectedProtocolIndexNumber >= 0 && 
				selectedProtocolIndexNumber < myChosenProtocols.Count &&
				myChosenProtocols.Count > 0)
			{
				IProtocol protocol = myChosenProtocols.GetByIndex(selectedProtocolIndexNumber) as IProtocol;
				if (protocol != null)
				{
					firstQuadrant = protocol.InitialQuadrant;
					lastQuadrant  = (QuadrantId)((int)firstQuadrant + (int)protocol.QuadrantCount - 1);
				}
			}
		}

		#if bdr9
		public ProtocolConsumable[,] GetProtocolConsumables(int selectedProtocolIndexNumber,
            	out string protocolName, out ProtocolClass protocolClass, out int initialQuadrant)
		{
			ProtocolConsumable[,] protocolConsumables = null;
           protocolName = string.Empty;
			protocolClass = 0;
			initialQuadrant = 0;
			try
			{		
				QuadrantId lookupKey = (QuadrantId)myChosenProtocols.GetKey(selectedProtocolIndexNumber);
				protocolConsumables = (ProtocolConsumable[,])myProtocolConsumables[lookupKey];
                		protocolName = ((IProtocol)myChosenProtocols[lookupKey]).Label;
				protocolClass = ((IProtocol)myChosenProtocols[lookupKey]).Classification;
				initialQuadrant = (int)((IProtocol)myChosenProtocols[lookupKey]).InitialQuadrant;
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
			
			return protocolConsumables;
		}		
		
		#else
			
		public ProtocolConsumable[,] GetProtocolConsumables(int selectedProtocolIndexNumber,
            out string protocolName)
		{
			ProtocolConsumable[,] protocolConsumables = null;
            protocolName = string.Empty;
			try
			{
				QuadrantId lookupKey = (QuadrantId)myChosenProtocols.GetKey(selectedProtocolIndexNumber);
				protocolConsumables = (ProtocolConsumable[,])myProtocolConsumables[lookupKey];
                protocolName = ((IProtocol)myChosenProtocols[lookupKey]).Label;
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
			
			return protocolConsumables;
		}
		#endif
        
		// NOTE: This is currently unused
		public ProtocolConsumable[,] GetProtocolConsumables(QuadrantId initialQuadrant)
		{
			ProtocolConsumable[,] protocolConsumables = null;
			try
			{
				if (myProtocolConsumables.ContainsKey(initialQuadrant))
				{
					protocolConsumables = (ProtocolConsumable[,])myProtocolConsumables[initialQuadrant];
				}
				else
				{
					string strErrorNoMatchingData = string.Format("No consumables data for quadrant {0}", initialQuadrant);
					throw new ApplicationException(strErrorNoMatchingData);
				}
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
			
			return protocolConsumables;
		}


/*CR*/	public void GetCustomNames(string name, out string[]customNames, out int numQuadrants)
		{
			//customNames = new string[8];
			mySeparator.GetCustomNames(name, out customNames, out numQuadrants);
		}
		public void GetResultVialSelection(string name, out bool[]resultChecks, out int numQuadrants)
		{
			//customNames = new string[8];
			mySeparator.GetResultVialSelection(name, out resultChecks, out numQuadrants);
		}

		public void SetProtocolReagentLotIds(int chosenProtocolIndex, 
			ReagentLotIdentifiers reagentLotIds)
		{
			if (chosenProtocolIndex < myReagentLotIds.Count)
			{
				myReagentLotIds[chosenProtocolIndex] = reagentLotIds;
			}
			else
			{
				myReagentLotIds.Add(reagentLotIds);
			}
		}

		public void ScheduleRun()
		{
			// We only need to schedule the initial quadrant for each protocol
			mySeparator.ScheduleRun(myCurrentQuadrant);
		}

		public void StartRun(bool isSharing, int[] sharedSectorsTranslation, int[]sharedSectorsProtocolIndex)
		{
			NotifyChosenProtocolTableUpdate(myProtocolMix);
			mySeparator.StartRun(myBatchRunUserId, 
				(ReagentLotIdentifiers[])
					myReagentLotIds.ToArray(typeof(ReagentLotIdentifiers)),
					isSharing,sharedSectorsTranslation,sharedSectorsProtocolIndex);
		}

		#if bdr10
        public bool IsLidClosed() // bdr
        {
        	bool isLidClosed = false;
            try
            {
                isLidClosed = mySeparator.IsLidClosed();
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }

		return isLidClosed;            
        }
		#endif     
        
        public int SetIgnoreLidSensor(int sw)                        //CWJ
        {
            int rtn = -1;
            try
            {
                rtn = mySeparator.SetIgnoreLidSensor(sw);
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
            return rtn;
        }

        public bool GetIgnoreLidSensor()                        //CWJ
        {
            bool rtn = false;
            try
            {
                rtn = mySeparator.GetIgnoreLidSensor();
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
            return rtn;
        }

        public int GetCurrentSec()                        //CWJ
        {
            int rtn = -1;
            try
            {
                rtn = mySeparator.GetCurrentSec();
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
            return rtn;
        }

        public int GetCurrentSeq()                        //CWJ
        {
            int rtn = -1;
            try
            {
                rtn = mySeparator.GetCurrentSeq();
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
            return rtn;
        }

        public int GetCurrentID()                        //CWJ
        {
            int rtn = -1;
            try
            {
                rtn = mySeparator.GetCurrentID();
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
            return rtn;
        }

        public int InitScanVialBarcode()                        //CWJ
        {
            int rtn = -1;
            try
            {
                rtn = mySeparator.InitScanVialBarcode();
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
            return rtn;
        }

        public int ScanVialBarcodeAt(int Quadrant, int Vial)    //CWJ
        {
            int rtn = -1;
            try
            {
                rtn = mySeparator.ScanVialBarcodeAt( Quadrant, Vial);
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
            return rtn;
        }

        public string GetVialBarcodeAt(int Quadrant, int Vial)                 //CWJ
        {
            string rtn = "Error";
            try
            {
                rtn = mySeparator.GetVialBarcodeAt(Quadrant, Vial);
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
            return rtn;
        }
        public string GetInstrumentState()
		{
			string state ="";
			try
			{
				state = mySeparator.GetInstrumentState();
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
			return state;
		}

		//RL - pause command - 03/29/06
		public bool isPauseCommand()
		{
			bool isPauseCommand=false;
			try
			{
				isPauseCommand = mySeparator.isPauseCommand();
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
			return isPauseCommand;
		}

		public string getPauseCommandCaption() 
		{
			string pauseCommandCaption="";
			try
			{
				pauseCommandCaption = mySeparator.getPauseCommandCaption();
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
			return pauseCommandCaption;
		}

        public void ConfirmHydraulicFluidRefilled()
        {
            try
            {
                mySeparator.ConfirmHydraulicFluidRefilled();
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
        }


		public void ReloadProtocols()
		{
			try
			{
				mySeparator.ReloadProtocols();
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}

        #endregion UI object services

        #region Application support

		private int myPauseInstrumentTimeout;

		public int PauseInstrumentTimeout_ms
		{
			get
			{
				return myPauseInstrumentTimeout;
			}
		}

		private int myInstrumentConnectionTimeout_ms;

		public int InstrumentConnectionTimeout_ms
		{
			get
			{
				return myInstrumentConnectionTimeout_ms;
			}
		}

		private void ResetUiSpecificState()
		{
			// Set initial values for sample quadrant(s) selection
			myCurrentQuadrant = QuadrantId.NoQuadrant;
		}

        private void InitialiseSeparatorConnection()
        {
            // Get Separator Connection configuration parameters
			#if (OPERATORCONSOLEEXE)
            // NOTE: for reference only (part of CDP implementation)
			RemotingConfiguration.Configure("OperatorConsole.exe.config"); 
			#endif
            NameValueCollection nvc = (NameValueCollection)	
				ConfigurationSettings.GetConfig("OperatorConsole/SeparatorConnection");
            int retryCount, retryWait;				
            try
            {
                retryCount = int.Parse(nvc.Get("RetryCount"));
                retryWait  = int.Parse(nvc.Get("RetryWait_ms"));
            }
            catch
            {
                // Set default values in case the parse fails
                retryCount = 30;	
                retryWait  = 2000;	// milliseconds
            }

			myInstrumentConnectionTimeout_ms = retryCount * retryWait;
			Debug.WriteLine(String.Format("------------------InitialiseSeparatorConnection {0} = {1} * {2}",
				myInstrumentConnectionTimeout_ms , retryCount , retryWait));

            do
            {
                try
                {
					LogFile.AddMessage(TraceLevel.Info,"<<<<<<<<<<<<<<<<<<<<<<<,,, 1 "+RemotingInfo.GetInstance().SeparatorServerURL);
                    // Connect to the Separator 
                    mySeparator = (ISeparator)Activator.GetObject(typeof(ISeparator),
						RemotingInfo.GetInstance().SeparatorServerURL); 
					LogFile.AddMessage(TraceLevel.Info,"<<<<<<<<<<<<<<<<<<<<<<<,,, 2");                       

                    // Create a Separator event sink 					
					myEventSink = new SeparatorEventSink((ISeparatorEvents)mySeparator);
					LogFile.AddMessage(TraceLevel.Info,"<<<<<<<<<<<<<<<<<<<<<<<,,, 3");
                    if (myEventSink == null)
                    {
                        throw new ApplicationException(
                            SeparatorResourceManager.GetSeparatorString(
                            StringId.ExFailedToCreateSeparatorEventSink));
                    }
						
                    retryCount = 0;	// Clear the retry count as we're now connected.
                }
                catch (Exception ex)
                {
                    // Connection attempt failed.  Re-try if the number of attempts so far is
                    // within the retry count specified in the application configuration file,
                    // otherwise propagate an exception to tell the caller we failed to connect.
                    mySeparator = null;
                    LogFile.AddMessage(TraceLevel.Warning, "retryCount = " + retryCount + " " + ex.Message);
                    if (--retryCount <= 0)
                    {						
                        throw new ApplicationException(
                            SeparatorResourceManager.GetSeparatorString(
                            StringId.ExFailedToConnectToSeparator));
                    }
                    // Pause before retrying connection attempt
                    Thread.Sleep(retryWait);
                }
            } while (retryCount > 0);
        }

		private void LoadConfigurationSettings()
		{
			NameValueCollection nvc = (NameValueCollection)	
				ConfigurationSettings.GetConfig("OperatorConsole/ConsoleConfiguration");
			try
			{
				myPauseInstrumentTimeout = int.Parse(nvc.Get("PauseInstrumentTimeout_ms"));
			}
			catch
			{
				// Set default values in case the parse fails
				myPauseInstrumentTimeout = 90000;	// milliseconds
			}
		}

        private void RegisterForSeparatorEvents()
        {
            // Register to receive ISeparatorEvents from the event sink
            myEventSink.UpdateSeparatorState += new SeparatorStatusDelegate(AtSeparatorStateUpdate);
            myEventSink.UpdateChosenProtocols += new ChosenProtocolsDelegate(AtChosenProtocolsUpdate);
            myEventSink.UpdateSeparationProtocolListMRU += new SeparationProtocolListDelegate(AtSeparationProtocolListMRU_Update);
			myEventSink.UpdateProtocolConsumables += new ProtocolConsumablesDelegate(AtProtocolConsumablesUpdate);
			myEventSink.ReportInstrumentConfiguration += new InstrumentConfigurationDelegate(AtReportInstrumentConfiguration);
            myEventSink.ReportHydraulicFluidRefillStatus += new HydraulicFluidLevelDelegate(AtHydraulicFluidRefillStatusUpdate);
        }

        private bool isInstrumentInitialised;

        public bool IsInstrumentInitialised
        {
            get
            {
                return isInstrumentInitialised;
            }
        }        

        private bool isHydraulicFluidRefillRequired;

        public bool IsHydraulidFluidRefillRequired
        {
            get
            {
                return isHydraulicFluidRefillRequired;
            }
        }        

        private void AtSeparatorStateUpdate(SeparatorState newState)
        {
            try
            {                
				// Special case related to the splash screen shown at start time.
				if (newState == SeparatorState.SeparatorUnloaded &&
					( ! isInstrumentInitialised))
				{
					lock(this)
					{
						
						System.Diagnostics.Debug.WriteLine(String.Format("------------------SeparatorGateway AtSeparatorStateUpdate 1 {0} {1}",mySeparatorState, newState));
						// Update our state cache
						mySeparatorState = newState;

						// Set the flag to indicate the instrument has finished its
						// initialisation phase.
						isInstrumentInitialised = true;                   
						
						// Clear the run message log
						ResetMessageLog();
					}
				}
				else if (isInstrumentInitialised)	// normal processing
				{
					lock(this)
					{
						System.Diagnostics.Debug.WriteLine(String.Format("------------------SeparatorGateway AtSeparatorStateUpdate 2 {0} {1}",mySeparatorState, newState));
						// Update our state cache
						myPreviousSeparatorState = mySeparatorState;
						mySeparatorState = newState;

						// Update the state of controls according to the system state
						switch (newState)
						{
							case SeparatorState.SeparatorUnloaded:							
								// Clear the run message log at the start of each run 
								// (differentiate between the carousel being unloaded at the
								// start of a run, verses being unloaded if the user deselects
								// all protocols.
								if (myPreviousSeparatorState >= SeparatorState.Running)
								{
									ResetMessageLog();
								}
								break;
							case SeparatorState.NoSample:
							case SeparatorState.Configured:
								break;
							case SeparatorState.Ready:
								break;
							case SeparatorState.Running:
								AddRunLogMessage(TraceLevel.Info, StatusCode.TSC3004, new string[]{}, true);
								goto RunStarted;
							case SeparatorState.ShuttingDown:
								AddRunLogMessage(TraceLevel.Info, StatusCode.TSC3005, new string[]{}, true);
								goto RunStarted;
							RunStarted:
								break;				
							case SeparatorState.Paused:
								AddRunLogMessage(TraceLevel.Info, StatusCode.TSC3003, new string[]{}, true);
								break;
							case SeparatorState.BatchHalted:					
								AddRunLogMessage(TraceLevel.Info, StatusCode.TSC3001, new string[]{}, true);
								goto RunComplete;
							case SeparatorState.BatchComplete:
								AddRunLogMessage(TraceLevel.Info, StatusCode.TSC3002, new string[]{}, true);
								AddRunLogMessage(TraceLevel.Info, StatusCode.TSC3330, new string[]{}, true);
								goto RunComplete;
							RunComplete:
								ResetUiSpecificState();
								break;
							case SeparatorState.Shutdown:							
								break;			
							case SeparatorState.PauseCommand:
								AddRunLogMessage(TraceLevel.Info, StatusCode.TSC3003, new string[]{}, true);
								break;
							default:
								System.Diagnostics.Debug.WriteLine(String.Format("------------------SeparatorGateway AtSeparatorStateUpdate 3 {0} undefined???",newState));
								break;
						}
					}
				}
				// Notify the GUI of the update
				NotifyGuiStateChangeUpdate();
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
        }

        private void AtChosenProtocolsUpdate(IProtocol[] chosenProtocols)
        {
            try
            {
                if (chosenProtocols != null)
                {
                    lock(this)
                    {
                        // Update our cache of the chosen protocols, checking the "sub type"
						// of chosen protocol as we do so.  The "mix" of chosen protocols
						// may affect the way the UI is rendered.
                        myChosenProtocols.Clear();

						// Initially assume only ISeparationProtocol have been chosen.
						ProtocolMix protocolMix = ProtocolMix.SeparationOnly;	

                        foreach (IProtocol protocol in chosenProtocols)
                        {
                            myChosenProtocols.Add(protocol.InitialQuadrant, protocol);                            

							// Determine the "protocol mix"
							if (protocol as ISeparationProtocol == null)
							{
								// One of the protocols is not a separation protocol, so
								// we must have either maintenance or shutdown protocols
								// seleted.
								if (protocol as IShutdownProtocol != null)
								{
									protocolMix = ProtocolMix.ShutdownOnly;
								}
								else if (protocol as IMaintenanceProtocol != null)
								{
									protocolMix = ProtocolMix.MaintenanceOnly;
								}
							}							
                        }

						// Record for later use whether all chosen protocols are 
						// ISeparationProtocols
						myProtocolMix = protocolMix;
					
                        // Now update the protocol table
                        RefreshProtocolTableContents(myProtocolMix);
						// Notify the GUI of the update
						NotifyGuiStateChangeUpdate();
                    }
                }
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
        }


        private static string theSeparationProtocolsPrimaryKey = "Id";

        public static string SeparationProtocolsPrimaryKey
        {
            get
            {
                return theSeparationProtocolsPrimaryKey;
            }
        }


        private void AtSeparationProtocolListMRU_Update(ISeparationProtocol[] separationProtocolsInMRU)
        {
            try
            {
                if (separationProtocolsInMRU != null &&
                    separationProtocolsInMRU.GetLength(0) >= 0)
                {
                    lock(this)
                    {
                        mySeparationProtocols = separationProtocolsInMRU;
						DataTable aSeparationProtocolData = new DataTable("SeparationProtocolsInfo");
						aSeparationProtocolData.Columns.Add("ProtocolName", typeof(string));						
						aSeparationProtocolData.Columns.Add("Quadrants", typeof(string));
						aSeparationProtocolData.Columns.Add("Type", typeof(string));
                        aSeparationProtocolData.Columns.Add("Used", typeof(DateTime));
						aSeparationProtocolData.Columns.Add(theSeparationProtocolsPrimaryKey, 
                            typeof(int));

                        foreach (ISeparationProtocol separationProtocol in mySeparationProtocols)
                        {                          
							aSeparationProtocolData.Rows.Add(new object[]{
								separationProtocol.Label,
								separationProtocol.QuadrantCount.ToString(),
								separationProtocol.StringClassification,/*CR*/
                                separationProtocol.LastUsed,
								separationProtocol.Id});  
                        }
                        NotifySeparationProtocolTableUpdate(aSeparationProtocolData);
                    }
                }
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
        }

		private void AtProtocolConsumablesUpdate(int protocolId, 
			QuadrantId[] quadrantIds, ProtocolConsumable[,] consumables)
		{
			try
			{
				int quadrantCount = quadrantIds.GetLength(0);
				// Both myChosenProtocols and myProtocolConsumables are sorted by
				// the protocol's initial quadrant.  NOTE: these are absolute quadrant IDs.				
				if (quadrantCount != 0)
				{
					lock(this)
					{
						// If the given 'consumables' are null, then the update indicates
						// the resources are being released; otherwise, record the new
						// consumables data.
						if (consumables == null)
						{
							// Remove any previous consumables information for the quadrant(s)
							for (QuadrantId quadrantId = quadrantIds[quadrantIds.GetLowerBound(0)]; 
								quadrantId <= quadrantIds[quadrantIds.GetUpperBound(0)]; 
								++quadrantId)
							{
								if (myProtocolConsumables.ContainsKey(quadrantId))
								{
									myProtocolConsumables.Remove(quadrantId);
								}
								// Remove the sample volume
								mySampleVolumes[(int)quadrantId] = new FluidVolume(-1.0d, FluidVolumeUnit.MilliLitres);
							}
						}
						else	// consumables != null
						{
							// Allocate space to store the new consumables information
							ProtocolConsumable[,] aConsumables = 
								new ProtocolConsumable[(int)quadrantCount, 
								(int)RelativeQuadrantLocation.NUM_RELATIVE_QUADRANT_LOCATIONS];

							// Store the updated consumables so they can be redisplayed if the 
							// user changes the protocol selection.
							int relativeQuadrantIndex = 0;
							for (QuadrantId quadrantId = quadrantIds[quadrantIds.GetLowerBound(0)]; 
								quadrantId <= quadrantIds[quadrantIds.GetUpperBound(0)]; 
								++quadrantId)
							{
								// Update the consumables for the protocol
								for (RelativeQuadrantLocation location = RelativeQuadrantLocation.START_LOCATION; 
									location < RelativeQuadrantLocation.NUM_RELATIVE_QUADRANT_LOCATIONS; ++location)
								{
									ProtocolConsumable update;							
									update	= consumables[relativeQuadrantIndex, (int)location];

									aConsumables[relativeQuadrantIndex, (int)location].IsRequired = 
										update.IsRequired;
									aConsumables[relativeQuadrantIndex, (int)location].Capacity	 =
										update.Capacity;
									aConsumables[relativeQuadrantIndex, (int)location].Volume	 = 
										update.Volume;
									aConsumables[relativeQuadrantIndex, (int)location].AbsoluteQuadrant =
										quadrantId;

									// Update the sample volumes
									if (location == RelativeQuadrantLocation.SampleTube)
									{
										mySampleVolumes[(int)quadrantId] = update.Volume;
										mySampleVolumes[(int)quadrantId].Unit = FluidVolumeUnit.MilliLitres;
									}
								}
								myProtocolConsumables[quadrantId] = aConsumables;

								// Move to the next relative quadrant
								++relativeQuadrantIndex;
							}	
						}

						// Now update the protocol table (using the newly received sample volumes)
						RefreshProtocolTableContents(myProtocolMix);
					}
				}
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}

		private void AtReportInstrumentConfiguration(FluidVolume bccmDeadVolume)
		{
			try
			{
				lock(this)
				{
					myBccmDeadVolume.Unit = bccmDeadVolume.Unit;
					myBccmDeadVolume.Amount = bccmDeadVolume.Amount;
				}
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}

        private void AtHydraulicFluidRefillStatusUpdate(bool isRefillRequired)
        {
            try
            {
                lock(this)
                {
                    isHydraulicFluidRefillRequired = isRefillRequired;
                }
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
        }

		private void RefreshProtocolTableContents(ProtocolMix protocolMix)
		{
			myChosenProtocolsTableInfo.Clear();

			// Set the appropriate culture, as this method may be called on a thread other than the UI thread.
			Thread.CurrentThread.CurrentCulture	= myCultureInfo;
			Thread.CurrentThread.CurrentUICulture = myCultureInfo;

			foreach (IProtocol protocol in myChosenProtocols.Values)
			{
				string protocolType = string.Empty;
				switch (protocol.Classification)
				{
					case ProtocolClass.Positive:
						protocolType = SeparatorResourceManager.GetSeparatorString(StringId.ProtocolClassPositiveText);
						break;
					case ProtocolClass.Negative:
						protocolType = SeparatorResourceManager.GetSeparatorString(StringId.ProtocolClassNegativeText);
						break;
					case ProtocolClass.HumanPositive:/*CR*/
						protocolType = SeparatorResourceManager.GetSeparatorString(StringId.ProtocolClassHumanPositiveText);
						break;
					case ProtocolClass.HumanNegative:/*CR*/
						protocolType = SeparatorResourceManager.GetSeparatorString(StringId.ProtocolClassHumanNegativeText);
						break;
					case ProtocolClass.MousePositive:/*CR*/
						protocolType = SeparatorResourceManager.GetSeparatorString(StringId.ProtocolClassMousePositiveText);
						break;
					case ProtocolClass.MouseNegative:/*CR*/
						protocolType = SeparatorResourceManager.GetSeparatorString(StringId.ProtocolClassMouseNegativeText);
						break;
					case ProtocolClass.WholeBloodPositive:/*CR*/
						protocolType = SeparatorResourceManager.GetSeparatorString(StringId.ProtocolClassWholeBloodPositiveText);
						break;
					case ProtocolClass.WholeBloodNegative:
						protocolType = SeparatorResourceManager.GetSeparatorString(StringId.ProtocolClassWholeBloodNegativeText);
						break;
					case ProtocolClass.Maintenance:
						protocolType = SeparatorResourceManager.GetSeparatorString(StringId.ProtocolClassMaintenanceText);
						break;
					case ProtocolClass.Shutdown:
						protocolType = SeparatorResourceManager.GetSeparatorString(StringId.ProtocolClassShutdownText);
						break;
				}
				myChosenProtocolsTableInfo.Rows.Add(new object[]
					{
						(int)protocol.InitialQuadrant,
						protocol.Label,
						FormatFluidVolume(mySampleVolumes[(int)protocol.InitialQuadrant], true),																				
						protocolType,
						protocol.QuadrantCount.ToString()
					});
			}
					
			// Now update the display
			NotifyChosenProtocolTableUpdate(protocolMix);
		}

        #endregion Application support        

        //
        // ADDITION FOR EASE OF MODIFICATION
        //
        public ISeparationProtocol[] getSeparationProtocols()
        {
            return mySeparationProtocols;
        }

        public void refreshProtocolsList()
        {
        }
    }
}

