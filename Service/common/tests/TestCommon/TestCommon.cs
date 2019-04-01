//----------------------------------------------------------------------------
// TestCommon
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
using NUnit.Framework;

using Tesla.Common;
using Tesla.Common.Separator;

namespace TestCommon
{
	/// <summary>
	/// Unit test harness for the Tesla.Common project
	/// </summary>
	[TestFixture]
	public class TestCommon
	{
		[Test, Ignore("Test yet to be implemented")]
		public void TestFluidVolume()
		{
			FluidVolume fluidVolume = new FluidVolume(2000.0, FluidVolumeUnit.MicroLitres);
		}
	}
}
