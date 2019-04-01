using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Serialization;

using Tesla.Common.ResourceManagement;

using GUI_Controls;

namespace GUI_Console
{
    public partial class RoboSep_ProtocolList : UserControl
    {
        // variable for UserProtocolList
        private RoboSep_UserProtocolList myUserList;
        private RoboSep_UserDB myDB;
        private List<RoboSep_Protocol> myProtocols;
        private List<RoboSep_Protocol> myFilterList;
        // search filters
        private string search;
        private bool H;
        private bool M;
        private bool P;
        private bool N;

        // Variable Declaration
        private static string myProtocolsPath = Application.StartupPath + "\\..\\protocols\\";
        private static string myXSDPath = Application.StartupPath + "\\..\\config\\RoboSepUser.xsd";
        private static RoboSep_ProtocolList myProtocolList;
        private RoboSep_Protocol[] selectedProtocols;

        private RoboSep_ProtocolList()
        {
            InitializeComponent();
            // load files to list from protocols

            // create events for when search buttons are pressed
            this.SearchBar.button_human.Click += new System.EventHandler(this.UpdateFilters);
            this.SearchBar.button_mouse.Click += new System.EventHandler(this.UpdateFilters);
            this.SearchBar.button_positive.Click += new System.EventHandler(this.UpdateFilters);
            this.SearchBar.button_negative.Click += new System.EventHandler(this.UpdateFilters);
            this.SearchBar.textBox_search.TextChanged += new System.EventHandler(this.UpdateFilters);
            this.SearchBar.textBox_search.Enter += new System.EventHandler(this.activateTextbox);
            this.SearchBar.textBox_search.MouseClick += new System.Windows.Forms.MouseEventHandler(this.mouseActivateTextbox);

            SuspendLayout();

            // change graphics for buttons
            List<Image> ilist = new List<Image>();
            // usb load button
            ilist.Add(GUI_Controls.Properties.Resources.Button_USBLOAD0);
            ilist.Add(GUI_Controls.Properties.Resources.Button_USBLOAD1);
            ilist.Add(GUI_Controls.Properties.Resources.Button_USBLOAD2);
            ilist.Add(GUI_Controls.Properties.Resources.Button_USBLOAD3);
            button_USBload.ChangeGraphics(ilist);
            // my profile button
            ilist.Clear();
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_MYPROFILE0);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_MYPROFILE1);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_MYPROFILE2);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_MYPROFILE3);
            button_myProfile.ChangeGraphics(ilist);
            // Add button
            ilist.Clear();
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_ADDPROFILE0);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_ADDPROFILE1);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_ADDPROFILE2);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_ADDPROFILE3);
            button_Add.ChangeGraphics(ilist);
            // save list button
            ilist.Clear();
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_SAVELIST0);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_SAVELIST1);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_SAVELIST2);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_SAVELIST3);
            button_SaveList.ChangeGraphics(ilist);

            ResumeLayout();

            // LOG
            string logMSG = "Initializing Protocol List user control";
            GUI_Controls.uiLog.LOG(this, "RoboSep_ProtocolList", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        public static RoboSep_ProtocolList getInstance()
        {
            if (myProtocolList == null)
                myProtocolList = new RoboSep_ProtocolList();
            return myProtocolList;
        }

        private void RoboSep_ProtocolList_Load(object sender, EventArgs e)
        {
            // grab userProtocolListform
            myUserList = RoboSep_UserProtocolList.getInstance();

            // load username from userConsole
            label_userName.Text = RoboSep_UserConsole.strCurrentUser;

            // load all users and protocols
            myDB = RoboSep_UserDB.getInstance();

            // get protocols
            myProtocols = myDB.getAllProtocols();
            myFilterList = new List<RoboSep_Protocol>();
            selectedProtocols = null;
            updateFilteredList();

            // add all protocols to listview
            refreshList();
        }

        public void resetForm()
        {
            // LOG
            string logMSG = "";
            GUI_Controls.uiLog.LOG(this, "resetForm", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

            // remove all filters
            SearchBar.button_human.Check = false;
            SearchBar.button_mouse.Check = false;
            SearchBar.button_negative.Check = false;
            SearchBar.button_positive.Check = false;
            SearchBar.textBox_search.Text = string.Empty;
            
            // set user name to current user
            label_userName.Text = RoboSep_UserConsole.strCurrentUser;

            // reset user list
            RoboSep_UserProtocolList.getInstance().resetForm();
            highlightItems();

            // load user protocols
            List<RoboSep_Protocol> tempList =
                RoboSep_UserDB.getInstance().loadUserProtocols(RoboSep_UserConsole.strCurrentUser);
            if (tempList != null)
            { RoboSep_UserProtocolList.getInstance().addProtocols( tempList); }
        }

        public void getFilters()
        {
            SearchBar.getFilters(out search, out H, out M, out P, out N);
        }

        private int findInList(string pString)
        {
            getFilters();
            if ((H || M) && (P||N))
            {
                // compare to string of SpeciesSelection in robo_protocol
                for (int i = 0; i < myProtocols.Count; i++)
                {
                    if (pString == myProtocols[i].SpeciesSelection_Name)
                        return i;
                }
            }
            else if ((H || M) && !(P || N))
            {
                // compare to string of Species in robo_protocol
                for (int i = 0; i < myProtocols.Count; i++)
                {
                    if (pString == myProtocols[i].Species_Name)
                        return i;
                }
            }
            else if (!(H || M) && (P || N))
            {
                // compare to string of selection in robo_protocol
                for (int i = 0; i < myProtocols.Count; i++)
                {
                    if (pString == myProtocols[i].Selection_Name)
                        return i;
                }
            }
            else
            {
                // compare to unfiltered name
                for (int i = 0; i < myProtocols.Count; i++)
                {
                    if (pString == myProtocols[i].Protocol_Name)
                        return i;
                }
            }
            return -1;
        }

        private void highlightItems()
        {
            Color bg_offcolour = Color.FromArgb(215, 209, 215);
            Color bg_colour = Color.White;
            Color SelectedColour = Color.FromArgb(232, 195, 247);
            Color SelectedColour2 = Color.FromArgb(213, 176, 228);
            for (int i = 0; i < lvProtocolList.Items.Count; i++)
            {
                if ((i % 2) == 1)
                {
                    lvProtocolList.Items[i].BackColor = bg_offcolour;
                }
                else
                {
                    lvProtocolList.Items[i].BackColor = bg_colour;
                }
            }
            // highlight items currently on users protocol list.
            List<RoboSep_Protocol> userList = RoboSep_UserProtocolList.getInstance().usrProtocols;
            for (int i = 0; i <userList.Count; i++)
            {
                for (int j = 0; j < myFilterList.Count; j++)
                {
                    if (myFilterList[j] == userList[i])
                    {
                        lvProtocolList.Items[j].BackColor = (j % 2) == 0 ? SelectedColour : SelectedColour2;
                    }
                }
            }
            
        }

        //UpdateFilters
        private void UpdateFilters(object sender, EventArgs e)
        {
            // LOG
            string logMSG = "Updating Filtered list";
            GUI_Controls.uiLog.LOG(this, "UpdateFilters", GUI_Controls.uiLog.LogLevel.DEBUG, logMSG);

            // update list
            updateFilteredList();
            
            // display filtered list^
            refreshList();
        }

        public void refreshList()
        {
            lvProtocolList.SuspendLayout();

            // determine which protocol name to display
            getFilters();
            if (H || M)
            {
                lvProtocolList.Font = new System.Drawing.Font("Arial Narrow", 13F);
                if (P || N)
                {
                    // show Species & Selection filtered name
                    lvProtocolList.Items.Clear();
                    for (int i = 0; i < myFilterList.Count; i++)
                        lvProtocolList.Items.Add(myFilterList[i].SpeciesSelection_Name);
                }
                else
                {
                    // show Species filtered name
                    lvProtocolList.Items.Clear();
                    for (int i = 0; i < myFilterList.Count; i++)
                        lvProtocolList.Items.Add(myFilterList[i].Species_Name);
                }
            }
            else if (P || N)
            {
                lvProtocolList.Font = new System.Drawing.Font("Arial Narrow", 13F);
                // show Selection filtered name
                lvProtocolList.Items.Clear();
                for (int i = 0; i < myFilterList.Count; i++)
                    lvProtocolList.Items.Add(myFilterList[i].Selection_Name);
            }
            else
            {
                lvProtocolList.Font = new System.Drawing.Font("Arial Narrow", 12F);
                // show un-filtered name
                lvProtocolList.Items.Clear();
                for (int i = 0; i < myFilterList.Count; i++)
                    lvProtocolList.Items.Add(myFilterList[i].Protocol_Name);
            }
            lvProtocolList.ResumeLayout(true);
            highlightItems();

            // reselect previously selected items
            if (selectedProtocols != null)
            for (int i = 0; i < selectedProtocols.Length; i++)
            {
                bool topIndexFound = false;
                for (int j = 0; j < lvProtocolList.Items.Count; j++)
                {
                    string[] names = new string[] { 
                        selectedProtocols[i].Protocol_Name,
                        selectedProtocols[i].Selection_Name,
                        selectedProtocols[i].Species_Name,
                        selectedProtocols[i].SpeciesSelection_Name };
                    for (int k = 0; k < names.Length; k++)
                    {
                        if (names[k] == lvProtocolList.Items[j].Text)
                        {
                            lvProtocolList.TopItem = lvProtocolList.Items[j];
                            lvProtocolList.Items[j].Selected = true;
                            topIndexFound = true;
                            break;
                        }
                    }
                    if (topIndexFound) { break; }
                }
                if (topIndexFound) { break; }
            }
        }

        private void updateFilteredList()
        {
            List<RoboSep_Protocol> tempList = new List<RoboSep_Protocol>();
            getFilters();
            bool[] currentFilters = new bool[] { H, M, P, N };
            List<int> filterIndex = new List<int>();
            if (H || M)
            {
                // filtered by species
                filterIndex.Add(0);
                filterIndex.Add(1);
            }
            if (P || N)
            {
                // filtered by selection type
                filterIndex.Add(2);
                filterIndex.Add(3);
            }

            if (!(H || M) && !(P || N))
            {
                myFilterList.Clear();
                for (int i = 0; i < myProtocols.Count; i++)
                    tempList.Add(myProtocols[i]);
            }
            else
            {
                // filter by button filters
                for (int i = 0; i < myProtocols.Count; i++)
                {
                    bool addThisProtocol = true;
                    for (int j = 0; j < filterIndex.Count; j++)
                    {
                        if (currentFilters[filterIndex[j]] != myProtocols[i].bfilters[j])
                        {
                            addThisProtocol = false;
                            break;
                        }
                    }
                    if (addThisProtocol)
                    {
                        tempList.Add(myProtocols[i]);
                    }
                }
            }

            myFilterList.Clear();
            // filter by search txt
            if (search == string.Empty || search == null)
            {
                for (int i = 0; i < tempList.Count; i++)
                    myFilterList.Add( tempList[i] );
            }
            else
            {
                // search all protocols after button filtering
                for (int i = 0; i < tempList.Count; i++)
                {
                    int searchResult = searchForTextInString(search, tempList[i].Protocol_Name);
                    if (searchResult != -1)
                    {
                        myFilterList.Add(tempList[i]);
                    }
                }
            }
        }

        private static int searchForTextInString(string search, string theString)
        {
            for (int i = 0; i < theString.Length; i++)
            {
                if (theString[i].ToString().ToUpper() == search[0].ToString().ToUpper())
                {
                    string temp = string.Empty;
                    for (int j = 0; j < search.Length; j++)
                    {
                        if ((i + j) < theString.Length)
                            temp += theString[i + j];
                    }
                    if (search.ToUpper() == temp.ToUpper())
                    {
                        return i;
                    }
                }
            }
            return -1;
        }


        #region Form Events
        private void button_Cancel_Click(object sender, EventArgs e)
        {
            // Close control, go back to Protocol Window
            RoboSep_UserConsole.getInstance().Controls.Remove(this);
            RoboSep_UserConsole.getInstance().Controls.Add(RoboSep_Protocols.getInstance());

            // LOG
            string logMSG = "Cancel button clicked";
            GUI_Controls.uiLog.LOG(this, "button_Cancel_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }
        
        private void button_SaveList_Click(object sender, EventArgs e)
        {
            // save list
            RoboSep_UserProtocolList.getInstance().saveUserList();

            // close window, go back to protocols window
            RoboSep_UserConsole.getInstance().Controls.Remove(this);
            RoboSep_UserConsole.getInstance().Controls.Add(RoboSep_Protocols.getInstance());

            // Update user list
            RoboSep_Protocols.getInstance().LoadUsers();

            // LOG
            string logMSG = "Save Protocol List";
            GUI_Controls.uiLog.LOG(this, "button_SaveList_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        private void button_Add_Click(object sender, EventArgs e)
        {
            // add items to user list
            List<RoboSep_Protocol> toBeAdded = new List<RoboSep_Protocol>();
            for (int i = 0; i < lvProtocolList.SelectedItems.Count; i++)
            {
                toBeAdded.Add(myFilterList[lvProtocolList.SelectedIndices[i]]);
            }
            RoboSep_UserProtocolList.getInstance().addProtocols(toBeAdded);
            this.ActiveControl = lvProtocolList;
            highlightItems();

            // LOG
            string logMSG = "Adding " + toBeAdded.Count.ToString() + " protocols to user profile" ;
            GUI_Controls.uiLog.LOG(this, "button_Add_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        private void button_myProfile_Click(object sender, EventArgs e)
        {
            // switch to my user protocols window
            RoboSep_UserConsole.getInstance().Controls.Remove(this);
            RoboSep_UserConsole.getInstance().Controls.Add(RoboSep_UserProtocolList.getInstance());

            string logMSG = "My Profile button clicked";
            GUI_Controls.uiLog.LOG(this, "button_myProfile_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }


        private void lvProtocolList_MouseLeave(object sender, EventArgs e)
        {
            selectedProtocols = new RoboSep_Protocol[lvProtocolList.SelectedItems.Count];
            for (int i = 0; i < selectedProtocols.Length; i++)
            {
                selectedProtocols[i] = myFilterList[lvProtocolList.SelectedIndices[i]];
            }                        
        }

        private void activateTextbox(object sender, EventArgs e)
        {
            if (RoboSep_UserConsole.KeyboardEnabled)
                createKeybaord();
        }

        private void mouseActivateTextbox(object sender, MouseEventArgs e)
        {
            if (RoboSep_UserConsole.KeyboardEnabled)
                createKeybaord();
        }

        private void createKeybaord()
        {
            // Show window overlay
            RoboSep_UserConsole.getInstance().frmOverlay.Show();
            RoboSep_UserConsole.getInstance().frmOverlay.BringToFront();

            // Create keybaord control
            GUI_Controls.Keyboard newKeyboard =
                new GUI_Controls.Keyboard(RoboSep_UserConsole.getInstance(),
                    SearchBar.textBox_search, RoboSep_UserConsole.getInstance().frmOverlay);
            newKeyboard.Show();

            // add keyboard control to user console "track form"
            RoboSep_UserConsole.getInstance().addForm(newKeyboard, newKeyboard.Offset);
            //RoboSep_UserConsole.myUserConsole.addForm(newKeyboard.overlay, newKeyboard.overlayOffset);
        }

        private void button_USBload_Click(object sender, EventArgs e)
        {
            // add protocols from USB folder
            // first check to see which drives exist (E:\ , F:\,  G:\)
            string[] drives = new string[] { "E:\\", "F:\\", "G:\\" };
            string ProtocolsPath = string.Empty;
            for (int i = 0; i < drives.Length; i++)
            {
                if (Directory.Exists( drives[i] ))
                {
                    ProtocolsPath = drives[i];
                    if (Directory.Exists(drives[i] + "Protocols"))
                    {
                        ProtocolsPath = drives[i] + "Protocols";
                        break;
                    }
                }
            }

            string destinationPath = RoboSep_UserDB.getInstance().GetProtocolsPath();

            // determine usb directory
            string[] dirs = new string[] { "E:\\", "F:\\", "G:\\" };
            string USBpath = string.Empty;
            try
            {
                for (int i = 0; i < dirs.Length; i++)
                {
                    if (Directory.Exists( dirs[i] ))
                    {
                        USBpath = dirs[i];
                        if (Directory.Exists((dirs[i] + "Protocols\\")))
                        {
                            USBpath += "Protocols\\";
                            break;
                        }
                    }
                }

                if (USBpath != string.Empty)
                {
                    // load file browser window
                    FileBrowser myFB = new FileBrowser(RoboSep_UserConsole.getInstance(), 
                        USBpath, destinationPath, new string[] { "xml" }, FileBrowser.BrowseResult.Copy);
                    int x = RoboSep_UserConsole.getInstance().Location.X + myFB.Offset.X;
                    int y = RoboSep_UserConsole.getInstance().Location.Y + myFB.Offset.Y + 20;
                    myFB.Location = new Point(x, y);
                    RoboSep_UserConsole.getInstance().frmOverlay.Show();
                    myFB.ShowDialog();
                    RoboSep_UserConsole.getInstance().frmOverlay.Hide();
                    // if protocols added, re-attain and show protocol list
                    if (myFB.DialogResult == DialogResult.OK)
                    {
                        myProtocols.Clear();
                        myProtocols = myDB.reloadProtocols();
                        updateFilteredList();
                        refreshList();
                    }

                    // LOG
                    string logMSG = "Loading from USB directory '" + USBpath + "'";
                    GUI_Controls.uiLog.LOG(this, "button_USBload_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
                }
                else
                {
                    string sMSG = GUI_Controls.fromINI.getValue("GUI", "msgUSBFail");
                    sMSG = sMSG == null ? "USB drive not detected.  Insert drive and try again" : sMSG;
                    RoboMessagePanel err = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), sMSG);
                    RoboSep_UserConsole.getInstance().frmOverlay.Show();
                    err.ShowDialog();
                    RoboSep_UserConsole.getInstance().frmOverlay.Hide();

                    // LOG
                    string LOGmsg = "Error: USB drive not detected";
                    GUI_Controls.uiLog.LOG(this, "button_USBload_Click", GUI_Controls.uiLog.LogLevel.WARNING, LOGmsg);
                }
            }
            catch
            {
                string sMSG = GUI_Controls.fromINI.getValue("GUI", "msgUSBFail");
                sMSG = sMSG == null ? "USB drive not detected.  Insert drive and try again" : sMSG;
                RoboMessagePanel err = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), sMSG);
                RoboSep_UserConsole.getInstance().frmOverlay.Show();
                err.ShowDialog();
                RoboSep_UserConsole.getInstance().frmOverlay.Hide();

                // LOG
                string logMSG = "Error: USB drive not detected";
                GUI_Controls.uiLog.LOG(this, "button_USBload_Click", GUI_Controls.uiLog.LogLevel.ERROR, logMSG);
            }
        }

    }
        #endregion
}
