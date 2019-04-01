using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

using Invetech.ApplicationLog;

using Tesla.Common;
using GUI_Controls;

namespace GUI_Console
{

    public delegate void CopyFileEventHandler(object sender, CopyFileEventArgs e);  
    public delegate void CopyFileEndEventHandler(object sender, CopyFileEventArgs e);  

    public partial class FileBrowser : Form
    {
        // declare variables
        private string sourceFolder = "G:\\Protocols\\";
        private string targetDestination = "C:\\JasonB\\Protocols\\";
        private string[] fExtension = {"*.xml"};

        private string tempFolderForFileRestoration;
        private List<string> lstOverWrittenFileNames = new List<string>();
        private bool keepOriginalFilesForRestoration = false;
        private bool enableFilesOverwritten = true;

        private ImageList FileIcons;
        private string prevDirectory;
        private BrowseResult CompletionAction;
        public Point Offset;
        private bool SearchDirsOnly;
        private string fileTarget;
        private DirectoriesSorter Sorter;

        private StringFormat theFormat;
        private Color BG_ColorEven;
        private Color BG_ColorOdd;
        private Color BG_Selected;
        private Color Txt_Color;

        private IniFile LanguageINI;
        private const int MAX_ITEMS_DISPLAYED = 10;

        RoboMessagePanel copyingFiles;
        bool showProgressBarWhileCopying = false;

        private Rectangle rcScrollBar;
        private Rectangle rcListViewFileBrowser;
        private int ListViewColumn1Width;
        private int SpacingBetweenLVAndScrollBar = 0;

        private System.Windows.Forms.ListViewItemSelectionChangedEventHandler lvItemSelectionChangeHandler;
        private CopyFileEndEventHandler CopyfileFinishedHandler;
        private CopyFileEventHandler CopyfileHandler;

        private CopyFileInfo copyFileInfo = new CopyFileInfo();

        public event CopyFileEventHandler CopyFile;
        public event CopyFileEndEventHandler CopyFileEnd;
        


        public enum BrowseResult
        {
            Copy,
            SelectTargetDir,
            SelectFile,
            Open, 
            Delete
        };
        public enum IOItemType
        {
            eUndefined,
            eFile,
            eDirectory
        };

        public FileBrowser(Form prevForm,  string title, string source, string destination, string[] extension, BrowseResult complete)
        {
            InitializeComponent();
            SuspendLayout();

            listView_Browser.MultiSelect = complete == BrowseResult.Copy;
            labelTitle.Text = title;            
            sourceFolder = source;
            targetDestination = destination;
            fExtension = extension;
            CompletionAction = complete;
            int x = (640 - this.Size.Width) / 2;
            int y = (480 - this.Size.Height) / 2;
            Offset = new Point(x, y);
            this.Location = new Point(prevForm.Location.X + x, prevForm.Location.Y + y);

            SearchDirsOnly = false;

            if ( prevForm.GetType() == typeof(RoboSep_UserConsole) )
            {
                setLabelTexts();
            }

            ResumeLayout();

            // LOG
            string logMSG = "source: " + source + "destination: " + destination + "mode: " + complete.ToString();
            //GUI_Controls.uiLog.LOG(this, "FileBrowser", GUI_Controls.uiLog.LogLevel.INFO, logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
        }

        public FileBrowser(Form prevForm, string title, string startDir, string sourceFile)
        {
            InitializeComponent();
            labelTitle.Text = title;
            sourceFolder = startDir;
            targetDestination = sourceFile;
            fExtension = null;
            SearchDirsOnly = true;
            CompletionAction = BrowseResult.SelectTargetDir;
            int x = (640 - this.Size.Width) / 2;
            int y = (480 - this.Size.Height) / 2;
            Offset = new Point(x, y);
            this.Location = new Point(prevForm.Location.X + x, prevForm.Location.Y + y);

            setLabelTexts();
            labelFilterFor.Visible = false;
            textBox_fileExtension.Visible = false;
            checkBox_SelectAll.Visible = false;
            label_SelectAll.Visible = false;

            // LOG
            string logMSG = "Start Dir: " + sourceFolder + "File Location: " + targetDestination + "mode: " + CompletionAction.ToString();
            //GUI_Controls.uiLog.LOG(this, "FileBrowser", GUI_Controls.uiLog.LogLevel.INFO, logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
        }

        private void FileBrowser_Load(object sender, EventArgs e)
        {
            this.SuspendLayout();
            // set button graphics
            // back button
            List<Image> ilist = new List<Image>();
            ilist.Add(Properties.Resources.fbBack_STD);
            ilist.Add(Properties.Resources.fbBack_OVER);
            ilist.Add(Properties.Resources.fbBack_OVER);
            ilist.Add(Properties.Resources.fbBack_CLICK);
            button_Back.ChangeGraphics(ilist);
            // up button
            ilist.Clear();
            ilist.Add(Properties.Resources.fbUp_STD);
            ilist.Add(Properties.Resources.fbUp_OVER);
            ilist.Add(Properties.Resources.fbUp_OVER);
            ilist.Add(Properties.Resources.fbUp_CLICK);
            button_Up.ChangeGraphics(ilist);
            // refresh button
            ilist.Clear();
            ilist.Add(Properties.Resources.fbRefresh_STD);
            ilist.Add(Properties.Resources.fbRefresh_OVER);
            ilist.Add(Properties.Resources.fbRefresh_OVER);
            ilist.Add(Properties.Resources.fbRefresh_CLICK);
            button_Refresh.ChangeGraphics(ilist);
            // New Dir button
            ilist.Clear();
            ilist.Add(Properties.Resources.fbNewDir_STD);
            ilist.Add(Properties.Resources.fbNewDir_OVER);
            ilist.Add(Properties.Resources.fbNewDir_OVER);
            ilist.Add(Properties.Resources.fbNewDir_Click);
            button_CreateDirectory.ChangeGraphics(ilist);
            // ok and cancel buttons
            ilist.Clear();
            ilist.Add(Properties.Resources.btnsmall_STD);
            ilist.Add(Properties.Resources.btnsmall_STD);
            ilist.Add(Properties.Resources.btnsmall_STD);
            ilist.Add(Properties.Resources.btnsmall_CLICK);
            button_Cancel.ChangeGraphics(ilist);
            button_OK.ChangeGraphics(ilist);

            // set up list view and image list
            FileIcons = new ImageList();
            FileIcons.ImageSize = new Size(32, 32);
            listView_Browser.LargeImageList = FileIcons;
            listView_Browser.SmallImageList = FileIcons;

            // set file extension and file path labels
            if (!SearchDirsOnly)
            {
                string tempstring = fExtension[0];
                for (int i = 1; i < fExtension.Length; i++)
                    tempstring += ", " + fExtension[i];
                textBox_fileExtension.Text = tempstring;
            }

            Sorter = new DirectoriesSorter();
            listView_Browser.ListViewItemSorter = Sorter;
            listView_Browser.Sorting = SortOrder.Ascending;

            LoadDirectory(this.sourceFolder);


            // set up listview drag scroll properties
            listView_Browser.LINE_SIZE = 14;
            listView_Browser.VERTICAL_PAGE_SIZE = 9;
            listView_Browser.NUM_VISIBLE_ELEMENTS = 9;
            listView_Browser.VisibleRow =9;

            scrollBar_FileBrowser.LargeChange = listView_Browser.VisibleRow - 1;
            listView_Browser.VScrollbar = scrollBar_FileBrowser;
            listView_Browser.UpdateScrollbar();
            listView_Browser.ResizeVerticalHeight(true);
            Rectangle rcScrollbar = this.scrollBar_FileBrowser.Bounds;
            this.scrollBar_FileBrowser.SetBounds(rcScrollbar.X, listView_Browser.Bounds.Y, rcScrollbar.Width, listView_Browser.Bounds.Height);
            rcScrollBar = this.scrollBar_FileBrowser.Bounds;
            rcListViewFileBrowser = listView_Browser.Bounds;
            SpacingBetweenLVAndScrollBar = rcScrollBar.X - (rcListViewFileBrowser.X + rcListViewFileBrowser.Width);
            ListViewColumn1Width = listView_Browser.Columns[0].Width;

            lvItemSelectionChangeHandler = new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(listView_Browser_ItemSelectionChanged);
            this.listView_Browser.ItemSelectionChanged += lvItemSelectionChangeHandler;

            // set up for drawing
            theFormat = new StringFormat();
            theFormat.Alignment = StringAlignment.Near;
            theFormat.LineAlignment = StringAlignment.Center;
            BG_ColorEven = Color.FromArgb(216, 217, 218);
            BG_ColorOdd = Color.FromArgb(243, 243, 243);
            BG_Selected = Color.FromArgb(78, 38, 131);
            Txt_Color = Color.FromArgb(95, 96, 98);

            UpdateScrollBarVisibility();

            this.ResumeLayout();
        }


        private void UpdateScrollBarVisibility()
        {
            int nScrollBarWidth = rcScrollBar.Width;
            bool bScrollBarVisible = true;

            listView_Browser.UpdateScrollbar();

            // Do not show the scroll bar if it is not needed
            if (this.listView_Browser.Items.Count > this.listView_Browser.VERTICAL_PAGE_SIZE)
            {
                this.listView_Browser.SetBounds(rcListViewFileBrowser.X, rcListViewFileBrowser.Y, rcListViewFileBrowser.Width, rcListViewFileBrowser.Height);
                this.listView_Browser.Columns[0].Width = ListViewColumn1Width;
                this.scrollBar_FileBrowser.Visible = true;
            }
            else
                bScrollBarVisible = false;

            if (!bScrollBarVisible)
            {
                this.listView_Browser.SetBounds(rcListViewFileBrowser.X, rcListViewFileBrowser.Y, rcListViewFileBrowser.Width + nScrollBarWidth + SpacingBetweenLVAndScrollBar, rcListViewFileBrowser.Height);
                int nCount = this.listView_Browser.Columns.Count;
                this.listView_Browser.Columns[0].Width = ListViewColumn1Width + nScrollBarWidth + SpacingBetweenLVAndScrollBar;
                this.scrollBar_FileBrowser.Visible = false;
            }
        }

        private void setLabelTexts()
        {
            if (LanguageINI == null)
                LanguageINI = GUI_Console.RoboSep_UserConsole.getInstance().LanguageINI;

            // set labels text
            labelFilterFor.Text = LanguageINI.GetString("lblFilter");
            label_SelectAll.Text = LanguageINI.GetString("lblAll");
            label_NewDirectory.Text = LanguageINI.GetString("lblNewDirectoryName");
            button_newDirAccept.Text = LanguageINI.GetString("lblAccept");
            button_OK.Text = LanguageINI.GetString("Select");
            button_Cancel.Text = LanguageINI.GetString("Cancel");

        }

        private void showDirectoryPath( string path )
        {
            string[] dirs = path.Split(Path.DirectorySeparatorChar);
            int charMax = 18;
            int currentChars = 0;
            string displaypath = string.Empty;
            StringBuilder showPath = new StringBuilder();
            int dirStartAt = 0;
            for (int i = dirs.Length-1; i >= 0; i--)
            {
                if ((currentChars + dirs[i].Length) < charMax)
                {
                    currentChars += dirs[i].Length;
                }
                // reached max chars
                else
                {
                    showPath.Append(@"..\");
                    //displaypath += @"..\";
                    dirStartAt = i;
                    break;
                }
            }
            for (int j = dirStartAt; j < dirs.Length; j++)
            {
                showPath.Append(dirs[j]);
                showPath.Append("\\");
                //displaypath += dirs[j] + "\\";
            }
            //displaypath += "..";
            showPath.Append("..");
            labelPath.Text = showPath.ToString(); //displaypath;
        }

        private void LoadDirectory(string directoryPath)
        {
            // LOG
            string logMSG = "Loading Directory '" + directoryPath;
            //GUI_Controls.uiLog.LOG(this, "LoadDirectory", GUI_Controls.uiLog.LogLevel.DEBUG, logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);

            // set up file directory info
            DirectoryInfo theSourceDirectory = new DirectoryInfo(sourceFolder);
            ListViewItem item;
            
            // clear current file list
            listView_Browser.BeginUpdate();
            listView_Browser.Items.Clear();

            // update file directory labl
            showDirectoryPath(directoryPath);

            try
            {
                if (Directory.Exists(sourceFolder))
                {
                    // for each file in USB directory \ protocols
                    // create a listview item and set its icon to that extracted
                    // from its file
                    int count = 0;
                    int countExtensions = -1;

                    if (!SearchDirsOnly)
                    // add files
                    for (int i = 0; i < fExtension.Length; i++)
                    {
                        foreach (System.IO.FileInfo file in theSourceDirectory.GetFiles(fExtension[i]))
                        {
                            Icon iconForFile = SystemIcons.WinLogo;

                            item = new ListViewItem(file.Name, 1);
                            item.Tag =  IOItemType.eFile;
                            iconForFile = Icon.ExtractAssociatedIcon(file.FullName);

                            // see if imagelist already contains icon for file extension
                            // if not, add icon for file type
                            if (!FileIcons.Images.ContainsKey(file.Extension))
                            {
                                FileIcons.Images.Add(file.Extension, iconForFile);
                                countExtensions++;
                            }
                            item.ImageKey = file.Extension;
                            item.ImageIndex = FileIcons.Images.IndexOfKey(file.Extension);

                            listView_Browser.Items.Add(item);
                            count++;
                        }
                    }
                
                    FileIcons.Images.Add(GUI_Controls.Properties.Resources.RoboFolder32);
                    // add sub directories last
                    foreach (System.IO.DirectoryInfo d in theSourceDirectory.GetDirectories())
                    {
                        item = new ListViewItem(d.Name, FileIcons.Images.Count - 1);
                        item.Tag = IOItemType.eDirectory;
                        listView_Browser.Items.Add(item);
                    }

                    if (0 < listView_Browser.Items.Count)
                    {
                        listView_Browser.Sorting = SortOrder.Ascending;
                        listView_Browser.Sort();
                    }
                }

                listView_Browser.EndUpdate();
            }
            catch (Exception ex)
            {
                // LOG
                LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);
            }
        }

        private void CopyFiles()
        {
            if (listView_Browser.SelectedItems.Count == 0)
                return;
            
            List<string> lstrFileNames = new List<string>();
            string filename;
            for (int i = 0; i < listView_Browser.SelectedItems.Count; i++)
            {
                filename = listView_Browser.SelectedItems[i].Text;
                lstrFileNames.Add(filename);
            }

            // Start lengthy process
            Thread lengthyProcessThread = new Thread(new ParameterizedThreadStart(CopyFilesProcess));
            lengthyProcessThread.IsBackground = true;  // so not to have stray running threads if the form is closed
            lengthyProcessThread.Start(lstrFileNames.ToArray());
        }


        void CopyFilesProcess(object pb)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke((MethodInvoker)delegate() { this.CopyFilesProcess(pb); });
                }
                else
                {
                    string[] aFileNames = pb as string[];
                    if (aFileNames == null || aFileNames.Length == 0)
                    {
                        // Close the message box
                        this.Invoke(
                            (MethodInvoker)delegate()
                            {
                                CloseWaitDialog();
                            }
                        );
                        return;
                    }

                    CopyFileEventArgs copyArgs = null;
                    copyArgs = new CopyFileEventArgs(aFileNames);
                    copyArgs.TotalFiles = aFileNames.Length;
                    if (showProgressBarWhileCopying)
                    {
                        SendToBack();
                        // show progress bar
                        string msg = LanguageINI.GetString("msgWaitCopyingFiles");
                        copyingFiles = new RoboMessagePanel(this, MessageIcon.MBICON_NOTAPPLICABLE, msg, true, true);
                        copyingFiles.Show();

                        if (CopyfileFinishedHandler == null)
                            CopyfileFinishedHandler = new CopyFileEndEventHandler(FileBrowser_CopyFileEnd);

                        if (CopyfileHandler == null)
                            CopyfileHandler = new CopyFileEventHandler(FileBrowser_CopyFile);

                        CopyFileEnd += CopyfileFinishedHandler;
                        CopyFile += CopyfileHandler;
                    }

                    if (sourceFolder.LastIndexOf('\\') != (sourceFolder.Length - 1))
                    {
                        sourceFolder += "\\";
                    }

                    lstOverWrittenFileNames.Clear();
                    bool bSkipShowMessage = false;
                    string filename;
                    DialogResult dlgResult = DialogResult.Cancel;
                    for (int i = 0; i < aFileNames.Length; i++)
                    {
                        if (string.IsNullOrEmpty(aFileNames[i]))
                            continue;

                        filename = sourceFolder + aFileNames[i];
                        string destination = targetDestination + aFileNames[i];
                        if (File.Exists(filename) && Directory.Exists(targetDestination))
                        {
                            // check if file already exists in destination folder
                            if (File.Exists(destination))
                            {
                                if (!bSkipShowMessage)
                                {
                                    // set up message prompt
                                    string sMSG = "File '" + filename;
                                    string sMSG2;
                                    RoboMessagePanel2 prompt = null;
                                    if (enableFilesOverwritten)
                                    {
                                        sMSG2 = LanguageINI.GetString("msgOverWrite");
                                        sMSG += sMSG2;
                                        prompt = new RoboMessagePanel2(this, MessageIcon.MBICON_WARNING, sMSG, LanguageINI.GetString("CopyFailHeader"), LanguageINI.GetString("Yes"), LanguageINI.GetString("No"), true, LanguageINI.GetString("SkipForAllConflicts"));
                                        RoboSep_UserConsole.showOverlay();
                                    }
                                    else
                                    {
                                        sMSG2 = LanguageINI.GetString("msgNoOverWrite");
                                        sMSG += sMSG2;
                                        prompt = new RoboMessagePanel2(this, MessageIcon.MBICON_WARNING, sMSG, LanguageINI.GetString("CopyFailHeader"), LanguageINI.GetString("Ok"), "", true, LanguageINI.GetString("SkipForAllConflicts"));
                                        RoboSep_UserConsole.showOverlay();
                                    }

                                    //prompt user
                                    prompt.ShowDialog();
                                    RoboSep_UserConsole.hideOverlay();
                                    bSkipShowMessage = prompt.CheckBoxChecked;
                                    dlgResult = prompt.DialogResult;
                                    prompt.Dispose();
                                }

                                if (dlgResult == DialogResult.OK && enableFilesOverwritten)
                                {
                                    bool bSkipDelete = false;
                                    // Copy to a temp folder
                                    if (keepOriginalFilesForRestoration && !string.IsNullOrEmpty(tempFolderForFileRestoration))
                                    {
                                        if (!Directory.Exists(tempFolderForFileRestoration))
                                        {
                                            Directory.CreateDirectory(tempFolderForFileRestoration);
                                        }

                                        if (Directory.Exists(tempFolderForFileRestoration))
                                        {
                                            string temp = tempFolderForFileRestoration;
                                            if (temp.LastIndexOf('\\') != temp.Length - 1)
                                            {
                                                temp += "\\";
                                            }

                                            temp += aFileNames[i];

                                            // check if file exists
                                            if (!File.Exists(temp))
                                            {
                                                File.Move(destination, temp);
                                                lstOverWrittenFileNames.Add(temp);
                                                bSkipDelete = true;
                                            }
                                        }
                                    }

                                    if (!bSkipDelete)
                                        File.Delete(destination);

                                    File.Copy(filename, destination);
                                    if (copyArgs != null)
                                    {
                                        if (copyArgs.DictFileNames.ContainsKey(aFileNames[i]))
                                        {
                                            copyArgs.DictFileNames[filename] = true;
                                            copyArgs.FileNumber += 1;
                                        }
                                        OnCopyFile(copyArgs, bSkipShowMessage);
                                    }

                                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Warning, "Overwriting file '" + destination + "'");
                                }
                                else
                                {
                                    if (copyArgs != null)
                                        copyArgs.TotalFiles--;
                                }
                            }
                            // file doesn't already exist, copy to destination
                            else
                            {
                                File.Copy(filename, destination);

                                if (copyArgs != null)
                                {
                                    if (copyArgs.DictFileNames.ContainsKey(aFileNames[i]))
                                    {
                                        copyArgs.DictFileNames[filename] = true;
                                        copyArgs.FileNumber += 1;
                                    }
                                    OnCopyFile(copyArgs);
                                }
                            }
                        }
                        else
                        {
                            // prompt user that file copy has failed
                            string sTitle = LanguageINI.GetString("headerCopyFail");
                            string sMSG = LanguageINI.GetString("msgCopyFail1");
                            sMSG += aFileNames[i] + "'";
                            if (File.Exists(filename))
                            {
                                string sMSG2 = LanguageINI.GetString("msgCopyFail2");
                                sMSG += sMSG2;
                            }

                            if (!Directory.Exists(targetDestination))
                            {
                                sMSG += LanguageINI.GetString("msgCopyFail3") + targetDestination.ToString() + LanguageINI.GetString("msgCopyFail4");
                            }
                            RoboMessagePanel prompt = new RoboMessagePanel(this, MessageIcon.MBICON_ERROR, sMSG, sTitle, LanguageINI.GetString("Ok"));
                            RoboSep_UserConsole.showOverlay();
                            //prompt user
                            prompt.ShowDialog();
                            prompt.Dispose();
                            RoboSep_UserConsole.hideOverlay();
                        }
                    }
                    if (copyArgs != null)
                        OnCopyFileEnd(copyArgs);

                    // return dialogue result OK
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }

            }
            catch (Exception ex)
            {
                // LOG
                string logMSG = String.Format("Failed to copy file. Exception: {0}.", ex.Message);

                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
            }
        }
        
        public string[] OverWrittenFileNames
        {
            get
            {
                return lstOverWrittenFileNames.ToArray();
            }
        }

        public string TempFolderForFileRestoration
        {
            set
            {
                tempFolderForFileRestoration = value;
            }
        }

        public bool KeepOriginalFilesForRestoration
        {
            get
            {
                return keepOriginalFilesForRestoration;
            }
            set
            {
                keepOriginalFilesForRestoration = value;
            }
        }

        public bool EnableFilesOverwritten
        {
            get
            {
                return enableFilesOverwritten;
            }
            set
            {
                enableFilesOverwritten = value;
            }
        }

        public bool ShowProgressBarWhileCopying
        {
            get
            {
                return showProgressBarWhileCopying;
            }
            set
            {
                showProgressBarWhileCopying = value;
            }
        }

        public string Target
        {
            get
            {
                if (CompletionAction == BrowseResult.SelectFile)
                {
                    return fileTarget;
                }
                else
                {
                    string[] dirs = sourceFolder.Split('\\');
                    if (dirs[dirs.Length - 1] != string.Empty)
                        return (sourceFolder + "\\");
                    return sourceFolder;
                }
            }
        }

        public void CopyToTargetDirEx(string[] sourceFileNames, string destinationDir)
        {
            if (copyFileInfo == null)
            {
                copyFileInfo = new CopyFileInfo();
            }

            copyFileInfo.SourceFileNames = sourceFileNames;
            copyFileInfo.DestinationFolder = destinationDir;
            string files = string.Join(",", copyFileInfo.SourceFileNames);

            try
            {
                Thread copyThread = new Thread(new ThreadStart(this.CopyFileThread));
                copyThread.IsBackground = true;
                copyThread.Start();


                string LOGmsg = string.Format("Copying file from: {0}\nCopying file to : {1}", files, copyFileInfo.DestinationFolder);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, LOGmsg);
            }
            catch (Exception ex)
            {
                LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);
            }
        }

        public void CopyToTargetDir( string copyFrom, string copyTo )
        {
            if (string.IsNullOrEmpty(copyFrom) || string.IsNullOrEmpty(copyTo))
                return;

            try
            {
                if (copyFileInfo == null)
                {
                    copyFileInfo = new CopyFileInfo();
                }

                string[] sourceFileNames = new string[] { copyFrom };

                copyFileInfo.SourceFileNames = sourceFileNames;
                copyFileInfo.DestinationFolder = copyTo;

                Thread copyThread = new Thread(new ThreadStart(this.CopyFileThread));
                copyThread.IsBackground = true;
                copyThread.Start();

                //waiting.closeDialogue(DialogResult.OK);
                string LOGmsg = string.Format("Copying file from: {0}\nCopying file to : {1}", copyFrom, copyTo);
                //GUI_Controls.uiLog.LOG(this, "CopyToTargetDir", uiLog.LogLevel.EVENTS, LOGmsg);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, LOGmsg);
            }
            catch (Exception ex)
            {
                LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);
            }
        }

        private void CopyFileThread()
        {
            if (copyFileInfo == null)
                return;

            // remove illegal characters at the beginning and the end
            string targetDestination = Utilities.RemoveIllegalCharsInDirectory(copyFileInfo.DestinationFolder);
            if (targetDestination.LastIndexOf('\\') != targetDestination.Length - 1)
            {
                targetDestination += "\\";
            }

            CopyFileEventArgs copyArgs = null;
            if (showProgressBarWhileCopying)
            {
                copyArgs = new CopyFileEventArgs(copyFileInfo.SourceFileNames);
                copyArgs.TotalFiles = copyFileInfo.SourceFileNames.Length;

                // show loading protocol message
                string msg = LanguageINI.GetString("msgWaitCopyingFiles");
                copyingFiles = new RoboMessagePanel(this, MessageIcon.MBICON_NOTAPPLICABLE, msg, true, true);
                RoboSep_UserConsole.showOverlay();
                copyingFiles.Show();
                copyingFiles.Refresh();
                RoboSep_UserConsole.hideOverlay();

                if (CopyfileFinishedHandler == null)
                    CopyfileFinishedHandler = new CopyFileEndEventHandler(FileBrowser_CopyFileEnd);

                if (CopyfileHandler == null)
                    CopyfileHandler = new CopyFileEventHandler(FileBrowser_CopyFile);

                CopyFileEnd += CopyfileFinishedHandler;
                CopyFile += CopyfileHandler;
            }

            lstOverWrittenFileNames.Clear();
            bool bSkipShowMessage = false;
            DialogResult dlgResult = DialogResult.Cancel;

            string sSourceFile, sDestinationFile = String.Empty;
            for (int i = 0; i < copyFileInfo.SourceFileNames.Length; i++)
            {
                if (string.IsNullOrEmpty(copyFileInfo.SourceFileNames[i]))
                    continue;

                // remove illegal characters at the beginning and the end
                sSourceFile = Utilities.RemoveIllegalCharsInDirectory(copyFileInfo.SourceFileNames[i]);
                bool bCopyFile = true;
                try
                {
                    FileInfo fileInfo = new FileInfo(sSourceFile);
                    sDestinationFile = targetDestination + fileInfo.Name;
                    if (File.Exists(sDestinationFile))
                    {
                        if (LanguageINI == null)
                        {
                            LanguageINI = GUI_Console.RoboSep_UserConsole.getInstance().LanguageINI;
                        }
                        if (!bSkipShowMessage)
                        {
                            // set up message prompt
                            string sMSG = "File '" + sDestinationFile;
                            string sMSG2;
                            RoboMessagePanel2 prompt = null;
                            if (enableFilesOverwritten)
                            {
                                sMSG2 = LanguageINI.GetString("msgOverWrite");
                                sMSG += sMSG2;
                                prompt = new RoboMessagePanel2(this, MessageIcon.MBICON_WARNING, sMSG, LanguageINI.GetString("CopyFailHeader"), LanguageINI.GetString("Yes"), LanguageINI.GetString("No"), true, LanguageINI.GetString("SkipForAllConflicts"));
                                RoboSep_UserConsole.showOverlay();
                            }
                            else
                            {
                                sMSG2 = LanguageINI.GetString("msgNoOverWrite");
                                sMSG += sMSG2;
                                prompt = new RoboMessagePanel2(this, MessageIcon.MBICON_WARNING, sMSG, LanguageINI.GetString("CopyFailHeader"), LanguageINI.GetString("Ok"), "", true, LanguageINI.GetString("SkipForAllConflicts"));
                                RoboSep_UserConsole.showOverlay();
                            }

                            if (copyingFiles != null)
                                copyingFiles.Hide();

                            //prompt user
                            prompt.ShowDialog();
                            RoboSep_UserConsole.hideOverlay();

                            if (copyingFiles != null)
                            {
                                copyingFiles.Show();
                                copyingFiles.Refresh();
                            }

                            bSkipShowMessage = prompt.CheckBoxChecked;
                            dlgResult = prompt.DialogResult;
                            prompt.Dispose();
                        }
                        if (dlgResult == DialogResult.OK)
                        {
                            File.Delete(sDestinationFile);
                        }
                        else
                        {
                            bCopyFile = false;
                            copyArgs.TotalFiles--; 
                        }
                    }

                    bool bSuccess = false;
                    if (bCopyFile)
                    {
                        File.Copy(fileInfo.FullName, sDestinationFile);

                        bool FileCopied = false;
                        int nCount = 0;
                        do
                        {
                            // Check whether file can be opened. Max. tries = 3
                            FileCopied = TestFileCopy(sDestinationFile);
                            Thread.Sleep(50);
                            nCount++;
                        } while (!FileCopied && nCount <= 3);

                        if (nCount <= 3)
                        {
                            bSuccess = true;
                        }
                        if (copyArgs != null)
                        {
                            if (copyArgs.DictFileNames.ContainsKey(fileInfo.FullName))
                            {
                                copyArgs.DictFileNames[fileInfo.FullName] = bSuccess;
                                copyArgs.FileNumber += 1;
                            }

                            OnCopyFile(copyArgs);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // LOG
                    string logMSG = String.Format("Failed to copy source file {0} to  destination file {1}. Exception: {2}.", copyFileInfo.SourceFileNames[i], sDestinationFile, ex.Message);

                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
                }
            }

            OnCopyFileEnd(copyArgs);

        }

        private bool TestFileCopy(string sourceFile)
        {
            try
            {
                System.IO.File.Open(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                return true;
            }
            catch (Exception ex)
            {
                // LOG
                string logMSG = String.Format("Failed to open file {0}. Exception: {1}.", sourceFile, ex.Message);

                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
                return false;
            }
        }

        protected void OnCopyFile(CopyFileEventArgs e, bool skipMessage)
        {
            if (CopyFile != null)
            {
                if (copyingFiles != null)
                {
                    if (!skipMessage || !copyingFiles.Refreshed)
                    {
                        copyingFiles.Refresh();
                        if (skipMessage)
                            copyingFiles.Refreshed = true;
                    }
                }
                CopyFile(this, e);
            }
        }

        protected void OnCopyFile(CopyFileEventArgs e)
        {
            OnCopyFile(e, true);
        }

        protected void OnCopyFileEnd(CopyFileEventArgs e)
        {
            if (CopyFileEnd != null)
            {
                CopyFileEnd(this, e);
            }
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            // file copy / move / delete etc action
            if (selectItems())
            {
                switch (CompletionAction)
                {
                    case BrowseResult.Copy:
                        CopyFiles();
                        break;                    
                    case BrowseResult.Delete:
                        //...
                        break;
                    case BrowseResult.Open:
                        //...
                        break;
                    case BrowseResult.SelectFile:
                        this.DialogResult = DialogResult.OK;
                        break;
                }
                
            }
            // LOG
            string logMSG = "FileBrowser Ok button clicked";
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            // do nothing

            // return dialog result CANCEL
            this.DialogResult = DialogResult.Cancel;
            this.Close();

            // LOG
            string logMSG = "File Browser Cancel button clicked";
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
        }

        private void textBox_fileExtension_TextChanged(object sender, EventArgs e)
        {
            string temp = textBox_fileExtension.Text;
            if (temp != string.Empty)
            {
                // break string up to allow for multiple file types
                string[] extensions = temp.Split(' ', ',');
                List<string> ext = new List<string>();
                for (int i = 0; i < extensions.Length; i++)
                {
                    if (extensions[i] != string.Empty && extensions[i].Length > 1)
                    {
                        if (extensions[i][0] != '.')
                            temp = "*." + extensions[i];
                        else
                            temp = "*" + extensions[i];
                        ext.Add(temp);
                    }
                }
                fExtension = new string[ext.Count];
                for (int i = 0; i < ext.Count; i++)
                {
                    fExtension[i] = ext[i];
                }
            }
            // LOG
            string logMSG = "FileBrowser: Changed file filter to: ";
            for (int i = 0; i < fExtension.Length; i++)
                logMSG += fExtension[i] + " ";
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
        }

        private void button_Back_Click(object sender, EventArgs e)
        {
            if (prevDirectory != null && prevDirectory != string.Empty)
            {
                string temp = sourceFolder;
                sourceFolder = prevDirectory;
                prevDirectory = temp;

                LoadDirectory(sourceFolder);

                UpdateScrollBarVisibility();
            }
            // LOG
            string logMSG = "FileBrowser: Back button clicked";
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
        }

        private void button_Up_Click(object sender, EventArgs e)
        {
            string[] dirs = sourceFolder.Split(Path.DirectorySeparatorChar);
            // move up one directory
            //string ParentDirectory = string.Empty;
            StringBuilder parentDir = new StringBuilder();
            parentDir.Append(dirs[0]);
            //ParentDirectory = dirs[0];
            for (int i = 1; i < dirs.Length - 1; i++)
            {
                //ParentDirectory += "\\" + dirs[i] ;
                parentDir.Append("\\");
                parentDir.Append(dirs[i]);
            }
            prevDirectory = sourceFolder;
            sourceFolder = parentDir.ToString();  //ParentDirectory;
            LoadDirectory(sourceFolder);

            UpdateScrollBarVisibility();

            // LOG
            string logMSG = "FileBrowser: Up Directory button clicked";
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
        }

        private void button_Refresh_Click(object sender, EventArgs e)
        {
            LoadDirectory(sourceFolder);

            UpdateScrollBarVisibility();

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "FileBrowser: Refresh Button clicked");
        }

        private void listView_Browser_DoubleClick(object sender, EventArgs e)
        {
            if (selectItems())
            {
                switch (CompletionAction)
                {
                    case BrowseResult.Copy:
                        CopyFiles();
                        break;
                    case BrowseResult.Delete:
                        //...
                        break;
                    case BrowseResult.Open:
                        //...
                        break;
                    case BrowseResult.SelectFile:
                        this.DialogResult = DialogResult.OK;
                        break;
                }
            }
            // LOG
            string logMSG = "FileBrowser: Item double clicked";
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
        }

        private bool selectItems()
        {
             if (listView_Browser.SelectedItems.Count == 0 && CompletionAction == BrowseResult.SelectTargetDir)
            {
                // close 
                this.DialogResult = DialogResult.Yes;
                // CopyToTargetDir();
                return false;
            }

            // check that all that is selected are files (not folders)
            for (int i = 0; i < listView_Browser.SelectedItems.Count; i++)
            {
                ListViewItem lvItem = listView_Browser.SelectedItems[i];
                if (lvItem == null)
                    continue;

                IOItemType ioType = (IOItemType)lvItem.Tag;

                // if item is file type
                if (ioType == IOItemType.eFile)
                {
                    // determined by BrowserResult settings
                    if (CompletionAction == BrowseResult.SelectFile)
                    {
                        fileTarget = sourceFolder + "\\" + listView_Browser.SelectedItems[0].Text;
                    }
                }
                // a folder is selected
                else
                {
                    // go to folder directory
                    prevDirectory = sourceFolder;
                    if (sourceFolder[sourceFolder.Length - 1] != '\\')
                        sourceFolder += "\\";
                    sourceFolder += listView_Browser.SelectedItems[i].Text;
                    LoadDirectory(sourceFolder);
                    button_OK.Text = LanguageINI.GetString("Save");
                    UpdateScrollBarVisibility();
                    return false;
                }
            }
            return true;
        }

        private void textBox_fileExtension_KeyDown(object sender, KeyEventArgs e)
        {
            // check if key pressed is enter key
            if (e.KeyValue == 13)
            {
                string temp = textBox_fileExtension.Text;
                if (temp != string.Empty)
                {
                    // break string up to allow for multiple file types
                    string[] extensions = temp.Split(' ', ',');
                    List<string> ext = new List<string>();
                    for (int i = 0; i < extensions.Length; i++)
                    {
                        if (extensions[i] != string.Empty && extensions[i].Length > 1)
                        {
                            if (extensions[i][0] != '.')
                                temp = "*." + extensions[i];
                            else
                                temp = "*" + extensions[i];
                            ext.Add(temp);
                        }
                    }
                    fExtension = new string[ext.Count];
                    for (int i = 0; i < ext.Count; i++)
                    {
                        fExtension[i] = ext[i];
                    }
                }

                LoadDirectory(sourceFolder);

                UpdateScrollBarVisibility();
            }
        }

        private void button_CreateDirectory_Click(object sender, EventArgs e)
        {
            // create new directory
            robopanel_NewDir.Visible = true;
            robopanel_NewDir.BringToFront();
            
            string LOGmsg = "FileBrowser: New Directory button clicked";
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);
        }

        private void textBox_DirName_Click(object sender, EventArgs e)
        {
            if (RoboSep_UserConsole.KeyboardEnabled)
            {
                // Create new Keyboard control form
                GUI_Controls.Keyboard newKeyboard = GUI_Controls.Keyboard.getInstance(
                    this, textbox_DirName, null , RoboSep_UserConsole.getInstance().frmOverlay, false);
                newKeyboard.ShowDialog();
                newKeyboard.Dispose();
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "Generating touch keybaord");
            }
        }

        private void button_newDirAccept_Click(object sender, EventArgs e)
        {
            if (textbox_DirName.Text != string.Empty)
            {
                // hide new directory option box
                robopanel_NewDir.Visible = false;

                // set new folder name
                string newFolder = sourceFolder;
                newFolder += "\\" + textbox_DirName.Text;

                // check if folder already exists
                if (!Directory.Exists(newFolder))
                    Directory.CreateDirectory(newFolder);

                // store history
                prevDirectory = sourceFolder;
                sourceFolder = newFolder;

                // open new folder
                LoadDirectory(newFolder);

                UpdateScrollBarVisibility();

                LogFile.AddMessage(System.Diagnostics.TraceLevel.Warning, "Directory created '" + newFolder + "'");
            }
        }

        private void button_newDirCancel_Click(object sender, EventArgs e)
        {
            robopanel_NewDir.Visible = false;
        }

        private void listView_Browser_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            System.Drawing.Drawing2D.LinearGradientBrush GradientBrush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.Blue, Color.LightBlue, 270);
            SolidBrush theBrush = new SolidBrush(Color.Yellow);
            SolidBrush theBrush2 = new SolidBrush(Color.White);

            Pen thePen = new Pen(theBrush2, 2);
            Font theFont = new Font("Arial", 12);
            
            e.Graphics.FillRectangle(GradientBrush, e.Bounds);

            e.Graphics.DrawRectangle(thePen, e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2);

            e.Graphics.DrawString(e.Header.Text, theFont, theBrush, e.Bounds);

            theFont.Dispose();
            thePen.Dispose();
            theBrush.Dispose();
            theBrush2.Dispose();

        }

        private void listView_Browser_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            int w = listView_Browser.Width;

            SolidBrush theBrush; 

            if (e.Item.Selected)
            {
                theBrush = new SolidBrush(BG_Selected);
                e.Graphics.FillRectangle(theBrush, e.Bounds);
            }
            else if (e.ItemIndex % 2 == 0)
            {
                theBrush = new SolidBrush(BG_ColorEven);
                e.Graphics.FillRectangle(theBrush, e.Bounds);
            }
            else
            {
                theBrush = new SolidBrush(BG_ColorOdd);
                e.Graphics.FillRectangle(theBrush, e.Bounds);
            }
            theBrush.Dispose();
        }

        private void listView_Browser_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            // format text
             Rectangle TextRectangle = new Rectangle(e.Bounds.X + 34, e.Bounds.Y, e.Bounds.Width - 34, e.Bounds.Height);
             SolidBrush theBrush;
            if (e.Item.Selected)
            {
                theBrush = new SolidBrush(Color.White); 
                e.Graphics.DrawString(e.Item.Text, listView_Browser.Font, theBrush, TextRectangle, theFormat);
                theBrush.Dispose();
            }
            else
            {
                theBrush = new SolidBrush(Txt_Color);
                e.Graphics.DrawString(e.Item.Text, listView_Browser.Font, theBrush, TextRectangle, theFormat);
                theBrush.Dispose();
            }

            int imgIndex = e.Item.ImageIndex;
            if (0 <= imgIndex && imgIndex < FileIcons.Images.Count)
            {
                Image itemImage = FileIcons.Images[imgIndex];
                e.Graphics.DrawImage(itemImage, new Rectangle(e.Bounds.X, e.Bounds.Y, 32, 32));
            }
            
        }

        private void listView_Browser_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (listView_Browser.Items.Count == 0 )
                return;

            if (listView_Browser.SelectedItems.Count == 0)
            {
                button_OK.Text = LanguageINI.GetString("Select");
                return;
            }

            ListViewItem lvItem = e.Item;
            if (lvItem == null)
                return;

            IOItemType eType = (IOItemType)lvItem.Tag;
            ListViewItem lvItem1;
            IOItemType eType1;
            if (listView_Browser.SelectedItems.Count == 1)
            {
                button_OK.Text = (eType == IOItemType.eDirectory && listView_Browser.SelectedItems.Count == 1) ? LanguageINI.GetString("Open") : LanguageINI.GetString("Select");
            }
            else if (listView_Browser.SelectedItems.Count > 1)
            {
                if (eType == IOItemType.eFile)
                {
                    // Remove the directiories that are highlighted
                    this.listView_Browser.ItemSelectionChanged -= lvItemSelectionChangeHandler;
                    int[] array1 = new int[listView_Browser.SelectedIndices.Count];
                    listView_Browser.SelectedIndices.CopyTo(array1, 0);

                    for (int i = 0; i < array1.Length; i++)
                    {
                        lvItem1 = listView_Browser.Items[array1[i]];
                        eType1 = (IOItemType)lvItem1.Tag;

                        if (eType1 != IOItemType.eDirectory)
                            continue;

                        lvItem1.Selected = false;
                    }
                    this.listView_Browser.ItemSelectionChanged += lvItemSelectionChangeHandler;
                    button_OK.Text = LanguageINI.GetString("Select");
                }
                else if (eType == IOItemType.eDirectory)
                {
                   // Remove all highlight items except the most recent one
                    this.listView_Browser.ItemSelectionChanged -= lvItemSelectionChangeHandler;

                    int[] array1 = new int[listView_Browser.SelectedIndices.Count];
                    listView_Browser.SelectedIndices.CopyTo(array1, 0);

                    for (int i = 0; i < array1.Length; i++)
                    {
                        lvItem1 = listView_Browser.Items[array1[i]];

                        if (lvItem1.Text == lvItem.Text)
                            continue;

                        lvItem1.Selected = false;
                    }
                    this.listView_Browser.ItemSelectionChanged += lvItemSelectionChangeHandler;
                    button_OK.Text = LanguageINI.GetString("Open");
                }
            }
            else
            {
                // do nothing
            }
        }

        private void listView_Browser_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView_Browser.Items.Count == 0)
                return;

            if (listView_Browser.SelectedItems.Count == 0)
            {
                button_OK.Text = LanguageINI.GetString("Select");
                return;
            }

            ListViewItem lvItem = listView_Browser.SelectedItems[0];
            if (lvItem == null)
                return;

            IOItemType eType = (IOItemType)lvItem.Tag;
            button_OK.Text = (eType == IOItemType.eDirectory && listView_Browser.SelectedItems.Count == 1) ? LanguageINI.GetString("Open") : LanguageINI.GetString("Select");
        }

        private void checkBox_SelectAll_Click(object sender, EventArgs e)
        {
            if (listView_Browser.Items.Count == 0)
                return;

            // show spinner
            string title = LanguageINI.GetString("Wait");
            RoboMessagePanel4 spinnerPanel = new RoboMessagePanel4(RoboSep_UserConsole.getInstance(), title, 0, 0);
            RoboSep_UserConsole.showOverlay();
            spinnerPanel.ShowDialog();
            // check that all that is selected are files (not folders)
            for (int i = 0; i < listView_Browser.Items.Count; i++)
            {
                ListViewItem lvItem = listView_Browser.Items[i];
                if (lvItem == null)
                    continue;

                IOItemType ioType = (IOItemType)lvItem.Tag;

                // if item is file type
                if (ioType != IOItemType.eFile)
                {
                    if (ioType == IOItemType.eDirectory)
                        lvItem.Selected = false;
                    continue;
                }

                lvItem.Selected = checkBox_SelectAll.Checked;
            }
            spinnerPanel.Dispose();
        }

        private void FileBrowser_CopyFile(object sender, CopyFileEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate() { this.FileBrowser_CopyFile(sender, e); });
            }
            else
            {
                long TotalFiles = Math.Max(1, e.TotalFiles);
                long FileNumber = e.FileNumber;
                double completion = (double)FileNumber * 100.00 / (double)TotalFiles;
                if (copyingFiles != null)
                {
                    copyingFiles.setProgress((int)completion);
                }
            }
        }

        private void FileBrowser_CopyFileEnd(object sender, CopyFileEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate() { this.FileBrowser_CopyFileEnd(sender, e); });
            }
            else
            {
                CopyFileEnd -= CopyfileFinishedHandler;
                CopyFile -= CopyfileHandler;

                if (copyingFiles != null)
                {
                    copyingFiles.Close();
                    copyingFiles = null;
                }

                SendToBack();

                string LOGmsg = string.Format("Successfully copied {0} of {1} files", e.FileNumber, e.TotalFiles);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);

                // prompt user that file copy is complete
                string msg = LanguageINI.GetString("msgCopyComplete");
                msg += e.FileNumber.ToString() + " ";
                msg += LanguageINI.GetString("msgCopyComplete2");
                string header = LanguageINI.GetString("headerCopy");
                GUI_Controls.RoboMessagePanel CopyComplete = new GUI_Controls.RoboMessagePanel(
                    this, MessageIcon.MBICON_INFORMATION, msg, header, LanguageINI.GetString("Ok"));
                RoboSep_UserConsole.showOverlay();
                CopyComplete.ShowDialog();
                CopyComplete.Dispose();
                RoboSep_UserConsole.hideOverlay();
            }
        }


        private void CloseWaitDialog()
        {
            if (copyingFiles != null)
            {
                copyingFiles.Close();
                copyingFiles = null;
            }
        }
    }


    public class CopyFileInfo
    {
        private List<string> lstSourceFileNames = new List<string>();
        private string destinationFolder;

        public CopyFileInfo()
        {
            lstSourceFileNames.Clear();
            destinationFolder = String.Empty;
        }

        public CopyFileInfo(string[] SourceFileNames, string DestinationDir)
        {
            lstSourceFileNames.Clear();
            lstSourceFileNames.AddRange(SourceFileNames);
            destinationFolder = DestinationDir;
        }

        // Files Copied
        public string [] SourceFileNames 
        { 
            get { return lstSourceFileNames.ToArray();}

            set
            {
                if (value != null && value.Length > 0)
                {
                    lstSourceFileNames.Clear();
                    lstSourceFileNames.AddRange(value);
                }
            }
        }
        public string DestinationFolder 
        {
            get { return destinationFolder; }
            set { destinationFolder = value; }
         }
    }
         
    public class DirectoriesSorter : System.Collections.IComparer
    {
        int System.Collections.IComparer.Compare(Object o1, Object o2)
        {
            if (!(o1 is ListViewItem))
                return (0);

            if (!(o2 is ListViewItem))
                return (0);

            ListViewItem lvi1 = (ListViewItem)o1;
            FileBrowser.IOItemType c1 = (FileBrowser.IOItemType)lvi1.Tag;
 
            ListViewItem lvi2 = (ListViewItem)o2;
            FileBrowser.IOItemType c2 = (FileBrowser.IOItemType)lvi2.Tag;

            int result = 0;
            if (lvi1.ListView.Sorting == SortOrder.Ascending)
            {
                if (c1 == FileBrowser.IOItemType.eDirectory && c2 == FileBrowser.IOItemType.eDirectory)
                    result = lvi1.Text.CompareTo(lvi2.Text);
                else if (c1 == FileBrowser.IOItemType.eDirectory && c2 == FileBrowser.IOItemType.eFile)
                    result = -1;
                else if (c1 == FileBrowser.IOItemType.eFile && c2 == FileBrowser.IOItemType.eDirectory)
                    result = 1;
                else
                    result = lvi1.Text.CompareTo(lvi2.Text);
            }
            else if (lvi1.ListView.Sorting == SortOrder.Descending)
            {
                if (c1 == FileBrowser.IOItemType.eDirectory && c2 == FileBrowser.IOItemType.eDirectory)
                    result = lvi2.Text.CompareTo(lvi1.Text);
                else if (c1 == FileBrowser.IOItemType.eDirectory && c2 == FileBrowser.IOItemType.eFile)
                    result = 1;
                else if (c1 == FileBrowser.IOItemType.eFile && c2 == FileBrowser.IOItemType.eDirectory)
                    result = -1;
                else
                    result = lvi2.Text.CompareTo(lvi1.Text);
            }
            else
            {
                result = 0;
            }
            return result;
        }
    }   

}
