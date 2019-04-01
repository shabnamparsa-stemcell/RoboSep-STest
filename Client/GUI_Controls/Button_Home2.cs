using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace GUI_Controls
{
    public class btn_home2 : Button_Circle
    {
        public btn_home2()
        {
            // add specific graphics
            BackImage.Add(Properties.Resources.carousel_52x52_STD);
            BackImage.Add(Properties.Resources.carousel_52x52_OVER);
            BackImage.Add(Properties.Resources.carousel_52x52_OVER);
            BackImage.Add(Properties.Resources.carousel_52x52_CLICK);

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
            // btn_home2
            // 
            this.BackgroundImage = global::GUI_Controls.Properties.Resources.carousel_52x52_STD;
            this.Name = "btn_home2";
            this.Size = new System.Drawing.Size(52, 52);
            this.Text = "  Button_Home2";
            this.ResumeLayout(false);

        }

        protected override void GUIButton_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            // Do nothing
        }
    }
}
