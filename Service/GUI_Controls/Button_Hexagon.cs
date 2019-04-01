using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Drawing;

namespace GUI_Controls
{
    public class Button_Hexagon : GUIButton
    {
        public Button_Hexagon()
        {
            SuspendLayout();
            BackImage.Add(Properties.Resources.HEX_SAMPLE0);
            BackImage.Add(Properties.Resources.HEX_SAMPLE1);
            BackImage.Add(Properties.Resources.HEX_SAMPLE2);
            BackImage.Add(Properties.Resources.HEX_SAMPLE3);

            this.BackgroundImage = BackImage[0];
            this.Size = BackImage[0].Size;
            reDraw();
            ResumeLayout();
        }

        protected override void reDraw()
        {
            int X = 5;
            int Y = 4;
            List<Point> hexPoints = new List<Point>();
            hexPoints.Add(new Point(0 + X, 71 + Y));
            hexPoints.Add(new Point(41 + X, 0 + Y));
            hexPoints.Add(new Point(124 + X, 0 + Y));
            hexPoints.Add(new Point(169 + X, 76 + Y));
            hexPoints.Add(new Point(128 + X, 146 + Y));
            hexPoints.Add(new Point(44 + X, 146 + Y));

            // creates a Hexagonal Form shape
            System.Drawing.Drawing2D.GraphicsPath HexPath = new System.Drawing.Drawing2D.GraphicsPath();
            HexPath.AddPolygon(new Point[] {hexPoints[0], hexPoints[1], hexPoints[2], hexPoints[3], hexPoints[4], hexPoints[5] });
            Region HexRegion = new Region(HexPath);
            this.Region = HexRegion;
            this.Size = new Size(180, 156);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Button_Hexagon
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackgroundImage = global::GUI_Controls.Properties.Resources.HEX_SYSTEM0;
            this.Name = "Button_Hexagon";
            this.Size = new System.Drawing.Size(170, 147);
            this.ResumeLayout(false);
        }

        protected override void GUIButton_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            // Do nothing
        }
    }
}
