//----------------------------------------------------------------------------
// 
// ProtocolPrintingUtils by CR - Cecelia Redding
//
//	03/24/06 - Print layout fix - RL
//  03/29/06 - pause command - RL (look for PauseCommand)
//----------------------------------------------------------------------------
//
// 2011-09-19 sp 
//     - change fixed application path folder to variable obtained during program execution 
//
//----------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Collections;																	
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Configuration;

using System.Xml;
using System.Xml.Schema;

using Tesla.ProtocolEditorModel;
using Tesla.Common.Protocol;
using Tesla.Common.ProtocolCommand;
using Tesla.Common.ResourceManagement;
using Tesla.Common.Separator;
using Tesla.DataAccess;

namespace Tesla.ProtocolEditor
{
	public class ProtocolPrintingUtils
	{

		#region Variables

		private ProtocolModel	theProtocolModel;
		private PrintDocument	printDoc;

		/*Specialized Font*/
		private Font sectionFont = new Font("Arial", 14, FontStyle.Bold);
		private Font titleFont = new Font("Arial", 10, FontStyle.Bold);
		private Font dataFont = new Font("Arial", 10, FontStyle.Regular);
		private Font smallDataFont = new Font("Arial", 9, FontStyle.Regular);
		private Font headerFont = new Font("Arial", 12, FontStyle.Regular);  //IAT

		/*Specialized Format*/
		StringFormat dataFormat = new StringFormat();

		/*Declare the header and footer image*/
        // 2011-09-19 sp
        // replace fixed directory path with the installed path
        Bitmap header = new Bitmap(Tesla.Common.Utilities.GetDefaultAppPath() + "bin\\StemCellSmallHeader.bmp");
        //Bitmap header = new Bitmap("C:\\Program Files\\STI\\RoboSep\\bin\\StemCellSmallHeader.bmp");
		
		/*Specialized Pen*/
		private Pen myPen = new Pen(Brushes.Black);

		/*Command Info Variables*/
		IProtocolCommand[] commandInfo;
		SubCommand current;
		//command sequence data
		string[] data = new string[11];
		bool[] gray = new bool[11];

		/*Declare Basic Page Settings*/
		private int topMargin = 40;
		private const int leftMargin = 70;
		
		/*Other*/
		private int pageNum = 0;
		private int pageTotal = 0;
        private const int field = 20;
        private int commandNum = 0;
        private int commandPartNum = 0;
		private int commandTotal = 0;
		private int rowNum = 0;

		private ProtocolClass pc;
        private customNames[] allQuadrantsCustomNames;


		#endregion

		#region Construction

		public ProtocolPrintingUtils()
		{			
			dataFormat.FormatFlags = StringFormatFlags.LineLimit;
			dataFormat.Trimming = StringTrimming.EllipsisWord;	
		}

		#endregion Construction

		#region Accessible Utils

		public bool PrintDocumentPage(Graphics g,ProtocolModel theProtocolModel, PrintDocument printDoc, int pageHeight)
		{
			pageNum++;
			this.theProtocolModel = theProtocolModel;
			this.printDoc = printDoc;
			pc = theProtocolModel.ProtocolClass;

            allQuadrantsCustomNames = new customNames[4];

            for (int i = 0; i < allQuadrantsCustomNames.Length; i++)
			{
                try
                {
                    allQuadrantsCustomNames[i] = theProtocolModel.GetCustomNames(i);
                }
                catch (Exception)
                {
                    allQuadrantsCustomNames[i] = new customNames();
                    allQuadrantsCustomNames[i].bufferBottle =
                    allQuadrantsCustomNames[i].wasteTube =
                    allQuadrantsCustomNames[i].lysisBufferTube =
                    allQuadrantsCustomNames[i].selectionCocktailVial =
                   allQuadrantsCustomNames[i].magneticParticleVial =
                    allQuadrantsCustomNames[i].antibodyCocktailVial =
                    allQuadrantsCustomNames[i].sampleTube =
                    allQuadrantsCustomNames[i].separationTube =
                    allQuadrantsCustomNames[i].bufferBottle34 =
                    allQuadrantsCustomNames[i].bufferBottle56 = "";
                }
			}

			topMargin = 40;

			//calculate total pages
			if (pageNum == 1)
			{
				CalculateTotalPages();
			}
			
			/*Print the header on every page*/
			PrintPageHeader(g);
			
			/*Only print on the first page*/
			if (pageNum == 1)
			{
				PrintProtocolDetails(g);
				PrintApplicationDetails(g,pageHeight);
                PrintFeatureSwitches(g, pageHeight);
			}
			
			/*Start or Continue printing*/
			PrintCommandSequence(g,pageHeight);
			

			return (commandNum<commandTotal)?true:false;

			/*More Pages?*/
			//return (pageNum == pageTotal) ? false : true;
		}

		private void CalculateTotalPages()
		{
			/*Get command sequence and number of them*/
			theProtocolModel.GetCommandSequence(out commandInfo);
			commandTotal = commandInfo.Length;

			int c = 0; //number of characters
			int rows = 0; //number of rows

			for(int i=0; i<commandTotal; i++)
			{
				switch (commandInfo[i].CommandSubtype)
				{
					case SubCommand.HomeAllCommand:
					case SubCommand.IncubateCommand:
					case SubCommand.SeparateCommand:
					case SubCommand.PauseCommand:
						rows++;
						c = commandInfo[i].CommandLabel.Length;
						if (c > 15)
							rows++;
						break;
                    case (SubCommand.TopUpMixTransSepTransCommand):
                        rows += 9;
						c = commandInfo[i].CommandLabel.Length;
						if (c > 30)
							rows++;
                        break;
                    case (SubCommand.TopUpMixTransCommand):
                        rows += 6;
						c = commandInfo[i].CommandLabel.Length;
						if (c > 30)
							rows++;
                        break;
                    case (SubCommand.TopUpTransSepTransCommand):
                        rows += 7;
						c = commandInfo[i].CommandLabel.Length;
						if (c > 30)
							rows++;
                        break;
                    case (SubCommand.TopUpTransCommand):
                        rows += 4;
						c = commandInfo[i].CommandLabel.Length;
						if (c > 30)
							rows++;
                        break;
                    case (SubCommand.ResusMixSepTransCommand):
                        rows += 7;
						c = commandInfo[i].CommandLabel.Length;
						if (c > 30)
							rows++;
                        break;
                    case (SubCommand.ResusMixCommand):
                        rows += 4;
						c = commandInfo[i].CommandLabel.Length;
						if (c > 30)
							rows++;
                        break;
                    case (SubCommand.MixTransCommand):
                        rows += 4;
						c = commandInfo[i].CommandLabel.Length;
						if (c > 30)
							rows++;
                        break;
					default:
						rows += 2;
						c = commandInfo[i].CommandLabel.Length;
						if (c > 30)
							rows++;
						break;
				}
			}

			if (rows < 9) pageTotal = 1;
			else pageTotal = (rows-9)/29 + 2;
		}


		#endregion

		#region Printing Helpers
		
		// RL - Print fix - 03/24/06
		private string DateToString(DateTime date)
		{
			string[] months = {"Jan", "Feb", "Mar", "Apr", "May", "Jun",
								  "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"};
			return date.Day.ToString("00")+"-"+ months[date.Month-1]+"-"+date.Year;
		}

		private void PrintPageHeader(Graphics g)
		{
			/*Draw Header Image*/
			g.DrawImage(header,new Rectangle(leftMargin,topMargin,785,127));

			/*Draw Current Date*/
			DateTime currentDate = DateTime.Now;
				
			string formatCurrentDate = DateToString(currentDate)+" "+
				currentDate.Hour.ToString("00")+":"+
				currentDate.Minute.ToString("00")+":"+
				currentDate.Second.ToString("00"); //IAT
			g.DrawString(formatCurrentDate+"",titleFont,Brushes.Gray,445,143); //IAT

			/*Draw Page Numbers*/
			g.DrawString(pageNum+"          "+pageTotal,headerFont,Brushes.Gray,766,141);
		}

        

		private void PrintProtocolDetails(Graphics g)
		{
			/*Local Variables*/
			int rightMargin = leftMargin+550;
			int dataL = leftMargin+125;
			int dataR = rightMargin+150;
			int S1 = topMargin+127;

            string[] title1 = {"Type:","Protocol Label:", "Author:", "File Name:", "RoboSep Type:"};

            string[] title2 = {"Protocol Number:", "Date Created:", "Date Modified:", "Version:",
                             "Abs Vol Multiplier:"};
			
			// RL - Print fix - 03/24/06
			if(theProtocolModel.ProtocolNumber1==null)theProtocolModel.ProtocolNumber1="";
			if(theProtocolModel.ProtocolNumber2==null)theProtocolModel.ProtocolNumber2="";
			if(theProtocolModel.ProtocolNumber1.Length < 5)
			{
				string pad = "";
				for(int i= 0; i< 5-theProtocolModel.ProtocolNumber1.Length; i++)
				{
					pad+="0";
				}
				theProtocolModel.ProtocolNumber1 = pad + theProtocolModel.ProtocolNumber1;	
			}
			if(theProtocolModel.ProtocolNumber2.Length < 4)
			{
				string pad = "";
				for(int i= 0; i< 4-theProtocolModel.ProtocolNumber2.Length; i++)
				{
					pad+="0";
				}
				theProtocolModel.ProtocolNumber2 = pad + theProtocolModel.ProtocolNumber2;	
			}
			if(theProtocolModel.ProtocolModificationDate == DateTime.MinValue)
			{
				theProtocolModel.ProtocolModificationDate = DateTime.Now;
			}
			string[] data1 = {theProtocolModel.ProtocolClass+"",theProtocolModel.ProtocolLabel+"", theProtocolModel.ProtocolAuthor+"", 
								theProtocolModel.ProtocolFile+"", theProtocolModel.ProtocolFormat+""};
            string[] data2 = {theProtocolModel.ProtocolNumber1+"-"+theProtocolModel.ProtocolNumber2, 
								DateToString(theProtocolModel.ProtocolCreationDate), 
								DateToString(theProtocolModel.ProtocolModificationDate),
								theProtocolModel.ProtocolVersion+"", theProtocolModel.ProtocolHackAbsoluteVolumeMultipler+""};


			/*Specialized Format*/
			dataFormat.Alignment = StringAlignment.Near;
			StringFormat titleFormat = new StringFormat();
				titleFormat.Alignment = StringAlignment.Far;

			/*Section Title*/
			g.DrawString("Protocol Details", sectionFont, Brushes.Black, leftMargin, S1+10);

			string temp;
			System.Drawing.SizeF size;
			int currentAdd=0;
            int totalAdd = 40;
            int iMax = (SeparatorResourceManager.isPlatformRS16()) ? 5 : 4;
            for (int i = 0; i < iMax; ++i)
			{
				temp = data1[i];
				size=g.MeasureString(data1[i],dataFont,425);
				currentAdd=Convert.ToInt32(size.Height)+1;

				g.DrawRectangle(myPen, leftMargin, S1+totalAdd, 125, currentAdd);
				g.DrawString(title1[i],titleFont,Brushes.Black,
					new Rectangle(leftMargin,S1+totalAdd,125,currentAdd),titleFormat);
				g.DrawRectangle(myPen,dataL,S1+totalAdd,425,currentAdd);
				g.DrawString(data1[i],dataFont,Brushes.Black,
					new Rectangle(dataL+5,S1+totalAdd+1,425,currentAdd),dataFormat);

				g.DrawRectangle(myPen, rightMargin, S1+totalAdd, 150, currentAdd);
				g.DrawString(title2[i],titleFont,Brushes.Black,
					new Rectangle(rightMargin,S1+totalAdd,150,currentAdd),titleFormat);
				g.DrawRectangle(myPen,dataR,S1+totalAdd,125,currentAdd);
				g.DrawString(data2[i],dataFont,Brushes.Black,
					new Rectangle(dataR+5,S1+totalAdd+1,125,currentAdd),dataFormat);
				totalAdd+=currentAdd;

			}

			topMargin+=totalAdd;

			/*
			int[] j = {0, 0, 0, 0}; //IAT
			//First Column
			for (int i = 0; i < 4; i++)//IAT
				{
				// if more too many characters for one line, set flag.
				string temp = data[i];
				if(temp.Length > 50)//IAT
					j[i] = 1;
                
				g.DrawRectangle(myPen, leftMargin, S1+add[i], 125, field*(j[i]+1));//IAT
				g.DrawString(title[i],titleFont,Brushes.Black,
					new Rectangle(leftMargin,S1+add[i],125,field*(j[i]+1)),titleFormat);//IAT
				g.DrawRectangle(myPen,dataL,S1+add[i],425,field*(j[i]+1));//IAT
				g.DrawString(data[i],dataFont,Brushes.Black,
					new Rectangle(dataL+5,S1+add[i]+1,425,field*(j[i]+1)),dataFormat);//IAT
			}

			//Second Column
			for (int i = 4; i < 8; i++)//IAT
			{
				g.DrawRectangle(myPen, rightMargin, S1+add[i], 150, field*(j[i-4]+1));//IAT
				g.DrawString(title[i],titleFont,Brushes.Black,
					new Rectangle(rightMargin,S1+add[i],150,field*(j[i-4]+1)),titleFormat);//IAT

				g.DrawRectangle(myPen,dataR,S1+add[i],125,field*(j[i-4]+1));//IAT
				g.DrawString(data[i],dataFont,Brushes.Black,
					new Rectangle(dataR+5,S1+add[i]+1,125,field*(j[i-4]+1)),dataFormat);//IAT
			}
			*/
		}

		private void PrintApplicationDetails(Graphics g, int pageHeight)
		{
			/*Local Variables*/
			int dataL = leftMargin+250;
			//int S1 = 352;//IAT
			int S1 = topMargin+180;

			string[] title = {"Sample minimum volume (uL):","Sample maximum volume (uL):","","Working volume threshold (uL):","","Low resuspension volume (uL):","High resuspension volume (uL):"};//IAT
			string[] data = {theProtocolModel.SampleVolumeMinimum+"",theProtocolModel.SampleVolumeMaximum+"","",theProtocolModel.WorkingVolumeSampleThreshold+"",
								"",theProtocolModel.WorkingVolumeLowVolume+"",theProtocolModel.WorkingVolumeHighVolume+""};//IAT

			/*Section Title*/
			g.DrawString("Application:", sectionFont, Brushes.Black, leftMargin, S1+10);

			/*Protocol Description*/
			

			dataFont = new Font("Arial",12);
			System.Drawing.SizeF size;
			size=g.MeasureString(theProtocolModel.ProtocolDesc,dataFont,850);
			
			if(size.Height<20)
				size.Height=20;

			if(pageHeight-S1<size.Height-20+20*9+20*4)
			{
				size.Height=pageHeight-S1-20*9+20-20*4;
			}
			
			
			if(size.Height<20)
				size.Height=20;
			
			g.DrawString(theProtocolModel.ProtocolDesc,dataFont,Brushes.Black,new Rectangle(leftMargin+125,S1+14, 850, Convert.ToInt32(size.Height)),dataFormat);
			dataFont = new Font("Arial",11);

			S1+=Convert.ToInt32(size.Height)-20;

			topMargin=S1+20*9;

			/*Draw each row*/
			for (int i=2; i<9; i++)//IAT
			{
				switch(i)
				{
					//IAT
					case 2: 
					case 3: // Bold values for Min and Max volume values
						g.DrawRectangle(myPen, leftMargin, S1+20*i, 250, field);
						g.DrawString(title[i-2],titleFont,Brushes.Black,new Rectangle(leftMargin,S1+20*i,250,field));

						g.DrawRectangle(myPen,dataL,S1+20*i,150,field);
						g.DrawString(data[i-2],titleFont,Brushes.Black,new Rectangle(dataL+5,S1+20*i,150,field));
						break;
					case 4:
					case 6://blank row
						g.DrawRectangle(myPen, leftMargin, S1+20*i,250,field);
						g.DrawRectangle(myPen,dataL,S1+20*i,150,field);
						break;
					default:
						g.DrawRectangle(myPen, leftMargin, S1+20*i, 250, field);
						g.DrawString(title[i-2],dataFont,Brushes.Black,new Rectangle(leftMargin,S1+20*i,250,field));//IAT
		
						g.DrawRectangle(myPen,dataL,S1+20*i,150,field);
						g.DrawString(data[i-2],dataFont,Brushes.Black,new Rectangle(dataL+5,S1+20*i,150,field));//IAT
						break;
				}
			}
		}

        private void PrintFeatureSwitches(Graphics g, int pageHeight)
        {
            feature[] protocolFeature = theProtocolModel.getFeatureList();
            if (protocolFeature == null) return;

            int S1 = topMargin;
            int i = 0;
            int dataL = leftMargin + 250;
            int totalAdd = 40;

            /*Section Title*/
            g.DrawString("Features:", sectionFont, Brushes.Black, leftMargin, S1 + 5);
            S1 += 30;
            foreach (DataAccess.feature f in protocolFeature)
            {
                g.DrawRectangle(myPen, leftMargin, S1 + 20 * i, 250, field);
                g.DrawString(f.name, dataFont, Brushes.Black, new Rectangle(leftMargin, S1 + 20 * i, 250, field));

                g.DrawRectangle(myPen, dataL, S1 + 20 * i, 150, field);
                g.DrawString(f.inputData, dataFont, Brushes.Black, new Rectangle(dataL + 5, S1 + 20 * i, 150, field));

                i++;
                totalAdd += 20;
            }
            topMargin += totalAdd;
        }
		private void PrintCommandSequence(Graphics g, int pageHeight)
		{
			int S1 = topMargin+20;//552;//for page 1
			int S2 = topMargin+135;//for all other pages
			int height = 0;//height of all rows
			int movingTop = 0;//moving top header
			int need = 0;//rows needed

			//start back at row 1
			rowNum = 1;

			//table headers
			string[] titles = {"\nStep", "\nCommand", "\nLabel", "\nSet Time", "Extension Time", 
							"\nSource", "\nDestination", "\nVolume", "Value/ Proportion", "Free Air Dispense", "Tip Rack"};//IAT

			if (pageNum == 1)
			{
				/*Section Title*/
				g.DrawString("Command Sequence", sectionFont, Brushes.Black, leftMargin, S1+10);
				/*Table Header*/
				DrawTableHeaderRow(g,titles,S1+40,field*2,titleFont);
				//Start at row 1 and top under section title and with the first command
				movingTop = S1 + 80;
				commandNum = 0;
                commandPartNum = 0;
			}
			else
			{
				/*Table Header*/
				DrawTableHeaderRow(g,titles,S2,field*2,titleFont);
				//start new page with with row one under table header
				movingTop = S2+40;
			}


			//start printing row by row
			for (int i=commandNum; i<commandTotal ;i++,commandNum = i,commandPartNum = 0)
			{
				/*Gather general info*/
				current = commandInfo[i].CommandSubtype;
				data[0] = commandInfo[i].CommandSequenceNumber+"";
				data[1] = commandInfo[i].CommandSubtype+"";
				data[2] = commandInfo[i].CommandLabel+"";
				data[4] = commandInfo[i].CommandExtensionTime+"";


                ResetToNoGray();

				switch(current)
				{
					case(SubCommand.FlushCommand):
						FlushData(out need, out height, i);
						break;
					case(SubCommand.PrimeCommand):
						PrimeData(out need, out height, i);
						break;
					case(SubCommand.HomeAllCommand):
						HomeAllData(out need, out height, i);
						break;
					case(SubCommand.DemoCommand):
						DemoData(out need, out height, i);
						break;
					case(SubCommand.PumpLifeCommand):
						PumpLifeData(out need, out height, i);
						break;
					case(SubCommand.IncubateCommand):
						IncubateData(out need, out height, i);
						break;
					case(SubCommand.SeparateCommand):
						SeparateData(out need, out height, i);//IAT
						break;
					case(SubCommand.MixCommand):
						MixData(out need, out height, i);
						break;
					case(SubCommand.ResuspendVialCommand):
						ResuspendVialData(out need, out height, i);
						break;
					case(SubCommand.TransportCommand):
						TransportData(out need, out height, i);
						break;
					case(SubCommand.TopUpVialCommand):
						TopUpVialData(out need, out height, i);
                        break;
                    case (SubCommand.PauseCommand):
                        PauseData(out need, out height, i);//IAT
                        break;
                    case (SubCommand.TopUpMixTransSepTransCommand):
                        {
                            ProtocolTopUpMixTransSepTransCommand cmd = (ProtocolTopUpMixTransSepTransCommand)commandInfo[i];
                            if (commandPartNum <= 0)
                            {
                                TopUpVialDataEx(out need, out height, (AbsoluteResourceLocation)cmd.SourceVial, (AbsoluteResourceLocation)cmd.DestinationVial,
                                                cmd.FreeAirDispense, cmd.TipRackSpecified, cmd.TipRack, cmd.VolumeTypeSpecifier, cmd.RelativeVolumeProportion);

                                if (CheckOffPageAndDrawTableRow(g, pageHeight, height, need, ref movingTop)) return;
                                commandPartNum = 1;
                            } 
                            data[2] = ""; data[4] = "";
                            if (commandPartNum <= 1)
                            {
                                MixDataEx(out need, out height, (AbsoluteResourceLocation)cmd.DestinationVial, (AbsoluteResourceLocation)cmd.DestinationVial,
                                                cmd.TipRackSpecified, cmd.TipRack, cmd.VolumeTypeSpecifier2, cmd.AbsoluteVolume_uL2, cmd.RelativeVolumeProportion2,
                                                cmd.MixCycles,cmd.TipTubeBottomGap_uL);
                                gray[2] = true; gray[4] = true;
                                if (CheckOffPageAndDrawTableRow(g, pageHeight, height, need, ref movingTop)) return;
                                commandPartNum = 2;
                            }
                            if (commandPartNum <= 2)
                            {
                                TransportDataEx(out need, out height, (AbsoluteResourceLocation)cmd.SourceVial2, (AbsoluteResourceLocation)cmd.DestinationVial2,
                                                cmd.FreeAirDispense, cmd.TipRackSpecified, cmd.TipRack, cmd.UseBufferTip, cmd.VolumeTypeSpecifier3, cmd.AbsoluteVolume_uL3, cmd.RelativeVolumeProportion3);
                                gray[2] = true; gray[4] = true;
                                if (CheckOffPageAndDrawTableRow(g, pageHeight, height, need, ref movingTop)) return;
                                commandPartNum = 3;
                            }
                            if (commandPartNum <= 3)
                            {
                                SeparateDataEx(out need, out height,cmd.WaitCommandTimeDuration);
                                gray[2] = true; gray[4] = true;
                                if (CheckOffPageAndDrawTableRow(g, pageHeight, height, need, ref movingTop)) return;
                                commandPartNum = 4;
                            }
                                
                            
                            TransportDataEx(out need, out height, (AbsoluteResourceLocation)cmd.SourceVial3, (AbsoluteResourceLocation)cmd.DestinationVial3,
                                                cmd.FreeAirDispense, cmd.TipRackSpecified, cmd.TipRack, cmd.UseBufferTip, cmd.VolumeTypeSpecifier4, cmd.AbsoluteVolume_uL4, cmd.RelativeVolumeProportion4);
                            gray[2] = true; gray[4] = true;
                        }
                        break;
                    case (SubCommand.TopUpMixTransCommand):
                        {
                            ProtocolTopUpMixTransCommand cmd = (ProtocolTopUpMixTransCommand)commandInfo[i];
                            if (commandPartNum <= 0)
                            {
                                TopUpVialDataEx(out need, out height, (AbsoluteResourceLocation)cmd.SourceVial, (AbsoluteResourceLocation)cmd.DestinationVial,
                                                cmd.FreeAirDispense, cmd.TipRackSpecified, cmd.TipRack, cmd.VolumeTypeSpecifier, cmd.RelativeVolumeProportion);

                                if (CheckOffPageAndDrawTableRow(g, pageHeight, height, need, ref movingTop)) return;
                                commandPartNum = 1;
                            }
                            data[2] = ""; data[4] = "";
                            if (commandPartNum <= 1)
                            {
                                MixDataEx(out need, out height, (AbsoluteResourceLocation)cmd.DestinationVial, (AbsoluteResourceLocation)cmd.DestinationVial,
                                                cmd.TipRackSpecified, cmd.TipRack, cmd.VolumeTypeSpecifier2, cmd.AbsoluteVolume_uL2, cmd.RelativeVolumeProportion2,
                                                cmd.MixCycles, cmd.TipTubeBottomGap_uL);
                                gray[2] = true; gray[4] = true;
                                if (CheckOffPageAndDrawTableRow(g, pageHeight, height, need, ref movingTop)) return;
                                commandPartNum = 2;
                            }

                            TransportDataEx(out need, out height, (AbsoluteResourceLocation)cmd.SourceVial2, (AbsoluteResourceLocation)cmd.DestinationVial2,
                                                cmd.FreeAirDispense, cmd.TipRackSpecified, cmd.TipRack, cmd.UseBufferTip, cmd.VolumeTypeSpecifier3, cmd.AbsoluteVolume_uL3, cmd.RelativeVolumeProportion3);
                            gray[2] = true; gray[4] = true;
                        }
                        break;
                    case (SubCommand.TopUpTransSepTransCommand):
                        {
                            ProtocolTopUpTransSepTransCommand cmd = (ProtocolTopUpTransSepTransCommand)commandInfo[i];
                            if (commandPartNum <= 0)
                            {
                                TopUpVialDataEx(out need, out height, (AbsoluteResourceLocation)cmd.SourceVial, (AbsoluteResourceLocation)cmd.DestinationVial,
                                                cmd.FreeAirDispense, cmd.TipRackSpecified, cmd.TipRack, cmd.VolumeTypeSpecifier, cmd.RelativeVolumeProportion);

                                if (CheckOffPageAndDrawTableRow(g, pageHeight, height, need, ref movingTop)) return;
                                commandPartNum = 1;
                            }
                            data[2] = ""; data[4] = "";
                            if (commandPartNum <= 1)
                            {
                                TransportDataEx(out need, out height, (AbsoluteResourceLocation)cmd.SourceVial2, (AbsoluteResourceLocation)cmd.DestinationVial2,
                                                cmd.FreeAirDispense, cmd.TipRackSpecified, cmd.TipRack, cmd.UseBufferTip, cmd.VolumeTypeSpecifier2, cmd.AbsoluteVolume_uL2, cmd.RelativeVolumeProportion2);
                                gray[2] = true; gray[4] = true;
                                if (CheckOffPageAndDrawTableRow(g, pageHeight, height, need, ref movingTop)) return;
                                commandPartNum = 2;
                            }
                            if (commandPartNum <= 2)
                            {
                                SeparateDataEx(out need, out height, cmd.WaitCommandTimeDuration);
                                gray[2] = true; gray[4] = true;
                                if (CheckOffPageAndDrawTableRow(g, pageHeight, height, need, ref movingTop)) return;
                                commandPartNum = 3;
                            }


                            TransportDataEx(out need, out height, (AbsoluteResourceLocation)cmd.SourceVial3, (AbsoluteResourceLocation)cmd.DestinationVial3,
                                                cmd.FreeAirDispense, cmd.TipRackSpecified, cmd.TipRack, cmd.UseBufferTip, cmd.VolumeTypeSpecifier3, cmd.AbsoluteVolume_uL3, cmd.RelativeVolumeProportion3);
                            gray[2] = true; gray[4] = true;
                        }
                        break;
                    case (SubCommand.TopUpTransCommand):
                        {
                        ProtocolTopUpTransCommand cmd = (ProtocolTopUpTransCommand)commandInfo[i];
                            if (commandPartNum <= 0)
                            {
                                TopUpVialDataEx(out need, out height, (AbsoluteResourceLocation)cmd.SourceVial, (AbsoluteResourceLocation)cmd.DestinationVial,
                                                cmd.FreeAirDispense, cmd.TipRackSpecified, cmd.TipRack, cmd.VolumeTypeSpecifier, cmd.RelativeVolumeProportion);

                                if (CheckOffPageAndDrawTableRow(g, pageHeight, height, need, ref movingTop)) return;
                                commandPartNum = 1;
                            }
                            data[2] = ""; data[4] = "";
                            TransportDataEx(out need, out height, (AbsoluteResourceLocation)cmd.SourceVial2, (AbsoluteResourceLocation)cmd.DestinationVial2,
                                               cmd.FreeAirDispense, cmd.TipRackSpecified, cmd.TipRack, cmd.UseBufferTip, cmd.VolumeTypeSpecifier2, cmd.AbsoluteVolume_uL2, cmd.RelativeVolumeProportion2);
                            gray[2] = true; gray[4] = true;
                        }
                        break;
                    case (SubCommand.ResusMixSepTransCommand):
                        {
                            ProtocolResusMixSepTransCommand cmd = (ProtocolResusMixSepTransCommand)commandInfo[i];
                            if (commandPartNum <= 0)
                            {
                                ResuspendVialDataEx(out need, out height, (AbsoluteResourceLocation)cmd.SourceVial, (AbsoluteResourceLocation)cmd.DestinationVial,
                                                cmd.FreeAirDispense, cmd.TipRackSpecified, cmd.TipRack, cmd.VolumeTypeSpecifier, 0, cmd.RelativeVolumeProportion);

                                if (CheckOffPageAndDrawTableRow(g, pageHeight, height, need, ref movingTop)) return;
                                commandPartNum = 1;
                            }
                            data[2] = ""; data[4] = "";
                            if (commandPartNum <= 1)
                            {
                                MixDataEx(out need, out height, (AbsoluteResourceLocation)cmd.DestinationVial, (AbsoluteResourceLocation)cmd.DestinationVial,
                                                cmd.TipRackSpecified, cmd.TipRack, cmd.VolumeTypeSpecifier2, cmd.AbsoluteVolume_uL2, cmd.RelativeVolumeProportion2,
                                                cmd.MixCycles, cmd.TipTubeBottomGap_uL);
                                gray[2] = true; gray[4] = true;
                                if (CheckOffPageAndDrawTableRow(g, pageHeight, height, need, ref movingTop)) return;
                                commandPartNum = 2;
                            }
                            if (commandPartNum <= 2)
                            {
                                SeparateDataEx(out need, out height, cmd.WaitCommandTimeDuration);
                                gray[2] = true; gray[4] = true;
                                if (CheckOffPageAndDrawTableRow(g, pageHeight, height, need, ref movingTop)) return;
                                commandPartNum = 3;
                            }


                            TransportDataEx(out need, out height, (AbsoluteResourceLocation)cmd.SourceVial2, (AbsoluteResourceLocation)cmd.DestinationVial2,
                                                cmd.FreeAirDispense, cmd.TipRackSpecified, cmd.TipRack, cmd.UseBufferTip, cmd.VolumeTypeSpecifier3, cmd.AbsoluteVolume_uL3, cmd.RelativeVolumeProportion3);
                            gray[2] = true; gray[4] = true;
                        }
                        break;
                    case (SubCommand.ResusMixCommand):
                        {
                            ProtocolResusMixCommand cmd = (ProtocolResusMixCommand)commandInfo[i];
                            if (commandPartNum <= 0)
                            {
                                ResuspendVialDataEx(out need, out height, (AbsoluteResourceLocation)cmd.SourceVial, (AbsoluteResourceLocation)cmd.DestinationVial,
                                                cmd.FreeAirDispense, cmd.TipRackSpecified, cmd.TipRack, cmd.VolumeTypeSpecifier, 0, cmd.RelativeVolumeProportion);

                                if (CheckOffPageAndDrawTableRow(g, pageHeight, height, need, ref movingTop)) return;
                                commandPartNum = 1;
                            }
                            data[2] = ""; data[4] = "";
                            MixDataEx(out need, out height, (AbsoluteResourceLocation)cmd.DestinationVial, (AbsoluteResourceLocation)cmd.DestinationVial,
                                            cmd.TipRackSpecified, cmd.TipRack, cmd.VolumeTypeSpecifier2, cmd.AbsoluteVolume_uL2, cmd.RelativeVolumeProportion2,
                                            cmd.MixCycles, cmd.TipTubeBottomGap_uL);

                            gray[2] = true; gray[4] = true;
                        }
                        break;
                    case (SubCommand.MixTransCommand):
                        {
                            ProtocolMixTransCommand cmd = (ProtocolMixTransCommand)commandInfo[i];

                            if (commandPartNum <= 0)
                            {
                                MixDataEx(out need, out height, (AbsoluteResourceLocation)cmd.SourceVial, (AbsoluteResourceLocation)cmd.DestinationVial,
                                                cmd.TipRackSpecified, cmd.TipRack, cmd.VolumeTypeSpecifier, cmd.AbsoluteVolume_uL, cmd.RelativeVolumeProportion,
                                                cmd.MixCycles, cmd.TipTubeBottomGap_uL);

                                if (CheckOffPageAndDrawTableRow(g, pageHeight, height, need, ref movingTop)) return;
                                commandPartNum = 1;
                            }
                            data[2] = ""; data[4] = "";
                            TransportDataEx(out need, out height, (AbsoluteResourceLocation)cmd.SourceVial2, (AbsoluteResourceLocation)cmd.DestinationVial2,
                                            false, cmd.TipRackSpecified, cmd.TipRack, false, cmd.VolumeTypeSpecifier2, cmd.AbsoluteVolume_uL2, cmd.RelativeVolumeProportion2);
                            gray[2] = true; gray[4] = true;
                        }
                        break;

				}//switch

                if (CheckOffPageAndDrawTableRow(g, pageHeight, height, need, ref movingTop)) return;

			}//for
		}

        private void ResetToNoGray()
        {
            gray[0] = gray[1] = gray[2] = gray[3] = gray[4] = gray[5] = false;
            gray[6] = gray[7] = gray[8] = gray[9] = gray[10] = false;
        }

        private bool CheckOffPageAndDrawTableRow(Graphics g, int pageHeight, int height, int need, ref int movingTop)
        {
            //check to see if off page
            //if (((pageNum==1)&&(rowNum+need > 9))||((pageNum!=1)&&(rowNum+need > 29)))
            if (pageHeight < movingTop + height)
            {
                //commandNum = i; 
                return true;
            }

            //print info
            DrawTableRow(g, data, movingTop, height, dataFont);
            //increment top margin
            movingTop += height;
            //increment the number of rows printed
            rowNum += need;
            return false;
        }

		public void GetTranslatedVial(out string data, AbsoluteResourceLocation loc, ProtocolClass pc)
		{
            customNames currentQuadrantCustomNames = allQuadrantsCustomNames[0];

			//TUBE NAMES SYNC
			string quadrantString = "Q0, ";
			switch(loc)
			{
                case AbsoluteResourceLocation.TPC0001:
                case AbsoluteResourceLocation.TPC0034:
                case AbsoluteResourceLocation.TPC0056:
					quadrantString = "Q0, ";
					break;
				case AbsoluteResourceLocation.TPC0101:
				case AbsoluteResourceLocation.TPC0102:
				case AbsoluteResourceLocation.TPC0103:
				case AbsoluteResourceLocation.TPC0104:
				case AbsoluteResourceLocation.TPC0105:
				case AbsoluteResourceLocation.TPC0106:
				case AbsoluteResourceLocation.TPC0107:
					quadrantString = "Q1, ";
                    currentQuadrantCustomNames = allQuadrantsCustomNames[0];
					break;
				case AbsoluteResourceLocation.TPC0201:
				case AbsoluteResourceLocation.TPC0202:
				case AbsoluteResourceLocation.TPC0203:
				case AbsoluteResourceLocation.TPC0204:
				case AbsoluteResourceLocation.TPC0205:
				case AbsoluteResourceLocation.TPC0206:
				case AbsoluteResourceLocation.TPC0207:
					quadrantString = "Q2, ";
                    currentQuadrantCustomNames = allQuadrantsCustomNames[1];
					break;
				case AbsoluteResourceLocation.TPC0301:
				case AbsoluteResourceLocation.TPC0302:
				case AbsoluteResourceLocation.TPC0303:
				case AbsoluteResourceLocation.TPC0304:
				case AbsoluteResourceLocation.TPC0305:
				case AbsoluteResourceLocation.TPC0306:
				case AbsoluteResourceLocation.TPC0307:
					quadrantString = "Q3, ";
					currentQuadrantCustomNames = allQuadrantsCustomNames[2];
					break;
				case AbsoluteResourceLocation.TPC0401:
				case AbsoluteResourceLocation.TPC0402:
				case AbsoluteResourceLocation.TPC0403:
				case AbsoluteResourceLocation.TPC0404:
				case AbsoluteResourceLocation.TPC0405:
				case AbsoluteResourceLocation.TPC0406:
				case AbsoluteResourceLocation.TPC0407:
					quadrantString = "Q4, ";
					currentQuadrantCustomNames = allQuadrantsCustomNames[3];
					break;
			}
			switch(loc)
			{
				case AbsoluteResourceLocation.TPC0001:
                    data = currentQuadrantCustomNames.bufferBottle;
					if (data == "" || data==null)
						data = SeparatorResourceManager.GetSeparatorString(StringId.QuadrantBuffer);
					break;
				case AbsoluteResourceLocation.TPC0101:
				case AbsoluteResourceLocation.TPC0201:
				case AbsoluteResourceLocation.TPC0301:
				case AbsoluteResourceLocation.TPC0401:
                    data = quadrantString + currentQuadrantCustomNames.wasteTube;
					if(data == quadrantString)
					{
						data += SeparatorResourceManager.GetSeparatorString(StringId.WasteTube);
					}
					break;
				case AbsoluteResourceLocation.TPC0102:
				case AbsoluteResourceLocation.TPC0202:
				case AbsoluteResourceLocation.TPC0302:
				case AbsoluteResourceLocation.TPC0402:
                    data = quadrantString + currentQuadrantCustomNames.lysisBufferTube;
					if(data == quadrantString)
					{
						if (pc == ProtocolClass.WholeBloodPositive)
							data += SeparatorResourceManager.GetSeparatorString(StringId.LysisBufferTube);
						else if (pc == ProtocolClass.WholeBloodNegative)
							data += SeparatorResourceManager.GetSeparatorString(StringId.LysisBufferNegativeFractionTube);
						else
							data += SeparatorResourceManager.GetSeparatorString(StringId.NegativeFractionTube);
					}
					break;
				case AbsoluteResourceLocation.TPC0103:
				case AbsoluteResourceLocation.TPC0203:
				case AbsoluteResourceLocation.TPC0303:
				case AbsoluteResourceLocation.TPC0403:
					data = quadrantString + currentQuadrantCustomNames.selectionCocktailVial;
					if (data == quadrantString)
					{
                        data += SeparatorResourceManager.getVialBStringFromProtocolClass(pc);
					}
					break;
				case AbsoluteResourceLocation.TPC0104:
				case AbsoluteResourceLocation.TPC0204:
				case AbsoluteResourceLocation.TPC0304:
				case AbsoluteResourceLocation.TPC0404:
                    data = quadrantString + currentQuadrantCustomNames.magneticParticleVial;
					if (data == quadrantString)
					{
						data += SeparatorResourceManager.GetSeparatorString(StringId.VialA);
					}
					break;
				case AbsoluteResourceLocation.TPC0105:
				case AbsoluteResourceLocation.TPC0205:
				case AbsoluteResourceLocation.TPC0305:
				case AbsoluteResourceLocation.TPC0405:
                    data = quadrantString + currentQuadrantCustomNames.antibodyCocktailVial;
					if (data == quadrantString)
					{
						data += SeparatorResourceManager.GetSeparatorString(StringId.VialC);
					}
					break;
				case AbsoluteResourceLocation.TPC0106:
				case AbsoluteResourceLocation.TPC0206:
				case AbsoluteResourceLocation.TPC0306:
				case AbsoluteResourceLocation.TPC0406:
                    data = quadrantString + currentQuadrantCustomNames.sampleTube;
					if (data == quadrantString)
					{
						data += SeparatorResourceManager.GetSeparatorString(StringId.SampleTube);
					}
					break;
				case AbsoluteResourceLocation.TPC0107:
				case AbsoluteResourceLocation.TPC0207:
				case AbsoluteResourceLocation.TPC0307:
				case AbsoluteResourceLocation.TPC0407:
                    data = quadrantString + currentQuadrantCustomNames.separationTube;
					if (data == quadrantString)
					{
						data += SeparatorResourceManager.GetSeparatorString(StringId.SeparationTube);
					}
					break;
				/*case AbsoluteResourceLocation.TPC0108:
					data = "Q1, " + SeparatorResourceManager.GetSeparatorString(StringId.VialBpos);
					break;
				case AbsoluteResourceLocation.TPC0109:
					data = "Q1, " + SeparatorResourceManager.GetSeparatorString(StringId.VialBneg);
					break;
				case AbsoluteResourceLocation.TPC0110:
					data = "Q1, " + SeparatorResourceManager.GetSeparatorString(StringId.NegativeFractionTube);
					break;*/

                case AbsoluteResourceLocation.TPC0034:
                    data = currentQuadrantCustomNames.bufferBottle34;
                    if (data == "" || data == null)
                        data = SeparatorResourceManager.GetSeparatorString(StringId.QuadrantBuffer34);
                    break;
                case AbsoluteResourceLocation.TPC0056:
                    data = currentQuadrantCustomNames.bufferBottle56;
                    if (data == "" || data == null)
                        data = SeparatorResourceManager.GetSeparatorString(StringId.QuadrantBuffer56);
                    break;

				

				default:
					data = null;
					break;
			}
		}

		private void DrawTableHeaderRow(Graphics g, string[] info, int y, int h, Font font)
		{
			/*moving left margin*/
			int movingLeft = leftMargin-20;

			/*Specialized Format*/
			dataFormat.Alignment = StringAlignment.Center;

			/*get the drawing page width*/
			int pageWidth = printDoc.DefaultPageSettings.Bounds.Width - (leftMargin*2);

			//width of columns
			int[] w = {(int)(pageWidth*0.038),(int)(pageWidth*0.099),(int)(pageWidth*0.135),(int)(pageWidth*0.075),
						  (int)(pageWidth*0.080),(int)(pageWidth*0.138),(int)(pageWidth*0.138),
						  (int)(pageWidth*0.080),(int)(pageWidth*0.085),(int)(pageWidth*0.075),(int)(pageWidth*0.057)};//IAT

			for (int j = 0; j < 11; j++)//IAT
			{
				if (info[j] == null)//no info
				{
					g.FillRectangle(Brushes.LightGray,movingLeft,y,w[j],h);
					g.DrawRectangle(myPen,movingLeft,y,w[j],h);
				}
				else
				{
					g.DrawRectangle(myPen,movingLeft,y,w[j],h);
					g.DrawString(info[j],font,Brushes.Black,new Rectangle(movingLeft,y+2,w[j],h),dataFormat);
				}

				//increment left margin
				movingLeft += w[j];
			}
		}

		private void DrawTableRow(Graphics g, string[] info, int y, int h, Font font)
		{
			/*moving left margin*/
			int movingLeft = leftMargin-20;

			/*Specialized Format*/
			dataFormat.Alignment = StringAlignment.Center;

			/*get the drawing page width*/
			int pageWidth = printDoc.DefaultPageSettings.Bounds.Width - (leftMargin*2);

			//width of columns
			int[] w = {(int)(pageWidth*0.038),(int)(pageWidth*0.099),(int)(pageWidth*0.135),(int)(pageWidth*0.075),
						  (int)(pageWidth*0.080),(int)(pageWidth*0.138),(int)(pageWidth*0.138),
						  (int)(pageWidth*0.080),(int)(pageWidth*0.085),(int)(pageWidth*0.075),(int)(pageWidth*0.057)};//IAT

			for (int j = 0; j < 11; j++)
			{
				if (info[j] == null)//no info
				{
					g.FillRectangle(Brushes.LightGray,movingLeft,y,w[j],h);
					g.DrawRectangle(myPen,movingLeft,y,w[j],h);
				}
				else if ((j == 2) || (j == 5) || (j == 6))
				{
					if(gray[j])
						g.FillRectangle(Brushes.LightGray,movingLeft,y,w[j],h);
					g.DrawRectangle(myPen,movingLeft,y,w[j],h);
					g.DrawString(info[j],smallDataFont,Brushes.Black,new Rectangle(movingLeft,y+2,w[j],h),dataFormat);
				}
				else
				{
					if(gray[j])
						g.FillRectangle(Brushes.LightGray,movingLeft,y,w[j],h);
					g.DrawRectangle(myPen,movingLeft,y,w[j],h);
					g.DrawString(info[j],font,Brushes.Black,new Rectangle(movingLeft,y+2,w[j],h),dataFormat);
				}

				//increment left margin
				movingLeft += w[j];
			}
		}


		#endregion

		#region Command Specific Data Retrieval

		private void FlushData(out int need, out int height, int i)
		{
			need = height = 0;
			//extra data
			ProtocolFlushCommand flushCommand = (ProtocolFlushCommand)commandInfo[i];
			GetTranslatedVial(out data[5],(AbsoluteResourceLocation)flushCommand.SourceVial,pc);
			//GetTranslatedVial(out data[6],(AbsoluteResourceLocation)flushCommand.DestinationVial,pc);
			data[6]="Current Waste Tube";


			gray[5]=gray[6]=true;

			//data that doesnt exist
			data[3] = data[7] = data[8] = data[9] = data[10] = null;
			//change Command string
			data[1] = "Flush";
			//determine total rows needed and height of row
			need = data[2].Length/15+1;
			if (need > 3) need = 3;//max of 3 rows
			if (need == 1) need = 2;//at least 2
			height = field*need;
		}
		private void PrimeData(out int need, out int height, int i)
		{
			need = height = 0;
			//extra data
			ProtocolPrimeCommand primeCommand = (ProtocolPrimeCommand)commandInfo[i];
			GetTranslatedVial(out data[5],(AbsoluteResourceLocation)primeCommand.SourceVial,pc);
			//GetTranslatedVial(out data[6],(AbsoluteResourceLocation)primeCommand.DestinationVial,pc);
			data[6]="Current Waste Tube";

			gray[5]=gray[6]=true;

			//data that doesnt exist
			data[3] = data[7] = data[8] = data[9] = data[10] = null; 
			//change Command string
			data[1] = "Prime";
			//determine total rows needed and height of row
			need = data[2].Length/15+1;
			if (need > 3) need = 3;//max of 3 rows
			if (need == 1) need = 2;//at least 2
			height = field*need;
		}

		private void HomeAllData(out int need, out int height, int i)
		{
			need = height = 0;
			//data that doesnt exist
			data[3] = data[5] = data[6] = data[7] = data[8] = data[9] = data[10] = null;
			//change Command string
			data[1] = "Home All";
			//determine total rows needed and height of row
			need = data[2].Length/15+1;
			if (need > 3) need = 3;//max of 3 rows
			height = field*need;
		}
		
		private void DemoData(out int need, out int height, int i)
		{
			 need = height = 0;
			 //data that doesnt exist
			 data[3] = data[5] = data[6] = data[7] = data[8] = data[9] = data[10] = null;
			 //change Command string
			 data[1] = "Demo";
			 //determine total rows needed and height of row
			 need = data[2].Length/15+1;
			 if (need > 3) need = 3;//max of 3 rows
			 height = field*need;
		}
		
		private void PumpLifeData(out int need, out int height, int i)
		 {
			 need = height = 0;
			 //data that doesnt exist
			 data[3] = data[5] = data[6] = data[7] = data[8] = data[9] = data[10] = null;
			 //change Command string
			 data[1] = "PumpLife";
			 //determine total rows needed and height of row
			 need = data[2].Length/15+1;
			 if (need > 3) need = 3;//max of 3 rows
			 height = field*need;
		 }

		private void IncubateData(out int need, out int height, int i)
		{
			need = height = 0;
			//extra data
			ProtocolIncubateCommand incubateCommand = (ProtocolIncubateCommand)commandInfo[i];
			data[3] = incubateCommand.WaitCommandTimeDuration+"";
			//data that doesnt exist
			data[5] = data[6] = data[7] = data[8] = data[9] = data[10] = null;
			//change Command string
			data[1] = "Incubate";
			//determine total rows needed and height of row
			need = data[2].Length/15+1;
			if (need > 3) need = 3;//max of 3 rows
			height = field*need;
		}

		private void SeparateData(out int need, out int height, int i)//IAT
		{
            ProtocolSeparateCommand cmd = (ProtocolSeparateCommand)commandInfo[i];
            SeparateDataEx(out need, out height, cmd.WaitCommandTimeDuration);
        }
        private void SeparateDataEx(out int need, out int height,uint waitTime)
        {
            ResetToNoGray();
			need = height = 0;
			//extra data
            data[3] = waitTime + "";
			//data that doesnt exist
			data[5] = data[6] = data[7] = data[8] = data[9] = data[10] = null;
			//change Command string
			data[1] = "Separate";//IAT
			//determine total rows needed and height of row
			need = data[2].Length/15+1;
			if (need > 3) need = 3;//max of 3 rows
			height = field*need;
		}

		private void MixData(out int need, out int height, int i)
		{
            ProtocolMixCommand cmd = (ProtocolMixCommand)commandInfo[i];
            MixDataEx(out need, out height, (AbsoluteResourceLocation)cmd.SourceVial, (AbsoluteResourceLocation)cmd.DestinationVial,
            cmd.TipRackSpecified, cmd.TipRack,
            cmd.VolumeTypeSpecifier, cmd.AbsoluteVolume_uL, cmd.RelativeVolumeProportion, cmd.MixCycles, cmd.TipTubeBottomGap_uL);
        }
        private void MixDataEx(out int need, out int height,
            AbsoluteResourceLocation src, AbsoluteResourceLocation dst, bool boTipRackSpecified,
            int iTipRack, VolumeType volType, int absVol, double relProp, int mixCycles, int tipTubeBottomGap)
        {
            ResetToNoGray();
			need = height = 0;
			//extra data
			GetTranslatedVial(out data[5],src,pc);
            if (boTipRackSpecified)
                data[10] = iTipRack + "";
			else data[10] = "default";
			//data that doesnt exist (time, dest) - if null will be grayed out
			data[3] = data[6] = null;
            data[7] = volType + "";
			if (data[7] == VolumeType.Absolute+"")
			{
				data[8] = mixCycles+" * "+absVol+"";
				data[9] = tipTubeBottomGap+"";
			}
			else if (data[7] == VolumeType.Relative+"")
			{
				data[8] = mixCycles+" * "+relProp+"";
				data[9] = null;
			}
			else
			{
				data[7] = VolumeType.Relative+"";
				data[8] ="3 * 0.5";
				data[9] = null;
			}
			//change Command string
			data[1] = "Mix";
			//determine total rows needed and height of row
			need = data[2].Length/15+1;
			if (need > 3) need = 3;//max of 3 rows
			if (need == 1) need = 2;//at least 2
			height = field*need;
		}

		private void ResuspendVialData(out int need, out int height, int i)
        {
            ProtocolResuspendVialCommand cmd = (ProtocolResuspendVialCommand)commandInfo[i];
            ResuspendVialDataEx(out need, out height, (AbsoluteResourceLocation)cmd.SourceVial, (AbsoluteResourceLocation)cmd.DestinationVial,
            cmd.FreeAirDispense, cmd.TipRackSpecified, cmd.TipRack,
            cmd.VolumeTypeSpecifier, cmd.AbsoluteVolume_uL, cmd.RelativeVolumeProportion);
        }
        private void ResuspendVialDataEx(out int need, out int height,
            AbsoluteResourceLocation src, AbsoluteResourceLocation dst, bool boFreeAirDispense, bool boTipRackSpecified,
            int iTipRack, VolumeType volType, int absVol, double relProp)
        {
            ResetToNoGray();
			need = height = 0;
			//extra data
			GetTranslatedVial(out data[5],src,pc);
			GetTranslatedVial(out data[6],dst,pc);
			
			gray[5]=true;
			
			data[9] = boFreeAirDispense+"";
            if (boTipRackSpecified)
                data[10] = iTipRack + "";
			else data[10] = "default";
			//data that doesnt exist
			data[3] = data[8] = null;

            data[7] = volType + "";
			if(data[7] != VolumeType.Relative+"")
			{
				data[7] = "Threshold";
			}
			else
			{
                data[7] = volType + ""; //which is relative
                data[8] = relProp + "";
			}

			//change Command string
			data[1] = "Resuspend Vial";
			//determine total rows needed and height of row
			need = data[2].Length/15+1;
			if (need > 3) need = 3;//max of 3 rows
			if (need == 1) need = 2;//at least 2 rows
			height = field*need;
		}

		private void TransportData(out int need, out int height, int i)
        {
            ProtocolTransportCommand cmd = (ProtocolTransportCommand)commandInfo[i];
            TransportDataEx(out need, out height, (AbsoluteResourceLocation)cmd.SourceVial, (AbsoluteResourceLocation)cmd.DestinationVial,
            cmd.FreeAirDispense,cmd.TipRackSpecified, cmd.TipRack, cmd.UseBufferTip,
            cmd.VolumeTypeSpecifier, cmd.AbsoluteVolume_uL, cmd.RelativeVolumeProportion);
        }
        private void TransportDataEx(out int need, out int height,
            AbsoluteResourceLocation src, AbsoluteResourceLocation dst, bool boFreeAirDispense, bool boTipRackSpecified,
            int iTipRack, bool boUseBufferTip, VolumeType volType, int absVol, double relProp)
        {
            ResetToNoGray();
			need = height = 0;
			//extra data
			GetTranslatedVial(out data[5],src,pc);
			GetTranslatedVial(out data[6],dst,pc);
            data[9] = boFreeAirDispense + "";
            data[7] = volType + "";
			if (data[7] == VolumeType.Absolute+"")
				data[8] = absVol+"";
			else if (data[7] == VolumeType.Relative+"")
				data[8] = relProp+"";
			else
			{
				data[7] = data[8] = null;
			}
			if (boUseBufferTip)
				data[8] += "\nBuffer Tip";
			if (boTipRackSpecified)
				data[10] = iTipRack+"";
			else data[10] = "default";
			//data that doesnt exist
			data[3] = null;
			//change Command string
			data[1] = "Transport";
			//determine total rows needed and height of row
			need = data[2].Length/15+1;
			if (need > 3) need = 3;//max of 3 rows
			if (need == 1) need = 2;//at least 2
			height = field*need;
		}

		private void TopUpVialData(out int need, out int height, int i)
		{
			ProtocolTopUpVialCommand protocolTopUpVialCommand = (ProtocolTopUpVialCommand)commandInfo[i];
            TopUpVialDataEx(out need, out height,(AbsoluteResourceLocation)protocolTopUpVialCommand.SourceVial,(AbsoluteResourceLocation)protocolTopUpVialCommand.DestinationVial,
                protocolTopUpVialCommand.FreeAirDispense,protocolTopUpVialCommand.TipRackSpecified,protocolTopUpVialCommand.TipRack,
                protocolTopUpVialCommand.VolumeTypeSpecifier,protocolTopUpVialCommand.RelativeVolumeProportion);
        }
        private void TopUpVialDataEx(out int need, out int height,
            AbsoluteResourceLocation src, AbsoluteResourceLocation dst, bool boFreeAirDispense, bool boTipRackSpecified,
            int iTipRack, VolumeType volType, double fRelativeVolumeProportion)
        {
            ResetToNoGray();
			need = height = 0;
			//extra data
			GetTranslatedVial(out data[5],src,pc);
			GetTranslatedVial(out data[6],dst,pc);
			gray[5]=true;
            data[9] = boFreeAirDispense + "";
			if (boTipRackSpecified)
                data[10] = iTipRack + "";
			else data[10] = "default";
			//data that doesnt exist
			data[3] = data[7] = data[8] = null;

            data[7] = volType + "";
			if(data[7] != VolumeType.Relative+"")
			{
				data[7] = "Threshold";
			}
			else
			{
                data[7] = volType + ""; //which is relative
                data[8] = fRelativeVolumeProportion + "";
			}

			//change Command string
			data[1] = "Top Up Vial";
			//determine total rows needed and height of row
			need = data[2].Length/15+1;
			if (need > 3) need = 3;//max of 3 rows
			if (need == 1) need = 2;//at least 2 rows
			height = field*need;
		}

		private void PauseData(out int need, out int height, int i)//IAT
		{
			need = height = 0;
			//extra data
			ProtocolPauseCommand pauseCommand = (ProtocolPauseCommand)commandInfo[i];
			data[3] = data[4] = null;
			//data that doesnt exist
			data[5] = data[6] = data[7] = data[8] = data[9] = data[10] = null;
			//change Command string
			data[1] = "Pause";//IAT
			//determine total rows needed and height of row
			need = data[2].Length/15+1;
			if (need > 3) need = 3;//max of 3 rows
			height = field*need;
		}

		#endregion

		#region Data Access

		public int pageNumber
		{
			get
			{
				return pageNum;
			}
			set
			{
				pageNum = 0;
			}
		}

		#endregion
	
	}
}
