//----------------------------------------------------------------------------
// Quadrant
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

using Tesla.Common;
using Tesla.Common.Separator;

namespace Tesla.Separator
{
	/// <summary>
	/// Representation of a physical carousel Quadrant.
	/// </summary>
	public class Quadrant
	{
		private QuadrantId			myQuadrantId;
		private SeparatorState		myState;
		private QuadrantResource[]	myResources;	// Stores whether resources are required & configured
		private int					myProtocolId;
		private bool				myProtocolInitialQuadrant;
		private bool				isConfigured;	// Stores whether a protocol has been marked as configured
		private struct CapacityTableEntry
		{
			public RelativeQuadrantLocation	aLocation;
			public double					aCapacity;
			public FluidVolumeUnit			aVolumeUnit;

			public CapacityTableEntry(RelativeQuadrantLocation location, double volume, FluidVolumeUnit unit)
			{
				aLocation	= location;
				aCapacity	= volume;
				aVolumeUnit	= unit;
			}
		};

		// NOTE: these are defined statically as a left-over from the earlier version of the GUI.  However, in
		// the current GUI, these values are not part of the display.  If they need to be used for validation,
		// or if the capacities again need to be on the display, then the instrument interface API should be 
		// changed so these values can be retrieved from the Instrument Control layer (sourced from the
		// instrument configuration file or equivalent).
		private static readonly CapacityTableEntry[] theQuadrantReceptacleCapacityTable = 
		{			
			new CapacityTableEntry(RelativeQuadrantLocation.SampleTube,			14000,	FluidVolumeUnit.MicroLitres),
			new CapacityTableEntry(RelativeQuadrantLocation.SeparationTube,	    14000,	FluidVolumeUnit.MicroLitres),
			new CapacityTableEntry(RelativeQuadrantLocation.WasteTube,			50000,	FluidVolumeUnit.MicroLitres),
			new CapacityTableEntry(RelativeQuadrantLocation.LysisBufferTube,	50000,	FluidVolumeUnit.MicroLitres),
			new CapacityTableEntry(RelativeQuadrantLocation.VialA,				  500,	FluidVolumeUnit.MicroLitres),			
			new CapacityTableEntry(RelativeQuadrantLocation.VialB,				 2000,	FluidVolumeUnit.MicroLitres),
			new CapacityTableEntry(RelativeQuadrantLocation.VialC,				 2000,	FluidVolumeUnit.MicroLitres),						
			new CapacityTableEntry(RelativeQuadrantLocation.QuadrantBuffer,	   250000,	FluidVolumeUnit.MicroLitres)
		};  

		public Quadrant(QuadrantId quadrantId)
		{
			myResources = new QuadrantResource[(int)RelativeQuadrantLocation.NUM_RELATIVE_QUADRANT_LOCATIONS];
			myQuadrantId = quadrantId;    

			// Initialise the Quadrant Resources that are also Receptacles
			for (RelativeQuadrantLocation location = RelativeQuadrantLocation.START_LOCATION; 
				location < RelativeQuadrantLocation.NUM_RELATIVE_QUADRANT_LOCATIONS; ++location)
			{
				// Assume the QuadrantReceptacleCapacityTable is not in any particular order.
				// In other words, search the table for the relevant QuadrantLocation details.
				for (int entryIndex = 0; 
					entryIndex < theQuadrantReceptacleCapacityTable.GetLength(0); ++entryIndex)
				{
					if (location == theQuadrantReceptacleCapacityTable[entryIndex].aLocation)
					{
						double amount = 
							theQuadrantReceptacleCapacityTable[entryIndex].aCapacity;
						FluidVolumeUnit unit = 
							theQuadrantReceptacleCapacityTable[entryIndex].aVolumeUnit;
						FluidVolume capacity = new FluidVolume(amount, unit);
						myResources[(int)location] = new Receptacle(capacity);
						break;	// Proceed with the next 'location'
					}
				}			
			}

			// Initialise the Quadrant Resources that are not Receptacles
			myResources[(int)RelativeQuadrantLocation.TipsBox] = new QuadrantResource();

			//Set the initial state of the quadrant
			Clear(false);
		}

		public SeparatorState State
		{
			get
			{
				UpdateState(false);
				return myState;
			}
		}

		public int	ProtocolId
		{
			get
			{
				return myProtocolId;
			}
			set
			{
				myProtocolId = value;
				// When a protocol is first selected, it defaults to the unconfigured state 
				isConfigured = false;
				UpdateState(true);
			}
		}

		public bool IsProtocolInitialQuadrant
		{
			get
			{
				return myProtocolInitialQuadrant;
			}
			set
			{
				myProtocolInitialQuadrant = value;
				UpdateState(true);
			}
		}

		/// <summary>
		/// Puts the quadrant into the configured state.
		/// </summary>
		public bool IsConfigured
		{
			get
			{
				return isConfigured;
			}
			set
			{
				isConfigured = value;
				UpdateState(true);
			}
		}

		/// <summary>
		/// Resets the quadrant to its default, empty state.
		/// Can supress the state change notification if required.
		/// </summary>
		public void Clear( bool notifyStateChange )
		{
			myProtocolId = 0;
			isConfigured = false;
			myProtocolInitialQuadrant = true;
			SampleTube.IsRequired = true;
			UpdateState(notifyStateChange);
		}
		/// <summary>
		/// Resets the quadrant to its default, empty state.
		/// </summary>
		public void Clear()
		{
			Clear(true);
		}

        public Receptacle SampleTube
        {
            get
            {
                return (Receptacle)myResources[(int)RelativeQuadrantLocation.SampleTube];
            }
        }

        public Receptacle SeparationTube
        {
            get
            {
                return (Receptacle)myResources[(int)RelativeQuadrantLocation.SeparationTube];
            }
        }

        public Receptacle VialA
        {
            get
            {
                return (Receptacle)myResources[(int)RelativeQuadrantLocation.VialA];
            }            
        }
		
        public Receptacle VialB
        {
            get
            {
                return (Receptacle)myResources[(int)RelativeQuadrantLocation.VialB];
            }           
        }

        public Receptacle VialC
        {
            get
            {
                return (Receptacle)myResources[(int)RelativeQuadrantLocation.VialC];
            }           
        }
		
        public Receptacle LysisBufferTube
        {
            get
            {
                return (Receptacle)myResources[(int)RelativeQuadrantLocation.LysisBufferTube];
            }           
        }

        public Receptacle WasteTube
        {
            get
            {
                return (Receptacle)myResources[(int)RelativeQuadrantLocation.WasteTube];
            }           
        }

		public Receptacle QuadrantBuffer
		{
			get
			{
				return (Receptacle)myResources[(int)RelativeQuadrantLocation.QuadrantBuffer];
			}           
		}

		public QuadrantResource TipsBox
		{
			get
			{
				return (QuadrantResource)myResources[(int)RelativeQuadrantLocation.TipsBox];
			}
		}
	
		// Recalculate the quadrant's state.
		// This is called any time something has changed that may cause the state to change.
		// NOTE: This doesn't allow for transitions after the Configured state - presently
		// they are not used.
		public void UpdateState(bool notifyStateChange)
		{
			SeparatorState prevState = myState;

			// No protocol selected
			if (myProtocolId == 0)
			{
				myState = SeparatorState.SeparatorUnloaded;
			}
			// If this isn't the initial quadrant, we relax the requirements for the quadrant to
			// have a sample volume and be configured, as this is taken care of by the initial
			// quadrant for the protocol.
			else if ( !myProtocolInitialQuadrant )
			{
				myState = SeparatorState.Configured;
			}
			// We have everything that is required.
			else if ( isConfigured )
			{
				myState = SeparatorState.Configured;
			}
			// We have the protocol, and sample volume (if required), but it is not configured,
			// so we should be in the selected state
			else
			{
				myState = SeparatorState.Selected;
			}

			// Check if the state has changed
			if ( (prevState != myState) && notifyStateChange )
			{
				FireQuadrantStateUpdate();
			}
		}

		#region Events

		public event QuadrantStateDelegate				UpdateQuadrantState;
		/// <summary>
		/// Event signature for Quadrant state change
		/// </summary>
		public delegate void QuadrantStateDelegate(QuadrantId quadrant, SeparatorState newState);

		private void FireQuadrantStateUpdate()
		{
			if (UpdateQuadrantState != null)
			{
				UpdateQuadrantState(myQuadrantId, myState);
			}
		}

		#endregion Events
	}
}
