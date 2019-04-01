using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Tesla.Common;
using GUI_Controls;

namespace GUI_Console
{
    public partial class RoboSep_VideoLogs : BasePannel
    {
        private const string VIDEO_ERROR_LOG_PATH = "\\STI\\RoboSep\\logs\\videoerrorlog\\";
        private const string VIDEO_LOG_PATH = "\\STI\\RoboSep\\logs\\videolog\\";

        private static RoboSep_VideoLogs myVideoLogsPage;
        private IniFile LanguageINI;

        private RoboSep_VideoLogs()
        {
            InitializeComponent();
            LanguageINI = GUI_Console.RoboSep_UserConsole.getInstance().LanguageINI;
            logTabs.Tab1 = LanguageINI.GetString("tabVideoLog");
            logTabs.Tab2 = LanguageINI.GetString("tabVideoErrorLog");
            logTabs.Tab1_Click += new System.EventHandler(this.logTab1ClickHandler);
            logTabs.Tab2_Click += new System.EventHandler(this.logTab2ClickHandler);
            refreshVideoLogList();
            videoLogPanel.Visible = true;
            videoErrorLogPanel.Visible = false;
        }

        public static RoboSep_VideoLogs getInstance()
        {
            if (myVideoLogsPage == null)
            {
                myVideoLogsPage = new RoboSep_VideoLogs();
            }
            return myVideoLogsPage;
        }

        private void refreshVideoErrorLogList()
        {
            string folderPath = Utilities.ProgramFiles() + VIDEO_ERROR_LOG_PATH;
            string[] logsPaths = Directory.GetFiles(folderPath, "*.asf");
            refreshTableLayoutPanel(videoErrorLogPanel, logsPaths);
        }

        private void refreshVideoLogList()
        {
            string folderPath = Utilities.ProgramFiles() + VIDEO_LOG_PATH;
            string[] logsPaths = Directory.GetFiles(folderPath, "*.asf");
            refreshTableLayoutPanel(videoLogPanel, logsPaths);
        }

        private void refreshTableLayoutPanel()
        {
            if (logTabs.TabActive == HorizontalTabs.tabs.Tab1)
                refreshVideoLogList();
            else
                refreshVideoErrorLogList();
        }

        private void refreshTableLayoutPanel(TableLayoutPanel p, string[] logsPaths)
        {
            p.Controls.Clear();
            if (logsPaths != null && logsPaths.Length > 0)
            {
                p.RowCount = logsPaths.Length;
                for (int i = 0; i < logsPaths.Length; i++)
                {
                    TableLayoutPanel subPanel = new TableLayoutPanel();
                    subPanel.Width = 610;
                    subPanel.Height = 70;
                    subPanel.RowCount = 1;
                    subPanel.ColumnCount = 2;
                    CheckBox_Square cb = new CheckBox_Square();
                    cb.Width = 60;
                    cb.Height = 60;
                    Label label = new Label();
                    Padding lPadding = label.Padding;
                    lPadding.Top = 7;
                    label.Padding = lPadding;
                    label.Text = getLogFileName(logsPaths[i]);
                    label.Height = 70;
                    label.Width = 550;
                    label.Click += new System.EventHandler(this.labelClickHandler);
                    label.ForeColor = Color.FromArgb(95, 96, 98);
                    label.Font = new Font("Arial", 15);
                    subPanel.Controls.Add(cb, 0, 0);
                    subPanel.Controls.Add(label, 1, 0);
                    
                    p.Controls.Add(subPanel, 0, i);
                }
            }
            selectAllCheckBox.Check = false;
        }

        private void labelClickHandler(object sender, EventArgs e)
        {
            if (sender != null)
            {
                TableLayoutPanel p = (sender as Label).Parent as TableLayoutPanel;
                CheckBox_Square cb = p.GetControlFromPosition(0, 0) as CheckBox_Square;
                cb.Check = !cb.Check;
            }
        }

        private string getLogFileName(string path)
        {
            string fileName = null;
            if (path != null && path.Trim().Length > 0)
            {
                int idx = path.LastIndexOf("\\");
                if (idx >= 0)
                    return path.Substring(idx + 1);
            }
            return fileName;
        }

        private string getPathInSMIExt(string path)
        {
            string result = path.Substring(0, path.Length - 3) + "smi";
            return result;
        }

        private void logTab1ClickHandler(object sender, EventArgs e)
        {
            if (logTabs.TabActive == HorizontalTabs.tabs.Tab1)
                return;

            logTabs.TabActive = HorizontalTabs.tabs.Tab1;
            refreshVideoLogList();
            videoErrorLogPanel.Visible = false;
            videoLogPanel.Visible = true;
        }

        private void logTab2ClickHandler(object sender, EventArgs e)
        {
            if (logTabs.TabActive == HorizontalTabs.tabs.Tab2)
                return;

            logTabs.TabActive = HorizontalTabs.tabs.Tab2;
            refreshVideoErrorLogList();
            videoLogPanel.Visible = false;
            videoErrorLogPanel.Visible = true;
        }

        private List<string> getSelectedLogs()
        {
            List<string> paths = new List<string>();
            TableLayoutPanel p;
            string basePath = Utilities.ProgramFiles();
            if (logTabs.TabActive == HorizontalTabs.tabs.Tab1)
            {
                basePath += VIDEO_LOG_PATH;
                p = videoLogPanel;
            }
            else
            {
                basePath += VIDEO_ERROR_LOG_PATH;
                p = videoErrorLogPanel;
            }
            for (int i = 0; i < p.RowCount; i++)
            {
                TableLayoutPanel subPanel = p.GetControlFromPosition(0, i) as TableLayoutPanel;
                if ((subPanel.GetControlFromPosition(0, 0) as CheckBox_Square).Check)
                {
                    paths.Add(basePath + (subPanel.GetControlFromPosition(1, 0) as Label).Text);
                }
            }
            return paths;
        }

        private void showNoSelectionErrorMessage()
        {
            showGeneralErrorMessage("Please pick a log to proceed");
        }

        private void showNoUSBErrorMessage()
        {
            showGeneralErrorMessage("No Flash Drive is found");
        }

        private void showGeneralErrorMessage(string errMsg)
        {
            RoboMessagePanel dlg = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_ERROR,
                            errMsg, "Error", LanguageINI.GetString("Ok"));
            RoboSep_UserConsole.showOverlay();
            dlg.ShowDialog();
            dlg.Dispose();
            RoboSep_UserConsole.hideOverlay();
        }

        private void copyDeleteFiles(bool isCopy)
        {
            List<string> paths = getSelectedLogs();

            if (paths.Count > 0)
            {
                if (isCopy)
                {
                    List<string> lstUSBDrives = Utilities.GetUSBDrives();
                    if (lstUSBDrives != null && lstUSBDrives.Count > 0)
                    {
                        // determine usb directory
                        string USBpath = string.Empty;
                        try
                        {
                            for (int i = 0; i < lstUSBDrives.Count; i++)
                            {
                                if (Directory.Exists(lstUSBDrives[i]))
                                {
                                    USBpath = lstUSBDrives[i];
                                    break;
                                }
                            }
                            if (USBpath != string.Empty)
                            {
                                FileBrowser SelectFolder = new FileBrowser(RoboSep_UserConsole.getInstance(), "Please pick a destination folder", USBpath, "");
                                SelectFolder.ShowDialog();
                                if (SelectFolder.DialogResult == DialogResult.Yes)
                                {
                                    String destinationFolder = SelectFolder.Target;
                                    for (int i = 0; i < paths.Count; i++)
                                    {
                                        File.Copy(paths[i], destinationFolder + getLogFileName(paths[i]), true);
                                        File.Copy(getPathInSMIExt(paths[i]), destinationFolder + getPathInSMIExt(getLogFileName(paths[i])), true);
                                    }
                                }
                                SelectFolder.Dispose();
                            }
                        }
                        catch (Exception ex)
                        {
                            showGeneralErrorMessage(ex.Message);
                        }
                    }
                    else
                    {
                        showNoUSBErrorMessage();
                    }
                }
                else
                {
                    RoboMessagePanel dlg = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING,
                            "Are you sure to delete the selected logs?", "", LanguageINI.GetString("Yes"), LanguageINI.GetString("No"));
                    RoboSep_UserConsole.showOverlay();
                    dlg.ShowDialog();
                    RoboSep_UserConsole.hideOverlay();

                    if (dlg.DialogResult == DialogResult.OK)
                    {
                        for (int i = 0; i < paths.Count; i++)
                        {
                            File.Delete(paths[i]);
                            File.Delete(getPathInSMIExt(paths[i]));
                        }
                    }
                    dlg.Dispose();
                }
                refreshTableLayoutPanel();
            }
            else
                showNoSelectionErrorMessage();
        }

        private void copyButtonClickHandler(object sender, MouseEventArgs e)
        {
            copyDeleteFiles(true);
        }

        private void deleteButtonClickHandler(object sender, MouseEventArgs e)
        {
            copyDeleteFiles(false);
        }

        private void selectUnselectAll(bool isSelect)
        {
            TableLayoutPanel p;
            if (logTabs.TabActive == HorizontalTabs.tabs.Tab1)
                p = videoLogPanel;
            else
                p = videoErrorLogPanel;
            for (int i = 0; i < p.RowCount; i++)
            {
                TableLayoutPanel subPanel = p.GetControlFromPosition(0, i) as TableLayoutPanel;
                CheckBox_Square cb = subPanel.GetControlFromPosition(0, 0) as CheckBox_Square;
                cb.Check = isSelect;
            }
        }

        private void selectAllClickHandler(object sender, EventArgs e)
        {
            selectUnselectAll(selectAllCheckBox.Check);
        }
    }
}
