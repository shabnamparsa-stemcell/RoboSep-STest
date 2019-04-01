//----------------------------------------------------------------------------
// PauseResumeDialog
//
// Invetech Pty Ltd
// Victoria, 3149, Australia
// Phone:   (+61) 3 9211 7700
// Fax:     (+61) 3 9211 7701
//
// The copyright to the computer program(s) herein is the property of 
// Invetech Pty Ltd, Australia.
// The program(s) may be used and/or copied only with the written permission 
// of Invetech Pty Ltd or in accordance with the terms and conditions 
// stipulated in the agreement/contract under which the program(s)
// have been supplied.
// 
// Copyright © 2004. All Rights Reserved.
//
// Notes:
//   03/24/05 - added setliduppausemode to notify user of lidup halt - bdr
//   03/29/06 - pause command - RL
//----------------------------------------------------------------------------

#define bdr15

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

using Tesla.Separator;
using Tesla.Common.Separator;
using Tesla.Common.ResourceManagement;
using Invetech.ApplicationLog;



namespace GUI_Console
{
    public class PauseResumeDialog : GUI_Controls.RoboMessagePanel
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Timer timerPausingTimeout;
        private string myMessage,
                            myCaption;
        private string[] myButtonLabels;
        public enum PauseStatus
        {
            Waiting,
            Manual,
            TipStrip,
            Recovery,
            Lid
        }
        public PauseStatus myPauseStatus;
        private bool isTipStripError = false;

        private const int defaultPauseTimeout = 90000;	// timeout in ms

        private SeparatorEventSink myEventSink = null;

        private SeparatorStatusDelegate myStatusDelegate = null;

        // Language file
        private IniFile LanguageINI = GUI_Console.RoboSep_UserConsole.getInstance().LanguageINI;

        public int PauseTimeout_ms
        {
            set
            {
                timerPausingTimeout.Interval = value;
            }
        }

        public PauseResumeDialog(SeparatorEventSink eventSink)
            : base(RoboSep_UserConsole.getInstance(), GUI_Controls.MessageIcon.MBICON_INFORMATION, "", "")
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            // Keep a reference to the given event sink so we can register locally for 
            // separator events.
            myEventSink = eventSink;

            // configure the timeout timer
            timerPausingTimeout.Interval = defaultPauseTimeout;


            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "PauseResumeDialog form created");                
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }


#if bdr15

        // Create the modal dialog in the Pausing or Paused state (as indicated)
        public DialogResult ShowDialog(Form parent, bool isPausing, bool isLidClosed, bool isPauseCommand, string pauseCommandCaption) // bdr new
        {
            // LOG

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "Show PauseResumeDialog");                
            DialogResult result = DialogResult.Abort;

            if (myEventSink != null)
            {
                if (isPausing)
                {
                    System.Diagnostics.Debug.WriteLine("(new) in PauseResumeDlg.cs ShowDLg() state: isPausing");  // bdr

                    myMessage = string.Format(
                        SeparatorResourceManager.GetSeparatorString(StringId.PausePausingMessage),
                        (timerPausingTimeout.Interval / 1000)
                        );
                    myCaption = SeparatorResourceManager.GetSeparatorString(StringId.RunPausingText);
                    this.label_WindowTitle.Text = myCaption;
                    SetIcon(GUI_Controls.MessageIcon.MBICON_INFORMATION);
                    myButtonLabels = new string[] { };

                    // hide the OK button
                    SetButton1Visible(false);
                    SetButton2Visible(false);

                    // enable the timeout timer
                    timerPausingTimeout.Start();

                    LogFile.AddMessage(TraceLevel.Warning, "Run is pausing");
                }
                else // already Paused
                {
                    System.Diagnostics.Debug.WriteLine("(new) in PauseResumeDlg.cs ShowDLg() state: Paused");  // bdr
                    SetPausedMode(isLidClosed, isPauseCommand, pauseCommandCaption); // bdr new

                    LogFile.AddMessage(TraceLevel.Warning, "Run Paused");
                }

                // Register to receive SeparatorState changes from the event sink
                if (myStatusDelegate == null)
                    myStatusDelegate = new SeparatorStatusDelegate(AtSeparatorStateUpdate);

                myEventSink.UpdateSeparatorState += myStatusDelegate;

                //result = base.ShowDialog(parent, myMessage, myCaption, myButtonLabels,
                //    System.Drawing.SystemIcons.Information);

                //
                // << Jason Modification to fit new GUI >>
                //
                this.PreviousForm = parent != null ? parent : RoboSep_UserConsole.getInstance();
                this.message = myMessage;
                this.SetMessage (myMessage);
                this.button_1.Text = LanguageINI.GetString("lblResume");
                this.button_2.Text = LanguageINI.GetString("lblStop");
                this.SetIcon(GUI_Controls.MessageIcon.MBICON_WAIT);
                this.Size = determineSize(myMessage);
                ResizeButtons();
                this.button_1.Refresh();
                this.button_2.Refresh();

                // show dialog
                RoboSep_UserConsole.showOverlay();
                this.ShowDialog();
                RoboSep_UserConsole.hideOverlay();

                result = this.DialogResult;

                string dbg = string.Format("PsRsDlg result {0}", result);
                System.Diagnostics.Debug.WriteLine(dbg); // bdr

                // De-Register to receive SeparatorState changes from the event sink
                if (myStatusDelegate != null)
                    myEventSink.UpdateSeparatorState -= myStatusDelegate;

                this.Dispose();
            }

            return result;
        }


 

#else

		// Create the modal dialog in the Pausing or Paused state (as indicated)
		public DialogResult ShowDialog(Form parent, bool isPausing)
		{
			DialogResult result = DialogResult.Abort;

			if (myEventSink != null)
			{
				if (isPausing)
				{
					myMessage = string.Format(
						SeparatorResourceManager.GetSeparatorString(StringId.PausePausingMessage),
						(timerPausingTimeout.Interval/1000)
						);
					myCaption = SeparatorResourceManager.GetSeparatorString(StringId.RunPausingText);

					myButtonLabels = new string[]{};

					// hide the OK button
					okButton.Visible = false;

					// enable the timeout timer
					timerPausingTimeout.Start();
				}
				else	// already Paused
				{
					SetPausedMode();
				}

				// Register to receive SeparatorState changes from the event sink
				myEventSink.UpdateSeparatorState += new SeparatorStatusDelegate(AtSeparatorStateUpdate);

				result = base.ShowDialog(parent, myMessage, myCaption, myButtonLabels, 
					System.Drawing.SystemIcons.Information);

				// De-Register to receive SeparatorState changes from the event sink
				myEventSink.UpdateSeparatorState -= new SeparatorStatusDelegate(AtSeparatorStateUpdate);
			}

			return result;
		}

#endif

        /// <summary>
        /// Updates the dialog when the Paused state is reached.
        /// </summary>
        /// <param name="newState"></param>
        private void AtSeparatorStateUpdate(SeparatorState newState)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("------------------------------ AtSeparatorStateUpdate 1------------------------------"));
            try
            {
                if (this.InvokeRequired)
                {
                    SeparatorStatusDelegate eh = new SeparatorStatusDelegate(this.AtSeparatorStateUpdate);
                    this.Invoke(eh, new object[] { newState });
                }
                else
                {
                    switch (newState)
                    {
                        default:
                            break;
                        case SeparatorState.Paused:

                            System.Diagnostics.Debug.WriteLine("------------------------------ - AtSepStateUpdate in PauseResumeDialog, State is PAUSED...call SetPausedMode lidClosed");  // bdr
#if bdr15
                            SetPausedMode(true, false, ""); // bdr new							
#else
                            SetPausedMode();
#endif
                            break;

                        // The instrument has either resumed or the batch run has
                        // completed.  Since this was an instrument-initiated event,
                        // use a different return code (that is, we do not want to
                        // send Resume or Halt commands to the instrument).
                        case SeparatorState.Running:
                        case SeparatorState.BatchComplete:
                        case SeparatorState.BatchHalted:
                            DialogResult = DialogResult.Abort;
                            break;
                    } //switch
                }
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
        }


#if bdr15
        private void SetPausedMode(bool lidClosed, bool isPauseCommand, string pauseCommandCaption)
        {
            // Instrument has paused -- disable the timeout timer
            timerPausingTimeout.Stop();

            // enable buttons
            SetButton2Visible(!isTipStripError);
            button_2.Enabled = !isTipStripError;

            SetButton1Visible(true);
            button_1.Enabled = true;

            button_1.Text = SeparatorResourceManager.GetSeparatorString(StringId.RunResumeText);
            button_2.Text = SeparatorResourceManager.GetSeparatorString(StringId.RunHaltText);
            //myButtonLabels = new string[2] { okButton.Text, cancelButton.Text };
            GUI_Controls.MessageIcon icon = GUI_Controls.MessageIcon.MBICON_INFORMATION;

            if (isPauseCommand)
            {
                myPauseStatus = PauseStatus.Manual;
                myCaption = SeparatorResourceManager.GetSeparatorString(StringId.RunPauseText);
                myMessage = pauseCommandCaption;
                System.Diagnostics.Debug.WriteLine("(new) in PauseResumeDlg.cs - SetPausedMode()...setting paused msg type 0 - PAUSE COMMAND\r");  // bdr

                LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, "System paused manually");            
            }
            else if (pauseCommandCaption.Equals("Manual tip strip"))
            {
                isTipStripError = true;
                myPauseStatus = PauseStatus.TipStrip;
                // show general paused msg or custom paused-due-to-lid-up msg
                icon = GUI_Controls.MessageIcon.MBICON_ERROR;
                myCaption = LanguageINI.GetString("headerTipStripError");
                myMessage = LanguageINI.GetString("msgTipStripError");
                System.Diagnostics.Debug.WriteLine("(new) in PauseResumeDlg.cs - SetPausedMode()...setting paused msg type TIP - lid is closed\r");  // bdr
                pauseCommandCaption = "";

                SetButton2Visible(false);
                button_2.Enabled = false;
                //myButtonLabels = new string[1] { okButton.Text };

                LogFile.AddMessage(TraceLevel.Warning, "PauseResumeDialog: Tip Strip error warning displayed");
            }
            else if (pauseCommandCaption.Equals("Tip strip recovery"))
            {
                isTipStripError = false;
                myPauseStatus = PauseStatus.Recovery;
                // show general paused msg or custom paused-due-to-lid-up msg
                myCaption = LanguageINI.GetString("headerTipStripRecovery");
                myMessage = LanguageINI.GetString("msgTipStripRecovery");
                myMessage = myMessage + '\n' + SeparatorResourceManager.GetSeparatorString(StringId.PausePausedMessage);
                System.Diagnostics.Debug.WriteLine("(new) in PauseResumeDlg.cs - SetPausedMode()...setting paused msg type TIP2 - lid is closed\r");  // bdr
                pauseCommandCaption = "";

                SetButton2Visible(true);
                button_2.Enabled = true;


                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "PauseResumeDialog: Tip strip recovery window displayed");                
            }
            else if (lidClosed)
            {
                myPauseStatus = PauseStatus.Lid;
                // show general paused msg or custom paused-due-to-lid-up msg
                myCaption = SeparatorResourceManager.GetSeparatorString(StringId.RunPauseText);
                myMessage = SeparatorResourceManager.GetSeparatorString(StringId.PausePausedMessage);
                System.Diagnostics.Debug.WriteLine("(new) in PauseResumeDlg.cs - SetPausedMode()...setting paused msg type 1 - lid is closed\r");  // bdr

                LogFile.AddMessage(TraceLevel.Warning, "PauseResumeDialog: Lid Sensor pause window displayed");
            }
            else
            {
                //this branch is never used... ?? RL
                myCaption = SeparatorResourceManager.GetSeparatorString(StringId.RunPauseText);
                myMessage = SeparatorResourceManager.GetSeparatorString(StringId.PauseLidUpMessage);
                System.Diagnostics.Debug.WriteLine("(new) in PauseResumeDlg.cs - SetPausedMode()...setting paused msg type 2 - lid is open\r");  // bdr

                LogFile.AddMessage(TraceLevel.Warning, "PauseResumeDialog: unexpected branch");
            }

            SetIcon(icon);
            SetMessage(myMessage);
            this.label_WindowTitle.Text = myCaption;
            this.Size = determineSize(myMessage);
            ResizeButtons();
            this.button_1.Refresh();
            this.button_2.Refresh();
            Refresh();
        }

#else
        
		private void SetPausedMode()
		{
			// Instrument has paused -- disable the timeout timer
			timerPausingTimeout.Stop();

			// enable buttons
			cancelButton.Visible = true;
			cancelButton.Enabled = true;
			okButton.Visible = true;
			okButton.Enabled = true;

			okButton.Text = SeparatorResourceManager.GetSeparatorString(StringId.RunResumeText);
			cancelButton.Text = SeparatorResourceManager.GetSeparatorString(StringId.RunHaltText);
			myMessage = SeparatorResourceManager.GetSeparatorString(StringId.PausePausedMessage);
			lblMessageText.Text = myMessage;
			myButtonLabels = new string[2]{okButton.Text, cancelButton.Text};

			this.Icon = System.Drawing.SystemIcons.Question;
			myCaption = SeparatorResourceManager.GetSeparatorString(StringId.RunPauseText);
			this.Caption = myCaption;
			Refresh();
		}
        
#endif

        /// <summary>
        /// Timeout the pausing request
        /// </summary>
        private void timerPausingTimout_Tick(object sender, System.EventArgs e)
        {
            // We use DialogResult.No to mean the maximum response time was reached and 
            // the instrument has not paused.
            timerPausingTimeout.Stop();
            DialogResult = DialogResult.No;
        }

        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timerPausingTimeout = new System.Windows.Forms.Timer(this.components);
            // 
            // lblMessageText
            // 
            //this.lblMessageText.Name = "lblMessageText";
            // 
            // okButton
            // 
            //this.okButton.Name = "okButton";
            // 
            // cancelButton
            // 
            //this.cancelButton.Name = "cancelButton";
            // 
            // timerPausingTimeout
            // 
            this.timerPausingTimeout.Interval = 30000;
            this.timerPausingTimeout.Tick += new System.EventHandler(this.timerPausingTimout_Tick);
            // 
            // PauseResumeDialog
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(480, 256);
            this.Name = "PauseResumeDialog";

        }
        #endregion
    }
}

