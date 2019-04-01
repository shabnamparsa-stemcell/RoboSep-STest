using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace GUI_Controls
{
	/// <summary>
	/// ListView with embedded control
	/// </summary>
	public class ListViewEmbedCtrl : ListView
	{
		#region Interop-Defines
		[DllImport("user32.dll")]
		private	static extern IntPtr SendMessage(IntPtr hWnd, int msg,	IntPtr wPar, IntPtr	lPar);
        
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern IntPtr SendMessage(IntPtr hwnd, long wMsg, long wParam, long lParam);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(HandleRef hwnd, out RECT lpRect);
   

    const int GWL_STYLE = -16;
    const int WS_VSCROLL = 0x00200000;
    const int WS_HSCROLL = 0x00100000;

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
        public struct RECT 
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

         const long LVM_GETHEADER = (LVM_FIRST + 31);
 
        #endregion

        
        /// <summary>
		/// Structure to hold an embedded control's info
		/// </summary>
		private struct EmbeddedControl
		{
			public Control Control;
			public int Column;
			public int Row;
			public DockStyle Dock;
			public ListViewItem Item;
		}

		private ArrayList _embeddedControls = new ArrayList();
        private uint _visibleRow = 4;

        public ListViewEmbedCtrl() { }

        public ListViewEmbedCtrl(uint VisibleRow)
        {
            this._visibleRow = VisibleRow;
        }

		/// <summary>
		/// Retrieve the order in which columns appear
		/// </summary>
		/// <returns>Current display order of column indices</returns>
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

		/// <summary>
		/// Retrieve the bounds of a ListViewSubItem
		/// </summary>
		/// <param name="Item">The Item containing the SubItem</param>
		/// <param name="SubItem">Index of the SubItem</param>
		/// <returns>Subitem's bounds</returns>
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

		/// <summary>
		/// Add a control to the ListView
		/// </summary>
		/// <param name="c">Control to be added</param>
		/// <param name="col">Index of column</param>
		/// <param name="row">Index of row</param>
		public void AddEmbeddedControl(Control c, int col, int row)
		{
			AddEmbeddedControl(c,col,row,DockStyle.Fill);
		}
		/// <summary>
		/// Add a control to the ListView
		/// </summary>
		/// <param name="c">Control to be added</param>
		/// <param name="col">Index of column</param>
		/// <param name="row">Index of row</param>
		/// <param name="dock">Location and resize behavior of embedded control</param>
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
		
		/// <summary>
		/// Remove a control from the ListView
		/// </summary>
		/// <param name="c">Control to be removed</param>
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
		
		/// <summary>
		/// Retrieve the control embedded at a given location
		/// </summary>
		/// <param name="col">Index of Column</param>
		/// <param name="row">Index of Row</param>
		/// <returns>Control found at given location or null if none assigned.</returns>
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

        public void SizeLastColumn()
        {
            int nClientWidth = ClientRectangle.Width;
          
            int nColWidth = 0;

            for (int i = 0; i < Columns.Count; i++)
            {
                
                if ((i == Columns.Count - 1) && (nColWidth < nClientWidth ))
                {
                    int nLastColWidth = nClientWidth - nColWidth;
                    if (nLastColWidth != Columns[i].Width)
                    {
                        //Columns[i].Width = nLastColWidth;

                        Columns[i].Width =-2;


                    }
                }
                else
                {
                    nColWidth += Columns[i].Width;
                }
            }
        }

        public void UpdateRow()
        {
            int nhs = SystemInformation.HorizontalScrollBarHeight;
            int nvh = SystemInformation.VerticalScrollBarArrowHeight;
            int nch = SystemInformation.VerticalScrollBarWidth;

            RECT rch = new RECT();
            IntPtr hwnd = SendMessage((this.Handle), LVM_GETHEADER, 0, 0);
            if (hwnd != null)
            {
                    if (GetWindowRect(new HandleRef(null, hwnd), out rch))
                    {
                        int headerHeight = rch.Bottom - rch.Top;
                    }
            }

          Rectangle t = ClientRectangle;
          Size y = this.Size; 

            Size s = ClientSize;
            Rectangle rc = ClientRectangle;
            Rectangle bound = this.Bounds;
            int height = Height;
            int width = this.Width;
            Size si =  this.Size;
            ListViewItem x = this.TopItem;
            
            int itemHeight = x.GetBounds(ItemBoundsPortion.Entire).Height;
            int itemHeight2 = GetItemRect(0).Height;


            Rectangle r1 = x.GetBounds(ItemBoundsPortion.Entire );
            Rectangle r2 = x.GetBounds(ItemBoundsPortion.Icon);
            Rectangle r3 = x.GetBounds(ItemBoundsPortion.ItemOnly);
            Rectangle r4 = x.GetBounds(ItemBoundsPortion.Label);
 
        

            for (int i = 0; i < 6; i++)
            {
                //ListViewItem item 
                 Rectangle rcItem = GetItemRect(i);
                 ListViewItem k = GetItemAt(rcItem.X, rcItem.Y);
             //    Rectangle rcItem2 = k.GetBounds(ItemBoundsPortion.Entire);
    		
            }
            // Calculate the position of all embedded controls
            foreach (EmbeddedControl ec in _embeddedControls)
            {
                Rectangle rcTemp = this.GetSubItemBounds(ec.Item, ec.Column);

            }

            this.Scrollable = true;
            t = this.ClientRectangle;
            y = this.Size;


            this.Scrollable = false;
            t = this.ClientRectangle;
            y = this.Size;

 



            this.ClientSize = new Size( t.Width, t.Height + 16);

            y = this.Size;


    }

		protected override void WndProc(ref Message m)
		{
			switch (m.Msg)
			{
				case WM_PAINT:

					if (View != View.Details)
						break;

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
