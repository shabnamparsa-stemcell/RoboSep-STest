using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Xml;
using System.Text;
using System.Windows.Forms;
using Invetech.ApplicationLog;
namespace GUI_Console 
{
    public partial class RoboSep_Logs : UserControl
    {
        private static RoboSep_Logs myLogs;
        private int TabCheck;
        UserControl[] TabControls = new UserControl[3];
        
        private RoboSep_Logs()
        {
            InitializeComponent();
            TabCheck = RoboSep_UserConsole.intCurrentLogTab;

            // LOG
            string logMSG = "Initializing Logs Tab Page ";
            //GUI_Controls.uiLog.LOG(this, "RoboSep_Logs", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                

            TabControls[0] = RoboSep_Reports.getInstance();
            TabControls[1] = new BasePannel();
            TabControls[2] = TabControls[1];
            // resize controls to fit
            TabControls[1].Size = TabControls[0].Size;
            TabControls[2].Size = TabControls[0].Size;
            for (int i = 0; i < TabControls.Length; i++)
            {
                TabControls[i].Location = new Point(0, 0);
            }
            this.Controls.Add(TabControls[TabCheck]);
        }

        public static RoboSep_Logs getInstance()
        {
            if (myLogs == null)
            {
                myLogs = new RoboSep_Logs();
            }
            GC.Collect();
            return myLogs;
        }

        private void Load_TabControl(int TabNumber)
        {
            // LOG
            string logMSG = "Loading Log Tab " + TabNumber;
            //GUI_Controls.uiLog.LOG(this, "RoboSep_Logs", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);            

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                
            this.SuspendLayout();
            this.Controls.Remove(TabControls[TabCheck]);
            this.Controls.Add(TabControls[TabNumber]);
            TabCheck = tabs1.CurrentTab;
            this.ResumeLayout();
        }

        private void tabs1_BackgroundImageChanged(object sender, EventArgs e)
        {
            if (TabCheck != tabs1.CurrentTab)
            {
                Load_TabControl(tabs1.CurrentTab);
                RoboSep_UserConsole.intCurrentLogTab = tabs1.CurrentTab;
            }
        }

        private void RoboSep_Logs_Load(object sender, EventArgs e)
        {
            Load_TabControl(TabCheck);
        }


    }
}
