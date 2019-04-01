//----------------------------------------------------------------------------
// ProtocolTypes
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
//	03/29/06 - Short Description for Sample Volume Dialog - RL
//	03/30/06 - Protocol Type Sync - RL
//----------------------------------------------------------------------------
using System;

using Tesla.Common.Separator;

namespace Tesla.Common.Protocol
{
	#region enums

	/// <summary>
	/// Separation protocol 'classes'
	/// </summary>
	public enum ProtocolClass	  	
	{
		//Positive,
		//Negative,
		//Maintenance,
		//Shutdown
		/*CR*/
		Positive,
		Negative,
		HumanPositive,
		MousePositive,
		HumanNegative,
		MouseNegative,
		WholeBloodPositive,
		WholeBloodNegative,
		Undefined,
		Maintenance,
		Shutdown,
	}		

	public enum MandatoryShutdownProtocol
	{
		Shutdown,
		FullShutdown
	}

	#endregion enums

	#region interfaces

	/// <summary>
	/// Behaviour common to all Separator Protocols
	/// </summary>
	public interface IProtocol
	{
		int Id
		{
			get;
		}

		string Label
		{
			get;
		}

		// RL - Short Description for Sample Volume Dialog - 03/29/06
		string Description
		{
			get;
		}

        ProtocolClass Classification
        {
            get;
        }

/*CR*/	string StringClassification
		{
			get;
		}

		QuadrantId InitialQuadrant
		{
			get;
			set;
		}

		int	QuadrantCount
		{
			get;
		}
	}

	/// <summary>
	/// Public API for Instrument Protocols run on samples
	/// </summary>
	public interface ISeparationProtocol : IProtocol
	{
		double MinimumSampleVolume
		{
			get;	// Minimum sample volumes vary according to the separation protocol type
		}		

		double MaximumSampleVolume
		{
			get;
		}

        DateTime LastUsed
        {
            get;
        }
	}

	/// <summary>
	/// Public API for non-sample-related Instrument Protocols 
	/// </summary>
	public interface IMaintenanceProtocol : IProtocol
	{
        int TaskId
        {
            get;
        }
	}

	/// <summary>
	/// 
	/// </summary>
	public interface IShutdownProtocol : IProtocol
	{
	}

	#endregion interfaces
}
