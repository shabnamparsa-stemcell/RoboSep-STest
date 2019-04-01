using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GUI_Console
{
    public enum enumButtonClick
    {
        eButton_Undefined,
        eButton1,
        eButton2,
        eButton3
    }

    public partial class RoboMessagePanel6 : Form
    {
        protected Form PreviousForm;
        public Point Offset;
        private const int GRAPHICSIZE = 19;
        private const int TOPGRAPHIC_HEIGHT = 35;
        private const int BORDERWIDTH = 5;
        protected string FormTitle = "RoboSep Message";
        protected string message = "";

        private int LeftMessageMargin = 0;
        private int LeftMargin = 0;
        private int RightMargin = 0;
        private int DeltaYBetweenMsgBoxAndButton = 0;
        private int DeltaYBetweenButtonAndForm = 0;
        private int DeltaXBetweenButtons = 0;
        private int DeltaXBetweenButtonAndForm = 0;
        private enumButtonClick eClick = enumButtonClick.eButton1;
        private GUI_Controls.MessageIcon msgIcon = GUI_Controls.MessageIcon.MBICON_INFORMATION;

        bool button1Visible = false;
        bool button2Visible = false; 
        bool button3Visible = false;

        private List<Image> ilistButton1 = null;
        private List<Image> ilistButton2 = null;
        private List<Image> ilistButton3 = null;


        public RoboMessagePanel6(Form PrevForm, GUI_Controls.MessageIcon msgIcon, string Title, string Message, Image[] aImageButton1, Image[] aImageButton2, Image[] aImageButton3)
        {
            // message box option where there are no buttons, and response will come from system.
            // just displays a message until receives propt to continue
            InitializeComponent();

            GetSpacings();

            // set previous form
            PreviousForm = PrevForm;
            this.msgIcon = msgIcon;

            SuspendLayout();

            this.DoubleBuffered = true;
            this.UpdateStyles();

            // button 1
            if (aImageButton1 != null)
            {
                ilistButton1 = new List<Image>();
                ilistButton1.AddRange(aImageButton1);
                button_1.ChangeGraphics(ilistButton1);
                button1Visible = true;
                button_1.Click += new EventHandler(button_1_Click);
            }
            // button 2
            if (aImageButton2 != null)
            {
                ilistButton2 = new List<Image>();
                ilistButton2.AddRange(aImageButton2);
                button_2.ChangeGraphics(ilistButton2);
                button2Visible = true;
                button_2.Click += new EventHandler(button_2_Click);
            }
            // button 3
            if (aImageButton3 != null)
            {
                ilistButton3 = new List<Image>();
                ilistButton3.AddRange(aImageButton3);
                button_3.ChangeGraphics(ilistButton3);
                button3Visible = true;
                button_3.Click += new EventHandler(button_3_Click);
            }

            // set size of form based on amount of text
            this.Size = determineSize(Message);
            // create region for form based on size
            drawRegion();
            // set message to given message
            message = Message;

            if (!string.IsNullOrEmpty(Title))
                FormTitle = Title;

            // set offset
            Offset = new Point((PreviousForm.Size.Width - this.Width) / 2, (PreviousForm.Size.Height - this.Size.Height) / 2 + 25);

            SetDialogProperties();
            ResumeLayout();
        }

        public void SetMessage(string sMsg)
        {
            if (string.IsNullOrEmpty(sMsg))
                return;

            txtMessage.Text = sMsg;
            this.Refresh();
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

        private void GetSpacings()
        {

            LeftMessageMargin = txtMessage.Bounds.X - this.Bounds.X;
            LeftMargin = this.pictureBox1.Bounds.X - this.Bounds.X;
            RightMargin = this.Bounds.X + this.Width - (txtMessage.Bounds.X + txtMessage.Width);
            DeltaYBetweenMsgBoxAndButton = button_1.Bounds.Y - (txtMessage.Bounds.Y + txtMessage.Height);
            DeltaYBetweenButtonAndForm = this.Bounds.Y + this.Height - (button_1.Bounds.Y + button_1.Height);

            DeltaXBetweenButtons = button_1.Bounds.X - (button_2.Bounds.X + button_2.Bounds.Width);
            DeltaXBetweenButtonAndForm = this.Bounds.X + this.Bounds.Width - (button_1.Bounds.X + button_1.Bounds.Width);
        }
        
        protected Size determineSize(string text)
        {
            int minHeight = 100;
            int hpadding = 50;
            int maxHeight = 450 - hpadding;
            int width = txtMessage.Bounds.Width;   // 350
            if (msgIcon == GUI_Controls.MessageIcon.MBICON_NOTAPPLICABLE)
            {
                width += (txtMessage.Bounds.X - pictureBox1.Bounds.X);
            }

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

            // text box height
            txtMessage.Size = new Size(width, height);
            Y += height;
            Y += DeltaYBetweenMsgBoxAndButton;

            int X = this.Bounds.X + this.Bounds.Width;
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

            //button3 
            if (button3Visible == true)
            {
                XDelta = button2Visible ? DeltaXBetweenButtons : (button1Visible ? DeltaXBetweenButtons : DeltaXBetweenButtonAndForm);
                X -= (XDelta + button_3.Bounds.Width);
                button_3.SetBounds(X, Y, button_3.Bounds.Width, button_3.Bounds.Height);
                maxButtonHeight = Math.Max(maxButtonHeight, button_3.Bounds.Height);
            }

            Y += maxButtonHeight;
            Y += DeltaYBetweenButtonAndForm;

            if (Y < minHeight)
                Y = minHeight;

            if (Y > maxHeight)
                Y = maxHeight;

            int nDialogWidth = msgIcon == GUI_Controls.MessageIcon.MBICON_NOTAPPLICABLE ? width + LeftMargin + RightMargin : width + LeftMessageMargin + RightMargin;
            return new Size(nDialogWidth, Y);
        }

        public enumButtonClick ButtonClick
        {
            get { return eClick; }
        }

        private void RoboMessagePanel6_RegionChanged(object sender, EventArgs e)
        {
            // change control label to name of Form
            label_WindowTitle.Text = this.Text;
        }

        private void drawRegion()
        {
        }

        private void RoboMessagePanel6_SizeChanged(object sender, EventArgs e)
        {
            SuspendLayout();

            // txt_MessageBox re- size
            int RECwidth, RECheight;
            RECwidth = this.Size.Width - 1;
            RECheight = this.Size.Height - 1;

            rectangleShape1.Size = new Size(RECwidth, RECheight);
            panel1.Size = new Size(this.Size.Width - 3, 37);

            if (msgIcon == GUI_Controls.MessageIcon.MBICON_NOTAPPLICABLE)
            {
                txtMessage.SetBounds(LeftMargin, txtMessage.Bounds.Y, txtMessage.Bounds.Width, txtMessage.Bounds.Height);
            }
            ResumeLayout();
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

        private void RoboMessagePanel6_Deactivate(object sender, EventArgs e)
        {
            this.Activate();
            this.BringToFront();
        }

        private void RoboMessagePanel6_Leave(object sender, EventArgs e)
        {
            this.Activate();
            this.BringToFront();
        }

        private void RoboMessagePanel6_Load(object sender, EventArgs e)
        {
            int nWidth = this.Bounds.Width;
            nWidth -= 2 * LeftMargin;

            button_3.Visible = button1Visible? true : false;
            button_2.Visible = button2Visible? true : false;
            button_1.Visible = button3Visible? true : false;

            if (msgIcon == GUI_Controls.MessageIcon.MBICON_NOTAPPLICABLE)
            {
                pictureBox1.Visible = false;
            }
            else
            {
                pictureBox1.BackColor = Color.Transparent;
                switch (msgIcon)
                {
                    case GUI_Controls.MessageIcon.MBICON_WARNING:
                        pictureBox1.Image = GUI_Controls.Properties.Resources.Message_Warning;
                        break;

                    case GUI_Controls.MessageIcon.MBICON_INFORMATION:
                        pictureBox1.Image = GUI_Controls.Properties.Resources.Message_Information;
                        break;

                    case GUI_Controls.MessageIcon.MBICON_QUESTION:
                        pictureBox1.Image = GUI_Controls.Properties.Resources.Message_Question;
                        break;

                    case GUI_Controls.MessageIcon.MBICON_ERROR:
                        pictureBox1.Image = GUI_Controls.Properties.Resources.Message_Error;
                        break;
                }
                pictureBox1.SetBounds(pictureBox1.Bounds.X, txtMessage.Bounds.Y, pictureBox1.Bounds.Width, pictureBox1.Bounds.Height);
            }
        }

        private void button_1_Click(object sender, EventArgs e)
        {
            eClick = enumButtonClick.eButton1;
            this.Hide();
        }
        private void button_2_Click(object sender, EventArgs e)
        {
            eClick = enumButtonClick.eButton2;
            this.Hide();
        }
        private void button_3_Click(object sender, EventArgs e)
        {
            eClick = enumButtonClick.eButton3;
            this.Hide();
        }
    }
}
