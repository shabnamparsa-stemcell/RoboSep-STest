using Tesla.ProtocolEditorControls;
namespace Tesla.ProtocolEditor
{
    partial class ProtocolFeatureSwitch
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnAcceptNewName = new System.Windows.Forms.Button();
            this.lbDefaultNames = new System.Windows.Forms.CheckedListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtFeatureDescription = new System.Windows.Forms.TextBox();
            this.gbArguments = new System.Windows.Forms.GroupBox();
            this.lstArgumentTypeAndData = new System.Windows.Forms.ListBox();
            this.txtData = new Tesla.ProtocolEditorControls.ProtocolTextbox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.gbArguments.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAcceptNewName
            // 
            this.btnAcceptNewName.Location = new System.Drawing.Point(208, 101);
            this.btnAcceptNewName.Name = "btnAcceptNewName";
            this.btnAcceptNewName.Size = new System.Drawing.Size(134, 23);
            this.btnAcceptNewName.TabIndex = 13;
            this.btnAcceptNewName.Text = "Accept";
            this.btnAcceptNewName.Click += new System.EventHandler(this.btnAcceptNewName_Click);
            // 
            // lbDefaultNames
            // 
            this.lbDefaultNames.Location = new System.Drawing.Point(6, 19);
            this.lbDefaultNames.Name = "lbDefaultNames";
            this.lbDefaultNames.Size = new System.Drawing.Size(148, 124);
            this.lbDefaultNames.TabIndex = 15;
            this.lbDefaultNames.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lbDefaultNames_ItemCheck);
            this.lbDefaultNames.SelectedIndexChanged += new System.EventHandler(this.lbDefaultNames_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtFeatureDescription);
            this.groupBox1.Location = new System.Drawing.Point(178, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(182, 157);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Selected Feature Description";
            // 
            // txtFeatureDescription
            // 
            this.txtFeatureDescription.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.txtFeatureDescription.ForeColor = System.Drawing.SystemColors.ControlText;
            this.txtFeatureDescription.Location = new System.Drawing.Point(6, 19);
            this.txtFeatureDescription.Multiline = true;
            this.txtFeatureDescription.Name = "txtFeatureDescription";
            this.txtFeatureDescription.ReadOnly = true;
            this.txtFeatureDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtFeatureDescription.Size = new System.Drawing.Size(170, 124);
            this.txtFeatureDescription.TabIndex = 1;
            // 
            // gbArguments
            // 
            this.gbArguments.Controls.Add(this.lstArgumentTypeAndData);
            this.gbArguments.Controls.Add(this.txtData);
            this.gbArguments.Controls.Add(this.btnAcceptNewName);
            this.gbArguments.Location = new System.Drawing.Point(12, 175);
            this.gbArguments.Name = "gbArguments";
            this.gbArguments.Size = new System.Drawing.Size(348, 130);
            this.gbArguments.TabIndex = 18;
            this.gbArguments.TabStop = false;
            this.gbArguments.Text = "Selected Feature Arguments";
            // 
            // lstArgumentTypeAndData
            // 
            this.lstArgumentTypeAndData.FormattingEnabled = true;
            this.lstArgumentTypeAndData.Location = new System.Drawing.Point(6, 19);
            this.lstArgumentTypeAndData.Name = "lstArgumentTypeAndData";
            this.lstArgumentTypeAndData.Size = new System.Drawing.Size(336, 69);
            this.lstArgumentTypeAndData.TabIndex = 15;
            this.lstArgumentTypeAndData.SelectedIndexChanged += new System.EventHandler(this.lstArgumentTypeAndData_SelectedIndexChanged);
            // 
            // txtData
            // 
            this.txtData.Location = new System.Drawing.Point(7, 104);
            this.txtData.Name = "txtData";
            this.txtData.Size = new System.Drawing.Size(195, 20);
            this.txtData.TabIndex = 14;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lbDefaultNames);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(160, 157);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "List of Features";
            // 
            // ProtocolFeatureSwitch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(376, 315);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.gbArguments);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProtocolFeatureSwitch";
            this.Text = "ProtocolFeatureSwitch";
            this.Load += new System.EventHandler(this.ProtocolFeatureSwitch_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gbArguments.ResumeLayout(false);
            this.gbArguments.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnAcceptNewName;
        private System.Windows.Forms.CheckedListBox lbDefaultNames;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox gbArguments;
        private System.Windows.Forms.ListBox lstArgumentTypeAndData;
        private ProtocolTextbox txtData;
        private System.Windows.Forms.TextBox txtFeatureDescription;
        private System.Windows.Forms.GroupBox groupBox3;
    }
}