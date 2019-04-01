namespace GUI_Console
{
    partial class Form_UserSelect
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_UserSelect));
            this.roboPanel_Panel = new GUI_Controls.RoboPanel();
            this.button_Cancel1 = new GUI_Controls.Button_Cancel();
            this.listView_users = new System.Windows.Forms.ListView();
            this.Column1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.roboPanel_Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // roboPanel_Panel
            // 
            this.roboPanel_Panel.AccessibleDescription = "";
            this.roboPanel_Panel.AccessibleName = "Users";
            this.roboPanel_Panel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.roboPanel_Panel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.roboPanel_Panel.Controls.Add(this.button_Cancel1);
            this.roboPanel_Panel.Controls.Add(this.listView_users);
            this.roboPanel_Panel.Location = new System.Drawing.Point(0, 0);
            this.roboPanel_Panel.Name = "roboPanel_Panel";
            this.roboPanel_Panel.Size = new System.Drawing.Size(300, 350);
            this.roboPanel_Panel.TabIndex = 0;
            // 
            // button_Cancel1
            // 
            this.button_Cancel1.AccessibleName = "  ";
            this.button_Cancel1.BackgroundImage = GUI_Controls.Properties.Resources.Button_QUADRANTCANCEL;
            this.button_Cancel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Cancel1.Check = false;
            this.button_Cancel1.Location = new System.Drawing.Point(256, 8);
            this.button_Cancel1.Name = "button_Cancel1";
            this.button_Cancel1.Size = new System.Drawing.Size(33, 33);
            this.button_Cancel1.TabIndex = 16;
            this.button_Cancel1.Click += new System.EventHandler(this.button_Cancel1_Click);
            // 
            // listView_users
            // 
            this.listView_users.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.listView_users.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView_users.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Column1});
            this.listView_users.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 16.6F, System.Drawing.FontStyle.Bold);
            this.listView_users.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.listView_users.FullRowSelect = true;
            this.listView_users.GridLines = true;
            this.listView_users.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView_users.HideSelection = false;
            this.listView_users.Location = new System.Drawing.Point(23, 58);
            this.listView_users.MultiSelect = false;
            this.listView_users.Name = "listView_users";
            this.listView_users.Size = new System.Drawing.Size(242, 270);
            this.listView_users.TabIndex = 15;
            this.listView_users.TileSize = new System.Drawing.Size(1, 30);
            this.listView_users.UseCompatibleStateImageBehavior = false;
            this.listView_users.View = System.Windows.Forms.View.Details;
            this.listView_users.Click += new System.EventHandler(this.listView_users_Click);
            // 
            // Column1
            // 
            this.Column1.Text = "Date / Time";
            this.Column1.Width = 257;
            // 
            // Form_UserSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 350);
            this.Controls.Add(this.roboPanel_Panel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form_UserSelect";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form_UserSelect";
            this.Deactivate += new System.EventHandler(this.Form_UserSelect_Deactivate);
            this.Load += new System.EventHandler(this.Form_UserSelect_Load);
            this.LocationChanged += new System.EventHandler(this.Form_UserSelect_LocationChanged);
            this.roboPanel_Panel.ResumeLayout(false);
            this.roboPanel_Panel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private GUI_Controls.RoboPanel roboPanel_Panel;
        private System.Windows.Forms.ListView listView_users;
        private System.Windows.Forms.ColumnHeader Column1;
        private GUI_Controls.Button_Cancel button_Cancel1;

    }
}