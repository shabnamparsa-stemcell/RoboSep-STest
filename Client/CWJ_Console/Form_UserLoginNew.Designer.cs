namespace GUI_Console
{
    partial class Form_UserLoginNew
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
            this.label_User = new System.Windows.Forms.Label();
            this.textBox_NewUserLoginID = new System.Windows.Forms.TextBox();
            this.lineShape1 = new Microsoft.VisualBasic.PowerPacks.LineShape();
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.lineShape3 = new Microsoft.VisualBasic.PowerPacks.LineShape();
            this.lineShape2 = new Microsoft.VisualBasic.PowerPacks.LineShape();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox_User = new System.Windows.Forms.PictureBox();
            this.button_Cancel = new GUI_Controls.Button_Cancel();
            this.button_User_Settings = new GUI_Controls.Button_Circle();
            this.button_Save = new GUI_Controls.Button_Circle();
            this.button_LoadImageHD = new GUI_Controls.Button_Circle();
            this.button_LoadImageUSB = new GUI_Controls.Button_Circle();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_User)).BeginInit();
            this.SuspendLayout();
            // 
            // label_User
            // 
            this.label_User.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label_User.BackColor = System.Drawing.Color.Transparent;
            this.label_User.Font = new System.Drawing.Font("Arial Narrow", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_User.Location = new System.Drawing.Point(44, 79);
            this.label_User.Name = "label_User";
            this.label_User.Size = new System.Drawing.Size(115, 31);
            this.label_User.TabIndex = 69;
            this.label_User.Text = "User profile";
            // 
            // textBox_NewUserLoginID
            // 
            this.textBox_NewUserLoginID.Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_NewUserLoginID.Location = new System.Drawing.Point(174, 79);
            this.textBox_NewUserLoginID.Name = "textBox_NewUserLoginID";
            this.textBox_NewUserLoginID.Size = new System.Drawing.Size(247, 29);
            this.textBox_NewUserLoginID.TabIndex = 70;
            this.textBox_NewUserLoginID.TextChanged += new System.EventHandler(this.textBox_NewUserLoginID_TextChanged);
            // 
            // lineShape1
            // 
            this.lineShape1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(146)))), ((int)(((byte)(108)))), ((int)(((byte)(175)))));
            this.lineShape1.BorderWidth = 3;
            this.lineShape1.Name = "lineShape1";
            this.lineShape1.X1 = 3;
            this.lineShape1.X2 = 643;
            this.lineShape1.Y1 = 363;
            this.lineShape1.Y2 = 363;
            // 
            // shapeContainer1
            // 
            this.shapeContainer1.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer1.Name = "shapeContainer1";
            this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.lineShape3,
            this.lineShape2,
            this.lineShape1});
            this.shapeContainer1.Size = new System.Drawing.Size(640, 480);
            this.shapeContainer1.TabIndex = 79;
            this.shapeContainer1.TabStop = false;
            // 
            // lineShape3
            // 
            this.lineShape3.Name = "lineShape3";
            this.lineShape3.Visible = false;
            this.lineShape3.X1 = 157;
            this.lineShape3.X2 = 246;
            this.lineShape3.Y1 = 301;
            this.lineShape3.Y2 = 301;
            // 
            // lineShape2
            // 
            this.lineShape2.Name = "lineShape2";
            this.lineShape2.Visible = false;
            this.lineShape2.X1 = 160;
            this.lineShape2.X2 = 249;
            this.lineShape2.Y1 = 158;
            this.lineShape2.Y2 = 158;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.BackgroundImage = global::GUI_Console.Properties.Resources.logo_mainscreen;
            this.pictureBox2.Location = new System.Drawing.Point(42, 20);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(172, 32);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox2.TabIndex = 72;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox_User
            // 
            this.pictureBox_User.Image = global::GUI_Console.Properties.Resources.DefaultUserPlus_PURPLE;
            this.pictureBox_User.Location = new System.Drawing.Point(49, 157);
            this.pictureBox_User.Name = "pictureBox_User";
            this.pictureBox_User.Size = new System.Drawing.Size(110, 146);
            this.pictureBox_User.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_User.TabIndex = 71;
            this.pictureBox_User.TabStop = false;
            this.pictureBox_User.Visible = false;
            this.pictureBox_User.Click += new System.EventHandler(this.pictureBox_User_Click);
            // 
            // button_Cancel
            // 
            this.button_Cancel.BackColor = System.Drawing.Color.Transparent;
            this.button_Cancel.BackgroundImage = global::GUI_Console.Properties.Resources.L_104x86_single_arrow_left_STD;
            this.button_Cancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Cancel.Check = false;
            this.button_Cancel.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Cancel.ForeColor = System.Drawing.Color.White;
            this.button_Cancel.Location = new System.Drawing.Point(415, 389);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(104, 86);
            this.button_Cancel.TabIndex = 78;
            this.button_Cancel.Text = "Cancel";
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // button_User_Settings
            // 
            this.button_User_Settings.BackColor = System.Drawing.Color.Transparent;
            this.button_User_Settings.BackgroundImage = global::GUI_Console.Properties.Resources.GE_BTN14M_preferences_STD;
            this.button_User_Settings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_User_Settings.Check = false;
            this.button_User_Settings.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_User_Settings.ForeColor = System.Drawing.Color.White;
            this.button_User_Settings.Location = new System.Drawing.Point(9, 405);
            this.button_User_Settings.Name = "button_User_Settings";
            this.button_User_Settings.Size = new System.Drawing.Size(52, 52);
            this.button_User_Settings.TabIndex = 76;
            this.button_User_Settings.Text = "User Settings";
            this.button_User_Settings.Visible = false;
            this.button_User_Settings.Click += new System.EventHandler(this.button_User_Settings_Click);
            // 
            // button_Save
            // 
            this.button_Save.BackColor = System.Drawing.Color.Transparent;
            this.button_Save.BackgroundImage = global::GUI_Console.Properties.Resources.GE_BTN10L_save_STD;
            this.button_Save.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Save.Check = false;
            this.button_Save.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Save.ForeColor = System.Drawing.Color.White;
            this.button_Save.Location = new System.Drawing.Point(528, 389);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(104, 86);
            this.button_Save.TabIndex = 75;
            this.button_Save.Text = " ";
            this.button_Save.Click += new System.EventHandler(this.button_Save_Click);
            // 
            // button_LoadImageHD
            // 
            this.button_LoadImageHD.BackColor = System.Drawing.Color.Transparent;
            this.button_LoadImageHD.BackgroundImage = global::GUI_Console.Properties.Resources.US_BTN04L_hard_drive_STD;
            this.button_LoadImageHD.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_LoadImageHD.Check = false;
            this.button_LoadImageHD.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_LoadImageHD.ForeColor = System.Drawing.Color.White;
            this.button_LoadImageHD.Location = new System.Drawing.Point(245, 258);
            this.button_LoadImageHD.Name = "button_LoadImageHD";
            this.button_LoadImageHD.Size = new System.Drawing.Size(104, 86);
            this.button_LoadImageHD.TabIndex = 74;
            this.button_LoadImageHD.Text = "Hard Drive";
            this.button_LoadImageHD.Visible = false;
            this.button_LoadImageHD.Click += new System.EventHandler(this.button_LoadImageHD_Click);
            // 
            // button_LoadImageUSB
            // 
            this.button_LoadImageUSB.BackColor = System.Drawing.Color.Transparent;
            this.button_LoadImageUSB.BackgroundImage = global::GUI_Console.Properties.Resources.US_BTN04L_usb_STD;
            this.button_LoadImageUSB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_LoadImageUSB.Check = false;
            this.button_LoadImageUSB.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_LoadImageUSB.ForeColor = System.Drawing.Color.White;
            this.button_LoadImageUSB.Location = new System.Drawing.Point(243, 116);
            this.button_LoadImageUSB.Name = "button_LoadImageUSB";
            this.button_LoadImageUSB.Size = new System.Drawing.Size(104, 86);
            this.button_LoadImageUSB.TabIndex = 73;
            this.button_LoadImageUSB.Text = "USB";
            this.button_LoadImageUSB.Visible = false;
            this.button_LoadImageUSB.Click += new System.EventHandler(this.button_LoadImageUSB_Click);
            // 
            // Form_UserLoginNew
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this.ClientSize = new System.Drawing.Size(640, 480);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_User_Settings);
            this.Controls.Add(this.button_Save);
            this.Controls.Add(this.button_LoadImageHD);
            this.Controls.Add(this.button_LoadImageUSB);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label_User);
            this.Controls.Add(this.textBox_NewUserLoginID);
            this.Controls.Add(this.pictureBox_User);
            this.Controls.Add(this.shapeContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form_UserLoginNew";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Form_UserLoginNew";
            this.Deactivate += new System.EventHandler(this.Form_UserLoginNew_Deactivate);
            this.Load += new System.EventHandler(this.Form_UserLoginNew_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_User)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GUI_Controls.Button_Cancel button_Cancel;
        private GUI_Controls.Button_Circle button_User_Settings;
        private GUI_Controls.Button_Circle button_Save;
        private GUI_Controls.Button_Circle button_LoadImageHD;
        private GUI_Controls.Button_Circle button_LoadImageUSB;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label_User;
        private System.Windows.Forms.TextBox textBox_NewUserLoginID;
        private System.Windows.Forms.PictureBox pictureBox_User;
        private Microsoft.VisualBasic.PowerPacks.LineShape lineShape1;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private Microsoft.VisualBasic.PowerPacks.LineShape lineShape3;
        private Microsoft.VisualBasic.PowerPacks.LineShape lineShape2;
    }
}