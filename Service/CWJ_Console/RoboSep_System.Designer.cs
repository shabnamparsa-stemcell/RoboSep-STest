namespace GUI_Console
{
    partial class RoboSep_System
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoboSep_System));
            this.Button_BasicPrime = new GUI_Controls.Button_Rectangle();
            this.label_liquidSensor = new System.Windows.Forms.Label();
            this.label_lidSensor = new System.Windows.Forms.Label();
            this.checkBox_liquidSensor = new GUI_Controls.CheckBox();
            this.checkBox_lidSensor = new GUI_Controls.CheckBox();
            this.checkBox_keyboard = new GUI_Controls.CheckBox();
            this.label_keyboard = new System.Windows.Forms.Label();
            this.label_serviceButton = new System.Windows.Forms.Label();
            this.button_Service = new GUI_Controls.Button_SmallPink();
            this.button_Shutdown = new GUI_Controls.Button_SmallPink();
            this.label1 = new System.Windows.Forms.Label();
            this.roboPanel_SystemOptions = new GUI_Controls.RoboPanel();
            this.roboPanel_Maintenance = new GUI_Controls.RoboPanel();
            this.button_Home = new GUI_Controls.Button_Rectangle();
            this.button_Diagnostic = new GUI_Controls.Button_SmallPink();
            this.label2 = new System.Windows.Forms.Label();
            this.roboPanel_SystemOptions.SuspendLayout();
            this.roboPanel_Maintenance.SuspendLayout();
            this.SuspendLayout();
            // 
            // Button_BasicPrime
            // 
            this.Button_BasicPrime.AccessibleName = "Basic Prime";
            this.Button_BasicPrime.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Button_BasicPrime.BackgroundImage")));
            this.Button_BasicPrime.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Button_BasicPrime.Check = false;
            this.Button_BasicPrime.Location = new System.Drawing.Point(15, 64);
            this.Button_BasicPrime.Name = "Button_BasicPrime";
            this.Button_BasicPrime.Size = new System.Drawing.Size(243, 74);
            this.Button_BasicPrime.TabIndex = 1;
            this.Button_BasicPrime.Click += new System.EventHandler(this.Button_BasicPrime_Click);
            // 
            // label_liquidSensor
            // 
            this.label_liquidSensor.AutoSize = true;
            this.label_liquidSensor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.label_liquidSensor.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 16F, System.Drawing.FontStyle.Bold);
            this.label_liquidSensor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(70)))));
            this.label_liquidSensor.Location = new System.Drawing.Point(72, 211);
            this.label_liquidSensor.Name = "label_liquidSensor";
            this.label_liquidSensor.Size = new System.Drawing.Size(141, 52);
            this.label_liquidSensor.TabIndex = 16;
            this.label_liquidSensor.Text = "Enable Liquid Level\nSensor";
            this.label_liquidSensor.Click += new System.EventHandler(this.label_liquidSensor_Click);
            // 
            // label_lidSensor
            // 
            this.label_lidSensor.AutoSize = true;
            this.label_lidSensor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.label_lidSensor.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 16F, System.Drawing.FontStyle.Bold);
            this.label_lidSensor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(70)))));
            this.label_lidSensor.Location = new System.Drawing.Point(72, 148);
            this.label_lidSensor.Name = "label_lidSensor";
            this.label_lidSensor.Size = new System.Drawing.Size(133, 26);
            this.label_lidSensor.TabIndex = 15;
            this.label_lidSensor.Text = "Enable Lid Sensor";
            this.label_lidSensor.Click += new System.EventHandler(this.label_lidSensor_Click);
            // 
            // checkBox_liquidSensor
            // 
            this.checkBox_liquidSensor.AccessibleName = "  ";
            this.checkBox_liquidSensor.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("checkBox_liquidSensor.BackgroundImage")));
            this.checkBox_liquidSensor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.checkBox_liquidSensor.Check = false;
            this.checkBox_liquidSensor.Location = new System.Drawing.Point(19, 214);
            this.checkBox_liquidSensor.Name = "checkBox_liquidSensor";
            this.checkBox_liquidSensor.Size = new System.Drawing.Size(35, 41);
            this.checkBox_liquidSensor.TabIndex = 13;
            this.checkBox_liquidSensor.Click += new System.EventHandler(this.checkBox_liquidSensor_Click);
            // 
            // checkBox_lidSensor
            // 
            this.checkBox_lidSensor.AccessibleName = "  ";
            this.checkBox_lidSensor.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("checkBox_lidSensor.BackgroundImage")));
            this.checkBox_lidSensor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.checkBox_lidSensor.Check = false;
            this.checkBox_lidSensor.Location = new System.Drawing.Point(19, 139);
            this.checkBox_lidSensor.Name = "checkBox_lidSensor";
            this.checkBox_lidSensor.Size = new System.Drawing.Size(35, 41);
            this.checkBox_lidSensor.TabIndex = 12;
            this.checkBox_lidSensor.Click += new System.EventHandler(this.checkBox_lidSensor_Click);
            // 
            // checkBox_keyboard
            // 
            this.checkBox_keyboard.AccessibleName = "  ";
            this.checkBox_keyboard.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("checkBox_keyboard.BackgroundImage")));
            this.checkBox_keyboard.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.checkBox_keyboard.Check = false;
            this.checkBox_keyboard.Location = new System.Drawing.Point(19, 69);
            this.checkBox_keyboard.Name = "checkBox_keyboard";
            this.checkBox_keyboard.Size = new System.Drawing.Size(35, 41);
            this.checkBox_keyboard.TabIndex = 11;
            this.checkBox_keyboard.Click += new System.EventHandler(this.checkBox_keyboard_Click);
            // 
            // label_keyboard
            // 
            this.label_keyboard.AutoSize = true;
            this.label_keyboard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.label_keyboard.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 16F, System.Drawing.FontStyle.Bold);
            this.label_keyboard.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(70)))));
            this.label_keyboard.Location = new System.Drawing.Point(72, 78);
            this.label_keyboard.Name = "label_keyboard";
            this.label_keyboard.Size = new System.Drawing.Size(173, 26);
            this.label_keyboard.TabIndex = 14;
            this.label_keyboard.Text = "Enable Touch Keyboard";
            this.label_keyboard.Click += new System.EventHandler(this.label_keybaord_Click);
            // 
            // label_serviceButton
            // 
            this.label_serviceButton.AutoSize = true;
            this.label_serviceButton.BackColor = System.Drawing.Color.Transparent;
            this.label_serviceButton.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 15F, System.Drawing.FontStyle.Bold);
            this.label_serviceButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(70)))));
            this.label_serviceButton.Location = new System.Drawing.Point(208, 436);
            this.label_serviceButton.Name = "label_serviceButton";
            this.label_serviceButton.Size = new System.Drawing.Size(92, 24);
            this.label_serviceButton.TabIndex = 17;
            this.label_serviceButton.Text = "Service Menu";
            this.label_serviceButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // button_Service
            // 
            this.button_Service.AccessibleName = "  ";
            this.button_Service.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Service.BackgroundImage")));
            this.button_Service.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Service.Check = false;
            this.button_Service.Location = new System.Drawing.Point(206, 336);
            this.button_Service.Name = "button_Service";
            this.button_Service.Size = new System.Drawing.Size(97, 97);
            this.button_Service.TabIndex = 18;
            this.button_Service.Click += new System.EventHandler(this.button_Service_Click);
            // 
            // button_Shutdown
            // 
            this.button_Shutdown.AccessibleName = "  ";
            this.button_Shutdown.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Shutdown.BackgroundImage")));
            this.button_Shutdown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Shutdown.Check = false;
            this.button_Shutdown.Location = new System.Drawing.Point(507, 336);
            this.button_Shutdown.Name = "button_Shutdown";
            this.button_Shutdown.Size = new System.Drawing.Size(97, 97);
            this.button_Shutdown.TabIndex = 19;
            this.button_Shutdown.Click += new System.EventHandler(this.button_Shutdown_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 15F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(70)))));
            this.label1.Location = new System.Drawing.Point(494, 436);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 24);
            this.label1.TabIndex = 20;
            this.label1.Text = "Software Shutdown";
            // 
            // roboPanel_SystemOptions
            // 
            this.roboPanel_SystemOptions.AccessibleName = "System Options";
            this.roboPanel_SystemOptions.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.roboPanel_SystemOptions.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.roboPanel_SystemOptions.Controls.Add(this.checkBox_keyboard);
            this.roboPanel_SystemOptions.Controls.Add(this.label_keyboard);
            this.roboPanel_SystemOptions.Controls.Add(this.checkBox_lidSensor);
            this.roboPanel_SystemOptions.Controls.Add(this.checkBox_liquidSensor);
            this.roboPanel_SystemOptions.Controls.Add(this.label_lidSensor);
            this.roboPanel_SystemOptions.Controls.Add(this.label_liquidSensor);
            this.roboPanel_SystemOptions.Location = new System.Drawing.Point(32, 28);
            this.roboPanel_SystemOptions.Name = "roboPanel_SystemOptions";
            this.roboPanel_SystemOptions.Size = new System.Drawing.Size(271, 298);
            this.roboPanel_SystemOptions.TabIndex = 17;
            // 
            // roboPanel_Maintenance
            // 
            this.roboPanel_Maintenance.AccessibleName = "Maintenance Protocols";
            this.roboPanel_Maintenance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.roboPanel_Maintenance.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.roboPanel_Maintenance.Controls.Add(this.button_Home);
            this.roboPanel_Maintenance.Controls.Add(this.Button_BasicPrime);
            this.roboPanel_Maintenance.Location = new System.Drawing.Point(333, 28);
            this.roboPanel_Maintenance.Name = "roboPanel_Maintenance";
            this.roboPanel_Maintenance.Size = new System.Drawing.Size(271, 298);
            this.roboPanel_Maintenance.TabIndex = 21;
            // 
            // button_Home
            // 
            this.button_Home.AccessibleName = "Home All";
            this.button_Home.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Home.BackgroundImage")));
            this.button_Home.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Home.Check = false;
            this.button_Home.Location = new System.Drawing.Point(15, 148);
            this.button_Home.Name = "button_Home";
            this.button_Home.Size = new System.Drawing.Size(243, 74);
            this.button_Home.TabIndex = 11;
            this.button_Home.Click += new System.EventHandler(this.button_Home_Click);
            // 
            // button_Diagnostic
            // 
            this.button_Diagnostic.AccessibleName = "  ";
            this.button_Diagnostic.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Diagnostic.BackgroundImage")));
            this.button_Diagnostic.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Diagnostic.Check = false;
            this.button_Diagnostic.Location = new System.Drawing.Point(357, 336);
            this.button_Diagnostic.Name = "button_Diagnostic";
            this.button_Diagnostic.Size = new System.Drawing.Size(97, 97);
            this.button_Diagnostic.TabIndex = 22;
            this.button_Diagnostic.Click += new System.EventHandler(this.button_Diagnostic_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 15F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(70)))));
            this.label2.Location = new System.Drawing.Point(344, 436);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(126, 24);
            this.label2.TabIndex = 23;
            this.label2.Text = "Diagnostic Package";
            // 
            // RoboSep_System
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button_Diagnostic);
            this.Controls.Add(this.roboPanel_Maintenance);
            this.Controls.Add(this.button_Shutdown);
            this.Controls.Add(this.button_Service);
            this.Controls.Add(this.label_serviceButton);
            this.Controls.Add(this.roboPanel_SystemOptions);
            this.Controls.Add(this.label1);
            this.Name = "RoboSep_System";
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.roboPanel_SystemOptions, 0);
            this.Controls.SetChildIndex(this.label_serviceButton, 0);
            this.Controls.SetChildIndex(this.button_Service, 0);
            this.Controls.SetChildIndex(this.button_Shutdown, 0);
            this.Controls.SetChildIndex(this.roboPanel_Maintenance, 0);
            this.Controls.SetChildIndex(this.button_Diagnostic, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.roboPanel_SystemOptions.ResumeLayout(false);
            this.roboPanel_SystemOptions.PerformLayout();
            this.roboPanel_Maintenance.ResumeLayout(false);
            this.roboPanel_Maintenance.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GUI_Controls.Button_Rectangle button_HomeAll;
        private GUI_Controls.Button_Rectangle Button_BasicPrime;
        private System.Windows.Forms.Label label_keyboard;
        private GUI_Controls.CheckBox checkBox_liquidSensor;
        private GUI_Controls.CheckBox checkBox_lidSensor;
        private GUI_Controls.CheckBox checkBox_keyboard;
        private System.Windows.Forms.Label label_liquidSensor;
        private System.Windows.Forms.Label label_lidSensor;
        private System.Windows.Forms.Label label_serviceButton;
        private GUI_Controls.Button_SmallPink button_Service;
        private GUI_Controls.Button_SmallPink button_Shutdown;
        private System.Windows.Forms.Label label1;
        private GUI_Controls.RoboPanel roboPanel_SystemOptions;
        private GUI_Controls.RoboPanel roboPanel_Maintenance;
        private GUI_Controls.Button_Rectangle button_Home;
        private GUI_Controls.Button_SmallPink button_Diagnostic;
        private System.Windows.Forms.Label label2;

    }
}
