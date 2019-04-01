//----------------------------------------------------------------------------
// ISeparatorEvents
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

using Tesla.Common.Protocol;
using Tesla.Common.Separator;

namespace Tesla.Separator
{
    #region delegates

    /// <summary>
    /// Event signature for Separator state change
    /// </summary>
    public delegate void SeparatorStatusDelegate(SeparatorState newState);

	/// <summary>
	/// Event signature for changes to the list of Separation Protocols
	/// </summary>
	public delegate void SeparationProtocolListDelegate(ISeparationProtocol[] separationProtocolsInMRU);

	/// <summary>
	/// Event signature for changes to the list of Maintenance Protocols
	/// </summary>
	public delegate void MaintenanceProtocolListDelegate(IMaintenanceProtocol[] maintenanceProtocols);

	/// <summary>
	/// Event signature for changes to the list of Shutdown Protocols
	/// </summary>
	public delegate void ShutdownProtocolListDelegate(IShutdownProtocol[] shutdownProtocols);

	/// <summary>
	/// Event signature for changes to the set of chosen protocols
	/// </summary>
	public delegate void ChosenProtocolsDelegate(IProtocol[] chosenProtocols);

	/// <summary>
	/// Event signature for Protocol consumables change
	/// </summary>
	public delegate void ProtocolConsumablesDelegate(int protocolId, QuadrantId[] quadrantIds, 
		ProtocolConsumable[,] consumables);

    /// <summary>
    /// Event signature for Batch duration estimate change
    /// </summary>
    public delegate void BatchDurationDelegate(TimeSpan estimatedDuration, 
        int endTimeSpan_minutes, int endTimeSpanWarningThreshold_minutes);

    /// <summary>
    /// Event signature for Quadrant state change
    /// </summary>
    public delegate void QuadrantStateDelegate(QuadrantId quadrant, SeparatorState newState);

	/// <summary>
	/// Event signature for reporting quadrant/carousel configuration information
	/// </summary>
	public delegate void InstrumentConfigurationDelegate(FluidVolume bccmDeadVolume);

    /// <summary>
    /// Event signature for reporting hydraulic fluid level refill status
    /// </summary>
    public delegate void HydraulicFluidLevelDelegate(bool isRefillRequired);

	/// <summary>
	/// Event signature for reporting general instrument information including serial number
	/// </summary>
	public delegate void ReportInstrumentInfoDelegate(
		string gatewayURL, string gatewayInterfaceVersion, string serverUptime_seconds, 
		string instrumentControlVersion, string instrumentSerialNumber, string serviceConnection);

	/// <summary>
	/// Event signature for reporting instrument status message (informational only, not for errors)
	/// </summary>
	public delegate void ReportStatusDelegate(string statusCode, string[] statusMessageValues);

    /// <summary>
    /// Event signature for reporting instrument errors
    /// </summary>
    public delegate void ReportErrorDelegate(int severityLevel, string errorCode,
		string[] errorMessageValues);

    #endregion delegates

	/// <summary>
	/// Public API for the EasySep separator
	/// </summary>
	/// <remarks>
	/// This interface represents the Separator-to-OperatorConsole direction.
	/// </remarks>
	/// <seealso>
	/// ISeparator
	/// </seealso>
	public interface ISeparatorEvents
	{
        /// <summary>
        /// Report Separator state change
        /// </summary>
        event SeparatorStatusDelegate				UpdateSeparatorState;

		/// <summary>
		/// Report a change to the list of available separation protocols
		/// </summary>
		event SeparationProtocolListDelegate		UpdateSeparationProtocolListMRU;

		/// <summary>
		/// Report a change to the list of available maintenance protocols
		/// </summary>
		event MaintenanceProtocolListDelegate		UpdateMaintenanceProtocolList;

		/// <summary>
		/// Report a change to the list of available shutdown protocols
		/// </summary>
		event ShutdownProtocolListDelegate			UpdateShutdownProtocolList;

		/// <summary>
		/// Report a change to the set of chosen protocols
		/// </summary>
		event ChosenProtocolsDelegate				UpdateChosenProtocols;

		/// <summary>
		/// Report updated list of Consumables for a protocol
		/// </summary>
		event ProtocolConsumablesDelegate			UpdateProtocolConsumables;

		/// <summary>
		/// Report estimated batch processing time
		/// </summary>		
		event BatchDurationDelegate					UpdateBatchDuration;

        /// <summary>
        /// Report Quadrant state change
        /// </summary>
        event QuadrantStateDelegate					UpdateQuadrantState;

		/// <summary>
		/// Report quadrant/carousel configuration information
		/// </summary>
		event InstrumentConfigurationDelegate		ReportInstrumentConfiguration;

        /// <summary>
        /// Report hydraulic fluid level refill status
        /// </summary>
        event HydraulicFluidLevelDelegate           ReportHydraulicFluidRefillStatus;

		/// <summary>
		/// Report general instrument information (e.g. serial number)
		/// </summary>
		event ReportInstrumentInfoDelegate			ReportInstrumentInformation;

		/// <summary>
		/// Report instrument status message (information level only, not for errors)
		/// </summary>
		event ReportStatusDelegate					ReportStatus;

        /// <summary>
        /// Report instrument error
        /// </summary>
        event ReportErrorDelegate					ReportError;
	}
}