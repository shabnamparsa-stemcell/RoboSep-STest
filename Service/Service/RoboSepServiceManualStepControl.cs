using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Collections.Specialized;
using System.Configuration;
using System.Drawing.Text;
using GUI_Console;

namespace Tesla.Service
{
    public partial class RoboSepServiceManualStepControl : UserControl
    {
        private ProcessScript processScript = null;
        private enum Axis
        {
            None,
            Carousel,
            Theta,
            Z,
            TipStripper
        }
        private Axis currentAxis;

        List<Axis> axisList;
        List<double> axisCoarseList;
        List<double> axisFineList;
        List<string> axisUnitsList;

        private bool vertArmFlip;
        private string barcode_app_path;

        public ServiceDialog servDlg;

        private bool isSelectedAxisPowerOn;

        public RoboSepServiceManualStepControl()
        {
            InitializeComponent();

            string logMSG = "Creating new RoboSepServiceManualStepControl";
            GUI_Controls.uiLog.SERVICE_LOG(this, "RoboSepServiceManualStepControl", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

            SuspendLayout();
            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            /*
            PrivateFontCollection pfc = new PrivateFontCollection();
            string test = directoryName + @"\Resources\FrutigerLTStd-Bold.otf";
            pfc.AddFontFile(directoryName + @"\Resources\CourseFine2.png"); //FrutigerLTStd-Light.otf
            //C:\Projects\TESLA\_2012_09_10_update\Service\Build\Debug\Resources\FrutigerLTStd-Bold.otf
            Font tmpFont = new Font(pfc.Families[0], 14, FontStyle.Regular);
             * */
            
            
            //change toggle button graphics
            List<Image> ilist = new List<Image>();

            #region new coarse fine accept back restore

            ilist.Clear();
            ilist.Add(Service.Properties.Resources.CourseFine1);
            ilist.Add(Service.Properties.Resources.CourseFine1);
            ilist.Add(Service.Properties.Resources.CourseFine2);
            ilist.Add(Service.Properties.Resources.CourseFine2);
            button_CoarseFine.disableImage = Service.Properties.Resources.CourseFine0;
            button_CoarseFine.ChangeGraphics(ilist);
            button_CoarseFine.setToggle(true);
            button_CoarseFine.Check = true;

            ilist.Clear();
            ilist.Add(Service.Properties.Resources.AcceptButton0);
            ilist.Add(Service.Properties.Resources.AcceptButton2);
            ilist.Add(Service.Properties.Resources.AcceptButton2);
            ilist.Add(Service.Properties.Resources.AcceptButton2);
            button_Accept.disableImage = Service.Properties.Resources.AcceptButtonDisable;
            button_Accept.ChangeGraphics(ilist);
            lblAccept.Text = "ACCEPT\nCHANGES";
            ilist.Clear();
            ilist.Add(Service.Properties.Resources.RestoreButton0);
            ilist.Add(Service.Properties.Resources.RestoreButton2);
            ilist.Add(Service.Properties.Resources.RestoreButton2);
            ilist.Add(Service.Properties.Resources.RestoreButton2);
            button_Restore.disableImage = Service.Properties.Resources.RestoreButtonDisable;
            button_Restore.ChangeGraphics(ilist);
            lblRestore.Text = "RESTORE\nPOSITION";
            ilist.Clear();
            ilist.Add(Service.Properties.Resources.BackButton0);
            ilist.Add(Service.Properties.Resources.BackButton2);
            ilist.Add(Service.Properties.Resources.BackButton2);
            ilist.Add(Service.Properties.Resources.BackButton2);
            button_Back.disableImage = Service.Properties.Resources.BackButtonDisable;
            button_Back.ChangeGraphics(ilist);
            button_AxisFreeEnable.disableImage = Service.Properties.Resources.BackButtonDisable;
            button_AxisFreeEnable.ChangeGraphics(ilist);

            ilist.Clear();
            ilist.Add(Service.Properties.Resources.AbortButton0);
            ilist.Add(Service.Properties.Resources.AbortButton2);
            ilist.Add(Service.Properties.Resources.AbortButton2);
            ilist.Add(Service.Properties.Resources.AbortButton2);
            button_abort.disableImage = Service.Properties.Resources.AbortButtonDisable;
            button_abort.ChangeGraphics(ilist);


            ilist.Clear();
            ilist.Add(Service.Properties.Resources.barcode_0);
            ilist.Add(Service.Properties.Resources.barcode_2);
            ilist.Add(Service.Properties.Resources.barcode_2);
            ilist.Add(Service.Properties.Resources.barcode_2);
            button_Barcode.disableImage = Service.Properties.Resources.barcode_inactive;
            button_Barcode.ChangeGraphics(ilist);
            button_Barcode.Visible = false;
            lblBarcode.Visible = false;
            #endregion

            axisList = new List<Axis>() { Axis.Carousel, Axis.Theta, Axis.Z };

            axisCoarseList = new List<double>() { 2, 2, 2 };
            axisFineList = new List<double>() { 0.25, 0.25, 0.25 };
            axisUnitsList = new List<string>() {"deg","deg","mm"};


            currentAxis = Axis.None;

            
            try
            {
                // grab value from config file
                NameValueCollection nvc = (NameValueCollection)
                    ConfigurationSettings.GetConfig("Service/VertArm");
                vertArmFlip =  bool.Parse(nvc.Get("flip"));
            }
            catch
            {
                vertArmFlip = false;
            }

            try
            {
                // grab value from config file
                NameValueCollection nvc = (NameValueCollection)
                    ConfigurationSettings.GetConfig("Service/BarcodeApp");
                barcode_app_path = nvc.Get("barcode_app_path");
            }
            catch
            {
                barcode_app_path = @"c:\OEM\Jadak\Imageview.exe";
            }

            // LOG
            string LOGmsg = "Service Manual Step Control generated";
            GUI_Controls.uiLog.SERVICE_LOG(this, "RoboSepServiceManualStepControl", GUI_Controls.uiLog.LogLevel.EVENTS, LOGmsg);

            servDlg = null;
            m_pendantMode = false;

            lastMode = Tesla.Service.ProcessScript.EnableModes.Disable;

            isSelectedAxisPowerOn = true;

            ResumeLayout();
        }


        public static PictureButton makeNewButtonWithParent(Control p, Rectangle r, Rectangle imgRect, Bitmap BackgroundImage, Bitmap PressedImage,
            Bitmap BackgroundImageSelect, Bitmap BackgroundImageDisabled, Bitmap PressedImageSelect)
        {
            PictureButton button = new PictureButton();
            button.Parent = p;
            button.Bounds = r;
            button.ImageRect = imgRect;
            button.ForeColor = Color.White;
            button.BackgroundImage = BackgroundImage;
            button.PressedImage = PressedImage;
            button.BackgroundImageSelect = BackgroundImageSelect;
            button.BackgroundImageDisabled = BackgroundImageDisabled;
            button.PressedImageSelect = PressedImageSelect;
            //button.Click += new EventHandler(button_Click);

            return button;
        }
        private PictureButton makeNewButton(Rectangle r, Rectangle imgRect, Bitmap BackgroundImage, Bitmap PressedImage,
            Bitmap BackgroundImageSelect, Bitmap BackgroundImageDisabled, Bitmap PressedImageSelect)
        {
            return makeNewButtonWithParent(this,r, imgRect, BackgroundImage, PressedImage,
                                           BackgroundImageSelect, BackgroundImageDisabled, PressedImageSelect);
        }


        private void btnThetaInc_Click(object sender, EventArgs e)
        {
            if (InstrumentComms.getInstance().commState != InstrumentComms.CommsStates.CONNECTED) { return; }
            string axis = "Robot_ThetaAxis";
            try
            {
                float increment = 1.0f * (float)Convert.ToDouble(txtCoarseFine.Text);

                string logMSG = "moveAxis " + axis + " " + increment;
                GUI_Controls.uiLog.SERVICE_LOG(this, "btnThetaInc_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

                InstrumentComms.getInstance().moveAxis(axis, true, increment);
                //InstrumentComms.getInstance().getThetaAxisState();
                if (servDlg != null) servDlg.UpdateHardwareInfo(InstrumentComms.getInstance().GetInstrumentAxisStatusSet((int)Tesla.Service.InstrumentComms.InstrumentAxisStatusSet.THETA));
            }
            catch { }
        }

        private void btnThetaDec_Click(object sender, EventArgs e)
        {
            if (InstrumentComms.getInstance().commState != InstrumentComms.CommsStates.CONNECTED) { return; }
            string axis = "Robot_ThetaAxis";
            try
            {
                float increment = -1.0f * (float)Convert.ToDouble(txtCoarseFine.Text);
                string logMSG = "moveAxis " + axis + " " + increment;
                GUI_Controls.uiLog.SERVICE_LOG(this, "btnThetaDec_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
                InstrumentComms.getInstance().moveAxis(axis, true, increment);
                //InstrumentComms.getInstance().getThetaAxisState();
                if (servDlg != null) servDlg.UpdateHardwareInfo(InstrumentComms.getInstance().GetInstrumentAxisStatusSet((int)Tesla.Service.InstrumentComms.InstrumentAxisStatusSet.THETA));
            }
            catch { }
        }

        private void btnTipStripperEngage_Click(object sender, EventArgs e)
        {
            if (InstrumentComms.getInstance().commState != InstrumentComms.CommsStates.CONNECTED) { return; }
            string logMSG = "moveAxis engage";
            GUI_Controls.uiLog.SERVICE_LOG(this, "btnTipStripperEngage_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            InstrumentComms.getInstance().moveTipStripper(true);
            //InstrumentComms.getInstance().getTipStripperState();
            if (servDlg != null) servDlg.UpdateHardwareInfo(InstrumentComms.getInstance().GetInstrumentAxisStatusSet((int)Tesla.Service.InstrumentComms.InstrumentAxisStatusSet.STRIPPER_ARM));
           
        }
        private void btnTipStripperDisengage_Click(object sender, EventArgs e)
        {
            if (InstrumentComms.getInstance().commState != InstrumentComms.CommsStates.CONNECTED) { return; }
            string logMSG = "moveAxis disengage";
            GUI_Controls.uiLog.SERVICE_LOG(this, "btnTipStripperDisengage_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            InstrumentComms.getInstance().moveTipStripper(false);
            //InstrumentComms.getInstance().getTipStripperState();
            if (servDlg != null) servDlg.UpdateHardwareInfo(InstrumentComms.getInstance().GetInstrumentAxisStatusSet((int)Tesla.Service.InstrumentComms.InstrumentAxisStatusSet.STRIPPER_ARM));
           
        }

        private void btnCarouselInc_Click(object sender, EventArgs e)
        {
            if (InstrumentComms.getInstance().commState != InstrumentComms.CommsStates.CONNECTED) { return; }
            string axis = "Carousel";
            try
            {
                float increment = 1.0f * (float)Convert.ToDouble(txtCoarseFine.Text);

                string logMSG = "moveAxis " + axis + " " + increment;
                GUI_Controls.uiLog.SERVICE_LOG(this, "btnCarouselInc_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
                InstrumentComms.getInstance().moveAxis(axis, true, increment);
                //InstrumentComms.getInstance().getCarouselState();
                if (servDlg != null) servDlg.UpdateHardwareInfo(InstrumentComms.getInstance().GetInstrumentAxisStatusSet((int)Tesla.Service.InstrumentComms.InstrumentAxisStatusSet.CAROUSEL));
            }
            catch { }
        }

        private void btnCarouselDec_Click(object sender, EventArgs e)
        {
            if (InstrumentComms.getInstance().commState != InstrumentComms.CommsStates.CONNECTED) { return; }
            string axis = "Carousel";
            try
            {
                float increment = -1.0f * (float)Convert.ToDouble(txtCoarseFine.Text);
                string logMSG = "moveAxis " + axis + " " + increment;
                GUI_Controls.uiLog.SERVICE_LOG(this, "btnCarouselDec_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
                InstrumentComms.getInstance().moveAxis(axis, true, increment);
                //InstrumentComms.getInstance().getCarouselState();
                if (servDlg != null) servDlg.UpdateHardwareInfo(InstrumentComms.getInstance().GetInstrumentAxisStatusSet((int)Tesla.Service.InstrumentComms.InstrumentAxisStatusSet.CAROUSEL));
            }
            catch { }
        }

        private void btnZInc_Click(object sender, EventArgs e)
        {
            if (InstrumentComms.getInstance().commState != InstrumentComms.CommsStates.CONNECTED) { return; }
            string axis = "Robot_ZAxis";
            try
            {
                float increment = (vertArmFlip ? -1.0f : 1.0f) * (float)Convert.ToDouble(txtCoarseFine.Text);
                string logMSG = "moveAxis " + axis + " " + increment;
                GUI_Controls.uiLog.SERVICE_LOG(this, "btnZInc_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
                InstrumentComms.getInstance().moveAxis(axis, true, increment);
                //InstrumentComms.getInstance().getZAxisState();
                if (servDlg != null) servDlg.UpdateHardwareInfo(InstrumentComms.getInstance().GetInstrumentAxisStatusSet((int)Tesla.Service.InstrumentComms.InstrumentAxisStatusSet.Z));
            }
            catch { }
        }

        private void btnZDec_Click(object sender, EventArgs e)
        {
            if (InstrumentComms.getInstance().commState != InstrumentComms.CommsStates.CONNECTED) { return; }
            string axis = "Robot_ZAxis";
            try
            {
                float increment = (vertArmFlip ? 1.0f : -1.0f) * (float)Convert.ToDouble(txtCoarseFine.Text);
                string logMSG = "moveAxis " + axis + " " + increment;
                GUI_Controls.uiLog.SERVICE_LOG(this, "btnZDec_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
                InstrumentComms.getInstance().moveAxis(axis, true, increment);
                //InstrumentComms.getInstance().getZAxisState();
                if (servDlg != null) servDlg.UpdateHardwareInfo(InstrumentComms.getInstance().GetInstrumentAxisStatusSet((int)Tesla.Service.InstrumentComms.InstrumentAxisStatusSet.Z));
            }
            catch { }
        }

        public ProcessScript ProcessScript
        {
            set 
            {
                Enabled = value!=null;
                processScript = value; 
            }
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (processScript != null)
            {
                string logMSG = "clicked";
                GUI_Controls.uiLog.SERVICE_LOG(this, "btnAccept_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
                processScript.accept();
            }
        }

        private void btnResetPosition_Click(object sender, EventArgs e)
        {
            if (processScript != null)
            {
                string logMSG = "clicked";
                GUI_Controls.uiLog.SERVICE_LOG(this, "btnResetPosition_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
                processScript.resetPosition();
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (processScript != null)
            {
                string logMSG = "clicked";
                GUI_Controls.uiLog.SERVICE_LOG(this, "btnBack_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
                processScript.back();
            }
        }

        private void btnCoarseFine_Click(object sender, EventArgs e)
        {
            //button_CoarseFine.Check = !button_CoarseFine.Check;
            string logMSG = "clicked";
            GUI_Controls.uiLog.SERVICE_LOG(this, "btnCoarseFine_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            updateCoarseFine(currentAxis,button_CoarseFine.Check);
        }

        private void updateCoarseFine(Axis currentAxis, bool isFine)
        {
            txtCoarseFine.Text = "0";
                int idx = axisList.IndexOf(currentAxis);
                if (idx != -1)
                {
                    if (isFine)
                    {
                        txtCoarseFine.Text = axisCoarseList[idx].ToString();
                    }
                    else
                    {
                        txtCoarseFine.Text = axisFineList[idx].ToString();
                    }
                    txtUnits.Text = axisUnitsList[idx].ToString();
                }
        }

        private void setAxisEnable(Axis axis)
        {

            if(currentAxis == axis && axis != Axis.None)return;
            this.SuspendLayout();

            //reset old axis' power if needed
            if (!isSelectedAxisPowerOn)
            {
                button_AxisFreeEnable_Click(this, null);
            }


            string logMSG = axis.ToString();
            GUI_Controls.uiLog.SERVICE_LOG(this, "setAxisEnable", GUI_Controls.uiLog.LogLevel.GENERAL, logMSG);

            

            //reenable the selected one
            if (axis != Axis.None)
            {
                currentAxis = axis;
                    updateCoarseFine(currentAxis,button_CoarseFine.Check);
            }


            this.ResumeLayout();
        }

        private void btnTipStripper_Click(object sender, EventArgs e)
        {
            setAxisEnable(Axis.TipStripper);
        }

        private void btnThetaEnable_Click(object sender, EventArgs e)
        {
            setAxisEnable(Axis.Theta);
        }

        private void btnZEnable_Click(object sender, EventArgs e)
        {
            setAxisEnable(Axis.Z);
        }

        private void btnCarouselEnable_Click(object sender, EventArgs e)
        {
            setAxisEnable(Axis.Carousel);
        }

        internal void setInitialAxis()
        {
            setAxisEnable(Axis.Z);
            robosepModelButton1.setInitialAxis();
        }

        internal void setAxisDefaults(string name, string units, double coarse, double fine)
        {
            List<string> axisNameInScript = new List<string>(){"Carousel","Robot_ThetaAxis","Robot_ZAxis"};
            int idx = axisNameInScript.IndexOf(name);
            if(idx!=-1)
            {
                axisCoarseList[idx] = coarse;
                axisFineList[idx] = fine;
                axisUnitsList[idx] = units;
            }
        }

        private void RoboSepServiceManualStepControl_EnabledChanged(object sender, EventArgs e)
        {
            robosepModelButton1.IsStripperArmEngaged = InstrumentComms.getInstance().tipstripper == "ENG";
        }

        private void button_StripperArmEnable_Click(object sender, EventArgs e)
        {
            setAxisEnable(Axis.TipStripper);
        }
        private void button_StripperArm_Click(object sender, EventArgs e)
        {
            if (robosepModelButton1.IsStripperArmEngaged)
            {
                btnTipStripperEngage_Click(sender, e);
            }
            else
            {
                btnTipStripperDisengage_Click(sender, e);
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }


        private void label3_Click(object sender, EventArgs e)
        {
            button_CoarseFine.Check = !button_CoarseFine.Check; 
            btnCoarseFine_Click(sender, e);
        }

        private void button_abort_Click(object sender, EventArgs e)
        {
            if (processScript != null &&  servDlg!= null)
            {
                string logMSG = "clicked";
                GUI_Controls.uiLog.SERVICE_LOG(this, "btnAbort_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
                processScript.endScript();

                //InstrumentComms.getInstance().disconnectFromInstrument();
                servDlg.SetScriptDoneText();
                //MessageBox.Show("Script done.");

                string sMSG = "Script aborted.";
                servDlg.ShowMessage(sMSG);
                servDlg.ShowMenu();
                return;
            }
        }



        private Service.ProcessScript.EnableModes lastMode;
        internal void enableControls(Service.ProcessScript.EnableModes mode)
        {
            lastMode = mode;
            enableControlsHelper(mode);
        }

        private void enableControlsHelper(Service.ProcessScript.EnableModes mode)
        {
            button_Barcode.Visible = false;
            lblBarcode.Visible = false;
            switch (mode)
            {
                case Service.ProcessScript.EnableModes.Disable: Enabled = false; 
                    break;
                case Service.ProcessScript.EnableModes.EnableAll: Enabled = true;
                    foreach (Control c in this.Controls)
                    {
                        c.Enabled = true;
                    }
                    break;
                case Service.ProcessScript.EnableModes.EnableAllWithBarcode:Enabled = true;
                    foreach (Control c in this.Controls)
                    {
                        c.Enabled = true;
                    }
                    button_Barcode.Visible = true;
                    lblBarcode.Visible = true;
                    break;
                case Service.ProcessScript.EnableModes.EnableAbortOnly: Enabled = true;
                    this.button_abort.Enabled = true;
                    this.lblAbort.Enabled = true;
                    foreach (Control c in this.Controls)
                    {
                        if (c != this.button_abort && c != this.lblAbort)
                            c.Enabled = false;
                    }
                    break;
            };
        }

        private void txtCoarseFine_MouseClick(object sender, MouseEventArgs e)
        {
            if (RoboSep_UserConsole.KeyboardEnabled)
            {
                // Show window overlay
                RoboSep_UserConsole.getInstance().frmOverlay.Show();
                RoboSep_UserConsole.getInstance().frmOverlay.BringToFront();

                // Create keybaord control
                GUI_Controls.Keyboard newKeyboard =
                    new GUI_Controls.Keyboard(RoboSep_UserConsole.getInstance(),
                        txtCoarseFine, null, RoboSep_UserConsole.getInstance().frmOverlay, false);
                newKeyboard.Show();

                // add keyboard control to user console "track form"
                RoboSep_UserConsole.getInstance().addForm(newKeyboard, newKeyboard.Offset);
                //RoboSep_UserConsole.myUserConsole.addForm(newKeyboard.overlay, newKeyboard.overlayOffset);
            }
        }

        private void txtUnits_MouseClick(object sender, MouseEventArgs e)
        {
            txtCoarseFine_MouseClick( sender,  e);
        }

        private void button_Barcode_Click(object sender, EventArgs e)
        {
            if (servDlg == null) return;
            string path = barcode_app_path; //servDlg.barcode_app_path;
            if (path != null && File.Exists(path))
            {
                System.Diagnostics.Process.Start(path, "");
            }
            else
            {
                string sMSG = "The path " + path+ " doesn't exist";
                servDlg.ShowMessage(sMSG);
            }
        }


        private bool m_pendantMode;
        public void SetPendantMode(bool pendantMode)
        {
            m_pendantMode = pendantMode;
            lblAxisFreeEnable.Visible = m_pendantMode;
            button_AxisFreeEnable.Visible = m_pendantMode;
            if (pendantMode)
            {
                enableControlsHelper(Service.ProcessScript.EnableModes.EnableAllWithBarcode);

            }
            else
            {
                enableControls(lastMode);
            }
        }

        private void button_AxisFreeEnable_Click(object sender, EventArgs e)
        {
            if (InstrumentComms.getInstance().commState != InstrumentComms.CommsStates.CONNECTED) { return; }
            string axis = null;
            switch(currentAxis)
            {
                case Axis.Carousel: axis = "Carousel"; break;
                case Axis.Theta: axis = "Robot_ThetaAxis"; break;
                case Axis.Z: axis = "Robot_ZAxis"; break;
            }
            if (axis == null) return;

            if (isSelectedAxisPowerOn)
            {
                lblAxisFreeEnable.Text = "Enable";
            }
            else
            {
                lblAxisFreeEnable.Text = "Free";
            }
            isSelectedAxisPowerOn = !isSelectedAxisPowerOn;

            string logMSG = "powerAxis " + axis + " set to " + isSelectedAxisPowerOn;
            GUI_Controls.uiLog.SERVICE_LOG(this, "button_AxisFreeEnable_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            InstrumentComms.getInstance().powerAxis(axis, isSelectedAxisPowerOn);
        }
         
    }
}
