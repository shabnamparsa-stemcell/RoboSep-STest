using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.Runtime.InteropServices;
using Tesla.Common.ResourceManagement;
using Tesla.OperatorConsoleControls;
using Tesla.Common.Protocol;
using Tesla.Common;

using Invetech.ApplicationLog;

using GUI_Controls;

namespace GUI_Console
{
    public partial class RoboSep_ProtocolList : BasePannel
    {
        private const int ITEMS_ON_SCREEN = 5;
        private const int IMAGELIST_WIDTH = 40;
        private const int IMAGELIST_HEIGHT = 40;
        private const int IMAGEBOX_SIZE = 30;
        private const int CellPaddingSize = 4;
        private const int EndPaddingSize = 6;

        // variable for UserProtocolList
        private RoboSep_UserProtocolList myUserList;
        private RoboSep_UserDB myDB;
        private bool initialized = false;

        private List<RoboSep_Protocol> listMasterAllProtocols;
        private List<RoboSep_Protocol> listWorkingAllProtocols;
        private List<RoboSep_Protocol> listAllProtocolsFiltered;

        private List<RoboSep_Protocol> myMasterProtocolsList;
        private List<RoboSep_Protocol> myWorkingProtocolsList;
        private List<RoboSep_Protocol> myFilteredList;

        private BackgroundWorker myAsyncWorker = new BackgroundWorker();
        private DoWorkEventHandler evhReloadProtocols;
        private RunWorkerCompletedEventHandler evhReloadProtocolsCompleted;

        private static object padLock = new object();
        private RoboMessagePanel5 loading = null;
        private RoboMessagePanel copyingMsg = null;

        private List<string> listChosenProtocolsFromAllProtocols = null;
        private List<string> listChosenProtocolsFromMyProtocols = null;

        public string strUserName;
        // search filters
        private string search;
        private bool H = false;
        private bool M = false;
        private bool P = false;
        private bool N = false;
        private bool WB = false;

        // listview drawing vars
        private StringFormat textFormat;
        private Color BG_ColorEven;
        private Color BG_ColorOdd;
        private Color BG_Selected;
        private Color Txt_Color;

        // Variable Declaration
        private static RoboSep_ProtocolList myProtocolList;

        private static string listAllProtocolsPath = Application.StartupPath + "\\..\\protocols\\";
        private static string myXSDPath = Application.StartupPath + "\\..\\config\\RoboSepUser.xsd";
 

        private const string ProtocolFileExtension = ".xml";
        private const string TextMyProtocols = "My Protocols";
        private const string TextAllProtocols = "All Protocols";

        private RoboSep_Protocol[] selectedProtocols;
        private IniFile LanguageINI = GUI_Console.RoboSep_UserConsole.getInstance().LanguageINI;

        private int thumbSize = 15;

        private Rectangle rcScrollIndicator;
        private Rectangle rcListViewProtocols;

        private Rectangle rcButtonRemoveAllList;
        private Rectangle rcButtonRemoveMyList;

        private int ListViewLastColumnWidth;
        private int SpacingBetweenLVAndIndicator = 0;
        private bool DisplayProtocolsAbbv = true;
        private bool UseFirstTabToDisplayAllProtocolsList = false;
        private bool SwitchToMyProtocolListAfterAddingAProtocol = false;
        private bool bMyProtocolsListDirty = false;
        private bool bAllProtocolsListDirty = false;
        private bool CurrentUserIsPresetUser = false;

        private bool isProtocolLoading = false;

        private string lastSelectedProtocolName = "";
        private string tempFolderForFileRestoration;

        private System.EventHandler evhHandleSearchBoxTextChanged = null;
        private HorizontalTabs.tabs tabMyProtocols = HorizontalTabs.tabs.Tab1;
        private HorizontalTabs.tabs tabAllProtocols = HorizontalTabs.tabs.Tab2;
        private ISeparationProtocol[] UserProtocolsListFromServer = null;
        private System.EventHandler evhHandleUserProtocolsReloaded = null;

        private FileBrowser myFB = null;

        private List<Image> ilistUserAdd;
        private List<Image> ilistUserRemove;
        private List<Image> ilist1, ilist2;

        private const int WM_HSCROLL = 0x114;
        private const int WM_VSCROLL = 0x115;
        private const uint SB_VERT = 1;
        private const uint SB_HORZ = 0;

        private const uint ESB_DISABLE_BOTH = 0x3;
        private const uint ESB_ENABLE_BOTH = 0x0;

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, UIntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static public extern bool EnableScrollBar(System.IntPtr hWnd, uint wSBflags, uint wArrows);


        private RoboSep_ProtocolList()
        {
            InitializeComponent();

            SuspendLayout();

            this.DoubleBuffered = true;
            this.UpdateStyles();

            ilistUserRemove = new List<Image>();
            ilistUserRemove.Add(Properties.Resources.PR_BTN09L_remove_protocol_STD);
            ilistUserRemove.Add(Properties.Resources.PR_BTN09L_remove_protocol_OVER);
            ilistUserRemove.Add(Properties.Resources.PR_BTN09L_remove_protocol_OVER);
            ilistUserRemove.Add(Properties.Resources.PR_BTN09L_remove_protocol_CLICK);

            ilistUserAdd = new List<Image>();
            ilistUserAdd.Add(Properties.Resources.PR_BTN08L_add_protocol_STD);
            ilistUserAdd.Add(Properties.Resources.PR_BTN08L_add_protocol_OVER);
            ilistUserAdd.Add(Properties.Resources.PR_BTN08L_add_protocol_OVER);
            ilistUserAdd.Add(Properties.Resources.PR_BTN08L_add_protocol_CLICK);
            btn_AddProtocols.ChangeGraphics(ilistUserAdd);
 
            ilist1 = new List<Image>();
            ilist1.Add(Properties.Resources.PR_BTN09L_remove_protocol_STD);
            ilist1.Add(Properties.Resources.PR_BTN09L_remove_protocol_OVER);
            ilist1.Add(Properties.Resources.PR_BTN09L_remove_protocol_OVER);
            ilist1.Add(Properties.Resources.PR_BTN09L_remove_protocol_CLICK);
            btn_RemoveProtocols.ChangeGraphics(ilist1);

            ilist2 = new List<Image>();
            ilist2.Add(Properties.Resources.US_BTN04L_usb_add_STD);
            ilist2.Add(Properties.Resources.US_BTN04L_usb_add_OVER);
            ilist2.Add(Properties.Resources.US_BTN04L_usb_add_OVER);
            ilist2.Add(Properties.Resources.US_BTN04L_usb_add_CLICK);
            btn_USBload.ChangeGraphics(ilist2);
          
            SetUpSearchBarEvents();

            SetUpScrollIndicatorThumbs();

             IniFile LanguageINI = GUI_Console.RoboSep_UserConsole.getInstance().LanguageINI;

            // set text values on buttons and labels to specified language
            protocol_SearchBar2.SearchBoxText = LanguageINI.GetString("lblSearch");

            listMasterAllProtocols = new List<RoboSep_Protocol>();
            listWorkingAllProtocols = new List<RoboSep_Protocol>();
            listAllProtocolsFiltered = new List<RoboSep_Protocol>();
            myMasterProtocolsList = new List<RoboSep_Protocol>();
            myWorkingProtocolsList = new List<RoboSep_Protocol>();
            myFilteredList = new List<RoboSep_Protocol>();
            listChosenProtocolsFromAllProtocols = new List<string>();
            listChosenProtocolsFromMyProtocols = new List<string>();

            rcButtonRemoveAllList = new Rectangle(btn_RemoveProtocols.Location, btn_RemoveProtocols.Size);
            rcButtonRemoveMyList = new Rectangle(btn_AddProtocols.Location, btn_AddProtocols.Size);
            ResumeLayout();

            // set up for drawing
            textFormat = new StringFormat();
            textFormat.Alignment = StringAlignment.Near;
            textFormat.LineAlignment = StringAlignment.Center;
            textFormat.FormatFlags = StringFormatFlags.NoWrap;
            BG_ColorEven = Color.FromArgb(216, 217, 218);
            BG_ColorOdd = Color.FromArgb(243, 243, 243);
            BG_Selected = Color.FromArgb(78, 38, 131);
            Txt_Color = Color.FromArgb(95, 96, 98);

            // LOG
            string logMSG = "Initializing Protocol List user control";

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                
        }

        private void RoboSep_ProtocolList_Load(object sender, EventArgs e)
        {
            // grab userProtocolListform
            myUserList = RoboSep_UserProtocolList.getInstance();

            UserNameHeader.Text = RoboSep_UserConsole.strCurrentUser;

            // load all users and protocols
            myDB = RoboSep_UserDB.getInstance();

            // get protocols
            listMasterAllProtocols.Clear();
            listMasterAllProtocols.AddRange(myDB.getAllProtocols());
            listWorkingAllProtocols.AddRange(listMasterAllProtocols);

            LoadMyProtocolsList();
            CreateImageListGraphics();

            lvProtocolList.NUM_VISIBLE_ELEMENTS = 5;
            lvProtocolList.VERTICAL_PAGE_SIZE = 4;

            DisplayProtocolsAbbv = RoboSep_UserDB.getInstance().UseAbbreviationsForProtocolNames;
            UseFirstTabToDisplayAllProtocolsList = RoboSep_UserDB.getInstance().UseFirstTabToDisplayAllProtocolsList;
            SwitchToMyProtocolListAfterAddingAProtocol = RoboSep_UserDB.getInstance().SwitchToMyProtocolListAfterAddingAProtocol;

            selectedProtocols = null;

            ShowTabsOrder();

            // is user user preset user?
            CurrentUserIsPresetUser = RoboSep_UserDB.getInstance().IsPresetUser(RoboSep_UserConsole.strCurrentUser);
            if (CurrentUserIsPresetUser)
            {
                DisableTabMyProtocols();
            }

            // update the listview
            if (tabAllProtocols == HorizontalTabs.tabs.Tab1)
            {
                UpdateProtocolOperatorButtons(tabAllProtocols);
                for (int pNum = 0; pNum < listWorkingAllProtocols.Count; pNum++)
                {
                    ListViewItem lvItem = new ListViewItem();
                    lvItem.Tag = listWorkingAllProtocols[pNum];
                    lvItem.Text = DisplayProtocolsAbbv ? listWorkingAllProtocols[pNum].Protocol_Name_Abbv : listWorkingAllProtocols[pNum].Protocol_Name;
                    lvProtocolList.Items.Add(lvItem);
                }
            }
            else if (tabMyProtocols == HorizontalTabs.tabs.Tab1)
            {
                UpdateProtocolOperatorButtons(tabMyProtocols);
                for (int pNum = 0; pNum < myWorkingProtocolsList.Count; pNum++)
                {
                    ListViewItem lvItem = new ListViewItem();
                    lvItem.Tag = myWorkingProtocolsList[pNum];
                    lvItem.Text = DisplayProtocolsAbbv ? myWorkingProtocolsList[pNum].Protocol_Name_Abbv : myWorkingProtocolsList[pNum].Protocol_Name;
                    lvProtocolList.Items.Add(lvItem);
                }
            }
            else
            {
                // do nothing
            }

            UpdateDisplayList();

            // Resize last column, make allowances for up to 4 quadrants
            lvProtocolList.ResizeColumnWidth(columnHeader1, textFormat, (lvProtocolList.Width + 4 * IMAGELIST_WIDTH));

            this.scrollIndicator1.LargeChange = lvProtocolList.VisibleRow - 1;
            lvProtocolList.VScrollbar = this.scrollIndicator1;

            // listView_UserProtocols.ResizeVerticalHeight();
            Rectangle rcScrollbar = this.scrollIndicator1.Bounds;
            this.scrollIndicator1.SetBounds(rcScrollbar.X, lvProtocolList.Bounds.Y, rcScrollbar.Width, lvProtocolList.Bounds.Height);

            lvProtocolList.UpdateScrollbar();

            rcScrollIndicator = scrollIndicator1.Bounds;
            rcListViewProtocols = lvProtocolList.Bounds;
            SpacingBetweenLVAndIndicator = rcScrollIndicator.X - (rcListViewProtocols.X + rcListViewProtocols.Width);

            int nCount = lvProtocolList.Columns.Count;
            ListViewLastColumnWidth = lvProtocolList.Columns[nCount - 1].Width;

            UpdateScrollIndicatorVisibility();

            myAsyncWorker.WorkerSupportsCancellation = true;
            evhReloadProtocols = new DoWorkEventHandler(bwReloadProtocols_DoWork);
            evhReloadProtocolsCompleted = new RunWorkerCompletedEventHandler(bwReloadProtocols_RunWorkerCompleted);

            initialized = true;
        }

        public bool IsInitialized
        {
            get { return initialized; }
        }

        public void ReInitialized()
        {
            DisplayProtocolsAbbv = RoboSep_UserDB.getInstance().UseAbbreviationsForProtocolNames;
            UseFirstTabToDisplayAllProtocolsList = RoboSep_UserDB.getInstance().UseFirstTabToDisplayAllProtocolsList;
            SwitchToMyProtocolListAfterAddingAProtocol = RoboSep_UserDB.getInstance().SwitchToMyProtocolListAfterAddingAProtocol;

            ShowTabsOrder();

            // is user user preset user?
            CurrentUserIsPresetUser = RoboSep_UserDB.getInstance().IsPresetUser(RoboSep_UserConsole.strCurrentUser);
            if (CurrentUserIsPresetUser)
            {
                DisableTabMyProtocols();
            }

            UserNameHeader.Text = RoboSep_UserConsole.strCurrentUser;

            ReloadAllProtocolsList();

            LoadMyProtocolsList();

            UpdateDisplayList();
        }

        private void StartReloadingAllProtocols()
        {
            // Attach the event handlers
            myAsyncWorker.DoWork += evhReloadProtocols;
            myAsyncWorker.RunWorkerCompleted += evhReloadProtocolsCompleted;

            // show loading protocol message
            string title = LanguageINI.GetString("headerLoadingProtocols");
            string msg = LanguageINI.GetString("LoadingAllProtocols");
            loading = new RoboMessagePanel5(RoboSep_UserConsole.getInstance(), title, msg, GUI_Controls.GifAnimationMode.eUploadingMultipleFiles);
            RoboSep_UserConsole.showOverlay();

            loading.Show();

            // Kick off the Async thread
            myAsyncWorker.RunWorkerAsync();
        }

        private void bwReloadProtocols_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            if (!bw.CancellationPending)
            {
                // Reload all protocols
                RoboSep_UserDB.getInstance().ReloadAllProtocols();

                // Reload all protocol lists
                ReloadAllProtocolsList();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ReloadAllProtocols: User aborts reloading protocols");
                Thread.Sleep(200);
                e.Cancel = true;
            }

            myAsyncWorker.DoWork -= evhReloadProtocols;
        }

        private void bwReloadProtocols_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            myAsyncWorker.RunWorkerCompleted -= evhReloadProtocolsCompleted;

            this.Invoke(
                (MethodInvoker)delegate()
                {
                    bAllProtocolsListDirty = false;

                    ReloadUserProtocols();
                 }
             );
        }

        private void ReloadAllProtocolsList()
        {
            listMasterAllProtocols.Clear();
            listWorkingAllProtocols.Clear();

            lock (padLock)
            {
                // get protocols
                List<RoboSep_Protocol> lst = myDB.getAllProtocols();
                if (lst != null)
                {
                    listMasterAllProtocols.AddRange(lst);
                    listWorkingAllProtocols.AddRange(listMasterAllProtocols);
                }
            }
        }

        private void ReloadUserProtocols()
        {
            System.Diagnostics.Debug.WriteLine(String.Format("--------- Reload user protocols from the server. Current ThreadID = {0} ----------", Thread.CurrentThread.ManagedThreadId));

            LoadProtocolsToServer();
            SeparatorGateway.GetInstance().separatorUpdating = true;

            if (evhHandleUserProtocolsReloaded == null)
                evhHandleUserProtocolsReloaded = new EventHandler(HandleUserProtocolsReloaded);

            RoboSep_UserConsole.getInstance().NotifyUserSeparationProtocolsUpdated += evhHandleUserProtocolsReloaded;
        }

        private void CloseWaitDialog()
        {
            if (loading == null)
                return;

            loading.Close();
            loading = null;
            RoboSep_UserConsole.hideOverlay();
        }

        private void ShowScrollIndicator(bool visible)
        {
            int nScrollIndicatorWidth = rcScrollIndicator.Width;

            if (visible)
            {
                // Do not show the scroll indicator if it is not needed
                if (lvProtocolList.Items.Count > lvProtocolList.VisibleRow)
                {
                    lvProtocolList.SetBounds(rcListViewProtocols.X, rcListViewProtocols.Y, rcListViewProtocols.Width, rcListViewProtocols.Height);
                    int nCount = lvProtocolList.Columns.Count;
                    lvProtocolList.Columns[nCount - 1].Width = ListViewLastColumnWidth;
                    scrollIndicator1.Visible = true;
                }
            }
            else
            {
                lvProtocolList.SetBounds(rcListViewProtocols.X, rcListViewProtocols.Y, rcListViewProtocols.Width + nScrollIndicatorWidth + SpacingBetweenLVAndIndicator, rcListViewProtocols.Height);
                int nCount = lvProtocolList.Columns.Count;
                lvProtocolList.Columns[nCount - 1].Width = ListViewLastColumnWidth + nScrollIndicatorWidth + SpacingBetweenLVAndIndicator;
                scrollIndicator1.Visible = false;
            }
        }

        private void UpdateScrollIndicatorVisibility()
        {
            int nScrollIndicatorWidth = rcScrollIndicator.Width;
            bool bIndicatorVisible = true;

            // Do not show the scroll indicator if it is not needed
            if (lvProtocolList.Items.Count > lvProtocolList.VERTICAL_PAGE_SIZE)
            {
                lvProtocolList.SetBounds(rcListViewProtocols.X, rcListViewProtocols.Y, rcListViewProtocols.Width, rcListViewProtocols.Height);
                int nCount = lvProtocolList.Columns.Count;
                lvProtocolList.Columns[nCount - 1].Width = ListViewLastColumnWidth;
                scrollIndicator1.Visible = true;
            }
            else
                bIndicatorVisible = false;

            if (!bIndicatorVisible)
            {
                lvProtocolList.SetBounds(rcListViewProtocols.X, rcListViewProtocols.Y, rcListViewProtocols.Width + nScrollIndicatorWidth + SpacingBetweenLVAndIndicator, rcListViewProtocols.Height);
                int nCount = lvProtocolList.Columns.Count;
                lvProtocolList.Columns[nCount - 1].Width = ListViewLastColumnWidth + nScrollIndicatorWidth + SpacingBetweenLVAndIndicator;
                scrollIndicator1.Visible = false;
            }
        }

        public static RoboSep_ProtocolList getInstance()
        {
            if (myProtocolList == null)
                myProtocolList = new RoboSep_ProtocolList();
            GC.Collect();
            return myProtocolList;
        }
        
        private void SetUpSearchBarEvents()
        {
            evhHandleSearchBoxTextChanged = new System.EventHandler(this.textBox_search_TextChanged);

            this.protocol_SearchBar2.textBox_search.TextChanged += evhHandleSearchBoxTextChanged;
            this.protocol_SearchBar2.textBox_search.MouseClick += new System.Windows.Forms.MouseEventHandler(this.mouseActivateTextbox);
            this.protocol_SearchBar2.btn_ScrollRight.Click += new System.EventHandler(btn_ScrollRight_Click);
            this.protocol_SearchBar2.btn_ScrollLeft.Click += new System.EventHandler(btn_ScrollLeft_Click);
            this.protocol_SearchBar2.btn_ScrollDown.Click += new System.EventHandler(btn_ScrollDown_Click);
            this.protocol_SearchBar2.btn_ScrollDown.MouseUp += new MouseEventHandler(btn_ScrollDown_MouseUp);
            this.protocol_SearchBar2.btn_ScrollUp.Click += new System.EventHandler(btn_ScrollUp_Click);
            this.protocol_SearchBar2.btn_ScrollUp.MouseUp += new MouseEventHandler(btn_ScrollUp_MouseUp);
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

        private void CreateImageListGraphics()
        {
            Bitmap bmUserProtocolChosen = new Bitmap(Properties.Resources.GE_BTN09S_added);
            Bitmap bmCheck = new Bitmap(Properties.Resources.GE_BTN07S_select_CLICK);
            Bitmap bmNewlyAdded = new Bitmap(Properties.Resources.GE_BTN25S_newly_added);
            Bitmap bmOverwritten = new Bitmap(Properties.Resources.GE_BTN25S_overwritten);
            Bitmap bmMarkedAsAdded = new Bitmap(Properties.Resources.GE_BTN25S_marked_as_added);
            Bitmap bmMarkedAsDeleted = new Bitmap(Properties.Resources.GE_BTN25S_marked_as_deleted);

            // add to the image list
            imageList1.ImageSize = new System.Drawing.Size(IMAGELIST_WIDTH, IMAGELIST_HEIGHT);
            imageList1.Images.Clear();
            this.imageList1.Images.Add(bmUserProtocolChosen);
            this.imageList1.Images.Add(bmCheck);
            this.imageList1.Images.Add(bmNewlyAdded);
            this.imageList1.Images.Add(bmOverwritten);
            this.imageList1.Images.Add(bmMarkedAsAdded);
            this.imageList1.Images.Add(bmMarkedAsDeleted);
            var dummy = imageList1.Handle;          //This is dumb, but it has to be here.
            bmUserProtocolChosen.Dispose();
            bmCheck.Dispose();
            bmNewlyAdded.Dispose();
            bmOverwritten.Dispose();
            bmMarkedAsAdded.Dispose();
            bmMarkedAsDeleted.Dispose();
        }

        private void ShowTabsOrder()
        {
            tabAllProtocols = UseFirstTabToDisplayAllProtocolsList ? HorizontalTabs.tabs.Tab1 : HorizontalTabs.tabs.Tab2; ;
            tabMyProtocols = tabAllProtocols == HorizontalTabs.tabs.Tab1 ? HorizontalTabs.tabs.Tab2 : HorizontalTabs.tabs.Tab1;

            horizontalTabs1.Tab1 = tabAllProtocols == HorizontalTabs.tabs.Tab1 ? TextAllProtocols : TextMyProtocols;
            horizontalTabs1.Tab2 = horizontalTabs1.Tab1 == TextAllProtocols ? TextMyProtocols : TextAllProtocols;
        }

        private void LoadMyProtocolsList()
        {
            string strUserName = RoboSep_UserConsole.strCurrentUser;
            myMasterProtocolsList.Clear();
            myWorkingProtocolsList.Clear();
            bMyProtocolsListDirty = false;

            Tesla.DataAccess.RoboSepUser currentUser = RoboSep_UserDB.getInstance().XML_LoadRoboSepUserObject();
            if (strUserName == currentUser.UserName)
            {
                // determine validity of user ID
                string[] usrProtocols = currentUser.ProtocolFile;

                UserProtocolsListFromServer = RoboSep_UserConsole.getInstance().XML_getUserProtocols();
                usrProtocols = new string[UserProtocolsListFromServer.Length];
                for (int i = 0; i < UserProtocolsListFromServer.Length; i++)
                {
                    usrProtocols[i] = UserProtocolsListFromServer[i].Label;
                }

                // update my protocol list
                int nIndex = 0;
                for (int i = 0; i < usrProtocols.Length; i++)
                {
                    nIndex = findInList(usrProtocols[i]);
                    if (nIndex < 0)
                    {
                        continue;
                    }
                    myMasterProtocolsList.Add(listWorkingAllProtocols[nIndex]);
                    myWorkingProtocolsList.Add(listWorkingAllProtocols[nIndex]);
                }
            }
        }

        private bool IsProtocolRemovable(string sProtocolFileName)
        {
            bool bPresetProtocol = RoboSep_UserDB.getInstance().IsPresetProtocol(sProtocolFileName);
            return !bPresetProtocol;
        }

        private void RefreshProtocolDisplayList(List<RoboSep_Protocol> protocolList, List<RoboSep_Protocol> protocolListFiltered)
        {
            if (protocolList == null || protocolListFiltered == null)
                return;

            // get currently selected item
            string selectedProtocolName = "";
            if (lvProtocolList.SelectedItems.Count > 0)
            {
                RoboSep_Protocol tempProtocol = (RoboSep_Protocol)lvProtocolList.SelectedItems[0].Tag;
                if (tempProtocol != null)
                {
                    selectedProtocolName = tempProtocol.Protocol_Name;
                }
            }

            // protocols list
            updateFilteredList(protocolList, protocolListFiltered);

            // display filtered list
            refreshList(protocolListFiltered);

            // select last selected item if there is no item selected
            if (string.IsNullOrEmpty(selectedProtocolName))
                selectedProtocolName = lastSelectedProtocolName;
            else
                lastSelectedProtocolName = selectedProtocolName;

            if (!string.IsNullOrEmpty(selectedProtocolName) && lvProtocolList.Items.Count > 0)
            {
                RoboSep_Protocol tempProtocol = null;

                foreach (ListViewItem lvItem in lvProtocolList.Items)
                {
                    // ensure the previous selected item is visible 
                    if (lvItem.Tag != null)
                    {
                        tempProtocol = (RoboSep_Protocol)lvItem.Tag;
                        if (tempProtocol.Protocol_Name.Equals(selectedProtocolName))
                        {
                            // Select the previous selected protocol
                            lvItem.Selected = true;
                            lvProtocolList.EnsureVisible(lvItem.Index);
                            break;
                        }
                    }
                }
            }
        }

        public void getFilters()
        {
            protocol_SearchBar2.getFilters(out search);
        }

        public void clearFilters()
        {
            protocol_SearchBar2.clearFilters();
        }

        private int findInList(string pString)
        {
            if (string.IsNullOrEmpty(pString))
                return -1;

            int nLastIndex = pString.LastIndexOf('.');
            if (nLastIndex!=-1 && ((pString.Length - nLastIndex) == ProtocolFileExtension.Length) && pString.Substring(nLastIndex).ToLower() == ProtocolFileExtension.ToLower())
            {
                // check first if string is filename
                for (int i = 0; i < listWorkingAllProtocols.Count; i++)
                {
                    if (pString == listWorkingAllProtocols[i].Protocol_FileName)
                        return i;
                }
                return -1;
            }
            else
            {
                getFilters();
                if ((H || M) && (P || N))
                {
                    // compare to string of SpeciesSelection in robo_protocol
                    for (int i = 0; i < listWorkingAllProtocols.Count; i++)
                    {
                        if (pString == listWorkingAllProtocols[i].SpeciesSelection_Name)
                            return i;
                    }
                }
                else if ((H || M) && !(P || N))
                {
                    // compare to string of Species in robo_protocol
                    for (int i = 0; i < listWorkingAllProtocols.Count; i++)
                    {
                        if (pString == listWorkingAllProtocols[i].Species_Name)
                            return i;
                    }
                }
                else if (!(H || M) && (P || N))
                {
                    // compare to string of selection in robo_protocol
                    for (int i = 0; i < listWorkingAllProtocols.Count; i++)
                    {
                        if (pString == listWorkingAllProtocols[i].Selection_Name)
                            return i;
                    }
                }
                else
                {
                    // compare to unfiltered name
                    for (int i = 0; i < listWorkingAllProtocols.Count; i++)
                    {
                        if (pString == listWorkingAllProtocols[i].Protocol_Name)
                            return i;
                    }
                }
                return -1;
            }
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
                for (int j = 0; j < myFilteredList.Count; j++)
                {
                    if (myFilteredList[j] == userList[i])
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
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);            

            List<RoboSep_Protocol> longList = null;
            List<RoboSep_Protocol> filteredList = null;
            if (horizontalTabs1.TabActive == tabAllProtocols)
            {
                longList = listWorkingAllProtocols;
                filteredList = listAllProtocolsFiltered;
            }
            else if (horizontalTabs1.TabActive == tabMyProtocols)
            {
                longList = myWorkingProtocolsList;
                filteredList = myFilteredList;
            }
            else
            {
                // do nothing
            }

            // update list
            if (longList != null && filteredList != null)
            {
                updateFilteredList(longList, filteredList);

                // display filtered list
                refreshList(filteredList);

                lvProtocolList.UpdateScrollbar();
            }
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
                    for (int i = 0; i < myFilteredList.Count; i++)
                        lvProtocolList.Items.Add(myFilteredList[i].SpeciesSelection_Name);
                }
                else
                {
                    // show Species filtered name
                    lvProtocolList.Items.Clear();
                    for (int i = 0; i < myFilteredList.Count; i++)
                        lvProtocolList.Items.Add(myFilteredList[i].Species_Name);
                }
            }
            else if (P || N)
            {
                lvProtocolList.Font = new System.Drawing.Font("Arial Narrow", 13F);
                // show Selection filtered name
                lvProtocolList.Items.Clear();
                for (int i = 0; i < myFilteredList.Count; i++)
                    lvProtocolList.Items.Add(myFilteredList[i].Selection_Name);
            }
            else
            {
                lvProtocolList.Font = new System.Drawing.Font("Arial Narrow", 12F);
                // show un-filtered name
                lvProtocolList.Items.Clear();
                for (int i = 0; i < myFilteredList.Count; i++)
                    lvProtocolList.Items.Add(myFilteredList[i].Protocol_Name);
            }

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

             lvProtocolList.ResumeLayout(true);
           }

        public void refreshList(List<RoboSep_Protocol> displayProtocolList)
        {
            if (displayProtocolList == null)
                return;

            lvProtocolList.SuspendLayout();

            lvProtocolList.BeginUpdate();

            if (displayProtocolList.Count > 0)
            {
                // determine which protocol name to display
                getFilters();
                if (H || M)
                {
                    if (P || N)
                    {
                        // show Species & Selection filtered name
                        lvProtocolList.Items.Clear();
                        for (int i = 0; i < displayProtocolList.Count; i++)
                        {
                            ListViewItem lvItem = new ListViewItem();
                            lvItem.Tag = displayProtocolList[i];
                            lvItem.Text = DisplayProtocolsAbbv ? displayProtocolList[i].SpeciesSelection_Name_Abbv : displayProtocolList[i].SpeciesSelection_Name;
                            lvProtocolList.Items.Add(lvItem);
                        }
                    }
                    else
                    {
                        // show Species filtered name
                        lvProtocolList.Items.Clear();
                        for (int i = 0; i < displayProtocolList.Count; i++)
                        {
                            ListViewItem lvItem = new ListViewItem();
                            lvItem.Tag = displayProtocolList[i];
                            lvItem.Text = DisplayProtocolsAbbv ? displayProtocolList[i].Species_Name_Abbv : displayProtocolList[i].Species_Name;
                            lvProtocolList.Items.Add(lvItem);
                        }
                    }
                }
                else if (P || N)
                {
                    // show Selection filtered name
                    lvProtocolList.Items.Clear();
                    for (int i = 0; i < displayProtocolList.Count; i++)
                    {
                        ListViewItem lvItem = new ListViewItem();
                        lvItem.Tag = displayProtocolList[i];
                        lvItem.Text = DisplayProtocolsAbbv ? displayProtocolList[i].Selection_Name_Abbv : displayProtocolList[i].Selection_Name;
                        lvProtocolList.Items.Add(lvItem);
                    }
                }
                else
                {
                    // show un-filtered name
                    lvProtocolList.Items.Clear();
                    for (int i = 0; i < displayProtocolList.Count; i++)
                    {
                        ListViewItem lvItem = new ListViewItem();
                        lvItem.Tag = displayProtocolList[i];
                        lvItem.Text = DisplayProtocolsAbbv ? displayProtocolList[i].Protocol_Name_Abbv : displayProtocolList[i].Protocol_Name;
                        lvProtocolList.Items.Add(lvItem);
                    }
                }
            }
            else
            {
                lvProtocolList.Items.Clear();
            }
            // Resize last column
            lvProtocolList.ResizeColumnWidth(columnHeader1, textFormat, lvProtocolList.Width);

            lvProtocolList.EndUpdate();
            lvProtocolList.ResumeLayout(true);
        }

        private void updateFilteredList(List<RoboSep_Protocol> protocolList, List<RoboSep_Protocol> protocolListFiltered)
        {
            if (protocolList == null || protocolListFiltered == null)
                return;

            List<RoboSep_Protocol> tempList = new List<RoboSep_Protocol>();
            getFilters();
             for (int i = 0; i < protocolList.Count; i++)
                    tempList.Add(protocolList[i]);

            protocolListFiltered.Clear();
            // filter by search txt
            if (search == string.Empty || search == null)
            {
                for (int i = 0; i < tempList.Count; i++)
                    protocolListFiltered.Add(tempList[i]);
            }
            else
            {
                // search all protocols after button filtering
                for (int i = 0; i < tempList.Count; i++)
                {
                    int searchResult = searchForTextInString(search, DisplayProtocolsAbbv ? tempList[i].Protocol_Name_Abbv : tempList[i].Protocol_Name);
                    if (searchResult != -1)
                    {
                        protocolListFiltered.Add(tempList[i]);
                    }
                }
            }
        }

        private void StoreChosenProtocolsWhenSwitchingTabs(List<string> listProtocols)
        {
            if (listProtocols == null)
                return;

            listProtocols.Clear();

            for (int i = 0; i < lvProtocolList.Items.Count; i++)
            {
                if (lvProtocolList.Items[i].Checked == false)
                    continue;

                RoboSep_Protocol tempProtocol = lvProtocolList.Items[i].Tag as RoboSep_Protocol;
                if (tempProtocol == null)
                {
                    continue;
                }
                listProtocols.Add(tempProtocol.Protocol_Name);
            }
        }

        private void RestoreChosenProtocolsAfterSwitchingTabs(List<string> listChosenProtocols)
        {
            if (lvProtocolList.Items.Count == 0 || listChosenProtocols == null || listChosenProtocols.Count == 0)
                return;

            RoboSep_Protocol pRoboSepProtocol = null;
            int index = 0;
            foreach (ListViewItem lvItem in lvProtocolList.Items)
            {
                if (lvItem.Tag == null)
                    continue;

                pRoboSepProtocol = lvItem.Tag as RoboSep_Protocol;
                if (string.IsNullOrEmpty(pRoboSepProtocol.Protocol_Name))
                    continue;

                index = listChosenProtocols.FindIndex(0, x => { return (x != null && pRoboSepProtocol.Protocol_Name.ToLower() == x.ToLower()); });
                if (0 <= index)
                {
                    lvItem.Checked = true;
                }
            }
        }

        private void UpdateProtocolOperatorButtons(HorizontalTabs.tabs tab)
        {
            if (tabMyProtocols == tab)
            {
                // Update Remove button bound
                btn_RemoveProtocols.SetBounds(rcButtonRemoveMyList.X, rcButtonRemoveMyList.Y, rcButtonRemoveMyList.Width, rcButtonRemoveMyList.Height);

                // Set visibility of buttons
                btn_USBload.Visible = false;
                btn_AddProtocols.Visible = false;
                btn_RemoveProtocols.Visible = CurrentUserIsPresetUser ? false : true;
            }
            else if (tabAllProtocols == tab)
            {
                // Update Remove button
                btn_RemoveProtocols.SetBounds(rcButtonRemoveAllList.X, rcButtonRemoveAllList.Y, rcButtonRemoveAllList.Width, rcButtonRemoveAllList.Height);

                // Set visibility of buttons
                btn_USBload.Visible = true;
                btn_AddProtocols.Visible = CurrentUserIsPresetUser ? false : true;
                btn_RemoveProtocols.Visible = CurrentUserIsPresetUser ? false : true;
            }

            string txtSearch = LanguageINI.GetString("lblSearch");
            string txt = protocol_SearchBar2.textBox_search.Text.Trim();

            if (!string.IsNullOrEmpty(txt) && txt != txtSearch)
            {
                this.protocol_SearchBar2.textBox_search.TextChanged -= evhHandleSearchBoxTextChanged;
                protocol_SearchBar2.textBox_search.Text = txtSearch;
                this.protocol_SearchBar2.textBox_search.TextChanged += evhHandleSearchBoxTextChanged;
            }
        }

        private void DisableTabMyProtocols()
        {
            tabAllProtocols  = HorizontalTabs.tabs.Tab1;
            tabMyProtocols = HorizontalTabs.tabs.Tab2;
            horizontalTabs1.Tab1 = TextAllProtocols;
            horizontalTabs1.Tab2 = "";

            horizontalTabs1.TabActive = HorizontalTabs.tabs.Tab1;
        }

        private bool IsMyProtocolsListDirty()
        {
            // Update my protocol list
            if (bMyProtocolsListDirty == false)
            {
                 return false;
            }

            // Check if the master list and the working list contain the same items
            string[] sMasterProtocolNames = null, aWorkingProtocolNames = null;
            sMasterProtocolNames = new string[myMasterProtocolsList.Count];
            for (int i = 0; i < myMasterProtocolsList.Count; i++)
            {
                sMasterProtocolNames[i] = (myMasterProtocolsList[i].Protocol_Name);
            }

            aWorkingProtocolNames = new string[myWorkingProtocolsList.Count];
            for (int i = 0; i < myWorkingProtocolsList.Count; i++)
            {
                aWorkingProtocolNames[i] = (myWorkingProtocolsList[i].Protocol_Name);
            }

            if (Utilities.UnorderedEqual(sMasterProtocolNames, aWorkingProtocolNames))
            {
                 // equal, not dirty
                return false;
            }

            // dirty
            return true;
        }

        private void DeleteProtocolsFromAllList(List<RoboSep_Protocol> listMarkedAsDeletedAllProtocols)
        {
            if (listMarkedAsDeletedAllProtocols == null || listMarkedAsDeletedAllProtocols.Count == 0)
                return;

            List<string> listFileNamesToBeDeleted = new List<string>();
            List<string> listNamesToBeDeleted = new List<string>();

            // delete those protocols not in the working list
            foreach (RoboSep_Protocol protocol in listMarkedAsDeletedAllProtocols)
            {
                listNamesToBeDeleted.Add(protocol.Protocol_Name);
                listFileNamesToBeDeleted.Add(protocol.Protocol_FileName);
            }

            // Ask user to confirm whether they want to delete protocols
            string msg = LanguageINI.GetString("msgDeleteProtocols");
            RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING, msg,
                LanguageINI.GetString("headerDeleteProtocols"), LanguageINI.GetString("Yes"), LanguageINI.GetString("No"));
            RoboSep_UserConsole.showOverlay();
            prompt.ShowDialog();
            RoboSep_UserConsole.hideOverlay();
            //
            if (prompt.DialogResult != DialogResult.OK)
            {
                prompt.Dispose();                
                return;
            }
            prompt.Dispose();                

            string protocolPath = RoboSep_UserDB.getInstance().GetProtocolsPath();

            // Remove files
            try
            {
                if (!string.IsNullOrEmpty(protocolPath))
                {
                    string fullPathFileName, logMSG;
                    foreach (string file in listFileNamesToBeDeleted)
                    {
                        if (string.IsNullOrEmpty(file))
                            continue;

                        fullPathFileName = protocolPath + file;
                        if (File.Exists(fullPathFileName))
                        {
                            File.Delete(fullPathFileName);

                            logMSG = String.Format("Delete protocol file from all protocols list: {0}", file);

                            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string logMSG = String.Format("Failed to delete protocol file from all protocols list. Exception: {0}", ex.Message);

                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                
            }

            // Get the list of users
            string[] allUsers = RoboSep_UserDB.getInstance().getAllUsers();
            if (allUsers != null)
            {
                string[] mruToBeDeleted = listNamesToBeDeleted.ToArray();
                string[] userProtocolToBeDeleted = listFileNamesToBeDeleted.ToArray();
                foreach (string user in allUsers)
                {
                    // Remove entries in user protocols list
                    RoboSep_UserDB.getInstance().deleteUserProtocols(user, userProtocolToBeDeleted);

                    // Remove MRU entries
                    RoboSep_UserDB.getInstance().RemoveMRUEntriesInProtocolList(user, mruToBeDeleted);
                }
            }

            // Remove deleted protocols from my list
            RemoveDeletedProtocolsFromMyList(listMarkedAsDeletedAllProtocols);

            // Clear the marked as deleted list
            listMarkedAsDeletedAllProtocols.Clear();

            // Reload all protocol lists
            StartReloadingAllProtocols();
        }

        private void RemoveDeletedProtocolsFromMyList(List<RoboSep_Protocol> listMarkedAsDeletedAllProtocols)
        {
            if (listMarkedAsDeletedAllProtocols == null || listMarkedAsDeletedAllProtocols.Count == 0)
                return;

            // Update the my master list and working list
            for (int i = 0; i < listMarkedAsDeletedAllProtocols.Count; i++)
            {
                RemoveProtocolFromUserList(listMarkedAsDeletedAllProtocols[i]);
            }

            IsUserProtocolFilesUpdated();
        }

        private bool IsUserProtocolFilesUpdated()
        {
            // Update my protocol list
            if (bMyProtocolsListDirty == false)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("---------No updating user protocol files is required."));
                return false;
            }

            // Check if the master list and the working list contain the same items
            string[] sMasterProtocolNames = null, aWorkingProtocolNames = null;
            sMasterProtocolNames = new string[myMasterProtocolsList.Count];
            for (int i = 0; i < myMasterProtocolsList.Count; i++)
            {
                sMasterProtocolNames[i] = (myMasterProtocolsList[i].Protocol_Name);
            }
            aWorkingProtocolNames = new string[myWorkingProtocolsList.Count];
            for (int i = 0; i < myWorkingProtocolsList.Count; i++)
            {
                aWorkingProtocolNames[i] = (myWorkingProtocolsList[i].Protocol_Name);
            }
            if (Utilities.UnorderedEqual(sMasterProtocolNames, aWorkingProtocolNames))
            {
                System.Diagnostics.Debug.WriteLine(String.Format("---------No changes in the user protocol list, no reloading of user protocols is required."));

                // equal, no need to do update
                bMyProtocolsListDirty = false;
                return false;
            }

            // Remove MRU entries if the protocols are no long in my protocol list
            RemoveMRUEntriesNotInMyProtocolList(aWorkingProtocolNames);

            // Update the user protocols file
            try
            {
                System.Diagnostics.Debug.WriteLine(String.Format("--------- Save dirty user protocols list."));

                string[] aWorkingProtocolFileNames = null;
                aWorkingProtocolFileNames = new string[aWorkingProtocolNames.Length];
                for (int i = 0; i < aWorkingProtocolNames.Length; i++)
                {
                    aWorkingProtocolFileNames[i] = RoboSep_UserDB.getInstance().getProtocolFilename(aWorkingProtocolNames[i]);
                }

                RoboSep_UserDB.getInstance().saveUserProtocols(RoboSep_UserConsole.strCurrentUser, aWorkingProtocolFileNames);
            }
            catch (Exception ex)
            {
                // LOG
               LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);               
            }

            return true;
        }

        private bool UpdateAndReloadUserProtocols()
        {
            if (IsUserProtocolFilesUpdated() == false)
                return false;

            System.Diagnostics.Debug.WriteLine(String.Format("---------Changes have been made in the user protocol list, reloading of user protocols is required."));

            // show loading protocol message
            string title = LanguageINI.GetString("headerLoadingProtocols");
            string msg = LanguageINI.GetString("LoadingAllProtocols");
            loading = new RoboMessagePanel5(RoboSep_UserConsole.getInstance(), title, msg, GUI_Controls.GifAnimationMode.eUploadingMultipleFiles);
            RoboSep_UserConsole.showOverlay();
            loading.Show();
            try
            {
                System.Diagnostics.Debug.WriteLine(String.Format("--------- Reload user protocols from the server. Current ThreadID = {0} ----------", Thread.CurrentThread.ManagedThreadId));

                LoadProtocolsToServer();
                SeparatorGateway.GetInstance().separatorUpdating = true;

                if (evhHandleUserProtocolsReloaded == null)
                    evhHandleUserProtocolsReloaded = new EventHandler(HandleUserProtocolsReloaded);

                RoboSep_UserConsole.getInstance().NotifyUserSeparationProtocolsUpdated += evhHandleUserProtocolsReloaded;

            }
            catch (Exception ex)
            {
                // LOG
                isProtocolLoading = false;
                LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);               
            }

            bMyProtocolsListDirty = false;
            return true;
        }

        private void RemoveMRUEntriesNotInMyProtocolList(string[] aWorkingProtocolNames)
        {
            if (aWorkingProtocolNames == null || aWorkingProtocolNames.Length == 0)
                return;

            RoboSep_UserDB.getInstance().RemoveMRUEntriesNotInProtocolList(RoboSep_UserConsole.strCurrentUser, aWorkingProtocolNames);
        }

        private void HandleUserProtocolsReloaded(object sender, EventArgs e)
        {
            RoboSep_UserConsole.getInstance().NotifyUserSeparationProtocolsUpdated -= evhHandleUserProtocolsReloaded;

            // set separatorUpdating back to false 
            SeparatorGateway.GetInstance().separatorUpdating = false;

            // reload user protocols
            LoadMyProtocolsList();

            // update the scroll bar 
            UpdateDisplayList();

            // close loading dialog
            if (loading != null)
            {
                loading.Close();
                loading = null;
            }
            RoboSep_UserConsole.hideOverlay();

            if (RoboSep_RunSamples.getInstance().IsInitialized)
            {
                RoboSep_RunSamples.getInstance().ReInitialize();
            }

            isProtocolLoading = false;
       }

        private void LoadProtocolsToServer()
        {
            // Reload protocols with SeparatorGateway using a thread
            Thread reloadProtocolsThread = new Thread(new ThreadStart(this.ReloadProtocolsThread));
            reloadProtocolsThread.IsBackground = true;
            reloadProtocolsThread.Start();
            isProtocolLoading = true;
        }

        // function run to refresh server's active user protocol list
        private void ReloadProtocolsThread()
        {
            // save list as old robosep would
            SeparatorGateway.GetInstance().ReloadProtocols();
            SeparatorGateway.GetInstance().Connect(false);
        }

        private static int searchForTextInString(string search, string theString)
        {
            for (int i = 0; i < theString.Length; i++)
            {
                if (theString[i].ToString().ToUpper() == search[0].ToString().ToUpper())
                {
                    //string temp = string.Empty;
                    StringBuilder tempString = new StringBuilder();
                    for (int j = 0; j < search.Length; j++)
                    {
                        if ((i + j) < theString.Length)
                            tempString.Append(theString[i + j]);
                    }
                    if (search.ToUpper() == tempString.ToString().ToUpper())//temp.ToUpper())
                    {
                        return i;
                    }
                }
            }
            return -1;
        }


        #region Form Events
 
        private void RemoveProtocolFromUserList(RoboSep_Protocol ProtocolToBeRemoved)
        {
            if (ProtocolToBeRemoved == null || string.IsNullOrEmpty(ProtocolToBeRemoved.Protocol_Name))
                return;

            if (myWorkingProtocolsList.Count > 0)
            {
                int index = myWorkingProtocolsList.FindIndex(0, x => { return (x != null && !string.IsNullOrEmpty(x.Protocol_Name) && x.Protocol_Name.ToLower() == ProtocolToBeRemoved.Protocol_Name.ToLower()); });
                if (0 <= index)
                {
                    myWorkingProtocolsList.RemoveAt(index);
                    bMyProtocolsListDirty = true;
                }
            }
        }

        private void RemoveProtocolFromAllList(List<RoboSep_Protocol> listMarkedAsDeletedAllProtocols, RoboSep_Protocol ProtocolToBeRemoved)
        {
            if (listMarkedAsDeletedAllProtocols == null || ProtocolToBeRemoved == null || string.IsNullOrEmpty(ProtocolToBeRemoved.Protocol_Name))
                return;

            if (listWorkingAllProtocols.Count > 0)
            {
                int index = listWorkingAllProtocols.FindIndex(0, x => { return (x != null && !string.IsNullOrEmpty(x.Protocol_Name) && x.Protocol_Name.ToLower() == ProtocolToBeRemoved.Protocol_Name.ToLower()); });
                if (0 <= index)
                {
                    index = listMarkedAsDeletedAllProtocols.FindIndex(0, x => { return (x != null && !string.IsNullOrEmpty(x.Protocol_Name) && x.Protocol_Name.ToLower() == ProtocolToBeRemoved.Protocol_Name.ToLower()); });
                    if (index < 0)
                    {
                        listMarkedAsDeletedAllProtocols.Add(ProtocolToBeRemoved);
                    }
                    bAllProtocolsListDirty = true;
                }
            }
        }

        private void AddProtocolToAllList(RoboSep_Protocol [] ProtocolsToBeAdd)
        {
            if (ProtocolsToBeAdd == null)
                return;

            foreach (RoboSep_Protocol protocol in ProtocolsToBeAdd)
            {
                if (protocol == null)
                    continue;

                int index = listWorkingAllProtocols.FindIndex(0, x => { return (x != null && !string.IsNullOrEmpty(x.Protocol_Name) && x.Protocol_Name.ToLower() == protocol.Protocol_Name.ToLower()); });
                if (index < 0)
                {
                    listWorkingAllProtocols.Add(protocol);
                    bAllProtocolsListDirty = true;
                }
            }
        }

        private void lvProtocolList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            RoboSep_Protocol tempProtocol = e.Item.Tag as RoboSep_Protocol;
            if (tempProtocol == null)
                return;

            if (lvProtocolList.MultiSelect == false && e.Item.Checked == true)
            {
                e.Item.Checked = false;
            }
        }
        private void lvProtocolList_ItemChecked(Object sender, ItemCheckedEventArgs e)
        {
            if (horizontalTabs1.TabActive == tabMyProtocols)
            {
                int nItemChecked = 0;
                ListViewItem lvItemChecked = null;
                for (int i = 0; i < lvProtocolList.Items.Count; i++)
                {
                    if (lvProtocolList.Items[i].Checked == false)
                        continue;

                    nItemChecked++;
                    if (nItemChecked == 1)
                    {
                        lvItemChecked = lvProtocolList.Items[i];
                    }
                }

                if (lvProtocolList.SelectedItems.Count == 1 && e.Item.Checked == false)
                {
                    if (nItemChecked <= 1)
                    {
                        ListViewItem lvItemSelected = lvProtocolList.SelectedItems[0];
                        lvItemSelected.Selected = false;
                        lvItemSelected.Focused = false;
                    }
                    
                    if (nItemChecked == 1 && lvItemChecked != null)
                    {
                        lvItemChecked.Selected = true;
                        lvItemChecked.Focused = true;
                    }
                    lvProtocolList.Invalidate();
                }
            }
        }

        private void lvProtocolList_Click(object sender, EventArgs e)
        {
            if (lvProtocolList.SelectedItems.Count == 0)
                return;

            ListViewItem lvItem = lvProtocolList.SelectedItems[0];
            if (lvItem == null || lvItem.Tag == null)
                return;

            RoboSep_Protocol tempProtocol = lvItem.Tag as RoboSep_Protocol;
            if (horizontalTabs1.TabActive == tabAllProtocols)
            {
                if (myWorkingProtocolsList.Count > 0)
                {
                    int index = myWorkingProtocolsList.FindIndex(0, x => { return (x != null && !string.IsNullOrEmpty(x.Protocol_Name) && x.Protocol_Name.ToLower() == tempProtocol.Protocol_Name.ToLower()); });
                    if (0 <= index)
                        return;
                }

                lvItem.Checked = !lvItem.Checked;
            }
            else if (horizontalTabs1.TabActive == tabMyProtocols)
            {
                 lvItem.Checked = !lvItem.Checked;
            }
        }

        private void btn_ScrollUp_Click(object sender, EventArgs e)
        {
             this.lvProtocolList.PageUp();
        }
        private void btn_ScrollDown_Click(object sender, EventArgs e)
        {
            this.lvProtocolList.PageDown();
        }
        private void btn_ScrollLeft_Click(object sender, EventArgs e)
        {
            SendMessage(this.lvProtocolList.Handle, (uint)WM_HSCROLL, (System.UIntPtr)ScrollEventType.LargeDecrement, (System.IntPtr)0);
        }
        private void btn_ScrollRight_Click(object sender, EventArgs e)
        {
            SendMessage(this.lvProtocolList.Handle, (uint)WM_HSCROLL, (System.UIntPtr)ScrollEventType.LargeIncrement, (System.IntPtr)0);
        }
        private void StartScrollUp(object sender, MouseEventArgs e)
        {
            this.lvProtocolList.StartScrollingUp();
        }
        private void StopScrollUp(object sender, MouseEventArgs e)
        {
            this.lvProtocolList.StopScrollingUp();
        }
        private void StartScrollDown(object sender, MouseEventArgs e)
        {
            this.lvProtocolList.StartScrollingDown();
        }
        private void StopScrollDown(object sender, MouseEventArgs e)
        {
            this.lvProtocolList.StopScrollingDown();
        }
        private void btn_ScrollUp_MouseDown(object sender, MouseEventArgs e)
        {
            StartScrollUp(sender, e);
        }
        private void btn_ScrollUp_MouseUp(object sender, MouseEventArgs e)
        {
            StopScrollUp(sender, e);
        }
        private void btn_ScrollDown_MouseDown(object sender, MouseEventArgs e)
        {
            StartScrollDown(sender, e);
        }
        private void btn_ScrollDown_MouseUp(object sender, MouseEventArgs e)
        {
            StopScrollDown(sender, e);
        }
        private void button_Cancel_Click(object sender, EventArgs e)
        {
            // Check if User list contains atleast 1 protocol (otherwise don't exit screen)
            bool validUser = myUserList.usrProtocols.Count > 0;
            if (validUser)
            {
                // Close control, go back to Protocol Window
                RoboSep_UserConsole.getInstance().SuspendLayout();
                RoboSep_UserConsole.getInstance().Controls.Add(RoboSep_Protocols.getInstance());
                RoboSep_Protocols.getInstance().UserName = strUserName;
                RoboSep_UserConsole.getInstance().Controls.Remove(this);
                RoboSep_UserConsole.getInstance().ResumeLayout();

                // LOG
                string logMSG = "Cancel button clicked";
                //GUI_Controls.uiLog.LOG(this, "button_Cancel_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                
            }
            else
            {
                RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING, 
                    LanguageINI.GetString("msgValidUser"), LanguageINI.GetString("headerValidUser"), LanguageINI.GetString("Ok"));
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                prompt.Dispose();                
                RoboSep_UserConsole.hideOverlay();
            }
        }
  
        private void button_myProfile_Click(object sender, EventArgs e)
        {
            // switch to my user protocols window
            RoboSep_UserConsole.getInstance().Controls.Remove(this);
            RoboSep_UserConsole.getInstance().Controls.Add(RoboSep_UserProtocolList.getInstance());

            string logMSG = "My Profile button clicked";
            //GUI_Controls.uiLog.LOG(this, "button_myProfile_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                
        }
        private void lvProtocolList_MouseLeave(object sender, EventArgs e)
        {
            if (lvProtocolList.SelectedItems.Count < 2 && lvProtocolList.MultiSelect)
                lvProtocolList.MultiSelect = false;
        }

        private void textBox_search_TextChanged(object sender, EventArgs e)
        {
            string txtSearch = LanguageINI.GetString("lblSearch");

            if (protocol_SearchBar2.textBox_search.Text.Trim() == txtSearch)
            {
                return;
            }

            UpdateFilters(sender, e);

            if (string.IsNullOrEmpty(protocol_SearchBar2.textBox_search.Text.Trim()))
            {
                protocol_SearchBar2.SearchBoxText = txtSearch;
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
            string txtSearch = LanguageINI.GetString("lblSearch");

            if (protocol_SearchBar2.textBox_search.Text.Trim() == txtSearch)
            {
                this.protocol_SearchBar2.textBox_search.TextChanged -= evhHandleSearchBoxTextChanged;
                protocol_SearchBar2.textBox_search.Text = "";
                this.protocol_SearchBar2.textBox_search.TextChanged += evhHandleSearchBoxTextChanged;
            }

            // Create keybaord control
            GUI_Controls.Keyboard newKeyboard =
                GUI_Controls.Keyboard.getInstance(RoboSep_UserConsole.getInstance(),
                    protocol_SearchBar2.textBox_search, null, RoboSep_UserConsole.getInstance().frmOverlay, false);
            newKeyboard.ShowDialog();

            if (string.IsNullOrEmpty(protocol_SearchBar2.textBox_search.Text.Trim()))
            {
                protocol_SearchBar2.SearchBoxText = txtSearch;
            }

            // add keyboard control to user console "track form"
            RoboSep_UserConsole.getInstance().addForm(newKeyboard, newKeyboard.Offset);

            newKeyboard.Dispose();
        }

        private List<string> AskUserToInsertUSBDrive()
        {
            List<string> lstUSBDrives = Utilities.GetUSBDrives();
            if (lstUSBDrives == null)
                return null;

            if (lstUSBDrives.Count > 0)
                return lstUSBDrives;

            // prompt if user to insert a USB drive
            string msg = LanguageINI.GetString("msgInsertUSB2");
            RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_INFORMATION, msg,
                LanguageINI.GetString("headerInsertUSB"), LanguageINI.GetString("Ok"), LanguageINI.GetString("Cancel"));
            RoboSep_UserConsole.showOverlay();
            prompt.ShowDialog();
            RoboSep_UserConsole.hideOverlay();

            if (prompt.DialogResult != DialogResult.OK)
            {
                prompt.Dispose();                
                return null;
            }
            prompt.Dispose();                
            return AskUserToInsertUSBDrive();
        }

        private void AddProtocolToUserList(RoboSep_Protocol ProtocolToBeAdded)
        {
            if (ProtocolToBeAdded == null || string.IsNullOrEmpty(ProtocolToBeAdded.Protocol_FileName) || string.IsNullOrEmpty(ProtocolToBeAdded.Protocol_Name))
                return;

            // check for duplicates 
            if (myWorkingProtocolsList.Count > 0)
            {
                int index = myWorkingProtocolsList.FindIndex(0, x => { return (x != null && !string.IsNullOrEmpty(x.Protocol_Name) && x.Protocol_Name.ToLower() == ProtocolToBeAdded.Protocol_Name.ToLower()); });
                if (0 <= index)
                {
                    // LOG
                    string LOGmsg = string.Format("Protocol {0} already in the user list. Do not add this protocol to the list.", ProtocolToBeAdded.Protocol_Name);

                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);                
                    return;
                }
            }

            System.Diagnostics.Debug.WriteLine(String.Format("---------Add protocol {0} to my protocol list.", ProtocolToBeAdded.Protocol_Name));

            // No duplicates
            myWorkingProtocolsList.Add(ProtocolToBeAdded);
            bMyProtocolsListDirty = true;
        }

        private void exit()
        {
            // close window, go back to protocols window
            RoboSep_Protocols.getInstance().UserName = strUserName;
            RoboSep_UserConsole.getInstance().Controls.Add(RoboSep_Protocols.getInstance());
            RoboSep_UserConsole.getInstance().Controls.Remove(this);
        }

        private void AddToMyList()
        {
            if (horizontalTabs1.TabActive != tabAllProtocols || lvProtocolList.Items.Count == 0)
                return;

            RoboSep_Protocol tempProtocol;
            int nCount = 0;
            for (int i = 0; i < lvProtocolList.Items.Count; i++)
            {
                if (lvProtocolList.Items[i].Checked == false)
                    continue;

                tempProtocol = lvProtocolList.Items[i].Tag as RoboSep_Protocol;
                if (tempProtocol == null)
                {
                    continue;
                }
                AddProtocolToUserList(tempProtocol);
                lvProtocolList.Items[i].Checked = false;
                nCount++;
            }

            if (SwitchToMyProtocolListAfterAddingAProtocol && nCount > 0)
            {
                horizontalTabs1.TabActive = tabMyProtocols;
                UpdateProtocolOperatorButtons(tabMyProtocols);
                RefreshProtocolDisplayList(myWorkingProtocolsList, myFilteredList);
            }
            else
            {
                lvProtocolList.Invalidate();
            }
        }

        private void RemoveFromMyList()
        {
            if (horizontalTabs1.TabActive != tabMyProtocols || lvProtocolList.Items.Count == 0)
                return;

            RoboSep_Protocol tempProtocol = null;
            for (int i = 0; i < lvProtocolList.Items.Count; i++)
            {
                if (lvProtocolList.Items[i].Checked == false)
                    continue;

                tempProtocol = lvProtocolList.Items[i].Tag as RoboSep_Protocol;
                if (tempProtocol == null)
                {
                    continue;
                }

                RemoveProtocolFromUserList(tempProtocol);

                lvProtocolList.Items[i].Checked = false;
            }
        }

        private void btn_AddProtocols_Click(object sender, EventArgs e)
        {
            if (horizontalTabs1.TabActive == tabAllProtocols)
            {
                AddToMyList();
            }

            UpdateScrollIndicatorVisibility();
        }

        private void btn_RemoveProtocols_Click(object sender, EventArgs e)
        {
            if (lvProtocolList.Items.Count == 0)
                return;

            RoboSep_Protocol tempProtocol = null;
            if (horizontalTabs1.TabActive == tabAllProtocols)
            {
                string sMSG, sMSG1, temp;

                List<RoboSep_Protocol> listMarkedAsDeletedAllProtocols = new List<RoboSep_Protocol>();

                // Remove protocols from all protocols list
                for (int i = 0; i < lvProtocolList.Items.Count; i++)
                {
                    if (lvProtocolList.Items[i].Checked == false)
                        continue;

                    tempProtocol = lvProtocolList.Items[i].Tag as RoboSep_Protocol;
                    if (tempProtocol == null)
                    {
                        continue;
                    }

                    if (IsProtocolRemovable(tempProtocol.Protocol_FileName) == true)
                    {
                        RemoveProtocolFromAllList(listMarkedAsDeletedAllProtocols, tempProtocol);
                    }
                    else
                    {
                        // display a message that this protocol cannot be deleted
                        sMSG = LanguageINI.GetString("headerProtocolsUndeletable");
                        temp = LanguageINI.GetString("msgProtocolsUndeletable");
                        sMSG1 = String.Format(temp, tempProtocol.Protocol_Name);

                        GUI_Controls.RoboMessagePanel prompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_ERROR,
                            sMSG1, sMSG, LanguageINI.GetString("Ok"));
                        RoboSep_UserConsole.showOverlay();
                        prompt.ShowDialog();
                        prompt.Dispose();                
                        RoboSep_UserConsole.hideOverlay();
                    }
                    lvProtocolList.Items[i].Checked = false;
                }

                if (listMarkedAsDeletedAllProtocols.Count > 0)
                {
                    // Delete protocols from All Protocol
                    DeleteProtocolsFromAllList(listMarkedAsDeletedAllProtocols);
                }
            }
            else if (horizontalTabs1.TabActive == tabMyProtocols)
            {
                // Remove protocols from my protocols list
                for (int i = 0; i < lvProtocolList.Items.Count; i++)
                {
                    if (lvProtocolList.Items[i].Checked == false)
                        continue;

                    tempProtocol = lvProtocolList.Items[i].Tag as RoboSep_Protocol;
                    if (tempProtocol == null)
                    {
                        continue;
                    }

                    RemoveProtocolFromUserList(tempProtocol);

                    lvProtocolList.Items[i].Checked = false;
                }
            }

            UpdateDisplayList();
            UpdateScrollIndicatorVisibility();
        }

        private bool IsProtocolListDirty(string[] aMasterProtocolNames, string[] aWorkingProtocolNames)
        {
            if (aMasterProtocolNames == null || aWorkingProtocolNames == null || aMasterProtocolNames.Length == 0 || aWorkingProtocolNames.Length == 0)
                return false;

            return !Utilities.UnorderedEqual(aMasterProtocolNames, aWorkingProtocolNames);
        }

        private void btn_USBload_Click(object sender, EventArgs e)
        {
            List<string> lstUSBDrives = AskUserToInsertUSBDrive();
            if (lstUSBDrives == null || lstUSBDrives.Count == 0)
            {
                return;
            }

            // add protocols from USB folder
            string destinationPath = RoboSep_UserDB.getInstance().GetProtocolsPath();

            // determine usb directory
            string USBpath = string.Empty;
            try
            {
                for (int i = 0; i < lstUSBDrives.Count; i++)
                {
                    if (Directory.Exists(lstUSBDrives[i]))
                    {
                        USBpath = lstUSBDrives[i];
                        if (Directory.Exists((lstUSBDrives[i] + "Protocols\\")))
                        {
                            USBpath += "Protocols\\";
                            break;
                        }
                    }
                }
                if (USBpath != string.Empty)
                {
                    // clean up
                    if (myFB != null)
                    {
                        myFB.Dispose();
                        myFB = null;
                    }

                    // load file browser window
                    string sTitle = LanguageINI.GetString("headerSelectProtocol");
                    myFB = new FileBrowser(RoboSep_UserConsole.getInstance(), sTitle, 
                        USBpath, destinationPath, new string[] { "xml" }, FileBrowser.BrowseResult.Copy);

                    myFB.KeepOriginalFilesForRestoration = false;
                    myFB.EnableFilesOverwritten = true;
                    myFB.ShowProgressBarWhileCopying = true;

                    Point offset = myFB.Offset;
                    int x = RoboSep_UserConsole.getInstance().Location.X + offset.X;
                    int y = RoboSep_UserConsole.getInstance().Location.Y + offset.Y + 20;

                    myFB.Location = new Point(x, y);

                    // show file browser
                    RoboSep_UserConsole.showOverlay();
                    myFB.ShowDialog();
                    RoboSep_UserConsole.hideOverlay();

                    // if protocols added, re-attain and show protocol list
                    if (myFB.DialogResult == DialogResult.OK)
                    {
                        // check if userl protocols list dirty
                        bool bDirty = IsMyProtocolsListDirty();
                        if (bDirty)
                        {
                            // Save user protocol files
                            IsUserProtocolFilesUpdated();
                        }

                        // Save automatically
                        // Reload all protocol lists
                        StartReloadingAllProtocols();

                        // LOG
                        string logMSG = "Loading from USB directory '" + USBpath + "'";

                        LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                
                    }
                    myFB.Dispose();
                }
                else
                {
                    // set up message prompt
                    string sTitle = LanguageINI.GetString("headerUSBFail");
                    string sMSG = LanguageINI.GetString("msgUSBFail");
                    RoboMessagePanel err = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_ERROR, sMSG, sTitle, LanguageINI.GetString("Ok"));
                    RoboSep_UserConsole.showOverlay();
                    // prompt user
                    err.ShowDialog();
                    err.Dispose();
                    RoboSep_UserConsole.hideOverlay();

                    // LOG
                    string LOGmsg = "Error: USB drive not detected";
                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);
                }
            }
            catch (Exception ex)
            {
                // set up message prompt
                string sTitle = LanguageINI.GetString("headerUSBFail");
                string sMSG = LanguageINI.GetString("msgUSBFail");
                RoboMessagePanel err = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_ERROR, sMSG, sTitle, LanguageINI.GetString("Ok"));
                RoboSep_UserConsole.showOverlay();
                // prompt user
                err.ShowDialog();
                err.Dispose();
                RoboSep_UserConsole.hideOverlay();

                // LOG
                LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);               
            }
        }

        private void CopyProtocolFile(object sender, CopyFileEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate() { this.CopyProtocolFile(sender, e); });
            }
            else
            {
                if (myFB != null)
                {
                //    myFB.Hide();
                }
                if (copyingMsg == null)
                {
                    // show progress bar
                    string msg = LanguageINI.GetString("msgWaitCopyingFiles");
                    copyingMsg = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_NOTAPPLICABLE, msg, true, true);

                  //  copyingMsg = new RoboMessagePanel(myFB, MessageIcon.MBICON_NOTAPPLICABLE, msg, true, true);
                    RoboSep_UserConsole.showOverlay();
                    copyingMsg.Show(myFB);
                    RoboSep_UserConsole.hideOverlay();
                }
                int percent = e.FileNumber / e.TotalFiles;
                copyingMsg.setProgress(percent);
                copyingMsg.BringToFront();
            }
        }

        private void CopyProtocolFilesFinished(object sender, CopyFileEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate() { this.CopyProtocolFilesFinished(sender, e); });
            }
            else
            {
                // Clean up
                if (copyingMsg != null)
                {
                    copyingMsg.Close();
                    copyingMsg = null;
                }

                string LOGmsg = string.Format("Successfully copied {0} of {1} files", e.FileNumber, e.TotalFiles);

                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);                
                // prompt user that file copy is complete
                string msg = LanguageINI.GetString("msgCopyComplete");
                msg += e.FileNumber.ToString() + " ";
                msg += LanguageINI.GetString("msgCopyComplete2");
                string header = LanguageINI.GetString("headerCopy");
                GUI_Controls.RoboMessagePanel CopyComplete = new GUI_Controls.RoboMessagePanel(
                    RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_INFORMATION, msg, header, LanguageINI.GetString("Ok"));
                RoboSep_UserConsole.showOverlay();
                CopyComplete.ShowDialog();
                CopyComplete.Dispose();
                RoboSep_UserConsole.hideOverlay();
            }
        }

        private void lvProtocolList_KeyDown(object sender, KeyEventArgs e)
        {
            // check for modifier keys
            Keys CntrlKey = e.Modifiers;
            if (CntrlKey == Keys.Shift)
            {
                lvProtocolList.MultiSelect = true;
            }
            else if (CntrlKey == Keys.Control)
            {
                lvProtocolList.MultiSelect = true;
                // keyData for CTRL = 17, A = 65
                if ((Keys)e.KeyValue == Keys.A)
                {
                    for (int i = 0; i < lvProtocolList.Items.Count; i++)
                    {
                        lvProtocolList.Items[i].Selected = true;
                    }
                }
            }
        }
        private void lvProtocolList_KeyUp(object sender, KeyEventArgs e)
        {
            Keys CntrlKey = Control.ModifierKeys;
            if (CntrlKey != Keys.Shift && CntrlKey == Keys.Control)
            {
                if (lvProtocolList.SelectedItems.Count < 2)
                    lvProtocolList.MultiSelect = false;
            }
        }

        private void horizontalTabs1_Tab1_Click(object sender, EventArgs e)
        {
            if (horizontalTabs1.TabActive == HorizontalTabs.tabs.Tab1)
                return;

            horizontalTabs1.TabActive = HorizontalTabs.tabs.Tab1;

            // clear filter when swtiching tabs
            clearFilters();

            UpdateDisplayList();

            UpdateScrollIndicatorVisibility();
        }

        private void horizontalTabs1_Tab2_Click(object sender, EventArgs e)
        {
            if (horizontalTabs1.TabActive == HorizontalTabs.tabs.Tab2)
                return;

            horizontalTabs1.TabActive = HorizontalTabs.tabs.Tab2;

            // clear filter when swtiching tabs
            clearFilters();

            UpdateDisplayList();

            UpdateScrollIndicatorVisibility();
        }

        private void UpdateDisplayList()
        {
            if (tabMyProtocols == horizontalTabs1.TabActive)
            {
                StoreChosenProtocolsWhenSwitchingTabs(listChosenProtocolsFromAllProtocols);
                UpdateProtocolOperatorButtons(tabMyProtocols);
                RefreshProtocolDisplayList(myWorkingProtocolsList, myFilteredList);
                // RestoreChosenProtocolsAfterSwitchingTabs(listChosenProtocolsFromMyProtocols);
            }
            else if (tabAllProtocols == horizontalTabs1.TabActive)
            {
                StoreChosenProtocolsWhenSwitchingTabs(listChosenProtocolsFromMyProtocols);
                UpdateProtocolOperatorButtons(tabAllProtocols);
                RefreshProtocolDisplayList(listWorkingAllProtocols, listAllProtocolsFiltered);
                // RestoreChosenProtocolsAfterSwitchingTabs(listChosenProtocolsFromAllProtocols);
            }
            else
            {
                // do nothing
            }

            lvProtocolList.UpdateScrollbar();
        }

        private void lvProtocolList_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            System.Drawing.Drawing2D.LinearGradientBrush GradientBrush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.Blue, Color.LightBlue, 270);
            Pen thePen = new Pen(new SolidBrush(Color.White), 2);
            SolidBrush theBrush = new SolidBrush(Color.Yellow);
            Font theFont = new Font("Arial", 12);
            e.Graphics.FillRectangle(GradientBrush, e.Bounds);

            e.Graphics.DrawRectangle(thePen, e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2);

            e.Graphics.DrawString(e.Header.Text, theFont, theBrush, e.Bounds);

            GradientBrush.Dispose();
            thePen.Dispose();
            theBrush.Dispose();
            theFont.Dispose();
        }

        private void lvProtocolList_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            Color textColour = Txt_Color;
            SolidBrush theBrush1, theBrush2;

            Rectangle itemBounds = new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width, e.Bounds.Bottom - e.Bounds.Top);
            if (e.Item.Selected)
            {
                theBrush1 = new SolidBrush(BG_Selected);
                e.Graphics.FillRectangle(theBrush1, itemBounds);
                textColour = Color.White;
                theBrush1.Dispose();
            }
            else if (e.ItemIndex % 2 == 0)
            {
                theBrush1 = new SolidBrush(BG_ColorEven);
                e.Graphics.FillRectangle(theBrush1, itemBounds);
                theBrush1.Dispose();
            }
            else
            {
                theBrush1 = new SolidBrush(BG_ColorOdd);
                e.Graphics.FillRectangle(theBrush1, itemBounds);
                theBrush1.Dispose();
            }

            if (horizontalTabs1.TabActive == tabMyProtocols)
            {
                ListViewItem lvItem = e.Item;
                RoboSep_Protocol protocol = lvItem.Tag as RoboSep_Protocol;
                if (protocol != null && !string.IsNullOrEmpty(protocol.Protocol_Name))
                {
                    itemBounds = DrawIconInMyList(protocol.Protocol_Name, protocol.numQuadrants, e.Graphics, itemBounds, e.ItemIndex, e.Item.Selected, e.Item.Checked);
                 }
            }
            else if (horizontalTabs1.TabActive == tabAllProtocols)
            {
                ListViewItem lvItem = e.Item;
                RoboSep_Protocol protocol = lvItem.Tag as RoboSep_Protocol;
                if (protocol != null && !string.IsNullOrEmpty(protocol.Protocol_Name))
                {
                    itemBounds = DrawIconInAllList(protocol.Protocol_Name, protocol.Protocol_FileName, e.Graphics, itemBounds, e.ItemIndex, e.Item.Selected, e.Item.Checked);
                }
            }
            theBrush2 = new SolidBrush(textColour);
            e.Graphics.DrawString(e.Item.Text, lvProtocolList.Font, theBrush2 ,
                new Rectangle(itemBounds.Left, itemBounds.Top, itemBounds.Width, itemBounds.Bottom - itemBounds.Top), textFormat);
            theBrush2.Dispose();
        }

        private Rectangle DrawIconInMyList(string protocolName, int numQuadrant, Graphics graphics, Rectangle rectCell, int itemIndex, bool bSelected, bool bChecked)
        {
            int th, ty, tw, tx;
            SolidBrush theBrush1, theBrush2;
            th = IMAGEBOX_SIZE + (CellPaddingSize * 2);
            tw = IMAGEBOX_SIZE + (CellPaddingSize * 3);

            if ((tw > rectCell.Width) || (th > rectCell.Height))
                return rectCell;					// not enough room to draw the image, bail out

            if (bSelected)
            {
                if (itemIndex % 2 == 0)
                {
                    theBrush1 = new SolidBrush(BG_ColorEven);
                    graphics.FillRectangle(theBrush1, rectCell);
                    theBrush1.Dispose();
                }
                else
                {
                    theBrush1 = new SolidBrush(BG_ColorOdd);
                    graphics.FillRectangle(theBrush1, rectCell);
                    theBrush1.Dispose();
                }
            }

            ty = rectCell.Y + ((rectCell.Height - th) / 2);

            // Draw check marked if the item is checked and multiselect is enabled
            if (bChecked)
            {
                tx = rectCell.X + CellPaddingSize;
                graphics.DrawImage(this.imageList1.Images[1], tx, ty);

                // remove the width that we used for the graphic from the cell
                rectCell.Width -= (IMAGEBOX_SIZE + 2 * (CellPaddingSize));
                rectCell.X += (tw + CellPaddingSize);
            }
            theBrush2 = new SolidBrush(BG_Selected);
            if (bSelected)
                graphics.FillRectangle(theBrush2, rectCell);
            theBrush2.Dispose();

            return rectCell;
        }

        private Rectangle DrawIconInAllList(string protocolName, string protocolFileName, Graphics graphics, Rectangle rectCell, int itemIndex, bool bSelected, bool bChecked)
        {
            if (myWorkingProtocolsList.Count == 0)
            {
                if (!bChecked)
                    return rectCell;
            }
            else
            {
                int index = myWorkingProtocolsList.FindIndex(0, x => { return (x != null && !string.IsNullOrEmpty(x.Protocol_Name) && x.Protocol_Name.ToLower() == protocolName.ToLower()); });
                if (index < 0 && !bChecked)
                    return rectCell;
            }

            int th, ty, tw, tx;
            th = IMAGEBOX_SIZE + (CellPaddingSize * 2);
            tw = IMAGEBOX_SIZE + (CellPaddingSize * 3);

            if ((tw > rectCell.Width) || (th > rectCell.Height))
                return rectCell;					// not enough room to draw the image, bail out

            if (bSelected)
            {
                SolidBrush theBrush;
                if (itemIndex % 2 == 0)
                {
                    theBrush = new SolidBrush(BG_ColorEven);
                    graphics.FillRectangle(theBrush, rectCell);
                }
                else
                {
                    theBrush = new SolidBrush(BG_ColorOdd);
                    graphics.FillRectangle(theBrush, rectCell);
                }
                theBrush.Dispose();
            }
            ty = rectCell.Y + ((rectCell.Height - th) / 2);
            tx = rectCell.X + CellPaddingSize;


            int imageIndex = bChecked ? 1 : 0;
            graphics.DrawImage(this.imageList1.Images[imageIndex], tx, ty);

            // remove the width that we used for the graphic from the cell
            rectCell.Width -= (IMAGEBOX_SIZE + 2 * CellPaddingSize);
            rectCell.X += (tw + CellPaddingSize);

            SolidBrush theBrush2 = new SolidBrush(BG_Selected);
            
            if (bSelected)
                graphics.FillRectangle(theBrush2, rectCell);

            theBrush2.Dispose();

            return rectCell;
        }

        private bool IsProtocolMarkedAsAddedToMyListFromAllList(string protocolName)
        {
            if (string.IsNullOrEmpty(protocolName))
                return false;

            // must be in my working list
             bool bMarkedAsAddedToMyListFromAllList = false;
            if (myWorkingProtocolsList != null && (myWorkingProtocolsList.Count > 0))
            {
                int index = myWorkingProtocolsList.FindIndex(0, x => { return (x != null && !string.IsNullOrEmpty(x.Protocol_Name) && x.Protocol_Name.ToLower() == protocolName.ToLower()); });
                if (0 <= index)
                {
                    bMarkedAsAddedToMyListFromAllList = IsProtocolNewlyAddedToMyList(protocolName);
                }
           }
           return bMarkedAsAddedToMyListFromAllList;
        }

        private bool IsProtocolNewlyAddedToMyList(string protocolName)
        {
            if (string.IsNullOrEmpty(protocolName))
                return false;

            bool bNewlyAddedToMyList = true;
            if (myMasterProtocolsList != null && myMasterProtocolsList.Count > 0)
            {
                int index = myMasterProtocolsList.FindIndex(0, x => { return (x != null && !string.IsNullOrEmpty(x.Protocol_Name) && x.Protocol_Name.ToLower() == protocolName.ToLower()); });
                if (0 <= index)
                    bNewlyAddedToMyList = false;
            }
            return bNewlyAddedToMyList;
        }

        private enum protocolName
        {
            Full,
            Species,
            Selection,
            Species_and_Selection
        };

        private void lvProtocolList_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
        }

        private void RemoveTempFolder()
        {
            if (!string.IsNullOrEmpty(tempFolderForFileRestoration))
            {
                try
                {
                    if (Directory.Exists(tempFolderForFileRestoration))
                    {
                        Utilities.RemoveTempFileDirectory(tempFolderForFileRestoration);
                    }
                }
                // Catch any exception if a file cannot be accessed
                // e.g. due to security restriction
                catch (Exception ex)
                {
                    string sErrMsg = String.Format("Failed to remove temporary directory {0}. Exception: {1}", tempFolderForFileRestoration, ex.Message);

                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, sErrMsg);                
                }

                tempFolderForFileRestoration = String.Empty;
            }
        }

        private void SaveMyProtocolList()
        {
            // User protocols
            UpdateAndReloadUserProtocols();

            string logMSG = "Save user protocol list";

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                
        }

        private void ClearLists()
        {
            listMasterAllProtocols.Clear();
            listWorkingAllProtocols.Clear();
            listAllProtocolsFiltered.Clear();


            myMasterProtocolsList.Clear();
            myWorkingProtocolsList.Clear();
            myFilteredList.Clear();

            if (listChosenProtocolsFromAllProtocols != null)
            {
                listChosenProtocolsFromAllProtocols.Clear();
            }
            if (listChosenProtocolsFromMyProtocols != null)
            {
                listChosenProtocolsFromMyProtocols.Clear();
            }
            bMyProtocolsListDirty = false;
            bAllProtocolsListDirty = false;
        }


        protected override void btn_home_Click(object sender, EventArgs e)
        {
            btn_home.Enabled = false;
            
            SaveMyProtocolList();
            UpdateDisplayList();

            // Clear lists
            ClearLists();

            do
            {
                Application.DoEvents();
                Thread.Sleep(10);
            } while (isProtocolLoading == true);

            btn_home.Enabled = true;

            base.btn_home_Click(sender, e);
        }
    }
        #endregion





}
