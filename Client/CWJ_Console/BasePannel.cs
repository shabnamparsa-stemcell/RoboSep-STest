using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Xml;
using System.Text;
using System.Windows.Forms;

using Tesla.Common.ResourceManagement;
using Tesla.OperatorConsoleControls;
using Tesla.Common.OperatorConsole;
using Tesla.Separator;
using Invetech.ApplicationLog;
// Creates basic RoboSep panel with
namespace GUI_Console
{
    public partial class BasePannel : UserControl
    {
        protected SeparatorEventSink myEventSink;
        protected ISeparator mySeparator;
        private RoboSep_Home myHome;

        public BasePannel()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        protected virtual void btn_home_Click(object sender, EventArgs e)
        {
            // Open Home Window
            myHome = RoboSep_Home.getInstance();
            RoboSep_UserConsole.getInstance().SuspendLayout();
            myHome.Location = new Point(0, 0);
            RoboSep_UserConsole.getInstance().Controls.Add(myHome);
            RoboSep_UserConsole.getInstance().Controls.Remove(RoboSep_UserConsole.ctrlCurrentUserControl);
            myHome.Visible = true;
            myHome.Enabled = true;
            myHome.UpdateButtons();
            myHome.BringToFront();
            RoboSep_UserConsole.getInstance().SuspendLayout();
            //RoboSep_UserConsole.getInstance().frmHomeOverlay.Show();
            //RoboSep_UserConsole.getInstance().frmHomeOverlay.BringToFront();
            //myHome.Location = new Point(
            //RoboSep_UserConsole.getInstance().Location.X + RoboSep_UserConsole.getInstance().myHome.GetOffset().X,
            //RoboSep_UserConsole.getInstance().Location.Y + RoboSep_UserConsole.getInstance().myHome.GetOffset().Y);
            //myHome.Show();
            //myHome.BringToFront();

            // LOG
            string logMSG = "Home button clicked";
            //GUI_Controls.uiLog.LOG(this, "btn_home_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);
        }

        private void BasePanel1_Resize(object sender, EventArgs e)
        {
            // Code here for resizing window
            // change bg graphic at incriments of screen size (640x480, 800x600, etc)
            // move home button with screen size
        }

        public virtual void Initialise()
        {

        }
    }
}

