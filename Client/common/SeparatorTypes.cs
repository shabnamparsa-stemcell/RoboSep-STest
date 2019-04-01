//----------------------------------------------------------------------------
// SeparatorTypes
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
// Notes:
//   03/24/05 - new enum and structs to support variable load list - bdr
//   05/09/05 - added Sample Tube ID to ReagentLotIdentifiers struct - bdr
//----------------------------------------------------------------------------

using System;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

using Invetech.ApplicationLog;

namespace Tesla.Common.Separator
{
	#region enums

	/// <summary>
	/// Carousel quadrants
	/// </summary>
	public enum QuadrantId	
	{
		NoQuadrant = -1,
		Quadrant1,
		Quadrant2,
		Quadrant3,
		Quadrant4,
		NUM_QUADRANTS
	}

	// Reference quadrant/instrument resources as relative locations, in the 
	// customer-preferred list order.
	public enum RelativeQuadrantLocation
	{
		SampleTube,
		VialA,	// Magnetic Nanoparticle Vial
		VialB,	// Selection Cocktail Vial		
		VialC,	// Antibody Cocktail Vial
		SeparationTube,	// 14mL tube - Magnet
		WasteTube,		// 50mL tube - Prime/Waste
		LysisBufferTube,// 50mL tube - Negative fraction/Lysis buffer
		TipsBox,		// Tips box!
		QuadrantBuffer,	// Buffer bottle
		NUM_RELATIVE_QUADRANT_LOCATIONS,
		START_LOCATION = SampleTube
	}

	/// <summary>
	/// Separator states
	/// </summary>
	public enum SeparatorState
	{
        Initialising,
		SeparatorUnloaded,
        NoSample,
        Selected, 
        Configured,
        Ready,
        Running,
		Paused,
		BatchHalted,
        BatchComplete,
		ShuttingDown,
		Shutdown,
		PauseCommand
	}	

	/// <summary>
	/// Instrument Control states
	/// </summary>
	public enum InstrumentState
	{
		RESET,
		INIT,
		IDLE,
		RUNNING,
		PAUSED,
		HALTED,
		SHUTTINGDOWN,
		SHUTDOWN,
		OFF,
		ESTOP,
		SERVICE,		// Added for completeness/consistency, but this state is currently 'orthogonal' to normal OperatorConsole operation!  That is, interaction/operation of both OperatorConsole and the Service application is undefined.
		PAUSECOMMAND
	}

	// Reference quadrant/instrument resources as absolute locations.
	// This enum must match the definition in the 'shared' file ProtocolConfig.txt.
	public enum AbsoluteResourceLocation
	{
		//
		// 0000 - 0099: Global instrument resources
		//
		//TPC0000, //RESERVED
		TPC0001, //BCCMContainer

		//
		// 0100 - 0199: Carousel Quadrant 1 resources
		//
		//TPC0100, //RESERVED
		TPC0101, //WasteVial
		TPC0102, //LysisVial
		TPC0103, //CocktailVial
		TPC0104, //ParticleVial
		TPC0105, //AntibodyVial
		TPC0106, //SampleVial
		TPC0107, //SeparationVial
		//TPC0108, //TipBox
		/*CR*/
		//TPC0108,//VialBpos
		//TPC0109,//VialBneg
		//TPC0110,//NegativeFraction

		//
		// 0200 - 0299: Carousel Quadrant 2 resources
		//
		//TPC0200, //RESERVED
		TPC0201, //WasteVial +7
		TPC0202, //LysisVial
		TPC0203, //CocktailVial
		TPC0204, //ParticleVial
		TPC0205, //AntibodyVial
		TPC0206, //SampleVial
		TPC0207, //SeparationVial
		//TPC0208, //TipBox
		/*CR*/
		//TPC0208,//VialBpos
		//TPC0209,//VialBneg
		//TPC0210,//NegativeFraction

		//
		// 0300 - 0399: Carousel Quadrant 3 resources
		//
		//TPC0300, //RESERVED
		TPC0301, //WasteVial +14
		TPC0302, //LysisVial
		TPC0303, //CocktailVial
		TPC0304, //ParticleVial
		TPC0305, //AntibodyVial
		TPC0306, //SampleVial
		TPC0307, //SeparationVial
		//TPC0308, //TipBox
		/*CR*/
		//TPC0308,//VialBpos
		//TPC0309,//VialBneg
		//TPC0310,//NegativeFraction

		//
		// 0400 - 0499: Carousel Quadrant 4 resources
		//
		//TPC0400, //RESERVED
		TPC0401, //WasteVial +21
		TPC0402, //LysisVial
		TPC0403, //CocktailVial
		TPC0404, //ParticleVial
		TPC0405, //AntibodyVial
		TPC0406, //SampleVial
		TPC0407, //SeparationVial
		//TPC0408, //TipBox
		/*CR*/
		//TPC0408,//VialBpos
		//TPC0409,//VialBneg
		//TPC0410,//NegativeFraction

		NUM_LOCATIONS
	}
	
	// consumables load order lists - bdr
	public enum LoadOrder  
	{
		StandardOrder,
		HumanPositive,
		HumanNegative,
		MousePositive,
		MouseNegative,
		WholeBlood,
		NUM_ORDERS
	}

	#endregion enums

	#region structs

	// NOTE: in future, refer to "Value Type Usage Guidelines" documentation in MSDN, and use 
	// using System.Double.Parse as an example, for ways of including operator overloads to get 
    // value semantics (would be useful in the case of this struct).

	// a struct to reposition consumables in the load list - bdr
	[Serializable]
	public struct LoadOrderItem {
		public string  label;
		public RelativeQuadrantLocation order;

		public LoadOrderItem(RelativeQuadrantLocation  psn, string lbl) 
		{order = psn; label = lbl; }
		}

	// bdr - test track spe-z
	[Serializable]
	public struct ResourceLoadTable	 {
		public LoadOrderItem[] table;
		public int		 tableSize;
		public int		 bfrLocation;
		public LoadOrder orderType;
        
		public ResourceLoadTable(LoadOrderItem[] tbl, int size, int locn, LoadOrder type) 
			{ 
			table = tbl;
			tableSize = size;
			bfrLocation = locn;
			orderType = type;
			}
		}
	
	[Serializable]
	public struct FluidVolume
	{
		private double			myVolume;
		private FluidVolumeUnit	myVolumeUnit;

		public FluidVolume(double amount, FluidVolumeUnit unit)
		{
			myVolume = amount;
			myVolumeUnit = FluidVolumeUnit.MilliLitres;	// Assume mL
			myVolumeUnit = unit;						// Adjust volume value if required
		}		

		public FluidVolumeUnit Unit
		{
			get
			{
				return myVolumeUnit;
			}
			set
			{
                if (myVolumeUnit != value)
                {                    
                    // Scale the current volume value as necessary to perform the required
                    // unit conversion.
                    myVolume *= (double)myVolumeUnit/(double)value;
                    myVolumeUnit = value;                 
                }             
			}
		}

		public double Amount
		{
			get
			{
				return myVolume;
			}
            set
            {
                myVolume = value;
            }
		}
	}

	[Serializable]
	public struct ProtocolConsumable
	{
		private bool			myIsRequired;
		private FluidVolume		myCapacity;
		private FluidVolume		myVolume;
		private QuadrantId		myAbsoluteQuadrant;

		// NOTE: for backward-compatibility with CDP (deprecated)
		public ProtocolConsumable(bool isRequired, FluidVolume capacity, FluidVolume volume)
		{
			myIsRequired = isRequired;
			myCapacity	 = capacity;
			myVolume	 = volume;
			myAbsoluteQuadrant = QuadrantId.NoQuadrant;
		}

		public bool IsRequired
		{
			get
			{
				return myIsRequired;
			}
			set
			{
				myIsRequired = value;
			}
		}

		public FluidVolume Capacity 
		{
			get
			{
				return myCapacity;
			}
			set
			{
				myCapacity = value;
			}
		}

		public FluidVolume Volume
		{
			get
			{
				return myVolume;
			}
			set
			{
				myVolume = value;
			}
		}		

		public QuadrantId AbsoluteQuadrant
		{
			get
			{
				return myAbsoluteQuadrant;
			}
			set
			{
				myAbsoluteQuadrant = value;
			}
		}
	}

	[Serializable]
	public struct ReagentLotIdentifiers
	{
		public string myCocktailLabel;
		public string myParticleLabel;			
		public string myAntibodyLabel;
		public string myLysisLabel;
        public string myBufferLabel;
		public string mySampleLabel;


		public void Reset()
		{
			myCocktailLabel = string.Empty;
			myParticleLabel = string.Empty;	
			myAntibodyLabel = string.Empty;
			myLysisLabel    = string.Empty;
            myBufferLabel = string.Empty;
			mySampleLabel	= string.Empty;
		}
	}

	#endregion structs

	#region types

    /// <summary>
    /// Helper class for use in Remoting connections in this application, including XML-RPC
    /// connections.
    /// </summary>
	public class RemotingInfo
	{
		private static string	theHttpPrefix = "http://";
        private static string   theTcpPrefix  = "tcp://";
        private static string   theLocalMachineName = "localhost";
        private static string   theLoopbackAddress = "127.0.0.1";
		private static int		theIcsServerPort = 8000;
		private static int		theSeparatorEventSinkPort = 8001;
        private static int      theSeparatorServerPort = 3149;
        private static string   theConfiguredFullyQualifiedDomainName;
		private static string	theMachineFullyQualifiedDomainName;
		private static string	theIcsEventSinkURI = "InstrumentControlEventSink";
        private static string   theSeparatorServerURI = "Separator";
		private static string	theIcsServerURL;
		private static string	theIcsEventSinkURL;
        private static string   theSeparatorServerURL;

		private static RemotingInfo	theRemotingInfo;	// Singleton instance variable

        public static RemotingInfo ConfigureInstance(string serverAddress)
        {
            theConfiguredFullyQualifiedDomainName = string.Empty;
            if (serverAddress != null && serverAddress != string.Empty)
            {
                theConfiguredFullyQualifiedDomainName = serverAddress;
            }
            return GetInstance();
        }

		public static RemotingInfo GetInstance()
		{
			if (theRemotingInfo == null)
			{				
				theRemotingInfo = new RemotingInfo();						
			}
			return theRemotingInfo;
		}

		private RemotingInfo()
		{
			// Determine the name of the machine on which we're running.
            // NOTE: while we use the term "fully qualified domain name" (FQDN), the value could 
            // either be the FQDN or the IP address for our purposes (and actually the latter
            // is recommended to avoid any problems connecting the service software via a
            // crossover cable).
			try
			{
                if (theConfiguredFullyQualifiedDomainName != null &&
                    theConfiguredFullyQualifiedDomainName != string.Empty)
                {
                    theMachineFullyQualifiedDomainName = theConfiguredFullyQualifiedDomainName;
                }
                else
                {
                    // Oct 1, 2013 Sunny commented out
                    // Dns.Resolve - Obsolete
                    // IPHostEntry hostInfo = Dns.Resolve(theLoopbackAddress);

                    IPHostEntry hostInfo = Dns.GetHostEntry(theLoopbackAddress);
                    theMachineFullyQualifiedDomainName = hostInfo.HostName;
                }

                bool isMachineFqdnUnspecified = theMachineFullyQualifiedDomainName == null ||
                    theMachineFullyQualifiedDomainName == string.Empty;
                string remotingAddress = isMachineFqdnUnspecified ? 
                    "<Not configured>" : theMachineFullyQualifiedDomainName;
                LogFile.AddMessage(TraceLevel.Verbose, "Remoting Address = " + remotingAddress);               
			}
			catch
			{
				theMachineFullyQualifiedDomainName = theLocalMachineName;
			}

			// Determine the Server and EventSink URLs
			theIcsServerURL = theHttpPrefix + theMachineFullyQualifiedDomainName + 
				":" + theIcsServerPort.ToString();
			theIcsEventSinkURL = theHttpPrefix + theMachineFullyQualifiedDomainName 
				+ ":" +	theSeparatorEventSinkPort.ToString() + "/" + theIcsEventSinkURI;

            theSeparatorServerURL = theTcpPrefix + theMachineFullyQualifiedDomainName
                + ":" + theSeparatorServerPort.ToString() + "/" + theSeparatorServerURI;

            LogFile.AddMessage(TraceLevel.Verbose, "ICS Server URL       = " + theIcsServerURL);
            LogFile.AddMessage(TraceLevel.Verbose, "ICS EventSink URL    = " + theIcsEventSinkURL);
            LogFile.AddMessage(TraceLevel.Verbose, "Separator Server URL =  " + theSeparatorServerURL);
		}

        public string ConfiguredFullyQualifiedDomainName
        {
            set
            {
                theConfiguredFullyQualifiedDomainName = value;
            }
        }

        public string HostDomainName
        {
            get
            {
                return theMachineFullyQualifiedDomainName;              
            }
        }

        public string LocalMachineName
        {
            get
            {
                return theLocalMachineName;
            }
        }

        public string LoopbackAddress
        {
            get
            {
                return theLoopbackAddress;
            }
        }

		public string IcsServerURL
		{
			get
			{
				return theIcsServerURL;
			}
		}

		public string IcsEventSinkURI
		{
			get
			{
				return theIcsEventSinkURI;
			}
		}

		public string IcsEventSinkURL
		{
			get
			{
				return theIcsEventSinkURL;
			}
		}

        public string SeparatorServerURI
        {
            get
            {
                return theSeparatorServerURI;
            }
        }

        public string SeparatorServerURL
        {
            get
            {
                return theSeparatorServerURL;
            }
        }
	}

	#endregion types
}
