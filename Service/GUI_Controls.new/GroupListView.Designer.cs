namespace GUI_Controls
{
    partial class GroupListView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GroupListView));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.barcodeListView = new GUI_Controls.BarcodeListView();
            this.button_Group = new GUI_Controls.GUIButton();
            this.button_Scan = new GUI_Controls.GUIButton();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.Controls.Add(this.button_Group);
            this.panel1.Controls.Add(this.button_Scan);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(553, 40);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Arial Narrow", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.DarkGray;
            this.label1.Location = new System.Drawing.Point(49, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(218, 31);
            this.label1.TabIndex = 0;
            this.label1.Text = "Quadrant";
            // 
            // barcodeListView
            // 
            this.barcodeListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.barcodeListView.GridLines = true;
            this.barcodeListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.barcodeListView.LabelEdit = true;
            this.barcodeListView.Location = new System.Drawing.Point(0, 40);
            this.barcodeListView.Margin = new System.Windows.Forms.Padding(0);
            this.barcodeListView.MaximumSize = new System.Drawing.Size(560, 300);
            this.barcodeListView.Name = "barcodeListView";
            this.barcodeListView.RowHeight = 40;
            this.barcodeListView.Size = new System.Drawing.Size(553, 167);
            this.barcodeListView.TabIndex = 2;
            this.barcodeListView.UseCompatibleStateImageBehavior = false;
            this.barcodeListView.View = System.Windows.Forms.View.Details;
            this.barcodeListView.VisibleRow = 4;
            // 
            // button_Group
            // 
            this.button_Group.BackColor = System.Drawing.Color.Transparent;
            this.button_Group.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Group.BackgroundImage")));
            this.button_Group.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Group.Check = false;
            this.button_Group.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Group.ForeColor = System.Drawing.Color.White;
            this.button_Group.Location = new System.Drawing.Point(4, 1);
            this.button_Group.Name = "button_Group";
            this.button_Group.Size = new System.Drawing.Size(40, 40);
            this.button_Group.TabIndex = 3;
            this.button_Group.Text = "  ";
            this.button_Group.Click += new System.EventHandler(this.button_Group_Click);
            // 
            // button_Scan
            // 
            this.button_Scan.BackColor = System.Drawing.Color.Transparent;
            this.button_Scan.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Scan.BackgroundImage")));
            this.button_Scan.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Scan.Check = false;
            this.button_Scan.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Scan.ForeColor = System.Drawing.Color.White;
            this.button_Scan.Location = new System.Drawing.Point(511, -2);
            this.button_Scan.Name = "button_Scan";
            this.button_Scan.Size = new System.Drawing.Size(40, 40);
            this.button_Scan.TabIndex = 2;
            this.button_Scan.Text = "  ";
            // 
            // GroupListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.barcodeListView);
            this.Controls.Add(this.panel1);
            this.Name = "GroupListView";
            this.Size = new System.Drawing.Size(553, 208);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private BarcodeListView barcodeListView;
        private GUIButton button_Scan;
        private GUIButton button_Group;
    }
}
