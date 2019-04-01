using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace GUI_Controls
{
    public partial class HorizontalTabs : UserControl
    {
        private string[] theTabs;
        public enum tabs { Tab1, Tab2, Tab3 };
        tabs myActiveTab = tabs.Tab1;
        private const int TAB_SIDE_SIZE = 35;
        private const int TAB_HEIGHT = 38;
        
        
        public HorizontalTabs()
        {
            InitializeComponent();
            theTabs = new string[3];

            this.DoubleBuffered = true;
            this.UpdateStyles();
        }

        #region Add Items to Control Properties
        public event EventHandler Tab1_Click;
        public event EventHandler Tab2_Click;
        public event EventHandler Tab3_Click;

        private void handleTabClickEvent(object sender, EventArgs e)
        {
            this.onTabClickEvent(EventArgs.Empty);
        }

        protected virtual void onTabClickEvent(EventArgs e)
        {
            EventHandler handler = this.Tab1_Click;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        [Description("Tab 1 Text"),
        Category("Values"),
        DefaultValue(""),
        Browsable(true)]
        public string Tab1
        {
            get
            {
                return theTabs[0];
            }
            set
            {
                theTabs[0] = value;
                TabText1.Text = value;
                setActiveTab();
            }
        }
        
        [Description("Tab 2 Text"),
        Category("Values"),
        DefaultValue(""),
        Browsable(true)]
        public string Tab2
        {
            get
            {
                return theTabs[1];
            }
            set
            {
                theTabs[1] = value;
                TabText2.Text = value;
                setActiveTab();
            }
        }

        [Description("Tab 3 Text"),
        Category("Values"),
        DefaultValue(""),
        Browsable(true)]
        public string Tab3
        {
            get
            {
                return theTabs[2];
            }
            set
            {
                theTabs[2] = value;
                TabText3.Text = value;
                setActiveTab();
            }
        }

        
        [Description("Tab Active"),
        Category("Values"),
        DefaultValue(tabs.Tab1),
        Browsable(true)]
        public tabs TabActive
        {
            get
            {
                return myActiveTab;
            }
            set
            {
                myActiveTab = value;
                setActiveTab();
            }
        }
        #endregion

        private void ReSizeTabs()
        {
            // set sizes
            int[] tabSize = new int[3];
            for (int i = 0; i < 3; i++)
            {
                int width = getLabelSize(theTabs[i]) + 20;
                width = width > 125 ? width : 125;
                tabSize[i] = width;
            }

            TabText1.Size = new Size(tabSize[0], TAB_HEIGHT);
            TabText2.Size = new Size(tabSize[1], TAB_HEIGHT);
            TabText3.Size = new Size(tabSize[2], TAB_HEIGHT);

            // move objects
            int xPoint = pictureBox1.Size.Width-1;
            TabText1.Location = new Point(xPoint, 0);

            xPoint += TabText1.Size.Width -1;
            pictureBox2.Location = new Point(xPoint, 0);

            xPoint += pictureBox2.Size.Width -1;
            pictureBox3.Location = new Point(xPoint, 0);

            xPoint += pictureBox3.Size.Width - 1;
            TabText2.Location = new Point(xPoint, 0);

            xPoint += TabText2.Size.Width -1;
            pictureBox4.Location = new Point(xPoint, 0);

            xPoint += pictureBox4.Size.Width - 1;
            pictureBox5.Location = new Point(xPoint, 0);

            xPoint += pictureBox5.Size.Width - 1;
            TabText3.Location = new Point(xPoint, 0);

            xPoint += TabText3.Size.Width - 1;
            pictureBox6.Location = new Point(xPoint, 0);
        }

        private int getLabelSize(string txt)
        {
            Size txtSize = new Size(int.MaxValue, TAB_HEIGHT);
            TextFormatFlags flags = TextFormatFlags.WordBreak;

            // dertermine size of 1 line:
            int StrLength = TextRenderer.MeasureText(txt, TabText1.Font, txtSize, flags).Width;
            return StrLength;
        }

        private void setActiveTab()
        {
            ReSizeTabs();
            int numTabsDisplayed = 1;
            for (int i = 1; i < 3; i++)
            {
                if (theTabs[i] != string.Empty && theTabs[i] != null)
                    numTabsDisplayed++;
                else
                    break;
            }

            TabText2.Visible = false;
            TabText3.Visible = false;
            pictureBox3.Visible = false;
            pictureBox4.Visible = false;
            pictureBox5.Visible = false;
            pictureBox6.Visible = false;

            // set graphic elements of tabs
            switch (myActiveTab)
            {
                case tabs.Tab1:
                    TabText1.ForeColor = Color.White;
                    TabText2.ForeColor = Color.FromArgb(95, 96, 98);
                    TabText3.ForeColor = Color.FromArgb(95, 96, 98);
                    pictureBox1.BackgroundImage = Properties.Resources.TabLEFT_ACTIVE;
                    TabText1.BackgroundImage = Properties.Resources.TabBG_ACTIVE;
                    pictureBox2.BackgroundImage = Properties.Resources.TabRIGHT_ACTIVE;

                    if (numTabsDisplayed > 1)
                    {
                        pictureBox3.Visible = true;
                        TabText2.Visible = true;
                        pictureBox4.Visible = true;
                        pictureBox3.BackgroundImage = Properties.Resources.TabLEFT_INACTIVE;
                        TabText2.BackgroundImage = Properties.Resources.TabBG_INACTIVE;
                        pictureBox4.BackgroundImage = Properties.Resources.TabRIGHT_INACTIVE;
                    }
                    if (numTabsDisplayed > 2)
                    {
                        pictureBox5.Visible = true;
                        TabText3.Visible = true;
                        pictureBox6.Visible = true;
                        pictureBox5.BackgroundImage = Properties.Resources.TabLEFT_INACTIVE;
                        TabText3.BackgroundImage = Properties.Resources.TabBG_INACTIVE;
                        pictureBox6.BackgroundImage = Properties.Resources.TabRIGHT_INACTIVE;
                    }

                    break;
                case tabs.Tab2:
                    TabText1.ForeColor = Color.FromArgb(95, 96, 98);
                    TabText2.ForeColor = Color.White;
                    TabText3.ForeColor = Color.FromArgb(95, 96, 98);
                    pictureBox3.Visible = true;
                    TabText2.Visible = true;
                    pictureBox4.Visible = true;
                    pictureBox1.BackgroundImage = Properties.Resources.TabLEFT_INACTIVE;
                    TabText1.BackgroundImage = Properties.Resources.TabBG_INACTIVE;
                    pictureBox2.BackgroundImage = Properties.Resources.TabRIGHT_INACTIVE;
                    pictureBox3.BackgroundImage = Properties.Resources.TabLEFT_ACTIVE;
                    TabText2.BackgroundImage = Properties.Resources.TabBG_ACTIVE;
                    pictureBox4.BackgroundImage = Properties.Resources.TabRIGHT_ACTIVE;
                    if (numTabsDisplayed > 2)
                    {
                        pictureBox5.Visible = true;
                        TabText3.Visible = true;
                        pictureBox6.Visible = true;
                        pictureBox5.BackgroundImage = Properties.Resources.TabLEFT_INACTIVE;                        
                        TabText3.BackgroundImage = Properties.Resources.TabBG_INACTIVE;
                        pictureBox6.BackgroundImage = Properties.Resources.TabRIGHT_INACTIVE;
                    }
                    break;
                case tabs.Tab3:
                    TabText1.ForeColor = Color.FromArgb(95, 96, 98);
                    TabText2.ForeColor = Color.FromArgb(95, 96, 98);
                    TabText3.ForeColor = Color.White;
                    pictureBox3.Visible = true;
                    TabText2.Visible = true;
                    pictureBox4.Visible = true;
                    pictureBox5.Visible = true;
                    TabText3.Visible = true;
                    pictureBox6.Visible = true;
                    pictureBox1.BackgroundImage = Properties.Resources.TabLEFT_INACTIVE;
                    TabText1.BackgroundImage = Properties.Resources.TabBG_INACTIVE;
                    pictureBox2.BackgroundImage = Properties.Resources.TabRIGHT_INACTIVE;
                    pictureBox3.BackgroundImage = Properties.Resources.TabLEFT_INACTIVE;
                    TabText2.BackgroundImage = Properties.Resources.TabBG_INACTIVE;
                    pictureBox4.BackgroundImage = Properties.Resources.TabRIGHT_INACTIVE;
                    pictureBox5.BackgroundImage = Properties.Resources.TabLEFT_ACTIVE;
                    TabText3.BackgroundImage = Properties.Resources.TabBG_ACTIVE;
                    pictureBox6.BackgroundImage = Properties.Resources.TabRIGHT_ACTIVE;
                    break;
            }
        }

        private void Tab1_Clicked(object sender, EventArgs e)
        {
            if (Tab1_Click != null)
                this.Tab1_Click.Invoke(this, EventArgs.Empty);
        }

        private void Tab2_Clicked(object sender, EventArgs e)
        {
            if (Tab2_Click != null)
                this.Tab2_Click.Invoke(this, EventArgs.Empty);
        }

        private void Tab3_Clicked(object sender, EventArgs e)
        {
            if (Tab3_Click != null)
                this.Tab3_Click.Invoke(this, EventArgs.Empty);
        }
        
    }
}
