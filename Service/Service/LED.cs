using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Windows.Forms;

namespace Tesla.Service
{
    class LED: Control
    {
        public enum LED_MODE
        {
            Black,
            Yellow,
            Green
        }

        Image imgBlack;
        Image imgYellow;
        Image imgGreen;
        Rectangle imgRect;
        LED_MODE mode;

        public LED()
        {
            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            int icon_width = 27;
            int icon_height = 27;
            Bounds = new Rectangle(0, 0, icon_width, icon_height);
            imgRect = new Rectangle(0, 0, icon_width, icon_height);
            imgBlack = Service.Properties.Resources.LED_0;
            imgYellow = Service.Properties.Resources.LED_1;
            imgGreen = Service.Properties.Resources.LED_2;
            ForeColor = Color.White;

            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.BackColor = Color.Transparent;
        }

        public LED_MODE Mode
        {
            get { return mode; }
            set { mode = value; Invalidate(); }
        }

        private void DrawImageAtImageRect(PaintEventArgs e, Image img)
        {
            if (img == null) { return; }
            //int width = this.ClientSize.Width <= img.Width ? 0 : (this.ClientSize.Width - img.Width)/4;
            //int height = this.ClientSize.Height <= img.Height ? 0 : (this.ClientSize.Height - img.Height) / 4;
            //e.Graphics.DrawImage(img, width, height);
            Rectangle r = new Rectangle(imgRect.X, imgRect.Y, img.Width, img.Height);
            e.Graphics.DrawImage(img, r);
        }

        // Override the OnPaint method to draw the background image and the text.
        protected override void OnPaint(PaintEventArgs e)
        {
            if (!this.Enabled)
            {
                DrawImageAtImageRect(e, this.imgBlack);
            }
            else
            {
                switch (mode)
                {
                    case LED_MODE.Black:
                        DrawImageAtImageRect(e, this.imgBlack);
                        break;
                    case LED_MODE.Yellow:
                        DrawImageAtImageRect(e, this.imgYellow);
                        break;
                    case LED_MODE.Green:
                        DrawImageAtImageRect(e, this.imgGreen); 
                        break;
                }
            }


            base.OnPaint(e);
        }
    }
}
