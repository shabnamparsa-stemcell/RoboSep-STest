using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using System.IO;
using Tesla.DataAccess;

using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.Management;
using System.Text.RegularExpressions;

using Invetech.ApplicationLog;

using Tesla.Common.Protocol;
using Tesla.Common.ResourceManagement;
using Tesla.Common;
using GUI_Controls;

namespace GUI_Console
{
    class RoboSep_UserDB
    {
        private static RoboSep_UserDB myUserDB;
        private string myProtocolsPath;

        private static int iSystemArchitecture = -1;
        private const string SystemPath32   = "C:\\Program Files\\STI\\RoboSep\\";
        private const string SystemPath64   = "C:\\Program Files (x86)\\STI\\RoboSep\\";
        private const string XSDpath        = "config\\RoboSepUser.xsd";

        private const string UserInfoFileName = "UserInfo.ini";
        private const string PrefixProtocol = "Protocol";
        private const string PrefixProtocolLabel = "MruProtocolLabelQ";
        private const string PrefixSampleVolumeUl = "MruSampleVolumeUlQ";
        private const string PrefixUsageTime = "MruUsageTimeQ";


        // !!Hardcode
        //public string myXSDPath = Application.StartupPath + "\\..\\config\\RoboSepUser.xsd";
        public string myXSDPath;
        public string mySystemPath;
        // !!Hardcode
        static private string myUserConfigPath = "UserConfig.config";
        static private string ABBREVIATION_FILENAME = "protocolAbbreviation.txt";

        private List<RoboSep_Protocol> lstAllProtocols;
        //private string[] arrsAllUsers;
        private string myUserDBFile = "User1";

        //
        // user preferences
        //
        // User settings
        private const string TextUseAbbreviationsForProtocolNames = "UseAbbreviationsForProtocolNames";
        private const string TextLoadMRUProtocolsAtStartUp = "LoadMRUProtocolsAtStartUp";
        private const string TextUseFirstTabToDisplayAllProtocolsList = "UseFirstTabToDisplayAllProtocolsList";
        private const string TextEnableProgressBarDetailsView = "EnableProgressBarDetailsView";
        private const string TextSwitchToMyProtocolListAfterAddingAProtocol = "SwitchToMyProtocolListAfterAddingAProtocol";
        private const string TextSkipResourcesScreen = "SkipResourcesScreen";
        private const string TextEnableAutoScanBarcodes = "EnableAutoScanBarcodes";

        // Device settings
        private const string TextDisableCamera = "DisableCamera";
        private const string TextDeleteVideoLogFiles = "DeleteVideoLogFiles";
        private const string TextEnableBeepSwitch = "EnableBeepSwitch";
        private const string ProtocolSuffix = "Protocol";

        // User settings
        private bool DefaultUseAbbreviationsForProtocolNames = true;
        private bool DefaultLoadMRUProtocolAtStartUp = true;
        private bool DefaultUseFirstTabToDisplayAllProtocolsList = false;
        private bool DefaultEnableProgressBarDetailsView = true;
        private bool DefaultSwitchToMyProtocolListAfterAddingAProtocol = false;
        private bool DefaultSkipResourcesScreen = false;
        private bool DefaultShowBarcodesUserInterface = false;
        private bool DefaultEnableAutoScanBarcodes = false;

        // Device settings
        private bool DefaultDisableCamera = false;
        private bool DefaultDeleteVideoLogFiles = false;
        private bool DefaultEnableBeepSwitch = true;

        private Dictionary<string, string> dictAbbreviations;
        private Dictionary<string, bool> dictCurrentUserUserPreferences = new Dictionary<string, bool>();
        private Dictionary<string, bool> dictCurrentUserDevicePreferences = new Dictionary<string, bool>();
        private string currentUser;

        // Presets
        private List<string> lstPresetNames = null;

        // Language file
        private IniFile LanguageINI;

        // define static constants
        private const string USER_DB_EXTENSION = ".udb";

        private int getMyOsinfo()
        {
            int DataBit = -1;
            try
            {
                DataBit = Utilities.getOsInfo();
            }
            catch (Exception)
            {
                return 32;
            }
            return DataBit;
        }

        public static int OS_Bit
        {
            set
            {
                iSystemArchitecture = value;
                UserDetails.OS_Bit = value;
                myUserDB = RoboSep_UserDB.getInstance();
                switch (value)
                {
                    case 32:
                        myUserDB.mySystemPath = SystemPath32;
                        myUserDB.myXSDPath = SystemPath32 + XSDpath;
                        break;
                    case 64:
                        myUserDB.mySystemPath = SystemPath64;
                        myUserDB.myXSDPath = SystemPath64 + XSDpath;
                        break;
                }      

            }
        }

        public int getOSArchitecture()
        {
            if (iSystemArchitecture > 0)
            {
                return iSystemArchitecture;
            }
#if False
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2",
                    "SELECT * FROM Win32_Processor");

                foreach (ManagementObject queryObj in searcher.Get())
                {
                    iSystemArchitecture = Convert.ToInt32(queryObj["DataWidth"]);

                    // LOG
                    string logMSG = "System Architecture " + iSystemArchitecture.ToString() + " Bits";
                    //GUI_Controls.uiLog.LOG(this, "getOSArchitecture", uiLog.LogLevel.GENERAL, logMSG);

                    return iSystemArchitecture;
                    //return 32;
                }
            }
            catch (Exception e)
            {
                // LOG
                string logMSG = "Exception, return Defaulted of 32 Bit";
                //GUI_Controls.uiLog.LOG(this, "getOSArchitecture", uiLog.LogLevel.ERROR, logMSG);

                return 32;
            }
            return 32;                            
#else
            return getMyOsinfo();
#endif
        }

        private RoboSep_UserDB()
        {
            // LOG
            string logMSG = "Initializing UserDB";
            //GUI_Controls.uiLog.LOG(this, "Robosep_UserDB()", uiLog.LogLevel.DEBUG, logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);            

            int osBit = getOSArchitecture();

            switch (osBit)
            {
                case 32:
                    mySystemPath = SystemPath32;  
                    myXSDPath    = SystemPath32 + XSDpath;
                    break;
                case 64:
                    mySystemPath = SystemPath64;  
                    myXSDPath    = SystemPath64 + XSDpath;                    
                    break;
            }

            //string[] arrsAllUsers = getAllUsers();
           
            string fullPathAbbrevFileName = GetAppConfigPath();
            fullPathAbbrevFileName += ABBREVIATION_FILENAME;

            ProtocolAbbreviation protocolAbbrev = new ProtocolAbbreviation();
            protocolAbbrev.LoadAbbreviations(fullPathAbbrevFileName);
            dictAbbreviations = protocolAbbrev.dictAbbreviations;

            myProtocolsPath = GetProtocolsPath();

            // Load all protocol list
            lstAllProtocols = new List<RoboSep_Protocol>();
            LoadAllProtocols(ref lstAllProtocols);

            // Build protocol presets 
            BuildProtocolPresetsForUserInfoINIFile();

            // Tidy up the userinfo.ini file
            CheckUserInfoIniFile();
        }

        public string sysPath
        {
            get
            {
                return mySystemPath;
            }
        }

        public static RoboSep_UserDB getInstance()
        {
            if (myUserDB == null)
            {
                myUserDB = new RoboSep_UserDB();
            }
            return myUserDB;
        }

        public string GetAppConfigPath()
        {
            if (mySystemPath == null || myXSDPath == null)
            {
                GetSystemAndXSDPaths();
            }

            string appConfigPath = mySystemPath + "config\\";

            // LOG
            string logMSG = "Application config path: " + appConfigPath;
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG); 
            return appConfigPath;
         }


        public string GetProtocolsPath()
        {
            // Check the "protocols" directory exists, otherwise create it
#if (False)
            string startupPath = Application.StartupPath;
            int lastBackslashIndex = startupPath.LastIndexOf('\\');  // must escape the '\'
            startupPath = startupPath.Remove(lastBackslashIndex, startupPath.Length - lastBackslashIndex);
            string protocolsPath = startupPath + @"\protocols\";
#else
            if (mySystemPath == null || myXSDPath == null)
            {
                GetSystemAndXSDPaths();
            }

            string protocolsPath = mySystemPath + "protocols\\";
#endif
            if (!Directory.Exists(protocolsPath))
            {
                // We're not running from the application installation directory,
                // (probably because we're running from development directories)
                // so create a  "protocols" directory.
                Directory.CreateDirectory(protocolsPath);
            }

            // LOG
            string logMSG = "output: " + protocolsPath;
            //GUI_Controls.uiLog.LOG(this, "GetProtocolsPath", uiLog.LogLevel.DEBUG, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG); 
            return protocolsPath;

            // !!Hardcoded
            //return myProtocolsPath;
        }


        private void GetSystemAndXSDPaths()
        {
            if (mySystemPath == null || myXSDPath == null)
            {
                int osBit = getOSArchitecture();

                switch (osBit)
                {
                    case 32:
                        mySystemPath = SystemPath32;
                        myXSDPath = SystemPath32 + XSDpath;
                        break;
                    case 64:
                        mySystemPath = SystemPath64;
                        myXSDPath = SystemPath64 + XSDpath;
                        break;
                }
            }
        }

        public void LoadAllProtocols(ref List<RoboSep_Protocol> lstProtocols)
        {
            if (lstProtocols == null)
                return;

            DirectoryInfo di = new DirectoryInfo(myProtocolsPath);
            FileInfo[] rgFiles = di.GetFiles("*.xml");

            lstProtocols.Clear();

            foreach (FileInfo fi in rgFiles)
            {
                if (!fi.Name.Equals("prime.xml") &&
                    !fi.Name.Equals("shutdown.xml") &&
                    !fi.Name.Equals("home_axes.xml"))
                {
                    // create protocols object to represent each file
                    // add protocols to lstAllProtocols
                    RoboSep_Protocol tempProtocol = RoboSep_Protocol.generateProtocolFromFile(fi);
                    tempProtocol.Protocol_Name_Abbv = getProtocolAbbreviatedName(tempProtocol.Protocol_Name);
                    tempProtocol.Selection_Name_Abbv = getProtocolAbbreviatedName(tempProtocol.Selection_Name);
                    tempProtocol.Species_Name_Abbv = getProtocolAbbreviatedName(tempProtocol.Species_Name);
                    tempProtocol.SpeciesSelection_Name_Abbv = getProtocolAbbreviatedName(tempProtocol.SpeciesSelection_Name);

                    lstProtocols.Add(tempProtocol);

                    //YW added
                    /*
                    RoboSepProtocol ProtocolInfo = new RoboSepProtocol();
                    if (File.Exists(fi.FullName))
                    {
                        // serializer
                        XmlSerializer myXmlSerializer = new XmlSerializer(typeof(RoboSepProtocol));

                        // Initialise a file stream for reading
                        FileStream fs = new FileStream(fi.FullName, FileMode.Open);

                        try
                        {
                            XmlReaderSettings settings = new XmlReaderSettings();
                            //settings.ValidationType = ValidationType.Schema;
                            settings.ValidationType = ValidationType.None;
                            settings.Schemas.Add("STI", RoboSep_Protocol.GetProtocolXSD());
                            XmlReader reader = XmlReader.Create(fs, settings);
                            ProtocolInfo = (RoboSepProtocol)myXmlSerializer.Deserialize(reader);
                        }
                        catch (Exception ex)
                        {
                            string sMsg = String.Format("Robosep Protocol File '{0}' is invalid. Exception={1}", fi.FullName, ex.Message);
                            System.Diagnostics.Debug.Write("!!!RoboSep_Protocols : Read Robosep Protocol failed.");


                            MessageBox.Show(sMsg);
                            //myCurrentActionContext = "User1.udb";
                        }
                        finally
                        {
                            // Close the file stream
                            fs.Close();
                        }
                    }
                    string featureName1 = ProtocolInfo.constraints.featuresSwitches[0].name;
                    string featureDesc2 = ProtocolInfo.constraints.featuresSwitches[0].desc);
                    string featureInputType2 = ProtocolInfo.constraints.featuresSwitches[0].inputType;
                    string featureInputData1 =ProtocolInfo.constraints.featuresSwitches[0].inputData;
                    string featureName2 = ProtocolInfo.constraints.featuresSwitches[1].name;
                    string featureDesc2 = ProtocolInfo.constraints.featuresSwitches[1].desc;
                    string featureInputType2 = ProtocolInfo.constraints.featuresSwitches[1].inputType;
                    string featureInputData2 = ProtocolInfo.constraints.featuresSwitches[1].inputData;
                    */
                    //End of YW
                }
            }

            // LOG
            string logMSG = lstProtocols.Count.ToString() + "protocols loaded";
            //GUI_Controls.uiLog.LOG(this, "LoadAllProtocols", uiLog.LogLevel.DEBUG, logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);            
        }

        public void ReloadAllProtocols()
        {
            if (lstAllProtocols == null)
                lstAllProtocols = new List<RoboSep_Protocol>();

            LoadAllProtocols(ref lstAllProtocols);
        }


        public void reloadProtocols(ref List<RoboSep_Protocol> lstProtocols)
        {
            LoadAllProtocols(ref lstProtocols);

            // LOG
            string logMSG = lstProtocols.Count.ToString() + "protocols re-loaded";
            //GUI_Controls.uiLog.LOG(this, "reloadProtocols", uiLog.LogLevel.DEBUG, logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);            

            return;
        }

        private string getProtocolAbbreviatedName(string iLongName)
        {
            if (string.IsNullOrEmpty(iLongName))
                return null;
 
            string longName = iLongName.Trim();

            char[] delimiters = new char[] { ' ' };
            string[] tokens = longName.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            string temp;
            for (int i = 0; i < tokens.Length; i++)
            {
                if (string.IsNullOrEmpty(tokens[i]))
                    continue;

                temp = tokens[i];

                if (dictAbbreviations.ContainsKey(temp.ToLower()))
                {
                    tokens[i] = getAbbreviatedToken(temp, ' ');
                }
                else
                {
                    if (0 < tokens[i].IndexOf('-'))
                    {
                        tokens[i] = getAbbreviatedToken(temp, '-');
                    }
                    else if (0 < tokens[i].IndexOf(';'))
                    {
                        tokens[i] = getAbbreviatedToken(temp, ';');
                    }
                }
            }

            return String.Join(" ", tokens);
        }

        private string getAbbreviatedToken(string longToken, char tokenSeparator)
        {
            if (string.IsNullOrEmpty(longToken))
                return null;

            string[] tokens = longToken.Split(tokenSeparator);
            string temp, value;
            char[] charArr, tempCharArr;
            for (int i = 0; i < tokens.Length; i++)
            {
                temp = tokens[i];
                if (dictAbbreviations.ContainsKey(temp.ToLower()))
                {
                    value = "";
                    if (dictAbbreviations.TryGetValue(temp.ToLower(), out value))
                    {
                        tokens[i] = value;
                        charArr = value.ToCharArray();
                        if (char.IsLetter(charArr[0]) && !char.IsUpper(charArr[0]))
                        {
                            tempCharArr = temp.ToCharArray();
                            // Check if the first character is upper case
                            if (char.IsLetter(tempCharArr[0]) && char.IsUpper(tempCharArr[0]))
                            {
                                charArr[0] = char.ToUpper(charArr[0]);
                                tokens[i] = new string(charArr);
                            }
                        }
                    }
                }
            }
            return String.Join(tokenSeparator.ToString(), tokens);
        }

        public string getProtocolFilename(string PLabel)
        {
            for (int i = 0; i < lstAllProtocols.Count; i++)
            {
                if (lstAllProtocols[i].Protocol_Name == PLabel)
                    return lstAllProtocols[i].Protocol_FileName;
            }
            return string.Empty;
        }

        public string getProtocolAbbvNameFromList(string PLabel)
        {
            for (int i = 0; i < lstAllProtocols.Count; i++)
            {
                if (lstAllProtocols[i].Protocol_Name == PLabel)
                    return lstAllProtocols[i].Protocol_Name_Abbv;
            }
            return string.Empty;
        }

        public RoboSep_Protocol_VialBarcodes getProtocolAssignedVialBarcodes(string protocolName, int quadrant)
        {
            RoboSep_Protocol_VialBarcodes result = null;
            for (int i = 0; i < lstAllProtocols.Count; i++)
            {
                if (lstAllProtocols[i].Protocol_Name.Equals(protocolName))
                {
                    if (lstAllProtocols[i].vialBarcodes != null)
                    {
                        for (int j = 0; j < lstAllProtocols[i].vialBarcodes.Length; j++)
                        {
                            if (lstAllProtocols[i].vialBarcodes[j].quadrant == quadrant)
                            {
                                result = new RoboSep_Protocol_VialBarcodes(lstAllProtocols[i].vialBarcodes[j].quadrant,
                                            lstAllProtocols[i].vialBarcodes[j].squareVialBarcode,
                                            lstAllProtocols[i].vialBarcodes[j].circleVialBarcode,
                                            lstAllProtocols[i].vialBarcodes[j].triangleVialBarcode);
                                break;
                            }
                        }
                    }
                    break;
                }
            }
            return result;
        }

        public void getProtocolCommandList(string fileName, out int[] cmds, out int[] times, out string[] descriptions)
        {
            int[] commands = null;
            int[] estTimes = null;
            string[] desc = null;

            string filePath = GetProtocolsPath() + fileName;
            if (File.Exists(filePath))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);
                XmlNodeList commandList = doc.GetElementsByTagName("commands");

                if (commandList.Count == 1)
                {
                    //Console.WriteLine(commandList[0].Attributes["number"]);
                    int commandSize = Convert.ToInt32(commandList[0].Attributes["number"].Value);
                    commands = new int[commandSize];
                    estTimes = new int[commandSize];
                    desc = new string[commandSize];
                    for (int i = 0; i < commandList[0].ChildNodes.Count; i++)
                    {
                        XmlNode commandNode = commandList[0].ChildNodes[i];
                        //Console.WriteLine(commandNode.Name);
                        //Console.WriteLine(commandNode.Attributes["label"]);
                        commands[i] = getCommandInfo(commandNode.Name, true);
                        //if (estTimes[i] == 0)
                        estTimes[i] = getCommandInfo(commandNode.Name, false);
                        if (estTimes[i] < 0)
                            estTimes[i] = 1000;
                        for (int j = 0; j < commandNode.ChildNodes.Count; j++)
                        {
                            XmlNode node = commandNode.ChildNodes[j];
                            if (node.Name == "processingTime")
                            {
                                estTimes[i] = Convert.ToInt32(node.Attributes["duration"].Value);
                                node = null;                                
                                break;
                            }
                            node = null;
                        }
                        desc[i] = (string)commandNode.Attributes["label"].Value;
                        commandNode = null;
                    }
                }
                commandList = null;
                doc = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            cmds = commands;
            times = estTimes;
            descriptions = desc;

            // LOG
            string logMSG = "returned " + commands.Length.ToString() + "commands";
            //GUI_Controls.uiLog.LOG(this, "getProtocolCommandList", uiLog.LogLevel.DEBUG, logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);   
        }

        private int getCommandInfo(string command, bool getPos)
        {
            int result = 0;
            switch (command)
            {
                case "IncubateCommand":
                    if (getPos)
                        result = 0;
                    else
                        result = -1;
                    break;
                case "MixCommand":
                    if (getPos)
                        result = 1;
                    else
                        result = 37;
                    break;
                case "TransportCommand":
                    if (getPos)
                        result = 2;
                    else
                        result = 70;
                    break;
                case "SeparateCommand":
                    if (getPos)
                        result = 3;
                    else
                        result = -1;
                    break;
                case "ResuspendVialCommand":
                    if (getPos)
                        result = 4;
                    else
                        result = 70;
                    break;
                case "TopUpVialCommand":
                    if (getPos)
                        result = 5;
                    else
                        result = 70;
                    break;
                case "FlushCommand":
                    if (getPos)
                        result = 6;
                    else
                        result = 10;
                    break;
                case "PrimeCommand":
                    if (getPos)
                        result = 7;
                    else
                        result = 25;
                    break;
                case "HomeAllCommand":
                    if (getPos)
                        result = 8;
                    else
                        result = 30;
                    break;
                case "DemoCommand":
                    if (getPos)
                        result = 9;
                    else
                        result = 200;
                    break;
                case "ParkCommand":
                    if (getPos)
                        result = 10;
                    else
                        result = 10;
                    break;
                case "PumpLifeCommand":
                    if (getPos)
                        result = 11;
                    else
                        result = 200;
                    break;
                case "MixTransCommand":
                    if (getPos)
                        result = 12;
                    else
                        result = 200;
                    break;
                case "TopUpMixTransCommand":
                    if (getPos)
                        result = 13;
                    else
                        result = 200;
                    break;
                case "ResusMixSepTransCommand":
                    if (getPos)
                        result = 14;
                    else
                        result = 200;
                    break;
                case "ResusMixCommand":
                    if (getPos)
                        result = 15;
                    else
                        result = 200;
                    break;
                case "TopUpTransCommand":
                    if (getPos)
                        result = 16;
                    else
                        result = 200;
                    break;
                case "TopUpTransSepTransCommand":
                    if (getPos)
                        result = 17;
                    else
                        result = 200;
                    break;
                case "TopUpMixTransSepTransCommand":
                    if (getPos)
                        result = 18;
                    else
                        result = 200;
                    break;
            }
            return result;
        }

        //public void getProtocolCommandList(string fileName, out int[] cmds, out int[] times, out string[] descriptions)
        //{
        //    int[] commands = null;
        //    int[] estTimes = null;
        //    string[] desc = null;
        //    int cmdNum = 0;

        //    string[] cmdStrings = new string[] 
        //            {
        //                "IncubateCommand",                        
        //                "MixCommand",
        //                "TransportCommand",
        //                "SeparateCommand",
        //                "ResuspendVialCommand",
        //                "TopUpVialCommand",
        //                "FlushCommand",
        //                "PrimeCommand",
        //                "HomeAllCommand",
        //                "DemoCommand",
        //                "ParkCommand",
        //                "PumpLifeCommand"
        //            };
        //    int[] cmdTimes = new int[]
        //            {
        //                -1, 37, 70, -1, 70,  70, 10, 25, 30, 200, 10, 200
        //            };

        //    string Filepath = GetProtocolsPath() + fileName;
            
        //    // open file, grab information from file
        //    if (File.Exists(Filepath))
        //    {
        //        using (StreamReader sr = new StreamReader(Filepath))
        //        {
        //            // get rid of 1st line
        //            sr.ReadLine();
        //            string line;
        //            // get set of times from cfg file to match command names.
        //            // non defined values should be available in .xml file
                    
        //            // read line with file info
        //            while ((line = sr.ReadLine()) != null)
        //            {
        //                if (line == "")
        //                    continue;
        //                // split up line by
        //                if (commands == null)
        //                {
        //                    string[] lineBreakdown = line.Split(new char[] { '=', '<', '>' });
        //                    for (int i = 0; i < lineBreakdown.Length; i++)
        //                    {
        //                        // first find how many commands there are going to be
        //                        if (lineBreakdown[i] == "commands number")
        //                        {
        //                            string[] bd = lineBreakdown[i + 1].Split('"');
        //                            if (Convert.ToInt32(bd[1]) > 0)
        //                            {
        //                                commands = new int[Convert.ToInt32(bd[1])];
        //                                estTimes = new int[Convert.ToInt32(bd[1])];
        //                                desc = new string[Convert.ToInt32(bd[1])];
        //                                break;
        //                            }
        //                        }
        //                    }
        //                }
        //                // add commands and times
        //                else if (cmdNum >= commands.Length)
        //                    break;
        //                else
        //                {
        //                    int isCommand = -1;

        //                    line = line.Trim(new char[] {' ', '\t'});
        //                    int idx1 = line.IndexOf('<');
        //                    if (idx1 >= 0)
        //                    {
        //                        int idx2;
        //                        for (idx2 = idx1; line[idx2] == ' '; idx2++)
        //                        {
        //                        }
        //                        int idx3 = line.IndexOf(' ', idx2 + 1);
        //                        if (idx3 >= 0)
        //                        {
        //                            string nodeName = line.Substring(idx2 + 1, idx3 - idx2 - 1);
        //                            for (int i = 0; i < cmdStrings.Length; i++)
        //                            {
        //                                if (nodeName == cmdStrings[i])
        //                                {
        //                                    isCommand = i;
        //                                    break;
        //                                }
        //                            }
        //                            if (isCommand >= 0)
        //                            {
        //                                string descriptionWording = "label=\"";
        //                                int descriptionIdx1 = line.IndexOf(descriptionWording);
        //                                if (descriptionIdx1 >= 0)
        //                                {
        //                                    int descriptionIdx2 = line.IndexOf('"', descriptionIdx1 + descriptionWording.Length);
        //                                    if (descriptionIdx2 >= 0)
        //                                        desc[cmdNum] = line.Substring(descriptionIdx1 + descriptionWording.Length, descriptionIdx2 - descriptionIdx1 - descriptionWording.Length);
        //                                }

        //                                commands[cmdNum] = isCommand;
        //                                if (estTimes[cmdNum] == 0)
        //                                    estTimes[cmdNum] = cmdTimes[isCommand];
        //                                for (int ii = 0; ii < cmdNum; ii++)
        //                                {
        //                                    if (estTimes[ii] < 0)
        //                                        estTimes[ii] = 1000;
        //                                }
        //                                cmdNum++;
        //                            }
        //                            else
        //                            {
        //                                // if is not command line, look for times and volumes
        //                                string processTimeWording = "processingTime duration";
        //                                int estTimesIdx1 = line.IndexOf(processTimeWording);
        //                                if (estTimesIdx1 >= 0)
        //                                {
        //                                    int estTimesIdx2 = line.IndexOf('"', estTimesIdx1 + processTimeWording.Length);
        //                                    if (estTimesIdx2 >= 0)
        //                                    {
        //                                        int estTimesIdx3 = line.IndexOf('"', estTimesIdx2 + 1);
        //                                        if (estTimesIdx3 >= 0)
        //                                            estTimes[cmdNum - 1] = Convert.ToInt32(line.Substring(estTimesIdx2 + 1, estTimesIdx3 - estTimesIdx2 - 1));
        //                                    }
        //                                }
        //                                int volIdx1 = line.IndexOf("absoluteVolume value_uL");
        //                                if (volIdx1 >= 0)
        //                                {
        //                                    // figure out transport time
        //                                }
        //                            }
        //                        }
                                
        //                    }

        //                    /*
        //                    Console.WriteLine(">>>>>>>>>>>>2" + temp);
        //                    string[] lineBreakdown2 = temp.Split(new char[] { ' ', '<', '>' });

        //                    string[] lineBreakdown = line.Split(new char[] { ' ', '<', '>' });
        //                    string[] getDescription = line.Split('"');
        //                    for (int i = 0; i < cmdStrings.Length; i++)
        //                    {
        //                        if (lineBreakdown[5] == cmdStrings[i])
        //                        {
        //                            isCommand = i;
        //                            break;
        //                        }
        //                    }
        //                    if (isCommand >= 0)
        //                    {
        //                        desc[cmdNum] = getDescription[3];
        //                        commands[cmdNum] = isCommand;
        //                        if (estTimes[cmdNum] == 0)
        //                            estTimes[cmdNum] = cmdTimes[isCommand];
        //                        for (int ii = 0; ii < cmdNum; ii++)
        //                        {
        //                            if (estTimes[ii] < 0)
        //                                estTimes[ii] = 1000;
        //                        }
        //                        cmdNum++;
        //                    }
        //                    else
        //                    {
        //                        // if is not command line, look for times and volumes
        //                        lineBreakdown = line.Split(new char[] { '<', '=', '"' });
        //                        if (lineBreakdown[1] == "processingTime duration")
        //                        {
        //                            estTimes[cmdNum-1] = Convert.ToInt32(lineBreakdown[3]);
        //                        }
        //                        else if (lineBreakdown[1] == "absoluteVolume value_uL")
        //                        {
        //                            // figure out transport time
        //                        }
        //                    }*/
        //                }
        //            }
        //        }
        //    }
        //    cmds = commands;
        //    times = estTimes;
        //    descriptions = desc;

        //    // LOG
        //    string logMSG = "returned " + commands.Length.ToString() + "commands";
        //    //GUI_Controls.uiLog.LOG(this, "getProtocolCommandList", uiLog.LogLevel.DEBUG, logMSG);
        //                LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);            
        //}

        public string[] getAllUsers()
        {
            string[] users = UserDetails.getAllSections("USER");
            return users;
        }

        public List<RoboSep_Protocol> getAllProtocols()
        {
            return lstAllProtocols;
        }

        // tidy up the section and section data if necessary
        public void CheckUserInfoIniFile()
        {
            string fullPathUserFileName = Utilities.GetRoboSepSysPath();

            fullPathUserFileName += "protocols\\";
            fullPathUserFileName += UserInfoFileName;

            if (!File.Exists(fullPathUserFileName))
            {
                return;
            }

            string[] users = getAllUsers();
            if (users == null || users.Length == 0)
                return;

            // read values
            string[] lines = File.ReadAllLines(fullPathUserFileName, System.Text.Encoding.UTF8);
            if (lines == null || lines.Length == 0)
                return;

            Array.Sort(users);
            List<string> lst = new List<string>();
            lst.AddRange(lines);

            List<string> lstNew = new List<string>();

            // Add comments
            int index1 = 0;
            string temp, temp1;
            do
            {
                temp = @"//";
                index1 = lst.FindIndex(index1, x => { return (!string.IsNullOrEmpty(x) && x.Length >= temp.Length && x.Contains(temp) && x.Trim().Substring(0, temp.Length) == temp); });
                if (index1 >= 0)
                {
                    temp1 = lst[index1];
                    lstNew.Add(temp1);
                    index1++;
                }
            }
            while (0 <= index1);

            lstNew.Add("");
            for (int i = 0; i < users.Length; i++)
            {
                temp = String.Format("[{0}]", users[i]);

                // user name 
                index1 = lst.FindIndex(x => { return (!string.IsNullOrEmpty(x) && x.Length == temp.Length && x == temp); });
                if (index1 >= 0)
                {
                    temp1 = lst[index1];
                    lstNew.Add(temp1);
                }

                // user image
                temp = users[i] + "Image";
                index1 = lst.FindIndex(x => { return (!string.IsNullOrEmpty(x) && x.Length >= temp.Length && x.ToLower().Contains(temp.ToLower()) && x.Trim().Substring(0, temp.Length) == temp); });
                if (index1 >= 0)
                {
                    temp1 = lst[index1];
                    lstNew.Add(temp1);
                }

                // user protocols
                index1 = 0;
                temp = users[i] + PrefixProtocol;
                do
                {
                    index1 = lst.FindIndex(index1, x => { return (!string.IsNullOrEmpty(x) && x.Length >= temp.Length && x.ToLower().Contains(temp.ToLower()) && x.Trim().Substring(0, temp.Length) == temp); });
                    if (0 <= index1)
                    {
                        temp1 = lst[index1];
                        if (!lstNew.Contains(temp1))
                            lstNew.Add(temp1);

                        index1++;
                    }

                } while (0 <= index1);

                // user MRU
                string tempPrefixLabel = users[i] + PrefixProtocolLabel;
                string tempPrefixSampleVolume = users[i] + PrefixSampleVolumeUl;
                string tempPrefixUsageTime = users[i] + PrefixUsageTime;
                int quadrant = 0;

                for (int j = 0; j < 4; j++)
                {
                    // Quadrant starts at 1 instead of 0
                    quadrant = j + 1;
                    temp = tempPrefixLabel + quadrant.ToString();
                    index1 = lst.FindIndex(x => { return (!string.IsNullOrEmpty(x) && x.Length >= temp.Length && x.ToLower().Contains(temp.ToLower()) && x.Trim().Substring(0, temp.Length) == temp); });
                    if (0 <= index1)
                    {
                        temp1 = lst[index1];
                        if (!lstNew.Contains(temp1))
                            lstNew.Add(temp1);
                    }

                    temp = tempPrefixSampleVolume + quadrant.ToString();
                    index1 = lst.FindIndex(x => { return (!string.IsNullOrEmpty(x) && x.Length >= temp.Length && x.ToLower().Contains(temp.ToLower()) && x.Trim().Substring(0, temp.Length) == temp); });
                    if (0 <= index1)
                    {
                        temp1 = lst[index1];
                        if (!lstNew.Contains(temp1))
                            lstNew.Add(temp1);
                    }

                    temp = tempPrefixUsageTime + quadrant.ToString();
                    index1 = lst.FindIndex(x => { return (!string.IsNullOrEmpty(x) && x.Length >= temp.Length && x.ToLower().Contains(temp.ToLower()) && x.Trim().Substring(0, temp.Length) == temp); });
                    if (0 <= index1)
                    {
                        temp1 = lst[index1];
                        if (!lstNew.Contains(temp1))
                            lstNew.Add(temp1);
                    }
                }

                lstNew.Add("");
            }

            // write text to file
            File.WriteAllLines(fullPathUserFileName, lstNew.ToArray());
        }

        public void deleteUserProtocols(string userName, string[] userProtocols)
        {
            if (string.IsNullOrEmpty(userName) || userProtocols == null || userProtocols.Length == 0)
                return;

            // LOG
            string logMSG = "deleting " + userProtocols.Length.ToString() + "protocols from user " + userName;
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG); 
            // if user name exists save section
            List<string> listUserProtocols = new List<string>();
            listUserProtocols.AddRange(userProtocols);

            if (userNameExists(userName))
            {
                System.Collections.Specialized.NameValueCollection userItems = UserDetails.getSection("USER", userName);

                //enumerate the keys
                string temp;
                string[] parts;
                string UserProtocolPrefix = userName.Trim() + ProtocolSuffix;

                foreach (string key in userItems)
                {
                    if (string.IsNullOrEmpty(key))
                        continue;

                    if (key.ToLower() == UserProtocolPrefix.ToLower())
                    {
                        temp = userItems[key];
                        if (string.IsNullOrEmpty(temp))
                            continue;

                        parts = temp.Split(',');
                        int index = 0;
                        for (int i = 0; i < parts.Length; i++)
                        {
                            index = listUserProtocols.FindIndex(x => { return (!string.IsNullOrEmpty(x) && x.Length == parts[i].Trim().Length && x.ToLower() == parts[i].Trim().ToLower()); });
                            if (0 <= index)
                            {
                                UserDetails.DeleteEntry("USER", UserProtocolPrefix, parts[i]);
                            }
                        }
                        break;
                    }
                }
            }
        }

        public void saveUserProtocols(string userName, string[] userProtocols)
        {
            // LOG
            string logMSG = "saving " + userProtocols.Length.ToString() + "protocols to user " + userName;
            //GUI_Controls.uiLog.LOG(this, "saveUserProtocols", uiLog.LogLevel.INFO, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG); 
            // if user name exists save section
            List<string> listUserProtocols = new List<string>();
            listUserProtocols.AddRange(userProtocols);

            if (userNameExists(userName))
            {
                System.Collections.Specialized.NameValueCollection userItems = UserDetails.getSection("USER", userName);

                //enumerate the keys
                string temp;
                string[] parts;
                string UserProtocolPrefix = userName.Trim() + ProtocolSuffix;

                foreach (string key in userItems)
                {
                    if (key.ToLower() == UserProtocolPrefix.ToLower())
                    {
                        temp = userItems[key];
                        if (string.IsNullOrEmpty(temp))
                            continue;

                        parts = temp.Split(',');
                        int index = 0;
                        for (int i = 0; i < parts.Length; i++)
                        {
                            index = listUserProtocols.FindIndex(x => { return (!string.IsNullOrEmpty(x) && x.Length ==  parts[i].Trim().Length && x.ToLower() == parts[i].Trim().ToLower()); });
                            if (index < 0)
                            {
                                UserDetails.DeleteEntry("USER", UserProtocolPrefix, parts[i]);
                            }
                            else
                            {
                                listUserProtocols.RemoveAt(index);
                            }
                        }
                        break;
                    }
                }

                if (listUserProtocols.Count > 0)
                {
                    foreach (string protocol in listUserProtocols)
                    {
                        if (string.IsNullOrEmpty(protocol))
                            continue;
                        UserDetails.InsertEntry("USER", UserProtocolPrefix, protocol);
                    }
                }
            }
            // if user name doesn't exist, add section
            else
            {
                string[] addSection = new string[userProtocols.Length + 2];
                addSection[0] = "\n[" + userName + "]";
                addSection[1] = userName + "Image = Default";
                for (int i = 0; i < userProtocols.Length; i++)
                {
                    addSection[i + 2] = userName + "Protocol = " + userProtocols[i];
                }

                UserDetails.addSection("USER", addSection);
            }

            //
            // Save over User1.udb
            //
            XML_SaveUserProfile(RoboSep_UserConsole.strCurrentUser, userProtocols);

            // test for proper save
            if (XML_UserProfileDBToName() != RoboSep_UserConsole.strCurrentUser)
            {
                LanguageINI = LanguageINI = GUI_Console.RoboSep_UserConsole.getInstance().LanguageINI;
                GUI_Controls.RoboMessagePanel newPrompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_ERROR,
                    LanguageINI.GetString("saveUserDBFail"), LanguageINI.GetString("headerSaveUserDBFail"), LanguageINI.GetString("Ok"));
                RoboSep_UserConsole.getInstance().addForm(newPrompt, newPrompt.Offset);
                RoboSep_UserConsole.showOverlay();
                newPrompt.ShowDialog();
                newPrompt.Dispose();
                RoboSep_UserConsole.hideOverlay();
            }

            // update user list
            //arrsAllUsers = getAllUsers();
        }


        public List<RoboSep_Protocol> loadUserProtocols(string userName)
        {
            // get list from INI
            List<RoboSep_Protocol> rpTempList = new List<RoboSep_Protocol>();
            NameValueCollection nvc = UserDetails.getSection("USER", userName);
            if (nvc == null)
                return rpTempList;

            string key = userName + "Protocol";
            try
            {
                string[] temp = nvc.GetValues(key);
                if (temp == null || temp.Length == 0)
                    return rpTempList;

                string pattern = @".xml";
                string[] userProtocols = Regex.Split(temp[0], pattern);
                if (userProtocols == null || userProtocols.Length == 0)
                    return rpTempList;

                for (int i = 0; i < userProtocols.Length; i++)
                {
                    if (string.IsNullOrEmpty(userProtocols[i]))
                        continue;
                    userProtocols[i] = userProtocols[i].Trim();
                    userProtocols[i] = userProtocols[i].TrimStart(',');
                    userProtocols[i] += ".xml";
                }

                // add protocols that match strings to list
                foreach (string s in userProtocols)
                {
                    if (string.IsNullOrEmpty(s))
                        continue;

                    bool protocolExists = false;
                    for (int i = 0; i < lstAllProtocols.Count; i++)
                    {
                        if (s == lstAllProtocols[i].Protocol_FileName)
                        {
                            rpTempList.Add(lstAllProtocols[i]);
                            protocolExists = true;
                            break;
                        }
                    }
                    if (!protocolExists)
                    {
                        // LOG
                        string logMSG = "err loading protocol " + s + " from USB";
                        //GUI_Controls.uiLog.LOG(this, "loadUserProtocols", uiLog.LogLevel.DEBUG, logMSG);
                        LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);            

                        LanguageINI = RoboSep_UserConsole.getInstance().LanguageINI;

                        string sTitle = LanguageINI.GetString("headerProtocolNotFound");
                        string sMSG = LanguageINI.GetString("ProtocolAddFail1") + s + LanguageINI.GetString("ProtocolAddFail2");
                        RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_ERROR, sMSG, sTitle, LanguageINI.GetString("Ok"));
                        RoboSep_UserConsole.showOverlay();
                        prompt.ShowDialog();
                        prompt.Dispose();
                        RoboSep_UserConsole.hideOverlay();
                    }
                }

                // LOG
                string LOGMSG = "Loaded " + rpTempList.Count.ToString() + " protocol from User " + userName;
                //GUI_Controls.uiLog.LOG(this, "loadUserProtocols", uiLog.LogLevel.DEBUG, LOGMSG);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, LOGMSG);            

                return rpTempList;
            }
            catch (Exception ex)
            {
                // LOG
                //string logMSG = "Failed to load " + userName + "protocol list";
                LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);                               
                return null;
            }
        }

        private bool userNameExists(string userName)
        {
            // update user list
            string[]  arrsAllUsers = getAllUsers();
            if (arrsAllUsers == null)
                return false;

            // checks all user names to see if given user name exists
            bool exists = false;
            try
            {
                for (int i = 0; i < arrsAllUsers.Length; i++)
                {
                    if (userName == arrsAllUsers[i])
                    {
                        exists = true;
                        break;
                    }
                }
                return exists;
            }
            catch
            {
                return exists;
            }
        }

        public bool CheckUserProtocolIntegrity()
        {
            Tesla.DataAccess.RoboSepUser currentUser = RoboSep_UserDB.getInstance().XML_LoadRoboSepUserObject();

            // check user protocols
            ISeparationProtocol[] UserProtocolsListFromServer = null;
            if (RoboSep_UserConsole.strCurrentUser == currentUser.UserName)
            {
                // determine validity of user ID
                string[] usrProtocols = currentUser.ProtocolFile;

                RoboSep_UserConsole pRoboSepUserConsole = RoboSep_UserConsole.getInstance();
                if (pRoboSepUserConsole == null)
                    return false;

                UserProtocolsListFromServer = pRoboSepUserConsole.XML_getUserProtocols();
                if (UserProtocolsListFromServer == null)
                    return false;

                usrProtocols = new string[UserProtocolsListFromServer.Length];
                for (int i = 0; i < UserProtocolsListFromServer.Length; i++)
                {
                    usrProtocols[i] = UserProtocolsListFromServer[i].Label;
                }

                // update my protocol list
                int nIndex = 0;
                bool bDirty = false;
                List<string> lstProtocolFilenames = new List<string>();
                List<string> lstProtocolNames = new List<string>();
                for (int i = 0; i < usrProtocols.Length; i++)
                {
                    nIndex = findInList(usrProtocols[i]);
                    if (nIndex < 0)
                    {
                        bDirty = true;
                        continue;
                    }

                    lstProtocolFilenames.Add(lstAllProtocols[nIndex].Protocol_FileName);
                    lstProtocolNames.Add(lstAllProtocols[nIndex].Protocol_Name);
                }

                if (bDirty)
                {
                    RoboSep_RunSamples.getInstance().CancelAllQuadrants();
                    saveUserProtocols(RoboSep_UserConsole.strCurrentUser,lstProtocolFilenames.ToArray());

                    RemoveMRUEntriesNotInProtocolList(RoboSep_UserConsole.strCurrentUser, lstProtocolNames.ToArray());
                }
            }
            return true;
        }

        public double GetFreeHDDSizeInMB(string driveName)
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name == driveName)
                {
                    return drive.AvailableFreeSpace / (1024 * 1024);     // in MB
                }
            }
            return -1;
        }

        private int findInList(string pString)
        {
            if (pString.Split('.').Length > 1)
            {
                // check first if string is filename
                for (int i = 0; i < lstAllProtocols.Count; i++)
                {
                    if (pString == lstAllProtocols[i].Protocol_FileName)
                        return i;
                }
                return -1;
            }
            else
            {
                // compare to unfiltered name
                for (int i = 0; i < lstAllProtocols.Count; i++)
                {
                    if (pString == lstAllProtocols[i].Protocol_Name)
                        return i;
                }
                return -1;
            }
        }

        public void RemoveMRUEntriesInProtocolList(string strUser, string[] UserProtocolsList)
        {
            if (UserProtocolsList == null || string.IsNullOrEmpty(strUser))
                return;

            // get protocols MRU 
            ProtocolMRUList MRUList = new ProtocolMRUList();
            if (MRUList == null)
                return;

            MRUList.LoadUserProtocolsMRU(strUser);
            ProtocolMostRecentlyUsed[] arrProtocolMRU = MRUList.ListProtocolMRU;
            if (arrProtocolMRU != null && arrProtocolMRU.Length > 0)
            {
                List<string> lstMRUToBeRemoved = new List<string>();
                for (int i = 0; i < arrProtocolMRU.Length; i++)
                {
                    if (arrProtocolMRU[i] == null || arrProtocolMRU[i].ProtocolLabel.Trim().Length == 0)
                        continue;

                    bool bFound = false;
                    foreach (string protocolName in UserProtocolsList)
                    {
                        if (!string.IsNullOrEmpty(protocolName) && (protocolName.ToLower() == arrProtocolMRU[i].ProtocolLabel.ToLower()))
                        {
                            bFound = true;
                            break;
                        }
                    }
                    if (bFound)
                        lstMRUToBeRemoved.Add(arrProtocolMRU[i].ProtocolLabel);
                }

                // remove all those MRU protocols not in my protocol list
                for (int j = 0; j < lstMRUToBeRemoved.Count; j++)
                {
                    MRUList.DeleteMostRecentlyUsedProtocol(strUser, lstMRUToBeRemoved[j]);
                }
            }
        }

        public void RemoveMRUEntriesNotInProtocolList(string strUser, string[] UserProtocolsList)
        {
            if (UserProtocolsList == null || string.IsNullOrEmpty(strUser))
                return;

            // get protocols MRU 
            ProtocolMRUList MRUList = new ProtocolMRUList();
            if (MRUList == null)
                return;

            if (UserProtocolsList.Length == 0)
            {
                MRUList.DeleteAllMostRecentlyUsedProtocols(strUser);
                return;
            }

            MRUList.LoadUserProtocolsMRU(strUser);
            ProtocolMostRecentlyUsed[] arrProtocolMRU = MRUList.ListProtocolMRU;

            if (arrProtocolMRU != null && arrProtocolMRU.Length > 0)
            {
                List<string> lstMRUToBeRemoved = new List<string>();
                for (int i = 0; i < arrProtocolMRU.Length; i++)
                {
                    if (arrProtocolMRU[i] == null || arrProtocolMRU[i].ProtocolLabel.Trim().Length == 0)
                        continue;

                    bool bFound = false;
                    foreach (string protocolName in UserProtocolsList)
                    {
                        if (!string.IsNullOrEmpty(protocolName) && (protocolName.ToLower() == arrProtocolMRU[i].ProtocolLabel.ToLower()))
                        {
                            bFound = true;
                            break;
                        }
                    }
                    if (!bFound)
                        lstMRUToBeRemoved.Add(arrProtocolMRU[i].ProtocolLabel);
                }

                // remove all those MRU protocols not in the input protocol list
                for (int j = 0; j < lstMRUToBeRemoved.Count; j++)
                {
                    MRUList.DeleteMostRecentlyUsedProtocol(strUser, lstMRUToBeRemoved[j]);
                }
            }
        }

        public void RenameProtocolsAndMRUs(string sOldUserLoginID, string sNewUserLoginID)
        {
            ProtocolMRUList.RenameProtocolsAndMRUs(sOldUserLoginID, sNewUserLoginID);
        }

        public bool loadCurrentUserPreferences(string loginUser)
        {
            currentUser = loginUser;
            bool bRet = UserDetails.LoadUserPreferencesFromUserConfig(loginUser, dictCurrentUserUserPreferences, dictCurrentUserDevicePreferences);
            if (bRet)
            {
                bool bDisableCamera = DisableCamera;
                Utilities.DisableCamera(loginUser, bDisableCamera);
            }
            return bRet;
        }

        public bool loadUserPreferences(string userName, Dictionary<string, bool> dictUserPreferences, Dictionary<string, bool> dictDevicePreferences)
        {
            return UserDetails.LoadUserPreferencesFromUserConfig(userName, dictUserPreferences, dictDevicePreferences);
        }

        public bool saveUserPreferences(string userName, Dictionary<string, bool> dictUserPreferences, Dictionary<string, bool> dictDevicePreferences)
        {
            if (string.IsNullOrEmpty(userName))
                return false;

            return UserDetails.SaveUserPreferences(userName, dictUserPreferences, dictDevicePreferences);
        }

        public bool deleteUserPreferences(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return false;

            return UserDetails.DeleteUserPreferences(userName);
        }

        public bool UseAbbreviationsForProtocolNames
        {
            get
            {
                bool bUseAbbreviation = DefaultUseAbbreviationsForProtocolNames;
                if (dictCurrentUserUserPreferences.ContainsKey(TextUseAbbreviationsForProtocolNames))
                {
                    bUseAbbreviation = dictCurrentUserUserPreferences[TextUseAbbreviationsForProtocolNames];
                }
                return bUseAbbreviation;
            }
        }

        public bool LoadMRUProtocolsAtStartUp
        {
            get
            {
                bool bLoadMRU = DefaultLoadMRUProtocolAtStartUp;
                if (dictCurrentUserUserPreferences.ContainsKey(TextLoadMRUProtocolsAtStartUp))
                {
                    bLoadMRU = dictCurrentUserUserPreferences[TextLoadMRUProtocolsAtStartUp];
                }
                return bLoadMRU;
            }
        }

        public bool UseFirstTabToDisplayAllProtocolsList
        {
            get
            {
                bool bUseFirstTabToDisplayAllProtocolsList = DefaultUseFirstTabToDisplayAllProtocolsList;
                if (dictCurrentUserUserPreferences.ContainsKey(TextUseFirstTabToDisplayAllProtocolsList))
                {
                    bUseFirstTabToDisplayAllProtocolsList = dictCurrentUserUserPreferences[TextUseFirstTabToDisplayAllProtocolsList];
                }
                return bUseFirstTabToDisplayAllProtocolsList;
            }
        }

        public bool EnableProgressBarDetailsView
        {
            get
            {
                bool bEnableProgressBarDetailsView = DefaultEnableProgressBarDetailsView;
                if (dictCurrentUserUserPreferences.ContainsKey(TextEnableProgressBarDetailsView))
                {
                    bEnableProgressBarDetailsView = dictCurrentUserUserPreferences[TextEnableProgressBarDetailsView];
                }
                return bEnableProgressBarDetailsView;
            }
        }

        public bool SwitchToMyProtocolListAfterAddingAProtocol
        {
            get
            {
                bool bSwitchToMyProtolistAfterAddingAProtocol = DefaultSwitchToMyProtocolListAfterAddingAProtocol;
                if (dictCurrentUserUserPreferences.ContainsKey(TextSwitchToMyProtocolListAfterAddingAProtocol))
                {
                    bSwitchToMyProtolistAfterAddingAProtocol = dictCurrentUserUserPreferences[TextSwitchToMyProtocolListAfterAddingAProtocol];
                }
                return bSwitchToMyProtolistAfterAddingAProtocol;
            }
        }

        public bool SkipResourcesScreen
        {
            get
            {
                bool bSkipResourcesScreen = DefaultSkipResourcesScreen;
                if (dictCurrentUserUserPreferences.ContainsKey(TextSkipResourcesScreen))
                {
                    bSkipResourcesScreen = dictCurrentUserUserPreferences[TextSkipResourcesScreen];
                }
                return bSkipResourcesScreen;
            }
        }

        public bool EnableAutoScanBarcodes
        {
            get
            {
                bool bEnableAutoScanBarcodes = DefaultEnableAutoScanBarcodes;
                if (dictCurrentUserUserPreferences.ContainsKey(TextEnableAutoScanBarcodes))
                {
                    bEnableAutoScanBarcodes = dictCurrentUserUserPreferences[TextEnableAutoScanBarcodes];
                }
                return bEnableAutoScanBarcodes;
            }
        }

        public string KeyEnableAutoScanBarcodes
        {
            get
            {
                return TextEnableAutoScanBarcodes;
            }
        }

        public string KeySkipResourcesScreen
        {
            get
            {
                return TextSkipResourcesScreen;
            }
        }

        // Device settings
        public bool DisableCamera
        {
            get
            {
                bool bDisableCamera = DefaultDisableCamera;
                if (dictCurrentUserDevicePreferences.ContainsKey(TextDisableCamera))
                {
                    bDisableCamera = dictCurrentUserDevicePreferences[TextDisableCamera];
                }
                return bDisableCamera;
            }
        }

        public string KeyDisableCamera
        {
            get
            {
                return TextDisableCamera;
            }
        }
        public bool DeleteVideoLogFiles
        {
            get
            {
                bool bDeleteVideoLogFiles = DefaultDeleteVideoLogFiles;
                if (dictCurrentUserDevicePreferences.ContainsKey(TextDeleteVideoLogFiles))
                {
                    bDeleteVideoLogFiles = dictCurrentUserDevicePreferences[TextDeleteVideoLogFiles];
                }
                return bDeleteVideoLogFiles;
            }
        }

        public bool EnableBeepSwitch
        {
            get
            {
                bool bEnableBeepSwitch = DefaultEnableBeepSwitch;
                if (dictCurrentUserDevicePreferences.ContainsKey(TextEnableBeepSwitch))
                {
                    bEnableBeepSwitch = dictCurrentUserDevicePreferences[TextEnableBeepSwitch];
                }
                return bEnableBeepSwitch;
            }
        }
 
        public string KeyDeleteVideoLogFiles
        {
            get
            {
                return TextDeleteVideoLogFiles;
            }
        }


        public bool getUserAndDevicePreferences(string userName, Dictionary<string, bool> dictUserPreferences, Dictionary<string, bool> dictDevicePreferences)
        {
            if (string.IsNullOrEmpty(userName))
              return false;

            return loadUserPreferences(userName, dictUserPreferences, dictDevicePreferences);
        }

        public bool getDefaultUserAndDevicePreferences(Dictionary<string, bool> dictUserPreferences, Dictionary<string, bool> dictDevicePreferences)
        {
            return UserDetails.GetDefaultUserAndDevicePreferences(dictUserPreferences, dictDevicePreferences);
        }


        // Get section name
        private string[] GetSectionNames(string sFullNameFilePath)
        {
            if (string.IsNullOrEmpty(sFullNameFilePath))
                return null;

            List<string> SectionNames = new List<string>();
          
            string sectionName = String.Empty;
            string logMSG = String.Empty;
            if (!File.Exists(sFullNameFilePath))
            {
                return null;
            }
            else
            {// when file exists
                try
                {
                    StreamReader sr = new StreamReader(sFullNameFilePath, true);
                    string tempString = "";
                    while (!sr.EndOfStream)
                    {
                        try
                        {
                            tempString = sr.ReadLine();

                            if (!(tempString.Length == 0 || tempString == null || tempString == string.Empty))
                            {
                                if (tempString[0].Equals('/') || tempString[0].Equals('#') || (tempString[0].Equals('[') && tempString.Length < 2))
                                {
                                    continue;
                                }

                                // Create Sections
                                if (tempString[0].Equals('['))
                                {
                                    //
                                    // beginning of a section marks end of previous section
                                    //

                                    // previous section
                                    string temp = "";
                                    for (int i = 1; i < tempString.Length - 1; i++)
                                    {
                                        temp += tempString[i];
                                    }
                                    sectionName = temp;
                                    SectionNames.Add(sectionName);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // readline got a blank line
                            // LOG
                            LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);                               
                        }
                    }

                    sr.Close();

                }
                catch (Exception ex)
                {
                    // log exception
                    LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);                               
                }

            }
            return SectionNames.ToArray();
        }

        // Get section data
        private bool GetSectionData(string sFullNameFilePath, NameValueCollection sections)
        {
            if (string.IsNullOrEmpty(sFullNameFilePath) || sections == null)
                return false;

            bool bRet = true;

            List<string>  SectionNames = new List<string>();
            List<string> sectionData = new List<string>();

            string sectionName = String.Empty;
            string logMSG = String.Empty;
            sections.Clear();
            if (!File.Exists(sFullNameFilePath))
            {
                return false;
            }
            else
            {// when file exists
                try
                {
                    int nCount = 0;
                    StreamReader sr = new StreamReader(sFullNameFilePath, true);
                    string tempString = "";
                    while (!sr.EndOfStream)
                    {
                        try
                        {
                            tempString = sr.ReadLine();

                            if (!(tempString.Length == 0 || tempString == null || tempString == string.Empty))
                            {
                                if (!tempString[0].Equals('/') && !tempString[0].Equals('#') && !tempString[0].Equals('[') && tempString.Length > 3)
                                {
                                    sectionData.Add(tempString.Trim());
                                }
                                // Create Sections
                                else if (tempString[0].Equals('['))
                                {
                                    //
                                    // beginning of a section marks end of previous section
                                    //

                                    // previous section
                                    string temp = "";
                                    if (!string.IsNullOrEmpty(sectionName) && sectionData.Count > 0)
                                    {
                                        temp = string.Join(",", sectionData.ToArray());
                                        sections.Set(sectionName, temp);
                                     }
                                    // new section
                                    temp = String.Empty;
                                    for (int i = 1; i < tempString.Length - 1; i++)
                                    {
                                        temp += tempString[i];
                                    }
                                    sectionName = temp;
                                    sectionData.Clear();
                                    nCount++;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // readline got a blank line
                            // LOG
                            LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);                               
                        }
                    }
                    if ((nCount - sections.Count) == 1)
                    {
                        // Add last section data
                        if (!string.IsNullOrEmpty(sectionName))
                        {
                            string sName = Array.Find(sections.AllKeys, (x => { return (!string.IsNullOrEmpty(x) && x.ToLower() == sectionName.ToLower()); }));
                            if (string.IsNullOrEmpty(sName))
                            {
                                string temp = String.Empty;
                                if (sectionData.Count > 0)
                                {
                                    temp = string.Join(",", sectionData.ToArray());
                                }
                                sections.Set(sectionName, temp);
                            }
                        }
                    }
                    sr.Close();
                }
                catch (Exception ex)
                {
                    // log exception
                    LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);                               
                }

            }
            return bRet;
        }

        public void BuildProtocolPresetsForUserInfoINIFile()
        {
            // First find whether the preset sections exists in userinfo.ini file
            string fullPathUserFileName = sysPath;

            fullPathUserFileName += "config\\";
            fullPathUserFileName += "protocolPresets.txt";

            if (!File.Exists(fullPathUserFileName))
            {
                return;
            }

            string[] presetNames = null;
            if (lstPresetNames == null)
            {
                lstPresetNames = new List<string>();
                presetNames = GetSectionNames(fullPathUserFileName);
                if (presetNames == null || presetNames.Length == 0)
                    return;

                lstPresetNames.AddRange(presetNames);
            }
            else
            {
                presetNames = lstPresetNames.ToArray();
            }
            fullPathUserFileName = sysPath;
            fullPathUserFileName += "protocols\\";
            fullPathUserFileName += "UserInfo.ini";

            if (!File.Exists(fullPathUserFileName))
            {
                return;
            }

            List<string> lstPresetNamesToBeAdded = new List<string>();
            string[] sectionNames = GetSectionNames(fullPathUserFileName);
            if (sectionNames == null || sectionNames.Length == 0)
            {
                lstPresetNamesToBeAdded.AddRange(presetNames);
            }
            else
            {
                // Verify whether presets exist
                for (int i = 0; i < presetNames.Length; i++)
                {
                    if (string.IsNullOrEmpty(presetNames[i]))
                        continue;

                    string sName = Array.Find(sectionNames, (x => { return (!string.IsNullOrEmpty(x) && x.ToLower() == presetNames[i].ToLower()); }));
                    if (string.IsNullOrEmpty(sName))
                    {
                        // Add preset
                        lstPresetNamesToBeAdded.Add(presetNames[i]);
                    }
                }
            }

            // Put presets into userinfo.ini file 
            if (lstPresetNamesToBeAdded.Count > 0)
            {
                Dictionary<string,string[]> dictPresets = new Dictionary<string,string[]>();
                getAllProtocolPresetsFileNames(dictPresets);

                for (int i = 0; i < lstPresetNamesToBeAdded.Count; i++)
                {
                    if (string.IsNullOrEmpty(lstPresetNamesToBeAdded[i]))
                        continue;

                    foreach (KeyValuePair<string, string[]> kvp in dictPresets)
                    {
                        if (kvp.Key.ToLower() == lstPresetNamesToBeAdded[i].ToLower())
                        {
                            writePresetToUserInfoINIFile(kvp.Key, kvp.Value);
                        }
                    }
                }
            }
            return;
        }

        public bool writePresetToUserInfoINIFile(string presetName, string[] protocolFileNames)
        {
            if (string.IsNullOrEmpty(presetName) || protocolFileNames == null || protocolFileNames.Length == 0)
                return false;

            string userName = presetName.Trim();
            string[] addSection = new string[protocolFileNames.Length + 2];
            addSection[0] = "\n[" + userName + "]";
            addSection[1] = userName + "Image = Default";

            for (int i = 0; i < protocolFileNames.Length; i++)
            {
                addSection[i + 2] = userName + "Protocol = " + protocolFileNames[i];
            }

            UserDetails.addSection("USER", addSection);
            return true;
        }
 
        public bool getAllProtocolPresetsFileNames(Dictionary<string, string[]> dictProtocolPresets)
        {
            if (dictProtocolPresets == null || lstAllProtocols == null || lstAllProtocols.Count == 0)
                return false;


            string fullPathUserFileName = sysPath;

            fullPathUserFileName += "config\\";
            fullPathUserFileName += "protocolPresets.txt";

            if (!File.Exists(fullPathUserFileName))
            {
                return false;
            }

            bool bRet = true;
            string logMSG = "";
            try
            {
                NameValueCollection nvcSections = new NameValueCollection();
                bRet = GetSectionData(fullPathUserFileName, nvcSections);
                if (!bRet || nvcSections.Count == 0)
                {
                    return false;
                }

                // Determine the distinct tokens
                //enumerate the keys
                string temp;
                string[] parts;
                char[] separator = new char[]{','};
                string pattern = @"^(\d+)(.*)$";

                Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                List<string> lstDistinctWords = new List<string>();
                foreach (string key in nvcSections)
                {
                    if (string.IsNullOrEmpty(key))
                        continue;

                    temp = nvcSections[key];
                    if (string.IsNullOrEmpty(temp))
                        continue;

                    parts = temp.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    if (parts == null || parts.Length == 0)
                        continue;

                    for (int i = 0; i < parts.Length; i++)
                    {
                        var match = rgx.Match(parts[i]);
                        if (2 < match.Groups.Count)
                        {
                            string s1 = match.Groups[2].Value;
                            if (!string.IsNullOrEmpty(match.Groups[2].Value))
                            {
                                bool bAddToList = true;
                                if (lstDistinctWords.Count > 0)
                                {
                                    int index = lstDistinctWords.FindIndex(x => { return (!string.IsNullOrEmpty(x) && x.ToLower().Contains(s1.Trim().ToLower())); });
                                    if (0 <= index)
                                    {
                                        bAddToList = false;
                                    }
                                }
                                if (bAddToList)
                                    lstDistinctWords.Add(s1);
                                 
                            }
                        }
                    }
                }

                dictProtocolPresets.Clear();

                List<string> lstProtocolFileNames = new List<string>();

                //enumerate the keys
                foreach (string key in nvcSections)
                {
                    if (string.IsNullOrEmpty(key))
                        continue;

                    temp = nvcSections[key];
                    if (string.IsNullOrEmpty(temp))
                        continue;

                    parts = temp.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    if (parts == null || parts.Length == 0)
                        continue;

                    // Clear the list
                    lstProtocolFileNames.Clear();

                    int index = -1;
                    for (int i = 0; i < parts.Length; i++)
                    {
                        temp = parts[i];

                        string sTemp1, sTemp2 = "";
                        for (int k = 0; k < lstDistinctWords.Count; k++)
                        {
                            int nStart = temp.IndexOf(lstDistinctWords[k]);
                            bool bAdd = true;
                            if (0 <= nStart)
                            {
                                sTemp1 = temp.Substring(nStart, temp.Length - nStart);
                                if (sTemp1 == lstDistinctWords[k])
                                {
                                    bAdd = false;
                                }
                            }
                            if (bAdd)
                            {
                                if (!string.IsNullOrEmpty(sTemp2))
                                {
                                    sTemp2 += "|";
                                }
                                sTemp2 += lstDistinctWords[k];
                            }

                        }
                        string sExclude = String.Empty;
                        if (!string.IsNullOrEmpty(sTemp2))
                        {
                            sExclude = String.Format("(?!({0}))", sTemp2);  // use negative lookahead
                            sExclude = sExclude.ToLower();
                        }

                        int nResult = 0;
                        if (Int32.TryParse(parts[i], out nResult))
                        {
                            pattern = String.Format("(\\b)(?i)({0}){1}(\\D)+(\\W|$)", parts[i], sExclude); // Could be more than one types  
                        }
                        else
                        {
                            pattern = String.Format("(\\b)(?i)({0})(\\W|$)", parts[i]); // Exact match
                        }

                        index = 0;
                        do
                        {
                            index = lstAllProtocols.FindIndex(index, x => { return (!string.IsNullOrEmpty(x.Protocol_FileName) && x.Protocol_FileName.ToLower().Contains(parts[i].Trim().ToLower())); });
                            if (0 <= index)
                            {
                                var match = Regex.Match(lstAllProtocols[index].Protocol_FileName, pattern);
                                if (match.Groups.Count > 1 && !string.IsNullOrEmpty(match.Groups[0].Value))
                                {
                                    lstProtocolFileNames.Add(lstAllProtocols[index].Protocol_FileName);
                                }

                                index++;
                            }
                        }
                        while (0 <= index);
                    }

                    temp = key.Trim();
                    if (!dictProtocolPresets.ContainsKey(temp))
                    {
                        dictProtocolPresets.Add(temp, lstProtocolFileNames.ToArray());
                    }
                }
     
            }
            catch (Exception ex)
            {
                // LOG
                LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);                               
                return false;
            }
            return true;
        }

        public bool IsPresetUser(string sName)
        {
            bool bPreset = false;
            if (!string.IsNullOrEmpty(sName))
            {
                if (lstPresetNames != null && lstPresetNames.Count > 0)
                {
                    int index = lstPresetNames.FindIndex((x => { return (!string.IsNullOrEmpty(x) && x.ToLower() == sName.ToLower()); }));
                    if (0 <= index)
                    {
                        bPreset = true;
                    }
                }
            }
            return bPreset;
        }


        public bool IsPresetProtocol(string sProtocolFileName)
        {
            if (string.IsNullOrEmpty(sProtocolFileName))
                return false;

            string fullPathUserFileName = RoboSep_UserDB.getInstance().sysPath;

            fullPathUserFileName += "protocols\\";
            fullPathUserFileName += UserInfoFileName;

            if (!File.Exists(fullPathUserFileName))
            {
                return false;
            }

            string logMSG = "";
            bool bPresetProtocol = false;

            try
            {
                if (lstPresetNames != null && lstPresetNames.Count > 0)
                {
                    NameValueCollection nvcSections = new NameValueCollection();
                    string temp;
                    string[] parts;
                    foreach  (string sPresetName in lstPresetNames)
                    {
                        nvcSections.Clear();
                        nvcSections =  UserDetails.getSection("USER", sPresetName);
                        if (nvcSections== null || nvcSections.Count == 0)
                        {
                            continue;
                        }
      
                        string UserProtocolPrefix = sPresetName.Trim() + ProtocolSuffix;

                        //enumerate the keys
                        foreach (string key in nvcSections)
                        {
                            if (string.IsNullOrEmpty(key))
                                continue;

                            if (key.ToLower() == UserProtocolPrefix.ToLower())
                            {
                                temp = nvcSections[key];
                                if (string.IsNullOrEmpty(temp))
                                    continue;

                                parts = temp.Split(',');
                                if (parts != null && 0 < parts.Length)
                                {
                                    string sName = Array.Find(parts, (x => { return (!string.IsNullOrEmpty(x) && x.ToLower() == sProtocolFileName.ToLower()); }));
                                    if (!string.IsNullOrEmpty(sName))
                                    {
                                        bPresetProtocol = true;
                                    }
                                }
                                break;
                            }
                        }

                        if (bPresetProtocol)
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write("IsPresetProtocol : Failed to get preset protocols.");
                LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);                               
            }
            return bPresetProtocol;
        }

        public string[] GetProtocolPresetNames()
        {
            if (lstPresetNames == null )
            {
                // First find whether the preset sections exists in userinfo.ini file
                string fullPathUserFileName = sysPath;

                fullPathUserFileName += "config\\";
                fullPathUserFileName += "protocolPresets.txt";

                if (!File.Exists(fullPathUserFileName))
                {
                    return null;
                }

                lstPresetNames = new List<string>();
                string[] presetNames = GetSectionNames(fullPathUserFileName);
                if (presetNames == null || presetNames.Length == 0)
                    return null;

                lstPresetNames.AddRange(presetNames);
            }
            if (lstPresetNames.Count == 0 )
            {
                return null;
            }
            return lstPresetNames.ToArray();
        }


        #region OLD ROBOSEP FUCTIONS FOR XML READ/WRITE

        string myCurrentActionContext = string.Empty;
        static private XmlSerializer myXmlSerializer = new XmlSerializer(typeof(RoboSepUser));

        // UN-NEEDED FUNCTION
        // reads config file to get current user
        private void XML_LoadCurrentUser()
        {
            RoboSepUserConfig config = null;
            if (File.Exists(myUserConfigPath))
            {
                // Initialise a file stream for reading
                FileStream fs = new FileStream(myUserConfigPath, FileMode.Open);

                try
                {
                    // Deserialize a RoboSepProtocol XML description into a RoboSepProtocol 
                    // object that matches the contents of the specified protocol file.	

                    //  Oct 1, 2013 Sunny commented out
                    //  These codes are obsolete
				    /*
                    XmlReader reader = new XmlTextReader(fs);

                    // Create a validating reader to process the file.  Report any errors to the 
                    // validation page.
                    XmlValidatingReader validatingReader = new XmlValidatingReader(reader);
                    validatingReader.ValidationType = ValidationType.Schema;

                    // Get the RoboSep protocol schema and add it to the collection for the 
                    // validator
                    XmlSchemaCollection xsc = new XmlSchemaCollection();
                    xsc.Add("STI", myXSDPath);
                    validatingReader.Schemas.Add(xsc);

                    // 'Rehydrate' the object (that is, deserialise data into the object)					
                    config = (RoboSepUserConfig)myXmlSerializer.Deserialize(validatingReader);
 
                    */

                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.ValidationType = ValidationType.Schema;
                    settings.Schemas.Add("STI", myXSDPath);
                    XmlReader reader = XmlReader.Create(fs, settings);
                    config = (RoboSepUserConfig)myXmlSerializer.Deserialize(reader);

                    myCurrentActionContext = "User1.udb";
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Write("RoboSep_UserDB : Failed to load User profile.");
                    string sMsg = String.Format("XML_LoadCurrentUser failed. User Config File Invalid. Exception: {0}", ex.Message);
                    MessageBox.Show(sMsg);
                    myCurrentActionContext = "User1.udb";
                }
                finally
                {
                    // Close the file stream
                    fs.Close();
                }
            }
            else
            {

                myCurrentActionContext = "User1.udb";
            }

        }

        // saves Current User to user.config file
        public void XML_SaveUserConfig()
        {
            FileStream fs = null;
            XmlTextWriter writer = null;

            // LOG
            string logMSG = "set user.db file to : User1.udb";
            //GUI_Controls.uiLog.LOG(this, "XML_SaveUserConfig", uiLog.LogLevel.INFO, logMSG);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG); 
            XmlSerializer thisXmlSerializer = new XmlSerializer(typeof(RoboSepUserConfig));

            string startupPath = Application.StartupPath;
            int lastBackslashIndex = startupPath.LastIndexOf('\\');  // must escape the '\'
            //startupPath.Remove(lastBackslashIndex, startupPath.Length - lastBackslashIndex);
            //ConfigPath += @"\config\UserConfig.config";
            string ConfigPath = mySystemPath + "config\\";
            ConfigPath += myUserConfigPath;


            //string ConfigPath = myUserConfigPath;

            RoboSepUserConfig config = new RoboSepUserConfig();
            config.CurrentUser = "User1.udb";
            
            try
            {
                fs = new FileStream(/*myUser*/ConfigPath, FileMode.Create,FileAccess.ReadWrite);
                writer = new XmlTextWriter(fs, new UTF8Encoding());
                writer.Formatting = Formatting.Indented;
                thisXmlSerializer.Serialize(writer, config);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
                if (fs != null)
                    fs.Close();
            }
        }

        // use to check if save to UDB works
        public string[] XML_LoadUserProfile()
        {
            string[] protocols;
            RoboSepUser user = XML_LoadRoboSepUserObject();
            if (user != null)
            {
                //txtUserName.Text = user.UserName;
                if (user.ProtocolFile != null)
                {
                    protocols = new string[user.ProtocolFile.Length];
                    for (int i = 0; i < user.ProtocolFile.Length; ++i)
                    {
                        if (File.Exists(myProtocolsPath + user.ProtocolFile[i]))
                        //lstUserProtocols.Items.Add(user.ProtocolFile[i]);
                        {
                            protocols[i] = user.ProtocolFile[i];
                        }
                    }
                    
                    // LOG
                    string logMSG = "loading " + protocols.Length.ToString() + " protocols from User1.udb" ;
                    //GUI_Controls.uiLog.LOG(this, "XML_LoadUserProfile", uiLog.LogLevel.DEBUG, logMSG);
                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);            

                    return protocols;
                }
            }
            // LOG
            string LOGmsg = "Failed to load protocols from user db file";
            //GUI_Controls.uiLog.LOG(this, "XML_LoadUserProfile", uiLog.LogLevel.WARNING, LOGmsg);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Warning, LOGmsg);

            return null;
        }

        // called by other functions, returns robosep_user object
        public RoboSepUser XML_LoadRoboSepUserObject()
        {
            string UserDBFilePath = myUserDB.GetProtocolsPath() + myUserDBFile + USER_DB_EXTENSION;
            RoboSepUser user = null;
            //string pp = "C:\\BACKUP\\SOURCE\\SOURCE_Sept14\\separator\\bin\\Debug\\..\\protocols\\";
            if (File.Exists(UserDBFilePath))
            {

                try
                {
                    using (FileStream fs = new FileStream(UserDBFilePath, FileMode.Open))
                    {
                        // Deserialize a RoboSepProtocol XML description into a RoboSepProtocol 
                        // object that matches the contents of the specified protocol file.	

                        //  Oct 1, 2013 Sunny commented out
                        //  These codes are obsolete
			            /*
                        XmlReader reader = new XmlTextReader(fs);

                        // Create a validating reader to process the file.  Report any errors to the 
                        // validation page.
                         XmlValidatingReader validatingReader = new XmlValidatingReader(reader);
                         validatingReader.ValidationType = ValidationType.Schema;

                        // Get the RoboSep protocol schema and add it to the collection for the 
                        // validator
                        XmlSchemaCollection xsc = new XmlSchemaCollection();
                        xsc.Add("STI", myUserDB.myXSDPath);
                        validatingReader.Schemas.Add(xsc);
 
                        // 'Rehydrate' the object (that is, deserialise data into the object)					
                        user = (RoboSepUser)myXmlSerializer.Deserialize(validatingReader);
                        */


                        XmlReaderSettings settings = new XmlReaderSettings();
                        settings.ValidationType = ValidationType.Schema;
                        settings.Schemas.Add("STI", myUserDB.myXSDPath);
                        XmlReader reader = XmlReader.Create(fs, settings);
                        user = (RoboSepUser)myXmlSerializer.Deserialize(reader);
                    }

                }
                catch (Exception ex)
                {
                    // LOG
                    //string logMSG = "error loading" + UserDBFilePath;
                    LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);                               
                    MessageBox.Show("User DB File Invalid");
                }
            }
            else
            {
                user = XML_LoadRoboSepUserObject();
            }
            // LOG
            string LOGmsg = "Loaded Robosep User";
            //GUI_Controls.uiLog.LOG(this, "XML_LoadRoboSepUserObject", uiLog.LogLevel.DEBUG, LOGmsg);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, LOGmsg);            

            return user;
        }

        // takes user data and creates udb out of it (stores to user1.udb)
        public void XML_SaveUserProfile(string userName, string[] protocols)
        {
            FileStream fs = null;
            XmlTextWriter writer = null;

            // set user config to point to User1.udb;
            XML_SaveUserConfig();

            // delete old files
            string[] usrDBfiles = new string[3] { 
                (myProtocolsPath + myUserDBFile + ".udb"),
                (myProtocolsPath + myUserDBFile + "_profile.db"),
                (myProtocolsPath + myUserDBFile + "_protocols.db")};
            
            for (int fileNum = 0; fileNum < usrDBfiles.Length; fileNum++)
            {
                if (File.Exists(usrDBfiles[fileNum]))
                {
                    File.Delete(usrDBfiles[fileNum]);
                }
            }

            // save user name & protocols to XML User1.udb
            RoboSepUser user = new RoboSepUser();
            user.UserName = userName;
            user.ProtocolFile = new string[protocols.Length];
            for (int i = 0; i < protocols.Length; ++i)
            {
                user.ProtocolFile[i] = protocols[i];
            }
            if (File.Exists(myProtocolsPath + myUserDBFile + USER_DB_EXTENSION))
                File.Delete(myProtocolsPath + myUserDBFile + USER_DB_EXTENSION);
            try
            {
                fs = new FileStream(usrDBfiles[0], FileMode.Create);
                writer = new XmlTextWriter(fs, new UTF8Encoding());
                writer.Formatting = Formatting.Indented;
                myXmlSerializer.Serialize(writer, user);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
                if (fs != null)
                    fs.Close();
            }
            // LOG
            string logMSG = "Saved User Profile " + userName + " with " + protocols.Length.ToString() + " protocols";
            //GUI_Controls.uiLog.LOG(this, "XML_SaveUserProfile", uiLog.LogLevel.DEBUG, logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);            
        }

        // gets user name of current user
        static public string XML_UserProfileDBToName()
        {
            string filepath = RoboSep_UserDB.getInstance().myProtocolsPath + RoboSep_UserDB.getInstance().myUserDBFile + ".udb";
            if (!File.Exists(filepath))
            {// create empty user.db file
                // LOG
                string logMSG = "User db does not exist, generating default files";
                //GUI_Controls.uiLog.LOG(RoboSep_UserDB.getInstance(), "XML_UserProfileDBToName", uiLog.LogLevel.WARNING, logMSG);
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Warning, logMSG);

                myUserDB.XML_SaveUserProfile("UserName", new string[] { string.Empty });
            }
            RoboSepUser user = RoboSep_UserDB.getInstance().XML_LoadRoboSepUserObject();

            return user == null ? System.IO.Path.GetFileNameWithoutExtension(filepath) : user.UserName;
        }

        #endregion
    }
}
