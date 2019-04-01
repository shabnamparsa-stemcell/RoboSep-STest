//----------------------------------------------------------------------------
// SeparatorImpl
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
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Serialization.Formatters;
using System.IO;
using System.Threading;
using System.Diagnostics;

using CookComputing.XmlRpc;

using Invetech.ApplicationLog;

using Tesla.InstrumentControl;
using Tesla.Common.Separator;
using Tesla.Common.ResourceManagement;
using CWJ_Console;
using GUI_Console;


namespace Tesla.Separator
{
	/// <summary>
	/// Implementation for the "Separator" service; entry point for the Separator layer.
	/// </summary>
	/// <remarks>
	/// Singleton.
	/// </remarks>
	public class SeparatorImpl : ApplicationContext
	{				
		private static SeparatorImpl	theSeparatorImpl;	// Singleton instance variable

        private static IInstrumentControlEvents theInstrumentControlEventSink;

		private InstrumentControlProxy	myInstrument;
        private QuadrantManager			myQuadrantManager;

		public static SeparatorImpl GetInstance()
		{
			if (theSeparatorImpl == null)
			{
				try
				{
					theSeparatorImpl = new SeparatorImpl();
				}
				catch
				{
					theSeparatorImpl = null;
				}
			}
			return theSeparatorImpl;
		}

		private SeparatorImpl()
		{
			try
			{
                // -------------------
                // Set the fully-qualified machine name for the server (optionally supplied in 
                // the application configuration file).
                NameValueCollection nvc = (NameValueCollection)	
                    ConfigurationSettings.GetConfig("Separator/InstrumentControlConnection");
                string serverAddress = string.Empty;        
                try
                {
                    serverAddress = nvc.Get("ServerAddress");
                }
                finally
                {
                    // Ignore any exceptions -- if we can't read the IP address configuration 
                    // setting then we act as if it were not specified.
                }

                RemotingInfo.ConfigureInstance(serverAddress);

                string configuredAddress = (serverAddress == null || serverAddress == string.Empty) ? 
                    "<Not specified>" : serverAddress;
                LogFile.AddMessage(TraceLevel.Verbose, "Configured address = " + configuredAddress);

				// -------------------
				// Register an event handler for IInstrumentControlEvents				
				RemotingConfiguration.Configure("Separator.exe.config"); 

				// -------------------
				// Setup TCP Server channel
				BinaryClientFormatterSinkProvider tcpClientSinkProvider = new BinaryClientFormatterSinkProvider();
				
				BinaryServerFormatterSinkProvider tcpServerSinkProvider = new BinaryServerFormatterSinkProvider();
				tcpServerSinkProvider.TypeFilterLevel = TypeFilterLevel.Full;				

				ListDictionary tcpChannelProperties = new ListDictionary();
				tcpChannelProperties.Add("name", "SeparatorAPI");
				tcpChannelProperties.Add("port", 3149);

				TcpChannel tcpChannel = new TcpChannel(tcpChannelProperties, tcpClientSinkProvider, tcpServerSinkProvider);

				ChannelServices.RegisterChannel(tcpChannel);

				RemotingConfiguration.RegisterWellKnownServiceType(
					typeof(InstrumentControlEventSink),
					RemotingInfo.GetInstance().IcsEventSinkURI,
					WellKnownObjectMode.Singleton);

				// -------------------
                // Create the InstrumentControlEventSink
                try
                {

                    theInstrumentControlEventSink = 
                        (IInstrumentControlEvents)Activator.GetObject(
                        typeof(InstrumentControlEventSink),
                        RemotingInfo.GetInstance().IcsEventSinkURL);
					theInstrumentControlEventSink.ping();    // Call a method to trigger the server

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
			catch (Exception ex)
			{
				myInstrument = null;

				Console.WriteLine(ex.GetType());
				Console.WriteLine(ex.Message + "\n" + ex.InnerException);				
				Console.WriteLine(ex.StackTrace);		
		
				throw new ApplicationException();
			}
		}

		public void RegisterLayerServices()
		{			
            // Register Separator API service
			RemotingConfiguration.RegisterWellKnownServiceType(
				typeof(Separator),
				"Separator",
				WellKnownObjectMode.Singleton);			
		}

		public void ShutdownInstrument()
		{
			if (myInstrument != null)
			{
				myInstrument.Halt();
				myInstrument = null;				
			}
		}
		
		public void Initialise()
		{
			// Create a proxy to IInstrumentControl
			myInstrument = InstrumentControlProxy.GetInstance();

			// Reference the Quadrant Manager
			myQuadrantManager = QuadrantManager.GetInstance();
		}

		public void Connect(bool reqSubscribe)
		{
			if(reqSubscribe)
				myInstrument.Subscribe();
			myQuadrantManager.Connect();
		}

		public void Disconnect(bool isExit)
		{
			if(isExit)
			{
				this.ExitThread();
			}
			else
			{
				myInstrument.Unsubscribe();
			}
		}		

        public IInstrumentControlEvents InstrumentControlEventSink
        {
            get
            {
                return theInstrumentControlEventSink as IInstrumentControlEvents;
            }
        }

		/// <summary>
		/// Entry point for the Separator application.
		/// </summary>
        [STAThread]
		static void Main(string[] args)
		{			
			// Define top-level exception handlers
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
			Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);

			SeparatorImpl separatorImpl = null;
            SeparatorResourceManager SepResMgr = null;  
			try
			{
				// Initialise the application log file
				LogFile.GetInstance().Initialise(GetLogsPath(), "robosep_ui.log");

				// Create the Separator layer
				separatorImpl = SeparatorImpl.GetInstance();
				if (separatorImpl != null)
				{
					// Register the Separator service
					separatorImpl.RegisterLayerServices();

                    SepResMgr = SeparatorResourceManager.GetInstance();

					// Start the UI layer (Operator Console is the local instrument console)
					Thread.CurrentThread.Name = "OperatorConsole";
					//FrmOperatorConsole aOperatorConsole = new FrmOperatorConsole();
					//Application.Run(aOperatorConsole);

                    //FrmCWJMain theCWJMainCon = new FrmCWJMain();
                    //Application.Run(theCWJMainCon);
                    Application.Run(RoboSep_UserConsole.getInstance());
				}
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);                    
			}
			finally
			{
			
                // Shutdown the Separator layer
				if (separatorImpl != null)
				{
					separatorImpl.ShutdownInstrument();
				}

				// Close the application log file
				LogFile.GetInstance().Dispose();
			}
		}

		private static string GetLogsPath()
		{			
			// Check the "logs" sub-directory exists, otherwise create it
			string startupPath = Application.StartupPath;
			int lastBackslashIndex = startupPath.LastIndexOf('\\');  // must escape the '\'
			startupPath = startupPath.Remove(lastBackslashIndex, startupPath.Length - lastBackslashIndex);
			string logsPath = startupPath + @"\logs";
			if ( ! Directory.Exists(logsPath))
			{
				// We're not running from the application installation directory,
				// (probably because we're running from development directories)
				// so create a "logs" directory.
				Directory.CreateDirectory(logsPath);
			}
			return logsPath;
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
		{
			Exception ex = (Exception) args.ExceptionObject;			
			LogFile.LogException(TraceLevel.Error, ex);
		}

		private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			LogFile.LogException(TraceLevel.Error, e.Exception);
		}
	}
}
