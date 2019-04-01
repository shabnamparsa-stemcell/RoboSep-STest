using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GUI_Console
{
    public partial class RoboSep_ScanSelection : Form
    {
        // Variable declaration
        string searchString;

        List<RoboSep_Protocol> userList;
        List<RoboSep_Protocol> allList;
        List<RoboSep_Protocol> filterList;
        RoboSep_Protocol SelectedProtocol;
        
        List<Image> IList = new List<Image>();

        public RoboSep_ScanSelection()
        {
            InitializeComponent();

            IList.Add(GUI_Controls.Properties.Resources.BUTTON_HOMECANCEL0);
            IList.Add(GUI_Controls.Properties.Resources.BUTTON_HOMECANCEL0);
            IList.Add(GUI_Controls.Properties.Resources.BUTTON_HOMECANCEL0);
            IList.Add(GUI_Controls.Properties.Resources.BUTTON_HOMECANCEL1);
            button_Cancel.ChangeGraphics(IList);

            userList = RoboSep_UserDB.getInstance().loadUserProtocols(RoboSep_UserConsole.strCurrentUser);
            allList = RoboSep_UserDB.getInstance().getAllProtocols();
            filterList = new List<RoboSep_Protocol>();
        }

        private void RoboSep_ScanSelection_Load(object sender, EventArgs e)
        {
            updateBarcodeMatches();
        }

        private void updateBarcodeMatches()
        {
            // clear filtered list
            filterList.Clear();
            for (int protocolNumber = 0; protocolNumber < allList.Count; protocolNumber++)
            {
                int containsText = searchForTextInString(searchString, allList[protocolNumber].Protocol_Name);
                bool BarcodeMatch = containsText > -1;
                if (BarcodeMatch)
                {
                    filterList.Add(allList[protocolNumber]);
                }
            }

            // clear displayed items
            listView_Protocols.Items.Clear();
            for (int i = 0; i < filterList.Count; i++)
            {
                listView_Protocols.Items.Add(filterList[i].Protocol_Name);
            }
        }

        private int searchForTextInString(string search, string theString)
        {
            for (int i = 0; i < theString.Length; i++)
            {
                if (theString[i].ToString().ToUpper() == search[0].ToString().ToUpper())
                {
                    string temp = string.Empty;
                    for (int j = 0; j < search.Length; j++)
                    {
                        if ((i + j) < theString.Length)
                            temp += theString[i + j];
                    }
                    if (search.ToUpper() == temp.ToUpper())
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        private void textBox_BarCode_TextChanged(object sender, EventArgs e)
        {
            if (textBox_BarCode.Text.Length > 4)
            {
                string BCode = textBox_BarCode.Text.Substring(0, 5);
                if (BCode != searchString)
                {
                    searchString = BCode;
                    updateBarcodeMatches();
                }
            }
        }

        private void button_Keyboard1_Click(object sender, EventArgs e)
        {
            // set selection from listview to selected protocol
            SelectProtocol();

            // complete selection..
        }

        private void listView_Protocols_DoubleClick(object sender, EventArgs e)
        {
            // set selection from listview to selected protocol
            SelectProtocol();

            // complete selection..
        }

        private void SelectProtocol()
        {
            if (listView_Protocols.SelectedItems.Count > 0)
            {
                // match selected protocol to text from listview item
                for (int i = 0; i < filterList.Count; i++)
                {
                    if (listView_Protocols.SelectedItems[0].Text == filterList[i].Protocol_Name)
                        SelectedProtocol = filterList[i];
                }

                //  do something with it!!!!!
                //
                //
                //
            }
        }

    }
}
