using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Xml;
using System.Text;
using System.Windows.Forms;
using Invetech.ApplicationLog;
namespace GUI_Controls
{
    public partial class NumberPad : Form
    {
        // variable for control that is going to receive value
        private double UpperLimit;
        private double LowerLimit;
        private string Message;
        private Label returnLabel;
        private Form overlay;
        private Form PreviousForm;
        public Point Offset;

        // strings for multilanguage messaging
        string[] NPMessages;
        private List<Image> ilist1 = new List<Image>();
        private List<Image> ilist2 = new List<Image>();
        private List<Image> ilist3 = new List<Image>();
        
        public NumberPad(Form prevForm, Form over, Label lblOutput, string message, double min, double max, string[] msgs)
        {
            InitializeComponent();
            SuspendLayout();

            // LOG
            string logMSG = "Opening New Number Pad window";
            //GUI_Controls.uiLog.LOG(this, "NumberPad", GUI_Controls.uiLog.LogLevel.DEBUG, logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);            

            // initialize variables
            UpperLimit = max;
            LowerLimit = min;
            Message = message;
            text_instructions.Text = message;
            overlay = over;
            returnLabel = lblOutput;
            
            // set labels
            NPMessages = msgs;
            button_cancel.Text = NPMessages[5]; // cancel txt
            button_enter.Text = NPMessages[6]; // enter text
            button_clear.Text = NPMessages[7]; // clear txt


            try
            {
                textBox_value.Text = lblOutput.Text;
                if (lblOutput.Text != string.Empty)
                    textBox_value.Text = Convert.ToDouble(lblOutput.Text).ToString();

            }
            catch
            {
                string val = string.Empty;
                for (int i = 0; i < lblOutput.Text.Length; i++)
                {
                    if ((int)lblOutput.Text[i] >= (int)('0') && (int)lblOutput.Text[i] <= (int)('9')
                        || lblOutput.Text[i] == '.')
                        val += lblOutput.Text[i];
                }
                textBox_value.Text = val;
            }
            PreviousForm = prevForm;
            // find location to place number pad
            Offset = new Point((prevForm.Size.Width - this.Size.Width) / 2, 50);
            this.Location = new Point(prevForm.Location.X + Offset.X, prevForm.Location.Y + Offset.Y);

            // change graphics for number buttons
            ilist1.Add(Properties.Resources.btnKeyboard_STD);
            ilist1.Add(Properties.Resources.btnKeyboard_STD);
            ilist1.Add(Properties.Resources.btnKeyboard_STD);
            ilist1.Add(Properties.Resources.btnKeyboard_CLICK);
            button_0.ChangeGraphics(ilist1);
            button_1.ChangeGraphics(ilist1);
            button_2.ChangeGraphics(ilist1);
            button_3.ChangeGraphics(ilist1);
            button_4.ChangeGraphics(ilist1);
            button_5.ChangeGraphics(ilist1);
            button_6.ChangeGraphics(ilist1);
            button_7.ChangeGraphics(ilist1);
            button_8.ChangeGraphics(ilist1);
            button_9.ChangeGraphics(ilist1);
            button_point.ChangeGraphics(ilist1);
            // change clear & cancel graphics

            ilist2.Add(Properties.Resources.btnCaps_STD);
            ilist2.Add(Properties.Resources.btnCaps_STD);
            ilist2.Add(Properties.Resources.btnCaps_STD);
            ilist2.Add(Properties.Resources.btnCaps_CLICK);
            button_cancel.ChangeGraphics(ilist2);
            button_clear.ChangeGraphics(ilist2);
            // change graphcis for enter

            ilist3.Add(Properties.Resources.btnEnterThick_STD);
            ilist3.Add(Properties.Resources.btnEnterThick_STD);
            ilist3.Add(Properties.Resources.btnEnterThick_STD);
            ilist3.Add(Properties.Resources.btnEnterThick_CLICK);
            button_enter.ChangeGraphics(ilist3);

            ResumeLayout();
        }

        private void validateEntry()
        {
            try
            {
                // if value is within specified range
                if (textBox_value.Text != string.Empty &&
                    Convert.ToDouble(textBox_value.Text) <= UpperLimit &&
                    Convert.ToDouble(textBox_value.Text) >= LowerLimit)
                // enter button closes control and returns
                // value to previous control
                {
                    string returnedString = string.Format("{0:0.000}", Convert.ToDouble(textBox_value.Text));
                    returnLabel.Text = returnedString;
                    overlay.Hide();
                    this.DialogResult = DialogResult.OK;

                    // LOG
                    string logMSG = "Volume: " + returnedString;
                    //GUI_Controls.uiLog.LOG(this, "validateEntry", GUI_Controls.uiLog.LogLevel.DEBUG, logMSG);
                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);            
                }
                else
                {
                    string sMSG = NPMessages[0];
                    RoboMessagePanel prompt = new RoboMessagePanel(this, MessageIcon.MBICON_WARNING, sMSG, NPMessages[1], NPMessages[4]);
                    prompt.ShowDialog();
                    // clear text box and make active control
                    textBox_value.Text = "";
                    ActiveControl = textBox_value;

                    // LOG
                    string logMSG = "Validating volume failed, not within range";
                    //GUI_Controls.uiLog.LOG(this, "validateEntry", GUI_Controls.uiLog.LogLevel.DEBUG, logMSG);
                    //  (logMSG);
                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);             
                    prompt.Dispose();
                }
            }
            catch
            {
                string sMSG = NPMessages[0];
                RoboMessagePanel prompt = new RoboMessagePanel(this, MessageIcon.MBICON_ERROR, sMSG, NPMessages[1], NPMessages[4]);
                prompt.ShowDialog();
                // clear text box and make active control
                textBox_value.Text = "";
                ActiveControl = textBox_value;

                // LOG
                string logMSG = "Validating volume failed, not within range";
                //GUI_Controls.uiLog.LOG(this, "validateEntry", GUI_Controls.uiLog.LogLevel.DEBUG, logMSG);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);            
                prompt.Dispose();
            }
            
        }

        public void disableCancelButton()
        {
            button_cancel.Enabled = false;
        }

        private void NumberPad_Load(object sender, EventArgs e)
        {
            // bring messages to front
            text_instructions.BringToFront();

            // set up NumberPad Control
            theMin.Text = NPMessages[2] + " " + LowerLimit.ToString() + " " + NPMessages[3] + " " + UpperLimit.ToString() + " mL";
            ActiveControl = textBox_value;
            this.Size = new Size(296, 413);
            
            overlay.Show();
            this.BringToFront();
        }
        
        private void button_enter_Click(object sender, EventArgs e)
        {
            // see if entry is within range, if so store value to 
            // previous controls text box
            if (textBox_value.Text != string.Empty)
                validateEntry();
        }


        private void button_cancel_Click(object sender, EventArgs e)
        {
            // remove NumberPad Control            
            this.Close();
            DialogResult = DialogResult.Cancel;

            overlay.Hide();
            PreviousForm.Activate();

            // LOG
            string logMSG = "Cancel button click";
            //GUI_Controls.uiLog.LOG(this, "button_cancel_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);             
        }

        private void button_0_Click(object sender, EventArgs e)
        {
            if (textBox_value.Text != "0" || textBox_value.Text.Length > 1)
            {
                ActiveControl = textBox_value;
                SendKeys.Send("0");
            }
        }

        private void button_1_Click(object sender, EventArgs e)
        {
            ActiveControl = textBox_value;
            SendKeys.Send("1");
        }

        private void button_2_Click(object sender, EventArgs e)
        {
            ActiveControl = textBox_value;
            SendKeys.Send("2");
        }

        private void button_3_Click(object sender, EventArgs e)
        {
            ActiveControl = textBox_value;
            SendKeys.Send("3");
        }

        private void button_4_Click(object sender, EventArgs e)
        {
            ActiveControl = textBox_value;
            SendKeys.Send("4");
        }

        private void button_5_Click(object sender, EventArgs e)
        {
            ActiveControl = textBox_value;
            SendKeys.Send("5");
        }

        private void button_6_Click(object sender, EventArgs e)
        {
            ActiveControl = textBox_value;
            SendKeys.Send("6");
        }

        private void button_7_Click(object sender, EventArgs e)
        {
            ActiveControl = textBox_value;
            SendKeys.Send("7");
        }

        private void button_8_Click(object sender, EventArgs e)
        {
            ActiveControl = textBox_value;
            SendKeys.Send("8");
        }

        private void button_9_Click(object sender, EventArgs e)
        {
            ActiveControl = textBox_value;
            SendKeys.Send("9");
        }

        private void button_point_Click(object sender, EventArgs e)
        {
            ActiveControl = textBox_value;
            try
            {
                if (textBox_value.Text.Length == 0)
                {                    
                    SendKeys.Send("0");
                    SendKeys.Send(".");
                }
                Convert.ToDouble(textBox_value.Text + ".");
                SendKeys.Send(".");
            }
            catch
            { }
        }

        private void button_clear_Click(object sender, EventArgs e)
        {
            // replace text in box with 0
            textBox_value.Text = "";
        }

        private void textBox_value_KeyPress(object sender, KeyPressEventArgs e)
        {

            int dot = 46;
            int one = 48;   // beginnning of ascii number chars
            int zero = 57;  // end of ascii number chars
            int del = 127;
            int DEL = 8;
            int entr = 13;
            if (((int)e.KeyChar < one || (int)e.KeyChar > zero) && (int)e.KeyChar != dot 
                && (int)e.KeyChar != del && (int)e.KeyChar != entr && (int)e.KeyChar != DEL)
            {
                // don't read key stroke
                e.Handled = true;        
            }
            else if ((int)e.KeyChar == entr)
            {
                validateEntry();
            }
            else if ((int)e.KeyChar == dot)
            {
                string tempstring = textBox_value.Text;
                tempstring += e.KeyChar;
                try
                {
                    if (textBox_value.Text.Length == 0)
                    {
                        // allow key stroke
                    }
                    else
                    {
                        Convert.ToDouble(textBox_value.Text + ".");
                    }
                }
                catch
                {
                    e.Handled = true; 
                }
            }

        }

        private void NumberPad_Deactivate(object sender, EventArgs e)
        {
            overlay.BringToFront();
            this.BringToFront();
            this.Activate();
        }


        private void button_space_Click(object sender, EventArgs e)
        {

        }
    }
}
