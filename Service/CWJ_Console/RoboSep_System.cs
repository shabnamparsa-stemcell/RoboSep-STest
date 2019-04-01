using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using System.Data;
using System.Xml;
using System.Text;
using System.Windows.Forms;
using GUI_Controls;
using System.Configuration;
using System.Collections.Specialized;

using System.IO;


namespace GUI_Console
{
    public partial class RoboSep_System : BasePannel
    {
        private static RoboSep_System mySystem;
        private List<Image> Ilist = new List<Image>();

        private RoboSep_System()
        {
            InitializeComponent();

            // LOG
            string logMSG = "Initializing System user control";
            GUI_Controls.uiLog.LOG(this, "RoboSep_System", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

            Ilist.Add(GUI_Controls.Properties.Resources.Button_SHUTDOWN0);
            Ilist.Add(GUI_Controls.Properties.Resources.Button_SHUTDOWN1);
            Ilist.Add(GUI_Controls.Properties.Resources.Button_SHUTDOWN2);
            Ilist.Add(GUI_Controls.Properties.Resources.Button_SHUTDOWN3);
            button_Shutdown.ChangeGraphics(Ilist);

            Ilist.Clear();
            Ilist.Add(GUI_Controls.Properties.Resources.Button_PACKAGE0);
            Ilist.Add(GUI_Controls.Properties.Resources.Button_PACKAGE1);
            Ilist.Add(GUI_Controls.Properties.Resources.Button_PACKAGE2);
            Ilist.Add(GUI_Controls.Properties.Resources.Button_PACKAGE3);
            button_Diagnostic.ChangeGraphics(Ilist);

            // set checkboxes to on off based on system data
            checkBox_keyboard.Check = RoboSep_UserConsole.KeyboardEnabled;
            checkBox_lidSensor.Check = RoboSep_UserConsole.bLidSensorEnable;
            checkBox_liquidSensor.Check = RoboSep_UserConsole.bLiquidSensorEnable;
        }

        public static RoboSep_System getInstance()
        {
            if (mySystem == null)
            {
                mySystem = new RoboSep_System();
            }
            return mySystem;
        }

        // Touch Keyboard Enable / Disable
        private void checkBox_keyboard_Click(object sender, EventArgs e)
        {
            RoboSep_UserConsole.KeyboardEnabled = checkBox_keyboard.Check;

            // LOG
            string logMSG = "Touch Keyboard enabled = " + checkBox_keyboard.Check.ToString();
            GUI_Controls.uiLog.LOG(this, "checkBox_keyboard_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }
        
        private void label_keybaord_Click(object sender, EventArgs e)
        {
            if (checkBox_keyboard.Check)
            { checkBox_keyboard.Check = false; }
            else
            { checkBox_keyboard.Check = true; }
            RoboSep_UserConsole.KeyboardEnabled = checkBox_keyboard.Check;

            // LOG
            string logMSG = "Touch Keyboard enabled = " + checkBox_keyboard.Check.ToString();
            GUI_Controls.uiLog.LOG(this, "label_keybaord_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        // RoboSep Lid Sensor Enable / Disable
        private void checkBox_lidSensor_Click(object sender, EventArgs e)
        {
            RoboSep_UserConsole.bLidSensorEnable = checkBox_lidSensor.Check;
            fromINI.setValue("GUI", "EnableLidSensor",
                checkBox_lidSensor.Check ? "true" : "false");

            // LOG
            string logMSG = "Lid Sensor enabled = " + checkBox_lidSensor.Check.ToString();
            GUI_Controls.uiLog.LOG(this, "checkBox_lidSensor_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        private void label_lidSensor_Click(object sender, EventArgs e)
        {
            if (checkBox_lidSensor.Check)
            { checkBox_lidSensor.Check = false; }
            else
            { checkBox_lidSensor.Check = true; }
            RoboSep_UserConsole.bLidSensorEnable = checkBox_lidSensor.Check;
            fromINI.setValue("GUI", "EnableLidSensor",
                checkBox_lidSensor.Check ? "true" : "false");

            // LOG
            string logMSG = "Lid Sensor enabled = " + checkBox_lidSensor.Check.ToString();
            GUI_Controls.uiLog.LOG(this, "label_lidSensor_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        //
        // RoboSep Liquid Level Sensor Enable / Disable
        //
        private void checkBox_liquidSensor_Click(object sender, EventArgs e)
        {
            RoboSep_UserConsole.bLiquidSensorEnable = checkBox_liquidSensor.Check;
            fromINI.setValue("GUI", "EnableLiquidLevelSensor",
                checkBox_lidSensor.Check ? "true" : "false");

            // LOG
            string logMSG = "Liquid Level Sensor enabled = " + checkBox_liquidSensor.Check.ToString();
            GUI_Controls.uiLog.LOG(this, "checkBox_liquidSensor_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        private void label_liquidSensor_Click(object sender, EventArgs e)
        {
            if (checkBox_liquidSensor.Check)
            { checkBox_liquidSensor.Check = false; }
            else
            { checkBox_liquidSensor.Check = true; }
            RoboSep_UserConsole.bLiquidSensorEnable = checkBox_liquidSensor.Check;
            fromINI.setValue("GUI", "EnableLiquidLevelSensor",
                checkBox_lidSensor.Check ? "true" : "false");

            // LOG
            string logMSG = "Liquid Level Sensor enabled = " + checkBox_liquidSensor.Check.ToString();
            GUI_Controls.uiLog.LOG(this, "label_liquidSensor_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        private void button_Shutdown_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Application.OpenForms.Count; i++)
            {
                Application.OpenForms[0].Close();
            }

            // LOG
            string logMSG = "Shutdown Software clicked";
            GUI_Controls.uiLog.LOG(this, "button_Shutdown_Click", GUI_Controls.uiLog.LogLevel.GENERAL, logMSG);
        }

        private void button_Service_Click(object sender, EventArgs e)
        {
            RoboSep_ServiceLogin ServiceWindow = new RoboSep_ServiceLogin();
            ServiceWindow.Location = new Point(0, 0);
            this.Visible = false;
            RoboSep_UserConsole.getInstance().Controls.Add(ServiceWindow);
            RoboSep_UserConsole.getInstance().Controls.Remove(RoboSep_UserConsole.ctrlCurrentUserControl);
            RoboSep_UserConsole.ctrlCurrentUserControl = ServiceWindow;

            // LOG
            string logMSG = "Service Menu button clicked";
            GUI_Controls.uiLog.LOG(this, "button_Service_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        private void Button_BasicPrime_Click(object sender, EventArgs e)
        {
            //RoboSep_UserConsole.getInstance().InvokeShutdownAction(); // shutdown protocol
            runMaintenance(0);

            // LOG
            string logMSG = "Basic Prime Maintenance button clicked";
            GUI_Controls.uiLog.LOG(this, "Button_BasicPrime_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        private void button_Home_Click(object sender, EventArgs e)
        {
            runMaintenance(1);

            // LOG
            string logMSG = "Home All Maintenance button clicked";
            GUI_Controls.uiLog.LOG(this, "button_Home_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        private void runMaintenance(int MaintenanceIndex)
        {
            // add maintenance protocol to sample processing window
            RoboSep_UserConsole.getInstance().InvokeMaintenanceAction(MaintenanceIndex);

            // open sample processing window
            RoboSep_RunSamples.getInstance().Visible = false;
            RoboSep_UserConsole.getInstance().Controls.Add(RoboSep_RunSamples.getInstance());
            RoboSep_UserConsole.getInstance().Controls.Remove(this);
            RoboSep_RunSamples.getInstance().Visible = true;
            RoboSep_UserConsole.ctrlCurrentUserControl = RoboSep_RunSamples.getInstance();
        }

        private void button_Diagnostic_Click(object sender, EventArgs e)
        {
            // run package for diagnostic

            // show dialog prompting user to insert USB key
            string msg = "This action can take up to 5 minutes to complete. Create Diagnostic Package?";
            RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(),
                msg, "Enter USB Storage", "Yes", "Cancel");
            RoboSep_UserConsole.getInstance().frmOverlay.Show();
            prompt.ShowDialog();

            if (prompt.DialogResult == DialogResult.OK)
            {              
                // see if zip target directory exist
                string systemPath = RoboSep_UserDB.getInstance().sysPath;

                string zipTarget = systemPath + "ZippedFiles\\";
                if (!Directory.Exists(zipTarget))
                    Directory.CreateDirectory(zipTarget);

                // run zip
                string zipProgram = systemPath + "bin\\dist\\zipDiagLog.exe";
                //System.Diagnostics.Process ZipProcess = System.Diagnostics.Process.Start(zipProgram);
                ProcessStartInfo zipStartInfo = new ProcessStartInfo();
                zipStartInfo.WorkingDirectory = systemPath + "bin\\dist\\";
                zipStartInfo.FileName = "zipDiagLog.exe";
                zipStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Process ZipDiagnostic = new Process();
                ZipDiagnostic.StartInfo = zipStartInfo;
                ZipDiagnostic.Start();                

                // copy zip file to disk
                DirectoryInfo zipDir = new DirectoryInfo(zipTarget);
                FileInfo[] zipFiles = zipDir.GetFiles();

                Array.Sort( zipFiles, delegate(FileInfo f1, FileInfo f2)
                {
                    return f1.LastWriteTime.CompareTo(f2.LastWriteTime);
                });
                
                // zip file copied should be last in list
                FileInfo theZipFile = zipFiles[zipFiles.Length - 1];

                // search for usb directory
                string[] USBdirs = new string[] { "D:\\", "E:\\", "F:\\" };
                int usbDirIndex = -1;
                for (int i = 0; i < USBdirs.Length; i++)
                {
                    if (Directory.Exists(USBdirs[i]))
                    {
                        usbDirIndex = i;
                        break;
                    }
                }

                msg = "Please enter your USB storage device with atleast 100 Mb of free space.  ";
                msg += "USB drive entered?";
                prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), msg, "Enter USB drive", "Yes", "Cancel");
                prompt.ShowDialog();

                // wait for zip to complete
                //if (ZipDiagnostic
                ZipDiagnostic.WaitForExit();

                prompt.closeDialogue(DialogResult.OK);

                if (usbDirIndex > -1)
                {
                    // open file browser to allow user to select specific file location
                    FileBrowser SelectFolder = new FileBrowser(RoboSep_UserConsole.getInstance(), USBdirs[usbDirIndex], theZipFile.FullName);
                    SelectFolder.ShowDialog();
                    RoboSep_UserConsole.getInstance().frmOverlay.Hide();

                    // get target directory from file browser dialog
                    if (SelectFolder.DialogResult == DialogResult.Yes)
                    {
                        string Target = SelectFolder.Target;
                        Target += theZipFile.Name;
                        
                        // Copy file
                        SelectFolder.CopyToTargetDir(theZipFile.FullName, Target);
                    }
                }
                else
                {
                    msg = "USB drive not detected.  Enter USB and try again.";
                    prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), msg, "USB Storage Not Detected", "Ok");
                    prompt.ShowDialog();
                    RoboSep_UserConsole.getInstance().frmOverlay.Hide();
                }
            }
        }

    }
}
