//----------------------------------------------------------------------------
// VolumeCommandPanel
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
using System.Collections.Generic;
using Tesla.ProtocolEditorModel;

namespace Tesla.ProtocolEditorControls
{
    public class VolumeCommandPanel : Tesla.ProtocolEditorControls.CommandPanel
	{
		// IAT - made all public so can be manipulated by child classes
		public System.Windows.Forms.Label			lblVolumeCommandSourceVial; 
		public System.Windows.Forms.Label			lblVolumeCommandDestinationVial;
		public System.Windows.Forms.ErrorProvider	errorAbsoluteVolume;
		public System.Windows.Forms.ErrorProvider	errorRelativeVolumeProportion; 
		public System.Windows.Forms.ComboBox		cmbSourceVial; 
		public System.Windows.Forms.ComboBox		cmbDestinationVial;
		private System.ComponentModel.IContainer	components;
		public System.Windows.Forms.ComboBox		cmbVolumeTipRack; 
		public System.Windows.Forms.CheckBox		cbVolumeTipRack; 
		public System.Windows.Forms.ToolTip			tipRelative;
		public System.Windows.Forms.ToolTip			tipAbsolute;
		public System.Windows.Forms.Label			lblVolumeTipRack;
		public System.Windows.Forms.CheckBox		cbAbsoluteSpecified;
		public System.Windows.Forms.Label			lblRelativeVolume;
		public System.Windows.Forms.Label			lblAbsoluteProportion;
		public System.Windows.Forms.CheckBox		cbRelativeSpecified;
        public System.Windows.Forms.TextBox txtVolumeCommandAbsolute;
		public System.Windows.Forms.Label			lblVolumeCommandAbsolute;
        public System.Windows.Forms.TextBox txtVolumeCommandRelative;
		public System.Windows.Forms.Label			lblVolumeCommandRelative;
		public System.Windows.Forms.Label			lblRelativeProportion;
		public System.Windows.Forms.Label			lblAbsoluteVolume; 
		public System.Windows.Forms.ErrorProvider	errorcanttransport;

        public const int MAX_MIX_CYCLE_RS16 = 10;
        public const int MAX_MIX_CYCLE_RSS = 5;

        // added 2011-09-06
        // provide volume limits from configuration INI file
        public int maximumCapacity_ReagentTipVolume_ul;
        public int maximumCapacity_SampleTipVolume_ul;
        public double minimumRelative_TipVolume;
        public double minimumRelative_Ratio;
        public double maximumRelative_Ratio;
        public double minimumRelative_SampleTopUp_Ratio;
        public double maximumRelative_SampleTopUp_Ratio;


        #region Construction/destruction
 
		public VolumeCommandPanel()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			isVolumeSpecificationRequired=true;

			//setup tool tips
    		tipRelative.SetToolTip(this.cbRelativeSpecified, "Addition relative to sample volume.");
    		tipAbsolute.SetToolTip(this.cbAbsoluteSpecified, "Absolute addition.");

			// Initialise the source/destination lists
            ComboBox[] boxes = { cmbSourceVial, cmbDestinationVial };
            FillComboBoxesWithVialNames(boxes);

			isThresholdStyle=false;
			isChangeAllowed=true;

            UpateLiquidVolumeLimits();
		}

        #region filling and updating comboboxes

        public string VialIDToQuadrantString(AbsoluteResourceLocation location)
        {
            string itemDescription = string.Empty;
            switch (location)
            {
                // 0001 - 0099: Global instrument resources
                case AbsoluteResourceLocation.TPC0001:
                case AbsoluteResourceLocation.TPC0034:
                case AbsoluteResourceLocation.TPC0056:
                    itemDescription = "";
                    break;

                case AbsoluteResourceLocation.TPC0101:
                case AbsoluteResourceLocation.TPC0102:
                case AbsoluteResourceLocation.TPC0103:
                case AbsoluteResourceLocation.TPC0104:
                case AbsoluteResourceLocation.TPC0105:
                case AbsoluteResourceLocation.TPC0106:
                case AbsoluteResourceLocation.TPC0107:
                    //case AbsoluteResourceLocation.TPC0108:
                    itemDescription = "Q1, ";
                    break;

                case AbsoluteResourceLocation.TPC0201:
                case AbsoluteResourceLocation.TPC0202:
                case AbsoluteResourceLocation.TPC0203:
                case AbsoluteResourceLocation.TPC0204:
                case AbsoluteResourceLocation.TPC0205:
                case AbsoluteResourceLocation.TPC0206:
                case AbsoluteResourceLocation.TPC0207:
                    //case AbsoluteResourceLocation.TPC0208:
                    itemDescription = "Q2, ";
                    break;
                case AbsoluteResourceLocation.TPC0301:
                case AbsoluteResourceLocation.TPC0302:
                case AbsoluteResourceLocation.TPC0303:
                case AbsoluteResourceLocation.TPC0304:
                case AbsoluteResourceLocation.TPC0305:
                case AbsoluteResourceLocation.TPC0306:
                case AbsoluteResourceLocation.TPC0307:
                    //case AbsoluteResourceLocation.TPC0308:
                    itemDescription = "Q3, ";
                    break;
                case AbsoluteResourceLocation.TPC0401:
                case AbsoluteResourceLocation.TPC0402:
                case AbsoluteResourceLocation.TPC0403:
                case AbsoluteResourceLocation.TPC0404:
                case AbsoluteResourceLocation.TPC0405:
                case AbsoluteResourceLocation.TPC0406:
                case AbsoluteResourceLocation.TPC0407:
                //case AbsoluteResourceLocation.TPC0408:
                    itemDescription = "Q4, ";
                    break;

                default:
                    itemDescription = string.Empty;
                    break;
            }
            return itemDescription;
        }
        public string VialIDToVialString(AbsoluteResourceLocation location)
        {
            string itemDescription = string.Empty;
            switch (location)
            {
                // 0001 - 0099: Global instrument resources
                case AbsoluteResourceLocation.TPC0001:
                    itemDescription = SeparatorResourceManager.GetSeparatorString(
                        StringId.QuadrantBuffer);
                    break;
                case AbsoluteResourceLocation.TPC0034:
                    itemDescription = SeparatorResourceManager.GetSeparatorString(
                        StringId.QuadrantBuffer34);
                    break;
                case AbsoluteResourceLocation.TPC0056:
                    itemDescription = SeparatorResourceManager.GetSeparatorString(
                        StringId.QuadrantBuffer56);
                    break;

                case AbsoluteResourceLocation.TPC0101:
                case AbsoluteResourceLocation.TPC0201:
                case AbsoluteResourceLocation.TPC0301:
                case AbsoluteResourceLocation.TPC0401:
                    itemDescription = SeparatorResourceManager.GetSeparatorString(StringId.WasteTube);
                    break;
                case AbsoluteResourceLocation.TPC0102:
                case AbsoluteResourceLocation.TPC0202:
                case AbsoluteResourceLocation.TPC0302:
                case AbsoluteResourceLocation.TPC0402:
                    itemDescription = SeparatorResourceManager.GetSeparatorString(StringId.LysisBufferTube);
                    break;
                case AbsoluteResourceLocation.TPC0103:
                case AbsoluteResourceLocation.TPC0203:
                case AbsoluteResourceLocation.TPC0303:
                case AbsoluteResourceLocation.TPC0403:
                    itemDescription = SeparatorResourceManager.GetSeparatorString(StringId.VialB);
                    break;
                case AbsoluteResourceLocation.TPC0104:
                case AbsoluteResourceLocation.TPC0204:
                case AbsoluteResourceLocation.TPC0304:
                case AbsoluteResourceLocation.TPC0404:
                    itemDescription = SeparatorResourceManager.GetSeparatorString(StringId.VialA);
                    break;
                case AbsoluteResourceLocation.TPC0105:
                case AbsoluteResourceLocation.TPC0205:
                case AbsoluteResourceLocation.TPC0305:
                case AbsoluteResourceLocation.TPC0405:
                    itemDescription = SeparatorResourceManager.GetSeparatorString(StringId.VialC);
                    break;
                case AbsoluteResourceLocation.TPC0106:
                case AbsoluteResourceLocation.TPC0206:
                case AbsoluteResourceLocation.TPC0306:
                case AbsoluteResourceLocation.TPC0406:
                    itemDescription = SeparatorResourceManager.GetSeparatorString(StringId.SampleTube);
                    break;
                case AbsoluteResourceLocation.TPC0107:
                case AbsoluteResourceLocation.TPC0207:
                case AbsoluteResourceLocation.TPC0307:
                case AbsoluteResourceLocation.TPC0407:
                    itemDescription = SeparatorResourceManager.GetSeparatorString(StringId.SeparationTube);
                    break;
                /*case AbsoluteResourceLocation.TPC0108:
                  case AbsoluteResourceLocation.TPC0208:
                  case AbsoluteResourceLocation.TPC0308:
                  case AbsoluteResourceLocation.TPC0408:
                    itemDescription = SeparatorResourceManager.GetSeparatorString(StringId.TipsBox);
                    break;*/
                default:
                    itemDescription = string.Empty;
                    break;
            }
            return itemDescription;
        }

        //Update uses hardcoded values assoicated with fill
        public void FillComboBoxesWithVialNames(ComboBox[] boxes)
        {
            foreach (ComboBox cmb in boxes)
            {
                cmb.Items.Clear();
            }
            for (int i = 0; i < (int)AbsoluteResourceLocation.NUM_LOCATIONS; ++i)
            {
                AbsoluteResourceLocation location = (AbsoluteResourceLocation)i;

                if (!SeparatorResourceManager.isPlatformRS16())
                {
                    //no buffer34 or buffer56 if not RS-16
                    if (location == AbsoluteResourceLocation.TPC0034 || location == AbsoluteResourceLocation.TPC0056) continue;
                }


                string itemDescription = string.Empty;
                string itemDescription1 = VialIDToQuadrantString(location);
                string itemDescription2 = VialIDToVialString(location);

                if (itemDescription2 != string.Empty)
                {
                    itemDescription = itemDescription1 + itemDescription2;
                    foreach (ComboBox cmb in boxes)
                    {
                        cmb.Items.Add(itemDescription);
                    }
                }
            }

            foreach (ComboBox cmb in boxes)
            {
                cmb.SelectedIndex = 0;
            }

        }

        public void UpateLiquidVolumeLimits()
        {
            VolumeLimits volumeLimits = VolumeLimits.GetInstance();
            volumeLimits.UpateLiquidVolumeLimits();

            maximumCapacity_ReagentTipVolume_ul = volumeLimits.maximumCapacity_ReagentTipVolume_ul;
            maximumCapacity_SampleTipVolume_ul = volumeLimits.maximumCapacity_SampleTipVolume_ul;
            minimumRelative_TipVolume = volumeLimits.minimumRelative_TipVolume;
            minimumRelative_Ratio = volumeLimits.minimumRelative_Ratio;
            maximumRelative_Ratio = volumeLimits.maximumRelative_Ratio;
             minimumRelative_SampleTopUp_Ratio = volumeLimits.minimumRelative_SampleTopUp_Ratio;
             maximumRelative_SampleTopUp_Ratio = volumeLimits.maximumRelative_SampleTopUp_Ratio;
        }

        public void UpdateComboBoxes(ComboBox[] boxes, ProtocolClass pc)
        {
            string vialName = string.Empty;

            //remember idx
            List<int> indexes = new List<int>();
            foreach (ComboBox cmb in boxes)
            {
                indexes.Add(cmb.SelectedIndex);
            }

            //refill comboxes because of robosep type (buffer34 an buffer56)
            FillComboBoxesWithVialNames(boxes);

            //replace vial names
            int[] lysisIdx = { 2, 9, 16, 23 };
            int[] cocktailIdx = { 3, 10, 17, 24 };
            if (SeparatorResourceManager.isPlatformRS16())
            {
                for (int index = 0; index < lysisIdx.Length; index++)
                {
                    lysisIdx[index] += 2;
                    cocktailIdx[index] += 2;
                }
            }

            //determine lysis or neg fraction tube
            switch (pc)
            {
                case ProtocolClass.WholeBloodPositive:
                    vialName = SeparatorResourceManager.GetSeparatorString(StringId.LysisBufferTube);
                    break;
                case ProtocolClass.WholeBloodNegative:
                    vialName = SeparatorResourceManager.GetSeparatorString(StringId.LysisBufferNegativeFractionTube);
                    break;
                default:
                    vialName = SeparatorResourceManager.GetSeparatorString(StringId.NegativeFractionTube);
                    break;
            }
            foreach (int i in lysisIdx)
            {
                foreach (ComboBox cmb in boxes)
                {
                    cmb.Items.RemoveAt(i);
                    cmb.Items.Insert(i, "Q" + (((i - 2) / 7) + 1) + ", " + vialName);
                }
            }

            //determine selection cocktail type
            switch (pc)
            {
                case ProtocolClass.Positive:
                case ProtocolClass.HumanPositive:
                case ProtocolClass.MousePositive:
                case ProtocolClass.WholeBloodPositive:
                    vialName = SeparatorResourceManager.GetSeparatorString(StringId.VialBpos);
                    break;
                case ProtocolClass.Negative:
                case ProtocolClass.HumanNegative:
                case ProtocolClass.MouseNegative:
                case ProtocolClass.WholeBloodNegative:
                    vialName = SeparatorResourceManager.GetSeparatorString(StringId.VialBneg);
                    break;
                default:
                    vialName = SeparatorResourceManager.GetSeparatorString(StringId.VialB);
                    break;
            }
            foreach (int i in cocktailIdx)
            {
                foreach (ComboBox cmb in boxes)
                {
                    cmb.Items.RemoveAt(i);
                    cmb.Items.Insert(i, "Q" + (((i - 3) / 7) + 1) + ", " + vialName);
                }
            }

            //set idx back
            for (int i = 0; i < indexes.Count;i++)
            {
                boxes[i].SelectedIndex = (indexes[i]<boxes[i].Items.Count)?indexes[i]:0;
            }
        }

        #endregion


        public virtual void UpdateComboBoxes(ProtocolClass pc)
		{
            ComboBox[] boxes = { cmbSourceVial, cmbDestinationVial };
            UpdateComboBoxes(boxes, pc);
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

        public bool IsContentValid_helper(bool isContentValid, VolumeType type, TextBox txtRel, double fRelValue, ErrorProvider errRel,
            TextBox txtAbs, double fAbsValue, ErrorProvider errAbs, bool isVolSpecReq)
        {
            if (type == VolumeType.Relative)
            {
                isContentValid &= txtRel.Text.Length > 0 && fRelValue >= 0.0f && errRel.GetError(txtRel) == string.Empty;
            }
            else if (type == VolumeType.Absolute)
            {
                isContentValid &= txtAbs.Text.Length > 0 && fAbsValue >= 0 && errAbs.GetError(txtAbs) == string.Empty;
            }
            else if (isVolSpecReq)
            {
                errAbs.SetError(txtAbs, "One of Absolute or Relative must be selected.");
                errRel.SetError(txtRel, "One of Absolute or Relative must be selected.");
                isContentValid = false;
            }
            return isContentValid;
        }

		public override bool IsContentValid()
		{
			bool isContentValid = base.IsContentValid();

            isContentValid = IsContentValid_helper(isContentValid, myVolumeTypeSpecifier,
                txtVolumeCommandRelative, myRelativeVolumeProportion, errorRelativeVolumeProportion,
                txtVolumeCommandAbsolute, myAbsoluteVolume_uL, errorAbsoluteVolume, isVolumeSpecificationRequired);

			return isContentValid;
		}

        public AbsoluteResourceLocation ComboIndexToVialID(int idx)
        {
            if (!SeparatorResourceManager.isPlatformRS16())
            {
                //update index because of lack of buffer34 and buffer56
                if (idx > 0)
                {
                    idx += 2;
                }
            }
            System.Array locationIds = Enum.GetValues(typeof(AbsoluteResourceLocation));
            return (AbsoluteResourceLocation)locationIds.GetValue(idx);
        }
        public int VialIDToComboIndex(AbsoluteResourceLocation location)
        {
            int i = -1;
            if (Enum.IsDefined(typeof(AbsoluteResourceLocation), location))
            {
                System.Array locationIds = Enum.GetValues(typeof(AbsoluteResourceLocation));
                for (i=0; i < locationIds.GetLength(0); ++i)
                {
                    if (object.Equals(locationIds.GetValue(i), location))
                    {
                        break;
                    }
                }
            }
            if (!SeparatorResourceManager.isPlatformRS16())
            {
                //update index because of lack of buffer34 and buffer56
                if (i > 2)
                {
                    i -= 2;
                }
            }
            return i;
        }

		public AbsoluteResourceLocation SourceVial
		{
			get
			{
                return ComboIndexToVialID(cmbSourceVial.SelectedIndex);
			}
			set
			{
                int idx = VialIDToComboIndex(value);
                if (idx != -1 && idx < cmbSourceVial.Items.Count) cmbSourceVial.SelectedIndex = idx;
			}
		}

		public AbsoluteResourceLocation DestinationVial
		{
			get
			{
                return ComboIndexToVialID(cmbDestinationVial.SelectedIndex);
			}
			set
			{

                int idx = VialIDToComboIndex(value);
                if (idx != -1 && idx < cmbDestinationVial.Items.Count) cmbDestinationVial.SelectedIndex = idx;
			}
		}

		private bool isVolumeSpecificationRequired;

		public bool IsVolumeSpecificationRequired
		{
			get
			{
				return isVolumeSpecificationRequired;
			}
			set
			{
				isVolumeSpecificationRequired = value;
			}
		}

		private bool isThresholdStyle;

		public bool IsThresholdStyle
		{
			get
			{
				return isThresholdStyle;
			}
			set
			{
				isThresholdStyle = value;
			}
		}

		private bool isChangeAllowed;

		public bool IsChangeAllowed
		{
			get
			{
				return isChangeAllowed;
			}
			set
			{
				isChangeAllowed = value;
			}
		}


        protected void SetCtrlFromVolumeType(VolumeType type, CheckBox cbRel, Label lblRel, TextBox txtRel,
            CheckBox cbAbs, Label lblAbs, TextBox txtAbs)
        {
            bool absActive = false;
            bool relActive = false;
            switch (type)
            {
                case VolumeType.Relative:
                case VolumeType.NotSpecified:
                    absActive = false;
                    relActive = true;
                    break;
                case VolumeType.Absolute:
                    absActive = true;
                    relActive = false;
                    break;
            }

            cbAbs.Checked = (type == VolumeType.NotSpecified) ? false : absActive;
            cbRel.Checked = (type == VolumeType.NotSpecified) ? false : relActive;

            lblRel.Visible = relActive;
            lblAbs.Visible = absActive;
            txtRel.Visible = relActive;
            txtAbs.Visible = absActive;
        }

		private VolumeType myVolumeTypeSpecifier;

		public VolumeType VolumeTypeSpecifier
		{
			get
			{
				return myVolumeTypeSpecifier;
			}
			set
            {
                myVolumeTypeSpecifier = value;
                SetCtrlFromVolumeType(value,cbRelativeSpecified,lblVolumeCommandRelative,txtVolumeCommandRelative,
                    cbAbsoluteSpecified,lblVolumeCommandAbsolute,txtVolumeCommandAbsolute);

                this.Refresh();			
            }
		}

		private double myRelativeVolumeProportion;

		public double RelativeVolumeProportion
		{
			get
			{
				return myRelativeVolumeProportion;
			}
			set
			{
				txtVolumeCommandRelative.Text = value.ToString("F4");
			}
		}

		private int myAbsoluteVolume_uL;

		public int AbsoluteVolume_uL
		{
			get
			{
				return myAbsoluteVolume_uL;
			}
			set
			{
				txtVolumeCommandAbsolute.Text = value.ToString();
			}
		}


      	public int TipRack
		{
			set
			{
				if (value>0 && value<5)
					cmbVolumeTipRack.SelectedIndex = value-1;
			}
			get
			{
				if (TipRackSpecified)
					return (cmbVolumeTipRack.SelectedIndex + 1);
				else
					return 1;
			}
		}


      	private bool tipRackSpecified = false;
      	public bool TipRackSpecified
		{
			get
			{
				return tipRackSpecified;
			}
			set
			{
				tipRackSpecified = value;
			}
		}


      	public bool CheckTipRack
		{
			set
			{
				cbVolumeTipRack.Checked = value;
			}
		}

		
      	private bool myUseBufferTip = false;
		public bool UseBufferTip
		{
			get
			{
				return myUseBufferTip;
			}
			set
			{
				myUseBufferTip = value;
			}
		}



		#endregion Properties

        #region Data Entry Error Indicators

        // 2011-09-06 sp
        protected void txtVolumeCommandRelative_TextChanged(object sender, System.EventArgs e)
        {
            // if returns true, we want to stop user from making changes
            if (CheckTransportDestVial()) //IAT
            {
                return;
            }
            ReportCommandDetailChanged();
            if (txtVolumeCommandRelative.TextLength == 0 && cbRelativeSpecified.Checked)
            {
                ShowRelativeVolumeProportionError(VolumeError.MINIMUM_AMOUNT);
            }
            else
            {
                if (isCommandResuspend())
                {
                    ClearRelativeVolumeProportionError();
                }
                else
                {
                    try
                    {
                        if (cbRelativeSpecified.Checked)
                        {
                            myRelativeVolumeProportion = double.Parse(txtVolumeCommandRelative.Text);
                            if ((myRelativeVolumeProportion < minimumRelative_Ratio ||
                                myRelativeVolumeProportion > maximumRelative_Ratio) && !isThresholdStyle)
                            {
                                //                           ShowRelativeVolumeProportionError(0);
                                ShowRelativeVolumeProportionError(VolumeError.RELATIVE_VIAL_AMOUNT);
                            }
                            //                        else if ((myRelativeVolumeProportion < 0.0f ||
                            //                            myRelativeVolumeProportion > 20.0f) && isThresholdStyle)
                            else if ((myRelativeVolumeProportion < minimumRelative_SampleTopUp_Ratio ||
                                myRelativeVolumeProportion > maximumRelative_SampleTopUp_Ratio) && isThresholdStyle)
                            {
                                //                           ShowRelativeVolumeProportionError(20);
                                ShowRelativeVolumeProportionError(VolumeError.RELATIVE_TUBE_AMOUNT);
                            }
                            else
                            {
                                ClearRelativeVolumeProportionError();
                            }
                        }
                        else
                        {
                            ClearRelativeVolumeProportionError();
                        }
                    }
                    catch
                    {
                        myRelativeVolumeProportion = minimumRelative_TipVolume;
                    }
                }
            }
        }


        // 2011-09-06 sp
        // replace with defined error code handling routine if volumes are out of range
        // rewrite 2011-09-06 sp
        public void ShowRelativeVolumeProportionError(VolumeError errorVol)
        {
            ShowRelativeVolumeProportionError(errorVol, errorRelativeVolumeProportion, txtVolumeCommandRelative); 
        }

        public void ClearRelativeVolumeProportionError()
        {
            errorRelativeVolumeProportion.SetError(txtVolumeCommandRelative, string.Empty);
        }

        protected void ShowRelativeVolumeProportionError(VolumeError errorVol, ErrorProvider ep, TextBox txtRel)
        {
            ep.SetIconAlignment(txtRel, ErrorIconAlignment.MiddleLeft);
            switch (errorVol)
            {
                case VolumeError.MINIMUM_AMOUNT:
                    ep.SetError(txtRel, "Proportion must not be empty. ");
                    break;
                case VolumeError.RELATIVE_VIAL_AMOUNT:
                    ep.SetError(txtRel, "Proportion must be between " + minimumRelative_Ratio.ToString() +
                            " and " + maximumRelative_Ratio.ToString());
                    break;
                case VolumeError.RELATIVE_TUBE_AMOUNT:
                    ep.SetError(txtRel, "Proportion must be between " + minimumRelative_SampleTopUp_Ratio.ToString() +
                            " and " + maximumRelative_SampleTopUp_Ratio.ToString());
                    break;
                //case VolumeError.ABSOLUTE_TUBE_AMOUNT:
                  //  ep.SetError(txtRel, "Proportion * Max Volume <= " + maximumAllowable_TubeVolume.ToString() + "uL");
                    //break;
                default:
                    ep.SetError(txtRel, string.Empty);
                    break;
            }
        }


        // 2011-09-06 sp
        // replace using values from configuration INI file

        // 2011-09-06 sp
        // rewritten to use configuration INI values and avoid checking for absolute resuspend/topup
        protected void txtVolumeCommandAbsolute_TextChanged(object sender, System.EventArgs e)
        {
            txtVolumeCommandAbsolute_helper(cbAbsoluteSpecified, txtVolumeCommandAbsolute, ClearAbsoluteVolumeError,
                                            ShowAbsoluteVolumeError, ref myAbsoluteVolume_uL, (string)cmbSourceVial.SelectedItem);
        }

        public delegate void ShowVolumeError_delegate(VolumeError verr);
        public delegate void ClearVolumeError_delegate();
        public delegate void ClearRelativeVolumeProportionError_delegate();
        protected void txtVolumeCommandAbsolute_helper(CheckBox cbAbs, TextBox txtAbs, ClearVolumeError_delegate clearErr,
            ShowVolumeError_delegate showErr,ref int absVol, string src)
        {
            //string src = this.cmbSourceVial.Text;
            ReportCommandDetailChanged();

            int minimumAllowable_TipVolume_ul = 0;

            if (txtAbs.TextLength == 0 && cbAbs.Checked)
            {
                showErr(VolumeError.MINIMUM_AMOUNT);
            }
            else
            {
                try
                {
                    if (cbAbs.Checked)
                    {
                        absVol = int.Parse(txtAbs.Text);
                        if (absVol < minimumAllowable_TipVolume_ul && cbAbs.Checked)
                        {
                            showErr(VolumeError.MINIMUM_AMOUNT);
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
                    //absVol = (short)minimumAllowable_TipVolume_ul;
                    showErr(VolumeError.CANT_CONVERT_ABSOULTE_AMOUNT);
                }

            }
        }

        // 2011-11-03 sp 
        // check for top up vial commands
        protected bool isCommandTopup()
        {
            if (this.CommandType.Equals("Top Up Vial"))
                return true;
            else
                return false;
        }

        // 2011-11-03 sp 
        // check for resuspend commands
        protected bool isCommandResuspend()
        {
            if (this.CommandType.Equals("Resuspend Vial"))
                return true;
            else
                return false;
        }


        public bool isVialStringSmallVial(string txtVial)
        {
            return (txtVial.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.VialA)) >= 0) ||
                (txtVial.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.VialB)) >= 0) ||
                (txtVial.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.VialC)) >= 0);
        }

		// if "buffer bottle" is selected as the destination vial for Transport, disable
		// user ability to edit absolute and relative volumes.
		//Added extra if statement that checks to make sure destination isnt a small vial
        protected delegate void setEnabled_delegate(bool enab);
        protected delegate void ShowCantTransportError_delegate();
        protected delegate void ClearCantTransportError_delegate();
        protected bool CheckTransportDestVial_helper(ComboBox cmbDest, bool isTransport, setEnabled_delegate setCtlEnabled,
            ShowCantTransportError_delegate showErr, ClearCantTransportError_delegate clearErr)
        {
            bool disabledControlsFlag = false;
            string dst = cmbDest.Text;

            if (isTransport && (dst == "Buffer Bottle"))
            {
                setCtlEnabled(false);
                disabledControlsFlag = true; // disabled controls
            }
            else if (isTransport && isVialStringSmallVial(dst) && !ProtocolTransportCommand.canReagentVialBeTransportDestination()) //COME BACK
            {
                // if the destination is a small vial create an error:
                showErr();
            }
            else
            {
                // clear error if destination is ok:
                setCtlEnabled(true);
                disabledControlsFlag = false;
                clearErr();
            }
            return disabledControlsFlag;
        }

		public virtual bool CheckTransportDestVial() //IAT
		{
            return CheckTransportDestVial_helper(cmbDestinationVial, (this.CommandType == "Transport"),
                setEnabled, ShowCantTransportError, ClearCantTransportError);
		}


        protected void setEnabled_helper(bool enab, bool isThresStyle, CheckBox cbRel, CheckBox cbAbs, TextBox txtRel, Label lblRel, TextBox txtAbs, Label lblAbs)
        {
            cbRel.Enabled = enab;
            cbAbs.Enabled = enab;
            if (!isThresStyle)
            {
                txtRel.Enabled = enab;
                lblRel.Enabled = enab;

                txtAbs.Enabled = enab;
                lblAbs.Enabled = enab;

                if (enab)
                {
                    txtRel.Enabled = cbRel.Checked;
                    txtAbs.Enabled = cbAbs.Checked;
                }
            }

        }
		protected void setEnabled(bool enab)
		{
            setEnabled_helper(enab, isThresholdStyle, cbRelativeSpecified, cbAbsoluteSpecified,
                txtVolumeCommandRelative, lblRelativeProportion, txtVolumeCommandAbsolute, lblAbsoluteVolume);
		}

// bdr	// disable usebuffertip if source is small vial
        //bug fix? should be checking source??
		public bool CheckTransportSourceVial() 
		{
			string src = this.cmbSourceVial.Text;
			bool useBufferTip = true;

            // modify 2011-09-06 sp
            // change to use logic to define the state of the return value rather than returning the default myUseBufferTip when conditions not met
            if ((this.CommandType == "Transport") && isVialStringSmallVial(src))
            		useBufferTip = false;
            else if (this.CommandType != "Transport")
                useBufferTip = false;

            myUseBufferTip = useBufferTip;

            return useBufferTip;
        }

		public void ShowCantTransportError()
		{
			errorcanttransport.SetError(
				cmbDestinationVial, "Can't do transport to the specified vial");
		}

		public void ClearCantTransportError()
		{
			errorcanttransport.SetError(cmbDestinationVial, string.Empty);
		}

        // 2011-09-06 sp
        // replace with defined error code handling routine if volumes are out of range


        public void ShowwVolumeError(VolumeError errorVol)
        {
            switch (myVolumeTypeSpecifier)
            {
                case VolumeType.Absolute:
                    ShowAbsoluteVolumeError(errorVol);
                    break;
                case VolumeType.Relative:
                    ShowRelativeVolumeProportionError(errorVol);
                    break;
                default:
                    break;
            }
        }

        // rewrite 2011-09-06 sp
        public void ShowAbsoluteVolumeError(VolumeError errorVol)
        {
            ShowAbsoluteVolumeError(errorVol, errorAbsoluteVolume, txtVolumeCommandAbsolute);
        }

        public void ClearAbsoluteVolumeError()
        {
            errorAbsoluteVolume.SetError(txtVolumeCommandAbsolute, string.Empty);
        }

        protected void ShowAbsoluteVolumeError(VolumeError errorVol, ErrorProvider ep, TextBox txtAbs)
        {
            int minimumAllowable_TipVolume_ul = 0;
            ep.SetIconAlignment(txtAbs, ErrorIconAlignment.MiddleLeft);
            switch (errorVol)
            {
                case VolumeError.MINIMUM_AMOUNT:
                    ep.SetError(txtAbs, "Volume must be >= " + minimumAllowable_TipVolume_ul.ToString());
                    break;
                //case VolumeError.ABSOLUTE_TUBE_AMOUNT:
                  //  ep.SetError(txtAbs, "Volume must be <= " + maximumAllowable_TubeVolume.ToString());
                    //break;
                case VolumeError.ABSOLUTE_1MLTIP_AMOUNT:
                    ep.SetError(txtAbs, "Volume must be <= " + maximumCapacity_ReagentTipVolume_ul.ToString());
                    break;
                case VolumeError.ABSOLUTE_5MLTIP_AMOUNT:
                    ep.SetError(txtAbs, "Volume must be <= " + maximumCapacity_SampleTipVolume_ul.ToString());
                    break;
                case VolumeError.CANT_CONVERT_ABSOULTE_AMOUNT:
                    ep.SetError(txtAbs, "Volume must be >= 0 and <= 2,147,483,647");
                    break;
                default:
                    ep.SetError(txtAbs, string.Empty);
                    break;
            }
        }


		protected void cmbSourceVial_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			ReportCommandDetailChanged();
			if (!cmbDestinationVial.Enabled) //IAT
			{
				cmbDestinationVial.SelectedIndex = cmbSourceVial.SelectedIndex;
			}
			CheckTransportDestVial(); 
			CheckTransportSourceVial(); 
			ReportCommandDetailChanged();
			base.CommandPanel_VisibleChanged(sender, e);
		}

		virtual protected void cmbDestinationVial_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			ReportCommandDetailChanged();
			CheckTransportDestVial(); //IAT
		}

		protected void VolumeCommandPanel_VisibleChanged(object sender, System.EventArgs e)
		{
			if (this.Visible)
			{
				// Trigger re-evaluation of the error providers
				base.CommandPanel_VisibleChanged(sender, e);
				txtVolumeCommandRelative_TextChanged(sender, e);
				txtVolumeCommandAbsolute_TextChanged(sender, e);
				CheckTransportDestVial(); //IAT
			}
		}

        #endregion Data Entry Error Indicators

		#region Control events

		protected void cbVolumeTipRack_CheckedChanged(object sender, System.EventArgs e)
		{
			cmbVolumeTipRack.Enabled = cbVolumeTipRack.Checked;
			TipRackSpecified = cbVolumeTipRack.Checked;
			ReportCommandDetailChanged();
		}


		// IAT
		protected void cbRelativeSpecified_CheckedChanged(object sender, System.EventArgs e)
		{
	
			if(cbRelativeSpecified.Checked)
			{
				myVolumeTypeSpecifier = VolumeType.Relative;
				
				// if relative is checked, uncheck absolute
				if(cbAbsoluteSpecified.Checked)
					cbAbsoluteSpecified.Checked = false;
				
				ClearAbsoluteVolumeError();
				ClearRelativeVolumeProportionError();
			}
			else if(cbAbsoluteSpecified.Checked)
			{
				myVolumeTypeSpecifier = VolumeType.Absolute;
			}
			else
			{
				myVolumeTypeSpecifier = VolumeType.NotSpecified;
			}
			
			// toggle checkboxes and labels
			txtVolumeCommandRelative.Visible = cbRelativeSpecified.Checked;
			lblVolumeCommandRelative.Visible = cbRelativeSpecified.Checked;
			txtVolumeCommandRelative.Enabled = cbRelativeSpecified.Checked;

			txtVolumeCommandAbsolute.Visible = !cbRelativeSpecified.Checked;
			lblVolumeCommandAbsolute.Visible = (isThresholdStyle)?false:!cbRelativeSpecified.Checked;
			txtVolumeCommandAbsolute.Enabled = cbAbsoluteSpecified.Checked;

			if(isThresholdStyle)
			{
				AllowChangeUpdate();
			}
			//this.Refresh();
			ReportCommandDetailChanged();
		}
		
		//IAT
		protected void cbAbsoluteSpecified_CheckedChanged(object sender, System.EventArgs e)
		{
			if(cbAbsoluteSpecified.Checked)
			{
				myVolumeTypeSpecifier = VolumeType.Absolute;
				
				// if absolute is checked, uncheck relative
				if(cbRelativeSpecified.Checked)
					cbRelativeSpecified.Checked = false;
				
				ClearAbsoluteVolumeError();
				ClearRelativeVolumeProportionError();
			}
			else if(cbRelativeSpecified.Checked)
			{
				myVolumeTypeSpecifier = VolumeType.Relative;
			}
			else
			{
				myVolumeTypeSpecifier = VolumeType.NotSpecified;
			}

			// toggle checkboxes and labels
			txtVolumeCommandRelative.Visible = cbRelativeSpecified.Checked;
			lblVolumeCommandRelative.Visible = cbRelativeSpecified.Checked;
			txtVolumeCommandRelative.Enabled = cbRelativeSpecified.Checked;
			
			txtVolumeCommandAbsolute.Visible = !cbRelativeSpecified.Checked;
			lblVolumeCommandAbsolute.Visible = (isThresholdStyle)?false:!cbRelativeSpecified.Checked;
			txtVolumeCommandAbsolute.Enabled = cbAbsoluteSpecified.Checked;

			if(isThresholdStyle)
			{
                this.AbsoluteVolume_uL = SeparatorResourceManager.GetMaxVolume();
				AllowChangeUpdate();
			}
			//this.Refresh();
			ReportCommandDetailChanged();
		}

		protected void AllowChangeUpdate()
		{
			if(isThresholdStyle)
			{
				if(isChangeAllowed)
				{
					txtVolumeCommandRelative.Enabled	= cbRelativeSpecified.Checked;
				}
				else
				{
					txtVolumeCommandRelative.Enabled	= false;
				}
				txtVolumeCommandAbsolute.Enabled    = false;
			}
		}

		#endregion Control events

		#region Control overrides

		protected override void OnPaint(PaintEventArgs e)
		{
				// Hide the relative/absolute volume entry controls if relative/absolute volume
				// is not relevant in the current case (e.g. "Mix" command)
				if (isVolumeSpecificationRequired)
				{
					cbAbsoluteSpecified.Visible			= true; //IAT
					cbRelativeSpecified.Visible			= true; //IAT
					lblAbsoluteVolume.Visible			= true; //IAT
					lblRelativeProportion.Visible		= true; //IAT
					
					lblVolumeCommandRelative.Visible	= cbRelativeSpecified.Checked;
					lblVolumeCommandAbsolute.Visible	= (isThresholdStyle)?false:!cbRelativeSpecified.Checked; //IAT
					txtVolumeCommandRelative.Visible	= cbRelativeSpecified.Checked; //IAT
					txtVolumeCommandAbsolute.Visible	= !cbRelativeSpecified.Checked; //IAT

					txtVolumeCommandRelative.Enabled	= cbRelativeSpecified.Checked && cbRelativeSpecified.Enabled; //IAT
					txtVolumeCommandAbsolute.Enabled	= cbAbsoluteSpecified.Checked && cbAbsoluteSpecified.Enabled; //IAT

					if(isThresholdStyle)
					{
						lblAbsoluteVolume.Text = "Threshold Volume";
						AllowChangeUpdate();
					}
					else
					{
						lblAbsoluteVolume.Text = "Absolute Volume";
					}
				

				}
				else
				{				
					cbAbsoluteSpecified.Visible			= false; //IAT
					cbRelativeSpecified.Visible			= false; //IAT
					lblAbsoluteVolume.Visible			= false; //IAT
					lblRelativeProportion.Visible		= false; //IAT
					
					lblVolumeCommandRelative.Visible	= false;
					lblVolumeCommandAbsolute.Visible	= false; //IAT
					txtVolumeCommandRelative.Visible	= false; //IAT
					txtVolumeCommandAbsolute.Visible	= false; //IAT

					txtVolumeCommandRelative.Enabled	= false; //IAT
					txtVolumeCommandAbsolute.Enabled	= false; //IAT
				}
			base.OnPaint(e);	
		}

		#endregion Control overrides

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
            this.lblVolumeCommandSourceVial = new System.Windows.Forms.Label();
            this.lblVolumeCommandDestinationVial = new System.Windows.Forms.Label();
            this.txtVolumeCommandRelative = new System.Windows.Forms.TextBox();
            this.lblVolumeCommandRelative = new System.Windows.Forms.Label();
            this.txtVolumeCommandAbsolute = new System.Windows.Forms.TextBox();
            this.lblVolumeCommandAbsolute = new System.Windows.Forms.Label();
            this.errorRelativeVolumeProportion = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorAbsoluteVolume = new System.Windows.Forms.ErrorProvider(this.components);
            this.cmbSourceVial = new System.Windows.Forms.ComboBox();
            this.cmbDestinationVial = new System.Windows.Forms.ComboBox();
            this.cmbVolumeTipRack = new System.Windows.Forms.ComboBox();
            this.cbVolumeTipRack = new System.Windows.Forms.CheckBox();
            this.lblVolumeTipRack = new System.Windows.Forms.Label();
            this.tipRelative = new System.Windows.Forms.ToolTip(this.components);
            this.tipAbsolute = new System.Windows.Forms.ToolTip(this.components);
            this.cbAbsoluteSpecified = new System.Windows.Forms.CheckBox();
            this.cbRelativeSpecified = new System.Windows.Forms.CheckBox();
            this.lblRelativeProportion = new System.Windows.Forms.Label();
            this.lblAbsoluteVolume = new System.Windows.Forms.Label();
            this.errorcanttransport = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorRelativeVolumeProportion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorAbsoluteVolume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorcanttransport)).BeginInit();
            this.SuspendLayout();
            // 
            // lblVolumeCommandSourceVial
            // 
            this.lblVolumeCommandSourceVial.Location = new System.Drawing.Point(14, 62);
            this.lblVolumeCommandSourceVial.Name = "lblVolumeCommandSourceVial";
            this.lblVolumeCommandSourceVial.Size = new System.Drawing.Size(48, 18);
            this.lblVolumeCommandSourceVial.TabIndex = 6;
            this.lblVolumeCommandSourceVial.Text = "Source";
            // 
            // lblVolumeCommandDestinationVial
            // 
            this.lblVolumeCommandDestinationVial.Location = new System.Drawing.Point(14, 86);
            this.lblVolumeCommandDestinationVial.Name = "lblVolumeCommandDestinationVial";
            this.lblVolumeCommandDestinationVial.Size = new System.Drawing.Size(63, 18);
            this.lblVolumeCommandDestinationVial.TabIndex = 8;
            this.lblVolumeCommandDestinationVial.Text = "Destination";
            // 
            // txtVolumeCommandRelative
            // 
            this.txtVolumeCommandRelative.Enabled = false;
            this.txtVolumeCommandRelative.Location = new System.Drawing.Point(408, 110);
            this.txtVolumeCommandRelative.Name = "txtVolumeCommandRelative";
            this.txtVolumeCommandRelative.Size = new System.Drawing.Size(116, 20);
            this.txtVolumeCommandRelative.TabIndex = 15;
            this.txtVolumeCommandRelative.TextChanged += new System.EventHandler(this.txtVolumeCommandRelative_TextChanged);
            // 
            // lblVolumeCommandRelative
            // 
            this.lblVolumeCommandRelative.Location = new System.Drawing.Point(323, 110);
            this.lblVolumeCommandRelative.Name = "lblVolumeCommandRelative";
            this.lblVolumeCommandRelative.Size = new System.Drawing.Size(79, 20);
            this.lblVolumeCommandRelative.TabIndex = 14;
            this.lblVolumeCommandRelative.Text = "Proportion";
            this.lblVolumeCommandRelative.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtVolumeCommandAbsolute
            // 
            this.txtVolumeCommandAbsolute.Enabled = false;
            this.txtVolumeCommandAbsolute.Location = new System.Drawing.Point(408, 110);
            this.txtVolumeCommandAbsolute.Name = "txtVolumeCommandAbsolute";
            this.txtVolumeCommandAbsolute.Size = new System.Drawing.Size(116, 20);
            this.txtVolumeCommandAbsolute.TabIndex = 16;
            this.txtVolumeCommandAbsolute.Visible = false;
            this.txtVolumeCommandAbsolute.TextChanged += new System.EventHandler(this.txtVolumeCommandAbsolute_TextChanged);
            // 
            // lblVolumeCommandAbsolute
            // 
            this.lblVolumeCommandAbsolute.Location = new System.Drawing.Point(323, 110);
            this.lblVolumeCommandAbsolute.Name = "lblVolumeCommandAbsolute";
            this.lblVolumeCommandAbsolute.Size = new System.Drawing.Size(79, 20);
            this.lblVolumeCommandAbsolute.TabIndex = 15;
            this.lblVolumeCommandAbsolute.Text = "Value (uL)";
            this.lblVolumeCommandAbsolute.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // errorRelativeVolumeProportion
            // 
            this.errorRelativeVolumeProportion.ContainerControl = this;
            // 
            // errorAbsoluteVolume
            // 
            this.errorAbsoluteVolume.ContainerControl = this;
            // 
            // cmbSourceVial
            // 
            this.cmbSourceVial.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSourceVial.Location = new System.Drawing.Point(78, 59);
            this.cmbSourceVial.Name = "cmbSourceVial";
            this.cmbSourceVial.Size = new System.Drawing.Size(202, 21);
            this.cmbSourceVial.TabIndex = 7;
            this.cmbSourceVial.SelectedIndexChanged += new System.EventHandler(this.cmbSourceVial_SelectedIndexChanged);
            // 
            // cmbDestinationVial
            // 
            this.cmbDestinationVial.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDestinationVial.Location = new System.Drawing.Point(78, 83);
            this.cmbDestinationVial.Name = "cmbDestinationVial";
            this.cmbDestinationVial.Size = new System.Drawing.Size(202, 21);
            this.cmbDestinationVial.TabIndex = 9;
            this.cmbDestinationVial.SelectedIndexChanged += new System.EventHandler(this.cmbDestinationVial_SelectedIndexChanged);
            // 
            // cmbVolumeTipRack
            // 
            this.cmbVolumeTipRack.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVolumeTipRack.Enabled = false;
            this.cmbVolumeTipRack.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4"});
            this.cmbVolumeTipRack.Location = new System.Drawing.Point(361, 83);
            this.cmbVolumeTipRack.Name = "cmbVolumeTipRack";
            this.cmbVolumeTipRack.Size = new System.Drawing.Size(48, 21);
            this.cmbVolumeTipRack.TabIndex = 18;
            this.cmbVolumeTipRack.SelectedIndexChanged += new System.EventHandler(this.cmbVolumeTipRack_SelectedIndexChanged);
            // 
            // cbVolumeTipRack
            // 
            this.cbVolumeTipRack.Location = new System.Drawing.Point(337, 83);
            this.cbVolumeTipRack.Name = "cbVolumeTipRack";
            this.cbVolumeTipRack.Size = new System.Drawing.Size(16, 24);
            this.cbVolumeTipRack.TabIndex = 19;
            this.cbVolumeTipRack.CheckedChanged += new System.EventHandler(this.cbVolumeTipRack_CheckedChanged);
            // 
            // lblVolumeTipRack
            // 
            this.lblVolumeTipRack.Location = new System.Drawing.Point(281, 86);
            this.lblVolumeTipRack.Name = "lblVolumeTipRack";
            this.lblVolumeTipRack.Size = new System.Drawing.Size(56, 16);
            this.lblVolumeTipRack.TabIndex = 20;
            this.lblVolumeTipRack.Text = "Tip Rack";
            // 
            // cbAbsoluteSpecified
            // 
            this.cbAbsoluteSpecified.Location = new System.Drawing.Point(264, 110);
            this.cbAbsoluteSpecified.Name = "cbAbsoluteSpecified";
            this.cbAbsoluteSpecified.Size = new System.Drawing.Size(16, 24);
            this.cbAbsoluteSpecified.TabIndex = 11;
            this.cbAbsoluteSpecified.CheckedChanged += new System.EventHandler(this.cbAbsoluteSpecified_CheckedChanged);
            // 
            // cbRelativeSpecified
            // 
            this.cbRelativeSpecified.Location = new System.Drawing.Point(124, 110);
            this.cbRelativeSpecified.Name = "cbRelativeSpecified";
            this.cbRelativeSpecified.Size = new System.Drawing.Size(16, 24);
            this.cbRelativeSpecified.TabIndex = 22;
            this.cbRelativeSpecified.CheckedChanged += new System.EventHandler(this.cbRelativeSpecified_CheckedChanged);
            // 
            // lblRelativeProportion
            // 
            this.lblRelativeProportion.Location = new System.Drawing.Point(15, 115);
            this.lblRelativeProportion.Name = "lblRelativeProportion";
            this.lblRelativeProportion.Size = new System.Drawing.Size(103, 20);
            this.lblRelativeProportion.TabIndex = 23;
            this.lblRelativeProportion.Text = "Relative Proportion";
            // 
            // lblAbsoluteVolume
            // 
            this.lblAbsoluteVolume.Location = new System.Drawing.Point(146, 115);
            this.lblAbsoluteVolume.Name = "lblAbsoluteVolume";
            this.lblAbsoluteVolume.Size = new System.Drawing.Size(110, 20);
            this.lblAbsoluteVolume.TabIndex = 24;
            this.lblAbsoluteVolume.Text = "Absolute Volume";
            // 
            // errorcanttransport
            // 
            this.errorcanttransport.ContainerControl = this;
            // 
            // VolumeCommandPanel
            // 
            this.Controls.Add(this.lblAbsoluteVolume);
            this.Controls.Add(this.lblRelativeProportion);
            this.Controls.Add(this.cbRelativeSpecified);
            this.Controls.Add(this.cbAbsoluteSpecified);
            this.Controls.Add(this.lblVolumeTipRack);
            this.Controls.Add(this.cbVolumeTipRack);
            this.Controls.Add(this.cmbVolumeTipRack);
            this.Controls.Add(this.cmbDestinationVial);
            this.Controls.Add(this.cmbSourceVial);
            this.Controls.Add(this.lblVolumeCommandDestinationVial);
            this.Controls.Add(this.lblVolumeCommandSourceVial);
            this.Controls.Add(this.lblVolumeCommandRelative);
            this.Controls.Add(this.lblVolumeCommandAbsolute);
            this.Controls.Add(this.txtVolumeCommandRelative);
            this.Controls.Add(this.txtVolumeCommandAbsolute);
            this.EnableExtension = true;
            this.Name = "VolumeCommandPanel";
            this.Size = new System.Drawing.Size(540, 156);
            this.VisibleChanged += new System.EventHandler(this.VolumeCommandPanel_VisibleChanged);
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
            ((System.ComponentModel.ISupportInitialize)(this.errorRelativeVolumeProportion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorAbsoluteVolume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorcanttransport)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		protected void cmbVolumeTipRack_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			TipRack = cmbVolumeTipRack.SelectedIndex + 1;
			ReportCommandDetailChanged();
		}


        public void SetVolumeCommandPanelParams(AbsoluteResourceLocation SourceVial, AbsoluteResourceLocation DestinationVial,
            VolumeType VolumeTypeSpecifier, double RelativeVolumeProportion, int AbsoluteVolume_uL,
            int TipRack, bool TipRackSpecified, string CommandLabel, uint CommandExtensionTime)
        {
            this.SourceVial = SourceVial;
            this.DestinationVial = DestinationVial;
            this.VolumeTypeSpecifier = VolumeTypeSpecifier;


            this.RelativeVolumeProportion = RelativeVolumeProportion;
            this.AbsoluteVolume_uL = AbsoluteVolume_uL;

            this.TipRack = TipRack;
            this.TipRackSpecified = TipRackSpecified;
            this.CommandLabel = CommandLabel;
            this.CommandExtensionTime = CommandExtensionTime;
        }
	
	}
}

