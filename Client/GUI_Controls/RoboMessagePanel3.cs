using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Invetech.ApplicationLog;
namespace GUI_Controls
{
    public partial class RoboMessagePanel3 : Form
    {
        protected Form PreviousForm;
        public Point Offset;
        private const int GRAPHICSIZE = 19;
        private const int TOPGRAPHIC_HEIGHT = 35;
        private const int BORDERWIDTH = 5;

        protected string FormTitle = "RoboSep Message";
        protected string message = "";
        private string editBoxText = "";

        private int YOffset = 25;
        private int LeftMessageMargin = 0;
        private int LeftMargin = 0;
        private int RightMargin = 0;
        private int DeltaYBetweenControls = 0;
        private int DeltaYBetweenControlAndButton;
        private int DeltaYBetweenButtonAndForm = 0;
        private int DeltaXBetweenButtons = 0;
        private int DeltaXBetweenButtonAndForm = 0;

        private bool KeyBoardEnabled = true;
        private bool button1Visible = false;
        private bool button2Visible = false;
        private bool bShowEditBox = false;

        MessageIcon msgIcon = MessageIcon.MBICON_INFORMATION;

        public RoboMessagePanel3(Form PrevForm, MessageIcon msgIcon, string Message, string Title)
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
            FormTitle = Title;;

            // set size of form based on amount of text
            this.Size = determineSize(Message);

            // set message to given message
            message = Message;

            // set up buttons
            button2.Visible = false;
            button1.Text = "OK";

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

        public RoboMessagePanel3(Form PrevForm, MessageIcon msgIcon, string Message, string Title, string OkButton)
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

            // set up button text
            button1.Text = OkButton;

            // set offset
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

        public RoboMessagePanel3(Form PrevForm, MessageIcon msgIcon, string Message, string Title, string b1, string b2)
        {
            InitializeComponent();
            SuspendLayout();

            this.DoubleBuffered = true;
            this.UpdateStyles();

            GetSpacings();

            SetButton1Visible(true);

            // set main button text
            button1.Text = b1;

            // turn 2nd button on?
            if (b2 != string.Empty)
            {
                button2.Enabled = true;
                SetButton2Visible(true);
            }
            // set 2nd button text
            button2.Text = b2;

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

        public RoboMessagePanel3(Form PrevForm, MessageIcon msgIcon, string Message, string Title, string b1, string b2, bool bShowEditBox, bool bKeyBoardEnabled, bool bHideUserEntry)
        {
            // message box option where there are no buttons, and response will come from system.
            // just displays a message until receives propt to continue
            InitializeComponent();

            this.DoubleBuffered = true;
            this.UpdateStyles();

            this.msgIcon = msgIcon;
            this.bShowEditBox = bShowEditBox;
            SuspendLayout();

            GetSpacings();

            KeyBoardEnabled = bKeyBoardEnabled;
            this.bShowEditBox = bShowEditBox;

            SuspendLayout();

            // set previous form
            PreviousForm = PrevForm;

            // show edit box
            this.EditBox.Visible = bShowEditBox ? true : false;

            if (bHideUserEntry)
            {
                this.EditBox.PasswordChar = '*';
            }

            // set message
            message = Message;
            // set window title
            FormTitle = Title;

            // set main button text
            button1.Text = b1;
            SetButton1Visible(true);

            // turn 2nd button on?
            if (b2 != string.Empty)
            {
                SetButton2Visible(true);
                button2.Enabled = true;
                button2.Visible = true;
            }
            // set 2nd button text
            button2.Text = b2;

            // set size of form based on amount of text
            this.Size = determineSize(Message);

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

        public void closeDialogue(DialogResult givenResult)
        {
            this.DialogResult = givenResult;
        }

        public string EditBoxText
        {
            get
            {
                return editBoxText;
            }
        }

        private void SetDialogProperties()
        {
            // set text components
            label_WindowTitle.Text = FormTitle;
            txtMessage.Text = message;

            this.TopMost = true;

            // set position
            this.Location = new Point((PreviousForm.Location.X + this.Offset.X), (PreviousForm.Location.Y + this.Offset.Y));
        }

        protected Size determineSize(string text)
        {
            int minHeight = 50;
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
                maxTextBoxHeight -= Math.Max(button1.Bounds.Height, button2.Bounds.Height);
                maxTextBoxHeight -= DeltaYBetweenControls;
            }
            if (bShowEditBox)
            {
                maxTextBoxHeight -= (EditBox.Bounds.Height + DeltaYBetweenControls);
            }
 
            Size txtSize = new Size(width, int.MaxValue);
            TextFormatFlags flags = TextFormatFlags.WordBreak;

            txtSize = TextRenderer.MeasureText(text, txtMessage.Font, txtSize, flags);

            // check if text height is larger than max height
            if (txtSize.Height > maxTextBoxHeight)
            {
                txtSize = new Size(width, maxTextBoxHeight);
           //     txtMessage.AutoEllipsis = true;
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
            if (bShowEditBox)
            {
                EditBox.SetBounds(EditBox.Bounds.X, Y, EditBox.Bounds.Width, EditBox.Bounds.Height);
                Y += EditBox.Bounds.Height;
                Y += DeltaYBetweenControls;
            }

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

            int nDialogWidth = msgIcon == MessageIcon.MBICON_NOTAPPLICABLE ? width + LeftMargin + RightMargin : width + LeftMessageMargin + RightMargin;
            return new Size(nDialogWidth, Y);
        }

        private void RoboMessagePane3_SizeChanged(object sender, EventArgs e)
        {
            SuspendLayout();

            // txt_MessageBox re- size
            int RECwidth, RECheight;
            RECwidth = this.Size.Width - 1;
            RECheight = this.Size.Height - 1;

            rectangleShape1.Size = new Size(RECwidth, RECheight);
            this.shapeContainer1.Size = new Size(RECwidth + 2, RECheight + 3);

            panel1.Size = new Size(this.Size.Width - 3, 37);

            if (msgIcon == MessageIcon.MBICON_NOTAPPLICABLE)
            {
                txtMessage.SetBounds(LeftMargin, txtMessage.Bounds.Y, txtMessage.Bounds.Width, txtMessage.Bounds.Height);
            }
            ResumeLayout();
        }

        private void GetSpacings()
        {
            LeftMessageMargin = txtMessage.Bounds.X - this.Bounds.X;
            LeftMargin = this.pictureBox1.Bounds.X - this.Bounds.X;
            RightMargin = this.Bounds.X + this.Width - (txtMessage.Bounds.X + txtMessage.Width);
            DeltaYBetweenControls = EditBox.Bounds.Y - (txtMessage.Bounds.Y + txtMessage.Height);
            DeltaYBetweenControlAndButton = button1.Bounds.Y - (EditBox.Bounds.Y + EditBox.Height);
            DeltaYBetweenButtonAndForm = this.Bounds.Y + this.Height - (button1.Bounds.Y + button1.Height);

            DeltaXBetweenButtons = button1.Bounds.X - (button2.Bounds.X + button2.Bounds.Width);
            DeltaXBetweenButtonAndForm = this.Bounds.X + this.Bounds.Width - (button1.Bounds.X + button1.Bounds.Width);
        }


        public void SetButton1Visible(bool bVisible)
        {
            button1Visible = bVisible;
            button1.Visible = button1Visible;
        }

        public void SetButton2Visible(bool bVisible)
        {
            button2Visible = bVisible;
            button2.Visible = button2Visible;
        }


        private void ResizeButtons()
        {
            SuspendLayout();

            string b1Text = button1.Text;
            string b2Text = button2.Text;

            Rectangle rcButton1 = button1.Bounds;
            Rectangle rcButton2 = button2.Bounds;

            int nRightMargin = this.Bounds.X + this.Bounds.Width - (rcButton1.X + rcButton1.Width);

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
                int nButtonSpacing = rcButton1.X - (rcButton2.X + rcButton2.Width);
                int nMaxButtonWidth = Math.Max(nWidthButton1, nWidthButton2);
                int nX1 = this.Bounds.X + this.Bounds.Width - nMaxButtonWidth - nRightMargin;
                int nX2 = nX1 - nButtonSpacing - nMaxButtonWidth;
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
                    int nX1 = this.Size.Width - nWidthButton1 - nRightMargin;
                    button1.SetBounds(nX1, rcButton1.Y, nWidthButton1, rcButton1.Height);
                    button1.SetImageWidth(nWidthButton1);
                }
            }
            else if (!button1.Visible && button2.Visible)
            {
                if (0 < nWidthButton2)
                {
                    button2.AutoSize = false;
                    int nX2 = this.Bounds.X + this.Bounds.Width - nWidthButton2 - nRightMargin;
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

        private void RoboMessagePanel3_RegionChanged(object sender, EventArgs e)
        {
            // change control label to name of Form
            label_WindowTitle.Text = this.Text;
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

        private void txtMessage_Enter(object sender, EventArgs e)
        {
            ActiveControl = panel1;
        }

        private void RoboMessagePanel_Leave(object sender, EventArgs e)
        {
            this.Activate();
            this.BringToFront();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // OK button
            if (string.IsNullOrEmpty(EditBox.Text.Trim()))
            {
                MessageBox.Show(this, "The password cannot be blank",
                    "Invalid Password",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            editBoxText = EditBox.Text;
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Cancel button
            editBoxText = String.Empty;
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        private void EditBox_Click(object sender, EventArgs e)
        {
            if (KeyBoardEnabled)
                createKeybaord();
        }

        private void createKeybaord()
        {
            // Create keybaord control
            GUI_Controls.Keyboard newKeyboard =  GUI_Controls.Keyboard.getInstance(this, EditBox, null, null, true);

            this.SendToBack();

            newKeyboard.ShowDialog();

            this.BringToFront();

            System.Diagnostics.Debug.WriteLine(String.Format("createKeybaord return "));

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "Generating touch keybaord");             
            newKeyboard.Dispose();
        }

        private void RoboMessagePanel3_Load(object sender, EventArgs e)
        {
            SuspendLayout();

            //change toggle button graphics
            List<Image> ilist = new List<Image>();
            ilist.Add(Properties.Resources.btnsmall_STD);
            ilist.Add(Properties.Resources.btnsmall_STD);
            ilist.Add(Properties.Resources.btnsmall_STD);
            ilist.Add(Properties.Resources.btnsmall_CLICK);
            button1.ChangeGraphics(ilist);
            button2.ChangeGraphics(ilist);
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
            if (bShowEditBox)
                EditBox.Focus();
            this.BringToFront();
        }


        private void RoboMessagePane3_VisibleChanged(object sender, EventArgs e)
        {
            this.EditBox.Focus();
        }
    }



























}
