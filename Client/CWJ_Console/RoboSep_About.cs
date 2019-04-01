using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Xml;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;

using Tesla.Common;
using Tesla.Common.ResourceManagement;
using Tesla.Common.OperatorConsole;
using Tesla.Separator;

using Invetech.ApplicationLog;

using GUI_Controls;

namespace GUI_Console
{
    public partial class RoboSep_About : BasePannel
    {
        private static RoboSep_About myAboutPage;
        private const string HELP_VIDEO_PATH = "\\STI\\RoboSep\\helpVideos";
        private const string DATE_FORMAT = @"d/M/yyyy";

        private IniFile GUIini;
        private IniFile LanguageINI;
        private Color TextForeColor;


        private RoboSep_About()
        {
            InitializeComponent();

            GUIini = new IniFile(IniFile.iniFile.GUI);

            // set up language specific labels
            LanguageINI = GUI_Console.RoboSep_UserConsole.getInstance().LanguageINI;
            lblInstrumentControl.Text = LanguageINI.GetString("lblInstrumentControl");
            label_LastService.Text = LanguageINI.GetString("lblLastService");
            label_NextService.Text = LanguageINI.GetString("lblNextService");
            label_LastServicePerformedBy.Text = LanguageINI.GetString("lblLastServicePerformedBy");
            label_WarrantyStartDate.Text = LanguageINI.GetString("lblWarrantyStartDate");
            label_WarrantyEndDate.Text = LanguageINI.GetString("lblWarrantyExpireDate");
            label_WarrantyStatus.Text = LanguageINI.GetString("lblWarrantyStatus");
            lblSerialNumber.Text = LanguageINI.GetString("lblSerial");
            lblServiceAddress.Text = LanguageINI.GetString("lblServiceAddress");
            lblSupport.Text = LanguageINI.GetString("lblSupport");            
            lblSalesRep.Text = LanguageINI.GetString("lblSalesRep");
            SoftwareLabel.Text = LanguageINI.GetString("lblSoftware");
            InstrumentLabel.Text = LanguageINI.GetString("lblInstrument");
            Tab_help.Tab1 = LanguageINI.GetString("tabAbout");
            Tab_help.Tab2 = LanguageINI.GetString("tabHelpVids");
            Tab_help.Tab1_Click += new System.EventHandler(this.Tab_help_Tab1_Click);
            Tab_help.Tab2_Click += new System.EventHandler(this.Tab_help_Tab2_Click);
            TextForeColor = lblWarrantyStatus.ForeColor;
            mediaPlayer.Visible = false;
            mediaPlayer.PlayStateChange += new AxWMPLib._WMPOCXEvents_PlayStateChangeEventHandler(mediaPlayer_PlayStateChange);
            initializeVideoList();
            PopulateInfoFromGuiINIFile ();
            // LOG
            string logMSG = "Initializing About user control";
            //GUI_Controls.uiLog.LOG(this, "RoboSep_About", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                
        }

        public static RoboSep_About getInstance()
        {
            if (myAboutPage == null)
            {
                myAboutPage = new RoboSep_About();
            }
            GC.Collect();
            return myAboutPage;
        }

        // label info grabbed from server and stored
        // in Robosep_UserConsole
        public void UpdateInstrumentLabelInfo(string[] info)
        {
            string sIPAddress = GetIPAddress();
            if (string.IsNullOrEmpty(sIPAddress))
            {
                sIPAddress  = String.Empty;
            }
            lblInstrumentControlinfo.Text = info[0];
            lblSerialinfo.Text = info[1];
            lblServiceAddressInfo.Text = sIPAddress;
            lblGUIinfo.Text = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
        }

        private string getVideoFileName(string path)
        {
            string fileName = null;
            if (path != null && path.Trim().Length > 0)
            {
                int idx = path.LastIndexOf("\\");
                int idx2 = path.LastIndexOf(".");
                if (idx >= 0 && idx2 >= 0)
                    return path.Substring(idx + 1, idx2 - idx - 1);
            }
            return fileName;
        }

        private void initializeVideoList()
        {
            bool existed = false;
            try
            {
                string videosFolderPath = Utilities.ProgramFiles() + HELP_VIDEO_PATH;
                string[] videosPaths = Directory.GetFiles(videosFolderPath, "*.mp4");
                if (videosPaths != null && videosPaths.Length > 0)
                {
                    videoListPanel.RowCount = videosPaths.Length;
                    for (int i = 0; i < videosPaths.Length; i++)
                    {
                        LinkLabel label = new LinkLabel();
                        label.Text = getVideoFileName(videosPaths[i]);
                        label.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
                        LinkLabel.Link link = new LinkLabel.Link();
                        link.LinkData = videosPaths[i];
                        label.Links.Add(link);
                        label.Height = 70;
                        label.Width = 600;
                        label.TextAlign = ContentAlignment.MiddleLeft;
                        label.ForeColor = Color.FromArgb(95, 96, 98);
                        label.Font = new Font("Arial", 15);
                        label.Click += new System.EventHandler(this.linkLabel_Click);
                        videoListPanel.Controls.Add(label, 0, i);
                    }
                    existed = true;
                }
            }
            catch (DirectoryNotFoundException)
            { }
            if (!existed)
                Tab_help.Tab2 = "";
        }

        private void mediaPlayer_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            switch (e.newState)
            {
                case 1:     // Stopped
                    mediaPlayer.fullScreen = false;
                    mediaPlayer.Visible = false;
                    mediaPlayer.windowlessVideo = false;
                    break;
                case 3:    // Playing
                    mediaPlayer.fullScreen = true;
                    mediaPlayer.Visible = true;
                    mediaPlayer.windowlessVideo = true;
                    break;
            }
        }

        private void linkLabel_Click(object sender, EventArgs e)
        {
            if (RoboSep_UserConsole.bIsRunning)
            {
                RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING,
                    LanguageINI.GetString("tabHelpVideoCannotBePlayed"), LanguageINI.GetString("Warning"), LanguageINI.GetString("Ok"));
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                prompt.Dispose();
                RoboSep_UserConsole.hideOverlay();
            }
            else
            {
                LinkLabel.Link link = (sender as LinkLabel).Links[0] as LinkLabel.Link;
                string path = link.LinkData as string;
                mediaPlayer.URL = @path;
                mediaPlayer.Ctlcontrols.play();
            }
        }

        private void RoboSep_About_Load(object sender, EventArgs e)
        {
            PopulateInfoFromGuiINIFile();
        }

        public void PopulateInfoFromGuiINIFile()
        {
            // get service dates from GUI.ini
            string sLastService = GUIini.IniReadValue("About", "LastServiceDate", "");
            ShowDate(sLastService, lblLastService);

            // get next service date from GUI.ini
            string sNextService = GUIini.IniReadValue("About", "NextServiceDate", "");
            ShowDate(sNextService, lblNextService);

            // get last service person from GUI.ini
            string sLastServicePerformedBy = GUIini.IniReadValue("About", "LastServicePerformedBy", "");
            lblLastServicePerformedBy.Text = sLastServicePerformedBy;

            // get warranty start date from GUI.ini
            string sWarrantyStartDate = GUIini.IniReadValue("About", "WarrantyStartDate", "");
            ShowDate(sWarrantyStartDate, lblWarrantyStartDate);

            // get warranty end date from GUI.ini
            string sWarrantyExpireDate = GUIini.IniReadValue("About", "WarrantyExpireDate", "");
            ShowDate(sWarrantyExpireDate, lblWarrantyExpireDate);

            // get warranty status from GUI.ini
            string sWarrantyStatus = GUIini.IniReadValue("About", "WarrantyStatus", "");
            lblWarrantyStatus.Text = sWarrantyStatus;

            // get support e-mail from GUI.ini
            string sEmail = GUIini.IniReadValue("About", "SupportEmail", "");
            lblTechEmail.Text = sEmail;

            // get support phone from GUI.ini
            string sPhone = GUIini.IniReadValue("About", "SupportPhone", "");
            lblTechPhone.Text = sPhone;

            string sSalesName = GUIini.GetString ( "SalesRepName" );
            label_SalesName.Text = sSalesName;

            string sSalesEmail = GUIini.GetString("SalesRepEmail");
            label_SalesEmail.Text = sSalesEmail;

            string sSalesPhone = GUIini.GetString("SalesRepPhone");
            label_SalesPhone.Text = sSalesPhone;

            // get auto compute warranty status from GUI.ini
            bool bAutoComputeWarrantyStatus = true;
            string sAutoComputeWarrantyStatus = GUIini.IniReadValue("About", "AutoComputeWarrantyStatus", "true");
            if (!string.IsNullOrEmpty(sAutoComputeWarrantyStatus) && (sAutoComputeWarrantyStatus.ToLower() == "false" || sAutoComputeWarrantyStatus.ToLower() == "no"))
            {
                bAutoComputeWarrantyStatus = false;
            }

            string sWarrantyInfo = string.Empty;
            lblWarrantyStatus.ForeColor = TextForeColor;
            if (!string.IsNullOrEmpty(sWarrantyExpireDate) && bAutoComputeWarrantyStatus)
            {
                try
                {
                    object obj = ConvertDateStringToDate(sWarrantyExpireDate);
                    if (obj != null)
                    {
                        DateTime dateExpire = (DateTime)obj;
                        if (DateTime.Now.CompareTo(dateExpire) <= 0)
                        {
                            // Valid
                            sWarrantyInfo = LanguageINI.GetString("lblValidThrough");
                        }
                        else
                        {
                            // Expired
                            lblWarrantyStatus.ForeColor = Color.Red;
                            sWarrantyInfo = LanguageINI.GetString("lblExpired");
                        }
                        sWarrantyInfo += " ";
                        sWarrantyInfo += String.Format("{0:dd MMM, yyyy}", dateExpire);
                        lblWarrantyStatus.Text = sWarrantyInfo;
                    }
                    else
                    {
                        lblWarrantyStatus.ForeColor = Color.Red;
                        lblWarrantyStatus.Text = LanguageINI.GetString("msgInvalidDate");
                    }
                }
                catch (Exception ex)
                {
                    LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);
                }
            }
        }

        private string GetIPAddress()
        {
            string sHostName = System.Net.Dns.GetHostName();
            if (string.IsNullOrEmpty(sHostName))
                return null;

            IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(sHostName);
            if (ipEntry == null)
                return null;

            IPAddress[] addr = ipEntry.AddressList;
            if (addr == null || addr.Length < 1)
                return null;

            return addr[addr.Length - 1].ToString();
        }


        private void ShowDate(string sDate, Label lbl)
        {
            if (!string.IsNullOrEmpty(sDate) || lbl == null)
            {
                DateTime dtDate;
                object obj = ConvertDateStringToDate(sDate);
                if (obj != null)
                {
                    dtDate = (DateTime)obj;
                    lbl.Text = string.Format("{0:dd MMM, yyyy}", dtDate);
                }
                else
                {
                    lbl.ForeColor = Color.Red;
                    lbl.Text = LanguageINI.GetString("msgInvalidDate");
                }
            }
            else
            {
                lbl.ForeColor = Color.Red;
                lbl.Text = LanguageINI.GetString("msgNoRecordedDate");
            }
        }

        private object ConvertDateStringToDate(string sDate)
        {
            string errMsg;
            if (string.IsNullOrEmpty(sDate))
            {
                errMsg = String.Format("Failed to convert string to date. Input parameter is null");
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Error, errMsg);
                return null;
            }
           
            DateTime dateValue;
            if (!DateTime.TryParseExact(sDate, DATE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
            {
                // error for parsing
                errMsg = String.Format("Failed to convert variable '{0}' to valid date.", sDate);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Error, errMsg);
                return null;
            }
            return dateValue;
        }

        private void Tab_help_Tab1_Click(object sender, EventArgs e)
        { 
            if (Tab_help.TabActive == HorizontalTabs.tabs.Tab1)
                return;

            Tab_help.TabActive = HorizontalTabs.tabs.Tab1;
            videoListPanel.Visible = false;
        }

        private void Tab_help_Tab2_Click(object sender, EventArgs e)
        {
            // Navigate to Help Videos page
            if (Tab_help.TabActive == HorizontalTabs.tabs.Tab2)
                return;

            Tab_help.TabActive = HorizontalTabs.tabs.Tab2;
            videoListPanel.Visible = true;
        }

        

   
    }
}
