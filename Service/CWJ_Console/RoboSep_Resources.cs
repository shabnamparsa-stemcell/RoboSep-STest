using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Xml;
using System.Text;
using System.Windows.Forms;

using GUI_Controls;

namespace GUI_Console
{
    public partial class RoboSep_Resources : UserControl
    {
        // define constant variables
        private const int CarouselLocations = 8;
        private const int NUMBEROFBUTTONS = 4;
        private const int QUADRANT1 = 0;
        private const int QUADRANT2 = 1;
        private const int QUADRANT3 = 2;
        private const int QUADRANT4 = 3;

        // define class variables
        private List<robo_Resource> Carousel = new List<robo_Resource>();
        List<Image> ilist = new List<Image>();
        List<Image> Selectedilist = new List<Image>();
        private GUI_Controls.Button_Quadrant[] QuadrantsSelect = new GUI_Controls.Button_Quadrant[4];
        private int TopButton = 0;
        private int CurrentCarouselItem;
        private int currentQuadrant;
        private List<List<ResourceData>> QuadrantResources = new List<List<ResourceData>>();
        private GUI_Controls.Button_Rectangle[] ListButtons = new GUI_Controls.Button_Rectangle[4];
        private SharingProtocol[] theRunConfig;
        private int[] mySharinfProfile = new int[4];

        
        // Constructor
        public RoboSep_Resources( SharingProtocol[] RunConfig, int[] sharing)
        {
            theRunConfig = RunConfig;
            mySharinfProfile = sharing;
            InitializeComponent();

            // LOG
            string LOGmsg = "Generating Robosep Resources user control";
            GUI_Controls.uiLog.LOG(this, "RoboSep_Resources", GUI_Controls.uiLog.LogLevel.EVENTS, LOGmsg);
            
            this.SuspendLayout();
            //
            // Carousel Items
            //
            this.Carousel.Add(new robo_Resource("Tip Rack", new Point(382, 181), new Image[3] 
                {GUI_Controls.Properties.Resources.Carousel_Section_TipRack,
                GUI_Controls.Properties.Resources.Carousel_Section_TipRackSelect,
                GUI_Controls.Properties.Resources.Carousel_Section_TipRackG}, pictureBox_TipRack));
            Point[] tipRackRegion = new Point[] { new Point(0, 18), new Point(30, 0), new Point(96, 0), new Point(96, 62), new Point(59, 96), new Point(0, 96) };
            Carousel[0].drawRegion(tipRackRegion);
            this.Carousel.Add(new robo_Resource("Waste Tube", new Point(472, 121), new Image[3] 
                {GUI_Controls.Properties.Resources.Carousel_Section_Waste,
                GUI_Controls.Properties.Resources.Carousel_Section_WasteSelect,
                GUI_Controls.Properties.Resources.Carousel_Section_WasteG}, pictureBox_WasteTube));
            Carousel[1].drawCircleRegion();
            this.Carousel.Add(new robo_Resource("Negative Fraction Tube", new Point(390, 114), new Image[3] 
                {GUI_Controls.Properties.Resources.Carousel_Section_NegativeFraction,
                GUI_Controls.Properties.Resources.Carousel_Section_NegativeFractionSelect,
                GUI_Controls.Properties.Resources.Carousel_Section_NegativeFractionG},pictureBox_WasteTube));
            Carousel[2].drawCircleRegion();
            this.Carousel.Add(new robo_Resource("Negative Selection Vial", new Point(359, 135), new Image[3] 
                {GUI_Controls.Properties.Resources.Carousel_Section_NegativeSelection,
                GUI_Controls.Properties.Resources.Carousel_Section_NegativeSelectionSelect,
                GUI_Controls.Properties.Resources.Carousel_Section_NegativeSelectionG}, pictureBox_1mL));
            Carousel[3].drawCircleRegion();
            this.Carousel.Add(new robo_Resource("Magnetic Particle vial", new Point(355, 104), new Image[3] 
                {GUI_Controls.Properties.Resources.Carousel_Section_MagneticParticle,
                GUI_Controls.Properties.Resources.Carousel_Section_MagneticParticleSelect,
                GUI_Controls.Properties.Resources.Carousel_Section_MagneticParticleG}, pictureBox_1mL));
            Carousel[4].drawCircleRegion();
            this.Carousel.Add(new robo_Resource("Antibody Vial", new Point(367, 164), new Image[3] 
                {GUI_Controls.Properties.Resources.Carousel_Section_Antibody,
                GUI_Controls.Properties.Resources.Carousel_Section_AntibodySelect,
                GUI_Controls.Properties.Resources.Carousel_Section_AntibodyG}, pictureBox_1mL));
            Carousel[5].drawCircleRegion();
            this.Carousel.Add(new robo_Resource("Sample Vial", new Point(442, 266), new Image[3] 
                {GUI_Controls.Properties.Resources.Carousel_Section_Sample,
                GUI_Controls.Properties.Resources.Carousel_Section_SampleSelect,
                GUI_Controls.Properties.Resources.Carousel_Section_SampleG}, pictureBox_SampleVial));
            Carousel[6].drawCircleRegion();
            this.Carousel.Add(new robo_Resource("Separation Tube and \nEasySep Magnet", new Point(486, 180), new Image[3] 
                {GUI_Controls.Properties.Resources.Carousel_Section_Magnet,
                GUI_Controls.Properties.Resources.Carousel_Section_MagnetSelect,
                GUI_Controls.Properties.Resources.Carousel_Section_MagnetG}, pictureBox_SampleVial));
            Point[] MagnetRegion = new Point[] { new Point(0, 56), new Point(47, 0), new Point(140, 0), new Point(140, 177), new Point(0, 177) };
            Carousel[7].drawRegion( MagnetRegion );
            this.Carousel.Add(new robo_Resource("Tip Rack", new Point(382, 181), new Image[3] 
                {GUI_Controls.Properties.Resources.Carousel_Section_TipRack,
                GUI_Controls.Properties.Resources.Carousel_Section_TipRackSelect,
                GUI_Controls.Properties.Resources.Carousel_Section_TipRackG}, pictureBox_TipRack));
            
            // Add picture boxes from Robo_resource to
            for (int i = 0; i < Carousel.Count; i++)
            {
                Carousel[i].picBox.Name = "CarouselComponent" + i.ToString();
                this.Controls.Add(Carousel[i].picBox);
            }

            // add click event for carousel picture box items
            this.Carousel[0].picBox.Click += new System.EventHandler(this.CarouselItem0_Click);
            this.Carousel[1].picBox.Click += new System.EventHandler(this.CarouselItem1_Click);
            this.Carousel[2].picBox.Click += new System.EventHandler(this.CarouselItem2_Click);
            this.Carousel[3].picBox.Click += new System.EventHandler(this.CarouselItem3_Click);
            this.Carousel[4].picBox.Click += new System.EventHandler(this.CarouselItem4_Click);
            this.Carousel[5].picBox.Click += new System.EventHandler(this.CarouselItem5_Click);
            this.Carousel[6].picBox.Click += new System.EventHandler(this.CarouselItem6_Click);
            this.Carousel[7].picBox.Click += new System.EventHandler(this.CarouselItem7_Click);
            this.Carousel[8].picBox.Click += new System.EventHandler(this.CarouselItem8_Click);
            

            // change corner graphics
            this.ResumeLayout(true);
        }

        private void RoboSep_Resources_Load(object sender, EventArgs e)
        {
            // load current user name
            label_userName.Text = RoboSep_UserConsole.strCurrentUser;

            // change Arrow Graphics
            // up button
            ilist.Clear();
            ilist.Add(GUI_Controls.Properties.Resources.Button_UP0);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_UP1);
            ilist.Add(GUI_Controls.Properties.Resources.Button_UP2);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_UP3);
            button_ListUP.ChangeGraphics(ilist);
            // down button
            ilist.Clear();
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_DOWN0);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_DOWN1);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_DOWN2);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_DOWN3);
            button_ListDOWN.ChangeGraphics(ilist);

            // change list button graphics
            ilist.Clear();
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_RESOURCE_LIST0);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_RESOURCE_LIST1);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_RESOURCE_LIST2);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_RESOURCE_LIST3);
            button_ListItem1.ChangeGraphics(ilist);
            button_ListItem1.setToggle(true);
            button_ListItem2.ChangeGraphics(ilist);
            button_ListItem2.setToggle(true);
            button_ListItem3.ChangeGraphics(ilist);
            button_ListItem3.setToggle(true);
            button_ListItem4.ChangeGraphics(ilist);
            button_ListItem4.setToggle(true);

            // create ilist for selected buttons
            Selectedilist.Add(GUI_Controls.Properties.Resources.BUTTON_RESOURCE_Selected);
            Selectedilist.Add(GUI_Controls.Properties.Resources.BUTTON_RESOURCE_Selected);
            Selectedilist.Add(GUI_Controls.Properties.Resources.BUTTON_RESOURCE_Selected);
            Selectedilist.Add(GUI_Controls.Properties.Resources.BUTTON_RESOURCE_Selected);

            // create array of buttons
            ListButtons[0] = button_ListItem1;
            ListButtons[1] = button_ListItem2;
            ListButtons[2] = button_ListItem3;
            ListButtons[3] = button_ListItem4;
            // create array of quadrants
            QuadrantsSelect[0] = button_Quadrant1;
            QuadrantsSelect[1] = button_Quadrant2;
            QuadrantsSelect[2] = button_Quadrant3;
            QuadrantsSelect[3] = button_Quadrant4;


            // set disable arrow images
            button_ListUP.disableImage = GUI_Controls.Properties.Resources.BUTTON_UP_Disable;
            button_ListDOWN.disableImage = GUI_Controls.Properties.Resources.BUTTON_DOWN_Disable;

            // hide up button to start
            button_ListUP.Visible = false;

            // load quadrant resources (quadrant 1)            
            grabQuadrantResources();
        }
        
        // routine to grab quadrant resources
        private void grabQuadrantResources()
        {
            int ANTIBODY = 5;
            int NEGATIVESELECTION = 4;
            int MAGNETICPARTICLE = 3;
            // for now just fill up list w/ quadrant resources and volumes
            // quadrant 1
            
            for (int i = 0; i < 4; i++)
            {
                QuadrantResources.Add(new List<ResourceData>());
            }

            if (theRunConfig[0].Name == "Basic prime")
            {
                string ResourceName = theRunConfig[0].CustomNames[1];
                if (theRunConfig[0].CustomNames[1] == string.Empty)
                    ResourceName = Carousel[1].sName;
                ResourceData tempResource = new ResourceData
                    (1, ResourceName, theRunConfig[0].Consumables[0, 1].Volume.Amount / 1000);
                // check to see if quadrant is leeching
                // if so don't add vial a-c (magnetic particle, antibody, negative selection)
                if (mySharinfProfile[(theRunConfig[0].InitQuadrant) + 0] == 0 ||
                   (1 != ANTIBODY && 1 != NEGATIVESELECTION && 1 != MAGNETICPARTICLE))
                    QuadrantResources[(theRunConfig[0].InitQuadrant) + 0].Add(tempResource);
            }
            else
            {

                // for each protocol
                for (int i = 0; i < theRunConfig.Length; i++)
                {
                    for (int j = 0; j < theRunConfig[i].Consumables.GetLength(0); j++)
                    {
                        // for each resource item per quadrant
                        for (int k = 0; k < 8; k++)
                        {
                            // remove some elements if sharing reagents


                            if (theRunConfig[i].Consumables[j, k].IsRequired)
                            {
                                string ResourceName = theRunConfig[i].CustomNames[k] == null || theRunConfig[i].CustomNames[k] == string.Empty
                                    ? Carousel[k].sName : theRunConfig[i].CustomNames[k];
                                ResourceData tempResource = new ResourceData
                                    (k, ResourceName, theRunConfig[i].Consumables[j, k].Volume.Amount / 1000);
                                // check to see if quadrant is leeching
                                // if so don't add vial a-c (magnetic particle, antibody, negative selection)
                                if (mySharinfProfile[(theRunConfig[i].InitQuadrant) + j] == 0 ||
                                   (k != ANTIBODY && k != NEGATIVESELECTION && k != MAGNETICPARTICLE))
                                    QuadrantResources[(theRunConfig[i].InitQuadrant) + j].Add(tempResource);
                            }
                        }
                    }
                }
            }

            // set up quadrants (
            bool firstquadrant = true;
            for (int i = 0; i < 4; i++)
            {
                if (firstquadrant && QuadrantResources[i].Count > 0)
                {
                    QuadrantsSelect[i].Check = true;
                    currentQuadrant = i;
                    QuadrantsSelect[i].BackImage[0] = QuadrantsSelect[i].BackImage[4];
                    QuadrantsSelect[i].BackImage[1] = QuadrantsSelect[i].BackImage[4];
                    firstquadrant = false;
                }
                // do not show quadrants with no resources
                if (QuadrantResources[i].Count <= 0)
                { 
                    QuadrantsSelect[i].Visible = false;
                }
            }
            // load quadrant data after grabbing it
            loadQuadrantResources(currentQuadrant);

            // LOG
            string LOGmsg = "Loading resources from sharing protocols";
            GUI_Controls.uiLog.LOG(this, "grabQuadrantResources", GUI_Controls.uiLog.LogLevel.DEBUG, LOGmsg);
        }

        // routine that displays appropriate quadrant resources
        private void loadQuadrantResources(int QuadrantNumber)
        {
            // LOG
            string LOGmsg = "Displaying quadrant " + QuadrantNumber.ToString() + " resources";
            GUI_Controls.uiLog.LOG(this, "grabQuadrantResources", GUI_Controls.uiLog.LogLevel.DEBUG, LOGmsg);

            // hide all carousel items
            for (int i = 0; i < CarouselLocations; i++)
            {
                Carousel[i].show(false);
            }
            
            // Display all buttons
            for (int i = 0; i < NUMBEROFBUTTONS; i++)
            {
                ListButtons[i].Visible = true;
            }

            // display down button
            button_ListDOWN.Visible = true;

            // display appropriate carousel items,  change volumes to represent values
            // defined in protocol selection
            for (int i = 0; i < QuadrantResources[QuadrantNumber].Count; i++)
            {
                try
                {
                    // show specific carousel component
                    Carousel[QuadrantResources[QuadrantNumber][i].carouselLocation].show(true);
                    // don't move item 3 or 5 to front as they are large and cause overlay with other graphics
                    if (QuadrantResources[QuadrantNumber][i].carouselLocation != 0 &&
                        QuadrantResources[QuadrantNumber][i].carouselLocation != 7)
                    {
                        Carousel[QuadrantResources[QuadrantNumber][i].carouselLocation].picBox.BringToFront();
                    }                    
                }
                catch
                {
                    // do nothing
                }
            }
            
            // only display number of buttens requires (max 4)
            if (QuadrantResources[QuadrantNumber].Count <= NUMBEROFBUTTONS)
            {
                button_ListDOWN.Visible = false;
                button_ListUP.Visible = false;
                for (int i = NUMBEROFBUTTONS; i > QuadrantResources[QuadrantNumber].Count; i--)
                {
                    ListButtons[i - 1].Visible = false;
                }
            }

            // set first item on list to "viewed" 
            QuadrantResources[QuadrantNumber][0].viewed = true;
            // set current carousel location to location of first item on list
            CurrentCarouselItem = QuadrantResources[QuadrantNumber][0].carouselLocation;
            // display appropriate carousel graphics
            Carousel[QuadrantResources[QuadrantNumber][0].carouselLocation].select(true);
            // refresh buttons
            UpdateButtons();
        }

        // update button list
        private void UpdateButtons()
        {
            int buttonsInList = NUMBEROFBUTTONS;
            if (QuadrantResources[currentQuadrant].Count < NUMBEROFBUTTONS)
            {
                buttonsInList = QuadrantResources[currentQuadrant].Count; }
            for (int i = 0; i < buttonsInList; i++)
            {
                // check if button is Selected
                if (QuadrantResources[currentQuadrant][TopButton + i].carouselLocation == CurrentCarouselItem)
                {
                    ListButtons[i].BackImage = Selectedilist;
                    ListButtons[i].BackgroundImage = ListButtons[i].BackImage[2];
                }
                else if (ListButtons[i].BackImage[0] == Selectedilist[0])
                {
                    ListButtons[i].BackImage = ilist;
                    ListButtons[i].BackgroundImage = ListButtons[i].BackImage[0];
                    if (ListButtons[i].Check = QuadrantResources[currentQuadrant][TopButton + i].viewed)
                    {
                        ListButtons[i].BackgroundImage = ListButtons[i].BackImage[2];
                    }
                }
                // change button text
                ListButtons[i].AccessibleName = QuadrantResources[currentQuadrant][TopButton + i].ButtonText;
                // set button state
                ListButtons[i].Check = QuadrantResources[currentQuadrant][TopButton + i].viewed;
                // refresh each button
                ListButtons[i].Refresh();
                
            }
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
            Carousel[CurrentCarouselItem].select(false);
            Carousel[itemNumber].select(true);            
            CurrentCarouselItem = itemNumber;
            
            // go to item in button list
            // search for item in resource list
            int locationInList = 0;
            for (int i = 0; i < QuadrantResources[currentQuadrant].Count; i++)
            {
                if (QuadrantResources[currentQuadrant][i].carouselLocation == itemNumber)
                {
                    locationInList = i;
                }
            }
            QuadrantResources[currentQuadrant][locationInList].viewed = true;
            // check if button is currently displayed
            if (locationInList < TopButton || locationInList > (TopButton + 3))
            {
                TopButton = locationInList;
                button_ListUP.Visible = true;
                if (TopButton == 0) { button_ListUP.Visible = false; }
                if (TopButton > (QuadrantResources[currentQuadrant].Count - 4))
                {
                    TopButton = QuadrantResources[currentQuadrant].Count - 4;
                    button_ListDOWN.Visible = false;
                }
            }
            UpdateButtons();
        }

        private void button_ListUP_Click(object sender, EventArgs e)
        {
            // check if can go up list any further
            TopButton -= 1;
            if (TopButton == 0)
            {
                button_ListUP.Visible = false;
            }
            // enable down button
            if ((TopButton + 4) != QuadrantResources[currentQuadrant].Count)
            {
                button_ListDOWN.Visible = true;
            }
            // move down list
            UpdateButtons();
        }

        private void button_ListDOWN_Click(object sender, EventArgs e)
        {
            // check if can go down list further
            TopButton += 1;
            if ((TopButton + 4) == QuadrantResources[currentQuadrant].Count)
            {
                button_ListDOWN.Visible = false;
            }
            // enable up button
            if (TopButton != 0)
            {
                button_ListUP.Visible = true;
            }
            // move down list
            UpdateButtons();
        }

        private void button_Cancel1_Click(object sender, EventArgs e)
        {
            // unload progress bar
            // cancling takes user back to run samples window
            // clicking the run button will re-add wanted protocols
            RoboSep_RunProgress.getInstance().Unload( this );

            RoboSep_RunSamples SamplingWindow = RoboSep_RunSamples.getInstance();
            SamplingWindow.Location = new Point(0, 0);
            SamplingWindow.Visible = true;
            this.Visible = false;
            RoboSep_UserConsole.getInstance().Controls.Add(SamplingWindow);
            RoboSep_UserConsole.getInstance().Controls.Remove(RoboSep_UserConsole.ctrlCurrentUserControl);
            RoboSep_UserConsole.ctrlCurrentUserControl = SamplingWindow;

            // LOG
            string LOGmsg = "Cancel button clicked";
            GUI_Controls.uiLog.LOG(this, "button_Cancel1_Click", GUI_Controls.uiLog.LogLevel.DEBUG, LOGmsg);
        }

        private void button_ListItem1_Click(object sender, EventArgs e)
        {
            // turn off previously selected item
            Carousel[CurrentCarouselItem].select(false);
            // set button to "has been viewed"
            QuadrantResources[currentQuadrant][TopButton].viewed = true;
            // set current item to "currentCarouselItem" and display it
            CurrentCarouselItem = QuadrantResources[currentQuadrant][TopButton].carouselLocation;
            Carousel[CurrentCarouselItem].select(true);
            UpdateButtons();
        }

        private void button_ListItem2_Click(object sender, EventArgs e)
        {
            // turn off previously selected item
            Carousel[CurrentCarouselItem].select(false);
            // set button to "has been viewed"
            QuadrantResources[currentQuadrant][TopButton + 1].viewed = true;
            // set current item to "currentCarouselItem" and display it
            CurrentCarouselItem = QuadrantResources[currentQuadrant][TopButton + 1].carouselLocation;
            Carousel[CurrentCarouselItem].select(true);
            UpdateButtons();
        }

        private void button_ListItem3_Click(object sender, EventArgs e)
        {
            // turn off previously selected item
            Carousel[CurrentCarouselItem].select(false);
            // set button to "has been viewed"
            QuadrantResources[currentQuadrant][TopButton + 2].viewed = true;
            // set current item to "currentCarouselItem" and display it
            CurrentCarouselItem = QuadrantResources[currentQuadrant][TopButton +2].carouselLocation;
            Carousel[CurrentCarouselItem].select(true);
            UpdateButtons();
        }

        private void button_ListItem4_Click(object sender, EventArgs e)
        {
            // turn off previously selected item
            Carousel[CurrentCarouselItem].select(false);
            // set button to "has been viewed"
            QuadrantResources[currentQuadrant][TopButton + 3].viewed = true;
            // set current item to CurrentCarouselItem and display it
            CurrentCarouselItem = QuadrantResources[currentQuadrant][TopButton + 3].carouselLocation;
            Carousel[CurrentCarouselItem].select(true);
            UpdateButtons();
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
            currentQuadrant = newQuadrant;
            for (int i = 0; i < 4; i++)
            {
                QuadrantsSelect[i].Check = false;
                if (i == newQuadrant)
                {
                    QuadrantsSelect[i].Check = true;
                }
            }
            // change selection to initial item
            Carousel[CurrentCarouselItem].select(false);
            for (int i = 0; i < Carousel.Count; i++)
            {
                // reset picture box graphics
                Carousel[i].Reset();
            }
            // re-select initial selection to highlight it
            Carousel[CurrentCarouselItem].select(false);
            TopButton = 0;
            //Carousel[CurrentCarouselItem].select(true);
            loadQuadrantResources(newQuadrant);
        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            string sMSG = GUI_Controls.fromINI.getValue("GUI", "msgCarouselSetup");
            sMSG = sMSG == null ? "All required carousel resources are set up and ready for Separation?" : sMSG;

            RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(),
                sMSG, "Begin Separation", "Yes", "No");
            RoboSep_UserConsole.getInstance().frmOverlay.Show();
            prompt.ShowDialog();
            RoboSep_UserConsole.getInstance().frmOverlay.Hide();
            if (prompt.DialogResult == DialogResult.OK)
            {
                // run protocols,  run progress window should be set up through
                // run samples window before coming to resources window.
                // do separatorgateway.startrun at this point
                RoboSep_RunSamples.getInstance().startSeparationRun();

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
                GUI_Controls.uiLog.LOG(this, "buttonRun_Click", GUI_Controls.uiLog.LogLevel.DEBUG, LOGmsg);

                // tell progress bars to begin.
                RoboSep_RunProgress.getInstance().Start();
            }
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
        public Image[] graphics = new Image[3];

        public robo_Resource(string name, Point pos, Image[] pics, PictureBox Resource)
        {
            sName = name;
            graphics[0] = pics[0];
            graphics[1] = pics[1];
            graphics[2] = pics[2];
            picBox = new PictureBox();
            picBox.Visible = false;
            if (pics[0] != null)
                picBox.Size = pics[0].Size;
            picBox.Location = pos;
            picBox.Image = pics[0];
            pbResource = Resource;
        }

        public void show(bool showit)
        {
            picBox.Visible = showit;
        }

        public void select(bool turnOn)
        {
            if (turnOn)
            { 
                picBox.Image = graphics[1];
                pbResource.Visible = true;
            }
            else
            { 
                picBox.Image = graphics[2];
                pbResource.Visible = false;
            }
        }

        // create circular region for circular carousel sections
        public void drawCircleRegion()
        {
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
            tempRect.Size = graphics[0].Size;
            regionPath.AddPolygon(regionPoints);
            Region newRegion = new Region(regionPath);
            picBox.Region = newRegion;
        }

        public void Reset()
        {
            picBox.Image = graphics[0];
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

        public ResourceData(int CarouselLocation, string Name, double Volume)
        {
            sName = Name;
            ButtonText = sName;
            volume = Volume;
            if (volume > 0.00)
            { ButtonText += " - " + volume.ToString() + "mL"; }
            carouselLocation = CarouselLocation;
            viewed = false;
            LotID = string.Empty;
        }

        public ResourceData(int CarouselLocation, string Name, double Volume, string ID)
        {
            sName = Name;
            ButtonText = sName;
            volume = Volume;
            if (volume > 0.00)
            { ButtonText += " - " + volume.ToString() + "mL"; }
            carouselLocation = CarouselLocation;
            viewed = false;
            LotID = ID;
        }
    }
}