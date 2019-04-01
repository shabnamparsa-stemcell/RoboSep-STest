namespace Tesla.Service
{
    partial class ServiceDialog
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServiceDialog));
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtIpAddr = new System.Windows.Forms.TextBox();
            this.timerMachineStateUpdate = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblPumpHome = new System.Windows.Forms.Label();
            this.lblPumpOn = new System.Windows.Forms.Label();
            this.lblBufferPosition = new System.Windows.Forms.Label();
            this.lblTipStripperHome = new System.Windows.Forms.Label();
            this.lblTipStripperLimit = new System.Windows.Forms.Label();
            this.lblCarouselHome = new System.Windows.Forms.Label();
            this.lblCarouselOn = new System.Windows.Forms.Label();
            this.lblThetaHome = new System.Windows.Forms.Label();
            this.lblThetaOn = new System.Windows.Forms.Label();
            this.lblTipStripper = new System.Windows.Forms.Label();
            this.lblScriptRunning = new System.Windows.Forms.Label();
            this.lblZHome = new System.Windows.Forms.Label();
            this.lblZOn = new System.Windows.Forms.Label();
            this.lblPumpVolume = new System.Windows.Forms.Label();
            this.lblCarouselPosition = new System.Windows.Forms.Label();
            this.lblZPosition = new System.Windows.Forms.Label();
            this.lblThetaPosition = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblVideo = new System.Windows.Forms.Label();
            this.button_PageDown = new GUI_Controls.Button_Rectangle();
            this.button_PageUp = new GUI_Controls.Button_Rectangle();
            this.panel3 = new System.Windows.Forms.Panel();
            this.txtScriptStepText = new GUI_Controls.AlphaBlendTextBox();
            this.ctlManualStep = new Tesla.Service.RoboSepServiceManualStepControl();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnect.Location = new System.Drawing.Point(289, 12);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Visible = false;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // txtIpAddr
            // 
            this.txtIpAddr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtIpAddr.Location = new System.Drawing.Point(285, 37);
            this.txtIpAddr.Name = "txtIpAddr";
            this.txtIpAddr.Size = new System.Drawing.Size(100, 20);
            this.txtIpAddr.TabIndex = 1;
            this.txtIpAddr.Text = "localhost";
            this.txtIpAddr.Visible = false;
            // 
            // timerMachineStateUpdate
            // 
            this.timerMachineStateUpdate.Enabled = true;
            this.timerMachineStateUpdate.Interval = 500;
            this.timerMachineStateUpdate.Tick += new System.EventHandler(this.timerMachineStateUpdate_Tick);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.lblPumpHome);
            this.panel1.Controls.Add(this.lblPumpOn);
            this.panel1.Controls.Add(this.lblBufferPosition);
            this.panel1.Controls.Add(this.lblTipStripperHome);
            this.panel1.Controls.Add(this.lblTipStripperLimit);
            this.panel1.Controls.Add(this.lblCarouselHome);
            this.panel1.Controls.Add(this.lblCarouselOn);
            this.panel1.Controls.Add(this.lblThetaHome);
            this.panel1.Controls.Add(this.lblThetaOn);
            this.panel1.Controls.Add(this.lblTipStripper);
            this.panel1.Controls.Add(this.lblScriptRunning);
            this.panel1.Controls.Add(this.lblZHome);
            this.panel1.Controls.Add(this.lblZOn);
            this.panel1.Controls.Add(this.lblPumpVolume);
            this.panel1.Controls.Add(this.lblCarouselPosition);
            this.panel1.Controls.Add(this.lblZPosition);
            this.panel1.Controls.Add(this.lblThetaPosition);
            this.panel1.ForeColor = System.Drawing.Color.White;
            this.panel1.Location = new System.Drawing.Point(12, 92);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(172, 372);
            this.panel1.TabIndex = 23;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // lblPumpHome
            // 
            this.lblPumpHome.AutoSize = true;
            this.lblPumpHome.BackColor = System.Drawing.Color.Transparent;
            this.lblPumpHome.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPumpHome.ForeColor = System.Drawing.Color.Black;
            this.lblPumpHome.Location = new System.Drawing.Point(64, 267);
            this.lblPumpHome.Name = "lblPumpHome";
            this.lblPumpHome.Size = new System.Drawing.Size(27, 13);
            this.lblPumpHome.TabIndex = 42;
            this.lblPumpHome.Text = "HM";
            // 
            // lblPumpOn
            // 
            this.lblPumpOn.AutoSize = true;
            this.lblPumpOn.BackColor = System.Drawing.Color.Transparent;
            this.lblPumpOn.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPumpOn.ForeColor = System.Drawing.Color.Black;
            this.lblPumpOn.Location = new System.Drawing.Point(64, 292);
            this.lblPumpOn.Name = "lblPumpOn";
            this.lblPumpOn.Size = new System.Drawing.Size(29, 13);
            this.lblPumpOn.TabIndex = 41;
            this.lblPumpOn.Text = "LIM";
            // 
            // lblBufferPosition
            // 
            this.lblBufferPosition.BackColor = System.Drawing.Color.White;
            this.lblBufferPosition.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBufferPosition.ForeColor = System.Drawing.Color.Black;
            this.lblBufferPosition.Location = new System.Drawing.Point(60, 242);
            this.lblBufferPosition.Name = "lblBufferPosition";
            this.lblBufferPosition.Size = new System.Drawing.Size(70, 20);
            this.lblBufferPosition.TabIndex = 40;
            this.lblBufferPosition.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTipStripperHome
            // 
            this.lblTipStripperHome.AutoSize = true;
            this.lblTipStripperHome.BackColor = System.Drawing.Color.Transparent;
            this.lblTipStripperHome.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTipStripperHome.ForeColor = System.Drawing.Color.Black;
            this.lblTipStripperHome.Location = new System.Drawing.Point(116, 267);
            this.lblTipStripperHome.Name = "lblTipStripperHome";
            this.lblTipStripperHome.Size = new System.Drawing.Size(29, 13);
            this.lblTipStripperHome.TabIndex = 39;
            this.lblTipStripperHome.Text = "EXT";
            // 
            // lblTipStripperLimit
            // 
            this.lblTipStripperLimit.AutoSize = true;
            this.lblTipStripperLimit.BackColor = System.Drawing.Color.Transparent;
            this.lblTipStripperLimit.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTipStripperLimit.ForeColor = System.Drawing.Color.Black;
            this.lblTipStripperLimit.Location = new System.Drawing.Point(116, 242);
            this.lblTipStripperLimit.Name = "lblTipStripperLimit";
            this.lblTipStripperLimit.Size = new System.Drawing.Size(29, 13);
            this.lblTipStripperLimit.TabIndex = 38;
            this.lblTipStripperLimit.Text = "RET";
            // 
            // lblCarouselHome
            // 
            this.lblCarouselHome.AutoSize = true;
            this.lblCarouselHome.BackColor = System.Drawing.Color.Transparent;
            this.lblCarouselHome.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCarouselHome.ForeColor = System.Drawing.Color.Black;
            this.lblCarouselHome.Location = new System.Drawing.Point(119, 178);
            this.lblCarouselHome.Name = "lblCarouselHome";
            this.lblCarouselHome.Size = new System.Drawing.Size(27, 13);
            this.lblCarouselHome.TabIndex = 37;
            this.lblCarouselHome.Text = "HM";
            // 
            // lblCarouselOn
            // 
            this.lblCarouselOn.AutoSize = true;
            this.lblCarouselOn.BackColor = System.Drawing.Color.Transparent;
            this.lblCarouselOn.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCarouselOn.ForeColor = System.Drawing.Color.Black;
            this.lblCarouselOn.Location = new System.Drawing.Point(119, 203);
            this.lblCarouselOn.Name = "lblCarouselOn";
            this.lblCarouselOn.Size = new System.Drawing.Size(29, 13);
            this.lblCarouselOn.TabIndex = 36;
            this.lblCarouselOn.Text = "LIM";
            // 
            // lblThetaHome
            // 
            this.lblThetaHome.AutoSize = true;
            this.lblThetaHome.BackColor = System.Drawing.Color.Transparent;
            this.lblThetaHome.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblThetaHome.ForeColor = System.Drawing.Color.Black;
            this.lblThetaHome.Location = new System.Drawing.Point(120, 112);
            this.lblThetaHome.Name = "lblThetaHome";
            this.lblThetaHome.Size = new System.Drawing.Size(27, 13);
            this.lblThetaHome.TabIndex = 35;
            this.lblThetaHome.Text = "HM";
            // 
            // lblThetaOn
            // 
            this.lblThetaOn.AutoSize = true;
            this.lblThetaOn.BackColor = System.Drawing.Color.Transparent;
            this.lblThetaOn.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblThetaOn.ForeColor = System.Drawing.Color.Black;
            this.lblThetaOn.Location = new System.Drawing.Point(120, 137);
            this.lblThetaOn.Name = "lblThetaOn";
            this.lblThetaOn.Size = new System.Drawing.Size(29, 13);
            this.lblThetaOn.TabIndex = 34;
            this.lblThetaOn.Text = "LIM";
            // 
            // lblTipStripper
            // 
            this.lblTipStripper.BackColor = System.Drawing.Color.White;
            this.lblTipStripper.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTipStripper.ForeColor = System.Drawing.Color.Black;
            this.lblTipStripper.Location = new System.Drawing.Point(64, 173);
            this.lblTipStripper.Name = "lblTipStripper";
            this.lblTipStripper.Size = new System.Drawing.Size(70, 20);
            this.lblTipStripper.TabIndex = 33;
            this.lblTipStripper.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblScriptRunning
            // 
            this.lblScriptRunning.BackColor = System.Drawing.Color.Transparent;
            this.lblScriptRunning.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblScriptRunning.ForeColor = System.Drawing.Color.White;
            this.lblScriptRunning.Location = new System.Drawing.Point(4, 7);
            this.lblScriptRunning.Name = "lblScriptRunning";
            this.lblScriptRunning.Size = new System.Drawing.Size(165, 28);
            this.lblScriptRunning.TabIndex = 27;
            this.lblScriptRunning.Text = "Script: N/A";
            this.lblScriptRunning.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblZHome
            // 
            this.lblZHome.AutoSize = true;
            this.lblZHome.BackColor = System.Drawing.Color.Transparent;
            this.lblZHome.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblZHome.ForeColor = System.Drawing.Color.Black;
            this.lblZHome.Location = new System.Drawing.Point(119, 48);
            this.lblZHome.Name = "lblZHome";
            this.lblZHome.Size = new System.Drawing.Size(27, 13);
            this.lblZHome.TabIndex = 32;
            this.lblZHome.Text = "HM";
            // 
            // lblZOn
            // 
            this.lblZOn.AutoSize = true;
            this.lblZOn.BackColor = System.Drawing.Color.Transparent;
            this.lblZOn.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblZOn.ForeColor = System.Drawing.Color.Black;
            this.lblZOn.Location = new System.Drawing.Point(119, 73);
            this.lblZOn.Name = "lblZOn";
            this.lblZOn.Size = new System.Drawing.Size(29, 13);
            this.lblZOn.TabIndex = 31;
            this.lblZOn.Text = "LIM";
            // 
            // lblPumpVolume
            // 
            this.lblPumpVolume.BackColor = System.Drawing.Color.White;
            this.lblPumpVolume.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPumpVolume.ForeColor = System.Drawing.Color.Black;
            this.lblPumpVolume.Location = new System.Drawing.Point(64, 203);
            this.lblPumpVolume.Name = "lblPumpVolume";
            this.lblPumpVolume.Size = new System.Drawing.Size(70, 20);
            this.lblPumpVolume.TabIndex = 27;
            this.lblPumpVolume.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCarouselPosition
            // 
            this.lblCarouselPosition.BackColor = System.Drawing.Color.White;
            this.lblCarouselPosition.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCarouselPosition.ForeColor = System.Drawing.Color.Black;
            this.lblCarouselPosition.Location = new System.Drawing.Point(64, 132);
            this.lblCarouselPosition.Name = "lblCarouselPosition";
            this.lblCarouselPosition.Size = new System.Drawing.Size(70, 20);
            this.lblCarouselPosition.TabIndex = 26;
            this.lblCarouselPosition.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblZPosition
            // 
            this.lblZPosition.BackColor = System.Drawing.Color.White;
            this.lblZPosition.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblZPosition.ForeColor = System.Drawing.Color.Black;
            this.lblZPosition.Location = new System.Drawing.Point(64, 58);
            this.lblZPosition.Name = "lblZPosition";
            this.lblZPosition.Size = new System.Drawing.Size(70, 20);
            this.lblZPosition.TabIndex = 25;
            this.lblZPosition.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblThetaPosition
            // 
            this.lblThetaPosition.BackColor = System.Drawing.Color.White;
            this.lblThetaPosition.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblThetaPosition.ForeColor = System.Drawing.Color.Black;
            this.lblThetaPosition.Location = new System.Drawing.Point(64, 95);
            this.lblThetaPosition.Name = "lblThetaPosition";
            this.lblThetaPosition.Size = new System.Drawing.Size(70, 20);
            this.lblThetaPosition.TabIndex = 24;
            this.lblThetaPosition.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(497, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(61, 41);
            this.button1.TabIndex = 28;
            this.button1.Text = "Up";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel2.Controls.Add(this.lblVideo);
            this.panel2.Controls.Add(this.button_PageDown);
            this.panel2.Controls.Add(this.button_PageUp);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.txtScriptStepText);
            this.panel2.Location = new System.Drawing.Point(223, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(414, 80);
            this.panel2.TabIndex = 30;
            // 
            // lblVideo
            // 
            this.lblVideo.AutoSize = true;
            this.lblVideo.BackColor = System.Drawing.Color.Transparent;
            this.lblVideo.Enabled = false;
            this.lblVideo.Font = new System.Drawing.Font("Frutiger LT Std 45 Light", 9.749999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVideo.ForeColor = System.Drawing.Color.Black;
            this.lblVideo.Location = new System.Drawing.Point(359, 60);
            this.lblVideo.Name = "lblVideo";
            this.lblVideo.Size = new System.Drawing.Size(46, 15);
            this.lblVideo.TabIndex = 44;
            this.lblVideo.Text = "VIDEO";
            this.lblVideo.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // button_PageDown
            // 
            this.button_PageDown.AccessibleName = "  ";
            this.button_PageDown.BackColor = System.Drawing.Color.Transparent;
            this.button_PageDown.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_PageDown.BackgroundImage")));
            this.button_PageDown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_PageDown.Check = false;
            this.button_PageDown.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button_PageDown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.button_PageDown.Location = new System.Drawing.Point(319, 41);
            this.button_PageDown.Name = "button_PageDown";
            this.button_PageDown.Size = new System.Drawing.Size(32, 32);
            this.button_PageDown.TabIndex = 26;
            this.button_PageDown.Click += new System.EventHandler(this.button_PageDown_Click);
            // 
            // button_PageUp
            // 
            this.button_PageUp.AccessibleName = "  ";
            this.button_PageUp.BackColor = System.Drawing.Color.Transparent;
            this.button_PageUp.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_PageUp.BackgroundImage")));
            this.button_PageUp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_PageUp.Check = false;
            this.button_PageUp.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button_PageUp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.button_PageUp.Location = new System.Drawing.Point(319, 6);
            this.button_PageUp.Name = "button_PageUp";
            this.button_PageUp.Size = new System.Drawing.Size(32, 32);
            this.button_PageUp.TabIndex = 25;
            this.button_PageUp.Click += new System.EventHandler(this.button_PageUp_Click);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Transparent;
            this.panel3.Location = new System.Drawing.Point(318, 5);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(24, 70);
            this.panel3.TabIndex = 27;
            // 
            // txtScriptStepText
            // 
            this.txtScriptStepText.BackAlpha = 0;
            this.txtScriptStepText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.txtScriptStepText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtScriptStepText.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtScriptStepText.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtScriptStepText.ForeColor = System.Drawing.Color.Red;
            this.txtScriptStepText.Location = new System.Drawing.Point(6, 5);
            this.txtScriptStepText.Multiline = true;
            this.txtScriptStepText.Name = "txtScriptStepText";
            this.txtScriptStepText.ReadOnly = true;
            this.txtScriptStepText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtScriptStepText.Size = new System.Drawing.Size(330, 70);
            this.txtScriptStepText.TabIndex = 22;
            this.txtScriptStepText.TextChanged += new System.EventHandler(this.txtScriptStepText_TextChanged);
            // 
            // ctlManualStep
            // 
            this.ctlManualStep.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ctlManualStep.BackColor = System.Drawing.Color.Transparent;
            this.ctlManualStep.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ctlManualStep.Enabled = false;
            this.ctlManualStep.Location = new System.Drawing.Point(230, 95);
            this.ctlManualStep.Name = "ctlManualStep";
            this.ctlManualStep.Size = new System.Drawing.Size(400, 370);
            this.ctlManualStep.TabIndex = 21;
            // 
            // ServiceDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackgroundImage = global::Tesla.Service.Properties.Resources.robosep_service_background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(640, 480);
            this.ControlBox = false;
            this.Controls.Add(this.ctlManualStep);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtIpAddr);
            this.Controls.Add(this.btnConnect);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ServiceDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ServiceMain";
            this.Shown += new System.EventHandler(this.ServiceDialog_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtIpAddr;
        private System.Windows.Forms.Timer timerMachineStateUpdate;
        private RoboSepServiceManualStepControl ctlManualStep;
        private GUI_Controls.AlphaBlendTextBox txtScriptStepText;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblZHome;
        private System.Windows.Forms.Label lblZOn;
        private System.Windows.Forms.Label lblPumpVolume;
        private System.Windows.Forms.Label lblCarouselPosition;
        private System.Windows.Forms.Label lblZPosition;
        private System.Windows.Forms.Label lblThetaPosition;
        private System.Windows.Forms.Label lblScriptRunning;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lblTipStripper;
        private System.Windows.Forms.Label lblTipStripperHome;
        private System.Windows.Forms.Label lblTipStripperLimit;
        private System.Windows.Forms.Label lblCarouselHome;
        private System.Windows.Forms.Label lblCarouselOn;
        private System.Windows.Forms.Label lblThetaHome;
        private System.Windows.Forms.Label lblThetaOn;
        private System.Windows.Forms.Label lblPumpHome;
        private System.Windows.Forms.Label lblPumpOn;
        private System.Windows.Forms.Label lblBufferPosition;
        private System.Windows.Forms.Panel panel2;
        private GUI_Controls.Button_Rectangle button_PageUp;
        private GUI_Controls.Button_Rectangle button_PageDown;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label lblVideo;
    }
}

