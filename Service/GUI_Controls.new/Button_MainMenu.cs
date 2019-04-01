using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GUI_Controls
{
    public partial class Button_MainMenu : GUI_Controls.GUIButton
    {
         public Button_MainMenu()
        {
            // add graphics for states
            BackImage.Add(Properties.Resources.GE_BTN01M_home_STD);
            BackImage.Add(Properties.Resources.GE_BTN01M_home_OVER);
            BackImage.Add(Properties.Resources.GE_BTN01M_home_OVER);
            BackImage.Add(Properties.Resources.GE_BTN01M_home_CLICK);
            this.BackgroundImage = BackImage[0];
            this.Size = BackImage[0].Size;
        }
        

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Button_MainMenu
            // 
            this.BackgroundImage = global::GUI_Controls.Properties.Resources.GE_BTN01M_home_STD;
            this.Name = "Button_MainMenu";
            this.Size = new System.Drawing.Size(52, 52);
            this.ResumeLayout(false);

        }

        protected override void GUIButton_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            // Do nothing
        }
    }
}
