using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Drawing;

namespace GUI_Controls
{
    public class Button_Circle : GUIButton
    {
        public Size CircleSize;
   
        public Button_Circle()
        {
        // default should not be used for this class
        // default of children classes..
            
        }

        protected override void reDraw()
        {
            if (BackImage.Count > 0)
            {
                // check for smallest image dimension
                int x;
                if (BackImage[0].Size.Height > BackImage[0].Size.Width)
                { x = BackImage[0].Size.Width - 1; }
                else
                { x = BackImage[0].Size.Height - 1; }
                CircleSize = new Size(x, x);

                // re draw boundry region based on size of graphic
                System.Drawing.Drawing2D.GraphicsPath CircPath = new System.Drawing.Drawing2D.GraphicsPath();
                Rectangle tempRect = new Rectangle();
                tempRect.Size = CircleSize;
                CircPath.AddEllipse(tempRect);
                Region CircRegion = new Region(CircPath);
                this.Region = CircRegion;
                this.Size = CircleSize;
            }
        }

        protected override void GUIButton_Load(object sender, EventArgs e)
        {
            try
            {
                if (BackImage.Count > 0)
                {
                    CircleSize = new Size(BackImage[0].Size.Width - 1, BackImage[0].Size.Height - 1);

                    // Define Region for Circular button
                    System.Drawing.Drawing2D.GraphicsPath CircPath = new System.Drawing.Drawing2D.GraphicsPath();
                    Rectangle tempRect = new Rectangle();
                    tempRect.Size = CircleSize;
                    CircPath.AddEllipse(tempRect);
                    Region CircRegion = new Region(CircPath);
                    this.Region = CircRegion;
                    this.Size = CircleSize;
                }
                //reDraw();
            }
            catch
            {
                //button1.Text = "FAIL";
            }
        }

        protected override void GUIButton_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            // Do nothing
        }

    }
}
