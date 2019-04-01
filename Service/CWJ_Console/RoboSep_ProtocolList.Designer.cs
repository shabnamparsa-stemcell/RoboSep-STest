namespace GUI_Console
{
    partial class RoboSep_ProtocolList
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Item1");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("I2 aaaaaaaa aaaaaaaaa aaaaaaaaaaaaaaaaaaaaaaaaaaaaaa aaaaaaaaa aaa");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("I3");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("I4");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("I5");
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("I6");
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem("I7");
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem("I8");
            System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem("I9");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoboSep_ProtocolList));
            this.label_userName = new System.Windows.Forms.Label();
            this.lblAllProtocols = new System.Windows.Forms.Label();
            this.lvProtocolList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.spacer = new System.Windows.Forms.ImageList(this.components);
            this.SearchBar = new GUI_Controls.Protocol_SearchBar();
            this.button_SaveList = new GUI_Controls.Button_SmallPink();
            this.button_Add = new GUI_Controls.Button_SmallPink();
            this.button_myProfile = new GUI_Controls.Button_SmallPink();
            this.button_USBload = new GUI_Controls.Button_Pill();
            this.button_Cancel = new GUI_Controls.Button_Cancel();
            this.SuspendLayout();
            // 
            // label_userName
            // 
            this.label_userName.AutoSize = true;
            this.label_userName.BackColor = System.Drawing.Color.Transparent;
            this.label_userName.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_userName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.label_userName.Location = new System.Drawing.Point(423, 12);
            this.label_userName.Name = "label_userName";
            this.label_userName.Size = new System.Drawing.Size(75, 24);
            this.label_userName.TabIndex = 25;
            this.label_userName.Text = "User Name";
            // 
            // lblAllProtocols
            // 
            this.lblAllProtocols.AutoSize = true;
            this.lblAllProtocols.BackColor = System.Drawing.Color.Transparent;
            this.lblAllProtocols.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 18F, System.Drawing.FontStyle.Bold);
            this.lblAllProtocols.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.lblAllProtocols.Location = new System.Drawing.Point(22, 65);
            this.lblAllProtocols.Name = "lblAllProtocols";
            this.lblAllProtocols.Size = new System.Drawing.Size(111, 31);
            this.lblAllProtocols.TabIndex = 31;
            this.lblAllProtocols.Text = "All Protocols";
            // 
            // lvProtocolList
            // 
            this.lvProtocolList.BackColor = System.Drawing.Color.White;
            this.lvProtocolList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvProtocolList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvProtocolList.Font = new System.Drawing.Font("Arial Narrow", 12F);
            this.lvProtocolList.FullRowSelect = true;
            this.lvProtocolList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvProtocolList.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6,
            listViewItem7,
            listViewItem8,
            listViewItem9});
            this.lvProtocolList.Location = new System.Drawing.Point(21, 118);
            this.lvProtocolList.Name = "lvProtocolList";
            this.lvProtocolList.ShowGroups = false;
            this.lvProtocolList.Size = new System.Drawing.Size(510, 301);
            this.lvProtocolList.SmallImageList = this.spacer;
            this.lvProtocolList.TabIndex = 32;
            this.lvProtocolList.UseCompatibleStateImageBehavior = false;
            this.lvProtocolList.View = System.Windows.Forms.View.Details;
            this.lvProtocolList.SelectedIndexChanged += new System.EventHandler(this.lvProtocolList_MouseLeave);
            this.lvProtocolList.MouseLeave += new System.EventHandler(this.lvProtocolList_MouseLeave);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Protocols";
            this.columnHeader1.Width = 493;
            // 
            // spacer
            // 
            this.spacer.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.spacer.ImageSize = new System.Drawing.Size(1, 42);
            this.spacer.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // SearchBar
            // 
            this.SearchBar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("SearchBar.BackgroundImage")));
            this.SearchBar.Location = new System.Drawing.Point(184, 55);
            this.SearchBar.Name = "SearchBar";
            this.SearchBar.Size = new System.Drawing.Size(351, 51);
            this.SearchBar.TabIndex = 33;
            // 
            // button_SaveList
            // 
            this.button_SaveList.AccessibleName = "  ";
            this.button_SaveList.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_SaveList.BackgroundImage")));
            this.button_SaveList.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_SaveList.Check = false;
            this.button_SaveList.Location = new System.Drawing.Point(549, 378);
            this.button_SaveList.Name = "button_SaveList";
            this.button_SaveList.Size = new System.Drawing.Size(77, 80);
            this.button_SaveList.TabIndex = 30;
            this.button_SaveList.Click += new System.EventHandler(this.button_SaveList_Click);
            // 
            // button_Add
            // 
            this.button_Add.AccessibleName = "  ";
            this.button_Add.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Add.BackgroundImage")));
            this.button_Add.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Add.Check = false;
            this.button_Add.Location = new System.Drawing.Point(549, 272);
            this.button_Add.Name = "button_Add";
            this.button_Add.Size = new System.Drawing.Size(79, 59);
            this.button_Add.TabIndex = 29;
            this.button_Add.Click += new System.EventHandler(this.button_Add_Click);
            // 
            // button_myProfile
            // 
            this.button_myProfile.AccessibleName = "  ";
            this.button_myProfile.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_myProfile.BackgroundImage")));
            this.button_myProfile.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_myProfile.Check = false;
            this.button_myProfile.Location = new System.Drawing.Point(549, 150);
            this.button_myProfile.Name = "button_myProfile";
            this.button_myProfile.Size = new System.Drawing.Size(82, 72);
            this.button_myProfile.TabIndex = 28;
            this.button_myProfile.Click += new System.EventHandler(this.button_myProfile_Click);
            // 
            // button_USBload
            // 
            this.button_USBload.AccessibleName = "  ";
            this.button_USBload.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_USBload.BackgroundImage")));
            this.button_USBload.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_USBload.Check = false;
            this.button_USBload.Location = new System.Drawing.Point(28, 425);
            this.button_USBload.Name = "button_USBload";
            this.button_USBload.Size = new System.Drawing.Size(138, 41);
            this.button_USBload.TabIndex = 27;
            this.button_USBload.Click += new System.EventHandler(this.button_USBload_Click);
            // 
            // button_Cancel
            // 
            this.button_Cancel.AccessibleName = "  ";
            this.button_Cancel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Cancel.BackgroundImage")));
            this.button_Cancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Cancel.Check = false;
            this.button_Cancel.Location = new System.Drawing.Point(600, 8);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(33, 33);
            this.button_Cancel.TabIndex = 24;
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // RoboSep_ProtocolList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::GUI_Console.Properties.Resources.BG_EditProtocolList;
            this.Controls.Add(this.SearchBar);
            this.Controls.Add(this.lvProtocolList);
            this.Controls.Add(this.lblAllProtocols);
            this.Controls.Add(this.button_SaveList);
            this.Controls.Add(this.button_Add);
            this.Controls.Add(this.button_myProfile);
            this.Controls.Add(this.button_USBload);
            this.Controls.Add(this.label_userName);
            this.Controls.Add(this.button_Cancel);
            this.Name = "RoboSep_ProtocolList";
            this.Size = new System.Drawing.Size(640, 480);
            this.Load += new System.EventHandler(this.RoboSep_ProtocolList_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GUI_Controls.Button_Cancel button_Cancel;
        private System.Windows.Forms.Label label_userName;
        private GUI_Controls.Button_Pill button_USBload;
        private GUI_Controls.Button_SmallPink button_myProfile;
        private GUI_Controls.Button_SmallPink button_Add;
        private GUI_Controls.Button_SmallPink button_SaveList;
        private System.Windows.Forms.Label lblAllProtocols;
        private System.Windows.Forms.ListView lvProtocolList;
        private System.Windows.Forms.ImageList spacer;
        private GUI_Controls.Protocol_SearchBar SearchBar;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}
