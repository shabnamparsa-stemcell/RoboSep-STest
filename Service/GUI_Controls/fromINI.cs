using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Forms;
using System.Configuration;
using System.Management;

namespace GUI_Controls
{
    public class fromINI
    {
        //private Dictionary<string, string> library;
        private NameValueCollection library;
        private List<string> SectionNames;
        private List<int[]> SectionIndices;
        private string filepath;
        private static int iSystemArchitecture = -1;
        private const string SystemPath32 = "C:\\Program Files\\STI\\RoboSep\\";//config\\";
        private const string SystemPath64 = "C:\\Program Files (x86)\\STI\\RoboSep\\";//config\\";

        public static int getOSArchitecture()
        {
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
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2",
                        "SELECT * FROM Win32_Processor");

                    foreach (ManagementObject queryObj in searcher.Get())
                    {
                        iSystemArchitecture = Convert.ToInt32(queryObj["DataWidth"]);
                        //!!Hardcode
                        //iSystemArchitecture = 32;
                        return iSystemArchitecture;
                        
                    }
                }
                catch (Exception e)
                {
                    return 32;
                }
                return 32;
            }
        }

        private fromINI(string setINI)
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

        private static string getFilePath(string INI)
        {
            NameValueCollection nvc = (NameValueCollection) 
                ConfigurationSettings.GetConfig("OperatorConsole/ConsoleConfiguration");

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
                        case 32 :
                            fpath = SystemPath32;//+ "GUI.ini";
                            break;
                        case 64 :
                            fpath = SystemPath64;// + "GUI.ini";
                            break;
                    }
#endif

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
            }
            return fpath;
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
            "msgHalt = Are you sure you want to abort the current run?",
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
            "msgCarouselSpace = not enough consecutive quadrants available to accomodate this protocol, possibly re-organize to make space",
            "msgHydraulicLow = Hydraulic fluid level is low.  Top up fluid before continuing.",
            "",
            "// Protocols window",
            "msgChangeUser = There are protocols selected that are associated with the current User.  Changing users will remove these protocols from your Run Samples window.",
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
            "msgShutDown = Are you sure you want to shut down the RoboSep system? ",
            "",
            @"// Service Login Window",
            "msgLoginError1 = User name should not include numbers, symbols, or punctuation",
            "msgLoginError2 = Incorrenctly typed User Name",
            "msgLoginError3 = User Name entered is not long enough",
            "",
            @"// File Browser",
            "msgOverWrite = ' alraedy exists in destination folder.	Overwrite file?",
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
            "EnableLidSensor = true",
            "EnableLiquidLevelSensor = true",
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
            int count = 0;
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
                    string tempstring = "";
                    while (!sr.EndOfStream)
                    {
                        try
                        {
                            tempstring = sr.ReadLine();
                            if (!(tempstring.Length == 0 || tempstring == null || tempstring == string.Empty))
                            {
                                if (!tempstring[0].Equals('/') && !tempstring[0].Equals('[') && tempstring.Length > 5)
                                {
                                    // split string by = sign
                                    string[] parts = tempstring.Split('=');
                                    if (parts.Length > 1)
                                    {
                                        string key = "";
                                        string value = "";
                                        // create key string 
                                        for (int i = 0; i < parts[0].Length - 1; i++)
                                        { key += parts[0][i]; }
                                        // create value string
                                        for (int i = 1; i < parts[1].Length; i++)
                                        { value += parts[1][i]; }

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
                                else if (tempstring[0].Equals('['))
                                {
                                    // beginning of a section marks end of previous section
                                    // set index of last section
                                    count = 0;
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

        public static string getValue(string INI, string key)
        {
            try
            {
                fromINI ini = new fromINI(INI);
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
            // read all lines from current ini
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
                for (int i = sectionRange[1]; i < Lines.Length; i++)
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
                }
            }
            catch
            {
                // log exception

            }

        }

        public static NameValueCollection getSection(string INI, string sectName)
        {
            try
            {
                fromINI ini = new fromINI(INI);
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
                fromINI ini = new fromINI(INI);
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

    }
}
