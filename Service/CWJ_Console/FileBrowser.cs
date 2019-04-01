using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

using GUI_Controls;

namespace GUI_Console
{
    public partial class FileBrowser : Form
    {
        // declare variables
        private string sourceFolder = "G:\\Protocols\\";
        private string destinationFolder = "C:\\JasonB\\Protocols\\";
        private string[] fExtension = {"*.xml"};
        private ImageList FileIcons;
        private string prevDirectory;
        private BrowseResult CompletionAction;
        public Point Offset;
        private bool SearchDirsOnly;

        public enum BrowseResult
        {
            Copy,
            SelectTargetDir,
            Open, 
            Delete
        };

        public FileBrowser(Form prevForm,  string source, string destination, string[] extension, BrowseResult complete)
        {
            InitializeComponent();
            this.Region = roboPanel_REGION.Region;
            sourceFolder = source;
            destinationFolder = destination;
            fExtension = extension;
            CompletionAction = complete;
            int x = (640 - this.Size.Width) / 2;
            int y = (480 - this.Size.Height) / 2;
            Offset = new Point(x, y);
            this.Location = new Point(prevForm.Location.X + x, prevForm.Location.Y + y);

            SearchDirsOnly = false;

            // LOG
            string logMSG = "source: " + source + "destination: " + destination + "mode: " + complete.ToString();
            GUI_Controls.uiLog.LOG(this, "FileBrowser", GUI_Controls.uiLog.LogLevel.INFO, logMSG);
        }

        public FileBrowser(Form prevForm, string startDir, string sourceFile)
        {
            InitializeComponent();
            this.Region = roboPanel_REGION.Region;
            sourceFolder = startDir;
            destinationFolder = sourceFile;
            fExtension = null;
            SearchDirsOnly = true;
            CompletionAction = BrowseResult.SelectTargetDir;
            int x = (640 - this.Size.Width) / 2;
            int y = (480 - this.Size.Height) / 2;
            Offset = new Point(x, y);
            this.Location = new Point(prevForm.Location.X + x, prevForm.Location.Y + y);

            labelFilterFor.Visible = false;
            textBox_fileExtension.Visible = false;

            // LOG
            string logMSG = "Start Dir: " + sourceFolder + "File Location: " + destinationFolder + "mode: " + CompletionAction.ToString();
            GUI_Controls.uiLog.LOG(this, "FileBrowser", GUI_Controls.uiLog.LogLevel.INFO, logMSG);
        }

        private void FileBrowser_Load(object sender, EventArgs e)
        {
            // set button graphics
            // back button
            List<Image> ilist = new List<Image>();
            ilist.Add(GUI_Controls.Properties.Resources.fbBack);
            ilist.Add(GUI_Controls.Properties.Resources.fbBack);
            ilist.Add(GUI_Controls.Properties.Resources.fbBack);
            ilist.Add(GUI_Controls.Properties.Resources.fbBack3);
            button_Back.ChangeGraphics(ilist);
            // up button
            ilist.Clear();
            ilist.Add(GUI_Controls.Properties.Resources.fbUp);
            ilist.Add(GUI_Controls.Properties.Resources.fbUp);
            ilist.Add(GUI_Controls.Properties.Resources.fbUp);
            ilist.Add(GUI_Controls.Properties.Resources.fbUp3);
            button_Up.ChangeGraphics(ilist);
            // refresh button
            ilist.Clear();
            ilist.Add(GUI_Controls.Properties.Resources.fbRefresh);
            ilist.Add(GUI_Controls.Properties.Resources.fbRefresh);
            ilist.Add(GUI_Controls.Properties.Resources.fbRefresh);
            ilist.Add(GUI_Controls.Properties.Resources.fbRefresh3);
            button_Refresh.ChangeGraphics(ilist);
            // ok and cancel buttons
            ilist.Clear();
            ilist.Add(GUI_Controls.Properties.Resources.fbButton0);
            ilist.Add(GUI_Controls.Properties.Resources.fbButton0);
            ilist.Add(GUI_Controls.Properties.Resources.fbButton0);
            ilist.Add(GUI_Controls.Properties.Resources.fbButton3);
            button_Cancel.ChangeGraphics(ilist);
            button_OK.ChangeGraphics(ilist);

            // set up list view and image list
            FileIcons = new ImageList();
            listView_Browser.LargeImageList = FileIcons;
            listView_Browser.SmallImageList = FileIcons;
            FileIcons.ImageSize = new Size(35, 35);

            // set file extension and file path labels

            if (!SearchDirsOnly)
            {
                string tempstring = fExtension[0];
                for (int i = 1; i < fExtension.Length; i++)
                    tempstring += ", " + fExtension[i];
                textBox_fileExtension.Text = tempstring;
            }

            LoadDirectory(this.sourceFolder);
        }

        private void showDirectoryPath( string path )
        {
            string[] dirs = path.Split(Path.DirectorySeparatorChar);
            int charMax = 18;
            int currentChars = 0;
            string displaypath = string.Empty;
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
                    displaypath += @"..\";
                    dirStartAt = i;
                    break;
                }
            }
            for (int j = dirStartAt; j < dirs.Length; j++)
            {
                displaypath += dirs[j] + "\\";
            }
            displaypath += "..";
            labelPath.Text = displaypath;
        }

        private void LoadDirectory(string directoryPath)
        {
            // LOG
            string logMSG = "Loading Directory '" + directoryPath;
            GUI_Controls.uiLog.LOG(this, "LoadDirectory", GUI_Controls.uiLog.LogLevel.DEBUG, logMSG);

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
                            iconForFile = Icon.ExtractAssociatedIcon(file.FullName);

                            // see if imagelist already contains icon for file extension
                            // if not, add icon for file type
                            if (!FileIcons.Images.ContainsKey(file.Extension))
                            {
                                FileIcons.Images.Add(file.Extension, iconForFile);
                                countExtensions++;
                            }
                            item.ImageKey = file.Extension;
                            listView_Browser.Items.Add(item);
                            listView_Browser.Items[count].ImageIndex = FileIcons.Images.IndexOfKey(file.Extension);
                            count += 1;
                        }
                    }

                    FileIcons.Images.Add(GUI_Controls.Properties.Resources.Folder);
                    // add sub directories last
                    foreach (System.IO.DirectoryInfo d in theSourceDirectory.GetDirectories())
                    {
                        item = new ListViewItem(d.Name, FileIcons.Images.Count - 1);
                        listView_Browser.Items.Add(item);
                    }
                }
                if (listView_Browser.Items.Count < 10)
                    columnHeader1.Width = 450;
                listView_Browser.EndUpdate();
            }
            catch
            {
                // LOG
                logMSG = "Error while loading directory '" + directoryPath;
                GUI_Controls.uiLog.LOG(this, "LoadDirectory", GUI_Controls.uiLog.LogLevel.ERROR, logMSG);
            }
        }

        private void CopyFiles()
        {
            for (int i = 0; i < listView_Browser.SelectedItems.Count; i++)
            {
                string filename = sourceFolder + listView_Browser.SelectedItems[i].Text;
                string destination = destinationFolder + "\\" + listView_Browser.SelectedItems[i].Text;
                if (File.Exists(filename) && Directory.Exists(destinationFolder))
                {
                    // check if file already exists in destination folder
                    if (File.Exists(destination))
                    {
                        string sMSG = "File '" + listView_Browser.SelectedItems[i].Text;
                        string sMSG2 = GUI_Controls.fromINI.getValue("GUI", "sMSGOverWrite");
                        sMSG2 = sMSG2 == null ? "' alraedy exists in destination folder. sMSG \nOverwrite file?" : sMSG2;
                        sMSG += sMSG2;
                        RoboMessagePanel prompt = new RoboMessagePanel(this, sMSG, "File Copy Exception", "Yes", "No");
                        RoboSep_UserConsole.getInstance().frmOverlay.Show();
                        prompt.ShowDialog();
                        RoboSep_UserConsole.getInstance().frmOverlay.Hide();
                        if (prompt.DialogResult == DialogResult.OK)
                        {
                            File.Delete(destination);
                            File.Copy(filename, destination);
                        }
                        else { }
                        // do nothing
                    }
                    // file doesn't alraedy exist, copy to destination
                    else
                    {
                        File.Copy(filename, destination);
                    }
                }
                else
                {
                    // prompt user that file copy has failed
                    
                    string sMSG = GUI_Controls.fromINI.getValue("GUI", "sMSGCopyFail1");
                    sMSG = sMSG == null ? "Failed to copy file '" : sMSG;
                    sMSG += listView_Browser.SelectedItems[i].Text + "'";
                    if (File.Exists(filename))
                    {
                        string sMSG2 = GUI_Controls.fromINI.getValue("GUI", "sMSGCopyFail2");
                        sMSG2 = sMSG2 == null ? " file does not exist in current directory" : sMSG2;
                        sMSG += sMSG2;
                    }
                        
                    if (!Directory.Exists(destinationFolder))
                    {
                        sMSG += ".  Destination folder '" + destinationFolder.ToString() + "' does not exist.";
                    }
                    RoboMessagePanel prompt = new RoboMessagePanel(this, sMSG);
                }
            }
            // return dialogue result OK
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public string Target
        {
            get
            {
                return sourceFolder;
            }
        }

        public void CopyToTargetDir( string copyFrom, string copyTo )
        {
            // show waiting dialog
            //string msg = "Please wait while the file is copied to your USB storage device";
            //RoboMessagePanel waiting = new RoboMessagePanel(this, msg, true, false);
            //waiting.Show();
            //waiting.BringToFront();
            try
            {
                //File.Copy(copyTo, sourceFolder);
                GLOBAL_FROM = copyFrom;
                GLOBAL_TO = copyTo;

                Thread copyThread = new Thread(new ThreadStart(this.CopyFileThread));
                copyThread.IsBackground = true;
                copyThread.Start();

                //waiting.closeDialogue(DialogResult.OK);
                string LOGmsg = string.Format("Copying file from: {0}\nCopying file to : {1}", copyFrom, copyTo);
                GUI_Controls.uiLog.LOG(this, "CopyToTargetDir", uiLog.LogLevel.EVENTS, LOGmsg);
            }
            catch
            {
                string LOGmsg = string.Format("Failed to copy file: {0} to detination folder", copyFrom);
                GUI_Controls.uiLog.LOG(this, "CopyToTargetDir", uiLog.LogLevel.ERROR, LOGmsg);
            }
        }

        public string GLOBAL_TO;
        public string GLOBAL_FROM;

        private void CopyFileThread()
        {
            File.Copy(GLOBAL_FROM, GLOBAL_TO);    
    
            bool FileCopied = false;
            do
            {
                FileCopied = TestFileCopy(GLOBAL_TO);
                Thread.Sleep(50);
            } while (!FileCopied);
        }

        private bool TestFileCopy(string sourceFile)
        {
            try
            {
                System.IO.File.Open(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                return true;
            }
            catch
            {
                return false;
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
                }
                
            }
            // LOG
            string logMSG = "Ok button clicked";
            GUI_Controls.uiLog.LOG(this, "button_OK_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            // do nothing

            // return dialog result CANCEL
            this.DialogResult = DialogResult.Cancel;
            this.Close();

            // LOG
            string logMSG = "Cancel button clicked";
            GUI_Controls.uiLog.LOG(this, "button_Cancel_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
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
            string logMSG = "Changed file filter to: ";
            for (int i = 0; i < fExtension.Length; i++)
                logMSG += fExtension[i] + " ";
            GUI_Controls.uiLog.LOG(this, "button_Cancel_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        private void button_Back_Click(object sender, EventArgs e)
        {
            if (prevDirectory != null && prevDirectory != string.Empty)
            {
                string temp = sourceFolder;
                sourceFolder = prevDirectory;
                prevDirectory = temp;

                LoadDirectory(sourceFolder);
            }
            // LOG
            string logMSG = "Back button clicked";
            GUI_Controls.uiLog.LOG(this, "button_Back_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        private void button_Up_Click(object sender, EventArgs e)
        {
            string[] dirs = sourceFolder.Split(Path.DirectorySeparatorChar);
            // move up one directory
            string ParentDirectory = string.Empty;
            ParentDirectory = dirs[0];
            for (int i = 1; i < dirs.Length - 1; i++)
            {
                ParentDirectory += "\\" + dirs[i] ;
            }
            prevDirectory = sourceFolder;
            sourceFolder = ParentDirectory;
            LoadDirectory(ParentDirectory);

            // LOG
            string logMSG = "Up Directory button clicked";
            GUI_Controls.uiLog.LOG(this, "button_Up_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        private void button_Refresh_Click(object sender, EventArgs e)
        {
            LoadDirectory(sourceFolder);
        }

        private void listView_Browser_MouseDoubleClick(object sender, MouseEventArgs e)
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
                }
            }
            // LOG
            string logMSG = "Item double clicked";
            GUI_Controls.uiLog.LOG(this, "listView_Browser_MouseDoubleClick", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
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
                // if item has file extension (signified by '.' in item name)
                if (listView_Browser.SelectedItems[i].Text.Split('.').Length > 1)
                {
                    // determined by BrowserResult settings
                }
                // a folder is selected
                else
                {
                    // go to folder directory
                    prevDirectory = sourceFolder;
                    if (sourceFolder[sourceFolder.Length - 1] != '\\')
                        sourceFolder += "\\";
                    sourceFolder += listView_Browser.SelectedItems[i].Text  + "\\";
                    LoadDirectory(sourceFolder);
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
            }
        }



    }


}
