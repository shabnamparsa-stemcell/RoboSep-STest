using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Xml;
using System.Text;
using System.Windows.Forms;

using GUI_Controls;
using Tesla.Common.Protocol;
using Tesla.OperatorConsoleControls;
using Tesla.Common.Separator;
using Tesla.OperatorConsoleControls;
using Tesla.Common.ResourceManagement;
using Invetech.ApplicationLog;

namespace GUI_Console
{
    public partial class RoboSep_RunSamples : BasePannel
    {
        private static RoboSep_RunSamples myRunSamples;
        private QuadrantInfo[] myQuadrants;
        private SeparatorGateway mySeparatorGateway;
        private int iProtocolSelectCount;

        private int[] sharedSectorsTranslation = new int[4];
        private int[] sharedSectorsProtocolIndex = new int[4];
        private bool isSharing = false;

        private ReagentLotIdentifiers[] myReagentLotIds;

        private RoboSep_RunSamples()
        {
            InitializeComponent();
            mySeparatorGateway = SeparatorGateway.GetInstance();

            // LOG
            string logMSG = "Initializing Run Samples user control";
            GUI_Controls.uiLog.LOG(this, "RoboSep_RunSamples", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        private void RoboSep_RunSamples_Load(object sender, EventArgs e)
        {
            // change graphics for scan button
            List<Image> ilist = new List<Image>();
            ilist.Add(GUI_Controls.Properties.Resources.Button_RECT_MED0);
            ilist.Add(GUI_Controls.Properties.Resources.Button_RECT_MED1);
            ilist.Add(GUI_Controls.Properties.Resources.Button_RECT_MED2);
            ilist.Add(GUI_Controls.Properties.Resources.Button_RECT_MED3);
            button_Scan.ChangeGraphics(ilist);
            // play button 
            ilist.Clear();
            ilist.Add(GUI_Controls.Properties.Resources.Button_RUN0);
            ilist.Add(GUI_Controls.Properties.Resources.Button_RUN1);
            ilist.Add(GUI_Controls.Properties.Resources.Button_RUN2);
            ilist.Add(GUI_Controls.Properties.Resources.Button_RUN3);
            button_RunSamples.ChangeGraphics(ilist);
            // Quadrant Buttons
            ilist.Clear();
            ilist.Add(GUI_Controls.Properties.Resources.Quadrant_Purple);
            ilist.Add(GUI_Controls.Properties.Resources.Quadrant_Purple);
            ilist.Add(GUI_Controls.Properties.Resources.Quadrant_Grey2);
            ilist.Add(GUI_Controls.Properties.Resources.Quadrant_Grey2);
            Q1.ChangeGraphics(ilist);
            Q2.ChangeGraphics(ilist);
            Q3.ChangeGraphics(ilist);
            Q4.ChangeGraphics(ilist);
            Q1.Check = true;
            Q2.Check = true;
            Q3.Check = true;
            Q4.Check = true;

            // space out deviders & graphics
            int Height = roboPanel_runInfo.Size.Height - 8;
            int BORDERWIDTH = 6;

            // initialize myQuadrants
            myQuadrants = new QuadrantInfo[4] {
                new QuadrantInfo(Q1_protocolName, Q1_Vol, Q1_Cancel, Q1),
                new QuadrantInfo(Q2_protocolName, Q2_Vol, Q2_Cancel, Q2),
                new QuadrantInfo(Q3_protocolName, Q3_Vol, Q3_Cancel, Q3),
                new QuadrantInfo(Q4_protocolName, Q4_Vol, Q4_Cancel, Q4) };
            myQuadrants[0].setDivider(divider1);
            myQuadrants[1].setDivider(divider2);
            myQuadrants[2].setDivider(divider3);

            iProtocolSelectCount = 0;

            int[] ClearSharing = { 0, 0, 0, 0 };
            sharedSectorsTranslation = ClearSharing;
            sharedSectorsProtocolIndex = ClearSharing;

            
        }

        public static RoboSep_RunSamples getInstance()
        {
            if (myRunSamples == null)
            {
                myRunSamples = new RoboSep_RunSamples();
            }
            return myRunSamples;
        }

        public int[] iSelectedProtocols
        {
            // returns the number of protocols that have been
            // selected by the user.
            get
            {
                int[] CurrentlySelected = new int[iProtocolSelectCount];
                int count = 0;
                for (int i = 0; i < 4; i++)
                {
                    if (myQuadrants[i].QuadrantsRequired > 0)
                    {
                        CurrentlySelected[count] = i;
                        count++;
                    }
                }
                return CurrentlySelected;
            }
        }

        private ISeparationProtocol getSelectedProtocol(string protocolName)
        {
            // returns the appropriate IseparationProtocol from server
            ISeparationProtocol[] SelectedProtocols = RoboSep_UserConsole.getInstance().XML_getUserProtocols();
            for (int i = 0; i < SelectedProtocols.Length; i++)
            {
                if (SelectedProtocols[i].Label == protocolName)
                { return SelectedProtocols[i]; }
            }
            return null;
        }

        public void addToRun(int Q, string ProtocolName)
        {            
            // search for protocol in SelectedProtocols list
            ISeparationProtocol selectedProtocol = getSelectedProtocol(ProtocolName);

            // LOG
            string logMSG = "Adding protocol '" + ProtocolName + "' to run at quadrant" + Q.ToString();
            GUI_Controls.uiLog.LOG(this, "addToRun", GUI_Controls.uiLog.LogLevel.DEBUG, logMSG);

            // get volume from user
            NumberPad numPad = new NumberPad(RoboSep_UserConsole.getInstance(), RoboSep_UserConsole.getInstance().frmOverlay,
                myQuadrants[Q].GetVolumeLabel(), selectedProtocol.Description,
                selectedProtocol.MinimumSampleVolume / 1000, selectedProtocol.MaximumSampleVolume / 1000);
            //numPad.disableCancelButton();
            RoboSep_UserConsole.getInstance().addForm(numPad, numPad.Offset);
            RoboSep_UserConsole.getInstance().frmOverlay.Show();
            numPad.ShowDialog();

            if (numPad.DialogResult != DialogResult.Cancel)
            {

                // back up quadrant info, in case quadrant add doesn't fit
                QuadrantInfo tempQuadrant = myQuadrants[Q];
                if (myQuadrants[Q].bQuadrantInUse)
                {
                    iProtocolSelectCount -= 1;
                    for (int i = 0; i < myQuadrants[Q].QuadrantsRequired - 1; i++)
                    {
                        myQuadrants[Q + i].Clear();
                    }
                }


                // check if protocol requires more than one quadrant
                if (selectedProtocol.QuadrantCount > 1)
                {
                    bool fit = true;
                    for (int i = 0; i < selectedProtocol.QuadrantCount; i++)
                    {
                        if ((Q + i) >= 4 || myQuadrants[Q + i].bQuadrantInUse)
                            fit = false;
                    }

                    if (fit)
                    {
                        // first attempt to add protocol to server "chosen protocols" list
                        iProtocolSelectCount += 1;
                        RoboSep_UserConsole.getInstance().mySeparator.SelectProtocol((QuadrantId)(Q), selectedProtocol);

                        // verify protocol will fit (with server)
                        //mySeparatorGateway.VerifyProtocolSelection( mySamplesCurrentQuadrant, selectedProtocolIndex);

                        // pass protocol information to quadrant info
                        myQuadrants[Q].setProtocol(selectedProtocol);
                        for (int i = 1; i < selectedProtocol.QuadrantCount; i++)
                        {
                            myQuadrants[Q + i].bQuadrantInUse = true;
                            myQuadrants[Q + i].Update();
                            if (i > 0 && myQuadrants[Q + i - 1].Divider != null)
                            {
                                myQuadrants[Q + i - 1].Divider.Visible = false;
                            }
                        }

                        // inform server of volume update
                        FluidVolume QVolume = new FluidVolume(myQuadrants[Q].QuadrantVolume, Tesla.Common.FluidVolumeUnit.MilliLitres);
                        RoboSep_UserConsole.getInstance().SetSampleVolume((QuadrantId)(Q), QVolume);
                    }
                    else
                    {
                        // re add previlously stored protocol (as selected protocol doesnt fit)
                        addToRun(Q, tempQuadrant.QuadrantLabel);
                    }
                }
                else
                {
                    // first attempt to add protocol to server "chosen protocols" list
                    RoboSep_UserConsole.getInstance().mySeparator.SelectProtocol((QuadrantId)(Q), selectedProtocol);
                    // pass protocol information to quadrant info
                    myQuadrants[Q].setProtocol(selectedProtocol);

                    // inform server of volume update
                    FluidVolume QVolume = new FluidVolume(myQuadrants[Q].QuadrantVolume, Tesla.Common.FluidVolumeUnit.MilliLitres);
                    RoboSep_UserConsole.getInstance().SetSampleVolume((QuadrantId)(Q), QVolume);

                    iProtocolSelectCount += 1;

                    // verify protocol will fit (with server)
                    // mySeparatorGateway.VerifyProtocolSelection( mySamplesCurrentQuadrant, selectedProtocolIndex);
                }
            }
        }

        private void UpdateResourcesPageData(int protocolIndex)
        {
            // Get the configuration data and protocol name 

            string name;
            string[] customNames;// = new string[8];/*CR*/
            int numQuadrants;
            ProtocolClass type;
            int initQuad;
            ProtocolConsumable[,] consumables;

            consumables = mySeparatorGateway.GetProtocolConsumables(protocolIndex, out name, out type, out initQuad);
            mySeparatorGateway.GetCustomNames(name, out customNames, out numQuadrants);/*CR*/


        }

        public bool ConfirmReadytoRun()
        {
            string sMSG = GUI_Controls.fromINI.getValue("GUI", "msgConfirmRun");
            sMSG = sMSG == null ? "Run selected protocols?" : sMSG;

            RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(),
                sMSG, "Confrim Run", "Ok", "Cancel");
            RoboSep_UserConsole.getInstance().frmOverlay.Show();
            prompt.ShowDialog();
            RoboSep_UserConsole.getInstance().frmOverlay.Hide();
            if (prompt.DialogResult == DialogResult.OK)
            { 
                return true;
                
                // LOG
                string logMSG = "User confirmed ready to run protocols";
                GUI_Controls.uiLog.LOG(this, "ConfirmReadytoRun", GUI_Controls.uiLog.LogLevel.DEBUG, logMSG);
            }
            else
            { return false; }
        }

        private SharingProtocol[] getConsumables()
        {
            // generate lot IDs for all quadrants
            myReagentLotIds = new ReagentLotIdentifiers[iProtocolSelectCount];

            try
            {
                SharingProtocol[] AllProtocols = new SharingProtocol[iProtocolSelectCount];
                for (int i = 0; i < AllProtocols.Length; i++)
                {
                    string name;
                    ProtocolClass type;
                    int initQuad = 0;
                    double amount = 0;
                    string[] customNames;
                    int numQuadrants;
                    ProtocolConsumable[,] consumables;
                    consumables = mySeparatorGateway.GetProtocolConsumables(i, out name, out type, out initQuad);
                    amount = consumables[0, (int)RelativeQuadrantLocation.SampleTube].Volume.Amount;
                    mySeparatorGateway.GetCustomNames(name, out customNames, out numQuadrants);
                    AllProtocols[i] = new SharingProtocol(name, amount, initQuad, consumables, customNames);

                    // load ReagetnLotIds
                    myReagentLotIds[i].Reset();
                }
                // return sharingProtocols
                return AllProtocols;
            }
            catch
            {
                return getConsumables();
            }
            
            // LOG
            string logMSG = "Getting Consumables for " + iProtocolSelectCount.ToString() + " quadrants";
            GUI_Controls.uiLog.LOG(this, "getConsumables", GUI_Controls.uiLog.LogLevel.DEBUG, logMSG);
        }

        private void setSharing( SharingProtocol[] allProtocols)
        {
            QuadrantSharing qs = new QuadrantSharing(allProtocols);
            //process sharing information
            qs.queryAllProtocols();
            isSharing = qs.IsSharing;
            sharedSectorsTranslation = qs.SharedSectorsTranslation;
            sharedSectorsProtocolIndex = qs.SharedSectorsProtocolIndex;
        }

        public void addMaintenance(IMaintenanceProtocol MaintenanceProtocol)
        {
            // LOG
            string logMSG = "Adding Maintenance Protocol";
            GUI_Controls.uiLog.LOG(this, "addMaintenance", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

            // remove all other protocols
            for (int i = 0; i < (int)QuadrantId.NUM_QUADRANTS; i++)
            {
                if (myQuadrants[i].QuadrantsRequired > 0)
                    RoboSep_UserConsole.getInstance().mySeparator.DeselectProtocol((QuadrantId)(i));
                myQuadrants[i].Clear();
            }

            // add Maintenance protocol as only protocol in run.

            try
            {
                RoboSep_UserConsole.getInstance().mySeparator.SelectProtocol(QuadrantId.Quadrant1, MaintenanceProtocol);
                RoboSep_UserConsole.getInstance().mySeparator.ScheduleRun(QuadrantId.Quadrant1);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "Selected " + MaintenanceProtocol.Label);
                myQuadrants[0].QuadrantLabel = MaintenanceProtocol.Label;
                myQuadrants[0].bQuadrantInUse = true;
                myQuadrants[0].bIsMaintenance = true;
                myQuadrants[0].Update();
                iProtocolSelectCount = 1;
            }
            catch
            {
                // LOG
                string LOGmsg = "Failed to add Maintenance protocol";
                GUI_Controls.uiLog.LOG(this, "addMaintenance", GUI_Controls.uiLog.LogLevel.ERROR, LOGmsg);
            }
        }

        private bool ConfirmLidClosed()
        {
            bool isLidClosed = mySeparatorGateway.IsLidClosed();
            return isLidClosed;

            // LOG
            string logMSG = "Lid Closed = " + isLidClosed.ToString();
            GUI_Controls.uiLog.LOG(this, "ConfirmLidClosed", GUI_Controls.uiLog.LogLevel.INFO, logMSG);
        }

        public void Unload()
        {
            // remove protocols from form
            for (int i = 0; i < 4; i++)
            {
                myQuadrants[i].Clear();
            }
            iProtocolSelectCount = 0;

            // LOG
            string logMSG = "Clearing all selected protocols";
            GUI_Controls.uiLog.LOG(this, "Unload", GUI_Controls.uiLog.LogLevel.INFO, logMSG);
        }

        public bool disableRun
        {
            set
            {
                button_RunSamples.Enabled = !value;
            }
        }

        private void scheduleRun()
        {
            // protocols are all separation type
            // schedule all runs
            for (int i = 0; i < (int)QuadrantId.NUM_QUADRANTS; i++)
            {
                if (myQuadrants[i].QuadrantsRequired > 0)
                    RoboSep_UserConsole.getInstance().scheduleRun((QuadrantId)(i));
            }
        }

        private void button_RunSamples_Click(object sender, EventArgs e)
        {
            // LOG
            string logMSG = "Run Samples button clicked";
            GUI_Controls.uiLog.LOG(this, "button_RunSamples_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

            // at least 1 protocol must be selected so that this button can be activated
            if (iProtocolSelectCount > 0)
            {
                bool requiresResources = true;
                SharingProtocol[] myRunConfig = getConsumables();
                setSharing(myRunConfig);

                if (ConfirmReadytoRun() && ConfirmLidClosed())
                {
                    // schedule runs
                    scheduleRun();                    
                    // LOG
                    GUI_Controls.uiLog.LOG(this, "button_RunSamples_Click", GUI_Controls.uiLog.LogLevel.DEBUG, "Scheduling run");

                    // update myRunConfig after setting sharing options
                    myRunConfig = getConsumables();

                    // run protocol
                    // load user name to server
                    mySeparatorGateway.BatchRunUserId = RoboSep_UserConsole.strCurrentUser;

                    myReagentLotIds = new ReagentLotIdentifiers[iProtocolSelectCount];
                    for (int i = 0; i < iProtocolSelectCount; i++)
                    {
                        myReagentLotIds[i].Reset();
                        mySeparatorGateway.SetProtocolReagentLotIds(i, myReagentLotIds[i]);
                    }

                    // add protocols to run progress window
                    for (int i = 0; i < 4; i++)
                    {
                        if (myQuadrants[i].QuadrantLabel != string.Empty)
                        {
                            // add protocol to run progress window
                            // get run commands from protocol file
                            int[] commands;
                            int[] times;
                            string[] processDescriptions;

                            string name = string.Empty;
                            // check if protocol is a maintenance protocol
                            if (myQuadrants[0].bIsMaintenance)
                            {
                                // set name based on quadrant label
                                switch (myQuadrants[0].QuadrantLabel)
                                {
                                    case "Basic prime":
                                        name = "prime.xml";
                                        break;
                                    case "Home all":
                                        name = "home_axes.xml";
                                        break;
                                }
                            }
                            else
                            {
                                // set file names for maintenance protocols (file names don't match labels)
                                name = RoboSep_UserDB.getInstance().getProtocolFilename(myQuadrants[i].QuadrantLabel);
                            }
                                
                            RoboSep_UserDB.getInstance().getProtocolCommandList(name, out commands, out times, out processDescriptions);
                            // LOG
                            logMSG = "protocol: " + name + " commands: " + commands.Length.ToString();
                            GUI_Controls.uiLog.LOG(this, "button_RunSamples_Click", GUI_Controls.uiLog.LogLevel.DEBUG, logMSG);
                            RoboSep_RunProgress.RunInfo(i, times, commands, processDescriptions, myQuadrants[i].QuadrantsRequired);

                            if (name == "home_axes.xml")
                            {
                                GUI_Controls.uiLog.LOG(this, "button_RunSamples_Click", GUI_Controls.uiLog.LogLevel.DEBUG, "Home Axex protocol, skip resources");
                                // home does not have any quadrant resources required.  Skip right to run window
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

                                // tell progress bars to begin.
                                RoboSep_RunProgress.getInstance().Start();
                                requiresResources = false;
                                break;
                            }
                        }
                    }

                    if (requiresResources)
                    {
                        // open resources help window
                        RoboSep_Resources ResourceWindow = new RoboSep_Resources(myRunConfig, sharedSectorsTranslation);
                        ResourceWindow.Location = new Point(0, 0);
                        this.Visible = false;
                        RoboSep_UserConsole.getInstance().Controls.Add(ResourceWindow);
                        RoboSep_UserConsole.getInstance().Controls.Remove(RoboSep_UserConsole.ctrlCurrentUserControl);
                        RoboSep_UserConsole.ctrlCurrentUserControl = ResourceWindow;
                    }
                }
            }
        }

        private void CheckHydraulicFluid()
        {         
            bool HydraulicLow = SeparatorGateway.GetInstance().IsHydraulidFluidRefillRequired;
            if (HydraulicLow)
            {
                // LOG
                string logMSG = "Hydraulic fluid lvl low";
                GUI_Controls.uiLog.LOG(this, "CheckHydraulicFluid", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

                string sMSG = GUI_Controls.fromINI.getValue("GUI", "msgHydraulicLow");
                sMSG = sMSG == null ? "Hydraulic fluid level is low.  Top up fluid before continuing." : sMSG;
                sMSG = SeparatorResourceManager.GetSeparatorString(StringId.HydraulicFluidLevelWarning);
                RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), sMSG, "Hydraulic Fluid Low", "Refilled", "Ignore");

                RoboSep_UserConsole.getInstance().frmOverlay.Show();
                prompt.ShowDialog();
                RoboSep_UserConsole.getInstance().frmOverlay.Hide();

                if (prompt.DialogResult == DialogResult.OK)
                {
                    mySeparatorGateway.ConfirmHydraulicFluidRefilled();
                    
                    // LOG
                    logMSG = "User confirmed fluid top up";
                    GUI_Controls.uiLog.LOG(this, "CheckHydraulicFluid", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
                }
            }
        }

        public void startSeparationRun()
        {
            mySeparatorGateway.StartRun(isSharing, sharedSectorsTranslation, sharedSectorsProtocolIndex);
            RoboSep_RunProgress.getInstance().Sharing = isSharing;

            // LOG
            string logMSG = "Start Separation run";
            GUI_Controls.uiLog.LOG(this, "CheckHydraulicFluid", GUI_Controls.uiLog.LogLevel.GENERAL, logMSG);
        }

        private void Q1_Select(object sender, EventArgs e)
        {
            Protocol_Selection(0);
            ActiveControl = button_Scan;
        }
        private void Q2_Select(object sender, EventArgs e)
        {
            Protocol_Selection(1);
            ActiveControl = button_Scan;
        }
        private void Q3_Select(object sender, EventArgs e)
        {
            Protocol_Selection(2);
            ActiveControl = button_Scan;
        }
        private void Q4_Select(object sender, EventArgs e)
        {
            Protocol_Selection(3);
            ActiveControl = button_Scan;
        }

        private void Protocol_Selection(int QuadrantNumber)
        {
            // don't allow protocol selection if maintenance
            // protocol is present
            if (myQuadrants[0].bIsMaintenance)
            {
                string sMSG = GUI_Controls.fromINI.getValue("GUI", "msgProtocolMixing");
                sMSG = sMSG == null ? "Maintenance protocol selected. Maintenance and Separation protocols can not "
                 + "be included in the same run.  \r\n\r\nTo continue with Separation protocol selection, remove Maintenance protocol from quadrant 1." : sMSG;
                
                string sMSG2 = GUI_Controls.fromINI.getValue("GUI", "msgMaintenancePresent");
                sMSG2 = sMSG2 == null ? "To select alternate Maintenance protocol return to System window.  To select Separation protocols"
                    + " first remove Maintenance protocol from quadrant 1." : sMSG2;

                sMSG = QuadrantNumber != 0 ? sMSG : sMSG2;
                   
                RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), sMSG);
                RoboSep_UserConsole.getInstance().frmOverlay.Show();
                prompt.ShowDialog();
                RoboSep_UserConsole.getInstance().frmOverlay.Hide();
            }
            else
            {
                // count available quadrants
                int available = myQuadrants[QuadrantNumber].QuadrantsRequired;
                for (int i = (QuadrantNumber + myQuadrants[QuadrantNumber].QuadrantsRequired); i < 4; i++)
                {
                    if (myQuadrants[i].bQuadrantInUse)
                        break;
                    available += 1;
                }
                // Sends user to Protocol Selection window
                RoboSep_ProtocolSelect ProtocolSelectWindow = new RoboSep_ProtocolSelect(QuadrantNumber, available);
                ProtocolSelectWindow.Location = new Point(0, 0);
                this.Visible = false;
                RoboSep_UserConsole.getInstance().Controls.Add(ProtocolSelectWindow);
                RoboSep_UserConsole.getInstance().Controls.Remove(RoboSep_UserConsole.ctrlCurrentUserControl);
                RoboSep_UserConsole.ctrlCurrentUserControl = ProtocolSelectWindow;

                // pass quadrant value to protocol select window

                // LOG
                string logMSG = "Opening protocol selection for quadrant " + (QuadrantNumber +1).ToString() ;
                GUI_Controls.uiLog.LOG(this, "Protocol_Selection", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            }
        }


        private void Q1_Vol_Click(object sender, EventArgs e)
        {
            EditVolume(0);
        }
        private void Q2_Vol_Click(object sender, EventArgs e)
        {
            EditVolume(1);
        }
        private void Q3_Vol_Click(object sender, EventArgs e)
        {
            EditVolume(2);
        }
        private void Q4_Vol_Click(object sender, EventArgs e)
        {
            EditVolume(3);
        }

        void EditVolume(int Q)
        {
            if (myQuadrants[Q].bQuadrantInUse)
            {
                // check if quadrant is used by another protocol (above)
                if (myQuadrants[Q].QuadrantVolume == 0)
                { /* do nothing */ }
                else
                {
                    // LOG
                    string logMSG = "Modifying volume for quadrant " + (Q + 1).ToString();
                    GUI_Controls.uiLog.LOG(this, "Protocol_Selection", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

                    // get info on quadrant
                    ISeparationProtocol[] SelectedProtocols = RoboSep_UserConsole.getInstance().XML_getUserProtocols();

                    for (int i = 0; i < SelectedProtocols.Length; i++)
                    {
                        if (myQuadrants[Q].QuadrantLabel == SelectedProtocols[i].Label)
                        {
                            RoboSep_UserConsole.getInstance().frmOverlay.Show();
                            // open number pad control
                            NumberPad numPad = new NumberPad(RoboSep_UserConsole.getInstance(), RoboSep_UserConsole.getInstance().frmOverlay,
                                myQuadrants[Q].GetVolumeLabel(), myQuadrants[Q].Quadrant_message,
                                myQuadrants[Q].volMin, myQuadrants[Q].volMax);
                            numPad.ShowDialog();
                            RoboSep_UserConsole.getInstance().frmOverlay.Hide();

                            // update with results of volume edit
                            myQuadrants[Q].updateVolume();
                            myQuadrants[Q].Update();

                            // inform server of volume update
                            FluidVolume QVolume = new FluidVolume(myQuadrants[Q].QuadrantVolume, Tesla.Common.FluidVolumeUnit.MilliLitres);
                            RoboSep_UserConsole.getInstance().SetSampleVolume((QuadrantId)(Q), QVolume);
                        }
                    }
                }
            }
            else
            {
                Protocol_Selection(Q);
            }
        }

        private void Q1_Cncl(object sender, EventArgs e)
        {
            CancelQuadrant(0);
        }

        private void Q2_Cncl(object sender, EventArgs e)
        {
            CancelQuadrant(1);
        }

        private void Q3_Cncl(object sender, EventArgs e)
        {
            CancelQuadrant(2);
        }

        private void Q4_Cncl(object sender, EventArgs e)
        {
            CancelQuadrant(3);
        }

        public void CancelQuadrant(int Q)
        {
            if (myQuadrants[Q].bQuadrantInUse)
            {
                // LOG
                string logMSG = "Removing protocol at quadrant " + (Q+1).ToString();
                GUI_Controls.uiLog.LOG(this, "CancelQuadrant", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
                
                // remove from servers chosen protocol list
                iProtocolSelectCount -= 1;
                RoboSep_UserConsole.getInstance().mySeparator.DeselectProtocol((QuadrantId)(Q));

                // remove from sample processing selection
                int repeat = myQuadrants[Q].QuadrantsRequired;
                myQuadrants[Q].Clear();
                for (int i = 0; i < repeat; i++)
                {
                    myQuadrants[Q + i].Clear();
                }
            }
            else
                Protocol_Selection(Q);
        }

    }
}
