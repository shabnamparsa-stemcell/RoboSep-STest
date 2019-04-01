//----------------------------------------------------------------------------
// SeparatorEventSink
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
//----------------------------------------------------------------------------
using System;
using System.Collections;
using System.Runtime.Remoting.Messaging;

using Tesla.Common.Protocol;
using Tesla.Common.Separator;

using Tesla.Separator;

namespace Tesla.Separator
{
	/// <summary>
	/// Event handlers for ISeparatorEvents.
	/// </summary>
	public class SeparatorEventSink : MarshalByRefObject, ISeparatorEvents
	{
        ISeparatorEvents myEventSource;

		public SeparatorEventSink(ISeparatorEvents eventSource)
		{
            myEventSource = eventSource;

			// Register our callback methods with the remote event source
            myEventSource.UpdateSeparatorState += new SeparatorStatusDelegate(AtSeparatorStateUpdate);
			myEventSource.UpdateChosenProtocols+= new ChosenProtocolsDelegate(AtUpdateChosenProtocolsUpdate);
			myEventSource.UpdateProtocolConsumables += new ProtocolConsumablesDelegate(AtProtocolConsumablesUpdate);
			myEventSource.UpdateSeparationProtocolListMRU += new SeparationProtocolListDelegate(AtSeparationProtocolListMRU_Update);
			myEventSource.UpdateMaintenanceProtocolList += new MaintenanceProtocolListDelegate(AtMaintenanceProtocolListUpdate);
			myEventSource.UpdateShutdownProtocolList += new ShutdownProtocolListDelegate(AtShutdownProtocolListUpdate);
			myEventSource.UpdateBatchDuration += new BatchDurationDelegate(AtBatchDurationUpdate);
            myEventSource.UpdateQuadrantState += new QuadrantStateDelegate(AtQuadrantStateUpdate);            
			myEventSource.ReportInstrumentConfiguration += new InstrumentConfigurationDelegate(AtReportInstrumentConfiguration);
            myEventSource.ReportHydraulicFluidRefillStatus += new HydraulicFluidLevelDelegate(AtHydraulicFluidLevelStatusUpdate);
			myEventSource.ReportInstrumentInformation += new ReportInstrumentInfoDelegate(AtReportInstrumentInformation);
			myEventSource.ReportStatus		  += new ReportStatusDelegate(AtReportStatus);	
            myEventSource.ReportError         += new ReportErrorDelegate(AtReportError);
		}

        ~SeparatorEventSink()
        {
            try
            {
                myEventSource = null;
            }
            finally
            {
            }
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

		#region ISeparatorEvents Sources

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

        [OneWay]
        public void AtSeparatorStateUpdate(SeparatorState newState)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("------------------------------ SeparatorEventSink: AtSeparatorStatusUpdate-------------------------"));

            if (UpdateSeparatorState != null)
            {
                UpdateSeparatorState(newState);
            }
        }

		[OneWay]
		public void AtSeparationProtocolListMRU_Update(ISeparationProtocol[] separationProtocolsInMRU)
		{
			if (UpdateSeparationProtocolListMRU != null)
			{
				UpdateSeparationProtocolListMRU(separationProtocolsInMRU);
			}
		}

		[OneWay]
		public void AtMaintenanceProtocolListUpdate(IMaintenanceProtocol[] maintenanceProtocols)
		{
			if (UpdateMaintenanceProtocolList != null)
			{
				UpdateMaintenanceProtocolList(maintenanceProtocols);
			}
		}

		[OneWay]
		public void AtShutdownProtocolListUpdate(IShutdownProtocol[] shutdownProtocols)
		{
			if (UpdateShutdownProtocolList != null)
			{
				UpdateShutdownProtocolList(shutdownProtocols);
			}
		}

		[OneWay]
		public void AtUpdateChosenProtocolsUpdate(IProtocol[] chosenProtocols)
		{
			if (UpdateChosenProtocols != null)
			{
				UpdateChosenProtocols(chosenProtocols);
			}
		}

		[OneWay]
		public void AtProtocolConsumablesUpdate(int protocolId, QuadrantId[] quadrantIds, 
			ProtocolConsumable[,] consumables)
		{
			if (UpdateProtocolConsumables != null)
			{
				UpdateProtocolConsumables(protocolId, quadrantIds, consumables);
			}
		}

		[OneWay]
		public void AtBatchDurationUpdate(TimeSpan estimatedDuration, 
            int endTimeSpan_minutes, int endTimeSpanWarningThreshold_minutes)
		{			
			if (UpdateBatchDuration != null)
			{
				UpdateBatchDuration(estimatedDuration, 
                    endTimeSpan_minutes, endTimeSpanWarningThreshold_minutes);
			}
		}

        [OneWay]
        public void AtQuadrantStateUpdate(QuadrantId quadrant, 
            SeparatorState newState)
        {
            if (UpdateQuadrantState != null)
            {
                UpdateQuadrantState(quadrant, newState);
            }
        }

		[OneWay]
		public void AtReportInstrumentConfiguration(FluidVolume bccmDeadVolume)
		{
			if (ReportInstrumentConfiguration != null)
			{
				ReportInstrumentConfiguration(bccmDeadVolume);
			}
		}

        [OneWay]
        public void AtHydraulicFluidLevelStatusUpdate(bool isRefillRequired)
        {
            if (ReportHydraulicFluidRefillStatus != null)
            {
                ReportHydraulicFluidRefillStatus(isRefillRequired);
            }
        }

		[OneWay]
		public void AtReportInstrumentInformation(string gatewayURL, 
			string gatewayInterfaceVersion, string serverUptime_seconds, 
			string instrumentControlVersion, string instrumentSerialNumber,
            string serviceConnection)
		{
			if (ReportInstrumentInformation != null)
			{
				ReportInstrumentInformation(gatewayURL, gatewayInterfaceVersion, 
					serverUptime_seconds, instrumentControlVersion, instrumentSerialNumber,
                    serviceConnection);
			}
		}

		[OneWay]
		public void AtReportStatus(string statusCode, string[] statusMessageValues)
		{
			System.Diagnostics.Debug.WriteLine(String.Format("------------------AtReportStatus 2"));
			if (ReportStatus != null)
			{
				ReportStatus(statusCode, statusMessageValues);
			}
		}

        [OneWay]
        public void AtReportError(int severityLevel, string errorCode, 
            string[] errorMessageValues)
		{
			System.Diagnostics.Debug.WriteLine(String.Format("------------------AtReportError 2"));
            if (ReportError != null)
            {
                ReportError(severityLevel, errorCode, errorMessageValues);
            }
        }

        #endregion       
    }
}
