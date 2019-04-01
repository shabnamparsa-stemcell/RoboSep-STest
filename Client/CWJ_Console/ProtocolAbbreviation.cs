using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.IO;

namespace GUI_Console
{
    class ProtocolAbbreviation
    {
        private const string ABBREVIATION_DELIMITER = "^";
        private const string ABBREVIATION_TOKEN_SEPARATOR = "|^";
        private const string TEXT_SEPARATOR = "separator";
        private const string TEXT_DELIMITER = "delimiter";

        private string tokenDelimiter;
        private string tokenSeparator;
        public Dictionary<string, string> dictAbbreviations;

        public ProtocolAbbreviation()
        {
            tokenDelimiter = ABBREVIATION_DELIMITER;
            tokenSeparator = ABBREVIATION_TOKEN_SEPARATOR;
            dictAbbreviations = new Dictionary<string, string>();
        }

        public bool LoadAbbreviations(string fullPathAbbrevFileName)
        {
            // Check the existance of the "protocols abbrevaiation file"
            if (string.IsNullOrEmpty(fullPathAbbrevFileName))
            {
                return false;
            }

            if (!File.Exists(fullPathAbbrevFileName))
            {
                return false;
            }

            StreamReader sr = new StreamReader(fullPathAbbrevFileName);
            string Line = sr.ReadLine();
            if (Line != null)
            {
                Line.Trim();
                Line.ToLower();
            }

            bool bFoundDelimiter = false;
            bool bFoundSeparator = false;
            while ((Line != null) && (Line != " "))
            {
                // Look for separator 
                if (!bFoundSeparator && Line.Contains(TEXT_SEPARATOR))
                {
                    int index1 = Line.IndexOf(TEXT_SEPARATOR);
                    if (0 <= index1)
                    {
                        int index2 = Line.LastIndexOf('=');

                        if ((index2 - index1) == TEXT_SEPARATOR.Length)
                        {
                            tokenSeparator = Line.Substring(index2 + 1, 1);
                            bFoundSeparator = true;
                        }
                    }
                }

                // Look for delimiter 
                if (!bFoundDelimiter && Line.Contains(TEXT_DELIMITER))
                {
                    int index1 = Line.IndexOf(TEXT_DELIMITER);
                    if (0 <= index1)
                    {
                        int index2 = Line.LastIndexOf('=');

                        if ((index2 - index1) == TEXT_DELIMITER.Length)
                        {
                            tokenDelimiter = Line.Substring(index2 + 1, 1);
                            bFoundDelimiter = true;
                        }
                    }
                }

                if (bFoundDelimiter && bFoundSeparator)
                    break;

                Line = sr.ReadLine();
                if (Line != null)
                {
                    Line.Trim();
                    Line.ToLower();
                }
            }

            sr.Close();

            if (!bFoundDelimiter || !bFoundSeparator)
                return false;

            sr = new StreamReader(fullPathAbbrevFileName);
            Line = sr.ReadLine();
            Line.Trim();
            Line.ToLower();

            while ((Line != null) && (Line != " "))
            {
                if (Line.Contains(tokenSeparator) && Line.Contains(tokenDelimiter))
                {
                    string[] tokens = Line.Split(tokenDelimiter.ToCharArray());
                    string temp;
                    for (int i = 0; i < tokens.Length; i++)
                    {
                        temp = tokens[i];
                        temp.Trim();
                        if (temp.Contains(tokenSeparator))
                        {
                            char[] separator = tokenSeparator.ToCharArray();
                            int index = temp.IndexOf(separator[0]);

                            string key = temp.Substring(0, index);
                            string value = temp.Substring(index + 1);

                            if (!dictAbbreviations.ContainsKey(key))
                            {
                                dictAbbreviations[key] = value;
                            }
                        }
                    }
                }
                Line = sr.ReadLine();
                if (Line != null)
                {
                    Line.Trim();
                }
            }
            sr.Close();
            return true;
        }
    }
}
