using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;

namespace GUI_Controls
{
    public class Button_Filter : Button_Circle
    {
        // setup

        public Button_Filter ()
        {
            // add specific graphics
            BackImage.Add(Properties.Resources.BUTTON_HUMAN0);
            BackImage.Add(Properties.Resources.BUTTON_HUMAN1);
            BackImage.Add(Properties.Resources.BUTTON_HUMAN2a);
            BackImage.Add(Properties.Resources.BUTTON_HUMAN3a);
            this.setToggle(true);
            this.BackgroundImage = BackImage[0];
            //button1.Size = BackImage[0].Size;
            this.Size = BackImage[0].Size;   
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Button_Filter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackgroundImage = global::GUI_Controls.Properties.Resources.BUTTON_HOMECANCEL0;
            this.Name = "Button_Filter";
            this.ResumeLayout(false);

        }

        protected override void GUIButton_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            // Do nothing
        }
    }
}
