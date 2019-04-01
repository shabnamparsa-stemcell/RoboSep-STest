using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using Invetech.ApplicationLog;
using GUI_Controls;
using Tesla.Common;


namespace GUI_Console
{
    public partial class RoboSep_UserPreferences : UserControl
    {
        // Language file
        private IniFile LanguageINI = GUI_Console.RoboSep_UserConsole.getInstance().LanguageINI;

        private const string UserPreferencesFileExtension = ".ini";
        private const string Yes = "yes";
        private const string No = "no";
        private const string True = "true";
        private const string False = "false";
 
        public Dictionary<string, PreferenceSettings> dictUserPreferencesFromConfig = null;
        public Dictionary<string, PreferenceSettings> dictDevicePreferencesFromConfig = null;

        private Dictionary<string, bool> dictMasterUserPreferences = new Dictionary<string, bool>();
        private Dictionary<string, bool> dictMasterDevicePreferences = new Dictionary<string, bool>();

        private Dictionary<string, bool> dictUserPreferences = new Dictionary<string, bool>();
        private Dictionary<string, bool> dictDevicePreferences = new Dictionary<string, bool>();

        private HorizontalTabs.tabs tabUserSettings = HorizontalTabs.tabs.Tab1;
        private HorizontalTabs.tabs tabDeviceSettings = HorizontalTabs.tabs.Tab2;

        private string lastSelectedItem;

        // Listview Drawing Vars
        private StringFormat theFormat;
        private Color BG_ColorEven;
        private Color BG_ColorOdd;
        private Color BG_Selected;
        private Color Txt_Color;
        private const int CHECKBOX_SIZE = 32;
        private const int BUTTON_SIZE = 32;
        private const int CellPaddingSize = 4;
        private const int EndPaddingSize = 6;

        private int thumbSize = 15;

        private bool bIsNewUser = false;
        private string userName;

        private List<Image> ilistSaveNormal = new List<Image>();
        private List<Image> ilistSaveDirty = new List<Image>();

        private Rectangle rcScrollIndicator;
        private Rectangle rcListViewUserPreferences;
        private int ListViewColumn1Width;
        private int SpacingBetweenLVAndIndicator = 0;

        private object syncobj;
        public event EventHandler GetUserPreferences = null;
        public event EventHandler ClosingUserPreferencesApp = null;

        private Action<string, bool> ItemAction;
        private RoboMessagePanel5 loading = null;

        public RoboSep_UserPreferences(string UserName, bool bNewUser, bool bFromHomeScreen)
        {
            InitializeComponent();

            SuspendLayout();

            this.DoubleBuffered = true;
            this.UpdateStyles();

            //change toggle button graphics
            List<Image> ilist = new List<Image>();

            ilist.Clear();
            ilist.Add(Properties.Resources.GE_BTN03M_up_arrow_STD);
            ilist.Add(Properties.Resources.GE_BTN03M_up_arrow_OVER);
            ilist.Add(Properties.Resources.GE_BTN03M_up_arrow_OVER);
            ilist.Add(Properties.Resources.GE_BTN03M_up_arrow_CLICK);
            button_ScrollUp.ChangeGraphics(ilist);

            ilist.Clear();
            ilist.Add(Properties.Resources.GE_BTN04M_down_arrow_STD);
            ilist.Add(Properties.Resources.GE_BTN04M_down_arrow_OVER);
            ilist.Add(Properties.Resources.GE_BTN04M_down_arrow_OVER);
            ilist.Add(Properties.Resources.GE_BTN04M_down_arrow_CLICK);
            button_ScrollDown.ChangeGraphics(ilist);

            ilist.Clear();
            if (bFromHomeScreen)
            {
                ilist.Add(Properties.Resources.GE_BTN22L_home_STD);
                ilist.Add(Properties.Resources.GE_BTN22L_home_OVER);
                ilist.Add(Properties.Resources.GE_BTN22L_home_OVER);
                ilist.Add(Properties.Resources.GE_BTN22L_home_CLICK);
            }
            else
            {
                ilist.Add(Properties.Resources.L_104x86_single_arrow_left_STD);
                ilist.Add(Properties.Resources.L_104x86_single_arrow_left_OVER);
                ilist.Add(Properties.Resources.L_104x86_single_arrow_left_OVER);
                ilist.Add(Properties.Resources.L_104x86_single_arrow_left_CLICK);
            }

            button_Cancel.ChangeGraphics(ilist);

            ilistSaveDirty.Clear();
            ilistSaveDirty.Add(Properties.Resources.L_104x86_save_green);
            ilistSaveDirty.Add(Properties.Resources.GE_BTN10L_save_OVER);
            ilistSaveDirty.Add(Properties.Resources.GE_BTN10L_save_OVER);
            ilistSaveDirty.Add(Properties.Resources.GE_BTN10L_save_CLICK);

            ilistSaveNormal.Clear();
            if (bIsNewUser == true)
            {
                ilistSaveNormal.Add(Properties.Resources.GE_BTN20L_ok_STD);
                ilistSaveNormal.Add(Properties.Resources.GE_BTN20L_ok_OVER);
                ilistSaveNormal.Add(Properties.Resources.GE_BTN20L_ok_OVER);
                ilistSaveNormal.Add(Properties.Resources.GE_BTN20L_ok_CLICK);
            }
            else
            {
                ilistSaveNormal.Add(Properties.Resources.GE_BTN10L_save_STD);
                ilistSaveNormal.Add(Properties.Resources.GE_BTN10L_save_OVER);
                ilistSaveNormal.Add(Properties.Resources.GE_BTN10L_save_OVER);
                ilistSaveNormal.Add(Properties.Resources.GE_BTN10L_save_CLICK);
            }
            button_Save.ChangeGraphics(ilistSaveNormal);

            this.userName = UserName;
            this.bIsNewUser = bNewUser;

            ResumeLayout();
        }

        private void RoboSep_UserPreferences_Load(object sender, EventArgs e)
        {
            if (dictUserPreferencesFromConfig == null)
            {
                dictUserPreferencesFromConfig = new Dictionary<string, PreferenceSettings>();
            }
            if (dictDevicePreferencesFromConfig == null)
            {
                dictDevicePreferencesFromConfig = new Dictionary<string, PreferenceSettings>();
            }
            syncobj = new object();
            Bitmap b1 = new Bitmap(Properties.Resources.GE_BTN08S_unselect_STD);
            Bitmap b2 = new Bitmap(Properties.Resources.GE_BTN07S_select_CLICK);
            Bitmap b3 = new Bitmap(Properties.Resources.PF_BTN01S_details_STD);
 
            this.imageList1.ColorDepth = ColorDepth.Depth32Bit;
            this.imageList1.ImageSize = new Size(40, 40); // this will affect the row height
            this.imageList1.Images.Add("unchecked", b1);
            this.imageList1.Images.Add("checked", b2);
            this.imageList1.Images.Add("help", b3);
            listView_UserPref.SmallImageList = this.imageList1;

            UserNameHeader.Text = userName;

            // set up for drawing
            theFormat = new StringFormat();
            theFormat.Alignment = StringAlignment.Near;
            theFormat.LineAlignment = StringAlignment.Center;
            BG_ColorEven = Color.FromArgb(216, 217, 218);
            BG_ColorOdd = Color.FromArgb(243, 243, 243);
            BG_Selected = Color.FromArgb(78, 38, 131);
            Txt_Color = Color.FromArgb(95, 96, 98);

            // show loading protocol message
            string title = LanguageINI.GetString("headerLoadingUserPreferences");
            string msg = LanguageINI.GetString("msgLoadingUserPreferences");
            loading = new RoboMessagePanel5(RoboSep_UserConsole.getInstance(), title, msg, GUI_Controls.GifAnimationMode.eUploadingMultipleFiles);
//            RoboSep_UserConsole.showOverlay();  //CWJ TOOKOUT  
            loading.Show();
            button_Cancel.Enabled = false;
            button_Save.Enabled = false;
            timer1.Start();

            b1.Dispose();
            b2.Dispose();
            b3.Dispose();
            this.Focus();
        }

        private void Initialize()
        {
            LoadUserPreferences(userName);
//            RoboSep_UserConsole.hideOverlay();  //CWJ TOOKOUT
            if (loading != null)
            {
                loading.Close();
                loading = null;
            }

            RefreshPreferencesDisplayList(dictUserPreferencesFromConfig);
            listView_UserPref.ResizeVerticalHeight(true);

            this.scrollIndicator1.LargeChange = listView_UserPref.VisibleRow - 1;
            this.scrollIndicator1.Maximum = listView_UserPref.Items.Count;
            listView_UserPref.VScrollbar = this.scrollIndicator1;

            Rectangle rcScrollbar = this.scrollIndicator1.Bounds;
            this.scrollIndicator1.SetBounds(rcScrollbar.X, listView_UserPref.Bounds.Y, rcScrollbar.Width, listView_UserPref.Bounds.Height);
            listView_UserPref.UpdateScrollbar();

            SetUpScrollIndicatorThumbs();

            rcScrollIndicator = scrollIndicator1.Bounds;
            rcListViewUserPreferences = listView_UserPref.Bounds;
            SpacingBetweenLVAndIndicator = rcScrollIndicator.X - (rcListViewUserPreferences.X + rcListViewUserPreferences.Width);
            ListViewColumn1Width = listView_UserPref.Columns[0].Width;

            UpdateScrollIndicatorVisibility();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // do initialization
            timer1.Stop();
            Initialize();
            button_Cancel.Enabled = true;
            button_Save.Enabled = true;
        }

        private void SetUpScrollIndicatorThumbs()
        {
            Size s = new Size(scrollIndicator1.Width / 2, thumbSize);
            GraphicsPath gpMarker = new GraphicsPath();
            PointF[] markerPoly = new PointF[4];
            markerPoly[0] = new PointF(0.0F, s.Height / 2);
            markerPoly[1] = new PointF(s.Width, 0.00F);
            markerPoly[2] = new PointF(s.Width / 2, s.Height / 2);
            markerPoly[3] = new PointF(s.Width, s.Height);
            gpMarker.AddPolygon(markerPoly);
            scrollIndicator1.ThumbCustomShape = gpMarker;
        }

        private void UpdateScrollIndicatorVisibility()
        {
            int nScrollIndicatorWidth = rcScrollIndicator.Width;
            bool bIndicatorVisible = true;

            // Do not show the scroll indicator if it is not needed
            if (listView_UserPref.Items.Count > listView_UserPref.VERTICAL_PAGE_SIZE)
            {
                listView_UserPref.SetBounds(rcListViewUserPreferences.X, rcListViewUserPreferences.Y, rcListViewUserPreferences.Width, rcListViewUserPreferences.Height);
                listView_UserPref.Columns[0].Width = ListViewColumn1Width;
                scrollIndicator1.Visible = true;
            }
            else
                bIndicatorVisible = false;

            if (!bIndicatorVisible)
            {
                listView_UserPref.SetBounds(rcListViewUserPreferences.X, rcListViewUserPreferences.Y, rcListViewUserPreferences.Width + nScrollIndicatorWidth + SpacingBetweenLVAndIndicator, rcListViewUserPreferences.Height);
                int nCount = listView_UserPref.Columns.Count;
                listView_UserPref.Columns[0].Width = ListViewColumn1Width + nScrollIndicatorWidth + SpacingBetweenLVAndIndicator;
                scrollIndicator1.Visible = false;
            }
        }

        public bool LoadUserPreferences(string userName)
        {
            if (string.IsNullOrEmpty(userName) && !bIsNewUser)
                return false;

            string logMSG = "";
            logMSG = ":Load " + userName + "preferences";

            dictMasterUserPreferences.Clear();
            dictMasterDevicePreferences.Clear();

            if (bIsNewUser)
            {
                RoboSep_UserDB.getInstance().getDefaultUserAndDevicePreferences(dictMasterUserPreferences, dictMasterDevicePreferences);
            }
            else
            {
                Dictionary<string, bool> dictUserDefault = new Dictionary<string, bool>();
                Dictionary<string, bool> dictDeviceDefault = new Dictionary<string, bool>();
                RoboSep_UserDB.getInstance().getDefaultUserAndDevicePreferences(dictUserDefault, dictDeviceDefault);
                RoboSep_UserDB.getInstance().getUserAndDevicePreferences(userName, dictMasterUserPreferences, dictMasterDevicePreferences);

                bool bUserDirty = UpdateDefaultPreferences(dictUserDefault, dictMasterUserPreferences);
                bool bDeviceDirty = UpdateDefaultPreferences(dictDeviceDefault, dictMasterDevicePreferences);

                if (bUserDirty || bDeviceDirty)
                {
                    // Update the user preferences file
                    UserDetails.SaveUserPreferences(userName, dictMasterUserPreferences, dictMasterDevicePreferences);
                }
            }

            if (dictMasterUserPreferences.Count > 0)
            {
                //enumerate the keys
                string dispName, helpString;
                dictUserPreferencesFromConfig.Clear();
                IDictionaryEnumerator aEnumerator = dictMasterUserPreferences.GetEnumerator();
                while (aEnumerator.MoveNext())
                {
                    dispName = LanguageINI.GetString((string)aEnumerator.Key);
                    helpString = LanguageINI.GetString((string)aEnumerator.Key + "Help");

                    if (string.IsNullOrEmpty(dispName))
                        dispName = (string)aEnumerator.Key;

                    if (string.IsNullOrEmpty(helpString))
                        helpString = (string)aEnumerator.Key;

                    PreferenceSettings s = new PreferenceSettings((bool)aEnumerator.Value, dispName, helpString);
                    dictUserPreferencesFromConfig.Add((string)aEnumerator.Key, s);
                }
            }

            if (dictMasterDevicePreferences.Count > 0)
            {
                //enumerate the keys
                string dispName, helpString;
                dictDevicePreferencesFromConfig.Clear();
                IDictionaryEnumerator aEnumerator = dictMasterDevicePreferences.GetEnumerator();
                while (aEnumerator.MoveNext())
                {
                    dispName = LanguageINI.GetString((string)aEnumerator.Key);
                    helpString = LanguageINI.GetString((string)aEnumerator.Key + "Help");
                    if (string.IsNullOrEmpty(dispName))
                        dispName = (string)aEnumerator.Key;
                    if (string.IsNullOrEmpty(helpString))
                        helpString = (string)aEnumerator.Key;

                    PreferenceSettings s = new PreferenceSettings((bool)aEnumerator.Value, dispName, helpString);
                    dictDevicePreferencesFromConfig.Add((string)aEnumerator.Key, s);
                }
            }

            // LOG
            logMSG = "Get user preferences settings";
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG); 
            return true;
        }

        private bool UpdateDefaultPreferences(Dictionary<string, bool> dictDefaultPreferences, Dictionary<string, bool> dictPreferences)
        {
            if (dictDefaultPreferences == null || dictPreferences == null || dictDefaultPreferences.Count == 0 || dictPreferences.Count == 0)
                return false;

            bool bDirty = false;
            Dictionary<string, bool> dictTemp = new Dictionary<string, bool>();
            IDictionaryEnumerator aEnumerator = dictPreferences.GetEnumerator();
            while (aEnumerator.MoveNext())
            {
                if (!dictDefaultPreferences.ContainsKey((string)aEnumerator.Key))
                {
                    dictTemp.Add((string)aEnumerator.Key, (bool)aEnumerator.Value);
                }
            }
            // remove obsolete config. variables
            if (dictTemp.Count > 0)
            {
                aEnumerator = dictTemp.GetEnumerator();
                while (aEnumerator.MoveNext())
                {
                    dictPreferences.Remove((string)aEnumerator.Key);
                }
                bDirty = true;
             }

            // add new config.variable
            aEnumerator = dictDefaultPreferences.GetEnumerator();
            while (aEnumerator.MoveNext())
            {
                if (!dictPreferences.ContainsKey((string)aEnumerator.Key))
                {
                    dictPreferences.Add((string)aEnumerator.Key, (bool)aEnumerator.Value);
                    bDirty = true;
                }
            }
            return bDirty;
        }

        private bool IsPreferencesDirty(Dictionary<string, PreferenceSettings> dictSettings, Dictionary<string, bool> dictPreferences)
        {
            if (dictSettings == null || dictPreferences == null)
                return false;

            bool bDirty = false;
            IDictionaryEnumerator aEnumerator = dictSettings.GetEnumerator();
            while (aEnumerator.MoveNext())
            {
                PreferenceSettings s = (PreferenceSettings)aEnumerator.Value;
                string key = (string)aEnumerator.Key;
                bool bValue = s.value;
                bool bMasterValue = dictPreferences[key];

                if (bValue != bMasterValue)
                {
                    bDirty = true;
                    break;
                }
            }
            return bDirty;
        }

        private void ChangeSaveButtonGraphics()
        {
            bool bDirty = IsPreferencesDirty(dictUserPreferencesFromConfig, dictMasterUserPreferences);
            if (!bDirty)
            {
                bDirty = IsPreferencesDirty(dictDevicePreferencesFromConfig, dictMasterDevicePreferences);
            }
             button_Save.ChangeGraphics(bDirty ? ilistSaveDirty : ilistSaveNormal);
        }

        private void horizontalTabs1_Tab1_Click(object sender, EventArgs e)
        {
            if (horizontalTabs1.TabActive == HorizontalTabs.tabs.Tab1)
                return;

            horizontalTabs1.TabActive = HorizontalTabs.tabs.Tab1;
            RefreshPreferencesDisplayList(dictUserPreferencesFromConfig);
            UpdateScrollIndicatorVisibility();
            listView_UserPref.UpdateScrollbar();
        }

        private void horizontalTabs1_Tab2_Click(object sender, EventArgs e)
        {
            if (horizontalTabs1.TabActive == HorizontalTabs.tabs.Tab2)
                return;

            horizontalTabs1.TabActive = HorizontalTabs.tabs.Tab2;
            RefreshPreferencesDisplayList(dictDevicePreferencesFromConfig);
            UpdateScrollIndicatorVisibility();
            listView_UserPref.UpdateScrollbar();
        }

        private void RefreshPreferencesDisplayList(Dictionary<string, PreferenceSettings> dictSettings)
        {
            if (dictSettings == null)
                return;

            // get currently selected item
            string selectedItem = "";
            if (listView_UserPref.SelectedItems.Count > 0)
            {
                selectedItem = listView_UserPref.SelectedItems[0].Tag as string;
            }

            // display list
            refreshList(dictSettings);

            // select last selected item if there is no item selected
            if (string.IsNullOrEmpty(selectedItem))
                selectedItem = lastSelectedItem;
            else
                lastSelectedItem = selectedItem;

            if (!string.IsNullOrEmpty(selectedItem) && listView_UserPref.Items.Count > 0)
            {
                string tempKey = "";
                foreach (ListViewItem lvItem in listView_UserPref.Items)
                {
                    // ensure the previous selected item is visible 
                    if (lvItem.Tag != null)
                    {
                        tempKey = lvItem.Tag as string;
                        if (selectedItem.Equals(tempKey))
                        {
                            // Select the previous selected item
                            lvItem.Selected = true;
                            listView_UserPref.EnsureVisible(lvItem.Index);
                            break;
                        }
                    }
                }
            }
        }

        public void refreshList(Dictionary<string, PreferenceSettings> dictSettings)
        {
            if (dictSettings == null)
                return;

            this.listView_UserPref.ItemChecked -= new System.Windows.Forms.ItemCheckedEventHandler(this.listView_UserPref_HandleItemChecked);
 
            listView_UserPref.SuspendLayout();
            listView_UserPref.Items.Clear();
            if (dictSettings.Count > 0)
            {
                ListViewItem lvItem;
                ListViewItem.ListViewSubItem lvSubItem;
                IDictionaryEnumerator aEnumerator = dictSettings.GetEnumerator();
                while (aEnumerator.MoveNext())
                {
                    PreferenceSettings s = (PreferenceSettings)aEnumerator.Value;
                    string key = (string)aEnumerator.Key;

                    // Sunny to do 
                    lvItem = new ListViewItem();
                    lvSubItem = new ListViewItem.ListViewSubItem();
                    lvItem.Tag = key;
                    lvItem.Text = dictSettings[key].displayName;
                    lvItem.Checked = dictSettings[key].value;
                    lvSubItem.Tag = dictSettings[key].helpString;
                    lvItem.SubItems.Add(lvSubItem);
                    listView_UserPref.Items.Add(lvItem);
                }
            
                aEnumerator = dictSettings.GetEnumerator();
                while (aEnumerator.MoveNext())
                {
                    PreferenceSettings s = (PreferenceSettings)aEnumerator.Value;
                    string key = (string)aEnumerator.Key;

                    // Add help button
                    GUI_Controls.GUIButton b = new GUI_Controls.GUIButton();
                    b.Size = new Size(BUTTON_SIZE, BUTTON_SIZE);
  
                    List<Image> ilist = new List<Image>();
                    ilist.Clear();
                    ilist.Add(Properties.Resources.PF_BTN01S_details_STD);
                    ilist.Add(Properties.Resources.PF_BTN01S_details_OVER);
                    ilist.Add(Properties.Resources.PF_BTN01S_details_OVER);
                    ilist.Add(Properties.Resources.PF_BTN01S_details_CLICK);
                    b.ChangeGraphics(ilist);

                    b.Tag = dictSettings[key].helpString;
                    b.Click += new EventHandler(b_Click);

                    for (int i = 0; i < listView_UserPref.Items.Count; i++)
                    {
                        ListViewItem lvItemTemp = listView_UserPref.Items[i];
                        string temp = lvItemTemp.Tag as string;
                        if (temp == key)
                        {
                            // Put it in the second column of every row
                            listView_UserPref.AddEmbeddedControl(b, 1, lvItemTemp.Index);
                            break;
                        }
                     }
                }
            }

            this.listView_UserPref.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listView_UserPref_HandleItemChecked);
            listView_UserPref.ResumeLayout(true);
        }

        private void b_Click(object sender, EventArgs e)
        {
            GUI_Controls.GUIButton b = sender as GUI_Controls.GUIButton;
            if (b == null)
                return;

            string msg = b.Tag as string;
            if (string.IsNullOrEmpty(msg))
                return;

            GUI_Controls.RoboMessagePanel prompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_INFORMATION, msg, String.Empty);
            RoboSep_UserConsole.showOverlay();
            prompt.ShowDialog();
            prompt.Dispose();
            RoboSep_UserConsole.hideOverlay();
            return;
        }

        private void listView_UserPref_HandleItemChecked(Object sender, ItemCheckedEventArgs e)
        {
            updatePreferences((ListViewItem)e.Item);
            handleItemAction((ListViewItem)e.Item);
        }

        private void listView_UserPref_Click(object sender, EventArgs e)
        {
            if (listView_UserPref.SelectedItems.Count == 0)
                return;

            ListViewItem lvItem = listView_UserPref.SelectedItems[0];
            if (lvItem == null || lvItem.Tag == null)
                return;

            lvItem.Checked = !lvItem.Checked;
        }

        private void updatePreferences(ListViewItem lvItem)
        {
            if (lvItem == null || lvItem.Tag == null)
                return;

            lock (syncobj)
            {
                string key = lvItem.Tag as string;
                if (horizontalTabs1.TabActive == tabUserSettings)
                {
                    if (dictUserPreferencesFromConfig.ContainsKey(key))
                    {
                        PreferenceSettings p = dictUserPreferencesFromConfig[key];
                        p.value = lvItem.Checked;
                        dictUserPreferencesFromConfig[key] = p;
                    }
                }
                else if (horizontalTabs1.TabActive == tabDeviceSettings)
                {
                    if (dictDevicePreferencesFromConfig.ContainsKey(key))
                    {
                        PreferenceSettings p = dictDevicePreferencesFromConfig[key];
                        p.value = lvItem.Checked;
                        dictDevicePreferencesFromConfig[key] = p;
                    }
                }
                else
                {
                }
            }
            ChangeSaveButtonGraphics();
        }

        private void handleItemAction(ListViewItem lvItem)
        {
            if (lvItem == null || lvItem.Tag == null)
                return;

            string key = lvItem.Tag as string;
            if (string.IsNullOrEmpty(key))
                return;

            ItemAction = null;
            if (horizontalTabs1.TabActive == tabDeviceSettings)
            {
                if (RoboSep_UserDB.getInstance().KeyDisableCamera == key)
                {
                    ItemAction = Utilities.DisableCamera;

                }
                else if (RoboSep_UserDB.getInstance().KeyDeleteVideoLogFiles == key)
                {
                    ItemAction = Utilities.DeleteVideoLogFiles;
                }

                try
                {
                    if (ItemAction != null)
                        ItemAction(userName, lvItem.Checked);
                }
                catch (Exception ex)
                {
                    // log exception message
                    //  (ex.Message);
                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, ex.Message); 
                }
            }
            else if (horizontalTabs1.TabActive == tabUserSettings && lvItem.Checked)
            {
                string sItem = String.Empty;
                if (RoboSep_UserDB.getInstance().KeyEnableAutoScanBarcodes == key)
                {
                    ItemAction = UncheckItem;
                    sItem = RoboSep_UserDB.getInstance().KeySkipResourcesScreen;
                }
                else if (RoboSep_UserDB.getInstance().KeySkipResourcesScreen == key)
                {
                    ItemAction = UncheckItem;
                    sItem = RoboSep_UserDB.getInstance().KeyEnableAutoScanBarcodes;
                }
                try
                {
                    if (ItemAction != null)
                        ItemAction(sItem, true);
                }
                catch (Exception ex)
                {
                    // log exception message
                    //  (ex.Message);
                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, ex.Message); 
                }
            }
        }

        private void listView_UserPref_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            System.Drawing.Drawing2D.LinearGradientBrush GradientBrush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.Blue, Color.LightBlue, 270);
            
            SolidBrush theBrush1 = new SolidBrush(Color.White);
            SolidBrush theBrush2 = new SolidBrush(Color.Yellow);
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

        private void listView_UserPref_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                Color textColour = Txt_Color;
                SolidBrush theBrush, theBrush2;
                Rectangle itemBounds = new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width, e.Bounds.Bottom - e.Bounds.Top - 1);

                if (e.Item.Selected)
                {
                    theBrush = new SolidBrush(BG_Selected);
                    e.Graphics.FillRectangle(theBrush, itemBounds);
                    textColour = Color.White;
                }
                else if (e.ItemIndex % 2 == 0)
                {
                    theBrush = new SolidBrush(BG_ColorEven);
                    e.Graphics.FillRectangle(theBrush, itemBounds);

                }
                else
                {
                    theBrush = new SolidBrush(BG_ColorOdd);
                    e.Graphics.FillRectangle(theBrush, itemBounds);
                }

                // draw checkboxes in this column if required
                if (e.Item.ListView.CheckBoxes)
                    itemBounds = DrawCheckBox(e.Graphics, itemBounds, e.Item.Checked);

                theBrush2 = new SolidBrush(textColour);
                e.Graphics.DrawString(e.Item.Text, listView_UserPref.Font, theBrush2,
                         new Rectangle(itemBounds.Left + EndPaddingSize, itemBounds.Top, itemBounds.Width, itemBounds.Bottom - itemBounds.Top), theFormat);
                theBrush.Dispose();
                theBrush2.Dispose();

            }
            else if (e.ColumnIndex == 1)
            {
                int th, ty, tw, tx;
                th = CHECKBOX_SIZE + (CellPaddingSize * 2);
                tw = CHECKBOX_SIZE + (CellPaddingSize * 2);
                if ((tw > e.Bounds.Width) || (th > e.Bounds.Height))
                    return;					// not enough room to draw the image, bail out

                ty = e.Bounds.Y + ((e.Bounds.Height - th) / 2);
                tx = e.Bounds.X + CellPaddingSize / 2;

                Rectangle itemBounds = new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width, e.Bounds.Bottom - e.Bounds.Top);
                e.Graphics.DrawImage(this.imageList1.Images[2], tx, ty);
            }
        }

        public void UncheckItem(string sKey, bool bUnchecked)
        {
            if (string.IsNullOrEmpty(sKey) || !bUnchecked)
                return;

            if (listView_UserPref.Items.Count > 0)
            {
                string tempKey = "";
                foreach (ListViewItem lvItem in listView_UserPref.Items)
                {
                    // Find the item 
                    if (lvItem.Tag != null)
                    {
                        tempKey = lvItem.Tag as string;
                        if (sKey.Equals(tempKey))
                        {
                            // Uncheck the previously checked item
                            if (lvItem.Checked)
                                lvItem.Checked = false;
                            break;
                        }
                    }
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
            tx = rectCell.X + CellPaddingSize/2;

            if (bChecked)
                graphics.DrawImage(this.imageList1.Images[1], tx, ty);
              else
                graphics.DrawImage(this.imageList1.Images[0], tx, ty);

            // remove the width that we used for the graphic from the cell
            rectCell.Width -= (CHECKBOX_SIZE + (CellPaddingSize * 2));
            rectCell.X += tw;
            return rectCell;
        }

        public Dictionary<string, bool> UserPreferences
        {
            get
            {
                return dictUserPreferences;
            }
        }

        public Dictionary<string, bool> DevicePreferences
        {
            get
            {
                return dictDevicePreferences;
            }
        }

        private void getPreferences(Dictionary<string, PreferenceSettings> dictSettings, Dictionary<string, bool> dictPreferences)
        {
            if (dictSettings == null || dictPreferences == null)
                return;

            dictPreferences.Clear();

            string key;
            bool bValue;
            IDictionaryEnumerator aEnumerator = dictSettings.GetEnumerator();
            while (aEnumerator.MoveNext())
            {
                PreferenceSettings s = (PreferenceSettings)aEnumerator.Value;
                key = (string)aEnumerator.Key;
                bValue = s.value;
                dictPreferences.Add(key, bValue);
            }
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            string logMSG = "Saving user preferences for '" +userName + "'";
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG); 
            getPreferences(dictUserPreferencesFromConfig, dictUserPreferences);
            getPreferences(dictDevicePreferencesFromConfig, dictDevicePreferences);

            if (bIsNewUser == true)
            {
                if (GetUserPreferences != null)
                {
                    GetUserPreferences(this, new EventArgs());
                }
            }
            else
            {
                string title = LanguageINI.GetString("headerSavingInProgress");
                RoboMessagePanel4 prompt = new GUI_Controls.RoboMessagePanel4(RoboSep_UserConsole.getInstance(), title, 500, 20);
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();

                RoboSep_UserConsole.hideOverlay();

                // update the user preferences file for old user
                UserDetails.SaveUserPreferences(userName, dictUserPreferences, dictDevicePreferences);

                // reload the user preferences to refresh the changes
                RoboSep_UserDB.getInstance().loadCurrentUserPreferences(userName);

                prompt.Dispose();
            }

            this.SendToBack();

            if (ClosingUserPreferencesApp != null)
            {
                ClosingUserPreferencesApp(this, new EventArgs());
            }

            this.Hide();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.SendToBack();

            if (ClosingUserPreferencesApp != null)
            {
                ClosingUserPreferencesApp(this, new EventArgs());
            }
             this.Hide();
        }

        private void button_ScrollUp_Click(object sender, EventArgs e)
        {
            this.listView_UserPref.LineUp();
        }

        private void button_ScrollDown_Click(object sender, EventArgs e)
        {
            this.listView_UserPref.LineDown();
        }
    }

    public struct PreferenceSettings
    {
        public bool value;
        public string displayName;
        public string helpString;

        public void Clear()
        {
            value = false;
            displayName = "";
            helpString = "";
        }
        public PreferenceSettings(bool value, string displayName, string helpString)
        {
            this.value = value;
            this.displayName = displayName;
            this.helpString = helpString;
        }
    }
}
