using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.Layout;
using System.Runtime.InteropServices;


namespace GUI_Controls
{
    public class ScrollPanel : FlowLayoutPanel
    {

        [StructLayout(LayoutKind.Sequential)]
        struct SCROLLINFO
        {
            public uint cbSize;
            public uint fMask;
            public int nMin;
            public int nMax;
            public uint nPage;
            public int nPos;
            public int nTrackPos;
        }

        private enum ScrollInfoMask
        {
            SIF_RANGE = 0x1,
            SIF_PAGE = 0x2,
            SIF_POS = 0x4,
            SIF_DISABLENOSCROLL = 0x8,
            SIF_TRACKPOS = 0x10,
            SIF_ALL = SIF_RANGE + SIF_PAGE + SIF_POS + SIF_TRACKPOS
        }

        private enum SBTYPES
        {
            SB_HORZ = 0,
            SB_VERT = 1,
            SB_CTL = 2,
            SB_BOTH = 3
        }

        private enum LPCSCROLLINFO
        {
            SIF_RANGE = 0x0001,
            SIF_PAGE = 0x0002,
            SIF_POS = 0x0004,
            SIF_DISABLENOSCROLL = 0x0008,
            SIF_TRACKPOS = 0x0010,
            SIF_ALL = (SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS)
        }

        private int displayRows = 6;
        private List<int> lstDisplayRows = new List<int>();
        private int updating = 0;
        private int topItem = 0;
        private int scrollPanelMinimumHeight = 0;
        private int headeHeight = 0;
        private bool setMinScrollPanelHeight = false;
             
        private ICustomScrollbar vScrollbar = null;
        private int disableChangeEvents = 0;

        // Delegate and related events to process Group Expansion and Collapse:
        public delegate void AllGroupsExpansionHandler(object sender, EventArgs e);
        public event AllGroupsExpansionHandler AllGroupsExpanded;
        public event AllGroupsExpansionHandler AllGroupsCollapsed;

        Dictionary<string, int> dictGroupStartRow = new Dictionary<string, int>();

        const int GWL_STYLE = -16;
        const int WS_VSCROLL = 0x00200000;

        private const int WM_VSCROLL = 0x0115;
        private const int WM_NCCALCSIZE = 0x83;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetScrollInfo(IntPtr hwnd, int fnBar, ref SCROLLINFO lpsi);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

        public ScrollPanel()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            // Default configuration. 
            this.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.WrapContents = true;
            this.AutoScroll = true;
            this.AutoSize = false;
            this.VScroll = true; 
            this.HScroll = true;

            this.VerticalScroll.Visible = false;
            this.VerticalScroll.SmallChange = 40;
            this.VerticalScroll.LargeChange = this.Height;
            this.VerticalScroll.Minimum = 0;
            this.VerticalScroll.Maximum = this.Height;
            this.HorizontalScroll.Visible = false;
            this.HorizontalScroll.Enabled = false;

            this.Padding = new Padding(0);

            // Add a local handler for the ControlAdded Event.
            this.ControlAdded += new ControlEventHandler(GroupListControl_ControlAdded);
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            this.Invalidate();
            base.OnScroll(se);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_CLIPCHILDREN
                return cp;
            }
        }

        public ICustomScrollbar VScrollbar
        {
            get { return vScrollbar; }
            set
            {
                if (value != null)
                {
                    UpdateScrollbar();

                    value.ValueChanged += new ScrollValueChangedDelegate(value_ValueChanged);
                }
                vScrollbar = value;
            }   
        }

        public void UpdateScrollbar()
        {
            if (vScrollbar == null)
                return;

            int max, min, pos, smallchange, largechange;
            GetScrollPosition(out min, out max, out pos, out smallchange, out largechange);
            BeginDisableChangeEvents();
            vScrollbar.Maximum = max - largechange;
            vScrollbar.Minimum = min;
            vScrollbar.SmallChange = smallchange;
            vScrollbar.LargeChange = largechange;
            vScrollbar.Value = pos;
            EndDisableChangeEvents();
        }

        public void GetScrollPosition(out int min, out int max, out int pos, out int smallchange, out int largechange)
        {
            SCROLLINFO scrollinfo = new SCROLLINFO();
            scrollinfo.cbSize = (uint)Marshal.SizeOf(typeof(SCROLLINFO));
            scrollinfo.fMask = (int)ScrollInfoMask.SIF_ALL;
            if (GetScrollInfo(this.Handle, (int)SBTYPES.SB_VERT, ref scrollinfo))
            {
                min = scrollinfo.nMin;
                max = scrollinfo.nMax;
                // Check ranges
                if (scrollinfo.nPos > scrollinfo.nMax)
                    pos = scrollinfo.nMax;
                else if (scrollinfo.nPos < scrollinfo.nMin)
                    pos = scrollinfo.nMin;
                else
                    pos = scrollinfo.nPos;               

                smallchange = 1;
                largechange = (int)scrollinfo.nPage;
            }
            else
            {
                min = 0;
                max = 0;
                pos = 0;
                smallchange = 0;
                largechange = 0;
            }
        }

        private void SetScrollPanelMinimumHeight()
        {
            System.Diagnostics.Debug.WriteLine(String.Format("SetScrollPanelMinimumHeight 1: this.height= {0}. ",  this.Height));

            foreach (IGroupView glv in this.Controls)
            {
                if (glv == null)
                    continue;

                // Rought estimation
                scrollPanelMinimumHeight = glv.GetHeaderHeight() * 2 + glv.GetRowHeight() * 3;
                headeHeight = glv.GetHeaderHeight();
                setMinScrollPanelHeight = true;
                break;
            }
            System.Diagnostics.Debug.WriteLine(String.Format("SetScrollPanelMinimumHeight 2: this.height= {0}. ", this.Height));

        }

        private void BeginDisableChangeEvents()
        {
            disableChangeEvents++;
        }

        private void EndDisableChangeEvents()
        {
            if (disableChangeEvents > 0)
                disableChangeEvents--;
        }

        void GroupListControl_ControlAdded(object sender, ControlEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("GroupListControl_ControlAdded."));

            IGroupView glv = (IGroupView)e.Control;
            if (glv == null)
                return;

            glv.Width = this.Width;

            glv.GroupCollapsed += new GroupExpansionHandler(glv_GroupCollapsed);
            glv.GroupExpanded += new GroupExpansionHandler(glv_GroupExpanded);
        }

        public bool SingleItemOnlyExpansion { get; set; }

        void glv_GroupExpanded(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("glv_GroupExpanded called."));

            // Grab a reference to the DragScrollListView3 which sent the message:
            IGroupView expanded = (IGroupView)sender;

            if (expanded == null)
                return;

            // If Single item only expansion, collapse all ListGroups in except
            // the one currently exanding:
            if (this.SingleItemOnlyExpansion)
            {
                this.SuspendLayout();
                foreach (IGroupView glv in this.Controls)
                {
                    if (!glv.Equals(expanded))
                    {
                        glv.Collapse();
                    }
                        
                }
                this.ResumeLayout(true);
            }
            BuildDisplayRows();

            if (AreAllGroupsExpanded())
            {
                if (this.AllGroupsExpanded != null)
                {
                    this.AllGroupsExpanded(this, new EventArgs());
                }
            }
        }

        void glv_GroupCollapsed(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("glv_GroupCollapsed called."));

            BuildDisplayRows();

            if (AreAllGroupsCollapsed())
            {
                if (this.AllGroupsCollapsed != null)
                {
                    this.AllGroupsCollapsed(this, new EventArgs());
                }
            }
        }

        public void ExpandAll()
        {
            System.Diagnostics.Debug.WriteLine(String.Format("ExpandAll called."));

            foreach (IGroupView glv in this.Controls)
            {
                if (glv == null)
                    continue;

                glv.Expand();
                glv.RefreshView();
            }
            BuildDisplayRows();
            this.Refresh();

            if (this.AllGroupsExpanded != null)
            {
                this.AllGroupsExpanded(this, new EventArgs());
            }
        }

        public void CollapseAll()
        {
            System.Diagnostics.Debug.WriteLine(String.Format("CollapseAll called."));

            foreach (IGroupView glv in this.Controls)
            {
                if (glv == null)
                    continue;

                glv.Collapse();
                glv.RefreshView();
            }

            BuildDisplayRows();
            this.Refresh();

            if (this.AllGroupsCollapsed != null)
            {
                this.AllGroupsCollapsed(this, new EventArgs());
            }
        }

        private bool AreAllGroupsCollapsed()
        {
            bool bAllGroupCollapsed = true;
            foreach (IGroupView glv in this.Controls)
            {
                if (glv == null)
                    continue;

                bAllGroupCollapsed &= glv.IsCollapsed();
            }
            return bAllGroupCollapsed;
        }

        private bool AreAllGroupsExpanded()
        {
            bool bAllGroupCollapsed = true;
            foreach (IGroupView glv in this.Controls)
            {
                if (glv == null)
                    continue;

                bAllGroupCollapsed &= glv.IsExpanded();
            }
            return bAllGroupCollapsed;
        }

        public void BuildDisplayRows()
        {
            if (setMinScrollPanelHeight == false)
            {
                SetScrollPanelMinimumHeight();
            }
            lstDisplayRows.Clear();
            dictGroupStartRow.Clear();
            int nVerticalMarginsBetweenControls = DefaultMargin.Top;
            int nRowHeight;
            foreach (IGroupView glv in this.Controls)
            {
                if (glv == null)
                    continue;

                if (!string.IsNullOrEmpty(glv.ID) && !dictGroupStartRow.ContainsKey(glv.ID))
                    dictGroupStartRow.Add(glv.ID, lstDisplayRows.Count);
                
                lstDisplayRows.Add(glv.GetHeaderHeight() + nVerticalMarginsBetweenControls);
                nRowHeight = glv.GetRowHeight();
                for (int i = 0; i < glv.GetListViewVisibleRowsCount(); i++)
                {
                    lstDisplayRows.Add(nRowHeight);
                }
                nVerticalMarginsBetweenControls = DefaultMargin.Bottom + DefaultMargin.Top;
            }
            Rebuild();
        }

        public void Rebuild()
        {
            if (updating > 0)
                return;

            int nTop = 0;
            int nItemNo = topItem;
            int nNewLarge = 0;

            while ((nTop < this.Height) && (nItemNo < lstDisplayRows.Count))
            {
                nTop += lstDisplayRows[nItemNo];
                nNewLarge++;
                nItemNo++;
            }

            this.VerticalScroll.SmallChange = 1;
            this.VerticalScroll.LargeChange = nNewLarge > 0 ? nNewLarge - 1 : 0;
            this.VerticalScroll.Minimum = 0;
             this.VerticalScroll.Maximum = lstDisplayRows.Sum();

            if (vScrollbar != null)
            {
                vScrollbar.LargeChange = nNewLarge > 0 ? nNewLarge - 1 : 0;
                vScrollbar.Maximum = lstDisplayRows.Count - vScrollbar.LargeChange;
            }

            SetScrollPanelHeight();
        }

        protected override Point ScrollToControl(Control activeControl)
        {
            return this.AutoScrollPosition;
        }  

        private void value_ValueChanged(ICustomScrollbar sender, int newValue)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("value_ValueChanged called: newValue = {0}.", newValue));
 

            if (disableChangeEvents > 0)
                return;

            if (vScrollbar == null)
                return;

            SetScrollPosition(vScrollbar.Value);
        }

        public void SetScrollPosition(int pos)
        {
            pos = Math.Min(lstDisplayRows.Count - 1, pos);

            if (pos < 0 || pos >= lstDisplayRows.Count)
                return;

            if (topItem == pos)
                return;

            topItem = pos;

            if ((topItem > -1) && (topItem < lstDisplayRows.Count))
                SetScrollPanelHeight();

            int nValue = GetVerticalPos(pos);

            if (pos == 0)
                nValue = this.VerticalScroll.Minimum;

            this.VerticalScroll.Value = nValue;
            PerformLayout();
            this.Invalidate();
        }

        public void EnsureGroupVisible(string id)
        {
            if (dictGroupStartRow == null || dictGroupStartRow.Count == 0 || vScrollbar == null || string.IsNullOrEmpty(id))
                return;

            if (!dictGroupStartRow.ContainsKey(id))
                return;

            vScrollbar.Value = dictGroupStartRow[id];
        }

        private int GetVerticalPos(int nItem)
        {
            if (nItem <= 0 || lstDisplayRows.Count <= nItem )
                return 0;

            return lstDisplayRows.Take(nItem).Sum(); 
        }

        private void SetScrollPanelHeight()
        {
            int nCount = lstDisplayRows.Count;
            int nY1 = lstDisplayRows.Take(topItem).Sum();
            int nY2 = nY1;
            int nIndex = topItem + displayRows;

            if ((nIndex -1) > nCount)
            {
                 nY2 = lstDisplayRows.Sum();
                 nY2 -= nY1;
                 nY2 += ((nIndex - nCount) * headeHeight);
            }
            else
            {
                nY2 = lstDisplayRows.Take(nIndex).Sum();
                nY2 -= nY1;
            }

            nY2 += DefaultMargin.Bottom;
            if (setMinScrollPanelHeight)
            {
               if (nY2 < scrollPanelMinimumHeight)
                    nY2 = scrollPanelMinimumHeight;
            }
  
            Rectangle rc = this.Bounds;
            this.SetBounds(rc.X, rc.Y, rc.Width, nY2);
        }

        protected override void WndProc(ref Message m)
        {
            ShowScrollBar(this.Handle, (int)SBTYPES.SB_HORZ, false);
            switch (m.Msg)
            {
                case WM_VSCROLL:
                    base.WndProc(ref m);
                    int max, min, pos, smallchange, largechange;
                    GetScrollPosition(out min, out max, out pos, out smallchange, out largechange);

                    if (vScrollbar != null)
                        vScrollbar.Value = pos;

                    break;
                case WM_NCCALCSIZE:
                    int style = (int)GetWindowLong(this.Handle, GWL_STYLE);
                    if ((style & WS_VSCROLL) == WS_VSCROLL)
                        SetWindowLong(this.Handle, GWL_STYLE, style & ~WS_VSCROLL);

                    base.WndProc(ref m);
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
            
        }

        public static int GetWindowLong(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 4)
                return (int)GetWindowLong32(hWnd, nIndex);
            else
                return (int)(long)GetWindowLongPtr64(hWnd, nIndex);
        }

        public static int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong)
        {
            if (IntPtr.Size == 4)
                return (int)SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
            else
                return (int)(long)SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
        }
    }
}
