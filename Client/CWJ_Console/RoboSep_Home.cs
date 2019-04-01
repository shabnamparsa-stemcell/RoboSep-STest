using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Xml;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using GUI_Controls;
using Invetech.ApplicationLog;
using System.Threading;

namespace GUI_Console
{
    public partial class RoboSep_Home : UserControl
    {
        //public Point Offset = new Point(4, 26);
        public Point Offset = new Point(0, 0);
        private static RoboSep_Home myHome;
        RoboSep_UserPreferences myUserPreferences;
        private IniFile LanguageINI;
        const string SERVICE_EXE_FILENAME = "Service.exe";

        List<Image> IListRunSamples;
        List<Image> IListRunProgress;
        List<Image> IList1, IList2, IList3, IList4, IList5, IList6, IList7;

        private RoboSep_Home()
        {
            InitializeComponent();

            // LOG
            string logMSG = "Creating new Form_Home";
            //GUI_Controls.uiLog.LOG(this, "Form_Home", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);            

            this.SuspendLayout();

            IListRunProgress = new List<Image>();
            IListRunProgress.Add(Properties.Resources.GE_BTN24L_progress_STD);
            IListRunProgress.Add(Properties.Resources.GE_BTN24L_progress_OVER);
            IListRunProgress.Add(Properties.Resources.GE_BTN24L_progress_OVER);
            IListRunProgress.Add(Properties.Resources.GE_BTN24L_progress_CLICK);

            IListRunSamples = new List<Image>();
            IListRunSamples.Add(Properties.Resources.carousel_104x86_STD);
            IListRunSamples.Add(Properties.Resources.carousel_104x86_OVER);
            IListRunSamples.Add(Properties.Resources.carousel_104x86_OVER);
            IListRunSamples.Add(Properties.Resources.carousel_104x86_CLICK);
            hex_sampling.ChangeGraphics(IListRunSamples);
            hex_sampling.disableImage = Properties.Resources.GE_BTN22L_home_DISABLE;

            // close button, change graphics
            IList1 = new List<Image>();

            // Users button
            IList1.Add(Properties.Resources.L_104x86_Users_single_STD);
            IList1.Add(Properties.Resources.L_104x86_Users_single_OVER);
            IList1.Add(Properties.Resources.L_104x86_Users_single_OVER);
            IList1.Add(Properties.Resources.L_104x86_Users_single_CLICK);
            hex_Users.ChangeGraphics(IList1);
            hex_Users.disableImage = Properties.Resources.L_104x86_Users_single_DISABLE;

            // System Button, change graphics
            IList2 = new List<Image>();
            IList2.Add(Properties.Resources.GE_BTN16L_settings_STD);
            IList2.Add(Properties.Resources.GE_BTN16L_settings_OVER);
            IList2.Add(Properties.Resources.GE_BTN16L_settings_OVER);
            IList2.Add(Properties.Resources.GE_BTN16L_settings_CLICK);
            hex_system.ChangeGraphics(IList2);
            hex_system.disableImage = Properties.Resources.GE_BTN16L_settings_DISABLE;

            // System Button, change graphics
            IList3 = new List<Image>();
            IList3.Add(Properties.Resources.GE_BTN14L_preferences_STD);
            IList3.Add(Properties.Resources.GE_BTN14L_preferences_OVER);
            IList3.Add(Properties.Resources.GE_BTN14L_preferences_OVER);
            IList3.Add(Properties.Resources.GE_BTN14L_preferences_CLICK);
            hex_UserPreferences.ChangeGraphics(IList3);
            hex_UserPreferences.disableImage = Properties.Resources.GE_BTN14L_preferences_DISABLE;

            // Reports Button, change graphics
            IList4 = new List<Image>();
            IList4.Add(Properties.Resources.GE_BTN17L_reports_STD);
            IList4.Add(Properties.Resources.GE_BTN17L_reports_OVER);
            IList4.Add(Properties.Resources.GE_BTN17L_reports_OVER);
            IList4.Add(Properties.Resources.GE_BTN17L_reports_CLICK);
            hex_logs.ChangeGraphics(IList4);

            // Help Button, change graphics
            IList5 = new List<Image>();
            IList5.Add(Properties.Resources.GE_BTN18L_help_STD);
            IList5.Add(Properties.Resources.GE_BTN18L_help_OVER);
            IList5.Add(Properties.Resources.GE_BTN18L_help_OVER);
            IList5.Add(Properties.Resources.GE_BTN18L_help_CLICK);
            hex_help.ChangeGraphics(IList5);

            // Protocols button, change graphics
            IList6 = new List<Image>();
            IList6.Add(Properties.Resources.GE_BTN23L_protocol_STD);
            IList6.Add(Properties.Resources.GE_BTN23L_protocol_OVER);
            IList6.Add(Properties.Resources.GE_BTN23L_protocol_OVER);
            IList6.Add(Properties.Resources.GE_BTN23L_protocol_CLICK);
            hex_protocols.ChangeGraphics(IList6);
            hex_protocols.disableImage = Properties.Resources.GE_BTN23L_protocol_DISABLE;

            // Shutdown Button, change graphics
            IList7 = new List<Image>();
            IList7.Add(Properties.Resources.GE_BTN19L_shut_down_STD);
            IList7.Add(Properties.Resources.GE_BTN19L_shut_down_OVER);
            IList7.Add(Properties.Resources.GE_BTN19L_shut_down_OVER);
            IList7.Add(Properties.Resources.GE_BTN19L_shut_down_CLICK);
            hex_shutdown.ChangeGraphics(IList7);
            hex_shutdown.disableImage = Properties.Resources.GE_BTN19L_shut_down_DISABLE;

            this.DoubleBuffered = true;
            this.ResumeLayout(false);
        }

        public static RoboSep_Home getInstance()
        {
            if (myHome == null)
            {
                myHome = new RoboSep_Home();
            }
            GC.Collect();
            return myHome;
        }

        private void Form_Home_Load(object sender, EventArgs e)
        {
            LanguageINI = GUI_Console.RoboSep_UserConsole.getInstance().LanguageINI;
            label_Settings.Text = LanguageINI.GetString("lblMaintenance");
            label_Protocols.Text = LanguageINI.GetString("lblProtocols");
            label_Preferences.Text = LanguageINI.GetString("lblPreferences");
            label_Reports.Text = LanguageINI.GetString("lblReports");
            label_Help.Text = LanguageINI.GetString("lblHelp");
            label_Shutdown.Text = LanguageINI.GetString("lblShutDown");

            label_Settings.Location = GetHorizontalLabelLocation(hex_system.Bounds, label_Settings.Bounds);
            label_Protocols.Location = GetHorizontalLabelLocation(hex_protocols.Bounds, label_Protocols.Bounds);
            label_Preferences.Location = GetHorizontalLabelLocation(hex_UserPreferences.Bounds, label_Preferences.Bounds);
            label_Reports.Location = GetHorizontalLabelLocation(hex_logs.Bounds, label_Reports.Bounds);
            label_Help.Location = GetHorizontalLabelLocation(hex_help.Bounds, label_Help.Bounds);
            label_Shutdown.Location = GetHorizontalLabelLocation(hex_shutdown.Bounds, label_Shutdown.Bounds);
            UpdateButtons();
        }

        public void UpdateButtons()
        {
            try
            {
                hex_UserPreferences.disable(RoboSep_UserConsole.bIsRunning);
                hex_system.disable(RoboSep_UserConsole.bIsRunning);
                hex_shutdown.disable(RoboSep_UserConsole.bIsRunning);
                hex_protocols.disable(RoboSep_UserConsole.bIsRunning);
                hex_Users.disable(RoboSep_UserConsole.bIsRunning);
                hex_sampling.ChangeGraphics(RoboSep_UserConsole.bIsRunning ? IListRunProgress : IListRunSamples);
                hex_sampling.disableImage = RoboSep_UserConsole.bIsRunning ? Properties.Resources.GE_BTN24L_progress_DISABLE : Properties.Resources.GE_BTN22L_home_DISABLE;
                label_runsamples.Text = RoboSep_UserConsole.bIsRunning ? LanguageINI.GetString("lblRunProgress") : LanguageINI.GetString("lblRunSamples");
                label_runsamples.Location = GetHorizontalLabelLocation(hex_sampling.Bounds, label_runsamples.Bounds);
            }
            catch (Exception ex)
            {
                // LOG
                string logMSG = String.Format("Failed to call UpdateButtons. Exception: {0}.", ex.Message);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);            
            }
        }

        private Point GetHorizontalLabelLocation(Rectangle rcButton, Rectangle rcLabel)
        {
            if (rcButton == null || rcLabel == null)
                throw new ArgumentNullException("Input parameter(s) cannot be null");

            Point location = new Point(rcLabel.X, rcLabel.Y);
            location.X = rcButton.X - ((rcLabel.Width - rcButton.Width) / 2);
            return location;
        }

        private void closeHomeWindow()
        {
            for (int i = 0; i < Application.OpenForms.Count; i++)
            {
                if (RoboSep_UserConsole.getInstance().frmHomeOverlay == Application.OpenForms[i])
                {
                    Application.OpenForms[i].Hide();
                }
            }
            GC.Collect();
            RoboSep_UserConsole.getInstance().Controls.Remove(this);
            RoboSep_UserConsole.getInstance().ResumeLayout();
            LogFile.AddMessage(TraceLevel.Verbose, "Hiding Home window");
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            RoboSep_UserConsole.getInstance().SuspendLayout();

            RoboSep_UserConsole UC = RoboSep_UserConsole.getInstance();
            UC.Controls.Add(RoboSep_UserConsole.ctrlCurrentUserControl);

            closeHomeWindow();

            // LOG
            string logMSG = "Close Window button clicked";
            //GUI_Controls.uiLog.LOG(this, "btn_close_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);            
        }

        public Point GetOffset()
        {
            return Offset;
        }

        private void Form_Home_Deactivate(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                RoboSep_UserConsole.getInstance().frmHomeOverlay.BringToFront();
            }
        }

        private void hex_sampling_Click(object sender, EventArgs e)
        {
            // LOG
            string logMSG = "Run Sample button clicked";

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                
            RoboSep_UserConsole.getInstance().SuspendLayout();

            closeHomeWindow();  
  
            // open sampling window unless in "run mode"
            if (!RoboSep_UserConsole.bIsRunning)
            {
                RoboSep_RunSamples.getInstance().Visible = false;
                RoboSep_RunSamples.getInstance().Location = new Point(0, 0);
                RoboSep_UserConsole.getInstance().Controls.Add(RoboSep_RunSamples.getInstance());
                RoboSep_UserConsole.ctrlCurrentUserControl = RoboSep_RunSamples.getInstance();
                RoboSep_RunSamples.getInstance().Visible = true;
                string UserName = RoboSep_RunSamples.getInstance().UserName;
                RoboSep_RunSamples.getInstance().UserName = RoboSep_UserConsole.strCurrentUser;
                bool bRefreshDisplayName = true;
                if (UserName != RoboSep_UserConsole.strCurrentUser)
                {
                    RoboSep_RunSamples.getInstance().LoadProtocolsMRU();
                    bRefreshDisplayName = false;
                }

                if (bRefreshDisplayName && RoboSep_RunSamples.getInstance().IsInitialized)
                {
                    RoboSep_RunSamples.getInstance().refreshSelectedProtocolsDisplayName();
                }

                RoboSep_RunSamples.getInstance().BringToFront();

                logMSG = "System running, opening RunProgress";
                LogFile.AddMessage(TraceLevel.Verbose, logMSG);
            }
            else
            {
                RoboSep_RunProgress.getInstance().Visible = false;
                RoboSep_RunProgress.getInstance().Location = new Point(0, 0);
                RoboSep_UserConsole.getInstance().Controls.Add(RoboSep_RunProgress.getInstance());
                RoboSep_UserConsole.ctrlCurrentUserControl = RoboSep_RunProgress.getInstance();
                RoboSep_RunProgress.getInstance().Visible = true;

                RoboSep_RunProgress.getInstance().BringToFront();
                logMSG = "Opening RunSamples page";
                LogFile.AddMessage(TraceLevel.Verbose, logMSG);
            }

        }

        private void hex_protocols_Click(object sender, EventArgs e)
        {
            // check if any protocols are selected
            int[] CurrentQuadrants = RoboSep_RunSamples.getInstance().iSelectedProtocols;
            if (CurrentQuadrants.Length > 0)
            {
                // prompt if user is sure they want to manage protocols if all protocol selections are removed
                string msg = LanguageINI.GetString("msgManageProtocols");
                RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING, msg,
                    LanguageINI.GetString("headerManageProtocols"), LanguageINI.GetString("Ok"), LanguageINI.GetString("Cancel"));
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                RoboSep_UserConsole.hideOverlay();

                if (prompt.DialogResult != DialogResult.OK)
                {
                    prompt.Dispose();
                    return;
                }
                prompt.Dispose();
            }

            // remove all currently selected protocols
            for (int i = 0; i < CurrentQuadrants.Length; i++)
                RoboSep_RunSamples.getInstance().CancelQuadrant(CurrentQuadrants[i]);

            RoboSep_UserConsole.getInstance().SuspendLayout();
            RoboSep_ProtocolList myProtocolsListPage = RoboSep_ProtocolList.getInstance();
            if (myProtocolsListPage.IsInitialized)
                myProtocolsListPage.ReInitialized();

            myProtocolsListPage.Location = new Point(0, 0);
            myProtocolsListPage.strUserName = RoboSep_UserConsole.strCurrentUser;
            RoboSep_UserConsole.getInstance().Controls.Add(myProtocolsListPage);
            RoboSep_UserConsole.ctrlCurrentUserControl = myProtocolsListPage;

            closeHomeWindow();

            myProtocolsListPage.BringToFront();
            // LOG
            string logMSG = "Protocols button clicked";

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                
        }

        private void hex_system_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(ShowSystemWindowHelper));
        }

        private void ShowSystemWindowHelper(object stateinfo)
        {
            bool bCheckBoxKeyboard = RoboSep_UserConsole.KeyboardEnabled;
            bool bCheckBoxLidSensor = !RoboSep_UserConsole.getInstance().GetIgnoreLidSensor();
            bool bCheckBoxLiquidSensor = !RoboSep_UserConsole.getInstance().GetIgnoreHydraulicSensor();

            ShowSystemWindowHelperInUiThread(bCheckBoxKeyboard, bCheckBoxLidSensor, bCheckBoxLiquidSensor);
        }

        private void ShowSystemWindowHelperInUiThread(bool bCheckBoxKeyboard, bool bCheckBoxLidSensor, bool bCheckBoxLiquidSensor)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate() { this.ShowSystemWindowHelperInUiThread(bCheckBoxKeyboard, bCheckBoxLidSensor, bCheckBoxLiquidSensor); });
            }
            else
            {

                RoboSep_UserConsole.getInstance().SuspendLayout();
                RoboSep_System SystemWindow = RoboSep_System.getInstance();
                SystemWindow.Visible = false;
                SystemWindow.Location = new Point(0, 0);
                RoboSep_UserConsole.getInstance().Controls.Add(SystemWindow);
                RoboSep_UserConsole.ctrlCurrentUserControl = SystemWindow;
                SystemWindow.RefreshSettings(bCheckBoxKeyboard, bCheckBoxLidSensor, bCheckBoxLiquidSensor);
                SystemWindow.Visible = true;
                closeHomeWindow();
                SystemWindow.BringToFront();
                SystemWindow.Focus();

                // LOG
                string logMSG = "System button clicked";
                //GUI_Controls.uiLog.LOG(this, "hex_system_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
            }
        }

        private void hex_UserPreferences_Click(object sender, EventArgs e)
        {
            RoboSep_UserConsole.getInstance().SuspendLayout();

            if (myUserPreferences != null)
            {
                myUserPreferences.Dispose();
                myUserPreferences = null;
            }

            myUserPreferences = new RoboSep_UserPreferences(RoboSep_UserConsole.strCurrentUser, false, true);
            myUserPreferences.Parent = this.Parent;
            myUserPreferences.ClosingUserPreferencesApp += new EventHandler(HandleClosingUserPreferencesApp);
 
            //myUserPreferences.Location = new Point(0, 0);
            // RoboSep_UserConsole.getInstance().Controls.Add(myUserPreferences);
            //   RoboSep_UserConsole.ctrlCurrentUserControl = myUserPreferences;
            myUserPreferences.Show();
            myUserPreferences.BringToFront();
            myUserPreferences.Focus();
            // LOG
            string logMSG = "User Preferences button clicked";

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                
        }

        private void hex_Users_Click(object sender, EventArgs e)
        {
            // check if any protocols are selected
            int[] CurrentQuadrants = RoboSep_RunSamples.getInstance().iSelectedProtocols;
            if (CurrentQuadrants.Length > 0)
            {
                // prompt if user is sure they want to switch users if
                // all protocol selections are removed
                string msg = LanguageINI.GetString("msgChangeUser");
                RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING, msg,
                    LanguageINI.GetString("headerChangeUsr"), LanguageINI.GetString("Ok"), LanguageINI.GetString("Cancel"));
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                RoboSep_UserConsole.hideOverlay();

                if (prompt.DialogResult != DialogResult.OK)
                {
                    prompt.Dispose();                
                    return;
                }
                prompt.Dispose();                
            }

            // remove all currently selected protocols
            for (int i = 0; i < CurrentQuadrants.Length; i++)
                RoboSep_RunSamples.getInstance().CancelQuadrant(CurrentQuadrants[i]);

            // change to robosep user select window
            RoboSep_UserConsole.getInstance().SuspendLayout();
            RoboSep_UserSelect usrSelect = new RoboSep_UserSelect();
            RoboSep_UserConsole.getInstance().Controls.Add(usrSelect);
            RoboSep_UserConsole.ctrlCurrentUserControl = usrSelect;

            usrSelect.BringToFront();
            usrSelect.Focus();
        }

        private void HandleClosingUserPreferencesApp(object sender, EventArgs e)
        {
            this.Visible = true;
            this.Show();
            this.BringToFront();

            if (myUserPreferences != null)
            {
                myUserPreferences.Dispose();
                myUserPreferences = null;
            }
        }

        private void hex_help_Click(object sender, EventArgs e)
        {
            RoboSep_UserConsole.getInstance().SuspendLayout();
            RoboSep_About about = null;
            switch (RoboSep_UserConsole.CurrentHelpTab)
            {
                case RoboSep_UserConsole.HelpTab.About:
                    about = RoboSep_About.getInstance();
                    about.Location = new Point(0, 0);
                    RoboSep_UserConsole.getInstance().Controls.Add(about);
                    RoboSep_UserConsole.ctrlCurrentUserControl = about;
                    break;
                case RoboSep_UserConsole.HelpTab.HelpVid:
                    break;
            }
                
            closeHomeWindow();

            if (about != null)
            {
                about.BringToFront();
                about.Focus();
            }

            // LOG
            string logMSG = "Help button clicked";
            //GUI_Controls.uiLog.LOG(this, "hex_help_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                
        }

        private void hex_service_Click(object sender, EventArgs e)
        {
            // LOG
            string logMSG = "Service button clicked";
            //GUI_Controls.uiLog.LOG(this, "hex_service_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                

            RoboSep_UserConsole.getInstance().SuspendLayout();
            closeHomeWindow();

            // see if Service target directory exist
            string systemPath = RoboSep_UserDB.getInstance().sysPath;
            string[] directories = systemPath.Split('\\');

            //string servicePath = string.Empty;
            //for (int i = 0; i < directories.Length-2; i++)
            //    servicePath += directories[i] + "\\";
            //servicePath += "RoboSepService\\bin\\";

            StringBuilder sPath = new StringBuilder();
            for (int i = 0; i < directories.Length - 2; i++)
            {
                sPath.Append(directories[i]);
                sPath.Append("\\");
            }
            sPath.Append("RoboSepService\\bin\\");

            string servicePath = sPath.ToString();

            // check if service directory exists
            if (!System.IO.Directory.Exists(servicePath))
                System.IO.Directory.CreateDirectory(servicePath);

            // if Service.exe exists, run service.exe
            if (System.IO.File.Exists(servicePath + SERVICE_EXE_FILENAME))
            {
                // run Service
                ProcessStartInfo ServiceStartInfo = new ProcessStartInfo();
                ServiceStartInfo.WorkingDirectory = servicePath;
                ServiceStartInfo.FileName = SERVICE_EXE_FILENAME;
                ServiceStartInfo.WindowStyle = ProcessWindowStyle.Normal;

                Process ServiceProgram = new Process();
                ServiceProgram.StartInfo = ServiceStartInfo;
                ServiceProgram.Start();

                LogFile.AddMessage(TraceLevel.Warning, "Starting Service program");
            }
            else
            {
                logMSG = "Could not locate Service Program at '" + servicePath + SERVICE_EXE_FILENAME + "'";
                LogFile.AddMessage(TraceLevel.Warning,logMSG);
            }
            
        }

        private void hex_shutdown_Click(object sender, EventArgs e)
        {
            // LOG
            string logMSG = "SHUTDOWN button clicked";
            //GUI_Controls.uiLog.LOG(this, "hex_shutdown_Click", GUI_Controls.uiLog.LogLevel.GENERAL, logMSG);

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                
            // set up msg prompt
            string sMSG = LanguageINI.GetString("msgShutDown");
            GUI_Controls.RoboMessagePanel newPrompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING, sMSG, 
                LanguageINI.GetString("UserShutdown"), LanguageINI.GetString("Ok"), LanguageINI.GetString("Cancel"));
            RoboSep_UserConsole.getInstance().addForm(newPrompt, newPrompt.Offset);
            
            // prompt user
            newPrompt.ShowDialog();
            if (newPrompt.DialogResult == DialogResult.OK)
            {
                this.Hide();
                RoboSep_UserConsole.getInstance().frmHomeOverlay.Hide();
                // shut down robosep system
                logMSG = "User Shutdown Confirmed";
                LogFile.AddMessage(TraceLevel.Info, logMSG);

                RoboSep_UserConsole.getInstance().InvokeShutdownAction();
            }
            newPrompt.Dispose();
        }

        private void hex_logs_Click(object sender, EventArgs e)
        {
            // refresh run log content
            RoboSep_Reports.RefreshList();

            RoboSep_UserConsole.getInstance().SuspendLayout();
            RoboSep_Reports reportsWindow = RoboSep_Reports.getInstance();
            reportsWindow.Location = new Point(0, 0);
            RoboSep_UserConsole.getInstance().Controls.Add(reportsWindow);
            RoboSep_UserConsole.ctrlCurrentUserControl = reportsWindow;
            closeHomeWindow();

            reportsWindow.BringToFront();

            // LOG
            string logMSG = "Logs button clicked";
            //GUI_Controls.uiLog.LOG(this, "hex_logs_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                
        }

        private void button_UserSelect_Click(object sender, EventArgs e)
        {
            // clear current run samples page
            ClearRunSamples();
            RoboSep_UserConsole.getInstance().SuspendLayout();
            RoboSep_UserSelect usrSelect = new RoboSep_UserSelect();
            usrSelect.Location = new Point(0, 0);
            RoboSep_UserConsole.getInstance().Controls.Add(usrSelect);
            RoboSep_UserConsole.ctrlCurrentUserControl = usrSelect;
            closeHomeWindow();

            // LOG
            string logMSG = "User Selection button clicked";
            //GUI_Controls.uiLog.LOG(this, "hex_logs_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                
        }

        private void ClearRunSamples()
        {
            RoboSep_RunSamples RS = RoboSep_RunSamples.getInstance();
            for (int i = 0; i < 4; i++)
            {
                if (RS.RunInfo[i].QuadrantLabel != string.Empty)
                    RS.CancelQuadrant(i);
            }
        }



        
    }
}
