//
// 2011-09-05 to 2011-09-16 sp various changes
//     - provide support for use in smaller screen displays (support for scrollbar in other files)
//     - align and resize panels for more unify displays  
//     - add checking for recommended volume levels and provide warnings
//     - add volume level thresholds (recommended and acceptable) using parameter file entries instead of fixed code  
//     - add checking of absolute volume levels  
//
//----------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Tesla.Common.ProtocolCommand;
using Tesla.Common.Protocol;
using Tesla.Common.Separator;
using Tesla.Common.ResourceManagement;

namespace Tesla.ProtocolEditorControls
{
	public class MixCommandPanel : Tesla.ProtocolEditorControls.VolumeCommandPanel
	{
		public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.ComboBox cbxMixCycles;
        public System.Windows.Forms.TextBox txtTipTubeBottomGap;
		private System.ComponentModel.IContainer components = null;

		public MixCommandPanel()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			this.lblVolumeCommandAbsolute.Text = "Mix Volume (uL)";

            UpdateMaxMixCycle();

			// TODO: Add any initialization after the InitializeComponent call
		}

        public void UpdateMaxMixCycle()
        {
            this.cbxMixCycles.Items.Clear();
            int max_cycles = (SeparatorResourceManager.isPlatformRS16()) ? MAX_MIX_CYCLE_RS16 : MAX_MIX_CYCLE_RSS;
            for (int i = 0; i < max_cycles; i++)
            {
                this.cbxMixCycles.Items.Add(i + 1);
            }
            this.cbxMixCycles.SelectedIndex = 0;
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

	
		private int myTipTubeBottomGap;

		public int TipTubeBottomGap
		{
			get
			{
				return myTipTubeBottomGap;
			}
			set
			{
				myTipTubeBottomGap = value;
				txtTipTubeBottomGap.Text = value.ToString();
			}
		}

		private int myMixCycles;

		public int MixCycles
		{
			get
			{
				return myMixCycles;
			}
			set
			{

				if(value>0)
				{
                    int num = (cbxMixCycles.Items.Count < value) ? cbxMixCycles.Items.Count : value;
                    myMixCycles = num;
                    cbxMixCycles.SelectedIndex = num - 1;
				}
			}
		}


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
            this.cbxMixCycles = new System.Windows.Forms.ComboBox();
            this.txtTipTubeBottomGap = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.errorAbsoluteVolume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorRelativeVolumeProportion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorcanttransport)).BeginInit();
            this.SuspendLayout();
            // 
            // lblVolumeCommandSourceVial
            // 
            this.lblVolumeCommandSourceVial.Location = new System.Drawing.Point(13, 61);
            // 
            // lblVolumeCommandDestinationVial
            // 
            this.lblVolumeCommandDestinationVial.Location = new System.Drawing.Point(13, 85);
            // 
            // cmbSourceVial
            // 
            this.cmbSourceVial.Location = new System.Drawing.Point(77, 58);
            // 
            // cmbDestinationVial
            // 
            this.cmbDestinationVial.Location = new System.Drawing.Point(77, 82);
            // 
            // cmbVolumeTipRack
            // 
            this.cmbVolumeTipRack.Location = new System.Drawing.Point(360, 82);
            // 
            // cbVolumeTipRack
            // 
            this.cbVolumeTipRack.Location = new System.Drawing.Point(340, 83);
            this.cbVolumeTipRack.Size = new System.Drawing.Size(21, 21);
            // 
            // lblVolumeTipRack
            // 
            this.lblVolumeTipRack.Location = new System.Drawing.Point(286, 85);
            this.lblVolumeTipRack.Size = new System.Drawing.Size(56, 18);
            // 
            // cbAbsoluteSpecified
            // 
            this.cbAbsoluteSpecified.Location = new System.Drawing.Point(263, 106);
            this.tipAbsolute.SetToolTip(this.cbAbsoluteSpecified, "Absolute addition.");
            this.cbAbsoluteSpecified.CheckedChanged += new System.EventHandler(this.cbAbsoluteSpecified_CheckedChanged);
            this.cbAbsoluteSpecified.Click += new System.EventHandler(this.cbAbsoluteSpecified_Click);
            // 
            // cbRelativeSpecified
            // 
            this.cbRelativeSpecified.Location = new System.Drawing.Point(122, 106);
            this.tipRelative.SetToolTip(this.cbRelativeSpecified, "Addition relative to sample volume.");
            // 
            // txtVolumeCommandAbsolute
            // 
            this.txtVolumeCommandAbsolute.Location = new System.Drawing.Point(408, 108);
            // 
            // lblVolumeCommandAbsolute
            // 
            this.lblVolumeCommandAbsolute.Location = new System.Drawing.Point(306, 108);
            this.lblVolumeCommandAbsolute.Size = new System.Drawing.Size(102, 20);
            this.lblVolumeCommandAbsolute.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.lblVolumeCommandAbsolute.Click += new System.EventHandler(this.lblVolumeCommandAbsolute_Click);
            // 
            // txtVolumeCommandRelative
            // 
            this.txtVolumeCommandRelative.Location = new System.Drawing.Point(408, 108);
            // 
            // lblVolumeCommandRelative
            // 
            this.lblVolumeCommandRelative.Location = new System.Drawing.Point(306, 108);
            this.lblVolumeCommandRelative.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            // 
            // lblRelativeProportion
            // 
            this.lblRelativeProportion.Location = new System.Drawing.Point(13, 110);
            // 
            // lblAbsoluteVolume
            // 
            this.lblAbsoluteVolume.Location = new System.Drawing.Point(148, 110);
            // 
            // cbxMixCycles
            // 
            this.cbxMixCycles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxMixCycles.Location = new System.Drawing.Point(227, 133);
            this.cbxMixCycles.Name = "cbxMixCycles";
            this.cbxMixCycles.Size = new System.Drawing.Size(52, 21);
            this.cbxMixCycles.TabIndex = 28;
            this.cbxMixCycles.SelectedIndexChanged += new System.EventHandler(this.cbxMixCycles_SelectedIndexChanged);
            // 
            // txtTipTubeBottomGap
            // 
            this.txtTipTubeBottomGap.Location = new System.Drawing.Point(408, 134);
            this.txtTipTubeBottomGap.Name = "txtTipTubeBottomGap";
            this.txtTipTubeBottomGap.Size = new System.Drawing.Size(116, 20);
            this.txtTipTubeBottomGap.TabIndex = 27;
            this.txtTipTubeBottomGap.TextChanged += new System.EventHandler(this.txtTipTubeBottomGap_TextChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(148, 136);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 16);
            this.label1.TabIndex = 26;
            this.label1.Text = "Mix Cycle(s)";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(306, 128);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 32);
            this.label2.TabIndex = 25;
            this.label2.Text = "Tip to Tube Bottom Gap (uL)";
            // 
            // MixCommandPanel
            // 
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtTipTubeBottomGap);
            this.Controls.Add(this.cbxMixCycles);
            this.Name = "MixCommandPanel";
            this.Size = new System.Drawing.Size(532, 156);
            this.Controls.SetChildIndex(this.cbxMixCycles, 0);
            this.Controls.SetChildIndex(this.txtTipTubeBottomGap, 0);
            this.Controls.SetChildIndex(this.label1, 0);
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
            this.Controls.SetChildIndex(this.label2, 0);
            ((System.ComponentModel.ISupportInitialize)(this.errorAbsoluteVolume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorRelativeVolumeProportion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorcanttransport)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void cbAbsoluteSpecified_CheckedChanged(object sender, System.EventArgs e)
		{
			txtTipTubeBottomGap.Enabled=false;
            if (cbAbsoluteSpecified.Checked)
            {
                txtTipTubeBottomGap.Enabled = true;
            }
		}

        private void cbAbsoluteSpecified_Click(object sender, EventArgs e)
        {
            if (cbAbsoluteSpecified.Checked &&  !isVialStringSmallVial((string)cmbSourceVial.SelectedItem))
            {
                MessageBox.Show("Recommend Relative instead of Absolute mix volume.", "Protocol Warning");
            }
        }

		private void cbxMixCycles_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			MixCycles=cbxMixCycles.SelectedIndex+1;
			ReportCommandDetailChanged();
		}

		private void txtTipTubeBottomGap_TextChanged(object sender, System.EventArgs e)
		{ 
			try
			{
				TipTubeBottomGap=int.Parse(txtTipTubeBottomGap.Text);
			}
			catch
			{
				TipTubeBottomGap=0;
			}
			ReportCommandDetailChanged();
		}

		public override bool IsContentValid()
		{
			ClearAbsoluteVolumeError();
			bool isContentValid = base.IsContentValid();
			bool result;
			if (VolumeTypeSpecifier == VolumeType.Absolute )
            {
                VolumeLimits volumeLimits = VolumeLimits.GetInstance();
                volumeLimits.UpateLiquidVolumeLimits();
                double minimumAllowable_TipVolume_ul = (double)0;
                double maximumCapacity_ReagentTipVolume_ul = (double)volumeLimits.maximumCapacity_ReagentTipVolume_ul;
                double maximumCapacity_SampleTipVolume_ul = (double)volumeLimits.maximumCapacity_SampleTipVolume_ul;

                if (AbsoluteVolume_uL < minimumAllowable_TipVolume_ul)
                {
                    ShowAbsoluteVolumeError(VolumeError.MINIMUM_AMOUNT);
                    result = false;
                }
                else if ((cmbSourceVial.Text.IndexOf(
                    SeparatorResourceManager.GetSeparatorString(StringId.VialA)) >= 0) ||
                    (cmbSourceVial.Text.IndexOf(
                    SeparatorResourceManager.GetSeparatorString(StringId.VialB)) >= 0) ||
                    (cmbSourceVial.Text.IndexOf(
                    SeparatorResourceManager.GetSeparatorString(StringId.VialC)) >= 0)
                    )
                {
                    result = AbsoluteVolume_uL <= maximumCapacity_ReagentTipVolume_ul;
                    if (!result)
                        ShowAbsoluteVolumeError(VolumeError.ABSOLUTE_1MLTIP_AMOUNT);
                }
                else
                {
                    result = AbsoluteVolume_uL <= maximumCapacity_SampleTipVolume_ul;
                    if (!result)
                        ShowAbsoluteVolumeError(VolumeError.ABSOLUTE_5MLTIP_AMOUNT);
                }

                isContentValid &= result;
			}
			return isContentValid;
		}

        private void txtCommandType_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblVolumeCommandAbsolute_Click(object sender, EventArgs e)
        {

        }

	}
}

