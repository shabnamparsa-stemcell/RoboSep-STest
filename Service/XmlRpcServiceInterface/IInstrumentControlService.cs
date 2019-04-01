//----------------------------------------------------------------------------
// IInstrumentControlService
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
using System.Collections;
using CookComputing.XmlRpc;

namespace Tesla.InstrumentControl
{	
	/// <summary>
	/// Public XML-RPC API for the InstrumentControl subsystem
	/// </summary>
	/// <seealso>
	/// IInstrumentControlEvents
	/// </seealso>
	/// <remarks>
	/// This is the .NET description of the XML-RPC API published by the Instrument Control 
	/// subsystem.  That is, we redefine the methods and parameters in terms of .NET types where 
	/// possible, using features in the .NET XML-RPC implementation to help with data 
	/// transformation to/from .NET structures.
	/// XML-RPC methods must return a value.  Our convention for the Instrument Control
	/// interface is that methods will return bool (and a run-time value 'true') if there
	/// is no other return type.  
	/// </remarks>
	public interface IInstrumentControlService
	{
		#region Instrument Control XML-RPC Server

		/// <summary>
		/// Verify whether we can communicate with the Instrument Control XML-RPC server
		/// </summary>
		/// <returns>True if the Instrument Control server is running.</returns>
		bool		Ping();

		/// <summary>
		/// Tell the Instrument Control XML-RPC Server to execute a controlled shut-down.
		/// </summary>
		/// <returns>
		/// Successful Halt method invocation: true; unsuccessful invocation: false.
		/// </returns>
		/// <remarks>
		/// This method is exposed by the XML-RPC server but is intended for "internal" use only.		
		/// </remarks>
		bool		Halt();

		/// <summary>
		/// Get descriptive information on the Instrument Control XML-RPC Server.
		/// </summary>
		/// <returns>
		/// An untyped (object) array containing (string) gateway URL, (string) server version, (int) uptime in 
		/// seconds.
		/// </returns>
		Object[]	GetServerInfo();

		/// <summary>
		/// Register an IInstrumentControlEvents event sink (XML-RPC server) with the Instrument Control XML-RPC 
		/// server.
		/// </summary>
		/// <param name="eventSinkURL"></param>
		/// <returns>
		/// Successful subscription: true; unsuccessful subscription: false.
		/// </returns>
		bool		Subscribe(string eventSinkURL);

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		string[]	GetSubscriberList();

		/// <summary>
		/// Cancel registration of an IInstrumentControlEvents event sink (XML-RPC server) with the Instrument 
		/// Control XML-RPC server.
		/// </summary>
		/// <param name="eventSinkURL"></param>
		/// <returns>
		/// Successful subscription cancellation: true; unsuccessful subscription cancellation: false.
		/// </returns>
		
		bool		Unsubscribe(string eventSinkURL);

		#endregion Instrument Control XML-RPC Server		

		#region Instrument

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		string[]	GetErrorLog(int numEntries, int entryOffset);
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		string		GetInstrumentState();

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		bool		Shutdown();

		#endregion Instrument
	}
}
