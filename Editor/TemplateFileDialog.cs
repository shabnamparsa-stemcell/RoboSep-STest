//----------------------------------------------------------------------------
//
// 2011-09-20 sp various changes
//     - change fixed application path folder to variable obtained during program execution 
//     - remove loading of serial.dat file; set version to default of 0000
//
//----------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;


namespace TemplateFileDialog
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class TemplateDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListBox FileList;
		private System.Windows.Forms.Button btn_OK;
		private System.Windows.Forms.Button btn_Cancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private FileInfo[] fi;

		private string TemplateFileName;
		private string TemplateDirName;
		private int    DisableVersionUpdate = 1;

		public TemplateDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
            // 2011-09-19 sp
            // replace fixed directory path with the installed path
            TemplateDirName = Tesla.Common.Utilities.GetDefaultAppPath() + "config";
            //TemplateDirName = @"C:\Program Files\STI\RoboSep\config";
			TemplateFileName = string.Empty;		
			//
		}
		public string DirName
		{
			set
			{
				TemplateDirName =  Tesla.Common.Utilities.GetDefaultAppPath() + "config\\"+value;
			}
		}
		public string TemplateName
		{
			get
			{
				return TemplateFileName;
			}
		}
		public int DisableAutoVersionUpdater
		{
			get
			{
				return DisableVersionUpdate;
			}
		}
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.FileList = new System.Windows.Forms.ListBox();
			this.btn_OK = new System.Windows.Forms.Button();
			this.btn_Cancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// FileList
			// 
			this.FileList.HorizontalScrollbar = true;
			this.FileList.Location = new System.Drawing.Point(0, 0);
			this.FileList.Name = "FileList";
			this.FileList.Size = new System.Drawing.Size(528, 316);
			this.FileList.TabIndex = 0;
			this.FileList.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FileList_KeyPress);
			// 
			// btn_OK
			// 
			this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btn_OK.Location = new System.Drawing.Point(360, 320);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new System.Drawing.Size(75, 40);
			this.btn_OK.TabIndex = 1;
			this.btn_OK.Text = "OK";
			this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
			// 
			// btn_Cancel
			// 
			this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btn_Cancel.Location = new System.Drawing.Point(448, 320);
			this.btn_Cancel.Name = "btn_Cancel";
			this.btn_Cancel.Size = new System.Drawing.Size(75, 40);
			this.btn_Cancel.TabIndex = 2;
			this.btn_Cancel.Text = "Cancel";
			// 
			// TemplateDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(530, 363);
			this.Controls.Add(this.btn_Cancel);
			this.Controls.Add(this.btn_OK);
			this.Controls.Add(this.FileList);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TemplateDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Templates";
			this.TopMost = true;
			this.Load += new System.EventHandler(this.TemplateDialog_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void TemplateDialog_Load(object sender, System.EventArgs e)
		{
			string JustName;
			
			FileList.Items.Clear();

            try
            {
                DirectoryInfo di = new DirectoryInfo(TemplateDirName);
                fi = di.GetFiles("*.xml");
                foreach (FileInfo fiTemp in fi)
                {
                    JustName = fiTemp.Name.Remove((fiTemp.Name.Length - 4), 4);
                    FileList.Items.Add(JustName);
                }
            }
            catch
            {
            }
		}

		private void btn_OK_Click(object sender, System.EventArgs e)
		{
			string FullName;
			
			if (FileList.Items.Count <= 0)
			{				
				this.DialogResult = DialogResult.Cancel;
				return;
			}

			if (FileList.SelectedIndex == -1)
				FileList.SelectedIndex = 0;

			FullName = fi[FileList.SelectedIndex].FullName;
			TemplateFileName = FullName;				
		}

		private void FileList_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (e.KeyChar == 12)  // 12 -> Left Ctrl + L
			{
				//if (DisableVersionUpdate == 0)
				//{
				//	MessageBox.Show("Auto Version Updater Disabled!");
				//	DisableVersionUpdate = 1;
				//}
				//else
				//{
				//	MessageBox.Show("Auto Version Updater Enabled!");
				//	DisableVersionUpdate = 0;
				//}
			}
		}
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
	}
}

namespace ProtocolVersionManager
{
	public class VersionManager
	{
		private static VersionManager versioncheck = null;
		private readonly string[] mComment = new string[]{"<!-- DO NOT EDIT THIS LINE!! #RoboSep Client:"," #RoboSep Serial:"," #Time:"," //DO NOT EDIT THIS LINE!! -->"};
		private			 int[]  mThisVersions;
		private          int[]  mInputVersions;
		private			 string mRoboSepSerial;
		
		public int[] ThisVersions
		{
			get
			{
				return mThisVersions;
			}
		}

		public int[] InputVersions
		{
			get
			{
				return mInputVersions;
			}
		}

		public string StringHostVersion
		{
			set
			{
				int[] rtn;
				GetHostVersion(value, out rtn);
				mThisVersions[0] = rtn[0];
				mThisVersions[1] = rtn[1];
				mThisVersions[2] = rtn[2];
				mThisVersions[3] = rtn[3];
			}
		}
		

		public static VersionManager GetInstance()
		{
			if (versioncheck == null)
				versioncheck = new VersionManager();

			return versioncheck;
		}

		public VersionManager()
		{
            // 2011-09-20
            // replace fixed directory path with the installed path
            // remove reliance on loading of serial.dat file on startup
            // string SerialName = Tesla.Common.Utilities.GetDefaultAppPath() + "config\\serial.dat";
            //string SerialName = @"C:\Program Files\STI\RoboSep\config\serial.dat";
			mThisVersions			= new int[4];
			mInputVersions			= new int[4];
			Array.Clear(mThisVersions,0,4);
			Array.Clear(mInputVersions,0,4);

            // 2011-09-20
            // remove reliance on loading of serial.dat file on startup and return the default serial number
            /* 
            using (StreamReader sr = new StreamReader(SerialName)) 
			{
				try 
				{
					mRoboSepSerial = sr.ReadLine();
				}
				catch(Exception e)
				{
					mRoboSepSerial = "0000";
				}
			}
            //*/
            mRoboSepSerial = "0000";
        }

		protected bool GetVersion(string Version, out int[] ver)
		{
			string		delimStr = " .:";
			char[]		delimiter = delimStr.ToCharArray();
			string[]	split = null;
			
			ver= new int[4];

			split = Version.Split(delimiter, 32);
			try
			{
				ver[0] = Int32.Parse(split[8]);
				ver[1] = Int32.Parse(split[9]);
				ver[2] = Int32.Parse(split[10]);
				ver[3] = Int32.Parse(split[11]);
			}
			catch(Exception)
			{
				return false;
			}
			

			return true;
		}

		protected bool GetHostVersion(string Version, out int[] ver)
		{
			string		delimStr = " .:";
			char[]		delimiter = delimStr.ToCharArray();
			string[]	split = null;
			
			ver= new int[4];

			split = Version.Split(delimiter, 32);
			try
			{
				ver[0] = Int32.Parse(split[0]);
				ver[1] = Int32.Parse(split[1]);
				ver[2] = Int32.Parse(split[2]);
				ver[3] = Int32.Parse(split[3]);
			}
			catch(Exception)
			{
				return false;
			}
			

			return true;
		}

		public bool CheckFileVersion(string FileName)
		{
			try 
			{
				// Create an instance of StreamReader to read from a file.
				// The using statement also closes the StreamReader.
				using (StreamReader sr = new StreamReader(FileName)) 
				{
					String line;
					int[] rtn;
					int	  cmp;
					// Read and display lines from the file until the end of 
					// the file is reached.
					while ((line = sr.ReadLine()) != null) 
					{
						cmp = String.CompareOrdinal(line,0,mComment[0],0,4);
						
						if (cmp == 0)
						{
							if (!GetVersion( line, out rtn ))
							{
								return false;								
							}
							else
							{
							
							}
							Array.Copy(rtn, mInputVersions,4);
							if (mInputVersions[0] > mThisVersions[0])
								return false;
							if (  (mInputVersions[0] == mThisVersions[0])
								&&(mInputVersions[1] > mThisVersions[1]))
								return false;
							if (  (mInputVersions[0] == mThisVersions[0])
								&&(mInputVersions[1] == mThisVersions[1])
								&&(mInputVersions[2] > mThisVersions[2]))
								return false;
//							if (mInputVersions[3] > mThisVersions[3])
//								return false;
						}
					
					}
					return true;
				}
			}
			catch (Exception) 
			{
				return false;
			}		

		}

		public void SetFileVersion(string FileName)
		{
			string msg;
			DateTime CurrTime = DateTime.Now;

			msg = string.Format( 
								 mComment[0] +
				                 "{0}.{1}.{2}.{3}" +
				                 mComment[1] +
								 "{4}" + 
				                 mComment[2] +
				                 "{5}" +
								 mComment[3],
								 mThisVersions[0].ToString(),
								 mThisVersions[1].ToString(),
								 mThisVersions[2].ToString(),
								 mThisVersions[3].ToString(),
								 mRoboSepSerial,
								 CurrTime
							    );
			
			using (StreamWriter sw = File.AppendText(FileName)) 
			{
				sw.WriteLine("");
				sw.WriteLine(msg);
				sw.Close();
			}    		
		}

		public void CleanInputVersions()
		{
			Array.Clear(mInputVersions,0,4);
		}
	}
}