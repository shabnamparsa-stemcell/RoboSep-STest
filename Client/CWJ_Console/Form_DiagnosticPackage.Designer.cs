namespace GUI_Console
{
    partial class Form_DiagnosticPackage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_DiagnosticPackage));
            this.label_WindowTitle = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rectangleShape1 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.checkBox_WindowsEvents = new GUI_Controls.CheckBox_Square();
            this.label_WindowEvent = new GUI_Controls.SizableLabel();
            this.checkBox_USB = new GUI_Controls.CheckBox_Circle();
            this.label_USB = new GUI_Controls.SizableLabel();
            this.label_FTPServer = new GUI_Controls.SizableLabel();
            this.checkBox_FTPServer = new GUI_Controls.CheckBox_Circle();
            this.button_Cancel = new GUI_Controls.Button_Cancel();
            this.button_OK = new GUI_Controls.Button_Circle();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label_WindowTitle
            // 
            this.label_WindowTitle.BackColor = System.Drawing.Color.Transparent;
            this.label_WindowTitle.Font = new System.Drawing.Font("Arial", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_WindowTitle.ForeColor = System.Drawing.Color.White;
            this.label_WindowTitle.Location = new System.Drawing.Point(8, 4);
            this.label_WindowTitle.Margin = new System.Windows.Forms.Padding(0);
            this.label_WindowTitle.Name = "label_WindowTitle";
            this.label_WindowTitle.Size = new System.Drawing.Size(297, 27);
            this.label_WindowTitle.TabIndex = 33;
            this.label_WindowTitle.Text = "Window Title";
            this.label_WindowTitle.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // groupBox2
            // 
            this.groupBox2.Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.groupBox2.Location = new System.Drawing.Point(28, 66);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(260, 139);
            this.groupBox2.TabIndex = 45;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Destination";
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.Controls.Add(this.label_WindowTitle);
            this.panel1.Location = new System.Drawing.Point(2, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(312, 35);
            this.panel1.TabIndex = 46;
            // 
            // rectangleShape1
            // 
            this.rectangleShape1.BorderWidth = 2;
            this.rectangleShape1.Location = new System.Drawing.Point(1, 2);
            this.rectangleShape1.Name = "rectangleShape1";
            this.rectangleShape1.Size = new System.Drawing.Size(314, 405);
            // 
            // shapeContainer1
            // 
            this.shapeContainer1.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer1.Name = "shapeContainer1";
            this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.rectangleShape1});
            this.shapeContainer1.Size = new System.Drawing.Size(316, 408);
            this.shapeContainer1.TabIndex = 47;
            this.shapeContainer1.TabStop = false;
            // 
            // checkBox_WindowsEvents
            // 
            this.checkBox_WindowsEvents.BackColor = System.Drawing.Color.Transparent;
            this.checkBox_WindowsEvents.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("checkBox_WindowsEvents.BackgroundImage")));
            this.checkBox_WindowsEvents.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.checkBox_WindowsEvents.Check = false;
            this.checkBox_WindowsEvents.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_WindowsEvents.ForeColor = System.Drawing.Color.White;
            this.checkBox_WindowsEvents.Location = new System.Drawing.Point(15, 231);
            this.checkBox_WindowsEvents.Name = "checkBox_WindowsEvents";
            this.checkBox_WindowsEvents.Size = new System.Drawing.Size(32, 32);
            this.checkBox_WindowsEvents.TabIndex = 48;
            this.checkBox_WindowsEvents.Text = "checkBox_Square1";
            // 
            // label_WindowEvent
            // 
            this.label_WindowEvent.AutoSize = true;
            this.label_WindowEvent.BackColor = System.Drawing.Color.Transparent;
            this.label_WindowEvent.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_WindowEvent.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.label_WindowEvent.Location = new System.Drawing.Point(55, 225);
            this.label_WindowEvent.Margin = new System.Windows.Forms.Padding(5);
            this.label_WindowEvent.Name = "label_WindowEvent";
            this.label_WindowEvent.Size = new System.Drawing.Size(233, 41);
            this.label_WindowEvent.TabIndex = 40;
            this.label_WindowEvent.Text = "Include Windows Events";
            // 
            // checkBox_USB
            // 
            this.checkBox_USB.BackColor = System.Drawing.Color.Transparent;
            this.checkBox_USB.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("checkBox_USB.BackgroundImage")));
            this.checkBox_USB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.checkBox_USB.Check = false;
            this.checkBox_USB.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_USB.ForeColor = System.Drawing.Color.White;
            this.checkBox_USB.Location = new System.Drawing.Point(55, 104);
            this.checkBox_USB.Name = "checkBox_USB";
            this.checkBox_USB.Size = new System.Drawing.Size(32, 32);
            this.checkBox_USB.TabIndex = 38;
            this.checkBox_USB.Text = "  ";
            this.checkBox_USB.Click += new System.EventHandler(this.checkBox_USB_Click);
            // 
            // label_USB
            // 
            this.label_USB.AutoSize = true;
            this.label_USB.BackColor = System.Drawing.Color.Transparent;
            this.label_USB.Font = new System.Drawing.Font("Arial Narrow", 14F, System.Drawing.FontStyle.Bold);
            this.label_USB.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.label_USB.Location = new System.Drawing.Point(95, 98);
            this.label_USB.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.label_USB.Name = "label_USB";
            this.label_USB.Size = new System.Drawing.Size(184, 40);
            this.label_USB.TabIndex = 41;
            this.label_USB.Text = "USB";
            // 
            // label_FTPServer
            // 
            this.label_FTPServer.AutoSize = true;
            this.label_FTPServer.BackColor = System.Drawing.Color.Transparent;
            this.label_FTPServer.Font = new System.Drawing.Font("Arial Narrow", 14F, System.Drawing.FontStyle.Bold);
            this.label_FTPServer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.label_FTPServer.Location = new System.Drawing.Point(95, 152);
            this.label_FTPServer.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.label_FTPServer.Name = "label_FTPServer";
            this.label_FTPServer.Size = new System.Drawing.Size(184, 36);
            this.label_FTPServer.TabIndex = 43;
            this.label_FTPServer.Text = "FTP Server";
            // 
            // checkBox_FTPServer
            // 
            this.checkBox_FTPServer.BackColor = System.Drawing.Color.Transparent;
            this.checkBox_FTPServer.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("checkBox_FTPServer.BackgroundImage")));
            this.checkBox_FTPServer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.checkBox_FTPServer.Check = false;
            this.checkBox_FTPServer.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_FTPServer.ForeColor = System.Drawing.Color.White;
            this.checkBox_FTPServer.Location = new System.Drawing.Point(55, 155);
            this.checkBox_FTPServer.Name = "checkBox_FTPServer";
            this.checkBox_FTPServer.Size = new System.Drawing.Size(32, 32);
            this.checkBox_FTPServer.TabIndex = 42;
            this.checkBox_FTPServer.Text = "  ";
            this.checkBox_FTPServer.Click += new System.EventHandler(this.checkBox_FTPServer_Click);
            // 
            // button_Cancel
            // 
            this.button_Cancel.BackColor = System.Drawing.Color.Transparent;
            this.button_Cancel.BackgroundImage = global::GUI_Console.Properties.Resources.GE_BTN13L_cancel_STD;
            this.button_Cancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Cancel.Check = false;
            this.button_Cancel.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Cancel.ForeColor = System.Drawing.Color.White;
            this.button_Cancel.Location = new System.Drawing.Point(97, 308);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(104, 86);
            this.button_Cancel.TabIndex = 35;
            this.button_Cancel.Text = "button_Cancel2";
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // button_OK
            // 
            this.button_OK.BackColor = System.Drawing.Color.Transparent;
            this.button_OK.BackgroundImage = global::GUI_Console.Properties.Resources.GE_BTN20L_ok_STD;
            this.button_OK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_OK.Check = false;
            this.button_OK.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_OK.ForeColor = System.Drawing.Color.White;
            this.button_OK.Location = new System.Drawing.Point(207, 308);
            this.button_OK.Name = "button_OK";
            this.button_OK.Size = new System.Drawing.Size(104, 86);
            this.button_OK.TabIndex = 34;
            this.button_OK.Text = "OK";
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // Form_DiagnosticPackage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(316, 408);
            this.Controls.Add(this.checkBox_WindowsEvents);
            this.Controls.Add(this.label_WindowEvent);
            this.Controls.Add(this.checkBox_USB);
            this.Controls.Add(this.label_USB);
            this.Controls.Add(this.label_FTPServer);
            this.Controls.Add(this.checkBox_FTPServer);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.shapeContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form_DiagnosticPackage";
            this.Text = "Form_DiagnosticPackage";
            this.Load += new System.EventHandler(this.Form_DiagnosticPackage_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.Label label_WindowTitle;
        private GUI_Controls.Button_Cancel button_Cancel;
        private GUI_Controls.Button_Circle button_OK;
        private GUI_Controls.SizableLabel label_WindowEvent;
        private GUI_Controls.CheckBox_Circle checkBox_USB;
        private GUI_Controls.SizableLabel label_USB;
        private GUI_Controls.SizableLabel label_FTPServer;
        private GUI_Controls.CheckBox_Circle checkBox_FTPServer;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel panel1;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape1;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private GUI_Controls.CheckBox_Square checkBox_WindowsEvents;

    }
}