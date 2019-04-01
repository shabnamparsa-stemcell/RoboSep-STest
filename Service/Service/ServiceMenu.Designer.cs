namespace Tesla.Service
{
    partial class ServiceMenu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServiceMenu));
            this.listboxMenu = new System.Windows.Forms.ListBox();
            this.listboxSubMenu = new System.Windows.Forms.ListBox();
            this.txtIpAddr = new System.Windows.Forms.TextBox();
            this.pnlCalibrate = new System.Windows.Forms.Panel();
            this.lblVersion = new System.Windows.Forms.Label();
            this.pnlTeaching = new System.Windows.Forms.Panel();
            this.listboxTeachingSubMenu = new System.Windows.Forms.ListBox();
            this.pnlServiceLog = new System.Windows.Forms.Panel();
            this.button_PageDown = new GUI_Controls.Button_Rectangle();
            this.button_PageUp = new GUI_Controls.Button_Rectangle();
            this.listServiceLogs = new GUI_Controls.ListboxNoScroll();
            this.panel2 = new System.Windows.Forms.Panel();
            this.imgMainSelectionBridge = new System.Windows.Forms.PictureBox();
            this.pnlSettings = new System.Windows.Forms.Panel();
            this.button_BarcodeRescan = new GUI_Controls.Button_Rectangle();
            this.button_RestoreBackup = new GUI_Controls.Button_Rectangle();
            this.button_LoggingLevel = new GUI_Controls.Button_Rectangle();
            this.label2 = new System.Windows.Forms.Label();
            this.button_RestoreFactory = new GUI_Controls.Button_Rectangle();
            this.button_SaveAsFactory = new GUI_Controls.Button_Rectangle();
            this.button_TimedStart = new GUI_Controls.Button_Rectangle();
            this.button_Lid = new GUI_Controls.Button_Rectangle();
            this.lblTimedStart = new System.Windows.Forms.Label();
            this.lblLidSensor = new System.Windows.Forms.Label();
            this.pnlExit = new System.Windows.Forms.Panel();
            this.button_Pendant = new GUI_Controls.Button_Rectangle();
            this.lblPendant = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblConnect = new System.Windows.Forms.Label();
            this.btn_close = new GUI_Controls.Button_Circle();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label3 = new System.Windows.Forms.Label();
            this.button_Connect = new GUI_Controls.Button_Rectangle();
            this.pnlCalibrate.SuspendLayout();
            this.pnlTeaching.SuspendLayout();
            this.pnlServiceLog.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgMainSelectionBridge)).BeginInit();
            this.pnlSettings.SuspendLayout();
            this.pnlExit.SuspendLayout();
            this.SuspendLayout();
            // 
            // listboxMenu
            // 
            this.listboxMenu.BackColor = System.Drawing.Color.LightGray;
            this.listboxMenu.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listboxMenu.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.listboxMenu.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listboxMenu.FormattingEnabled = true;
            this.listboxMenu.ItemHeight = 25;
            this.listboxMenu.Items.AddRange(new object[] {
            "Calibrate",
            "Teaching",
            "Settings",
            "Service Logs",
            "Exit"});
            this.listboxMenu.Location = new System.Drawing.Point(0, 0);
            this.listboxMenu.Name = "listboxMenu";
            this.listboxMenu.Size = new System.Drawing.Size(190, 274);
            this.listboxMenu.TabIndex = 0;
            this.listboxMenu.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listboxMenu_DrawItem);
            this.listboxMenu.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.listboxMenu_MeasureItem);
            this.listboxMenu.SelectedIndexChanged += new System.EventHandler(this.listboxMenu_SelectedIndexChanged);
            // 
            // listboxSubMenu
            // 
            this.listboxSubMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(38)))), ((int)(((byte)(131)))));
            this.listboxSubMenu.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listboxSubMenu.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.listboxSubMenu.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listboxSubMenu.FormattingEnabled = true;
            this.listboxSubMenu.ItemHeight = 1;
            this.listboxSubMenu.Location = new System.Drawing.Point(0, 0);
            this.listboxSubMenu.Name = "listboxSubMenu";
            this.listboxSubMenu.Size = new System.Drawing.Size(260, 274);
            this.listboxSubMenu.TabIndex = 1;
            this.listboxSubMenu.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listboxSubMenu_DrawItem);
            this.listboxSubMenu.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.listboxSubMenu_MeasureItem);
            // 
            // txtIpAddr
            // 
            this.txtIpAddr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtIpAddr.BackColor = System.Drawing.Color.Black;
            this.txtIpAddr.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtIpAddr.ForeColor = System.Drawing.Color.White;
            this.txtIpAddr.Location = new System.Drawing.Point(233, 49);
            this.txtIpAddr.Name = "txtIpAddr";
            this.txtIpAddr.Size = new System.Drawing.Size(94, 27);
            this.txtIpAddr.TabIndex = 3;
            this.txtIpAddr.TabStop = false;
            this.txtIpAddr.Text = "localhost";
            this.txtIpAddr.MouseClick += new System.Windows.Forms.MouseEventHandler(this.txtIpAddr_MouseClick);
            // 
            // pnlCalibrate
            // 
            this.pnlCalibrate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlCalibrate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(38)))), ((int)(((byte)(131)))));
            this.pnlCalibrate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnlCalibrate.Controls.Add(this.listboxSubMenu);
            this.pnlCalibrate.Location = new System.Drawing.Point(189, 85);
            this.pnlCalibrate.Name = "pnlCalibrate";
            this.pnlCalibrate.Size = new System.Drawing.Size(260, 274);
            this.pnlCalibrate.TabIndex = 5;
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.BackColor = System.Drawing.Color.Transparent;
            this.lblVersion.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.ForeColor = System.Drawing.Color.White;
            this.lblVersion.Location = new System.Drawing.Point(45, 30);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(166, 19);
            this.lblVersion.TabIndex = 26;
            this.lblVersion.Text = "Version:   v4.6.0.11";
            // 
            // pnlTeaching
            // 
            this.pnlTeaching.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlTeaching.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(38)))), ((int)(((byte)(131)))));
            this.pnlTeaching.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnlTeaching.Controls.Add(this.listboxTeachingSubMenu);
            this.pnlTeaching.Location = new System.Drawing.Point(189, 85);
            this.pnlTeaching.Name = "pnlTeaching";
            this.pnlTeaching.Size = new System.Drawing.Size(260, 274);
            this.pnlTeaching.TabIndex = 99;
            // 
            // listboxTeachingSubMenu
            // 
            this.listboxTeachingSubMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(38)))), ((int)(((byte)(131)))));
            this.listboxTeachingSubMenu.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listboxTeachingSubMenu.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.listboxTeachingSubMenu.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listboxTeachingSubMenu.FormattingEnabled = true;
            this.listboxTeachingSubMenu.ItemHeight = 1;
            this.listboxTeachingSubMenu.Location = new System.Drawing.Point(0, 0);
            this.listboxTeachingSubMenu.Name = "listboxTeachingSubMenu";
            this.listboxTeachingSubMenu.Size = new System.Drawing.Size(260, 274);
            this.listboxTeachingSubMenu.TabIndex = 22;
            this.listboxTeachingSubMenu.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listboxTeachingSubMenu_DrawItem);
            this.listboxTeachingSubMenu.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.listboxTeachingSubMenu_MeasureItem);
            // 
            // pnlServiceLog
            // 
            this.pnlServiceLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlServiceLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(38)))), ((int)(((byte)(131)))));
            this.pnlServiceLog.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnlServiceLog.Controls.Add(this.button_PageDown);
            this.pnlServiceLog.Controls.Add(this.button_PageUp);
            this.pnlServiceLog.Controls.Add(this.listServiceLogs);
            this.pnlServiceLog.Location = new System.Drawing.Point(189, 85);
            this.pnlServiceLog.Name = "pnlServiceLog";
            this.pnlServiceLog.Size = new System.Drawing.Size(260, 274);
            this.pnlServiceLog.TabIndex = 7;
            // 
            // button_PageDown
            // 
            this.button_PageDown.AccessibleName = "  ";
            this.button_PageDown.BackColor = System.Drawing.Color.Transparent;
            this.button_PageDown.BackgroundImage = global::Tesla.Service.Properties.Resources.PageDownButton0;
            this.button_PageDown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_PageDown.Check = false;
            this.button_PageDown.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button_PageDown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.button_PageDown.Location = new System.Drawing.Point(225, 199);
            this.button_PageDown.Name = "button_PageDown";
            this.button_PageDown.Size = new System.Drawing.Size(32, 32);
            this.button_PageDown.TabIndex = 28;
            this.button_PageDown.Click += new System.EventHandler(this.button_PageDown_Click);
            // 
            // button_PageUp
            // 
            this.button_PageUp.AccessibleName = "  ";
            this.button_PageUp.BackColor = System.Drawing.Color.Transparent;
            this.button_PageUp.BackgroundImage = global::Tesla.Service.Properties.Resources.PageUpButton0;
            this.button_PageUp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_PageUp.Check = false;
            this.button_PageUp.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button_PageUp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.button_PageUp.Location = new System.Drawing.Point(225, 44);
            this.button_PageUp.Name = "button_PageUp";
            this.button_PageUp.Size = new System.Drawing.Size(32, 32);
            this.button_PageUp.TabIndex = 27;
            this.button_PageUp.Click += new System.EventHandler(this.button_PageUp_Click);
            // 
            // listServiceLogs
            // 
            this.listServiceLogs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(38)))), ((int)(((byte)(131)))));
            this.listServiceLogs.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listServiceLogs.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.listServiceLogs.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listServiceLogs.FormattingEnabled = true;
            this.listServiceLogs.ItemHeight = 25;
            this.listServiceLogs.Location = new System.Drawing.Point(0, 0);
            this.listServiceLogs.Name = "listServiceLogs";
            this.listServiceLogs.ShowScrollbar = false;
            this.listServiceLogs.Size = new System.Drawing.Size(219, 274);
            this.listServiceLogs.TabIndex = 2;
            this.listServiceLogs.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listServiceLogs_DrawItem);
            this.listServiceLogs.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.listServiceLogs_MeasureItem);
            this.listServiceLogs.SelectedIndexChanged += new System.EventHandler(this.listServiceLogs_SelectedIndexChanged);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel2.Controls.Add(this.listboxMenu);
            this.panel2.Location = new System.Drawing.Point(9, 85);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(190, 274);
            this.panel2.TabIndex = 6;
            // 
            // imgMainSelectionBridge
            // 
            this.imgMainSelectionBridge.BackgroundImage = global::Tesla.Service.Properties.Resources.MainMenuSelectionBridge;
            this.imgMainSelectionBridge.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.imgMainSelectionBridge.Location = new System.Drawing.Point(218, 86);
            this.imgMainSelectionBridge.Name = "imgMainSelectionBridge";
            this.imgMainSelectionBridge.Size = new System.Drawing.Size(64, 66);
            this.imgMainSelectionBridge.TabIndex = 0;
            this.imgMainSelectionBridge.TabStop = false;
            this.imgMainSelectionBridge.Visible = false;
            // 
            // pnlSettings
            // 
            this.pnlSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(38)))), ((int)(((byte)(131)))));
            this.pnlSettings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnlSettings.Controls.Add(this.button_BarcodeRescan);
            this.pnlSettings.Controls.Add(this.button_RestoreBackup);
            this.pnlSettings.Controls.Add(this.button_LoggingLevel);
            this.pnlSettings.Controls.Add(this.label2);
            this.pnlSettings.Controls.Add(this.button_RestoreFactory);
            this.pnlSettings.Controls.Add(this.button_SaveAsFactory);
            this.pnlSettings.Controls.Add(this.button_TimedStart);
            this.pnlSettings.Controls.Add(this.button_Lid);
            this.pnlSettings.Controls.Add(this.lblTimedStart);
            this.pnlSettings.Controls.Add(this.lblLidSensor);
            this.pnlSettings.ForeColor = System.Drawing.Color.Black;
            this.pnlSettings.Location = new System.Drawing.Point(189, 85);
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.Size = new System.Drawing.Size(260, 274);
            this.pnlSettings.TabIndex = 6;
            // 
            // button_BarcodeRescan
            // 
            this.button_BarcodeRescan.AccessibleName = "Set Barcode Offset";
            this.button_BarcodeRescan.BackColor = System.Drawing.Color.Transparent;
            this.button_BarcodeRescan.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_BarcodeRescan.BackgroundImage")));
            this.button_BarcodeRescan.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_BarcodeRescan.Check = false;
            this.button_BarcodeRescan.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button_BarcodeRescan.Font = new System.Drawing.Font("Frutiger LT Std 45 Light", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_BarcodeRescan.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.button_BarcodeRescan.Location = new System.Drawing.Point(3, 130);
            this.button_BarcodeRescan.Name = "button_BarcodeRescan";
            this.button_BarcodeRescan.Size = new System.Drawing.Size(275, 28);
            this.button_BarcodeRescan.TabIndex = 22;
            this.button_BarcodeRescan.Click += new System.EventHandler(this.button_BarcodeRescan_Click);
            // 
            // button_RestoreBackup
            // 
            this.button_RestoreBackup.AccessibleName = "Restore from Backup";
            this.button_RestoreBackup.BackColor = System.Drawing.Color.Transparent;
            this.button_RestoreBackup.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_RestoreBackup.BackgroundImage")));
            this.button_RestoreBackup.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_RestoreBackup.Check = false;
            this.button_RestoreBackup.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button_RestoreBackup.Font = new System.Drawing.Font("Frutiger LT Std 45 Light", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_RestoreBackup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.button_RestoreBackup.Location = new System.Drawing.Point(3, 166);
            this.button_RestoreBackup.Name = "button_RestoreBackup";
            this.button_RestoreBackup.Size = new System.Drawing.Size(275, 28);
            this.button_RestoreBackup.TabIndex = 21;
            this.button_RestoreBackup.Click += new System.EventHandler(this.button_RestoreBackup_Click);
            // 
            // button_LoggingLevel
            // 
            this.button_LoggingLevel.AccessibleName = "Enable";
            this.button_LoggingLevel.BackColor = System.Drawing.Color.Transparent;
            this.button_LoggingLevel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_LoggingLevel.BackgroundImage")));
            this.button_LoggingLevel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_LoggingLevel.Check = false;
            this.button_LoggingLevel.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button_LoggingLevel.Font = new System.Drawing.Font("Frutiger LT Std 45 Light", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_LoggingLevel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.button_LoggingLevel.Location = new System.Drawing.Point(154, 88);
            this.button_LoggingLevel.Name = "button_LoggingLevel";
            this.button_LoggingLevel.Size = new System.Drawing.Size(86, 32);
            this.button_LoggingLevel.TabIndex = 20;
            this.button_LoggingLevel.Click += new System.EventHandler(this.button_LoggingLevel_Click);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(15, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 24);
            this.label2.TabIndex = 19;
            this.label2.Text = "Logging Level";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button_RestoreFactory
            // 
            this.button_RestoreFactory.AccessibleName = "Restore to Factory";
            this.button_RestoreFactory.BackColor = System.Drawing.Color.Transparent;
            this.button_RestoreFactory.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_RestoreFactory.BackgroundImage")));
            this.button_RestoreFactory.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_RestoreFactory.Check = false;
            this.button_RestoreFactory.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button_RestoreFactory.Font = new System.Drawing.Font("Frutiger LT Std 45 Light", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_RestoreFactory.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.button_RestoreFactory.Location = new System.Drawing.Point(2, 203);
            this.button_RestoreFactory.Name = "button_RestoreFactory";
            this.button_RestoreFactory.Size = new System.Drawing.Size(275, 28);
            this.button_RestoreFactory.TabIndex = 18;
            this.button_RestoreFactory.Click += new System.EventHandler(this.btnRestoreWithFactory_Click);
            // 
            // button_SaveAsFactory
            // 
            this.button_SaveAsFactory.AccessibleName = "Save As Factory";
            this.button_SaveAsFactory.BackColor = System.Drawing.Color.Transparent;
            this.button_SaveAsFactory.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_SaveAsFactory.BackgroundImage")));
            this.button_SaveAsFactory.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_SaveAsFactory.Check = false;
            this.button_SaveAsFactory.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button_SaveAsFactory.Font = new System.Drawing.Font("Frutiger LT Std 45 Light", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_SaveAsFactory.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.button_SaveAsFactory.Location = new System.Drawing.Point(3, 239);
            this.button_SaveAsFactory.Name = "button_SaveAsFactory";
            this.button_SaveAsFactory.Size = new System.Drawing.Size(275, 28);
            this.button_SaveAsFactory.TabIndex = 17;
            this.button_SaveAsFactory.Click += new System.EventHandler(this.btnSaveAsFactory_Click);
            // 
            // button_TimedStart
            // 
            this.button_TimedStart.AccessibleName = "Enable";
            this.button_TimedStart.BackColor = System.Drawing.Color.Transparent;
            this.button_TimedStart.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_TimedStart.BackgroundImage")));
            this.button_TimedStart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_TimedStart.Check = false;
            this.button_TimedStart.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button_TimedStart.Font = new System.Drawing.Font("Frutiger LT Std 45 Light", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_TimedStart.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.button_TimedStart.Location = new System.Drawing.Point(154, 47);
            this.button_TimedStart.Name = "button_TimedStart";
            this.button_TimedStart.Size = new System.Drawing.Size(86, 32);
            this.button_TimedStart.TabIndex = 16;
            this.button_TimedStart.Click += new System.EventHandler(this.btnTimedStart_Click);
            // 
            // button_Lid
            // 
            this.button_Lid.AccessibleName = "Enable";
            this.button_Lid.BackColor = System.Drawing.Color.Transparent;
            this.button_Lid.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Lid.BackgroundImage")));
            this.button_Lid.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Lid.Check = false;
            this.button_Lid.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button_Lid.Font = new System.Drawing.Font("Frutiger LT Std 45 Light", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Lid.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.button_Lid.Location = new System.Drawing.Point(154, 5);
            this.button_Lid.Name = "button_Lid";
            this.button_Lid.Size = new System.Drawing.Size(86, 32);
            this.button_Lid.TabIndex = 15;
            this.button_Lid.Click += new System.EventHandler(this.btnLidSensor_Click);
            // 
            // lblTimedStart
            // 
            this.lblTimedStart.BackColor = System.Drawing.Color.Transparent;
            this.lblTimedStart.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTimedStart.ForeColor = System.Drawing.Color.White;
            this.lblTimedStart.Location = new System.Drawing.Point(14, 49);
            this.lblTimedStart.Name = "lblTimedStart";
            this.lblTimedStart.Size = new System.Drawing.Size(116, 24);
            this.lblTimedStart.TabIndex = 11;
            this.lblTimedStart.Text = "Timed Start";
            this.lblTimedStart.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLidSensor
            // 
            this.lblLidSensor.BackColor = System.Drawing.Color.Transparent;
            this.lblLidSensor.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLidSensor.ForeColor = System.Drawing.Color.White;
            this.lblLidSensor.Location = new System.Drawing.Point(14, 7);
            this.lblLidSensor.Name = "lblLidSensor";
            this.lblLidSensor.Size = new System.Drawing.Size(116, 24);
            this.lblLidSensor.TabIndex = 10;
            this.lblLidSensor.Text = "Lid Sensor";
            this.lblLidSensor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlExit
            // 
            this.pnlExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(38)))), ((int)(((byte)(131)))));
            this.pnlExit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnlExit.Controls.Add(this.button_Pendant);
            this.pnlExit.Controls.Add(this.lblPendant);
            this.pnlExit.Controls.Add(this.lblVersion);
            this.pnlExit.Location = new System.Drawing.Point(189, 85);
            this.pnlExit.Name = "pnlExit";
            this.pnlExit.Size = new System.Drawing.Size(260, 274);
            this.pnlExit.TabIndex = 7;
            // 
            // button_Pendant
            // 
            this.button_Pendant.AccessibleName = "Enable";
            this.button_Pendant.BackColor = System.Drawing.Color.Transparent;
            this.button_Pendant.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Pendant.BackgroundImage")));
            this.button_Pendant.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Pendant.Check = false;
            this.button_Pendant.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button_Pendant.Font = new System.Drawing.Font("Frutiger LT Std 45 Light", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Pendant.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.button_Pendant.Location = new System.Drawing.Point(146, 121);
            this.button_Pendant.Name = "button_Pendant";
            this.button_Pendant.Size = new System.Drawing.Size(86, 32);
            this.button_Pendant.TabIndex = 28;
            this.button_Pendant.Click += new System.EventHandler(this.button_Pendant_Click);
            // 
            // lblPendant
            // 
            this.lblPendant.BackColor = System.Drawing.Color.Transparent;
            this.lblPendant.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPendant.ForeColor = System.Drawing.Color.White;
            this.lblPendant.Location = new System.Drawing.Point(17, 123);
            this.lblPendant.Name = "lblPendant";
            this.lblPendant.Size = new System.Drawing.Size(116, 24);
            this.lblPendant.TabIndex = 27;
            this.lblPendant.Text = "Pendant Mode";
            this.lblPendant.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(200, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 23);
            this.label1.TabIndex = 8;
            this.label1.Text = "IP:";
            // 
            // lblConnect
            // 
            this.lblConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblConnect.BackColor = System.Drawing.Color.Black;
            this.lblConnect.Font = new System.Drawing.Font("Frutiger LT Std 45 Light", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConnect.ForeColor = System.Drawing.Color.White;
            this.lblConnect.Location = new System.Drawing.Point(337, 42);
            this.lblConnect.Name = "lblConnect";
            this.lblConnect.Size = new System.Drawing.Size(112, 24);
            this.lblConnect.TabIndex = 9;
            this.lblConnect.Text = "Disconnect";
            this.lblConnect.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblConnect.Visible = false;
            this.lblConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btn_close
            // 
            this.btn_close.AccessibleName = " ";
            this.btn_close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_close.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_close.BackgroundImage")));
            this.btn_close.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_close.Check = false;
            this.btn_close.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.btn_close.Location = new System.Drawing.Point(414, 4);
            this.btn_close.Name = "btn_close";
            this.btn_close.Size = new System.Drawing.Size(45, 45);
            this.btn_close.TabIndex = 10;
            this.btn_close.Click += new System.EventHandler(this.button1_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "hardware.ini";
            this.openFileDialog1.Filter = "All files (*.*)|*.*";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(6, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(162, 28);
            this.label3.TabIndex = 11;
            this.label3.Text = "Service Menu";
            // 
            // button_Connect
            // 
            this.button_Connect.AccessibleName = "Enable";
            this.button_Connect.BackColor = System.Drawing.Color.Transparent;
            this.button_Connect.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Connect.BackgroundImage")));
            this.button_Connect.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Connect.Check = false;
            this.button_Connect.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button_Connect.Font = new System.Drawing.Font("Frutiger LT Std 45 Light", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Connect.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.button_Connect.Location = new System.Drawing.Point(343, 46);
            this.button_Connect.Name = "button_Connect";
            this.button_Connect.Size = new System.Drawing.Size(86, 32);
            this.button_Connect.TabIndex = 22;
            this.button_Connect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // ServiceMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(456, 371);
            this.Controls.Add(this.pnlExit);
            this.Controls.Add(this.pnlSettings);
            this.Controls.Add(this.pnlServiceLog);
            this.Controls.Add(this.button_Connect);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btn_close);
            this.Controls.Add(this.imgMainSelectionBridge);
            this.Controls.Add(this.lblConnect);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.txtIpAddr);
            this.Controls.Add(this.pnlCalibrate);
            this.Controls.Add(this.pnlTeaching);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ServiceMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ServiceMenu";
            this.Load += new System.EventHandler(this.ServiceMenu_Load);
            this.Shown += new System.EventHandler(this.ServiceMenu_Shown);
            this.pnlCalibrate.ResumeLayout(false);
            this.pnlTeaching.ResumeLayout(false);
            this.pnlServiceLog.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imgMainSelectionBridge)).EndInit();
            this.pnlSettings.ResumeLayout(false);
            this.pnlExit.ResumeLayout(false);
            this.pnlExit.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listboxMenu;
        private System.Windows.Forms.ListBox listboxSubMenu;
        private System.Windows.Forms.TextBox txtIpAddr;
        private System.Windows.Forms.Panel pnlCalibrate;
        private System.Windows.Forms.Panel pnlTeaching;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox imgMainSelectionBridge;
        private System.Windows.Forms.Panel pnlSettings;
        private System.Windows.Forms.Panel pnlServiceLog;
        private System.Windows.Forms.Panel pnlExit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblConnect;
        private System.Windows.Forms.Label lblTimedStart;
        private System.Windows.Forms.Label lblLidSensor;
        private GUI_Controls.Button_Circle btn_close;
        private GUI_Controls.Button_Rectangle button_Lid;
        private GUI_Controls.Button_Rectangle button_TimedStart;
        private GUI_Controls.Button_Rectangle button_SaveAsFactory;
        private GUI_Controls.Button_Rectangle button_RestoreFactory;
        private GUI_Controls.Button_Rectangle button_LoggingLevel;
        private System.Windows.Forms.Label label2;
        private GUI_Controls.Button_Rectangle button_RestoreBackup;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label3;
        private GUI_Controls.Button_Rectangle button_Connect;
        private GUI_Controls.ListboxNoScroll listServiceLogs;
        private GUI_Controls.Button_Rectangle button_PageDown;
        private GUI_Controls.Button_Rectangle button_PageUp;
        private System.Windows.Forms.ListBox listboxTeachingSubMenu;
        private System.Windows.Forms.Label lblVersion;
        private GUI_Controls.Button_Rectangle button_BarcodeRescan;
        private GUI_Controls.Button_Rectangle button_Pendant;
        private System.Windows.Forms.Label lblPendant;
    }
}