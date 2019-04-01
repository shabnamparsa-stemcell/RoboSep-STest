

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using Tesla.ProtocolEditorModel;
using Tesla.Common.Protocol;
using Tesla.Common.Separator;
using Tesla.Common.ProtocolCommand;

namespace Tesla.ProtocolEditorControls
{
	/// <summary>
	/// Summary description for CommandPanel.
	/// </summary>
    public class GenericMultiStepCommandPanel2 : Tesla.ProtocolEditorControls.MixCommandPanel
    {
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage3;
        public Label lblAbsoluteVolume2;
        public Label lblRelativeProportion2;
        public CheckBox cbRelativeSpecified2;
        public CheckBox cbAbsoluteSpecified2;
        public ComboBox cmbDestinationVial2;
        public ComboBox cmbSourceVial2;
        public Label label11;
        public Label label12;
        public Label lblVolumeCommandRelative2;
        public Label lblVolumeCommandAbsolute2;
        public TextBox txtVolumeCommandAbsolute2;
        public TextBox txtVolumeCommandRelative2;
        public ErrorProvider errorRelativeVolumeProportion2;
        private IContainer components;
        public ErrorProvider errorAbsoluteVolume2;
        public ErrorProvider errorcanttransport2;

        int idxTrans1;
		

        public bool boNotReady = true;

        #region Construction/destruction

        public override bool IsContentValid()
        {
            int idxWithErr = -1;
            bool isContentValid = base.IsContentValid();
            if (idxWithErr == -1 && !isContentValid) idxWithErr = 0;


            isContentValid = IsContentValid_helper(isContentValid, myVolumeTypeSpecifier2,
                txtVolumeCommandRelative2, myRelativeVolumeProportion2, errorRelativeVolumeProportion2,
                txtVolumeCommandAbsolute2, myAbsoluteVolume_uL2, errorAbsoluteVolume2, isVolumeSpecificationRequired2);
            if (idxWithErr == -1 && !isContentValid) idxWithErr = idxTrans1;


            if (!isContentValid)
            {
                if (idxWithErr == -1)
                {
                    System.Diagnostics.Debug.WriteLine("Fix generic multistep command panel .iscontentvalid()");
                }
                this.tabControl1.SelectedIndex = idxWithErr;
            }

            return isContentValid;
        }


        public GenericMultiStepCommandPanel2()
		{
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            idxTrans1=1;
		
            isVolumeSpecificationRequired2 = true;

            ComboBox[] boxes = { cmbSourceVial, cmbDestinationVial, cmbSourceVial2, cmbDestinationVial2 };
            FillComboBoxesWithVialNames(boxes);


            cmbDestinationVial.Enabled = false;
            cmbSourceVial2.Enabled = false;


            boNotReady = false;
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			



			base.Dispose( disposing );
		}	
	
        #endregion Construction/destruction

        public override void UpdateComboBoxes(ProtocolClass pc)
        {
            ComboBox[] boxes = { cmbSourceVial, cmbDestinationVial, cmbSourceVial2, cmbDestinationVial2};
            UpdateComboBoxes(boxes, pc);
        }

       

        public AbsoluteResourceLocation SourceVial2
        {
            get
            {
                return ComboIndexToVialID(cmbSourceVial2.SelectedIndex);
            }
            set
            {
                int idx = VialIDToComboIndex(value);
                if (idx != -1 && idx < cmbSourceVial2.Items.Count) cmbSourceVial2.SelectedIndex = idx;
            }
        }

        public AbsoluteResourceLocation DestinationVial2
        {
            get
            {
                return ComboIndexToVialID(cmbDestinationVial2.SelectedIndex);
            }
            set
            {

                int idx = VialIDToComboIndex(value);
                if (idx != -1 && idx < cmbDestinationVial2.Items.Count) cmbDestinationVial2.SelectedIndex = idx;
            }
        }


        
        private bool isVolumeSpecificationRequired2;

        public bool IsVolumeSpecificationRequired2
        {
            get
            {
                return isVolumeSpecificationRequired2;
            }
            set
            {
                isVolumeSpecificationRequired2 = value;

            }
        }

        
        private VolumeType myVolumeTypeSpecifier2;

        public VolumeType VolumeTypeSpecifier2
        {
            get
            {
                return myVolumeTypeSpecifier2;
            }
            set
            {
                myVolumeTypeSpecifier2 = value;

                SetCtrlFromVolumeType(value, cbRelativeSpecified2, lblVolumeCommandRelative2, txtVolumeCommandRelative2,
                    cbAbsoluteSpecified2, lblVolumeCommandAbsolute2, txtVolumeCommandAbsolute2);


                this.Refresh();
            }
        }


        
        private double myRelativeVolumeProportion2;

        public double RelativeVolumeProportion2
        {
            get
            {
                return myRelativeVolumeProportion2;
            }
            set
            {
                txtVolumeCommandRelative2.Text = value.ToString("F4");
            }
        }

        private int myAbsoluteVolume_uL2;

        public int AbsoluteVolume_uL2
        {
            get
            {
                return myAbsoluteVolume_uL2;
            }
            set
            {
                txtVolumeCommandAbsolute2.Text = value.ToString();
            }
        }

       

        public void ShowRelativeVolumeProportion2Error(VolumeError errorVol)
        {
            ShowRelativeVolumeProportionError(errorVol, errorRelativeVolumeProportion2, txtVolumeCommandRelative2);
        }

        public void ClearRelativeVolumeProportion2Error()
        {
            errorRelativeVolumeProportion2.SetError(txtVolumeCommandRelative2, string.Empty);
        }


        public void ShowCantTransportError2()
        {
            errorcanttransport2.SetError(
                cmbDestinationVial2, "Can't do transport to the specified vial");
        }

        public void ClearCantTransportError2()
        {
            errorcanttransport2.SetError(cmbDestinationVial2, string.Empty);
        }

        protected void setEnabled2(bool enab)
        {
            setEnabled_helper(enab,false,cbRelativeSpecified2,cbAbsoluteSpecified2,
                txtVolumeCommandRelative2,lblRelativeProportion2,
                txtVolumeCommandAbsolute2,lblAbsoluteVolume2);
        }

        
        public bool CheckTransportDestVial2()
        {
            return CheckTransportDestVial_helper(cmbDestinationVial2, true, setEnabled2, ShowCantTransportError2, ClearCantTransportError2);
        }
        

        public void ShowAbsoluteVolumeError2(VolumeError errorVol)
        {
            ShowAbsoluteVolumeError(errorVol, errorAbsoluteVolume2, txtVolumeCommandAbsolute2);
        }

        public void ClearAbsoluteVolumeError2()
        {
            errorAbsoluteVolume2.SetError(txtVolumeCommandAbsolute2, string.Empty);
        }


        public void SetVolumeCommandPanelParams2(AbsoluteResourceLocation SourceVial, AbsoluteResourceLocation DestinationVial,
            VolumeType VolumeTypeSpecifier, double RelativeVolumeProportion, int AbsoluteVolume_uL)
        {
            this.SourceVial2 = SourceVial;
            this.DestinationVial2 = DestinationVial;
            this.VolumeTypeSpecifier2 = VolumeTypeSpecifier;


            this.RelativeVolumeProportion2 = RelativeVolumeProportion;
            this.AbsoluteVolume_uL2 = AbsoluteVolume_uL;
        }

       
	
	
	

        protected override void OnPaint(PaintEventArgs e)
        {



            base.OnPaint(e);	
        }

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.lblAbsoluteVolume2 = new System.Windows.Forms.Label();
            this.lblRelativeProportion2 = new System.Windows.Forms.Label();
            this.cbRelativeSpecified2 = new System.Windows.Forms.CheckBox();
            this.cbAbsoluteSpecified2 = new System.Windows.Forms.CheckBox();
            this.cmbDestinationVial2 = new System.Windows.Forms.ComboBox();
            this.cmbSourceVial2 = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txtVolumeCommandRelative2 = new System.Windows.Forms.TextBox();
            this.lblVolumeCommandRelative2 = new System.Windows.Forms.Label();
            this.lblVolumeCommandAbsolute2 = new System.Windows.Forms.Label();
            this.txtVolumeCommandAbsolute2 = new System.Windows.Forms.TextBox();
            this.errorRelativeVolumeProportion2 = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorAbsoluteVolume2 = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorcanttransport2 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorAbsoluteVolume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorRelativeVolumeProportion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorcanttransport)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorRelativeVolumeProportion2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorAbsoluteVolume2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorcanttransport2)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(400, 6);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(279, 31);
            this.label2.Size = new System.Drawing.Size(125, 16);
            // 
            // cbxMixCycles
            // 
            this.cbxMixCycles.Location = new System.Drawing.Point(468, 4);
            // 
            // txtTipTubeBottomGap
            // 
            this.txtTipTubeBottomGap.Location = new System.Drawing.Point(403, 29);
            // 
            // lblVolumeCommandSourceVial
            // 
            this.lblVolumeCommandSourceVial.Location = new System.Drawing.Point(5, 7);
            this.lblVolumeCommandSourceVial.TabIndex = 25;
            // 
            // lblVolumeCommandDestinationVial
            // 
            this.lblVolumeCommandDestinationVial.Location = new System.Drawing.Point(5, 31);
            this.lblVolumeCommandDestinationVial.TabIndex = 27;
            // 
            // cmbSourceVial
            // 
            this.cmbSourceVial.Location = new System.Drawing.Point(69, 4);
            this.cmbSourceVial.TabIndex = 26;
            // 
            // cmbDestinationVial
            // 
            this.cmbDestinationVial.Location = new System.Drawing.Point(69, 28);
            this.cmbDestinationVial.TabIndex = 28;
            // 
            // cmbVolumeTipRack
            // 
            this.cmbVolumeTipRack.Location = new System.Drawing.Point(352, 4);
            this.cmbVolumeTipRack.TabIndex = 34;
            // 
            // cbVolumeTipRack
            // 
            this.cbVolumeTipRack.Location = new System.Drawing.Point(330, 2);
            this.cbVolumeTipRack.TabIndex = 35;
            // 
            // lblVolumeTipRack
            // 
            this.lblVolumeTipRack.Location = new System.Drawing.Point(276, 6);
            this.lblVolumeTipRack.TabIndex = 36;
            // 
            // cbAbsoluteSpecified
            // 
            this.cbAbsoluteSpecified.Location = new System.Drawing.Point(255, 55);
            this.cbAbsoluteSpecified.TabIndex = 29;
            this.tipAbsolute.SetToolTip(this.cbAbsoluteSpecified, "Absolute addition.");
            // 
            // cbRelativeSpecified
            // 
            this.cbRelativeSpecified.Location = new System.Drawing.Point(115, 55);
            this.cbRelativeSpecified.TabIndex = 37;
            this.tipRelative.SetToolTip(this.cbRelativeSpecified, "Addition relative to sample volume.");
            // 
            // txtVolumeCommandAbsolute
            // 
            this.txtVolumeCommandAbsolute.Location = new System.Drawing.Point(403, 55);
            this.txtVolumeCommandAbsolute.TabIndex = 33;
            // 
            // lblVolumeCommandAbsolute
            // 
            this.lblVolumeCommandAbsolute.Location = new System.Drawing.Point(314, 55);
            this.lblVolumeCommandAbsolute.TabIndex = 31;
            // 
            // txtVolumeCommandRelative
            // 
            this.txtVolumeCommandRelative.Location = new System.Drawing.Point(403, 55);
            this.txtVolumeCommandRelative.TabIndex = 32;
            // 
            // lblVolumeCommandRelative
            // 
            this.lblVolumeCommandRelative.Location = new System.Drawing.Point(314, 55);
            this.lblVolumeCommandRelative.TabIndex = 30;
            // 
            // lblRelativeProportion
            // 
            this.lblRelativeProportion.Location = new System.Drawing.Point(6, 60);
            this.lblRelativeProportion.TabIndex = 38;
            // 
            // lblAbsoluteVolume
            // 
            this.lblAbsoluteVolume.Location = new System.Drawing.Point(137, 60);
            this.lblAbsoluteVolume.TabIndex = 39;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(0, 50);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(538, 108);
            this.tabControl1.TabIndex = 8;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.txtVolumeCommandAbsolute);
            this.tabPage1.Controls.Add(this.cbVolumeTipRack);
            this.tabPage1.Controls.Add(this.lblAbsoluteVolume);
            this.tabPage1.Controls.Add(this.lblRelativeProportion);
            this.tabPage1.Controls.Add(this.cbRelativeSpecified);
            this.tabPage1.Controls.Add(this.cbAbsoluteSpecified);
            this.tabPage1.Controls.Add(this.lblVolumeTipRack);
            this.tabPage1.Controls.Add(this.cmbVolumeTipRack);
            this.tabPage1.Controls.Add(this.cmbDestinationVial);
            this.tabPage1.Controls.Add(this.cmbSourceVial);
            this.tabPage1.Controls.Add(this.lblVolumeCommandDestinationVial);
            this.tabPage1.Controls.Add(this.lblVolumeCommandSourceVial);
            this.tabPage1.Controls.Add(this.lblVolumeCommandRelative);
            this.tabPage1.Controls.Add(this.txtVolumeCommandRelative);
            this.tabPage1.Controls.Add(this.lblVolumeCommandAbsolute);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.cbxMixCycles);
            this.tabPage1.Controls.Add(this.txtTipTubeBottomGap);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(530, 82);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Mix";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.lblAbsoluteVolume2);
            this.tabPage3.Controls.Add(this.lblRelativeProportion2);
            this.tabPage3.Controls.Add(this.cbRelativeSpecified2);
            this.tabPage3.Controls.Add(this.cbAbsoluteSpecified2);
            this.tabPage3.Controls.Add(this.cmbDestinationVial2);
            this.tabPage3.Controls.Add(this.cmbSourceVial2);
            this.tabPage3.Controls.Add(this.label11);
            this.tabPage3.Controls.Add(this.label12);
            this.tabPage3.Controls.Add(this.txtVolumeCommandRelative2);
            this.tabPage3.Controls.Add(this.lblVolumeCommandRelative2);
            this.tabPage3.Controls.Add(this.lblVolumeCommandAbsolute2);
            this.tabPage3.Controls.Add(this.txtVolumeCommandAbsolute2);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(530, 82);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Transport";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // lblAbsoluteVolume2
            // 
            this.lblAbsoluteVolume2.Location = new System.Drawing.Point(137, 60);
            this.lblAbsoluteVolume2.Name = "lblAbsoluteVolume2";
            this.lblAbsoluteVolume2.Size = new System.Drawing.Size(110, 20);
            this.lblAbsoluteVolume2.TabIndex = 54;
            this.lblAbsoluteVolume2.Text = "Absolute Volume";
            // 
            // lblRelativeProportion2
            // 
            this.lblRelativeProportion2.Location = new System.Drawing.Point(6, 60);
            this.lblRelativeProportion2.Name = "lblRelativeProportion2";
            this.lblRelativeProportion2.Size = new System.Drawing.Size(103, 20);
            this.lblRelativeProportion2.TabIndex = 53;
            this.lblRelativeProportion2.Text = "Relative Proportion";
            // 
            // cbRelativeSpecified2
            // 
            this.cbRelativeSpecified2.Location = new System.Drawing.Point(115, 55);
            this.cbRelativeSpecified2.Name = "cbRelativeSpecified2";
            this.cbRelativeSpecified2.Size = new System.Drawing.Size(16, 24);
            this.cbRelativeSpecified2.TabIndex = 52;
            this.cbRelativeSpecified2.CheckedChanged += new System.EventHandler(this.cbRelativeSpecified2_CheckedChanged);
            // 
            // cbAbsoluteSpecified2
            // 
            this.cbAbsoluteSpecified2.Location = new System.Drawing.Point(255, 55);
            this.cbAbsoluteSpecified2.Name = "cbAbsoluteSpecified2";
            this.cbAbsoluteSpecified2.Size = new System.Drawing.Size(16, 24);
            this.cbAbsoluteSpecified2.TabIndex = 44;
            this.cbAbsoluteSpecified2.CheckedChanged += new System.EventHandler(this.cbAbsoluteSpecified2_CheckedChanged);
            // 
            // cmbDestinationVial2
            // 
            this.cmbDestinationVial2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDestinationVial2.Location = new System.Drawing.Point(69, 28);
            this.cmbDestinationVial2.Name = "cmbDestinationVial2";
            this.cmbDestinationVial2.Size = new System.Drawing.Size(202, 21);
            this.cmbDestinationVial2.TabIndex = 43;
            this.cmbDestinationVial2.SelectedIndexChanged += new System.EventHandler(this.cmbDestinationVial2_SelectedIndexChanged);
            // 
            // cmbSourceVial2
            // 
            this.cmbSourceVial2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSourceVial2.Location = new System.Drawing.Point(69, 4);
            this.cmbSourceVial2.Name = "cmbSourceVial2";
            this.cmbSourceVial2.Size = new System.Drawing.Size(202, 21);
            this.cmbSourceVial2.TabIndex = 41;
            this.cmbSourceVial2.SelectedIndexChanged += new System.EventHandler(this.cmbSourceVial2_SelectedIndexChanged);
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(5, 31);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(63, 18);
            this.label11.TabIndex = 42;
            this.label11.Text = "Destination";
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(5, 7);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(48, 18);
            this.label12.TabIndex = 40;
            this.label12.Text = "Source";
            // 
            // txtVolumeCommandRelative2
            // 
            this.txtVolumeCommandRelative2.Enabled = false;
            this.txtVolumeCommandRelative2.Location = new System.Drawing.Point(403, 55);
            this.txtVolumeCommandRelative2.Name = "txtVolumeCommandRelative2";
            this.txtVolumeCommandRelative2.Size = new System.Drawing.Size(116, 20);
            this.txtVolumeCommandRelative2.TabIndex = 48;
            this.txtVolumeCommandRelative2.Visible = false;
            this.txtVolumeCommandRelative2.TextChanged += new System.EventHandler(this.txtVolumeCommandRelative2_TextChanged);
            // 
            // lblVolumeCommandRelative2
            // 
            this.lblVolumeCommandRelative2.Location = new System.Drawing.Point(314, 55);
            this.lblVolumeCommandRelative2.Name = "lblVolumeCommandRelative2";
            this.lblVolumeCommandRelative2.Size = new System.Drawing.Size(79, 20);
            this.lblVolumeCommandRelative2.TabIndex = 45;
            this.lblVolumeCommandRelative2.Text = "Proportion";
            this.lblVolumeCommandRelative2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblVolumeCommandAbsolute2
            // 
            this.lblVolumeCommandAbsolute2.Location = new System.Drawing.Point(314, 55);
            this.lblVolumeCommandAbsolute2.Name = "lblVolumeCommandAbsolute2";
            this.lblVolumeCommandAbsolute2.Size = new System.Drawing.Size(79, 20);
            this.lblVolumeCommandAbsolute2.TabIndex = 46;
            this.lblVolumeCommandAbsolute2.Text = "Value (uL)";
            this.lblVolumeCommandAbsolute2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtVolumeCommandAbsolute2
            // 
            this.txtVolumeCommandAbsolute2.Enabled = false;
            this.txtVolumeCommandAbsolute2.Location = new System.Drawing.Point(403, 55);
            this.txtVolumeCommandAbsolute2.Name = "txtVolumeCommandAbsolute2";
            this.txtVolumeCommandAbsolute2.Size = new System.Drawing.Size(116, 20);
            this.txtVolumeCommandAbsolute2.TabIndex = 47;
            this.txtVolumeCommandAbsolute2.TextChanged += new System.EventHandler(this.txtVolumeCommandAbsolute2_TextChanged);
            // 
            // errorRelativeVolumeProportion2
            // 
            this.errorRelativeVolumeProportion2.ContainerControl = this;
            // 
            // errorAbsoluteVolume2
            // 
            this.errorAbsoluteVolume2.ContainerControl = this;
            // 
            // errorcanttransport2
            // 
            this.errorcanttransport2.ContainerControl = this;
            // 
            // GenericMultiStepCommandPanel2
            // 
            this.Controls.Add(this.tabControl1);
            this.Name = "GenericMultiStepCommandPanel2";
            this.VisibleChanged += new System.EventHandler(this.TopUpMixTransSepTransCommandPanel_VisibleChanged);
            this.Controls.SetChildIndex(this.tabControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.errorAbsoluteVolume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorRelativeVolumeProportion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorcanttransport)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorRelativeVolumeProportion2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorAbsoluteVolume2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorcanttransport2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        
        public void updateCheckboxAndTextbox(out VolumeType type, CheckBox chk1, CheckBox chk2, TextBox txt1, TextBox txt2,
            Label lbl1, Label lbl2, ClearVolumeError_delegate clear1, ClearRelativeVolumeProportionError_delegate clear2, bool is1Rel  )
        {
            if (chk2.Checked)
            {
                type = is1Rel ? VolumeType.Absolute : VolumeType.Relative;
                if (chk1.Checked)
                {
                    chk1.Checked = false;
                }
                clear1();
                clear2();
            }
            else if (chk1.Checked)
            {
                type = is1Rel ? VolumeType.Relative : VolumeType.Absolute;
            }
            else
            {
                type = VolumeType.NotSpecified;
            }

            // toggle checkboxes and labels
            txt2.Visible = chk2.Checked;
            lbl2.Visible = chk2.Checked;
            txt2.Enabled = chk2.Checked;

            txt1.Visible = !chk2.Checked;
            lbl1.Visible = !chk2.Checked;
            txt1.Enabled = chk2.Checked;

            ReportCommandDetailChanged();
        }



        private void cbRelativeSpecified2_CheckedChanged(object sender, EventArgs e)
        {
            updateCheckboxAndTextbox(out myVolumeTypeSpecifier2, cbAbsoluteSpecified2, cbRelativeSpecified2,
                txtVolumeCommandAbsolute2, txtVolumeCommandRelative2,
                    lblVolumeCommandAbsolute2, lblVolumeCommandRelative2,
                    ClearAbsoluteVolumeError2, ClearRelativeVolumeProportion2Error, false);
        }

        private void cbAbsoluteSpecified2_CheckedChanged(object sender, EventArgs e)
        {
            updateCheckboxAndTextbox(out myVolumeTypeSpecifier2, cbRelativeSpecified2, cbAbsoluteSpecified2,
                  txtVolumeCommandRelative2, txtVolumeCommandAbsolute2,
            lblVolumeCommandRelative2, lblVolumeCommandAbsolute2,
            ClearRelativeVolumeProportion2Error, ClearAbsoluteVolumeError2, true);
        }

       
        public delegate bool CheckDest_delegate();
        protected void txtVolumeCommandRelative_helper(CheckDest_delegate checkDest, CheckBox cbRel, TextBox txtRel, 
            ClearVolumeError_delegate clearErr,
            ShowVolumeError_delegate showErr, ref double relVol)
        {
            // if returns true, we want to stop user from making changes
            if (checkDest != null && checkDest()) //IAT
            {
                return;
            }
            ReportCommandDetailChanged();
            if (txtRel.TextLength == 0 && cbRel.Checked)
            {
                showErr(VolumeError.MINIMUM_AMOUNT);
            }
            else
            {
                try
                {
                    if (cbRel.Checked)
                    {
                        relVol = double.Parse(txtRel.Text);
                        if ((relVol < minimumRelative_Ratio ||
                            relVol > maximumRelative_Ratio) )
                        {
                            //                           ShowRelativeVolumeProportionError(0);
                            showErr(VolumeError.RELATIVE_VIAL_AMOUNT);
                        }
                        else
                        {
                            clearErr();
                        }
                    }
                    else
                    {
                        clearErr();
                    }
                }
                catch
                {
                    relVol = minimumRelative_TipVolume;
                }
            }
        }

        private void txtVolumeCommandRelative2_TextChanged(object sender, EventArgs e)
        {
            txtVolumeCommandRelative_helper(CheckTransportDestVial2, cbRelativeSpecified2, txtVolumeCommandRelative2,
                     ClearRelativeVolumeProportion2Error, ShowRelativeVolumeProportion2Error, ref myRelativeVolumeProportion2);
        }


        
        private void txtVolumeCommandAbsolute2_TextChanged(object sender, EventArgs e)
        {
            txtVolumeCommandAbsolute_helper(cbAbsoluteSpecified2, txtVolumeCommandAbsolute2, ClearAbsoluteVolumeError2,
                                            ShowAbsoluteVolumeError2, ref myAbsoluteVolume_uL2, (string)cmbSourceVial2.SelectedItem);
        }

        

        private void cmbSourceVialMix_SelectedIndexChanged(object sender, EventArgs e)
        {
            //
        }

        private void cmbSourceVial2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //
        }

        private void cmbSourceVial3_SelectedIndexChanged(object sender, EventArgs e)
        {
            //
        }

        override protected void cmbDestinationVial_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if(boNotReady)return;
            ReportCommandDetailChanged();
            CheckTransportDestVial();

            cmbSourceVial2.SelectedIndex = cmbDestinationVial.SelectedIndex;
        }

      

        private void TopUpMixTransSepTransCommandPanel_VisibleChanged(object sender, EventArgs e)
        {
            //
        }

        private void cmbDestinationVial2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (boNotReady) return;
            ReportCommandDetailChanged();
            CheckTransportDestVial2();
        }



        
        //tiprack, tiptube bottom mixcycers, wait time _textchange
	}
}
