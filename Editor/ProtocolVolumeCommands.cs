//----------------------------------------------------------------------------
// ProtocolVolumeCommands
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
//	09/05/07 - usebuffertip command - bdr
//
//----------------------------------------------------------------------------

using System;

using Tesla.Common.ProtocolCommand;
using Tesla.Common.Separator;

namespace Tesla.ProtocolEditorModel
{
	#region Volume command base classes

	/// <summary>
	/// Summary description for ProtocolVolumeCommand.
	/// </summary>
	public abstract class ProtocolVolumeCommand : ProtocolCommand, IVolumeCommand
	{
		private AbsoluteResourceLocation	mySourceVial,
											myDestinationVial;

		private VolumeType	myVolumeTypeSpecifier = VolumeType.NotSpecified;
		private int		myAbsoluteVolume_uL = -1;
		private double		myRelativeVolumeProportion = -1.0f;
      	private int			tipRack = 1;
      	private bool		tipRackSpecified = false;

		#region IVolumeCommand Members

		public AbsoluteResourceLocation SourceVial
		{
			get
			{
				return mySourceVial;
			}
			set
			{
				mySourceVial = value;
			}
		}

		public AbsoluteResourceLocation DestinationVial
		{
			get
			{
				return myDestinationVial;				
			}
			set
			{
				myDestinationVial = value;
			}
		}

		public VolumeType VolumeTypeSpecifier
		{
			get
			{
				return myVolumeTypeSpecifier;
			}
			set
			{
				myVolumeTypeSpecifier = value;
			}
		}

		public double RelativeVolumeProportion
		{
			get
			{
				double relativeVolumeProportion = -1.0f;
				if (myVolumeTypeSpecifier == VolumeType.Relative)
				{
					relativeVolumeProportion = myRelativeVolumeProportion;
				}
				else
				{
					relativeVolumeProportion = -1.0f;	// Volume type & value combination not valid
				}
				return relativeVolumeProportion;
			}
			set
			{
				if (myVolumeTypeSpecifier == VolumeType.Relative)
				{
					myRelativeVolumeProportion = value;
				}
			}
		}

		public int AbsoluteVolume_uL
		{
			get
			{
				int absoluteVolume_uL = 0;
				if (myVolumeTypeSpecifier == VolumeType.Absolute)
				{
					absoluteVolume_uL = myAbsoluteVolume_uL;
				}
				else
				{
					absoluteVolume_uL = -1;	// Volume type & value combination not valid
				}
				return absoluteVolume_uL;
			}
			set
			{
				if (myVolumeTypeSpecifier == VolumeType.Absolute)
				{
					myAbsoluteVolume_uL = value;
				}
			}
		}

      	public int TipRack
		{
			get
			{
				return tipRack;
			}
			set
			{
				tipRack = value;
				//check for bad value
				if ((tipRack > 4) || (tipRack < 1))
					tipRack = 1;
			}
		}
      	public bool TipRackSpecified
		{
			get
			{
				return tipRackSpecified;
			}
			set
			{
				tipRackSpecified = value;
			}
		}
		#endregion

        protected static bool isSmallVial(AbsoluteResourceLocation vial)
        {
            string s = vial.ToString();
            return (s.EndsWith("03") || s.EndsWith("04") || s.EndsWith("05"));
        }
        protected static bool isVolumeBelow(int acceptableVol, VolumeType volumeTypeSpecifier, int absoluteVolume_uL, double relativeVolumeProportion, double sampleVolMin)
        {
            // Three cases:
            // if Absolute Volume checkbox ticked, Total Volume = Absolute Volume
            if (volumeTypeSpecifier == VolumeType.Absolute)
            {
                return (absoluteVolume_uL < acceptableVol);
            }
            // if the Relative Proportion checkbox has been ticked, Total Volume = 
            // (Max Volume)*(Relative Proportion)
            else if (volumeTypeSpecifier == VolumeType.Relative)
            {
                return (((int)(relativeVolumeProportion * sampleVolMin)) < acceptableVol);
            }
            // if neither box checked, Total Volume = Max Volume
            else
            {
                return (sampleVolMin < acceptableVol);
            }
        }
        protected static bool isVolumeAbove(int acceptableVol, VolumeType volumeTypeSpecifier, int absoluteVolume_uL, double relativeVolumeProportion, double sampleVol)
        {
            // Three cases:
            // if Absolute Volume checkbox ticked, Total Volume = Absolute Volume
            if (volumeTypeSpecifier == VolumeType.Absolute)
            {
                return (absoluteVolume_uL > acceptableVol);
            }
            // if the Relative Proportion checkbox has been ticked, Total Volume = 
            // (Max Volume)*(Relative Proportion)
            else if (volumeTypeSpecifier == VolumeType.Relative)
            {
                return (((int)(relativeVolumeProportion * sampleVol)) > acceptableVol);
            }
            // if neither box checked, Total Volume = Max Volume
            else
            {
                return (sampleVol > acceptableVol);
            }
        }

        protected static VolumeError getErrorFromSpecifier(VolumeType sp)
        {
            switch (sp)
            {
                case VolumeType.Absolute:
                    return VolumeError.ABOVE_ABSOLUTE_AMOUNT;
                case VolumeType.Relative:
                    return VolumeError.ABOVE_RELATIVE_AMOUNT;
                default:
                    return VolumeError.ABOVE_MAXIMUM_SAMPLE_VOLUME;
            }
        }
	}

	public abstract class ProtocolWorkingVolumeCommand : ProtocolVolumeCommand, IWorkingVolumeCommand
	{
		#region IWorkingVolumeCommand Members

		private bool myFreeAirDispense;

		public bool FreeAirDispense
		{
			get
			{
				return myFreeAirDispense;
			}
			set
			{
				myFreeAirDispense = value;
			}
		}

		private int myWorkingVolume_uL;

		public int WorkingVolume_uL
		{
			get
			{
				return myWorkingVolume_uL;
			}
			set
			{
				myWorkingVolume_uL = value;
			}
		}

		private bool myUseBufferTip; // bdr

		public bool UseBufferTip
		{
			get
			{
				return myUseBufferTip;
			}
			set
			{
				myUseBufferTip = value;
			}
		}
		#endregion
	}

    public abstract class ProtocolTopUpResuspend : ProtocolWorkingVolumeCommand
    {
        protected string msg_prefix;
        public override VolumeError validateCommandAndUpdateCommandStatus(VolumeLimits volumeLimits, int sampleVolumeMin, int sampleVolumeMax)
        {
            VolumeError err = VolumeError.NO_ERROR;
            this.CommandCheckStatus = VolumeCheck.VOLUMES_VALID;
            this.CommandStatus = "";

            int acceptableVolDst = volumeLimits.getCapacity(DestinationVial);
            if (VolumeTypeSpecifier == VolumeType.Relative)
            {
                if (isVolumeAbove(acceptableVolDst - sampleVolumeMin, VolumeTypeSpecifier, 0, RelativeVolumeProportion, sampleVolumeMin))
                {
                    err = getErrorFromSpecifier(VolumeTypeSpecifier);
                    this.CommandCheckStatus = VolumeCheck.VOLUMES_WARNINGS;
                    this.CommandStatus = msg_prefix+" vol > dst cap of " + acceptableVolDst + " uL";
                }
            }

            return err;
        }
    }

	public abstract class ProtocolVolumeMaintenanceCommand : ProtocolVolumeCommand, IVolumeMaintenanceCommand
	{
		private bool myHomeFlag;

		#region IVolumeMaintenanceCommand Members

		public bool HomeFlag
		{
			get
			{
				return myHomeFlag;
			}
			set
			{
				myHomeFlag = value;
			}
		}

		#endregion
	}

	#endregion Volume command base classes

	// Protocol Volume commands
	public class ProtocolBaseMixCommand : ProtocolVolumeCommand
	{
		private int		myTipTubeBottomGap_uL = 0;
		private int			myMixCycles = 3;
		public ProtocolBaseMixCommand()
		{
		}

		public int TipTubeBottomGap_uL
		{
			get
			{
				int tipTubeBottomGap_uL = 0;
				if (VolumeTypeSpecifier == VolumeType.Absolute)
				{
					tipTubeBottomGap_uL = myTipTubeBottomGap_uL;
				}
				return tipTubeBottomGap_uL;
			}
			set
			{
				if (VolumeTypeSpecifier == VolumeType.Absolute)
				{
					myTipTubeBottomGap_uL = value;
				}
			}
		}

		public int MixCycles
		{
			get
			{
				return myMixCycles;
			}
			set
			{
				myMixCycles = value;
				//check for bad value
				if ((myMixCycles > 5) && (myMixCycles < 1))
					myMixCycles = 1;
			}
		}

        public override VolumeError validateCommandAndUpdateCommandStatus(VolumeLimits volumeLimits, int sampleVolumeMin, int sampleVolumeMax)
        {
            VolumeError err = VolumeError.NO_ERROR;
            this.CommandCheckStatus = VolumeCheck.VOLUMES_VALID;
            this.CommandStatus = "";

            int acceptableVolSrc = volumeLimits.getCapacity(SourceVial) - volumeLimits.getDeadVolume(SourceVial);
            if (VolumeTypeSpecifier == VolumeType.Absolute)
            {
                if (isVolumeAbove(acceptableVolSrc, VolumeTypeSpecifier, AbsoluteVolume_uL, 0.0f, 0))
                {
                    err = getErrorFromSpecifier(VolumeTypeSpecifier);
                    this.CommandCheckStatus = VolumeCheck.VOLUMES_WARNINGS;
                    this.CommandStatus = "vol > src cap of " + acceptableVolSrc + " uL";
                }
                else if(!isSmallVial(SourceVial))
                {
                    err = VolumeError.MIXING_WITH_ABSOLUTE_AMOUNT;
                    this.CommandCheckStatus = VolumeCheck.VOLUMES_WARNINGS;
                    this.CommandStatus = "using abs vol for mixing";
                }
            }
            else if (VolumeTypeSpecifier == VolumeType.Relative)
            {
                int acceptableTipVol = volumeLimits.maximumCapacity_SampleTipVolume_ul;
                if (isSmallVial(SourceVial))
                {
                    acceptableTipVol = volumeLimits.maximumCapacity_ReagentTipVolume_ul;
                }
                if (isVolumeAbove(acceptableTipVol, VolumeTypeSpecifier, 0, RelativeVolumeProportion, sampleVolumeMax))
                {
                    err = getErrorFromSpecifier(VolumeTypeSpecifier);
                    this.CommandCheckStatus = VolumeCheck.VOLUMES_WARNINGS;
                    this.CommandStatus = "vol > tip cap of " + acceptableTipVol + " uL";
                }
            }

            return err;
        }
	}

	public class ProtocolTransportCommand : ProtocolWorkingVolumeCommand
	{
		public ProtocolTransportCommand()
		{
		}
        public override VolumeError validateCommandAndUpdateCommandStatus(VolumeLimits volumeLimits, int sampleVolumeMin, int sampleVolumeMax)
        {
            VolumeError err = VolumeError.NO_ERROR;
            this.CommandCheckStatus = VolumeCheck.VOLUMES_VALID;
            this.CommandStatus = "";
            if (CalculatedVolumeBelowRecommended(volumeLimits,sampleVolumeMin,sampleVolumeMax))
            {
                err = VolumeError.BELOW_AMOUNT;
                return err;
            }
            else if (CalculatedVolumeAboveRecommended(volumeLimits, sampleVolumeMin, sampleVolumeMax, out err))
            {
                return err;
            }
            else if (isSmallVial(DestinationVial) && !canReagentVialBeTransportDestination()) 
            {
                this.CommandCheckStatus = VolumeCheck.VOLUMES_WARNINGS;
                this.CommandStatus = "dst is reagent vial";
                err = VolumeError.DEST_SMALL_VIAL;
                return err; 
            }
            return err;
        }

        private static bool isReagentVialBeTransportDestinationInitialized = false;
        private static bool myCanReagentVialBeTransportDestination = false;
        public static bool canReagentVialBeTransportDestination()
        {
            if (!isReagentVialBeTransportDestinationInitialized)
            {
                isReagentVialBeTransportDestinationInitialized = true;
                myCanReagentVialBeTransportDestination =
                    Tesla.Common.Utilities.GetSoftwareConfigInt("Editor_Options", "enable_reagent_vial_as_destination", 0)!=0;
            }

            return myCanReagentVialBeTransportDestination;
        }

        private bool CalculatedVolumeBelowRecommended(VolumeLimits volumeLimits, int sampleVolumeMin, int sampleVolumeMax)
        {
            // if at least one 1mL vial is used (id ends with 03, 04 or 05) as source  
            //  vial, then TOTAL volume to be transferred must be above the acceptable amounts
            int acceptableVol = 0;
            string status = "";
            bool result = false;
            if (isSmallVial(SourceVial))
            {
                acceptableVol = volumeLimits.minimumRecommended_ReagentTipVolume_ul;
                status = "vol < reagent tip vol of " + acceptableVol +" uL";
            }
            else
            {
                acceptableVol = volumeLimits.minimumRecommended_SampleTipVolume_ul;
                status = "vol < sample tip vol of " + acceptableVol + " uL";
            }
            if (isVolumeBelow(acceptableVol, VolumeTypeSpecifier, AbsoluteVolume_uL, RelativeVolumeProportion, sampleVolumeMin))
            {
                this.CommandCheckStatus = VolumeCheck.VOLUMES_WARNINGS;
                this.CommandStatus = status;
                result = true;
            }
            return result;
        }
        private bool CalculatedVolumeAboveRecommended(VolumeLimits volumeLimits, int sampleVolumeMin, int sampleVolumeMax, out VolumeError err)
        {
            bool result = false;
            int acceptableVolSrc = volumeLimits.getCapacity(SourceVial) - volumeLimits.getDeadVolume(SourceVial);
            int acceptableVolDst = volumeLimits.getCapacity(DestinationVial);
            err = VolumeError.NO_ERROR;
            if (isVolumeAbove(acceptableVolSrc, VolumeTypeSpecifier, AbsoluteVolume_uL, RelativeVolumeProportion, sampleVolumeMax))
            {
                err = getErrorFromSpecifier(VolumeTypeSpecifier);
                this.CommandCheckStatus = VolumeCheck.VOLUMES_WARNINGS;
                this.CommandStatus = "vol > src cap of " + acceptableVolSrc + " uL";
                result = true;
            }
            else if (isVolumeAbove(acceptableVolDst, VolumeTypeSpecifier, AbsoluteVolume_uL, RelativeVolumeProportion, sampleVolumeMax))
            {
                err = getErrorFromSpecifier(VolumeTypeSpecifier);
                this.CommandCheckStatus = VolumeCheck.VOLUMES_WARNINGS;
                this.CommandStatus = "vol > dst cap of " + acceptableVolDst + " uL";
                result = true;
            }
            return result;
        }

        #region Exceptions
     
        #endregion
    }

    public class ProtocolTopUpVialCommand : ProtocolTopUpResuspend
	{
		public ProtocolTopUpVialCommand()
		{
            msg_prefix="TopUp";
		}
        
	}

    public class ProtocolResuspendVialCommand : ProtocolTopUpResuspend
	{
		public ProtocolResuspendVialCommand()
        {
            msg_prefix = "Resus";
		}
	}

	// Protocol Volume Maintenance commands
	public class ProtocolFlushCommand : ProtocolVolumeMaintenanceCommand
	{
		public ProtocolFlushCommand()
		{
		}
	}

	public class ProtocolPrimeCommand : ProtocolVolumeMaintenanceCommand
	{
		public ProtocolPrimeCommand()
		{
		}
	}

    public class ProtocolGenericMultiStepsCommand : ProtocolWorkingVolumeCommand, IMultiSrcDestCommand, IWaitCommand
    {
        private AbsoluteResourceLocation mySourceVial2, myDestinationVial2, mySourceVial3, myDestinationVial3;
        private int myTipTubeBottomGap_uL = 0;
        private int myMixCycles = 3;
        private uint myDuration;


        private VolumeType myVolumeTypeSpecifier2 = VolumeType.NotSpecified;
        private int myAbsoluteVolume_uL2 = -1;
        private double myRelativeVolumeProportion2 = -1.0f;


        private VolumeType myVolumeTypeSpecifier3 = VolumeType.NotSpecified;
        private int myAbsoluteVolume_uL3 = -1;
        private double myRelativeVolumeProportion3 = -1.0f;

        private VolumeType myVolumeTypeSpecifier4 = VolumeType.NotSpecified;
        private int myAbsoluteVolume_uL4 = -1;
        private double myRelativeVolumeProportion4 = -1.0f;
        
        public AbsoluteResourceLocation SourceVial2
        {
            get
            {
                return mySourceVial2;
            }
            set
            {
                mySourceVial2 = value;
            }
        }
        public AbsoluteResourceLocation DestinationVial2
        {
            get
            {
                return myDestinationVial2;
            }
            set
            {
                myDestinationVial2 = value;
            }
        }

        public VolumeType VolumeTypeSpecifier2
        {
            get
            {
                return myVolumeTypeSpecifier2;
            }
            set
            {
                myVolumeTypeSpecifier2 = value;
            }
        }

        public double RelativeVolumeProportion2
        {
            get
            {
                double relativeVolumeProportion = -1.0f;
                if (myVolumeTypeSpecifier2 == VolumeType.Relative)
                {
                    relativeVolumeProportion = myRelativeVolumeProportion2;
                }
                else
                {
                    relativeVolumeProportion = -1.0f;	// Volume type & value combination not valid
                }
                return relativeVolumeProportion;
            }
            set
            {
                if (myVolumeTypeSpecifier2 == VolumeType.Relative)
                {
                    myRelativeVolumeProportion2 = value;
                }
            }
        }

        public int AbsoluteVolume_uL2
        {
            get
            {
                int absoluteVolume_uL = 0;
                if (myVolumeTypeSpecifier2 == VolumeType.Absolute)
                {
                    absoluteVolume_uL = myAbsoluteVolume_uL2;
                }
                else
                {
                    absoluteVolume_uL = -1;	// Volume type & value combination not valid
                }
                return absoluteVolume_uL;
            }
            set
            {
                if (myVolumeTypeSpecifier2 == VolumeType.Absolute)
                {
                    myAbsoluteVolume_uL2 = value;
                }
            }
        }

        public AbsoluteResourceLocation SourceVial3
        {
            get
            {
                return mySourceVial3;
            }
            set
            {
                mySourceVial3 = value;
            }
        }
        public AbsoluteResourceLocation DestinationVial3
        {
            get
            {
                return myDestinationVial3;
            }
            set
            {
                myDestinationVial3 = value;
            }
        }

        public VolumeType VolumeTypeSpecifier3
        {
            get
            {
                return myVolumeTypeSpecifier3;
            }
            set
            {
                myVolumeTypeSpecifier3 = value;
            }
        }

        public double RelativeVolumeProportion3
        {
            get
            {
                double relativeVolumeProportion = -1.0f;
                if (myVolumeTypeSpecifier3 == VolumeType.Relative)
                {
                    relativeVolumeProportion = myRelativeVolumeProportion3;
                }
                else
                {
                    relativeVolumeProportion = -1.0f;	// Volume type & value combination not valid
                }
                return relativeVolumeProportion;
            }
            set
            {
                if (myVolumeTypeSpecifier3 == VolumeType.Relative)
                {
                    myRelativeVolumeProportion3 = value;
                }
            }
        }

        public int AbsoluteVolume_uL3
        {
            get
            {
                int absoluteVolume_uL = 0;
                if (myVolumeTypeSpecifier3 == VolumeType.Absolute)
                {
                    absoluteVolume_uL = myAbsoluteVolume_uL3;
                }
                else
                {
                    absoluteVolume_uL = -1;	// Volume type & value combination not valid
                }
                return absoluteVolume_uL;
            }
            set
            {
                if (myVolumeTypeSpecifier3 == VolumeType.Absolute)
                {
                    myAbsoluteVolume_uL3 = value;
                }
            }
        }

        public VolumeType VolumeTypeSpecifier4
        {
            get
            {
                return myVolumeTypeSpecifier4;
            }
            set
            {
                myVolumeTypeSpecifier4 = value;
            }
        }

        public double RelativeVolumeProportion4
        {
            get
            {
                double relativeVolumeProportion = -1.0f;
                if (myVolumeTypeSpecifier4 == VolumeType.Relative)
                {
                    relativeVolumeProportion = myRelativeVolumeProportion4;
                }
                else
                {
                    relativeVolumeProportion = -1.0f;	// Volume type & value combination not valid
                }
                return relativeVolumeProportion;
            }
            set
            {
                if (myVolumeTypeSpecifier4 == VolumeType.Relative)
                {
                    myRelativeVolumeProportion4 = value;
                }
            }
        }

        public int AbsoluteVolume_uL4
        {
            get
            {
                int absoluteVolume_uL = 0;
                if (myVolumeTypeSpecifier4 == VolumeType.Absolute)
                {
                    absoluteVolume_uL = myAbsoluteVolume_uL4;
                }
                else
                {
                    absoluteVolume_uL = -1;	// Volume type & value combination not valid
                }
                return absoluteVolume_uL;
            }
            set
            {
                if (myVolumeTypeSpecifier4 == VolumeType.Absolute)
                {
                    myAbsoluteVolume_uL4 = value;
                }
            }
        }

        public ProtocolGenericMultiStepsCommand()
        {

        }

        #region Mix
        public int TipTubeBottomGap_uL
        {
            get
            {
                int tipTubeBottomGap_uL = 0;
                if (VolumeTypeSpecifier == VolumeType.Absolute)
                {
                    tipTubeBottomGap_uL = myTipTubeBottomGap_uL;
                }
                return tipTubeBottomGap_uL;
            }
            set
            {
                if (VolumeTypeSpecifier == VolumeType.Absolute)
                {
                    myTipTubeBottomGap_uL = value;
                }
            }
        }

        public int MixCycles
        {
            get
            {
                return myMixCycles;
            }
            set
            {
                myMixCycles = value;
                //check for bad value
                if ((myMixCycles > 5) && (myMixCycles < 1))
                    myMixCycles = 1;
            }
        }
        

        #endregion

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

        public void SetVolumeCommandTypeParam( object volSpec)
        {
            if (volSpec as Tesla.DataAccess.absoluteVolume != null)
            {
                // Absolute volume specified
                this.VolumeTypeSpecifier = VolumeType.Absolute;
                DataAccess.absoluteVolume absVol = (Tesla.DataAccess.absoluteVolume)volSpec;
                this.AbsoluteVolume_uL = int.Parse(absVol.value_uL);
            }
            else if (volSpec as DataAccess.relativeVolume != null)
            {
                // Relative volume specified
                this.VolumeTypeSpecifier = VolumeType.Relative;
                DataAccess.relativeVolume relVol = (Tesla.DataAccess.relativeVolume)volSpec;
                this.RelativeVolumeProportion = (double)relVol.proportion;
            }
            else
            {
                // Absolute volume specified
                this.VolumeTypeSpecifier = VolumeType.Absolute;
                this.AbsoluteVolume_uL = 0;
            }
        }
        public void SetVolumeCommandTypeParam2( object volSpec)
        {
            if (volSpec as Tesla.DataAccess.absoluteVolume2 != null)
            {
                // Absolute volume specified
                this.VolumeTypeSpecifier2 = VolumeType.Absolute;
                DataAccess.absoluteVolume2 absVol = (Tesla.DataAccess.absoluteVolume2)volSpec;
                this.AbsoluteVolume_uL2 = int.Parse(absVol.value_uL);
            }
            else if (volSpec as DataAccess.relativeVolume2 != null)
            {
                // Relative volume specified
                this.VolumeTypeSpecifier2 = VolumeType.Relative;
                DataAccess.relativeVolume2 relVol = (Tesla.DataAccess.relativeVolume2)volSpec;
                this.RelativeVolumeProportion2 = (double)relVol.proportion;
            }
            else
            {
                // Absolute volume specified
                this.VolumeTypeSpecifier2 = VolumeType.Absolute;
                this.AbsoluteVolume_uL2 = 0;
            }
        }
        public void SetVolumeCommandTypeParam3(object volSpec)
        {
            if (volSpec as Tesla.DataAccess.absoluteVolume3 != null)
            {
                // Absolute volume specified
                this.VolumeTypeSpecifier3 = VolumeType.Absolute;
                DataAccess.absoluteVolume3 absVol = (Tesla.DataAccess.absoluteVolume3)volSpec;
                this.AbsoluteVolume_uL3 = int.Parse(absVol.value_uL);
            }
            else if (volSpec as DataAccess.relativeVolume3 != null)
            {
                // Relative volume specified
                this.VolumeTypeSpecifier3 = VolumeType.Relative;
                DataAccess.relativeVolume3 relVol = (Tesla.DataAccess.relativeVolume3)volSpec;
                this.RelativeVolumeProportion3 = (double)relVol.proportion;
            }
            else
            {
                // Absolute volume specified
                this.VolumeTypeSpecifier3 = VolumeType.Absolute;
                this.AbsoluteVolume_uL3 = 0;
            }
        }
        public void SetVolumeCommandTypeParam4(object volSpec)
        {
            if (volSpec as Tesla.DataAccess.absoluteVolume4 != null)
            {
                // Absolute volume specified
                this.VolumeTypeSpecifier4 = VolumeType.Absolute;
                DataAccess.absoluteVolume4 absVol = (Tesla.DataAccess.absoluteVolume4)volSpec;
                this.AbsoluteVolume_uL4 = int.Parse(absVol.value_uL);
            }
            else if (volSpec as DataAccess.relativeVolume4 != null)
            {
                // Relative volume specified
                this.VolumeTypeSpecifier4 = VolumeType.Relative;
                DataAccess.relativeVolume4 relVol = (Tesla.DataAccess.relativeVolume4)volSpec;
                this.RelativeVolumeProportion4 = (double)relVol.proportion;
            }
            else
            {
                // Absolute volume specified
                this.VolumeTypeSpecifier4 = VolumeType.Absolute;
                this.AbsoluteVolume_uL4 = 0;
            }

        }
    }




    public class ProtocolTopUpMixTransCommand : ProtocolGenericMultiStepsCommand
    {


    }
    public class ProtocolTopUpMixTransSepTransCommand : ProtocolGenericMultiStepsCommand
    {


    }

    public class ProtocolTopUpTransCommand : ProtocolGenericMultiStepsCommand
    {


    }
    public class ProtocolTopUpTransSepTransCommand : ProtocolGenericMultiStepsCommand
    {


    }
    public class ProtocolResusMixCommand : ProtocolGenericMultiStepsCommand
    {


    }
    public class ProtocolResusMixSepTransCommand : ProtocolGenericMultiStepsCommand
    {


    }
    public class ProtocolMixCommand : ProtocolBaseMixCommand
    {
    }
    public class ProtocolMixTransCommand : ProtocolBaseMixCommand
    {
        private AbsoluteResourceLocation mySourceVial2, myDestinationVial2;
        private VolumeType myVolumeTypeSpecifier2 = VolumeType.NotSpecified;
        private int myAbsoluteVolume_uL2 = -1;
        private double myRelativeVolumeProportion2 = -1.0f;

        public AbsoluteResourceLocation SourceVial2
        {
            get
            {
                return mySourceVial2;
            }
            set
            {
                mySourceVial2 = value;
            }
        }
        public AbsoluteResourceLocation DestinationVial2
        {
            get
            {
                return myDestinationVial2;
            }
            set
            {
                myDestinationVial2 = value;
            }
        }

        public VolumeType VolumeTypeSpecifier2
        {
            get
            {
                return myVolumeTypeSpecifier2;
            }
            set
            {
                myVolumeTypeSpecifier2 = value;
            }
        }

        public double RelativeVolumeProportion2
        {
            get
            {
                double relativeVolumeProportion = -1.0f;
                if (myVolumeTypeSpecifier2 == VolumeType.Relative)
                {
                    relativeVolumeProportion = myRelativeVolumeProportion2;
                }
                else
                {
                    relativeVolumeProportion = -1.0f;	// Volume type & value combination not valid
                }
                return relativeVolumeProportion;
            }
            set
            {
                if (myVolumeTypeSpecifier2 == VolumeType.Relative)
                {
                    myRelativeVolumeProportion2 = value;
                }
            }
        }

        public int AbsoluteVolume_uL2
        {
            get
            {
                int absoluteVolume_uL = 0;
                if (myVolumeTypeSpecifier2 == VolumeType.Absolute)
                {
                    absoluteVolume_uL = myAbsoluteVolume_uL2;
                }
                else
                {
                    absoluteVolume_uL = -1;	// Volume type & value combination not valid
                }
                return absoluteVolume_uL;
            }
            set
            {
                if (myVolumeTypeSpecifier2 == VolumeType.Absolute)
                {
                    myAbsoluteVolume_uL2 = value;
                }
            }
        }

        public void SetVolumeCommandTypeParam(object volSpec)
        {
            if (volSpec as Tesla.DataAccess.absoluteVolume != null)
            {
                // Absolute volume specified
                this.VolumeTypeSpecifier = VolumeType.Absolute;
                DataAccess.absoluteVolume absVol = (Tesla.DataAccess.absoluteVolume)volSpec;
                this.AbsoluteVolume_uL = int.Parse(absVol.value_uL);
            }
            else if (volSpec as DataAccess.relativeVolume != null)
            {
                // Relative volume specified
                this.VolumeTypeSpecifier = VolumeType.Relative;
                DataAccess.relativeVolume relVol = (Tesla.DataAccess.relativeVolume)volSpec;
                this.RelativeVolumeProportion = (double)relVol.proportion;
            }
            else
            {
                // Absolute volume specified
                this.VolumeTypeSpecifier = VolumeType.Absolute;
                this.AbsoluteVolume_uL = 0;
            }
        }
        public void SetVolumeCommandTypeParam2(object volSpec)
        {
            if (volSpec as Tesla.DataAccess.absoluteVolume2 != null)
            {
                // Absolute volume specified
                this.VolumeTypeSpecifier2 = VolumeType.Absolute;
                DataAccess.absoluteVolume2 absVol = (Tesla.DataAccess.absoluteVolume2)volSpec;
                this.AbsoluteVolume_uL2 = int.Parse(absVol.value_uL);
            }
            else if (volSpec as DataAccess.relativeVolume2 != null)
            {
                // Relative volume specified
                this.VolumeTypeSpecifier2 = VolumeType.Relative;
                DataAccess.relativeVolume2 relVol = (Tesla.DataAccess.relativeVolume2)volSpec;
                this.RelativeVolumeProportion2 = (double)relVol.proportion;
            }
            else
            {
                // Absolute volume specified
                this.VolumeTypeSpecifier2 = VolumeType.Absolute;
                this.AbsoluteVolume_uL2 = 0;
            }

        }
    }
}