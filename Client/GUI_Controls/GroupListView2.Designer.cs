namespace GUI_Controls
{
    partial class GroupListView2
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
            int i;
            for (i = 0; i < ilistGroupEmpty.Count; i++)
            {
                ilistGroupEmpty[i].Dispose();
            }
            for (i = 0; i < ilistGroupExpanded.Count; i++)
            {
                ilistGroupExpanded[i].Dispose();
            }
            for (i = 0; i < ilistGroupCollapsed.Count; i++)
            {
                ilistGroupCollapsed[i].Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GroupListView2));
            this.panel1 = new System.Windows.Forms.Panel();
            this.button_Group = new GUI_Controls.GUIButton();
            this.label1 = new System.Windows.Forms.Label();
            this.configListView = new GUI_Controls.ConfigListView();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.Controls.Add(this.button_Group);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(0, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(553, 40);
            this.panel1.TabIndex = 3;
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
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Arial Narrow", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.DarkGray;
            this.label1.Location = new System.Drawing.Point(49, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(218, 31);
            this.label1.TabIndex = 0;
            this.label1.Text = "Section";
            // 
            // configListView
            // 
            this.configListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.configListView.GridLines = true;
            this.configListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.configListView.Location = new System.Drawing.Point(0, 42);
            this.configListView.Margin = new System.Windows.Forms.Padding(0);
            this.configListView.MaximumSize = new System.Drawing.Size(560, 300);
            this.configListView.MultiSelect = false;
            this.configListView.Name = "configListView";
            this.configListView.RowHeight = 40;
            this.configListView.Scrollable = false;
            this.configListView.ShowGroups = false;
            this.configListView.Size = new System.Drawing.Size(553, 205);
            this.configListView.TabIndex = 5;
            this.configListView.UseCompatibleStateImageBehavior = false;
            this.configListView.View = System.Windows.Forms.View.Details;
            this.configListView.VisibleRow = 5;
            this.configListView.VScrollbar = null;
            // 
            // GroupListView2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.configListView);
            this.Controls.Add(this.panel1);
            this.Name = "GroupListView2";
            this.Size = new System.Drawing.Size(553, 208);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private GUIButton button_Group;
        private System.Windows.Forms.Label label1;
        private ConfigListView configListView;
    }
}
