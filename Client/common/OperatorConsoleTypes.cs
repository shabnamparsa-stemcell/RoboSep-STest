//----------------------------------------------------------------------------
// OperatorConsoleTypes
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

namespace Tesla.Common.OperatorConsole
{
	/// <summary>
	/// Types shared throughout the UI (OperatorConsole and OperatorConsoleControls)
	/// </summary>
	
	#region enums

	/// <summary>
	/// Unique UI Page/Subpage identifiers
	/// </summary>
	public enum UiPage	
	{
		// Hosted by SampleProcessing
		Run,
		Protocols,
		Configuration,

		// Hosted by InstrumentTasks
		Maintenance,
		MessageLog,
	}

	/// <summary>
	/// Control access to aspects of the UI when the instrument is in different states
	/// </summary>
	public enum UiAccessMode
	{
		All,
		RunningMode
	}

	/// <summary>
	/// Indicate the "mix" of chosen protocols
	/// </summary>
	public enum ProtocolMix
	{
		SeparationOnly,		
		MaintenanceOnly,
		ShutdownOnly
	}

	#endregion enums

	#region Delegates

	public delegate void UiPageJumpDelegate(UiPage uiPage);

	public delegate void ShutdownInitiatedDelegate();

	#endregion Delegates
}
