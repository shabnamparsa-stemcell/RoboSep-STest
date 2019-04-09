using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Xml.Linq;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml;

using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace Report_Crystal
{
    public class EndOfRunReportHelper
    {
        private const string TableNameRunSummary = "RunSummary";
        private const string TableNameRunDetails = "RunDetails";
        private const int defaultHeight = 228;

        private string[,] RunDetailsDisplayElementNames = new string[,]
        {
            {"operatorID", "FO_RD_OperatorID1"},
            {"operatorID", "FO_RD_OperatorID2"}
        };
        private string[,] RunSummaryDisplayElementNames = new string[,]
        {
            {"sampleNo", "TB_RS_SampleNumber", "FO_RS_SampleNumber"},
            {"sampleID", "TB_RS_SampleID", "FO_RS_SampleID"},
            {"protocol", "TB_RS_Protocol", "FO_RS_Protocol"},
            {"sampleVolume_ML", "TB_RS_SampleVol_mL", "FO_RS_SampleVol_mL"},
            {"magneticParticlesLotID", "TB_RS_MagneticParticles", "FO_RS_MagneticParticles"},
            {"selectionCocktailLotID", "TB_RS_Selection_Cocktail", "FO_RS_Selection_Cocktail"},
            {"antibodyCocktailLotID", "TB_RS_Antibody_Cocktail", "FO_RS_Antibody_Cocktail"},
            {"tipRack", "TB_RS_TipRack", "FO_RS_TipRack"},
            {"bufferLotID", "TB_RS_BufferLotID", "FO_RS_BufferLotID"},
            {"sampleTube", "TB_RS_SampleTube", "FO_RS_SampleTube"},
            {"separationTube", "TB_RS_SeparationTube", "FO_RS_SeparationTube"},
            {"negativeFractionTube", "TB_RS_NegativeFractionTube", "FO_RS_NegativeFractionTube"},
            {"wasteTube", "TB_RS_WasteTube", "FO_RS_WasteTube"}
        };
        // 
        private List<DisplayElement> lstRunSummaryDisplayElements = new List<DisplayElement>();
        private List<DisplayElement2> lstRunDetailsDisplayElements = new List<DisplayElement2>();
        private Dictionary<int, string> dictStringmax = new Dictionary<int, string>();
        private int conversionFactor = 15;
        private ReportDocument cryRptDoc = null;

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int GetDeviceCaps(IntPtr hDC, int nIndex);

        public enum DeviceCap
        {
            LOGPIXELSX = 88, // Logical pixels inch in X
            LOGPIXELSY = 90  // Logical pixels inch in Y
        }

        public EndOfRunReportHelper()
        {
            InitializeDisplayElements();

            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            int Ydpi = GetDeviceCaps(desktop, (int)DeviceCap.LOGPIXELSY);
            conversionFactor = (int)(1440.0f / Ydpi);
            g.Dispose();
        }

        private void InitializeDisplayElements()
        {
            string[] aNames = null;
            string columnName;
            lstRunSummaryDisplayElements.Clear();

            for (int i = 0; i < RunSummaryDisplayElementNames.GetLength(0); i++)
            {
                int nSize = RunSummaryDisplayElementNames.GetLength(1);
                if (nSize < 2)
                    continue;

                aNames = new string[nSize - 1];
                columnName = RunSummaryDisplayElementNames[i, 0];
                for (int j = 0; j < nSize -1; j++)
                {
                    aNames[j] = String.Empty;
                    aNames[j] = RunSummaryDisplayElementNames[i, j + 1];
                }

                DisplayElement d = new DisplayElement(columnName, aNames);
                lstRunSummaryDisplayElements.Add(d);
            }

            string tableFieldName, displayFieldName;
            lstRunDetailsDisplayElements.Clear();
            for (int i = 0; i < RunDetailsDisplayElementNames.GetLength(0); i++)
            {
                int nSize = RunDetailsDisplayElementNames.GetLength(1);
                if (nSize < 1)
                    continue;
                tableFieldName = RunDetailsDisplayElementNames[i, 0];
                displayFieldName = RunDetailsDisplayElementNames[i, 1];

                if (string.IsNullOrEmpty(tableFieldName) || string.IsNullOrEmpty(displayFieldName))
                    continue;

                DisplayElement2 d = new DisplayElement2(tableFieldName, displayFieldName);
                lstRunDetailsDisplayElements.Add(d);
            }
        }

        private void ClearDisplayElementData()
        {
            if (lstRunSummaryDisplayElements != null)
            {
                foreach (DisplayElement de in lstRunSummaryDisplayElements)
                {
                    if (de == null)
                        continue;

                    de.ResetData();
                }
            }
            if (lstRunDetailsDisplayElements != null)
            {
                foreach (DisplayElement2 de2 in lstRunDetailsDisplayElements)
                {
                    if (de2 == null)
                        continue;

                    de2.ResetData();
                }
            }
        }

        private void GetTableData(RunDataSet1 dsTempReport)
        {
            if (dsTempReport == null)
                return;

            ClearDisplayElementData();

            for (int i = 0; i < dsTempReport.Tables.Count; i++)
            {
                DataTable dtable = dsTempReport.Tables[i];
                if (dtable.TableName == TableNameRunDetails)
                {
                    for (int j = 0; j < dtable.Rows.Count; j++)
                    {
                        DataRow objDataRow = dtable.Rows[j];
                        if (objDataRow == null)
                            continue;

                        int nIndex = 0;
                        foreach (DataColumn objDataColumn in dtable.Columns)
                        {
                            System.Diagnostics.Debug.WriteLine(String.Format("Column Name = {0}, Value = {1}", objDataColumn.ColumnName, objDataRow[objDataColumn]));

                            if ((objDataRow[objDataColumn] != null || objDataRow[objDataColumn] != System.DBNull.Value) && objDataRow[objDataColumn].GetType() == (typeof(string)))
                            {
                                string sData = (string)objDataRow[objDataColumn];
                                if (string.IsNullOrEmpty(sData))
                                    continue;

                                nIndex = 0;
                                do
                                {
                                    nIndex = lstRunDetailsDisplayElements.FindIndex(nIndex, x => { return (!string.IsNullOrEmpty(x.TableFieldName) && x.TableFieldName.ToLower() == objDataColumn.ColumnName.ToLower()); });
                                    if (0 <= nIndex)
                                    {
                                        if (string.IsNullOrEmpty(lstRunDetailsDisplayElements[nIndex].Name))
                                        {
                                            lstRunDetailsDisplayElements[nIndex].Name = sData;
                                        }
                                        else
                                        {
                                            // assume longer strings occupy more spaces.
                                            if (lstRunDetailsDisplayElements[nIndex].Name.Length < sData.Length)
                                                lstRunDetailsDisplayElements[nIndex].Name = sData;
                                        }
                                        nIndex++;
                                    }
                                } while (0 <= nIndex);
                            }
                        }
                    }
                }

                if (dtable.TableName == TableNameRunSummary)
                {
                    for (int j = 0; j < dtable.Rows.Count; j++)
                    {
                        DataRow objDataRow = dtable.Rows[j];
                        if (objDataRow == null)
                            continue;

                        int nIndex = 0;
                        foreach (DataColumn objDataColumn in dtable.Columns)
                        {
                            if ((objDataRow[objDataColumn] != null || objDataRow[objDataColumn] != System.DBNull.Value) && objDataRow[objDataColumn].GetType() == (typeof(string)))
                            {
                                string sData = (string)objDataRow[objDataColumn];
                                if (string.IsNullOrEmpty(sData))
                                    continue;

                                nIndex = lstRunSummaryDisplayElements.FindIndex(x => { return (!string.IsNullOrEmpty(x.ColumnName) && x.ColumnName.ToLower() == objDataColumn.ColumnName.ToLower()); });
                                if (0 <= nIndex)
                                {
                                    lstRunSummaryDisplayElements[nIndex].ListData.Add(sData);
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool FormatReport(string fullPathXmlFileName, string fullPathReportGeneratorFileName)
        {
            if (string.IsNullOrEmpty(fullPathXmlFileName) || string.IsNullOrEmpty(fullPathReportGeneratorFileName))
                return false;

            // check for existences of files
            if (!File.Exists(fullPathXmlFileName))
                return false;

            if (!File.Exists(fullPathReportGeneratorFileName))
                return false;

            if (cryRptDoc == null)
            {
                cryRptDoc = new ReportDocument();
                if (cryRptDoc == null)
                    return false;
            }

            // create temp dataset to read xml information
            RunDataSet1 dsTempReport = new RunDataSet1();

            try
            {
                // using ReadXml method of DataSet read XML data from books.xml file
                dsTempReport.ReadXml(fullPathXmlFileName, XmlReadMode.IgnoreSchema);

                GetTableData(dsTempReport);

                cryRptDoc.Load(fullPathReportGeneratorFileName);
                cryRptDoc.SetDataSource(dsTempReport);
                AdjustLayout(cryRptDoc);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }

        public void ExportReport(string fullPathXmlFileName, string fullPathReportGeneratorFileName, string fullPathEndOfRunReportPdfFileName)
        {
            if (string.IsNullOrEmpty(fullPathXmlFileName) || string.IsNullOrEmpty(fullPathReportGeneratorFileName) || string.IsNullOrEmpty(fullPathEndOfRunReportPdfFileName))
                return;

            try
            {
                if (FormatReport(fullPathXmlFileName, fullPathReportGeneratorFileName) == false)
                {
                    return;
                }

                // Declare variables and get the export options.
                ExportOptions exportOpts = new ExportOptions();
                PdfFormatOptions pdfFormatOpts = new PdfFormatOptions();
                DiskFileDestinationOptions diskOpts = new DiskFileDestinationOptions();
                exportOpts = cryRptDoc.ExportOptions;

                // Set the pdf format options.
                exportOpts.ExportFormatType = ExportFormatType.PortableDocFormat;
                exportOpts.FormatOptions = pdfFormatOpts;

                // Set the disk file options and export.
                exportOpts.ExportDestinationType = ExportDestinationType.DiskFile;
                diskOpts.DiskFileName = fullPathEndOfRunReportPdfFileName;
                exportOpts.DestinationOptions = diskOpts;
                cryRptDoc.Export();

                // Close the report
                cryRptDoc.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public ReportDocument ReportDoc
        {
            get { return cryRptDoc; }
        }

        private void AdjustLayout(ReportDocument icryRptDoc)
        {
            if (icryRptDoc == null)
                return;

            bool bAdjust1 = false, bAdjust2 = false, bAdjust3 = false;
            Font theFont;
            int refWidth = 0;

            for (int i = 0; i < icryRptDoc.ReportDefinition.Sections.Count; i++)
            {
                ReportObjects robjs = icryRptDoc.ReportDefinition.ReportObjects;
                if (robjs == null || robjs.Count == 0)
                    continue;

                for (int j = 0; j < robjs.Count; j++)
                {
                    ReportObject robj = robjs[j];
                    if (robj == null)
                        continue;

                    ReportObjectKind robkKind = robj.Kind;
                    if (robkKind == ReportObjectKind.FieldObject && !string.IsNullOrEmpty(robj.Name))
                    {
                        FieldObject fieldObj = (FieldObject)robj;
                        if (fieldObj == null)
                            continue;

                        int nIndex = lstRunDetailsDisplayElements.FindIndex(x =>
                        {
                            if ((!string.IsNullOrEmpty(x.DisplayFieldName) && x.DisplayFieldName == fieldObj.Name))
                            {
                                return true;
                            }
                            return false;
                        });

                        if (0 <= nIndex)
                        {
                            DisplayElement2 d2 = lstRunDetailsDisplayElements[nIndex];
                            if (d2 != null && !string.IsNullOrEmpty(d2.Name))
                            {
                                float fontSize = d2.EstimateFontSize(d2.Name, new Size(fieldObj.Width / conversionFactor, fieldObj.Height / conversionFactor), fieldObj.Font);
                                if (fontSize > 0.0f && fontSize < fieldObj.Font.Size)
                                {
                                    theFont = new Font(fieldObj.Font.Name, fontSize, fieldObj.Font.Style, fieldObj.Font.Unit);
                                    fieldObj.ApplyFont(theFont);
                                    theFont.Dispose();
                                }
                            }
                            d2.FontSizeAdjusted = true;

                            bool bAllAdjusted = true;
                            foreach (DisplayElement2 de in lstRunDetailsDisplayElements)
                            {
                                if (de == null)
                                    continue;

                                bAllAdjusted &= de.FontSizeAdjusted;
                            }

                            bAdjust1 = bAllAdjusted;
                        }
                    }

                    if (robkKind == ReportObjectKind.TextObject && !string.IsNullOrEmpty(robj.Name))
                    {
                        TextObject textObj = (TextObject)robj;
                        if (textObj == null)
                            continue;

                        if ((!string.IsNullOrEmpty(textObj.Name) && textObj.Name == "TB_PageFooterCopyright"))
                        {
                            theFont = new Font(textObj.Font.Name, 6.0f, textObj.Font.Style, textObj.Font.Unit);
                            textObj.ApplyFont(theFont);
                            bAdjust2 = true;
                            theFont.Dispose();
                        }
                    }

                    if (robkKind == ReportObjectKind.SubreportObject && !string.IsNullOrEmpty(robj.Name) && robj.Name == "Subreport_RunSummary")
                    {
                        SubreportObject subReportObj = (SubreportObject)robj;
                        ReportDocument subReportDoc = subReportObj.OpenSubreport(subReportObj.SubreportName);

                        if (subReportDoc == null)
                            break;

                        ReportObjects subrobjs = subReportDoc.ReportDefinition.ReportObjects;
                        if (subrobjs == null || subrobjs.Count == 0)
                            continue;

                        // Estimate heights
                        for (int m = 0; m < subrobjs.Count; m++)
                        {
                            ReportObject subrobj = subrobjs[m];
                            if (subrobj == null)
                                continue;

                            Font font;
                            switch (subrobj.Name)
                            {
                                case "FO_RS_SampleNumber":
                                case "FO_RS_SampleID":
                                case "FO_RS_Protocol":
                                case "FO_RS_SampleVol_mL":
                                case "FO_RS_MagneticParticles":
                                case "FO_RS_Selection_Cocktail":
                                case "FO_RS_Antibody_Cocktail":
                                case "FO_RS_TipRack":
                                case "FO_RS_BufferLotID":
                                case "FO_RS_SampleTube":
                                case "FO_RS_SeparationTube":
                                case "FO_RS_NegativeFractionTube":
                                case "FO_RS_WasteTube":
                                    FieldObject fb = (FieldObject)subrobj;
                                    font = fb.Font;
                                    EstimateHeight(subrobj, font, subrobj.Name);
                                    break;
                            }
                        }

                        // Set heights of cells
                        for (int n = 0; n < subrobjs.Count; n++)
                        {
                            ReportObject subrobj = subrobjs[n];
                            if (subrobj == null)
                                continue;

                            switch (subrobj.Name)
                            {
                                case "TB_RS_SampleNumber":
                                case "TB_RS_SampleID":
                                case "TB_RS_Protocol":
                                case "TB_RS_SampleVol_mL":
                                case "TB_RS_MagneticParticles":
                                case "TB_RS_Selection_Cocktail":
                                case "TB_RS_Antibody_Cocktail":
                                case "TB_RS_TipRack":
                                case "TB_RS_BufferLotID":
                                case "TB_RS_SampleTube":
                                case "TB_RS_SeparationTube":
                                case "TB_RS_NegativeFractionTube":
                                case "TB_RS_WasteTube":

                                    SetHeight(subrobj, subrobj.Name);
                                    break;
                            }
                        } // end for
                        for (int n = 0; n < subrobjs.Count; n++)
                        {
                            ReportObject subrobj = subrobjs[n];
                            if (subrobj == null)
                                continue;

                            switch (subrobj.Name)
                            {
                                case "TB_RS_SampleVol_mL":
                                    refWidth = subrobj.Width;
                                    break;
                                case "TB_RS_MagneticParticles":
                                case "TB_RS_Selection_Cocktail":
                                case "TB_RS_Antibody_Cocktail":
                                    subrobj.Width = refWidth;
                                    break;
                            }
                        } // end for

                        bAdjust3 = true; 
                    } // end if sub report

                    if (bAdjust1 && bAdjust2 && bAdjust3)
                        break;
                }// end for section object
             }
        }

        private void EstimateHeight(ReportObject robj, Font font, string fieldName)
        {
            if (robj == null || font == null || string.IsNullOrEmpty(fieldName))
                return;

            int nHeight = 0;
            int nIndex = lstRunSummaryDisplayElements.FindIndex(x =>
            {
                foreach (string s in x.Names)
                {
                    if ((!string.IsNullOrEmpty(s) && s == fieldName))
                    {
                        return true;
                    }
                }
                return false;
            });
            if (0 <= nIndex)
            {
                lstRunSummaryDisplayElements[nIndex].EstimateHeightInPixel(font, robj.Width/conversionFactor, out nHeight);
                int nTemp = defaultHeight /conversionFactor;
                if (nHeight < nTemp)
                {
                    nHeight = nTemp;
                    lstRunSummaryDisplayElements[nIndex].HeightInPixel = nHeight;
                }
            }
        }

        private void SetHeight(ReportObject robj, string fieldName)
        {
            if (robj == null || string.IsNullOrEmpty(fieldName))
                return;

            int nIndex = lstRunSummaryDisplayElements.FindIndex(x =>
            {
                foreach (string s in x.Names)
                {
                    if ((!string.IsNullOrEmpty(s) && s == fieldName))
                    {
                        return true;
                    }
                }
                return false;
            });

            int nHeight = 0;
            if (0 <= nIndex)
            {
                DisplayElement de = lstRunSummaryDisplayElements[nIndex];
                if (de.IsHeightEstimated)
                {
                    nHeight = de.HeightInPixel *conversionFactor;
                    if (nHeight < defaultHeight)
                        nHeight = defaultHeight;

                    if (nHeight > robj.Height)
                       robj.Height = nHeight;
                }
            }
        }
    }

    class DisplayElement
    {
        private int bottomMarginInPixel = 3;
        private string[] aNames;
        private List<string> lstData = new List<string>();
        private bool bEstimated = false;

        int height = 0;

        public string ColumnName { get; set; }
        public int HeightInPixel
        {
            get
            {
                if (!bEstimated)
                    return -1;
                return height;
            }

            set
            {
                height = value;
                bEstimated = true;
            }
        }

        public DisplayElement(string columnName, string[] aNames)
        {
            if (aNames == null || string.IsNullOrEmpty(columnName))
                return;

            this.ColumnName = columnName;
            this.aNames = new string[aNames.Length];
            Array.Copy(aNames, this.aNames, aNames.Length);
        }

        public void ResetData()
        {
            lstData.Clear();
            bEstimated = false;
        }

        public bool EstimateHeightInPixel(Font font, int iWidth, out int oHeight)
        {
            oHeight = 0;

            if (bEstimated)
            {
                oHeight = height;
                return true;
            }

            if (font == null)
                return false;

            TextFormatFlags flags = TextFormatFlags.WordBreak | TextFormatFlags.LeftAndRightPadding;

            if (lstData == null || lstData.Count == 0)
                return false;

            Size txtSize = new Size(iWidth, int.MaxValue);
            height = 0;
            string temp;
            for (int i = 0; i < lstData.Count; i++)
            {
                temp = lstData[i];
                if (string.IsNullOrEmpty(temp))
                    continue;

                txtSize = TextRenderer.MeasureText(temp, font, txtSize, flags);
                txtSize.Height += bottomMarginInPixel;
                if (txtSize.Height > height)
                    height = txtSize.Height;

                // reset to initial size
                txtSize.Width = iWidth;
                txtSize.Height = int.MaxValue;
            }

            oHeight = height;
            bEstimated = true;
            return true;
        }

        public string[] Names
        {
            get { return aNames; }
            set
            {
                if (value == null)
                    return;

                if (aNames == null)
                {
                    aNames = new string[value.Length];
                    Array.Copy(aNames, value, value.Length);
                }
            }
        }

        public List<string> ListData
        {
            get { return lstData; }
        }

        public bool IsHeightEstimated
        {
            get { return bEstimated; }
        }
    }

    class DisplayElement2
    {
        private bool bEstimated = false;
        private bool bAdjusted = false;

        float fontSize = -1.0f;

        public string Name { get; set; }
        public string TableFieldName { get; set; }
        public string DisplayFieldName { get; set; }

        public float FontSize 
        {
            get
            {
                if (!bEstimated)
                    return -1.0f;
                return fontSize;
            }

            set
            {
                fontSize = value;
                bEstimated = true;
            }
        }

        public DisplayElement2(string tableFieldName, string displayFieldName)
        {
            if (string.IsNullOrEmpty(tableFieldName) || string.IsNullOrEmpty(displayFieldName))
                return;

            this.TableFieldName = tableFieldName;
            this.DisplayFieldName = displayFieldName;
        }

        public void ResetData()
        {
            Name = String.Empty;
            bEstimated = false;
            bAdjusted = false;
        }

        public float EstimateFontSize(string s, Size fitTo, Font f)
        {
            if (string.IsNullOrEmpty(s) || f == null)
                return -1.0f;

            TextFormatFlags format = TextFormatFlags.SingleLine;
            Size p = new Size(fitTo.Width, int.MaxValue);
            Size textSize = TextRenderer.MeasureText(s, f, p, format);

            if (textSize.Width <= fitTo.Width)
                return f.Size;
            else
            {
                float fontsize = f.Size;
                Font newFont;
                do
                {
                    fontsize -= 0.5f;
                    newFont = new Font(f.FontFamily, fontsize, f.Style);

                    //And check again
                    textSize = TextRenderer.MeasureText(s, newFont, p, format);

                    newFont.Dispose();
                    newFont = null;

                    if (textSize.Width <= fitTo.Width)
                        return fontsize;

                } while (fitTo.Width <= textSize.Width);

                return fontsize;
            }
        }

        public bool IsFontSizeEstimated
        {
            get { return bEstimated; }
        }

        public bool FontSizeAdjusted
        {
            get { return bAdjusted; }
            set { bAdjusted = value; }
        }
    }


}
