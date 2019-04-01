//----------------------------------------------------------------------------
// RoboSepSamplesGrid
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
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using Invetech.ApplicationLog;

using Tesla.Common.Separator;
using Tesla.Common.Protocol;
using Tesla.Common.ResourceManagement;
using Tesla.Common.DrawingUtilities;
using Tesla.Common.OperatorConsole;

namespace Tesla.OperatorConsoleControls
{
	/// <summary>
	/// Grid layout to display Sample/Quadrant configuration, including multi-quadrant
	/// protocols when required.
	/// </summary>
	public class RoboSepSamplesGrid : System.Windows.Forms.UserControl
	{		
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#region Construction/destruction

		public RoboSepSamplesGrid()
		{
			// Pre-InitializeComponent, Initialise layout 
			// Note: we need to do this as the InitializeComponent() routine will trigger
			// a Resize, and as part of the layout calculations used by Resize, we need to
			// have initialised some of the default layout.
			for (int i = 0; i < (int)QuadrantId.NUM_QUADRANTS; ++i)
			{
				myProtocolQuadrantCounts[i] = 1;
			}

            // Initialise action grid data
            myActionGrid = new ArrayList();

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();			

			// Initialise Control Styles to avoid WM_ERASEBACKGROUND default behaviour, etc.				
			this.SetStyle(ControlStyles.DoubleBuffer    |
				ControlStyles.AllPaintingInWmPaint      |
				ControlStyles.UserPaint                 |
				ControlStyles.ResizeRedraw				|
				ControlStyles.StandardClick,
				true); 
			this.UpdateStyles();

            // Register for chosen protocol data updates
            if ( ! this.DesignMode)
            {
                SeparatorGateway.GetInstance().UpdateChosenProtocolTable += new SampleTableDelegate(AtChosenProtocolTableUpdate);
            }
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

		public delegate void InvokeCellActionDelegate(QuadrantId quadrantId, SamplesGridColumn samplesColumn);

		public event InvokeCellActionDelegate	InvokeCellAction;

		private void SamplesGridCellClicked(ActionCell actionCell)
		{
			if (InvokeCellAction != null)
			{
				InvokeCellAction(QuadrantIdFromProtocolRowIndex(actionCell.Row), actionCell.Column);
			}
		}

		#endregion Events

        #region Event handlers

        private string[,] mySamplesInfo = new string[0, (int)SamplesGridColumn.NUM_SAMPLE_GRID_COLUMNS];

		private ProtocolMix myProtocolMix;
			
        private void AtChosenProtocolTableUpdate(DataTable chosenProtocolsTableInfo,
			ProtocolMix protocolMix)
        {
			try
			{
				if (this.InvokeRequired)
				{
					SampleTableDelegate eh = new SampleTableDelegate(this.AtChosenProtocolTableUpdate);
					this.Invoke(eh, new object[]{chosenProtocolsTableInfo, protocolMix});
				}
				else
				{
					// Record whether the chosen protocols are all ISeparationProtocols or not
					myProtocolMix = protocolMix;

					// Number of selected protocols
					int protocolCount = chosenProtocolsTableInfo.Rows.Count;
					
					// Calculate the number of quadrants used by the selected quadrants
					int myTotalQuadrantCount = 0;
					foreach (DataRow infoRow in chosenProtocolsTableInfo.Rows)
					{
						string strQuadrantCount = (string)infoRow["QuadrantCount"];
						myTotalQuadrantCount += Int32.Parse(strQuadrantCount);
					}
					// mySamplesInfo needs to have a total of 4 quadrants stored in it.  
					// Note, however, due to multi-quadrant protocols, is may have fewer 
					// than four records. 
					// Calculate how many quadrants are used in the selected protocols, then 
					// add a row for each remaining empty quadrant.
					int numEmptyQuadrants = (int)QuadrantId.NUM_QUADRANTS - myTotalQuadrantCount;
					mySamplesInfo = new string[protocolCount + numEmptyQuadrants,
						(int)SamplesGridColumn.NUM_SAMPLE_GRID_COLUMNS ];
                    int currQuadrant = 0;
					// Fill in the remaining rows of mySamplesInfo, that correspond to empty quadrants
					for (int row = 0; row < (protocolCount+numEmptyQuadrants); row++)
					{
						// Search for a selected protocol for the current quadrant, and add it if found
						bool isFound = false;
						foreach (DataRow infoRow in chosenProtocolsTableInfo.Rows)
						{
							// NOTE: the table column names used here must match the definition in
							// SeparatorGateway.
							if ( (int)infoRow["InitialQuadrant"] == currQuadrant )
							{						
								// Initial Quadrant.  
								mySamplesInfo[row,(int)SamplesGridColumn.QuadrantNumber] = 
									((int)infoRow["InitialQuadrant"]).ToString();    
								// Protocol label (name)
								mySamplesInfo[row,(int)SamplesGridColumn.ProtocolName] =
									(string)infoRow["ProtocolName"];
								// Sample volume
								mySamplesInfo[row,(int)SamplesGridColumn.SampleVolume] =
									(string)infoRow["SampleVolume"];
								// Protocol type
								mySamplesInfo[row,(int)SamplesGridColumn.ProtocolType] =
									(string)infoRow["Type"];
								
								// Quadrant count
								string strQuadrantCount = (string)infoRow["QuadrantCount"];
								mySamplesInfo[row,(int)SamplesGridColumn.QuadrantCount] = 
									strQuadrantCount;
								// Update the current Quadrant
								currQuadrant += Int32.Parse(strQuadrantCount);
								// We should only ever find one match, so stop looking for others
								isFound = true;
								break;
							}
						}

						// This quadrant is empty, so fill it in accordingly
						if ( ! isFound)
						{
							// Initial Quadrant.  
							mySamplesInfo[row,(int)SamplesGridColumn.QuadrantNumber] = 
								((int) currQuadrant).ToString();                       
							// Protocol label (name)
							mySamplesInfo[row,(int)SamplesGridColumn.ProtocolName] = string.Empty;
							// Sample volume
							mySamplesInfo[row,(int)SamplesGridColumn.SampleVolume] = string.Empty;
							// Protocol type
							mySamplesInfo[row,(int)SamplesGridColumn.ProtocolType] = string.Empty;
							// Quadrant count
							mySamplesInfo[row,(int)SamplesGridColumn.QuadrantCount] = ((int) 1).ToString();
							currQuadrant++;
						}
					}
					
					RecalculateActionGrid();
					Refresh();
				}
				
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
        }
        
        #endregion Event handlers

		#region RoboSep behaviour

		private bool isReadOnly;

		public bool ReadOnly
		{
			set
			{
				if (isReadOnly != value)
				{
					isReadOnly = value;	
			        RecalculateActionGrid();
					Refresh();
				}
			}
		}

		#endregion RoboSep behaviour

		#region Protocol information

		public void ProtocolCount(out int readyToRun, out int protocolCount)
		{
			protocolCount = 0;
			readyToRun = 0;	
			for (int row=0; row < mySamplesInfo.GetLength(0); row++)
			{	
				if (mySamplesInfo[row,(int)SamplesGridColumn.ProtocolName].Length != 0)
				{
					++protocolCount;

					double sampleVolume = 
						ParseSampleVolume(mySamplesInfo[row,(int)SamplesGridColumn.SampleVolume]);
					if (sampleVolume > 0.0d || 
						(mySamplesInfo[row,(int)SamplesGridColumn.ProtocolType] == 
						GetLocalisedProtocolClassName(ProtocolClass.Maintenance)))
					{
						++readyToRun;
					}	
				}
			}
		}

		private string GetLocalisedProtocolClassName(ProtocolClass protocolClass)
		{
			string localisedName = string.Empty;
			switch (protocolClass)
			{
				case ProtocolClass.Positive:
					localisedName = SeparatorResourceManager.GetSeparatorString(StringId.ProtocolClassPositiveText);
					break;
				case ProtocolClass.Negative:
					localisedName = SeparatorResourceManager.GetSeparatorString(StringId.ProtocolClassNegativeText);
					break;
				case ProtocolClass.HumanPositive:
					localisedName = SeparatorResourceManager.GetSeparatorString(StringId.ProtocolClassHumanPositiveText);
					break;
				case ProtocolClass.HumanNegative:
					localisedName = SeparatorResourceManager.GetSeparatorString(StringId.ProtocolClassHumanNegativeText);
					break;
				case ProtocolClass.MousePositive:
					localisedName = SeparatorResourceManager.GetSeparatorString(StringId.ProtocolClassMousePositiveText);
					break;
				case ProtocolClass.MouseNegative:
					localisedName = SeparatorResourceManager.GetSeparatorString(StringId.ProtocolClassMouseNegativeText);
					break;
				case ProtocolClass.WholeBloodPositive:
					localisedName = SeparatorResourceManager.GetSeparatorString(StringId.ProtocolClassWholeBloodPositiveText);
					break;
				case ProtocolClass.WholeBloodNegative:
					localisedName = SeparatorResourceManager.GetSeparatorString(StringId.ProtocolClassWholeBloodNegativeText);
					break;
				case ProtocolClass.Maintenance:
					localisedName = SeparatorResourceManager.GetSeparatorString(StringId.ProtocolClassMaintenanceText);
					break;
				case ProtocolClass.Shutdown:
					localisedName = SeparatorResourceManager.GetSeparatorString(StringId.ProtocolClassShutdownText);
					break;
			}
			return localisedName;
		}

		/// <summary>
		/// Get the protocol initial quadrant for the specified protocol/samples table 
		/// row index.
		/// </summary>
		/// <param name="rowIndex"></param>
		/// <returns></returns>
		public QuadrantId QuadrantIdFromProtocolRowIndex(int rowIndex)
		{
			QuadrantId quadrantId = QuadrantId.NoQuadrant;
			if (rowIndex < mySamplesInfo.GetLength(0))
			{
				quadrantId = (QuadrantId)Enum.Parse(typeof(QuadrantId), 
					mySamplesInfo[rowIndex,(int)SamplesGridColumn.QuadrantNumber]);
			}
			return quadrantId;
		}

		#endregion Protocol information		

		#region Layout calculations

		public enum SamplesGridColumn
		{
			QuadrantNumber,
			ProtocolName,
			SampleVolume,
			ProtocolType,
			DeleteSelection,
			NUM_VISIBLE_SAMPLE_GRID_COLUMNS,
            QuadrantCount = NUM_VISIBLE_SAMPLE_GRID_COLUMNS,
            NUM_SAMPLE_GRID_COLUMNS,
			FIRST_COLUMN = QuadrantNumber
		}

		private enum Endpoint
		{
			Start = 0,	// Left  or Top
			Finish		// Right or Bottom
		}

		private const float theQuadrantColumnPercentage			=  4/100f;
		private const float theProtocolNameColumnPercentage		= 58/100f;
		private const float theSampleVolumeColumnPercentage		= 14/100f;
		private const float theProtocolTypeColumnPercentage		= 14/100f;
		private const float theDeleteProtocolColumnPercentage	= 10/100f;
		private const int	theColumnHeaderAreaHeight		= 25;
		private const int   theLayoutGridRowDimension		= 1+(int)QuadrantId.NUM_QUADRANTS;
		private const int	theLayoutGridColDimension		= 1+(int)SamplesGridColumn.NUM_VISIBLE_SAMPLE_GRID_COLUMNS;
		

		private Rectangle myProtocolNameColumnHeaderRect = new Rectangle();
		private Rectangle mySampleVolumeColumnHeaderRect = new Rectangle();
		private Rectangle myProtocolTypeColumnHeaderRect = new Rectangle();
		private Rectangle myDeleteSelectionColumnHeaderRect = new Rectangle();
		private Rectangle[] myQuadrantColumnRects = new Rectangle[(int)QuadrantId.NUM_QUADRANTS];

		private int[]		myProtocolQuadrantCounts = new int[(int)QuadrantId.NUM_QUADRANTS];

		private int[]		mySampleRowYOffsets = new int[(int)QuadrantId.NUM_QUADRANTS];

		private Point[,]	myLayoutGridRowPoints = new Point[theLayoutGridRowDimension, 2];
		private Point[,]	myLayoutGridColPoints = new Point[theLayoutGridColDimension, 2];

		#region Action Grid definitions

		private struct ActionCell
		{
			private string				myCellText;
			private Rectangle			myBoundingRectangle;
			private int					myRow;
			private SamplesGridColumn	myColumn;
            private bool                isActive;

			public string Text
			{
				get
				{
					return myCellText;
				}
				set
				{
					myCellText = value;
				}
			}

			public Rectangle BoundingRectangle
			{
				get
				{
					return myBoundingRectangle;
				}
				set
				{
					myBoundingRectangle = value;
				}
			}

			public int Row
			{
				get
				{
					return myRow;
				}
				set
				{
					myRow = value;
				}
			}

			public SamplesGridColumn Column
			{
				get
				{
					return myColumn;
				}
				set
				{
					myColumn = value;
				}
			}

            public bool IsActive
            {
                get
                {
                    return isActive;
                }
                set
                {
                    isActive = value;
                }
            }
		}

		private ArrayList myActionGrid;

		#endregion Action Grid definitions

        int myQuadrantColumnLeft;
        int myNameColumnLeft;
        int myVolumeColumnLeft;
        int myTypeColumnLeft;
		int myDeleteColumnLeft;
		int myDeleteColumnRight;
		int mySingleRowHeight;
        int myHeaderTopYOffset;
        int myHeaderBottomYOffset;

		private void RecalculateLayoutBoundaries()
		{
			myQuadrantColumnLeft = 0 + DrawingUtilities.MinimumTabMargin;
			myNameColumnLeft = myQuadrantColumnLeft + (int)(this.Width * theQuadrantColumnPercentage);
			myVolumeColumnLeft = myNameColumnLeft + (int)(this.Width * theProtocolNameColumnPercentage);
			myTypeColumnLeft = myVolumeColumnLeft + (int)(this.Width * theSampleVolumeColumnPercentage);
			myDeleteColumnLeft = myTypeColumnLeft + (int)(this.Width * theProtocolTypeColumnPercentage);
			myDeleteColumnRight = this.Width - DrawingUtilities.MinimumTabMargin;

			mySingleRowHeight= (int)((float)(this.Height - 2 * DrawingUtilities.MinimumTabMargin -
				theColumnHeaderAreaHeight) / (float)QuadrantId.NUM_QUADRANTS);

			mySampleRowYOffsets[(int)QuadrantId.Quadrant1] = 
				myProtocolQuadrantCounts[(int)QuadrantId.Quadrant1] * mySingleRowHeight;

			mySampleRowYOffsets[(int)QuadrantId.Quadrant2] =
                myProtocolQuadrantCounts[(int)QuadrantId.Quadrant2] * mySingleRowHeight 
				+ mySampleRowYOffsets[(int)QuadrantId.Quadrant1];

			mySampleRowYOffsets[(int)QuadrantId.Quadrant3] = 
				myProtocolQuadrantCounts[(int)QuadrantId.Quadrant3] * mySingleRowHeight
				+ mySampleRowYOffsets[(int)QuadrantId.Quadrant2];

			mySampleRowYOffsets[(int)QuadrantId.Quadrant4] =
				myProtocolQuadrantCounts[(int)QuadrantId.Quadrant4] * mySingleRowHeight
				+ mySampleRowYOffsets[(int)QuadrantId.Quadrant3];

			myHeaderTopYOffset = DrawingUtilities.MinimumTabMargin/2;
			myHeaderBottomYOffset = myHeaderTopYOffset + theColumnHeaderAreaHeight;

			// Define the layout grid column line endpoints
			for (int col = 0; col <= (int)SamplesGridColumn.NUM_VISIBLE_SAMPLE_GRID_COLUMNS; ++col)
			{
				int xOffset = 0;
				switch(col)
				{
					case (int)SamplesGridColumn.QuadrantNumber:
						xOffset = myQuadrantColumnLeft;
						break;
					case (int)SamplesGridColumn.ProtocolName:
						xOffset = myNameColumnLeft;
						break;
					case (int)SamplesGridColumn.SampleVolume:
						xOffset = myVolumeColumnLeft;
						break;
					case (int)SamplesGridColumn.ProtocolType:
						xOffset = myTypeColumnLeft;
						break;
					case (int)SamplesGridColumn.DeleteSelection:
						xOffset = myDeleteColumnLeft;
						break;
					case (int)SamplesGridColumn.NUM_VISIBLE_SAMPLE_GRID_COLUMNS:
						xOffset = myDeleteColumnRight;
						break;
				}
				myLayoutGridColPoints[col, (int)Endpoint.Start].X = xOffset;
				myLayoutGridColPoints[col, (int)Endpoint.Finish].X= xOffset;
				myLayoutGridColPoints[col, (int)Endpoint.Start].Y = myHeaderBottomYOffset;
				myLayoutGridColPoints[col, (int)Endpoint.Finish].Y= myHeaderBottomYOffset + mySingleRowHeight * (int)QuadrantId.NUM_QUADRANTS;
			}

			// Define the layout grid row line endpoints
			for (int row = 0; row < theLayoutGridRowDimension; ++row)
			{
				int yOffset = myHeaderBottomYOffset + row * mySingleRowHeight;
				int xFinish = myQuadrantColumnLeft;

				switch(row)
				{
					default:
						int cumulativeQuadrantCount = 0;
						for (int q = 0; q < row; ++q)
						{
							cumulativeQuadrantCount += myProtocolQuadrantCounts[q];
						}
						xFinish = (cumulativeQuadrantCount > row) ? 
							myNameColumnLeft : myDeleteColumnRight;
						break;
					case (int)QuadrantId.Quadrant1:
					case (int)QuadrantId.Quadrant4:
						xFinish = myDeleteColumnRight;
						break;
				}
				myLayoutGridRowPoints[row, (int)Endpoint.Start].X = myQuadrantColumnLeft;
				myLayoutGridRowPoints[row, (int)Endpoint.Finish].X= xFinish;
				myLayoutGridRowPoints[row, (int)Endpoint.Start].Y = yOffset;
				myLayoutGridRowPoints[row, (int)Endpoint.Finish].Y= yOffset;
			}

			// Define the Quadrant number column rectangles
			for (int row = 0; row < (int)QuadrantId.NUM_QUADRANTS; ++row)
			{
				myQuadrantColumnRects[row].X = myQuadrantColumnLeft;
				myQuadrantColumnRects[row].Y = myHeaderBottomYOffset + row * mySingleRowHeight;
				myQuadrantColumnRects[row].Width  = myNameColumnLeft - myQuadrantColumnLeft;
				myQuadrantColumnRects[row].Height = mySingleRowHeight;
			}			

			// Define the column header rectangles
			myProtocolNameColumnHeaderRect.Y = myHeaderTopYOffset;
			myProtocolNameColumnHeaderRect.X = myNameColumnLeft;
			myProtocolNameColumnHeaderRect.Width = myVolumeColumnLeft - myNameColumnLeft;
			myProtocolNameColumnHeaderRect.Height= theColumnHeaderAreaHeight;

			mySampleVolumeColumnHeaderRect.Y = myHeaderTopYOffset;
			mySampleVolumeColumnHeaderRect.X = myVolumeColumnLeft;
			mySampleVolumeColumnHeaderRect.Width = myTypeColumnLeft - myVolumeColumnLeft;
			mySampleVolumeColumnHeaderRect.Height= theColumnHeaderAreaHeight;
			
			myProtocolTypeColumnHeaderRect.Y = myHeaderTopYOffset;
			myProtocolTypeColumnHeaderRect.X = myTypeColumnLeft;
			myProtocolTypeColumnHeaderRect.Width = myDeleteColumnLeft - myTypeColumnLeft;
			myProtocolTypeColumnHeaderRect.Height= theColumnHeaderAreaHeight;

			myDeleteSelectionColumnHeaderRect.Y = myHeaderTopYOffset;
			myDeleteSelectionColumnHeaderRect.X = myDeleteColumnLeft;
			myDeleteSelectionColumnHeaderRect.Width = myDeleteColumnRight - myDeleteColumnLeft;
			myDeleteSelectionColumnHeaderRect.Height= theColumnHeaderAreaHeight;
		}

		/// <summary>
		/// Recalculate the 'action grid' (the set of inner rectangles where the user
		/// can click and therefore potentially navigate to another screen, enter a
		/// cell value, etc).
		/// </summary>
        private void RecalculateActionGrid()
        {
            myActionGrid.Clear();

            // If there is sample data, add it to the action grid
            if (mySamplesInfo.GetLength(0) > 0) // that is, if # protocols (sample grid rows) > 0
            {
                // Add an action cell for each:
                // (1) chosen protocol name,
                // (2) each sample volume > 0, and 
                // (3) each protocol type,
				// (4) to delete each protocol,
                // taking into account the quadrant count for the protocol.
                for (int row = 0; row < mySamplesInfo.GetLength(0); ++row)
                {
                    AddProtocolNameActionCell(row);
					AddSampleVolumeActionCell(row);
                    AddProtocolTypeActionCell(row);
					AddProtocolDeleteActionCell(row);
				}
            }
        }

		private static Regex theNumberPattern = new Regex("(([0-9]+.[0-9]*)|([0-9]*.[0-9]+)|([0-9]+))");

		private double ParseSampleVolume(string strSampleVolumeWithUnits)
		{
			// NOTE: future versions should consider instead passing the volume as a number and 
            // using a method on FluidVolume to add the units string...for now, extract the
			// numeric value from the formatted string (with units).
			string strValue = theNumberPattern.Match(strSampleVolumeWithUnits).Value;
			double result = 0.0d;
			if (strValue != string.Empty)
			{
				result = double.Parse(strValue);
			}
			return result;
		}

        private void AddProtocolNameActionCell(int row)
        {
            ActionCell cell = new ActionCell();

			// All non-empty protocol name cells are active if all chosen protocols are
			// ISeparationProtocol protocols (including the "< Select Protocol >" placeholder
			// when in "edit mode").
            cell.IsActive = myProtocolMix == ProtocolMix.SeparationOnly;
            
			Rectangle actionRect = Rectangle.Empty;
            actionRect.X = myNameColumnLeft;
			actionRect.Y = CalculateCumulativeChosenProtocolYOffset(row);
            actionRect.Height = CalculateChosenProtocolHeight(row);
            actionRect.Width  = myVolumeColumnLeft - myNameColumnLeft;
            actionRect.Inflate(DrawingUtilities.CellActionAreaInflateFactor, 
                DrawingUtilities.CellActionAreaInflateFactor);

			cell.Text = mySamplesInfo[row,(int)SamplesGridColumn.ProtocolName];
			// If the protocol name is empty, show "< Select Protocol >" if we're in "edit mode"
            if (cell.Text == string.Empty && ( ! isReadOnly) && 
				myProtocolMix == ProtocolMix.SeparationOnly)
            {
                cell.Text = SeparatorResourceManager.GetSeparatorString(
                    StringId.RunSamplesSelectProtocolPromptText);
            }
            cell.Row = row;
			cell.Column = SamplesGridColumn.ProtocolName;
            cell.BoundingRectangle = actionRect;
            myActionGrid.Add(cell);
        }

        private int CalculateCumulativeChosenProtocolYOffset(int row)
        {
            int yOffset = myHeaderBottomYOffset;
            for (int previousRow = 0; previousRow < row; ++previousRow)
            {
                yOffset += mySingleRowHeight *
                    Int32.Parse(mySamplesInfo[previousRow,(int)SamplesGridColumn.QuadrantCount]);
            }
            return yOffset;
        }

		private int CalculateChosenProtocolHeight(int row)
		{
			int quadrantCount = Int32.Parse(mySamplesInfo[row,
				(int)SamplesGridColumn.QuadrantCount]);
			// Catch the case where the quadrant count hasn't been set
			if (quadrantCount == 0)
				quadrantCount = 1;
			return mySingleRowHeight * quadrantCount;
		}

        private void AddSampleVolumeActionCell(int row)
        {
            ActionCell cell = new ActionCell();
            cell.IsActive = false;
 
			Rectangle actionRect = Rectangle.Empty;
            actionRect.X = myVolumeColumnLeft;
			actionRect.Y = CalculateCumulativeChosenProtocolYOffset(row);
			actionRect.Height = CalculateChosenProtocolHeight(row);
			actionRect.Width  = myTypeColumnLeft - myVolumeColumnLeft;
            actionRect.Inflate(DrawingUtilities.CellActionAreaInflateFactor, 
                DrawingUtilities.CellActionAreaInflateFactor);

            cell.Row = row;
            cell.Text = mySamplesInfo[row,(int)SamplesGridColumn.SampleVolume];

			// We do not want to display negative sample volumes
			if ( cell.Text.StartsWith("-") )
			{
				cell.Text = string.Empty;
			}

			if (cell.Text.Length > 0)
			{
				// Allow the user to edit the Sample Volume in the case of Separation 
				// Protocols only
				switch (myProtocolMix)
				{
					default:
						cell.IsActive = false;
						cell.Text = string.Empty;
						break;
					case ProtocolMix.SeparationOnly:
						cell.IsActive = true;
						break;	
				}
			}			

			cell.Column = SamplesGridColumn.SampleVolume;
            cell.BoundingRectangle = actionRect;
            myActionGrid.Add(cell);
        }

        private void AddProtocolTypeActionCell(int row)
        {
            ActionCell cell = new ActionCell();
            cell.IsActive = false;
    
			Rectangle actionRect = Rectangle.Empty;
            actionRect.X = myTypeColumnLeft;
            actionRect.Y = CalculateCumulativeChosenProtocolYOffset(row);
			actionRect.Height = CalculateChosenProtocolHeight(row);
			actionRect.Width  = myDeleteColumnLeft - myTypeColumnLeft;
            actionRect.Inflate(DrawingUtilities.CellActionAreaInflateFactor, 
                DrawingUtilities.CellActionAreaInflateFactor);

            cell.Row = row;
            cell.Text = mySamplesInfo[row,(int)SamplesGridColumn.ProtocolType];
            cell.Column = SamplesGridColumn.ProtocolType;
            cell.BoundingRectangle = actionRect;
            myActionGrid.Add(cell);
        }

		private void AddProtocolDeleteActionCell(int row)
		{
			ActionCell cell = new ActionCell();
			cell.IsActive = false;
 
			Rectangle actionRect = Rectangle.Empty;
			actionRect.X = myDeleteColumnLeft;
			actionRect.Y = CalculateCumulativeChosenProtocolYOffset(row);
			actionRect.Height = CalculateChosenProtocolHeight(row);
			actionRect.Width  = myDeleteColumnRight - myDeleteColumnLeft;
			actionRect.Inflate(DrawingUtilities.CellActionAreaInflateFactor, 
				DrawingUtilities.CellActionAreaInflateFactor);

			cell.Row = row;
			//Conditionally display delete
			if (mySamplesInfo[row,(int)SamplesGridColumn.ProtocolName].Length > 0 &&
				myProtocolMix != ProtocolMix.ShutdownOnly)
			{
				cell.IsActive = true;
				cell.Text = "X";
			}
			cell.Column = SamplesGridColumn.DeleteSelection;
			cell.BoundingRectangle = actionRect;
			myActionGrid.Add(cell);
		}

		#endregion Layout calculations

		#region User Control overrides 

		protected override void OnPaintBackground(PaintEventArgs pevent)
		{			
			using (SolidBrush backgroundBrush = new SolidBrush(
					   ColourScheme.GetColour(ColourSchemeItem.NamedAreaStandardBackground)))
			{
				// Fill the background
				pevent.Graphics.FillRectangle(backgroundBrush, pevent.ClipRectangle);

				if (this.DesignMode)
				{
					// Apply standard Graphics settings (note we do this here and not
					// above the 'fill the background' step, as otherwise when we're hosted
					// on the same colour background, seem to miss painting a pixel width
					// line at the top and left of the control boundary.
					DrawingUtilities.ApplyStandardContext(pevent.Graphics);

					// Draw a bounding rectangle, just so we can see the boundary if hosted
					// on a control with the NamedAreaStandardBackground colour
					Rectangle insideBoundary = new Rectangle(pevent.ClipRectangle.Location,
						pevent.ClipRectangle.Size);
					insideBoundary.Inflate(-1,-1);
					pevent.Graphics.DrawRectangle(Pens.Purple, insideBoundary);
				}
			}			
		}		

		protected override void OnPaint(PaintEventArgs e)
		{
			// Default painting behaviour
			base.OnPaint(e);

			// Apply standard Graphics settings
			DrawingUtilities.ApplyStandardContext(e.Graphics);
			
			// Draw the grid lines
			Pen gridPen = new Pen(
				ColourScheme.GetColour(ColourSchemeItem.GridLine), 1.0f);
			for (int col = 0; col <= (int)SamplesGridColumn.NUM_VISIBLE_SAMPLE_GRID_COLUMNS; ++col)
			{
				e.Graphics.DrawLine(gridPen,
					myLayoutGridColPoints[col, (int)Endpoint.Start],
					myLayoutGridColPoints[col, (int)Endpoint.Finish]);
			}										
			int sampleIndex = 0;
			for (int row = 0; row <= (int)QuadrantId.NUM_QUADRANTS; ++row)
			{
				// Always draw the top and bottom table horizontal boundary lines
				if (row == 0 || row == (int)QuadrantId.NUM_QUADRANTS)
				{
					e.Graphics.DrawLine(gridPen,
						myLayoutGridRowPoints[row, (int)Endpoint.Start],
						myLayoutGridRowPoints[row, (int)Endpoint.Finish]);
				}

				// Draw the intermediate row lines between quadrant numbers
				if (row < (int)QuadrantId.NUM_QUADRANTS)
				{
					int quadrantCount = 1;
					try
					{
						quadrantCount = Int32.Parse(mySamplesInfo[sampleIndex,(int)SamplesGridColumn.QuadrantCount]);
					}
					catch (Exception)
					{
						// If we run out of samples (chosen protocols) data, then quadrant count defaults to 1 quadrant's worth
						quadrantCount = 1;
					}

					// Determine the extent of the protocol's quadrant usage
					int gridRowIndex = row + 1;
					int protocolLastGridRow = row + quadrantCount;

					if (quadrantCount == 1)
					{
						// Draw a grid line across all columns
						e.Graphics.DrawLine(gridPen,
							myLayoutGridRowPoints[gridRowIndex, (int)Endpoint.Start],
							myLayoutGridRowPoints[gridRowIndex, (int)Endpoint.Finish]);
					}
					else if (quadrantCount > 1)
					{														
						while (gridRowIndex <= protocolLastGridRow)
						{
							if (gridRowIndex < protocolLastGridRow)
							{
								// Draw the grid line for intermediate quadrants, in the quadrant number column only.
								int yDivider = myLayoutGridRowPoints[gridRowIndex, (int)Endpoint.Start].Y;
								e.Graphics.DrawLine(gridPen,
									myLayoutGridRowPoints[gridRowIndex, (int)Endpoint.Start],
									new Point(myNameColumnLeft, yDivider));
							}
							else
							{
								// Draw the grid line across all columns
								e.Graphics.DrawLine(gridPen,
									myLayoutGridRowPoints[gridRowIndex, (int)Endpoint.Start],
									myLayoutGridRowPoints[gridRowIndex, (int)Endpoint.Finish]);
							}

							// Move to the next intermediate grid row line
							++gridRowIndex;
						}							
					}

					// Index to the next sample/chosen protocol data
					++sampleIndex;

					// Jump to the last row for the current protocol's quadrant extent
					// (note that the 'row' variable is then incremented as part of the 
					// "for" loop, immediately after this statement).
					row = protocolLastGridRow - 1;
				}
			}

			// Get a brush for drawing the main grid text
			SolidBrush gridTextBrush = new SolidBrush(
				ColourScheme.GetColour(ColourSchemeItem.GridTextForeground));

			// Get a brush for drawing the outer grid text
			SolidBrush textBrush = new SolidBrush(
				ColourScheme.GetColour(ColourSchemeItem.NamedAreaTextForeground));

			// Get a string format for left-aligned text, centered vertically
			StringFormat stringFormat = DrawingUtilities.CentreCentreStringFormat;           			

			// Get the rendered string size
			Font quadrantColFont = SeparatorResourceManager.GetFont(SeparatorResourceManager.FontId.SampleGridColumnHeader);
			SizeF textSize = e.Graphics.MeasureString("1", 
				quadrantColFont,
				myQuadrantColumnRects[(int)QuadrantId.Quadrant1].Width,
				stringFormat);

			// Draw the Quadrant numbers
			for (int row = 0; row < (int)QuadrantId.NUM_QUADRANTS; ++row)
			{
				string strQuadrantNumber = (row+1).ToString();
				e.Graphics.DrawString(strQuadrantNumber,
					quadrantColFont,
					textBrush,
                    myQuadrantColumnRects[row],
					stringFormat);
			}

			// Draw action grid contents
            if (myActionGrid != null && myActionGrid.Count > 0)
            {
				SolidBrush activeCellBrush = null;
                System.Collections.IEnumerator aEnumerator = myActionGrid.GetEnumerator();
                while (aEnumerator.MoveNext())
                {
                    ActionCell cell = (ActionCell)aEnumerator.Current;
					if (cell.IsActive && ( ! isReadOnly))
					{
						// Get a brush for painting the active cell area
						if (activeCellBrush == null)
						{
							activeCellBrush = new SolidBrush(
								ColourScheme.GetColour(ColourSchemeItem.GridCellActive));
						}
						e.Graphics.FillRectangle(activeCellBrush, cell.BoundingRectangle);
					}

					// Draw the cell text.
					// Do not display the delete cell if the table is read only.
					if (!( isReadOnly && (cell.Column == SamplesGridColumn.DeleteSelection)) )
					{
						e.Graphics.DrawString(cell.Text,
							quadrantColFont,
							gridTextBrush,
							cell.BoundingRectangle,
							stringFormat);
					}
                }

				// Clean up graphics resources
				if (activeCellBrush != null)
				{
					activeCellBrush.Dispose();
				}
            }

			// Draw the column headers
			Font headerColFont = SeparatorResourceManager.GetFont(
				SeparatorResourceManager.FontId.SampleGridColumnHeader);

			string		columnHeader = string.Empty;
			Rectangle	columnHeaderRect;
			for (SamplesGridColumn col = SamplesGridColumn.FIRST_COLUMN+1;
				col < SamplesGridColumn.NUM_VISIBLE_SAMPLE_GRID_COLUMNS;
				++col)
			{
				switch(col)
				{
					default:
						continue;
					case SamplesGridColumn.ProtocolName:
						columnHeader = SeparatorResourceManager.GetSeparatorString(
							StringId.RunProtocolNameColumnHeaderText);
						columnHeaderRect = myProtocolNameColumnHeaderRect;
						break;
					case SamplesGridColumn.SampleVolume:
						columnHeader = SeparatorResourceManager.GetSeparatorString(
							StringId.RunSampleVolumeColumnHeaderText);
						columnHeaderRect = mySampleVolumeColumnHeaderRect;
						break;
					case SamplesGridColumn.ProtocolType:
						columnHeader = SeparatorResourceManager.GetSeparatorString(
							StringId.RunProtocolTypeColumnHeaderText);
						columnHeaderRect = myProtocolTypeColumnHeaderRect;
						break;
					case SamplesGridColumn.DeleteSelection:
						columnHeader = SeparatorResourceManager.GetSeparatorString(
							StringId.RunDeleteSelectionColumnHeaderText);
						columnHeaderRect = myDeleteSelectionColumnHeaderRect;
						break;
				}

				// Draw the column header text
				e.Graphics.DrawString(columnHeader,
					headerColFont,
					textBrush, 
					columnHeaderRect,
					stringFormat);
#if (TESLA_UI_SHOW_BOUNDING_RECTANGLES)
				e.Graphics.DrawRectangle(Pens.CornflowerBlue, columnHeaderRect);					
#endif
			}

			// Clean up graphics resources
			textBrush.Dispose();			
			gridTextBrush.Dispose();
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			RecalculateLayoutBoundaries();
            // Recalculate the 'action grid' (the set of inner rectangles where the user
            // can click and therefore potentially navigate to another screen, enter a
            // cell value, etc).
            RecalculateActionGrid();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if ( ! isReadOnly)
			{
				// Determine whether the user clicked within an action cell, and if so,
				// invoke the relevant action.
				// NOTE: The action may result in the table being modified, therefore we exit
				// the foreach loop beforehand to be safe.
				Point clickPoint = new Point(e.X, e.Y);
				ActionCell clickedCell = new ActionCell();
				foreach (ActionCell cell in myActionGrid)
				{				
					if (cell.BoundingRectangle.Contains(clickPoint))
					{
						clickedCell = cell;
						break;
					}
				}
				if (clickedCell.IsActive)
				{
					switch(clickedCell.Column)
					{
						case SamplesGridColumn.ProtocolName:
							SamplesGridCellClicked(clickedCell);
							break;
						case SamplesGridColumn.SampleVolume:
							SamplesGridCellClicked(clickedCell);
							break;
						case SamplesGridColumn.DeleteSelection:
							SamplesGridCellClicked(clickedCell);
							break;
					}
				}
				
			}
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
			// RoboSepSamplesGrid
			// 
			this.Name = "RoboSepSamplesGrid";
			this.Size = new System.Drawing.Size(540, 160);

		}
        #endregion
    }
}
