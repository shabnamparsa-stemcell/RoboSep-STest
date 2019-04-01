namespace GUI_Console
{
    partial class RoboSep_Home
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
            for (i = 0; i < IListRunProgress.Count; i++ )
            {
                IListRunProgress[i].Dispose();
            }
            for (i = 0; i < IListRunSamples.Count; i++)
            {
                IListRunSamples[i].Dispose();
            }
            for (i = 0; i < IList1.Count; i++)
            {
                IList1[i].Dispose();
            }
            for (i = 0; i < IList2.Count; i++)
            {
                IList2[i].Dispose();
            }
            for (i = 0; i < IList3.Count; i++)
            {
                IList3[i].Dispose();
            }
            for (i = 0; i < IList4.Count; i++)
            {
                IList4[i].Dispose();
            }
            for (i = 0; i < IList5.Count; i++)
            {
                IList5[i].Dispose();
            }
            for (i = 0; i < IList6.Count; i++)
            {
                IList6[i].Dispose();
            }
            for (i = 0; i < IList7.Count; i++)
            {
                IList7[i].Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modifyS
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button_UserSelect = new System.Windows.Forms.Button();
            this.label_runsamples = new System.Windows.Forms.Label();
            this.label_Settings = new System.Windows.Forms.Label();
            this.label_Help = new System.Windows.Forms.Label();
            this.label_Shutdown = new System.Windows.Forms.Label();
            this.label_Reports = new System.Windows.Forms.Label();
            this.label_Preferences = new System.Windows.Forms.Label();
            this.label_Protocols = new System.Windows.Forms.Label();
            this.label_users = new System.Windows.Forms.Label();
            this.hex_Users = new GUI_Controls.Button_Circle();
            this.hex_protocols = new GUI_Controls.Button_Circle();
            this.hex_UserPreferences = new GUI_Controls.Button_Circle();
            this.hex_logs = new GUI_Controls.Button_Circle();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.hex_shutdown = new GUI_Controls.Button_Circle();
            this.hex_help = new GUI_Controls.Button_Circle();
            this.hex_system = new GUI_Controls.Button_Circle();
            this.hex_sampling = new GUI_Controls.Button_Circle();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // button_UserSelect
            // 
            this.button_UserSelect.Location = new System.Drawing.Point(53, 12);
            this.button_UserSelect.Name = "button_UserSelect";
            this.button_UserSelect.Size = new System.Drawing.Size(99, 102);
            this.button_UserSelect.TabIndex = 8;
            this.button_UserSelect.Text = "Select User";
            this.button_UserSelect.UseVisualStyleBackColor = true;
            this.button_UserSelect.Visible = false;
            this.button_UserSelect.Click += new System.EventHandler(this.button_UserSelect_Click);
            // 
            // label_runsamples
            // 
            this.label_runsamples.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label_runsamples.AutoSize = true;
            this.label_runsamples.BackColor = System.Drawing.Color.Transparent;
            this.label_runsamples.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_runsamples.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.label_runsamples.Location = new System.Drawing.Point(53, 253);
            this.label_runsamples.Name = "label_runsamples";
            this.label_runsamples.Size = new System.Drawing.Size(104, 20);
            this.label_runsamples.TabIndex = 16;
            this.label_runsamples.Text = "RUN SAMPLES";
            // 
            // label_Settings
            // 
            this.label_Settings.AutoSize = true;
            this.label_Settings.BackColor = System.Drawing.Color.Transparent;
            this.label_Settings.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Settings.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.label_Settings.Location = new System.Drawing.Point(192, 427);
            this.label_Settings.Name = "label_Settings";
            this.label_Settings.Size = new System.Drawing.Size(104, 20);
            this.label_Settings.TabIndex = 18;
            this.label_Settings.Text = "MAINTENANCE";
            // 
            // label_Help
            // 
            this.label_Help.AutoSize = true;
            this.label_Help.BackColor = System.Drawing.Color.Transparent;
            this.label_Help.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Help.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.label_Help.Location = new System.Drawing.Point(357, 427);
            this.label_Help.Name = "label_Help";
            this.label_Help.Size = new System.Drawing.Size(44, 20);
            this.label_Help.TabIndex = 19;
            this.label_Help.Text = "HELP";
            // 
            // label_Shutdown
            // 
            this.label_Shutdown.AutoSize = true;
            this.label_Shutdown.BackColor = System.Drawing.Color.Transparent;
            this.label_Shutdown.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Shutdown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.label_Shutdown.Location = new System.Drawing.Point(485, 427);
            this.label_Shutdown.Name = "label_Shutdown";
            this.label_Shutdown.Size = new System.Drawing.Size(87, 20);
            this.label_Shutdown.TabIndex = 20;
            this.label_Shutdown.Text = "SHUT DOWN";
            this.label_Shutdown.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_Reports
            // 
            this.label_Reports.AutoSize = true;
            this.label_Reports.BackColor = System.Drawing.Color.Transparent;
            this.label_Reports.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Reports.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.label_Reports.Location = new System.Drawing.Point(68, 427);
            this.label_Reports.Name = "label_Reports";
            this.label_Reports.Size = new System.Drawing.Size(72, 20);
            this.label_Reports.TabIndex = 23;
            this.label_Reports.Text = "REPORTS";
            // 
            // label_Preferences
            // 
            this.label_Preferences.AutoSize = true;
            this.label_Preferences.BackColor = System.Drawing.Color.Transparent;
            this.label_Preferences.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Preferences.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.label_Preferences.Location = new System.Drawing.Point(477, 253);
            this.label_Preferences.Name = "label_Preferences";
            this.label_Preferences.Size = new System.Drawing.Size(107, 20);
            this.label_Preferences.TabIndex = 26;
            this.label_Preferences.Text = "PREFERENCES";
            // 
            // label_Protocols
            // 
            this.label_Protocols.AutoSize = true;
            this.label_Protocols.BackColor = System.Drawing.Color.Transparent;
            this.label_Protocols.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Protocols.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.label_Protocols.Location = new System.Drawing.Point(341, 253);
            this.label_Protocols.Name = "label_Protocols";
            this.label_Protocols.Size = new System.Drawing.Size(91, 20);
            this.label_Protocols.TabIndex = 28;
            this.label_Protocols.Text = "PROTOCOLS";
            // 
            // label_users
            // 
            this.label_users.AutoSize = true;
            this.label_users.BackColor = System.Drawing.Color.Transparent;
            this.label_users.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_users.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.label_users.Location = new System.Drawing.Point(214, 253);
            this.label_users.Name = "label_users";
            this.label_users.Size = new System.Drawing.Size(54, 20);
            this.label_users.TabIndex = 30;
            this.label_users.Text = "USERS";
            // 
            // hex_Users
            // 
            this.hex_Users.BackColor = System.Drawing.Color.Transparent;
            this.hex_Users.BackgroundImage = global::GUI_Console.Properties.Resources.L_104x86_Users_single_STD;
            this.hex_Users.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.hex_Users.Check = false;
            this.hex_Users.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hex_Users.ForeColor = System.Drawing.Color.White;
            this.hex_Users.Location = new System.Drawing.Point(190, 148);
            this.hex_Users.Name = "hex_Users";
            this.hex_Users.Size = new System.Drawing.Size(104, 86);
            this.hex_Users.TabIndex = 29;
            this.hex_Users.Text = "  ";
            this.hex_Users.Click += new System.EventHandler(this.hex_Users_Click);
            // 
            // hex_protocols
            // 
            this.hex_protocols.BackColor = System.Drawing.Color.Transparent;
            this.hex_protocols.BackgroundImage = global::GUI_Console.Properties.Resources.GE_BTN23L_protocol_STD;
            this.hex_protocols.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.hex_protocols.Check = false;
            this.hex_protocols.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hex_protocols.ForeColor = System.Drawing.Color.White;
            this.hex_protocols.Location = new System.Drawing.Point(328, 148);
            this.hex_protocols.Name = "hex_protocols";
            this.hex_protocols.Size = new System.Drawing.Size(104, 86);
            this.hex_protocols.TabIndex = 27;
            this.hex_protocols.Text = "  ";
            this.hex_protocols.Click += new System.EventHandler(this.hex_protocols_Click);
            // 
            // hex_UserPreferences
            // 
            this.hex_UserPreferences.BackColor = System.Drawing.Color.Transparent;
            this.hex_UserPreferences.BackgroundImage = global::GUI_Console.Properties.Resources.GE_BTN14L_preferences_STD;
            this.hex_UserPreferences.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.hex_UserPreferences.Check = false;
            this.hex_UserPreferences.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hex_UserPreferences.ForeColor = System.Drawing.Color.White;
            this.hex_UserPreferences.Location = new System.Drawing.Point(474, 148);
            this.hex_UserPreferences.Name = "hex_UserPreferences";
            this.hex_UserPreferences.Size = new System.Drawing.Size(104, 86);
            this.hex_UserPreferences.TabIndex = 24;
            this.hex_UserPreferences.Text = "  ";
            this.hex_UserPreferences.Click += new System.EventHandler(this.hex_UserPreferences_Click);
            // 
            // hex_logs
            // 
            this.hex_logs.BackColor = System.Drawing.Color.Transparent;
            this.hex_logs.BackgroundImage = global::GUI_Console.Properties.Resources.GE_BTN17L_reports_STD;
            this.hex_logs.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.hex_logs.Check = false;
            this.hex_logs.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hex_logs.ForeColor = System.Drawing.Color.White;
            this.hex_logs.Location = new System.Drawing.Point(53, 324);
            this.hex_logs.Name = "hex_logs";
            this.hex_logs.Size = new System.Drawing.Size(104, 86);
            this.hex_logs.TabIndex = 22;
            this.hex_logs.Text = "  ";
            this.hex_logs.Click += new System.EventHandler(this.hex_logs_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = global::GUI_Console.Properties.Resources.RoboSep_Logo;
            this.pictureBox1.Location = new System.Drawing.Point(220, 66);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(190, 39);
            this.pictureBox1.TabIndex = 21;
            this.pictureBox1.TabStop = false;
            // 
            // hex_shutdown
            // 
            this.hex_shutdown.BackColor = System.Drawing.Color.Transparent;
            this.hex_shutdown.BackgroundImage = global::GUI_Console.Properties.Resources.GE_BTN19L_shut_down_STD;
            this.hex_shutdown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.hex_shutdown.Check = false;
            this.hex_shutdown.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hex_shutdown.ForeColor = System.Drawing.Color.White;
            this.hex_shutdown.Location = new System.Drawing.Point(474, 324);
            this.hex_shutdown.Name = "hex_shutdown";
            this.hex_shutdown.Size = new System.Drawing.Size(104, 86);
            this.hex_shutdown.TabIndex = 13;
            this.hex_shutdown.Text = "  ";
            this.hex_shutdown.Click += new System.EventHandler(this.hex_shutdown_Click);
            // 
            // hex_help
            // 
            this.hex_help.BackColor = System.Drawing.Color.Transparent;
            this.hex_help.BackgroundImage = global::GUI_Console.Properties.Resources.GE_BTN18L_help_STD;
            this.hex_help.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.hex_help.Check = false;
            this.hex_help.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hex_help.ForeColor = System.Drawing.Color.White;
            this.hex_help.Location = new System.Drawing.Point(328, 324);
            this.hex_help.Name = "hex_help";
            this.hex_help.Size = new System.Drawing.Size(104, 86);
            this.hex_help.TabIndex = 12;
            this.hex_help.Text = "  ";
            this.hex_help.Click += new System.EventHandler(this.hex_help_Click);
            // 
            // hex_system
            // 
            this.hex_system.BackColor = System.Drawing.Color.Transparent;
            this.hex_system.BackgroundImage = global::GUI_Console.Properties.Resources.GE_BTN16L_settings_STD;
            this.hex_system.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.hex_system.Check = false;
            this.hex_system.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hex_system.ForeColor = System.Drawing.Color.White;
            this.hex_system.Location = new System.Drawing.Point(190, 324);
            this.hex_system.Name = "hex_system";
            this.hex_system.Size = new System.Drawing.Size(104, 86);
            this.hex_system.TabIndex = 10;
            this.hex_system.Text = "  ";
            this.hex_system.Click += new System.EventHandler(this.hex_system_Click);
            // 
            // hex_sampling
            // 
            this.hex_sampling.BackColor = System.Drawing.Color.Transparent;
            this.hex_sampling.BackgroundImage = global::GUI_Console.Properties.Resources.carousel_104x86_STD;
            this.hex_sampling.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.hex_sampling.Check = false;
            this.hex_sampling.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hex_sampling.ForeColor = System.Drawing.Color.White;
            this.hex_sampling.Location = new System.Drawing.Point(53, 148);
            this.hex_sampling.Name = "hex_sampling";
            this.hex_sampling.Size = new System.Drawing.Size(104, 86);
            this.hex_sampling.TabIndex = 9;
            this.hex_sampling.Text = "  ";
            this.hex_sampling.Click += new System.EventHandler(this.hex_sampling_Click);
            // 
            // RoboSep_Home
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this.Controls.Add(this.label_users);
            this.Controls.Add(this.hex_Users);
            this.Controls.Add(this.label_Protocols);
            this.Controls.Add(this.hex_protocols);
            this.Controls.Add(this.label_Preferences);
            this.Controls.Add(this.hex_UserPreferences);
            this.Controls.Add(this.label_Reports);
            this.Controls.Add(this.hex_logs);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label_Shutdown);
            this.Controls.Add(this.label_Help);
            this.Controls.Add(this.label_Settings);
            this.Controls.Add(this.label_runsamples);
            this.Controls.Add(this.hex_shutdown);
            this.Controls.Add(this.hex_help);
            this.Controls.Add(this.hex_system);
            this.Controls.Add(this.hex_sampling);
            this.Controls.Add(this.button_UserSelect);
            this.Name = "RoboSep_Home";
            this.Size = new System.Drawing.Size(640, 480);
            this.Load += new System.EventHandler(this.Form_Home_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_UserSelect;
        private GUI_Controls.Button_Circle hex_sampling;
        private GUI_Controls.Button_Circle hex_system;
        private GUI_Controls.Button_Circle hex_help;
        private GUI_Controls.Button_Circle hex_shutdown;
        private System.Windows.Forms.Label label_runsamples;
        private System.Windows.Forms.Label label_Settings;
        private System.Windows.Forms.Label label_Help;
        private System.Windows.Forms.Label label_Shutdown;
        private System.Windows.Forms.PictureBox pictureBox1;
        private GUI_Controls.Button_Circle hex_logs;
        private System.Windows.Forms.Label label_Reports;
        private GUI_Controls.Button_Circle hex_UserPreferences;
        private System.Windows.Forms.Label label_Preferences;
        private GUI_Controls.Button_Circle hex_protocols;
        private System.Windows.Forms.Label label_Protocols;
        private GUI_Controls.Button_Circle hex_Users;
        private System.Windows.Forms.Label label_users;
    }
}