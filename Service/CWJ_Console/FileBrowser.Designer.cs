namespace GUI_Console
{
    partial class FileBrowser
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileBrowser));
            this.labelTitle = new System.Windows.Forms.Label();
            this.labelPath = new System.Windows.Forms.Label();
            this.labelFilterFor = new System.Windows.Forms.Label();
            this.textBox_fileExtension = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button_OK = new GUI_Controls.Button_Rectangle();
            this.button_Cancel = new GUI_Controls.Button_Rectangle();
            this.button_Back = new GUI_Controls.Button_Pill();
            this.button_Refresh = new GUI_Controls.Button_Pill();
            this.button_Up = new GUI_Controls.Button_Pill();
            this.roboPanelContent = new GUI_Controls.RoboPanel();
            this.listView_Browser = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.roboPanel_REGION = new GUI_Controls.RoboPanel();
            this.panel1.SuspendLayout();
            this.roboPanelContent.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.labelTitle.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 16F, System.Drawing.FontStyle.Bold);
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Location = new System.Drawing.Point(12, 5);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(104, 26);
            this.labelTitle.TabIndex = 5;
            this.labelTitle.Text = "File Browser";
            // 
            // labelPath
            // 
            this.labelPath.AutoSize = true;
            this.labelPath.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.labelPath.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 14F);
            this.labelPath.ForeColor = System.Drawing.Color.White;
            this.labelPath.Location = new System.Drawing.Point(117, 6);
            this.labelPath.Name = "labelPath";
            this.labelPath.Size = new System.Drawing.Size(98, 24);
            this.labelPath.TabIndex = 6;
            this.labelPath.Text = "E:\\Protocols\\..";
            // 
            // labelFilterFor
            // 
            this.labelFilterFor.AutoSize = true;
            this.labelFilterFor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.labelFilterFor.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFilterFor.ForeColor = System.Drawing.Color.White;
            this.labelFilterFor.Location = new System.Drawing.Point(12, 385);
            this.labelFilterFor.Name = "labelFilterFor";
            this.labelFilterFor.Size = new System.Drawing.Size(71, 25);
            this.labelFilterFor.TabIndex = 7;
            this.labelFilterFor.Text = "Filter For:";
            // 
            // textBox_fileExtension
            // 
            this.textBox_fileExtension.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_fileExtension.Font = new System.Drawing.Font("Arial Narrow", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_fileExtension.Location = new System.Drawing.Point(1, 1);
            this.textBox_fileExtension.Name = "textBox_fileExtension";
            this.textBox_fileExtension.Size = new System.Drawing.Size(88, 32);
            this.textBox_fileExtension.TabIndex = 8;
            this.textBox_fileExtension.Text = ".XML";
            this.textBox_fileExtension.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_fileExtension.TextChanged += new System.EventHandler(this.textBox_fileExtension_TextChanged);
            this.textBox_fileExtension.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_fileExtension_KeyDown);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Controls.Add(this.textBox_fileExtension);
            this.panel1.Location = new System.Drawing.Point(83, 381);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(90, 34);
            this.panel1.TabIndex = 9;
            // 
            // button_OK
            // 
            this.button_OK.AccessibleName = "OK";
            this.button_OK.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_OK.BackgroundImage")));
            this.button_OK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_OK.Check = false;
            this.button_OK.Location = new System.Drawing.Point(371, 380);
            this.button_OK.Name = "button_OK";
            this.button_OK.Size = new System.Drawing.Size(91, 34);
            this.button_OK.TabIndex = 4;
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // button_Cancel
            // 
            this.button_Cancel.AccessibleName = "Cancel";
            this.button_Cancel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Cancel.BackgroundImage")));
            this.button_Cancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Cancel.Check = false;
            this.button_Cancel.Location = new System.Drawing.Point(261, 380);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(91, 34);
            this.button_Cancel.TabIndex = 3;
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // button_Back
            // 
            this.button_Back.AccessibleName = "  ";
            this.button_Back.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Back.BackgroundImage")));
            this.button_Back.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Back.Check = false;
            this.button_Back.Location = new System.Drawing.Point(317, 2);
            this.button_Back.Name = "button_Back";
            this.button_Back.Size = new System.Drawing.Size(39, 34);
            this.button_Back.TabIndex = 0;
            this.button_Back.Click += new System.EventHandler(this.button_Back_Click);
            // 
            // button_Refresh
            // 
            this.button_Refresh.AccessibleName = "  ";
            this.button_Refresh.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Refresh.BackgroundImage")));
            this.button_Refresh.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Refresh.Check = false;
            this.button_Refresh.Location = new System.Drawing.Point(424, 2);
            this.button_Refresh.Name = "button_Refresh";
            this.button_Refresh.Size = new System.Drawing.Size(39, 34);
            this.button_Refresh.TabIndex = 2;
            this.button_Refresh.Click += new System.EventHandler(this.button_Refresh_Click);
            // 
            // button_Up
            // 
            this.button_Up.AccessibleName = "  ";
            this.button_Up.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Up.BackgroundImage")));
            this.button_Up.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Up.Check = false;
            this.button_Up.Location = new System.Drawing.Point(371, 2);
            this.button_Up.Name = "button_Up";
            this.button_Up.Size = new System.Drawing.Size(39, 34);
            this.button_Up.TabIndex = 1;
            this.button_Up.Click += new System.EventHandler(this.button_Up_Click);
            // 
            // roboPanelContent
            // 
            this.roboPanelContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.roboPanelContent.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.roboPanelContent.Controls.Add(this.listView_Browser);
            this.roboPanelContent.Location = new System.Drawing.Point(0, 30);
            this.roboPanelContent.Name = "roboPanelContent";
            this.roboPanelContent.Size = new System.Drawing.Size(480, 354);
            this.roboPanelContent.TabIndex = 10;
            // 
            // listView_Browser
            // 
            this.listView_Browser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.listView_Browser.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView_Browser.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listView_Browser.FullRowSelect = true;
            this.listView_Browser.GridLines = true;
            this.listView_Browser.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView_Browser.Location = new System.Drawing.Point(12, 12);
            this.listView_Browser.Name = "listView_Browser";
            this.listView_Browser.Size = new System.Drawing.Size(455, 332);
            this.listView_Browser.TabIndex = 11;
            this.listView_Browser.TileSize = new System.Drawing.Size(435, 60);
            this.listView_Browser.UseCompatibleStateImageBehavior = false;
            this.listView_Browser.View = System.Windows.Forms.View.Details;
            this.listView_Browser.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView_Browser_MouseDoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Files";
            this.columnHeader1.Width = 435;
            // 
            // roboPanel_REGION
            // 
            this.roboPanel_REGION.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.roboPanel_REGION.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.roboPanel_REGION.Location = new System.Drawing.Point(0, 0);
            this.roboPanel_REGION.Name = "roboPanel_REGION";
            this.roboPanel_REGION.Size = new System.Drawing.Size(480, 417);
            this.roboPanel_REGION.TabIndex = 11;
            this.roboPanel_REGION.Visible = false;
            // 
            // FileBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.ClientSize = new System.Drawing.Size(480, 420);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_Back);
            this.Controls.Add(this.button_Refresh);
            this.Controls.Add(this.button_Up);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.labelFilterFor);
            this.Controls.Add(this.labelPath);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.roboPanelContent);
            this.Controls.Add(this.roboPanel_REGION);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Location = new System.Drawing.Point(20, 30);
            this.Name = "FileBrowser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Load += new System.EventHandler(this.FileBrowser_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.roboPanelContent.ResumeLayout(false);
            this.roboPanelContent.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GUI_Controls.Button_Pill button_Back;
        private GUI_Controls.Button_Pill button_Up;
        private GUI_Controls.Button_Pill button_Refresh;
        private GUI_Controls.Button_Rectangle button_Cancel;
        private GUI_Controls.Button_Rectangle button_OK;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelPath;
        private System.Windows.Forms.Label labelFilterFor;
        private System.Windows.Forms.TextBox textBox_fileExtension;
        private System.Windows.Forms.Panel panel1;
        private GUI_Controls.RoboPanel roboPanelContent;
        private GUI_Controls.RoboPanel roboPanel_REGION;
        private System.Windows.Forms.ListView listView_Browser;
        private System.Windows.Forms.ColumnHeader columnHeader1;

    }
}
