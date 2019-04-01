using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using GUI_Controls;
using System.Threading;
using Tesla.OperatorConsoleControls;
using Tesla.Common.Protocol;

namespace GUI_Console
{
    public partial class RoboSep_ProtocolSelect : UserControl
    {
        private int CurrentQuadrant;
        private int QuadrantsAvailable;
        private Thread myReloadProtocolsThread;
        // protocol management
        private RoboSep_UserDB myDB;
        private List<RoboSep_Protocol> myProtocols;
        private List<RoboSep_Protocol> myFilterList;
        ISeparationProtocol[] SelectedProtocols;
        // search filters
        private string search;
        private bool H;
        private bool M;
        private bool P;
        private bool N;
        private bool WB = false;

        private const int SERVER_WAIT_TIME = 200;
        private int reloadProtocolCount = 0;
        private int reloadMaximum = 3;
        private string selectingProtocol;
        private bool preLoad = false;

        RoboMessagePanel loading;

        public RoboSep_ProtocolSelect(int QuadrantNumber, int available)
        {
            InitializeComponent();
            CurrentQuadrant = QuadrantNumber;
            QuadrantsAvailable = available;

            SuspendLayout();

            // create events for when search buttons are pressed
            this.SearchBar.button_human.Click += new System.EventHandler(this.UpdateFilters);
            this.SearchBar.button_mouse.Click += new System.EventHandler(this.UpdateFilters);
            this.SearchBar.button_positive.Click += new System.EventHandler(this.UpdateFilters);
            this.SearchBar.button_negative.Click += new System.EventHandler(this.UpdateFilters);
            this.SearchBar.textBox_search.TextChanged += new System.EventHandler(this.UpdateFilters);
            this.SearchBar.textBox_search.Enter += new System.EventHandler(this.activateTextbox);
            this.SearchBar.textBox_search.MouseClick += new System.Windows.Forms.MouseEventHandler(this.mouseActivateTextbox);

            //change toggle button graphics
            List<Image> ilist = new List<Image>();
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_SELECTPROTOCOL_ALLMY0);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_SELECTPROTOCOL_ALLMY1);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_SELECTPROTOCOL_ALLMY2);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_SELECTPROTOCOL_ALLMY3);
            button_SelectionMode.ChangeGraphics(ilist);
            button_SelectionMode.setToggle(true);
            button_SelectionMode.Check = true;
            
            // set grab user name
            label_userName.Text = RoboSep_UserConsole.strCurrentUser;
            
            // LOG
            string LOGmsg = "New Protocol Select user control generated";
            GUI_Controls.uiLog.LOG(this, "RoboSep_ProtocolSelect", GUI_Controls.uiLog.LogLevel.EVENTS, LOGmsg);
            ResumeLayout();
        }

        private void RoboSep_ProtocolSelect_Load(object sender, EventArgs e)
        {
            header_devide.BringToFront();

            // update user info....
            string username = label_userName.Text;
            Tesla.DataAccess.RoboSepUser currentUser = RoboSep_UserDB.getInstance().XML_LoadRoboSepUserObject();
            string[] usrProtocols = currentUser.ProtocolFile;
            if (username != currentUser.UserName)
            {
                if (username != "All Human" && username != "All Mouse" && username != "Whole Blood"
                    && username != "Full List" && username != "USER NAME")
                {
                    // reload user profiles from userInfo.ini
                    List<RoboSep_Protocol> usrList = RoboSep_UserDB.getInstance().loadUserProtocols(username);
                    usrProtocols = new string[usrList.Count];
                    for (int i = 0; i < 0; i++)
                        usrProtocols[i] = usrList[i].Protocol_FileName;
                }
            }
            // save over current user file
            RoboSep_UserDB.getInstance().XML_SaveUserProfile(username, usrProtocols);
            //LoadProtocolsToServer();

            // get user protocols
            SelectedProtocols = null;
            SelectedProtocols = RoboSep_UserConsole.getInstance().XML_getUserProtocols();

            foreach (ISeparationProtocol p in SelectedProtocols)
            {
                string[] splitString = p.Label.Split('.');
                listView_UserProtocols.Items.Add(splitString[0]);
                listView_UserProtocols.Items[listView_UserProtocols.Items.Count - 1].SubItems.Add(p.StringClassification);
                listView_UserProtocols.Items[listView_UserProtocols.Items.Count - 1].SubItems.Add(p.QuadrantCount.ToString());
            }
            highlightItems();

            // load protocols
            myDB = RoboSep_UserDB.getInstance();

            // get protocols
            myProtocols = myDB.getAllProtocols();
            myFilterList = new List<RoboSep_Protocol>();            

            // deal with preset protocol lists
            if (! dealWithPresetLists(label_userName.Text) )
                updateFilteredList();
        }

        /*
        public void resetForm()
        {
            // remove all filters
            SearchBar.button_human.Check = false;
            SearchBar.button_mouse.Check = false;
            SearchBar.button_negative.Check = false;
            SearchBar.button_positive.Check = false;
            WB = false;
            SearchBar.textBox_search.Text = string.Empty;
            button_SelectionMode.Enabled = true;

            // set user name to current user
            label_userName.Text = RoboSep_UserConsole.strCurrentUser;
            // deal with preset protocol lists
            dealWithPresetLists(label_userName.Text);

            // load user protocols
            ISeparationProtocol[] SelectedProtocols = RoboSep_UserConsole.getInstance().XML_getUserProtocols();
            foreach (ISeparationProtocol p in SelectedProtocols)
            {
                string[] splitString = p.Label.Split('.');
                listView_UserProtocols.Items.Add(splitString[0]);
                listView_UserProtocols.Items[listView_UserProtocols.Items.Count - 1].SubItems.Add(p.StringClassification);
                listView_UserProtocols.Items[listView_UserProtocols.Items.Count - 1].SubItems.Add(p.QuadrantCount.ToString());
            }
            highlightItems();
        }*/

        private bool dealWithPresetLists(string username)
        {
            // check if user is preset user
            switch (username)
            {
                case "All Human":
                    // set selection mode to "all protocols"
                    button_SelectionMode.Check = false;
                    // disable changing to "user protocols"
                    button_SelectionMode.Enabled = false;
                    // set filters
                    SearchBar.button_human.Check = true;
                    switchSelectionList();
                    return true;
                    break;
                case "All Mouse":
                    // set selection mode to "all protocols"
                    button_SelectionMode.Check = false;
                    // disable changing to "user protocols"
                    button_SelectionMode.Enabled = false;
                    
                    // set filters
                    SearchBar.button_mouse.Check = true;
                    // refresh list
                    switchSelectionList();
                    return true;
                    break;
                case "Whole Blood":
                    // set selection mode to "all protocols"
                    button_SelectionMode.Check = false;
                    // disable changing to "user protocols"
                    button_SelectionMode.Enabled = false;
                    // set filters
                    WB = true;
                    // refresh list
                    switchSelectionList();
                    return true;
                    break;
                case "Full List":
                    // set selection mode to "all protocols"
                    button_SelectionMode.Check = false;                    
                    // refresh list
                    switchSelectionList();
                    return true;
                    break;
            }
            return false;
        }

        public void getFilters()
        {
            SearchBar.getFilters(out search, out H, out M, out P, out N);
        }

        private void highlightItems()
        {
            Color bg_offcolour = Color.FromArgb(215, 209, 215);
            Color bg_colour = Color.White;
            Color SelectedColour = Color.LightSteelBlue;
            for (int i = 0; i < listView_UserProtocols.Items.Count; i++)
            {
                if ((i % 2) == 1)
                {
                    listView_UserProtocols.Items[i].BackColor = bg_offcolour;

                }
                else
                {
                    //listView_robostat.Items[i].BackColor = Color.FromArgb(233, 233, 233);
                }
                // check if selected protocol
                int ProtocolQuadrants = Convert.ToInt32(listView_UserProtocols.Items[i].SubItems[2].Text);
                if ( QuadrantsAvailable < ProtocolQuadrants )
                {
                    listView_UserProtocols.Items[i].BackColor = Color.LightGray;
                    listView_UserProtocols.Items[i].ForeColor = Color.White;
                }
            }
        }

        //UpdateFilters
        private void UpdateFilters(object sender, EventArgs e)
        {
            // update list
            updateFilteredList();

            // display filtered list^
            refreshList();
        }

        public void refreshList()
        {
            listView_UserProtocols.SuspendLayout();

            // check what display mode to show (user / all protocols)
            // true = user
            // false = all protocols
            if (!button_SelectionMode.Check)
            {
                // determine which protocol name to display
                getFilters();
                if (H || M)
                {
                    if (P || N)
                    {
                        // show Species & Selection filtered name
                        listView_UserProtocols.Items.Clear();
                        for (int i = 0; i < myFilterList.Count; i++)
                        {
                            listView_UserProtocols.Items.Add(myFilterList[i].SpeciesSelection_Name);
                            listView_UserProtocols.Items[i].SubItems.Add("");
                            listView_UserProtocols.Items[i].SubItems.Add(myFilterList[i].numQuadrants.ToString());
                        }
                    }
                    else
                    {
                        // show Species filtered name
                        listView_UserProtocols.Items.Clear();
                        for (int i = 0; i < myFilterList.Count; i++)
                        {
                            listView_UserProtocols.Items.Add(myFilterList[i].Species_Name);
                            listView_UserProtocols.Items[i].SubItems.Add("");
                            listView_UserProtocols.Items[i].SubItems.Add(myFilterList[i].numQuadrants.ToString());
                        }
                    }
                }
                else if (P || N)
                {
                    // show Selection filtered name
                    listView_UserProtocols.Items.Clear();
                    for (int i = 0; i < myFilterList.Count; i++)
                    {
                        listView_UserProtocols.Items.Add(myFilterList[i].Selection_Name);
                        listView_UserProtocols.Items[i].SubItems.Add("");
                        listView_UserProtocols.Items[i].SubItems.Add(myFilterList[i].numQuadrants.ToString());
                    }
                }
                else
                {
                    // show un-filtered name
                    listView_UserProtocols.Items.Clear();
                    for (int i = 0; i < myFilterList.Count; i++)
                    {
                        listView_UserProtocols.Items.Add(myFilterList[i].Protocol_Name);
                        listView_UserProtocols.Items[i].SubItems.Add("");
                        listView_UserProtocols.Items[i].SubItems.Add(myFilterList[i].numQuadrants.ToString());
                    }
                }
            }
            highlightItems();
            listView_UserProtocols.ResumeLayout(true);
            
        }

        private void updateFilteredList()
        {
            List<RoboSep_Protocol> tempList = new List<RoboSep_Protocol>();
            getFilters();
            bool[] currentFilters = new bool[] { H, M, P, N, WB };
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
            if (WB)
            {
                filterIndex.Add(4);
            }

            if (!(H || M) && !(P || N) && !WB)
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
                        if (currentFilters[filterIndex[j]] != myProtocols[i].bfilters[filterIndex[j]])
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
                    myFilterList.Add(tempList[i]);
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

        private void button_Accept_Click(object sender, EventArgs e)
        {
            // check if an item is selected in List view
            if (listView_UserProtocols.SelectedItems.Count > 0)
            {
                // check if adding from current user list or from "all protocols"
                // true = user list
                // false = all list
                if (!button_SelectionMode.Check)
                {
                    RoboSep_Protocol addition = myProtocols[findInList(listView_UserProtocols.SelectedItems[0].Text)];

                    // add protocol to user list
                    selectingProtocol = addition.Protocol_Name;                    

                    AddProtocolToUserList(addition.Protocol_FileName);
                    //Thread.Sleep(1000);
                    // update user list with server
                    // grab user list
                    // select protocol as "chosen protocol" with server
                        

                    // LOG
                    string LOGmsg = "Protocol Selected from All Protocols list";
                    GUI_Controls.uiLog.LOG(this, "button_Accept_Click", GUI_Controls.uiLog.LogLevel.EVENTS, LOGmsg);
                }
                else
                {
                    string theProtocol = listView_UserProtocols.SelectedItems[0].Text;
                    int Qcount = Convert.ToInt32(listView_UserProtocols.SelectedItems[0].SubItems[2].Text);
                    SelectProtocol( theProtocol, Qcount );

                    // LOG
                    string LOGmsg = "Protocol Selected from User list";
                    GUI_Controls.uiLog.LOG(this, "button_Accept_Click", GUI_Controls.uiLog.LogLevel.EVENTS, LOGmsg);
                }
            }
        }

        

        private void SelectProtocol(string theSelectedProtocol, int quadrants)
        {
            // Check if there are enough quadrants available to accomodate protocol
            if (quadrants <= QuadrantsAvailable && (CurrentQuadrant + quadrants - 1) < 4)
            {

                // switch to Sampling window
                RoboSep_RunSamples.getInstance().Visible = false;
                RoboSep_UserConsole.getInstance().Controls.Add(RoboSep_RunSamples.getInstance());

                // update run samples window (gets window)
                RoboSep_RunSamples.getInstance().addToRun(CurrentQuadrant, theSelectedProtocol);
                RoboSep_RunSamples.getInstance().Visible = true;
                this.SendToBack();

                RoboSep_UserConsole.getInstance().Controls.Remove(this);
                RoboSep_UserConsole.ctrlCurrentUserControl = RoboSep_RunSamples.getInstance();

                // LOG
                string LOGmsg = "Adding protocol to current run";
                GUI_Controls.uiLog.LOG(this, "SelectProtocol", GUI_Controls.uiLog.LogLevel.INFO, LOGmsg);
            }
            else
            {
                string sMSG = fromINI.getValue("GUI", "msgCarouselSpace");
                sMSG = sMSG == null ? "protocol, possibly re-organize to make space" : sMSG;
                // inform user of inability to add protocol
                RoboMessagePanel newPrompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(),
                    sMSG, "Quadrant Scheduling problem", "OK", "Cancel");
                RoboSep_UserConsole.getInstance().addForm(newPrompt, newPrompt.Offset);
                RoboSep_UserConsole.getInstance().frmOverlay.Show();
                newPrompt.ShowDialog();
                RoboSep_UserConsole.getInstance().frmOverlay.Hide();

                // LOG
                string LOGmsg = "Could not add protocol to run, insufficient space";
                GUI_Controls.uiLog.LOG(this, "SelectProtocol", GUI_Controls.uiLog.LogLevel.WARNING, LOGmsg);
            }
        }

        private void AddProtocolToUserList( string AddProtocol)
        {

            // show loading protocol message
            string msg = "Loading protocol from server...";
            loading = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), msg, true, false);
            RoboSep_UserConsole.getInstance().frmOverlay.Show();
            loading.Show();
            try
            {
                // load user from ini
                string[] sProtocols = new string[SelectedProtocols.Length + 1];
                for (int i = 0; i < SelectedProtocols.Length; i++)
                    sProtocols[i] = (SelectedProtocols[i].Label + ".xml");
                sProtocols[sProtocols.Length - 1] = AddProtocol;

                // save over user1.udb
                RoboSep_UserDB.getInstance().XML_SaveUserProfile(RoboSep_UserConsole.strCurrentUser, sProtocols);
                Thread.Sleep(50);

                // Reload protocols with SeparatorGateway using a thread
                myReloadProtocolsThread = new Thread(new ThreadStart(this.ReloadProtocolsThread));
                myReloadProtocolsThread.IsBackground = true;
                myReloadProtocolsThread.Start();

                SeparatorGateway.GetInstance().Connect(false);

                LoadProtocolTimer.Start();
            }
            catch
            {
                // LOG
                string LOGmsg = "Failed to add protocol to user list";
                GUI_Controls.uiLog.LOG(this, "AddProtocolToUserList", GUI_Controls.uiLog.LogLevel.ERROR, LOGmsg);
            }
        }

        private void LoadProtocolsToServer()
        {
        // Reload protocols with SeparatorGateway using a thread
            Thread.Sleep(500);
            myReloadProtocolsThread = new Thread(new ThreadStart(this.ReloadProtocolsThread));
            myReloadProtocolsThread.IsBackground = true;
            myReloadProtocolsThread.Start();

            SeparatorGateway.GetInstance().Connect(false);            
        }

        // function run to refresh server's active user protocol list
        private void ReloadProtocolsThread()
        {
            // save list as old robosep would
            System.Threading.Thread.Sleep(150);
            SeparatorGateway.GetInstance().ReloadProtocols();
        }

        private bool selectAddition( string protocolName )
        {
            // wait until user has been updated
            // check if user has been updated
            
            // while loop to check for update until
            // update is present
            ISeparationProtocol[]  myExtendedSelection = RoboSep_UserConsole.getInstance().XML_getUserProtocols();
            
            for (int i = 0; i < myExtendedSelection.Length; i++)
            {
                if ( myExtendedSelection[i].Label == protocolName )
                {
                    SelectProtocol(myExtendedSelection[i].Label,
                        myExtendedSelection[i].QuadrantCount);
                    return true;
                }
            }
            return false;
        }

        private int loadState = 0;
        private void LoadProtocolTimer_Tick(object sender, EventArgs e)
        {
            if (loadState == 0 && RoboSep_UserConsole.getInstance().bLoadingProtocols)
            {
                loadState++;
            }
            if (loadState == 1 && RoboSep_UserConsole.getInstance().bLoadingProtocols)
            {
                // update progress counter
            }
            else if (loadState == 1 && !RoboSep_UserConsole.getInstance().bLoadingProtocols)
            {
                // peform completion test
                // check if given list matches
                // list returned from server
                List<RoboSep_Protocol> given = RoboSep_UserDB.getInstance().loadUserProtocols(RoboSep_UserConsole.strCurrentUser);
                Thread.Sleep(1500);
                Tesla.Common.Protocol.ISeparationProtocol[] serverList = RoboSep_UserConsole.getInstance().XML_getUserProtocols();
                bool ServerUpdated = false;
                if (given.Count == serverList.Length)
                {
                    ServerUpdated = true;
                    for (int i = 0; i < serverList.Length; i++)
                    {
                        bool IsInList = false;
                        for (int j = 0; j < given.Count; j++)
                        {
                            if (given[j].Protocol_Name.ToUpper() == serverList[i].Label.ToUpper())
                            {
                                IsInList = true;
                                break;
                            }
                        }
                        if (!IsInList)
                        {
                            ServerUpdated = false;
                            break;
                        }
                    }
                }
                int ProtocolIndex;
                int attempts = 0;
                do
                {
                    ProtocolIndex = findInList(selectingProtocol);
                    attempts++;
                } while (ProtocolIndex < 0 || attempts > 1);
                RoboSep_Protocol addition = myProtocols[ProtocolIndex];

                if ( ServerUpdated || reloadProtocolCount > 1)
                {
                    // close loading dialog
                    loading.Close();
                    RoboSep_UserConsole.getInstance().frmOverlay.Hide();
                    // reset count
                    reloadProtocolCount = 0;

                    LoadProtocolTimer.Stop();
                    Thread.Sleep(100);
                    loadState = 0;

                    reloadMaximum++;

                    if (!selectAddition(addition.Protocol_Name) && reloadMaximum < 3)
                    {
                        AddProtocolToUserList(addition.Protocol_FileName);
                        return;
                    }
                    else if (reloadMaximum == 3)
                    {
                        // re-attempt to add protocol
                        AddProtocolToUserList(addition.Protocol_FileName);
                    }
                    else
                        reloadMaximum = 0;
                    LoadProtocolTimer.Stop();
                }
                else
                {
                    reloadProtocolCount++;                    
                    LoadProtocolsToServer();
                    LoadProtocolTimer.Start();
                }
            }
        }

        private int findInList(string pString)
        {
            getFilters();
            if ((H || M) && (P || N))
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

        private void button_SelectionMode_Click(object sender, EventArgs e)
        {
            switchSelectionList();
            
            // LOG
            string msg = button_SelectionMode.Check ? "User Protocols" : "All Protocols";
            string LOGmsg = "Change Protocol selection mode to" + msg;
            GUI_Controls.uiLog.LOG(this, "button_SelectionMode_Click", GUI_Controls.uiLog.LogLevel.EVENTS, LOGmsg);
        }

        private void switchSelectionList()
        {
            // pre-load user profile prior to
            // attempting to add protocol from
            // protocols list
            if (!preLoad)
            {
                // Reload protocols with SeparatorGateway using a thread
                myReloadProtocolsThread = new Thread(new ThreadStart(this.ReloadProtocolsThread));
                myReloadProtocolsThread.IsBackground = true;
                myReloadProtocolsThread.Start();
                preLoad = true;
            }

            // display user list or all list
            // true = user list
            // false = all protocols
            if (button_SelectionMode.Check)
            {
                Header2.Visible = true;
                Header3.Visible = true;
                Column1.Width = 462;
                Column2.Width = 84;
                Column3.Width = 23;
                listView_UserProtocols.SuspendLayout();
                listView_UserProtocols.Items.Clear();
                foreach (ISeparationProtocol p in SelectedProtocols)
                {
                    string[] splitString = p.Label.Split('.');
                    listView_UserProtocols.Items.Add(splitString[0]);
                    listView_UserProtocols.Items[listView_UserProtocols.Items.Count - 1].SubItems.Add(p.StringClassification);
                    listView_UserProtocols.Items[listView_UserProtocols.Items.Count - 1].SubItems.Add(p.QuadrantCount.ToString());
                }
                highlightItems();
                listView_UserProtocols.ResumeLayout(true);
            }
            else
            {
                Header2.Visible = false;
                Header3.Visible = true;
                Column1.Width = 555;
                Column2.Width = 1;
                Column3.Width = 23;
                listView_UserProtocols.SuspendLayout();
                updateFilteredList();
                refreshList();
                listView_UserProtocols.ResumeLayout(true);
            }


            // show sarch bar if showing all list
            SearchBar.Enabled = !button_SelectionMode.Check;
            SearchBar.Visible = !button_SelectionMode.Check;
        }

        private void button_Cancel1_Click(object sender, EventArgs e)
        {
            RoboSep_RunSamples SamplingWindow = RoboSep_RunSamples.getInstance();
            RoboSep_RunSamples.getInstance().Visible = true;
            SamplingWindow.Location = new Point(0, 0);
            this.Visible = false;
            RoboSep_UserConsole.getInstance().Controls.Add(SamplingWindow);
            RoboSep_UserConsole.getInstance().Controls.Remove(RoboSep_UserConsole.ctrlCurrentUserControl);
            RoboSep_UserConsole.ctrlCurrentUserControl = SamplingWindow;

            // LOG
            string LOGmsg = "Protocol Selection cancel clicked";
            GUI_Controls.uiLog.LOG(this, "button_Cancel1_Click", GUI_Controls.uiLog.LogLevel.EVENTS, LOGmsg);
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
    }
}
