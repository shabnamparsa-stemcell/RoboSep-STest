//----------------------------------------------------------------------------
// ProtocolCommandTypes
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
//	03/29/06 - pause command - RL
//	09/05/07 - usebuffertip command - bdr
//
//----------------------------------------------------------------------------
//
// 2011-09-05 to 2011-09-16 sp various changes
//     - add checking for recommended volume levels and provide warnings
//     - add volume level thresholds (recommended and acceptable) using parameter file entries instead of fixed code  
//
//----------------------------------------------------------------------------

using System;

using Tesla.Common.Separator;

namespace Tesla.Common.ProtocolCommand
{
	#region enums

	public enum SubCommand
	{
		HomeAllCommand = 0,
		DemoCommand,
		IncubateCommand,
		SeparateCommand,
		MixCommand,	
		TransportCommand,				
		TopUpVialCommand,
		ResuspendVialCommand,
		FlushCommand,
		PrimeCommand,
		PauseCommand,
		PumpLifeCommand,
        TopUpMixTransSepTransCommand,
        TopUpMixTransCommand,
        TopUpTransSepTransCommand,
        TopUpTransCommand,
        ResusMixSepTransCommand,
        ResusMixCommand,
        MixTransCommand
                        		
	}

    public enum VolumeType
    {
        NotSpecified,
        Relative,
        Absolute
    }

    // added 2011-09-06 sp
    // provide status of volume levels checking
    public enum VolumeCheck
    {
        VOLUMES_UNCHECKED,
        VOLUMES_VALID,
        VOLUMES_INFO,
        VOLUMES_WARNINGS,
        VOLUMES_INVALID
    }

	#endregion enums

	#region interfaces

	#region Command base type

	public interface IProtocolCommand
	{
		SubCommand CommandSubtype
		{
			get;
		}

        uint CommandSequenceNumber
        {
            get;
            set;
        }

        string CommandLabel
		{
			get;
			set;
		}		

		uint CommandExtensionTime
		{
			get;
			set;
		}

      	string CommandSummary
		{
			get;
		}

        // added 2011-09-06 sp
        // provide status to indicate if the volume levels are correct
        VolumeCheck CommandCheckStatus
        {
            get;
            set;
        }

        // added 2011-09-09 sp
        // provide status informatiion regarding the volume levels
        string CommandStatus
        {
            get;
            set;
        }

        VolumeError validateCommandAndUpdateCommandStatus(VolumeLimits volumeLimits, int sampleVolumeMin, int sampleVolumeMax);
    }

	#endregion Command base type

	#region Service command type

	public interface IServiceCommand : IProtocolCommand
	{
	}

	#endregion Service command type

	#region Wait command type

	public interface IWaitCommand : IProtocolCommand
	{
		uint WaitCommandTimeDuration
		{
			get;
			set;
		}
	}

	#endregion Wait command type

	#region Volume command type

	public interface IVolumeCommand : IProtocolCommand
	{		
		AbsoluteResourceLocation SourceVial
		{
			get;
			set;
		}

		AbsoluteResourceLocation DestinationVial
		{
			get;
			set;
		}

		VolumeType VolumeTypeSpecifier
		{
			get;
			set;
		}

		double RelativeVolumeProportion
		{
			get;
			set;
		}

		int AbsoluteVolume_uL
		{
			get;
			set;
		}
      	int TipRack
		{
			get;
			set;
		}
      	bool TipRackSpecified
		{
			get;
			set;
		}
	}

	#endregion Volume command type

	#region Volume command type (with working volume)

	public interface IWorkingVolumeCommand : IVolumeCommand
	{				
		bool FreeAirDispense
		{
			get;
			set;
		}

		int WorkingVolume_uL
		{
			get;
			set;
		}

		bool UseBufferTip 
		{
			get;
			set;
		}
	}

	#endregion Volume command type (with working volume)

	#region Volume Maintenance command type

	public interface IVolumeMaintenanceCommand : IVolumeCommand
	{
		bool HomeFlag
		{
			get;
			set;
		}
	}

	#endregion Volume Maintenance command type


    public interface IMultiSrcDestCommand : IProtocolCommand
    {
        AbsoluteResourceLocation SourceVial
        {
            get;
            set;
        }

        AbsoluteResourceLocation DestinationVial
        {
            get;
            set;
        }

        AbsoluteResourceLocation SourceVial2
        {
            get;
            set;
        }

        AbsoluteResourceLocation DestinationVial2
        {
            get;
            set;
        }

        AbsoluteResourceLocation SourceVial3
        {
            get;
            set;
        }

        AbsoluteResourceLocation DestinationVial3
        {
            get;
            set;
        }
    }

	#endregion interfaces
}
