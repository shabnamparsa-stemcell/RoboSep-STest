//----------------------------------------------------------------------------
// ProtocolModel
//
// Invetech Pty Ltd
// Victoria, 3149, Australia
// Phone:   (+61) 3 9211 7700
// Fax:     (+61) 3 9211 7701
//
// The copyright to the computer program(s) herein is the property of 
// Invetech Pty Ltd, Australia.
// The program(s) may be used and/or copied only with the written permission 
// of Invetech Pty Ltd or in accordance with the terms and conditions 
// stipulated in the agreement/contract under which the program(s)
// have been supplied.
// 
// Copyright � 2004. All Rights Reserved.
//
//
//	03/24/06 - Uniform Multi Sample - RL
//  03/29/06 - pause command - RL (look for PauseCommand)
//	09/05/07 - usebuffertip command - bdr
//	02/06/08 - modify SaveAsProtocolFile not to use threading
//
//----------------------------------------------------------------------------
//
// 2011-09-05 to 2011-09-19 sp various changes
//     - add volume level thresholds (recommended and acceptable) using parameter file entries instead of fixed code  
//
//----------------------------------------------------------------------------

#define TRACE

using System;
using System.Windows.Forms;
using System.Configuration;
using System.Threading;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.Collections;
using System.Diagnostics;

using Tesla.Common.Protocol;										  
using Tesla.Common.ProtocolCommand;
using Tesla.Common.Separator;
using Tesla.DataAccess;
using System.Runtime.Serialization.Formatters.Binary;
using Tesla.Common.ResourceManagement;
using System.Collections.Generic;


namespace Tesla.ProtocolEditorModel
{
	/// <summary>
	/// Model (aka business) layer for the Protocol Editor.  Serves to decouple the GUI from 
	/// the data source.
	/// </summary>
	public class ProtocolModel
	{		
		private static ProtocolModel	theProtocolModel;	// Singleton instance variable
		private static string			theSchemaPath;		// Path to RoboSep protocol schema
		private const string			theSchemaFileName = "RoboSepProtocol.xsd";
		private const string			theSchemaNamespace= "STI";
		private string					myProtocolFileName;
        private bool valid = false;
        public const int NUM_CUST_NAMES_PER_QUAD = 10; //8;
        public const int NUM_VIAL_BARCODES_PER_QUAD = 4;


        private delegate RoboSepProtocol convertFunction();
        private convertFunction[,] convertTable = new convertFunction[Enum.GetNames(typeof(ProtocolFormat)).Length, Enum.GetNames(typeof(ProtocolFormat)).Length];


		#region Construction/destruction

		public static ProtocolModel GetInstance()
		{
			if (theProtocolModel == null)
			{
				try
				{
					theProtocolModel = new ProtocolModel();
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex.Message);
					if(null != ex.InnerException)
					{
						Console.WriteLine(ex.InnerException.Message);
						Console.WriteLine(ex.InnerException.GetType().FullName);
						Console.WriteLine(ex.InnerException.Source);
						Console.WriteLine(ex.InnerException.StackTrace);
						Console.WriteLine(ex.InnerException.TargetSite);
					}
					theProtocolModel = null;
				}
			}
			return theProtocolModel;
		}

		private ProtocolModel()
		{			
			if (myProtocol == null)
			{
				ClearModel();

                convertTable[(int)ProtocolFormat.RoboSepS_1_0, (int)ProtocolFormat.RoboSepS_1_1] = convertRS10ToRS11;
                convertTable[(int)ProtocolFormat.RoboSepS_1_0, (int)ProtocolFormat.RoboSepS_1_2] = convertRS10ToRS12;
                convertTable[(int)ProtocolFormat.RoboSepS_1_0, (int)ProtocolFormat.RoboSep16_1_2] = convertRS10ToRS1612;
                convertTable[(int)ProtocolFormat.RoboSepS_1_1, (int)ProtocolFormat.RoboSepS_1_0] = convertRS11ToRS10;
                convertTable[(int)ProtocolFormat.RoboSepS_1_1, (int)ProtocolFormat.RoboSepS_1_2] = convertRS11ToRS12;
                convertTable[(int)ProtocolFormat.RoboSepS_1_1, (int)ProtocolFormat.RoboSep16_1_2] = convertRS11ToRS1612;
                convertTable[(int)ProtocolFormat.RoboSepS_1_2, (int)ProtocolFormat.RoboSepS_1_0] = convertRS12ToRS10;
                convertTable[(int)ProtocolFormat.RoboSepS_1_2, (int)ProtocolFormat.RoboSepS_1_1] = convertRS12ToRS11;
                convertTable[(int)ProtocolFormat.RoboSepS_1_2, (int)ProtocolFormat.RoboSep16_1_2] = convertRS12ToRS1612;
     
			}
		}

		static ProtocolModel()
		{			
			// Set the path location for the RoboSep Protocol schema 		
			try
			{
                // 2011-09-19 sp
                // replace fixed directory path with the installed path
                theSchemaPath = Tesla.Common.Utilities.GetDefaultAppPath() + ConfigurationSettings.AppSettings["ProtocolsSchemaPath"];
                //theSchemaPath = ConfigurationSettings.AppSettings["ProtocolsSchemaPath"];					
			}
			catch
			{
				throw new ApplicationException("Protocol schema path not configured.");
			}			
		}

		#endregion Construction/destruction

		#region Events

		public event ValidationEventHandler ReportValidationException;

		public event EventHandler			ReportDataAvailable;

		public event EventHandler			ReportDataVolumeCheck;
			
		#endregion Events

		#region Data access

		// Declare a RoboSepProtocol to populate (deserialise) from a protocol file.
		private RoboSepProtocol		myProtocol;

		// Track whether the protocol data held by the model is valid
		private bool myIsDataAvailable = false;		

		public void ClearModel()
		{
			myProtocol = new RoboSepProtocol();
			if (myProtocol.header == null)
			{
				myProtocol.header = new header();
			}
			if (myProtocol.customNames == null)
			{
				myProtocol.customNames = new customNames[4];
				for(int i=0;i<4;i++)
					myProtocol.customNames[i] = new customNames();
            }
            if (myProtocol.vialBarcodes == null)
            {
                myProtocol.vialBarcodes = new vialBarcodes[4];
                for (int i = 0; i < 4; i++)
                    myProtocol.vialBarcodes[i] = new vialBarcodes();
            }
			if (myProtocol.constraints == null)
			{
				myProtocol.constraints = new constraints();
			}
			if (myProtocol.constraints.sampleVolume == null)
			{
				InitialiseSampleVolume();
			}
			if (myProtocol.constraints.quadrants == null)
			{
				myProtocol.constraints.quadrants = new quadrants();
			}
			if (myProtocol.constraints.workingVolume == null)
			{
				InitialiseWorkingVolume();
			}
			if (myProtocol.commands == null)
			{
				myProtocol.commands = new commands();
			}
			if (myProtocol.commands.Items == null)
			{
				myProtocol.commands.Items = new CommandType[0];				
			}
			if (myProtocol.header.author == null)
			{
				myProtocol.header.author = new author();
			}
			if (myProtocol.header.date == null)
			{
				myProtocol.header.date = new date();
				myProtocol.header.date.created = DateTime.Now;
				myProtocol.header.date.modifiedSpecified = false;
			}

            AddRobosepTypeIfAbsent();
            if (myProtocol.header.workaround == null)
            {
                myProtocol.header.workaround = new workaround();
                myProtocol.header.workaround.absoluteVolMultiplier = "0";
            }

            if (myProtocol.header.versionInfo == null)
            {
                myProtocol.header.versionInfo = new versionInfo();
                myProtocol.header.versionInfo.protocolRevision = "0";
                //myProtocol.header.versionInfo.protocolFormat = "0";
                ProtocolFormat = ProtocolFormat.RoboSepS_1_2;
                myProtocol.header.versionInfo.protocolEditorVersion = "0";
                myProtocol.header.versionInfo.protocolEditorINIVersion = "0";
            }

			// RL - Uniform Multi Sample - 03/24/06
			if (myProtocol.multipleSamples == null)
			{
				myProtocol.multipleSamples = new multipleSamples();
			}

			// Now update interested parties to display the "blank data"
			if (ReportDataAvailable != null)
			{
				ReportDataAvailable(this, new System.EventArgs());
			}
		}

      	public bool ValidProtocol
		{
			get
			{
				return valid;
			}
			set
			{
				valid = value;
			}
		}
		public ProtocolClass ProtocolClass
		{
			get
			{
				return (ProtocolClass)myProtocol.type;				
			}
			set
			{
				switch(value)
				{
					case Tesla.Common.Protocol.ProtocolClass.HumanPositive:
						myProtocol.type = Tesla.DataAccess.RoboSepProtocolType.HumanPositive;
						break;
					case Tesla.Common.Protocol.ProtocolClass.HumanNegative:
						myProtocol.type = Tesla.DataAccess.RoboSepProtocolType.HumanNegative;
						break;
					case Tesla.Common.Protocol.ProtocolClass.WholeBloodPositive:
						myProtocol.type = Tesla.DataAccess.RoboSepProtocolType.WholeBloodPositive;
						break;
					case Tesla.Common.Protocol.ProtocolClass.WholeBloodNegative:
						myProtocol.type = Tesla.DataAccess.RoboSepProtocolType.WholeBloodNegative;
						break;
					case Tesla.Common.Protocol.ProtocolClass.MousePositive:
						myProtocol.type = Tesla.DataAccess.RoboSepProtocolType.MousePositive;
						break;
					case Tesla.Common.Protocol.ProtocolClass.MouseNegative:
						myProtocol.type = Tesla.DataAccess.RoboSepProtocolType.MouseNegative;
						break;
					case Tesla.Common.Protocol.ProtocolClass.Maintenance:
						myProtocol.type = Tesla.DataAccess.RoboSepProtocolType.Maintenance;
						break;
					case Tesla.Common.Protocol.ProtocolClass.Shutdown:
						myProtocol.type = Tesla.DataAccess.RoboSepProtocolType.Shutdown;
						break;
	    			case Tesla.Common.Protocol.ProtocolClass.Positive:
						myProtocol.type = Tesla.DataAccess.RoboSepProtocolType.Positive;
						break;
    				case Tesla.Common.Protocol.ProtocolClass.Negative:
						myProtocol.type = Tesla.DataAccess.RoboSepProtocolType.Negative;
						break;

					default:
						myProtocol.type = Tesla.DataAccess.RoboSepProtocolType.Undefined;
						break;
				}
			}
		}

		public string ProtocolLabel
		{
			get
			{
				return myProtocol.header.label;
			}
			set
			{
				myProtocol.header.label = value;
			}
		}		

      	public string ProtocolDesc
		{
			get
			{
				return myProtocol.header.description;
			}
			set
			{
				myProtocol.header.description = value;
			}
		}	

		//IAT
		public string ProtocolNumber1
			{
			get
				{
				return myProtocol.header.protocolNum1;
				}
			set
				{
				myProtocol.header.protocolNum1 = value;
				}
			}	

		//IAT
		public string ProtocolNumber2
		{
			get
			{
				return myProtocol.header.protocolNum2;
			}
			set
			{
				
				myProtocol.header.protocolNum2= value;
			}
		}

      	public string ProtocolFile
		{
			get
			{
				return myProtocolFileName;
			}
			set
			{
				myProtocolFileName = value;
			}
		}

		public string ProtocolVersion
		{
			get
			{
				return myProtocol.header.version;
			}
			set
			{
				myProtocol.header.version = value;
                SetVersionInfoProtocolRevision(value);
			}
		}

		public string ProtocolAuthor
		{
			get
			{
				return myProtocol.header.author.name;
			}
			set
			{
				myProtocol.header.author.name = value;
			}
		}

        public ProtocolFormat ProtocolFormat
        {
            get
            {
                return SeparatorResourceManager.GetProtocolFormat();
            }
            set
            {
                SeparatorResourceManager.SetProtocolFormat(value);
                SetVersionInfoProtocolFormat(value);
            }
        }

        public int ProtocolHackAbsoluteVolumeMultipler
        {
            get
            {
                try
                {
                    return int.Parse(myProtocol.header.workaround.absoluteVolMultiplier);
                }
                catch (Exception) 
                {
                    return 0;
                }
            }
            set
            {
                myProtocol.header.workaround.absoluteVolMultiplier = value.ToString();
            }
        }

		public bool ProtocolDescManualFill
		{
			get
			{
				
				if(myProtocol.header.ProtocolDescriptionManualFillSpecified)
					return myProtocol.header.ProtocolDescriptionManualFill;
				else
					return true;
					
			}
			set
			{
				myProtocol.header.ProtocolDescriptionManualFillSpecified =true;
				myProtocol.header.ProtocolDescriptionManualFill = value;
			}
		}

        public customNames GetCustomNames(int idx)
        {
            customNames results = new customNames();
            if (idx < 0 || idx >= 4 || idx >= myProtocol.customNames.Length)
                idx = 0;

            results.bufferBottle = myProtocol.customNames[idx].bufferBottle;
            results.wasteTube = myProtocol.customNames[idx].wasteTube;
            results.lysisBufferTube = myProtocol.customNames[idx].lysisBufferTube;
            results.selectionCocktailVial = myProtocol.customNames[idx].selectionCocktailVial;
            results.magneticParticleVial = myProtocol.customNames[idx].magneticParticleVial;
            results.antibodyCocktailVial = myProtocol.customNames[idx].antibodyCocktailVial;
            results.sampleTube = myProtocol.customNames[idx].sampleTube;
            results.separationTube = myProtocol.customNames[idx].separationTube;

             bool boRobo16 = SeparatorResourceManager.isPlatformRS16();
             if (boRobo16)
             {
                 results.bufferBottle34 = myProtocol.customNames[idx].bufferBottle34;
                 results.bufferBottle56 = myProtocol.customNames[idx].bufferBottle56;
             }
             return results;
        }

		// create custom names array if missing then clear names array
		public void InitCustomNames()
		{
			// for protocol with no  or partial custom names
			if (myProtocol.customNames == null || myProtocol.customNames.Length < 4) 
			{
				myProtocol.customNames = new customNames[4];

				for (int i = 0; i < 4; i++) {
					myProtocol.customNames[i] = new customNames(); 

					myProtocol.customNames[i].bufferBottle = 
					myProtocol.customNames[i].wasteTube = 
					myProtocol.customNames[i].lysisBufferTube =  
					myProtocol.customNames[i].selectionCocktailVial = 
					myProtocol.customNames[i].magneticParticleVial =
					myProtocol.customNames[i].antibodyCocktailVial =
					myProtocol.customNames[i].sampleTube = 
					myProtocol.customNames[i].separationTube =
                    myProtocol.customNames[i].bufferBottle34 =
                    myProtocol.customNames[i].bufferBottle56 = "";
				}
			}
		}
		      	           
 
		// given a full array of custom names update a protocol's names array
        public void ApplyCustomNames(int idx, customNames customVialNames)
		{
			if (idx < 0 || idx >= 4)	idx = 0;

			// initialilze a protocol with no custom names
			if (myProtocol.customNames == null) {
				InitCustomNames();			
			}
			// for protocol with partial set of custom names reuse q1 names
			else if(myProtocol.customNames.Length<4)
			{
                int replace_idx;
                customNames[] replacement = new customNames[4];
                for (int i = 0; i < 4; i++)
                {
                    replacement[i] = new customNames();
                    replace_idx = (i < myProtocol.customNames.Length) ? i : 0;

                    replacement[i].bufferBottle = myProtocol.customNames[replace_idx].bufferBottle;
                    replacement[i].wasteTube = myProtocol.customNames[replace_idx].wasteTube;
                    replacement[i].lysisBufferTube = myProtocol.customNames[replace_idx].lysisBufferTube;
                    replacement[i].selectionCocktailVial = myProtocol.customNames[replace_idx].selectionCocktailVial;
                    replacement[i].magneticParticleVial = myProtocol.customNames[replace_idx].magneticParticleVial;
                    replacement[i].antibodyCocktailVial = myProtocol.customNames[replace_idx].antibodyCocktailVial;
                    replacement[i].sampleTube = myProtocol.customNames[replace_idx].sampleTube;
                    replacement[i].separationTube = myProtocol.customNames[replace_idx].separationTube;
                    replacement[i].bufferBottle34 = myProtocol.customNames[replace_idx].bufferBottle34;
                    replacement[i].bufferBottle56 = myProtocol.customNames[replace_idx].bufferBottle56;
                }
                myProtocol.customNames = replacement;
			}

			// load custom names 
            myProtocol.customNames[idx].bufferBottle = customVialNames.bufferBottle;
            myProtocol.customNames[idx].wasteTube = customVialNames.wasteTube;
            myProtocol.customNames[idx].lysisBufferTube = customVialNames.lysisBufferTube;
            myProtocol.customNames[idx].selectionCocktailVial = customVialNames.selectionCocktailVial;
            myProtocol.customNames[idx].magneticParticleVial = customVialNames.magneticParticleVial;
            myProtocol.customNames[idx].antibodyCocktailVial = customVialNames.antibodyCocktailVial;
            myProtocol.customNames[idx].sampleTube = customVialNames.sampleTube;
            myProtocol.customNames[idx].separationTube = customVialNames.separationTube;
            myProtocol.customNames[idx].bufferBottle34 = customVialNames.bufferBottle34;
            myProtocol.customNames[idx].bufferBottle56 = customVialNames.bufferBottle56;
		}

        public void NormalizeVialBarcodes()
        {
            vialBarcodes[] replacement = new vialBarcodes[4];
            if (myProtocol.vialBarcodes != null)
            {
                foreach (vialBarcodes barcode in myProtocol.vialBarcodes)
                {
                    try
                    {
                        int q = Int32.Parse(barcode.quadrant);
                        if (q >= 0 && q < 4)
                        {
                            replacement[q] = barcode;

                        }
                    }
                    catch (Exception) { }
                }
            }

            for (int i = 0; i < 4; i++)
            {
                if (replacement[i] == null)
                {
                    replacement[i] = new vialBarcodes();
                    replacement[i].quadrant = replacement[i].squareVialBarcode =
                    replacement[i].circleVialBarcode = replacement[i].triangleVialBarcode = "";
                }
            }

            myProtocol.vialBarcodes = replacement;
        }

        public void GetVialBarcodes(int idx, out string[] vialBarcodes)
        {
            if (idx < 0 || idx >= 4 || idx >= myProtocol.vialBarcodes.Length)
                idx = 0;
            vialBarcodes = new string[NUM_VIAL_BARCODES_PER_QUAD];
            vialBarcodes[0] = myProtocol.vialBarcodes[idx].quadrant;
            vialBarcodes[1] = myProtocol.vialBarcodes[idx].squareVialBarcode;
            vialBarcodes[2] = myProtocol.vialBarcodes[idx].circleVialBarcode;
            vialBarcodes[3] = myProtocol.vialBarcodes[idx].triangleVialBarcode;
        }

        public void RemoveVialBarcodes()
        {
            RemoveVialBarcodes(myProtocol);
        }

        public void RemoveVialBarcodes(RoboSepProtocol myCopy)
        {
            myCopy.vialBarcodes = null;
        }

        // create vial barcodes array if missing then clear barcodes array
        public void AddVialBarcodesIfAbsent()
        {
            AddVialBarcodesIfAbsent(myProtocol);
        }
        public void AddVialBarcodesIfAbsent(RoboSepProtocol myCopy)
        {
            // for protocol with no barcodes or partial vial barcodes
            if (myCopy.vialBarcodes == null || myCopy.vialBarcodes.Length < 4)
            {
                myCopy.vialBarcodes = new vialBarcodes[4];

                for (int i = 0; i < 4; i++)
                {
                    myCopy.vialBarcodes[i] = new vialBarcodes();

                    myCopy.vialBarcodes[i].quadrant =
                    myCopy.vialBarcodes[i].squareVialBarcode =
                    myCopy.vialBarcodes[i].circleVialBarcode =
                    myCopy.vialBarcodes[i].triangleVialBarcode = "";
                }
            }
        }

        public void UpdateVialBarcodeQuadrant(bool[] quadrantUsed)
        {
            if (myProtocol.vialBarcodes != null)
            {
                for (int i = 0; i < myProtocol.vialBarcodes.Length; i++)
                {
                    if (myProtocol.vialBarcodes[i] != null && i < quadrantUsed.Length &&
                        quadrantUsed[i])
                    {
                        myProtocol.vialBarcodes[i].quadrant = i.ToString();
                    }
                }
            }
        }

        // given a full array of barcodes update a protocol's barcode array
        public void ApplyVialBarcodes(int idx, string[] vialBarcodes)
        {
            if (idx < 0 || idx >= 4) idx = 0;

            // initialilze a protocol with no barcodes
            if (myProtocol.vialBarcodes == null)
            {
                AddVialBarcodesIfAbsent();
            }
            // for protocol with partial set of barcodes reuse q1 barcodes
            else if (myProtocol.vialBarcodes.Length < 4)
            {
                vialBarcodes[] replacement = new vialBarcodes[4];
                for (int i = 0; i < 4; i++)
                {
                    replacement[i] = new vialBarcodes();
                    if (i < myProtocol.vialBarcodes.Length)
                    {
                        replacement[i].quadrant = myProtocol.vialBarcodes[i].quadrant;
                        replacement[i].squareVialBarcode = myProtocol.vialBarcodes[i].squareVialBarcode;
                        replacement[i].circleVialBarcode = myProtocol.vialBarcodes[i].circleVialBarcode;
                        replacement[i].triangleVialBarcode = myProtocol.vialBarcodes[i].triangleVialBarcode;
                    }
                    else
                    {
                        replacement[i].quadrant =
                        replacement[i].squareVialBarcode =
                        replacement[i].circleVialBarcode =
                        replacement[i].triangleVialBarcode = "";
                    }
                }
                myProtocol.vialBarcodes = replacement;
            }

            if (vialBarcodes[0]!="" || vialBarcodes[1] != "" || vialBarcodes[2] != "" || vialBarcodes[3] != "")
            {
                myProtocol.vialBarcodes[idx].quadrant = idx.ToString();// vialBarcodes[0];
            }
            else
            {
                myProtocol.vialBarcodes[idx].quadrant = "";
            }
            myProtocol.vialBarcodes[idx].squareVialBarcode = vialBarcodes[1];
            myProtocol.vialBarcodes[idx].circleVialBarcode = vialBarcodes[2];
            myProtocol.vialBarcodes[idx].triangleVialBarcode = vialBarcodes[3];
        }

        //obsolete now... keep in case RS16 still needs it...
        private void AddRobosepTypeIfAbsent()
        {
            if (myProtocol.header.robosep == null)
            {
                myProtocol.header.robosep = new robosep();
            }
        }
        private void AddVersionInfoIfAbsent()
        {
            AddVersionInfoIfAbsent(myProtocol);
        }
        private void AddVersionInfoIfAbsent(RoboSepProtocol myCopy)
        {
            if (myCopy.header.versionInfo == null)
            {
                myCopy.header.versionInfo = new versionInfo();
            }
        }
        private void SetVersionInfoProtocolFormat(ProtocolFormat value)
        {
            AddVersionInfoIfAbsent();
            myProtocol.header.versionInfo.protocolFormat = value.ToString();
            switch (value)
            {
                case ProtocolFormat.RoboSepS_1_0:
                case ProtocolFormat.RoboSepS_1_1:
                case ProtocolFormat.RoboSepS_1_2:
                    myProtocol.header.versionInfo.platform = SeparatorResourceManager.GetSeparatorString(StringId.RoboSepS);;
                    break;
                case ProtocolFormat.RoboSep16:
                case ProtocolFormat.RoboSep16_1_2:
                    myProtocol.header.versionInfo.platform = SeparatorResourceManager.GetSeparatorString(StringId.RoboSep16);;
                    break;
            }
        }
        public void SetVersionInfoProtocolEditorVersion(string value)
        {
            AddVersionInfoIfAbsent();
            myProtocol.header.versionInfo.protocolEditorVersion = value;
        }
        public void SetVersionInfoProtocolEditorIniVersion(string value)
        {
            AddVersionInfoIfAbsent();
            myProtocol.header.versionInfo.protocolEditorINIVersion = value;
        }
        private void SetVersionInfoProtocolRevision(string value)
        {
            AddVersionInfoIfAbsent();
            myProtocol.header.versionInfo.protocolRevision = value;
        }
        private void RemoveVersionInfo(RoboSepProtocol myCopy)
        {
            myCopy.header.versionInfo = null;
        }


		// RL - Uniform Multi Sample - 03/24/06
		public void getMultipleSamples(out bool[] samplesRequired)
		{
			int quads=(int)Tesla.Common.Separator.QuadrantId.NUM_QUADRANTS;
			samplesRequired = new bool[quads];
			samplesRequired[0] = myProtocol.multipleSamples.Q1;
			samplesRequired[1] = myProtocol.multipleSamples.Q2;
			samplesRequired[2] = myProtocol.multipleSamples.Q3;
			samplesRequired[3] = myProtocol.multipleSamples.Q4;
		}

		// RL - Uniform Multi Sample - 03/24/06
		public void setMultipleSamples(bool[] samplesRequired)
		{
			int quads=(int)Tesla.Common.Separator.QuadrantId.NUM_QUADRANTS;
			if(samplesRequired != null && samplesRequired.Length == quads)
			{
				if(myProtocol.multipleSamples == null)
				{
					myProtocol.multipleSamples = new multipleSamples();
				}
				myProtocol.multipleSamples.Q1Specified = true;
				myProtocol.multipleSamples.Q2Specified = true;
				myProtocol.multipleSamples.Q3Specified = true;
				myProtocol.multipleSamples.Q4Specified = true;
				myProtocol.multipleSamples.Q1=samplesRequired[0];
				myProtocol.multipleSamples.Q2=samplesRequired[1];
				myProtocol.multipleSamples.Q3=samplesRequired[2];
				myProtocol.multipleSamples.Q4=samplesRequired[3];
			}
		}

		public void InitResultVialChecks()
		{
			if (myProtocol.resultVialChecks == null || myProtocol.resultVialChecks.Length<4) 
			{
				myProtocol.resultVialChecks = new resultVialChecks[4];
				for(int i=0;i<4;i++)
					myProtocol.resultVialChecks[i] = new resultVialChecks(); 
			}
			
			for(int i=0;i<4;i++)
			{
                InittResultVialChecks(myProtocol.resultVialChecks[i]);
			}
		}


        private static void InittResultVialChecks(resultVialChecks results)
        {
            results.bufferBottle =
            results.bufferBottle34 =
            results.bufferBottle56 =
                results.wasteTube =
                results.lysisBufferTube =
                results.selectionCocktailVial =
                results.magneticParticleVial =
                results.antibodyCocktailVial =
                results.sampleTube =
                results.separationTube = false;
        }

        public resultVialChecks GetResultVialChecks(int quadrantIdx)
        {
            resultVialChecks results = new resultVialChecks();
            InittResultVialChecks(results);

            if (myProtocol == null || myProtocol.resultVialChecks == null)
            {
                return results;
            }

            int idx = quadrantIdx;
            if (idx < 0 || idx >= 4 || idx >= myProtocol.resultVialChecks.Length)
                idx = 0;
            results.bufferBottle = myProtocol.resultVialChecks[idx].bufferBottle;
            results.wasteTube = myProtocol.resultVialChecks[idx].wasteTube;
            results.lysisBufferTube = myProtocol.resultVialChecks[idx].lysisBufferTube;
            results.selectionCocktailVial = myProtocol.resultVialChecks[idx].selectionCocktailVial;
            results.magneticParticleVial = myProtocol.resultVialChecks[idx].magneticParticleVial;
            results.antibodyCocktailVial = myProtocol.resultVialChecks[idx].antibodyCocktailVial;
            results.sampleTube = myProtocol.resultVialChecks[idx].sampleTube;
            results.separationTube = myProtocol.resultVialChecks[idx].separationTube;

            bool boRobo16 = SeparatorResourceManager.isPlatformRS16();
            if (boRobo16)
            {
                results.bufferBottle34 = myProtocol.resultVialChecks[idx].bufferBottle34;
                results.bufferBottle56 = myProtocol.resultVialChecks[idx].bufferBottle56;
            }
            return results;
        }


		public void GetResultVialChecks(int idx, out bool[] resultVialChecks)
		{
			if(idx<0 ||idx>=4||idx>=myProtocol.resultVialChecks.Length)
				idx=0;

            bool boRobo16 = SeparatorResourceManager.isPlatformRS16();
            resultVialChecks = new bool[boRobo16?10:8];
			resultVialChecks[0] = myProtocol.resultVialChecks[idx].bufferBottle;
			resultVialChecks[1] = myProtocol.resultVialChecks[idx].wasteTube;
			resultVialChecks[2] = myProtocol.resultVialChecks[idx].lysisBufferTube;
			resultVialChecks[3] = myProtocol.resultVialChecks[idx].selectionCocktailVial;
			resultVialChecks[4] = myProtocol.resultVialChecks[idx].magneticParticleVial;
			resultVialChecks[5] = myProtocol.resultVialChecks[idx].antibodyCocktailVial;
			resultVialChecks[6] = myProtocol.resultVialChecks[idx].sampleTube;
			resultVialChecks[7] = myProtocol.resultVialChecks[idx].separationTube;
            if (boRobo16)
            {
                resultVialChecks[8] = myProtocol.resultVialChecks[idx].bufferBottle34;
                resultVialChecks[9] = myProtocol.resultVialChecks[idx].bufferBottle56;
            }
		}

        public void UpdateResultVialChecks(int idx, resultVialChecks results)
        {
            if (idx < 0 || idx >= 4)
                idx = 0;
            //protocol with no resultVialChecks
            if (myProtocol.resultVialChecks == null)
            {
                myProtocol.resultVialChecks = new resultVialChecks[4];
                for (int i = 0; i < 4; i++)
                    myProtocol.resultVialChecks[i] = new resultVialChecks();
            }
            myProtocol.resultVialChecks[idx].bufferBottleSpecified = true;
			myProtocol.resultVialChecks[idx].wasteTubeSpecified = true;
			myProtocol.resultVialChecks[idx].lysisBufferTubeSpecified = true;
			myProtocol.resultVialChecks[idx].selectionCocktailVialSpecified = true;
			myProtocol.resultVialChecks[idx].magneticParticleVialSpecified = true;
			myProtocol.resultVialChecks[idx].antibodyCocktailVialSpecified = true;
			myProtocol.resultVialChecks[idx].sampleTubeSpecified = true;
			myProtocol.resultVialChecks[idx].separationTubeSpecified = true;
			myProtocol.resultVialChecks[idx].bufferBottle = results.bufferBottle;
			myProtocol.resultVialChecks[idx].wasteTube = results.wasteTube;
			myProtocol.resultVialChecks[idx].lysisBufferTube =  results.lysisBufferTube;
			myProtocol.resultVialChecks[idx].selectionCocktailVial = results.selectionCocktailVial;
			myProtocol.resultVialChecks[idx].magneticParticleVial = results.magneticParticleVial;
			myProtocol.resultVialChecks[idx].antibodyCocktailVial = results.antibodyCocktailVial;
			myProtocol.resultVialChecks[idx].sampleTube = results.sampleTube;
			myProtocol.resultVialChecks[idx].separationTube = results.separationTube;

            if (SeparatorResourceManager.isPlatformRS16())
            {
                myProtocol.resultVialChecks[idx].bufferBottle34Specified = true;
                myProtocol.resultVialChecks[idx].bufferBottle56Specified = true;
                myProtocol.resultVialChecks[idx].bufferBottle34 = results.bufferBottle34;
                myProtocol.resultVialChecks[idx].bufferBottle56 = results.bufferBottle56;
            }
        }
		public void UpdateResultVialChecks(int idx, bool[] resultVialChecks)
		{
			if(idx<0 ||idx>=4)
				idx=0;
					
			//protocol with no resultVialChecks
			if (myProtocol.resultVialChecks == null) 
			{
				myProtocol.resultVialChecks = new resultVialChecks[4];
				for(int i=0;i<4;i++)
					myProtocol.resultVialChecks[i] = new resultVialChecks(); 
			}
			
			myProtocol.resultVialChecks[idx].bufferBottleSpecified = true;
			myProtocol.resultVialChecks[idx].wasteTubeSpecified = true;
			myProtocol.resultVialChecks[idx].lysisBufferTubeSpecified = true;
			myProtocol.resultVialChecks[idx].selectionCocktailVialSpecified = true;
			myProtocol.resultVialChecks[idx].magneticParticleVialSpecified = true;
			myProtocol.resultVialChecks[idx].antibodyCocktailVialSpecified = true;
			myProtocol.resultVialChecks[idx].sampleTubeSpecified = true;
			myProtocol.resultVialChecks[idx].separationTubeSpecified = true;
			myProtocol.resultVialChecks[idx].bufferBottle = resultVialChecks[0];
			myProtocol.resultVialChecks[idx].wasteTube = resultVialChecks[1];
			myProtocol.resultVialChecks[idx].lysisBufferTube =  resultVialChecks[2];
			myProtocol.resultVialChecks[idx].selectionCocktailVial = resultVialChecks[3];
			myProtocol.resultVialChecks[idx].magneticParticleVial = resultVialChecks[4];
			myProtocol.resultVialChecks[idx].antibodyCocktailVial = resultVialChecks[5];
			myProtocol.resultVialChecks[idx].sampleTube = resultVialChecks[6];
			myProtocol.resultVialChecks[idx].separationTube = resultVialChecks[7];

            if (SeparatorResourceManager.isPlatformRS16())
            {
                if (resultVialChecks.Length >= 10)
                {
                    myProtocol.resultVialChecks[idx].bufferBottle34Specified = true;
                    myProtocol.resultVialChecks[idx].bufferBottle56Specified = true;
                    myProtocol.resultVialChecks[idx].bufferBottle34 = resultVialChecks[8];
                    myProtocol.resultVialChecks[idx].bufferBottle56 = resultVialChecks[9];
                }
            }
		}

		public DateTime ProtocolCreationDate
		{
			get
			{
				return myProtocol.header.date.created;
			}
			set
			{
				myProtocol.header.date.created = value;
			}
		}

		public bool ProtocolModificationDateIsSpecified
		{
			get
			{
				return myProtocol.header.date.modifiedSpecified;
			}
		}

		public DateTime ProtocolModificationDate
		{
			get
			{
				return myProtocol.header.date.modified;
			}
			set
			{
				myProtocol.header.date.modified = value;
				myProtocol.header.date.modifiedSpecified = true;
			}
		}

        private void InitialiseSampleVolume()
        {
            if (myProtocol.constraints.sampleVolume == null)
            {
                myProtocol.constraints.sampleVolume = new sampleVolume();
                myProtocol.constraints.sampleVolume.min_uL = string.Empty;
                myProtocol.constraints.sampleVolume.max_uL = string.Empty;
            }
        }

		public double SampleVolumeMinimum
		{
			get
			{
				int min_uL = 1;		//IAT - changed from 0 to 1
				try
				{
					min_uL = int.Parse(myProtocol.constraints.sampleVolume.min_uL);
				}
				catch
				{
					min_uL = 1;
				}
				return min_uL;
			}
			set
			{				
                if (myProtocol.constraints.sampleVolume == null)
                {
                    InitialiseSampleVolume();
                }
				myProtocol.constraints.sampleVolume.min_uL = value.ToString();
			}
		}

		public double SampleVolumeMaximum
		{
			get
			{
				int max_uL = 1;		 //IAT- - changed from 0 to 1
				try
				{
					max_uL = int.Parse(myProtocol.constraints.sampleVolume.max_uL);
				}
				catch
				{
					max_uL = 1;
				}
				return max_uL;
			}
			set
			{	
                if (myProtocol.constraints.sampleVolume == null)
                {
                    InitialiseSampleVolume();
                }
				myProtocol.constraints.sampleVolume.max_uL = value.ToString();
			}
		}

		public int QuadrantCount
		{
			set
			{				
				myProtocol.constraints.quadrants.number = value.ToString();
			}
		}

        private void InitialiseWorkingVolume()
        {
            if (myProtocol.constraints.workingVolume == null)
            {
                myProtocol.constraints.workingVolume = new workingVolume();
                myProtocol.constraints.workingVolume.sampleThreshold_uL = string.Empty;
                myProtocol.constraints.workingVolume.lowVolume_uL = string.Empty;
                myProtocol.constraints.workingVolume.highVolume_uL = string.Empty;
            }
        }

		public int WorkingVolumeSampleThreshold
		{
			get
			{
				int sampleThreshold_uL = 1;	//IAT - changed from 0 to 1
				try
				{
					sampleThreshold_uL = 
						int.Parse(myProtocol.constraints.workingVolume.sampleThreshold_uL);
				}
				catch
				{
					sampleThreshold_uL = 1;
				}
				return sampleThreshold_uL;
			}
			set
			{
                if (myProtocol.constraints.workingVolume == null)
                {
                    InitialiseWorkingVolume();
                }
				myProtocol.constraints.workingVolume.sampleThreshold_uL = value.ToString();
			}
		}

		public int WorkingVolumeLowVolume
		{
			get
			{
				int lowVolume_uL = 1;		//IAT - changed from 0 to 1
				try
				{
					lowVolume_uL = 
						int.Parse(myProtocol.constraints.workingVolume.lowVolume_uL);
				}
				catch
				{
					lowVolume_uL = 1;
				}
				return lowVolume_uL;
			}
			set
			{
                if (myProtocol.constraints.workingVolume == null)
                {
                    InitialiseWorkingVolume();
                }
				myProtocol.constraints.workingVolume.lowVolume_uL = value.ToString();
			}
		}

		public int WorkingVolumeHighVolume
		{
			get
			{
				int highVolume_uL = 1;		//IAT - changed from 0 to 1
				try
				{
					highVolume_uL = 
						int.Parse(myProtocol.constraints.workingVolume.highVolume_uL);
				}
				catch
				{
					highVolume_uL = 1;		//IAT - changed from 0 to 1
				} 
				return highVolume_uL;
			}
			set
			{
                if (myProtocol.constraints.workingVolume == null)
                {
                    InitialiseWorkingVolume();
                }
				myProtocol.constraints.workingVolume.highVolume_uL = value.ToString();
			}
		}

		public void GetCommandSequence(out IProtocolCommand[] commandInfo)
		{
			int commandCount = myProtocol.commands.Items.GetLength(0);
			commandInfo = new IProtocolCommand[commandCount];            
			for (int i = 0; i < commandCount; ++i) 
			{
				// Note: we read the 'seq' numbers here, but ignore whether they
				// are actually in sequence or not, as we do our own sort later on the list of
				// commands.
				CommandType currentCommand = myProtocol.commands.Items[i];

				// Allocate a 'ProtocolCommand' instance and fill out the base type members
				SubCommand protocolCommandSubtype = SubCommand.HomeAllCommand;
				commandInfo[i] = GetProtocolCommandSubTypeInstance(currentCommand,
					out protocolCommandSubtype);				
				commandInfo[i].CommandSequenceNumber = uint.Parse(currentCommand.seq);
				commandInfo[i].CommandLabel = currentCommand.label;
				commandInfo[i].CommandExtensionTime = uint.Parse(currentCommand.extensionTime);
                // added 2011-09-06 sp
                // provide support for volume status checking
                commandInfo[i].CommandCheckStatus = VolumeCheck.VOLUMES_UNCHECKED;
				
				// Fill out the subtype members
				switch (protocolCommandSubtype)
				{
					case SubCommand.HomeAllCommand:
						// No additional fields
						break;
					case SubCommand.DemoCommand:
						ProtocolDemoCommand demoCommand =
							(ProtocolDemoCommand)commandInfo[i];
						demoCommand.IterationCount = 
							uint.Parse(((DemoCommand)currentCommand).iterations);
						break;
					case SubCommand.PumpLifeCommand:
						ProtocolPumpLifeCommand pumpLifeCommand =
							(ProtocolPumpLifeCommand)commandInfo[i];
						pumpLifeCommand.IterationCount = 
							uint.Parse(((PumpLifeCommand)currentCommand).iterations);
						break;
					case SubCommand.IncubateCommand:
						ProtocolIncubateCommand incubateCommand = 
							(ProtocolIncubateCommand)commandInfo[i];
						incubateCommand.WaitCommandTimeDuration = 
							uint.Parse(((IncubateCommand)currentCommand).processingTime.duration);
						break;
					case SubCommand.SeparateCommand:
						ProtocolSeparateCommand separateCommand =
							(ProtocolSeparateCommand)commandInfo[i];
						separateCommand.WaitCommandTimeDuration =
							uint.Parse(((SeparateCommand)currentCommand).processingTime.duration);
						break;

					case SubCommand.TransportCommand:  // bdr
						ProtocolTransportCommand transportCommand =
							(ProtocolTransportCommand)commandInfo[i];
						transportCommand.SourceVial = (AbsoluteResourceLocation)
							Enum.Parse(typeof(AbsoluteResourceLocation),
							(((TransportCommand)currentCommand).vials.src));
						transportCommand.DestinationVial = (AbsoluteResourceLocation)
							Enum.Parse(typeof(AbsoluteResourceLocation),
							(((TransportCommand)currentCommand).vials.dest));
						// Retrieve absolute or relative volume
						if (((TransportCommand)currentCommand).Item 
							as Tesla.DataAccess.absoluteVolume != null) // MCHERE - STUPID CODE?
						{
							// Absolute volume specified
							transportCommand.VolumeTypeSpecifier = VolumeType.Absolute;
							DataAccess.absoluteVolume absVol = (Tesla.DataAccess.absoluteVolume)((TransportCommand)currentCommand).Item;
							transportCommand.AbsoluteVolume_uL = int.Parse(absVol.value_uL);							
						}
						else if (((TransportCommand)currentCommand).Item 
							as Tesla.DataAccess.relativeVolume != null)
						{
							// Relative volume specified
							transportCommand.VolumeTypeSpecifier = VolumeType.Relative;
							DataAccess.relativeVolume relVol = (Tesla.DataAccess.relativeVolume)((TransportCommand)currentCommand).Item;
							transportCommand.RelativeVolumeProportion = (double)relVol.proportion;
						}
						// Free Air Dispense, use buffer tip - bdr
						transportCommand.UseBufferTip = ((TransportCommand)currentCommand).useBufferTip;
						transportCommand.FreeAirDispense = ((TransportCommand)currentCommand).freeAirDispense;
						transportCommand.TipRack = ((TransportCommand)currentCommand).tipRack;
						transportCommand.TipRackSpecified = ((TransportCommand)currentCommand).tipRackSpecified;
						break;
					
					case SubCommand.MixCommand:
						ProtocolMixCommand protocolMixCommand =
							(ProtocolMixCommand)commandInfo[i];
						MixCommand currentMixCommand = (MixCommand)currentCommand;
						protocolMixCommand.SourceVial = (AbsoluteResourceLocation)
							Enum.Parse(typeof(AbsoluteResourceLocation),
							currentMixCommand.vials.src);
						protocolMixCommand.DestinationVial = (AbsoluteResourceLocation)
							Enum.Parse(typeof(AbsoluteResourceLocation),
							currentMixCommand.vials.dest);

						protocolMixCommand.MixCycles=3;
						protocolMixCommand.TipTubeBottomGap_uL=0;

						if(currentMixCommand.mixCycles!=null)
							protocolMixCommand.MixCycles=int.Parse(currentMixCommand.mixCycles);

						protocolMixCommand.VolumeTypeSpecifier = VolumeType.NotSpecified;
						// Retrieve absolute or relative volume
						if (currentMixCommand.Item as Tesla.DataAccess.absoluteVolume != null)
						{
							// Absolute volume specified
							protocolMixCommand.VolumeTypeSpecifier = VolumeType.Absolute;
							DataAccess.absoluteVolume absVol = (Tesla.DataAccess.absoluteVolume)currentMixCommand.Item;
							protocolMixCommand.AbsoluteVolume_uL = int.Parse(absVol.value_uL);	
							
							if(currentMixCommand.tipTubeBottomGap!=null)
								protocolMixCommand.TipTubeBottomGap_uL=int.Parse(currentMixCommand.tipTubeBottomGap);
						}
						else if (currentMixCommand.Item as Tesla.DataAccess.relativeVolume != null)
						{
							// Relative volume specified
							protocolMixCommand.VolumeTypeSpecifier = VolumeType.Relative;
							DataAccess.relativeVolume relVol = (Tesla.DataAccess.relativeVolume)currentMixCommand.Item;
							protocolMixCommand.RelativeVolumeProportion = (double)relVol.proportion;
						}
						else
						{
							// relative volume specified
							protocolMixCommand.VolumeTypeSpecifier = VolumeType.Relative;
							protocolMixCommand.RelativeVolumeProportion = (double)(0.5);	
						}

      					protocolMixCommand.TipRack = ((MixCommand)currentCommand).tipRack;
      					protocolMixCommand.TipRackSpecified = ((MixCommand)currentCommand).tipRackSpecified;
						break;

					case SubCommand.TopUpVialCommand:
						ProtocolTopUpVialCommand protocolTopUpVialCommand =
							(ProtocolTopUpVialCommand)commandInfo[i];
						TopUpVialCommand topUpVialCommand = (TopUpVialCommand)currentCommand;
						protocolTopUpVialCommand.SourceVial = (AbsoluteResourceLocation)
							Enum.Parse(typeof(AbsoluteResourceLocation),
							(((TopUpVialCommand)currentCommand).vials.src));
						protocolTopUpVialCommand.DestinationVial = (AbsoluteResourceLocation)
							Enum.Parse(typeof(AbsoluteResourceLocation),
							(((TopUpVialCommand)currentCommand).vials.dest));
						// Retrieve absolute or relative volume
						if (topUpVialCommand.Item as Tesla.DataAccess.absoluteVolume != null)
						{
							// Absolute volume specified
							protocolTopUpVialCommand.VolumeTypeSpecifier = VolumeType.Absolute;
							DataAccess.absoluteVolume absVol = (Tesla.DataAccess.absoluteVolume)topUpVialCommand.Item;
							protocolTopUpVialCommand.AbsoluteVolume_uL = int.Parse(absVol.value_uL);							
						}
						else if (topUpVialCommand.Item as DataAccess.relativeVolume != null)
						{
							// Relative volume specified
							protocolTopUpVialCommand.VolumeTypeSpecifier = VolumeType.Relative;
							DataAccess.relativeVolume relVol = (Tesla.DataAccess.relativeVolume)topUpVialCommand.Item;
							protocolTopUpVialCommand.RelativeVolumeProportion = (double)relVol.proportion;
						}
						else
						{
							// Absolute volume specified
							protocolTopUpVialCommand.VolumeTypeSpecifier = VolumeType.Absolute;
							protocolTopUpVialCommand.AbsoluteVolume_uL = 0;				
						}
						// Free Air Dispense
						protocolTopUpVialCommand.FreeAirDispense = topUpVialCommand.freeAirDispense;
      					protocolTopUpVialCommand.TipRack = topUpVialCommand.tipRack;
      					protocolTopUpVialCommand.TipRackSpecified = topUpVialCommand.tipRackSpecified;
						break;

					case SubCommand.ResuspendVialCommand:
						ProtocolResuspendVialCommand protocolResuspendVialCommand =
							(ProtocolResuspendVialCommand)commandInfo[i];
						ResuspendVialCommand resuspendVialCommand = (ResuspendVialCommand)currentCommand;
						protocolResuspendVialCommand.SourceVial = (AbsoluteResourceLocation)
							Enum.Parse(typeof(AbsoluteResourceLocation),
							(((ResuspendVialCommand)currentCommand).vials.src));
						protocolResuspendVialCommand.DestinationVial = (AbsoluteResourceLocation)
							Enum.Parse(typeof(AbsoluteResourceLocation),
							(((ResuspendVialCommand)currentCommand).vials.dest));
						// Retrieve absolute or relative volume
						if (resuspendVialCommand.Item as Tesla.DataAccess.absoluteVolume != null)
						{
							// Absolute volume specified
							protocolResuspendVialCommand.VolumeTypeSpecifier = VolumeType.Absolute;
							DataAccess.absoluteVolume absVol = (Tesla.DataAccess.absoluteVolume)resuspendVialCommand.Item;
							protocolResuspendVialCommand.AbsoluteVolume_uL = int.Parse(absVol.value_uL);							
						}
						else if (resuspendVialCommand.Item as DataAccess.relativeVolume != null)
						{
							// Relative volume specified
							protocolResuspendVialCommand.VolumeTypeSpecifier = VolumeType.Relative;
							DataAccess.relativeVolume relVol = (Tesla.DataAccess.relativeVolume)resuspendVialCommand.Item;
							protocolResuspendVialCommand.RelativeVolumeProportion = (double)relVol.proportion;
						}
						else
						{
							// Absolute volume specified
							protocolResuspendVialCommand.VolumeTypeSpecifier = VolumeType.Absolute;
							protocolResuspendVialCommand.AbsoluteVolume_uL = 0;				
						}
						// Free Air Dispense
						protocolResuspendVialCommand.FreeAirDispense = resuspendVialCommand.freeAirDispense;
      					protocolResuspendVialCommand.TipRack = resuspendVialCommand.tipRack;
      					protocolResuspendVialCommand.TipRackSpecified = resuspendVialCommand.tipRackSpecified;
						break;

					case SubCommand.FlushCommand:
						ProtocolFlushCommand protocolFlushCommand =
							(ProtocolFlushCommand)commandInfo[i];
						FlushCommand flushCommand = (FlushCommand)currentCommand;
						protocolFlushCommand.SourceVial = (AbsoluteResourceLocation)
							Enum.Parse(typeof(AbsoluteResourceLocation),
							(flushCommand.vials.src));
						protocolFlushCommand.DestinationVial = (AbsoluteResourceLocation)
							Enum.Parse(typeof(AbsoluteResourceLocation),
							(flushCommand.vials.dest));
						if (flushCommand.flags != null)
						{
							protocolFlushCommand.HomeFlag = flushCommand.flags.home;
						}
      					protocolFlushCommand.TipRack = flushCommand.tipRack;
      					protocolFlushCommand.TipRackSpecified = flushCommand.tipRackSpecified;
						break;

					case SubCommand.PrimeCommand:
						ProtocolPrimeCommand protocolPrimeCommand =
							(ProtocolPrimeCommand)commandInfo[i];
						PrimeCommand primeCommand = (PrimeCommand)currentCommand;
						protocolPrimeCommand.SourceVial = (AbsoluteResourceLocation)
							Enum.Parse(typeof(AbsoluteResourceLocation),
							(primeCommand.vials.src));
						protocolPrimeCommand.DestinationVial = (AbsoluteResourceLocation)
							Enum.Parse(typeof(AbsoluteResourceLocation),
							(primeCommand.vials.dest));
						if (primeCommand.flags != null)
						{
							protocolPrimeCommand.HomeFlag = primeCommand.flags.home;
						}
      					protocolPrimeCommand.TipRack = primeCommand.tipRack;
      					protocolPrimeCommand.TipRackSpecified = primeCommand.tipRackSpecified;
						break;

                    case SubCommand.PauseCommand:
                        ProtocolPauseCommand pauseCommand =
                            (ProtocolPauseCommand)commandInfo[i];
                        pauseCommand.WaitCommandTimeDuration =
                            uint.Parse(((PauseCommand)currentCommand).processingTime.duration);
                        break;
                    case SubCommand.TopUpMixTransSepTransCommand:
                        {
                            ProtocolTopUpMixTransSepTransCommand cmd =
                                (ProtocolTopUpMixTransSepTransCommand)commandInfo[i];

                            TopUpMixTransSepTransCommand dataAccessCmd = (TopUpMixTransSepTransCommand)currentCommand;
                            cmd.SourceVial = (AbsoluteResourceLocation)Enum.Parse(typeof(AbsoluteResourceLocation), (dataAccessCmd.vials.src));
                            cmd.DestinationVial = (AbsoluteResourceLocation)Enum.Parse(typeof(AbsoluteResourceLocation), (dataAccessCmd.vials.dest));
                            cmd.SourceVial2 = (AbsoluteResourceLocation)Enum.Parse(typeof(AbsoluteResourceLocation), (dataAccessCmd.vials2.src));
                            cmd.DestinationVial2 = (AbsoluteResourceLocation)Enum.Parse(typeof(AbsoluteResourceLocation), (dataAccessCmd.vials2.dest));
                            cmd.SourceVial3 = (AbsoluteResourceLocation)Enum.Parse(typeof(AbsoluteResourceLocation), (dataAccessCmd.vials3.src));
                            cmd.DestinationVial3 = (AbsoluteResourceLocation)Enum.Parse(typeof(AbsoluteResourceLocation), (dataAccessCmd.vials3.dest));


                            cmd.SetVolumeCommandTypeParam(dataAccessCmd.Item);
                            cmd.SetVolumeCommandTypeParam2(dataAccessCmd.Item1);
                            cmd.SetVolumeCommandTypeParam3(dataAccessCmd.Item2);
                            cmd.SetVolumeCommandTypeParam4(dataAccessCmd.Item3);

                            cmd.WaitCommandTimeDuration = uint.Parse(dataAccessCmd.duration); ;
                            cmd.TipRack = dataAccessCmd.tipRack;
                            cmd.TipRackSpecified = dataAccessCmd.tipRackSpecified;

                            cmd.MixCycles = 3;
                            cmd.TipTubeBottomGap_uL = 0;

                            if (dataAccessCmd.mixCycles != null)
                                cmd.MixCycles = int.Parse(dataAccessCmd.mixCycles);
                            if (dataAccessCmd.tipTubeBottomGap != null)
                                cmd.TipTubeBottomGap_uL = int.Parse(dataAccessCmd.tipTubeBottomGap);
                        }

                        break;
                    case SubCommand.TopUpMixTransCommand:
                        {
                            ProtocolTopUpMixTransCommand cmd =
                                (ProtocolTopUpMixTransCommand)commandInfo[i];

                            TopUpMixTransCommand dataAccessCmd = (TopUpMixTransCommand)currentCommand;
                            cmd.SourceVial = (AbsoluteResourceLocation)Enum.Parse(typeof(AbsoluteResourceLocation), (dataAccessCmd.vials.src));
                            cmd.DestinationVial = (AbsoluteResourceLocation)Enum.Parse(typeof(AbsoluteResourceLocation), (dataAccessCmd.vials.dest));
                            cmd.SourceVial2 = (AbsoluteResourceLocation)Enum.Parse(typeof(AbsoluteResourceLocation), (dataAccessCmd.vials2.src));
                            cmd.DestinationVial2 = (AbsoluteResourceLocation)Enum.Parse(typeof(AbsoluteResourceLocation), (dataAccessCmd.vials2.dest));

                            cmd.SetVolumeCommandTypeParam(dataAccessCmd.Item);
                            cmd.SetVolumeCommandTypeParam2(dataAccessCmd.Item1);
                            cmd.SetVolumeCommandTypeParam3(dataAccessCmd.Item2);

                            cmd.TipRack = dataAccessCmd.tipRack;
                            cmd.TipRackSpecified = dataAccessCmd.tipRackSpecified;

                            cmd.MixCycles = 3;
                            cmd.TipTubeBottomGap_uL = 0;

                            if (dataAccessCmd.mixCycles != null)
                                cmd.MixCycles = int.Parse(dataAccessCmd.mixCycles);
                            if (dataAccessCmd.tipTubeBottomGap != null)
                                cmd.TipTubeBottomGap_uL = int.Parse(dataAccessCmd.tipTubeBottomGap);
                        }

                        break;
                    case SubCommand.TopUpTransSepTransCommand:
                        {
                            ProtocolTopUpTransSepTransCommand cmd =
                                (ProtocolTopUpTransSepTransCommand)commandInfo[i];

                            TopUpTransSepTransCommand dataAccessCmd = (TopUpTransSepTransCommand)currentCommand;
                            cmd.SourceVial = (AbsoluteResourceLocation)Enum.Parse(typeof(AbsoluteResourceLocation), (dataAccessCmd.vials.src));
                            cmd.DestinationVial = (AbsoluteResourceLocation)Enum.Parse(typeof(AbsoluteResourceLocation), (dataAccessCmd.vials.dest));
                            cmd.SourceVial2 = (AbsoluteResourceLocation)Enum.Parse(typeof(AbsoluteResourceLocation), (dataAccessCmd.vials2.src));
                            cmd.DestinationVial2 = (AbsoluteResourceLocation)Enum.Parse(typeof(AbsoluteResourceLocation), (dataAccessCmd.vials2.dest));
                            cmd.SourceVial3 = (AbsoluteResourceLocation)Enum.Parse(typeof(AbsoluteResourceLocation), (dataAccessCmd.vials3.src));
                            cmd.DestinationVial3 = (AbsoluteResourceLocation)Enum.Parse(typeof(AbsoluteResourceLocation), (dataAccessCmd.vials3.dest));

                            cmd.SetVolumeCommandTypeParam(dataAccessCmd.Item);
                            cmd.SetVolumeCommandTypeParam2(dataAccessCmd.Item1);
                            cmd.SetVolumeCommandTypeParam3(dataAccessCmd.Item2);

                            cmd.WaitCommandTimeDuration = uint.Parse(dataAccessCmd.duration); ;
                            cmd.TipRack = dataAccessCmd.tipRack;
                            cmd.TipRackSpecified = dataAccessCmd.tipRackSpecified;

                        }

                        break;
                    case SubCommand.TopUpTransCommand:
                        {
                            ProtocolTopUpTransCommand cmd =
                                (ProtocolTopUpTransCommand)commandInfo[i];

                            TopUpTransCommand dataAccessCmd = (TopUpTransCommand)currentCommand;

                            cmd.SourceVial = (AbsoluteResourceLocation)Enum.Parse(typeof(AbsoluteResourceLocation), (dataAccessCmd.vials.src));
                            cmd.DestinationVial = (AbsoluteResourceLocation)Enum.Parse(typeof(AbsoluteResourceLocation), (dataAccessCmd.vials.dest));
                            cmd.SourceVial2 = (AbsoluteResourceLocation)Enum.Parse(typeof(AbsoluteResourceLocation), (dataAccessCmd.vials2.src));
                            cmd.DestinationVial2 = (AbsoluteResourceLocation)Enum.Parse(typeof(AbsoluteResourceLocation), (dataAccessCmd.vials2.dest));

                            cmd.SetVolumeCommandTypeParam(dataAccessCmd.Item);
                            cmd.SetVolumeCommandTypeParam2(dataAccessCmd.Item1);

                            cmd.TipRack = dataAccessCmd.tipRack;
                            cmd.TipRackSpecified = dataAccessCmd.tipRackSpecified;

                        }

                        break;
                    case SubCommand.ResusMixSepTransCommand:
                        {
                            ProtocolResusMixSepTransCommand cmd =
                                (ProtocolResusMixSepTransCommand)commandInfo[i];

                            ResusMixSepTransCommand dataAccessCmd = (ResusMixSepTransCommand)currentCommand;
                            cmd.SourceVial = (AbsoluteResourceLocation)Enum.Parse(typeof(AbsoluteResourceLocation), (dataAccessCmd.vials.src));
                            cmd.DestinationVial = (AbsoluteResourceLocation)Enum.Parse(typeof(AbsoluteResourceLocation), (dataAccessCmd.vials.dest));
                            cmd.SourceVial2 = (AbsoluteResourceLocation)Enum.Parse(typeof(AbsoluteResourceLocation), (dataAccessCmd.vials2.src));
                            cmd.DestinationVial2 = (AbsoluteResourceLocation)Enum.Parse(typeof(AbsoluteResourceLocation), (dataAccessCmd.vials2.dest));

                            cmd.SetVolumeCommandTypeParam(dataAccessCmd.Item);
                            cmd.SetVolumeCommandTypeParam2(dataAccessCmd.Item1);
                            cmd.SetVolumeCommandTypeParam3(dataAccessCmd.Item2);

                            cmd.WaitCommandTimeDuration = uint.Parse(dataAccessCmd.duration); ;
                            cmd.TipRack = dataAccessCmd.tipRack;
                            cmd.TipRackSpecified = dataAccessCmd.tipRackSpecified;

                            cmd.MixCycles = 3;
                            cmd.TipTubeBottomGap_uL = 0;

                            if (dataAccessCmd.mixCycles != null)
                                cmd.MixCycles = int.Parse(dataAccessCmd.mixCycles);
                            if (dataAccessCmd.tipTubeBottomGap != null)
                                cmd.TipTubeBottomGap_uL = int.Parse(dataAccessCmd.tipTubeBottomGap);
                        }

                        break;
                    case SubCommand.ResusMixCommand:
                        {
                            ProtocolResusMixCommand cmd =
                                (ProtocolResusMixCommand)commandInfo[i];

                            ResusMixCommand dataAccessCmd = (ResusMixCommand)currentCommand;
                            cmd.SourceVial = (AbsoluteResourceLocation)Enum.Parse(typeof(AbsoluteResourceLocation), (dataAccessCmd.vials.src));
                            cmd.DestinationVial = (AbsoluteResourceLocation)Enum.Parse(typeof(AbsoluteResourceLocation), (dataAccessCmd.vials.dest));

                            cmd.SetVolumeCommandTypeParam(dataAccessCmd.Item);
                            cmd.SetVolumeCommandTypeParam2(dataAccessCmd.Item1);

                            cmd.TipRack = dataAccessCmd.tipRack;
                            cmd.TipRackSpecified = dataAccessCmd.tipRackSpecified;

                            cmd.MixCycles = 3;
                            cmd.TipTubeBottomGap_uL = 0;

                            if (dataAccessCmd.mixCycles != null)
                                cmd.MixCycles = int.Parse(dataAccessCmd.mixCycles);
                            if (dataAccessCmd.tipTubeBottomGap != null)
                                cmd.TipTubeBottomGap_uL = int.Parse(dataAccessCmd.tipTubeBottomGap);
                        }

                        break;
                    case SubCommand.MixTransCommand:
                        {
                            ProtocolMixTransCommand cmd =
                                (ProtocolMixTransCommand)commandInfo[i];

                            MixTransCommand dataAccessCmd = (MixTransCommand)currentCommand;
                            cmd.SourceVial = (AbsoluteResourceLocation)Enum.Parse(typeof(AbsoluteResourceLocation), (dataAccessCmd.vials.src));
                            cmd.DestinationVial = (AbsoluteResourceLocation)Enum.Parse(typeof(AbsoluteResourceLocation), (dataAccessCmd.vials.src));
                            cmd.SourceVial2 = (AbsoluteResourceLocation)Enum.Parse(typeof(AbsoluteResourceLocation), (dataAccessCmd.vials.src));
                            cmd.DestinationVial2 = (AbsoluteResourceLocation)Enum.Parse(typeof(AbsoluteResourceLocation), (dataAccessCmd.vials.dest));

                            cmd.SetVolumeCommandTypeParam(dataAccessCmd.Item);
                            cmd.SetVolumeCommandTypeParam2(dataAccessCmd.Item1);

                            cmd.TipRack = dataAccessCmd.tipRack;
                            cmd.TipRackSpecified = dataAccessCmd.tipRackSpecified;

                            cmd.MixCycles = 3;
                            cmd.TipTubeBottomGap_uL = 0;

                            if (dataAccessCmd.mixCycles != null)
                                cmd.MixCycles = int.Parse(dataAccessCmd.mixCycles);
                            if (dataAccessCmd.tipTubeBottomGap != null)
                                cmd.TipTubeBottomGap_uL = int.Parse(dataAccessCmd.tipTubeBottomGap);
                        }

                        break;
				}
			}
		}

		public void SetCommandSequence(IProtocolCommand[] commandInfo)
		{
			int commandCount = commandInfo.GetLength(0);
			ArrayList commandSequence = new ArrayList(commandCount);
			
			for (int i = 0; i < commandCount; ++i) 
			{
				switch ((SubCommand)commandInfo[i].CommandSubtype)
				{
					default:
					case SubCommand.HomeAllCommand:
					{
						HomeAllCommand homeAllCommand = new HomeAllCommand();
						ProtocolCommand currentCommand = (ProtocolCommand)commandInfo[i];
						homeAllCommand.seq			= currentCommand.CommandSequenceNumber.ToString();
						homeAllCommand.label		= currentCommand.CommandLabel;
						homeAllCommand.extensionTime= currentCommand.CommandExtensionTime.ToString();
						commandSequence.Add(homeAllCommand);
						break;
					}
					case SubCommand.DemoCommand:
					{
						DemoCommand demoCommand = new DemoCommand();
						ProtocolDemoCommand currentCommand = (ProtocolDemoCommand)commandInfo[i];
						demoCommand.seq				= currentCommand.CommandSequenceNumber.ToString();
						demoCommand.label			= currentCommand.CommandLabel;
						demoCommand.extensionTime	= currentCommand.CommandExtensionTime.ToString();
						demoCommand.iterations		= currentCommand.IterationCount.ToString();
						commandSequence.Add(demoCommand);
						break;
					}
					case SubCommand.PumpLifeCommand:
					 {
						 PumpLifeCommand pumpLifeCommand = new PumpLifeCommand();
						 ProtocolPumpLifeCommand currentCommand = (ProtocolPumpLifeCommand)commandInfo[i];
						 pumpLifeCommand.seq				= currentCommand.CommandSequenceNumber.ToString();
						 pumpLifeCommand.label			= currentCommand.CommandLabel;
						 pumpLifeCommand.extensionTime	= currentCommand.CommandExtensionTime.ToString();
						 pumpLifeCommand.iterations		= currentCommand.IterationCount.ToString();
						 commandSequence.Add(pumpLifeCommand);
						 break;
					 }
					case SubCommand.IncubateCommand:
					{
						IncubateCommand incubateCommand = new IncubateCommand();
						ProtocolIncubateCommand currentCommand = (ProtocolIncubateCommand)commandInfo[i];
						incubateCommand.seq				= currentCommand.CommandSequenceNumber.ToString();
						incubateCommand.label			= currentCommand.CommandLabel;
						incubateCommand.extensionTime	= currentCommand.CommandExtensionTime.ToString();
						incubateCommand.processingTime  = new processingTime();
						incubateCommand.processingTime.duration  = currentCommand.WaitCommandTimeDuration.ToString();
						commandSequence.Add(incubateCommand);
						break;
					}
					case SubCommand.SeparateCommand:
					{
						SeparateCommand separateCommand = new SeparateCommand();
						ProtocolSeparateCommand currentCommand = (ProtocolSeparateCommand)commandInfo[i];
						separateCommand.seq				= currentCommand.CommandSequenceNumber.ToString();
						separateCommand.label			= currentCommand.CommandLabel;
						separateCommand.extensionTime	= currentCommand.CommandExtensionTime.ToString();
						separateCommand.processingTime  = new processingTime();
						separateCommand.processingTime.duration  = currentCommand.WaitCommandTimeDuration.ToString();
						commandSequence.Add(separateCommand);
						break;
					}
					case SubCommand.MixCommand:
					{
						MixCommand mixCommand = new MixCommand();
						ProtocolMixCommand currentCommand = (ProtocolMixCommand)commandInfo[i];
						mixCommand.seq				= currentCommand.CommandSequenceNumber.ToString();
						mixCommand.label			= currentCommand.CommandLabel;
						mixCommand.extensionTime	= currentCommand.CommandExtensionTime.ToString();
						//mixCommand.Item = null;
						if (currentCommand.VolumeTypeSpecifier == VolumeType.Relative)
						{
							relativeVolume relVol = new relativeVolume();
							relVol.proportion = (decimal)currentCommand.RelativeVolumeProportion;
							mixCommand.Item = relVol;
						}
						else if (currentCommand.VolumeTypeSpecifier == VolumeType.Absolute)
						{
							absoluteVolume absVol = new absoluteVolume();
							absVol.value_uL = currentCommand.AbsoluteVolume_uL.ToString();
							mixCommand.Item = absVol;
						}
						mixCommand.vials = new vials();
						mixCommand.vials.src = currentCommand.SourceVial.ToString();
						mixCommand.vials.dest= currentCommand.DestinationVial.ToString();
      					mixCommand.tipRack = currentCommand.TipRack;
      					mixCommand.tipRackSpecified = currentCommand.TipRackSpecified;

						mixCommand.mixCycles = currentCommand.MixCycles.ToString();
						mixCommand.tipTubeBottomGap = currentCommand.TipTubeBottomGap_uL.ToString();

						commandSequence.Add(mixCommand);
						break;
					}

					case SubCommand.TransportCommand:
					{
						TransportCommand transportCommand = new TransportCommand();
						ProtocolTransportCommand currentCommand = (ProtocolTransportCommand)commandInfo[i];
						transportCommand.seq			= currentCommand.CommandSequenceNumber.ToString();
						transportCommand.label			= currentCommand.CommandLabel;
						transportCommand.extensionTime	= currentCommand.CommandExtensionTime.ToString();
						if (currentCommand.VolumeTypeSpecifier == VolumeType.Relative)
						{
							relativeVolume relVol = new relativeVolume();
							relVol.proportion = (decimal)currentCommand.RelativeVolumeProportion;
							transportCommand.Item = relVol;
						}
						else if (currentCommand.VolumeTypeSpecifier == VolumeType.Absolute)
						{
							absoluteVolume absVol = new absoluteVolume();
							absVol.value_uL = currentCommand.AbsoluteVolume_uL.ToString();
							transportCommand.Item = absVol;
						}
						transportCommand.vials = new vials();
						transportCommand.vials.src = currentCommand.SourceVial.ToString();
                        transportCommand.vials.dest = currentCommand.DestinationVial.ToString();
                        transportCommand.useBufferTip = currentCommand.UseBufferTip;
                        transportCommand.useBufferTipSpecified = true;
						transportCommand.freeAirDispense = currentCommand.FreeAirDispense;
      					transportCommand.tipRack = currentCommand.TipRack;
      					transportCommand.tipRackSpecified = currentCommand.TipRackSpecified;
						commandSequence.Add(transportCommand);
						break;		
					}
					
					case SubCommand.TopUpVialCommand:
					{
						TopUpVialCommand topUpVialCommand = new TopUpVialCommand();
						ProtocolTopUpVialCommand currentCommand = (ProtocolTopUpVialCommand)commandInfo[i];
						topUpVialCommand.seq			= currentCommand.CommandSequenceNumber.ToString();
						topUpVialCommand.label			= currentCommand.CommandLabel;
						topUpVialCommand.extensionTime	= currentCommand.CommandExtensionTime.ToString();
						if (currentCommand.VolumeTypeSpecifier == VolumeType.Relative)
						{
							relativeVolume relVol = new relativeVolume();
							relVol.proportion = (decimal)currentCommand.RelativeVolumeProportion;
							topUpVialCommand.Item = relVol;
						}
						else if (currentCommand.VolumeTypeSpecifier == VolumeType.Absolute)
						{
							absoluteVolume absVol = new absoluteVolume();
							absVol.value_uL = currentCommand.AbsoluteVolume_uL.ToString();
							topUpVialCommand.Item = absVol;
						}
						topUpVialCommand.vials = new vials();
						topUpVialCommand.vials.src = currentCommand.SourceVial.ToString();
						topUpVialCommand.vials.dest= currentCommand.DestinationVial.ToString();
						topUpVialCommand.freeAirDispense = currentCommand.FreeAirDispense;
      					topUpVialCommand.tipRack = currentCommand.TipRack;
      					topUpVialCommand.tipRackSpecified = currentCommand.TipRackSpecified;
						commandSequence.Add(topUpVialCommand);
						break;
					}

					case SubCommand.ResuspendVialCommand:
					{
						ResuspendVialCommand resuspendVialCommand = new ResuspendVialCommand();
						ProtocolResuspendVialCommand currentCommand = (ProtocolResuspendVialCommand)commandInfo[i];
						resuspendVialCommand.seq			= currentCommand.CommandSequenceNumber.ToString();
						resuspendVialCommand.label			= currentCommand.CommandLabel;
						resuspendVialCommand.extensionTime	= currentCommand.CommandExtensionTime.ToString();
						if (currentCommand.VolumeTypeSpecifier == VolumeType.Relative)
						{
							relativeVolume relVol = new relativeVolume();
							relVol.proportion = (decimal)currentCommand.RelativeVolumeProportion;
							resuspendVialCommand.Item = relVol;
						}
						else if (currentCommand.VolumeTypeSpecifier == VolumeType.Absolute)
						{
							absoluteVolume absVol = new absoluteVolume();
							absVol.value_uL = currentCommand.AbsoluteVolume_uL.ToString();
							resuspendVialCommand.Item = absVol;
						}
						resuspendVialCommand.vials = new vials();
						resuspendVialCommand.vials.src = currentCommand.SourceVial.ToString();
						resuspendVialCommand.vials.dest= currentCommand.DestinationVial.ToString();
						resuspendVialCommand.freeAirDispense = currentCommand.FreeAirDispense;
      			 		resuspendVialCommand.tipRack = currentCommand.TipRack;
      					resuspendVialCommand.tipRackSpecified = currentCommand.TipRackSpecified;
						commandSequence.Add(resuspendVialCommand);
						break;
					}

					case SubCommand.FlushCommand:
					{
						FlushCommand flushCommand = new FlushCommand();
						ProtocolFlushCommand currentCommand = (ProtocolFlushCommand)commandInfo[i];
						flushCommand.seq			= currentCommand.CommandSequenceNumber.ToString();
						flushCommand.label			= currentCommand.CommandLabel;
						flushCommand.extensionTime	= currentCommand.CommandExtensionTime.ToString();
						if (currentCommand.VolumeTypeSpecifier == VolumeType.Relative)
						{
							relativeVolume relVol = new relativeVolume();
							relVol.proportion = (decimal)currentCommand.RelativeVolumeProportion;
							flushCommand.Item = relVol;
						}
						else if (currentCommand.VolumeTypeSpecifier == VolumeType.Absolute)
						{
							absoluteVolume absVol = new absoluteVolume();
							absVol.value_uL = currentCommand.AbsoluteVolume_uL.ToString();
							flushCommand.Item = absVol;
						}
						flushCommand.vials = new vials();
						flushCommand.vials.src = currentCommand.SourceVial.ToString();
						flushCommand.vials.dest= currentCommand.DestinationVial.ToString();
						flushCommand.flags = new flags();
						flushCommand.flags.home = currentCommand.HomeFlag;
      					flushCommand.tipRack = currentCommand.TipRack;
      					flushCommand.tipRackSpecified = currentCommand.TipRackSpecified;
						commandSequence.Add(flushCommand);
						break;
					}

					case SubCommand.PrimeCommand:
					{
						PrimeCommand primeCommand = new PrimeCommand();
						ProtocolPrimeCommand currentCommand = (ProtocolPrimeCommand)commandInfo[i];
						primeCommand.seq			= currentCommand.CommandSequenceNumber.ToString();
						primeCommand.label			= currentCommand.CommandLabel;
						primeCommand.extensionTime	= currentCommand.CommandExtensionTime.ToString();
						if (currentCommand.VolumeTypeSpecifier == VolumeType.Relative)
						{
							relativeVolume relVol = new relativeVolume();
							relVol.proportion = (decimal)currentCommand.RelativeVolumeProportion;
							primeCommand.Item = relVol;
						}
						else if (currentCommand.VolumeTypeSpecifier == VolumeType.Absolute)
						{
							absoluteVolume absVol = new absoluteVolume();
							absVol.value_uL = currentCommand.AbsoluteVolume_uL.ToString();
							primeCommand.Item = absVol;
						}
						primeCommand.vials = new vials();
						primeCommand.vials.src = currentCommand.SourceVial.ToString();
						primeCommand.vials.dest= currentCommand.DestinationVial.ToString();
						primeCommand.flags = new flags();
						primeCommand.flags.home = currentCommand.HomeFlag;
      					primeCommand.tipRack = currentCommand.TipRack;
      					primeCommand.tipRackSpecified = currentCommand.TipRackSpecified;
						commandSequence.Add(primeCommand);
						break;
					}

					case SubCommand.PauseCommand:
					{
						PauseCommand pauseCommand = new PauseCommand();
						ProtocolPauseCommand currentCommand = (ProtocolPauseCommand)commandInfo[i];
						pauseCommand.seq				= currentCommand.CommandSequenceNumber.ToString();
						pauseCommand.label			= currentCommand.CommandLabel;
						pauseCommand.extensionTime	= currentCommand.CommandExtensionTime.ToString();
						pauseCommand.processingTime  = new processingTime();
						pauseCommand.processingTime.duration  = "10";
						commandSequence.Add(pauseCommand);
						break;
					}

                    case SubCommand.TopUpMixTransSepTransCommand:
                    {
                        TopUpMixTransSepTransCommand cmd = new TopUpMixTransSepTransCommand();
                        ProtocolTopUpMixTransSepTransCommand currentCommand = (ProtocolTopUpMixTransSepTransCommand)commandInfo[i];
                        cmd.seq = currentCommand.CommandSequenceNumber.ToString();
                        cmd.label = currentCommand.CommandLabel;
                        cmd.extensionTime = currentCommand.CommandExtensionTime.ToString();


                        cmd.SetVolumeType(currentCommand.VolumeTypeSpecifier, currentCommand.RelativeVolumeProportion, currentCommand.AbsoluteVolume_uL);

                        cmd.vials = new vials();
                        cmd.vials.src = currentCommand.SourceVial.ToString();
                        cmd.vials.dest = currentCommand.DestinationVial.ToString();


                        cmd.SetVolumeType2(currentCommand.VolumeTypeSpecifier2, currentCommand.RelativeVolumeProportion2, currentCommand.AbsoluteVolume_uL2);
                        
                        cmd.SetVolumeType3(currentCommand.VolumeTypeSpecifier3, currentCommand.RelativeVolumeProportion3, currentCommand.AbsoluteVolume_uL3);

                        cmd.vials2 = new vials2();
                        cmd.vials2.src = currentCommand.SourceVial2.ToString();
                        cmd.vials2.dest = currentCommand.DestinationVial2.ToString();

                        cmd.SetVolumeType4(currentCommand.VolumeTypeSpecifier4, currentCommand.RelativeVolumeProportion4, currentCommand.AbsoluteVolume_uL4);

                        cmd.vials3 = new vials3();
                        cmd.vials3.src = currentCommand.SourceVial3.ToString();
                        cmd.vials3.dest = currentCommand.DestinationVial3.ToString();


                        cmd.duration = currentCommand.WaitCommandTimeDuration.ToString();
                        cmd.tipTubeBottomGap = currentCommand.TipTubeBottomGap_uL.ToString();
                        cmd.mixCycles = currentCommand.MixCycles.ToString();

                        cmd.numOfStagesSpecified = true;
                        cmd.numOfStages = 4;

                        commandSequence.Add(cmd);
                        break;
                    }
                    case SubCommand.TopUpMixTransCommand:
                    {
                        TopUpMixTransCommand cmd = new TopUpMixTransCommand();
                        ProtocolTopUpMixTransCommand currentCommand = (ProtocolTopUpMixTransCommand)commandInfo[i];
                        cmd.seq = currentCommand.CommandSequenceNumber.ToString();
                        cmd.label = currentCommand.CommandLabel;
                        cmd.extensionTime = currentCommand.CommandExtensionTime.ToString();


                        cmd.SetVolumeType(currentCommand.VolumeTypeSpecifier, currentCommand.RelativeVolumeProportion, currentCommand.AbsoluteVolume_uL);

                        cmd.vials = new vials();
                        cmd.vials.src = currentCommand.SourceVial.ToString();
                        cmd.vials.dest = currentCommand.DestinationVial.ToString();


                        cmd.SetVolumeType2(currentCommand.VolumeTypeSpecifier2, currentCommand.RelativeVolumeProportion2, currentCommand.AbsoluteVolume_uL2);

                        cmd.SetVolumeType3(currentCommand.VolumeTypeSpecifier3, currentCommand.RelativeVolumeProportion3, currentCommand.AbsoluteVolume_uL3);

                        cmd.vials2 = new vials2();
                        cmd.vials2.src = currentCommand.SourceVial2.ToString();
                        cmd.vials2.dest = currentCommand.DestinationVial2.ToString();

                        
                        cmd.tipTubeBottomGap = currentCommand.TipTubeBottomGap_uL.ToString();
                        cmd.mixCycles = currentCommand.MixCycles.ToString();
                        
                        cmd.numOfStagesSpecified = true;
                        cmd.numOfStages = 3;
                        commandSequence.Add(cmd);
                        break;
                    }
                    case SubCommand.TopUpTransSepTransCommand:
                    {
                        TopUpTransSepTransCommand cmd = new TopUpTransSepTransCommand();
                        ProtocolTopUpTransSepTransCommand currentCommand = (ProtocolTopUpTransSepTransCommand)commandInfo[i];
                        cmd.seq = currentCommand.CommandSequenceNumber.ToString();
                        cmd.label = currentCommand.CommandLabel;
                        cmd.extensionTime = currentCommand.CommandExtensionTime.ToString();


                        cmd.SetVolumeType(currentCommand.VolumeTypeSpecifier, currentCommand.RelativeVolumeProportion, currentCommand.AbsoluteVolume_uL);

                        cmd.vials = new vials();
                        cmd.vials.src = currentCommand.SourceVial.ToString();
                        cmd.vials.dest = currentCommand.DestinationVial.ToString();


                        cmd.SetVolumeType2(currentCommand.VolumeTypeSpecifier2, currentCommand.RelativeVolumeProportion2, currentCommand.AbsoluteVolume_uL2);

                        cmd.vials2 = new vials2();
                        cmd.vials2.src = currentCommand.SourceVial2.ToString();
                        cmd.vials2.dest = currentCommand.DestinationVial2.ToString();

                        cmd.SetVolumeType3(currentCommand.VolumeTypeSpecifier3, currentCommand.RelativeVolumeProportion3, currentCommand.AbsoluteVolume_uL3);

                        cmd.vials3 = new vials3();
                        cmd.vials3.src = currentCommand.SourceVial3.ToString();
                        cmd.vials3.dest = currentCommand.DestinationVial3.ToString();


                        cmd.duration = currentCommand.WaitCommandTimeDuration.ToString();
                        
                        cmd.numOfStagesSpecified = true;
                        cmd.numOfStages = 3;
                        commandSequence.Add(cmd);
                        break;
                    }
                    case SubCommand.TopUpTransCommand:
                    {
                        TopUpTransCommand cmd = new TopUpTransCommand();
                        ProtocolTopUpTransCommand currentCommand = (ProtocolTopUpTransCommand)commandInfo[i];
                        cmd.seq = currentCommand.CommandSequenceNumber.ToString();
                        cmd.label = currentCommand.CommandLabel;
                        cmd.extensionTime = currentCommand.CommandExtensionTime.ToString();


                        cmd.SetVolumeType(currentCommand.VolumeTypeSpecifier, currentCommand.RelativeVolumeProportion, currentCommand.AbsoluteVolume_uL);

                        cmd.vials = new vials();
                        cmd.vials.src = currentCommand.SourceVial.ToString();
                        cmd.vials.dest = currentCommand.DestinationVial.ToString();

                        cmd.SetVolumeType2(currentCommand.VolumeTypeSpecifier2, currentCommand.RelativeVolumeProportion2, currentCommand.AbsoluteVolume_uL2);

                        cmd.vials2 = new vials2();
                        cmd.vials2.src = currentCommand.SourceVial2.ToString();
                        cmd.vials2.dest = currentCommand.DestinationVial2.ToString();

                        cmd.numOfStagesSpecified = true;
                        cmd.numOfStages = 2;
                        commandSequence.Add(cmd);
                        break;
                    }
                    case SubCommand.ResusMixSepTransCommand:
                    {
                        ResusMixSepTransCommand cmd = new ResusMixSepTransCommand();
                        ProtocolResusMixSepTransCommand currentCommand = (ProtocolResusMixSepTransCommand)commandInfo[i];
                        cmd.seq = currentCommand.CommandSequenceNumber.ToString();
                        cmd.label = currentCommand.CommandLabel;
                        cmd.extensionTime = currentCommand.CommandExtensionTime.ToString();


                        cmd.SetVolumeType(currentCommand.VolumeTypeSpecifier, currentCommand.RelativeVolumeProportion, currentCommand.AbsoluteVolume_uL);

                        cmd.vials = new vials();
                        cmd.vials.src = currentCommand.SourceVial.ToString();
                        cmd.vials.dest = currentCommand.DestinationVial.ToString();


                        cmd.SetVolumeType2(currentCommand.VolumeTypeSpecifier2, currentCommand.RelativeVolumeProportion2, currentCommand.AbsoluteVolume_uL2);

                        cmd.SetVolumeType3(currentCommand.VolumeTypeSpecifier3, currentCommand.RelativeVolumeProportion3, currentCommand.AbsoluteVolume_uL3);

                        cmd.vials2 = new vials2();
                        cmd.vials2.src = currentCommand.SourceVial2.ToString();
                        cmd.vials2.dest = currentCommand.DestinationVial2.ToString();

                        cmd.duration = currentCommand.WaitCommandTimeDuration.ToString();
                        cmd.tipTubeBottomGap = currentCommand.TipTubeBottomGap_uL.ToString();
                        cmd.mixCycles = currentCommand.MixCycles.ToString();
                        
                        cmd.numOfStagesSpecified = true;
                        cmd.numOfStages = 3;
                        commandSequence.Add(cmd);
                        break;
                    }
                    case SubCommand.ResusMixCommand:
                    {
                        ResusMixCommand cmd = new ResusMixCommand();
                        ProtocolResusMixCommand currentCommand = (ProtocolResusMixCommand)commandInfo[i];
                        cmd.seq = currentCommand.CommandSequenceNumber.ToString();
                        cmd.label = currentCommand.CommandLabel;
                        cmd.extensionTime = currentCommand.CommandExtensionTime.ToString();


                        cmd.SetVolumeType(currentCommand.VolumeTypeSpecifier, currentCommand.RelativeVolumeProportion, currentCommand.AbsoluteVolume_uL);

                        cmd.vials = new vials();
                        cmd.vials.src = currentCommand.SourceVial.ToString();
                        cmd.vials.dest = currentCommand.DestinationVial.ToString();


                        cmd.SetVolumeType2(currentCommand.VolumeTypeSpecifier2, currentCommand.RelativeVolumeProportion2, currentCommand.AbsoluteVolume_uL2);

                        cmd.tipTubeBottomGap = currentCommand.TipTubeBottomGap_uL.ToString();
                        cmd.mixCycles = currentCommand.MixCycles.ToString();
                        
                        cmd.numOfStagesSpecified = true;
                        cmd.numOfStages = 2;
                        commandSequence.Add(cmd);
                        break;
                    }

                    case SubCommand.MixTransCommand:
                    {
                        MixTransCommand cmd = new MixTransCommand();
                        ProtocolMixTransCommand currentCommand = (ProtocolMixTransCommand)commandInfo[i];
                        cmd.seq = currentCommand.CommandSequenceNumber.ToString();
                        cmd.label = currentCommand.CommandLabel;
                        cmd.extensionTime = currentCommand.CommandExtensionTime.ToString();


                        cmd.SetVolumeType(currentCommand.VolumeTypeSpecifier, currentCommand.RelativeVolumeProportion, currentCommand.AbsoluteVolume_uL);

                        cmd.vials = new vials();
                        cmd.vials.src = currentCommand.SourceVial.ToString();
                        cmd.vials.dest = currentCommand.DestinationVial2.ToString();


                        cmd.SetVolumeType2(currentCommand.VolumeTypeSpecifier2, currentCommand.RelativeVolumeProportion2, currentCommand.AbsoluteVolume_uL2);

                        

                        cmd.tipTubeBottomGap = currentCommand.TipTubeBottomGap_uL.ToString();
                        cmd.mixCycles = currentCommand.MixCycles.ToString();
                        cmd.numOfStagesSpecified = true;
                        cmd.numOfStages = 2;
                        commandSequence.Add(cmd);
                        break;
                    }
				}
			}
			myProtocol.commands.number = commandCount.ToString();
			myProtocol.commands.Items = (CommandType[])commandSequence.ToArray(typeof(CommandType));
		}

		private IProtocolCommand GetProtocolCommandSubTypeInstance(CommandType aCommand,
			out SubCommand subtype)
		{
			// Create a IProtocolCommand instance that corresponds to the CommandType 
			// described in the RoboSepProtocol schema
			IProtocolCommand protocolCommand = null;
			subtype = SubCommand.HomeAllCommand;

			if (null != aCommand as HomeAllCommand)
			{
				protocolCommand = new ProtocolHomeAllCommand();
				subtype = SubCommand.HomeAllCommand;
			}
			else if (null != aCommand as DemoCommand)
			{
				protocolCommand = new ProtocolDemoCommand();
				subtype = SubCommand.DemoCommand;
			}
			else if (null != aCommand as PumpLifeCommand)
			{
				protocolCommand = new ProtocolPumpLifeCommand();
				subtype = SubCommand.PumpLifeCommand;
			}
			else if (null != aCommand as IncubateCommand)
			{
				protocolCommand = new ProtocolIncubateCommand();
				subtype = SubCommand.IncubateCommand;
			}
			else if (null != aCommand as SeparateCommand)
			{
				protocolCommand = new ProtocolSeparateCommand();
				subtype = SubCommand.SeparateCommand;
			}
			else if (null != aCommand as TransportCommand)
			{
				protocolCommand = new ProtocolTransportCommand();
				subtype = SubCommand.TransportCommand;
			}
			else if (null != aCommand as MixCommand)
			{
				protocolCommand = new ProtocolMixCommand();	
				subtype = SubCommand.MixCommand;
			}
			else if (null != aCommand as TopUpVialCommand)
			{
				protocolCommand = new ProtocolTopUpVialCommand();
				subtype = SubCommand.TopUpVialCommand;
			}
			else if (null != aCommand as ResuspendVialCommand)
			{
				protocolCommand = new ProtocolResuspendVialCommand();
				subtype = SubCommand.ResuspendVialCommand;
			}
			else if (null != aCommand as FlushCommand)
			{
				protocolCommand = new ProtocolFlushCommand();
				subtype = SubCommand.FlushCommand;
			}
			else if (null != aCommand as PrimeCommand)
			{
				protocolCommand = new ProtocolPrimeCommand();
				subtype = SubCommand.PrimeCommand;
            }
            else if (null != aCommand as PauseCommand)
            {
                protocolCommand = new ProtocolPauseCommand();
                subtype = SubCommand.PauseCommand;
            }
            else if (null != aCommand as TopUpMixTransSepTransCommand)
            {
                protocolCommand = new ProtocolTopUpMixTransSepTransCommand();
                subtype = SubCommand.TopUpMixTransSepTransCommand;
            }
            else if (null != aCommand as TopUpMixTransCommand)
            {
                protocolCommand = new ProtocolTopUpMixTransCommand();
                subtype = SubCommand.TopUpMixTransCommand;
            }
            else if (null != aCommand as TopUpTransSepTransCommand)
            {
                protocolCommand = new ProtocolTopUpTransSepTransCommand();
                subtype = SubCommand.TopUpTransSepTransCommand;
            }
            else if (null != aCommand as TopUpTransCommand)
            {
                protocolCommand = new ProtocolTopUpTransCommand();
                subtype = SubCommand.TopUpTransCommand;
            }
            else if (null != aCommand as ResusMixSepTransCommand)
            {
                protocolCommand = new ProtocolResusMixSepTransCommand();
                subtype = SubCommand.ResusMixSepTransCommand;
            }
            else if (null != aCommand as ResusMixCommand)
            {
                protocolCommand = new ProtocolResusMixCommand();
                subtype = SubCommand.ResusMixCommand;
            }
            else if (null != aCommand as MixTransCommand)
            {
                protocolCommand = new ProtocolMixTransCommand();
                subtype = SubCommand.MixTransCommand;
            }

			return protocolCommand;
		}

		#endregion Data access







		
		#region Protocol file operations
				
		//private string			myProtocolFileName;
		private XmlSerializer	myXmlSerializer = new XmlSerializer(typeof(RoboSepProtocol));

		public bool OpenProtocolFile(string filename)
		{
			bool isOpenSuccessful = true;

			try
			{								
				// As we're about to open a new protocol file, reset the validation error 
				// count.
				myValidationErrorCount = 0;

				// Defer the file open operation to a worker thread.
      			myProtocolFileName = filename;
				Thread fileOpenThread = new Thread(new ThreadStart(this.FileOpenHelper));
				fileOpenThread.Start(); 
			}
			catch (Exception /*ex*/)
			{
				isOpenSuccessful = false;
				myProtocolFileName = string.Empty;
			}
			
			return isOpenSuccessful;
		}

		private void FileOpenHelper()
		{
			bool isOpenSuccessful = true;	// assume success

			if (ReportDataAvailable != null)
				ReportDataAvailable(this, new System.EventArgs());

            using (FileStream fs = new FileStream(myProtocolFileName, FileMode.Open, FileAccess.Read))  // 2012-03-01 sp added read access
            {
                try
                {
                    // Initialise a file stream for reading

                    // Deserialize a RoboSepProtocol XML description into a RoboSepProtocol 
                    // object that matches the contents of the specified protocol file.										
                    XmlReader reader = new XmlTextReader(fs);

                    // Create a validating reader to process the file.  Report any errors to the 
                    // validation page.
                    XmlValidatingReader validatingReader = new XmlValidatingReader(reader);
                    validatingReader.ValidationType = ValidationType.Schema;
                    validatingReader.ValidationEventHandler += new ValidationEventHandler(ValidationErrorHandler);

                    // Get the RoboSep protocol schema and add it to the collection for the 
                    // validator
                    XmlSchemaCollection xsc = new XmlSchemaCollection();
                    xsc.Add(theSchemaNamespace, theSchemaPath + @"\" + theSchemaFileName);
                    validatingReader.Schemas.Add(xsc);

                    // 'Rehydrate' the object (that is, deserialise data into the object)					
                    myProtocol = (RoboSepProtocol)myXmlSerializer.Deserialize(validatingReader);

                    //init new variables not in old version protocols
                    AddRobosepTypeIfAbsent();
                    if (myProtocol.header.workaround == null)
                    {
                        myProtocol.header.workaround = new workaround();
                    }


                    //determine protocol format
                    if (myProtocol.header.versionInfo == null)
                    {
                        if (myProtocol.header.robosep.type == null)
                        {
                            if (myProtocol.vialBarcodes == null)
                            {
                                ProtocolFormat = ProtocolFormat.RoboSepS_1_0;
                            }
                            else
                            {
                                ProtocolFormat = ProtocolFormat.RoboSepS_1_1;
                            }
                        }
                        else
                        {
                            ProtocolFormat = SeparatorResourceManager.StringToProtocolFormat(myProtocol.header.robosep.type);
                        }
                    }
                    else
                    {
                        ProtocolFormat = SeparatorResourceManager.StringToProtocolFormat(myProtocol.header.versionInfo.protocolFormat);
                        
                    }



                    //do RS-16 hack conversion
                    // div by multipler
                    ApplyHackAbsoulteVolumeMultipler(myProtocol, true);

                }
                catch (Exception /*ex*/)
                {
                    isOpenSuccessful = false;
                }
                finally
                {
                    // Close the file stream
                    fs.Close();
                }
            }

			// Update data state tracking.  Alert interested parties if the file was 
			// successfully opened.
			myIsDataAvailable = isOpenSuccessful;
			if (isOpenSuccessful && myValidationErrorCount == 0)
			{
				if (ReportDataAvailable != null)
				{
					ReportDataAvailable(this, new System.EventArgs());
					
					if (ReportDataVolumeCheck != null)
						ReportDataVolumeCheck(this, new System.EventArgs());
				}
			}

            
		}


        public bool SaveProtocolFile()        // 2012-03-02 sp -- added return for error checking
		{
			return( SaveAsProtocolFile(myProtocolFileName, false, ProtocolFormat.RoboSepS_1_0,false) );
		}

        public bool SaveAsProtocolFile(string filename, bool boConvert, ProtocolFormat convertToType, bool resetRevision)        // 2012-03-02 sp -- added return for error checking
		{
			// Defer the file save operation to a worker thread.
			myProtocolFileName = filename;

			//Thread fileSaveThread = new Thread(new ThreadStart(this.FileSaveHelper));	//CWJ MOD
			//fileSaveThread.Start();													//CWJ MOD
            return (FileSaveHelper(boConvert, convertToType, resetRevision));													//CWJ MOD // SP added return
		}

        public bool FileSaveHelper(bool boConvert, ProtocolFormat convertToType, bool resetRevision)        // 2012-03-02 sp -- added return for error checking
		{
			// Serialise the current RoboSepProtocol object data (to disk, overwriting
			// the file if it already exists)

            bool fileSaveOK = true;
            if ( System.IO.File.Exists(myProtocolFileName) )
            {
                System.IO.FileInfo f = new System.IO.FileInfo(myProtocolFileName);
                if( f.IsReadOnly )
                {
                    MessageBox.Show( "This file is read only.\nTry saving using a different file name.", "Save Protocol",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    fileSaveOK = false;
                }
            }
            if( fileSaveOK )
            {
                using (FileStream fs = new FileStream(myProtocolFileName, FileMode.Create))
                {
                    try
                    {
                        int oldMultiplier = ProtocolHackAbsoluteVolumeMultipler;
                        if (resetRevision)
                        {
                            ProtocolVersion = "1";
                        }

                        RoboSepProtocol myCopy = (boConvert)?
                            convertTable[(int)ProtocolFormat, (int)convertToType]():
                            DeepClone(myProtocol);

                        int newMultiplier = ProtocolHackAbsoluteVolumeMultipler;

                        if (oldMultiplier == newMultiplier) //if not converting to new multiplier then apply
                        {
                            ApplyHackAbsoulteVolumeMultipler(myCopy, false);
                        }

                        /*
                        //if convert RS-S to RS-16 only
                        if (convertToType == ProtocolRobosepTypes.RoboSep16 && ProtocolRoboSepType != ProtocolRobosepTypes.RoboSep16)
                        {
                            ProtocolRoboSepType = ProtocolRobosepTypes.RoboSep16;
                            ProtocolHackAbsoluteVolumeMultipler = 5;
                            myCopy = DeepClone(myProtocol);
                        }
                        else
                        {
                            //do RS-16 hack conversion
                            //make copy and convert
                            myCopy = DeepClone(myProtocol);
                            ApplyHackAbsoulteVolumeMultipler(myCopy, false);
                        }



                        //remove unneeded tags from robo-s protocol
                        if (SeparatorResourceManager.StringToRobosepType(myCopy.header.robosep.type) != ProtocolRobosepTypes.RoboSep16)
                        {
                            myCopy.header.robosep = null;
                            myCopy.header.workaround = null;
                            
                            if (myCopy.customNames != null)
                            {
                                foreach (customNames names in myCopy.customNames)
                                {
                                    names.bufferBottle34 = null;
                                    names.bufferBottle56 = null;
                                }

                            }
                        }
                        */

                        AddMissingFieldsAndRemoveExtraFieldsBaseOnProtocolVersion(myCopy);



                        XmlTextWriter writer = new XmlTextWriter(fs, new UTF8Encoding());
                        writer.Formatting = Formatting.Indented;
                        myXmlSerializer.Serialize(writer, myCopy);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                        fileSaveOK = false;
                    }
                    finally
                    {
                        fs.Close();
                    }
                }
            }
            return (fileSaveOK);
		}

        private void AddMissingFieldsAndRemoveExtraFieldsBaseOnProtocolVersion(RoboSepProtocol myCopy)
        {
            if (ProtocolFormat == ProtocolFormat.RoboSep16)
            {
                //make sure robosep.type is filled in...
                AddRobosepTypeIfAbsent();
                myCopy.header.robosep.type = ProtocolFormat.RoboSep16.ToString();

                //add protocol number
                AddProtocolNumToRS16(myCopy);
                //remove feature list, rev info
                RemoveFeatureList(myCopy);
                RemoveVersionInfo(myCopy);
            }
            else if (ProtocolFormat == ProtocolFormat.RoboSep16_1_2)
            {
                //obsolete field
                myCopy.header.robosep = null;

                //add protocol number
                AddProtocolNumToRS16(myCopy);
            }
            else if (ProtocolFormat == ProtocolFormat.RoboSepS_1_0)
            {
                notRS16Helper(myCopy);
                //remove barcode vial, feature list, rev info
                RemoveVialBarcodes(myCopy);
                RemoveFeatureList(myCopy);
                RemoveVersionInfo(myCopy);
            }
            else if (ProtocolFormat == ProtocolFormat.RoboSepS_1_1)
            {
                notRS16Helper(myCopy);
                //add barcode vial
                AddVialBarcodesIfAbsent(myCopy);
                //remove feature list, rev info
                RemoveFeatureList(myCopy);
                RemoveVersionInfo(myCopy);
            }
            else if (ProtocolFormat == ProtocolFormat.RoboSepS_1_2)
            {
                notRS16Helper(myCopy);
                //add barcode vial
                AddVialBarcodesIfAbsent(myCopy);
            }
        }



		#region Protocol file validation error handling

		private int myValidationErrorCount;

		public void ValidationErrorHandler(object sender, ValidationEventArgs args)
		{
			++myValidationErrorCount;

			if (myProtocol != null)
			{
				myProtocol = null;
			}

			if (ReportValidationException != null)
			{
				ReportValidationException(sender, args);
			}
		}

		#endregion Protocol file validation error handling




        private void ApplyHackAbsoulteVolumeMultipler(RoboSepProtocol myProtocol, bool boUndo)
        {
            if(myProtocol.header.workaround == null) return;

            //shortcut if it is robosep s
            if (!SeparatorResourceManager.isPlatformRS16()) return;

            //shortcut if multiplier is 1 or less
            if (int.Parse(myProtocol.header.workaround.absoluteVolMultiplier) <= 1) return;

            double multiplier = double.Parse(myProtocol.header.workaround.absoluteVolMultiplier);
            if (boUndo && multiplier!=0)
            {
                multiplier = 1 / multiplier; //won't be 0, so it's ok.
            }

            //now find all of following
            /*
             * The following field values must be multiplied by 5 when the XML file is created: 
             * Minimum, Maximum, Working Volume Threshold, Low, High. 
             * Also at any time if an Absolute Volume is selected on a step, it must be multipled by 5 as well. 
             * 
             */
            myProtocol.constraints.sampleVolume.min_uL = ApplyMultiplier(myProtocol.constraints.sampleVolume.min_uL, multiplier);
            myProtocol.constraints.sampleVolume.max_uL = ApplyMultiplier(myProtocol.constraints.sampleVolume.max_uL, multiplier);

            myProtocol.constraints.workingVolume.sampleThreshold_uL = ApplyMultiplier(myProtocol.constraints.workingVolume.sampleThreshold_uL, multiplier);
            myProtocol.constraints.workingVolume.lowVolume_uL = ApplyMultiplier(myProtocol.constraints.workingVolume.lowVolume_uL, multiplier);
            myProtocol.constraints.workingVolume.highVolume_uL = ApplyMultiplier(myProtocol.constraints.workingVolume.highVolume_uL, multiplier);

            #region loop through commands
            foreach (var currentCommand in myProtocol.commands.Items)
            {
                // Allocate a 'ProtocolCommand' instance and fill out the base type members
                SubCommand protocolCommandSubtype = SubCommand.HomeAllCommand;
                GetProtocolCommandSubTypeInstance(currentCommand, out protocolCommandSubtype);

                // Fill out the subtype members
                switch (protocolCommandSubtype)
                {
                    case SubCommand.HomeAllCommand:
                    case SubCommand.DemoCommand:
                    case SubCommand.PumpLifeCommand:
                    case SubCommand.IncubateCommand:
                    case SubCommand.SeparateCommand:
                    case SubCommand.FlushCommand:
                    case SubCommand.PrimeCommand:
                    case SubCommand.PauseCommand:
                        // No action required
                        break;

                    case SubCommand.TransportCommand:
                        {
                            TransportCommand dataAccessCmd = (TransportCommand)currentCommand;
                            ApplyMultiplierToAbsoluteVolume(dataAccessCmd.Item, multiplier);
                        }
                        break;

                    case SubCommand.MixCommand:
                        {
                            MixCommand dataAccessCmd = (MixCommand)currentCommand;
                            ApplyMultiplierToAbsoluteVolume(dataAccessCmd.Item, multiplier);
                        }
                        break;

                    case SubCommand.TopUpVialCommand:
                        {
                            TopUpVialCommand dataAccessCmd = (TopUpVialCommand)currentCommand;
                            ApplyMultiplierToAbsoluteVolume(dataAccessCmd.Item, multiplier);
                        }
                        break;
                    case SubCommand.ResuspendVialCommand:
                        {
                            ResuspendVialCommand dataAccessCmd = (ResuspendVialCommand)currentCommand;
                            ApplyMultiplierToAbsoluteVolume(dataAccessCmd.Item, multiplier);
                        }
                        break;

                    case SubCommand.TopUpMixTransSepTransCommand:
                        {
                            TopUpMixTransSepTransCommand dataAccessCmd = (TopUpMixTransSepTransCommand)currentCommand;
                            ApplyMultiplierToAbsoluteVolume(dataAccessCmd.Item, multiplier);
                            ApplyMultiplierToAbsoluteVolume2(dataAccessCmd.Item1, multiplier);
                            ApplyMultiplierToAbsoluteVolume3(dataAccessCmd.Item2, multiplier);
                            ApplyMultiplierToAbsoluteVolume4(dataAccessCmd.Item3, multiplier);
                        }
                        break;
                    case SubCommand.TopUpMixTransCommand:
                        {
                            TopUpMixTransCommand dataAccessCmd = (TopUpMixTransCommand)currentCommand;
                            ApplyMultiplierToAbsoluteVolume(dataAccessCmd.Item, multiplier);
                            ApplyMultiplierToAbsoluteVolume2(dataAccessCmd.Item1, multiplier);
                            ApplyMultiplierToAbsoluteVolume3(dataAccessCmd.Item2, multiplier);
                        }
                        break;
                    case SubCommand.TopUpTransSepTransCommand:
                        {
                            TopUpTransSepTransCommand dataAccessCmd = (TopUpTransSepTransCommand)currentCommand;
                            ApplyMultiplierToAbsoluteVolume(dataAccessCmd.Item, multiplier);
                            ApplyMultiplierToAbsoluteVolume2(dataAccessCmd.Item1, multiplier);
                            ApplyMultiplierToAbsoluteVolume3(dataAccessCmd.Item2, multiplier);
                        }
                        break;
                    case SubCommand.TopUpTransCommand:
                        {
                            TopUpTransCommand dataAccessCmd = (TopUpTransCommand)currentCommand;
                            ApplyMultiplierToAbsoluteVolume(dataAccessCmd.Item, multiplier);
                            ApplyMultiplierToAbsoluteVolume2(dataAccessCmd.Item1, multiplier);
                        }
                        break;
                    case SubCommand.ResusMixSepTransCommand:
                        {
                            ResusMixSepTransCommand dataAccessCmd = (ResusMixSepTransCommand)currentCommand;
                            ApplyMultiplierToAbsoluteVolume(dataAccessCmd.Item, multiplier);
                            ApplyMultiplierToAbsoluteVolume2(dataAccessCmd.Item1, multiplier);
                            ApplyMultiplierToAbsoluteVolume3(dataAccessCmd.Item2, multiplier);
                        }
                        break;
                    case SubCommand.ResusMixCommand:
                        {
                            ResusMixCommand dataAccessCmd = (ResusMixCommand)currentCommand;
                            ApplyMultiplierToAbsoluteVolume(dataAccessCmd.Item, multiplier);
                            ApplyMultiplierToAbsoluteVolume2(dataAccessCmd.Item1, multiplier);
                        }
                        break;
                    case SubCommand.MixTransCommand:
                        {
                            MixTransCommand dataAccessCmd = (MixTransCommand)currentCommand;
                            ApplyMultiplierToAbsoluteVolume(dataAccessCmd.Item, multiplier);
                            ApplyMultiplierToAbsoluteVolume2(dataAccessCmd.Item1, multiplier);
                        }
                        break;
                }
            }
            #endregion

        }

        private static void ApplyMultiplierToAbsoluteVolume(object item, double multiplier)
        {
            if (item as Tesla.DataAccess.absoluteVolume != null)
            {
                DataAccess.absoluteVolume absVol = (Tesla.DataAccess.absoluteVolume)item;
                absVol.value_uL = ApplyMultiplier(absVol.value_uL, multiplier);
            }
        }
        private static void ApplyMultiplierToAbsoluteVolume2(object item, double multiplier)
        {
            if (item as Tesla.DataAccess.absoluteVolume2 != null)
            {
                DataAccess.absoluteVolume2 absVol = (Tesla.DataAccess.absoluteVolume2)item;
                absVol.value_uL = ApplyMultiplier(absVol.value_uL, multiplier);
            }
        }
        private static void ApplyMultiplierToAbsoluteVolume3(object item, double multiplier)
        {
            if (item as Tesla.DataAccess.absoluteVolume3 != null)
            {
                DataAccess.absoluteVolume3 absVol = (Tesla.DataAccess.absoluteVolume3)item;
                absVol.value_uL = ApplyMultiplier(absVol.value_uL, multiplier);
            }
        }
        private static void ApplyMultiplierToAbsoluteVolume4(object item, double multiplier)
        {
            if (item as Tesla.DataAccess.absoluteVolume4 != null)
            {
                DataAccess.absoluteVolume4 absVol = (Tesla.DataAccess.absoluteVolume4)item;
                absVol.value_uL = ApplyMultiplier(absVol.value_uL, multiplier);
            }
        }
        private static string ApplyMultiplier(string sValue, double multiplier)
        {
            return Convert.ToInt32(int.Parse(sValue) * multiplier).ToString();
        }
        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;
                return (T)formatter.Deserialize(ms);
            }
        }



		#endregion Protocol file operations		

        public List<AbsoluteResourceLocation> GetListOfReagentVialsUsedInProtocol()
        {
            List<AbsoluteResourceLocation> result = new List<AbsoluteResourceLocation>();
            IProtocolCommand[] myCommandSequence = null;
            GetCommandSequence(out myCommandSequence);
            if (myCommandSequence == null) return result;

            #region loop through commands
            foreach (var currentCommand in myCommandSequence)
            {
                // Allocate a 'ProtocolCommand' instance and fill out the base type members

                IVolumeCommand volumeCommand = currentCommand as IVolumeCommand;
                if (volumeCommand != null)
                {
                    AddAbsoluteResourceLocationToList(result, volumeCommand.SourceVial);
                    AddAbsoluteResourceLocationToList(result, volumeCommand.DestinationVial);
                }

                IMultiSrcDestCommand multiSrcDestCmd = currentCommand as IMultiSrcDestCommand;
                if (multiSrcDestCmd != null)
                {
                    AddAbsoluteResourceLocationToList(result, multiSrcDestCmd.SourceVial);
                    AddAbsoluteResourceLocationToList(result, multiSrcDestCmd.DestinationVial);
                    AddAbsoluteResourceLocationToList(result, multiSrcDestCmd.SourceVial2);
                    AddAbsoluteResourceLocationToList(result, multiSrcDestCmd.DestinationVial2);
                    AddAbsoluteResourceLocationToList(result, multiSrcDestCmd.SourceVial3);
                    AddAbsoluteResourceLocationToList(result, multiSrcDestCmd.DestinationVial3);
                }

            }
            #endregion
            return result;
        }

        private static void AddAbsoluteResourceLocationToList(List<AbsoluteResourceLocation> result, AbsoluteResourceLocation sLoc)
        {
            //try
            {
                //sLoc = (AbsoluteResourceLocation)Enum.Parse(typeof(AbsoluteResourceLocation), srcVial);
                if (!result.Contains(sLoc))
                {
                    result.Add(sLoc);
                }
            }
            //catch (Exception) { }
            return;
        }

        public bool IsConversionAvailable(ProtocolFormat type)
        {
            return convertTable[(int)ProtocolFormat, (int)type]!=null;
        }


        //convertTable
        private RoboSepProtocol notRS16Helper(RoboSepProtocol myCopy)
        {
            myCopy.header.robosep = null;
            myCopy.header.workaround = null;

            NoBuffer34AndBuffer56(myCopy);
            return myCopy;
        }
        //RS-S to RS-16
        private void RSToRS16HelperBeforeDeepCopy()
        {
            ProtocolFormat = ProtocolFormat.RoboSep16_1_2;
            ProtocolHackAbsoluteVolumeMultipler = 5;
            AddProtocolNumToRS16(myProtocol);
        }

        private void AddProtocolNumToRS16(RoboSepProtocol myCopy)
        {
            if (myCopy.header.protocolNum1 == null)
            {
                myCopy.header.protocolNum1 = "00000";
            }
            if (myCopy.header.protocolNum2 == null)
            {
                myCopy.header.protocolNum2 = "0000";
            }
        }
        private void NoBuffer34AndBuffer56(RoboSepProtocol myCopy)
        {
            if (myCopy.customNames != null)
            {
                foreach (customNames names in myCopy.customNames)
                {
                    names.bufferBottle34 = null;
                    names.bufferBottle56 = null;
                }

            }
        }



        public void UpdateForReagentCustomVialUse()
        {
            bool[] quadrantUsed = new bool[] { false, false, false, false };
            List<AbsoluteResourceLocation> lstUsed = GetListOfReagentVialsUsedInProtocol();
            for (int i = 0; i < lstUsed.Count; i++)
            {
                switch (lstUsed[i])
                {
                    case AbsoluteResourceLocation.TPC0103:
                    case AbsoluteResourceLocation.TPC0105:
                    case AbsoluteResourceLocation.TPC0104:
                        quadrantUsed[0] = true;
                        break;

                    case AbsoluteResourceLocation.TPC0203:
                    case AbsoluteResourceLocation.TPC0205:
                    case AbsoluteResourceLocation.TPC0204:
                        quadrantUsed[1] = true;
                        break;

                    case AbsoluteResourceLocation.TPC0303:
                    case AbsoluteResourceLocation.TPC0305:
                    case AbsoluteResourceLocation.TPC0304:
                        quadrantUsed[2] = true;
                        break;

                    case AbsoluteResourceLocation.TPC0403:
                    case AbsoluteResourceLocation.TPC0405:
                    case AbsoluteResourceLocation.TPC0404:
                        quadrantUsed[3] = true;
                        break;

                }
            }

            UpdateVialBarcodeQuadrant(quadrantUsed);
        }
        public bool[] GetQuadrantUse()
        {
            bool[] quadrantUsed = new bool[] { false, false, false, false };
            List<AbsoluteResourceLocation> lstUsed = GetListOfReagentVialsUsedInProtocol();
            for (int i = 0; i < lstUsed.Count; i++)
            {
                switch (lstUsed[i])
                {
                    case AbsoluteResourceLocation.TPC0101:
                    case AbsoluteResourceLocation.TPC0102:
                    case AbsoluteResourceLocation.TPC0103:
                    case AbsoluteResourceLocation.TPC0105:
                    case AbsoluteResourceLocation.TPC0104:
                    case AbsoluteResourceLocation.TPC0106:
                    case AbsoluteResourceLocation.TPC0107:
                        quadrantUsed[0] = true;
                        break;

                    case AbsoluteResourceLocation.TPC0201:
                    case AbsoluteResourceLocation.TPC0202:
                    case AbsoluteResourceLocation.TPC0203:
                    case AbsoluteResourceLocation.TPC0205:
                    case AbsoluteResourceLocation.TPC0204:
                    case AbsoluteResourceLocation.TPC0206:
                    case AbsoluteResourceLocation.TPC0207:
                        quadrantUsed[1] = true;
                        break;

                    case AbsoluteResourceLocation.TPC0301:
                    case AbsoluteResourceLocation.TPC0302:
                    case AbsoluteResourceLocation.TPC0303:
                    case AbsoluteResourceLocation.TPC0305:
                    case AbsoluteResourceLocation.TPC0304:
                    case AbsoluteResourceLocation.TPC0306:
                    case AbsoluteResourceLocation.TPC0307:
                        quadrantUsed[2] = true;
                        break;

                    case AbsoluteResourceLocation.TPC0401:
                    case AbsoluteResourceLocation.TPC0402:
                    case AbsoluteResourceLocation.TPC0403:
                    case AbsoluteResourceLocation.TPC0405:
                    case AbsoluteResourceLocation.TPC0404:
                    case AbsoluteResourceLocation.TPC0406:
                    case AbsoluteResourceLocation.TPC0407:
                        quadrantUsed[3] = true;
                        break;

                }
            }

            return quadrantUsed;
        }

        private RoboSepProtocol convertRS10ToRS11()
        {
            AddVialBarcodesIfAbsent();
            UpdateForReagentCustomVialUse();
            ProtocolFormat = ProtocolFormat.RoboSepS_1_1;
            RoboSepProtocol myCopy = DeepClone(myProtocol);
            myCopy = notRS16Helper(myCopy);
            return myCopy;
        }
        private RoboSepProtocol convertRS10ToRS12()
        {
            AddVialBarcodesIfAbsent();
            UpdateForReagentCustomVialUse();
            ProtocolFormat = ProtocolFormat.RoboSepS_1_2;
            RoboSepProtocol myCopy = DeepClone(myProtocol);
            myCopy = notRS16Helper(myCopy);
            return myCopy;
        }
        private RoboSepProtocol convertRS10ToRS1612()
        {
            RSToRS16HelperBeforeDeepCopy();
            RoboSepProtocol myCopy = DeepClone(myProtocol);
            return myCopy;
        }
        private RoboSepProtocol convertRS11ToRS10()
        {
            ProtocolFormat = ProtocolFormat.RoboSepS_1_0;
            RoboSepProtocol myCopy = DeepClone(myProtocol);
            myCopy.vialBarcodes = null;
            myCopy = notRS16Helper(myCopy);
            return myCopy;
        }
        private RoboSepProtocol convertRS11ToRS12()
        {
            ProtocolFormat = ProtocolFormat.RoboSepS_1_2;
            RoboSepProtocol myCopy = DeepClone(myProtocol);
            myCopy = notRS16Helper(myCopy);
            return myCopy;
        }
        private RoboSepProtocol convertRS11ToRS1612()
        {
            RSToRS16HelperBeforeDeepCopy();
            RoboSepProtocol myCopy = DeepClone(myProtocol);
            return myCopy;
        }

        private RoboSepProtocol convertRS12ToRS10()
        {
            ProtocolFormat = ProtocolFormat.RoboSepS_1_0;
            RoboSepProtocol myCopy = DeepClone(myProtocol);
            myCopy.vialBarcodes = null;
            myCopy = notRS16Helper(myCopy);
            return myCopy;
        }
        private RoboSepProtocol convertRS12ToRS11()
        {
            ProtocolFormat = ProtocolFormat.RoboSepS_1_1;
            RoboSepProtocol myCopy = DeepClone(myProtocol);
            myCopy = notRS16Helper(myCopy);
            return myCopy;
        }
        private RoboSepProtocol convertRS12ToRS1612()
        {
            RSToRS16HelperBeforeDeepCopy();
            RoboSepProtocol myCopy = DeepClone(myProtocol);
            return myCopy;
        }

        public feature FindFeature(string name)
        {
            feature[] protocolFeature = getFeatureList();
            if (protocolFeature == null) return null;
            foreach (DataAccess.feature f in protocolFeature)
            {
                if (f.name == name)
                {
                    return f;
                }
            }
            return null;
        }

        public feature[] getFeatureList()
        {
            return myProtocol.constraints.featuresSwitches;
        }

        public void RemoveFeatureList(RoboSepProtocol myCopy)
        {
            myCopy.constraints.featuresSwitches = null;
        }

        public void setFeatureList(List<Feature> features)
        {
            //TODO make sure version is > 1.2

            List<Feature> inUseFeatures = new List<Feature>();
            foreach (Feature f in features)
            {
                if (f.Checked)
                {
                    inUseFeatures.Add(f);
                }
            }

            myProtocol.constraints.featuresSwitches = new feature[inUseFeatures.Count];
            for (int i = 0; i < myProtocol.constraints.featuresSwitches.Length; i++)
            {
                Feature src = inUseFeatures[i];
                myProtocol.constraints.featuresSwitches[i] = src.MakeFeatureForXML();
            }
        }

    }

    public class Feature
    {
        public string Name;
        public string Description;
        public List<string> ArgumentTypes;
        public List<string> ArgumentData;
        public bool Checked;

        public Feature(string name, string ini_value)
        {
            //from ini
            //RSS3mlVial   =  [string, int, int]  RSS 3mL reagent vials

            Name = name;
            Checked = false;

            string tmp = ini_value;

            int left_bracket_idx = tmp.IndexOf('[');
            int right_bracket_idx = tmp.IndexOf(']');

            if (left_bracket_idx != -1 && right_bracket_idx != -1 && right_bracket_idx > left_bracket_idx)
            {
                Description = tmp.Substring(right_bracket_idx + 1).Trim();
                tmp = tmp.Substring(left_bracket_idx + 1, right_bracket_idx - left_bracket_idx - 1);
                ArgumentTypes = new List<string>(tmp.Split(','));
            }
            else
            {
                Description = tmp;
                ArgumentTypes = new List<string>();
            }

            //has to be same size as type
            ArgumentData = new List<string>();
            for (int i = 0; i < ArgumentTypes.Count; ++i)
            {
                ArgumentTypes[i] = ArgumentTypes[i].Trim(); //get rid of leading spaces
                ArgumentData.Add("");
            }

            printObj();
        }
        public Feature(feature f)
        {
            //from xml
            //  <feature name="RSS3mlVial" desc=�RSS 3mL reagent vials� 
            //           inputType="string, int, int" inputData=�blahblah; 1; 3�/> 

            Name = f.name;
            Description = f.desc;
            ArgumentTypes = new List<string>();
            ArgumentData = new List<string>();
            updateFeature(f);
        }

        public void updateFeature(feature f)
        {
            List<string> tmpArgumentTypes = new List<string>(f.inputType.Split(','));
            ArgumentTypes = tmpArgumentTypes.Count >= ArgumentTypes.Count ? tmpArgumentTypes : ArgumentTypes;


            ArgumentData = new List<string>(f.inputData.Split(';'));

            if (ArgumentTypes.Count > ArgumentData.Count)
            {
                int extras = ArgumentTypes.Count - ArgumentData.Count;
                for (int i = 0; i < extras; ++i)
                {
                    ArgumentData.Add("");
                }

            }

            for (int i = 0; i < ArgumentTypes.Count; ++i)
            {
                ArgumentTypes[i] = ArgumentTypes[i].Trim(); //get rid of leading spaces
            }

            Checked = true;
            printObj();
        }

        private void printObj()
        {
            System.Diagnostics.Debug.WriteLine("Name: " + this.Name + " Description: " + this.Description + " Checked: " + this.Checked);
            int i = 1;
            foreach (string arg in this.ArgumentTypes)
            {
                System.Diagnostics.Debug.WriteLine("Arg" + i + ": " + arg);
                i++;
            }
        }
        public feature MakeFeatureForXML()
        {
            feature dest = new feature();
            dest.name = Name;
            dest.desc = Description;
            dest.inputType = toTypesString();
            dest.inputData = toDataString();
            return dest;
        }
        internal string toTypesString()
        {
            return String.Join(",", ArgumentTypes.ToArray());
        }

        internal string toDataString()
        {
            return String.Join(";", ArgumentData.ToArray());
        }

        public string[] ArgumentTypeAndDataArray()
        {
            int count = ArgumentTypes.Count;
            if (count <= 0) return new string[0];

            string[] results = new string[count];
            for (int i = 0; i < count; ++i)
            {
                results[i] = "(" + ArgumentTypes[i] + ") " + (ArgumentData.Count>i?ArgumentData[i]:"");
            }

            return results;
        }

        public void ClearData()
        {
            ArgumentData.Clear();
            for (int i = 0; i < ArgumentTypes.Count; ++i)
            {
                ArgumentData.Add("");
            }

        }
    }
}

