using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Xml;
using System.Text;
using System.Windows.Forms;

using Tesla.Common;
using Invetech.ApplicationLog;

namespace GUI_Console
{
    public partial class Form_UserSelect : Form
    {
        private const char reservedChar = '^';
        private const char spaceChar = ' ';
        private Point Offset;
        private List<string> slistUserNames;
        private IniFile LanguageINI;
        private string SelectedUser;
        private StringFormat theFormat;
        private Color BG_ColorEven;
        private Color BG_ColorOdd;
        private Color BG_Selected;
        private Color Txt_Color;
        private const int MAX_USERS_DISPLAYED = 8;

        private List<Image> iListSortAscend;
        private List<Image> iListSortDescend;

        private List<Image> ilistOKNormal = new List<Image>();
        private List<Image> ilistOKGreen = new List<Image>();


        public Form_UserSelect(List<string> names, string title)
        {
            InitializeComponent();

            SuspendLayout();

            // LOG
            string logMSG = "Creating new User Select window";
            //GUI_Controls.uiLog.LOG(this, "Form_UserSelect", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                

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
            button_Sort.ChangeGraphics(iListSortAscend);

            // OK button
            ilistOKNormal.Clear();
            ilistOKNormal.Add(Properties.Resources.GE_BTN20L_ok_STD);
            ilistOKNormal.Add(Properties.Resources.GE_BTN20L_ok_OVER);
            ilistOKNormal.Add(Properties.Resources.GE_BTN20L_ok_OVER);
            ilistOKNormal.Add(Properties.Resources.GE_BTN20L_ok_CLICK);
            button_OK.ChangeGraphics(ilistOKNormal);

            ilistOKGreen.Clear();
            ilistOKGreen.Add(Properties.Resources.L_104x86_check_green);
            ilistOKGreen.Add(Properties.Resources.GE_BTN20L_ok_OVER);
            ilistOKGreen.Add(Properties.Resources.GE_BTN20L_ok_OVER);
            ilistOKGreen.Add(Properties.Resources.GE_BTN20L_ok_CLICK);

            //change toggle button graphics
            List<Image> ilist = new List<Image>();

            // ScrollUp
            ilist.Clear();
            ilist.Add(Properties.Resources.GE_BTN03M_up_arrow_STD);
            ilist.Add(Properties.Resources.GE_BTN03M_up_arrow_OVER);
            ilist.Add(Properties.Resources.GE_BTN03M_up_arrow_OVER);
            ilist.Add(Properties.Resources.GE_BTN03M_up_arrow_CLICK);
            button_ScrollUp.ChangeGraphics(ilist);

            // Down
            ilist.Clear();
            ilist.Add(Properties.Resources.GE_BTN04M_down_arrow_STD);
            ilist.Add(Properties.Resources.GE_BTN04M_down_arrow_OVER);
            ilist.Add(Properties.Resources.GE_BTN04M_down_arrow_OVER);
            ilist.Add(Properties.Resources.GE_BTN04M_down_arrow_CLICK);
            button_ScrollDown.ChangeGraphics(ilist);

            ilist.Clear();
            ilist.Add(Properties.Resources.L_104x86_single_arrow_left_STD);
            ilist.Add(Properties.Resources.L_104x86_single_arrow_left_OVER);
            ilist.Add(Properties.Resources.L_104x86_single_arrow_left_OVER);
            ilist.Add(Properties.Resources.L_104x86_single_arrow_left_CLICK);
            button_Cancel.ChangeGraphics(ilist);

            LanguageINI = GUI_Console.RoboSep_UserConsole.getInstance().LanguageINI;
            label_WindowTitle.Text = title;

            slistUserNames = names;

            ResumeLayout();
        }

        private void Form_UserSelect_Load(object sender, EventArgs e)
        {
             // add forms to form tracker
            Offset = new Point((RoboSep_UserConsole.getInstance().Size.Width - this.Size.Width) / 2,
                (RoboSep_UserConsole.getInstance().Size.Height - this.Size.Height) / 2);

            this.Location = new Point(Offset.X + RoboSep_UserConsole.getInstance().Location.X,
                Offset.Y + RoboSep_UserConsole.getInstance().Location.Y);

            RoboSep_UserConsole.getInstance().addForm(this, Offset);

            // set up listview drag scroll properties
            listView_users.VERTICAL_PAGE_SIZE = listView_users.VisibleRow;

            if (listView_users.VERTICAL_PAGE_SIZE > (listView_users.VisibleRow -1))
                listView_users.VERTICAL_PAGE_SIZE = listView_users.VisibleRow -1;

            // Load users into listview items
            loadUserList();

            // set up for drawing
            theFormat = new StringFormat();
            theFormat.Alignment = StringAlignment.Near;
            theFormat.LineAlignment = StringAlignment.Center;
            BG_ColorEven = Color.FromArgb(216, 217, 218);
            BG_ColorOdd = Color.FromArgb(243, 243, 243);
            BG_Selected = Color.FromArgb(78, 38, 131);
            Txt_Color = Color.FromArgb(95, 96, 98);
        }

        // loads names from slistUserNames list to listview items
        private void loadUserList()
        {
            // add items to list view
            listView_users.SuspendLayout();

            int nIndex;
            string sName;
            ListViewItem lvItem;
            for (int i = 0; i < slistUserNames.Count; i++)
            {
                sName = slistUserNames[i];
                nIndex = slistUserNames[i].IndexOf(reservedChar);
                if (0 < nIndex)
                {
                    sName = sName.Replace(reservedChar, spaceChar);
                }
                lvItem = new ListViewItem(sName);
                lvItem.Tag = slistUserNames[i];

                listView_users.Items.Add(lvItem);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, "adding user " + slistUserNames[i] + " to UserSelect window");

            }

            listView_users.ResumeLayout();
            listView_users.Refresh();
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

        private void Form_UserSelect_Deactivate(object sender, EventArgs e)
        {
            RoboSep_UserConsole.getInstance().frmOverlay.BringToFront();
            this.BringToFront();
            this.Activate();
        }

        private void Form_UserSelect_LocationChanged(object sender, EventArgs e)
        {
            RoboSep_UserConsole.getInstance().frmOverlay.BringToFront();
            this.BringToFront();
        }

        private void listView_users_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            System.Drawing.Drawing2D.LinearGradientBrush GradientBrush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.Blue, Color.LightBlue, 270);
            SolidBrush theBrush = new SolidBrush(Color.White);
            SolidBrush theBrush2 = new SolidBrush(Color.Yellow);
            Pen thePen = new Pen(theBrush, 2);
            Font theFont = new Font("Arial", 12); 
            e.Graphics.FillRectangle(GradientBrush, e.Bounds);

            e.Graphics.DrawRectangle(thePen, e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2);

            e.Graphics.DrawString(e.Header.Text, theFont, theBrush2, e.Bounds);
            theFont.Dispose();
            thePen.Dispose();
            theBrush2.Dispose();
            theBrush.Dispose();
            GradientBrush.Dispose();
        }

        private void listView_users_DrawItem(object sender, DrawListViewItemEventArgs e)
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
        }

        public string User
        {
            get
            {
                return SelectedUser;
            }
        }

        private void listView_users_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            // format text
            SolidBrush theBrush;
            if (e.Item.Selected)
            {
                theBrush = new SolidBrush(Color.White);
                e.Graphics.DrawString(e.Item.Text, listView_users.Font, theBrush, e.Bounds, theFormat);
                theBrush.Dispose();

            }
            else
            {
                theBrush = new SolidBrush(Txt_Color);
                e.Graphics.DrawString(e.Item.Text, listView_users.Font, theBrush, e.Bounds, theFormat); 
                theBrush.Dispose();

            }
        }

        private void listView_users_SelectedIndexChanged(object sender, EventArgs e)
        {
            button_OK.ChangeGraphics(listView_users.SelectedItems.Count > 0 ? ilistOKGreen : ilistOKNormal);
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            if (listView_users.SelectedItems.Count == 0)
            {
                // prompt user
                string title = LanguageINI.GetString("headerSelectUserFromList");
                string msg = LanguageINI.GetString("lblSelectUserFromList");
                string buttonTxt = LanguageINI.GetString("Ok");
                GUI_Controls.RoboMessagePanel prompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(), GUI_Controls.MessageIcon.MBICON_WARNING, msg, title, buttonTxt);
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                prompt.Dispose();
                RoboSep_UserConsole.hideOverlay();
                return;
            }
            SelectedUser = listView_users.SelectedItems[0].Tag as string;
            RoboSep_UserConsole.hideOverlay();
            this.DialogResult = DialogResult.OK;
            this.Close();

            // LOG
            string logMSG = "User " + listView_users.SelectedItems[0].Text + " Selected";

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                

        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            RoboSep_UserConsole.hideOverlay();
            this.Close();

            RoboSep_UserConsole.getInstance().Activate();
            this.DialogResult = DialogResult.Cancel;

            // LOG
            string logMSG = "User Select window closed without selection";

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                
        }

        private void button_ScrollUp_Click(object sender, EventArgs e)
        {
            this.listView_users.LineUp();
        }

        private void button_ScrollDown_Click(object sender, EventArgs e)
        {
            this.listView_users.LineDown();
        }


        private void StartScrollUp(object sender, MouseEventArgs e)
        {
            this.listView_users.StartScrollingUp();
        }
        private void StopScrollUp(object sender, MouseEventArgs e)
        {
            this.listView_users.StopScrollingUp();
        }

        private void StartScrollDown(object sender, MouseEventArgs e)
        {
            this.listView_users.StartScrollingDown();
        }
        private void StopScrollDown(object sender, MouseEventArgs e)
        {
            this.listView_users.StopScrollingDown();
        }
        private void button_ScrollUp_MouseDown(object sender, MouseEventArgs e)
        {
            StartScrollUp(sender, e);
        }
        private void button_ScrollUp_MouseUp(object sender, MouseEventArgs e)
        {
            StopScrollUp(sender, e);
        }
        private void button_ScrollDown_MouseDown(object sender, MouseEventArgs e)
        {
            StartScrollDown(sender, e);
        }
        private void button_ScrollDown_MouseUp(object sender, MouseEventArgs e)
        {
            StopScrollDown(sender, e);
        }
        private void button_Sort_Click(object sender, EventArgs e)
        {
            if (this.listView_users.Items.Count == 0)
                return;

            // get currently selected item
            string selectedUserName = "";
            if (this.listView_users.SelectedItems.Count > 0)
            {
                selectedUserName = listView_users.SelectedItems[0].Text;
            }
            switch (this.listView_users.Sorting)
            {
                case SortOrder.Ascending:
                    this.listView_users.Sorting = SortOrder.Descending;
                    button_Sort.ChangeGraphics(iListSortDescend);
                    break;
                case SortOrder.Descending:
                    this.listView_users.Sorting = SortOrder.Ascending;
                    button_Sort.ChangeGraphics(iListSortAscend);
                    break;
                case SortOrder.None:
                    this.listView_users.Sorting = SortOrder.Ascending;
                    button_Sort.ChangeGraphics(iListSortAscend);
                    break;
                default:
                    break;
            }
            this.listView_users.Sort();

            if (!string.IsNullOrEmpty(selectedUserName))
            {
                foreach (ListViewItem lvItem in listView_users.Items)
                {
                    // ensure the previous selected item is visible 
                    if (lvItem.Text == selectedUserName)
                    {
                        // Select the previous selected protocol
                        lvItem.Selected = true;
                        listView_users.EnsureVisible(lvItem.Index);
                        break;
                    }
                }
            }
       }
    }
}
