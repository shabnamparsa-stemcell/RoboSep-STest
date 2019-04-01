using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Tesla.Common;


namespace GUI_Controls
{
    public partial class RoboMessagePanel2 : Form
    {
        protected Form PreviousForm;
        public Point Offset;
        private const int GRAPHICSIZE = 19;
        private const int TOPGRAPHIC_HEIGHT = 35;
        private const int BORDERWIDTH = 5;
        protected string FormTitle = "RoboSep Message";
        protected string message = "";


        private int XSpacingsBetweenButtons = 0;
        private int RightMargin = 0;
        private int LeftMargin = 0;
        private int DeltaYBetweenControls = 0;
        private int DeltaYBetweenControlAndButton = 0;
        private int DeltaYBetweenButtonAndForm = 0;
        private int secs;
        private int mins;
        private int hrs;
        private string elapsedTime = string.Empty;
        private string percentCompleted = string.Empty;
        private bool ShowElapseTime = false;
        private bool ShowCheckBox = false;


        public RoboMessagePanel2(Form PrevForm, string Message)
        {
            InitializeComponent();

            SuspendLayout();

            // set previous form
            PreviousForm = PrevForm;

            GetSpacings();

            // set size of form based on amount of text
            this.Size = determineSize(Message);

            // set message to given message
            message = Message;

            // set up buttons
            button2.Visible = false;
            button1.Text = "OK"; ;

            // set offset
            Offset = new Point((PreviousForm.Size.Width - this.Width) / 2, (PreviousForm.Size.Height - this.Size.Height) / 2 + 25);

            SetDialogProperties();
            ResumeLayout();
       }

        public RoboMessagePanel2(Form PrevForm, string Message, string panelName)
        {
            InitializeComponent();
            SuspendLayout();

            // set previous form
            PreviousForm = PrevForm;

            GetSpacings();

            // set size of form based on amount of text
            this.Size = determineSize(Message);

            // set message to given message
            message = Message;

            // Set Form title
            FormTitle = panelName;

            // set up buttons
            button1.Visible = false;

            // set offset
            Offset = new Point((PreviousForm.Size.Width - this.Width) / 2, (PreviousForm.Size.Height - this.Size.Height) / 2 + 25);
            
            SetDialogProperties();
            ResumeLayout();
        }

        public RoboMessagePanel2(Form PrevForm, string Message, string Title, string b1Text, string b2Text, bool bShowCheckBox, string ckBoxText)
        {
            InitializeComponent();
            SuspendLayout();

            // set previous form
            PreviousForm = PrevForm;

            GetSpacings();

            ShowCheckBox = bShowCheckBox;

            if (ShowCheckBox)
            {
                checkBox1.Visible = true;
                label_CheckBox.Text = Utilities.TruncatedString(ckBoxText, label_CheckBox.ClientRectangle, label_CheckBox.Font, TextFormatFlags.SingleLine);
                label_CheckBox.Visible = true;
            }

            // set size of form based on amount of text
            this.Size = determineSize(Message);

            // set message
            message = Message;
    
            // set window title
            FormTitle = Title;

            // set main button text
            button1.Text = b1Text;

            // turn 2nd button on?
            if (!string.IsNullOrEmpty(b2Text))
            {
                button2.Enabled = true;
                button2.Visible = true;
            }

            // set 2nd button text
            button2.Text = b2Text;

            // set offset
            Offset = new Point((PreviousForm.Size.Width - this.Width) / 2, (PreviousForm.Size.Height - this.Size.Height) / 2 + 25);
                        
            SetDialogProperties();
            ResumeLayout();
        }

        public bool CheckBoxChecked
        {
            get
            {
                return checkBox1.Checked;
            }
        }

        public void closeDialogue(DialogResult givenResult)
        {
            this.DialogResult = givenResult;
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
            if (ShowCheckBox)
            {
                Y += DeltaYBetweenControls;
                Y += label_CheckBox.Bounds.Height;
                Y += DeltaYBetweenControlAndButton;
            }

            // button height
            Y += button1.Bounds.Height;
            Y += DeltaYBetweenButtonAndForm;

            return new Size(width + hpadding, Y + vpadding);
        }


        private void RoboMessagePanel2_RegionChanged(object sender, EventArgs e)
        {
            // change control label to name of Form
            label_WindowTitle.Text = this.Text;
        }


        private void RoboMessagePanel2_SizeChanged(object sender, EventArgs e)
        {
            SuspendLayout();

            // txt_MessageBox re- size
            int RECwidth, RECheight;
            RECwidth = this.Size.Width - 1;
            RECheight = this.Size.Height - 1;
            rectangleShape1.Size = new Size(RECwidth, RECheight);
            panel1.Size = new Size(this.Size.Width - 3, 37);

            int Y = txtMessage.Bounds.Y;

            if (ShowCheckBox)
            {
                Y = Y + txtMessage.Height + DeltaYBetweenControls;
                checkBox1.Location = new Point(checkBox1.Bounds.X, Y);
                label_CheckBox.Location = new Point(label_CheckBox.Bounds.X, Y);

                Y += label_CheckBox.Height;
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
            DeltaYBetweenControls = label_CheckBox.Bounds.Y - (txtMessage.Bounds.Y + txtMessage.Height);
            DeltaYBetweenControlAndButton = button1.Bounds.Y - (label_CheckBox.Bounds.Y + label_CheckBox.Height);
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

        private void RoboMessagePanel2_Load(object sender, EventArgs e)
        {
            //change toggle button graphics
            List<Image> ilist = new List<Image>();
            ilist.Add(Properties.Resources.btnsmall_STD);
            ilist.Add(Properties.Resources.btnsmall_STD);
            ilist.Add(Properties.Resources.btnsmall_STD);
            ilist.Add(Properties.Resources.btnsmall_CLICK);
            this.button1.ChangeGraphics(ilist);
            this.button2.ChangeGraphics(ilist);

            ResizeButtons();
            this.button1.Refresh();
            this.button2.Refresh();
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
    }



    

}
