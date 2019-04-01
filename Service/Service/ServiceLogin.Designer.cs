namespace Tesla.Service
{
    partial class ServiceLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServiceLogin));
            this.roboPanel2 = new GUI_Controls.RoboPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.button_Cancel = new GUI_Controls.Button_Rectangle();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox_standardUser = new GUI_Controls.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBox_UserName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBox_maintenanceUser = new GUI_Controls.CheckBox();
            this.checkBox_superUser = new GUI_Controls.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label_standardUser = new System.Windows.Forms.Label();
            this.label_maintenanceUser = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textBox_servicePassword = new System.Windows.Forms.TextBox();
            this.label_superUser = new System.Windows.Forms.Label();
            this.button_Login = new GUI_Controls.Button_Rectangle();
            this.roboPanel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // roboPanel2
            // 
            this.roboPanel2.AccessibleName = "Service Login";
            this.roboPanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.roboPanel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.roboPanel2.Controls.Add(this.button1);
            this.roboPanel2.Controls.Add(this.button_Cancel);
            this.roboPanel2.Controls.Add(this.label1);
            this.roboPanel2.Controls.Add(this.checkBox_standardUser);
            this.roboPanel2.Controls.Add(this.panel1);
            this.roboPanel2.Controls.Add(this.label4);
            this.roboPanel2.Controls.Add(this.checkBox_maintenanceUser);
            this.roboPanel2.Controls.Add(this.checkBox_superUser);
            this.roboPanel2.Controls.Add(this.label5);
            this.roboPanel2.Controls.Add(this.label_standardUser);
            this.roboPanel2.Controls.Add(this.label_maintenanceUser);
            this.roboPanel2.Controls.Add(this.panel2);
            this.roboPanel2.Controls.Add(this.label_superUser);
            this.roboPanel2.Controls.Add(this.button_Login);
            this.roboPanel2.Location = new System.Drawing.Point(0, 0);
            this.roboPanel2.Name = "roboPanel2";
            this.roboPanel2.Size = new System.Drawing.Size(293, 402);
            this.roboPanel2.TabIndex = 12;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(239, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(37, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "gen";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button_Cancel
            // 
            this.button_Cancel.AccessibleName = "Cancel";
            this.button_Cancel.BackColor = System.Drawing.Color.Transparent;
            this.button_Cancel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Cancel.BackgroundImage")));
            this.button_Cancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Cancel.Check = false;
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button_Cancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.button_Cancel.Location = new System.Drawing.Point(91, 362);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(86, 32);
            this.button_Cancel.TabIndex = 24;
            this.button_Cancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(95)))), ((int)(((byte)(95)))));
            this.label1.Location = new System.Drawing.Point(12, 378);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 15);
            this.label1.TabIndex = 25;
            this.label1.Text = "v4.6.0.11";
            // 
            // checkBox_standardUser
            // 
            this.checkBox_standardUser.AccessibleName = "  ";
            this.checkBox_standardUser.BackColor = System.Drawing.Color.Transparent;
            this.checkBox_standardUser.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("checkBox_standardUser.BackgroundImage")));
            this.checkBox_standardUser.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.checkBox_standardUser.Check = true;
            this.checkBox_standardUser.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.checkBox_standardUser.Location = new System.Drawing.Point(34, 52);
            this.checkBox_standardUser.Name = "checkBox_standardUser";
            this.checkBox_standardUser.Size = new System.Drawing.Size(35, 41);
            this.checkBox_standardUser.TabIndex = 2;
            this.checkBox_standardUser.Click += new System.EventHandler(this.checkBox_standardUser_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.textBox_UserName);
            this.panel1.Location = new System.Drawing.Point(53, 219);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(190, 49);
            this.panel1.TabIndex = 20;
            // 
            // textBox_UserName
            // 
            this.textBox_UserName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_UserName.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.textBox_UserName.Location = new System.Drawing.Point(2, 7);
            this.textBox_UserName.Name = "textBox_UserName";
            this.textBox_UserName.Size = new System.Drawing.Size(185, 30);
            this.textBox_UserName.TabIndex = 2;
            this.textBox_UserName.Text = "stemcell";
            this.textBox_UserName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_UserName.MouseClick += new System.Windows.Forms.MouseEventHandler(this.textBox_UserName_MouseClick);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(95)))), ((int)(((byte)(95)))));
            this.label4.Location = new System.Drawing.Point(48, 190);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(125, 25);
            this.label4.TabIndex = 21;
            this.label4.Text = "User Name:";
            // 
            // checkBox_maintenanceUser
            // 
            this.checkBox_maintenanceUser.AccessibleName = "  ";
            this.checkBox_maintenanceUser.BackColor = System.Drawing.Color.Transparent;
            this.checkBox_maintenanceUser.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("checkBox_maintenanceUser.BackgroundImage")));
            this.checkBox_maintenanceUser.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.checkBox_maintenanceUser.Check = false;
            this.checkBox_maintenanceUser.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.checkBox_maintenanceUser.Location = new System.Drawing.Point(34, 99);
            this.checkBox_maintenanceUser.Name = "checkBox_maintenanceUser";
            this.checkBox_maintenanceUser.Size = new System.Drawing.Size(35, 41);
            this.checkBox_maintenanceUser.TabIndex = 11;
            this.checkBox_maintenanceUser.Click += new System.EventHandler(this.checkBox_maintenanceUser_Click);
            // 
            // checkBox_superUser
            // 
            this.checkBox_superUser.AccessibleName = "  ";
            this.checkBox_superUser.BackColor = System.Drawing.Color.Transparent;
            this.checkBox_superUser.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("checkBox_superUser.BackgroundImage")));
            this.checkBox_superUser.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.checkBox_superUser.Check = false;
            this.checkBox_superUser.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.checkBox_superUser.Location = new System.Drawing.Point(34, 146);
            this.checkBox_superUser.Name = "checkBox_superUser";
            this.checkBox_superUser.Size = new System.Drawing.Size(35, 41);
            this.checkBox_superUser.TabIndex = 12;
            this.checkBox_superUser.Click += new System.EventHandler(this.checkBox_superUser_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(95)))), ((int)(((byte)(95)))));
            this.label5.Location = new System.Drawing.Point(48, 272);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(109, 25);
            this.label5.TabIndex = 23;
            this.label5.Text = "Password:";
            // 
            // label_standardUser
            // 
            this.label_standardUser.AutoSize = true;
            this.label_standardUser.BackColor = System.Drawing.Color.Transparent;
            this.label_standardUser.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_standardUser.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(95)))), ((int)(((byte)(95)))));
            this.label_standardUser.Location = new System.Drawing.Point(86, 53);
            this.label_standardUser.Name = "label_standardUser";
            this.label_standardUser.Size = new System.Drawing.Size(150, 25);
            this.label_standardUser.TabIndex = 17;
            this.label_standardUser.Text = "Standard User";
            this.label_standardUser.Click += new System.EventHandler(this.label_standardUser_Click);
            // 
            // label_maintenanceUser
            // 
            this.label_maintenanceUser.AutoSize = true;
            this.label_maintenanceUser.BackColor = System.Drawing.Color.Transparent;
            this.label_maintenanceUser.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_maintenanceUser.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(95)))), ((int)(((byte)(95)))));
            this.label_maintenanceUser.Location = new System.Drawing.Point(86, 100);
            this.label_maintenanceUser.Name = "label_maintenanceUser";
            this.label_maintenanceUser.Size = new System.Drawing.Size(191, 25);
            this.label_maintenanceUser.TabIndex = 18;
            this.label_maintenanceUser.Text = "Maintenance User";
            this.label_maintenanceUser.Click += new System.EventHandler(this.label_maintenanceUser_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this.textBox_servicePassword);
            this.panel2.Location = new System.Drawing.Point(53, 301);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(190, 49);
            this.panel2.TabIndex = 22;
            this.panel2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.textBox_servicePassword_MouseClick);
            // 
            // textBox_servicePassword
            // 
            this.textBox_servicePassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_servicePassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.textBox_servicePassword.Location = new System.Drawing.Point(2, 7);
            this.textBox_servicePassword.Name = "textBox_servicePassword";
            this.textBox_servicePassword.PasswordChar = '*';
            this.textBox_servicePassword.Size = new System.Drawing.Size(185, 30);
            this.textBox_servicePassword.TabIndex = 3;
            this.textBox_servicePassword.Text = "stemcell";
            this.textBox_servicePassword.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_servicePassword.MouseClick += new System.Windows.Forms.MouseEventHandler(this.textBox_servicePassword_MouseClick);
            this.textBox_servicePassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_servicePassword_KeyDown);
            // 
            // label_superUser
            // 
            this.label_superUser.AutoSize = true;
            this.label_superUser.BackColor = System.Drawing.Color.Transparent;
            this.label_superUser.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_superUser.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(95)))), ((int)(((byte)(95)))));
            this.label_superUser.Location = new System.Drawing.Point(86, 147);
            this.label_superUser.Name = "label_superUser";
            this.label_superUser.Size = new System.Drawing.Size(117, 25);
            this.label_superUser.TabIndex = 19;
            this.label_superUser.Text = "Super User";
            this.label_superUser.Click += new System.EventHandler(this.label_superUser_Click);
            // 
            // button_Login
            // 
            this.button_Login.AccessibleName = "Log In";
            this.button_Login.BackColor = System.Drawing.Color.Transparent;
            this.button_Login.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Login.BackgroundImage")));
            this.button_Login.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Login.Check = false;
            this.button_Login.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button_Login.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.button_Login.Location = new System.Drawing.Point(195, 362);
            this.button_Login.Name = "button_Login";
            this.button_Login.Size = new System.Drawing.Size(86, 32);
            this.button_Login.TabIndex = 4;
            this.button_Login.Click += new System.EventHandler(this.button_Login_Click);
            // 
            // ServiceLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(293, 402);
            this.ControlBox = false;
            this.Controls.Add(this.roboPanel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ServiceLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ServiceLogin";
            this.Load += new System.EventHandler(this.ServiceLogin_Load);
            this.roboPanel2.ResumeLayout(false);
            this.roboPanel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private GUI_Controls.RoboPanel roboPanel2;
        private GUI_Controls.CheckBox checkBox_standardUser;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBox_UserName;
        private System.Windows.Forms.Label label4;
        private GUI_Controls.CheckBox checkBox_maintenanceUser;
        private GUI_Controls.CheckBox checkBox_superUser;
        private System.Windows.Forms.Label label5;
        private GUI_Controls.Button_Rectangle button_Login;
        private System.Windows.Forms.Label label_standardUser;
        private System.Windows.Forms.Label label_maintenanceUser;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox textBox_servicePassword;
        private System.Windows.Forms.Label label_superUser;
        private GUI_Controls.Button_Rectangle button_Cancel;
        private System.Windows.Forms.Label label1;
    }
}