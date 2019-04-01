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

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

using System.Text;

using CookComputing.XmlRpc;

using Invetech.ApplicationLog;

using Tesla.InstrumentControl;
using Tesla.Common.Separator;
using Tesla.Common.ResourceManagement;
using CWJ_Console;
using GUI_Console;
using Tesla.Common.ProtocolCommand;

namespace Tesla.Separator
{
    public class OrcaTCPServer
    {
        public delegate void OrcaTCPExceptionEvent(string msg);
        public delegate void OrcaTCPEvent(string msg);

        public event OrcaTCPExceptionEvent NotifyExceptionError = null;
        public event OrcaTCPEvent NotifyEvent = null;

        private object serverLock = new object();
        private bool showText = true;
        private int port;
        private Socket serverSocket;
        private bool OrcaStatus = true;
        private bool isOrcaTCPRunning = false;
        private static OrcaTCPServer theTCPSvr;	        

        private void OnOrcaTCPException(string msg)
        {
            if (NotifyExceptionError != null)
                NotifyExceptionError(msg);
        }

        private void OnOrcaTCPEvent(string msg)
        {
            if (NotifyEvent != null)
                NotifyEvent(msg);
        }

        private class ConnectionInfo
        {
            public Socket Socket;
            public byte[] Buffer;
        }

        private List<ConnectionInfo> connections =
                                new List<ConnectionInfo>();

        private void SetupServerSocket()
        {
            IPEndPoint myEndpoint = new IPEndPoint(
                IPAddress.Any, port);

            // Create the socket, bind it, and start listening
            serverSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Blocking = false; 

            serverSocket.Bind(myEndpoint);
            serverSocket.Listen((int)SocketOptionName.MaxConnections);
        }

        private void CloseConnection(ConnectionInfo ci)
        {
            ci.Socket.Close();
            lock (connections) connections.Remove(ci);
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            ConnectionInfo connection = (ConnectionInfo)result.AsyncState;
            Console.WriteLine(">>>> OrcaTCPSvr ReceiveCallback\n");           
            try
            {
                int bytesRead = connection.Socket.EndReceive(result);
                if (0 != bytesRead)
                {
                    lock (serverLock)
                    {
                        if (showText)
                        {
                            string text = Encoding.UTF8.GetString(connection.Buffer, 0, bytesRead);
                            Console.Write(text);
                        }
                    }
                    lock (connections)
                    {
                        foreach (ConnectionInfo conn in connections)
                        {
                            if (connection != conn)
                            {
                                conn.Socket.Send(connection.Buffer, bytesRead,
                                    SocketFlags.None);
                            }
                        }
                    }
                    connection.Socket.BeginReceive(connection.Buffer, 0,
                        connection.Buffer.Length, SocketFlags.None,
                        new AsyncCallback(ReceiveCallback), connection);
                }
                else CloseConnection(connection);
            }
            catch (SocketException)
            {
                CloseConnection(connection);
                OnOrcaTCPException(">> SockerException Error\n");
            }
            catch (Exception)
            {
                CloseConnection(connection);
                OnOrcaTCPException(">> Exception Error\n");
            }
        }

        private void AcceptCallback(IAsyncResult result)
        {
            Console.WriteLine(">>>> OrcaTCPSvr Accept!\n");
            ConnectionInfo connection = new ConnectionInfo();

            try
            {
                // Finish Accept
                Socket s = (Socket)result.AsyncState;
                connection.Socket = s.EndAccept(result);
                connection.Socket.Blocking = false;
                connection.Buffer = new byte[255];
                lock (connections) connections.Add(connection);

                Console.WriteLine(">>>> OrcaTCPSvr New connection from " + s);

                // Start Receive
                
                connection.Socket.BeginReceive(connection.Buffer, 0,
                    connection.Buffer.Length, SocketFlags.None,
                    new AsyncCallback(ReceiveCallback), connection);
                
                // Start new Accept
                serverSocket.BeginAccept(new AsyncCallback(AcceptCallback),
                    result.AsyncState);

                BroadcastOrcaStatus(OrcaStatus);
            }
            catch (SocketException exc)
            {
                CloseConnection(connection);
                Console.WriteLine(">>>> OrcaTCPSvr Socket exception: " + exc.SocketErrorCode);
                OnOrcaTCPException(">>>> OrcaTCPSvr Socket exception: " + exc.SocketErrorCode);
            }
            catch (Exception exc)
            {
                CloseConnection(connection);
                Console.WriteLine(">>>> OrcaTCPSvr Exception: " + exc);
                OnOrcaTCPException(">>>> OrcaTCPSvr Exception: " + exc);
            }
        }

        public static OrcaTCPServer GetInstance()
        {
            if (theTCPSvr == null)
            {
                try
                {
                    theTCPSvr = new OrcaTCPServer();
                }
                catch
                {
                    theTCPSvr = null;
                }
            }
            return theTCPSvr;
        }

        public void Start()
        {
            if (isOrcaTCPRunning)
                                return;

            Console.Write(">>>>Starting Orca TCP server...\n ");
            try
            {
                SetupServerSocket();
                for (int i = 0; i < 10; i++)
                    serverSocket.BeginAccept(
                        new AsyncCallback(AcceptCallback), serverSocket);
            }
            catch (Exception e)
            {
                Console.WriteLine(">>>>Orca Fail.\n");
                Console.WriteLine(e);
                OnOrcaTCPException(">>>>Orca Fail.\n");
            }
            isOrcaTCPRunning = true;            
            Console.WriteLine(">>>> OrcaTCPSvr Listening.\n");
            OnOrcaTCPEvent(">> Orca TCP is Listening...\n");
        }

        public void Stop()
        {
            if (!isOrcaTCPRunning)
                return;

            Console.Write(">>>>Orca TCP Shutting down server... \n");
            lock (connections)
            {
                for (int i = connections.Count - 1; i >= 0; i--)
                {
                    CloseConnection(connections[i]);
                }
            }
            isOrcaTCPRunning = false;
            Console.WriteLine(">>>>Orca TCP Bye.\n");
            Thread.Sleep(500);
            OnOrcaTCPEvent(">> Orca TCP Stopped\n");
        }

        public void BroadcastOrcaStatus(bool status)
        {
            string CMD = null;

            OrcaStatus = status;

            switch (status)
            {
                case false:
                    CMD = "RC false";
                    break;
                case true:
                    CMD = "RC true";
                    break;
            }

            foreach (ConnectionInfo conn in connections)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(CMD + "\n");
                conn.Socket.Send(bytes, bytes.Length, SocketFlags.None);
            }
        }

        public int TCPPort
        {
            set
            {
                port = value;
            }
        }
    }

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
        private const string ROBOSEP_LOG_FILENAME = "_robosep_ui.log";
        private static string WorkingLofFileName = "";

		private InstrumentControlProxy	myInstrument;
        private QuadrantManager			myQuadrantManager;
        private OrcaTCPServer           myTCPSvr;
        private int                     myTCP_Port = 5899;

        private static bool isLogFileClose = false;

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
                NameValueCollection nvc = (NameValueCollection)System.Configuration.ConfigurationManager.GetSection("Separator/InstrumentControlConnection");

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
				RemotingConfiguration.Configure("Separator.exe.config", false); 

				// -------------------
				// Setup TCP Server channel
				BinaryClientFormatterSinkProvider tcpClientSinkProvider = new BinaryClientFormatterSinkProvider();
				
				BinaryServerFormatterSinkProvider tcpServerSinkProvider = new BinaryServerFormatterSinkProvider();
				tcpServerSinkProvider.TypeFilterLevel = TypeFilterLevel.Full;				

				ListDictionary tcpChannelProperties = new ListDictionary();
				tcpChannelProperties.Add("name", "SeparatorAPI");
				tcpChannelProperties.Add("port", 3149);

				TcpChannel tcpChannel = new TcpChannel(tcpChannelProperties, tcpClientSinkProvider, tcpServerSinkProvider);

				ChannelServices.RegisterChannel(tcpChannel, false);

				RemotingConfiguration.RegisterWellKnownServiceType(
					typeof(InstrumentControlEventSink),
					RemotingInfo.GetInstance().IcsEventSinkURI,
					WellKnownObjectMode.Singleton);

                myTCPSvr = OrcaTCPServer.GetInstance();
                myTCPSvr.TCPPort = myTCP_Port;
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

            myTCPSvr.Start();
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
            myTCPSvr.Stop();
        }		

        public IInstrumentControlEvents InstrumentControlEventSink
        {
            get
            {
                return theInstrumentControlEventSink as IInstrumentControlEvents;
            }
        }

        public void CloseLogFileSystem()
        {
            // Close the application log file
            try
            {
                if (SeparatorImpl.isLogFileClose == false)
                {
                    LogFile.GetInstance().Dispose();
                    string FinalLogFileName = GetLogsPath() + @"\robosep_ui.log";
                    System.IO.File.Delete(FinalLogFileName);
                    System.IO.File.Move(WorkingLofFileName, FinalLogFileName);
                    SeparatorImpl.isLogFileClose = true;
                    System.Diagnostics.Debug.WriteLine(@">>>> LogFile is closed and renamed.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(@">>>> Exception error on LogFile is closing.");            
            }
            finally
            {
                System.Diagnostics.Debug.WriteLine(@">>>> Finalization on LogFile is closing.");                            
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
                string sTemp = GetCurrentTimeStamp();
                sTemp += ROBOSEP_LOG_FILENAME;
#if false
                LogFile.GetInstance().Initialise(GetLogsPath(), "robosep_ui.log");
#else
                LogFile.GetInstance().Initialise(GetLogsPath(), sTemp);
                WorkingLofFileName = GetLogsPath() + @"\" + sTemp;
#endif
				// Create the Separator layer
				separatorImpl = SeparatorImpl.GetInstance();
				if (separatorImpl != null)
				{
                    SeparatorImpl.isLogFileClose = false;
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

                SeparatorImpl.GetInstance().CloseLogFileSystem();
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

        private static string GetCurrentTimeStamp()
        {
            DateTime value = DateTime.Now;
            return value.ToString("yyMMdd_HHmm");
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

        public XmlRpcProtocolCommand[] getProtocolCommandList(int protocolIdx)
        {
            return myInstrument.getProtocolCommandList(protocolIdx);
        }

        public bool IsSchedulerBusy()
        {
            return myInstrument.IsSchedulerBusy();
        }
    }
}
