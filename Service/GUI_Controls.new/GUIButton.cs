using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Xml;
using System.Text;
using System.Windows.Forms;

using Tesla.Common;

namespace GUI_Controls
{
    public partial class GUIButton : UserControl
    {
        public List<Image> BackImage = new List<Image>();
        public Image disableImage;
        protected bool Toggle = false;
        public bool Checked = false;

        private StringFormat theFormat;

        protected string buttonLabel = "";

        public GUIButton()
        {
            this.Text = "  ";
            InitializeComponent();
            this.DoubleBuffered = true;

            theFormat = new StringFormat();
            theFormat.Alignment = StringAlignment.Center;
            theFormat.LineAlignment = StringAlignment.Center;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Text to display on button"),
        Category("Values"),
        DefaultValue(""),
        Browsable(true)]
        public string Text
        {
            get
            {
                return buttonLabel;
            }
            set
            {
                buttonLabel = value;
                this.Refresh();
            }
        }

        public void ChangeGraphics(List<Image> alternateGraphics)
        // function to change button graphics 
        // Changes button size to image size
        {
            BackImage.Clear();
            // add images to button image list
            for (int i = 0; i < 4; i++)
            {
                BackImage.Add(alternateGraphics[i]);
            }
            this.BackgroundImage = BackImage[0];
            this.Size = BackImage[0].Size;
            for (int i = 1; i < BackImage.Count; i++)
            {
                int newSize = BackImage[i].Size.Width * BackImage[i].Size.Height;
                if (newSize > (this.Size.Width * this.Size.Height))
                    this.Size = BackImage[i].Size;
            }
            //reDraw();
        }

        public void setToggle(bool ToggleOnOff)
        // turns button into toggle button
        // toggle buttons store on / off value
        // manage click and hover events differently
        {
            Toggle = ToggleOnOff;
        }

        public bool Check
        {
            // get set method to check the on / off value of a toggle button (see above)
            get
            {
                return Checked;
            }
            set
            {
                string s = Name;
                // if setting to current value
                if (value == Checked)
                {
                    Checked = value;
                }
                // if changing value, also change image (opposite toggle mode graphic)
                else
                {
                    Checked = value;
                    if (Checked)
                    {
                        this.BackgroundImage = BackImage[2];
                    }
                    else
                    {
                        this.BackgroundImage = BackImage[0];
                    }
                }
            }
        }

        protected virtual void reDraw()
        {
            // method defines region for buttons with non rectangular shape
            // used when graphic is changed for image to be larger / smaller
            // re-draws region based on properties & math
            try
            {
                // Define Region for rect button
                // can be overridden by child class shapes
                System.Drawing.Drawing2D.GraphicsPath RectPath = new System.Drawing.Drawing2D.GraphicsPath();
                Rectangle tempRect = new Rectangle();
                tempRect.Size = new Size(BackImage[0].Size.Width - 1, BackImage[0].Size.Height - 1);
                RectPath.AddRectangle(tempRect);
                Region RectRegion = new Region(RectPath);
                this.Region = RectRegion;
            }
            catch
            {
                // do nothing;
            }
        }

        protected virtual void GUIButton_Load(object sender, EventArgs e)
        {
            // virtual function can be overloaded for different buttont types
            // draws region, draws text if there is any
        }

        public void disable(bool BOOLEAN)
        {
            if (BOOLEAN && this.Enabled)
            {
                this.Enabled = false;
                this.Visible = true;
                this.BackgroundImage = disableImage;
                click_timer.Stop();
            }
            else if (!BOOLEAN && !this.Enabled)
            {
                this.BackgroundImage = BackImage[0];
                this.Enabled = true;
                this.Visible = true;
            }
        }
        
        protected virtual void GUIButton_Click(object sender, EventArgs e)
        {
            // check if button is set to toggle mode
            // if not, check mouse down
            string test = Name;

            if (Toggle)
            {
                if (Checked)
                {
                    this.BackgroundImage = BackImage[0];
                    Checked = false;
                    //this.Enabled = false;
                }
                else
                {
                    this.BackgroundImage = BackImage[2];
                    Checked = true;
                    //this.Enabled = false;
                }
            }
        }

        protected virtual void GUIButton_DoubleClick(object sender, EventArgs e)
        {
            // check if button is set to toggle mode
            // if not, check mouse down
            if (Toggle)
            {
                if (Checked)
                {
                    this.BackgroundImage = BackImage[0];
                    Checked = false;
                    //this.Enabled = false;
                }
                else
                {
                    this.BackgroundImage = BackImage[2];
                    Checked = true;
                    //this.Enabled = false;
                }
            }
        }

        protected virtual void GUIButton_MouseEnter(object sender, EventArgs e)
        {
            // check if button is set to toggle mode
            if (Toggle)
            {
                if (Checked)
                {
                    this.BackgroundImage = BackImage[3];
                }
                else
                {
                    this.BackgroundImage = BackImage[1];
                }
            }
            // treat as regular button
            // change image and set timer
            else
            {
                this.BackgroundImage = BackImage[1];
                tempTimer.Enabled = true;
                tempTimer.Start();
            }
        }

        protected virtual void tempTimer_Tick(object sender, EventArgs e)
        {
            // timer for transition graphics
            tempTimer.Stop();
            tempTimer.Enabled = false;
            this.BackgroundImage = BackImage[2];
        }

        protected virtual void GUIButton_MouseLeave(object sender, EventArgs e)
        {
            // check if button is set to toggle mode
            if (Toggle)
            {
                if (Checked)
                {
                    this.BackgroundImage = BackImage[2];
                }
                else
                {
                    this.BackgroundImage = BackImage[0];
                }
            }
            // treat as regular button
            else
            {
                tempTimer.Enabled = false;

                if (this.Enabled)
                    this.BackgroundImage = BackImage[0];
            }
        }

        protected virtual void GUIButton_MouseDown(object sender, MouseEventArgs e)
        {
            // check if button is toggle type button
            if (Toggle)
            { // do nothing
            }
            else
            {
                this.BackgroundImage = BackImage[3];
                click_timer.Enabled = true;
                click_timer.Start();
            }
        }

        protected virtual void GUIButton_MouseHover(object sender, MouseEventArgs e)
        {
            // check if button is toggle type button
            if (Toggle)
            { // do nothing
            }
            else
            {
                this.BackgroundImage = BackImage[3];
                click_timer.Enabled = true;
                click_timer.Start();
            }
        }

        protected virtual void GUIButton_Paint(object sender, PaintEventArgs e)
        {
            if (buttonLabel != null && buttonLabel != string.Empty)
            {
				//	draw text
				using (Brush brushText = new SolidBrush(this.ForeColor))
                {

                    string sTemp = Utilities.TruncatedString(this.Font, buttonLabel, this.Width, this.Margin.Left, e.Graphics);
                    e.Graphics.DrawString(sTemp, this.Font,
                        brushText, new Point(this.Size.Width / 2, this.Size.Height / 2), theFormat);
				}                
            }
        }

        protected virtual void click_timer_Tick(object sender, EventArgs e)
        {
            click_timer.Stop();
            this.BackgroundImage = BackImage[0];
        }

    }
}
