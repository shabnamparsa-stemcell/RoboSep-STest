using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace GUI_Controls
{
    public partial class SizableLabel : UserControl
    {
        private string textString = string.Empty;
        private string labelString;
        private StringAlignment myStringAlign;
        private StringFormatFlags myFormatFlags = 0;

        public SizableLabel()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Text"),
        Category("Values"),
        DefaultValue(""),
        Browsable(true)]
        public new string Text
        {
            get
            {
                return textString;
            }
            set
            {
                textString = value;
                txtChange(textString);
                this.Refresh();
            }
        }

        [Description("Text Alignment"),
        Category("Values"),
        DefaultValue(StringAlignment.Near),
        Browsable(true)]
        public StringAlignment TxtAlignment
        {
            get
            {
                return myStringAlign;
            }
            set
            {
                myStringAlign = value;
                this.Refresh();
            }
        }

        [Description("Text Format Flags"),
        Category("Values"),
        DefaultValue(0),
        Browsable(true)]
        public StringFormatFlags TxtFormatFlags
        {
            get
            {
                return myFormatFlags;
            }
            set
            {
                myFormatFlags = value;
                this.Refresh();
            }
        }

        private void txtChange(string txt)
        {

            int width = this.Size.Width;
            int maxHeight = this.Size.Height;

            Size txtSize = new Size(width, int.MaxValue);
            TextFormatFlags flags = TextFormatFlags.WordBreak;

            // dertermine size of 1 line:
            int currentLineHeight = TextRenderer.MeasureText("a1", this.Font, txtSize, flags).Height;

            // determine size of current string
            txtSize = TextRenderer.MeasureText(txt, this.Font, txtSize, flags);

            StringBuilder finalString = new StringBuilder();

            if (txtSize.Height > currentLineHeight)
            {
                string[] stringBreakdown = txt.Split();

                StringBuilder testString = new StringBuilder();

                testString.Append(stringBreakdown[0]);
                finalString.Append(stringBreakdown[0]);
                for (int i = 1; i < stringBreakdown.Length; i++)
                {
                    testString.Append(" ");
                    testString.Append(stringBreakdown[i]);
                    int strHeight = TextRenderer.MeasureText(testString.ToString(), this.Font, txtSize, flags).Height;

                    if (strHeight > currentLineHeight)
                    {
                        finalString.Append("\n");
                        finalString.Append(stringBreakdown[i]);
                        currentLineHeight = strHeight;
                    }
                    else
                    {
                        finalString.Append(" ");
                        finalString.Append(stringBreakdown[i]);
                    }
                }
                labelString = finalString.ToString();
            }
            else
            {
                labelString = txt;
            }
            this.Refresh();
        }

        

        private void SizableLabel_Paint(object sender, PaintEventArgs e)
        {
            // format text
            StringFormat theFormat = new StringFormat();
            theFormat.Alignment = myStringAlign;
            theFormat.LineAlignment = StringAlignment.Center;
            theFormat.FormatFlags = myFormatFlags;

            int xStartLocation = 0;
            // set location of text
            switch (myStringAlign)
            {
                case StringAlignment.Near:
                    xStartLocation = 0;
                    break;
                case StringAlignment.Center:
                    xStartLocation = this.Size.Width / 2;
                    break;
                case StringAlignment.Far:
                    xStartLocation = this.Size.Width;
                    break;
            }

            SolidBrush theBrush = new SolidBrush(this.ForeColor);
            // draw text
            e.Graphics.DrawString(labelString, this.Font,
                theBrush, new Point(xStartLocation, this.Size.Height / 2), theFormat);

            theBrush.Dispose();

        }

        private void SizableLabel_Load(object sender, EventArgs e)
        {
            
        }

        private void SizableLabel_Resize(object sender, EventArgs e)
        {
            this.Text = textString;
        }



    }
}
