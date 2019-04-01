using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Xml;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Threading;
using Invetech.ApplicationLog;
using GUI_Controls;
using Tesla.Common;
using Report_Crystal;


namespace GUI_Console
{
    public partial class RoboSep_Reports : BasePannel
    {
        private static RoboSep_Reports myReports;
        private FileInfo[] myReportFiles;
        private const int MAX_RUNS_DISPLAYED = 25;
        private const int IMAGELIST_WIDTH = 40;
        private const int IMAGELIST_HEIGHT = 40;
        private const int CHECKBOX_SIZE = 32;
        private const int BUTTON_SIZE = 32;
        private const int CellPaddingSize = 4;
        private const int EndPaddingSize = 6;

        private const int minPdfZoomPercent = 10;
        private const int maxPdfZoomPercent = 5000;
        private string DefaultPdfZoomPercent = "50";

        // GUI file
        private IniFile GUIini = new IniFile(IniFile.iniFile.GUI);

        // Language file
        private IniFile LanguageINI = GUI_Console.RoboSep_UserConsole.getInstance().LanguageINI;

        private RoboMessagePanel WaitingMSG = null;

        // listview drawing vars
        private StringFormat theFormat;
        private Color BG_ColorEven;
        private Color BG_ColorOdd;
        private Color BG_InUserList;
        private Color BG_Selected;
        private Color Txt_Color;

        private List<Image> ilist1 = new List<Image>();

        private RoboSep_Reports()
        {
            InitializeComponent();


            ilist1.Add(Properties.Resources.btnLG_STD);
            ilist1.Add(Properties.Resources.btnLG_STD);
            ilist1.Add(Properties.Resources.btnLG_STD);
            ilist1.Add(Properties.Resources.btnLG_CLICK);
            button_SaveReports.ChangeGraphics(ilist1);

            // set label and button text based on language setting
            horizontalTabs1.Tab1 = LanguageINI.GetString("lblReports2");
            lblDateTime.Text = LanguageINI.GetString("lblDateTime");
            lblUser.Text = LanguageINI.GetString("lblUser");
            lblRunInfo.Text = LanguageINI.GetString("lblRunInfo");
            button_SaveReports.Text = LanguageINI.GetString("lblSaveReports");
            button_ViewReport.Text = LanguageINI.GetString("lblViewReport");
            label_SelectAll.Text = LanguageINI.GetString("lblAll");

            CreateImageListGraphics();

            // set up listview drag scroll properties
            listView_robostat.LINE_SIZE = 14;
            listView_robostat.VERTICAL_PAGE_SIZE = 5;
            listView_robostat.NUM_VISIBLE_ELEMENTS = 6;
            listView_robostat.VisibleRow = 6;

            scrollBar_Report.LargeChange = listView_robostat.VisibleRow - 1;
            listView_robostat.VScrollbar = scrollBar_Report;

            // set colours for drawing
            theFormat = new StringFormat();
            theFormat.Alignment = StringAlignment.Near;
            theFormat.LineAlignment = StringAlignment.Center;
            BG_ColorEven = Color.FromArgb(216, 217, 218);
            BG_ColorOdd = Color.FromArgb(243, 243, 243);
            BG_InUserList = Color.FromArgb(146, 108, 175);
            BG_Selected = Color.FromArgb(78, 38, 131);
            Txt_Color = Color.FromArgb(95, 96, 98);

            // LOG
            string logMSG = "Initializing Log Tab Run Statistics";
            //GUI_Controls.uiLog.LOG(this, "RoboSep_Reports", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                
        }

        public static void RefreshList()
        {
            RoboSep_Reports.getInstance().LoadReportContent2();
        }

        public static RoboSep_Reports getInstance()
        {
            if (myReports == null)
            {
                myReports = new RoboSep_Reports();
            }
            GC.Collect();
            return myReports;
        }

        private void CreateImageListGraphics()
        {
            imageList1.ImageSize = new System.Drawing.Size(IMAGELIST_WIDTH, IMAGELIST_HEIGHT);
            imageList1.Images.Clear();

            Bitmap b1 = new Bitmap(Properties.Resources.GE_BTN08S_unselect_STD);
            Bitmap b2 = new Bitmap(Properties.Resources.GE_BTN07S_select_CLICK);
            this.imageList1.Images.Add("unchecked", b1);
            this.imageList1.Images.Add("checked", b2);
            var dummy = this.imageList1.Handle;
            b1.Dispose();
            b2.Dispose();
        }

        private FileInfo[] getReportFiles()
        {
            FileInfo[] theReportFiles = null;
            string sysPath = RoboSep_UserDB.getInstance().sysPath;
            string reportsPath = sysPath + "Reports\\";
            
            DirectoryInfo DI = new DirectoryInfo(reportsPath);
            theReportFiles = DI.GetFiles("*.html", SearchOption.AllDirectories);

            Array.Sort(theReportFiles, delegate(FileInfo f1, FileInfo f2)
            {
                return f1.LastWriteTime.CompareTo(f2.LastWriteTime);
            });

            int DisplayedLogsCount = MAX_RUNS_DISPLAYED;
            if (theReportFiles.Length < MAX_RUNS_DISPLAYED)
                DisplayedLogsCount = theReportFiles.Length;

            FileInfo[] returnreports = new FileInfo[DisplayedLogsCount];
            for (int i = 0; i < DisplayedLogsCount; i++)
                returnreports[i] = theReportFiles[ (theReportFiles.Length-1)-i];
            return returnreports;
        }

        private FileInfo[] getReportFiles2()
        {
            FileInfo[] theReportFiles = null;
            string sysPath = RoboSep_UserDB.getInstance().sysPath;
            string reportsPath = sysPath + "Reports\\";

            DirectoryInfo DI = new DirectoryInfo(reportsPath);
            theReportFiles = DI.GetFiles("*.xml", SearchOption.AllDirectories);

            Array.Sort(theReportFiles, delegate(FileInfo f1, FileInfo f2)
            {
                return f1.LastWriteTime.CompareTo(f2.LastWriteTime);
            });

            int DisplayedLogsCount = MAX_RUNS_DISPLAYED;
            if (theReportFiles.Length < MAX_RUNS_DISPLAYED)
                DisplayedLogsCount = theReportFiles.Length;

            FileInfo[] returnreports = new FileInfo[DisplayedLogsCount];
            for (int i = 0; i < DisplayedLogsCount; i++)
                returnreports[i] = theReportFiles[(theReportFiles.Length - 1) - i];
            return returnreports;
        }


        private void LoadReportContent()
        {
            myReportFiles = getReportFiles();

            listView_robostat.Items.Clear();
            
            // values of interest
            DateTime FileCreated;
            string UserName = string.Empty;
            string CompleteStatus = string.Empty;
            string QuadrantsUsed = string.Empty;
            
            // parse documents
            for (int i = 0; i < myReportFiles.Length; i++)
            {
                FileCreated = myReportFiles[i].LastWriteTime;

                if (File.Exists(myReportFiles[i].FullName))
                {
                    using (StreamReader sr = new StreamReader(myReportFiles[i].FullName))
                    {
                        string line = string.Empty;
                        // throw away first lines
                        for (int j=0; j< 38; j++)
                        {
                            line = sr.ReadLine();
                        }
                        string[] splitString = line.Split('<', '>');
                        CompleteStatus = splitString[2];

                        for (int j = 0; j < 4; j++)
                        {
                            line = sr.ReadLine();
                        }
                        splitString = line.Split('<', '>');
                        UserName = splitString[2];

                        string lookForText = "Number of Quadrants Used";
                        bool foundText = false;
                        do
                        {
                            line = sr.ReadLine();
                            splitString = line.Split('<', '>');
                            if (splitString.Length > 5 && splitString[4] == lookForText)
                                foundText = true;
                        } while (!foundText);

                        int numberOfQuadrants = 0;
                        bool activeQuadrant;
                        do
                        {
                            activeQuadrant = false;
                            line = sr.ReadLine();
                            splitString = line.Split('<', '>');
                            if (splitString[2] == "1" || splitString[2] == "2" || splitString[2] == "3" || splitString[2] == "4")
                            {
                                numberOfQuadrants++;
                                activeQuadrant = true;
                            }
                        } while (activeQuadrant);
                        QuadrantsUsed = numberOfQuadrants.ToString();
                    }

                    // add item to listview
                    ListViewItem lvItem = listView_robostat.Items.Add(string.Format("{0:HH:mm dd/MM/yy}", FileCreated));
                    lvItem.Tag = i;

                    listView_robostat.Items[i].SubItems.Add(UserName);

                    string tmp = QuadrantsUsed + " Protocol";
                    tmp = QuadrantsUsed == "1" ? tmp + "  - " : tmp + "s - ";
                    tmp += CompleteStatus.StartsWith("ABORTED") ? "ABORTED by User" : CompleteStatus;
                    listView_robostat.Items[i].SubItems.Add(tmp);
                }
            }

            listView_robostat.UpdateScrollbar();
            listView_robostat.ResizeVerticalHeight(true);
            Rectangle rcScrollbar = this.scrollBar_Report.Bounds;
            this.scrollBar_Report.SetBounds(rcScrollbar.X, listView_robostat.Bounds.Y, rcScrollbar.Width, listView_robostat.Bounds.Height);

            listView_robostat.Refresh();
        }



        private void LoadReportContent2()
        {
            myReportFiles = getReportFiles2();

            listView_robostat.Items.Clear();

            XmlDocument myXmlDocument = new XmlDocument();
            XmlNode node;

            // values of interest
            DateTime FileCreated;
            string UserName = string.Empty;
            string CompleteStatus = string.Empty;
            string QuadrantsUsed = string.Empty;

            // parse documents
            for (int i = 0; i < myReportFiles.Length; i++)
            {
                FileCreated = myReportFiles[i].LastWriteTime;

                if (File.Exists(myReportFiles[i].FullName))
                {
                    myXmlDocument.Load(myReportFiles[i].FullName);
                    node = myXmlDocument.DocumentElement;

                    int nSample = 0;
                    UserName = string.Empty;
                    CompleteStatus = string.Empty;
                    QuadrantsUsed = string.Empty;

                    foreach (XmlNode node1 in node.ChildNodes)
                    {
                        if (string.IsNullOrEmpty(node1.Name)) 
                            continue;
                        
                        // table 'RunDetails'
                        if (node1.Name == "RunDetails")
                        {
                            int nCount = 0;
                            foreach (XmlNode node2 in node1.ChildNodes)
                            {
                                if (string.IsNullOrEmpty(node2.Name))
                                    continue;

                                if (node2.Name == "operatorID" && string.IsNullOrEmpty(UserName))
                                {
                                    UserName = node2.InnerText;
                                    nCount++;
                                }
                                if (node2.Name == "finalStatus" && string.IsNullOrEmpty(CompleteStatus))
                                {
                                    CompleteStatus = node2.InnerText;
                                    nCount++;
                                }
                                if (nCount == 2)
                                    break;
                            }
                        }
                        
                        // Count the number of samples
                        if (node1.Name == "RunSample")
                        {
                            nSample++;
                        }
                    } // end for node1

                    QuadrantsUsed = nSample.ToString();

                    // add item to listview
                    ListViewItem lvItem = listView_robostat.Items.Add(string.Format("{0:HH:mm dd/MM/yy}", FileCreated));
                    lvItem.Tag = i;

                    listView_robostat.Items[i].SubItems.Add(UserName);

                    string tmp = QuadrantsUsed + " Protocol";
                    tmp = QuadrantsUsed == "1" ? tmp + "  - " : tmp + "s - ";
                    tmp += CompleteStatus.StartsWith("ABORTED") ? "ABORTED by User" : CompleteStatus;
                    listView_robostat.Items[i].SubItems.Add(tmp);

                } 
            } 

            listView_robostat.UpdateScrollbar();
            listView_robostat.ResizeVerticalHeight(true);
            Rectangle rcScrollbar = this.scrollBar_Report.Bounds;
            this.scrollBar_Report.SetBounds(rcScrollbar.X, listView_robostat.Bounds.Y, rcScrollbar.Width, listView_robostat.Bounds.Height);

            listView_robostat.Refresh();
        }


        private int[] GetItemsChecked()
        {
            List<int> lstItemsChecked = new List<int>();

            // verify that at least one item checked
            ListViewItem ivItem;
            for (int i = 0; i < listView_robostat.Items.Count; i++)
            {
                ivItem = listView_robostat.Items[i];

                if (ivItem.Checked == false)
                    continue;

                int nIndex = (Int32)ivItem.Tag;

                lstItemsChecked.Add(nIndex);
            }
            return lstItemsChecked.ToArray();
        }

        private void checkBox_SelectAll_Click(object sender, EventArgs e)
        {
            if (listView_robostat.Items.Count == 0)
                return;

            ListViewItem lvItem;
            int nItemChecked = 0;
            for (int i = 0; i < listView_robostat.Items.Count; i++)
            {
                lvItem = listView_robostat.Items[i];
                if (lvItem == null)
                    continue;

                // Check/uncheck all the items
                lvItem.Checked = checkBox_SelectAll.Checked;

                if (checkBox_SelectAll.Checked)
                    nItemChecked++;
            }
            button_ViewReport.Visible = nItemChecked > 1 ? false : true;
        }

        private void listView_robostat_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView_robostat.UpdateScrollbar();
        }

        private void listView_robostat_Click(object sender, EventArgs e)
        {
            if (listView_robostat.SelectedItems.Count == 0)
                return;

            ListViewItem lvItem = listView_robostat.FocusedItem;
            if (lvItem == null)
                return;

            lvItem.Checked = !lvItem.Checked;

            int[] arrItemsChecked = GetItemsChecked();
            button_ViewReport.Visible = (arrItemsChecked != null && arrItemsChecked.Length > 1) ? false : true;
            if (arrItemsChecked != null && arrItemsChecked.Length == 1)
            {
                if ((Int32)lvItem.Tag == arrItemsChecked[0])
                    return;

                // Remove the highlight for this item
                lvItem.Selected = false;
                lvItem.Focused = false;

                ListViewItem lvItem1;
                for (int i = 0; i < listView_robostat.Items.Count; i++)
                {
                    lvItem1 = listView_robostat.Items[i];
                    if (lvItem1 == null || !lvItem1.Checked)
                        continue;

                    // Highlight the checked item
                    lvItem1.Selected = true;
                    lvItem1.Focused = true;

                    if (!IsItemVisible(lvItem1.Index))
                        listView_robostat.EnsureVisible(lvItem1.Index);

                    break;
                }
             }
        }

        private bool IsItemVisible(int index)
        {
            return (index < listView_robostat.TopItem.Index || index > listView_robostat.TopItem.Index + listView_robostat.VisibleRow) ? false : true;
        }


        private void ViewReport()
        {
            int[] arrItemsChecked = GetItemsChecked();
            if (arrItemsChecked == null || arrItemsChecked.Length != 1)
                return;

            int index = arrItemsChecked[0];

            FileInfo fInfo = new FileInfo(myReportFiles[index].FullName);
            if (fInfo == null)
                return;

            int nIndex = fInfo.Name.LastIndexOf('.');
            if (nIndex < 0)
                return;

            string sFileName = fInfo.Name.Substring(0, nIndex);
            sFileName += ".pdf";

            string sFullFileName = fInfo.DirectoryName;
            if (sFullFileName.LastIndexOf('\\') != sFullFileName.Length - 1)
            {
                sFullFileName += "\\";
            }

            sFullFileName += sFileName;
            string errMsg;
            if (File.Exists(sFullFileName))
            {
                try
                {
                    var adobe = Registry.LocalMachine.OpenSubKey("Software").OpenSubKey("Classes").OpenSubKey("acrobat").OpenSubKey("shell").OpenSubKey("open").OpenSubKey("command");
                    if (adobe == null)
                    {
                        errMsg = String.Format("Failed to view report because it cannot find the registry key of acrobat reader.");
                        LogFile.AddMessage(System.Diagnostics.TraceLevel.Warning, errMsg);
                        return;
                    }
                    var pathAdobe = adobe.GetValue("");
                    string path = pathAdobe as string;
                    if (string.IsNullOrEmpty(path))
                    {
                        // Show message that Adobe has not been installed
                        IniFile LanguageINI = RoboSep_UserConsole.getInstance().LanguageINI;
                        string sMSG = LanguageINI.GetString("msgViewReportFailed");
                        RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_ERROR, sMSG,
                            LanguageINI.GetString("headerViewReportFailed"), LanguageINI.GetString("Ok"));
                        RoboSep_UserConsole.showOverlay();
                        prompt.ShowDialog();
                        RoboSep_UserConsole.hideOverlay();
                        errMsg = String.Format("Failed to view report because it cannot find the registry key of acrobat reader.");
                        LogFile.AddMessage(System.Diagnostics.TraceLevel.Warning, errMsg);
                        prompt.Dispose();                        
                        return;
                    }

                    string pattern = @"\b[a-zA-Z]:[\\/]([ ()._a-zA-Z0-9]+[\\/]?)*([_a-zA-Z0-9]+\.[_a-zA-Z0-9]{0,3})?";
                    Match m = Regex.Match(path, pattern, RegexOptions.Singleline);
                    if (!m.Success)
                    {
                        // Show message that Adobe has not been installed
                        IniFile LanguageINI = RoboSep_UserConsole.getInstance().LanguageINI;
                        string sMSG = LanguageINI.GetString("msgViewReportFailed2");
                        RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_ERROR, sMSG,
                            LanguageINI.GetString("headerViewReportFailed"), LanguageINI.GetString("Ok"));
                        RoboSep_UserConsole.showOverlay();
                        prompt.ShowDialog();
                        RoboSep_UserConsole.hideOverlay();
                        errMsg = String.Format("Failed to view report because it cannot the directory of acrobat reader.");
                        LogFile.AddMessage(System.Diagnostics.TraceLevel.Warning, errMsg);
                        prompt.Dispose();                        
                        return;
                    }

                    string sZoom = GUIini.IniReadValue("Report", "DefaultPdfZoomPercent", DefaultPdfZoomPercent);
                    if (string.IsNullOrEmpty(sZoom))
                    {
                        errMsg = String.Format("Failed to view report because DefaultPdfZoomPercent is not configured in GUI.ini file.");
                        LogFile.AddMessage(System.Diagnostics.TraceLevel.Warning, errMsg);
                        return;
                    }
                    int zoom;
                    if (!int.TryParse(sZoom.Trim(), out zoom))
                    {
                        errMsg = String.Format("Failed to view report because DefaultPdfZoomPercent cannot be converted to a number, it is not configured properly in GUI.ini file.");
                        LogFile.AddMessage(System.Diagnostics.TraceLevel.Warning, errMsg);
                        return;
                    }

                    // Check range
                    if (zoom < minPdfZoomPercent)
                        zoom = minPdfZoomPercent;

                    if (zoom > maxPdfZoomPercent)
                        zoom = maxPdfZoomPercent;

                    string temp = String.Format("zoom={0}=OpenActions", zoom);
                    string param = "/A \"" + temp + "\" " + sFullFileName;
                    FileInfo fileInfo = new FileInfo(m.Value);

                    System.Diagnostics.Process.Start(fileInfo.Name, param);
                }
                catch (Win32Exception ex)
                {
                    errMsg = String.Format("Failed to view report. Exception: {0}", ex.Message);
                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Warning, errMsg);
                }
            }
        }

        private void ViewReport2()
        {
            int[] arrItemsChecked = GetItemsChecked();
            if (arrItemsChecked == null || arrItemsChecked.Length != 1)
                return;

            int index = arrItemsChecked[0];

            FileInfo fInfo = new FileInfo(myReportFiles[index].FullName);
            if (fInfo == null)
                return;

            int nIndex = fInfo.Name.LastIndexOf('.');
            if (nIndex < 0)
                return;

            string sFileName = fInfo.Name.Substring(0, nIndex);
            string sXmlFileName = sFileName + ".xml";
            string sPdfFileName = sFileName + ".pdf";

            sFileName += ".pdf";

            string sFullFileName = fInfo.DirectoryName;
            if (sFullFileName.LastIndexOf('\\') != sFullFileName.Length - 1)
            {
                sFullFileName += "\\";
            }

            string sFullPdfFileName = sFullFileName + sPdfFileName;

            // Generate PDF file if not created
            if (!File.Exists(sFullPdfFileName) == true)
            {
                string sMsg = LanguageINI.GetString("msgGeneratingReport");
                string sHeader = LanguageINI.GetString("headerGeneratingReport");
                WaitingMSG = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_INFORMATION, sMsg, sHeader, true, false);
                RoboSep_UserConsole.showOverlay();
                WaitingMSG.Show();

                string sFullXmlFileName = sFullFileName + sXmlFileName;
                string[] aFileNames = new string[] { sFullXmlFileName, sFullPdfFileName};

                BackgroundWorker bwGenerateReport = new BackgroundWorker();
                bwGenerateReport.WorkerSupportsCancellation = true;

                // Attach the event handlers
                bwGenerateReport.DoWork += new DoWorkEventHandler(bwGenerateReport_DoWork);
                bwGenerateReport.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwGenerateReport_RunWorkerCompleted);

                // Kick off the Async thread
                bwGenerateReport.RunWorkerAsync(aFileNames);
            }
            else
            {
                LaunchAdobe(sFullPdfFileName);
            }
        }

        private void bwGenerateReport_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            string[] aFileNames = e.Argument as string[];
            if (aFileNames == null || aFileNames.Length == 0)
                return;

            RoboSep_UserConsole.getInstance().GenerateReport(aFileNames[0], aFileNames[1]);

            if (!e.Cancel)
            {
                // PDF file name
                e.Result = aFileNames[1];
            }
        }

        private void bwGenerateReport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Invoke(
               (MethodInvoker)delegate()
               {
                   CloseWaitMessage();

                   string fileName = e.Result as string;
                   LaunchAdobe(fileName);
               }
            );
        }

        private void CloseWaitMessage()
        {
            // clean up
            if (WaitingMSG != null)
            {
                WaitingMSG.Close();
                WaitingMSG = null;
                RoboSep_UserConsole.hideOverlay();
            }
        }
        private void LaunchAdobe(string sFullPdfFileName)
        {
            if (string.IsNullOrEmpty(sFullPdfFileName))
                return;

            string errMsg;
            if (File.Exists(sFullPdfFileName))
            {
                try
                {
                    var adobe = Registry.LocalMachine.OpenSubKey("Software").OpenSubKey("Classes").OpenSubKey("acrobat").OpenSubKey("shell").OpenSubKey("open").OpenSubKey("command");
                    if (adobe == null)
                    {
                        errMsg = String.Format("Failed to view report because it cannot find the registry key of acrobat reader.");
                        LogFile.AddMessage(System.Diagnostics.TraceLevel.Warning, errMsg);
                        return;
                    }
                    var pathAdobe = adobe.GetValue("");
                    string path = pathAdobe as string;
                    if (string.IsNullOrEmpty(path))
                    {
                        // Show message that Adobe has not been installed
                        IniFile LanguageINI = RoboSep_UserConsole.getInstance().LanguageINI;
                        string sMSG = LanguageINI.GetString("msgViewReportFailed");
                        RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_ERROR, sMSG,
                            LanguageINI.GetString("headerViewReportFailed"), LanguageINI.GetString("Ok"));
                        RoboSep_UserConsole.showOverlay();
                        prompt.ShowDialog();
                        RoboSep_UserConsole.hideOverlay();
                        errMsg = String.Format("Failed to view report because it cannot find the registry key of acrobat reader.");
                        LogFile.AddMessage(System.Diagnostics.TraceLevel.Warning, errMsg);
                        prompt.Dispose();                        
                        return;
                    }

                    string pattern = @"\b[a-zA-Z]:[\\/]([ ()._a-zA-Z0-9]+[\\/]?)*([_a-zA-Z0-9]+\.[_a-zA-Z0-9]{0,3})?";
                    Match m = Regex.Match(path, pattern, RegexOptions.Singleline);
                    if (!m.Success)
                    {
                        // Show message that Adobe has not been installed
                        IniFile LanguageINI = RoboSep_UserConsole.getInstance().LanguageINI;
                        string sMSG = LanguageINI.GetString("msgViewReportFailed2");
                        RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_ERROR, sMSG,
                            LanguageINI.GetString("headerViewReportFailed"), LanguageINI.GetString("Ok"));
                        RoboSep_UserConsole.showOverlay();
                        prompt.ShowDialog();
                        RoboSep_UserConsole.hideOverlay();
                        errMsg = String.Format("Failed to view report because it cannot the directory of acrobat reader.");
                        LogFile.AddMessage(System.Diagnostics.TraceLevel.Warning, errMsg);
                        prompt.Dispose();
                        return;
                    }

                    string sZoom = GUIini.IniReadValue("Report", "DefaultPdfZoomPercent", DefaultPdfZoomPercent);
                    if (string.IsNullOrEmpty(sZoom))
                    {
                        errMsg = String.Format("Failed to view report because DefaultPdfZoomPercent is not configured in GUI.ini file.");
                        LogFile.AddMessage(System.Diagnostics.TraceLevel.Warning, errMsg);
                        return;
                    }
                    int zoom;
                    if (!int.TryParse(sZoom.Trim(), out zoom))
                    {
                        errMsg = String.Format("Failed to view report because DefaultPdfZoomPercent cannot be converted to a number, it is not configured properly in GUI.ini file.");
                        LogFile.AddMessage(System.Diagnostics.TraceLevel.Warning, errMsg);
                        return;
                    }

                    // Check range
                    if (zoom < minPdfZoomPercent)
                        zoom = minPdfZoomPercent;

                    if (zoom > maxPdfZoomPercent)
                        zoom = maxPdfZoomPercent;

                    string temp = String.Format("zoom={0}=OpenActions", zoom);
                    string param = "/A \"" + temp + "\" " + sFullPdfFileName;
                    FileInfo fileInfo = new FileInfo(m.Value);

                    System.Diagnostics.Process.Start(fileInfo.Name, param);
                }
                catch (Win32Exception ex)
                {
                    errMsg = String.Format("Failed to view report. Exception: {0}", ex.Message);
                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Warning, errMsg);
                }
            }
        }


        private void button_ViewReport_Click(object sender, EventArgs e)
        {
            ViewReport2();
        }

        private void button_SaveReports_Click(object sender, EventArgs e)
        {
            string msg = "", title = "";
            GUI_Controls.RoboMessagePanel prompt = null;

            int[] arrItemsChecked = GetItemsChecked();
            if (arrItemsChecked == null || arrItemsChecked.Length == 0)
            {
                msg = LanguageINI.GetString("msgSelectReports");
                title = LanguageINI.GetString("headerSelectReports");
                prompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING, msg, title, LanguageINI.GetString("Ok"));
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                RoboSep_UserConsole.hideOverlay();
                prompt.Dispose();                
                return;
            }

            List<string> lstUSBDrives = AskUserToInsertUSBDrive();
            if (lstUSBDrives == null || lstUSBDrives.Count == 0)
            {
                return;
            }

            string USBDrive = "";
            // search for directory
            foreach (string drive in lstUSBDrives)
            {
                if (Directory.Exists(drive))
                {
                    USBDrive = drive;
                    break;
                }
            }

            if (string.IsNullOrEmpty(USBDrive))
            {
                msg = LanguageINI.GetString("msgUSBFail");
                prompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_ERROR, msg, LanguageINI.GetString("headerSaveUSB"), LanguageINI.GetString("Ok"));
                //RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                RoboSep_UserConsole.hideOverlay();

                //GUI_Controls.uiLog.LOG(this, "button_SaveReports_Click", GUI_Controls.uiLog.LogLevel.WARNING, "USB drive not detected");
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "USB drive not detected");
                prompt.Dispose();                
                return;
            }

            // see if save target directory exist
            string systemPath = RoboSep_UserDB.getInstance().sysPath;
            string reportsPath = systemPath + "reports\\";
            if (!Directory.Exists(reportsPath))
                Directory.CreateDirectory(reportsPath);

            string sTitle = LanguageINI.GetString("headerSaveReportUSB");
            FileBrowser SelectFolder = new FileBrowser(RoboSep_UserConsole.getInstance(), sTitle, USBDrive, "");
            SelectFolder.ShowDialog();
            if (SelectFolder.DialogResult != DialogResult.Yes)
            {
                RoboSep_UserConsole.hideOverlay();
                SelectFolder.Dispose();
                return;
            }

            string Target = SelectFolder.Target;

            List<string> lstSourceFiles = new List<string>();

            // copy all selected folders to Target Directory
            FileInfo selectedReport;
            int nIndex = 0;
            for (int i = 0; i < arrItemsChecked.Length; i++)
            {
                int ReportNumber = arrItemsChecked[i];

                // open file browser to allow user to select specific file location
                selectedReport = myReportFiles[ReportNumber];

                nIndex = selectedReport.Name.LastIndexOf('.');
                if (nIndex < 0)
                    return;

                string sFileName = selectedReport.Name.Substring(0, nIndex);
                sFileName += ".pdf";

                string sFullFileName = selectedReport.DirectoryName;
                if (sFullFileName.LastIndexOf('\\') != sFullFileName.Length - 1)
                {
                    sFullFileName += "\\";
                }

                sFullFileName += sFileName;
                if (File.Exists(sFullFileName))
                {
                    lstSourceFiles.Add(sFullFileName);
                }
            }

            // Copy files
            SelectFolder.ShowProgressBarWhileCopying = true;
            SelectFolder.CopyToTargetDirEx(lstSourceFiles.ToArray(), Target);
            SelectFolder.Dispose();
        }


        private List<string> AskUserToInsertUSBDrive()
        {
            List<string> lstUSBDrives = Utilities.GetUSBDrives();
            if (lstUSBDrives == null)
                return null;

            if (lstUSBDrives.Count > 0)
                return lstUSBDrives;

            // show dialog prompting user to insert USB key
            string msg = LanguageINI.GetString("SaveReport");
            GUI_Controls.RoboMessagePanel prompt = new GUI_Controls.RoboMessagePanel(
                RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_INFORMATION, msg, LanguageINI.GetString("headerSaveReport"),
                LanguageINI.GetString("Yes"), LanguageINI.GetString("Cancel"));
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

        private void listView_robostat_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            System.Drawing.Drawing2D.LinearGradientBrush GradientBrush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.Blue, Color.LightBlue, 270);
            SolidBrush theBrush1, theBrush2;
            
            theBrush1 = new SolidBrush(Color.White);
            theBrush2 = new SolidBrush(Color.Yellow);
            Pen thePen = new Pen(theBrush1, 2);
            Font theFont = new Font("Arial", 12); 
            e.Graphics.FillRectangle(GradientBrush, e.Bounds);

            e.Graphics.DrawRectangle(thePen, e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2);

            e.Graphics.DrawString(e.Header.Text, theFont, theBrush2, e.Bounds);

            theFont.Dispose();
            thePen.Dispose();
            theBrush1.Dispose();
            theBrush2.Dispose();
        }

        private void listView_robostat_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if (listView_robostat.Items.Count == 0)
                return;

            SolidBrush theBrush;

            if (e.Item.Selected)
            {
                theBrush = new SolidBrush(BG_Selected);
                e.Graphics.FillRectangle(theBrush, e.Bounds);
                theBrush.Dispose();
            }
            else if (e.ItemIndex % 2 == 0)
            {
                theBrush = new SolidBrush(BG_ColorEven);
                e.Graphics.FillRectangle(theBrush, e.Bounds);
                theBrush.Dispose();
            }
            else
            {
                theBrush = new SolidBrush(BG_ColorOdd);
                e.Graphics.FillRectangle(theBrush, e.Bounds);
                theBrush.Dispose();
            }

            Rectangle itemBounds = new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width, e.Bounds.Bottom - e.Bounds.Top - 1);

            // draw checkboxes in this column if required
            if (e.Item.ListView.CheckBoxes)
                itemBounds = DrawCheckBox(e.Graphics, itemBounds, e.Item.Checked);

        }

        private void listView_robostat_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            if (listView_robostat.Items.Count == 0)
                return;

            int tw = CHECKBOX_SIZE + (CellPaddingSize * 2) + EndPaddingSize;
  
            // format text
            int XStartPos1 = tw + listView_robostat.Location.X ;
            int XStartPos2 = listView_robostat.Location.X + 2;

            int XPos = 0, nWidth = 0;
            Rectangle textBounds;
            SolidBrush theBrush;
            if (e.Item.Selected)
            {
                for (int i = 0; i < e.Item.SubItems.Count; i++)
                {
                    theBrush = new SolidBrush(Color.White);
                    nWidth = i == 0 ? listView_robostat.Columns[i].Width - tw : listView_robostat.Columns[i].Width;
                    XPos = i == 0 ? XStartPos1 : i == 1 ? XStartPos2 + listView_robostat.Columns[i - 1].Width : XPos + +listView_robostat.Columns[i - 1].Width;
                    textBounds = new Rectangle(XPos, e.Bounds.Y, nWidth, e.Bounds.Height);
                    e.Graphics.DrawString(e.Item.SubItems[i].Text, listView_robostat.Font, theBrush, textBounds, theFormat);
                    theBrush.Dispose();
                }
            }
            else
            {
                for (int i = 0; i < e.Item.SubItems.Count; i++)
                {
                    theBrush = new SolidBrush(Txt_Color);
                    nWidth = i == 0 ? listView_robostat.Columns[i].Width - tw : listView_robostat.Columns[i].Width;
                    XPos = i == 0 ? XStartPos1 : i == 1 ? XStartPos2 + listView_robostat.Columns[i - 1].Width : XPos + +listView_robostat.Columns[i - 1].Width;
                    textBounds = new Rectangle(XPos, e.Bounds.Y, nWidth, e.Bounds.Height);
                    e.Graphics.DrawString(e.Item.SubItems[i].Text, listView_robostat.Font, theBrush, textBounds, theFormat);
                    theBrush.Dispose();
                }
            }
        }


        private Rectangle DrawCheckBox(Graphics graphics, Rectangle rectCell, bool bChecked)
        {
            int th, ty, tw, tx;

            th = CHECKBOX_SIZE + (CellPaddingSize * 2);
            tw = CHECKBOX_SIZE + (CellPaddingSize * 2);

            if ((tw > rectCell.Width) || (th > rectCell.Height))
                return rectCell;					// not enough room to draw the image, bail out

            ty = rectCell.Y + ((rectCell.Height - th) / 2);
            tx = rectCell.X + CellPaddingSize / 2;

            if (bChecked)
                graphics.DrawImage(this.imageList1.Images[1], tx, ty);
            else
                graphics.DrawImage(this.imageList1.Images[0], tx, ty);

            // remove the width that we used for the graphic from the cell
            rectCell.Width -= (CHECKBOX_SIZE + (CellPaddingSize * 2));
            rectCell.X += tw;
            return rectCell;
        }

    }
}
