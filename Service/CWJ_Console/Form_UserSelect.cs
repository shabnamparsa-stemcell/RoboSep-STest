using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Xml;
using System.Text;
using System.Windows.Forms;

namespace GUI_Console
{
    public partial class Form_UserSelect : Form
    {
        Point Offset;
        Control cntrlTextbox;
        List<string> slistUserNames;

        public Form_UserSelect(Control txtBox, List<string> names)
        {
            InitializeComponent();

            // LOG
            string logMSG = "Creating new User Select window";
            GUI_Controls.uiLog.LOG(this, "Form_UserSelect", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

            cntrlTextbox = txtBox;
            slistUserNames = names;
        }

        // off colour every 2nd item in listview
        private void highlightOdd()
        {
            for (int i = 0; i < listView_users.Items.Count; i++)
            {
                if ((i % 2) == 0)
                {
                    listView_users.Items[i].BackColor = Color.FromArgb(215, 209, 215);
                }
            }
        }

        // loads names from slistUserNames list to listview items
        private void loadUserList()
        {
            for (int i = 0; i < slistUserNames.Count; i++)
            {
                listView_users.Items.Add(slistUserNames[i]);
            }
        }

        private void Form_UserSelect_Load(object sender, EventArgs e)
        {
            // Load users into listview items
            loadUserList();

            // set form region to robo-panel region
            this.Region = roboPanel_Panel.Region;

            // resize column to fit
            Column1.Width = listView_users.Size.Width;
            if (slistUserNames.Count > 9)
            {
                int x = listView_users.Size.Width;
                listView_users.Size = new Size(x +12, listView_users.Size.Height);
                Column1.Width = x - 5;
            }

            // add forms to form tracker
            Offset = new Point((RoboSep_UserConsole.getInstance().Size.Width - this.Size.Width) / 2,
                (RoboSep_UserConsole.getInstance().Size.Height - this.Size.Height) / 2);

            this.Location = new Point(Offset.X + RoboSep_UserConsole.getInstance().Location.X,
                Offset.Y + RoboSep_UserConsole.getInstance().Location.Y);

            RoboSep_UserConsole.getInstance().addForm(this, Offset);

            // off colour every 2nd item in listview
            highlightOdd();

            // Change graphic for cancel button
            List<Image> ilist = new List<Image>();
            ilist.Add(GUI_Controls.Properties.Resources.Button_QUADRANTCANCEL);
            ilist.Add(GUI_Controls.Properties.Resources.Button_QUADRANTCANCEL);
            ilist.Add(GUI_Controls.Properties.Resources.Button_QUADRANTCANCEL);
            ilist.Add(GUI_Controls.Properties.Resources.Button_QUADRANTCANCEL);
            button_Cancel1.ChangeGraphics(ilist);
            button_Cancel1.BringToFront();
        }

        private void button_Cancel1_Click(object sender, EventArgs e)
        {
            RoboSep_UserConsole.getInstance().frmOverlay.Hide();
            this.Close();
            //RoboSep_UserConsole.UserConsole.removeForm(overlay);
            //RoboSep_UserConsole.UserConsole.removeForm(this);
            RoboSep_UserConsole.getInstance().Activate();

            // LOG
            string logMSG = "User Select window closed without selection";
            GUI_Controls.uiLog.LOG(this, "button_Cancel1_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        private void Form_UserSelect_Deactivate(object sender, EventArgs e)
        {
            RoboSep_UserConsole.getInstance().frmOverlay.BringToFront();
            this.BringToFront();
            this.Activate();
        }

        private void listView_users_Click(object sender, EventArgs e)
        {
            // LOG
            string logMSG = "User " + listView_users.SelectedItems[0].Text + " Selected";
            GUI_Controls.uiLog.LOG(this, "listView_users_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

            RoboSep_UserConsole.strCurrentUser = listView_users.SelectedItems[0].Text;
            cntrlTextbox.Text = listView_users.SelectedItems[0].Text;
            //RoboSep_UserConsole.UserConsole.removeForm(overlay);
            //RoboSep_UserConsole.UserConsole.removeForm(this);
            RoboSep_UserConsole.getInstance().frmOverlay.Hide();
            this.Close();
        }

        private void Form_UserSelect_LocationChanged(object sender, EventArgs e)
        {
            RoboSep_UserConsole.getInstance().frmOverlay.BringToFront();
            this.BringToFront();
        }
    }
}
