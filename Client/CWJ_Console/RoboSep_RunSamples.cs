using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Xml;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using GUI_Controls;
using Tesla.Common;
using Tesla.Common.Protocol;
using Tesla.OperatorConsoleControls;
using Tesla.Common.Separator;
using Tesla.Common.ResourceManagement;
using Invetech.ApplicationLog;
using Tesla.Common.ProtocolCommand;
using System.Collections;


namespace GUI_Console
{
    public partial class RoboSep_RunSamples : BasePannel
    {
        private const char reservedChar = '^';
        private const char spaceChar = ' ';
        private const int MaxQuadrantNumberIndex = 3;   // zero based
        private static RoboSep_RunSamples myRunSamples;
        private QuadrantInfo[] myQuadrants;
        private SeparatorGateway mySeparatorGateway;
        private int iProtocolSelectCount;
 
        private int[] sharedSectorsTranslation = new int[4];
        private int[] sharedSectorsProtocolIndex = new int[4];
        private bool isSharing = false;
        private bool isInitialized = false;
        private string loginUserName;

        public readonly object SyncRoot = new object();

        Queue<ProtocolInfo2> queueLoadProtocols;


        // set language ini file
        private IniFile LanguageINI = GUI_Console.RoboSep_UserConsole.getInstance().LanguageINI;
        public EventHandler NotifyUserRestoreProtocolsFinished = null;
        public ProtocolInfoList lstProtocolInfo = null;

        private List<Image> iListLock;
        private List<Image> iListUnlock;
        private List<Image> iList;

        private EventHandler evhRunSamplesClick = null;
 
        private RoboSep_RunSamples()
        {
            InitializeComponent();
            mySeparatorGateway = SeparatorGateway.GetInstance();

            SuspendLayout();

            this.DoubleBuffered = true;
            this.UpdateStyles();

            // LOG
            string logMSG = "Initializing Run Samples user control";
            //GUI_Controls.uiLog.LOG(this, "RoboSep_RunSamples", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                                                                            
            iListLock = new List<Image>();
            iListLock.Add(Properties.Resources.lock_STD);
            iListLock.Add(Properties.Resources.lock_OVER);
            iListLock.Add(Properties.Resources.lock_OVER);
            iListLock.Add(Properties.Resources.lock_CLICK);

            iListUnlock = new List<Image>();
            iListUnlock.Add(Properties.Resources.unlock_STD);
            iListUnlock.Add(Properties.Resources.unlock_OVER);
            iListUnlock.Add(Properties.Resources.unlock_OVER);
            iListUnlock.Add(Properties.Resources.unlock_CLICK);

            // change graphics for buttons
            iList = new List<Image>();
   
            // forwards button 
            iList.Clear();
            iList.Add(Properties.Resources.L_104x86_single_arrow_right_STD);
            iList.Add(Properties.Resources.L_104x86_single_arrow_right_OVER);
            iList.Add(Properties.Resources.L_104x86_single_arrow_right_OVER);
            iList.Add(Properties.Resources.L_104x86_single_arrow_right_CLICK);
            button_RunSamples.ChangeGraphics(iList);

            // Quadrant Buttons initially set to disabled
            Q1.Enabled = false;
            Q2.Enabled = false;
            Q3.Enabled = false;
            Q4.Enabled = false;

            // Cancel selected Buttons
            iList.Clear();
            iList.Add(Properties.Resources.HM_BTN01L_delete_STD);
            iList.Add(Properties.Resources.HM_BTN01L_delete_OVER);
            iList.Add(Properties.Resources.HM_BTN01L_delete_OVER);
            iList.Add(Properties.Resources.HM_BTN01L_delete_CLCIK);
            button_CancelSelected.ChangeGraphics(iList);

            //UpdateRemoteDesktopButtons();

            // initialize myQuadrants
            myQuadrants = new QuadrantInfo[4] {
                new QuadrantInfo(Q1_protocolName, Q1_Vol, Q1_lbl_SelectProtocol, Q1),
                new QuadrantInfo(Q2_protocolName, Q2_Vol, Q2_lbl_SelectProtocol, Q2),
                new QuadrantInfo(Q3_protocolName, Q3_Vol, Q3_lbl_SelectProtocol, Q3),
                new QuadrantInfo(Q4_protocolName, Q4_Vol, Q4_lbl_SelectProtocol, Q4) };
            myQuadrants[0].setDivider(divider1);
            myQuadrants[1].setDivider(divider2);
            myQuadrants[2].setDivider(divider3);


            Rectangle rc1 = Q1.ClientRectangle;
            Rectangle rc2 = button_CancelSelected.ClientRectangle;

            Point ptCancelSelectedButton = this.button_CancelSelected.Location;
            ptCancelSelectedButton.X = Q1.Location.X - ((rc2.Width - rc1.Width) / 2);
            this.button_CancelSelected.Location = ptCancelSelectedButton;

            iProtocolSelectCount = 0;
            ResumeLayout();
        }

        private void RoboSep_RunSamples_Load(object sender, EventArgs e)
        {
            int[] ClearSharing = { 0, 0, 0, 0 };
            sharedSectorsTranslation = ClearSharing;
            sharedSectorsProtocolIndex = ClearSharing;

            RoboSep_UserConsole.getInstance().RestoreInitialOpacity();
            
            UserName = RoboSep_UserConsole.strCurrentUser;

            // attach run samples event click
            evhRunSamplesClick = new EventHandler(this.button_RunSamples_Click);
            this.button_RunSamples.Click += evhRunSamplesClick;

            // load user preferences
            RoboSep_UserDB.getInstance().loadCurrentUserPreferences(RoboSep_UserConsole.strCurrentUser);

            LoadProtocolsMRU();

            UpdateDeleteAndPlayButtonStates();

            isInitialized = true;
        }

        public static RoboSep_RunSamples getInstance()
        {
            if (myRunSamples == null)
            {
                myRunSamples = new RoboSep_RunSamples();
            }
            GC.Collect();

            return myRunSamples;
        }

        public string UserName
        {
            get { return loginUserName; }
            set
            {
                loginUserName = value;
                nameHeader.userName = loginUserName;
                string sDisplayName = loginUserName;
                int nIndex = sDisplayName.IndexOf(reservedChar);
                if (0 < nIndex)
                {
                    sDisplayName = sDisplayName.Replace(reservedChar, spaceChar);
                }
                button_Tab1.Text = sDisplayName;
                button_Tab1.Refresh();
            }
             
        }

        public bool IsInitialized
        {
            get { return isInitialized; }
        }

        public void ReInitialize()
        {
            //UpdateRemoteDesktopButtons();
            LoadProtocolsMRU();
        }

        public bool IsAnyQuadrantMarkedAsCancelled()
        {
            return IsQuadrantMarkedAsCancelled(0) ||
                   IsQuadrantMarkedAsCancelled(1) ||
                   IsQuadrantMarkedAsCancelled(2) ||
                   IsQuadrantMarkedAsCancelled(3);
        }


        public bool IsQuadrantMarkedAsCancelled(int Q)
        {
            if (0 > Q && Q > 3)
                return false;

            bool bMarkAsCancelled = false;
            switch (Q)
            {
                case 0:
                    bMarkAsCancelled = Q1.Cancelled;
                    break;
                case 1:
                    bMarkAsCancelled = Q2.Cancelled;
                    break;
               case 2:
                    bMarkAsCancelled = Q3.Cancelled;
                    break;
               case 3:
                    bMarkAsCancelled = Q4.Cancelled;
                    break;
                default:
                    break;
            }
            return bMarkAsCancelled;
        }

        public void MarkQuadrantAsCancelled(int Q)
        {
            if (0 > Q && Q > 3)
                return;

            switch (Q)
            {
                case 0:
                    Q1.Cancelled = true;
                    break;
                case 1:
                    Q2.Cancelled = true;
                    break;
                case 2:
                    Q3.Cancelled = true;
                    break;
                case 3:
                    Q4.Cancelled = true;
                    break;
                default:
                    break;
            }
        }

        public void LoadProtocols(ProtocolInfo[] arrProtocol)
        {
            if (arrProtocol == null)
                return;

            if (lstProtocolInfo == null)
            {
                lstProtocolInfo = new ProtocolInfoList();
            }
            
            lock (SyncRoot)
            {
                if (queueLoadProtocols == null)
                    queueLoadProtocols = new Queue<ProtocolInfo2>();

                foreach (ProtocolInfo pInfo in arrProtocol)
                {
                    ProtocolInfo2 pInfo2 = new ProtocolInfo2(pInfo.Quadrant, pInfo.ProtocolName, pInfo.SampleVolumeUl,
                    pInfo.NumberOfQuadrantRequired, pInfo.MarkAsCancelled, this);
                    queueLoadProtocols.Enqueue(pInfo2);
                 }
            }

            ThreadPool.QueueUserWorkItem(new WaitCallback(RestoreProtocols), queueLoadProtocols);
        }

        static void RestoreProtocols(Object state) 
        {
            Queue<ProtocolInfo2> pQueue = (Queue<ProtocolInfo2>)state;
            if (pQueue == null)
                return;

            ProtocolInfo2 protocolInfo2;
            while (pQueue.Count != 0)
            {
                protocolInfo2 = pQueue.Dequeue();
                if (protocolInfo2 == null || string.IsNullOrEmpty(protocolInfo2.ProtocolName))
                    continue;

                System.Diagnostics.Debug.WriteLine(String.Format("--------- Restore protocols {0}. Initial QuadrantId={1} --------------", protocolInfo2.ProtocolName, protocolInfo2.Quadrant));
                protocolInfo2.AddToRun();
                ProtocolInfoList.asyncOpsAreDone.WaitOne();
                System.Diagnostics.Debug.WriteLine(String.Format("--------- RestoreProtocols: WaitOne returned --------------"));
                ProtocolInfoList.asyncOpsAreDone.Reset();
             }

            RoboSep_RunSamples pRoboSepRunSamples = RoboSep_RunSamples.getInstance();
            if (pRoboSepRunSamples!= null && pRoboSepRunSamples.NotifyUserRestoreProtocolsFinished != null)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("--------- RestoreProtocols: BeginInvoke: NotifyUserRestoreProtocolsFinished --------------"));

                if (pRoboSepRunSamples.InvokeRequired)
                {
                    pRoboSepRunSamples.Invoke((MethodInvoker)delegate { pRoboSepRunSamples.NotifyUserRestoreProtocolsFinished(pRoboSepRunSamples, new EventArgs()); });
                }
                else
                {
                    pRoboSepRunSamples.NotifyUserRestoreProtocolsFinished(pRoboSepRunSamples, new EventArgs());
                }
            }
        }

        public void LoadProtocolsMRU()
        {
            bool loadMRUProtocolsAtStartUp = RoboSep_UserDB.getInstance().LoadMRUProtocolsAtStartUp;
            if (loadMRUProtocolsAtStartUp == false)
                return;

           // get protocols MRU 
            ProtocolMRUList MRUList = new ProtocolMRUList();
            MRUList.LoadUserProtocolsMRU(RoboSep_UserConsole.strCurrentUser);
            ProtocolMostRecentlyUsed[] arrLoadProtocols = MRUList.ListProtocolMRU;

            if (arrLoadProtocols != null && arrLoadProtocols.Length > 0)
            {
                if (queueLoadProtocols == null)
                {
                    queueLoadProtocols = new Queue<ProtocolInfo2>();
                }
    
                queueLoadProtocols.Clear();
                ISeparationProtocol pProtocol = null;
                for (int i = 0; i < arrLoadProtocols.Length; i++)
                {
                    if (arrLoadProtocols[i] == null || arrLoadProtocols[i].ProtocolLabel.Trim().Length == 0)
                        continue;

                    // check whether the protocol exists
                    pProtocol = getProtocolFromUserProtocolsList(arrLoadProtocols[i].ProtocolLabel);
                    if (pProtocol == null)
                        continue;


                    ProtocolInfo2 pInfo = new ProtocolInfo2((QuadrantId)arrLoadProtocols[i].QuadrantNumber, arrLoadProtocols[i].ProtocolLabel,
                                            arrLoadProtocols[i].SampleVolumeUl/1000, 1, false,  this);

                    queueLoadProtocols.Enqueue(pInfo);
                }

                if (queueLoadProtocols.Count > 0)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(RestoreProtocols), queueLoadProtocols);
                }
            }
         }

        private void PersistProtocolsMRU()
        {
            // get the list of selected protocols
            int [] arrProtocolsSelected = iSelectedProtocols;
            if (arrProtocolsSelected == null || arrProtocolsSelected.Length == 0)
                return;
    
            ProtocolMostRecentlyUsed[] arrProtocols = new ProtocolMostRecentlyUsed[4];
            if (arrProtocols == null)
                return;

            for (int i = 0; i < arrProtocols.Length; i++)
            {
                arrProtocols[i] = new ProtocolMostRecentlyUsed();
                arrProtocols[i].QuadrantNumber = i +1;

                foreach (int j in arrProtocolsSelected)
                {
                    if (i == j)
                    {
                        if (!myQuadrants[j].bIsMaintenance)
                        {
                            arrProtocols[i].ProtocolLabel = myQuadrants[j].QuadrantLabel;
                            arrProtocols[i].usageTime = DateTime.Now;
                            if (!string.IsNullOrEmpty(myQuadrants[j].GetVolumeLabel().Text) && myQuadrants[j].GetVolumeLabel().Text.Trim().Length != 0)
                            {
                                string strValue = myQuadrants[j].GetVolumeLabel().Text.Trim();
                                double volume = 0.00;
                                // get rid of the text mL
                                if (strValue.Contains("mL"))
                                {
                                    string temp = "";
                                    int index = strValue.LastIndexOf("mL");
                                    if (0 <= index)
                                    {
                                        temp = strValue.Substring(0, index);
                                        volume = Convert.ToDouble(temp);
                                    }
                                }
                                else
                                {
                                    volume = Convert.ToDouble(strValue);
                                }
                                arrProtocols[i].SampleVolumeUl = volume;
                                arrProtocols[i].SampleVolumeUl *= 1000;
                            }
                        }
                    }
                }
            }
 
            ProtocolMRUList MRUList = new ProtocolMRUList();
            if (MRUList != null)
            {
                MRUList.UpdateUserProtocolsMRU(RoboSep_UserConsole.strCurrentUser, arrProtocols);
            }
        }

        private bool CanCovert(String value, Type type)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(type);
            return converter.IsValid(value);
        }

        public void refreshSelectedProtocolsDisplayName()
        {
            for (int i =0; i < myQuadrants.Length; i++)
            {
                myQuadrants[i].RefreshDisplayName();
            }
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

        public QuadrantInfo[] RunInfo
        {
            get
            {
                return myQuadrants;
            }
            set
            {
                myQuadrants = value;
            }
        }

        public ProtocolClass getProtocolClassType(string protocolName)
        {
            ISeparationProtocol iProtocol = getProtocolFromUserProtocolsList(protocolName);
            ProtocolClass type = ProtocolClass.Undefined;
            if (iProtocol != null)
            {
                type = iProtocol.Classification;
            }
            return type;
        }

         ISeparationProtocol getProtocolFromUserProtocolsList(string protocolName)
         {
            if (string.IsNullOrEmpty(protocolName))
            {
                return null;
            }
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
            if (string.IsNullOrEmpty(ProtocolName))
                return;

            System.Diagnostics.Debug.WriteLine(String.Format("---------Called SelectProtocol"));


            // search for protocol in user Protocols list
            ISeparationProtocol selectedProtocol = getProtocolFromUserProtocolsList(ProtocolName);
            if (selectedProtocol == null)
            {
                // Sunny to do
                // Show error
                string sMsg = "Selected Protocol is not in the user list.";
                string sMSG = LanguageINI.GetString("msgProtocol");
                string sMSG1 = LanguageINI.GetString("msgNotInUserList");
                string sMSG2 = LanguageINI.GetString("msgEnter");
                string sMSG3 = sMSG + " " + ProtocolName + sMsg;

                GUI_Controls.RoboMessagePanel prompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_ERROR,
                    sMSG3, sMSG2, LanguageINI.GetString("Ok"));
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                prompt.Dispose();                
                RoboSep_UserConsole.hideOverlay();
                return;
            }

            // LOG
            string logMSG = "Adding protocol '" + ProtocolName + "' to run at quadrant" + Q.ToString();
            //GUI_Controls.uiLog.LOG(this, "addToRun", GUI_Controls.uiLog.LogLevel.DEBUG, logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);            

            // create array of strings to give to num pad for all messages
            string[] numPadMessages = new string[] {
                LanguageINI.GetString("NumPadmsg1"),
                LanguageINI.GetString("NumPadmsg2"),
                LanguageINI.GetString("NumPadmsg3"),
                LanguageINI.GetString("NumPadmsg4"),
                LanguageINI.GetString("Ok"),
                LanguageINI.GetString("Cancel"),
                LanguageINI.GetString("Enter"),
                LanguageINI.GetString("Clear")};

            // get volume from user
            NumberPad numPad = new NumberPad(RoboSep_UserConsole.getInstance(), RoboSep_UserConsole.getInstance().frmOverlay,
                myQuadrants[Q].GetVolumeLabel(), selectedProtocol.Description,
                selectedProtocol.MinimumSampleVolume / 1000, selectedProtocol.MaximumSampleVolume / 1000, numPadMessages);
            //numPad.disableCancelButton();
            RoboSep_UserConsole.getInstance().addForm(numPad, numPad.Offset);
            RoboSep_UserConsole.showOverlay();
            numPad.ShowDialog();

            if (numPad.DialogResult == DialogResult.Cancel)
            {
                numPad.Dispose();
                return;
            }
            numPad.Dispose();

            lock (SyncRoot)
            {
                string storeVolume = myQuadrants[Q].GetVolumeLabel().Text;

                // back up quadrant info, in case quadrant add doesn't fit
                QuadrantInfo tempQuadrant = myQuadrants[Q];
                if (myQuadrants[Q].QuadrantUsed)
                {
                    int initialQuadrant = Q;
                    for (int i=0; i<=Q; i++)
                    {
                        if (myQuadrants[Q-i].QuadrantLabel == string.Empty)
                        {
                            initialQuadrant--;
                        }
                        else
                            break;
                    }
                    CancelQuadrant(initialQuadrant);
                }

                myQuadrants[Q].GetVolumeLabel().Text = storeVolume;

                // check if protocol requires more than one quadrant
                if (selectedProtocol.QuadrantCount > 1)
                {
                    bool fit = true;
                    for (int i = 0; i < selectedProtocol.QuadrantCount; i++)
                    {
                        if ((Q + i) >= 4 || myQuadrants[Q + i].QuadrantUsed)
                            fit = false;
                    }

                    if (fit)
                    {
                        // first attempt to add protocol to server "chosen protocols" list
                        iProtocolSelectCount += 1;

                        RoboSep_UserConsole.getInstance().SelectProtocol((QuadrantId)(Q), selectedProtocol);

                        // verify protocol will fit (with server)
                        //mySeparatorGateway.VerifyProtocolSelection( mySamplesCurrentQuadrant, selectedProtocolIndex);

                        // pass protocol information to quadrant info
                        myQuadrants[Q].setProtocol(selectedProtocol);

                        EnableQuadrantButton((QuadrantId)Q, true);
                        UpdateQuadrantButtonSelectState((QuadrantId)Q, true);
                        UpdatePlayButtonState();

                        for (int i = 1; i < selectedProtocol.QuadrantCount; i++)
                        {
                            myQuadrants[Q + i].QuadrantUsed = true;
                            myQuadrants[Q + i].Update();
                            UpdateQuadrantButtonSelectState((QuadrantId)(Q+i), true);
                            if (i > 0 && myQuadrants[Q + i - 1].Divider != null)
                            {
                                myQuadrants[Q + i - 1].Divider.Visible = false;
                            }
                        }

                        // inform server of volume update
                        //SetSampleVolume((QuadrantId)Q, myQuadrants[Q].QuadrantVolume);
                        ThreadPool.QueueUserWorkItem(new WaitCallback(DoSetSampleVolumeHelper),new SetVolumeParam((QuadrantId)Q, myQuadrants[Q].QuadrantVolume));

                    }
                    else
                    {
                        // re add previously stored protocol (as selected protocol doesnt fit)
                        addToRun(Q, tempQuadrant.QuadrantLabel);
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("RunSamples 24: mySeparator.SelectProtocol: QuadrantId={0}, selectedProtocol.Id={1}", (QuadrantId)Q, selectedProtocol.Id));
 
                    // first attempt to add protocol to server "chosen protocols" list
                    RoboSep_UserConsole.getInstance().SelectProtocol((QuadrantId)(Q), selectedProtocol);

                    // pass protocol information to quadrant info
                    myQuadrants[Q].setProtocol(selectedProtocol);

                    // update protocol selected count
                     iProtocolSelectCount += 1;

                    // verify protocol will fit (with server)
                    //mySeparatorGateway.VerifyProtocolSelection((QuadrantId)(Q), selectedProtocolIndex);

                    EnableQuadrantButton((QuadrantId)Q, true);
                    UpdateQuadrantButtonSelectState((QuadrantId)Q, true);
                    UpdatePlayButtonState();

                    //System.Diagnostics.Debug.WriteLine(String.Format("RunSamples 25: mySeparator.SelectProtocol: QuadrantId={0}, selectedProtocol.Id={1}", (QuadrantId)Q, selectedProtocol.Id));
 
                    // inform server of volume update
                    //SetSampleVolume((QuadrantId)Q, myQuadrants[Q].QuadrantVolume); 
                    ThreadPool.QueueUserWorkItem(new WaitCallback(DoSetSampleVolumeHelper),new SetVolumeParam((QuadrantId)Q, myQuadrants[Q].QuadrantVolume));

                    
                    //System.Diagnostics.Debug.WriteLine(String.Format("RunSamples 26: mySeparator.SelectProtocol: QuadrantId={0}, selectedProtocol.Id={1}", (QuadrantId)Q, selectedProtocol.Id));
 
                }
            }
        }


        //This whole thing is a a work around to avoid a race condition where
        //RoboSep_UserConsole.getInstance().SelectProtocol((QuadrantId)(Q), selectedProtocol); is not done before
        //SetSampleVolume((QuadrantId)Q, myQuadrants[Q].QuadrantVolume);
        //When is happens, myChosenProtocols is empty in QuadrantManager.
        //SelectProtocol is defined with [OneWay] so it becomes async and you can't be sure that it is done before SetSampleVolume is called
        //The proper way to fix is to sent events or use wait objects
        //But I see some code with wait objects already here but not used in this case and I am not sure why so I don't want to just use it.
        //Also AddToRun is very entwined and I don't want to mess with it too much for now
        private void DoSetSampleVolumeHelper(object stateinfo)
        {
            SetVolumeParam param = (SetVolumeParam)stateinfo;
            Thread.Sleep(500);
            DoSetSampleVolumeHelperInUiThread(param.Q, param.vol);
        }

        private void DoSetSampleVolumeHelperInUiThread(QuadrantId Q, double vol)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate() { this.DoSetSampleVolumeHelperInUiThread(Q, vol); });
            }
            else
            {
                SetSampleVolume(Q, vol); 
            }
        }

        public bool addToRun(int Q, string ProtocolName, double Vol )
        {
            // search for protocol in user Protocols list
            ISeparationProtocol selectedProtocol = getProtocolFromUserProtocolsList(ProtocolName);
            if (selectedProtocol == null)
            {
                // Sunny to do
                // should show error in status bar
                // Show error
                string sMSG = LanguageINI.GetString("msgProtocol");
                string sMSG1 = LanguageINI.GetString("msgNotInUserList");
                string sMSG2 = LanguageINI.GetString("msgEnter");
                string sMSG3 = sMSG + " " + ProtocolName + sMSG1;

                GUI_Controls.RoboMessagePanel prompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_ERROR,
                    sMSG3, sMSG2, LanguageINI.GetString("Ok"));
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                prompt.Dispose();                
                RoboSep_UserConsole.hideOverlay();
                return false;
            }

            double VolUL = Vol * 1000;
            if (VolUL < selectedProtocol.MinimumSampleVolume || VolUL > selectedProtocol.MaximumSampleVolume)
            {
                return false;
            }

            lock (SyncRoot)
            {            
                // back up quadrant info, in case quadrant add doesn't fit
                QuadrantInfo tempQuadrant = myQuadrants[Q];
                if (myQuadrants[Q].QuadrantUsed)
                {
                    iProtocolSelectCount -= 1;
                    for (int i = 0; i < myQuadrants[Q].QuadrantsRequired - 1; i++)
                    {
                        System.Diagnostics.Debug.WriteLine(String.Format("Run Samples 30 : addToRun : Clear QuadrantId={0}, protocol = {1).", (QuadrantId)(Q + i), myQuadrants[Q + i].QuadrantLabel));
                        myQuadrants[Q + i].Clear();
                    }
                }

                myQuadrants[Q].QuadrantVolume = Vol;
                Label tempLabel = myQuadrants[Q].GetVolumeLabel();
                tempLabel.Text = string.Format("{0:000}", Vol);

                // check if protocol requires more than one quadrant
                if (selectedProtocol.QuadrantCount > 1)
                {
                    bool fit = true;
                    for (int i = 0; i < selectedProtocol.QuadrantCount; i++)
                    {
                        if ((Q + i) >= 4 || myQuadrants[Q + i].QuadrantUsed)
                            fit = false;
                    }

                    if (fit)
                    {
                        // first attempt to add protocol to server "chosen protocols" list
                         System.Diagnostics.Debug.WriteLine(String.Format("RunSamples 31 : mySeparator.SelectProtocol: QuadrantId={0}, selectedProtocol.Id={1}", (QuadrantId)Q, selectedProtocol.Id));

                        // RoboSep_UserConsole.getInstance().SelectProtocol((QuadrantId)(Q), selectedProtocol);

                        ProtocolSeparatorInputParam pParam = new ProtocolSeparatorInputParam((QuadrantId)Q, selectedProtocol, Vol, this);
                        ThreadPool.QueueUserWorkItem(new WaitCallback(SepartorSelectProtocol), pParam);
  
                        System.Diagnostics.Debug.WriteLine(String.Format("RunSamples 21 : mySeparator.SelectProtocol: QuadrantId={0}, selectedProtocol.Id={1}", (QuadrantId)Q, selectedProtocol.Id));

                        // verify protocol will fit (with server)
                        //mySeparatorGateway.VerifyProtocolSelection( mySamplesCurrentQuadrant, selectedProtocolIndex);

                        // pass protocol information to quadrant info
                        myQuadrants[Q].setProtocol(selectedProtocol);
                        for (int i = 1; i < selectedProtocol.QuadrantCount; i++)
                        {
                            myQuadrants[Q + i].QuadrantUsed = true;
                            myQuadrants[Q + i].Update();
                            UpdateQuadrantButtonSelectState((QuadrantId)(Q + i), true);
                            if (i > 0 && myQuadrants[Q + i - 1].Divider != null)
                            {
                                myQuadrants[Q + i - 1].Divider.Visible = false;
                            }
                        }

                        // update protocol selected count
                        iProtocolSelectCount += 1;

                        // enable the quadrant number button
                        EnableQuadrantButton((QuadrantId)Q, true);
                        UpdateQuadrantButtonSelectState((QuadrantId)Q, true);
                        UpdatePlayButtonState();

                        // inform server of volume update
                       // SetSampleVolume((QuadrantId)Q, Vol);
                    }
                }
                else
                {
                    // first attempt to add protocol to server "chosen protocols" list
                    System.Diagnostics.Debug.WriteLine(String.Format("RunSamplesWindow : mySeparator.SelectProtocol: QuadrantId={0}, selectedProtocol.Id={1}", (QuadrantId)Q, selectedProtocol.Id));

                    // RoboSep_UserConsole.getInstance().SelectProtocol((QuadrantId)(Q), selectedProtocol);

                    ProtocolSeparatorInputParam pParam = new ProtocolSeparatorInputParam((QuadrantId)Q, selectedProtocol, Vol, this);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(SepartorSelectProtocol), pParam);
 
                    // pass protocol information to quadrant info
                    myQuadrants[Q].setProtocol(selectedProtocol);

                    // update protocol selected count
                    iProtocolSelectCount += 1;

                    // enable the quadrant number button
                    EnableQuadrantButton((QuadrantId)Q, true);
                    UpdateQuadrantButtonSelectState((QuadrantId)Q, true);
                    UpdatePlayButtonState();

                    // inform server of volume update
                   // SetSampleVolume((QuadrantId)Q, Vol);
                }
            }
            return true;
        }

        static void SepartorSelectProtocol(Object state)
        {
            ProtocolSeparatorInputParam pParam = (ProtocolSeparatorInputParam)state;
            if (pParam == null)
                return;

            // Select a protocol is a two steps process
            // 1.) Select the protocol
            // 2.) Set sample volume
            //
            // Note: those 2 steps are not performed indivually in one single call. They are a chain of calls. First it calls the server to select the protocol. 
            //       Then the server calls the Quadrant Manager to select the protocol. The Quadrant Manager performs a series of other tasks that may call the server again.
            //       Synchorization is a big problem if it starts to select another protocol without completing the previous selection.
            // 
            ProtocolSeparatorInputParam.asyncProtocolSelectionIsDone.Reset();

            System.Diagnostics.Debug.WriteLine(String.Format("SepartorSelectProtocol : pParam.SepatorSelectProtocol"));

            pParam.SepatorSelectProtocol();

            ProtocolSeparatorInputParam.asyncProtocolSelectionIsDone.WaitOne();
            ProtocolSeparatorInputParam.asyncProtocolSelectionIsDone.Reset();

            System.Diagnostics.Debug.WriteLine(String.Format("SepartorSelectProtocol : pParam.SetSampleVolume"));

            pParam.SetSampleVolume();

            // Wait until set volume is completed.
            ProtocolSeparatorInputParam.asyncProtocolSelectionIsDone.WaitOne();

            // Set volume has been completed. Signal that it is ready to select another protocol.
            ProtocolInfoList.asyncOpsAreDone.Set();
        }

        public void SetSampleVolume(QuadrantId Q, double vol)
        {
            // inform server of volume update
            FluidVolume QVolume = new FluidVolume(vol, Tesla.Common.FluidVolumeUnit.MilliLitres);
            System.Diagnostics.Debug.WriteLine(String.Format("RunSamples 32 : setSampleVolume QuadrantId={0}, Volume={1}", (QuadrantId)Q, QVolume.Amount));
            RoboSep_UserConsole.getInstance().SetSampleVolume(Q, QVolume);
            System.Diagnostics.Debug.WriteLine(String.Format("RunSamples 33 : setSampleVolume QuadrantId={0} completed.", (QuadrantId)Q));
        }

        public void SeparatorSelectProtocol(QuadrantId Q, IProtocol protocol)
        {
            RoboSep_UserConsole.getInstance().SelectProtocol(Q, protocol);
        }


        public void EnableQuadrantButton(QuadrantId Q, bool bEnable)
        {
            if (Q < QuadrantId.Quadrant1 || Q > QuadrantId.Quadrant4)
                return;

            switch (Q)
            {
                case QuadrantId.Quadrant1:
                    Q1.Enabled = bEnable;
                    break;
                case QuadrantId.Quadrant2:
                    Q2.Enabled = bEnable;
                    break;
                case QuadrantId.Quadrant3:
                    Q3.Enabled = bEnable;
                    break;
                case QuadrantId.Quadrant4:
                    Q4.Enabled = bEnable;
                    break;
                default:
                    break;
            }
        }

        private void UpdateQuadrantButtonSelectState(QuadrantId Q, bool bSelected)
        {
            if (Q < QuadrantId.Quadrant1 || Q > QuadrantId.Quadrant4)
                return;

            switch (Q)
            {
                case QuadrantId.Quadrant1:
                    Q1.Selected = bSelected;
                    Q1.Invalidate();
                    break;
                case QuadrantId.Quadrant2:
                    Q2.Selected = bSelected;
                    Q2.Invalidate();
                    break;
                case QuadrantId.Quadrant3:
                    Q3.Selected = bSelected;
                    Q3.Invalidate();
                    break;
                case QuadrantId.Quadrant4:
                    Q4.Selected = bSelected;
                    Q4.Invalidate();
                    break;
                default:
                    break;
            }
        }

        private void UpdateDeleteAndPlayButtonStates()
        {
            if (iSelectedProtocols.Length > 0)
            {
                disableRun = (Q1.Cancelled || Q2.Cancelled || Q3.Cancelled || Q4.Cancelled) == true ? true : false;
                button_CancelSelected.Visible = (Q1.Cancelled || Q2.Cancelled || Q3.Cancelled || Q4.Cancelled) == true ? true : false;
                btn_home.Visible = button_CancelSelected.Visible ? false : true;
            }
            else
            {
                disableRun = true;
                button_CancelSelected.Visible = false;
                btn_home.Visible = true;
            }
        }

        private void UpdatePlayButtonState()
        {
            if (iSelectedProtocols.Length > 0)
            {
                disableRun = (Q1.Cancelled || Q2.Cancelled || Q3.Cancelled || Q4.Cancelled) == true ? true : false;
            }
            else
            {
                disableRun = true;
            }
        }

        private bool verifyProtocols()
        {
            Tesla.Common.Protocol.ISeparationProtocol[] serverList = RoboSep_UserConsole.getInstance().XML_getUserProtocols();
            for (int i = (int)QuadrantId.Quadrant1; i < (int)QuadrantId.NUM_QUADRANTS; i++)
            {
                if (myQuadrants[i].QuadrantLabel != string.Empty)
                {
                    bool protocolMatch = false;
                    for (int j = 0; j < serverList.Length; j++)
                    {
                        if (myQuadrants[i].QuadrantLabel == serverList[j].Label)
                        {
                            protocolMatch = true;
                            break;
                        }
                    }
                    if (!protocolMatch)
                    {
                        return false;
                    }

                }
            }
            return true;
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

        private SharingProtocol[] getConsumables()
        {
            string logMSG;
            // generate lot IDs for all quadrants
            
            System.Diagnostics.Debug.WriteLine(String.Format("---------Run Samples: getConsumables called : iProtocolSelectCount = {0}", iProtocolSelectCount));
            int nIndex = 0;
            SharingProtocol[] AllProtocols = null;

            try
            {

                AllProtocols = new SharingProtocol[iProtocolSelectCount];
                for (int i = 0; i < AllProtocols.Length; i++)
                {
                    string name;
                    ProtocolClass type;
                    int initQuad = 0;
                    double amount = 0;
                    string[] customNames;
                    int numQuadrants;
                    
                    nIndex = i;

                    ProtocolConsumable[,] consumables;
                    consumables = mySeparatorGateway.GetProtocolConsumables(i, out name, out type, out initQuad);
                    if (consumables == null)
                    {
                        System.Diagnostics.Debug.WriteLine(String.Format("---------getConsumables: mySeparatorGateway.GetProtocolConsumables return consumables = null."));

                        // Log error
                        logMSG = "getConsumables call failed to get Protocol Consumables from mySeparatorGateway. ";
                        LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);            
                        return null;
                    }


                    amount = consumables[0, (int)RelativeQuadrantLocation.SampleTube].Volume.Amount;

                    // Check if protocol uses standard reagent kits
                    bool[] resultChecks;
                    bool isUseDefault =  true;

                    
                    mySeparatorGateway.GetResultVialSelection(name, out resultChecks, out numQuadrants);
                    for (int j = 0; j < resultChecks.Length; j++)
                    {
                        if (resultChecks[j])
                        {

                            // Mar 18, 2013. Sunny 
                            // The following line of code is not called normally except for 'Retry' from the original codes.
                            // Always use the default.
                            //
                            //  isUseDefault = false;
                        }
                    }
                    
                    if (isUseDefault)
                    {
                        switch (type)
                        {
                            case ProtocolClass.Positive:
                            case ProtocolClass.HumanPositive:
                            case ProtocolClass.MousePositive:
                            case ProtocolClass.WholeBloodPositive:
                                consumables[0, (int)RelativeQuadrantLocation.SeparationTube].IsRequired = true;
                                break;
                            case ProtocolClass.Negative:
                            case ProtocolClass.HumanNegative:
                            case ProtocolClass.MouseNegative:
                            case ProtocolClass.WholeBloodNegative:
                                if (consumables.GetLength(0) < 2)
                                {
                                    consumables[0, (int)RelativeQuadrantLocation.LysisBufferTube].IsRequired = true;
                                }
                                else
                                {
                                    //iUseQOffset+=1;
                                    //consumables[0,(int)RelativeQuadrantLocation.SampleTube].IsRequired=true;
                                    //consumables[1, (int)RelativeQuadrantLocation.SampleTube].IsRequired = true;
                                }
                                break;
                            default:
                                consumables[0, (int)RelativeQuadrantLocation.WasteTube].IsRequired = true;
                                break;
                        }
                        //customNames = new string[] { "Buffer Bottle", "Waste Tube", "Negative Fraction Tube\n",
                        //           "Magnetic Particle", "Negative Selection", "Antibody", "Sample Tube", "Separation Tube"};
                    }
                    else
                    {
                        
                        consumables[0, (int)RelativeQuadrantLocation.QuadrantBuffer].IsRequired = resultChecks[0];
                        consumables[0, (int)RelativeQuadrantLocation.WasteTube].IsRequired = resultChecks[1];
                        consumables[0, (int)RelativeQuadrantLocation.LysisBufferTube].IsRequired = resultChecks[2];
                        consumables[0, (int)RelativeQuadrantLocation.VialA].IsRequired = resultChecks[3];
                        consumables[0, (int)RelativeQuadrantLocation.VialB].IsRequired = resultChecks[4];
                        consumables[0, (int)RelativeQuadrantLocation.VialC].IsRequired = resultChecks[5];
                        consumables[0, (int)RelativeQuadrantLocation.SampleTube].IsRequired = resultChecks[6];
                        consumables[0, (int)RelativeQuadrantLocation.SeparationTube].IsRequired = resultChecks[7];
                    }

                    mySeparatorGateway.GetCustomNames(name, out customNames, out numQuadrants);
                    // fix long strings in custom names
                    // add \n to names longer than 28 chars
                    for (int iName = 0; iName < customNames.Length; iName++)
                    {
                        if (customNames[iName].Length > 27)
                        {
                            string[] split = customNames[iName].Split();
                            string tempString = split[0];
                            int LineBreakPoint = split[0].Length;
                            bool lineBroken = false;
                            for (int secNum = 1; secNum < split.Length; secNum++)
                            {
                                if (LineBreakPoint + 1 + split[secNum].Length < 30)
                                {
                                    LineBreakPoint = LineBreakPoint + 1 + split[secNum].Length;
                                    tempString += " " + split[secNum];
                                }
                                else if (!lineBroken)
                                {
                                    tempString += "\n" + split[secNum];
                                    lineBroken = true;
                                }
                                else
                                {
                                    tempString += " " + split[secNum];
                                }
                            }
                            customNames[iName] = tempString;
                        }
                    }

                    AllProtocols[i] = new SharingProtocol(name, amount, initQuad, consumables, customNames);

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("---------Run Samples - getConsumables failed for quadrant {0}: Exception: {1}.", ++nIndex, ex.Message));
               
                // LOG
                logMSG = string.Format("Failed to get consumables for quadrant {0}. Exception: {1}.", nIndex, ex.Message);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);            

                return getConsumables();
            }
            
            // LOG
            logMSG = "Getting Consumables for " + iProtocolSelectCount.ToString() + " quadrants";
            //GUI_Controls.uiLog.LOG(this, "getConsumables", GUI_Controls.uiLog.LogLevel.DEBUG, logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);            

            return AllProtocols;
        }

        private void setSharing( SharingProtocol[] allProtocols)
        {
            if (allProtocols == null)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("---------setSharing: allProtocols = null--------------"));
            }
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
            //GUI_Controls.uiLog.LOG(this, "addMaintenance", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                                                                            
            // remove all other protocols
            for (int i = 0; i < (int)QuadrantId.NUM_QUADRANTS; i++)
            {
                if (myQuadrants[i].QuadrantsRequired > 0)
                    RoboSep_UserConsole.getInstance().DeselectProtocol((QuadrantId)(i));

                CancelQuadrant(i);
            }

            // add Maintenance protocol as only protocol in run.
            try
            {
                RoboSep_UserConsole.getInstance().SelectProtocol(QuadrantId.Quadrant1, MaintenanceProtocol);
                RoboSep_UserConsole.getInstance().ScheduleRun(QuadrantId.Quadrant1);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "Selected " + MaintenanceProtocol.Label);

                int Q = (int)QuadrantId.Quadrant1;

                myQuadrants[Q].QuadrantsRequired = MaintenanceProtocol.QuadrantCount;
                myQuadrants[Q].QuadrantLabel = MaintenanceProtocol.Label;
                iProtocolSelectCount = 1;

                EnableQuadrantButton(QuadrantId.Quadrant1, true);
                UpdateQuadrantButtonSelectState(QuadrantId.Quadrant1, true);
                UpdatePlayButtonState();

                for (int i = Q; i < (Q + MaintenanceProtocol.QuadrantCount); i++)
                {
                    myQuadrants[i].QuadrantUsed = true;
                    myQuadrants[i].bIsMaintenance = true;
                    myQuadrants[i].Update();
                    UpdateQuadrantButtonSelectState((QuadrantId)i, true);
                    if (i > Q && myQuadrants[i - Q].Divider != null)
                    {
                        myQuadrants[i - Q].Divider.Visible = false;
                    }
                }

            }
            catch (Exception ex)
            {
                // LOG
                LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);               
            }
        }

        private bool ConfirmLidClosed()
        {
            bool isLidClosed = mySeparatorGateway.IsLidClosed();
            return isLidClosed;
        }

        public void Unload()
        {
            // remove protocols from form
            for (int i = 0; i < 4; i++)
            {
                ResetSampleButtonInQuadrant(i);
            }
            iProtocolSelectCount = 0;

            // LOG
            string logMSG = "Clearing all selected protocols";
            //GUI_Controls.uiLog.LOG(this, "Unload", GUI_Controls.uiLog.LogLevel.INFO, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                                                                            
        }

        public bool disableRun
        {
            set
            {
                button_RunSamples.disable(value);
                button_RunSamples.Invalidate();
            }
            get
            {
                return !button_RunSamples.Enabled;
            }
        }

        private void scheduleRun()
        {
            // protocols are all separation type
            // schedule all runs
            for (int i = 0; i < (int)QuadrantId.NUM_QUADRANTS; i++)
            {
                if (myQuadrants[i].QuadrantsRequired > 0)
                    RoboSep_UserConsole.getInstance().ScheduleRun((QuadrantId)(i));
            }
        }

        private void button_RunSamples_Click(object sender, EventArgs e)
        {
            // LOG
            string logMSG = "Run Samples button clicked";
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                                                                            

            // prevent multiple clicks
            this.button_RunSamples.Click -= evhRunSamplesClick;


            while (SeparatorGateway.GetInstance().ControlApi.IsSchedulerBusy())
            {
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, ">>Scheduler in progress");
                Thread.Sleep(250);
            }

            // at least 1 protocol must be selected so that this button can be activated
            if (iProtocolSelectCount > 0)
            {
                SeparatorState myState = mySeparatorGateway.SeparatorState;
                int stateCheckCount = 0;
                while (myState != SeparatorState.Ready && stateCheckCount < 10)
                {
                    System.Threading.Thread.Sleep(100);
                    myState = mySeparatorGateway.SeparatorState;
                    stateCheckCount++;
                    if (stateCheckCount < 2) { scheduleRun(); }
                }

                if (myState == SeparatorState.Ready)
                {

                    bool requiresResources = true;
                    SharingProtocol[] myRunConfig = null;
                    {
                        // schedule runs
                        scheduleRun();

                        // LOG
#if true
                        while (SeparatorGateway.GetInstance().ControlApi.IsISeparatorBusy())
                        {
                            Application.DoEvents();
                            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "---->> ISeparator busy");                                                                                                        
                            Thread.Sleep(10);
                        }
#endif
                        LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "Scheduling Run");                                                                            
                        // update myRunConfig after setting sharing options
                        RoboSep_Resources ResourceWindow = RoboSep_Resources.getInstance();
                        string err = "fake error";
                        while (err != "")
                        {
                            myRunConfig = getConsumables();
                            if (myRunConfig == null)
                                break;
                            setSharing(myRunConfig);

                            err = mySeparatorGateway.isSharingComboValid(isSharing, sharedSectorsTranslation, sharedSectorsProtocolIndex);

                            if (err != "")
                            {
                                RoboMessagePanel errorPrompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_ERROR,
                                    LanguageINI.GetString("msgOverload2") + err, LanguageINI.GetString("Error"), LanguageINI.GetString("Ok"));
                                RoboSep_UserConsole.showOverlay();
                                errorPrompt.ShowDialog();
                                RoboSep_UserConsole.hideOverlay();
                                errorPrompt.Dispose();


                                // Attach the event handler again
                                this.button_RunSamples.Click += evhRunSamplesClick;

                                return;
                            }


                        }

                        // run protocol
                        // load user name to server
                        mySeparatorGateway.BatchRunUserId = RoboSep_UserConsole.strCurrentUser;


                        int protocolIdx = 0, tmpIdx;

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
                                    RoboSep_UserDB.getInstance().getProtocolCommandList(name, out commands, out times, out processDescriptions);
                                }
                                else
                                {
                                    // set file names for maintenance protocols (file names don't match labels)
                                    name = RoboSep_UserDB.getInstance().getProtocolFilename(myQuadrants[i].QuadrantLabel);
                                    logMSG = "protocol: " + name + " is acquired.";
                                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);            

#if false
                                    //int[] commands2;
                                    //int[] times2;
                                    //string[] processDescriptions2;
                                    //RoboSep_UserDB.getInstance().getProtocolCommandList(name, out commands2, out times2, out processDescriptions2);
                                    RoboSep_UserDB.getInstance().getProtocolCommandList(name, out commands, out times, out processDescriptions);
#else
                                    tmpIdx = protocolIdx;
                                    logMSG = ">> Starting acquiring protocol command list" + "[" + tmpIdx.ToString() + "]";
                                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);

                                    logMSG = ">>>> Entering to getProtocolCommandList" + "[" + protocolIdx.ToString() + "]";
                                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);            
                                    mySeparator = SeparatorGateway.GetInstance().ControlApi;
                                    XmlRpcProtocolCommand[] cmds = mySeparator.getProtocolCommandList(protocolIdx);
                                    logMSG = ">>>> Leaving getProtocolCommandList" + "[" + protocolIdx.ToString() + "]";
                                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);            
                                    protocolIdx++;

                                    ArrayList tmpCmds = new ArrayList();
                                    ArrayList tmpTimes = new ArrayList();
                                    ArrayList tmpDescription = new ArrayList();

                                    foreach (XmlRpcProtocolCommand c in cmds)
                                    {
                                        tmpCmds.Add(c.cmd);
                                        tmpTimes.Add(c.time);
                                        tmpDescription.Add(c.description);
                                    }

                                    logMSG = ">>>> Entering to ConvertCmdStringListToIntList";
                                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);            
                                    commands = ConvertCmdStringListToIntList(tmpCmds); //(int[])tmpCmds.ToArray(typeof(int));
                                    logMSG = ">>>> Leaving ConvertCmdStringListToIntList";
                                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);            
                                    times = (int[])tmpTimes.ToArray(typeof(int));
                                    processDescriptions = (string[])tmpDescription.ToArray(typeof(string));

                                    logMSG = ">> finishing acquiring protocol command list" + "[" + tmpIdx.ToString() + "]";
                                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);            

#endif
                                }
                                
                                // LOG
                                logMSG = "protocol: " + name + " commands: " + commands.Length.ToString();
                                LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);            

                                RoboSep_RunProgress. RunInfo(myQuadrants[i].QuadrantLabel ,i, times, commands, processDescriptions, myQuadrants[i].QuadrantsRequired);

                                logMSG = "Run progress info set.";
                                LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);

                                if (name == "home_axes.xml")
                                {
                                    // LOG
                                    //GUI_Controls.uiLog.LOG(this, "button_RunSamples_Click", GUI_Controls.uiLog.LogLevel.DEBUG, "Home Axex protocol, skip resources");

                                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "Home All protocol, skipping resource window");                                                                            

                                    // home does not have any quadrant resources required.  Skip right to run window
                                    // run protocols,  run progress window should be set up through
                                    // run samples window before coming to resources window.
                                    // do separatorgateway.startrun at this point
                                    //RoboSep_RunSamples.getInstance().startSeparationRun();

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
                                    RoboSep_RunProgress.getInstance().Start(true);
                                    requiresResources = false;
                                    break;
                                }
                            }
                        }

                        if (requiresResources)
                        {
                            // open resources help window
                            
                            ResourceWindow.setSharing(myRunConfig, isSharing, sharedSectorsTranslation, sharedSectorsProtocolIndex); //, myReagentLotIds);
                            if (ResourceWindow.IsInitialized)
                                ResourceWindow.LoadResources();

                            ResourceWindow.Location = new Point(0, 0);
                            this.Visible = false;
                            RoboSep_UserConsole.getInstance().Controls.Add(ResourceWindow);
                            RoboSep_UserConsole.getInstance().Controls.Remove(RoboSep_UserConsole.ctrlCurrentUserControl);
                            RoboSep_UserConsole.ctrlCurrentUserControl = ResourceWindow;
                            ResourceWindow.Visible = true;
                            ResourceWindow.BringToFront();
                            
                            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "Resource window is foreground");                                                                            
                        }
                    }
                }
            }

            // Attach the event handler again
            this.button_RunSamples.Click += evhRunSamplesClick;
        }

        private int[] ConvertCmdStringListToIntList(ArrayList tmpCmds)
        {
            ArrayList tmpInts = new ArrayList();
            string[] cmdStrings = new string[] 
                    {
                        
                        "IncubateCommand",                        
                        "MixCommand",
                        "TransportCommand",
                        "SeparateCommand",
                        "ResuspendVialCommand",
                        "TopUpVialCommand",
                        "FlushCommand",
                        "PrimeCommand",
                        "HomeAllCommand",
                        "DemoCommand",
                        "ParkCommand",
                        "PumpLifeCommand",
                        "MixTransCommand",
                        "TopUpMixTransCommand",
                        "ResusMixSepTransCommand",
                        "ResusMixCommand",
                        "TopUpTransCommand",
                        "TopUpTransSepTransCommand",
                        "TopUpMixTransSepTransCommand",
                        "EndOfProtocolCommand"
                    };
            foreach (string s in tmpCmds)
            {
                int isCommand = 0;
                for (int i = 0; i < cmdStrings.Length; i++)
                {
                    if (s == cmdStrings[i])
                    {
                        isCommand = i;
                        break;
                    }
                }
                tmpInts.Add(isCommand);
            }
            return (int[])tmpInts.ToArray(typeof(int));
        }

        public void startSeparationRun()
        {
            RoboSep_Resources ResourceWindow = RoboSep_Resources.getInstance();

            ReagentLotIdentifiers[][] reagentLotIdentifiers = ResourceWindow.ReagentLotIdentifiers;

            mySeparatorGateway.StartRun(isSharing, sharedSectorsTranslation, sharedSectorsProtocolIndex, reagentLotIdentifiers); 
            RoboSep_RunProgress.getInstance().Sharing = isSharing;

            // Update the MRU in the file userinfo.ini 
            PersistProtocolsMRU();

            // LOG
            string logMSG = "Start Separation run";
            //GUI_Controls.uiLog.LOG(this, "CheckHydraulicFluid", GUI_Controls.uiLog.LogLevel.GENERAL, logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Warning, logMSG);
        }
         
 
        private void Q1_Select(object sender, EventArgs e)
        {
            Protocol_Selection(0);
        }
        private void Q2_Select(object sender, EventArgs e)
        {
            Protocol_Selection(1);
        }
        private void Q3_Select(object sender, EventArgs e)
        {
            Protocol_Selection(2);
        }
        private void Q4_Select(object sender, EventArgs e)
        {
            Protocol_Selection(3);
        }

        private void Protocol_Selection(int QuadrantNumber)
        {
            // don't allow protocol selection if maintenance
            // protocol is present
            if (myQuadrants[0].bIsMaintenance)
            {
                string sTitle = QuadrantNumber != 0 ? LanguageINI.GetString("headerProtocolMixing") : LanguageINI.GetString("headerMaintenancePresent");
                string sMSG = QuadrantNumber != 0 ? LanguageINI.GetString("msgProtocolMixing") : LanguageINI.GetString("msgMaintenancePresent");

                RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING, sMSG, sTitle, LanguageINI.GetString("Ok"));
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                prompt.Dispose();                
                RoboSep_UserConsole.hideOverlay();
            }
                // don't allow protocol selection if protocol is present
            else if (myQuadrants[QuadrantNumber].bQuadrantInUse)
            {
                string sTitle = LanguageINI.GetString("headerProtocolMixing");
                string sMSG = LanguageINI.GetString("existingProtocolSelectionError");

                RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING, sMSG, sTitle, LanguageINI.GetString("Ok"));
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                prompt.Dispose();
                RoboSep_UserConsole.hideOverlay();
            }
            else
            {
                if (myQuadrants[QuadrantNumber].QuadrantUsed)
                {
                    int initialQuadrant = QuadrantNumber;
                    for (int i = 0; i <= QuadrantNumber; i++)
                    {
                        if (myQuadrants[QuadrantNumber - i].QuadrantLabel == string.Empty)
                        {
                            initialQuadrant--;
                        }
                        else
                        {
                            QuadrantNumber = initialQuadrant;
                            break;
                        }
                    }
                }

                // count available quadrants
                int available = myQuadrants[QuadrantNumber].QuadrantsRequired;
                for (int i = (QuadrantNumber + myQuadrants[QuadrantNumber].QuadrantsRequired); i < 4; i++)
                {
                    if (myQuadrants[i].QuadrantUsed)
                        break;
                    available += 1;
                }
                // Sends user to Protocol Selection window
                RoboSep_ProtocolSelect ProtocolSelectWindow = RoboSep_ProtocolSelect.GetInstance();
                if (ProtocolSelectWindow != null)
                {
                    ProtocolSelectWindow.Dispose();
                    ProtocolSelectWindow = null;
                }
                ProtocolSelectWindow = new RoboSep_ProtocolSelect((QuadrantId)QuadrantNumber, available, myQuadrants);
                ProtocolSelectWindow.Location = new Point(0, 0);
                RoboSep_UserConsole.getInstance().Controls.Add(ProtocolSelectWindow);
                RoboSep_UserConsole.getInstance().Controls.Remove(RoboSep_UserConsole.ctrlCurrentUserControl);
                RoboSep_UserConsole.ctrlCurrentUserControl = ProtocolSelectWindow;
                this.Visible = false;
                ProtocolSelectWindow.BringToFront();
                ProtocolSelectWindow.Focus();

                // LOG
                string logMSG = "Opening protocol selection for quadrant " + (QuadrantNumber +1).ToString() ;
                //GUI_Controls.uiLog.LOG(this, "Protocol_Selection", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
                //  (logMSG);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                                                                            
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
            if (myQuadrants[Q].QuadrantUsed)
            {
                // check if quadrant is used by another protocol (above)
                if (myQuadrants[Q].QuadrantVolume == 0)
                { /* do nothing */ }
                else
                {
                    // LOG
                    string logMSG = "Modifying volume for quadrant " + (Q + 1).ToString();
                    //GUI_Controls.uiLog.LOG(this, "Protocol_Selection", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
                    //  (logMSG);
                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                                                                            
                    // get info on quadrant
                    ISeparationProtocol[] SelectedProtocols = RoboSep_UserConsole.getInstance().XML_getUserProtocols();

                    for (int i = 0; i < SelectedProtocols.Length; i++)
                    {
                        if (myQuadrants[Q].QuadrantLabel == SelectedProtocols[i].Label)
                        {
                            RoboSep_UserConsole.showOverlay();

                            // create array of strings to give to num pad for all messages
                            string[] numPadMessages = new string[] {
                                LanguageINI.GetString("NumPadmsg1"),
                                LanguageINI.GetString("NumPadmsg2"),
                                LanguageINI.GetString("NumPadmsg3"),
                                LanguageINI.GetString("NumPadmsg4"),
                                LanguageINI.GetString("Ok"),
                                LanguageINI.GetString("Cancel"),
                                LanguageINI.GetString("Enter"),
                                LanguageINI.GetString("Clear")};
                                

                            // open number pad control
                            NumberPad numPad = new NumberPad(RoboSep_UserConsole.getInstance(), RoboSep_UserConsole.getInstance().frmOverlay,
                                myQuadrants[Q].GetVolumeLabel(), myQuadrants[Q].Quadrant_message,
                                myQuadrants[Q].volMin, myQuadrants[Q].volMax, numPadMessages);
                            numPad.ShowDialog();
                            numPad.Dispose();
                            RoboSep_UserConsole.hideOverlay();

                            // Sunny add 
                            this.BringToFront();
                            
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
            if (myQuadrants[Q].QuadrantUsed)
            {
                RemoveProtocolsInQuadrant(Q);
            }
        }

        public void CancelAllQuadrants()
        {
            for (int Q = 0; Q < 4; Q++)
            {
                CancelQuadrant(Q);
            }
        }

        public void RemoveProtocolsInQuadrant(int Q)
        {
            if (myQuadrants[Q].QuadrantUsed == false)
                return;

            // LOG
            string logMSG = "Removing protocol at quadrant " + (Q + 1).ToString();
            //GUI_Controls.uiLog.LOG(this, "CancelQuadrant", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                                                                            
            // remove from servers chosen protocol list
            iProtocolSelectCount -= 1;
            RoboSep_UserConsole.getInstance().DeselectProtocol((QuadrantId)(Q));

            // remove from sample processing selection
            ResetSampleButtonInQuadrant(Q);
        }

        private void ResetSampleButtonInQuadrant(int Q)
        {
            if (Q < 0 || Q > MaxQuadrantNumberIndex)
                return;

            // remove from sample processing selection
            int repeat = myQuadrants[Q].QuadrantsRequired;
            myQuadrants[Q].Clear();
            for (int i = 0; i < repeat; i++)
            {
                if ((Q + i) > MaxQuadrantNumberIndex)
                    break;

                myQuadrants[Q + i].Clear();
                switch (Q + i)
                {
                    case 0:
                        Q1.Cancelled = false;
                        Q1.Selected = false;
                        Q1.Enabled = false;
                        Q1.Invalidate();
                        break;
                    case 1:
                        Q2.Cancelled = false;
                        Q2.Selected = false;
                        Q2.Enabled = false;
                        Q2.Invalidate();
                        break;
                    case 2:
                        Q3.Cancelled = false;
                        Q3.Selected = false;
                        Q3.Enabled = false;
                        Q3.Invalidate();
                        break;
                    case 3:
                        Q4.Cancelled = false;
                        Q4.Selected = false;
                        Q4.Enabled = false;
                        Q4.Invalidate();
                        break;
                    default:
                        break;
                }
            }
        }

        private void button_CancelSelected_Click(object sender, EventArgs e)
        {
            // Cancel those have been selected
            if (Q1.Cancelled)
            {
                RemoveProtocolsInQuadrant(0);
            }

            if (Q2.Cancelled)
            {
                RemoveProtocolsInQuadrant(1);
            }

            if (Q3.Cancelled)
            {
                RemoveProtocolsInQuadrant(2);
            }

            if (Q4.Cancelled)
            {
                RemoveProtocolsInQuadrant(3);
            }

            // disable Run and hide delete button if there are no active protocols
            UpdateDeleteAndPlayButtonStates();
        }

        private void Q1_Click(object sender, EventArgs e)
        {
            UpdateQuadrantButtonsCancelState(0); 
        }
        private void Q2_Click(object sender, EventArgs e)
        {
            UpdateQuadrantButtonsCancelState(1);
        }

        private void Q3_Click(object sender, EventArgs e)
        {
            UpdateQuadrantButtonsCancelState(2);
        }

        private void Q4_Click(object sender, EventArgs e)
        {
            UpdateQuadrantButtonsCancelState(3);
        }

        private void UpdateQuadrantButtonsCancelState(int startIndex)
        {
            if (startIndex < 0 || startIndex > 3)
                return;

            bool bFirst = false;
            switch (startIndex)
            {
                case 0:
                    Q1.Cancelled = !Q1.Cancelled;
                    bFirst = Q1.Cancelled;
                    Q1.Invalidate();
                    break;
                case 1:
                    Q2.Cancelled = !Q2.Cancelled;
                    bFirst = Q2.Cancelled;
                    Q2.Invalidate();
                    break;
                case 2:
                    Q3.Cancelled = !Q3.Cancelled;
                    bFirst = Q3.Cancelled;
                    Q3.Invalidate();
                    break;
                case 3:
                    Q4.Cancelled = !Q4.Cancelled;
                    bFirst = Q4.Cancelled;
                    Q4.Invalidate();
                    break;
                default:
                    break;
            }

            int repeat = myQuadrants[startIndex].QuadrantsRequired;
            for (int i = 1; i < repeat; i++)
            {
                switch (startIndex + i)
                {
                    case 0:
                        Q1.Cancelled = bFirst;
                        Q1.Invalidate();
                        break;
                    case 1:
                        Q2.Cancelled = bFirst;
                        Q2.Invalidate();
                        break;
                    case 2:
                        Q3.Cancelled = bFirst;
                        Q3.Invalidate();
                        break;
                    case 3:
                        Q4.Cancelled = bFirst;
                        Q4.Invalidate();
                        break;
                    default:
                        break;
                }
            }

            UpdateDeleteAndPlayButtonStates();
        }

        private void button_Tab1_Click(object sender, EventArgs e)
        {
            // check if any protocols are selected
            int[] CurrentQuadrants = RoboSep_RunSamples.getInstance().iSelectedProtocols;
            if (CurrentQuadrants.Length > 0)
            {
                // prompt if user is sure they want to switch users if
                // all protocol selections are removed
                string msg = LanguageINI.GetString("msgChangeUser");
                RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING, msg,
                    LanguageINI.GetString("headerChangeUsr"), LanguageINI.GetString("Ok"), LanguageINI.GetString("Cancel"));
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                RoboSep_UserConsole.hideOverlay();

                if (prompt.DialogResult != DialogResult.OK)
                {
                    prompt.Dispose();
                    return;
                }
                prompt.Dispose();
            }

            // remove all currently selected protocols
            for (int i = 0; i < CurrentQuadrants.Length; i++)
                RoboSep_RunSamples.getInstance().CancelQuadrant(CurrentQuadrants[i]);

            // change to robosep user select window
            RoboSep_UserConsole.getInstance().SuspendLayout();
            RoboSep_UserSelect usrSelect = new RoboSep_UserSelect();
            RoboSep_UserConsole.getInstance().Controls.Add(usrSelect);
            RoboSep_UserConsole.ctrlCurrentUserControl = usrSelect;

            usrSelect.BringToFront();
            usrSelect.Focus();
        }
        
        private void UpdateRemoteDesktopButtons()
        {
            if (RoboSep_UserConsole.getInstance().IsRemoteDestopLock)
            {
                button_RemoteDeskTop.ChangeGraphics(iListLock);
            }
            else
            {
                button_RemoteDeskTop.ChangeGraphics(iListUnlock);
            }
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

                GUI_Controls.RoboMessagePanel prompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_QUESTION,
                    sMsg, sHeader, sButtonText);
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                prompt.Dispose();
                RoboSep_UserConsole.hideOverlay();

                // Lock
                RoboSep_UserConsole.getInstance().BroadcastOrcaTCPServer(false);
                RoboSep_UserConsole.getInstance().IsRemoteDestopLock = true;
            }

            UpdateRemoteDesktopButtons();
        }

    }

    public class ProtocolInfo2 : ProtocolInfo
    {
        public delegate bool AddToRunDelegate(int Q, string protocolName, double vol);
        public delegate void MarkAsCancelledDelegate(int Q);

        private bool bSelectionCompleted;
        public event SeparatorIsReadyDelegate evSeparatorReady = null;
        public RoboSep_RunSamples pRoboSepRunSamples = null;

        public ProtocolInfo2(QuadrantId Q, string protocolName, double Vol, int QuadrantRequired, bool MarkAsCancelled, RoboSep_RunSamples pRoboSepRunSamples)
        : base (Q, protocolName, Vol, QuadrantRequired, MarkAsCancelled)
        {
            this.pRoboSepRunSamples = pRoboSepRunSamples;
            bSelectionCompleted = false;
        }

        public bool SelectionCompleted
        {
            get
            {
                return this.bSelectionCompleted;
            }
            set
            {
                this.bSelectionCompleted = value;
            }
        }

        public void AddToRun()
        {
            if (string.IsNullOrEmpty(ProtocolName))
            {
                System.Diagnostics.Debug.WriteLine(String.Format("AddToRun: Input parameter ProtocolName is null."));
                return;
            }

            bool bRet = true;
            try
            {
                
                if (pRoboSepRunSamples.InvokeRequired)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("AddToRun: initial QuadrantId {0} - restoring protocol: {1} ---------", initialQuadrant, protocolName));
                    pRoboSepRunSamples.Invoke(new AddToRunDelegate(pRoboSepRunSamples.addToRun), (int)initialQuadrant, ProtocolName, SampleVolumeUl);
                }
                else
                {
                    bRet = pRoboSepRunSamples.addToRun((int)initialQuadrant, ProtocolName, SampleVolumeUl);
                }

                if (markAsCancelled == true)
                {
                    for (int i = 0; i < NumOfQuadrantRequired; i++)
                    {
                        if (pRoboSepRunSamples.InvokeRequired)
                        {
                            pRoboSepRunSamples.Invoke(new MarkAsCancelledDelegate(pRoboSepRunSamples.MarkQuadrantAsCancelled), (int)initialQuadrant + i);
                        }
                        else
                        {
                            pRoboSepRunSamples.MarkQuadrantAsCancelled((int)initialQuadrant + i);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // LOG
                LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);               
            }

            if (evSeparatorReady == null)
                evSeparatorReady = new SeparatorIsReadyDelegate(HandleSeparatorIsReadyToAcceptProtocol);

            RoboSep_UserConsole.getInstance().NotifyUserSeparatorIsReadyToAcceptProtocol += evSeparatorReady;
            System.Diagnostics.Debug.WriteLine(String.Format("Exit AddToRun"));
        }

        private void HandleSeparatorIsReadyToAcceptProtocol(object sender, IntegerEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("--------- HandleSeparatorIsReadyToAcceptProtocol is called. ThreadID = {0} ----------", Thread.CurrentThread.ManagedThreadId));

            RoboSep_UserConsole.getInstance().NotifyUserSeparatorIsReadyToAcceptProtocol -= evSeparatorReady;
            bSelectionCompleted = true;

            ProtocolSeparatorInputParam.asyncProtocolSelectionIsDone.Set();
        } 
    }

    public class ProtocolInfoList
    {
        public List<ProtocolInfo2> lstProtocolInfo2 = new List<ProtocolInfo2>();
        public static AutoResetEvent asyncOpsAreDone = new AutoResetEvent(false);
    }


    public class ProtocolSeparatorInputParam
    {
        public delegate void SelectProtocolDelegate(QuadrantId Q, IProtocol protocol);
        public delegate void SetSampleVolumeDelegate(QuadrantId Q, double volMl);

        protected QuadrantId initialQuadrant;
        protected IProtocol protocol = null;
        protected double volMl = -1;
        public RoboSep_RunSamples pRoboSepRunSamples = null;

        public static AutoResetEvent asyncProtocolSelectionIsDone = new AutoResetEvent(false);
        public event ChosenProtocolsFinishedDelegate evChosenProtocolsFinished = null;
 
        public ProtocolSeparatorInputParam(ProtocolSeparatorInputParam protocolInputParam)
        {
            if (protocolInputParam != null)
            {
                this.initialQuadrant = protocolInputParam.initialQuadrant;
                this.protocol = protocolInputParam.protocol;
                this.volMl = protocolInputParam.volMl;
            }
        }

        public ProtocolSeparatorInputParam(QuadrantId initialQuadrant, IProtocol protocol, double volMl,  RoboSep_RunSamples pRoboSepRunSamples)
        {
            this.protocol = protocol;
            this.initialQuadrant = initialQuadrant;
            this.volMl = volMl;
            this.pRoboSepRunSamples = pRoboSepRunSamples;
        }

        public void SepatorSelectProtocol()
        {
            if (evChosenProtocolsFinished == null)
            {
                evChosenProtocolsFinished = new ChosenProtocolsFinishedDelegate(HandleChosenProtocolFinished);
            }

            RoboSep_UserConsole pRoboSepUserConsole = RoboSep_UserConsole.getInstance();

            pRoboSepUserConsole.NotifyUserChosenProtocolsUpdated += evChosenProtocolsFinished;
            if (pRoboSepRunSamples.InvokeRequired)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("ProtocolSeparatorInputParam: SelectProtocols: initial QuadrantId {0}, protocol: {1} ---------", initialQuadrant, protocol.Label));
                pRoboSepRunSamples.Invoke(new SelectProtocolDelegate(pRoboSepRunSamples.SeparatorSelectProtocol), initialQuadrant, protocol);
            }
            else
            {
                pRoboSepRunSamples.SeparatorSelectProtocol(initialQuadrant, protocol);
            }
        }

        public void SetSampleVolume()
        {
            if (pRoboSepRunSamples.InvokeRequired)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("ProtocolSeparatorInputParam : SetSampleVolume: initial QuadrantId {0}, Vol: {1} ---------", initialQuadrant, volMl.ToString()));
                pRoboSepRunSamples.Invoke(new SetSampleVolumeDelegate(pRoboSepRunSamples.SetSampleVolume), initialQuadrant, volMl);
            }
            else
            {
                pRoboSepRunSamples.SetSampleVolume(initialQuadrant, volMl);
            }
        }


        private void HandleChosenProtocolFinished(object sender, ProtocolEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("--------- HandleChosenProtocolFinished is called. ThreadID = {0} ----------", Thread.CurrentThread.ManagedThreadId));

            RoboSep_UserConsole.getInstance().NotifyUserChosenProtocolsUpdated -= evChosenProtocolsFinished;
            ProtocolSeparatorInputParam.asyncProtocolSelectionIsDone.Set();
        }

        public QuadrantId InitialQuadrant
        {
            get
            {
                return this.initialQuadrant;
            }
        }
        public IProtocol Protocol
        {
            get
            {
                return this.protocol;
            }
        }

    }
    public class SetVolumeParam
    {
        public QuadrantId Q;
        public double vol;

        public SetVolumeParam(QuadrantId Q, double vol)
        {
            this.Q = Q;
            this.vol = vol;
        }
    }
       
}

