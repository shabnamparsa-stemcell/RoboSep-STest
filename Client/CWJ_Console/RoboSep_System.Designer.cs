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
                StopWatchUSB();
                components.Dispose();
            }
            int i;
            for (i = 0; i < Ilist.Count; i++)
                Ilist[i].Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoboSep_System));
            this.Button_BasicPrime = new GUI_Controls.Button_Rectangle();
            this.label_liquidSensor = new GUI_Controls.SizableLabel();
            this.label_lidSensor = new GUI_Controls.SizableLabel();
            this.checkBox_liquidSensor = new GUI_Controls.CheckBox_Circle();
            this.checkBox_lidSensor = new GUI_Controls.CheckBox_Circle();
            this.checkBox_keyboard = new GUI_Controls.CheckBox_Circle();
            this.checkBox_StartTimer = new GUI_Controls.CheckBox_Circle();
            this.label_keyboard = new GUI_Controls.SizableLabel();
            this.lblServiceMenu = new System.Windows.Forms.Label();
            this.button_Service = new GUI_Controls.Button_SmallPink();
            this.button_Shutdown = new GUI_Controls.Button_SmallPink();
            this.lblSoftwareShutdown = new System.Windows.Forms.Label();
            this.label_StartTimer = new GUI_Controls.SizableLabel();
            this.button_Home = new GUI_Controls.Button_Rectangle();
            this.button_Diagnostic = new GUI_Controls.Button_SmallPink();
            this.lblDiagnosticPackage = new System.Windows.Forms.Label();
            this.SystemPanel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ExitSoftwareTimer = new System.Windows.Forms.Timer(this.components);
            this.button_Logs = new GUI_Controls.Button_SmallPink();
            this.SystemPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Button_BasicPrime
            // 
            this.Button_BasicPrime.BackColor = System.Drawing.Color.Transparent;
            this.Button_BasicPrime.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Button_BasicPrime.BackgroundImage")));
            this.Button_BasicPrime.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Button_BasicPrime.Check = false;
            this.Button_BasicPrime.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Button_BasicPrime.ForeColor = System.Drawing.Color.White;
            this.Button_BasicPrime.Location = new System.Drawing.Point(40, 79);
            this.Button_BasicPrime.Name = "Button_BasicPrime";
            this.Button_BasicPrime.Size = new System.Drawing.Size(188, 58);
            this.Button_BasicPrime.TabIndex = 1;
            this.Button_BasicPrime.Text = "Basic Prime";
            this.Button_BasicPrime.Click += new System.EventHandler(this.Button_BasicPrime_Click);
            // 
            // label_liquidSensor
            // 
            this.label_liquidSensor.AutoSize = true;
            this.label_liquidSensor.BackColor = System.Drawing.Color.Transparent;
            this.label_liquidSensor.Font = new System.Drawing.Font("Arial Narrow", 14F, System.Drawing.FontStyle.Bold);
            this.label_liquidSensor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.label_liquidSensor.Location = new System.Drawing.Point(64, 167);
            this.label_liquidSensor.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.label_liquidSensor.Name = "label_liquidSensor";
            this.label_liquidSensor.Size = new System.Drawing.Size(190, 40);
            this.label_liquidSensor.TabIndex = 16;
            this.label_liquidSensor.Text = "Enable Liquid Level Sensor";
            this.label_liquidSensor.Click += new System.EventHandler(this.label_liquidSensor_Click);
            // 
            // label_lidSensor
            // 
            this.label_lidSensor.AutoSize = true;
            this.label_lidSensor.BackColor = System.Drawing.Color.Transparent;
            this.label_lidSensor.Font = new System.Drawing.Font("Arial Narrow", 14F, System.Drawing.FontStyle.Bold);
            this.label_lidSensor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.label_lidSensor.Location = new System.Drawing.Point(64, 109);
            this.label_lidSensor.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.label_lidSensor.Name = "label_lidSensor";
            this.label_lidSensor.Size = new System.Drawing.Size(190, 41);
            this.label_lidSensor.TabIndex = 15;
            this.label_lidSensor.Text = "Enable Lid Sensor";
            this.label_lidSensor.Click += new System.EventHandler(this.label_lidSensor_Click);
            // 
            // checkBox_liquidSensor
            // 
            this.checkBox_liquidSensor.BackColor = System.Drawing.Color.Transparent;
            this.checkBox_liquidSensor.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("checkBox_liquidSensor.BackgroundImage")));
            this.checkBox_liquidSensor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.checkBox_liquidSensor.Check = false;
            this.checkBox_liquidSensor.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_liquidSensor.ForeColor = System.Drawing.Color.White;
            this.checkBox_liquidSensor.Location = new System.Drawing.Point(24, 165);
            this.checkBox_liquidSensor.Name = "checkBox_liquidSensor";
            this.checkBox_liquidSensor.Size = new System.Drawing.Size(32, 32);
            this.checkBox_liquidSensor.TabIndex = 13;
            this.checkBox_liquidSensor.Text = "  ";
            this.checkBox_liquidSensor.Click += new System.EventHandler(this.checkBox_liquidSensor_Click);
            // 
            // checkBox_lidSensor
            // 
            this.checkBox_lidSensor.BackColor = System.Drawing.Color.Transparent;
            this.checkBox_lidSensor.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("checkBox_lidSensor.BackgroundImage")));
            this.checkBox_lidSensor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.checkBox_lidSensor.Check = false;
            this.checkBox_lidSensor.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_lidSensor.ForeColor = System.Drawing.Color.White;
            this.checkBox_lidSensor.Location = new System.Drawing.Point(24, 114);
            this.checkBox_lidSensor.Name = "checkBox_lidSensor";
            this.checkBox_lidSensor.Size = new System.Drawing.Size(32, 32);
            this.checkBox_lidSensor.TabIndex = 12;
            this.checkBox_lidSensor.Text = "  ";
            this.checkBox_lidSensor.Click += new System.EventHandler(this.checkBox_lidSensor_Click);
            // 
            // checkBox_keyboard
            // 
            this.checkBox_keyboard.BackColor = System.Drawing.Color.Transparent;
            this.checkBox_keyboard.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("checkBox_keyboard.BackgroundImage")));
            this.checkBox_keyboard.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.checkBox_keyboard.Check = false;
            this.checkBox_keyboard.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_keyboard.ForeColor = System.Drawing.Color.White;
            this.checkBox_keyboard.Location = new System.Drawing.Point(24, 63);
            this.checkBox_keyboard.Name = "checkBox_keyboard";
            this.checkBox_keyboard.Size = new System.Drawing.Size(32, 32);
            this.checkBox_keyboard.TabIndex = 11;
            this.checkBox_keyboard.Text = "  ";
            this.checkBox_keyboard.Click += new System.EventHandler(this.checkBox_keyboard_Click);
            // 
            // checkBox_StartTimer
            // 
            this.checkBox_StartTimer.BackColor = System.Drawing.Color.Transparent;
            this.checkBox_StartTimer.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("checkBox_StartTimer.BackgroundImage")));
            this.checkBox_StartTimer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.checkBox_StartTimer.Check = false;
            this.checkBox_StartTimer.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_StartTimer.ForeColor = System.Drawing.Color.White;
            this.checkBox_StartTimer.Location = new System.Drawing.Point(24, 216);
            this.checkBox_StartTimer.Name = "checkBox_StartTimer";
            this.checkBox_StartTimer.Size = new System.Drawing.Size(32, 32);
            this.checkBox_StartTimer.TabIndex = 17;
            this.checkBox_StartTimer.Text = "  ";
            this.checkBox_StartTimer.Visible = false;
            this.checkBox_StartTimer.Click += new System.EventHandler(this.checkBox_StartTimer_Click);
            // 
            // label_keyboard
            // 
            this.label_keyboard.AutoSize = true;
            this.label_keyboard.BackColor = System.Drawing.Color.Transparent;
            this.label_keyboard.Font = new System.Drawing.Font("Arial Narrow", 14F, System.Drawing.FontStyle.Bold);
            this.label_keyboard.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.label_keyboard.Location = new System.Drawing.Point(64, 58);
            this.label_keyboard.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.label_keyboard.Name = "label_keyboard";
            this.label_keyboard.Size = new System.Drawing.Size(193, 41);
            this.label_keyboard.TabIndex = 14;
            this.label_keyboard.Text = "Enable Touch Keyboard";
            this.label_keyboard.Click += new System.EventHandler(this.label_keybaord_Click);
            // 
            // lblServiceMenu
            // 
            this.lblServiceMenu.AutoSize = true;
            this.lblServiceMenu.BackColor = System.Drawing.Color.Transparent;
            this.lblServiceMenu.Font = new System.Drawing.Font("Arial Narrow", 11F, System.Drawing.FontStyle.Bold);
            this.lblServiceMenu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.lblServiceMenu.Location = new System.Drawing.Point(382, 436);
            this.lblServiceMenu.Name = "lblServiceMenu";
            this.lblServiceMenu.Size = new System.Drawing.Size(102, 20);
            this.lblServiceMenu.TabIndex = 17;
            this.lblServiceMenu.Text = "SERVICE MENU";
            this.lblServiceMenu.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // button_Service
            // 
            this.button_Service.BackColor = System.Drawing.Color.Transparent;
            this.button_Service.BackgroundImage = global::GUI_Console.Properties.Resources.SR_BT03L_service_menu_STD;
            this.button_Service.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Service.Check = false;
            this.button_Service.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Service.ForeColor = System.Drawing.Color.White;
            this.button_Service.Location = new System.Drawing.Point(378, 346);
            this.button_Service.Name = "button_Service";
            this.button_Service.Size = new System.Drawing.Size(108, 86);
            this.button_Service.TabIndex = 18;
            this.button_Service.Text = "  ";
            this.button_Service.Click += new System.EventHandler(this.button_Service_Click);
            // 
            // button_Shutdown
            // 
            this.button_Shutdown.BackColor = System.Drawing.Color.Transparent;
            this.button_Shutdown.BackgroundImage = global::GUI_Console.Properties.Resources.SR_BT04L_exit_software_STD;
            this.button_Shutdown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Shutdown.Check = false;
            this.button_Shutdown.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Shutdown.ForeColor = System.Drawing.Color.White;
            this.button_Shutdown.Location = new System.Drawing.Point(520, 346);
            this.button_Shutdown.Name = "button_Shutdown";
            this.button_Shutdown.Size = new System.Drawing.Size(108, 86);
            this.button_Shutdown.TabIndex = 19;
            this.button_Shutdown.Text = "  ";
            this.button_Shutdown.Click += new System.EventHandler(this.button_Shutdown_Click);
            // 
            // lblSoftwareShutdown
            // 
            this.lblSoftwareShutdown.AutoSize = true;
            this.lblSoftwareShutdown.BackColor = System.Drawing.Color.Transparent;
            this.lblSoftwareShutdown.Font = new System.Drawing.Font("Arial Narrow", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSoftwareShutdown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.lblSoftwareShutdown.Location = new System.Drawing.Point(515, 436);
            this.lblSoftwareShutdown.Name = "lblSoftwareShutdown";
            this.lblSoftwareShutdown.Size = new System.Drawing.Size(121, 20);
            this.lblSoftwareShutdown.TabIndex = 20;
            this.lblSoftwareShutdown.Text = "EXIT TO DESKTOP";
            // 
            // label_StartTimer
            // 
            this.label_StartTimer.AutoSize = true;
            this.label_StartTimer.BackColor = System.Drawing.Color.Transparent;
            this.label_StartTimer.Font = new System.Drawing.Font("Arial Narrow", 14F, System.Drawing.FontStyle.Bold);
            this.label_StartTimer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.label_StartTimer.Location = new System.Drawing.Point(64, 213);
            this.label_StartTimer.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.label_StartTimer.Name = "label_StartTimer";
            this.label_StartTimer.Size = new System.Drawing.Size(190, 36);
            this.label_StartTimer.TabIndex = 18;
            this.label_StartTimer.Text = "Enable Delayed Start";
            this.label_StartTimer.Visible = false;
            this.label_StartTimer.Click += new System.EventHandler(this.label_StartTimer_Click);
            // 
            // button_Home
            // 
            this.button_Home.BackColor = System.Drawing.Color.Transparent;
            this.button_Home.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Home.BackgroundImage")));
            this.button_Home.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Home.Check = false;
            this.button_Home.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Home.ForeColor = System.Drawing.Color.White;
            this.button_Home.Location = new System.Drawing.Point(40, 159);
            this.button_Home.Name = "button_Home";
            this.button_Home.Size = new System.Drawing.Size(188, 58);
            this.button_Home.TabIndex = 11;
            this.button_Home.Text = "Home All";
            this.button_Home.Click += new System.EventHandler(this.button_Home_Click);
            // 
            // button_Diagnostic
            // 
            this.button_Diagnostic.BackColor = System.Drawing.Color.Transparent;
            this.button_Diagnostic.BackgroundImage = global::GUI_Console.Properties.Resources.SR_BT01L_diagnostic_STD;
            this.button_Diagnostic.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Diagnostic.Check = false;
            this.button_Diagnostic.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Diagnostic.ForeColor = System.Drawing.Color.White;
            this.button_Diagnostic.Location = new System.Drawing.Point(236, 346);
            this.button_Diagnostic.Name = "button_Diagnostic";
            this.button_Diagnostic.Size = new System.Drawing.Size(108, 86);
            this.button_Diagnostic.TabIndex = 22;
            this.button_Diagnostic.Text = "  ";
            this.button_Diagnostic.Click += new System.EventHandler(this.button_Diagnostic_Click);
            // 
            // lblDiagnosticPackage
            // 
            this.lblDiagnosticPackage.AutoSize = true;
            this.lblDiagnosticPackage.BackColor = System.Drawing.Color.Transparent;
            this.lblDiagnosticPackage.Font = new System.Drawing.Font("Arial Narrow", 11F, System.Drawing.FontStyle.Bold);
            this.lblDiagnosticPackage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.lblDiagnosticPackage.Location = new System.Drawing.Point(211, 436);
            this.lblDiagnosticPackage.Name = "lblDiagnosticPackage";
            this.lblDiagnosticPackage.Size = new System.Drawing.Size(154, 20);
            this.lblDiagnosticPackage.TabIndex = 23;
            this.lblDiagnosticPackage.Text = "DIAGNOSTIC PACKAGE";
            // 
            // SystemPanel1
            // 
            this.SystemPanel1.BackgroundImage = global::GUI_Console.Properties.Resources.SectionDevider;
            this.SystemPanel1.Controls.Add(this.label1);
            this.SystemPanel1.Controls.Add(this.checkBox_keyboard);
            this.SystemPanel1.Controls.Add(this.label_lidSensor);
            this.SystemPanel1.Controls.Add(this.checkBox_liquidSensor);
            this.SystemPanel1.Controls.Add(this.label_liquidSensor);
            this.SystemPanel1.Controls.Add(this.checkBox_lidSensor);
            this.SystemPanel1.Controls.Add(this.label_keyboard);
            this.SystemPanel1.Controls.Add(this.label_StartTimer);
            this.SystemPanel1.Controls.Add(this.checkBox_StartTimer);
            this.SystemPanel1.Location = new System.Drawing.Point(25, 40);
            this.SystemPanel1.Name = "SystemPanel1";
            this.SystemPanel1.Size = new System.Drawing.Size(273, 271);
            this.SystemPanel1.TabIndex = 24;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Arial Narrow", 18.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(46, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(174, 30);
            this.label1.TabIndex = 26;
            this.label1.Text = "System Options";
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::GUI_Console.Properties.Resources.SectionDevider;
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.Button_BasicPrime);
            this.panel1.Controls.Add(this.button_Home);
            this.panel1.Location = new System.Drawing.Point(340, 40);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(273, 271);
            this.panel1.TabIndex = 25;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Arial Narrow", 18.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(46, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(192, 30);
            this.label2.TabIndex = 27;
            this.label2.Text = "System Protocols";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Arial Narrow", 11F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.label3.Location = new System.Drawing.Point(100, 436);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 20);
            this.label3.TabIndex = 27;
            this.label3.Text = "VIDEO LOGS";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ExitSoftwareTimer
            // 
            this.ExitSoftwareTimer.Interval = 25;
            this.ExitSoftwareTimer.Tick += new System.EventHandler(this.ExitSoftwareTimer_Tick);
            // 
            // button_Logs
            // 
            this.button_Logs.BackColor = System.Drawing.Color.Transparent;
            this.button_Logs.BackgroundImage = global::GUI_Console.Properties.Resources.SR_BT02L_run_logs_STD;
            this.button_Logs.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Logs.Check = false;
            this.button_Logs.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Logs.ForeColor = System.Drawing.Color.White;
            this.button_Logs.Location = new System.Drawing.Point(83, 347);
            this.button_Logs.Name = "button_Logs";
            this.button_Logs.Size = new System.Drawing.Size(104, 86);
            this.button_Logs.TabIndex = 28;
            this.button_Logs.Text = "  ";
            this.button_Logs.Click += new System.EventHandler(this.button_Logs_Click);
            // 
            // RoboSep_System
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button_Logs);
            this.Controls.Add(this.SystemPanel1);
            this.Controls.Add(this.button_Shutdown);
            this.Controls.Add(this.button_Service);
            this.Controls.Add(this.lblDiagnosticPackage);
            this.Controls.Add(this.lblServiceMenu);
            this.Controls.Add(this.lblSoftwareShutdown);
            this.Controls.Add(this.button_Diagnostic);
            this.Name = "RoboSep_System";
            this.Load += new System.EventHandler(this.RoboSep_System_Load);
            this.Enter += new System.EventHandler(this.RoboSep_System_Enter);
            this.Leave += new System.EventHandler(this.RoboSep_System_Leave);
            this.Controls.SetChildIndex(this.button_Diagnostic, 0);
            this.Controls.SetChildIndex(this.lblSoftwareShutdown, 0);
            this.Controls.SetChildIndex(this.lblServiceMenu, 0);
            this.Controls.SetChildIndex(this.lblDiagnosticPackage, 0);
            this.Controls.SetChildIndex(this.button_Service, 0);
            this.Controls.SetChildIndex(this.button_Shutdown, 0);
            this.Controls.SetChildIndex(this.SystemPanel1, 0);
            this.Controls.SetChildIndex(this.button_Logs, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.panel1, 0);
            this.Controls.SetChildIndex(this.btn_home, 0);
            this.SystemPanel1.ResumeLayout(false);
            this.SystemPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GUI_Controls.Button_Rectangle Button_BasicPrime;
        private GUI_Controls.CheckBox_Circle checkBox_liquidSensor;
        private GUI_Controls.CheckBox_Circle checkBox_lidSensor;
        private GUI_Controls.CheckBox_Circle checkBox_keyboard;
        private GUI_Controls.CheckBox_Circle checkBox_StartTimer;
        private GUI_Controls.SizableLabel label_keyboard;
        private GUI_Controls.SizableLabel label_liquidSensor;
        private GUI_Controls.SizableLabel label_lidSensor;
        private GUI_Controls.SizableLabel label_StartTimer;
        private GUI_Controls.Button_SmallPink button_Service;
        private GUI_Controls.Button_SmallPink button_Shutdown;
        private System.Windows.Forms.Label lblSoftwareShutdown;
        private System.Windows.Forms.Label lblServiceMenu;
        private GUI_Controls.Button_Rectangle button_Home;
        private GUI_Controls.Button_SmallPink button_Diagnostic;
        private System.Windows.Forms.Label lblDiagnosticPackage;
        private System.Windows.Forms.Panel SystemPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Timer ExitSoftwareTimer;
        private GUI_Controls.Button_SmallPink button_Logs;

    }
}
