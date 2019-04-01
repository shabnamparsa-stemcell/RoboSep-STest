namespace GUI_Console
{
    partial class Form_Home
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
        /// Required method for Designer support - do not modifyS
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Home));
            this.btn_close = new GUI_Controls.Button_Circle();
            this.hex_logs = new GUI_Controls.Button_Hexagon();
            this.hex_shutdown = new GUI_Controls.Button_Hexagon();
            this.hex_service = new GUI_Controls.Button_Hexagon();
            this.hex_help = new GUI_Controls.Button_Hexagon();
            this.hex_system = new GUI_Controls.Button_Hexagon();
            this.hex_protocols = new GUI_Controls.Button_Hexagon();
            this.hex_sampling = new GUI_Controls.Button_Hexagon();
            this.SuspendLayout();
            // 
            // btn_close
            // 
            this.btn_close.AccessibleName = " ";
            this.btn_close.BackgroundImage = GUI_Controls.Properties.Resources.Button_CANCEL0;
            this.btn_close.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_close.Check = false;
            this.btn_close.Location = new System.Drawing.Point(577, 12);
            this.btn_close.Name = "btn_close";
            this.btn_close.Size = new System.Drawing.Size(52, 50);
            this.btn_close.TabIndex = 7;
            this.btn_close.Load += new System.EventHandler(this.button_Circle1_Load);
            this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
            // 
            // hex_logs
            // 
            this.hex_logs.AccessibleName = " ";
            this.hex_logs.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.hex_logs.Check = false;
            this.hex_logs.Location = new System.Drawing.Point(426, 326);
            this.hex_logs.Name = "hex_logs";
            this.hex_logs.Size = new System.Drawing.Size(180, 156);
            this.hex_logs.TabIndex = 6;
            this.hex_logs.Click += new System.EventHandler(this.hex_logs_Click);
            // 
            // hex_shutdown
            // 
            this.hex_shutdown.AccessibleName = " ";
            this.hex_shutdown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.hex_shutdown.Check = false;
            this.hex_shutdown.Location = new System.Drawing.Point(424, 169);
            this.hex_shutdown.Name = "hex_shutdown";
            this.hex_shutdown.Size = new System.Drawing.Size(180, 156);
            this.hex_shutdown.TabIndex = 5;
            this.hex_shutdown.Click += new System.EventHandler(this.hex_shutdown_Click);
            // 
            // hex_service
            // 
            this.hex_service.AccessibleName = " ";
            this.hex_service.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.hex_service.Check = false;
            this.hex_service.Location = new System.Drawing.Point(288, 247);
            this.hex_service.Name = "hex_service";
            this.hex_service.Size = new System.Drawing.Size(180, 156);
            this.hex_service.TabIndex = 4;
            this.hex_service.Click += new System.EventHandler(this.hex_service_Click);
            // 
            // hex_help
            // 
            this.hex_help.AccessibleName = " ";
            this.hex_help.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.hex_help.Check = false;
            this.hex_help.Location = new System.Drawing.Point(286, 91);
            this.hex_help.Name = "hex_help";
            this.hex_help.Size = new System.Drawing.Size(180, 156);
            this.hex_help.TabIndex = 3;
            this.hex_help.Click += new System.EventHandler(this.hex_help_Click);
            // 
            // hex_system
            // 
            this.hex_system.AccessibleName = " ";
            this.hex_system.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.hex_system.Check = false;
            this.hex_system.Location = new System.Drawing.Point(151, 169);
            this.hex_system.Name = "hex_system";
            this.hex_system.Size = new System.Drawing.Size(180, 156);
            this.hex_system.TabIndex = 2;
            this.hex_system.Click += new System.EventHandler(this.hex_system_Click);
            // 
            // hex_protocols
            // 
            this.hex_protocols.AccessibleName = " ";
            this.hex_protocols.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.hex_protocols.Check = false;
            this.hex_protocols.Location = new System.Drawing.Point(151, 11);
            this.hex_protocols.Name = "hex_protocols";
            this.hex_protocols.Size = new System.Drawing.Size(180, 156);
            this.hex_protocols.TabIndex = 1;
            this.hex_protocols.Click += new System.EventHandler(this.hex_protocols_Click);
            // 
            // hex_sampling
            // 
            this.hex_sampling.AccessibleName = " ";
            this.hex_sampling.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.hex_sampling.Check = false;
            this.hex_sampling.Location = new System.Drawing.Point(14, 91);
            this.hex_sampling.Name = "hex_sampling";
            this.hex_sampling.Size = new System.Drawing.Size(180, 156);
            this.hex_sampling.TabIndex = 0;
            this.hex_sampling.Click += new System.EventHandler(this.hex_sampling_Click);
            // 
            // Form_Home
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = GUI_Controls.Properties.Resources.BG_HEXGRID;
            this.ClientSize = new System.Drawing.Size(640, 480);
            this.Controls.Add(this.btn_close);
            this.Controls.Add(this.hex_logs);
            this.Controls.Add(this.hex_shutdown);
            this.Controls.Add(this.hex_service);
            this.Controls.Add(this.hex_help);
            this.Controls.Add(this.hex_system);
            this.Controls.Add(this.hex_protocols);
            this.Controls.Add(this.hex_sampling);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form_Home";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form_Home";
            this.TransparencyKey = System.Drawing.Color.Black;
            this.Deactivate += new System.EventHandler(this.Form_Home_Deactivate);
            this.ResumeLayout(false);

        }

        #endregion


        private GUI_Controls.Button_Hexagon hex_sampling;
        private GUI_Controls.Button_Hexagon hex_protocols;
        private GUI_Controls.Button_Hexagon hex_help;
        private GUI_Controls.Button_Hexagon hex_system;
        private GUI_Controls.Button_Hexagon hex_logs;
        private GUI_Controls.Button_Hexagon hex_shutdown;
        private GUI_Controls.Button_Hexagon hex_service;
        private GUI_Controls.Button_Circle btn_close;
    }
}