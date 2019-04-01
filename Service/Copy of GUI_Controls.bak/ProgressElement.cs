using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace GUI_Controls
{
    public partial class ProgressElement : UserControl
    {
        private int EstimatedCompletionTime; // given in seconds
        private int myProgressColour;
        private int tickCount;
        private Color[,] colours;
        private ProgressBar myParentProgressBar;    // created so that when element progresses, refresh parent bar

        public ProgressElement()
        {
            InitializeComponent();
            EstimatedCompletionTime = 30;
            myProgressColour = 0;

            setColours();

            // set colour of "progress tracker"
            rectProgress.FillColor = colours[1, myProgressColour];
            rectProgress.BorderColor = colours[1, myProgressColour];

            // set background colour to less saturated, darker version of progress colour
            this.BackColor = colours[0, myProgressColour];
            drawRegion();
        }
                
        public ProgressElement(ProgressBar myParentBar, int est, int progressColour )
        {
            InitializeComponent();
            EstimatedCompletionTime = est;
            myProgressColour = progressColour;
            myParentProgressBar = myParentBar;

            setColours();

            // set colour of "progress tracker"
            rectProgress.FillColor = colours[1, myProgressColour];
            rectProgress.BorderColor = colours[1, myProgressColour];

            // set background colour to less saturated, darker version of progress colour
            this.BackColor = colours[0, myProgressColour];
            //drawRegion();
        }

        private void setColours()
        {
            Color[] active = new Color[12] {
                Color.FromArgb(71, 187, 96),
                Color.FromArgb(12, 245, 181),
                Color.FromArgb(71, 187, 187),
                Color.FromArgb(145, 220, 237),
                Color.FromArgb(92, 134, 217),
                Color.FromArgb(120, 137, 195),
                Color.FromArgb(155, 122, 190),
                Color.FromArgb(209, 118, 184),
                Color.FromArgb(222, 71, 49),
                Color.FromArgb(217, 121, 58),
                Color.FromArgb(239, 235, 66),
                Color.FromArgb(139, 220, 71)};
            Color[] inactive = new Color[12] {
                Color.FromArgb(97, 118, 101),
                Color.FromArgb(100, 142, 131),
                Color.FromArgb(102, 123, 123),
                Color.FromArgb(137, 177, 196),
                Color.FromArgb(114, 132, 169),
                Color.FromArgb(118, 122, 132),
                Color.FromArgb(88, 80, 96),
                Color.FromArgb(167, 144, 161),
                Color.FromArgb(112, 84, 80),
                Color.FromArgb(127, 110, 99),
                Color.FromArgb(180, 179, 148),
                Color.FromArgb(138, 152, 125)};
            colours = new Color[2, 12];
            for (int i = 0; i < 12; i++)
            {
                colours[0, i] = inactive[i];
                colours[1, i] = active[i];
            }

            active = new Color[12] {
                Color.FromArgb(71, 187, 96),
                Color.FromArgb(12, 245, 181),
                Color.FromArgb(71, 187, 187),
                Color.FromArgb(145, 220, 237),
                Color.FromArgb(71, 110, 187),
                Color.FromArgb(120, 137, 195),
                Color.FromArgb(127, 85, 172),
                Color.FromArgb(171, 87, 148),
                Color.FromArgb(222, 71, 49),
                Color.FromArgb(217, 121, 58),
                Color.FromArgb(239, 235, 66),
                Color.FromArgb(139, 220, 71)};
             inactive = new Color[12] {
                Color.FromArgb(97, 118, 101),
                Color.FromArgb(100, 142, 131),
                Color.FromArgb(102, 123, 123),
                Color.FromArgb(172, 186, 189),
                Color.FromArgb(77, 85, 99),
                Color.FromArgb(118, 122, 132),
                Color.FromArgb(88, 80, 96),
                Color.FromArgb(102, 87, 99),
                Color.FromArgb(112, 84, 80),
                Color.FromArgb(127, 110, 99),
                Color.FromArgb(180, 179, 148),
                Color.FromArgb(138, 152, 125)};

        }

        protected void drawRegion()
        {
            int Chamfer = 2;
            List<Point> pnts = new List<Point>();
            pnts.Add(new Point(Chamfer, 0));
            pnts.Add(new Point(this.Size.Width - Chamfer, 0));
            pnts.Add(new Point(this.Size.Width, Chamfer));
            pnts.Add(new Point(this.Size.Width, this.Size.Height - Chamfer));
            pnts.Add(new Point(this.Size.Width - Chamfer, this.Size.Height));
            pnts.Add(new Point(Chamfer, this.Size.Height));
            pnts.Add(new Point(0, this.Size.Height - Chamfer));
            pnts.Add(new Point(0, Chamfer));

            // creates a Hexagonal Form shape
            System.Drawing.Drawing2D.GraphicsPath octPath = new System.Drawing.Drawing2D.GraphicsPath();
            octPath.AddPolygon(new Point[] { pnts[0], pnts[1], pnts[2], pnts[3], pnts[4], pnts[5], pnts[6], pnts[7] });
            Region HexRegion = new Region(octPath);
            this.Region = HexRegion;
        }

        private void ProgressElement_Load(object sender, EventArgs e)
        {
            //if (myProgressColour == Color.Red)
            //    this.Start();
        }

        public void Start()
        {
            rectProgress.Visible = true;
            EstimatedProgressTimer.Enabled = true;
            EstimatedProgressTimer.Start();
        }

        public void pause()
        {
            EstimatedProgressTimer.Stop();
        }

        public void setProgress(int PercentCompleted)
        {
            double dCompleted = (double)PercentCompleted/ 100;
            int setTicks = (int)((double)EstimatedCompletionTime * dCompleted);
            tickCount = setTicks;
            double width = (double)this.Size.Width * (double)PercentCompleted/ 100.00;
            rectProgress.Size = new Size((int)width, this.Size.Height);
            myParentProgressBar.Refresh();
            //Application.DoEvents();
        }

        public void Finished()
        {
            EstimatedProgressTimer.Stop();
            rectProgress.Visible = true;
            rectProgress.Size = this.Size;
            myParentProgressBar.Refresh();
            //Application.DoEvents();
        }

        public void Completed()
        {
            setProgress(100);
            myParentProgressBar.Refresh();
            //Application.DoEvents();
        }

        public int elementProgress
        {
            get
            {
                double ticks = (double)tickCount;
                double estCompletion = (double)EstimatedCompletionTime;
                int percentComplete = (int)(ticks / estCompletion * 100.00);
                return percentComplete;
            }
        }


        private void EstimatedProgressTimer_Tick(object sender, EventArgs e)
        {
            // will tick every 1.0 seconds and update "progress"
            if (tickCount < EstimatedCompletionTime)
            {
                double ticks = (double)tickCount;
                double estCompletion = (double)EstimatedCompletionTime;
                double thisWidth = (double)this.Size.Width;
                double percentComplete = ticks / estCompletion;
                // calc size of progress rectablge based on % complete
                int rectWidth = (int)(thisWidth * percentComplete);
                rectProgress.Size = new Size(rectWidth, this.Size.Height);
                tickCount++;
                myParentProgressBar.Refresh();
            }
            else
            {
                EstimatedProgressTimer.Stop();
                rectProgress.Size = this.Size;
                //myParentProgressBar.Refresh();
            }
        }

        private void ProgressElement_Resize(object sender, EventArgs e)
        {
            rectProgress.Size = new Size(rectProgress.Size.Width, this.Size.Height);
            //drawRegion();
        }
    }
}
