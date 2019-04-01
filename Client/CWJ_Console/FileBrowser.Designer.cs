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
            this.labelPath = new System.Windows.Forms.Label();
            this.labelFilterFor = new System.Windows.Forms.Label();
            this.textBox_fileExtension = new System.Windows.Forms.TextBox();
            this.textbox_DirName = new System.Windows.Forms.TextBox();
            this.label_NewDirectory = new System.Windows.Forms.Label();
            this.robopanel_NewDir = new System.Windows.Forms.Panel();
            this.button_newDirCancel = new GUI_Controls.Button_Rectangle();
            this.button_newDirAccept = new GUI_Controls.Button_Rectangle();
            this.shapeContainer2 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.rectangleShape2 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.scrollBar_FileBrowser = new GUI_Controls.ScrollBar();
            this.lineShape1 = new Microsoft.VisualBasic.PowerPacks.LineShape();
            this.shapeContainer3 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.lineShape2 = new Microsoft.VisualBasic.PowerPacks.LineShape();
            this.rectangleShape1 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelTitle = new System.Windows.Forms.Label();
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.button_OK = new GUI_Controls.Button_Rectangle();
            this.button_Cancel = new GUI_Controls.Button_Rectangle();
            this.button_Back = new GUI_Controls.Button_Rectangle();
            this.button_Up = new GUI_Controls.Button_Rectangle();
            this.label_SelectAll = new GUI_Controls.SizableLabel();
            this.checkBox_SelectAll = new GUI_Controls.CheckBox();
            this.button_CreateDirectory = new GUI_Controls.Button_Rectangle();
            this.button_Refresh = new GUI_Controls.Button_Rectangle();
            this.listView_Browser = new GUI_Controls.DragScrollListView3();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.robopanel_NewDir.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelPath
            // 
            this.labelPath.BackColor = System.Drawing.Color.Transparent;
            this.labelPath.Font = new System.Drawing.Font("Arial Narrow", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(38)))), ((int)(((byte)(131)))));
            this.labelPath.Location = new System.Drawing.Point(12, 46);
            this.labelPath.Name = "labelPath";
            this.labelPath.Size = new System.Drawing.Size(255, 25);
            this.labelPath.TabIndex = 6;
            this.labelPath.Text = "E:\\Protocols\\..";
            // 
            // labelFilterFor
            // 
            this.labelFilterFor.AutoSize = true;
            this.labelFilterFor.BackColor = System.Drawing.Color.Transparent;
            this.labelFilterFor.Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFilterFor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(38)))), ((int)(((byte)(131)))));
            this.labelFilterFor.Location = new System.Drawing.Point(11, 393);
            this.labelFilterFor.Name = "labelFilterFor";
            this.labelFilterFor.Size = new System.Drawing.Size(83, 23);
            this.labelFilterFor.TabIndex = 7;
            this.labelFilterFor.Text = "Filter For:";
            // 
            // textBox_fileExtension
            // 
            this.textBox_fileExtension.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_fileExtension.Font = new System.Drawing.Font("Arial Narrow", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_fileExtension.Location = new System.Drawing.Point(89, 389);
            this.textBox_fileExtension.Name = "textBox_fileExtension";
            this.textBox_fileExtension.Size = new System.Drawing.Size(79, 32);
            this.textBox_fileExtension.TabIndex = 8;
            this.textBox_fileExtension.Text = ".XML";
            this.textBox_fileExtension.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_fileExtension.TextChanged += new System.EventHandler(this.textBox_fileExtension_TextChanged);
            this.textBox_fileExtension.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_fileExtension_KeyDown);
            // 
            // textbox_DirName
            // 
            this.textbox_DirName.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.textbox_DirName.Location = new System.Drawing.Point(20, 38);
            this.textbox_DirName.Name = "textbox_DirName";
            this.textbox_DirName.Size = new System.Drawing.Size(185, 30);
            this.textbox_DirName.TabIndex = 2;
            this.textbox_DirName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textbox_DirName.Click += new System.EventHandler(this.textBox_DirName_Click);
            // 
            // label_NewDirectory
            // 
            this.label_NewDirectory.AutoSize = true;
            this.label_NewDirectory.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.label_NewDirectory.Location = new System.Drawing.Point(5, 5);
            this.label_NewDirectory.Name = "label_NewDirectory";
            this.label_NewDirectory.Size = new System.Drawing.Size(214, 26);
            this.label_NewDirectory.TabIndex = 12;
            this.label_NewDirectory.Text = "New Directory Name";
            // 
            // robopanel_NewDir
            // 
            this.robopanel_NewDir.Controls.Add(this.button_newDirCancel);
            this.robopanel_NewDir.Controls.Add(this.textbox_DirName);
            this.robopanel_NewDir.Controls.Add(this.button_newDirAccept);
            this.robopanel_NewDir.Controls.Add(this.label_NewDirectory);
            this.robopanel_NewDir.Controls.Add(this.shapeContainer2);
            this.robopanel_NewDir.Location = new System.Drawing.Point(241, 86);
            this.robopanel_NewDir.Name = "robopanel_NewDir";
            this.robopanel_NewDir.Size = new System.Drawing.Size(228, 148);
            this.robopanel_NewDir.TabIndex = 22;
            this.robopanel_NewDir.Visible = false;
            // 
            // button_newDirCancel
            // 
            this.button_newDirCancel.BackColor = System.Drawing.Color.Transparent;
            this.button_newDirCancel.BackgroundImage = global::GUI_Console.Properties.Resources.btnsmall_STD;
            this.button_newDirCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_newDirCancel.Check = false;
            this.button_newDirCancel.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_newDirCancel.ForeColor = System.Drawing.Color.White;
            this.button_newDirCancel.Location = new System.Drawing.Point(39, 88);
            this.button_newDirCancel.Name = "button_newDirCancel";
            this.button_newDirCancel.Size = new System.Drawing.Size(74, 35);
            this.button_newDirCancel.TabIndex = 14;
            this.button_newDirCancel.Text = "Cancel";
            this.button_newDirCancel.Click += new System.EventHandler(this.button_newDirCancel_Click);
            // 
            // button_newDirAccept
            // 
            this.button_newDirAccept.BackColor = System.Drawing.Color.Transparent;
            this.button_newDirAccept.BackgroundImage = global::GUI_Console.Properties.Resources.btnsmall_STD;
            this.button_newDirAccept.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_newDirAccept.Check = false;
            this.button_newDirAccept.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_newDirAccept.ForeColor = System.Drawing.Color.White;
            this.button_newDirAccept.Location = new System.Drawing.Point(131, 88);
            this.button_newDirAccept.Name = "button_newDirAccept";
            this.button_newDirAccept.Size = new System.Drawing.Size(74, 35);
            this.button_newDirAccept.TabIndex = 13;
            this.button_newDirAccept.Text = "Accept";
            this.button_newDirAccept.Click += new System.EventHandler(this.button_newDirAccept_Click);
            // 
            // shapeContainer2
            // 
            this.shapeContainer2.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer2.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer2.Name = "shapeContainer2";
            this.shapeContainer2.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.rectangleShape2});
            this.shapeContainer2.Size = new System.Drawing.Size(228, 148);
            this.shapeContainer2.TabIndex = 22;
            this.shapeContainer2.TabStop = false;
            // 
            // rectangleShape2
            // 
            this.rectangleShape2.BorderWidth = 2;
            this.rectangleShape2.Location = new System.Drawing.Point(0, 0);
            this.rectangleShape2.Name = "rectangleShape2";
            this.rectangleShape2.Size = new System.Drawing.Size(226, 146);
            // 
            // scrollBar_FileBrowser
            // 
            this.scrollBar_FileBrowser.ActiveBackColor = System.Drawing.Color.Gray;
            this.scrollBar_FileBrowser.LargeChange = 10;
            this.scrollBar_FileBrowser.Location = new System.Drawing.Point(428, 82);
            this.scrollBar_FileBrowser.Maximum = 99;
            this.scrollBar_FileBrowser.Minimum = 0;
            this.scrollBar_FileBrowser.Name = "scrollBar_FileBrowser";
            this.scrollBar_FileBrowser.Size = new System.Drawing.Size(49, 293);
            this.scrollBar_FileBrowser.SmallChange = 1;
            this.scrollBar_FileBrowser.TabIndex = 56;
            this.scrollBar_FileBrowser.Text = "scrollBar1";
            this.scrollBar_FileBrowser.ThumbStyle = GUI_Controls.ScrollBar.EnumThumbStyle.Auto;
            this.scrollBar_FileBrowser.Value = 0;
            // 
            // lineShape1
            // 
            this.lineShape1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(146)))), ((int)(((byte)(108)))), ((int)(((byte)(175)))));
            this.lineShape1.BorderWidth = 3;
            this.lineShape1.Name = "lineShape1";
            this.lineShape1.X1 = 1;
            this.lineShape1.X2 = 475;
            this.lineShape1.Y1 = 79;
            this.lineShape1.Y2 = 79;
            // 
            // shapeContainer3
            // 
            this.shapeContainer3.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer3.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer3.Name = "shapeContainer3";
            this.shapeContainer3.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.lineShape2,
            this.lineShape1});
            this.shapeContainer3.Size = new System.Drawing.Size(478, 426);
            this.shapeContainer3.TabIndex = 30;
            this.shapeContainer3.TabStop = false;
            // 
            // lineShape2
            // 
            this.lineShape2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(146)))), ((int)(((byte)(108)))), ((int)(((byte)(175)))));
            this.lineShape2.BorderWidth = 3;
            this.lineShape2.Name = "lineShape2";
            this.lineShape2.X1 = 2;
            this.lineShape2.X2 = 476;
            this.lineShape2.Y1 = 384;
            this.lineShape2.Y2 = 384;
            // 
            // rectangleShape1
            // 
            this.rectangleShape1.BorderWidth = 2;
            this.rectangleShape1.Location = new System.Drawing.Point(0, 0);
            this.rectangleShape1.Name = "rectangleShape1";
            this.rectangleShape1.Size = new System.Drawing.Size(477, 425);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.Controls.Add(this.labelTitle);
            this.panel1.Controls.Add(this.shapeContainer1);
            this.panel1.Location = new System.Drawing.Point(1, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(476, 37);
            this.panel1.TabIndex = 29;
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.BackColor = System.Drawing.Color.Transparent;
            this.labelTitle.Font = new System.Drawing.Font("Arial", 15F);
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Location = new System.Drawing.Point(6, 6);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(121, 23);
            this.labelTitle.TabIndex = 5;
            this.labelTitle.Text = "File Browser";
            // 
            // shapeContainer1
            // 
            this.shapeContainer1.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer1.Name = "shapeContainer1";
            this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.rectangleShape1});
            this.shapeContainer1.Size = new System.Drawing.Size(476, 37);
            this.shapeContainer1.TabIndex = 6;
            this.shapeContainer1.TabStop = false;
            // 
            // button_OK
            // 
            this.button_OK.BackColor = System.Drawing.Color.Transparent;
            this.button_OK.BackgroundImage = global::GUI_Console.Properties.Resources.btnsmall_STD;
            this.button_OK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_OK.Check = false;
            this.button_OK.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_OK.ForeColor = System.Drawing.Color.White;
            this.button_OK.Location = new System.Drawing.Point(399, 387);
            this.button_OK.Name = "button_OK";
            this.button_OK.Size = new System.Drawing.Size(74, 35);
            this.button_OK.TabIndex = 4;
            this.button_OK.Text = "OK";
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // button_Cancel
            // 
            this.button_Cancel.BackColor = System.Drawing.Color.Transparent;
            this.button_Cancel.BackgroundImage = global::GUI_Console.Properties.Resources.btnsmall_STD;
            this.button_Cancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Cancel.Check = false;
            this.button_Cancel.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Cancel.ForeColor = System.Drawing.Color.White;
            this.button_Cancel.Location = new System.Drawing.Point(312, 387);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(74, 35);
            this.button_Cancel.TabIndex = 3;
            this.button_Cancel.Text = "Cancel";
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // button_Back
            // 
            this.button_Back.BackColor = System.Drawing.Color.Transparent;
            this.button_Back.BackgroundImage = global::GUI_Console.Properties.Resources.fbBack_STD;
            this.button_Back.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Back.Check = false;
            this.button_Back.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Back.ForeColor = System.Drawing.Color.White;
            this.button_Back.Location = new System.Drawing.Point(335, 40);
            this.button_Back.Name = "button_Back";
            this.button_Back.Size = new System.Drawing.Size(39, 34);
            this.button_Back.TabIndex = 0;
            this.button_Back.Text = "  ";
            this.button_Back.Click += new System.EventHandler(this.button_Back_Click);
            // 
            // button_Up
            // 
            this.button_Up.BackColor = System.Drawing.Color.Transparent;
            this.button_Up.BackgroundImage = global::GUI_Console.Properties.Resources.fbUp_STD;
            this.button_Up.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Up.Check = false;
            this.button_Up.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Up.ForeColor = System.Drawing.Color.White;
            this.button_Up.Location = new System.Drawing.Point(383, 40);
            this.button_Up.Name = "button_Up";
            this.button_Up.Size = new System.Drawing.Size(39, 34);
            this.button_Up.TabIndex = 1;
            this.button_Up.Text = "  ";
            this.button_Up.Click += new System.EventHandler(this.button_Up_Click);
            // 
            // label_SelectAll
            // 
            this.label_SelectAll.AutoSize = true;
            this.label_SelectAll.BackColor = System.Drawing.Color.Transparent;
            this.label_SelectAll.Font = new System.Drawing.Font("Arial Narrow", 14F, System.Drawing.FontStyle.Bold);
            this.label_SelectAll.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(38)))), ((int)(((byte)(131)))));
            this.label_SelectAll.Location = new System.Drawing.Point(210, 393);
            this.label_SelectAll.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.label_SelectAll.Name = "label_SelectAll";
            this.label_SelectAll.Size = new System.Drawing.Size(59, 23);
            this.label_SelectAll.TabIndex = 32;
            this.label_SelectAll.Text = "All";
            // 
            // checkBox_SelectAll
            // 
            this.checkBox_SelectAll.AutoSize = true;
            this.checkBox_SelectAll.BackColor = System.Drawing.Color.Transparent;
            this.checkBox_SelectAll.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("checkBox_SelectAll.BackgroundImage")));
            this.checkBox_SelectAll.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.checkBox_SelectAll.Check = false;
            this.checkBox_SelectAll.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_SelectAll.ForeColor = System.Drawing.Color.White;
            this.checkBox_SelectAll.Location = new System.Drawing.Point(178, 393);
            this.checkBox_SelectAll.Name = "checkBox_SelectAll";
            this.checkBox_SelectAll.Size = new System.Drawing.Size(21, 22);
            this.checkBox_SelectAll.TabIndex = 31;
            this.checkBox_SelectAll.Text = "  ";
            this.checkBox_SelectAll.Click += new System.EventHandler(this.checkBox_SelectAll_Click);
            // 
            // button_CreateDirectory
            // 
            this.button_CreateDirectory.BackColor = System.Drawing.Color.Transparent;
            this.button_CreateDirectory.BackgroundImage = global::GUI_Console.Properties.Resources.fbNewDir_STD;
            this.button_CreateDirectory.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_CreateDirectory.Check = false;
            this.button_CreateDirectory.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_CreateDirectory.ForeColor = System.Drawing.Color.White;
            this.button_CreateDirectory.Location = new System.Drawing.Point(287, 40);
            this.button_CreateDirectory.Name = "button_CreateDirectory";
            this.button_CreateDirectory.Size = new System.Drawing.Size(39, 34);
            this.button_CreateDirectory.TabIndex = 12;
            this.button_CreateDirectory.Text = "  ";
            this.button_CreateDirectory.Click += new System.EventHandler(this.button_CreateDirectory_Click);
            // 
            // button_Refresh
            // 
            this.button_Refresh.BackColor = System.Drawing.Color.Transparent;
            this.button_Refresh.BackgroundImage = global::GUI_Console.Properties.Resources.fbRefresh_STD;
            this.button_Refresh.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Refresh.Check = false;
            this.button_Refresh.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Refresh.ForeColor = System.Drawing.Color.White;
            this.button_Refresh.Location = new System.Drawing.Point(430, 39);
            this.button_Refresh.Name = "button_Refresh";
            this.button_Refresh.Size = new System.Drawing.Size(39, 37);
            this.button_Refresh.TabIndex = 2;
            this.button_Refresh.Text = "  ";
            this.button_Refresh.Click += new System.EventHandler(this.button_Refresh_Click);
            // 
            // listView_Browser
            // 
            this.listView_Browser.AutoArrange = false;
            this.listView_Browser.BackColor = System.Drawing.SystemColors.Control;
            this.listView_Browser.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView_Browser.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listView_Browser.FullRowSelect = true;
            this.listView_Browser.GridLines = true;
            this.listView_Browser.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView_Browser.HideSelection = false;
            this.listView_Browser.Location = new System.Drawing.Point(3, 82);
            this.listView_Browser.Name = "listView_Browser";
            this.listView_Browser.OwnerDraw = true;
            this.listView_Browser.Size = new System.Drawing.Size(419, 291);
            this.listView_Browser.TabIndex = 11;
            this.listView_Browser.TileSize = new System.Drawing.Size(435, 60);
            this.listView_Browser.UseCompatibleStateImageBehavior = false;
            this.listView_Browser.View = System.Windows.Forms.View.Details;
            this.listView_Browser.VisibleRow = 8;
            this.listView_Browser.VScrollbar = null;
            this.listView_Browser.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.listView_Browser_DrawColumnHeader);
            this.listView_Browser.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.listView_Browser_DrawItem);
            this.listView_Browser.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.listView_Browser_DrawSubItem);
            this.listView_Browser.SelectedIndexChanged += new System.EventHandler(this.listView_Browser_SelectedIndexChanged);
            this.listView_Browser.DoubleClick += new System.EventHandler(this.listView_Browser_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Files";
            this.columnHeader1.Width = 433;
            // 
            // FileBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(478, 426);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.textBox_fileExtension);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_Back);
            this.Controls.Add(this.button_Up);
            this.Controls.Add(this.label_SelectAll);
            this.Controls.Add(this.checkBox_SelectAll);
            this.Controls.Add(this.labelFilterFor);
            this.Controls.Add(this.robopanel_NewDir);
            this.Controls.Add(this.button_CreateDirectory);
            this.Controls.Add(this.labelPath);
            this.Controls.Add(this.button_Refresh);
            this.Controls.Add(this.scrollBar_FileBrowser);
            this.Controls.Add(this.listView_Browser);
            this.Controls.Add(this.shapeContainer3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Location = new System.Drawing.Point(20, 30);
            this.Name = "FileBrowser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Load += new System.EventHandler(this.FileBrowser_Load);
            this.robopanel_NewDir.ResumeLayout(false);
            this.robopanel_NewDir.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GUI_Controls.Button_Rectangle button_Back;
        private GUI_Controls.Button_Rectangle button_Up;
        private GUI_Controls.Button_Rectangle button_Refresh;
        private GUI_Controls.Button_Rectangle button_Cancel;
        private GUI_Controls.Button_Rectangle button_OK;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelPath;
        private System.Windows.Forms.Label labelFilterFor;
        private System.Windows.Forms.TextBox textBox_fileExtension;
        private GUI_Controls.DragScrollListView3 listView_Browser;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private GUI_Controls.Button_Rectangle button_CreateDirectory;
        private System.Windows.Forms.Label label_NewDirectory;
        private GUI_Controls.Button_Rectangle button_newDirCancel;
        private GUI_Controls.Button_Rectangle button_newDirAccept;
        private System.Windows.Forms.TextBox textbox_DirName;
        private System.Windows.Forms.Panel robopanel_NewDir;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer2;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape2;
        private System.Windows.Forms.Panel panel1;
        private Microsoft.VisualBasic.PowerPacks.LineShape lineShape1;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer3;
        private Microsoft.VisualBasic.PowerPacks.LineShape lineShape2;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape1;
        private GUI_Controls.SizableLabel label_SelectAll;
        private GUI_Controls.CheckBox checkBox_SelectAll;
        private GUI_Controls.ScrollBar scrollBar_FileBrowser;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;

    }
}
