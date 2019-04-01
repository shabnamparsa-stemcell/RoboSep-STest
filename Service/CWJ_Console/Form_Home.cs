using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Xml;
using System.Text;
using System.Windows.Forms;

namespace GUI_Console
{
    public partial class Form_Home : Form
    {
        //public Point Offset = new Point(4, 26);
        public Point Offset = new Point(0, 0);
        private static Form_Home myHome;

        private Form_Home()
        {
            InitializeComponent();

            // LOG
            string logMSG = "Creating new Form_Home";
            GUI_Controls.uiLog.LOG(this, "Form_Home", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

            this.SuspendLayout();
            // close button, change graphics
            List<Image> IList = new List<Image>();
            IList.Add(GUI_Controls.Properties.Resources.BUTTON_HOMECANCEL0);
            IList.Add(GUI_Controls.Properties.Resources.BUTTON_HOMECANCEL0);
            IList.Add(GUI_Controls.Properties.Resources.BUTTON_HOMECANCEL0);
            IList.Add(GUI_Controls.Properties.Resources.BUTTON_HOMECANCEL1);
            btn_close.ChangeGraphics(IList);

            // Protocols button, change graphics
            IList.Clear();
            IList.Add(GUI_Controls.Properties.Resources.HEX_PROTOCOLS0);
            IList.Add(GUI_Controls.Properties.Resources.HEX_PROTOCOLS1);
            IList.Add(GUI_Controls.Properties.Resources.HEX_PROTOCOLS2);
            IList.Add(GUI_Controls.Properties.Resources.HEX_PROTOCOLS3);
            hex_protocols.ChangeGraphics(IList);

            // System Button, change graphics
            IList.Clear();
            IList.Add(GUI_Controls.Properties.Resources.HEX_SYSTEM0);
            IList.Add(GUI_Controls.Properties.Resources.HEX_SYSTEM1);
            IList.Add(GUI_Controls.Properties.Resources.HEX_SYSTEM2);
            IList.Add(GUI_Controls.Properties.Resources.HEX_SYSTEM3);
            hex_system.ChangeGraphics(IList);

            // Help Button, change graphics
            IList.Clear();
            IList.Add(GUI_Controls.Properties.Resources.HEX_HELP0);
            IList.Add(GUI_Controls.Properties.Resources.HEX_HELP1);
            IList.Add(GUI_Controls.Properties.Resources.HEX_HELP2);
            IList.Add(GUI_Controls.Properties.Resources.HEX_HELP3);
            hex_help.ChangeGraphics(IList);

            // Service Button, change graphics
            IList.Clear();
            IList.Add(GUI_Controls.Properties.Resources.HEX_SERVICE0);
            IList.Add(GUI_Controls.Properties.Resources.HEX_SERVICE1);
            IList.Add(GUI_Controls.Properties.Resources.HEX_SERVICE2);
            IList.Add(GUI_Controls.Properties.Resources.HEX_SERVICE3);
            hex_service.ChangeGraphics(IList);

            // Shutdown Button, change graphics
            IList.Clear();
            IList.Add(GUI_Controls.Properties.Resources.HEX_SHUTDOWN0);
            IList.Add(GUI_Controls.Properties.Resources.HEX_SHUTDOWN1);
            IList.Add(GUI_Controls.Properties.Resources.HEX_SHUTDOWN2);
            IList.Add(GUI_Controls.Properties.Resources.HEX_SHUTDOWN3);
            hex_shutdown.ChangeGraphics(IList);

            // Logs Button, change graphics
            IList.Clear();
            IList.Add(GUI_Controls.Properties.Resources.HEX_LOG0);
            IList.Add(GUI_Controls.Properties.Resources.HEX_LOG1);
            IList.Add(GUI_Controls.Properties.Resources.HEX_LOG2);
            IList.Add(GUI_Controls.Properties.Resources.HEX_LOG3);
            hex_logs.ChangeGraphics(IList);

            this.ResumeLayout(false);
        }

        public static Form_Home getInstance()
        {
            if (myHome == null)
            {
                myHome = new Form_Home();
            }
            return myHome;
        }

        private void button_Circle1_Load(object sender, EventArgs e)
        {
            
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
            this.Hide();
            RoboSep_UserConsole.getInstance().Activate();
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            closeHomeWindow();
            
            // LOG
            string logMSG = "Close Window button clicked";
            GUI_Controls.uiLog.LOG(this, "btn_close_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
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
                this.BringToFront();
                this.Activate();
            }
        }

        private void hex_sampling_Click(object sender, EventArgs e)
        {
            // open sampling window unless in "run mode"
            if (!RoboSep_UserConsole.bIsRunning)
            {
                if (RoboSep_UserConsole.ctrlCurrentUserControl != RoboSep_RunSamples.getInstance())
                {
                    RoboSep_RunSamples.getInstance().Visible = false;
                    RoboSep_RunSamples.getInstance().Location = new Point(0, 0);
                    RoboSep_UserConsole.getInstance().Controls.Add(RoboSep_RunSamples.getInstance());
                    RoboSep_UserConsole.getInstance().Controls.Remove(RoboSep_UserConsole.ctrlCurrentUserControl);
                    RoboSep_UserConsole.ctrlCurrentUserControl = RoboSep_RunSamples.getInstance();
                    RoboSep_RunSamples.getInstance().Visible = true;
                }
            }
            else
            {
                if (RoboSep_UserConsole.ctrlCurrentUserControl != RoboSep_RunProgress.getInstance())
                {
                    RoboSep_RunProgress.getInstance().Visible = false;
                    RoboSep_RunProgress.getInstance().Location = new Point(0, 0);
                    RoboSep_UserConsole.getInstance().Controls.Add(RoboSep_RunProgress.getInstance());
                    RoboSep_UserConsole.getInstance().Controls.Remove(RoboSep_UserConsole.ctrlCurrentUserControl);
                    RoboSep_UserConsole.ctrlCurrentUserControl = RoboSep_RunProgress.getInstance();
                    RoboSep_RunProgress.getInstance().Visible = true;
                }
            }
            closeHomeWindow();
            
            // LOG
            string logMSG = "Run Sample button clicked";
            GUI_Controls.uiLog.LOG(this, "hex_sampling_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        private void hex_system_Click(object sender, EventArgs e)
        {
            if (RoboSep_UserConsole.ctrlCurrentUserControl != RoboSep_System.getInstance())
            {
                RoboSep_System SystemWindow = RoboSep_System.getInstance();
                SystemWindow.Visible = false;
                SystemWindow.Location = new Point(0, 0);
                RoboSep_UserConsole.getInstance().Controls.Add(SystemWindow);
                RoboSep_UserConsole.getInstance().Controls.Remove(RoboSep_UserConsole.ctrlCurrentUserControl);
                RoboSep_UserConsole.ctrlCurrentUserControl = SystemWindow;
                SystemWindow.Visible = true;
            }
            closeHomeWindow();

            // LOG
            string logMSG = "System button clicked";
            GUI_Controls.uiLog.LOG(this, "hex_system_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        private void hex_protocols_Click(object sender, EventArgs e)
        {
            if (RoboSep_UserConsole.ctrlCurrentUserControl != RoboSep_Protocols.getInstance())
            {
                //RoboSep_Protocols ProtocolWindow = RoboSep_Protocols.getInstance();
                RoboSep_Protocols.getInstance().Location = new Point(0, 0);
                RoboSep_UserConsole.getInstance().Controls.Add(RoboSep_Protocols.getInstance());
                RoboSep_UserConsole.getInstance().Controls.Remove(RoboSep_UserConsole.ctrlCurrentUserControl);
                RoboSep_UserConsole.ctrlCurrentUserControl = RoboSep_Protocols.getInstance();
            }
            closeHomeWindow();

            // LOG
            string logMSG = "Protocols button clicked";
            GUI_Controls.uiLog.LOG(this, "hex_protocols_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        private void hex_help_Click(object sender, EventArgs e)
        {
            if (RoboSep_UserConsole.ctrlCurrentUserControl != RoboSep_Help.getInstance())
            {
                RoboSep_Help HelpWindow = RoboSep_Help.getInstance();
                HelpWindow.Location = new Point(0, 0);
                RoboSep_UserConsole.getInstance().Controls.Add(HelpWindow);
                RoboSep_UserConsole.getInstance().Controls.Remove(RoboSep_UserConsole.ctrlCurrentUserControl);
                RoboSep_UserConsole.ctrlCurrentUserControl = HelpWindow;
            }
            closeHomeWindow();

            // LOG
            string logMSG = "Help button clicked";
            GUI_Controls.uiLog.LOG(this, "hex_help_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        private void hex_service_Click(object sender, EventArgs e)
        {
            RoboSep_ServiceLogin serviceLoginWindow = new RoboSep_ServiceLogin();
            serviceLoginWindow.Location = new Point(0, 0);
            RoboSep_UserConsole.getInstance().Controls.Add(serviceLoginWindow);
            RoboSep_UserConsole.getInstance().Controls.Remove(RoboSep_UserConsole.ctrlCurrentUserControl);
            RoboSep_UserConsole.ctrlCurrentUserControl = serviceLoginWindow;
            closeHomeWindow();

            // LOG
            string logMSG = "Service button clicked";
            GUI_Controls.uiLog.LOG(this, "hex_service_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        private void hex_shutdown_Click(object sender, EventArgs e)
        {
            string sMSG = GUI_Controls.fromINI.getValue("GUI", "sMSGShutdown");
            sMSG = sMSG == null ? "Are you sure you want to shut down the RoboSep system?" : sMSG;
            GUI_Controls.RoboMessagePanel newPrompt = new GUI_Controls.RoboMessagePanel(this,sMSG, "User Shutdown", "OK", "Cancel");
            RoboSep_UserConsole.getInstance().addForm(newPrompt, newPrompt.Offset);
            RoboSep_UserConsole.getInstance().frmOverlay.Show();
            newPrompt.ShowDialog();
            RoboSep_UserConsole.getInstance().frmOverlay.Hide();

            if (newPrompt.DialogResult == DialogResult.OK)
            {
                this.Hide();
                RoboSep_UserConsole.getInstance().frmHomeOverlay.Hide();
                // shut down robosep system
                RoboSep_UserConsole.getInstance().InvokeShutdownAction();

                // LOG
                string logMSG = "SHUTDOWN button clicked";
                GUI_Controls.uiLog.LOG(this, "hex_shutdown_Click", GUI_Controls.uiLog.LogLevel.GENERAL, logMSG);
            }
        }

        private void hex_logs_Click(object sender, EventArgs e)
        {
            if (RoboSep_UserConsole.ctrlCurrentUserControl != RoboSep_Logs.getInstance())
            {
                // refresh run log content
                RoboSep_RunStatsLog.RefreshList();

                RoboSep_Logs LogWindow = RoboSep_Logs.getInstance();
                LogWindow.Location = new Point(0, 0);
                RoboSep_UserConsole.getInstance().Controls.Add(LogWindow);
                RoboSep_UserConsole.getInstance().Controls.Remove(RoboSep_UserConsole.ctrlCurrentUserControl);
                RoboSep_UserConsole.ctrlCurrentUserControl = LogWindow;
            }
            closeHomeWindow();

            // LOG
            string logMSG = "Logs button clicked";
            GUI_Controls.uiLog.LOG(this, "hex_logs_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }
    }
}
