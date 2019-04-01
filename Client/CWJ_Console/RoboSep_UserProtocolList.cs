using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using GUI_Controls;
using Invetech.ApplicationLog;
namespace GUI_Console
{
    public partial class RoboSep_UserProtocolList : UserControl
    {
        private RoboSep_ProtocolList myProtocolList;
        private List<RoboSep_Protocol> myUserProtocols;
        private static RoboSep_UserProtocolList myUserProtocolList;
        private GUI_Controls.GUIButton[] cancelButtons;
        private IniFile LanguageINI;
        public string strUserName;
        private const int ITEMS_ON_SCREEN = 7;
        private StringFormat theFormat;
        private Color BG_ColorEven;
        private Color BG_ColorOdd;
        private Color BG_Selected;
        private Color Txt_Color;
        private Image[] cncl;
        
        private RoboSep_UserProtocolList()
        {
            InitializeComponent();
            myUserProtocols = new List<RoboSep_Protocol>();

            cancelButtons = new GUI_Controls.GUIButton[ITEMS_ON_SCREEN] {
                cncl0, cncl1, cncl2, cncl3, cncl4, cncl5, cncl6};

            List<Image> ilist = new List<Image>();
            ilist.Add(Properties.Resources.BACK_STD);
            ilist.Add(Properties.Resources.BACK_OVER);
            ilist.Add(Properties.Resources.BACK_OVER);
            ilist.Add(Properties.Resources.Back_CLICK);
            button_Cancel.ChangeGraphics(ilist);

            cncl = new Image[2] { Properties.Resources.Cancel_STD, Properties.Resources.Cancel_STD1};

            // set up listview drag scroll properties (based on specific page properties)
            lvUserProtocols.LINE_SIZE = 9;
            lvUserProtocols.NUM_VISIBLE_ELEMENTS = 7;
            lvUserProtocols.VERTICAL_PAGE_SIZE = 4;

            // LOG
            string logMSG = "Generating New UserProtocol user control";
            //GUI_Controls.uiLog.LOG(this, "RoboSep_UserProtocolList", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG); 
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
            UserNameHeader.Text = strUserName;

            // set labels and button text based on language settings
            LanguageINI = RoboSep_UserConsole.getInstance().LanguageINI;
            //lblUsrProtocols.Text = LanguageINI.GetString("lblUserProtocols");
            TabLists.Tab1 = LanguageINI.GetString("lblEditList");
            TabLists.Tab2 = LanguageINI.GetString("lblMyList");
            button_Remove.Text = LanguageINI.GetString("lblRemoveProtocol");
            button_SaveList.Text = LanguageINI.GetString("lblSaveList");

            // set up for drawing
            theFormat = new StringFormat();
            theFormat.Alignment = StringAlignment.Near;
            theFormat.LineAlignment = StringAlignment.Center;
            BG_ColorEven = Color.FromArgb(216, 217, 218);
            BG_ColorOdd = Color.FromArgb(243, 243, 243);
            BG_Selected = Color.FromArgb(78, 38, 131);
            Txt_Color = Color.FromArgb(95, 96, 98);
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
                    //GUI_Controls.uiLog.LOG(this, "addProtocols", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
                    //  (logMSG);
                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG); 
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
            if (lvUserProtocols.Items.Count >= ITEMS_ON_SCREEN)
            {
                int columnWidth = lvUserProtocols.Size.Width - 18;
                if (columnHeader1.Width != columnWidth)
                {
                    columnHeader1.Width = columnWidth;
                    ScrollbarVisible = true;
                }
            }
            else
            {
                int columnWidth = lvUserProtocols.Size.Width;
                if (columnHeader1.Width != columnWidth)
                {
                    // scroll bar will not be visible
                    columnHeader1.Width = columnWidth;
                    ScrollbarVisible = false;
                }
            }
            highlight();
            /* // show cancel buttons
            for (int i = 0; i < ITEMS_ON_SCREEN; i++)
            {
                cancelButtons[i].Visible = false; // i < lvUserProtocols.Items.Count;
            }
            */
            lvUserProtocols.ResumeLayout();
            lvUserProtocols.Refresh();

            // make space for scrollbar if more than 8 items are on screen
            
        }

        private void highlight()
        {
            /*
            Color bg_offcolour = Color.FromArgb(215, 209, 215);
            Color bg_colour = Color.White;
            Color Selected_Colour = Color.FromArgb(78, 38, 131);
            for (int i = 0; i < lvUserProtocols.Items.Count; i++)
            {
                if ((i % 2) == 1)
                {
                    lvUserProtocols.Items[i].BackColor = bg_offcolour;
                }
                else
                    lvUserProtocols.Items[i].BackColor = lvUserProtocols.BackColor;
            }
            // hightlight selected
            for (int i = 0; i < lvUserProtocols.SelectedIndices.Count; i++)
            {
                int index = lvUserProtocols.SelectedIndices[i];
                lvUserProtocols.Items[index].BackColor = Selected_Colour;
            }
            */
        }

        public void saveUserList()
        {
            // create array of protocols
            string[] protocolList = new string[myUserProtocols.Count];
            for (int i = 0; i < myUserProtocols.Count; i++)
            {
                protocolList[i] = myUserProtocols[i].Protocol_FileName;
            }
            RoboSep_UserDB.getInstance().saveUserProtocols(/*RoboSep_UserConsole.strCurrentUser*/ strUserName, protocolList);            
        }

        public void resetForm()
        {
            // LOG
            string logMSG = "Reset user protocol list";
            //GUI_Controls.uiLog.LOG(this, "resetForm", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG); 
            // clear listview
            lvUserProtocols.Items.Clear();

            // clear protocols
            myUserProtocols.Clear();

            // change user name to current user
            //label_userName.Text = strUserName; // RoboSep_UserConsole.strCurrentUser;
            UserNameHeader.Text = strUserName;
        }

        #region Form Events
        private void button_Cancel_Click(object sender, EventArgs e)
        {
            // Check if User list contains atleast 1 protocol (otherwise don't exit screen)
            bool validUser = myUserProtocols.Count > 0;
            if (validUser)
            {

                // close 
                RoboSep_Protocols.getInstance().UserName = strUserName;
                RoboSep_UserConsole.getInstance().Controls.Add(RoboSep_Protocols.getInstance());
                RoboSep_UserConsole.getInstance().Controls.Remove(this);

                // LOG
                string logMSG = "Cancel button clicked";
                //  (logMSG);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG); 
            }
            else
            {
                GUI_Controls.RoboMessagePanel prompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING, 
                    LanguageINI.GetString("msgValidUser"), LanguageINI.GetString("headerValidUser"), LanguageINI.GetString("Ok"));
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                prompt.Dispose();
                RoboSep_UserConsole.hideOverlay();
            }
        }

        private void button_AllProtocols_Click(object sender, EventArgs e)
        {
            // switch to all protocols control
            RoboSep_UserConsole.getInstance().Controls.Remove(this);
            RoboSep_UserConsole.getInstance().Controls.Add(RoboSep_ProtocolList.getInstance());

            // LOG
            string logMSG = "All Protocols button clicked";
            //GUI_Controls.uiLog.LOG(this, "button_AllProtocols_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG); 
        }

        private void button_SaveList_Click(object sender, EventArgs e)
        {
            // Check if User list contains atleast 1 protocol (otherwise don't exit screen)
            bool validUser = myUserProtocols.Count > 0;
            if (validUser)
            {
                // save list
                saveUserList();

                // close protocol list and user protocol list
                RoboSep_Protocols.getInstance().UserName = strUserName;
                RoboSep_UserConsole.getInstance().Controls.Add(RoboSep_Protocols.getInstance());
                RoboSep_UserConsole.getInstance().Controls.Remove(this);

                // Update user list
                RoboSep_Protocols.getInstance().LoadUsers();

                // LOG
                string logMSG = "Save List button clicked";
                //GUI_Controls.uiLog.LOG(this, "button_SaveList_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
                //  (logMSG);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG); 
            }
            else
            {
                GUI_Controls.RoboMessagePanel prompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_WARNING,
                    LanguageINI.GetString("msgValidUser"), LanguageINI.GetString("headerValidUser"), LanguageINI.GetString("Ok"));
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                prompt.Dispose();
                RoboSep_UserConsole.hideOverlay();
            }
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
                        //GUI_Controls.uiLog.LOG(this, "button_Remove_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
                        //  (logMSG);
                        LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                         
                        myUserProtocols.RemoveAt(j);
                        break;
                    }
                }
            }
            refreshList();
            RoboSep_ProtocolList.getInstance().refreshList();
        }

        private void lvUserProtocols_KeyDown(object sender, KeyEventArgs e)
        {
            // check for modifier keys
            Keys CntrlKey = e.Modifiers;
            if (CntrlKey == Keys.Control)
            {
                // keyData for CTRL = 17, A = 65
                if ((Keys)e.KeyValue == Keys.A)
                {
                    for (int i = 0; i < lvUserProtocols.Items.Count; i++)
                    {
                        lvUserProtocols.Items[i].Selected = true;
                    }
                }
            }
        }

        private void cncl0_Click(object sender, EventArgs e)
        { removeProtocol(0); }
        private void cncl1_Click(object sender, EventArgs e)
        { removeProtocol(1); }
        private void cncl2_Click(object sender, EventArgs e)
        { removeProtocol(2); }
        private void cncl3_Click(object sender, EventArgs e)
        { removeProtocol(3); }
        private void cncl4_Click(object sender, EventArgs e)
        { removeProtocol(4); }
        private void cncl5_Click(object sender, EventArgs e)
        { removeProtocol(5); }
        private void cncl6_Click(object sender, EventArgs e)
        { removeProtocol(6); }
        private void cncl7_Click(object sender, EventArgs e)
        { removeProtocol(7); }

        private void removeProtocol(int visibleListRef)
        {
            int topItemIndex = FindTopListItemIndex();

            if (topItemIndex >= 0)
            {
                string removeItem = lvUserProtocols.Items[topItemIndex + visibleListRef].Text;
                // look for item in user list
                for (int i = 0; i < myUserProtocols.Count; i++)
                {
                    if (myUserProtocols[i].Protocol_Name == removeItem)
                    {
                        myUserProtocols.RemoveAt(i);
                        RoboSep_ProtocolList.getInstance().refreshList();
                        break;
                    }
                }
                lvUserProtocols.Items.RemoveAt(topItemIndex + visibleListRef);
                if (lvUserProtocols.Items.Count == ITEMS_ON_SCREEN-1)
                    refreshList();
            }
        }

        private int FindTopListItemIndex()
        {
            ListViewItem topItem = lvUserProtocols.TopItem;
            int topItemIndex = -1;
            // find item index
            for (int i = 0; i < lvUserProtocols.Items.Count; i++)
            {
                if (lvUserProtocols.Items[i].Text == topItem.Text)
                {
                    topItemIndex = i;
                    break;
                }
            }
            return topItemIndex;
        }

        private void Tab_help_Tab1_Click(object sender, EventArgs e)
        {
            RoboSep_UserConsole.getInstance().SuspendLayout();
            // switch to all protocols control
            RoboSep_UserConsole.getInstance().Controls.Add(RoboSep_ProtocolList.getInstance());
            RoboSep_UserConsole.getInstance().Controls.Remove(this);            
            RoboSep_UserConsole.getInstance().ResumeLayout();

            // LOG
            string logMSG = "All Protocols button clicked";
            //GUI_Controls.uiLog.LOG(this, "button_AllProtocols_Click", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG); 
        }

        private void Tab_help_Tab2_Click(object sender, EventArgs e)
        {
            // do nothing, this tab represents the current page
        }

        private void lvUserProtocols_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //
        // draw listview
        //

        private void lvUserProtocols_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            System.Drawing.Drawing2D.LinearGradientBrush GradientBrush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.Blue, Color.LightBlue, 270);
            SolidBrush theBrush1, theBrush2;
            theBrush1 = new SolidBrush(Color.White);
            theBrush2 = new SolidBrush(Color.Yellow);
            Pen thePen = new Pen(theBrush1, 2);
            Font thefont = new Font("Arial", 12);

            e.Graphics.FillRectangle(GradientBrush, e.Bounds);

            e.Graphics.DrawRectangle(thePen, e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2);

            e.Graphics.DrawString(e.Header.Text, thefont, theBrush2, e.Bounds);

            thefont.Dispose();
            thePen.Dispose();
            theBrush2.Dispose();
            theBrush1.Dispose();
            GradientBrush.Dispose();
        }

        private void lvUserProtocols_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            SolidBrush theBrush;
            if (e.Item.Selected)
            {
                theBrush = new SolidBrush(BG_Selected);
                e.Graphics.FillRectangle(theBrush, e.Bounds);
                theBrush.Dispose();
            }
            else if (e.ItemIndex % 2 == 0)
            {
                theBrush = new SolidBrush(BG_ColorEven);
                e.Graphics.FillRectangle(theBrush, e.Bounds);
                theBrush.Dispose();
            }
            else
            {
                theBrush = new SolidBrush(BG_ColorOdd);
                e.Graphics.FillRectangle(theBrush, e.Bounds);
                theBrush.Dispose();
            }

            /*  no longer uses picture box objects to cancel items
            if (e.ItemIndex >= lvUserProtocols.Items.Count - 1)
            {
                if (FindTopListItemIndex() == (lvUserProtocols.Items.Count - (ITEMS_ON_SCREEN -1) ))
                {
                    cancelButtons[ITEMS_ON_SCREEN - 1].Visible = false;
                }
            }
            else if (!cancelButtons[ITEMS_ON_SCREEN - 1].Visible && lvUserProtocols.Items.Count >= ITEMS_ON_SCREEN)
                cancelButtons[ITEMS_ON_SCREEN - 1].Visible = true;
            */
        }

        bool ScrollbarVisible = false;
        private void lvUserProtocols_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            
            // format text
            SolidBrush theBrush;
            Rectangle removeIcon = ScrollbarVisible ?
                new Rectangle(lvUserProtocols.Size.Width - 18 - cncl[0].Size.Width -5, e.Bounds.Y +6, cncl[0].Size.Width, cncl[0].Size.Height) :
                new Rectangle(lvUserProtocols.Size.Width - cncl[0].Size.Width - 5, e.Bounds.Y +6, cncl[0].Size.Width, cncl[0].Size.Height);
            Rectangle textBounds = new Rectangle(e.Bounds.X, e.Bounds.Y, columnHeader1.Width - cncl[0].Size.Width - 5, e.Bounds.Height);
            if (e.Item.Selected)
            {
                theBrush = new SolidBrush(Color.White);
                e.Graphics.DrawString(e.Item.Text, lvUserProtocols.Font, theBrush, textBounds, theFormat);
                e.Graphics.DrawImage(cncl[1], removeIcon);
                theBrush.Dispose();
            }
            else
            {
                theBrush = new SolidBrush(Txt_Color);
                e.Graphics.DrawString(e.Item.Text, lvUserProtocols.Font, theBrush, textBounds, theFormat);
                e.Graphics.DrawImage(cncl[0], removeIcon);
                theBrush.Dispose();
            }
        }

        private void lvUserProtocols_Click(object sender, EventArgs e)
        {
            // determine cancel click range
            int xMin = columnHeader1.Width + lvUserProtocols.Location.X - cncl[0].Size.Width - 5;
            int xMax = xMin + cncl[0].Size.Width - 5;

            // check if clicked within bounds of cancel button
            Point windowLocation = RoboSep_UserConsole.getInstance().Location;
            this.Cursor = new Cursor(Cursor.Current.Handle);
            //Cursor.Clip = new Rectangle(this.Location, this.Size);
            Point CursorLocation = Cursor.Position; // new Point(Cursor.Position.X /*- windowLocation.X*/, Cursor.Position.Y /*- windowLocation.Y*/ - lvUserProtocols.Location.Y);
            Point ClickLocation = new Point(CursorLocation.X - windowLocation.X, CursorLocation.Y - windowLocation.Y - lvUserProtocols.Location.Y);

            if (ClickLocation.X >= xMin && ClickLocation.X <= xMax)
            {
                // check which displayed item was clicked
                int itemHeight = lvUserProtocols.Size.Height / ITEMS_ON_SCREEN;
                int itemIndex = (int)((double)ClickLocation.Y / (double)itemHeight);

                // remove appropriate canceled item
                removeProtocol(itemIndex);
            }
        }
    }
        #endregion
}
