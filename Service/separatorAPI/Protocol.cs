//----------------------------------------------------------------------------
// Protocol
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
using Tesla.Common.Separator;
using Tesla.Common.ResourceManagement;/*CR*/

namespace Tesla.Separator
{
	/// <summary>
	/// Abstract base class for Separator protocols.
	/// </summary>
	[Serializable]
	public abstract class Protocol : IProtocol
	{
		protected int			myId;				// Unique identifier (note this is NOT the one supplied by InstrumentControl layer, which is only used in that contect, but our own)
		protected string		myLabel;
		
		// RL - Short Description for Sample Volume Dialog - 03/29/06
		protected string		myDescription;		
		protected int			myQuadrantCount;	// Resource usage (number of whole quadrants required to run this protocol)
		protected QuadrantId	myInitialQuadrant;
        private ProtocolClass	myClassification;  
		private string			myStringClassification;/*CR*/

		protected Protocol(int id, ProtocolClass classification,
            string label, string description, int numQuadrants)
		{
			myId				= id;
            myClassification    = classification;
			myLabel				= label;
			myDescription		= description;
			myQuadrantCount		= numQuadrants;
			myInitialQuadrant	= QuadrantId.NoQuadrant;
			switch (myClassification)/*CR*/
			{
				case ProtocolClass.HumanNegative:
					myStringClassification = "Human \nNegative";
					break;
				case ProtocolClass.HumanPositive:
					myStringClassification = "Human \nPositive";
					break;
				case ProtocolClass.MousePositive:
					myStringClassification = "Mouse \nPositive";
					break;
				case ProtocolClass.MouseNegative:
					myStringClassification = "Mouse \nNegative";
					break;
				case ProtocolClass.Positive:
					myStringClassification = "Positive";
					break;
				case ProtocolClass.Negative:
					myStringClassification = "Negative";
					break;
				case ProtocolClass.WholeBloodPositive:
					myStringClassification = "Whole Blood \nPositive";
					break;
				case ProtocolClass.WholeBloodNegative:
					myStringClassification = "Whole Blood \nNegative";
					break;
				case ProtocolClass.Maintenance:
					myStringClassification = "Maintenance";
					break;
				case ProtocolClass.Shutdown:
					myStringClassification = "Shutdown";
					break;
			}
		}

		#region IProtocol Members

		public int Id
		{
			get
			{
				return myId;
			}
		}

		public string Label
		{
			get
			{
				return myLabel;
			}
		}

		public string Description
		{
			get
			{
				return myDescription;
			}
		}

        public ProtocolClass Classification
        {
            get
            {
                return myClassification;
            }
        }

/*CR*/	public string StringClassification
		{
			get
			{
				return myStringClassification;
			}
		}

		public QuadrantId InitialQuadrant
		{
			get
			{
				return myInitialQuadrant;
			}
			set
			{
				myInitialQuadrant = value;
			}
		}

		public int	QuadrantCount
		{
			get
			{
				return myQuadrantCount;
			}
		}

		#endregion
	}
}
