using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;

namespace GUI_Controls
{
    public class Button_Home : Button_Circle
    {
        public Button_Home()
        {
            // add specific graphics
            BackImage.Add(Properties.Resources.Home_STD);
            BackImage.Add(Properties.Resources.Home_OVER);
            BackImage.Add(Properties.Resources.Home_OVER);
            BackImage.Add(Properties.Resources.Home_CLICK);

            this.BackgroundImage = BackImage[0];
            this.Size = BackImage[0].Size;
        }

        protected override void Dispose(bool disposing)
        {
            int i;
            for (i = 0; i < BackImage.Count; i++)
            {
                BackImage[i].Dispose();
            }
            BackImage.Clear();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Button_Home
            // 
            this.BackgroundImage = global::GUI_Controls.Properties.Resources.Home_OVER;
            this.Name = "Button_Home";
            this.ResumeLayout(false);

        }

        protected override void GUIButton_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            // Do nothing
        }
    }
}
