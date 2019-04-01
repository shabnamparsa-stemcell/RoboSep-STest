

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
using Tesla.Common.ResourceManagement;

namespace Tesla.ProtocolEditorControls
{
	/// <summary>
	/// Summary description for CommandPanel.
	/// </summary>
    public class GenericMultiStepCommandPanel : Tesla.ProtocolEditorControls.VolumeCommandPanel
    {
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private TabPage tabPage4;
        public Label lblAbsoluteVolumeMix;
        public Label lblRelativeProportionMix;
        public CheckBox cbRelativeSpecifiedMix;
        public CheckBox cbAbsoluteSpecifiedMix;
        public ComboBox cmbDestinationVialMix;
        public ComboBox cmbSourceVialMix;
        public Label label4;
        public Label label5;
        public Label lblVolumeCommandRelativeMix;
        public Label lblVolumeCommandAbsoluteMix;
        public TextBox txtVolumeCommandAbsoluteMix;
        public TextBox txtVolumeCommandRelativeMix;
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
        public Label lblAbsoluteVolume3;
        public Label lblRelativeProportion3;
        public CheckBox cbRelativeSpecified3;
        public CheckBox cbAbsoluteSpecified3;
        public ComboBox cmbDestinationVial3;
        public ComboBox cmbSourceVial3;
        public Label label18;
        public Label label19;
        public Label lblVolumeCommandRelative3;
        public Label lblVolumeCommandAbsolute3;
        public TextBox txtVolumeCommandAbsolute3;
        public TextBox txtVolumeCommandRelative3;
        private ComboBox cbxMixCycles;
        private Label label22;
        private Label label23;
        public TextBox txtTipTubeBottomGap;
        private TextBox txtSeparationProcessingTime;
        private Label lblSeparateProcessingTime;
        public ErrorProvider errorRelativeVolumeProportion2;
        private IContainer components;
        public ErrorProvider errorRelativeVolumeProportion3;
        public ErrorProvider errorRelativeVolumeProportionMix;
        public ErrorProvider errorAbsoluteVolume2;
        public ErrorProvider errorAbsoluteVolume3;
        public ErrorProvider errorAbsoluteVolumeMix;
        public ErrorProvider errorcanttransport2;
        public ErrorProvider errorcanttransport3;
        public ErrorProvider errorcanttransportMix;
        private TabPage tabPage5;
        public ErrorProvider errorSeparationProcessingTime;
        private bool boIgnoreSepTab;
        private bool boIgnoreTrans2Tab;
        private bool boIgnoreTrans1Tab;
        private bool boIgnoreMixTab;
        private bool bo1stTabResus;
        int idxMix;
        int idxTrans1;
        int idxSep;
        int idxTrans2;
		

        public bool boNotReady = true;

        #region Construction/destruction
        public void initForResusMixSepTransCommandPanel()
        {
            bo1stTabResus = true;
            tabControl1.TabPages[0].Text = "Resus";
            TabPage trans1 = tabControl1.TabPages[2];
            tabControl1.TabPages.Remove(trans1);  //remove trans1
            tabControl1.TabPages.RemoveAt(3);  //remove trans2
            tabControl1.TabPages.Add(trans1); //add trans1 at the back
            boIgnoreTrans2Tab = true;
            idxMix = 1;
            idxTrans1 = 3;
            idxSep = 2;
            idxTrans2 = -1;
        }
        public void initForResusMixCommandPanel()
        {
            bo1stTabResus = true;
            tabControl1.TabPages[0].Text = "Resus";
            tabControl1.TabPages.RemoveAt(2); //remove trans1
            tabControl1.TabPages.RemoveAt(2); //remove sep
            tabControl1.TabPages.RemoveAt(2); //remove trans2
            boIgnoreTrans1Tab = true;
            boIgnoreSepTab = true;
            boIgnoreTrans2Tab = true;
            idxMix = 1;
            idxTrans1 = -1;
            idxSep = -1;
            idxTrans2 = -1;
        }

        public void initForTopUpMixTransCommandPanel()
        {
            tabControl1.TabPages.RemoveAt(3); //remove sep
            tabControl1.TabPages.RemoveAt(3);  //remove trans2
            boIgnoreSepTab = true;
            boIgnoreTrans2Tab = true;
            idxMix = 1;
            idxTrans1 = 2;
            idxSep = -1;
            idxTrans2 = -1;
        }
        public void initForTopUpTransCommandPanel()
        {
            tabControl1.TabPages.RemoveAt(1); //remove mix
            tabControl1.TabPages.RemoveAt(2); //remove sep
            tabControl1.TabPages.RemoveAt(2);  //remove trans2
            boIgnoreSepTab = true;
            boIgnoreTrans2Tab = true;
            boIgnoreMixTab = true;
            idxMix = -1;
            idxTrans1 = 1;
            idxSep = -1;
            idxTrans2 = -1;
        }

        public void initForTopUpTransSepTransCommandPanel()
        {
            tabControl1.TabPages.RemoveAt(1); //remove mix
            boIgnoreMixTab = true;
            idxMix = -1;
            idxTrans1 = 1;
            idxSep = 2;
            idxTrans2 = 3;
        }

         public override bool IsContentValid()
        {
            int idxWithErr = -1;
            bool isContentValid = base.IsContentValid();
            if (idxWithErr == -1 && !isContentValid) idxWithErr=0;

            if (!boIgnoreMixTab)
            {
                isContentValid = IsContentValid_helper(isContentValid, myVolumeTypeSpecifierMix,
                    txtVolumeCommandRelativeMix, myRelativeVolumeProportionMix, errorRelativeVolumeProportionMix,
                    txtVolumeCommandAbsoluteMix, myAbsoluteVolume_uLMix, errorAbsoluteVolumeMix, isVolumeSpecificationRequiredMix);
                if (idxWithErr == -1 && !isContentValid) idxWithErr = idxMix;
            }

            if (!boIgnoreTrans1Tab)
            {
                isContentValid = IsContentValid_helper(isContentValid, myVolumeTypeSpecifier2,
                    txtVolumeCommandRelative2, myRelativeVolumeProportion2, errorRelativeVolumeProportion2,
                    txtVolumeCommandAbsolute2, myAbsoluteVolume_uL2, errorAbsoluteVolume2, isVolumeSpecificationRequired2);
                if (idxWithErr == -1 && !isContentValid) idxWithErr = idxTrans1;
            }


            if (!boIgnoreSepTab && mySeparationProcessingTime < 0)
            {
                idxWithErr = idxSep;
                isContentValid = false;
            }

            if (!boIgnoreTrans2Tab)
            {
                isContentValid = IsContentValid_helper(isContentValid, myVolumeTypeSpecifier3,
                    txtVolumeCommandRelative3, myRelativeVolumeProportion3, errorRelativeVolumeProportion3,
                    txtVolumeCommandAbsolute3, myAbsoluteVolume_uL3, errorAbsoluteVolume3, isVolumeSpecificationRequired3);
                if (idxWithErr == -1 && !isContentValid) idxWithErr = idxTrans2;
            }

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


        public GenericMultiStepCommandPanel()
		{
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            bo1stTabResus = false;
            boIgnoreMixTab = false;
            boIgnoreSepTab = false;
            boIgnoreTrans2Tab = false; 
            idxMix=1;
            idxTrans1=2;
            idxSep=3;
            idxTrans2=4;
		

            isVolumeSpecificationRequired2 = true;
            isVolumeSpecificationRequired3 = true;
            isVolumeSpecificationRequiredMix = true;

            ComboBox[] boxes = { cmbSourceVial, cmbDestinationVial, cmbSourceVial2, cmbDestinationVial2, 
                                   cmbSourceVial3, cmbDestinationVial3, cmbSourceVialMix, cmbDestinationVialMix };
            FillComboBoxesWithVialNames(boxes);

            UpdateMaxMixCycle();


            cmbSourceVialMix.Enabled = false;
            cmbDestinationVialMix.Enabled = false;
            cmbSourceVial2.Enabled = false;
            cmbSourceVial3.Enabled = false;


            //test
            //tabControl1.TabPages.Remove(tabControl1.TabPages[3]);
            //so far isValid uses idx to display tabpage

            boNotReady = false;
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
			



			base.Dispose( disposing );
		}	
	
        #endregion Construction/destruction

        public override void UpdateComboBoxes(ProtocolClass pc)
        {
            ComboBox[] boxes = { cmbSourceVial, cmbDestinationVial, cmbSourceVial2, cmbDestinationVial2, 
                                   cmbSourceVial3, cmbDestinationVial3, cmbSourceVialMix, cmbDestinationVialMix };
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

        public AbsoluteResourceLocation SourceVial3
        {
            get
            {
                return ComboIndexToVialID(cmbSourceVial3.SelectedIndex);
            }
            set
            {
                int idx = VialIDToComboIndex(value);
                if (idx != -1 && idx < cmbSourceVial3.Items.Count) cmbSourceVial3.SelectedIndex = idx;
            }
        }

        public AbsoluteResourceLocation DestinationVial3
        {
            get
            {
                return ComboIndexToVialID(cmbDestinationVial3.SelectedIndex);
            }
            set
            {

                int idx = VialIDToComboIndex(value);
                if (idx != -1 && idx < cmbDestinationVial3.Items.Count) cmbDestinationVial3.SelectedIndex = idx;
            }
        }

        public AbsoluteResourceLocation SourceVialMix
        {
            get
            {
                return ComboIndexToVialID(cmbSourceVialMix.SelectedIndex);
            }
            set
            {
                int idx = VialIDToComboIndex(value);
                if (idx != -1 && idx < cmbSourceVialMix.Items.Count) cmbSourceVialMix.SelectedIndex = idx;
            }
        }

        public AbsoluteResourceLocation DestinationVialMix
        {
            get
            {
                return ComboIndexToVialID(cmbDestinationVialMix.SelectedIndex);
            }
            set
            {

                int idx = VialIDToComboIndex(value);
                if (idx != -1 && idx < cmbDestinationVialMix.Items.Count) cmbDestinationVialMix.SelectedIndex = idx;
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

        private bool isVolumeSpecificationRequired3;

        public bool IsVolumeSpecificationRequired3
        {
            get
            {
                return isVolumeSpecificationRequired3;
            }
            set
            {
                isVolumeSpecificationRequired3 = value;
            }
        }

        private bool isVolumeSpecificationRequiredMix;

        public bool IsVolumeSpecificationRequiredMix
        {
            get
            {
                return isVolumeSpecificationRequiredMix;
            }
            set
            {
                isVolumeSpecificationRequiredMix = value;
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

        private VolumeType myVolumeTypeSpecifier3;

        public VolumeType VolumeTypeSpecifier3
        {
            get
            {
                return myVolumeTypeSpecifier3;
            }
            set
            {
                myVolumeTypeSpecifier3 = value;

                SetCtrlFromVolumeType(value, cbRelativeSpecified3, lblVolumeCommandRelative3, txtVolumeCommandRelative3,
                    cbAbsoluteSpecified3, lblVolumeCommandAbsolute3, txtVolumeCommandAbsolute3);

                this.Refresh();
            }
        }

        private VolumeType myVolumeTypeSpecifierMix;

        public VolumeType VolumeTypeSpecifierMix
        {
            get
            {
                return myVolumeTypeSpecifierMix;
            }
            set
            {
                myVolumeTypeSpecifierMix = value;

                SetCtrlFromVolumeType(value, cbRelativeSpecifiedMix, lblVolumeCommandRelativeMix, txtVolumeCommandRelativeMix,
                    cbAbsoluteSpecifiedMix, lblVolumeCommandAbsoluteMix, txtVolumeCommandAbsoluteMix);

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

        private double myRelativeVolumeProportion3;

        public double RelativeVolumeProportion3
        {
            get
            {
                return myRelativeVolumeProportion3;
            }
            set
            {
                txtVolumeCommandRelative3.Text = value.ToString("F4");
            }
        }

        private int myAbsoluteVolume_uL3;

        public int AbsoluteVolume_uL3
        {
            get
            {
                return myAbsoluteVolume_uL3;
            }
            set
            {
                txtVolumeCommandAbsolute3.Text = value.ToString();
            }
        }

        private double myRelativeVolumeProportionMix;

        public double RelativeVolumeProportionMix
        {
            get
            {
                return myRelativeVolumeProportionMix;
            }
            set
            {
                txtVolumeCommandRelativeMix.Text = value.ToString("F4");
            }
        }

        private int myAbsoluteVolume_uLMix;

        public int AbsoluteVolume_uLMix
        {
            get
            {
                return myAbsoluteVolume_uLMix;
            }
            set
            {
                txtVolumeCommandAbsoluteMix.Text = value.ToString();
            }
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

                if (value > 0)
                {
                    int num = (cbxMixCycles.Items.Count < value) ? cbxMixCycles.Items.Count : value;
                    myMixCycles = num;
                    cbxMixCycles.SelectedIndex = num - 1;
                }
            }
        }

        private int mySeparationProcessingTime = -1;

        public uint WaitCommandTimeDuration
        {
            get
            {
                return mySeparationProcessingTime < 0 ?
                    (uint)0 : (uint)mySeparationProcessingTime;
            }
            set
            {
                // Set the separation time to the supplied value.  NOTE: the 
                // 'text changed' handler also checks to see if the new value
                // is valid.
                txtSeparationProcessingTime.Text = value.ToString();
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

        public void ShowRelativeVolumeProportion3Error(VolumeError errorVol)
        {
            ShowRelativeVolumeProportionError(errorVol, errorRelativeVolumeProportion3, txtVolumeCommandRelative3);
        }

        public void ClearRelativeVolumeProportion3Error()
        {
            errorRelativeVolumeProportion3.SetError(txtVolumeCommandRelative3, string.Empty);
        }

        public void ShowRelativeVolumeProportionMixError(VolumeError errorVol)
        {
            ShowRelativeVolumeProportionError(errorVol, errorRelativeVolumeProportionMix, txtVolumeCommandRelativeMix);
        }

        public void ClearRelativeVolumeProportionMixError()
        {
            errorRelativeVolumeProportionMix.SetError(txtVolumeCommandRelativeMix, string.Empty);
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
        public void ShowCantTransportError3()
        {
            errorcanttransport3.SetError(
                cmbDestinationVial3, "Can't do transport to the specified vial");
        }

        public void ClearCantTransportError3()
        {
            errorcanttransport3.SetError(cmbDestinationVial3, string.Empty);
        }

        public void ShowCantTransportErrorMix()
        {
            errorcanttransportMix.SetError(
                cmbDestinationVialMix, "Can't do transport to the specified vial");
        }

        public void ClearCantTransportErrorMix()
        {
            errorcanttransportMix.SetError(cmbDestinationVialMix, string.Empty);
        }


        protected void setEnabled2(bool enab)
        {
            setEnabled_helper(enab,false,cbRelativeSpecified2,cbAbsoluteSpecified2,
                txtVolumeCommandRelative2,lblRelativeProportion2,
                txtVolumeCommandAbsolute2,lblAbsoluteVolume2);
        }
        protected void setEnabled3(bool enab)
        {
            setEnabled_helper(enab, false, cbRelativeSpecified3, cbAbsoluteSpecified3,
                txtVolumeCommandRelative3, lblRelativeProportion3,
                txtVolumeCommandAbsolute3, lblAbsoluteVolume3);
        }
        protected void setEnabledMix(bool enab)
        {
            setEnabled_helper(enab, false, cbRelativeSpecifiedMix, cbAbsoluteSpecifiedMix,
                txtVolumeCommandRelativeMix, lblRelativeProportionMix,
                txtVolumeCommandAbsoluteMix, lblAbsoluteVolumeMix);
        }



        
        public bool CheckTransportDestVial2()
        {
            return CheckTransportDestVial_helper(cmbDestinationVial2, true, setEnabled2, ShowCantTransportError2, ClearCantTransportError2);
        }
        public bool CheckTransportDestVial3()
        {
            return CheckTransportDestVial_helper(cmbDestinationVial3, true, setEnabled3, ShowCantTransportError3, ClearCantTransportError3);
        }

        public void ShowAbsoluteVolumeError2(VolumeError errorVol)
        {
            ShowAbsoluteVolumeError(errorVol, errorAbsoluteVolume2, txtVolumeCommandAbsolute2);
        }

        public void ClearAbsoluteVolumeError2()
        {
            errorAbsoluteVolume2.SetError(txtVolumeCommandAbsolute2, string.Empty);
        }
        public void ShowAbsoluteVolumeError3(VolumeError errorVol)
        {
            ShowAbsoluteVolumeError(errorVol, errorAbsoluteVolume3, txtVolumeCommandAbsolute3);
        }

        public void ClearAbsoluteVolumeError3()
        {
            errorAbsoluteVolume3.SetError(txtVolumeCommandAbsolute3, string.Empty);
        }
        public void ShowAbsoluteVolumeErrorMix(VolumeError errorVol)
        {
            ShowAbsoluteVolumeError(errorVol, errorAbsoluteVolumeMix, txtVolumeCommandAbsoluteMix);
        }

        public void ClearAbsoluteVolumeErrorMix()
        {
            errorAbsoluteVolumeMix.SetError(txtVolumeCommandAbsoluteMix, string.Empty);
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

        public void SetVolumeCommandPanelParams3(AbsoluteResourceLocation SourceVial, AbsoluteResourceLocation DestinationVial,
            VolumeType VolumeTypeSpecifier, double RelativeVolumeProportion, int AbsoluteVolume_uL)
        {
            this.SourceVial3 = SourceVial;
            this.DestinationVial3 = DestinationVial;
            this.VolumeTypeSpecifier3 = VolumeTypeSpecifier;


            this.RelativeVolumeProportion3 = RelativeVolumeProportion;
            this.AbsoluteVolume_uL3 = AbsoluteVolume_uL;
        }


        public void SetVolumeCommandPanelParamsMix(AbsoluteResourceLocation SourceVial, AbsoluteResourceLocation DestinationVial,
            VolumeType VolumeTypeSpecifier, double RelativeVolumeProportion, int AbsoluteVolume_uL,
            int MixCycles, int TipTubeBottomGap)
        {
            this.SourceVialMix = SourceVial;
            this.DestinationVialMix = DestinationVial;
            this.VolumeTypeSpecifierMix = VolumeTypeSpecifier;


            this.RelativeVolumeProportionMix = RelativeVolumeProportion;
            this.AbsoluteVolume_uLMix = AbsoluteVolume_uL;

            this.MixCycles = MixCycles;
            this.TipTubeBottomGap = TipTubeBottomGap;
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
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.cbxMixCycles = new System.Windows.Forms.ComboBox();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.txtTipTubeBottomGap = new System.Windows.Forms.TextBox();
            this.lblAbsoluteVolumeMix = new System.Windows.Forms.Label();
            this.lblRelativeProportionMix = new System.Windows.Forms.Label();
            this.cbRelativeSpecifiedMix = new System.Windows.Forms.CheckBox();
            this.cbAbsoluteSpecifiedMix = new System.Windows.Forms.CheckBox();
            this.cmbDestinationVialMix = new System.Windows.Forms.ComboBox();
            this.cmbSourceVialMix = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtVolumeCommandRelativeMix = new System.Windows.Forms.TextBox();
            this.lblVolumeCommandRelativeMix = new System.Windows.Forms.Label();
            this.lblVolumeCommandAbsoluteMix = new System.Windows.Forms.Label();
            this.txtVolumeCommandAbsoluteMix = new System.Windows.Forms.TextBox();
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
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.txtSeparationProcessingTime = new System.Windows.Forms.TextBox();
            this.lblSeparateProcessingTime = new System.Windows.Forms.Label();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.lblAbsoluteVolume3 = new System.Windows.Forms.Label();
            this.lblRelativeProportion3 = new System.Windows.Forms.Label();
            this.cbRelativeSpecified3 = new System.Windows.Forms.CheckBox();
            this.cbAbsoluteSpecified3 = new System.Windows.Forms.CheckBox();
            this.cmbDestinationVial3 = new System.Windows.Forms.ComboBox();
            this.cmbSourceVial3 = new System.Windows.Forms.ComboBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.txtVolumeCommandRelative3 = new System.Windows.Forms.TextBox();
            this.lblVolumeCommandRelative3 = new System.Windows.Forms.Label();
            this.lblVolumeCommandAbsolute3 = new System.Windows.Forms.Label();
            this.txtVolumeCommandAbsolute3 = new System.Windows.Forms.TextBox();
            this.errorRelativeVolumeProportion2 = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorRelativeVolumeProportion3 = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorRelativeVolumeProportionMix = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorAbsoluteVolume2 = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorAbsoluteVolume3 = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorAbsoluteVolumeMix = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorcanttransport2 = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorcanttransport3 = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorcanttransportMix = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorSeparationProcessingTime = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorAbsoluteVolume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorRelativeVolumeProportion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorcanttransport)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorRelativeVolumeProportion2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorRelativeVolumeProportion3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorRelativeVolumeProportionMix)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorAbsoluteVolume2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorAbsoluteVolume3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorAbsoluteVolumeMix)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorcanttransport2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorcanttransport3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorcanttransportMix)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorSeparationProcessingTime)).BeginInit();
            this.SuspendLayout();
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
            this.txtVolumeCommandAbsolute.Location = new System.Drawing.Point(395, 55);
            this.txtVolumeCommandAbsolute.TabIndex = 33;
            // 
            // lblVolumeCommandAbsolute
            // 
            this.lblVolumeCommandAbsolute.Location = new System.Drawing.Point(314, 55);
            this.lblVolumeCommandAbsolute.TabIndex = 31;
            // 
            // txtVolumeCommandRelative
            // 
            this.txtVolumeCommandRelative.Location = new System.Drawing.Point(395, 55);
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
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Location = new System.Drawing.Point(0, 50);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(538, 108);
            this.tabControl1.TabIndex = 8;
            // 
            // tabPage1
            // 
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
            this.tabPage1.Controls.Add(this.txtVolumeCommandAbsolute);
            this.tabPage1.Controls.Add(this.lblVolumeCommandAbsolute);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(530, 82);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "TopUp";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.cbxMixCycles);
            this.tabPage2.Controls.Add(this.label22);
            this.tabPage2.Controls.Add(this.label23);
            this.tabPage2.Controls.Add(this.txtTipTubeBottomGap);
            this.tabPage2.Controls.Add(this.lblAbsoluteVolumeMix);
            this.tabPage2.Controls.Add(this.lblRelativeProportionMix);
            this.tabPage2.Controls.Add(this.cbRelativeSpecifiedMix);
            this.tabPage2.Controls.Add(this.cbAbsoluteSpecifiedMix);
            this.tabPage2.Controls.Add(this.cmbDestinationVialMix);
            this.tabPage2.Controls.Add(this.cmbSourceVialMix);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.txtVolumeCommandRelativeMix);
            this.tabPage2.Controls.Add(this.lblVolumeCommandRelativeMix);
            this.tabPage2.Controls.Add(this.lblVolumeCommandAbsoluteMix);
            this.tabPage2.Controls.Add(this.txtVolumeCommandAbsoluteMix);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(530, 82);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Mix";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // cbxMixCycles
            // 
            this.cbxMixCycles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxMixCycles.Location = new System.Drawing.Point(463, 4);
            this.cbxMixCycles.Name = "cbxMixCycles";
            this.cbxMixCycles.Size = new System.Drawing.Size(52, 21);
            this.cbxMixCycles.TabIndex = 58;
            this.cbxMixCycles.SelectedIndexChanged += new System.EventHandler(this.cbxMixCycles_SelectedIndexChanged);
            // 
            // label22
            // 
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.Location = new System.Drawing.Point(297, 29);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(96, 24);
            this.label22.TabIndex = 55;
            this.label22.Text = "Tip to Tube Bottom Gap (uL)";
            // 
            // label23
            // 
            this.label23.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.Location = new System.Drawing.Point(406, 7);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(61, 16);
            this.label23.TabIndex = 56;
            this.label23.Text = "Mix Cycle(s)";
            // 
            // txtTipTubeBottomGap
            // 
            this.txtTipTubeBottomGap.Location = new System.Drawing.Point(395, 29);
            this.txtTipTubeBottomGap.Name = "txtTipTubeBottomGap";
            this.txtTipTubeBottomGap.Size = new System.Drawing.Size(116, 20);
            this.txtTipTubeBottomGap.TabIndex = 57;
            this.txtTipTubeBottomGap.TextChanged += new System.EventHandler(this.txtTipTubeBottomGap_TextChanged);
            // 
            // lblAbsoluteVolumeMix
            // 
            this.lblAbsoluteVolumeMix.Location = new System.Drawing.Point(137, 60);
            this.lblAbsoluteVolumeMix.Name = "lblAbsoluteVolumeMix";
            this.lblAbsoluteVolumeMix.Size = new System.Drawing.Size(110, 20);
            this.lblAbsoluteVolumeMix.TabIndex = 54;
            this.lblAbsoluteVolumeMix.Text = "Absolute Volume";
            // 
            // lblRelativeProportionMix
            // 
            this.lblRelativeProportionMix.Location = new System.Drawing.Point(6, 60);
            this.lblRelativeProportionMix.Name = "lblRelativeProportionMix";
            this.lblRelativeProportionMix.Size = new System.Drawing.Size(103, 20);
            this.lblRelativeProportionMix.TabIndex = 53;
            this.lblRelativeProportionMix.Text = "Relative Proportion";
            // 
            // cbRelativeSpecifiedMix
            // 
            this.cbRelativeSpecifiedMix.Location = new System.Drawing.Point(115, 55);
            this.cbRelativeSpecifiedMix.Name = "cbRelativeSpecifiedMix";
            this.cbRelativeSpecifiedMix.Size = new System.Drawing.Size(16, 24);
            this.cbRelativeSpecifiedMix.TabIndex = 52;
            this.cbRelativeSpecifiedMix.CheckedChanged += new System.EventHandler(this.cbRelativeSpecifiedMix_CheckedChanged);
            // 
            // cbAbsoluteSpecifiedMix
            // 
            this.cbAbsoluteSpecifiedMix.Location = new System.Drawing.Point(255, 55);
            this.cbAbsoluteSpecifiedMix.Name = "cbAbsoluteSpecifiedMix";
            this.cbAbsoluteSpecifiedMix.Size = new System.Drawing.Size(16, 24);
            this.cbAbsoluteSpecifiedMix.TabIndex = 44;
            this.cbAbsoluteSpecifiedMix.CheckedChanged += new System.EventHandler(this.cbAbsoluteSpecifiedMix_CheckedChanged);
            // 
            // cmbDestinationVialMix
            // 
            this.cmbDestinationVialMix.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDestinationVialMix.Location = new System.Drawing.Point(69, 28);
            this.cmbDestinationVialMix.Name = "cmbDestinationVialMix";
            this.cmbDestinationVialMix.Size = new System.Drawing.Size(202, 21);
            this.cmbDestinationVialMix.TabIndex = 43;
            this.cmbDestinationVialMix.SelectedIndexChanged += new System.EventHandler(this.cmbDestinationVialMix_SelectedIndexChanged);
            // 
            // cmbSourceVialMix
            // 
            this.cmbSourceVialMix.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSourceVialMix.Location = new System.Drawing.Point(69, 4);
            this.cmbSourceVialMix.Name = "cmbSourceVialMix";
            this.cmbSourceVialMix.Size = new System.Drawing.Size(202, 21);
            this.cmbSourceVialMix.TabIndex = 41;
            this.cmbSourceVialMix.SelectedIndexChanged += new System.EventHandler(this.cmbSourceVialMix_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(5, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 18);
            this.label4.TabIndex = 42;
            this.label4.Text = "Destination";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(5, 7);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 18);
            this.label5.TabIndex = 40;
            this.label5.Text = "Source";
            // 
            // txtVolumeCommandRelativeMix
            // 
            this.txtVolumeCommandRelativeMix.Enabled = false;
            this.txtVolumeCommandRelativeMix.Location = new System.Drawing.Point(395, 55);
            this.txtVolumeCommandRelativeMix.Name = "txtVolumeCommandRelativeMix";
            this.txtVolumeCommandRelativeMix.Size = new System.Drawing.Size(116, 20);
            this.txtVolumeCommandRelativeMix.TabIndex = 48;
            this.txtVolumeCommandRelativeMix.Visible = false;
            this.txtVolumeCommandRelativeMix.TextChanged += new System.EventHandler(this.txtVolumeCommandRelativeMix_TextChanged);
            // 
            // lblVolumeCommandRelativeMix
            // 
            this.lblVolumeCommandRelativeMix.Location = new System.Drawing.Point(314, 55);
            this.lblVolumeCommandRelativeMix.Name = "lblVolumeCommandRelativeMix";
            this.lblVolumeCommandRelativeMix.Size = new System.Drawing.Size(79, 20);
            this.lblVolumeCommandRelativeMix.TabIndex = 45;
            this.lblVolumeCommandRelativeMix.Text = "Proportion";
            this.lblVolumeCommandRelativeMix.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblVolumeCommandAbsoluteMix
            // 
            this.lblVolumeCommandAbsoluteMix.Location = new System.Drawing.Point(314, 55);
            this.lblVolumeCommandAbsoluteMix.Name = "lblVolumeCommandAbsoluteMix";
            this.lblVolumeCommandAbsoluteMix.Size = new System.Drawing.Size(79, 20);
            this.lblVolumeCommandAbsoluteMix.TabIndex = 46;
            this.lblVolumeCommandAbsoluteMix.Text = "Value (uL)";
            this.lblVolumeCommandAbsoluteMix.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtVolumeCommandAbsoluteMix
            // 
            this.txtVolumeCommandAbsoluteMix.Enabled = false;
            this.txtVolumeCommandAbsoluteMix.Location = new System.Drawing.Point(395, 55);
            this.txtVolumeCommandAbsoluteMix.Name = "txtVolumeCommandAbsoluteMix";
            this.txtVolumeCommandAbsoluteMix.Size = new System.Drawing.Size(116, 20);
            this.txtVolumeCommandAbsoluteMix.TabIndex = 47;
            this.txtVolumeCommandAbsoluteMix.TextChanged += new System.EventHandler(this.txtVolumeCommandAbsoluteMix_TextChanged);
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
            this.txtVolumeCommandRelative2.Location = new System.Drawing.Point(396, 55);
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
            this.txtVolumeCommandAbsolute2.Location = new System.Drawing.Point(395, 55);
            this.txtVolumeCommandAbsolute2.Name = "txtVolumeCommandAbsolute2";
            this.txtVolumeCommandAbsolute2.Size = new System.Drawing.Size(116, 20);
            this.txtVolumeCommandAbsolute2.TabIndex = 47;
            this.txtVolumeCommandAbsolute2.TextChanged += new System.EventHandler(this.txtVolumeCommandAbsolute2_TextChanged);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.txtSeparationProcessingTime);
            this.tabPage4.Controls.Add(this.lblSeparateProcessingTime);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(530, 82);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Separate";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // txtSeparationProcessingTime
            // 
            this.txtSeparationProcessingTime.Location = new System.Drawing.Point(289, 8);
            this.txtSeparationProcessingTime.Name = "txtSeparationProcessingTime";
            this.txtSeparationProcessingTime.Size = new System.Drawing.Size(112, 20);
            this.txtSeparationProcessingTime.TabIndex = 8;
            this.txtSeparationProcessingTime.TextChanged += new System.EventHandler(this.txtSeparationProcessingTime_TextChanged);
            // 
            // lblSeparateProcessingTime
            // 
            this.lblSeparateProcessingTime.Location = new System.Drawing.Point(179, 11);
            this.lblSeparateProcessingTime.Name = "lblSeparateProcessingTime";
            this.lblSeparateProcessingTime.Size = new System.Drawing.Size(108, 23);
            this.lblSeparateProcessingTime.TabIndex = 9;
            this.lblSeparateProcessingTime.Text = "Separation Time (s)";
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.lblAbsoluteVolume3);
            this.tabPage5.Controls.Add(this.lblRelativeProportion3);
            this.tabPage5.Controls.Add(this.cbRelativeSpecified3);
            this.tabPage5.Controls.Add(this.cbAbsoluteSpecified3);
            this.tabPage5.Controls.Add(this.cmbDestinationVial3);
            this.tabPage5.Controls.Add(this.cmbSourceVial3);
            this.tabPage5.Controls.Add(this.label18);
            this.tabPage5.Controls.Add(this.label19);
            this.tabPage5.Controls.Add(this.txtVolumeCommandRelative3);
            this.tabPage5.Controls.Add(this.lblVolumeCommandRelative3);
            this.tabPage5.Controls.Add(this.lblVolumeCommandAbsolute3);
            this.tabPage5.Controls.Add(this.txtVolumeCommandAbsolute3);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(530, 82);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Transport2";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // lblAbsoluteVolume3
            // 
            this.lblAbsoluteVolume3.Location = new System.Drawing.Point(137, 60);
            this.lblAbsoluteVolume3.Name = "lblAbsoluteVolume3";
            this.lblAbsoluteVolume3.Size = new System.Drawing.Size(110, 20);
            this.lblAbsoluteVolume3.TabIndex = 54;
            this.lblAbsoluteVolume3.Text = "Absolute Volume";
            // 
            // lblRelativeProportion3
            // 
            this.lblRelativeProportion3.Location = new System.Drawing.Point(6, 60);
            this.lblRelativeProportion3.Name = "lblRelativeProportion3";
            this.lblRelativeProportion3.Size = new System.Drawing.Size(103, 20);
            this.lblRelativeProportion3.TabIndex = 53;
            this.lblRelativeProportion3.Text = "Relative Proportion";
            // 
            // cbRelativeSpecified3
            // 
            this.cbRelativeSpecified3.Location = new System.Drawing.Point(115, 55);
            this.cbRelativeSpecified3.Name = "cbRelativeSpecified3";
            this.cbRelativeSpecified3.Size = new System.Drawing.Size(16, 24);
            this.cbRelativeSpecified3.TabIndex = 52;
            this.cbRelativeSpecified3.CheckedChanged += new System.EventHandler(this.cbRelativeSpecified3_CheckedChanged);
            // 
            // cbAbsoluteSpecified3
            // 
            this.cbAbsoluteSpecified3.Location = new System.Drawing.Point(255, 55);
            this.cbAbsoluteSpecified3.Name = "cbAbsoluteSpecified3";
            this.cbAbsoluteSpecified3.Size = new System.Drawing.Size(16, 24);
            this.cbAbsoluteSpecified3.TabIndex = 44;
            this.cbAbsoluteSpecified3.CheckedChanged += new System.EventHandler(this.cbAbsoluteSpecified3_CheckedChanged);
            // 
            // cmbDestinationVial3
            // 
            this.cmbDestinationVial3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDestinationVial3.Location = new System.Drawing.Point(69, 28);
            this.cmbDestinationVial3.Name = "cmbDestinationVial3";
            this.cmbDestinationVial3.Size = new System.Drawing.Size(202, 21);
            this.cmbDestinationVial3.TabIndex = 43;
            this.cmbDestinationVial3.SelectedIndexChanged += new System.EventHandler(this.cmbDestinationVial3_SelectedIndexChanged);
            // 
            // cmbSourceVial3
            // 
            this.cmbSourceVial3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSourceVial3.Location = new System.Drawing.Point(69, 4);
            this.cmbSourceVial3.Name = "cmbSourceVial3";
            this.cmbSourceVial3.Size = new System.Drawing.Size(202, 21);
            this.cmbSourceVial3.TabIndex = 41;
            this.cmbSourceVial3.SelectedIndexChanged += new System.EventHandler(this.cmbSourceVial3_SelectedIndexChanged);
            // 
            // label18
            // 
            this.label18.Location = new System.Drawing.Point(5, 31);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(63, 18);
            this.label18.TabIndex = 42;
            this.label18.Text = "Destination";
            // 
            // label19
            // 
            this.label19.Location = new System.Drawing.Point(5, 7);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(48, 18);
            this.label19.TabIndex = 40;
            this.label19.Text = "Source";
            // 
            // txtVolumeCommandRelative3
            // 
            this.txtVolumeCommandRelative3.Enabled = false;
            this.txtVolumeCommandRelative3.Location = new System.Drawing.Point(395, 55);
            this.txtVolumeCommandRelative3.Name = "txtVolumeCommandRelative3";
            this.txtVolumeCommandRelative3.Size = new System.Drawing.Size(116, 20);
            this.txtVolumeCommandRelative3.TabIndex = 48;
            this.txtVolumeCommandRelative3.Visible = false;
            this.txtVolumeCommandRelative3.TextChanged += new System.EventHandler(this.txtVolumeCommandRelative3_TextChanged);
            // 
            // lblVolumeCommandRelative3
            // 
            this.lblVolumeCommandRelative3.Location = new System.Drawing.Point(314, 55);
            this.lblVolumeCommandRelative3.Name = "lblVolumeCommandRelative3";
            this.lblVolumeCommandRelative3.Size = new System.Drawing.Size(79, 20);
            this.lblVolumeCommandRelative3.TabIndex = 45;
            this.lblVolumeCommandRelative3.Text = "Proportion";
            this.lblVolumeCommandRelative3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblVolumeCommandAbsolute3
            // 
            this.lblVolumeCommandAbsolute3.Location = new System.Drawing.Point(314, 55);
            this.lblVolumeCommandAbsolute3.Name = "lblVolumeCommandAbsolute3";
            this.lblVolumeCommandAbsolute3.Size = new System.Drawing.Size(79, 20);
            this.lblVolumeCommandAbsolute3.TabIndex = 46;
            this.lblVolumeCommandAbsolute3.Text = "Value (uL)";
            this.lblVolumeCommandAbsolute3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtVolumeCommandAbsolute3
            // 
            this.txtVolumeCommandAbsolute3.Enabled = false;
            this.txtVolumeCommandAbsolute3.Location = new System.Drawing.Point(395, 55);
            this.txtVolumeCommandAbsolute3.Name = "txtVolumeCommandAbsolute3";
            this.txtVolumeCommandAbsolute3.Size = new System.Drawing.Size(116, 20);
            this.txtVolumeCommandAbsolute3.TabIndex = 47;
            this.txtVolumeCommandAbsolute3.TextChanged += new System.EventHandler(this.txtVolumeCommandAbsolute3_TextChanged);
            // 
            // errorRelativeVolumeProportion2
            // 
            this.errorRelativeVolumeProportion2.ContainerControl = this;
            // 
            // errorRelativeVolumeProportion3
            // 
            this.errorRelativeVolumeProportion3.ContainerControl = this;
            // 
            // errorRelativeVolumeProportionMix
            // 
            this.errorRelativeVolumeProportionMix.ContainerControl = this;
            // 
            // errorAbsoluteVolume2
            // 
            this.errorAbsoluteVolume2.ContainerControl = this;
            // 
            // errorAbsoluteVolume3
            // 
            this.errorAbsoluteVolume3.ContainerControl = this;
            // 
            // errorAbsoluteVolumeMix
            // 
            this.errorAbsoluteVolumeMix.ContainerControl = this;
            // 
            // errorcanttransport2
            // 
            this.errorcanttransport2.ContainerControl = this;
            // 
            // errorcanttransport3
            // 
            this.errorcanttransport3.ContainerControl = this;
            // 
            // errorcanttransportMix
            // 
            this.errorcanttransportMix.ContainerControl = this;
            // 
            // errorSeparationProcessingTime
            // 
            this.errorSeparationProcessingTime.ContainerControl = this;
            // 
            // TopUpMixTransSepTransCommandPanel
            // 
            this.Controls.Add(this.tabControl1);
            this.Name = "TopUpMixTransSepTransCommandPanel";
            this.VisibleChanged += new System.EventHandler(this.TopUpMixTransSepTransCommandPanel_VisibleChanged);
            this.Controls.SetChildIndex(this.tabControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.errorAbsoluteVolume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorRelativeVolumeProportion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorcanttransport)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorRelativeVolumeProportion2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorRelativeVolumeProportion3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorRelativeVolumeProportionMix)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorAbsoluteVolume2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorAbsoluteVolume3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorAbsoluteVolumeMix)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorcanttransport2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorcanttransport3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorcanttransportMix)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorSeparationProcessingTime)).EndInit();
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


        private void cbRelativeSpecifiedMix_CheckedChanged(object sender, EventArgs e)
        {
            updateCheckboxAndTextbox(out myVolumeTypeSpecifierMix, cbAbsoluteSpecifiedMix,cbRelativeSpecifiedMix,
                 txtVolumeCommandAbsoluteMix,  txtVolumeCommandRelativeMix,
            lblVolumeCommandAbsoluteMix, lblVolumeCommandRelativeMix, 
            ClearAbsoluteVolumeErrorMix, ClearRelativeVolumeProportionMixError,false);
        }

        private void cbAbsoluteSpecifiedMix_CheckedChanged(object sender, EventArgs e)
        {
            updateCheckboxAndTextbox(out myVolumeTypeSpecifierMix, cbRelativeSpecifiedMix, cbAbsoluteSpecifiedMix,
                  txtVolumeCommandRelativeMix, txtVolumeCommandAbsoluteMix,
            lblVolumeCommandRelativeMix, lblVolumeCommandAbsoluteMix,
            ClearRelativeVolumeProportionMixError, ClearAbsoluteVolumeErrorMix, true);
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

        private void cbRelativeSpecified3_CheckedChanged(object sender, EventArgs e)
        {
            updateCheckboxAndTextbox(out myVolumeTypeSpecifier3, cbAbsoluteSpecified3, cbRelativeSpecified3,
                txtVolumeCommandAbsolute3, txtVolumeCommandRelative3,
                    lblVolumeCommandAbsolute3, lblVolumeCommandRelative3,
                    ClearAbsoluteVolumeError3, ClearRelativeVolumeProportion3Error, false);
        }

        private void cbAbsoluteSpecified3_CheckedChanged(object sender, EventArgs e)
        {
            updateCheckboxAndTextbox(out myVolumeTypeSpecifier3, cbRelativeSpecified3, cbAbsoluteSpecified3,
                  txtVolumeCommandRelative3, txtVolumeCommandAbsolute3,
            lblVolumeCommandRelative3, lblVolumeCommandAbsolute3,
            ClearRelativeVolumeProportion3Error, ClearAbsoluteVolumeError3, true);
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

        private void txtVolumeCommandRelative3_TextChanged(object sender, EventArgs e)
        {
            txtVolumeCommandRelative_helper(CheckTransportDestVial3, cbRelativeSpecified3, txtVolumeCommandRelative3,
                     ClearRelativeVolumeProportion3Error, ShowRelativeVolumeProportion3Error, ref myRelativeVolumeProportion3);
        }

        private void txtVolumeCommandRelativeMix_TextChanged(object sender, EventArgs e)
        {
            txtVolumeCommandRelative_helper(null, cbRelativeSpecifiedMix, txtVolumeCommandRelativeMix,
                     ClearRelativeVolumeProportionMixError, ShowRelativeVolumeProportionMixError, ref myRelativeVolumeProportionMix);
        }


        
        private void txtVolumeCommandAbsolute2_TextChanged(object sender, EventArgs e)
        {
            txtVolumeCommandAbsolute_helper(cbAbsoluteSpecified2, txtVolumeCommandAbsolute2, ClearAbsoluteVolumeError2,
                                             ShowAbsoluteVolumeError2, ref myAbsoluteVolume_uL2, (string)cmbSourceVial2.SelectedItem);
        }

        private void txtVolumeCommandAbsolute3_TextChanged(object sender, EventArgs e)
        {
            txtVolumeCommandAbsolute_helper(cbAbsoluteSpecified3, txtVolumeCommandAbsolute3, ClearAbsoluteVolumeError3,
                                           ShowAbsoluteVolumeError3, ref myAbsoluteVolume_uL3, (string)cmbSourceVial3.SelectedItem);
        
        }

        private void txtVolumeCommandAbsoluteMix_TextChanged(object sender, EventArgs e)
        {

            txtVolumeCommandAbsolute_helper(cbAbsoluteSpecifiedMix, txtVolumeCommandAbsoluteMix, ClearAbsoluteVolumeErrorMix,
                                            ShowAbsoluteVolumeErrorMix, ref myAbsoluteVolume_uLMix, (string)cmbSourceVialMix.SelectedItem);
            
        
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

            cmbSourceVialMix.SelectedIndex = cmbDestinationVial.SelectedIndex;
            cmbDestinationVialMix.SelectedIndex = cmbDestinationVial.SelectedIndex;
            cmbSourceVial2.SelectedIndex = cmbDestinationVial.SelectedIndex;
        }

        private void cmbDestinationVialMix_SelectedIndexChanged(object sender, EventArgs e)
        {
            //
        }

        private void cmbDestinationVial2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (boNotReady) return;
            ReportCommandDetailChanged();
            CheckTransportDestVial2();
            cmbSourceVial3.SelectedIndex = cmbDestinationVial2.SelectedIndex;
        }

        private void cmbDestinationVial3_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReportCommandDetailChanged();
            CheckTransportDestVial3(); 
        }

        private void TopUpMixTransSepTransCommandPanel_VisibleChanged(object sender, EventArgs e)
        {
            //
        }

        private void cbxMixCycles_SelectedIndexChanged(object sender, EventArgs e)
        {
            MixCycles = cbxMixCycles.SelectedIndex + 1;
            ReportCommandDetailChanged();
        }

        private void txtTipTubeBottomGap_TextChanged(object sender, EventArgs e)
        {
            try
            {
                TipTubeBottomGap = int.Parse(txtTipTubeBottomGap.Text);
            }
            catch
            {
                TipTubeBottomGap = 0;
            }
            ReportCommandDetailChanged();
        }

        private void txtSeparationProcessingTime_TextChanged(object sender, EventArgs e)
        {
            ReportCommandDetailChanged();
            if (txtSeparationProcessingTime.TextLength == 0)
            {
                ShowSeparationProcessingTimeError();
            }
            else
            {
                try
                {
                    mySeparationProcessingTime = int.Parse(txtSeparationProcessingTime.Text);
                    if (mySeparationProcessingTime < 0)
                    {
                        ShowSeparationProcessingTimeError();
                    }
                    else
                    {
                        ClearSeparationProcessingTimeError();
                    }
                }
                catch
                {
                    mySeparationProcessingTime = -1;
                }
            }
        }
        private void ShowSeparationProcessingTimeError()
        {
            errorSeparationProcessingTime.SetIconAlignment(
                txtSeparationProcessingTime, ErrorIconAlignment.MiddleLeft);
            errorSeparationProcessingTime.SetError(
                txtSeparationProcessingTime, "Separation processing time must be >= 0");
        }

        private void ClearSeparationProcessingTimeError()
        {
            errorSeparationProcessingTime.SetError(txtSeparationProcessingTime, string.Empty);
        }

        //tiprack, tiptube bottom mixcycers, wait time _textchange
	}
}
