//----------------------------------------------------------------------------
// VolumeMaintenanceCommandPanel
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
	public class VolumeMaintenanceCommandPanel : Tesla.ProtocolEditorControls.VolumeCommandPanel
	{
		private System.Windows.Forms.CheckBox chkVolumeMaintenanceCommandHomeFlag;
		private System.Windows.Forms.Label lblVolumeMaintenanceCommandHomeFlag;
		private System.ComponentModel.IContainer components = null;

		#region Construction/destruction

		public VolumeMaintenanceCommandPanel()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();
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

		public bool HomeFlag
		{
			get
			{
				return chkVolumeMaintenanceCommandHomeFlag.Checked;
			}
			set
			{
				chkVolumeMaintenanceCommandHomeFlag.Checked = value;
			}
		}

		#endregion Properties

		#region Data Entry Error Indicators

		private void chkVolumeMaintenanceCommandHomeFlag_CheckedChanged(object sender, System.EventArgs e)
		{
			ReportCommandDetailChanged();
		}

		private void VolumeMaintenanceCommandPanel_VisibleChanged(object sender, System.EventArgs e)
		{
			if (this.Visible)
			{
				// Trigger re-evaluation of the error providers
				base.VolumeCommandPanel_VisibleChanged(sender, e);
			}
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
            this.chkVolumeMaintenanceCommandHomeFlag = new System.Windows.Forms.CheckBox();
            this.lblVolumeMaintenanceCommandHomeFlag = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.errorAbsoluteVolume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorRelativeVolumeProportion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorcanttransport)).BeginInit();
            this.SuspendLayout();
            // 
            // lblVolumeCommandSourceVial
            // 
            this.lblVolumeCommandSourceVial.Location = new System.Drawing.Point(14, 65);
            this.lblVolumeCommandSourceVial.Visible = false;
            // 
            // lblVolumeCommandDestinationVial
            // 
            this.lblVolumeCommandDestinationVial.Location = new System.Drawing.Point(13, 89);
            this.lblVolumeCommandDestinationVial.Visible = false;
            // 
            // cmbSourceVial
            // 
            this.cmbSourceVial.Location = new System.Drawing.Point(77, 62);
            this.cmbSourceVial.Visible = false;
            // 
            // cmbDestinationVial
            // 
            this.cmbDestinationVial.Location = new System.Drawing.Point(77, 86);
            this.cmbDestinationVial.Visible = false;
            // 
            // cmbVolumeTipRack
            // 
            this.cmbVolumeTipRack.Location = new System.Drawing.Point(360, 86);
            this.cmbVolumeTipRack.Visible = false;
            // 
            // cbVolumeTipRack
            // 
            this.cbVolumeTipRack.Location = new System.Drawing.Point(338, 86);
            this.cbVolumeTipRack.Visible = false;
            // 
            // lblVolumeTipRack
            // 
            this.lblVolumeTipRack.Location = new System.Drawing.Point(285, 88);
            this.lblVolumeTipRack.Size = new System.Drawing.Size(56, 18);
            this.lblVolumeTipRack.Visible = false;
            // 
            // cbAbsoluteSpecified
            // 
            this.cbAbsoluteSpecified.Location = new System.Drawing.Point(263, 113);
            this.tipAbsolute.SetToolTip(this.cbAbsoluteSpecified, "Absolute addition.");
            // 
            // cbRelativeSpecified
            // 
            this.cbRelativeSpecified.Location = new System.Drawing.Point(123, 113);
            this.tipRelative.SetToolTip(this.cbRelativeSpecified, "Addition relative to sample volume.");
            // 
            // txtVolumeCommandAbsolute
            // 
            this.txtVolumeCommandAbsolute.Location = new System.Drawing.Point(408, 113);
            // 
            // lblVolumeCommandAbsolute
            // 
            this.lblVolumeCommandAbsolute.Location = new System.Drawing.Point(323, 114);
            // 
            // lblRelativeProportion
            // 
            this.lblRelativeProportion.Location = new System.Drawing.Point(13, 114);
            // 
            // lblAbsoluteVolume
            // 
            this.lblAbsoluteVolume.Location = new System.Drawing.Point(159, 113);
            this.lblAbsoluteVolume.Size = new System.Drawing.Size(101, 20);
            // 
            // chkVolumeMaintenanceCommandHomeFlag
            // 
            this.chkVolumeMaintenanceCommandHomeFlag.Location = new System.Drawing.Point(484, 84);
            this.chkVolumeMaintenanceCommandHomeFlag.Name = "chkVolumeMaintenanceCommandHomeFlag";
            this.chkVolumeMaintenanceCommandHomeFlag.Size = new System.Drawing.Size(16, 24);
            this.chkVolumeMaintenanceCommandHomeFlag.TabIndex = 11;
            this.chkVolumeMaintenanceCommandHomeFlag.CheckedChanged += new System.EventHandler(this.chkVolumeMaintenanceCommandHomeFlag_CheckedChanged);
            // 
            // lblVolumeMaintenanceCommandHomeFlag
            // 
            this.lblVolumeMaintenanceCommandHomeFlag.Location = new System.Drawing.Point(444, 88);
            this.lblVolumeMaintenanceCommandHomeFlag.Name = "lblVolumeMaintenanceCommandHomeFlag";
            this.lblVolumeMaintenanceCommandHomeFlag.Size = new System.Drawing.Size(44, 18);
            this.lblVolumeMaintenanceCommandHomeFlag.TabIndex = 10;
            this.lblVolumeMaintenanceCommandHomeFlag.Text = "Home";
            // 
            // VolumeMaintenanceCommandPanel
            // 
            this.Controls.Add(this.chkVolumeMaintenanceCommandHomeFlag);
            this.Controls.Add(this.lblVolumeMaintenanceCommandHomeFlag);
            this.Name = "VolumeMaintenanceCommandPanel";
            this.Size = new System.Drawing.Size(538, 156);
            this.VisibleChanged += new System.EventHandler(this.VolumeMaintenanceCommandPanel_VisibleChanged);
            this.Controls.SetChildIndex(this.txtVolumeCommandAbsolute, 0);
            this.Controls.SetChildIndex(this.txtVolumeCommandRelative, 0);
            this.Controls.SetChildIndex(this.lblVolumeCommandAbsolute, 0);
            this.Controls.SetChildIndex(this.lblVolumeCommandRelative, 0);
            this.Controls.SetChildIndex(this.cbAbsoluteSpecified, 0);
            this.Controls.SetChildIndex(this.cbRelativeSpecified, 0);
            this.Controls.SetChildIndex(this.lblRelativeProportion, 0);
            this.Controls.SetChildIndex(this.lblAbsoluteVolume, 0);
            this.Controls.SetChildIndex(this.lblVolumeTipRack, 0);
            this.Controls.SetChildIndex(this.lblVolumeCommandSourceVial, 0);
            this.Controls.SetChildIndex(this.lblVolumeCommandDestinationVial, 0);
            this.Controls.SetChildIndex(this.cmbSourceVial, 0);
            this.Controls.SetChildIndex(this.cmbDestinationVial, 0);
            this.Controls.SetChildIndex(this.cmbVolumeTipRack, 0);
            this.Controls.SetChildIndex(this.cbVolumeTipRack, 0);
            this.Controls.SetChildIndex(this.lblVolumeMaintenanceCommandHomeFlag, 0);
            this.Controls.SetChildIndex(this.chkVolumeMaintenanceCommandHomeFlag, 0);
            ((System.ComponentModel.ISupportInitialize)(this.errorAbsoluteVolume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorRelativeVolumeProportion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorcanttransport)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion		
	}
}

