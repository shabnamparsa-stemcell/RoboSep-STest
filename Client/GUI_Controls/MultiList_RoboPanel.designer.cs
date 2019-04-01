namespace GUI_Controls
{
    partial class MultiList_RoboPanel
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
            System.Windows.Forms.ListViewItem listViewItem10 = new System.Windows.Forms.ListViewItem(new string[] {
            "Item 1",
            "...",
            "..."}, -1);
            System.Windows.Forms.ListViewItem listViewItem11 = new System.Windows.Forms.ListViewItem(new string[] {
            "Item 2",
            "abc",
            "def"}, -1);
            System.Windows.Forms.ListViewItem listViewItem12 = new System.Windows.Forms.ListViewItem("Item 3");
            System.Windows.Forms.ListViewItem listViewItem13 = new System.Windows.Forms.ListViewItem("Item 4");
            System.Windows.Forms.ListViewItem listViewItem14 = new System.Windows.Forms.ListViewItem("a");
            System.Windows.Forms.ListViewItem listViewItem15 = new System.Windows.Forms.ListViewItem("b");
            System.Windows.Forms.ListViewItem listViewItem16 = new System.Windows.Forms.ListViewItem("c");
            System.Windows.Forms.ListViewItem listViewItem17 = new System.Windows.Forms.ListViewItem("d");
            System.Windows.Forms.ListViewItem listViewItem18 = new System.Windows.Forms.ListViewItem("e");
            this.listView_robostat = new System.Windows.Forms.ListView();
            this.Column1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Column2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Column3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.header_devide = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.Border_right = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.Border_left = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.Border_bottom = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.Border_top = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.Header1 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listView_robostat
            // 
            this.listView_robostat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.listView_robostat.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView_robostat.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Column1,
            this.Column2,
            this.Column3});
            this.listView_robostat.Font = new System.Drawing.Font("Arial Narrow", 13F, System.Drawing.FontStyle.Bold);
            this.listView_robostat.FullRowSelect = true;
            this.listView_robostat.GridLines = true;
            this.listView_robostat.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView_robostat.HideSelection = false;
            this.listView_robostat.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem10,
            listViewItem11,
            listViewItem12,
            listViewItem13,
            listViewItem14,
            listViewItem15,
            listViewItem16,
            listViewItem17,
            listViewItem18});
            this.listView_robostat.Location = new System.Drawing.Point(4, 38);
            this.listView_robostat.MultiSelect = false;
            this.listView_robostat.Name = "listView_robostat";
            this.listView_robostat.OwnerDraw = true;
            this.listView_robostat.Size = new System.Drawing.Size(518, 205);
            this.listView_robostat.TabIndex = 14;
            this.listView_robostat.TileSize = new System.Drawing.Size(1, 30);
            this.listView_robostat.UseCompatibleStateImageBehavior = false;
            this.listView_robostat.View = System.Windows.Forms.View.Details;
            this.listView_robostat.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.listView_robostat_DrawItem);
            // 
            // Column1
            // 
            this.Column1.Text = "Date / Time";
            this.Column1.Width = 133;
            // 
            // Column2
            // 
            this.Column2.Text = "Run Completion";
            this.Column2.Width = 133;
            // 
            // Column3
            // 
            this.Column3.Text = "Messages";
            this.Column3.Width = 235;
            // 
            // shapeContainer1
            // 
            this.shapeContainer1.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer1.Name = "shapeContainer1";
            this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.header_devide,
            this.Border_right,
            this.Border_left,
            this.Border_bottom,
            this.Border_top});
            this.shapeContainer1.Size = new System.Drawing.Size(525, 250);
            this.shapeContainer1.TabIndex = 15;
            this.shapeContainer1.TabStop = false;
            // 
            // header_devide
            // 
            this.header_devide.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.header_devide.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.header_devide.BorderColor = System.Drawing.Color.Transparent;
            this.header_devide.FillGradientColor = System.Drawing.Color.White;
            this.header_devide.Location = new System.Drawing.Point(10, 35);
            this.header_devide.Name = "header_devide";
            this.header_devide.Size = new System.Drawing.Size(500, 2);
            // 
            // Border_right
            // 
            this.Border_right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.Border_right.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.Border_right.BorderColor = System.Drawing.Color.Transparent;
            this.Border_right.FillGradientColor = System.Drawing.Color.White;
            this.Border_right.Location = new System.Drawing.Point(517, 6);
            this.Border_right.Name = "Border_right";
            this.Border_right.Size = new System.Drawing.Size(4, 25);
            // 
            // Border_left
            // 
            this.Border_left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.Border_left.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.Border_left.BorderColor = System.Drawing.Color.Transparent;
            this.Border_left.FillGradientColor = System.Drawing.Color.White;
            this.Border_left.Location = new System.Drawing.Point(0, 4);
            this.Border_left.Name = "Border_left";
            this.Border_left.Size = new System.Drawing.Size(4, 25);
            // 
            // Border_bottom
            // 
            this.Border_bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.Border_bottom.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.Border_bottom.BorderColor = System.Drawing.Color.Transparent;
            this.Border_bottom.FillGradientColor = System.Drawing.Color.White;
            this.Border_bottom.Location = new System.Drawing.Point(7, 244);
            this.Border_bottom.Name = "Border_bottom";
            this.Border_bottom.Size = new System.Drawing.Size(25, 4);
            // 
            // Border_top
            // 
            this.Border_top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.Border_top.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.Border_top.BorderColor = System.Drawing.Color.Transparent;
            this.Border_top.BorderStyle = System.Drawing.Drawing2D.DashStyle.Custom;
            this.Border_top.FillGradientColor = System.Drawing.Color.White;
            this.Border_top.Location = new System.Drawing.Point(0, 0);
            this.Border_top.Name = "Border_top";
            this.Border_top.Size = new System.Drawing.Size(25, 4);
            // 
            // Header1
            // 
            this.Header1.AutoSize = true;
            this.Header1.Font = new System.Drawing.Font("Agency FB", 15F, System.Drawing.FontStyle.Bold);
            this.Header1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.Header1.Location = new System.Drawing.Point(13, 6);
            this.Header1.Name = "Header1";
            this.Header1.Size = new System.Drawing.Size(83, 24);
            this.Header1.TabIndex = 16;
            this.Header1.Text = "Date / Time";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Agency FB", 15F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.label1.Location = new System.Drawing.Point(148, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 24);
            this.label1.TabIndex = 17;
            this.label1.Text = "Run Completion";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Agency FB", 15F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.label2.Location = new System.Drawing.Point(284, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 24);
            this.label2.TabIndex = 18;
            this.label2.Text = "Messages";
            // 
            // MultiList_RoboPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Header1);
            this.Controls.Add(this.listView_robostat);
            this.Controls.Add(this.shapeContainer1);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 3, 3);
            this.Name = "MultiList_RoboPanel";
            this.Size = new System.Drawing.Size(525, 250);
            this.Load += new System.EventHandler(this.MultiList_RoboPanel_Load);
            this.Resize += new System.EventHandler(this.MultiList_RoboPanel_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView_robostat;
        private System.Windows.Forms.ColumnHeader Column1;
        private System.Windows.Forms.ColumnHeader Column2;
        private System.Windows.Forms.ColumnHeader Column3;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape Border_top;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape Border_right;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape Border_left;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape Border_bottom;
        private System.Windows.Forms.Label Header1;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape header_devide;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}
