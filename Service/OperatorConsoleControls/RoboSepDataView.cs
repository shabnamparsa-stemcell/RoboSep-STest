//----------------------------------------------------------------------------
// RoboSepDataView
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
using System.Data;
using System.ComponentModel;
using System.Windows.Forms;

namespace Tesla.OperatorConsoleControls
{
	/// <summary>
	/// Helper class to ensure that all updates are performed from the GUI thread.
	/// </summary>
	public class RoboSepDataView : DataView
	{
		DataGrid myDataGrid;

		public RoboSepDataView(DataGrid dataGrid, DataTable dataTable)				
			: base(dataTable)
		{
			myDataGrid = dataGrid;
		}

		private delegate void OnListChangedDelegate(ListChangedEventArgs e);

		protected override void OnListChanged(ListChangedEventArgs e)
		{
			if (myDataGrid != null && myDataGrid.InvokeRequired)
			{
				myDataGrid.Invoke(new OnListChangedDelegate(OnListChanged), 
					new object[]{e});
			}
			else
			{
				base.OnListChanged(e);
			}
		}

		private delegate void IndexListChangedDelegate(object sender,
			ListChangedEventArgs e);

		protected override void IndexListChanged(object sender, ListChangedEventArgs e)
		{
			if (myDataGrid != null && myDataGrid.InvokeRequired)
			{
				myDataGrid.Invoke(new IndexListChangedDelegate(IndexListChanged),
					new object[]{sender, e});
			}
			else
			{
				base.IndexListChanged(sender, e);
			}
		}
	}
}
