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
    public partial class Button_Keyboard : UserControl
    {
        private string keyLabel;
        private List<Image> BackImage = new List<Image>();
        private bool Toggle = false;
        private bool Checked = false;
        private Timer resetButtonTimer;

        public Button_Keyboard()
        {
            InitializeComponent();
            BackImage.Add(Properties.Resources.KeybaordButton_L0);
            BackImage.Add(Properties.Resources.KeybaordButton_M0);
            BackImage.Add(Properties.Resources.KeybaordButton_R0);
            BackImage.Add(Properties.Resources.KeybaordButton_L1);
            BackImage.Add(Properties.Resources.KeybaordButton_M1);
            BackImage.Add(Properties.Resources.KeybaordButton_R1);
            resetButtonTimer = new Timer();
            resetButtonTimer.Interval = 400;
            this.Text = "  ";
        }

        [Description("Text to display on button"),
        Category("Values"),
        DefaultValue(""),
        Browsable(true)]
        public override string Text
        {
            get
            {
                return keyLabel;
            }
            set
            {
                keyLabel = value;
                this.Refresh();
            }
        }

        private void Button_Keyboard_Paint(object sender, PaintEventArgs e)
        {
            keyLabel = this.Text;
            StringFormat theFormat = new StringFormat();
            theFormat.Alignment = StringAlignment.Center;
            theFormat.LineAlignment = StringAlignment.Center;
            Font theFont = new Font("Agency FB", 16, FontStyle.Bold);
            SolidBrush theBrush = new SolidBrush(Color.FromArgb(233, 233, 233));
            e.Graphics.DrawString(keyLabel, theFont, theBrush
                , new Point(this.Size.Width / 2, this.Size.Height / 2), theFormat);
            theBrush.Dispose();
            theFont.Dispose();
        }

        private void Button_Keyboard_Load(object sender, EventArgs e)
        {
            keyLabel = this.Text;
        }

        private void Button_Keyboard_MouseDown(object sender, MouseEventArgs e)
        {
            if (Checked)
            {
                EdgeL.Image = BackImage[0];
                this.BackgroundImage = BackImage[1];
                EdgeR.Image = BackImage[2];
            }
            else
            {
                EdgeL.Image = BackImage[3];
                this.BackgroundImage = BackImage[4];
                EdgeR.Image = BackImage[5];
            }
        }

        private void Button_Keyboard_Leave(object sender, EventArgs e)
        {
            if (Checked)
            {
                // do nothing
            }
            else
            {
                EdgeL.Image = BackImage[0];
                this.BackgroundImage = BackImage[1];
                EdgeR.Image = BackImage[2];
            }
        }

        private void Button_Keyboard_Resize(object sender, EventArgs e)
        {
            int width = this.Size.Width;
            this.Size = new Size(width, 50);
            EdgeL.Location = new Point(0,0);
            EdgeR.Location = new Point(this.Size.Width - 7, 0);
        }

        public bool check
        {
            get
            {
                return Checked;
            }
            set
            {
                Checked = value;
                if (value)
                {
                    EdgeL.Image = BackImage[3];
                    this.BackgroundImage = BackImage[4];
                    EdgeR.Image = BackImage[5];
                }
                else
                {
                    EdgeL.Image = BackImage[0];
                    this.BackgroundImage = BackImage[1];
                    EdgeR.Image = BackImage[2];
                }
            }
        }

        public void setToggle(bool tog)
        {
            Toggle = tog;
        }
    }
}
