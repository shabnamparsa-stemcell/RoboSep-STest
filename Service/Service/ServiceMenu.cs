using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Invetech.ApplicationLog;

using Tesla.InstrumentControl;
using Tesla.Common.ResourceManagement;

using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Threading;
using System.Reflection;
using GUI_Console;
using GUI_Controls;
using System.Configuration;

namespace Tesla.Service
{
    public partial class ServiceMenu : Form
    {
        public static string MENU_SERVICE_LOGS = "Service Logs";
        public static string MENU_SETTINGS = "Settings";
        public static string CALIBRATION = "Calibration";
        public static string TEACHING = "Teaching";
        public static string ABOUT = "About";
        public static string EXIT = "Exit";

        private ReportCommsStateDelegate reportCommsStateDelegate;
        private ReportStatusUpdateDelegate reportStatusUpdateDelegate;
        private script startScript = null;
        private Bitmap imgMainSelection;
        private Panel[] panels;
        private string[] panelNames;

        private string m_userLevel;
        private script[] all_scripts;
        private int num_scripts;
        private static int MAX_SCRIPTS = 100;

        private Brush brushPressed;
        private Brush brushSubActiveText;
        private Brush brushSubDisabledText;
        private Brush brushMenuSelectBG;


        private static int MENU_HEIGHT = 45;

        public bool PendantMode;

        #region program file helpers
        public static string ProgramFiles()
        {

            string programFiles = Environment.GetEnvironmentVariable("ProgramFiles(x86)");

            if (programFiles == null)
            {

                programFiles = Environment.GetEnvironmentVariable("ProgramFiles");

            }



            return programFiles;

        }



        public static int getOsInfo()
        {

            int DataBit = -1;

            try
            {

                string programeFiles = ProgramFiles();

                if (programeFiles.Contains("x86"))
                {

                    DataBit = 64;

                }

                else
                {

                    DataBit = 32;

                }

            }

            catch (Exception)
            {

                return 32;

            }

            return DataBit;

        }



        public bool Is64bitOS
        {

            get { return (Environment.GetEnvironmentVariable("ProgramFiles(x86)") != null); }

        }

 
        #endregion

        public ServiceMenu(string userLevel, string lastScriptRun, bool pendantMode)
        {
            InitializeComponent();

            this.openFileDialog1.InitialDirectory = ProgramFiles()+"\\STI\\RoboSep\\config\\hwbackup";

            m_userLevel = userLevel;
            PendantMode = pendantMode;

            // LOG
            string logMSG = "Creating new ServiceMenu";
            GUI_Controls.uiLog.SERVICE_LOG(this, "ServiceMenu", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

            this.SuspendLayout();
            List<Image> IList = new List<Image>();
            IList.Add(GUI_Controls.Properties.Resources.Button_CANCEL0);
            IList.Add(GUI_Controls.Properties.Resources.Button_CANCEL0);
            IList.Add(GUI_Controls.Properties.Resources.Button_CANCEL0);
            IList.Add(GUI_Controls.Properties.Resources.Button_CANCEL1);
            btn_close.ChangeGraphics(IList);

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

            reportCommsStateDelegate = new ReportCommsStateDelegate(ReportCommsStateHandler);
            reportStatusUpdateDelegate = new ReportStatusUpdateDelegate(ReportStatusUpdateHandler);
            InstrumentComms.ReportCommsState += (reportCommsStateDelegate);
            InstrumentComms.ReportStatusUpdate += (reportStatusUpdateDelegate);

            panelNames = new string[] { CALIBRATION, TEACHING, MENU_SETTINGS, MENU_SERVICE_LOGS, ABOUT, EXIT };
            panels = new Panel[] { pnlCalibrate, pnlTeaching, pnlSettings, pnlServiceLog, pnlExit, pnlExit };

            
            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            imgMainSelection = new Bitmap(directoryName + @"\Resources\MainMenuSelection.png");

            string basename;
            try
            {
                /*
                basename = "ConnectButton";
                PictureButton connnectButton = RoboSepServiceManualStepControl.makeNewButtonWithParent(this, new Rectangle(lblConnect.Left + lblConnect.Width, lblConnect.Top-3, 37, 37),
                    new Rectangle(0, 0, 37, 37),
                    new Bitmap(directoryName + @"\Resources\" + basename + "0.bmp"),
                    new Bitmap(directoryName + @"\Resources\" + basename + "1.bmp"),
                new Bitmap(directoryName + @"\Resources\" + basename + "0.bmp"),
                new Bitmap(directoryName + @"\Resources\" + basename + "0.bmp"),
                new Bitmap(directoryName + @"\Resources\" + basename + "1.bmp"));
                connnectButton.Parent = this;
                connnectButton.Click += new EventHandler(btnConnect_Click);
                */

                //btn_close.Bounds = new Rectangle(connnectButton.Left+connnectButton.Width+7,connnectButton.Top+4, 37, 37);
            }
            catch
            {
            }
            //listboxMenu.ItemHeight = 66;
            //listboxMenu.Height = 254;
            //listboxSubMenu.Parent = listboxMenu;

            //if (myConfig == null)
            {
                LoadServiceConfig();
            }

            int lastScriptListboxIndex = 0;
            int lastScriptIndex = 0;
            all_scripts = new script[MAX_SCRIPTS];
            num_scripts=0;

            listboxSubMenu.Items.Clear();
            listboxTeachingSubMenu.Items.Clear();
            
            if (m_userLevel == ServiceLogin.SUPER_USER)
            {
                foreach (script s in myConfig.superScripts.script)
                {
                    int new_count = listboxSubMenu.Items.Count + 1;
                    s.name = new_count.ToString() + ". " + s.name;
                    if (s.name == lastScriptRun)
                    {
                        lastScriptListboxIndex = 0;
                        lastScriptIndex = listboxSubMenu.Items.Count;
                    }
                    all_scripts[num_scripts++]=s;
                    listboxSubMenu.Items.Add(s.name);
                }
            }

            if (m_userLevel == ServiceLogin.SUPER_USER || m_userLevel == ServiceLogin.MAINTENANCE_USER)
            {
                foreach (script s in myConfig.maintenanceScripts.script)
                {
                    int new_count = listboxTeachingSubMenu.Items.Count + 1;
                    s.name = new_count.ToString() + ". " + s.name;
                    if (s.name == lastScriptRun)
                    {
                        lastScriptListboxIndex = 1;
                        lastScriptIndex = listboxTeachingSubMenu.Items.Count;
                    }
                    all_scripts[num_scripts++] = s;
                    listboxTeachingSubMenu.Items.Add(s.name);
                }
            }

            foreach (script s in myConfig.userScripts.script)
            {
                int new_count = listboxTeachingSubMenu.Items.Count + 1;
                s.name = new_count.ToString() + ". " + s.name;
                if (s.name == lastScriptRun)
                {
                    lastScriptListboxIndex = 1;
                    lastScriptIndex = listboxTeachingSubMenu.Items.Count;
                }
                all_scripts[num_scripts++] = s;
                listboxTeachingSubMenu.Items.Add(s.name);
            }

            listboxMenu.Items.Clear();
            listboxMenu.Items.AddRange(panelNames);
            if (m_userLevel != ServiceLogin.SUPER_USER)
            {
                listboxMenu.Items.Remove(CALIBRATION);
            }

            //can only be set after myconfig
            listboxMenu.SelectedIndex = 0;


            //if just finished a script, bring back and highlight last script
            if (lastScriptRun != "")
            {
                listboxMenu.SelectedIndex = (m_userLevel != ServiceLogin.SUPER_USER) ? lastScriptListboxIndex - 1 : lastScriptListboxIndex;
                if (lastScriptListboxIndex == 0)
                {
                    listboxSubMenu.SelectedIndex = lastScriptIndex;
                }
                else
                {
                    listboxTeachingSubMenu.SelectedIndex = lastScriptIndex;
                }
            } 
            this.listboxSubMenu.SelectedIndexChanged += new System.EventHandler(this.listboxSubMenu_SelectedIndexChanged);
            this.listboxTeachingSubMenu.SelectedIndexChanged += new System.EventHandler(this.listboxTeachingSubMenu_SelectedIndexChanged);
            
            //load files in log folder
            string service_log_path = GUI_Controls.uiLog.GET_SERVICE_LOG_DIR();
            string[] filePaths = Directory.GetFiles(Path.GetDirectoryName(service_log_path));
            listServiceLogs.Items.Clear();
            foreach (string p in filePaths)
            {
                if (p.EndsWith("Service.log"))
                {
                    listServiceLogs.Items.Add(p);
                }
            }


            //get logging level
            GUI_Controls.uiLog.SET_SERVICE_LOG_OUTPUT_LEVEL(myConfig.LoggingLevel=="Normal"?
                GUI_Controls.uiLog.LogLevel.EVENTS:uiLog.LogLevel.DEBUG );

            //get server addr from separator.exe.config
            XmlDocument doc = new XmlDocument();
            string ip = "127.0.0.1";
            try
            {
                doc.Load(ProgramFiles() + @"\STI\RoboSep\bin\Separator.exe.config");
                ip = doc.DocumentElement.SelectSingleNode("/configuration/Separator/InstrumentControlConnection/add[@key='ServerAddress']").Attributes["value"].Value;
            }
            catch
            {

            }
            txtIpAddr.Text = ip;

           
            brushPressed = new SolidBrush(Color.FromArgb(146, 108, 175));
            brushSubActiveText = new SolidBrush(Color.White);
            brushSubDisabledText = new SolidBrush(Color.FromArgb(95, 96, 98));
            brushMenuSelectBG = new SolidBrush(Color.FromArgb(78, 38, 131));

            updatePanelsEnabled();

            lblVersion.Text = "Version:     "+ServiceLogin.VERSION;


            button_BarcodeRescan.Visible = true;
            button_RestoreBackup.Visible = true;
            button_RestoreFactory.Visible = true;
            button_SaveAsFactory.Visible = true;

            if (m_userLevel == ServiceLogin.MAINTENANCE_USER)
            {
                //button_RestoreFactory.Visible = false;
                button_SaveAsFactory.Visible = false;
            }
            else if (m_userLevel == ServiceLogin.STANDARD_USER)
            {
                button_BarcodeRescan.Visible = false;
                //button_RestoreBackup.Visible = false;
                //button_RestoreFactory.Visible = false;
                button_SaveAsFactory.Visible = false;
            }

            update_pendant_text();
            if (m_userLevel == ServiceLogin.SUPER_USER)
            {
                lblPendant.Visible = true;
                button_Pendant.Visible = true;
            }
            else
            {
                lblPendant.Visible = false;
                button_Pendant.Visible = false;
            }

            this.ResumeLayout(false);
        }
        ~ServiceMenu()
        {
            InstrumentComms.ReportCommsState -= (reportCommsStateDelegate);
            InstrumentComms.ReportStatusUpdate -= (reportStatusUpdateDelegate);

        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            //DialogResult res = MessageBox.Show("Are you sure you want to exit the program?", "Warning", MessageBoxButtons.YesNo);
            /*
            string sMSG = "Are you sure you want to exit the program?";
            RoboMessagePanel prompt = new RoboMessagePanel(this, sMSG, "Warning", "Yes", "No");
            RoboSep_UserConsole.getInstance().frmOverlay.Show();
            prompt.ShowDialog();
            //RoboSep_UserConsole.getInstance().frmOverlay.Hide();
            listboxMenu.Invalidate();
            if (prompt.DialogResult == DialogResult.OK)
             */ 
            {
                string logMSG = "Quiting Service";
                GUI_Controls.uiLog.SERVICE_LOG(this, "btnQuit_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            
                this.Enabled = false;

                InstrumentComms.getInstance().disconnectFromInstrument();
                Thread oThread = new Thread(new ThreadStart(waitAndQuit));
                oThread.IsBackground = true;

                // Start the thread
                oThread.Start();
            }
            /*
            else
            {
                listboxMenu.SelectedIndex = 0;
            }
            */
        }
        private void waitAndQuit()
        {
            Thread.Sleep(3000);

            string logMSG = "Application.Exit()";
            GUI_Controls.uiLog.SERVICE_LOG(this, "waitAndQuit", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            
            Application.Exit();

        }
        private void updatePanelsEnabled()
        {
            bool isEnabled = InstrumentComms.getInstance().commState == InstrumentComms.CommsStates.CONNECTED;

            foreach (Panel p in panels)
            {
                p.Enabled = isEnabled;
            }
            
                //TODO: fix hack with disabled label font color
                pnlSettings.Enabled = true;
                button_Lid.Enabled = isEnabled;
                button_TimedStart.Enabled = isEnabled;
                button_LoggingLevel.Enabled = isEnabled;
                button_SaveAsFactory.Enabled = isEnabled;
                button_RestoreFactory.Enabled = isEnabled;
                button_RestoreBackup.Enabled = isEnabled;
                button_BarcodeRescan.Enabled = isEnabled;

            //Service Log panel should always be enabled
           pnlServiceLog.Enabled = true;


            pnlExit.Enabled = true;

            button_Connect.Invalidate();
        }

        private void listboxMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = Array.IndexOf(panelNames, listboxMenu.SelectedItem.ToString());
            panels[idx].BringToFront();
            //imgMainSelectionBridge.BringToFront();

            //TODO: Messagebox if not connected
            if (listboxMenu.SelectedItem.ToString() == EXIT)
            {
                btnQuit_Click(sender, e);
                //Close();
            }
            else if (listboxMenu.SelectedItem.ToString() == MENU_SETTINGS)
            {
                UpdateBtnTimedStartText();
                UpdateBtnLidSensorText();
                UpdateBtnLoggingLevelText();
            }
            
        }

        private void listboxSubMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listboxMenu.SelectedItem.ToString() == CALIBRATION)
            {
                string sMSG = "Do you want to run this script?";
                RoboMessagePanel prompt = new RoboMessagePanel(this, sMSG, "Run Script", "Yes", "No");
                RoboSep_UserConsole.getInstance().frmOverlay.Show();
                prompt.ShowDialog();
                //RoboSep_UserConsole.getInstance().frmOverlay.Hide();
                listboxMenu.Invalidate();
                if (prompt.DialogResult == DialogResult.OK)
                //if (MessageBox.Show("Do you want to run this script?",
                //"Run Script", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                //    DialogResult.Yes)
                {
                    for(int i=0;i<num_scripts;i++)
                    {
                        script s = all_scripts[i];
                        if (s.name == listboxSubMenu.SelectedItem.ToString())
                        {
                            //TODO: Add dialog to confirm start script!!
                            startScript = s;

                            string logMSG = "Prepare to start script: "+s.name;
                            GUI_Controls.uiLog.SERVICE_LOG(this, "listboxSubMenu_SelectedIndexChanged", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

                            Close();
                            break;
                        }
                    }
                }
            }
        }

        private void ServiceMenu_Shown(object sender, EventArgs e)
        {
            if (InstrumentComms.getInstance().commState == InstrumentComms.CommsStates.CONNECTED)
            {
                lblConnect.Text = SeparatorResourceManager.GetSeparatorString(StringId.Disconnect);//"Disconnect";
                button_Connect.AccessibleName = SeparatorResourceManager.GetSeparatorString(StringId.Disconnect);//"Disconnect";
        
            }
            else
            {
                lblConnect.Text = SeparatorResourceManager.GetSeparatorString(StringId.Connect); //"Connect";
                button_Connect.AccessibleName = SeparatorResourceManager.GetSeparatorString(StringId.Connect); //"Connect";
            } 
            button_Connect.Invalidate();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string logMSG = (InstrumentComms.getInstance().commState==InstrumentComms.CommsStates.DISCONNECTED)?"Connecting":"Disconnecting";
            GUI_Controls.uiLog.SERVICE_LOG(this, "btnConnect_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            
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
                else
                {
                    if (InstrumentComms.getInstance().commState == InstrumentComms.CommsStates.CONNECTED)
                    {
                        lblConnect.Text = SeparatorResourceManager.GetSeparatorString(StringId.Disconnect);//"Disconnect";
                        button_Connect.AccessibleName = SeparatorResourceManager.GetSeparatorString(StringId.Disconnect);//"Disconnect";
                    }
                    else
                    {
                        lblConnect.Text = SeparatorResourceManager.GetSeparatorString(StringId.Connect); //"Connect";
                        button_Connect.AccessibleName = SeparatorResourceManager.GetSeparatorString(StringId.Connect); //"Connect";
                    }
                    updatePanelsEnabled();
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
                else
                {

                }
            }

            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
        }
        #endregion

        private XmlSerializer myXmlSerializer = new XmlSerializer(typeof(RoboSepService));
        static private string myConfigPath = Application.StartupPath + "\\..\\config\\RoboSepService.config";
        static private string myXSDPath = Application.StartupPath + "\\..\\config\\RoboSepService.xsd";
        static private RoboSepService myConfig = null;
        private void LoadServiceConfig()
        {
            if (File.Exists(myConfigPath))
            {

                // Initialise a file stream for reading
                FileStream fs = new FileStream(myConfigPath, FileMode.Open);

                try
                {
                    // Deserialize a RoboSepProtocol XML description into a RoboSepProtocol 
                    // object that matches the contents of the specified protocol file.										
                    XmlReader reader = new XmlTextReader(fs);

                    // Create a validating reader to process the file.  Report any errors to the 
                    // validation page.
                    XmlValidatingReader validatingReader = new XmlValidatingReader(reader);
                    validatingReader.ValidationType = ValidationType.Schema;

                    // Get the RoboSep protocol schema and add it to the collection for the 
                    // validator
                    XmlSchemaCollection xsc = new XmlSchemaCollection();
                    xsc.Add("STI", myXSDPath);
                    validatingReader.Schemas.Add(xsc);

                    // 'Rehydrate' the object (that is, deserialise data into the object)					
                    myConfig = (RoboSepService)myXmlSerializer.Deserialize(validatingReader);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("RoboSepService Config File Invalid "+ex.Message);
                }
                finally
                {
                    // Close the file stream
                    fs.Close();
                }
            }

        }

        public void SaveServiceConfig()
        {
            XmlSerializer myXmlSerializer = new XmlSerializer(typeof(RoboSepService));
            string myUserConfigPath = Application.StartupPath + "\\..\\config\\RoboSepService.config";
            using (FileStream fs = new FileStream(myUserConfigPath, FileMode.Create))
            {
                XmlTextWriter writer = new XmlTextWriter(fs, new UTF8Encoding());
                writer.Formatting = Formatting.Indented;
                myXmlSerializer.Serialize(writer, myConfig);
            }
        }

        public script StartScript
        {
            get { return startScript; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string logMSG = "Closing ServiceMenu";
            GUI_Controls.uiLog.SERVICE_LOG(this, "ServiceMenu", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            
            Close();
        }

        private void listboxMenu_DrawItem(object sender, DrawItemEventArgs e)
        {
            // Draw the background of the ListBox control for each item.
            e.DrawBackground();
            // Define the default color of the brush as black.
            Brush brushBG = new SolidBrush(listboxMenu.BackColor);

            // Determine the color of the brush to draw each item based 
            // on the index of the item to draw.
            /*
            switch (e.Index)
            {
                case 0:
                    myBrush = Brushes.Red;
                    break;
                case 1:
                    myBrush = Brushes.Orange;
                    break;
                case 2:
                    myBrush = Brushes.Purple;
                    break;
            }
             */

            Rectangle r = e.Bounds;
            r.Height = MENU_HEIGHT;// 66;
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                //e.Graphics.DrawImage(imgMainSelection, r);
                imgMainSelectionBridge.Top = r.Y + panel2.Top + 12;
                e.Graphics.FillRectangle(brushMenuSelectBG, r);
            }
            else
            {
                //some code when the item is unselected
                r = e.Bounds;
                r.Height = MENU_HEIGHT; // 66;
                e.Graphics.FillRectangle(brushBG, r);
            }

            // Draw the current item text based on the current Font 
            // and the custom brush settings.
            r = e.Bounds;
            r.X += 20;
            r.Y += 15;
            if (e.Index > -1 && e.Index < listboxMenu.Items.Count)
            {
                e.Graphics.DrawString(listboxMenu.Items[e.Index].ToString(),
                    e.Font, ((e.State & DrawItemState.Selected) == DrawItemState.Selected) ? brushSubActiveText : brushSubDisabledText,
                    r, StringFormat.GenericDefault);
            }
            // If the ListBox has focus, draw a focus rectangle around the selected item.
            //e.DrawFocusRectangle();
        }

        private void listboxMenu_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = MENU_HEIGHT;
        }

        private void listboxSubMenu_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = 25;
        }

        private void listboxSubMenu_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            Brush myBrush = brushSubDisabledText;
            if (listboxSubMenu.Enabled)
            {
                myBrush = ((e.State & DrawItemState.Selected) == DrawItemState.Selected) ? brushSubActiveText : brushSubActiveText;
            }
            Rectangle r = e.Bounds;
            e.Graphics.FillRectangle(brushMenuSelectBG, r);
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                //r.Width -= 10;
                //e.Graphics.DrawImage(imgMainSelection, r);
                e.Graphics.FillRectangle(brushPressed, r);
            }

            // Draw the current item text based on the current Font 
            // and the custom brush settings.
            r = e.Bounds;
            r.X += 20;
            r.Y += 2;
            if (e.Index > -1 && e.Index < listboxSubMenu.Items.Count)
            {
                e.Graphics.DrawString(listboxSubMenu.Items[e.Index].ToString(),
                    e.Font,
                    myBrush,
                    r, StringFormat.GenericDefault);
            }
        }

        private void UpdateBtnTimedStartText()
        {
            button_TimedStart.AccessibleName = InstrumentComms.getInstance().GetTimedStart() ? "Disable" : "Enable";
        }

        private void UpdateBtnLidSensorText()
        {
            button_Lid.AccessibleName = InstrumentComms.getInstance().GetIgnoreLidSensor() ? "Enable" : "Disable";
        }
        private void btnLidSensor_Click(object sender, EventArgs e)
        {
            string logMSG = "clicked: " + (button_Lid.AccessibleName == "Disable");
            GUI_Controls.uiLog.SERVICE_LOG(this, "btnLidSensor_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            InstrumentComms.getInstance().SetIgnoreLidSensor(button_Lid.AccessibleName == "Disable");
            UpdateBtnLidSensorText();
        }

        private void btnTimedStart_Click(object sender, EventArgs e)
        {
            string logMSG = "clicked: "+(button_TimedStart.AccessibleName == "Enable");
            GUI_Controls.uiLog.SERVICE_LOG(this, "btnTimedStart_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            InstrumentComms.getInstance().SetTimedStart(button_TimedStart.AccessibleName == "Enable");
            UpdateBtnTimedStartText();
        }

        private void btnRestoreWithFactory_Click(object sender, EventArgs e)
        {
            string logMSG = "clicked";
            GUI_Controls.uiLog.SERVICE_LOG(this, "btnRestoreWithFactory_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            InstrumentComms.getInstance().RestoreIniWithFactory();
            //MessageBox.Show("Done");

            string sMSG = "Done";
            GUI_Controls.RoboMessagePanel newPrompt = new GUI_Controls.RoboMessagePanel(this, sMSG);
            RoboSep_UserConsole.getInstance().frmOverlay.Show();
            newPrompt.ShowDialog();
            //RoboSep_UserConsole.getInstance().frmOverlay.Hide();
            listboxMenu.Invalidate();
        }

        private void btnSaveAsFactory_Click(object sender, EventArgs e)
        {
            string logMSG = "clicked";
            GUI_Controls.uiLog.SERVICE_LOG(this, "btnSaveAsFactory_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            InstrumentComms.getInstance().SaveIniAsFactory();
            //MessageBox.Show("Done");

            string sMSG = "Done";
            GUI_Controls.RoboMessagePanel newPrompt = new GUI_Controls.RoboMessagePanel(this,sMSG);
            RoboSep_UserConsole.getInstance().frmOverlay.Show();
            newPrompt.ShowDialog();
            //RoboSep_UserConsole.getInstance().frmOverlay.Hide();
            listboxMenu.Invalidate();
        }

        private void ServiceMenu_Load(object sender, EventArgs e)
        {
            List<Image> ilist = new List<Image>();
            ilist.Add(Service.Properties.Resources.BUTTON_PROMPTBUTTON_BLACK0);
            ilist.Add(Service.Properties.Resources.BUTTON_PROMPTBUTTON_BLACK0);
            ilist.Add(Service.Properties.Resources.BUTTON_PROMPTBUTTON_BLACK0);
            ilist.Add(Service.Properties.Resources.BUTTON_PROMPTBUTTON_BLACK3);
            button_Lid.ChangeGraphics(ilist);
            button_TimedStart.ChangeGraphics(ilist);
            button_LoggingLevel.ChangeGraphics(ilist);
            button_Connect.ChangeGraphics(ilist);
            button_Pendant.ChangeGraphics(ilist);

            ilist.Clear();
            ilist.Add(Service.Properties.Resources.BUTTON_PROMPTBUTTON_BLACK_BIG0);
            ilist.Add(Service.Properties.Resources.BUTTON_PROMPTBUTTON_BLACK_BIG0);
            ilist.Add(Service.Properties.Resources.BUTTON_PROMPTBUTTON_BLACK_BIG0);
            ilist.Add(Service.Properties.Resources.BUTTON_PROMPTBUTTON_BLACK_BIG3);
            button_SaveAsFactory.AccessibleName = "Save As Factory";
            button_SaveAsFactory.ChangeGraphics(ilist);
            button_RestoreFactory.AccessibleName = "Restore To Factory";
            button_RestoreFactory.ChangeGraphics(ilist);
            button_RestoreBackup.AccessibleName = "Restore To Backup";
            button_RestoreBackup.ChangeGraphics(ilist);
            button_BarcodeRescan.AccessibleName = "Barcode Offset";
            button_BarcodeRescan.ChangeGraphics(ilist);

        }

        private void listServiceLogs_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = 28;
        }

        private void listServiceLogs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listServiceLogs.SelectedIndex != -1)
            {
                System.Diagnostics.Process.Start("notepad.exe", listServiceLogs.Items[listServiceLogs.SelectedIndex].ToString());
                listServiceLogs.SelectedIndex = -1;
            }
        }

        private void listServiceLogs_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            Brush myBrush = brushSubActiveText;
            Brush brushBG = brushMenuSelectBG;

            //146 108 175

            if (listServiceLogs.Enabled)
            {
                //myBrush = ((e.State & DrawItemState.Selected) == DrawItemState.Selected) ? brush2 : brush1;
                //myBrush = brush1;
            }
            Rectangle r = e.Bounds;
            e.Graphics.FillRectangle(brushBG, r);
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                //r.Width -= 10;
                //e.Graphics.DrawImage(imgMainSelection, r);
                e.Graphics.FillRectangle(brushPressed, r);
            }

            // Draw the current item text based on the current Font 
            // and the custom brush settings.
            r = e.Bounds;
            r.X += 20;
            r.Y += 2;

            if (e.Index > -1 && e.Index < listServiceLogs.Items.Count)
            {
                e.Graphics.DrawString(Path.GetFileNameWithoutExtension(listServiceLogs.Items[e.Index].ToString()),
                    e.Font,
                    myBrush,
                    r, StringFormat.GenericDefault);
            }
        }

        private void button_LoggingLevel_Click(object sender, EventArgs e)
        {
            string logMSG = "clicked: Verbose=" + (button_LoggingLevel.AccessibleName == "Verbose");
            GUI_Controls.uiLog.SERVICE_LOG(this, "button_LoggingLevel_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

            GUI_Controls.uiLog.SET_SERVICE_LOG_OUTPUT_LEVEL((button_LoggingLevel.AccessibleName == "Verbose")?
                GUI_Controls.uiLog.LogLevel.DEBUG:GUI_Controls.uiLog.LogLevel.EVENTS);

            UpdateBtnLoggingLevelText();

            myConfig.LoggingLevel = GUI_Controls.uiLog.GET_SERVICE_LOG_OUTPUT_LEVEL() == GUI_Controls.uiLog.LogLevel.DEBUG ?  "Verbose" : "Normal" ;
            SaveServiceConfig();
        }

        private void UpdateBtnLoggingLevelText()
        {
            button_LoggingLevel.AccessibleName = GUI_Controls.uiLog.GET_SERVICE_LOG_OUTPUT_LEVEL() == GUI_Controls.uiLog.LogLevel.DEBUG ? "Normal" : "Verbose";
        }

        private void button_RestoreBackup_Click(object sender, EventArgs e)
        {
            string logMSG = "clicked";
            GUI_Controls.uiLog.SERVICE_LOG(this, "button_RestoreBackup_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            
            //MessageBox.Show("Done");

            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                GUI_Controls.uiLog.SERVICE_LOG(this, "selected hardware.ini: " + file, GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

                InstrumentComms.getInstance().LoadSelectedHardwareINI(file);

                string sMSG = "Done";
                GUI_Controls.RoboMessagePanel newPrompt = new GUI_Controls.RoboMessagePanel(this, sMSG);
                RoboSep_UserConsole.getInstance().frmOverlay.Show();
                newPrompt.ShowDialog();
                //RoboSep_UserConsole.getInstance().frmOverlay.Hide();
                listboxMenu.Invalidate();
            }

        }


        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        private const uint WM_VSCROLL = 0x0115;
        private const int SB_PAGEUP = 2; // Scrolls one page up
        private const int SB_PAGEDOWN = 3; // Scrolls one page down

        private void button_PageUp_Click(object sender, EventArgs e)
        {
            SendMessage(listServiceLogs.Handle, WM_VSCROLL, SB_PAGEUP, 0);
        }

        private void button_PageDown_Click(object sender, EventArgs e)
        {
            SendMessage(listServiceLogs.Handle, WM_VSCROLL, SB_PAGEDOWN, 0);
        }

        private void listboxTeachingSubMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listboxMenu.SelectedItem.ToString() == TEACHING)
            {

                string sMSG = "Do you want to run this script?";
                RoboMessagePanel prompt = new RoboMessagePanel(this, sMSG, "Run Script", "Yes", "No");
                RoboSep_UserConsole.getInstance().frmOverlay.Show();
                prompt.ShowDialog();
                //RoboSep_UserConsole.getInstance().frmOverlay.Hide();
                listboxMenu.Invalidate();
                if (prompt.DialogResult == DialogResult.OK)
                //if (MessageBox.Show("Do you want to run this script?",
                //"Run Script", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                //    DialogResult.Yes)
                {
                    for (int i = 0; i < num_scripts; i++)
                    {
                        script s = all_scripts[i];
                        if (s.name == listboxTeachingSubMenu.SelectedItem.ToString())
                        {
                            //TODO: Add dialog to confirm start script!!
                            startScript = s;

                            string logMSG = "Prepare to start script: " + s.name;
                            GUI_Controls.uiLog.SERVICE_LOG(this, "listboxTeachingSubMenu_SelectedIndexChanged", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

                            Close();
                            break;
                        }
                    }
                }
            }
        }

        private void listboxTeachingSubMenu_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = 25;
        }

        private void listboxTeachingSubMenu_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            Brush myBrush = brushSubDisabledText;
            if (listboxTeachingSubMenu.Enabled)
            {
                myBrush = ((e.State & DrawItemState.Selected) == DrawItemState.Selected) ? brushSubActiveText : brushSubActiveText;
            }
            Rectangle r = e.Bounds;
            e.Graphics.FillRectangle(brushMenuSelectBG, r);
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                //r.Width -= 10;
                //e.Graphics.DrawImage(imgMainSelection, r);
                e.Graphics.FillRectangle(brushPressed, r);
            }

            // Draw the current item text based on the current Font 
            // and the custom brush settings.
            r = e.Bounds;
            r.X += 20;
            r.Y += 2;
            if (e.Index > -1 && e.Index < listboxTeachingSubMenu.Items.Count)
            {
                e.Graphics.DrawString(listboxTeachingSubMenu.Items[e.Index].ToString(),
                    e.Font,
                    myBrush,
                    r, StringFormat.GenericDefault);
            }
        }

        private void button_BarcodeRescan_Click(object sender, EventArgs e)
        {
            string logMSG = "clicked";
            GUI_Controls.uiLog.SERVICE_LOG(this, "button_Barcode_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

            string sMSG = "Current Value";
            double currentValue = InstrumentComms.getInstance().GetBarcodeThetaOffset();
            RoboBarcodeRescanPanel prompt = new RoboBarcodeRescanPanel(this, sMSG, "Set Barcode Offset Value", "OK", "Cancel", currentValue.ToString());
            RoboSep_UserConsole.getInstance().frmOverlay.Show();
            prompt.ShowDialog();
            if (prompt.DialogResult == DialogResult.OK)
            {
                currentValue = Double.Parse(prompt.CurrentValue);
                InstrumentComms.getInstance().SetBarcodeThetaOffset(currentValue);

                sMSG = "Barcode offset is set to " + currentValue+".";
                GUI_Controls.RoboMessagePanel newPrompt = new GUI_Controls.RoboMessagePanel(this, sMSG);
                RoboSep_UserConsole.getInstance().frmOverlay.Show();
                newPrompt.ShowDialog();
            }
            else
            {
                //MessageBox.Show("Cancel");
            }

        }

        private void txtIpAddr_MouseClick(object sender, MouseEventArgs e)
        {
            if (RoboSep_UserConsole.KeyboardEnabled)
            {
                // Show window overlay
                RoboSep_UserConsole.getInstance().frmOverlay.Show();
                RoboSep_UserConsole.getInstance().frmOverlay.BringToFront();

                // Create keybaord control
                GUI_Controls.Keyboard newKeyboard =
                    new GUI_Controls.Keyboard(RoboSep_UserConsole.getInstance(),
                        txtIpAddr, null, RoboSep_UserConsole.getInstance().frmOverlay,false);
                newKeyboard.Show();

                // add keyboard control to user console "track form"
                RoboSep_UserConsole.getInstance().addForm(newKeyboard, newKeyboard.Offset);
                //RoboSep_UserConsole.myUserConsole.addForm(newKeyboard.overlay, newKeyboard.overlayOffset);
            }
        }

        private void button_Pendant_Click(object sender, EventArgs e)
        {
            PendantMode = !PendantMode;
            update_pendant_text();
        }
        private void update_pendant_text()
        {
            button_Pendant.AccessibleName = PendantMode ? "Disable" : "Enable";
        }

    }
}
