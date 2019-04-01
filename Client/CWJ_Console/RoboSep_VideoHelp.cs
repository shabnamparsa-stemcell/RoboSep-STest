using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Invetech.ApplicationLog;
namespace GUI_Console
{
    public partial class RoboSep_VideoHelp : BasePannel
    {
        public RoboSep_VideoHelp()
        {
            InitializeComponent();
            
            // LOG
            string logMSG = "Initializing Help Tab: Video Help";
            //GUI_Controls.uiLog.LOG(this, "RoboSep_VideoHelp", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);             

        }
    }
}
