using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Tesla.Common.ResourceManagement;

namespace Tesla.ProtocolEditor
{
    public partial class ListSelect : Form
    {
        private List<string> items;
        public ListSelect(List<string> myItems,string title, string okButtonCaption)
        {
            InitializeComponent();
            items = myItems;

            Text = title;
            btnNew.Text = okButtonCaption;
        }


        private void ProtocolTypeSelect_Load(object sender, EventArgs e)
        {
            if (items.Count <= 0) return;
            foreach (string s in items)
            {
               cmbRobosepType.Items.Add(s);
            }
            cmbRobosepType.SelectedIndex = 0;

        }

        public int GetSelectedIndex()
        {
            return cmbRobosepType.SelectedIndex;
        }
    }
}
