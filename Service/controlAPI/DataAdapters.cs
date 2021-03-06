//----------------------------------------------------------------------------
// DataAdapters
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
// Copyright � 2004. All Rights Reserved.
//
//  03/24/06 - Uniform Multi Sample - RL
//	03/29/06 - Short Description for Sample Volume Dialog - RL
//----------------------------------------------------------------------------
namespace Tesla.InstrumentControl
{
	#region XML-RPC data adapters 
	
	/// <summary>
	/// 
	/// </summary>
	public struct XmlRpcContainer
	{
		/// <summary>
		/// 
		/// </summary>
		public double	bulkBufferDeadVolume;
	}

	/// <summary>
	/// 
	/// </summary>
	public struct XmlRpcProtocol
	{
		/// <summary>
		/// 
		/// </summary>
		public int		ID;	

		/// <summary>
		/// 
		/// </summary>
		public string	label;		

		/// <summary>
		/// 
		/// </summary>
		public string	description;	
		
		/// <summary>
		/// 
		/// </summary>
		public double	minVol;

		/// <summary>
		/// 
		/// </summary>
		public double	maxVol;

		/// <summary>
		/// 
		/// </summary>
		public string	protocolClass;

		/// <summary>
		/// 
		/// </summary>
		public int		numQuadrants;			
	}

	/// <summary>
	/// 
	/// </summary>
	public struct XmlRpcConsumables
	{		
		/// <summary>
		/// 
		/// </summary>
		public double	particleVolume;

		/// <summary>
		/// 
		/// </summary>
		public double	lysisVolume;

		/// <summary>
		/// 
		/// </summary>
		public double	antibodyVolume;
        
		/// <summary>
		/// 
		/// </summary>
		public double	cocktailVolume;

		/// <summary>
		/// 
		/// </summary>
		public double	bulkBufferVolume;

		/// <summary>
		/// 
		/// </summary>
		public bool		wasteVesselRequired;

		/// <summary>
		/// 
		/// </summary>
		public bool		separationVesselRequired;

		/// <summary>
		/// 
		/// </summary>
		public bool		sampleVesselRequired;

		/// <summary>
		/// 
		/// </summary>
		public bool		sampleVesselVolumeRequired;

		/// <summary>
		/// 
		/// </summary>
		public bool		tipBoxRequired;
	}

	/// <summary>
	/// 
	/// </summary>
	public struct XmlRpcSample
	{
		/// <summary>
		/// 
		/// </summary>
		public int		ID;

		/// <summary>
		/// 
		/// </summary>
		public string	label;

		/// <summary>
		/// 
		/// </summary>
		public int		protocolID;

		/// <summary>
		/// 
		/// </summary>
		public double	volume_uL;

		/// <summary>
		/// 
		/// </summary>
		public int		initialQuadrant;

		/// <summary>
		/// 
		/// </summary>
		public string	cocktailLabel;

		/// <summary>
		/// 
		/// </summary>
		public string	particleLabel;

		/// <summary>
		/// 
		/// </summary>
		public string	lysisLabel;

		/// <summary>
		/// 
		/// </summary>
		public string	antibodyLabel;
        
		/// <summary>
		/// 
		/// </summary>
		public string	sample1Label;

		/// <summary>
		/// 
		/// </summary>
		public string	sample2Label;
	}

	/// <summary>
	/// 
	/// </summary>
	public struct XmlRpcMaintenanceTask
	{
		/// <summary>
		/// 
		/// </summary>
		public string	taskID;

		/// <summary>
		/// 
		/// </summary>
		public string	taskDescription;
	}

	#endregion XML-RPC data adapters
}