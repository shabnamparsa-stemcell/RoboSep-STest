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
        private int DeltaYBetweenControls = 0;
        private int LeftMargin = 0;
        private int RightMargin= 0;
        private int DeltaYBetweenButtonAndForm = 0;
        private int DeltaXBetweenButtonAndForm = 0;
        private int DeltaXBetweenButtons = 0;

        bool button1Visible = true;
        bool button2Visible = false;

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

            // set up buttons
            button1Visible = false;
            button2Visible = false;
            button1.Visible = false;
            button2.Visible = false;

            GetSpacings();

            // set size of form based on amount of text
            this.Size = determineSize();

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

            // turn 2nd button on?
            if (b2)
            {
                button2.Enabled = true;
                button2.Visible = true;
                button2Visible = true;
            }

            GetSpacings();
            this.Size = determineSize();

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

        protected Size determineSize()
        {
            int width = circularProgressControl1.Bounds.Width;
            int minHeight = 100;
            int hpadding = 50;
            int maxHeight = 450 - hpadding;

            // Start from the progress control
            int Y = circularProgressControl1.Bounds.Y;
            Y += circularProgressControl1.Bounds.Height;

            int X = this.Bounds.Width;
            int maxButtonHeight = 0, XDelta = 0;

            //button1 
            if (button1Visible == true)
            {
                X -= (DeltaXBetweenButtonAndForm + button1.Bounds.Width);
                button1.SetBounds(X, Y, button1.Bounds.Width, button1.Bounds.Height);
                maxButtonHeight = button1.Bounds.Height;
            }

            //button2 
            if (button2Visible == true)
            {
                XDelta = button1Visible ? DeltaXBetweenButtons : DeltaXBetweenButtonAndForm;
                X -= (XDelta + button2.Bounds.Width);
                button2.SetBounds(X, Y, button2.Bounds.Width, button2.Bounds.Height);
                maxButtonHeight = Math.Max(maxButtonHeight, button2.Bounds.Height);
            }

            Y += maxButtonHeight;
            Y += DeltaYBetweenButtonAndForm;

            if (Y < minHeight)
                Y = minHeight;

            if (Y > maxHeight)
                Y = maxHeight;


            System.Diagnostics.Debug.WriteLine(String.Format("Dialog size: width = {0}, height = {1}", width + hpadding, Y));
            return new Size(width + LeftMargin + RightMargin, Y);
        }

        private void GetSpacings()
        {
            LeftMargin = circularProgressControl1.Bounds.X - this.Bounds.X;
            RightMargin = this.Bounds.X + this.Width - (circularProgressControl1.Bounds.X + circularProgressControl1.Width);
            DeltaYBetweenControls = button1.Bounds.Y - (circularProgressControl1.Bounds.Y + circularProgressControl1.Bounds.Height);
            XSpacingsBetweenButtons = button1.Bounds.X - (button2.Bounds.X + button2.Width);
            RightMargin = this.Bounds.X + this.Width - (button1.Bounds.X + button1.Width);
            DeltaYBetweenButtonAndForm = this.Bounds.Y + this.Height - (button1.Bounds.Y + button1.Height);
            DeltaXBetweenButtons = button1.Bounds.X - (button2.Bounds.X + button2.Bounds.Width);
            DeltaXBetweenButtonAndForm = this.Bounds.X + this.Bounds.Width - (button1.Bounds.X + button1.Bounds.Width);
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

            circularProgressControl1.StartAngle = 30;
            circularProgressControl1.Start();

            this.BringToFront();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (circularProgressControl1.NumberOfRotation > 1)
            {
                timer1.Stop();
                this.DialogResult = DialogResult.OK;
                this.Hide();
                this.Close();
                return;
            }
        }
    }
}
