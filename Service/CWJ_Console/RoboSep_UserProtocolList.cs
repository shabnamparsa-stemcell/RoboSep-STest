using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;


namespace GUI_Console
{
    public partial class RoboSep_UserProtocolList : UserControl
    {
        private RoboSep_ProtocolList myProtocolList;
        private List<RoboSep_Protocol> myUserProtocols;
        private static RoboSep_UserProtocolList myUserProtocolList;
        

        private RoboSep_UserProtocolList()
        {
            InitializeComponent();
            myUserProtocols = new List<RoboSep_Protocol>();

            // Change button graphics
            List<Image> ilist = new List<Image>();
            // Protocol List
            ilist.Add(GUI_Controls.Properties.Resources.Button_PROTOCOLLIST0);
            ilist.Add(GUI_Controls.Properties.Resources.Button_PROTOCOLLIST1);
            ilist.Add(GUI_Controls.Properties.Resources.Button_PROTOCOLLIST2);
            ilist.Add(GUI_Controls.Properties.Resources.Button_PROTOCOLLIST3);
            button_ProtocolList.ChangeGraphics(ilist);
            // Remove Button
            ilist.Clear();
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_REMOVEPROTOCOL0);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_REMOVEPROTOCOL1);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_REMOVEPROTOCOL2);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_REMOVEPROTOCOL3);
            button_Remove.ChangeGraphics(ilist);
            // save list button
            ilist.Clear();
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_SAVELIST0);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_SAVELIST1);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_SAVELIST2);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_SAVELIST3);
            button_SaveList.ChangeGraphics(ilist);

            // LOG
            string logMSG = "Generating New UserProtocol user control";
            GUI_Controls.uiLog.LOG(this, "RoboSep_UserProtocolList", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        public static RoboSep_UserProtocolList getInstance()
        {
            if (myUserProtocolList == null)
                myUserProtocolList = new RoboSep_UserProtocolList();
            return myUserProtocolList;
        }

        private void RoboSep_UserProtocolList_Load(object sender, EventArgs e)
        {
            // set my protocolList to protocol list created by protocols window
            myProtocolList = RoboSep_ProtocolList.getInstance();
            
            // load user name from UserConsole window
            label_userName.Text = RoboSep_UserConsole.strCurrentUser;
           
        }

        public List<RoboSep_Protocol> usrProtocols
        {
            get
            {
                return myUserProtocols;
            }
        }

        public void addProtocols(List<RoboSep_Protocol> rpNew)
        {
            // check if protocol already exists in list
            for (int j = 0; j < rpNew.Count; j++)
            {
                bool alreadyAdded = false;
                for (int i = 0; i < myUserProtocols.Count; i++)
                {
                    if (rpNew[j] == myUserProtocols[i])
                        alreadyAdded = true;
                }
                if (!alreadyAdded)
                {
                    // add if it doesnt already exist
                    myUserProtocols.Add(rpNew[j]);

                    // LOG
                    string logMSG = "Adding protocol " + rpNew[j].Protocol_Name + " to user list";
                    GUI_Controls.uiLog.LOG(this, "addProtocols", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
                }
            }
            // refresh list view to include new protocols
            refreshList();
        }

        private void refreshList()
        {
            // add items to list view
            lvUserProtocols.SuspendLayout();
            lvUserProtocols.Items.Clear();
            List<string> UsrProtocolNames = new List<string>();
            for (int i = 0; i < myUserProtocols.Count; i++)
            {
                UsrProtocolNames.Add(myUserProtocols[i].Protocol_Name);
            }
            UsrProtocolNames.Sort();
            for (int i = 0; i < myUserProtocols.Count; i++)
            {
                //lvUserProtocols.Items.Add(myUserProtocols[i].Protocol_Name);
                lvUserProtocols.Items.Add(UsrProtocolNames[i]);
            }
            if (lvUserProtocols.Items.Count > 7)
                columnHeader1.Width = lvUserProtocols.Size.Width - 18;
            highlight();
            lvUserProtocols.ResumeLayout();
        }

        private void highlight()
        {
            Color bg_offcolour = Color.FromArgb(215, 209, 215);
            Color bg_colour = Color.White;
            for (int i = 0; i < lvUserProtocols.Items.Count; i++)
            {
                if ((i % 2) == 1)
                {
                    lvUserProtocols.Items[i].BackColor = bg_offcolour;
                }
                else
                    lvUserProtocols.Items[i].BackColor = lvUserProtocols.BackColor;
            }
        }

        public void saveUserList()
        {
            // create array of protocols
            string[] protocolList = new string[myUserProtocols.Count];
            for (int i = 0; i < myUserProtocols.Count; i++)
            {
                protocolList[i] = myUserProtocols[i].Protocol_FileName;
            }
            RoboSep_UserDB.getInstance().saveUserProtocols(RoboSep_UserConsole.strCurrentUser, protocolList);            
        }

        public void resetForm()
        {
            // LOG
            string logMSG = "Reset user protocol list";
            GUI_Controls.uiLog.LOG(this, "resetForm", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

            // clear listview
            lvUserProtocols.Items.Clear();

            // clear protocols
            myUserProtocols.Clear();

            // change user name to current user
            label_userName.Text = RoboSep_UserConsole.strCurrentUser;
        }

        #region Form Events
        private void button_Cancel_Click(object sender, EventArgs e)
        {
            // close 
            RoboSep_UserConsole.getInstance().Controls.Remove(this);
            RoboSep_UserConsole.getInstance().Controls.Add(RoboSep_Protocols.getInstance());

            // LOG
            string logMSG = "Cancel button clicked";
            GUI_Controls.uiLog.LOG(this, "button_Cancel_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        private void button_AllProtocols_Click(object sender, EventArgs e)
        {
            // switch to all protocols control
            RoboSep_UserConsole.getInstance().Controls.Remove(this);
            RoboSep_UserConsole.getInstance().Controls.Add(RoboSep_ProtocolList.getInstance());

            // LOG
            string logMSG = "All Protocols button clicked";
            GUI_Controls.uiLog.LOG(this, "button_AllProtocols_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        private void button_SaveList_Click(object sender, EventArgs e)
        {
            // save list
            saveUserList();

            // close protocol list and user protocol list
            RoboSep_UserConsole.getInstance().Controls.Remove(this);
            RoboSep_UserConsole.getInstance().Controls.Add(RoboSep_Protocols.getInstance());

            // Update user list
            RoboSep_Protocols.getInstance().LoadUsers();

            // LOG
            string logMSG = "Save List button clicked";
            GUI_Controls.uiLog.LOG(this, "button_SaveList_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        private void button_Remove_Click(object sender, EventArgs e)
        {
            // remove selected item from userProtoclList
            for (int i = 0; i < lvUserProtocols.SelectedItems.Count; i++)
            {
                for (int j = 0; j < myUserProtocols.Count; j++)
                {
                    if (lvUserProtocols.SelectedItems[i].Text == myUserProtocols[j].Protocol_Name)
                    {
                        // LOG
                        string logMSG = "Removing protocol '" + myUserProtocols[j].Protocol_Name + "' from user list";
                        GUI_Controls.uiLog.LOG(this, "button_Remove_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
                        
                        myUserProtocols.RemoveAt(j);
                        break;
                    }
                }
            }
            refreshList();
            RoboSep_ProtocolList.getInstance().refreshList();
        }

    }
        #endregion
}
