using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace GUI_Controls
{
    public class Button_Rectangle : GUIButton, IButtonControl
    {
        List<Image> Ilist_backup = new List<Image>();
        //public Image disableImage;

        public Button_Rectangle()
        {
            // add graphics for states
            BackImage.Add(Properties.Resources.Button_RECT0);
            BackImage.Add(Properties.Resources.Button_RECT1);
            BackImage.Add(Properties.Resources.Button_RECT2);
            BackImage.Add(Properties.Resources.Button_RECT3);
            Ilist_backup = BackImage;

            this.BackgroundImage = BackImage[0];
            //button1.Size = BackImage[0].Size;
            this.Size = BackImage[0].Size;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Button_Rectangle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackgroundImage = global::GUI_Controls.Properties.Resources.BUTTON_HOMECANCEL0;
            this.Name = "Button_Rectangle";
            this.ResumeLayout(false);

        }

        /*public void disable(bool BOOLEAN)
        {
            if (BOOLEAN && this.Enabled)
            {
                //for (int i = 0; i < BackImage.Count; i++)
                //{
                //    BackImage[i] = Properties.Resources.Button_RECT_Disable;
                //}
                this.Enabled = false;
                this.BackgroundImage = disableImage;
            }
            else if (!BOOLEAN && !this.Enabled)
            {
                this.BackgroundImage = BackImage[0];
                this.Enabled = true;
            }
        }*/

        private DialogResult result;
        public DialogResult DialogResult
        {
            get
            {
                return result;
            }
            set
            {
                result = value;
            }
        }

        void IButtonControl.NotifyDefault(bool value)
        {
        }

        void IButtonControl.PerformClick()
        {
            this.OnClick(EventArgs.Empty);
        }
    }
}
