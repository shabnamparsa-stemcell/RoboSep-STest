using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;



namespace GUI_Controls
{
    public partial class Protocol_SearchBar3 : UserControl
    {
        private string DefaultSearchtext = "Search";
        List<Image> iListSortAscend;
        List<Image> iListSortDescend;
        string textSearchBox;


        public Protocol_SearchBar3()
        {
            InitializeComponent();

            // Change button images
            iListSortAscend = new List<Image>();
            iListSortDescend = new List<Image>();

            // Sorting
            iListSortDescend.Add(Properties.Resources.GE_BTN12M_sort_descend_STD);
            iListSortDescend.Add(Properties.Resources.GE_BTN12M_sort_descend_OVER);
            iListSortDescend.Add(Properties.Resources.GE_BTN12M_sort_descend_OVER);
            iListSortDescend.Add(Properties.Resources.GE_BTN12M_sort_descend_CLICK);

            iListSortAscend.Add(Properties.Resources.GE_BTN11M_sort_ascend_STD);
            iListSortAscend.Add(Properties.Resources.GE_BTN11M_sort_ascend_OVER);
            iListSortAscend.Add(Properties.Resources.GE_BTN11M_sort_ascend_OVER);
            iListSortAscend.Add(Properties.Resources.GE_BTN11M_sort_ascend_CLICK);
            btn_Sorting.ChangeGraphics(iListSortAscend);

            List<Image> iList;
            iList = new List<Image>();

            // Human
            iList.Add(Properties.Resources.PR_BTN01M_human_STD);
            iList.Add(Properties.Resources.PR_BTN01M_human_OVER);
            iList.Add(Properties.Resources.PR_BTN01M_human_CLICK);
            iList.Add(Properties.Resources.PR_BTN01M_human_OVER);
            btn_FilterHuman.ChangeGraphics(iList);

            // Mouse
            iList.Clear();
            iList = new List<Image>();
            iList.Add(Properties.Resources.PR_BTN02M_mouse_STD);
            iList.Add(Properties.Resources.PR_BTN02M_mouse_OVER);
            iList.Add(Properties.Resources.PR_BTN02M_mouse_CLICK);
            iList.Add(Properties.Resources.PR_BTN02M_mouse_OVER);
            btn_FilterMouse.ChangeGraphics(iList);

            // Positive
            iList.Clear();
            iList = new List<Image>();
            iList.Add(Properties.Resources.PR_BTN03M_positive_STD);
            iList.Add(Properties.Resources.PR_BTN03M_positive_OVER);
            iList.Add(Properties.Resources.PR_BTN03M_positive_CLICK);
            iList.Add(Properties.Resources.PR_BTN03M_positive_OVER);
            btn_FilterPos.ChangeGraphics(iList);

            // Negative
            iList.Clear();
            iList = new List<Image>();
            iList.Add(Properties.Resources.PR_BTN04M_negative_STD);
            iList.Add(Properties.Resources.PR_BTN04M_negative_OVER);
            iList.Add(Properties.Resources.PR_BTN04M_negative_CLICK);
            iList.Add(Properties.Resources.PR_BTN04M_negative_OVER);
            btn_FilterNeg.ChangeGraphics(iList);

            // Up
            iList.Clear();
            iList = new List<Image>();
            iList.Add(Properties.Resources.GE_BTN03M_up_arrow_STD);
            iList.Add(Properties.Resources.GE_BTN03M_up_arrow_OVER);
            iList.Add(Properties.Resources.GE_BTN03M_up_arrow_OVER);
            iList.Add(Properties.Resources.GE_BTN03M_up_arrow_CLICK);
            btn_ScrollUp.ChangeGraphics(iList);

            // Down
            iList.Clear();
            iList.Add(Properties.Resources.GE_BTN04M_down_arrow_STD);
            iList.Add(Properties.Resources.GE_BTN04M_down_arrow_OVER);
            iList.Add(Properties.Resources.GE_BTN04M_down_arrow_OVER);
            iList.Add(Properties.Resources.GE_BTN04M_down_arrow_CLICK);
            btn_ScrollDown.ChangeGraphics(iList);

            // Left
            iList.Clear();
            iList.Add(Properties.Resources.GE_BTN05M_left_arrow_STD);
            iList.Add(Properties.Resources.GE_BTN05M_left_arrow_OVER);
            iList.Add(Properties.Resources.GE_BTN05M_left_arrow_OVER);
            iList.Add(Properties.Resources.GE_BTN05M_left_arrow_CLICK);
            btn_ScrollLeft.ChangeGraphics(iList);

            // Right
            iList.Clear();
            iList.Add(Properties.Resources.GE_BTN06M_right_arrow_STD);
            iList.Add(Properties.Resources.GE_BTN06M_right_arrow_OVER);
            iList.Add(Properties.Resources.GE_BTN06M_right_arrow_OVER);
            iList.Add(Properties.Resources.GE_BTN06M_right_arrow_CLICK);
            btn_ScrollRight.ChangeGraphics(iList);

            textSearchBox = DefaultSearchtext;
            textBox_search.Text = textSearchBox;
            ResumeLayout();
        }

        private void Protocol_SearchBar3_Load(object sender, EventArgs e)
        {
            btn_FilterHuman.setToggle(true);
            btn_FilterMouse.setToggle(true);
            btn_FilterPos.setToggle(true);
            btn_FilterNeg.setToggle(true);

            btn_FilterHuman.Checked = false;
            btn_FilterMouse.Checked = false;
            btn_FilterPos.Checked = false;
            btn_FilterNeg.Checked = false;

            btn_ScrollUp.disableImage = Properties.Resources.NavigateUp_DISABLE;
            btn_ScrollDown.disableImage = Properties.Resources.NavigateDown_DISABLE;

            btn_ScrollUp.setToggle(false);
            btn_ScrollDown.setToggle(false);
            btn_ScrollLeft.setToggle(false);
            btn_ScrollRight.setToggle(false);

        }

        public string DefaultSearchText
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                DefaultSearchtext = value;
                textBox_search.Text = DefaultSearchtext;
            }
        }

        public string  SearchBoxText
        {
            set 
            {
                textSearchBox = value;
                textBox_search.Text = textSearchBox;
            }
        }

        private void btn_FilterHuman_Click(object sender, EventArgs e)
        {
            if (btn_FilterMouse.Check)
            {
                // turn mouse filter off
                btn_FilterMouse.Check = false;
            }
        }

        private void btn_FilterMouse_Click(object sender, EventArgs e)
        {
            if (btn_FilterHuman.Check)
            {
                // turn human filter off
                btn_FilterHuman.Check = false;
            }
        }

        private void btn_FilterPos_Click(object sender, EventArgs e)
        {
            if (btn_FilterNeg.Check)
            {
                // turn negative filter off
                btn_FilterNeg.Check = false;
            }
        }

        private void btn_FilterNeg_Click(object sender, EventArgs e)
        {
            if (btn_FilterPos.Check)
            {
                // turn positive filter off
                btn_FilterPos.Check = false;
            }
        }

        public void getFilters(out string search, out bool Hum, out bool Mou,
            out bool Pos, out bool Neg)
        {
            string sSearch = textBox_search.Text;
            sSearch.Trim();
            if (sSearch == textSearchBox)
            {
                sSearch = "";
            }

            search = sSearch;
            Hum = btn_FilterHuman.Check;
            Mou = btn_FilterMouse.Check;
            Pos = btn_FilterPos.Check;
            Neg = btn_FilterNeg.Check;
        }

        public void clearFilters()
        {
            textBox_search.Text = DefaultSearchtext;
            btn_FilterHuman.Check = false;
            btn_FilterMouse.Check = false;
            btn_FilterPos.Check = false;
            btn_FilterNeg.Check = false;
        }
 
        public void changeSortingButtonGraphicsAscending()
        {
            btn_Sorting.ChangeGraphics(iListSortAscend);
        }
        public void changeSortingButtonGraphicsDescending()
        {
            btn_Sorting.ChangeGraphics(iListSortDescend);
        }

    }

}
