using System;
using System.Collections.Generic;
using System.Text;

using System.Diagnostics;
using System.Configuration;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;

using Invetech.ApplicationLog;
using Tesla.InstrumentControl;
using System.Threading;
using System.Windows.Forms;

//FIX TODOS IN THIS FILE!!!

namespace Tesla.Service
{
    //TODO: MAKE THIS SINGLETON
    class InstrumentComms
    {

        #region Constants
        // These are the python method names to get a device's status
        const string getPositionMethod = "Position";
        const string getAngleMethod = "Theta";
        const string getVolumeMethod = "Volume";
        const string getValvePositionMethod = "_ValvePosition";
        const string getHomeMethod = "getHomeStatus";
        const string getLimitMethod = "getLimitStatus";
        const string getStepperStatusMethod = "getStepperStatus";
        const string getTipStripperEngagedMethod = "getEngageDriveStatus";
        const string getTipStripperDisengagedMethod = "getDisengageDriveStatus";
        const string getDeviceUsageMethod = "getDeviceUsage";
        const string getTipStripperIsEngagedMethod = "IsEngaged";

        

        // Names of the instrument devices that are linked to the status screens
        const string instrumentObjectName = "Subsystem: Instrument";
        const string zAxisObjectName = "Device: Robot_ZAxis";
        const string thetaAxisObjectName = "Device: Robot_ThetaAxis";
        const string carouselObjectName = "Device: Carousel";
        const string pumpObjectName = "Device: Pump";
        const string tipStripperObjectName = "Device: TipStripper";
        
        const string THETA_AXIS = "Robot_ThetaAxis";
        const string Z_AXIS = "Robot_ZAxis";
        const string CAROUSEL_AXIS = "Carousel";

        public const string UNITS_MM = "mm";
        public const string UNITS_DEG = "deg";
        public const string UNITS_UL = "uL";

        #endregion

        #region Enums
        public enum CommsStates
        {
            CONNECTED = 0,
            DISCONNECTED = 1,
            CONNECTING = 2,
            COMMS_ERROR = 3
        };
        public enum InstrumentStates
        {
            RESET = 0,
            INITIALISE = 1,
            IDLE = 2,
            RUNNING = 3,
            PAUSED = 4,
            HALTED = 5,
            SHUTTINGDOWN = 6,
            SHUTDOWN = 7,
            OFF = 8,
            ESTOP = 9,
            SERVICE = 10,
            UNKNOWN = 11
        };

        public enum servicePermissionLevel { none, standard, advanced, superUser };
        #endregion

        #region Variables
        private XmlRpcServiceInterface theXmlRpcServiceInterface;
        private ServiceXmlRpcProxy theXmlRpcProxy;
        private IServiceEvents theServiceEventSink;
        private CommandExecuter theCommandExecuter;
        private CommandExecuter theStatusExecuter;
        private CommsStates m_commsState;
        private InstrumentStates m_currentInstrumentState;

        private servicePermissionLevel m_permissionLevel;
        private static InstrumentComms theInstrumentComms = null;

        #region machine stepper positions etc
        private string m_zAxisPosition;
        private string m_zAxisHome;
        private string m_zAxisLimit;
        private string m_zAxisStepper;

        private string m_thetaAxisPosition;
        private string m_thetaAxisHome;
        private string m_thetaAxisLimit;
        private string m_thetaAxisStepper;

        private string m_carouselPosition;
        private string m_carouselHome;
        private string m_carouselLimit;
        private string m_carouselStepper;

        private string m_pumpVolume;
        private string m_pumpHome;
        private string m_pumpLimit;
        private string m_pumpStepper;
        private string m_pumplValve;

        private string m_tipStripperDriveEngaged;
        private string m_tipStripperDriveDisengaged;
        private string m_tipStripperHome;
        private string m_tipStripperLimit;

        private string m_zAxisUsage;
        private string m_thetaAxisUsage;
        private string m_carouselUsage;
        private string m_pumpUsage;
        private string m_tipStripperUsage;

        public string zAxisPosition { get { try { return m_zAxisPosition == null ? "" : Math.Round(Convert.ToDouble(m_zAxisPosition), 2).ToString(); } catch { return ""; } } }
        public string zAxisHome { get { return m_zAxisHome; } }
        public string zAxisLimit { get { return m_zAxisLimit; } }
        public string zAxisStepper { get { return m_zAxisStepper; } }

        public string thetaAxisPosition { get { try { return m_thetaAxisPosition == null ? "" : Math.Round(Convert.ToDouble(m_thetaAxisPosition), 2).ToString(); } catch { return ""; } } }
        public string thetaAxisHome { get { return m_thetaAxisHome; } }
        public string thetaAxisLimit { get { return m_thetaAxisLimit; } }
        public string thetaAxisStepper { get { return m_thetaAxisStepper; } }

        public string carouselPosition { get { try { return m_carouselPosition == null ? "" : Math.Round(Convert.ToDouble(m_carouselPosition), 2).ToString(); } catch { return ""; } } }
        public string carouselHome { get { return m_carouselHome; } }
        public string carouselLimit { get { return m_carouselLimit; } }
        public string carouselStepper { get { return m_carouselStepper; } }

        public string pumpVolume { get { try { return m_pumpVolume == null ? "" : Math.Round(Convert.ToDouble(m_pumpVolume), 2).ToString(); } catch { return ""; } } }
        public string pumpHome { get { return m_pumpHome; } }
        public string pumpLimit { get { return m_pumpLimit; } }
        public string pumpStepper { get { return m_pumpStepper; } }
        public string pumplValve { get { return m_pumplValve; } }

        public string tipStripperDriveEngaged { get { return m_tipStripperDriveEngaged; } }
        public string tipStripperDriveDisengaged { get { return m_tipStripperDriveDisengaged; } }
        public string tipStripperHome { get { return m_tipStripperHome; } }
        public string tipStripperLimit { get { return m_tipStripperLimit; } }

        public string zAxisUsage { get { return m_zAxisUsage; } }
        public string thetaAxisUsage { get { return m_thetaAxisUsage; } }
        public string carouselUsage { get { return m_carouselUsage; } }
        public string pumpUsage { get { return m_pumpUsage; } }
        public string tipStripperUsage { get { return m_tipStripperUsage; } }
        #endregion

        public CommsStates commState
        {
            get { return m_commsState; }
        }

        #endregion

        private InstrumentComms()
        {
            m_commsState = CommsStates.DISCONNECTED;
            m_currentInstrumentState = InstrumentStates.UNKNOWN;
            m_permissionLevel = servicePermissionLevel.none;
        }

        public static InstrumentComms getInstance()
        {
            if (theInstrumentComms == null)
            {
                theInstrumentComms = new InstrumentComms();
            }
            return theInstrumentComms;
        }

        #region Event handler methods

        static public event ReportCommsStateDelegate ReportCommsState;
        static public event ReportStatusUpdateDelegate ReportStatusUpdate;
        static public event ReportCommandCompleteDelegate ReportCommandComplete;
        
        public void ReportCommsStateHandler(string commsState)
        {
            if (commsState == "Connected")
                m_commsState = CommsStates.CONNECTED;
            else if (commsState == "Disconnected")
                m_commsState = CommsStates.DISCONNECTED;
            else if (commsState == "Connecting")
                m_commsState = CommsStates.CONNECTING;
            else
                m_commsState = CommsStates.COMMS_ERROR;

            //Break the thread here by posting to the main dialog
            if (ReportCommsState != null)
                ReportCommsState(commsState);
        }
        public void RunCompleteHandler()
        {
            string str = "RunCompleteHandler called in Instruement Comms";
            GUI_Controls.uiLog.SERVICE_LOG(this, "RunCompleteHandler", GUI_Controls.uiLog.LogLevel.DEBUG, str);
            MessageBox.Show(str, "Info", MessageBoxButtons.OK);
        }
        private void AddEventHandlers()
        {
            if (theServiceEventSink != null)
            {
                try
                {
                    theServiceEventSink.ReportRunComplete += (new RunCompleteDelegate(RunCompleteHandler));
                    theServiceEventSink.ReportStatus += (new ReportStatusDelegate(ReportStatusHandler));
                    theServiceEventSink.ReportError += (new ReportErrorDelegate(ReportErrorHandler));
                }
                catch
                {
                }
            }
        }
        private void RemoveEventHandlers()
        {
            if (theServiceEventSink != null)
            {
                try
                {
                    theServiceEventSink.ReportRunComplete -= (new RunCompleteDelegate(RunCompleteHandler));
                    theServiceEventSink.ReportStatus -= (new ReportStatusDelegate(ReportStatusHandler));
                    theServiceEventSink.ReportError -= (new ReportErrorDelegate(ReportErrorHandler));
                }
                catch
                {
                }
            }
        }
        public void ReportCommandCompleteHandler(bool commandSucceeded, bool sequenceComplete, string returnString)
        {
            string logEvent = "Command Complete : commandSucceeded = "
                + commandSucceeded.ToString()
                + ", sequenceComplete = "
                + sequenceComplete.ToString()
                + ", Return value/Error = "
                + returnString + "\n";

            GUI_Controls.uiLog.SERVICE_LOG(this, "ReportCommandCompleteHandler", GUI_Controls.uiLog.LogLevel.DEBUG, logEvent);
            if (commandSucceeded)
            {
                // do something with the return value
                if (returnString != null && returnString != "")
                {
                    string errorMsg = "Received response:\n" + returnString;
                    GUI_Controls.uiLog.SERVICE_LOG(this, "ReportCommandCompleteHandler", GUI_Controls.uiLog.LogLevel.ERROR, errorMsg);
                    //AfxMessageBox(errorMsg,MB_TOPMOST | MB_SETFOREGROUND | MB_OK);
                }
            }
            else
            {
                // notify user of error
                string errorMsg = "Command failed to execute. Command sequence aborted.\n" + returnString;
                GUI_Controls.uiLog.SERVICE_LOG(this, "ReportCommandCompleteHandler", GUI_Controls.uiLog.LogLevel.ERROR, errorMsg);
                //AfxMessageBox(errorMsg,MB_TOPMOST | MB_SETFOREGROUND | MB_OK);
            }

            //instance().m_pRequestingDialog.PostMessage(SDLG_CMD_SEQ_COMPLETE, (ULONG)sequenceComplete);


            if (ReportCommandComplete != null)
                ReportCommandComplete(commandSucceeded, sequenceComplete, returnString);
        }
        public void ReportErrorHandler(int severityLevel, string errorMessage)
        {
            string str = "An instrument error has occured:\n";
            str += "   Severity level = " + severityLevel;
            str += "\n   Error message = " + errorMessage + "\n";
            GUI_Controls.uiLog.SERVICE_LOG(this, "ReportErrorHandler", GUI_Controls.uiLog.LogLevel.ERROR, str);
            //AfxMessageBox(str,MB_TOPMOST | MB_SETFOREGROUND | MB_OK);
        }
        public void ReportStatusHandler(string state, string statusMessage)
        {
            GUI_Controls.uiLog.SERVICE_LOG(this, "ReportStatusHandler", GUI_Controls.uiLog.LogLevel.DEBUG, "Instrument reported state information. State = " + state);
            if (statusMessage != null)
            {
                string str = "Status message: " + statusMessage;
                GUI_Controls.uiLog.SERVICE_LOG(this, "ReportStatusHandler", GUI_Controls.uiLog.LogLevel.DEBUG, str);
                // display to user
                //sm_pServiceDlg.outputError(str);
            }

            setInstrumentState(state);
        }
        private void setInstrumentState(string state)
        {
            if (state == "RESET")
                m_currentInstrumentState = InstrumentStates.RESET;
            else if (state == "INIT")
                m_currentInstrumentState = InstrumentStates.INITIALISE;
            else if (state == "IDLE")
                m_currentInstrumentState = InstrumentStates.IDLE;
            else if (state == "RUNNING")
                m_currentInstrumentState = InstrumentStates.RUNNING;
            else if (state == "PAUSED")
                m_currentInstrumentState = InstrumentStates.PAUSED;
            else if (state == "HALTED")
                m_currentInstrumentState = InstrumentStates.HALTED;
            else if (state == "SHUTTINGDOWN")
                m_currentInstrumentState = InstrumentStates.SHUTTINGDOWN;
            else if (state == "SHUTDOWN")
                m_currentInstrumentState = InstrumentStates.SHUTDOWN;
            else if (state == "OFF")
                m_currentInstrumentState = InstrumentStates.OFF;
            else if (state == "ESTOP")
                m_currentInstrumentState = InstrumentStates.ESTOP;
            else if (state == "SERVICE")
                m_currentInstrumentState = InstrumentStates.SERVICE;
            else
            {
                m_currentInstrumentState = InstrumentStates.UNKNOWN;
                //TODO error
            }

            //NOTE don't update any of the status dialogs or post messages until after the current
            //state has been changed (above) as the main thread may be blocked waiting for the instrument
            //to go IDLE and thus cannot process dialog updates.
            //((CStatusInstDlg*)m_statusDlg[INSTRUMENT]).setState(state);

            //if( sm_pServiceDlg != NULL )
            //sm_pServiceDlg.PostMessage(SDLG_INSTRUMENT_STATE_CHANGE, (ULONG)instance().m_currentInstrumentState, 0);
        }
        private void AddStaticEventHandlers()
        {
            XmlRpcServiceInterface.ReportCommsState += (new ReportCommsStateDelegate(ReportCommsStateHandler));
        }
        private void RemoveStaticEventHandlers()
        {
            XmlRpcServiceInterface.ReportCommsState -= (new ReportCommsStateDelegate(ReportCommsStateHandler));
        }
        private InstrumentStates getCurrentInstrumentState()
        {
            GUI_Controls.uiLog.SERVICE_LOG(this, "getCurrentInstrumentState", GUI_Controls.uiLog.LogLevel.DEBUG, "getCurrentInstrumentState");
            try
            {
                string state = (string)theXmlRpcProxy.GetInstrumentState();
                setInstrumentState(state);
                GUI_Controls.uiLog.SERVICE_LOG(this, "getCurrentInstrumentState", GUI_Controls.uiLog.LogLevel.DEBUG, "Requested Instrument State. Received: " + state);
            }
            catch (Exception ex)
            {
                string errorMsg = "Connection Lost\n" + ex.Message;
                GUI_Controls.uiLog.SERVICE_LOG(this, "getCurrentInstrumentState", GUI_Controls.uiLog.LogLevel.ERROR, errorMsg);
                //AfxMessageBox(errorMsg,MB_TOPMOST | MB_SETFOREGROUND | MB_OK);
            }
            return m_currentInstrumentState;
        }
        public void ReportStatusUpdateHandler(string obj, string method, string arguments, string returnString)
        {
            GUI_Controls.uiLog.SERVICE_LOG(this, "ReportStatusUpdateHandler", GUI_Controls.uiLog.LogLevel.DEBUG, "ReportStatusUpdateHandler - Status Update Event Received: " +
                "      object = " + obj +
             ", method = " + method +
             ", arguments = " + arguments +
             ", Return value = " + returnString + "\n");


            // We need to establish what has been updated:
            // Device usage check must go first
            if (method.CompareTo(getDeviceUsageMethod) == 0)
                updateUsageStatus(obj, returnString);
            else if (obj.CompareTo(zAxisObjectName) == 0)
                updateZAxisStatus(method, returnString);
            else if (obj.CompareTo(thetaAxisObjectName) == 0)
                updateThetaAxisStatus(method, returnString);
            else if (obj.CompareTo(carouselObjectName) == 0)
                updateCarouselStatus(method, returnString);
            else if (obj.CompareTo(pumpObjectName) == 0)
                updatePumpStatus(method, returnString);
            else if (obj.CompareTo(tipStripperObjectName) == 0)
                updateTipStripperStatus(method, returnString);
            else
            {
                // Just ignore anything else
            }

            if (ReportStatusUpdate != null)
                ReportStatusUpdate(obj, method, arguments, returnString);
            
        }


        #endregion

        #region Connection

        public void ConfigureInstrument(string instIPAddress)
        {
            string instrumentIP;
            string instrumentPort;
            string eventSinkIP;
            string eventSinkPort;
            try
            {
                NameValueCollection nvc = (NameValueCollection)ConfigurationManager.GetSection("Service/ServerConnection");
                instrumentIP = (nvc.Get("instrumentIP"));
                instrumentPort = (nvc.Get("instrumentPort"));
                eventSinkIP = (nvc.Get("eventSinkIP"));
                eventSinkPort = (nvc.Get("eventSinkPort"));
            }
            catch
            {
                // Use default init values in case the parse fails
                instrumentIP = "localhost";
                instrumentPort = "8000";
                eventSinkIP = "localhost";
                eventSinkPort = "8003";
            }
            if (instIPAddress != null && instIPAddress != "")
            {
                instrumentIP = instIPAddress;
                eventSinkIP = instIPAddress;
            }
            XmlRpcServiceInterface.SetConnectionSettings(instrumentIP, instrumentPort, eventSinkIP, eventSinkPort);
        }
        public void connectToInstrument(string instIPAddress)
        {
            GUI_Controls.uiLog.SERVICE_LOG(this, "connectToInstrument", GUI_Controls.uiLog.LogLevel.DEBUG, "Connect To Instrument : IP => " + instIPAddress);

            // It takes a considerable amount of time for the XmlRpcServiceInterface dll to load,
            // so we update the comms status to provide user feedback before calling it.
            ReportCommsStateHandler("Connecting");

            ConfigureInstrument(instIPAddress);

            Thread oThread = new Thread(new ThreadStart(ConnectionRoutine));
            oThread.IsBackground = true;

            // Start the thread
            oThread.Start();

            //AfxBeginThread(connectThreadProc, NULL);
            // the new thread will do the rest, then return
        }
        public void ConnectionRoutine()
        {
            GUI_Controls.uiLog.SERVICE_LOG(this, "ConnectionRoutine", GUI_Controls.uiLog.LogLevel.DEBUG, "Connecting to Instrument");

            AddStaticEventHandlers();

            theXmlRpcServiceInterface = XmlRpcServiceInterface.GetInstance();

            //Check if the XmlRpcServiceInterface started successfully
            if (theXmlRpcServiceInterface == null)
            {
                RemoveStaticEventHandlers();
                GUI_Controls.uiLog.SERVICE_LOG(this, "ConnectionRoutine", GUI_Controls.uiLog.LogLevel.ERROR, "Error connecting to instrument, check IP address");
                MessageBox.Show("Error connecting to instrument, check IP address.", "Conncection Error", MessageBoxButtons.OK);
                //AfxMessageBox(errorMsg,MB_TOPMOST | MB_SETFOREGROUND | MB_OK);
            }
            else
            {
                theXmlRpcProxy = theXmlRpcServiceInterface.XmlRpcProxy;
                theServiceEventSink = theXmlRpcServiceInterface.ServiceEventSink;

                // Add the non-static event handlers
                AddEventHandlers();

                // Start up the worker thread to manage instrument command execution
                theCommandExecuter = new CommandExecuter();

                // Add the event handlers so we are notified when commands complete
                theCommandExecuter.ReportCommandComplete += (
                    new ReportCommandCompleteDelegate(ReportCommandCompleteHandler));

                theStatusExecuter = new CommandExecuter();
                theStatusExecuter.ReportStatusUpdate += (
                    new ReportStatusUpdateDelegate(ReportStatusUpdateHandler));

                // Attempt to enter service state
                getCurrentInstrumentState();
                try
                {
                    EnterServiceState();
                }
                catch (Exception ex)
                {
                    string errorMsg = "Could not enter service state. " + (string)ex.Message;
                    GUI_Controls.uiLog.SERVICE_LOG(this, "ConnectionRoutine", GUI_Controls.uiLog.LogLevel.ERROR, errorMsg);
                    MessageBox.Show("Server could not enter service state. Please try again when the server is not busy.", "Conncection Error", MessageBoxButtons.OK);
                
                    //AfxMessageBox(errorMsg,MB_TOPMOST | MB_SETFOREGROUND | MB_OK);
                }
            }
        }

        public void disconnectFromInstrument()
        {
            GUI_Controls.uiLog.SERVICE_LOG(this, "disconnectFromInstrument", GUI_Controls.uiLog.LogLevel.DEBUG, "Disconnect From Instrument");

            m_commsState = CommsStates.DISCONNECTED; //let the machine thinks it is already disconnect to prevent new commands from coming in

            Thread oThread = new Thread(new ThreadStart(DisconnectionRoutine));
            oThread.IsBackground = true;
            // Start the thread
            oThread.Start();
            //AfxBeginThread(disconnectThreadProc, NULL);
            // the new thread will do the rest, then return
        }
        public void DisconnectionRoutine()
        {
            // return the instrument back to its 'normal' state
            try
            {
                ExitServiceState();
            }
            catch (Exception ex)
            {
                string errorMsg = "Could not exit service state. " + ex.Message;
                GUI_Controls.uiLog.SERVICE_LOG(this, "DisconnectionRoutine", GUI_Controls.uiLog.LogLevel.ERROR, errorMsg);

                //AfxMessageBox(errorMsg,MB_TOPMOST | MB_SETFOREGROUND | MB_OK);
            }

            theXmlRpcProxy = null;
            theServiceEventSink = null;
            if (theXmlRpcServiceInterface != null)
            {
                theXmlRpcServiceInterface = XmlRpcServiceInterface.DestroyInstance();
            }
            setInstrumentState("UNKNOWN");

            RemoveEventHandlers();
            RemoveStaticEventHandlers();
            
            if (theCommandExecuter != null)
            {
                theCommandExecuter.Dispose();
                theCommandExecuter = null;
            }
            if (theStatusExecuter != null)
            {
                theStatusExecuter.Dispose();
                theStatusExecuter = null;
            }
            
            // Disable the back channel
            // REVISIT - TBD (InstrumentControlEventSink should call back into this
            // singleton to report its events; its clients should register with
            // event objects exposed by this class).

            // REVISIT - need to indicate to the user somehow that the connection to the
            // instrument has been lost!!

        }

        // REVISIT: Add a service state button. Enable/disable (or change action) according to state
        public void EnterServiceState()
        {
            GUI_Controls.uiLog.SERVICE_LOG(this, "EnterServiceState", GUI_Controls.uiLog.LogLevel.DEBUG, "Enter Service State");

            try
            {
                if (getCurrentInstrumentState() != InstrumentStates.SERVICE)
                {
                    // wait for idle state
                    //TODO: maxWaitForIdle
                    int maxWaitForIdle = 10; //atoi( CConfig::GetStringSetting(STR_CONNECTION,_T("MaxWaitForIdleState") , _T("10000")) );
                    int sleepTime = 100;
                    int numRetries = (int)(maxWaitForIdle / sleepTime);
                    for (int i = 0; (m_currentInstrumentState != InstrumentStates.IDLE) && i <= numRetries; i++)
                    {
                        Thread.Sleep(sleepTime);
                    }
                    if (m_currentInstrumentState != InstrumentStates.IDLE)
                        throw new System.Exception("Can not enter SERVICE state becuase the instrument is not in IDLE idle.");

                    // attempt to enter service state
                    for (int i = 0; (getCurrentInstrumentState() != InstrumentStates.SERVICE) && i < 10; i++)
                    {
                        theXmlRpcProxy.EnterServiceState();

                        // at the moment the instrument doesn't notify us of a service state change, so find out ourselves
                        if (getCurrentInstrumentState() != InstrumentStates.SERVICE)
                            Thread.Sleep(sleepTime);
                    }

                    if (m_currentInstrumentState != InstrumentStates.SERVICE)
                        throw new System.Exception("Request to enter service state was not successful.");
                }
            }
            catch (Exception ex)
            {
                // log any errors
                string errorMsg = "Error Detected : " + "Could not enter service state. Exception caught: " + ex.Message;

                GUI_Controls.uiLog.SERVICE_LOG(this, "EnterServiceState", GUI_Controls.uiLog.LogLevel.ERROR, errorMsg);
                throw;
            }
        }
        public void ExitServiceState()
        {
            GUI_Controls.uiLog.SERVICE_LOG(this, "ExitServiceState", GUI_Controls.uiLog.LogLevel.DEBUG, "Exit Service State");

            try
            {
                if (theXmlRpcProxy != null)
                {
                    if (getCurrentInstrumentState() == InstrumentStates.SERVICE)
                    {
                        if (!(theXmlRpcProxy.ExitServiceState()))
                            throw new Exception("Request to exit service state was refused.");
                    }
                }
            }
            catch (Exception ex)
            {
                // log any errors
                string errorMsg = "Error Detected : " + "Could not exit service state. Exception caught: " + ex.Message;

                GUI_Controls.uiLog.SERVICE_LOG(this, "ExitServiceState", GUI_Controls.uiLog.LogLevel.ERROR, errorMsg);
                throw;
            }
        }

        #endregion



        #region Get Machine States
        //public float getThetaDeg();
        //public float getCarouselDeg();
        //public float getZmm();
        public void getZAxisState()
        {
            GUI_Controls.uiLog.SERVICE_LOG(this, "getZAxisState", GUI_Controls.uiLog.LogLevel.DEBUG, 
                "Requesting device state: Z Axis");

            // Queue up the status commands, and then start them executing
            try
            {
                bool success = true;
                if (!theStatusExecuter.AddStatusCommand(zAxisObjectName, getPositionMethod, ""))
                    success = false;
                if (!theStatusExecuter.AddStatusCommand(zAxisObjectName, getHomeMethod, ""))
                    success = false;
                //		if( !theStatusExecuter.AddStatusCommand(zAxisObjectName, getLimitMethod, "") )
                //			success = false;
                if (!theStatusExecuter.AddStatusCommand(zAxisObjectName, getStepperStatusMethod, ""))
                    success = false;
                if (success)
                    success = theStatusExecuter.Start();
                else
                    theStatusExecuter.Abort();
            }
            catch (Exception ex)
            {
                string errorMsg = "Error requesting device status update: Z Axis. Exception: ";
                errorMsg += ex.Message;

                GUI_Controls.uiLog.SERVICE_LOG(this, "getZAxisState", GUI_Controls.uiLog.LogLevel.ERROR, errorMsg);
                MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK);
                disconnectFromInstrument();
            }
        }


        public void getThetaAxisState()
        {
            GUI_Controls.uiLog.SERVICE_LOG(this, "getThetaAxisState", GUI_Controls.uiLog.LogLevel.DEBUG, "Request Device State : Theta Axis");

            // Queue up the status commands, and then start them executing
            try
            {
                bool success = true;
                if (!theStatusExecuter.AddStatusCommand(thetaAxisObjectName, getAngleMethod, ""))
                    success = false;
                if (!theStatusExecuter.AddStatusCommand(thetaAxisObjectName, getHomeMethod, ""))
                    success = false;
                //		if( !theStatusExecuter.AddStatusCommand(thetaAxisObjectName, getLimitMethod, "") )
                //			success = false;
                if (!theStatusExecuter.AddStatusCommand(thetaAxisObjectName, getStepperStatusMethod, ""))
                    success = false;
                if (success)
                    success = theStatusExecuter.Start();
                else
                    theStatusExecuter.Abort();
            }
            catch (Exception ex)
            {
                string errorMsg = "Error requesting device status update: Theta Axis. Exception: ";
                errorMsg += ex.Message;
                GUI_Controls.uiLog.SERVICE_LOG(this, "getThetaAxisState", GUI_Controls.uiLog.LogLevel.ERROR, errorMsg);
                MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK);

                disconnectFromInstrument();
            }
        }


        public void getCarouselState()
        {
            GUI_Controls.uiLog.SERVICE_LOG(this, "getCarouselState", GUI_Controls.uiLog.LogLevel.DEBUG, "Request Device State : Carousel");

            // Queue up the status commands, and then start them executing
            try
            {
                bool success = true;
                if (!theStatusExecuter.AddStatusCommand(carouselObjectName, getAngleMethod, ""))
                    success = false;
                if (!theStatusExecuter.AddStatusCommand(carouselObjectName, getHomeMethod, ""))
                    success = false;
                //		if( !theStatusExecuter.AddStatusCommand(carouselObjectName, getLimitMethod, "") )
                //			success = false;
                if (!theStatusExecuter.AddStatusCommand(carouselObjectName, getStepperStatusMethod, ""))
                    success = false;
                if (success)
                    success = theStatusExecuter.Start();
                else
                    theStatusExecuter.Abort();
            }
            catch (Exception ex)
            {
                string errorMsg = "Error requesting device status update: Carousel. Exception: ";
                errorMsg += (string)ex.Message;
                GUI_Controls.uiLog.SERVICE_LOG(this, "getCarouselState", GUI_Controls.uiLog.LogLevel.ERROR, errorMsg);
                
                MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK);
                disconnectFromInstrument();
            }

        }

        public void getPumpState()
        {
            GUI_Controls.uiLog.SERVICE_LOG(this, "getPumpState", GUI_Controls.uiLog.LogLevel.DEBUG, "Request Device State : Pump");

            // Queue up the status commands, and then start them executing
            try
            {
                bool success = true;
                if (!theStatusExecuter.AddStatusCommand(pumpObjectName, getVolumeMethod, ""))
                    success = false;
                if (!theStatusExecuter.AddStatusCommand(pumpObjectName, getHomeMethod, ""))
                    success = false;
                //		if( !theStatusExecuter.AddStatusCommand(pumpObjectName, getLimitMethod, "") )
                //			success = false;
                if (!theStatusExecuter.AddStatusCommand(pumpObjectName, getStepperStatusMethod, ""))
                    success = false;
                if (!theStatusExecuter.AddStatusCommand(pumpObjectName, getValvePositionMethod, ""))
                    success = false;
                if (success)
                    success = theStatusExecuter.Start();
                else
                    theStatusExecuter.Abort();
            }
            catch (Exception ex)
            {
                string errorMsg = "Error requesting device status update: Pump. Exception: ";
                errorMsg += (string)ex.Message;
                GUI_Controls.uiLog.SERVICE_LOG(this, "getPumpState", GUI_Controls.uiLog.LogLevel.ERROR, errorMsg);
                MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK);
                disconnectFromInstrument();
            }

        }

        public void getTipStripperState()
        {
            GUI_Controls.uiLog.SERVICE_LOG(this, "getTipStripperState", GUI_Controls.uiLog.LogLevel.DEBUG, "Request Device State : Tip Stripper");

            // Queue up the status commands, and then start them executing
            try
            {
                bool success = true;
                if (!theStatusExecuter.AddStatusCommand(tipStripperObjectName, getTipStripperIsEngagedMethod, ""))
                    success = false;
                /*    
                if (!theStatusExecuter.AddStatusCommand(tipStripperObjectName, getTipStripperEngagedMethod, ""))
                    success = false;
                if (!theStatusExecuter.AddStatusCommand(tipStripperObjectName, getTipStripperDisengagedMethod, ""))
                    success = false;
                if (!theStatusExecuter.AddStatusCommand(tipStripperObjectName, getHomeMethod, ""))
                    success = false;
                if (!theStatusExecuter.AddStatusCommand(tipStripperObjectName, getLimitMethod, ""))
                    success = false;
                */
                if (success)
                    success = theStatusExecuter.Start();
                else
                    theStatusExecuter.Abort();
            }
            catch (Exception ex)
            {
                string errorMsg = "Error requesting device status update: Tip Stripper. Exception: ";
                errorMsg += (string)ex.Message;
                GUI_Controls.uiLog.SERVICE_LOG(this, "getTipStripperState", GUI_Controls.uiLog.LogLevel.ERROR, errorMsg);
                
                MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK);
                disconnectFromInstrument();
            }
        }

        public void getUsageState()
        {
            GUI_Controls.uiLog.SERVICE_LOG(this, "getUsageState", GUI_Controls.uiLog.LogLevel.DEBUG, "Request Device State : Device Usage");

            // Queue up the status commands, and then start them executing
            try
            {
                bool success = true;
                if (!theStatusExecuter.AddStatusCommand(zAxisObjectName, getDeviceUsageMethod, ""))
                    success = false;
                if (!theStatusExecuter.AddStatusCommand(thetaAxisObjectName, getDeviceUsageMethod, ""))
                    success = false;
                if (!theStatusExecuter.AddStatusCommand(carouselObjectName, getDeviceUsageMethod, ""))
                    success = false;
                if (!theStatusExecuter.AddStatusCommand(pumpObjectName, getDeviceUsageMethod, ""))
                    success = false;
                if (!theStatusExecuter.AddStatusCommand(tipStripperObjectName, getDeviceUsageMethod, ""))
                    success = false;
                if (success)
                    success = theStatusExecuter.Start();
                else
                    theStatusExecuter.Abort();
            }
            catch (Exception ex)
            {
                string errorMsg = "Error requesting device status update: Device Usage. Exception: ";
                errorMsg += (string)ex.Message;
                GUI_Controls.uiLog.SERVICE_LOG(this, "getUsageState", GUI_Controls.uiLog.LogLevel.ERROR, errorMsg);
                
                MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK);
                disconnectFromInstrument();
            }

        }

        #endregion

        #region Execute Commands

        public void moveAxis(string axis, bool isRelativeMove, float position)
        {
            GUI_Controls.uiLog.SERVICE_LOG(this, "moveAxis", GUI_Controls.uiLog.LogLevel.DEBUG, "CInstrumentComms::moveAxis");
            string moveType;
            string moveCommand;
            string axisPosition;
            string blockingParam = "";
            int commandCount = 0;
            bool success = true;

            axisPosition = string.Format("{0:0.0}", position);

            // Need to use different commands depending on the axis
            if (axis == "Robot_ZAxis")
            {
                if (isRelativeMove)
                    moveType = "IncrementPosition";
                else
                    moveType = "SetPosition";
            }
            else if (axis == "Robot_ThetaAxis" ||
                axis == "Carousel")
            {
                if (isRelativeMove)
                    moveType = "IncrementTheta";
                else
                {
                    moveType = "SetTheta";
                    blockingParam = "0";
                }
            }

            // Axis not supported
            else
            {
                //TODO when invalid?
                return;
            }


            moveCommand = "Device:";
            moveCommand += " ";
            moveCommand += axis;
            moveCommand += " ";
            moveCommand += moveType;
            moveCommand += " ";
            moveCommand += axisPosition;
            if (blockingParam != null && blockingParam != "")
            {
                moveCommand += " ";
                moveCommand += blockingParam;
            }
            moveCommand += ";";

            //TODO when fails?
            success = executeCommandSequence(moveCommand, commandCount);
        }

        public void powerAxis(string axis, bool turnOn)
        {
            GUI_Controls.uiLog.SERVICE_LOG(this, "powerAxis", GUI_Controls.uiLog.LogLevel.DEBUG, "CInstrumentComms::powerAxis turnOn="+turnOn);
            string powerType;
            string powerCommand;
            int commandCount = 0;
            bool success = true;

            if (turnOn)
                powerType = "applyPower";
            else
                powerType = "removePower";

            
            powerCommand = "Device:";
            powerCommand += " ";
            powerCommand += axis;
            powerCommand += " ";
            powerCommand += powerType;
            powerCommand += ";";

            //TODO when fails?
            success = executeCommandSequence(powerCommand, commandCount);
        }


        public void moveTipStripper(bool isEngage)
        {
            executeCommandSequence(string.Format("Device: TipStripper {0} ;", (isEngage?"Engage":"Disengage")), 1);
        }

        public bool executeCommandSequence(string commandSequence, int commandCount)
        {
            GUI_Controls.uiLog.SERVICE_LOG(this, "executeCommandSequence", GUI_Controls.uiLog.LogLevel.DEBUG, commandSequence);

            //Before doing anything lets see if the instrument is connected
            //and in the correct state
            if (m_commsState != CommsStates.CONNECTED)
            {
                string errorMsg = "Error starting command sequence, instrument is not connected";
                GUI_Controls.uiLog.SERVICE_LOG(this, "executeCommandSequence", GUI_Controls.uiLog.LogLevel.ERROR, errorMsg);

                MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK);
                return false;
            }

            if (m_currentInstrumentState != InstrumentStates.SERVICE)
            {
                string errorMsg = "Error starting command sequence, instrument is not in the SERVICE state";
                GUI_Controls.uiLog.SERVICE_LOG(this, "executeCommandSequence", GUI_Controls.uiLog.LogLevel.ERROR, errorMsg);
                MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK);
                return false;
            }

            string nextCmdAndParams, sendCmd;
            List<string> parameters = new List<string>();
            string nextParameter;
            string nextCommand;
            string nextObject;
            bool success = false;


            //Reset back to zero before counting commands
            commandCount = 0;

            bool resetCommandSequence = true;

            while (commandSequence.Length != 0)
            {
                nextObject = "";
                nextCommand = "";
                if (commandSequence.IndexOf(';') != -1)
                {
                    // get a command and its parameters (separated by a ';')
                    nextCmdAndParams = commandSequence.Substring(0, commandSequence.IndexOf(';'));

                    // We are assuming that the first 2 words are the object name,
                    // the first of which is "Device: " or "Subsystem: "
                    // TODO: "Device: " and "Subsystem: " should be constants
                    if (nextCmdAndParams.IndexOf("Device: ") != -1)
                    {
                        nextObject = "Device: ";
                        nextCmdAndParams = nextCmdAndParams.Substring(nextCmdAndParams.IndexOf(' ') + 1);
                    }
                    else if (nextCmdAndParams.IndexOf("Subsystem: ") != -1)
                    {
                        nextObject = "Subsystem: ";
                        nextCmdAndParams = nextCmdAndParams.Substring(nextCmdAndParams.IndexOf(' ') + 1);
                    }
                    else
                    {
                        //TODO didn't find a device/subsystem identifier, need to take some action
                        // throw an exception?
                    }
                    if (nextCmdAndParams.IndexOf(' ') != -1)
                    {
                        nextObject += nextCmdAndParams.Substring(0, nextCmdAndParams.IndexOf(' '));
                        nextCmdAndParams = nextCmdAndParams.Substring(nextCmdAndParams.IndexOf(' ') + 1);

                        if (nextCmdAndParams.IndexOf(' ') != -1)
                        {
                            nextCommand = nextCmdAndParams.Substring(0, nextCmdAndParams.IndexOf(' '));
                            nextCmdAndParams = nextCmdAndParams.Substring(nextCmdAndParams.IndexOf(' ') + 1);

                            //If there is more than one parameter get all those up to the last one
                            while (nextCmdAndParams.IndexOf(' ') != -1)
                            {
                                nextParameter = nextCmdAndParams.Substring(0, nextCmdAndParams.IndexOf(' '));
                                parameters.Add(nextParameter);
                                nextCmdAndParams = nextCmdAndParams.Substring(nextCmdAndParams.IndexOf(' ') + 1);
                            }

                            //Now process the last parameter
                            nextParameter = nextCmdAndParams;
                            parameters.Add(nextParameter);
                        }
                        else
                        {
                            nextCommand = nextCmdAndParams;
                        }
                    }
                    else
                    {
                        //TODO: Error - expecting at least an object and a command
                    }

                    commandCount++;

                    //Increment the sequence number
                    //m_commandId++;

                    //Create the object array for the strings
                    string commandParameters = "";
                    for (int i = 0; i < parameters.Count; i++)
                    {
                        if (i != 0)
                            commandParameters += " ";
                        commandParameters += parameters[i];
                    }

                    //Add the command to the list to be executed
                    success = false;
                    try
                    {
                        success = theCommandExecuter.AddCommand(nextObject, nextCommand, commandParameters);
                    }
                    catch
                    {
                    }
                    if (!success)
                    {
                        string errorMsg = "Error adding command to sequence";
                        GUI_Controls.uiLog.SERVICE_LOG(this, "executeCommandSequence", GUI_Controls.uiLog.LogLevel.ERROR, errorMsg);
                        MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK);
                    }
                    //theXmlRpcProxy.Execute(nextObject, nextCommand, commandParameters);

                    //Only reset for the first command in the sequence.
                    resetCommandSequence = false;

                    // delete the command just processed and clear the parameters
                    commandSequence = commandSequence.Substring(commandSequence.IndexOf(';') + 1);
                    parameters.Clear();
                    nextCmdAndParams = "";
                }
                else
                {
                    commandSequence = "";
                }
            }

            //Now call the start method.
            success = false;
            try
            {
                success = theCommandExecuter.Start();

            }
            catch (Exception ex)
            {
                string errorMsg = "Error starting command sequence. Exception: ";
                errorMsg += (string)ex.Message;
                GUI_Controls.uiLog.SERVICE_LOG(this, "executeCommandSequence", GUI_Controls.uiLog.LogLevel.ERROR, errorMsg);
                MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK);
                return false;
            }

            if (!success)
            {
                string errorMsg = "Failed to execute command sequence. Possible command syntax error.";
                GUI_Controls.uiLog.SERVICE_LOG(this, "executeCommandSequence", GUI_Controls.uiLog.LogLevel.ERROR, errorMsg);
                MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK);
            }

            // This will return false is no valid commands were found
            return success;
        }

        public void ReInitInstrument()
        {
            int commandCount = 0;
            executeCommandSequence("Centre reInitInstrument;", commandCount);
        }

        public void abortCommandSequence()
        {
            GUI_Controls.uiLog.SERVICE_LOG(this, "abortCommandSequence", GUI_Controls.uiLog.LogLevel.DEBUG, "Abort Command Sequence");

            theCommandExecuter.Abort();
        }

        public void pauseCommandSequence()
        {
            GUI_Controls.uiLog.SERVICE_LOG(this, "pauseCommandSequence", GUI_Controls.uiLog.LogLevel.DEBUG, "Pause Command Sequence");

            theCommandExecuter.Pause();
        }

        public void resumeCommandSequence()
        {
            GUI_Controls.uiLog.SERVICE_LOG(this, "resumeCommandSequence", GUI_Controls.uiLog.LogLevel.DEBUG, "Resume Command Sequence");

            theCommandExecuter.Start();
        }

        public void getLastErrorEvent()
        {
            GUI_Controls.uiLog.SERVICE_LOG(this, "getLastErrorEvent", GUI_Controls.uiLog.LogLevel.DEBUG, "Request Last Error");

            try
            {
                // get errors from server
                string[] errorList = theXmlRpcProxy.GetErrorLog(20, 0);

                //TODO: send error list to callback
            }
            catch (Exception ex)
            {
                GUI_Controls.uiLog.SERVICE_LOG(this, "getLastErrorEvent", GUI_Controls.uiLog.LogLevel.ERROR, " Get last error failed. Exception" + ex.Message + "\n");
            }
        }

        #endregion

        #region Callback methods
        //GetInstrumentAxisStatusSet
        private static string Z_HEIGHT_KEY = "z_height";
        private static string THETA_DEG_KEY = "theta_deg";
        private static string CAROUSEL_DEG_KEY = "carousel_deg";
        private static string PUMP_VOL_KEY = "pump_vol";
        private static string LOW_BUFFER_LVL_KEY = "low_buffer_lvl";
        private static string STRIPPER_ENGAGED_KEY = "stripper_eng";
        public void updateAxisInfo(Dictionary<string, string> dictionary)
        {
            if (dictionary.ContainsKey(Z_HEIGHT_KEY))
            {
                m_zAxisPosition = dictionary[Z_HEIGHT_KEY];
            }
            if (dictionary.ContainsKey(THETA_DEG_KEY))
            {
                m_thetaAxisPosition = dictionary[THETA_DEG_KEY];
            }
            if (dictionary.ContainsKey(CAROUSEL_DEG_KEY))
            {
                m_carouselPosition = dictionary[CAROUSEL_DEG_KEY];
            }
            if (dictionary.ContainsKey(PUMP_VOL_KEY))
            {
                m_pumpVolume = dictionary[PUMP_VOL_KEY];
            }
            if (dictionary.ContainsKey(LOW_BUFFER_LVL_KEY))
            {
                bufferLevel = dictionary[LOW_BUFFER_LVL_KEY]=="True"?"LOW":"OK";
            }
            if (dictionary.ContainsKey(STRIPPER_ENGAGED_KEY))
            {
                tipstripper = dictionary[STRIPPER_ENGAGED_KEY]=="True"?"ENG":"DIS";
            }
            
        }

        public void updateZAxisStatus(string method, string returnValue)
        {
            GUI_Controls.uiLog.SERVICE_LOG(this, "updateZAxisStatus", GUI_Controls.uiLog.LogLevel.DEBUG, "updateZAxisStatus");

            if (method.CompareTo(getPositionMethod) == 0)
                m_zAxisPosition = returnValue;
            else if (method.CompareTo(getHomeMethod) == 0)
                m_zAxisHome = returnValue;
            else if (method.CompareTo(getLimitMethod) == 0)
                m_zAxisLimit = returnValue;
            else if (method.CompareTo(getStepperStatusMethod) == 0)
                m_zAxisStepper = returnValue;

            //((CStatusZAxisDlg*)m_statusDlg[ZAXIS])->setLastUpdateTime();
        }

        public void updateThetaAxisStatus(string method, string returnValue)
        {
            GUI_Controls.uiLog.SERVICE_LOG(this, "updateThetaAxisStatus", GUI_Controls.uiLog.LogLevel.DEBUG, "updateThetaAxisStatus");

            if (method.CompareTo(getAngleMethod) == 0)
                m_thetaAxisPosition = returnValue;
            else if (method.CompareTo(getHomeMethod) == 0)
                m_thetaAxisHome = returnValue;
            else if (method.CompareTo(getLimitMethod) == 0)
                m_thetaAxisLimit = returnValue;
            else if (method.CompareTo(getStepperStatusMethod) == 0)
                m_thetaAxisStepper = returnValue;

            //((CStatusThetaAxisDlg*)m_statusDlg[THETAAXIS])->setLastUpdateTime();
        }

        public void updateCarouselStatus(string method, string returnValue)
        {
            GUI_Controls.uiLog.SERVICE_LOG(this, "updateCarouselStatus", GUI_Controls.uiLog.LogLevel.DEBUG, "updateCarouselStatus");

            if (method.CompareTo(getAngleMethod) == 0)
                m_carouselPosition = returnValue;
            else if (method.CompareTo(getHomeMethod) == 0)
                m_carouselHome = returnValue;
            else if (method.CompareTo(getLimitMethod) == 0)
                m_carouselLimit = returnValue;
            else if (method.CompareTo(getStepperStatusMethod) == 0)
                m_carouselStepper = returnValue;

            //((CStatusCarouselDlg*)m_statusDlg[CAROUSEL])->setLastUpdateTime();
        }

        public void updatePumpStatus(string method, string returnValue)
        {
            GUI_Controls.uiLog.SERVICE_LOG(this, "updatePumpStatus", GUI_Controls.uiLog.LogLevel.DEBUG, "updatePumpStatus");

            if (method.CompareTo(getVolumeMethod) == 0)
                m_pumpVolume = returnValue;
            else if (method.CompareTo(getHomeMethod) == 0)
                m_pumpHome = returnValue;
            else if (method.CompareTo(getLimitMethod) == 0)
                m_pumpLimit = returnValue;
            else if (method.CompareTo(getStepperStatusMethod) == 0)
                m_pumpStepper = returnValue;
            else if (method.CompareTo(getValvePositionMethod) == 0)
                m_pumplValve = returnValue;

            //((CStatusPumpDlg*)m_statusDlg[PUMP])->setLastUpdateTime();
        }

        public void updateTipStripperStatus(string method, string returnValue)
        {
            GUI_Controls.uiLog.SERVICE_LOG(this, "updateTipStripperStatus", GUI_Controls.uiLog.LogLevel.DEBUG, "updateTipStripperStatus");

            if (method.CompareTo(getTipStripperEngagedMethod) == 0)
                m_tipStripperDriveEngaged = returnValue;
            else if (method.CompareTo(getTipStripperDisengagedMethod) == 0)
                m_tipStripperDriveDisengaged = returnValue;
            else if (method.CompareTo(getHomeMethod) == 0)
                m_tipStripperHome = returnValue;
            else if (method.CompareTo(getLimitMethod) == 0)
                m_tipStripperLimit = returnValue;
            else if (method.CompareTo(getTipStripperIsEngagedMethod) == 0)
                tipstripper = returnValue == "True" ? "ENG" : "DIS";

            //((CStatusTipStripperDlg*)m_statusDlg[TIPSTRIPPER])->setLastUpdateTime();
        }



        public void updateUsageStatus(string obj, string returnValue)
        {
            GUI_Controls.uiLog.SERVICE_LOG(this, "updateUsageStatus", GUI_Controls.uiLog.LogLevel.DEBUG, "updateUsageStatus");

            if (obj.CompareTo(zAxisObjectName) == 0)
                m_zAxisUsage = returnValue;
            else if (obj.CompareTo(thetaAxisObjectName) == 0)
                m_thetaAxisUsage = returnValue;
            else if (obj.CompareTo(carouselObjectName) == 0)
                m_carouselUsage = returnValue;
            else if (obj.CompareTo(pumpObjectName) == 0)
                m_pumpUsage = returnValue;
            else if (obj.CompareTo(tipStripperObjectName) == 0)
                m_tipStripperUsage = returnValue;

            //((CStatusUsageDlg*)m_statusDlg[DEVICEUSAGE])->setLastUpdateTime();
        }

        #endregion



        #region permission
        public void setPermissionLevel(servicePermissionLevel permissionLevel)
        {
            m_permissionLevel = permissionLevel;

            switch (m_permissionLevel)
            {
                case servicePermissionLevel.none:
                    {
                        GUI_Controls.uiLog.SERVICE_LOG(this, "setPermissionLevel", GUI_Controls.uiLog.LogLevel.DEBUG, "User Permission Level : NONE");
                        break;
                    }
                case servicePermissionLevel.standard:
                    {
                        GUI_Controls.uiLog.SERVICE_LOG(this, "setPermissionLevel", GUI_Controls.uiLog.LogLevel.DEBUG, "User Permission Level : STANDARD");
                        break;
                    }
                case servicePermissionLevel.advanced:
                    {
                        GUI_Controls.uiLog.SERVICE_LOG(this, "setPermissionLevel", GUI_Controls.uiLog.LogLevel.DEBUG, "User Permission Level : ADVANCED");
                        break;
                    }
                case servicePermissionLevel.superUser:
                    {
                        GUI_Controls.uiLog.SERVICE_LOG(this, "setPermissionLevel", GUI_Controls.uiLog.LogLevel.DEBUG, "User Permission Level : SUPER");
                        break;
                    }
                default:
                    // missing an enumeration!!
                    Debug.Assert(false);
                    GUI_Controls.uiLog.SERVICE_LOG(this, "setPermissionLevel", GUI_Controls.uiLog.LogLevel.ERROR, "User Permission Level : UNKNOWN");
                    break;
            }
        }

        public string getPermissionLevelAsString()
        {
            string level;

            switch (m_permissionLevel)
            {
                case servicePermissionLevel.none: level = "NONE"; break;
                case servicePermissionLevel.standard: level = "STANDARD"; break;
                case servicePermissionLevel.advanced: level = "ADVANCED"; break;
                case servicePermissionLevel.superUser: level = "SUPER"; break;
                default:
                    // missing an enumeration!!
                    Debug.Assert(false);
                    level = "Error";
                    GUI_Controls.uiLog.SERVICE_LOG(this, "getPermissionLevelAsString", GUI_Controls.uiLog.LogLevel.ERROR, "getPermissionLevelAsString : UNKNOWN");
                    break;
            }

            return level;
        }


        public servicePermissionLevel getPermissionLevel()
        {
            return m_permissionLevel;
        }

        #endregion

        #region settings
        
        public void SetIgnoreLidSensor(bool isOn)
        {
            if (theXmlRpcProxy == null) return;
            theXmlRpcProxy.SetIgnoreLidSensor(isOn);
        }
        public bool GetIgnoreLidSensor()
        {
            return (theXmlRpcProxy == null)?true:theXmlRpcProxy.GetIgnoreLidSensor();
        }
        public void SetTimedStart(bool isOn)
        {
            if (theXmlRpcProxy == null) return;
            theXmlRpcProxy.SetTimedStart(isOn);
        }
        public bool GetTimedStart()
        {
            return (theXmlRpcProxy == null) ? true : theXmlRpcProxy.GetTimedStart();
        }
        public bool SaveIniAsFactory()
        {
            return (theXmlRpcProxy == null) ? true : theXmlRpcProxy.SaveIniAsFactory();
        }
        public bool RestoreIniWithFactory()
        {
            return (theXmlRpcProxy == null) ? true : theXmlRpcProxy.RestoreIniWithFactory();
        }
        public bool LoadSelectedHardwareINI(string path)
        {
            return (theXmlRpcProxy == null) ? true : theXmlRpcProxy.LoadSelectedHardwareINI(path);
        }
        #endregion

        #region New Instrument Status Functions

        public enum InstrumentAxisStatusSet
        {
            NONE = -1,
            Z = 1,
            THETA = 2,
            CAROUSEL = 3,
            PUMP = 4,
            STRIPPER_ARM = 5,
            BUFFER = 6,
            FULLSET_EXCEPT_STRIPPER = 7,
            FULLSET_VERSION_1 = 100
        };
        public Dictionary<string, string> GetInstrumentAxisStatusSet(int combo)
        {
            Dictionary<string, string> statusSet = null;
            if (theXmlRpcProxy != null)
            {
                try
                {
                    string result = theXmlRpcProxy.GetInstrumentAxisStatusSet(combo);
                    if (result != null && result != "")
                    {
                        statusSet = new System.Collections.Generic.Dictionary<string, string>();
                        char[] delimiters = new char[] { ',', ' ' };
                        string[] pairs = result.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string p in pairs)
                        {
                            string[] keyValue = p.Split('=');
                            if (keyValue.Length == 2)
                            {
                                statusSet.Add(keyValue[0], keyValue[1]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Server error. Please restart RoboSep Server and try again.", "Error", MessageBoxButtons.OK);
                }
            }

            return statusSet;
        }
        #endregion


        public string bufferLevel { get; set; }
        public string tipstripper { get; set; }



        public void SetBarcodeRescanOffset(double offset)
        {
            if (theXmlRpcProxy == null) return;
            theXmlRpcProxy.SetBarcodeRescanOffset(offset);
        }
        public double GetBarcodeRescanOffset()
        {
            return (theXmlRpcProxy == null) ? 0 : theXmlRpcProxy.GetBarcodeRescanOffset();
        }
        public void SetBarcodeThetaOffset(double offset)
        {
            if (theXmlRpcProxy == null) return;
            theXmlRpcProxy.SetBarcodeThetaOffset(offset);
        }
        public double GetBarcodeThetaOffset()
        {
            return (theXmlRpcProxy == null) ? 0 : theXmlRpcProxy.GetBarcodeThetaOffset();
        }
    }
}
