// RL - Uniform Multi Sample - 03/24/06

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Tesla.ProtocolEditorModel;

namespace Tesla.ProtocolEditor
{
	public class ProtocolMultipleSamples : System.Windows.Forms.Form
	{
		private static ProtocolModel theProtocolModel;

		private System.Windows.Forms.CheckBox Q1Sample;
		private System.Windows.Forms.CheckBox Q2Sample;
		private System.Windows.Forms.CheckBox Q3Sample;
		private System.Windows.Forms.CheckBox Q4Sample;
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;

		private bool[] multipleSamples = new bool[(int)Tesla.Common.Separator.QuadrantId.NUM_QUADRANTS];

		public ProtocolMultipleSamples()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// Get a local reference to the protocol model. 
			theProtocolModel = ProtocolModel.GetInstance();
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

		public void SetupMultipleSamples()
		{
			for(int i=0; i < multipleSamples.Length;i++)
			{
				multipleSamples[i] = false;
			}

			try
			{
				theProtocolModel.getMultipleSamples(out multipleSamples);
			}
			catch(Exception){}

			Q1Sample.Checked = true;
			Q2Sample.Checked = multipleSamples[1];
			Q3Sample.Checked = multipleSamples[2];
			Q4Sample.Checked = multipleSamples[3];
			
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProtocolMultipleSamples));
            this.Q1Sample = new System.Windows.Forms.CheckBox();
            this.Q2Sample = new System.Windows.Forms.CheckBox();
            this.Q3Sample = new System.Windows.Forms.CheckBox();
            this.Q4Sample = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Q1Sample
            // 
            this.Q1Sample.Checked = true;
            this.Q1Sample.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Q1Sample.Enabled = false;
            this.Q1Sample.Location = new System.Drawing.Point(72, 24);
            this.Q1Sample.Name = "Q1Sample";
            this.Q1Sample.Size = new System.Drawing.Size(144, 24);
            this.Q1Sample.TabIndex = 0;
            this.Q1Sample.Text = "Quadrant 1 Sample";
            // 
            // Q2Sample
            // 
            this.Q2Sample.Location = new System.Drawing.Point(72, 56);
            this.Q2Sample.Name = "Q2Sample";
            this.Q2Sample.Size = new System.Drawing.Size(144, 24);
            this.Q2Sample.TabIndex = 1;
            this.Q2Sample.Text = "Quadrant 2 Sample";
            // 
            // Q3Sample
            // 
            this.Q3Sample.Location = new System.Drawing.Point(72, 88);
            this.Q3Sample.Name = "Q3Sample";
            this.Q3Sample.Size = new System.Drawing.Size(144, 24);
            this.Q3Sample.TabIndex = 2;
            this.Q3Sample.Text = "Quadrant3 Sample";
            // 
            // Q4Sample
            // 
            this.Q4Sample.Location = new System.Drawing.Point(72, 120);
            this.Q4Sample.Name = "Q4Sample";
            this.Q4Sample.Size = new System.Drawing.Size(144, 24);
            this.Q4Sample.TabIndex = 3;
            this.Q4Sample.Text = "Quadrant 4 Sample";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(32, 168);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(184, 168);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ProtocolMultipleSamples
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(296, 208);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.Q4Sample);
            this.Controls.Add(this.Q3Sample);
            this.Controls.Add(this.Q2Sample);
            this.Controls.Add(this.Q1Sample);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProtocolMultipleSamples";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Multiple Samples Selection";
            this.ResumeLayout(false);

		}
		#endregion

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			multipleSamples[0] = Q1Sample.Checked;
			multipleSamples[1] = Q2Sample.Checked;
			multipleSamples[2] = Q3Sample.Checked;
			multipleSamples[3] = Q4Sample.Checked;
			theProtocolModel.setMultipleSamples(multipleSamples);
			this.Close();
		}
	}
}

