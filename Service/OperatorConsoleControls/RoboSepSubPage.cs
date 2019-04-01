//----------------------------------------------------------------------------
// RoboSepSubPage
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
using Tesla.Common.OperatorConsole;

using Tesla.Separator;

namespace Tesla.OperatorConsoleControls
{
	/// <summary>
	/// Base class for UI Subpages
	/// </summary>
	/// <remarks>
	/// Note: use of the 'abstract' keyword means the designer can fail to show 
	/// sub-classes (fails with an error that this class is marked abstract and so
	/// can't be constructed).  Future versions should investigate if there's a way around this 
	/// (for example, by setting the design-time flag).
	/// </remarks>
	public /*abstract*/ class RoboSepSubPage : System.Windows.Forms.UserControl
	{
		public enum MdiChild
		{
			Maintenance = 0,
			MessageLog,
			UserProfile,
			About,
			NUM_MDI_CHILDREN,
			FIRST_CHILD = Maintenance
		}

		#region Events

		public delegate void MdiChildActivateDelegate(MdiChild activatedChild);

		public event MdiChildActivateDelegate ActivateMdiChild;

		#endregion Events
		
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;		

		#region Construction/destruction

		// NOTE: refer to comments at the class level (we seem to need to explicitly define a 
        // default constructor in order to keep the design-time environment happy).
		protected RoboSepSubPage()
		{
		}		

		public RoboSepSubPage(MdiChild id)
		{
			// Set control styles to ignore WM_ERASEBACKGROUND, and thereby avoid the 
			// default black-background redraw performed by Windows
			this.SetStyle(ControlStyles.DoubleBuffer    |
				ControlStyles.AllPaintingInWmPaint      |
				ControlStyles.UserPaint                 |
				ControlStyles.ResizeRedraw				|
				ControlStyles.StandardClick				|
				ControlStyles.ContainerControl,
				true); 
			this.UpdateStyles();	

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// Initialise constructor-supplied members
			myMdiChildId = id;
		}

		/// <summary>
		/// Two-stage construction is required so we can construct the sub page before attempting to connect 
		/// to the Separator server.
		/// </summary>
		public virtual void Initialise()
		{
			// Register for Instrument events
			RegisterForSeparatorEvents();
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

		#region Global MDI Support

        // NOTE: this method is defined as virtual and not abstract as otherwise we need to
        // define the class abstact, and that means we (apparently) can't display at design-time.
        // Is there a way around this (and still be able to use the 'abstract' keyword)???
        // If not, the current public (in subclass) and protected (in this base class)
        // setup is OK...as a design-time convenience.
		public virtual void EnableSelectionAccess(UiAccessMode accessMode)
		{
		}

		public virtual void DisableSelectionAccess(UiAccessMode accessMode)
		{
		}

		#endregion Global MDI Support

		#region Pseudo MDI Support

		private MdiChild	myMdiChildId;
		protected Panel		pnlTabArea;

		public MdiChild MdiChildId
		{
			get
			{
				return myMdiChildId;
			}
		}

        private GraphicsPath    myTabXorPath = new GraphicsPath();
        private Rectangle       myTabAreaRect;

		protected void ResizeHandler(object sender, System.EventArgs e)
		{
            if (myTabXorPath != null)
            {
                myTabXorPath.Dispose();
                myTabXorPath = new GraphicsPath();
            }

            int tabCount = (int)RoboSepSubPage.MdiChild.NUM_MDI_CHILDREN;
            int resizedTabWidth = this.Width/tabCount;
            int tabGap = DrawingUtilities.MinimumTabMargin * 2;
            Rectangle tabRectangle = new Rectangle();

			switch(myMdiChildId)
			{
				case RoboSepSubPage.MdiChild.Maintenance:                   
                    // Define the tab boundary
                    tabRectangle.X = tabGap;
                    tabRectangle.Y = 0;
                    tabRectangle.Width = resizedTabWidth - 2 * tabGap;
                    tabRectangle.Height = pnlTabArea.Height;
                    CalculateTabPath(tabRectangle, ref myTabXorPath, ref myTabAreaRect);
					break;
				case RoboSepSubPage.MdiChild.MessageLog:
					// Define the tab boundary
					tabRectangle.X = tabGap + resizedTabWidth;
					tabRectangle.Y = 0;
					tabRectangle.Width = resizedTabWidth - 2 * tabGap;
					tabRectangle.Height= pnlTabArea.Height;
					CalculateTabPath(tabRectangle, ref myTabXorPath, ref myTabAreaRect);					
					break;
				case RoboSepSubPage.MdiChild.UserProfile:
					// Define the tab boundary
					tabRectangle.X = tabGap + 2*resizedTabWidth+ tabGap;
					tabRectangle.Y = 0;
					tabRectangle.Width = resizedTabWidth - 2 * tabGap;
					tabRectangle.Height= pnlTabArea.Height;
					CalculateTabPath(tabRectangle, ref myTabXorPath, ref myTabAreaRect);					
					break;
				case RoboSepSubPage.MdiChild.About:
                    // Define the tab boundary
                    tabRectangle.X = this.Width - resizedTabWidth + tabGap;
                    tabRectangle.Y = 0;
                    tabRectangle.Width = resizedTabWidth - 2 * tabGap;
                    tabRectangle.Height= pnlTabArea.Height;
                    CalculateTabPath(tabRectangle, ref myTabXorPath, ref myTabAreaRect);
					break;
			}

            // Define the name area rectangle
            pnlTabArea.Location = myTabAreaRect.Location;
            pnlTabArea.Width = myTabAreaRect.Width;
            pnlTabArea.Height= myTabAreaRect.Height;
			
			Rectangle baseRectangle = new Rectangle(this.Location.X,
				this.Location.Y, this.Width, this.Height);			
			Region formBoundary = new Region(baseRectangle);

            // Exclude the non-tab areas from this window's region
            formBoundary.Xor(myTabXorPath);
			
            // Set the updated region boundary
            this.Region = formBoundary;
		}

        private void CalculateTabPath(Rectangle tabBoundingBox, 
            ref GraphicsPath tabXorPath, ref Rectangle tabNameAreaRect)
        {
            int tabCurveWidth = DrawingUtilities.RoundingRadius;
            Rectangle tabCurveLL, tabCurveTL, tabCurveTR, tabCurveLR;

            tabCurveLL = new Rectangle();
            tabCurveLL.X = tabBoundingBox.X;
            tabCurveLL.Y = tabBoundingBox.Height - tabCurveWidth;
            tabCurveLL.Width = tabCurveWidth;
            tabCurveLL.Height= tabCurveWidth;

            tabCurveTL = new Rectangle();
            tabCurveTL.X = tabCurveLL.X + tabCurveWidth;
            tabCurveTL.Y = this.Location.Y;
            tabCurveTL.Width = tabCurveWidth;
            tabCurveTL.Height= tabCurveWidth;

            tabCurveTR = new Rectangle();
            tabCurveTR.X = tabBoundingBox.X + tabBoundingBox.Width - 2 * tabCurveWidth;
            tabCurveTR.Y = this.Location.Y;
            tabCurveTR.Width = tabCurveWidth;
            tabCurveTR.Height= tabCurveWidth;

            tabCurveLR = new Rectangle();
            tabCurveLR.X = tabCurveTR.X + tabCurveWidth;
            tabCurveLR.Y = tabCurveLL.Y;
            tabCurveLR.Width = tabCurveWidth;
            tabCurveLR.Height= tabCurveWidth;

            tabNameAreaRect.X = tabCurveTL.X + tabCurveTL.Width;
            tabNameAreaRect.Y = 0;
            tabNameAreaRect.Width = tabCurveTR.X - tabNameAreaRect.X;
            tabNameAreaRect.Height= tabBoundingBox.Height;

            if (tabXorPath != null)
            {
                tabXorPath.Dispose();
                tabXorPath = new GraphicsPath();
            }
            // Define the graphics path (Note: 'AddArc' angles are clockwise-positive from
            // zero degrees -- that is, the opposite to normal mathematics convention.)
            GraphicsPath tabXorPathLeft = new GraphicsPath();
            GraphicsPath tabXorPathRight= new GraphicsPath();
            // Define the path to remove to create the left-of-tab area
            tabXorPathLeft.StartFigure();
            tabXorPathLeft.AddLine(this.Location.X, this.Location.Y, 
                this.Location.X, tabBoundingBox.Height);
            // Follow the tab's lower left corner
            tabXorPathLeft.AddArc(tabCurveLL, 90, -90);
            // Follow the tab's top left corner
            tabXorPathLeft.AddArc(tabCurveTL, 180, 90);
            // Close the path
            tabXorPathLeft.CloseFigure();

            // Define the path to remove to create the right-of-tab area
            tabXorPathRight.StartFigure();
            tabXorPathRight.AddLine(this.Location.X + this.Width, 
                this.Location.Y,
                this.Location.X + this.Width, tabBoundingBox.Height);
            // Follow the tab's lower right corner
            tabXorPathRight.AddArc(tabCurveLR, 90, 90);
            // Follow the tab's top right corner
            tabXorPathRight.AddArc(tabCurveTR, 0, -90);
            // Close the path
            tabXorPathRight.CloseFigure();
            
            // Combine the XOR paths
            tabXorPath.AddPath(tabXorPathLeft, false);
            tabXorPath.AddPath(tabXorPathRight, false);
        }

		private void pnlTabArea_Click(object sender, System.EventArgs e)
		{
			// The user has clicked our tab, so inform our parent 'tab control' in case
			// it needs to change the active sub page
			if (ActivateMdiChild != null)
			{
				ActivateMdiChild(myMdiChildId);
			}
		}

		protected override void OnClick(EventArgs e)
		{
			base.OnClick (e);
			// The user has clicked our tab, so inform our parent 'tab control' in case
			// it needs to change the active sub page
			if (ActivateMdiChild != null)
			{
				ActivateMdiChild(myMdiChildId);
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown (e);
		}

		#endregion Pseudo MDI Support		

		#region User Control overrides

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

		private const int theNameAreaOffset = DrawingUtilities.MinimumTabMargin;
		private Rectangle myNameAreaRectangle = new Rectangle();

		protected override void OnPaint(PaintEventArgs e)
		{
			// Perform default painting behaviour
			base.OnPaint(e);

			// Apply standard Graphics settings
			DrawingUtilities.ApplyStandardContext(e.Graphics);

			// Render the Name text if it exists
			if (this.Text != null && this.Text != string.Empty)
			{
				// Get a brush for drawing text
				SolidBrush textBrush = new SolidBrush(
					ColourScheme.GetColour(ColourSchemeItem.TabTextForeground));

				/// Get a string format for centre-aligned text, centered vertically
				StringFormat stringFormat = DrawingUtilities.CentreCentreStringFormat;          			

				// Get the rendered string size
				SizeF textSize = e.Graphics.MeasureString(this.Text, 
					SeparatorResourceManager.GetFont(SeparatorResourceManager.FontId.MaintenanceSubpageTitle),
					pnlTabArea.Width,
					stringFormat);
				
				// Vertically centre the text in the region
				myNameAreaRectangle.X = pnlTabArea.Location.X;
				myNameAreaRectangle.Width = pnlTabArea.Width;
				int textSizeHeight = (int)textSize.Height;				
				int textHeight = (textSizeHeight >= (this.pnlTabArea.Height - theNameAreaOffset)) ? 
					this.pnlTabArea.Height - theNameAreaOffset : textSizeHeight;
				myNameAreaRectangle.Height = textHeight;

				// Because we've adjusted the region to give a tab appearance, a simple
				// "vertically centered" calculation for Y offset does not seem to work/
				// does not appear vertically centered.  The figures appear correct but 
				// somehow the clipping region offset is non-zero.  So, adjust the nominal
				// Y offset slightly.
				myNameAreaRectangle.Y = (this.pnlTabArea.Height - textHeight)/2 +
					e.ClipRectangle.Y - theNameAreaOffset/2;
	
#if (TESLA_UI_SHOW_BOUNDING_RECTANGLES)
				e.Graphics.DrawRectangle(Pens.CornflowerBlue, myNameAreaRectangle);					
#endif
					
				// Add the Name string to the display
				e.Graphics.DrawString(this.Text,
					SeparatorResourceManager.GetFont(SeparatorResourceManager.FontId.MaintenanceSubpageTitle), 
					textBrush, 
					myNameAreaRectangle,
					stringFormat);

				// Clean up graphics resources
				textBrush.Dispose();
			}
		}

		#endregion User Control overrides 

		#region Separator Events

		protected ISeparator		 mySeparator;
		protected SeparatorEventSink myEventSink;

        /// <summary>
        /// Allow subclasses to register for specific instrument events.
        /// </summary>
        /// <remarks>
        /// Refer to class comments section regarding a design-time problem with the
        /// use of the 'abstract' keyword.  As a partial workaround, just declare this
        /// method as virtual.
        /// </remarks>
        public /*abstract*/ virtual void RegisterForSeparatorEvents()
        {
            // At design-time, we don't want to actually call methods on the 
            // control/events APIs, so check whether they actually exist (only at
            // run-time) before assigning local references.
            ISeparator aSeparator = SeparatorGateway.GetInstance().ControlApi;
            if (aSeparator != null)
            {
                mySeparator = aSeparator;
            }
            SeparatorEventSink aEventSink = SeparatorGateway.GetInstance().EventsApi;
            if (aEventSink != null)
            {
                myEventSink = aEventSink;
            }
            
        }

		#endregion Separator Events

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.pnlTabArea = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// pnlTabArea
			// 
			this.pnlTabArea.BackColor = System.Drawing.Color.Orange;
			this.pnlTabArea.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlTabArea.Location = new System.Drawing.Point(0, 0);
			this.pnlTabArea.Name = "pnlTabArea";
			this.pnlTabArea.Size = new System.Drawing.Size(620, 50);
			this.pnlTabArea.TabIndex = 3;
			this.pnlTabArea.Visible = false;
			this.pnlTabArea.Click += new System.EventHandler(this.pnlTabArea_Click);
			// 
			// RoboSepSubPage
			// 
			this.Controls.Add(this.pnlTabArea);
			this.Name = "RoboSepSubPage";
			this.Size = new System.Drawing.Size(620, 410);			
			this.ResumeLayout(false);

		}
		#endregion
	}
}
