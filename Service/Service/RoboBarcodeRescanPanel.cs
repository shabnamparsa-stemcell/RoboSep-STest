using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Xml;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using GUI_Console;

namespace GUI_Controls
{
    public partial class RoboBarcodeRescanPanel : Form
    {
        private Form PreviousForm;
        public Point Offset;
        private const int GRAPHICSIZE = 19;
        private const int TOPGRAPHIC_HEIGHT = 35;
        private const int BORDERWIDTH = 5;
        private string FormTitle = "RoboSep Message";
        private string message = "";

        public RoboBarcodeRescanPanel(Form PrevForm, string Message)
        {
            InitializeComponent();
            SuspendLayout();
            // set previous form
            PreviousForm = PrevForm;
            // set size of form based on amount of text
            this.Size = determineSize(Message);
            // create region for form based on size
            drawRegion();
            // set message to given message
            message = Message;
            // set up buttons
            button2.Visible = false;
            button1.Text = "OK";
            // set offset
            Offset = new Point((PreviousForm.Size.Width - this.Width) / 2, (PreviousForm.Size.Height - this.Size.Height) / 2 + 25);
                        
            SetDialogProperties();
            ResumeLayout();
        }

        public RoboBarcodeRescanPanel(Form PrevForm, string Message, string panelName, string OkButton)
        {
            InitializeComponent();
            SuspendLayout();
            // set previous form
            PreviousForm = PrevForm;
            // set size of form based on amount of text
            this.Size = determineSize(Message);
            // create region for form based on size
            drawRegion();
            // set message to given message
            message = Message;
            // Set Form title
            FormTitle = panelName;
            // set up buttons
            button2.Visible = false;
            button1.Text = OkButton;
            // set offset
            Offset = new Point((PreviousForm.Size.Width - this.Width) / 2, (PreviousForm.Size.Height - this.Size.Height) / 2 + 25);
            
            SetDialogProperties();
            ResumeLayout();
        }

        public RoboBarcodeRescanPanel(Form PrevForm, string Message, string Title, string b1, string b2, string currentValue)
        {
            InitializeComponent();
            SuspendLayout();
            // set previous form
            PreviousForm = PrevForm;
            // set size of form based on amount of text
            this.Size = determineSize(Message);
            // create region for form based on size
            drawRegion();
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


            txtValue.Text = currentValue;
            txtValue.Visible = true;

            SetDialogProperties();
            ResumeLayout();
        }

        public RoboBarcodeRescanPanel(Form PrevForm, string Message, bool isWaitDialog, bool showProgress)
        {
            // message box option where there are no buttons, and response will come from system.
            // just displays a message until receives propt to continue
            InitializeComponent();
            // set previous form
            PreviousForm = PrevForm;
            SuspendLayout();
            // set up progress bar
            if (showProgress)
            {
                ProgressPanel.Visible = true;
                MSGprogress.setElements(new int[] { 10 }, new int[] { 5 });
                MSGprogress.Visible = true;
                MSGprogress.Start();
            }
            else
            {
                ProgressPanel.Visible = false;
                MSGprogress.Visible = false;
            }
            // set size of form based on amount of text
            this.Size = determineSize(Message);
            // create region for form based on size
            drawRegion();
            // set message to given message
            message = Message;
            // set up buttons
            button2.Visible = false;
            button1.Visible = false;
            button1.Text = "OK";
            // set offset
            Offset = new Point((PreviousForm.Size.Width - this.Width) / 2, (PreviousForm.Size.Height - this.Size.Height) / 2 + 25);

            SetDialogProperties();
            ResumeLayout();
        }

        // so class can be inherited to create
        // quadrant selection dialog
        public RoboBarcodeRescanPanel()
        {
            PreviousForm = null;
            Offset = new Point(0, 0);
            FormTitle = "Quadrant Reagent Sharing";
            drawRegion();
            button2.Visible = true;
            button2.Text = "Cancel";
            button1.Text = "Done";
        }

        public void closeDialogue(DialogResult givenResult)
        {
            this.DialogResult = givenResult;
        }

        private void SetDialogProperties()
        {
            SuspendLayout();
            // set dialogue results
            button1.DialogResult = DialogResult.OK;
            button2.DialogResult = DialogResult.Cancel;
            this.AcceptButton = button1;
            this.CancelButton = button2;

            // set button appearance

            List<Image> ilist = new List<Image>();
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_PROMPTBUTTON0);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_PROMPTBUTTON0);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_PROMPTBUTTON0);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_PROMPTBUTTON3);
            button1.ChangeGraphics(ilist);

            button2.ChangeGraphics(ilist);
            // set text components
            label_WindowTitle.Text = FormTitle;
            txtMessage.Text = message;

            // set position
            this.Location = new Point((PreviousForm.Location.X + this.Offset.X), (PreviousForm.Location.Y + this.Offset.Y));
            ResumeLayout();
        }

        private void RoboMessagePanel_Load(object sender, EventArgs e)
        {
            
        }

        private Size determineSize(string text)
        {
            int width = 350;
            int minHeight = 100;
            int vpadding = 100;
            int hpadding = 50;
            int maxHeight = 420 - hpadding;

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
            return new Size(width + hpadding, height + vpadding);

        }

        private void RoboMessagePanel_RegionChanged(object sender, EventArgs e)
        {
            // change control label to name of Form
            //label_WindowTitle.Text = this.AccessibleName;

            /*
            // Set colour of border rectangles
            Rect1.BackColor = Color.FromArgb(108, 36, 119);
            Rect2.BackColor = Color.FromArgb(108, 36, 119);
            Rect3.BackColor = Color.FromArgb(108, 36, 119);
            Rect4.BackColor = Color.FromArgb(108, 36, 119);
             * */
        }

        public void setProgress(int PercentProgress)
        {
            MSGprogress.setProgress(PercentProgress);
        }

        private void drawRegion()
        {
            // create region
            // create pionts
            Point[] panelPoints = new Point[28];
            // top left
            panelPoints[0] = new Point(0, 0); // new Point(0, GRAPHICSIZE - 2);
            panelPoints[1] = new Point(0, 0); // new Point(1, GRAPHICSIZE - 6);
            panelPoints[2] = new Point(0, 0); // new Point(3, GRAPHICSIZE - 10);
            panelPoints[3] = new Point(0, 0); // new Point(5, GRAPHICSIZE - 13);
            panelPoints[4] = new Point(0, 0); // new Point(9, GRAPHICSIZE - 16);
            panelPoints[5] = new Point(0, 0); // new Point(14, GRAPHICSIZE - 18);
            panelPoints[6] = new Point(0, 0); // new Point(19, GRAPHICSIZE - 19);
            // Top Right
            panelPoints[7] = new Point(this.Size.Width, 0); // new Point(this.Size.Width - GRAPHICSIZE + 2, 0);
            panelPoints[8] = new Point(this.Size.Width, 0); // new Point(this.Size.Width - GRAPHICSIZE + 6, 1);
            panelPoints[9] = new Point(this.Size.Width, 0); // new Point(this.Size.Width - GRAPHICSIZE + 10, 3);
            panelPoints[10] = new Point(this.Size.Width, 0); // new Point(this.Size.Width - GRAPHICSIZE + 13, 5);
            panelPoints[11] = new Point(this.Size.Width, 0); // new Point(this.Size.Width - GRAPHICSIZE + 16, 9);
            panelPoints[12] = new Point(this.Size.Width, 0); // new Point(this.Size.Width - GRAPHICSIZE + 18, 14);
            panelPoints[13] = new Point(this.Size.Width, 19);
            // Bottom Right
            panelPoints[14] = new Point(this.Size.Width, this.Size.Height - GRAPHICSIZE + 2);
            panelPoints[15] = new Point(this.Size.Width - 1, this.Size.Height - GRAPHICSIZE + 6);
            panelPoints[16] = new Point(this.Size.Width - 3, this.Size.Height - GRAPHICSIZE + 10);
            panelPoints[17] = new Point(this.Size.Width - 5, this.Size.Height - GRAPHICSIZE + 13);
            panelPoints[18] = new Point(this.Size.Width - 9, this.Size.Height - GRAPHICSIZE + 16);
            panelPoints[19] = new Point(this.Size.Width - 14, this.Size.Height - GRAPHICSIZE + 18);
            panelPoints[20] = new Point(this.Size.Width - 19, this.Size.Height);
            // Bottom Left
            panelPoints[21] = new Point(GRAPHICSIZE - 2, this.Size.Height - 0);
            panelPoints[22] = new Point(GRAPHICSIZE - 6, this.Size.Height - 1);
            panelPoints[23] = new Point(GRAPHICSIZE - 10, this.Size.Height - 3);
            panelPoints[24] = new Point(GRAPHICSIZE - 13, this.Size.Height - 5);
            panelPoints[25] = new Point(GRAPHICSIZE - 16, this.Size.Height - 9);
            panelPoints[26] = new Point(GRAPHICSIZE - 18, this.Size.Height - 14);
            panelPoints[27] = new Point(GRAPHICSIZE - 19, this.Size.Height - 19);
            // Draw Path
            System.Drawing.Drawing2D.GraphicsPath PanelPath = new System.Drawing.Drawing2D.GraphicsPath();
            PanelPath.AddPolygon(panelPoints);
            Region PanelRegion = new Region(PanelPath);
            this.Region = PanelRegion;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);



            Graphics g = e.Graphics;
            g.FillRectangle(Brushes.White, ClientRectangle);

            Brush linearGradientBrush = new LinearGradientBrush(new Rectangle(0, 0, Width, Height),
                Color.FromArgb(220, 220, 220), Color.FromArgb(244, 244, 244), 90);
            g.FillRectangle(linearGradientBrush, new Rectangle(0, 0, Width, Height));
            linearGradientBrush.Dispose();

            int bar_height = 30;
            linearGradientBrush = new LinearGradientBrush(new Rectangle(0, 0, Width, bar_height),
                Color.FromArgb(125, 125, 125), Color.FromArgb(85, 85, 85), 90);
            g.FillRectangle(linearGradientBrush, new Rectangle(0, 0, Width, bar_height));
            linearGradientBrush.Dispose();

        }


        private void RoboMessagePanel_SizeChanged(object sender, EventArgs e)
        {
            //  re-draw region
            drawRegion();

            // place corner graphics
            //Corner_BL.Location = new Point(0, this.Size.Height - GRAPHICSIZE);
            //Corner_BR.Location = new Point(this.Size.Width - GRAPHICSIZE, this.Size.Height - GRAPHICSIZE);
            //Corner_TR.Location = new Point(this.Size.Width - GRAPHICSIZE, 0);
/*
            // resize and move border rectangles
            // Rect1 = top border
            Rect1.Size = new Size(this.Size.Width - (2 * GRAPHICSIZE), 26);
            Rect1.Location = new Point(GRAPHICSIZE, 0);

            // Rect2 = bottom Border
            //Rect2.Size = new Size(Rect1.Size.Width, BORDERWIDTH + 1);
            //Rect2.Location = new Point(GRAPHICSIZE, this.Size.Height - (BORDERWIDTH + 1));

            // Rect3 = left hand Border
            Rect3.Size = new Size(BORDERWIDTH, this.Size.Height - (GRAPHICSIZE + TOPGRAPHIC_HEIGHT));
            Rect3.Location = new Point(0, TOPGRAPHIC_HEIGHT);

            // Rect4 = right hand Border
            Rect4.Size = Rect3.Size;
            Rect4.Location = new Point(this.Size.Width - BORDERWIDTH - 1, TOPGRAPHIC_HEIGHT);
            */
            // txt_MessageBox re- size
            txtMessage.Location = new Point(25, 35);

            int x = 20;
            int y = this.Height - 90;
            ProgressPanel.Location = new Point(x, y);
            ProgressPanel.Size = new Size((this.Width - (2 * x)), 50);

            MSGprogress.Size = new Size(ProgressPanel.Size.Width - 8, ProgressPanel.Size.Height - 8);
            MSGprogress.Location = new Point(4, 4);
        }

        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            button1.BackgroundImage = Properties.Resources.BUTTON_PROMPTBUTTON3;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.BackgroundImage = Properties.Resources.BUTTON_PROMPTBUTTON0;
        }

        private void button2_MouseDown(object sender, MouseEventArgs e)
        {
            button2.BackgroundImage = Properties.Resources.BUTTON_PROMPTBUTTON3;
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            button2.BackgroundImage = Properties.Resources.BUTTON_PROMPTBUTTON0;
        }

        private void button_Click(object sender, EventArgs e)
        {
            this.CurrentValue = txtValue.Text;
            DialogResult = ((GUI_Controls.Button_Rectangle)sender).DialogResult;
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
            ActiveControl = label_WindowTitle;// Corner_TL;
        }


        public string CurrentValue { get; set; }

        private void txtValue_MouseClick(object sender, MouseEventArgs e)
        {
            if (RoboSep_UserConsole.KeyboardEnabled)
            {
                // Show window overlay
                RoboSep_UserConsole.getInstance().frmOverlay.Show();
                RoboSep_UserConsole.getInstance().frmOverlay.BringToFront();

                // Create keybaord control
                GUI_Controls.Keyboard newKeyboard =
                    new GUI_Controls.Keyboard(RoboSep_UserConsole.getInstance(),
                        txtValue, null, RoboSep_UserConsole.getInstance().frmOverlay, false);
                newKeyboard.Show();

                // add keyboard control to user console "track form"
                RoboSep_UserConsole.getInstance().addForm(newKeyboard, newKeyboard.Offset);
                //RoboSep_UserConsole.myUserConsole.addForm(newKeyboard.overlay, newKeyboard.overlayOffset);
            }
        }
    }

}