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
            BackImage.Add(Properties.Resources.Button_Home0);
            BackImage.Add(Properties.Resources.Button_Home1);
            BackImage.Add(Properties.Resources.Button_Home2);
            BackImage.Add(Properties.Resources.Button_Home3);

            this.BackgroundImage = BackImage[0];
            this.Size = BackImage[0].Size;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Button_Home
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackgroundImage = global::GUI_Controls.Properties.Resources.BUTTON_HOMECANCEL0;
            this.Name = "Button_Home";
            this.ResumeLayout(false);

        }

        protected override void GUIButton_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            // Do nothing
        }
    }
}
