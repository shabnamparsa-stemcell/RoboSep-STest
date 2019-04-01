using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace GUI_Controls
{
    public class Button_Quadrant : GUIButton
    {
        public Button_Quadrant()
        {
            // add graphics for states
            BackImage.Add(Properties.Resources.TOGGLE_QUADRANT2);
            BackImage.Add(Properties.Resources.TOGGLE_QUADRANT2);
            BackImage.Add(Properties.Resources.TOGGLE_QUADRANT1);
            BackImage.Add(Properties.Resources.TOGGLE_QUADRANT1);
            BackImage.Add(Properties.Resources.TOGGLE_QUADRANT0);

            this.BackgroundImage = BackImage[0];
            this.Size = BackImage[0].Size;

            // make quadrants toggle buttons
            this.setToggle(true);
        }

        protected override void GUIButton_Load(object sender, EventArgs e)
        {
            
        }

        protected override void GUIButton_Click(object sender, EventArgs e)
        {
            base.GUIButton_Click(sender, e);
            BackImage[0] = BackImage[4];
            BackImage[1] = BackImage[4];
        }

        protected override void GUIButton_Paint(object sender, PaintEventArgs e)
        {
            // Grab text to be drawn from number at the end of control.Name
            buttonLabel = this.Name[this.Name.Length - 1].ToString();

            // format text
            StringFormat theFormat = new StringFormat();
            theFormat.Alignment = StringAlignment.Center;
            theFormat.LineAlignment = StringAlignment.Center;
            // draw text
            e.Graphics.DrawString(buttonLabel, new Font("Arial", 25, FontStyle.Bold),
                new SolidBrush(Color.White), new Point(20,30), theFormat);
        }
    }
}
