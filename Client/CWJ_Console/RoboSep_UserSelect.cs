using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Tesla.Common;
using System.Threading;
using GUI_Controls;
using Tesla.OperatorConsoleControls;
using Invetech.ApplicationLog;

namespace GUI_Console
{
    public partial class RoboSep_UserSelect : BasePannel /*UserControl*/
    {
        private const char reservedChar = '^';
        private const char spaceChar = ' ';
        
        private List<UserAccount> Users;
        private IniFile UserINI;
        private IniFile LanguageINI;
        private StringFormat theFormat;
        private Color BG_ColorEven;
        private Color BG_ColorOdd;
        private Color BG_Selected;
        private Color BG_Preset;
        private Color Txt_Color;
        private static int NUM_USERS = 4;

        private Image[] defaultIcons;
        private string systemPath;
        private string SelectedUser;
        private System.Windows.Forms.ImageList imageListUser;
        private const string ProtocolSuffix = "Protocol";
        protected Graphics imageGraphics;

        private List<Image> iListSortAscend;
        private List<Image> iListSortDescend;

        private System.EventHandler evhHandleUserProtocolsReloaded = null;

        public RoboSep_UserSelect()
        {
            InitializeComponent();

            UserINI = new IniFile(IniFile.iniFile.User);
            Users = new List<UserAccount>();

            SuspendLayout();

            iListSortAscend = new List<Image>();
            iListSortDescend = new List<Image>();

            // Sorting
            iListSortDescend.Add(Properties.Resources.GE_BTN12M_sort_descend_STD);
            iListSortDescend.Add(Properties.Resources.GE_BTN12M_sort_descend_OVER);
            iListSortDescend.Add(Properties.Resources.GE_BTN12M_sort_descend_OVER);
            iListSortDescend.Add(Properties.Resources.GE_BTN12M_sort_descend_CLICK);

            iListSortAscend.Add(Properties.Resources.GE_BTN11M_sort_ascend_STD);
            iListSortAscend.Add(Properties.Resources.GE_BTN11M_sort_ascend_OVER);
            iListSortAscend.Add(Properties.Resources.GE_BTN11M_sort_ascend_OVER);
            iListSortAscend.Add(Properties.Resources.GE_BTN11M_sort_ascend_CLICK);
            button_Sort.ChangeGraphics(iListSortAscend);
            

            List<Image> ilist = new List<Image>();

            // Edit User
            ilist.Add(Properties.Resources.US_BTN05L_edit_user_STD);
            ilist.Add(Properties.Resources.US_BTN05L_edit_user_OVER);
            ilist.Add(Properties.Resources.US_BTN05L_edit_user_OVER);
            ilist.Add(Properties.Resources.US_BTN05L_edit_user_CLICK);
            button_EditUser.ChangeGraphics(ilist);

            // Clone User
            ilist.Clear();
            ilist.Add(Properties.Resources.US_BTN01L_clone_user_STD);
            ilist.Add(Properties.Resources.US_BTN01L_clone_user_OVER);
            ilist.Add(Properties.Resources.US_BTN01L_clone_user_OVER);
            ilist.Add(Properties.Resources.US_BTN01L_clone_user_CLICK);
            button_CloneUser.ChangeGraphics(ilist);

            // New User
            ilist.Clear();
            ilist.Add(Properties.Resources.US_BTN02L_add_user_STD);
            ilist.Add(Properties.Resources.US_BTN02L_add_user_OVER);
            ilist.Add(Properties.Resources.US_BTN02L_add_user_OVER);
            ilist.Add(Properties.Resources.US_BTN02L_add_user_CLICK);
            button_NewUser.ChangeGraphics(ilist);

            // Delete User
            ilist.Clear();
            ilist.Add(Properties.Resources.US_BTN03L_remove_user_STD);
            ilist.Add(Properties.Resources.US_BTN03L_remove_user_OVER);
            ilist.Add(Properties.Resources.US_BTN03L_remove_user_OVER);
            ilist.Add(Properties.Resources.US_BTN03L_remove_user_CLICK);
            button_DeleteUser.ChangeGraphics(ilist);

            // Listview 
            this.imageListUser = new System.Windows.Forms.ImageList(this.components);
            this.imageListUser.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageListUser.ImageSize = new System.Drawing.Size(40, 48);
            this.imageListUser.TransparentColor = System.Drawing.Color.Transparent;
            lvUser.SmallImageList = this.imageListUser;
            lvUser.SmallImageList.TransparentColor = System.Drawing.Color.Transparent;
            imageGraphics = Graphics.FromHwnd(lvUser.Handle);

            this.scrollBar_User.LargeChange = lvUser.VisibleRow - 1;
            lvUser.VScrollbar = this.scrollBar_User;

            ResumeLayout();

            // get Current users
            getUserProfiles();

            // set labels
            LanguageINI = RoboSep_UserConsole.getInstance().LanguageINI;
        }

        private void getUserProfiles()
        {
            List<string> usrNames = UserINI.IniGetCategories();
            usrNames.Sort();
            Users.Clear();

            for (int i = 0; i < usrNames.Count; i++)
            {
                // get user image path
                string imageKey = usrNames[i] + "Image";
                string usrImage = UserINI.GetString(imageKey);
                if (usrImage == null)
                {
                    usrImage = "Default";
                    UserINI.IniWriteValue(usrNames[i], imageKey, usrImage);
                }

                // add user to user list
                Users.Add(new UserAccount(usrNames[i], usrImage));
            }
        }

        private void UpdateUserProfiles()
        {
            getUserProfiles();
            refreshList();
        }

        private void populateList()
        {
            lvUser.View = View.Details;
            lvUser.VisibleRow = NUM_USERS;
            lvUser.GridLines = true;

            ColumnHeader ch1 = new ColumnHeader();
            ch1.Text = "User";
            ch1.Width = lvUser.ClientSize.Width;
            lvUser.Columns.Add(ch1);
            refreshList();

            lvUser.ResizeVerticalHeight(true);
            Rectangle rcScrollbar = this.scrollBar_User.Bounds;
            this.scrollBar_User.SetBounds(rcScrollbar.X, lvUser.Bounds.Y, rcScrollbar.Width, lvUser.Bounds.Height);
        }

        private void refreshList()
        {
            // add items to list view
            lvUser.SuspendLayout();
            lvUser.Items.Clear();
            imageListUser.Images.Clear();

            // Add users
            int nIndex = 0;
            string sName;
            for (int i = 0; i < Users.Count; i++)
            {
                if (Users[i] == null || string.IsNullOrEmpty(Users[i].Username) || Users[i].getIcon() == null)
                    continue;

                sName = Users[i].Username;
                nIndex = Users[i].Username.IndexOf(reservedChar);
                if (0 < nIndex)
                {
                    sName = sName.Replace(reservedChar, spaceChar); 
                }
                ListViewItem lvItem = new ListViewItem(sName);
                lvItem.Tag = Users[i].Username;
                imageListUser.Images.Add(Users[i].getIcon());
                lvItem.ImageIndex = i;
                lvUser.Items.Add(lvItem);
            }

            lvUser.ResumeLayout();
            lvUser.Refresh();
        }

        private void RoboSep_UserSelect_Load(object sender, EventArgs e)
        {
            // set up for drawing
            theFormat = new StringFormat();
            theFormat.Alignment = StringAlignment.Near;
            theFormat.LineAlignment = StringAlignment.Center;
            BG_ColorEven = Color.FromArgb(216, 217, 218);
            BG_ColorOdd = Color.FromArgb(243, 243, 243);
            BG_Selected = Color.FromArgb(78, 38, 131);
            BG_Preset = Color.FromArgb(255, 211, 168);
            Txt_Color = Color.FromArgb(95, 96, 98);

            // set up listview drag scroll properties
            if (lvUser.VERTICAL_PAGE_SIZE > (lvUser.VisibleRow - 1))
                lvUser.VERTICAL_PAGE_SIZE = lvUser.VisibleRow - 1;

            // Populate screen with users
            defaultIcons = new Image[] {
                Properties.Resources.DefaultUser_GREY,
                Properties.Resources.DefaultUser_PURPLE,
                Properties.Resources.DefaultUser_BLUE,
                Properties.Resources.DefaultUser_GREEN,
                Properties.Resources.USBIcon
            };

            populateList();
            lvUser.UpdateScrollbar();
            EnsureCurrentUserVisible();
        }

        private void EnsureCurrentUserVisible()
        {
            if (!string.IsNullOrEmpty(RoboSep_UserConsole.strCurrentUser))
            {
                foreach (ListViewItem lvItem in lvUser.Items)
                {
                    // ensure the previous selected item is visible 
                    if (lvItem.Text == RoboSep_UserConsole.strCurrentUser)
                    {
                        // Select the previous selected protocol
                       // lvItem.Selected = true;
                        lvUser.EnsureVisible(lvItem.Index);
                        lvUser.UpdateScrollbar();
                        break;
                    }
                }
            }
        }

        private void listview_lvUser_Click(object sender, EventArgs e)
        {
            if (lvUser.SelectedItems.Count == 0)
                return;

            SelectedUser = lvUser.SelectedItems[0].Tag as string;
            if (string.IsNullOrEmpty(SelectedUser))
                return;

            SelectedUser = SelectedUser.Trim();
#if false
            string userNameText = LanguageINI.GetString("lblUserName");
            if (userNameText.ToLower() == SelectedUser.ToLower())
            {
                this.button_NewUser.Tag = SelectedUser;
                this.button_NewUser_Click(this.button_NewUser, new EventArgs());
                return;
            }
#endif
            SelectUser(SelectedUser);
 
            // LOG
            string logMSG = "User '" + lvUser.SelectedItems[0].Text + "' is selected to run RoboSep-S";
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG); 
        }


        private void listview_lvUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            lvUser.UpdateScrollbar();
        }


        private void listview_lvUser_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            ListViewItem lvItem = e.Item;
            string sUserName = lvItem.Tag as string;
            if (string.IsNullOrEmpty(sUserName))
            {
                sUserName = lvItem.Text;
            }

            Rectangle rc = e.Bounds;
            SolidBrush theBrush;
            if (e.Item.Selected)
            {
                theBrush = new SolidBrush(BG_Selected);
                e.Graphics.FillRectangle(theBrush, rc);
                theBrush.Dispose();
            }
            else if (RoboSep_UserDB.getInstance().IsPresetUser(sUserName))
            {
                theBrush = new SolidBrush(BG_Preset);
                e.Graphics.FillRectangle(theBrush, rc);
                theBrush.Dispose();
            }
            else if (e.ItemIndex % 2 == 0)
            {
                theBrush = new SolidBrush(BG_ColorEven);
                e.Graphics.FillRectangle(theBrush, rc);
                theBrush.Dispose();
            }
            else
            {
                theBrush = new SolidBrush(BG_ColorOdd);
                e.Graphics.FillRectangle(theBrush, rc);
                theBrush.Dispose();
            }

        }

        private void listview_lvUser_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {

            ListViewItem lvItem = e.Item;
            Rectangle rc = e.Bounds;

            // June 26, 2013 Sunny
            // Commented out because image is not displayed in the list
            /*
            if (lvItem.ImageList != null)
            {
                rc = lvItem.GetBounds(ItemBoundsPortion.Label);
            }
            else
            {
                rc = e.Bounds;
            }
             */ 
            // format text
            rc.X += Margin.Left + 5;

            SolidBrush theBrush;
            if (e.Item.Selected)
            {
                theBrush = new SolidBrush(Color.White);
                e.Graphics.DrawString(e.Item.Text, lvUser.Font, theBrush, rc, theFormat);
                theBrush.Dispose();
            }
            else
            {
                theBrush = new SolidBrush(Txt_Color);
                e.Graphics.DrawString(e.Item.Text, lvUser.Font, theBrush, rc, theFormat);
                theBrush.Dispose();
            }

        }

        public string User
        {
            get
            {
                return SelectedUser;
            }
        }

        private void listView_users_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            // format text
            SolidBrush theBrush;
            if (e.Item.Selected)
            {
                theBrush = new SolidBrush(Color.White);
                e.Graphics.DrawString(e.Item.Text, lvUser.Font, theBrush, e.Bounds, theFormat);
                theBrush.Dispose();
            }
            else
            {
                theBrush = new SolidBrush(Txt_Color);
                e.Graphics.DrawString(e.Item.Text, lvUser.Font, theBrush, e.Bounds, theFormat);
                theBrush.Dispose();
            }

        }

        private void SelectUser(string UserLogin)
        {
            if (string.IsNullOrEmpty(UserLogin))
                return;
            
            string usrName = UserLogin;
            RoboSep_UserConsole.strCurrentUser = usrName;

#if false
            // load user preferences
            RoboSep_UserDB.getInstance().loadCurrentUserPreferences(usrName);

            // load user protocols
            List<RoboSep_Protocol> tempList = RoboSep_UserDB.getInstance().loadUserProtocols(usrName);

            // it could be new user with no protocols 
            if (tempList != null && tempList.Count > 0)
            {
                // load user protocols
                RoboSep_Protocols.getInstance().LoadUserToServer(usrName);
                RoboSep_Protocols.getInstance().setProtocolLoading(true);
                SeparatorGateway.GetInstance().separatorUpdating = true;

                if (evhHandleUserProtocolsReloaded == null)
                    evhHandleUserProtocolsReloaded = new EventHandler(HandleUserProtocolsReloadedBeforeExiting);

                RoboSep_UserConsole.getInstance().NotifyUserSeparationProtocolsUpdated += evhHandleUserProtocolsReloaded;
                return;
            }
#else
            // load user protocols
            List<RoboSep_Protocol> tempList = RoboSep_UserDB.getInstance().loadUserProtocols(usrName);

            // it could be new user with no protocols 
            if (tempList != null && tempList.Count > 0)
            {
                // load user protocols
                RoboSep_Protocols.getInstance().LoadUserToServer(usrName);
                RoboSep_Protocols.getInstance().setProtocolLoading(true);
                SeparatorGateway.GetInstance().separatorUpdating = true;

                // load user preferences
                RoboSep_UserDB.getInstance().loadCurrentUserPreferences(usrName);

                if (evhHandleUserProtocolsReloaded == null)
                    evhHandleUserProtocolsReloaded = new EventHandler(HandleUserProtocolsReloadedBeforeExiting);

                RoboSep_UserConsole.getInstance().NotifyUserSeparationProtocolsUpdated += evhHandleUserProtocolsReloaded;
                return;
            }
            else
            {
                // load user preferences
                RoboSep_UserDB.getInstance().loadCurrentUserPreferences(usrName);
            }

#endif
            // open run samples window
            RoboSep_UserConsole myUC = RoboSep_UserConsole.getInstance();
            RoboSep_UserConsole.ctrlCurrentUserControl = RoboSep_RunSamples.getInstance();
            RoboSep_RunSamples.getInstance().UserName = usrName;
            RoboSep_RunSamples.getInstance().Enabled = false;
            myUC.SuspendLayout();
            myUC.Controls.Remove(this);
            myUC.Controls.Add(RoboSep_RunSamples.getInstance());
            myUC.ResumeLayout();
            if (RoboSep_RunSamples.getInstance().IsInitialized)
            {
                RoboSep_RunSamples.getInstance().ReInitialize();
            }
            RoboSep_RunSamples.getInstance().Enabled = true;
        }

        private void HandleUserProtocolsReloadedBeforeExiting(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("--------- RoboSep_UserSelect: HandleUserProtocolsReloadedBeforeExiting is called. ThreadID = {0}, ----------", Thread.CurrentThread.ManagedThreadId));

            RoboSep_UserConsole.getInstance().NotifyUserSeparationProtocolsUpdated -= evhHandleUserProtocolsReloaded;
            
            RoboSep_Protocols.getInstance().setProtocolLoading(false);
            
            // open run samples window
            RoboSep_UserConsole myUC = RoboSep_UserConsole.getInstance();
            RoboSep_UserConsole.ctrlCurrentUserControl = RoboSep_RunSamples.getInstance();
            RoboSep_RunSamples.getInstance().UserName = SelectedUser;
            RoboSep_RunSamples.getInstance().Enabled = false;
            myUC.SuspendLayout();
            myUC.Controls.Remove(this);
            myUC.Controls.Add(RoboSep_RunSamples.getInstance());
            myUC.ResumeLayout();
            if (RoboSep_RunSamples.getInstance().IsInitialized)
            {
                RoboSep_RunSamples.getInstance().ReInitialize();
            }
            RoboSep_RunSamples.getInstance().Enabled = true;
        }

        public bool LoadUSBPicture(string UserName)
        {
            // load from usb....
            systemPath = RoboSep_UserDB.getInstance().sysPath;

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "Report SysPath: " + systemPath); 
            //systemPath = @"C:\Program Files (x86)\STI\RoboSep\";

            string IconsPath = systemPath + "images\\";
            if (!Directory.Exists(IconsPath))
                Directory.CreateDirectory(IconsPath);

            // search for usb directory
            string[] USBdirs = new string[] { "D:\\", "E:\\", "F:\\", "G:\\" };
            int usbDirIndex = -1;
            for (int i = 0; i < USBdirs.Length; i++)
            {
                if (Directory.Exists(USBdirs[i]))
                {
                    usbDirIndex = i;
                    break;
                }
            }

            if (usbDirIndex > -1)
            {
                string sTitle = LanguageINI.GetString("headerSelectImageFile");
                FileBrowser fb = new FileBrowser(RoboSep_UserConsole.getInstance(), sTitle, USBdirs[usbDirIndex], IconsPath,
                    new string[] { ".jpg", ".jpeg", ".png", ".tga", ".bmp" }, FileBrowser.BrowseResult.SelectFile);
                fb.ShowDialog();

                // make sure that file browser finished properly
                if (fb.DialogResult == DialogResult.OK)
                {

                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "File Browser result: OK!!"); 
                    // store target file path
                    string fileTarget = fb.Target;

                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "File target = " + fileTarget); 
                    // generate icon graphic
                    string IconPath = GenerateProfileIcon(fileTarget, UserName);

                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "New User " + UserName + " Icon Path : " + IconPath); 
                    // create user info in INI file
                    UserINI.IniWriteValue(UserName, (UserName + "Image"), IconPath);
                }

                fb.Dispose();

                return true;

            }
            else
            {

                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "Failed to locate USB Drive"); 
            }
            return false;
        }


        private void button_NewUser_Click(object sender, EventArgs e)
        {
            SuspendLayout();

            bool bNewUser = true;
            string sUserNameParameter = string.Empty;
            Button_Circle btn = sender as Button_Circle;
            if (btn != null)
            {
                sUserNameParameter = btn.Tag as string;
                if (!string.IsNullOrEmpty(sUserNameParameter))
                {
                    bNewUser = false;
                }
            }

            // create new user select control form
            List<string> UserNames = new List<string>();
            for (int i = 0; i < Users.Count; i++)
            {
                string name = Users[i].Username;
                UserNames.Add(name);
            }

            // Show Overlay window
            RoboSep_UserConsole.showOverlay();

            Form_UserLoginNew NewUserForm = new Form_UserLoginNew(UserNames, sUserNameParameter, bNewUser);

            NewUserForm.ShowDialog();
            string newUserLoginID = "";
            if (NewUserForm.DialogResult == DialogResult.OK)
            {
                newUserLoginID = (NewUserForm.NewUserLoginID);

                // update the list
                UpdateUserProfiles();

                // ensure the new user is visible
                ListViewItem lvItem = lvUser.FindItemWithText(newUserLoginID);
                if (lvItem != null)
                {
                    lvItem.Selected = true;
                    lvUser.EnsureVisible(lvItem.Index);
                }

                lvUser.UpdateScrollbar();
            }

            NewUserForm.Dispose();
                
            RoboSep_UserConsole.hideOverlay();

            ResumeLayout();

            // LOG
            string logMSG = "Opening new user login screen";
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG); 
        }


        private void button_EditUser_Click(object sender, EventArgs e)
        {
            // Get list of user names
            List<string> UserNames = new List<string>();
            for (int i = 0; i < Users.Count; i++)
            {
                if (Users[i].Username != RoboSep_UserConsole.strCurrentUser)
                {
                    UserNames.Add(Users[i].Username);
                }
            }

            // Show Overlay window
            RoboSep_UserConsole.showOverlay();
            RoboSep_UserConsole.getInstance().frmOverlay.BringToFront();

            // create new user select control form
            IniFile LanguageINI = GUI_Console.RoboSep_UserConsole.getInstance().LanguageINI;
            string windowTitle = LanguageINI.GetString("lblSelectUserToEdit");

            Form_UserSelect UserSelectMenu = new Form_UserSelect(UserNames, windowTitle);
            UserSelectMenu.ShowDialog();

            RoboSep_UserConsole.hideOverlay();
            if (UserSelectMenu.DialogResult != DialogResult.OK)
            {
                UserSelectMenu.Dispose();
                ResumeLayout();
                return;
            }

            // check to see if user name was selected
            string selectedUser = UserSelectMenu.User;
            UserSelectMenu.Dispose();
            if (selectedUser == null && selectedUser == string.Empty)
                return;

            // Show Overlay window
            RoboSep_UserConsole.showOverlay();

            Form_UserLoginNew NewUserForm = new Form_UserLoginNew(UserNames, selectedUser, false);
            NewUserForm.ShowDialog();
            string newUserLoginID = "";
            if (NewUserForm.DialogResult == DialogResult.OK)
            {
                newUserLoginID = (NewUserForm.NewUserLoginID);

                // update the list
                UpdateUserProfiles();

                // ensure the edit user is visible
                ListViewItem lvItem = lvUser.FindItemWithText(newUserLoginID);
                if (lvItem != null)
                {
                    lvItem.Selected = true;
                    lvUser.EnsureVisible(lvItem.Index);
                }

                lvUser.UpdateScrollbar();
            }
            NewUserForm.Dispose();
            RoboSep_UserConsole.hideOverlay();

            ResumeLayout();

            // LOG
            string logMSG = "Opening user screen to select user to be edited.";
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG); 
        }

        private void button_CloneUser_Click(object sender, EventArgs e)
        {
            // Get list of user names
            List<string> UserNames = new List<string>();
            for (int i = 0; i < Users.Count; i++)
            {
                UserNames.Add(Users[i].Username);
            }

            // Show Overlay window
            RoboSep_UserConsole.showOverlay();
            RoboSep_UserConsole.getInstance().frmOverlay.BringToFront();

            // create new user select control form
            IniFile LanguageINI = GUI_Console.RoboSep_UserConsole.getInstance().LanguageINI;
            string windowTitle = LanguageINI.GetString("lblSelectUserToClone");

            Form_UserSelect UserSelectMenu = new Form_UserSelect(UserNames, windowTitle);
            UserSelectMenu.ShowDialog();

            RoboSep_UserConsole.hideOverlay();
            if (UserSelectMenu.DialogResult != DialogResult.OK)
            {
                UserSelectMenu.Dispose();
                ResumeLayout();
                return;
            }

            // check to see if user name was selected
            string selectedUser = UserSelectMenu.User;
            UserSelectMenu.Dispose();
            if (selectedUser == null && selectedUser == string.Empty)
                return;

            // Show Overlay window
            RoboSep_UserConsole.showOverlay();

            Form_UserLoginNew NewUserForm = new Form_UserLoginNew(UserNames, null, true);
            NewUserForm.ShowDialog();
            string newUserLoginID = "";
            if (NewUserForm.DialogResult == DialogResult.OK)
            {
                newUserLoginID = (NewUserForm.NewUserLoginID);

                CloneUser(newUserLoginID, selectedUser);

                // update the list
                UpdateUserProfiles();

                // ensure the new user is visible
                ListViewItem lvItem = lvUser.FindItemWithText(newUserLoginID);
                if (lvItem != null)
                {
                    lvItem.Selected = true;
                    lvUser.EnsureVisible(lvItem.Index);
                }

                lvUser.UpdateScrollbar();
            }
            NewUserForm.Dispose();
            RoboSep_UserConsole.hideOverlay();

            ResumeLayout();

            // LOG
            string logMSG = "Opening user screen to select user to be cloned.";
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG); 

        }

        private void button_DeleteUser_Click(object sender, EventArgs e)
        {
            // Get list of user names
            List<string> UserNames = new List<string>();
            for (int i = 0; i < Users.Count; i++)
            {
                if (Users[i].Username != RoboSep_UserConsole.strCurrentUser && !RoboSep_UserDB.getInstance().IsPresetUser(Users[i].Username))
                {
                    UserNames.Add(Users[i].Username);
                }
            }

            // Show Overlay window
            RoboSep_UserConsole.showOverlay();
            RoboSep_UserConsole.getInstance().frmOverlay.BringToFront();

            // create new user select control form
            IniFile LanguageINI = GUI_Console.RoboSep_UserConsole.getInstance().LanguageINI;
            string windowTitle = LanguageINI.GetString("lblSelectUserToDelete");

            Form_UserSelect UserSelectMenu = new Form_UserSelect(UserNames, windowTitle);
            UserSelectMenu.ShowDialog();

            RoboSep_UserConsole.hideOverlay();
            if (UserSelectMenu.DialogResult != DialogResult.OK)
            {
                UserSelectMenu.Dispose();                
                ResumeLayout();
                return;
            }

            // check to see if user name was selected
            string selectedUser = UserSelectMenu.User;
            UserSelectMenu.Dispose();                
            if (selectedUser == null && selectedUser == string.Empty)
                return;

            // Show Overlay window
            RoboSep_UserConsole.showOverlay();

            DeleteUser(selectedUser);

            // update the list
            UpdateUserProfiles();

            lvUser.UpdateScrollbar();

            RoboSep_UserConsole.hideOverlay();

            ResumeLayout();

            // LOG
            string logMSG = "Opening user screen to select user to be deleted.";
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG); 
        }

        private void CloneUser(string usrName, string selectedUser)
        {
            // clone user
            List<string> userLines = UserINI.IniGetKeys(selectedUser);

            System.Collections.Specialized.NameValueCollection userItems = GUI_Controls.UserDetails.getSection("USER", selectedUser);

            //enumerate the keys
            string temp;
            string[] parts;
            string selectUserProtocolPrefix = selectedUser.Trim() + ProtocolSuffix;
            string newUserProtocolPrefix = usrName.Trim() + ProtocolSuffix;

            foreach (string key in userItems)
            {
                if (key.ToLower() == selectUserProtocolPrefix.ToLower())
                {
                    temp = userItems[key];
                    if (string.IsNullOrEmpty(temp))
                        continue;

                    parts = temp.Split(',');
                    for (int i = 0; i < parts.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(parts[i]) && parts[i].Trim().Length > 0 && parts[i].Trim().Substring(0, 2) != @"//")
                        {
                            UserDetails.InsertEntry("USER", newUserProtocolPrefix, parts[i].Trim());
                        }
                    }
                    break;
                }
            }
        }

        private void DeleteUser(string selectedUser)
        {
            if (string.IsNullOrEmpty(selectedUser))
            {
                return;
            }

            // Remove user preference file for this user
            RoboSep_UserDB.getInstance().deleteUserPreferences(selectedUser);

            // Delete all MRU protocols 
            ProtocolMRUList MRUList = new ProtocolMRUList();
            if (MRUList != null)
            {
                MRUList.DeleteAllMostRecentlyUsedProtocols(selectedUser);
            }

            // Delete the image file associated with this user
            UserDetails.deleteUserImageFile(selectedUser);

            // Remove all the protocols associated with this user
            UserDetails.removeSection("USER", selectedUser);
        }

        private void populateUserProtocols()
        {
            /*
                        // update user name
                        string usrName = textBox_UserName.Text;
                        RoboSep_UserConsole.strCurrentUser = usrName;

                        // open edit list page
                        RoboSep_UserConsole UC = RoboSep_UserConsole.getInstance();
                        UC.Controls.Remove(this);
                        RoboSep_ProtocolList PL = RoboSep_ProtocolList.getInstance();
                        PL.strUserName = usrName;
                        PL.resetForm();
                        UC.Controls.Add(PL);
                        RoboSep_UserConsole.ctrlCurrentUserControl = RoboSep_Protocols.getInstance();
                        RoboSep_UserProtocolList.getInstance().strUserName = usrName;
                        RoboSep_Protocols.getInstance().LoadRequired = true;
             * */
        }

        #region GRAPHIC_EDITING

        private string GenerateProfileIcon(string imgPath, string UserName)
        {

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "Generating Profile Icon"); 
            Image source = Image.FromFile(imgPath);

            Image workingImage = CropImage(source);

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "Cropped Image"); 
            // save copy
            //string cropPath = changeFileName( imgPath, (UserName + "_crop"));
            //saveJPG(cropPath, (Bitmap)workingImage, 100L);

            workingImage = scaleImage(workingImage, 48);

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "Scaled Image"); 

            //string scaledPath = changeFileName(imgPath, (UserName + "_scale"));
            //saveJPG(scaledPath, (Bitmap)workingImage, 100L);

            workingImage = frameImage(workingImage);

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "Framed Image");             
            string framedPath = changeFileName(imgPath, (UserName + "_framed"), ".png");
            try
            {
                // fails when saving over file it is currently using
                savePNG(framedPath, (Bitmap)workingImage, 100L);
                workingImage.Dispose();
            }
            catch
            {
                // change name and try to save again
                framedPath = changeFileName(imgPath, (UserName + "1_framed"), ".png");
                savePNG(framedPath, (Bitmap)workingImage, 100L);
                workingImage.Dispose();
            }

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "Profile Icon Saved");             
            return framedPath;
        }

        private string changeFileName(string originalPath, string newFileName, string extension)
        {
            StringBuilder temp = new StringBuilder();
            temp.Append(systemPath);
            temp.Append("images\\");
            temp.Append(newFileName);
            temp.Append(extension);

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "Save User Icon: " + temp.ToString());             
            return temp.ToString();
        }

        private void saveJPG(string path, Bitmap img, long quality)
        {
            // set image quality
            EncoderParameter jpgQuality = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

            // set image compression codec
            ImageCodecInfo jpgCodec = getCodecInfo("image/jpeg");

            if (jpgCodec == null)
                return;

            EncoderParameters eParams = new EncoderParameters();
            eParams.Param[0] = jpgQuality;

            img.Save(path, jpgCodec, eParams);
        }

        private void savePNG(string path, Bitmap img, long quality)
        {
            // set image quality
            EncoderParameter pngQuality = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

            // set image compression codec
            ImageCodecInfo pngCodec = getCodecInfo("image/png");

            if (pngCodec == null)
                return;

            EncoderParameters eParams = new EncoderParameters();
            eParams.Param[0] = pngQuality;

            if (File.Exists(path))
            {

            }
            img.Save(path, pngCodec, eParams);
        }

        private ImageCodecInfo getCodecInfo(string type)
        {
            // get all codecs
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // find jpg encoder
            for (int i = 0; i < codecs.Length; i++)
            {
                if (codecs[i].MimeType == type)
                    return codecs[i];
            }
            return null;
        }

        private Image CropImage(Image img)
        {
            Bitmap imgCopy = new Bitmap(img);
            int X = imgCopy.Size.Width;
            int Y = imgCopy.Size.Height;
            int min;
            bool portrait;
            int centeringOffset;
            Rectangle sqCrop;

            if (X < Y)
            {
                min = X;
                portrait = true;
                // centre crop
                centeringOffset = (int)((double)(Y - X) / 2.0);
            }
            else
            {
                min = Y;
                portrait = false;
                // centre crop
                centeringOffset = (int)((double)(X - Y) / 2.0);
            }

            if (portrait)
            {
                sqCrop = new Rectangle(0, centeringOffset, min, min);
            }
            else
            {
                sqCrop = new Rectangle(centeringOffset, 0, min, min);
            }

            Bitmap imgCropped = imgCopy.Clone(sqCrop, imgCopy.PixelFormat);
            
            imgCopy.Dispose();
            
            return (Image)(imgCropped);
        }

        private Image scaleImage(Image img, int pixelDimension)
        {

            Bitmap imgResized = new Bitmap(pixelDimension, pixelDimension);
            Graphics g = Graphics.FromImage((Image)imgResized);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            g.DrawImage(img, 0, 0, pixelDimension, pixelDimension);
            g.Dispose();

            return (Image)imgResized;
        }

        private Image frameImage(Image img)
        {
            Image frame = Properties.Resources.CustomUserPic_Border; //Image.FromFile(systemPath + "\\images\\Frame.png");
            Bitmap framedImage = new Bitmap(frame.Size.Width, frame.Size.Height);

            Graphics g = Graphics.FromImage(framedImage);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            //g.DrawImage(img, new Rectangle(0, 0, frame.Size.Width, frame.Size.Height), 
            //    new Rectangle(0, 0, frame.Size.Width, frame.Size.Height), GraphicsUnit.Pixel);
            g.DrawImage(img, new Point(0, 0));
            g.DrawImage(frame, new Rectangle(new Point(0, 0), frame.Size));


            g.Save();

            g.Dispose();
            return (Image)framedImage;
        }

        #endregion


        private void button_HeaderUser_Click(object sender, EventArgs e)
        {

        }

        private void button_Sort_Click(object sender, EventArgs e)
        {
            if (this.lvUser.Items.Count == 0)
                return;

            // get currently selected item
            string selectedUserName = "";
            if (this.lvUser.SelectedItems.Count > 0)
            {
                selectedUserName = lvUser.SelectedItems[0].Text;
            }
            switch (this.lvUser.Sorting)
            {
                case SortOrder.Ascending:
                    this.lvUser.Sorting = SortOrder.Descending;
                    button_Sort.ChangeGraphics(iListSortDescend);
                    break;
                case SortOrder.Descending:
                    this.lvUser.Sorting = SortOrder.Ascending;
                    button_Sort.ChangeGraphics(iListSortAscend);
                    break;
                case SortOrder.None:
                    this.lvUser.Sorting = SortOrder.Ascending;
                    button_Sort.ChangeGraphics(iListSortAscend);
                    break;
                default:
                    break;
            }
            this.lvUser.Sort();

            if (!string.IsNullOrEmpty(selectedUserName))
            {
                foreach (ListViewItem lvItem in lvUser.Items)
                {
                    // ensure the previous selected item is visible 
                    if (lvItem.Text == selectedUserName)
                    {
                        // Select the previous selected protocol
                        lvItem.Selected = true;
                        lvUser.EnsureVisible(lvItem.Index);
                        break;
                    }
                }
            }
        }
    }
    // class
    #region USERACCOUNT
    public class UserAccount
    {
        public string Username;
        public string IconPath;
        public Image UserIcon;

        public UserAccount(string User, string imgPath)
        {
            Username = User;
            IconPath = imgPath;
        }

        public Image getIcon()
        {
            Image icon = null;
            if (IconPath.StartsWith("Default"))
            {
                // set default icon
                switch (IconPath)
                {
                    default:
                        icon = Properties.Resources.DefaultUser_GREY;
                        break;
                    case "Default1":
                        icon= Properties.Resources.DefaultUser_GREY;
                        break;
                    case "Default2":
                        icon = Properties.Resources.DefaultUser_PURPLE;
                        break;
                    case "Default3":
                        icon = Properties.Resources.DefaultUser_BLUE;
                        break;
                    case "Default4":
                        icon =  Properties.Resources.DefaultUser_GREEN;
                        break;
                }
            }
            else
            {
                string iPath = IconPath;
                
                if (File.Exists(iPath))
                {
                    FileInfo iFile = new FileInfo(iPath);
                    switch (iFile.Extension.ToLower())
                    {
                        case (".jpg"):
                        case (".bmp"):
                        case (".png"):
                        case (".tga"):
                        case (".jpeg"):
                            // create Image class and store
                            // file as graphic
                            icon = Image.FromFile(iPath);
                            break;
                        default:
                            // not a graphic file
                            icon = GUI_Controls.Properties.Resources.UserIcon01;
                            break;
                    }
                }
                return icon;
            }

            return GUI_Controls.Properties.Resources.UserIcon01;
        }
    }

    #endregion

    // class
    #region USER_VISIBLE_ELEMENTS
    public class UserVisibleElements
    {
        public Label lblUserName;
        public PictureBox pbUserIcon;

        public UserVisibleElements(Label usrLabel, PictureBox usrIcon)
        {
            lblUserName = usrLabel;
            pbUserIcon = usrIcon;
            lblUserName.Visible = false;
            pbUserIcon.Visible = false;
        }

        public void attachUser(UserAccount newUser)
        {
            lblUserName.Text = newUser.Username;
            lblUserName.Visible = true;

            pbUserIcon.Image = newUser.getIcon();
            pbUserIcon.Visible = true;
        }

        public void clearUser()
        {
            lblUserName.Text = string.Empty;
            pbUserIcon.Image = null;

            pbUserIcon.Visible = false;
            lblUserName.Visible = false;
        }
    }
    #endregion
}
