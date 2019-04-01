namespace GUI_Console
{
    partial class RoboSep_UserProtocolList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoboSep_UserProtocolList));
            this.lblAllProtocols = new System.Windows.Forms.Label();
            this.button_Cancel = new GUI_Controls.Button_Cancel();
            this.label_userName = new System.Windows.Forms.Label();
            this.button_SaveList = new GUI_Controls.Button_SmallPink();
            this.button_Remove = new GUI_Controls.Button_SmallPink();
            this.button_ProtocolList = new GUI_Controls.Button_SmallPink();
            this.lvUserProtocols = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.spacer = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // lblAllProtocols
            // 
            this.lblAllProtocols.AutoSize = true;
            this.lblAllProtocols.BackColor = System.Drawing.Color.Transparent;
            this.lblAllProtocols.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 18F, System.Drawing.FontStyle.Bold);
            this.lblAllProtocols.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.lblAllProtocols.Location = new System.Drawing.Point(123, 71);
            this.lblAllProtocols.Name = "lblAllProtocols";
            this.lblAllProtocols.Size = new System.Drawing.Size(129, 31);
            this.lblAllProtocols.TabIndex = 32;
            this.lblAllProtocols.Text = "User Protocols";
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
            this.button_Cancel.TabIndex = 33;
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
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
            this.label_userName.TabIndex = 34;
            this.label_userName.Text = "User Name";
            // 
            // button_SaveList
            // 
            this.button_SaveList.AccessibleName = "  ";
            this.button_SaveList.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_SaveList.BackgroundImage")));
            this.button_SaveList.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_SaveList.Check = false;
            this.button_SaveList.Location = new System.Drawing.Point(17, 378);
            this.button_SaveList.Name = "button_SaveList";
            this.button_SaveList.Size = new System.Drawing.Size(77, 80);
            this.button_SaveList.TabIndex = 35;
            this.button_SaveList.Click += new System.EventHandler(this.button_SaveList_Click);
            // 
            // button_Remove
            // 
            this.button_Remove.AccessibleName = "  ";
            this.button_Remove.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Remove.BackgroundImage")));
            this.button_Remove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Remove.Check = false;
            this.button_Remove.Location = new System.Drawing.Point(10, 272);
            this.button_Remove.Name = "button_Remove";
            this.button_Remove.Size = new System.Drawing.Size(88, 59);
            this.button_Remove.TabIndex = 36;
            this.button_Remove.Click += new System.EventHandler(this.button_Remove_Click);
            // 
            // button_ProtocolList
            // 
            this.button_ProtocolList.AccessibleName = "  ";
            this.button_ProtocolList.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_ProtocolList.BackgroundImage")));
            this.button_ProtocolList.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_ProtocolList.Check = false;
            this.button_ProtocolList.Location = new System.Drawing.Point(12, 150);
            this.button_ProtocolList.Name = "button_ProtocolList";
            this.button_ProtocolList.Size = new System.Drawing.Size(82, 72);
            this.button_ProtocolList.TabIndex = 37;
            this.button_ProtocolList.Click += new System.EventHandler(this.button_AllProtocols_Click);
            // 
            // lvUserProtocols
            // 
            this.lvUserProtocols.BackColor = System.Drawing.Color.White;
            this.lvUserProtocols.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvUserProtocols.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvUserProtocols.Font = new System.Drawing.Font("Arial Narrow", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvUserProtocols.FullRowSelect = true;
            this.lvUserProtocols.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvUserProtocols.Location = new System.Drawing.Point(119, 119);
            this.lvUserProtocols.Name = "lvUserProtocols";
            this.lvUserProtocols.ShowGroups = false;
            this.lvUserProtocols.Size = new System.Drawing.Size(500, 342);
            this.lvUserProtocols.SmallImageList = this.spacer;
            this.lvUserProtocols.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvUserProtocols.TabIndex = 38;
            this.lvUserProtocols.UseCompatibleStateImageBehavior = false;
            this.lvUserProtocols.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Protocols";
            this.columnHeader1.Width = 490;
            // 
            // spacer
            // 
            this.spacer.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.spacer.ImageSize = new System.Drawing.Size(1, 42);
            this.spacer.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // RoboSep_UserProtocolList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::GUI_Console.Properties.Resources.BG_UserProtocolList;
            this.Controls.Add(this.lvUserProtocols);
            this.Controls.Add(this.button_ProtocolList);
            this.Controls.Add(this.button_Remove);
            this.Controls.Add(this.button_SaveList);
            this.Controls.Add(this.label_userName);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.lblAllProtocols);
            this.Name = "RoboSep_UserProtocolList";
            this.Size = new System.Drawing.Size(640, 480);
            this.Load += new System.EventHandler(this.RoboSep_UserProtocolList_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblAllProtocols;
        private GUI_Controls.Button_Cancel button_Cancel;
        private System.Windows.Forms.Label label_userName;
        private GUI_Controls.Button_SmallPink button_SaveList;
        private GUI_Controls.Button_SmallPink button_Remove;
        private GUI_Controls.Button_SmallPink button_ProtocolList;
        private System.Windows.Forms.ListView lvUserProtocols;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ImageList spacer;
    }
}
