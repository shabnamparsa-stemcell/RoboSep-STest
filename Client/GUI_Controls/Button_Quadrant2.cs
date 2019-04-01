using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

using Tesla.Common;

namespace GUI_Controls
{
    public class Button_Quadrant2 : GUIButton
    {
        enum Direction { eDirectionNone, eDirectionDescending, eDirectionAscending }
        enum StatusBits { eStatusBitsNone = 0x00, 
                          eStatusBitsSelected = 0x01, 
                          eStatusBitsCancelled = 0x02, 
                          eStatusBitsEnabled = 0x04 }

        public bool Cancelled
        {
            get { return cancelled; }
            set { cancelled = value; Check = cancelled; }
        }
        private bool cancelled;

        public bool Selected
        {
            get { return selected; }
            set { selected = value; Check = !selected; }
        }
        private bool selected;

        public Button_Quadrant2()
        {
            // add graphics for states
            BackImage.Clear();
            BackImage.Add(Properties.Resources.QuadrantNumber_UNSELECTED);
            BackImage.Add(Properties.Resources.QuadrantNumber_UNSELECTED);
            BackImage.Add(Properties.Resources.QuadrantNumber_SELECTED);
            BackImage.Add(Properties.Resources.QuadrantNumber_SELECTED);
            BackImage.Add(Properties.Resources.QuadrantNumber_DISABLED);
  
            this.BackgroundImage = BackImage[0];
            this.Size = BackImage[0].Size;
            disableImage = BackImage[4];
            // make quadrants toggle buttons
            this.setToggle(true);
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

        protected override void GUIButton_Click(object sender, EventArgs e)
        {
        }

        protected override void GUIButton_MouseEnter(object sender, EventArgs e)
        {
        }

        protected override void tempTimer_Tick(object sender, EventArgs e)
        {
            // timer for transition graphics
            tempTimer.Stop();
            tempTimer.Enabled = false;
        }

        protected override void GUIButton_MouseLeave(object sender, EventArgs e)
        {
            // check if button is set to toggle mode
            if (Toggle)
            {
                ChooseBackgroundImage();
            }
            // treat as regular button
            else
            {
                tempTimer.Enabled = false;
                this.BackgroundImage = BackImage[0];
            }
        }

        protected override void GUIButton_MouseDown(object sender, MouseEventArgs e)
        {
            // check if button is toggle type button
            if (Toggle)
            { // do nothing
            }
            else
            {
                this.BackgroundImage = BackImage[3];
                click_timer.Enabled = true;
                click_timer.Start();
            }
        }

        protected override void GUIButton_MouseHover(object sender, MouseEventArgs e)
        {
            // check if button is toggle type button
            if (Toggle)
            { // do nothing
            }
            else
            {
                this.BackgroundImage = BackImage[3];
                click_timer.Enabled = true;
                click_timer.Start();
            }
        }

        protected override void GUIButton_Paint(object sender, PaintEventArgs e)
        {
            ChooseBackgroundImage();
            base.GUIButton_Paint(sender, e);

            // format text
            StringFormat theFormat = new StringFormat();
            theFormat.Alignment = StringAlignment.Center;
            theFormat.LineAlignment = StringAlignment.Center;

            // draw text
            SolidBrush theBrush = new SolidBrush(this.ForeColor);
            e.Graphics.DrawString(buttonLabel, this.Font,
                theBrush, new Point(this.Size.Width / 2, this.Size.Height / 2), theFormat);
            theBrush.Dispose();

            if (Cancelled == true)
            {
                Utilities.DrawCancelledMarker(this.BackgroundImage.Width, this.BackgroundImage.Height, e.Graphics);
            }

        }

        private void ChooseBackgroundImage()
        {
            StatusBits eStatusBit = !selected ? StatusBits.eStatusBitsNone : StatusBits.eStatusBitsSelected;
            eStatusBit |= !cancelled ? StatusBits.eStatusBitsNone : StatusBits.eStatusBitsCancelled;
            eStatusBit |= !Enabled ? StatusBits.eStatusBitsNone : StatusBits.eStatusBitsEnabled;
            int imgIndex = 0;
            switch ((int)eStatusBit)
            {
                case 0:
                    imgIndex = 4;
                    break;
                case 1:
                    imgIndex = 2;
                    break;
                case 2:
                    imgIndex = 4;
                    break;
                case 3:
                    imgIndex = 0;
                    break;
                case 4:
                    imgIndex = 0;
                    break;
                case 5:
                    imgIndex = 2;
                    break;
                case 6:
                    imgIndex = 0;
                    break;
                case 7:
                    imgIndex = 0;
                    break;
                default:
                    imgIndex = 0;
                    break;
            }
            this.BackgroundImage = BackImage[imgIndex];
        }


        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Button_Quadrant2
            // 
            this.BackgroundImage = global::GUI_Controls.Properties.Resources.QuadrantNumber_UNSELECTED;
            this.Name = "Button_Quadrant2";
            this.ResumeLayout(false);

        }
    }
}
