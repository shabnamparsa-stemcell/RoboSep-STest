using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;

namespace GUI_Controls
{
    public class Button_SmallPink : GUIButton
    {
        public Button_SmallPink()
        {
            // add graphics for states
            BackImage.Add(Properties.Resources.Button_SERVICE0);
            BackImage.Add(Properties.Resources.Button_SERVICE1);
            BackImage.Add(Properties.Resources.Button_SERVICE2);
            BackImage.Add(Properties.Resources.Button_SERVICE3);

            this.BackgroundImage = BackImage[0];
            this.Size = BackImage[0].Size;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Button_SmallPink
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackgroundImage = global::GUI_Controls.Properties.Resources.Button_SERVICE0;
            this.Name = "Button_SmallPink";
            this.ResumeLayout(false);
        }

        protected override void GUIButton_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            // Do nothing
        }
    }
}
