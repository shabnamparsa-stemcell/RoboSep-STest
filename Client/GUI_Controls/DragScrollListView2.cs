using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;




namespace GUI_Controls
{
    public partial class DragScrollListView2 : DragScrollListView
    {
		#region Interop-Defines

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

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetScrollInfo(IntPtr hwnd, int fnBar, ref SCROLLINFO lpsi);

		[DllImport("user32.dll")]
		private	static extern IntPtr SendMessage(IntPtr hWnd, int msg,	IntPtr wPar, IntPtr	lPar);
        
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern IntPtr SendMessage(IntPtr hwnd, long wMsg, long wParam, long lParam);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(HandleRef hwnd, out RECT lpRect);
   
        const int GWL_STYLE = -16;
        const int WS_VSCROLL = 0x00200000;
        const int WS_HSCROLL = 0x00100000;

        private const int WM_VSCROLL = 0x0115;
        private const int WM_NCCALCSIZE = 0x83;

        public static int GetWindowLong(IntPtr hWnd, int nIndex) 
        {
            if (IntPtr.Size == 4)
                return (int)GetWindowLong32(hWnd, nIndex);
            else
                return (int)(long)GetWindowLongPtr64(hWnd, nIndex);
        }

        public static int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong) {
            if (IntPtr.Size == 4)
                return (int)SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
            else
                return (int)(long)SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, int dwNewLong);


		// ListView messages
		private const int LVM_FIRST					= 0x1000;
		private const int LVM_GETCOLUMNORDERARRAY	= (LVM_FIRST + 59);
		
		// Windows Messages
		private const int WM_PAINT = 0x000F;
	
        [Serializable, StructLayout(LayoutKind.Sequential)]
        public new struct RECT 
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

         const long LVM_GETHEADER = (LVM_FIRST + 31);
 
        #endregion

		// Structure to hold an embedded control's info
		private struct EmbeddedControl
		{
			public Control Control;
			public int Column;
			public int Row;
			public DockStyle Dock;
			public ListViewItem Item;
		}


        private ICustomScrollbar vScrollbar = null;
        private int disableChangeEvents = 0;
        public delegate void ScrollPositionChangedDelegate(GUI_Controls.DragScrollListView listview, int pos);
        public event ScrollPositionChangedDelegate ScrollPositionChanged;
		private ArrayList _embeddedControls = new ArrayList();
        private uint _visibleRow = 4;

        public DragScrollListView2() { }

        public DragScrollListView2(uint VisibleRow)
        {
            this._visibleRow = VisibleRow;
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
            vScrollbar.Maximum = max > largechange? (max - largechange + 1) : max;
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

		// Retrieve the order in which columns appear
		// <returns> Current display order of column indices
		protected int[] GetColumnOrder()
		{
			IntPtr lPar	= Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)) * Columns.Count);

			IntPtr res = SendMessage(Handle, LVM_GETCOLUMNORDERARRAY, new IntPtr(Columns.Count), lPar);
			if (res.ToInt32() == 0)	// Something went wrong
			{
				Marshal.FreeHGlobal(lPar);
				return null;
			}

			int	[] order = new int[Columns.Count];
			Marshal.Copy(lPar, order, 0, Columns.Count);

			Marshal.FreeHGlobal(lPar);
			return order;
		}

	
		// Retrieve the bounds of a ListViewSubItem
		protected Rectangle GetSubItemBounds(ListViewItem Item, int SubItem)
		{
			Rectangle subItemRect = Rectangle.Empty;

			if (Item == null)
				throw new ArgumentNullException("Item");

			int[] order = GetColumnOrder();
			if (order == null) // No Columns
				return subItemRect;

			if (SubItem >= order.Length)
				throw new IndexOutOfRangeException("SubItem "+SubItem+" out of range");

			// Retrieve the bounds of the entire ListViewItem (all subitems)
			Rectangle lviBounds = Item.GetBounds(ItemBoundsPortion.Entire);
			int	subItemX = lviBounds.Left;

			// Calculate the X position of the SubItem.
			// Because the columns can be reordered we have to use Columns[order[i]] instead of Columns[i] !
			ColumnHeader col;
			int i;
			for (i=0; i<order.Length; i++)
			{
				col = this.Columns[order[i]];
 				if (col.Index == SubItem)
					break;
				subItemX += col.Width;
			}
 
			subItemRect	= new Rectangle(subItemX, lviBounds.Top, this.Columns[order[i]].Width, lviBounds.Height);

			return subItemRect;
		}


		// Add a control to the ListView
		public void AddEmbeddedControl(Control c, int col, int row)
		{
			AddEmbeddedControl(c,col,row,DockStyle.Fill);
		}

		// Add a control to the ListView
		public void AddEmbeddedControl(Control c, int col, int row, DockStyle dock)
		{
			if (c==null)
				throw new ArgumentNullException();
			if (col>=Columns.Count || row>=Items.Count)
				throw new ArgumentOutOfRangeException();

			EmbeddedControl ec;
			ec.Control = c;
			ec.Column = col;
			ec.Row = row;
			ec.Dock = dock;
			ec.Item = Items[row];

			_embeddedControls.Add(ec);

			// Add a Click event handler to select the ListView row when an embedded control is clicked
			c.Click += new EventHandler(_embeddedControl_Click);
			
			this.Controls.Add(c);
		}
		
		// Remove a control from the ListView
		public void RemoveEmbeddedControl(Control c)
		{
			if (c == null)
				throw new ArgumentNullException();

			for (int i=0; i<_embeddedControls.Count; i++)
			{
				EmbeddedControl ec = (EmbeddedControl)_embeddedControls[i];
				if (ec.Control == c)
				{
					c.Click -= new EventHandler(_embeddedControl_Click);
					this.Controls.Remove(c);
					_embeddedControls.RemoveAt(i);
					return;
				}
			}
			throw new Exception("Control not found!");
		}
		

		// Retrieve the control embedded at a given location
		public Control GetEmbeddedControl(int col, int row)
		{
			foreach (EmbeddedControl ec in _embeddedControls)
				if (ec.Row == row && ec.Column == col)
					return ec.Control;

			return null;
		}

		[DefaultValue(View.LargeIcon)]
		public new View View
		{
			get 
			{
				return base.View;
			}
			set
			{
				// Embedded controls are rendered only when we're in Details mode
				foreach (EmbeddedControl ec in _embeddedControls)
					ec.Control.Visible = (value == View.Details);

				base.View = value;
			}
		}

        public void ResizeLastColumn()
        {
            int nClientWidth = ClientRectangle.Width;
            int nColWidth = 0;

            for (int i = 0; i < Columns.Count; i++)
            {
                // last column cannnot be resized since the sum of the column widths before the last 
                // column already has exceeded the client width
                if ((nColWidth + Columns[i].Width)> nClientWidth && i < (Columns.Count - 1))
                    break;

                if ((i == Columns.Count - 1))
                {
                    int nLastColumnWidth = Columns[i].Width;
                    AutoResizeColumn(Columns.Count - 1, ColumnHeaderAutoResizeStyle.ColumnContent);
                    if (Columns[i].Width <= nLastColumnWidth || Columns[i].Width <= (nClientWidth - nColWidth))
                    {
                        Columns[i].Width = nClientWidth - nColWidth;
                        break;
                    }
                }
                else
                {
                    nColWidth += Columns[i].Width;
                }
            }

          //  AutoResizeColumn(Columns.Count - 1, ColumnHeaderAutoResizeStyle.ColumnContent);

           // Columns[Columns.Count - 1].Width = -2;
        }

		protected override void WndProc(ref Message m)
		{
			switch (m.Msg)
			{
                case WM_VSCROLL:
                    
                    base.WndProc(ref m);

                    int max, min, pos, smallchange, largechange;
                    GetScrollPosition(out min, out max, out pos, out smallchange, out largechange);

                    if (ScrollPositionChanged != null)
                        ScrollPositionChanged(this, pos);

                    if (vScrollbar != null)
                        vScrollbar.Value = pos;

                     break;
  
				case WM_PAINT:
					if (View == View.Details)
                    {
					    // Calculate the position of all embedded controls
					    foreach (EmbeddedControl ec in _embeddedControls)
					    {
						    Rectangle rc = this.GetSubItemBounds(ec.Item, ec.Column);
                           
						    if ((this.HeaderStyle != ColumnHeaderStyle.None) &&
							    (rc.Top<this.Font.Height)) // Control overlaps ColumnHeader
						    {
							    ec.Control.Visible = false;
							    continue;
						    }
						    else
						    {
							    ec.Control.Visible = true;
						    }
                            
                            ec.Control.Visible = true;

						    switch (ec.Dock)
						    {
							    case DockStyle.Fill:
								    break;
							    case DockStyle.Top:
								    rc.Height = ec.Control.Height;
								    break;
							    case DockStyle.Left:
								    rc.Width = ec.Control.Width;
								    break;
							    case DockStyle.Bottom:
								    rc.Offset(0, rc.Height-ec.Control.Height);
								    rc.Height = ec.Control.Height;
								    break;
							    case DockStyle.Right:
								    rc.Offset(rc.Width-ec.Control.Width, 0);
								    rc.Width = ec.Control.Width;
								    break;
							    case DockStyle.None:
								    rc.Size = ec.Control.Size;
								    break;
						    }

						    // Set embedded control's bounds
						    ec.Control.Bounds = rc;
					    }
                    }
                    base.WndProc(ref m);
					break;

                case 0x83: // WM_NCCALCSIZE
                    int style = (int)GetWindowLong(this.Handle, GWL_STYLE);
                    if ((style & WS_VSCROLL) == WS_VSCROLL)
                    {   
                        style &= ~WS_VSCROLL;
                        SetWindowLong(this.Handle, GWL_STYLE, style);
                    }

                    if ((style & WS_HSCROLL) == WS_HSCROLL)
                    {   
                        style &= ~WS_HSCROLL;
                        SetWindowLong(this.Handle, GWL_STYLE, style);
                    }
                    base.WndProc(ref m);
                    break;

                default:
                    base.WndProc(ref m);
                    break;
			}
		}

		private void _embeddedControl_Click(object sender, EventArgs e)
		{
			// When a control is clicked the ListViewItem holding it is selected
			foreach (EmbeddedControl ec in _embeddedControls)
			{
				if (ec.Control == (Control)sender)
				{
					this.SelectedItems.Clear();
					ec.Item.Selected = true;
				}
			}
		}

    }
}
