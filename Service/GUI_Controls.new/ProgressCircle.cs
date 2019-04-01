using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GUI_Controls
{
    [DefaultProperty("BlockSize")]
    public partial class ProgressCircle : UserControl
    {
        private int value = 30;

        [DefaultValue(30)]
        public int Value
        {
            get { return value; }
            set
            {
                if (minimum <= value && value <= maximum)
                {
                    this.value = value;
                    Invalidate();
                }
            }
        }

        private Color gradientStartColor = Color.Purple;

        [DefaultValue(typeof(Color), "Purple")]
        public Color GradientStartColor
        {
            get { return gradientStartColor; }
            set
            {
                gradientStartColor = value;
                Invalidate();
            }
        }

        private Color gradientEndColor = Color.Fuchsia;

        [DefaultValue(typeof(Color), "Fuchsia")]
        public Color GradientEndColor
        {
            get { return gradientEndColor; }
            set
            {
                gradientEndColor = value;
                Invalidate();
            }
        }

        // Angle of due North clockwise from x-axis
        private float startAngle = 270.0f;
 
        [DefaultValue(270.0f)]
        public float StartAngle     
        {
            get { return startAngle; }
            set
            {
                startAngle = value;
            }
        }

        private int penWidth  = 30;

        [DefaultValue(30)]
        public int PenWidth
        {
            get { return penWidth; }
            set
            {
                penWidth = value;
            }
        }

        private int size = 50;

        [DefaultValue(50)]
        public int SquareSize
        {
            get { return size; }
            set
            {
                size = value;
                Size = new Size(size, size);
            }
        }

        private int maximum = 360;

        [DefaultValue(360)]
        public int Maximum
        {
            get { return maximum; }
            set
            {
                if (value >= minimum)
                    maximum = value;
            }
        }

        private int minimum = 0;

        [DefaultValue(0)]
        public int Minimum
        {
            get { return minimum; }
            set
            {
                if (value <= maximum)
                    minimum = value;
            }
        }

        public ProgressCircle()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rc1 = this.ClientRectangle;
            Rectangle rc2 = Rectangle.Inflate(this.ClientRectangle, -penWidth, -penWidth);

            // Percentage of a complete circle to draw
            float percent = (float)(this.value - minimum) / (float)Math.Max(1, (maximum - minimum));

            using (LinearGradientBrush lb = new LinearGradientBrush(rc1, gradientStartColor, gradientEndColor, LinearGradientMode.Horizontal))
            {
                using (Pen pen = new Pen(lb, penWidth))
                {
                    // Draws a clockwise arc starting from due north
                    e.Graphics.DrawArc(pen, rc2, startAngle, 360.0f * percent);
                }
            }

            // Calling the base class OnPaint
            base.OnPaint(e);
        }
    }
}