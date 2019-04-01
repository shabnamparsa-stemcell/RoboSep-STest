namespace GUI_Console
{
    partial class RoboSep_ProtocolSelect
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoboSep_ProtocolSelect));
            this.label_userName = new System.Windows.Forms.Label();
            this.listView_UserProtocols = new System.Windows.Forms.ListView();
            this.Column1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Column2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Column3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.Header3 = new System.Windows.Forms.Label();
            this.Header2 = new System.Windows.Forms.Label();
            this.Header1 = new System.Windows.Forms.Label();
            this.header_devide = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.SearchBar = new GUI_Controls.Protocol_SearchBar();
            this.button_Accept = new GUI_Controls.Button_Pill();
            this.button_SelectionMode = new GUI_Controls.Button_Rectangle();
            this.button_Cancel1 = new GUI_Controls.Button_Cancel();
            this.LoadProtocolTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label_userName
            // 
            this.label_userName.AutoSize = true;
            this.label_userName.BackColor = System.Drawing.Color.Transparent;
            this.label_userName.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_userName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.label_userName.Location = new System.Drawing.Point(423, 13);
            this.label_userName.Name = "label_userName";
            this.label_userName.Size = new System.Drawing.Size(41, 24);
            this.label_userName.TabIndex = 0;
            this.label_userName.Text = "label1";
            // 
            // listView_UserProtocols
            // 
            this.listView_UserProtocols.BackColor = System.Drawing.Color.White;
            this.listView_UserProtocols.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView_UserProtocols.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Column1,
            this.Column2,
            this.Column3});
            this.listView_UserProtocols.Font = new System.Drawing.Font("Arial", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listView_UserProtocols.FullRowSelect = true;
            this.listView_UserProtocols.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView_UserProtocols.HideSelection = false;
            this.listView_UserProtocols.LabelWrap = false;
            this.listView_UserProtocols.Location = new System.Drawing.Point(21, 142);
            this.listView_UserProtocols.MultiSelect = false;
            this.listView_UserProtocols.Name = "listView_UserProtocols";
            this.listView_UserProtocols.Size = new System.Drawing.Size(597, 275);
            this.listView_UserProtocols.SmallImageList = this.imageList1;
            this.listView_UserProtocols.TabIndex = 15;
            this.listView_UserProtocols.TileSize = new System.Drawing.Size(1, 45);
            this.listView_UserProtocols.UseCompatibleStateImageBehavior = false;
            this.listView_UserProtocols.View = System.Windows.Forms.View.Details;
            // 
            // Column1
            // 
            this.Column1.Text = "Protocol Name";
            this.Column1.Width = 462;
            // 
            // Column2
            // 
            this.Column2.Text = "Type";
            this.Column2.Width = 84;
            // 
            // Column3
            // 
            this.Column3.Text = "Quadrants";
            this.Column3.Width = 23;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(1, 35);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // Header3
            // 
            this.Header3.AutoSize = true;
            this.Header3.BackColor = System.Drawing.Color.Transparent;
            this.Header3.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 16F, System.Drawing.FontStyle.Bold);
            this.Header3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.Header3.Location = new System.Drawing.Point(543, 113);
            this.Header3.Name = "Header3";
            this.Header3.Size = new System.Drawing.Size(82, 26);
            this.Header3.TabIndex = 21;
            this.Header3.Text = "Quadrants";
            // 
            // Header2
            // 
            this.Header2.AutoSize = true;
            this.Header2.BackColor = System.Drawing.Color.White;
            this.Header2.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 16F, System.Drawing.FontStyle.Bold);
            this.Header2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.Header2.Location = new System.Drawing.Point(489, 113);
            this.Header2.Name = "Header2";
            this.Header2.Size = new System.Drawing.Size(48, 26);
            this.Header2.TabIndex = 20;
            this.Header2.Text = "Type";
            // 
            // Header1
            // 
            this.Header1.AutoSize = true;
            this.Header1.BackColor = System.Drawing.Color.White;
            this.Header1.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 16F, System.Drawing.FontStyle.Bold);
            this.Header1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.Header1.Location = new System.Drawing.Point(24, 113);
            this.Header1.Name = "Header1";
            this.Header1.Size = new System.Drawing.Size(113, 26);
            this.Header1.TabIndex = 19;
            this.Header1.Text = "Protocol Name";
            // 
            // header_devide
            // 
            this.header_devide.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.header_devide.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.header_devide.BorderColor = System.Drawing.Color.Transparent;
            this.header_devide.FillGradientColor = System.Drawing.Color.White;
            this.header_devide.Location = new System.Drawing.Point(24, 140);
            this.header_devide.Name = "header_devide";
            this.header_devide.Size = new System.Drawing.Size(592, 3);
            // 
            // shapeContainer1
            // 
            this.shapeContainer1.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer1.Name = "shapeContainer1";
            this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.header_devide});
            this.shapeContainer1.Size = new System.Drawing.Size(640, 480);
            this.shapeContainer1.TabIndex = 22;
            this.shapeContainer1.TabStop = false;
            // 
            // SearchBar
            // 
            this.SearchBar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("SearchBar.BackgroundImage")));
            this.SearchBar.Location = new System.Drawing.Point(276, 54);
            this.SearchBar.Name = "SearchBar";
            this.SearchBar.Size = new System.Drawing.Size(351, 51);
            this.SearchBar.TabIndex = 16;
            this.SearchBar.Visible = false;
            // 
            // button_Accept
            // 
            this.button_Accept.AccessibleName = "  ";
            this.button_Accept.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Accept.BackgroundImage")));
            this.button_Accept.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Accept.Check = false;
            this.button_Accept.Location = new System.Drawing.Point(480, 426);
            this.button_Accept.Name = "button_Accept";
            this.button_Accept.Size = new System.Drawing.Size(138, 41);
            this.button_Accept.TabIndex = 3;
            this.button_Accept.Click += new System.EventHandler(this.button_Accept_Click);
            // 
            // button_SelectionMode
            // 
            this.button_SelectionMode.AccessibleName = "  ";
            this.button_SelectionMode.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_SelectionMode.BackgroundImage")));
            this.button_SelectionMode.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_SelectionMode.Check = false;
            this.button_SelectionMode.Location = new System.Drawing.Point(18, 60);
            this.button_SelectionMode.Name = "button_SelectionMode";
            this.button_SelectionMode.Size = new System.Drawing.Size(251, 36);
            this.button_SelectionMode.TabIndex = 1;
            this.button_SelectionMode.Click += new System.EventHandler(this.button_SelectionMode_Click);
            // 
            // button_Cancel1
            // 
            this.button_Cancel1.AccessibleName = "  ";
            this.button_Cancel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Cancel1.BackgroundImage")));
            this.button_Cancel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Cancel1.Check = false;
            this.button_Cancel1.Location = new System.Drawing.Point(600, 8);
            this.button_Cancel1.Name = "button_Cancel1";
            this.button_Cancel1.Size = new System.Drawing.Size(33, 33);
            this.button_Cancel1.TabIndex = 23;
            this.button_Cancel1.Click += new System.EventHandler(this.button_Cancel1_Click);
            // 
            // LoadProtocolTimer
            // 
            this.LoadProtocolTimer.Interval = 50;
            this.LoadProtocolTimer.Tick += new System.EventHandler(this.LoadProtocolTimer_Tick);
            // 
            // RoboSep_ProtocolSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::GUI_Console.Properties.Resources.BG_ProtocolSelection;
            this.Controls.Add(this.button_Cancel1);
            this.Controls.Add(this.SearchBar);
            this.Controls.Add(this.listView_UserProtocols);
            this.Controls.Add(this.button_Accept);
            this.Controls.Add(this.button_SelectionMode);
            this.Controls.Add(this.label_userName);
            this.Controls.Add(this.shapeContainer1);
            this.Controls.Add(this.Header3);
            this.Controls.Add(this.Header2);
            this.Controls.Add(this.Header1);
            this.Name = "RoboSep_ProtocolSelect";
            this.Size = new System.Drawing.Size(640, 480);
            this.Load += new System.EventHandler(this.RoboSep_ProtocolSelect_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_userName;
        private GUI_Controls.Button_Rectangle button_SelectionMode;
        private GUI_Controls.Button_Pill button_Accept;
        private System.Windows.Forms.ListView listView_UserProtocols;
        private System.Windows.Forms.ColumnHeader Column1;
        private System.Windows.Forms.ColumnHeader Column2;
        private System.Windows.Forms.ColumnHeader Column3;
        private GUI_Controls.Protocol_SearchBar SearchBar;
        private System.Windows.Forms.Label Header3;
        private System.Windows.Forms.Label Header2;
        private System.Windows.Forms.Label Header1;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape header_devide;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private GUI_Controls.Button_Cancel button_Cancel1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Timer LoadProtocolTimer;
    }
}
