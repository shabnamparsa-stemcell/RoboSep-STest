namespace GUI_Console
{
    partial class RoboSep_ServiceLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoboSep_ServiceLogin));
            this.label4 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textBox_servicePassword = new System.Windows.Forms.TextBox();
            this.shapeContainer2 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.rectangleShape2 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.label_superUser = new System.Windows.Forms.Label();
            this.label_maintenanceUser = new System.Windows.Forms.Label();
            this.label_standardUser = new System.Windows.Forms.Label();
            this.checkBox_superUser = new GUI_Controls.CheckBox();
            this.checkBox_maintenanceUser = new GUI_Controls.CheckBox();
            this.checkBox_standardUser = new GUI_Controls.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBox_UserName = new System.Windows.Forms.TextBox();
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.rectangleShape1 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.button_Login = new GUI_Controls.Button_Rectangle();
            this.roboPanel2 = new GUI_Controls.RoboPanel();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.roboPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.label4.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 16F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.label4.Location = new System.Drawing.Point(48, 272);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 26);
            this.label4.TabIndex = 23;
            this.label4.Text = "Password:";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.textBox_servicePassword);
            this.panel2.Controls.Add(this.shapeContainer2);
            this.panel2.Location = new System.Drawing.Point(53, 301);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(190, 49);
            this.panel2.TabIndex = 22;
            // 
            // textBox_servicePassword
            // 
            this.textBox_servicePassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_servicePassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.textBox_servicePassword.Location = new System.Drawing.Point(1, 7);
            this.textBox_servicePassword.Name = "textBox_servicePassword";
            this.textBox_servicePassword.PasswordChar = '*';
            this.textBox_servicePassword.Size = new System.Drawing.Size(185, 30);
            this.textBox_servicePassword.TabIndex = 3;
            this.textBox_servicePassword.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_servicePassword.MouseClick += new System.Windows.Forms.MouseEventHandler(this.textBox_servicePassword_MouseClick);
            this.textBox_servicePassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_servicePassword_KeyDown);
            // 
            // shapeContainer2
            // 
            this.shapeContainer2.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer2.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer2.Name = "shapeContainer2";
            this.shapeContainer2.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.rectangleShape2});
            this.shapeContainer2.Size = new System.Drawing.Size(190, 49);
            this.shapeContainer2.TabIndex = 3;
            this.shapeContainer2.TabStop = false;
            // 
            // rectangleShape2
            // 
            this.rectangleShape2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.rectangleShape2.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.rectangleShape2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.rectangleShape2.Location = new System.Drawing.Point(2, 9);
            this.rectangleShape2.Name = "rectangleShape1";
            this.rectangleShape2.Size = new System.Drawing.Size(185, 30);
            // 
            // label_superUser
            // 
            this.label_superUser.AutoSize = true;
            this.label_superUser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.label_superUser.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 16F, System.Drawing.FontStyle.Bold);
            this.label_superUser.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.label_superUser.Location = new System.Drawing.Point(86, 147);
            this.label_superUser.Name = "label_superUser";
            this.label_superUser.Size = new System.Drawing.Size(91, 26);
            this.label_superUser.TabIndex = 19;
            this.label_superUser.Text = "Super User";
            this.label_superUser.Click += new System.EventHandler(this.label_superUser_Click);
            // 
            // label_maintenanceUser
            // 
            this.label_maintenanceUser.AutoSize = true;
            this.label_maintenanceUser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.label_maintenanceUser.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 16F, System.Drawing.FontStyle.Bold);
            this.label_maintenanceUser.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.label_maintenanceUser.Location = new System.Drawing.Point(86, 100);
            this.label_maintenanceUser.Name = "label_maintenanceUser";
            this.label_maintenanceUser.Size = new System.Drawing.Size(135, 26);
            this.label_maintenanceUser.TabIndex = 18;
            this.label_maintenanceUser.Text = "Maintenance User";
            this.label_maintenanceUser.Click += new System.EventHandler(this.label_maintenanceUser_Click);
            // 
            // label_standardUser
            // 
            this.label_standardUser.AutoSize = true;
            this.label_standardUser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.label_standardUser.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 16F, System.Drawing.FontStyle.Bold);
            this.label_standardUser.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.label_standardUser.Location = new System.Drawing.Point(86, 53);
            this.label_standardUser.Name = "label_standardUser";
            this.label_standardUser.Size = new System.Drawing.Size(112, 26);
            this.label_standardUser.TabIndex = 17;
            this.label_standardUser.Text = "Standard User";
            this.label_standardUser.Click += new System.EventHandler(this.label_standardUser_Click);
            // 
            // checkBox_superUser
            // 
            this.checkBox_superUser.AccessibleName = "  ";
            this.checkBox_superUser.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("checkBox_superUser.BackgroundImage")));
            this.checkBox_superUser.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.checkBox_superUser.Check = false;
            this.checkBox_superUser.Location = new System.Drawing.Point(28, 137);
            this.checkBox_superUser.Name = "checkBox_superUser";
            this.checkBox_superUser.Size = new System.Drawing.Size(35, 41);
            this.checkBox_superUser.TabIndex = 12;
            this.checkBox_superUser.Click += new System.EventHandler(this.checkBox_superUser_Click);
            // 
            // checkBox_maintenanceUser
            // 
            this.checkBox_maintenanceUser.AccessibleName = "  ";
            this.checkBox_maintenanceUser.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("checkBox_maintenanceUser.BackgroundImage")));
            this.checkBox_maintenanceUser.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.checkBox_maintenanceUser.Check = false;
            this.checkBox_maintenanceUser.Location = new System.Drawing.Point(28, 90);
            this.checkBox_maintenanceUser.Name = "checkBox_maintenanceUser";
            this.checkBox_maintenanceUser.Size = new System.Drawing.Size(35, 41);
            this.checkBox_maintenanceUser.TabIndex = 11;
            this.checkBox_maintenanceUser.Click += new System.EventHandler(this.checkBox_maintenanceUser_Click);
            // 
            // checkBox_standardUser
            // 
            this.checkBox_standardUser.AccessibleName = "  ";
            this.checkBox_standardUser.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("checkBox_standardUser.BackgroundImage")));
            this.checkBox_standardUser.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.checkBox_standardUser.Check = true;
            this.checkBox_standardUser.Location = new System.Drawing.Point(28, 43);
            this.checkBox_standardUser.Name = "checkBox_standardUser";
            this.checkBox_standardUser.Size = new System.Drawing.Size(35, 41);
            this.checkBox_standardUser.TabIndex = 2;
            this.checkBox_standardUser.Click += new System.EventHandler(this.checkBox_standardUser_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.label3.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 16F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.label3.Location = new System.Drawing.Point(48, 190);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 26);
            this.label3.TabIndex = 21;
            this.label3.Text = "User Name:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textBox_UserName);
            this.panel1.Controls.Add(this.shapeContainer1);
            this.panel1.Location = new System.Drawing.Point(53, 219);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(190, 49);
            this.panel1.TabIndex = 20;
            // 
            // textBox_UserName
            // 
            this.textBox_UserName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_UserName.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.textBox_UserName.Location = new System.Drawing.Point(1, 7);
            this.textBox_UserName.Name = "textBox_UserName";
            this.textBox_UserName.Size = new System.Drawing.Size(185, 30);
            this.textBox_UserName.TabIndex = 2;
            this.textBox_UserName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_UserName.MouseClick += new System.Windows.Forms.MouseEventHandler(this.textBox_UserName_MouseClick);
            // 
            // shapeContainer1
            // 
            this.shapeContainer1.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer1.Name = "shapeContainer1";
            this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.rectangleShape1});
            this.shapeContainer1.Size = new System.Drawing.Size(190, 49);
            this.shapeContainer1.TabIndex = 3;
            this.shapeContainer1.TabStop = false;
            // 
            // rectangleShape1
            // 
            this.rectangleShape1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.rectangleShape1.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.rectangleShape1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.rectangleShape1.Location = new System.Drawing.Point(2, 9);
            this.rectangleShape1.Name = "rectangleShape1";
            this.rectangleShape1.Size = new System.Drawing.Size(185, 30);
            // 
            // button_Login
            // 
            this.button_Login.AccessibleName = "Log In";
            this.button_Login.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Login.BackgroundImage")));
            this.button_Login.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Login.Check = false;
            this.button_Login.Location = new System.Drawing.Point(105, 355);
            this.button_Login.Name = "button_Login";
            this.button_Login.Size = new System.Drawing.Size(86, 32);
            this.button_Login.TabIndex = 4;
            this.button_Login.Click += new System.EventHandler(this.button_Login_Click);
            // 
            // roboPanel2
            // 
            this.roboPanel2.AccessibleName = "Service Login";
            this.roboPanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.roboPanel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.roboPanel2.Controls.Add(this.checkBox_standardUser);
            this.roboPanel2.Controls.Add(this.panel1);
            this.roboPanel2.Controls.Add(this.label3);
            this.roboPanel2.Controls.Add(this.checkBox_maintenanceUser);
            this.roboPanel2.Controls.Add(this.checkBox_superUser);
            this.roboPanel2.Controls.Add(this.label4);
            this.roboPanel2.Controls.Add(this.button_Login);
            this.roboPanel2.Controls.Add(this.label_standardUser);
            this.roboPanel2.Controls.Add(this.label_maintenanceUser);
            this.roboPanel2.Controls.Add(this.panel2);
            this.roboPanel2.Controls.Add(this.label_superUser);
            this.roboPanel2.Location = new System.Drawing.Point(184, 40);
            this.roboPanel2.Name = "roboPanel2";
            this.roboPanel2.Size = new System.Drawing.Size(293, 402);
            this.roboPanel2.TabIndex = 2;
            // 
            // RoboSep_ServiceLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.roboPanel2);
            this.Name = "RoboSep_ServiceLogin";
            this.Load += new System.EventHandler(this.RoboSep_ServiceLogin_Load);
            this.Controls.SetChildIndex(this.roboPanel2, 0);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.roboPanel2.ResumeLayout(false);
            this.roboPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private GUI_Controls.CheckBox checkBox_standardUser;
        private GUI_Controls.CheckBox checkBox_superUser;
        private GUI_Controls.CheckBox checkBox_maintenanceUser;
        private System.Windows.Forms.TextBox textBox_UserName;
        private System.Windows.Forms.Label label_superUser;
        private System.Windows.Forms.Label label_maintenanceUser;
        private System.Windows.Forms.Label label_standardUser;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox textBox_servicePassword;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer2;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape2;
        private System.Windows.Forms.Label label3;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape1;
        private GUI_Controls.Button_Rectangle button_Login;
        private GUI_Controls.RoboPanel roboPanel2;
    }
}
