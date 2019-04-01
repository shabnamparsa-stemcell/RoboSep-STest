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
        private string DefaultSearchtext = "Search";
        string textSearchBox;


        public Protocol_SearchBar2()
        {
            InitializeComponent();

            // Change button images
            // Up
            List<Image>  iList = new List<Image>();
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

        private void Protocol_SearchBar2_Load(object sender, EventArgs e)
        {
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

        public void getFilters(out string search)
        {
            string sSearch = textBox_search.Text;
            sSearch.Trim();
            if (sSearch == textSearchBox)
            {
                sSearch = "";
            }

            search = sSearch;
         }

        public void clearFilters()
        {
            textBox_search.Text = DefaultSearchtext;
        }
    }
}
