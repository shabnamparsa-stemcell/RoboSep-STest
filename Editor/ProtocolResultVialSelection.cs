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

namespace Tesla.ProtocolEditor
{
	/// <summary>
	/// Summary description for ProtocolResultVialSelection.
	/// </summary>
	public class ProtocolResultVialSelection : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabQ1;
		private System.Windows.Forms.TabPage tabQ2;
		private System.Windows.Forms.TabPage tabQ3;
		private System.Windows.Forms.TabPage tabQ4;
		private System.Windows.Forms.Button btnOk;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.CheckedListBox lbDefaultNames;

		private ProtocolClass protocolClass = ProtocolClass.HumanPositive;
		private static ProtocolModel theProtocolModel;
		private static int m_iLastIdx;

        private resultVialChecks results;

		public ProtocolResultVialSelection()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			theProtocolModel = ProtocolModel.GetInstance();

            results = new resultVialChecks();
           
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

			/*
			if ((protocolClass == ProtocolClass.Positive) || (protocolClass == ProtocolClass.HumanPositive) ||
				(protocolClass == ProtocolClass.MousePositive) || (protocolClass == ProtocolClass.WholeBloodPositive))
				lbDefaultNames.Items.Add(SeparatorResourceManager.GetSeparatorString(StringId.VialBpos));
			else if ((protocolClass == ProtocolClass.Negative) || (protocolClass == ProtocolClass.HumanNegative) ||
				(protocolClass == ProtocolClass.MouseNegative) || (protocolClass == ProtocolClass.WholeBloodNegative))
				lbDefaultNames.Items.Add(SeparatorResourceManager.GetSeparatorString(StringId.VialBneg));
			else
				lbDefaultNames.Items.Add(SeparatorResourceManager.GetSeparatorString(StringId.VialB));

			lbDefaultNames.Items.Add(SeparatorResourceManager.GetSeparatorString(StringId.VialA));
			lbDefaultNames.Items.Add(SeparatorResourceManager.GetSeparatorString(StringId.VialC));
             */ 
			lbDefaultNames.Items.Add(SeparatorResourceManager.GetSeparatorString(StringId.SampleTube));
			lbDefaultNames.Items.Add(SeparatorResourceManager.GetSeparatorString(StringId.SeparationTube));


			lbDefaultNames.SelectedIndex = 0;
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProtocolResultVialSelection));
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbDefaultNames = new System.Windows.Forms.CheckedListBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabQ1 = new System.Windows.Forms.TabPage();
            this.tabQ2 = new System.Windows.Forms.TabPage();
            this.tabQ3 = new System.Windows.Forms.TabPage();
            this.tabQ4 = new System.Windows.Forms.TabPage();
            this.btnOk = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.lbDefaultNames);
            this.panel1.Location = new System.Drawing.Point(24, 46);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 192);
            this.panel1.TabIndex = 12;
            // 
            // lbDefaultNames
            // 
            this.lbDefaultNames.CheckOnClick = true;
            this.lbDefaultNames.Location = new System.Drawing.Point(16, 8);
            this.lbDefaultNames.Name = "lbDefaultNames";
            this.lbDefaultNames.Size = new System.Drawing.Size(168, 169);
            this.lbDefaultNames.TabIndex = 2;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabQ1);
            this.tabControl1.Controls.Add(this.tabQ2);
            this.tabControl1.Controls.Add(this.tabQ3);
            this.tabControl1.Controls.Add(this.tabQ4);
            this.tabControl1.ItemSize = new System.Drawing.Size(42, 18);
            this.tabControl1.Location = new System.Drawing.Point(24, 22);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(200, 24);
            this.tabControl1.TabIndex = 11;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabQ1
            // 
            this.tabQ1.Location = new System.Drawing.Point(4, 22);
            this.tabQ1.Name = "tabQ1";
            this.tabQ1.Size = new System.Drawing.Size(192, 0);
            this.tabQ1.TabIndex = 0;
            this.tabQ1.Text = "Q1";
            // 
            // tabQ2
            // 
            this.tabQ2.Location = new System.Drawing.Point(4, 22);
            this.tabQ2.Name = "tabQ2";
            this.tabQ2.Size = new System.Drawing.Size(192, 0);
            this.tabQ2.TabIndex = 1;
            this.tabQ2.Text = "Q2";
            this.tabQ2.Visible = false;
            // 
            // tabQ3
            // 
            this.tabQ3.Location = new System.Drawing.Point(4, 22);
            this.tabQ3.Name = "tabQ3";
            this.tabQ3.Size = new System.Drawing.Size(192, 0);
            this.tabQ3.TabIndex = 2;
            this.tabQ3.Text = "Q3";
            this.tabQ3.Visible = false;
            // 
            // tabQ4
            // 
            this.tabQ4.Location = new System.Drawing.Point(4, 22);
            this.tabQ4.Name = "tabQ4";
            this.tabQ4.Size = new System.Drawing.Size(192, 0);
            this.tabQ4.TabIndex = 3;
            this.tabQ4.Text = "Q4";
            this.tabQ4.Visible = false;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(83, 256);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(80, 23);
            this.btnOk.TabIndex = 9;
            this.btnOk.Text = "Ok";
            // 
            // ProtocolResultVialSelection
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(250, 291);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProtocolResultVialSelection";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Result Vial Selection";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.ProtocolResultVialSelection_Closing);
            this.Load += new System.EventHandler(this.ProtocolResultVialSelection_Load);
            this.panel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void ProtocolResultVialSelection_Load(object sender, System.EventArgs e)
		{
			SetupListBox();		
			SetupCheckboxes(0);
			m_iLastIdx=0;
		}

		private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if(m_iLastIdx!=-1)
			{
				UpdateCheckboxes(m_iLastIdx);
				m_iLastIdx=tabControl1.SelectedIndex;
				SetupCheckboxes(m_iLastIdx);
			}
		}
		private void SetupCheckboxes(int idx)
		{
			//uncheck all
			IEnumerator myEnumerator;
			myEnumerator = lbDefaultNames.CheckedIndices.GetEnumerator();
			int i;
			while (myEnumerator.MoveNext() != false)
			{
				i =(int) myEnumerator.Current;
				lbDefaultNames.SetItemChecked(i, false);
			}

			//load checks
			//bool[] resultVialChecks;
			try
			{
                /*
                theProtocolModel.GetResultVialChecks(idx, out resultVialChecks);
				if (resultVialChecks != null)//set checks 
				{
					for(i=0;i<lbDefaultNames.Items.Count;++i)
					{
						lbDefaultNames.SetItemChecked(i, resultVialChecks[i]);
					}
				}
                 */

                results = theProtocolModel.GetResultVialChecks(idx);
                i=lbDefaultNames.Items.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.SampleTube));
                lbDefaultNames.SetItemChecked(i, results.sampleTube);
                i = lbDefaultNames.Items.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.SeparationTube));
                lbDefaultNames.SetItemChecked(i, results.separationTube);
                i = lbDefaultNames.Items.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.WasteTube));
                lbDefaultNames.SetItemChecked(i, results.wasteTube);
                i = lbDefaultNames.Items.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.QuadrantBuffer));
                lbDefaultNames.SetItemChecked(i, results.bufferBottle);
                i = lbDefaultNames.Items.IndexOf(SeparatorResourceManager.getLysisStringFromProtocolClass(protocolClass));
                lbDefaultNames.SetItemChecked(i, results.lysisBufferTube);

                //for rs16
                i = lbDefaultNames.Items.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.QuadrantBuffer34));
                lbDefaultNames.SetItemChecked(i, results.bufferBottle34);
                i = lbDefaultNames.Items.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.QuadrantBuffer56));
                lbDefaultNames.SetItemChecked(i, results.bufferBottle56);



                
			}
			catch(Exception){}


		}
		
		private void UpdateCheckboxes(int idx)
		{
			IEnumerator myEnumerator;
			int i;
			bool[] resultVialChecks = new bool[8];
			for(i=0;i<8;++i)
			{
				resultVialChecks[i]=false;
			}
			myEnumerator = lbDefaultNames.CheckedIndices.GetEnumerator();
			while (myEnumerator.MoveNext() != false)
			{
				i =(int) myEnumerator.Current;
				resultVialChecks[i]=true;
            }
            
            try
            {
            i = lbDefaultNames.Items.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.SampleTube));
            results.sampleTube = lbDefaultNames.GetItemCheckState(i)==CheckState.Checked;
            i = lbDefaultNames.Items.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.SeparationTube));
            results.separationTube = lbDefaultNames.GetItemCheckState(i) == CheckState.Checked;
            i = lbDefaultNames.Items.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.WasteTube));
            results.wasteTube = lbDefaultNames.GetItemCheckState(i) == CheckState.Checked;
            i = lbDefaultNames.Items.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.QuadrantBuffer));
            results.bufferBottle = lbDefaultNames.GetItemCheckState(i) == CheckState.Checked;
            i = lbDefaultNames.Items.IndexOf(SeparatorResourceManager.getLysisStringFromProtocolClass(protocolClass));
            results.lysisBufferTube = lbDefaultNames.GetItemCheckState(i) == CheckState.Checked;

            //for rs16
            i = lbDefaultNames.Items.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.QuadrantBuffer34));
            results.bufferBottle34 = lbDefaultNames.GetItemCheckState(i) == CheckState.Checked;
            i = lbDefaultNames.Items.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.QuadrantBuffer56));
            results.bufferBottle56 = lbDefaultNames.GetItemCheckState(i) == CheckState.Checked;
			
			}
			catch(Exception){}

			//resultVialChecks[0]=false;
			//lbDefaultNames.SetItemCheckState(0,CheckState.Unchecked);

            //theProtocolModel.UpdateResultVialChecks(idx, resultVialChecks);
            theProtocolModel.UpdateResultVialChecks(idx, results);
		}

		private void ProtocolResultVialSelection_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			UpdateCheckboxes(m_iLastIdx);
		
		}

		public void SetEnabledTotalQuad(int quads)
		{
			m_iLastIdx=-1;
			tabControl1.Controls.Clear();
			tabControl1.Controls.Add(tabQ1);
			if(quads>1)
				tabControl1.Controls.Add(tabQ2);
			if(quads>2)
				tabControl1.Controls.Add(tabQ3);
			if(quads>3)
				tabControl1.Controls.Add(tabQ4);
		}
	}
}
