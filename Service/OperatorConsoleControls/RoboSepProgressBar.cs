//----------------------------------------------------------------------------
// RoboSepProgressBar
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

using Tesla.Common.ResourceManagement;
using Tesla.Common.DrawingUtilities;

namespace Tesla.OperatorConsoleControls
{
	/// <summary>
	/// Summary description for RoboSepProgressBar.
	/// </summary>
	public class RoboSepProgressBar : System.Windows.Forms.UserControl
	{
		#region Types

		public enum DisplayMode
		{
			Normal,
			Warning,
			Error
		}

		#endregion Types

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#region Construction/destruction

		public RoboSepProgressBar()
		{
			// Initialise Control Styles to avoid WM_ERASEBACKGROUND default behaviour, etc.				
			this.SetStyle(ControlStyles.DoubleBuffer        |
				ControlStyles.AllPaintingInWmPaint          |
				ControlStyles.UserPaint                     |
				ControlStyles.SupportsTransparentBackColor  |
				ControlStyles.ResizeRedraw				    |
				ControlStyles.StandardClick				    |
				ControlStyles.ContainerControl,
				true); 
			this.UpdateStyles();

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// Set default values
			myMinimum = 0;
			myMaximum = 100;
			myValue	  = 0;
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

		#region Progress Bar behaviour

		private DisplayMode	myDisplayMode;

		public DisplayMode ProgressDisplayMode
		{
			get
			{
				return myDisplayMode;				
			}
			set
			{
				myDisplayMode = value;
				this.Invalidate();
			}
		}

		private int myMinimum,
					myMaximum,
					myValue;

		public int Minimum
		{
			get
			{
				return myMinimum;
			}
			set
			{
				// Update the progress minimum setting
				myMinimum = value;

				// Adjust other values if necessary to maintain data integrity
				if (myMinimum > myMaximum)
				{
					myMaximum = myMinimum;
				}
				if (myMinimum > myValue)
				{
					myValue = myMinimum;
				}

				// Update the display
				this.Invalidate();
			}
		}

		public int Maximum
		{
			get
			{
				return myMaximum;
			}
			set
			{
				// Update the progress maximum setting
				myMaximum = value;

				// Adjust other values if necessary to maintain data integrity
				if (myMaximum < myValue)
				{
					myValue = myMaximum;
				}
				if (myMaximum < myMinimum)
				{
					myMinimum = myMaximum;
				}

				// Update the display
				this.Invalidate();
			}
		}

		public int Value
		{
			get
			{
				return myValue;
			}
			set
			{
				// Check the supplied value is within range and if so, update the setting and display.
				if (myMinimum <= value && value <= myMaximum)
				{
					myValue = value;
					this.Invalidate();
				}
			}
		}

		public void Increment(int value)
		{
			myValue += value;
			if (myValue > myMaximum)
			{
				myValue = myMaximum;
			}
            
			this.Invalidate();
		}

		#endregion Progress Bar behaviour

		#region UserControl overrides

		protected override void OnPaint(PaintEventArgs e)
		{
			// Default behaviour
			base.OnPaint(e);

			// Draw the progress bar, coloured according to display mode
			Color barColour;
			switch(myDisplayMode)
			{
				default:
				case DisplayMode.Normal:
					barColour = Color.LightGreen;
					break;
				case DisplayMode.Warning:
					barColour = Color.Yellow;
					break;
				case DisplayMode.Error:
					barColour = Color.Red;
					break;
			}

			// Draw the standard 3D look
			Rectangle borderRect = new Rectangle(0, 0, this.Width, this.Height);
			ControlPaint.DrawBorder3D(e.Graphics, borderRect, Border3DStyle.SunkenInner);

			// Calculate and display the progress bar
            float progressPercentage = (float)myValue / (float)(myMaximum - myMinimum) * (float)100.0;
			int progressWidth = (int)(this.Width * progressPercentage / (float)100.0);
			Rectangle progressRect = new Rectangle(0, 0, progressWidth, this.Height);
			using (SolidBrush progressBrush = new SolidBrush(barColour))
			{
				e.Graphics.FillRectangle(progressBrush, progressRect);
			}

            // Get a brush for drawing progress percentage text
            SolidBrush textBrush = new SolidBrush(
                ColourScheme.GetColour(ColourSchemeItem.TabTextForeground));

            // Get a string format for centre-aligned text, centered vertically
            StringFormat stringFormat = DrawingUtilities.CentreCentreStringFormat;          			

            // Add the percentage progress string to the display
            string percentageProgress = (progressPercentage <= 0.0) ? string.Empty : 
                ((int)progressPercentage).ToString() + "%";
            e.Graphics.DrawString(percentageProgress,
                SeparatorResourceManager.GetFont(SeparatorResourceManager.FontId.RunProgressInfo), 
                textBrush, 
                this.ClientRectangle,
                stringFormat);

            // Clean up graphics resources
            textBrush.Dispose();
		}

		#endregion UserControl overrides

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
	}
}
