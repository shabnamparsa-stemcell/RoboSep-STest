using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Xml;
using System.Text;
using System.Windows.Forms;
using System.Configuration;

using System.Globalization;
using System.Threading;

using Invetech.ApplicationLog;

using Tesla.Common.ResourceManagement;
using Tesla.Common.DrawingUtilities;
using Tesla.Common.Separator;
using Tesla.Common.OperatorConsole;
using Tesla.Common;
using Tesla.Separator;
using Tesla.OperatorConsoleControls;
using Tesla.DataAccess;

using Tesla.Common.Protocol;

using GUI_Controls;

namespace GUI_Console
{
    public partial class RoboSep_UserConsole : Form
    {
        // Define Global Variables
        // Current Control
        public static UserControl ctrlCurrentUserControl;
        // Track open Forms
        private List<FormTracker> frmList_AllForms = new List<FormTracker>();
        // Tab Memory
        public static int intCurrentHelpTab = 0;  // indicates what tab # was most recently active
        public static int intCurrentLogTab = 0;   // indicates what tab # was most recently active
        //Options
        public static bool KeyboardEnabled = true;// indicates whether touch keyboard is enabled
        public static bool bLiquidSensorEnable = true; // indicates whether liquid level sensor is active
        public static bool bLidSensorEnable = true; // indicates whether lid sensor is active
        // User Name
        public static string strCurrentUser = "User Name";     // remembers most recently logged in user
        // State
        public static bool bIsRunning;          // Boolean set to TRUE if RoboSep is currently running protocols
        // Store Config data to nvc from app.config
        public NameValueCollection nvc;
        // Create Home window
        public Form_Home myHome;
        // Create Home Overlay window
        public Form frmHomeOverlay;
        public Form frmOverlay;

        // error handling enum variable
        private ErrorAction myErrorAction;

        private SeparatorGateway mySeparatorGateway;
        private CultureInfo myCultureInfo;
        private Thread mySeparatorConnectionThread;
        public  ISeparator mySeparator = null;
        public  SeparatorEventSink myEventSink = null;

        // about window
        public string m_sInsVersion = "";
        public string m_sInsSerial = "";
        public string m_sInsAddress = "";
        public string m_sGatewayURL = "";
        public string m_sGatewayVer = "";
        public string m_sSvrUptime = "";   
        // run progress window
        private string myEstimatedCompletionTime;
        private string pauseCommandCaption = "";
        private string RunStatus = "";
        private List<ProgressUpdate> Progress;
        public struct ProgressUpdate
        {
            public int ID, Step, Quad;
            public string Command;

            public ProgressUpdate(int id, int step, int quad, string cmd)
            {
                this.ID = id;
                this.Step = step;
                this.Quad = quad;
                this.Command = cmd;
            }
        }

        private List<IShutdownProtocol> lstShutdownProtocols;
        private List<IMaintenanceProtocol> lstMaintenanceProtocols;

        private enum ErrorAction
        {
            HaltRun,
            Shutdown
        };

        // Protocol Loading
        public bool bLoadingProtocols;
        public string strLoatingProtocol;
        public int ProtocolsLoaded;

        //
        // Create Singleton RoboSep User Control
        //
        private static RoboSep_UserConsole myUserConsole;

        private RoboSep_UserConsole()
        {
            mySeparatorGateway = SeparatorGateway.GetInstance();
            myCultureInfo  = new CultureInfo("en-US", true);
            
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            ctrlCurrentUserControl = basePannel_Empty;
            // grab config data
            //nvc = (NameValueCollection) ConfigurationManager.GetSection("OperatorConsole/ConsoleConfiguration");

            // LOG
            string logMSG = "Initializing User Console";
            GUI_Controls.uiLog.LOG(this, "RoboSep_UserConsole", GUI_Controls.uiLog.LogLevel.GENERAL, logMSG);

            // create new Home window
            myHome = Form_Home.getInstance();
            // create new Home Overlay window
            frmHomeOverlay = createOverlay(0.8, 30, 10, 100);
            frmOverlay = createOverlay(0.6, 40, 10, 40);

            Thread.CurrentThread.CurrentCulture = myCultureInfo;	// Set culture for formatting
            Thread.CurrentThread.CurrentUICulture = myCultureInfo;	// Set culture for UI resources

            m_Timer.Interval = 500;
            m_Timer.Enabled = false;

            grabSystemOptions();
        }

        public static RoboSep_UserConsole getInstance()
        {
            if (myUserConsole == null)
            {
                myUserConsole = new RoboSep_UserConsole();
            }
            return myUserConsole;
        }

        private void GUIShutdown()
        {
            // LOG
            string logMSG = "System Shutdown";
            GUI_Controls.uiLog.LOG(this, "GUIShutdown", GUI_Controls.uiLog.LogLevel.GENERAL, logMSG);

            try
            {
                // Disallow further operator input
                this.Enabled = false;

                // Rather than unsubscribe from event notifications, since we're shutting
                // down the whole application anyway, just stop the Separator layer before/
                // at the same time as the UI.
                mySeparatorGateway.Disconnect(true);
            }
            finally
            {
                // Shutdown all message loops and exit
                Application.Exit();
            }
        }

        private void grabSystemOptions()
        {          
            try
            {
                NameValueCollection nvc = fromINI.getSection("GUI", "SYSTEM");
                bLidSensorEnable = nvc.Get("EnableLidSensor") == "true" ? true : false;
                bLiquidSensorEnable = nvc.Get("EnableLiquidLevelSensor") == "true" ? true : false;
            }
            catch
            {
                bLiquidSensorEnable = true;
                bLidSensorEnable = true;
            }

            // LOG
            string logMSG = "Get Sys Options: Lid Sensor = " + bLidSensorEnable.ToString() + "Liquid Sensor = " + bLiquidSensorEnable.ToString() ;
            GUI_Controls.uiLog.LOG(this, "grabSystemOptions", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        #region System Delegate Functions

        // Grab info for About page       
        private void AtReportInstrumentInformation(string gatewayURL,
                    string gatewayInterfaceVersion, string serverUptime_seconds,
                    string instrumentControlVersion, string instrumentSerialNumber,
                    string serviceConnection)
        {
            m_sInsVersion   = instrumentControlVersion;
            m_sInsSerial    = instrumentSerialNumber;
            m_sInsAddress   = serviceConnection;
            m_sGatewayVer   = gatewayInterfaceVersion;
            m_sGatewayURL   = gatewayURL;
            m_sSvrUptime    = serverUptime_seconds;

            // LOG
            string logMSG = "Get system information for About menu";
            GUI_Controls.uiLog.LOG(this, "AtReportInstrumentInformation", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        private void AtShutdownProtocolListUpdate(IShutdownProtocol[] shutdownProtocols)
        {
            lstShutdownProtocols = new List<IShutdownProtocol>();
            try
            {
                int shutdownProtocolCount = shutdownProtocols.GetLength(0);
                if (shutdownProtocols == null || shutdownProtocolCount <= 0)
                {
                    // Instrument must always be configured with at least one shutdown 
                    // protocol.
                    throw new ApplicationException(
                        SeparatorResourceManager.GetSeparatorString(
                        StringId.ExMissingMandatoryProtocol));
                }
                else
                {
                    for (int i = 0; i < shutdownProtocolCount; ++i)
                    {
                        lstShutdownProtocols.Add(shutdownProtocols[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);
            }
        }

        public void InvokeShutdownAction()
        {
            IShutdownProtocol SDProtocol = lstShutdownProtocols[0];
            if (SDProtocol != null)
            {
                // run splash pace as system shuts down.
                RoboSep_Splash.shutDown();

                mySeparator.Shutdown(SDProtocol);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "Invoked " + SDProtocol.Label);
            }

            // LOG
            string logMSG = "Invoking Shutdown Action";
            GUI_Controls.uiLog.LOG(this, "InvokeShutdownAction", GUI_Controls.uiLog.LogLevel.GENERAL, logMSG);
        }

        private void AtMaintenanceProtocolListUpdate(IMaintenanceProtocol[] maintenanceProtocols)
        {
            lstMaintenanceProtocols = new List<IMaintenanceProtocol>();
            try
            {
                // Assume required maintenance protocols are not defined, then enable their
                // use if they are actually present in the configured list of maintenance
                // protocols.

                int maintenanceProtocolCount = maintenanceProtocols.GetLength(0);
                if (maintenanceProtocols != null && maintenanceProtocolCount > 0)
                {
                    // Check that all required Maintenance protocols are supplied

                    // Add supplied Shutdown protocols to the display
                    for (int i = 0; i < maintenanceProtocolCount; ++i)
                    {
                        lstMaintenanceProtocols.Add(maintenanceProtocols[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);
            }
        }

        public void InvokeMaintenanceAction(int MaintenanceIndex)
        {
            IMaintenanceProtocol MaintenanceProtocol = lstMaintenanceProtocols[MaintenanceIndex];
            if (MaintenanceProtocol != null)
            {
                RoboSep_RunSamples.getInstance().addMaintenance(MaintenanceProtocol);

                // LOG
                string msg = MaintenanceIndex == 0 ? "Prime" : "Home";
                string logMSG = "Invoking maintenance action " + msg;
                GUI_Controls.uiLog.LOG(this, "InvokeMaintenanceAction", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            }
            else
            {
                // LOG
                string logMSG = "Unable to invoke Maintenance action";
                GUI_Controls.uiLog.LOG(this, "InvokeMaintenanceAction", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            }
        }

        private void AtBatchDurationUpdate(TimeSpan estimatedDuration,
            int endTimeSpan_minutes, int endTimeSpanWarningThreshold_minutes)
        {
            //myEstimatedCompletionTime = estimatedDuration;
            string temp = string.Empty;
            if (estimatedDuration.Hours != 0)
                temp += estimatedDuration.Hours.ToString() + ":";
            int tmpMins = estimatedDuration.Minutes + 1;
            temp += tmpMins.ToString("00") + ":00";
            myEstimatedCompletionTime = temp;

            // LOG
            string logMSG = "Estimated run completion = " + myEstimatedCompletionTime;
            GUI_Controls.uiLog.LOG(this, "AtBatchDurationUpdate", GUI_Controls.uiLog.LogLevel.INFO, logMSG);
        }

        public string EstimatedCompletion
        {
            get
            {
                return myEstimatedCompletionTime;
            }
        }

        

        public void AtReportStatus(string statusCode, string[] statusMessageValues)
        {
            System.Diagnostics.Debug.WriteLine("------------** AtReportStatus in SampleProcessing " + statusCode);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "------------** AtReportStatus in SampleProcessing "+statusCode);
            try
            {
                if (statusCode == "TSC3003")
                {
                    pauseCommandCaption = statusMessageValues[0];
                    if (statusMessageValues[0] == "Pause Command")
                        pauseCommandCaption = statusMessageValues[1];
                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, String.Format("------------------pauseCommandCaption {0}", pauseCommandCaption));

                }
                else if (statusCode == "TSC1002")
                {
                    if (statusMessageValues[0].StartsWith("Loading", StringComparison.CurrentCultureIgnoreCase))
                    {
                        bLoadingProtocols = true;
                        //strLoadingProtocol = statusMessageValues[0];
                        ProtocolsLoaded++;
                    }
                    else
                    {
                        bLoadingProtocols = false;
                        ProtocolsLoaded = 0;
                    }

                }
                else if (statusCode == "TSC1001")
                {
                    string stateValue, stateDescription;
                    string command = statusMessageValues[0];
                    System.Diagnostics.Debug.WriteLine("==" + command + "==");
                    SeparatorResourceManager.GetSeparatorStateStrings(typeof(SeparatorState),
                        SeparatorState.Running, out stateValue, out stateDescription);

                    // set run status to command string
                    if (command == "TopUpVial")
                    {
                        command = "TopUp Vial";
                    }
                    else if (command == "ResuspendVial")
                    {
                        command = "Resuspend";
                    }
                    else if (command == "HomeAll")
                    {
                        command = "Home All";
                    }

                    RunStatus = command;


                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "<<<<<<<<<<<<<<<<<<< AtReportStatus:  " + command);
                    
                    int id, seq, sec;

                    id = mySeparator.GetCurrentID();
                    seq = mySeparator.GetCurrentSeq();
                    sec = mySeparator.GetCurrentSec();

                    string str = String.Format("CWJ--> ** AtReportStatus ID: {0}, Seq: {1}, Quad: {2}", id, seq, sec);
                    System.Diagnostics.Debug.WriteLine(str);

                    // check if values are update, or unchanged from previous update                    
                    Progress.Add(new ProgressUpdate(id, seq, sec, command));
                }
            }
            catch (Exception ex)
            {
                LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);
            }

        }

        
        public void updateRunProgress(int step, out int id, out int seq, out int sec, out string cmd)
        {
            id = mySeparator.GetCurrentID();
            seq = mySeparator.GetCurrentSeq() - 1;
            sec = mySeparator.GetCurrentSec() - 1;
            cmd = Progress[step].Command;
        }

        public bool checkProgressUpdate(int currentStep)
        {
            // check if there is any new progress states
            // since last check
            bool isUpdate = false;

            if (currentStep == (Progress.Count - 1))
            {
                isUpdate = false;
            }
            // new step is available
            else
            {
                isUpdate = true;
            }

            return isUpdate;
        }

        // clears tracked progress (for when run is aborted or completed)
        public void ClearProgress()
        {
            while (Progress.Count != 1)
            {
                Progress.RemoveAt(1);
            }
        }

        private void AttemptSeparatorConnection()
        {
            // LOG
            string logMSG = "Connecting...";
            GUI_Controls.uiLog.LOG(this, "AttemptSeparatorConnection", GUI_Controls.uiLog.LogLevel.INFO, logMSG);

            try
            {
                // Initialise the Separator connection
                mySeparatorGateway.InitialiseConnection(myCultureInfo);

                // Actually connect the UI to the Separator server
                mySeparatorGateway.Connect(true);

                mySeparator = SeparatorGateway.GetInstance().ControlApi;
                myEventSink = SeparatorGateway.GetInstance().EventsApi;

                //---- Register All Delegrates here!  
                if (myEventSink != null)
                {
                    myEventSink.ReportInstrumentInformation += new Tesla.Separator.ReportInstrumentInfoDelegate(AtReportInstrumentInformation);
                    myEventSink.UpdateShutdownProtocolList += new ShutdownProtocolListDelegate(AtShutdownProtocolListUpdate);
                    myEventSink.UpdateMaintenanceProtocolList += new MaintenanceProtocolListDelegate(AtMaintenanceProtocolListUpdate);
                    mySeparatorGateway.EventsApi.UpdateBatchDuration += new BatchDurationDelegate(AtBatchDurationUpdate);
                    //myEventSink.UpdateSeparatorState -= new SeparatorStatusDelegate(AtSeparatorStateUpdate);
                    mySeparatorGateway.EventsApi.ReportStatus += new ReportStatusDelegate(AtReportStatus);
                    mySeparatorGateway.EventsApi.ReportError += new ReportErrorDelegate(AtReportError);
                }
            }
            catch (ApplicationException /*aex*/)
            {
                // Allow application exceptions to propagate upwards.  That is, if the 
                // InitialiseConnection times out, we use the generated ApplicationException
                // to trigger application shutdown.
            }
            catch (Exception ex)
            {
                LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);
            }
        }

        private void AtReportError(int severityLevel, string errorCode,
            string[] errorMessageValues)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("------------------AtReportError in SampleProcessing"));
            try
            {
                if (this.InvokeRequired)
                {
                    ReportErrorDelegate eh = new ReportErrorDelegate(this.AtReportError);
                    this.Invoke(eh, new object[] { severityLevel, errorCode, errorMessageValues });
                }
                else
                {
                    bool isRunning = (mySeparatorGateway.SeparatorState == SeparatorState.Running);
                    bool isFatalError = (severityLevel > 1);
                    bool haltRequired = false;
                    bool exitRequired = false;

                    // Use the errorCode to lookup a localised string, and use
                    // errorMessageValues to populate its run-time parameters (if required)
                    string errorMessage = SeparatorResourceManager.GetErrorMessageString(errorCode);
                    errorMessage = string.Format(errorMessage, errorMessageValues);
                    string errorText = string.Empty;

                    if (isFatalError)
                    {
                        errorText = SeparatorResourceManager.GetSeparatorString(StringId.ErrorFatal);
                        // Display the error dialog
                        // Halt the run and exit the application
                        errorText += SeparatorResourceManager.GetSeparatorString(StringId.ErrorTerminate);
                        exitRequired = true;
                        if (isRunning)
                        {
                            haltRequired = true;
                        }
                        // Display the error dialog
                        // Exit the application
                        errorText += " " +
                            SeparatorResourceManager.GetSeparatorString(StringId.ErrorReportError);
                        errorMessage = errorText + "\r\n\r\n" + errorMessage;
                        mySeparatorGateway.AddRunLogMessage(System.Diagnostics.TraceLevel.Error, StatusCode.TSC3006,
                            new string[] { errorMessage }, false);
                        LogFile.AddMessage(System.Diagnostics.TraceLevel.Error, errorMessage);
                        /*
                        messageDialog.ShowErrorDialog(
                            (System.Windows.Forms.Form)this.TopLevelControl,
                            errorMessage,
                            SeparatorResourceManager.GetSeparatorString(StringId.Error),
                            new string[] {
											 SeparatorResourceManager.GetSeparatorString(StringId.OK)
										 },
                            System.Drawing.SystemIcons.Error
                            );
                         */
                        RoboMessagePanel errorPrompt = new RoboMessagePanel(this, errorMessage,
                            SeparatorResourceManager.GetSeparatorString(StringId.Error), "Ok");
                        errorPrompt.ShowDialog();

                    }
                    else	// Non-Fatal error	
                    {
                        if (isRunning)
                        {
                            // Give the user the option of ending the run or continuing
                            errorText += SeparatorResourceManager.GetSeparatorString(StringId.ErrorTerminateQuestion);
                            errorMessage = errorText + "\r\n\r\n" + errorMessage;

                            mySeparatorGateway.AddRunLogMessage(System.Diagnostics.TraceLevel.Error, StatusCode.TSC3006,
                                new string[] { errorMessage }, false);
                            LogFile.AddMessage(System.Diagnostics.TraceLevel.Error, errorMessage);
                            RoboMessagePanel errorPrompt = new RoboMessagePanel( this, errorMessage,
                                SeparatorResourceManager.GetSeparatorString(StringId.Error), 
                                SeparatorResourceManager.GetSeparatorString(StringId.Yes),
								SeparatorResourceManager.GetSeparatorString(StringId.No));
                            errorPrompt.ShowDialog();
                            DialogResult dlgResult = errorPrompt.DialogResult;
                            
                            /*
                            messageDialog.ShowErrorDialog(
                                (System.Windows.Forms.Form)this.TopLevelControl,
                                errorMessage,
                                SeparatorResourceManager.GetSeparatorString(StringId.Error),
                                new string[] {
												 SeparatorResourceManager.GetSeparatorString(StringId.Yes),
												 SeparatorResourceManager.GetSeparatorString(StringId.No)
											 },
                                System.Drawing.SystemIcons.Error
                                );
                             */
                            // Check the dialog result
                            haltRequired = (dlgResult == DialogResult.OK);
                        }
                        else
                        {
                            System.Diagnostics.TraceLevel reportAsLevel = System.Diagnostics.TraceLevel.Error;
                            // Special case for scheduling errors
                            if (errorCode == "TEC1222")
                            {
                                errorText = SeparatorResourceManager.GetSeparatorString(StringId.ErrorCantSchedule);
                                // Report the scheduling error (but treat it as a non-fatal error -- that is, report the error but
                                // use TraceLevel.Warning in the log file).
                                reportAsLevel = System.Diagnostics.TraceLevel.Warning;
                                errorMessage = errorText;
                            }
                            else
                            {
                                errorMessage = errorText + "\r\n\r\n" + errorMessage;
                            }

                            mySeparatorGateway.AddRunLogMessage(reportAsLevel, StatusCode.TSC3006,
                                new string[] { errorMessage }, false);
                            LogFile.AddMessage(reportAsLevel, errorMessage);
                            // Display the error dialog
                            RoboMessagePanel errorPrompt = new RoboMessagePanel(this, errorMessage,
                                SeparatorResourceManager.GetSeparatorString(StringId.Error),
                                SeparatorResourceManager.GetSeparatorString(StringId.OK));
                            /*
                            messageDialog.ShowErrorDialog(
                                (System.Windows.Forms.Form)this.TopLevelControl,
                                errorMessage,
                                SeparatorResourceManager.GetSeparatorString(StringId.Error),
                                new string[] {
												 SeparatorResourceManager.GetSeparatorString(StringId.OK)
											 },
                                System.Drawing.SystemIcons.Error
                                );
                           */
                        }
                    }
                    // We're done with the message dialog, so release its resources

                    // Determine what action is required
                    if (haltRequired)
                    {
                        myErrorAction = ErrorAction.HaltRun;
                        errorTimer.Start();
                    }
                    //Exit?
                    if (exitRequired)
                    {
                        // Close the GUI (triggers an orderly shutdown of the application & 
                        // instrument).  Note we must check first if the reference is null
                        // in case the instrument did not initialise in time (in which case
                        // the GUI won't have been created).
                        Form mdiParent = (System.Windows.Forms.Form)this.TopLevelControl;
                        if (mdiParent != null)
                        {
                            mdiParent.Close();
                        }
                    }
                } //invoke else
            } //try
            catch (Exception ex)
            {
                LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);
            }
        }

        #endregion

        private void RoboSep_UserConsole_Load(object sender, EventArgs e)
        {
            // LOG
            string logMSG = "Loading User Console";
            GUI_Controls.uiLog.LOG(this, "RoboSep_UserConsole_Load", GUI_Controls.uiLog.LogLevel.INFO, logMSG);

            this.Size = new Size(640, 480);
            Progress = new List<ProgressUpdate>();
            Progress.Add(new ProgressUpdate(0,0,0, "Initializing"));
            StartSplash();

            mySeparatorConnectionThread = new Thread(new ThreadStart(this.AttemptSeparatorConnection));
            mySeparatorConnectionThread.IsBackground = true;
            mySeparatorConnectionThread.Start();
            m_Timer.Enabled = true;

            // LOG
            logMSG = "Starting GUI Thread";
            GUI_Controls.uiLog.LOG(this, "RoboSep_UserConsole_Load", GUI_Controls.uiLog.LogLevel.GENERAL, logMSG);

            RoboSep_UserDB.getInstance().XML_SaveUserConfig();
            strCurrentUser = RoboSep_UserDB.XML_UserProfileDBToName();
        }

        private Form createOverlay(double opacity, int R, int G, int B)
        {
            // create form to overlay
            Form Overlay = new Form();
            //Point OverlayOffset = new Point(4, 26);
            Point OverlayOffset = new Point(0, 0);
            Overlay.Size = new Size(640, 480);
            Overlay.BackColor = Color.Black;
            Overlay.Opacity = opacity;
            Overlay.StartPosition = FormStartPosition.CenterScreen;
            //Point winStartPnt = new Point(this.Location.X + OverlayOffset.X, this.Location.Y + OverlayOffset.Y);
            //Overlay.Location = winStartPnt;
            Overlay.FormBorderStyle = FormBorderStyle.None;
            Overlay.Enabled = false;
            Overlay.ShowInTaskbar = false;

            Overlay.TopMost = false;

            // create rectangle for overlay screen
            Microsoft.VisualBasic.PowerPacks.ShapeContainer canvas = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            canvas.Parent = Overlay;
            Microsoft.VisualBasic.PowerPacks.RectangleShape rec = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            rec.Size = new Size(640, 480);
            rec.FillColor = Color.FromArgb(R, G, B);
            rec.BackColor = Color.Blue;
            rec.FillStyle = Microsoft.VisualBasic.PowerPacks.FillStyle.Percent25;
            rec.Location = new Point(0, 0);
            rec.Parent = canvas;
            return Overlay;
        }

        #region Functions to enable form tracking (makes all forms move together to look as one)

        // allows easy addition of forms to open forms list
        public void addForm(Form FormA, Point offset)
        {
            frmList_AllForms.Add(new FormTracker(FormA, offset));
        }

        // removes all forms that no longer exist
        public void removeForms()
        {
            // look through list "all forms"
            for (int i = (frmList_AllForms.Count -1); i >= 0 ; i--)
            {
                bool exists = false;
                // compare all forms to openforms
                for (int j = 0; j < Application.OpenForms.Count; j++)
                {
                    if (frmList_AllForms[i].getForm().Equals(Application.OpenForms[j]))
                    {
                        exists = true;
                    }
                }
                // if form from frmList_AllForms is not in OpenForms then remove it
                if (exists == false)
                {
                    frmList_AllForms.RemoveAt(i);
                }
            }
        }

        private void RoboSep_UserConsole_ControlAdded(object sender, ControlEventArgs e)
        {
            removeForms();
        }

        private void RoboSep_UserConsole_LocationChanged(object sender, EventArgs e)
        {
            removeForms();

            // set home screen location
            //Point fullScreenOffset = new Point(4, 26);
            Point fullScreenOffset = new Point(0, 0);
            myHome.Location = new Point(this.Location.X + fullScreenOffset.X, this.Location.Y + fullScreenOffset.Y);
            frmHomeOverlay.Location = myHome.Location;
            frmOverlay.Location = myHome.Location;
            
            // move other forms as well
            for (int i = 0; i < frmList_AllForms.Count; i++)
            {
                Form frm = frmList_AllForms[i].getForm();
                Point pnt = frmList_AllForms[i].getOffset();
                frm.Location = new Point(this.Location.X + pnt.X, this.Location.Y + pnt.Y);
            }
        }

        #endregion



        private void m_Timer_Tick(object sender, EventArgs e)
        {
            if (mySeparatorGateway.IsInstrumentInitialised == true)
            {
                m_Timer.Enabled = false;
                StopSplash();                
            }
            else
            {
                ProgressSplash();
            }
        }

        public void StartSplash()
        {
            RoboSep_Splash.getInstance().Opacity = 0;
            RoboSep_Splash.getInstance().Show();
        }

        public void ProgressSplash()
        {
        }
        public void StopSplash()
        {
            // LOG
            string logMSG = "Stopping Splash page";
            GUI_Controls.uiLog.LOG(this, "ProgressSplash", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

            basePannel_Empty.Enabled = true;

            // Pass info to About window
            string[] data = new string[6];

            data[0] = m_sInsVersion;
            data[1] = m_sInsSerial;
            data[2] = m_sInsAddress;
            data[3] = m_sGatewayVer;
            data[4] = m_sGatewayURL;
            data[5] = m_sSvrUptime;

            RoboSep_About.getInstance().UpdateLabelInfo(data);

            // change to robosep runsamples window
            this.Controls.Add(RoboSep_RunSamples.getInstance());
            ctrlCurrentUserControl.Visible = false;
            this.Controls.Remove(ctrlCurrentUserControl);
            ctrlCurrentUserControl = RoboSep_RunSamples.getInstance();


            // close Splash Screen
            RoboSep_Splash.getInstance().close();
            
        }
        

        private void button_Refresh_Click(object sender, EventArgs e)
        {
            /*ISeparationProtocol[] SelectedProtocols = mySeparatorGateway.getSeparationProtocols();
            textBox2.Text = strCurrentUser;
            listView1.Items.Clear();
            foreach (ISeparationProtocol p in SelectedProtocols)
            {
                listView1.Items.Add(p.Label);
                listView1.Items[listView1.Items.Count - 1].SubItems.Add(p.Id.ToString());
                listView1.Items[listView1.Items.Count - 1].SubItems.Add(p.QuadrantCount.ToString());
            }
            */

            // get consumables list....
            string name;
            ProtocolClass type;
            int initQuad;
            ProtocolConsumable[,] consumables;
            int chosenProtocolIndex = 0;
            int numberOfQuadrants = 0;
            string[] customNames;

            consumables = mySeparatorGateway.GetProtocolConsumables(chosenProtocolIndex, out name, out type, out initQuad);
            mySeparatorGateway.GetCustomNames(name, out customNames, out numberOfQuadrants);

            textBox2.Text = name;
            textBox3.Text = numberOfQuadrants.ToString();

            listBox1.Items.Clear();
            for (int i = 0; i < customNames.Length; i++)
            { listBox1.Items.Add(customNames[i]); }
        }

        #region separator Gateway link functions

        public void scheduleRun(QuadrantId Q)
        {
            mySeparator.ScheduleRun(Q);
        }

        public void SetSampleVolume(QuadrantId quadrantId, FluidVolume mySamplesVolume)
        {
            mySeparatorGateway.SetSampleVolume(quadrantId, mySamplesVolume);

            // LOG
            string logMSG = "Quad: " + ((int)quadrantId +1).ToString() + " Sample Vol " + (mySamplesVolume.Amount/1000).ToString();
            GUI_Controls.uiLog.LOG(this, "SetSampleVolume", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        public ISeparationProtocol[] XML_getUserProtocols()
        {
            return mySeparatorGateway.getSeparationProtocols();
        }
        #endregion

        private void errorTimer_Tick(object sender, EventArgs e)
        {
            errorTimer.Stop();
            // run events requiruired by error from server
            if (myErrorAction == ErrorAction.HaltRun)
            {
                RoboSep_RunProgress.getInstance().Abbort();
            }
            else if (myErrorAction == ErrorAction.Shutdown)
            {
                InvokeShutdownAction(); // shutdown protocol
            }

        }
        private void RoboSep_UserConsole_FormClosing(object sender, FormClosingEventArgs e)
        {
            GUIShutdown();            
        }

    }

    #region FormTracker class - used to track open forms and their positions relative to main console window

    // Class for tracking open forms and their standard offsets
    // for when someone moves the main screen.
    public class FormTracker
    {
        private Form trackedForm;
        private Point formOffsets;

        public FormTracker(Form someForm, Point somePoint)
        {
            trackedForm = someForm;
            formOffsets = somePoint;
        }

        public bool isForm(Form comparedForm)
        {
            if (trackedForm == comparedForm)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Point getOffset()
        {
            return formOffsets;
        }

        public Form getForm()
        {
            return trackedForm;
        }
    }
}

    #endregion
