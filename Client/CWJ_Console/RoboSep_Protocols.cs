using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Xml;
using System.Text;
using System.Windows.Forms;
using GUI_Controls;
using Invetech.ApplicationLog;
using System.Threading;
using Tesla.OperatorConsoleControls;

namespace GUI_Console
{
    public partial class RoboSep_Protocols : BasePannel
    {
        private static RoboSep_Protocols myProtocols;
        private  RoboSep_UserProtocolList myUserProtocolList;
        private RoboSep_ProtocolList myProtocolList;
        private string[] allUsers;
        private Thread myReloadProtocolsThread;

        // Language file
        private IniFile LanguageINI = GUI_Console.RoboSep_UserConsole.getInstance().LanguageINI;

        // Track Profile changes
        private bool LoadUserRequired = false;
        private bool isProtocolLoading = false;
        // Loading Message Box
        RoboMessagePanel5 loading;
        private int loadState = 0;
        private int protocolsToLoad;
        private int reloadProtocolCount = 0;
        private List<Image> ilist1 = new List<Image>();
        private List<Image> ilist2 = new List<Image>();        
        public  void setProtocolLoading(bool sw)
        {
            isProtocolLoading = sw; 
        }
        
        public RoboSep_Protocols()
        {
            InitializeComponent();

            // LOG
            string logMSG = "Initializing Protocols user control";
            //GUI_Controls.uiLog.LOG(this, "RoboSep_Protocols", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                

            // get user names
            allUsers = RoboSep_UserDB.getInstance().getAllUsers();
  
            // change button graphics.
            // LOAD USER BUTTON

            ilist1.Add(GUI_Controls.Properties.Resources.btnLG_STD);
            ilist1.Add(GUI_Controls.Properties.Resources.btnLG_STD);
            ilist1.Add(GUI_Controls.Properties.Resources.btnLG_STD);
            ilist1.Add(GUI_Controls.Properties.Resources.btnLG_HIGHLIGHT);
            button_LoadUser.ChangeGraphics(ilist1);
            // EDIT LIST BUTTON

            ilist2.Add(GUI_Controls.Properties.Resources.btnLG_STD);
            ilist2.Add(GUI_Controls.Properties.Resources.btnLG_STD);
            ilist2.Add(GUI_Controls.Properties.Resources.btnLG_STD);
            ilist2.Add(GUI_Controls.Properties.Resources.btnLG_CLICK);
            button_EditList.ChangeGraphics(ilist2);

            // set label and button text based on language settings
            //roboPanel2.Title = LanguageINI.GetString("lblUserList");
            lblUser.Text = LanguageINI.GetString("lblUsr");
            button_EditList.Text = LanguageINI.GetString("lblEditList");
            lblProtocolEdit.Text = LanguageINI.GetString("lblProtocolEditor");
            button_AllHuman.Text = LanguageINI.GetString("lblHuman");
            button_AllMouse.Text = LanguageINI.GetString("lblMouse");
            button_WholeBlood.Text = LanguageINI.GetString("lblWB");
            button_FullList.Text = LanguageINI.GetString("lblFull");
            button_LoadUser.Text = LanguageINI.GetString("lblLoadUser");
            
        }

        private void RoboSep_Protocols_Load(object sender, EventArgs e)
        {
            // load users to customAutoCompleteSource of textbox
            LoadUsers();

            // set disable image for Edit List
            //button_EditList.disableImage = GUI_Controls.Properties.Resources.Button_RECT_Disable;
            //button_EditList.disable(false);
            //button_EditList.Visible = true;
            //button_EditList.BringToFront();

            // load user name from main form
            //UserNameHeader.userName = RoboSep_UserConsole.strCurrentUser;
            textBox_UserName.Text = nameHeader.userName;
        }

        public string UserName
        {
            get
            { return textBox_UserName.Text; }
            set
            { textBox_UserName.Text = value; }
        }

        public static RoboSep_Protocols getInstance()
        {
            if (myProtocols == null)
            {
                myProtocols = new RoboSep_Protocols();
            }
            GC.Collect();
            myProtocols.textBox_UserName.Enabled = !RoboSep_UserConsole.bIsRunning;
            return myProtocols;
        }

        private void openProtocolList()
        {
            // create user protocol list window
            myUserProtocolList = RoboSep_UserProtocolList.getInstance();
            myUserProtocolList.strUserName = textBox_UserName.Text;

            // create all protocols list window
            myProtocolList = RoboSep_ProtocolList.getInstance();
            myProtocolList.strUserName = textBox_UserName.Text;
            
            // show protocol list
            myProtocolList.Visible = false;

            // reset protocol list form and user protocol list
       //     myProtocolList.resetForm();

            // show protocol list control
            RoboSep_UserConsole.getInstance().Controls.Add(myProtocolList);
            RoboSep_UserConsole.getInstance().Controls.Remove(this);
            myProtocolList.Visible = true;
        }

        public bool LoadRequired
        {
            get
            {
                return LoadUserRequired;
            }
            set
            {
                List<Image> Ilist = new List<Image>();
                if (value)
                {
                    Ilist.Add(GUI_Controls.Properties.Resources.btnLG_HIGHLIGHT);
                    Ilist.Add(GUI_Controls.Properties.Resources.btnLG_HIGHLIGHT);
                    Ilist.Add(GUI_Controls.Properties.Resources.btnLG_HIGHLIGHT);
                    Ilist.Add(GUI_Controls.Properties.Resources.btnLG_HIGHLIGHT_CLICK);
                    button_LoadUser.ChangeGraphics(Ilist);
                }
                else
                {
                    Ilist.Add(GUI_Controls.Properties.Resources.btnLG_STD);
                    Ilist.Add(GUI_Controls.Properties.Resources.btnLG_STD);
                    Ilist.Add(GUI_Controls.Properties.Resources.btnLG_STD);
                    Ilist.Add(GUI_Controls.Properties.Resources.btnLG_CLICK);
                    button_LoadUser.ChangeGraphics(Ilist);
                }
                LoadUserRequired = value;
                int i;
                for (i = 0; i < Ilist.Count; i++)
                    Ilist[i].Dispose();
            }
        }

        public RoboSep_UserProtocolList getUserList()
        {
            return myUserProtocolList;
        }

        public RoboSep_ProtocolList getPList()
        {
            return myProtocolList;
        }

        private void dropDownMenu_Click(object sender, EventArgs e)
        {
            if (!RoboSep_UserConsole.bIsRunning)
            {
                // display drop down menu
                string userString;
                createDropDown(out userString);
                textBox_UserName.Text = userString;
            }
            else
            {
                Prompt_RunInProgress();
            }
        }

        public bool createDropDown(out string saveString)
        {
            // LOG
            string logMSG = "Selecting User from Dropdown";
            //GUI_Controls.uiLog.LOG(this, "dropDownMenu_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                
            // Create list of names, add them to an auto complete source
            List<string> UserNames = new List<string>();
            for (int i = 0; i < textBox_UserName.AutoCompleteCustomSource.Count; i++)
            {
                UserNames.Add(textBox_UserName.AutoCompleteCustomSource[i]);
            }

            // Show Overlay window
            RoboSep_UserConsole.showOverlay();
            RoboSep_UserConsole.getInstance().frmOverlay.BringToFront();

            // create new user select control form
            IniFile LanguageINI = GUI_Console.RoboSep_UserConsole.getInstance().LanguageINI;
            string windowTitle = LanguageINI.GetString("lblSelectUser");
            Form_UserSelect UserSelectMenu = new Form_UserSelect(UserNames, windowTitle);
            UserSelectMenu.ShowDialog();
            // check to see if user name was selected
            string temp = UserSelectMenu.User;
            saveString = "????";
            if (temp != null && temp != string.Empty)
                saveString = temp;
            
            
            bool userSelected = UserSelectMenu.DialogResult == DialogResult.OK;
            
            UserSelectMenu.Dispose();
            
            return userSelected;
        }

        #region Load Current User to Server

        public void LoadUserToServer(string ActiveUser)
        {
            // LOG
            string logMSG = "Loading user profile";

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                
            // load user from ini
            List<RoboSep_Protocol> tempList = RoboSep_UserDB.getInstance().loadUserProtocols(ActiveUser);
            if (tempList == null)
            {
                // prompt user that no protocols to load
                string sTitle = LanguageINI.GetString("headerNoProtocols");
                string sMsg = LanguageINI.GetString("msgNoProtocols");
                string sButton = LanguageINI.GetString("Ok");
                RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING, sMsg, sTitle, sButton);
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                prompt.Dispose();                
                RoboSep_UserConsole.hideOverlay();
                return;
            }

            StartLoadingProtocolsToServer(ActiveUser, tempList);
        }

        public void StartLoadingProtocolsToServer(string ActiveUser, List<RoboSep_Protocol> tempList)
        {
            if (tempList == null)
            {
                return;
            }

            // LOG
            string logMSG = "Start thread for loading profile";

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                
            try
            {
                RoboSep_UserConsole.strCurrentUser = ActiveUser;// textBox_UserName.Text;
                UserNameHeader.Text = ActiveUser;

                string[] sProtocols = new string[tempList.Count];
                for (int i = 0; i < tempList.Count; i++)
                {
                    sProtocols[i] = tempList[i].Protocol_FileName;
                }

                // save over user1.udb
                RoboSep_UserDB.getInstance().XML_SaveUserProfile(ActiveUser, sProtocols);
                protocolsToLoad = sProtocols.Length;
 
                // Reload protocols with SeparatorGateway using a thread
                myReloadProtocolsThread = new Thread(new ThreadStart(this.ReloadProtocolsThread));
                myReloadProtocolsThread.IsBackground = true;
                myReloadProtocolsThread.Start();

                // set sep-gateway to updating Separator Protocols
                // so that we can watch for when it is updated
                // (in timer_tick)
                SeparatorGateway.GetInstance().separatorUpdating = true;

                string sMSG = LanguageINI.GetString("msgLoadingProtocols");
                string sTitle = LanguageINI.GetString("headerLoadingUserProtocols");
                loading = new RoboMessagePanel5(RoboSep_UserConsole.getInstance(), sTitle, sMSG, GUI_Controls.GifAnimationMode.eUploadingMultipleFiles);
                RoboSep_UserConsole.showOverlay();
                loading.Show();
                //Thread.Sleep(SERVER_WAIT_TIME);
                // add loop to that polls loading status
                LoadUserTimer.Start();
               
            }
            catch (Exception ex)
            {
                // LOG
                logMSG = "Failed to save user to server";

                LogFile.AddMessage(System.Diagnostics.TraceLevel.Error, logMSG);
            }
        }

        private void LoadUserTimer_Tick(object sender, EventArgs e)
        {
             if (loadState == 0 && RoboSep_UserConsole.getInstance().bLoadingProtocols)
            {
                loadState++;
            }
            else if (loadState == 1 && RoboSep_UserConsole.getInstance().bLoadingProtocols)
            {
                int Loaded = RoboSep_UserConsole.getInstance().ProtocolsLoaded;
                
                // calculate percent
                double completion = (double)Loaded * 100.00 / (double)protocolsToLoad;
                loading.setProgress((int)completion);
            }
            else //if (loadState == 1 && !RoboSep_UserConsole.getInstance().bLoadingProtocols)
            {
                // check if server list is updated
                // server delegate function will update
                // protocolUpdated value when completed
                if (!SeparatorGateway.GetInstance().protocolUpdated)
                {
                    Thread.Sleep(100);
                }
                // peform completion test
                // check if given list matches
                // list returned from server
                else
                {
                    if (isProtocolLoading)
                        return;

                    SeparatorGateway.GetInstance().protocolUpdated = false;
                    bool ServerUpdated = true;

                    LoadUserTimer.Stop();

                    if (ServerUpdated || reloadProtocolCount > 2)
                    {
                        loading.setProgress(90);
                        reloadProtocolCount = 0;
                        //RoboSep_UserConsole.getInstance().XML_getUserProtocols();
                        loadState = 0;

                        // close dialog window
                        loading.Close();
                        RoboSep_UserConsole.hideOverlay();

                        if (RoboSep_UserConsole.ctrlCurrentUserControl == this)
                        {
                            // run base home button click
                            // base.btn_home_Click(sender, e);
                            return;
                        }
                        else
                        {
                            // loading user from protocol selection window
                            // check if current user control in UserConsole form
                            // is protocol select control.. if so re-load page
                            if (RoboSep_UserConsole.ctrlCurrentUserControl.GetType() == typeof(RoboSep_ProtocolSelect))
                            {
                                RoboSep_UserConsole UC = RoboSep_UserConsole.getInstance();
                                UC.Controls.Remove(RoboSep_UserConsole.ctrlCurrentUserControl);
                                RoboSep_ProtocolSelect PS = new RoboSep_ProtocolSelect(0, 4, null);
                                UC.Controls.Add(PS);
                                RoboSep_UserConsole.ctrlCurrentUserControl = PS;
                            }
                            return;
                        }
                    }
                    else
                    {
                        reloadProtocolCount++;
                        LoadUserToServer(textBox_UserName.Text);
                    }
                }
            }
        }

        // function run to refresh server's active user protocol list
        private void ReloadProtocolsThread()
        {
            // save list as old robosep would
            SeparatorGateway.GetInstance().ReloadProtocols();
            SeparatorGateway.GetInstance().Connect(false);
        }

        #endregion

        private void button_AllHuman_Click(object sender, EventArgs e)
        {
            if (!RoboSep_UserConsole.bIsRunning)
            {
                textBox_UserName.Text = "All Human";
                RoboSep_UserConsole.strCurrentUser = textBox_UserName.Text;

                // LOG
                string LOGmsg = "All Human selected";
                //GUI_Controls.uiLog.LOG(this, "button_AllHuman_Click", GUI_Controls.uiLog.LogLevel.EVENTS, LOGmsg);

                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);                
            }
            else
            {
                Prompt_RunInProgress();
            }
        }

        private void button_AllMouse_Click(object sender, EventArgs e)
        {
            if (!RoboSep_UserConsole.bIsRunning)
            {
                textBox_UserName.Text = "All Mouse";
                RoboSep_UserConsole.strCurrentUser = textBox_UserName.Text;

                // LOG
                string LOGmsg = "All Mouse selected";
                //GUI_Controls.uiLog.LOG(this, "button_AllMouse_Click", GUI_Controls.uiLog.LogLevel.EVENTS, LOGmsg);
                
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);                
            }
            else
            {
                Prompt_RunInProgress();
            }
        }

        private void button_WholeBlood_Click(object sender, EventArgs e)
        {
            if (!RoboSep_UserConsole.bIsRunning)
            {
                textBox_UserName.Text = "Whole Blood";
                RoboSep_UserConsole.strCurrentUser = textBox_UserName.Text;

                // LOG
                string LOGmsg = "Whole Blood";
                //GUI_Controls.uiLog.LOG(this, "button_WholeBlood_Click", GUI_Controls.uiLog.LogLevel.EVENTS, LOGmsg);

                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);                
            }
            else
            {
                Prompt_RunInProgress();
            }
        }

        private void button_FullList_Click(object sender, EventArgs e)
        {
            if (!RoboSep_UserConsole.bIsRunning)
            {
                textBox_UserName.Text = "Full List";
                RoboSep_UserConsole.strCurrentUser = textBox_UserName.Text;

                // LOG
                string LOGmsg = "Full List selected";
                //GUI_Controls.uiLog.LOG(this, "button_FullList_Click", GUI_Controls.uiLog.LogLevel.EVENTS, LOGmsg);
                
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);                
            }
            else
            {
                Prompt_RunInProgress();
            }
        }

        
        private void textBox_UserName_TextChanged(object sender, EventArgs e)
        {
            if (RoboSep_UserConsole.ctrlCurrentUserControl == this)
                LoadRequired = true;

            // check if there are protocols selected
            string txtBox = textBox_UserName.Text.ToUpper();
            if (txtBox == "ALL HUMAN" || txtBox == "ALL MOUSE" || txtBox == "WHOLE BLOOD" 
                || txtBox == "FULL LIST" || txtBox == "USER NAME")
            {
                //button_EditList.disable(true);
            }
            else if (!button_EditList.Enabled)
            {
                //button_EditList.disable(false);
            }

            UpdateUserImage(textBox_UserName.Text, false);

        }

        private void textBox_UserName_Click(object sender, EventArgs e)
        {
            if (!RoboSep_UserConsole.bIsRunning)
            {
                if (RoboSep_UserConsole.KeyboardEnabled)
                {
                    generateKeyboard();
                    // LOG
                    string LOGmsg = "Opening Touch Keyboard";
                    //GUI_Controls.uiLog.LOG(this, "textBox_UserName_MouseClick", GUI_Controls.uiLog.LogLevel.EVENTS, LOGmsg);
                    //  (LOGmsg);
                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);                
                }
            }
            else
            {
                Prompt_RunInProgress();
                this.ActiveControl = this;
            }
        }

        private void generateKeyboard()
        {
            // Show form overlay
            //RoboSep_UserConsole.showOverlay();
            //RoboSep_UserConsole.getInstance().frmOverlay.BringToFront();

            // Create new Keyboard control form
            GUI_Controls.Keyboard newKeyboard =
                GUI_Controls.Keyboard.getInstance(RoboSep_UserConsole.getInstance(),
                    textBox_UserName, textBox_UserName.AutoCompleteCustomSource, RoboSep_UserConsole.getInstance().frmOverlay, false);
            newKeyboard.ShowDialog();
            newKeyboard.Dispose();
            RoboSep_UserConsole.hideOverlay();
        }

        private void button_EditList_Click(object sender, EventArgs e)
        {
            if (!RoboSep_UserConsole.bIsRunning)
            {
                // check if protocols loaded for run will be lost if list is re-loaded
                // caused by adding from the "All list" section of the selct protocol window
                if (RoboSep_RunSamples.getInstance().iSelectedProtocols.Length > 0)
                {
                    List<RoboSep_Protocol> usrLst = RoboSep_UserDB.getInstance().loadUserProtocols(textBox_UserName.Text);
                    QuadrantInfo[] RunInfo = RoboSep_RunSamples.getInstance().RunInfo;

                    List<int> removeQuadrants = new List<int>();

                    for (int Quadrant = 0; Quadrant < 4; Quadrant++)
                    {
                        if (RunInfo[Quadrant].bQuadrantInUse)
                        {
                            bool ProtocolInList = false;
                            for (int i = 0; i < usrLst.Count; i++)
                            {
                                if (RunInfo[Quadrant].QuadrantLabel == usrLst[i].Protocol_Name)
                                {
                                    ProtocolInList = true;
                                    break;
                                }
                            }
                            if (!ProtocolInList)
                            {
                                for (int i = 0; i < RunInfo[Quadrant].QuadrantsRequired; i++)
                                    removeQuadrants.Add(Quadrant + i);
                            }
                        }
                    }

                    // check if any protocols have been selected
                    // for the current run and will be lost
                    // when reloading profile
                    if (removeQuadrants.Count > 0)
                    {
                        string sMSG = LanguageINI.GetString("msgRefreshList") + "\r\n\r\n" + LanguageINI.GetString("msgRefreshList2");
                        RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING, 
                            sMSG, LanguageINI.GetString("Warning"), LanguageINI.GetString("Yes"), LanguageINI.GetString("Cancel"));
                        RoboSep_UserConsole.showOverlay();
                        prompt.ShowDialog();

                        // wait for response

                        if (prompt.DialogResult == DialogResult.OK)
                        {
                            for (int i = 0; i < removeQuadrants.Count; i++)
                            {
                                RoboSep_RunSamples.getInstance().CancelQuadrant(removeQuadrants[i]);
                            }
                        }
                        
                        prompt.Dispose();                
                    }
                    else
                    {
                        return;
                    }
                }


                LoadRequired = true;

                //RoboSep_UserConsole.strCurrentUser = textBox_UserName.Text;
                openProtocolList();

                // LOG
                string LOGmsg = "Edit list button clicked";
                //GUI_Controls.uiLog.LOG(this, "button_EditList_Click", GUI_Controls.uiLog.LogLevel.EVENTS, LOGmsg);
                //  (LOGmsg);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);                
            }
            else
            {
                Prompt_RunInProgress();
            }
        }

        public void LoadUsers()
        {
            // update users list
            // load auto complete source
            textBox_UserName.AutoCompleteCustomSource.Clear();
            string[] UserSource = RoboSep_UserDB.getInstance().getAllUsers();
            if (UserSource != null)
            {
                for (int i = 0; i < UserSource.Length; i++)
                {
                    if (UserSource[i] != "All Human" && UserSource[i] != "All Mouse" &&
                        UserSource[i] != "Whole Blood" && UserSource[i] != "Full List")
                    {
                        textBox_UserName.AutoCompleteCustomSource.Add(UserSource[i]);
                    }
                }
            }
            
            // LOG
            string LOGmsg = "Reloading Auto Complete for User Textbox";
            //GUI_Controls.uiLog.LOG(this, "LoadUsers", GUI_Controls.uiLog.LogLevel.INFO, LOGmsg);
            //  (LOGmsg);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);                
        }

        private void textBox_UserName_Enter(object sender, EventArgs e)
        {
            // check if system is currently running protocols.
            if (!RoboSep_UserConsole.bIsRunning)
            {
                // check to see if there are protocols selected
                checkForSelectedProtocols();
            }
            else
            {
                Prompt_RunInProgress();
                this.ActiveControl = this;
            }
        }

        private bool checkForSelectedProtocols( )
        {
            // check if Protocols have been selected on Run Samples page
            // can not load protocols from multiple users
            if (RoboSep_RunSamples.getInstance().iSelectedProtocols.Length > 0)
            {
                // set up message prompt
                string sMSG = LanguageINI.GetString("msgChangeUser");

                RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING, sMSG, 
                    LanguageINI.GetString("headerChangeUsr"), LanguageINI.GetString("Ok"), LanguageINI.GetString("Cancel"));
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                RoboSep_UserConsole.hideOverlay();

                if (prompt.DialogResult == DialogResult.OK)
                {
                    int[] CurrentlySelectedProtocols = RoboSep_RunSamples.getInstance().iSelectedProtocols;
                    // remove protocols from sample list
                    for (int i = 0; i < CurrentlySelectedProtocols.Length; i++)
                    {
                        int Q = CurrentlySelectedProtocols[i];
                        RoboSep_RunSamples.getInstance().CancelQuadrant(Q);

                        // LOG
                        string LOGmsg = "Changing User and removing all selected protocols from current run";
                        //GUI_Controls.uiLog.LOG(this, "textBox_UserName_Enter", GUI_Controls.uiLog.LogLevel.EVENTS, LOGmsg);
                        //  (LOGmsg);
                        LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);                
                    }
                    prompt.Dispose();                                
                    return false;
                }
                prompt.Dispose();                                
                return true;
            }
            else
            {
                return false;
            }
        }

        /*
        protected override void btn_home_Click(object sender, EventArgs e)
        {
            if (LoadUserRequired && !RoboSep_UserConsole.bIsRunning)
            {
                // changing profile to a pre-defined protocol list will not
                // require loading protocols to server
                if (textBox_UserName.Text != "All Human" && textBox_UserName.Text != "All Mouse" && textBox_UserName.Text != "Whole Blood"
                    && textBox_UserName.Text != "Full List" && textBox_UserName.Text != "USER NAME")
                {
                    LoadUserRequired = false;
                    LoadUserToServer(textBox_UserName.Text);
                }
                elseYou go
                {
                    //
                    //
                    // Set up for No- All Protocols
                    // !! REMOVE ALL PROTOCOLS LIST 
                    //
#if true            // get list for preset
                    string[] PresetList = RoboSep_ProtocolList.getInstance().LoadPresetList(this.textBox_UserName.Text);
                    protocolsToLoad = PresetList.Length;

                    // save list to User1.udb
                    RoboSep_UserDB.getInstance().XML_SaveUserProfile(this.textBox_UserName.Text, PresetList);

                    // load user1.udb to server.
                    myReloadProtocolsThread = new Thread(new ThreadStart(this.ReloadProtocolsThread));
                    myReloadProtocolsThread.IsBackground = true;
                    myReloadProtocolsThread.Start();

                    // set sep-gateway to updating Separator Protocols
                    // so that we can watch for when it is updated
                    // (in timer_tick)
                    SeparatorGateway.GetInstance().separatorUpdating = true;

                    //Thread.Sleep(SERVER_WAIT_TIME);
                    // add loop to that polls loading status
                    LoadUserTimer.Start();
#else
                    base.btn_home_Click(sender, e);
#endif
                }
            }
            else
            {
                base.btn_home_Click(sender, e);
            }
            
        }
        */

        private void Prompt_RunInProgress()
        {
            string sMSG = LanguageINI.GetString("msgIsRunning");
            RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING, sMSG,
                LanguageINI.GetString("headerIsRunning"), LanguageINI.GetString("Ok"));
            RoboSep_UserConsole.showOverlay();
            prompt.ShowDialog();
            prompt.Dispose();                
            RoboSep_UserConsole.hideOverlay();
        }

        private void Button_ProtocolEdit_Click(object sender, EventArgs e)
        {
            // LAUNCH PROTOCOL EDITOR
            // see if Service target directory exist
            string systemPath = RoboSep_UserDB.getInstance().sysPath;
            string[] directories = systemPath.Split('\\');

            string EditorPath = string.Empty;
            for (int i = 0; i < (directories.Length-2); i++)
                EditorPath += directories[i] + "\\";
            EditorPath += "RoboSepEditor\\bin\\";

            // check if service directory exists
            if (!System.IO.Directory.Exists(EditorPath))
                System.IO.Directory.CreateDirectory(EditorPath);

            // if Service.exe exists, run service.exe
            if (System.IO.File.Exists(EditorPath + "ProtocolEditor.exe"))
            {
                // run Service
                System.Diagnostics.ProcessStartInfo EditorStartInfo = new System.Diagnostics.ProcessStartInfo();
                EditorStartInfo.WorkingDirectory = EditorPath;
                EditorStartInfo.FileName = "ProtocolEditor.exe";
                EditorStartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;

                System.Diagnostics.Process ProtocolEditorProgram = new System.Diagnostics.Process();
                ProtocolEditorProgram.StartInfo = EditorStartInfo;
                ProtocolEditorProgram.Start();
            }
        }

        private void button_LoadUser_Click(object sender, EventArgs e)
        {
            LoadRequired = false;
            if (!RoboSep_UserConsole.bIsRunning)
            {
                if (!checkForSelectedProtocols())
                    LoadUserToServer(textBox_UserName.Text);
            }
            else
            {
                Prompt_RunInProgress();
            }
        }

        private void button_AddUser_Click(object sender, EventArgs e)
        {
            // get all users to see if new user name has been entered
            string[] UserNames = RoboSep_UserDB.getInstance().getAllUsers();
            // Create keyboard to allow user to enter user name
            generateKeyboard();
            // check if name entered is new user
            bool isNewUser = true;
            for (int userNum = 0; userNum < UserNames.Length; userNum++)
            {
                if (textBox_UserName.Text == UserNames[userNum])
                    isNewUser = false;
            }
            if (isNewUser)
                button_EditList_Click(sender, e);
        }

        private void button_RemoveUser_Click(object sender, EventArgs e)
        {
            string newUser = textBox_UserName.Text;
            bool removeUser = createDropDown(out newUser);
            textBox_UserName.Text = newUser;

            if (removeUser)
            {
                UserDetails.removeSection("USER", textBox_UserName.Text);
                this.ActiveControl = dropDownMenu;
                textBox_UserName.Text = string.Empty;
                LoadUsers();
            }
        }

        private void horizontalTabs1_Tab1_Click(object sender, EventArgs e)
        {
            // do nothing.  just used as a header
        }

        private void IconChange_Click(object sender, EventArgs e)
        {
            UpdateUserImage(textBox_UserName.Text, true);
        }

        public void UpdateUserImage(string userName, bool ChangeIcon)
        {
            IniFile UserINI = new IniFile(IniFile.iniFile.User);
            List<string> users = UserINI.IniGetCategories();
            bool userExists = false;
            // first check if profile already exists
            // profile must exist to change graphic
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i] == userName)
                {
                    userExists = true;
                    break;
                }
            }

            if (userExists)
            {
                // only do this part if on the protocols page
                if (ChangeIcon)
                {
                    Form_IconSelect myIconSelect = new Form_IconSelect(userName);
                    RoboSep_UserConsole.showOverlay();
                    myIconSelect.ShowDialog();
                    myIconSelect.Dispose();
                    RoboSep_UserConsole.hideOverlay();
                }

                string ImgPath = UserINI.GetString(userName + "Image");
                if (ImgPath.StartsWith("Default"))
                {
                    switch (ImgPath)
                    {
                        default:
                            UserIcon.Image = Properties.Resources.DefaultUser_GREY;
                            break;
                        case "Default1":
                            UserIcon.Image = Properties.Resources.DefaultUser_GREY;
                            break;
                        case "Default2":
                            UserIcon.Image = Properties.Resources.DefaultUser_PURPLE;
                            break;
                        case "Default3":
                            UserIcon.Image = Properties.Resources.DefaultUser_BLUE;
                            break;
                        case "Default4":
                            UserIcon.Image = Properties.Resources.DefaultUser_GREEN;
                            break;
                    }
                }
                else
                {
                    UserIcon.Image = Image.FromFile(ImgPath);
                }
            }
        }

    }
}
