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
    public delegate void GroupExpansionHandler(object sender, EventArgs e);

    public interface IGroupView
    {
        event GroupExpansionHandler GroupExpanded;
        event GroupExpansionHandler GroupCollapsed;
        int GetHeaderHeight();
        int GetRowHeight();
        bool Equal(object gv);
        void Collapse();
        void Expand();
        void RefreshView();
        bool IsCollapsed();
        bool IsExpanded();
        int GetListViewVisibleRowsCount();

        int Width { get; set; }
        string ID { get; set; }
    }

    public partial class GroupListView : UserControl, IGroupView
    {
        private const int IMAGELIST_WIDTH = 40;
        private const int IMAGELIST_HEIGHT = 40;

        private string id;

        List<Image> ilistGroupEmpty = new List<Image>();
        List<Image> ilistGroupExpanded = new List<Image>();
        List<Image> ilistGroupCollapsed = new List<Image>();

        string labelText = "";
        ImageList imglist = new ImageList();

        // Delegate and related events to process Group Expansion and Collapse:
        public event GroupExpansionHandler GroupExpanded;
        public event GroupExpansionHandler GroupCollapsed;

        public GroupListView()
        {
            InitializeComponent();

            List<Image> ilist = new List<Image>();
            ilist.Add(Properties.Resources.scan_N_STD);
            ilist.Add(Properties.Resources.scan_N_OVER);
            ilist.Add(Properties.Resources.scan_N_OVER);
            ilist.Add(Properties.Resources.scan_N_CLICK);
            button_Scan.ChangeGraphics(ilist);
            button_Scan.disableImage = Properties.Resources.scan_N_DISABLE;

            ilistGroupEmpty.Add(Properties.Resources.BC_GroupEmpty_STD);
            ilistGroupEmpty.Add(Properties.Resources.BC_GroupEmpty_OVER);
            ilistGroupEmpty.Add(Properties.Resources.BC_GroupEmpty_OVER);
            ilistGroupEmpty.Add(Properties.Resources.BC_GroupEmpty_CLICK);

            ilistGroupExpanded.Add(Properties.Resources.BC_GroupExpanded_STD);
            ilistGroupExpanded.Add(Properties.Resources.BC_GroupExpanded_OVER);
            ilistGroupExpanded.Add(Properties.Resources.BC_GroupExpanded_OVER);
            ilistGroupExpanded.Add(Properties.Resources.BC_GroupExpanded_CLICK);

            ilistGroupCollapsed.Add(Properties.Resources.BC_GroupCollapsed_STD);
            ilistGroupCollapsed.Add(Properties.Resources.BC_GroupCollapsed_OVER);
            ilistGroupCollapsed.Add(Properties.Resources.BC_GroupCollapsed_OVER);
            ilistGroupCollapsed.Add(Properties.Resources.BC_GroupCollapsed_CLICK);
            button_Group.ChangeGraphics(ilistGroupCollapsed);

            VScroll = false;
            HScroll = false;
        }

        private void SetupImageList()
        {
            imglist.ImageSize = new System.Drawing.Size(IMAGELIST_WIDTH, IMAGELIST_HEIGHT);
            Color textColour = Color.FromArgb(95, 96, 98);
            Font textFont = this.Font;
            StringFormat textFormat = new StringFormat();
            textFormat.Alignment = StringAlignment.Center;
            textFormat.LineAlignment = StringAlignment.Center;

            Bitmap bmQuadrantNumber = new Bitmap(Properties.Resources.QuadrantNumber);
            Bitmap bm1, bm2;

            for (int i = 0; i <= 3; i++)
            {
                int number = i + 1;

                bm1 = new Bitmap(bmQuadrantNumber);
                bm2 = new Bitmap(bm1);
                Graphics g = Graphics.FromImage(bm1);
                SolidBrush br = new SolidBrush(textColour);
                g.DrawString(number.ToString(), textFont, br,
                        new Rectangle(0, 0, IMAGELIST_WIDTH, IMAGELIST_HEIGHT), textFormat);

                string quadrant = "QUADRANT_IMAGE_KEY";
                quadrant += i.ToString();
                imglist.Images.Add(quadrant, bm2);
                var dummy = imglist.Handle; //This is dumb, but it has to be here
                bm1.Dispose();
                bm2.Dispose();
                br.Dispose();
                g.Dispose();
            }
            bmQuadrantNumber.Dispose();
        }

        public BarcodeListView EmbeddLV
        {
            get
            {
                return this.barcodeListView;
            }
        }

        public GUIButton EmbeddButtonScan
        {
            get
            {
                return this.button_Scan;
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
            if (this.barcodeListView.Visible == false || this.barcodeListView.Items.Count == 0)
            {
                bCollapsed = true;
            }
            return bCollapsed;
        }

        public bool IsExpanded()
        {
            bool bExpanded = false;
            if (this.barcodeListView.Visible == true && this.barcodeListView.Items.Count > 0)
            {
                bExpanded = true;
            }
            return bExpanded;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            panel1.Width = this.Width;
            this.barcodeListView.Width = this.Width;
        }
        

        private void button_Group_Click(object sender, EventArgs e)
        {
            if (this.barcodeListView.Items.Count == 0)
            {
                button_Group.ChangeGraphics(ilistGroupEmpty);
                button_Scan.disable(true);
            }
            else if (this.barcodeListView.Visible == false)
            {
                // Expand
                Expand();
                button_Group.ChangeGraphics(ilistGroupExpanded);
            }
            else if (this.barcodeListView.Visible == true)
            {
                // Collapse
                Collapse();
                button_Group.ChangeGraphics(ilistGroupCollapsed);
            }
        }

        public void Collapse()
        {
            if (this.barcodeListView.Visible == true)
            {
                this.barcodeListView.Visible = false;
                this.Height = this.panel1.Height;
                if (0 < this.barcodeListView.Items.Count)
                {
                    button_Group.ChangeGraphics(ilistGroupCollapsed);

                    // Raise the Collapsed event to notify client code that the Group has collapsed:
                    if (this.GroupCollapsed != null)
                        this.GroupCollapsed(this, new EventArgs());
                }
                else
                {
                    button_Group.ChangeGraphics(ilistGroupEmpty);
                    button_Scan.disable(true);
                }
            }
        }

        public void Expand()
        {
            if (this.barcodeListView.Visible == false)
            {
                if (0 < this.barcodeListView.Items.Count)
                {
                    this.barcodeListView.Visible = true;
                    this.Height = GetPreferedHeight();
                    button_Group.ChangeGraphics(ilistGroupExpanded);

                    // Raise the Expanded event to notify client code that the Group has expanded:
                    if (this.GroupExpanded != null)
                        this.GroupExpanded(this, new EventArgs());
                }
                else
                {
                    button_Group.ChangeGraphics(ilistGroupEmpty);
                    button_Scan.disable(true);
                }
            }
        }

        public bool Equal(object obj)
        {
            if (obj == null)
                return false;

            GroupListView gv = obj as GroupListView;
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
            return this.barcodeListView.Items.Count * this.barcodeListView.RowHeight + this.panel1.Size.Height /* + this.barcodeListView.Margin.Top + this.barcodeListView.Margin.Bottom */; 
        }

        public void SetPreferedHeight()
        {
            this.Height = this.barcodeListView.Items.Count * this.barcodeListView.RowHeight + this.panel1.Size.Height /* + this.barcodeListView.Margin.Top + this.barcodeListView.Margin.Bottom */ ;
        }

        public int GetRowHeight()
        {
            return this.barcodeListView.RowHeight;
        }

        public int GetHeaderHeight()
        {
            return this.panel1.Height;
        }

        public int GetListViewVisibleRowsCount()
        {
            int nVisibleRows = 0;
            if (this.barcodeListView.Visible == true)
            {
                nVisibleRows = this.barcodeListView.Items.Count;
            }
            return nVisibleRows;
        }

        public void EnableScanButton(bool bEnable)
        {
            button_Scan.disable(!bEnable);
        }

    }
}
