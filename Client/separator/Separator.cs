//----------------------------------------------------------------------------
// Separator
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
// Copyright � 2004. All Rights Reserved.
//
// Notes:
//    03/24/05 - added islidclosed() and parkarm() - bdr
// 	  04/11/05 - added parkpump to deenergize pump - bdr
//
//----------------------------------------------------------------------------

#define bdr13   // islidclosed
#define bdr14   // park arm,pump


using System;
using System.Diagnostics;
using System.Collections;
using System.Threading;

using Invetech.ApplicationLog;

using Tesla.Common.Protocol;
using Tesla.Common.Separator;
using Tesla.InstrumentControl;
using Tesla.Common.ProtocolCommand; // bdr

namespace Tesla.Separator
{
    /// <summary>
    /// The Separator service exposed by the Separator API
    /// </summary>
    /// <remarks>
    /// TCP/IP server.  Created at run-time in response to the initial client call to 
    /// the registered service.
    /// Implicitly a Singleton because the singleton attribute is set when the service
    /// is registered.  Hence, multiple clients will call the same instance. 
    /// Delegates actual work to the 'SeparatorImpl' (implementation) object.
    /// NOTE: no resource locking yet (this implementation only supports single 
    /// clients at present).
    /// </remarks>
	public class Separator : MarshalByRefObject, ISeparator, ISeparatorEvents
	{
		private static SeparatorImpl	theSeparatorImpl;
		private static QuadrantManager	theQuadrantManager;
        private static OrcaTCPServer    theOrcaTCPServer;

		public Separator()
		{
			AppDomain domain = Thread.GetDomain();
			string d = domain.ToString();

            // NOTE: future versions may consider querying a SeparatorLayerManager or 
            // similar for these objects, so that we can protect against theSeparatorImpl 
            // being null in the worst case, but have construction of this object always 
            // succeed (even if we have to supply it with a reference to a mock separator 
            // impl).
			theSeparatorImpl	= SeparatorImpl.GetInstance();
			theQuadrantManager	= QuadrantManager.GetInstance();
            theOrcaTCPServer    = OrcaTCPServer.GetInstance();
		}

        public void CloseOrcaLogFileSystem()
        {
            try
            {
                lock (this)
                {
                    theSeparatorImpl.CloseLogFileSystem();
                }
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }

        }
        
        public void StartOrcaTCPServer()
        {
            try
            {
                lock (this)
                {
                    theOrcaTCPServer.Start();
                }
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
        }

        public void StopOrcaTCPServer()
        {
            try
            {
                lock (this)
                {
                    theOrcaTCPServer.Stop();
                }
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
        }

        public void BroadcastOrcaTCPServer(bool status)
        {
            try
            {
                lock (this)
                {
                    theOrcaTCPServer.BroadcastOrcaStatus(status);
                }
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
        }

        public override object InitializeLifetimeService()
        {
            // NOTE: future versions may re-consider ctor comments in the light of 'Server Activation' documentation
            return null;
        }

		#region ISeparator Members

		public void SetSampleVolume(QuadrantId initialQuadrant, FluidVolume volume)
		{
			try
			{
				lock(this)
				{
					theQuadrantManager.SetSampleVolume(initialQuadrant, volume);
				}
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}

        #if bdr13
        public bool IsLidClosed()  // bdr
        {
			bool isLidClosed = false;
            try
            {
                //lock(this)
                {
                    //theQuadrantManager.IsLidClosed();
                    isLidClosed = theQuadrantManager.IsLidClosed();
                }
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
                rtn = theQuadrantManager.IsHydroFluidLow();
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
				state = theQuadrantManager.GetInstrumentState();
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
			return state;
		}

        public int SetIgnoreLidSensor(int sw)     //CWJ
        {
            int rtn = -1;
            try
            {
                lock (this)
                {
                    rtn = theQuadrantManager.SetIgnoreLidSensor(sw);
                }
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }

            return rtn;
        }
        public bool GetIgnoreLidSensor()     //CWJ
        {
            bool rtn = false;
            try
            {
                lock (this)
                {
                    rtn = theQuadrantManager.GetIgnoreLidSensor();
                }
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }

            return rtn;
        }
        public int SetIgnoreHydraulicSensor(int sw)     //CWJ
        {
            int rtn = -1;
            try
            {
                lock (this)
                {
                    rtn = theQuadrantManager.SetIgnoreHydraulicSensor(sw);
                }
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }

            return rtn;
        }
        public bool GetIgnoreHydraulicSensor()     //CWJ
        {
            bool rtn = false;
            try
            {
                lock (this)
                {
                    rtn = theQuadrantManager.GetIgnoreHydraulicSensor();
                }
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }

            return rtn;
        }

        public int GetCurrentID()     //CWJ
        {
            int rtn = -1;
            try
            {
                lock (this)
                {
                    rtn = theQuadrantManager.GetCurrentID();
                }
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }

            return rtn;
        }

        public int ConvertIdToSampleIndex(int id)
        {
            int rtn = -1;
            try
            {
                lock (this)
                {
                    rtn = theQuadrantManager.ConvertIdToSampleIndex(id);
                }
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }

            return rtn;
        }

        public int GetCurrentSeq()     //CWJ
        {
            int rtn = -1;
            try
            {
                lock (this)
                {
                    rtn = theQuadrantManager.GetCurrentSeq();
                }
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }

            return rtn;
        }

        public int GetCurrentSec()     //CWJ
        {
            int rtn = -1;
            try
            {
                lock (this)
                {
                    rtn = theQuadrantManager.GetCurrentSec();
                }
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }

            return rtn;
        }

        public bool Ping()
        {
            bool rtn = false;
            try
            {
                rtn = theQuadrantManager.Ping();
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }

            return rtn;
        }

        public int InitScanVialBarcode(bool freeaxis)     //CWJ
        {
            int rtn = -1;
            try
            {
                rtn = theQuadrantManager.InitScanVialBarcode(freeaxis);
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }

            return rtn;
        }

        public bool AbortScanVialBarcode()     //twong
        {
            bool rtn = false;
            try
            {
                rtn = theQuadrantManager.AbortScanVialBarcode();
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
            return rtn;
        }

        public bool IsBarcodeScanningDone()     //twong
        {
            bool rtn = false;
            try
            {
                rtn = theQuadrantManager.IsBarcodeScanningDone();
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }

            return rtn;
        }

        public int ScanVialBarcodeAt(int Quadrant, int Vial)     //CWJ
        {
            int rtn = 0;
            try
            {
                rtn = theQuadrantManager.ScanVialBarcodeAt(Quadrant, Vial);
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }

            return rtn;
        }
        public string GetVialBarcodeAt(int Quadrant, int Vial)     //CWJ
        {
            string rtn = "Error";
            try
            {
                rtn = theQuadrantManager.GetVialBarcodeAt(Quadrant, Vial);
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }

            return rtn;
        }


        public string GetEndOfRunXMLFullFileName()     // 09-25-2013 - Sunny added
        {
            string rtn = "Error";
            try
            {
                rtn = theQuadrantManager.GetEndOfRunXMLFullFileName();
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }

            return rtn;
        }





        //RL - pause command - 03/29/06
		public bool isPauseCommand()
		{
			bool isPauseCommand=false;
			try
			{
				isPauseCommand = theQuadrantManager.isPauseCommand();
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
				pauseCommandCaption = theQuadrantManager.getPauseCommandCaption();
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
			return pauseCommandCaption;
		}
        
/*CR*/	public void GetCustomNames(string name, out string[] customNames, out int numQuadrants)
		{
			//customNames = new string[8];
			theQuadrantManager.GetCustomNames(name, out customNames, out numQuadrants);
		}
		public void GetResultVialSelection(string name, out bool[] resultChecks, out int numQuadrants)
		{
			//customNames = new string[8];
			theQuadrantManager.GetResultVialSelection(name, out resultChecks, out numQuadrants);
		}
        
        public void ConfirmHydraulicFluidRefilled()
        {
            try
            {
                lock(this)
                {
                    theQuadrantManager.ConfirmHydraulicFluidRefilled();
                }
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
        }

		public void SelectProtocol(QuadrantId initialQuadrant, IProtocol protocol)
		{
            System.Diagnostics.Debug.WriteLine(String.Format("--------- Separator: SelectProtocol called. initialQuadrant={0}", initialQuadrant));
            //Thread.Sleep(1000);

            if (protocol == null)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("--------- Separator: SelectProtocol called. input parameter protocol = null"));
                return;
            }

			try
			{
                bool bRet = false;

				lock(this)
				{
                    System.Diagnostics.Debug.WriteLine(String.Format("--------- Separator3: theQuadrantManager.SelectProtocol. initialQuadrant={0}. protocol={1}.", initialQuadrant, protocol.Label));

                    bRet = theQuadrantManager.SelectProtocol(initialQuadrant, protocol);
				}
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);

			}
		}

		public void DeselectProtocol(QuadrantId initialQuadrant)
		{
			try
			{
				lock(this)
				{
					theQuadrantManager.DeselectProtocol(initialQuadrant);
				}
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}
        private bool bQuadrantSchedulerBusy = false;
        public void ScheduleRun(QuadrantId quadrant)
		{
            bQuadrantSchedulerBusy = true;            
            try
			{
				lock(this)
				{
					theQuadrantManager.ScheduleRun(quadrant);
				}
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
            bQuadrantSchedulerBusy = false;            
		}

		public void StartRun(string batchRunUserId, 
			ReagentLotIdentifiers[][] protocolReagentLotIds,
			bool isSharing, int[] sharedSectorsTranslation, int[]sharedSectorsProtocolIndex)
		{
			try
			{
				lock(this)
				{
					theQuadrantManager.StartRun(batchRunUserId, protocolReagentLotIds,
						isSharing,sharedSectorsTranslation,sharedSectorsProtocolIndex);
				}
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}

		#if bdr14
		public void ParkArm() // bdr
		{
			try
			{
				lock(this)
				{
					theQuadrantManager.ParkArm();
				}
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}

		public void ParkPump() // bdr
		{
			try
			{
				lock(this)
				{
					theQuadrantManager.ParkPump();
				}
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
				lock(this)
				{
					theQuadrantManager.HaltRun();
				}
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
				lock(this)
				{
					theQuadrantManager.AbortRun();
				}
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
				lock(this)
				{
					theQuadrantManager.PauseRun();
				}
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
				lock(this)
				{
					theQuadrantManager.ResumeRun();
				}
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}

        public void Unload()
        {
			try
			{
				lock(this)
				{
					theQuadrantManager.ResetBatchState();
				}
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
				lock(this)
				{
					theQuadrantManager.Shutdown(protocol);
				}
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}

		public void Connect(bool reqSubscribe)
		{
			try
			{
				lock(this)
				{
					AppDomain domain = Thread.GetDomain();
					string d = domain.ToString();

					// Initialise objects at this layer
					theSeparatorImpl.Initialise();

					if(reqSubscribe)
					{
						// Connect to the event source(s) for this layer
						theQuadrantManager.UpdateSeparatorState += new SeparatorStatusDelegate(AtSeparatorStatusUpdate);
						theQuadrantManager.UpdateSeparationProtocolListMRU += new SeparationProtocolListDelegate(AtSeparationProtocolListMRU_Update);
						theQuadrantManager.UpdateMaintenanceProtocolList += new MaintenanceProtocolListDelegate(AtMaintenanceProtocolList_Update);
						theQuadrantManager.UpdateShutdownProtocolList += new ShutdownProtocolListDelegate(AtShutdownProtocolListUpdate);
						theQuadrantManager.UpdateChosenProtocols += new ChosenProtocolsDelegate(AtChosenProtocolsUpdate);
						theQuadrantManager.UpdateProtocolConsumables += new ProtocolConsumablesDelegate(AtProtocolConsumablesUpdate);
						theQuadrantManager.UpdateBatchDuration  += new BatchDurationDelegate(AtBatchDurationUpdate);
						theQuadrantManager.UpdateQuadrantState  += new QuadrantStateDelegate(AtQuadrantStateUpdate);
						theQuadrantManager.ReportInstrumentConfiguration += new InstrumentConfigurationDelegate(AtReportInstrumentConfiguration);
						theQuadrantManager.ReportHydraulicFluidRefillStatus += new HydraulicFluidLevelDelegate(AtHydraulicFluidRefillStatusUpdate);
						theQuadrantManager.ReportInstrumentInformation += new ReportInstrumentInfoDelegate(AtReportInstrumentInformation);					
						theQuadrantManager.ReportStatus			+= new ReportStatusDelegate(AtReportStatus);
						theQuadrantManager.ReportError          += new ReportErrorDelegate(AtReportError);
					}

					// We're now ready to connect to the instrument and receive updates
					theSeparatorImpl.Connect(reqSubscribe);

					// Trigger an initial status update for the new client
					theQuadrantManager.FireInstrumentInformationUpdate();
					theQuadrantManager.FireInstrumentConfigurationUpdate();
                    theQuadrantManager.FireHydraulicFluidRefillStatusUpdate();
					theQuadrantManager.FireSeparatorStatusUpdate();
					theQuadrantManager.FireBatchDurationUpdate();
					theQuadrantManager.FireChosenProtocolsUpdate();
					theQuadrantManager.FireSeparationProtocolListMRU_Update();
					theQuadrantManager.FireMaintenanceProtocolListUpdate();
					theQuadrantManager.FireShutdownProtocolListUpdate();
				}
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}

		public void Disconnect(bool isExit)
		{
			try
			{
				lock(this)
				{
					// Disconnect from the event source(s) for this layer
					theQuadrantManager.UpdateSeparatorState -= new SeparatorStatusDelegate(AtSeparatorStatusUpdate);
					theQuadrantManager.UpdateSeparationProtocolListMRU -= new SeparationProtocolListDelegate(AtSeparationProtocolListMRU_Update);
					theQuadrantManager.UpdateMaintenanceProtocolList -= new MaintenanceProtocolListDelegate(AtMaintenanceProtocolList_Update);
					theQuadrantManager.UpdateShutdownProtocolList -= new ShutdownProtocolListDelegate(AtShutdownProtocolListUpdate);
					theQuadrantManager.UpdateChosenProtocols -= new ChosenProtocolsDelegate(AtChosenProtocolsUpdate);
					theQuadrantManager.UpdateProtocolConsumables -= new ProtocolConsumablesDelegate(AtProtocolConsumablesUpdate);
					theQuadrantManager.UpdateBatchDuration  -= new BatchDurationDelegate(AtBatchDurationUpdate);
					theQuadrantManager.UpdateQuadrantState  -= new QuadrantStateDelegate(AtQuadrantStateUpdate);            
					theQuadrantManager.ReportInstrumentConfiguration -= new InstrumentConfigurationDelegate(AtReportInstrumentConfiguration);
                    theQuadrantManager.ReportHydraulicFluidRefillStatus -= new HydraulicFluidLevelDelegate(AtHydraulicFluidRefillStatusUpdate);
					theQuadrantManager.ReportInstrumentInformation -= new ReportInstrumentInfoDelegate(AtReportInstrumentInformation);
					theQuadrantManager.ReportStatus			-= new ReportStatusDelegate(AtReportStatus);
					theQuadrantManager.ReportError          -= new ReportErrorDelegate(AtReportError);

					// Trigger shutdown of all objects/services in this layer
					theSeparatorImpl.Disconnect(isExit);
				}
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}


		public void  ReloadProtocols()
		{
			try
			{
				lock(this)
				{	
					theQuadrantManager.ReloadProtocols();
				}
			}		
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}
		#endregion

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

        private void AtSeparatorStatusUpdate(SeparatorState newState)
        {

            System.Diagnostics.Debug.WriteLine(String.Format("------------------------------ Separator: AtSeparatorStatusUpdate-------------------------"));

			try
			{
				if (UpdateSeparatorState != null)
				{
					UpdateSeparatorState(newState);
				}
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
        }

		private void AtSeparationProtocolListMRU_Update(ISeparationProtocol[] separationProtocolsInMRU)
		{
			try
			{
				if (UpdateSeparationProtocolListMRU != null)
				{
					UpdateSeparationProtocolListMRU(separationProtocolsInMRU);
				}
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}

		private void AtMaintenanceProtocolList_Update(IMaintenanceProtocol[] maintenanceProtocols)
		{
			try
			{
				if (UpdateMaintenanceProtocolList != null)
				{
					UpdateMaintenanceProtocolList(maintenanceProtocols);
				}
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}

		private void AtShutdownProtocolListUpdate(IShutdownProtocol[] shutdownProtocols)
		{
			try
			{
				if (UpdateShutdownProtocolList != null)
				{
					UpdateShutdownProtocolList(shutdownProtocols);
				}
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
				if (UpdateChosenProtocols != null)
				{
					UpdateChosenProtocols(chosenProtocols);
				}
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}

		private void AtProtocolConsumablesUpdate(int protocolId, QuadrantId[] quadrantIds, 
			ProtocolConsumable[,] consumables)
		{
			try
			{
				if (UpdateProtocolConsumables != null)
				{
					UpdateProtocolConsumables(protocolId, quadrantIds, consumables);
				}
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}

        private void AtBatchDurationUpdate(TimeSpan estimatedDuration, 
            int endTimeSpan_minutes, int endTimeSpanWarningThreshold_minutes)
        {
			try
			{
				if (UpdateBatchDuration != null)
				{
					UpdateBatchDuration(estimatedDuration, 
                        endTimeSpan_minutes, endTimeSpanWarningThreshold_minutes);
				}
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
        }

        private void AtQuadrantStateUpdate(QuadrantId quadrant, SeparatorState newState)
        {
			try
			{
				if (UpdateQuadrantState != null)
				{
					UpdateQuadrantState(quadrant, newState);
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
				if (ReportInstrumentConfiguration != null)
				{
					ReportInstrumentConfiguration(bccmDeadVolume);
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
                if (ReportHydraulicFluidRefillStatus != null)
                {
                    ReportHydraulicFluidRefillStatus(isRefillRequired);
                }
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
        }

		private void AtReportInstrumentInformation(string gatewayURL, 
			string gatewayInterfaceVersion, string serverUptime_seconds, 
			string instrumentControlVersion, string instrumentSerialNumber,
            string serviceConnection)
		{
			try
			{
				if (ReportInstrumentInformation != null)
				{
					ReportInstrumentInformation(gatewayURL, gatewayInterfaceVersion, 
						serverUptime_seconds, instrumentControlVersion, 
						instrumentSerialNumber, serviceConnection);
				}
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}

		private void AtReportStatus(string statusCode, string[] statusMessageValues)
		{
			try
			{
				if (ReportStatus != null)
				{
					ReportStatus(statusCode, statusMessageValues);
				}
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}

        private void AtReportError(int severityLevel, string errorCode, 
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

        #endregion
        
        public bool IsSchedulerBusy()
        {
            bool rtn = false;
            try
            {
                rtn = theSeparatorImpl.IsSchedulerBusy();
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);            
            }

            return rtn;
        }
        
        public bool IsISeparatorBusy()
        {
            bool rtn = Monitor.TryEnter(this);
            if (rtn)
            {
                Monitor.Exit(this);
            }
            return !rtn;
        }

        public XmlRpcProtocolCommand[] getProtocolCommandList(int protocolIdx)
        {
            XmlRpcProtocolCommand[] cmds = null;
            try
            {
                LogFile.AddMessage(TraceLevel.Verbose, "++++++getProtocolCommandList() before lock");
                lock (this)
                {
                    cmds = theSeparatorImpl.getProtocolCommandList(protocolIdx);
                }
                LogFile.AddMessage(TraceLevel.Verbose, "++++++getProtocolCommandList() After lock");
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
            return cmds;
        }

        public string isSharingComboValid(string batchRunUserId,  bool isSharing, int[] sharedSectorsTranslation, int[] sharedSectorsProtocolIndex)
        {
            string result="";
            try
            {
                lock (this)
                {
                    result = theQuadrantManager.isSharingComboValid(batchRunUserId,
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
