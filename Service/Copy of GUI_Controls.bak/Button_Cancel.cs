using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Drawing;

namespace GUI_Controls
{
    public class Button_Cancel : Button_Circle
    {
        public Button_Cancel()
        {           
            // add specific graphics
            BackImage.Add(Properties.Resources.Button_CANCEL0);
            BackImage.Add(Properties.Resources.Button_CANCEL0);
            BackImage.Add(Properties.Resources.Button_CANCEL0);
            BackImage.Add(Properties.Resources.Button_CANCEL1);

            this.BackgroundImage = BackImage[0];
            //button1.Size = BackImage[0].Size;
            this.Size = new Size(BackImage[0].Size.Width - 1, BackImage[0].Size.Height - 1);

            CircleSize = BackImage[0].Size;
        }

        protected override void GUIButton_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            // Do nothing
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Button_Cancel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackgroundImage = global::GUI_Controls.Properties.Resources.Button_CANCEL0;
            this.Name = "Button_Cancel";
            this.ResumeLayout(false);

        }
    }
}
