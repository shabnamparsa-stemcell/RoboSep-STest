//----------------------------------------------------------------------------
// ProtocolServiceCommands
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

using Tesla.Common.ProtocolCommand;

namespace Tesla.ProtocolEditorModel
{
	/// <summary>
	/// Summary description for ProtocolServiceCommand.
	/// </summary>
	public abstract class ProtocolServiceCommand : ProtocolCommand, IServiceCommand
	{
		public ProtocolServiceCommand()
		{
		}
	}

	public class ProtocolHomeAllCommand : ProtocolServiceCommand
	{
		public ProtocolHomeAllCommand()
		{
		}
	}

	public class ProtocolDemoCommand : ProtocolServiceCommand
	{
		private uint	myIterationCount;

		public uint IterationCount
		{
			get
			{
				return myIterationCount;
			}
			set
			{
				myIterationCount = value;
			}
		}
	}
	public class ProtocolPumpLifeCommand : ProtocolServiceCommand
	{
		private uint	myIterationCount;

		public uint IterationCount
		{
			get
			{
				return myIterationCount;
			}
			set
			{
				myIterationCount = value;
			}
		}
	}
}
