using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Tesla.InstrumentControl;
using Invetech.ApplicationLog;
using System.Diagnostics;
using GUI_Console;
using System.IO;
using System.Reflection;

namespace Tesla.Service
{
    public class ProcessScript
    {
        private script currentScript = null;
        private int currentStep = -1;
        private System.Windows.Forms.TextBox txtScriptStepText;
        private bool isFinished = false;
        private RoboSepServiceManualStepControl ctlManualStep;
        private ServiceDialog servDialog;

        private ReportCommandCompleteDelegate reportCommandCompleteDelegate;

        public ProcessScript(script script, TextBox txtScriptStepText, RoboSepServiceManualStepControl ctlManualStep, ServiceDialog servDialog)
        {
            // TODO: Complete member initialization
            this.currentScript = script;
            this.txtScriptStepText = txtScriptStepText;
            this.ctlManualStep = ctlManualStep;
            this.reportCommandCompleteDelegate = new ReportCommandCompleteDelegate(ReportCommandCompleteHandler);
            InstrumentComms.ReportCommandComplete += (reportCommandCompleteDelegate);
            this.servDialog = servDialog;
        }
        ~ProcessScript()
        {
            InstrumentComms.ReportCommandComplete -= (reportCommandCompleteDelegate);
        }
        public void ReportCommandCompleteHandler(bool commandSucceeded, bool sequenceComplete, string returnString)
        {
            if (isFinished || currentScript == null || !sequenceComplete || currentStep < 0 || currentScript.command.Length <= currentStep) { return; }
            if (!currentScript.command[currentStep].IsManualStep)
            {
                accept();

            }
        }

        #region delegates to work with forms
        public delegate void UpdateScriptStepTextDelegate(string newText, string video_path, string barcode_app_path);

        public void updateScriptStepText(string newText, string video_path, string barcode_app_path)
        {

            try
            {
                if (txtScriptStepText.InvokeRequired)
                {
                    UpdateScriptStepTextDelegate eh = new UpdateScriptStepTextDelegate(this.updateScriptStepText);
                    txtScriptStepText.Invoke(eh, new object[] { newText, video_path, barcode_app_path });
                }
                servDialog.lineNumScripText = 0;
                txtScriptStepText.Text = newText;
                servDialog.video_path = video_path;
                servDialog.update_video_button();
                servDialog.barcode_app_path = barcode_app_path;
            }

            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
        }
        public delegate void EnableCtlManualStepDelegate(EnableModes mode);

        public enum EnableModes
        {
            Disable,
            EnableAll,
            EnableAbortOnly,
            EnableAllWithBarcode,
        };
        public void enableCtlManualStep(EnableModes mode)
        {

            try
            {
                if (ctlManualStep.InvokeRequired)
                {
                    EnableCtlManualStepDelegate eh = new EnableCtlManualStepDelegate(this.enableCtlManualStep);
                    ctlManualStep.Invoke(eh, new object[] { mode });
                }
                ctlManualStep.enableControls(mode);

            }

            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
        }

        #endregion

        public void back()
        {
            if (isFinished || currentStep <= 0 || currentScript == null) { return; }
            System.Diagnostics.Debug.WriteLine("============== BACK: " + currentStep);

            //todo: move to last last IsBackStop -1 and call accept to run back stop

            //find current back stop
            while (currentStep > 0 && !currentScript.command[currentStep--].IsBackStop) ;

            //find previous back stop
            while (currentStep > 0 && !currentScript.command[currentStep--].IsBackStop) ;

            //process command, accept process currentStep+1 so do decrement first if currentStep is 0
            if (currentStep == 0) { --currentStep; }
            accept();
        }
        public void resetPosition()
        {
            if (isFinished || currentStep <= 0 || currentScript == null) { return; }
            System.Diagnostics.Debug.WriteLine("============== RESET POSITION: " + currentStep);

            //todo: move to last last IsBackStop -1 and call accept to run back stop

            //find current back stop
            while (currentStep > 0 && !currentScript.command[currentStep--].IsBackStop) ;

            //process command, accept process currentStep+1 so do decrement first if currentStep is 0
            if (currentStep == 0) { --currentStep; }
            accept();
        }
        public void accept()
        {
            if (isFinished || currentScript == null || currentScript.command.Length <= currentStep) { return; }
            System.Diagnostics.Debug.WriteLine("============== accept: " + currentStep);

            ++currentStep;

            //no more steps
            if (currentScript.command.Length <= currentStep)
            {
                endScript();
                //InstrumentComms.getInstance().disconnectFromInstrument();
                servDialog.SetScriptDoneText();
                //MessageBox.Show("Script done.");

                string sMSG = "Script done.";
                servDialog.ShowMessage(sMSG);
                servDialog.ShowMenu();
                return;
            }

            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string VideoPath = null;
            string BarcodeAppPath = null;
            if(currentScript.command[currentStep].VideoFilename!=null)
            {
                VideoPath= directoryName + @"\Resources\" + currentScript.command[currentStep].VideoFilename;
            }
            //show barcode app button if in command
            if (currentScript.command[currentStep].RunBarcodeApp != null)
            {
                BarcodeAppPath = currentScript.command[currentStep].RunBarcodeApp;
            }
            updateScriptStepText(currentScript.command[currentStep].Description,
                VideoPath, BarcodeAppPath);

            if (currentScript.command[currentStep].IsMessageOnly)
            {
                enableCtlManualStep(EnableModes.Disable);
                //MessageBox.Show(currentScript.command[currentStep].Description);
                string sMSG = currentScript.command[currentStep].Description;
                if (currentStep == 0)
                {
                    if (servDialog.ShowMessageWithAbort(sMSG) != DialogResult.OK)
                    {
                        endScript();
                        servDialog.ShowMenu();
                        return;
                    }
                }
                else
                {
                    servDialog.ShowMessage(sMSG);
                }
                accept();
                return;
            }
            else if (!currentScript.command[currentStep].IsManualStep)
            {

                enableCtlManualStep(EnableModes.EnableAbortOnly);

                //run command
                if (InstrumentComms.getInstance().commState != InstrumentComms.CommsStates.CONNECTED)
                {
                    //MessageBox.Show("Error: Not connected to instr//ent. End script now.");
                    servDialog.ShowMessage("Error: Not connected to instrument. End script now.");
                    endScript();
                    servDialog.ShowMenu();
                    return;
                }

                if (currentScript.command[currentStep].AxisUnits != null)
                {
                    //default only command
                    ctlManualStep.setAxisDefaults(currentScript.command[currentStep].AxisName, currentScript.command[currentStep].AxisUnits,
                                                  currentScript.command[currentStep].AxisCoarse, currentScript.command[currentStep].AxisFine);
                    accept();
                    return;
                }
                else if (currentScript.command[currentStep].ServerCommand != null)
                {
                    InstrumentComms.getInstance().executeCommandSequence(currentScript.command[currentStep].ServerCommand, 0);
                }
            }
            else
            {
                enableCtlManualStep((currentScript.command[currentStep].RunBarcodeApp != null)?
                    EnableModes.EnableAllWithBarcode:EnableModes.EnableAll);
            }

        }
        public bool IsFinsihed
        {
            get { return isFinished; }
        }
        public void endScript()
        {
            isFinished = true;
            updateScriptStepText("",null, null);
            InstrumentComms.ReportCommandComplete -= (reportCommandCompleteDelegate);


            enableCtlManualStep(EnableModes.Disable);
        }

        public bool isOnAutomatedStep()
        {
            return !isFinished &&
                    currentScript != null &&
                    currentScript.command.Length > currentStep &&
                    currentStep >= 0 &&
                    !currentScript.command[currentStep].IsManualStep;
        }
    }
}
