using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using GUI_Controls;
using GUI_Console;
using Tesla.Common;


namespace Util_GUI
{
    public partial class Form_Config : Form
    {
        private const int IMAGELIST_WIDTH = 40;
        private const int IMAGELIST_HEIGHT = 40;
        private const int TextBoxColumn = 1;
        private const int MinColumnWidth = IMAGELIST_WIDTH;
        private const string GUI_Config = "GUI";
        private const string TextPassPhase = "roboseptechsupport";
        private const string TextPassword = "password";

        private int FirstColumnWidth = MinColumnWidth;
        private int SecondColumnWidth = 300;

        private List<SectionParam> sectionParam = new List<SectionParam>();

        private int LVSmallChange = IMAGELIST_HEIGHT;
        private int LVLargeChange = IMAGELIST_HEIGHT;
        private bool allGroupsCollapsed = true;
        private bool RoboSepSystemInitialized = true;
        private bool configModified = false;

        // Listview Drawing Vars
        private StringFormat textFormat;
        private Color BG_ColorEven;
        private Color BG_ColorOdd;
        private Color BG_Selected;
        private Color Txt_Color;
        private Color TextBoxBackColor;
        private Color TextBoxForeColor;
        private Color TextBoxErrorBackColor;
        private Color TextBoxErrorForeColor;
        private Font TextBoxFont;

        private List<Image> headerImageList = new List<Image>();
        private List<Image> ilistGroupEmpty = new List<Image>();
        private List<Image> ilistGroupExpanded = new List<Image>();
        private List<Image> ilistGroupCollapsed = new List<Image>();
        private List<Image> ilistSaveNormal = new List<Image>();
        private List<Image> ilistSaveDirty = new List<Image>();

        private ConfigParam1[] myAllConfigParams = null;
        private IniFile LanguageINI = null;


        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        public Form_Config()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.UpdateStyles();

            ilistGroupEmpty.Add(GUI_Controls.Properties.Resources.BC_GroupEmpty_STD);
            ilistGroupEmpty.Add(GUI_Controls.Properties.Resources.BC_GroupEmpty_OVER);
            ilistGroupEmpty.Add(GUI_Controls.Properties.Resources.BC_GroupEmpty_OVER);
            ilistGroupEmpty.Add(GUI_Controls.Properties.Resources.BC_GroupEmpty_CLICK);

            ilistGroupExpanded.Add(GUI_Controls.Properties.Resources.BC_GroupExpanded_STD);
            ilistGroupExpanded.Add(GUI_Controls.Properties.Resources.BC_GroupExpanded_OVER);
            ilistGroupExpanded.Add(GUI_Controls.Properties.Resources.BC_GroupExpanded_OVER);
            ilistGroupExpanded.Add(GUI_Controls.Properties.Resources.BC_GroupExpanded_CLICK);

            ilistGroupCollapsed.Add(GUI_Controls.Properties.Resources.BC_GroupCollapsed_STD);
            ilistGroupCollapsed.Add(GUI_Controls.Properties.Resources.BC_GroupCollapsed_OVER);
            ilistGroupCollapsed.Add(GUI_Controls.Properties.Resources.BC_GroupCollapsed_OVER);
            ilistGroupCollapsed.Add(GUI_Controls.Properties.Resources.BC_GroupCollapsed_CLICK);
            button_AllGroups.ChangeGraphics(ilistGroupCollapsed);

            List<Image> ilist = new List<Image>();
            ilist.Add(GUI_Console.Properties.Resources.GE_BTN13L_cancel_STD);
            ilist.Add(GUI_Console.Properties.Resources.GE_BTN13L_cancel_OVER);
            ilist.Add(GUI_Console.Properties.Resources.GE_BTN13L_cancel_OVER);
            ilist.Add(GUI_Console.Properties.Resources.GE_BTN13L_cancel_CLICK);
            button_Cancel.ChangeGraphics(ilist);
            button_Cancel.disableImage = GUI_Console.Properties.Resources.GE_BTN21L_left_arrow_DISABLE;

            ilistSaveDirty.Add(GUI_Console.Properties.Resources.L_104x86_save_green);
            ilistSaveDirty.Add(GUI_Console.Properties.Resources.GE_BTN10L_save_OVER);
            ilistSaveDirty.Add(GUI_Console.Properties.Resources.GE_BTN10L_save_OVER);
            ilistSaveDirty.Add(GUI_Console.Properties.Resources.GE_BTN10L_save_CLICK);

            ilistSaveNormal.Add(GUI_Console.Properties.Resources.GE_BTN10L_save_STD);
            ilistSaveNormal.Add(GUI_Console.Properties.Resources.GE_BTN10L_save_OVER);
            ilistSaveNormal.Add(GUI_Console.Properties.Resources.GE_BTN10L_save_OVER);
            ilistSaveNormal.Add(GUI_Console.Properties.Resources.GE_BTN10L_save_CLICK);
            button_Save.ChangeGraphics(ilistSaveNormal);
            button_Save.disableImage = GUI_Console.Properties.Resources.GE_BTN10L_save_DISABLE;

            // set up for drawing
            textFormat = new StringFormat();
            textFormat.Alignment = StringAlignment.Near;
            textFormat.LineAlignment = StringAlignment.Center;
            textFormat.FormatFlags = StringFormatFlags.NoWrap;
            BG_ColorEven = Color.FromArgb(216, 217, 218);
            BG_ColorOdd = Color.FromArgb(243, 243, 243);
            BG_Selected = Color.FromArgb(78, 38, 131);
            Txt_Color = Color.FromArgb(95, 96, 98);
            TextBoxBackColor = Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(151)))), ((int)(((byte)(200)))));
            TextBoxForeColor = Color.Black;
            TextBoxFont = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            TextBoxErrorBackColor = Color.Red;
            TextBoxErrorForeColor = Color.White;
        }

        private void Form_Config_Load(object sender, EventArgs e)
        {
            RoboSepSystemInitialized = RoboSep_System.getInstance().IsInitialized;

            // get language file
            LanguageINI = GUI_Console.RoboSep_UserConsole.getInstance().LanguageINI;

            Rectangle rcPrimaryScreen = Screen.PrimaryScreen.Bounds;

            // center the form
            this.Location = new Point(rcPrimaryScreen.X + (rcPrimaryScreen.Width - this.Size.Width) / 2, rcPrimaryScreen.Y + (rcPrimaryScreen.Height - this.Size.Height) / 2);

            loadingTimer.Start();
        }


        private void LoadConfigVars()
        {
            string[] sections = UserDetails.getAllSections("GUI");
            if (sections == null)
                return;

            System.Collections.Specialized.NameValueCollection configItems = null;

            foreach (string section in sections)
            {
                if (string.IsNullOrEmpty(section))
                    continue;

                configItems = UserDetails.getSection("GUI", section);
                if (configItems == null || configItems.Count == 0)
                    continue;

                SectionParam para = new SectionParam(section);

                //enumerate the keys
                string value;
                foreach (string key in configItems)
                {
                    value = configItems[key];
                    if (string.IsNullOrEmpty(value))
                        continue;

                    if (key.ToLower() == TextPassword.ToLower())
                    {
                        try
                        {
                            // decrypt password
                            string sDecryptedPw = Utilities.DecryptString(value, TextPassPhase);
                            para.AddParam<string>(key, sDecryptedPw);
                        }
                        catch (Exception ex)
                        {
                            string msg = String.Format("Failed to decrypt password. Exception={0}", ex.Message);
                            MessageBox.Show(msg);
                        }
                    }
                    else
                    {
                        para.AddParam<string>(key, value);
                    }
                }
                sectionParam.Add(para);
            }
        }

        private void SetUpList()
        {
            if (sectionParam.Count == 0)
                return;

            this.SuspendLayout();

            // Create a GroupedListControl instance:
            ScrollPanel bsp = this.configScrollPanel;
            bsp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));

            myAllConfigParams = new ConfigParam1[sectionParam.Count];

            int nIndex = 0;
            foreach (SectionParam para in sectionParam)
            {
                if (para == null)
                    continue;

                myAllConfigParams[nIndex] = new ConfigParam1(para.SectionName);
                AddSection(ref bsp, para, myAllConfigParams[nIndex].SectionVars);
                nIndex++;
            }

            SetColumnWidths();

            AddEmbeddedControl();

            this.configScrollPanel.CollapseAll();

            this.configScrollPanel.VScrollbar = this.scrollBar1;
            bsp.AllGroupsExpanded += new ScrollPanel.AllGroupsExpansionHandler(bsp_AllGroupsExpanded);
            bsp.AllGroupsCollapsed += new ScrollPanel.AllGroupsExpansionHandler(bsp_AllGroupsCollapsed);

            this.ResumeLayout(true);
        }

        private void AddSection(ref ScrollPanel bsp, SectionParam para, List<ConfigParam2> lstVars)
        {
            if (bsp == null || para == null || lstVars == null)
                return;

            GroupListView2 glv2 = new GroupListView2();
            ConfigListView clv = glv2.EmbeddLV;
            clv.Scrollable = false;

            // Add  columns
            ColumnHeader columnHeader1 = new ColumnHeader();
            columnHeader1.Width = FirstColumnWidth;
            columnHeader1.TextAlign = HorizontalAlignment.Left;
            columnHeader1.Text = "Name";

            ColumnHeader columnHeader2 = new ColumnHeader();
            columnHeader2.Width = SecondColumnWidth;
            columnHeader2.TextAlign = HorizontalAlignment.Left;
            columnHeader2.Text = "Description";

            clv.Name = para.SectionName;
            clv.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2 });

            glv2.LabelText = para.SectionName;

            Node node = para.getNode();
            string configDesc, configValue;
            int nColumn1Width;
            while (node != null)
            {
                configDesc = ((TypeList<string>)node).getDesc();
                configValue = ((TypeList<string>)node).getData();

                AddItem(ref clv, lstVars, para.SectionName, configDesc, configValue);

                nColumn1Width = DetermineWidth(configDesc);
                FirstColumnWidth = nColumn1Width > FirstColumnWidth ? nColumn1Width : FirstColumnWidth;
                node = node.getNext();
            }

            // Add custom draw
            clv.OwnerDraw = true;
            clv.DrawItem += new DrawListViewItemEventHandler(clv_DrawItem);

            if (clv.RowHeight > LVSmallChange)
                LVSmallChange = clv.RowHeight;

            if (glv2.Height > LVLargeChange)
                LVLargeChange = glv2.Height;

            glv2.SetPreferedHeight();

            bsp.Controls.Add(glv2);
        }

        private void AddItem(ref ConfigListView lg, List<ConfigParam2> lstVars, string sSectionName, string sItemDesc, string sItemValue)
        {
            if (lg == null || lstVars == null)
                return;

            ConfigParam2 para2 = new ConfigParam2(sSectionName, sItemDesc, sItemValue, null);

            ListViewItem item = new ListViewItem(sItemDesc);
            ListViewItem.ListViewSubItem subItem1 = new ListViewItem.ListViewSubItem();
            subItem1.Text = sItemValue;
            subItem1.Tag = para2;
            item.SubItems.Add(subItem1);
            lg.Items.Add(item);
            lstVars.Add(para2);
        }

        public int DetermineWidth(string text)
        {
            if (string.IsNullOrEmpty(text))
                return -1;

            int width = MinColumnWidth;
            Size txtSize = new Size(width, int.MaxValue);
            TextFormatFlags flags = TextFormatFlags.LeftAndRightPadding;
            txtSize = TextRenderer.MeasureText(text, this.Font, txtSize, flags);
            width = txtSize.Width;

            if (width < MinColumnWidth)
                width = MinColumnWidth;

            width = width + Margin.Left + Margin.Right;
            return width;
        }

        private void SetColumnWidths()
        {
            ScrollPanel glc = this.configScrollPanel;
            int nControlCount = glc.Controls.Count;

            for (int g = 0; g < glc.Controls.Count; g++)
            {
                GroupListView2 glv = (GroupListView2)glc.Controls[g];
                if (glv == null)
                    continue;

                ConfigListView listview = glv.EmbeddLV;
                if (listview == null)
                    continue;

                if (1 < listview.Columns.Count)
                {
                    int nWidth = listview.Width;
                    listview.Columns[0].Width = FirstColumnWidth;
                    listview.Columns[1].Width = nWidth - FirstColumnWidth;
                }
            }
        }

        private void AddEmbeddedControl()
        {
            ScrollPanel glc = this.configScrollPanel;
            int nControlCount = glc.Controls.Count;

            for (int g = 0; g < glc.Controls.Count; g++)
            {
                GroupListView2 glv = (GroupListView2)glc.Controls[g];
                if (glv == null)
                    continue;

                ConfigListView listview = glv.EmbeddLV;
                if (listview == null)
                    continue;

                string temp;
                for (int i = 0; i < listview.Items.Count; i++)
                {
                    // Add edit box 
                    TextBox tb = new TextBox();
                    tb.Size = new Size(SecondColumnWidth, 40);
                    tb.Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    tb.BorderStyle = BorderStyle.FixedSingle;
                    tb.TextAlign = HorizontalAlignment.Left;
                    tb.AutoSize = false;
                    tb.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left);
                    tb.BackColor = TextBoxBackColor;
                    tb.Click += new System.EventHandler(this.tb_Click);
                    tb.TextChanged += new System.EventHandler(this.tb_TextChanged);
 
                    ListViewItem lvItemTemp = listview.Items[i];
                    temp = lvItemTemp.Text;
                    tb.Text = listview.Items[i].SubItems[TextBoxColumn].Text;
                    tb.Tag = listview.Items[i].SubItems[TextBoxColumn].Tag;

                    // text box
                    ConfigParam2 para2 = listview.Items[i].SubItems[TextBoxColumn].Tag as ConfigParam2;
                    para2.Obj = tb;

                    // Put it in the second column of every row
                    listview.AddEmbeddedControl(tb, TextBoxColumn, lvItemTemp.Index);
                    tb.Dispose();
                }
            }
        }


        private void loadingTimer_Tick(object sender, EventArgs e)
        {
            // stop timer
            loadingTimer.Stop();

            // load the config. variables
            LoadConfigVars();

            // set up the group list
            SetUpList();
        }


        private void tb_Click(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb == null)
                return;

            if (RoboSep_UserConsole.KeyboardEnabled)
                createKeybaord(tb);
        }

        private void tb_TextChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("tb_TextChanged called"));

            TextBox tb = sender as TextBox;
            if (tb == null)
                return;

            bool bDirty = IsConfigDirty();
            ChangeSaveButtonGraphics(bDirty);
        }

        private void createKeybaord(TextBox tb)
        {
            // Create keybaord control
            GUI_Controls.Keyboard newKeyboard =
                GUI_Controls.Keyboard.getInstance(this,
                   tb, null, null, false);
            newKeyboard.ShowDialog();
            newKeyboard.Dispose();
        }

        private void stopUSBWatcher()
        {
            RoboSep_System pSystem = RoboSep_System.getInstance();
            if (pSystem == null)
                return;

            bool bInitialized = pSystem.IsInitialized;
            if (bInitialized && !RoboSepSystemInitialized)
            {
                pSystem.StopWatchUSB();
            }
        }

        private void ChangeSaveButtonGraphics(bool bDirty)
        {
            button_Save.ChangeGraphics(bDirty ? ilistSaveDirty : ilistSaveNormal);
        }

        private bool IsConfigDirty()
        {
            if (myAllConfigParams == null || myAllConfigParams.Length == 0)
                return false;

            bool bDirty = false;
            List<ConfigParam2> sectionVars;
            foreach (ConfigParam1 para1 in myAllConfigParams)
            {
                if (para1 == null)
                    continue;

                sectionVars = para1.SectionVars;
                if (sectionVars == null)
                    continue;

                foreach (ConfigParam2 para2 in sectionVars)
                {
                    TextBox tb = para2.Obj as TextBox;
                    if (tb == null)
                        continue;

                    if (tb.Text.Trim() != para2.ConfigValue)
                    {
                        bDirty = true;
                        break;
                    }
                }

                if (bDirty)
                    break;
            }
            return bDirty;
        }

        private bool SaveConfig()
        {
            if (myAllConfigParams == null || myAllConfigParams.Length == 0)
                return true;

            string sSectionName;
            List<ConfigParam2> sectionVars;
            string[] parts = new string[2];
            List<string> lstVars = new List<string>();
            bool bDirty = false;
            foreach (ConfigParam1 para1 in myAllConfigParams)
            {
                if (para1 == null)
                    continue;

                sectionVars = para1.SectionVars;
                if (sectionVars == null)
                    continue;

                foreach (ConfigParam2 para2 in sectionVars)
                {
                    TextBox tb = para2.Obj as TextBox;
                    if (tb == null)
                        continue;

                    if (!bDirty && (tb.Text.Trim() != para2.ConfigValue))
                    {
                        bDirty = true;
                    }
                    parts[0] = para2.ConfigVar;
                    parts[1] = tb.Text.Trim();

                    if (parts[0].ToLower() == TextPassword.ToLower())
                    {
                        try
                        {
                            // decrypt password
                            string sEncryptedPw = Utilities.EncryptString(parts[1], TextPassPhase);
                            parts[1] = sEncryptedPw;
                        }
                        catch (Exception ex)
                        {
                            string msg = String.Format("Failed to encrypt password. Exception={0}", ex.Message);
                            MessageBox.Show(msg);
                        }
                    }

                    lstVars.Add(string.Join("=", parts));
                }

                sSectionName = para1.SectionName;

                if (bDirty && 0 < lstVars.Count)
                {
                    string[] tempArray = lstVars.ToArray();
                    Array.Sort(tempArray);
                    UserDetails.saveSection(GUI_Config, sSectionName, tempArray);
                    configModified = true;
                }
                lstVars.Clear();
                bDirty = false;
            }
            return true;
        }

        private void lg_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            if (this.headerImageList.Count == 0)
                return;

            System.Drawing.Drawing2D.LinearGradientBrush GradientBrush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.Blue, Color.LightBlue, 270);
            SolidBrush theBrush1 = new SolidBrush(Color.White);
            SolidBrush theBrush2 = new SolidBrush(Color.Yellow);
            Pen thePen = new Pen(theBrush1, 2);
            Font theFont = new Font("Arial", 12); 
            
            e.Graphics.FillRectangle(GradientBrush, e.Bounds);

            e.Graphics.DrawRectangle(thePen, e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2);
            e.Graphics.DrawImage(this.headerImageList[4], e.Bounds);

            e.Graphics.DrawString(e.Header.Text, theFont, theBrush2, e.Bounds);

            theFont.Dispose();
            thePen.Dispose();
            theBrush1.Dispose();
            theBrush2.Dispose();

        }

        private void clv_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            ConfigListView lg = (ConfigListView)sender;
            SolidBrush theBrush1, theBrush2;

            Color textColour = Txt_Color;
            Rectangle itemBounds = new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width, e.Bounds.Bottom - e.Bounds.Top);
            if (e.Item.Selected)
            {
                theBrush1 = new SolidBrush(BG_Selected); 
                e.Graphics.FillRectangle(theBrush1, itemBounds);
                textColour = Color.White;
            }
            else if (e.ItemIndex % 2 == 0)
            {
                theBrush1 = new SolidBrush(BG_ColorEven);
                e.Graphics.FillRectangle(theBrush1, itemBounds);
            }
            else
            {
                theBrush1 = new SolidBrush(BG_ColorOdd);
                e.Graphics.FillRectangle(theBrush1, itemBounds);
            }

            theBrush2 = new SolidBrush(textColour);
            e.Graphics.DrawString(e.Item.Text, lg.Font, theBrush2,
                new Rectangle(itemBounds.Left, itemBounds.Top, itemBounds.Width, itemBounds.Bottom - itemBounds.Top), textFormat);
            theBrush1.Dispose();
            theBrush2.Dispose();

        }

        private void clv_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
        }

        private void bsp_AllGroupsCollapsed(object sender, EventArgs e)
        {
            button_AllGroups.ChangeGraphics(ilistGroupCollapsed);
            allGroupsCollapsed = true;
        }
        private void bsp_AllGroupsExpanded(object sender, EventArgs e)
        {
            button_AllGroups.ChangeGraphics(ilistGroupExpanded);
            allGroupsCollapsed = false;
        }

        private void button_AllGroups_Click(object sender, EventArgs e)
        {
            if (allGroupsCollapsed)
            {
                this.configScrollPanel.ExpandAll();
            }
            else
            {
                this.configScrollPanel.CollapseAll();
            }
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            bool bDirty = IsConfigDirty();

            if (bDirty)
            {
                if (!SaveConfig())
                    return;

                ChangeSaveButtonGraphics(false);

                // Ask user to exit
                string msgHeader = LanguageINI.GetString("headerSaveGUIConfig");
                string msg = LanguageINI.GetString("msgSaveConfigAndExit"); 
                GUI_Controls.RoboMessagePanel prompt = new RoboMessagePanel(this, MessageIcon.MBICON_QUESTION, msg,
                msgHeader, "Yes", "No");
                // prompt user
                prompt.ShowDialog();
                if (prompt.DialogResult != DialogResult.OK)
                {
                    prompt.Dispose();
                    return;
                }
                prompt.Dispose();
            }
            else
            {
                // Ask user to exit
                string msgHeader = LanguageINI.GetString("headerExitUtil");
                string msg = LanguageINI.GetString("msgExitUtil");
                GUI_Controls.RoboMessagePanel prompt = new RoboMessagePanel(this, MessageIcon.MBICON_QUESTION, msg,
                msgHeader, "Yes", "No");
                // prompt user
                prompt.ShowDialog();
                if (prompt.DialogResult != DialogResult.OK)
                {
                    prompt.Dispose();
                    return;
                }
                prompt.Dispose();
            }

            stopUSBWatcher();

            this.DialogResult = configModified ? DialogResult.OK : DialogResult.Cancel;
            this.Close();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            stopUSBWatcher();
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }

    public class ConfigParam1
    {
        string sectionName;
        List<ConfigParam2> sectionVars = new List<ConfigParam2>();

        public ConfigParam1(string sectionName)
        {
            this.sectionName = sectionName;
        }

        public string SectionName
        {
            get { return sectionName; }
            set { sectionName = value; }
        }

        public List<ConfigParam2> SectionVars
        {
            get { return sectionVars; }
            set
            {
                if (value != null)
                {
                    sectionVars.Clear();
                    sectionVars.AddRange(value);
                }
            }
        }
    }

    public class ConfigParam2
    {
        string sectionName;
        string configVar;
        string configValue;
        object obj = null;

        public ConfigParam2(string sectionName, string configVar, string configValue, object obj)
        {
            this.sectionName = sectionName;
            this.configVar = configVar;
            this.configValue = configValue;
            this.obj = obj;

        }
        public string SectionName
        {
            get { return sectionName; }
            set { sectionName = value; }
        }
        public string ConfigVar
        {
            get { return configVar; }
            set { configVar = value; }
        }
        public string ConfigValue
        {
            get { return configValue; }
            set { configValue = value; }
        }
        public object Obj
        {
            get { return obj; }
            set { obj = value; }
        }
    }



    public class ConfigParam<T>
    {
        string configDesc;
        T configValue;
        public ConfigParam(string desc, T configValue)
        {
            this.configDesc = desc;
            this.configValue = configValue;
        }

        public string ConfigDesc
        {
            get { return configDesc; }
            set { configDesc = value; }
        }
        public T ConfigValue
        {
            get { return configValue; }
            set { configValue = value; }
        }
    }

    public class SectionParam
    {
        private Node node_;
        string sectionName_;

        public SectionParam(string name)
        {
            sectionName_ = name;
        }

        public void AddParam<T>(string name, T data)
        {
            node_ = new TypeList<T>(data, name, node_);
        }

        public string SectionName
        {
            get { return sectionName_; }
        }

        public Node getNode()
        {
            return node_;
        }

    }

    public class Node
    {
        private Node next_;

        public Node(Node next)
        {
            next_ = next;
        }

        public Node getNext()
        {
            return next_;
        }
    }

    internal sealed class TypeList<T> : Node
    {
        T data_;
        string desc_;

        public T getData()
        {
            return data_;
        }

        public string getDesc()
        {
            return desc_;
        }

        public TypeList(T data, string desc, Node next)
            : base(next)
        {
            data_ = data;
            desc_ = desc;
        }

        public TypeList(T data, string desc)
            : this(data, desc, null)
        {

        }
    }
    class Dummmy : Object
    {
        public override String ToString()
        {
            return "Dummytype".ToString();

        }
    }

}
