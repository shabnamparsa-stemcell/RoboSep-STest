using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GUI_Controls
{
    public partial class GroupListView2 : UserControl, IGroupView
    {
        private const int IMAGELIST_WIDTH = 40;
        private const int IMAGELIST_HEIGHT = 40;

        private List<Image> ilistGroupEmpty = new List<Image>();
        private List<Image> ilistGroupExpanded = new List<Image>();
        private List<Image> ilistGroupCollapsed = new List<Image>();

        private string labelText = "";
        private string id;

        private ImageList imglist = new ImageList();

         // Delegate and related events to process Group Expansion and Collapse:
        public event GroupExpansionHandler GroupExpanded;
        public event GroupExpansionHandler GroupCollapsed;

        public GroupListView2()
        {
            InitializeComponent();

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
            button_Group.ChangeGraphics(ilistGroupCollapsed);

            VScroll = false;
            HScroll = false;
        }

        public ConfigListView EmbeddLV
        {
            get
            {
                return this.configListView;
            }
        }

        public string LabelText
        {
            set
            {
                labelText = value;
                label1.Text = labelText;
            }
        }

        public string ID
        {
            get
            {
                return this.id;
            }

            set
            {
                this.id = value;
            }
        }

        public bool IsCollapsed()
        {
            bool bCollapsed = false;
            if (this.configListView.Visible == false || this.configListView.Items.Count == 0)
            {
                bCollapsed = true;
            }
            return bCollapsed;
        }

        public bool IsExpanded()
        {
            bool bExpanded = false;
            if (this.configListView.Visible == true && this.configListView.Items.Count > 0)
            {
                bExpanded = true;
            }
            return bExpanded;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            panel1.Width = this.Width;
            this.configListView.Width = this.Width;
        }
        
        private void button_Group_Click(object sender, EventArgs e)
        {
            if (this.configListView.Items.Count == 0)
            {
                button_Group.ChangeGraphics(ilistGroupEmpty);
            }
            else if (this.configListView.Visible == false)
            {
                // Expand
                Expand();
                button_Group.ChangeGraphics(ilistGroupExpanded);
            }
            else if (this.configListView.Visible == true)
            {
                // Collapse
                Collapse();
                button_Group.ChangeGraphics(ilistGroupCollapsed);
            }
        }

        public void Collapse()
        {
            if (this.configListView.Visible == true)
            {
                this.configListView.Visible = false;
                this.Height = this.panel1.Height;
                if (0 < this.configListView.Items.Count)
                {
                    button_Group.ChangeGraphics(ilistGroupCollapsed);

                    // Raise the Collapsed event to notify client code that the Group has collapsed:
                    if (this.GroupCollapsed != null)
                        this.GroupCollapsed(this, new EventArgs());
                }
                else
                {
                    button_Group.ChangeGraphics(ilistGroupEmpty);
                }
            }
        }

        public void Expand()
        {
            if (this.configListView.Visible == false)
            {
                if (0 < this.configListView.Items.Count)
                {
                    this.configListView.Visible = true;
                    this.Height = GetPreferedHeight();
                    button_Group.ChangeGraphics(ilistGroupExpanded);

                    // Raise the Expanded event to notify client code that the Group has expanded:
                    if (this.GroupExpanded != null)
                        this.GroupExpanded(this, new EventArgs());
                }
                else
                {
                    button_Group.ChangeGraphics(ilistGroupEmpty);
                }
            }
        }

        public bool Equal(object obj)
        {
            if (obj == null)
                return false;

            GroupListView2 gv = obj as GroupListView2;
            if (gv == null)
                return false;

            bool bEqual = false;
            if (this.Equal(gv))
            {
                bEqual = true;
            }
            return bEqual;
        }

        public void RefreshView()
        {
            this.Refresh();
        }

        public int PreferedHeight
        {
            get { return GetPreferedHeight(); }
        }

        public int GetPreferedHeight()
        {
            return this.configListView.Items.Count * this.configListView.RowHeight + this.panel1.Size.Height; 
        }

        public void SetPreferedHeight()
        {
            this.configListView.VisibleRow = this.configListView.Items.Count;
            int nHeight = this.configListView.Items.Count * this.configListView.RowHeight;
            if (this.configListView.MaximumSize.Height < nHeight)
            {
                this.configListView.MaximumSize = new Size(this.configListView.MaximumSize.Width, nHeight);
            }
            this.configListView.Height = nHeight;
            this.Height = this.configListView.Items.Count * this.configListView.RowHeight + this.panel1.Size.Height;
        }

        public int GetRowHeight()
        {
            return this.configListView.RowHeight;
        }

        public int GetHeaderHeight()
        {
            return this.panel1.Height;
        }

        public int GetListViewVisibleRowsCount()
        {
            int nVisibleRows = 0;
            if (this.configListView.Visible == true)
            {
                nVisibleRows = this.configListView.Items.Count;
            }
            return nVisibleRows;
        }
    }
}
