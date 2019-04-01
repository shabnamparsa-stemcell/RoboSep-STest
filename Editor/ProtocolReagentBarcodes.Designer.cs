using Tesla.ProtocolEditorControls;
namespace Tesla.ProtocolEditor
{
    partial class ProtocolReagentBarcodes
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProtocolReagentBarcodes));
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbQuadrantRequired = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblDefaultNames = new System.Windows.Forms.Label();
            this.lbCustomNames = new System.Windows.Forms.ListBox();
            this.lbDefaultNames = new System.Windows.Forms.ListBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabQ1 = new System.Windows.Forms.TabPage();
            this.tabQ2 = new System.Windows.Forms.TabPage();
            this.tabQ3 = new System.Windows.Forms.TabPage();
            this.tabQ4 = new System.Windows.Forms.TabPage();
            this.lblNewName = new System.Windows.Forms.Label();
            this.btnAcceptNewName = new System.Windows.Forms.Button();
            this.txtCustomName = new Tesla.ProtocolEditorControls.ProtocolTextbox();
            this.tipDefaultName = new System.Windows.Forms.ToolTip(this.components);
            this.tipNewName = new System.Windows.Forms.ToolTip(this.components);
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.cbQuadrantRequired);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.lblDefaultNames);
            this.panel1.Controls.Add(this.lbCustomNames);
            this.panel1.Controls.Add(this.lbDefaultNames);
            this.panel1.Location = new System.Drawing.Point(13, 32);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(352, 152);
            this.panel1.TabIndex = 13;
            // 
            // cbQuadrantRequired
            // 
            this.cbQuadrantRequired.AutoSize = true;
            this.cbQuadrantRequired.Location = new System.Drawing.Point(8, 126);
            this.cbQuadrantRequired.Name = "cbQuadrantRequired";
            this.cbQuadrantRequired.Size = new System.Drawing.Size(81, 17);
            this.cbQuadrantRequired.TabIndex = 9;
            this.cbQuadrantRequired.Text = "Custom Vial";
            this.cbQuadrantRequired.UseVisualStyleBackColor = true;
            this.cbQuadrantRequired.Visible = false;
            this.cbQuadrantRequired.CheckedChanged += new System.EventHandler(this.cbQuadrantRequired_CheckedChanged);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(184, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 16);
            this.label1.TabIndex = 8;
            this.label1.Text = "Barcodes:";
            // 
            // lblDefaultNames
            // 
            this.lblDefaultNames.BackColor = System.Drawing.Color.Transparent;
            this.lblDefaultNames.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDefaultNames.Location = new System.Drawing.Point(8, 8);
            this.lblDefaultNames.Name = "lblDefaultNames";
            this.lblDefaultNames.Size = new System.Drawing.Size(112, 16);
            this.lblDefaultNames.TabIndex = 7;
            this.lblDefaultNames.Text = "Vial Names:";
            // 
            // lbCustomNames
            // 
            this.lbCustomNames.Enabled = false;
            this.lbCustomNames.Location = new System.Drawing.Point(184, 24);
            this.lbCustomNames.Name = "lbCustomNames";
            this.lbCustomNames.Size = new System.Drawing.Size(160, 95);
            this.lbCustomNames.TabIndex = 6;
            // 
            // lbDefaultNames
            // 
            this.lbDefaultNames.Location = new System.Drawing.Point(8, 24);
            this.lbDefaultNames.Name = "lbDefaultNames";
            this.lbDefaultNames.Size = new System.Drawing.Size(160, 95);
            this.lbDefaultNames.TabIndex = 1;
            this.lbDefaultNames.SelectedIndexChanged += new System.EventHandler(this.lbDefaultNames_SelectedIndexChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabQ1);
            this.tabControl1.Controls.Add(this.tabQ2);
            this.tabControl1.Controls.Add(this.tabQ3);
            this.tabControl1.Controls.Add(this.tabQ4);
            this.tabControl1.ItemSize = new System.Drawing.Size(42, 18);
            this.tabControl1.Location = new System.Drawing.Point(13, 8);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(352, 24);
            this.tabControl1.TabIndex = 12;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabQ1
            // 
            this.tabQ1.Location = new System.Drawing.Point(4, 22);
            this.tabQ1.Name = "tabQ1";
            this.tabQ1.Size = new System.Drawing.Size(344, 0);
            this.tabQ1.TabIndex = 0;
            this.tabQ1.Text = "Q1";
            // 
            // tabQ2
            // 
            this.tabQ2.Location = new System.Drawing.Point(4, 22);
            this.tabQ2.Name = "tabQ2";
            this.tabQ2.Size = new System.Drawing.Size(344, 0);
            this.tabQ2.TabIndex = 1;
            this.tabQ2.Text = "Q2";
            // 
            // tabQ3
            // 
            this.tabQ3.Location = new System.Drawing.Point(4, 22);
            this.tabQ3.Name = "tabQ3";
            this.tabQ3.Size = new System.Drawing.Size(344, 0);
            this.tabQ3.TabIndex = 2;
            this.tabQ3.Text = "Q3";
            // 
            // tabQ4
            // 
            this.tabQ4.Location = new System.Drawing.Point(4, 22);
            this.tabQ4.Name = "tabQ4";
            this.tabQ4.Size = new System.Drawing.Size(344, 0);
            this.tabQ4.TabIndex = 3;
            this.tabQ4.Text = "Q4";
            // 
            // lblNewName
            // 
            this.lblNewName.BackColor = System.Drawing.Color.Transparent;
            this.lblNewName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewName.Location = new System.Drawing.Point(13, 190);
            this.lblNewName.Name = "lblNewName";
            this.lblNewName.Size = new System.Drawing.Size(192, 21);
            this.lblNewName.TabIndex = 11;
            this.lblNewName.Text = "Change Reagent Barcode";
            // 
            // btnAcceptNewName
            // 
            this.btnAcceptNewName.Location = new System.Drawing.Point(213, 204);
            this.btnAcceptNewName.Name = "btnAcceptNewName";
            this.btnAcceptNewName.Size = new System.Drawing.Size(132, 23);
            this.btnAcceptNewName.TabIndex = 10;
            this.btnAcceptNewName.Text = "Accept New Barcode";
            this.btnAcceptNewName.Click += new System.EventHandler(this.btnAcceptNewName_Click);
            // 
            // txtCustomName
            // 
            this.txtCustomName.Location = new System.Drawing.Point(13, 208);
            this.txtCustomName.Name = "txtCustomName";
            this.txtCustomName.Size = new System.Drawing.Size(160, 20);
            this.txtCustomName.TabIndex = 9;
            // 
            // ProtocolReagentBarcodes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 241);
            this.Controls.Add(this.txtCustomName);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.lblNewName);
            this.Controls.Add(this.btnAcceptNewName);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProtocolReagentBarcodes";
            this.Text = "Reagent Barcodes";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblDefaultNames;
        private System.Windows.Forms.ListBox lbCustomNames;
        private System.Windows.Forms.ListBox lbDefaultNames;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabQ1;
        private System.Windows.Forms.TabPage tabQ2;
        private System.Windows.Forms.TabPage tabQ3;
        private System.Windows.Forms.TabPage tabQ4;
        private System.Windows.Forms.Label lblNewName;
        private System.Windows.Forms.Button btnAcceptNewName;
        private ProtocolTextbox txtCustomName;
        private System.Windows.Forms.ToolTip tipDefaultName;
        private System.Windows.Forms.ToolTip tipNewName;
        private System.Windows.Forms.CheckBox cbQuadrantRequired;
    }
}