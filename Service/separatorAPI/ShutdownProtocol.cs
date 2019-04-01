//----------------------------------------------------------------------------
// ShutdownProtocol
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
//----------------------------------------------------------------------------
using System;

using Tesla.Common.Protocol;

namespace Tesla.Separator
{
	/// <summary>
	/// Specialisation of Protocol for shutdown tasks.
	/// </summary>
	[Serializable]
	public class ShutdownProtocol : Protocol, IShutdownProtocol
	{
		// RL - Short Description for Sample Volume Dialog - 03/29/06
		//private string  myDescription;

		public ShutdownProtocol(int id, string label,
            int numQuadrants, string shutdownProtocolDescription) 
			: base(id, ProtocolClass.Shutdown, label, shutdownProtocolDescription, numQuadrants)
		{
			//myDescription = shutdownProtocolDescription;
		}

        #region IShutdownProtocol Members

        #endregion
	}
}
