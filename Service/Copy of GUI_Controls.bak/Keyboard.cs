using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Xml;
using System.Text;
using System.Windows.Forms;

namespace GUI_Controls
{
    public partial class Keyboard : Form
    {
        private Control lastControl;
        public Form frmOverlay;
        private Form PreviousForm;
        public Point overlayOffset = new Point(4, 26);
        public Point Offset;

        public Keyboard(Form previousform, Control previousControl, AutoCompleteStringCollection source, Form overlayForm)
        {
            InitializeComponent();
            textbox_input.Text = previousControl.Text;
            lastControl = previousControl;
            PreviousForm = previousform;
            textbox_input.AutoCompleteCustomSource = source;
            Offset = new Point((previousform.Size.Width - this.Size.Width) / 2, 150);
            this.Location = new Point(previousform.Location.X + Offset.X, previousform.Location.Y + Offset.Y);
            frmOverlay = overlayForm;

            SuspendLayout();
            // change close graphics
            List<Image> ilist = new List<Image>();
            ilist.Add(Properties.Resources.Button_QUADRANTCANCEL);
            ilist.Add(Properties.Resources.Button_QUADRANTCANCEL);
            ilist.Add(Properties.Resources.Button_QUADRANTCANCEL);
            ilist.Add(Properties.Resources.Button_QUADRANTCANCEL);
            button_Cancel1.ChangeGraphics(ilist);

            // set shift and caps to toggle buttons
            buttonCaps.setToggle(true);
            buttonShift.setToggle(true);

            this.Region = roboPanel1.Region;

            ResumeLayout();

            // LOG
            string logMSG = "New Touch Keyboard generated w/ Autocomplete";
            GUI_Controls.uiLog.LOG(this, "Keyboard", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        public Keyboard(Form previousform, Control previousControl, Form overlayForm)
        {
            InitializeComponent();
            textbox_input.Text = previousControl.Text;
            lastControl = previousControl;
            PreviousForm = previousform;
            Offset = new Point((previousform.Size.Width - this.Size.Width) / 2, 150);
            this.Location = new Point(previousform.Location.X + Offset.X, previousform.Location.Y + Offset.Y);
            frmOverlay = overlayForm;

            SuspendLayout();
            // change close graphics
            List<Image> ilist = new List<Image>();
            ilist.Add(Properties.Resources.Button_QUADRANTCANCEL);
            ilist.Add(Properties.Resources.Button_QUADRANTCANCEL);
            ilist.Add(Properties.Resources.Button_QUADRANTCANCEL);
            ilist.Add(Properties.Resources.Button_QUADRANTCANCEL);
            button_Cancel1.ChangeGraphics(ilist);

            // set shift and caps to toggle buttons
            buttonCaps.setToggle(true);
            buttonShift.setToggle(true);

            this.Region = roboPanel1.Region;

            ResumeLayout();

            // LOG
            string logMSG = "New Touch Keyboard generated";
            GUI_Controls.uiLog.LOG(this, "Keyboard", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

            this.TopMost = true;
        }

        private void Keyboard_Load(object sender, EventArgs e)
        {
            button_Cancel1.BringToFront();

            /* Load overlay so that background is unselectable
            // create form to overlay
            overlay = new Form();
            overlay.Size = new Size(640, 480);
            overlay.BackColor = Color.Black;
            overlay.Opacity = 0.6;
            overlay.Location = new Point(PreviousForm.Location.X + overlayOffset.X, PreviousForm.Location.Y + overlayOffset.Y);
            overlay.StartPosition = this.StartPosition;
            overlay.FormBorderStyle = FormBorderStyle.None;
            overlay.Enabled = false;
            overlay.ShowInTaskbar = false;

            // create rectangle for overlay screen
            Microsoft.VisualBasic.PowerPacks.ShapeContainer canvas = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            canvas.Parent = overlay;
            Microsoft.VisualBasic.PowerPacks.RectangleShape rec = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            rec.Size = new Size(640, 480);
            rec.FillColor = Color.FromArgb(40, 10, 40);
            rec.BackColor = Color.Blue;
            rec.FillStyle = Microsoft.VisualBasic.PowerPacks.FillStyle.Percent25;
            rec.Location = new Point(0, 0);
            rec.Parent = canvas;
            overlay.Show();

            this.BringToFront();*/

                    
        }

        private void button10_Click(object sender, EventArgs e)
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
            if (buttonShift.check)
            {
                buttonShift.check = false;
                // change state image
            }
            if (buttonCaps.check)
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
            if (buttonCaps.check)
            {
                buttonShift.check = false;
            }
            if (buttonShift.check)
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
            frmOverlay.Hide();
            this.Close();
            PreviousForm.Activate();

            // LOG
            string logMSG = "Keboard output: '" + textbox_input.Text + "'";
            GUI_Controls.uiLog.LOG(this, "buttonEnter_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
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
            if (buttonCaps.check || buttonShift.check)
            {
                SendKeys.Send("Q");
                buttonShift.check = false;
            }
            else
            {
                SendKeys.Send("q");
            }
        }

        private void buttonW_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.check || buttonShift.check)
            {
                SendKeys.Send("W");
                buttonShift.check = false;
            }
            else
            {
                SendKeys.Send("w");
            }
        }

        private void buttonE_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.check || buttonShift.check)
            {
                SendKeys.Send("E");
                buttonShift.check = false;
            }
            else
            {
                SendKeys.Send("e");
            }
        }

        private void buttonR_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.check || buttonShift.check)
            {
                SendKeys.Send("R");
                buttonShift.check = false;
            }
            else
            {
                SendKeys.Send("r");
            }
        }

        private void buttonT_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.check || buttonShift.check)
            {
                SendKeys.Send("T");
                buttonShift.check = false;
            }
            else
            {
                SendKeys.Send("t");
            }
        }

        private void buttonY_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.check || buttonShift.check)
            {
                SendKeys.Send("Y");
                buttonShift.check = false;
            }
            else
            {
                SendKeys.Send("y");
            }
        }

        private void buttonU_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.check || buttonShift.check)
            {
                SendKeys.Send("U");
                buttonShift.check = false;
            }
            else
            {
                SendKeys.Send("u");
            }
        }

        private void buttonI_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.check || buttonShift.check)
            {
                SendKeys.Send("I");
                buttonShift.check = false;
            }
            else
            {
                SendKeys.Send("i");
            }
        }

        private void buttonO_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.check || buttonShift.check)
            {
                SendKeys.Send("O");
                buttonShift.check = false;
            }
            else
            {
                SendKeys.Send("o");
            }
        }

        private void buttonP_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.check || buttonShift.check)
            {
                SendKeys.Send("P");
                buttonShift.check = false;
            }
            else
            {
                SendKeys.Send("p");
            }
        }

        private void buttonA_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.check || buttonShift.check)
            {
                SendKeys.Send("A");
                buttonShift.check = false;
            }
            else
            {
                SendKeys.Send("a");
            }
        }

        private void buttonS_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.check || buttonShift.check)
            {
                SendKeys.Send("S");
                buttonShift.check = false;
            }
            else
            {
                SendKeys.Send("s");
            }
        }

        private void buttonD_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.check || buttonShift.check)
            {
                SendKeys.Send("D");
                buttonShift.check = false;
            }
            else
            {
                SendKeys.Send("d");
            }
        }

        private void buttonF_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.check || buttonShift.check)
            {
                SendKeys.Send("F");
                buttonShift.check = false;
            }
            else
            {
                SendKeys.Send("f");
            }
        }

        private void buttonG_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.check || buttonShift.check)
            {
                SendKeys.Send("G");
                buttonShift.check = false;
            }
            else
            {
                SendKeys.Send("g");
            }
        }

        private void buttonH_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.check || buttonShift.check)
            {
                SendKeys.Send("H");
                buttonShift.check = false;
            }
            else
            {
                SendKeys.Send("h");
            }
        }

        private void buttonJ_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.check || buttonShift.check)
            {
                SendKeys.Send("J");
                buttonShift.check = false;
            }
            else
            {
                SendKeys.Send("j");
            }
        }

        private void buttonK_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.check || buttonShift.check)
            {
                SendKeys.Send("K");
                buttonShift.check = false;
            }
            else
            {
                SendKeys.Send("k");
            }
        }

        private void buttonL_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.check || buttonShift.check)
            {
                SendKeys.Send("L");
                buttonShift.check = false;
            }
            else
            {
                SendKeys.Send("l");
            }
        }

        private void buttonZ_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.check || buttonShift.check)
            {
                SendKeys.Send("Z");
                buttonShift.check = false;
            }
            else
            {
                SendKeys.Send("z");
            }
        }

        private void buttonX_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.check || buttonShift.check)
            {
                SendKeys.Send("X");
                buttonShift.check = false;
            }
            else
            {
                SendKeys.Send("x");
            }
        }

        private void buttonC_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.check || buttonShift.check)
            {
                SendKeys.Send("C");
                buttonShift.check = false;
            }
            else
            {
                SendKeys.Send("c");
            }
        }

        private void buttonV_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.check || buttonShift.check)
            {
                SendKeys.Send("V");
                buttonShift.check = false;
            }
            else
            {
                SendKeys.Send("v");
            }
        }

        private void buttonB_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.check || buttonShift.check)
            {
                SendKeys.Send("B");
                buttonShift.check = false;
            }
            else
            {
                SendKeys.Send("b");
            }
        }

        private void buttonN_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.check || buttonShift.check)
            {
                SendKeys.Send("N");
                buttonShift.check = false;
            }
            else
            {
                SendKeys.Send("n");
            }
        }

        private void buttonM_Click(object sender, EventArgs e)
        {
            ActiveControl = textbox_input;
            if (buttonCaps.check || buttonShift.check)
            {
                SendKeys.Send("M");
                buttonShift.check = false;
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
            if (!buttonCaps.check)
            {
                buttonShift.check = true;
            }
        }

        private void buttonCaps_Click(object sender, EventArgs e)
        {
            if (buttonCaps.check)
            {
                buttonCaps.check = false;
            }
            else
            {
                buttonCaps.check = true;
                buttonShift.check = false;
            }
        }

        private void Keyboard_Deactivate(object sender, EventArgs e)
        {
            frmOverlay.BringToFront();
            this.BringToFront();
            this.Activate();
        }

        private void textbox_input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                lastControl.Text = textbox_input.Text;
                frmOverlay.Hide();
                this.Close();
                PreviousForm.Activate();

                // LOG
                string logMSG = "Keboard output: '" + textbox_input.Text + "'";
                GUI_Controls.uiLog.LOG(this, "textbox_input_KeyDown", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            }
        }

        private void button_Cancel1_Click(object sender, EventArgs e)
        {
            frmOverlay.Hide();
            this.Close();

            // LOG
            string logMSG = "Cancel button clicked" ;
            GUI_Controls.uiLog.LOG(this, "button_Cancel1_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        private void Keyboard_LocationChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                this.frmOverlay.BringToFront();
                this.BringToFront();
            }
        }
    }
}
