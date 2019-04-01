namespace GUI_Console
{
    partial class RoboSep_Resources
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
            for (i = 0; i < ilist.Count; i++)
                ilist[i].Dispose();
            for (i = 0; i < iListRun1.Count; i++)
                iListRun1[i].Dispose();
            for (i = 0; i < iListRun2.Count; i++)
                iListRun2[i].Dispose();
            if (bmParticles != null)
                bmParticles.Dispose();
            if (bmCocktail != null)
                bmCocktail.Dispose();
            if (bmAntibodyVial != null)
                bmAntibodyVial.Dispose();
            if (bmSampleVial != null)
                bmSampleVial.Dispose();
            if (bmTipRack != null)
                bmTipRack.Dispose();
            if (bmSeparationTube != null)
                bmSeparationTube.Dispose();
            if (bmWasteFraction != null)
                bmWasteFraction.Dispose();
            if (bmNegativeFraction != null)
                bmNegativeFraction.Dispose();
            if (bmBufferBottle != null)
                bmBufferBottle.Dispose();

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoboSep_Resources));
            this.txtBoxLotID = new System.Windows.Forms.TextBox();
            this.lblLotID = new System.Windows.Forms.Label();
            this.pictureCarouselNumber = new System.Windows.Forms.PictureBox();
            this.picturebox_Buffer = new System.Windows.Forms.PictureBox();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.RunActivationTimer = new System.Windows.Forms.Timer(this.components);
            this.pictureBox_SampleVial = new System.Windows.Forms.PictureBox();
            this.pictureBox_SeparationVial = new System.Windows.Forms.PictureBox();
            this.pictureBox_1mL = new System.Windows.Forms.PictureBox();
            this.pictureBox_WasteTube = new System.Windows.Forms.PictureBox();
            this.pictureBox_TipRack = new System.Windows.Forms.PictureBox();
            this.label_protocolName = new System.Windows.Forms.Label();
            this.label_Description = new System.Windows.Forms.Label();
            this.buttonRun = new GUI_Controls.Button_Circle();
            this.UserNameHeader = new GUI_Controls.nameHeader();
            this.btn_home = new GUI_Controls.btn_home2();
            this.button_Scan = new GUI_Controls.Button_Circle();
            this.button_ScrollUp = new GUI_Controls.Button_Scroll();
            this.button_ScrollDown = new GUI_Controls.Button_Scroll();
            this.listView_Resources = new GUI_Controls.DragScrollListView2();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button_Quadrant4 = new GUI_Controls.Button_Quadrant();
            this.button_Quadrant1 = new GUI_Controls.Button_Quadrant();
            this.button_Quadrant3 = new GUI_Controls.Button_Quadrant();
            this.button_Quadrant2 = new GUI_Controls.Button_Quadrant();
            ((System.ComponentModel.ISupportInitialize)(this.pictureCarouselNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picturebox_Buffer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_SampleVial)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_SeparationVial)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_1mL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_WasteTube)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_TipRack)).BeginInit();
            this.SuspendLayout();
            // 
            // txtBoxLotID
            // 
            this.txtBoxLotID.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.txtBoxLotID.Location = new System.Drawing.Point(466, 39);
            this.txtBoxLotID.Name = "txtBoxLotID";
            this.txtBoxLotID.Size = new System.Drawing.Size(171, 30);
            this.txtBoxLotID.TabIndex = 30;
            this.txtBoxLotID.MouseClick += new System.Windows.Forms.MouseEventHandler(this.txtBoxLotID_MouseClick);
            this.txtBoxLotID.TextChanged += new System.EventHandler(this.txtBoxLotID_TextChanged);
            // 
            // lblLotID
            // 
            this.lblLotID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLotID.BackColor = System.Drawing.Color.Transparent;
            this.lblLotID.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold);
            this.lblLotID.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.lblLotID.Location = new System.Drawing.Point(307, 42);
            this.lblLotID.Name = "lblLotID";
            this.lblLotID.Size = new System.Drawing.Size(150, 24);
            this.lblLotID.TabIndex = 31;
            this.lblLotID.Text = "Reagent Lot ID:";
            this.lblLotID.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // pictureCarouselNumber
            // 
            this.pictureCarouselNumber.Location = new System.Drawing.Point(409, 272);
            this.pictureCarouselNumber.Name = "pictureCarouselNumber";
            this.pictureCarouselNumber.Size = new System.Drawing.Size(33, 32);
            this.pictureCarouselNumber.TabIndex = 32;
            this.pictureCarouselNumber.TabStop = false;
            // 
            // picturebox_Buffer
            // 
            this.picturebox_Buffer.Image = global::GUI_Console.Properties.Resources.bottle;
            this.picturebox_Buffer.Location = new System.Drawing.Point(230, 156);
            this.picturebox_Buffer.Name = "picturebox_Buffer";
            this.picturebox_Buffer.Size = new System.Drawing.Size(100, 211);
            this.picturebox_Buffer.TabIndex = 58;
            this.picturebox_Buffer.TabStop = false;
            this.picturebox_Buffer.Visible = false;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(70, 70);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // RunActivationTimer
            // 
            this.RunActivationTimer.Tick += new System.EventHandler(this.RunActivationTimer_Tick);
            // 
            // pictureBox_SampleVial
            // 
            this.pictureBox_SampleVial.Image = global::GUI_Console.Properties.Resources.sample;
            this.pictureBox_SampleVial.Location = new System.Drawing.Point(259, 165);
            this.pictureBox_SampleVial.Name = "pictureBox_SampleVial";
            this.pictureBox_SampleVial.Size = new System.Drawing.Size(51, 211);
            this.pictureBox_SampleVial.TabIndex = 69;
            this.pictureBox_SampleVial.TabStop = false;
            this.pictureBox_SampleVial.Visible = false;
            // 
            // pictureBox_SeparationVial
            // 
            this.pictureBox_SeparationVial.Image = global::GUI_Console.Properties.Resources.separation;
            this.pictureBox_SeparationVial.Location = new System.Drawing.Point(259, 165);
            this.pictureBox_SeparationVial.Name = "pictureBox_SeparationVial";
            this.pictureBox_SeparationVial.Size = new System.Drawing.Size(51, 211);
            this.pictureBox_SeparationVial.TabIndex = 76;
            this.pictureBox_SeparationVial.TabStop = false;
            this.pictureBox_SeparationVial.Visible = false;
            // 
            // pictureBox_1mL
            // 
            this.pictureBox_1mL.Image = global::GUI_Console.Properties.Resources.OneML;
            this.pictureBox_1mL.Location = new System.Drawing.Point(265, 164);
            this.pictureBox_1mL.Name = "pictureBox_1mL";
            this.pictureBox_1mL.Size = new System.Drawing.Size(39, 211);
            this.pictureBox_1mL.TabIndex = 70;
            this.pictureBox_1mL.TabStop = false;
            this.pictureBox_1mL.Visible = false;
            // 
            // pictureBox_WasteTube
            // 
            this.pictureBox_WasteTube.Image = global::GUI_Console.Properties.Resources.waste;
            this.pictureBox_WasteTube.Location = new System.Drawing.Point(259, 165);
            this.pictureBox_WasteTube.Name = "pictureBox_WasteTube";
            this.pictureBox_WasteTube.Size = new System.Drawing.Size(55, 211);
            this.pictureBox_WasteTube.TabIndex = 71;
            this.pictureBox_WasteTube.TabStop = false;
            this.pictureBox_WasteTube.Visible = false;
            // 
            // pictureBox_TipRack
            // 
            this.pictureBox_TipRack.Image = global::GUI_Console.Properties.Resources.tipRack;
            this.pictureBox_TipRack.Location = new System.Drawing.Point(242, 164);
            this.pictureBox_TipRack.Name = "pictureBox_TipRack";
            this.pictureBox_TipRack.Size = new System.Drawing.Size(85, 211);
            this.pictureBox_TipRack.TabIndex = 72;
            this.pictureBox_TipRack.TabStop = false;
            this.pictureBox_TipRack.Visible = false;
            // 
            // label_protocolName
            // 
            this.label_protocolName.BackColor = System.Drawing.Color.Transparent;
            this.label_protocolName.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_protocolName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.label_protocolName.Location = new System.Drawing.Point(254, 402);
            this.label_protocolName.Name = "label_protocolName";
            this.label_protocolName.Size = new System.Drawing.Size(151, 58);
            this.label_protocolName.TabIndex = 74;
            this.label_protocolName.Text = "label1";
            // 
            // label_Description
            // 
            this.label_Description.AutoEllipsis = true;
            this.label_Description.BackColor = System.Drawing.Color.Transparent;
            this.label_Description.Font = new System.Drawing.Font("Arial", 8.25F);
            this.label_Description.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.label_Description.Location = new System.Drawing.Point(337, 329);
            this.label_Description.Name = "label_Description";
            this.label_Description.Size = new System.Drawing.Size(95, 46);
            this.label_Description.TabIndex = 75;
            this.label_Description.Text = "label2";
            // 
            // buttonRun
            // 
            this.buttonRun.BackColor = System.Drawing.Color.Transparent;
            this.buttonRun.BackgroundImage = global::GUI_Console.Properties.Resources.GE_BTN15L_run_sample_STD;
            this.buttonRun.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.buttonRun.Check = false;
            this.buttonRun.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRun.ForeColor = System.Drawing.Color.White;
            this.buttonRun.Location = new System.Drawing.Point(531, 390);
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Size = new System.Drawing.Size(104, 86);
            this.buttonRun.TabIndex = 79;
            this.buttonRun.Text = "button_Circle1";
            this.buttonRun.Click += new System.EventHandler(this.buttonRun_Click);
            // 
            // UserNameHeader
            // 
            this.UserNameHeader.BackColor = System.Drawing.Color.Transparent;
            this.UserNameHeader.BackgroundImage = global::GUI_Console.Properties.Resources.User_HeaderSml2;
            this.UserNameHeader.Location = new System.Drawing.Point(285, 1);
            this.UserNameHeader.Name = "UserNameHeader";
            this.UserNameHeader.Size = new System.Drawing.Size(353, 37);
            this.UserNameHeader.TabIndex = 78;
            // 
            // btn_home
            // 
            this.btn_home.BackColor = System.Drawing.Color.Transparent;
            this.btn_home.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_home.BackgroundImage")));
            this.btn_home.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_home.Check = false;
            this.btn_home.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_home.ForeColor = System.Drawing.Color.White;
            this.btn_home.Location = new System.Drawing.Point(8, 417);
            this.btn_home.Name = "btn_home";
            this.btn_home.Size = new System.Drawing.Size(51, 51);
            this.btn_home.TabIndex = 77;
            this.btn_home.Text = "btn_home";
            this.btn_home.Click += new System.EventHandler(this.btn_home_Click);
            // 
            // button_Scan
            // 
            this.button_Scan.BackColor = System.Drawing.Color.Transparent;
            this.button_Scan.BackgroundImage = global::GUI_Console.Properties.Resources.RE_BTN16L_scan_STD;
            this.button_Scan.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Scan.Check = false;
            this.button_Scan.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Scan.ForeColor = System.Drawing.Color.White;
            this.button_Scan.Location = new System.Drawing.Point(420, 390);
            this.button_Scan.Name = "button_Scan";
            this.button_Scan.Size = new System.Drawing.Size(104, 86);
            this.button_Scan.TabIndex = 73;
            this.button_Scan.Text = "Run Protocols";
            this.button_Scan.Click += new System.EventHandler(this.button_Scan_Click);
            // 
            // button_ScrollUp
            // 
            this.button_ScrollUp.BackColor = System.Drawing.Color.Transparent;
            this.button_ScrollUp.BackgroundImage = global::GUI_Console.Properties.Resources.RE_BTN13N_up_arrow_STD;
            this.button_ScrollUp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_ScrollUp.Check = false;
            this.button_ScrollUp.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_ScrollUp.ForeColor = System.Drawing.Color.White;
            this.button_ScrollUp.Location = new System.Drawing.Point(89, 32);
            this.button_ScrollUp.Name = "button_ScrollUp";
            this.button_ScrollUp.Size = new System.Drawing.Size(40, 40);
            this.button_ScrollUp.TabIndex = 64;
            this.button_ScrollUp.Text = "  ";
            this.button_ScrollUp.Click += new System.EventHandler(this.button_ScrollUp_Click);
            // 
            // button_ScrollDown
            // 
            this.button_ScrollDown.BackColor = System.Drawing.Color.Transparent;
            this.button_ScrollDown.BackgroundImage = global::GUI_Console.Properties.Resources.RE_BTN14N_down_arrow_STD;
            this.button_ScrollDown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_ScrollDown.Check = false;
            this.button_ScrollDown.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_ScrollDown.ForeColor = System.Drawing.Color.White;
            this.button_ScrollDown.Location = new System.Drawing.Point(87, 372);
            this.button_ScrollDown.Name = "button_ScrollDown";
            this.button_ScrollDown.Size = new System.Drawing.Size(40, 40);
            this.button_ScrollDown.TabIndex = 65;
            this.button_ScrollDown.Text = "  ";
            this.button_ScrollDown.Click += new System.EventHandler(this.button_ScrollDown_Click);
            // 
            // listView_Resources
            // 
            this.listView_Resources.BackColor = System.Drawing.SystemColors.Control;
            this.listView_Resources.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listView_Resources.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listView_Resources.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listView_Resources.FullRowSelect = true;
            this.listView_Resources.GridLines = true;
            this.listView_Resources.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView_Resources.HideSelection = false;
            this.listView_Resources.Location = new System.Drawing.Point(21, 78);
            this.listView_Resources.MultiSelect = false;
            this.listView_Resources.Name = "listView_Resources";
            this.listView_Resources.OwnerDraw = true;
            this.listView_Resources.Size = new System.Drawing.Size(176, 288);
            this.listView_Resources.SmallImageList = this.imageList1;
            this.listView_Resources.TabIndex = 59;
            this.listView_Resources.UseCompatibleStateImageBehavior = false;
            this.listView_Resources.View = System.Windows.Forms.View.Details;
            this.listView_Resources.VisibleRow = 4;
            this.listView_Resources.VScrollbar = null;
            this.listView_Resources.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.listView_Resources_DrawItem);
            this.listView_Resources.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.listView_Resources_DrawSubItem);
            this.listView_Resources.Click += new System.EventHandler(this.listView_Resources_Click);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 120;
            // 
            // button_Quadrant4
            // 
            this.button_Quadrant4.BackColor = System.Drawing.Color.Transparent;
            this.button_Quadrant4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Quadrant4.BackgroundImage")));
            this.button_Quadrant4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Quadrant4.Check = false;
            this.button_Quadrant4.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Quadrant4.ForeColor = System.Drawing.Color.White;
            this.button_Quadrant4.Location = new System.Drawing.Point(200, 420);
            this.button_Quadrant4.Name = "button_Quadrant4";
            this.button_Quadrant4.NowInProgress = false;
            this.button_Quadrant4.Size = new System.Drawing.Size(40, 40);
            this.button_Quadrant4.TabIndex = 18;
            this.button_Quadrant4.Text = "4";
            this.button_Quadrant4.Click += new System.EventHandler(this.button_Quadrant4_Click);
            // 
            // button_Quadrant1
            // 
            this.button_Quadrant1.BackColor = System.Drawing.Color.Transparent;
            this.button_Quadrant1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Quadrant1.BackgroundImage")));
            this.button_Quadrant1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Quadrant1.Check = false;
            this.button_Quadrant1.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Quadrant1.ForeColor = System.Drawing.Color.White;
            this.button_Quadrant1.Location = new System.Drawing.Point(66, 420);
            this.button_Quadrant1.Name = "button_Quadrant1";
            this.button_Quadrant1.NowInProgress = false;
            this.button_Quadrant1.Size = new System.Drawing.Size(40, 40);
            this.button_Quadrant1.TabIndex = 15;
            this.button_Quadrant1.Text = "1";
            this.button_Quadrant1.Click += new System.EventHandler(this.button_Quadrant1_Click);
            // 
            // button_Quadrant3
            // 
            this.button_Quadrant3.BackColor = System.Drawing.Color.Transparent;
            this.button_Quadrant3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Quadrant3.BackgroundImage")));
            this.button_Quadrant3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Quadrant3.Check = false;
            this.button_Quadrant3.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Quadrant3.ForeColor = System.Drawing.Color.White;
            this.button_Quadrant3.Location = new System.Drawing.Point(157, 420);
            this.button_Quadrant3.Name = "button_Quadrant3";
            this.button_Quadrant3.NowInProgress = false;
            this.button_Quadrant3.Size = new System.Drawing.Size(40, 40);
            this.button_Quadrant3.TabIndex = 17;
            this.button_Quadrant3.Text = "3";
            this.button_Quadrant3.Click += new System.EventHandler(this.button_Quadrant3_Click);
            // 
            // button_Quadrant2
            // 
            this.button_Quadrant2.BackColor = System.Drawing.Color.Transparent;
            this.button_Quadrant2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Quadrant2.BackgroundImage")));
            this.button_Quadrant2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Quadrant2.Check = false;
            this.button_Quadrant2.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Quadrant2.ForeColor = System.Drawing.Color.White;
            this.button_Quadrant2.Location = new System.Drawing.Point(111, 420);
            this.button_Quadrant2.Name = "button_Quadrant2";
            this.button_Quadrant2.NowInProgress = false;
            this.button_Quadrant2.Size = new System.Drawing.Size(40, 40);
            this.button_Quadrant2.TabIndex = 16;
            this.button_Quadrant2.Text = "2";
            this.button_Quadrant2.Click += new System.EventHandler(this.button_Quadrant2_Click);
            // 
            // RoboSep_Resources
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this.BackgroundImage = global::GUI_Console.Properties.Resources.Background_Resources;
            this.Controls.Add(this.buttonRun);
            this.Controls.Add(this.UserNameHeader);
            this.Controls.Add(this.btn_home);
            this.Controls.Add(this.label_Description);
            this.Controls.Add(this.label_protocolName);
            this.Controls.Add(this.button_Scan);
            this.Controls.Add(this.pictureBox_TipRack);
            this.Controls.Add(this.pictureBox_WasteTube);
            this.Controls.Add(this.pictureBox_1mL);
            this.Controls.Add(this.pictureBox_SampleVial);
            this.Controls.Add(this.pictureBox_SeparationVial);
            this.Controls.Add(this.button_ScrollUp);
            this.Controls.Add(this.button_ScrollDown);
            this.Controls.Add(this.listView_Resources);
            this.Controls.Add(this.picturebox_Buffer);
            this.Controls.Add(this.pictureCarouselNumber);
            this.Controls.Add(this.lblLotID);
            this.Controls.Add(this.txtBoxLotID);
            this.Controls.Add(this.button_Quadrant4);
            this.Controls.Add(this.button_Quadrant1);
            this.Controls.Add(this.button_Quadrant3);
            this.Controls.Add(this.button_Quadrant2);
            this.Name = "RoboSep_Resources";
            this.Size = new System.Drawing.Size(640, 480);
            this.Load += new System.EventHandler(this.RoboSep_Resources_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureCarouselNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picturebox_Buffer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_SampleVial)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_SeparationVial)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_1mL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_WasteTube)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_TipRack)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GUI_Controls.Button_Quadrant button_Quadrant4;
        private GUI_Controls.Button_Quadrant button_Quadrant3;
        private GUI_Controls.Button_Quadrant button_Quadrant2;
        private GUI_Controls.Button_Quadrant button_Quadrant1;
        private System.Windows.Forms.TextBox txtBoxLotID;
        private System.Windows.Forms.Label lblLotID;
        private System.Windows.Forms.PictureBox pictureCarouselNumber;
        private System.Windows.Forms.PictureBox picturebox_Buffer;
        private GUI_Controls.DragScrollListView2 listView_Resources;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private GUI_Controls.Button_Scroll button_ScrollUp;
        private GUI_Controls.Button_Scroll button_ScrollDown;
        private System.Windows.Forms.Timer RunActivationTimer;
        private System.Windows.Forms.PictureBox pictureBox_SampleVial;
        private System.Windows.Forms.PictureBox pictureBox_SeparationVial;
        private System.Windows.Forms.PictureBox pictureBox_1mL;
        private System.Windows.Forms.PictureBox pictureBox_WasteTube;
        private System.Windows.Forms.PictureBox pictureBox_TipRack;
        private GUI_Controls.Button_Circle button_Scan;
        private System.Windows.Forms.Label label_protocolName;
        private System.Windows.Forms.Label label_Description;
        private GUI_Controls.btn_home2 btn_home;
        private GUI_Controls.nameHeader UserNameHeader;
        private GUI_Controls.Button_Circle buttonRun;
    }
}
