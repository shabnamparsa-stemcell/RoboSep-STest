//----------------------------------------------------------------------------
// BaseTypes
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
namespace Tesla.Common
{
	#region enums

	/// <summary>
	/// Units in which fluid volumes are specified in protocols, by the user, etc.
	/// </summary>
	public enum FluidVolumeUnit
	{
		MicroLitres = 1,
		MilliLitres = 1000
	}

	/// <summary>
	/// Tesla application status codes
	/// </summary>
	/// <remarks>
	/// NOTE: these must be maintained in accordance with statusConfig.txt
	/// </remarks>
	public enum StatusCode
	{
		// Separator layer messages
		TSC2000,		// Supplied sample volume out of range.

		// Run log messages
		TSC3000,		// Carousel unloaded.
		TSC3001,		// Batch run halted.
		TSC3002,		// Batch run completed successfully.
		TSC3003,		// Batch run paused.
		TSC3004,		// Processing.
		TSC3005,		// Shutdown initiated.
		TSC3006,		// Instrument error: {0}
		TSC3007,		// Selecting protocol: {0}
		TSC3008,		// Deselecting protocol: {0}
		TSC3330,		// IAT Clear myChosenProtocols
		TSC3331,		// IAT Reload __mrd.db
	}

	#endregion enums

	#region Tesla application exception

	public class SeparatorException : System.Exception
	{
		private StatusCode	myStatusCode;
		private string[]	myStatusMessageParameters;

		public SeparatorException(StatusCode statusCode, params string[] statusMessageParameters)
		{
			myStatusCode = statusCode;
			myStatusMessageParameters = statusMessageParameters;
		}

		public Tesla.Common.StatusCode StatusCode 
		{
			get
			{
				return myStatusCode;
			}
		}

		public string[] StatusMessageParameters
		{
			get
			{
				return myStatusMessageParameters;
			}
		}
	}

	#endregion Tesla application exception

}

