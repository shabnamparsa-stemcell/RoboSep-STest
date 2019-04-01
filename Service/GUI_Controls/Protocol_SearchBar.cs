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
    public partial class Protocol_SearchBar : UserControl
    {
        // requires this code to access button events within protocol page
        // this.control_SearchBar.button_human.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Human_Click);
        //
        public Protocol_SearchBar()
        {
            InitializeComponent();

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

            // Change button images
            // Mouse
            List<Image> iList = new List<Image>();
            iList.Add(Properties.Resources.BUTTON_MOUSE0);
            iList.Add(Properties.Resources.BUTTON_MOUSE1);
            iList.Add(Properties.Resources.BUTTON_MOUSE2);
            iList.Add(Properties.Resources.BUTTON_MOUSE3);
            button_mouse.ChangeGraphics(iList);
            // Positive
            iList.Clear();
            iList.Add(Properties.Resources.BUTTON_POSITIVE0);
            iList.Add(Properties.Resources.BUTTON_POSITIVE1);
            iList.Add(Properties.Resources.BUTTON_POSITIVE2a);
            iList.Add(Properties.Resources.BUTTON_POSITIVE3a);
            button_positive.ChangeGraphics(iList);
            // Negative
            iList.Clear();
            iList.Add(Properties.Resources.BUTTON_NEGATIVE0);
            iList.Add(Properties.Resources.BUTTON_NEGATIVE1);
            iList.Add(Properties.Resources.BUTTON_NEGATIVE2a);
            iList.Add(Properties.Resources.BUTTON_NEGATIVE3a);
            button_negative.ChangeGraphics(iList);
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
    }
}
