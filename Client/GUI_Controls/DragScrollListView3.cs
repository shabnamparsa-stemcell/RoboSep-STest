using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms;



namespace GUI_Controls
{
    public partial class DragScrollListView3 : DragScrollListView
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

        //fnBar values
        private enum SBTYPES
        {
            SB_HORZ = 0,
            SB_VERT = 1,
            SB_CTL = 2,
            SB_BOTH = 3
        }
        //lpsi values
        private enum LPCSCROLLINFO
        {
            SIF_RANGE = 0x0001,
            SIF_PAGE = 0x0002,
            SIF_POS = 0x0004,
            SIF_DISABLENOSCROLL = 0x0008,
            SIF_TRACKPOS = 0x0010,
            SIF_ALL = (SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS)
        }

        private ICustomScrollbar vScrollbar = null;

        private int disableChangeEvents = 0;
        public delegate void ScrollPositionChangedDelegate(GUI_Controls.DragScrollListView listview, int pos);
        public event ScrollPositionChangedDelegate ScrollPositionChanged;

        const int GWL_STYLE = -16;
        const int WS_VSCROLL = 0x00200000;

        private const UInt32 WM_VSCROLL = 0x0115;
        private const UInt32 WM_NCCALCSIZE = 0x83;
   

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

        public DragScrollListView3()
        {
            InitializeComponent();
        }

        void DragScrollListView3_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateScrollbar();
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
            vScrollbar.Maximum = max > largechange ? (max - largechange + 1) : max;
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

        private void BeginDisableChangeEvents()
        {
            disableChangeEvents++;
        }

        private void EndDisableChangeEvents()
        {
            if (disableChangeEvents > 0)
                disableChangeEvents--;
        }

        private void value_ValueChanged(ICustomScrollbar sender, int newValue)
        {
            if (disableChangeEvents > 0)
                return;

            if (vScrollbar == null)
                return;

            SetScrollPosition(vScrollbar.Value);
        }

        public void SetScrollPosition(int pos)
        {
            pos = Math.Min(Items.Count - 1, pos);

            if (pos < 0 || pos >= Items.Count)
                return;

            SuspendLayout();
            EnsureVisible(pos);

            for (int i = 0; i < 10; i++)
            {
                if (TopItem != null && TopItem.Index != pos)
                {
                    TopItem = Items[pos];
                }
            }
            ResumeLayout();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_VSCROLL)
            {
                base.WndProc(ref m);
                int max, min, pos, smallchange, largechange;
                GetScrollPosition(out min, out max, out pos, out smallchange, out largechange);

                if (ScrollPositionChanged != null)
                    ScrollPositionChanged(this, pos);

                if (vScrollbar != null)
                {
                    vScrollbar.Value = pos;
                }
            }
            else if (m.Msg == WM_NCCALCSIZE) // WM_NCCALCSIZE
            {
                int style = (int)GetWindowLong(this.Handle, GWL_STYLE);
                if ((style & WS_VSCROLL) == WS_VSCROLL)
                    SetWindowLong(this.Handle, GWL_STYLE, style & ~WS_VSCROLL);
                base.WndProc(ref m);

            }
            else
            {
                base.WndProc(ref m);
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
