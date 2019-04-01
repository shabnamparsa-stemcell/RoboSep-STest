namespace GUI_Console
{
    partial class RoboSep_RunStatsLog
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoboSep_RunStatsLog));
            this.button_ViewReport = new GUI_Controls.Button_Rectangle();
            this.button_SaveReports = new GUI_Controls.Button_Rectangle();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.header_devide = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.listView_robostat = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.Border_right = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Header1 = new System.Windows.Forms.Label();
            this.roboPanel1 = new GUI_Controls.RoboPanel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_ViewReport
            // 
            this.button_ViewReport.AccessibleName = "View Report";
            this.button_ViewReport.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_ViewReport.BackgroundImage")));
            this.button_ViewReport.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_ViewReport.Check = false;
            this.button_ViewReport.Location = new System.Drawing.Point(119, 383);
            this.button_ViewReport.Name = "button_ViewReport";
            this.button_ViewReport.Size = new System.Drawing.Size(202, 74);
            this.button_ViewReport.TabIndex = 1;
            this.button_ViewReport.Load += new System.EventHandler(this.button_Rectangle1_Load);
            this.button_ViewReport.Click += new System.EventHandler(this.button_Rectangle1_Click);
            // 
            // button_SaveReports
            // 
            this.button_SaveReports.AccessibleName = "Save Reports to USB";
            this.button_SaveReports.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_SaveReports.BackgroundImage")));
            this.button_SaveReports.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_SaveReports.Check = false;
            this.button_SaveReports.Location = new System.Drawing.Point(363, 383);
            this.button_SaveReports.Name = "button_SaveReports";
            this.button_SaveReports.Size = new System.Drawing.Size(202, 74);
            this.button_SaveReports.TabIndex = 2;
            this.button_SaveReports.Click += new System.EventHandler(this.button_SaveReports_Click);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(1, 25);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // header_devide
            // 
            this.header_devide.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.header_devide.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.header_devide.BorderColor = System.Drawing.Color.Transparent;
            this.header_devide.FillGradientColor = System.Drawing.Color.White;
            this.header_devide.Location = new System.Drawing.Point(23, 82);
            this.header_devide.Name = "header_devide";
            this.header_devide.Size = new System.Drawing.Size(500, 2);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Location = new System.Drawing.Point(23, 57);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(540, 292);
            this.panel1.TabIndex = 24;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.Controls.Add(this.listView_robostat);
            this.panel2.Controls.Add(this.shapeContainer1);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.Header1);
            this.panel2.Location = new System.Drawing.Point(6, 6);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(527, 280);
            this.panel2.TabIndex = 28;
            // 
            // listView_robostat
            // 
            this.listView_robostat.AutoArrange = false;
            this.listView_robostat.BackColor = System.Drawing.SystemColors.Control;
            this.listView_robostat.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView_robostat.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listView_robostat.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.listView_robostat.FullRowSelect = true;
            this.listView_robostat.GridLines = true;
            this.listView_robostat.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView_robostat.HideSelection = false;
            this.listView_robostat.Location = new System.Drawing.Point(3, 33);
            this.listView_robostat.Name = "listView_robostat";
            this.listView_robostat.Size = new System.Drawing.Size(520, 244);
            this.listView_robostat.SmallImageList = this.imageList1;
            this.listView_robostat.TabIndex = 33;
            this.listView_robostat.UseCompatibleStateImageBehavior = false;
            this.listView_robostat.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 123;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Width = 100;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Width = 274;
            // 
            // shapeContainer1
            // 
            this.shapeContainer1.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer1.Name = "shapeContainer1";
            this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.Border_right});
            this.shapeContainer1.Size = new System.Drawing.Size(527, 280);
            this.shapeContainer1.TabIndex = 32;
            this.shapeContainer1.TabStop = false;
            // 
            // Border_right
            // 
            this.Border_right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.Border_right.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.Border_right.BorderColor = System.Drawing.Color.Transparent;
            this.Border_right.FillGradientColor = System.Drawing.Color.White;
            this.Border_right.Location = new System.Drawing.Point(5, 30);
            this.Border_right.Name = "Border_right";
            this.Border_right.Size = new System.Drawing.Size(516, 3);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 15F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.label2.Location = new System.Drawing.Point(229, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 24);
            this.label2.TabIndex = 31;
            this.label2.Text = "Run Info";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 15F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.label1.Location = new System.Drawing.Point(129, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 24);
            this.label1.TabIndex = 30;
            this.label1.Text = "User";
            // 
            // Header1
            // 
            this.Header1.AutoSize = true;
            this.Header1.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 15F, System.Drawing.FontStyle.Bold);
            this.Header1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.Header1.Location = new System.Drawing.Point(15, 6);
            this.Header1.Name = "Header1";
            this.Header1.Size = new System.Drawing.Size(83, 24);
            this.Header1.TabIndex = 29;
            this.Header1.Text = "Date / Time";
            // 
            // roboPanel1
            // 
            this.roboPanel1.AccessibleName = "Run Statistics Log";
            this.roboPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.roboPanel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.roboPanel1.Location = new System.Drawing.Point(14, 14);
            this.roboPanel1.Name = "roboPanel1";
            this.roboPanel1.Size = new System.Drawing.Size(558, 351);
            this.roboPanel1.TabIndex = 25;
            // 
            // RoboSep_RunStatsLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button_ViewReport);
            this.Controls.Add(this.button_SaveReports);
            this.Controls.Add(this.roboPanel1);
            this.Name = "RoboSep_RunStatsLog";
            this.Size = new System.Drawing.Size(573, 480);
            this.Controls.SetChildIndex(this.roboPanel1, 0);
            this.Controls.SetChildIndex(this.button_SaveReports, 0);
            this.Controls.SetChildIndex(this.button_ViewReport, 0);
            this.Controls.SetChildIndex(this.panel1, 0);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private GUI_Controls.Button_Rectangle button_ViewReport;
        private GUI_Controls.Button_Rectangle button_SaveReports;
        private System.Windows.Forms.ImageList imageList1;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape header_devide;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label Header1;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape Border_right;
        private GUI_Controls.RoboPanel roboPanel1;
        private System.Windows.Forms.ListView listView_robostat;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
    }
}
