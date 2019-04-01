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
	/// Summary description for RoboSepProtocolFileView.
	/// </summary>
	public class RoboSepProtocolFileView : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListBox lstAllProtocols;
		private System.Windows.Forms.ListBox lstUserProtocols;
		private Tesla.OperatorConsoleControls.RoboSepButton btnAdd;
		private Tesla.OperatorConsoleControls.RoboSepButton btnRemove;	
		private OpenFileDialog myOpenFileDialog;
		private System.Windows.Forms.Label lblUserProtocols;
		private System.Windows.Forms.Label lblAllProtocols;
		static private string myProtocolsPath = Application.StartupPath+"\\..\\protocols\\";
		static private string myXSDPath = Application.StartupPath+"\\..\\config\\RoboSepUser.xsd";
		private string myUserDBFile;
		private System.Windows.Forms.TextBox txtUserName;
		private System.Windows.Forms.Label lblName;
		private Tesla.OperatorConsoleControls.RoboSepButton btnSave;
		private Tesla.OperatorConsoleControls.RoboSepButton btnCancel;
		private Tesla.OperatorConsoleControls.RoboSepNamedPanel pnlUpdateUsers;

		private bool isPresetUser=false;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#region Construction/destruction
		public RoboSepProtocolFileView(string userDBPath)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			btnAdd.Text = "Add             .";
			btnRemove.Text = "Remove      .";
			//btnAddExt.Text = "Add to Current List of Protocols          (i.e. Add from USB Key or Network)";
			btnSave.Text = "Save";
			btnCancel.Text = "Cancel";

			pnlUpdateUsers.Text="Modify User";

			btnAdd.Role = RoboSepButton.ButtonRole.Warning_RIGHT;
			btnRemove.Role = RoboSepButton.ButtonRole.Warning_LEFT;
			//btnAddExt.Role = RoboSepButton.ButtonRole.OK_BIG;
			btnSave.Role = RoboSepButton.ButtonRole.OK;
			btnCancel.Role = RoboSepButton.ButtonRole.Error;


			Font font = SeparatorResourceManager.GetFont(SeparatorResourceManager.FontId.InstrumentTasksPageTitle);
			lblAllProtocols.Font= font;
			lblName.Font= font;
			lblUserProtocols.Font= font;

			Color activeSubPageBackground= ColourScheme.GetColour(ColourSchemeItem.ActiveSubPageBackground);
			this.BackColor = activeSubPageBackground;
			pnlUpdateUsers.FillColor = ColourScheme.GetColour(ColourSchemeItem.NamedAreaStandardBackground);

			Color backColor = ColourScheme.GetColour(ColourSchemeItem.NamedAreaStandardBackground);
			lstAllProtocols.BackColor = backColor;
			lstUserProtocols.BackColor = backColor;
			txtUserName.BackColor = backColor;
			lblAllProtocols.BackColor = backColor;
			lblName.BackColor = backColor;
			lblUserProtocols.BackColor = backColor;
			btnSave.BackColor = backColor;
			btnCancel.BackColor = backColor;
			//btnAddExt.BackColor = backColor;
			btnRemove.BackColor = backColor;
			btnAdd.BackColor = backColor;

			Color foreColor = ColourScheme.GetColour(ColourSchemeItem.NamedAreaTextForeground);
			lstAllProtocols.ForeColor = foreColor;
			lstUserProtocols.ForeColor = foreColor;
			lblAllProtocols.ForeColor = foreColor;
			lblName.ForeColor = foreColor;
			lblUserProtocols.ForeColor = foreColor;

			//load files to list from protocols
			LoadLstAllProtocols();

			
			string[] presetUserNames = new string[]{UserProfileSubPage.ALL_HUMAN,
													   UserProfileSubPage.ALL_MOUSE,
													   UserProfileSubPage.ALL_WHOLE_BLOOD,
													   UserProfileSubPage.ALL_OTHER,
													   UserProfileSubPage.ALL_TYPES};
			isPresetUser=false;
			for(int i=0;i<presetUserNames.Length;i++)
			{
				if(userDBPath == presetUserNames[i])
				{
					isPresetUser=true;
					break;
				}
			}
			
			//load user protocols to list from user name
			myUserDBFile=userDBPath;
			if(isPresetUser)
			{
				lstUserProtocols.Items.Add(myUserDBFile);
				btnSave.Text = "Update";
				pnlUpdateUsers.Text="Update Database";
				btnAdd.Enabled=false;
				btnRemove.Enabled=false;
				lstAllProtocols.Enabled=false;
				lstUserProtocols.Enabled=false;
				txtUserName.Text=userDBPath;
				txtUserName.Enabled=false;
				lblName.Text="Database Name:";
				lblUserProtocols.Text="Database Protocols";
			}
			else
			{
				LoadUserProfile();
			}

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
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lstAllProtocols = new System.Windows.Forms.ListBox();
			this.lstUserProtocols = new System.Windows.Forms.ListBox();
			this.btnAdd = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.btnRemove = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.btnSave = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.lblUserProtocols = new System.Windows.Forms.Label();
			this.lblAllProtocols = new System.Windows.Forms.Label();
			this.txtUserName = new System.Windows.Forms.TextBox();
			this.lblName = new System.Windows.Forms.Label();
			this.btnCancel = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.pnlUpdateUsers = new Tesla.OperatorConsoleControls.RoboSepNamedPanel();
			this.SuspendLayout();
			// 
			// lstAllProtocols
			// 
			this.lstAllProtocols.Location = new System.Drawing.Point(8, 136);
			this.lstAllProtocols.Name = "lstAllProtocols";
			this.lstAllProtocols.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.lstAllProtocols.Size = new System.Drawing.Size(330, 368);
			this.lstAllProtocols.TabIndex = 0;
			// 
			// lstUserProtocols
			// 
			this.lstUserProtocols.Location = new System.Drawing.Point(456, 136);
			this.lstUserProtocols.Name = "lstUserProtocols";
			this.lstUserProtocols.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.lstUserProtocols.Size = new System.Drawing.Size(330, 368);
			this.lstUserProtocols.TabIndex = 1;
			// 
			// btnAdd
			// 
			this.btnAdd.Location = new System.Drawing.Point(352, 152);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(80, 80);
			this.btnAdd.TabIndex = 2;
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// btnRemove
			// 
			this.btnRemove.Location = new System.Drawing.Point(352, 408);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Size = new System.Drawing.Size(80, 80);
			this.btnRemove.TabIndex = 4;
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			// 
			// btnSave
			// 
			this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
			this.btnSave.Location = new System.Drawing.Point(672, 512);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(120, 48);
			this.btnSave.TabIndex = 7;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// lblUserProtocols
			// 
			this.lblUserProtocols.Location = new System.Drawing.Point(456, 112);
			this.lblUserProtocols.Name = "lblUserProtocols";
			this.lblUserProtocols.Size = new System.Drawing.Size(280, 23);
			this.lblUserProtocols.TabIndex = 8;
			this.lblUserProtocols.Text = "User Protocols";
			// 
			// lblAllProtocols
			// 
			this.lblAllProtocols.Location = new System.Drawing.Point(8, 112);
			this.lblAllProtocols.Name = "lblAllProtocols";
			this.lblAllProtocols.Size = new System.Drawing.Size(272, 23);
			this.lblAllProtocols.TabIndex = 9;
			this.lblAllProtocols.Text = "Current List of Protocols";
			// 
			// txtUserName
			// 
			this.txtUserName.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
			this.txtUserName.Location = new System.Drawing.Point(240, 48);
			this.txtUserName.Name = "txtUserName";
			this.txtUserName.Size = new System.Drawing.Size(312, 32);
			this.txtUserName.TabIndex = 10;
			this.txtUserName.Text = "someone";
			// 
			// lblName
			// 
			this.lblName.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
			this.lblName.Location = new System.Drawing.Point(16, 56);
			this.lblName.Name = "lblName";
			this.lblName.Size = new System.Drawing.Size(216, 23);
			this.lblName.TabIndex = 11;
			this.lblName.Text = "User Name:";
			this.lblName.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
			this.btnCancel.Location = new System.Drawing.Point(8, 512);
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
			// RoboSepProtocolFileView
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(800, 568);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.lblName);
			this.Controls.Add(this.txtUserName);
			this.Controls.Add(this.lblAllProtocols);
			this.Controls.Add(this.lblUserProtocols);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.btnRemove);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.lstUserProtocols);
			this.Controls.Add(this.lstAllProtocols);
			this.Controls.Add(this.pnlUpdateUsers);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "RoboSepProtocolFileView";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "RoboSepProtocolFileView";
			this.Resize += new System.EventHandler(this.RoboSepProtocolFileView_Resize);
			this.ResumeLayout(false);

		}
		#endregion

		#region Control event handlers

		private void btnSave_Click(object sender, System.EventArgs e)
		{
			if(isPresetUser)
			{
				File.Delete(myProtocolsPath+myUserDBFile+"_profile.db");
				File.Delete(myProtocolsPath+myUserDBFile+"_protocols.db");
			}
			else
			{
				//save to profile
				SaveUserProfile();
			}
			DialogResult = DialogResult.OK;
		}

		
		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			
			DialogResult = DialogResult.Cancel;
		}

		private void btnAddExt_Click(object sender, System.EventArgs e)
		{
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
				else if( (new RoboSepDlgBox("Warning","Do you want to overwrite existing file?")).ShowDialog() == 
					DialogResult.OK)
				{
					File.Copy(filename,destName,true);
				}

				//refresh protocols list
				LoadLstAllProtocols();
			}

		}

		private void btnAdd_Click(object sender, System.EventArgs e)
		{
			for(int i=0; i<lstAllProtocols.SelectedItems.Count;++i)
			{
				object item = lstAllProtocols.SelectedItems[i];
				if(!lstUserProtocols.Items.Contains(item))
				{
					lstUserProtocols.Items.Add(item);
				}
			}
		}

		private void btnRemove_Click(object sender, System.EventArgs e)
		{
			int totalSelectedItems = lstUserProtocols.SelectedItems.Count;
			
			object[] items = new object[totalSelectedItems];

			for(int i=0; i<totalSelectedItems;++i)
			{
				object item = lstUserProtocols.SelectedItems[i];
				items[i]=item;
			}
			for(int i=0; i<totalSelectedItems;++i)
			{
				lstUserProtocols.Items.Remove(items[i]);
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


		static private XmlSerializer	myXmlSerializer = new XmlSerializer(typeof(RoboSepUser));
		static private RoboSepUser LoadRoboSepUserObject(string filename)
		{
			RoboSepUser user = null;
			if(File.Exists(myProtocolsPath+filename))
			{

				// Initialise a file stream for reading
				FileStream fs = new FileStream(myProtocolsPath+filename, FileMode.Open);

				try
				{
					// Deserialize a RoboSepProtocol XML description into a RoboSepProtocol 
					// object that matches the contents of the specified protocol file.										
					XmlReader reader = new XmlTextReader(fs);

					// Create a validating reader to process the file.  Report any errors to the 
					// validation page.
					XmlValidatingReader validatingReader = new XmlValidatingReader(reader);
					validatingReader.ValidationType = ValidationType.Schema;

					// Get the RoboSep protocol schema and add it to the collection for the 
					// validator
					XmlSchemaCollection xsc = new XmlSchemaCollection();			
					xsc.Add("STI", myXSDPath );
					validatingReader.Schemas.Add(xsc);
						
					// 'Rehydrate' the object (that is, deserialise data into the object)					
					user = (RoboSepUser) myXmlSerializer.Deserialize(validatingReader);

				}
				catch (Exception /*ex*/)
				{
					MessageBox.Show("User DB File Invalid");
				}
				finally
				{
					// Close the file stream
					fs.Close();
				}
			}
			return user;
		}
		private void LoadUserProfile()
		{
			RoboSepUser user = LoadRoboSepUserObject(myUserDBFile);
			if(user!=null)
			{
				txtUserName.Text = user.UserName;
				if(user.ProtocolFile!=null)
				{
					for(int i =0; i< user.ProtocolFile.Length; ++i)
					{
						if(File.Exists(myProtocolsPath+user.ProtocolFile[i]))
							lstUserProtocols.Items.Add(user.ProtocolFile[i]);
					}
				}
			}
			else
			{
				txtUserName.Text = System.IO.Path.GetFileNameWithoutExtension(myUserDBFile);
			}
		}


		private void SaveUserProfile()
		{
			RoboSepUser user= new RoboSepUser();
			user.UserName=txtUserName.Text;
			user.ProtocolFile = new string[lstUserProtocols.Items.Count];
			for(int i=0; i<lstUserProtocols.Items.Count; ++i)
			{
				user.ProtocolFile[i]=(string)lstUserProtocols.Items[i];
			}

			using (FileStream fs = new FileStream(myProtocolsPath+myUserDBFile, FileMode.Create))
			{
				XmlTextWriter writer = new XmlTextWriter(fs, new UTF8Encoding());
				writer.Formatting = Formatting.Indented;
				myXmlSerializer.Serialize(writer, user);
			}

		}

		static public string UserProfileDBToName(string filename)
		{
			RoboSepUser user = LoadRoboSepUserObject(filename);

			return user==null? System.IO.Path.GetFileNameWithoutExtension(filename):user.UserName;
		}

		static public string UserProfileDBExtension
		{
			get
			{
				return "udb";
			}
		}
		static public string UserProfileDBPath
		{
			get
			{
				return myProtocolsPath;
			}
		}

		static public int GetUserProtocolsCount(string filename)
		{
			int result =0;
			RoboSepUser user = LoadRoboSepUserObject(filename);
			if(user!=null)
			{
				result = user.ProtocolFile.Length;
			}
			return result;
		}

		private void RoboSepProtocolFileView_Resize(object sender, System.EventArgs e)
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
	}
}
