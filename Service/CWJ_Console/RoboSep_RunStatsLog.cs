using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Xml;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace GUI_Console
{
    public partial class RoboSep_RunStatsLog : BasePannel
    {
        private static RoboSep_RunStatsLog myRunLog;
        private FileInfo[] myReportFiles;
        private const int MAX_RUNS_DISPLAYED = 25;

        private struct ReportFiles
        {
            public string name;
            public string filepath;
            public string CompleteStatus;
            public int protocolsRun;
        }

        private RoboSep_RunStatsLog()
        {
            InitializeComponent();

            List<Image> ilist = new List<Image>();
            ilist.Add(GUI_Controls.Properties.Resources.Button_RECT_MED0);
            ilist.Add(GUI_Controls.Properties.Resources.Button_RECT_MED1);
            ilist.Add(GUI_Controls.Properties.Resources.Button_RECT_MED2);
            ilist.Add(GUI_Controls.Properties.Resources.Button_RECT_MED3);
            button_ViewReport.ChangeGraphics(ilist);
            button_SaveReports.ChangeGraphics(ilist);

            // LOG
            string logMSG = "Initializing Log Tab Run Statistics";
            GUI_Controls.uiLog.LOG(this, "RoboSep_RunStatsLog", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        private void button_Rectangle1_Load(object sender, EventArgs e)
        {
            LoadReportContent();
        }

        public static void RefreshList()
        {
            RoboSep_RunStatsLog.getInstance().LoadReportContent();
        }

        public static RoboSep_RunStatsLog getInstance()
        {
            
            if (myRunLog == null)
            {
                myRunLog = new RoboSep_RunStatsLog();
            }
            return myRunLog;
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
                    listView_robostat.Items.Add(string.Format("{0:HH:mm dd/MM/yy}", FileCreated));

                    listView_robostat.Items[i].SubItems.Add(UserName);

                    string tmp = QuadrantsUsed + " Protocol";
                    tmp = QuadrantsUsed == "1" ? tmp + "  - " : tmp + "s - ";
                    tmp += CompleteStatus.StartsWith("ABORTED") ? "ABORTED by User" : CompleteStatus;
                    listView_robostat.Items[i].SubItems.Add(tmp);
                }
            }
            listView_robostat.Refresh();
        }

        private void button_Rectangle1_Click(object sender, EventArgs e)
        {
            if (listView_robostat.SelectedIndices.Count > 0)
            {
                int index = listView_robostat.SelectedIndices[0];

                if (File.Exists(myReportFiles[index].FullName))
                {
                    System.Diagnostics.Process.Start( myReportFiles[index].FullName );                    
                }
            }
        }

        private void button_SaveReports_Click(object sender, EventArgs e)
        {
            // show dialog prompting user to insert USB key
            string msg = "Please enter your USB storage device.  ";
            msg += "USB drive entered?";
            GUI_Controls.RoboMessagePanel prompt = new GUI_Controls.RoboMessagePanel(
                RoboSep_UserConsole.getInstance(), msg, "Enter USB drive", "Yes", "Cancel");
            prompt.ShowDialog();


            // see if zip target directory exist
            string systemPath = RoboSep_UserDB.getInstance().sysPath;

            string reportsPath = systemPath + "reports\\";
            if (!Directory.Exists(reportsPath))
                Directory.CreateDirectory(reportsPath);

            // search for usb directory
            string[] USBdirs = new string[] { "D:\\", "E:\\", "F:\\" };
            int usbDirIndex = -1;
            for (int i = 0; i < USBdirs.Length; i++)
            {
                if (Directory.Exists(USBdirs[i]))
                {
                    usbDirIndex = i;
                    break;
                }
            }

            prompt.closeDialogue(DialogResult.OK);

            if (usbDirIndex > -1)
            {                    
                FileBrowser SelectFolder = new FileBrowser(RoboSep_UserConsole.getInstance(), USBdirs[usbDirIndex], "");
                SelectFolder.ShowDialog();
                string Target = SelectFolder.Target;

                // copy all selted folders to Target Directory
                for (int SelectedReports = 0; SelectedReports < listView_robostat.SelectedItems.Count; SelectedReports++)
                {
                    RoboSep_UserConsole.getInstance().frmOverlay.Hide();

                    // get target directory from file browser dialog
                    if (SelectFolder.DialogResult == DialogResult.Yes)
                    {
                        // open file browser to allow user to select specific file location
                        FileInfo selectedReport = myReportFiles[listView_robostat.SelectedIndices[SelectedReports]];
                            
                        // target including file name
                        string CopyTarget = Target + selectedReport.Name;

                        // Copy file
                        SelectFolder.CopyToTargetDir(selectedReport.FullName, CopyTarget);
                    }
                }
                string LOGmsg = string.Format("Successfully copied {0} report files to USB", listView_robostat.SelectedIndices.Count);
                GUI_Controls.uiLog.LOG(this, "button_SaveReports_Click", 
                    GUI_Controls.uiLog.LogLevel.EVENTS, LOGmsg);
            }
            else
            {
                msg = "USB drive not detected.  Enter USB and try again.";
                prompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(), msg, "USB Storage Not Detected", "Ok");
                prompt.ShowDialog();
                RoboSep_UserConsole.getInstance().frmOverlay.Hide();

                GUI_Controls.uiLog.LOG(this, "button_SaveReports_Click", GUI_Controls.uiLog.LogLevel.WARNING,
                    "USB drive not detected");
            }
        }
    }
}
