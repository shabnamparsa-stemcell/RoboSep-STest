//----------------------------------------------------------------------------
// WorkingVolumeCommandPanel
//
// Invetech Pty Ltd
// Victoria, 3149, Australia
// Phone:   (+61) 3 9211 7700
// Fax:     (+61) 3 9211 7701
//
// The copyright to the computer program(s) herein is the property of 
// Invetech Pty Ltd, Australia.
// The program(s) may be used and/or copied only with the written permission 
// of Invetech Pty Ltd or in accordance with the terms and conditions 
// stipulated in the agreement/contract under which the program(s)
// have been supplied.
// 
// Copyright © 2004. All Rights Reserved.
//
//----------------------------------------------------------------------------
//
// 2011-09-05 to 2011-09-16 sp various changes
//     - provide support for use in smaller screen displays (support for scrollbar in other files)
//     - align and resize panels for more unify displays  
//
//----------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Tesla.ProtocolEditorControls
{
	public class WorkingVolumeCommandPanel : Tesla.ProtocolEditorControls.VolumeCommandPanel
	{
		private System.Windows.Forms.Label lblVolumeCommandFreeAirDispense;
		private System.Windows.Forms.CheckBox chkVolumeCommandFreeAirDispense;
		private System.Windows.Forms.ErrorProvider errorWorkingVolume;
		private System.Windows.Forms.CheckBox chkVolumeCommandUseBufferTip;
		private System.Windows.Forms.Label lblVolumeCommandUseBufferTip;
		private System.ComponentModel.IContainer components = null;

		private System.Windows.Forms.ToolTip tipFreeAir;
		private System.Windows.Forms.ToolTip tipUseBufferTip;

		#region Construction/destruction

		public WorkingVolumeCommandPanel()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			//setup tool tip
			tipFreeAir.SetToolTip(this.lblVolumeCommandFreeAirDispense, "If checked, air volume will be added.");
			tipUseBufferTip.SetToolTip(this.lblVolumeCommandUseBufferTip, "If checked, the mixing tip will be re-used for transport.");

			UseBufferTip = base.UseBufferTip;
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

		#region Properties

		public bool FreeAirDispense
		{
			get
			{
				return chkVolumeCommandFreeAirDispense.Checked;
			}
			set
			{
				chkVolumeCommandFreeAirDispense.Checked = value;
			}
		}

		public bool UseBufferTip
		{
			get
			{
				return (chkVolumeCommandUseBufferTip.Enabled && 
						chkVolumeCommandUseBufferTip.Checked);
			}
			set
			{
				chkVolumeCommandUseBufferTip.Checked = value;
			}
		}

		#endregion Properties

		#region Data Entry Error Indicators

		public bool CheckTransportSourceVial()
		{
			bool flg = true;

			// override show for topup, mix or small vials (1ml tips)
			if (this.Visible)
			{ 
				string src = SourceVial.ToString();
				
				if ((this.CommandType != "Transport") || (src == "TPC0001") ||
				    src.EndsWith("03") || src.EndsWith("04") || src.EndsWith("05")) 
					flg = false;
				
				if (flg == false)
					chkVolumeCommandUseBufferTip.Checked = false;
				lblVolumeCommandUseBufferTip.Enabled = flg;
				chkVolumeCommandUseBufferTip.Enabled = flg;
			}
			return flg;
		}

		private void chkVolumeCommandFreeAirDispense_CheckedChanged(object sender, System.EventArgs e)
		{
			ReportCommandDetailChanged();
		}

		private void chkVolumeCommandUseBufferTip_CheckedChanged(object sender, System.EventArgs e)
		{
			ReportCommandDetailChanged();		
		}


		//      TPC0101, //WasteVial
		//      TPC0102, //LysisVial
		//      TPC0103, //CocktailVial
		//      TPC0104, //ParticleVial
		//      TPC0105, //AntibodyVial
		//      TPC0106, //SampleVial
		//      TPC0107, //SeparationVial

		private void WorkingVolumeCommandPanel_VisibleChanged(object sender, System.EventArgs e)
		{
			if (this.Visible)
			{
				// Trigger re-evaluation of the error providers
				base.VolumeCommandPanel_VisibleChanged(sender, e);
				
				// override show for topup, mix or small vials (1ml tips)
				bool show = CheckTransportSourceVial();
				if (show == false)
					chkVolumeCommandUseBufferTip.Checked = false;
				lblVolumeCommandUseBufferTip.Enabled = show;
				chkVolumeCommandUseBufferTip.Enabled = show;
			}
		}

		// on usebuffertip checked, show warning message and get user confirm
		private void chkVolumeCommandUseBufferTip_Click(object sender, System.EventArgs e)
		{
			// user has set usebuffertip - verify
			if ( chkVolumeCommandUseBufferTip.Checked == true ) {

				string msg = "Please note that the Use Buffer Tip checkbox is checked.  This can " +  
					"result in buffer tip contamination with the sample. This feature is intended " +
					"for the final transport step in single-quadrant negative selections. Please " +
					"be sure that this is the case for this protocol.";

				DialogResult bufferTipWarning = MessageBox.Show(this, msg, "Transport Tip Override", 
					MessageBoxButtons.OK, MessageBoxIcon.Warning);			
				ReportCommandDetailChanged();
			}
			// user has cancelled usebuffertip
			else ReportCommandDetailChanged();
		}


		#endregion Data Entry Error Indicators

        // 2011-09-08 to 2011-09-16 sp various changes
        //     - provide support for use in smaller screen displays (support for scrollbar in other files)
        //     - align and resize panels for more unify displays  
        #region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.lblVolumeCommandFreeAirDispense = new System.Windows.Forms.Label();
            this.chkVolumeCommandFreeAirDispense = new System.Windows.Forms.CheckBox();
            this.errorWorkingVolume = new System.Windows.Forms.ErrorProvider(this.components);
            this.tipFreeAir = new System.Windows.Forms.ToolTip(this.components);
            this.chkVolumeCommandUseBufferTip = new System.Windows.Forms.CheckBox();
            this.lblVolumeCommandUseBufferTip = new System.Windows.Forms.Label();
            this.tipUseBufferTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorAbsoluteVolume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorRelativeVolumeProportion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorcanttransport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorWorkingVolume)).BeginInit();
            this.SuspendLayout();
            // 
            // lblVolumeCommandSourceVial
            // 
            this.lblVolumeCommandSourceVial.Location = new System.Drawing.Point(12, 61);
            // 
            // lblVolumeCommandDestinationVial
            // 
            this.lblVolumeCommandDestinationVial.Location = new System.Drawing.Point(12, 85);
            // 
            // cmbSourceVial
            // 
            this.cmbSourceVial.Location = new System.Drawing.Point(76, 58);
            this.cmbSourceVial.Size = new System.Drawing.Size(203, 21);
            // 
            // cmbDestinationVial
            // 
            this.cmbDestinationVial.Location = new System.Drawing.Point(76, 82);
            this.cmbDestinationVial.Size = new System.Drawing.Size(203, 21);
            // 
            // cmbVolumeTipRack
            // 
            this.cmbVolumeTipRack.Location = new System.Drawing.Point(366, 58);
            this.cmbVolumeTipRack.Size = new System.Drawing.Size(41, 21);
            // 
            // cbVolumeTipRack
            // 
            this.cbVolumeTipRack.Location = new System.Drawing.Point(344, 58);
            // 
            // lblVolumeTipRack
            // 
            this.lblVolumeTipRack.Location = new System.Drawing.Point(285, 59);
            this.lblVolumeTipRack.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbAbsoluteSpecified
            // 
            this.cbAbsoluteSpecified.Location = new System.Drawing.Point(263, 108);
            this.tipAbsolute.SetToolTip(this.cbAbsoluteSpecified, "Absolute addition.");
            // 
            // cbRelativeSpecified
            // 
            this.cbRelativeSpecified.Location = new System.Drawing.Point(123, 108);
            this.tipRelative.SetToolTip(this.cbRelativeSpecified, "Addition relative to sample volume.");
            // 
            // txtVolumeCommandAbsolute
            // 
            this.txtVolumeCommandAbsolute.Location = new System.Drawing.Point(408, 108);
            // 
            // lblVolumeCommandAbsolute
            // 
            this.lblVolumeCommandAbsolute.Location = new System.Drawing.Point(309, 114);
            this.lblVolumeCommandAbsolute.Size = new System.Drawing.Size(92, 20);
            this.lblVolumeCommandAbsolute.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            // 
            // txtVolumeCommandRelative
            // 
            this.txtVolumeCommandRelative.Location = new System.Drawing.Point(408, 108);
            // 
            // lblVolumeCommandRelative
            // 
            this.lblVolumeCommandRelative.Location = new System.Drawing.Point(310, 114);
            this.lblVolumeCommandRelative.Size = new System.Drawing.Size(92, 20);
            this.lblVolumeCommandRelative.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            // 
            // lblRelativeProportion
            // 
            this.lblRelativeProportion.Location = new System.Drawing.Point(12, 113);
            this.lblRelativeProportion.Size = new System.Drawing.Size(105, 20);
            // 
            // lblAbsoluteVolume
            // 
            this.lblAbsoluteVolume.Location = new System.Drawing.Point(147, 110);
            this.lblAbsoluteVolume.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblVolumeCommandFreeAirDispense
            // 
            this.lblVolumeCommandFreeAirDispense.Location = new System.Drawing.Point(441, 52);
            this.lblVolumeCommandFreeAirDispense.Name = "lblVolumeCommandFreeAirDispense";
            this.lblVolumeCommandFreeAirDispense.Size = new System.Drawing.Size(55, 34);
            this.lblVolumeCommandFreeAirDispense.TabIndex = 16;
            this.lblVolumeCommandFreeAirDispense.Text = "Free Air Dispense ";
            // 
            // chkVolumeCommandFreeAirDispense
            // 
            this.chkVolumeCommandFreeAirDispense.Location = new System.Drawing.Point(507, 56);
            this.chkVolumeCommandFreeAirDispense.Name = "chkVolumeCommandFreeAirDispense";
            this.chkVolumeCommandFreeAirDispense.Size = new System.Drawing.Size(16, 24);
            this.chkVolumeCommandFreeAirDispense.TabIndex = 17;
            this.chkVolumeCommandFreeAirDispense.CheckedChanged += new System.EventHandler(this.chkVolumeCommandFreeAirDispense_CheckedChanged);
            // 
            // errorWorkingVolume
            // 
            this.errorWorkingVolume.ContainerControl = this;
            // 
            // tipFreeAir
            // 
            this.tipFreeAir.AutoPopDelay = 5000;
            this.tipFreeAir.InitialDelay = 500;
            this.tipFreeAir.ReshowDelay = 100;
            // 
            // chkVolumeCommandUseBufferTip
            // 
            this.chkVolumeCommandUseBufferTip.Location = new System.Drawing.Point(391, 82);
            this.chkVolumeCommandUseBufferTip.Name = "chkVolumeCommandUseBufferTip";
            this.chkVolumeCommandUseBufferTip.Size = new System.Drawing.Size(16, 24);
            this.chkVolumeCommandUseBufferTip.TabIndex = 25;
            this.chkVolumeCommandUseBufferTip.CheckedChanged += new System.EventHandler(this.chkVolumeCommandUseBufferTip_CheckedChanged);
            this.chkVolumeCommandUseBufferTip.Click += new System.EventHandler(this.chkVolumeCommandUseBufferTip_Click);
            // 
            // lblVolumeCommandUseBufferTip
            // 
            this.lblVolumeCommandUseBufferTip.Location = new System.Drawing.Point(285, 82);
            this.lblVolumeCommandUseBufferTip.Name = "lblVolumeCommandUseBufferTip";
            this.lblVolumeCommandUseBufferTip.Size = new System.Drawing.Size(80, 20);
            this.lblVolumeCommandUseBufferTip.TabIndex = 26;
            this.lblVolumeCommandUseBufferTip.Text = "Use Buffer Tip";
            this.lblVolumeCommandUseBufferTip.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // WorkingVolumeCommandPanel
            // 
            this.Controls.Add(this.lblVolumeCommandUseBufferTip);
            this.Controls.Add(this.chkVolumeCommandUseBufferTip);
            this.Controls.Add(this.chkVolumeCommandFreeAirDispense);
            this.Controls.Add(this.lblVolumeCommandFreeAirDispense);
            this.Name = "WorkingVolumeCommandPanel";
            this.VisibleChanged += new System.EventHandler(this.WorkingVolumeCommandPanel_VisibleChanged);
            this.Controls.SetChildIndex(this.txtVolumeCommandAbsolute, 0);
            this.Controls.SetChildIndex(this.txtVolumeCommandRelative, 0);
            this.Controls.SetChildIndex(this.lblVolumeCommandAbsolute, 0);
            this.Controls.SetChildIndex(this.lblVolumeCommandRelative, 0);
            this.Controls.SetChildIndex(this.lblVolumeCommandSourceVial, 0);
            this.Controls.SetChildIndex(this.lblVolumeCommandDestinationVial, 0);
            this.Controls.SetChildIndex(this.cmbSourceVial, 0);
            this.Controls.SetChildIndex(this.cmbDestinationVial, 0);
            this.Controls.SetChildIndex(this.cmbVolumeTipRack, 0);
            this.Controls.SetChildIndex(this.cbVolumeTipRack, 0);
            this.Controls.SetChildIndex(this.lblVolumeTipRack, 0);
            this.Controls.SetChildIndex(this.cbAbsoluteSpecified, 0);
            this.Controls.SetChildIndex(this.cbRelativeSpecified, 0);
            this.Controls.SetChildIndex(this.lblRelativeProportion, 0);
            this.Controls.SetChildIndex(this.lblAbsoluteVolume, 0);
            this.Controls.SetChildIndex(this.lblVolumeCommandFreeAirDispense, 0);
            this.Controls.SetChildIndex(this.chkVolumeCommandFreeAirDispense, 0);
            this.Controls.SetChildIndex(this.chkVolumeCommandUseBufferTip, 0);
            this.Controls.SetChildIndex(this.lblVolumeCommandUseBufferTip, 0);
            ((System.ComponentModel.ISupportInitialize)(this.errorAbsoluteVolume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorRelativeVolumeProportion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorcanttransport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorWorkingVolume)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion
	}
}

