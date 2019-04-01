//----------------------------------------------------------------------------
// QuadrantManager
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
// Notes:
//    	03/24/05 - added parkarm() and islidclosed() - bdr
// 	04/11/05 - added parkpump to deenergize pump - bdr
//	03/29/06 - Short Description for Sample Volume Dialog - RL
//
//----------------------------------------------------------------------------

#define bdr11  // islidclosed()
#define bdr12  // park arm,pump


using System;
using System.Diagnostics;
using System.Collections;
using System.Globalization;

using Invetech.ApplicationLog;

using Tesla.InstrumentControl; 

using Tesla.Common;
using Tesla.Common.Protocol;
using Tesla.Common.Separator;
using System.Threading;

namespace Tesla.Separator
{
	/// <summary>
	/// Quadrant resource and state management for the carousel.
	/// </summary>
	/// <remarks>
	/// Singleton.
	/// </remarks>
	public class QuadrantManager : ISeparatorEvents
	{
        #region Constants

        // Define a threshold value against which to compare the batch end time spread.
        // Ideally this would be part of the configuration information provided by the 
        // instrument (e.g. similar to IInstrumentControl's GetContainerVolumes method),
        // but supposedly the value is unlikely to change.
        private const int   BATCH_END_TIME_SPREAD_THRESHOLD_MINUTES = 10;

        #endregion Constants

		private static QuadrantManager          theQuadrantManager;	// singleton instance variable
		private static SeparatorDataStore       theDataStore;		// in-memory database
		private static IInstrumentControlEvents theInstrumentControlEventSink;       

		// Storage for instrument configuration
		private static FluidVolume theBccmDeadVolume = new FluidVolume(25000, FluidVolumeUnit.MicroLitres);
		
		private InstrumentControlProxy		myInstrument;
		private InstrumentState				myInstrumentState;
		private Quadrant[]					myQuadrants;
		private SortedList					myChosenProtocols;
		private SeparatorState				myCarouselState;
		private bool                        myReadyToRun;
		private TimeSpan					myTimeRemaining;		
        private bool                        isHydraulicFluidRefillRequired;

		XmlRpcProtocol[] protocols;//CR
		private string[][] allCustomNames;
		private bool[][] allResultChecks;
		#region Construction/destruction

		public static QuadrantManager GetInstance()
		{
			if (theQuadrantManager == null)
			{
				try
				{
					theQuadrantManager = new QuadrantManager();
				}
				catch
				{
					theQuadrantManager = null;
				}
			}
			return theQuadrantManager;
		}

		private QuadrantManager()
		{
			// Reference the Instrument Control Proxy
			myInstrument = InstrumentControlProxy.GetInstance();
			// Start our tracking of the instrument state
			myInstrumentState = InstrumentState.RESET;

			// Initialise the quadrant resources
			try
			{
				myQuadrants = new Quadrant[(int)QuadrantId.NUM_QUADRANTS];
				for (int i = 0; i < myQuadrants.GetLength(0); ++i)
				{
					myQuadrants[i] = new Quadrant( (QuadrantId)i );
					// Listen for state change events
					myQuadrants[i].UpdateQuadrantState	+= 
						new Quadrant.QuadrantStateDelegate(FireQuadrantStateUpdate);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			// Allocate the batch time remaining 
			myTimeRemaining		= new TimeSpan();

			// Allocate the list of chosen Protocols
			myChosenProtocols	= new SortedList();

			// Clear batch-related state
			ResetBatchState();

			// "Connect" to our "database"
			theDataStore = SeparatorDataStore.GetInstance();

			// Create the InstrumentControlEventSink
			theInstrumentControlEventSink = SeparatorImpl.GetInstance().InstrumentControlEventSink;

			try
			{
				// Register for IInstrumentControlEvents
				theInstrumentControlEventSink.ReportRunComplete	+= new RunCompleteDelegate(QuadrantManager.AtRunComplete);
				theInstrumentControlEventSink.ReportETC			+= new ReportEstimatedTimeOfCompletionDelegate(QuadrantManager.AtReportETC);
				theInstrumentControlEventSink.ReportError		+= new Tesla.InstrumentControl.ReportErrorDelegate(QuadrantManager.AtReportError);
				theInstrumentControlEventSink.ReportStatus		+= new Tesla.InstrumentControl.ReportStatusDelegate(QuadrantManager.AtReportStatus);

				// Register with the proxy to get comms errors
				myInstrument.ReportError += new Tesla.InstrumentControl.ReportErrorDelegate(QuadrantManager.AtReportError);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		public void Connect()
		{
			// Connect to the instrument and retrieve the details of current instrument
            // configuration, including the list of supported protocols.

            // Query the hydraulic fluid level
            isHydraulicFluidRefillRequired = myInstrument.IsHydraulicFluidLow();

			// Get and store the details on the supported protocols
			protocols = myInstrument.GetProtocols();//CR
			allCustomNames = myInstrument.GetCustomNames();//CR
			allResultChecks = myInstrument.GetResultChecks();
			theDataStore.Clear();

			foreach (XmlRpcProtocol protocol in protocols)
			{				
				// Firstly, check that the "protocol class" field specifies a recognised type.
				ProtocolClass protocolClass = ProtocolClass.Maintenance;
				if ( ! Enum.IsDefined(typeof(ProtocolClass), protocol.protocolClass))					
				{					
					continue;
				}
				else
				{
					protocolClass = (ProtocolClass)Enum.Parse(typeof(ProtocolClass), 
						protocol.protocolClass);
				}

				// Store the common ("base class") protocol items
				SeparatorDataStore.ProtocolRow pRow = theDataStore.Protocol.NewProtocolRow();
				pRow.ProtocolInstrumentControlId	= protocol.ID;
				pRow.ProtocolLabel					= protocol.label;

				// RL - Short Description for Sample Volume Dialog - 03/29/06
				pRow.ProtocolDescription			= protocol.description;

                // Special case handling for "zero quadrant" protocols.  That is, if a protocol
                // such as a Maintenance or Shutdown protocol notionally requires no carousel
                // quadrant resources, we "force" the required quadrant count to 1 internally
                // since in practice we use quadrant usage as a determinant to whether more than
                // on protocol or protocol type can be run in the same batch.  The actual
                // resource allocation for each quadrant is calculated separated as part of
                // scheduling the protocol(s) and if no actual resources are required the
                // consumables list will empty (actually all quadrant resources will be listed
                // as "not required").
				pRow.ProtocolQuadrantCount			= protocol.numQuadrants > 0 ? protocol.numQuadrants : 1;				
				pRow.ProtocolUsageCount				= 0;
				pRow.ProtocolUsageTime				= DateTime.Now;
				string dbg = string.Format("Changing protocol time = {0}to(1)", pRow.ProtocolLabel,pRow.ProtocolUsageTime); 
				System.Diagnostics.Debug.WriteLine(dbg);

				// Store specialised ("derived class") protocol items according to the 
				// "protocol class" field.
				switch (protocolClass)
				{
					case ProtocolClass.Positive:
					case ProtocolClass.Negative:
					case ProtocolClass.HumanPositive:
					case ProtocolClass.HumanNegative:
					case ProtocolClass.MousePositive:
					case ProtocolClass.MouseNegative:
					case ProtocolClass.WholeBloodPositive:
					case ProtocolClass.WholeBloodNegative:
						SeparatorDataStore.SeparationProtocolRow spRow = theDataStore.SeparationProtocol.NewSeparationProtocolRow();
						spRow.SeparationProtocolProtocolId	  = pRow.ProtocolId;
						spRow.SeparationProtocolMinimumVolume = protocol.minVol;
						spRow.SeparationProtocolMaximumVolume = protocol.maxVol;
						spRow.SeparationProtocolClass		  = protocol.protocolClass;
						theDataStore.Protocol.AddProtocolRow(pRow);
						theDataStore.SeparationProtocol.AddSeparationProtocolRow(spRow);
						break;
					case ProtocolClass.Maintenance:
						SeparatorDataStore.MaintenanceProtocolRow mpRow = theDataStore.MaintenanceProtocol.NewMaintenanceProtocolRow();                    
						mpRow.MaintenanceProtocolProtocolId = pRow.ProtocolId;
						mpRow.MaintenanceProtocolTaskId     = 0;				
						mpRow.MaintenanceProtocolTaskDescription = string.Empty;    // NOTE: description not currently used
						theDataStore.Protocol.AddProtocolRow(pRow);
						theDataStore.MaintenanceProtocol.AddMaintenanceProtocolRow(mpRow);
						break;
					case ProtocolClass.Shutdown:
						SeparatorDataStore.ShutdownProtocolRow shutRow = theDataStore.ShutdownProtocol.NewShutdownProtocolRow();						
						shutRow.ShutdownProtocolProtocolId = pRow.ProtocolId;
						shutRow.ShutdownProtocolDescription = string.Empty;	// NOTE: description not currently used
						theDataStore.Protocol.AddProtocolRow(pRow);
						theDataStore.ShutdownProtocol.AddShutdownProtocolRow(shutRow);
						break;
				}
			}

            // Update the protocol MRU information.
            theDataStore.RetrieveMRU_Statistics();
		}

		#endregion Construction/destruction
/*CR*/	public void GetCustomNames(string name, out string[] customNames, out int numQuadrants)
		{
			int position = 0;
			numQuadrants=0;
			foreach (XmlRpcProtocol protocol in protocols)
			{
				numQuadrants = protocol.numQuadrants;
				if (protocol.label == name)
					break;
				position++;
			}

			customNames = new string[8*numQuadrants];
			
			for(int i=0, j=0; i<numQuadrants; i++)
			{
				j=i;
				if(allCustomNames[position].Length<=(j*8))
					j=0;
				customNames[0+(8*i)] = allCustomNames[position][0+(8*j)];
				customNames[1+(8*i)] = allCustomNames[position][1+(8*j)];
				customNames[2+(8*i)] = allCustomNames[position][2+(8*j)];
				customNames[3+(8*i)] = allCustomNames[position][3+(8*j)];
				customNames[4+(8*i)] = allCustomNames[position][4+(8*j)];
				customNames[5+(8*i)] = allCustomNames[position][5+(8*j)];
				customNames[6+(8*i)] = allCustomNames[position][6+(8*j)];
				customNames[7+(8*i)] = allCustomNames[position][7+(8*j)];
			}
		}

		public void GetResultVialSelection(string name, out bool[] resultChecks, out int numQuadrants)
		{
			int position = 0;
			numQuadrants=0;
			foreach (XmlRpcProtocol protocol in protocols)
			{
				numQuadrants = protocol.numQuadrants;
				if (protocol.label == name)
					break;
				position++;
			}

			resultChecks = new bool[8*numQuadrants];
			
			for(int i=0, j=0; i<numQuadrants; i++)
			{
				j=i;
				if(allResultChecks[position].Length<=(j*8))
					j=0;
				resultChecks[0+(8*i)] = allResultChecks[position][0+(8*j)];
				resultChecks[1+(8*i)] = allResultChecks[position][1+(8*j)];
				resultChecks[2+(8*i)] = allResultChecks[position][2+(8*j)];
				resultChecks[3+(8*i)] = allResultChecks[position][3+(8*j)];
				resultChecks[4+(8*i)] = allResultChecks[position][4+(8*j)];
				resultChecks[5+(8*i)] = allResultChecks[position][5+(8*j)];
				resultChecks[6+(8*i)] = allResultChecks[position][6+(8*j)];
				resultChecks[7+(8*i)] = allResultChecks[position][7+(8*j)];
			}
		}

		#region Quadrant services

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// Handles determination of SeparatorState prior to achieving SeparatorState.Ready,
		/// after which SeparatorState changes are determined by changes in InstrumentState
		/// (which are notified via IInstrumentControlEvents).
		/// </remarks>
		private void RecalculateCarouselState()
		{
            if (myCarouselState != SeparatorState.Initialising)
            {               
                lock(this)
                {
                    SeparatorState  newState = SeparatorState.SeparatorUnloaded;
                    bool readyToRun = true;
				
                    if (myCarouselState >= SeparatorState.SeparatorUnloaded &&
                        myCarouselState <= SeparatorState.Ready)
                    {
                        foreach (Quadrant q in myQuadrants)
                        {
                            SeparatorState quadrantState = q.State;

                            // If all Quadrants are in either Unloaded or Configured state, we 
                            // are ready to schedule the run 
                            readyToRun &=  (quadrantState == SeparatorState.SeparatorUnloaded ||
                                quadrantState == SeparatorState.Configured);

                            // Find the "lowest common denominator (LCD)" state for all 
                            // quadrants.
                            // This will be "Unloaded" if no quadrants are in use.  Otherwise,
                            // it is the LCD state greater than "Unloaded".
                            if (newState == SeparatorState.SeparatorUnloaded &&
                                newState < quadrantState)
                            {
                                newState = quadrantState;
                            }
                            else if (quadrantState > SeparatorState.SeparatorUnloaded &&
                                newState > quadrantState)
                            {
                                newState = quadrantState;
                            }
                        }

                        // Test if we're ready to run                
                        if (newState == SeparatorState.Configured && readyToRun)
                        {
                            // Recalculate the estimated batch duration 
                            myReadyToRun = true;
                            RecalculateAndUpdateScheduleEstimate();
                            if (myTimeRemaining > TimeSpan.Zero)
                            {
                                newState = SeparatorState.Ready;
                            }
                        }
                        else
                        {
                            // We're not actually ready to run unless all quadrants used by 
                            // protocols are in "Configured" state
                            readyToRun = myReadyToRun = false; 						
                            RecalculateAndUpdateScheduleEstimate();
                        }
                    }				

                    // If the carousel state has changed, record the change and alert 
                    // interested parties
                    if (newState != myCarouselState && 
                        newState <= SeparatorState.Running)
                    {
                        UpdateCarouselState(newState);               
                    }				
                }
            }
		}

		public void ResetBatchState()
		{
			try
			{
				lock(this)
				{                
					// Clear the list of selected Protocols and inform interested parties
					myChosenProtocols.Clear();
					FireChosenProtocolsUpdate();

					// Clear the Quadrant state
					for(QuadrantId q = QuadrantId.Quadrant1;
						q < QuadrantId.NUM_QUADRANTS; ++q)
					{
						myQuadrants[(int)q].Clear();
					}

					// Set initial carousel state
					RecalculateCarouselState();

					// Clear the batch time remaining state.  Note we do this after recalculating
					// the carousel state as doing so has a side-effect with respect to the display 
					// of the "time remaining" on the UI.  (That is, a zero "time remaining" is 
					// shown if a running protocol has finished, as a confirmation the run has 
					// terminated normally -- otherwise, a zero "time remaining" is not considered 
					// useful information and no time display is shown.)
					myTimeRemaining	= TimeSpan.Zero;
					FireBatchDurationUpdate();

                    // Refresh the hydraulic fluid level status
                    bool isHydraulicFluidLow = myInstrument.IsHydraulicFluidLow();
                    if (isHydraulicFluidRefillRequired != isHydraulicFluidLow)
                    {
                        isHydraulicFluidRefillRequired = isHydraulicFluidLow;
                        FireHydraulicFluidRefillStatusUpdate();
                    }
				}
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}

		#if bdr11
        public bool IsLidClosed() // bdr
        {
			bool isLidClosed = false;
            try
            {
                isLidClosed = myInstrument.IsLidClosed();
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
			}
		return isLidClosed;            
        }
		#endif

        public bool IsHydroFluidLow()
        {
            bool rtn = false;

            try
            {
                rtn = myInstrument.IsHydraulicFluidLow();
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
				state = myInstrument.GetInstrumentState();
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
				isPauseCommand = myInstrument.isPauseCommand();
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
			return isPauseCommand;
		}
        public int SetIgnoreLidSensor(int sw)
        {
            int rtn = -1;
            try
            {
                rtn = myInstrument.SetIgnoreLidSensor(sw);
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
            return rtn;

        }
        public bool GetIgnoreLidSensor()
        {
            bool rtn = false;
            try
            {
                rtn = myInstrument.GetIgnoreLidSensor();
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
            return rtn;

        }
        public int SetIgnoreHydraulicSensor(int sw)
        {
            int rtn = -1;
            try
            {
                rtn = myInstrument.SetIgnoreHydraulicSensor(sw);
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
            return rtn;

        }
        public bool GetIgnoreHydraulicSensor()
        {
            bool rtn = false;
            try
            {
                rtn = myInstrument.GetIgnoreHydraulicSensor();
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
            return rtn;

        }

        public int GetCurrentID()                     //CWJ
        {
            int rtn = -1;
            try
            {
                rtn = myInstrument.GetCurrentID();
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
            return rtn;
        }
        public int GetCurrentSec()                     //CWJ
        {
            int rtn = -1;
            try
            {
                rtn = myInstrument.GetCurrentSec();
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
            return rtn;
        }
        public int GetCurrentSeq()                     //CWJ
        {
            int rtn = -1;
            try
            {
                rtn = myInstrument.GetCurrentSeq();
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
            return rtn;
        }

        public bool Ping()                     //CWJ
        {
            bool rtn = false; 
            try
            {
                rtn = myInstrument.Ping();
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
            return rtn;
        }

        public int InitScanVialBarcode(bool freeaxis)                     //CWJ
        {
            int rtn = -1;
            try
            {
                rtn = myInstrument.InitScanVialBarcode(freeaxis);
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
            return rtn;
        }

        public bool AbortScanVialBarcode()                     //twong
        {
            bool rtn = false;
            try
            {
                rtn = myInstrument.AbortScanVialBarcode();
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
            return rtn;
        }

        public bool IsBarcodeScanningDone()                     //twong
        {
            bool rtn = false;
            try
            {
                rtn = myInstrument.IsBarcodeScanningDone();
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
            return rtn;
        }

        

        public int ScanVialBarcodeAt(int Quadrant, int Vial) //CWJ
        {
            int rtn = 0;
            try
            {
                rtn = myInstrument.ScanVialBarcodeAt(Quadrant, Vial);
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
            return rtn;
        }

        public string GetVialBarcodeAt(int Quadrant, int Vial) //CWJ
        {
            string rtn = "Error";//Scanning, Moving, Barcode....
            try
            {
                rtn = myInstrument.GetVialBarcodeAt(Quadrant, Vial);
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
            return rtn;
        }

        public string GetEndOfRunXMLFullFileName() // 09-25-2013 - Sunny added
        {
            string rtn = "Error";
            try
            {
                rtn = myInstrument.GetEndOfRunXMLFullFileName();
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
            return rtn;
        }


        public string getPauseCommandCaption() 
		{
			string pauseCommandCaption="";
			try
			{
				pauseCommandCaption = myInstrument.getPauseCommandCaption();
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
                myInstrument.ConfirmHydraulicFluidRefilled();
                isHydraulicFluidRefillRequired = false;
                FireHydraulicFluidRefillStatusUpdate();
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
        }

		public void SetSampleVolume(QuadrantId quadrantId, FluidVolume volume)
		{

            System.Diagnostics.Debug.WriteLine(String.Format("QuadrantManager: SetSampleVolume called. QuadrantId={0}, Volume={1} ", quadrantId, volume.Amount.ToString()));

            try
            {
                // Ensure the specified quadrant ID is suitable for accepting a sample volume.
                if (quadrantId < QuadrantId.Quadrant1 || quadrantId > QuadrantId.Quadrant4)
                {
                    throw new ApplicationException(string.Format("QuadrantID {0} out of range", quadrantId));
                }

                // Ensure that the specified quadrant is the initial quadrant for this protocol.
                // That is, the sample is assumed to be only ever loaded in the initial
                // quadrant.
                if (!myQuadrants[(int)quadrantId].IsProtocolInitialQuadrant)
                {
                    throw new ApplicationException(string.Format("QuadrantID {0} is not the initial quadrant for ProtocolID {1}",
                        quadrantId, myQuadrants[(int)quadrantId].ProtocolId));
                }

                // Check the supplied volume is within the range allowed for by the protocol.
                // (Note: this should have been done already as a client-side (GUI) check, but 
                // double-check anyway in case we have future clients with different 
                // implementations, for example, scripting interfaces).

                //there's a race condition where myChosenProtocols is empty still after calling mySeparator.SelectProtocol((QuadrantId)(Q), selectedProtocol);
                //so...

                ISeparationProtocol protocol = myChosenProtocols[quadrantId] as ISeparationProtocol;
                
                if (protocol != null)
                {
                    if (volume.Amount < protocol.MinimumSampleVolume ||
                        volume.Amount > protocol.MaximumSampleVolume)
                    {
                        System.Diagnostics.Debug.WriteLine(String.Format("Throw exception because volume is not within range."));

                        throw new SeparatorException(StatusCode.TSC2000,
                            new string[]{volume.Amount.ToString(), 
						protocol.MinimumSampleVolume.ToString(), 
							protocol.MaximumSampleVolume.ToString()});
                    }
                }
                if (protocol == null)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("myChosenProtocols: QuadrantId={0}, return protocol=null", quadrantId));
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("CalculateAndUpdateQuadrantConsumables: QuadrantId={0}, protocol ID = {1}).", quadrantId, protocol.Id));
                }

                // Recalculate the consumables required for this protocol and sample volume
                CalculateAndUpdateQuadrantConsumables(myQuadrants[(int)quadrantId].ProtocolId,
                    quadrantId, volume);

                // Reschedule the batch to take account of the new sample volume.
                ScheduleRun(quadrantId);
            }
            catch (SeparatorException sepex)
            {
                if (ReportStatus != null)
                {
                    ReportStatus(sepex.StatusCode.ToString(), sepex.StatusMessageParameters);
                }
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }

			return;			
		}

		private void CalculateAndUpdateQuadrantConsumables(int protocolId,
			QuadrantId initialQuadrantId, 
			FluidVolume sampleVolume)
		{	
			// Get the InstrumentControl Protocol Id for this protocol			
			int icProtocolId = theDataStore.GetInstrumentControlProtocolId(protocolId);
            System.Diagnostics.Debug.WriteLine(String.Format("---------theDataStore.GetInstrumentControlProtocolId return icProtocolId = {0}", icProtocolId));

				
			// Get the expected number of quadrants used by this protocol
			int quadrantCount = theDataStore.GetProtocolQuadrantCount(protocolId);   
							
            Quadrant initialQuadrant = myQuadrants[(int)initialQuadrantId];	   
            // NOTE: for future revisions, consider defining a FluidVolume.Zero constant for the following.
            initialQuadrant.SampleTube.IsRequired = (sampleVolume.Amount > 0.0d);
            if (quadrantCount <= 0)
            {
                // Special case for "zero quadrant" protocols.
                // We are forcing at least one quadrant to be 'required' as the resource 
                // calculation below will want to always include a waste vial (for example) in 
                // the required resources.
                quadrantCount = 1;
            }

			// Initialise a 'null' set of resources for this protocol
			ProtocolConsumable[,] protocolConsumables = 
				new ProtocolConsumable[quadrantCount, (int)RelativeQuadrantLocation.NUM_RELATIVE_QUADRANT_LOCATIONS];

			for (int relativeQuadrant = 0; relativeQuadrant < quadrantCount; ++relativeQuadrant)
			{
				for (RelativeQuadrantLocation location = RelativeQuadrantLocation.START_LOCATION;
					location < RelativeQuadrantLocation.NUM_RELATIVE_QUADRANT_LOCATIONS; ++location)
				{
					protocolConsumables[relativeQuadrant, (int)location].IsRequired = false;					
				}
			}			         

            // Update the sample volume information, if appropriate.  
            // Strictly speaking, we calculate so-called "consumables" based on the sample 
            // volume and so the sample tube volume is not a consumable.  However, it seems 
            // useful to include the sample tube as just another "Receptacle" in the list of 
            // consumables, since:
            // (1) it seems likely that in a future revision we will need to allow the user
            //     to input the actual volume entered
            // (2) it's probably better to update all quadrant resource information from
            //     the one source, even though sample volume is the independent variable and
            //	   the other resources are calculated based on that.	

			// Update the sample volume working value (this is not 'finally' recorded --
			// that is, in a sample record entry -- until the run is started, in other words
			// there is no restriction on the user changing their mind and entering a 
			// different sample volume, but we don't bother to record such 'tentative'
			// changes in a sample record, we just keep it as quadrant state for now.)			
			initialQuadrant.SampleTube.Volume = sampleVolume;

			// Force the quadrant's state to update
			initialQuadrant.UpdateState(true);

			// Create a copy of the information to send in the consumables update event...

			// Add the sample information
			int sampleQuadrantIndex = 0;	// Sample is always at zeroth relative quadrant 
            if (quadrantCount > 0)
            {
                protocolConsumables[sampleQuadrantIndex, 
                    (int)RelativeQuadrantLocation.SampleTube].IsRequired	= 
                    initialQuadrant.SampleTube.IsRequired;

                protocolConsumables[sampleQuadrantIndex, 
                    (int)RelativeQuadrantLocation.SampleTube].Capacity		= 
                    initialQuadrant.SampleTube.Capacity;

                protocolConsumables[sampleQuadrantIndex,
                    (int)RelativeQuadrantLocation.SampleTube].Volume		= sampleVolume;
            }

			// Calculate the variable consumables required for this protocol and sample volume.
			// NOTE: the convention is that a -ve sample volume is specified for all 
			// "non-Separation" protocols (e.g. Maintenance and Shutdown protocol 'classes').
			Hashtable[]	consumables;		
			myInstrument.CalculateConsumables(
				icProtocolId, 
				sampleVolume.Amount,	
				out consumables);

			int numberOfConsumablesQuadrants = consumables.GetLength(0);
			
			// Allocate space to collate the actual quadrant IDs used by this protocol
			QuadrantId[] actualQuadrantIds = new QuadrantId[quadrantCount];
			for (int q = 0; q < quadrantCount; ++q)
			{
				actualQuadrantIds[q] = QuadrantId.NoQuadrant;
			}		
			
			// Check for the special case where there may be no further consumables for a 
			// separation protocol (that is, other than the sample itself).
			if (numberOfConsumablesQuadrants == 0 && initialQuadrant.SampleTube.IsRequired)
			{
				actualQuadrantIds[0] = initialQuadrantId;
			}
			else if (quadrantCount == numberOfConsumablesQuadrants)			
			{
				// Aside from the special case above (handled by the 'if' clause), the 
				// expected and calculated quadrant counts should be the same.				
				Debug.Assert(quadrantCount == numberOfConsumablesQuadrants, 
					string.Format("Expected {0} quadrants with consumables but calculated {1} quadrants", 
					quadrantCount, numberOfConsumablesQuadrants));

				// Store consumable values in their respective QuadrantResource. 
				// NOTE: this is where we perform the mapping between the "relative quadrant ID"
				// returned from the 'CalculateConsumables' call and the actual quadrant ID(s).		
				QuadrantId quadrantId = initialQuadrantId;
				for (int relativeQuadrant = 0; relativeQuadrant < quadrantCount; ++relativeQuadrant)
				{
					// Record the actual quadrant Id where the consumables are to be configured
					actualQuadrantIds[relativeQuadrant] = quadrantId;

					// Update the records of consumables for this quadrant
					IDictionaryEnumerator aEnumerator = consumables[relativeQuadrant].GetEnumerator();
					while (aEnumerator.MoveNext())
					{						
						FluidVolume fluidVolume = (FluidVolume)aEnumerator.Value;
						bool isRequired = fluidVolume.Amount >= 0.0; 
						QuadrantResource aResource = null;
						RelativeQuadrantLocation location = (RelativeQuadrantLocation)aEnumerator.Key;

 						switch (location)
						{							
							case RelativeQuadrantLocation.SampleTube:
								// If the supplied consumables do not specify the sample volume then
								// revert to the value entered by the user (should be the same in the normal
								// course of events).
								if (relativeQuadrant == 0 && fluidVolume.Amount == 0.0d)
								{
									// Default to the sample volume entered by the user
									fluidVolume.Amount = sampleVolume.Amount;									
								}
								aResource = myQuadrants[(int)quadrantId].SampleTube;								
								break;							
							case RelativeQuadrantLocation.VialA:
								aResource = myQuadrants[(int)quadrantId].VialA;
								break;
							case RelativeQuadrantLocation.VialB:
								aResource = myQuadrants[(int)quadrantId].VialB;
								break;
							case RelativeQuadrantLocation.VialC:
								aResource = myQuadrants[(int)quadrantId].VialC;
								break;
							case RelativeQuadrantLocation.SeparationTube:                            
								aResource = myQuadrants[(int)quadrantId].SeparationTube;
								break;  
							case RelativeQuadrantLocation.WasteTube:
								aResource = myQuadrants[(int)quadrantId].WasteTube;
								break;
							case RelativeQuadrantLocation.LysisBufferTube:
								aResource = myQuadrants[(int)quadrantId].LysisBufferTube;
								break;							
							case RelativeQuadrantLocation.TipsBox:
								aResource = myQuadrants[(int)quadrantId].TipsBox;
								break;
							case RelativeQuadrantLocation.QuadrantBuffer:
								aResource = myQuadrants[(int)quadrantId].QuadrantBuffer;
								break;
						}

						if (aResource != null)
						{
							// Update the actual Quadrant Resource's data, and a copy to send 
							// in the update event							
							aResource.IsRequired = isRequired;
							protocolConsumables[relativeQuadrant, (int)location].IsRequired	= 
								isRequired;

							// Check if the Quadrant Resource is also a Receptacle
							Receptacle aReceptacle = aResource as Receptacle;
							if (aReceptacle != null)
							{
								// Update the actual Receptacle's data, and a copy to send
								// in the update event								
								aReceptacle.Volume   = fluidVolume;							
								protocolConsumables[relativeQuadrant, (int)location].Capacity = 
									aReceptacle.Capacity;
								protocolConsumables[relativeQuadrant, (int)location].Volume	= 
									fluidVolume;								
							}
						}
					}
				                
					// Update the quadrant reference in case the required resources
					// extend beyond the initial quadrant.
					quadrantId = (QuadrantId)
						((int)++quadrantId % (int)QuadrantId.NUM_QUADRANTS);               
				}
			}

			// Update the consumables display
			FireConsumablesUpdate(protocolId, actualQuadrantIds, protocolConsumables);
		}

		private void ClearAndUpdateQuadrantConsumables(int protocolId,
			QuadrantId initialQuadrantId)
		{								
			FluidVolume zeroVolume = new FluidVolume(0.0d, FluidVolumeUnit.MicroLitres);

            System.Diagnostics.Debug.WriteLine(String.Format("---------ClearAndUpdateQuadrantConsumables called. protocolId={0}, initialQuadrantId={1}.", protocolId, initialQuadrantId));


			// Get the expected number of quadrants used by this protocol
			int quadrantCount = theDataStore.GetProtocolQuadrantCount(protocolId);

			// Allocate space to collate the actual quadrant IDs used by this protocol			
			QuadrantId[] actualQuadrantIds = new QuadrantId[quadrantCount];
			QuadrantId quadrantId = initialQuadrantId;
			for (int relativeQuadrant = 0; relativeQuadrant < quadrantCount; ++relativeQuadrant)
			{
				// Record the actual quadrant Id where the consumables are to be configured
				actualQuadrantIds[relativeQuadrant] = quadrantId;

				// Update the quadrant reference in case the required resources
				// extend beyond the initial quadrant.
				quadrantId = (QuadrantId)
					((int)++quadrantId % (int)QuadrantId.NUM_QUADRANTS);
			}
			
			// Update the consumables display -- we pass 'null' for the list of consumables,
			// to indicate that we are clearing the consumables resources for this 
			// protocol's quadrant(s).			
			FireConsumablesUpdate(protocolId, actualQuadrantIds, null);
		}

		public bool SelectProtocol(QuadrantId initialQuadrant, IProtocol protocol)
		{
            System.Diagnostics.Debug.WriteLine(String.Format("QuadrantManager: SelectProtocol called."));
 
            if (protocol == null)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("QuadrantManager: SelectProtocol called. input parameter protocol=null"));
                return false;
            }

			try
			{
				lock(this)
				{             				
					// Determine the quadrant "footprint" for the specified protocol (that is,
					// which actual quadrants are required for its use for this new selection)
					QuadrantId finalQuadrant;
					try
					{
						finalQuadrant = (QuadrantId)((int)initialQuadrant + 
							(protocol.QuadrantCount - 1));

                        System.Diagnostics.Debug.WriteLine(String.Format("QuadrantManager1: finalQuadrant={0}", finalQuadrant));

                        if (finalQuadrant < initialQuadrant)
                        {
                            // Special case processing for "zero quadrant count" protocols,
                            // for example, maintenance protocols.
                            finalQuadrant = initialQuadrant;
                        }

                        System.Diagnostics.Debug.WriteLine(String.Format("QuadrantManager2: finalQuadrant={0}", finalQuadrant));

					}
					catch
					{
						// NOTE: future versions should throw a suitable exception, as we have 
                        // lost protocol data integrity at this point!

                        System.Diagnostics.Debug.WriteLine(String.Format("QuadrantManager3: Exception"));
						return false;
					}

					// NOTE: at this point we need to deal with quadrant 'already in use' 
                    // clashes, including the case where the newly selected protocol has a 
                    // different quadrantCount to the original.  
					// For now, selection of a Maintenance protocol and Separation protocol is
					// not allowed (they're treated as mutually exclusive).
					bool isShutdownInitiated = false;
					ArrayList protocolsToDeselect = new ArrayList();
					if (protocol as IMaintenanceProtocol != null)
					{
                        System.Diagnostics.Debug.WriteLine(String.Format("QuadrantManager5: protocol as IMaintenanceProtocol"));

						// Check if there are any Separation Protocols already selected, and if so,
						// deselect them.
						foreach (IProtocol chosenProtocol in myChosenProtocols.Values)
						{
							ISeparationProtocol separationProtocol = 
								chosenProtocol as ISeparationProtocol;
							if (separationProtocol != null)
							{
								protocolsToDeselect.Add(separationProtocol);
							}
						}					

                        // Check if there are any Maintenance protocols already selected, and if so,
                        // deselect them.
                        foreach (IProtocol chosenProtocol in myChosenProtocols.Values)
                        {
                            IMaintenanceProtocol maintenanceProtocol = 
                                chosenProtocol as IMaintenanceProtocol;
                            if (maintenanceProtocol != null)
                            {
                                protocolsToDeselect.Add(maintenanceProtocol);
                            }
                        }
					}
					else if (protocol as ISeparationProtocol != null)
					{
                        System.Diagnostics.Debug.WriteLine(String.Format("QuadrantManager6: protocol as ISeparationProtocol"));

						// Check if there are any Maintenance protocols already selected, and if so,
						// deselect them.
						foreach (IProtocol chosenProtocol in myChosenProtocols.Values)
						{
							IMaintenanceProtocol maintenanceProtocol = 
								chosenProtocol as IMaintenanceProtocol;
							if (maintenanceProtocol != null)
							{
								protocolsToDeselect.Add(maintenanceProtocol);
							}
						}
					}				
					else if (protocol as IShutdownProtocol != null)
					{
                        System.Diagnostics.Debug.WriteLine(String.Format("QuadrantManager7: protocol as IShutdownProtocol"));

						isShutdownInitiated = true;
						foreach (IProtocol chosenProtocol in myChosenProtocols.Values)
						{
							protocolsToDeselect.Add(chosenProtocol);
						}
					}

					if (protocolsToDeselect.Count > 0)
					{
						foreach (IProtocol protocolToDeselect in protocolsToDeselect)
						{
							// Deselect the specified protocol.  
							// In the special case of a Shutdown Protocol, we suppress notification 
							// of the change as we're about to add/update the list of chosen 
							// protocols (supressing the update due to deselection is more efficient and
							// simplifies handling in the GUI).  This complication arises because
							// when a Shutdown Protocol is selected, it is immediately run by the 
							// Instrument Control layer (that is, the Instrument Control layer 
							// triggers the run).
							if (isShutdownInitiated)
							{
								DeselectProtocol_ChangeNotificationRequired(
									protocolToDeselect.InitialQuadrant, false);
							}
							else
							{
                                System.Diagnostics.Debug.WriteLine(String.Format("--------- About to call DeselectProtocol. protocolToDeselect.InitialQuadrant={0}", protocolToDeselect.InitialQuadrant));
								DeselectProtocol(protocolToDeselect.InitialQuadrant);
							}
						}
					}

					// Record a new initialQuadrant/protocol association for all quadrants
					// used by this protocol
					for (QuadrantId q = initialQuadrant; q <= finalQuadrant; ++q)
					{
						Quadrant quadrant = myQuadrants[(int)q % (int)QuadrantId.NUM_QUADRANTS];
						quadrant.ProtocolId = protocol.Id;
						quadrant.IsProtocolInitialQuadrant = (q == initialQuadrant);

                        System.Diagnostics.Debug.WriteLine(String.Format("QuadrantManager10: quadrant.ProtocolId={0}", quadrant.ProtocolId));
					}

 					// Record this protocol as selected (updating existing association if 
					// required).               
					protocol.InitialQuadrant = initialQuadrant;	// update the protocol's own association
					if (myChosenProtocols.Contains(initialQuadrant))
					{				
						// Update an existing selected protocol
						myChosenProtocols[initialQuadrant] = protocol;
                        System.Diagnostics.Debug.WriteLine(String.Format("QuadrantManager15: myChosenProtocols[{0}]={1}", initialQuadrant, protocol.Label));
					}
					else
					{
						// Add a newly selected protocol
						myChosenProtocols.Add(initialQuadrant, protocol);
                        System.Diagnostics.Debug.WriteLine(String.Format("QuadrantManager20: myChosenProtocols.Add({0}, {1})", initialQuadrant, protocol.Label));
					}                

					// Signal the selection change to interested parties
					FireChosenProtocolsUpdate();

					// Record the state change, if any, for this quadrant
					if ((protocol as ISeparationProtocol) != null)
					{
						// Normal protocol
					}
					else if ((protocol as IMaintenanceProtocol) != null)
					{						
						// Recalculate the consumables required for this protocol (pass a zero
						// sample volume since there is no sample volume required with a Maintenance protocol).	
                        myQuadrants[(int)initialQuadrant].ProtocolId   = protocol.Id;
						CalculateAndUpdateQuadrantConsumables(protocol.Id, 
							initialQuadrant, new FluidVolume(0 ,FluidVolumeUnit.MicroLitres));               
					}
					else if ((protocol as IShutdownProtocol) != null)
					{
						// When selected, Shutdown protocols are run immediately by control
						// layer.  For our purposes, set the quadrant state to "Configured", so
						// we're ready to receive the "shutting down" indication.
                        myQuadrants[(int)initialQuadrant].ProtocolId   = protocol.Id;
						myQuadrants[(int)initialQuadrant].IsConfigured = true;                        
						// Recalculate the consumables required for this protocol (pass an
						// arbitary positive sample volume).	
						CalculateAndUpdateQuadrantConsumables(protocol.Id, 
							initialQuadrant, new FluidVolume(1, FluidVolumeUnit.MicroLitres));
					}
					else
					{
						LogFile.AddMessage(TraceLevel.Error, "SelectProtocol - unknown/invalid protocol class");
					}
				}
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
                return false;
			}
            return false;
		}

		/// <summary>
		/// Deselect the protocol with the specified initial quadrant
		/// </summary>
		/// <param name="initialQuadrant"></param>
		/// <remarks>
		/// This is a wrapper function that represents the "usual case" processing for 
		/// deselecting a protocol.
		/// </remarks>
		public void DeselectProtocol(QuadrantId initialQuadrant)
		{
			try
			{
				DeselectProtocol_ChangeNotificationRequired(initialQuadrant, true);
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}

		/// <summary>
		/// Deselect a protocol, optionally supressing quadrant state change notification and
		/// notification of the change in the list of chosen protocols.
		/// </summary>
		/// <param name="initialQuadrant"></param>
		/// <param name="isChangeNotificationRequired"></param>
		private void DeselectProtocol_ChangeNotificationRequired(QuadrantId initialQuadrant, 
			bool isChangeNotificationRequired)
		{
			lock(this)
			{    				
   				// Integrity check -- ensure we've been called with details of a previously
				// selected protocol
				if ( ! myChosenProtocols.Contains(initialQuadrant))
				{		
					// We've been asked to deselect the protocol associated with a quadrant
					// for which there is no currently selected protocol -- so, nothing to do.
					return;
				}       

				// Determine the quadrant "footprint" for the specified protocol (that is,
				// which actual quadrants are required for its use for this new selection)
				QuadrantId	finalQuadrant;
				IProtocol	protocol = null;				
				try
				{
					protocol = (IProtocol)myChosenProtocols[initialQuadrant];
                    if (protocol == null)
                    {
                        return;
                    }
					finalQuadrant = (QuadrantId)((int)initialQuadrant + 
						protocol.QuadrantCount - 1);
                    if (finalQuadrant < initialQuadrant)
                    {
                        // Special case processing for "zero quadrant count" protocols,
                        // for example, maintenance protocols.
                        finalQuadrant = initialQuadrant;
                    }
				}
				catch
				{
                    // NOTE: future versions should throw a suitable exception, as we have 
                    // lost protocol data integrity at this point!
                    System.Diagnostics.Debug.WriteLine(String.Format("DeselectProtocol_ChangeNotificationRequired Exception. initialQuadrant={0}.", initialQuadrant));
					return;
				}

                System.Diagnostics.Debug.WriteLine(String.Format("---------DeselectProtocol_ChangeNotificationRequired: About to call ClearAndUpdateQuadrantConsumables. protocolId={0}, initialQuadrant={1}.", protocol.Id, initialQuadrant));

				// Clear previously calculated consumables and update interested parties
				ClearAndUpdateQuadrantConsumables(protocol.Id, initialQuadrant);

                System.Diagnostics.Debug.WriteLine(String.Format("---------DeselectProtocol_ChangeNotificationRequired: myChosenProtocols.Remove({0}).", initialQuadrant));

				// Remove this protocol from the list of chosen protocols
				myChosenProtocols.Remove(initialQuadrant);   

				// Remove the initialQuadrant/protocol association for all quadrants
				// used by this protocol and update the protocol state accordingly.
				// (Note that changing the quadrant state will also recalculate the estimated
				// batch duration as a side-effect.  As such, the list of chosen protocols
				// (held by myChosenProtocols) must have been updated before this recalculation,
				// as is done above.)
				for (QuadrantId quadrantId = initialQuadrant; quadrantId <= finalQuadrant; 
					++quadrantId)
				{
					Quadrant quadrant = myQuadrants[(int)quadrantId];
					quadrant.Clear(isChangeNotificationRequired);
				}				          				

				// Signal the change in the chosen protocols to interested parties, if required
				if (isChangeNotificationRequired)
				{
                    System.Diagnostics.Debug.WriteLine(String.Format("---------DeselectProtocol_ChangeNotificationRequired: about to call FireChosenProtocolsUpdate"));

					FireChosenProtocolsUpdate();								
				}
			}
		}

		private void PrepareScheduleParameters(bool isActualRun, 
			out IProtocol[] chosenProtocols, out int[] instrumentControlProtocolIDs, 
			out int[] sampleIDs, out double[] sampleVolumes)
		{
			// Return the lists of sample volumes, sample IDs, Instrument Control protocol IDs,
			// and the actual chosen protocols.
			
			// If we're called as part of an actual run, then create real sample records
			// and return their IDs, otherwise we're called as part of a schedule estimate
			// so return dummy sample IDs (that is, we don't treat sample volumes as 
			// "confirmed" until the StartRun call -- this allows the user to change their
			// mind about the sample volume or even deselect a protocol, without us having
			// to then create and delete a sample record).
			int chosenProtocolsCount		= myChosenProtocols.Count;
			chosenProtocols					= new IProtocol[chosenProtocolsCount];
			instrumentControlProtocolIDs	= new int[chosenProtocolsCount];
			sampleIDs						= new int[chosenProtocolsCount];
			sampleVolumes					= new double[chosenProtocolsCount];

            currentSampleIDs = sampleIDs;

			int protocolIndex = 0;
			myChosenProtocols.Values.CopyTo(chosenProtocols, 0);
			foreach(IProtocol protocol in chosenProtocols)
			{
				// Get the Instrument Control protocol ID for the protocol.
				int icID = theDataStore.GetInstrumentControlProtocolId(protocol.Id);
				instrumentControlProtocolIDs[protocolIndex] = icID;				
											
				// Get the tentative/confirmed sample volume (logically tentative if this 
				// routine is called as part of a schedule estimate calculation, and
				// logically confirmed if called as part of an actual run).				
				sampleVolumes[protocolIndex] = 
					myQuadrants[(int)protocol.InitialQuadrant].SampleTube.Volume.Amount;

				// Get the sample IDs (dummy values for a schedule estimate calculation,
				// and real IDs if called as part of an actual run.
				// Of course, we only do this if the protocol is a Separation Protocol.
				ISeparationProtocol separationProtocol = protocol as ISeparationProtocol;
				if ( ! isActualRun || separationProtocol == null)
				{
					// Get the dummy sample ID
					sampleIDs[protocolIndex] = protocolIndex;
				}
				else
				{
					// Create a new Sample record
					SeparatorDataStore.SampleRow sampleRow = theDataStore.Sample.NewSampleRow();
					SeparatorDataStore.ProtocolRow protocolRow = 
						theDataStore.Protocol.FindByProtocolId(protocol.Id);					
					SeparatorDataStore.SeparationProtocolRow[] separationRows = 
						protocolRow.GetSeparationProtocolRows();
					Debug.Assert(separationRows.GetLength(0) == 1);
					sampleRow.SampleSeparationProtocolId = separationRows[0].SeparationProtocolId;
					sampleRow.SampleLabel = string.Empty;	// NOTE: these are not supplied by the user for this version of the instrument.
					sampleRow.SampleVolume = sampleVolumes[protocolIndex];
					
					try
					{
						// Store the new sample record
						theDataStore.Sample.AddSampleRow(sampleRow);

						// Get the actual sample ID
						sampleIDs[protocolIndex] = sampleRow.SampleId;
					}
					catch //(Exception ex)
					{
						// NOTE - unable to add the new sample record -- future versions
                        // should make this a fatal error.
					}					
				}

				// Increment record index for the next record
				++protocolIndex;
			}
		}

        private int[] currentSampleIDs;
        public int ConvertIdToSampleIndex(int id)
        {
            for (int idx = 0; idx < currentSampleIDs.Length; idx++)
            {
                if (id == currentSampleIDs[idx])
                {
                    return idx;
                }
            }
            return 0;
        }

        public void ScheduleRun(QuadrantId quadrant)
		{
			try
			{
				// Update the state for the specified quadrant to "Configured".  If all selected
				// protocols are in this state, the schedule estimate will be recalculated.
                // (NOTE: the schedule estimate recalculation happens indirectly, as a side 
                // effect of the Quadrant State update event processing.)
				if (myQuadrants[(int)quadrant].State != SeparatorState.SeparatorUnloaded)
				{
					//RL Feb26 2015 bug fix update sample volume doesn't call schedule run
                    //this is such a stupid way to call schedule run... if you want to call schedule run
                    //then call the damn schedule run... don't depend on the side effect of
                    //setting IsConfigured to call it for you...
                    //fix is to fire quadrant update directly if quadrant is already configured
#if false
                    if (myQuadrants[(int)quadrant].IsConfigured)
                    {
                        myQuadrants[(int)quadrant].FireQuadrantStateUpdate();
                    }
                    else
                    {
					    myQuadrants[(int)quadrant].IsConfigured =  true;
                    }
#else
					myQuadrants[(int)quadrant].IsConfigured =  true;
#endif
                }        
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}

		private void RecalculateAndUpdateScheduleEstimate()
		{
			// Recalculate the batch run ETC
			TimeSpan scheduleEstimate = TimeSpan.Zero;
			if (myReadyToRun && myChosenProtocols.Count > 0)
			{				        
				int[]		icIDs			= null;
				int[]		sampleIDs		= null;
				double[]	sampleVolumes	= null;
				IProtocol[]	chosenProtocols	= null;
				PrepareScheduleParameters(false /* we're not preparing an actual run, just a schedule re-estimate */,
					out chosenProtocols, out icIDs, out sampleIDs, out sampleVolumes);
                   
                // Schedule the batch run to determine the schedule estimate
				scheduleEstimate = 
					myInstrument.ScheduleRun(chosenProtocols, icIDs, sampleIDs, sampleVolumes);
                LogFile.AddMessage(TraceLevel.Verbose,
                    string.Format("Time remaining (ScheduleRun ETC):\t{0}", scheduleEstimate.TotalDays));

                // Get the updated end time span and end time span warning threshold for the 
                // batch.
                myInstrument.GetBatchEndTimeSpanAndWarningThreshold_minutes(
                    out myEndTimeSpan_minutes,
                    out myEndTimeSpanWarningThreshold_minutes);
			}
			myTimeRemaining = scheduleEstimate;            
			FireBatchDurationUpdate();
		}

		public void StartRun(string batchRunUserId, 
			ReagentLotIdentifiers[][] protocolReagentLotIds,
			bool isSharing, int[] sharedSectorsTranslation, int[]sharedSectorsProtocolIndex)
		{
			try
			{
                LogFile.AddMessage(TraceLevel.Verbose, "QuadrantManager::StartRun");
				TimeSpan scheduleEstimate = TimeSpan.Zero;
				if (myReadyToRun)
				{                    
					int[]		icIDs			= null;
					int[]		sampleIDs		= null;
					double[]	sampleVolumes	= null;
					IProtocol[]	chosenProtocols	= null;
					PrepareScheduleParameters(true /* is an actual run */,
						out chosenProtocols, out icIDs, out sampleIDs, out sampleVolumes);
                
					// Start the batch run
					scheduleEstimate = 
						myInstrument.StartRun(batchRunUserId, chosenProtocols, 
							icIDs, sampleIDs, sampleVolumes, protocolReagentLotIds,
						isSharing,sharedSectorsTranslation,sharedSectorsProtocolIndex);
                    LogFile.AddMessage(TraceLevel.Verbose,
                        string.Format("Time remaining (StartRun ETC):\t{0}", scheduleEstimate.TotalDays));

					// Update the MRU statistic for the chosen protocols
					bool isSeparationProtocol = false, isMaintenanceProtocol = false,
						isShutdownProtocol = false;
					foreach (IProtocol protocol in chosenProtocols)
					{                    
						theDataStore.RecordProtocolUsageTime(protocol.Id);
						if (protocol as ISeparationProtocol != null)
						{
							isSeparationProtocol |= true;
						}
						if (protocol as IMaintenanceProtocol != null)
						{
							isMaintenanceProtocol |= true;
						}
						if (protocol as IShutdownProtocol != null)
						{
							isShutdownProtocol |= true;
						}
					}
					if (isSeparationProtocol)
					{
						FireSeparationProtocolListMRU_Update();
					}

					// Persist changes to the MRU statistics
					theDataStore.PersistMRU_Statistics();

                }
				myTimeRemaining = scheduleEstimate;                
				FireBatchDurationUpdate();
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
        }


		#if bdr12
		public void ParkArm()  // bdr
		{
			try
			{
        		System.Diagnostics.Debug.WriteLine("QMgr  - myInst.parkArm");
				myInstrument.ParkArm();
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}

		public void ParkPump()  // bdr
		{
			try
			{
        		System.Diagnostics.Debug.WriteLine("QMgr  - myInst.parkPump");
				myInstrument.ParkPump();
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}
		#endif

		public void HaltRun()
		{
			try
			{
				myInstrument.HaltRun();
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}

		public void AbortRun()
		{
			try
			{
				myInstrument.AbortRun();
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}

		public void PauseRun()
		{
			try
			{
				myInstrument.PauseRun();
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}

		public void ResumeRun()
		{
			try
			{
				myInstrument.ResumeRun();
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}

		public void Shutdown(IShutdownProtocol protocol)
		{
			try
			{
				// Select the shutdown protocol (this will remove any selected protocols)
				SelectProtocol(QuadrantId.Quadrant1, protocol);

				// Since we're about to call StartRun, there is no need to call ScheduleRun in
				// order to calculate an estimated time of completion.  

				// Now, start the shutdown protocol
				StartRun(string.Empty, null,false,new int[4], new int[4]);
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}

		private void UpdateCarouselState(SeparatorState newState)
		{
			// If the carousel state has changed, record the change and alert 
			// interested parties
			if (newState != myCarouselState)
			{
				myCarouselState = newState;
				FireSeparatorStatusUpdate();
			}
		}

		public void ReloadProtocols()
		{
			try
			{
				myInstrument.ReloadProtocols();
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}

		#endregion Quadrant services

        #region ISeparatorEvents Event Sources

        public event SeparatorStatusDelegate			UpdateSeparatorState;
		public event SeparationProtocolListDelegate		UpdateSeparationProtocolListMRU;
		public event MaintenanceProtocolListDelegate	UpdateMaintenanceProtocolList;
		public event ShutdownProtocolListDelegate		UpdateShutdownProtocolList;
		public event ChosenProtocolsDelegate			UpdateChosenProtocols;
		public event ProtocolConsumablesDelegate		UpdateProtocolConsumables;
		public event BatchDurationDelegate				UpdateBatchDuration;
        public event QuadrantStateDelegate				UpdateQuadrantState;
		public event InstrumentConfigurationDelegate	ReportInstrumentConfiguration;
        public event HydraulicFluidLevelDelegate        ReportHydraulicFluidRefillStatus;
		public event ReportInstrumentInfoDelegate		ReportInstrumentInformation;
		public event ReportStatusDelegate				ReportStatus;
        public event ReportErrorDelegate				ReportError;

        #endregion

		#region ISeparatorEvents Helper Methods

        // NOTE: some of these event notification methods have 'internal' visibility
        // so that initial updates may be requested explicitly by the Separator in
        // order to initialise the UI.  However, in general, event notification
        // methods have 'private' visibility - that is event updates are normally
        // triggered 'from below' (by receiving events from Instrument Control).

		internal void FireSeparatorStatusUpdate()
		{

            System.Diagnostics.Debug.WriteLine(String.Format("------------------------------ FireSeparatorStatusUpdate()-------------------------"));

			if (UpdateSeparatorState != null)
			{
				UpdateSeparatorState(myCarouselState);
			}
		}

		internal void FireSeparationProtocolListMRU_Update()
		{
			if (UpdateSeparationProtocolListMRU != null)
			{
				// Update the list of Separation Protocols in 'Most Recently Used' (MRU) order				
				UpdateSeparationProtocolListMRU(theDataStore.ListSeparationProtocolsInMRU());
			}
		}

		internal void FireMaintenanceProtocolListUpdate()
		{
			if (UpdateMaintenanceProtocolList != null)
			{
				// Update the list of Maintenance Protocols in Alphabetical order
				UpdateMaintenanceProtocolList(theDataStore.ListMaintenanceProtocolsInAlphabetical());
			}
		}

		internal void FireShutdownProtocolListUpdate()
		{
			if (UpdateShutdownProtocolList != null)
			{
				// Update the list of Shutdown Protocols in Alphabetical order
				UpdateShutdownProtocolList(theDataStore.ListShutdownProtocolsInAlphabetical());
			}
		}

		internal void FireChosenProtocolsUpdate()
		{
			if (UpdateChosenProtocols != null)
			{
				lock(this)
				{
					IProtocol[] chosenProtocols = new IProtocol[myChosenProtocols.Count];
					myChosenProtocols.Values.CopyTo(chosenProtocols, 0);					
					UpdateChosenProtocols(chosenProtocols);
				}				
			}
		}

        private int myEndTimeSpan_minutes, 
                    myEndTimeSpanWarningThreshold_minutes;

		internal void FireBatchDurationUpdate()
		{
			if (UpdateBatchDuration != null)
			{                                
                // Return the batch duration estimate and its end time span
				UpdateBatchDuration(myTimeRemaining, myEndTimeSpan_minutes,
                    myEndTimeSpanWarningThreshold_minutes);
			}
		}

		internal void FireInstrumentConfigurationUpdate()
		{
			if (ReportInstrumentConfiguration != null)
			{
				// Get the Instrument Configuration
				double bulkBufferDeadVolume;
				myInstrument.GetContainerVolumes(out bulkBufferDeadVolume);

				theBccmDeadVolume.Unit = FluidVolumeUnit.MicroLitres;
				theBccmDeadVolume.Amount = bulkBufferDeadVolume;

				// Report the instrument configuration to interested parties
				ReportInstrumentConfiguration(theBccmDeadVolume);
			}
		}

        internal void FireHydraulicFluidRefillStatusUpdate()
        {
            if (ReportHydraulicFluidRefillStatus != null)
            {
                ReportHydraulicFluidRefillStatus(isHydraulicFluidRefillRequired);
            }
        }

		internal void FireInstrumentInformationUpdate()
		{
			if (ReportInstrumentInformation != null)
			{
				// Get the instrument information
				string gatewayURL, gatewayInterfaceVersion, serverUptime_seconds, 
						instrumentControlVersion, instrumentSerialNumber, serviceConnection;
				myInstrument.GetServerInfo(out gatewayURL, out gatewayInterfaceVersion,
					out serverUptime_seconds, out instrumentControlVersion, 
					out instrumentSerialNumber, out serviceConnection);

				// Report the information to interested parties
				ReportInstrumentInformation(gatewayURL, gatewayInterfaceVersion,
					serverUptime_seconds, instrumentControlVersion, instrumentSerialNumber,
                    serviceConnection);
			}
		}

		private void FireQuadrantStateUpdate(QuadrantId quadrant, SeparatorState newState)
		{
			if (UpdateQuadrantState != null)
			{
				UpdateQuadrantState(quadrant, newState);
			}
			RecalculateCarouselState();
		}

		private void FireConsumablesUpdate(int protocolId, QuadrantId[] quadrantIds, 
			ProtocolConsumable[,] consumables)
		{
			int affectedQuadrantsCount = quadrantIds.GetLength(0);
			if (UpdateProtocolConsumables != null)
			{
				UpdateProtocolConsumables(protocolId, quadrantIds, consumables);								
			}
		}

		#endregion ISeparatorEvents Helper Methods

        #region IInstrumentControlEvents Event Handlers

		#region RunComplete handler

        public void RunCompleteUpdate()
        {
			myTimeRemaining = TimeSpan.Zero;			
            FireBatchDurationUpdate();
            UpdateCarouselState(SeparatorState.BatchComplete);
        }

        public static void AtRunComplete()
        {
			if (theQuadrantManager != null)
			{
				theQuadrantManager.RunCompleteUpdate();
			}
        }

		#endregion RunComplete handler

		#region ReportETC handler

		public void ReportEstimatedTimeOfCompletionUpdate(DateTime etcUpdate)
		{
            bool isUpdateRequired = false;
			lock(this)
			{
				DateTime currentTime = DateTime.Now;

				if (myCarouselState == SeparatorState.Running &&
					(DateTime.Compare(etcUpdate, currentTime) > 0))
				{
                    // Log the value of the update ETC
                    DateTimeFormatInfo currentDateTimeFormatInfo = DateTimeFormatInfo.CurrentInfo;
                    LogFile.AddMessage(TraceLevel.Verbose,
                        string.Format("ETC update: {0}", 
                            etcUpdate.ToString(currentDateTimeFormatInfo.SortableDateTimePattern)));

                    // Calculate time remaining based on the updated ETC
					myTimeRemaining = etcUpdate.Subtract(currentTime);                   
                    if (myTimeRemaining.TotalSeconds < 60.0d)
                    {
                        // Perform the same rounding for protocols of duration < 60 seconds as
                        // done for the ScheduleRun/StartRun calls.
                        myTimeRemaining = myTimeRemaining.Add(
                            TimeSpan.FromSeconds(59.0));
                    }
                    LogFile.AddMessage(TraceLevel.Verbose,
                        string.Format("Time remaining (ETC update):\t{0}", myTimeRemaining.TotalDays));
					isUpdateRequired = true;
				}
			}
            // Send batch duration update event, if required (outside the lock).
            if (isUpdateRequired)
            {
                FireBatchDurationUpdate();
            }
		}

		public static void AtReportETC(DateTime etc)
		{
			if (theQuadrantManager != null)
			{
				theQuadrantManager.ReportEstimatedTimeOfCompletionUpdate(etc);
			}
		}

		#endregion ReportETC handler

		#region ReportStatus handler

		public void ReportStatusUpdate(InstrumentState newInstrumentState, string statusCode, 
			string[] statusMessageValues)
		{			
			try
			{
                SeparatorState	previousSeparatorState = SeparatorState.SeparatorUnloaded;
				lock(this)
				{				
					// Record the change to the InstrumentState
					InstrumentState previousInstrumentState = myInstrumentState;
					myInstrumentState = newInstrumentState;					

                    // Save current Separator state so we can test for changes later
                    previousSeparatorState = myCarouselState;

					// Determine the state change as far as the Separator is concerned; send an 
					// state change event only if required.
					switch(newInstrumentState)
					{
						case InstrumentState.RESET:
						case InstrumentState.INIT:										
							// There is no visible (Separator) state change for entry into
							// these InstrumentStates.					
							break;	
						case InstrumentState.ESTOP:
							// In future we may have special handling in this case, but for now
							// assume that the 'reportError' method will suffice to alert the user
							// that there is a serious problem, and wait for the 'SHUTDOWN' state
							// to be reached to precipitate further action.
							break;
						case InstrumentState.HALTED:
							// This is a special case because the "HALTED" state is metastable --
							// it should revert to "IDLE" almost immediately.  Therefore, don't
							// bother to trigger a Separator state change; instead, wait for the
							// more stable "IDLE" state.
							break;	
						case InstrumentState.IDLE:
							if (previousInstrumentState == InstrumentState.INIT)
							{
								// The instrument has finished its initialisation, so
								// inform the UI it may now process user requests.
								myCarouselState = SeparatorState.SeparatorUnloaded;
							}
							if (previousInstrumentState == InstrumentState.RUNNING)
							{
								// The batch has completed normally.
								RunCompletedUpdates(SeparatorState.BatchComplete);
							}
							else if (previousInstrumentState == InstrumentState.HALTED)
							{
								// The batch has completed running due to user intervention.
								RunCompletedUpdates(SeparatorState.BatchHalted);
							}
							break;
						case InstrumentState.RUNNING:
							myCarouselState = SeparatorState.Running;                            
							break;
						case InstrumentState.PAUSED:
							myCarouselState = SeparatorState.Paused;                            
							break;
						case InstrumentState.SHUTTINGDOWN:
							myCarouselState = SeparatorState.ShuttingDown;                            
							break;
						case InstrumentState.SHUTDOWN:
							// There are multiple ways we can reach this state, but the action
							// is the same.  
							// We are going to shut down, but for now, send the state change to
							// the GUI anyway so it can initiate a shutdown in a 'clean' manner
							// (by disconnecting).  In future, this should be handled by a
							// SessionManager object.
							myCarouselState = SeparatorState.Shutdown;
							break;
						case InstrumentState.SERVICE:
							// NOTE: operation of the OperatorConsole once the Service
							// application is connected is presently undefined.
							LogFile.AddMessage(TraceLevel.Warning, newInstrumentState.ToString());
							break;
						
						case InstrumentState.PAUSECOMMAND:
							myCarouselState = SeparatorState.PauseCommand;                            
							break;
						default:
							System.Diagnostics.Debug.WriteLine(String.Format("------------------QuadrantManager ReportStatusUpdate 2 {0} undefined???",newInstrumentState));
							break;
					}					
				}

                // Send a state change event if required
                if (previousSeparatorState != myCarouselState)
                {
                    LogFile.AddMessage(TraceLevel.Verbose, 
                        "Carousel state changed to: " + myCarouselState.ToString());
                    FireSeparatorStatusUpdate();
                }

                // Send the status message update if required
                if (statusCode != null && statusCode != string.Empty && 
                    (statusCode.CompareTo("TSC0999") > 0)	
                    && ReportStatus != null)
                {
                    ReportStatus(statusCode, statusMessageValues);
                }				
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}

		private void RunCompletedUpdates(SeparatorState resultantSeparatorState)
		{
            // Don't need our own lock here as this method should only be called from
            // a synchronised context.

			// Clear consumables information for each of the protocols involved in the run.
			//foreach (IProtocol protocol in myChosenProtocols.Values)
			//{
			//	ClearAndUpdateQuadrantConsumables(protocol.Id, protocol.InitialQuadrant);
			//}
            
            // Update the time remaining to 'zero'
			myTimeRemaining = TimeSpan.Zero;
			FireBatchDurationUpdate();
			myCarouselState = resultantSeparatorState;            
		}		

		public static void AtReportStatus(InstrumentState state, string statusCode, 
			string[] statusMessageValues)
		{
			try
			{
				if (theQuadrantManager != null)
				{
					theQuadrantManager.ReportStatusUpdate(state, statusCode, statusMessageValues);
				}
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}

		#endregion ReportStatus handler

		#region ReportError handler

        public void ReportErrorUpdate(int severityLevel, string errorCode, 
            string[] errorMessageValues)
        {
			try
			{
				if (ReportError != null)
				{
					ReportError(severityLevel, errorCode, errorMessageValues);
				}
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
        }

        public static void AtReportError(int severityLevel, string errorCode, 
			string[] errorMessageValues)
		{
			try
			{
				if (theQuadrantManager != null)
				{
					theQuadrantManager.ReportErrorUpdate(severityLevel, errorCode, errorMessageValues);
				}
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
        }

		#endregion ReportError handler

		#endregion IInstrumentControlEvents Event Handlers

        public string isSharingComboValid(string batchRunUserId, bool isSharing, int[] sharedSectorsTranslation, int[] sharedSectorsProtocolIndex)
        {
            string result = "";
            try
            {
                LogFile.AddMessage(TraceLevel.Verbose, "QuadrantManager::StartRun");
                TimeSpan scheduleEstimate = TimeSpan.Zero;
                if (myReadyToRun)
                {
                    int[] icIDs = null;
                    int[] sampleIDs = null;
                    double[] sampleVolumes = null;
                    IProtocol[] chosenProtocols = null;
                    PrepareScheduleParameters(true /* is an actual run */,
                        out chosenProtocols, out icIDs, out sampleIDs, out sampleVolumes);

                    result = myInstrument.isSharingComboValid(batchRunUserId, chosenProtocols,
                            icIDs, sampleIDs, sampleVolumes,
                        isSharing, sharedSectorsTranslation, sharedSectorsProtocolIndex);
                    
                }
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
            return result;
        }
		
	}
}
