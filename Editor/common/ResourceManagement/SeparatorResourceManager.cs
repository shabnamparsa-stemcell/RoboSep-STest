//----------------------------------------------------------------------------
// SeparatorResourceManager
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
// Copyright � 2004. All Rights Reserved.
//
//----------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Diagnostics;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;

using Tesla.Common.Separator;
using Tesla.Common.Protocol;

namespace Tesla.Common.ResourceManagement
{
	#region UI String Resource IDs

	// The list of lookup keys for Separator UI string resources.  
	// NOTE: We do this for UI strings and not status/error codes at this stage because the
	// latter are shared resources (shared between the .NET and Python layers).  Status
	// and Error codes are sent to us from the Instrument Control layer and are therefore
	// ultimately sourced from the original status/error codes text file.  Hence, it seems
	// superfluous to define an enum for the status/error codes (and would necessitate 
	// maintenance of the enum when the status/error codes change).  This distinction 
	// indicates one (slight) disadvantage of using separate environments (.NET and Python) 
	// which currently implies limited type and resource sharing.  (NOTE: the upcoming 
	// "IronPython" -- Python as a native .NET language -- should be considered for future 
	// projects.)
	public enum StringId
	{
		// Run Log message TraceLevel strings
		TraceLevelVerbose, 
		TraceLevelInfo,
		TraceLevelWarning,
		TraceLevelError,

		// Separator strings - General
		Error,
		OK,
		Cancel,
		Yes,
		No,
        Or,
        Press,
        Confirm,
        Ignore,
		ConsoleVersion,

        // Separator strings - Initialisation
        InitialisationFailure,
        InitialSplashText,
        ErrorSplashText,

		// Separator strings - Units
		MicroLitres,
		MilliLitres,

		// Separator strings - Protocol types
		ProtocolClassPositiveText,
		ProtocolClassHumanPositiveText,
		ProtocolClassMousePositiveText,
		ProtocolClassNegativeText,
		ProtocolClassHumanNegativeText,
		ProtocolClassMouseNegativeText,
		ProtocolClassWholeBloodPositiveText,
		ProtocolClassWholeBloodNegativeText,
		ProtocolClassMaintenanceText,
		ProtocolClassShutdownText,

		// Separator strings - Protocol configuration
		EnterSampleVolume,
		EmptyResource,
		TubeResource,
		VialResource,
		FullTipsBoxResource,
		ReagentLotId,
		SampleTubeId, // bdr

		// Separator strings - Batch run
		HydraulicFluidLevelWarningCaption,
		HydraulicFluidLevelWarning,
		MaintenanceAndSeparationIncompatibleWarningCaption,
		MaintenanceAndSeparationIncompatibleWarning,
		SeparationAndMaintenanceIncompatibleWarningCaption,
		SeparationAndMaintenanceIncompatibleWarning,

		// Separator strings - Sample Processing page (& items common to its subpages)
		SampleProcessingText,
		MessagesText,
		CancelText,
		ProtocolNameText,

		// Sample Processing, Run subpage
		RunSamplesText,
        RunProgressText,
        RunOperatorIdText,
		RunHaltText,
		RunHaltingText,
		RunAbortingText,
		RunPauseText,
		RunPausingText,
		RunResumeText,
		RunStartText,
        RunStartingText,
		RunUnloadText,
        RunUnloadingText,
		RunSchedulingText,
		RunResumingText,
		RunSamplesSelectProtocolPromptText,
		RunProgressTimeRemainingText,
        RunMessagesRunOrSelectText,
        RunMessagesEndTimeSpanText,
		RunMessagesSelectProtocolText,		
		RunProtocolNameColumnHeaderText,
		RunSampleVolumeColumnHeaderText,
		RunProtocolTypeColumnHeaderText,
        RunDeleteSelectionColumnHeaderText,

		// Sample Processing, Protocols subpage
		ProtocolsSelectionText,
		ProtocolsMessagesSelectProtocolText,
		ProtocolsProtocolNameColumnText,
		ProtocolsQuadrantsColumnText,
		ProtocolsTypeColumnText,
        ProtocolsMRUColumnText,

		// Sample Processing, Sample Volume dialog
		SampleVolumeText,
		SampleVolumePromptText,
		SampleVolumeOutOfRangeText,

		// Sample Processing, User ID dialog
		UserIdText,
		UserIdPromptText,

		// Sample Processing, RoboSep Keypad
		KeypadCancelText,
		KeypadClearText,
		KeypadEnterText,

		// Sample Processing, Configuration subpage
		ConfigurationReagentsResourcesText,
		ConfigurationLocationText,
		ConfigurationLoadedText,
		ConfigurationRunText,
        ConfigurationMessagesLoadedRunConfirmationText,
		ConfigurationQuadrantNumberPromptText,
		ConfigurationNoneRequired,

		// Sample Processing, Reagent Lot ID dialog
		ReagentLotIdText,
		ReagentLotIdIdPromptText,
		ReagentLotIdInvalidText,

		// Sample Processing, Sample Tube ID dialog - bdr
		SampleTubeIdText,
		SampleTubeIdPromptText,

		// Separator strings - Sample Processing, Pause/Resume dialog
	  	PauseLidUpCaption,
	  	PauseLidUpMessage,
	  	PauseRetryMessage,
		PausePausingMessage,
		PausePausedMessage,

        // Separator strings - Run Confirm dialog - bdr
		ReadyToRunCaption,
		ReadyToRunConfirm,
        
		// Separator strings - Abort Confirm dialog - bdr
		AbortRunCaption,
		AbortRunConfirm,

		// Separator strings - Instrument Tasks page
		InstrumentTasksText,

		// Separator strings - Instrument Tasks, Maintenance subpage
		MaintenanceText,
        MaintenancePanelText,
        ShutdownPanelText,

		// Separator strings - Instrument Tasks, Message Log subpage
		MessageLogText,
		MessageLogTimeColumnText,
		MessageLogSeverityColumnText,
		MessageLogMessageColumnText,

        // Separator strings - About sub page
		AboutText,
        ButtonTextExit,
		AboutSoftwareGroupText,
		AboutSoftwareUiText,
		AboutSoftareInstrumentControlText,
		AboutInstrumentGroupText,
		AboutInstrumentSerialNumberText,
        AboutInstrumentServiceConnectionText,
        AboutInstrumentConnectNetworkCableText,

		// Separator strings - Exceptions
        ExInstrumentCommunicationsFailure,
		ExFailedToConnectToInstrumentControl,
		ExFailedToSubscribeToInstrumentControl,
		ExFailedToCreateSeparatorEventSink,
		ExFailedToConnectToSeparator,
		ExMissingMandatoryProtocol,

		// Separator strings - Relative Quadrant Locations
		SampleTube,
		VialB,//selection
		VialA,//magnetic
		VialC,//antibody
		SeparationTube,
		WasteTube,
		LysisBufferTube,
		TipRack,
        QuadrantBuffer,
        QuadrantBuffer34,
        QuadrantBuffer56,

		//specific strings
		VialBpos,
		VialBneg,
		NegativeFractionTube,
		
		LysisBufferNegativeFractionTube,

		// Separator strings - Error Messages
		ErrorFatal, 
		ErrorTerminate,
		ErrorTerminateQuestion,
		ErrorReportError,
		ErrorCantSchedule,

        RoboSepS,
        RoboSepS_1_0,
        RoboSepS_1_1,
        RoboSepS_1_2,
        RoboSep16,
        RoboSep16_1_2,

        ReagentCustom,
        ReagentNotUsed,
	}

	#endregion UI String Resource IDs


    public enum ProtocolFormat
    {
        RoboSepS_1_0,
        RoboSepS_1_1,
        RoboSepS_1_2,
        RoboSep16,
        RoboSep16_1_2,

    }


 	/// <summary>
	/// Common point of access for Tesla resource-only assemblies.
	/// </summary>
	/// <remarks>
	/// Singleton.
	/// </remarks>
    public class SeparatorResourceManager 
    {
        private static SeparatorResourceManager	theSeparatorResourceManager;
        private static ResourceManager			theSeparatorResources,
												theSeparatorFontResources,
												theStatusConfigResources,
												theErrorConfigResources;												

        // Members to help lookup resources -- we use regular expressions to match the type 
        // name at the end of "namespace.type".
        private static string					theSeparatorTypePatternString = @"\w+$";		
        private static Regex					theSeparatorTypePattern;
        private static char						theResourcesSeparator = '.';
        private static string					theResourcesDescriptionSuffix = theResourcesSeparator + "Description";

        #region Font Resources

        private static Font[]   theSeparatorFonts = new Font[(int)FontId.NUM_FONTS];

        private Font LookupFont(FontId fontId)
        {
            return theSeparatorFonts[(int)fontId];
        }

        public enum FontId
        {
			// Default
			ApplicationDefault,

            // Sample Processing page 
            SampleProcessingPageTitle,
			SampleGridColumnHeader,
			RunProgressInfo,
			ConfigurationQuadrantPrompt,
			ConfigurationQuadrantLocation,

            // Instrument Tasks page
            InstrumentTasksPageTitle,

            // Maintenance subpage
            MaintenanceSubpageTitle,
            MaintenanceListItem,

			Small,
			Smaller,

            NUM_FONTS
        } 

        public enum FontFamilyId
        {
            Default
        }    

        public enum FontSizeId
        {
            Default,

			// Sample Processing page
            SampleProcessingPageTitle,
			SampleGridColumnHeader,
			RunProgressInfo,
			ConfigurationQuadrantPrompt,
			ConfigurationQuadrantLocation,

			// Maintenance subpage
            MaintenanceSubpageTitle,

			Small,
			Smaller
        }

        private struct FontTableEntry
        {
            public FontFamilyId aFontFamilyId;
            public FontSizeId   aFontSizeId;

            public FontTableEntry(FontFamilyId fontFamilyId, FontSizeId fontSizeId)
            {
                aFontFamilyId = fontFamilyId;
                aFontSizeId = fontSizeId;
            }
        }

        private static readonly FontTableEntry[] theFontTable =
        {
			// Application default
			new FontTableEntry(FontFamilyId.Default,	FontSizeId.Default),

            // Sample Processing page
            // FontId.SampleProcessingPageTitle
            new FontTableEntry(FontFamilyId.Default,    FontSizeId.SampleProcessingPageTitle),
			// FontId.SampleGridColumnHeader
			new FontTableEntry(FontFamilyId.Default,	FontSizeId.SampleGridColumnHeader),
			// FontId.RunProgressInfo
			new FontTableEntry(FontFamilyId.Default,	FontSizeId.RunProgressInfo),
			// FontId.ConfigurationQuadrantPrompt
			new FontTableEntry(FontFamilyId.Default,	FontSizeId.ConfigurationQuadrantPrompt),
			// FontId.ConfigurationQuadrantLocation
			new FontTableEntry(FontFamilyId.Default,	FontSizeId.ConfigurationQuadrantLocation),

            // Instrument Tasks page
            // FontId.InstrumentTasksPageTitle
            new FontTableEntry(FontFamilyId.Default,    FontSizeId.SampleProcessingPageTitle),

            // Maintenance subpage
            // FontId.MaintenanceSubpageTitle
            new FontTableEntry(FontFamilyId.Default,	FontSizeId.MaintenanceSubpageTitle),
            // FontId.MaintenanceListItem
            new FontTableEntry(FontFamilyId.Default,	FontSizeId.Default),

			
			// Application small default
			new FontTableEntry(FontFamilyId.Default,	FontSizeId.Small),
			new FontTableEntry(FontFamilyId.Default,	FontSizeId.Smaller),
        };        

        #endregion Font Resources

        #region Resource access 

        // FUTURE: consider implementing a cache (hashtable?) based on the resource lookup
        // string.  You'd need to flush the cache if the thread culture changed, or have the
        // cache as thread-local storage (and then control the 'thread caches' individually).

        public static void GetSeparatorStateStrings(Type resourceType, 
            SeparatorState separatorState, out string stateName, out string stateDescription)
        {
            SeparatorResourceManager.GetInstance().LookupSeparatorStateStrings(resourceType, 
                separatorState, out stateName, out stateDescription);
        }

        public static string GetSeparatorString(StringId lookupKey)
        {
            return SeparatorResourceManager.GetInstance().
                LookupSeparatorResourceString(lookupKey);
        }

        public static Font GetFont(FontId lookupKey)
        {
            return SeparatorResourceManager.GetInstance().LookupFont(lookupKey);
        }

        public static string GetStatusMessageString(string key)
        {
            return SeparatorResourceManager.GetInstance().LookupStatusMessageString(key);
        }

        public static string GetErrorMessageString(string key)
        {
            return SeparatorResourceManager.GetInstance().LookupErrorMessageString(key);
        }

        public static SeparatorResourceManager GetInstance()
        {
            if (theSeparatorResourceManager == null)
            {
                try
                {
                    theSeparatorResourceManager = new SeparatorResourceManager();
                }
                catch
                {
                    theSeparatorResourceManager = null;
                }
            }
            return theSeparatorResourceManager;
        }

        private SeparatorResourceManager()
        {
            protocolFormat = ProtocolFormat.RoboSepS_1_2;
        }

        static SeparatorResourceManager()
        {
            // Create the RegEx object to perform type name pattern matching.						
            theSeparatorTypePattern = new Regex(theSeparatorTypePatternString);	

            // Load resources managers to access localized resources at run-time
            Assembly commonResourcesAssembly = System.Reflection.Assembly.Load("CommonResources");
            theSeparatorResources = new ResourceManager("Tesla.Common.Separator", commonResourcesAssembly);
            theSeparatorFontResources = new ResourceManager("Tesla.Common.SeparatorFonts", commonResourcesAssembly);
            theStatusConfigResources = new ResourceManager("Tesla.Common.statusConfig", commonResourcesAssembly);
            theErrorConfigResources = new ResourceManager("Tesla.Common.errorConfig", commonResourcesAssembly);

            // Initialise the font resources
            InitialiseFontTable();

        }

        private static void InitialiseFontTable()
        {
            Debug.Assert(((int)FontId.NUM_FONTS == theFontTable.GetLength(0)),
                "FontId.NUM_FONTS does not equal the number of fonts defined in the font table");
            for (int i = 0; i < (int)FontId.NUM_FONTS; ++i)
            {
                string fontFamily = SeparatorResourceManager.GetInstance().
                    LookupFontFamilyString(theFontTable[i].aFontFamilyId);
                int fontSize = SeparatorResourceManager.GetInstance().
                    LookupFontSize(theFontTable[i].aFontSizeId);
				if (fontSize != 0) 
				{
					Font separatorFont = new Font(fontFamily, fontSize);
					theSeparatorFonts[i] = separatorFont;
				}
            }
        }

        private void LookupSeparatorStateStrings(Type resourceType, 
            SeparatorState separatorState, out string stateName, out string stateDescription)
        {			
            string resourceLookupString = string.Empty;

            try
            {
                // Build up the resource base lookup string
                string resourcePrefix = theSeparatorTypePattern.Match(
                    resourceType.ToString()).Value + theResourcesSeparator;

                resourceLookupString = resourcePrefix + 
                    Enum.GetName(resourceType, separatorState);

                // Get the string for the SeparatorState itself
                stateName = theSeparatorResources.GetString(resourceLookupString);
            }
            catch
            {
                stateName = String.Empty;
            }

            try
            {
                // Get the string for the SeparatorState description
                stateDescription = theSeparatorResources.GetString(resourceLookupString + 
                    theResourcesDescriptionSuffix);
            }
            catch
            {
                stateDescription = String.Empty;
            }
        }

        private string LookupSeparatorResourceString(StringId lookupKey)
		{
		 return GetStringResource(theSeparatorResources, typeof(string), 
                Enum.GetName(typeof(StringId), lookupKey));
        }

        private string LookupFontFamilyString(FontFamilyId lookupKey)
        { 
            return GetStringResource(theSeparatorFontResources, typeof(FontFamily),
                Enum.GetName(typeof(FontFamilyId), lookupKey));
        }

        private int LookupFontSize(FontSizeId lookupKey)
        {
            int fontSize = 0;
            try
            {
                fontSize = int.Parse(
                    GetStringResource(theSeparatorFontResources, typeof(Int16),
                    Enum.GetName(typeof(FontSizeId), lookupKey)));
            }
            catch
            {
                fontSize = 0;
            }
            return fontSize;
        }

		private string LookupStatusMessageString(string key)
		{
			return GetStringResource(theStatusConfigResources, null, key);
		}

		private string LookupErrorMessageString(string key)
		{
			// Lookup the error message value for the given error key
			string strErrorMessage = string.Empty;
	
			try
			{
				strErrorMessage = GetStringResource(theErrorConfigResources, null, key);
			}
			catch
			{
				strErrorMessage = string.Empty;
			}

			// Format and return the localised result
			return strErrorMessage;
		}

		/// <summary>
		/// Generic string resource lookup.
		/// </summary>
		/// <param name="resourceManager"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		private string GetStringResource(ResourceManager resourceManager, 
			Type resourceType, string key)
		{			
			string resourceLookupString	= string.Empty;			
			string resourceString		= string.Empty;

			try
			{ 
				// Determine the resource prefix
				string resourcePrefix = resourceType == null ? string.Empty:
					theSeparatorTypePattern.Match(
						resourceType.ToString()).Value + theResourcesSeparator;
				// Determine the full resource key
				resourceLookupString = resourcePrefix + key;

				// Return the associated resource
				resourceString = resourceManager.GetString(resourceLookupString);
				
			}
			catch
			{
				resourceString = string.Empty;
			}

			return resourceString;
		}

		#endregion Resource access 

        #region name to string id
        public static StringId convertNameToStringID(string name)
        {
            StringId id = StringId.WasteTube;
            if (name == SeparatorResourceManager.GetSeparatorString(StringId.WasteTube))
            {
                id = StringId.WasteTube;
            }
            else if (name == SeparatorResourceManager.GetSeparatorString(StringId.SeparationTube))
            {
                id = StringId.SeparationTube;
            }
            else if (name == SeparatorResourceManager.GetSeparatorString(StringId.LysisBufferTube))
            {
                id = StringId.LysisBufferTube;
            }
            else if (name == SeparatorResourceManager.GetSeparatorString(StringId.NegativeFractionTube))
            {
                id = StringId.NegativeFractionTube;
            }
            else if (name == SeparatorResourceManager.GetSeparatorString(StringId.LysisBufferNegativeFractionTube))
            {
                id = StringId.LysisBufferNegativeFractionTube;
            }
            else if (name == SeparatorResourceManager.GetSeparatorString(StringId.VialA))
            {
                id = StringId.VialA;
            }
            else if (name == SeparatorResourceManager.GetSeparatorString(StringId.VialB))
            {
                id = StringId.VialB;
            }
            else if (name == SeparatorResourceManager.GetSeparatorString(StringId.VialBpos))
            {
                id = StringId.VialBpos;
            }
            else if (name == SeparatorResourceManager.GetSeparatorString(StringId.VialBneg))
            {
                id = StringId.VialBneg;
            }
            else if (name == SeparatorResourceManager.GetSeparatorString(StringId.VialC))
            {
                id = StringId.VialC;
            }
            else if (name == SeparatorResourceManager.GetSeparatorString(StringId.SampleTube))
            {
                id = StringId.SampleTube;
            }
            else if (name == SeparatorResourceManager.GetSeparatorString(StringId.QuadrantBuffer))
            {
                id = StringId.QuadrantBuffer;
            }
            else if (name == SeparatorResourceManager.GetSeparatorString(StringId.QuadrantBuffer34))
            {
                id = StringId.QuadrantBuffer34;
            }
            else if (name == SeparatorResourceManager.GetSeparatorString(StringId.QuadrantBuffer56))
            {
                id = StringId.QuadrantBuffer56;
            }
            return id;
        }
        #endregion

        #region protocol format to string
        public static string ProtocolFormatToFeatureSectionName(ProtocolFormat format)
        {
            string result = "";
            switch (format)
            {
                case ProtocolFormat.RoboSepS_1_0:
                case ProtocolFormat.RoboSepS_1_1:
                case ProtocolFormat.RoboSepS_1_2:
                    result = "FeatureList_RSS";
                    break;
                case ProtocolFormat.RoboSep16:
                case ProtocolFormat.RoboSep16_1_2:
                    result = "FeatureList_RS16";
                    break;

            }
            return result;
        }
        public static string ProtocolFormatToUserDisplayString(ProtocolFormat format)
        {
            string result = "";
            switch (format)
            {
                case ProtocolFormat.RoboSepS_1_0:
                    result = SeparatorResourceManager.GetSeparatorString(StringId.RoboSepS_1_0);
                    break;
                case ProtocolFormat.RoboSepS_1_1:
                    result = SeparatorResourceManager.GetSeparatorString(StringId.RoboSepS_1_1);
                    break;
                case ProtocolFormat.RoboSepS_1_2:
                    result = SeparatorResourceManager.GetSeparatorString(StringId.RoboSepS_1_2);
                    break;
                case ProtocolFormat.RoboSep16:
                    result = SeparatorResourceManager.GetSeparatorString(StringId.RoboSep16);
                    break;
                case ProtocolFormat.RoboSep16_1_2:
                    result = SeparatorResourceManager.GetSeparatorString(StringId.RoboSep16_1_2);
                    break;

            }
            return result;
        }
        public static ProtocolFormat StringToProtocolFormat(string s)
        {
            ProtocolFormat result = ProtocolFormat.RoboSepS_1_2;
            try
            {
                ProtocolFormat newValue = (ProtocolFormat)Enum.Parse(typeof(ProtocolFormat), s, true);
                if (Enum.IsDefined(typeof(ProtocolFormat), newValue))
                {
                    result = newValue;
                    Console.WriteLine("Converted '{0}' to {1}.", s, newValue.ToString());
                }
                else
                {
                    Console.WriteLine("{0} is not an underlying value of the ProtocolFormat enumeration.", s);
                }
            }
            catch (ArgumentException)
            {
                Console.WriteLine("{0} is not a member of the ProtocolFormat enumeration.", s);
            }
            return result;
        }


        public static bool isPlatformRS16()
        {
            return isPlatformRS16(SeparatorResourceManager.GetProtocolFormat());
        }

        public static bool isPlatformRS16(ProtocolFormat format)
        {
            bool result = false;
            switch (format)
            {
                case ProtocolFormat.RoboSep16:
                case ProtocolFormat.RoboSep16_1_2:
                    result = true;
                    break;

            }
            return result;
        }
        #endregion

        private ProtocolFormat protocolFormat;
        public static void SetProtocolFormat(ProtocolFormat type)
        {
            SeparatorResourceManager.GetInstance().protocolFormat = type;
        }
        public static ProtocolFormat GetProtocolFormat()
        {
            return SeparatorResourceManager.GetInstance().protocolFormat;
        }
        public static int GetMaxVolume()
        {
            /*
            int result = 0;
            int ROBOSEP_S_MAX_VOLUME_UL = 10000;
            int ROBOSEP_16_MAX_VOLUME_UL = 3000;
            //txtVolumeCommandAbsolute.Text = MAX_VOLUME_UL;
            switch (SeparatorResourceManager.GetInstance().protocolFormat)
            {
                case ProtocolFormat.RoboSepS_1_0:
                case ProtocolFormat.RoboSepS_1_1:
                case ProtocolFormat.RoboSepS_1_2:
                    result= ROBOSEP_S_MAX_VOLUME_UL;
                    break;
                case ProtocolFormat.RoboSep16:
                case ProtocolFormat.RoboSep16_1_2:
                    result= ROBOSEP_16_MAX_VOLUME_UL;
                    break;
            }
            return result;
             */
            return VolumeLimits.GetInstance().maximum_SampleVolume_ul;
        }



        public static string getLysisStringFromProtocolClass(ProtocolClass pc)
        {
            if (pc == ProtocolClass.WholeBloodPositive)
                return SeparatorResourceManager.GetSeparatorString(StringId.LysisBufferTube);
            else if (pc == ProtocolClass.WholeBloodNegative)
                return SeparatorResourceManager.GetSeparatorString(StringId.LysisBufferNegativeFractionTube);
            else
                return SeparatorResourceManager.GetSeparatorString(StringId.NegativeFractionTube);
        }
        public static string getVialBStringFromProtocolClass(ProtocolClass pc)
        {
            // for positive protocols use positive vial name
            if ((pc == ProtocolClass.Positive) || (pc == ProtocolClass.HumanPositive) ||
                (pc == ProtocolClass.MousePositive) || (pc == ProtocolClass.WholeBloodPositive))
                return SeparatorResourceManager.GetSeparatorString(StringId.VialBpos);
            // for negative protocols use negative vial name
            else if ((pc == ProtocolClass.Negative) || (pc == ProtocolClass.HumanNegative) ||
                (pc == ProtocolClass.MouseNegative) || (pc == ProtocolClass.WholeBloodNegative))
                return SeparatorResourceManager.GetSeparatorString(StringId.VialBneg);
            // else use normal bead vial name
            else
                return SeparatorResourceManager.GetSeparatorString(StringId.VialB);
        }
    }
}
