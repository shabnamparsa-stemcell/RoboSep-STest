using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Windows.Forms;


namespace GUI_Controls
{
    public class Button_Filter : GUIButton
    {
        // setup

        public Button_Filter ()
        {
            // add specific graphics
            BackImage.Add(Properties.Resources.filterHuman_STD);
            BackImage.Add(Properties.Resources.filterHuman_OVER);
            BackImage.Add(Properties.Resources.filterHuman_CLICK);
            BackImage.Add(Properties.Resources.filterHuman_OVER);
            this.setToggle(true);
            this.BackgroundImage = BackImage[0];
            //button1.Size = BackImage[0].Size;
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
            // Button_Filter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackgroundImage = global::GUI_Controls.Properties.Resources.filterHuman_STD;
            this.Name = "Button_Filter";
            this.ResumeLayout(false);

        }
   /*     
        protected override void GUIButton_MouseDown(object sender, MouseEventArgs e)
        {
            base.GUIButton_MouseDown(sender, e);

            if (MouseDownEvent != null)
            {
                MouseDownEvent(sender, e);
            }
        }
    */    
        protected override void GUIButton_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            // Do nothing
        }
    }
}
