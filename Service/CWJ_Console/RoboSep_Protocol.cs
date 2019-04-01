using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Serialization;

using Tesla.Common.ResourceManagement;
using Tesla.DataAccess;

namespace GUI_Console
{
    public class RoboSep_Protocol
    {

        // Initialize Variables
        public string Protocol_FileName;   // stores protocol file path
        public string Protocol_FilePath;
        public string Protocol_Name;       // stores protocols full name
        public string Species_Name;        // stores protocol name w/o species name included (ie no Human)
        public string Selection_Name;      // stores protocol name w/o selection name included (ie no positive selection)
        public string SpeciesSelection_Name;   // stores protocol name w/o Species or Selection type
        public int numQuadrants;
        
        // protocol filters
        public bool isHuman;
        public bool isMouse;
        public bool isPositive;
        public bool isNegative;
        public bool isWholeBlood;
        public bool[] bfilters;

        private RoboSep_Protocol(string FilePath, string FileName, string Name, string Species, string Selection, string SpeciesSelection, int Quadrants,
            bool H, bool M, bool P, bool N, bool W)
        {
            Protocol_FilePath = FilePath;
            Protocol_FileName = FileName;
            Protocol_Name = Name;
            Species_Name = Species;
            Selection_Name = Selection;
            SpeciesSelection_Name = SpeciesSelection;
            numQuadrants = Quadrants;

            isHuman = H;
            isMouse = M;
            isPositive = P;
            isNegative = N;
            isWholeBlood = W;
            bfilters = new bool[] { H, M, P, N, W };
        }

        // function grabs protocols from //protocols folder in STI directory                
        public static RoboSep_Protocol generateProtocolFromFile(FileInfo Pfile)
        {
            // temp variables
            RoboSep_Protocol tempProtocol;
            string filePath = Pfile.FullName;
            string FileName = Pfile.Name;
            string Name = string.Empty;
            string Species = string.Empty;
            string Selection = string.Empty;
            string SpeciesSelection = string.Empty;
            bool Hum = false;
            bool Mou = false;
            bool Pos = false;
            bool Neg = false;
            bool WBl = false;

            int Q = -1;

            // open file, grab information from file
            if (File.Exists(filePath))
            {
                using (StreamReader sr = new StreamReader(filePath))
                {   
                    // get rid of 1st line
                    sr.ReadLine();
                    string line;
                    // read line with file info
                    if ((line = sr.ReadLine()) != null)
                    {
                        // split up line by " marks
                        string[] fileInfo = line.Split('"');
                        // Check Protocol Type

                        #region setBools
                        string tempProtocolType = "";
                        for (int i = 0; i < fileInfo[5].Length; i++)
                        {
                            tempProtocolType += fileInfo[5][i];
                        }

                        // determine filters active on
                        // protocol based on protocol type
                        switch (tempProtocolType)
                        {
                            case "HumanNegative":
                                Hum = true;
                                Mou = false;
                                Pos = false;
                                Neg = true;
                                break;
                            case "HumanPositive":
                                Hum = true;
                                Mou = false;
                                Pos = true;
                                Neg = false;
                                break;
                            case "Positive":
                                Hum = false;
                                Mou = false;
                                Pos = true;
                                Neg = false;
                                break;
                            case "Negative":
                                Hum = false;
                                Mou = false;
                                Pos = false;
                                Neg = true;
                                break;
                            case "WholeBloodPositive":
                                Hum = true;
                                Mou = false;
                                Pos = true;
                                Neg = false;
                                WBl = true;
                                break;
                            case "WholeBloodNegative":
                                Hum = true;
                                Mou = false;
                                Pos = false;
                                Neg = true;
                                WBl = true;
                                break;
                            case "MouseNegative":
                                Hum = false;
                                Mou = true;
                                Pos = false;
                                Neg = true;
                                break;
                            case "MousePositive":
                                Hum = false;
                                Mou = true;
                                Pos = true;
                                Neg = false;
                                break;
                        }
                        #endregion
                    }
                    if ((line = sr.ReadLine()) != null)
                    {
                        string[] protocolLabel = line.Split('"');
                        Name = protocolLabel[1];
                    }

                    // determine number of quadrants
                    
                    do
                    {
                        if ((line = sr.ReadLine()) != null)
                        {
                            string[] segments = line.Split('<', '"');
                            if (segments[1].StartsWith("quadrants"))
                            {
                                Q = Convert.ToInt32(segments[2]);
                            }
                        }
                    } while (Q == -1);
                }                        

                #region setStrings

                // set up Protocol "name" strings
                //for (int i = 0; i < FileName.Length - 4; i++)
                //{ Name += FileName[i]; }

                // set species string
                if (Hum || Mou)
                {
                    // get rid of first 6 chars (Human or Mouse)
                    Species = removeTextFromString("Human ", Name);
                    Species = removeTextFromString("Mouse ", Species);
                }
                else
                    Species = Name;

                // set selection string
                string pselect = "Positive Selection ";
                string nselect = "Negative Selection ";

                Selection = removeTextFromString(pselect, Name);
                Selection = removeTextFromString(nselect, Selection);

                if (Hum || Mou)
                {
                    // get rid of first 6 chars (Human or Mouse)
                    SpeciesSelection = removeTextFromString("Human ", Selection);
                    SpeciesSelection = removeTextFromString("Mouse ", SpeciesSelection);
                    
                }
                else
                    SpeciesSelection = Selection;

                // get number of quadrants
                #endregion

            }
            tempProtocol = new RoboSep_Protocol(filePath, FileName, Name, Species, Selection, SpeciesSelection, Q, Hum, Mou, Pos, Neg, WBl);
            return tempProtocol;
        }

        private static int searchForTextInString(string search, string theString)
        {
            // looks for text string within greater string
            for (int i = 0; i < theString.Length; i++)
            {
                if (theString[i] == search[0])
                {
                    string temp = string.Empty;
                    for (int j = 0; j < search.Length; j++)
                    {
                        if ((i + j) < theString.Length)
                            temp += theString[i+j];
                    }
                    if (search.ToUpper() == temp.ToUpper())
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        private static string removeTextFromString(string text, string theString)
        {
            // find and remove text from a greater string
            int S_Index = searchForTextInString(text, theString);
            if (S_Index != -1)
            {
                string Temp = string.Empty;
                for (int k = 0; k < S_Index; k++)
                {
                    Temp += theString[k];
                }
                for (int k = (S_Index + text.Length); k < theString.Length; k++)
                {
                    Temp += theString[k];
                }
                return Temp;
            }
            else return theString;
        }
    }
}
