using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Tesla.Service
{
    public partial class PassthroughButton : UserControl
    {
        PictureButton realButton=null;
        public PassthroughButton()
        {
            InitializeComponent();

            this.BackColor = Color.Transparent;
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }
        public PictureButton RealButton
        {
            get { return realButton; }
            set { realButton = value; }
        }
        // When the mouse button is pressed, set the "pressed" flag to true 
        // and invalidate the form to cause a repaint.  The .NET Compact Framework 
        // sets the mouse capture automatically.
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (realButton != null && realButton.Enabled)
            {
                realButton.DoOnMouseDown(e);
            }
        }

        // When the mouse is released, reset the "pressed" flag 
        // and invalidate to redraw the button in the unpressed state.
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (realButton != null && realButton.Enabled)
            {
                realButton.DoOnMouseUp(e);
            }
        }
        // Override the OnPaint method to draw the background image and the text.
        protected override void OnPaint(PaintEventArgs e)
        {
            // Draw a border around the outside of the 
            // control to look like Pocket PC buttons.
            if (PictureButton.DrawDebugRect)
            {
                e.Graphics.DrawRectangle(new Pen(Color.Black), 0, 0,
                    this.ClientSize.Width - 1, this.ClientSize.Height - 1);
            }

            base.OnPaint(e);
        }

        protected override void OnClick(EventArgs e)
        {
            if (realButton != null && realButton.Enabled)
            {
                realButton.DoOnClick(e);
            }
            else
            {
                base.OnClick(e);
            }
        }
    }
}
