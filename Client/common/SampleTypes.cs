//----------------------------------------------------------------------------
// SampleTypes
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
namespace Tesla.Common.Sample
{
	#region interfaces

	/// <summary>
	/// Behaviour relating to Samples
	/// </summary>
	public interface ISample
	{		
		/// <summary>
		/// Get the value of the sample fluid volume in the specified unit
		/// </summary>
		/// <param name="volumeUnit"></param>
		/// <returns></returns>
		/// <remarks>
		/// Note: cannot use a Property since we need to pass a parameter to get/set
		/// </remarks>
		float GetSampleVolume(FluidVolumeUnit volumeUnit);

		/// <summary>
		/// Set the value of the sample fluid volume in the specified unit
		/// </summary>
		/// <param name="volumeUnit"></param>
		/// <param name="volumeValue"></param>
		void SetSampleVolume(FluidVolumeUnit volumeUnit, float volumeValue);
	}

	#endregion interfaces
}
