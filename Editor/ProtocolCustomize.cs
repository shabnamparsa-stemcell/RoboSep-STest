using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Tesla.ProtocolEditorModel;
using Tesla.Common.Protocol;
using Tesla.Common.ProtocolCommand;
using Tesla.Common.ResourceManagement;
using Tesla.Common.Separator;
using Tesla.DataAccess;
using Tesla.ProtocolEditorControls;

namespace Tesla.ProtocolEditor
{
	/// <summary>
	/// Summary description for ProtocolCustomize.
	/// </summary>
	public class ProtocolCustomize : System.Windows.Forms.Form
	{
		private static ProtocolModel theProtocolModel;
		private ProtocolTextbox txtCustomName;
		private System.Windows.Forms.Button btnAcceptNewName;
		private System.Windows.Forms.ToolTip tipDefaultName;
		private System.Windows.Forms.ToolTip tipNewName;
		private System.Windows.Forms.Label lblNewName;
		private System.ComponentModel.IContainer components;

		/* Local custom name variables */
		private string wasteTubeC;
		private string separationTubeC;
		private string lysisBufferTubeC;
		private string magneticParticleVialC;
		private string selectionCocktailVialC;
		private string antibodyCocktailVialC;
        private string sampleTubeC;
        private string bufferBottleC;
        private string bufferBottle34C;
        private string bufferBottle56C;

        //private string[] customNames = new string[ProtocolModel.NUM_CUST_NAMES_PER_QUAD];
        private customNames customVialNames = new customNames();
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ListBox lbDefaultNames;
		private System.Windows.Forms.ListBox lbCustomNames;
		private System.Windows.Forms.Label lblDefaultNames;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TabPage tabQ1;
		private System.Windows.Forms.TabPage tabQ2;
		private System.Windows.Forms.TabPage tabQ3;
		private System.Windows.Forms.TabPage tabQ4;
		private System.Windows.Forms.TabControl tabControl1;

		private ProtocolClass protocolClass = ProtocolClass.HumanPositive;

		public ProtocolCustomize()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Get a local reference to the protocol model. 
			theProtocolModel = ProtocolModel.GetInstance();

			tipDefaultName.SetToolTip(this.lbDefaultNames, "Select the default vial name to change.");
			tipNewName.SetToolTip(this.txtCustomName, "Type the custom name desired for selected default name.");
			
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

		public ProtocolClass SetProtocolClass
		{
			set
			{
				protocolClass = value;
			}
		}


		// compact debugmsg utility fn	  // bdr
		public void DbgView(string msg) 
        { 	System.Diagnostics.Debug.WriteLine(msg); }


		// called from CreateCustomWindow and Quadrant select tabs
		public void SetupCustomNames(int idx)
		{
			if (idx < 0 ||idx >= 4)		idx = 0;

			// extract existing custom names from the protocol
			try
			{
				customVialNames=theProtocolModel.GetCustomNames(idx);
			}
			catch(Exception){
            customVialNames= null;
            }

			// old protocols have no custom names set
			if (customVialNames == null)
			{
				SetDefaultCustomNames();
				theProtocolModel.ApplyCustomNames(idx, customVialNames);
			}
            
            if(customVialNames.bufferBottle==null) customVialNames.bufferBottle="";
            if(customVialNames.wasteTube==null) customVialNames.wasteTube="";
            if(customVialNames.lysisBufferTube==null) customVialNames.lysisBufferTube="";
            if(customVialNames.selectionCocktailVial==null) customVialNames.selectionCocktailVial="";
            if(customVialNames.magneticParticleVial==null) customVialNames.magneticParticleVial="";
            if(customVialNames.antibodyCocktailVial==null) customVialNames.antibodyCocktailVial="";
            if(customVialNames.sampleTube==null) customVialNames.sampleTube="";
            if(customVialNames.separationTube==null) customVialNames.separationTube="";
            if(customVialNames.bufferBottle34==null) customVialNames.bufferBottle34="";
            if(customVialNames.bufferBottle56==null) customVialNames.bufferBottle56="";

            bufferBottleC			= customVialNames.bufferBottle;
            wasteTubeC				= customVialNames.wasteTube;
            lysisBufferTubeC		= customVialNames.lysisBufferTube;
            selectionCocktailVialC	= customVialNames.selectionCocktailVial;
            magneticParticleVialC	= customVialNames.magneticParticleVial;
            antibodyCocktailVialC	= customVialNames.antibodyCocktailVial;
            sampleTubeC				= customVialNames.sampleTube;
            separationTubeC = customVialNames.separationTube;
            bufferBottle34C = customVialNames.bufferBottle34;
            bufferBottle56C = customVialNames.bufferBottle56;
            
			// fill protocol's custom names array with names
			this.SetupCustomListBox();
		}


		// set the default vial names as custom names - vial names vary with the type of protocol
		public void SetDefaultCustomNames()
		{
            customVialNames = new customNames();
			customVialNames.bufferBottle = SeparatorResourceManager.GetSeparatorString(StringId.QuadrantBuffer);
			customVialNames.wasteTube = SeparatorResourceManager.GetSeparatorString(StringId.WasteTube);
			customVialNames.magneticParticleVial = SeparatorResourceManager.GetSeparatorString(StringId.VialA);
			customVialNames.antibodyCocktailVial = SeparatorResourceManager.GetSeparatorString(StringId.VialC);
			customVialNames.sampleTube = SeparatorResourceManager.GetSeparatorString(StringId.SampleTube);
            customVialNames.separationTube = SeparatorResourceManager.GetSeparatorString(StringId.SeparationTube);
            customVialNames.bufferBottle34 = SeparatorResourceManager.GetSeparatorString(StringId.QuadrantBuffer34);
            customVialNames.bufferBottle56 = SeparatorResourceManager.GetSeparatorString(StringId.QuadrantBuffer56);
				
				customVialNames.lysisBufferTube = SeparatorResourceManager.getLysisStringFromProtocolClass(protocolClass);
			
				customVialNames.selectionCocktailVial = SeparatorResourceManager.getVialBStringFromProtocolClass(protocolClass);
		}



		public void SetupListBox()
		{
			//TUBE NAMES SYNC
			lbDefaultNames.Items.Clear();

			//general items
			lbDefaultNames.Items.Add(SeparatorResourceManager.GetSeparatorString(StringId.QuadrantBuffer));
            if (SeparatorResourceManager.isPlatformRS16())
            {
                lbDefaultNames.Items.Add(SeparatorResourceManager.GetSeparatorString(StringId.QuadrantBuffer34));
                lbDefaultNames.Items.Add(SeparatorResourceManager.GetSeparatorString(StringId.QuadrantBuffer56));
            }
            lbDefaultNames.Items.Add(SeparatorResourceManager.GetSeparatorString(StringId.WasteTube));

		    lbDefaultNames.Items.Add(SeparatorResourceManager.getLysisStringFromProtocolClass(protocolClass));
			
			lbDefaultNames.Items.Add(SeparatorResourceManager.getVialBStringFromProtocolClass(protocolClass));

			lbDefaultNames.Items.Add(SeparatorResourceManager.GetSeparatorString(StringId.VialA));
			lbDefaultNames.Items.Add(SeparatorResourceManager.GetSeparatorString(StringId.VialC));
			lbDefaultNames.Items.Add(SeparatorResourceManager.GetSeparatorString(StringId.SampleTube));
            lbDefaultNames.Items.Add(SeparatorResourceManager.GetSeparatorString(StringId.SeparationTube));


			lbDefaultNames.SelectedIndex = 0;
			this.SetupCustomListBox();
		}

		public void SetupCustomListBox()
		{
			lbCustomNames.Items.Clear();

			//general items
			lbCustomNames.Items.Add(bufferBottleC);

            
            if (SeparatorResourceManager.isPlatformRS16())
            {
                lbCustomNames.Items.Add(bufferBottle34C);
                lbCustomNames.Items.Add(bufferBottle56C);
            }

			lbCustomNames.Items.Add(wasteTubeC);
			lbCustomNames.Items.Add(lysisBufferTubeC);
			lbCustomNames.Items.Add(selectionCocktailVialC);
			lbCustomNames.Items.Add(magneticParticleVialC);
			lbCustomNames.Items.Add(antibodyCocktailVialC);
			lbCustomNames.Items.Add(sampleTubeC);
            lbCustomNames.Items.Add(separationTubeC);


//			lbCustomNames.SelectedIndex = 0;
		}


		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProtocolCustomize));
            this.txtCustomName = new Tesla.ProtocolEditorControls.ProtocolTextbox();
            this.btnAcceptNewName = new System.Windows.Forms.Button();
            this.tipDefaultName = new System.Windows.Forms.ToolTip(this.components);
            this.lblDefaultNames = new System.Windows.Forms.Label();
            this.tipNewName = new System.Windows.Forms.ToolTip(this.components);
            this.lblNewName = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.lbCustomNames = new System.Windows.Forms.ListBox();
            this.lbDefaultNames = new System.Windows.Forms.ListBox();
            this.tabQ1 = new System.Windows.Forms.TabPage();
            this.tabQ2 = new System.Windows.Forms.TabPage();
            this.tabQ3 = new System.Windows.Forms.TabPage();
            this.tabQ4 = new System.Windows.Forms.TabPage();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtCustomName
            // 
            this.txtCustomName.Location = new System.Drawing.Point(24, 235);
            this.txtCustomName.Name = "txtCustomName";
            this.txtCustomName.Size = new System.Drawing.Size(160, 20);
            this.txtCustomName.TabIndex = 1;
            // 
            // btnAcceptNewName
            // 
            this.btnAcceptNewName.Location = new System.Drawing.Point(208, 235);
            this.btnAcceptNewName.Name = "btnAcceptNewName";
            this.btnAcceptNewName.Size = new System.Drawing.Size(112, 23);
            this.btnAcceptNewName.TabIndex = 2;
            this.btnAcceptNewName.Text = "Accept New Name";
            this.btnAcceptNewName.Click += new System.EventHandler(this.btnAcceptNewName_Click);
            // 
            // lblDefaultNames
            // 
            this.lblDefaultNames.BackColor = System.Drawing.Color.Transparent;
            this.lblDefaultNames.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDefaultNames.Location = new System.Drawing.Point(8, 8);
            this.lblDefaultNames.Name = "lblDefaultNames";
            this.lblDefaultNames.Size = new System.Drawing.Size(112, 16);
            this.lblDefaultNames.TabIndex = 7;
            this.lblDefaultNames.Text = "Default Vial Name:";
            this.tipDefaultName.SetToolTip(this.lblDefaultNames, "Choose a default vial name to customize.");
            // 
            // lblNewName
            // 
            this.lblNewName.BackColor = System.Drawing.Color.Transparent;
            this.lblNewName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewName.Location = new System.Drawing.Point(24, 219);
            this.lblNewName.Name = "lblNewName";
            this.lblNewName.Size = new System.Drawing.Size(152, 16);
            this.lblNewName.TabIndex = 4;
            this.lblNewName.Text = "Change Custom Vial Name:";
            this.tipNewName.SetToolTip(this.lblNewName, "Type the custom vial name for the selected default vial.");
            this.lblNewName.Click += new System.EventHandler(this.lblNewName_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.lblDefaultNames);
            this.panel1.Controls.Add(this.lbCustomNames);
            this.panel1.Controls.Add(this.lbDefaultNames);
            this.panel1.Location = new System.Drawing.Point(24, 40);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(352, 176);
            this.panel1.TabIndex = 8;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(184, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 16);
            this.label1.TabIndex = 8;
            this.label1.Text = "Custom Vial Name:";
            // 
            // lbCustomNames
            // 
            this.lbCustomNames.Enabled = false;
            this.lbCustomNames.Location = new System.Drawing.Point(184, 24);
            this.lbCustomNames.Name = "lbCustomNames";
            this.lbCustomNames.Size = new System.Drawing.Size(160, 147);
            this.lbCustomNames.TabIndex = 6;
            // 
            // lbDefaultNames
            // 
            this.lbDefaultNames.Location = new System.Drawing.Point(8, 24);
            this.lbDefaultNames.Name = "lbDefaultNames";
            this.lbDefaultNames.Size = new System.Drawing.Size(160, 147);
            this.lbDefaultNames.TabIndex = 1;
            this.lbDefaultNames.SelectedIndexChanged += new System.EventHandler(this.lbDefaultNames_SelectedIndexChanged);
            // 
            // tabQ1
            // 
            this.tabQ1.Location = new System.Drawing.Point(4, 22);
            this.tabQ1.Name = "tabQ1";
            this.tabQ1.Size = new System.Drawing.Size(344, 0);
            this.tabQ1.TabIndex = 0;
            this.tabQ1.Text = "Q1";
            // 
            // tabQ2
            // 
            this.tabQ2.Location = new System.Drawing.Point(4, 22);
            this.tabQ2.Name = "tabQ2";
            this.tabQ2.Size = new System.Drawing.Size(344, 0);
            this.tabQ2.TabIndex = 1;
            this.tabQ2.Text = "Q2";
            // 
            // tabQ3
            // 
            this.tabQ3.Location = new System.Drawing.Point(4, 22);
            this.tabQ3.Name = "tabQ3";
            this.tabQ3.Size = new System.Drawing.Size(344, 0);
            this.tabQ3.TabIndex = 2;
            this.tabQ3.Text = "Q3";
            // 
            // tabQ4
            // 
            this.tabQ4.Location = new System.Drawing.Point(4, 22);
            this.tabQ4.Name = "tabQ4";
            this.tabQ4.Size = new System.Drawing.Size(344, 0);
            this.tabQ4.TabIndex = 3;
            this.tabQ4.Text = "Q4";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabQ1);
            this.tabControl1.Controls.Add(this.tabQ2);
            this.tabControl1.Controls.Add(this.tabQ3);
            this.tabControl1.Controls.Add(this.tabQ4);
            this.tabControl1.ItemSize = new System.Drawing.Size(42, 18);
            this.tabControl1.Location = new System.Drawing.Point(24, 16);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(352, 24);
            this.tabControl1.TabIndex = 7;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // ProtocolCustomize
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(394, 265);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.lblNewName);
            this.Controls.Add(this.btnAcceptNewName);
            this.Controls.Add(this.txtCustomName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProtocolCustomize";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Customize Vial Names";
            this.panel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void lbDefaultNames_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			UpdateTextbox();
		}

		private void UpdateTextbox(){
			StringId id =  SeparatorResourceManager.convertNameToStringID((string)lbDefaultNames.SelectedItem);
			switch (id)
			{
				case StringId.WasteTube:
					txtCustomName.Text = wasteTubeC;
					break;
				case StringId.SeparationTube:
					txtCustomName.Text = separationTubeC;
					break;
				case StringId.NegativeFractionTube:
				case StringId.LysisBufferTube:
				case StringId.LysisBufferNegativeFractionTube:
					txtCustomName.Text = lysisBufferTubeC;
					break;
				case StringId.VialA:
					txtCustomName.Text = magneticParticleVialC;
					break;
				case StringId.VialB:
				case StringId.VialBpos:
				case StringId.VialBneg:
					txtCustomName.Text = selectionCocktailVialC;
					break;
				case StringId.VialC:
					txtCustomName.Text = antibodyCocktailVialC;
					break;
				case StringId.SampleTube:
					txtCustomName.Text = sampleTubeC;
                    break;
                case StringId.QuadrantBuffer:
                    txtCustomName.Text = bufferBottleC;
                    break;
                case StringId.QuadrantBuffer34:
                    txtCustomName.Text = bufferBottle34C;
                    break;
                case StringId.QuadrantBuffer56:
                    txtCustomName.Text = bufferBottle56C;
                    break;
			}
		}

		private void btnAcceptNewName_Click(object sender, System.EventArgs e)
		{
			string newName = txtCustomName.Text;
            StringId id = SeparatorResourceManager.convertNameToStringID((string)lbDefaultNames.SelectedItem);
			switch (id)
			{
				case StringId.QuadrantBuffer:
					bufferBottleC = customVialNames.bufferBottle = newName;
					break;
				case StringId.WasteTube:
					wasteTubeC = customVialNames.wasteTube = newName;
					break;
				case StringId.NegativeFractionTube:
				case StringId.LysisBufferTube:
				case StringId.LysisBufferNegativeFractionTube:
					lysisBufferTubeC = customVialNames.lysisBufferTube = newName;
					break;
				case StringId.VialB:
				case StringId.VialBpos:
				case StringId.VialBneg:
					selectionCocktailVialC = customVialNames.selectionCocktailVial = newName;
					break;
				case StringId.VialA:
					magneticParticleVialC = customVialNames.magneticParticleVial = newName;
					break;
				case StringId.VialC:
					antibodyCocktailVialC = customVialNames.antibodyCocktailVial = newName;
					break;
				case StringId.SampleTube:
					sampleTubeC = customVialNames.sampleTube = newName;
					break;
				case StringId.SeparationTube:
					separationTubeC = customVialNames.separationTube = newName;
                    break;
                case StringId.QuadrantBuffer34:
                    bufferBottle34C = customVialNames.bufferBottle34 = newName;
                    break;
                case StringId.QuadrantBuffer56:
                    bufferBottle56C = customVialNames.bufferBottle56 = newName;
                    break;
			}
//			theProtocolModel.SetCustomNames(customNames);
			DbgView("btnAcceptNewName - calling ApplyCustomNames()");  //bdr

			theProtocolModel.ApplyCustomNames(tabControl1.SelectedIndex,customVialNames);
			this.SetupCustomListBox();
		}

		

		private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			SetupCustomNames(tabControl1.SelectedIndex);
			UpdateTextbox();
		}

		public void SetEnabledTotalQuad(int quads)
		{

			tabControl1.Controls.Clear();
			tabControl1.Controls.Add(tabQ1);
			if(quads>1)
				tabControl1.Controls.Add(tabQ2);
			if(quads>2)
				tabControl1.Controls.Add(tabQ3);
			if(quads>3)
				tabControl1.Controls.Add(tabQ4);
		}

		private void lblNewName_Click(object sender, System.EventArgs e)
		{
		
		}

		private void panel1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
		
		}

	}
}

