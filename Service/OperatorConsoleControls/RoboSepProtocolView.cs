using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using System.IO;
using Tesla.DataAccess;

using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Serialization;

using Tesla.Common.DrawingUtilities;
using Tesla.Common.ResourceManagement;

namespace Tesla.OperatorConsoleControls
{
	/// <summary>
	/// Summary description for RoboSepProtocolView.
	/// </summary>
	public class RoboSepProtocolView : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListBox lstAllProtocols;	
		private OpenFileDialog myOpenFileDialog;
		static private string myProtocolsPath = Application.StartupPath+"\\..\\protocols\\";
		static private string myXSDPath = Application.StartupPath+"\\..\\config\\RoboSepUser.xsd";
		private Tesla.OperatorConsoleControls.RoboSepButton btnSave;
		private Tesla.OperatorConsoleControls.RoboSepButton btnCancel;
		private Tesla.OperatorConsoleControls.RoboSepNamedPanel pnlUpdateUsers;
		private Tesla.OperatorConsoleControls.RoboSepButton btnExit;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#region Construction/destruction
		public RoboSepProtocolView()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//btnAddExt.Text = "Add to Current List of Protocols          (i.e. Add from USB Key or Network)";
			btnSave.Text = "Add";
			btnCancel.Text = "Remove";
			btnExit.Text = "Exit";

			pnlUpdateUsers.Text="Current List of Protocols";

			btnSave.Role = RoboSepButton.ButtonRole.Warning;
			btnCancel.Role = RoboSepButton.ButtonRole.Warning;
			btnExit.Role = RoboSepButton.ButtonRole.OK;


			Font font = SeparatorResourceManager.GetFont(SeparatorResourceManager.FontId.InstrumentTasksPageTitle);
			
			Color activeSubPageBackground= ColourScheme.GetColour(ColourSchemeItem.ActiveSubPageBackground);
			this.BackColor = activeSubPageBackground;
			pnlUpdateUsers.FillColor = ColourScheme.GetColour(ColourSchemeItem.NamedAreaStandardBackground);

			Color backColor = ColourScheme.GetColour(ColourSchemeItem.NamedAreaStandardBackground);
			lstAllProtocols.BackColor = backColor;
			btnSave.BackColor = backColor;
			btnCancel.BackColor = backColor;
			btnExit.BackColor = backColor;
			
			Color foreColor = ColourScheme.GetColour(ColourSchemeItem.NamedAreaTextForeground);
			lstAllProtocols.ForeColor = foreColor;
			
			//load files to list from protocols
			LoadLstAllProtocols();

			this.Invalidate();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}


		#endregion Construction/destruction

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code Exit.
		/// </summary>
		private void InitializeComponent()
		{
			this.lstAllProtocols = new System.Windows.Forms.ListBox();
			this.btnSave = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.btnCancel = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.pnlUpdateUsers = new Tesla.OperatorConsoleControls.RoboSepNamedPanel();
			this.btnExit = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.SuspendLayout();
			// 
			// lstAllProtocols
			// 
			this.lstAllProtocols.Location = new System.Drawing.Point(8, 56);
			this.lstAllProtocols.Name = "lstAllProtocols";
			this.lstAllProtocols.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.lstAllProtocols.Size = new System.Drawing.Size(784, 433);
			this.lstAllProtocols.TabIndex = 0;
			// 
			// btnSave
			// 
			this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
			this.btnSave.Location = new System.Drawing.Point(8, 512);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(120, 48);
			this.btnSave.TabIndex = 7;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
			this.btnCancel.Location = new System.Drawing.Point(336, 512);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(120, 48);
			this.btnCancel.TabIndex = 12;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// pnlUpdateUsers
			// 
			this.pnlUpdateUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.pnlUpdateUsers.BackColor = System.Drawing.SystemColors.Control;
			this.pnlUpdateUsers.FillColor = System.Drawing.Color.White;
			this.pnlUpdateUsers.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
			this.pnlUpdateUsers.Location = new System.Drawing.Point(0, 0);
			this.pnlUpdateUsers.Name = "pnlUpdateUsers";
			this.pnlUpdateUsers.Size = new System.Drawing.Size(620, 558);
			this.pnlUpdateUsers.TabIndex = 0;
			// 
			// btnExit
			// 
			this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
			this.btnExit.Location = new System.Drawing.Point(672, 512);
			this.btnExit.Name = "btnExit";
			this.btnExit.Size = new System.Drawing.Size(120, 48);
			this.btnExit.TabIndex = 13;
			this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
			// 
			// RoboSepProtocolView
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(800, 568);
			this.Controls.Add(this.btnExit);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.lstAllProtocols);
			this.Controls.Add(this.pnlUpdateUsers);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "RoboSepProtocolView";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "RoboSepProtocolView";
			this.Resize += new System.EventHandler(this.RoboSepProtocolView_Resize);
			this.ResumeLayout(false);

		}
		#endregion

		#region Control event handlers

		

		private void btnSave_Click(object sender, System.EventArgs e)
		{
			lstAllProtocols.SelectedIndex=-1;
			lstAllProtocols.Invalidate();

			myOpenFileDialog = new OpenFileDialog();
			myOpenFileDialog.Title="Please Select a Protocol";
			myOpenFileDialog.InitialDirectory = myProtocolsPath;
			myOpenFileDialog.Filter = @"Protocol files (*.xml)|*.xml";//|All files (*.*)|*.*";
			myOpenFileDialog.FilterIndex = 1;
			myOpenFileDialog.RestoreDirectory = true;
			
			if(myOpenFileDialog.ShowDialog() == DialogResult.OK)
			{
				string filename = myOpenFileDialog.FileName;
				string basename = new FileInfo(filename).Name;
				string destName = myProtocolsPath+basename;
				
				//check for overwrite
				//copy file to protocols
				if(!File.Exists(destName))
				{
					File.Copy(filename,destName);
				}
				//else if(MessageBox.Show("Do you want to overwrite existing file?",
				//	"Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == 
				//	DialogResult.Yes)
				else if( (new RoboSepDlgBox("Warning","Would you like to overwrite existing file?")).ShowDialog() == 
					DialogResult.OK)
				{
					File.Copy(filename,destName,true);
				}

				//refresh protocols list
				LoadLstAllProtocols();
			}

		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			int totalSelectedItems = lstAllProtocols.SelectedItems.Count;

			if(totalSelectedItems<=0)
				return;
			if( (new RoboSepDlgBox("Warning","Removed the protocol(s) will be in the protocols\\archive folder. Would you like to continue?")).ShowDialog() == 
					DialogResult.Cancel)
				return;

			object[] items = new object[totalSelectedItems];
			string filename, basename, destName, dir;

			dir = myProtocolsPath+"archive\\";
			if(!Directory.Exists(dir))
				Directory.CreateDirectory(dir);

			for(int i=0; i<totalSelectedItems;++i)
			{
				object item = lstAllProtocols.SelectedItems[i];
				items[i]=item;
			}
			for(int i=0; i<totalSelectedItems;++i)
			{
				basename = (string)items[i];
				filename = myProtocolsPath+basename;
				destName = myProtocolsPath+"archive\\"+basename;
				File.Delete(destName);
				File.Move(filename,destName);
				lstAllProtocols.Items.Remove(items[i]);
			}
		}

		#endregion Control event handlers

		private void LoadLstAllProtocols()
		{
			DirectoryInfo di = new DirectoryInfo(myProtocolsPath);
			FileInfo[] rgFiles = di.GetFiles("*.xml");
			lstAllProtocols.Items.Clear();
			foreach(FileInfo fi in rgFiles)
			{
				if(!fi.Name.Equals("prime.xml") && 
					!fi.Name.Equals("shutdown.xml") &&
					!fi.Name.Equals("home_axes.xml") )
					lstAllProtocols.Items.Add(fi.Name);       
			}
		}


		private void RoboSepProtocolView_Resize(object sender, System.EventArgs e)
		{
			//pnlUpdateUsers.Left = 2 * DrawingUtilities.MinimumTabMargin;
			pnlUpdateUsers.Width = (this.Width );//- 3 * DrawingUtilities.MinimumTabMargin);			
			pnlUpdateUsers.Height = this.Height;
		}

		private void lstAllProtocols_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("------------------000000000000000000-----------------"+"lstAllProtocols_DrawItem");
			lstAllProtocols.DrawMode = DrawMode.OwnerDrawFixed;
			e.DrawBackground();
			Brush myBrush = Brushes.Black;
			switch(e.Index%2)
			{
				case 0: myBrush = Brushes.Red;
					break;
				case 1: myBrush = Brushes.Blue;
					break;
				case 2: myBrush = Brushes.Yellow;
					break;

			}

			e.Graphics.DrawString(lstAllProtocols.Items[e.Index].ToString(),
				e.Font,
				myBrush,
				e.Bounds,
				StringFormat.GenericDefault);
			e.DrawFocusRectangle();
		}

		private void btnExit_Click(object sender, System.EventArgs e)
		{
		
			DialogResult = DialogResult.OK;
		}
	}
}
