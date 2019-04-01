using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GUI_Controls
{
    public partial class CircularProgressControl : UserControl
    {
        private const int DEFAULT_INTERVAL = 60;
        private readonly Color DEFAULT_TICK_COLOR = Color.FromArgb(71, 29, 160);

        private const int DEFAULT_TICK_WIDTH = 2;
        private const int MINIMUM_INNER_RADIUS = 4;
        private const int MINIMUM_OUTER_RADIUS = 8;
        private const int MINIMUM_PEN_WIDTH = 2;

        private const float INNER_RADIUS_FACTOR = 0.175F;
        private const float OUTER_RADIUS_FACTOR = 0.3125F;

        private Size MINIMUM_CONTROL_SIZE = new Size(28, 28);

        public enum Direction
        {
            CLOCKWISE,
            ANTICLOCKWISE
        }

        struct Spoke
        {
            public PointF StartPoint;
            public PointF EndPoint;

            public Spoke(PointF pt1, PointF pt2)
            {
                StartPoint = pt1;
                EndPoint = pt2;
            }
        }

        private Pen _Pen = null;

        private PointF _CentrePt = new PointF();

        private int _Interval;
        private int _InnerRadius = 0;
        private int _OuterRadius = 0;
        private int _AlphaStartValue = 0;
        private int _SpokesCount = 0;
        private int _AlphaChange = 0;
        private int _AlphaLowerLimit = 0;

        private float _StartAngle = 0;
        private float _StartAngleOrigin = 0;
        private float _AngleIncrement = 0;

        private Direction _Rotation;
        private System.Timers.Timer _Timer = null;
        private List<Spoke> _Spokes = null;

        public Color TickColor { get; set; }

        public int NumberOfRotation { get; private set;  }

        // Time interval for each tick.
        public int Interval
        {
            get
            {
                return _Interval;
            }
            set
            {
                if (value > 0)
                {
                    _Interval = value;
                }
                else
                {
                    _Interval = DEFAULT_INTERVAL;
                }
            }
        }

        // Direction of rotation - CLOCKWISE/ANTICLOCKWISE
        public Direction Rotation
        {
            get
            {
                return _Rotation;
            }
            set
            {
                _Rotation = value;
                CalculateSpokesPoints();
            }
        }

        // Angle at which the tick should start
        public float StartAngle
        {
            get
            {
                return _StartAngle;
            }
            set
            {
                _StartAngle = value;
                _StartAngleOrigin = _StartAngle;
                CalculateAlpha();
            }
        }

        // Calculate the Alpha Value of the Spoke drawn at 0 degrees angle
        private void CalculateAlpha()
        {
            if (this.Rotation == Direction.CLOCKWISE)
            {
                if (_StartAngle >= 0)
                {
                    _AlphaStartValue = 255 - (((int)((_StartAngle % 360) / _AngleIncrement) + 1) * _AlphaChange);
                }
                else
                {
                    _AlphaStartValue = 255 - (((int)((360 + (_StartAngle % 360)) / _AngleIncrement) + 1) * _AlphaChange);
                }
            }
            else
            {
                if (_StartAngle >= 0)
                {
                    _AlphaStartValue = 255 - (((int)((360 - (_StartAngle % 360)) / _AngleIncrement) + 1) * _AlphaChange);
                }
                else
                {
                    _AlphaStartValue = 255 - (((int)(((360 - _StartAngle) % 360) / _AngleIncrement) + 1) * _AlphaChange);
                }
            }
        }

        // Initialization
        public CircularProgressControl()
        {
            this.DoubleBuffered = true;

            // Set Default Values
            this.BackColor = Color.Transparent;
            this.TickColor = DEFAULT_TICK_COLOR;
            this.MinimumSize = MINIMUM_CONTROL_SIZE;
            this.Interval = DEFAULT_INTERVAL;
            this.NumberOfRotation = 1;

            // Default starting angle is 12 o'clock
            this.StartAngle = 30;
            this._StartAngleOrigin = this.StartAngle;

            // Default number of Spokes in this control is 12
            _SpokesCount = 12;

            // Default alpha value of the first spoke is 255
            _AlphaStartValue = 255;

            // Set the Lower limit of the Alpha value (The spokes will be shown in 
            // alpha values ranging from 255 to _AlphaLowerLimit)
            _AlphaLowerLimit = 15;

            // Create the Pen
            _Pen = new Pen(TickColor, DEFAULT_TICK_WIDTH);
            _Pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            _Pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;

            // Default rotation direction is clockwise
            this.Rotation = Direction.CLOCKWISE;

            InitializeComponent();

            // Calculate the Spoke Points
            CalculateSpokesPoints();

            _Timer = new System.Timers.Timer(this.Interval);
            _Timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimerElapsed);
        }

        // Calculate the Spoke Points and store them
        private void CalculateSpokesPoints()
        {
            _Spokes = new List<Spoke>();

            // Calculate the angle between adjacent spokes
            _AngleIncrement = (360 / (float)_SpokesCount);

            // Calculate the change in alpha between adjacent spokes
            _AlphaChange = (int)((255 - _AlphaLowerLimit) / _SpokesCount);

            // Calculate the location around which the spokes will be drawn
            int width = (this.Width < this.Height) ? this.Width : this.Height;
            _CentrePt = new PointF(this.Width / 2, this.Height / 2);

            // Calculate the width of the pen which will be used to draw the spokes
            _Pen.Width = (int)(width / 15);
            if (_Pen.Width < MINIMUM_PEN_WIDTH)
                _Pen.Width = MINIMUM_PEN_WIDTH;

            // Calculate the inner and outer radii of the control. The radii should not be less than the
            // Minimum values
            _InnerRadius = (int)(width * INNER_RADIUS_FACTOR);
            if (_InnerRadius < MINIMUM_INNER_RADIUS)
                _InnerRadius = MINIMUM_INNER_RADIUS;

            _OuterRadius = (int)(width * OUTER_RADIUS_FACTOR);
            if (_OuterRadius < MINIMUM_OUTER_RADIUS)
                _OuterRadius = MINIMUM_OUTER_RADIUS;

            float angle = 0;

            for (int i = 0; i < _SpokesCount; i++)
            {
                PointF pt1 = new PointF(_InnerRadius * (float)Math.Cos(ConvertDegreesToRadians(angle)), _InnerRadius * (float)Math.Sin(ConvertDegreesToRadians(angle)));
                PointF pt2 = new PointF(_OuterRadius * (float)Math.Cos(ConvertDegreesToRadians(angle)), _OuterRadius * (float)Math.Sin(ConvertDegreesToRadians(angle)));

                pt1.X += _CentrePt.X;
                pt1.Y += _CentrePt.Y;
                pt2.X += _CentrePt.X;
                pt2.Y += _CentrePt.Y;

                // Create a spoke based on the points generated
                Spoke spoke = new Spoke(pt1, pt2);

                // Add the spoke to the List
                _Spokes.Add(spoke);

                if (Rotation == Direction.CLOCKWISE)
                {
                    angle -= _AngleIncrement;
                }
                else if (Rotation == Direction.ANTICLOCKWISE)
                {
                    angle += _AngleIncrement;
                }
            }
        }

        // Handler for the event when the size of the control changes
        protected override void OnClientSizeChanged(EventArgs e)
        {
            CalculateSpokesPoints();
        }

        // Handle the Tick event
        void OnTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Rotation == Direction.CLOCKWISE)
            {
                _StartAngle += _AngleIncrement;

                if (_StartAngle >= 360)
                    _StartAngle = 0;

                if (_StartAngle == _StartAngleOrigin)
                {
                    NumberOfRotation++;
                }
            }
            else if (Rotation == Direction.ANTICLOCKWISE)
            {
                _StartAngle -= _AngleIncrement;

                if (_StartAngle <= -360)
                    _StartAngle = 0;
            }

            // Change Alpha value accordingly
            _AlphaStartValue -= _AlphaChange;

            if (_AlphaStartValue < _AlphaLowerLimit)
                _AlphaStartValue = 255 - _AlphaChange;

            Invalidate();
        }

        // Handles the Paint Event of the control
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            int alpha = _AlphaStartValue;

            // Render the spokes
            for (int i = 0; i < _SpokesCount; i++)
            {
                _Pen.Color = Color.FromArgb(alpha, this.TickColor);
                e.Graphics.DrawLine(_Pen, _Spokes[i].StartPoint, _Spokes[i].EndPoint);

                alpha -= _AlphaChange;
                if (alpha < _AlphaLowerLimit)
                    alpha = 255 - _AlphaChange;
            }
        }

        // Converts Degrees to Radians
        private double ConvertDegreesToRadians(float degrees)
        {
            return ((Math.PI / (double)180) * degrees);
        }

        // Start the Tick Control rotation
        public void Start()
        {
            if (_Timer != null)
            {
                _Timer.Interval = this.Interval;
                _Timer.Enabled = true;
            }
        }

        // Stop the Tick Control rotation
        public void Stop()
        {
            if (_Timer != null)
            {
                _Timer.Enabled = false;
            }
        }
    }
}
