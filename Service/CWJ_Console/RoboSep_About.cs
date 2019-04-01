using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Xml;
using System.Text;
using System.Windows.Forms;

using Tesla.Common.ResourceManagement;
using Tesla.Common.OperatorConsole;
using Tesla.Separator;

namespace GUI_Console
{
    public partial class RoboSep_About : BasePannel
    {
        private static RoboSep_About myAboutPage;
        private string m_sInsVersion = "";
        private string m_sInsSerial = "";
        private string m_sInsAddress = "";
        private string m_sGatewayVer = "";
        private string m_sGatewayURL = "";
        private string m_sSvrUptime = "";
        
        private RoboSep_About()
        {
            InitializeComponent();

            // LOG
            string logMSG = "Initializing About user control";
            GUI_Controls.uiLog.LOG(this, "RoboSep_About", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        public static RoboSep_About getInstance()
        {
            if (myAboutPage == null)
            {
                myAboutPage = new RoboSep_About();
            }
            return myAboutPage;
        }

        // label info grabbed from server and stored
        // in Robosep_UserConsole
        public void UpdateLabelInfo(string[] info)
        {
            lblInstrumentControlinfo.Text = info[0];
            lblSerialinfo.Text = info[1];
            lblServiceAddressInfo.Text = info[2];
            lblGUIinfo.Text = info[3];
        }

        private void RoboSep_About_Load(object sender, EventArgs e)
        {
            // get service dates from GUI.ini
            string sLastService = GUI_Controls.fromINI.getValue("GUI", "LastServiceDate");
            DateTime dtLastService;
            int[] convertedDate;
            if (sLastService != null && sLastService != string.Empty)
            {
                string[] dateBreakup = sLastService.Split('/');
                convertedDate = new int[3] {
                    Convert.ToInt32( dateBreakup[0]),
                    Convert.ToInt32( dateBreakup[1]),
                    Convert.ToInt32( dateBreakup[2]) };
                dtLastService = new DateTime(convertedDate[2], convertedDate[1], convertedDate[0], 0, 0, 0);
                lblLastService.Text = string.Format("{0:dd MMM, yyyy}", dtLastService);
                if (convertedDate[1] <= 6)
                    convertedDate[1] += 6;
                else
                {
                    convertedDate[2]++;
                    convertedDate[1] = (convertedDate[1] + 6) % 12;
                }
                dtLastService = new DateTime(convertedDate[2], convertedDate[1], convertedDate[0], 0, 0, 0);
                lblReccomendService.Text = string.Format("{0:dd MMM, yyyy}", dtLastService);
            }
            else
            {
                lblLastService.Text = "No Service calls recorded";
                lblReccomendService.Text = "";
            }

            // get Unit ID from GUI.ini
            string sUID = GUI_Controls.fromINI.getValue("GUI", "UnitID");
            lblUID.Text = sUID;

            // get support e-mail from GUI.ini
            string sEmail = GUI_Controls.fromINI.getValue("GUI", "SupportEmail");
            lblTechEmail.Text = sEmail;

            // get support phone from GUI.ini
            string sPhone = GUI_Controls.fromINI.getValue("GUI", "SupportPhone");
            lblTechPhone.Text = sPhone;

            // Reccomended Next Service Date
            //string sReccomendService = GUI_Controls.fromINI.getValue("GUI", "ReccomendedServiceDate");
            //sReccomendService = sReccomendService == null ? "" : sReccomendService;
            //lblReccomendService.Text = sReccomendService;
        }
    }
}
