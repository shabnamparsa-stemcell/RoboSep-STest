using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

using Tesla.Common;
using Tesla.Common.Separator;
using GUI_Controls;
using Tesla.Common.Protocol;
using Tesla.OperatorConsoleControls;

using Invetech.ApplicationLog;

namespace GUI_Console
{
    public partial class Form_BarcodeScan : Form
    {
        private const int IMAGELIST_WIDTH = 40;
        private const int IMAGELIST_HEIGHT = 40;
        private const int IMAGEBOX_SIZE = 30;
        private const int CellPaddingSize = 4;
        private const int EndPaddingSize = 6;
        private const int MaxQuadrantNumberIndex = 3;   // zero based
        private const int MinColumnWidth = IMAGELIST_WIDTH;
        private const int ScanButtonColumn = 2;
        private const int TextBoxColumn = 1;

        private const string NONE_DETECTED = "NONE_DETECTED";
        private const string DEVICE_IO_ERROR = "DEVICE_IO_ERROR";

        private Color TextBoxBackColor;
        private Color TextBoxForeColor;
        private Color TextBoxErrorBackColor;
        private Color TextBoxErrorForeColor;
        private Color TextBoxWarningBackColor;
        private Color TextBoxWarningForeColor;

        private Color TextBoxDataAcquisitionBackColor;
        private Color TextBoxDataAcquisitionForeColor;

        private Font TextBoxFont;
        private string sBarcodeScannerNotInit;
        private string sBarcodeScanError;
        private string sEmptyBarcode;
        private string sScanInProgressText;

        private const int ScanParameterSampleTube_Undefined = 4;
        private const int ScanParameterMagneticParticles = 3;
        private const int ScanParameterCocktail = 2;
        private const int ScanParameterAntibody = 1;

        private int FirstColumnWidth = MinColumnWidth;
        private int SecondColumnWidth = MinColumnWidth;
        private int ThirdColumnWidth = MinColumnWidth;

        private int MaxRetryCount = 1;
        private const int BUTTON_SIZE = 32;

        private int LVSmallChange = IMAGELIST_HEIGHT;
        private int LVLargeChange = IMAGELIST_HEIGHT;
        private bool allGroupsCollapsed = true;

        private Point Offset;

        // Language file
        private IniFile LanguageINI = GUI_Console.RoboSep_UserConsole.getInstance().LanguageINI;

        private ReagentLotIdentifiers1[] myReagentLotIDs1 = null;
        private BarcodeScanner barcodeScanner = null;

        private BackgroundWorker myAsyncWorker = new BackgroundWorker();
        private DoWorkEventHandler evhScanSingleVial;
        private DoWorkEventHandler evhScanQuadrants;

        private RunWorkerCompletedEventHandler evhScanSingleVialCompleted;
        private RunWorkerCompletedEventHandler evhScanQuadrantsCompleted;

        private BarcodeScannerParam1[] myAllQuadrantsScanParams = null;
        private BarcodeScannerParam2 myVialScanParam = null;

        private string[] AbsoluteQuadrantNames = new string[] { "Quadrant1", "Quadrant2", "Quadrant3", "Quadrant4" };

        private RoboMessagePanel WaitingMSG;
        private bool freeAxisAfterBarcodeScan = true;

        // Listview Drawing Vars
        private StringFormat textFormat;
        private Color BG_ColorEven;
        private Color BG_ColorOdd;
        private Color BG_Selected;
        private Color Txt_Color;

        private List<Image> headerImageList = new List<Image>();
        private List<Image> ilistGroupEmpty = new List<Image>();
        private List<Image> ilistGroupExpanded = new List<Image>();
        private List<Image> ilistGroupCollapsed = new List<Image>();
        private List<Image> ilistSaveNormal = new List<Image>();
        private List<Image> ilistSaveDirty = new List<Image>();
        private List<Image> ilistEmbed = new List<Image>();

        private List<FlashTextBox> iListFlashTextBox = new List<FlashTextBox>();

        private Rectangle rcButtonAbort;
        private Rectangle rcButtonScan;

        private EventHandler evHandleLostFocus;

        public Form_BarcodeScan(ReagentLotIdentifiers1[] arryReagentLotIDs)
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.UpdateStyles();

            myReagentLotIDs1 = arryReagentLotIDs;

            ilistGroupEmpty.Add(GUI_Controls.Properties.Resources.BC_GroupEmpty_STD);
            ilistGroupEmpty.Add(GUI_Controls.Properties.Resources.BC_GroupEmpty_OVER);
            ilistGroupEmpty.Add(GUI_Controls.Properties.Resources.BC_GroupEmpty_OVER);
            ilistGroupEmpty.Add(GUI_Controls.Properties.Resources.BC_GroupEmpty_CLICK);

            ilistGroupExpanded.Add(GUI_Controls.Properties.Resources.BC_GroupExpanded_STD);
            ilistGroupExpanded.Add(GUI_Controls.Properties.Resources.BC_GroupExpanded_OVER);
            ilistGroupExpanded.Add(GUI_Controls.Properties.Resources.BC_GroupExpanded_OVER);
            ilistGroupExpanded.Add(GUI_Controls.Properties.Resources.BC_GroupExpanded_CLICK);

            ilistGroupCollapsed.Add(GUI_Controls.Properties.Resources.BC_GroupCollapsed_STD);
            ilistGroupCollapsed.Add(GUI_Controls.Properties.Resources.BC_GroupCollapsed_OVER);
            ilistGroupCollapsed.Add(GUI_Controls.Properties.Resources.BC_GroupCollapsed_OVER);
            ilistGroupCollapsed.Add(GUI_Controls.Properties.Resources.BC_GroupCollapsed_CLICK);
            button_AllGroups.ChangeGraphics(ilistGroupCollapsed);

            ilistSaveDirty.Add(Properties.Resources.L_104x86_save_green);
            ilistSaveDirty.Add(Properties.Resources.GE_BTN10L_save_OVER);
            ilistSaveDirty.Add(Properties.Resources.GE_BTN10L_save_OVER);
            ilistSaveDirty.Add(Properties.Resources.GE_BTN10L_save_CLICK);

            ilistSaveNormal.Add(Properties.Resources.GE_BTN10L_save_STD);
            ilistSaveNormal.Add(Properties.Resources.GE_BTN10L_save_OVER);
            ilistSaveNormal.Add(Properties.Resources.GE_BTN10L_save_OVER);
            ilistSaveNormal.Add(Properties.Resources.GE_BTN10L_save_CLICK);
            button_Save.ChangeGraphics(ilistSaveNormal);
            button_Save.disableImage = Properties.Resources.GE_BTN10L_save_DISABLE;

            List<Image> ilist = new List<Image>();
            ilist.Add(Properties.Resources.L_104x86_single_arrow_left_STD);
            ilist.Add(Properties.Resources.L_104x86_single_arrow_left_OVER);
            ilist.Add(Properties.Resources.L_104x86_single_arrow_left_OVER);
            ilist.Add(Properties.Resources.L_104x86_single_arrow_left_CLICK);
            button_Cancel.ChangeGraphics(ilist);
            button_Cancel.disableImage = Properties.Resources.L_104x86_single_arrow_left_DISABLE;

            ilist.Clear();
            ilist.Add(GUI_Controls.Properties.Resources.scan_N_STD);
            ilist.Add(GUI_Controls.Properties.Resources.scan_N_OVER);
            ilist.Add(GUI_Controls.Properties.Resources.scan_N_OVER);
            ilist.Add(GUI_Controls.Properties.Resources.scan_N_CLICK);
            button_ScanAllQuadrants.ChangeGraphics(ilist);
            button_ScanAllQuadrants.disableImage = GUI_Controls.Properties.Resources.scan_N_DISABLE;

            // abort button
            ilist.Clear();
            ilist.Add(Properties.Resources.RN_BTN02L_abort_104x86_STD);
            ilist.Add(Properties.Resources.RN_BTN02L_abort_104x86_OVER);
            ilist.Add(Properties.Resources.RN_BTN02L_abort_104x86_OVER);
            ilist.Add(Properties.Resources.RN_BTN02L_abort_104x86_CLICK);
            button_Abort.ChangeGraphics(ilist);
            rcButtonAbort = button_Abort.Bounds;
            rcButtonScan = button_Save.Bounds;

            // set up for drawing
            textFormat = new StringFormat();
            textFormat.Alignment = StringAlignment.Near;
            textFormat.LineAlignment = StringAlignment.Center;
            textFormat.FormatFlags = StringFormatFlags.NoWrap;
            BG_ColorEven = Color.FromArgb(216, 217, 218);
            BG_ColorOdd = Color.FromArgb(243, 243, 243);
            BG_Selected = Color.FromArgb(78, 38, 131);
            Txt_Color = Color.FromArgb(95, 96, 98);
            TextBoxBackColor = Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(151)))), ((int)(((byte)(200)))));
            TextBoxForeColor = Color.Black;
            TextBoxFont = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            TextBoxErrorBackColor = Color.Red;
            TextBoxErrorForeColor = Color.White;
            TextBoxWarningBackColor = Color.FromArgb(255, 212, 42);
            TextBoxWarningForeColor = Color.Black;
            TextBoxDataAcquisitionBackColor = Color.LightGreen;
            TextBoxDataAcquisitionForeColor = Color.White;
        }

        private void Form_BarcodeScan_Load(object sender, EventArgs e)
        {
            sBarcodeScannerNotInit = LanguageINI.GetString("BarcodeScanNotInit");
            sBarcodeScanError = LanguageINI.GetString("BarcodeScanError");
            sEmptyBarcode = LanguageINI.GetString("EmptyBarcodeIndicator");
            sScanInProgressText = LanguageINI.GetString("ScanInProgress");
            lblScanQuadrants.Text = LanguageINI.GetString("AllQuadrants");

            this.SuspendLayout();

            // Create a GroupedListControl instance:
            ScrollPanel bsp = this.barcodeScrollPanel;
            bsp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));

            DetermineColumnWidths();

            for (QuadrantId Q = QuadrantId.Quadrant1; Q < QuadrantId.NUM_QUADRANTS; Q++)
            {
                for (int i = 0; i < myReagentLotIDs1.Length; i++)
                {
                    if (myReagentLotIDs1[i].QId != Q || 
                        (!myReagentLotIDs1[i].bRequireAntibodyLabel &&
                        !myReagentLotIDs1[i].bRequireParticleLabel &&
                        !myReagentLotIDs1[i].bRequireCocktailLabel && 
                        !myReagentLotIDs1[i].bRequireSampleLabel))
                        continue;

                    AddQuadrant(ref bsp, i);
                }
            }
            AddEmbeddedControl();

            this.barcodeScrollPanel.CollapseAll();

            // add forms to form tracker
            Offset = new Point((RoboSep_UserConsole.getInstance().Size.Width - this.Size.Width) / 2,
                (RoboSep_UserConsole.getInstance().Size.Height - this.Size.Height) / 2);

            this.Location = new Point(Offset.X + RoboSep_UserConsole.getInstance().Location.X,
                Offset.Y + RoboSep_UserConsole.getInstance().Location.Y);

            RoboSep_UserConsole.getInstance().addForm(this, Offset);

            this.barcodeScrollPanel.VScrollbar = this.scrollBar1;
            bsp.AllGroupsExpanded += new ScrollPanel.AllGroupsExpansionHandler(bsp_AllGroupsExpanded);
            bsp.AllGroupsCollapsed += new ScrollPanel.AllGroupsExpansionHandler(bsp_AllGroupsCollapsed);

            myAsyncWorker.WorkerSupportsCancellation = true;
            evhScanSingleVial = new DoWorkEventHandler(bwScanSingleVial_DoWork);
            evhScanQuadrants = new DoWorkEventHandler(bwScanQuadrants_DoWork);

            evhScanSingleVialCompleted = new RunWorkerCompletedEventHandler(bwScanSingleVial_RunWorkerCompleted);
            evhScanQuadrantsCompleted = new RunWorkerCompletedEventHandler(bwScanQuadrants_RunWorkerCompleted);

            evHandleLostFocus = new EventHandler(tb_LostFocus);

            IniFile GUIini = new IniFile(IniFile.iniFile.GUI);
            string sTemp = GUIini.IniReadValue("System", "FreeAxisAfterBarcodeScan", "true");
            if (!string.IsNullOrEmpty(sTemp))
            {
                if (sTemp.ToLower() == "true")
                {
                    freeAxisAfterBarcodeScan = true;
                }
                else
                {
                    freeAxisAfterBarcodeScan = false;
                }
            }

            this.ResumeLayout(true);

            // Start autoscan if enabled from config.
            if (RoboSep_UserDB.getInstance().EnableAutoScanBarcodes)
            {
                button_ScanAllQuadrants_Click(this.button_ScanAllQuadrants, new EventArgs());
            }
        }

        public void UpdateUIControlStates(bool bEnable)
        {
            ScrollPanel glc = this.barcodeScrollPanel;
            for (int g = 0; g < glc.Controls.Count; g++)
            {
                GroupListView glv = (GroupListView)glc.Controls[g];
                if (glv == null)
                    continue;

                glv.EnableScanButton(bEnable);

                BarcodeListView blv = glv.EmbeddLV;
                if (blv == null)
                    continue;

                if (blv.Items.Count > 0)
                {
                    for (int i = 0; i < blv.Items.Count; i++)
                    {
                        Control ctrl = blv.GetEmbeddedControl(ScanButtonColumn, i);
                        if (ctrl != null)
                        {
                            GUI_Controls.GUIButton b = ctrl as GUI_Controls.GUIButton;
                            if (b != null)
                            {
                                b.disable(!bEnable);
                            }
                        }

                        ctrl = blv.GetEmbeddedControl(TextBoxColumn, i);
                        if (ctrl != null)
                        {
                            FlashTextBox tb = ctrl as FlashTextBox;
                            if (tb != null)
                            {
                                tb.Enabled = bEnable;
                            }
                        }
                    }

                    if (bEnable && blv.Items.Count == 1)
                    {
                        ListViewItem lvItem = blv.Items[0];
                        if (lvItem.Text == LanguageINI.GetString("SampleVial"))
                        {
                            glv.EnableScanButton(false);
                        }
                    }
                }
            }

            SuspendLayout();

            // Back button
            button_Cancel.Visible = bEnable;

            // Save button
            button_Save.Visible = bEnable;

            // Abort button
            button_Abort.Visible = false;
            button_Abort.Bounds = button_Save.Visible ? rcButtonAbort : rcButtonScan;

            if (button_Abort.Visible == button_Save.Visible)
            {
                button_Abort.Visible = !button_Save.Visible;
            }

            // Scan All Quadrants
            button_ScanAllQuadrants.disable(!bEnable);

            // Check whether the barcodes are dirty
            button_Save.ChangeGraphics(IsBarcodesDirty() == true ? ilistSaveDirty : ilistSaveNormal);
            PerformLayout();
        }

        private void DetermineColumnWidths()
        {
            string strTemp = LanguageINI.GetString("MagneticParticlesVial");
            int nWidth = DetermineWidth(strTemp);

            strTemp = LanguageINI.GetString("CockTailVial");
            int nTemp = DetermineWidth(strTemp);
            nWidth = Math.Max(nWidth, nTemp);

            strTemp = LanguageINI.GetString("AntibodyVial");
            nTemp = DetermineWidth(strTemp);
            nWidth = Math.Max(nWidth, nTemp);

            strTemp = LanguageINI.GetString("SampleVial");
            nTemp = DetermineWidth(strTemp);
            nWidth = Math.Max(nWidth, nTemp);

            FirstColumnWidth = nWidth + IMAGELIST_WIDTH + EndPaddingSize;
            ThirdColumnWidth = IMAGELIST_WIDTH + EndPaddingSize;
            SecondColumnWidth = this.barcodeScrollPanel.Width - FirstColumnWidth - ThirdColumnWidth - this.barcodeScrollPanel.Margin.Left;

            System.Diagnostics.Debug.WriteLine(String.Format("FirstColumnWidth={0}, SecondColumnWidth={1}, ThirdColumnWidth={2}", FirstColumnWidth, SecondColumnWidth, ThirdColumnWidth));
        }

        private void AddQuadrant(ref ScrollPanel bsp, int Index)
        {
            if (myReagentLotIDs1 == null || Index < 0 || myReagentLotIDs1.Length <= Index)
                return;

            // Range check
            if (myReagentLotIDs1[Index].QId < QuadrantId.Quadrant1 || myReagentLotIDs1[Index].QId > QuadrantId.Quadrant4)
                return;

            int AbsoluteQuadrant = (int)myReagentLotIDs1[Index].QId + 1;
            GroupListView glv = new GroupListView();
            glv.ID = AbsoluteQuadrantNames[(int)myReagentLotIDs1[Index].QId];

            BarcodeListView blv = glv.EmbeddLV;

            blv.Scrollable = false;

            // Add  columns
            ColumnHeader columnHeader1 = new ColumnHeader();
            columnHeader1.Text = "Quadrant " + AbsoluteQuadrant.ToString();
            columnHeader1.Width = FirstColumnWidth;

            ColumnHeader columnHeader2 = new ColumnHeader();
            columnHeader2.Text = "Barcode";
            columnHeader2.Width = SecondColumnWidth;

            ColumnHeader columnHeader3 = new ColumnHeader();
            columnHeader3.Text = "ReScan All";
            columnHeader3.Width = ThirdColumnWidth;

            System.Diagnostics.Debug.WriteLine(String.Format("bsp.Width = {0}. Before: GroupListView.Width = {1}", bsp.Width, glv.Width));

            glv.Width = FirstColumnWidth + SecondColumnWidth + ThirdColumnWidth;

            System.Diagnostics.Debug.WriteLine(String.Format("bsp.Width = {0}. After: GroupListView.Width = {1}", bsp.Width, glv.Width));

            blv.Name = "Quadrant" + AbsoluteQuadrant.ToString();
            blv.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3 });

            string sItemDesc, sSubItemDesc1, sSubItemDesc2;
            int nImageIndex = -1;

            string sQuadrant = LanguageINI.GetString("Quadrant");
            string sScan = LanguageINI.GetString("Scan");
            string sRescan = LanguageINI.GetString("Rescan");

            glv.LabelText = sQuadrant + " " + AbsoluteQuadrant.ToString();

            BarcodeScannerParam1 para1 = new BarcodeScannerParam1(AbsoluteQuadrant);
            Dictionary<int, object> dict = para1.DictTextbox;

            // Magnetic Particles
            if (myReagentLotIDs1[Index].bRequireParticleLabel)
            {
                sItemDesc = LanguageINI.GetString("MagneticParticlesVial");
                if (string.IsNullOrEmpty(myReagentLotIDs1[Index].LotIDs.myParticleLabel))
                {
                    sSubItemDesc1 = sEmptyBarcode;
                    sSubItemDesc2 = sScan;
                }
                else
                {
                    sSubItemDesc1 = myReagentLotIDs1[Index].LotIDs.myParticleLabel;
                    sSubItemDesc2 = sRescan;
                }
                nImageIndex = 1;
                dict.Add(ScanParameterMagneticParticles, null);              // Scan parameter, 3 => Magnetic Particles
                AddItem(ref blv, sItemDesc, sSubItemDesc1, sSubItemDesc2, nImageIndex, AbsoluteQuadrant, ScanParameterMagneticParticles);
            }
            // Cock Tail
            if (myReagentLotIDs1[Index].bRequireCocktailLabel)
            {
                sItemDesc = LanguageINI.GetString("CockTailVial");

                if (string.IsNullOrEmpty(myReagentLotIDs1[Index].LotIDs.myCocktailLabel))
                {
                    sSubItemDesc1 = sEmptyBarcode;
                    sSubItemDesc2 = sScan;
                }
                else
                {
                    sSubItemDesc1 = myReagentLotIDs1[Index].LotIDs.myCocktailLabel;
                    sSubItemDesc2 = sRescan;
                }
                nImageIndex = 3;
                dict.Add(ScanParameterCocktail, null);          // Scan parameter, 2 => Cocktail
                AddItem(ref blv, sItemDesc, sSubItemDesc1, sSubItemDesc2, nImageIndex, AbsoluteQuadrant, ScanParameterCocktail);

            }
            // Anti-body
            if (myReagentLotIDs1[Index].bRequireAntibodyLabel)
            {
                sItemDesc = LanguageINI.GetString("AntibodyVial");
                if (string.IsNullOrEmpty(myReagentLotIDs1[Index].LotIDs.myAntibodyLabel))
                {
                    sSubItemDesc1 = sEmptyBarcode;
                    sSubItemDesc2 = sScan;
                }
                else
                {
                    sSubItemDesc1 = myReagentLotIDs1[Index].LotIDs.myAntibodyLabel;
                    sSubItemDesc2 = sRescan;
                }
                nImageIndex = 5;
                dict.Add(ScanParameterAntibody, null);      // Scan parameter, 1 => Antibogy
                AddItem(ref blv, sItemDesc, sSubItemDesc1, sSubItemDesc2, nImageIndex, AbsoluteQuadrant, ScanParameterAntibody);
            }
            // Sample Tube
            if (myReagentLotIDs1[Index].bRequireSampleLabel)
            {
                sItemDesc = LanguageINI.GetString("SampleVial");
                sSubItemDesc1 = sEmptyBarcode;
                sSubItemDesc2 = sEmptyBarcode;

                if (string.IsNullOrEmpty(myReagentLotIDs1[Index].LotIDs.mySampleLabel))
                    {
                        sSubItemDesc1 = sEmptyBarcode;
                        sSubItemDesc2 = sScan;
                    }
                    else
                    {
                    sSubItemDesc1 = myReagentLotIDs1[Index].LotIDs.mySampleLabel;
                        sSubItemDesc2 = sRescan;
                    }

                nImageIndex = 7;
                AddItem(ref blv, sItemDesc, sSubItemDesc1, sSubItemDesc2, nImageIndex, AbsoluteQuadrant, ScanParameterSampleTube_Undefined);
            }

            if (blv.Items.Count == 0 || (!myReagentLotIDs1[Index].bRequireParticleLabel && !myReagentLotIDs1[Index].bRequireCocktailLabel && !myReagentLotIDs1[Index].bRequireAntibodyLabel))
            {
                glv.EnableScanButton(false);
            }

            // Add custom draw
            //lg.OwnerDraw = true;
            blv.DrawItem += new DrawListViewItemEventHandler(lg_DrawItem);
            blv.DrawSubItem += new DrawListViewSubItemEventHandler(lg_DrawSubItem);
            blv.DrawColumnHeader += new DrawListViewColumnHeaderEventHandler(lg_DrawColumnHeader);

            if (blv.RowHeight > LVSmallChange)
                LVSmallChange = blv.RowHeight;

            if (glv.Height > LVLargeChange)
                LVLargeChange = glv.Height;

            glv.EmbeddButtonScan.Tag = para1;
            glv.EmbeddButtonScan.Click += new EventHandler(EmbeddButton1_Click);

            glv.SetPreferedHeight();
            bsp.Controls.Add(glv);
        }

        public int DetermineWidth(string text)
        {
            if (string.IsNullOrEmpty(text))
                return -1;

            int width = MinColumnWidth;
            Size txtSize = new Size(width, int.MaxValue);
            TextFormatFlags flags = TextFormatFlags.LeftAndRightPadding;
            txtSize = TextRenderer.MeasureText(text, this.Font, txtSize, flags);
            width = txtSize.Width;

            if (width < MinColumnWidth)
                width = MinColumnWidth;

            width = width + Margin.Left + Margin.Right;
            return width;
        }
        private void AddItem(ref BarcodeListView lg, string sItemDesc, string sSubItemDesc1, string sSubItemDesc2, int nImageIndex, int AbsoluteQuadrant, int Vial)
        {
            ListViewItem item = new ListViewItem(sItemDesc);
            item.ImageIndex = nImageIndex;

            ListViewItem.ListViewSubItem subItem1 = new ListViewItem.ListViewSubItem();
            BarcodeScannerParam2 para1 = new BarcodeScannerParam2(AbsoluteQuadrant, Vial, null);
            subItem1.Text = sSubItemDesc1;
            subItem1.Tag = para1;
            item.SubItems.Add(subItem1);

            ListViewItem.ListViewSubItem subItem2 = new ListViewItem.ListViewSubItem();
            BarcodeScannerParam2 para2 = new BarcodeScannerParam2(AbsoluteQuadrant, Vial, null);
            if (sItemDesc == LanguageINI.GetString("SampleVial"))
                subItem2.Text = "";
            else
                subItem2.Text = sSubItemDesc1;
            subItem2.Tag = para2;
            item.SubItems.Add(subItem2);
            lg.Items.Add(item);
        }

        private void AddEmbeddedControl()
        {
            ScrollPanel glc = this.barcodeScrollPanel;
            int nControlCount = glc.Controls.Count;
            BarcodeScannerParam1[] aParaAllQuadrants = null;
            if (0 < nControlCount)
            {
                aParaAllQuadrants = new BarcodeScannerParam1[nControlCount];
                button_ScanAllQuadrants.Tag = aParaAllQuadrants;
            }

            for (int g = 0; g < glc.Controls.Count; g++)
            {
                GroupListView glv = (GroupListView)glc.Controls[g];
                if (glv == null)
                    continue;

                BarcodeListView listview = glv.EmbeddLV;
                if (listview == null)
                    continue;

                GUIButton button = glv.EmbeddButtonScan;
                if (button == null || button.Tag == null)
                    continue;

                BarcodeScannerParam1 para1 = button.Tag as BarcodeScannerParam1;
                if (para1 == null)
                    continue;

                Dictionary<int, object> dict = para1.DictTextbox;
                if (dict == null)
                    continue;

                for (int i = 0; i < listview.Items.Count; i++)
                {
                    // Add edit box 
                    FlashTextBox tb = new FlashTextBox(TextBoxFont, TextBoxBackColor, TextBoxErrorBackColor, TextBoxForeColor, TextBoxErrorForeColor);
                    iListFlashTextBox.Add(tb);

                    tb.Size = new Size(SecondColumnWidth, 40);
                    tb.BorderStyle = BorderStyle.FixedSingle;
                    tb.TextAlign = HorizontalAlignment.Left;
                    tb.AutoSize = false;

                    tb.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left);

                    tb.Click += new System.EventHandler(this.tb_Click);
                    tb.LostFocus += evHandleLostFocus;

                    ListViewItem lvItemTemp = listview.Items[i];
                    string temp = lvItemTemp.Text;
                    tb.Text = listview.Items[i].SubItems[1].Text;
                    tb.Tag = listview.Items[i].SubItems[1].Tag;

                    // button
                    BarcodeScannerParam2 para2 = listview.Items[i].SubItems[2].Tag as BarcodeScannerParam2;
                    para2.Obj = tb;
                    if (dict.ContainsKey(para2.Vial))
                    {
                        dict[para2.Vial] = tb;
                    }

                    // update the scan all quadrants
                    aParaAllQuadrants[g] = new BarcodeScannerParam1(para1);

                    // Put it in the second column of every row
                    listview.AddEmbeddedControl(tb, TextBoxColumn, lvItemTemp.Index);

                    if (para2.Vial != ScanParameterSampleTube_Undefined)
                    {
                        // Add scan button
                        GUI_Controls.GUIButton b = new GUI_Controls.GUIButton();
                        b.Size = new Size(BUTTON_SIZE, BUTTON_SIZE);
                        b.Tag = para2;
                        b.Click += new EventHandler(b_Click);
                        ilistEmbed.Add(GUI_Controls.Properties.Resources.scan_N_STD);
                        ilistEmbed.Add(GUI_Controls.Properties.Resources.scan_N_OVER);
                        ilistEmbed.Add(GUI_Controls.Properties.Resources.scan_N_OVER);
                        ilistEmbed.Add(GUI_Controls.Properties.Resources.scan_N_CLICK);
                        b.ChangeGraphics(ilistEmbed);
                        // Put it in the third column of every row
                        listview.AddEmbeddedControl(b, ScanButtonColumn, lvItemTemp.Index);
                    }
                }
            }
        }

        private void tb_Click(object sender, EventArgs e)
        {
            FlashTextBox tb = sender as FlashTextBox;
            if (tb == null)
                return;

            if (tb.FlasherTextBoxStatus == true)
            {
                // Stop flashing
                tb.FlasherTextBoxStop();
                tb.BackColor = tb.FlasherTextBoxColorBackgroundOff;
                tb.ForeColor = tb.FlasherTextBoxColorForegroundOff;
            }
            else
            {
                tb.Font = TextBoxFont;
                tb.BackColor = TextBoxBackColor;
                tb.ForeColor = TextBoxForeColor;

                if (RoboSep_UserConsole.KeyboardEnabled)
                {
                    tb.LostFocus -= evHandleLostFocus;
                    createKeybaord(tb);
                }
                button_Save.ChangeGraphics(IsBarcodesDirty() ? ilistSaveDirty : ilistSaveNormal);
                if (RoboSep_UserConsole.KeyboardEnabled)
                {
                    tb.LostFocus += evHandleLostFocus;
                }
            }
        }

        private void tb_LostFocus(object sender, EventArgs e)
        {
            FlashTextBox tb = sender as FlashTextBox;
            if (tb == null || tb.FlasherTextBoxStatus == true)
                return;

            button_Save.ChangeGraphics(IsBarcodesDirty() ? ilistSaveDirty : ilistSaveNormal);
        }

        private void b_Click(object sender, EventArgs e)
        {
            GUI_Controls.GUIButton b = sender as GUI_Controls.GUIButton;
            if (b == null)
                return;

            BarcodeScannerParam2 para2 = b.Tag as BarcodeScannerParam2;
            if (para2 == null || para2.Obj == null)
                return;

            int nQuadrant = para2.AbsoluteQuadrant;
            if (nQuadrant < 1 || nQuadrant > 4)
                return;

            // Ensure that lid is closed
            if (!RoboSep_Resources.getInstance().IsLidClosed())
                return;

            myVialScanParam = para2;

            UpdateUIControlStates(false);

            System.Diagnostics.Debug.WriteLine(String.Format(String.Format("Scan barcode for Quadrant: {0}. Vial: {1}.", para2.AbsoluteQuadrant, para2.Vial)));

            BarcodeScannerParam4 para4 = new BarcodeScannerParam4(nQuadrant, para2.Vial);

            // Attach the event handlers
            myAsyncWorker.DoWork += evhScanSingleVial;
            myAsyncWorker.RunWorkerCompleted += evhScanSingleVialCompleted;

            // Kick off the Async thread
            myAsyncWorker.RunWorkerAsync(para4);

            // LOG
            string LOGmsg = String.Format(String.Format("Scan barcode for Quadrant: {0}. Vial: {1} .", para2.AbsoluteQuadrant, para2.Vial));
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);
            return;
        }

        private void bwScanSingleVial_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            BarcodeScannerParam4 para4 = e.Argument as BarcodeScannerParam4;
            if (para4 == null)
                return;

            int nAbsoluteQuadrant = para4.AbsoluteQuadrant;
            if (nAbsoluteQuadrant < 1 || nAbsoluteQuadrant > 4)
                return;

            if (!bw.CancellationPending)
            {
                this.Invoke(
                    (MethodInvoker)delegate()
                    {
                        FlashSingleVialTextBoxDuringDataAcquisition(nAbsoluteQuadrant, para4.Vial);
                    }
                );

                bool bError = false, bSkip = false;
                string sBarcode = ScanAndGetBarCode(nAbsoluteQuadrant, para4.Vial, ref bError, ref bSkip, bw);
                if (bError == false && sBarcode != NONE_DETECTED && sBarcode != DEVICE_IO_ERROR && !bw.CancellationPending)
                {
                    para4.Barcode = sBarcode;
                }
                else
                {
                    para4.Error = true;
                    if (sBarcode == NONE_DETECTED)
                        para4.ErrMessage = LanguageINI.GetString("NoneDetectedError");
                    else if (sBarcode == DEVICE_IO_ERROR)
                        para4.ErrMessage = LanguageINI.GetString("DeviceIOError");
                    else
                        para4.ErrMessage = sBarcode;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ScanSingleVial: User aborts barcode scanning");
                Thread.Sleep(1200);
                e.Cancel = true;
            }

            if (!e.Cancel)
            {
                this.Invoke(
                    (MethodInvoker)delegate()
                    {
                        UpdateSingleVialBarcode(para4);
                    }
                 );
            }
            myAsyncWorker.DoWork -= evhScanSingleVial;
        }

        private void bwScanSingleVial_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            myAsyncWorker.RunWorkerCompleted -= evhScanSingleVialCompleted;

            this.Invoke(
                (MethodInvoker)delegate()
                {
                    UpdateUIControlStates(true);
                }
             );

            this.Invoke(
            (MethodInvoker)delegate()
            {
                CloseWaitDialogAndFreeAxis();
            }
            );
        }

        private void UpdateTextBox(FlashTextBox tb, string text)
        {
            if (tb.InvokeRequired)
            { tb.Invoke(new Action<FlashTextBox, String>(UpdateTextBox), new object[] { tb, text }); }
            else
            { tb.Text = text; }
        }

        private void createKeybaord(TextBox tb)
        {
            // Create keybaord control
            GUI_Controls.Keyboard newKeyboard =
                GUI_Controls.Keyboard.getInstance(RoboSep_UserConsole.getInstance(),
                   tb, null, RoboSep_UserConsole.getInstance().frmOverlay, false);
            newKeyboard.ShowDialog();
            newKeyboard.Dispose();
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "Generating touch keyboard");
        }

        private void EmbeddButton1_Click(object sender, EventArgs e)
        {
            GUI_Controls.GUIButton b = sender as GUI_Controls.GUIButton;
            if (b == null)
                return;

            BarcodeScannerParam1 para1 = b.Tag as BarcodeScannerParam1;
            if (para1 == null)
                return;

            this.barcodeScrollPanel.Refresh();

            // Ensure that lid is closed
            if (!RoboSep_Resources.getInstance().IsLidClosed())
                return;

            BarcodeScannerParam1[] aPara1 = new BarcodeScannerParam1[1];
            aPara1[0] = para1;
            myAllQuadrantsScanParams = aPara1;

            SortedDictionary<int, List<BarcodeScannerParam3>> dictQuadrantsScanParameters = new SortedDictionary<int, List<BarcodeScannerParam3>>();

            Dictionary<int, object> dict = para1.DictTextbox;
            if (dict == null || dict.Count == 0)
                return;

            int nQuadrant = para1.AbsoluteQuadrant;
            if (nQuadrant < 1 || nQuadrant > 4)
                return;

            UpdateUIControlStates(false);

            List<BarcodeScannerParam3> lstVialScanParameters = new List<BarcodeScannerParam3>();
            foreach (KeyValuePair<int, object> pair in dict)
            {
                BarcodeScannerParam3 para3 = new BarcodeScannerParam3(pair.Key);
                lstVialScanParameters.Add(para3);
            }

            // Sort the items
            lstVialScanParameters.Sort(delegate(BarcodeScannerParam3 x, BarcodeScannerParam3 y)
            {
                return x.Vial.CompareTo(y.Vial);
            });

            dictQuadrantsScanParameters.Add(nQuadrant, lstVialScanParameters);

            // Attach the event handlers
            myAsyncWorker.DoWork += evhScanQuadrants;
            myAsyncWorker.RunWorkerCompleted += evhScanQuadrantsCompleted;

            // Kick off the Async thread
            myAsyncWorker.RunWorkerAsync(dictQuadrantsScanParameters);
        }

        private void button_ScanAllQuadrants_Click(object sender, EventArgs e)
        {
            GUI_Controls.GUIButton b = sender as GUI_Controls.GUIButton;

            BarcodeScannerParam1[] aParaAllQuadrants = b.Tag as BarcodeScannerParam1[];
            if (aParaAllQuadrants == null || aParaAllQuadrants.Length == 0)
                return;

            // Ensure that lid is closed
            if (!RoboSep_Resources.getInstance().IsLidClosed())
                return;

            myAllQuadrantsScanParams = aParaAllQuadrants;
            UpdateUIControlStates(false);

            SortedDictionary<int, List<BarcodeScannerParam3>> dictQuadrantsScanParameters = new SortedDictionary<int, List<BarcodeScannerParam3>>();
            for (int i = 0; i < aParaAllQuadrants.Length; i++)
            {
                BarcodeScannerParam1 para1 = aParaAllQuadrants[i];
                if (para1 == null)
                    continue;

                Dictionary<int, object> dict = para1.DictTextbox;
                if (dict == null || dict.Count == 0)
                    continue;

                int nQuadrant = para1.AbsoluteQuadrant;
                if (nQuadrant < 1 || nQuadrant > 4)
                    return;

                List<BarcodeScannerParam3> lstVialScanParameters = new List<BarcodeScannerParam3>();
                foreach (KeyValuePair<int, object> pair in dict)
                {
                    BarcodeScannerParam3 para3 = new BarcodeScannerParam3(pair.Key);
                    lstVialScanParameters.Add(para3);
                }

                // Sort the items
                lstVialScanParameters.Sort(delegate(BarcodeScannerParam3 x, BarcodeScannerParam3 y) {
                    return x.Vial.CompareTo(y.Vial);
                });

                dictQuadrantsScanParameters.Add(nQuadrant, lstVialScanParameters);
            }

            // Attach the event handlers
            myAsyncWorker.DoWork += evhScanQuadrants;
            myAsyncWorker.RunWorkerCompleted += evhScanQuadrantsCompleted;

            // Kick off the Async thread
            myAsyncWorker.RunWorkerAsync(dictQuadrantsScanParameters);
        }

        private void bwScanQuadrants_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            SortedDictionary<int, List<BarcodeScannerParam3>> dict = e.Argument as SortedDictionary<int, List<BarcodeScannerParam3>>;
            if (dict == null)
                return;

            foreach (KeyValuePair<int, List<BarcodeScannerParam3>> pair in dict)
            {
                if (!bw.CancellationPending)
                {
                    int nAbsoluteQuadrant = pair.Key;
                    this.Invoke(
                        (MethodInvoker)delegate()
                        {
                            FlashQuadrantTextBoxesDuringDataAcquisition(nAbsoluteQuadrant);
                        }
                    );

                    List<BarcodeScannerParam3> lstPara3 = pair.Value;

                    int index1, index2, index3;

                    index1 = lstPara3.FindIndex(x => { return (x.Vial == ScanParameterMagneticParticles); });
                    index2 = lstPara3.FindIndex(x => { return (x.Vial == ScanParameterCocktail); });
                    index3 = lstPara3.FindIndex(x => { return (x.Vial == ScanParameterAntibody); });

                    if (index1 >= 0 && index2 >= 0 && index3 >= 0)
                    {
                        bool bHasError = false;
                        int nRetryCount = 0;
                        do
                        {
                            string sBarcodeMagneticParticles = "", sBarcodeCocktail = "", sBarcodeAntibody = "";
                            bool bErrVialMagneticParticles = false, bErrVialCocktail = false, bErrVialAntibody = false, bSkip = false;
                            int nRetCode = 0;

                            // scan all vials 
                            if (ScanQuadrantAndGetBarCodes(nAbsoluteQuadrant, ref sBarcodeMagneticParticles, ref sBarcodeCocktail, ref sBarcodeAntibody, ref nRetCode, ref bSkip, bw) == false)
                            {
                                List<KeyValuePair<int, int>> lstQuadrantVial = new List<KeyValuePair<int, int>>();
                                DecodeQuadrantScanRetCodes((uint)nRetCode, lstQuadrantVial);
                                bErrVialMagneticParticles = IsErrorDuringBarcodeAcquisition(lstQuadrantVial, nAbsoluteQuadrant, ScanParameterMagneticParticles);
                                bErrVialCocktail = IsErrorDuringBarcodeAcquisition(lstQuadrantVial, nAbsoluteQuadrant, ScanParameterCocktail);
                                bErrVialAntibody = IsErrorDuringBarcodeAcquisition(lstQuadrantVial, nAbsoluteQuadrant, ScanParameterAntibody);
                                bHasError = bErrVialMagneticParticles || bErrVialCocktail || bErrVialAntibody;
                                if (!bHasError)
                                {
                                     // Check cocktail barcode matching
                                    if (!bw.CancellationPending)
                                        this.Invoke(
                                            (MethodInvoker)delegate()
                                            {
                                                if (!IsBarcodeMatchProtocolName(nAbsoluteQuadrant, sBarcodeCocktail))
                                                {
                                                    bHasError = true;
                                                }
                                            }
                                        );
                                }
                            }

                            if (!bHasError || nRetryCount == MaxRetryCount)
                            {
                                foreach (BarcodeScannerParam3 para3 in lstPara3)
                                {
                                    if (para3 == null)
                                        continue;

                                    switch (para3.Vial)
                                    {
                                        case ScanParameterMagneticParticles:
                                            if (bErrVialMagneticParticles == false && sBarcodeMagneticParticles != NONE_DETECTED && sBarcodeMagneticParticles != DEVICE_IO_ERROR && !bw.CancellationPending)
                                            {
                                                para3.Barcode = sBarcodeMagneticParticles;
                                            }
                                            else
                                            {
                                                para3.Error = true;
                                                if (sBarcodeMagneticParticles == NONE_DETECTED)
                                                    para3.ErrMessage = LanguageINI.GetString("NoneDetectedError");
                                                else if (sBarcodeMagneticParticles == DEVICE_IO_ERROR)
                                                    para3.ErrMessage = LanguageINI.GetString("DeviceIOError");
                                                else
                                                    para3.ErrMessage = sBarcodeMagneticParticles;
                                            }
                                            break;
                                        case ScanParameterCocktail:
                                            if (bErrVialCocktail == false && sBarcodeCocktail != NONE_DETECTED && sBarcodeCocktail != DEVICE_IO_ERROR && !bw.CancellationPending)
                                            {
                                                para3.Barcode = sBarcodeCocktail;
                                            }
                                            else
                                            {
                                                para3.Error = true;
                                                if (sBarcodeCocktail == NONE_DETECTED)
                                                    para3.ErrMessage = LanguageINI.GetString("NoneDetectedError");
                                                else if (sBarcodeCocktail == DEVICE_IO_ERROR)
                                                    para3.ErrMessage = LanguageINI.GetString("DeviceIOError");
                                                else
                                                    para3.ErrMessage = sBarcodeCocktail;
                                            }
                                            break;
                                        case ScanParameterAntibody:
                                            if (bErrVialAntibody == false && sBarcodeAntibody != NONE_DETECTED && sBarcodeAntibody != DEVICE_IO_ERROR && !bw.CancellationPending)
                                            {
                                                para3.Barcode = sBarcodeAntibody;
                                            }
                                            else
                                            {
                                                para3.Error = true;
                                                if (sBarcodeAntibody == NONE_DETECTED)
                                                    para3.ErrMessage = LanguageINI.GetString("NoneDetectedError");
                                                else if (sBarcodeAntibody == DEVICE_IO_ERROR)
                                                    para3.ErrMessage = LanguageINI.GetString("DeviceIOError");
                                                else
                                                    para3.ErrMessage = sBarcodeAntibody;
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                } // for each
                            } // bAnyError

                            if (bHasError)
                                nRetryCount++;

                        } while (bHasError && nRetryCount <= MaxRetryCount && !bw.CancellationPending);
                    }
                    else
                    {
                        foreach (BarcodeScannerParam3 para3 in lstPara3)
                        {
                            if (para3 == null)
                                continue;

                        
                            int nRetryCount = 0;
                            bool bHasError = false;

                            do
                            {
                                bool bError = false, bSkip = false;
                                string sBarcode = ScanAndGetBarCode(nAbsoluteQuadrant, para3.Vial, ref bError, ref bSkip, bw);
                                bHasError = bError;

                                if (!bHasError || nRetryCount == MaxRetryCount)
                                {
                                    if (!bHasError && sBarcode != NONE_DETECTED && sBarcode != DEVICE_IO_ERROR && !bw.CancellationPending)
                                    {
                                        para3.Barcode = sBarcode;
                                    }
                                    else
                                    {
                                        para3.Error = true;
                                        if (sBarcode == NONE_DETECTED)
                                            para3.ErrMessage = LanguageINI.GetString("NoneDetectedError");
                                        else if (sBarcode == DEVICE_IO_ERROR)
                                            para3.ErrMessage = LanguageINI.GetString("DeviceIOError");
                                        else
                                            para3.ErrMessage = sBarcode;
                                    }
                                }

                                if (bHasError)
                                    nRetryCount++;

                            } while (bHasError && nRetryCount <= MaxRetryCount);
                        }
                        if (bw.CancellationPending)
                        {
                            System.Diagnostics.Debug.WriteLine("ScanQuadrants (individual vials) : User aborts barcode scanning");
                            Thread.Sleep(1200);
                            e.Cancel = true;
                        }
                    }

                    
                    this.Invoke(
                        (MethodInvoker)delegate()
                        {
                            UpdateQuadrantBarcodes(nAbsoluteQuadrant, lstPara3);
                        }
                    );
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ScanQuadrants (all vials) : User aborts barcode scanning");
                    Thread.Sleep(1200);
                    e.Cancel = true;
                    break;
                }
            }
            myAsyncWorker.DoWork -= evhScanQuadrants;

        }

        private bool IsErrorDuringBarcodeAcquisition(List<KeyValuePair<int, int>> lstQuadrantVial, int Q, int V)
        {
            if (lstQuadrantVial == null)
                return false;

            int nIndex = lstQuadrantVial.FindIndex(x =>
            {
                if (x.Key == Q && x.Value == V)
                {
                    return true;
                }
                return false;
            });
            if (0 <= nIndex)
            {
                return true;
            }
            return false;
        }


        private void bwScanQuadrants_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // The background process is complete.
            myAsyncWorker.RunWorkerCompleted -= evhScanQuadrantsCompleted;

            this.Invoke(
                (MethodInvoker)delegate()
                {
                    UpdateUIControlStates(true);
                }
             );

            this.Invoke(
                (MethodInvoker)delegate()
                {
                    CloseWaitDialogAndFreeAxis();
                }
            );
        }

        private void FlashQuadrantTextBoxesDuringDataAcquisition(int absoluteQuadrant)
        {
            // check range 
            if (absoluteQuadrant < 1 || absoluteQuadrant > 4 || myAllQuadrantsScanParams == null)
                return;

            ScrollPanel glc = this.barcodeScrollPanel;
            string QuadrantName = AbsoluteQuadrantNames[absoluteQuadrant - 1];
            for (int g = 0; g < glc.Controls.Count; g++)
            {
                GroupListView glv = (GroupListView)glc.Controls[g];
                if (glv == null)
                    continue;

                if (glv.ID != QuadrantName)
                    continue;

                glv.Expand();
            }
            glc.EnsureGroupVisible(AbsoluteQuadrantNames[absoluteQuadrant - 1]);

            for (int i = 0; i < myAllQuadrantsScanParams.Length; i++)
            {
                BarcodeScannerParam1 para1 = myAllQuadrantsScanParams[i];
                if (para1 == null)
                    continue;

                if (para1.AbsoluteQuadrant == absoluteQuadrant)
                {
                    Dictionary<int, object> dict = para1.DictTextbox;
                    if (dict == null || dict.Count == 0)
                        continue;

                    foreach (KeyValuePair<int, object> pair in dict)
                    {
                        switch (pair.Key)
                        {
                            case ScanParameterMagneticParticles:
                                if (pair.Value != null)
                                {
                                    FlashTextBox tb = pair.Value as FlashTextBox;
                                    FlashTextBoxDuringDataAcquisition(tb);
                                }
                                break;

                            case ScanParameterCocktail:
                                if (pair.Value != null)
                                {
                                    FlashTextBox tb = pair.Value as FlashTextBox;
                                    FlashTextBoxDuringDataAcquisition(tb);
                                }
                                break;

                            case ScanParameterAntibody:
                                if (pair.Value != null)
                                {
                                    FlashTextBox tb = pair.Value as FlashTextBox;
                                    FlashTextBoxDuringDataAcquisition(tb);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        private void FlashSingleVialTextBoxDuringDataAcquisition(int absoluteQuadrant, int vial)
        {
            // check range 
            if (myVialScanParam == null || absoluteQuadrant < 1 || absoluteQuadrant > 4 )
                return;

            if (myVialScanParam.AbsoluteQuadrant == absoluteQuadrant && myVialScanParam.Vial == vial)
            {
                FlashTextBox tb = myVialScanParam.Obj as FlashTextBox;
                FlashTextBoxDuringDataAcquisition(tb);
            }
        }

        private void FlashTextBoxDuringDataAcquisition(FlashTextBox tb)
        {
            if (tb == null)
                return;

            tb.FlasherTextBoxStop();
            tb.FlasherTextBoxColorBackgroundOn = TextBoxDataAcquisitionBackColor;
            tb.FlasherTextBoxColorForegroundOn = TextBoxDataAcquisitionForeColor;
            tb.Text = sScanInProgressText;
        }

        private void FlashTextBoxBarcodeError(FlashTextBox tb)
        {
            if (tb == null)
                return;

            tb.FlasherTextBoxStop();
            tb.FlasherTextBoxColorBackgroundOn = TextBoxBackColor;
            tb.FlasherTextBoxColorForegroundOn = TextBoxForeColor;
            tb.FlasherTextBoxColorBackgroundOff = TextBoxErrorBackColor;
            tb.FlasherTextBoxColorForegroundOff = TextBoxErrorForeColor;
            tb.FlasherTextBoxStart(GUI_Controls.FlashIntervalSpeed.Slow);
        }

        private void FlashTextBoxBarcodeWarning(FlashTextBox tb)
        {
            if (tb == null)
                return;

            tb.FlasherTextBoxStop();
            tb.FlasherTextBoxColorBackgroundOn = TextBoxBackColor;
            tb.FlasherTextBoxColorForegroundOn = TextBoxForeColor;
            tb.FlasherTextBoxColorBackgroundOff = TextBoxWarningBackColor;
            tb.FlasherTextBoxColorForegroundOff = TextBoxWarningForeColor;
            tb.FlasherTextBoxStart(GUI_Controls.FlashIntervalSpeed.Slow);
        }

        private void ResetTextBox(FlashTextBox tb)
        {
            if (tb == null)
                return;

            tb.FlasherTextBoxColorBackgroundOn = TextBoxBackColor;
            tb.FlasherTextBoxColorForegroundOn = TextBoxForeColor;

            tb.FlasherTextBoxColorBackgroundOff = TextBoxBackColor;
            tb.FlasherTextBoxColorForegroundOff = TextBoxForeColor;
            if (tb.FlasherTextBoxStatus)
            {
                tb.FlasherTextBoxReset();
            }
        }

        private void UpdateQuadrantBarcodes(int AbsoluteQuadrant, List<BarcodeScannerParam3> lstPara3)
        {
            if (lstPara3 == null || AbsoluteQuadrant < 1 || AbsoluteQuadrant > 4)
                return;

            bool bError = false;
            foreach (BarcodeScannerParam3 para3 in lstPara3)
            {
                if (para3 == null)
                    continue;

                UpdateVialBarcode(AbsoluteQuadrant, para3, ref bError);
            }
        }

        private void FreeAxis()
        {
            if (barcodeScanner != null)
            {
                if (freeAxisAfterBarcodeScan == true)
                {
                    while (!barcodeScanner.IsBarcodeScanningDone())
                    {
                        Thread.Sleep(1000);
                    }
                    string LOGmsg = String.Format(String.Format("Call Freeaxis."));
                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);
                    barcodeScanner.FreeaxisBarcode();
                }
            }
        }

        private bool IsVialBarcodeValid(string code1, string code2)
        {
            int vialBarcodeCharsCheckLength = 5;
            bool result = false;
            if (code1.Trim().Length >= vialBarcodeCharsCheckLength && code2.Trim().Length >= vialBarcodeCharsCheckLength)
            {
                if (code1.Trim().Substring(0, vialBarcodeCharsCheckLength) == code2.Trim().Substring(0, vialBarcodeCharsCheckLength))
                    result = true;
            }
            return result;
        }

        private bool NewBarcodeCheckingHandler(Object para, string expectedBarcode, FlashTextBox tb)
        {
            bool result = false;
            string pBarcode, pErrMessage;
            int pVial;
            bool pError;
            if (para is BarcodeScannerParam3)
            {
                pBarcode = ((BarcodeScannerParam3)para).Barcode;
                pVial = ((BarcodeScannerParam3)para).Vial;
                pError = ((BarcodeScannerParam3)para).Error;
                pErrMessage = ((BarcodeScannerParam3)para).ErrMessage;
            }
            else
            {
                pBarcode = ((BarcodeScannerParam4)para).Barcode;
                pVial = ((BarcodeScannerParam4)para).Vial;
                pError = ((BarcodeScannerParam4)para).Error;
                pErrMessage = ((BarcodeScannerParam4)para).ErrMessage;
            }

            if (pBarcode == null || pBarcode.Trim().Length == 0)
            {
                if (pError && pErrMessage != null && pErrMessage == LanguageINI.GetString("DeviceIOError"))
                {
                    tb.Text = pErrMessage;
                    FlashTextBoxBarcodeError(tb);
                    result = true;
                }
                else if (expectedBarcode.Length == 0)
                {
                    ResetTextBox(tb);
                    tb.Text = LanguageINI.GetString("CustomVialWarning");
                    FlashTextBoxBarcodeWarning(tb);
                }
                else
                {
                    if (pErrMessage != null)
                        tb.Text = pErrMessage;
                    else
                        tb.Text = LanguageINI.GetString("IncorrectBarcodeError"); 
                    FlashTextBoxBarcodeError(tb);
                    result = true;
                }
            }
            else
            {
                if (expectedBarcode.Length == 0)
                {
                    ResetTextBox(tb);
                    tb.Text = LanguageINI.GetString("NoBarcodeShouldBeReadWarning"); ;
                    FlashTextBoxBarcodeWarning(tb);
                }
                else
                {
                    if (IsVialBarcodeValid(pBarcode, expectedBarcode))
                    {
                        ResetTextBox(tb);
                        tb.Text = pBarcode;
                        
                    }
                    else
                    {
                        tb.Text = String.Format("{0} ({1})", LanguageINI.GetString("IncorrectBarcodeError"), pBarcode);
                        FlashTextBoxBarcodeError(tb);
                        result = true;
                    }
                }
            }
            return result;
        }

        private void UpdateVialBarcode(int AbsoluteQuadrant, BarcodeScannerParam3 para3, ref bool error)
        {
            if (myAllQuadrantsScanParams == null || para3 == null)
                return;

            for (int i = 0; i < myAllQuadrantsScanParams.Length; i++)
            {
                BarcodeScannerParam1 para1 = myAllQuadrantsScanParams[i];
                if (para1 == null)
                    continue;

                if (para1.AbsoluteQuadrant == AbsoluteQuadrant)
                {
                    Dictionary<int, object> dict = para1.DictTextbox;
                    if (dict == null || dict.Count == 0)
                        continue;

                    IProtocol currentProtocol = SeparatorGateway.GetInstance().GetProtocol((QuadrantId)(AbsoluteQuadrant - 1));
                    RoboSep_Protocol_VialBarcodes barcodes = RoboSep_UserDB.getInstance().getProtocolAssignedVialBarcodes(currentProtocol.Label, (int)AbsoluteQuadrant - (int)currentProtocol.InitialQuadrant - 1);

                    foreach (KeyValuePair<int, object> pair in dict)
                    {
                        if (pair.Key != para3.Vial)
                            continue;

                        if (pair.Value != null)
                        {
                            FlashTextBox tb = pair.Value as FlashTextBox;
                            if (tb != null)
                            {
                                if (barcodes != null)
                                {
                                    string expectedBarcode = barcodes.triangleVialBarcode.Trim();
                                    if (para3.Vial == ScanParameterCocktail)
                                        expectedBarcode = barcodes.squareVialBarcode.Trim();
                                    else if (para3.Vial == ScanParameterAntibody)
                                        expectedBarcode = barcodes.circleVialBarcode.Trim();
                                    error = NewBarcodeCheckingHandler(para3, expectedBarcode, tb);
                                }
                                else
                                {
                                    if (!para3.Error)
                                    {
                                        // do protocol matching
                                        if (pair.Key == ScanParameterCocktail)
                                        {
                                            // Do the protocol matching
                                            if (IsBarcodeMatchProtocolName(AbsoluteQuadrant, para3.Barcode) == true)
                                            {
                                                ResetTextBox(tb);
                                                tb.Text = para3.Barcode;
                                            }
                                            else
                                            {
                                                tb.Text = para3.Barcode;
                                                FlashTextBoxBarcodeWarning(tb);
                                            }
                                        }
                                        else
                                        {
                                            ResetTextBox(tb);
                                            tb.Text = para3.Barcode;
                                        }
                                    }
                                    else
                                    {
                                        tb.Text = para3.ErrMessage;
                                        FlashTextBoxBarcodeError(tb);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool IsBarcodeMatchProtocolName(int AbsoluteQuadrant, string sBarcode)
        {
            if (string.IsNullOrEmpty(sBarcode))
                return false;


            // string protocolName = SeparatorGateway.GetInstance().GetProtocolName((QuadrantId)(AbsoluteQuadrant - 1));
            //if (protocolName != null && protocolName.Substring(0, 4).ToUpper() == CUST_LABEL)
            //    return true;
            
            string sCatalog = String.Empty;

            // Determine the catalog #
            string pattern = @"(\b\d+)";
            var match = Regex.Match(sBarcode, pattern);
            if (1 < match.Groups.Count)
            {
                if (!string.IsNullOrEmpty(match.Groups[1].Value))
                {
                    sCatalog = match.Groups[1].Value;
                }
            }

            if (sCatalog.Length != 5)
            {
                return false;
            }

            pattern = String.Format("[\\D+]({0})($|\\D+)", sCatalog); 
            bool bMatch = false;

            // Check to see if it matches the protocol name
            foreach ( ReagentLotIdentifiers1 lotInfo in myReagentLotIDs1)
            {
                if (lotInfo == null || ((int)lotInfo.QId + 1) != AbsoluteQuadrant)
                    continue;

                if (!string.IsNullOrEmpty(lotInfo.QuadrantLabel) && lotInfo.QuadrantLabel.ToLower().Contains(sCatalog.Trim().ToLower()))
                {
                    match = Regex.Match(lotInfo.QuadrantLabel, pattern);
                    if (1 < match.Groups.Count)
                    {
                        if (!string.IsNullOrEmpty(match.Groups[1].Value))
                        {
                            if (sCatalog == match.Groups[1].Value)
                            {
                                bMatch = true;
                            }
                        }
                    }
                }
                break;
            }
            return bMatch;
        }

        private void UpdateSingleVialBarcode(BarcodeScannerParam4 para4)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<BarcodeScannerParam4>(UpdateSingleVialBarcode), new object[] { para4 });
            }
            else
            {
                if (para4 == null || myVialScanParam == null)
                    return;

                if (myVialScanParam.AbsoluteQuadrant == para4.AbsoluteQuadrant && myVialScanParam.Vial == para4.Vial)
                {
                    FlashTextBox tb = myVialScanParam.Obj as FlashTextBox;

                    if (tb == null)
                        return;

                    IProtocol currentProtocol = SeparatorGateway.GetInstance().GetProtocol((QuadrantId)(myVialScanParam.AbsoluteQuadrant - 1));
                    RoboSep_Protocol_VialBarcodes barcodes = RoboSep_UserDB.getInstance().getProtocolAssignedVialBarcodes(currentProtocol.Label, myVialScanParam.AbsoluteQuadrant - (int)currentProtocol.InitialQuadrant - 1);

                    if (barcodes != null)
                    {
                        string expectedBarcode = barcodes.triangleVialBarcode.Trim();
                        if (para4.Vial == ScanParameterCocktail)
                            expectedBarcode = barcodes.squareVialBarcode.Trim();
                        else if (para4.Vial == ScanParameterAntibody)
                            expectedBarcode = barcodes.circleVialBarcode.Trim();
                        NewBarcodeCheckingHandler(para4, expectedBarcode, tb);
                    }
                    else
                    {
                        if (!para4.Error)
                        {
                            // do protocol matching
                            if (myVialScanParam.Vial == ScanParameterCocktail)
                            {
                                // Do the protocol matching
                                if (IsBarcodeMatchProtocolName(para4.AbsoluteQuadrant, para4.Barcode) == true)
                                {
                                    ResetTextBox(tb);
                                    tb.Text = para4.Barcode;
                                }
                                else
                                {
                                    tb.Text = para4.Barcode;
                                    FlashTextBoxBarcodeWarning(tb);
                                }
                            }
                            else
                            {
                                ResetTextBox(tb);
                                tb.Text = para4.Barcode;
                            }
                        }
                        else
                        {
                            tb.Text = para4.ErrMessage;
                            FlashTextBoxBarcodeError(tb);
                        }
                    }
                }
            }
        }

        private bool IsBarcodeScannerInitialized()
        {
            // Start using barcode scanner to scan
            int nRet = 0;
            bool bRet = true;
            string msgHeader, msg;
            RoboMessagePanel prompt;

            if (barcodeScanner == null)
            {
                barcodeScanner = new BarcodeScanner();
                nRet = barcodeScanner.Init();
                if (nRet != 1)
                {
                    msgHeader = LanguageINI.GetString("headerBarcodeScannerInitFailed");
                    msg = LanguageINI.GetString("BarcodeScannerInitFailed");
                    prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_ERROR, msg,
                        msgHeader, LanguageINI.GetString("Skip"), LanguageINI.GetString("Cancel"));
                    RoboSep_UserConsole.showOverlay();
                    DialogResult result = prompt.ShowDialog();
                    RoboSep_UserConsole.hideOverlay();
                    bRet = result == DialogResult.OK ? true : false;
                    prompt.Dispose();
                    return bRet;
                }
            }
            return bRet;
        }

        private bool ScanQuadrantAndGetBarCodes(int Quadrant, ref string sBarcodeMagneticParticles, ref string sBarcodeCocktail, ref string sBarcodeAntibody, ref int nRetCode, ref bool bSkip, BackgroundWorker bw)
        {
            //  uint ScanVialBarcodeAt(int Quadrant, int Vial)
            //  Quadrant:
            //          1-4: only selected quadrant is selected. 
            //  Vial:
            //           0 : all vials in selected quadrat are selected (1,2,3)

            // Range check
            if (Quadrant < 1 || Quadrant > 4)
                return false;

            if (!IsBarcodeScannerInitialized())
                return false;

            string LOGmsg = String.Format(String.Format("Scan barcodes for Quadrant: {0}. Vial: 1, 2 and 3.", Quadrant));
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);

            bool result = true;
            bool bError = false;
            barcodeScanner.ScanVialBarcodeAt(Quadrant, 0, out bError);
            while (!bw.CancellationPending && !barcodeScanner.IsBarcodeScanningDone())
            {
                Thread.Sleep(2000);
            }
            if (!bw.CancellationPending)
            {
                sBarcodeAntibody = barcodeScanner.GetVialBarcodeAt(Quadrant, ScanParameterAntibody);
                sBarcodeCocktail = barcodeScanner.GetVialBarcodeAt(Quadrant, ScanParameterCocktail);
                sBarcodeMagneticParticles = barcodeScanner.GetVialBarcodeAt(Quadrant, ScanParameterMagneticParticles);
            }
            else
            {
                sBarcodeAntibody = NONE_DETECTED;
                sBarcodeCocktail = NONE_DETECTED;
                sBarcodeMagneticParticles = NONE_DETECTED;
                bError = true;
            }
            return result;
        }

        private string ScanAndGetBarCode(int Quadrant, int Vial, ref bool Error, ref bool Skip, BackgroundWorker bw)
        {
            // Range check
            if (Quadrant < 1 || Quadrant > 4)
                return String.Empty;

            if (!IsBarcodeScannerInitialized())
            {
                Error = true;
                return sBarcodeScannerNotInit;
            }

            string LOGmsg = String.Format(String.Format("Scan barcode for Quadrant: {0}. Vial: {1} .", Quadrant, Vial));

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);

            bool bError = false;
            if (!bw.CancellationPending)
                barcodeScanner.ScanVialBarcodeAt(Quadrant, Vial, out bError);
            while (!bw.CancellationPending && !barcodeScanner.IsBarcodeScanningDone())
            {
                Thread.Sleep(2000);
            }
            if (!bw.CancellationPending)
            {

                Error = false;
                return barcodeScanner.GetVialBarcodeAt(Quadrant, Vial);
            }
            else
            {
                Error = false;
                return NONE_DETECTED;
            }
        }

        private void DecodeQuadrantScanRetCodes(uint scanCode, List<KeyValuePair<int, int>> lstQuadrantVial)
        {
            uint n1 = scanCode & 255;
            uint n2 = (scanCode >> 8) & 255;
            uint n3 = (scanCode >> 16) & 255;
            uint n4 = (scanCode >> 24) & 255;

            if (n1 != 0)
            {
                DecodeVials(1, n1, lstQuadrantVial);
            }
            if (n2 != 0)
            {
                DecodeVials(2, n2, lstQuadrantVial);
            }
            if (n3 != 0)
            {
                DecodeVials(3, n3, lstQuadrantVial);
            }
            if (n4 != 0)
            {
                DecodeVials(4, n4, lstQuadrantVial);
            }
        }

        private void DecodeVials(int quad, uint vialCode, List<KeyValuePair<int, int>> lstQuadrantVial)
        {
            if ((vialCode & 1) != 0)
            {
                lstQuadrantVial.Add(new KeyValuePair<int, int>(quad, 1));
            }
            if ((vialCode & 2) != 0)
            {
                lstQuadrantVial.Add(new KeyValuePair<int, int>(quad, 2));
            }
            if ((vialCode & 4) != 0)
            {
                lstQuadrantVial.Add(new KeyValuePair<int, int>(quad, 3));
            }
        }

        private void IsVialReturnError(uint scanCode, int quad, uint vial)
        {
            bool bError = false;
            uint uTemp = 0;
            switch (quad)
            {
                case 1:
                    uTemp = scanCode & 255;
                    break;

                case 2:
                    uTemp = (scanCode >> 8) & 255;
                    break;

                case 3:
                    uTemp = (scanCode >> 16) & 255;
                    break;

                case 4:
                    uTemp = (scanCode >> 24) & 255;
                    break;
            }

            switch (vial)
            {
                case ScanParameterAntibody:
                    uTemp &= 1;
                    break;

                case ScanParameterCocktail:
                    uTemp &= 2;
                    break;

                case ScanParameterMagneticParticles:
                    uTemp &= 4;
                    break;
            }

            bError = uTemp == 0 ? false : true;
         }


        private string GetVialDisplayString(int Vial)
        {
            string strDescription = "???";
            switch (Vial)
            {
                case ScanParameterAntibody: // VialC
                    strDescription = LanguageINI.GetString("AntibodyVial");
                    break;
                case ScanParameterCocktail:   // VialB
                    strDescription = LanguageINI.GetString("CockTailVial");
                    break;
                case ScanParameterMagneticParticles:   // VialA
                    strDescription = LanguageINI.GetString("MagneticParticlesVial");
                    break;
                case ScanParameterSampleTube_Undefined:   // Sample Tube
                    strDescription = LanguageINI.GetString("SampleVial");
                    break;
                default:
                    break;

            }
            return strDescription;
        }

        private void lg_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            if (this.headerImageList.Count == 0)
                return;

            System.Drawing.Drawing2D.LinearGradientBrush GradientBrush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.Blue, Color.LightBlue, 270);
            SolidBrush theBrush  = new SolidBrush(Color.White);
            SolidBrush theBrush2 = new SolidBrush(Color.Yellow); 
            Pen thePen = new Pen(theBrush, 2);
            Font theFont = new Font("Arial", 12);
            e.Graphics.FillRectangle(GradientBrush, e.Bounds);

            e.Graphics.DrawRectangle(thePen, e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2);
            e.Graphics.DrawImage(this.headerImageList[4], e.Bounds);

            e.Graphics.DrawString(e.Header.Text, theFont, theBrush2, e.Bounds);

            theBrush.Dispose();
            theBrush2.Dispose();
            thePen.Dispose();
            theFont.Dispose();
            theFont = null;
        }

        private void lg_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            BarcodeListView lg = (BarcodeListView)sender;

            SolidBrush theBrush, theBrush2;
            Color textColour = Txt_Color;

            Rectangle itemBounds = new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width, e.Bounds.Bottom - e.Bounds.Top);
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
            theBrush2 = new SolidBrush(textColour);
            e.Graphics.DrawString(e.Item.Text, lg.Font, theBrush2,
                new Rectangle(itemBounds.Left, itemBounds.Top, itemBounds.Width, itemBounds.Bottom - itemBounds.Top), textFormat);
            theBrush.Dispose();
            theBrush2.Dispose();
        }

        private void lg_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
        }

        private void scrollBar1_ValueChanged(ICustomScrollbar sender, int newValue)
        {
            SetScrollPosition(newValue);
        }

        public void SetScrollPosition(int newValue)
        {
            if (newValue < this.barcodeScrollPanel.VerticalScroll.Minimum || newValue > this.barcodeScrollPanel.VerticalScroll.Maximum)
            {
                return;
            }
            this.barcodeScrollPanel.VerticalScroll.Value = newValue;
            this.barcodeScrollPanel.Invalidate();

            System.Diagnostics.Debug.WriteLine("Flow Panel - Scroll Bar - Max: " + this.VerticalScroll.Maximum.ToString() + "  Min: " + this.VerticalScroll.Minimum.ToString());
            System.Diagnostics.Debug.WriteLine("Custom Scroll Bar - Max: " + this.scrollBar1.Maximum.ToString() + "  Min: " + this.scrollBar1.Minimum.ToString());
            System.Diagnostics.Debug.WriteLine("Flow Panel vscroll: " + this.VerticalScroll.Value.ToString() + "  custom: " + this.scrollBar1.Value.ToString());

            //       this.PerformLayout();
        }

        private bool IsBarcodesDirty()
        {
            ScrollPanel bsp = this.barcodeScrollPanel;
            if (bsp == null)
                return false;

            bool bDirty = false;
            foreach (GroupListView glv in bsp.Controls)
            {
                if (glv == null)
                    continue;

                BarcodeListView blv = glv.EmbeddLV;
                if (blv == null)
                    continue;

                if (blv.Items.Count == 0)
                    continue;

                for (int i = 0; i < blv.Items.Count; i++)
                {
                    ListViewItem lvItem = blv.Items[i];
                    ListViewItem.ListViewSubItem lvSubItem = lvItem.SubItems[1];
                    Control ctrl = blv.GetEmbeddedControl(TextBoxColumn, i);
                    if (ctrl == null)
                        continue;

                    TextBox tb = ctrl as TextBox;
                    if (tb == null)
                        continue;

                    BarcodeScannerParam2 para2 = lvSubItem.Tag as BarcodeScannerParam2;
                    if (para2 == null)
                        continue;

                    int nAbsoluteQuadrant = para2.AbsoluteQuadrant;
                    QuadrantId QId = (QuadrantId)(nAbsoluteQuadrant - 1);
                    int nVial = para2.Vial;
                    string sBarcode = tb.Text.Trim();
                    if (string.IsNullOrEmpty(sBarcode) || (!string.IsNullOrEmpty(sBarcode) && sBarcode == sEmptyBarcode))
                        sBarcode = String.Empty;

                    if (IsIDDirty(QId, nVial, sBarcode) == true)
                    {
                        bDirty = true;
                        break;
                    }
                }
                if (bDirty)
                    break;
            }
            return bDirty;
        }

        private bool IsIDDirty(QuadrantId iQId, int iVial, string iBarcode)
        {
            if (iQId < QuadrantId.Quadrant1 || iQId > QuadrantId.Quadrant4)
                return false;

            string sTemp = iBarcode;
            if (sTemp == null)
                sTemp = String.Empty;

            bool bDirty = false;
            string sLotID;
            for (int i = 0; i < myReagentLotIDs1.Length; i++)
            {
                if (myReagentLotIDs1[i].QId != iQId)
                    continue;

                switch (iVial)
                {
                    case ScanParameterAntibody: // VialC
                        sLotID = myReagentLotIDs1[i].LotIDs.myAntibodyLabel;
                        if (sLotID == null)
                            sLotID = String.Empty;
                        if (sLotID != sTemp)
                        {
                            bDirty = true;
                        }
                        break;
                    case ScanParameterCocktail:   // VialB
                        sLotID = myReagentLotIDs1[i].LotIDs.myCocktailLabel;
                        if (sLotID == null)
                            sLotID = String.Empty;
                        if (sLotID != sTemp)
                        {
                            bDirty = true;
                        }
                        break;
                    case ScanParameterMagneticParticles:   // VialA
                        sLotID = myReagentLotIDs1[i].LotIDs.myParticleLabel;
                        if (sLotID == null)
                            sLotID = String.Empty;
                        if (sLotID != sTemp)
                        {
                            bDirty = true;
                        }
                        break;
                    case ScanParameterSampleTube_Undefined:   // Sample Tube
                        sLotID = myReagentLotIDs1[i].LotIDs.mySampleLabel;
                            if (sLotID == null)
                                sLotID = String.Empty;
                            if (sLotID != sTemp)
                            {
                                bDirty = true;
                            }
                        break;
                    default:
                        break;
                }
            }
            return bDirty;
        }

        private void UpdateMyReagentID(QuadrantId QId, int Vial, string Barcode)
        {
            if (QId < QuadrantId.Quadrant1 || QId > QuadrantId.Quadrant4)
                return;

            for (int i = 0; i < myReagentLotIDs1.Length; i++)
            {
                if (myReagentLotIDs1[i].QId != QId)
                    continue;

                switch (Vial)
                {
                    case ScanParameterAntibody: // VialC
                        myReagentLotIDs1[i].LotIDs.myAntibodyLabel = Barcode.Equals(sEmptyBarcode) ? String.Empty : Barcode;
                        break;
                    case ScanParameterCocktail:   // VialB
                        myReagentLotIDs1[i].LotIDs.myCocktailLabel = Barcode.Equals(sEmptyBarcode) ? String.Empty : Barcode;
                        break;
                    case ScanParameterMagneticParticles:   // VialA
                        myReagentLotIDs1[i].LotIDs.myParticleLabel = Barcode.Equals(sEmptyBarcode) ? String.Empty : Barcode;
                        break;
                    case ScanParameterSampleTube_Undefined:   // Sample Tube
                        myReagentLotIDs1[i].LotIDs.mySampleLabel = Barcode.Equals(sEmptyBarcode) ? String.Empty : Barcode;
                        break;
                    default:
                        break;
                }
            }
        }

        private void CloseWaitDialogAndFreeAxis()
        {

            if (WaitingMSG != null)
            {
                WaitingMSG.Close();
                RoboSep_UserConsole.hideOverlay();
                WaitingMSG = null;
            }

            // Free the axis
            FreeAxis();
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            ScrollPanel bsp = this.barcodeScrollPanel;
            foreach (GroupListView glv in bsp.Controls)
            {
                if (glv == null)
                    continue;

                BarcodeListView blv = glv.EmbeddLV;
                if (blv == null)
                    continue;

                if (blv.Items.Count == 0)
                    continue;

                for (int i = 0; i < blv.Items.Count; i++)
                {
                    ListViewItem lvItem = blv.Items[i];
                    ListViewItem.ListViewSubItem lvSubItem = lvItem.SubItems[1];
                    Control ctrl = blv.GetEmbeddedControl(TextBoxColumn, i);
                    if (ctrl == null)
                        continue;

                    TextBox tb = ctrl as TextBox;
                    if (tb == null)
                        continue;

                    BarcodeScannerParam2 para2 = lvSubItem.Tag as BarcodeScannerParam2;
                    if (para2 == null)
                        continue;

                    int nAbsoluteQuadrant = para2.AbsoluteQuadrant;
                    QuadrantId QId = (QuadrantId)(nAbsoluteQuadrant - 1);
                    int nVial = para2.Vial;
                    string sBarcode = tb.Text.Trim();

                    System.Diagnostics.Debug.WriteLine("Saving barcode: " + sBarcode + "  for quadrant " + nAbsoluteQuadrant.ToString() + ", Vial " + GetVialDisplayString(nVial));

                    UpdateMyReagentID(QId, nVial, sBarcode);
                }
            }

            // Free the axis
            FreeAxis();

            // remove Control            
            this.Close();
            DialogResult = DialogResult.OK;
        }


        private void button_Cancel_Click(object sender, EventArgs e)
        {
            // Free the axis
            FreeAxis();

            this.DialogResult = DialogResult.Cancel;

            this.Close();
            RoboSep_UserConsole.getInstance().Activate();
            // LOG
            string logMSG = "Scan barcode window closed";
            
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
        }

        private void button_AllGroups_Click(object sender, EventArgs e)
        {
            if (allGroupsCollapsed)
            {
                this.barcodeScrollPanel.ExpandAll();
            }
            else
            {
                this.barcodeScrollPanel.CollapseAll();
            }
        }

        private void bsp_AllGroupsCollapsed(object sender, EventArgs e)
        {
            button_AllGroups.ChangeGraphics(ilistGroupCollapsed);
            allGroupsCollapsed = true;
        }
        private void bsp_AllGroupsExpanded(object sender, EventArgs e)
        {
            button_AllGroups.ChangeGraphics(ilistGroupExpanded);
            allGroupsCollapsed = false;
        }

        private void button_Abort_Click(object sender, EventArgs e)
        {
            if (myAsyncWorker.CancellationPending || !barcodeScanner.Initialized)
                return;

            myAsyncWorker.CancelAsync();

            barcodeScanner.AbortScanVialBarcode();

            string sMSG = LanguageINI.GetString("AbortBarcodeScan");
            string sHeader = LanguageINI.GetString("headerAbortBarcodeScan");
            WaitingMSG = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING, sMSG, sHeader, true, false);
            RoboSep_UserConsole.showOverlay();
            WaitingMSG.Show();
        }
    }

    public class BarcodeScannerParam1
    {
        int absoluteQuadrant = 0;
        Dictionary<int, object> dictTextbox = new Dictionary<int,object>();

        public BarcodeScannerParam1(BarcodeScannerParam1 param1)
        {
            if (param1 != null)
            {
                this.absoluteQuadrant = param1.AbsoluteQuadrant;
                this.dictTextbox = param1.dictTextbox.ToDictionary(entry => entry.Key,
                                               entry => entry.Value);
            }
        }

        public BarcodeScannerParam1(int AbsoluteQuadrant)
        {
            this.absoluteQuadrant = AbsoluteQuadrant;
        }

        public int AbsoluteQuadrant
        {
            get { return absoluteQuadrant; }
            set { absoluteQuadrant = value; }
        }

        public Dictionary<int, object> DictTextbox
        {
            get { return dictTextbox; }
            set
            {
                dictTextbox.Clear();
                dictTextbox = value.ToDictionary(entry => entry.Key,
                                               entry => entry.Value);
            }
        }
    }
    public class BarcodeScannerParam2
    {
        int absoluteQuadrant = 0;
        int vial = -1;
        object obj = null;

        public BarcodeScannerParam2(int AbsoluteQuadrant, int Vial, object obj)
        {
            this.absoluteQuadrant = AbsoluteQuadrant;
            this.vial = Vial;
            this.obj = obj;

        }
        public int AbsoluteQuadrant
        {
            get { return absoluteQuadrant; }
            set { absoluteQuadrant = value; }
        }

        public int Vial
        {
            get { return vial; }
            set { vial = value; }
        }

        public object Obj
        {
            get { return obj; }
            set { obj = value; }
        }
    }

    public class BarcodeScannerParam3
    {
        int vial = -1;
        bool error = false;
        string barcode;
        string errMessage;

        public BarcodeScannerParam3(int Vial)
        {
             this.vial = Vial;
        }
        public int Vial
        {
            get { return vial; }
            set { vial = value; }
        }
        public bool Error
        {
            get { return error; }
            set { error = value; }
        }
        public string Barcode
        {
            get { return barcode; }
            set { barcode = value; }
        }
        public string ErrMessage
        {
            get { return errMessage; }
            set { errMessage = value; }
        }
    }

    public class BarcodeScannerParam4 : BarcodeScannerParam3
    {
        int absoluteQuadrant = 0;
        public BarcodeScannerParam4(int absoluteQuadrant, int Vial) : base (Vial)
        {
            this.absoluteQuadrant = absoluteQuadrant;
        }
        public int AbsoluteQuadrant
        {
            get { return absoluteQuadrant; }
            set { absoluteQuadrant = value; }
        }
    }
}
