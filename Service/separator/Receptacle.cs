//----------------------------------------------------------------------------
// Receptacle
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

using Tesla.Common.Separator;

namespace Tesla.Separator
{
	/// <summary>
	/// Generic fluid container physically resident in a Quadrant (vial/tube)
	/// </summary>
	public class Receptacle : QuadrantResource
	{		
		private FluidVolume	myFluidCapacity;
		private FluidVolume myFluidVolume;

		public Receptacle(FluidVolume capacity)
		{
			myFluidCapacity = capacity;
		}

		public FluidVolume Volume
		{
			get
			{
				return myFluidVolume;
			}
			set
			{
				// Catch attempts to set volume > capacity
				if (myFluidCapacity.Amount >= value.Amount)
				{
					myFluidVolume = value;
				}
				else
				{
					// Normally we would throw an exception here.  
					// Currently, the configuration/capacities are fixed and not shown on the current GUI.
					// Hence, responsibility for volume/capacity checking is left to the Instrument Control
					// layer.  If this situation changes, then the Instrument Control API needs to be extended
					// so that the capacity table defined in the Quadrant class can be initialised correctly.
					// Refer to the Quadrant class for more details.
				}
			}
		}

		public FluidVolume Capacity
		{
			get
			{
				return myFluidCapacity;
			}
			set
			{
				myFluidCapacity = value;
			}
		}

		
	}
}
