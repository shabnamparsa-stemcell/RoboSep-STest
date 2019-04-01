using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GUI_Controls
{
    public partial class RoboMessagePanel3 : Form
    {
        protected Form PreviousForm;
        public Point Offset;

        protected string FormTitle = "RoboSep Message";
        protected string message = "";
        private string editBoxText = "";

        private bool KeyBoardEnabled = true;
        private bool ShowEditBox = false;
        private int XSpacingsBetweenButtons = 0;
        private int RightMargin= 0;
        private int LeftMargin = 0;
        private int DeltaYBetweenControls = 0;
        private int DeltaYBetweenControlAndButton = 0;
        private int DeltaYBetweenButtonAndForm = 0;

        public RoboMessagePanel3(Form PrevForm, string Message)
        {
            InitializeComponent();

            SuspendLayout();

            // set previous form
            PreviousForm = PrevForm;

            // set size of form based on amount of text
            this.Size = determineSize(Message);

            // set message to given message
            message = Message;

            // set up buttons
            button2.Visible = false;

            // set offset
            Offset = new Point((PreviousForm.Size.Width - this.Width) / 2, (PreviousForm.Size.Height - this.Size.Height) / 2 + 25);

            SetDialogProperties();
            ResumeLayout();
       }

        public RoboMessagePanel3(Form PrevForm, string Message, string panelName)
        {
            InitializeComponent();
            SuspendLayout();

            // set previous form
            PreviousForm = PrevForm;

            // set size of form based on amount of text
            this.Size = determineSize(Message);

            // set message to given message
            message = Message;

            // Set Form title
            FormTitle = panelName;

            // set up buttons
            button2.Visible = false;

            // set offset
            Offset = new Point((PreviousForm.Size.Width - this.Width) / 2, (PreviousForm.Size.Height - this.Size.Height) / 2 + 25);
            
            SetDialogProperties();
            ResumeLayout();
        }

        public RoboMessagePanel3(Form PrevForm, string Message, string Title, bool b2)
        {
            InitializeComponent();
            SuspendLayout();
            // set previous form
            PreviousForm = PrevForm;
            // set size of form based on amount of text
            this.Size = determineSize(Message);
            // set message
            message = Message;
            // set window title
            FormTitle = Title;

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

        public RoboMessagePanel3(Form PrevForm, string Message, string Title, string b1, string b2, bool bShowEditBox, bool bKeyBoardEnabled, bool bHideUserEntry)
        {
            // message box option where there are no buttons, and response will come from system.
            // just displays a message until receives propt to continue
            InitializeComponent();

            GetSpacings();

            KeyBoardEnabled = bKeyBoardEnabled;
            ShowEditBox = bShowEditBox;

            SuspendLayout();

            // set previous form
            PreviousForm = PrevForm;
  
            // set size of form based on amount of text
            this.Size = determineSize(Message);

            // show edit box
            this.EditBox.Visible = ShowEditBox ? true : false;

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
            // turn 2nd button on?
            if (b2 != string.Empty)
            {
                button2.Enabled = true;
                button2.Visible = true;
            }
            // set 2nd button text
            button2.Text = b2;

            // set offset
            Offset = new Point((PreviousForm.Size.Width - this.Width) / 2, (PreviousForm.Size.Height - this.Size.Height) / 2 + 25);

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
            //return new Size(350, 300);
            int width = 350;
            int minHeight = 100;
            int vpadding = 20;
            int hpadding = 50;
            int maxHeight = 450 - hpadding;

            // Start from text box
            int Y = txtMessage.Bounds.Y - this.Bounds.Y;
 
            Size txtSize = new Size(width, int.MaxValue);
            TextFormatFlags flags = TextFormatFlags.WordBreak;

            txtSize = TextRenderer.MeasureText(text, txtMessage.Font, txtSize, flags);

            // check if text height is larger than max height
            if (txtSize.Height > maxHeight)
            {
                width = 450;
                txtSize = new Size(width, int.MaxValue);
                txtSize = TextRenderer.MeasureText(text, txtMessage.Font, txtSize, flags);
            }

            // maybe allow for bigger.. but thats getting to be a large string
            txtMessage.Size = txtSize.Height > maxHeight ? new Size(width, maxHeight) : txtSize;

            int height = txtSize.Height;

            if (txtSize.Height < minHeight)
                height = minHeight;

            if (txtSize.Height > maxHeight)
                height = maxHeight;

            // text box height
            txtMessage.Size = new Size(width, height);
            Y += height;

            //edit box height
            if (ShowEditBox)
            {
                Y += DeltaYBetweenControls;
                Y += EditBox.Bounds.Height;
                Y += DeltaYBetweenControlAndButton;
            }

            // button height
            Y += button1.Bounds.Height;
            Y += DeltaYBetweenButtonAndForm;

            return new Size(width + hpadding, Y + vpadding);
        }

        private void RoboMessagePane3_SizeChanged(object sender, EventArgs e)
        {
            SuspendLayout();

            // txt_MessageBox re- size
            int RECwidth, RECheight;
            RECwidth = this.Size.Width - 1;
            RECheight = this.Size.Height - 1;

            rectangleShape1.Size = new Size(RECwidth, RECheight);
            panel1.Size = new Size(this.Size.Width - 3, 37);

            int Y = 0;
            if (ShowEditBox)
            {
                EditBox.Size = new Size(this.Size.Width - LeftMargin - RightMargin, EditBox.Height);
                Y = txtMessage.Bounds.Y + txtMessage.Height + DeltaYBetweenControls;
                EditBox.Location = new Point(EditBox.Bounds.X, Y);
                Y += EditBox.Height;
            }
            
            Y += DeltaYBetweenControlAndButton;
   
            int X = 0;
            X = this.Size.Width - RightMargin - button1.Width;
            button1.Location = new Point(X, Y);
            X -= XSpacingsBetweenButtons;
            X -= button2.Width;
            button2.Location = new Point(X, Y);
            ResumeLayout();
        }

        private void GetSpacings()
        {
            XSpacingsBetweenButtons = button1.Bounds.X - (button2.Bounds.X + button2.Width);
            LeftMargin = txtMessage.Bounds.X - this.Bounds.X;
            RightMargin = this.Bounds.X + this.Width - (button1.Bounds.X + button1.Width);
            DeltaYBetweenControls = EditBox.Bounds.Y - (txtMessage.Bounds.Y + txtMessage.Height);
            DeltaYBetweenControlAndButton = button1.Bounds.Y - (EditBox.Bounds.Y + EditBox.Height);
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
            GUI_Controls.SP_Logger.log.Info("Generating touch keybaord");
        }

        private void RoboMessagePanel3_Load(object sender, EventArgs e)
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
            EditBox.Focus();
            this.BringToFront();
        }


        private void RoboMessagePane3_VisibleChanged(object sender, EventArgs e)
        {
            this.EditBox.Focus();
        }
    }



























}
