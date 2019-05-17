﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Xml;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Threading;
using Invetech.ApplicationLog;

using GUI_Controls;
using Tesla.Common.Separator;
using Tesla.OperatorConsoleControls;
using Tesla.Common.ResourceManagement;
using Tesla.Common;
using Tesla.Common.Protocol;
//YW Added
using Tesla.DataAccess;
using System.IO;
using System.Xml.Serialization;
//End of Addition

namespace GUI_Console
{
    public partial class RoboSep_Resources : UserControl
    {
        public enum enumBarcodeStatus
        {
            Unknown,
            AllEmpty,
            PartiallyFilled,
            FullyFilled,
            NUM_BARCODE_STATUS
        }

        public enum enumResourcesScreenStatus
        {
            eUnDefined = -1,
            eInitialized,
            eEndOfRunCompleted,
            NUM_RESOURCES_SCREEN_STATUS
        } 
        
        // define constant variables
        private const string MagneticParticleVial = "Magnetic Particle Vial";
        private const string NegativeSelectionVial = "Negative Selection Vial";
        private const string AntibodyVial = "Antibody Vial";
        private const string SampleVial = "Sample Vial";
        private const string TipRack = "Tip Rack";
        private const string SeparationTube = "Separation Tube";
        private const string WasteTube = "Waste Tube";
        private const string NegativeFractionTube = "Negative Fraction Tube";
        private const string BufferBottle = "Buffer Bottle";
        private const string CUSTOM_LABEL = "CUSTOM";

        private const double HDD_LOW_MARK = 100.0;
        private const int CarouselLocations = 8;
        private const int NUMBEROFBUTTONS = 4;
        private const int QUADRANT1 = 0;
        private const int QUADRANT2 = 1;
        private const int QUADRANT3 = 2;
        private const int QUADRANT4 = 3;
        private const int ScanParameterSampleTube_Undefined = 4;
        private const int ScanParameterMagneticParticles = 3;
        private const int ScanParameterCocktail = 2;
        private const int ScanParameterAntibody = 1;

        private StringFormat theFormat;
        private Color BG_ColorEven;
        private Color BG_ColorOdd;
        private Color Border_Color;
        private Color BG_Selected1;
        private Color BG_Selected2;
        private Color Txt_Color;


        // define class variables
        private List<robo_Resource> Carousel = new List<robo_Resource>();
        private List<Image> ilist = new List<Image>();
        private List<Image> iListRun1 = new List<Image>();
        private List<Image> iListRun2 = new List<Image>();
        private List<Image> Selectedilist = new List<Image>();
        private GUI_Controls.Button_Quadrant[] QuadrantsSelect = new GUI_Controls.Button_Quadrant[4];
        private int CurrentCarouselItem;
        private int currentQuadrant;
        private List<List<ResourceData>> QuadrantResources = new List<List<ResourceData>>();
        private SharingProtocol[] myCloneRunConfig;
        private bool myIsSharing = false;
        private int[] mySharingTranslation = new int[4];
        private int[] mySharingProtocolIndex = new int[4];

        private Dictionary<string, string> dictDescription = new Dictionary<string, string>();
        private SortedDictionary<QuadrantId, QuadrantRunInfo> dictQuadrantRunInfo = new SortedDictionary<QuadrantId, QuadrantRunInfo>();

        private ReagentLotIdentifiers1[] myReagentLotIDs1;

        private int[] lotIdRef;
        Image[] CarouselNumberImage;

        Bitmap bmParticles = null;
        Bitmap bmCocktail = null;
        Bitmap bmAntibodyVial = null;
        Bitmap bmSampleVial = null;
        Bitmap bmTipRack = null;
        Bitmap bmSeparationTube = null;
        Bitmap bmWasteFraction = null;
        Bitmap bmNegativeFraction = null;
        Bitmap bmBufferBottle = null;

        private enumResourcesScreenStatus eScreenStatus = enumResourcesScreenStatus.eUnDefined;
        private bool bCarouselItemsEventConnected = false;
        private float cornerRounding = 16.0f;

        int lvLastSelectedIndex = -1;

        // Language file
        private IniFile LanguageINI = GUI_Console.RoboSep_UserConsole.getInstance().LanguageINI;

        private IniFile GUIini = null;

        //
        // Create Singleton RoboSep resources
        //
        private static RoboSep_Resources myResources; 

        // Event handlders
        System.EventHandler evhHandleCarouselItem0Click = null;
        System.EventHandler evhHandleCarouselItem1Click = null;
        System.EventHandler evhHandleCarouselItem2Click = null;
        System.EventHandler evhHandleCarouselItem3Click = null;
        System.EventHandler evhHandleCarouselItem4Click = null;
        System.EventHandler evhHandleCarouselItem5Click = null;
        System.EventHandler evhHandleCarouselItem6Click = null;
        System.EventHandler evhHandleCarouselItem7Click = null;
        System.Windows.Forms.ListViewItemSelectionChangedEventHandler evhItemSelectionChanged;

        // Constructor
        public RoboSep_Resources()
        {
            InitializeComponent();

            lotIdRef = new int[4];
            for (int i = 0; i < 4; i++)
            {
                lotIdRef[i] = -1;
            }

            // LOG
            string LOGmsg = "Generating Robosep Resources user control";
            //GUI_Controls.uiLog.LOG(this, "RoboSep_Resources", GUI_Controls.uiLog.LogLevel.EVENTS, LOGmsg);
            //  (LOGmsg);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);                
            this.SuspendLayout();

            this.DoubleBuffered = true;
            this.UpdateStyles();

            // set up for drawing
            theFormat = new StringFormat();
            theFormat.Alignment = StringAlignment.Near;
            theFormat.LineAlignment = StringAlignment.Center;
            BG_ColorEven = Color.FromArgb(216, 217, 218);
            BG_ColorOdd = Color.FromArgb(243, 243, 243);
            BG_Selected1 = Color.FromArgb(78, 38, 131);
            BG_Selected2 = Color.FromArgb(78, 38, 131);     // use same color as BG_Selected1, not blue. Color.FromArgb(59, 147, 223);
            Txt_Color = Color.FromArgb(95, 96, 98);
            Border_Color = Color.FromArgb(155, 155, 155);

            // Run Button
            iListRun2.Clear();
            iListRun2.Add(Properties.Resources.GE_BTN15L_run_sample_green);
            iListRun2.Add(Properties.Resources.GE_BTN15L_run_sample_OVER);
            iListRun2.Add(Properties.Resources.GE_BTN15L_run_sample_OVER);
            iListRun2.Add(Properties.Resources.GE_BTN15L_run_sample_CLICK);

            iListRun1.Clear();
            iListRun1.Add(Properties.Resources.GE_BTN15L_run_sample_STD);
            iListRun1.Add(Properties.Resources.GE_BTN15L_run_sample_OVER);
            iListRun1.Add(Properties.Resources.GE_BTN15L_run_sample_OVER);
            iListRun1.Add(Properties.Resources.GE_BTN15L_run_sample_CLICK);
            buttonRun.ChangeGraphics(iListRun1);

            // button scan
            ilist.Clear();
            ilist.Add(Properties.Resources.RE_BTN16L_scan_STD);
            ilist.Add(Properties.Resources.RE_BTN16L_scan_OVER);
            ilist.Add(Properties.Resources.RE_BTN16L_scan_OVER);
            ilist.Add(Properties.Resources.RE_BTN16L_scan_CLICK);
            button_Scan.ChangeGraphics(ilist);

            // Scroll Up
            ilist.Clear();
            ilist.Add(Properties.Resources.RE_BTN13N_up_arrow_STD);
            ilist.Add(Properties.Resources.RE_BTN13N_up_arrow_OVER);
            ilist.Add(Properties.Resources.RE_BTN13N_up_arrow_OVER);
            ilist.Add(Properties.Resources.RE_BTN13N_up_arrow_CLICK);
            button_ScrollUp.ChangeGraphics(ilist);

            // Scroll Down
            ilist.Clear();
            ilist.Add(Properties.Resources.RE_BTN14N_down_arrow_STD);
            ilist.Add(Properties.Resources.RE_BTN14N_down_arrow_OVER);
            ilist.Add(Properties.Resources.RE_BTN14N_down_arrow_OVER);
            ilist.Add(Properties.Resources.RE_BTN14N_down_arrow_CLICK);
            button_ScrollDown.ChangeGraphics(ilist);
    
            // change list button graphics
            ilist.Clear();
            ilist.Add(Properties.Resources.ResourceButton_PURPLE);
            ilist.Add(Properties.Resources.ResourceButton_PURPLE);
            ilist.Add(Properties.Resources.ResourceButton_GREY);
            ilist.Add(Properties.Resources.ResourceButton_GREY);

            // create ilist for selected buttons
            Selectedilist.Add(Properties.Resources.ResourceButton_GREEN);
            Selectedilist.Add(Properties.Resources.ResourceButton_GREEN);
            Selectedilist.Add(Properties.Resources.ResourceButton_GREEN);
            Selectedilist.Add(Properties.Resources.ResourceButton_GREEN);

            // create array of quadrants
            QuadrantsSelect[0] = button_Quadrant1;
            QuadrantsSelect[1] = button_Quadrant2;
            QuadrantsSelect[2] = button_Quadrant3;
            QuadrantsSelect[3] = button_Quadrant4;
     
            // create the resource description
            dictDescription.Add(MagneticParticleVial, "EasySep\u2122 Reagent Vial");
            dictDescription.Add(NegativeSelectionVial, "EasySep\u2122 Reagent Vial");
            dictDescription.Add(AntibodyVial, "EasySep\u2122 Reagent Vial");
            dictDescription.Add(SampleVial, "14 mL tube with sample");
            dictDescription.Add(TipRack, "RoboSep\u2122 Tip Rack");
            dictDescription.Add(SeparationTube, "14 mL tube");
            dictDescription.Add(WasteTube, "50 mL tube");
            dictDescription.Add(NegativeFractionTube, "50 mL tube");
            dictDescription.Add(BufferBottle, "RoboSep\u2122 Buffer Bottle");
   
            //
            // Carousel Items
            // 

            // Magnetic Particle vial
            this.Carousel.Add(new robo_Resource(MagneticParticleVial, new Point(357, 104), new Image[4] 
                {GUI_Controls.Properties.Resources.Carousel_Section_MagneticParticle,
                GUI_Controls.Properties.Resources.Carousel_Section_MagneticParticleSelect,
                GUI_Controls.Properties.Resources.Carousel_Section_MagneticParticle,
                GUI_Controls.Properties.Resources.Carousel_Section_MagneticParticleE}, pictureBox_1mL, txtBoxLotID));
            Carousel[0].drawCircleRegion();

            // Negative Selection Vial
            this.Carousel.Add(new robo_Resource(NegativeSelectionVial, new Point(360, 135), new Image[4] 
                {GUI_Controls.Properties.Resources.Carousel_Section_NegativeSelection,
                GUI_Controls.Properties.Resources.Carousel_Section_NegativeSelectionSelect,
                GUI_Controls.Properties.Resources.Carousel_Section_NegativeSelection,
                GUI_Controls.Properties.Resources.Carousel_Section_NegativeSelectionE}, pictureBox_1mL, txtBoxLotID));
            Carousel[1].drawCircleRegion();

            // Antibody Vial
            this.Carousel.Add(new robo_Resource(AntibodyVial, new Point(369, 164), new Image[4] 
                {GUI_Controls.Properties.Resources.Carousel_Section_Antibody,
                GUI_Controls.Properties.Resources.Carousel_Section_AntibodySelect,
                GUI_Controls.Properties.Resources.Carousel_Section_Antibody,
                GUI_Controls.Properties.Resources.Carousel_Section_AntibodyE}, pictureBox_1mL, txtBoxLotID));
            Carousel[2].drawCircleRegion();

            // Sample Vial
            this.Carousel.Add(new robo_Resource(SampleVial, new Point(445, 266), new Image[4] 
                {GUI_Controls.Properties.Resources.Carousel_Section_Sample,
                GUI_Controls.Properties.Resources.Carousel_Section_SampleSelect,
                GUI_Controls.Properties.Resources.Carousel_Section_Sample,
                GUI_Controls.Properties.Resources.Carousel_Section_SampleE}, pictureBox_SampleVial, txtBoxLotID));
            Carousel[3].drawCircleRegion();

            // Tip Rack
            this.Carousel.Add(new robo_Resource(TipRack, new Point(384, 180), new Image[4] 
                {GUI_Controls.Properties.Resources.Carousel_Section_TipRack,
                GUI_Controls.Properties.Resources.Carousel_Section_TipRackSelect,
                GUI_Controls.Properties.Resources.Carousel_Section_TipRack,
                GUI_Controls.Properties.Resources.Carousel_Section_TipRackE}, pictureBox_TipRack, txtBoxLotID));
            Point[] tipRackRegion = new Point[] { new Point(0, 18), new Point(30, 0), new Point(96, 0), new Point(96, 62), new Point(59, 96), new Point(0, 96) };
            Carousel[4].drawRegion(tipRackRegion);

            // Separation Tube
            this.Carousel.Add(new robo_Resource(SeparationTube, new Point(486, 180), new Image[4] 
                {GUI_Controls.Properties.Resources.Carousel_Section_Magnet,
                GUI_Controls.Properties.Resources.Carousel_Section_MagnetSelect,
                GUI_Controls.Properties.Resources.Carousel_Section_Magnet,
                GUI_Controls.Properties.Resources.Carousel_Section_MagnetE}, pictureBox_SeparationVial, txtBoxLotID));
            Point[] MagnetRegion = new Point[] { new Point(0, 56), new Point(47, 0), new Point(140, 0), new Point(140, 177), new Point(0, 177) };
            Carousel[5].drawRegion(MagnetRegion);

            // Waste Tube
            this.Carousel.Add(new robo_Resource(WasteTube, new Point(472, 121), new Image[4] 
                {GUI_Controls.Properties.Resources.Carousel_Section_Waste,
                GUI_Controls.Properties.Resources.Carousel_Section_WasteSelect,
                GUI_Controls.Properties.Resources.Carousel_Section_Waste,
                GUI_Controls.Properties.Resources.Carousel_Section_WasteE}, pictureBox_WasteTube, txtBoxLotID));
            Carousel[6].drawCircleRegion();

            // Negative Fraction Tube
            this.Carousel.Add(new robo_Resource(NegativeFractionTube, new Point(393, 112), new Image[4] 
                {GUI_Controls.Properties.Resources.Carousel_Section_NegativeFraction,
                GUI_Controls.Properties.Resources.Carousel_Section_NegativeFractionSelect,
                GUI_Controls.Properties.Resources.Carousel_Section_NegativeFraction,
                GUI_Controls.Properties.Resources.Carousel_Section_NegativeFractionE}, pictureBox_WasteTube, txtBoxLotID));
            Carousel[7].drawCircleRegion();

            // Buffer Bottle
            this.Carousel.Add(new robo_Resource(BufferBottle, new Point(244, 247), null, picturebox_Buffer, txtBoxLotID));
            
            // Add picture boxes from Robo_resource to
            for (int i = 0; i < Carousel.Count-1; i++)
            {
                Carousel[i].picBox.Name = "CarouselComponent" + i.ToString();
                this.Controls.Add(Carousel[i].picBox);
            }

            // add click event for carousel picture box items
            evhHandleCarouselItem0Click = new System.EventHandler(this.CarouselItem0_Click);
            evhHandleCarouselItem1Click = new System.EventHandler(this.CarouselItem1_Click);
            evhHandleCarouselItem2Click = new System.EventHandler(this.CarouselItem2_Click);
            evhHandleCarouselItem3Click = new System.EventHandler(this.CarouselItem3_Click);
            evhHandleCarouselItem4Click = new System.EventHandler(this.CarouselItem4_Click);
            evhHandleCarouselItem5Click = new System.EventHandler(this.CarouselItem5_Click);
            evhHandleCarouselItem6Click = new System.EventHandler(this.CarouselItem6_Click);
            evhHandleCarouselItem7Click = new System.EventHandler(this.CarouselItem7_Click);

            ConnectCarouselItemEventHandler(true);

            evhItemSelectionChanged = new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listView_Resources_ItemSelected);
            this.listView_Resources.ItemSelectionChanged += evhItemSelectionChanged;

            // set label text
            lblLotID.Text = LanguageINI.GetString("lblReagentLotID");

            // add graphics for carousel label & create region
            CarouselNumberImage = new Image[4];
            CarouselNumberImage[0] = GUI_Controls.Properties.Resources.CarouselNumber_1;
            CarouselNumberImage[1] = GUI_Controls.Properties.Resources.CarouselNumber_2;
            CarouselNumberImage[2] = GUI_Controls.Properties.Resources.CarouselNumber_3;
            CarouselNumberImage[3] = GUI_Controls.Properties.Resources.CarouselNumber_4;

            // create points to define region
            List<Point> imgPoints = new List<Point>();
            imgPoints.Add(new Point(18 , 0));
            imgPoints.Add(new Point(32, 16));
            imgPoints.Add(new Point(14, 31));
            imgPoints.Add(new Point(0, 15));
            
            // creates a picture box region
            System.Drawing.Drawing2D.GraphicsPath imgPath = new System.Drawing.Drawing2D.GraphicsPath();
            imgPath.AddPolygon(new Point[] { imgPoints[0], imgPoints[1], imgPoints[2], imgPoints[3] });
            Region imgRegion = new Region(imgPath);
            pictureCarouselNumber.Region = imgRegion;

            GUIini = new IniFile(IniFile.iniFile.GUI);
    
            this.ResumeLayout(true);
        }

        private void RoboSep_Resources_Load(object sender, EventArgs e)
        {
            SetUpList();

            // load quadrant resources (quadrant 1)            
            LoadResources();

            eScreenStatus = enumResourcesScreenStatus.eInitialized;
         }

        public void LoadResources()
        {
            grabQuadrantResources();
        }

        public static RoboSep_Resources getInstance()
        {
            if (myResources == null)
            {
                myResources = new RoboSep_Resources();
            }
            GC.Collect();
            return myResources;
        }

        public bool IsInitialized
        {
            get 
            { 
                bool bInitialized = false;
                if (eScreenStatus > enumResourcesScreenStatus.eUnDefined)
                    bInitialized = true;

                return bInitialized; 
            }
        }

        //this is used in PrepareSampleInformation
        public ReagentLotIdentifiers[][] ReagentLotIdentifiers
        {
            get
            {
                ReagentLotIdentifiers[][] result = null;
                if (myReagentLotIDs1 != null && myReagentLotIDs1.Length > 0)
                {
					//assume myReagentLotIDs1 is based on quadrant number and not protocol number
					//assume idx is stored in ascending order consecutively
                    int myRunConfigIdxMax = myReagentLotIDs1[myReagentLotIDs1.Length-1].myRunConfigIdx;
                    result = new ReagentLotIdentifiers[myRunConfigIdxMax+1][]; //number of protocols in run

                    int currentResultIdx = 0;
                    int currentMyRunConfigIdx = -1;
                    List<ReagentLotIdentifiers> tmpLotIDs = null;
                    for (int i = 0; i < myReagentLotIDs1.Length; i++)
                    {
                        //idx changed so store tmpLotIDs into result
                        if (myReagentLotIDs1[i].myRunConfigIdx != currentMyRunConfigIdx)
                        {
                            if (tmpLotIDs != null && currentResultIdx > -1 && currentResultIdx < result.Length)
                            {
                                result[currentResultIdx] = tmpLotIDs.ToArray();
                                currentResultIdx++;
                            }

                            tmpLotIDs = new List<ReagentLotIdentifiers>();
                            currentMyRunConfigIdx = myReagentLotIDs1[i].myRunConfigIdx;
                        }

                        tmpLotIDs.Add(myReagentLotIDs1[i].LotIDs);
                    }

                    if (tmpLotIDs != null && currentResultIdx > -1 && currentResultIdx < result.Length)
                    {
                        result[currentResultIdx] = tmpLotIDs.ToArray();
                    }
                }
                return result;
            }
        }

        public bool IsLidClosed()
        {
            bool isLidClosed = Tesla.OperatorConsoleControls.SeparatorGateway.GetInstance().IsLidClosed();
            // LOG
            string logMSG = "Ensure lid is closed for barcode scanning = " + isLidClosed.ToString();
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                
            // check first that lid is closed
            if (!isLidClosed)
            {
                string sMSG = LanguageINI.GetString("BarcodeCloseLid");
                RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING, sMSG,
                    LanguageINI.GetString("headerCloseLid"), LanguageINI.GetString("Ok"));
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                RoboSep_UserConsole.hideOverlay();
                prompt.Dispose();                
                return false;
            }
            return true;
        }

        public bool ShouldShowResourcesAfterRunningSamples()
        {
            if (QuadrantResources == null || QuadrantResources.Count == 0)
                return false;

            // do not show resources screen after run if it is maintenance protocol
            if (HasMaintenanceProtocol() == true)
                return false;

            // check if any item being checked in the Result Check dialog of the Protocol Editor
            bool bShouldShowResourcesScreen = false;
            bool bConfigExclusiveUseResultCheckForHighlight = false;

            string sTemp = GUIini.IniReadValue("System", "ExclusiveUseResultCheckForHighlight", "false");
            if (!string.IsNullOrEmpty(sTemp))
            {
                if (sTemp.ToLower() == "true")
                {
                    bConfigExclusiveUseResultCheckForHighlight = true;
                }
                else
                {
                    bShouldShowResourcesScreen = true;
                }
            }

            if (bConfigExclusiveUseResultCheckForHighlight)
            {
                if (myCloneRunConfig != null)
                {
                    bool[] resultChecks;
                    int numQuadrants;

                    for (int i = 0; i < myCloneRunConfig.Length; i++)
                    {
                        SeparatorGateway.GetInstance().GetResultVialSelection(myCloneRunConfig[i].Name, out resultChecks, out numQuadrants);
                        if (resultChecks == null)
                            continue;

                        for (int j = 0; j < resultChecks.Length; j++)
                        {
                            if (resultChecks[j])
                            {
                                bShouldShowResourcesScreen = true;
                                break;
                            }
                        }
                    }
                }
            }
            return bShouldShowResourcesScreen;
        }

        public void UnloadCarouselCompleted(bool bCompleted)
        {
            if (bCompleted)
                eScreenStatus = enumResourcesScreenStatus.eEndOfRunCompleted;
            else
                eScreenStatus = enumResourcesScreenStatus.eInitialized;

            // find the first quadrant
            int nFirstQuadrant = -1;
            for (int i = 0; i < 4; i++)
            {
                if (QuadrantResources[i].Count > 0)
                {
                    nFirstQuadrant = i;
                    break;
                }
            }

             // show the quadrant data
            Quadrant_Click(nFirstQuadrant);
        }

        public void setSharing(SharingProtocol[] RunConfig, bool isSharing, int[] sharing, int[] sharingProtocolIndex)//, ReagentLotIdentifiers[] reagentLotIDs)
        {
            if (RunConfig != null)
            {
                // make a deep copy
                myCloneRunConfig = new SharingProtocol[RunConfig.Length];
                for (int i = 0; i < myCloneRunConfig.Length; i++)
                {
                    myCloneRunConfig[i] = RunConfig[i].Clone();
                }
            }
            myIsSharing = isSharing;
            mySharingTranslation = sharing;
            mySharingProtocolIndex = sharingProtocolIndex;

            if (myCloneRunConfig != null)  
            {
                updateMyReagentLotIDs1(myCloneRunConfig);
            }
        }

        private void updateMyReagentLotIDs1(SharingProtocol[] myRunConfig)
        {
            List<ReagentLotIdentifiers1> list = new List<ReagentLotIdentifiers1>();
            for (int i = 0; i < myRunConfig.Length; i++)
            {
                if (myRunConfig[i].Consumables != null)
                {
                    ReagentLotIdentifiers1 tempIdentifiers1 = null;
                    for (int j = 0; j < myRunConfig[i].Consumables.GetLength(0); j++)
                    {
                        // for each resource item per quadrant
                        /*
                        for (int k = 1; k <= 4; k++)
                        {
                            if (myRunConfig[i].Consumables[j, k].IsRequired == true)
                            {
                                tempIdentifiers1 = new ReagentLotIdentifiers1();
                                break;
                            }
                        }*/
                        tempIdentifiers1 = new ReagentLotIdentifiers1();
                        if (tempIdentifiers1 != null)
                        {
                            tempIdentifiers1.myRunConfigIdx = i;
                            tempIdentifiers1.QId = (QuadrantId) (myRunConfig[i].InitQuadrant + j);
                            tempIdentifiers1.QuadrantLabel = myRunConfig[i].Name;
                            tempIdentifiers1.LotIDs = new ReagentLotIdentifiers();
                            tempIdentifiers1.LotIDs.Reset();
                            list.Add(tempIdentifiers1);
                            tempIdentifiers1 = null;
                        }
                    }
                }
            }
            myReagentLotIDs1 = list.ToArray();
        }

        private bool HasMaintenanceProtocol()
        {
            if (QuadrantResources == null || QuadrantResources.Count == 0)
                return false;

            bool bMaintenance = false;
            foreach (KeyValuePair<QuadrantId, QuadrantRunInfo> kvp in dictQuadrantRunInfo)
            {
                if (kvp.Value.IsMaintenance == true)
                {
                    bMaintenance = true;
                    break;
                }
            }
            return bMaintenance;
        }

        private void ConnectCarouselItemEventHandler(bool bConnect)
        {
            if (bConnect )
            {
                if (!bCarouselItemsEventConnected)
                {
                    this.Carousel[0].picBox.Click += evhHandleCarouselItem0Click;
                    this.Carousel[1].picBox.Click += evhHandleCarouselItem1Click;
                    this.Carousel[2].picBox.Click += evhHandleCarouselItem2Click;
                    this.Carousel[3].picBox.Click += evhHandleCarouselItem3Click;
                    this.Carousel[4].picBox.Click += evhHandleCarouselItem4Click;
                    this.Carousel[5].picBox.Click += evhHandleCarouselItem5Click;
                    this.Carousel[6].picBox.Click += evhHandleCarouselItem6Click;
                    this.Carousel[7].picBox.Click += evhHandleCarouselItem7Click;
                    bCarouselItemsEventConnected = true;
                }
            }
            else
            {
                if (bCarouselItemsEventConnected)
                {
                    this.Carousel[0].picBox.Click -= evhHandleCarouselItem0Click;
                    this.Carousel[1].picBox.Click -= evhHandleCarouselItem1Click;
                    this.Carousel[2].picBox.Click -= evhHandleCarouselItem2Click;
                    this.Carousel[3].picBox.Click -= evhHandleCarouselItem3Click;
                    this.Carousel[4].picBox.Click -= evhHandleCarouselItem4Click;
                    this.Carousel[5].picBox.Click -= evhHandleCarouselItem5Click;
                    this.Carousel[6].picBox.Click -= evhHandleCarouselItem6Click;
                    this.Carousel[7].picBox.Click -= evhHandleCarouselItem7Click;
                    bCarouselItemsEventConnected = false;
                }
            }
        }

        private enum CustomNameOrder
        {
            Buffer,
            Waste,
            NegFraction,
            Cocktail,
            MagParticle,
            Antibody,
            Sample,
            Separation,
            TipRack
        }

        private enum ConsumableOrder
        {
            Sample,
            MagParticle,
            Cocktail,
            Antibody,
            Separation,
            Waste,
            NegFraction,
            TipRack,
            Buffer = 8
        }

        private enum ResultCheckOrder
        {
            Buffer = 0,
            Waste,
            NegFraction,
            Cocktail,
            MagParticle,
            Antibody,
            Sample,
            Separation, 
            Num_Of_ResultChecks
        }

        private void SetUpList()
        {
            bmParticles = new Bitmap(Properties.Resources.RE_BTN01R_magnetic);            
            bmCocktail = new Bitmap(Properties.Resources.RE_BTN02R_cocktail);
            bmAntibodyVial = new Bitmap(Properties.Resources.RE_BTN10R_antibody);
            bmSampleVial = new Bitmap(Properties.Resources.RE_BTN03R_sample_tube);
            bmTipRack = new Bitmap(Properties.Resources.RE_BTN04R_tip_racks);
            bmSeparationTube = new Bitmap(Properties.Resources.RE_BTN05R_seperation);
            bmWasteFraction = new Bitmap(Properties.Resources.RE_BTN06R_waste);
            bmNegativeFraction = new Bitmap(Properties.Resources.RE_BTN07R_fraction);
            bmBufferBottle = new Bitmap(Properties.Resources.RE_BTN08R_buffer_bottle);
            this.imageList1.ColorDepth = ColorDepth.Depth32Bit;
            this.imageList1.ImageSize = new Size(70, 70);
            listView_Resources.SmallImageList = this.imageList1;
        }

        // routine to grab quadrant resources
        private void grabQuadrantResources()
        {
            int MAGNETICPARTICLE = 0;
            int NEGATIVESELECTION = 1;
            int ANTIBODY = 2;

            button_Scan.Visible = RoboSep_UserDB.getInstance().EnableAutoScanBarcodes ? false : true;

            eScreenStatus = enumResourcesScreenStatus.eInitialized;

            // Set listview to single selection
            listView_Resources.MultiSelect = false;

            // Reset the run button graphics
            buttonRun.ChangeGraphics(iListRun1);

            // Reset quadrant resources
            ResetQuadrantResources();

            for (int i = 0; i < 4; i++)
            {
                QuadrantResources.Add(new List<ResourceData>());
            }

            // reset Lot ID ref array
            for (int i = 0; i < lotIdRef.Length; i++)
            {
                lotIdRef[i] = -1;
            }
            
            // get quadrant run infos
            GetQuadrantRunInfos();

            if (myCloneRunConfig != null && myCloneRunConfig[0].Name == "Basic prime")
            {
                bool isUseDefault = true;

                // Check if protocol uses standard reagent kits
                bool[] resultChecks;
                int numQuadrants;

                SeparatorGateway.GetInstance().GetResultVialSelection(myCloneRunConfig[0].Name, out resultChecks, out numQuadrants);
                for (int j = 0; j < resultChecks.Length; j++)
                {
                    if (resultChecks[j])
                    {
                        isUseDefault = false;
                    }
                }
                if (isUseDefault)
                {
                    for (int j = 0; j < myCloneRunConfig[0].Consumables.GetLength(0); j++)
                    {
                        // for each resource item per quadrant
                        for (int k = 0; k < 8; k++)
                        {
                            // remove some elements if sharing reagents
                            if (myCloneRunConfig[0].Consumables[j, k].IsRequired)
                            {
                                // the index is the order of the resources added to the array Carousel[]
                                int index = -1;
                                switch ((RelativeQuadrantLocation)k)
                                {
                                    case RelativeQuadrantLocation.SampleTube:
                                        index = 3;
                                        break;
                                    case RelativeQuadrantLocation.VialA:
                                        index = 0;
                                        break;
                                    case RelativeQuadrantLocation.VialB:
                                        index = 1;
                                        break;
                                    case RelativeQuadrantLocation.VialC:
                                        index = 2;
                                        break;
                                    case RelativeQuadrantLocation.SeparationTube:
                                        index = 5;
                                        break;
                                    case RelativeQuadrantLocation.WasteTube:
                                        index = 6;
                                        break;
                                    case RelativeQuadrantLocation.LysisBufferTube:
                                        index = 7;
                                        break;
                                    case RelativeQuadrantLocation.TipsBox:
                                        index = 4;
                                        break;
                                    case RelativeQuadrantLocation.QuadrantBuffer:
                                        index = 8;
                                        break;
                                    default:
                                        break;
                                }
                                string ResourceName = TipRack;
                                if (0 < myCloneRunConfig[0].CustomNames.Length && !string.IsNullOrEmpty(myCloneRunConfig[0].CustomNames[k]))
                                {
                                    ResourceName = myCloneRunConfig[0].CustomNames[k];
                                }
                                else
                                {
                                    if (0 <= index && index < Carousel.Count)
                                        ResourceName = Carousel[index].sName;
                                }

                                ResourceData tempResource = new ResourceData
                                    (index, ResourceName, myCloneRunConfig[0].Consumables[j, k].Volume.Amount / 1000);

                                // check to see if quadrant is leeching
                                // if so don't add vial a-c (magnetic particle, antibody, negative selection)
                                if (mySharingTranslation[(myCloneRunConfig[0].InitQuadrant) + j] == 0 ||
                                    (index != ANTIBODY && index != NEGATIVESELECTION && index != MAGNETICPARTICLE))
                                    QuadrantResources[(myCloneRunConfig[0].InitQuadrant) + j].Add(tempResource);

                            }
                        }

                    }
                }
            }
            else
            {
                //int[] CustomNamesOrder = { 6, 4, 3, 5, 8, 7, 1, 2, 0 }; // order to list items
                //int[] ConsumablesOrder = { 0, 1, 2, 3, 7, 4, 7, 5, 8 }; // order to give appropriate consumables def to listed items

                int[] CustomNamesOrder = new int[9];
                int[] ConsumablesOrder = new int[9];

                // to re-organize this, also re-organize Carousel
                // in constructor
                CustomNamesOrder[0] = (int)CustomNameOrder.MagParticle;
                ConsumablesOrder[0] = (int)ConsumableOrder.MagParticle;
                CustomNamesOrder[1] = (int)CustomNameOrder.Cocktail;
                ConsumablesOrder[1] = (int)ConsumableOrder.Cocktail;
                CustomNamesOrder[2] = (int)CustomNameOrder.Antibody;
                ConsumablesOrder[2] = (int)ConsumableOrder.Antibody;
                CustomNamesOrder[3] = (int)CustomNameOrder.Sample;
                ConsumablesOrder[3] = (int)ConsumableOrder.Sample;
                CustomNamesOrder[4] = (int)CustomNameOrder.TipRack;
                ConsumablesOrder[4] = (int)ConsumableOrder.TipRack;
                CustomNamesOrder[5] = (int)CustomNameOrder.Separation;
                ConsumablesOrder[5] = (int)ConsumableOrder.Separation;
                CustomNamesOrder[6] = (int)CustomNameOrder.Waste;
                ConsumablesOrder[6] = (int)ConsumableOrder.Waste;
                CustomNamesOrder[7] = (int)CustomNameOrder.NegFraction;
                ConsumablesOrder[7] = (int)ConsumableOrder.NegFraction;
                CustomNamesOrder[8] = (int)CustomNameOrder.Buffer;
                ConsumablesOrder[8] = (int)ConsumableOrder.Buffer;

                bool isFirstQuadrant = true;

                double dTotalBufferVolume = 0;
                string sBufferResourceName = "Buffer Bottle";

                if (myCloneRunConfig != null)
                {
                    // for each protocol
                    for (int i = 0; i < myCloneRunConfig.Length; i++)
                    {
                        UpdateReagentVolumes(myCloneRunConfig[i].InitQuadrant);

                        // set lot id reference for later manual entry of lot id
                        for (int Qref = 0; Qref < myCloneRunConfig[i].Quadrants; Qref++)
                        {
                            lotIdRef[myCloneRunConfig[i].InitQuadrant + Qref] = i;
                        }
                        for (int j = 0; j < myCloneRunConfig[i].Consumables.GetLength(0); j++)
                        {
                            // for each resource item per quadrant
                            for (int k = 0; k < myCloneRunConfig[i].Consumables.GetLength(1); k++)
                            {
                                // remove some elements if sharing reagents
                                if (myCloneRunConfig[i].Consumables[j, ConsumablesOrder[k]].IsRequired)
                                {
                                    string ResourceName = "Tip Rack";
                                    if (CustomNamesOrder[k] != 8)
                                    {
                                        if (CustomNamesOrder[k] < myCloneRunConfig[i].CustomNames.Length && !string.IsNullOrEmpty(myCloneRunConfig[i].CustomNames[CustomNamesOrder[k]]))
                                        {
                                            ResourceName = myCloneRunConfig[i].CustomNames[CustomNamesOrder[k]];
                                        }
                                        else
                                        {
                                            ResourceName = Carousel[k].sName;
                                        }
                                    }

                                    ResourceData tempResource = new ResourceData
                                        (k, ResourceName, myCloneRunConfig[i].Consumables[j, ConsumablesOrder[k]].Volume.Amount / 1000);

                                    //don't add any buffers here 
                                    if (k != (int)ConsumableOrder.Buffer)
                                    {
                                        // check to see if quadrant is leeching
                                        // if so don't add vial a-c (magnetic particle, antibody, negative selection)
                                        if (mySharingTranslation[(myCloneRunConfig[i].InitQuadrant) + j] == 0 ||
                                           (k != ANTIBODY && k != NEGATIVESELECTION && k != MAGNETICPARTICLE))
                                            QuadrantResources[(myCloneRunConfig[i].InitQuadrant) + j].Add(tempResource);
                                    }
                                    else if(isFirstQuadrant)
                                    {
                                        //recalculate buffer bottle volume here
                                        dTotalBufferVolume = Tesla.OperatorConsoleControls.SeparatorGateway.GetInstance().TotalBufferVolume.Amount;
                                        sBufferResourceName = ResourceName;
                                    }
                                }
                            }
                            if (isFirstQuadrant)
                            {
                                //only add buffer once here!
                                ResourceData bufferBottle = new ResourceData((int)ConsumableOrder.Buffer, sBufferResourceName, dTotalBufferVolume);
                                QuadrantResources[(myCloneRunConfig[i].InitQuadrant)].Add(bufferBottle);
                                isFirstQuadrant = false;
                            }
                        }
                    }
                }
            }

            // set up quadrants (
            bool firstquadrant = true;

            for (int i = 0; i < 4; i++)
            {
                QuadrantsSelect[i].Reset_Button();
                for (int j = 0; j < QuadrantResources[i].Count; j++)
                {
                    int carouselLocation = QuadrantResources[i][j].carouselLocation;
                    if (carouselLocation == 0 || carouselLocation == 1 || carouselLocation == 2)
                    {
                        break;
                    }    
                }

                if (firstquadrant && QuadrantResources[i].Count > 0)
                {
                    QuadrantsSelect[i].Check = true;
                    currentQuadrant = i;
                    QuadrantsSelect[i].BackImage[0] = QuadrantsSelect[i].BackImage[4];
                    firstquadrant = false;

                    // update the protocol name
                    UpdateProtocoNameText(i);
                }

                // do not show quadrants with no resources
                if (QuadrantResources[i].Count <= 0)
                {
                    QuadrantsSelect[i].Visible = false;
                }
             
            }

            // load quadrant data after grabbing it
            loadQuadrantResources(currentQuadrant);

            // set flag to indicate that this quadrant has been viewed
            if (dictQuadrantRunInfo.ContainsKey((QuadrantId)currentQuadrant))
            {
                QuadrantRunInfo qinfo = dictQuadrantRunInfo[(QuadrantId)currentQuadrant];
                qinfo.Viewed = true;
            }

            // check if there is any unviewed quadrant
            if (HasUnviewedQuadrant() == false)
            {
                // No unviewed quadrants, show the green 'Play' button
                buttonRun.ChangeGraphics(iListRun2);
            }

            listView_Resources.ResizeLastColumn();
 
            this.SuspendLayout();

            ListItem_Select(0);

            if (listView_Resources.Items.Count > 0)
            {
                listView_Resources.Items[0].Selected = true;
                listView_Resources.EnsureVisible(listView_Resources.Items[0].Index);
            }

            // update the scroll buttons
            UpdateScrollButtonsState();

            this.ResumeLayout(true);
            // LOG
            string LOGmsg = "Loading resources from sharing protocols";
            //GUI_Controls.uiLog.LOG(this, "grabQuadrantResources", GUI_Controls.uiLog.LogLevel.DEBUG, LOGmsg);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, LOGmsg);            

            if (RoboSep_UserDB.getInstance().SkipResourcesScreen)
            {
                RunActivationTimer.Start();
            }
        }

        private bool HasUnviewedQuadrant()
        {
            if (dictQuadrantRunInfo == null)
                return false;

            bool bHasUnviewedQuadrant = false;
            foreach (KeyValuePair<QuadrantId, QuadrantRunInfo> kvp in dictQuadrantRunInfo)
            {
                if (kvp.Value.Viewed)
                    continue;

                bHasUnviewedQuadrant = true;
                break;
            }
            return bHasUnviewedQuadrant;
        }


        private void GetQuadrantRunInfos()
        {
            // reset quadrant viewed dictionay
            if (dictQuadrantRunInfo != null)
            {
                dictQuadrantRunInfo.Clear();
            }
            // update the protocol name
            QuadrantInfo[] QInfos = RoboSep_RunSamples.getInstance().RunInfo;
            if (QInfos != null && QInfos.Length > 0)
            {
                for (int i = 0; i < QInfos.Length; i++)
                {
                    QuadrantInfo qinfo = QInfos[i];
                    if (qinfo != null && qinfo.bQuadrantInUse == true)
                    {
                        QuadrantRunInfo qInfo = new QuadrantRunInfo(qinfo.QuadrantLabel, qinfo.bIsMaintenance, false);
                        dictQuadrantRunInfo.Add((QuadrantId)i, qInfo);
                    }
                }
            }
        }

        private void UpdateAllBufferLotIDs(string sBufferLotID)
        {
            if (string.IsNullOrEmpty(sBufferLotID))
                return;

            foreach (ReagentLotIdentifiers1 tempLot in myReagentLotIDs1)
            {
                if (tempLot == null)
                    continue;

                tempLot.LotIDs.myBufferLabel = sBufferLotID;
            }
        }

        private void UpdateReagentVolumes(int Quadrant)
        {
            SharingProtocol shareProtocol = new SharingProtocol();

            if (Quadrant < 0 || Quadrant > 3 || myCloneRunConfig == null)
                return;

            ProtocolConsumable[,] consumables = null;
            for (int i = 0; i < myCloneRunConfig.Length; i++)
            {
                shareProtocol = myCloneRunConfig[i];
                if (shareProtocol == null || shareProtocol.InitQuadrant != Quadrant)
                    continue;

                consumables = shareProtocol.Consumables;
                break;
            }

            if (consumables == null)
                return;

            int initQuad = Quadrant;

            //temporarily boost volume in a sharing quadrant
            //if sharing AND not leeching
            if (myIsSharing && mySharingTranslation[initQuad] == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    //Q(i+1) is sharing with this quadrant
                    if ((initQuad + 1) == mySharingTranslation[i])
                    {
                        int index = mySharingProtocolIndex[i];
                        if (index < 0 || myCloneRunConfig.Length < index)
                            continue;

                        ProtocolConsumable[,] tmpConsumables = myCloneRunConfig[index].Consumables;
                        if (tmpConsumables == null)
                            continue;

                        int tmpQuadrants = tmpConsumables.GetLength(0);
                        for (int j = 0; j < tmpQuadrants; j++)
                        {
                            double A = tmpConsumables[j, (int)RelativeQuadrantLocation.VialA].Volume.Amount;
                            double B = tmpConsumables[j, (int)RelativeQuadrantLocation.VialB].Volume.Amount;
                            double C = tmpConsumables[j, (int)RelativeQuadrantLocation.VialC].Volume.Amount;

                            FluidVolume VolumeA = consumables[j, (int)RelativeQuadrantLocation.VialA].Volume;
                            FluidVolume VolumeB = consumables[j, (int)RelativeQuadrantLocation.VialB].Volume;
                            FluidVolume VolumeC = consumables[j, (int)RelativeQuadrantLocation.VialC].Volume;

                            //hard code deadvolume...
                            VolumeA.Amount += A > 0 ? (A - 100) : 0;
                            VolumeB.Amount += B > 0 ? (B - 100) : 0;
                            VolumeC.Amount += C > 0 ? (C - 100) : 0;

                            consumables[j, (int)RelativeQuadrantLocation.VialA].Volume = VolumeA;
                            consumables[j, (int)RelativeQuadrantLocation.VialB].Volume = VolumeB;
                            consumables[j, (int)RelativeQuadrantLocation.VialC].Volume = VolumeC;
                        }
                    }
                }
            }

            /*
            //reagent error checking
            string myProtocolPath = RoboSep_UserDB.getInstance().GetProtocolsPath() + shareProtocol.Name + ".xml";
            FileInfo file = new FileInfo(myProtocolPath);
            RoboSepProtocol ProtocolInfo = new RoboSepProtocol();
            if (File.Exists(file.FullName))
            {
                // serializer
                XmlSerializer myXmlSerializer = new XmlSerializer(typeof(RoboSepProtocol));

                // Initialise a file stream for reading
                FileStream fs = new FileStream(file.FullName, FileMode.Open);

                try
                {
                    XmlReaderSettings settings = new XmlReaderSettings();
                    //settings.ValidationType = ValidationType.Schema;
                    settings.ValidationType = ValidationType.None;
                    settings.Schemas.Add("STI", RoboSep_Protocol.GetProtocolXSD());
                    XmlReader reader = XmlReader.Create(fs, settings);
                    ProtocolInfo = (RoboSepProtocol)myXmlSerializer.Deserialize(reader);
                }
                catch (Exception ex)
                {
                    string sMsg = String.Format("Robosep Protocol File '{0}' is invalid. Exception={1}", file.FullName, ex.Message);
                    System.Diagnostics.Debug.Write("!!!RoboSep_Protocols : Read Robosep Protocol failed.");


                    MessageBox.Show(sMsg);
                    //myCurrentActionContext = "User1.udb";
                }
                finally
                {
                    // Close the file stream
                    fs.Close();
                }
            }

            int vialLimit = 0;
            try
            {
                vialLimit = Convert.ToInt32(ProtocolInfo.constraints.featuresSwitches[0].inputData);
            }
            catch (Exception ex)
            {
                string sMsg = String.Format("Protocol is not designed for current software version. Reagent Vials range checking will be skipped");
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Warning, sMsg);

                MessageBox.Show(sMsg);
                return;
            }

            int quadrants = consumables.GetLength(0);
            for (int i = 0; i < quadrants; ++i)
            {
                if (consumables[i, (int)RelativeQuadrantLocation.VialA].Volume.Amount > vialLimit ||
                    consumables[i, (int)RelativeQuadrantLocation.VialB].Volume.Amount > vialLimit||
                    consumables[i, (int)RelativeQuadrantLocation.VialC].Volume.Amount > vialLimit)
                {

                    // prompt user that reagent voulme exceeded
                    string sMSG = LanguageINI.GetString("ExceedReagentLimit");
                    RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING, sMSG,
                    LanguageINI.GetString("headerReagentLimit"), LanguageINI.GetString("Ok"));
                    RoboSep_UserConsole.showOverlay();
                    prompt.ShowDialog();
                    RoboSep_UserConsole.hideOverlay();
                    prompt.Dispose();                    
                }
            }*/
            
        }


        private void ResetQuadrantResources()
        {
            if (Carousel != null)
            {
                // hide all carousel items
                for (int i = 0; i < Carousel.Count; i++)
                {
                    if (Carousel[i] == null)
                        continue;
                    Carousel[i].show(false);
                    Carousel[i].showResource(false);
                    Carousel[i].select(false);
                    Carousel[i].Reset();
                }
            }

            for (int i = 0; i < 4; i++)
            {
                QuadrantsSelect[i].Check = false;
            }

            // Clear protocol name label
            label_protocolName.Text = String.Empty;

            // Clear quadrant resources
            if (QuadrantResources.Count > 0)
            {
                IEnumerator e = QuadrantResources.GetEnumerator();
                while (e.MoveNext())
                {
                    List<ResourceData> lResource = (List<ResourceData>)e.Current;
                    if (lResource == null)
                        continue;

                    lResource.Clear();
                }
                QuadrantResources.Clear();
            }
        }

        // routine that displays appropriate quadrant resources
        private void loadQuadrantResources(int QuadrantNumber)
        {         
            // LOG
            string LOGmsg = "Displaying quadrant " + QuadrantNumber.ToString() + " resources";
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, LOGmsg);            

            if (QuadrantNumber < QUADRANT1 || QuadrantNumber > QUADRANT4 || QuadrantResources.Count == 0)
            {
                LOGmsg = String.Format("loadQuadrantResources called: Invalid input parameter = {0}", QuadrantNumber);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, LOGmsg);            
                return;
            }

            if (Carousel != null)
            {
                // hide all carousel items
                for (int i = 0; i < Carousel.Count; i++)
                {
                    if (Carousel[i] == null)
                        continue;
                    Carousel[i].show(false);
                    Carousel[i].showResource(false);
                }
            }

            // display appropriate carousel items,  change volumes to represent values
            // defined in protocol selection
            int nCarouselLocationIndex = 0;
            for (int i = 0; i < QuadrantResources[QuadrantNumber].Count; i++)
            {
                // show specific carousel component
                nCarouselLocationIndex = QuadrantResources[QuadrantNumber][i].carouselLocation;

                if (nCarouselLocationIndex >= 0 && nCarouselLocationIndex < Carousel.Count && Carousel[nCarouselLocationIndex] != null)
                {
                    try
                    {
                        if (eScreenStatus != enumResourcesScreenStatus.eEndOfRunCompleted)
                        {
                            // set image to light purple as viewed initially
                            Carousel[nCarouselLocationIndex].select(false);
                        }

                        // show item 
                        Carousel[nCarouselLocationIndex].show(true);

                        // don't move item 3 or 5 to front as they are large and cause overlay with other graphics
                        if (nCarouselLocationIndex != 0 && nCarouselLocationIndex != 7)
                        {
                            if (Carousel[nCarouselLocationIndex].picBox != null)                          
                                Carousel[nCarouselLocationIndex].picBox.BringToFront();
                        }   
                    }
                    catch (Exception ex)
                    {
                        // do nothing
                        LOGmsg = String.Format("loadQuadrantResources: Quadrant number(range 0 to 3) ={0}, nCarouselLocationIndex={1}, Exception = {2}", QuadrantNumber, nCarouselLocationIndex, ex.Message);
                        LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, LOGmsg);            
                    }
                }
            }
            
            // refresh list
            UpdateList(QuadrantNumber);

            // show highlighted item at end of run
            ShowHighLightItem(QuadrantNumber);

            if (eScreenStatus != enumResourcesScreenStatus.eEndOfRunCompleted)
            {
                // set first item on list to "viewed" 
                QuadrantResources[QuadrantNumber][0].viewed = true;
                if (QuadrantResources[QuadrantNumber][0].carouselLocation >= 0)
                {
                    // set current carousel location to location of first item on list
                    CurrentCarouselItem = QuadrantResources[QuadrantNumber][0].carouselLocation;
                }
            }

            UpdateLotIDText();

            // load carousel number image
            pictureCarouselNumber.Image = CarouselNumberImage[QuadrantNumber];
            pictureCarouselNumber.BringToFront();
        }

        private void ShowHighLightItem(int QuadrantNumber)
        {
            string LOGmsg;
            if (QuadrantNumber < QUADRANT1 || QuadrantNumber > QUADRANT4 || QuadrantResources.Count == 0)
            {
                LOGmsg = String.Format("ShowHighLightItem called: Invalid input parameter = {0}", QuadrantNumber);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, LOGmsg);            
                return;
            }

            EnableControls(true);

            if (myCloneRunConfig == null)
                return;

            if (eScreenStatus == enumResourcesScreenStatus.eEndOfRunCompleted)
            {
                Dictionary<string, string> dictItemDesc = new Dictionary<string, string>();

                bool bConfigExclusiveUseResultCheckForHighlight = false;

                string sTemp = GUIini.IniReadValue("System", "ExclusiveUseResultCheckForHighlight", "false");
                if (!string.IsNullOrEmpty(sTemp))
                {
                    if (sTemp.ToLower() == "true")
                    {
                        bConfigExclusiveUseResultCheckForHighlight = true;
                    }
                }
                if (!bConfigExclusiveUseResultCheckForHighlight)
                {
                    //
                    //  Old RoboSep way
                    //  use result checks if available
                    //

                    bool[] resultChecks;// = new string[8];/*CR*/
                    
                    int numQuadrants;
                    int nNumChecks = (int)ResultCheckOrder.Num_Of_ResultChecks;
                    ProtocolClass type;
                    ProtocolConsumable[,] consumables;
                    for (int i = 0; i < myCloneRunConfig.Length; i++)
                    {
                        if (myCloneRunConfig[i] == null || (myCloneRunConfig[i].InitQuadrant <= QuadrantNumber && QuadrantNumber < (myCloneRunConfig[i].InitQuadrant + myCloneRunConfig[i].Quadrants)) == false)
                            continue;

                        int nIndexBegin = (QuadrantNumber - myCloneRunConfig[i].InitQuadrant) * nNumChecks;
                        if (nIndexBegin < 0)
                            nIndexBegin = 0;

                        int nIndexEnd = nIndexBegin + nNumChecks;

                        type = RoboSep_RunSamples.getInstance().getProtocolClassType(myCloneRunConfig[i].Name);
                        SeparatorGateway.GetInstance().GetResultVialSelection(myCloneRunConfig[i].Name, out resultChecks, out numQuadrants);
                        if (resultChecks == null || resultChecks.Length < nIndexEnd)
                            continue;

                        bool isUseDefault = true;
                        for (int j = 0; j < resultChecks.Length; ++j)
                        {
                            if (resultChecks[j])
                            {
                                isUseDefault = false;
                            }
                        }
                        string sListItemDesc = String.Empty;
                        string sCarouselItemDesc = String.Empty;
                        consumables = myCloneRunConfig[i].Consumables;
                        if (isUseDefault)
                        {
                            switch (type)
                            {
                                case ProtocolClass.Positive:
                                case ProtocolClass.HumanPositive:
                                case ProtocolClass.MousePositive:
                                case ProtocolClass.WholeBloodPositive:
                                    sListItemDesc = LanguageINI.GetString("SeparationTube");
                                    sCarouselItemDesc = SeparationTube;
                                    break;
                                case ProtocolClass.Negative:
                                case ProtocolClass.HumanNegative:
                                case ProtocolClass.MouseNegative:
                                case ProtocolClass.WholeBloodNegative:
                                    if (consumables != null)
                                    {
                                        if (consumables.GetLength(0) < 2)
                                        {
                                            sListItemDesc = LanguageINI.GetString("NegativeFraction");
                                            sCarouselItemDesc = NegativeFractionTube;
                                        }
                                        else
                                        {
                                            if ((myCloneRunConfig[i].InitQuadrant + 1) == QuadrantNumber)
                                            {
                                                sListItemDesc = LanguageINI.GetString("SampleVial");
                                                sCarouselItemDesc = SampleVial;
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    sListItemDesc = LanguageINI.GetString("WasteFraction");
                                    sCarouselItemDesc = WasteTube;
                                    break;
                            }

                            if (!string.IsNullOrEmpty(sListItemDesc) && !string.IsNullOrEmpty(sCarouselItemDesc))
                            {
                                if (!dictItemDesc.ContainsKey(sCarouselItemDesc))
                                    dictItemDesc.Add(sCarouselItemDesc, sListItemDesc);
                            }
                        }
                        else
                        {
                            for (int j = nIndexBegin; j < nIndexEnd; j++)
                            {
                                if (resultChecks.Length <= j)
                                    break;

                                if (!resultChecks[j])
                                    continue;

                                int nIndex = j - nIndexBegin;
                                if (nIndex > nNumChecks)
                                    continue;

                                AddHighlightItems((ResultCheckOrder)nIndex, dictItemDesc);
                            }
                        }
                    }
                }
                else
                {
                    //
                    // Exclusively use result checks
                    //
                    int nNumChecks = (int)ResultCheckOrder.Num_Of_ResultChecks;
                    bool[] resultChecks;
                    int numQuadrants;
                    for (int i = 0; i < myCloneRunConfig.Length; i++)
                    {
                        if (myCloneRunConfig[i] == null || (myCloneRunConfig[i].InitQuadrant <= QuadrantNumber && QuadrantNumber < (myCloneRunConfig[i].InitQuadrant + myCloneRunConfig[i].Quadrants)) == false)
                            continue;

                        int nIndexBegin = (QuadrantNumber - myCloneRunConfig[i].InitQuadrant) * nNumChecks;
                        if (nIndexBegin < 0)
                            nIndexBegin = 0;

                        int nIndexEnd = nIndexBegin + nNumChecks;

                        SeparatorGateway.GetInstance().GetResultVialSelection(myCloneRunConfig[i].Name, out resultChecks, out numQuadrants);
                        if (resultChecks == null || resultChecks.Length < nIndexEnd)
                            continue;

                        for (int j = nIndexBegin; j < nIndexEnd; j++)
                        {
                            if (resultChecks.Length <= j)
                                break;

                            if (!resultChecks[j])
                                continue;

                            int nIndex = j - nIndexBegin;
                            if (nIndex > nNumChecks)
                                continue;

                            AddHighlightItems((ResultCheckOrder)nIndex, dictItemDesc);
                        }
                    }
                }

                // Highlight the end of run items
                SelectResourceItems(dictItemDesc);
       
                // disable controls
                EnableControls(false);
            }
        }

        private void AddHighlightItems(ResultCheckOrder nIndex, Dictionary<string, string> dictItemDesc)
        {
            if (dictItemDesc == null)
                return;
 
            string sListItemDesc = String.Empty;
            string sCarouselItemDesc = String.Empty;

            switch (nIndex)
            {
                case ResultCheckOrder.Buffer:
                    sListItemDesc = LanguageINI.GetString("BufferBottle");
                    sCarouselItemDesc = BufferBottle;
                    break;

                case ResultCheckOrder.Waste:
                    sListItemDesc = LanguageINI.GetString("WasteFraction");
                    sCarouselItemDesc = WasteTube;
                    break;

                case ResultCheckOrder.NegFraction:
                    sListItemDesc = LanguageINI.GetString("NegativeFraction");
                    sCarouselItemDesc = NegativeFractionTube;
                    break;

                case ResultCheckOrder.Cocktail:
                    sListItemDesc = LanguageINI.GetString("CockTailVial");
                    sCarouselItemDesc = NegativeSelectionVial;
                    break;

                case ResultCheckOrder.MagParticle:
                    sListItemDesc = LanguageINI.GetString("MagneticParticlesVial");
                    sCarouselItemDesc = MagneticParticleVial;
                    break;

                case ResultCheckOrder.Antibody:
                    sListItemDesc = LanguageINI.GetString("AntibodyVial");
                    sCarouselItemDesc = AntibodyVial;
                    break;

                case ResultCheckOrder.Sample:
                    sListItemDesc = LanguageINI.GetString("SampleVial");
                    sCarouselItemDesc = SampleVial;
                    break;

                case ResultCheckOrder.Separation:
                    sListItemDesc = LanguageINI.GetString("SeparationTube");
                    sCarouselItemDesc = SeparationTube;
                    break;

                default:
                    break;
            }

            if (!string.IsNullOrEmpty(sListItemDesc) && !string.IsNullOrEmpty(sCarouselItemDesc))
            {
                if (!dictItemDesc.ContainsKey(sCarouselItemDesc))
                    dictItemDesc.Add(sCarouselItemDesc, sListItemDesc);
            }
        }

        private void SelectResourceItems(Dictionary<string, string> dictItemDesc)
        {
            if (dictItemDesc == null || Carousel == null || listView_Resources.Items.Count == 0)
                return;

            // reset the display
            for (int i = 0; i < Carousel.Count; i++)
            {
                Carousel[i].showResource(false);
                Carousel[i].Reset();
            }

            listView_Resources.MultiSelect = true;

            int index  = 0;
            foreach (KeyValuePair<string, string> kvp in dictItemDesc)
            {
                index = Carousel.FindIndex(0, x => { return (x != null && !string.IsNullOrEmpty(x.sName) && x.sName.ToLower() == kvp.Key.ToLower()); });
                if (0 <= index)
                {
                    // Highlight the appropriate item
                    ListViewItem lvItem = listView_Resources.FindItemWithText(kvp.Value);
                    if (lvItem != null)
                    {
                        this.listView_Resources.ItemSelectionChanged -= evhItemSelectionChanged;
                        lvItem.Selected = true;
                        listView_Resources.EnsureVisible(lvItem.Index);
                        this.listView_Resources.ItemSelectionChanged += evhItemSelectionChanged;
                        this.listView_Resources.Enabled = false;

                        // update the scroll buttons
                        UpdateScrollButtonsState();

                        // Update the description
                        label_Description.Text = dictDescription.ContainsKey(Carousel[index].sName) ? dictDescription[Carousel[index].sName] : string.Empty;

                        Carousel[index].showHighLight();
                        Carousel[index].showResource(true);
                    }
                }
            }

            if (this.listView_Resources.SelectedItems.Count == 1)
            {
                // Can only display one hand draw item
                ListViewItem lvItem = listView_Resources.SelectedItems[0];
                if (0 <= currentQuadrant && currentQuadrant < QuadrantResources.Count)
                {
                    CurrentCarouselItem = QuadrantResources[currentQuadrant][lvItem.Index].carouselLocation;
                }
            }
            else
            {
                // Hide other controls if there are more than one highlighted items
                for (int i = 0; i < Carousel.Count; i++)
                {
                    Carousel[i].showResource(false);
                }
                label_Description.Text = string.Empty;
                txtBoxLotID.Visible = false;

                CurrentCarouselItem = (int)lotLocationRef.Empty;
            }
        }

        private void SelectFractionTube(string sTubeName, string sDescription)
        {
            if (string.IsNullOrEmpty(sTubeName) || string.IsNullOrEmpty(sDescription) || Carousel == null)
                return;

            // reset the display
            for (int i = 0; i < Carousel.Count; i++)
            {
                Carousel[i].Reset();
            }

            int index = Carousel.FindIndex(0, x => { return (x != null && !string.IsNullOrEmpty(x.sName) && x.sName.ToLower() == sTubeName.ToLower()); });
            if (0 <= index)
            {
                // Highlight the appropriate item
                if (listView_Resources.Items.Count > 0)
                {
                    ListViewItem lvItem = listView_Resources.FindItemWithText(sDescription);
                    if (lvItem != null)
                    {
                        this.listView_Resources.ItemSelectionChanged -= evhItemSelectionChanged;
                        lvItem.Selected = true;
                        listView_Resources.EnsureVisible(lvItem.Index);
                        this.listView_Resources.ItemSelectionChanged += evhItemSelectionChanged;
                        this.listView_Resources.Enabled = false;

                        // update the scroll buttons
                        UpdateScrollButtonsState();

                        // Update the description
                        label_Description.Text = dictDescription.ContainsKey(Carousel[index].sName) ? dictDescription[Carousel[index].sName] : string.Empty;
                    }
                }
                Carousel[index].showHighLight();
                Carousel[index].showResource(true);
            }
        }

        private void EnableControls(bool bEnable)
        {
            // connect/disconnect the event handlers
            ConnectCarouselItemEventHandler(bEnable);
            buttonRun.Visible  = bEnable ? true : false;

            if (!RoboSep_UserDB.getInstance().EnableAutoScanBarcodes)
            {
                button_Scan.Visible = bEnable ? true : false;
            }
            txtBoxLotID.Enabled = bEnable;
            this.listView_Resources.MultiSelect = false;
            this.listView_Resources.Enabled = bEnable;
        }

        private void UpdateProtocoNameText(int quadrant)
        {
            if (quadrant < QUADRANT1 || quadrant > QUADRANT4 || dictQuadrantRunInfo == null)
            {
                return;
            }

            // update the protocol name in the label
            if (dictQuadrantRunInfo.ContainsKey((QuadrantId)quadrant))
            {
                label_protocolName.Text = dictQuadrantRunInfo[(QuadrantId)quadrant].QuadrantLabel;
            }
        }

        // update list view
        private void UpdateList(int QuadrantNumber)
        {
            if (QuadrantNumber < QUADRANT1 || QuadrantNumber > QUADRANT4)
            {
                string LOGmsg = String.Format("UpdateList called. Invalid input parameter={0}", QuadrantNumber);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, LOGmsg);            
                return;
            }

            int imgIndex = 0, len = myCloneRunConfig.Length, offset;
            int carouselLocation, qdrtIndex = 0;
            double vol;
            string volText;
            SharingProtocol thisProtocol = null;
            for (qdrtIndex = 0; qdrtIndex < len; qdrtIndex++)
            {
                thisProtocol = myCloneRunConfig[qdrtIndex];                
                if (myCloneRunConfig[qdrtIndex].InitQuadrant == QuadrantNumber)
                {
                    break;
                }
            }

            listView_Resources.BeginUpdate();
            
            this.imageList1.Images.Clear();
            listView_Resources.Items.Clear();
                         
            for (int i = 0; i < QuadrantResources[QuadrantNumber].Count; i++)
            {
                RoboSep_Protocol_VialBarcodes barcodes = null;
                carouselLocation = QuadrantResources[QuadrantNumber][i].carouselLocation;
                IProtocol currentProtocol = SeparatorGateway.GetInstance().GetProtocol((QuadrantId)QuadrantNumber);
                if (currentProtocol != null)
                {
                    int relativeQuadrant = (int)QuadrantNumber - (int)currentProtocol.InitialQuadrant;
                    barcodes = RoboSep_UserDB.getInstance().getProtocolAssignedVialBarcodes(currentProtocol.Label, relativeQuadrant);
                }
                vol = QuadrantResources[QuadrantNumber][i].volume;
                volText = "";
                if (vol > 0)
                {
                    volText = string.Format("> {0} mL", vol);
                }

                string strDescription;
                offset = (thisProtocol.Quadrants == 1) ? 0 : (QuadrantNumber * 8);
                switch (carouselLocation)
                {
                    case 0:
                        // Magnetic particles
                        this.imageList1.Images.Add("Particles", bmParticles);
                        if (string.IsNullOrEmpty(thisProtocol.CustomNames[4 + offset]))
                        {
                            strDescription = LanguageINI.GetString("MagneticParticlesVial");
                        }
                        else
                        {
                            strDescription = thisProtocol.CustomNames[4 + offset];
                        }
                        if (!string.IsNullOrEmpty(volText))
                        {
                            strDescription += "\n";
                            strDescription += volText;
                        }
                        if (barcodes != null)
                        {
                            strDescription += "\n";
                            int length = barcodes.triangleVialBarcode.Length;
                            if (barcodes.triangleVialBarcode.Trim().Length > 0)
                                strDescription += barcodes.triangleVialBarcode.Trim();
                            else
                                strDescription += CUSTOM_LABEL;
                        }
                        ListViewAddItem(strDescription, 0, imgIndex);
                        imgIndex++;
                        break;
                    case 1:
                        // Negative selection vial
                        this.imageList1.Images.Add("CockTail", bmCocktail);
                        if (string.IsNullOrEmpty(thisProtocol.CustomNames[3 + offset]))
                        {
                            strDescription = LanguageINI.GetString("CockTailVial");
                        }
                        else
                        {
                            strDescription = thisProtocol.CustomNames[3 + offset];
                        }
                        if (!string.IsNullOrEmpty(volText))
                        {
                            strDescription += "\n";
                            strDescription += volText;
                        }
                        if (barcodes != null)
                        {
                            strDescription += "\n";
                            if (barcodes.squareVialBarcode.Trim().Length > 0)
                                strDescription += barcodes.squareVialBarcode.Trim();
                            else
                                strDescription += CUSTOM_LABEL;
                        }
                        ListViewAddItem(strDescription, 1, imgIndex);
                        imgIndex++;
                        break;
                    case 2:
                        // Antibody vial
                        this.imageList1.Images.Add("AntibodyVial", bmAntibodyVial);
                        if (string.IsNullOrEmpty(thisProtocol.CustomNames[5 + offset]))
                        {
                            strDescription = LanguageINI.GetString("AntibodyVial");
                        }
                        else
                        {
                            strDescription = thisProtocol.CustomNames[5 + offset];
                        }
                        if (!string.IsNullOrEmpty(volText))
                        {
                            strDescription += "\n";
                            strDescription += volText;
                        }
                        if (barcodes != null)
                        {
                            strDescription += "\n";
                            if (barcodes.circleVialBarcode.Trim().Length > 0)
                                strDescription += barcodes.circleVialBarcode.Trim();
                            else
                                strDescription += CUSTOM_LABEL;
                        }
                        ListViewAddItem(strDescription, 2, imgIndex);
                        imgIndex++;
                        break;
                    case 3:
                        // Sample vial
                        this.imageList1.Images.Add("SampleVial", bmSampleVial);
                        strDescription = LanguageINI.GetString("SampleVial");
                        if (!string.IsNullOrEmpty(volText))
                        {
                            volText = string.Format("{0} mL", vol);
                            strDescription += "\n";
                            strDescription += volText;
                        }
                        ListViewAddItem(strDescription, 3, imgIndex);
                        imgIndex++;
                        break;
                    case 4:
                        // Tip rack
                        this.imageList1.Images.Add("TipRack", bmTipRack);
                        strDescription = LanguageINI.GetString("TipRack");
                        if (!string.IsNullOrEmpty(volText))
                        {
                            strDescription += "\n";
                            strDescription += volText;
                        }
                        ListViewAddItem(strDescription, 4, imgIndex);
                        imgIndex++;
                        break;
                    case 5:
                        // Separation tube
                        this.imageList1.Images.Add("SeparationTube", bmSeparationTube);
                        strDescription = LanguageINI.GetString("SeparationTube");
                        if (!string.IsNullOrEmpty(volText))
                        {
                            strDescription += "\n";
                            strDescription += volText;
                        }
                        ListViewAddItem(strDescription, 5, imgIndex);
                        imgIndex++;
                        break;
                    case 6:
                        // waste tube
                        this.imageList1.Images.Add("WasteFraction", bmWasteFraction);
                        strDescription = LanguageINI.GetString("WasteFraction");
                        if (!string.IsNullOrEmpty(volText))
                        {
                            strDescription += "\n";
                            strDescription += volText;
                        }
                        ListViewAddItem(strDescription, 6, imgIndex);
                        imgIndex++;
                        break;
                    case 7:
                        // Negative fraction tube, don't show volume
                        this.imageList1.Images.Add("NegativeFraction", bmNegativeFraction);
                        strDescription = LanguageINI.GetString("NegativeFraction");
                        ListViewAddItem(strDescription, 7, imgIndex);
                        imgIndex++;
                        break;
                    case 8:
                        // buffer bottle
                        this.imageList1.Images.Add("BufferBottle", bmBufferBottle);
                        strDescription = LanguageINI.GetString("BufferBottle");
                        if (!string.IsNullOrEmpty(volText))
                        {
                            strDescription += "\n";
                            strDescription += volText;
                        }
                        ListViewAddItem(strDescription, 8, imgIndex);
                        imgIndex++;
                        break;
                    default:
                        break;
                }
            }
            if (listView_Resources.Items.Count > 0)
            {
                listView_Resources.SizeLastColumn();
                listView_Resources.ResizeVerticalHeight(false);
            }

            listView_Resources.EndUpdate();
        }
        private void ListViewAddItem(string volText, int carouselLocation, int imgIndex)
        {
            ListViewItem lvItem = new ListViewItem();
            lvItem.Text = volText;
            lvItem.Tag = imgIndex;
            lvItem.ImageIndex = imgIndex;
            listView_Resources.Items.Add(lvItem);
        }

        private void ShowItemInList(int locationInList)
        {
            if (listView_Resources.Items.Count == 0)
                return;

            foreach (ListViewItem lvItem in listView_Resources.Items)
            {
                int index = (int)lvItem.Tag;
                if (index < 0)
                    continue;

                if (index == locationInList)
                {
                    lvItem.Selected = true;
                    listView_Resources.EnsureVisible(lvItem.Index);
                    break;
                }
            }

            // update the scroll buttons
            UpdateScrollButtonsState();
        }
       
        #region Control Events

        private void CarouselItem0_Click(object sender, EventArgs e)
        { CarouselItemClick(0); }
        private void CarouselItem1_Click(object sender, EventArgs e)
        { CarouselItemClick(1); }
        private void CarouselItem2_Click(object sender, EventArgs e)
        { CarouselItemClick(2); }
        private void CarouselItem3_Click(object sender, EventArgs e)
        { CarouselItemClick(3); }
        private void CarouselItem4_Click(object sender, EventArgs e)
        { CarouselItemClick(4); }
        private void CarouselItem5_Click(object sender, EventArgs e)
        { CarouselItemClick(5); }
        private void CarouselItem6_Click(object sender, EventArgs e)
        { CarouselItemClick(6); }
        private void CarouselItem7_Click(object sender, EventArgs e)
        { CarouselItemClick(7); }
        private void CarouselItem8_Click(object sender, EventArgs e)
        { CarouselItemClick(8); }
        private void CarouselItemClick(int itemNumber)
        {
            // change selected item on carousel side
            if ( 0 <= CurrentCarouselItem && CurrentCarouselItem < Carousel.Count)
                Carousel[CurrentCarouselItem].select(false);
         
            if ( 0 <= itemNumber && itemNumber < Carousel.Count)
                Carousel[itemNumber].select(true);   
         
            CurrentCarouselItem = itemNumber;

            // update lot id text that corresponds to item
            UpdateLotIDText();
            
            // go to item in button list
            // search for item in resource list
            int locationInList = 0;
            for (int i = 0; i < QuadrantResources[currentQuadrant].Count; i++)
            {
                if (QuadrantResources[currentQuadrant][i].carouselLocation == itemNumber)
                {
                    locationInList = i;
                    break;
                }
            }

            QuadrantResources[currentQuadrant][locationInList].viewed = true;

            // Show item in list 
            ShowItemInList(locationInList);

        }

        private void btn_home_Click(object sender, EventArgs e)
        {
            // unload progress bar
            // cancling takes user back to run samples window
            // clicking the run button will re-add wanted protocols
            RoboSep_RunProgress.getInstance().Unload( this );

            // reset flag to initialized
            eScreenStatus = enumResourcesScreenStatus.eInitialized;

            // Set listview to single selection
            listView_Resources.MultiSelect = false;

            RoboSep_RunSamples SamplingWindow = RoboSep_RunSamples.getInstance();
            SamplingWindow.Location = new Point(0, 0);
            SamplingWindow.Visible = true;
            this.Visible = false;
            RoboSep_UserConsole.getInstance().Controls.Add(SamplingWindow);
            RoboSep_UserConsole.getInstance().Controls.Remove(RoboSep_UserConsole.ctrlCurrentUserControl);
            RoboSep_UserConsole.ctrlCurrentUserControl = SamplingWindow;

            // LOG
            string LOGmsg = "Cancel button clicked";
            //GUI_Controls.uiLog.LOG(this, "button_Cancel1_Click", GUI_Controls.uiLog.LogLevel.DEBUG, LOGmsg);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, LOGmsg);            
        }

        private void ListItem_Select(int buttonNumber)
        {
            this.SuspendLayout();

            // turn off previously selected item
            if (0 <= CurrentCarouselItem && CurrentCarouselItem < Carousel.Count)
                Carousel[CurrentCarouselItem].select(false);
 
            // set button to "has been viewed"
            QuadrantResources[currentQuadrant][buttonNumber].viewed = true;
            if (QuadrantResources[currentQuadrant][buttonNumber].carouselLocation >= 0)
            {
                // set current item to "currentCarouselItem" and display it
                CurrentCarouselItem = QuadrantResources[currentQuadrant][buttonNumber].carouselLocation;
                Carousel[CurrentCarouselItem].select(true);
            }
            else
            {
                CurrentCarouselItem = 8;
                Carousel[CurrentCarouselItem].select(true);
            }
            if (0 <= CurrentCarouselItem && CurrentCarouselItem < Carousel.Count)
                label_Description.Text = dictDescription.ContainsKey(Carousel[CurrentCarouselItem].sName) ? dictDescription[Carousel[CurrentCarouselItem].sName] : string.Empty;

            // update Lot ID text
            UpdateLotIDText();

            this.ResumeLayout(true);
        }

        enum lotLocationRef
        {    
            Empty    = -1,
            Particle = 0,
            Cocktail = 1,
            Antibody = 2,
            Sample   = 3,
            Buffer   = 8
        }

        private void txtBoxLotID_TextChanged(object sender, EventArgs e)
        {
            // get correct reagent lot to modify
            int lotRef = lotIdRef[currentQuadrant];
            if (lotRef < 0)
                return;

            bool bSharedBufferLotID = true;
            string sTemp = GUIini.IniReadValue("System", "SharedBufferLotID", "true");
            if (sTemp.Trim().ToLower() == "true")
            {
                bSharedBufferLotID = true;
            }
            else
            {
                bSharedBufferLotID = false;
            }

            string updateLotID = txtBoxLotID.Text;

            ReagentLotIdentifiers1 tempReagentLotIds1 = getReagentLotIdentifiers1ByQuadNumber(currentQuadrant);
            if (tempReagentLotIds1 == null)
                return;
            ReagentLotIdentifiers tempLotID = tempReagentLotIds1.LotIDs;

            switch (CurrentCarouselItem)
            {
                case (int)lotLocationRef.Sample:
                    tempLotID.mySampleLabel = updateLotID;
                    break;
                case (int)lotLocationRef.Cocktail:
                    tempLotID.myCocktailLabel = updateLotID;
                    break;
                case (int)lotLocationRef.Particle:
                    tempLotID.myParticleLabel = updateLotID;
                    break;
                case (int)lotLocationRef.Antibody:
                    tempLotID.myAntibodyLabel = updateLotID;
                    break;
                case (int)lotLocationRef.Buffer:
                    if (bSharedBufferLotID)
                    {
                        UpdateAllBufferLotIDs(updateLotID);
                        return; //return here so updateReagentLotIdByQuadNumber does overwrite
                    }
                    else
                    {
                        tempLotID.myBufferLabel = updateLotID;
                    }
                    break;
                default:
                    break;
            }
            // update specified lot id
            updateReagentLotIdByQuadNumber(currentQuadrant, tempLotID);
        }

        private ReagentLotIdentifiers1 getReagentLotIdentifiers1ByQuadNumber(int quad)
        {
            ReagentLotIdentifiers1 result = null;
            for (int i = 0; i < myReagentLotIDs1.Length; i++)
            {
                if ((int)myReagentLotIDs1[i].QId == quad)
                {
                    result = myReagentLotIDs1[i];
                    break;
                }
            }
            return result;
        }

        private void updateReagentLotIdByQuadNumber(int quad, ReagentLotIdentifiers LotIDs)
        {
            ReagentLotIdentifiers1 reagentLotIDs1 = getReagentLotIdentifiers1ByQuadNumber(quad);
            if (reagentLotIDs1 != null)
                reagentLotIDs1.LotIDs = LotIDs;
            /*for (int i = 0; i < myReagentLotIDs1.Length; i++)
            {
                if ((int)myReagentLotIDs1[i].QId == quad)
                {
                    myReagentLotIDs1[i].LotIDs = LotIDs;
                    break;
                }
            }*/
        }

        private void updateReagentLotIdVialRequirementByQuadNumber(int quad, bool isRequired, int vialType)
        {
            ReagentLotIdentifiers1 reagentLotIDs1 = getReagentLotIdentifiers1ByQuadNumber(quad);
            if (reagentLotIDs1 != null)
            {
                switch (vialType)
                {
                    case 1:
                        reagentLotIDs1.bRequireAntibodyLabel = isRequired;
                        break;
                    case 2:
                        reagentLotIDs1.bRequireCocktailLabel = isRequired;
                        break;
                    case 3:
                        reagentLotIDs1.bRequireParticleLabel = isRequired;
                        break;
                    case 4:
                        reagentLotIDs1.bRequireSampleLabel= isRequired;
                        break;
                }
            }
        }

        private void UpdateLotIDText()
        {
            // get correct reagent lot to modify
            int lotRef = lotIdRef[currentQuadrant];
            if (lotRef < 0)
                return;

            ReagentLotIdentifiers1 tempReagentLotIds1 = getReagentLotIdentifiers1ByQuadNumber(currentQuadrant);
            if (tempReagentLotIds1 == null)
                return;
            ReagentLotIdentifiers tempLotID = tempReagentLotIds1.LotIDs;
            lblLotID.Text = LanguageINI.GetString("lblReagentLotID");
            switch (CurrentCarouselItem)
            {
                case (int)lotLocationRef.Sample:
                    txtBoxLotID.Visible = true;
                    txtBoxLotID.Text = tempLotID.mySampleLabel;
                    lblLotID.Text = LanguageINI.GetString("lblSampleID");
                    break;
                case (int)lotLocationRef.Cocktail:
                    txtBoxLotID.Visible = true;
                    txtBoxLotID.Text = tempLotID.myCocktailLabel;
                    break;
                case (int)lotLocationRef.Particle:
                    txtBoxLotID.Visible = true;
                    txtBoxLotID.Text = tempLotID.myParticleLabel;
                    break;
                case (int)lotLocationRef.Antibody:
                    txtBoxLotID.Visible = true;
                    txtBoxLotID.Text = tempLotID.myAntibodyLabel;
                    break;

                case (int)lotLocationRef.Buffer:
                    txtBoxLotID.Visible = true;
                    txtBoxLotID.Text = tempLotID.myBufferLabel;
                    lblLotID.Text = LanguageINI.GetString("lblBufferLotID");
                    break;

                case (int)lotLocationRef.Empty:
                default:
                    {
                        txtBoxLotID.Visible = false;
                        txtBoxLotID.Text = string.Empty;
                        break;
                    }
            }
        }

        private void button_Quadrant1_Click(object sender, EventArgs e)
        { Quadrant_Click(QUADRANT1); }
        private void button_Quadrant2_Click(object sender, EventArgs e)
        { Quadrant_Click(QUADRANT2); }
        private void button_Quadrant3_Click(object sender, EventArgs e)
        { Quadrant_Click(QUADRANT3); }
        private void button_Quadrant4_Click(object sender, EventArgs e)
        { Quadrant_Click(QUADRANT4); }
        private void Quadrant_Click(int newQuadrant)
        {
            if (newQuadrant < QUADRANT1 || newQuadrant > QUADRANT4)
            {
                // LOG
                string LOGmsg = String.Format("Quadrant_Click called. Invalid input parameter={0}", newQuadrant);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, LOGmsg);            
                return;
            }

            currentQuadrant = newQuadrant;
            for (int i = 0; i < 4; i++)
            {
                QuadrantsSelect[i].Check = false;
                if (i == currentQuadrant)
                {
                    QuadrantsSelect[i].Check = true;
                    UpdateProtocoNameText(i);
                }
            }

            // change selection to initial item
            if (0 <= CurrentCarouselItem && CurrentCarouselItem < Carousel.Count && CurrentCarouselItem != 8)
                Carousel[CurrentCarouselItem].select(false);

            for (int i = 0; i < Carousel.Count-1; i++)
            {
                Carousel[i].select(false);

                // reset picture box graphics
                Carousel[i].Reset();
            }

            Carousel[Carousel.Count - 1].select(false);

            loadQuadrantResources(currentQuadrant);

            if (eScreenStatus != enumResourcesScreenStatus.eEndOfRunCompleted)
            {
                // set flag to indicate that this quadrant has been viewed
                if (dictQuadrantRunInfo.ContainsKey((QuadrantId)currentQuadrant))
                {
                    QuadrantRunInfo qinfo = dictQuadrantRunInfo[(QuadrantId)currentQuadrant];
                    qinfo.Viewed = true;
                }

                // select top item
                ListItem_Select(0);

                if (listView_Resources.Items.Count > 0)
                {
                    listView_Resources.Items[0].Selected = true;
                    listView_Resources.EnsureVisible(listView_Resources.Items[0].Index);
                }

                // check if there is any unviewed quadrant
                if (HasUnviewedQuadrant() == false)
                {
                    // No unviewed quadrants, show the green 'Play' button
                    buttonRun.ChangeGraphics(iListRun2);
                }
            }

            // update the scroll buttons
            UpdateScrollButtonsState();
        }

        private bool ConfirmLidClosed()
        {
            bool isLidClosed = Tesla.OperatorConsoleControls.SeparatorGateway.GetInstance().IsLidClosed();
            return isLidClosed;
        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            // advance to next quadrant that has not been viewed
            int nCount = 0;
            int nextQuadrant = -1;
            foreach (KeyValuePair<QuadrantId, QuadrantRunInfo> kvp in dictQuadrantRunInfo)
            {
                if (kvp.Value.Viewed == true)
                    continue;

                switch ((int)kvp.Key)
                {
                    case 0:
                        if (nextQuadrant == -1)
                        {
                            nextQuadrant = QUADRANT1;
                        }
                        break;
                    case 1:
                        if (nextQuadrant == -1)
                        {
                            nextQuadrant = QUADRANT2;
                        }
                        break;
                    case 2:
                        if (nextQuadrant == -1)
                        {
                            nextQuadrant = QUADRANT3;
                        }
                        break;
                    case 3:
                        if (nextQuadrant == -1)
                        {
                            nextQuadrant = QUADRANT4;
                        }
                        break;
                    default:
                        break;
                }

                if (QUADRANT1 <= nextQuadrant && nextQuadrant <= QUADRANT4)
                {
                    nCount++;
                }

            }
            if (QUADRANT1 <= nextQuadrant && nextQuadrant <= QUADRANT4)
            {
                // update the color of the button to green if there is only one unviewed quadrant
                if (nCount == 1)
                {
                    buttonRun.ChangeGraphics(iListRun2);
                }
                Quadrant_Click(nextQuadrant);
                return;
            }

            if (IsLidClosed())
            {
                // Show barcode UI if configured
                if (RoboSep_UserDB.getInstance().EnableAutoScanBarcodes && HasMaintenanceProtocol() == false)
                {
                    ShowBarcodeWindow();
                }

                // Check free disk spaces
                if (CheckFreeDiskSpaces() == false)
                    return;

                UpdateReagentLotIDs();
                RunResources();
            }
        }


        private bool CheckFreeDiskSpaces()
        {
            bool bRet = true;

            // Check free disk space
            int nIndex = 0;
            string systemPath = RoboSep_UserDB.getInstance().sysPath;
            if (!string.IsNullOrEmpty(systemPath))
            {
                nIndex = systemPath.IndexOf(':');
                if (0 <= nIndex)
                {
                    double HDDLowMarkInMB = HDD_LOW_MARK;
                    string sTemp = GUIini.IniReadValue("System", "DefaultHDDLowMarkInMegaByte", "100.00");
                    if (!Double.TryParse(sTemp.Trim(), out HDDLowMarkInMB))
                    {
                        HDDLowMarkInMB = HDD_LOW_MARK;
                        string errMsg = String.Format("Invalid DefaultHDDLowMarkInMegaByte value '{0}' in GUI.ini file.", sTemp);
                        LogFile.AddMessage(System.Diagnostics.TraceLevel.Warning, errMsg);
                    }

                    string drive = systemPath.Substring(0, nIndex + 1);
                    drive += "\\";
                    double dFreeDiskSpace = RoboSep_UserDB.getInstance().GetFreeHDDSizeInMB(drive);
                    if (dFreeDiskSpace < HDDLowMarkInMB)
                    {
                        // Show warning message
                        // prompt user that file copy has failed
                        string sTitle = LanguageINI.GetString("headerLowDiskSpace");
                        sTemp = LanguageINI.GetString("msgLowDiskSpace");
                        string sMsg = String.Format(sTemp, drive, dFreeDiskSpace.ToString());

                        RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING, sMsg, sTitle, LanguageINI.GetString("Continue"), LanguageINI.GetString("Cancel"));
                        RoboSep_UserConsole.showOverlay();
                        //prompt user
                        prompt.ShowDialog();
                        RoboSep_UserConsole.hideOverlay();
                        if (prompt.DialogResult != DialogResult.OK)
                        {
                            bRet = false;
                        }

                        // LOG
                        string logMSG = String.Format("Free disk space '{0} is running low.", dFreeDiskSpace.ToString());
                        LogFile.AddMessage(System.Diagnostics.TraceLevel.Warning, logMSG);
                        prompt.Dispose();                        
                    }
                }
            }
            return bRet;
        }

        private void UpdateReagentLotIDs()
        {
            int lotRef = -1;
            for (int i = 0; i < lotIdRef.Length; i++)
            {
                lotRef = lotIdRef[i];
                if (lotRef < 0)
                    continue;

                CheckVialsRequired(i);
            }
        }

        private void CheckVialsRequired(int lotRef)
        {
            ReagentLotIdentifiers1 reagentLotIds1 = getReagentLotIdentifiers1ByQuadNumber(lotRef);
            if (reagentLotIds1 != null)
            {
                QuadrantId Qid = reagentLotIds1.QId;
                updateReagentLotIdVialRequirementByQuadNumber((int)Qid, IsScanVialRequired(Qid, 1), 1);
                updateReagentLotIdVialRequirementByQuadNumber((int)Qid, IsScanVialRequired(Qid, 2), 2);
                updateReagentLotIdVialRequirementByQuadNumber((int)Qid, IsScanVialRequired(Qid, 3), 3);
                updateReagentLotIdVialRequirementByQuadNumber((int)Qid, IsScanVialRequired(Qid, 4), 4);    // Based on arbitary assumption, need to be changed if barcode API changed
            }
        }

        private string GetEmptyLotIdDisplayString(int lotRef)
        {
            if (lotRef < 0 || lotRef > 3)
                return null;

            string displayString = "";
            // use the first element in the samplelabels array
            if (myReagentLotIDs1[lotRef].bRequireSampleLabel && string.IsNullOrEmpty(myReagentLotIDs1[lotRef].LotIDs.mySampleLabel))
            {
                displayString = GetVialDisplayString(4);
            }
            else if (myReagentLotIDs1[lotRef].bRequireParticleLabel && string.IsNullOrEmpty(myReagentLotIDs1[lotRef].LotIDs.myParticleLabel))
            {
                displayString = GetVialDisplayString(3);
            }
            else if (myReagentLotIDs1[lotRef].bRequireCocktailLabel && string.IsNullOrEmpty(myReagentLotIDs1[lotRef].LotIDs.myCocktailLabel))
            {
                displayString = GetVialDisplayString(2);
            }
            else if (myReagentLotIDs1[lotRef].bRequireAntibodyLabel && string.IsNullOrEmpty(myReagentLotIDs1[lotRef].LotIDs.myAntibodyLabel))
            {
                displayString = GetVialDisplayString(1);
            }
            return displayString;
        }


        private void ShowReagentEmptyLotIDMessage(int lotRef)
        {
            if (lotRef < 0)
                return;

            QuadrantId QId = myReagentLotIDs1[lotRef].QId;
            if (QId < QuadrantId.Quadrant1 || QId > QuadrantId.Quadrant4)
                return;

 
            int nQuadrant = -1;
            switch (QId)
            {
                case QuadrantId.Quadrant1:
                    nQuadrant = QUADRANT1;
                    break;
                case QuadrantId.Quadrant2:
                    nQuadrant = QUADRANT2;
                    break;
                case QuadrantId.Quadrant3:
                    nQuadrant = QUADRANT3;
                    break;
                case QuadrantId.Quadrant4:
                    nQuadrant = QUADRANT4;
                    break;
                default:
                    break;
            }

            string msgHeader, msg, msgFormat;
            string vialDesc = "";
            msgHeader = LanguageINI.GetString("headerReagentLotID");
            if (myReagentLotIDs1[lotRef].bRequireParticleLabel && string.IsNullOrEmpty(myReagentLotIDs1[lotRef].LotIDs.myParticleLabel))
            {
                vialDesc = GetVialDisplayString(3);
                msgFormat = LanguageINI.GetString("ReagentLotIDMissing1");
                msg = String.Format(msgFormat, vialDesc, (int)QId + 1, vialDesc);
                DisplayEmptyBarcodeMessage(msgHeader, msg, ref myReagentLotIDs1[lotRef].LotIDs.myParticleLabel, ref myReagentLotIDs1[lotRef].bSkipParticleLabel);
            }
            
            if (myReagentLotIDs1[lotRef].bRequireCocktailLabel && string.IsNullOrEmpty(myReagentLotIDs1[lotRef].LotIDs.myCocktailLabel))
            {
                vialDesc = GetVialDisplayString(2);
                msgFormat = LanguageINI.GetString("ReagentLotIDMissing1");
                msg = String.Format(msgFormat, vialDesc, (int)QId + 1, vialDesc);
                DisplayEmptyBarcodeMessage(msgHeader, msg, ref myReagentLotIDs1[lotRef].LotIDs.myCocktailLabel, ref myReagentLotIDs1[lotRef].bSkipCocktailLabel);
            }
            
            if (myReagentLotIDs1[lotRef].bRequireAntibodyLabel && string.IsNullOrEmpty(myReagentLotIDs1[lotRef].LotIDs.myAntibodyLabel))
            {
                vialDesc = GetVialDisplayString(1);
                msgFormat = LanguageINI.GetString("ReagentLotIDMissing1");
                msg = String.Format(msgFormat, vialDesc, (int)QId + 1, vialDesc);
                DisplayEmptyBarcodeMessage(msgHeader, msg, ref myReagentLotIDs1[lotRef].LotIDs.myAntibodyLabel, ref myReagentLotIDs1[lotRef].bSkipAntibodyLabel);
            }
  
            if (QUADRANT1 <= nQuadrant && nQuadrant <= QUADRANT4)
                Quadrant_Click(nQuadrant); 

            return;
        }

        private void ShowSampleEmptyLotIDMessage(int lotRef)
        {
            if (lotRef < 0)
                return;

            QuadrantId QId = myReagentLotIDs1[lotRef].QId;
            if (QId < QuadrantId.Quadrant1 || QId > QuadrantId.Quadrant4)
                return;

            int nAbsoluteQuadrant = -1;
            switch (QId)
            {
                case QuadrantId.Quadrant1:
                    nAbsoluteQuadrant = QUADRANT1;
                    break;
                case QuadrantId.Quadrant2:
                    nAbsoluteQuadrant = QUADRANT2;
                    break;
                case QuadrantId.Quadrant3:
                    nAbsoluteQuadrant = QUADRANT3;
                    break;
                case QuadrantId.Quadrant4:
                    nAbsoluteQuadrant = QUADRANT4;
                    break;
                default:
                    break;
            }
            string msgHeader, msg, msgFormat;

            msgHeader = LanguageINI.GetString("headerEmptySampleLotID");
            msgFormat = LanguageINI.GetString("SampleVialLotIDEmpty");
            msg = String.Format(msgFormat, (int)QId + 1);
            DisplayEmptyBarcodeMessage(msgHeader, msg, ref myReagentLotIDs1[lotRef].LotIDs.mySampleLabel, ref myReagentLotIDs1[lotRef].bSkipSampleLabel);
            if (QUADRANT1 <= nAbsoluteQuadrant && nAbsoluteQuadrant <= QUADRANT4)
                Quadrant_Click(nAbsoluteQuadrant);

            return;
        }

        private DialogResult DisplayEmptyBarcodeMessage(string msgHeader, string msg, ref string lotID, ref bool bSkip)
        {
            RoboMessagePanel3 prompt = new RoboMessagePanel3(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING, msg, msgHeader, LanguageINI.GetString("Ok"), 
                LanguageINI.GetString("Skip"), true, RoboSep_UserConsole.KeyboardEnabled, false);
            RoboSep_UserConsole.showOverlay();
            DialogResult result = prompt.ShowDialog();
            RoboSep_UserConsole.hideOverlay();

            if (result == DialogResult.OK)
            {
                lotID = prompt.EditBoxText;
            }
            else if (result == DialogResult.Cancel)
            {
                bSkip = true;
            }
            prompt.Dispose();
            return result;
        }

         private string GetVialDisplayString(int Vial)
        {
            string strDescription = "???";
            switch (Vial)
            {
                case 1: // VialC
                    strDescription = LanguageINI.GetString("AntibodyVial");
                    break;
                case 2:   // VialB
                    strDescription = LanguageINI.GetString("CockTailVial");
                    break;
                case 3:   // VialA
                    strDescription = LanguageINI.GetString("MagneticParticlesVial");
                    break;
                case 4:   // VialA
                    strDescription = LanguageINI.GetString("SampleVial");
                    break;
                default:
                    break;

            }
            return strDescription;
        }

        public bool IsScanVialRequired(QuadrantId QId, int Vial)
        {
            if (QId < QuadrantId.Quadrant1 || QId > QuadrantId.Quadrant4)
                return false;

            // Assumptions
            // 4 => Sample tube
            // All others based on barcode scanner API

            // Convert input vial to carouselLocation 
            // SampleTube, carouselLocation = 3
            // VialA, carouselLocation = 0
            // VialB, carouselLocation = 1            
            // VialC, carouselLocation = 2
            int carouselLocation = -1;
            switch (Vial)
            {
               case 1: // VialC
                    carouselLocation = 2;
                    break;
              case 2:   // VialB
                    carouselLocation = 1;
                    break;
              case 3:   // VialA
                    carouselLocation = 0;
                    break;
              case 4:   // Sample Tube, magic number 
                    carouselLocation = 3;
                    break;
              default:
                    break;
             }

            if (carouselLocation == -1)
                return false;

            int nIndex = (int)QId;
            bool bRequired = false;
            List<ResourceData> lstResourceData = QuadrantResources[nIndex];
            foreach (ResourceData tempResource in lstResourceData)
            {
                if (tempResource.carouselLocation == carouselLocation)
                {
                    if (string.IsNullOrEmpty(tempResource.LotID))
                    {
                        bRequired = true;
                        break;
                    }
                }
            }
            return bRequired;
        }

        public void RunResources()
        {
            // Check if refill needed
            if (!CheckHydraulicFluid())
            {
                 return;
            }

            // check first that lid is closed
            if (!ConfirmLidClosed())
            {   
                // Show resources screen if configured to be hidden.
                if (this.Visible == false)
                    this.Visible = true;

                string sMSG = LanguageINI.GetString("msgCloseLid");
                RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING, sMSG,
                    LanguageINI.GetString("headerCloseLid"), LanguageINI.GetString("lblResume"));
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                prompt.Dispose();
                RoboSep_UserConsole.hideOverlay();
            }
            else
            {
                string sMSG = LanguageINI.GetString("msgCarouselSetup");
                RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_QUESTION, 
                    sMSG, LanguageINI.GetString("headerStartRun"), LanguageINI.GetString("Yes"), LanguageINI.GetString("No"));
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                RoboSep_UserConsole.hideOverlay();
                if (prompt.DialogResult == DialogResult.OK)
                {
                    // update Lot IDs
                   // for (int i = 0; i < myReagentLotIDs.Length; i++)
                    /*for (int i = 0; i < myReagentLotIDs1.Length; i++)
                    {
                      //  Tesla.OperatorConsoleControls.SeparatorGateway.GetInstance().SetProtocolReagentLotIds(i, myReagentLotIDs[i]);

                       Tesla.OperatorConsoleControls.SeparatorGateway.GetInstance().SetProtocolReagentLotIds(i, myReagentLotIDs1[i].LotIDs);
                    }*/

                    // Reset the run button graphics
                    buttonRun.ChangeGraphics(iListRun1);

                    //set robosepUserconsole to "run Mode"
                    RoboSep_UserConsole.bIsRunning = true;

                    // launch run window and tell quadrants to begin progressing
                    RoboSep_RunProgress.getInstance().Visible = false;
                    RoboSep_RunProgress.getInstance().Location = new Point(0, 0);
                    RoboSep_UserConsole.getInstance().Controls.Add(RoboSep_RunProgress.getInstance());
                    RoboSep_UserConsole.getInstance().Controls.Remove(RoboSep_UserConsole.ctrlCurrentUserControl);
                    RoboSep_UserConsole.ctrlCurrentUserControl = RoboSep_RunProgress.getInstance();
                    RoboSep_RunProgress.getInstance().Visible = true;

                    // LOG
                    string LOGmsg = "Run Protocols button clicked";
                    //GUI_Controls.uiLog.LOG(this, "buttonRun_Click", GUI_Controls.uiLog.LogLevel.DEBUG, LOGmsg);
                    //  (LOGmsg);
                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);                
                    if (!RoboSep_UserConsole.bStartTimerEbnable)
                    {
                        bool bMaintenaceProtocol = false;
                        if (myCloneRunConfig != null && (myCloneRunConfig[0].Name == "Basic prime"))
                            bMaintenaceProtocol = true;

                        RoboSep_RunProgress.getInstance().Start(bMaintenaceProtocol);
                    }
                    else
                    {
                        // ask user for desired run time

                        // begin waiting for run time
                    }
                }
                else
                {
                    this.BringToFront();
                }
                prompt.Dispose();
            }
        }

        private bool CheckHydraulicFluid()
        {
            // May 12, 2015: from TW
            // use isHydraulicFluidLow from InstrumentControlProxy to get the real time fluid flag
            bool HydraulicLow = Tesla.InstrumentControl.InstrumentControlProxy.GetInstance().IsHydraulicFluidLow();
            //SeparatorGateway.GetInstance().IsHydraulidFluidRefillRequired;
            if (HydraulicLow)
            {
                // LOG
                string logMSG = "The hydraulic fluid level is low.";//LanguageINI.GetString("msgHydraulicLow");
                //  (logMSG);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                
                string sMSG = LanguageINI.GetString("msgHydraulicLow");
                sMSG = SeparatorResourceManager.GetSeparatorString(StringId.HydraulicFluidLevelWarning);
                RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING, sMSG,
                    LanguageINI.GetString("headedFluidLevel"), LanguageINI.GetString("Confirm"), LanguageINI.GetString("Ignore"));

                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                RoboSep_UserConsole.hideOverlay();

                if (SeparatorGateway.GetInstance().GetIgnoreHydraulicSensor() == true)
                {
                    if (prompt.DialogResult == DialogResult.OK)
                    {
                        SeparatorGateway.GetInstance().ConfirmHydraulicFluidRefilled();
                        // LOG
                        logMSG = "User confirmed fluid top up";
                        // GUI_Controls.uiLog.LOG(this, "CheckHydraulicFluid", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
                        //  (logMSG);
                        LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                
                        prompt.Dispose();
                        return true;
                    }
                    else
                    {
                        // log user action
                        logMSG = String.Format("User clicks the '{0}' button for the warning message '{1}", LanguageINI.GetString("Ignore"), logMSG);
                        //  (logMSG);
                        LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                
                        prompt.Dispose();
                        return true;
                    }
                }
                else
                {
                    if (prompt.DialogResult == DialogResult.OK)
                    {
                        if (SeparatorGateway.GetInstance().IsHydroFluidLow() == false)
                        {
                            SeparatorGateway.GetInstance().ConfirmHydraulicFluidRefilled();

                            // LOG
                            logMSG = "User confirmed fluid top up";
                            //  (logMSG);
                            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                
                            prompt.Dispose();                            
                            return true;
                        }
                        else
                        {
                            prompt.Dispose();                            
                            return false;
                        }
                    }
                    else
                    {
                        // log user action
                        logMSG = String.Format("User clicks the '{0}' button for the warning message '{1}", LanguageINI.GetString("Ignore"), logMSG);
                        //  (logMSG);
                        LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                
                        prompt.Dispose();
                        return true;
                    }
                }
                //prompt.Dispose();
            }
            return true;
        }

        private void txtBoxLotID_Enter(object sender, EventArgs e)
        {
            // open keyboard if touch keyboard is enabled
            bool enabledKeyboard = RoboSep_UserConsole.KeyboardEnabled;

            if (enabledKeyboard)
            {
                Keyboard newKeyboard = Keyboard.getInstance(RoboSep_UserConsole.getInstance(),
                    this.txtBoxLotID, null, RoboSep_UserConsole.getInstance().frmOverlay, false);
                newKeyboard.ShowDialog();
                newKeyboard.Dispose();
            }
        }

        private void txtBoxLotID_MouseClick(object sender, MouseEventArgs e)
        {
            // open keyboard if touch keyboard is enabled
            bool enabledKeyboard = RoboSep_UserConsole.KeyboardEnabled;

            if (enabledKeyboard)
            {
                Keyboard newKeyboard = Keyboard.getInstance(RoboSep_UserConsole.getInstance(),
                    this.txtBoxLotID, null, RoboSep_UserConsole.getInstance().frmOverlay, false);
                RoboSep_UserConsole.showOverlay();
                newKeyboard.ShowDialog();
                newKeyboard.Dispose();
            }
        }

        private void listView_Resources_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            ListViewItem lvItem = e.Item;
            Rectangle rc = e.Bounds;
            ImageList imgList = lvItem.ImageList;
            if (lvItem.ImageList != null)
            {
                int index = lvItem.ImageIndex;
                Size si = imgList.ImageSize;
                Rectangle rc2 = lvItem.GetBounds(ItemBoundsPortion.Icon);

                imgList.Draw(e.Graphics, rc2.X, rc2.Y, rc2.Width, rc2.Height, index);
                rc.X += rc2.Width;
                rc.X += Margin.Left;
            }

            if (e.Item.Selected)
            {
                Color colorSelected = eScreenStatus != enumResourcesScreenStatus.eEndOfRunCompleted ? BG_Selected1 : BG_Selected2;
                if (lvItem.ImageList != null)
                {
                    Size si = imgList.ImageSize;
                    Rectangle rc3 = lvItem.GetBounds(ItemBoundsPortion.Icon);

                    float penWidth = 4.0f;
  
                    rc3.Inflate((int)-penWidth / 2, (int)-penWidth / 2);

                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    using (Pen p = new Pen(colorSelected, penWidth)) 
                    {
                        using (GraphicsPath path = Utilities.GetRoundedRect(rc3, cornerRounding)) 
                        {
                            e.Graphics.DrawPath(p, path);
                        }
                        p.Dispose();
                    }
                }
                SolidBrush b = new SolidBrush(colorSelected);
                e.Graphics.FillRectangle(b, rc);
                b.Dispose();
            }
            else 
            {
                if (lvItem.ImageList != null)
                {
                    Size si = imgList.ImageSize;
                    Rectangle rc4 = lvItem.GetBounds(ItemBoundsPortion.Entire);

                    float penWidth = 1.0f;

                    rc4.Inflate((int)-penWidth / 2, (int)-penWidth / 2);

                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    using (Pen p = new Pen(Border_Color, penWidth))
                    {
                        e.Graphics.DrawRectangle(p, rc4);
                        p.Dispose();
                    }
                }
            }
        }

        private void listView_Resources_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            ListViewItem lvItem = e.Item;
            Rectangle rc = e.Bounds;
            if (lvItem.ImageList != null)
            {
                rc = lvItem.GetBounds(ItemBoundsPortion.Label);
            }
            else
            {
                rc = e.Bounds;
            }
            // format text
            rc.X += Margin.Left;
            SolidBrush theBrush;
            if (e.Item.Selected)
            {
                theBrush = new SolidBrush(Color.White) ;
                e.Graphics.DrawString(e.Item.Text, listView_Resources.Font,theBrush , rc, theFormat);
            }
            else
            {
                theBrush = new SolidBrush(Txt_Color);
                e.Graphics.DrawString(e.Item.Text, listView_Resources.Font, theBrush, rc, theFormat);
            }
            theBrush.Dispose();
        }

        private void listView_Resources_ItemSelected(object sender, ListViewItemSelectionChangedEventArgs e) 
        {
            if (this.listView_Resources.SelectedItems.Count == 0)
                return;

            ListViewItem lvItem = listView_Resources.SelectedItems[0];
            if (lvItem.Index == lvLastSelectedIndex)
                return;

            lvLastSelectedIndex = lvItem.Index;
            if (lvItem.Tag != null)
            {
                int index = (int)lvItem.Tag;
                ListItem_Select(index);
            }

            if (lvItem.Index < this.listView_Resources.TopItem.Index || lvItem.Index > (this.listView_Resources.TopItem.Index + this.listView_Resources.VisibleRow - 1))
                this.listView_Resources.EnsureVisible(lvItem.Index);

            this.listView_Resources.Refresh();

            UpdateScrollButtonsState();
        }
 
        private void listView_Resources_Click(object sender, EventArgs e)
        {
            if (this.listView_Resources.SelectedItems.Count == 0)
                return;

            ListViewItem lvItem = listView_Resources.SelectedItems[0];
            if ((lvItem.Index - listView_Resources.TopItem.Index) == 0 && lvItem.Index > 0)
            {
                Thread.Sleep(200);

                // scroll up
                listView_Resources.LineUp();

                // update the scroll buttons
                UpdateScrollButtonsState();
            }
            else if (lvItem.Index == listView_Resources.TopItem.Index + listView_Resources.VisibleRow - 1)
            {
                Thread.Sleep(200);

                // scroll down
                listView_Resources.LineDown();

                // update the scroll buttons
                UpdateScrollButtonsState();
            }
        }

        private void UpdateScrollButtonsState()
        {
            if (this.listView_Resources.TopItem != null)
            {
                int TopIndex = this.listView_Resources.TopItem.Index;
                if (TopIndex <= 0)
                {
                    button_ScrollUp.Visible = false;
                }
                else
                {
                    button_ScrollUp.Visible = true;
                }

                int LastVisibleIndex = TopIndex + this.listView_Resources.VisibleRow - 1;
                if (this.listView_Resources.Items.Count <= LastVisibleIndex)
                {
                    button_ScrollDown.Visible = false;
                }
                else
                {
                    button_ScrollDown.Visible = true;
                }
            }
        }

        private void button_ScrollUp_Click(object sender, EventArgs e)
        {
            this.listView_Resources.LineUp();
            int TopIndex = this.listView_Resources.TopItem.Index;
            int LastVisibleIndex = TopIndex + this.listView_Resources.VisibleRow - 1;

            bool bUpdateScrollButton = true;
            if (this.listView_Resources.SelectedItems.Count > 0 && eScreenStatus != enumResourcesScreenStatus.eEndOfRunCompleted)
            {
                int SelectedIndex = listView_Resources.SelectedItems[0].Index;
                if (SelectedIndex > LastVisibleIndex)
                {
                    listView_Resources.Items[LastVisibleIndex].Selected = true;
                    bUpdateScrollButton = false;
                }
            }
            
            if (bUpdateScrollButton == true)
            {
                if (TopIndex <= 0)
                {
                    button_ScrollUp.Visible = false;
                }
                if (LastVisibleIndex < this.listView_Resources.Items.Count)
                {
                    button_ScrollDown.Visible = true;
                }
            }
             
        }

        private void button_ScrollDown_Click(object sender, EventArgs e)
        {
            this.listView_Resources.LineDown();
 
            int TopIndex = this.listView_Resources.TopItem.Index;

            bool bUpdateScrollButton = true;

            if (this.listView_Resources.SelectedItems.Count > 0 && eScreenStatus != enumResourcesScreenStatus.eEndOfRunCompleted)
            {
                int SelectedIndex = listView_Resources.SelectedItems[0].Index;
                if (SelectedIndex < TopIndex)
                {
                    this.listView_Resources.Items[TopIndex].Selected = true;
                    bUpdateScrollButton = false;
                }
            }
            
            if (bUpdateScrollButton == true)
            {
                if (TopIndex > 0)
                {
                    button_ScrollUp.Visible = true;
                }
                int LastVisibleIndex = TopIndex + this.listView_Resources.VisibleRow - 1;
                if (this.listView_Resources.Items.Count <= LastVisibleIndex)
                {
                    button_ScrollDown.Visible = false;
                }
            }
        }

        private void RunActivationTimer_Tick(object sender, EventArgs e)
        {
            RunActivationTimer.Stop();
            RunResources();
        }

        private DialogResult ShowBarcodeWindow()
        {
            UpdateReagentLotIDs();

            // Show barcode UI window
            Form_BarcodeScan BarcodeWindow = new Form_BarcodeScan(myReagentLotIDs1);
            this.Visible = false;
            DialogResult result = BarcodeWindow.ShowDialog();
            this.Visible = true;
            if (BarcodeWindow.DialogResult == DialogResult.OK)
            {
                // Update the resources window
                if (this.listView_Resources.SelectedItems.Count > 0)
                {
                    ListViewItem lvItem = listView_Resources.SelectedItems[0];
                    if (lvItem.Tag != null)
                    {
                        int index = (int)lvItem.Tag;
                        ListItem_Select(index);
                    }
                }
            }
            BarcodeWindow.Dispose();
            return result;
        }

        private void button_Scan_Click(object sender, EventArgs e)
        {
                ShowBarcodeWindow();
        }
    }

        #endregion

    // Class for storing info on run resources, and accompanying carousel
    // componenet graphics
    public class robo_Resource
    {
        public string sName;
        public PictureBox picBox;
        public PictureBox pbResource;
        public Image[] graphics;
        public TextBox myLotIdTextbox;

        public robo_Resource(string name, Point pos, Image[] pics, PictureBox Resource, TextBox LotId)
        {
            sName = name;

            if (pics != null && pics.Length > 0)
            {
                int nSize = pics.Length;
                graphics = new Image[nSize];
                for (int i = 0; i < nSize; i++)
                {
                    graphics[i] = pics[i];
                }
                picBox = new PictureBox();
                picBox.Visible = false;
                if (pics[0] != null)
                    picBox.Size = pics[0].Size;
                picBox.Location = pos;
                picBox.Image = pics[0];
            }
            
            pbResource = Resource;
            myLotIdTextbox = LotId;
        }

        public void show(bool showit)
        {
            if (picBox != null)
                picBox.Visible = showit;
        }

        public void showResource(bool showit)
        {
            if (pbResource != null)
                pbResource.Visible = showit;
        }

        public void select(bool turnOn)
        {
            if (graphics != null && graphics.Length >= 3)
            {
                if (turnOn)
                {
                    if (picBox != null && graphics[1] != null)
                        picBox.Image = graphics[1];
                    if (pbResource != null)
                        pbResource.Visible = true;
                }
                else
                {
                    if (picBox != null && graphics[2] != null)
                        picBox.Image = graphics[2];
                    if (pbResource != null)
                        pbResource.Visible = false;
                }
            }
            // for buffer bottle
            else
            {
                if (pbResource != null)
                {
                    if (turnOn)
                        pbResource.Visible = true;
                    else
                        pbResource.Visible = false;
                }
            }
        }

        // create circular region for circular carousel sections
        public void drawCircleRegion()
        {
            if (graphics == null || graphics.Length == 0 || graphics[0] == null)
                return;

            Size CircleSize = new Size(graphics[0].Size.Height, graphics[0].Size.Height);
            System.Drawing.Drawing2D.GraphicsPath CircPath = new System.Drawing.Drawing2D.GraphicsPath();
            Rectangle tempRect = new Rectangle();
            tempRect.Size = CircleSize;
            CircPath.AddEllipse(tempRect);
            Region CircRegion = new Region(CircPath);
            picBox.Region = CircRegion;
        }

        public void drawRegion( Point[] regionPoints )
        {
            System.Drawing.Drawing2D.GraphicsPath regionPath = new System.Drawing.Drawing2D.GraphicsPath();
            Rectangle tempRect = new Rectangle();
            if (graphics != null && graphics.Length > 0 && graphics[0] != null)
                tempRect.Size = graphics[0].Size;
            regionPath.AddPolygon(regionPoints);
            Region newRegion = new Region(regionPath);
            picBox.Region = newRegion;
        }

        public void Reset()
        {
            if (graphics == null || graphics.Length == 0 || graphics[0] == null)
                return;

            picBox.Image = graphics[0];
        }

        // show highlight at end of run
        public void showHighLight()
        {
            if (graphics != null && graphics.Length >= 3)
            {
                // Use dark purple color
                if (graphics[1] != null)
                {
                    if (picBox != null)
                        picBox.Image = graphics[1];
                    if (pbResource != null)
                        pbResource.Visible = true;
                }
            }
        }
    }
    

    public class ReagentLotIdentifiers1
    {
        public ReagentLotIdentifiers LotIDs;
        public bool bRequireSampleLabel;
        public bool bRequireCocktailLabel;
        public bool bRequireParticleLabel;
        public bool bRequireAntibodyLabel;
        public bool bRequireLysisLabel;
        public bool bRequireBufferLabel;
        public bool bSkipSampleLabel;
        public bool bSkipCocktailLabel;
        public bool bSkipParticleLabel;
        public bool bSkipAntibodyLabel;
        public bool bSkipLysisLabel;
        public bool bSkipBufferLabel;
        public QuadrantId QId = QuadrantId.NoQuadrant;
        public string QuadrantLabel;
        private RoboSep_Resources.enumBarcodeStatus eLotIDsBarcodeStatus = RoboSep_Resources.enumBarcodeStatus.Unknown;

        public int myRunConfigIdx;

        public void Reset()
        {
            bRequireSampleLabel = true;
            bRequireCocktailLabel = true;
            bRequireParticleLabel = true;
            bRequireAntibodyLabel = true;
            bRequireLysisLabel = true;
            bRequireBufferLabel = true;
            bSkipSampleLabel = false;
            bSkipCocktailLabel = false;
            bSkipParticleLabel = false;
            bSkipAntibodyLabel = false;
            bSkipLysisLabel = false;
            bSkipBufferLabel = false;
            QId = QuadrantId.NoQuadrant;
            QuadrantLabel = String.Empty;

            myRunConfigIdx=0;
        }

        public void CloneLotIDs(ReagentLotIdentifiers lotIDs)
        {
            ReagentLotIdentifiers? r = lotIDs;
            if (r == null)
                return;

            this.LotIDs = new ReagentLotIdentifiers();
            this.LotIDs.myCocktailLabel = lotIDs.myCocktailLabel;
            this.LotIDs.myParticleLabel = lotIDs.myParticleLabel;
            this.LotIDs.myAntibodyLabel = lotIDs.myAntibodyLabel;
            this.LotIDs.myLysisLabel = lotIDs.myLysisLabel;
            this.LotIDs.myBufferLabel = lotIDs.myBufferLabel;

            this.LotIDs.mySampleLabel = lotIDs.mySampleLabel;

        }

        public RoboSep_Resources.enumBarcodeStatus GetLotIDsStatus()
        {
            bool bAllFilled = true;
            bool bAllEmpty = true;
            bool bTemp;
            if (bRequireParticleLabel)
            {
                bTemp = string.IsNullOrEmpty(this.LotIDs.myParticleLabel) && !bSkipParticleLabel;
                bAllFilled = !bTemp;
                bAllEmpty = bTemp;
            }

            if (bRequireCocktailLabel)
            {
                bTemp = string.IsNullOrEmpty(this.LotIDs.myCocktailLabel) && !bSkipCocktailLabel;
                bAllFilled &= !bTemp;
                bAllEmpty &= bTemp;
            }

            if (bRequireAntibodyLabel)
            {
                bTemp = string.IsNullOrEmpty(this.LotIDs.myAntibodyLabel) && !bSkipAntibodyLabel;
                bAllFilled &= !bTemp;
                bAllEmpty &= bTemp;
            }

            // Barcode for sample tube has to be entered manually.
            // No check for the empty flag.

            if (bRequireSampleLabel)
            {
                bTemp = string.IsNullOrEmpty(this.LotIDs.mySampleLabel) && !bSkipSampleLabel;
                    bAllFilled &= !bTemp;
            }

            if (bAllFilled)
                eLotIDsBarcodeStatus = RoboSep_Resources.enumBarcodeStatus.FullyFilled;
            else if (bAllEmpty)
                eLotIDsBarcodeStatus = RoboSep_Resources.enumBarcodeStatus.AllEmpty;
            else
                eLotIDsBarcodeStatus = RoboSep_Resources.enumBarcodeStatus.PartiallyFilled;

            return eLotIDsBarcodeStatus;
        }
    }

    // data for resources, has value to locate which carousel location to interact with
    public class ResourceData
    {
        public string ButtonText;
        public int carouselLocation;
        public string sName;
        public double volume;
        public bool viewed;
        public string LotID;
        public int ReagentLotIDRef;

        public ResourceData(int CarouselLocation, string Name, double Volume)
        {
            sName = Name;
            ButtonText = sName;
            volume = Volume;
            if (volume > 0.00)
            { ButtonText += " " + volume.ToString() + "mL"; }
            ButtonText = setString(ButtonText);
            carouselLocation = CarouselLocation;
            viewed = false;
            ReagentLotIDRef = 0;
        }

        public ResourceData(int CarouselLocation, string Name, double Volume, int ID)
        {
            sName = Name;
            ButtonText = sName;
            volume = Volume;
            if (volume > 0.00)
            { ButtonText += " " + volume.ToString() + "mL"; }
            ButtonText = setString(ButtonText);
            carouselLocation = CarouselLocation;
            viewed = false;
            ReagentLotIDRef = ID;
        }

        private string setString(string txt)
        {
            int width = 195;
            Size txtSize = new Size(width, int.MaxValue);
            TextFormatFlags flags = TextFormatFlags.WordBreak;

            Font myFont = new Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            // dertermine size of 1 line:
            int currentLineHeight = TextRenderer.MeasureText("a1", myFont, txtSize, flags).Height;

            // determine size of current string
            txtSize = TextRenderer.MeasureText(txt, myFont, txtSize, flags);

            StringBuilder finalString = new StringBuilder();

            string[] stringBreakdown = txt.Split();

            StringBuilder testString = new StringBuilder();

            if (txtSize.Height > currentLineHeight)
            {
                testString.Append(stringBreakdown[0]);
                finalString.Append(stringBreakdown[0]);
                for (int i = 1; i < stringBreakdown.Length; i++)
                {
                    testString.Append(" ");
                    testString.Append(stringBreakdown[i]);
                    int strHeight = TextRenderer.MeasureText(testString.ToString(), myFont, txtSize, flags).Height;

                    if (strHeight > currentLineHeight)
                    {
                        finalString.Append("\n");
                        finalString.Append(stringBreakdown[i]);
                        currentLineHeight = strHeight;
                    }
                    else
                    {
                        finalString.Append(" ");
                        finalString.Append(stringBreakdown[i]);
                    }
                }
                myFont.Dispose();
                return finalString.ToString();
            }
            else
            {
                myFont.Dispose();
                return txt;
            }
        }
    }

    public class QuadrantRunInfo
    {
        private string sQuadrantLabel;
        private bool bMaintenance;
        private bool bViewed;

        public QuadrantRunInfo(string sQuadrantLabel, bool bMaintenance, bool bViewed)
        {
            this.sQuadrantLabel = sQuadrantLabel;
            this.bMaintenance = bMaintenance;
            this.bViewed = bViewed;
        }

        public string QuadrantLabel
        {
            get
            {
                return sQuadrantLabel;
            }
        }

        public bool IsMaintenance
        {
            get
            {
                return bMaintenance;
            }
        }

        public bool Viewed
        {
            get
            {
                return bViewed;
            }
            set
            {
                bViewed = value;
            }
        }
    }
}