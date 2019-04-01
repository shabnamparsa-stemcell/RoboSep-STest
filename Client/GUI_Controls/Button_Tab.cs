using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Tesla.Common;

namespace GUI_Controls
{
    public class Button_Tab : GUIButton
    {
        private Color TxtColor_CLICK;
        private Color TxtColor_STD;

        public Button_Tab()
        {
            InitializeComponent();

            // add graphics for states
            BackImage.Add(Properties.Resources.TabButton_STD);
            BackImage.Add(Properties.Resources.TabButton_OVER);
            BackImage.Add(Properties.Resources.TabButton_OVER);
            BackImage.Add(Properties.Resources.TabButton_CLICK);
            this.BackgroundImage = BackImage[0];
            this.Size = BackImage[0].Size;

            // Apply new region
            GraphicsPath graphicsPath = Utilities.CalculateControlGraphicsPath(Properties.Resources.TabButton_STD);

            if (this.Region != null)
                this.Region.Dispose();
            this.Region = new Region(graphicsPath);

            TxtColor_CLICK = System.Drawing.Color.White;
            TxtColor_STD = SystemColors.GrayText;
        }

        protected override void Dispose(bool disposing)
        {
            int i;
            for (i = 0; i < BackImage.Count; i++)
            {
                BackImage[i].Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Button_Tab
            // 
            this.BackColor = System.Drawing.Color.Transparent;
            this.BackgroundImage = global::GUI_Controls.Properties.Resources.TabButton_STD;
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "Button_Tab";
            this.Size = new System.Drawing.Size(227, 53);
            this.ResumeLayout(false);

        }

        protected override void GUIButton_Load(object sender, EventArgs e)
        {
            this.ForeColor = TxtColor_STD;
            this.Refresh();
        }

        protected override void GUIButton_MouseEnter(object sender, EventArgs e)
        {
            // check if button is set to toggle mode
            Bitmap theBitmap;
            if (Toggle)
            {
                if (Checked)
                {
                    this.ForeColor = TxtColor_CLICK;
                    this.BackgroundImage = BackImage[3];
 
                    // Apply new region
                    theBitmap = new Bitmap(BackImage[3]);
                    GraphicsPath graphicsPath = Utilities.CalculateControlGraphicsPath(theBitmap);
                    theBitmap.Dispose();

                    if (this.Region != null)
                        this.Region.Dispose();
                    this.Region = new Region(graphicsPath);
                }
                else
                {
                    this.ForeColor = TxtColor_STD;
                    this.BackgroundImage = BackImage[1];
                    // Apply new region
                    theBitmap = new Bitmap(BackImage[1]);
                    GraphicsPath graphicsPath = Utilities.CalculateControlGraphicsPath(theBitmap);
                    theBitmap.Dispose();
                    if (this.Region != null)
                        this.Region.Dispose();
                    this.Region = new Region(graphicsPath);
                }
            }
            // treat as regular button
            // change image and set timer
            else
            {
                this.ForeColor = TxtColor_STD;
                this.BackgroundImage = BackImage[1];

                // Apply new region
                theBitmap = new Bitmap(BackImage[1]);
                GraphicsPath graphicsPath = Utilities.CalculateControlGraphicsPath(theBitmap);
                theBitmap.Dispose();

                if (this.Region != null)
                    this.Region.Dispose();
                this.Region = new Region(graphicsPath);
                tempTimer.Enabled = true;
                tempTimer.Start();
            }
        }

        protected override void GUIButton_MouseLeave(object sender, EventArgs e)
        {
            this.ForeColor = TxtColor_STD;
            Bitmap theBitmap;
            // check if button is set to toggle mode
            if (Toggle)
            {
                if (Checked)
                {
                    this.BackgroundImage = BackImage[2];

                    // Apply new region
                    theBitmap = new Bitmap(BackImage[2]);
                    GraphicsPath graphicsPath = Utilities.CalculateControlGraphicsPath(theBitmap);
                    theBitmap.Dispose();

                    if (this.Region != null)
                        this.Region.Dispose();
                    this.Region = new Region(graphicsPath);
                }
                else
                {
                    this.BackgroundImage = BackImage[0];

                    // Apply new region
                    theBitmap = new Bitmap(BackImage[0]);
                    GraphicsPath graphicsPath = Utilities.CalculateControlGraphicsPath(theBitmap);
                    theBitmap.Dispose();

                    if (this.Region != null)
                        this.Region.Dispose();

                    this.Region = new Region(graphicsPath);
                }
            }
            // treat as regular button
            else
            {
                tempTimer.Enabled = false;

                if (this.Enabled)
                {
                    this.BackgroundImage = BackImage[0];

                    // Apply new region
                    theBitmap = new Bitmap(BackImage[0]);
                    GraphicsPath graphicsPath = Utilities.CalculateControlGraphicsPath(theBitmap);
                    theBitmap.Dispose();
                    if (this.Region != null)
                        this.Region.Dispose();

                    this.Region = new Region(graphicsPath);
                }
            }
        }

        protected override void GUIButton_MouseDown(object sender, MouseEventArgs e)
        {
            // check if button is toggle type button
            if (Toggle)
            { // do nothing
            }
            else
            {
                this.ForeColor = TxtColor_CLICK;
                this.BackgroundImage = BackImage[3];

                // Apply new region
                Bitmap theBitmap;
                theBitmap = new Bitmap(BackImage[3]);
                GraphicsPath graphicsPath = Utilities.CalculateControlGraphicsPath(theBitmap);
                theBitmap.Dispose();

                if (this.Region != null)
                    this.Region.Dispose();
                this.Region = new Region(graphicsPath);

                click_timer.Enabled = true;
                click_timer.Start();
            }
        }

        protected override void GUIButton_MouseHover(object sender, MouseEventArgs e)
        {
            // check if button is toggle type button
            if (Toggle)
            { // do nothing
            }
            else
            {
                this.ForeColor = TxtColor_CLICK;
                this.BackgroundImage = BackImage[3];

                // Apply new region
                GraphicsPath graphicsPath = Utilities.CalculateControlGraphicsPath(new Bitmap(BackImage[3]));
                if (this.Region != null)
                    this.Region.Dispose();

                this.Region = new Region(graphicsPath);

                click_timer.Enabled = true;
                click_timer.Start();
            }
        }

        protected override void GUIButton_Paint(object sender, PaintEventArgs e)
        {
            if (buttonLabel != null && buttonLabel != string.Empty)
            {
                //	draw text
                using (Brush brushText = new SolidBrush(this.ForeColor))
                {
                    string sTemp = Utilities.TruncatedString(this.Font, buttonLabel, this.Width, this.Margin.Left, e.Graphics);
                    e.Graphics.DrawString(sTemp, this.Font,
                        brushText, new Point(this.Size.Width / 2, this.Size.Height * 3/7), theFormat);
                    brushText.Dispose();

                }
            }
        }

    }
}
