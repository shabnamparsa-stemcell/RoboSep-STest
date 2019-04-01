//----------------------------------------------------------------------------
// InstrumentControlXmlRpcProxy
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
//    03/24/05 - added parkarm() and islidclosed() - bdr
// 	  04/11/05 - added parkpump to deenergize pump - bdr
//
//----------------------------------------------------------------------------

#define bdr31  // park arm pump
#define bdr32  // lid closed

using System;
using System.Diagnostics;
using System.Text;
using CookComputing.XmlRpc;

using Invetech.ApplicationLog;

namespace Tesla.InstrumentControl
{
	/// <summary>
	/// XML-RPC client that provides access to the InstrumentControl layer.
	/// </summary>
	/// <remarks>
	/// Application and other exceptions should not be caught here - they should be 
	/// allowed to bubble up to the parent so that it can decide what to do if there
	/// is any problem with communication in either direction.
	/// </remarks>
	public class InstrumentControlXmlRpcProxy : XmlRpcClientProtocol, IInstrumentControl
	{
        #region Construction/destruction

		/// <summary>
		/// 
		/// </summary>
		/// <param name="icServerURL"></param>
		public InstrumentControlXmlRpcProxy(string icServerURL)
		{
			// Set connection parameters
			this.Url = icServerURL;

			// Test the connection (throws an exception on failure)
            bool echoReceived = false;
            try
            {
                echoReceived = Ping();
            }
            finally
            {
                LogFile.AddMessage(TraceLevel.Verbose, 
                    "Instrument Control API Ping() returned " + 
                    echoReceived.ToString());
            }

		}

        #endregion Construction/destruction

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
            finally
            {
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
            finally
            {
            }
		}

        #if bdr31
		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// This method is for internal use only (undocumented in the Instrument Interface documentation).
		/// </remarks>
		[XmlRpcMethod("parkArm")]
		public bool ParkArm()	// bdr
		{
            try
            {
        		System.Diagnostics.Debug.WriteLine("QMgr  - ICPxyXmlRpc.parkArm"); // bdr
                return (bool)Invoke("ParkArm");
            }
            finally
            {
            }
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// This method is for internal use only (undocumented in the Instrument Interface documentation).
		/// </remarks>
		[XmlRpcMethod("parkPump")]
		public bool ParkPump()	// bdr
		{
            try
            {
        		System.Diagnostics.Debug.WriteLine("QMgr  - ICPxyXmlRpc.parkPump"); // bdr
                return (bool)Invoke("ParkPump");
            }
            finally
            {
            }
		}
        #endif
        
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[XmlRpcMethod("getServerInfo")]
		public string[] GetServerInfo()
		{
            try
            {
                string[] info = (string[])Invoke("GetServerInfo");
                if (info == null)
                {
                    throw new ApplicationException("Failed to GetServerInfo");
                }
                return info;
            }
            finally
            {
            }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventSinkURL"></param>
		/// <returns></returns>
		[XmlRpcMethod("subscribe")]
		public bool Subscribe(string eventSinkURL)
		{
            try
            {           
				bool subscribed = (bool)Invoke("Subscribe", new object[1]{eventSinkURL});
                if ( ! subscribed)
                {
                    throw new ApplicationException(
                        String.Format("Failed to subscribe Instrument Control to {0}", eventSinkURL));
                }             
                return subscribed;
            }
            finally
            {
            }
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
            finally
            {
            }			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventSinkURL"></param>
		/// <returns></returns>
		[XmlRpcMethod("unsubscribe")]
		public bool Unsubscribe(string eventSinkURL)
		{
            try
            {
				bool unsubscribed = (bool)Invoke("Unsubscribe", new object[1]{eventSinkURL});
                if ( ! unsubscribed)
                {
                    throw new ApplicationException(
                        String.Format("Failed to unsubscribe Instrument Control from {0}", eventSinkURL));
                }
                return unsubscribed;
            }
            finally
            {
            }			
		}

		#endregion Instrument Control XML-RPC Server

		#region Run

		/// <summary>
		/// 
		/// </summary>
		/// <param name="samples"></param>
		/// <returns></returns>
		[XmlRpcMethod("scheduleRun")]
		public DateTime ScheduleRun(XmlRpcSample[] samples)
		{
			try
			{                
                LogFile.AddMessage(TraceLevel.Info, FormatSamplesInfo(samples));
				return (DateTime)Invoke("ScheduleRun", samples);				
			}
			finally
			{
			}
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[XmlRpcMethod("startRun")]
		public DateTime StartRun(XmlRpcSample[] samples, string batchRunUserId,bool isSharing, int[] sharedSectorsTranslation, int[]sharedSectorsProtocolIndex)
		{
			try
			{
                LogFile.AddMessage(TraceLevel.Info, FormatSamplesInfo(samples));
				return (DateTime)Invoke("StartRun", samples, batchRunUserId,
					isSharing,sharedSectorsTranslation,sharedSectorsProtocolIndex);
			}
			finally
			{
			}			
		}

		/// <summary>
		/// 
		/// </summary>
		[XmlRpcMethod("pauseRun")]
		public bool PauseRun()
		{
			try
			{
				return (bool)Invoke("PauseRun");				
			}
			finally
			{
			}	
		}

		/// <summary>
		/// 
		/// </summary>
		[XmlRpcMethod("resumeRun")]
		public bool ResumeRun()
		{
			try
			{
				return (bool)Invoke("ResumeRun");				
			}
			finally
			{
			}	
		}

		/// <summary>
		/// 
		/// </summary>
		[XmlRpcMethod("haltRun")]
		public bool HaltRun()
		{	
			try
			{
				return (bool)Invoke("HaltRun");				
			}
			finally
			{
			}	
		}

		/// <summary>
		/// 
		/// </summary>
		[XmlRpcMethod("abortRun")]
		public bool AbortRun()
		{
			try
			{
				return (bool)Invoke("AbortRun");				           		
			}
			finally
			{
			}			
		}

		#endregion Run

		#region Protocol

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[XmlRpcMethod("getProtocols")]
		public XmlRpcProtocol[] GetProtocols()
		{
			try
			{
				// Protocols list may be null, so no need to error check the result here
				return (XmlRpcProtocol[])Invoke("GetProtocols");
			}
			finally
			{
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[XmlRpcMethod("getCustomNames")]
/*CR*/	public string[][] GetCustomNames()
		{
			try
			{
				// Protocols list may be null, so no need to error check the result here
				return (string[][])Invoke("GetCustomNames");
			}
			finally
			{
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[XmlRpcMethod("getResultChecks")]
		public bool[][] GetResultChecks()
		{
			try
			{
				// Protocols list may be null, so no need to error check the result here
				return (bool[][])Invoke("GetResultChecks");
			}
			finally
			{
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="protocolId"></param>
		/// <param name="sampleVolume_ul"></param>
		/// <returns></returns>
		[XmlRpcMethod("calculateConsumables")]
		public XmlRpcConsumables[] CalculateConsumables(int protocolId, double sampleVolume_ul)
		{
			try
			{				
				return (XmlRpcConsumables[])Invoke("CalculateConsumables", new object[2]{protocolId, sampleVolume_ul});
			}						
			finally
			{
			}					
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[XmlRpcMethod("reloadProtocols")]
		public bool ReloadProtocols()
		{
			try
			{
				return (bool)Invoke("ReloadProtocols");
			}
			finally
			{
			}
		}

		#endregion Protocol

		#region Instrument

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[XmlRpcMethod("getErrorLog")]
		public string[] GetErrorLog()
		{
			return null;
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
			finally
			{

            }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[XmlRpcMethod("getContainerVolumes")]
		public XmlRpcContainer GetContainerVolumes()
		{
			try
			{
				return (XmlRpcContainer)Invoke("GetContainerVolumes");
			}
			finally
			{
			}
		}

        /// <summary>
        /// Test if the hydraulic fluid needs refilling.
        /// </summary>
        /// <returns></returns>
        [XmlRpcMethod("isHydroFluidLow")]
        public bool IsHydroFluidLow()
        {
            try
            {
                return (bool)Invoke("IsHydroFluidLow");
            }
            finally
            {
            }
        }

        #if bdr32
        /// <summary>
        /// Test if the lid is closed
        /// </summary>
        /// <returns></returns>
        [XmlRpcMethod("isLidClosed")] 
        public bool IsLidClosed()	  // bdr
	        {
			bool isLidClosed = false;
            try {
				isLidClosed = (bool) Invoke("IsLidClosed");
	            }
			finally 
			{ 
			}
			return isLidClosed;
	        }
        #endif

		//RL - pause command - 03/29/06
		/// <summary>
		/// Pause Command
		/// </summary>
		/// <returns></returns>
		[XmlRpcMethod("isPauseCommand")] 
		public bool isPauseCommand()
		{
			bool isPauseCommand=false;
			try
			{
				isPauseCommand = (bool) Invoke("isPauseCommand");
				return isPauseCommand;
			}
			finally { }
		}

        [XmlRpcMethod("SetIgnoreLidSensor")]                  //CWJ
        public int SetIgnoreLidSensor(int sw)
        {
            int rtn = -1;
            try
            {
                rtn = (int)Invoke("SetIgnoreLidSensor", new object[1] { sw });
                return rtn;

            }
            finally { }
        }

        [XmlRpcMethod("GetIgnoreLidSensor")]                  //CWJ
        public bool GetIgnoreLidSensor()
        {
            bool rtn = false;
            try
            {
                rtn = (bool)Invoke("GetIgnoreLidSensor");
                return rtn;
            }
            finally { }
        }

        [XmlRpcMethod("GetCurrentID")]        //CWJ
        public int GetCurrentID()
        {
            int rtn = -1;
            try
            {
                rtn = (int)Invoke("GetCurrentID");
                return rtn;

            }
            finally { }
        }

        [XmlRpcMethod("GetCurrentSeq")]        //CWJ
        public int GetCurrentSeq()
        {
            int rtn = -1;
            try
            {
                rtn = (int)Invoke("GetCurrentSeq");
                return rtn;

            }
            finally { }
        }

        [XmlRpcMethod("GetCurrentSec")]        //CWJ
        public int GetCurrentSec()
        {
            int rtn = -1;
            try
            {
                rtn = (int)Invoke("GetCurrentSec");
                return rtn;

            }
            finally { }
        }

        [XmlRpcMethod("InitScanVialBarcode")]        //CWJ
        public int InitScanVialBarcode()
        {
            int rtn = -1;
            try
            {
                rtn = (int)Invoke("InitScanVialBarcode");
                return rtn;
                
            }
            finally { }
        }

        [XmlRpcMethod("ScanVialBarcodeAt")]        //CWJ
        public int ScanVialBarcodeAt(int Quadrant, int Vial)
        {
            int rtn = -1;
            try
            {
                rtn = (int)Invoke("ScanVialBarcodeAt", new object[2]{Quadrant, Vial});
                return rtn;

            }
            finally { }
        }

        [XmlRpcMethod("GetVialBarcodeAt")]        //CWJ
        public string GetVialBarcodeAt(int Quadrant, int Vial)
        {
            string rtn = "Error";
            try
            {
                rtn = (string)Invoke("GetVialBarcodeAt", new object[2]{Quadrant,Vial});
                return rtn;

            }
            finally { }
        }

        /// <summary>
		/// Pause Command
		/// </summary>
		/// <returns></returns>
		[XmlRpcMethod("getPauseCommandCaption")]
		public string getPauseCommandCaption()
		{
			string pauseCommandCaption="";
			try
			{
				pauseCommandCaption = (string) Invoke("getPauseCommandCaption");
				return pauseCommandCaption;
			}
			finally { }
		}

        /// <summary>
        /// Instruct the instrument to assume the user has refilled the hydraulic fluid container.
        /// </summary>
        /// <returns></returns>
        [XmlRpcMethod("setHydroFluidFull")]
        public bool SetHydroFluidFull()
        {
            try
            {
                return (bool)Invoke("SetHydroFluidFull");
            }
            finally
            {
            }
        }

        /// <summary>
        /// Get the time span of individual protocol end times for protocols in the batch.
        /// </summary>
        /// <returns>End time span in seconds.</returns>
        [XmlRpcMethod("getEndTimeSpan")]
        public int GetEndTimeSpan()
        {
            try
            {
                return (int)Invoke("GetEndTimeSpan");
            }
            finally
            {
            }
        }

        /// <summary>
        /// Get the end time span threshold beyond which the user should be warned that the
        /// end times for the batch vary by more than the threshold.
        /// </summary>
        /// <returns>End time span threshold in seconds.</returns>        
        [XmlRpcMethod("getEndTimeSpanThreshold")]
        public int GetEndTimeSpanThreshold()
        {
            try
            {
                return (int)Invoke("GetEndTimeSpanThreshold");
            }
            finally
            {
            }
        }

		#endregion Instrument

		#endregion IInstrumentControl Members

        #region Helper methods

        private string FormatSamplesInfo(XmlRpcSample[] samples)
        {
            StringBuilder message = new StringBuilder(null);
            int sampleCount = samples.GetLength(0);
            for (int i = 0; i < sampleCount; ++i)
            {
                message.AppendFormat("Sample {0}: Protocol ID:{1}, Volume:{2:F3}[uL]",
                    samples[i].ID, samples[i].protocolID, samples[i].volume_uL);
                if (sampleCount > 1 && i < sampleCount-1)
                {
                    message.Append(", ");
                }
            }
            return message.ToString();
        }

        #endregion Helper methods
	}
}
