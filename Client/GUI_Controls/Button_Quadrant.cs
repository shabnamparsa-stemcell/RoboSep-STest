using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace GUI_Controls
{
    public class Button_Quadrant : GUIButton
    {
        private bool nowInProgress;
        private Color textColour;

        public Button_Quadrant()
        {
            // add graphics for states
            Reset_Button();
            this.BackgroundImage = BackImage[0];
            this.disableImage = Properties.Resources.QuadrantNumber_DISABLED;

            this.Size = BackImage[0].Size;

            textColour = this.ForeColor;
            nowInProgress = false;

            // make quadrants toggle buttons
            this.setToggle(true);
        }

        public void Reset_Button()
        {
            // add graphics for states
            BackImage.Clear();
            BackImage.Add(Properties.Resources.QuadrantNumber);
            BackImage.Add(Properties.Resources.QuadrantNumber_UNSELECTED);
            BackImage.Add(Properties.Resources.QuadrantNumber_SELECTED);
            BackImage.Add(Properties.Resources.QuadrantNumber);
            BackImage.Add(Properties.Resources.QuadrantNumber_DISABLED);
            this.BackgroundImage = BackImage[0];
            this.Check = false;
            this.Visible = true;
        }

        public bool NowInProgress
        {
            get
            {
                return nowInProgress;
            }
            set
            {
                if (nowInProgress != value)
                {
                    nowInProgress = value;
                    UpdateDisplayText(nowInProgress);
                }
            }
        }

        private void UpdateDisplayText(bool bNowInprogress)
        {
            this.ForeColor = nowInProgress ? Color.Yellow : textColour;
        }

        protected override void GUIButton_Load(object sender, EventArgs e)
        {
            Refresh();
        }

        protected override void GUIButton_Click(object sender, EventArgs e)
        {
            base.GUIButton_Click(sender, e);
         //   BackImage[0] = BackImage[4];
          //  BackImage[1] = BackImage[4];
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Button_Quadrant
            // 
            this.BackgroundImage = global::GUI_Controls.Properties.Resources.QuadrantNumber;
            this.Name = "Button_Quadrant";
            this.ResumeLayout(false);

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

    }
}
