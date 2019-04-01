using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Forms;
using System.Configuration;
using System.Management;

using Tesla.Common;

using Invetech.ApplicationLog;

namespace GUI_Controls
{
    public class UserDetails
    {
        //private Dictionary<string, string> library;
        private NameValueCollection library;
        private List<string> SectionNames;
        private List<int[]> SectionIndices;
        private string filepath;
        private static int iSystemArchitecture = -1;
        private const string SystemPath32 = "C:\\Program Files\\STI\\RoboSep\\";//config\\";
        private const string SystemPath64 = "C:\\Program Files (x86)\\STI\\RoboSep\\";//config\\";
        private const string UserImageSuffix = "Image";

      
        public static int getOSArchitecture()
        {
            int nRet = 32;
            // if value has already been grabbed.  return stored value
            if (iSystemArchitecture > 0)
            {
                return iSystemArchitecture;
            }
            // first time checking os architecture.  Store value to iSystemArchitecture
            else
            {
                try
                {
                    iSystemArchitecture = Utilities.getOsInfo();
                    return iSystemArchitecture;
                }
                catch (Exception)
                {
                   
                }
                return nRet;
            }
        }

        public static int OS_Bit
        {
            set
            {
                iSystemArchitecture = value;
            }
        }

        private UserDetails(string setINI)
        {
            // describe file path for ini
            filepath = getFilePath(setINI);
            // fill library from ini
            //library = new Dictionary<string, string>();
            library = new NameValueCollection();
            SectionNames = new List<string>();
            SectionIndices = new List<int[]>();
            createLibrary();
        }

        private UserDetails(string setINI, string loginUser)
        {
            // describe file path for ini
            filepath = getFilePathEx(setINI, loginUser);
            // fill library from ini
            //library = new Dictionary<string, string>();
            library = new NameValueCollection();
            SectionNames = new List<string>();
            SectionIndices = new List<int[]>();
            createLibrary();
        }


        private static string getBasePath()
        {
            int osBit = getOSArchitecture();
            iSystemArchitecture = osBit;
            string fpath = null;

#if (False)
                    fpath = Application.StartupPath.ToString();
                    string temp = "";
                    string remove = "separator\\bin\\Debug";
                    for (int i = 0; i < fpath.Length - remove.Length; i++)
                        temp += fpath[i];
                    fpath = temp + "GUI_Controls\\bin\\Debug\\GUI.ini";
                    // !! HardCode
                    fpath = @"C:\Program Files (x86)\STI\RoboSep\config\GUI.ini";
#else
            switch (osBit)
            {
                case 32:
                    fpath = SystemPath32;//+ "GUI.ini";
                    break;
                case 64:
                    fpath = SystemPath64;// + "GUI.ini";
                    break;
            }
#endif

            return fpath;
        }


        private static string getFilePath(string INI)
        {
            string fpath = null;
            fpath = getBasePath();

            switch (INI)
            {
                case "GUI":
                    fpath += "config\\GUI.ini";
                    if (!File.Exists(fpath))
                    {
                        File.WriteAllLines(fpath, GUIiniDefault());
                    }
                    break;
                case "USER":
                    //fpath = nvc.Get("USER");
                    fpath += "protocols\\UserInfo.ini";
                     
                    if (!File.Exists(fpath))
                    {
                        string[] lines = new string[] {
                        "//",
                        "// Notes go here ",
                        "// ",
                        ""};
                        File.WriteAllLines(fpath, lines);
                    }
                    break;
                case "DEFAULT_USER_PREFERENCES":
                    fpath += "config\\UserPreferences.ini";
                    break;
                default:
                    break;
            }
            return fpath;
        }

        private static string getFilePathEx(string INI, string loginUser)
        {
            string fBasePath = getBasePath();
            string fullFileNamePath = null; 
            switch (INI)
            {
                case "PREFERENCES":
                    fullFileNamePath = string.Format("{0}users\\{1}.ini", fBasePath, loginUser);
                    if (!File.Exists(fullFileNamePath))
                    {
                        string defaultUserPreferenceFileNamePath = getFilePath("DEFAULT_USER_PREFERENCES");
                        if (!string.IsNullOrEmpty(defaultUserPreferenceFileNamePath))
                        {
                            File.Copy(defaultUserPreferenceFileNamePath, fullFileNamePath);
                        }
                    }
                    break;

                default:
                    break;

            }
            return fullFileNamePath;
        }


        private static string[] GUIiniDefault()
        {
            string[] tempString = new string[] {
            @"// lines that begin with // are not read",
            "",
            "[General]",
            "font = Agency FB",
            "backColour = 233,233,233",
            "forecolour = 62,6,69",
            "",
            @"// Run Progress Window",
            "msgUnloading = Unloading Carousel...",
            "msgHalt = Are you sure you want to stop the current run?",
            "msgHalting = Halting current run",
            "msgPause = Are you sure you want to pause the run currently in progress?",
            "msgPausing = The instrument is pausing, please wait while the instrument reaches a safe state to pause. This can take up to 90 seconds",
            "msgRunComplete1 = Batch run completed successfully at ",
            "msgRunComplete2 = Click Unload button on Run Samples window to begin new run",
            "",
            @"// Run Samples Window",
            "msgConfirmRun = Run selected protocols?",
            "msgProtocolMixing = Maintenance protocol selected. Maintenance and Separation protocols can not be included in the same run. ",
	        "/tTo continue with Separation protocol selection, remove Maintenance protocol from quadrant 1.",
            "msgMaintenancePresent = To select alternate Maintenance protocol return to System window.  ",
	        "/nTo select Separation protocols first remove Maintenance protocol from quadrant 1.",
            "msgSharing1 = You have selected quadrants ",
            "msgSharing2 =  with the protocol ",
            "msgSharing3 = . Do you wish to use a single reagent kit?",
            "msgOverload = You have selected a combination of sample volumes and/or multiple protocols that exceeds the recommended limit for the reagent vials. If greater than 1.1 mL is loaded into the reagent vials, then there is the potential for reagent overflow during pipetting.",
            "msgOverload2 = You have selected a combination of sample volumes and/or multiple protocols that exceeds the recommended limit for the reagent vials. Please correct the following: ",
            "msgCarouselSpace = not enough consecutive quadrants available to accomodate this protocol, possibly re-organize to make space",
            "msgHydraulicLow = Hydraulic fluid level is low.  Top up fluid before continuing.",
            "",
            "// Protocols window",
            "msgChangeUser = There are protocols selected that are associated with the current User.  Changing users will remove these protocols from your Run Samples window.",
            "msgOverwritePreviousProtocolSelection = The current quadrant is in use. Select protocol will overwrite the previous protocol associated with this quadrant in your Run Samples window.",
            "",
            @"// Resources Window",
            "msgCarouselSetup = All required carousel resources are set up and ready for Separation?",
            "",
            @"// uiLog file",
            "msgLogFileError = Unable to write to log file",
            "",
            @"// User DB",
            "msgXMLUserProfile = Failed to save file to User1.udb",
            "msgAddProtocol1 = Failed to add protocol",
            "msgAddProtocol2 = 'as protocol does not exist in current protocol directory.  To re-add protocol from your USB, click on the USB load button on your edit protocol list > all protocols page.",
            "msgUSBFail = USB drive not detected.  Insert drive and try again",
            "",
            @"// System",
            "msgShutDown = Are you sure you want to shut down the RoboSep-S system? ",
            "",
            @"// Service Login Window",
            "msgLoginError1 = User name should not include numbers, symbols, or punctuation",
            "msgLoginError2 = Incorrenctly typed User Name",
            "msgLoginError3 = User Name entered is not long enough",
            "",
            @"// File Browser",
            "msgOverWrite = ' already exists in destination folder.	Overwrite file?",
            "msgCopyFail1 = Failed to copy file '",
            "msgCopyFail2 =  file does not exist in current directory",
            "",
            "[SERVICE]",
            "password1 = stemcell",
            "password2 = stemcell",
            "password3 = stemcell",
            "",
            "[SYSTEM]",
            "EnableKeyboard = false",
            "",
            "[About]",
            "LastServiceDate = 5/10/2011",
            "ReccomendedServiceDate = 5/4/2012",
            "",
            "[640X480]",
            "",
            "[800X600]",
            };
            return tempString;
        }

        // Grabs lines from .ini file
        private void createLibrary()
        {
            //int count = 0;
            if (!File.Exists(filepath))
            {
                // log exception
                // display error msg
            }
            else
            {// when file exists
                try
                {
                    StreamReader sr = new StreamReader(filepath, true);
                    char[] charSeparators = new char[] { '=' };
                    string tempstring = "";
                    while (!sr.EndOfStream)
                    {
                        try
                        {
                            tempstring = sr.ReadLine();

                            if (!(tempstring.Length == 0 || tempstring == null || tempstring == string.Empty))
                            {
                                if (!tempstring[0].Equals('/') && !tempstring[0].Equals('#') && !tempstring[0].Equals('[') && tempstring.Length > 5)
                                {
                                    // split string by = sign
                                    string[] parts = tempstring.Split(charSeparators, 2, StringSplitOptions.RemoveEmptyEntries);
                                    if (parts.Length > 1)
                                    {
                                        string key = parts[0].Trim();
                                        string value = parts[1].Trim();

                                        // add item to library
                                        library.Add(key, value);
                                        if (0 < SectionIndices.Count )
                                            SectionIndices[SectionIndices.Count - 1][1] = library.Count - SectionIndices[SectionIndices.Count - 1][0];
                                    }
                                    else
                                    {
                                        if (library.Count > 0)
                                        {
                                            string temp = library.Get(library.GetKey(library.Count - 1));
                                            temp += "\r\n\r\n";
                                            temp += parts[0][0] == '\t' ? parts[0].Split('\t')[1] : parts[0];
                                            library.Set(library.GetKey(library.Count - 1), temp);
                                        }
                                    }
                                }
                                // Create Sections
                                else if (tempstring[0].Equals('['))
                                {
                                    // beginning of a section marks end of previous section
                                    // set index of last section
                                    string sectionName = "";
                                    for (int i = 1; i < tempstring.Length - 1; i++)
                                    {
                                        sectionName += tempstring[i];
                                    }
                                    SectionNames.Add(sectionName);
                                    SectionIndices.Add(new int[2] { library.Count, 0 });
                                }
                            }
                        }
                        catch
                        {
                            // readline got a blank line
                        }
                    }
                    sr.Close();
                    
                }
                catch
                {
                    // log exception
                }
                
            }
        }

        public static void ReadSection(string token, List<string> SectionNames, List<int[]> SectionIndices, NameValueCollection library)
        {
            if (!(token.Length == 0 || token == null || token == string.Empty))
            {
                if (!token[0].Equals('/') && !token[0].Equals('[') && token.Length > 5)
                {
                    // split string by = sign
                    string[] parts = token.Split('=');
                    if (parts.Length > 1)
                    {
                        string key = "";
                        string value = "";
                        // create key string 
                        //for (int i = 0; i < parts[0].Length - 1; i++)
                        //{ key += parts[0][i]; }
                        key = parts[0].Trim();
                        // create value string
                        //for (int i = 1; i < parts[1].Length; i++)
                        //{ value += parts[1][i]; }
                        value = parts[1].Trim();

                        // add item to library
                        library.Add(key, value);
                        SectionIndices[SectionIndices.Count - 1][1] = library.Count - SectionIndices[SectionIndices.Count - 1][0];
                    }
                    else
                    {
                        string temp = library.Get(library.GetKey(library.Count - 1));
                        temp += "\r\n\r\n";
                        temp += parts[0][0] == '\t' ? parts[0].Split('\t')[1] : parts[0];
                        library.Set(library.GetKey(library.Count - 1), temp);
                    }
                }
                // Create Sections
                else if (token[0].Equals('['))
                {
                    // beginning of a section marks end of previous section
                    // set index of last section
                    string sectionName = "";
                    for (int i = 1; i < token.Length - 1; i++)
                    {
                        sectionName += token[i];
                    }
                    SectionNames.Add(sectionName);
                    SectionIndices.Add(new int[2] { library.Count, 0 });
                }
            }
        }

        public static string getValue(string INI, string key)
        {
            try
            {
                UserDetails ini = new UserDetails(INI);
                string value = ini.library.Get(key);
                return value;
            }
            catch
            {
                // log exeeption
                return null;
            }
        }

        public static void setValue(string INI, string key, string value)
        {
            // set file path
            string iniPath = getFilePath(INI);

            // set value
            string[] lines = File.ReadAllLines(iniPath, System.Text.Encoding.UTF8);
            // find line that needs to be replaced
            // place all lines into a list<string>
            for (int i=0; i<lines.Length;i++)
            {
                try
                {
                    string temp2 = "";
                    int length = key.Length;
                    if (lines[i].Length < length)
                        length = lines[i].Length;
                    for (int j = 0; j < length; j++)
                    {
                        temp2 += lines[i][j];
                    }
                    if (temp2 == key)
                    {
                        lines[i] = key + " = " + value;
                    }
                }
                finally
                { // do nothing
                }
            }
            // write text to file
            File.WriteAllLines(iniPath, lines);
        }

        public static void InsertEntry(string INI, string item, string value)
        {
            if (string.IsNullOrEmpty(INI) || string.IsNullOrEmpty(item))
                return;

            
            // get file path
            string iniPath = getFilePath(INI);
            string tempValue = "";
            if (!string.IsNullOrEmpty(value))
            {
                tempValue = value;
                tempValue.Trim();
            }


            // set value
            string[] lines = File.ReadAllLines(iniPath, System.Text.Encoding.UTF8);
            List<string> lst = new List<string>();
            lst.AddRange(lines);
            int len = item.Length;
            int index = 0;
            string[] parts;
            bool bFindDuplicated = false;
            int lastIndex = -1;
     
            // check for duplicates
            do
            {
                index = lst.FindIndex(index, x => { return (!string.IsNullOrEmpty(x) && x.Length >= len && x.ToLower().Contains(item.ToLower())); });
                if (0 <= index)
                {
                    parts = lst[index].Split('=');
                    if (parts.Length > 1 && parts[0].Trim().ToLower() == item.ToLower())
                    {
                        if (!string.IsNullOrEmpty(parts[1]) && parts[1].Trim().Length > 0)
                        {
                            if (tempValue.ToLower() == parts[1].Trim().ToLower())
                            {
                                bFindDuplicated = true;
                            }
                        }
                    }
                    lastIndex = index++;
                }

            } while (0 <= index && !bFindDuplicated);

            if (bFindDuplicated)
                return;

            string temp = item;
            temp += " = ";
            temp += value;
            if (lastIndex < 0)
            {
                lst.Add(temp);
            }
            else
            {
                lst.Insert(lastIndex + 1, temp);
            }
   
            // write text to file
            File.WriteAllLines(iniPath, lst.ToArray());
        }

        public static void DeleteEntry(string INI, string item, string value)
        {
            if (string.IsNullOrEmpty(INI) || string.IsNullOrEmpty(item))
                return;

            string tempValue = "";
            if (!string.IsNullOrEmpty(value))
            {
                tempValue = value;
                tempValue.Trim();
            }

            // get file path
            string iniPath = getFilePath(INI);

            // get value
            string[] lines = File.ReadAllLines(iniPath, System.Text.Encoding.UTF8);
            List<string> lst = new List<string>();
            lst.AddRange(lines);

            int len = item.Length;
            int index = 0;
            string[] parts;

            do
            {
                index = lst.FindIndex(index, x => { return (!string.IsNullOrEmpty(x) && x.Length >= len && x.ToLower().Contains(item.ToLower())); });
                if (0 <= index)
                {
                    parts = lst[index].Split('=');
                    if (parts.Length > 1 && parts[0].Trim().ToLower() == item.ToLower())
                    {
                        if (!string.IsNullOrEmpty(parts[1]) && parts[1].Trim().Length > 0)
                        {
                            if (tempValue.ToLower() == parts[1].Trim().ToLower())
                            {
                                // Find item
                                lst.RemoveAt(index);
                                break;
                            }
                        }
                    }
                    index++;
                }

            } while (0 <= index);

            // write text to file
            File.WriteAllLines(iniPath, lst.ToArray());
        }

        public static void addSection(string INI, string[] addLines)
        {
            string iniPath = getFilePath(INI);
            
            // append current ini file
            try
            {
                if (File.Exists(iniPath))
                {
                    StreamWriter sw = new StreamWriter(iniPath, true);
                    foreach (string s in addLines)
                        sw.WriteLine(s);
                    sw.Close();
                }
            }
            catch
            {
                // do nothing
            }
        }

        public static void saveSection(string INI, string sect, string[] content)
        {
            // get file path
            string iniPath = getFilePath(INI);
            saveSectionData(iniPath, sect, content);
        
        }

        public static bool saveSectionEx(string INI, string sect, string userName, string[] content)
        {
            // get file path
            string iniPath = getFilePathEx(INI, userName);
            return saveSectionData(iniPath, sect, content);
        }

        public static bool saveSectionData(string iniPath, string sect, string[] content)
        {
            if (string.IsNullOrEmpty(iniPath) || string.IsNullOrEmpty(sect) || content == null)
                return false;

            // read all lines from current ini
            bool bRet = true;
            try
            {
                string[] Lines;
                try
                {
                    Lines = File.ReadAllLines(iniPath);
                }
                catch
                {
                    // log exception
                    Lines = new string[0];
                    bRet = false;
                }

                // find section to be "saved"
                string sectionName = "[" + sect + "]";
                int[] sectionRange = new int[2];
                // find range of section
                for (int i = 0; i < Lines.Length; i++)
                {
                    if (Lines[i] == string.Empty || Lines[i] == null)
                        Lines[i] = "";
                    if (Lines[i] == sectionName)
                    {
                        sectionRange[0] = i;
                        int tempcount = 0;
                        while ((i + tempcount++) < (Lines.Length - 1))
                        {
                            if (Lines[i + tempcount].Length > 0)
                            {
                                if (Lines[i + tempcount][0] == '[')
                                {
                                    sectionRange[1] = i + tempcount - 1;
                                    break;
                                }
                            }
                        }
                        if (sectionRange[1] < sectionRange[0])
                            sectionRange[1] = Lines.Length;

                    }
                }

                // compile lines
                int rangeLength = sectionRange[1] - sectionRange[0];
                List<string> linesComp = new List<string>();

                for (int i = 0; i <= sectionRange[0]; i++)
                {
                    linesComp.Add(Lines[i]);
                }
                for (int i = 0; i < content.Length; i++)
                {
                    linesComp.Add(content[i]);
                }
                for (int i = sectionRange[1] +1; i < Lines.Length; i++)
                {
                    linesComp.Add(Lines[i]);
                }

                // make linescomp into string
                string[] newLines = new string[linesComp.Count];
                for (int i = 0; i < linesComp.Count; i++)
                {
                    newLines[i] = linesComp[i];
                }

                try
                {
                    // write back to file
                    File.WriteAllLines(iniPath, newLines);
                }
                catch
                {
                    // log exception
                    // Failed to write section to ... .ini
                    bRet = false;
                }
            }
            catch
            {
                // log exception
                bRet = false;
            }
            return bRet;
        }

        public static void removeSection(string INI, string sectName)
        {
            try
            {
                string filePath = UserDetails.getFilePath(INI);
                if (File.Exists(filePath))
                {
                    string sectionString = "[" + sectName + "]";
                    string[] lines = File.ReadAllLines(filePath);

                    // find appropriate section
                    int sectionStart = -1;
                    int sectionEnd = -1;

                    for (int lineNumber = 0; lineNumber < lines.Length; lineNumber++)
                    {
                        if (lines[lineNumber] == sectionString)
                        {
                            sectionStart = lineNumber;
                        }
                        else if (lines[lineNumber].StartsWith("[") && sectionStart > 0)
                        {
                            sectionEnd = lineNumber - 1;
                            break;
                        }
                    }
                    sectionEnd = sectionEnd == -1 && sectionStart > -1 ? lines.Length - 1 : sectionEnd;

                    // remove section
                    List<string> appendedLines = new List<string>();
                    for (int lineNumber = 0; lineNumber < lines.Length; lineNumber++)
                    {
                        if (lineNumber < sectionStart || lineNumber > sectionEnd)
                            appendedLines.Add(lines[lineNumber]);
                    }
                    lines = new string[appendedLines.Count];
                    for (int i = 0; i < appendedLines.Count; i++)
                        lines[i] = appendedLines[i];

                    File.Delete(filePath);
                    File.WriteAllLines(filePath, lines);
                }
            }
            catch
            {

            }
        }

        public static NameValueCollection getSection(string INI, string sectName)
        {
            try
            {
                UserDetails ini = new UserDetails(INI);
                for (int i = 0; i < ini.SectionNames.Count; i++)
                {
                    if (ini.SectionNames[i] == sectName)
                    {
                        // is the section being searched for
                        // add elements between sectionIndices
                        NameValueCollection nvc = new NameValueCollection();
                        for (int j = 0; j < ini.SectionIndices[i][1]; j++)
                        {
                            string key = ini.library.AllKeys[ini.SectionIndices[i][0] + j];
                            nvc.Add(key, ini.library.Get(key));
                        }
                        return nvc;
                    }
                }
                return null;
            }
            catch
            {
                // log exception
                return null;
            }
        }

        public static NameValueCollection getSectionEx(string INI, string sectName, string loginUser)
        {
            try
            {
                UserDetails ini = new UserDetails(INI, loginUser);
                for (int i = 0; i < ini.SectionNames.Count; i++)
                {
                    if (ini.SectionNames[i] == sectName)
                    {
                        // is the section being searched for
                        // add elements between sectionIndices
                        NameValueCollection nvc = new NameValueCollection();
                        for (int j = 0; j < ini.SectionIndices[i][1]; j++)
                        {
                            string key = ini.library.AllKeys[ini.SectionIndices[i][0] + j];
                            nvc.Add(key, ini.library.Get(key));
                        }
                        return nvc;
                    }
                }
                return null;
            }
            catch
            {
                // log exception
                return null;
            }
        }

        public static string[] getAllSections(string INI)
        {
            try
            {
                //string iniPath = getFilePath(INI);
                UserDetails ini = new UserDetails(INI);
                string[] tempArray = new string[ini.SectionNames.Count];
                for (int i = 0; i < ini.SectionNames.Count; i++)
                {
                    tempArray[i] = ini.SectionNames[i];
                }
                Array.Sort(tempArray);
                return tempArray;
            }
            catch
            {
                // log exception
                return null;
            }
        }

        public static bool deleteUserImageFile(string selectedUser)
        {
            if (string.IsNullOrEmpty(selectedUser))
                return false;

            string systemPath = getBasePath();
            string userImageFolder = systemPath + "images";

            System.Collections.Specialized.NameValueCollection userItems = UserDetails.getSection("USER", selectedUser);

            //enumerate the keys
            string temp;
            string[] parts;
            string directoryName;
            string UserImagePrefix = selectedUser.Trim() + UserImageSuffix;

            foreach (string key in userItems)
            {
                if (key.ToLower() == UserImagePrefix.ToLower())
                {
                    temp = userItems[key];
                    if (string.IsNullOrEmpty(temp))
                        continue;

                    parts = temp.Split(',');
                    for (int i = 0; i < parts.Length; i++)
                    {
                        if (string.IsNullOrEmpty(parts[i]))
                            continue;

                        if (parts[i].StartsWith("\""))
                            parts[i] = parts[i].Remove(0, 1);

                        if (parts[i].EndsWith("\""))
                            parts[i] = parts[i].Remove(parts[i].Length - 1);

                        if (parts[i].StartsWith("\\"))
                            parts[i] = parts[i].Remove(0, 1);

                        if (parts[i].EndsWith("\\"))
                            parts[i] = parts[i].Remove(parts[i].Length - 1);


                        directoryName = Path.GetDirectoryName(parts[i]);
                        if (string.IsNullOrEmpty(directoryName))
                            continue;

                        if (directoryName.ToLower() != userImageFolder.ToLower())
                            continue;

                        if (File.Exists(parts[i]))
                        {
                            File.Delete(parts[i]);
                            break;
                        }
                    }
                }
             }
            return true;
        }

        public static bool GetDefaultUserAndDevicePreferences(Dictionary<string, bool> dictUserPreferences, Dictionary<string, bool> dictDevicePreferences)
        {
            return LoadDefaultUserPreferencesFromSysConfig(dictUserPreferences, dictDevicePreferences);
        }

        public static bool LoadDefaultUserPreferencesFromSysConfig(Dictionary<string, bool> dictDefaultUserPreferences, Dictionary<string, bool> dictDefaultDevicePreferences)
        {
            if (dictDefaultUserPreferences == null || dictDefaultDevicePreferences == null)
                return false;

            NameValueCollection nvcUserSection = null;
            NameValueCollection nvcDevicesSection = null;
            string logMSG = "";
            try
            {
                nvcUserSection = getSection("DEFAULT_USER_PREFERENCES", "USER");
                nvcDevicesSection = getSection("DEFAULT_USER_PREFERENCES", "DEVICE");
            }
            catch (Exception ex)
            {
                // LOG
                LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);                               
                return false;
            }

            dictDefaultUserPreferences.Clear();
            dictDefaultDevicePreferences.Clear();
            GetPreferences(nvcUserSection, dictDefaultUserPreferences);
            GetPreferences(nvcDevicesSection, dictDefaultDevicePreferences);
            return true;
        }

        public static bool LoadUserPreferencesFromUserConfig(string userName, Dictionary<string, bool> dictUserPreferences, Dictionary<string, bool> dictDevicePreferences)
        {
            if (string.IsNullOrEmpty(userName) || dictUserPreferences == null || dictDevicePreferences == null)
                return false;
            
            string systemPath = getBasePath();
            string targetDestination = systemPath + "users\\";

            if (!Directory.Exists(targetDestination))
                Directory.CreateDirectory(targetDestination);

            NameValueCollection nvcUserSection = null;
            NameValueCollection nvcDevicesSection = null;
            string logMSG = "";
            try
            {
                nvcUserSection = getSectionEx("PREFERENCES", "USER", userName);
                nvcDevicesSection = getSectionEx("PREFERENCES", "DEVICE", userName);
            }
            catch (Exception ex)
            {
                // LOG
                LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);                                               
                return false;
            }

            dictUserPreferences.Clear();
            dictDevicePreferences.Clear();
            GetPreferences(nvcUserSection, dictUserPreferences);
            GetPreferences(nvcDevicesSection, dictDevicePreferences);
            return true;
        }

        private const string TextLowerCaseYes = "yes";
        private const string TextLowerCaseTrue = "true";

        private static bool GetPreferences(NameValueCollection nvc, Dictionary<string, bool> dict)
        {
            if (nvc == null || nvc.Count == 0)
                return false;

            //enumerate the keys
            string temp;
            bool bValue;
            foreach (string key in nvc)
            {
                temp = nvc[key];
                if (string.IsNullOrEmpty(temp))
                    continue;

                bValue = false;
                if (temp.ToLower() == TextLowerCaseYes || temp.ToLower() == TextLowerCaseTrue)
                {
                    bValue = true;
                }
                dict.Add(key, bValue);
            }
            return true;
        }

        public static bool SaveUserPreferences(string userName, Dictionary<string, bool> dictUserPreferences, Dictionary<string, bool> dictDevicePreferences)
        {
            if (string.IsNullOrEmpty(userName) || dictUserPreferences == null || dictDevicePreferences == null)
                return false;

            string systemPath = getBasePath();
            string targetDestination = systemPath + "users\\";

            if (!Directory.Exists(targetDestination))
                Directory.CreateDirectory(targetDestination);

            // LOG
            string logMSG = "saving user preferences for user " + userName;
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);             

            bool bRet1, bRet2;
            bRet1 = SavePreferencesSection("USER", userName, dictUserPreferences);
            bRet2 = SavePreferencesSection("DEVICE", userName, dictDevicePreferences);
            return (bRet1 && bRet2);
        }

        private static bool SavePreferencesSection(string sectionName, string loginUser, Dictionary<string, bool> dictPreferences)
        {
            if (string.IsNullOrEmpty(sectionName) || string.IsNullOrEmpty(loginUser) || dictPreferences == null)
                return false;

            string[] preferencesForINI = new string[dictPreferences.Count + 1];

            if (dictPreferences.Count == 0)
                return false;

            
                IDictionaryEnumerator aEnumerator = dictPreferences.GetEnumerator();
                int i = 0;
                string [] parts = new string[2];
                bool bValue;
                while (aEnumerator.MoveNext())
                {
                    bValue = (bool)aEnumerator.Value;
                    parts[0] = (string)aEnumerator.Key;
                    parts[1] = bValue?  "yes" : "no";
                  
                    preferencesForINI[i++] = string.Join("=", parts);
                }

                // Write to the file
                return saveSectionEx("PREFERENCES", sectionName, loginUser, preferencesForINI);
          }


        public static bool DeleteUserPreferences(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return false;

            string systemPath = getBasePath();
            string fullFileNamePath = systemPath + "users\\";

            if (!Directory.Exists(fullFileNamePath))
                return false;

            // LOG
            string logMSG = "removing user preferences for user " + userName;
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);             

            fullFileNamePath += userName;
            fullFileNamePath += ".ini";

            if (!File.Exists(fullFileNamePath))
            {
                return false;
            }

            File.Delete(fullFileNamePath);
            return true;
        }

        public static bool RenameUserPreferencesFile(string oldUserName, string newUserName)
        {
            if (string.IsNullOrEmpty(oldUserName) || string.IsNullOrEmpty(newUserName))
                return false;

            string systemPath = getBasePath();
            string fullFileNamePath = systemPath + "users\\";

            if (!Directory.Exists(fullFileNamePath))
                return false;

            string oldfullFileNamePath = fullFileNamePath + oldUserName;
            oldfullFileNamePath += ".ini";

            if (!File.Exists(oldfullFileNamePath))
            {
                return false;
            }

            // LOG
            string logMSG = String.Format("renaming user preferences file name from {0}.ini to {1}.ini", oldUserName, newUserName);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);             
            string newfullFileNamePath = fullFileNamePath + newUserName;
            newfullFileNamePath += ".ini";

            if (File.Exists(newfullFileNamePath))
            {
                logMSG = String.Format("Failed to rename file from {0}.ini to {1}.ini. File {2}.ini is already existed.", oldUserName, newUserName, newUserName);
                //  (logMSG);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);             
                return false;
            }

            try
            {
                File.Move(oldfullFileNamePath, newfullFileNamePath);
            }
            // Catch any exception if a file cannot be accessed
            // e.g. due to security restriction
            catch (Exception ex)
            {
                logMSG = String.Format("Failed to rename file from {0}.ini to {1}.ini. Exception: {2}.", oldUserName, newUserName, ex.Message);
                //  (logMSG);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                             
                return false;
            }
            return true;
        }


     }
 }
