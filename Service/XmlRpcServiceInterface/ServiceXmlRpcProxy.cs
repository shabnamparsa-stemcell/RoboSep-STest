//----------------------------------------------------------------------------
// ServiceXmlRpcProxy
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
using CookComputing.XmlRpc;

namespace Tesla.InstrumentControl
{
	public delegate void CommunicationsErrorDelegate( string methodName, string errorMessage );

	/// <summary>
	/// XML-RPC client that provides access to the InstrumentControl layer.
	/// </summary>
	/// <remarks>
	/// Application and other exceptions should not be caught here - they should be 
	/// allowed to bubble up to the parent so that it can decide what to do if there
	/// is any problem with communication in either direction.
	/// </remarks>
	public class ServiceXmlRpcProxy : XmlRpcClientProtocol, IInstrumentControlService
	{
    
		/// <summary>
		/// 
		/// </summary>
		/// <param name="icServerURL"></param>
		public ServiceXmlRpcProxy(string icServerURL)
		{
			// Set connection parameters
			this.Url = icServerURL;

			// Test the connection (throws an exception on failure)
			Ping();
		}

		#region ErrorManagement

		public event CommunicationsErrorDelegate CommunicationsError;

		private Exception HandleException(Exception ex, string methodName)
		{
			// Start with a generic error message
			string message = ex.Message;

			// If it is a know exception type, extract some more detailed information.
			CookComputing.XmlRpc.XmlRpcFaultException faultEx = ex as CookComputing.XmlRpc.XmlRpcFaultException;
			if (faultEx != null)
			{
				message += "\n    FaultCode: " +	faultEx.FaultCode +
					",\n    FaultString: " + faultEx.FaultString;
			}

			// Fire an event to inform higher layers of the error
			if( CommunicationsError != null )
			{
				CommunicationsError(methodName, message);
			}

			// the updated exception
			return new Exception(message,ex);
		}

		#endregion ErrorManagement

		#region IInstrumentControl Members

		// NOTE: for method documentation, refer to comments in IInstrumentControl.cs

		#region Instrument Control XML-RPC Server
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[XmlRpcMethod("ping")]
		public bool Ping()
		{	
			try
			{
				bool echoReceived = (bool)Invoke("Ping");
				if ( ! echoReceived)
				{
					throw new ApplicationException("Connected to Instrument Control server but failed connection test.");
				}            
				return echoReceived;
			}
			catch( Exception ex )
			{
				throw HandleException(ex,"Ping");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// This method is for internal use only (undocumented in the Instrument Interface documentation).
		/// </remarks>
		[XmlRpcMethod("halt")]
		public bool Halt()
		{
            try
            {
                return (bool)Invoke("Halt");
            }
			catch( Exception ex )
			{
				throw HandleException(ex,"Halt");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[XmlRpcMethod("getServerInfo")]
		public object[] GetServerInfo()
		{
			object[] info;
            try
            {
                info = (object[])Invoke("GetServerInfo");
            }
			catch( Exception ex )
			{
				throw HandleException(ex,"GetServerInfo");
			}
            if (info == null)
            {
                throw new ApplicationException("Failed to GetServerInfo");
            }
            return info;
			//				// getServerInfo
			//				Object[] info = theInstrument.GetServerInfo();
			//				Console.WriteLine(info[0].ToString());
			//				Console.WriteLine(info[1].ToString());
			//				Console.WriteLine(info[2].ToString());
			//				//Tesla.InstrumentControl.ServerInfo info = (Tesla.InstrumentControl.ServerInfo)theInstrument.GetServerInfo();
			//				//Console.WriteLine("serverInfo: " + info.URL + " " + info.serverVersion + " " + info.uptimeInSeconds);
			//				Console.WriteLine();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventSinkURL"></param>
		/// <returns></returns>
		[XmlRpcMethod("subscribe")]
		public bool Subscribe(string eventSinkURL)
		{
			bool subscribed = false;
            try
            {           
				subscribed = (bool)Invoke("Subscribe", new object[1]{eventSinkURL});
            }
			catch( Exception ex )
			{
				throw HandleException(ex,"Subscribe");
			}
            if ( ! subscribed)
            {
                throw new ApplicationException(
                    String.Format("Failed to subscribe Instrument Control to {0}", eventSinkURL));
            }             
            return subscribed;
			//
            //				// subscribe
            //				bool subscribeResult = theInstrument.Subscribe(/*subscriptionURL*/eventSinkURL);
            //				Console.WriteLine("subscribeResult = " + subscribeResult);
            //				Console.WriteLine();
            //
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[XmlRpcMethod("getSubscriberList")]
		public string[] GetSubscriberList()
		{
            try
            {
                // Subscriber list may be null, so no need to error check the result here
                return (string[])Invoke("GetSubscriberList");
            }
			catch( Exception ex )
			{
				throw HandleException(ex,"GetSubscriberList");
			}
			//				// getSubscriberList
			//				subscribers = theInstrument.GetSubscriberList();
			//				Console.WriteLine("InstrumentControl subscriber list:");
			//				foreach (string subscriber in subscribers)
			//				{
			//					Console.WriteLine(subscriber);
			//				}
			//				Console.WriteLine();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventSinkURL"></param>
		/// <returns></returns>
		[XmlRpcMethod("unsubscribe")]
		public bool Unsubscribe(string eventSinkURL)
		{
			bool unsubscribed = false;
            try
            {
				unsubscribed = (bool)Invoke("Unsubscribe", new object[1]{eventSinkURL});
            }
			catch( Exception ex )
			{
				throw HandleException(ex,"Unsubscribe");
			}
            if ( ! unsubscribed)
            {
                throw new ApplicationException(
                    String.Format("Failed to unsubscribe Instrument Control from {0}", eventSinkURL));
            }
            return unsubscribed;
			//				// unsubscribe
			//				bool unsubscribeResult = theInstrument.Unsubscribe(/*subscriptionURL*/eventSinkURL);
			//				Console.WriteLine("unsubscribeResult = " + unsubscribeResult);
			//				Console.WriteLine();
			//
		}

		
		/// <summary>
		/// Request the instrument to enter service state.
		/// This is only possible from some states.
		/// </summary>
		/// <returns></returns>
		[XmlRpcMethod("enterServiceState")]
		public bool EnterServiceState()
		{
			bool enterServiceState = false;
			try
			{
				// This request can be denied, so check result
				enterServiceState = (bool)Invoke("EnterServiceState");
			}
			catch( Exception ex )
			{
				throw HandleException(ex,"EnterServiceState");
			}
			if ( ! enterServiceState)
			{
				throw new ApplicationException(
					String.Format("Failed to enter service state. Request Denied."));
			}             
			return enterServiceState;
		}

		/// <summary>
		/// Request the instrument to exit service state.
		/// OUTDATED: If successful, it will return to idle state.
		/// </summary>
		/// <returns></returns>
		[XmlRpcMethod("exitServiceState")]
		public bool ExitServiceState()
		{
			bool leftServiceState = false;
			try
			{
				leftServiceState = (bool)Invoke("ExitServiceState");

			}
			catch( Exception ex )
			{
				throw HandleException(ex,"ExitServiceState");
			}
			if ( ! leftServiceState)
			{
				throw new ApplicationException(
					String.Format("Failed to leave service state."));
			}             
			return leftServiceState;
		}

		/// <summary>
		/// Execute a service command.
		/// </summary>
		/// <returns>success</returns>
		[XmlRpcMethod("execute")]
		public string Execute(string sObject, string sMethod, string sArgs)
		{
			try
			{
				// Subscriber list may be null, so no need to error check the result here
				string success = (string)Invoke("Execute", new object[3]{sObject, sMethod, sArgs});

				return success;
			}
			catch( Exception ex )
			{
				throw HandleException(ex,"Execute");
			}
		}

		#endregion Instrument Control XML-RPC Server

		#region Instrument

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[XmlRpcMethod("getErrorLog")]
		public string[] GetErrorLog(int numEntries, int entryOffset)
		{
			try
			{
				return (string[])Invoke("GetErrorLog", new object[2]{numEntries, entryOffset});				           		
			}
			catch( Exception ex )
			{
				throw HandleException(ex,"GetErrorLog");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[XmlRpcMethod("getInstrumentState")]
		public string GetInstrumentState()
		{
			try
			{
				return (string)Invoke("GetInstrumentState");				           		
			}
			catch( Exception ex )
			{
				throw HandleException(ex,"GetInstrumentState");
			}
		}
		

		/// <summary>
		/// 
		/// </summary>
		[XmlRpcMethod("shutdown")]
		public bool Shutdown()
		{
			try
			{
				return (bool)Invoke("Shutdown");				           		
			}
			catch( Exception ex )
			{
				throw HandleException(ex,"Shutdown");
			}
		}

        [XmlRpcMethod("GetIgnoreLidSensor")]
        public bool GetIgnoreLidSensor()	
        {
            bool result = false;
            try
            {
                result = (bool)Invoke("GetIgnoreLidSensor");
            }
            catch
            {
            }
            return result;
        }
        [XmlRpcMethod("SetIgnoreLidSensor")]
        public int SetIgnoreLidSensor(bool isOn)
        {
            int result = 0;
            try
            {
                result = (int)Invoke("SetIgnoreLidSensor", new object[1] { isOn?1:0 });
            }
            catch
            {
            }
            return result;
        }
        [XmlRpcMethod("GetTimedStart")]
        public bool GetTimedStart()
        {
            bool result = false;
            try
            {
                result = (bool)Invoke("GetTimedStart");
            }
            catch
            {
            }
            return result;
        }
        [XmlRpcMethod("SetTimedStart")]
        public int SetTimedStart(bool isOn)
        {
            int result = 0;
            try
            {
                result = (int)Invoke("SetTimedStart", new object[1] { isOn ? 1 : 0 });
            }
            catch
            {
            }
            return result;
        }

        //factory stuff
        [XmlRpcMethod("SaveIniAsFactory")]
        public bool SaveIniAsFactory()
        {
            bool result = false;
            try
            {
                result = (bool)Invoke("SaveIniAsFactory");
            }
            catch
            {
            }
            return result;
        }
        [XmlRpcMethod("RestoreIniWithFactory")]
        public bool RestoreIniWithFactory()
        {
            bool result = false;
            try
            {
                result = (bool)Invoke("RestoreIniWithFactory");
            }
            catch
            {
            }
            return result;
        }

        [XmlRpcMethod("LoadSelectedHardwareINI")]
        public bool LoadSelectedHardwareINI(string path)
        {
            bool result = false;
            try
            {
                result = (bool)Invoke("LoadSelectedHardwareINI", new object[1] { path });
            }
            catch
            {
            }
            return result;
        }
		#endregion Instrument

		#endregion

        [XmlRpcMethod("GetInstrumentAxisStatusSet")]
        public string GetInstrumentAxisStatusSet(int combo)
        {
            try
            {
                return (string)Invoke("GetInstrumentAxisStatusSet", new object[1] { combo });
            }
            catch (Exception ex)
            {
                throw HandleException(ex, "GetInstrumentAxisStatusSet");
            }
        }


        [XmlRpcMethod("SetBarcodeRescanOffset")]
        public int SetBarcodeRescanOffset(double offset)
        {
            int result = 0;
            try
            {
                result = (int)Invoke("SetBarcodeRescanOffset", new object[1] { offset });
            }
            catch
            {
            }
            return result;
        }

        [XmlRpcMethod("GetBarcodeRescanOffset")]
        public double GetBarcodeRescanOffset()
        {
            double result = 0.0;
            try
            {
                result = (double)Invoke("GetBarcodeRescanOffset");
            }
            catch
            {
            }
            return result;
        }
        [XmlRpcMethod("SetBarcodeThetaOffset")]
        public int SetBarcodeThetaOffset(double offset)
        {
            int result = 0;
            try
            {
                result = (int)Invoke("SetBarcodeThetaOffset", new object[1] { offset });
            }
            catch
            {
            }
            return result;
        }

        [XmlRpcMethod("GetBarcodeThetaOffset")]
        public double GetBarcodeThetaOffset()
        {
            double result = 0.0;
            try
            {
                result = (double)Invoke("GetBarcodeThetaOffset");
            }
            catch
            {
            }
            return result;
        }
    }
}
