using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Xml;
using System.Text;
using System.Windows.Forms;

namespace GUI_Controls
{
    public partial class MultiList_RoboPanel : UserControl
    {
        private int BORDERSIZE;
        private const int HEADERSIZE = 133;
        private const int SCROLLSIZE = 17;
        public MultiList_RoboPanel()
        {
            InitializeComponent();
        }

        private void MultiList_RoboPanel_Resize(object sender, EventArgs e)
        {
            // Listview size change
            listView_robostat.Location = new Point(BORDERSIZE, 37);
            listView_robostat.Size = new Size(this.Size.Width - (2 * BORDERSIZE), this.Size.Height - listView_robostat.Location.Y - BORDERSIZE);
            listView_robostat.Columns[2].Width = this.Size.Width - (2 * BORDERSIZE) - (2 * HEADERSIZE) - SCROLLSIZE; // make message box take up more space as it is scaled

            // top border
            Border_top.Location = new Point(0, 0);
            Border_top.Size = new Size(this.Size.Width, BORDERSIZE);

            // left border
            Border_left.Location = new Point(0, BORDERSIZE);
            Border_left.Size = new Size(BORDERSIZE, this.Height - (2 * BORDERSIZE));

            // bottom border
            Border_bottom.Location = new Point(0, this.Size.Height - BORDERSIZE);
            Border_bottom.Size = new Size(this.Size.Width, BORDERSIZE);

            // right border
            Border_right.Location = new Point(this.Size.Width - BORDERSIZE, BORDERSIZE);
            Border_right.Size = new Size(BORDERSIZE, this.Size.Height - (2 * BORDERSIZE));

            // header devide
            header_devide.Location = new Point(BORDERSIZE, 35);
            header_devide.Size = new Size(this.Size.Width - (2 * BORDERSIZE), BORDERSIZE / 2);
        }

        public void Load_RunStatData()
        {
            
        }

        private void MultiList_RoboPanel_Load(object sender, EventArgs e)
        {
            // Get data to fill listview
            Load_RunStatData();

            // Bordersize based on margin Left
            BORDERSIZE = Margin.Left;

            // off colour every 2nd item in listview
            for (int i = 0; i < listView_robostat.Items.Count; i++)
            {
                if ((i % 2) == 1)
                {
                    listView_robostat.Items[i].BackColor = Color.FromArgb(215, 209, 215);
                }
                else
                {
                    //listView_robostat.Items[i].BackColor = Color.FromArgb(233, 233, 233);
                }
            }
            header_devide.BringToFront();
        }

        private void listView_robostat_DrawItem(object sender, DrawListViewItemEventArgs e)
        {

        }
    }
}
