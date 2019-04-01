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
    public partial class Tabs : UserControl
    {
        private Image[,] BackImage = new Image[3, 3];
        public int CurrentTab;
        private const int TAB1 = 0;
        private const int TAB2 = 1;
        private const int TAB3 = 2;
        
        public Tabs()
        {
            InitializeComponent();
            CurrentTab = TAB1;
        }

        public Tabs(int setTab)
        {
            InitializeComponent();
            CurrentTab = setTab;

        }

        public void ChangeGraphics(List<Image> AlternateGraphics)
        {
            for (int i = 0; i < 3; i++)
            {
                
            }
        }

        private void SideTabs_Load(object sender, EventArgs e)
        {
            this.SuspendLayout();
            if (this.Name == "HelpTabs")
            {
                BackImage[TAB1, 0] = Properties.Resources.TAB_ROBOSEP1;
                BackImage[TAB1, 1] = Properties.Resources.TAB_ROBOSEP2;
                BackImage[TAB1, 2] = Properties.Resources.TAB_ROBOSEP1;
                // tab 2
                BackImage[TAB2, 0] = Properties.Resources.TAB_HELPVIDEO1;
                BackImage[TAB2, 1] = Properties.Resources.TAB_HELPVIDEO2;
                BackImage[TAB2, 2] = Properties.Resources.TAB_HELPVIDEO2;

                Tab3.Enabled = false;
                Tab3.Visible = false;
            }
            else
            {
                // laod graphics for tab windows
                // tab 1
                BackImage[TAB1,0] = Properties.Resources.TAB_RUNSTAT1;
                BackImage[TAB1,1] = Properties.Resources.TAB_RUNSTAT2;
                BackImage[TAB1,2] = Properties.Resources.TAB_RUNSTAT3;
                // tab 2
                BackImage[TAB2, 0] = Properties.Resources.TAB_VIDLOG1;
                BackImage[TAB2, 1] = Properties.Resources.TAB_VIDLOG2;
                BackImage[TAB2, 2] = Properties.Resources.TAB_VIDLOG3;
                // Tab 3
                BackImage[TAB3, 0] = Properties.Resources.TAB_REPORTS1;
                BackImage[TAB3, 1] = Properties.Resources.TAB_REPORTS2;
                BackImage[TAB3, 2] = Properties.Resources.TAB_REPORTS3;
            }
            this.BackgroundImage = BackImage[CurrentTab, CurrentTab];
            this.ResumeLayout();
        }

        private void Tab1_MouseEnter(object sender, EventArgs e)
        {
            this.BackgroundImage = BackImage[CurrentTab,TAB1];
        }

        private void Tab1_MouseLeave(object sender, EventArgs e)
        {
            if (CurrentTab == TAB1)
            { // DO NOTHING
            }
            else
            {
                this.BackgroundImage = BackImage[CurrentTab,CurrentTab];
            }
        }

        private void Tab1_Click(object sender, EventArgs e)
        {
            if (CurrentTab == TAB1)
            {
                // do nothing
            }
            else
            {
                CurrentTab = TAB1;
                this.BackgroundImage = BackImage[CurrentTab,CurrentTab];
                // load page to go with this tab...
            }
        }

        private void Tab2_MouseEnter(object sender, EventArgs e)
        {
            this.BackgroundImage = BackImage[CurrentTab,TAB2];
            
        }

        private void Tab2_MouseLeave(object sender, EventArgs e)
        {
            if (CurrentTab == TAB2)
            {// do nothing
            }
            else
            {
                this.BackgroundImage = BackImage[CurrentTab,CurrentTab];
            }
        }

        private void Tab2_Click(object sender, EventArgs e)
        {
            if (CurrentTab == TAB2)
            {
                // do nothing
            }
            else
            {
                CurrentTab = TAB2;
                this.BackgroundImage = BackImage[CurrentTab,CurrentTab];
                // load page to go with this tab...
            }
        }

        private void Tab3_MouseEnter(object sender, EventArgs e)
        {
            this.BackgroundImage = BackImage[CurrentTab,TAB3];
        }

        private void Tab3_MouseLeave(object sender, EventArgs e)
        {
            if (CurrentTab == TAB3)
            { // Do Nothing
            }
            else
            {
                this.BackgroundImage = BackImage[CurrentTab,CurrentTab];
            }
        }

        private void Tab3_Click(object sender, EventArgs e)
        {
            if (CurrentTab == TAB3)
            {
                // do nothing
            }
            else
            {
                CurrentTab = TAB3;
                this.BackgroundImage = BackImage[CurrentTab,CurrentTab];
                // load page to go with this tab...
            }
        }

    }
}
