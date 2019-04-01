﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;

namespace GUI_Console
{
    public class IniFile
    {
        public string path;
        public string[] fileNames;
        private iniFile FileType;

        public enum iniFile
        {
            GUI,
            Language,
            Hardware,
            User,
            numIniFiles
        };

        [DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileStringW",
          SetLastError = true,
          CharSet = CharSet.Unicode, ExactSpelling = true,
          CallingConvention = CallingConvention.StdCall)]

        private static extern int GetPrivateProfileString(
          string lpAppName,
          string lpKeyName,
          string lpDefault,
          string lpReturnString,
          int nSize,
          string lpFilename);

        [DllImport("KERNEL32.DLL", EntryPoint = "WritePrivateProfileStringW",
          SetLastError = true,
          CharSet = CharSet.Unicode, ExactSpelling = true,
          CallingConvention = CallingConvention.StdCall)]

        private static extern int WritePrivateProfileString(
          string lpAppName,
          string lpKeyName,
          string lpString,
          string lpFilename);

        public IniFile(iniFile selectFile)
        {
            FileType = selectFile;
            fileNames = new string[(int)(iniFile.numIniFiles)] {
                "GUI.ini",
                "Language.ini",
                "hardware.ini",
                "UserInfo.ini"};

            if (selectFile != iniFile.User)
            {
                string configPath = RoboSep_UserDB.getInstance().sysPath + "config\\";
                path = configPath + fileNames[(int)FileType];
            }
            else
            {
                //string configPath = @"C:\Program Files (x86)\STI\RoboSep\protocols\";
                string configPath = RoboSep_UserDB.getInstance().sysPath + "protocols\\";
                path = configPath + fileNames[(int)FileType];
            }

            // LOG


            GenerateINI(FileType);
        }

        public void IniWriteValue(string Section, string Key, string Value)
        {
            // check for special characters \r\n
            if (Value.Split('\n').Length > 1)
            {
                string[] SplitString = Value.Split('\n');
                Value = string.Empty;
                for (int i = 0; i < SplitString.Length; i++)
                {
                    Value += i < (SplitString.Length - 1) ? SplitString[i] + (char)(182) : SplitString[i];
                }
            }

            WritePrivateProfileString(Section, Key, Value, this.path);
        }

        public string IniReadValue(string Section, string Key, string Value)
        {
            string result = new string(' ', 255);
            GetPrivateProfileString(Section, Key, "", result, 255, this.path);

            // test for empty string == no entry
            // write entry in case where no entry is present
            if (result[0] == '\0')
            {
                IniWriteValue(Section, Key, Value);
                return Value;
            }

            result = result.Split('\0')[0];
            
            // check for special characters that denote the '\n' character
            if (result.Split((char)(182)).Length > 1)
            {
                string[] SplitString = result.Split((char)(182));
                result = string.Empty;
                for (int i = 0; i < SplitString.Length; i++)
                {
                    result += i != (SplitString.Length-1) ? SplitString[i] + "\r\n" : SplitString[i];
                }
            }

            return result;
        }

        public string GetString( string key )
        {
            List<string> categoryList = IniGetCategories();
            List<List<string>> AllKeys = new List<List<string>>();

            // get all keys
            for (int i = 0; i < categoryList.Count; i++)
            {
                AllKeys.Add( IniGetKeys( categoryList[i] ) );
                for (int j = 0; j < AllKeys[i].Count; j++)
                {
                    if (AllKeys[i][j] == key)
                    {
                        string Value = IniReadValue(categoryList[i], AllKeys[i][j], "");
                        return Value.Split('\0')[0];
                    }
                }
            }
            return string.Empty;
        }

        public List<string> IniGetCategories()
        {
            string returnString = new string(' ', 65536);
            GetPrivateProfileString(null, null, null, returnString, 65536, this.path);
            List<string> result = new List<string>(returnString.Split('\0'));
            result.RemoveRange(result.Count - 2, 2);
            return result;
        }

        public List<string> IniGetKeys(string category)
        {
            string returnString = new string(' ', 32768);
            GetPrivateProfileString(category, null, null, returnString, 32768, this.path);
            List<string> result = new List<string>(returnString.Split('\0'));
            result.RemoveRange(result.Count - 2, 2);
            return result;
        }



        // Generate File if none exists
        private void GenerateINI(iniFile fileType)
        {
            // use functionality of IniReadValue to
            // check if value is present do nothing
            // if value is missing, store given value
            switch (FileType)
            {
                case iniFile.GUI:
                    IniReadValue("General", "Font", "Agency FB");
                    IniReadValue("General", "Language", "Default");
                    IniReadValue("General", "DefaultUserHDImageDirectory", "c:\\Share Data\\");
                    IniReadValue("General", "DefaultUtilityFilenames", "Util_GUI.exe");
                    IniReadValue("General", "DefaultUtilityTimeoutInSecond", "300");
                    IniReadValue("System", "EnableKeyboard", "false");
                    IniReadValue("System", "SharedBufferLotID", "true");
                    IniReadValue("System", "ExclusiveUseResultCheckForHighlight", "false");
                    IniReadValue("System", "DefaultHDDLowMarkInMegaByte", "1000");
                    IniReadValue("System", "FreeAxisAfterBarcodeScan", "true");
                    IniReadValue("About", "LastServiceDate", "00/0/0000");
                    IniReadValue("About", "NextServiceDate", "00/0/0000");
                    IniReadValue("About", "LastServicePerformedBy", "STI");
                    IniReadValue("About", "WarrantyStartDate", "00/0/0000");
                    IniReadValue("About", "WarrantyExpireDate", "00/0/0000");
                    IniReadValue("About", "WarrantyStatus", "Invalid");
                    IniReadValue("About", "AutoComputeWarrantyStatus", "true");
                    IniReadValue("About", "SupportPhone", "1-800-667-0322");
                    IniReadValue("About", "SupportEmail", "TechSupport @ STEMCELL.com");
                    IniReadValue("About", "SalesRepName", "Sales Rep");
                    IniReadValue("About", "SalesRepPhone", "1-800-667-0322");
                    IniReadValue("About", "SalesRepEmail", "sales.rep@STEMCELL.com");
                    IniReadValue("Logs", "msgLogWriteFail", "Unable to write to log file '");
                    IniReadValue("Report", "DefaultPdfZoomPercent", "50");
                    IniReadValue("FTP", "LoginID", "roboseptechsupport");
                    IniReadValue("FTP", "Password", "KS81h9Bc+mGSNdlfYedrww==");
                    IniReadValue("FTP", "Server Name/IP Address", "54.208.155.239");
                    IniReadValue("FTP", "Port", "21");
                    IniReadValue("NetworkORCA", "OrcaNumber", "1");
                    break;

                case iniFile.Language:
                    IniReadValue("General", "Ok", "OK");
                    IniReadValue("General", "Cancel", "Cancel");
                    IniReadValue("General", "Back", "Back");
                    IniReadValue("General", "Yes", "Yes");
                    IniReadValue("General", "No", "No");
                    IniReadValue("General", "Clear", "Clear");
                    IniReadValue("General", "Enter", "Enter");
                    IniReadValue("General", "Select", "Select");
                    IniReadValue("General", "Continue", "Continue");
                    IniReadValue("General", "Open", "Open");
                    IniReadValue("General", "Save", "Save");
                    IniReadValue("General", "Error", "Error");
                    IniReadValue("General", "Warning", "Warning");
                    IniReadValue("General", "lblSearch", "Search");
                    IniReadValue("General", "Wait", "Please Wait...");
                    IniReadValue("General", "WarrantyExpiring", "The warranty will expire next month."
                        + "  To renew your warranty contact your STEMCELL sales representative.  See the Help screen for more details.");
                    IniReadValue("General", "headerWarranty", "Warranty Notice");

                    IniReadValue("Home", "UserShutdown", "RoboSep-S Shutdown");
                    IniReadValue("Home", "msgShutDown", "Do you want to shut down the Windows Operating System?");
                    IniReadValue("Home", "headerRoboCon", "RoboCon");
                    IniReadValue("Home", "RoboConDisconnected", "RoboCon will be disconnected.");
                    IniReadValue("Home", "msgManageProtocols", "Select OK to add new protocols to the My Protocols tab. There are currently protocols assigned to a quadrant. "
                        + "To clear all protocols from assigned quadrants and enter the Protocols screen select OK.");
                    IniReadValue("Home", "headerManageProtocols", "Navigate to the All Protocols Screen");
                    IniReadValue("Home", "lblRunSamples", "RUN SAMPLES");
                    IniReadValue("Home", "lblRunProgress", "RUN PROGRESS");
                    IniReadValue("Home", "lblMaintenance", "MAINTENANCE");
                    IniReadValue("Home", "lblProtocols", "PROTOCOLS");
                    IniReadValue("Home", "lblPreferences", "PREFERENCES");
                    IniReadValue("Home", "lblReports", "REPORTS");
                    IniReadValue("Home", "lblShutDown", "SHUT DOWN");
                    IniReadValue("Home", "lblHelp", "HELP");

                    IniReadValue("Run Progress", "msgUnloading", "Please wait while the carousel unloads.");
                    IniReadValue("Run Progress", "msgHalt", "Are you sure you want to stop the current run?");
                    IniReadValue("Run Progress", "headerAbort", "Stop Run");
                    IniReadValue("Run Progress", "msgHalting", "Please wait while RoboSep-S stops the current run.");
                    IniReadValue("Run Progress", "headerPause", "Pause Run");
                    IniReadValue("Run Progress", "msgPause", "Are you sure you want to pause the current run?");
                    IniReadValue("Run Progress", "msgPausing", "Please wait while RoboSep-S reaches a safe state to pause.");
                    IniReadValue("Run Progress", "msgRunComplete1", "Run completed successfully at ");
                    IniReadValue("Run Progress", "msgRunComplete2", "Press the Unload button to identify the location of the isolated cells.");
                    IniReadValue("Run Progress", "msgPauseLastStep", "The run is about to finish. Please wait until it is completed.");
                    IniReadValue("Run Progress", "headerRunComplete", "Run Completed");
                    IniReadValue("Run Progress", "lblCurrent", "Current:");
                    IniReadValue("Run Progress", "lblPrevious", "Previous:");
                    IniReadValue("Run Progress", "lblElapsed", "Elapsed:");
                    IniReadValue("Run Progress", "lblEstimated", "Estimated:");
                    IniReadValue("Run Progress", "lblComplete", "Complete!");
                    IniReadValue("Run Progress", "lblPause", "Pause");
                    IniReadValue("Run Progress", "lblStop", "Stop");
                    IniReadValue("Run Progress", "lblAbort", "Abort");           // Do not change. Use for button text matching the description of a message
                    IniReadValue("Run Progress", "lblResume", "Resume");         // Do not change. Use for button text matching the description of a message
                    IniReadValue("Run Progress", "lblUnload", "Unload");
                    IniReadValue("Run Progress", "lblPauseRun", "Pause Run");
                    IniReadValue("Run Progress", "lblAbortRun", "Abort Run");
                    IniReadValue("Run Progress", "lblResumeRun", "Resume Run");
                    IniReadValue("Run Progress", "lblAbortDisable", "RoboSep-S is re-calibrating. Please wait until completed before stopping the run.");
                    IniReadValue("Run Progress", "headerRecovering", "RoboSep-S is Re-Calibrating");
                    IniReadValue("Run Progress", "lblPauseDisable", "RoboSep-S is re-calibrating. Please wait until completed before pausing the run.");
                    IniReadValue("Run Progress", "lblHomeDisable", "RoboSep-S is re-calibrating, please wait until after calibration is complete.");
                    IniReadValue("Run Progress", "headerLowDiskSpace", "Low Disk Spaces");
                    IniReadValue("Run Progress", "msgLowDiskSpace", "The hard drive '{0}' is running out of spaces ({1} MB). \n\nPlease free up some space.");
                    IniReadValue("Pause Resume", "headerTipStripError", "Tip Strip Failure");
                    IniReadValue("Pause Resume", "msgTipStripError", "A tip strip failure has occurred." + '\n' +
                            "Please select Resume to  prompt RoboSep-S to try stripping the tip again. " +
                            "If RoboSep-S cannot successfully strip the tip, " +
                            "manually remove the tip and replace it with a new tip.");

                    IniReadValue("Pause Resume", "headerTipStripRecovery", "Tip Strip Recovery");
                    IniReadValue("Pause Resume", "msgTipStripRecovery", "RoboSep-S has successfully recovered from a tip strip failure.");

                    IniReadValue("Run Samples", "lblName", "Protocol Name");
                    IniReadValue("Run Samples", "lblQuadrant", "Quadrant");
                    IniReadValue("Run Samples", "lblVolume", "Volume");
                    IniReadValue("Run Samples", "buttonScan", "Scan Carousel");
                    IniReadValue("Run Samples", "lblSearch", "Search");
                    IniReadValue("Run Samples", "lblProtocolName", "Protocol Name");
                    IniReadValue("Run Samples", "lblPType", "Type");
                    IniReadValue("Run Samples", "lblNumQuadrants", "Quadrants");
                    IniReadValue("Run Samples", "msgConfirmRun", "Run selected protocols?");
                    IniReadValue("Run Samples", "headerProtocolMixing", "Protocol Selection Error");
                    IniReadValue("Run Samples", "msgProtocolMixing", "Maintenance and separation protocols cannot run at the same time."
                        + "\nTo continue with the separation protocol, remove the maintenance protocol from the Run Samples screen.");
                    IniReadValue("Run Samples", "existingProtocolSelectionError", "Please discard the selected protocol first.");
                    IniReadValue("Run Samples", "headerMaintenancePresent", "Maintenance Protocol");
                    IniReadValue("Run Samples", "msgMaintenancePresent", "To select an alternate maintenance protocol return to the Settings screen.  "
                        + "\nTo select a separation protocol first remove the maintenance protocol from quadrant 1.");
                    IniReadValue("Run Samples", "msgSharing1", "You have selected quadrants ");
                    IniReadValue("Run Samples", "msgSharing2", " with the protocol ");
                    IniReadValue("Run Samples", "msgSharing3", ". Do you wish to use a single separation kit?");
                    IniReadValue("Run Samples", "msgOverload", "You have selected a combination of sample volumes and/or multiple protocols "
                        + "that exceeds the recommended limit for the reagent vials. To avoid the potential for reagent overflow during separation, "
                        + "do not use vials that contain > 1.1 mL of reagents.");
                    IniReadValue("Run Samples", "msgOverload2", "You have selected a combination of sample volumes and/or multiple protocols "
                        + "that exceeds the recommended limit for the reagent vials. To avoid the potential for reagent overflow during separation, "
                        + "please correct the following: ");
                    IniReadValue("Run Samples", "msgCarouselSpace", "This protocol needs at least two consecutive quadrants.");
                    IniReadValue("Run Samples", "headerCarouselSpace", "Consecutive Quadrants Not Available");
                    IniReadValue("Run Samples", "msgHydraulicLow", "The hydraulic fluid level is low. \nPlease fill the hydraulic fluid bottle with deionized water before continuing.");
                    IniReadValue("Run Samples", "msgResume", "Please wait while RoboSep-S resumes the run.");
                    IniReadValue("Run Samples", "headedFluidLevel", "Hydraulic Fluid Warning");
                    IniReadValue("Run Samples", "Confirm", "Confirm");      // Do not change. Use for button text matching the description of a message 
                    IniReadValue("Run Samples", "Ignore", "Ignore");        // Do not change. Use for button text matching the description of a message

                    IniReadValue("Protocols", "msgChangeUser", "Changing the user will remove protocols currently assigned to the carousel quadrants.");
                    IniReadValue("Protocols", "headerChangeUsr", "Change User");
                    IniReadValue("Protocols", "msgRefreshList", "Protocols have been included in your current run from the All Protocols tab. "
                        + "If you update your profile these protocols will be removed from your current run.");
                    IniReadValue("Protocols", "msgRefreshList2", "Remove Protocols?");
                    IniReadValue("Protocols", "msgLoadingProtocols", "Loading User Protocols...");
                    IniReadValue("Protocols", "msgIsRunning", "User profiles cannot be updated while a run is in progress.");
                    IniReadValue("Protocols", "msgNoProtocols", "No protocols are associated with this user profile. Add protocols to the My Protocols tab or select an alternate user.");
                    IniReadValue("Protocols", "headerNoProtocols", "No Protocols Available");
                    IniReadValue("Protocols", "headerOverwritePreviousProtocolSelection", "Quarant {0} is In Use");
                    IniReadValue("Protocols", "msgOverwritePreviousProtocolSelection", "The current quadrant is in use. Selecting this protocol will overwrite the protocol currently associated with this quadrant.");
                    IniReadValue("Protocols", "headerIsRunning", "Run in Progress");
                    IniReadValue("Protocols", "lblUserList", "User List");
                    IniReadValue("Protocols", "lblUsr", "user:");
                    IniReadValue("Protocols", "lblEditList", "Edit Protocol List");
                    IniReadValue("Protocols", "lblProtocolEditor", "Protocol Editor");
                    IniReadValue("Protocols", "lblPresets", "Preset Lists");
                    IniReadValue("Protocols", "lblHuman", "Human");
                    IniReadValue("Protocols", "lblMouse", "All Mouse");
                    IniReadValue("Protocols", "lblWB", "Whole Blood");
                    IniReadValue("Protocols", "lblFull", "Full List");
                    IniReadValue("Protocols", "lblUserList", "User Profile");
                    IniReadValue("Protocols", "lblLoadUser", "Load Selected User");
                    IniReadValue("ProtocolList", "lblAllProtocols", "All Protocols");
                    IniReadValue("ProtocolList", "msgValidUser", "No protocols are associated with this user profile. Add protocols to the My Protocols tab to validate this user profile.");
                    IniReadValue("ProtocolList", "headerValidUser", "No Protocols Available");
                    IniReadValue("ProtocolList", "headerNoSelectedProtocols", "No Protocol Selected");
                    IniReadValue("ProtocolList", "msgSelectUserProtocols", "There is no protocol selected. Please select a protocol from the My Protocols tab.");
                    IniReadValue("ProtocolList", "headerProtocolsList", "Protocols List Modified");
                    IniReadValue("ProtocolList", "msgProtocolsListModified", "The protocol lists have been modified. Please save all changes before importing protocols from a USB drive.");
                    IniReadValue("ProtocolList", "headerMultipleSelectedProtocols", "Multiple Protocols Selected");
                    IniReadValue("ProtocolList", "msgSelectOneUserProtocols", "There are multiple protocols selected. Please select only one protocol from the My Protocols tab.");
                    IniReadValue("ProtocolList", "headerLoadingUserProtocols", "Loading User Protocols");
                    IniReadValue("ProtocolList", "headerUpdatingInProgress", "Updating In Progress...");
                    IniReadValue("ProtocolList", "msgMyProtocolsUnsaved", "My Protocols tab has been modified. Continuing will discard these changes.");
                    IniReadValue("ProtocolList", "headerMyProtocolsUnsaved", "Discard Changes");
                    IniReadValue("ProtocolList", "headerSelectProtocol", "Select Protocol Directory");
                    IniReadValue("ProtocolList", "headerDeleteProtocols", "Delete Protocols");
                    IniReadValue("ProtocolList", "msgDeleteProtocols", "These protocols will be permanently deleted. \nAre you sure you want to delete the protocols?");
                    IniReadValue("ProtocolList", "headerProtocolsUndeletable", "Protocol Undeletable");
                    IniReadValue("ProtocolList", "msgProtocolsUndeletable", "Protocol '{0}' cannot be deleted. \nIt is associated with one or more presets.");
    
                    IniReadValue("UserList", "lblUserProtocols", "User Protocols");
                    IniReadValue("UserList", "lblSaveList", "Save List");
                    IniReadValue("UserList", "lblRemoveProtocol", "Remove Protocol");
                    IniReadValue("UserList", "lblEditList", "Edit List");
                    IniReadValue("UserList", "lblMyList", "My List");

                    IniReadValue("System", "msgSeparationProtocolsPresent", "Running a maintenance protocol will remove all separation protocols from the quadrants.");
                    IniReadValue("System", "headerSeparationProtocolPresent", "Run Maintenance Protocol");
                    IniReadValue("System", "msgInsertUSB", "Please insert a USB drive with at least 100 Mb of free space.");
                    IniReadValue("System", "headerZipping", "Generating Diagnostic Package");
                    IniReadValue("System", "msgWaitForZip", "Please wait while RoboSep-S generates the Diagnostic Package.");
                    IniReadValue("System", "headerExportingDiagPackage", "Exporting Diagnostic Package");
                    IniReadValue("System", "msgWaitExportingDiagPackage", "Please wait while RoboSep-S exports the Diagnostic Package.");
                    IniReadValue("System", "CreateDiagnostic", "This process can take up to 5 minutes to complete.");
                    IniReadValue("System", "lblSystemOpt", "System Options");
                    IniReadValue("System", "lblKeyboard", "Enable Touch Keyboard");
                    IniReadValue("System", "lblLidSensor", "Enable Lid Sensor");
                    IniReadValue("System", "lblLiquidSensor", "Enable Liquid Level\nSensor");
                    IniReadValue("System", "lblStartTimer", "Enable Delayed Start");
                    IniReadValue("System", "lblMaintenanceProtocol", "System Protocols");
                    IniReadValue("System", "lblBasicPrime", "Basic Prime");
                    IniReadValue("System", "lblHomeAll", "Home All");
                    IniReadValue("System", "lblServiceButton", "SERVICE MENU");
                    IniReadValue("System", "lblDiagnostic", "DIAGNOSTIC PACKAGE");
                    IniReadValue("System", "lblExitToDeskTop", "EXIT TO DESKTOP");
                    IniReadValue("System", "msgEmbedDLLFail", "Software Error: Failed to retrieve embedded DLL. Contact Technical Support.");
                    IniReadValue("System", "headerChooseDestination", "Saving Diagnostic Package");
                    IniReadValue("System", "msgChooseDestination", "Select where to save the Diagnostic Package.");
                    IniReadValue("System", "headerConnectingFTPserver", "Connecting To FTP Server");
                    IniReadValue("System", "msgConnectingFTPserver", "Please wait while RoboSep-S connects to the FTP server.");
                    IniReadValue("System", "headerConnectFTP", "Connecting to FTP server");
                    IniReadValue("System", "msgFailedToConnectFTP", "RoboSep-S failed to connect to the FTP server.");
                    IniReadValue("System", "headerFailedToConnectFTP", "Failed to Connect to FTP Server");
                    IniReadValue("System", "msgFailedToDisconnectFTP", "RoboSep-S failed to disconnect to the FTP server.");
                    IniReadValue("System", "headerFailedToDisconnectFTP", "Failed to Disconnect to FTP Server");
                    IniReadValue("System", "msgFailedToDecryptPassword", "RoboSep-S failed to decrypt the password to connect to the FTP server.");
                    IniReadValue("System", "headerFailedToDecryptPassword", "Failed to Connect to FTP Server");
                    IniReadValue("System", "msgExitSoftware", "Are you sure you want to exit the RoboSep-S software?");
                    IniReadValue("System", "headerExitSoftware", "Exit RoboSep-S");
                    IniReadValue("System", "headerFailedToZipDiagnosticPackage", "Failed to Generate Diagnostic Package");
                    IniReadValue("System", "msgFailedToZipDiagnosticPackage", "RoboSep-S failed to generate the diagnostic package.");
                    IniReadValue("System", "headerSaveDiagnosticPackageUSB", "Saving Diagnostic Package");
                    IniReadValue("System", "msgSaveDiagnosticPackageUSBOK", "The Diagnostic Package was successfully saved to the USB drive.");
                    IniReadValue("System", "headerDiagnosticPackageSendToFTP", "Sending Diagnostic Package to FTP Server");
                    IniReadValue("System", "msgDiagnosticPackageSendToFTPOK", "The Diagnostic Package was successfully sent to the FTP server.");
                    IniReadValue("System", "headerLaunchUtility", "Launching Utility");
                    IniReadValue("System", "headerUtilityTimeout", "Utility Session Timeout");
                    IniReadValue("System", "msgUtilityTimeout", "The session for utility '{0}' is timeout. Timeout is '{1}' seconds.");

                    IniReadValue("Resources", "msgCarouselSetup", "Have you removed all tube and vial caps?");
                    IniReadValue("Resources", "headerStartRun", "Start Run");
                    IniReadValue("Resources", "lblReagentLotID", "Reagent Lot ID: ");
                    IniReadValue("Resources", "lblSampleID", "Sample ID: ");
                    IniReadValue("Resources", "lblBufferLotID", "Buffer Lot ID: ");
                    IniReadValue("Resources", "lblRunProto", "Run Protocols");
                    IniReadValue("Resources", "msgCloseLid", "Please close the lid and select Resume to continue the run.");
                    IniReadValue("Resources", "headerCloseLid", "Close Lid");
                    IniReadValue("Resources", "MagneticParticlesVial", "Magnetic Particles");
                    IniReadValue("Resources", "CockTailVial", "Cocktail");
                    IniReadValue("Resources", "AntibodyVial", "Antibody");
                    IniReadValue("Resources", "SampleVial", "Sample Tube");
                    IniReadValue("Resources", "TipRack", "Tip Rack");
                    IniReadValue("Resources", "SeparationTube", "Separation Tube");
                    IniReadValue("Resources", "WasteFraction", "Waste Fraction");
                    IniReadValue("Resources", "NegativeFraction", "Negative Fraction");
                    IniReadValue("Resources", "BufferBottle", "Buffer Bottle");
                    IniReadValue("Resources", "BarcodeVialErrorDesc", "Failed to scan barcode for quadrant # '{0}', Vial '{1}'");
                    IniReadValue("Resources", "BarcodeQuadrantErrorDesc", "Failed to scan barcode for quadrant # {0}");
                    IniReadValue("Resources", "headerReagentLotID", "Missing Reagent Lot ID");
                    IniReadValue("Resources", "ReagentLotIDMissing", "Please enter missing Reagent Lot ID for quadrant # '{0}'.\nPlease enter missing Reagent Lot ID.");
                    IniReadValue("Resources", "ReagentLotIDMissing1", "'{0}' Lot ID for quadrant # '{1}' is empty.\nPlease enter '{2}' Reagent Lot ID.");
                    IniReadValue("Resources", "SampleVialLotIDEmpty", "'Please enter missing sample ID for quadrant # '{0}'.");
                    IniReadValue("Resources", "headerEmptySampleLotID", "Missing Sample ID");
                    IniReadValue("Resources", "headerBarcodeScannerInitFailed", "Barcode Scanner Failed");
                    IniReadValue("Resources", "BarcodeScannerInitFailed", "RoboSep-S failed to initialize the barcode scanner.");
                    IniReadValue("Resources", "Retry", "Retry");
                    IniReadValue("Resources", "Skip", "Skip");
                    IniReadValue("Resources", "BarcodeScanningForVial", "Scanning barcode for quadrant # '{0}', Vial '{1}' ...");
                    IniReadValue("Resources", "BarcodeScanningInProgress", "Barcode scanning in progress...");
                    IniReadValue("Resources", "headerReagentLimit", "Volume Exceeded Warning");
                    IniReadValue("Resources", "ExceedReagentLimit", "Transport volume exceeds the reagent vial capacity. Please lower start volume or re-adjust protocol settings.");

                    IniReadValue("BarcodeScan", "Scan", "Scan");
                    IniReadValue("BarcodeScan", "Rescan", "Rescan");
                    IniReadValue("BarcodeScan", "EmptyBarcodeIndicator", "---");
                    IniReadValue("BarcodeScan", "ScanInProgress", "Barcode scanning in progress...");
                    IniReadValue("BarcodeScan", "Quadrant", "Quadrant");
                    IniReadValue("BarcodeScan", "BarcodeScanError", "Error");
                    IniReadValue("BarcodeScan", "BarcodeScanNotInit", "Barcode Scanner not Initialized");
                    IniReadValue("BarcodeScan", "AllQuadrants", "All Quadrants");
                    IniReadValue("BarcodeScan", "headerAbortBarcodeScan", "Stop Barcode Scanning");
                    IniReadValue("BarcodeScan", "AbortBarcodeScan", "Stopping barcode scanning.");
                    IniReadValue("BarcodeScan", "BarcodeCloseLid", "Please close the lid and retry.");   
                    IniReadValue("BarcodeScan", "NoneDetectedError", "NONE_DETECTED");
                    IniReadValue("BarcodeScan", "CustomVialWarning", "CUSTOM");
                    IniReadValue("BarcodeScan", "DeviceIOError", "DEVICE_IO_ERROR");
                    IniReadValue("BarcodeScan", "IncorrectBarcodeError", "INCORRECT BARCODE");
                    IniReadValue("BarcodeScan", "NoBarcodeShouldBeReadWarning", "NO BARCODE SHOULD BE READ");
                    

                    IniReadValue("uiLog", "msgLogFileError", "Unable to write to log file");
                    IniReadValue("User DB", "msgXMLUserProfile", "Failed to save file to User1.udb");
                    IniReadValue("User DB", "msgAddProtocol1", "Failed to add protocol ");
                    IniReadValue("User DB", "msgAddProtocol2", "'as protocol does not exist in the current protocol directory.  To add the protocol "
                        + "from a USB drive, select the USB drive button on your edit protocol list > all protocols page.");
                    IniReadValue("User DB", "msgUSBFail", "The USB drive was not detected. Please insert a USB drive and try again.");
                    IniReadValue("User DB", "headerUSBFail", "USB Drive Error");
                    IniReadValue("Service", "msgLoginError1", "User name should not include symbols and cannot exceed 11 characters.");
                    IniReadValue("Service", "headerLoginError", "Invalid User Name");
                    IniReadValue("Service", "msgLoginError2", "Incorrenctly typed User Name");
                    IniReadValue("Service", "msgLoginError3", "User Name entered is too short.");
                    IniReadValue("Service", "headerLoginError3", "User Name Too Short");
                    IniReadValue("Service", "Password1", "stemcell");
                    IniReadValue("Service", "Password2", "stemcell");
                    IniReadValue("Service", "Password3", "stemcell");

                    IniReadValue("About", "lblSoftware", "Software");
                    IniReadValue("About", "lblInstrumentControl", "Instrument Control:");
                    IniReadValue("About", "lblLastService", "Last Service Date:");
                    IniReadValue("About", "lblNextService", "Next Service Date:");
                    IniReadValue("About", "lblLastServicePerformedBy", "Last Service Performed By:");
                    IniReadValue("About", "lblWarrantyStartDate", "Warranty Start Date:");
                    IniReadValue("About", "lblWarrantyExpireDate", "Warranty Expire Date:");
                    IniReadValue("About", "lblWarrantyStatus", "Warranty Status:");
                    IniReadValue("About", "lblInstrument", "Instrument");
                    IniReadValue("About", "lblSerial", "Serial Number:");
                    IniReadValue("About", "lblServiceAddress", "Network Address:");
                    IniReadValue("About", "lblSupport", "STEMCELL Support");
                    IniReadValue("About", "lblSalesRep", "Sales Representative:");
                    IniReadValue("About", "lblValidThrough", "Valid Through ");
                    IniReadValue("About", "lblExpired", "Expired");
                    IniReadValue("About", "msgInvalidDate", "Invalid Date");
                    IniReadValue("About", "msgNoServiceDate", "No Service calls recorded.");
                    IniReadValue("About", "msgNoRecordedDate", "No date recorded.");
                    IniReadValue("About", "tabAbout", "About RoboSep-S");
                    IniReadValue("About", "tabHelpVids", "Help Videos");
                    IniReadValue("About", "tabHelpVideoCannotBePlayed", "Video cannot be played when the device is in progressing mode.");

                    IniReadValue("VideoLogs", "tabVideoErrorLog", "Video Error Log");
                    IniReadValue("VideoLogs", "tabVideoLog", "Video Log");

                    IniReadValue("DiagnosticPackage", "lblUSB", "USB");
                    IniReadValue("DiagnosticPackage", "lblFTPServer", "FTP Server");
                    IniReadValue("DiagnosticPackage", "lblIncludeWindowsEvents", "Include Windows Events");

                    IniReadValue("File Browser", "msgOverWrite", "' already exists in destination folder.\n\nOverwrite file?");
                    IniReadValue("File Browser", "msgNoOverWrite", "' already exists in destination folder.\n\nPlease make sure the file does not exist.");
                    IniReadValue("File Browser", "CopyFailHeader", "Overwrite File");
                    IniReadValue("File Browser", "headerSelectReports", "End of Run Reports");
                    IniReadValue("File Browser", "msgSelectReports", "Select a report to be copied.");
                    IniReadValue("File Browser", "headerCopyFail", "Error In Copying File");
                    IniReadValue("File Browser", "msgCopyFail1", "Failed to copy file '");
                    IniReadValue("File Browser", "msgCopyFail2", "file does not exist in the current directory");
                    IniReadValue("File Browser", "msgCopyFail3", ".  Destination folder '");
                    IniReadValue("File Browser", "msgCopyFail4", "' does not exist.");
                    IniReadValue("File Browser", "SkipForAllConflicts", "Skip this message for all conflicts");
                    IniReadValue("File Browser", "lblFilter", "Filter For:");
                    IniReadValue("File Browser", "lblFileBrowser", "File Browser");
                    IniReadValue("File Browser", "lblAll", "All");
                    IniReadValue("File Browser", "lblNewDirectoryName", "New Directory Name");
                    IniReadValue("File Browser", "msgWaitCopyingFiles", "Please wait while file copying is in progress.");
 
                    IniReadValue("RunLog", "lblReports2", "Reports");
                    IniReadValue("RunLog", "SaveReport", "Please enter a USB drive.");
                    IniReadValue("RunLog", "headerSaveReport", "Save Report");
                    IniReadValue("RunLog", "headerSaveUSB", "USB Drive Error");
                    IniReadValue("RunLog", "msgCopyComplete", "File copying completed.\n\n");
                    IniReadValue("RunLog", "msgCopyComplete2", " files successfully copied.");
                    IniReadValue("RunLog", "headerCopy", "Files Copied");
                    IniReadValue("RunLog", "lblRunLog", "Run Statistics Log");
                    IniReadValue("RunLog", "lblDateTime", "Date / Time");
                    IniReadValue("RunLog", "lblUser", "User");
                    IniReadValue("RunLog", "lblRunInfo", "Run Info");
                    IniReadValue("RunLog", "lblViewReport", "View Report");
                    IniReadValue("RunLog", "lblSaveReports", "Save Reports to USB");
                    IniReadValue("RunLog", "msgWaitSaveReport", "Please wait while the report is saved to the USB drive.");
                    IniReadValue("RunLog", "msgViewReportFailed", "Failed to view report. Adobe reader may not be installed.");
                    IniReadValue("RunLog", "msgViewReportFailed2", "Failed to find the Adobe reader executable file");
                    IniReadValue("RunLog", "headerViewReportFailed", "End of Run Report Failure");
                    IniReadValue("RunLog", "headerSaveReportUSB", "Save Report");
                    IniReadValue("RunLog", "msgGeneratingReport", "Please wait while RoboSep-S generates the report.");
                    IniReadValue("RunLog", "headerGeneratingReport", "End of Run Report");

                    IniReadValue("QuadrantSharing", "QS1", "You have selected quadrants ");
                    IniReadValue("QuadrantSharing", "QS2", " with the protocol ");
                    IniReadValue("QuadrantSharing", "QS3", ".\n\nDo you wish to use a single separation kit?");
                    IniReadValue("QuadrantSharing", "headerQS", "Share Reagents");

                    IniReadValue("ProtocolSelection", "headerLoadingProtocols", "Loading Protocols");
                    IniReadValue("ProtocolSelection", "LoadingAllProtocols", "Loading protocols...");
                    IniReadValue("ProtocolSelection", "msgFailedtoLoadProtocol", "Failed to read the selected protocol.  "
                    + "Try to add this protocol again to the My Protocols tab."
                    + "\n\nIf issue persists, contact Technical Support.");

                    IniReadValue("UserDB", "headerSaveUserDBFail", "Save Error");
                    IniReadValue("UserDB", "saveUserDBFail", "Failed to save file to User1.udb");
                    IniReadValue("UserDB", "ProtocolAddFail1", "Failed to add protocol \n'");   // file name of failed protocol entered between ProtocolAddFail 1&2
                    string temp = "'\nas protocol does not exist in the current"
                            + " protocol directory. To add the protocol from a USB drive,"
                            + " select the USB drive button on your edit protocol list > all protocols page.";
                    IniReadValue("UserDB", "ProtocolAddFail2", temp);
                    IniReadValue("UserDB", "headerProtocolNotFound", "Protocol Not Found");
                    IniReadValue("NumPad", "NumPadmsg1", "Volume not within valid range.");
                    IniReadValue("NumPad", "NumPadmsg2", "Volume out of Range");
                    IniReadValue("NumPad", "NumPadmsg3", "Enter a volume between ");
                    IniReadValue("NumPad", "NumPadmsg4", " mL and ");

                    IniReadValue("UserSelect", "lblLoadUsr", "Load User");
                    IniReadValue("UserSelect", "lblSelectIcon", "Select Icon");
                    IniReadValue("UserSelect", "lblCloneUser", "Copy User");
                    IniReadValue("UserSelect", "lblMoreUsers", "More Users");
                    IniReadValue("UserSelect", "lblEnterUserName", "Please enter user name.");
                    IniReadValue("UserSelect", "lblDefault", "Default");
                    IniReadValue("UserSelect", "lblAccept", "Accept");
                    IniReadValue("UserSelect", "lblNewUser", "New User");
                    IniReadValue("UserSelect", "lblSelectUser", "Select User");
                    IniReadValue("UserSelect", "lblUserName", "User Name");

                    IniReadValue("FormUserSelect", "lblSelectUserToEdit", "Select the User to be Edited");
                    IniReadValue("FormUserSelect", "lblSelectUserToClone", "Select the User to be Copied");
                    IniReadValue("FormUserSelect", "lblSelectUserFromList", "There is no user selected.  Select a user from the list.");
                    IniReadValue("FormUserSelect", "headerSelectUserFromList", "No User Selected");
                    IniReadValue("FormUserSelect", "lblSelectUserToDelete", "Select the User to be Deleted");

                    IniReadValue("UserLoginNew", "lblUserIcon", "User Icon");
                    IniReadValue("UserLoginNew", "lblUserName", "User Name");
                    IniReadValue("UserLoginNew", "lblIsEmpty", "is Empty");
                    IniReadValue("UserLoginNew", "lblIsTooLong", "is Too Long");
                    IniReadValue("UserLoginNew", "lblUserNameMaxLength", "The maximum number of characters in user name is {0}.\n");
                    IniReadValue("UserLoginNew", "headerReservedChar", "Invalid User Name");
                    IniReadValue("UserLoginNew", "lblContainsIllegalChars", "contains reserved character '{0}'.\n");
                    IniReadValue("UserLoginNew", "lblIsNotLoaded", " is Not Loaded");
                    IniReadValue("UserLoginNew", "lblLoad", "Load");
                    IniReadValue("UserLoginNew", "lblIsNotValid", " is Not Valid");
                    IniReadValue("UserLoginNew", "lblPreviousImageLoaded", "Previous Image was Loaded");
                    IniReadValue("UserLoginNew", "lblOverwritePreviousImage", "Overwrite Previous Image");
                    IniReadValue("UserLoginNew", "headerInsertUSB", "Insert USB Drive");
                    IniReadValue("UserLoginNew", "msgInsertUSB2", "Please insert a USB drive.");
                    IniReadValue("UserLoginNew", "msgUserInvalidCharacters", "User name contains invalid character '{0}'. Please choose another user name.");
                    IniReadValue("UserLoginNew", "msgUserOccupied", "User name is not valid or is already used . Please choose another user name.");
                    IniReadValue("UserLoginNew", "headerUserInvalid", "Change User Name");
                    IniReadValue("UserLoginNew", "msgNameUsedByPresets", "User name is not valid or is already used. Please choose another user name.");
                    IniReadValue("UserLoginNew", "headerSelectImageFile", "Select Image File");
                     

                    // User preferences dialog
                    // Note: variable name is the same as the key name.
                    // Help string must have "Help" as suffix attached to the variable name. It is hardcoded for ease of retrieval.

                    // GENERAL
                    IniReadValue("UserPreferences", "headerSavingInProgress", "Saving In Progress...");
                    IniReadValue("UserPreferences", "headerLoadingUserPreferences", "Loading User Preferences");
                    IniReadValue("UserPreferences", "msgLoadingUserPreferences", "Loading user preferences in progress...");

                    // USER
                    IniReadValue("UserPreferences", "UseAbbreviationsForProtocolNames", "Use abbreviations for protocol names.");
                    IniReadValue("UserPreferences", "UseAbbreviationsForProtocolNamesHelp", "The abbreviated names will fit within the horizontal space of the touchscreen.");
                    IniReadValue("UserPreferences", "LoadMRUProtocolsAtStartUp", "Load the most recently used protocols at start-up.");
                    IniReadValue("UserPreferences", "LoadMRUProtocolsAtStartUpHelp", "RoboSep-S will remember the most recent protocols and sample volumes a user last ran and will automatically " 
                                  + "assign them to the Run Samples screen. This setting is helpful for users who routinely perform the same separation protocols.");
                    IniReadValue("UserPreferences", "UseFirstTabToDisplayAllProtocolsList", "Use the first tab to display the All Protocols tab.");
                    IniReadValue("UserPreferences", "UseFirstTabToDisplayAllProtocolsListHelp", "The arrangements of the My Protocols and All Protocols tabs are reversed such that All Protocols is listed first.");
                    IniReadValue("UserPreferences", "EnableProgressBarDetailsView", "Enable the detailed progress bar display toggle during sample runs.");
                    IniReadValue("UserPreferences", "EnableProgressBarDetailsViewHelp", "Enable the user to toggle between the standard and detailed view of the progress bar during sample runs.");
                    IniReadValue("UserPreferences", "SwitchToMyProtocolListAfterAddingAProtocol", "Automatically switch to the My Protocols tab after adding a protocol.");
                    IniReadValue("UserPreferences", "SwitchToMyProtocolListAfterAddingAProtocolHelp", "After adding protocols from the All Protocols tab, the view automatically switches to the My Protocols tab.");
                    IniReadValue("UserPreferences", "SkipResourcesScreen", "Skip the  Carousel Loading  screen.");
                    IniReadValue("UserPreferences", "SkipResourcesScreenHelp", "Bypass the guided Carousel Loading screen to immediately run samples.");
                    IniReadValue("UserPreferences", "ShowBarcodesUserInterface", "Show the barcode user interface screen.");
                    IniReadValue("UserPreferences", "ShowBarcodesUserInterfaceHelp", "Show the barcode user interface screen for scanning, manual entry or updating barcodes.");
                    IniReadValue("UserPreferences", "EnableAutoScanBarcodes", "Enable autoscanning of barcodes.");
                    IniReadValue("UserPreferences", "EnableAutoScanBarcodesHelp", "Automatically start scanning barcodes when entering the barcode user interface screen.");

                    // DEVICE
                    IniReadValue("UserPreferences", "DisableCamera", "Privacy Setting - Disable carousel video camera.");
                    IniReadValue("UserPreferences", "DisableCameraHelp", "This setting will inactivate the carousel video camera.");
                    IniReadValue("UserPreferences", "DeleteVideoLogFiles", "Privacy Settings - Delete video camera log files.");
                    IniReadValue("UserPreferences", "DeleteVideoLogFilesHelp", "This setting deletes all the saved carousel video camera log files.");
                    IniReadValue("UserPreferences", "EnableBeepSwitch", "Enable beep sound when a separation is paused.");
                    IniReadValue("UserPreferences", "EnableBeepSwitchHelp", "A beep will sound intermittently when a separation protocol is paused.");

                    // User Console
                    IniReadValue("UserConsole", "headerModifyProtocol", "Protocol {0}");
                    IniReadValue("UserConsole", "msgModifyProtocol", "Protocol '{0}' has been {1}. Reloading protocols will remove these protocols from their assigned quadrants.");
                    IniReadValue("UserConsole", "headerBarcodeInit", "Barcode Scanner Error");
                    IniReadValue("UserConsole", "BarcodeInitFailed", "Failed to initialize barcode scanner.");
                    IniReadValue("UserConsole", "headerPassword", "Enter Password");
                    IniReadValue("UserConsole", "msgEnterPassword", "Please enter your password.");

                    // Service Utilities
                    IniReadValue("Util_GUI", "headerSaveGUIConfig", "Save GUI Configuration");
                    IniReadValue("Util_GUI", "msgSaveConfigAndExit", "The GUI Configuration has been saved successfully. \n\nDo you want to exit the utility?");
                    IniReadValue("Util_GUI", "headerSaveConfig", "Save Configuration");
                    IniReadValue("Util_GUI", "msgSaveConfig", "The Configuration has been saved successfully.");
                    IniReadValue("Util_GUI", "headerExitUtil", "Exit Utility");
                    IniReadValue("Util_GUI", "msgExitUtil", "Do you want to exit the utility?");
                    IniReadValue("Util_GUI", "headerInvalidDate", "Invalid Date");
                    IniReadValue("Util_GUI", "msgInvalidDate", "The value '{0}' you enter for the variable '{1}' is not of the right format '{2}'.");
                    IniReadValue("Util_GUI", "headerInvalidBool", "Invalid Boolean Data");
                    IniReadValue("Util_GUI", "msgInvalidBool", "The value '{0}' you enter for the variable '{1}' is not a boolean data (true/false).");
                    IniReadValue("Util_GUI", "headerInvalidInterger", "Invalid Interger Number");
                    IniReadValue("Util_GUI", "msgInvalidInterger", "The value '{0}' you enter for the variable '{1}' is not a number.");
                    IniReadValue("Util_GUI", "headerInvalidFloat", "Invalid Floating Point Number");
                    IniReadValue("Util_GUI", "msgInvalidFloat", "The value '{0}' you enter for the variable '{1}' is not a floating point number.");
                    IniReadValue("Util_GUI", "headerInvalidDouble", "Invalid Double Number");
                    IniReadValue("Util_GUI", "msgInvalidFloat", "The value '{0}' you enter for the variable '{1}' is not a number of the type double.");
                    break;

                default:
                    break;
            }
        }
    }
}