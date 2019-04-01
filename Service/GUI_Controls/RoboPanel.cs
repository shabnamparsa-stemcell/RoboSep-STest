using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Xml;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace GUI_Controls
{
    public partial class RoboPanel : Panel
    {
        private const int GRAPHICSIZE = 19;
        private const int BORDERWIDTH = 6;
        
        public RoboPanel()
        {
            InitializeComponent();
            drawRegion();
        }

        private void RoboPanel_RegionChanged(object sender, EventArgs e)
        {
            label1.BackColor = Color.Transparent;// FromArgb(233, 233, 233);

            // change control label to name of panel
            label1.ForeColor = Color.White;// FromArgb(62, 6, 70);
            label1.Text = this.AccessibleName;
        }

        private void drawRegion()
        {
            SuspendLayout();
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
            panelPoints[7] = new Point(this.Size.Width, 0); //new Point(this.Size.Width - GRAPHICSIZE + 2, 0);
            panelPoints[8] = new Point(this.Size.Width, 0); //new Point(this.Size.Width - GRAPHICSIZE + 6, 1);
            panelPoints[9] = new Point(this.Size.Width, 0); //new Point(this.Size.Width - GRAPHICSIZE + 10, 3);
            panelPoints[10] = new Point(this.Size.Width, 0); //new Point(this.Size.Width - GRAPHICSIZE + 13, 5);
            panelPoints[11] = new Point(this.Size.Width, 0); //new Point(this.Size.Width - GRAPHICSIZE + 16, 9);
            panelPoints[12] = new Point(this.Size.Width, 0); //new Point(this.Size.Width - GRAPHICSIZE + 18, 14);
            panelPoints[13] = new Point(this.Size.Width, 0); //new Point(this.Size.Width, 19);
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

            ResumeLayout();
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

        private void RoboPanel_SizeChanged(object sender, EventArgs e)
        {
            //  re-draw region
            drawRegion();


            
        }
    }
}
