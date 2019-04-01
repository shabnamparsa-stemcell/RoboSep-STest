//----------------------------------------------------------------------------
// SeparationProtocol
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
	/// Specialisation of Protocol for cell separation tasks.
	/// </summary>
	[Serializable]
	public class SeparationProtocol : Protocol, ISeparationProtocol
	{
		private double			myMinimumVolume;
		private double			myMaximumVolume;
        private DateTime        myLastUsedTime;

		// RL - Short Description for Sample Volume Dialog - 03/29/06
		public SeparationProtocol(int id, ProtocolClass protocolClass, 
            string label, string description, int numQuadrants, double minVol, double maxVol, DateTime lastUsed) 
			: base(id, protocolClass, label, description, numQuadrants)
		{
			myId = id;
			myLabel = label;
			myQuadrantCount = numQuadrants;
			myMinimumVolume	= minVol;
			myMaximumVolume = maxVol;	
            myLastUsedTime  = lastUsed;
		}

		#region ISeparationProtocol Members

        public double MinimumSampleVolume
        {
            get
            {
                return myMinimumVolume;
            }
        }

        public double MaximumSampleVolume
        {
            get
            {
                return myMaximumVolume;
            }
        }

        public DateTime LastUsed
        {
            get
            {
                return myLastUsedTime;
            }
        }

		#endregion
	}
}
