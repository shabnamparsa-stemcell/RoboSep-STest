//----------------------------------------------------------------------------
// ColourScheme
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

namespace Tesla.Common.DrawingUtilities
{
	#region Operator Console items subject to colour configuration 

	public enum ColourSchemeItem
	{
		// Color for the application backdrop (behind the main pages)
		ApplicationBackdrop,

		// Colours for main pages
		ApplicationBackground,
		InactiveTabBackground,
		TabTextForeground,

		// Colours for subpages
		InactiveSubPageBackground,
		ActiveSubPageBackground,

		// Colours for Named Areas
		NamedAreaStandardBackground,
		NamedAreaBoundary,
		NamedAreaTextForeground,

		// Colours for Grids
		GridLine,
		GridCellActive,
		GridTextForeground,

		// Colours for Item Lists
		MaintenanceListItemBackground,
		ReagentsListItemHighlight,

		// Colours for buttons
		ButtonTextForeground,

		NUM_COLOUR_ITEMS,
		FIRST_COLOUR_ITEM	= ApplicationBackdrop,
		LAST_COLOUR_ITEM	= NUM_COLOUR_ITEMS - 1
	}

	#endregion Operator Console items subject to colour configuration

	/// <summary>
	/// Colour scheme manager for Operator Console
	/// </summary>
	public class ColourScheme
	{
		private static ColourScheme	theColourScheme;
		
		private static Color[]		theColourItems;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lookupItem"></param>
		/// <returns></returns>
		/// <remarks>
		/// Helper method -- simplifies call syntax for callers.
		/// </remarks>
		public static Color GetColour(ColourSchemeItem lookupItem)
		{
			return ColourScheme.GetInstance().LookupColour(lookupItem);
		}

		public static ColourScheme GetInstance()
		{			
			if (theColourScheme == null)
			{
				try
				{
					theColourScheme = new ColourScheme();
				}
				catch
				{
					theColourScheme = null;
				}
			}
			return theColourScheme;
		}

		private ColourScheme()
		{			
			// Future: Override default colour scheme from colours configuration file (if present)
		}

		static ColourScheme()
		{
			theColourItems = new Color[(int)ColourSchemeItem.NUM_COLOUR_ITEMS];
			for (ColourSchemeItem item = ColourSchemeItem.FIRST_COLOUR_ITEM; 
				item <= ColourSchemeItem.LAST_COLOUR_ITEM; ++item)
			{
				switch(item)
				{
					// Colour for the application backdrop (behind the main pages)
					case ColourSchemeItem.ApplicationBackdrop:
						theColourItems[(int)item] = Color.Plum;
						break;
					// Colours for main pages
					case ColourSchemeItem.ApplicationBackground:
						//theColourItems[(int)item] = Color.Plum;
						theColourItems[(int)item] = Color.LightGray;
						break;
                    case ColourSchemeItem.InactiveTabBackground:
                        //theColourItems[(int)item] = Color.Orchid;
						theColourItems[(int)item] = Color.DarkGray;
                        break;
                    case ColourSchemeItem.TabTextForeground:
                        //theColourItems[(int)item] = Color.Purple;
						theColourItems[(int)item] = Color.DarkSlateGray;
                        break;

					// Colours for subpages
					case ColourSchemeItem.InactiveSubPageBackground:
						//theColourItems[(int)item] = Color.Orchid;
						theColourItems[(int)item] = Color.DarkGray;
						break;
					case ColourSchemeItem.ActiveSubPageBackground:
						//theColourItems[(int)item] = Color.Thistle;
						theColourItems[(int)item] = Color.Silver;
						break;

					// Colours for Named Areas
					case ColourSchemeItem.NamedAreaStandardBackground:
						theColourItems[(int)item] = Color.White;
						break;
					case ColourSchemeItem.NamedAreaBoundary:
						//theColourItems[(int)item] = Color.Purple;
						theColourItems[(int)item] = Color.DimGray;
						break;
					case ColourSchemeItem.NamedAreaTextForeground:
						theColourItems[(int)item] = Color.DimGray;
						break;

					// Colours for Grids
					case ColourSchemeItem.GridLine:
						//theColourItems[(int)item] = Color.Purple;
						theColourItems[(int)item] = Color.DimGray;
						break;
					case ColourSchemeItem.GridCellActive:
						//theColourItems[(int)item] = Color.Salmon;
						theColourItems[(int)item] = Color.LightGray;
						break;
					case ColourSchemeItem.GridTextForeground:
						//theColourItems[(int)item] = Color.Purple;	//Color.Black;
						theColourItems[(int)item] = Color.Black;
						break;

					// Colours for Item Lists
					case ColourSchemeItem.MaintenanceListItemBackground:
						//theColourItems[(int)item] = Color.Salmon;
						theColourItems[(int)item] = Color.LightGray;
						break;
					case ColourSchemeItem.ReagentsListItemHighlight:
						theColourItems[(int)item] = Color.LightGreen;
						break;

					// Colours for buttons
					case ColourSchemeItem.ButtonTextForeground:
						theColourItems[(int)item] = Color.Black;
						break;
                    
				}
			}
		}

		public Color LookupColour(ColourSchemeItem lookupItem)
		{
			return theColourItems[(int)lookupItem];
		}
	}
}
