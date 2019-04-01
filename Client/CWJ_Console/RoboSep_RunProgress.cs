using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Threading;

using Invetech.ApplicationLog;
using GUI_Controls;
using Tesla.OperatorConsoleControls;
using Tesla.Separator;

using Tesla.Common;
using Tesla.Common.DrawingUtilities;
using Tesla.Common.ResourceManagement;
using Tesla.Common.OperatorConsole;
using Tesla.Common.Separator;
using Tesla.Common.Protocol;
using Tesla.InstrumentControl;

namespace GUI_Console
{
    public partial class RoboSep_RunProgress : BasePannel
    {
        private static RoboSep_RunProgress myRunProgress;
        private QuadrantProgress[] myQuadrantProgress;
        private SeparatorGateway mySeparatorGateway;
        private PauseState PauseButtonState;


        // progress tracking variables
        //private int CurrentProtocolID = -2;
        //private int CurrentProtocolQuadrant = -2;
        // private int currentProtocolSequence = -2;
        private int UpdateCounter = 0;

        private bool EnableProgressBarDetailsView = true;
        private GUI_Controls.Button_Quadrant[] QuadrantsButtons = new GUI_Controls.Button_Quadrant[4];
 
        public bool bRun_Initializing = true;


        // Language file
        private IniFile LanguageINI = GUI_Console.RoboSep_UserConsole.getInstance().LanguageINI;

        private RoboMessagePanel WaitingMSG;
        private int[] iQuadrantProxy;
        private int[] QuadrantIDs;
        private int[] IDSteps;
        private bool isSharing = false;
        private int[] CWJ_QuadrantIDs;
        private int[] CWJ_IDQuadrants;
        private int[] CWJ_IDSteps;
        private int CWJ_CurrentID;
        private int CWJ_CurrentSeq;
        private int CWJ_CurrentQdr;
        private string[,] CWJ_CommandList;

        // flag to disable buttons while resuming after pause event
        private bool disableProgressButtons = false;

        // flag to indicate that run samples successfully
        private bool runCompletedSuccessfully = false;

        private bool showResourcesScreenAfterRun = false;
        private bool maintenaceProtocol = false;

        private List<Image> iListPause;
        private List<Image> iListUnload;
        private List<Image> iListLock;
        private List<Image> iListUnlock;

        private RoboSep_RunProgress()
        {
            InitializeComponent();
 
            //
            // load proper graphics for buttons
            //
            //Unload
            iListUnload = new List<Image>();
            iListUnload.Add(Properties.Resources.unload_eject_green);
            iListUnload.Add(Properties.Resources.unload_eject_OVER);
            iListUnload.Add(Properties.Resources.unload_eject_OVER);
            iListUnload.Add(Properties.Resources.unload_eject_CLICK);

            // Lock
            iListLock = new List<Image>();
            iListLock.Add(Properties.Resources.lock_STD);
            iListLock.Add(Properties.Resources.lock_OVER);
            iListLock.Add(Properties.Resources.lock_OVER);
            iListLock.Add(Properties.Resources.lock_CLICK);

            // Unlock
            iListUnlock = new List<Image>();
            iListUnlock.Add(Properties.Resources.unlock_STD);
            iListUnlock.Add(Properties.Resources.unlock_OVER);
            iListUnlock.Add(Properties.Resources.unlock_OVER);
            iListUnlock.Add(Properties.Resources.unlock_CLICK);
            // UpdateRemoteDesktopButtons(false);

            // Pause
            iListPause = new List<Image>();
            iListPause.Add(Properties.Resources.RN_BTN01L_pause_STD);
            iListPause.Add(Properties.Resources.RN_BTN01L_pause_OVER);
            iListPause.Add(Properties.Resources.RN_BTN01L_pause_OVER);
            iListPause.Add(Properties.Resources.RN_BTN01L_pause_CLICK);
            button_Pause.ChangeGraphics(iListPause);
            button_Pause.disableImage = Properties.Resources.RN_BTN01L_pause_DISABLE;

            List<Image> ilist = new List<Image>();
            // abort button
            ilist.Add(Properties.Resources.RN_BTN02L_abort_104x86_STD);
            ilist.Add(Properties.Resources.RN_BTN02L_abort_104x86_OVER);
            ilist.Add(Properties.Resources.RN_BTN02L_abort_104x86_OVER);
            ilist.Add(Properties.Resources.RN_BTN02L_abort_104x86_CLICK);
            button_Abort.ChangeGraphics(ilist);
            button_Abort.disableImage = Properties.Resources.RN_BTN02L_abort_104x86_DISABLE;
       
            // Create QuadrantProgress objects for each quadrant
            // objects used to control sub objects used to display progress
            myQuadrantProgress = new QuadrantProgress[5] {
                new QuadrantProgress(Q1progress, labelQ1_current, lblQ1Current, labelQ1_previous, lblQ1previous, lblQ1Complete, /*panelProgressq1,*/ labelQ1_protocolName),
                new QuadrantProgress(Q2progress, labelQ2_current, lblQ2Current, labelQ2_previous, lblQ2previous, lblQ2Complete, /*panelProgressq2,*/ labelQ2_protocolName),
                new QuadrantProgress(Q3progress, labelQ3_current, lblQ3Current, labelQ3_previous, lblQ3previous, lblQ3Complete, /*panelProgressq3,*/ labelQ3_protocolName),
                new QuadrantProgress(Q4progress, labelQ4_current, lblQ4Current, labelQ4_previous, lblQ4previous, lblQ4Complete, /*panelProgressq4,*/ labelQ4_protocolName),
                new QuadrantProgress(ALLprogress, labelElapsed, elapsed, labelEstimated, estimated, ALLComplete, /*panelALL,*/ null)};

            // set button and label text based on language setting
            // button_Pause.Text = LanguageINI.GetString("lblPause");
            // button_Abort.Text = LanguageINI.GetString("lblAbort");

            // initialize Toatal Run progress bar
            myQuadrantProgress[4].isActive = true;
            int[] bars = new int[] { 500000 };
            int[] colours = new int[] { 12 };
            myQuadrantProgress[4].myProgressBar.setElements(bars, colours);
            myQuadrantProgress[4].myProgressBar.showTime = false;

            // create array of quadrant buttons
            QuadrantsButtons[0] = Q1;
            QuadrantsButtons[1] = Q2;
            QuadrantsButtons[2] = Q3;
            QuadrantsButtons[3] = Q4;

            // Pause button changes states, initialize to "pause" state
            PauseButtonState = PauseState.Pause;            

            // add delegate for batch time estimate
            mySeparatorGateway = SeparatorGateway.GetInstance();

            iQuadrantProxy      = new int[4] { 0, 1, 2, 3 };
            QuadrantIDs         = new int[4] { -1, -1, -1, -1 };
            IDSteps             = new int[4] { -1, -1, -1, -1 };

            CWJ_QuadrantIDs     = new int[4] {-1, -1, -1, -1};
            CWJ_IDQuadrants     = new int[4] { -1, -1, -1, -1 };
            CWJ_IDSteps         = new int[4] { -1, -1, -1, -1 };

            mySeparator = SeparatorGateway.GetInstance().ControlApi;
        }

        private void RoboSep_RunProgress_Load(object sender, EventArgs e)
        {
            Q1.Check = true;
            Q2.Check = true;
            Q3.Check = true;
            Q4.Check = true;

            // disable the abort button initially
            button_Abort.disable(true);
        }

        // Creates singleton of RunProgress window
        public static RoboSep_RunProgress getInstance()
        {
            if (myRunProgress == null)
            {
                myRunProgress = new RoboSep_RunProgress();
            }
            GC.Collect();
            return myRunProgress;
        }

        public enum PauseState
        {
            Pause,
            Resume,
            Unload
        }

        public bool Initializing
        {
            get
            {
                return bRun_Initializing;
            }
            set
            {
                bRun_Initializing = value;
                btn_home.Enabled = !value;
                btn_home.Visible = btn_home.Enabled? true : false;

                this.SuspendLayout();
                button_Pause.disable(value);  
                button_Abort.disable(value);
                this.ResumeLayout();
            }
        }

        public bool Sharing
        {
            set
            {
                isSharing = value;
            }
            get
            {
                return isSharing;
            }
        }

        public int CurrentQdr
        {
            get
            {
                return CWJ_CurrentQdr; // -1:Init,Prime ; 0 - 3 : Qdr1,2,3,4
            }
        }

        public void pollProgressUpdate(int id, int step, int quad, string cmd)
        {
            bool isUpdate = false;

            if ( !isUpdate )
            {
                // update counter used to check if new update is available
                UpdateCounter++;

                // id and step < 0 means the protocol being run is
                // a maintenance protocol.  Use different update
                // method.
                bool IsSeparationProtocol = id > -1 && step > -1;

                if (IsSeparationProtocol)
                {
                    // Apr 10, 2013 - Sunny comments out
                    //if (Check1DArrayIndex<int>(IDSteps, id))
                        IDSteps[id] = step;

                    UpdateProgress(id, step, quad, cmd);

                    UpdateQuadrantButtons();
                    // LOG
                    string logMSG = "Progress Update: Q = " + quad.ToString() + " Step = " + step.ToString() + " ID = " + id.ToString() + " CMD = " + cmd;
                    //  (logMSG);
                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                
                }

                if (UpdateCounter == 0)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (i != quad && myQuadrantProgress[i].isActive &&
                            myQuadrantProgress[i].myBriefCurrentStep == "Initializing")
                        {
                            myQuadrantProgress[i].myBriefCurrentStep = "Waiting";
                            myQuadrantProgress[i].myDetailsCurrentStep = "Waiting";
                            myQuadrantProgress[i].Refresh();
                        }
                    }
                }
            }
        }

        private void InitializeQuadrantProgress()
        {
            // Reset the quadrant buttons to active
            for (int i = 0; i < 4; i++)
            {
                // enable the button
                QuadrantsButtons[i].Check = false;
                QuadrantsButtons[i].disable(false);
                QuadrantsButtons[i].Check = true;
            }

            // Reset the progress bar 
            for (int i = 0; i < myQuadrantProgress.Length; i++)
            {
                myQuadrantProgress[i].myProgressBar.ResetTimer();
            }

            // update myQuadrantProgress
            int nExtraQuadrant = 0;
            for (int i = 0; i < 4; i++)
            {
                if (iQuadrantProxy == null || iQuadrantProxy.Length < i)
                    continue;

                int Q = iQuadrantProxy[i];
                if (Q >= myQuadrantProgress.Length)
                    continue;

                if (nExtraQuadrant > 0)
                {
                    nExtraQuadrant--;
                    continue;
                }

                if (!myQuadrantProgress[i].isActive)
                {
                    // Hide the quadrant if not active
                    myQuadrantProgress[i].HideAll();

                    // make sure the disable image got updated
                    QuadrantsButtons[i].Enabled = true;

                    // diable the button
                    QuadrantsButtons[i].disable(true);
                }
                else
                {
                    for (int j = i + 1; j < iQuadrantProxy.Length; j++)
                    {
                        if (iQuadrantProxy[j] == Q)
                            nExtraQuadrant++;
                    }
                    if (nExtraQuadrant > 0)
                    {
                        int nIndex = Q + nExtraQuadrant;
                        if (nIndex < myQuadrantProgress.Length)
                        {
                            // Hide the initial protocol label
                            myQuadrantProgress[Q].SetProtocolLabelVisibility(false);

                            // Hide the rest of the quadrant progress
                            for (int k = Q + 1; k <= nIndex; k++)
                            {
                                myQuadrantProgress[k].HideAll();
                            }

                            Rectangle rc1 = myQuadrantProgress[nIndex].myProgressBar.Bounds;
                            Rectangle rc2 = myQuadrantProgress[Q].myProgressBar.Bounds;
                            int nHeight = rc1.Y + rc1.Height - rc2.Y;
                            myQuadrantProgress[Q].myProgressBar.SetBounds(rc2.X, rc2.Y, rc2.Width, nHeight);

                            // set the protocol name to the last quadrant being used
                            myQuadrantProgress[nIndex].SetProtocolLabelVisibility(true);
                            myQuadrantProgress[nIndex].ProtocolName = myQuadrantProgress[Q].ProtocolName;
                        }
                    }
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
				case "Mix Transport":
                    return QuadrantProgress.ProtocolCommand.MixTransCommand;
                case "TopUp Mix Transport":
                    return QuadrantProgress.ProtocolCommand.TopUpMixTransCommand;
                case "Resuspend Mix Separate Transport":
                    return QuadrantProgress.ProtocolCommand.ResusMixSepTransCommand;
                case "Resuspend Mix":
                    return QuadrantProgress.ProtocolCommand.ResusMixCommand;
                case "TopUp Transport":
                    return QuadrantProgress.ProtocolCommand.TopUpTransCommand;
                case "TopUp Transport Separate Transport":
                    return QuadrantProgress.ProtocolCommand.TopUpTransSepTransCommand;
                case "TopUp Mix Transport Separate Transport":
                    return QuadrantProgress.ProtocolCommand.TopUpMixTransSepTransCommand;
            }
            return QuadrantProgress.ProtocolCommand.NULL;
        }

        public static void RunInfo(string pName, int Q, int[] stepTimes, int[] steps, string[] processDescriptions, int numQuadrants)
        {
            RoboSep_RunProgress runProgress = RoboSep_RunProgress.getInstance();

            if (runProgress == null || runProgress.myQuadrantProgress == null || Q < 0 || runProgress.myQuadrantProgress.Length <= Q)
                return;

            // display quadrant progress
            runProgress.myQuadrantProgress[Q].Clear();
            runProgress.myQuadrantProgress[Q].isActive = true;
            runProgress.myQuadrantProgress[Q].myProgressBar.setElements(stepTimes, steps);
            runProgress.myQuadrantProgress[Q].Refresh();
            runProgress.myQuadrantProgress[Q].ProtocolName = pName;

            if (processDescriptions != null)
            {
                RoboSep_RunProgress.getInstance().myQuadrantProgress[Q].ProcessDescriptions = new string[processDescriptions.Length];
                Array.Copy(processDescriptions, RoboSep_RunProgress.getInstance().myQuadrantProgress[Q].ProcessDescriptions, processDescriptions.Length);
            }

            for (int i = 0; i < numQuadrants && Q < 4; i++)
            {
                // check range
                if (Check1DArrayIndex<int>(RoboSep_RunProgress.getInstance().iQuadrantProxy, Q + i))
                    RoboSep_RunProgress.getInstance().iQuadrantProxy[Q + i] = Q;
            }
        }

        public void UpdateProgressBars()
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
            myQuadrantProgress[4].myBriefCurrentStep = myQuadrantProgress[4].myProgressBar.elapse;
            myQuadrantProgress[4].myDetailsCurrentStep = myQuadrantProgress[4].myProgressBar.elapse;
            myQuadrantProgress[4].Refresh();

           
            // check if batch run is complete
            if (mySeparatorGateway.SeparatorState == SeparatorState.BatchComplete 
                && PauseButtonState != PauseState.Unload)
            {
                if (RoboSep_UserConsole.ctrlCurrentUserControl == this)
                {
                    btn_home.Enabled = false;
                    btn_home.Visible = false;
                }

                // turn off timer
                updateTimer.Stop(); 
                CWJ_Timer.Stop();

                // set PauseButtonState to "Unload"
                PauseButtonState = PauseState.Unload;
                button_Pause.ChangeGraphics(iListUnload);

                // clear flag
                disableButtons = false;

                // set all protocols to 100% complete
                for (int i = 0; i < 5; i++)
                {
                    myQuadrantProgress[i].Completed();
                }

                // hide Abort button
                button_Abort.Visible = false;

                // set flag to indicate that run completed successfully
                runCompletedSuccessfully = true;

                // set flag to indicate whether to show resourcces screen
                showResourcesScreenAfterRun = RoboSep_Resources.getInstance().ShouldShowResourcesAfterRunningSamples();

                // generate the report
                GenerateEndOfRunReport();

                // prompt user that run is complete
                string completionTime = string.Format("{0:HH:mm}", DateTime.Now);
                string sMSG = LanguageINI.GetString("msgRunComplete1");
                sMSG += " " + completionTime + "\r\n";

                string sMSG2 = LanguageINI.GetString("msgRunComplete2");
                sMSG += sMSG2;

                RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_INFORMATION,
                    sMSG, LanguageINI.GetString("headerRunComplete"), LanguageINI.GetString("Ok"));
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                prompt.Dispose();
                RoboSep_UserConsole.hideOverlay();
            }
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            if (!Initializing)
            {
                // TEST GUI PAUSE FAILURE
                // look for update
                UpdateProgressBars();

                // test if runs are completed
               // bool allCompleted = true;
                for (int i = 0; i < 4; i++)
                {
                    if (myQuadrantProgress[i].myProgressBar.progress == 100
                        && !myQuadrantProgress[i].myLabelCompleted.Visible)
                    {
                        myQuadrantProgress[i].Completed();

                        // Reset the quadrant button color
                        if (i < QuadrantsButtons.Length)
                            QuadrantsButtons[i].NowInProgress = false;
                    }
                }
            }
        }

        public void Start(bool bMaintenaceProtocol)
        {
            maintenaceProtocol = bMaintenaceProtocol;

            // run protocols,  run progress window should be set up through
            // run samples window before coming to resources window.
            // do separatorgateway.startrun at this point
            RoboSep_RunSamples.getInstance().startSeparationRun();

            // UpdateRemoteDesktopButtons(false);

            // set protocol IDs            
            int count = 0;

            if (bMaintenaceProtocol == true)
            {
                //System.Diagnostics.Trace.WriteLine(String.Format("++++++++++++++++++++++++++++++++++"));
                //System.Diagnostics.Trace.WriteLine(String.Format("-->> Maintenace Protocol selected!"));
            }
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

            IDSteps[0] = -1;
            IDSteps[1] = -1;
            IDSteps[2] = -1;
            IDSteps[3] = -1;

            CWJ_IDSteps[0] = -1;
            CWJ_IDSteps[1] = -1;
            CWJ_IDSteps[2] = -1;
            CWJ_IDSteps[3] = -1;

            CWJ_CurrentID  = -1;
            CWJ_CurrentSeq = -1;
            CWJ_CurrentQdr = -1;

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
                        
                        break;
                    }                
                }                
                if (k > 3)
                        break;
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
                else
                {
                    // diable the button
                    QuadrantsButtons[i].disable(true);
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

            EnableProgressBarDetailsView = RoboSep_UserDB.getInstance().EnableProgressBarDetailsView;

            button_Abort.disable(true);
            button_Pause.disable(true);

            showProgressDetails(EnableProgressBarDetailsView);

            Initializing = true;

            // reset flags
            runCompletedSuccessfully = false;
            showResourcesScreenAfterRun = false;

            InitializeQuadrantProgress();

            updateTimer.Start();
            CWJ_Timer.Start();
            labelEstimated.Text = RoboSep_UserConsole.getInstance().EstimatedCompletion;
            myQuadrantProgress[4].myBriefPreviousStep = RoboSep_UserConsole.getInstance().EstimatedCompletion;
            myQuadrantProgress[4].myDetailsPreviousStep = RoboSep_UserConsole.getInstance().EstimatedCompletion;
            myQuadrantProgress[4].Refresh();

            // LOG
            string logMSG = " **INITIALIZING RUN";
            //UI_Controls.uiLog.LOG(this, "Start", uiLog.LogLevel.EVENTS, sTemp);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                

            for (int i = 0; i < 4; i++)
            {
                count = 0;
                if (myQuadrantProgress[i].isActive)
                {
                    myQuadrantProgress[i].quadrantID = count;
                    myQuadrantProgress[i].myBriefCurrentStep = "Initializing";
                    myQuadrantProgress[i].myDetailsCurrentStep = "Initializing";
                    myQuadrantProgress[i].myProgressBar.StartTimer();
                    myQuadrantProgress[i].Refresh();
                    count++;
                }
                QuadrantsButtons[i].NowInProgress = false;
            }
           // CurrentProtocolQuadrant = 0;
           // CurrentProtocolID = 0;
            UpdateCounter = 0;
        }

        private void button_Abort_Click(object sender, EventArgs e)
        {
            if (GetRemainingSteps() > 0)
            {
                // abort can be disabled when system is recovering from pause state
                // system will be homing at this time.
                if (disableProgressButtons)
                {
                    string sMSG = LanguageINI.GetString("lblAbortDisable");
                    RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING, sMSG,
                        LanguageINI.GetString("headerRecovering"), LanguageINI.GetString("Ok"));
                    RoboSep_UserConsole.showOverlay();
                    prompt.ShowDialog();
                    prompt.Dispose();
                    RoboSep_UserConsole.hideOverlay();
                }
                else
                {
                    string sMSG = LanguageINI.GetString("msgHalt");

                    RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_QUESTION, 
                        sMSG, LanguageINI.GetString("headerAbort"), LanguageINI.GetString("Yes"), LanguageINI.GetString("Cancel"));
                    RoboSep_UserConsole.showOverlay();
                    prompt.ShowDialog();
                    RoboSep_UserConsole.hideOverlay();
                    if (prompt.DialogResult == DialogResult.OK)
                    {
                        // abor trun
                        Abbort();
                    }
                    prompt.Dispose();                    
                }
            }
            else
            {
                string sMSG = LanguageINI.GetString("msgPauseLastStep");
                GUI_Controls.RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_INFORMATION, 
                    sMSG, LanguageINI.GetString("Pause"), LanguageINI.GetString("Ok"));
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                prompt.Dispose();
                RoboSep_UserConsole.hideOverlay();
            }
        }

        public void Abbort()
        {
            System.Diagnostics.Debug.WriteLine(String.Format("------------------Abbort is called"));
            //  (String.Format("Abbort is called"));            
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "Abbort is called");                
            updateTimer.Stop();
            CWJ_Timer.Stop();
            //HaltTimer.Start();
            halted = false;

            string sMSG = LanguageINI.GetString("msgHalting");

            // Clean up
            if (WaitingMSG != null)
            {
                WaitingMSG.Close();
                WaitingMSG = null;
            }

            WaitingMSG = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_INFORMATION, sMSG, true, false);
            RoboSep_UserConsole.showOverlay();
            WaitingMSG.Show();//ShowDialog();

            // Tell the Separator to halt the batch run
            mySeparatorGateway.ControlApi.HaltRun();

            System.Diagnostics.Debug.WriteLine(String.Format("------------------Run Samples unloaded."));
            //  (String.Format("Run Samples unloaded.(Abort)"));            
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, String.Format("Run Samples unloaded.(Abort)"));                
            // move to run samples window
            RoboSep_RunSamples.getInstance().Unload();
        }

        public void Unload(UserControl RoboSepWindow)
        {
            // hide all quadrants, will be unhidden
            // when they are re-added
            //  (String.Format("Unload is called."));            
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, String.Format("Unload is called."));                            
            updateTimer.Stop();
            CWJ_Timer.Stop();

            // use robosep server communication
            // to unload protocols from db
            if (RoboSepWindow == this)
            {
                mySeparatorGateway.ControlApi.ParkArm();
                mySeparatorGateway.ControlApi.Unload();

                // Clean up
                if (WaitingMSG != null)
                {
                    WaitingMSG.Close();
                    WaitingMSG = null;
                }

                // open message dialog that makes user wait while system unloads
                string sMSG = LanguageINI.GetString("msgUnloading");
                WaitingMSG = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WAIT, sMSG, true, false);
                if (!RoboSep_UserConsole.getInstance().frmOverlay.Visible)
                    RoboSep_UserConsole.showOverlay();
                WaitingMSG.Show();
                //  (String.Format("Unload message dialog is up!"));            
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, String.Format("Unload message dialog is up!"));                            
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    myQuadrantProgress[i].myProgressBar.Stop();
                    myQuadrantProgress[i].Clear();
                }

                // reset Total Progress
                int[] bars = new int[] { 500000 };
                int[] colours = new int[] { 0 };
                RoboSep_RunProgress.RunInfo("", 4, bars, colours, new string[] { "Total Progress" }, 1);
            }

            // reset quadrant button state
            for (int i = 0; i < 4; i++)
            {
                QuadrantsButtons[i].NowInProgress = false;
            }

            iQuadrantProxy = new int[] { 0, 1, 2, 3 };
            QuadrantIDs = new int[4] { -1, -1, -1, -1 };
            IDSteps = new int[4] { -1, -1, -1, -1 };

            UpdateCounter = 0;

            btn_home.Enabled = true;
            btn_home.Visible = true;

            isSharing = false;
        }

        public void HaltCompleted()
        {
            // log message
            string sMsg;            
            if (RoboSep_UserConsole.bIsBatchHaltDone == true)
            {
                sMsg = "HaltCompleted is called";
                RoboSep_UserConsole.bIsBatchHaltDone = false;
                //  (sMsg);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, sMsg);                            
            }
            else
            {
                sMsg = "HaltCompleted is already processing";
                //  (sMsg);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, sMsg);                            
                return; //CWJ Batch Halt 
            }

            halted = true;
            RoboSep_RunSamples.getInstance().disableRun = true;

            // Clean up
            if (WaitingMSG != null)
            {
                sMsg = "Close waiting message";
                //  (sMsg);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, sMsg);                            
                WaitingMSG.Close();
                WaitingMSG = null;
            }
            RoboSep_RunSamples.getInstance().disableRun = false;
            
            Unload(this);   //CWJ Batch Halt improve
        }

        public void PauseGuiElements()
        {
            //WaitingMSG.closeDialogue(DialogResult.OK);
            //WaitingMSG.Close();
            //RoboSep_UserConsole.hideOverlay();
            if (CWJ_Timer.Enabled || updateTimer.Enabled)
            {
                CWJ_Timer.Stop();
                updateTimer.Stop();
            }

            paused = true;

            // stop timers and progress bars
            for (int i = 0; i < 4; i++)
            {
                if (myQuadrantProgress[i].isActive)
                {
                    myQuadrantProgress[i].myProgressBar.Pause(false);
                }
                //button_Pause.Text = LanguageINI.GetString("lblResume");
                //button_Pause.Refresh();
            }
        }

        public void UnloadCompleted()
        {
            System.Diagnostics.Debug.WriteLine(String.Format("UnloadCompleted called. Current ThreadID = {0}", Thread.CurrentThread.ManagedThreadId));

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, String.Format("UnloadCompleted called. Current ThreadID = {0}", Thread.CurrentThread.ManagedThreadId));                            
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate() { this.UnloadCompleted(); });
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    myQuadrantProgress[i].myProgressBar.Stop();
                    myQuadrantProgress[i].Clear();
                }

                // reset button state
                for (int i = 0; i < 4; i++)
                {
                    QuadrantsButtons[i].NowInProgress = false;
                }

                // reset Total Progress
                int[] bars = new int[] { 500000 };
                int[] colours = new int[] { 0 };
                RoboSep_RunProgress.RunInfo("", 4, bars, colours, new string[] { "Total Progress" }, 1);

                //System.Threading.Thread.Sleep(5000);              //CWJ Batch Halt improve

                RoboSep_UserConsole.getInstance().StopBeepTimer();

                try
                {
                    // Delete video log file
                    Utilities.DeleteVideoLogFiles(RoboSep_UserConsole.strCurrentUser, RoboSep_UserDB.getInstance().DeleteVideoLogFiles);
                }
                catch (Exception ex)
                {
                    // log exception message

                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, ex.Message);                            
                }

                // Clean up
                if (WaitingMSG != null)
                {
                    WaitingMSG.Close();
                    WaitingMSG = null;

                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "WaitngMsg disappear - UnloadCompleted() ");                            
                }

                RoboSep_UserConsole.hideOverlay();
                RoboSep_RunSamples.getInstance().disableRun = false;

                // update server..
                leaveRunProgress();
            }
        }

        public void Resume()
        {

        }

        public void ResumeGuiElements()
        {
            // Clean up
            if (WaitingMSG != null)
            {
                WaitingMSG.Close();
                WaitingMSG = null;
            }

            RoboSep_UserConsole.hideOverlay();

            for (int i = 0; i < 4; i++)
            {
                if (myQuadrantProgress[i].isActive && myQuadrantProgress[i].myProgressBar.progress > 0)
                {
                    myQuadrantProgress[i].myProgressBar.Start();
                }
            }

            UpdateQuadrantButtons();

            disableButtons = false;
            updateTimer.Start();
            CWJ_Timer.Start();
        }

        private void UpdateQuadrantButtons()
        {
            int nQuadrantNowInProgress = CWJ_CurrentQdr;

            for (int i = 0; i < QuadrantsButtons.Length; i++)
            {
                QuadrantsButtons[i].NowInProgress = false;
            }

            // check range
            if (nQuadrantNowInProgress < 0 || QuadrantsButtons.Length <= nQuadrantNowInProgress)
                return;

            for (int i = 0; i < iQuadrantProxy.Length; i++)
            {
                if (QuadrantsButtons.Length <= i)
                    continue;

                // Highlight the quadrant numbers 
                if (iQuadrantProxy[i] == nQuadrantNowInProgress)
                {
                    QuadrantsButtons[i].NowInProgress = true;
                }
            }

            // button_RemoteDeskTop.Visible = true;
        }

        public bool disableButtons
        {
            set
            {
                disableProgressButtons = value;
            }
        }

        public bool halted = false;
        public bool paused = false;
        public bool unloaded = false;
        public bool resuming = false;

        private void PauseTimer_Tick(object sender, EventArgs e)
        {
            SeparatorState CurrentSepState = mySeparatorGateway.SeparatorState;
            if (CurrentSepState == SeparatorState.Paused && !paused)
            {
                // Clean up
                if (WaitingMSG != null)
                {
                    WaitingMSG.closeDialogue(DialogResult.OK);
                    WaitingMSG = null;
                }

                RoboSep_UserConsole.hideOverlay();
                PauseTimer.Stop();

                paused = true;

                // stop timers and progress bars
                for (int i = 0; i < 4; i++)
                {
                    if (myQuadrantProgress[i].isActive)
                    {
                        myQuadrantProgress[i].myProgressBar.Pause(true);
                    }
                    //button_Pause.Text = PauseButtonState;
                    //button_Pause.Refresh();
                }
            }            
        }

        private void HaltTimer_Tick(object sender, EventArgs e)
        {
            SeparatorState CurrentSepState = mySeparatorGateway.SeparatorState;
            if (CurrentSepState == SeparatorState.BatchHalted && !halted)
            {
                // reset everything to defaults
                // update server..
                halted = true;

                // Clean up
                if (WaitingMSG != null)
                {
                    WaitingMSG.closeDialogue(DialogResult.OK);
                    WaitingMSG.Close();
                    WaitingMSG = null;
                }

                System.Diagnostics.Debug.WriteLine(String.Format("------------------HaltTimer_Tick : RoboSep_RunSamples.getInstance().disableRun = false"));

                RoboSep_RunSamples.getInstance().disableRun = false;
                //RoboSep_UserConsole.hideOverlay();
                Unload(this);
                //leaveRunProgress();
            }
        }
#if false
        private void leaveRunProgress()
        {
            System.Diagnostics.Debug.WriteLine(String.Format("------------------leaveRunProgress is called."));

            // turn pause timer on
            //UnloadMinTimer.Start();
            updateTimer.Stop();
            CWJ_Timer.Stop();


            if (!runCompletedSuccessfully)    // probably from abort
            {
                System.Diagnostics.Debug.WriteLine(String.Format("------------------generate End Of Run Report (probably caused by 'Abort')."));
//                GenerateEndOfRunReport();
            }

            // move to run samples window
            RoboSep_RunSamples.getInstance().Unload();

            if (RoboSep_UserDB.getInstance().LoadMRUProtocolsAtStartUp)
            {
                if (RoboSep_RunSamples.getInstance().IsInitialized)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("------------------reload most recently used protocols."));
                    RoboSep_RunSamples.getInstance().ReInitialize();
                }
            }
            if (RoboSep_UserConsole.bIsRunning)
            {
                // tell robosep it is no longer in run mode
                RoboSep_UserConsole.bIsRunning = false;

                // switch to resources or run sampling window
                if (runCompletedSuccessfully && showResourcesScreenAfterRun)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("------------------switch to Resources screen to highlight end of run items."));

                    RoboSep_Resources ResourceWindow = RoboSep_Resources.getInstance();
                    if (ResourceWindow != null)
                    {
                        // set status that unload carousel completed and show resources screen
                        ResourceWindow.UnloadCarouselCompleted(true);

                        // show resources screen
                        ResourceWindow.Location = new Point(0, 0);
                        RoboSep_UserConsole.getInstance().Controls.Remove(RoboSep_UserConsole.ctrlCurrentUserControl);
                        RoboSep_UserConsole.ctrlCurrentUserControl = ResourceWindow;
                        RoboSep_UserConsole.getInstance().Controls.Add(ResourceWindow);
                        ResourceWindow.Visible = true;
                        ResourceWindow.BringToFront();
                        System.Diagnostics.Debug.WriteLine(String.Format("------------------show Resources screen."));
                    }
                }
                else
                {
                    // switch to RunSamples page
                    RoboSep_RunSamples.getInstance().Visible = false;
                    RoboSep_RunSamples.getInstance().Location = new Point(0, 0);
                    RoboSep_UserConsole.getInstance().Controls.Remove(RoboSep_UserConsole.ctrlCurrentUserControl);
                    RoboSep_UserConsole.getInstance().Controls.Add(RoboSep_RunSamples.getInstance());
                    RoboSep_UserConsole.ctrlCurrentUserControl = RoboSep_RunSamples.getInstance();
                    RoboSep_RunSamples.getInstance().Visible = true;
                    System.Diagnostics.Debug.WriteLine(String.Format("------------------show Run Samples screen."));
                }
            }
                
            System.Diagnostics.Debug.WriteLine(String.Format("------------------clear progress bars"));
 
            // clear run progress
            RoboSep_UserConsole.getInstance().ClearProgress();


            // update the button states in the Settings screen
            RoboSep_Home.getInstance().UpdateButtons();

            // reset flags in case they haven't been reset
            runCompletedSuccessfully = false;
            showResourcesScreenAfterRun = false;

            // re- set progress window
            PauseButtonState = PauseState.Pause;
            button_Abort.Visible = true;
            button_Pause.ChangeGraphics(iListPause);

            System.Diagnostics.Debug.WriteLine(String.Format("------------------leaveRunProgress returns."));
        }
#else
        private void leaveRunProgress()
        {
            System.Diagnostics.Debug.WriteLine(String.Format("------------------leaveRunProgress is called."));

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, String.Format("leaveRunProgress is called."));                            
            // turn pause timer on
            //UnloadMinTimer.Start();
            updateTimer.Stop();
            CWJ_Timer.Stop();

            // move to run samples window
            RoboSep_RunSamples.getInstance().Unload();
            System.Threading.Thread.Sleep(2000);        //CWJ BatchHalt improve add

            if (!runCompletedSuccessfully)    // probably from abort
            {
                System.Diagnostics.Debug.WriteLine(String.Format("------------------generate End Of Run Report (probably caused by 'Abort')."));

                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, String.Format("generate End Of Run Report (probably caused by 'Abort')."));                                            
                GenerateEndOfRunReport();
            }

            if (RoboSep_UserDB.getInstance().LoadMRUProtocolsAtStartUp)
            {
                if (RoboSep_RunSamples.getInstance().IsInitialized)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("------------------reload most recently used protocols."));

                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, String.Format("reload most recently used protocols."));                                            
                    RoboSep_RunSamples.getInstance().ReInitialize();
                }
            }
            if (RoboSep_UserConsole.bIsRunning)
            {
                // switch to resources or run sampling window
                if (runCompletedSuccessfully && showResourcesScreenAfterRun)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("------------------switch to Resources screen to highlight end of run items."));

                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, String.Format("switch to Resources screen to highlight end of run items."));                                                                
                    RoboSep_Resources ResourceWindow = RoboSep_Resources.getInstance();
                    if (ResourceWindow != null)
                    {
                        // set status that unload carousel completed and show resources screen
                        ResourceWindow.UnloadCarouselCompleted(true);

                        // show resources screen
                        ResourceWindow.Location = new Point(0, 0);
                        RoboSep_UserConsole.getInstance().Controls.Remove(RoboSep_UserConsole.ctrlCurrentUserControl);
                        RoboSep_UserConsole.ctrlCurrentUserControl = ResourceWindow;
                        RoboSep_UserConsole.getInstance().Controls.Add(ResourceWindow);
                        ResourceWindow.Visible = true;
                        ResourceWindow.BringToFront();
                        System.Diagnostics.Debug.WriteLine(String.Format("------------------show Resources screen."));

                        LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, String.Format("show Resources screen."));                                                                
                    }
                }
                else
                {
                    // switch to RunSamples page
                    RoboSep_RunSamples.getInstance().Visible = false;
                    RoboSep_RunSamples.getInstance().Location = new Point(0, 0);
                    RoboSep_UserConsole.getInstance().Controls.Remove(RoboSep_UserConsole.ctrlCurrentUserControl);
                    RoboSep_UserConsole.getInstance().Controls.Add(RoboSep_RunSamples.getInstance());
                    RoboSep_UserConsole.ctrlCurrentUserControl = RoboSep_RunSamples.getInstance();
                    RoboSep_RunSamples.getInstance().Visible = true;
                    System.Diagnostics.Debug.WriteLine(String.Format("------------------show Run Samples screen."));

                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, String.Format("show Run Samples screen."));                                                                
                }
                // tell robosep it is no longer in run mode
                RoboSep_UserConsole.bIsRunning = false;
            }

            System.Diagnostics.Debug.WriteLine(String.Format("------------------clear progress bars"));

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, String.Format("clear progress bars"));                                                                            
            // clear run progress
            RoboSep_UserConsole.getInstance().ClearProgress();


            // update the button states in the Settings screen
            RoboSep_Home.getInstance().UpdateButtons();

            // reset flags in case they haven't been reset
            runCompletedSuccessfully = false;
            showResourcesScreenAfterRun = false;

            // re- set progress window
            PauseButtonState = PauseState.Pause;
            button_Abort.Visible = true;
            button_Pause.ChangeGraphics(iListPause);

            RoboSep_UserConsole.bIsBatchHaltDone = true;

            System.Diagnostics.Debug.WriteLine(String.Format("------------------leaveRunProgress returns."));

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, String.Format("leaveRunProgress returns."));                                                                            
        }
#endif
        private void GenerateEndOfRunReport()
        {
            string sFullXmlFileName = RoboSep_UserConsole.getInstance().GetEndOfRunXMLFullFileName();
            if (string.IsNullOrEmpty(sFullXmlFileName))
            {
                // LOG
                string logMSG = "Failed to get End Of Run XML file from server. End Of Run report is not generated.";
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Warning, logMSG);
                return;
            }

            if (!File.Exists(sFullXmlFileName) == true)
            {
                // LOG
                string logMSG = String.Format("XML file '{0}' does not exist. End Of Run report is not generated.", sFullXmlFileName);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Warning, logMSG);
                return;
            }

            FileInfo fInfo = new FileInfo(sFullXmlFileName);
            if (fInfo == null)
                return;

            int nIndex = fInfo.Name.LastIndexOf('.');
            if (nIndex < 0)
                return;

            string sFileName = fInfo.Name.Substring(0, nIndex);
            string sPdfFileName = sFileName + ".pdf";

            string sFullFileName = fInfo.DirectoryName;
            if (sFullFileName.LastIndexOf('\\') != sFullFileName.Length - 1)
            {
                sFullFileName += "\\";
            }
            string sFullPdfFileName = sFullFileName + sPdfFileName;

            // Generate PDF file if not created
            if (!File.Exists(sFullPdfFileName) == true)
            {
                string[] aFileNames = new string[] { sFullXmlFileName, sFullPdfFileName };

                BackgroundWorker bwGenerateReport = new BackgroundWorker();
                bwGenerateReport.WorkerSupportsCancellation = true;

                // Attach the event handlers
                bwGenerateReport.DoWork += new DoWorkEventHandler(bwGenerateReport_DoWork);
                bwGenerateReport.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwGenerateReport_RunWorkerCompleted);

                // Kick off the Async thread
                bwGenerateReport.RunWorkerAsync(aFileNames);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(String.Format("------------------EOR does exist!"));
            }
        }

        private void bwGenerateReport_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            string[] aFileNames = e.Argument as string[];
            if (aFileNames == null || aFileNames.Length == 0)
                return;

            RoboSep_UserConsole.getInstance().GenerateReport(aFileNames[0], aFileNames[1]);

            if (!e.Cancel)
            {
                // PDF file name
                e.Result = aFileNames[1];
            }
        }

        private void bwGenerateReport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Invoke(
               (MethodInvoker)delegate()
               {

               }
            );
        }


        private int GetRemainingSteps()
        {
            int stepsRemaining = 0;
            foreach (QuadrantProgress QP in myQuadrantProgress)
            {
                if (QP.isActive)
                    stepsRemaining += QP.myProgressBar.NumStepsRemaining;
            }
            return stepsRemaining;
        }

        private void button_Pause_Click(object sender, EventArgs e)
        {   
            // check if button is in Pause or Resume state
            string PauseString = LanguageINI.GetString("lblPause");
            string ResumeString = LanguageINI.GetString("lblResume");
            string UnloadString = LanguageINI.GetString("lblUnload");

            // abort can be disabled when system is recovering from pause state
            // system will be homing at this time.
            if (disableProgressButtons)
            {
                string sMSG = LanguageINI.GetString("lblPauseDisable");
                RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING, sMSG,
                    LanguageINI.GetString("headerRecovering"), LanguageINI.GetString("Ok"));
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                prompt.Dispose();                
                RoboSep_UserConsole.hideOverlay();
            }
            else // buttons are not disabled
            {
                switch (PauseButtonState)
                {
                    case PauseState.Pause:
                        // pause run
                        // promt user, do you really want to pause this run
                        if (GetRemainingSteps() < 1)
                        {
                            string sMSG = LanguageINI.GetString("msgPauseLastStep");
                            GUI_Controls.RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_INFORMATION, 
                                sMSG, LanguageINI.GetString("Pause"), LanguageINI.GetString("Ok"));
                            RoboSep_UserConsole.showOverlay();
                            prompt.ShowDialog();
                            prompt.Dispose();
                            RoboSep_UserConsole.hideOverlay();
                        }
                        else
                        {
                            string sMSG = LanguageINI.GetString("msgPause");
                            string sHeader = LanguageINI.GetString("headerPause");
                            GUI_Controls.RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_QUESTION,
                                sMSG, sHeader, LanguageINI.GetString("Yes"), LanguageINI.GetString("Cancel"));
                            RoboSep_UserConsole.showOverlay();
                            prompt.ShowDialog();
                            RoboSep_UserConsole.hideOverlay();

                            if (prompt.DialogResult == DialogResult.OK)
                            {
                                // pause run through server
                                mySeparatorGateway.ControlApi.PauseRun();

                                // Display the pause dialog in 'pausing' mode
                                RoboSep_UserConsole.getInstance().DisplayPauseResumeDialog(true, false);
                            }
                            prompt.Dispose();                            
                        }
                        break;
                    // in Resume Mode
                    case PauseState.Resume:
                        mySeparatorGateway.ControlApi.ResumeRun();
                        PauseButtonState = PauseState.Pause;
                        button_Pause.ChangeGraphics(iListPause);
                        // button_Pause.Refresh();
                        for (int i = 0; i < 4; i++)
                        {
                            if (myQuadrantProgress[i].isActive && myQuadrantProgress[i].myProgressBar.progress > 0)
                                myQuadrantProgress[i].myProgressBar.Start();
                        }

                        // Clean up
                        if (WaitingMSG != null)
                        {
                            WaitingMSG.Close();
                            WaitingMSG = null;
                        }

                        string Smsg = LanguageINI.GetString("msgResume");
                        WaitingMSG = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_INFORMATION, Smsg, true, false);
                        RoboSep_UserConsole.showOverlay();
                        WaitingMSG.Show();

                        paused = false;
                        resuming = true;
                        break;
                    // is in Unload Mode
                    case PauseState.Unload:
                        Unload(this);
                        //leaveRunProgress();
                        break;
                }
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
                // range check
                if (Check1DArrayIndex<int>(iQuadrantProxy, QUAD))
                    updateQuad = iQuadrantProxy[QUAD];

                // range check
                if (Check1DArrayIndex<int>(QuadrantIDs, ID))
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

            // range check
            if (Check1DArrayIndex<int>(QuadrantIDs, ID))
                updateQuad = QuadrantIDs[ID];

            // update information based on IDStep and QuadrantIDs

            // range check
            if (Check1DArrayIndex<QuadrantProgress>(myQuadrantProgress, updateQuad) && Check1DArrayIndex<int>(IDSteps, ID))
                myQuadrantProgress[updateQuad].UpdateStep2(IDSteps[ID], STEP);

        }

        private static bool Check1DArrayIndex<T>(T[] array, int index)
        {
            if (array == null)
                return false;

            bool bWithinRange = false;
            if (0 <= index && index < array.Length)
            {
                bWithinRange = true;
            }
            return bWithinRange;
        }

        private static bool Check2DArrayIndexes<T>(T[,] array, int index0, int index1)
        {
            if (array == null)
                return false;

            bool bWithinRange = false;
            if (0 <= index0 && index0 < array.GetLength(0) && 0 <= index1 && index1 < array.GetLength(1))
            {
                bWithinRange = true;
            }
            return bWithinRange;
        }

        private void CWJ_Timer_Tick(object sender, EventArgs e)
        {
            if (!Initializing && true)
            {
                // Nov 7, 2013 Sunny
                // Add updating on maintenance run
                if (1 <= myQuadrantProgress[0].myProgressBar.elements.Count)
                {
                    int id, seq, sec;

                    if (!maintenaceProtocol)
                    {
                        seq = mySeparator.GetCurrentSeq() - 1;
                        id = mySeparator.ConvertIdToSampleIndex(mySeparator.GetCurrentID());
                        sec = mySeparator.GetCurrentSec() - 1;
                    }
                    else
                    {
                        seq = Math.Abs(mySeparator.GetCurrentSeq()) - 1;
                        id = 0;                                 // Always the first quadrant
                        sec = mySeparator.GetCurrentSec();
                    }
                    

                    if ((id > -1) && (seq > -1) && (id < 4))
                    {
                        // range check
                        if (id < CWJ_IDSteps.Length)
                        {
                            //System.Diagnostics.Debug.WriteLine(String.Format("CWJ_Timer_Tick called. Before CWJ_IDSteps[{0}] = {1}. seq = {2}.", id, CWJ_IDSteps[id], seq));
                            CWJ_IDSteps[id] = seq;
                            //System.Diagnostics.Debug.WriteLine(String.Format("CWJ_Timer_Tick called. After CWJ_IDSteps[{0}] = {1}. ", id, CWJ_IDSteps[id]));

                        }

                        if ((CWJ_CurrentID != id) || (CWJ_CurrentSeq != seq))
                        {
                            CWJ_CurrentID  = id;
                            CWJ_CurrentSeq = seq;
                            NotifyCWJEventChange(id, seq);
                        }
                    }
                    else
                    {
                        //System.Diagnostics.Trace.WriteLine("----------------------");
                        //System.Diagnostics.Trace.WriteLine(String.Format(">> Non Process event: ID = {0}, SEQUENCE STEP = {1}", id, seq));
                    } 
                }
            }
        }

        protected override void btn_home_Click(object sender, EventArgs e)
        {
            if (disableProgressButtons)
            {
                string sMSG = LanguageINI.GetString("lblHomeDisable");
                RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING, sMSG,
                    LanguageINI.GetString("headerRecovering"), LanguageINI.GetString("Ok"));
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                prompt.Dispose();
                RoboSep_UserConsole.hideOverlay();
 
            }
            else
            {
                base.btn_home_Click(sender, e);
            }
        }

        private void showProgressDetails(bool showDetails)
        {
            for (int Q = 0; Q < 4; Q++)
            {
                if (myQuadrantProgress[Q].isActive)
                {
                    myQuadrantProgress[Q].ShowDetails = showDetails;
                }
            }
        }

        private void NotifyCWJEventChange(int id, int seq)
        {
            int         qdr;
            string      strstep, cmd;
            string[]    strQ;
            string      str = "Quadrant : ???";
            string      desc = "???";


            strQ = new string[2];

            //System.Diagnostics.Debug.WriteLine("----------------------");
            for (int ProtocolID = 0; ProtocolID < 4; ProtocolID++)
            {
                if (CWJ_QuadrantIDs.Length <= ProtocolID)
                    break;

                qdr = CWJ_QuadrantIDs[ProtocolID];
                if (0 <= qdr)
                {
                    if (0 <= id && id < CWJ_QuadrantIDs.Length)
                    {
                        if (qdr == CWJ_QuadrantIDs[id])
                        {
                            str = String.Format("*Quadrant : {0}", qdr + 1);
                        }
                        else
                        {
                            str = String.Format(" Quadrant : {0}", qdr + 1);
                        }
                    }

                    if (ProtocolID < CWJ_IDSteps.Length)
                    {
                        strstep = String.Format(" Step : {0}", CWJ_IDSteps[ProtocolID] + 1);
                        desc = "???";

                        if (CWJ_IDSteps[ProtocolID] < 0)
                        {
                            strQ[0] = "  Previous: N/A";
                            strQ[1] = "  Current:  Init";
                        }
                        else if (CWJ_IDSteps[ProtocolID] == 0)
                        {
                            // Range check
                            if (0 <= CWJ_IDSteps[ProtocolID] && Check2DArrayIndexes<string>(CWJ_CommandList, ProtocolID, CWJ_IDSteps[ProtocolID]))
                            {
                                strQ[0] = "  Previous: Init";
                                strQ[1] = "  Current: " + CWJ_CommandList[ProtocolID, CWJ_IDSteps[ProtocolID]] + strstep;

                                if (0 <= qdr && qdr < myQuadrantProgress.Length)
                                {
                                    if (myQuadrantProgress[qdr].ProcessDescriptions != null && seq <= myQuadrantProgress[qdr].ProcessDescriptions.Length - 1)
                                    {
                                        desc = String.Format("Process Description: {0}. seq = {1}.", myQuadrantProgress[qdr].ProcessDescriptions[seq], seq);
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Range check
                            if ((0 <= CWJ_IDSteps[ProtocolID] - 1) && Check2DArrayIndexes<string>(CWJ_CommandList, ProtocolID, CWJ_IDSteps[ProtocolID]))
                            {
                                strQ[0] = "  Previous: " + CWJ_CommandList[ProtocolID, CWJ_IDSteps[ProtocolID] - 1];
                                strQ[1] = "  Current: " + CWJ_CommandList[ProtocolID, CWJ_IDSteps[ProtocolID]] + strstep;

                                if (myQuadrantProgress[qdr].ProcessDescriptions != null && seq <= myQuadrantProgress[qdr].ProcessDescriptions.Length - 1)
                                {
                                    desc = String.Format("  Process Description: {0}. seq = {1}.", myQuadrantProgress[qdr].ProcessDescriptions[seq], seq);
                                }
                            }
                        }
                    }
                    //System.Diagnostics.Trace.WriteLine(str);
                    //System.Diagnostics.Trace.WriteLine(strQ[1]);
                    //System.Diagnostics.Trace.WriteLine(strQ[0]);
                    //System.Diagnostics.Trace.WriteLine(desc);
                }
            }

            // Range check
            if (Check1DArrayIndex<int>(CWJ_QuadrantIDs, id) && Check1DArrayIndex<int>(CWJ_IDSteps, id) && Check2DArrayIndexes<string>(CWJ_CommandList, id, CWJ_IDSteps[id]))
            {
                qdr = CWJ_QuadrantIDs[id];
                cmd = CWJ_CommandList[id, CWJ_IDSteps[id]];
                CWJ_CurrentQdr = qdr;

                pollProgressUpdate(id, seq, qdr, cmd);
            }
        }

        private void UpdateRemoteDesktopButtons(bool bVisible)
        {
            if (RoboSep_UserConsole.getInstance().IsRemoteDestopLock)
            {
                button_RemoteDeskTop.ChangeGraphics(iListLock);
            }
            else
            {
                button_RemoteDeskTop.ChangeGraphics(iListUnlock);
            }

            button_RemoteDeskTop.Visible = bVisible;
        }

        private void button_RemoteDeskTop_Click(object sender, EventArgs e)
        {
            if (RoboSep_UserConsole.getInstance().IsRemoteDestopLock)
            {
                // Unlock
                RoboSep_UserConsole.getInstance().BroadcastOrcaTCPServer(true);
                RoboSep_UserConsole.getInstance().IsRemoteDestopLock = false;
            }
            else
            {
                string sHeader = LanguageINI.GetString("headerRoboCon");
                string sMsg = LanguageINI.GetString("RoboConDisconnected");
                string sButtonText = LanguageINI.GetString("Ok");

                GUI_Controls.RoboMessagePanel prompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING, 
                    sMsg, sHeader, sButtonText);
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                prompt.Dispose();                
                RoboSep_UserConsole.hideOverlay();

                // Lock
                RoboSep_UserConsole.getInstance().BroadcastOrcaTCPServer(false);
                RoboSep_UserConsole.getInstance().IsRemoteDestopLock = true;
            }

            UpdateRemoteDesktopButtons(true);
        }
    }
}
