using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Imaging;

namespace GUI_Controls
{
    public enum GifAnimationMode
    {
        eUploadingMultipleFiles,
        eExportSingleFile,
        eConnecting
    }

    public partial class RoboMessagePanel5 : Form
    {
        protected Form PreviousForm;
        public Point Offset;
        private const int GRAPHICSIZE = 19;
        private const int TOPGRAPHIC_HEIGHT = 35;
        private const int BORDERWIDTH = 5;
        protected string FormTitle = "RoboSep Message";
        protected string message = "";

        private int LeftMargin = 0;
        private int RightMargin = 0;
        private int DeltaYBetweenControls = 0;
        private int DeltaYBetweenControlAndForm = 0;

        GifAnimationMode animationMode = GifAnimationMode.eUploadingMultipleFiles;

        public RoboMessagePanel5()
        {
            InitializeComponent();
        }

        public RoboMessagePanel5(Form PrevForm, string Title, string Message, GifAnimationMode iAnimationMode)
        {
            InitializeComponent();

            GetSpacings();

            // set previous form
            PreviousForm = PrevForm;
            SuspendLayout();

            this.DoubleBuffered = true;
            this.UpdateStyles();

            // set size of form based on amount of text
            this.Size = determineSize(Message);
            // create region for form based on size
            drawRegion();
            // set message to given message
            message = Message;

            if (this.animationMode != iAnimationMode)
            {
                this.animationMode = iAnimationMode;

                switch (iAnimationMode)
                {
                    case GifAnimationMode.eUploadingMultipleFiles:
                        pictureBox1.Image = global::GUI_Controls.Properties.Resources.uploading_animation;
                        break;
                    case GifAnimationMode.eExportSingleFile:
                        pictureBox1.Image = global::GUI_Controls.Properties.Resources.exporting_animation;
                        break;
                    case GifAnimationMode.eConnecting:
                        pictureBox1.Image = global::GUI_Controls.Properties.Resources.connecting_animation;
                        break;
                }
            }


            if (!string.IsNullOrEmpty(Title))
                FormTitle = Title;

            // set offset
            Offset = new Point((PreviousForm.Size.Width - this.Width) / 2, (PreviousForm.Size.Height - this.Size.Height) / 2 + 25);

            SetDialogProperties();
            ResumeLayout();
            pictureBox1.PerformLayout();
            pictureBox1.Refresh();
        }

        public void setProgress(int PercentProgress)
        {
            int nPercent = PercentProgress;
        }

        public void SetMessageInfo(string sTitle, string sMsg, GifAnimationMode iAnimationMode)
        {
            SuspendLayout();

            this.Size = determineSize(sMsg);

            FormTitle = sTitle;
            message = sMsg;

            if (this.animationMode != iAnimationMode)
            {
                this.animationMode = iAnimationMode;

                switch (iAnimationMode)
                {
                    case GifAnimationMode.eUploadingMultipleFiles:
                        pictureBox1.Image = global::GUI_Controls.Properties.Resources.uploading_animation;
                        break;
                    case GifAnimationMode.eExportSingleFile:
                        pictureBox1.Image = global::GUI_Controls.Properties.Resources.exporting_animation;
                        break;
                    case GifAnimationMode.eConnecting:
                        pictureBox1.Image = global::GUI_Controls.Properties.Resources.connecting_animation;
                        break;
                }
            }

            SetDialogProperties();
            ResumeLayout();
            this.Refresh();
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
            LeftMargin = txtMessage.Bounds.X - this.Bounds.X;
            RightMargin = this.Bounds.X + this.Width - (txtMessage.Bounds.X + txtMessage.Width);
            DeltaYBetweenControls = pictureBox1.Bounds.Y - (txtMessage.Bounds.Y + txtMessage.Height);
            DeltaYBetweenControlAndForm = this.Bounds.Y + this.Height - ( pictureBox1.Bounds.Y + pictureBox1.Height );
        }

        protected Size determineSize(string text)
        {
            int width = txtMessage.Bounds.Width;
            int minHeight = 100;
            int hpadding = 50;
            int maxHeight = 450 - hpadding;

            // Start from text box
            int Y = txtMessage.Bounds.Y;

            Size txtSize = new Size(width, int.MaxValue);
            TextFormatFlags flags = TextFormatFlags.WordBreak | TextFormatFlags.NoPadding | TextFormatFlags.ExternalLeading;

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

            //picture box height
            Y += DeltaYBetweenControls;
            Y += pictureBox1.Bounds.Height;

            Y += DeltaYBetweenControlAndForm;

            if (Y < minHeight)
                Y = minHeight;

            if (Y > maxHeight)
                Y = maxHeight;


            System.Diagnostics.Debug.WriteLine(String.Format("Dialog size: width = {0}, height = {1}", width + hpadding, Y));

            return new Size(width + LeftMargin + RightMargin, Y);
        }

        private void RoboMessagePanel5_RegionChanged(object sender, EventArgs e)
        {
            // change control label to name of Form
            label_WindowTitle.Text = this.Text;
        }

        private void drawRegion()
        {
        }

        private void RoboMessagePanel5_SizeChanged(object sender, EventArgs e)
        {
            // Resize outer rectangle
            int RECwidth, RECheight;
            RECwidth = this.Size.Width - 1;
            RECheight = this.Size.Height - 1;

            rectangleShape1.Size = new Size(RECwidth, RECheight);
            panel1.Size = new Size(this.Size.Width - 3, 37);

            int X = 0, Y = 0;
            X = (this.Size.Width - pictureBox1.Bounds.Width) / 2;
            pictureBox1.Size = new Size(this.Size.Width - LeftMargin - RightMargin, pictureBox1.Height);

            Y = txtMessage.Bounds.Y + txtMessage.Height + DeltaYBetweenControls;
            pictureBox1.Location = new Point(X, Y);
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

        private void RoboMessagePanel5_Deactivate(object sender, EventArgs e)
        {
            this.Activate();
            this.BringToFront();
        }

        private void RoboMessagePanel5_Leave(object sender, EventArgs e)
        {
            this.Activate();
            this.BringToFront();
        }

        private void RoboMessagePanel5_Load(object sender, EventArgs e)
        {
        }
   }

}
