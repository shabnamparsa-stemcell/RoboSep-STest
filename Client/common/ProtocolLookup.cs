
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tesla.Common
{
    public class ProtocolLookup
    {
        private static ProtocolLookup instance = null; // singleton instance variable

        private const string PROTOCOL_LOOKUP_CSV_FILENAME = "protocolLookup.csv";

        private Dictionary<string, string> lookupCache = null;

        public static ProtocolLookup GetInstance()
        {
            if (instance == null)
            {
                try
                {
                    instance = new ProtocolLookup();
                }
                catch
                {
                    instance = null;
                }
            }
            return instance;
        }

        private ProtocolLookup()
        {
            readLookupData();
        }

        private string getProtocolLookupFilePath()
        {
            return Utilities.GetRoboSepSysPath() + "Config\\" + PROTOCOL_LOOKUP_CSV_FILENAME;
        }

        private void readLookupData()
        {
            lookupCache = new Dictionary<string, string>();
            if (System.IO.File.Exists(getProtocolLookupFilePath()))
            {
                string line;
                System.IO.StreamReader file = new System.IO.StreamReader(getProtocolLookupFilePath());
                while((line = file.ReadLine()) != null)
                {
                    string[] combination = line.Split(',');
                    lookupCache.Add(combination[0], combination[1]);
                }
                file.Close();
            }
        }

        // return 0 if keyVial doesn't exist
        public int GetKeyVial(string protocolName)
        {
            string fiveDigitCode = getFiveDigitCodeFromProtocol(protocolName);

            if (lookupCache == null || fiveDigitCode == null || !lookupCache.ContainsKey(fiveDigitCode))
                return 0;

            string keyVial = lookupCache[fiveDigitCode];

            int result;
            if (keyVial != null && int.TryParse(keyVial, out result))
                return result;
            return 0;
        }

        private string getFiveDigitCodeFromProtocol(string protocolName)
        {
            string result = "";
            bool found = false;
            int charCount = 0;
            if (protocolName != null && protocolName.Length > 0)
            {
                for (int idx = 0; idx < protocolName.Length; idx++)
                {
                    if (Char.IsDigit(protocolName[idx]))
                    {
                        charCount++;
                        result += protocolName[idx];
                        if (charCount == 5)
                        {
                            found = true;
                            break;
                        }
                    }
                    else
                    {
                        charCount = 0;
                        result = "";
                    }
                }
            }
            if (found)
                return result;
            return null;
        }

    }
}
