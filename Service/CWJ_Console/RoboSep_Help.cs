using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Xml;
using System.Text;
using System.Windows.Forms;

namespace GUI_Console
{
    public partial class RoboSep_Help : UserControl
    {
        private static RoboSep_Help myHelp;
        private int TabCheck = 0;
        UserControl[] TabControls = new UserControl[3];


        // SetTab given from RoboSep_UserConsole variable "intCurrentHelpTab"
        private RoboSep_Help()
        {
            InitializeComponent();

            // LOG
            string logMSG = "Initializing Help user control";
            GUI_Controls.uiLog.LOG(this, "RoboSep_Help", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

            TabCheck = RoboSep_UserConsole.intCurrentHelpTab;
            // Create tab tab control content
            TabControls[0] = RoboSep_About.getInstance();
            TabControls[1] = new BasePannel();
            TabControls[2] = TabControls[1];
            // resize controls to fit
            TabControls[1].Size = TabControls[0].Size;
            TabControls[2].Size = TabControls[0].Size;
            // set control locations to 0,0
            for (int i = 0; i < TabControls.Length; i++)
            {
                TabControls[i].Location = new Point(0, 0);
            }
            // load tab content to display most recent tab
            this.Controls.Add(TabControls[TabCheck]);
        }

        private void Load_TabControl(int TabNumber)
        {
            this.SuspendLayout();
            this.Controls.Remove(TabControls[TabCheck]);
            this.Controls.Add(TabControls[TabNumber]);
            TabCheck = HelpTabs.CurrentTab;
            this.ResumeLayout();
        }

        public static RoboSep_Help getInstance()
        {
            if (myHelp == null)
            {
                myHelp = new RoboSep_Help();
            }
            return myHelp;
        }

        private void HelpTabs_BackgroundImageChanged(object sender, EventArgs e)
        {
            if (TabCheck != HelpTabs.CurrentTab)
            {
                Load_TabControl(HelpTabs.CurrentTab);
                RoboSep_UserConsole.intCurrentHelpTab = HelpTabs.CurrentTab;
            }

            // LOG
            string logMSG = "Load Tab Page " + HelpTabs.CurrentTab.ToString();
            GUI_Controls.uiLog.LOG(this, "HelpTabs_BackgroundImageChanged", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        private void RoboSep_Help_Load(object sender, EventArgs e)
        {
            Load_TabControl(TabCheck);
        }
    }
}
