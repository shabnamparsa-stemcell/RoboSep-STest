//----------------------------------------------------------------------------
// RoboSepNamedPanel
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
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;

using Tesla.Common.ResourceManagement;
using Tesla.Common.DrawingUtilities;

namespace Tesla.OperatorConsoleControls
{
	/// <summary>
	/// Summary description for RoboSepNamedPanel.
	/// </summary>
	public class RoboSepNamedPanel : System.Windows.Forms.UserControl
	{		
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#region Construction/destruction

		public RoboSepNamedPanel()
		{
			// Initialise Control Styles to avoid WM_ERASEBACKGROUND default behaviour, etc.				
			this.SetStyle(ControlStyles.DoubleBuffer        |
				ControlStyles.AllPaintingInWmPaint          |
				ControlStyles.UserPaint                     |
				ControlStyles.SupportsTransparentBackColor  |
				//ControlStyles.Opaque						|
				ControlStyles.ResizeRedraw				    |
				ControlStyles.StandardClick				    |
				ControlStyles.ContainerControl,
				true); 
			this.UpdateStyles();

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			
			myNameAreaRectangle.X = theNameAreaOffset/2;
			myNameAreaRectangle.Y = theNameAreaOffset/2;
			myNameAreaRectangle.Width = 0;
			myNameAreaRectangle.Height = 0;

            // Set default fill colour
            myFillColour = ColourScheme.GetColour(ColourSchemeItem.NamedAreaStandardBackground);
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

        #region Named Panel specific

        private Color   myFillColour;

        public Color FillColor
        {
            get
            {
                return myFillColour;
            }
            set
            {
                myFillColour = value;
                Refresh();
            }
        }

        #endregion Named Panel specific

		#region User Control overrides

		protected override void OnBackColorChanged(EventArgs e)
		{
			base.OnBackColorChanged (e);
			bool isSolidBackColor = this.BackColor.A == 255;
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, isSolidBackColor);
			this.UpdateStyles();
		}


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

		public Rectangle NameTextAreaRectangle
		{
			get
			{
				return myNameAreaRectangle;
			}
		}

		protected override void OnPaintBackground(PaintEventArgs pevent)
		{	
			// Draw the background.  Do NOT call the base class method so that we
			// can support "transparent" effects by ignoring WM_ERASEBKGND and doing our
			// painting from the OnPaint method.
			using (SolidBrush backgroundBrush = new SolidBrush(this.BackColor))
			{
				pevent.Graphics.FillRectangle(backgroundBrush, pevent.ClipRectangle);
			}
		}

		private const int theNameAreaOffset = DrawingUtilities.MinimumTabMargin;
		private Rectangle myNameAreaRectangle = new Rectangle();
	
		protected override void OnPaint(PaintEventArgs e)
		{						
			// Apply standard Graphics settings
			DrawingUtilities.ApplyStandardContext(e.Graphics);			

			// Fill the bounding path
            using (SolidBrush fillBrush = new SolidBrush(this.FillColor))
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
			
#if (DEBUG)
			// Put some text in so we can see it in the design view.  (Otherwise,
			// all text that is displayed on the UI should come via the 
			// SeparatorResourceManager so internationalised string resources can be used.)
			if (this.Text == null || this.Text == string.Empty)
			{
				this.Text = "Text";
			}
#endif
			// Draw the Name text if it exists
			if (this.Text != null && this.Text != string.Empty)
			{
				// Get a brush for drawing text
				SolidBrush textBrush = new SolidBrush(
					ColourScheme.GetColour(ColourSchemeItem.NamedAreaTextForeground));

				// Get a string format for left-aligned text, centered vertically
				StringFormat stringFormat = DrawingUtilities.NearCentreStringFormat;           			

				// Get the rendered string size
				SizeF textSize = e.Graphics.MeasureString(this.Text, 
					SeparatorResourceManager.GetFont(SeparatorResourceManager.FontId.InstrumentTasksPageTitle),
					this.Width - theNameAreaOffset, 
					stringFormat);
				
				// Position the text box at top-left corner of the region
                myNameAreaRectangle.X = theNameAreaOffset;
				myNameAreaRectangle.Width = (int)textSize.Width + theNameAreaOffset/2;
				int textSizeHeight = (int)textSize.Height;				
				int textHeight = (textSizeHeight >= (this.Height - theNameAreaOffset)) ? 
                    this.Height - theNameAreaOffset : textSizeHeight;
				myNameAreaRectangle.Height = textHeight;
				myNameAreaRectangle.Y = theNameAreaOffset;	

#if (TESLA_UI_SHOW_BOUNDING_RECTANGLES)
				e.Graphics.DrawRectangle(Pens.CornflowerBlue, myNameAreaRectangle);					
#endif
					
				// Add the Name string to the display
				e.Graphics.DrawString(this.Text,
					SeparatorResourceManager.GetFont(SeparatorResourceManager.FontId.InstrumentTasksPageTitle), 
					textBrush, 
					myNameAreaRectangle,
					stringFormat);

				// Clean up graphics resources
				textBrush.Dispose();				
			}

			// Perform default painting behaviour
			base.OnPaint(e);
		}

		GraphicsPath myBoundingPath = new GraphicsPath();
		RectangleF   myBoundingRectangleF;

		private void RoboSepNamedPanel_Resize(object sender, System.EventArgs e)
		{		
			myBoundingRectangleF = new RectangleF(0.0f, 0.0f,
				this.Size.Width, this.Size.Height);
			DrawingUtilities.RoundedRectanglePath(myBoundingRectangleF, 
				DrawingUtilities.RoundingRadius, out myBoundingPath);
		}

		#endregion User Control overrides

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// RoboSepNamedPanel
			// 
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
			this.Name = "RoboSepNamedPanel";
			this.Size = new System.Drawing.Size(336, 72);
			this.Resize += new System.EventHandler(this.RoboSepNamedPanel_Resize);

		}
		#endregion		
	}
}
