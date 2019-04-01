using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
namespace Tesla.Service
{

    // This code shows a simple way to have a 
    // button-like control that has a background image.
    public class RobosepModelButton : Control
    {
        private enum RobosepControl
        {
            None,
            Carousel,
            Theta,
            VertArm,
            TipStripper,
            CarouselClockwise,
            CarouselCClockwise,
            ThetaLeft,
            ThetaRight,
            VertArmUp,
            VertArmDown,
            TipStripperDir
        }
        private RobosepControl mouseDownAxis;
        private RobosepControl mouseUpAxis;
        private RobosepControl activeAxis;

        Rectangle rectCarousel;
        Rectangle rectTheta;
        Rectangle rectVertArm;
        Rectangle rectTipStripper;
        Rectangle rectTipStripperDir;
        Rectangle rectCarouselClockwise;
        Rectangle rectCarouselCClockwise;
        Rectangle rectThetaLeft;
        Rectangle rectThetaRight;
        Rectangle rectVertArmUp;
        Rectangle rectVertArmDown;

        bool pressed = false;

        private bool isStripperArmEngaged;
        public bool IsStripperArmEngaged
        {
            get { return isStripperArmEngaged; }
            set
            {
                isStripperArmEngaged = value;
                this.Invalidate();
            }
        }

        public static bool DrawDebugRect = false;

        public RobosepModelButton()
        {

            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            this.BackColor = Color.Transparent;

            this.Size = new Size(378, 285);

            Image carousel = Service.Properties.Resources.Carousel0;
            Image theta = Service.Properties.Resources.Theta0;
            Image vertArm = Service.Properties.Resources.VertArm0;
            Image tipStripper = Service.Properties.Resources.Stripper0;
            Image carouselClockwise = Service.Properties.Resources.CarouselClockwise0;
            Image carouselCClockwise = Service.Properties.Resources.CarouselCClockwise0;
            Image thetaLeft = Service.Properties.Resources.ThetaLeft0;
            Image thetaRight = Service.Properties.Resources.ThetaRight0;
            Image vertArmUp = Service.Properties.Resources.VertArmUp0;
            Image vertArmDown = Service.Properties.Resources.VertArmDown0;
            Image tipStripperIn = Service.Properties.Resources.StripperIn0;
            Image tipStripperOut = Service.Properties.Resources.StripperOut0;

            rectCarousel = new Rectangle(71, 135, carousel.Width, carousel.Height);
            rectTheta = new Rectangle(208, 2, theta.Width, theta.Height);
            rectVertArm = new Rectangle(308, 36, vertArm.Width, vertArm.Height);

            rectTipStripper = new Rectangle(1, 38, tipStripper.Width, tipStripper.Height);
            rectCarouselClockwise = new Rectangle(245, 237, carouselClockwise.Width, carouselClockwise.Height);
            rectCarouselCClockwise = new Rectangle(52, 116, carouselCClockwise.Width, carouselClockwise.Height);
            rectThetaLeft = new Rectangle(123, 65, thetaLeft.Width, thetaLeft.Height);
            rectThetaRight = new Rectangle(225, 111, thetaRight.Width, thetaRight.Height);
            rectVertArmUp = new Rectangle(341, 50, vertArmUp.Width, vertArmUp.Height);
            rectVertArmDown = new Rectangle(341, 120, vertArmDown.Width, vertArmDown.Height);

            rectTipStripperDir = new Rectangle(21, 38 + tipStripper.Height, tipStripperIn.Width, tipStripperIn.Height);

            activeAxis = RobosepControl.None;
            isStripperArmEngaged = false;
        }


        // When the mouse button is pressed, set the "pressed" flag to true 
        // and invalidate the form to cause a repaint.  The .NET Compact Framework 
        // sets the mouse capture automatically.
        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.pressed = true;
            mouseDownAxis = coord2Axis(e.X, e.Y);
            this.Invalidate();
            base.OnMouseDown(e);
        }

        // When the mouse is released, reset the "pressed" flag 
        // and invalidate to redraw the button in the unpressed state.
        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.pressed = false;
            mouseUpAxis = coord2Axis(e.X,e.Y);

            //send click event
            if (mouseUpAxis == mouseDownAxis)
            {
                switch (mouseUpAxis)
                {
                    case RobosepControl.Carousel:
                        activeAxis = mouseUpAxis;
                        if (CarouselEnable_Click != null) CarouselEnable_Click(this, e);
                        break;
                    case RobosepControl.Theta:
                        activeAxis = mouseUpAxis;
                        if (ThetaEnable_Click != null) ThetaEnable_Click(this, e);
                        break;
                    case RobosepControl.VertArm:
                        activeAxis = mouseUpAxis;
                        if (VertArmEnable_Click!=null) VertArmEnable_Click(this, e);
                        break;
                    case RobosepControl.TipStripper:
                        activeAxis = mouseUpAxis;
                        if (StripperEnable_Click != null) StripperEnable_Click(this, e);
                        break;
                    case RobosepControl.TipStripperDir:
                        if (Stripper_Click != null)
                        {
                            isStripperArmEngaged = !isStripperArmEngaged;
                            Stripper_Click(this, e);
                        }
                        break;
                    case RobosepControl.CarouselClockwise: 
                        if (CarouselDec_Click != null) CarouselDec_Click(this, e);
                        break;
                    case RobosepControl.CarouselCClockwise: 
                        if (CarouselInc_Click != null) CarouselInc_Click(this, e);
                        break;
                    case RobosepControl.ThetaLeft: 
                        if (ThetaDec_Click != null) ThetaDec_Click(this, e);
                        break;
                    case RobosepControl.ThetaRight: 
                        if (ThetaInc_Click != null) ThetaInc_Click(this, e);
                        break;
                    case RobosepControl.VertArmUp: 
                        if (VertArmInc_Click != null) VertArmInc_Click(this, e);
                        break;
                    case RobosepControl.VertArmDown: 
                        if (VertArmDec_Click != null) VertArmDec_Click(this, e);
                        break;

                };

            }

            mouseUpAxis = RobosepControl.None;
            mouseDownAxis = RobosepControl.None;

            this.Invalidate();
            base.OnMouseUp(e);
        }

        private RobosepControl coord2Axis(int x, int y)
        {

            //ORDER MATTERS HERE!!!!
            if (rectTipStripperDir.Contains(x, y) && activeAxis == RobosepControl.TipStripper)
            {
                return RobosepControl.TipStripperDir;
            }
            else if (rectCarouselClockwise.Contains(x, y) && activeAxis == RobosepControl.Carousel)
            {
                return RobosepControl.CarouselClockwise;
            }
            else if (rectCarouselCClockwise.Contains(x, y) && activeAxis == RobosepControl.Carousel)
            {
                return RobosepControl.CarouselCClockwise;
            }
            else if (rectThetaLeft.Contains(x, y) && activeAxis == RobosepControl.Theta)
            {
                return RobosepControl.ThetaLeft;
            }
            else if (rectThetaRight.Contains(x, y) && activeAxis == RobosepControl.Theta)
            {
                return RobosepControl.ThetaRight;
            }
            else if (rectVertArmUp.Contains(x, y) && activeAxis == RobosepControl.VertArm)
            {
                return RobosepControl.VertArmUp;
            }
            else if (rectVertArmDown.Contains(x, y) && activeAxis == RobosepControl.VertArm)
            {
                return RobosepControl.VertArmDown;
            }
            else if (rectVertArm.Contains(x, y))
            {
                return RobosepControl.VertArm;
            }
            else if (rectCarousel.Contains(x, y))
            {
                return RobosepControl.Carousel;
            }
            else if (rectTheta.Contains(x, y))
            {
                return RobosepControl.Theta;
            }
            else if (rectTipStripper.Contains(x, y))
            {
                return RobosepControl.TipStripper;
            }

            return RobosepControl.None;
        }


        // Override the OnPaint method to draw the background image and the text.
        protected override void OnPaint(PaintEventArgs e)
        {
            Image carousel = null;
            Image theta = null;
            Image vertArm = null;
            Image tipStripper = null;
            Image carouselClockwise = null;
            Image carouselCClockwise = null;
            Image thetaLeft = null;
            Image thetaRight = null;
            Image vertArmUp = null;
            Image vertArmDown = null;
            Image tipStripperDir = null;


            carouselClockwise = Service.Properties.Resources.CarouselClockwise0;
            carouselCClockwise = Service.Properties.Resources.CarouselCClockwise0;
            thetaLeft = Service.Properties.Resources.ThetaLeft0;
            thetaRight = Service.Properties.Resources.ThetaRight0;
            vertArmUp = Service.Properties.Resources.VertArmUp0;
            vertArmDown = Service.Properties.Resources.VertArmDown0;
            tipStripperDir = isStripperArmEngaged ? Service.Properties.Resources.StripperOut0 : Service.Properties.Resources.StripperIn0;

            if (!this.Enabled)
            {
                carousel = Service.Properties.Resources.Carousel0;
                theta = Service.Properties.Resources.Theta0;
                vertArm = Service.Properties.Resources.VertArm0;
                tipStripper = Service.Properties.Resources.Stripper0;
            }
            else
            {
                carousel = Service.Properties.Resources.Carousel1;
                theta = Service.Properties.Resources.Theta1;
                vertArm = Service.Properties.Resources.VertArm1;
                tipStripper = Service.Properties.Resources.Stripper1;

                switch (activeAxis)
                {
                    case RobosepControl.Carousel:
                        carouselClockwise = Service.Properties.Resources.CarouselClockwise1; 
                        carouselCClockwise = Service.Properties.Resources.CarouselCClockwise1;
                        carousel = Service.Properties.Resources.Carousel2; break;
                    case RobosepControl.Theta:
                        thetaLeft = Service.Properties.Resources.ThetaLeft1;
                        thetaRight = Service.Properties.Resources.ThetaRight1;
                        theta = Service.Properties.Resources.Theta2; break;
                    case RobosepControl.VertArm:
                        vertArmUp = Service.Properties.Resources.VertArmUp1;
                        vertArmDown = Service.Properties.Resources.VertArmDown1;
                        vertArm = Service.Properties.Resources.VertArm2; break;
                    case RobosepControl.TipStripper:
                        tipStripperDir = isStripperArmEngaged ? Service.Properties.Resources.StripperOut1 : Service.Properties.Resources.StripperIn1;
                        tipStripper = Service.Properties.Resources.Stripper2; break;
                };
            }


            if (this.pressed && mouseDownAxis != RobosepControl.None)
            {
                switch (mouseDownAxis)
                {
                    case RobosepControl.Carousel:
                        carousel = Service.Properties.Resources.Carousel0; break;
                    case RobosepControl.Theta:
                        theta = Service.Properties.Resources.Theta0; break;
                    case RobosepControl.VertArm:
                        vertArm = Service.Properties.Resources.VertArm0; break;
                    case RobosepControl.TipStripper:
                        tipStripper = Service.Properties.Resources.Stripper0; break;
                    case RobosepControl.TipStripperDir:
                        tipStripperDir = isStripperArmEngaged ? Service.Properties.Resources.StripperOut0:Service.Properties.Resources.StripperIn0; break;
                    case RobosepControl.CarouselClockwise:
                        carouselClockwise = Service.Properties.Resources.CarouselClockwise0; break;
                    case RobosepControl.CarouselCClockwise:
                        carouselCClockwise = Service.Properties.Resources.CarouselCClockwise0; break;
                    case RobosepControl.ThetaLeft:
                        thetaLeft = Service.Properties.Resources.ThetaLeft0; break;
                    case RobosepControl.ThetaRight:
                        thetaRight = Service.Properties.Resources.ThetaRight0; break;
                    case RobosepControl.VertArmUp:
                        vertArmUp = Service.Properties.Resources.VertArmUp0; break;
                    case RobosepControl.VertArmDown:
                        vertArmDown = Service.Properties.Resources.VertArmDown0; break;

                };
            }

            e.Graphics.DrawImage(carousel, rectCarousel);
            e.Graphics.DrawImage(theta, rectTheta);
            e.Graphics.DrawImage(vertArm, rectVertArm);
            e.Graphics.DrawImage(tipStripper, rectTipStripper);

            e.Graphics.DrawImage(tipStripperDir, rectTipStripperDir);

            e.Graphics.DrawImage(carouselClockwise, rectCarouselClockwise);
            e.Graphics.DrawImage(carouselCClockwise, rectCarouselCClockwise);
            e.Graphics.DrawImage(thetaLeft, rectThetaLeft);
            e.Graphics.DrawImage(thetaRight, rectThetaRight);
            e.Graphics.DrawImage(vertArmUp, rectVertArmUp);
            e.Graphics.DrawImage(vertArmDown, rectVertArmDown); 

            // Draw the text if there is any.
            if (this.Text.Length > 0)
            {
                SizeF size = e.Graphics.MeasureString(this.Text, this.Font);

                // Center the text inside the client area of the PictureButton.
                e.Graphics.DrawString(this.Text,
                    this.Font,
                    new SolidBrush(this.ForeColor),
                    (this.ClientSize.Width - size.Width) / 2,
                    (this.ClientSize.Height - size.Height) / 2);
            }

            // Draw a border around the outside of the 
            // control to look like Pocket PC buttons.
            if (DrawDebugRect)
            {
                e.Graphics.DrawRectangle(new Pen(Color.Black), 0, 0,
                    this.ClientSize.Width - 1, this.ClientSize.Height - 1);
            }

            base.OnPaint(e);
        }

        internal void DoOnMouseDown(MouseEventArgs e)
        {
            this.OnMouseDown(e);
        }

        internal void DoOnMouseUp(MouseEventArgs e)
        {
            this.OnMouseUp(e);
        }

        internal void DoOnClick(EventArgs e)
        {
            this.OnClick(e);
        }
        
        public event EventHandler CarouselEnable_Click;
        public event EventHandler ThetaEnable_Click;
        public event EventHandler VertArmEnable_Click;
        public event EventHandler CarouselInc_Click;
        public event EventHandler CarouselDec_Click;
        public event EventHandler VertArmInc_Click;
        public event EventHandler VertArmDec_Click;
        public event EventHandler ThetaDec_Click;
        public event EventHandler ThetaInc_Click;
        public event EventHandler Stripper_Click;
        public event EventHandler StripperEnable_Click;


        internal void setInitialAxis()
        {
            activeAxis = RobosepControl.VertArm;
        }
    }
}
