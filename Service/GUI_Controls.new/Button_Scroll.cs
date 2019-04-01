using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace GUI_Controls
{

    public class Button_Scroll : GUIButton
    {
        public Button_Scroll ()
        {
            // add specific graphics
            BackImage.Add(Properties.Resources.GE_BTN04M_down_arrow_STD);
            BackImage.Add(Properties.Resources.GE_BTN04M_down_arrow_OVER);
            BackImage.Add(Properties.Resources.GE_BTN04M_down_arrow_OVER);
            BackImage.Add(Properties.Resources.GE_BTN04M_down_arrow_CLICK);
            this.setToggle(false);
            this.BackgroundImage = BackImage[0];
            //button1.Size = BackImage[0].Size;
            this.Size = BackImage[0].Size;   
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Button_Scroll
            // 
            this.BackgroundImage = global::GUI_Controls.Properties.Resources.GE_BTN04M_down_arrow_STD;
            this.Name = "Button_Scroll";
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

        protected override void click_timer_Tick(object sender, EventArgs e)
        {
            click_timer.Stop();
        }


        protected override void GUIButton_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            // Do nothing
        }
    }
}
