//----------------------------------------------------------------------------
// QuadrantResource
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

namespace Tesla.Separator
{
	/// <summary>
	/// Quadrant item physically resident in a Quadrant (vial/tube/tips box)
	/// </summary>
	public class QuadrantResource
	{
		protected bool	myRequired;
		protected bool	myConfigured;

		public QuadrantResource()
		{
		}

		public bool IsRequired
		{
			get
			{
				return myRequired;
			}
			set
			{
				myRequired = value;
			}
		}

		public bool IsConfigured
		{
			get
			{
				return myConfigured;
			}
			set
			{
				myConfigured = value;
			}
		}
	}
}
