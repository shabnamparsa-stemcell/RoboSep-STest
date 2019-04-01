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

using Tesla.Common;
using Tesla.Common.ResourceManagement;
using Tesla.DataAccess;
using Invetech.ApplicationLog;
namespace GUI_Console
{
    public class RoboSep_Protocol
    {
        // Initialize Variables
        public string Protocol_FileName;   // stores protocol file path
        public string Protocol_FilePath;
        public string Protocol_Name;       // stores protocols full name
        public string Protocol_Name_Abbv;  // stores protocols full name abbreviation 
        public string Species_Name;        // stores protocol name w/o species name included (ie no Human)
        public string Species_Name_Abbv;   // stores protocols name w/o species name included (ie no Human) abbreviation 
        public string Selection_Name;      // stores protocol name w/o selection name included (ie no positive selection)
        public string Selection_Name_Abbv; // stores protocol name w/o selection name included (ie no positive selection) abbreviation 
        public string SpeciesSelection_Name;   // stores protocol name w/o Species or Selection type
        public string SpeciesSelection_Name_Abbv;   // stores protocol name w/o Species or Selection type abbreviation

        public string classification;
 
        public int numQuadrants;
        
        // protocol filters
        public bool isHuman;
        public bool isMouse;
        public bool isPositive;
        public bool isNegative;
        public bool isWholeBlood;
        public bool[] bfilters;

        public RoboSep_Protocol_VialBarcodes[] vialBarcodes;

        private RoboSep_Protocol(string FilePath, string FileName, string Name, string Species, string Selection, string SpeciesSelection, string Class, int Quadrants,
            bool H, bool M, bool P, bool N, bool W, List<RoboSep_Protocol_VialBarcodes> vialBarcodes)
        {
            Protocol_FilePath = FilePath;
            Protocol_FileName = FileName;
            Protocol_Name = Name;
            Species_Name = Species;
            Selection_Name = Selection;
            SpeciesSelection_Name = SpeciesSelection;
            classification = Class;
            numQuadrants = Quadrants;

            isHuman = H;
            isMouse = M;
            isPositive = P;
            isNegative = N;
            isWholeBlood = W;
            bfilters = new bool[] { H, M, P, N, W };
            if (vialBarcodes != null)
                this.vialBarcodes = vialBarcodes.ToArray();
        }

        private static string RoboSepProtocol_xsdPath;
        
        //YW Added: Methond change from private to public to access the function from other class
        public static string GetProtocolXSD()
        {
            ///* HARDCODE 32 BIT
            if (RoboSepProtocol_xsdPath == null || RoboSepProtocol_xsdPath == string.Empty)
            {
                string sysPath = Utilities.GetRoboSepSysPath();
   
                // set path to xsd
                string[] pathDirs = sysPath.Split('\\');
                RoboSepProtocol_xsdPath = pathDirs[0];
                for (int i = 1; i < pathDirs.Length; i++)
                {
                    if (pathDirs[i] != "bin")
                    {
                        RoboSepProtocol_xsdPath += "\\" + pathDirs[i];
                    }
                    else
                        break;
                }

                // LOG
                string logMSG = "XSD Path : " + RoboSepProtocol_xsdPath;
                // GUI_Controls.uiLog.LOG(RoboSep_UserConsole.getInstance(), "GetProtocolXSD", GUI_Controls.uiLog.LogLevel.GENERAL, logMSG);             
                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
                //System.Diagnostics.Debug.WriteLine(logMSG);
                
                RoboSepProtocol_xsdPath += "\\config\\RoboSepProtocol.xsd";
            }
            if (File.Exists(RoboSepProtocol_xsdPath))
                return RoboSepProtocol_xsdPath;
            else
                return "C:\\Program Files\\STI\\RoboSep\\config\\RoboSepProtocol.xsd";
        }

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
            string Class = string.Empty;
            
            bool Hum = false;
            bool Mou = false;
            bool Pos = false;
            bool Neg = false;
            bool WBl = false;

            List<RoboSep_Protocol_VialBarcodes> vialBarcodes = null;
            
            int Q = -1;

            RoboSepProtocol ProtocolInfo = new RoboSepProtocol();

            if (File.Exists(Pfile.FullName))
            {
                // serializer
                XmlSerializer myXmlSerializer = new XmlSerializer(typeof(RoboSepProtocol));

                // Initialise a file stream for reading
                FileStream fs = new FileStream(Pfile.FullName, FileMode.Open);

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
                    xsc.Add("STI",  GetProtocolXSD());
                    validatingReader.Schemas.Add(xsc);

                    // 'Rehydrate' the object (that is, deserialise data into the object)					
                    ProtocolInfo = (RoboSepProtocol)myXmlSerializer.Deserialize(validatingReader);
                    
                    //myCurrentActionContext = "User1.udb";
                    */

                    XmlReaderSettings settings = new XmlReaderSettings();
                    //settings.ValidationType = ValidationType.Schema;
                    settings.ValidationType = ValidationType.None;
                    settings.Schemas.Add("STI", GetProtocolXSD());
                    XmlReader reader = XmlReader.Create(fs, settings);
                    ProtocolInfo = (RoboSepProtocol)myXmlSerializer.Deserialize(reader);
                }
                catch (Exception ex)
                {
                    string sMsg = String.Format("Robosep Protocol File '{0}' is invalid. Exception={1}", Pfile.FullName, ex.Message);
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
            
            // get number of quadrants
            Q = Convert.ToInt32(ProtocolInfo.constraints.quadrants.number);
            
            // get protocol type to set bools
            string pType = ProtocolInfo.type.ToString();
            switch (pType)
            {
                case "HumanNegative":
                    Hum = true;
                    Mou = false;
                    Pos = false;
                    Neg = true;
                    Class = "HN";
                    break;
                case "HumanPositive":
                    Hum = true;
                    Mou = false;
                    Pos = true;
                    Neg = false;
                    Class = "HP";
                    break;
                case "Positive":
                    Hum = false;
                    Mou = false;
                    Pos = true;
                    Neg = false;
                    Class = "Pos";
                    break;
                case "Negative":
                    Hum = false;
                    Mou = false;
                    Pos = false;
                    Neg = true;
                    Class = "Neg";
                    break;
                case "WholeBloodPositive":
                    Hum = true;
                    Mou = false;
                    Pos = true;
                    Neg = false;
                    WBl = true;
                    Class = "HP WB";
                    break;
                case "WholeBloodNegative":
                    Hum = true;
                    Mou = false;
                    Pos = false;
                    Neg = true;
                    WBl = true;
                    Class = "HN WB";
                    break;
                case "MouseNegative":
                    Hum = false;
                    Mou = true;
                    Pos = false;
                    Neg = true;
                    Class = "MN";
                    break;
                case "MousePositive":
                    Hum = false;
                    Mou = true;
                    Pos = true;
                    Neg = false;
                    Class = "MP";
                    break;
            }

            if (ProtocolInfo.vialBarcodes != null)
            {
                for (int i = 0; i < ProtocolInfo.vialBarcodes.Length; i++)
                {
                    if (ProtocolInfo.vialBarcodes[i].quadrant.Trim() != String.Empty)
                    {
                        RoboSep_Protocol_VialBarcodes barcodeData = new RoboSep_Protocol_VialBarcodes(Convert.ToUInt32(ProtocolInfo.vialBarcodes[i].quadrant), ProtocolInfo.vialBarcodes[i].squareVialBarcode,
                                                                ProtocolInfo.vialBarcodes[i].circleVialBarcode, ProtocolInfo.vialBarcodes[i].triangleVialBarcode);
                        if (vialBarcodes == null)
                            vialBarcodes = new List<RoboSep_Protocol_VialBarcodes>();
                        vialBarcodes.Add(barcodeData);
                    }
                }
            }

                #region setStrings
                // get protocol name (label)
                Name = ProtocolInfo.header.label.Trim();

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

            #endregion

            tempProtocol = new RoboSep_Protocol(filePath, FileName, Name, Species, Selection, SpeciesSelection, Class, Q, Hum, Mou, Pos, Neg, WBl, vialBarcodes);
            return tempProtocol;
        }

        private static int searchForTextInString(string search, string theString)
        {
            // looks for text string within greater string
            for (int i = 0; i < theString.Length; i++)
            {
                if (theString[i] == search[0])
                {
                    //string temp = string.Empty;
                    StringBuilder tempString = new StringBuilder();
                    for (int j = 0; j < search.Length; j++)
                    {
                        if ((i + j) < theString.Length)
                            //temp += theString[i+j];
                            tempString.Append(theString[i + j]);
                    }
                    if (search.ToUpper() == tempString.ToString().ToUpper()) //temp.ToUpper())
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
                StringBuilder tempString = new StringBuilder();
                for (int k = 0; k < S_Index; k++)
                {
                    tempString.Append(theString[k]);
                }
                for (int k = (S_Index + text.Length); k < theString.Length; k++)
                {
                    tempString.Append(theString[k]);
                }
                return tempString.ToString();
            }
            else return theString;
        }

    }

    public class RoboSep_Protocol_VialBarcodes {

        public uint quadrant; 
        public string squareVialBarcode;
        public string circleVialBarcode;
        public string triangleVialBarcode;

        public RoboSep_Protocol_VialBarcodes(uint quadrant, string squareVialBarcode, string circleVialBarcode, string triangleVialBarcode)
        {
            this.quadrant = quadrant;
            this.squareVialBarcode = squareVialBarcode;
            this.circleVialBarcode = circleVialBarcode;
            this.triangleVialBarcode = triangleVialBarcode;
        }
    }
}
