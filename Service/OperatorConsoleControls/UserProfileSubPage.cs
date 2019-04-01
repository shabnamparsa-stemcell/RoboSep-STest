using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;

using System.IO;
using Tesla.Common.DrawingUtilities;
using Tesla.Common.ResourceManagement;


using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Serialization;


using Tesla.DataAccess;
using Tesla.Common.OperatorConsole;
using System.Data;

using System.Threading;


namespace Tesla.OperatorConsoleControls
{
	public class UserProfileSubPage : Tesla.OperatorConsoleControls.RoboSepSubPage
	{
		private System.Windows.Forms.Panel pnlUserProfileBackground;
		private Tesla.OperatorConsoleControls.RoboSepButton btnLoad;
		private Tesla.OperatorConsoleControls.RoboSepButton btnUpdate;
		private Tesla.OperatorConsoleControls.RoboSepNamedPanel pnlCustomUsers;
		private Tesla.OperatorConsoleControls.RoboSepNamedPanel pnlPresetUsers;
		private Tesla.OperatorConsoleControls.RoboSepItemList lstCustomUsers;
		private Tesla.OperatorConsoleControls.RoboSepItemList lstPresetUsers;
		private System.ComponentModel.IContainer components = null;


		static private string myUserConfigPath = Application.StartupPath+"\\..\\config\\UserConfig.config";
		static private string myXSDPath = Application.StartupPath+"\\..\\config\\RoboSepUserConfig.xsd";
		static private string myProtocolsPath = Application.StartupPath+"\\..\\protocols\\";

		public const string ALL_HUMAN = "All Human";
		public const string ALL_MOUSE = "All Mouse";
		public const string ALL_WHOLE_BLOOD = "All Whole Blood";
		public const string ALL_OTHER = "All Other";
		public const string ALL_TYPES = "All";

		private const int totalCustomUsers = 5;
		
		private string myCurrentActionContext = null;
		private string mySelectedActionContext = null;

		private Thread myReloadProtocolsThread;
		private Tesla.OperatorConsoleControls.RoboSepButton btnProtocol;
		private Tesla.OperatorConsoleControls.RoboSepButton btnEditor;
		private RoboSepWaitForm myRoboSepWaitForm;

		#region Construction/destruction
		public UserProfileSubPage()
			: base(RoboSepSubPage.MdiChild.UserProfile)
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// Initialise fixed text
			this.Text = "Profile";

			btnUpdate.Text = "Modify/Update";
			btnUpdate.Role = RoboSepButton.ButtonRole.OK;
			btnLoad.Text = "Load";
			btnLoad.Role = RoboSepButton.ButtonRole.OK;
			btnProtocol.Text = "Protocol List";
			btnProtocol.Role = RoboSepButton.ButtonRole.Warning;
			btnEditor.Text = "Protocol Editor";
			btnEditor.Role = RoboSepButton.ButtonRole.Warning;

			pnlCustomUsers.Text = "Custom Users";
			pnlPresetUsers.Text = "Preset Protocol Databases";

			// Initialise colour scheme
			Color activeSubPageBackground= ColourScheme.GetColour(ColourSchemeItem.ActiveSubPageBackground);
			pnlCustomUsers.BackColor = activeSubPageBackground;
			pnlPresetUsers.BackColor	 = activeSubPageBackground;

			pnlCustomUsers.FillColor = ColourScheme.GetColour(ColourSchemeItem.NamedAreaStandardBackground);
			lstCustomUsers.BackColor = pnlCustomUsers.FillColor;
			pnlPresetUsers.FillColor = pnlCustomUsers.FillColor;					
			lstPresetUsers.BackColor = pnlCustomUsers.FillColor;

			LoadCustomUsers();
			LoadPresetUsers();

			LoadCurrentUser();

			SeparatorGateway.GetInstance().UpdateChosenProtocolTable += new SampleTableDelegate(AtUpdateChosenProtocolTableUpdate);
			//SeparatorGateway.GetInstance()
			//myCurrentActionContext = listPresetUsersItems[0].Tag;

			myRoboSepWaitForm= new RoboSepWaitForm();
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

		#endregion Construction/destruction

		private void AtUpdateChosenProtocolTableUpdate(DataTable chosenProtocols, ProtocolMix protocolMix)
		{
			//check if table empty...	
			//enable if empty else disable
			if(chosenProtocols.Rows.Count > 0)
			{
				DisableSelectionAccess(UiAccessMode.RunningMode);
			}
			else
			{
				EnableSelectionAccess(UiAccessMode.All);
			}
		}


		#region Global MDI Support
		
		public override void EnableSelectionAccess(UiAccessMode accessMode)
		{
			btnLoad.Enabled = true;
			btnUpdate.Enabled = true;
			pnlCustomUsers.Enabled = true;
			pnlPresetUsers.Enabled = true;
			btnProtocol.Enabled=true;
			btnEditor.Enabled=true;
		}

		public override void DisableSelectionAccess(UiAccessMode accessMode)
		{	
			btnLoad.Enabled = false;
			btnUpdate.Enabled = false;
			pnlCustomUsers.Enabled = false;
			pnlPresetUsers.Enabled = false;
			if(accessMode == UiAccessMode.All)
			{
				btnProtocol.Enabled=false;
				btnEditor.Enabled=false;
			}
		}

		#endregion Global MDI Support

		#region Control event handlers

		private void UserProfileSubPage_Resize(object sender, System.EventArgs e)
		{
			base.ResizeHandler(sender, e);
			pnlCustomUsers.Left = 2 * DrawingUtilities.MinimumTabMargin;
			pnlCustomUsers.Width = (pnlUserProfileBackground.Width - 3 * 
				DrawingUtilities.MinimumTabMargin)/2;			
			pnlPresetUsers.Width = pnlCustomUsers.Width;
			pnlPresetUsers.Left = pnlCustomUsers.Width + 
				2 * DrawingUtilities.MinimumTabMargin;
			pnlCustomUsers.Invalidate();
			pnlPresetUsers.Invalidate();
			
		}

		private void btnLoad_Click(object sender, System.EventArgs e)
		{
			if(lstCustomUsers.ContainsItemByTag(mySelectedActionContext) || 
				lstPresetUsers.ContainsItemByTag(mySelectedActionContext) )
			{
				
				myCurrentActionContext=mySelectedActionContext;

				Enabled=false;		

				// Reload protocols in a thread
				myReloadProtocolsThread = new Thread(new ThreadStart(this.ReloadProtocolsThread));
				myReloadProtocolsThread.IsBackground = true;
				myReloadProtocolsThread.Start();


				myRoboSepWaitForm.ShowDialog();
				//System.Threading.Thread.Sleep(2000);
				SeparatorGateway.GetInstance().Connect(false);
				//System.Threading.Thread.Sleep(2000);
				


				Enabled=true;
			}
			else
			{
				MessageBox.Show(this,"Please select a Custom User or a Preset User first.");
			}


		}

		private void ReloadProtocolsThread()
		{
			SaveUserConfig();
			System.Threading.Thread.Sleep(1000);
			SeparatorGateway.GetInstance().ReloadProtocols();
			//SeparatorGateway.GetInstance().Disconnect(false);
			//System.Threading.Thread.Sleep(3000);
			//SeparatorGateway.GetInstance().Connect(false);
			//SeparatorGateway.GetInstance().Connect(true);
			//System.Threading.Thread.Sleep(1000);
		}



		private void btnUpdate_Click(object sender, System.EventArgs e)
		{
			//if(lstCustomUsers.ContainsItemByTag(mySelectedActionContext) )
			{
				new RoboSepProtocolFileView(mySelectedActionContext).ShowDialog();
				//refresh for name change
				LoadCustomUsers();
				lstCustomUsers.SelectItemByTag(mySelectedActionContext);
				lstCustomUsers.Invalidate();	
			}
			/*
			else if(MessageBox.Show("Would you like to update "+mySelectedActionContext+"?",
				"Preset Update",MessageBoxButtons.YesNo, MessageBoxIcon.Question) == 
				DialogResult.Yes)
			{
				File.Delete(myProtocolsPath+mySelectedActionContext+"_profile.db");
				File.Delete(myProtocolsPath+mySelectedActionContext+"_protocols.db");
			}
			*/
		}

		private void lstCustomUsers_InvokeItemAction(object actionContext)
		{
			mySelectedActionContext=(string)actionContext;
			lstPresetUsers.SelectItemByTag(null);
			lstCustomUsers.SelectItemByTag(mySelectedActionContext);
		}

		private void lstPresetUsers_InvokeItemAction(object actionContext)
		{
			mySelectedActionContext=(string)actionContext;
			lstCustomUsers.SelectItemByTag(null);
			lstPresetUsers.SelectItemByTag(mySelectedActionContext);
		}

		private void UserProfileSubPage_Load(object sender, System.EventArgs e)
		{
			mySelectedActionContext=myCurrentActionContext;
			lstCustomUsers.SelectItemByTag(mySelectedActionContext);
			lstPresetUsers.SelectItemByTag(mySelectedActionContext);
		}

		private void UserProfileSubPage_Leave(object sender, System.EventArgs e)
		{
			mySelectedActionContext=myCurrentActionContext;
			lstCustomUsers.SelectItemByTag(mySelectedActionContext);
			lstPresetUsers.SelectItemByTag(mySelectedActionContext);
		}

		#endregion Control event handlers	



		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.pnlUserProfileBackground = new System.Windows.Forms.Panel();
			this.btnEditor = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.btnProtocol = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.pnlPresetUsers = new Tesla.OperatorConsoleControls.RoboSepNamedPanel();
			this.lstPresetUsers = new Tesla.OperatorConsoleControls.RoboSepItemList();
			this.pnlCustomUsers = new Tesla.OperatorConsoleControls.RoboSepNamedPanel();
			this.lstCustomUsers = new Tesla.OperatorConsoleControls.RoboSepItemList();
			this.btnUpdate = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.btnLoad = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.pnlUserProfileBackground.SuspendLayout();
			this.pnlPresetUsers.SuspendLayout();
			this.pnlCustomUsers.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlUserProfileBackground
			// 
			this.pnlUserProfileBackground.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.pnlUserProfileBackground.Controls.Add(this.btnEditor);
			this.pnlUserProfileBackground.Controls.Add(this.btnProtocol);
			this.pnlUserProfileBackground.Controls.Add(this.pnlPresetUsers);
			this.pnlUserProfileBackground.Controls.Add(this.pnlCustomUsers);
			this.pnlUserProfileBackground.Controls.Add(this.btnUpdate);
			this.pnlUserProfileBackground.Controls.Add(this.btnLoad);
			this.pnlUserProfileBackground.Location = new System.Drawing.Point(0, 50);
			this.pnlUserProfileBackground.Name = "pnlUserProfileBackground";
			this.pnlUserProfileBackground.Size = new System.Drawing.Size(620, 360);
			this.pnlUserProfileBackground.TabIndex = 0;
			// 
			// btnEditor
			// 
			this.btnEditor.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnEditor.Location = new System.Drawing.Point(150, 308);
			this.btnEditor.Name = "btnEditor";
			this.btnEditor.Size = new System.Drawing.Size(120, 48);
			this.btnEditor.TabIndex = 4;
			this.btnEditor.Click += new System.EventHandler(this.btnEditor_Click);
			// 
			// btnProtocol
			// 
			this.btnProtocol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnProtocol.Location = new System.Drawing.Point(15, 308);
			this.btnProtocol.Name = "btnProtocol";
			this.btnProtocol.Size = new System.Drawing.Size(120, 48);
			this.btnProtocol.TabIndex = 1;
			this.btnProtocol.Click += new System.EventHandler(this.btnProtocol_Click);
			// 
			// pnlPresetUsers
			// 
			this.pnlPresetUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
			this.pnlPresetUsers.BackColor = System.Drawing.SystemColors.Control;
			this.pnlPresetUsers.Controls.Add(this.lstPresetUsers);
			this.pnlPresetUsers.FillColor = System.Drawing.Color.White;
			this.pnlPresetUsers.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
			this.pnlPresetUsers.Location = new System.Drawing.Point(315, 10);
			this.pnlPresetUsers.Name = "pnlPresetUsers";
			this.pnlPresetUsers.Size = new System.Drawing.Size(300, 272);
			this.pnlPresetUsers.TabIndex = 5;
			// 
			// lstPresetUsers
			// 
			this.lstPresetUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lstPresetUsers.BackColor = System.Drawing.Color.SandyBrown;
			this.lstPresetUsers.IsMultiSelect = false;
			this.lstPresetUsers.Location = new System.Drawing.Point(18, 25);
			this.lstPresetUsers.Name = "lstPresetUsers";
			this.lstPresetUsers.Size = new System.Drawing.Size(264, 234);
			this.lstPresetUsers.TabIndex = 3;
			this.lstPresetUsers.InvokeItemAction += new Tesla.OperatorConsoleControls.RoboSepItemList.InvokeItemActionDelegate(this.lstPresetUsers_InvokeItemAction);
			// 
			// pnlCustomUsers
			// 
			this.pnlCustomUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
			this.pnlCustomUsers.BackColor = System.Drawing.SystemColors.Control;
			this.pnlCustomUsers.Controls.Add(this.lstCustomUsers);
			this.pnlCustomUsers.FillColor = System.Drawing.Color.White;
			this.pnlCustomUsers.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
			this.pnlCustomUsers.Location = new System.Drawing.Point(10, 10);
			this.pnlCustomUsers.Name = "pnlCustomUsers";
			this.pnlCustomUsers.Size = new System.Drawing.Size(300, 272);
			this.pnlCustomUsers.TabIndex = 4;
			// 
			// lstCustomUsers
			// 
			this.lstCustomUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lstCustomUsers.BackColor = System.Drawing.Color.SandyBrown;
			this.lstCustomUsers.IsMultiSelect = false;
			this.lstCustomUsers.Location = new System.Drawing.Point(18, 25);
			this.lstCustomUsers.Name = "lstCustomUsers";
			this.lstCustomUsers.Size = new System.Drawing.Size(264, 234);
			this.lstCustomUsers.TabIndex = 2;
			this.lstCustomUsers.InvokeItemAction += new Tesla.OperatorConsoleControls.RoboSepItemList.InvokeItemActionDelegate(this.lstCustomUsers_InvokeItemAction);
			// 
			// btnUpdate
			// 
			this.btnUpdate.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnUpdate.Location = new System.Drawing.Point(344, 308);
			this.btnUpdate.Name = "btnUpdate";
			this.btnUpdate.Size = new System.Drawing.Size(120, 48);
			this.btnUpdate.TabIndex = 5;
			this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
			// 
			// btnLoad
			// 
			this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnLoad.Location = new System.Drawing.Point(492, 308);
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.Size = new System.Drawing.Size(120, 48);
			this.btnLoad.TabIndex = 0;
			this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
			// 
			// UserProfileSubPage
			// 
			this.Controls.Add(this.pnlUserProfileBackground);
			this.Name = "UserProfileSubPage";
			this.Size = new System.Drawing.Size(620, 410);
			this.Resize += new System.EventHandler(this.UserProfileSubPage_Resize);
			this.Load += new System.EventHandler(this.UserProfileSubPage_Load);
			this.Leave += new System.EventHandler(this.UserProfileSubPage_Leave);
			this.pnlUserProfileBackground.ResumeLayout(false);
			this.pnlPresetUsers.ResumeLayout(false);
			this.pnlCustomUsers.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private string[] getCustomUsers()
		{
			DirectoryInfo di = new DirectoryInfo(RoboSepProtocolFileView.UserProfileDBPath);
			FileInfo[] rgFiles = di.GetFiles("*."+RoboSepProtocolFileView.UserProfileDBExtension);
			string[] names = null;

			if(rgFiles.Length < totalCustomUsers)
			{
				names = new string[totalCustomUsers];
				for(int i=0; i< totalCustomUsers; ++i)
				{
					names[i]="User"+(i+1)+".udb";	
				}
			}
			else
			{
				names = new string[rgFiles.Length];
				for(int i=0;i<rgFiles.Length;++i)
				{
					names[i]=rgFiles[i].Name;       
				}
			}

			return names;
		}

		private void LoadCustomUsers()
		{
			lstCustomUsers.Clear();
			string[] customUserNames = getCustomUsers();
			//string[] customUserNames = new string[4]{"Jason.udb","Jody.udb","Steve.udb","Sam.udb"};
			int CustomUsersCount =customUserNames.Length;
			RoboSepListItem[] listCustomUsersItems = new RoboSepListItem[CustomUsersCount];
			for (int i = 0; i < CustomUsersCount; ++i)								
			{
				listCustomUsersItems[i] = new RoboSepListItem();
				listCustomUsersItems[i].Text = RoboSepProtocolFileView.UserProfileDBToName(
					customUserNames[i]);
				listCustomUsersItems[i].Tag  = (object)customUserNames[i];
			}
			lstCustomUsers.AddRange(listCustomUsersItems);
			lstCustomUsers.Invalidate();
		}
		private void LoadPresetUsers()
		{
			lstPresetUsers.Clear();
			string[] presetUserNames = new string[]{ALL_HUMAN,ALL_MOUSE,ALL_WHOLE_BLOOD,ALL_OTHER,ALL_TYPES};
			RoboSepListItem[] listPresetUsersItems = new RoboSepListItem[presetUserNames.Length];
			for (int i = 0; i < presetUserNames.Length; ++i)								
			{
				listPresetUsersItems[i] = new RoboSepListItem();
				listPresetUsersItems[i].Text = presetUserNames[i];
				listPresetUsersItems[i].Tag  = (object)presetUserNames[i];
			}
			lstPresetUsers.AddRange(listPresetUsersItems);
			lstPresetUsers.Invalidate();
		}

		private XmlSerializer	myXmlSerializer = new XmlSerializer(typeof(RoboSepUserConfig));
		private void LoadCurrentUser()
		{

			RoboSepUserConfig config = null;
			if(File.Exists(myUserConfigPath))
			{

				// Initialise a file stream for reading
				FileStream fs = new FileStream(myUserConfigPath, FileMode.Open);

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
					config = (RoboSepUserConfig) myXmlSerializer.Deserialize(validatingReader);
					myCurrentActionContext = config.CurrentUser;
				}
				catch (Exception /*ex*/)
				{
					MessageBox.Show("User Config File Invalid");
					myCurrentActionContext = "User1.udb";
				}
				finally
				{
					// Close the file stream
					fs.Close();
				}
			}
			else
			{

				myCurrentActionContext = "User1.udb";
			}
			
		}

		private void SaveUserConfig()
		{
			RoboSepUserConfig config= new RoboSepUserConfig();
			config.CurrentUser= myCurrentActionContext;

			using (FileStream fs = new FileStream(myUserConfigPath, FileMode.Create))
			{
				XmlTextWriter writer = new XmlTextWriter(fs, new UTF8Encoding());
				writer.Formatting = Formatting.Indented;
				myXmlSerializer.Serialize(writer, config);
			}

		}

		private void btnProtocol_Click(object sender, System.EventArgs e)
		{
			new RoboSepProtocolView().ShowDialog();
		}

		private void btnEditor_Click(object sender, System.EventArgs e)
		{
			//string url = "C:\\Program Files\\STI\\RoboSep\\Service\\Service.exe";
			System.Diagnostics.Process process = new System.Diagnostics.Process();
			process.StartInfo.FileName = "C:\\Program Files\\STI\\RoboSep\\bin\\ProtocolEditor.exe"; //"rundll32.exe";
			process.StartInfo.WorkingDirectory = "C:\\Program Files\\STI\\RoboSep\\bin\\";
			//process.StartInfo.Arguments = "url.dll,FileProtocolHandler "+url;
			process.StartInfo.UseShellExecute = true;
			process.Start();
		}


		}
}

