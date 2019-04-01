using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Tesla.ProtocolEditorModel;
using System.Runtime.InteropServices;

namespace Tesla.ProtocolEditor
{
    public partial class ProtocolFeatureSwitch : Form
    {
        private static ProtocolModel theProtocolModel;

        public const string INPUT_DATA_TYPE_STRING = "[string]";
        public const string INPUT_DATA_TYPE_INT = "[int]";
        public const string INPUT_DATA_TYPE_DOUBLE = "[double]";
        public const string INPUT_DATA_TYPE_BOOL = "[bool]";
        public const string INPUT_DATA_TYPE_BOOL_TRUE = "true";
        public const string INPUT_DATA_TYPE_BOOL_FALSE = "false";

        private string definedFeatureSectionName;
        private List<Feature> definedFeatures;
        public ProtocolFeatureSwitch()
        {
            InitializeComponent();

            // Get a local reference to the protocol model. 
            theProtocolModel = ProtocolModel.GetInstance();
        }

        public string SetDefinedFeatureSectionName
        {
            set
            {
                definedFeatureSectionName = value;
            }
        }

        public List<string> SetDefinedFeatures
        {
            set
            {
                definedFeatures = new List<Feature>();
                foreach (string feat in value)
                {
                    string feat_value = Tesla.Common.Utilities.GetSoftwareConfigString(definedFeatureSectionName, feat, "");
                    Feature f = new Feature(feat, feat_value);

                    definedFeatures.Add(f);
                    lbDefaultNames.Items.Add(f.Name);
                }
            }
        }

        public void updateFeaturesFromProtocol(DataAccess.feature[] protocolFeature)
        {
            if (protocolFeature == null) return;
            foreach (DataAccess.feature f in protocolFeature)
            {
                bool isNew = true;
                foreach (Feature df in definedFeatures)
                {
                    if (f.name == df.Name)
                    {
                        df.updateFeature(f);
                        df.Checked = true;
                        isNew = false;

                        lbDefaultNames.SetItemChecked(definedFeatures.IndexOf(df),true);

                        break;
                    }
                }
                if (isNew)
                {
                    Feature tmp = new Feature(f);
                    definedFeatures.Add(tmp);
                }
            }
        }

        private void ProtocolFeatureSwitch_Load(object sender, EventArgs e)
        {
            //load features
            txtFeatureDescription.Enabled = false; //reid of stupid blinking cursor
        }

        private void lbDefaultNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            Feature f = definedFeatures[lbDefaultNames.SelectedIndex];
            txtFeatureDescription.Text = f.Description;
            lstArgumentTypeAndData.Items.Clear();
            lstArgumentTypeAndData.Items.AddRange(f.ArgumentTypeAndDataArray());

            gbArguments.Enabled = f.Checked;
            EnableTxtDataAndBtnAccept(lstArgumentTypeAndData.SelectedIndex != -1);
            txtData.Text = "";
            txtFeatureDescription.Enabled = true;
        }

        private void lbDefaultNames_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            //save state..... 
            definedFeatures[e.Index].Checked = e.NewValue == CheckState.Checked;

            gbArguments.Enabled = definedFeatures[e.Index].Checked;
            EnableTxtDataAndBtnAccept(lstArgumentTypeAndData.SelectedIndex != -1);
            txtData.Text = "";

            if (!definedFeatures[e.Index].Checked)
            {
                definedFeatures[e.Index].ClearData();
            }


            theProtocolModel.setFeatureList(definedFeatures);
        }

        private void btnAcceptNewName_Click(object sender, EventArgs e)
        {
            if (lbDefaultNames.SelectedIndex==-1 || lstArgumentTypeAndData.SelectedIndex == -1) return;

            //ASSUMED ArgumentData[listBox1.SelectedIndex] exists!!!

            //check arg
            string currentType = "[" + definedFeatures[lbDefaultNames.SelectedIndex].ArgumentTypes[lstArgumentTypeAndData.SelectedIndex] + "]";
            bool goSave = false;
            if (currentType == INPUT_DATA_TYPE_DOUBLE)
            {
                double d = 0;
                if (double.TryParse(txtData.Text, out d))
                {
                    goSave = true;
                    definedFeatures[lbDefaultNames.SelectedIndex].ArgumentData[lstArgumentTypeAndData.SelectedIndex] = txtData.Text;
                }
                else
                {
                    MessageBox.Show(this, txtData.Text+" is not of type double.", 
                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			
                }
            }
            else if (currentType == INPUT_DATA_TYPE_INT)
            {
                int i = 0;
                if (int.TryParse(txtData.Text, out i))
                {
                    goSave = true;
                    definedFeatures[lbDefaultNames.SelectedIndex].ArgumentData[lstArgumentTypeAndData.SelectedIndex] = txtData.Text;
                }
                else
                {
                    //message box error
                    MessageBox.Show(this, txtData.Text + " is not of type integer.",
                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else if (currentType == INPUT_DATA_TYPE_BOOL)
            {
                if (txtData.Text == INPUT_DATA_TYPE_BOOL_TRUE || txtData.Text == INPUT_DATA_TYPE_BOOL_FALSE)
                {
                    goSave = true;
                    definedFeatures[lbDefaultNames.SelectedIndex].ArgumentData[lstArgumentTypeAndData.SelectedIndex] = txtData.Text;
                }
                else
                {
                    //message box error

                    MessageBox.Show(this, "Argument has to be either \"true\" or \"false\".",
                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else if (currentType == INPUT_DATA_TYPE_STRING)
            {
                if (txtData.Text.IndexOf(';') == -1)
                {
                    goSave = true;
                    definedFeatures[lbDefaultNames.SelectedIndex].ArgumentData[lstArgumentTypeAndData.SelectedIndex] = txtData.Text;
                }
                else
                {
                    //message box error

                    MessageBox.Show(this, "Argument cannot contain semicolon.",
                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }


            if (goSave)
            {
                //save state....
                theProtocolModel.setFeatureList(definedFeatures);
                lbDefaultNames_SelectedIndexChanged(this, null);
            }
        }

        private void lstArgumentTypeAndData_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableTxtDataAndBtnAccept(lstArgumentTypeAndData.SelectedIndex != -1);
        }

        private void EnableTxtDataAndBtnAccept(bool on)
        {
            txtData.Enabled = on;
            btnAcceptNewName.Enabled = on;
        }
    }

    
}
