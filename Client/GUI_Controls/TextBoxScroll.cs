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
    public partial class TextBoxScroll : UserControl
    {
        private Color _Color1 = Color.Black;                    // First default color.
        private Color _Color2 = Color.FromArgb(95, 96, 98);     // Second default color.
        private Font  _TextFont;                                // For the font.

        protected Timer _Timer1;                                // Timer for text animation.
        protected string _sScrollText = null;                   // Text to be displayed in the control.
        private string _Gap = ".    ";
      
        public TextBoxScroll()
        {
            _Timer1 = new Timer();

            // Set the timer speed and properties.
            _Timer1.Interval = 250;
            _Timer1.Enabled = false;
            _Timer1.Tick += new EventHandler( Animate );
        }

        // Add a color property.
        public Color ScrollingText_Color1
        {
            get { return _Color1; }
            set 
            {
                _Color1 = value; 
                Invalidate();
            }
        }

        // Add a color property.
        public Color ScrollingTextColor2
        {
            get { return _Color2; }
            set 
            {
                _Color2 = value; 
                Invalidate();
            }
        }
        public string EndGap 
        {
            get { return _Gap; }
            set
            {
                _Gap = value;
                Invalidate();
            }
        }
        public int ScrollTimerInterval
        {
            get { return _Timer1.Interval; }
            set
            {
                _Timer1.Interval = value;
            }
        }
        public override string Text
        {
            get
            {
                return _sScrollText;
            }
            set
            {
                _sScrollText = value;
                OnTextChanged(EventArgs.Empty);
                Invalidate();
            }
        }

        void Animate( object sender, EventArgs e )
        {
            if (string.IsNullOrEmpty(_sScrollText))
                return;

            // Scroll text by triming one character at a time from the left, then adding that character to the right side of the control to make it look like scrolling text.
            _sScrollText = _sScrollText.Substring( 1, _sScrollText.Length-1 ) + _sScrollText.Substring(0, 1);
            
            Invalidate();
        }

        private bool IsTextTooLong()
        {
            if (string.IsNullOrEmpty(Text))
                return false;

            Font font = new Font(Font.Name, (Height * 3) / 4, Font.Style, GraphicsUnit.Pixel);

            int nAvailableWidth = this.MaximumSize.Width;
            TextFormatFlags format = TextFormatFlags.SingleLine;
            Size p = new Size(nAvailableWidth, int.MaxValue);
            Size textSize = TextRenderer.MeasureText(Text, font, p, format);
            
            font.Dispose();

            if (textSize.Width <= nAvailableWidth)
                return false;
            return true;
        }

        void StartStop( object sender, EventArgs e )
        {
            _Timer1.Enabled = !_Timer1.Enabled;
        }

        protected override void OnTextChanged( EventArgs e )
        {
            if (IsTextTooLong())
            {
                _sScrollText += _Gap;
                _Timer1.Enabled = true;
            }
            else
            {
                _Timer1.Enabled = false;
            }
            base.OnTextChanged( e );
        }

        protected override void OnClick( EventArgs e )
        {
            if (IsTextTooLong())
            {
                _Timer1.Enabled = !_Timer1.Enabled;
            }
            base.OnClick( e );
        }

        protected override void OnPaint( PaintEventArgs pe )
        {
            Brush brush = new SolidBrush(_Color2);

            // Get the font and use it to draw text in the control.  Resize to the height of the control if possible.
            _TextFont = new Font(Font.Name, (Height * 3) / 4, Font.Style, GraphicsUnit.Pixel);

            // Draw the text string in the control.
            pe.Graphics.DrawString(_sScrollText, _TextFont, brush, 0, 0);

            base.OnPaint (pe);

            // Clean up variables..
            brush.Dispose();
            _TextFont.Dispose();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // TextBoxScroll
            // 
            this.Name = "TextBoxScroll";
            this.Size = new System.Drawing.Size(150, 20);
            this.ResumeLayout(false);

        }
    }
}
