using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Xml;
using System.Text;
using System.Windows.Forms;
using GUI_Controls;

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
        private string initialUser;

        // Track Profile changes
        private bool ProfileChanged = false;

        // Loading Message Box
        RoboMessagePanel loading;
        private int loadState = 0;
        private int protocolsToLoad;
        private int reloadProtocolCount = 0;

        static int SERVER_WAIT_TIME = 1000;

        public RoboSep_Protocols()
        {
            InitializeComponent();

            // LOG
            string logMSG = "Initializing Protocols user control";
            GUI_Controls.uiLog.LOG(this, "RoboSep_Protocols", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

            // get user names
            allUsers = RoboSep_UserDB.getInstance().getAllUsers();
            initialUser = "";

            // change button graphics.
            List<Image> ilist = new List<Image>();
            ilist.Add(GUI_Controls.Properties.Resources.Button_PROTOCOLEDITOR0);
            ilist.Add(GUI_Controls.Properties.Resources.Button_PROTOCOLEDITOR1);
            ilist.Add(GUI_Controls.Properties.Resources.Button_PROTOCOLEDITOR2);
            ilist.Add(GUI_Controls.Properties.Resources.Button_PROTOCOLEDITOR3);
            Button_ProtocolEdit.ChangeGraphics(ilist);
        }

        private void RoboSep_Protocols_Load(object sender, EventArgs e)
        {
            // load user name from main form
            textBox_UserName.Text = RoboSep_UserConsole.strCurrentUser;
            
            // load users to customAutoCompleteSource of textbox
            LoadUsers();

            // set disable image for Edit List
            button_EditList.disableImage = GUI_Controls.Properties.Resources.Button_RECT_Disable;
        }

        public static RoboSep_Protocols getInstance()
        {
            if (myProtocols == null)
            {
                myProtocols = new RoboSep_Protocols();
            }
            return myProtocols;
        }

        private void openProtocolList()
        {
            // create user protocol list window
            myUserProtocolList = RoboSep_UserProtocolList.getInstance();

            // create all protocols list window
            myProtocolList = RoboSep_ProtocolList.getInstance();
            
            // show protocol list
            myProtocolList.Visible = false;

            // reset protocol list form and user protocol list
            myProtocolList.resetForm();

            // show protocol list control
            RoboSep_UserConsole.getInstance().Controls.Add(myProtocolList);
            RoboSep_UserConsole.getInstance().Controls.Remove(this);
            myProtocolList.Visible = true;
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
            // LOG
            string logMSG = "Selecting User from Dropdown";
            GUI_Controls.uiLog.LOG(this, "dropDownMenu_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

            // Create list of names, add them to an auto complete source
            List<string> UserNames = new List<string>();
            for (int i = 0; i < textBox_UserName.AutoCompleteCustomSource.Count; i++)
            {
                UserNames.Add(textBox_UserName.AutoCompleteCustomSource[i]);
            }
            
            // Show Overlay window
            RoboSep_UserConsole.getInstance().frmOverlay.Show();
            RoboSep_UserConsole.getInstance().frmOverlay.BringToFront();

            // create new user select control form
            Form_UserSelect UserSelectMenu = new Form_UserSelect(textBox_UserName, UserNames);
            UserSelectMenu.Show();
            UserSelectMenu.BringToFront();
        }

        #region Load Current User to Server
        private void LoadUserToServer()
        {
            // LOG
            string logMSG = "Saving user profile";
            GUI_Controls.uiLog.LOG(this, "LoadUserToServer", GUI_Controls.uiLog.LogLevel.INFO, logMSG);

            try
                {
                // load user from ini
                List<RoboSep_Protocol> tempList =
                    RoboSep_UserDB.getInstance().loadUserProtocols(textBox_UserName.Text);
                //for (int j = 0; j <2; j++)
                //{
                    if (tempList != null)
                    {
                        string[] sProtocols = new string[tempList.Count];
                        for (int i = 0; i < tempList.Count; i++)
                        { 
                            sProtocols[i] = tempList[i].Protocol_FileName; 
                        }

                        // save over user1.udb
                        RoboSep_UserDB.getInstance().XML_SaveUserProfile(textBox_UserName.Text, sProtocols);
                        protocolsToLoad = sProtocols.Length;
                    }


                    // Reload protocols with SeparatorGateway using a thread
                    myReloadProtocolsThread = new Thread(new ThreadStart(this.ReloadProtocolsThread));
                    myReloadProtocolsThread.IsBackground = true;
                    myReloadProtocolsThread.Start();

                    SeparatorGateway.GetInstance().Connect(false);

                    //Thread.Sleep(SERVER_WAIT_TIME);
                    // add loop to that polls loading status
                    LoadUserTimer.Start();  

                    //RoboSep_UserConsole.getInstance().XML_getUserProtocols();
                //}
            }
            catch
            {
                // LOG
                string LOGmsg = "Failed to save user to server";
                GUI_Controls.uiLog.LOG(this, "LoadUserToServer", GUI_Controls.uiLog.LogLevel.WARNING, LOGmsg);
            }
        }

        
        private void LoadUserTimer_Tick(object sender, EventArgs e)
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
                LoadUserTimer.Stop();

                // peform completion test
                // check if given list matches
                // list returned from server
                List<RoboSep_Protocol> given = RoboSep_UserDB.getInstance().loadUserProtocols(RoboSep_UserConsole.strCurrentUser);
                Thread.Sleep(100);
                Tesla.Common.Protocol.ISeparationProtocol[] serverList = RoboSep_UserConsole.getInstance().XML_getUserProtocols();
                bool ServerUpdated = false;
                if (given.Count == serverList.Length)
                {
                    ServerUpdated = true;
                    for (int i=0; i< serverList.Length; i++)
                    {
                        bool IsInList = false;
                        for (int j=0; j< given.Count; j++)
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
                if (ServerUpdated || reloadProtocolCount > 3)
                {
                    loading.setProgress(90);
                    reloadProtocolCount = 0;
                    RoboSep_UserConsole.getInstance().XML_getUserProtocols();
                    loadState = 0;

                    loading.Close();
                    RoboSep_UserConsole.getInstance().frmOverlay.Hide();

                    // run base home button click
                    base.btn_home_Click(sender, e);
                }
                else
                {
                    reloadProtocolCount++;
                    LoadUserToServer();
                }
            }
        }

        // function run to refresh server's active user protocol list
        private void ReloadProtocolsThread()
        {
            // save list as old robosep would
            System.Threading.Thread.Sleep(50);
            SeparatorGateway.GetInstance().ReloadProtocols();
                       
        }

        #endregion

        private void button_AllHuman_Click(object sender, EventArgs e)
        {
            textBox_UserName.Text = "All Human";
            RoboSep_UserConsole.strCurrentUser = textBox_UserName.Text;

            // LOG
            string LOGmsg = "All Human selected";
            GUI_Controls.uiLog.LOG(this, "button_AllHuman_Click", GUI_Controls.uiLog.LogLevel.EVENTS, LOGmsg);
        }

        private void button_AllMouse_Click(object sender, EventArgs e)
        {
            textBox_UserName.Text = "All Mouse";
            RoboSep_UserConsole.strCurrentUser = textBox_UserName.Text;

            // LOG
            string LOGmsg = "All Mouse selected";
            GUI_Controls.uiLog.LOG(this, "button_AllMouse_Click", GUI_Controls.uiLog.LogLevel.EVENTS, LOGmsg);
        }

        private void button_WholeBlood_Click(object sender, EventArgs e)
        {
            textBox_UserName.Text = "Whole Blood";
            RoboSep_UserConsole.strCurrentUser = textBox_UserName.Text;

            // LOG
            string LOGmsg = "Whole Blood";
            GUI_Controls.uiLog.LOG(this, "button_WholeBlood_Click", GUI_Controls.uiLog.LogLevel.EVENTS, LOGmsg);
        }

        private void button_FullList_Click(object sender, EventArgs e)
        {
            textBox_UserName.Text = "Full List";
            RoboSep_UserConsole.strCurrentUser = textBox_UserName.Text;

            // LOG
            string LOGmsg = "Full List selected";
            GUI_Controls.uiLog.LOG(this, "button_FullList_Click", GUI_Controls.uiLog.LogLevel.EVENTS, LOGmsg);
        }

        
        private void textBox_UserName_TextChanged(object sender, EventArgs e)
        {
            ProfileChanged = true;

            // check if there are protocols selected
            string txtBox = textBox_UserName.Text.ToUpper();
            if (txtBox == "ALL HUMAN" || txtBox == "ALL MOUSE" || txtBox == "WHOLE BLOOD" 
                || txtBox == "FULL LIST" || txtBox == "USER NAME")
            {
                button_EditList.disable(true);
            }
            else if (!button_EditList.Enabled)
            {
                button_EditList.disable(false);
            }

            if (RoboSep_UserConsole.ctrlCurrentUserControl == this)
            {
                if (checkForSelectedProtocols())
                {
                    textBox_UserName.Text = RoboSep_UserConsole.strCurrentUser;
                }
            }
        }

        private void textBox_UserName_MouseClick(object sender, MouseEventArgs e)
        {
            if (RoboSep_UserConsole.KeyboardEnabled)
            {
                generateKeyboard();
                // LOG
                string LOGmsg = "Opening Touch Keyboard";
                GUI_Controls.uiLog.LOG(this, "textBox_UserName_MouseClick", GUI_Controls.uiLog.LogLevel.EVENTS, LOGmsg);
            }
        }

        private void generateKeyboard()
        {
            // Show form overlay
            RoboSep_UserConsole.getInstance().frmOverlay.Show();
            RoboSep_UserConsole.getInstance().frmOverlay.BringToFront();


            // Create new Keyboard control form
            GUI_Controls.Keyboard newKeyboard =
                new GUI_Controls.Keyboard(RoboSep_UserConsole.getInstance(),
                    textBox_UserName, textBox_UserName.AutoCompleteCustomSource, RoboSep_UserConsole.getInstance().frmOverlay,false);
            newKeyboard.Show();
            newKeyboard.BringToFront();

            // add keyboard form to user_console "track form"
            RoboSep_UserConsole.getInstance().addForm(newKeyboard, newKeyboard.Offset);
        }

        private void button_EditList_Click(object sender, EventArgs e)
        {
            ProfileChanged = true;

            RoboSep_UserConsole.strCurrentUser = textBox_UserName.Text;
            openProtocolList();

            // LOG
            string LOGmsg = "Edit list button clicked";
            GUI_Controls.uiLog.LOG(this, "button_EditList_Click", GUI_Controls.uiLog.LogLevel.EVENTS, LOGmsg);
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
            GUI_Controls.uiLog.LOG(this, "LoadUsers", GUI_Controls.uiLog.LogLevel.INFO, LOGmsg);
        }

        private void textBox_UserName_Enter(object sender, EventArgs e)
        {
            // check to see if there are protocols selected
            checkForSelectedProtocols();
        }

        private bool checkForSelectedProtocols( )
        {
            // check if Protocols have been selected on Run Samples page
            // can not load protocols from multiple users
            if (RoboSep_RunSamples.getInstance().iSelectedProtocols.Length > 0)
            {
                string sMSG = GUI_Controls.fromINI.getValue("GUI", "msgChangeUser");
                sMSG = sMSG == null ? "There are protocols selected that are associated with the current User."
                    + "Changing users will remove these protocols from your Run Samples window." : sMSG;

                RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), sMSG, "Change Users", "Ok", "Cancel");
                RoboSep_UserConsole.getInstance().frmOverlay.Show();
                prompt.ShowDialog();
                RoboSep_UserConsole.getInstance().frmOverlay.Hide();

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
                        GUI_Controls.uiLog.LOG(this, "textBox_UserName_Enter", GUI_Controls.uiLog.LogLevel.EVENTS, LOGmsg);
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        
        protected override void btn_home_Click(object sender, EventArgs e)
        {
            // changing profile to a pre-defined protocol list will not
            // require loading protocols to server
            if (textBox_UserName.Text != "All Human" && textBox_UserName.Text != "All Mouse" && textBox_UserName.Text != "Whole Blood"
                && textBox_UserName.Text != "Full List" && textBox_UserName.Text != "USER NAME")
            {
                if (ProfileChanged)
                {
                    string sMSG = "Loading User Profile...";
                    loading = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), sMSG, true, true);
                    RoboSep_UserConsole.getInstance().frmOverlay.Show();
                    loading.Show();

                    ProfileChanged = false;
                    LoadUserToServer();
                }
                else
                {
                    base.btn_home_Click(sender, e);
                }
            }
            else
            {
                base.btn_home_Click(sender, e);
            }
        }




    }
}
