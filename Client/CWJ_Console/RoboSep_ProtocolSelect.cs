﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

using GUI_Controls;
using System.Threading;
using Tesla.OperatorConsoleControls;
using Tesla.Common.Protocol;
using Tesla.Common.Separator;
using Tesla.Common;

using Invetech.ApplicationLog;

namespace GUI_Console
{
    public partial class RoboSep_ProtocolSelect :  UserControl
    {
        public enum ClosingState
        {
            eUndefined,
            eLoadingUserProtocols,
            eLoadingUserProtocolsFinished,
            eLoadingUserProtocolsOK,
            eRestoringUserProtocols,
            eRestoringUserProtocolsOK,
        }

        private ClosingState eClosingState = ClosingState.eUndefined;
        private QuadrantId CurrentQuadrant;
        private int QuadrantsAvailable;
        private Thread myReloadProtocolsThread;
        private SeparatorGateway mySeparatorGateway;
        // protocol management
        private RoboSep_UserDB myDB;
        private List<RoboSep_Protocol> listAllProtocols;
        private List<RoboSep_Protocol> listAllProtocolsFiltered;
        private List<RoboSep_Protocol> myFilteredList;
 
        List<RoboSep_Protocol> myWorkingProtocolsList;
        List<RoboSep_Protocol> myMasterProtocolsList;

        List<string> listChosenProtocolsFromAllProtocols = null;
        List<string> listChosenProtocolsFromMyProtocols = null;

        public ProtocolInfo[] runSamplesLatestSelectedProtocols = null;
        ProtocolInfo protocolToBeSelectedBeforeClosing;
        
        string[] aWorkingProtocolNamesBeforeClosing = null;

        private RoboSep_Protocol protocolSelection = null;
        private readonly object lockObject = new object();

        private const string ProtocolSuffix = "Protocol";
        private const string ProtocolFileExtension = ".xml";
        private const string TextMyProtocols = "My Protocols";
        private const string TextAllProtocols = "All Protocols";

        private HorizontalTabs.tabs tabMyProtocols = HorizontalTabs.tabs.Tab1;
        private HorizontalTabs.tabs tabAllProtocols = HorizontalTabs.tabs.Tab2;

        Rectangle rcBtnAddProtocols;
        Rectangle rcBtnRemoveProtocols;
        Rectangle rcScrollIndicator;
        Rectangle rcListViewProtocols;
        int ListViewLastColumnWidth;
        int SpacingBetweenLVAndIndicator = 0;

        private bool playButtonPressed = false;

        private const int SERVER_WAIT_TIME = 200;
        private const int IMAGELIST_WIDTH = 40;
        private const int IMAGELIST_HEIGHT = 40;
        private const int IMAGEBOX_SIZE = 30;
        private const int CellPaddingSize = 4;
        private const int EndPaddingSize = 6;
        private const int MaxQuadrantNumberIndex = 3;   // zero based
               
        // Listview Drawing Vars
        private StringFormat textFormat;
        private Color BG_ColorEven;
        private Color BG_ColorOdd;
        private Color BG_Selected;
        private Color Txt_Color;
   
        // search filters
        private string search;

        private int reloadProtocolCount = 0;
        private int reloadMaximum = 3;

        private int reloadUserProtocolsCount = 0;
        private int reloadUserProtocolstMaxCount = 3;
        private int thumbSize = 15;

        private bool DisplayingUserProtocols = true;
        private bool DisplayProtocolsAbbv = true;
        private bool SwitchToMyProtocolListAfterAddingAProtocol = false;
        private bool UseAllProtocolsListForSelection = false;

        private bool isDirty = false;
        private bool bReturnToHomeScreen = false;
        private bool bSelectProtocolBeforeReturningToHomeScreen = false;
        private bool UseFirstTabToDisplayAllProtocolsList = false;
        private bool bShowRequiredQuadrants = false;
        private bool bDisableAllProtocol = true;

        private bool CurrentUserIsPresetUser = false;

        private float cornerRounding = 16.0f;
        private Color Box_Color;


        private string lastSelectedProtocolName = "";
        private const int WM_HSCROLL = 0x114;
        private const int WM_VSCROLL = 0x115;

        private IniFile LanguageINI = null;
        RoboMessagePanel5 loading;
        ISeparationProtocol[] UserProtocolsListFromServer = null;


        private System.EventHandler evhHandleSearchBoxTextChanged = null;
        private System.EventHandler evhHandleUserProtocolsReloaded = null;
        private System.EventHandler evhHandleRestoreProtocolsFinished = null;
        private System.EventHandler evhSelectProtocolClick = null;
        private List<Image> ilist1 = new List<Image>();
        private List<Image> ilist2 = new List<Image>();
        private List<Image> ilist3 = new List<Image>();

        System.Windows.Forms.ListViewItemSelectionChangedEventHandler evHandleItemSelectionChanged = null;
         
        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, UIntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static public extern bool EnableScrollBar(System.IntPtr hWnd, uint wSBflags, uint wArrows);

        private static RoboSep_ProtocolSelect theProtocolSelect = null;

        public RoboSep_ProtocolSelect(QuadrantId Quadrant, int available, QuadrantInfo[] quadInfos)
        {
            InitializeComponent();

            CurrentQuadrant = Quadrant;
            QuadrantsAvailable = available;

            SuspendLayout();

            this.DoubleBuffered = true;
            this.UpdateStyles();

            //change toggle button graphics
            ilist1.Add(Properties.Resources.PR_BTN08L_add_protocol_STD);
            ilist1.Add(Properties.Resources.PR_BTN08L_add_protocol_OVER);
            ilist1.Add(Properties.Resources.PR_BTN08L_add_protocol_OVER);
            ilist1.Add(Properties.Resources.PR_BTN08L_add_protocol_CLICK);
            btn_AddProtocols.ChangeGraphics(ilist1);


            ilist2.Add(Properties.Resources.PR_BTN09L_remove_protocol_STD);
            ilist2.Add(Properties.Resources.PR_BTN09L_remove_protocol_OVER);
            ilist2.Add(Properties.Resources.PR_BTN09L_remove_protocol_OVER);
            ilist2.Add(Properties.Resources.PR_BTN09L_remove_protocol_CLICK);
            btn_RemoveProtocols.ChangeGraphics(ilist2);

            ilist3.Add(Properties.Resources.L_104x86_single_arrow_right_STD);
            ilist3.Add(Properties.Resources.L_104x86_single_arrow_right_OVER);
            ilist3.Add(Properties.Resources.L_104x86_single_arrow_right_OVER);
            ilist3.Add(Properties.Resources.L_104x86_single_arrow_right_CLICK);
            btn_SelectProtocol.ChangeGraphics(ilist3);
            btn_SelectProtocol.disableImage = Properties.Resources.L_104x86_single_arrow_right_DISABLE;

            SetUpSearchBarEvents();

            myWorkingProtocolsList = new List<RoboSep_Protocol>();
            myMasterProtocolsList = new List<RoboSep_Protocol>();
            listChosenProtocolsFromAllProtocols = new List<string>();
            listChosenProtocolsFromMyProtocols = new List<string>();

            mySeparatorGateway = SeparatorGateway.GetInstance();

            StoreRunSamplesSelectedProtocols(quadInfos);

            // LOG
            string LOGmsg = "New Protocol Select user control generated";
            //GUI_Controls.uiLog.LOG(this, "RoboSep_ProtocolSelect", GUI_Controls.uiLog.LogLevel.EVENTS, LOGmsg);
            //  (LOGmsg);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);                

            SetUpScrollIndicatorThumbs();

            ResumeLayout();

            // get language file
            LanguageINI = GUI_Console.RoboSep_UserConsole.getInstance().LanguageINI;

            // set text values on buttons and labels to specified language
            protocol_SearchBar2.DefaultSearchText = LanguageINI.GetString("lblSearch");

            // set up for drawing
            textFormat = new StringFormat();
            textFormat.Alignment = StringAlignment.Near;
            textFormat.LineAlignment = StringAlignment.Center;
            textFormat.FormatFlags = StringFormatFlags.NoWrap;
            BG_ColorEven = Color.FromArgb(216, 217, 218);
            BG_ColorOdd = Color.FromArgb(243, 243, 243);
            BG_Selected = Color.FromArgb(78, 38, 131);
            Txt_Color = Color.FromArgb(95, 96, 98);

            Box_Color = Color.FromArgb(78, 38, 131);

            rcBtnAddProtocols = btn_AddProtocols.Bounds;
            rcBtnRemoveProtocols = btn_RemoveProtocols.Bounds;

            theProtocolSelect = this;
        }

        public static RoboSep_ProtocolSelect GetInstance()
        {
            return theProtocolSelect;
        }

        private void RoboSep_ProtocolSelect_Load(object sender, EventArgs e)
        {
            // load protocols
            myDB = RoboSep_UserDB.getInstance();

            // get all protocols list
            listAllProtocols = myDB.getAllProtocols();

            listAllProtocolsFiltered = new List<RoboSep_Protocol>();
            myFilteredList = new List<RoboSep_Protocol>();

            LoadUserProtocols();

            CreateImageListGraphics();

            listView_UserProtocols.NUM_VISIBLE_ELEMENTS = 5;
            listView_UserProtocols.VERTICAL_PAGE_SIZE = 4;

            DisplayProtocolsAbbv = RoboSep_UserDB.getInstance().UseAbbreviationsForProtocolNames;
            SwitchToMyProtocolListAfterAddingAProtocol = RoboSep_UserDB.getInstance().SwitchToMyProtocolListAfterAddingAProtocol;

            UseAllProtocolsListForSelection = false;

            UseFirstTabToDisplayAllProtocolsList = RoboSep_UserDB.getInstance().UseFirstTabToDisplayAllProtocolsList;
            ShowTabsOrder();

            if (bDisableAllProtocol)
                DisableTabAllProtocols();

            evHandleItemSelectionChanged = new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listView_UserProtocols_ItemSelectionChanged);

             // update the buttons and listview
            if (tabMyProtocols == HorizontalTabs.tabs.Tab1)
            {
                UpdateProtocolOperatorButtons(tabMyProtocols);
                for (int pNum = 0; pNum < myWorkingProtocolsList.Count; pNum++)
                {
                    // Sunny to do 
                    ListViewItem lvItem = new ListViewItem();
                    lvItem.Tag = myWorkingProtocolsList[pNum];
                    lvItem.Text = DisplayProtocolsAbbv ? myWorkingProtocolsList[pNum].Protocol_Name_Abbv : myWorkingProtocolsList[pNum].Protocol_Name;
                    listView_UserProtocols.Items.Add(lvItem);
                }
            }
            else if (tabAllProtocols == HorizontalTabs.tabs.Tab1)
            {
                UpdateProtocolOperatorButtons(tabAllProtocols);
                for (int pNum = 0; pNum < listAllProtocols.Count; pNum++)
                {
                    // Sunny to do 
                    ListViewItem lvItem = new ListViewItem();
                    lvItem.Tag = listAllProtocols[pNum];
                    lvItem.Text = DisplayProtocolsAbbv ? listAllProtocols[pNum].Protocol_Name_Abbv : listAllProtocols[pNum].Protocol_Name;
                    listView_UserProtocols.Items.Add(lvItem);
                }
            }
            else
            {
                // do nothing
            }

            // Resize last column, make allowances for up to 4 quadrants
            listView_UserProtocols.ResizeColumnWidth(Column1, textFormat, (listView_UserProtocols.Width + 4 * IMAGELIST_WIDTH));

            this.scrollIndicator1.LargeChange = listView_UserProtocols.VisibleRow - 1;
            listView_UserProtocols.VScrollbar = this.scrollIndicator1;

            listView_UserProtocols.ResizeVerticalHeight(true);
            Rectangle rcScrollbar = this.scrollIndicator1.Bounds;
            this.scrollIndicator1.SetBounds(rcScrollbar.X, listView_UserProtocols.Bounds.Y, rcScrollbar.Width, listView_UserProtocols.Bounds.Height);

            listView_UserProtocols.UpdateScrollbar();

            this.listView_UserProtocols.ItemSelectionChanged += evHandleItemSelectionChanged;

            rcScrollIndicator = scrollIndicator1.Bounds;
            rcListViewProtocols = listView_UserProtocols.Bounds;
            SpacingBetweenLVAndIndicator = rcScrollIndicator.X - (rcListViewProtocols.X + rcListViewProtocols.Width);

            int nCount = listView_UserProtocols.Columns.Count;
            ListViewLastColumnWidth = listView_UserProtocols.Columns[nCount - 1].Width;


            // attach select protocol event click
            evhSelectProtocolClick = new EventHandler(this.btn_SelectProtocol_Click);
            this.btn_SelectProtocol.Click += evhSelectProtocolClick;

            UpdateScrollIndicatorVisibility();
        }

        private void UpdateScrollIndicatorVisibility()
        {
            int nScrollIndicatorWidth = rcScrollIndicator.Width;
            bool bIndicatorVisible = true;

            // Do not show the scroll indicator if it is not needed
            if (listView_UserProtocols.Items.Count > listView_UserProtocols.VERTICAL_PAGE_SIZE)
            {
                listView_UserProtocols.SetBounds(rcListViewProtocols.X, rcListViewProtocols.Y, rcListViewProtocols.Width, rcListViewProtocols.Height);
                int nCount = listView_UserProtocols.Columns.Count;
                listView_UserProtocols.Columns[nCount - 1].Width = ListViewLastColumnWidth;
                scrollIndicator1.Visible = true;
            }
            else
                bIndicatorVisible = false;

            if (!bIndicatorVisible)
            {
                listView_UserProtocols.SetBounds(rcListViewProtocols.X, rcListViewProtocols.Y, rcListViewProtocols.Width + nScrollIndicatorWidth + SpacingBetweenLVAndIndicator, rcListViewProtocols.Height);
                int nCount = listView_UserProtocols.Columns.Count;
                listView_UserProtocols.Columns[nCount - 1].Width = ListViewLastColumnWidth + nScrollIndicatorWidth + SpacingBetweenLVAndIndicator;
                scrollIndicator1.Visible = false;
            }
        }

        private void SetUpScrollIndicatorThumbs()
        {
            Size s = new Size(scrollIndicator1.Width/2, thumbSize);
            GraphicsPath gpMarker = new GraphicsPath();
            PointF[] markerPoly = new PointF[4];
            markerPoly[0] = new PointF(0.0F, s.Height/2);
            markerPoly[1] = new PointF(s.Width, 0.00F);
            markerPoly[2] = new PointF(s.Width/2, s.Height/2);
            markerPoly[3] = new PointF(s.Width, s.Height);
            gpMarker.AddPolygon(markerPoly);
            scrollIndicator1.ThumbCustomShape = gpMarker;
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

        private void LoadUserProtocols()
        {
            string strUserName = RoboSep_UserConsole.strCurrentUser;

            // is user user preset user?
            CurrentUserIsPresetUser = RoboSep_UserDB.getInstance().IsPresetUser(strUserName);

            // load user protocols
            List<RoboSep_Protocol> tempList = RoboSep_UserDB.getInstance().loadUserProtocols(strUserName);

            myWorkingProtocolsList.Clear();
            myMasterProtocolsList.Clear();
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

                    myMasterProtocolsList.Add(listAllProtocols[nIndex]);
                    myWorkingProtocolsList.Add(listAllProtocols[nIndex]);
                }
            }
        }

        private void CreateImageListGraphics()
        {
            Bitmap bmUserProtocolChosen = new Bitmap(Properties.Resources.GE_BTN09S_added);
            Bitmap bmCheck = new Bitmap(Properties.Resources.GE_BTN07S_select_CLICK);
            Bitmap bmQuadrantRequired = new Bitmap(global::GUI_Controls.Properties.Resources.QuadrantNumber_DISABLED);
            Bitmap bmQuadrantUnselected = new Bitmap(global::GUI_Controls.Properties.Resources.QuadrantNumber_UNSELECTED);
            Bitmap bmQuadrantSelected = new Bitmap(global::GUI_Controls.Properties.Resources.QuadrantNumber_SELECTED);

            Color textColour = Txt_Color;
            Font textFont = listView_UserProtocols.Font;
            StringFormat textFormat = new StringFormat();
            textFormat.Alignment = StringAlignment.Center;
            textFormat.LineAlignment = StringAlignment.Center;

            // matching the font and text color from the run sample screen
            Button_Quadrant2 btnQuad = new Button_Quadrant2();
            if (btnQuad != null)
            {
                textFont = btnQuad.Font;
                textColour = btnQuad.ForeColor;
            }
            imageList1.ImageSize = new System.Drawing.Size(IMAGELIST_WIDTH, IMAGELIST_HEIGHT);
            imageList1.Images.Clear();
            this.imageList1.Images.Add(bmUserProtocolChosen);
            this.imageList1.Images.Add(bmCheck);
            this.imageList1.Images.Add(bmQuadrantRequired);

            Bitmap bm1, bm2, bm3, bm4;
            for (int i = 0; i <= MaxQuadrantNumberIndex; i++)
            {
                int number = i + 1;

                bm1 = new Bitmap(bmQuadrantSelected);
                bm2 = new Bitmap(bmQuadrantUnselected);
                Graphics graphicsSelected = Graphics.FromImage(bm1);
                Graphics graphicsUnselected = Graphics.FromImage(bm2);
                SolidBrush theBrush1 = new SolidBrush(textColour);
                SolidBrush theBrush2 = new SolidBrush(textColour);                
                graphicsSelected.DrawString(number.ToString(), textFont, theBrush1,
                      new Rectangle(0, 0, IMAGELIST_WIDTH, IMAGELIST_HEIGHT), textFormat);
                graphicsUnselected.DrawString(number.ToString(), textFont, theBrush2,
                     new Rectangle(0, 0, IMAGELIST_WIDTH, IMAGELIST_HEIGHT), textFormat);
 
                // draw cancelled marker
                Utilities.DrawCancelledMarker(IMAGELIST_WIDTH, IMAGELIST_HEIGHT, graphicsUnselected);
                string quadrant = string.Format("Q{0}", number);
                string quadrantMarkedAsCancelled = string.Format("Q{0}_MarkCancelled", number);
                bm3 = new Bitmap( bm1 );
                bm4 = new Bitmap( bm2 );
                this.imageList1.Images.Add(quadrant, bm3);
                this.imageList1.Images.Add(quadrantMarkedAsCancelled, bm4);
                var dummy = imageList1.Handle;  //This is dumb, but it has to be here.
                bm1.Dispose();
                bm2.Dispose();
                bm3.Dispose();
                bm4.Dispose();
                theBrush1.Dispose();
                theBrush2.Dispose();
                graphicsSelected.Dispose();
                graphicsUnselected.Dispose();
            }
            bmUserProtocolChosen.Dispose();
            bmCheck.Dispose();
            bmQuadrantRequired.Dispose();
            bmQuadrantUnselected.Dispose();
            bmQuadrantSelected.Dispose();

        }

        public void getFilters()
        {
            protocol_SearchBar2.getFilters(out search);
        }

        public void clearFilters()
        {
            protocol_SearchBar2.clearFilters();
        }

        private void horizontalTabs1_Tab1_Click(object sender, EventArgs e)
        {
            if (horizontalTabs1.TabActive == HorizontalTabs.tabs.Tab1)
                return;

            horizontalTabs1.TabActive = HorizontalTabs.tabs.Tab1;

            // clear filter when swtiching tabs
            clearFilters();

            UpdateDisplayList();

            listView_UserProtocols.UpdateScrollbar();
        }

        private void horizontalTabs1_Tab2_Click(object sender, EventArgs e)
        {
            if (horizontalTabs1.TabActive == HorizontalTabs.tabs.Tab2)
                return;

            horizontalTabs1.TabActive = HorizontalTabs.tabs.Tab2;

            // clear filter when swtiching tabs
            clearFilters();

            UpdateDisplayList();

            listView_UserProtocols.UpdateScrollbar();
        }


        private void UpdateDisplayList()
        {
            if (tabMyProtocols == horizontalTabs1.TabActive)
            {
                StoreChosenProtocolsWhenSwitchingTabs(listChosenProtocolsFromAllProtocols);
                UpdateProtocolOperatorButtons(tabMyProtocols);
                RefreshProtocolDisplayList(myWorkingProtocolsList, myFilteredList);
                RestoreChosenProtocolsAfterSwitchingTabs(listChosenProtocolsFromMyProtocols);
            }
            else if (tabAllProtocols == horizontalTabs1.TabActive)
            {
                StoreChosenProtocolsWhenSwitchingTabs(listChosenProtocolsFromMyProtocols);
                UpdateProtocolOperatorButtons(tabAllProtocols);
                RefreshProtocolDisplayList(listAllProtocols, listAllProtocolsFiltered);
                RestoreChosenProtocolsAfterSwitchingTabs(listChosenProtocolsFromAllProtocols);
            }
            else
            {
                // do nothing
            }

            string txtSearch = LanguageINI.GetString("lblSearch");
            string txt = protocol_SearchBar2.textBox_search.Text.Trim();
            if (!string.IsNullOrEmpty(txt) && txt != txtSearch)
            {
                this.protocol_SearchBar2.textBox_search.TextChanged -= evhHandleSearchBoxTextChanged;
                protocol_SearchBar2.textBox_search.Text = txtSearch;
                this.protocol_SearchBar2.textBox_search.TextChanged += evhHandleSearchBoxTextChanged;
            }

            UpdateScrollIndicatorVisibility();

            listView_UserProtocols.UpdateScrollbar();
        }

        private void StoreChosenProtocolsWhenSwitchingTabs(List<string>listProtocols)
        {
            if (listProtocols == null)
                return;

            listProtocols.Clear();

            for (int i = 0; i < listView_UserProtocols.Items.Count; i++)
            {
                if (listView_UserProtocols.Items[i].Checked == false)
                    continue;

                RoboSep_Protocol tempProtocol = listView_UserProtocols.Items[i].Tag as RoboSep_Protocol;
                if (tempProtocol == null)
                {
                    continue;
                }
                listProtocols.Add(tempProtocol.Protocol_Name);
            }
        }

        private void RestoreChosenProtocolsAfterSwitchingTabs(List<string> listChosenProtocols)
        {
            if (listView_UserProtocols.Items.Count == 0 || listChosenProtocols == null || listChosenProtocols.Count == 0)
                return;

            RoboSep_Protocol pRoboSepProtocol = null;
            int index = 0;
            foreach (ListViewItem lvItem in listView_UserProtocols.Items)
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

        private void ShowTabsOrder()
        {
            tabAllProtocols = UseFirstTabToDisplayAllProtocolsList ? HorizontalTabs.tabs.Tab1 : HorizontalTabs.tabs.Tab2; ;
            tabMyProtocols = tabAllProtocols == HorizontalTabs.tabs.Tab1 ? HorizontalTabs.tabs.Tab2 : HorizontalTabs.tabs.Tab1;

            horizontalTabs1.Tab1 = tabAllProtocols == HorizontalTabs.tabs.Tab1 ? TextAllProtocols : TextMyProtocols;
            horizontalTabs1.Tab2 = horizontalTabs1.Tab1 == TextAllProtocols ? TextMyProtocols : TextAllProtocols;
        }

        private void UpdateProtocolOperatorButtons( HorizontalTabs.tabs tab)
        {
            if (tabMyProtocols == tab)
            {
                btn_AddProtocols.Bounds = rcBtnAddProtocols;
                btn_SelectProtocol.Visible = true;
                btn_home2.Visible = true;
                btn_AddProtocols.Visible = false;

                if (bDisableAllProtocol)
                {
                    btn_RemoveProtocols.Visible = false;
                    listView_UserProtocols.MultiSelect = false;
                }
                else
                {
                    if (runSamplesLatestSelectedProtocols != null && runSamplesLatestSelectedProtocols.Length > 0)
                    {
                        btn_RemoveProtocols.Visible = false;
                        listView_UserProtocols.MultiSelect = false;
                    }
                    else
                    {
                        btn_RemoveProtocols.Visible = CurrentUserIsPresetUser ? false : true;
                        listView_UserProtocols.MultiSelect = true;
                    }
                }
            }
            else if (tabAllProtocols == tab)
            {
                ShowTabsOrder();

                btn_AddProtocols.Bounds = rcBtnRemoveProtocols;
                btn_SelectProtocol.Visible = false;
                btn_AddProtocols.Visible = CurrentUserIsPresetUser? false : true;
                btn_RemoveProtocols.Visible = false;

                btn_home2.Visible = false;
                listView_UserProtocols.MultiSelect = true;
            }
        }

        private void DisableTabAllProtocolsIfRequired()
        {
           if ((runSamplesLatestSelectedProtocols != null && runSamplesLatestSelectedProtocols.Length > 0) || CurrentUserIsPresetUser)
            {
                DisableTabAllProtocols();
            }
        }

        private void DisableTabAllProtocols()
        {
            tabMyProtocols = HorizontalTabs.tabs.Tab1;
            tabAllProtocols = HorizontalTabs.tabs.Tab2;
            horizontalTabs1.Tab1 = TextMyProtocols;
            horizontalTabs1.Tab2 = "";
            btn_RemoveProtocols.Visible = false;
            btn_AddProtocols.Visible = false;
        }

        private void RefreshProtocolDisplayList(List<RoboSep_Protocol> protocolList, List<RoboSep_Protocol> protocolListFiltered)
        {
            if (protocolList == null || protocolListFiltered == null)
                return;
    
            // get currently selected item
            string selectedProtocolName = "";
            if (listView_UserProtocols.SelectedItems.Count > 0)
            {
                RoboSep_Protocol tempProtocol = (RoboSep_Protocol)listView_UserProtocols.SelectedItems[0].Tag;
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

            if (!string.IsNullOrEmpty(selectedProtocolName) && listView_UserProtocols.Items.Count > 0)
            {
                RoboSep_Protocol tempProtocol = null;

                foreach (ListViewItem lvItem in listView_UserProtocols.Items)
                {
                    // ensure the previous selected item is visible 
                    if (lvItem.Tag != null)
                    {
                        tempProtocol = (RoboSep_Protocol)lvItem.Tag;
                        if (tempProtocol.Protocol_Name.Equals(selectedProtocolName))
                        {
                            // Select the previous selected protocol
                            lvItem.Selected = true;
                            listView_UserProtocols.EnsureVisible(lvItem.Index);
                            break;
                        }
                    }
                }
            }
        }

        //UpdateFilters
        private void UpdateFilters(object sender, EventArgs e)
        {
            List<RoboSep_Protocol> longList = null;
            List<RoboSep_Protocol> filteredList = null;
            if (horizontalTabs1.TabActive == tabAllProtocols)
            {
                longList = listAllProtocols;
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

                listView_UserProtocols.UpdateScrollbar();
            }
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
                    int searchResult = searchForTextInString(search, DisplayProtocolsAbbv? tempList[i].Protocol_Name_Abbv : tempList[i].Protocol_Name);
                    if (searchResult != -1)
                    {
                        protocolListFiltered.Add(tempList[i]);
                    }
                }
            }
        }

        public void refreshList(List<RoboSep_Protocol> displayProtocolList)
        {
            if (displayProtocolList == null)
                return;

            listView_UserProtocols.SuspendLayout();

            if (displayProtocolList.Count > 0)
            {
                // determine which protocol name to display
                getFilters();

                // show un-filtered name
                listView_UserProtocols.Items.Clear();
                for (int i = 0; i < displayProtocolList.Count; i++)
                {
                    ListViewItem lvItem = new ListViewItem();
                    lvItem.Tag = displayProtocolList[i];
                    lvItem.Text = DisplayProtocolsAbbv ? displayProtocolList[i].Protocol_Name_Abbv : displayProtocolList[i].Protocol_Name;
                    listView_UserProtocols.Items.Add(lvItem);
                }
            }
            else
            {
                listView_UserProtocols.Items.Clear();
            }

            // Resize last column
            listView_UserProtocols.ResizeColumnWidth(Column1, textFormat, listView_UserProtocols.Width);
            listView_UserProtocols.ResumeLayout(true);
        }

        private static int searchForTextInString(string search, string theString)
        {
            for (int i = 0; i < theString.Length; i++)
            {
                if (theString[i].ToString().ToUpper() == search[0].ToString().ToUpper())
                {
                    //string temp = string.Empty;
                    System.Text.StringBuilder tempString = new System.Text.StringBuilder();
                    for (int j = 0; j < search.Length; j++)
                    {
                        if ((i + j) < theString.Length)
                            //temp += theString[i + j];
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

        private void btn_AddProtocols_Click(object sender, EventArgs e)
        {
            // Check the list if it is all protocol or my protocol
            if (horizontalTabs1.TabActive == tabMyProtocols)
            {
                horizontalTabs1.TabActive = tabAllProtocols;
                UpdateProtocolOperatorButtons(tabAllProtocols);
                RefreshProtocolDisplayList(listAllProtocols, listAllProtocolsFiltered);
                return;
            }
            else if (horizontalTabs1.TabActive == tabAllProtocols)
            {
                if (listView_UserProtocols.Items.Count == 0)
                {
                    return;
                }
                int nCount = 0;
                for (int i = 0; i < listView_UserProtocols.Items.Count; i++)
                {
                    if (listView_UserProtocols.Items[i].Checked == false)
                        continue;

                    RoboSep_Protocol tempProtocol = listView_UserProtocols.Items[i].Tag as RoboSep_Protocol;
                    if (tempProtocol == null)
                    {
                        continue;
                    }
                    AddProtocolToUserList2(tempProtocol);
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
                    RefreshProtocolDisplayList(listAllProtocols, listAllProtocolsFiltered);
                }
            }

            UpdateScrollIndicatorVisibility();

            // Show some visual effects
            string title = LanguageINI.GetString("headerUpdatingInProgress");
            RoboMessagePanel4 prompt = new GUI_Controls.RoboMessagePanel4(RoboSep_UserConsole.getInstance(), title, 500, 20);
            RoboSep_UserConsole.showOverlay();
            prompt.ShowDialog();
            RoboSep_UserConsole.hideOverlay();
            prompt.Dispose();                
        }

        private void btn_RemoveProtocols_Click(object sender, EventArgs e)
        {
            if (listView_UserProtocols.Items.Count > 0)
            {
                RoboSep_Protocol tempProtocol = null;
                if (horizontalTabs1.TabActive == tabMyProtocols)
                {
                    for (int i = 0; i < listView_UserProtocols.Items.Count; i++)
                    {
                        if (listView_UserProtocols.Items[i].Checked == false)
                            continue;

                        tempProtocol = listView_UserProtocols.Items[i].Tag as RoboSep_Protocol;
                        if (tempProtocol == null)
                        {
                            continue;
                        }

                        RemoveProtocolFromUserList2(tempProtocol);
                    }

                    RefreshProtocolDisplayList(myWorkingProtocolsList, myFilteredList);
                }
            }

            UpdateScrollIndicatorVisibility();

            // Show some visual effects
            string title = LanguageINI.GetString("headerUpdatingInProgress");
            RoboMessagePanel4 prompt = new GUI_Controls.RoboMessagePanel4(RoboSep_UserConsole.getInstance(), title, 500, 20);
            RoboSep_UserConsole.showOverlay();
            prompt.ShowDialog();
            RoboSep_UserConsole.hideOverlay();
            prompt.Dispose();                
        }

        private void listView_UserProtocols_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            RoboSep_Protocol tempProtocol = e.Item.Tag as RoboSep_Protocol;
            if (tempProtocol == null)
                return;

            if (listView_UserProtocols.MultiSelect == false)
            {
                e.Item.Checked = e.Item.Selected ? true : false;
            }

            listView_UserProtocols.UpdateScrollbar();
        }

        private void listView_UserProtocols_Click(object sender, EventArgs e)
        {
            if (listView_UserProtocols.SelectedItems.Count == 0)
                return;

            ListViewItem lvItem = listView_UserProtocols.SelectedItems[0];
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
            }

            if (listView_UserProtocols.MultiSelect)
            {
                if (lvItem.Selected)
                {
                    lvItem.Checked = !lvItem.Checked;
                }
                else if (!lvItem.Selected && !lvItem.Checked)
                {
                    lvItem.Checked = true;
                }
            }

            // Hide the play button if there are more than one item checked in my protocol list
            if (horizontalTabs1.TabActive == tabMyProtocols)
            {
                btn_SelectProtocol.Visible = true;
                int nIndex = 0;
                int nCount = 0;
                foreach (ListViewItem item in listView_UserProtocols.Items)
                {
                    if (item.Checked)
                    {
                        nIndex = item.Index;
                        if (++nCount > 1)
                        {
                            btn_SelectProtocol.Visible = false;
                            break;
                        }
                    }
                }
                // Verify that the highlighted and checked item are the same item
                if (nCount == 1)
                {
                    lvItem = listView_UserProtocols.SelectedItems[0];
                    if (lvItem != null && lvItem.Index != nIndex)
                    {
                        btn_SelectProtocol.Visible = false;
                    }
                }
            }
        }

        private void BackToRunSamplesScreen()
        {
            RoboSep_RunSamples SamplingWindow = RoboSep_RunSamples.getInstance();
            RoboSep_RunSamples.getInstance().Visible = true;
            SamplingWindow.Location = new Point(0, 0);
            this.Visible = false;
            RoboSep_UserConsole.getInstance().Controls.Add(SamplingWindow);
            RoboSep_UserConsole.getInstance().Controls.Remove(RoboSep_UserConsole.ctrlCurrentUserControl);
            RoboSep_UserConsole.ctrlCurrentUserControl = SamplingWindow;
            SamplingWindow.BringToFront();
        }

        private void SelectProtocolAction()
        {
            // check if an item is selected in List view
            if (listView_UserProtocols.SelectedItems.Count == 0)
                return;

            RoboSep_Protocol tempProtocol = (RoboSep_Protocol)listView_UserProtocols.SelectedItems[0].Tag;
            string theProtocol = (tempProtocol != null) ? tempProtocol.Protocol_Name : listView_UserProtocols.SelectedItems[0].Text;

            //int Qcount = Convert.ToInt32(listView_UserProtocols.SelectedItems[0].SubItems[1].Text);
            int Qcount = (tempProtocol != null) ? tempProtocol.numQuadrants : 1;
                SelectProtocol(theProtocol, Qcount);

            // LOG
            string LOGmsg = "Protocol Selected from User list";
            //GUI_Controls.uiLog.LOG(this, "button_Accept_Click", GUI_Controls.uiLog.LogLevel.EVENTS, LOGmsg);
            //  (LOGmsg);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);                
        }

        private QuadrantInfo[] RunSamples_CurrentProtocols;

        private void RemoveCurrentProtocolsEx()
        {
            QuadrantInfo[] quadInfos = new QuadrantInfo[4];
            GetRunSamplesQuadrantInfos(ref quadInfos);

            List<string> lstProtocolsToBeUnselected = new List<string>();
            string QuadrantLabel;
            for (int Q = 0; Q < quadInfos.Length; Q++)
            {
                QuadrantLabel = quadInfos[Q].QuadrantLabel;
                if (string.IsNullOrEmpty(QuadrantLabel))
                    continue;

                bool bFound = false;
                foreach (RoboSep_Protocol item in myWorkingProtocolsList) 
                {
                    if (string.IsNullOrEmpty(item.Protocol_Name))
                        continue;

                    if (item.Protocol_Name == QuadrantLabel)
                    {
                        bFound = true;
                        break;
                    }
                }

                if (!bFound)
                {
                    string temp = lstProtocolsToBeUnselected.Find(x => { return (!string.IsNullOrEmpty(x) && x.Length == QuadrantLabel.Length && x.ToLower() == (QuadrantLabel.ToLower())); });
                    if (string.IsNullOrEmpty(temp))
                        lstProtocolsToBeUnselected.Add(QuadrantLabel);
                }
            }
         
            RemoveCurrentProtocols();

            if (0 < lstProtocolsToBeUnselected.Count && RunSamples_CurrentProtocols != null)
            {
                for (int i = 0; i < lstProtocolsToBeUnselected.Count; i++)
                {
                    if (string.IsNullOrEmpty(lstProtocolsToBeUnselected[i]))
                        continue;

                    // deselect previously selected protocols
                    for (int Q = 0; Q < RunSamples_CurrentProtocols.Length; Q++)
                    {
                        QuadrantLabel = RunSamples_CurrentProtocols[Q].QuadrantLabel;
                        if (!string.IsNullOrEmpty(QuadrantLabel) && QuadrantLabel.ToLower() == lstProtocolsToBeUnselected[i].ToLower())
                        {
                            RunSamples_CurrentProtocols[Q].Clear();

                            // continue to remove the selected protocols if the protocol happened to be in more than one quadrants.
                            // no break out of the loop
                        }
                    }

                    // remove previous selected protocols from the list of selected protocols
                    if (runSamplesLatestSelectedProtocols != null && runSamplesLatestSelectedProtocols.Length > 0)
                    {
                        List<ProtocolInfo> lst = new List<ProtocolInfo>();
                        lst.AddRange(runSamplesLatestSelectedProtocols);
                        lst.RemoveAll(x => { return (!string.IsNullOrEmpty(x.ProtocolName) && x.ProtocolName.ToLower() == lstProtocolsToBeUnselected[i].ToLower()); });
                        runSamplesLatestSelectedProtocols = lst.ToArray();
                    }
                 }
            }
        }

        private void GetRunSamplesQuadrantInfos(ref QuadrantInfo[] quadInfo)
        {
            if (quadInfo == null)
                return;

            // get status of current Run
            quadInfo[0] = new QuadrantInfo(RoboSep_RunSamples.getInstance().RunInfo[0]);
            quadInfo[1] = new QuadrantInfo(RoboSep_RunSamples.getInstance().RunInfo[1]);
            quadInfo[2] = new QuadrantInfo(RoboSep_RunSamples.getInstance().RunInfo[2]);
            quadInfo[3] = new QuadrantInfo(RoboSep_RunSamples.getInstance().RunInfo[3]);
        }

        private void RemoveCurrentProtocols()
        {
            // get status of current Run
            RunSamples_CurrentProtocols = new QuadrantInfo[4];
            GetRunSamplesQuadrantInfos(ref RunSamples_CurrentProtocols);
                
            // cancel current protocols
            for (int i = 0; i < RunSamples_CurrentProtocols.Length; i++)
            {
                if (RunSamples_CurrentProtocols[i].QuadrantLabel.Length > 1)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("Deselect protocol {0} from quadrant {1}", RunSamples_CurrentProtocols[i].QuadrantLabel,  i + 1));
                    RoboSep_RunSamples.getInstance().CancelQuadrant(i);
                }
            }
        }

        private void RestoreMyProtocols()
        {
            eClosingState = ClosingState.eRestoringUserProtocols;

            if (runSamplesLatestSelectedProtocols == null || runSamplesLatestSelectedProtocols.Length == 0)
            {
                eClosingState = ClosingState.eRestoringUserProtocolsOK;

                // Sunny to do
                // Log message
                string LOGmsg = "Restore previous protocols: current protocol parameter is null or count = 0.";
                //  (LOGmsg);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);                
                return;
            }

            System.Diagnostics.Debug.WriteLine(String.Format("---------Restoring my previously selected User protocols ---------"));

            lock (lockObject)
            {
                // eClosingState = ClosingState.eRestoringUserProtocolsOK;
                RoboSep_RunSamples.getInstance().LoadProtocols(runSamplesLatestSelectedProtocols);

                if (runSamplesLatestSelectedProtocols.Length > 0)
                {
                    if (evhHandleRestoreProtocolsFinished == null)
                        evhHandleRestoreProtocolsFinished = new EventHandler(HandleRestoreProtocolsFinished);
                    RoboSep_RunSamples.getInstance().NotifyUserRestoreProtocolsFinished += evhHandleRestoreProtocolsFinished;
                }
                System.Diagnostics.Debug.WriteLine(String.Format("Restoring my previously selected User protocols completed. eClosingState = {0}", eClosingState));
              }
        }

        private void SelectProtocol(string theSelectedProtocol, int quadrants)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("---------Protocol Select: SelectProtocol called."));


            // Check if there are enough quadrants available to accomodate protocol
            if (quadrants <= QuadrantsAvailable && ((int)CurrentQuadrant + quadrants - 1) < 4)
            {
                // switch to Sampling window
                RoboSep_RunSamples.getInstance().Visible = false;
                RoboSep_UserConsole.getInstance().Controls.Add(RoboSep_RunSamples.getInstance());


                System.Diagnostics.Debug.WriteLine(String.Format("----------Protocol Select: Called RoboSep_RunSamples.getInstance().addToRun"));


                playButtonPressed = true;
                listView_UserProtocols.Refresh();


                // update run samples window (gets window)
                RoboSep_RunSamples.getInstance().addToRun((int)CurrentQuadrant, theSelectedProtocol);
                RoboSep_RunSamples.getInstance().Visible = true;
                this.SendToBack();

                RoboSep_UserConsole.getInstance().Controls.Remove(this);
                RoboSep_UserConsole.ctrlCurrentUserControl = RoboSep_RunSamples.getInstance();

                System.Diagnostics.Debug.WriteLine(String.Format("----------Protocol Select: After RoboSep_RunSamples.getInstance().addToRun"));


                // LOG
                string LOGmsg = "Adding protocol to current run";
                //GUI_Controls.uiLog.LOG(this, "SelectProtocol", GUI_Controls.uiLog.LogLevel.INFO, LOGmsg);
                //  (LOGmsg);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);                
            }
            else
            {
                // set up message prompt
                string sMSG = LanguageINI.GetString("msgCarouselSpace");
                string sTitle = LanguageINI.GetString("headerCarouselSpace");
                RoboMessagePanel newPrompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_ERROR,
                    sMSG, sTitle, LanguageINI.GetString("Ok"));
                RoboSep_UserConsole.getInstance().addForm(newPrompt, newPrompt.Offset);
                RoboSep_UserConsole.showOverlay();
                
                // prompt user
                newPrompt.ShowDialog();
                RoboSep_UserConsole.hideOverlay();

                // LOG
                string LOGmsg = "Could not add protocol to run, insufficient space";
                //GUI_Controls.uiLog.LOG(this, "SelectProtocol", GUI_Controls.uiLog.LogLevel.WARNING, LOGmsg);
                newPrompt.Dispose();                
                LogFile.AddMessage(TraceLevel.Warning, LOGmsg);
            }
        }

        private void AddProtocolToUserList2(RoboSep_Protocol ProtocolToBeAdded)
        {
            if (ProtocolToBeAdded == null || string.IsNullOrEmpty(ProtocolToBeAdded.Protocol_FileName) || string.IsNullOrEmpty(ProtocolToBeAdded.Protocol_Name))
                return;

            // check for duplicates 
            for (int i = 0; i < myWorkingProtocolsList.Count; i++)
            {
                if (ProtocolToBeAdded.Protocol_Name == myWorkingProtocolsList[i].Protocol_Name)
                {
                    // LOG
                    string LOGmsg = string.Format("Protocol {0} already in the user list. Do not add this protocol to the list.", ProtocolToBeAdded.Protocol_Name);
                    //  (LOGmsg);
                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);                
                    return;
                }
             }

            System.Diagnostics.Debug.WriteLine(String.Format("---------Add protocol {0} to my protocol list.", ProtocolToBeAdded.Protocol_Name));

            // No duplicates
            myWorkingProtocolsList.Add(ProtocolToBeAdded);
            isDirty = true;
        }

        private void RemoveProtocolFromUserList(RoboSep_Protocol ProtocolToBeRemoved)
        {
            if (ProtocolToBeRemoved == null || string.IsNullOrEmpty(ProtocolToBeRemoved.Protocol_FileName) || string.IsNullOrEmpty(ProtocolToBeRemoved.Protocol_Name))
                return;

            bool bContinue = false;
            for (int i = 0; i < myWorkingProtocolsList.Count; i++)
            {
                // Remove the protocol from my protocol list 
                if (ProtocolToBeRemoved.Protocol_Name == myWorkingProtocolsList[i].Protocol_Name)
                {
                    myWorkingProtocolsList.RemoveAt(i);
                    isDirty = true;
                    bContinue = true;
                    break;
                }
            }

            // Proceed to remove the protocol if it has been found. If not, simply return.
            if (bContinue == false)
            {
                // LOG
                string LOGmsg = string.Format("Do not find protocol {0} in the user list. Fail to remove this protocol from the user list.", ProtocolToBeRemoved.Protocol_Name);
                //  (LOGmsg);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);                
                return;
            }

            RefreshProtocolDisplayList(myWorkingProtocolsList, myFilteredList);
        }

        private void RemoveProtocolFromUserList2(RoboSep_Protocol ProtocolToBeRemoved)
        {
            if (ProtocolToBeRemoved == null || string.IsNullOrEmpty(ProtocolToBeRemoved.Protocol_Name))
                return;

            for (int i = 0; i < myWorkingProtocolsList.Count; i++)
            {
                // Remove the protocol from my protocol list 
                if (ProtocolToBeRemoved.Protocol_Name == myWorkingProtocolsList[i].Protocol_Name)
                {
                    myWorkingProtocolsList.RemoveAt(i);
                    isDirty = true;
                    break;
                }
            }
        }


        private int loadState = 0;
        private int sleepCounter = 0;
        private void LoadProtocolTimer_Tick(object sender, EventArgs e)
        {
            if (loadState == 0 && RoboSep_UserConsole.getInstance().bLoadingProtocols)
            {
                loadState++;
            }
            if (loadState == 1 && RoboSep_UserConsole.getInstance().bLoadingProtocols)
            {
                int currentlyLoaded = RoboSep_UserConsole.getInstance().ProtocolsLoaded;
            }
            else if (loadState == 1 && !RoboSep_UserConsole.getInstance().bLoadingProtocols)
            {
                // check if server list is updated
                // server delegate function will update
                // protocolUpdated value when completed
                if (!mySeparatorGateway.protocolUpdated)
                {
                    //Thread.Sleep(100);
                    sleepCounter++;
                    System.Diagnostics.Debug.WriteLine(string.Format("Sleep Counter {0}", sleepCounter));
                }
                // peform completion test
                // check if given list matches
                // list returned from server
                else
                {
                    mySeparatorGateway.protocolUpdated = false;
                    sleepCounter = 0;
                    Thread.Sleep(500);

                    // get List of protocols loaded to server
                    // to see if selected protocol has been added
                    /*
                    Tesla.Common.Protocol.ISeparationProtocol[] serverList = RoboSep_UserConsole.getInstance().XML_getUserProtocols();
                    
                    // find protocol in array
                    bool ServerUpdated = false;
                    for (int i = 0; i < serverList.Length; i++)
                    {
                        if (serverList[i].Label == protocolSelection.Protocol_Name)
                        {
                            ServerUpdated = true;
                            break;
                        }
                    }
                     */

                    if (/*ServerUpdated*/ true || reloadProtocolCount > 1)
                    {
                        // set separatorUpdating back to false (
                        mySeparatorGateway.separatorUpdating = false;

                        // replace protocols previously removed
                        RestoreMyProtocols();

                        // close loading dialog
                        loading.Close();
                        loading = null;
                        RoboSep_UserConsole.hideOverlay();

                        // reset count
                        reloadProtocolCount = 0;
                        LoadProtocolTimer.Stop();
                        loadState = 0;
                        reloadMaximum++;


                        if (protocolSelection != null)
                        {
                            SelectProtocol(protocolSelection.Protocol_Name, protocolSelection.numQuadrants);
                        }
                        else
                        {
                            // Sunny to do
                            // Log error
                        }

                        //    return;
                        //}
                        //else if (reloadMaximum == 3)
                        //{
                        // re-attempt to add protocol
                        //AddProtocolToUserList(protocolSelection.Protocol_FileName);
                        //    SelectProtocol(protocolSelection.Protocol_Name, protocolSelection.numQuadrants);
                        //}
                        //else
                        //    reloadMaximum = 0;
                        //LoadProtocolTimer.Stop();
                    }
                    else
                    {
                        reloadProtocolCount++;
                        LoadProtocolsToServer();
                        LoadProtocolTimer.Start();
                    }
                }
            }
        }

        private void HandleRestoreProtocolsFinished(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("--------- HandleRestoreProtocolsFinished called --------------"));

            RoboSep_RunSamples.getInstance().NotifyUserRestoreProtocolsFinished -= evhHandleRestoreProtocolsFinished;

            // close loading dialog
            loading.Close();
            loading = null;

            RoboSep_UserConsole.hideOverlay();

            if (bSelectProtocolBeforeReturningToHomeScreen == true)
            {
                if (!string.IsNullOrEmpty(protocolToBeSelectedBeforeClosing.ProtocolName) && protocolToBeSelectedBeforeClosing.NumberOfQuadrantRequired > 0)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("--------- HandleRestoreProtocolsFinished: Select the protocol '{0}', quadrant required = {1} from the list before closing. --------------", protocolToBeSelectedBeforeClosing.ProtocolName, (int)protocolToBeSelectedBeforeClosing.NumberOfQuadrantRequired));
                    SelectProtocol(protocolToBeSelectedBeforeClosing.ProtocolName, (int)protocolToBeSelectedBeforeClosing.NumberOfQuadrantRequired);
                    System.Diagnostics.Debug.WriteLine(String.Format("--------- Select protocol '{0}', quadrant {1} completed. --------------", protocolToBeSelectedBeforeClosing.ProtocolName, (int)protocolToBeSelectedBeforeClosing.NumberOfQuadrantRequired));
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("--------- Exit Check point 1.  Back to the Run Samples screen since there is no newly selected protocol ----------"));
                    BackToRunSamplesScreen();
                }
            }
            else if (bReturnToHomeScreen)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("--------- Exit Check point 2. Back to the Run Samples screen since there is no newly selected protocol ----------"));
                BackToRunSamplesScreen();
            }
        }
        
        private void HandleUserProtocolsReloadedBeforeExiting(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("--------- RoboSep_ProtocolSelect: HandleUserProtocolsReloadedBeforeExiting is called. ThreadID = {0}, ----------", Thread.CurrentThread.ManagedThreadId));

           RoboSep_UserConsole.getInstance().NotifyUserSeparationProtocolsUpdated -= evhHandleUserProtocolsReloaded;

            // set separatorUpdating back to false 
            mySeparatorGateway.separatorUpdating = false;

            eClosingState = ClosingState.eLoadingUserProtocolsFinished;
            System.Diagnostics.Debug.WriteLine(String.Format("--------- Start timer to restore previously selected protocols ----------"));
            
            SelectProtocolTimer.Start();
        }

  
        private void SelectProtocolTimer_Tick(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("--------- Enter select protocol timer routine. reload count = {0}. Close state = {1} --------------", reloadUserProtocolsCount, eClosingState));

            SelectProtocolTimer.Stop();

            // server list has been updated
            //    mySeparatorGateway.protocolUpdated = false;

            // set separatorUpdating back to false 
            mySeparatorGateway.separatorUpdating = false;

            if (eClosingState == ClosingState.eLoadingUserProtocolsFinished)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("--------- Reload user protocols --------------"));

                // Reload user list
                ReloadUserProtocols();

                // check if user protocols have been loaded
                eClosingState = ClosingState.eLoadingUserProtocolsOK;
                if (aWorkingProtocolNamesBeforeClosing != null && aWorkingProtocolNamesBeforeClosing.Length > 0)
                {
                    if (IsRequiredToReloadUserProtocols(aWorkingProtocolNamesBeforeClosing) == true)
                    {
                        System.Diagnostics.Debug.WriteLine(String.Format("--------- Reload User protocols --------------"));

                        if (reloadUserProtocolsCount < reloadUserProtocolstMaxCount)
                        {
                            string[] aWorkingProtocolFileNames = null;
                            aWorkingProtocolFileNames = new string[aWorkingProtocolNamesBeforeClosing.Length];

                            RoboSep_UserDB pRoboSep_UserDB = RoboSep_UserDB.getInstance();
                            for (int i = 0; i < aWorkingProtocolNamesBeforeClosing.Length; i++)
                            {
                                aWorkingProtocolFileNames[i] = pRoboSep_UserDB.getProtocolFilename(aWorkingProtocolNamesBeforeClosing[i]);
                            }

                            RoboSep_UserDB.getInstance().XML_SaveUserProfile(RoboSep_UserConsole.strCurrentUser, aWorkingProtocolFileNames);
                            if (evhHandleUserProtocolsReloaded == null)
                                evhHandleUserProtocolsReloaded = new EventHandler(HandleUserProtocolsReloadedBeforeExiting);

                            System.Diagnostics.Debug.WriteLine(String.Format("--------- Load user protocols to server --------------"));
                            LoadProtocolsToServer();
                            SeparatorGateway.GetInstance().separatorUpdating = true;
                            RoboSep_UserConsole.getInstance().NotifyUserSeparationProtocolsUpdated += evhHandleUserProtocolsReloaded;
                            reloadUserProtocolsCount++;
                            eClosingState = ClosingState.eLoadingUserProtocols;
                            return;
                        }
                        else
                        {
                            // log error
                            string LOGmsg = "Failed to reload user protocol list";
                            LogFile.AddMessage(TraceLevel.Warning, LOGmsg);
                            // close loading dialog
                            loading.Close();
                            loading = null;
                            RoboSep_UserConsole.hideOverlay();
                            return;
                        }
                   }

                    System.Diagnostics.Debug.WriteLine(String.Format("--------- User protocols have been loaded --------------"));
                }
            }

            if (eClosingState == ClosingState.eLoadingUserProtocolsOK)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("--------- Restoring previously deleted protocols --------------"));

                // restore previously deleted protocols
                RestoreMyProtocols();
                if (eClosingState != ClosingState.eRestoringUserProtocolsOK)
                {
                    return;
                }
            }
            
            if (eClosingState == ClosingState.eRestoringUserProtocolsOK)
            {
                // close loading dialog
                loading.Close();
                loading = null;
                RoboSep_UserConsole.hideOverlay();

                if (bSelectProtocolBeforeReturningToHomeScreen == true)
                {
                    if (!string.IsNullOrEmpty(protocolToBeSelectedBeforeClosing.ProtocolName) && protocolToBeSelectedBeforeClosing.NumberOfQuadrantRequired > 0)
                    {
                        System.Diagnostics.Debug.WriteLine(String.Format("--------- Select the newly highlighted protocol '{0}', quadrant {1} from the list before closing. --------------", protocolToBeSelectedBeforeClosing.ProtocolName, (int)protocolToBeSelectedBeforeClosing.NumberOfQuadrantRequired));
                        SelectProtocol(protocolToBeSelectedBeforeClosing.ProtocolName, (int)protocolToBeSelectedBeforeClosing.NumberOfQuadrantRequired);
                        System.Diagnostics.Debug.WriteLine(String.Format("--------- Select protocol '{0}', quadrant {1} completed. --------------", protocolToBeSelectedBeforeClosing.ProtocolName, (int)protocolToBeSelectedBeforeClosing.NumberOfQuadrantRequired));
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(String.Format("--------- Exit Check point 1.  Back to the Run Samples screen since there is no newly selected protocol ----------"));
                        BackToRunSamplesScreen();
                    }
                }
                else if (bReturnToHomeScreen)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("--------- Exit Check point 2. Back to the Run Samples screen since there is no newly selected protocol ----------"));
                    BackToRunSamplesScreen();
                }
            }
            
            System.Diagnostics.Debug.WriteLine(String.Format("--------- Exit from Timer routine ----------"));
        }

         private void ReloadUserProtocols()
         {
            string[] usrProtocols;
            ISeparationProtocol[] serverList = RoboSep_UserConsole.getInstance().XML_getUserProtocols();
            usrProtocols = new string[serverList.Length];
            System.Diagnostics.Debug.WriteLine(String.Format("---------Reloading User Protocols from the server: number of protocols being reloaded = {0}  ---------", serverList.Length));

            for (int i = 0; i < serverList.Length; i++)
            {
                usrProtocols[i] = serverList[i].Label;
                System.Diagnostics.Debug.WriteLine(String.Format("---------Item {0} - protocol name: {1} is being reloaded.", i, usrProtocols[i]));
            }

            // get user list
            myWorkingProtocolsList.Clear();
            int nIndex = 0;
            for (int i = 0; i < usrProtocols.Length; i++)
            {
                nIndex = findInList(usrProtocols[i]);
                if (nIndex < 0)
                {
                    continue;
                }
                myWorkingProtocolsList.Add(listAllProtocols[nIndex]);
            }
        }

        private void AddProtocolToUserList(string AddProtocol)
        {
            if (string.IsNullOrEmpty(AddProtocol))
                return;

            // check for duplicates and load user from ini 
            string[] sProtocols = new string[myWorkingProtocolsList.Count + 1];
            for (int i = 0; i < myWorkingProtocolsList.Count; i++)
            {
                if (AddProtocol == myWorkingProtocolsList[i].Protocol_Name)
                {
                    // LOG
                    string LOGmsg = string.Format("Protocol {0} already in the user list. Do not add this protocol to the list.", AddProtocol);
                    //  (LOGmsg);
                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);                
                    return;
                }
                sProtocols[i] = myWorkingProtocolsList[i].Protocol_FileName;
            }
            sProtocols[sProtocols.Length - 1] = AddProtocol;
             
            // show loading protocol message
            string title = LanguageINI.GetString("headerLoadingProtocols");
            string msg = LanguageINI.GetString("LoadingAllProtocols");
            loading = new RoboMessagePanel5(RoboSep_UserConsole.getInstance(), title, msg, GUI_Controls.GifAnimationMode.eUploadingMultipleFiles);
            RoboSep_UserConsole.showOverlay();
            loading.Show();
            try
            {
                // save over user1.udb
                RoboSep_UserDB.getInstance().XML_SaveUserProfile(RoboSep_UserConsole.strCurrentUser, sProtocols);

                LoadProtocolsToServer();
                SeparatorGateway.GetInstance().separatorUpdating = true;
                
                LoadProtocolTimer.Start();
             }
            catch (Exception ex)
            {
                // LOG
                LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);               
            }
        }

        private void LoadProtocolsToServer()
        {
            // Reload protocols with SeparatorGateway using a thread
            myReloadProtocolsThread = new Thread(new ThreadStart(this.ReloadProtocolsThread));
            myReloadProtocolsThread.IsBackground = true;
            myReloadProtocolsThread.Start();
        }

        // function run to refresh server's active user protocol list
        private void ReloadProtocolsThread()
        {
            // save list as old robosep would
            SeparatorGateway.GetInstance().ReloadProtocols();

            //Thread.Sleep(600);

            SeparatorGateway.GetInstance().Connect(false);
        }

        private bool selectAddition( string protocolName )
        {
            // wait until user has been updated
            // check if user has been updated
            
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

        private int findInList(string pString)
        {
            if (string.IsNullOrEmpty(pString))
                return -1;

            int nLastIndex = pString.LastIndexOf('.');
            if (nLastIndex != -1 && ((pString.Length - nLastIndex) == ProtocolFileExtension.Length) && pString.Substring(nLastIndex).ToLower() == ProtocolFileExtension.ToLower())
            {
                // check first if string is filename
                for (int i = 0; i < listAllProtocols.Count; i++)
                {
                    if (pString == listAllProtocols[i].Protocol_FileName)
                        return i;
                }
                return -1;
            }
            else
            {
                getFilters();

                // compare to unfiltered name
                for (int i = 0; i < listAllProtocols.Count; i++)
                {
                    if (pString == listAllProtocols[i].Protocol_Name)
                        return i;
                }
            }
            return -1;
        }

        private void button_SelectionMode_Click(object sender, EventArgs e)
        {
            switchSelectionList();
            
            // LOG
            string msg = DisplayingUserProtocols ? "User Protocols" : "All Protocols";
            string LOGmsg = "Change Protocol selection mode to" + msg;
            //GUI_Controls.uiLog.LOG(this, "button_SelectionMode_Click", GUI_Controls.uiLog.LogLevel.EVENTS, LOGmsg);
            //  (LOGmsg);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);                
        }

        private void switchSelectionList()
        {
            // pre-load user profile prior to
            // attempting to add protocol from
            // protocols list
            /*
            if (false)//(!preLoad)
            {
                // Reload protocols with SeparatorGateway using a thread
                myReloadProtocolsThread = new Thread(new ThreadStart(this.ReloadProtocolsThread));
                myReloadProtocolsThread.IsBackground = true;
                myReloadProtocolsThread.Start();
                preLoad = true;
            }
             * */

            // display user list or all list
            // true = user list
            // false = all protocols
            if (DisplayingUserProtocols)
            {
                listView_UserProtocols.SuspendLayout();
                listView_UserProtocols.Items.Clear();
                for (int pNum = 0; pNum < myWorkingProtocolsList.Count; pNum++)
                {
                    ListViewItem lvItem = new ListViewItem();
                    lvItem.Tag = myWorkingProtocolsList[pNum];
                    lvItem.Text = DisplayProtocolsAbbv ? myWorkingProtocolsList[pNum].Protocol_Name_Abbv : myWorkingProtocolsList[pNum].Protocol_Name;
                    listView_UserProtocols.Items.Add(lvItem);
                }
                listView_UserProtocols.ResumeLayout(true);
            }
            else
            {
                listView_UserProtocols.SuspendLayout();
                updateFilteredList(myWorkingProtocolsList, myFilteredList);
                refreshList(myFilteredList);
                listView_UserProtocols.ResumeLayout(true);
            }

            // show search bar if showing all list
            protocol_SearchBar2.Enabled = !DisplayingUserProtocols;
            protocol_SearchBar2.Visible = !DisplayingUserProtocols;
        }

        private void button_Cancel1_Click(object sender, EventArgs e)
        {
            RoboSep_RunSamples SamplingWindow = RoboSep_RunSamples.getInstance();
            RoboSep_RunSamples.getInstance().Visible = true;
            SamplingWindow.Location = new Point(0, 0);
            SamplingWindow.EnableQuadrantButton((QuadrantId)CurrentQuadrant, false);
            this.Visible = false;
            RoboSep_UserConsole.getInstance().Controls.Add(SamplingWindow);
            RoboSep_UserConsole.getInstance().Controls.Remove(RoboSep_UserConsole.ctrlCurrentUserControl);
            RoboSep_UserConsole.ctrlCurrentUserControl = SamplingWindow;

            // LOG
            string LOGmsg = "Protocol Selection cancel clicked";
            //GUI_Controls.uiLog.LOG(this, "button_Cancel1_Click", GUI_Controls.uiLog.LogLevel.EVENTS, LOGmsg);
            //  (LOGmsg);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);                
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
            //RoboSep_UserConsole.myUserConsole.addForm(newKeyboard.overlay, newKeyboard.overlayOffset);
            newKeyboard.Dispose();
        }

        private void listView_UserProtocols_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
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
            theBrush2.Dispose();
            theBrush1.Dispose();
            GradientBrush.Dispose();
            

        }

        private void listView_UserProtocols_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            Color textColour = Txt_Color;
            Rectangle itemBounds = new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width, e.Bounds.Bottom - e.Bounds.Top);
            if (e.Item.Selected)
            {
                SolidBrush theBrush1 = new SolidBrush(BG_Selected);
                e.Graphics.FillRectangle(theBrush1, itemBounds);
                textColour = Color.White;
                theBrush1.Dispose();
            }
            else if (e.ItemIndex % 2 == 0)
            {
                SolidBrush theBrush2 = new SolidBrush(BG_ColorEven);
                e.Graphics.FillRectangle(theBrush2, itemBounds);
                theBrush2.Dispose();
            }
            else
            {
                SolidBrush theBrush3 = new SolidBrush(BG_ColorOdd);
                e.Graphics.FillRectangle(theBrush3, itemBounds);
                theBrush3.Dispose();
            }

            if (horizontalTabs1.TabActive == tabMyProtocols)
            {
                ListViewItem lvItem = e.Item;
                RoboSep_Protocol protocol = lvItem.Tag as RoboSep_Protocol;
                if (protocol != null && !string.IsNullOrEmpty(protocol.Protocol_Name))
                {
                    itemBounds = DrawQuadrants(protocol.Protocol_Name, protocol.numQuadrants, e.Graphics, itemBounds, e.ItemIndex, e.Item.Selected, e.Item.Checked, ref textColour);
                }
            }
            else if (horizontalTabs1.TabActive == tabAllProtocols)
            {
                ListViewItem lvItem = e.Item;
                RoboSep_Protocol protocol = lvItem.Tag as RoboSep_Protocol;
                if (protocol != null && !string.IsNullOrEmpty(protocol.Protocol_Name))
                {
                    itemBounds = DrawUserProtocolChosenIcons(protocol.Protocol_Name, e.Graphics, itemBounds, e.ItemIndex, e.Item.Selected, e.Item.Checked);
                }
            }
            SolidBrush theBrushFinal = new SolidBrush(textColour);
            e.Graphics.DrawString(e.Item.Text, listView_UserProtocols.Font, theBrushFinal,
                new Rectangle(itemBounds.Left, itemBounds.Top, itemBounds.Width, itemBounds.Bottom - itemBounds.Top), textFormat);
            theBrushFinal.Dispose();
        }

        private void listView_UserProtocols_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
        }

        private Rectangle DrawQuadrants(string protocolName, int numQuadrant, Graphics graphics, Rectangle rectCell, int itemIndex, bool bSelected, bool bChecked, ref Color textColour)
        {
            RoboSep_RunSamples RunSampleInstance = RoboSep_RunSamples.getInstance();
            if (RunSampleInstance == null)
                return rectCell;

            // get status of current Run
            QuadrantInfo[] Qinfo = RunSampleInstance.RunInfo;
            if (Qinfo == null)
                return rectCell;

            int th, ty, tw, tx;

            th = IMAGEBOX_SIZE + (CellPaddingSize * 2);
            tw = IMAGEBOX_SIZE + (CellPaddingSize * 3);

            if ((tw > rectCell.Width) || (th > rectCell.Height))
                return rectCell;					// not enough room to draw the image, bail out

            SolidBrush theBrush;
            if (bSelected)
            {
                if (itemIndex % 2 == 0)
                {
                    theBrush = new SolidBrush(BG_ColorEven);
                    graphics.FillRectangle(theBrush, rectCell);
                    theBrush.Dispose();
                }
                else
                {
                    theBrush = new SolidBrush(BG_ColorOdd);
                    graphics.FillRectangle(theBrush, rectCell);
                    theBrush.Dispose();
                }
             }

            ty = rectCell.Y + ((rectCell.Height - th) / 2);

            // Draw check marked if the item is checked and multiselect is enabled
     //       if (bChecked && listView_UserProtocols.MultiSelect)
            if (bChecked)
            {
                tx = rectCell.X + CellPaddingSize;
                graphics.DrawImage(this.imageList1.Images[1], tx, ty);

                // remove the width that we used for the graphic from the cell
                rectCell.Width -= (IMAGEBOX_SIZE + 2 * (CellPaddingSize));
                rectCell.X += (tw + CellPaddingSize);
             }

            // check if protocol selected and marked as cancelled
            bool bAddEndPadding = false;
            bool bIsProtocolSelected = false;
            for (int i = 0; i < Qinfo.Length; i++)
            {
                if (string.IsNullOrEmpty(Qinfo[i].QuadrantLabel) || Qinfo[i].QuadrantLabel.Length == 0)
                {
                    continue;
                }
                if (protocolName == Qinfo[i].QuadrantLabel)
                {
                    bool bDrawBackground = true;
                    bIsProtocolSelected = true;
                    for (int j = 0; j < Qinfo[i].QuadrantsRequired; j++)
                    {
                        // index must be less than the max. number of quadrants.
                        if ((i + j) > MaxQuadrantNumberIndex)
                            break;

                        if (Qinfo[i + j].QuadrantUsed == true)
                        {
                            if (bDrawBackground == true)
                            {
                                if (itemIndex % 2 == 0)
                                {
                                    SolidBrush thisBrush = new SolidBrush(BG_ColorEven);
                                    graphics.FillRectangle(thisBrush, rectCell);
                                    thisBrush.Dispose();
                                }
                                else
                                {
                                    SolidBrush thisBrush = new SolidBrush(BG_ColorOdd);                                    
                                    graphics.FillRectangle(thisBrush, rectCell);
                                    thisBrush.Dispose();
                                
                                }
                                bDrawBackground = false;
                            }
                            int Imgindex = (i + j) * 2 +3;
                            if (RunSampleInstance.IsQuadrantMarkedAsCancelled(i))
                            {
                                Imgindex++;
                            }

                            tx = rectCell.X + CellPaddingSize;
                            graphics.DrawImage(this.imageList1.Images[Imgindex], tx, ty);

                            // remove the width that we used for the graphic from the cell
                            rectCell.Width -= (IMAGEBOX_SIZE + 2 * (CellPaddingSize));

                            rectCell.X += (tw + CellPaddingSize);
                            bAddEndPadding = true;
                        }
                    }
                }
            }
            if (!bIsProtocolSelected && bShowRequiredQuadrants)
            {
                if (numQuadrant > 1)
                {
                    bool bDrawBackground = true;
                    SolidBrush theMyBrush;
                    for (int k = 0; k < numQuadrant; k++)
                    {
                        if (bDrawBackground == true)
                        {
                            if (itemIndex % 2 == 0)
                            {
                                theMyBrush = new SolidBrush(BG_ColorEven);
                                graphics.FillRectangle(theMyBrush, rectCell);
                                theMyBrush.Dispose();
                            }
                            else
                            {
                                theMyBrush = new SolidBrush(BG_ColorOdd);
                                graphics.FillRectangle(theMyBrush, rectCell);
                                theMyBrush.Dispose();
                            }
                            bDrawBackground = false;
                        }

                        tx = rectCell.X + CellPaddingSize;
                        graphics.DrawImage(this.imageList1.Images[2], tx, ty);

                        // remove the width that we used for the graphic from the cell
                        rectCell.Width -= (IMAGEBOX_SIZE + 2 * (CellPaddingSize));

                        rectCell.X += (tw + CellPaddingSize);
                        bAddEndPadding = true;
                    }
                }
            }
            
            // Draw a bounding rectangle
            if (bSelected && !bChecked && !bIsProtocolSelected && playButtonPressed)
            {
                // Remove the highlight color
                if (itemIndex % 2 == 0)
                {
                    theBrush = new SolidBrush(BG_ColorEven);
                    graphics.FillRectangle(theBrush, rectCell);
                    theBrush.Dispose();
                }
                else
                {
                    theBrush = new SolidBrush(BG_ColorOdd);
                    graphics.FillRectangle(theBrush, rectCell);
                    theBrush.Dispose();
                }
                int nWidth = rectCell.Width;

                if (nWidth > listView_UserProtocols.Width)
                    nWidth = listView_UserProtocols.Width;

                Rectangle rc = new Rectangle(rectCell.X, rectCell.Y, nWidth, rectCell.Height);
 
                float penWidth = 4.0f;

                rc.Inflate((int)-penWidth / 2, (int)-penWidth / 2);

                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (Pen p = new Pen(Box_Color, penWidth))
                {
                    using (GraphicsPath path = Utilities.GetRoundedRect(rc, cornerRounding))
                    {
                        graphics.DrawPath(p, path);
                    }
                    p.Dispose();
                }
                textColour = Txt_Color;
            }
            else
            {

                if (bAddEndPadding == true)
                {
                    rectCell.X += EndPaddingSize;
                    rectCell.Width -= EndPaddingSize;
                }

                theBrush = new SolidBrush(BG_Selected);
                if (bSelected)
                    graphics.FillRectangle(theBrush, rectCell);
                theBrush.Dispose();
            }
            
            return rectCell;
        }

        private Rectangle DrawUserProtocolChosenIcons(string protocolName, Graphics graphics, Rectangle rectCell, int itemIndex, bool bSelected, bool bChecked)
        {
            SolidBrush theBrush;

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

            // find protocol in user list
            int th, ty, tw, tx;

            th = IMAGEBOX_SIZE + (CellPaddingSize * 2);
            tw = IMAGEBOX_SIZE + (CellPaddingSize * 3);

            if ((tw > rectCell.Width) || (th > rectCell.Height))
                return rectCell;					// not enough room to draw the image, bail out

            if (bSelected)
            {
                if (itemIndex % 2 == 0)
                {
                    theBrush = new SolidBrush(BG_ColorEven);
                    graphics.FillRectangle(theBrush, rectCell);
                    theBrush.Dispose();
                }
                else
                {
                    theBrush = new SolidBrush(BG_ColorOdd);
                    graphics.FillRectangle(theBrush, rectCell);
                    theBrush.Dispose();
                }
            }
            ty = rectCell.Y + ((rectCell.Height - th) / 2);
            tx = rectCell.X + CellPaddingSize;
            int imageIndex = bChecked ? 1 : 0;
            graphics.DrawImage(this.imageList1.Images[imageIndex], tx, ty);

            // remove the width that we used for the graphic from the cell
            rectCell.Width -= (IMAGEBOX_SIZE + 2 * CellPaddingSize);
            rectCell.X += (tw + CellPaddingSize); 
            theBrush = new SolidBrush(BG_Selected);
            if (bSelected)
                graphics.FillRectangle(theBrush, rectCell);
            theBrush.Dispose();

            return rectCell;
        }

        private void btn_ScrollUp_Click(object sender, EventArgs e)
        {
            this.listView_UserProtocols.PageUp();
        }
        private void btn_ScrollDown_Click(object sender, EventArgs e)
        {
            this.listView_UserProtocols.PageDown();
        }
        private void btn_ScrollLeft_Click(object sender, EventArgs e)
        {
            SendMessage(this.listView_UserProtocols.Handle, (uint)WM_HSCROLL, (System.UIntPtr)ScrollEventType.LargeDecrement, (System.IntPtr)0);
        }
        private void btn_ScrollRight_Click(object sender, EventArgs e)
        {
            SendMessage(this.listView_UserProtocols.Handle, (uint)WM_HSCROLL, (System.UIntPtr)ScrollEventType.LargeIncrement, (System.IntPtr)0);
        }

        private void StartScrollUp(object sender, MouseEventArgs e)
        {
            this.listView_UserProtocols.StartScrollingUp();
        }
        private void StopScrollUp(object sender, MouseEventArgs e)
        {
            this.listView_UserProtocols.StopScrollingUp();
        }

        private void StartScrollDown(object sender, MouseEventArgs e)
        {
            this.listView_UserProtocols.StartScrollingDown();
        }
        private void StopScrollDown(object sender, MouseEventArgs e)
        {
            this.listView_UserProtocols.StopScrollingDown();
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

        public ProtocolInfo[] RunSamplesSelectedProtocols()
        {
            return runSamplesLatestSelectedProtocols;
        }

        public ClosingState enumClosingState
        {
            get
            {
                return eClosingState;
            }
            set
            {
                eClosingState = value;
            }
        }

        private void btn_home2_Click(object sender, EventArgs e)
        {
            // LOG
            string LOGmsg = "Protocol Selection home button clicked";
            //  (LOGmsg);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);                

            bReturnToHomeScreen = true; 

            bool bUpdating = IsUpdatingUserProtocols();
            if (bUpdating == false)
            {
                BackToRunSamplesScreen();
            }

            // LOG
            LOGmsg = "Protocol Selection home button exit";
            //  (LOGmsg);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);                
        }

        private void btn_SelectProtocol_Click(object sender, EventArgs e)
        {
            string LOGmsg = "Protocol Selection play button clicked";
            //  (LOGmsg);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);                
            if (listView_UserProtocols.Items.Count == 0)
            {
                return;
            }

            this.btn_SelectProtocol.Click -= evhSelectProtocolClick;

            RoboSep_Protocol tempProtocol = null;
            int nCount = 0;
            if (listView_UserProtocols.MultiSelect)
            {
                // Check how many items are checked
                foreach (ListViewItem lvItem in listView_UserProtocols.Items)
                {
                    if (lvItem.Checked)
                    {
                        nCount++;
                        if (nCount == 1)
                        {
                            if (lvItem.Tag != null)
                            {
                                tempProtocol = lvItem.Tag as RoboSep_Protocol;
                            }
                        }
                        if (nCount > 1)
                            break;
                    }
                }

                if (nCount == 0)
                {
                    string sMSG = LanguageINI.GetString("headerNoSelectedProtocols");
                    string sMSG1 = LanguageINI.GetString("msgSelectUserProtocols");
                    string sMSG2 = LanguageINI.GetString("msgEnter");
                    string sMSG3 = LanguageINI.GetString("Ok");

                    GUI_Controls.RoboMessagePanel prompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_INFORMATION,
                        sMSG1, sMSG, sMSG3);
                    RoboSep_UserConsole.showOverlay();
                    prompt.ShowDialog();
                    prompt.Dispose();
                    RoboSep_UserConsole.hideOverlay();

                    this.btn_SelectProtocol.Click += evhSelectProtocolClick;
                    return;
                }
                else if (1 < nCount)
                {
                    string sMSG = LanguageINI.GetString("headerMultipleSelectedProtocols");
                    string sMSG1 = LanguageINI.GetString("msgSelectOneUserProtocols");
                    string sMSG2 = LanguageINI.GetString("msgEnter");
                    string sMSG3 = LanguageINI.GetString("Ok");

                    GUI_Controls.RoboMessagePanel prompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING,
                        sMSG1, sMSG, sMSG3);
                    RoboSep_UserConsole.showOverlay();
                    prompt.ShowDialog();
                    prompt.Dispose();                    
                    RoboSep_UserConsole.hideOverlay();
                    this.btn_SelectProtocol.Click += evhSelectProtocolClick;
                    return;
                }
            }
            else
            {
                // Single selection
                if (listView_UserProtocols.Items.Count > 0 && listView_UserProtocols.SelectedItems.Count == 0)
                {
                    string sMSG = LanguageINI.GetString("headerNoSelectedProtocols");
                    string sMSG1 = LanguageINI.GetString("msgSelectUserProtocols");
                    string sMSG2 = LanguageINI.GetString("msgEnter");
                    string sMSG3 = LanguageINI.GetString("Ok");

                    GUI_Controls.RoboMessagePanel prompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING,
                        sMSG1, sMSG, sMSG3);
                    RoboSep_UserConsole.showOverlay();
                    prompt.ShowDialog();
                    prompt.Dispose();
                    RoboSep_UserConsole.hideOverlay();
                    this.btn_SelectProtocol.Click += evhSelectProtocolClick;
                    return;
                }

                tempProtocol = listView_UserProtocols.SelectedItems[0].Tag as RoboSep_Protocol;
                nCount++;
            }

            if (tempProtocol != null)
            {
                bool bCurrentQuadrantIsInUse = false;
                    
                // check if the current quadrant has been selected
                if (runSamplesLatestSelectedProtocols != null && runSamplesLatestSelectedProtocols.Length > 0)
                {
                    foreach (ProtocolInfo item in runSamplesLatestSelectedProtocols)
                    {
                        if (item.Quadrant == CurrentQuadrant)
                        {
                            bCurrentQuadrantIsInUse = true;
                            break;
                        }
                    }
                }

                if (bCurrentQuadrantIsInUse == true)
                {
                    // prompt if user is sure they want to overwrite the previous protocol selection
                    string msg = LanguageINI.GetString("msgOverwritePreviousProtocolSelection");
                    string header = LanguageINI.GetString("headerOverwritePreviousProtocolSelection");
                    int nAbsoluteQuadrant = (int)CurrentQuadrant +1;
                    string headerFormatted = string.Format(header, nAbsoluteQuadrant);
                    RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING, msg,
                        headerFormatted, LanguageINI.GetString("Ok"), LanguageINI.GetString("Cancel"));
                    RoboSep_UserConsole.showOverlay();
                    prompt.ShowDialog();
                    RoboSep_UserConsole.hideOverlay();
                    if (prompt.DialogResult != DialogResult.OK)
                    {
                        prompt.Dispose();
                        this.btn_SelectProtocol.Click += evhSelectProtocolClick;
                        return;
                    }
                    prompt.Dispose();
                }
                protocolToBeSelectedBeforeClosing = new ProtocolInfo(CurrentQuadrant, tempProtocol.Protocol_Name, -1, tempProtocol.numQuadrants, false);
                if (UseAllProtocolsListForSelection)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("---------Store the protocol {0},  number of required quadrant={1} for selection before exiting.", tempProtocol.Protocol_Name, tempProtocol.numQuadrants));
 
                    // make sure my user list does not accept duplicates
                    AddProtocolToUserList2(tempProtocol);
                }

                // remove the protocol from the restore list for the current quadrant so that it won't be selected
                if (runSamplesLatestSelectedProtocols != null && runSamplesLatestSelectedProtocols.Length > 0)
                {
                    List<ProtocolInfo> lst = new List<ProtocolInfo>();
                    lst.AddRange(runSamplesLatestSelectedProtocols);
                    lst.RemoveAll(x => { return (!string.IsNullOrEmpty(x.ProtocolName) && x.Quadrant == CurrentQuadrant); });
                    runSamplesLatestSelectedProtocols = lst.ToArray();
                }
            }
 
            bSelectProtocolBeforeReturningToHomeScreen = true;
            bool bUpdating = IsUpdatingUserProtocols();
            if (bUpdating == false)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("---------No need to reload my protocol list."));

                if (nCount == 1 && (horizontalTabs1.TabActive == tabMyProtocols || UseAllProtocolsListForSelection))
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("---------Select the protocol {0} before exiting.", protocolToBeSelectedBeforeClosing.ProtocolName));
                    SelectProtocol(protocolToBeSelectedBeforeClosing.ProtocolName, protocolToBeSelectedBeforeClosing.NumberOfQuadrantRequired);
                }
                else
                {
                    BackToRunSamplesScreen();
                }
            }

            this.btn_SelectProtocol.Click += evhSelectProtocolClick;

            // LOG
            LOGmsg = "Protocol Selection home button exit after sTime seconds.";
            //  (LOGmsg);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);                
        }

        private bool IsUpdatingUserProtocols()
        {
            bool bRequireUpdating = false;

            // Update my protocol list
            if (isDirty == false)
            {
                 System.Diagnostics.Debug.WriteLine(String.Format("---------No reloading of user protocols is required."));
                 return bRequireUpdating;
            }
            
            // Check if the master list and the working list contain the same items
            string[] sMasterProtocolNames = null;
            sMasterProtocolNames = new string[myMasterProtocolsList.Count];

            for (int i = 0; i < myMasterProtocolsList.Count; i++)
            {
                 sMasterProtocolNames[i] = (myMasterProtocolsList[i].Protocol_Name);
            }

            aWorkingProtocolNamesBeforeClosing = null;
            aWorkingProtocolNamesBeforeClosing = new string[myWorkingProtocolsList.Count];

            for (int i = 0; i < myWorkingProtocolsList.Count; i++)
            {
                aWorkingProtocolNamesBeforeClosing[i] = (myWorkingProtocolsList[i].Protocol_Name);
            }

            if (Utilities.UnorderedEqual(sMasterProtocolNames, aWorkingProtocolNamesBeforeClosing))
            {
                System.Diagnostics.Debug.WriteLine(String.Format("---------No changes in the user protocol list, no reloading of user protocols is required."));

                // equal, no need to do update
                isDirty = false;
                return bRequireUpdating;
            }

            System.Diagnostics.Debug.WriteLine(String.Format("---------Changes have been made in the user protocol list, reloading of user protocols is required."));

             // show loading protocol message
            string title = LanguageINI.GetString("headerLoadingProtocols");
            string msg = LanguageINI.GetString("LoadingAllProtocols");
            loading = new RoboMessagePanel5(RoboSep_UserConsole.getInstance(), title, msg, GUI_Controls.GifAnimationMode.eUploadingMultipleFiles);
            RoboSep_UserConsole.showOverlay();
            loading.Show();
            try
            {
                System.Diagnostics.Debug.WriteLine(String.Format("---------Removing MRU entries if the user protocols are removed from the user list."));

                // Remove MRU entries if the protocols are no long in my protocol list
                RemoveMRUEntriesNotInMyProtocolList();

                System.Diagnostics.Debug.WriteLine(String.Format("---------Deselect currently selected user protocols."));

                // remove currently selected protocols if they are no long belonged to my protocol list
                RemoveCurrentProtocolsEx();

                System.Diagnostics.Debug.WriteLine(String.Format("--------- Save dirty user protocols list."));

                string[] aWorkingProtocolFileNames = null;
                aWorkingProtocolFileNames = new string[aWorkingProtocolNamesBeforeClosing.Length];
                for( int i = 0; i < aWorkingProtocolNamesBeforeClosing.Length; i++)
                {
                    aWorkingProtocolFileNames[i] = RoboSep_UserDB.getInstance().getProtocolFilename(aWorkingProtocolNamesBeforeClosing[i]);
                }

                RoboSep_UserDB.getInstance().saveUserProtocols(RoboSep_UserConsole.strCurrentUser, aWorkingProtocolFileNames);
 
                System.Diagnostics.Debug.WriteLine(String.Format("--------- Reload user protocols from the server. Current ThreadID = {0} ----------", Thread.CurrentThread.ManagedThreadId));

                LoadProtocolsToServer();
                SeparatorGateway.GetInstance().separatorUpdating = true;

                if (evhHandleUserProtocolsReloaded == null)
                    evhHandleUserProtocolsReloaded = new EventHandler(HandleUserProtocolsReloadedBeforeExiting);

                RoboSep_UserConsole.getInstance().NotifyUserSeparationProtocolsUpdated += evhHandleUserProtocolsReloaded;

                reloadUserProtocolsCount = 0;
                bRequireUpdating = true;
            }
            catch (Exception ex)
            {
                // LOG
                LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);               
            }

            isDirty = false;
            return bRequireUpdating;
        }

        private void StoreRunSamplesSelectedProtocols(QuadrantInfo[] quadInfos)
        {
            if (quadInfos == null)
                return;

            List<ProtocolInfo> latestSelectedProtocols = new List<ProtocolInfo>();
 
            RoboSep_RunSamples RunSampleInstance = RoboSep_RunSamples.getInstance();
            bool bMarkedAsCancelled = false;
            for (int i = 0; i < quadInfos.Length; i++)
            {
                if (quadInfos[i] == null || string.IsNullOrEmpty(quadInfos[i].QuadrantLabel))
                    continue;
                bMarkedAsCancelled = RunSampleInstance.IsQuadrantMarkedAsCancelled(i);
                ProtocolInfo pInfo = new ProtocolInfo((QuadrantId)i, quadInfos[i].QuadrantLabel, quadInfos[i].QuadrantVolume, quadInfos[i].QuadrantsRequired, bMarkedAsCancelled);
                latestSelectedProtocols.Add(pInfo);
            }

            if (latestSelectedProtocols.Count > 0)
                runSamplesLatestSelectedProtocols = latestSelectedProtocols.ToArray(); 
        }

        private bool IsRequiredToReloadUserProtocols(string[] protocolNames)
        {
            if (protocolNames == null || protocolNames.Length == 0)
            {
                return false;
            }

            System.Diagnostics.Debug.WriteLine(String.Format("--------- Number of user protocols to be reloaded = {0} --------------", protocolNames.Length));

            // Note: due to limitation of ISeparatorProtocol that returns only the label name.
            // There is no guarantee that the label name matches the protocol name 


            UserProtocolsListFromServer = RoboSep_UserConsole.getInstance().XML_getUserProtocols();
            if (UserProtocolsListFromServer == null)
                return false;

            bool bReload = false;
            bool bFoundProtocol = false;
            ISeparationProtocol pProtocol = null;
            int nCount = 0;
            for (int i = 0; i < protocolNames.Length; i++)
            {
                bFoundProtocol = false;
                for (int j = 0; j < UserProtocolsListFromServer.Length; j++)
                {
                    pProtocol = UserProtocolsListFromServer[j];
                    if (pProtocol == null)
                        continue;

                    if (pProtocol.Label == protocolNames[i])
                    {
                        System.Diagnostics.Debug.WriteLine(String.Format("Protocol '{0}' has been loaded.", protocolNames[i]));
                        bFoundProtocol = true;
                        nCount++;
                        break;
                    }
                }

                if (bFoundProtocol == false)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("Protocol '{0}' is not found.", protocolNames[i]));
                }
            }

            if (nCount != protocolNames.Length)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("{0} protocol(s) not being found.", (protocolNames.Length - nCount)));

                if (protocolNames.Length != UserProtocolsListFromServer.Length)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("--------- Number of user protocols does not match the number of user protocols being loaded. Reloading protocols again. --------------"));
                    bReload = true;
                }
            }
            return bReload;
        }

        private void RemoveMRUEntriesNotInMyProtocolList()
        {
            List<string> lstUserProtocols = new List<string>();
            foreach (RoboSep_Protocol item in myWorkingProtocolsList)
            {
                lstUserProtocols.Add(item.Protocol_Name);
            }
            RoboSep_UserDB.getInstance().RemoveMRUEntriesNotInProtocolList(RoboSep_UserConsole.strCurrentUser, lstUserProtocols.ToArray());
        }

        static bool UnorderedEqual<T>(ICollection<T> a, ICollection<T> b)
        {
            // 1
            // Require that the counts are equal
            if (a.Count != b.Count)
            {
                return false;
            }
            // 2
            // Initialize new Dictionary of the type
            Dictionary<T, int> d = new Dictionary<T, int>();

            // 3
            // Add each key's frequency from collection A to the Dictionary
            foreach (T item in a)
            {
                int c;
                if (d.TryGetValue(item, out c))
                {
                    d[item] = c + 1;
                }
                else
                {
                    d.Add(item, 1);
                }
            }
            // 4
            // Add each key's frequency from collection B to the Dictionary
            // Return early if we detect a mismatch
            foreach (T item in b)
            {
                int c;
                if (d.TryGetValue(item, out c))
                {
                    if (c == 0)
                    {
                        return false;
                    }
                    else
                    {
                        d[item] = c - 1;
                    }
                }
                else
                {
                    // Not in dictionary
                    return false;
                }
            }
            // 5
            // Verify that all frequencies are zero
            foreach (int v in d.Values)
            {
                if (v != 0)
                {
                    return false;
                }
            }
            // 6
            // We know the collections are equal
            return true;
        }

        public void StartSelectProtocolTimer()
        {
            SelectProtocolTimer.Start();
        }
    }
 
    public class ListViewSorter : System.Collections.IComparer
    {
        public int Compare(object o1, object o2)
        {
            if (!(o1 is ListViewItem))
                return (0);

            if (!(o2 is ListViewItem))
                return (0);

            ListViewItem lvi1 = (ListViewItem)o1;
            RoboSep_Protocol c1 = lvi1.Tag as RoboSep_Protocol;
            string str1 = c1.Protocol_Name;

            ListViewItem lvi2 = (ListViewItem)o2;
            RoboSep_Protocol c2 = lvi2.Tag as RoboSep_Protocol;
            string str2 = c2.Protocol_Name;

            int result;
            if (lvi1.ListView.Sorting == SortOrder.Ascending)
                result = String.Compare(str1, str2);
            else
                result = String.Compare(str2, str1);

            return (result);
        }
    }   

}
