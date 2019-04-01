using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
namespace Tesla.Service
{

    // This code shows a simple way to have a 
    // button-like control that has a background image.
    public class PictureButton : Control
    {
        Image backgroundImage, pressedImage;
        Image backgroundImageSelect, pressedImageSelect;
        Image backgroundImageDisabled;
        bool pressed = false;
        bool selectMode = false;
        Rectangle imgRect;

        public static bool DrawDebugRect = false;

        public PictureButton()
        {

            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            this.BackColor = Color.Transparent;
            this.imgRect = new Rectangle(0, 0, 640, 480);

        }

        public Rectangle ImageRect
        {
            get { return this.imgRect; }
            set { this.imgRect = value; }
        }

        // Property for the background image to be drawn behind the button text.
        public Image BackgroundImage
        {
            get
            {
                return this.backgroundImage;
            }
            set
            {
                this.backgroundImage = value;
            }
        }

        // Property for the background image to be drawn behind the button text when
        // the button is pressed.
        public Image PressedImage
        {
            get
            {
                return this.pressedImage;
            }
            set
            {
                this.pressedImage = value;
            }
        }
        // Property for the background image to be drawn behind the button text.
        public Image BackgroundImageSelect
        {
            get
            {
                return this.backgroundImageSelect;
            }
            set
            {
                this.backgroundImageSelect = value;
            }
        }
        public Image BackgroundImageDisabled
        {
            get
            {
                return this.backgroundImageDisabled;
            }
            set
            {
                this.backgroundImageDisabled = value;
            }
        }

        // Property for the background image to be drawn behind the button text when
        // the button is pressed.
        public Image PressedImageSelect
        {
            get
            {
                return this.pressedImageSelect;
            }
            set
            {
                this.pressedImageSelect = value;
            }
        }

        public bool SelectMode
        {
            get { return selectMode; }
            set { selectMode = value; }
        }

        // When the mouse button is pressed, set the "pressed" flag to true 
        // and invalidate the form to cause a repaint.  The .NET Compact Framework 
        // sets the mouse capture automatically.
        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.pressed = true;
            this.Invalidate();
            base.OnMouseDown(e);
        }

        // When the mouse is released, reset the "pressed" flag 
        // and invalidate to redraw the button in the unpressed state.
        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.pressed = false;
            this.Invalidate();
            base.OnMouseUp(e);
        }

        private void DrawImageAtImageRect(PaintEventArgs e, Image img)
        {
            if (img == null) { return; }
            //int width = this.ClientSize.Width <= img.Width ? 0 : (this.ClientSize.Width - img.Width)/4;
            //int height = this.ClientSize.Height <= img.Height ? 0 : (this.ClientSize.Height - img.Height) / 4;
            //e.Graphics.DrawImage(img, width, height);
            Rectangle r = new Rectangle(ImageRect.X, ImageRect.Y, img.Width, img.Height);
            e.Graphics.DrawImage(img, r);
        }

        // Override the OnPaint method to draw the background image and the text.
        protected override void OnPaint(PaintEventArgs e)
        {
            if (!this.Enabled)
            {
                DrawImageAtImageRect(e, this.backgroundImageDisabled);
            }
            else if (selectMode)
            {
                if (this.pressed && this.pressedImageSelect != null)
                    DrawImageAtImageRect(e, this.pressedImageSelect);
                else if (this.backgroundImageSelect != null)
                    DrawImageAtImageRect(e, this.backgroundImageSelect);
            }
            else
            {
                if (this.pressed && this.pressedImage != null)
                    DrawImageAtImageRect(e, this.pressedImage);
                else if (this.backgroundImage != null)
                    DrawImageAtImageRect(e, this.backgroundImage);
            }
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
    }
}
