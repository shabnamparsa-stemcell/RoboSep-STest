//
// 2011-09-05 to 2011-09-19 sp various changes
//     - add support to read levels from parameter file entries instead of fixed code 
//     - change fixed application path folder to variable obtained during program execution 
//
//----------------------------------------------------------------------------

using System;

using System.Windows.Forms;
using System.Runtime.InteropServices;

// added 2011-09-06-2011-09-19 sp 
// support for parameters from software.ini
using System.Text;
using System.IO;
using System.Globalization;
using System.Collections.Generic;

namespace Tesla.Common
{
    /// <summary>
    /// Summary description for Utilities.
    /// </summary>
    public class Utilities
    {
        public Utilities()
        {
        }

        [DllImport("kernel32.dll")]
        public static extern bool Beep(int freq, int duration);


        // added 2011-09-06 sp 
        // importing methods from system DLLS
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileInt(String Section, String Key, int Default, String FilePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(String Section, String Key, String Default, StringBuilder retVal, int Size, String FilePath);

        // added 2011-09-06 sp 
        // get the installed directory path for RoboSep 
        public static String GetApplicationInstallPath()
        {
            String installDir = "";
            // find the registry key (first on a 32 bit installed system, and if not found try the location setup on a 64 bit system)
            Microsoft.Win32.RegistryKey robosepRegKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\STI");
            if (robosepRegKey == null)
            {
                robosepRegKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432NODE\\STI");
            }
            if (robosepRegKey != null)
            {
                try
                {
                    installDir = robosepRegKey.GetValue("ROBOSEPPATH").ToString();
                }
                catch (Exception)
                {
                    installDir = "Install_Path_Not_Found";
                }
            }
            return (installDir);
            //*/
        }

        // added 2011-09-06 sp 
        // get the current working directory (less the bin directory) 
        public static String GetApplicationParentPath()
        {
            String workingDir = Directory.GetCurrentDirectory();
            int lastIndex = workingDir.LastIndexOfAny(("bin").ToCharArray());
            if (lastIndex >= 2)
                workingDir = workingDir.Substring(0, lastIndex - 2);
            return (workingDir);
        }

        // added 2011-09-19 sp 
        // get the default working application path using either the installed directory or the program execution path
        public static String GetDefaultAppPath()
        {
            // return (GetApplicationInstallPath());
            return (GetApplicationParentPath());
        }

        // added 2011-09-06 sp 
        // Gets the file location for the software INI file specification
        public static String GetSoftwareIniFile()
        {
            String sw_INI_File = GetDefaultAppPath() + ("config\\software.ini");

            if (!File.Exists(sw_INI_File))
            {
                throw new MissingConfigFileException("Missing configuration file, please contact STEMCELL Tech support for the updated configuration file.");
            }

            return (sw_INI_File);
        }


        // added 2011-09-06 sp 
        // Gets an integer value from the software INI file referenced by Key and Section
        public static int GetSoftwareConfigInt(String Section, String Key, int Default)
        {
            int value = Default;
            if (IsKeyInSoftwareConfig(Section, Key))
            {
                value = GetPrivateProfileInt(Section, Key, Default, GetSoftwareIniFile());
            }
            return (value);
        }


        // added 2011-09-06 sp 
        // Gets a String value from the software file referenced by Key and Section
        public static String GetSoftwareConfigString(String Section, String Key, String Default)
        {
            int numChars = 256;
            StringBuilder buffer = new StringBuilder(numChars);
            if (IsKeyInSoftwareConfig(Section, Key))
            {
                GetPrivateProfileString(Section, Key, Default, buffer, numChars, GetSoftwareIniFile());
            }
            return (buffer.ToString());
        }


        // added 2011-09-06 sp 
        // Gets a double precision numeric value from the software INI file referenced by Key and Section
        public static Double GetSoftwareConfigDouble(String Section, String Key, Double Default)
        {
            Double value;
            int numChars = 256;
            StringBuilder buffer = new StringBuilder(numChars);
            if (IsKeyInSoftwareConfig(Section, Key))
            {
                GetPrivateProfileString(Section, Key, Default.ToString(), buffer, numChars, GetSoftwareIniFile());
            }

            NumberStyles style = NumberStyles.Number;
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            if (!Double.TryParse(buffer.ToString(), style, culture, out value))
                value = Default;
            return (value);
        }

        [DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileStringW", SetLastError = true, 
            CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern int GetPrivateProfileString2(string lpAppName, string lpKeyName, string lpDefault,
                                                        string lpReturnString, int nSize, string lpFilename);
        public static List<string> GetSoftwareConfigAllKeys(String Section)
        {
            int numChars = 32000;
            //StringBuilder buffer = new StringBuilder(numChars);    
            String buffer = new String(' ', numChars);
            int count = GetPrivateProfileString2(Section, null, "", buffer, numChars, GetSoftwareIniFile());
            List<string> stuff = new List<string>(buffer.ToString().Split('\0'));
            stuff.RemoveRange(stuff.Count - 2, 2);
            count = stuff.RemoveAll(item => item.Trim().StartsWith("#"));
            return stuff;
        }


        private static List<string> errList = new List<string>();


        public static bool ContainsInStringListIgnoreCase(List<string> list, string key)
        {
            string value = key.Trim();
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            foreach (string s in list)
            {
                if (comparer.Equals(s, value))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsKeyInSoftwareConfig(String Section, String Key)
        {
             
            //bool result = (GetSoftwareConfigAllKeys(Section).IndexOf(Key)>=0);
            bool result = ContainsInStringListIgnoreCase(GetSoftwareConfigAllKeys(Section),Key);


            if (!result)
            {
                //throw new MissingConfigVariableException("Missing configuration variable\n\tSECTION: " + Section + "\n\t KEY: " + Key + "\n\nPlease contact STEMCELL Tech support for the updated configuration file.");
                string msg = "Missing configuration variable\n\tSECTION: " + Section + "\n\t KEY: " + Key + "\n\nPlease contact STEMCELL Tech support for the updated configuration file.";


                if (!errList.Contains(msg))
                {
                    errList.Add(msg);
                    MessageBox.Show(msg, "Configuration Variable Missing Error");
                }
            
            }
            return result;
        }

    }


    public class MissingConfigFileException : Exception
    {
        public MissingConfigFileException(string msg): base(msg){}
    }
    public class MissingConfigVariableException : Exception
    {
        public MissingConfigVariableException(string msg) : base(msg) { }
    }
}
