//----------------------------------------------------------------------------
// RoboSepItemList
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
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;

using Tesla.Common.ResourceManagement;

using Tesla.Common.DrawingUtilities;

namespace Tesla.OperatorConsoleControls
{
	/// <summary>
	/// Summary description for RoboSepItemList.
	/// </summary>
	public class RoboSepItemList : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Panel pnlItemList;
		private System.Windows.Forms.Panel pnlItemListContainer;
		private System.Windows.Forms.Panel pnlScrollControl;
		private System.Windows.Forms.Button btnUp;
		private System.Windows.Forms.Button btnDown;

		private bool isMultiSelect =false;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#region Construction/destruction

		public RoboSepItemList()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// Configure the item list for scrolling
			pnlItemList.AutoScroll = true;
			OversizeListItemArea();
			btnUp.Visible = false;
			btnDown.Visible = false;			
			pnlScrollControl.Visible = false;

			// Configure colour scheme
			Color listItemColour = ColourScheme.GetColour(ColourSchemeItem.MaintenanceListItemBackground);
			btnUp.BackColor = listItemColour;
			btnDown.BackColor = listItemColour;
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

		#region Events

		public delegate void InvokeItemActionDelegate(object actionContext);

		public event InvokeItemActionDelegate	InvokeItemAction;

		internal void PerformItemAction(object actionContext)
		{
			if (InvokeItemAction != null)
			{
				InvokeItemAction(actionContext);
			}
		}

		#endregion Events

		#region Item List behaviour

		public int ItemCount
		{
			get
			{
				return pnlItemList.Controls.Count;
			}
		}

		private string myEmptyListMessage;

		public string EmptyListMessage
		{
			set
			{
				myEmptyListMessage = value;
			}
		}

		private int	myTopItemIndex;

		public void AddRange(RoboSepListItem[] range)
		{			
			pnlItemList.Controls.AddRange(range);			
			for (int i = 0; i < pnlItemList.Controls.Count; ++i)
			{
                pnlItemList.Controls[i].BackColor = this.BackColor;
				pnlItemList.Controls[i].Width = this.Width-2; // bdr
				if (i > 0)
				{
					pnlItemList.Controls[i].Location = new Point(0, 
						pnlItemList.Controls[i-1].Location.Y + 
						pnlItemList.Controls[i-1].Height);
				}				
			}
			RecalculateListItemsHeight();
			RecalculateScrollState();
		}

		public void Clear()
		{
			// If we are scrolled down when the list is cleared, we can enter an unsafe state
			while( !IsScrollAtTop() )
			{
				ScrollUpList();
			}
			pnlItemList.Controls.Clear();
			RecalculateListItemsHeight();
			RecalculateScrollState();
		}		

		public void SelectItemByTag(object selectionTag)
		{
			int listItemCount = pnlItemList.Controls.Count;
			if (listItemCount > 0)
			{
				// Select the given item and deselect any previous selection 
				// (multiple selection not allowed/implemented).
				for (int i = 0; i < listItemCount; ++i)
				{
					RoboSepListItem listItem = ((RoboSepListItem)pnlItemList.Controls[i]);
					object listItemTag = listItem.Tag;
					if(isMultiSelect)
					{
						if(object.Equals(listItemTag, selectionTag) && !listItem.IsSelectedAlwaysItem )
						{
							listItem.IsSelectedItem = !listItem.IsSelectedItem;
							listItem.IsFlashingItem = false;
						}
					}
					else
					{
						listItem.IsSelectedItem = object.Equals(listItemTag, selectionTag);	
					}
				}
			}
		}

		public bool ContainsItemByTag(object tag)
		{
			bool isContain =false;
			int listItemCount = pnlItemList.Controls.Count;
			if (listItemCount > 0)
			{
				for (int i = 0; i < listItemCount; ++i)
				{
					RoboSepListItem listItem = ((RoboSepListItem)pnlItemList.Controls[i]);
					object listItemTag = listItem.Tag;
					if(object.Equals(listItemTag,tag))
					{
							isContain =true;
					}
				}
			}
			return isContain;
		}


		Point myScrollToPosition = new Point();

		public void ScrollDownList()
		{
			if (myTopItemIndex < pnlItemList.Controls.Count-1 && ( ! IsScrollAtBottom()))
			{
				myScrollToPosition.Y = 
					pnlItemList.Location.Y - 
					pnlItemList.Controls[++myTopItemIndex].Height;

				int largestYOffset = pnlItemListContainer.Height - pnlItemList.Height;	
				if (myScrollToPosition.Y < largestYOffset)	// 'Y Offsets' are negative
				{
					myScrollToPosition.Y = largestYOffset;
				}				
				pnlItemList.Location = myScrollToPosition;				
				RecalculateScrollState();
				pnlItemList.Refresh();				
			}
		}

		public void ScrollUpList()
		{
			if (myTopItemIndex > 0 && ( ! IsScrollAtTop()))
			{
				// Normal item scrolling behaviour is to show the upper edge of the
				// topmost visible (unless we're at the bottom of the list), clipping the 
				// item at the bottom of the list if necessary.  Therefore, calculate
				// the 'scroll to' position as the top edge of the relevant item.
				--myTopItemIndex;
				int yOffset = 0;
				for (int i = 0; i < myTopItemIndex; ++i)
				{
					// Note: 'ScrollTo' Y offsets are negative
					yOffset -= pnlItemList.Controls[i].Height;
				}
				myScrollToPosition.Y = yOffset;					
				if (myScrollToPosition.Y > 0)	// 'Y Offsets' are negative
				{
					myScrollToPosition.Y = 0;
				}
				pnlItemList.Location = myScrollToPosition;
				RecalculateScrollState();
				pnlItemList.Refresh();
			}
		}

		public bool IsScrollAtBottom()
		{			
			return pnlItemList.Controls.Count == 0 ||
				myScrollToPosition.Y <= pnlItemListContainer.Height - pnlItemList.Height;
		}

		public bool IsScrollAtTop()
		{			
			return pnlItemList.Controls.Count == 0 ||
				myScrollToPosition.Y >= 0;
		}

		bool isAtTop = true;
		bool isAtBottom = true;

		private void RecalculateScrollState()
		{
			bool newIsScrollAtTop = IsScrollAtTop();
			bool newIsScrollAtBottom = IsScrollAtBottom();

			if (newIsScrollAtTop != isAtTop || newIsScrollAtBottom != isAtBottom)
			{
				isAtTop = newIsScrollAtTop;
				isAtBottom = newIsScrollAtBottom;				

				bool isScrollingRequired = ! (isAtTop && isAtBottom);
				btnUp.Visible = isScrollingRequired && ( ! isAtTop);
				btnDown.Visible = isScrollingRequired && ( ! isAtBottom);
				pnlScrollControl.Visible = isScrollingRequired;
			}			
		}

		GraphicsPath myBoundingPath = new GraphicsPath();
		RectangleF   myBoundingRectangleF;

		private void RoboSepItemList_Resize(object sender, System.EventArgs e)
		{
			// Set the region according to whether or not scrolling is required
			myBoundingRectangleF = new RectangleF(
				pnlItemListContainer.Location.X, pnlItemListContainer.Location.Y,
				pnlItemListContainer.Size.Width, pnlItemListContainer.Size.Height);
			
			Region panelBoundary = new Region(myBoundingRectangleF);
			panelBoundary.Union(pnlScrollControl.Bounds);
			this.Region = panelBoundary;

			// Resize the item list (to avoid seeing scrollbars on the item list panel)
			pnlItemList.SuspendLayout();
			OversizeListItemArea();				
			for (int i = 0; i < pnlItemList.Controls.Count; ++i)
			{
				pnlItemList.Controls[i].Width = this.Width-2;					
			}				
			pnlItemList.ResumeLayout(false);
			RecalculateListItemsHeight();
			RecalculateScrollState();
		}

		private void OversizeListItemArea()
		{
			pnlItemListContainer.Width = this.Width + 2 * DrawingUtilities.MinimumTabMargin;
			pnlItemList.Width = pnlItemListContainer.Width;
		}

		private void RecalculateListItemsHeight()
		{
			int listHeight = 0;
			for (int i = 0; i < pnlItemList.Controls.Count; ++i)
			{
				listHeight += pnlItemList.Controls[i].Height;
			}
			pnlItemList.Height = listHeight;
		}

		private void btnUp_Click(object sender, System.EventArgs e)
		{
			ScrollUpList();
		}

		private void btnDown_Click(object sender, System.EventArgs e)
		{
			ScrollDownList();
		}

		public bool IsMultiSelect
		{
			get
			{
				return isMultiSelect;
			}
			set
			{
				isMultiSelect = value ;
			}
		}

		#endregion Item List behaviour

		#region Item List painting

		private void pnlItemListContainer_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			if (pnlItemList.Controls.Count <= 0 && 
				myEmptyListMessage != null && myEmptyListMessage != string.Empty)
			{
				// Apply standard Graphics settings
				DrawingUtilities.ApplyStandardContext(e.Graphics);

				// Get a brush for drawing text
				SolidBrush textBrush = new SolidBrush(
					ColourScheme.GetColour(ColourSchemeItem.TabTextForeground));

				// Get a string format for left-aligned text, vertically aligned at top
				StringFormat stringFormat = DrawingUtilities.NearNearStringFormat;           			

				// Display a note indicating that no resources are required
				string txtMsg = myEmptyListMessage;	
				Rectangle txtRectangle = pnlItemListContainer.Bounds;
				txtRectangle.Width = this.Width;

#if (TESLA_UI_SHOW_BOUNDING_RECTANGLES)
				e.Graphics.DrawRectangle(Pens.CornflowerBlue, txtRectangle);			
#endif
					
				// Add the Name string to the display
				e.Graphics.DrawString(txtMsg,
					SeparatorResourceManager.GetFont(SeparatorResourceManager.FontId.MaintenanceListItem), 
					textBrush,
					txtRectangle,
					stringFormat);

				// Clean up graphics resources
				textBrush.Dispose();
			}
		}

		#endregion Item List painting

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.pnlItemList = new System.Windows.Forms.Panel();
			this.pnlItemListContainer = new System.Windows.Forms.Panel();
			this.pnlScrollControl = new System.Windows.Forms.Panel();
			this.btnUp = new System.Windows.Forms.Button();
			this.btnDown = new System.Windows.Forms.Button();
			this.pnlItemListContainer.SuspendLayout();
			this.pnlScrollControl.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlItemList
			// 
			this.pnlItemList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.pnlItemList.Location = new System.Drawing.Point(2, 3);
			this.pnlItemList.Name = "pnlItemList";
		  //this.pnlItemList.Size = new System.Drawing.Size(272, 232); - bdr1
			this.pnlItemList.Size = new System.Drawing.Size(250, 334);
			this.pnlItemList.TabIndex = 2;
			this.pnlItemList.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlItemList_Paint);
			// 
			// pnlItemListContainer
			// 
			this.pnlItemListContainer.Controls.Add(this.pnlItemList);
			this.pnlItemListContainer.Location = new System.Drawing.Point(0, 30);
			this.pnlItemListContainer.Name = "pnlItemListContainer";
		  //this.pnlItemListContainer.Size = new System.Drawing.Size(256, 232); - bdr
			this.pnlItemListContainer.Size = new System.Drawing.Size(246, 340);
			this.pnlItemListContainer.TabIndex = 3;
			this.pnlItemListContainer.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlItemListContainer_Paint);
			// 
			// pnlScrollControl
			// 
			this.pnlScrollControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
			this.pnlScrollControl.Controls.Add(this.btnUp);
			this.pnlScrollControl.Controls.Add(this.btnDown);
		  //this.pnlScrollControl.Location = new System.Drawing.Point(120, 0); - bdr
			this.pnlScrollControl.Location = new System.Drawing.Point(76, 0);
			this.pnlScrollControl.Name = "pnlScrollControl";
		  //this.pnlScrollControl.Size = new System.Drawing.Size(120, 32); - bdr
			this.pnlScrollControl.Size = new System.Drawing.Size(108, 402);
			this.pnlScrollControl.TabIndex = 4;
			this.pnlScrollControl.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlScrollControl_Paint);
			// 
			// btnUp
			// 
			this.btnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnUp.Font = new System.Drawing.Font("Webdings", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(2)));
		  //this.btnUp.Location = new System.Drawing.Point(8, 8); - bdr
			this.btnUp.Location = new System.Drawing.Point(0, 0);
			this.btnUp.Name = "btnUp";
		  //this.btnUp.Size = new System.Drawing.Size(48, 31); - bdr
			this.btnUp.Size = new System.Drawing.Size(108, 28);
			this.btnUp.TabIndex = 7;
			this.btnUp.Text = "5";
			this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
			// 
			// btnDown
			// 
			this.btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDown.Font = new System.Drawing.Font("Webdings", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(2)));
		  //this.btnDown.Location = new System.Drawing.Point(64, 8); - bdr
			this.btnDown.Location = new System.Drawing.Point(0, 374);
			this.btnDown.Name = "btnDown";
		  //this.btnDown.Size = new System.Drawing.Size(48, 31); - bdr
			this.btnDown.Size = new System.Drawing.Size(108, 28);
			this.btnDown.TabIndex = 6;
			this.btnDown.Text = "6";
			this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
			// 
			// RoboSepItemList
			// 
			this.BackColor = System.Drawing.Color.SandyBrown;
			this.Controls.Add(this.pnlItemListContainer);
			this.Controls.Add(this.pnlScrollControl);
			this.Name = "RoboSepItemList";
		  //this.Size = new System.Drawing.Size(240, 280); bdr
			this.Size = new System.Drawing.Size(248, 402);
			this.Resize += new System.EventHandler(this.RoboSepItemList_Resize);
			this.pnlItemListContainer.ResumeLayout(false);
			this.pnlScrollControl.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion								

		private void pnlScrollControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
		
		}

		private void pnlItemList_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
		
		}
	}
}



