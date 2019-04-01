//----------------------------------------------------------------------------
// RoboSepGrid
//
// Invetech Pty Ltd
// Victoria, 3149, Australia
// Phone:   (+61) 3 9211 7700
// Fax:     (+61) 3 9211 7701
//
// The copyright to the computer program(s) herein is the property of 
// Invetech Pty Ltd, Australia.
// The program(s) may be used and/or copied only with the written permission 
// of Invetech Pty Ltd or in accordance with the terms and conditions 
// stipulated in the agreement/contract under which the program(s)
// have been supplied.
// 
// Copyright © 2004. All Rights Reserved.
//
//----------------------------------------------------------------------------
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;

namespace Tesla.OperatorConsoleControls
{
	/// <summary>
	/// Summary description for RoboSepGrid.
	/// </summary>
	public class RoboSepGrid : System.Windows.Forms.DataGrid
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#region Construction/destruction

		public RoboSepGrid()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// Register event handlers to assist in hiding scrollbars
			this.VertScrollBar.VisibleChanged += new EventHandler(VertScrollBar_VisibleChanged);
			this.HorizScrollBar.VisibleChanged += new EventHandler(HorizScrollBar_VisibleChanged);

		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#endregion Construction/destruction

		#region RoboSep behaviour 

		#region Events

		public delegate void SelectedRowChangedDelegate(int selectedRowIndex);

		public event SelectedRowChangedDelegate	SelectedRowChanged;

		#endregion Events

		public void Reset()
		{
			ResetSelection();
			myTopRowIndex = 0;
			CurrentRowIndex = myTopRowIndex;
			Refresh();
		}

		#endregion RoboSep behaviour

		#region Mouse-free Scroll support

		int myTopRowIndex;
		int mySelectedRowIndex = -1;		

		public new void SetDataBinding(object dataSource, string dataMember)
		{
			myTopRowIndex = 0;
			base.SetDataBinding(dataSource, dataMember);			
		}

		/// <summary>
		/// Gets the number of rows in the DataSource. This provides a single method
		/// to get the number of rows for either a DataTable or a DataView source.
		/// </summary>
		/// <returns></returns>
		private int GetDataSourceRowCount()
		{
			int rowCount = 0;
			// The DataSource may be either a DataTable or a DataView
			if ((this.DataSource as DataTable) != null)
			{
				rowCount = ((DataTable)this.DataSource).Rows.Count;
			}
			else if ((this.DataSource as DataView) != null)
			{
				rowCount = ((DataView)this.DataSource).Count;
			}
			return rowCount;
		}

		public void ScrollUp()
		{
			// Unselect the currently selected row, if any
			ResetSelection();

            int rowCount = GetDataSourceRowCount();
            if (rowCount > 0)
            {
                int nextRowIndex = 
                    myTopRowIndex < CurrentRowIndex ? myTopRowIndex : myTopRowIndex - 1;
                if (nextRowIndex < 0)
                {
                    nextRowIndex = 0;
                }			
                CurrentRowIndex = myTopRowIndex = nextRowIndex;

                ScrollToRow(CurrentRowIndex);	
            }
		}

		public void ScrollDown()
		{			
			// Unselect the currently selected row, if any
			ResetSelection();

			// When scrolling down, we only need scroll down enough that the last
			// item is visible at the bottom of the client area (not to the index of
			// the last item in the list).
			int rowCount = GetDataSourceRowCount();

			if (this.VisibleRowCount + myTopRowIndex <= rowCount && rowCount > 0)
			{
				int nextRowIndex = myTopRowIndex + 1;
				int maxRowIndex = rowCount - 1;
				if (nextRowIndex >= maxRowIndex)
				{
					nextRowIndex = maxRowIndex;
				}
					
                // Allow for the case where the last row is partially visible.  We
                // allow the user to scroll down once more (to see the last row in 
                // full), but do not adjust the top row index.
                if (this.VisibleRowCount + myTopRowIndex < rowCount)
                {
                    myTopRowIndex = nextRowIndex;
                }
				CurrentRowIndex = nextRowIndex;
		
				ScrollToRow(CurrentRowIndex);
			}
		}		

		private void ScrollToRow(int row)
		{
			if (this.DataSource != null)
			{			
				this.GridVScrolled(this, 
					new ScrollEventArgs(ScrollEventType.LargeIncrement, row));
			}
		}

		/// <summary>
		/// Indicate to the caller whether we can scroll up further or not
		/// </summary>
		/// <returns></returns>
		public bool IsScrollAtTop()
		{
            bool isScrollAtTop = true;
            if (this.DataSource != null)
            {
				int rowCount = GetDataSourceRowCount();
                if (rowCount == 0)
                {
                    // Degenerate case
                    isScrollAtTop = true;
                }
                else
                {
                    isScrollAtTop = (myTopRowIndex == 0 && CurrentRowIndex == 0);
                }                            
            }            
            return isScrollAtTop;
		}

		/// <summary>
		/// Indicate to the caller whether we can scroll down further or not
		/// </summary>
		/// <returns></returns>
		public bool IsScrollAtBottom()
		{
			return (myTopRowIndex < CurrentRowIndex)    // Detect if at the bottom of a list larger than the grid height
                || 
                (myTopRowIndex == 0 && ( ! this.VertScrollBar.Visible));    // Detect if the full list falls within the grid height
		}

		private void VertScrollBar_VisibleChanged(object sender, EventArgs e)
		{
			// Ensure vertical scrollbar is always hidden
			SuspendLayout();
			VertScrollBar.Height = 0;
			VertScrollBar.Width = 0;
			ResumeLayout(true);
		}

		private void HorizScrollBar_VisibleChanged(object sender, EventArgs e)
		{
			// Ensure horizontal scrollbar is always hidden
			SuspendLayout();
			HorizScrollBar.Height = 0;
			VertScrollBar.Width = 0;
			ResumeLayout(true);
		}

		#endregion Mouse-free Scroll support

		#region DataGrid overrides

		protected override void OnMouseDown(MouseEventArgs e)
		{			
			//Determine the row that was clicked
			DataGrid.HitTestInfo hitInfo = this.HitTest(e.X, e.Y);
			
            if (hitInfo.Type == DataGrid.HitTestType.ColumnHeader)
            {
                if (this.AllowSorting)
                {
                    base.OnMouseDown(e);
                }
            }
			else if (hitInfo.Type == DataGrid.HitTestType.Cell && hitInfo.Row != -1)
			{
				ResetSelection();
				Select(hitInfo.Row);
				mySelectedRowIndex = hitInfo.Row;
                // Notify interested parties of the changed selection
				if (SelectedRowChanged != null)
				{					
					SelectedRowChanged(mySelectedRowIndex);
				}
			}
		}

		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			// Draw the background.  Do NOT call the base class method as otherwise we can get 
			// residual "transparent" effects around the grid border.
			using (SolidBrush backgroundBrush = new SolidBrush(this.BackColor))
			{
				pevent.Graphics.FillRectangle(backgroundBrush, pevent.ClipRectangle);
			}
		}

		#endregion DataGrid overrides

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion

		#region HeaderHeight

		private Int32 columnHeaderHeight = 13;

		[Browsable(true), Description("Sets the column header height in pixels"), Category("Layout"), DefaultValue(13)]
		public Int32 ColumnHeaderHeight
		{
			get
			{
				return columnHeaderHeight;
			}
			set
			{
				columnHeaderHeight = value;
				OnLayout(null);
			}
		}
 
		protected override void OnLayout(LayoutEventArgs levent)
		{
			setColumnHeaderSize();
			base.OnLayout (levent);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			setColumnHeaderSize();
			base.OnPaint (e);
		}


		private void setColumnHeaderSize()
		{
			try
			{

				if (null != this)
				{
					
					//
					// Use reflection to get the private class property 'headerFontHeight'
					//
					DataGrid myClass = new DataGrid();

					FieldInfo headerFontHeightFieldInfo = myClass.GetType().GetField("headerFontHeight", BindingFlags.NonPublic|BindingFlags.Instance);

					if (null != headerFontHeightFieldInfo)
					{
						Int32 currentHeight = (Int32)headerFontHeightFieldInfo.GetValue(this);

						//
						// Use public property to redefine 'headerFontHeight'
						//
						if (ColumnHeaderHeight >= 0)
						{
							headerFontHeightFieldInfo.SetValue(this, ColumnHeaderHeight);
						}
					}

				}

			}

			catch (Exception e)
			{

				System.Diagnostics.Debug.Assert(false, e.ToString());

				throw e;

			}
		}

		#endregion HeaderHeight
		
	}
}
