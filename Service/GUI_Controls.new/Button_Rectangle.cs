using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;


namespace GUI_Controls
{
    public class Button_Rectangle : GUIButton
    {
        List<Image> Ilist_backup = new List<Image>();

        int minWidth = 75;
        public Button_Rectangle()
        {
            InitializeComponent();
            // add graphics for states
            BackImage.Add(Properties.Resources.btnLong_STD);
            BackImage.Add(Properties.Resources.btnLong_STD);
            BackImage.Add(Properties.Resources.btnLong_STD);
            BackImage.Add(Properties.Resources.btnLong_CLICK);
            Ilist_backup = BackImage;

            this.BackgroundImage = BackImage[0];
            this.Size = BackImage[0].Size;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Button_Rectangle));
            this.SuspendLayout();
            // 
            // Button_Rectangle
            // 
            this.BackColor = System.Drawing.Color.Transparent;
            this.BackgroundImage = global::GUI_Controls.Properties.Resources.btnLong_STD;
            this.Name = "Button_Rectangle";
            this.Size = new System.Drawing.Size(157, 44);
            this.ResumeLayout(false);
        }

        public int determineWidth(string text)
        {
            if (string.IsNullOrEmpty(text))
                return -1;

            int width = minWidth;
            Size txtSize = new Size(width, int.MaxValue);
            TextFormatFlags flags = TextFormatFlags.LeftAndRightPadding;
            txtSize = TextRenderer.MeasureText(text, this.Font, txtSize, flags);
            width = txtSize.Width;

             if (width < minWidth)
                width = minWidth;

            width = width + Margin.Left + Margin.Right;
            return width;
        }

        private Image resizeImageWidth(Image imgToResize, int nWidth)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            Bitmap b = new Bitmap(nWidth, sourceHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, nWidth, sourceHeight);
            g.Dispose();
            return (Image)b;
        }

        public void SetImageWidth(int nWidth)
        {
            List<Image> lstNewImage = new List<Image>();
            Image b = null;
            foreach (Image img in BackImage)
            {
                if (img == null)
                    continue;
 
                b = resizeImageWidth(img, nWidth);
                if (b == null)
                    continue;

                lstNewImage.Add(b);
            }

            ChangeGraphics(lstNewImage);
            Ilist_backup = BackImage;
        }
    }
}
