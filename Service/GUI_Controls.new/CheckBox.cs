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
            BackImage.Add(Properties.Resources.toggle_STD);
            BackImage.Add(Properties.Resources.toggle_STD);
            BackImage.Add(Properties.Resources.toggle_CLICK);
            BackImage.Add(Properties.Resources.toggle_CLICK);

            this.setToggle(true);
            this.BackgroundImage = BackImage[0];
            this.Size = BackImage[0].Size;
        }

        protected override void GUIButton_Load(object sender, EventArgs e)
        {
            Refresh();
        }

        protected override void GUIButton_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            // Do nothing
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // CheckBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackgroundImage = global::GUI_Controls.Properties.Resources.toggle_STD;
            this.Name = "CheckBox";
            this.Size = new System.Drawing.Size(21, 21);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}