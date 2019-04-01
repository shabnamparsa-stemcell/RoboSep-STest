using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Xml;
using System.Text;
using System.Windows.Forms;

// Creates a search filter control
// Buttons toggle and can be used to determine which filters are active
namespace GUI_Controls
{
    public enum BtnDisplayMode
    {
        eBtnFilter,
        eBtnScroll
    }

    public partial class Protocol_SearchBar : UserControl
    {
        // requires this code to access button events within protocol page
        // this.control_SearchBar.button_human.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Human_Click);
        //
        public BtnDisplayMode BtnMode = BtnDisplayMode.eBtnScroll;

        List<Image> iListHuman;
        List<Image> iListMouse;
        List<Image> iListPos;
        List<Image> iListNeg;
        List<Image> iListModeFilter;

        List<Image> iListUp;
        List<Image> iListDown;
        List<Image> iListLeft;
        List<Image> iListRight;
        List<Image> iListModeScroll;

        public Protocol_SearchBar()
        {
            InitializeComponent();

            /*
            SuspendLayout();
            // Draw control region
            Point[] panelPoints = new Point[16];
            int gHght = this.BackgroundImage.Size.Height;
            int gWdth = this.BackgroundImage.Size.Width;
            // top left
            panelPoints[0] = new Point(17, gHght - 0);
            panelPoints[1] = new Point(10, gHght - 4);
            panelPoints[2] = new Point(3, gHght - 10);
            panelPoints[3] = new Point(0, gHght - 17);
            // Top Right
            panelPoints[4] = new Point(0, 17);
            panelPoints[5] = new Point(3, 10);
            panelPoints[6] = new Point(10, 4);
            panelPoints[7] = new Point(17, 0);
            // Bottom Right
            panelPoints[8] = new Point(gWdth - 17, 0);
            panelPoints[9] = new Point(gWdth - 10, 4);
            panelPoints[10] = new Point(gWdth - 3, 10);
            panelPoints[11] = new Point(gWdth - 0, 17);
            // Bottom Left
            panelPoints[12] = new Point(gWdth - 0, gHght - 17);
            panelPoints[13] = new Point(gWdth - 3, gHght - 10);
            panelPoints[14] = new Point(gWdth - 10, gHght - 4);
            panelPoints[15] = new Point(gWdth - 17, gHght - 0);
            // Draw Path
            System.Drawing.Drawing2D.GraphicsPath PanelPath = new System.Drawing.Drawing2D.GraphicsPath();
            PanelPath.AddPolygon(panelPoints);
            Region PanelRegion = new Region(PanelPath);
            this.Region = PanelRegion;
            */

            // Change button images
            // Human
            iListHuman = new List<Image>();
            iListHuman.Add(Properties.Resources.filterHuman_STD);
            iListHuman.Add(Properties.Resources.filterHuman_OVER);
            iListHuman.Add(Properties.Resources.filterHuman_CLICK);
            iListHuman.Add(Properties.Resources.filterHuman_OVER);
     
            // Mouse
            iListMouse = new List<Image>();
            iListMouse.Add(Properties.Resources.filterMouse_STD);
            iListMouse.Add(Properties.Resources.filterMouse_OVER);
            iListMouse.Add(Properties.Resources.filterMouse_CLICK);
            iListMouse.Add(Properties.Resources.filterMouse_OVER);

            // Positive
            iListPos = new List<Image>();
            iListPos.Add(Properties.Resources.filterPos_STD);
            iListPos.Add(Properties.Resources.filterPos_OVER);
            iListPos.Add(Properties.Resources.filterPos_CLICK);
            iListPos.Add(Properties.Resources.filterPos_OVER);

            // Negative
            iListNeg = new List<Image>();
            iListNeg.Add(Properties.Resources.filterNeg_STD);
            iListNeg.Add(Properties.Resources.filterNeg_OVER);
            iListNeg.Add(Properties.Resources.filterNeg_CLICK);
            iListNeg.Add(Properties.Resources.filterNeg_OVER);


            // Filter
            iListModeFilter = new List<Image>();
            iListModeFilter.Add(Properties.Resources.modeFILTER_STD);
            iListModeFilter.Add(Properties.Resources.modeFILTER_OVER);
            iListModeFilter.Add(Properties.Resources.modeFILTER_CLICK);
            iListModeFilter.Add(Properties.Resources.modeFILTER_OVER);


            // Up
            iListUp = new List<Image>();
            iListUp.Add(Properties.Resources.scrollUP_STD);
            iListUp.Add(Properties.Resources.scrollUP_OVER);
            iListUp.Add(Properties.Resources.scrollUP_CLICK);
            iListUp.Add(Properties.Resources.scrollUP_OVER);

            // Down
            iListDown = new List<Image>();
            iListDown.Add(Properties.Resources.scrollDOWN_STD);
            iListDown.Add(Properties.Resources.scrollDOWN_OVER);
            iListDown.Add(Properties.Resources.scrollDOWN_CLICK);
            iListDown.Add(Properties.Resources.scrollDOWN_OVER);

            // Left
            iListLeft = new List<Image>();
            iListLeft.Add(Properties.Resources.scrollLEFT_STD);
            iListLeft.Add(Properties.Resources.scrollLEFT_OVER);
            iListLeft.Add(Properties.Resources.scrollLEFT_CLICK);
            iListLeft.Add(Properties.Resources.scrollLEFT_OVER);
 
            // Right
            iListRight = new List<Image>();
            iListRight.Add(Properties.Resources.scrollRIGHT_STD);
            iListRight.Add(Properties.Resources.scrollRIGHT_OVER);
            iListRight.Add(Properties.Resources.scrollRIGHT_CLICK);
            iListRight.Add(Properties.Resources.scrollRIGHT_OVER);

            // Mode
            iListModeScroll = new List<Image>();
            iListModeScroll.Add(Properties.Resources.modeSCROLL_STD);
            iListModeScroll.Add(Properties.Resources.modeSCROLL_OVER);
            iListModeScroll.Add(Properties.Resources.modeSCROLL_CLICK);
            iListModeScroll.Add(Properties.Resources.modeSCROLL_OVER);

            button_human.ChangeGraphics(iListHuman);
            button_mouse.ChangeGraphics(iListMouse);
            button_positive.ChangeGraphics(iListPos);
            button_negative.ChangeGraphics(iListNeg);
            button_mode.ChangeGraphics(iListModeFilter);
            ResumeLayout();
        }

        private void Protocol_SearchBar_Load(object sender, EventArgs e)
        {

            
        }

        public void getFilters(out string search, out bool Hum, out bool Mou,
            out bool Pos, out bool Neg)
        {
            search = textBox_search.Text;
            Hum = button_human.Check;
            Mou = button_mouse.Check;
            Pos = button_positive.Check;
            Neg = button_negative.Check;
        }

        private void button_human_Click(object sender, EventArgs e)
        {
            if (button_mouse.Check)
            {
                // turn mouse filter off
                button_mouse.Check = false;
            }
        }

        private void button_mouse_Click(object sender, EventArgs e)
        {
            if (button_human.Check)
            {
                // turn human filter off
                button_human.Check = false;
            }
        }

        private void button_positive_Click(object sender, EventArgs e)
        {
            if (button_negative.Check)
            {
                // turn human filter off
                button_negative.Check = false;
            }
        }

        private void button_negative_Click(object sender, EventArgs e)
        {
            if (button_positive.Check)
            {
                // turn human filter off
                button_positive.Check = false;
            }
        }

        private void button_mode_Click(object sender, EventArgs e)
        {
            if (BtnMode == BtnDisplayMode.eBtnScroll)
            {
                button_human.ChangeGraphics(iListHuman);
                button_mouse.ChangeGraphics(iListMouse);
                button_positive.ChangeGraphics(iListPos);
                button_negative.ChangeGraphics(iListNeg);
                button_mode.ChangeGraphics(iListModeScroll);
                BtnMode = BtnDisplayMode.eBtnFilter;
            }
            else
            {
                button_human.ChangeGraphics(iListUp);
                button_mouse.ChangeGraphics(iListDown);
                button_positive.ChangeGraphics(iListLeft);
                button_negative.ChangeGraphics(iListRight);
                button_mode.ChangeGraphics(iListModeFilter);
                BtnMode = BtnDisplayMode.eBtnScroll;
            }
            button_mode.Check = false;
            this.Invalidate();
        }
    }
}
