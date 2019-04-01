using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Drawing;

namespace GUI_Controls
{
    public class CheckBox : GUIButton
    {
        public CheckBox()
        {
            // add graphics for states
            BackImage.Add(Properties.Resources.CHECKBOX0);
            BackImage.Add(Properties.Resources.CHECKBOX0);
            BackImage.Add(Properties.Resources.CHECKBOX1);
            BackImage.Add(Properties.Resources.CHECKBOX1);

            this.setToggle(true);
            this.BackgroundImage = BackImage[0];
            this.Size = BackImage[0].Size;
        }

        protected override void GUIButton_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            // Do nothing
        }
    }
}