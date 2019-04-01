using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Xml;
using System.Text;
using System.Windows.Forms;

namespace GUI_Controls
{
    public enum MessageIcon
    {
        MBICON_NOTAPPLICABLE,
        MBICON_WARNING,
        MBICON_INFORMATION,
        MBICON_QUESTION,
        MBICON_ERROR,
        MBICON_WAIT
    }

    public partial class RoboMessagePanel : Form
    {
        protected Form PreviousForm;
        public Point Offset;
        private const int GRAPHICSIZE = 19;
        private const int TOPGRAPHIC_HEIGHT = 35;
        private const int BORDERWIDTH = 5;
        protected string FormTitle = "RoboSep Message";
        protected string message = "";
        private bool refreshed = false;

        private int YOffset = 25;
        private int LeftMessageMargin = 0;
        private int LeftMargin = 0;
        private int RightMargin = 0;
        private int DeltaYBetweenControls = 0;
        private int DeltaYBetweenButtonAndForm = 0;
        private int DeltaXBetweenButtons = 0;
        private int DeltaXBetweenButtonAndForm = 0;

        bool button1Visible = false;
        bool button2Visible = false;
        bool bShowProgressBar = false;

        List<Image> ilist1 = new List<Image>();
        List<Image> ilist2 = new List<Image>();

        MessageIcon msgIcon = MessageIcon.MBICON_INFORMATION;

        public RoboMessagePanel(Form PrevForm, MessageIcon msgIcon, string Message, string Title)
        {
            InitializeComponent();
            SuspendLayout();

            this.DoubleBuffered = true;
            this.UpdateStyles();

            GetSpacings();
         
            SetButton1Visible(true);
            SetButton2Visible(false);

            // set previous form
            PreviousForm = PrevForm;
            this.msgIcon = msgIcon;

            // Set Form title
            FormTitle = Title;

            // set size of form based on amount of text
            this.Size = determineSize(Message);

            // set message to given message
            message = Message;

            // set up buttons
            button_2.Visible = false;
            button_1.Text = "OK";


            int nYOffSet = (PreviousForm.Size.Height - this.Size.Height) / 2 + YOffset;
            if (nYOffSet + this.Size.Height > PreviousForm.Size.Height)
            {
                nYOffSet = PreviousForm.Size.Height - this.Size.Height - 2;
            }

            // set offset
            Offset = new Point((PreviousForm.Size.Width - this.Width) / 2, nYOffSet);            
                        
            SetDialogProperties();
            ResumeLayout();
        }

        public RoboMessagePanel(Form PrevForm, MessageIcon msgIcon, string Message, string Title, string OkButton)
        {
            InitializeComponent();
            SuspendLayout();

            this.DoubleBuffered = true;
            this.UpdateStyles();

            GetSpacings();

            SetButton1Visible(true);
            SetButton2Visible(false);

            // set previous form
            PreviousForm = PrevForm;
            this.msgIcon = msgIcon;

            // set size of form based on amount of text
            this.Size = determineSize(Message);

            // set message to given message
            message = Message;

            // Set Form title
            FormTitle = Title;

            // set up buttons
          //  button_2.Visible = false;

            button_1.Text = OkButton;

            int nYOffSet = (PreviousForm.Size.Height - this.Size.Height) / 2 + YOffset;
            if (nYOffSet + this.Size.Height > PreviousForm.Size.Height)
            {
                nYOffSet = PreviousForm.Size.Height - this.Size.Height - 2;
            }

            // set offset
            Offset = new Point((PreviousForm.Size.Width - this.Width) / 2, nYOffSet);
            
            SetDialogProperties();
            ResumeLayout();
        }

        public RoboMessagePanel(Form PrevForm, MessageIcon msgIcon, string Message, string Title, string b1, string b2)
        {
            InitializeComponent();
            SuspendLayout();

            this.DoubleBuffered = true;
            this.UpdateStyles();

            GetSpacings();

            SetButton1Visible(true);

            // set main button text
            button_1.Text = b1;
            // turn 2nd button on?
            if (b2 != string.Empty)
            {
                button_2.Enabled = true;
                SetButton2Visible(true);
            }
            // set 2nd button text
            button_2.Text = b2;

            // set previous form
            PreviousForm = PrevForm;

            this.msgIcon = msgIcon;

            // set size of form based on amount of text
            this.Size = determineSize(Message);

            // set message
            message = Message;
            // set window title
            FormTitle = Title;

            int nYOffSet = (PreviousForm.Size.Height - this.Size.Height) / 2 + YOffset;
            if (nYOffSet + this.Size.Height > PreviousForm.Size.Height)
            {
                nYOffSet = PreviousForm.Size.Height - this.Size.Height - 2;
            }

            // set offset
            Offset = new Point((PreviousForm.Size.Width - this.Width) / 2, nYOffSet);
            SetDialogProperties();

            ResumeLayout();
        }

        public RoboMessagePanel(Form PrevForm, MessageIcon msgIcon, string Message, bool isWaitDialog, bool showProgress)
        {
            // message box option where there are no buttons, and response will come from system.
            // just displays a message until receives propt to continue
            InitializeComponent();
            // set previous form
            PreviousForm = PrevForm;
            this.msgIcon = msgIcon;
            bShowProgressBar = showProgress;
            SuspendLayout();

            this.DoubleBuffered = true;
            this.UpdateStyles();

            GetSpacings();

            SetButton1Visible(false);
            SetButton2Visible(false);

            // set up progress bar
            if (bShowProgressBar)
            {
                MSGprogress.Visible = true;
                MSGprogress.Maximum = 100;
                MSGprogress.Minimum = 0;
                MSGprogress.Step = 1;
                MSGprogress.Style = ProgressBarStyle.Continuous;
                this.msgIcon = MessageIcon.MBICON_NOTAPPLICABLE;
            }
            else
            {
                MSGprogress.Visible = false;
            }
            // set size of form based on amount of text
            this.Size = determineSize(Message);

            // set message to given message
            message = Message;
            // set up buttons
         //   button_2.Visible = false;
          //  button_1.Visible = false;

            button_1.Text = "OK";


            int nYOffSet = (PreviousForm.Size.Height - this.Size.Height) / 2 + YOffset;
            if (nYOffSet + this.Size.Height > PreviousForm.Size.Height)
            {
                nYOffSet = PreviousForm.Size.Height - this.Size.Height - 2;
            }

            // set offset
            Offset = new Point((PreviousForm.Size.Width - this.Width) / 2, nYOffSet);

            SetDialogProperties();
            ResumeLayout();
        }

        public RoboMessagePanel(Form PrevForm, MessageIcon msgIcon, string Message, string Title, bool isWaitDialog, bool showProgress)
        {
            // message box option where there are no buttons, and response will come from system.
            // just displays a message until receives propt to continue
            InitializeComponent();
            // set previous form
            PreviousForm = PrevForm;
            this.msgIcon = msgIcon;
            bShowProgressBar = showProgress;
            SuspendLayout();

            this.DoubleBuffered = true;
            this.UpdateStyles();

            GetSpacings();

            SetButton1Visible(false);
            SetButton2Visible(false);

            // set up progress bar
            if (bShowProgressBar)
            {
                MSGprogress.Visible = true;
                MSGprogress.Maximum = 100;
                MSGprogress.Minimum = 0;
                MSGprogress.Step = 1;
                MSGprogress.Style = ProgressBarStyle.Continuous;
                this.msgIcon = MessageIcon.MBICON_NOTAPPLICABLE;
            }
            else
            {
                MSGprogress.Visible = false;
            }
            // set size of form based on amount of text
            this.Size = determineSize(Message);
  
            if (!string.IsNullOrEmpty(Title))
                FormTitle = Title;

            // set message to given message
            message = Message;

            // set up buttons
         //   button_2.Visible = false;
         //   button_1.Visible = false;

            int nYOffSet = (PreviousForm.Size.Height - this.Size.Height) / 2 + YOffset;
            if (nYOffSet + this.Size.Height > PreviousForm.Size.Height)
            {
                nYOffSet = PreviousForm.Size.Height - this.Size.Height - 2;
            }

            // set offset
            Offset = new Point((PreviousForm.Size.Width - this.Width) / 2, nYOffSet);

            SetDialogProperties();

            ResumeLayout();
        }

        // so class can be inherited to create
        // quadrant selection dialog
        public RoboMessagePanel()
        {
            InitializeComponent();

            SuspendLayout();

            this.DoubleBuffered = true;
            this.UpdateStyles();

            GetSpacings();

            SetButton1Visible(true);
            SetButton2Visible(true);

            PreviousForm = null;
            Offset = new Point(0, 0);
            FormTitle = "Quadrant Reagent Sharing";

           // button_2.Visible = true;

            button_2.Text = "Cancel";
            button_1.Text = "Done";

            ResumeLayout();
        }
        public void SetTitle(string sTitle)
        {
            if (string.IsNullOrEmpty(sTitle))
                return;

            FormTitle = sTitle; ;
            this.Refresh();
        }
        public void SetMessage(string sMsg)
        {
            if (string.IsNullOrEmpty(sMsg))
                return;

            txtMessage.Text = sMsg;
            this.Refresh();
        }

        public void SetIcon(MessageIcon msgIcon)
        {
            this.msgIcon = msgIcon;
            this.Refresh();
        }

        public void SetButton1Visible(bool bVisible)
        {
            button1Visible = bVisible;
            button_1.Visible = button1Visible;
        }

        public void SetButton2Visible(bool bVisible)
        {
            button2Visible = bVisible;
            button_2.Visible = button2Visible;
        }

        public void closeDialogue(DialogResult givenResult)
        {
            this.DialogResult = givenResult;
        }

        public bool Refreshed
        {
            get { return refreshed; }

            set { refreshed = value; }
        }

        private void SetDialogProperties()
        {
            // set text components
            label_WindowTitle.Text = FormTitle;
            txtMessage.Text = message;

            this.TopMost = true;

            ilist1.Add(Properties.Resources.KeybaordButton_Wide);
            ilist1.Add(Properties.Resources.KeybaordButton_Wide);
            ilist1.Add(Properties.Resources.KeybaordButton_Wide);
            ilist1.Add(Properties.Resources.KeybaordButton_Wide3);
            button_1.ChangeGraphics(ilist1);
            button_2.ChangeGraphics(ilist1);

            // set position
            this.Location = new Point((PreviousForm.Location.X + this.Offset.X), (PreviousForm.Location.Y + this.Offset.Y));
        }

        private void GetSpacings()
        {
            LeftMessageMargin = txtMessage.Bounds.X - this.Bounds.X;
            LeftMargin = this.pictureBox1.Bounds.X - this.Bounds.X;
            RightMargin = this.Bounds.X + this.Width - (txtMessage.Bounds.X + txtMessage.Width);
            DeltaYBetweenControls = button_1.Bounds.Y - (txtMessage.Bounds.Y + txtMessage.Height);
            DeltaYBetweenButtonAndForm = this.Bounds.Y + this.Height - (button_1.Bounds.Y + button_1.Height);

            DeltaXBetweenButtons = button_1.Bounds.X - (button_2.Bounds.X + button_2.Bounds.Width);
            DeltaXBetweenButtonAndForm = this.Bounds.X + this.Bounds.Width - (button_1.Bounds.X + button_1.Bounds.Width);
        }

        protected Size determineSize(string text)
        {
            int minHeight = 75;
            int maxHeight = 450;
            int width = txtMessage.Bounds.Width;   // 350
            if (msgIcon == MessageIcon.MBICON_NOTAPPLICABLE)
            {
                width += (txtMessage.Bounds.X - pictureBox1.Bounds.X);
            }

            // Start from text box
            int Y = txtMessage.Bounds.Y;
            int maxTextBoxHeight = maxHeight - DeltaYBetweenButtonAndForm - Y;
            if (button1Visible || button2Visible)
            {
                maxTextBoxHeight -= Math.Max(button_1.Bounds.Height, button_2.Bounds.Height);
                maxTextBoxHeight -= DeltaYBetweenControls;
            }
            if (bShowProgressBar)
            {
                maxTextBoxHeight -= (MSGprogress.Bounds.Height + DeltaYBetweenControls);
            }

            Size txtSize = new Size(width, int.MaxValue);
            TextFormatFlags flags = TextFormatFlags.WordBreak;

            txtSize = TextRenderer.MeasureText(text, txtMessage.Font, txtSize, flags);

            // check if text height is larger than max height allowed
            if (txtSize.Height > maxTextBoxHeight)
            {
                txtSize = new Size(width, maxTextBoxHeight);
                txtMessage.AutoEllipsis = true;
            }

            // maybe allow for bigger.. but thats getting to be a large string
            txtMessage.Size = txtSize;

            int height = txtSize.Height;
            if (height < minHeight)
            {
                height = minHeight;
            }

            // text box height
            txtMessage.Size = new Size(width, height);
            Y += height;
            Y += DeltaYBetweenControls;
            if (bShowProgressBar)
            {
                Y += MSGprogress.Bounds.Height;
                Y += DeltaYBetweenControls;
            }

            int X = this.Bounds.Width;
            int maxButtonHeight = 0, XDelta = 0;

            //button1 
            if (button1Visible == true)
            {
                X -= (DeltaXBetweenButtonAndForm + button_1.Bounds.Width);
                button_1.SetBounds(X, Y, button_1.Bounds.Width, button_1.Bounds.Height);
                maxButtonHeight = button_1.Bounds.Height;
            }

            //button2 
            if (button2Visible == true)
            {
                XDelta = button1Visible ? DeltaXBetweenButtons : DeltaXBetweenButtonAndForm;
                X -= (XDelta + button_2.Bounds.Width);
                button_2.SetBounds(X, Y, button_2.Bounds.Width, button_2.Bounds.Height);
                maxButtonHeight = Math.Max(maxButtonHeight, button_2.Bounds.Height);
            }

            Y += maxButtonHeight;
            Y += DeltaYBetweenButtonAndForm;

            if (Y < minHeight)
                Y = minHeight;

            if (Y > maxHeight)
                Y = maxHeight;

            int nDialogWidth = msgIcon == MessageIcon.MBICON_NOTAPPLICABLE ? width + LeftMargin + RightMargin : width + LeftMessageMargin + RightMargin;
            return new Size(nDialogWidth, Y);
        }

        private void RoboMessagePanel_RegionChanged(object sender, EventArgs e)
        {
            // change control label to name of Form
            label_WindowTitle.Text = this.Text;
        }

        public void setProgress(int PercentProgress)
        {
            int nPercent = PercentProgress;

            if (nPercent < MSGprogress.Minimum)  
                nPercent = MSGprogress.Minimum;
            if (MSGprogress.Maximum < nPercent)
                nPercent = MSGprogress.Maximum;

            MSGprogress.Value = nPercent;
           // MSGprogress.Refresh();
        }

        private void RoboMessagePanel_SizeChanged(object sender, EventArgs e)
        {
            int RECwidth, RECheight;

            // Resize rectanglular box
            RECwidth = this.Size.Width - 1;
            RECheight = this.Size.Height - 1;
            rectangleShape1.Size = new Size(RECwidth, RECheight);
            this.shapeContainer1.Size = new Size(RECwidth + 2, RECheight + 3);

            panel1.Size = new Size(this.Size.Width - 3, 37);

            if (msgIcon == MessageIcon.MBICON_NOTAPPLICABLE)
            {
                txtMessage.SetBounds(LeftMargin, txtMessage.Bounds.Y, txtMessage.Bounds.Width, txtMessage.Bounds.Height);
            }
        }

        protected void ResizeButtons()
        {
             SuspendLayout();

            string b1Text = button_1.Text;
            string b2Text = button_2.Text;

            Rectangle rcButton1 = button_1.Bounds;
            Rectangle rcButton2 = button_2.Bounds;

            int nRightMargin = this.Bounds.X + this.Bounds.Width - (rcButton1.X + rcButton1.Width);

            int nWidthButton1 = -1;
            int nWidthButton2 = -1;
            if (button_1.Visible == true && !string.IsNullOrEmpty(b1Text))
            {
                nWidthButton1 = button_1.determineWidth(b1Text);
            }

            if (button_2.Visible == true && !string.IsNullOrEmpty(b2Text))
            {
                nWidthButton2 = button_2.determineWidth(b2Text);
            }

            if (button_1.Visible && button_2.Visible)
            {
                int nButtonSpacing = rcButton1.X - (rcButton2.X + rcButton2.Width);
                int nMaxButtonwidth = Math.Max(nWidthButton1, nWidthButton2);
                int nX1 = this.Bounds.X + this.Bounds.Width - nMaxButtonwidth - nRightMargin;
                int nX2 = nX1 - nButtonSpacing - nMaxButtonwidth;
                button_1.AutoSize = false;
                button_2.AutoSize = false;
                button_1.SetBounds(nX1, rcButton1.Y, nMaxButtonwidth, rcButton1.Height);
                button_2.SetBounds(nX2, rcButton2.Y, nMaxButtonwidth, rcButton2.Height);
                button_1.SetImageWidth(nMaxButtonwidth);
                button_2.SetImageWidth(nMaxButtonwidth);
            }
            else if (button_1.Visible && !button_2.Visible)
            {
                if (0 < nWidthButton1)
                {
                    button_1.AutoSize = false;
                    int nX1 = this.Bounds.X + this.Bounds.Width - nWidthButton1 - nRightMargin;
                    button_1.SetBounds(nX1, rcButton1.Y, nWidthButton1, rcButton1.Height);
                    button_1.SetImageWidth(nWidthButton1);
                }
            }
            else if (!button_1.Visible && button_2.Visible)
            {
                if (0 < nWidthButton2)
                {
                    button_2.AutoSize = false;
                    int nX2 = this.Bounds.X + this.Bounds.Width - nWidthButton2 - nRightMargin;
                    button_2.SetBounds(nX2, rcButton2.Y, nWidthButton2, rcButton2.Height);
                    button_2.SetImageWidth(nWidthButton2);
                }
            }
            else
            {
                // do nothing
            }

            ResumeLayout();
        }

        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            button_1.BackgroundImage = Properties.Resources.BUTTON_PROMPTBUTTON3;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button_1.BackgroundImage = Properties.Resources.BUTTON_PROMPTBUTTON0;
        }

        private void button2_MouseDown(object sender, MouseEventArgs e)
        {
            button_2.BackgroundImage = Properties.Resources.BUTTON_PROMPTBUTTON3;
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            button_2.BackgroundImage = Properties.Resources.BUTTON_PROMPTBUTTON0;
        }

        private void button_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RoboMessagePanel_Deactivate(object sender, EventArgs e)
        {
            this.BringToFront();
            this.Activate();
        }

        private void txt_MessageBox_Enter(object sender, EventArgs e)
        {
            this.ActiveControl = label_WindowTitle;
        }

        private void txt_MessageBox_Click(object sender, EventArgs e)
        {
            this.ActiveControl = label_WindowTitle;
        }

        private void txtMessage_Enter(object sender, EventArgs e)
        {
            ActiveControl = panel1;
        }

        private void RoboMessagePanel_Deactivate_1(object sender, EventArgs e)
        {
            this.Activate();
            this.BringToFront();
        }

        private void RoboMessagePanel_Leave(object sender, EventArgs e)
        {
            this.Activate();
            this.BringToFront();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        private void RoboMessagePanel_Load(object sender, EventArgs e)
        {
            SuspendLayout();

            ilist2.Add(Properties.Resources.btnsmall_STD);
            ilist2.Add(Properties.Resources.btnsmall_STD);
            ilist2.Add(Properties.Resources.btnsmall_STD);
            ilist2.Add(Properties.Resources.btnsmall_CLICK);
            this.button_1.ChangeGraphics(ilist2);
            this.button_2.ChangeGraphics(ilist2);
            if (msgIcon == MessageIcon.MBICON_NOTAPPLICABLE)
            {
                pictureBox1.Visible = false;
            }
            else
            {
                pictureBox1.BackColor = Color.Transparent;
                switch (msgIcon)
                {
                    case MessageIcon.MBICON_ERROR:
                        pictureBox1.Image = Properties.Resources.Message_Error;
                        break;

                    case MessageIcon.MBICON_INFORMATION:
                        pictureBox1.Image = Properties.Resources.Message_Information;
                        break;

                    case MessageIcon.MBICON_QUESTION:
                        pictureBox1.Image = Properties.Resources.Message_Question;
                        break;

                    case MessageIcon.MBICON_WARNING:
                        pictureBox1.Image = Properties.Resources.Message_Warning;
                        break;

                    case MessageIcon.MBICON_WAIT:
                        pictureBox1.Image = Properties.Resources.WaitAnimation;
                        break;

                }
                pictureBox1.SetBounds(pictureBox1.Bounds.X, txtMessage.Bounds.Y, pictureBox1.Bounds.Width, pictureBox1.Bounds.Height);
            }

            ResizeButtons();

            this.PerformLayout();

            this.button_1.Refresh();
            this.button_2.Refresh();
            this.BringToFront();
        }
    }
}