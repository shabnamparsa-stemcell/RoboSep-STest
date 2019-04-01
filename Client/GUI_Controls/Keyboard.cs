using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Xml;
using System.Text;
using System.Windows.Forms;
using Invetech.ApplicationLog;
using System.Runtime.InteropServices;

namespace GUI_Controls
{
    public partial class Keyboard : Form
    {
        private Control lastControl;
        private Form PreviousForm;

        public Form frmOverlay;
        public Point overlayOffset = new Point(4, 26);
        public Point Offset;
        public bool textInputMultiLine = false;
        public bool textInputAllowDrop = true;

        private static Keyboard myKeyboard;
        private List<Image> ilist1 = new List<Image>();
        private List<Image> ilist2 = new List<Image>();
        private List<Image> ilist3 = new List<Image>();
        private List<Image> ilist4 = new List<Image>();
        private List<Image> ilist5 = new List<Image>();
        private List<Image> ilist6 = new List<Image>();
        private List<Image> ilist7 = new List<Image>();
        private List<Image> ilist8 = new List<Image>();
        #region SuspendDrawing
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

        private const int WM_SETREDRAW = 11;

        public static void SuspendDrawing(Control parent)
        {
            SendMessage(parent.Handle, WM_SETREDRAW, false, 0);
        }

        public static void ResumeDrawing(Control parent)
        {
            SendMessage(parent.Handle, WM_SETREDRAW, true, 0);
            parent.Refresh();
        }
        #endregion


        public static Keyboard getInstance(Form previousform, Control previousControl, AutoCompleteStringCollection source, Form overlayForm, bool hideUserEntry)
        {
            // if initial creation of keyboard
            if (myKeyboard == null)
            {
                myKeyboard = new Keyboard(previousform, previousControl, source, overlayForm, hideUserEntry);
            }
            // modify current keyboard
            else
            {
                //SuspendDrawing(myKeyboard);
                myKeyboard.lastControl = previousControl;
                myKeyboard.PreviousForm = previousform;
 
                if (hideUserEntry)
                {
                    myKeyboard.textbox_input.Multiline = false;
                    myKeyboard.textbox_input.AllowDrop = false;
                    myKeyboard.textbox_input.PasswordChar = '*';
                }
                else
                {
                    myKeyboard.textbox_input.Multiline = myKeyboard.textInputMultiLine;
                    myKeyboard.textbox_input.AllowDrop = myKeyboard.textInputAllowDrop;
                    myKeyboard.textbox_input.PasswordChar = '\0';
                }

                if (source != null)
                    myKeyboard.textbox_input.AutoCompleteCustomSource = source;
                myKeyboard.frmOverlay = overlayForm;
                myKeyboard.textbox_input.Text = previousControl.Text;
            }
            if (overlayForm != null)
            {
                overlayForm.Show();
                overlayForm.BringToFront();
            }
            return myKeyboard;
        }


        public Keyboard(Form previousform, Control previousControl, AutoCompleteStringCollection source, Form overlayForm, bool hideUserEntry)
        {
            //InitializeComponent();
            InitializeComponent(); // optimized replacement for InitializeComponents
            this.DoubleBuffered = true;
            this.textInputMultiLine = this.textbox_input.Multiline;
            this.textInputAllowDrop = this.textbox_input.AllowDrop;
            if (hideUserEntry)
            {
                this.textbox_input.Multiline = false;
                this.textbox_input.AllowDrop = false;
                this.textbox_input.PasswordChar = '*';
            }

            textbox_input.Text = previousControl.Text;
            lastControl = previousControl;
            PreviousForm = previousform;
            if (source != null)
                textbox_input.AutoCompleteCustomSource = source;

            if (PreviousForm != null)
            {
                Rectangle rectPrimaryScreen = Screen.PrimaryScreen.Bounds;
                Offset = new Point((PreviousForm.Size.Width - this.Size.Width) / 2, 75);

                int Y = PreviousForm.Location.Y + Offset.Y;
                if ((Y + this.Size.Height) > rectPrimaryScreen.Height)
                {
                    Y = rectPrimaryScreen.Height - this.Size.Height - 30;
                }
                this.Location = new Point(PreviousForm.Location.X + Offset.X, Y);
            }
            else
            {
                Rectangle rectPrimaryScreen = Screen.PrimaryScreen.Bounds;
                Offset = new Point((rectPrimaryScreen.Width - this.Size.Width) / 2, (rectPrimaryScreen.Height - this.Size.Height) / 2);
                this.Location = new Point(rectPrimaryScreen.X + Offset.X, rectPrimaryScreen.X + Offset.Y);
            }

            frmOverlay = overlayForm;

            SuspendLayout();
            // change close graphics
            List<Image> ilist = new List<Image>();
            ilist1.Add(Properties.Resources.WindowClose_STD);
            ilist1.Add(Properties.Resources.WindowClose_STD);
            ilist1.Add(Properties.Resources.WindowClose_STD);
            ilist1.Add(Properties.Resources.WindowClose_CLICK);
            button_Cancel1.ChangeGraphics(ilist1);
            // graphics for key buttons

            ilist2.Add(Properties.Resources.btnKeyboard_STD);
            ilist2.Add(Properties.Resources.btnKeyboard_STD);
            ilist2.Add(Properties.Resources.btnKeyboard_STD);
            ilist2.Add(Properties.Resources.btnKeyboard_CLICK);
            button1.ChangeGraphics(ilist2);
            button2.ChangeGraphics(ilist2);
            button3.ChangeGraphics(ilist2);
            button4.ChangeGraphics(ilist2);
            button5.ChangeGraphics(ilist2);
            button6.ChangeGraphics(ilist2);
            button7.ChangeGraphics(ilist2);
            button8.ChangeGraphics(ilist2);
            button9.ChangeGraphics(ilist2);
            button0.ChangeGraphics(ilist2);
            buttonQ.ChangeGraphics(ilist2);
            buttonW.ChangeGraphics(ilist2);
            buttonE.ChangeGraphics(ilist2);
            buttonR.ChangeGraphics(ilist2);
            buttonT.ChangeGraphics(ilist2);
            buttonY.ChangeGraphics(ilist2);
            buttonU.ChangeGraphics(ilist2);
            buttonI.ChangeGraphics(ilist2);
            buttonO.ChangeGraphics(ilist2);
            buttonP.ChangeGraphics(ilist2);
            buttonA.ChangeGraphics(ilist2);
            buttonS.ChangeGraphics(ilist2);
            buttonD.ChangeGraphics(ilist2);
            buttonF.ChangeGraphics(ilist2);
            buttonG.ChangeGraphics(ilist2);
            buttonH.ChangeGraphics(ilist2);
            buttonJ.ChangeGraphics(ilist2);
            buttonK.ChangeGraphics(ilist2);
            buttonL.ChangeGraphics(ilist2);
            buttonZ.ChangeGraphics(ilist2);
            buttonX.ChangeGraphics(ilist2);
            buttonC.ChangeGraphics(ilist2);
            buttonV.ChangeGraphics(ilist2);
            buttonB.ChangeGraphics(ilist2);
            buttonN.ChangeGraphics(ilist2);
            buttonM.ChangeGraphics(ilist2);
            button1.ChangeGraphics(ilist2);
            button_.ChangeGraphics(ilist2);
            buttonDot.ChangeGraphics(ilist2);
            // enter & clear

            ilist3.Add(Properties.Resources.btnEnter_STD);
            ilist3.Add(Properties.Resources.btnEnter_STD);
            ilist3.Add(Properties.Resources.btnEnter_STD);
            ilist3.Add(Properties.Resources.btnEnter_CLICK);
            buttonClear.ChangeGraphics(ilist3);
            buttonEnter.ChangeGraphics(ilist3);
            // delete

            ilist4.Add(Properties.Resources.btnCaps_STD);
            ilist4.Add(Properties.Resources.btnCaps_STD);
            ilist4.Add(Properties.Resources.btnCaps_STD);
            ilist4.Add(Properties.Resources.btnCaps_CLICK);
            buttonDel.ChangeGraphics(ilist4);
            // caps

            ilist5.Add(Properties.Resources.btnCaps_STD);
            ilist5.Add(Properties.Resources.btnCaps_STD);
            ilist5.Add(Properties.Resources.btnCaps_CLICK);
            ilist5.Add(Properties.Resources.btnCaps_CLICK);
            buttonCaps.ChangeGraphics(ilist5);
            // shift button

            ilist6.Add(Properties.Resources.btnShift_STD);
            ilist6.Add(Properties.Resources.btnShift_STD);
            ilist6.Add(Properties.Resources.btnShift_CLICK);
            ilist6.Add(Properties.Resources.btnShift_CLICK);
            buttonShift.ChangeGraphics(ilist6);
            // space button

            ilist7.Add(Properties.Resources.btnSpace_STD);
            ilist7.Add(Properties.Resources.btnSpace_STD);
            ilist7.Add(Properties.Resources.btnSpace_STD);
            ilist7.Add(Properties.Resources.btnSpace_CLICK);
            button_space.ChangeGraphics(ilist7);

            // set shift and caps to toggle buttons
            buttonCaps.setToggle(true);
            buttonShift.setToggle(true);
            buttonCaps.Check = false;
            buttonShift.Check = false;

            ResumeLayout();

            // LOG
            string logMSG = "New Touch Keyboard generated w/ Autocomplete";
            //GUI_Controls.uiLog.LOG(this, "Keyboard", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);             
        }

        public Keyboard(Form previousform, Control previousControl, Form overlayForm)
        {
            InitializeComponent();
            textbox_input.Text = previousControl.Text;
            lastControl = previousControl;
            PreviousForm = previousform;

            if (PreviousForm != null)
            {
                Offset = new Point((PreviousForm.Size.Width - this.Size.Width) / 2, 135);
                this.Location = new Point(PreviousForm.Location.X + Offset.X, PreviousForm.Location.Y + Offset.Y);
            }
            else
            {
                Rectangle rectPrimaryScreen = Screen.PrimaryScreen.Bounds;
                Offset = new Point((rectPrimaryScreen.Width - this.Size.Width) / 2, (rectPrimaryScreen.Height - this.Size.Height) / 2);
                this.Location = new Point(rectPrimaryScreen.X + Offset.X, rectPrimaryScreen.X + Offset.Y);
            }

            frmOverlay = overlayForm;

            SuspendLayout();
            // change close graphics
            ilist8.Add(Properties.Resources.Button_QUADRANTCANCEL);
            ilist8.Add(Properties.Resources.Button_QUADRANTCANCEL);
            ilist8.Add(Properties.Resources.Button_QUADRANTCANCEL);
            ilist8.Add(Properties.Resources.Button_QUADRANTCANCEL);
            button_Cancel1.ChangeGraphics(ilist8);

            // set shift and caps to toggle buttons
            buttonCaps.setToggle(true);
            buttonShift.setToggle(true);

            ResumeLayout();

            // LOG
            string logMSG = "New Touch Keyboard generated";
            //GUI_Controls.uiLog.LOG(this, "Keyboard", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);             
        }

        private void Keyboard_Load(object sender, EventArgs e)
        {
            button_Cancel1.BringToFront();
            textbox_input.Focus();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            textbox_input.Text = "";
        }

        private void buttonDel_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            SendKeys.Send("{BS}");
        }

        

        private void buttonCaps_CheckedChanged(object sender, EventArgs e)
        {
            if (buttonShift.Check)
            {
                buttonShift.Check = false;
                // change state image
            }
            if (buttonCaps.Check)
            {
                // change CAP button image state
            }
            else
            {
                // change CAP button image state
            }
        }

        private void buttonShift_CheckedChanged(object sender, EventArgs e)
        {
            if (buttonCaps.Check)
            {
                buttonShift.Check = false;
            }
            if (buttonShift.Check)
            {
                // change Shift button image state
            }
            else
            {
                // change Shift button image state
            }
        }

        private void buttonEnter_Click(object sender, EventArgs e)
        {
            lastControl.Text = textbox_input.Text;
            if (frmOverlay != null)
                frmOverlay.Hide();

            if (PreviousForm != null)
                PreviousForm.Activate();

            this.DialogResult = DialogResult.OK;
            this.Hide();

            // LOG
            string logMSG = "Keyboard output: '" + textbox_input.Text + "'";
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);             
        }

        #region KeyStrokes
        private void button_space_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            SendKeys.Send(" ");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            SendKeys.Send("1");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            SendKeys.Send("2");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            SendKeys.Send("3");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            SendKeys.Send("4");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            SendKeys.Send("5");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            SendKeys.Send("6");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            SendKeys.Send("7");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            SendKeys.Send("8");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            SendKeys.Send("9");
        }

        private void button0_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            SendKeys.Send("0");
        }

        private void buttonQ_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.Check || buttonShift.Check)
            {
                SendKeys.Send("Q");
                buttonShift.Check = false;
            }
            else
            {
                SendKeys.Send("q");
            }
        }

        private void buttonW_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.Check || buttonShift.Check)
            {
                SendKeys.Send("W");
                buttonShift.Check = false;
            }
            else
            {
                SendKeys.Send("w");
            }
        }

        private void buttonE_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.Check || buttonShift.Check)
            {
                SendKeys.Send("E");
                buttonShift.Check = false;
            }
            else
            {
                SendKeys.Send("e");
            }
        }

        private void buttonR_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.Check || buttonShift.Check)
            {
                SendKeys.Send("R");
                buttonShift.Check = false;
            }
            else
            {
                SendKeys.Send("r");
            }
        }

        private void buttonT_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.Check || buttonShift.Check)
            {
                SendKeys.Send("T");
                buttonShift.Check = false;
            }
            else
            {
                SendKeys.Send("t");
            }
        }

        private void buttonY_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.Check || buttonShift.Check)
            {
                SendKeys.Send("Y");
                buttonShift.Check = false;
            }
            else
            {
                SendKeys.Send("y");
            }
        }

        private void buttonU_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.Check || buttonShift.Check)
            {
                SendKeys.Send("U");
                buttonShift.Check = false;
            }
            else
            {
                SendKeys.Send("u");
            }
        }

        private void buttonI_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.Check || buttonShift.Check)
            {
                SendKeys.Send("I");
                buttonShift.Check = false;
            }
            else
            {
                SendKeys.Send("i");
            }
        }

        private void buttonO_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.Check || buttonShift.Check)
            {
                SendKeys.Send("O");
                buttonShift.Check = false;
            }
            else
            {
                SendKeys.Send("o");
            }
        }

        private void buttonP_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.Check || buttonShift.Check)
            {
                SendKeys.Send("P");
                buttonShift.Check = false;
            }
            else
            {
                SendKeys.Send("p");
            }
        }

        private void buttonA_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.Check || buttonShift.Check)
            {
                SendKeys.Send("A");
                buttonShift.Check = false;
            }
            else
            {
                SendKeys.Send("a");
            }
        }

        private void buttonS_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.Check || buttonShift.Check)
            {
                SendKeys.Send("S");
                buttonShift.Check = false;
            }
            else
            {
                SendKeys.Send("s");
            }
        }

        private void buttonD_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.Check || buttonShift.Check)
            {
                SendKeys.Send("D");
                buttonShift.Check = false;
            }
            else
            {
                SendKeys.Send("d");
            }
        }

        private void buttonF_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.Check || buttonShift.Check)
            {
                SendKeys.Send("F");
                buttonShift.Check = false;
            }
            else
            {
                SendKeys.Send("f");
            }
        }

        private void buttonG_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.Check || buttonShift.Check)
            {
                SendKeys.Send("G");
                buttonShift.Check = false;
            }
            else
            {
                SendKeys.Send("g");
            }
        }

        private void buttonH_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.Check || buttonShift.Check)
            {
                SendKeys.Send("H");
                buttonShift.Check = false;
            }
            else
            {
                SendKeys.Send("h");
            }
        }

        private void buttonJ_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.Check || buttonShift.Check)
            {
                SendKeys.Send("J");
                buttonShift.Check = false;
            }
            else
            {
                SendKeys.Send("j");
            }
        }

        private void buttonK_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.Check || buttonShift.Check)
            {
                SendKeys.Send("K");
                buttonShift.Check = false;
            }
            else
            {
                SendKeys.Send("k");
            }
        }

        private void buttonL_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.Check || buttonShift.Check)
            {
                SendKeys.Send("L");
                buttonShift.Check = false;
            }
            else
            {
                SendKeys.Send("l");
            }
        }

        private void buttonZ_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.Check || buttonShift.Check)
            {
                SendKeys.Send("Z");
                buttonShift.Check = false;
            }
            else
            {
                SendKeys.Send("z");
            }
        }

        private void buttonX_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.Check || buttonShift.Check)
            {
                SendKeys.Send("X");
                buttonShift.Check = false;
            }
            else
            {
                SendKeys.Send("x");
            }
        }

        private void buttonC_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.Check || buttonShift.Check)
            {
                SendKeys.Send("C");
                buttonShift.Check = false;
            }
            else
            {
                SendKeys.Send("c");
            }
        }

        private void buttonV_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.Check || buttonShift.Check)
            {
                SendKeys.Send("V");
                buttonShift.Check = false;
            }
            else
            {
                SendKeys.Send("v");
            }
        }

        private void buttonB_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.Check || buttonShift.Check)
            {
                SendKeys.Send("B");
                buttonShift.Check = false;
            }
            else
            {
                SendKeys.Send("b");
            }
        }

        private void buttonN_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.Check || buttonShift.Check)
            {
                SendKeys.Send("N");
                buttonShift.Check = false;
            }
            else
            {
                SendKeys.Send("n");
            }
        }

        private void buttonM_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.Check || buttonShift.Check)
            {
                SendKeys.Send("M");
                buttonShift.Check = false;
            }
            else
            {
                SendKeys.Send("m");
            }
        }

        private void buttonDot_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            SendKeys.Send(".");
        }

        private void button__Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            SendKeys.Send("_");
        }
        #endregion

        private void buttonShift_Click(object sender, EventArgs e)
        {
            //if (!buttonCaps.Check)
            //{
            //    buttonShift.Check = true;
            //}
        }

        private void buttonCaps_Click(object sender, EventArgs e)
        {
            if (!buttonCaps.Check)
            {

            }
            else
            {
                buttonCaps.Check = true;
                buttonShift.Check = false;
            }
        }

        private void textbox_input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                lastControl.Text = textbox_input.Text;
                if (frmOverlay != null)
                    frmOverlay.Hide();

                this.Close();

                if (PreviousForm != null)
                    PreviousForm.Activate();

                this.DialogResult = DialogResult.OK;

                // LOG
                string logMSG = "Keboard output: '" + textbox_input.Text + "'";
                //GUI_Controls.uiLog.LOG(this, "textbox_input_KeyDown", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
                //  (logMSG);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);             
            }
        }

        private void button_Cancel1_Click(object sender, EventArgs e)
        {
            if (frmOverlay != null)
                frmOverlay.Hide();

            //this.Close();
            this.DialogResult = DialogResult.Cancel;
            this.Hide();

            // LOG
            string logMSG = "Cancel button clicked" ;
            //GUI_Controls.uiLog.LOG(this, "button_Cancel1_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);             
        }

        private void Keyboard_LocationChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                if (this.frmOverlay != null)
                    this.frmOverlay.BringToFront();

                this.BringToFront();
            }
        }

        private void Keyboard_Shown(object sender, EventArgs e)
        {
            this.BringToFront();
        }

        private void Keyboard_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                this.SendToBack();
            }
            else
                this.textbox_input.Focus();
        }

    }
}
