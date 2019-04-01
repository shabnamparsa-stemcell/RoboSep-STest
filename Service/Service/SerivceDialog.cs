using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;



using Invetech.ApplicationLog;

using Tesla.InstrumentControl;
using Tesla.Common.ResourceManagement;

using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using GUI_Console;

namespace Tesla.Service
{
    
    public partial class ServiceDialog : Form
    {
        private ProcessScript processScript;

        private ReportCommsStateDelegate reportCommsStateDelegate;
        private ReportStatusUpdateDelegate reportStatusUpdateDelegate;

        public delegate void NoArgsDelegate();
        public delegate DialogResult StringArgDelegate(string sMSG);

        private PictureButton ZIcon;
        private PictureButton ThetaIcon;
        private PictureButton TipStripperIcon;
        private PictureButton CarouselIcon;
        private PictureButton PumpIcon;
        private PictureButton BufferIcon;

        private PictureButton HelpTab;

        private LED ZHome;
        private LED ZLimit;
        private LED ThetaHome;
        private LED ThetaLimit;
        private LED CarouselHome;
        private LED CarouselLimit;
        private LED PumpHome;
        private LED PumpLimit;
        private LED TipStripperHome;
        private LED TipStripperLimit;
        private LED BufferLimit;

        private string m_userLevel;
        private bool m_PendantMode;

        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);


        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        private static extern System.IntPtr SendMessage(System.IntPtr hWnd, uint Msg, System.IntPtr wParam, System.IntPtr lParam);
       

        [DllImport("user32.dll")]
        private static extern bool GetScrollInfo(IntPtr hwnd, int fnBar, ref SCROLLINFO lpsi);
        [StructLayout(LayoutKind.Sequential)]
        struct SCROLLINFO
        {
            public int cbSize;
            public uint fMask;
            public int nMin;
            public int nMax;
            public uint nPage;
            public int nPos;
            public int nTrackPos;
        }
        public enum ScrollBarDirection
        {

            SB_HORZ = 0,

            SB_VERT = 1,

            SB_CTL = 2,

            SB_BOTH = 3

        }



        public enum ScrollInfoMask
        {

            SIF_RANGE = 0x1,

            SIF_PAGE = 0x2,

            SIF_POS = 0x4,

            SIF_DISABLENOSCROLL = 0x8,

            SIF_TRACKPOS = 0x10,

            SIF_ALL = SIF_RANGE + SIF_PAGE + SIF_POS + SIF_TRACKPOS

        }
        public ServiceDialog(string userLevel)
        {
            InitializeComponent();

            m_userLevel = userLevel;

            string logMSG = "Creating new ServiceDialog";
            GUI_Controls.uiLog.SERVICE_LOG(this, "ServiceDialog", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

            reportCommsStateDelegate = new ReportCommsStateDelegate(ReportCommsStateHandler);
            reportStatusUpdateDelegate = new ReportStatusUpdateDelegate(ReportStatusUpdateHandler);
            InstrumentComms.ReportCommsState += (reportCommsStateDelegate);
            InstrumentComms.ReportStatusUpdate += (reportStatusUpdateDelegate);
            
            btnConnect.Text = SeparatorResourceManager.GetSeparatorString(StringId.Connect);

            HideCaret(txtScriptStepText.Handle);

            string basename;
            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            try
            {
                basename = "ServiceButton";
                PictureButton serviceMenuButton = RoboSepServiceManualStepControl.makeNewButtonWithParent(this, new Rectangle(3, 45, Service.Properties.Resources.serviceMenu_0.Width, Service.Properties.Resources.serviceMenu_0.Height),
                    new Rectangle(0, 0, Service.Properties.Resources.serviceMenu_0.Width, Service.Properties.Resources.serviceMenu_0.Height),
                    Service.Properties.Resources.serviceMenu_2,
                    Service.Properties.Resources.serviceMenu_1,
                    Service.Properties.Resources.serviceMenu_1,
                Service.Properties.Resources.serviceMenu_2,
                Service.Properties.Resources.serviceMenu_1);
                serviceMenuButton.Parent = this;
                serviceMenuButton.Click += new EventHandler(btnServiceMenu_Click);
                
                basename = "HelpTab";
                HelpTab = RoboSepServiceManualStepControl.makeNewButtonWithParent(this, new Rectangle(348, 0, 63, 60),
                    new Rectangle(0, 0, 63, 60),
                    Service.Properties.Resources.video_0,
                    Service.Properties.Resources.video_2,
                Service.Properties.Resources.video_1,
                Service.Properties.Resources.video_inactive,
                Service.Properties.Resources.video_1);
                HelpTab.Parent = panel2;
                HelpTab.Click += new EventHandler(btnHelpTab_Click);
                HelpTab.SelectMode = false;


                List<Image> ilist = new List<Image>(); ilist.Clear();
                ilist.Add(Service.Properties.Resources.PageUpButton0);
                ilist.Add(Service.Properties.Resources.PageUpButton0);
                ilist.Add(Service.Properties.Resources.PageUpButton0);
                ilist.Add(Service.Properties.Resources.PageUpButton2);
                button_PageUp.disableImage = Service.Properties.Resources.PageUpButtonDisable;
                button_PageUp.ChangeGraphics(ilist);
                
                ilist.Clear();
                ilist.Add(Service.Properties.Resources.PageDownButton0);
                ilist.Add(Service.Properties.Resources.PageDownButton0);
                ilist.Add(Service.Properties.Resources.PageDownButton0);
                ilist.Add(Service.Properties.Resources.PageDownButton2);
                button_PageDown.disableImage = Service.Properties.Resources.PageDownButtonDisable;
                button_PageDown.ChangeGraphics(ilist);

                //serviceMenuButton.BringToFront();

                #region hardware info panel icon
                int hw_info_icon_x = -1;
                int row_height = 55;
                int hw_icon_width = 56;
                int hw_icon_height = 50;

                basename = "Vertaxis";
                ZIcon = RoboSepServiceManualStepControl.makeNewButtonWithParent(this, new Rectangle(hw_info_icon_x, 45, hw_icon_width, hw_icon_height),
                    new Rectangle(0, 0, 45, 44),
                    new Bitmap(directoryName + @"\Resources\" + basename + "2.png"),
                    new Bitmap(directoryName + @"\Resources\" + basename + "2.png"),
                new Bitmap(directoryName + @"\Resources\" + basename + "3.png"),
                new Bitmap(directoryName + @"\Resources\" + basename + "1.png"),
                new Bitmap(directoryName + @"\Resources\" + basename + "3.png"));
                ZIcon.Parent = panel1;

                basename = "AxisTheta";
                ThetaIcon = RoboSepServiceManualStepControl.makeNewButtonWithParent(this, new Rectangle(hw_info_icon_x, 45 + row_height * 1, hw_icon_width, hw_icon_height),
                    new Rectangle(0, 0, 45, 44),
                    new Bitmap(directoryName + @"\Resources\" + basename + "2.png"),
                    new Bitmap(directoryName + @"\Resources\" + basename + "2.png"),
                new Bitmap(directoryName + @"\Resources\" + basename + "3.png"),
                new Bitmap(directoryName + @"\Resources\" + basename + "1.png"),
                new Bitmap(directoryName + @"\Resources\" + basename + "3.png"));
                ThetaIcon.Parent = panel1;

                basename = "AxisCarousel";
                CarouselIcon = RoboSepServiceManualStepControl.makeNewButtonWithParent(this, new Rectangle(hw_info_icon_x, 45 + row_height * 2, hw_icon_width, hw_icon_height),
                   new Rectangle(0, 0, 45, 44),
                   new Bitmap(directoryName + @"\Resources\" + basename + "2.png"),
                   new Bitmap(directoryName + @"\Resources\" + basename + "2.png"),
               new Bitmap(directoryName + @"\Resources\" + basename + "3.png"),
               new Bitmap(directoryName + @"\Resources\" + basename + "1.png"),
               new Bitmap(directoryName + @"\Resources\" + basename + "3.png"));
                CarouselIcon.Parent = panel1;

                basename = "AxisStrip";
                TipStripperIcon = RoboSepServiceManualStepControl.makeNewButtonWithParent(this, new Rectangle(hw_info_icon_x, 45 + row_height * 4, hw_icon_width, hw_icon_height),
                    new Rectangle(0, 0, 45, 44),
                    new Bitmap(directoryName + @"\Resources\" + basename + "2.png"),
                    new Bitmap(directoryName + @"\Resources\" + basename + "2.png"),
                new Bitmap(directoryName + @"\Resources\" + basename + "3.png"),
                new Bitmap(directoryName + @"\Resources\" + basename + "1.png"),
                new Bitmap(directoryName + @"\Resources\" + basename + "3.png"));
                TipStripperIcon.Parent = panel1;

                basename = "AxisPump";
                PumpIcon = RoboSepServiceManualStepControl.makeNewButtonWithParent(this, new Rectangle(hw_info_icon_x, 45 + row_height * 3, hw_icon_width, hw_icon_height),
                   new Rectangle(0, 0, 45, 44),
                   new Bitmap(directoryName + @"\Resources\" + basename + "2.png"),
                   new Bitmap(directoryName + @"\Resources\" + basename + "2.png"),
               new Bitmap(directoryName + @"\Resources\" + basename + "3.png"),
               new Bitmap(directoryName + @"\Resources\" + basename + "1.png"),
               new Bitmap(directoryName + @"\Resources\" + basename + "3.png"));
                PumpIcon.Parent = panel1;

                basename = "AxisBuffer";
                BufferIcon = RoboSepServiceManualStepControl.makeNewButtonWithParent(this, new Rectangle(hw_info_icon_x, 45 + row_height * 5, hw_icon_width, hw_icon_height),
                   new Rectangle(0, 0, 45, 44),
                   new Bitmap(directoryName + @"\Resources\" + basename + "2.png"),
                   new Bitmap(directoryName + @"\Resources\" + basename + "2.png"),
               new Bitmap(directoryName + @"\Resources\" + basename + "3.png"),
               new Bitmap(directoryName + @"\Resources\" + basename + "1.png"),
               new Bitmap(directoryName + @"\Resources\" + basename + "3.png"));
                BufferIcon.Parent = panel1;



                #endregion

                #region hardware info panel labels
                int position_y_offset = 13;
                int position_x_offset = 53;
                lblZPosition.Top = ZIcon.Top + position_y_offset;
                lblZPosition.Left = ZIcon.Left + position_x_offset;
                lblThetaPosition.Top = ThetaIcon.Top + position_y_offset;
                lblThetaPosition.Left = ThetaIcon.Left + position_x_offset;
                lblCarouselPosition.Top = CarouselIcon.Top + position_y_offset;
                lblCarouselPosition.Left = CarouselIcon.Left + position_x_offset;
                lblTipStripper.Top = TipStripperIcon.Top + position_y_offset;
                lblTipStripper.Left = TipStripperIcon.Left + position_x_offset;
                lblPumpVolume.Top = PumpIcon.Top + position_y_offset;
                lblPumpVolume.Left = PumpIcon.Left + position_x_offset;
                lblBufferPosition.Top = BufferIcon.Top + position_y_offset;
                lblBufferPosition.Left = BufferIcon.Left + position_x_offset;
                int On_y_offset = 28;// 5;
                int Home_y_offset = 5;// 28;
                int On_x_offset = 123;
                lblZOn.Top = ZIcon.Top + On_y_offset;
                lblZOn.Left = ZIcon.Left + On_x_offset;
                lblZHome.Top = ZIcon.Top + Home_y_offset;
                lblZHome.Left = ZIcon.Left + On_x_offset;
                lblThetaOn.Top = ThetaIcon.Top + On_y_offset;
                lblThetaOn.Left = ThetaIcon.Left + On_x_offset;
                lblThetaHome.Top = ThetaIcon.Top + Home_y_offset;
                lblThetaHome.Left = ThetaIcon.Left + On_x_offset;
                lblCarouselOn.Top = CarouselIcon.Top + On_y_offset;
                lblCarouselOn.Left = CarouselIcon.Left + On_x_offset;
                lblCarouselHome.Top = CarouselIcon.Top + Home_y_offset;
                lblCarouselHome.Left = CarouselIcon.Left + On_x_offset;
                lblTipStripperLimit.Top = TipStripperIcon.Top + On_y_offset;
                lblTipStripperLimit.Left = TipStripperIcon.Left + On_x_offset;
                lblTipStripperHome.Top = TipStripperIcon.Top + Home_y_offset;
                lblTipStripperHome.Left = TipStripperIcon.Left + On_x_offset;
                lblPumpOn.Top = PumpIcon.Top + On_y_offset;
                lblPumpOn.Left = PumpIcon.Left + On_x_offset;
                lblPumpHome.Top = PumpIcon.Top + Home_y_offset;
                lblPumpHome.Left = PumpIcon.Left + On_x_offset;
                BufferLimit = new LED();
                BufferLimit.Location = new Point(BufferIcon.Left + On_x_offset+5, BufferIcon.Top + 8);
                BufferLimit.Parent = panel1;


                #endregion

                #region hardware info panel led
                int LED_1_y_offset = -3;// 0;
                int LED_2_y_offset = 20;// 25;
                int LED_x_offset = 147;
                ZHome = new LED();
                ZHome.Location = new Point(ZIcon.Left + LED_x_offset, ZIcon.Top + LED_1_y_offset);
                ZHome.Parent = panel1;
                ZLimit = new LED();
                ZLimit.Location = new Point(ZIcon.Left + LED_x_offset, ZIcon.Top + LED_2_y_offset);
                ZLimit.Parent = panel1;
                ThetaHome = new LED();
                ThetaHome.Location = new Point(ThetaIcon.Left + LED_x_offset, ThetaIcon.Top + LED_1_y_offset);
                ThetaHome.Parent = panel1;
                ThetaLimit = new LED();
                ThetaLimit.Location = new Point(ThetaIcon.Left + LED_x_offset, ThetaIcon.Top + LED_2_y_offset);
                ThetaLimit.Parent = panel1;
                CarouselHome = new LED();
                CarouselHome.Location = new Point(CarouselIcon.Left + LED_x_offset, CarouselIcon.Top + LED_1_y_offset);
                CarouselHome.Parent = panel1;
                CarouselLimit = new LED();
                CarouselLimit.Location = new Point(CarouselIcon.Left + LED_x_offset, CarouselIcon.Top + LED_2_y_offset);
                CarouselLimit.Parent = panel1;
                PumpHome = new LED();
                PumpHome.Location = new Point(PumpIcon.Left + LED_x_offset, PumpIcon.Top + LED_1_y_offset);
                PumpHome.Parent = panel1;
                PumpLimit = new LED();
                PumpLimit.Location = new Point(PumpIcon.Left + LED_x_offset, PumpIcon.Top + LED_2_y_offset);
                PumpLimit.Parent = panel1;
                TipStripperHome = new LED();
                TipStripperHome.Location = new Point(TipStripperIcon.Left + LED_x_offset, TipStripperIcon.Top + LED_1_y_offset);
                TipStripperHome.Parent = panel1;
                TipStripperLimit = new LED();
                TipStripperLimit.Location = new Point(TipStripperIcon.Left + LED_x_offset, TipStripperIcon.Top + LED_2_y_offset);
                TipStripperLimit.Parent = panel1;
                #endregion

                #region layers tweak
                lblThetaOn.SendToBack();
                lblThetaHome.SendToBack();
                lblZOn.SendToBack();
                lblZHome.SendToBack();
                lblCarouselOn.SendToBack();
                lblCarouselHome.SendToBack();
                lblTipStripperHome.SendToBack();
                lblTipStripperLimit.SendToBack();
                lblPumpOn.SendToBack();
                lblPumpHome.SendToBack();
                #endregion
            }
            catch
            {
            }

            panel1.Enabled = false;

            button_PageUp_Update(false);
            button_PageDown_Update(false);

            update_video_button();

            m_PendantMode = false;
        }
        ~ServiceDialog()
        {
            InstrumentComms.ReportCommsState -= (reportCommsStateDelegate);
            InstrumentComms.ReportStatusUpdate -= (reportStatusUpdateDelegate); 
        }


        //NOT USED
        private void btnConnect_Click(object sender, EventArgs e)
        {
            //connect/disconnect

            switch (InstrumentComms.getInstance().commState)
            {
                case (InstrumentComms.CommsStates.CONNECTED): InstrumentComms.getInstance().disconnectFromInstrument(); break;
                case (InstrumentComms.CommsStates.DISCONNECTED): InstrumentComms.getInstance().connectToInstrument(txtIpAddr.Text); break;
                case (InstrumentComms.CommsStates.COMMS_ERROR): InstrumentComms.getInstance().disconnectFromInstrument(); break;
                default: break;
            }
        }

        #region Event handlers
        public void ReportCommsStateHandler(string commsState)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    ReportCommsStateDelegate eh = new ReportCommsStateDelegate(this.ReportCommsStateHandler);
                    this.Invoke(eh, new object[] { commsState });
                }
                if (InstrumentComms.getInstance().commState == InstrumentComms.CommsStates.CONNECTED)
                {
                    btnConnect.Text = SeparatorResourceManager.GetSeparatorString(StringId.Disconnect);//"Disconnect";
                    panel1.Enabled = true;
                }
                else
                {
                    btnConnect.Text = SeparatorResourceManager.GetSeparatorString(StringId.Connect); //"Connect";
                    panel1.Enabled = false;
                    this.lblScriptRunning.Text = "Script: N/A";
                    ctlManualStep.Enabled = false;
                    txtScriptStepText.Text = "";
                    HelpTab.Enabled = false;
                    lblVideo.Enabled = false;
                }
            }

            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
        }

        public void ReportStatusUpdateHandler(string obj, string method, string arguments, string returnString)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    ReportStatusUpdateDelegate eh = new ReportStatusUpdateDelegate(this.ReportStatusUpdateHandler);
                    this.Invoke(eh, new object[] { obj, method, arguments, returnString });
                }
                if (InstrumentComms.getInstance().commState == InstrumentComms.CommsStates.CONNECTED)
                {
                    //just update all machine vars
                    lblThetaPosition.Text = InstrumentComms.getInstance().thetaAxisPosition + " " + InstrumentComms.UNITS_DEG;
                    lblZPosition.Text = InstrumentComms.getInstance().zAxisPosition + " " + InstrumentComms.UNITS_MM;
                    lblPumpVolume.Text = InstrumentComms.getInstance().pumpVolume + " " + InstrumentComms.UNITS_UL;
                    //lblTipStripperHome.Text = InstrumentComms.getInstance().tipStripperHome;
                    //lblTipStripperLimit.Text = InstrumentComms.getInstance().tipStripperLimit;
                    lblCarouselPosition.Text = InstrumentComms.getInstance().carouselPosition + " " + InstrumentComms.UNITS_DEG;
                }
            }

            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
        }
        #endregion

        #region instrument axis status key constants
        //GetInstrumentAxisStatusSet
        private static string Z_HEIGHT_KEY = "z_height";
        private static string Z_HOME_KEY = "z_home";
        private static string Z_LIMIT_KEY = "z_limit";
        private static string THETA_DEG_KEY = "theta_deg";
        private static string THETA_HOME_KEY = "theta_home";
        private static string THETA_LIMIT_KEY = "theta_limit";
        private static string CAROUSEL_DEG_KEY = "carousel_deg";
        private static string CAROUSEL_HOME_KEY = "carousel_home";
        private static string CAROUSEL_LIMIT_KEY = "carousel_limit";
        private static string PUMP_VOL_KEY = "pump_vol";
        private static string PUMP_HOME_KEY = "pump_home";
        private static string PUMP_LIMIT_KEY = "pump_limit";
        private static string STRIPPER_ENGAGED_KEY = "stripper_eng";
        private static string STRIPPER_EXT_KEY = "stripper_ext";
        private static string STRIPPER_RET_KEY = "stripper_ret";
        private static string LOW_BUFFER_LVL_KEY = "low_buffer_lvl";
        private static string ON = "ON";
        private static string OFF = "OFF";
        public void UpdateHardwareInfo(Dictionary<string, string> dictionary)
        {
            if (dictionary == null) return;

            InstrumentComms.getInstance().updateAxisInfo(dictionary);
            lblThetaPosition.Text = InstrumentComms.getInstance().thetaAxisPosition + " " + InstrumentComms.UNITS_DEG;
            lblZPosition.Text = InstrumentComms.getInstance().zAxisPosition + " " + InstrumentComms.UNITS_MM;
            lblPumpVolume.Text = InstrumentComms.getInstance().pumpVolume + " " + InstrumentComms.UNITS_UL;
            lblCarouselPosition.Text = InstrumentComms.getInstance().carouselPosition + " " + InstrumentComms.UNITS_DEG;
            lblBufferPosition.Text = InstrumentComms.getInstance().bufferLevel;
            lblTipStripper.Text = InstrumentComms.getInstance().tipstripper;

            UpdateLED(dictionary, Z_HOME_KEY, ZHome, false);
            UpdateLED(dictionary, Z_LIMIT_KEY, ZLimit, false);
            UpdateLED(dictionary, THETA_HOME_KEY, ThetaHome, false);
            UpdateLED(dictionary, THETA_LIMIT_KEY, ThetaLimit, false);
            UpdateLED(dictionary, CAROUSEL_HOME_KEY, CarouselHome, false);
            UpdateLED(dictionary, CAROUSEL_LIMIT_KEY, CarouselLimit, false);
            UpdateLED(dictionary, PUMP_HOME_KEY, PumpHome, false);
            UpdateLED(dictionary, PUMP_LIMIT_KEY, PumpLimit, false);
            UpdateLED(dictionary, STRIPPER_EXT_KEY, TipStripperHome, false);
            UpdateLED(dictionary, STRIPPER_RET_KEY, TipStripperLimit, false);
            UpdateLED(dictionary, LOW_BUFFER_LVL_KEY, BufferLimit, true);
        }

        private void UpdateLED(Dictionary<string, string> dictionary, string key, LED led,Boolean isLowBuffer)
        {
            if (dictionary.ContainsKey(key))
            {
                if (isLowBuffer)
                {
                    led.Mode = dictionary[key] == ON || dictionary[key] == "True" || dictionary[key] == "1" ?
                         LED.LED_MODE.Yellow : LED.LED_MODE.Green;
                }
                else
                {
                    led.Mode = dictionary[key] == ON || dictionary[key] == "True" || dictionary[key] == "1" ?
                        LED.LED_MODE.Green : LED.LED_MODE.Black;
                }
            }
        }
        #endregion
        private static int machineStatusUpdateCounter = 1;
        private void timerMachineStateUpdate_Tick(object sender, EventArgs e)
        {
            //don't do it on automated step either
            if (processScript != null && processScript.IsFinsihed) { processScript = null; }
            if (processScript != null && processScript.isOnAutomatedStep()) { return; }

            if (InstrumentComms.getInstance().commState == InstrumentComms.CommsStates.CONNECTED)
            {
                //InstrumentComms.getInstance().getThetaAxisState();
                //InstrumentComms.getInstance().getCarouselState();
                //InstrumentComms.getInstance().getZAxisState();
                //InstrumentComms.getInstance().getPumpState();
                //InstrumentComms.getInstance().getTipStripperState();

                //GUI_Controls.uiLog.SERVICE_LOG(this, "timerMachineStateUpdate_Tick", GUI_Controls.uiLog.LogLevel.EVENTS, 
                //    "machineStatusUpdateCounter start "+machineStatusUpdateCounter.ToString());//"FULLSET without stripper start");

                UpdateHardwareInfo(InstrumentComms.getInstance().GetInstrumentAxisStatusSet(machineStatusUpdateCounter));

                //GUI_Controls.uiLog.SERVICE_LOG(this, "timerMachineStateUpdate_Tick", GUI_Controls.uiLog.LogLevel.EVENTS,
                //    "machineStatusUpdateCounter end " + machineStatusUpdateCounter.ToString());// "FULLSET without stripper end");

                machineStatusUpdateCounter++;
                if (machineStatusUpdateCounter > 6) machineStatusUpdateCounter = 1;
            }
        }


        private void btnHelpTab_Click(object sender, EventArgs e)
        {
            if (video_path == null || !File.Exists(video_path))
            {
                ShowMessage("There is no help video for this step.");
            }
            else
            {
                System.Diagnostics.Process.Start("wmplayer.exe", "\""+video_path+"\"");
                
            }
        }

        public void ShowMenu()
        {
            if (this.InvokeRequired)
            {
                NoArgsDelegate eh = new NoArgsDelegate(this.ShowMenu);
                this.Invoke(eh, new object[] { });
            }
            else
            {
                btnServiceMenu_Click(this, null);
            }
        }

        private string lastScriptRun;
        private void btnServiceMenu_Click(object sender, EventArgs e)
        {

            string logMSG = "clicked";
            GUI_Controls.uiLog.SERVICE_LOG(this, "btnServiceMenu_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

            /*
            Dictionary<string,string> results = InstrumentComms.getInstance().GetInstrumentAxisStatusSet(1);
            if (results != null)
            {
                foreach (KeyValuePair<String, String> entry in results)
                {
                    Console.WriteLine(entry);
                }
            }
             */



            // Show window overlay
            RoboSep_UserConsole.getInstance().frmOverlay.Show();
            RoboSep_UserConsole.getInstance().frmOverlay.BringToFront();

            ServiceMenu menu = new ServiceMenu(m_userLevel, lastScriptRun, m_PendantMode);
            menu.ShowDialog();
            m_PendantMode = menu.PendantMode;
            ctlManualStep.SetPendantMode(m_PendantMode);
            ctlManualStep.servDlg = this;

            RoboSep_UserConsole.getInstance().frmOverlay.Hide();

            //lastScriptRun = "";
            if (menu.StartScript != null)
            {
                lblScriptRunning.Text = menu.StartScript.name; //"Script: " +

                //running new script, do init
                ctlManualStep.setInitialAxis();
                processScript = new ProcessScript(menu.StartScript, txtScriptStepText, ctlManualStep, this);
                ctlManualStep.ProcessScript = processScript;
                ctlManualStep.servDlg = this;

                logMSG = "Running script: " + menu.StartScript.name;
                GUI_Controls.uiLog.SERVICE_LOG(this, "btnServiceMenu_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

                lastScriptRun = menu.StartScript.name;

                processScript.accept(); //initialize to step 1;
            }
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            InstrumentComms.getInstance().disconnectFromInstrument();
            Thread oThread = new Thread(new ThreadStart(waitAndQuit));
            oThread.IsBackground = true;

            // Start the thread
            oThread.Start();
        }
        private void waitAndQuit()
        {
            Thread.Sleep(3000);
            Application.Exit();

        }


        internal void SetScriptDoneText()
        {
            if (this.InvokeRequired)
            {
                NoArgsDelegate eh = new NoArgsDelegate(this.SetScriptDoneText);
                this.Invoke(eh, new object[] {});
            }
            else
            {
                lblScriptRunning.Text = "Script: N/A";
                string logMSG = "reset text";
                GUI_Controls.uiLog.SERVICE_LOG(this, "SetScriptDoneText", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);


            }
        }

        public int lineNumScripText = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            int position = 0; lineNumScripText++;
            for (int i = 0; i < lineNumScripText; i++)
            {
                if (lineNumScripText < txtScriptStepText.Lines.Length)
                  position += txtScriptStepText.Lines[i].Length;
            }

            txtScriptStepText.SelectionStart = position;

            txtScriptStepText.ScrollToCaret();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            int position = 0; lineNumScripText--;
            for (int i = 0; i < lineNumScripText; i++)
            {
                if (lineNumScripText < txtScriptStepText.Lines.Length)
                    position += txtScriptStepText.Lines[i].Length;
            }

            txtScriptStepText.SelectionStart = position;

            txtScriptStepText.ScrollToCaret();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        internal DialogResult ShowMessageWithAbort(string sMSG)
        {
            DialogResult result = DialogResult.OK;
            if (this.InvokeRequired)
            {
                StringArgDelegate eh = new StringArgDelegate(this.ShowMessage);
                this.Invoke(eh, new object[] { sMSG });
            }
            else
            {
                GUI_Controls.RoboMessagePanel newPrompt = new GUI_Controls.RoboMessagePanel(this, sMSG, "", "Start", "Abort");
                RoboSep_UserConsole.getInstance().frmOverlay.Show();
                result = newPrompt.ShowDialog();
                RoboSep_UserConsole.getInstance().frmOverlay.Hide();
            }
            return result;
        }

        internal DialogResult ShowMessage(string sMSG)
        {
            DialogResult result = DialogResult.OK;
            if (this.InvokeRequired)
            {
                StringArgDelegate eh = new StringArgDelegate(this.ShowMessage);
                this.Invoke(eh, new object[] { sMSG });
            }
            else
            {
                GUI_Controls.RoboMessagePanel newPrompt = new GUI_Controls.RoboMessagePanel(this, sMSG);
                RoboSep_UserConsole.getInstance().frmOverlay.Show();
                result = newPrompt.ShowDialog();
                RoboSep_UserConsole.getInstance().frmOverlay.Hide();
            }
            return result;
        }

        private void button_PageUp_Click(object sender, EventArgs e)
        {
            const uint WM_VSCROLL = 0x115;
            const int SB_PAGEUP = 2;
            System.IntPtr handle = txtScriptStepText.Handle;
            System.IntPtr wParam = (System.IntPtr)SB_PAGEUP;
            System.IntPtr lParam = System.IntPtr.Zero;
            SendMessage(handle, WM_VSCROLL, wParam, lParam);
            button_PageUp_Update(true);
            button_PageDown.Enabled = true;
        }

        private void button_PageDown_Click(object sender, EventArgs e)
        {
            const uint WM_VSCROLL = 0x115;
            const int SB_PAGEDOWN = 3;
            System.IntPtr handle = txtScriptStepText.Handle;
            System.IntPtr wParam = (System.IntPtr)SB_PAGEDOWN;
            System.IntPtr lParam = System.IntPtr.Zero;
            SendMessage(handle, WM_VSCROLL, wParam, lParam);
            button_PageDown_Update(true);
            button_PageUp.Enabled = true;
        }

        private void txtScriptStepText_TextChanged(object sender, EventArgs e)
        {
            button_PageUp_Update(false);
            button_PageDown_Update(false);
        }

        private void button_PageUp_Update(bool boExtra)
        {
    
            System.IntPtr handle = txtScriptStepText.Handle;
            const int SB_VERT = 1;
            SCROLLINFO si = new SCROLLINFO();
            si.cbSize = Marshal.SizeOf(si);
            si.fMask = (uint)ScrollInfoMask.SIF_ALL;
            GetScrollInfo(handle, SB_VERT, ref si);
            
            button_PageUp.Enabled = si.nPos > si.nMin;
        }
        private void button_PageDown_Update(bool boExtra)
        {

            System.IntPtr handle = txtScriptStepText.Handle;
            const int SB_VERT = 1;
            SCROLLINFO si = new SCROLLINFO();
            si.cbSize = Marshal.SizeOf(si);
            si.fMask = (uint)ScrollInfoMask.SIF_ALL;
            GetScrollInfo(handle, SB_VERT, ref si);
            
            button_PageDown.Enabled = si.nPos + si.nPage <= si.nMax;
        }

        public string video_path;
        public void update_video_button()
        {
            bool isEnabled = false;
            if (video_path != null && File.Exists(video_path))
            {
                isEnabled = true;
            }
            lblVideo.Enabled = isEnabled;
            HelpTab.Enabled = isEnabled;
        }

        private void ServiceDialog_Shown(object sender, EventArgs e)
        {
            lastScriptRun = "";
            btnServiceMenu_Click(sender, e);
        }


        public string barcode_app_path;
    }
}
