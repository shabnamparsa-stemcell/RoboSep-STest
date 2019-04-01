using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Drawing;

namespace GUI_Controls
{
    public class Button_Cancel : GUIButton
    {
        public Button_Cancel()
        {           
            // add specific graphics
            BackImage.Add(Properties.Resources.Cancel_STD);
            BackImage.Add(Properties.Resources.Cancel_STD);
            BackImage.Add(Properties.Resources.Cancel_STD);
            BackImage.Add(Properties.Resources.Cancel_CLICK);

            this.BackgroundImage = BackImage[0];
            //button1.Size = BackImage[0].Size;
            this.Size = new Size(BackImage[0].Size.Width , BackImage[0].Size.Height );

            //CircleSize = BackImage[0].Size;
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
            this.BackgroundImage = global::GUI_Controls.Properties.Resources.Cancel_STD;
            this.Name = "Button_Cancel";
            this.Size = new System.Drawing.Size(31, 30);
            this.ResumeLayout(false);

        }
    }
}
