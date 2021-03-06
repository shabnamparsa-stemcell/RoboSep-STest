//----------------------------------------------------------------------------
// InstrumentControlProxy
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
// Notes:
//	 03/24/05 - added parkarm() and islidclosed() - bdr
// 	 04/11/05 - added parkpump to deenergize pump - bdr
//   03/23/06 - uniform multi sample - RL
//
//----------------------------------------------------------------------------

#define bdr31  // park arm pump
#define bdr32  // lid closed
#define bdr33  // sample tube id


using System;
using System.Diagnostics;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.Threading;
using System.Net;
using System.Globalization;

using Invetech.ApplicationLog;

using Tesla.Common;
using Tesla.Common.Protocol;
using Tesla.Common.Separator;
using Tesla.Common.ResourceManagement;

namespace Tesla.InstrumentControl
{
	/// <summary>
	/// Access to the InstrumentControl layer.
	/// </summary>
	/// <remarks>
	/// Singleton.
	/// Wraps the XmlRpc proxy so we can present non-XML-RPC types to callers.  We 
	/// expose only those members of IInstrumentControl that are useful to clients.
	/// </remarks>
	public class InstrumentControlProxy
	{
		// Singleton instance variable
		private static InstrumentControlProxy   theInstrumentControlProxy;

		// Reference to the proxy representing the Instrument Control API
		private InstrumentControlXmlRpcProxy	myXmlRpcProxy;

		#region Construction/destruction

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static InstrumentControlProxy GetInstance()
		{
			if (theInstrumentControlProxy == null)
			{				
				theInstrumentControlProxy = new InstrumentControlProxy();						
			}
			return theInstrumentControlProxy;
		}

		private InstrumentControlProxy()
		{
			// Create the objects that represent the control/event connections...

			// Make initial attempt to connect to the Instrument Control server.
			// Get the number of times to re-try if the connection attempt fails.
			NameValueCollection nvc = (NameValueCollection)	
				ConfigurationSettings.GetConfig("Separator/InstrumentControlConnection");
			int retryCount, retryWait;				            
			try
			{
				retryCount = int.Parse(nvc.Get("RetryCount"));
				retryWait  = int.Parse(nvc.Get("RetryWait_ms"));
			}
			catch
			{
				// Set default values in case the parse fails
				retryCount = 3;	
				retryWait  = 2000;	// milliseconds
			}

			do
			{
				try
				{
					myXmlRpcProxy = new InstrumentControlXmlRpcProxy(
						RemotingInfo.GetInstance().IcsServerURL);
					retryCount = 0;	// Clear the retry count as we're now connected.
				}
				catch (Exception ex)
				{
					// Connection attempt failed.  Re-try if the number of attempts so far is
					// within the retry count specified in the application configuration file,
					// otherwise propagate an exception to tell the caller we failed to connect.
					myXmlRpcProxy = null;
					LogFile.AddMessage(TraceLevel.Warning, "retryCount = " + retryCount + " " + ex.Message);
					if (--retryCount <= 0)
					{						
						throw new ApplicationException(
							SeparatorResourceManager.GetSeparatorString(
							StringId.ExFailedToConnectToInstrumentControl));
					}
					// Pause before retrying connection attempt
					Thread.Sleep(retryWait);
				}
			} while (retryCount > 0);
		}

		#endregion Construction/destruction

		#region Server connection

		/// <summary>
		/// Make the connection to the Instrument Control XML-RPC server.
		/// </summary>
		public void Subscribe()
		{
			try
			{
				if (myXmlRpcProxy != null)
				{
					// Register for Instrument Control Events                
					myXmlRpcProxy.Subscribe(RemotingInfo.GetInstance().IcsEventSinkURL);
				}
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);				
				//DisableConnection();
				throw new ApplicationException(
					SeparatorResourceManager.GetSeparatorString(
					StringId.ExFailedToSubscribeToInstrumentControl), ex);
			}
		}

		/// <summary>
		/// </summary>
		public void Unsubscribe()
		{
			try
			{
				if (myXmlRpcProxy != null && IsConnectionEnabled)
				{
					// Disconnect from the XML-RPC server
					myXmlRpcProxy.Unsubscribe(RemotingInfo.GetInstance().IcsEventSinkURL);
				}
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);				
				//DisableConnection();
				throw new ApplicationException(
					"Unsubscribe exception", ex);
			}
		}

		private bool IsConnectionEnabled
		{
			get
			{
				return myXmlRpcProxy != null;
			}
		}

		private void DisableConnection()
		{
			// Disable the forward channel
			myXmlRpcProxy = null;

			// Disable the back channel
			// NOTE: this should not be necessary in normal operation, but could consider
			// a future change to indicate the connection to the instrument has been lost! 
			// For example, throw an application exception/fire an event so the UI can 
			// indicate that server connectivity has been lost.
		}

		#endregion Server connection

		#region ErrorLogging

		/// <summary>
		/// 
		/// </summary>
		public event ReportErrorDelegate ReportError;

		private void LogException(Exception ex, string caller)
		{
			// Start with a generic error message
			string message = ex.Message;

			// If it is a known exception type, extract some more detailed information.
			CookComputing.XmlRpc.XmlRpcFaultException faultEx = ex as CookComputing.XmlRpc.XmlRpcFaultException;
			if (faultEx != null)
			{
				message += " FaultCode: " +	faultEx.FaultCode +
					", FaultString: " + faultEx.FaultString;
			}
			
			// Add more exception types if necessary
			string logMessage = SeparatorResourceManager.GetSeparatorString(
				StringId.ExInstrumentCommunicationsFailure) + message + " *** "+caller;
			LogFile.AddMessage(TraceLevel.Warning, logMessage);
			Debug.WriteLine(logMessage);

			// Report the error to the GUI so that it can take appropriate action
			ReportError(2, "TEC1300", new string[] {message});

		}

		#endregion ErrorLogging

		#region Instrument Control XML-RPC Server

		/// <summary>
		/// 
		/// </summary>
		public void Halt()
		{
			bool haltResult = false, unsubscribeResult = false;
			try
			{
				if (IsConnectionEnabled)
				{
					// Tell the XML-RPC server to shut down its "RoboSep" command handling
					haltResult = myXmlRpcProxy.Halt();

					// Disconnect from the XML-RPC server
					unsubscribeResult = myXmlRpcProxy.Unsubscribe(
						RemotingInfo.GetInstance().IcsEventSinkURL);					
				}
			}
			catch (WebException wex)
			{
				if (wex.Status != WebExceptionStatus.ConnectionClosed)
				{
					// Log the details of the exception
					LogException(wex,"Halt1");
				}
			}
			catch (Exception ex)
			{
				// Log the details of the exception
				LogException(ex,"Halt2");
			}
			finally
			{
				LogFile.AddMessage(TraceLevel.Verbose, 
					"Instrument Control API Halt() returned " + 
					haltResult.ToString());

				LogFile.AddMessage(TraceLevel.Verbose, 
					"Instrument Control API Unsubscribe() returned " + 
					unsubscribeResult.ToString());

				DisableConnection();
			}
		}

		/// <summary>
		/// Retrieve Instrument Control server information including version number.
		/// </summary>
		/// <param name="gatewayURL"></param>
		/// <param name="gatewayInterfaceVersion"></param>
		/// <param name="serverUptime_seconds"></param>
		/// <param name="instrumentControlVersion"></param>
		/// <param name="instrumentSerialNumber"></param>
		/// <param name="serviceConnection"></param>
		public void GetServerInfo(out string gatewayURL, out string gatewayInterfaceVersion,
			out string serverUptime_seconds, out string instrumentControlVersion,
			out string instrumentSerialNumber, out string serviceConnection)
		{
			gatewayURL = gatewayInterfaceVersion = serverUptime_seconds = string.Empty;
			instrumentControlVersion = instrumentSerialNumber = serviceConnection = string.Empty;
			try
			{
				if (IsConnectionEnabled)
				{
					string[] serverInfo = myXmlRpcProxy.GetServerInfo();
					if (serverInfo != null && serverInfo.GetLength(0) == 5)
					{
						gatewayURL					= serverInfo[0];
						gatewayInterfaceVersion		= serverInfo[1];
						serverUptime_seconds		= serverInfo[2];
						instrumentControlVersion	= serverInfo[3];
						instrumentSerialNumber		= serverInfo[4];
					}
					serviceConnection = RemotingInfo.GetInstance().HostDomainName;
					// If the service connection indicates the loopback address or local
					// machine, clear the service connection information (this will trigger
					// the UI to display a localised prompt for the user to connect a 
					// cable and re-start.
					if (serviceConnection == RemotingInfo.GetInstance().LocalMachineName ||
						serviceConnection == RemotingInfo.GetInstance().LoopbackAddress)
					{
						serviceConnection = string.Empty;                        
					}
				}
			}
			catch (Exception ex)
			{
				// Log the details of the exception
				LogException(ex,"GetServerInfo");
			}
		}

		#endregion Instrument Control XML-RPC Server

		#region Run

		/// <summary>
		/// 
		/// </summary>
		/// <param name="selectedProtocols"></param>
		/// <param name="selectedProtocolIDs"></param>
		/// <param name="sampleIDs"></param>
		/// <param name="sampleVolumes"></param>
		/// <returns></returns>
		public TimeSpan ScheduleRun(IProtocol[] selectedProtocols, int[] selectedProtocolIDs,
			int[] sampleIDs, double[] sampleVolumes)
		{
            for (int numIDs = 0; numIDs<sampleIDs.Length; numIDs++)
            {
                string ReportIDs = string.Format("**************** Sample IDs : {0} ****************", sampleIDs[numIDs]);
                System.Diagnostics.Debug.WriteLine(ReportIDs);
            }

			TimeSpan estimatedTimeToCompletion = TimeSpan.Zero;
			try
			{
				if (IsConnectionEnabled)
				{
					// Prepare samples info
					XmlRpcSample[] samples = PrepareSampleInformation(selectedProtocols,
						selectedProtocolIDs, sampleIDs, sampleVolumes, null);						

					// Schedule the run/calculate updated ETC
					LogFile.AddMessage(TraceLevel.Verbose, "Requesting ETC update (ScheduleRun)");
					DateTime estimatedTimeOfCompletion = myXmlRpcProxy.ScheduleRun(samples);

					// Log the ETC
					DateTimeFormatInfo currentDateTimeFormatInfo = DateTimeFormatInfo.CurrentInfo;
					LogFile.AddMessage(TraceLevel.Verbose,
						string.Format("ETC update: {0}", 
						estimatedTimeOfCompletion.ToString(currentDateTimeFormatInfo.SortableDateTimePattern)));

					// Calculate and report the time remaining (time to completion)
					CalculateEstimatedTimeToCompletion(estimatedTimeOfCompletion,
						ref estimatedTimeToCompletion);
				}
			}

			catch (Exception ex)
			{
				// Log the details of the exception
				LogException(ex,"ScheduleRun");
			}
			return estimatedTimeToCompletion;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="batchRunUserId"></param>
		/// <param name="selectedProtocols"></param>
		/// <param name="selectedProtocolIDs"></param>
		/// <param name="sampleIDs"></param>
		/// <param name="sampleVolumes"></param>
		/// <param name="protocolReagentLotIds"></param>
		/// <param name="isSharing"></param>
		/// <param name="sharedSectorsTranslation"></param>
		/// <param name="sharedSectorsProtocolIndex"></param>
		/// <returns></returns>
		public TimeSpan StartRun(string batchRunUserId, IProtocol[] selectedProtocols, 
			int[] selectedProtocolIDs, int[] sampleIDs, double[] sampleVolumes,
			ReagentLotIdentifiers[] protocolReagentLotIds,
			bool isSharing, int[] sharedSectorsTranslation, int[]sharedSectorsProtocolIndex)
		{
            for (int numIDs = 0; numIDs < sampleIDs.Length; numIDs++)
            {
                string ReportIDs = string.Format("**************** Sample IDs          : {0}", sampleIDs[numIDs]);
                System.Diagnostics.Debug.WriteLine(ReportIDs);
            }

			TimeSpan estimatedTimeToCompletion = TimeSpan.Zero;
			try
			{
				if (IsConnectionEnabled)
				{	
					// Prepare samples info	
					XmlRpcSample[] samples = PrepareSampleInformation(selectedProtocols,
						selectedProtocolIDs, sampleIDs, sampleVolumes, protocolReagentLotIds);

					// Start the run/calculate updated ETC
					LogFile.AddMessage(TraceLevel.Verbose, "Requesting ETC update (StartRun)");                     
					DateTime estimatedTimeOfCompletion = myXmlRpcProxy.StartRun(samples, batchRunUserId,
						isSharing,sharedSectorsTranslation,sharedSectorsProtocolIndex);

					// Log the ETC
					DateTimeFormatInfo currentDateTimeFormatInfo = DateTimeFormatInfo.CurrentInfo;
					LogFile.AddMessage(TraceLevel.Verbose,
						string.Format("ETC update: {0}", 
						estimatedTimeOfCompletion.ToString(currentDateTimeFormatInfo.SortableDateTimePattern)));

					// Calculate and report the time remaining (time to completion)
					CalculateEstimatedTimeToCompletion(estimatedTimeOfCompletion,
						ref estimatedTimeToCompletion);
				}
			}
			catch (Exception ex)
			{
				// Log the details of the exception
				LogException(ex,"StartRun");
			}
			return estimatedTimeToCompletion;
		}

		private XmlRpcSample[] PrepareSampleInformation(IProtocol[] selectedProtocols, 
			int[] selectedProtocolIDs, int[] sampleIDs, double[] sampleVolumes,
			ReagentLotIdentifiers[] protocolReagentLotIds)
		{
			int samplesCount = selectedProtocols.GetLength(0);
			XmlRpcSample[] samples = new XmlRpcSample[samplesCount];
			for (int i = 0; i < samplesCount; ++i)
			{
				samples[i] = new XmlRpcSample();
				samples[i].ID				= sampleIDs[i];
				samples[i].label			= string.Empty;	// NOTE: the user may supply these in a future version.
				samples[i].protocolID		= selectedProtocolIDs[i];
				samples[i].volume_uL		= sampleVolumes[i];

				// To specify 'initialQuadrant', we need to convert quadrant id range from 
				// [0..3] to [1..4] (hence the "+ 1").
				samples[i].initialQuadrant	= (int)selectedProtocols[i].InitialQuadrant + 1;	
			
				// add reagent lot ID information, if supplied
				if (protocolReagentLotIds == null)
				{
					samples[i].cocktailLabel = string.Empty;
					samples[i].particleLabel = string.Empty;
					samples[i].antibodyLabel = string.Empty;
					samples[i].lysisLabel	 = string.Empty;
					samples[i].sample1Label  = string.Empty; // bdr
					samples[i].sample2Label  = string.Empty; // bdr
				}
				else
				{
					try
					{
						samples[i].cocktailLabel = protocolReagentLotIds[i].myCocktailLabel;
						samples[i].particleLabel = protocolReagentLotIds[i].myParticleLabel;
						samples[i].antibodyLabel = protocolReagentLotIds[i].myAntibodyLabel;
						samples[i].lysisLabel	 = protocolReagentLotIds[i].myLysisLabel;
						
						string IDs="";
						for(int j=0;j<protocolReagentLotIds[i].mySampleLabels.Length;j++)
						{
							if (protocolReagentLotIds[i].mySampleLabels[j]!=null && 
								protocolReagentLotIds[i].mySampleLabels[j]!="")
							{
								if (IDs!="")
									IDs+=", ";
								IDs+=protocolReagentLotIds[i].mySampleLabels[j];
							}
						}
						samples[i].sample1Label  = IDs; // bdr
						//samples[i].sample1Label  = protocolReagentLotIds[i].mySample1Label; // bdr
						samples[i].sample2Label  = ""; //protocolReagentLotIds[i].mySample2Label; // bdr
						string dbg = string.Format("proxy sample1Label ID = {0}", samples[i].sample1Label); 
					}											   
					catch
					{
					}
				}
			}	
			return samples;
		}

		private void CalculateEstimatedTimeToCompletion(DateTime estimatedTimeOfCompletion,
			ref TimeSpan estimatedTimeToCompletion)
		{	
			DateTime currentTime = DateTime.Now;
			estimatedTimeToCompletion = TimeSpan.Zero;
			// Check the Estimated Time of Completion is in the future 
			if (DateTime.Compare(estimatedTimeOfCompletion, currentTime) > 0)
			{                
				// Round up the estimated time of completion to the next minute (because we're
				// currently showing HH:mm format -- no seconds -- round up so we show
				// at least one minute for schedules < 60 seconds).
				// If the protocol actually finishes soon after the 1 minute time is shown, no-one
				// will worry too much (in contrast, showing zero time remaining while the protocol
				// is still running is not as comforting).				
				estimatedTimeToCompletion = estimatedTimeOfCompletion.Subtract(currentTime);
				if (estimatedTimeToCompletion.TotalSeconds < 60.0d)
				{
					estimatedTimeToCompletion = estimatedTimeToCompletion.Add(
						TimeSpan.FromSeconds(59.0));
				}
#if (DEBUG)
                LogFile.AddMessage(TraceLevel.Verbose, "Calculated ETC = " + estimatedTimeToCompletion.ToString());
#endif
			}
		}


#if bdr31
		/// <summary>
		/// 
		/// </summary>
		public void ParkArm()  // bdr
		{
			try
			{
				if (IsConnectionEnabled)
				{
					System.Diagnostics.Debug.WriteLine("QMgr  - ICPxy.parkArm"); // bdr
					myXmlRpcProxy.ParkArm();
				}
			}
			catch (Exception ex)
			{
				// Log the details of the exception
				LogException(ex,"ParkArm");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void ParkPump()  // bdr
		{
			try
			{
				if (IsConnectionEnabled)
				{
					System.Diagnostics.Debug.WriteLine("QMgr  - ICPxy.parkPump"); // bdr
					myXmlRpcProxy.ParkPump();
				}
			}
			catch (Exception ex)
			{
				// Log the details of the exception
				LogException(ex,"ParkPump");
			}
		}
#endif
        
		/// <summary>
		/// 
		/// </summary>
		public void HaltRun()
		{
			try
			{
				if (IsConnectionEnabled)
				{
					myXmlRpcProxy.HaltRun();
				}
			}
			catch (Exception ex)
			{
				// Log the details of the exception
				LogException(ex,"HaltRun");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void AbortRun()
		{
			try
			{
				if (IsConnectionEnabled)
				{
					myXmlRpcProxy.AbortRun();
				}
			}
			catch (Exception ex)
			{
				// Log the details of the exception
				LogException(ex,"AbortRun");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void PauseRun()
		{
			try
			{
				if (IsConnectionEnabled)
				{
					myXmlRpcProxy.PauseRun();
				}
			}
			catch (Exception ex)
			{
				// Log the details of the exception
				LogException(ex,"PauseRun");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void ResumeRun()
		{
			try
			{
				if (IsConnectionEnabled)
				{
					myXmlRpcProxy.ResumeRun();
				}
			}
			catch (Exception ex)
			{
				// Log the details of the exception
				//LogException(ex,"ResumeRun");
				LogFile.AddMessage(TraceLevel.Info,"ResumeRun timeout. Can be ignore");
			}
		}

		#endregion Run

		#region Protocol

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public XmlRpcProtocol[] GetProtocols()
		{
			XmlRpcProtocol[] protocols = null;
			try
			{
				if (IsConnectionEnabled)
				{
					protocols = myXmlRpcProxy.GetProtocols();
				}                
			}
			catch (Exception ex)
			{
				// Log the details of the exception
				LogException(ex,"GetProtocols");
			}
			return protocols;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		/*CR*/	public string[][] GetCustomNames()
				{
					string[][] customNames = null;
					try
					{
						if (IsConnectionEnabled)
						{
							customNames = myXmlRpcProxy.GetCustomNames();
						}                
					}
					catch (Exception ex)
					{
						// Log the details of the exception
						LogException(ex,"GetCustomNames");
					}
					return customNames;
				}

		public bool[][] GetResultChecks()
		{
			bool[][] resultChecks = null;
			try
			{
				if (IsConnectionEnabled)
				{
					resultChecks = myXmlRpcProxy.GetResultChecks();
				}                
			}
			catch (Exception ex)
			{
				// Log the details of the exception
				LogException(ex,"GetResultChecks");
			}
			return resultChecks;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="protocolId"></param>
		/// <param name="sampleVolume_ul"></param>
		/// <param name="result"></param>
		public void CalculateConsumables(int protocolId, double sampleVolume_ul,
			out Hashtable[] result)
		{
			result = null;

			try
			{				
				if (IsConnectionEnabled)
				{
					// Get the consumables based on chosen protocol and sample volume (if any)					
					XmlRpcConsumables[] consumables = null;
					consumables = myXmlRpcProxy.CalculateConsumables(protocolId, sampleVolume_ul);

					// Allocate an array of hashtables with which to return the consumables
					int consumablesQuadrantCount = consumables.GetLength(0);
					result = new Hashtable[consumablesQuadrantCount];
					for (int i=0; i < consumablesQuadrantCount; ++i)
					{
						result[i] = new Hashtable();
					}

					// Assign consumable values on a per-quadrant basis.
					for (int relativeQuadrant = 0; relativeQuadrant < consumables.GetLength(0);
						++relativeQuadrant)
					{
						// Assign consumable values to the relevant quadrant locations.
						// Note this does include the SampleTube since in theory it may be used as a vessel
						// in protocols that use more than one quadrant.
						// NOTE: we use a key of "<relativeQuadrantId>.<quadrantLocation>" in the
						// returned hashtable so we can handle consumables requirements of
						// >= 1 quadrant's worth of resources.
						//
						// NOTE: we use the following convention to convert boolean 'required' flags to
						// equivalent fluid volumes for the cases that do not specify a volume:
						// -ve value:	this resource is not required and should not be loaded (e.g. no tube)
						// 0.0 value:	this resource is required (e.g. load tips box or load an empty tube)
						// +ve value:	this resource is required with the specified volume (e.g. load a tube with 1ml of relevant reagent)				
						if (consumables[relativeQuadrant].sampleVesselRequired)
						{
							// RL - uniform multi sample - 03/23/06
							if (consumables[relativeQuadrant].sampleVesselVolumeRequired && relativeQuadrant!=0)
							{
								result[relativeQuadrant].Add(
									RelativeQuadrantLocation.SampleTube,
									new FluidVolume(
									sampleVolume_ul,
									FluidVolumeUnit.MicroLitres));
							}
							else
							{
								result[relativeQuadrant].Add(
									RelativeQuadrantLocation.SampleTube,
									new FluidVolume(
									0.0d,
									FluidVolumeUnit.MicroLitres));
							}
						}

						if (consumables[relativeQuadrant].separationVesselRequired)
						{
							result[relativeQuadrant].Add(
								RelativeQuadrantLocation.SeparationTube, 
								new FluidVolume(
								0.0d,
								FluidVolumeUnit.MicroLitres));
						}

						if (consumables[relativeQuadrant].wasteVesselRequired)
						{
							result[relativeQuadrant].Add(
								RelativeQuadrantLocation.WasteTube, 
								new FluidVolume(
								0.0d,
								FluidVolumeUnit.MicroLitres));
						}

						if (consumables[relativeQuadrant].tipBoxRequired)
						{
							result[relativeQuadrant].Add(
								RelativeQuadrantLocation.TipsBox,	
								new FluidVolume(
								0.0d,
								FluidVolumeUnit.MicroLitres));
						}

						if (consumables[relativeQuadrant].particleVolume >= 0.0d)
						{
							result[relativeQuadrant].Add(
								RelativeQuadrantLocation.VialA, 
								new FluidVolume(
								consumables[relativeQuadrant].particleVolume,
								FluidVolumeUnit.MicroLitres));
						}

						if (consumables[relativeQuadrant].cocktailVolume >= 0.0d)
						{
							result[relativeQuadrant].Add(
								RelativeQuadrantLocation.VialB, 
								new FluidVolume(
								consumables[relativeQuadrant].cocktailVolume,
								FluidVolumeUnit.MicroLitres));
						}

						if (consumables[relativeQuadrant].antibodyVolume >= 0.0d)
						{
							result[relativeQuadrant].Add(
								RelativeQuadrantLocation.VialC, 
								new FluidVolume(
								consumables[relativeQuadrant].antibodyVolume,
								FluidVolumeUnit.MicroLitres));										
						}

						if (consumables[relativeQuadrant].lysisVolume >= 0.0d)
						{
							result[relativeQuadrant].Add(
								RelativeQuadrantLocation.LysisBufferTube,
								new FluidVolume(
								consumables[relativeQuadrant].lysisVolume,
								FluidVolumeUnit.MicroLitres));						
						}

						if (consumables[relativeQuadrant].bulkBufferVolume >= 0.0d)
						{
							result[relativeQuadrant].Add(
								RelativeQuadrantLocation.QuadrantBuffer,
								new FluidVolume(
								consumables[relativeQuadrant].bulkBufferVolume,
								FluidVolumeUnit.MicroLitres));
						}
					}
				}
			}
			catch (Exception ex)
			{
				// Log the details of the exception
				LogException(ex,"CalculateConsumables");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public void ReloadProtocols()
		{
			try
			{
				if (IsConnectionEnabled)
				{
					myXmlRpcProxy.ReloadProtocols();
				}   
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}


		#endregion Protocol

		#region Instrument

		/// <summary>
		/// 
		/// </summary>
		public string GetInstrumentState()
		{
			string state ="";
			try
			{
				state = myXmlRpcProxy.GetInstrumentState();
			}
			finally
			{
			}
			return state;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="bulkBufferDeadVolume"></param>
		public void GetContainerVolumes(out double bulkBufferDeadVolume)
		{
			bulkBufferDeadVolume = 0.0d;

			try
			{				
				if (IsConnectionEnabled)
				{
					XmlRpcContainer containers = myXmlRpcProxy.GetContainerVolumes();
					bulkBufferDeadVolume = containers.bulkBufferDeadVolume;
				}
			}
			catch (Exception ex)
			{
				// Log the details of the exception
				LogException(ex,"GetContainerVolumes");
			}
		}
        
#if bdr32
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool IsLidClosed()   // bdr
		{
			bool isLidClosed = false;
            
			try  
			{
				if (IsConnectionEnabled) 
				{
					isLidClosed = myXmlRpcProxy.IsLidClosed();
				}
			}
				// Log the details of the exception
			catch (Exception ex)  
			{
				LogException(ex,"IsLidClosed");
			}
			return isLidClosed;
		}
#endif
		
		//RL - pause command - 03/29/06
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool isPauseCommand()
		{
			bool isPauseCommand=false;
			try
			{
				isPauseCommand = myXmlRpcProxy.isPauseCommand();
			}
			catch (Exception ex)
			{
				LogException(ex,"isPauseCommand");
			}
			return isPauseCommand;
		}
        public bool GetIgnoreLidSensor()                               //CWJ
        {
            bool rtn = false;
            try
            {
                rtn = myXmlRpcProxy.GetIgnoreLidSensor();
            }
            catch (Exception ex)
            {
                LogException(ex, "GetIgnoreLidSensor");
            }
            return rtn;
        }
        public int SetIgnoreLidSensor(int sw)                               //CWJ
        {
            int rtn = -1;
            try
            {
                rtn = myXmlRpcProxy.SetIgnoreLidSensor(sw);
            }
            catch (Exception ex)
            {
                LogException(ex, "SetIgnoreLidSensor");
            }
            return rtn;
        }
        public int GetCurrentID()                               //CWJ
        {
            int rtn = -1;
            try
            {
                rtn = myXmlRpcProxy.GetCurrentID();
            }
            catch (Exception ex)
            {
                LogException(ex, "GetCurrentID");
            }
            return rtn;
        }
        public int GetCurrentSec()                               //CWJ
        {
            int rtn = -1;
            try
            {
                rtn = myXmlRpcProxy.GetCurrentSec();
            }
            catch (Exception ex)
            {
                LogException(ex, "GetCurrentSec");
            }
            return rtn;
        }
        public int GetCurrentSeq()                               //CWJ
        {
            int rtn = -1;
            try
            {
                rtn = myXmlRpcProxy.GetCurrentSeq();
            }
            catch (Exception ex)
            {
                LogException(ex, "GetCurrentSeq");
            }
            return rtn;
        }

        public int InitScanVialBarcode()                               //CWJ
        {
            int rtn = -1;
            try
            {
                rtn = myXmlRpcProxy.InitScanVialBarcode();
            }
            catch (Exception ex)
            {
                LogException(ex, "InitScanVialBarcode");
            }
            return rtn;
        }
        public int ScanVialBarcodeAt(int Quadrant, int Vial)        //CWJ
        {
            int rtn = -1;
            try
            {
                rtn = myXmlRpcProxy.ScanVialBarcodeAt(Quadrant, Vial);
            }
            catch (Exception ex)
            {
                LogException(ex, "ScanVialBarcodeAt");
            }
            return rtn;
        }
        public string GetVialBarcodeAt(int Quadrant, int Vial)        //CWJ
        {
            string rtn = "Error";
            try
            {
                rtn = myXmlRpcProxy.GetVialBarcodeAt(Quadrant, Vial);
            }
            catch (Exception ex)
            {
                LogException(ex, "GetVialBarcodeAt");
            }
            return rtn;
        }

        /// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public string getPauseCommandCaption() 
		{
			string pauseCommandCaption="";
			try
			{
				pauseCommandCaption = myXmlRpcProxy.getPauseCommandCaption();
			}
			catch (Exception ex)
			{
				LogException(ex,"getPauseCommandCaption");
			}
			return pauseCommandCaption;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool IsHydraulicFluidLow()
		{
			bool isHydraulicFluidLow = true;
			try
			{
				if (IsConnectionEnabled)
				{
					isHydraulicFluidLow = myXmlRpcProxy.IsHydroFluidLow();
				}
			}
			catch (Exception ex)
			{
				// Log the details of the exception
				LogException(ex,"IsHydraulicFluidLow");
			}
			return isHydraulicFluidLow;
		}

		/// <summary>
		/// 
		/// </summary>
		public void ConfirmHydraulicFluidRefilled()
		{
			bool isRefillConfirmed = false;
			try
			{
				if (IsConnectionEnabled)
				{
					isRefillConfirmed = myXmlRpcProxy.SetHydroFluidFull();                    
				}
			}
			catch (Exception ex)
			{
				// Log the details of the exception
				LogException(ex,"ConfirmHydraulicFluidRefilled");
			}
			finally
			{
				if ( ! isRefillConfirmed)
				{
					LogFile.AddMessage(TraceLevel.Verbose, 
						"Instrument Control API SetHydroFluidFull() returned " + 
						isRefillConfirmed.ToString());
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public void GetBatchEndTimeSpanAndWarningThreshold_minutes(out int endTimeSpan_minutes, 
			out int endTimeSpanWarningThreshold_minutes)            
		{
			endTimeSpan_minutes = 0;
			endTimeSpanWarningThreshold_minutes = 0;
			try
			{
				int endTimeSpan_s = myXmlRpcProxy.GetEndTimeSpan();
				int endTimeSpanWarningThreshold_s = myXmlRpcProxy.GetEndTimeSpanThreshold();
				TimeSpan endTimeSpan = TimeSpan.FromSeconds(endTimeSpan_s);
				TimeSpan endTimeSpanWarningThreshold = TimeSpan.FromSeconds(endTimeSpanWarningThreshold_s);
				endTimeSpan_minutes = (int)Math.Ceiling(endTimeSpan.TotalMinutes);
				endTimeSpanWarningThreshold_minutes = (int)Math.Ceiling(endTimeSpanWarningThreshold.TotalMinutes);
			}
			catch (Exception ex)
			{
				// Log the details of the exception
				LogException(ex,"GetBatchEndTimeSpanAndWarningThreshold_minutes");
			}
		}

		#endregion Instrument
	}
}
