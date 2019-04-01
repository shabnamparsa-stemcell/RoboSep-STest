//----------------------------------------------------------------------------
// ProtocolWaitCommands
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
//     03/29/06 - pause command - RL (look for PauseCommand)
//----------------------------------------------------------------------------
using System;

using Tesla.Common.ProtocolCommand;

namespace Tesla.ProtocolEditorModel
{
	/// <summary>
	/// Summary description for ProtocolWaitCommand.
	/// </summary>
	public abstract class ProtocolWaitCommand : ProtocolCommand, IWaitCommand
	{
		public enum WaitCommandItem
		{
			WaitCommandTimeDuration
		}

		private uint	myDuration;

		public ProtocolWaitCommand()
		{
		}

		#region IWaitCommand Members

		public uint WaitCommandTimeDuration
		{
			get
			{
				return myDuration;
			}
			set
			{
				myDuration = value;
			}
		}

		#endregion
	}

	public class ProtocolIncubateCommand : ProtocolWaitCommand
	{

		public ProtocolIncubateCommand()			
		{
		}
	}

	public class ProtocolSeparateCommand : ProtocolWaitCommand
	{
		public ProtocolSeparateCommand()
		{
		}
	}

	public class ProtocolPauseCommand : ProtocolWaitCommand
	{
		public ProtocolPauseCommand()
		{
		}
	}
}
