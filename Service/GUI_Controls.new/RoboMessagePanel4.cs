using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GUI_Controls
{
    public partial class RoboMessagePanel4 : Form
    {
        protected Form PreviousForm;
        private const int minTimerPeriod = 200;        // in msec
        private const int minStep = 10;        // in msec
 
        public Point Offset;

        protected string FormTitle = "RoboSep Message";
        private int XSpacingsBetweenButtons = 0;
        private int RightMargin= 0;
        private int DeltaYBetweenControlAndButton = 0;
        private int DeltaYBetweenButtonAndForm = 0;
        private int ProgressAngle = 0;
        private int ProgressStep = 20;


        public RoboMessagePanel4(Form PrevForm, string Title, int WaitPeriodInMsec, int Step)
        {
            InitializeComponent();

            SuspendLayout();

            // set previous form
            PreviousForm = PrevForm;

            // set message to given message
            FormTitle = Title;

            // set step
            if (minStep <= Step)
            {
                ProgressStep = Step;
            }

            // set time period
            if (minTimerPeriod <= WaitPeriodInMsec)
            {
                timer1.Interval = WaitPeriodInMsec / ProgressStep;
            }

            // set up buttons
            button1.Visible = false;
            button2.Visible = false;

            // set offset
            Offset = new Point((PreviousForm.Size.Width - this.Width) / 2, (PreviousForm.Size.Height - this.Size.Height) / 2 + 25);

            SetDialogProperties();
            ResumeLayout();

            timer1.Start();
       }

        public RoboMessagePanel4(Form PrevForm, string Title, int WaitPeriodInMsec, int Step, bool b2)
        {
            InitializeComponent();
            SuspendLayout();

            // set previous form
            PreviousForm = PrevForm;

            // set window title
            FormTitle = Title;

            // set step
            if (minStep <= Step)
            {
                ProgressStep = Step;
            }

            // set time period
            if (minTimerPeriod <= WaitPeriodInMsec)
            {
                timer1.Interval = WaitPeriodInMsec / ProgressStep;
            }

            // turn 2nd button on?
            if (b2)
            {
                button2.Enabled = true;
                button2.Visible = true;
            }

            // set offset
            Offset = new Point((PreviousForm.Size.Width - this.Width) / 2, (PreviousForm.Size.Height - this.Size.Height) / 2 + 25);
                        
            SetDialogProperties();
            ResumeLayout();
        }

        public void closeDialogue(DialogResult givenResult)
        {
            this.DialogResult = givenResult;
        }

        private void SetDialogProperties()
        {
            // set text components
            label_WindowTitle.Text = FormTitle;
            this.TopMost = true;

            // set position
            this.Location = new Point((PreviousForm.Location.X + this.Offset.X), (PreviousForm.Location.Y + this.Offset.Y));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ProgressAngle += 360 / ProgressStep;
            if (360 < ProgressAngle)
            {
                timer1.Stop();

                this.DialogResult = DialogResult.OK;
                this.Hide();
                return;
            }

            progressCircle1.Value = ProgressAngle;
        }

        private void GetSpacings()
        {
            XSpacingsBetweenButtons = button1.Bounds.X - (button2.Bounds.X + button2.Width);
            RightMargin = this.Bounds.X + this.Width - (button1.Bounds.X + button1.Width);
            DeltaYBetweenButtonAndForm = this.Bounds.Y + this.Height - (button1.Bounds.Y + button1.Height);
        }

        private void ResizeButtons()
        {
            SuspendLayout();

            string b1Text = button1.Text;
            string b2Text = button2.Text;

            Rectangle rcButton1 = button1.Bounds;
            Rectangle rcButton2 = button2.Bounds;

            int nWidthButton1 = -1;
            int nWidthButton2 = -1;
            if (button1.Visible == true && !string.IsNullOrEmpty(b1Text))
            {
                nWidthButton1 = button1.determineWidth(b1Text);
            }

            if (button2.Visible == true && !string.IsNullOrEmpty(b2Text))
            {
                nWidthButton2 = button2.determineWidth(b2Text);
            }

            if (button1.Visible && button2.Visible)
            {
                int nMaxButtonWidth = Math.Max(nWidthButton1, nWidthButton2);
                int nX1 = this.Size.Width - nMaxButtonWidth - RightMargin;
                int nX2 = nX1 - XSpacingsBetweenButtons - nMaxButtonWidth;
                button1.AutoSize = false;
                button2.AutoSize = false;
                button1.SetBounds(nX1, rcButton1.Y, nMaxButtonWidth, rcButton1.Height);
                button2.SetBounds(nX2, rcButton2.Y, nMaxButtonWidth, rcButton2.Height);
                button1.SetImageWidth(nMaxButtonWidth);
                button2.SetImageWidth(nMaxButtonWidth);
            }
            else if (button1.Visible && !button2.Visible)
            {
                if (0 < nWidthButton1)
                {
                    button1.AutoSize = false;
                    int nX1 = this.Size.Width - nWidthButton1 - RightMargin;
                    button1.SetBounds(nX1, rcButton1.Y, nWidthButton1, rcButton1.Height);
                    button1.SetImageWidth(nWidthButton1);
                }
            }
            else if (!button1.Visible && button2.Visible)
            {
                if (0 < nWidthButton2)
                {
                    button2.AutoSize = false;
                    int nX2 = this.Bounds.X + this.Bounds.Width - nWidthButton2 - RightMargin;
                    button2.SetBounds(nX2, rcButton2.Y, nWidthButton2, rcButton2.Height);
                    button2.SetImageWidth(nWidthButton2);
                }
            }
            else
            {
                // do nothing
            }

            ResumeLayout();
        }

        private void RoboMessagePanel4_RegionChanged(object sender, EventArgs e)
        {
            // change control label to name of Form
            label_WindowTitle.Text = this.Text;
        }

        private void button_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RoboMessagePanel4_Deactivate(object sender, EventArgs e)
        {
            this.BringToFront();
            this.Activate();
        }

        private void txtMessage_Enter(object sender, EventArgs e)
        {
            ActiveControl = panel1;
        }

        private void RoboMessagePanel4_Leave(object sender, EventArgs e)
        {
            this.Activate();
            this.BringToFront();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // OK button
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Cancel button
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        private void RoboMessagePanel4_Load(object sender, EventArgs e)
        {
            //change toggle button graphics
            List<Image> ilist = new List<Image>();
            ilist.Add(Properties.Resources.btnsmall_STD);
            ilist.Add(Properties.Resources.btnsmall_STD);
            ilist.Add(Properties.Resources.btnsmall_STD);
            ilist.Add(Properties.Resources.btnsmall_CLICK);
            button1.ChangeGraphics(ilist);
            button2.ChangeGraphics(ilist);
            ResizeButtons();
            button2.ChangeGraphics(ilist);
            this.BringToFront();
        }
    }
}
