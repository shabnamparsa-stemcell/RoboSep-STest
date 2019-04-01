using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Management;
using System.Net;

using GUI_Controls;
using Tesla.Common;
using Tesla.Separator;
using FTP_Client;

using Invetech.ApplicationLog;
namespace GUI_Console
{
    // Diagnostic package save destination
    public enum enumSaveDestination
    {
        eDestinationUnknown,
        eDestinationUSB,
        eDestinationFTP
    }

    public partial class RoboSep_System : BasePannel
    {
        private static RoboSep_System mySystem;
        private const string roboSepPackageFilePrefix = "RoboSepS";
        private const string diagnosticPackageFileExtension = "zip";
        private const string diagnosticPackageConfigFileName = "PackageList.ini";
        private const string diagnosticPackageTempSubFolderName = "ZippedFiles";
        private const string TextPassPhase = "roboseptechsupport";
        private const string windowsAppEventsLogFileName = "WindowsApplicationEvents.log";
        private const string windowsSysEventsLogFileName = "WindowsSystemEvents.log";
        private const string windowsEventTempSubFolderName = "WindowsEventsFiles";

        private const int MinUtilityTimeoutInSeconds = 30;
 
        private List<Image> Ilist = new List<Image>();
        private IniFile GUIini;
        private IniFile LanguageINI;
        private string tempZipTarget;

        private ManagementEventWatcher myUSBDeviceRemovedWatcher;
        private ManagementEventWatcher myUSBDeviceAddedWatcher;
        private BackgroundWorker myAsyncWorker = new BackgroundWorker();
        private DoWorkEventHandler evhUSBDeviceAdded;
        private RunWorkerCompletedEventHandler evhUSBDeviceAddedCompleted;


        private const string DefaultUtilityFilenames = "RoboSep_SetDefault.exe";
        private const string USBRemovedWatcherQuery = "SELECT * FROM __InstanceDeletionEvent WITHIN 10 WHERE TargetInstance ISA \"Win32_LogicalDisk\"";
        private const string USBAddedWatcherQuery = "SELECT * FROM __InstanceCreationEvent WITHIN 10 WHERE TargetInstance ISA \"Win32_LogicalDisk\"";
        private const string CaptionFieldName = "Caption";
        private const string DefaultPassword = "RoboSepService";

        private char[] FileSeparator = new char[] { ',', ';' };

        private static AutoResetEvent asyncZippingIsDone = new AutoResetEvent(false);

        RoboMessagePanel5 FTPMsg = null, USBMsg = null;
        FileBrowser selectFolder = null;
        

        private bool bInitialized = false;
        private bool bUSBWatcherInitialized = false;
         
        // FTP
        HE he = null;

        private RoboSep_System()
        {
            InitializeComponent();

            // LOG
            string logMSG = "Initializing System user control";
            //GUI_Controls.uiLog.LOG(this, "RoboSep_System", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                                                                            
            Ilist.Add(Properties.Resources.SR_BT04L_exit_software_STD);
            Ilist.Add(Properties.Resources.SR_BT04L_exit_software_OVER);
            Ilist.Add(Properties.Resources.SR_BT04L_exit_software_OVER);
            Ilist.Add(Properties.Resources.SR_BT04L_exit_software_CLICK);
            button_Shutdown.ChangeGraphics(Ilist);
            button_Shutdown.disableImage = Properties.Resources.SR_BT04L_exit_software_DISABLE;

            // Diagnostic button
            Ilist.Clear();
            Ilist.Add(Properties.Resources.SR_BT01L_diagnostic_STD);
            Ilist.Add(Properties.Resources.SR_BT01L_diagnostic_OVER);
            Ilist.Add(Properties.Resources.SR_BT01L_diagnostic_OVER);
            Ilist.Add(Properties.Resources.SR_BT01L_diagnostic_CLICK);
            button_Diagnostic.ChangeGraphics(Ilist);
            button_Diagnostic.disableImage = Properties.Resources.SR_BT01L_diagnostic_DISABLE;

            // Service button
            Ilist.Clear();
            Ilist.Add(Properties.Resources.SR_BT03L_service_menu_STD);
            Ilist.Add(Properties.Resources.SR_BT03L_service_menu_OVER);
            Ilist.Add(Properties.Resources.SR_BT03L_service_menu_OVER);
            Ilist.Add(Properties.Resources.SR_BT03L_service_menu_CLICK);
            button_Service.ChangeGraphics(Ilist);
            button_Service.disableImage = Properties.Resources.SR_BT03L_service_menu_DISABLE;

            // Logs button
            Ilist.Clear();
            Ilist.Add(Properties.Resources.SR_BT02L_run_logs_STD);
            Ilist.Add(Properties.Resources.SR_BT02L_run_logs_OVER);
            Ilist.Add(Properties.Resources.SR_BT02L_run_logs_OVER);
            Ilist.Add(Properties.Resources.SR_BT02L_run_logs_CLICK);
            button_Logs.ChangeGraphics(Ilist);

            // Home all and Basic Prime buttons
            Ilist.Clear();
            Ilist.Add(GUI_Controls.Properties.Resources.btnLG_STD);
            Ilist.Add(GUI_Controls.Properties.Resources.btnLG_STD);
            Ilist.Add(GUI_Controls.Properties.Resources.btnLG_STD);
            Ilist.Add(GUI_Controls.Properties.Resources.btnLG_CLICK);
            Button_BasicPrime.ChangeGraphics(Ilist);
            button_Home.ChangeGraphics(Ilist);

            GUIini = new IniFile(IniFile.iniFile.GUI);
        }

        public static RoboSep_System getInstance()
        {
            if (mySystem == null)
            {
                mySystem = new RoboSep_System();
            }
            return mySystem;
        }

        // Touch Keyboard Enable / Disable
        private void checkBox_keyboard_Click(object sender, EventArgs e)
        {
            RoboSep_UserConsole.KeyboardEnabled = checkBox_keyboard.Check;

            // LOG
            string logMSG = "Touch Keyboard enabled = " + checkBox_keyboard.Check.ToString();
            //GUI_Controls.uiLog.LOG(this, "checkBox_keyboard_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
        }
        
        private void label_keybaord_Click(object sender, EventArgs e)
        {
            if (checkBox_keyboard.Check)
            { checkBox_keyboard.Check = false; }
            else
            { checkBox_keyboard.Check = true; }
            RoboSep_UserConsole.KeyboardEnabled = checkBox_keyboard.Check;

            // LOG
            string logMSG = "Touch Keyboard enabled = " + checkBox_keyboard.Check.ToString();
            //GUI_Controls.uiLog.LOG(this, "label_keybaord_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
        }

        // RoboSep Lid Sensor Enable / Disable
        private void checkBox_lidSensor_Click(object sender, EventArgs e)
        {
            // LOG
            string logMSG = "Lid Sensor enabled = " + checkBox_lidSensor.Check.ToString();
            //GUI_Controls.uiLog.LOG(this, "checkBox_lidSensor_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
            // set liquid level sensor enable
            EnableLidSensor(checkBox_lidSensor.Check);
        }

        private void label_lidSensor_Click(object sender, EventArgs e)
        {
            if (checkBox_lidSensor.Check)
            { 
                checkBox_lidSensor.Check = false; 
            }
            else
            { 
                checkBox_lidSensor.Check = true; 
            }
            // LOG
            string logMSG = "Lid Sensor enabled = " + checkBox_lidSensor.Check.ToString();
            //GUI_Controls.uiLog.LOG(this, "label_lidSensor_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
            // set liquid level sensor enable
            EnableLidSensor(checkBox_lidSensor.Check);
        }

        // function to enable / disable Lid Sensor
        private void EnableLidSensor(bool enable)
        {
            int setEnableLidSensor = enable ? 0 : 1;
            RoboSep_UserConsole.getInstance().SetIgnoreLidSensor(setEnableLidSensor);
        }

        //
        // RoboSep Liquid Level Sensor Enable / Disable
        //
        private void checkBox_liquidSensor_Click(object sender, EventArgs e)
        {
            // LOG
            string logMSG = "Liquid Level Sensor enabled = " + checkBox_liquidSensor.Check.ToString();
            //GUI_Controls.uiLog.LOG(this, "checkBox_liquidSensor_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
            // set liquid level sensor enable
            EnableLiquidLevelSensor(checkBox_liquidSensor.Check);
        }

        private void label_liquidSensor_Click(object sender, EventArgs e)
        {
            if (checkBox_liquidSensor.Check)
            { 
                checkBox_liquidSensor.Check = false; 
            }
            else
            { 
                checkBox_liquidSensor.Check = true; 
            }
            // LOG
            string logMSG = "Liquid Level Sensor enabled = " + checkBox_liquidSensor.Check.ToString();
            //GUI_Controls.uiLog.LOG(this, "label_liquidSensor_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
            // set liquid level sensor enable
            EnableLiquidLevelSensor(checkBox_liquidSensor.Check);
        }

        // function to enable / disable Liquid Level Sensor
        private void EnableLiquidLevelSensor(bool enable)
        {
            int setEnableLiquidLevelSensor = enable ? 0 : 1;
            RoboSep_UserConsole.getInstance().SetIgnoreHydraulicSensor(setEnableLiquidLevelSensor);
        }

        private void checkBox_StartTimer_Click(object sender, EventArgs e)
        {
            RoboSep_UserConsole.bStartTimerEbnable = checkBox_StartTimer.Check;

            GUIini.IniWriteValue("System", "EnableStartTimer",
                checkBox_StartTimer.Check ? "true" : "false");

            // set liquid level sensor enable
            EnableStartTimer(checkBox_StartTimer.Check);

            // LOG
            string logMSG = "Start Timer enabled = " + checkBox_StartTimer.Check.ToString();
            //GUI_Controls.uiLog.LOG(this, "checkBox_StartTimer.Check", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
        }

        // function to enable / disable delayed start timer
        private void EnableStartTimer(bool enable)
        {
            int setEnableStartTimer = enable ? 0 : 1;
            // Sunny to do
            //    SeparatorGateway.GetInstance().SetIgnoreStartTimer(setEnableStartTimer);

            // modify Hardware.ini
         //   IniFile hardwareINI = new IniFile(IniFile.iniFile.Hardware);
          //  hardwareINI.IniWriteValue("Start Timer", "ignoreStartTimer", setEnableStartTimer.ToString());
        }

        private void label_StartTimer_Click(object sender, EventArgs e)
        {
            if (checkBox_StartTimer.Check)
            { checkBox_StartTimer.Check = false; }
            else
            { checkBox_StartTimer.Check = true; }
            RoboSep_UserConsole.bStartTimerEbnable = checkBox_StartTimer.Check;

            // LOG
            string logMSG = "Start Timer enabled = " + checkBox_StartTimer.Check.ToString();
            //GUI_Controls.uiLog.LOG(this, "label_StartTimer_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
            EnableStartTimer(checkBox_StartTimer.Check);
        }

        private void button_Shutdown_Click(object sender, EventArgs e)
        {
            // LOG
            string logMSG = "Shutdown Software clicked";
            //GUI_Controls.uiLog.LOG(this, "button_Shutdown_Click", GUI_Controls.uiLog.LogLevel.GENERAL, logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Warning, logMSG);
            
            // prompt user to confirm
            string msg = LanguageINI.GetString("msgExitSoftware");
            RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING, msg,
                LanguageINI.GetString("headerExitSoftware"), LanguageINI.GetString("Yes"), LanguageINI.GetString("No"));
            RoboSep_UserConsole.showOverlay();
            prompt.ShowDialog();
            RoboSep_UserConsole.hideOverlay();

            if (prompt.DialogResult != DialogResult.OK)
            {
                prompt.Dispose();
                return;
            }
            prompt.Dispose();
            // Send command to lock remote deskstop
            RemoteDesktopLock();
            //SHUTDOWN!!
            
            // Stop Orca server
            RoboSep_UserConsole.getInstance().StopOrcaTCPServer();

            ExitSoftwareTimer.Start();
        }

        private void button_Service_Click(object sender, EventArgs e)
        {
            //RoboSep_ServiceLogin ServiceWindow = new RoboSep_ServiceLogin();
            //ServiceWindow.Location = new Point(0, 0);
            //this.Visible = false;
            //RoboSep_UserConsole.getInstance().Controls.Add(ServiceWindow);
            //RoboSep_UserConsole.getInstance().Controls.Remove(RoboSep_UserConsole.ctrlCurrentUserControl);
            //RoboSep_UserConsole.ctrlCurrentUserControl = ServiceWindow;

            // see if Service target directory exist
            string systemPath = RoboSep_UserDB.getInstance().sysPath;
            string[] directories = systemPath.Split('\\');

            string servicePath = string.Empty;
            for (int i = 0; i < directories.Length - 2; i++)
                servicePath += directories[i] + "\\";
            servicePath += "RoboSepService\\bin\\";

            // check if service directory exists
            if (!System.IO.Directory.Exists(servicePath))
                System.IO.Directory.CreateDirectory(servicePath);

            // if Service.exe exists, run service.exe
            if (!System.IO.File.Exists(servicePath + "Service.exe"))
                return;

            this.SuspendLayout();

            // run Service
            ProcessStartInfo ServiceStartInfo = new ProcessStartInfo();
            ServiceStartInfo.WorkingDirectory = servicePath;
            ServiceStartInfo.FileName = "Service.exe";
            ServiceStartInfo.WindowStyle = ProcessWindowStyle.Normal;

            Process ServiceProgram = new Process();
            ServiceProgram.StartInfo = ServiceStartInfo;
            ServiceProgram.EnableRaisingEvents = true;
            ServiceProgram.Exited += new System.EventHandler(this.HandleServiceProgramExit);
            ServiceProgram.Start();
 
            this.ResumeLayout();
            this.Visible = false;

            // LOG
            string logMSG = "Service Menu button clicked";
            //GUI_Controls.uiLog.LOG(this, "button_Service_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
        }

        private void Button_BasicPrime_Click(object sender, EventArgs e)
        {
            //RoboSep_UserConsole.getInstance().InvokeShutdownAction(); // shutdown protocol
            runMaintenance(0);

            // LOG
            string logMSG = "Basic Prime Maintenance button clicked";
            //GUI_Controls.uiLog.LOG(this, "Button_BasicPrime_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
        }

        private void button_Home_Click(object sender, EventArgs e)
        {
            runMaintenance(1);

            // LOG
            string logMSG = "Home All Maintenance button clicked";
            //GUI_Controls.uiLog.LOG(this, "button_Home_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
        }

        private void RemoteDesktopLock()
        {
            // Lock
            RoboSep_UserConsole.getInstance().BroadcastOrcaTCPServer(false);
            RoboSep_UserConsole.getInstance().IsRemoteDestopLock = true;
        }

        private void HandleServiceProgramExit(object sender, EventArgs e)
        {
            this.Visible = true;
        }

        private void runMaintenance(int MaintenanceIndex)
        {
            // if run is currently in progress, do not run maintenance.
            DialogResult RunMaintenance = RoboSep_UserConsole.bIsRunning ? DialogResult.Cancel : DialogResult.OK;

            // check if protocols are loaded for separation run.
            if (RoboSep_RunSamples.getInstance().iSelectedProtocols.Length > 0 &&
                !RoboSep_UserConsole.bIsRunning)
            {
                string sMSG = LanguageINI.GetString("msgSeparationProtocolsPresent");
                RoboMessagePanel Prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING, sMSG, 
                    LanguageINI.GetString("headerSeparationProtocolPresent") ,LanguageINI.GetString("Yes"), LanguageINI.GetString("No"));
                RoboSep_UserConsole.showOverlay();
                Prompt.ShowDialog();
                RoboSep_UserConsole.hideOverlay();
                RunMaintenance = Prompt.DialogResult;
                Prompt.Dispose();
            }

            if (RunMaintenance == DialogResult.OK)
            {
                // add maintenance protocol to sample processing window
                RoboSep_UserConsole.getInstance().InvokeMaintenanceAction(MaintenanceIndex);

                // open sample processing window
                RoboSep_RunSamples.getInstance().Visible = false;
                RoboSep_UserConsole.getInstance().Controls.Add(RoboSep_RunSamples.getInstance());
                RoboSep_UserConsole.getInstance().Controls.Remove(this);
                RoboSep_RunSamples.getInstance().Visible = true;
                RoboSep_UserConsole.ctrlCurrentUserControl = RoboSep_RunSamples.getInstance();
            }
        }

        private void button_Diagnostic_Click(object sender, EventArgs e)
        {
            bool bIncludeWindowsEvents = false;
            enumSaveDestination eDestination = enumSaveDestination.eDestinationUnknown;
            RoboSep_UserConsole.showOverlay();
            Form_DiagnosticPackage diagnosticForm = new Form_DiagnosticPackage();
            diagnosticForm.ShowDialog();
            RoboSep_UserConsole.hideOverlay();
            if (diagnosticForm.DialogResult != DialogResult.OK)
            {
                diagnosticForm.Dispose();                
                return;
            }


            bIncludeWindowsEvents = diagnosticForm.IncludeWindowsEventsLogs();
            eDestination = diagnosticForm.Destination();
            diagnosticForm.Dispose();                
            switch (eDestination)
            {
                case enumSaveDestination.eDestinationUSB:
                    ExportToUSB(bIncludeWindowsEvents);
                    break;
                case enumSaveDestination.eDestinationFTP:
                    ExportToFTP(bIncludeWindowsEvents);
                    break;
                default:
                    break;
            }
        }

        private void ExportToUSB(bool includeWindowsEvents)
        {
            // stop monitoring USB  
            StopWatchUSB();

            // Ask user to insert USB 
            List<string> lstUSBDrives = AskUserToInsertUSBDrive();
            if (lstUSBDrives == null || lstUSBDrives.Count == 0)
            {
                return;
            }

            ShowPreparingDiagnosticPackageMsg();

            // do the package
            ZipRoboSepLogsToUSB(includeWindowsEvents);
        }

        private void WriteToUSB(string[] aFileNames)
        {
            string compressedFileName = aFileNames[0];
            string tempZipPath = aFileNames[1];

            if (string.IsNullOrEmpty(compressedFileName) || string.IsNullOrEmpty(tempZipPath))
                return;

            string systemPath = Utilities.GetRoboSepSysPath();
            string ExecutableDir = systemPath + "bin\\";
            bool bDeleteTempDirectory = true;


            // Ask user selects USB  
            List<string> lstUSBDrives = AskUserToInsertUSBDrive();
            if (lstUSBDrives == null || lstUSBDrives.Count == 0)
            {
                return;
            }

            // get serial number
            string sDirectorySerialNumber = lstUSBDrives[0];
            if (!string.IsNullOrEmpty(sDirectorySerialNumber))
            {
                if (sDirectorySerialNumber.LastIndexOf('\\') != (sDirectorySerialNumber.Length - 1))
                {
                    sDirectorySerialNumber += "\\";
                }
                sDirectorySerialNumber += RoboSep_UserConsole.getInstance().SerialNumber;

                if (!Directory.Exists(sDirectorySerialNumber))
                {
                    // Create the directory
                    Directory.CreateDirectory(sDirectorySerialNumber);
                }
            }

            // open file browser to allow user to select specific file location
            string sTitle = LanguageINI.GetString("headerSaveDiagnosticPackageUSB");
            selectFolder = new FileBrowser(RoboSep_UserConsole.getInstance(), sTitle, sDirectorySerialNumber, compressedFileName);
            selectFolder.ShowDialog();

            // get target directory from file browser dialog
            if (selectFolder.DialogResult == DialogResult.Yes)
            {
                string Target = selectFolder.Target;
                    
                // remove illegal characters at the beginning and the end
                string Source = Utilities.RemoveIllegalCharsInDirectory(tempZipTarget);

                if (!string.IsNullOrEmpty(Source))
                {
                    selectFolder.CopyFileEnd += new CopyFileEndEventHandler(CopyDiagnosticPackageFinished);

                    // Copy files to USB directory
                    selectFolder.CopyToTargetDir(tempZipTarget, Target);

                    bDeleteTempDirectory = false;
                }
                

            }
            selectFolder.Dispose();
            try
            {
                // delete the embedded DLL
                RemoveEmbeddedDLL(ExecutableDir);

                if (bDeleteTempDirectory)
                {
                    // delete temp folder 
                    Utilities.RemoveTempFileDirectory(tempZipPath);
                }
            }
            catch (Exception ex)
            {
                // LOG
                LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);               
            }
            RoboSep_UserConsole.hideOverlay();
        }

        private void ZipRoboSepLogsToUSB(bool includeWindowsEvents)
        {
            // Use background thread for doing the zipping
            BackgroundWorker bwWorkerZippingToUSB = new BackgroundWorker();

            bwWorkerZippingToUSB.WorkerSupportsCancellation = true;
            DoWorkEventHandler evhZipping = new DoWorkEventHandler(bwWorkerZippingRoboSepLogsToUSB_DoWork);
            RunWorkerCompletedEventHandler evhZippingCompleted = new RunWorkerCompletedEventHandler(bwWorkerZippingRoboSepLogsToUSB_RunWorkerCompleted);

            // Attach the event handlers
            bwWorkerZippingToUSB.DoWork += evhZipping;
            bwWorkerZippingToUSB.RunWorkerCompleted += evhZippingCompleted;

              // Kick off the Async thread
            bwWorkerZippingToUSB.RunWorkerAsync(includeWindowsEvents);
        }

        // function to do zipping
        private void bwWorkerZippingRoboSepLogsToUSB_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            bool bIncludeWindowsEvents = (bool)e.Argument;
            try
            {
                // Disable buttons
                this.Invoke(
                    (MethodInvoker)delegate()
                    {
                        UpdateButtons(false);
                    }
                );

                string[] aFileName = new string[2];
                aFileName[0] = String.Empty;
                aFileName[1] = String.Empty;

                // Zipping files
                DoZippingRoboSepLogsPackage(aFileName, bIncludeWindowsEvents);
                e.Result = aFileName;
            }
            catch (Exception ex)
            {
                e.Cancel = true;

                // LOG
                LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);               

                string sHeader = LanguageINI.GetString("headerFailedToZipDiagnosticPackage");
                string sMsg = LanguageINI.GetString("msgFailedToZipDiagnosticPackage");
                string sOK = LanguageINI.GetString("Ok");
                GUI_Controls.RoboMessagePanel prompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_ERROR, sMsg, sHeader, sOK);
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                RoboSep_UserConsole.hideOverlay();
                prompt.Dispose();
                return;
            }
        }

        private void bwWorkerZippingRoboSepLogsToUSB_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Invoke(
               (MethodInvoker)delegate()
               {
                   CloseUSBMsg();
                   if (!e.Cancelled && e.Error == null)
                   {
                       string[] para = e.Result as string[];
                       WriteToUSB(para);
                   }

                   // Enable the buttons again
                   UpdateButtons(true);
               }
           );
        }

        private void PrepareWindowsEventLogs(string iTempPath)
        {
            EventLog eLog = null;
            FileStream fS = null;
            StreamWriter sW = null;

            if (string.IsNullOrEmpty(iTempPath))
                return;

            string logMsg;
            string sHostName = Dns.GetHostName();
            if (string.IsNullOrEmpty(sHostName))
            {
                // LOG
                logMsg = string.Format("Failed to get host name.");
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Warning, logMsg);
                return;
            }
            IPHostEntry hostInfo = Dns.GetHostEntry(sHostName);
            if (hostInfo == null)
            {
                // LOG
                logMsg = string.Format("Dns.GetHostEntry failed to resolves a host name.");
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Warning, logMsg);
                return;
            }

            string tempPath = iTempPath;
            if (tempPath.LastIndexOf('\\') != (tempPath.Length - 1))
            {
                tempPath += "\\";
            }

            if (!Directory.Exists(iTempPath))
                Directory.CreateDirectory(iTempPath);

            try
            {
                using (new StopWatch("Retrieving Windows Events logs"))
                {
                    string fullPathUserFileName, sTemp;
                    eLog = new EventLog();
                    if (eLog == null)
                    {
                        logMsg = string.Format("Failed to retrieve Windows Events log because it failed to create an EventLog instance.");
                        LogFile.AddMessage(System.Diagnostics.TraceLevel.Warning, logMsg);
                        return;
                    }

                    // check for the application event log source on specified machine
                    if (EventLog.Exists("Application"))
                    {
                        eLog.Log = "Application";
                        logMsg = String.Format("No of Windows Application messages = {0}.", eLog.Entries.Count);
                        //  (logMsg);
                        LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMsg);

                        fullPathUserFileName = tempPath + windowsAppEventsLogFileName;

                        fS = new FileStream(fullPathUserFileName, FileMode.Create);
                        sW = new StreamWriter(fS);
                        // write log line to current log file
                        foreach (System.Diagnostics.EventLogEntry entry in eLog.Entries)
                        {
                            if (entry == null)
                                continue;

                            // Add event fields to the listbox, separated by tabspace (\t). 
                            sTemp = "-->" + entry.InstanceId
                                                + "\t" + entry.EntryType
                                                + "\t" + entry.TimeGenerated
                                                + "\t" + entry.Source
                                                + "\t" + entry.Message;
                            sW.WriteLine(sTemp);
                        }
                    }
                    else
                    {
                        // LOG
                        logMsg = string.Format("Windows Application log does not exist.");
                        LogFile.AddMessage(System.Diagnostics.TraceLevel.Warning, logMsg);
                    }

                    // check for the event log source on specified machine
                    if (EventLog.Exists("System"))
                    {
                        eLog.Log = "System";
                        logMsg = String.Format("No of Windows System messages = {0}.", eLog.Entries.Count);
                        //  (logMsg);
                        LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMsg);

                        // write log line to current log file
                        
                        fullPathUserFileName = tempPath + windowsSysEventsLogFileName;
                        if (sW != null)
                            sW.Close();
                        if (fS != null)
                            fS.Close();
                        
                        fS = new FileStream(fullPathUserFileName, FileMode.Create);
                        sW = new StreamWriter(fS);
                        foreach (System.Diagnostics.EventLogEntry entry in eLog.Entries)
                        {
                            if (entry == null)
                                continue;

                            // Add event fields to the listbox, separated by tabspace (\t). 
                            sTemp = "-->" + entry.InstanceId
                                                + "\t" + entry.EntryType
                                                + "\t" + entry.TimeGenerated
                                                + "\t" + entry.Source
                                                + "\t" + entry.Message;
                            sW.WriteLine(sTemp);
                        }
                    }
                    else 
                    {
                        // LOG
                        logMsg = string.Format("Windows System log does not exist.");
                        LogFile.AddMessage(System.Diagnostics.TraceLevel.Warning, logMsg);
                    }
                }
            }
            catch (Exception ex)
            {
                // LOG
                LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);               
            }
            finally
            {
                if (sW != null)
                {
                    sW.Close();
                    sW = null;
                }
                if (fS != null)
                {
                    fS.Close();
                    fS = null;
                }
                if (eLog != null)
                {
                    eLog.Close();
                    eLog = null;
                }
            }
        }

        private bool DoZippingRoboSepLogsPackage(string[] aFileNames, bool includeWindowsEvents)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("---------Zipping diagnostic files. Time:{0}", DateTime.Now.ToLongTimeString()));

            if (aFileNames == null || aFileNames.Length == 0)
                return false;

            string tempZipPath;

            // see if zip target directory exist
            string systemPath = Utilities.GetRoboSepSysPath();
            string TempPath = systemPath + "temp\\";
            if (!Directory.Exists(TempPath))
                Directory.CreateDirectory(TempPath);

            tempZipPath = Utilities.GetTempFileFolder();
            if (!Directory.Exists(tempZipPath))
                Directory.CreateDirectory(tempZipPath);

            // Include Windows Events logs if required
            string sTempWindowEvent = String.Empty;
            if (includeWindowsEvents)
            {
                sTempWindowEvent = tempZipPath;
                if (sTempWindowEvent.LastIndexOf('\\') != (sTempWindowEvent.Length - 1))
                {
                    sTempWindowEvent += "\\";
                }

                sTempWindowEvent += windowsEventTempSubFolderName;

                PrepareWindowsEventLogs(sTempWindowEvent);
            }

            string tempZipDestination = tempZipPath + "\\" + diagnosticPackageTempSubFolderName + "\\";
            if (!Directory.Exists(tempZipDestination))
                Directory.CreateDirectory(tempZipDestination);

            tempZipTarget = String.Empty;

            string compressedFileName = String.Format("{0}__SN{1}__{2}.{3}", roboSepPackageFilePrefix, 
                RoboSep_UserConsole.getInstance().SerialNumber, 
                Utilities.GetCurrentTimeStamp(), 
                diagnosticPackageFileExtension);

            tempZipDestination += compressedFileName;
            tempZipTarget = "\"" + tempZipDestination + "\"";

            string param = PrepareRoboSepLogsArguments(sTempWindowEvent);
            if (string.IsNullOrEmpty(param) || param.Trim() == string.Empty)
                return false;

            // sunny to do
            string ExecutableDir = systemPath + "bin\\";
            string msg;
            if (!GetEmbeddedDLL(ExecutableDir))
            {
                msg = LanguageINI.GetString("msgEmbedDLLFail");
                RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_ERROR, msg,
                    LanguageINI.GetString("headerSaveUSB"), LanguageINI.GetString("Ok"));
                prompt.ShowDialog();

                // LOG
                string logMSG = "Failed to get embedded DLL, 'cygwin1.dll' from GUI_Console.LIB.";
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Warning, logMSG);
                prompt.Dispose();
                return false;
            }

            aFileNames[0] = compressedFileName;
            aFileNames[1] = tempZipPath;


            //Package files and directories here.
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = ExecutableDir + "\\zip.exe";
            process.StartInfo.WorkingDirectory = ExecutableDir;
            process.StartInfo.Arguments = " -9r " + tempZipTarget + " " + param;
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();

            System.Diagnostics.Debug.WriteLine(String.Format("---------Zipping diagnostic files completed. Time:{0}", DateTime.Now.ToLongTimeString()));
            return true;
        }

        private void ShowPreparingDiagnosticPackageMsg()
        {
            // show zipping message
            string title = LanguageINI.GetString("headerZipping");
            string msg = LanguageINI.GetString("msgWaitForZip");
            USBMsg = new RoboMessagePanel5(RoboSep_UserConsole.getInstance(), title, msg, GUI_Controls.GifAnimationMode.eExportSingleFile);
            USBMsg.Show();
            USBMsg.Refresh();
        }

        private void UpdateUSBMsgToExportingFile()
        {
            if (USBMsg != null)
            {
                // show zipping message
                string title = LanguageINI.GetString("headerExportingDiagPackage");
                string msg = LanguageINI.GetString("msgWaitExportingDiagPackage");
                USBMsg.SetMessageInfo(title, msg, GUI_Controls.GifAnimationMode.eExportSingleFile);
            }
        }

        private void CloseUSBMsg()
        {
            if (USBMsg != null)
            {
                USBMsg.Close();
                RoboSep_UserConsole.hideOverlay();
                USBMsg = null;
            }
        }

        private void UpdateButtons(bool bEnable)
        {
            button_Diagnostic.disable(!bEnable);
            button_Service.disable(!bEnable);
            button_Shutdown.disable(!bEnable);       
        }

        private void CopyDiagnosticPackageFinished(object sender, CopyFileEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate() { this.CopyDiagnosticPackageFinished(sender, e); });
            }
            else
            {
                // Delete Temp File
                if (string.IsNullOrEmpty(tempZipTarget))
                    return;

                tempZipTarget = Utilities.RemoveIllegalCharsInDirectory(tempZipTarget);

                // delete temp folder 
                Utilities.RemoveTempFileDirectory(tempZipTarget);

                tempZipTarget = String.Empty;

                if (selectFolder != null)
                {
                    selectFolder.Dispose();
                    selectFolder = null;
                }

                string msg = LanguageINI.GetString("msgSaveDiagnosticPackageUSBOK");
                RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_INFORMATION, msg,
                    LanguageINI.GetString("headerSaveDiagnosticPackageUSB"), LanguageINI.GetString("Ok"));
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                RoboSep_UserConsole.hideOverlay();
                prompt.Dispose();
            }
        }

        private List<string> AskUserToInsertUSBDrive()
        {
            List<string> lstUSBDrives = GetUSBDrives();
            if (lstUSBDrives == null)
                return null;

            if (lstUSBDrives.Count > 0)
                return lstUSBDrives;

            string msg = LanguageINI.GetString("msgInsertUSB");
            RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_INFORMATION, msg,
                LanguageINI.GetString("headerZipping"), LanguageINI.GetString("Yes"), LanguageINI.GetString("Cancel"));
            RoboSep_UserConsole.showOverlay();
            prompt.ShowDialog();
            RoboSep_UserConsole.hideOverlay();
            if (prompt.DialogResult != DialogResult.OK)
            {
                prompt.Dispose();
                return null;
            }
            prompt.Dispose();
            return AskUserToInsertUSBDrive();
        }

        private List<string> GetUSBDrives()
        {
            List<string> lstUSBDrives = new List<string>();

            foreach (DriveInfo driveInfo in DriveInfo.GetDrives())
            {
                if (driveInfo.DriveType == DriveType.Removable && driveInfo.IsReady == true)
                {
                    lstUSBDrives.Add(driveInfo.Name);
                }
            }
            return lstUSBDrives;
        }
  
        private string PrepareRoboSepLogsArguments(string iWindowsEventsPath)
        {
            // read packaginglist config.
            string logMSG = "";
            string systemPath = Utilities.GetRoboSepSysPath();
            string diagnosticPackageConfig = systemPath + "config\\";
            diagnosticPackageConfig += diagnosticPackageConfigFileName;
            if (!File.Exists(diagnosticPackageConfig))
            {
                logMSG = "PackageList.ini config. file is not existed. Diagnostic Package is not configured.";
                //  (logMSG);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
                return null;
            }

            string STIpath = Utilities.GetRoboSepSTIpath();
            if (string.IsNullOrEmpty(STIpath))
                return null;

            // read config file
            string[] lines = File.ReadAllLines(diagnosticPackageConfig, System.Text.Encoding.UTF8);
            List<string> lst = new List<string>();
            lst.AddRange(lines);

            // Note: these variable names are from the PackageList.ini
            string txtZipDirectory = "zipDirectory";
            string txtExcludeDirectory = "excludeDirectory";

            string param = "";
            string[] tempZipDirectories = getConfigValues(ref lst, txtZipDirectory);
            if (tempZipDirectories == null)
                return null;

            string[] zipDirectories = GetValidDirectories(tempZipDirectories);
            if (zipDirectories == null || zipDirectories.Length == 0)
                return null;

            foreach (string directory in zipDirectories)
            {
                if (string.IsNullOrEmpty(directory))
                    continue;

                param += "\"" + directory + "\" ";
            }

            if (!string.IsNullOrEmpty(iWindowsEventsPath))
            {
                param += "\"" + iWindowsEventsPath;
                if (iWindowsEventsPath.LastIndexOf('\\') != (iWindowsEventsPath.Length - 1))
                {
                    param += "\" ";
                }
            }

            string[] tempExcludeDirectories = getConfigValues(ref lst, txtExcludeDirectory);
            if (tempExcludeDirectories != null && 0 < tempExcludeDirectories.Length)
            {
                string[] exculdeDirectories = GetValidDirectories(tempExcludeDirectories);
                if (exculdeDirectories != null && 0 < exculdeDirectories.Length)
                {
                    foreach (string directory in exculdeDirectories)
                    {
                        if (string.IsNullOrEmpty(directory))
                            continue;

                        param += "-x \"" + directory + "\\*\" ";
                    }
                }
            }
            return param;
        }

        private string[] getConfigValues(ref List<string> lst, string configVariable)
        {
            if (lst == null || string.IsNullOrEmpty(configVariable))
                return null;

            List<string> tempList = new List<string>();
            int index = 0;
            string[] parts;
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
                            tempList.Add(parts[1].Trim());
                        }
                    }
                    index++;
                    index = lst.FindIndex(index, x => { return (!string.IsNullOrEmpty(x) && x.Length >= configVariable.Length && x.ToLower().Contains(configVariable.ToLower())); });
                }

            } while (0 <= index);

            return tempList.ToArray();
        }

        private string[] GetValidDirectories(string[] directories)
        {
            if (directories == null || directories.Length == 0)
                return null;

            string STIpath = Utilities.GetRoboSepSTIpath();
            if (string.IsNullOrEmpty(STIpath))
                return null;

            int index = 0;
            string temp, subFolder;
            List<string> tempList = new List<string>();

            foreach (string directory in directories)
            {
                if (string.IsNullOrEmpty(directory))
                    continue;

                temp = directory;

                index = temp.LastIndexOf('\"');
                if (0 <= index)
                    temp = temp.Substring(0, index);

                index = temp.IndexOf('\"');
                if (0 <= index)
                    temp = temp.Remove(index, 1);

                index = temp.IndexOf('\\');
                if (0 == index)
                    temp = temp.Remove(index, 1);

                index = temp.LastIndexOf('\\');
                if (index == temp.Length - 1)
                    temp = temp.Substring(0, index);

                subFolder = STIpath + temp;
                if (!Directory.Exists(subFolder))
                    continue;

                tempList.Add(subFolder);
            }

            return tempList.ToArray();
        }

        private bool GetEmbeddedDLL(string targetPath)
        {
            if (string.IsNullOrEmpty(targetPath))
                return false;

            string p_sResourceName = "GUI_Console.Lib.cygwin1.dll";
            System.IO.Stream oStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(p_sResourceName);
            if (oStream == null)
                return false;

            string destFile = targetPath;
            if (destFile.LastIndexOf('\\') < 0)
            {
                destFile += "\\";
            }
            destFile += "cygwin1.dll";
            SaveStreamToFile(destFile, oStream);
            return true;
        }

        private void SaveStreamToFile(string fileFullPath, System.IO.Stream stream)
        {
            if (stream == null || stream.Length == 0) 
                return;

            try
            {
                if (File.Exists(fileFullPath))
                {
                    File.Delete(fileFullPath);
                }

                // Create a FileStream object to write a stream to a file
                using (System.IO.FileStream fileStream = System.IO.File.Create(fileFullPath, (int)stream.Length))
                {
                    const int bufSize = 0x1000;
                    byte[] buf = new byte[bufSize];
                    int bytesRead = 0;
                    int bytesWritten = 0;

                    while ((bytesRead = stream.Read(buf, 0, bufSize)) > 0)
                    {
                        fileStream.Write(buf, 0, bytesRead);
                        bytesWritten += bytesRead;
                    }
                }
            }
            // Catch any exception if a folder cannot be accessed
            // e.g. due to security restriction
            catch (Exception ex)
            {
                // LOG
                LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);               
            }
        }

        private bool RemoveEmbeddedDLL(string targetPath)
        {
            if (string.IsNullOrEmpty(targetPath))
                return false;

            string destFile = targetPath;
            if (destFile.LastIndexOf('\\') < 0)
            {
                destFile += "\\";
            }
            destFile += "cygwin1.dll";

            bool bRet = true;
            try
            {
                if (File.Exists(destFile))
                    File.Delete(destFile);
            }
            // Catch any exception if a folder cannot be accessed
            // e.g. due to security restriction
            catch (Exception ex)
            {
                // LOG
                LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);               
                bRet = false;
            }
            return bRet;
        }


        private void ExportToFTP(bool includeWindowsEvents)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("---------Start Exporting to FTP. Time:{0}", DateTime.Now.ToLongTimeString()));

            // show connectin FTP server message
            string title = LanguageINI.GetString("headerConnectingFTPserver");
            string msg = LanguageINI.GetString("msgConnectingFTPserver");
            FTPMsg = new RoboMessagePanel5(RoboSep_UserConsole.getInstance(), title, msg, GUI_Controls.GifAnimationMode.eConnecting);
            FTPMsg.Show();
            FTPMsg.Refresh();

            // Disable the buttons
            UpdateButtons(false);

            // Use background thread for connecting to FTP server
            BackgroundWorker bwWorkerConnectFTP = new BackgroundWorker();

            bwWorkerConnectFTP.WorkerSupportsCancellation = true;
            DoWorkEventHandler evhConnectFTPServer = new DoWorkEventHandler(bwWorkerConnectFTP_DoWork);
            RunWorkerCompletedEventHandler evhConnectFTPServerCompleted = new RunWorkerCompletedEventHandler(bwWorkerConnectFTP_RunWorkerCompleted);

            // Attach the event handlers
            bwWorkerConnectFTP.DoWork += evhConnectFTPServer;
            bwWorkerConnectFTP.RunWorkerCompleted += evhConnectFTPServerCompleted;

            // Kick off the Async thread
            bwWorkerConnectFTP.RunWorkerAsync(includeWindowsEvents);
        }

        // function to connect to FTP server
        private void bwWorkerConnectFTP_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            bool includeWindowsEvents = (bool)e.Argument;

            try
            {
                // Establish connection to FTP server
                ConnectToFTPServer();

                // Update message to zipping diagnostic files
                this.Invoke(
                    (MethodInvoker)delegate()
                    {
                        UpdateFTPMsgToZippingFile();
                    }
                );

                string[] aFileName = new string[2];
                aFileName[0] = String.Empty;
                aFileName[1] = String.Empty;

                // Zipping files
                DoZippingRoboSepLogsPackage(aFileName, includeWindowsEvents);

                // Update message to uploading file to FTP
                this.Invoke(
                    (MethodInvoker)delegate()
                    {
                        UpdateFTPMsgToUploadingFile();
                    }
                );

                // Uploading file
                UpLoadingFile(aFileName);

                // delete temp folder 
                Utilities.RemoveTempFileDirectory(aFileName[1]);
            }
            catch (Exception ex)
            {
                e.Cancel = true;

                this.Invoke(
                    (MethodInvoker)delegate()
                    {
                        CloseFTPMsg();
                    }
                );

                // LOG
                LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);               

                string sHeader = LanguageINI.GetString("headerFailedToConnectFTP");
                string sMsg= LanguageINI.GetString("msgFailedToConnectFTP");
                string sOK = LanguageINI.GetString("Ok");
                GUI_Controls.RoboMessagePanel prompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_ERROR, sMsg, sHeader, sOK);
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                RoboSep_UserConsole.hideOverlay();
                prompt.Dispose();
               return;
            }
        }

        private void bwWorkerConnectFTP_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Invoke(
                (MethodInvoker)delegate()
                {
                    CloseFTPMsg();

                    DisconnectFTPServer();

                    if (!e.Cancelled && e.Error == null)
                    {
                        ShowFTPSendMsg();
                    }
                    UpdateButtons(true);
                }
            );
        }

        private void CloseFTPMsg()
        {
            if (FTPMsg != null)
            {
                FTPMsg.Close();
                RoboSep_UserConsole.hideOverlay();
                FTPMsg = null;
            }
        }

        private void ShowFTPSendMsg()
        {
            string sHeader = LanguageINI.GetString("headerDiagnosticPackageSendToFTP");
            string sMsg = LanguageINI.GetString("msgDiagnosticPackageSendToFTPOK");
            string sOK = LanguageINI.GetString("Ok");
            GUI_Controls.RoboMessagePanel prompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_INFORMATION, sMsg, sHeader, sOK);
            RoboSep_UserConsole.showOverlay();
            prompt.ShowDialog();
            RoboSep_UserConsole.hideOverlay();
            prompt.Dispose();
        }

        private void UpdateFTPMsgToZippingFile()
        {
            if (FTPMsg != null)
            {
                // show zipping message
                string title = LanguageINI.GetString("headerZipping");
                string msg = LanguageINI.GetString("msgWaitForZip");
                FTPMsg.SetMessageInfo(title, msg, GUI_Controls.GifAnimationMode.eExportSingleFile);
             }
        }

        private void UpdateFTPMsgToUploadingFile()
        {
            if (FTPMsg != null)
            {
                // show zipping message
                string title = LanguageINI.GetString("headerExportingDiagPackage");
                string msg = LanguageINI.GetString("msgWaitExportingDiagPackage");
                FTPMsg.SetMessageInfo(title, msg, GUI_Controls.GifAnimationMode.eExportSingleFile);
             }
        }

        private void UpLoadingFile(string[] aFileNames)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("---------Start uploading diagnostic file to FTP. Time:{0}", DateTime.Now.ToLongTimeString()));

            string compressedFileName = aFileNames[0];
            string tempZipPath = aFileNames[1];

            if (string.IsNullOrEmpty(compressedFileName) || string.IsNullOrEmpty(tempZipPath))
                return;

            string fullPathFileName = String.Format("{0}\\{1}\\{2}", tempZipPath, diagnosticPackageTempSubFolderName,compressedFileName);
            UploadFileToFTPServer(fullPathFileName);
            
            System.Diagnostics.Debug.WriteLine(String.Format("---------Uploading diagnostic file to FTP completed. Time:{0}", DateTime.Now.ToLongTimeString()));
        }

        // FTP - connect to FTP server
        private bool ConnectToFTPServer()
        {
            string sIp = GUIini.IniReadValue("FTP", "Server Name/IP Address", "54.208.155.239");
            string sUser = GUIini.IniReadValue("FTP", "LoginID", "roboseptechsupport");
            string sPassword = GUIini.IniReadValue("FTP", "Password", "password");

            try
            {
                // decrypt password
                sPassword = Utilities.DecryptString(sPassword, TextPassPhase);
            }
            catch (Exception ex)
            {
                // LOG
                LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);               

                string sHeader = LanguageINI.GetString("headerFailedToDecryptPassword");
                string sMsg = LanguageINI.GetString("msgFailedToDecryptPassword");
                string sOK = LanguageINI.GetString("Ok");
                GUI_Controls.RoboMessagePanel prompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_ERROR, sMsg, sHeader, sOK);
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                RoboSep_UserConsole.hideOverlay();
                prompt.Dispose();
                return false;
            }

            string sPort = GUIini.IniReadValue("FTP", "Port", "21");
            int port = 0;
            if (!int.TryParse(sPort.Trim(), out port))
            {
                return false;
            }

            if (he == null)
                he = new HE();

            System.Diagnostics.Debug.WriteLine(String.Format("---------Attempt to establish FTP connection. Time:{0}", DateTime.Now.ToLongTimeString()));

            using (new StopWatch("Connecting to FTP server"))
            {
                he.ConnectServer(sIp, sUser, sPassword, port);
            }

            System.Diagnostics.Debug.WriteLine(String.Format("---------Establish FTP connection successfully. Time:{0}", DateTime.Now.ToLongTimeString()));
            return true;
        }

        // FTP - upload file to FTP server
        private bool UploadFileToFTPServer(string fullPathFileName)
        {
            if (string.IsNullOrEmpty(fullPathFileName) || he == null)
                return false;

            using (new StopWatch("Uploading diagnostic file to FTP server"))
            {
                he.SetSourcePath(fullPathFileName);
                he.UploadToFTP();
            }
            return true;
        }

        // FTP - connect to FTP server
        private void DisconnectFTPServer()
        {
            if (he == null)
                return;
            try
            {
                he.DisconnectServer();
            }
            catch (Exception ex)
            {
                // LOG
                LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);               

                string sHeader = LanguageINI.GetString("headerFailedToDisconnectFTP");
                string sMsg = LanguageINI.GetString("msgFailedToDisconnectFTP");
                string sOK = LanguageINI.GetString("Ok");
                GUI_Controls.RoboMessagePanel prompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_ERROR, sMsg, sHeader, sOK);
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                RoboSep_UserConsole.hideOverlay();
                prompt.Dispose();
                return;
            }
        }

        private void RoboSep_System_Enter(object sender, EventArgs e)
        {
            // monitor USB 
            // InitializeUSBWatchers();
        }


        private void RoboSep_System_Leave(object sender, EventArgs e)
        {
            StopWatchUSB();
        }

        private void RoboSep_System_Load(object sender, EventArgs e)
        {
            this.SuspendLayout();
            // set label and button text based on language setting
            LanguageINI = RoboSep_UserConsole.getInstance().LanguageINI;
            label_keyboard.Text = LanguageINI.GetString("lblKeyboard");
            label_lidSensor.Text = LanguageINI.GetString("lblLidSensor");
            label_liquidSensor.Text = LanguageINI.GetString("lblLiquidSensor");
            label_StartTimer.Text = LanguageINI.GetString("lblStartTimer");
            Button_BasicPrime.Text = LanguageINI.GetString("lblBasicPrime");
            button_Home.Text = LanguageINI.GetString("lblHomeAll");
            lblServiceMenu.Text = LanguageINI.GetString("lblServiceButton");
            lblDiagnosticPackage.Text = LanguageINI.GetString("lblDiagnostic");
            lblSoftwareShutdown.Text = LanguageINI.GetString("lblExitToDeskTop");
            //RefreshSettings();
            this.ResumeLayout();

            bInitialized = true;
        }

        public bool IsInitialized
        {
            get
            {
                return bInitialized;        
            }
        }

        public void RefreshSettings(bool bCheckBoxKeyboard, bool bCheckBoxLidSensor, bool bCheckBoxLiquidSensor)
        {
            checkBox_keyboard.Check = bCheckBoxKeyboard;
            checkBox_lidSensor.Check = bCheckBoxLidSensor;
            checkBox_liquidSensor.Check = bCheckBoxLiquidSensor;
        }
        private void ExitSoftwareTimer_Tick(object sender, EventArgs e)
        {
            if (RoboSep_UserConsole.getInstance().Opacity > 0.0F)
            {
                RoboSep_UserConsole.getInstance().Opacity -= 0.05F;
            }
            else
            {
                ExitSoftwareTimer.Stop();
                
                ISeparator mySeparator = Tesla.OperatorConsoleControls.SeparatorGateway.GetInstance().ControlApi;
                mySeparator.CloseOrcaLogFileSystem();

                Application.Exit();
            }
        }

        private void button_Logs_Click(object sender, EventArgs e)
        {
            /*// open Run logs window
            RoboSep_Logs.getInstance().Visible = false;
            RoboSep_UserConsole.getInstance().Controls.Add(RoboSep_Logs.getInstance());
            RoboSep_UserConsole.getInstance().Controls.Remove(this);
            RoboSep_Logs.getInstance().Visible = true;
            RoboSep_UserConsole.ctrlCurrentUserControl = RoboSep_Logs.getInstance();*/
            launchVideoLogsPage();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F1))
            {
                LaunchProtocolEditor();
                return true;
            }
            else if (keyData == (Keys.Control | Keys.F2))
            {
                LaunchServiceUtility();
                return true;
            }
            else if (keyData == (Keys.Control | Keys.F10))
            {
                launchVideoLogsPage();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void launchVideoLogsPage()
        {
            RoboSep_VideoLogs.getInstance().Visible = false;
            RoboSep_UserConsole.getInstance().Controls.Add(RoboSep_VideoLogs.getInstance());
            RoboSep_UserConsole.getInstance().Controls.Remove(this);
            RoboSep_VideoLogs.getInstance().Visible = true;
            RoboSep_UserConsole.ctrlCurrentUserControl = RoboSep_VideoLogs.getInstance();
        }

        private void LaunchProtocolEditor()
        {
            // see if protocol editor directory exist
            string systemPath = Utilities.GetRoboSepSTIpath();
            string ExecutableDir = systemPath + "RoboSepEditor\\bin";
            if (!Directory.Exists(ExecutableDir))
                return;

            //Package files and directories here.
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = ExecutableDir + "\\ProtocolEditor.exe";
            process.StartInfo.WorkingDirectory = ExecutableDir;
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            process.Start();
        }

        private void LaunchServiceUtility()
        {
            string logMsg;
            string sUtilityFilenameToken = GUIini.IniReadValue("General", "DefaultUtilityFilenames", DefaultUtilityFilenames);
            if (string.IsNullOrEmpty(sUtilityFilenameToken))
            {
                logMsg = String.Format("Failed to launch utility. It is not configured.");
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Warning, logMsg);
                return;
            }

            string systemPath = Utilities.GetRoboSepSTIpath();
            string ExecutableDir = systemPath + "RoboSep\\bin";
            string FullPathFileName = ExecutableDir + "\\" + sUtilityFilenameToken;

            // see if service utility exists
            if (!File.Exists(FullPathFileName))
            {
                logMsg = String.Format("Failed to launch utility '{0}'. It does not exist in directory '{1}.", sUtilityFilenameToken, ExecutableDir);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Warning, logMsg);
                return;
            }

            if (!ValidatePassword())
            {
                logMsg = String.Format("Failed to launch utility '{0}'. Password is not correct.", sUtilityFilenameToken);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Warning, logMsg);
                return;
             }

            this.SendToBack();

            KillUtility(sUtilityFilenameToken);

            string sTimeout = GUIini.IniReadValue("General", "DefaultUtilityTimeoutInSecond", MinUtilityTimeoutInSeconds.ToString());
    
            int nTimeOutInSec = 0;
            if (string.IsNullOrEmpty(sTimeout))
            {
                nTimeOutInSec = MinUtilityTimeoutInSeconds;
            }
            else
            {
                if (!int.TryParse(sTimeout.Trim(), out nTimeOutInSec))
                {
                    logMsg = String.Format("Failed to launch utility '{0}'. Failed to parse {1} to integer.", sUtilityFilenameToken, sTimeout);
                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Warning, logMsg);
                    return;
                }
            }

            // Check ranges
            if (nTimeOutInSec < MinUtilityTimeoutInSeconds)
                nTimeOutInSec = MinUtilityTimeoutInSeconds;

            // Launch utility 
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = FullPathFileName;
            process.StartInfo.WorkingDirectory = ExecutableDir;
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();

            // Wait for it to return
            if (process.WaitForExit(nTimeOutInSec * 1000))   // convert to msec
            {
                DialogResult nExitCode = (DialogResult)process.ExitCode;
                if (nExitCode == DialogResult.OK)    // something has been modified
                {
                    // Update About box 
                    RoboSep_About.getInstance().PopulateInfoFromGuiINIFile();
                }
            }
            else
            {
                // terminate utility
                KillUtility(sUtilityFilenameToken);

                // prompt user that warranty will soon expire
                string sTemp = LanguageINI.GetString("msgUtilityTimeout");
                string sMsg = String.Format(sTemp, sUtilityFilenameToken, nTimeOutInSec);
                RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING, sMsg,
                    LanguageINI.GetString("headerUtilityTimeout"), LanguageINI.GetString("Ok"));
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                RoboSep_UserConsole.hideOverlay();
                prompt.Dispose();
            }
            this.BringToFront();
        }

        private void KillUtility(string sUtilityFilename)
        {
            if (string.IsNullOrEmpty(sUtilityFilename))
                return;

            try
            {
                string sFileName = sUtilityFilename;
                int nindex = sUtilityFilename.LastIndexOf('.');
                if (0 < nindex)
                {
                    sFileName = sUtilityFilename.Substring(0, nindex);
                }

                System.Diagnostics.Process[] procs = System.Diagnostics.Process.GetProcessesByName(sFileName);
                if (procs != null)
                {
                    foreach (System.Diagnostics.Process proc in procs)
                    {
                        proc.Kill();
                    }
                }
            }
            catch (Exception ex)
            {
                // LOG
                LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);               
            }
        }

        private void InitializeUSBWatchers()
        {
            if (bUSBWatcherInitialized)
                return;

            myUSBDeviceRemovedWatcher = new ManagementEventWatcher(USBRemovedWatcherQuery);
            myUSBDeviceRemovedWatcher.EventArrived += (object sender, EventArrivedEventArgs e) =>
            {
                ManagementBaseObject targetInstance = e.NewEvent["TargetInstance"] as ManagementBaseObject;

                if (!IsRemovableDisk(targetInstance))
                    return;

                RoboSep_System pSystem = RoboSep_System.getInstance();
                if (pSystem != null)
                {
                    if (pSystem.InvokeRequired)
                    {
                        this.Invoke(
                            (MethodInvoker)delegate()
                            {
                                pSystem.CancelSearchUtilityBackgroundThread();
                            }
                         );
                    }
                    else
                    {
                        pSystem.CancelSearchUtilityBackgroundThread();
                    }
                }
            };

            myUSBDeviceRemovedWatcher.Start();

            myUSBDeviceAddedWatcher = new ManagementEventWatcher(USBAddedWatcherQuery);
            myUSBDeviceAddedWatcher.EventArrived += (object sender, EventArrivedEventArgs e) =>
            {
                ManagementBaseObject targetInstance = e.NewEvent["TargetInstance"] as ManagementBaseObject;

                if (!IsRemovableDisk(targetInstance))
                    return;

                string drive = targetInstance[CaptionFieldName] as string;

                // Kick off the Async thread
                myAsyncWorker.RunWorkerAsync(drive);
            };

            myUSBDeviceAddedWatcher.Start();

            myAsyncWorker.WorkerSupportsCancellation = true;
            evhUSBDeviceAdded = new DoWorkEventHandler(bwUSBDeviceAdded_DoWork);
            evhUSBDeviceAddedCompleted = new RunWorkerCompletedEventHandler(bwUSBDeviceAdded_RunWorkerCompleted);

            // Attach the event handlers
            myAsyncWorker.DoWork += evhUSBDeviceAdded;
            myAsyncWorker.RunWorkerCompleted += evhUSBDeviceAddedCompleted;

            bUSBWatcherInitialized = true;
        }

        private void bwUSBDeviceAdded_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            string drive = e.Argument as string;
            if (string.IsNullOrEmpty(drive))
                return;

            List<string> lstFileNames = new List<string>();
            if (!bw.CancellationPending)
            {
                string sUtilityFilenameToken = GUIini.IniReadValue("General", "DefaultUtilityFilenames", DefaultUtilityFilenames);
                if (string.IsNullOrEmpty(sUtilityFilenameToken))
                    return;

                string[] sUtilityFilenames = sUtilityFilenameToken.Split(FileSeparator);
                if (sUtilityFilenames == null || sUtilityFilenames.Length == 0)
                    return;

                try
                {
                    // Iterate through all the files
                    foreach (string sFileName in sUtilityFilenames)
                    {
                        if (string.IsNullOrEmpty(sFileName))
                            continue;

                        SearchFile(bw, e.Cancel, drive, sFileName.Trim(), lstFileNames);
                    }

                }
                // Catch any exception if a folder cannot be accessed
                // e.g. due to security restriction
                catch (Exception)
                {
                    e.Cancel = true;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("User aborts utility file searching");
                Thread.Sleep(1200);
                e.Cancel = true;
            }
            e.Result = lstFileNames.ToArray();
        }

        private void bwUSBDeviceAdded_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Error == null)
            {
                string[] aFileNames = e.Result as string[];
                if (aFileNames.Length > 0)
                {
                    launchUtility(aFileNames);
                }
            }
        }
        public static bool SearchFile(BackgroundWorker bw, bool cancel, string searchFolderName, string searchFileName, List<string> lstFileName)
        {
            if (bw == null || string.IsNullOrEmpty(searchFolderName) || string.IsNullOrEmpty(searchFileName))
                return false;

            bool bFound = SearchFilesInDirectory(bw, cancel, searchFolderName, searchFileName, lstFileName);
            return bFound;
        }

        private static bool SearchFilesInDirectory(BackgroundWorker bw, bool cancel, string searchFolderName, string searchFileName, List<string> lstFileNames)
        {
            if (bw == null || string.IsNullOrEmpty(searchFolderName) || string.IsNullOrEmpty(searchFileName) || lstFileNames == null)
                return false;

            bool bFound = false;
            try
            {
                // base directory
                string[] fullPathfileNames = Directory.GetFiles(searchFolderName, searchFileName, SearchOption.TopDirectoryOnly);
                if (fullPathfileNames != null && fullPathfileNames.Length > 0)
                {
                    for (int i = 0; i < fullPathfileNames.Length; i++)
                    {
                        if (string.IsNullOrEmpty(fullPathfileNames[i]))
                            continue;

                        FileInfo fileInfo = new FileInfo(fullPathfileNames[i]);
                        if (fileInfo == null || fileInfo.Name.ToLower() != searchFileName.ToLower())
                            continue;

                        lstFileNames.Add(fullPathfileNames[i]);
                        bFound = true;
                    }
                }

                if (!bFound)
                {
                    // Loop to explore all the directories under pPath
                    foreach (string dirName in Directory.GetDirectories(searchFolderName))
                    {
                        if (!bw.CancellationPending)
                        {
                            // Access the file system to get the directory size
                            DirectoryInfo dirInfo = new DirectoryInfo(dirName);

                            bFound = SearchFilesInSubDirectory(bw, cancel, dirInfo, searchFileName, lstFileNames);
                            if (bFound || cancel)
                                return true;

                            foreach (string fileName in Directory.GetFiles(dirName))
                            {
                                if (!bw.CancellationPending)
                                {
                                    try
                                    {
                                        FileInfo pFileInfo = new FileInfo(fileName);
                                        if (pFileInfo == null || pFileInfo.Name.ToLower() != searchFolderName.ToLower())
                                            continue;

                                        lstFileNames.Add(pFileInfo.FullName);
                                        bFound = true;
                                        break;
                                    }
                                    // Catch any exception if a file cannot be accessed
                                    // e.g. due to security restriction
                                    catch (Exception)
                                    {
                                        bFound = false;
                                    }
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine("User aborts utility file searching");
                                    Thread.Sleep(1200);
                                    cancel = true;
                                }
                                if (bFound)
                                    break;
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("User aborts utility file searching");
                            Thread.Sleep(1200);
                            cancel = true;
                        }
                    }
                }
            }
            // Catch any exception if a folder cannot be accessed
            // e.g. due to security restriction
            catch (Exception)
            {
                bFound = false;
            }
            return bFound;
        }

        // A recursive method to search files in subdirectory 
        private static bool SearchFilesInSubDirectory(BackgroundWorker bw, bool cancel, DirectoryInfo pDirInfo, string searchFileName, List<string> lstFileNames)
        {
            if (bw == null || pDirInfo == null || string.IsNullOrEmpty(searchFileName) || lstFileNames == null)
                return false;

            bool bFound = false;
            FileInfo[] fileInfos = pDirInfo.GetFiles();
            foreach (FileInfo fileInfo in fileInfos)
            {
                if (!bw.CancellationPending)
                {
                    try
                    {
                        if (fileInfo == null || fileInfo.Name.ToLower() != searchFileName.ToLower())
                            continue;

                        lstFileNames.Add(fileInfo.FullName);
                        bFound = true;
                        break;
                    }

                    // Catch any exception if a file cannot be accessed
                    // e.g. due to security restriction
                    catch (Exception)
                    {
                        bFound = false;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("User aborts utility file searching");
                    Thread.Sleep(1200);
                    cancel = true;
                }
            }

            if (!bFound && !cancel)
            {
                DirectoryInfo[] dirInfos = pDirInfo.GetDirectories();
                foreach (DirectoryInfo dirInfo in dirInfos)
                {
                    if (!bw.CancellationPending)
                    {
                        try
                        {
                            bFound = SearchFilesInSubDirectory(bw, cancel, dirInfo, searchFileName, lstFileNames);
                            if (!bFound)
                                continue;

                            break;
                        }
                        // Catch any exception if a folder cannot be accessed
                        // e.g. due to security restriction
                        catch (Exception)
                        {
                            bFound = false;
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("User aborts utility file searching");
                        Thread.Sleep(1200);
                        cancel = true;
                    }
                }
            }
            return bFound;
        }

        private bool ValidatePassword()
        {
            string msgHeader = LanguageINI.GetString("headerPassword");
            string msg = LanguageINI.GetString("msgEnterPassword");

            RoboMessagePanel3 prompt = new RoboMessagePanel3(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WAIT, msg, msgHeader, LanguageINI.GetString("Ok"),
                LanguageINI.GetString("Cancel"), true, RoboSep_UserConsole.KeyboardEnabled, true);
            RoboSep_UserConsole.showOverlay();
            DialogResult result = prompt.ShowDialog();
            RoboSep_UserConsole.hideOverlay();

            if (prompt.DialogResult != DialogResult.OK)
            {
                prompt.Dispose();
                return false;
            }
            else
            {
                string sPassword = prompt.EditBoxText;
                if (sPassword == DefaultPassword)
                {
                    string logMSG = String.Format("Change default settings - User password is correct.");
                    //  (logMSG);
                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
                    prompt.Dispose();
                    return true;
                }
                else
                {
                    // LOG
                    string logMSG = String.Format("Change default settings - User password '{0}' is not correct.", sPassword);
                    //  (logMSG);
                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
                    RoboSep_UserConsole.hideOverlay();
                    prompt.Dispose();
                }
            }
            return ValidatePassword();
        }

        public bool IsUtilityBackgroundThreadBusy
        {
            get
            {
                return myAsyncWorker.IsBusy;
            }
        }

        public void CancelSearchUtilityBackgroundThread()
        {
            if (myAsyncWorker.IsBusy)
            {
                myAsyncWorker.CancelAsync();
            }
        }
        
        public void StopWatchUSB()
        {
            CancelSearchUtilityBackgroundThread();
            Thread.Sleep(100);
            if (myUSBDeviceRemovedWatcher != null)
            {
                myUSBDeviceRemovedWatcher.Stop();
                myUSBDeviceRemovedWatcher.Dispose();
                myUSBDeviceRemovedWatcher = null;
            }
            if (myUSBDeviceAddedWatcher != null)
            {
                myUSBDeviceAddedWatcher.Stop();
                myUSBDeviceAddedWatcher.Dispose();
                myUSBDeviceAddedWatcher = null;
            }

            bUSBWatcherInitialized = false;
        }

        private void launchUtility(string[] fullPathfileNames)
        {
            if (fullPathfileNames == null || fullPathfileNames.Length == 0)
                return;

            string sExecutable = String.Empty;
            if (fullPathfileNames.Length > 1)
            {
                Array.Sort(fullPathfileNames, new DirectoryComparer());
                List<string> lstDirectories = new List<string>();
                FileBrowser SelectFolder;
                DialogResult result;
                int nIndex;
                foreach (string sFullFileName in fullPathfileNames)
                {
                    if (string.IsNullOrEmpty(sFullFileName))
                        continue;

                    FileInfo fileInfo = new FileInfo(sFullFileName);

                    if (fileInfo == null)
                        continue;

                    nIndex = lstDirectories.FindIndex(x => { return (!string.IsNullOrEmpty(x) && x == fileInfo.DirectoryName); });
                    if (0 <= nIndex)
                        continue;

                    lstDirectories.Add(fileInfo.DirectoryName);

                    string sTitle = LanguageINI.GetString("headerLaunchUtility");
                    SelectFolder = new FileBrowser(RoboSep_UserConsole.getInstance(), sTitle, fileInfo.DirectoryName, fileInfo.Name,
                     new string[] { fileInfo.Extension }, FileBrowser.BrowseResult.SelectFile);
                    result = SelectFolder.ShowDialog();
                    if (result != DialogResult.OK)
                    {
                        SelectFolder.Dispose();
                        continue;
                    }
                    sExecutable = SelectFolder.Target;
                    SelectFolder.Dispose();
                    break;
                }

            }
            else if (fullPathfileNames.Length == 1)
            {
                sExecutable = fullPathfileNames[0];
            }

            if (string.IsNullOrEmpty(sExecutable))
                return;

            if (!ValidatePassword())
            {
                return;
            }

            string sExecutablePath = Utilities.GetRoboSepSysPath();
            sExecutablePath += "bin\\";

            // Launch utility file
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = sExecutable;
            process.StartInfo.WorkingDirectory = sExecutablePath;
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
        }

        private bool IsRemovableDisk(ManagementBaseObject instance)
        {
            return ((uint)instance["DriveType"] == (uint)DriveType.Removable);
        }
    }
 
    

}
