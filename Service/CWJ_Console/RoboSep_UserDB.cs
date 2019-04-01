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

using Tesla.Common.ResourceManagement;

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
        // !!Hardcode
        //public string myXSDPath = Application.StartupPath + "\\..\\config\\RoboSepUser.xsd";
        public string myXSDPath;
        public string mySystemPath;
        // !!Hardcode
        static private string myUserConfigPath = "UserConfig.config";
        private List<RoboSep_Protocol> lstAllProtocols;
        private string[] arrsAllUsers;
        private string myUserDBFile = "User1.udb";

        // define static constants
        private const string USER_DB_EXTENSION = "udb";

        private int getMyOsinfo()
        {
            int DataBit = -1;
            ManagementObjectSearcher objMOS = new ManagementObjectSearcher("SELECT * FROM  Win32_OperatingSystem");
            object osA = null;
            try
            {
                foreach (ManagementObject objManagement in objMOS.Get())
                {
                    osA = objManagement.GetPropertyValue("OSArchitecture");
                    if (osA != null)
                    {
                        string osAString = osA.ToString();
                        // If "64" is anywhere in there, it's a 64-bit architectore.
                        DataBit = (osAString.Contains("64") ? 64 : 32);
                        //DataBit = 32;
                        iSystemArchitecture =  DataBit;
                    }

                }
            }
            catch (Exception)
            {
                return 32;
            }
            return DataBit;
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
                    GUI_Controls.uiLog.LOG(this, "getOSArchitecture", uiLog.LogLevel.GENERAL, logMSG);

                    return iSystemArchitecture;
                    //return 32;
                }
            }
            catch (Exception e)
            {
                // LOG
                string logMSG = "Exception, return Defaulted of 32 Bit";
                GUI_Controls.uiLog.LOG(this, "getOSArchitecture", uiLog.LogLevel.ERROR, logMSG);

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
            GUI_Controls.uiLog.LOG(this, "Robosep_UserDB()", uiLog.LogLevel.DEBUG, logMSG);

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
            
            arrsAllUsers = getAllUsers();
            
            myProtocolsPath = GetProtocolsPath();
            // !!Hardcoded
            //myProtocolsPath = "protocols\\";
            lstAllProtocols = new List<RoboSep_Protocol>();
            LoadAllProtocols();
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

        public string GetProtocolsPath()
        {
            // Check the "protocols" directory exists, otherwise create it
#if (False)
            string startupPath = Application.StartupPath;
            int lastBackslashIndex = startupPath.LastIndexOf('\\');  // must escape the '\'
            startupPath = startupPath.Remove(lastBackslashIndex, startupPath.Length - lastBackslashIndex);
            string protocolsPath = startupPath + @"\protocols\";
#else
            if (mySystemPath == null)
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
            GUI_Controls.uiLog.LOG(this, "GetProtocolsPath", uiLog.LogLevel.DEBUG, logMSG);

            return protocolsPath;

            // !!Hardcoded
            //return myProtocolsPath;
        }

        private void LoadAllProtocols()
        {
            DirectoryInfo di = new DirectoryInfo(myProtocolsPath);
            FileInfo[] rgFiles = di.GetFiles("*.xml");
            foreach (FileInfo fi in rgFiles)
            {
                if (!fi.Name.Equals("prime.xml") &&
                    !fi.Name.Equals("shutdown.xml") &&
                    !fi.Name.Equals("home_axes.xml"))
                {
                    // create protocols object to represent each file
                    // add protocols to lstAllProtocols
                    RoboSep_Protocol tempProtocol = RoboSep_Protocol.generateProtocolFromFile(fi);
                    lstAllProtocols.Add(tempProtocol);
                }
            }
            // LOG
            string logMSG = lstAllProtocols.Count.ToString() + "protocols loaded";
            GUI_Controls.uiLog.LOG(this, "LoadAllProtocols", uiLog.LogLevel.DEBUG, logMSG);
        }

        public List<RoboSep_Protocol> reloadProtocols()
        {
            lstAllProtocols.Clear();
            LoadAllProtocols();

            // LOG
            string logMSG = lstAllProtocols.Count.ToString() + "protocols re-loaded";
            GUI_Controls.uiLog.LOG(this, "reloadProtocols", uiLog.LogLevel.DEBUG, logMSG);

            return lstAllProtocols;
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

        public void getProtocolCommandList(string fileName, out int[] cmds, out int[] times, out string[] descriptions)
        {
            int[] commands = null;
            int[] estTimes = null;
            string[] desc = null;
            int cmdNum = 0;

            string[] cmdStrings = new string[] 
                    {
                        
                        "IncubateCommand",                        
                        "MixCommand",
                        "TransportCommand",
                        "SeparateCommand",
                        "ResuspendVialCommand",
                        "TopUpVialCommand",
                        "FlushCommand",
                        "PrimeCommand",
                        "HomeAllCommand",
                        "DemoCommand",
                        "ParkCommand",
                        "PumpLifeCommand"
                    };
            int[] cmdTimes = new int[]
                    {
                        -1, 37, 70, -1, 70,  70, 10, 25, 30, 200, 10, 200
                    };

            string Filepath = GetProtocolsPath() + fileName;
            
            // open file, grab information from file
            if (File.Exists(Filepath))
            {
                using (StreamReader sr = new StreamReader(Filepath))
                {
                    // get rid of 1st line
                    sr.ReadLine();
                    string line;
                    // get set of times from cfg file to match command names.
                    // non defined values should be available in .xml file
                    
                    // read line with file info
                    while ((line = sr.ReadLine()) != null)
                    {
                        // split up line by
                        if (commands == null)
                        {
                            string[] lineBreakdown = line.Split(new char[] { '=', '<', '>' });
                            for (int i = 0; i < lineBreakdown.Length; i++)
                            {
                                // first find how many commands there are going to be
                                if (lineBreakdown[i] == "commands number")
                                {
                                    string[] bd = lineBreakdown[i + 1].Split('"');
                                    if (Convert.ToInt32(bd[1]) > 0)
                                    {
                                        commands = new int[Convert.ToInt32(bd[1])];
                                        estTimes = new int[Convert.ToInt32(bd[1])];
                                        desc = new string[Convert.ToInt32(bd[1])];
                                        break;
                                    }
                                }
                            }
                        }
                        // add commands and times
                        else if (cmdNum >= commands.Length)
                            break;
                        else
                        {
                            int isCommand = -1;
                            string[] lineBreakdown = line.Split(new char[] { ' ', '<', '>' });
                            string[] getDescription = line.Split('"');
                            for (int i = 0; i < cmdStrings.Length; i++)
                            {
                                if (lineBreakdown[5] == cmdStrings[i])
                                {
                                    isCommand = i;
                                    break;
                                }
                            }
                            if (isCommand >= 0)
                            {
                                desc[cmdNum] = getDescription[3];
                                commands[cmdNum] = isCommand;
                                if (estTimes[cmdNum] == 0)
                                    estTimes[cmdNum] = cmdTimes[isCommand];
                                for (int ii = 0; ii < cmdNum; ii++)
                                {
                                    if (estTimes[ii] < 0)
                                        estTimes[ii] = 1000;
                                }
                                cmdNum++;
                            }
                            else
                            {
                                // if is not command line, look for times and volumes
                                lineBreakdown = line.Split(new char[] { '<', '=', '"' });
                                if (lineBreakdown[1] == "processingTime duration")
                                {
                                    estTimes[cmdNum-1] = Convert.ToInt32(lineBreakdown[3]);
                                }
                                else if (lineBreakdown[1] == "absoluteVolume value_uL")
                                {
                                    // figure out transport time
                                }
                            }
                        }
                    }
                }
            }
            cmds = commands;
            times = estTimes;
            descriptions = desc;

            // LOG
            string logMSG = "returned " + commands.Length.ToString() + "commands";
            GUI_Controls.uiLog.LOG(this, "getProtocolCommandList", uiLog.LogLevel.DEBUG, logMSG);
        }

        

        public string[] getAllUsers()
        {

            string[] users = fromINI.getAllSections("USER");
            return users;
        }

        public List<RoboSep_Protocol> getAllProtocols()
        {
            return lstAllProtocols;
        }

        public void saveUserProtocols(string userName, string[] userProtocols)
        {
            // LOG
            string logMSG = "saving " + userProtocols.Length.ToString() + "protocols to user " + userName;
            GUI_Controls.uiLog.LOG(this, "saveUserProtocols", uiLog.LogLevel.INFO, logMSG);

            // edit strings to fit ini format
            string[] userProtocolsForINI = new string[userProtocols.Length];

            for (int i = 0; i < userProtocolsForINI.Length; i++)
            {
                userProtocolsForINI[i] = userName + "Protocol = " + userProtocols[i];
            }
            // if user name exissts save section

            if (userNameExists(userName))
            {
                fromINI.saveSection("USER", userName, userProtocolsForINI);
            }
            // if user name doesn't exist, add section
            else
            {
                string[] addSection = new string[userProtocols.Length + 2];
                addSection[0] = "";
                addSection[1] = "[" + userName + "]";
                userProtocolsForINI.CopyTo(addSection,2);
                fromINI.addSection("USER", addSection);
            }

            //
            // Save over User1.udb
            //
            RoboSep_UserDB.getInstance().XML_SaveUserProfile(RoboSep_UserConsole.strCurrentUser, userProtocols);
            // test for proper save
            if (RoboSep_UserDB.XML_UserProfileDBToName() != RoboSep_UserConsole.strCurrentUser)
            {
                GUI_Controls.RoboMessagePanel newPrompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(),
                    "Failed to save file to User1.udb");
                RoboSep_UserConsole.getInstance().addForm(newPrompt, newPrompt.Offset);
                RoboSep_UserConsole.getInstance().frmOverlay.Show();
                newPrompt.ShowDialog();
                RoboSep_UserConsole.getInstance().frmOverlay.Hide();
            }

            // update user list
            arrsAllUsers = getAllUsers();
        }

        public List<RoboSep_Protocol> loadUserProtocols(string userName)
        {
            // get list from INI
            List<RoboSep_Protocol> rpTempList = new List<RoboSep_Protocol>();
            NameValueCollection nvc = fromINI.getSection("USER", userName);
            string key = userName + "Protocol";
            try
            {
                string[] temp = nvc.GetValues(key);
                string[] userProtocols = temp[0].Split(',');

                // add protocols that match strings to list
                foreach (string s in userProtocols)
                {
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
                        GUI_Controls.uiLog.LOG(this, "loadUserProtocols", uiLog.LogLevel.DEBUG, logMSG);

                        string sMSG = "Failed to add protocol \n'" + s + "'\nas protocol does not exist in current"
                            + " protocol directory.  To re-add protocol from your USB, click on the USB load button"
                            + " on your edit protocol list > all protocols page.";
                        RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), sMSG);
                        RoboSep_UserConsole.getInstance().frmOverlay.Show();
                        prompt.ShowDialog();
                        RoboSep_UserConsole.getInstance().frmOverlay.Hide();
                    }
                }

                // LOG
                string LOGMSG = "Loaded " + rpTempList.Count.ToString() + " protocol from User " + userName;
                GUI_Controls.uiLog.LOG(this, "loadUserProtocols", uiLog.LogLevel.DEBUG, LOGMSG);

                return rpTempList;
            }
            catch
            {
                // LOG
                string logMSG = "Failed to load " + userName + "protocol list";
                GUI_Controls.uiLog.LOG(this, "loadUserProtocols", uiLog.LogLevel.ERROR, logMSG);
                
                return null;
            }
        }

        private bool userNameExists(string userName)
        {
            // update user list
            arrsAllUsers = getAllUsers();
            // checks all user names to see if given user name exists
            bool exists = false;
            try
            {
                for (int i = 0; i < arrsAllUsers.Length; i++)
                {
                    if (userName == arrsAllUsers[i])
                    {
                        exists = true;
                    }
                }
                return exists;
            }
            catch
            {
                return exists;
            }
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
                    myCurrentActionContext = "User1.udb";
                }
                catch (Exception /*ex*/)
                {
                    MessageBox.Show("User Config File Invalid");
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
            // LOG
            string logMSG = "set user.db file to : User1.udb";
            GUI_Controls.uiLog.LOG(this, "XML_SaveUserConfig", uiLog.LogLevel.INFO, logMSG);

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

            using (FileStream fs = new FileStream(/*myUser*/ConfigPath, FileMode.Create))
            {
                XmlTextWriter writer = new XmlTextWriter(fs, new UTF8Encoding());
                writer.Formatting = Formatting.Indented;
                thisXmlSerializer.Serialize(writer, config);
            }

        }

        // use to check if save to UDB works
        private string[] XML_LoadUserProfile()
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
                    GUI_Controls.uiLog.LOG(this, "XML_LoadUserProfile", uiLog.LogLevel.DEBUG, logMSG);

                    return protocols;
                }
            }
            // LOG
            string LOGmsg = "Failed to load protocols from user db file";
            GUI_Controls.uiLog.LOG(this, "XML_LoadUserProfile", uiLog.LogLevel.WARNING, LOGmsg);

            return null;
        }

        // called by other functions, returns robosep_user object
        public RoboSepUser XML_LoadRoboSepUserObject()
        {
            string UserDBFilePath = myUserDB.GetProtocolsPath() + myUserDBFile;
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
                    }

                }
                catch (Exception /*ex*/)
                {
                    // LOG
                    string logMSG = "error loading" + UserDBFilePath;
                    GUI_Controls.uiLog.LOG(this, "XML_LoadRoboSepUserObject", uiLog.LogLevel.ERROR, logMSG);

                    MessageBox.Show("User DB File Invalid");
                }
            }
            else
            {
                user = XML_LoadRoboSepUserObject();
            }
            // LOG
            string LOGmsg = "Loaded Robosep User";
            GUI_Controls.uiLog.LOG(this, "XML_LoadRoboSepUserObject", uiLog.LogLevel.DEBUG, LOGmsg);

            return user;
        }

        // takes user data and creates udb out of it (stores to user1.udb)
        public void XML_SaveUserProfile(string userName, string[] protocols)
        {
            // set user config to point to User1.udb;
            XML_SaveUserConfig();

            // save user name & protocols to XML User1.udb
            RoboSepUser user = new RoboSepUser();
            user.UserName = userName;
            user.ProtocolFile = new string[protocols.Length];
            for (int i = 0; i < protocols.Length; ++i)
            {
                user.ProtocolFile[i] = protocols[i];
            }
            if (File.Exists(myProtocolsPath + myUserDBFile))
                File.Delete(myProtocolsPath + myUserDBFile);
            using (FileStream fs = new FileStream(myProtocolsPath + myUserDBFile, FileMode.Create))
            {
                XmlTextWriter writer = new XmlTextWriter(fs, new UTF8Encoding());
                writer.Formatting = Formatting.Indented;
                myXmlSerializer.Serialize(writer, user);
                fs.Close();
            }
            // LOG
            string logMSG = "Saved User Profile " + userName + " with " + protocols.Length.ToString() + "protocols";
            GUI_Controls.uiLog.LOG(this, "XML_SaveUserProfile", uiLog.LogLevel.DEBUG, logMSG);
        }

        // gets user name of current user
        static public string XML_UserProfileDBToName()
        {
            string filepath = RoboSep_UserDB.getInstance().myProtocolsPath + RoboSep_UserDB.getInstance().myUserDBFile;
            if (!File.Exists(filepath))
            {// create empty user.db file
                // LOG
                string logMSG = "User db does not exist, generating default files";
                GUI_Controls.uiLog.LOG(RoboSep_UserDB.getInstance(), "XML_UserProfileDBToName", uiLog.LogLevel.WARNING, logMSG);

                myUserDB.XML_SaveUserProfile("UserName", new string[] { string.Empty });
            }
            RoboSepUser user = RoboSep_UserDB.getInstance().XML_LoadRoboSepUserObject();

            return user == null ? System.IO.Path.GetFileNameWithoutExtension(filepath) : user.UserName;
        }

        #endregion
    }
}
