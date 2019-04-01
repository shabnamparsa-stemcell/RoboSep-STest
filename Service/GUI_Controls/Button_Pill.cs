using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;

namespace GUI_Controls
{
    public class Button_Pill : GUIButton
    {
        public Button_Pill()
        {
            // add graphics for states
            BackImage.Add(Properties.Resources.BUTTON_ACCEPT0);
            BackImage.Add(Properties.Resources.BUTTON_ACCEPT1);
            BackImage.Add(Properties.Resources.BUTTON_ACCEPT2);
            BackImage.Add(Properties.Resources.BUTTON_ACCEPT3);

            this.BackgroundImage = BackImage[0];
            this.Size = BackImage[0].Size;
        }

        protected override void GUIButton_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            // Do nothing
        }
    }
}
