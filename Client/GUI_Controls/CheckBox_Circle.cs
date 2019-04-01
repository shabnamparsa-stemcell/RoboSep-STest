using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GUI_Controls
{
    public class CheckBox_Circle : GUIButton
    {
        public CheckBox_Circle()
        {
            // add graphics for states
            BackImage.Add(Properties.Resources.GE_BTN08S_unselect_STD);
            BackImage.Add(Properties.Resources.GE_BTN08S_unselect_STD);
            BackImage.Add(Properties.Resources.GE_BTN07S_select_CLICK);
            BackImage.Add(Properties.Resources.GE_BTN07S_select_CLICK);

            this.setToggle(true);
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
            base.Dispose(disposing);
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
            // CheckBox2
            // 
            this.BackgroundImage = global::GUI_Controls.Properties.Resources.GE_BTN08S_unselect_STD;
            this.Name = "CheckBox2";
            this.Size = new System.Drawing.Size(32, 32);
            this.ResumeLayout(false);

        }
    }
    
}
