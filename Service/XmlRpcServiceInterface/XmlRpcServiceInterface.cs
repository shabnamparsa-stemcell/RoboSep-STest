//----------------------------------------------------------------------------
// XmlRpcServiceInterface
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
using System.Collections.Specialized;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Serialization.Formatters;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Net;

using CookComputing.XmlRpc;

using Invetech.ApplicationLog;
using Tesla.InstrumentControl;

namespace Tesla.InstrumentControl
{

	/// <summary>
	/// Implementation for the "Separator" service; entry point for the Separator layer.
	/// </summary>
	/// <remarks>
	/// Singleton.
	/// </remarks>
	public class XmlRpcServiceInterface
	{
		// Default the server URL to use "localhost", but normal operation will override this
        // by calling SetConnectionSettings to set the machine name/IP address.
		private static string theServerURL = @"http://localhost:8000/";
		private static string theEventSinkURI = "ServiceEventSink";	
		private static string theEventSinkPort = "8003";	
		private static string theEventSinkServer = @"http://localhost:8003/";
		private static string theEventSinkURL = theEventSinkServer + theEventSinkURI;
		

		private static XmlRpcServiceInterface	theServiceInterface;	// Singleton instance variable

		private static IServiceEvents theServiceEventSink;
		private static ServiceXmlRpcProxy	myXmlRpcProxy;

		private static string connectionStatus = "Disconnected";
		/// <summary>
		/// Status of the connection to the instrument.
		/// </summary>
		public static string ConnectionStatus 
		{
			// NOTE: Future versions could consider using a heartbeat (ping?) to monitor 
            // connection status.
			get 
			{
				return connectionStatus; 
			}
			set 
			{
				connectionStatus = value;
				if (ReportCommsState != null)
				{
					ReportCommsState(connectionStatus);
				}			
			}
		}
		public static void SetConnectionSettings( string instrumentIP, string instrumentPort, string eventSinkIP, string eventSinkPort )
		{
			theServerURL = @"http://" + instrumentIP + ":" + instrumentPort + "/";

			theEventSinkPort = eventSinkPort;
			theEventSinkServer = @"http://" + eventSinkIP + ":" + theEventSinkPort + "/";
			theEventSinkURL = theEventSinkServer + theEventSinkURI;
		}

		public static XmlRpcServiceInterface GetInstance()
		{
			if (theServiceInterface == null)
			{
				try
				{
					ConnectionStatus  = "Connecting";
					theServiceInterface = new XmlRpcServiceInterface();
				}
				catch
				{
					theServiceInterface = null;
				}
			}
			return theServiceInterface;
		}

		/// <summary>
		/// 
		/// </summary>
		public static XmlRpcServiceInterface DestroyInstance()
		{
			if( theServiceInterface != null )
			{
				if (myXmlRpcProxy != null)
				{
					// Stop listening for communications errors in the proxy
					myXmlRpcProxy.CommunicationsError
						-= new CommunicationsErrorDelegate(theServiceInterface.AtCommunicationsError);

					ConnectionStatus  = "Disconnected";
					// Unsubscribe for Instrument Control Events   
					// First check if we are subscribed
					try
					{
						string[] subList = myXmlRpcProxy.GetSubscriberList();
						foreach( string url in subList )
						{
							if( url.Equals(theEventSinkURL) )
							{
								myXmlRpcProxy.Unsubscribe(theEventSinkURL);
							}
						}
					}
					catch (Exception)
					{
					}

				}

				ConnectionStatus  = "Disconnected";
				theServiceInterface = null;
			}
			return theServiceInterface;
		}

		private XmlRpcServiceInterface()
		{
			try
			{
				CreateEventSink();
				CreateProxy();

				// Successful, no exceptions thrown
				ConnectionStatus  = "Connected";
			}
			catch (Exception ex)
			{
				theServiceEventSink = null;
				myXmlRpcProxy = null;

				Console.WriteLine(ex.GetType());
				Console.WriteLine(ex.Message + "\n" + ex.InnerException);				
				Console.WriteLine(ex.StackTrace);		
		
				ConnectionStatus  = "Disconnected";
				throw new ApplicationException();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void CreateEventSink()
		{
			// -------------------
			// Register an event handler for IServiceEvents
			// Only register the service if it hasn't already been registered 

			// Retrive the array of objects registered as well known types at
			// the service end.
			WellKnownServiceTypeEntry[] myEntries =
				RemotingConfiguration.GetRegisteredWellKnownServiceTypes();
			bool found = false;

			foreach( WellKnownServiceTypeEntry srv in myEntries )
			{
				if( srv.ObjectUri.Equals(theEventSinkURI) )
				{
					// Service has already been registered
					found = true;
					break;
				}
			}
			if( !found )
			{
				// We are no longer using the .config file to configure the event sink 
				IDictionary properties = new Hashtable();
				properties["name"] = theEventSinkURI;
				properties["port"] = theEventSinkPort;

				IServerChannelSinkProvider serverSink = new XmlRpcServerFormatterSinkProvider(null,null);            
				SoapServerFormatterSinkProvider backupServerSink = new SoapServerFormatterSinkProvider();
				backupServerSink.TypeFilterLevel = TypeFilterLevel.Full;
				serverSink.Next = backupServerSink;

				IClientChannelSinkProvider clientSink = new SoapClientFormatterSinkProvider();

				ChannelServices.RegisterChannel(
					new HttpChannel(properties, clientSink,	serverSink)	);

				RemotingConfiguration.RegisterWellKnownServiceType(
					typeof(ServiceEventSink),
					theEventSinkURI,
					WellKnownObjectMode.Singleton );
			}
			
			// -------------------
			// Create the ServiceEventSink
			try
			{
				//	Console.WriteLine("Creating event sink");
				theServiceEventSink = 
					(IServiceEvents)Activator.GetObject(
					typeof(ServiceEventSink),
					theEventSinkURL
					);
				theServiceEventSink.ping();    // Call a method to trigger the server
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void CreateProxy()
		{
			// Make initial attempt to connect to the Instrument Control server.
			// Get the number of times to re-try if the connection attempt fails.
			int retryCount = 3;
			int retryWait = 2000;

			do
			{
				try
				{
					myXmlRpcProxy = new ServiceXmlRpcProxy(theServerURL);
					retryCount = 0;	// Clear the retry count as we're now connected.
				}
				catch (Exception ex)
				{
					// Connection attempt failed.  Re-try if the number of attempts so far is
					// within the retry count specified in the application configuration file,
					// otherwise propagate an exception to tell the caller we failed to connect.
					myXmlRpcProxy = null;
					Console.WriteLine("retryCount = " + retryCount);
					if (--retryCount <= 0)
					{	
						throw new ApplicationException("Failed to connect to Instrument Control server.", ex);
					}
					// Pause before retrying connection attempt
					Thread.Sleep(retryWait);
				}
			} while (retryCount > 0);

			try
			{
				if (myXmlRpcProxy != null)
				{
					// Listen for communications errors in the proxy
					myXmlRpcProxy.CommunicationsError
						+= new CommunicationsErrorDelegate(this.AtCommunicationsError);

					// Register for Instrument Control Events   
					// First check if we already subscribed on a previous connection attempt
					string[] subList = myXmlRpcProxy.GetSubscriberList();
					bool found = false;
					foreach( string url in subList )
					{
						if( url.Equals(theEventSinkURL) )
						{
							found = true;
							break;
						}
					}
					if( !found )
						myXmlRpcProxy.Subscribe(theEventSinkURL);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				myXmlRpcProxy = null;
				throw new ApplicationException(
					"Failed to subscribe for Instrument Control Events.", ex);
			}
		}

		/// <summary>
		/// Reports on the status of the instrument connection
		/// </summary>
		/// <returns>True if the connection to the instrument is present, else False.</returns>
		public bool IsConnected()
		{
			if( (myXmlRpcProxy != null) && (theServiceEventSink != null) )
				return true;

			return false;
		}

		private void AtCommunicationsError(string methodName, string message)
		{
			// We do not want to disconnect if Execute throws an exception, as this is most likely
			// just a case of an invalid command/argument, which may not be fatal.
			if ( ! methodName.Equals("Execute") )
			{
				DestroyInstance();
			}
		}

        public IServiceEvents ServiceEventSink
        {
            get
            {
                return theServiceEventSink as IServiceEvents;
            }
        }

		public ServiceXmlRpcProxy XmlRpcProxy
		{
			get
			{
				return myXmlRpcProxy;
			}
		}

		#region Events

		/// <remarks>
		/// These events are declared static so they can be used before a class
		/// is instantiated. This is acceptable as they are to be used in a singleton class.
		/// </remarks>

		static public event ReportCommsStateDelegate ReportCommsState;

		#endregion
	}
}
