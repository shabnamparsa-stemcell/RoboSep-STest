using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using GUI_Controls;

using Tesla.OperatorConsoleControls;
using Tesla.Separator;

using Tesla.Common;
using Tesla.Common.DrawingUtilities;
using Tesla.Common.ResourceManagement;
using Tesla.Common.OperatorConsole;
using Tesla.Common.Separator;
using Tesla.Common.Protocol;

namespace GUI_Console
{
    public partial class RoboSep_RunProgress : BasePannel
    {
        private static RoboSep_RunProgress myRunProgress;
        private QuadrantProgress[] myQuadrantProgress;
        private SeparatorGateway mySeparatorGateway;
        private string PauseButtonState;
        // progress tracking variables
        private int CurrentProtocolID = -2;
        private int CurrentProtocolQuadrant = -2;
        private int currentProtocolSequence = -2;
        private int UpdateCounter = -1;
        private QuadrantProgress.ProtocolCommand CurrentProtocolComand;        

        private RoboMessagePanel WaitingMSG;
        private int[] iQuadrantProxy;
        private int[] QuadrantIDs;
        private string[,] CWJ_CommandList;
        private int[] IDSteps;
        private bool isSharing = false;
        private int[] CWJ_nProtocols;
        private int CWJ_nShareRunning;
        private int[] CWJ_QuadrantIDs;
        private int[] CWJ_IDQuadrants;
        private int[] CWJ_IDSteps;
        private int CWJ_ShareOffset;
        private string CompletionStatus;

        private RoboSep_RunProgress()
        {
            InitializeComponent();

            // Create QuadrantProgress objects for each quadrant
            // objects used to control sub objects used to display progress
            myQuadrantProgress = new QuadrantProgress[5] {
                new QuadrantProgress(Q1progress, labelQ1_current, lblQ1Current, labelQ1_previous, lblQ1previous, lblQ1Complete, panelProgressq1),
                new QuadrantProgress(Q2progress, labelQ2_current, lblQ2Current, labelQ2_previous, lblQ2previous, lblQ2Complete, panelProgressq2),
                new QuadrantProgress(Q3progress, labelQ3_current, lblQ3Current, labelQ3_previous, lblQ3previous, lblQ3Complete, panelProgressq3),
                new QuadrantProgress(Q4progress, labelQ4_current, lblQ4Current, labelQ4_previous, lblQ4previous, lblQ4Complete, panelProgressq4),
                new QuadrantProgress(ALLprogress, labelElapsed, elapsed, labelEstimated, estimated, ALLComplete, panelALL)};
            
            // initialize Toatal Run progress bar
            myQuadrantProgress[4].isActive = true;
            int[] bars = new int[] { 500000 };
            int[] colours = new int[] { 0 };
            myQuadrantProgress[4].myProgressBar.setElements(bars, colours);
            myQuadrantProgress[4].myProgressBar.showTime = false;

            // Pause button changes states, initialize to "pause" state
            PauseButtonState = "Pause";

            // add delegate for batch time estimate
            mySeparatorGateway = SeparatorGateway.GetInstance();

            iQuadrantProxy = new int[4] { 0, 1, 2, 3 };
            QuadrantIDs = new int[4] { -1, -1, -1, -1 };
            CWJ_QuadrantIDs = new int[4] {-1, -1, -1, -1};
            IDSteps = new int[4] { -1, -1, -1, -1};
            CWJ_IDQuadrants = new int[4] { -1, -1, -1, -1 };
            CWJ_nProtocols = new int[2] {0,0};
            CWJ_IDSteps = new int[4] { -1, -1, -1, -1 };
            CWJ_ShareOffset = -1;
            CWJ_nShareRunning = 0;
        }

        // Creates singleton of RunProgress window
        public static RoboSep_RunProgress getInstance()
        {
            if (myRunProgress == null)
            {
                myRunProgress = new RoboSep_RunProgress();
            }
            return myRunProgress;
        }

        public bool Sharing
        {
            set
            {
                isSharing = value;
            }
        }

        // update progress us activated on a delay
        // due to a communication issue between GUI and server.
        // incorrect values of ID STEP and QUAD
        // are sent when delegate updates system with
        // atReportStatus delegate
        private void UpdateProgress(int ID, int STEP, int QUAD, string CMD)
        {
            int updateQuad = -1;

            // use iquadrantproxy to determine appropriate quadrant to update
            if ((STEP < 2 && QUAD > -1) || ID < 0)
            {
                updateQuad = iQuadrantProxy[QUAD];
                QuadrantIDs[ID] = updateQuad;
            }

            // set prev quadrants steps to complete 
            //unless they are separate or incubate
            for (int i = 0; i < 4; i++)
            {
                if (QuadrantIDs[i] != -1)
                {
                    myQuadrantProgress[QuadrantIDs[i]].CompleteStep();
                }
            }

            // set update quadrant based on ID value
            updateQuad = QuadrantIDs[ID];

            // update information based on IDStep and QuadrantIDs
            myQuadrantProgress[updateQuad].UpdateStep( IDSteps[ID] );
        }

        // update progress us activated on a delay
        // due to a communication issue between GUI and server.
        // incorrect values of ID STEP and QUAD
        // are sent when delegate updates system with
        // atReportStatus delegate
        private void UpdateProgressOLDOLD( int ID, int STEP, int QUAD, string CMD)
        {
            int tempQuad = 0;

            
            // use iquadrantproxy to determine appropriate quadrant to update
            if ((STEP < 2 && QUAD > -1) || ID < 0)
            {
                tempQuad = iQuadrantProxy[QUAD];
                QuadrantIDs[ID] = tempQuad;
            }

            // set update quadrant based on ID value
            tempQuad = QuadrantIDs[ID];


            for (int i = 0; i < 4; i++)
            {
                if (myQuadrantProgress[i].isActive)
                {
                    if (!myQuadrantProgress[i].onTimedAction())
                    {
                        // set curret step text as previous step text
                        myQuadrantProgress[i].currentToPrevious();
                    }
                }
            }

            myQuadrantProgress[tempQuad].NextStep(CMD, STEP);
            
            // unless on incubate or separate command, complete previous step
            if (CurrentProtocolComand != QuadrantProgress.ProtocolCommand.IncubateCommand 
                && CurrentProtocolComand != QuadrantProgress.ProtocolCommand.SeparateCommand
                && currentProtocolSequence > -1)
            {
                List<GUI_Controls.ProgressElement> tmpElement = myQuadrantProgress[CurrentProtocolQuadrant].myProgressBar.elements;
                tmpElement[currentProtocolSequence].setProgress(100);
            }

            // update current information
            CurrentProtocolComand = getCMDtype(CMD);
            CurrentProtocolID = ID;
            CurrentProtocolQuadrant = tempQuad;
            currentProtocolSequence = STEP;
        }

        // check if current status is the same as stored (old) status
        private void pollProgressUpdate()
        {
            int id;
            int step;
            int quad;
            string cmd;
            bool isUpdate = false;

            isUpdate = RoboSep_UserConsole.getInstance().checkProgressUpdate(UpdateCounter);
            RoboSep_UserConsole.getInstance().updateRunProgress(UpdateCounter, out id, out step, out quad, out cmd);
            // check that all quadrants are runing their appropriate commands

            if ((isSharing))                //CWJ ADD
            {
                id = id - CWJ_ShareOffset;
            }

            if ( isUpdate )
            {
                // check if progress values are change with command
                // start timer, to rapidly check for updates
                UpdateCounter++;
                
                //add to updates to process
                string str = String.Format("CWJ2--> ** checkProgressUpdate ID: {0}, Seq: {1}, Qdr: {2}", id, step, quad);
                System.Diagnostics.Debug.WriteLine(str);

                if (id > -1 && step > -1)
                {
                    IDSteps[id] = step;
                    CWJ_IDSteps[id] = step;

                    UpdateProgress(id, step, quad, cmd);

                    // LOG
                    string sTemp = "Progress Update: Q = " + quad.ToString() + " Step = " + step.ToString() + " ID = " + id.ToString() + " CMD = " + cmd;
                    GUI_Controls.uiLog.LOG(this, "pollProgressUpdate", uiLog.LogLevel.EVENTS, sTemp);
                }
                // protocol is a maintenance protocol or a flush event
                else if ( 4 > myQuadrantProgress[0].myProgressBar.elements.Count)
                {
                    UpdateProgressOLDOLD(0, UpdateCounter-1, 0, cmd);
                }
            }

        }

        private QuadrantProgress.ProtocolCommand getCMDtype(string cmd)
        {
            switch (cmd)
            {
                case "Incubate":
                    return QuadrantProgress.ProtocolCommand.IncubateCommand;
                case "Mix":
                    return QuadrantProgress.ProtocolCommand.MixCommand;
                case "Transport":
                    return QuadrantProgress.ProtocolCommand.TransportCommand;
                case "Separate":
                    return QuadrantProgress.ProtocolCommand.SeparateCommand;
                case "Resuspend Vial":
                    return QuadrantProgress.ProtocolCommand.ResuspendVialCommand;
                case "TopUp Vial":
                    return QuadrantProgress.ProtocolCommand.TopUpVialCommand;
                case "Flush":
                    return QuadrantProgress.ProtocolCommand.FlushCommand;
                case "Prime":
                    return QuadrantProgress.ProtocolCommand.PrimeCommand;
                case "Home All":
                    return QuadrantProgress.ProtocolCommand.HomeAllCommand;
                case "Demo":
                    return QuadrantProgress.ProtocolCommand.DemoCommand;
                case "Park":
                    return QuadrantProgress.ProtocolCommand.ParkCommand;
                case "PumpLife":
                    return QuadrantProgress.ProtocolCommand.PumpLifeCommand;
            }
            return QuadrantProgress.ProtocolCommand.NULL;
        }

        private void RoboSep_RunProgress_Load(object sender, EventArgs e)
        {
            // load proper graphics for buttons
            List<Image> ilist = new List<Image>();
            // abort button
            ilist.Add(GUI_Controls.Properties.Resources.Button_ABBORT0);
            ilist.Add(GUI_Controls.Properties.Resources.Button_ABBORT1);
            ilist.Add(GUI_Controls.Properties.Resources.Button_ABBORT2);
            ilist.Add(GUI_Controls.Properties.Resources.Button_ABBORT3);
            button_Abort.ChangeGraphics(ilist);
            // pause button
            ilist.Clear();
            ilist.Add(GUI_Controls.Properties.Resources.Button_RECT_MED0);
            ilist.Add(GUI_Controls.Properties.Resources.Button_RECT_MED1);
            ilist.Add(GUI_Controls.Properties.Resources.Button_RECT_MED2);
            ilist.Add(GUI_Controls.Properties.Resources.Button_RECT_MED3);
            button_Pause.ChangeGraphics(ilist);

            // update myQuadrantProgress
            for (int i = 0; i < 4; i++)
                myQuadrantProgress[i].Refresh();
        }

        public static void RunInfo(int Q, int[] stepTimes, int[] steps, string[] processDescriptions, int numQuadrants)
        {
            // display quadrant progress
            RoboSep_RunProgress.getInstance().myQuadrantProgress[Q].Clear();
            RoboSep_RunProgress.getInstance().myQuadrantProgress[Q].isActive = true;
            RoboSep_RunProgress.getInstance().myQuadrantProgress[Q].myProgressBar.setElements(stepTimes, steps);
            RoboSep_RunProgress.getInstance().myQuadrantProgress[Q].Refresh();
            RoboSep_RunProgress.getInstance().myQuadrantProgress[Q].ProcessDescriptions = processDescriptions;
            for (int i = 0; i < numQuadrants && Q < 4; i++)
                RoboSep_RunProgress.getInstance().iQuadrantProxy[Q + i] = Q;
        }

        

        public void Update()
        {
            // quiery server to see what stage it is at
            // update current / previous information

            // update TotalProgress
            double OutOf = 0;
            double Progress = 0;
            for (int i = 0; i < 4; i++)
            {
                if (myQuadrantProgress[i].isActive)
                {
                    OutOf += 1.00;
                    Progress += (double)myQuadrantProgress[i].myProgressBar.progress;
                }
            }
            // calculate progress
            double totalProgress = Progress / OutOf;
            myQuadrantProgress[4].myProgressBar.setProgress( (int)totalProgress );
            myQuadrantProgress[4].myCurrentStep = myQuadrantProgress[4].myProgressBar.elapse;
            myQuadrantProgress[4].Refresh();

            // check if batch run is complete
            if (mySeparatorGateway.SeparatorState == SeparatorState.BatchComplete && PauseButtonState != "Unload")
            {
                // turn off timer
                updateTimer.Stop(); 
                CWJ_Timer.Stop();
                // set PauseButtonState to "Unload"
                PauseButtonState = "Unload";
                button_Pause.AccessibleName = PauseButtonState;
                button_Pause.Refresh();

                // set all protocols to 100% complete
                for (int i = 0; i < 5; i++)
                {
                    if (myQuadrantProgress[i].isActive)
                    {
                        myQuadrantProgress[i].Completed();
                    }
                }

                // hide Abort button
                button_Abort.Visible = false;

                // prompt user that run is complete
                string completionTime = string.Format("{0:HH:mm}", DateTime.Now);
                string sMSG = GUI_Controls.fromINI.getValue("GUI", "msgRunComplete1");
                sMSG = sMSG == null ? "Batch run completed successfully at " : sMSG;
                sMSG += completionTime + "\r\n";

                string sMSG2 = GUI_Controls.fromINI.getValue("GUI", "msgRunComplete2");
                sMSG2 = sMSG2 == null ? "Click Unload button on Run Samples window to begin new run" : sMSG2;

                sMSG += sMSG2;

                RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(),
                    sMSG, "Batch Run Completed", "Ok");
                RoboSep_UserConsole.getInstance().frmOverlay.Show();
                prompt.ShowDialog();
                RoboSep_UserConsole.getInstance().frmOverlay.Hide();
                
            }
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            int num = 0, id, seq, sec;
            string cmd;
            RoboSep_UserConsole.getInstance().updateRunProgress(num, out id, out seq, out sec, out cmd);
            
            //if ((id > -1) && (seq > -1))  CWJ Remark
            //    IDSteps[id] = seq;
            
            // look for update
            Update();
            pollProgressUpdate();

            // test if runs are completed
            bool allCompleted = true;
            for (int i = 0; i < 4; i++)
            {
                if (myQuadrantProgress[i].myProgressBar.progress == 100
                    && !myQuadrantProgress[i].myLabelCompleted.Visible)
                {
                    myQuadrantProgress[i].Completed();
                }
                if (myQuadrantProgress[i].isActive && !myQuadrantProgress[i].myLabelCompleted.Visible)
                    allCompleted = false;
            }
        }

        public void Start()
        {
            // set protocol IDs
            int lastQaud = 0;
            int count = 0;       

#if false
            for (int i=0; i<4; i++)
            {
                if (myQuadrantProgress[i].isActive)
                {
                    QuadrantIDs[count] = i;
                    count++;
                }
            }
#else
            int k = -1, offset = -1;
            CWJ_QuadrantIDs[0] = -1;
            CWJ_QuadrantIDs[1] = -1;
            CWJ_QuadrantIDs[2] = -1;
            CWJ_QuadrantIDs[3] = -1;
            CWJ_QuadrantIDs[0] = -1;
            CWJ_QuadrantIDs[1] = -1;
            CWJ_QuadrantIDs[2] = -1;
            CWJ_QuadrantIDs[3] = -1;
            CWJ_nProtocols[1] += CWJ_nProtocols[0];
            CWJ_nProtocols[0] = 0;

            IDSteps[0] = -1;
            IDSteps[1] = -1;
            IDSteps[2] = -1;
            IDSteps[3] = -1;

            CWJ_IDSteps[0] = -1;
            CWJ_IDSteps[1] = -1;
            CWJ_IDSteps[2] = -1;
            CWJ_IDSteps[3] = -1;
            
            CWJ_ShareOffset = -1;
            
            for (int i = 0; i < 4; i++)
            {
                offset = i;
                if (offset <= k)
                    offset = k;
                for (k = offset; k < 4; k++)
                {
                    if (myQuadrantProgress[k].isActive)
                    {
                        CWJ_QuadrantIDs[i] = k;
                        QuadrantIDs[i] = k;
                        k++;
                        (CWJ_nProtocols[0])++;
                        break;
                    }                
                }                
                if (k > 3)
                        break;
            }

            if (isSharing)
            {
                CWJ_nShareRunning++;
                CWJ_ShareOffset = CWJ_nProtocols[1] + 1;
            }
#endif
            // GET CMD LIST FOR CHUL
            // determine array size
            int cmdListLngth = 0;
            for (int i=0; i<4; i++)
            {
                if (myQuadrantProgress[i].isActive)
                {
                    int quadrantListLength = myQuadrantProgress[i].getCommandList().Length;
                    if (quadrantListLength > cmdListLngth)
                        cmdListLngth = quadrantListLength;
                }
            }
            // create 2d array
            CWJ_CommandList = new string[4, cmdListLngth];
#if false
            for (int i=0; i<4; i++)
            {
                string[] cmdList = null;
                if (myQuadrantProgress[i].isActive)
                {
                    cmdList = myQuadrantProgress[i].getCommandList();
                }
                if (cmdList != null )
                for (int j = 0; j < cmdList.Length; j++)
                {
                    CWJ_CommandList[i, j] = cmdList[j];
                }

            }
#else
            int j = 0;
            for (int i = 0; i < 4; i++)
            {
                string[] cmdList = null;                
                if (myQuadrantProgress[i].isActive)
                {
                    cmdList = myQuadrantProgress[i].getCommandList();
                }
                if (cmdList != null)
                {
                    for (int m = 0; m < cmdList.Length; m++)
                    {
                        CWJ_CommandList[j, m] = cmdList[m];
                    }
                    j++;
                }
            }
#endif

            updateTimer.Start();
            CWJ_Timer.Start();
            labelEstimated.Text = RoboSep_UserConsole.getInstance().EstimatedCompletion;
            myQuadrantProgress[4].myPreviousStep = RoboSep_UserConsole.getInstance().EstimatedCompletion;
            myQuadrantProgress[4].Refresh();

            // LOG
            string sTemp = " **INITIALIZING RUN";
            GUI_Controls.uiLog.LOG(this, "Start", uiLog.LogLevel.EVENTS, sTemp);

            for (int i = 0; i < 4; i++)
            {
                count = 0;
                if (myQuadrantProgress[i].isActive)
                {
                    myQuadrantProgress[i].quadrantID = count;
                    myQuadrantProgress[i].myCurrentStep = "Initializing";
                    myQuadrantProgress[i].myProgressBar.StartTimer();
                    myQuadrantProgress[i].Refresh();
                    count++;
                }
            }
            CurrentProtocolQuadrant = 0;
            CurrentProtocolID = 0;
            UpdateCounter = 0;
        }

        private void button_Abort_Click(object sender, EventArgs e)
        {
            string sMSG = GUI_Controls.fromINI.getValue("GUI", "msgHalt");
            sMSG = sMSG == null ? "Are you sure you want to abort the current run?" : sMSG;

            RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(),
                sMSG, "Abort Run", "Yes", "Cancel");
            RoboSep_UserConsole.getInstance().frmOverlay.Show();
            prompt.ShowDialog();
            RoboSep_UserConsole.getInstance().frmOverlay.Hide();
            if (prompt.DialogResult == DialogResult.OK)
            {
                // abor trun
                Abbort();
            }
        }

        public void Abbort()
        {
            // Tell the Separator to halt the batch run
            mySeparatorGateway.ControlApi.HaltRun();
            CompletionStatus = "Run Aborted";

            PauseTimer.Start();
            halted = false;

            string sMSG = GUI_Controls.fromINI.getValue("GUI", "msgHalting");
            sMSG = sMSG == null ? "Halting current run.  Wiat while Robot is disenguaged." : sMSG;

            WaitingMSG = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), sMSG, true, false);
            RoboSep_UserConsole.getInstance().frmOverlay.Show();
            WaitingMSG.ShowDialog();
            //StringId.RunHaltingText;

            // hide all quadrants, will be unhidden
            // when they are re-added
            for (int i = 0; i < 5; i++)
                myQuadrantProgress[i].Clear();

            
            //reset Total Progress
            myQuadrantProgress[4].isActive = true;
            int[] bars = new int[] { 500000 };
            int[] colours = new int[] { 0 };
            myQuadrantProgress[4].myProgressBar.setElements(bars, colours);
            myQuadrantProgress[4].Refresh();

            /*
            // move to run samples window
            RoboSep_RunSamples.getInstance().Unload();

            // unload carousel
            mySeparatorGateway.ControlApi.ParkArm();
            mySeparatorGateway.ControlApi.Unload();

            leaveRunProgress();
            */
        }

        public void Unload(UserControl RoboSepWindow)
        {
            // hide all quadrants, will be unhidden
            // when they are re-added
            updateTimer.Stop();
            CWJ_Timer.Stop();
            for (int i = 0; i < 5; i++)
            {
                myQuadrantProgress[i].myProgressBar.Stop();
                myQuadrantProgress[i].Clear();
            }

            // reset Total Progress
            int[] bars = new int[] { 500000 };
            int[] colours = new int[] { 0 };
            RoboSep_RunProgress.RunInfo(4, bars, colours, new string[] { "Total Progress" }, 1);

            // use robosep server communication
            // to unload protocols from db
            if (RoboSepWindow == this)
            {
                mySeparatorGateway.ControlApi.ParkArm();
                mySeparatorGateway.ControlApi.Unload();
                PauseTimer.Start();
                unloaded = false;

                // open message dialog that makes user wait while system unloads
                string sMSG = GUI_Controls.fromINI.getValue("GUI", "msgUnloading");
                sMSG = sMSG == null ? "Unloading in progress.  Carousel unloading can take up to 60 seconds." : sMSG;
                WaitingMSG = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), sMSG, true, false);
                if (!halted)
                    RoboSep_UserConsole.getInstance().frmOverlay.Show();
                WaitingMSG.ShowDialog();
            }

            iQuadrantProxy = new int[] { 0, 1, 2, 3 };
            QuadrantIDs = new int[4] { -1, -1, -1, -1 };
            IDSteps = new int[4] { -1, -1, -1, -1 };

            CurrentProtocolID = -2;
            CurrentProtocolQuadrant = -2;
            currentProtocolSequence = -2;
            UpdateCounter = -1;

            UpdateCounter = -2;
            isSharing = false;
        }

        private bool halted = false;
        private bool paused = false;
        private bool unloaded = false;
        private void PauseTimer_Tick(object sender, EventArgs e)
        {
            if (mySeparatorGateway.SeparatorState == SeparatorState.BatchHalted && !halted)
            {
                // reset everything to defaults
                PauseTimer.Stop();
                // update server..
                halted = true;
                WaitingMSG.closeDialogue(DialogResult.OK);
                RoboSep_RunSamples.getInstance().disableRun = false;
                Unload(this);
            }
            else if (mySeparatorGateway.SeparatorState == SeparatorState.Paused && !paused)
            {
                WaitingMSG.closeDialogue(DialogResult.OK);
                RoboSep_UserConsole.getInstance().frmOverlay.Hide();
                PauseTimer.Stop();

                // stop timers and progress bars
                for (int i = 0; i < 4; i++)
                {
                    if (myQuadrantProgress[i].isActive)
                    {
                        myQuadrantProgress[i].myProgressBar.Pause();
                    }
                    button_Pause.AccessibleName = PauseButtonState;
                    button_Pause.Refresh();
                }
            }
            else if (mySeparatorGateway.SeparatorState == SeparatorState.SeparatorUnloaded && !unloaded)
            {
                // reset everything to defaults
                PauseTimer.Stop();
                WaitingMSG.closeDialogue(DialogResult.OK);
                RoboSep_UserConsole.getInstance().frmOverlay.Hide();
                RoboSep_RunSamples.getInstance().disableRun = false;
                // update server..
                leaveRunProgress();
            }
        }               

        private void leaveRunProgress()
        {
            // turn pause timer on
            //UnloadMinTimer.Start();
            updateTimer.Stop();
            CWJ_Timer.Stop();
            
            // move to run samples window
            RoboSep_RunSamples.getInstance().Unload();
            // clear run progress
            RoboSep_UserConsole.getInstance().ClearProgress();

            // tell robosep it is no longer in run mode
            RoboSep_UserConsole.bIsRunning = false;

            // if current window is "this" switch to run sampling window
            if (RoboSep_UserConsole.ctrlCurrentUserControl == this)
            {
                // switch to RunSamples page
                RoboSep_RunSamples.getInstance().Visible = false;
                RoboSep_RunSamples.getInstance().Location = new Point(0, 0);
                RoboSep_UserConsole.getInstance().Controls.Add(RoboSep_RunSamples.getInstance());
                RoboSep_UserConsole.getInstance().Controls.Remove(RoboSep_UserConsole.ctrlCurrentUserControl);
                RoboSep_UserConsole.ctrlCurrentUserControl = RoboSep_RunSamples.getInstance();
                RoboSep_RunSamples.getInstance().Visible = true;
            }

            // re- set progress window
            PauseButtonState = "Pause";
            button_Abort.Visible = true;
            button_Pause.AccessibleName = PauseButtonState;
            button_Pause.Refresh();
        }

        private void button_Pause_Click(object sender, EventArgs e)
        {   
            // check if button is in Pause or Resume state
            switch (PauseButtonState)
            {
                case "Pause":
                    // pause run
                    // promt user, do you really want to pause this run
                    string sMSG = GUI_Controls.fromINI.getValue("GUI", "msgPause");
                    sMSG = sMSG == null ? "Are you sure you want to pause the run currently in progress?" : sMSG;

                    GUI_Controls.RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(),
                        sMSG, "Pause", "Pause", "Cancel");
                    RoboSep_UserConsole.getInstance().frmOverlay.Show();
                    prompt.ShowDialog();
                    RoboSep_UserConsole.getInstance().frmOverlay.Hide();
                    
                    if (prompt.DialogResult == DialogResult.OK)
                    {
                        updateTimer.Stop();
                        CWJ_Timer.Stop();

                        // set puase button state to "resume"
                        PauseButtonState = "Resume";

                        // pause run through server
                        mySeparatorGateway.ControlApi.PauseRun();
                                                
                        // open dialogue window while system pauses
                        string sMSG2 = GUI_Controls.fromINI.getValue("GUI", "msgPausing");
                        sMSG2 = sMSG2 == null ? "The instrument is pausing, please wait while the instrument reaches a safe state to pause. This can take up to 90 seconds" : sMSG2;
                        WaitingMSG = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), sMSG2, true, false);
                        PauseTimer.Start();
                        
                        RoboSep_UserConsole.getInstance().frmOverlay.Show();
                        WaitingMSG.ShowDialog();
                        // wait for separator state to become paused
                    }
                    break;
                // in Resume Mode
                case "Resume":
                    mySeparatorGateway.ControlApi.ResumeRun();
                    PauseButtonState = "Pause";
                    button_Pause.AccessibleName = PauseButtonState;
                    button_Pause.Refresh();
                    for (int i = 0; i < 4; i++)
                    {
                        if (myQuadrantProgress[i].isActive && myQuadrantProgress[i].myProgressBar.progress > 0)
                        myQuadrantProgress[i].myProgressBar.Start();
                    }
                    updateTimer.Start();
                    CWJ_Timer.Start();
                    break;
                // is in Unload Mode
                case "Unload":
                    Unload( this );
                    //leaveRunProgress();
                    break;
            }
        }

        

        private int unloadSecCount = 0;
        private void UnloadMinTimer_Tick(object sender, EventArgs e)
        {
            unloadSecCount++;
            if (unloadSecCount == 5)
            {
                UnloadMinTimer.Stop();
                unloadSecCount = 0;
                PauseTimer.Start();
            }
        }

        private void CWJ_Timer_Tick(object sender, EventArgs e)
        {
            // don't update on maintenance run
            if (4 <= myQuadrantProgress[0].myProgressBar.elements.Count)
            {
                int num = 0, id, seq, sec, ProtocolID, qdr, j, k = 0, newid =-1;
                string cmd, str, strstep;
                string[] strQ;

                strQ = new string[2];

                RoboSep_UserConsole.getInstance().updateRunProgress(num, out id, out seq, out sec, out cmd);

                
                if ((isSharing))
                {
                    id = id - CWJ_ShareOffset;
                }

                if ((id > -1) && (seq > -1))
                {
                    CWJ_IDSteps[id] = seq;
                    
                    IDSteps[id] = seq;
                    
                    System.Diagnostics.Debug.WriteLine("----------------------");
                    for (ProtocolID = 0; ProtocolID < 4; ProtocolID++)
                    {
                        qdr = CWJ_QuadrantIDs[ProtocolID];
                        if (qdr != -1)
                        {
                            str = String.Format("* Quadrant : {0}", qdr + 1);
                            strstep = String.Format(" Step : {0}", CWJ_IDSteps[ProtocolID] + 1);

                            if (CWJ_IDSteps[ProtocolID] < 0)
                            {
                                strQ[0] = "Previous: N/A";
                                strQ[1] = "Current:  Init";
                            }
                            else if (CWJ_IDSteps[ProtocolID] == 0)
                            {
                                strQ[0] = "Previous: Init";
                                strQ[1] = "Current: " + CWJ_CommandList[ProtocolID, CWJ_IDSteps[ProtocolID]] + strstep;
                            }
                            else
                            {
                                strQ[0] = "Previous: " + CWJ_CommandList[ProtocolID, CWJ_IDSteps[ProtocolID] - 1];
                                strQ[1] = "Current: " + CWJ_CommandList[ProtocolID, CWJ_IDSteps[ProtocolID]] + strstep;
                            }

                            System.Diagnostics.Debug.WriteLine(str);
                            System.Diagnostics.Debug.WriteLine(strQ[1]);
                            System.Diagnostics.Debug.WriteLine(strQ[0]);

                        }
                    }
                }

                // check if all steps are running their appropriate commands
                // if not initiate progress update for missing step

                for (int i = 0; i < 4; i++)
                {
                    if (QuadrantIDs[i] >= 0)
                    {
                        // check current tracked step vs IDsteps
                        // non matching value means missed command
                        if (IDSteps[i] >= 0 && myQuadrantProgress[QuadrantIDs[i]].currentStepNum != IDSteps[i])
                        {
                            if (missingUpdate)
                            {
                                UpdateProgress(i, IDSteps[i], QuadrantIDs[id], "Missing");
                                missingUpdate = false;
                            }
                            else
                            {
                                missingUpdate = true;
                                return;
                            }

                        }
                    }
                }
                missingUpdate = false;
            }
        }

        bool missingUpdate = false;
    }
}
