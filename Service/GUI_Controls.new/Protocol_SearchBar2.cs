using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace GUI_Controls
{
    public enum ButtonDisplayMode
    {
        eDisplayModeFilter,
        eDisplayModeScroll
    }

    public partial class Protocol_SearchBar2 : UserControl
    {
        public ButtonDisplayMode BtnDisplayMode = ButtonDisplayMode.eDisplayModeScroll;

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

        bool FilterHumanCheck = false;
        bool FilterMouseCheck = false;
        bool FilterPositiveCheck = false;
        bool FilterNegativeCheck = false;


        public Protocol_SearchBar2()
        {
            InitializeComponent();
                    
            // Change button images
            // Human
            iListHuman = new List<Image>();
            iListHuman.Add(Properties.Resources.human_STD_65x65);
            iListHuman.Add(Properties.Resources.human_STD_65x65);
            iListHuman.Add(Properties.Resources.human_CLICK_65x65);
            iListHuman.Add(Properties.Resources.human_STD_65x65);
     
            // Mouse
            iListMouse = new List<Image>();
            iListMouse.Add(Properties.Resources.mouse_STD_65x65);
            iListMouse.Add(Properties.Resources.mouse_STD_65x65);
            iListMouse.Add(Properties.Resources.mouse_CLICK_65x65);
            iListMouse.Add(Properties.Resources.mouse_STD_65x65);

            // Positive
            iListPos = new List<Image>();
            iListPos.Add(Properties.Resources.Pos_STD_65x65);
            iListPos.Add(Properties.Resources.Pos_STD_65x65);
            iListPos.Add(Properties.Resources.Pos_CLICK_65x65);
            iListPos.Add(Properties.Resources.Pos_STD_65x65);

            // Negative
            iListNeg = new List<Image>();
            iListNeg.Add(Properties.Resources.Neg_STD_65x65);
            iListNeg.Add(Properties.Resources.Neg_STD_65x65);
            iListNeg.Add(Properties.Resources.Neg_CLICK_65x65);
            iListNeg.Add(Properties.Resources.Neg_STD_65x65);


            // Filter
            iListModeFilter = new List<Image>();
            iListModeFilter.Add(Properties.Resources.Contract_STD_65x65);
            iListModeFilter.Add(Properties.Resources.Contract_OVER_65x65);
            iListModeFilter.Add(Properties.Resources.Contract_CLICK_65x65);
            iListModeFilter.Add(Properties.Resources.Contract_OVER_65x65);

            // Up
            iListUp = new List<Image>();
            iListUp.Add(Properties.Resources.NavigateUp_STD_65x65);
            iListUp.Add(Properties.Resources.NavigateUp_STD_65x65);
            iListUp.Add(Properties.Resources.NavigateUp_CLICK_65x65);
            iListUp.Add(Properties.Resources.NavigateUp_STD_65x65);

            // Down
            iListDown = new List<Image>();
            iListDown.Add(Properties.Resources.NavigateDown_STD_65x65);
            iListDown.Add(Properties.Resources.NavigateDown_STD_65x65);
            iListDown.Add(Properties.Resources.NavigateDown_CLICK_65x65);
            iListDown.Add(Properties.Resources.NavigateDown_STD_65x65);

            // Left
            iListLeft = new List<Image>();
            iListLeft.Add(Properties.Resources.NavigateLeft_STD_65x65);
            iListLeft.Add(Properties.Resources.NavigateLeft_STD_65x65);
            iListLeft.Add(Properties.Resources.NavigateLeft_CLICK_65x65);
            iListLeft.Add(Properties.Resources.NavigateLeft_STD_65x65);
 
            // Right
            iListRight = new List<Image>();
            iListRight.Add(Properties.Resources.NavigateRight_STD_65x65);
            iListRight.Add(Properties.Resources.NavigateRight_STD_65x65);
            iListRight.Add(Properties.Resources.NavigateRight_CLICK_65x65);
            iListRight.Add(Properties.Resources.NavigateRight_STD_65x65);

            // Mode
            iListModeScroll = new List<Image>();
            iListModeScroll.Add(Properties.Resources.Expand_STD_65x65);
            iListModeScroll.Add(Properties.Resources.Expand_OVER_65x65);
            iListModeScroll.Add(Properties.Resources.Expand_CLICK_65x65);
            iListModeScroll.Add(Properties.Resources.Expand_OVER_65x65);

            ChangeButtonGraphicsScrolling();
            ResumeLayout();
        }

        private void ChangeButtonGraphicsScrolling()
        {
            button_1.ChangeGraphics(iListUp);
            button_2.ChangeGraphics(iListDown);
            button_3.ChangeGraphics(iListLeft);
            button_4.ChangeGraphics(iListRight);
            button_mode.ChangeGraphics(iListModeScroll);
            button_1.disableImage = Properties.Resources.NavigateUp_DISABLE_65x65;
            button_2.disableImage = Properties.Resources.NavigateDown_DISABLE_65x65;
            button_1.setToggle(false);
            button_2.setToggle(false);
            button_3.setToggle(false);
            button_4.setToggle(false);
        }

        private void ChangeButtonGraphicsFiltering()
        {
            button_1.ChangeGraphics(iListHuman);
            button_2.ChangeGraphics(iListMouse);
            button_3.ChangeGraphics(iListPos);
            button_4.ChangeGraphics(iListNeg);
            button_mode.ChangeGraphics(iListModeFilter);
            button_1.disableImage = null;
            button_2.disableImage = null;
            button_1.setToggle(true);
            button_2.setToggle(true);
            button_3.setToggle(true);
            button_4.setToggle(true);

            button_1.Checked = false;
            button_2.Checked = false;
            button_3.Checked = false;
            button_4.Checked = false;

            button_1.Check = FilterHumanCheck;
            button_2.Check = FilterMouseCheck;
            button_3.Check = FilterPositiveCheck;
            button_4.Check = FilterNegativeCheck;

        } 
  
        private void button_mode_Click(object sender, EventArgs e)
        {
            if (BtnDisplayMode == ButtonDisplayMode.eDisplayModeScroll)
            {
                ChangeButtonGraphicsFiltering();
                BtnDisplayMode = ButtonDisplayMode.eDisplayModeFilter;
            }
            else
            {
                ChangeButtonGraphicsScrolling();
                BtnDisplayMode = ButtonDisplayMode.eDisplayModeScroll;
            }
            button_mode.Check = false;
            this.Invalidate();
        }

        private void button_1_Click(object sender, EventArgs e)
        {
            if (BtnDisplayMode == ButtonDisplayMode.eDisplayModeFilter)
            {
                if (button_2.Check)
                {
                    // turn mouse filter off
                    button_2.Check = false;
                }
                FilterHumanCheck = button_1.Check;
                FilterMouseCheck = button_2.Check;
            }
        }

        private void button_2_Click(object sender, EventArgs e)
        {
            if (BtnDisplayMode == ButtonDisplayMode.eDisplayModeFilter)
            {
                if (button_1.Check)
                {
                    // turn human filter off
                    button_1.Check = false;
                }
                FilterHumanCheck = button_1.Check;
                FilterMouseCheck = button_2.Check;
            }
        }

        private void button_3_Click(object sender, EventArgs e)
        {
            if (BtnDisplayMode == ButtonDisplayMode.eDisplayModeFilter)
            {
                if (button_4.Check)
                {
                    // turn negative filter off
                    button_4.Check = false;
                }
                FilterPositiveCheck = button_3.Check;
                FilterNegativeCheck = button_4.Check;
            }
        }

        private void button_4_Click(object sender, EventArgs e)
        {
            if (BtnDisplayMode == ButtonDisplayMode.eDisplayModeFilter)
            {
                if (button_3.Check)
                {
                    // turn positive filter off
                    button_3.Check = false;
                }
                FilterPositiveCheck = button_3.Check;
                FilterNegativeCheck = button_4.Check;
            }
        }

        private void Protocol_SearchBar_Load(object sender, EventArgs e)
        {

            
        }

        public void getFilters(out string search, out bool Hum, out bool Mou,
            out bool Pos, out bool Neg)
        {
            search = textBox_search.Text;
            Hum = FilterHumanCheck;
            Mou = FilterMouseCheck;
            Pos = FilterPositiveCheck;
            Neg = FilterNegativeCheck;
        }
    }
}
