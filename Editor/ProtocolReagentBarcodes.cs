using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Tesla.ProtocolEditorModel;
using Tesla.Common.ResourceManagement;
using Tesla.Common.Protocol;
using Tesla.Common.Separator;

namespace Tesla.ProtocolEditor
{
    public partial class ProtocolReagentBarcodes : Form
    {
        private static ProtocolModel theProtocolModel;
        private string[] vialBarcodes = new string[ProtocolModel.NUM_VIAL_BARCODES_PER_QUAD]; 
        private string squareVialBarcode;
        private string circleVialBarcode;
        private string triangleVialBarcode;
        private ProtocolClass protocolClass = ProtocolClass.HumanPositive;
        public ProtocolReagentBarcodes()
        {
            InitializeComponent();

            // Get a local reference to the protocol model. 
            theProtocolModel = ProtocolModel.GetInstance();

            tipDefaultName.SetToolTip(this.lbDefaultNames, "Select the vial name to add/edit barcode.");
            tipNewName.SetToolTip(this.txtCustomName, "Type the barcode desired for selected vial.");

            myUsedReagentVialList = new List<AbsoluteResourceLocation>();
        }

        // called from CreateReagentBarcodeWindow and Quadrant select tabs
        public void SetupVialBarcodes(int idx)
        {
            if (idx < 0 || idx >= 4) idx = 0;


            // extract existing barcodes from the protocol
            try
            {
                theProtocolModel.GetVialBarcodes(idx, out vialBarcodes);
            }
            catch (Exception) { }

            // old protocols have no barcodes set
            if ((vialBarcodes[0] == "") || (vialBarcodes[0] == null))
            {
                SetDefaultVialBarcodes();
                theProtocolModel.ApplyVialBarcodes(idx, vialBarcodes);
            }
            //quadrant = vialBarcodes[0];
            squareVialBarcode = vialBarcodes[1];
            circleVialBarcode = vialBarcodes[2];
            triangleVialBarcode = vialBarcodes[3];

            /*
            if (quadrant == "") //not used
            {
                cbQuadrantRequired.Checked = false;
            }
            else if (squareVialBarcode != "" || circleVialBarcode != "" || triangleVialBarcode != "")
            {
                //used with some barcode info
                cbQuadrantRequired.Checked = false;
            }
            else
            {
                //used with no barcode info
                cbQuadrantRequired.Checked = true;
            }
             */

            // fill protocol's custom names array with names
            this.SetupBarcodeListBox();
        }
        
        private void SetDefaultVialBarcodes()
        {
            vialBarcodes[0] = vialBarcodes[1] = vialBarcodes[2] = vialBarcodes[3] = "";
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
        { System.Diagnostics.Debug.WriteLine(msg); }


        public void SetupListBox()
        {
            //TUBE NAMES SYNC
            lbDefaultNames.Items.Clear();

            //vial B Square
            //vial C Circle
            //vial A Triangle

            //general items
            if ((protocolClass == ProtocolClass.Positive) || (protocolClass == ProtocolClass.HumanPositive) ||
                (protocolClass == ProtocolClass.MousePositive) || (protocolClass == ProtocolClass.WholeBloodPositive))
                lbDefaultNames.Items.Add(SeparatorResourceManager.GetSeparatorString(StringId.VialBpos));
            else if ((protocolClass == ProtocolClass.Negative) || (protocolClass == ProtocolClass.HumanNegative) ||
                (protocolClass == ProtocolClass.MouseNegative) || (protocolClass == ProtocolClass.WholeBloodNegative))
                lbDefaultNames.Items.Add(SeparatorResourceManager.GetSeparatorString(StringId.VialBneg));
            else
                lbDefaultNames.Items.Add(SeparatorResourceManager.GetSeparatorString(StringId.VialB));

            lbDefaultNames.Items.Add(SeparatorResourceManager.GetSeparatorString(StringId.VialC));
            lbDefaultNames.Items.Add(SeparatorResourceManager.GetSeparatorString(StringId.VialA));

            lbDefaultNames.SelectedIndex = 0;
            this.SetupBarcodeListBox();
        }

        private string GetBarcodeString(string barcode, int vialIdx)
        {
            //0 is vial B Square
            //1 is vial C Circle
            //2 is vial A Triangle
            //int vialIdx = lbDefaultNames.SelectedIndex;
            if (vialIdx < 0 || vialIdx > 2) vialIdx = 0;

            int quadrant = tabControl1.SelectedIndex;
            if (quadrant < 0 || quadrant > 3) quadrant = 0;

            return (barcode != "") ? barcode : (myUsedReagentVialList.Contains(GetLocation(quadrant, vialIdx)) ?
                                            SeparatorResourceManager.GetSeparatorString(StringId.ReagentCustom) :
                                            SeparatorResourceManager.GetSeparatorString(StringId.ReagentNotUsed));


        }
        private AbsoluteResourceLocation GetLocation(int quadrant, int vial)
        {
            //0 is vial B Square, Cocktail Vial, TPC0X03
            //1 is vial C Circle, Antibody Vial, TPC0X05
            //2 is vial A Triangle, Magnetic Particle Vial, TPC0X04

            AbsoluteResourceLocation[,] array = new AbsoluteResourceLocation[,]
            {
            {AbsoluteResourceLocation.TPC0103,AbsoluteResourceLocation.TPC0105,AbsoluteResourceLocation.TPC0104},
            {AbsoluteResourceLocation.TPC0203,AbsoluteResourceLocation.TPC0205,AbsoluteResourceLocation.TPC0204},
            {AbsoluteResourceLocation.TPC0303,AbsoluteResourceLocation.TPC0305,AbsoluteResourceLocation.TPC0304},
            {AbsoluteResourceLocation.TPC0403,AbsoluteResourceLocation.TPC0405,AbsoluteResourceLocation.TPC0404}
            };

            return array[quadrant,vial];
        }

        public void SetupBarcodeListBox()
        {
            lbCustomNames.Items.Clear();



            //general items
            lbCustomNames.Items.Add(GetBarcodeString(squareVialBarcode,0));
            lbCustomNames.Items.Add(GetBarcodeString(circleVialBarcode,1));
            lbCustomNames.Items.Add(GetBarcodeString(triangleVialBarcode,2));

            //			lbCustomNames.SelectedIndex = 0;
        }

        public void SetEnabledTotalQuad(int quads)
        {

            tabControl1.Controls.Clear();
            tabControl1.Controls.Add(tabQ1);
            if (quads > 1)
                tabControl1.Controls.Add(tabQ2);
            if (quads > 2)
                tabControl1.Controls.Add(tabQ3);
            if (quads > 3)
                tabControl1.Controls.Add(tabQ4);
        }

        private void lbDefaultNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTextbox();
        }

        private void UpdateTextbox()
        {
            StringId id = SeparatorResourceManager.convertNameToStringID((string)lbDefaultNames.SelectedItem);
            switch (id)
            {
                case StringId.VialA:
                    txtCustomName.Text = triangleVialBarcode;
                    break;
                case StringId.VialB:
                case StringId.VialBpos:
                case StringId.VialBneg:
                    txtCustomName.Text = squareVialBarcode;
                    break;
                case StringId.VialC:
                    txtCustomName.Text = circleVialBarcode;
                    break;
            }

            bool boEnabled = true;
            if (lbCustomNames.Items[lbDefaultNames.SelectedIndex].ToString() == SeparatorResourceManager.GetSeparatorString(StringId.ReagentNotUsed))
            {
                boEnabled = false; 
                txtCustomName.Text = SeparatorResourceManager.GetSeparatorString(StringId.ReagentNotUsed);
            }
            txtCustomName.Enabled = boEnabled;
            btnAcceptNewName.Enabled = boEnabled;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetupVialBarcodes(tabControl1.SelectedIndex);
            UpdateTextbox();
        }

        private void btnAcceptNewName_Click(object sender, EventArgs e)
        {

            string newName = txtCustomName.Text;
            StringId id = SeparatorResourceManager.convertNameToStringID((string)lbDefaultNames.SelectedItem);
            switch (id)
            {
                case StringId.VialB:
                case StringId.VialBpos:
                case StringId.VialBneg:
                    squareVialBarcode = vialBarcodes[1] = newName;
                    break;
                case StringId.VialA:
                    triangleVialBarcode = vialBarcodes[3] = newName;
                    break;
                case StringId.VialC:
                    circleVialBarcode = vialBarcodes[2] = newName;
                    break;
            }

            DbgView("btnAcceptNewName - calling ApplyVialBarcodes()");  //bdr

            theProtocolModel.ApplyVialBarcodes(tabControl1.SelectedIndex, vialBarcodes);
            this.SetupBarcodeListBox();
        }

        private void cbQuadrantRequired_CheckedChanged(object sender, EventArgs e)
        {
            bool boIsUsed = cbQuadrantRequired.Checked || squareVialBarcode != "" || circleVialBarcode != "" || triangleVialBarcode != "";
            vialBarcodes[0] = (boIsUsed)? (tabControl1.SelectedIndex + ""):"";
            theProtocolModel.ApplyVialBarcodes(tabControl1.SelectedIndex, vialBarcodes);

            /*
            if (boIsUsed && !cbQuadrantRequired.Checked)
            {
                cbQuadrantRequired.Checked = true;
            }
             */
        }

        private List<AbsoluteResourceLocation> myUsedReagentVialList;
        internal void SetUsedList(List<AbsoluteResourceLocation> UsedReagentVialList)
        {
            myUsedReagentVialList = UsedReagentVialList;
        }
    }
}
