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

using System.IO;
using System.Xml;

using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;


namespace Report_Crystal
{
    public partial class Form1 : Form
    {
        private string sXmlFileName = "TempData.xml";
        private string sEORFileName = "EndOfRunReport.rpt";
        private string sMsgExportSuccessfully = "The report '{0}' has been created successfully in the currently directory '{1}'.";
        private string sMsgExportFailed = "Failed to create the report '{0}'.";
        private string sMsgFileExisted = "The file '{0}' is already existed in the currently directory '{1}'. \n\n Overwrite file?";
        private string sCaptionFileExisted = "File Already Existed";

        EndOfRunReportHelper helper = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string CurrentPath = Directory.GetCurrentDirectory();
            int lastBackslashIndex = CurrentPath.LastIndexOf('\\');
            if (lastBackslashIndex+1 != CurrentPath.Length)
            {
                CurrentPath += "\\";
            }
            textBox1.Text = CurrentPath + sXmlFileName;
            textBox2.Text = CurrentPath + sEORFileName;
        }

        private void displayReport()
        {
            if (helper == null)
                helper = new EndOfRunReportHelper();

            string Path1 = @"..\..\TempData.XML";
            string Path2 = @"..\..\EndOfRunReport.rpt";

            string Temp1 = textBox1.Text.Trim();
            string Temp2 = textBox2.Text.Trim();
            if (!string.IsNullOrEmpty(Temp1) && !string.IsNullOrEmpty(Temp2))
            {
                Path1 = Temp1;
                Path2 = Temp2;
            }

            try
            {
                helper.FormatReport(Path1, Path2);
                crystalReportViewer1.ReportSource = helper.ReportDoc;
                crystalReportViewer1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void exportReportInPdf()
        {
            if (helper == null)
                helper = new EndOfRunReportHelper();

            string Path1 = @"..\..\TempData.XML";
            string Path2 = @"..\..\EndOfRunReport.rpt";
            string Path3 = @"..\..\TempData.pdf";

            string Temp1 = textBox1.Text.Trim();
            string Temp2 = textBox2.Text.Trim();
            string Temp3 = String.Empty;
            FileInfo fInfo = null;
            try
            {
                if (!string.IsNullOrEmpty(Temp1) && !string.IsNullOrEmpty(Temp2))
                {
                    Path1 = Temp1;
                    Path2 = Temp2;
                    fInfo = new FileInfo(Path1);
                    if (!string.IsNullOrEmpty(fInfo.Name))
                    {

                        Temp3 = fInfo.Name.Replace(fInfo.Extension, ".pdf");
                        Path3 = fInfo.DirectoryName;
                        int lastBackslashIndex = Path3.LastIndexOf('\\');
                        if (lastBackslashIndex + 1 != Path3.Length)
                        {
                            Path3 += "\\";
                        }
                        Path3 += Temp3;
                    }
                }

                if (File.Exists(Path3))
                {
                    Temp2 = String.Format(sMsgFileExisted, Temp3, fInfo.DirectoryName);
                    DialogResult dlg = MessageBox.Show(this, Temp2, sCaptionFileExisted, MessageBoxButtons.YesNo);
                    if (dlg != System.Windows.Forms.DialogResult.Yes)
                    {
                        return;
                    }

                    File.Delete(Path3);
                }
                helper.ExportReport(Path1, Path2, Path3);
                if (File.Exists(Path3))
                {
                    Temp2 = String.Format(sMsgExportSuccessfully, Temp3, fInfo.DirectoryName);
                    MessageBox.Show(Temp2);
                    File.Delete(Path3);
                }
                else
                {
                    Temp2 = String.Format(sMsgExportFailed, Temp3);
                    MessageBox.Show(Temp2);
                }

                // crystalReportViewer1.ReportSource = helper.ReportDoc;
                // crystalReportViewer1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            displayReport();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            exportReportInPdf();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
  
}
