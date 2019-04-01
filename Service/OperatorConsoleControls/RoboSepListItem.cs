//----------------------------------------------------------------------------
// RoboSepListItem
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
using System.Drawing.Text;
using System.Data;
using System.Windows.Forms;

using Tesla.Common.ResourceManagement;
using Tesla.Common.DrawingUtilities;

namespace Tesla.OperatorConsoleControls
{
	/// <summary>
	/// Summary description for RoboSepListItem.
	/// </summary>
	public class RoboSepListItem : System.Windows.Forms.UserControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private Image myImage = null;

		private bool isSelectedItem;

		private bool isFlashingItem;
		private bool isSelectedAlwaysItem;
		private int index;

		#region Construction/destruction

		public RoboSepListItem()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// Set appropriate control styles
			this.SetStyle(ControlStyles.DoubleBuffer        |
				ControlStyles.AllPaintingInWmPaint          |
				ControlStyles.UserPaint                     |
                ControlStyles.SupportsTransparentBackColor  |
				ControlStyles.ResizeRedraw				    |
				ControlStyles.StandardClick,
				true); 
			this.UpdateStyles();
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

		#region ListItem-specific behaviour

		public bool IsSelectedItem
		{
			get
			{
				return isSelectedItem;
			}
			set
			{
				isSelectedItem = value;
				Refresh();
			}
		}
		public bool IsFlashingItem
		{
			get
			{
				return isFlashingItem;
			}
			set
			{
				isFlashingItem = value;
			}
		}
		public bool IsSelectedAlwaysItem
		{
			get
			{
				return isSelectedAlwaysItem;
			}
			set
			{
				isSelectedAlwaysItem = value;
				Refresh();
			}
		}
		public int Index
		{
			get
			{
				return index;
			}
			set
			{
				index = value;
			}
		}
		#endregion ListItem-specific behaviour

		#region UserControl overrides

		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
			}
		}

		/// <summary>
		/// Image to display on the list item.
		/// The image must be sized appropriately, a 35x35 pixel image is recommended.
		/// </summary>
		public Image Image
		{
			get
			{
				return myImage;
			}
			set
			{
				myImage = value;
			}
		}

		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
            // Perform default background fill
            base.OnPaintBackground(pevent);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			// Apply standard Graphics settings
			DrawingUtilities.ApplyStandardContext(e.Graphics);	
		
            // Fill the list item region according to whether it's enabled/selected.
            Color fillColour;
			if(isSelectedAlwaysItem)
			{
				fillColour = ColourScheme.GetColour(ColourSchemeItem.ReagentsListItemHighlight);
			}
			else if ( ! this.Enabled)
			{
				// List/item disabled
				fillColour = ColourScheme.GetColour(ColourSchemeItem.NamedAreaStandardBackground);
			}
			else if (isSelectedItem)	// Enabled and selected
			{
				fillColour = ColourScheme.GetColour(ColourSchemeItem.ReagentsListItemHighlight);
			}
			else if(isFlashingItem)
			{
				if(DateTime.Now.Millisecond % 1000 > 500)
				{
					fillColour = ColourScheme.GetColour(ColourSchemeItem.NamedAreaStandardBackground);
				}
				else
				{
					fillColour = ColourScheme.GetColour(ColourSchemeItem.MaintenanceListItemBackground);
				}
			}
			else	// Enabled, but not selected
			{
				// Default list item background colour
				fillColour = ColourScheme.GetColour(ColourSchemeItem.MaintenanceListItemBackground);																						   
			}

            // Fill the bounding path
            using (SolidBrush fillBrush = new SolidBrush(fillColour))
            {                
                e.Graphics.FillPath(fillBrush, myBoundingPath);
            }

            // Draw the region boundary line
            using (Pen borderPen = new Pen(
                       ColourScheme.GetColour(ColourSchemeItem.NamedAreaBoundary), 2.0f))
            {
                borderPen.Alignment = PenAlignment.Center;
                e.Graphics.DrawPath(borderPen,	myBoundingPath);                
            }

			// Determine the bounding rectangles to render the text and image to 
			Rectangle imageRectangle;
			Rectangle textRectangle;
			if (myImage != null)
			{
				int padding = DrawingUtilities.MinimumTabMargin;
				imageRectangle = new Rectangle(
					this.ClientRectangle.Right - myImage.Width - padding,
					this.ClientRectangle.Top + padding,
					myImage.Width,
					myImage.Height );

				textRectangle = new Rectangle(
					this.ClientRectangle.Left,
					this.ClientRectangle.Top,
					ClientRectangle.Width - imageRectangle.Width,
					ClientRectangle.Height );
			}
			else
			{
				imageRectangle = ClientRectangle;
				textRectangle = ClientRectangle;
			}

			// Render the Image if it exists
			if (myImage != null)
            {
                e.Graphics.DrawImageUnscaled(myImage,imageRectangle);
            }

			// Render the Name text if it exists
			if (this.Text != null && this.Text != string.Empty)
			{
				// Get a brush for drawing text
				SolidBrush textBrush = new SolidBrush(
					ColourScheme.GetColour(ColourSchemeItem.GridTextForeground));

				// Get a string format for centre-aligned text, centered vertically
				StringFormat stringFormat = DrawingUtilities.CentreCentreStringFormat;

#if (TESLA_UI_SHOW_BOUNDING_RECTANGLES)
				e.Graphics.DrawRectangle(Pens.CornflowerBlue, myNameAreaRectangle);					
#endif
				// Add the list item description to the display
				e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
				e.Graphics.DrawString(this.Text,
					SeparatorResourceManager.GetFont(SeparatorResourceManager.FontId.MaintenanceListItem),
					textBrush, 
					textRectangle,
					stringFormat);

				// Clean up graphics resources
				textBrush.Dispose();
			}
			
            // Perform default painting behaviour
            base.OnPaint(e);
		}

		protected override void OnClick(EventArgs e)
		{
			base.OnClick (e);

			if (this.Tag != null)
			{
				RoboSepItemList listCtl = this.Parent.Parent.Parent as RoboSepItemList;
				if (listCtl != null)
				{
					listCtl.PerformItemAction(this.Tag);
				}
			}
		}

		#endregion UserControl overrides

		#region Control event handlers

		GraphicsPath myBoundingPath = new GraphicsPath();
		RectangleF   myBoundingRectangleF;

		private void RoboSepListItem_Resize(object sender, System.EventArgs e)
		{
			myBoundingRectangleF = new RectangleF(0.0f, 0.0f,
				this.Size.Width, this.Size.Height);
			DrawingUtilities.RoundedRectanglePath(myBoundingRectangleF, 
				DrawingUtilities.RoundingRadius, out myBoundingPath);
		}

		#endregion Control event handlers

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// RoboSepListItem
			// 
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Name = "RoboSepListItem";
			this.Size = new System.Drawing.Size(140, 56);
			this.Resize += new System.EventHandler(this.RoboSepListItem_Resize);

		}
		#endregion
	}
}
