//----------------------------------------------------------------------------
// DrawingUtilities
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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Collections;

namespace Tesla.Common.DrawingUtilities
{
	/// <summary>
	/// Drawing Utilities (constants, static methods for commonly used pens, brushes and 
	/// drawing routines)
	/// </summary>
	public class DrawingUtilities
	{
		#region Construction/destruction

		// This class is not intended to be instantiated -- it is a container for 
		// common (static) routines
		private DrawingUtilities()
		{		
		}

		static DrawingUtilities()
		{
			theNearNearStringFormat = new StringFormat();
			theNearNearStringFormat.Alignment = StringAlignment.Near;
			theNearNearStringFormat.LineAlignment = StringAlignment.Near;
			theNearNearStringFormat.Trimming = StringTrimming.EllipsisPath;

			theNearCentreStringFormat = new StringFormat();
			theNearCentreStringFormat.Alignment = StringAlignment.Near;
			theNearCentreStringFormat.LineAlignment = StringAlignment.Center;
			theNearCentreStringFormat.Trimming = StringTrimming.EllipsisPath;

			theCentreCentreStringFormat = new StringFormat();
			theCentreCentreStringFormat.Alignment = StringAlignment.Center;
			theCentreCentreStringFormat.LineAlignment = StringAlignment.Center;
			theCentreCentreStringFormat.Trimming = StringTrimming.EllipsisPath;

			theFarCentreStringFormat = new StringFormat();
			theFarCentreStringFormat.Alignment = StringAlignment.Far;
			theFarCentreStringFormat.LineAlignment = StringAlignment.Center;
			theFarCentreStringFormat.Trimming = StringTrimming.EllipsisPath;

			theFarNearStringFormat = new StringFormat();
			theFarNearStringFormat.Alignment = StringAlignment.Far;
			theFarNearStringFormat.LineAlignment = StringAlignment.Near;
			theFarNearStringFormat.Trimming = StringTrimming.EllipsisPath;
			
			theCentreNearStringFormat = new StringFormat();
			theCentreNearStringFormat.Alignment = StringAlignment.Center;
			theCentreNearStringFormat.LineAlignment = StringAlignment.Near;
			theCentreNearStringFormat.Trimming = StringTrimming.EllipsisPath;
		}

		#endregion Construction/destruction

		#region Constants

		public const int AntiScrollbarInflateFactor = -2;	// Negative inflate == shrink
		public const int CellActionAreaInflateFactor= -1;	// Negative inflate == shrink
		public const int RoundingRadius		= 10;			// pixels
		public const int MinimumTabMargin	= 6;			// pixels

		#endregion Constants

		#region Graphics configuration

		public static void ApplyStandardContext(Graphics gc)
		{
			gc.SmoothingMode = SmoothingMode.AntiAlias;
			gc.CompositingMode = CompositingMode.SourceOver;
			gc.TextRenderingHint = TextRenderingHint.AntiAlias;
		}

		#endregion Graphics configuration

		#region String formats

		private static StringFormat theNearNearStringFormat;
		private static StringFormat	theNearCentreStringFormat;
		private static StringFormat theCentreCentreStringFormat;
		private static StringFormat theFarCentreStringFormat;
		private static StringFormat theFarNearStringFormat;
		private static StringFormat theCentreNearStringFormat;

		public static StringFormat NearNearStringFormat
		{
			get
			{
				return theNearNearStringFormat;
			}
		}

		public static StringFormat NearCentreStringFormat
		{
			get
			{
				return theNearCentreStringFormat;
			}
		}

		public static StringFormat CentreCentreStringFormat
		{
			get
			{
				return theCentreCentreStringFormat;
			}
		}

		public static StringFormat FarCentreStringFormat
		{
			get
			{
				return theFarCentreStringFormat;
			}
		}

		public static StringFormat FarNearStringFormat
		{
			get
			{
				return theFarNearStringFormat;
			}
		}

		public static StringFormat CentreNearStringFormat
		{
			get
			{
				return theCentreNearStringFormat;
			}
		}

		#endregion String formats

		#region Drawing routines

		/// <summary>
		/// Deterimine the 'Rounded Rectangle' graphics path to enclose a supplied
		/// bounding rectangle with a given corner radius.
		/// </summary>
		/// <param name="boundingRectF"></param>
		/// <param name="cornerRadius"></param>
		/// <param name="roundedRectPath"></param>
		/// <remarks>
		/// This routine does not handle situations where the supplied bounding rectangle
		/// and corner radius parameters result in negative values for the calculated
		/// coordinates.  Therefore, it expects that forms/controls that call this routine
		/// have an appropriate minimum size set at design time, or otherwise protect from
		/// invalid parameters to this routine.
		/// </remarks>
		public static void RoundedRectanglePath(RectangleF boundingRectF, int cornerRadius,
			out GraphicsPath roundedRectPath)
		{
			roundedRectPath = new GraphicsPath();
			RectangleF boundaryRectF = new RectangleF(boundingRectF.Location,
				boundingRectF.Size);
			boundaryRectF.Inflate(-2,-2);
			int left, top, right, bottom, offsetX, offsetY;
			left	= (int)boundaryRectF.X; 
			right	= (int)(boundaryRectF.X + boundaryRectF.Width); 
			top		= (int)boundaryRectF.Y; 
			bottom	= (int)(boundaryRectF.Y + boundaryRectF.Height);

			// Calculate rounding offsets
			int maxRadiusAllowed = (int)Math.Min(boundingRectF.Width, boundingRectF.Height);
			offsetX = Math.Min(cornerRadius, maxRadiusAllowed);
			offsetY = offsetX;

			// Define the graphics path (Note: 'AddArc' angles are clockwise-positive from
			// zero degrees -- that is, the opposite to normal mathematics convention.)
			roundedRectPath.StartFigure();    
			// ...add the top-right corner
			roundedRectPath.AddArc(right-offsetX, top, offsetX, offsetY, 270, 90);    
			// ...add the bottom-right corner
			roundedRectPath.AddArc(right-offsetX, bottom-offsetY, offsetX, offsetY, 0, 90);    
			// ...add the bottom-left corner
			roundedRectPath.AddArc(left, bottom-offsetY, offsetX, offsetY, 90, 90);    
			// ...add the top-left corner
			roundedRectPath.AddArc(left, top, offsetX, offsetY, 180, 90);    
			// ...close the figure
			//roundedRectPath.AddLine(left+(offsetX/2), top, right-(offsetX/2), top);
			roundedRectPath.CloseFigure();
		}		

		#endregion Drawing routines
	}
}
