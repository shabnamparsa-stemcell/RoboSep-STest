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
using System.IO;
using System.Diagnostics;
using System.Management;


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
using Report_Crystal;

namespace GUI_Console
{
    public partial class RoboSep_UserConsole : Form
    {
        //HARDCODE
        public bool HARDCODE_OS_BIT = false;

        // Define Global Variables
        // Current Control
        public static UserControl ctrlCurrentUserControl;
        // Track open Forms
        private List<FormTracker> frmList_AllForms = new List<FormTracker>();

        // Tab Memory
        public enum HelpTab { About, HelpVid };
        public static HelpTab CurrentHelpTab = HelpTab.About;
        public static int intCurrentHelpTab = 0;  // indicates what tab # was most recently active
        public static int intCurrentLogTab = 0;   // indicates what tab # was most recently active

        //Options
        public static bool KeyboardEnabled = true;// indicates whether touch keyboard is enabled
        public static bool bStartTimerEbnable = false; // indicates wheter the Start Timer is active
        // User Name
        public static string strCurrentUser = "User Name";     // remembers most recently logged in user
        public static bool bIsRunning;          // Boolean set to TRUE if RoboSep is currently running protocols
        public static bool bIsBatchHaltDone = true;  //CWJ BatchHalt improve
        private static bool initialized = false;


        private const string networkORCAConfigFileName = "NetworkORCA.ini";
        private const string EndOfRunReportFileName = "EndOfRunReport.rpt";
        private const string DateFormat = @"d/M/yyyy";

        private const int myPauseBeepInterval = 30000;
        private const int myPauseBeepTotal = 60;  // 30min/30sec = 60 beeps
        private const int myRunFinishBeepInterval = 5000;
        private const int myRunFinishBeepTotal = 60; // 5min/5sec = 60 beeps
        private const int myFailureBeepInterval = 2000;
        private const int myFailureBeepTotal = 300;  // 10min/2sec = 300 beeps

        private int myBeepTimerCount = 0;
        private int myBeepTimerTotal = 0;
         
        
        // Store Config data to nvc from app.config
        public NameValueCollection nvc;
        // Create Home window
        public RoboSep_Home myHome;
        // Create Home Overlay window
        public Form frmHomeOverlay;
        public Form frmOverlay;

        private IniFile GUIini;
        public IniFile LanguageINI;
        public bool WarrantyExpired;
        public bool WarrantyNoticeRequired = false;

        // Pause handling vars
        private PauseResumeDialog myPauseResumeDialog;
        private FileSystemWatcher myProtocolFilesWatcher;


        private volatile bool inProtocolFileWatcherHandler = false;
      

        private bool isRemoteDestopLocked = false;

        // error handling enum variable
        private ErrorAction myErrorAction;

        private SeparatorGateway mySeparatorGateway;
        private CultureInfo myCultureInfo;
        private Thread mySeparatorConnectionThread;
        private  ISeparator mySeparator = null;
        public  SeparatorEventSink myEventSink = null;

        public RoboSep_Resources myResourcesWindow = null;

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

        public event SeparatorIsReadyDelegate NotifyUserSeparatorIsReadyToAcceptProtocol = null;
        public event EventHandler NotifyUserSeparationProtocolsUpdated = null;
        public event ChosenProtocolsFinishedDelegate NotifyUserChosenProtocolsUpdated = null;

        private int networkORCAExecutableProcessID = -1;
        private EndOfRunReportHelper reportHelper = null;

        private double InitialOpacity;

        //
        // Create Singleton RoboSep User Control
        //
        private static RoboSep_UserConsole myUserConsole;

        private RoboSep_UserConsole()
        {
            CheckForIllegalCrossThreadCalls = false;
            mySeparatorGateway = SeparatorGateway.GetInstance();
            myCultureInfo  = new CultureInfo("en-US", true);


            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            ctrlCurrentUserControl = basePannel_Empty;

            // Do not show in the task bar
            this.ShowInTaskbar = false;

            // grab config data
            //nvc = (NameValueCollection) ConfigurationManager.GetSection("OperatorConsole/ConsoleConfiguration");

 
            //GUI_Controls.uiLog.LOG(this, "RoboSep_UserConsole", GUI_Controls.uiLog.LogLevel.GENERAL, logMSG);

            // LOG
            string logMSG = "Initializing User Console";
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
            // create new Home Overlay window
            frmHomeOverlay = createOverlay(0.8, 30, 10, 100);
            frmHomeOverlay.GotFocus += new System.EventHandler(frmHomOverlay_Focus);  // event sets top item back on top using "BringToFront"
            frmOverlay = createOverlay(0.6, 40, 10, 40);
            frmOverlay.GotFocus += new System.EventHandler(frmOverlay_Focus);         // event sets top item back on top using "BringToFront"

            Thread.CurrentThread.CurrentCulture = myCultureInfo;	// Set culture for formatting
            Thread.CurrentThread.CurrentUICulture = myCultureInfo;	// Set culture for UI resources

            m_Timer.Interval = 500;
            m_Timer.Enabled = false;

            // set RoboSep Language Resource
            GUIini = new IniFile(IniFile.iniFile.GUI);
            string language = GUIini.IniReadValue("General", "Language", "Default"); // set default value to Language
            switch (language)
            {
                case "Default":
                    LanguageINI = new IniFile(IniFile.iniFile.Language);
                    break;
            }

            bool bAutoComputeWarrantyStatus = true;
            string sAutoComputeWarrantyStatus = GUIini.IniReadValue("About", "AutoComputeWarrantyStatus", "true");
            if (!string.IsNullOrEmpty(sAutoComputeWarrantyStatus) && (sAutoComputeWarrantyStatus.ToLower() == "false" || sAutoComputeWarrantyStatus.ToLower() == "no"))
            {
                bAutoComputeWarrantyStatus = false;
            }
            // determine warranty status
            string sExpired = "Expired";
            if (LanguageINI != null)
                sExpired = LanguageINI.GetString("lblExpired");

            string WarrantyStatus = GUIini.IniReadValue("About", "WarrantyStatus", "Expired");
            if (WarrantyStatus.ToLower() == sExpired.ToLower())
                WarrantyExpired = true;
            else if (bAutoComputeWarrantyStatus)
            {
                //string valid = "Valid Through: ";
                string ExpireDate = GUIini.IniReadValue("About", "WarrantyExpireDate", "01/01/2014");
                if (!string.IsNullOrEmpty(ExpireDate))
                {
                    try
                    {
                        object obj = ConvertDateStringToDate(ExpireDate);
                        if (obj != null)
                        {
                            DateTime dateExpire = (DateTime)obj;
                            if (DateTime.Now.CompareTo(dateExpire) <= 0)
                            {
                                // Valid
                                // warranty expiry is in the future
                                DateTime nextMonth = DateTime.Today.AddMonths(1);
                                if (DateTime.Compare(dateExpire, nextMonth) < 0)
                                {
                                    // warranty will expire within a month
                                    WarrantyNoticeRequired = true;
                                }
                            }
                            else
                            {
                                // Expired
                                // inform user their warranty is expired
                                GUIini.IniWriteValue("About", "WarrantyStatus", "Expired");
                            }
                        }
                        else
                        {
                            string LOGmsg = String.Format("WarrantyExpireDate in GUI.ini file is invalid.");

                            LogFile.AddMessage(TraceLevel.Error,LOGmsg);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);               
                    }
                }
            }
            else
            {

            }
  
            // create new Home window
            myHome = RoboSep_Home.getInstance();

            // launch Network ORCA
            launchNetworkORCA();

            // hide taskbar
            Utilities.SendCtrlEscKeys();

            initialized = true;
        }

        public static RoboSep_UserConsole getInstance()
        {
            if (myUserConsole == null)
            {
                myUserConsole = new RoboSep_UserConsole();
            }
            return myUserConsole;
        }

        public static bool IsInitialized
        {
            get { return initialized; }
        }

        private void GUIShutdown()
        {
            // LOG
            string logMSG = "System Shutdown";
            //GUI_Controls.uiLog.LOG(this, "GUIShutdown", GUI_Controls.uiLog.LogLevel.GENERAL, logMSG);

            LogFile.AddMessage(TraceLevel.Info, logMSG);
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

        private object ConvertDateStringToDate(string sDate)
        {
            string errMsg;
            if (string.IsNullOrEmpty(sDate))
            {
                errMsg = String.Format("Failed to convert string to date. Input parameter is null");
                LogFile.AddMessage(TraceLevel.Error, errMsg);                
                return null;
            }

            DateTime dateValue;
            if (!DateTime.TryParseExact(sDate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
            {
                // error for parsing
                errMsg = String.Format("Failed to convert variable '{0}' to valid date.", sDate);
                LogFile.AddMessage(TraceLevel.Error, errMsg);                
                return null;
            }
            return dateValue;
        }

        private void watchProtocolFiles()
        {
            string protocolPath = Utilities.GetRoboSepSysPath();
            protocolPath += "protocols\\";
            myProtocolFilesWatcher = new FileSystemWatcher();
            myProtocolFilesWatcher.Path = protocolPath;
            myProtocolFilesWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
            myProtocolFilesWatcher.Filter = "*.xml";

            // Begin watching.
            myProtocolFilesWatcher.EnableRaisingEvents = true;

            // Add event handlers.
            myProtocolFilesWatcher.Changed += new FileSystemEventHandler(OnProtocolFilesChanged);
            myProtocolFilesWatcher.Deleted += new FileSystemEventHandler(OnProtocolFilesChanged);
            myProtocolFilesWatcher.Created += new FileSystemEventHandler(OnProtocolFilesChanged);
            myProtocolFilesWatcher.Renamed += new RenamedEventHandler(OnProtocolFilesChanged);
        }

        private static void OnProtocolFilesChanged(object source, FileSystemEventArgs e)
        {
            RoboSep_UserConsole pRoboSepUserConsole = RoboSep_UserConsole.getInstance();
            if (pRoboSepUserConsole == null)
                return;

            if (pRoboSepUserConsole.inProtocolFileWatcherHandler) 
                return;

            pRoboSepUserConsole.inProtocolFileWatcherHandler = true;
            pRoboSepUserConsole.myProtocolFilesWatcher.EnableRaisingEvents = false;

            string changeDescription = "";
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                    changeDescription = "created";
                    break;
                case WatcherChangeTypes.Deleted:
                    changeDescription = "deleted";
                    break;
                case WatcherChangeTypes.Renamed:
                    changeDescription = "renamed";
                    break;
                case WatcherChangeTypes.Changed:
                    changeDescription = "changed";
                    break;
                default:
                    changeDescription = "modified with unknown type";
                    break;
            }
            
            string logMSG = string.Format("Detect protocol file '{0}' has been {0}.", e.FullPath, changeDescription);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);            

            RoboSep_RunSamples pRoboSepRunSamples = RoboSep_RunSamples.getInstance();
            if (pRoboSepRunSamples == null)
                return;

            int[] iSelectedProtocols = pRoboSepRunSamples.iSelectedProtocols;

            if (iSelectedProtocols != null && iSelectedProtocols.Length > 0)
            {
                // display a message to indicate that a protocol has been modified
                // Sunny to do
                // Show message
                string temp = pRoboSepUserConsole.LanguageINI.GetString("msgModifyProtocol");
                string sMSG = string.Format(temp, e.Name, changeDescription);
                temp = pRoboSepUserConsole.LanguageINI.GetString("headerModifyProtocol");
                string sHeader = string.Format(temp, changeDescription);

                RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING,
                    sMSG, sHeader, pRoboSepUserConsole.LanguageINI.GetString("Ok"), pRoboSepUserConsole.LanguageINI.GetString("Cancel"));
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                RoboSep_UserConsole.hideOverlay();
                if (prompt.DialogResult != DialogResult.OK)
                {
                    pRoboSepUserConsole.inProtocolFileWatcherHandler = false;
                    pRoboSepUserConsole.myProtocolFilesWatcher.EnableRaisingEvents = true;
                    prompt.Dispose();
                    return;
                }
                else
                {
                    prompt.Dispose();
                }
            }

            for (QuadrantId Qid = QuadrantId.Quadrant1; Qid < QuadrantId.NUM_QUADRANTS; Qid++)
            {
                pRoboSepRunSamples.CancelQuadrant((int)Qid);
            }

            // show loading protocol message
            string msg = pRoboSepUserConsole.LanguageINI.GetString("LoadingAllProtocols");
            RoboMessagePanel loading = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_INFORMATION, msg, true, false);
            RoboSep_UserConsole.showOverlay();
            loading.Show();

            // Reload all protocols when a file is changed, created, or deleted.
            RoboSep_UserDB.getInstance().ReloadAllProtocols();

            RoboSep_UserDB.getInstance().CheckUserProtocolIntegrity();

            // close loading dialog
            loading.Close();
            loading = null;

            RoboSep_UserConsole.hideOverlay();
            pRoboSepUserConsole.inProtocolFileWatcherHandler = false;
            pRoboSepUserConsole.myProtocolFilesWatcher.EnableRaisingEvents = true;
        }

        // launch network ORCA
        private bool launchNetworkORCA()
        {
            // read nertwork ORCA config.
            string logMSG = "";
            string systemPath = Utilities.GetRoboSepSysPath();
            string networkORCAConfig = systemPath + "config\\";
            networkORCAConfig += networkORCAConfigFileName;
            if (!File.Exists(networkORCAConfig))
            {
                logMSG = "Network ORCA config. file is not existed. Network ORCA is not configured.";
                //  (logMSG);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
                return false;
            }

            // read config file
            // read values
            string[] lines = File.ReadAllLines(networkORCAConfig, System.Text.Encoding.UTF8);
            List<string> lst = new List<string>();
            lst.AddRange(lines);

            // Note: these variable names are from the NetworkORCA.ini
            string txtArgumemtFormats = "argumentFormats";
            string txtExecutable = "executable";
            string txtSwitch = "switch";
            string txtORCAName = "orcaName";
            string txtORCANumber = "orcaNumber";
            string txtType = "type";
            string txtPortNumber = "portNumber";

            string strArgumemtFormats = getConfigValue(ref lst, txtArgumemtFormats);
            string strExecutable = getConfigValue(ref lst, txtExecutable);
            string strSwitch = getConfigValue(ref lst, txtSwitch);
            string strORCAName = getConfigValue(ref lst, txtORCAName);
            //string strORCANumber = getConfigValue(ref lst, txtORCANumber);                       // move to GUI.INI
            string strType = getConfigValue(ref lst, txtType);
            string strPortNumber = getConfigValue(ref lst, txtPortNumber);
            string strORCANumber = GUIini.IniReadValue("NetworkORCA", txtORCANumber, "1"); // get ORCA number from GUI.INI
            if (string.IsNullOrEmpty(strArgumemtFormats))
            {
                logMSG = "Network ORCA config. file: variable 'arguments' is not configured.";
                //  (logMSG);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
                return false;
            }

            if (string.IsNullOrEmpty(strExecutable))
            {
                logMSG = "Network ORCA config. file: variable 'executable' is not configured.";
                //  (logMSG);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
                return false;
            }

            if (string.IsNullOrEmpty(strSwitch))
            {
                logMSG = "Network ORCA config. file: variable 'switch' is not configured.";
                //  (logMSG);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
                return false;
            }

            if (string.IsNullOrEmpty(strORCAName))
            {
                logMSG = "Network ORCA config. file: variable 'orcaName' is not configured.";
                //  (logMSG);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
                return false;
            }

            if (string.IsNullOrEmpty(strType))
            {
                logMSG = "Network ORCA config. file: variable 'type' is not configured.";
                //  (logMSG);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
                return false;
            } 
            
            if (string.IsNullOrEmpty(strPortNumber))
            {
                logMSG = "Network ORCA config. file: variable 'portNumber' is not configured.";
                //  (logMSG);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
                return false;
            }

            // ORCA Number could be null
            if (strORCANumber == null)
            {
                strORCANumber = "";
            }

            string strArgument = string.Format(strArgumemtFormats, strSwitch, strORCAName, strORCANumber, strType, strPortNumber);

            logMSG = string.Format("Command to launch service '{0} {1}' for Network ORCA.", strExecutable, strArgument);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
            // check if the service executable for network ORCA target directory exists
            string executablePath = string.Empty;
            executablePath = GetFullPath(strExecutable); //This will give the folder path which is having the file

            if (string.IsNullOrEmpty(executablePath))
            {
                logMSG = string.Format("Failed to find the path of the executable '{0}'. Fail to launch service for network ORCA.", strExecutable);
                //  (logMSG);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
                return false;
            }

            // kill the previous service if it exists
            KillExectuable(strExecutable);

            // launch Network ORCA
            ProcessStartInfo ServiceStartInfo = new ProcessStartInfo();
            ServiceStartInfo.WorkingDirectory = executablePath;
            ServiceStartInfo.FileName = strExecutable;
            ServiceStartInfo.Arguments = strArgument;
            ServiceStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Process ServiceProgram = new Process();
            ServiceProgram.StartInfo = ServiceStartInfo;
            ServiceProgram.Start();
            networkORCAExecutableProcessID = ServiceProgram.Id;
            logMSG = string.Format("Service '{0}' for network ORCA has been launched. ProcessID='{1}'. Path='{2}'", strExecutable, networkORCAExecutableProcessID, executablePath);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
            return true;
        }

        private  bool ExistsOnPath(string fileName)
        {
            if (GetFullPath(fileName) != null)
                return true;
            return false;
        }

        private string GetFullPath(string fileName)
        {
            if (File.Exists(fileName))
                return Path.GetFullPath(fileName);

            var values = Environment.GetEnvironmentVariable("PATH");
            foreach (var path in values.Split(';'))
            {
                var fullPath = Path.Combine(path, fileName);
                if (File.Exists(fullPath))
                    return path;
            }
            return null;
        }

        private string getConfigValue(ref List<string> lst, string configVariable)
        {
            if (lst == null || string.IsNullOrEmpty(configVariable))
                return null;

            int index = 0;
            string[] parts;
            string ConfigValue = "";
            index = lst.FindIndex(index, x => { return (!string.IsNullOrEmpty(x) && x.Length >= configVariable.Length && x.ToLower().Contains(configVariable.ToLower())); });
            do 
            {
                if (0 <= index)
                {
                    parts = lst[index].Split('=');
                    if (parts.Length > 1 && parts[0].Trim().Substring(0, 2) != @"//" && parts[0].Trim() == configVariable)
                    {
                        if (!string.IsNullOrEmpty(parts[1]) && parts[1].Trim().Length > 0)
                        {
                            ConfigValue = parts[1].Trim();
                            break;
                        }
                    }
                    index++;
                    index = lst.FindIndex(index, x => { return (!string.IsNullOrEmpty(x) && x.Length >= configVariable.Length && x.ToLower().Contains(configVariable.ToLower())); });
                }

              } while (0 <=  index);

            return ConfigValue;
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
            //GUI_Controls.uiLog.LOG(this, "AtReportInstrumentInformation", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
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
                mySeparator.CloseOrcaLogFileSystem();
                mySeparator.Shutdown(SDProtocol);
            }

            // LOG
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
                //GUI_Controls.uiLog.LOG(this, "InvokeMaintenanceAction", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
                //  (logMSG);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
            }
            else
            {
                // LOG
                string logMSG = "Unable to invoke Maintenance action";
                //GUI_Controls.uiLog.LOG(this, "InvokeMaintenanceAction", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
                //  (logMSG);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
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
            //GUI_Controls.uiLog.LOG(this, "AtBatchDurationUpdate", GUI_Controls.uiLog.LogLevel.INFO, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
        }

        private void AtGuiStateChangeUpdate()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    //DbgView("in AtGuiStateChangeUpdate() in SampProc.cs - invoke req'd"); // bdr
                    
                    GuiStateChangeDelegate eh = new GuiStateChangeDelegate(this.AtGuiStateChangeUpdate);
                    this.Invoke(eh, new object[] { });

                }
                else
                {
                    // Update the state of controls according to the system state
                    //DbgView("in AtGuiStateChangeUpdate() in SampProc.cs - update state of ctrls"); // bdr
                    SetGuiState(mySeparatorGateway.SeparatorState, mySeparatorGateway.AvailableQuadrantsCount);
                }
            }
            catch (Exception ex)
            {
                LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);
            }

        }

        /// Sets the state of the GUI controls based on the state of the instrument.
        /// </summary>
        /// 

        private void SetGuiState(SeparatorState systemState, int availableQuadrantCount)
        {
            // Get the text name of the current state
            string stateValue, stateDescription;
            SeparatorResourceManager.GetSeparatorStateStrings(typeof(SeparatorState),
                systemState, out stateValue, out stateDescription);

            // Update the state of controls according to the system state
            switch (systemState)
            {            
                case SeparatorState.Ready:
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("------------------------------ SetGuiState:  SeparatorState.Ready -------------------------"));

                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, String.Format("SetGuiState:  SeparatorState.Ready -------------------------"));
                    //btnRunStart.Enabled = true;
                    // Update the prompt to indicate the user can either run
                    // or, if there are remaining quadrants, select another protocol 

                    // Press...
                    string strRunProtocolPrompt =
                        SeparatorResourceManager.GetSeparatorString(
                        StringId.Press);
                    // ...Start...
                    strRunProtocolPrompt += " " +
                        SeparatorResourceManager.GetSeparatorString(
                        StringId.RunStartText);
                    if (availableQuadrantCount > 0)
                    {
                        // ...to begin, or "Select Protocol" (etc).
                        strRunProtocolPrompt += " " +
                            SeparatorResourceManager.GetSeparatorString(
                            StringId.RunMessagesRunOrSelectText) + ", " +
                            SeparatorResourceManager.GetSeparatorString(
                            StringId.Or) + " " +
                            SeparatorResourceManager.GetSeparatorString(
                            StringId.RunMessagesSelectProtocolText);
                     }
                    else
                    {
                        // ...to begin.
                        strRunProtocolPrompt += " " +
                            SeparatorResourceManager.GetSeparatorString(
                            StringId.RunMessagesRunOrSelectText) + ".";
                    }

                    NotifyUserSeparatorIsReady(availableQuadrantCount);

                    if (RoboSep_RunSamples.getInstance().disableRun && !RoboSep_RunSamples.getInstance().IsAnyQuadrantMarkedAsCancelled())
                        RoboSep_RunSamples.getInstance().disableRun = false;

                    break;
                }
                case SeparatorState.PauseCommand:
                case SeparatorState.Paused:
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("------------------------------ SetGuiState:  SeparatorState.Paused or SeparatorState.PauseCommand -------------------------"));

                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, String.Format("SetGuiState:  SeparatorState.Paused or SeparatorState.PauseCommand -------------------------"));
                    // Display the pause dialog (if not already shown).  That is,
                    // the Paused state may be initiated by the user or the instrument.
                    // In the latter case we do not display the pausing message - we move
                    // straight to the paused display.

                    // pause GUI elements on run progress window
                    RoboSep_RunProgress.getInstance().PauseGuiElements();

                    System.Diagnostics.Debug.WriteLine("in SetGuiState() case sepState.pause pressed....call dspPausResDlg(0)");

                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, String.Format("in SetGuiState() case sepState.pause pressed....call dspPausResDlg(0)"));
                    DialogResult result = DisplayPauseResumeDialog(false,
                        SeparatorState.PauseCommand == systemState);  // fully paused, is pause command??

                    // Update the messages prompt
                    //UpdateRunMessagesText(stateDescription);
                    //LogFile.AddMessage(TraceLevel.Info,"<<<<<<<<<<<<<<<SetGuiState: pause "+stateDescription);

                    bool TimersEnabled = RoboSep_RunProgress.getInstance().CWJ_Timer.Enabled &&
                        RoboSep_RunProgress.getInstance().updateTimer.Enabled;

                    /*
                    if (!RoboSep_RunProgress.getInstance().paused)
                    {                        
                        RoboSep_RunProgress.getInstance().PauseCompleted();
                    }
                    */
                    break;
                }
                case SeparatorState.Running:
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("------------------------------ SetGuiState:  SeparatorState.Running -------------------------"));

                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, String.Format("SetGuiState:  SeparatorState.Running -------------------------"));
                    if (myPauseResumeDialog != null && myPauseResumeDialog.myPauseStatus != PauseResumeDialog.PauseStatus.TipStrip)
                    {
                        RoboSep_RunProgress.getInstance().ResumeGuiElements();
                    }
                    break;
                }
                case SeparatorState.BatchHalted:
#if false
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("------------------------------ SetGuiState:  SeparatorState.BatchHalted -------------------------"));
                      (String.Format("------------------------------ SetGuiState:  SeparatorState.BatchHalted -------------------------"));

                    if (!RoboSep_RunProgress.getInstance().halted)
                    {
                        bool TimersEnabled = RoboSep_RunProgress.getInstance().CWJ_Timer.Enabled &&
                        RoboSep_RunProgress.getInstance().updateTimer.Enabled;

                        if (TimersEnabled)
                        {
                            RoboSep_RunProgress.getInstance().CWJ_Timer.Enabled = false;
                            RoboSep_RunProgress.getInstance().updateTimer.Enabled = false;
                        }

                        RoboSep_RunProgress.getInstance().HaltCompleted();

                        //Beep time
                        myBeepTimer.Interval = myRunFinishBeepInterval;
                        myBeepTimerCount = 0;
                        myBeepTimerTotal = myRunFinishBeepTotal;
                        myBeepTimer.Start();

                    }
                    break;
                }
#else
                    System.Diagnostics.Debug.WriteLine(String.Format("------------------------------ SetGuiState:  SeparatorState.BatchHalted -------------------------"));

                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, String.Format("SetGuiState:  SeparatorState.BatchHalted -------------------------")); 
                    RoboSep_RunProgress.getInstance().CWJ_Timer.Enabled = false;
                    RoboSep_RunProgress.getInstance().updateTimer.Enabled = false;

                    RoboSep_RunProgress.getInstance().HaltCompleted(); //CWJ BatchHalt improve

                    myBeepTimer.Interval = myRunFinishBeepInterval;
                    myBeepTimerCount = 0;
                    myBeepTimerTotal = myRunFinishBeepTotal;
                    myBeepTimer.Start();
                    break;
#endif
                case SeparatorState.BatchComplete:
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("------------------------------ SetGuiState:  SeparatorState.BatchComplete -------------------------"));
                    //  (String.Format("SetGuiState:  SeparatorState.BatchComplete -------------------------"));
                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, String.Format("SetGuiState:  SeparatorState.BatchComplete -------------------------")); 
                    //Beep time
                    myBeepTimer.Interval = myRunFinishBeepInterval;
                    myBeepTimerCount = 0;
                    myBeepTimerTotal = myRunFinishBeepTotal;
                    myBeepTimer.Start();
                    break;
                }
                case SeparatorState.SeparatorUnloaded:
#if false
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("------------------------------ SetGuiState:  SeparatorState.SeparatorUnloaded -------------------------"));
                      (String.Format("------------------------------ SetGuiState:  SeparatorState.SeparatorUnloaded -------------------------"));
                    
                    if (bIsRunning)
                    {
                        bool TimersEnabled = RoboSep_RunProgress.getInstance().CWJ_Timer.Enabled &&
                        RoboSep_RunProgress.getInstance().updateTimer.Enabled;

                        if (TimersEnabled)
                        {
                            RoboSep_RunProgress.getInstance().CWJ_Timer.Enabled = false;
                            RoboSep_RunProgress.getInstance().updateTimer.Enabled = false;
                        }
                        myBeepTimer.Stop();
                        RoboSep_RunProgress.getInstance().UnloadCompleted();
                    }
                    break;
                }
#else
                    System.Diagnostics.Debug.WriteLine(String.Format("------------------------------ SetGuiState:  SeparatorState.SeparatorUnloaded -------------------------"));

                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, String.Format("SetGuiState:  SeparatorState.SeparatorUnloaded -------------------------")); 
                    if (bIsRunning)
                    {
                        RoboSep_RunProgress.getInstance().CWJ_Timer.Enabled = false;
                        RoboSep_RunProgress.getInstance().updateTimer.Enabled = false;

                        myBeepTimer.Stop();
                        RoboSep_RunProgress.getInstance().UnloadCompleted();
                    }
                    break;

#endif
                case SeparatorState.Selected:
                {
                    //RoboSep_RunSamples.getInstance().disableRun = true;
                    System.Diagnostics.Debug.WriteLine(String.Format("------------------------------ SetGuiState:  SeparatorState.Selected -------------------------"));

                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, String.Format("SetGuiState:  SeparatorState.Selected -------------------------")); 
                    break;
                }
            }
        }

        private void AtSeparationProtocolTableUpdate(DataTable aSeparationProtocolData)
        {
            if (NotifyUserSeparationProtocolsUpdated != null)
            {
                //  NotifyUserProtocolsReloaded(this, new EventArgs());
                //  this.Invoke((MethodInvoker)delegate() { this.NotifyUserProtocolsReloaded(this, new EventArgs()); });
                this.BeginInvoke((MethodInvoker)delegate { this.NotifyUserSeparationProtocolsUpdated(this, new EventArgs()); });
            }
        }

        private void NotifyUserSeparatorIsReady(int availableQuadrantCount)
        {
            if (NotifyUserSeparatorIsReadyToAcceptProtocol != null)
            {
                this.BeginInvoke((MethodInvoker)delegate { this.NotifyUserSeparatorIsReadyToAcceptProtocol(this, new IntegerEventArgs(availableQuadrantCount)); });
            }
        }

		// calls myPauseResumeDlg - with isLidClosed set to true
		public DialogResult DisplayPauseResumeDialog(bool isPausing, bool isPauseCommand)
		{
            if (myPauseResumeDialog != null)
            {
                myPauseResumeDialog.PauseTimeout_ms = mySeparatorGateway.PauseInstrumentTimeout_ms;
                System.Diagnostics.Debug.WriteLine(String.Format("------------------------------ DisplayPauseResumeDialog not active {0}", myPauseResumeDialog != Form.ActiveForm));
            }
			DialogResult result = DialogResult.Abort;

			// if the Pause/Resume dialog is already active, do nothing, otherwise display
			// it in the appropriate mode (i.e. pausing or already paused).
            if (myPauseResumeDialog != null && myPauseResumeDialog != Form.ActiveForm)    
			{
				System.Diagnostics.Debug.WriteLine(String.Format("------------------------------ DisplayPauseResumeDialog pausing {0}",isPausing));

                myPauseResumeDialog.Dispose();
                myPauseResumeDialog = null;

                myPauseResumeDialog = new PauseResumeDialog(mySeparatorGateway.EventsApi);
                myPauseResumeDialog.PauseTimeout_ms = mySeparatorGateway.PauseInstrumentTimeout_ms;

				//- cant use here!!! isLidClosed = mySeparatorGateway.IsLidClosed(); // bdr new
				// RL - bdr is right can't communicate here if is pausing or right after pause... =(
				// RL - pause state is really fragile, don't do rpc call during pausing or pause after pause!!!
				bool isLidClose = true;

				//Beep time
				if ((pauseCommandCaption.Equals("Manual tip strip")) || 
					(pauseCommandCaption.Equals("Tip strip recovery")))
				{
					myBeepTimer.Interval = myFailureBeepInterval;
					myBeepTimerCount = 0;
					myBeepTimerTotal = myFailureBeepTotal;

                    // Show the run progress screen if it is not in the background
                    ShowRunProgressScreen();
				}
				else
				{
					myBeepTimer.Interval = myPauseBeepInterval;
					myBeepTimerCount = 0;
					myBeepTimerTotal = myPauseBeepTotal;
				}

				myBeepTimer.Start();

                result = myPauseResumeDialog.ShowDialog( /*
					(System.Windows.Forms.Form)this.TopLevelControl*/ this,
					isPausing, isLidClose, isPauseCommand, pauseCommandCaption);
                myPauseResumeDialog.Dispose();
				if ( !pauseCommandCaption.Equals("Manual tip strip"))
				{
					pauseCommandCaption = "";
				}
				myBeepTimer.Stop();

				// DialogResult.OK is used to mean 'Resume'
				if (result == DialogResult.OK) 	
				{

                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, String.Format("User presses the 'Resume' button.")); 
                    RoboSep_RunProgress.getInstance().disableButtons = true;

                    mySeparatorGateway.ControlApi.ResumeRun();
 				
				}
					// DialogResult.Cancel is used to mean 'Halt'
				else if (result == DialogResult.Cancel)  
				{

                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, String.Format("User presses the 'Stop' button.")); 
					if (ConfirmAbortRun() == true) 
					{

                        LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, String.Format("User confirms to stop the current run.")); 
						AbortRun();
					}
					else 
					{

                        LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, String.Format("User confirms to resunme the current run.")); 
                        RoboSep_RunProgress.getInstance().disableButtons = true;
						mySeparatorGateway.ControlApi.ResumeRun();
						
                        //UpdateRunMessagesText(SeparatorResourceManager.GetSeparatorString(
						//	StringId.RunResumingText));
					}
							
				}
				else if (result == DialogResult.Abort) 	
				{

                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, String.Format("No action required.")); 
					System.Diagnostics.Debug.WriteLine("DspPsRsDlg result = Abort");
					// DialogResult.Abort is used to mean the instrument initiated a resume
					// or the batch run finished before the pause took effect.
					// There is no other action required here as the Pause/Resume dialog
					// has been dropped.
				}
				else  
				{	// timed out

                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, String.Format("Timed out occurred.")); 
					this.AtReportError(2,"TEC1231",new string[] {});
				}
			}
			return result;
		}

        private void ShowRunProgressScreen()
        {
            RoboSep_RunProgress rp = RoboSep_RunProgress.getInstance();
            if (rp != null)
            {
                Control[] aCtrl = this.Controls.Find(rp.Name, false);
                if (aCtrl == null || aCtrl.Length == 0)
                {
                    RoboSep_RunProgress.getInstance().Visible = false;
                    RoboSep_RunProgress.getInstance().Location = new Point(0, 0);
                    RoboSep_UserConsole.getInstance().Controls.Add(RoboSep_RunProgress.getInstance());
                    RoboSep_UserConsole.ctrlCurrentUserControl = RoboSep_RunProgress.getInstance();
                    RoboSep_RunProgress.getInstance().Visible = true;
                    RoboSep_RunProgress.getInstance().BringToFront();
                }
            }
        }

        private bool ConfirmAbortRun()
        {
            RoboMessagePanel confirmAbort = new RoboMessagePanel(this, MessageIcon.MBICON_WARNING, LanguageINI.GetString("msgHalt"),
                LanguageINI.GetString("headerAbort"), LanguageINI.GetString("Yes"), LanguageINI.GetString("No"));
            this.frmOverlay.Show();
            confirmAbort.ShowDialog();
            this.frmOverlay.Hide();
            DialogResult result = confirmAbort.DialogResult;

            bool AbortRun = result == DialogResult.OK;
            
            confirmAbort.Dispose();
            
            return AbortRun;
        }

        private void AbortRun()
        {
            RoboSep_RunProgress.getInstance().Abbort();
        }

        public string EstimatedCompletion
        {
            get
            {
                return myEstimatedCompletionTime;
            }
        }

        public static void showOverlay()
        {
            myUserConsole.frmOverlay.Show();
            myUserConsole.frmOverlay.Focus();           //CWJ_SCREEN
        }

        public static void hideOverlay()
        {
            myUserConsole.frmOverlay.Hide();
        }

        public void AtReportStatus(string statusCode, string[] statusMessageValues)
        {
            System.Diagnostics.Debug.WriteLine("------------** AtReportStatus in SampleProcessing " + statusCode);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "------------** AtReportStatus in SampleProcessing "+statusCode);

            if (statusMessageValues == null || statusMessageValues.Length < 1)
                return;

            try
            {
                if (statusCode == "TSC3003")
                {
                    pauseCommandCaption = statusMessageValues[0];
                    if (statusMessageValues[0] == "Pause Command" && 2 <= statusMessageValues.Length)
                        pauseCommandCaption = statusMessageValues[1];
                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, String.Format("------------------pauseCommandCaption {0}", pauseCommandCaption));

                }
                else if (statusCode == "TSC1002")
                {
                    if (statusMessageValues[0].StartsWith("Loading", StringComparison.CurrentCultureIgnoreCase))
                    {
                        bLoadingProtocols = true;
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
                    if (RoboSep_RunProgress.getInstance().Initializing)
                        RoboSep_RunProgress.getInstance().Initializing = false;

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
                    else if (command == "MixTrans")
                    {
                        command = "Mix Transport";
                    }
                    else if (command == "TopUpMixTrans")
                    {
                        command = "TopUp Mix Transport";
                    }
                    else if (command == "ResusMixSepTrans")
                    {
                        command = "Resuspend Mix Separate Transport";
                    }
                    else if (command == "ResusMix")
                    {
                        command = "Resuspend Mix";
                    }
                    else if (command == "TopUpTrans")
                    {
                        command = "TopUp Transport";
                    }
                    else if (command == "TopUpTransSepTrans")
                    {
                        command = "TopUp Transport Separate Transport";
                    }
                    else if (command == "TopUpMixTransSepTrans")
                    {
                        command = "TopUp Mix Transport Separate Transport";
                    }

                    RunStatus = command;


                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "<<<<<<<<<<<<<<<<<<< AtReportStatus:  " + command);

                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, String.Format("Progress Update, Next step " + command)); 
                    int id, seq, sec;

                    seq = mySeparator.GetCurrentSeq();
                    id = mySeparator.GetCurrentID();
                    sec = mySeparator.GetCurrentSec();

                    string str = String.Format("CWJ--> ** AtReportStatus ID: {0}, Seq: {1}, Quad: {2}", id, seq, sec);
                    System.Diagnostics.Debug.WriteLine(str);
                    

                    // check if values are update, or unchanged from previous update                    
                    Progress.Add(new ProgressUpdate(id, seq, sec, command));
               }
                else
                {
                    // Catch all other error messages
                    System.Diagnostics.Debug.WriteLine(string.Format(">>----------> Status Code: {0} <----------<<", statusCode));
                    for (int i=0; i< statusMessageValues.Length; i++)
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format(">>----------> {0}", statusMessageValues[i]));
                    }
                    string logMSG = string.Format("\nReport Status Error:\n>>----------> Status Code: {0} <----------<<\n", statusCode);
                    //uiLog.LOG(this, "AtReportStatus", uiLog.LogLevel.ERROR, logMSG );
                    LogFile.AddMessage(TraceLevel.Error, logMSG);
                }
            }
            catch (Exception ex)
            {
                LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);
            }
             
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
            while (Progress.Count > 1)
            {
                Progress.RemoveAt(1);
            }
        }

        private void AttemptSeparatorConnection()
        {
            // LOG
            string logMSG = "Connecting...";
            //GUI_Controls.uiLog.LOG(this, "AttemptSeparatorConnection", GUI_Controls.uiLog.LogLevel.INFO, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG); 
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
                    mySeparatorGateway.UpdateGuiStateChange += new GuiStateChangeDelegate(AtGuiStateChangeUpdate);
                    mySeparatorGateway.UpdateSeparationProtocolTable += new SeparationProtocolTableDelegate(AtSeparationProtocolTableUpdate);
                    mySeparatorGateway.EventsApi.ReportStatus += new ReportStatusDelegate(AtReportStatus);
                    mySeparatorGateway.EventsApi.ReportError += new ReportErrorDelegate(AtReportError);
                    myEventSink.UpdateSeparatorState += new SeparatorStatusDelegate(AtSeparatorStateUpdate);
                    mySeparatorGateway.EventsApi.UpdateChosenProtocols += new ChosenProtocolsDelegate(AtChosenProtocolsUpdate);
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

        private void AtSeparatorStateUpdate(SeparatorState newState)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    SeparatorStatusDelegate eh = new SeparatorStatusDelegate(this.AtSeparatorStateUpdate);
                    this.Invoke(eh, new object[] { newState });
                }
                else
                {
                    switch (newState)
                    {
                        default:
                            break;
                        case SeparatorState.Paused:

                            System.Diagnostics.Debug.WriteLine("------------------------------ - AtSepStateUpdate in PauseResumeDialog, State is PAUSED...call SetPausedMode lidClosed");  // bdr
                            break;

                        // The instrument has either resumed or the batch run has
                        // completed.  Since this was an instrument-initiated event,
                        // use a different return code (that is, we do not want to
                        // send Resume or Halt commands to the instrument).
                        case SeparatorState.Running:
                        case SeparatorState.BatchComplete:
                        case SeparatorState.BatchHalted:
                            DialogResult = DialogResult.Abort;
                            break;
                    } //switch
                }
            }
            catch (Exception ex)
            {
                LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);
            }
        }

        private void AtChosenProtocolsUpdate(IProtocol[] chosenProtocols)
        {
            if (NotifyUserChosenProtocolsUpdated != null)
            {
                this.BeginInvoke((MethodInvoker)delegate { this.NotifyUserChosenProtocolsUpdated(this, new ProtocolEventArgs(chosenProtocols)); });
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
                    //System.Diagnostics.Trace.WriteLine(String.Format("severityLevel={0}, errorCode={1}, errorMessageValues={2}", severityLevel, errorCode, errorMessage));

                    if (isFatalError)
                    {
                        //errorText = "Fatal Error. Please contact technical support.";
                        errorText = SeparatorResourceManager.GetSeparatorString(StringId.ErrorFatal);
                        // Display the error dialog
                        // Halt the run and exit the application
                        //errorText += SeparatorResourceManager.GetSeparatorString(StringId.ErrorTerminate);
                        exitRequired = true;
                        if (isRunning)
                        {
                            haltRequired = true;
                        }
                        // Display the error dialog
                        // Exit the application
                        //errorText += " " +SeparatorResourceManager.GetSeparatorString(StringId.ErrorReportError);
                        errorMessage = errorText + "\r\n\r\n" + errorMessage;
                        mySeparatorGateway.AddRunLogMessage(System.Diagnostics.TraceLevel.Error, StatusCode.TSC3006,
                            new string[] { errorMessage }, false);
                        LogFile.AddMessage(System.Diagnostics.TraceLevel.Error, errorMessage);

                        RoboMessagePanel errorPrompt = new RoboMessagePanel(this, MessageIcon.MBICON_ERROR, errorMessage,
                            SeparatorResourceManager.GetSeparatorString(StringId.Error), LanguageINI.GetString("Ok"));
                        frmOverlay.Show();
                        errorPrompt.ShowDialog();
                        errorPrompt.Dispose();
                        frmOverlay.Hide();

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
                            RoboMessagePanel errorPrompt = new RoboMessagePanel(this, MessageIcon.MBICON_QUESTION, errorMessage,
                                SeparatorResourceManager.GetSeparatorString(StringId.Error), 
                                SeparatorResourceManager.GetSeparatorString(StringId.Yes),
								SeparatorResourceManager.GetSeparatorString(StringId.No));
                            frmOverlay.Show();
                            errorPrompt.ShowDialog();

                            frmOverlay.Hide();
                            DialogResult dlgResult = errorPrompt.DialogResult;

                            errorPrompt.Dispose();                                                        
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
                            RoboMessagePanel errorPrompt = new RoboMessagePanel(this, MessageIcon.MBICON_ERROR, errorMessage,
                                SeparatorResourceManager.GetSeparatorString(StringId.Error),
                                SeparatorResourceManager.GetSeparatorString(StringId.OK));
                            frmOverlay.Show();
                            errorPrompt.ShowDialog();
                            errorPrompt.Dispose();                            
                            frmOverlay.Hide();
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
            //GUI_Controls.uiLog.LOG(this, "RoboSep_UserConsole_Load", GUI_Controls.uiLog.LogLevel.INFO, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG); 
            if (HARDCODE_OS_BIT)
            {
                RoboSep_UserDB.OS_Bit = 32;
            }

            this.Size = new Size(640, 480);
            this.CenterToScreen();
            InitialOpacity = this.Opacity;
            this.Opacity = 0.00D;

            Progress = new List<ProgressUpdate>();
            Progress.Add(new ProgressUpdate(0,0,0, "Initializing"));
            
            
            // StartSplash();

            mySeparatorConnectionThread = new Thread(new ThreadStart(this.AttemptSeparatorConnection));
            mySeparatorConnectionThread.IsBackground = true;
            mySeparatorConnectionThread.Start();
            m_Timer.Enabled = true;

            // LOG
            logMSG = "Starting GUI Thread";
            //GUI_Controls.uiLog.LOG(this, "RoboSep_UserConsole_Load", GUI_Controls.uiLog.LogLevel.GENERAL, logMSG);
            LogFile.AddMessage(TraceLevel.Error, logMSG);

            RoboSep_UserDB.getInstance().XML_SaveUserConfig();
            strCurrentUser = RoboSep_UserDB.XML_UserProfileDBToName();

            logMSG = string.Format("Current user ID : '{0}'.", strCurrentUser);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG); 
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
            Overlay.StartPosition = FormStartPosition.Manual;
            Point winStartPnt = new Point(this.Location.X + OverlayOffset.X, this.Location.Y + OverlayOffset.Y);
            Overlay.Location = winStartPnt;
            Overlay.FormBorderStyle = FormBorderStyle.None;
            Overlay.Enabled = false;
            Overlay.ShowInTaskbar = false;

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
            if (frmList_AllForms.Count == 0)
                return;
 
            try
            {
                var newList = new List<FormTracker>(frmList_AllForms);
                bool bDirty = false;

                // look through list "all forms"
                for (int i = (frmList_AllForms.Count - 1); i >= 0; i--)
                {
                    bool exists = false;
                    // compare all forms to openforms
                    for (int j = 0; j < Application.OpenForms.Count; j++)
                    {
                        if (frmList_AllForms[i].getForm().Equals(Application.OpenForms[j]))
                        {
                            exists = true;
                            break;
                        }
                    }

                    if (exists == false)
                    {
                        newList.Remove(frmList_AllForms[i]);
                        bDirty = true;
                    }
                }

                if (bDirty)
                    frmList_AllForms = newList;
            }
            catch (Exception ex)
            {
                // LOG
                string logMSG = String.Format("Failed to remove form. Exception: {0}.", ex.Message);
                //  (logMSG);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG); 
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
                ActivateRunSamplesScreen();                
            }
            else
            {
                ProgressSplash();
            }
        }

        private void myBeepTimer_Tick(object sender, System.EventArgs e)
        {
            bool bshouldBeep = RoboSep_UserDB.getInstance().EnableBeepSwitch;
            //System.Diagnostics.Debug.WriteLine("------------------beep " + (myBeepTimerCount + 1) + " " + myBeepTimerTotal + " " + bshouldBeep);
            if (bshouldBeep)
            {
                string logMsg = "[BEEP] " + (myBeepTimerCount + 1) + " " + myBeepTimerTotal + " " + bshouldBeep;
                if (myBeepTimer.Interval == myFailureBeepInterval)
                {
                    Tesla.Common.Utilities.Beep(500, 500);
                }
                else
                {
                    Tesla.Common.Utilities.Beep(400, 500);
                }
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMsg);
            }
            else
            {
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "[BEEP] disabled for this profile");
            }

            if (++myBeepTimerCount >= myBeepTimerTotal)
            {
                myBeepTimer.Stop();
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "[BEEP] stops");
            }
        }

        public void RestoreInitialOpacity()
        {
            this.Opacity = InitialOpacity;
        }

        public void StartSplash()
        {
            RoboSep_Splash.getInstance().Opacity = 0;
            RoboSep_Splash.getInstance().Show();
        }

        public void ProgressSplash()
        {
        }


        public void ActivateRunSamplesScreen()
        {
            // LOG
            string logMSG = "Stopping Splash page";
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG); 
            basePannel_Empty.Enabled = true;

            // Pass info to About window
            string[] data = new string[6];

            data[0] = m_sInsVersion;
            data[1] = m_sInsSerial;
            data[2] = m_sInsAddress;
            data[3] = m_sGatewayVer;
            data[4] = m_sGatewayURL;
            data[5] = m_sSvrUptime;

            RoboSep_About.getInstance().UpdateInstrumentLabelInfo(data);

            // create new resources window
            myResourcesWindow = RoboSep_Resources.getInstance();

            if (string.IsNullOrEmpty(strCurrentUser))
            {
                // change to robosep run sample window
                RoboSep_UserSelect userSelect = new RoboSep_UserSelect();
                this.Controls.Add(userSelect); 
                ctrlCurrentUserControl.Visible = false;
                this.Controls.Remove(ctrlCurrentUserControl);
                ctrlCurrentUserControl = userSelect; 
            }
            else
            {
                // change to robosep run sample window
                RoboSep_RunSamples myRunSamples = RoboSep_RunSamples.getInstance();
                this.Controls.Add(myRunSamples); //RoboSep_RunSamples.getInstance());
                ctrlCurrentUserControl.Visible = false;
                this.Controls.Remove(ctrlCurrentUserControl);
                ctrlCurrentUserControl = myRunSamples; //RoboSep_RunSamples.getInstance();
            }

            // close Splash Screen
            // RoboSep_Splash.getInstance().close();

            // Initialize pause resume dialog
            if (myPauseResumeDialog != null)
            {
                myPauseResumeDialog.Dispose();
                myPauseResumeDialog = null;
            }

            myPauseResumeDialog = new PauseResumeDialog(mySeparatorGateway.EventsApi);

            // open warranty warning if necessary
            if (WarrantyNoticeRequired)
            {
                // prompt user that warranty will soon expire
                string sMSG = LanguageINI.GetString("WarrantyExpiring");
                RoboMessagePanel prompt = new RoboMessagePanel(this, MessageIcon.MBICON_WARNING, sMSG,
                    LanguageINI.GetString("headerWarranty"), LanguageINI.GetString("Ok"));
                frmOverlay.Show();
                prompt.ShowDialog();
                prompt.Dispose();
                frmOverlay.Hide();
            }

        }

        public void StartOrcaTCPServer()
        {
            try
            {
                lock (this)
                {
                    mySeparator.StartOrcaTCPServer();
                }
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
        }

        public void StopOrcaTCPServer()
        {
            try
            {
                lock (this)
                {
                    mySeparator.StopOrcaTCPServer();
                }
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
        }

        public string SerialNumber
        {
            get
            {
                return m_sInsSerial;
            }
        }

        public bool IsRemoteDestopLock
        {
            get
            {
                return isRemoteDestopLocked;
            }

            set
            {
                isRemoteDestopLocked = value;
            }
        }

        public void BroadcastOrcaTCPServer(bool status)
        {
            try
            {
                lock (this)
                {
                    mySeparator.BroadcastOrcaTCPServer(status);
                }
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
        }

        public void StopBeepTimer()
        {
            myBeepTimer.Stop();
        }


        public int InitScanVialBarcode(bool freeaix)
        {
            return mySeparatorGateway.InitScanVialBarcode(freeaix);
        }

        public bool AbortScanVialBarcode()
        {
            return mySeparatorGateway.AbortScanVialBarcode();
        }

        public bool IsBarcodeScanningDone()
        {
            return mySeparatorGateway.IsBarcodeScanningDone();
        }

        public int ScanVialBarcodeAt(int Quadrant, int Vial)
        {
            return mySeparatorGateway.ScanVialBarcodeAt(Quadrant, Vial);
        }

        public string GetVialBarcodeAt(int Quadrant, int Vial)
        {
            return mySeparatorGateway.GetVialBarcodeAt(Quadrant, Vial);
        }

        public string GetEndOfRunXMLFullFileName()
        {
            return mySeparatorGateway.GetEndOfRunXMLFullFileName();
        }

        private bool bIsReportProc = false;
        
        public void GenerateReport(string sFullXmlFileName, string sFullPdfFileName)
        {
            
            if (string.IsNullOrEmpty(sFullXmlFileName) || string.IsNullOrEmpty(sFullPdfFileName))
                return;
            try
            {
                // Generate PDF file if not created
                string logMSG;
                logMSG = String.Format(">> enter GenerateReport()!");
                //  (logMSG);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG); 
                if (bIsReportProc == true)
                {
                    logMSG = String.Format("Report is already being processed, exit!");
                    //  (logMSG);
                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG); 
                    return;
                }
                bIsReportProc = true;
                if (!File.Exists(sFullPdfFileName) == true)
                {
                    // Report generator file
                    string fullPathReportGeneratorFileName = RoboSep_UserDB.getInstance().sysPath;
                    fullPathReportGeneratorFileName += "config\\";
                    fullPathReportGeneratorFileName += EndOfRunReportFileName;

                    if (reportHelper == null)
                    {
                        reportHelper = new EndOfRunReportHelper();
                    }
                    reportHelper.ExportReport(sFullXmlFileName, fullPathReportGeneratorFileName, sFullPdfFileName);
                }
                logMSG = String.Format(">> GenerateReport() is done: {0}", sFullPdfFileName);
                //  (logMSG);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG); 
                bIsReportProc = false;
            }
            catch (Exception ex)
            {
                // LOG
                string logMSG = String.Format("Failed to GenerateReport file. Exception: {0}.", ex.Message);
                //  (logMSG);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG); 
                bIsReportProc = false;
            }
        }
        
        public bool GetIgnoreLidSensor()
        {
            return mySeparatorGateway.GetIgnoreLidSensor();
        }

        public void SetIgnoreLidSensor(int sw) 
        {
            mySeparatorGateway.SetIgnoreLidSensor(sw);
        }

        public bool GetIgnoreHydraulicSensor()
        {
            return mySeparatorGateway.GetIgnoreHydraulicSensor();
        }

        public void SetIgnoreHydraulicSensor(int sw)
        {
            mySeparatorGateway.SetIgnoreHydraulicSensor(sw);
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

        public void ScheduleRun(QuadrantId Q)
        {
            mySeparator.ScheduleRun(Q);
        }

        public void SelectProtocol(QuadrantId initialQuadrant, IProtocol protocol)
        {
            mySeparator.SelectProtocol(initialQuadrant, protocol);
        }

        public void DeselectProtocol(QuadrantId quadrantId)
        {
            mySeparator.DeselectProtocol(quadrantId);
        }

        public void SetSampleVolume(QuadrantId quadrantId, FluidVolume mySamplesVolume)
        {
            mySeparatorGateway.SetSampleVolume(quadrantId, mySamplesVolume);

            // LOG
            string logMSG = "Quad: " + ((int)quadrantId +1).ToString() + " Sample Vol " + (mySamplesVolume.Amount/1000).ToString();
            //GUI_Controls.uiLog.LOG(this, "SetSampleVolume", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG); 
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

        private void frmHomOverlay_Focus(object sender, EventArgs e)
        {
            // get a list of all forms
            FormCollection AllForms = Application.OpenForms;
            // make the most recently created form active
            // and bring it to the front
            int lastFormIndex = AllForms.Count - 1;
            if (0 <= lastFormIndex)
            {
                if (lastFormIndex < AllForms.Count)
                    AllForms[lastFormIndex].BringToFront();
                if (lastFormIndex < AllForms.Count)
                    AllForms[lastFormIndex].Activate();
            }
        }

        private void frmOverlay_Focus(object sender, EventArgs e)
        {
            // get a list of all forms
            FormCollection AllForms = Application.OpenForms;
            // make the most recently created form active
            // and bring it to the front
            int lastFormIndex = AllForms.Count - 1;
            if (0 <= lastFormIndex)
            {
                if (lastFormIndex < AllForms.Count)
                    AllForms[lastFormIndex].BringToFront();
                if (lastFormIndex < AllForms.Count)
                    AllForms[lastFormIndex].Activate();
            }
        }

        private void RoboSep_UserConsole_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop the network ORCA service
            if (0 < networkORCAExecutableProcessID)
            {
                try
                {
                    Process proc = Process.GetProcessById(networkORCAExecutableProcessID);
                    if (proc != null)
                    {
                        proc.Kill();
                        string logMSG = string.Format("Stopping service for Network ORCA. processID={0}", networkORCAExecutableProcessID);
                        //  (logMSG);
                        LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG); 
                    }

                }
                catch (Exception ex)
                {
                    string logMSG = string.Format("Error in stopping service with processID={0} for network ORCA. Exception={1}", networkORCAExecutableProcessID, ex.Message.ToString());
                    //  (logMSG);
                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG); 
                }

            }

            GUIShutdown();            
        }

        private void KillExectuable(string strExecutable)
        {
            if (string.IsNullOrEmpty(strExecutable))
            {
                return;
            }

            string fileName = strExecutable;
            if (Path.HasExtension(fileName))
            {
                fileName = Path.GetFileNameWithoutExtension(strExecutable);
            }

            try
            {
                Process[] procs = Process.GetProcessesByName(fileName);
                foreach (Process proc in procs)
                {
                    if (proc != null)
                        proc.Kill();
                }

            }
            catch (Exception ex)
            {
                string logMSG = string.Format("Error in stopping service '{0}' for network ORCA. Exception={1}", strExecutable, ex.Message.ToString());
                //  (logMSG);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG); 
            }
        }
    }


    public class DirectoryComparer : System.Collections.IComparer
    {
        // Calls CaseInsensitiveComparer.Compare with the parameters
        int System.Collections.IComparer.Compare(Object x, Object y)
        {
            string a = Path.GetDirectoryName((string)x);
            string b = Path.GetDirectoryName((string)y);
            return (a.CompareTo(b));
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
