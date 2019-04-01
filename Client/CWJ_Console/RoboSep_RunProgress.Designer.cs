namespace GUI_Console
{
    partial class RoboSep_RunProgress
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
            
            for (i = 0; i < iListPause.Count; i++)
                iListPause[i].Dispose();
            for (i = 0; i < iListUnload.Count; i++)
                iListUnload[i].Dispose();
            for (i = 0; i < iListLock.Count; i++)
                iListLock[i].Dispose();
            for (i = 0; i < iListUnlock.Count; i++)
                iListUnlock[i].Dispose();

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoboSep_RunProgress));
            this.labelQ4_protocolName = new System.Windows.Forms.Label();
            this.labelQ3_protocolName = new System.Windows.Forms.Label();
            this.labelQ2_protocolName = new System.Windows.Forms.Label();
            this.labelQ1_protocolName = new System.Windows.Forms.Label();
            this.labelEstimated = new GUI_Controls.TextBoxScroll();
            this.labelElapsed = new GUI_Controls.TextBoxScroll();
            this.labelQ4_previous = new GUI_Controls.TextBoxScroll();
            this.labelQ4_current = new GUI_Controls.TextBoxScroll();
            this.labelQ3_previous = new GUI_Controls.TextBoxScroll();
            this.labelQ3_current = new GUI_Controls.TextBoxScroll();
            this.labelQ2_previous = new GUI_Controls.TextBoxScroll();
            this.labelQ2_current = new GUI_Controls.TextBoxScroll();
            this.labelQ1_previous = new GUI_Controls.TextBoxScroll();
            this.labelQ1_current = new GUI_Controls.TextBoxScroll();
            this.ALLComplete = new System.Windows.Forms.Label();
            this.lblQ4Complete = new System.Windows.Forms.Label();
            this.lblQ3Complete = new System.Windows.Forms.Label();
            this.lblQ2Complete = new System.Windows.Forms.Label();
            this.Q4progress = new GUI_Controls.ProgressBar();
            this.ALLprogress = new GUI_Controls.ProgressBar();
            this.estimated = new System.Windows.Forms.Label();
            this.lblQ4previous = new System.Windows.Forms.Label();
            this.lblQ3previous = new System.Windows.Forms.Label();
            this.Q2progress = new GUI_Controls.ProgressBar();
            this.lblQ2previous = new System.Windows.Forms.Label();
            this.Q3progress = new GUI_Controls.ProgressBar();
            this.lblQ1Complete = new System.Windows.Forms.Label();
            this.Q1progress = new GUI_Controls.ProgressBar();
            this.elapsed = new System.Windows.Forms.Label();
            this.AllQuadrants = new System.Windows.Forms.PictureBox();
            this.Q4 = new GUI_Controls.Button_Quadrant();
            this.Q3 = new GUI_Controls.Button_Quadrant();
            this.Q2 = new GUI_Controls.Button_Quadrant();
            this.Q1 = new GUI_Controls.Button_Quadrant();
            this.lblQ2Current = new System.Windows.Forms.Label();
            this.lblQ3Current = new System.Windows.Forms.Label();
            this.lblQ4Current = new System.Windows.Forms.Label();
            this.lblQ1previous = new System.Windows.Forms.Label();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.CWJ_Timer = new System.Windows.Forms.Timer(this.components);
            this.PauseTimer = new System.Windows.Forms.Timer(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblUsrProtocols = new System.Windows.Forms.Label();
            this.button_Abort = new GUI_Controls.Button_Circle();
            this.button_Pause = new GUI_Controls.Button_Circle();
            this.button_RemoteDeskTop = new GUI_Controls.GUIButton();
            this.UserNameHeader = new GUI_Controls.nameHeader();
            this.lblQ1Current = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.AllQuadrants)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_home
            // 
            this.btn_home.Location = new System.Drawing.Point(13, 418);
            // 
            // labelQ4_protocolName
            // 
            this.labelQ4_protocolName.BackColor = System.Drawing.Color.Transparent;
            this.labelQ4_protocolName.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelQ4_protocolName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.labelQ4_protocolName.Location = new System.Drawing.Point(84, 366);
            this.labelQ4_protocolName.Name = "labelQ4_protocolName";
            this.labelQ4_protocolName.Size = new System.Drawing.Size(533, 14);
            this.labelQ4_protocolName.TabIndex = 55;
            this.labelQ4_protocolName.Text = "label2";
            this.labelQ4_protocolName.Visible = false;
            // 
            // labelQ3_protocolName
            // 
            this.labelQ3_protocolName.BackColor = System.Drawing.Color.Transparent;
            this.labelQ3_protocolName.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelQ3_protocolName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.labelQ3_protocolName.Location = new System.Drawing.Point(84, 313);
            this.labelQ3_protocolName.Name = "labelQ3_protocolName";
            this.labelQ3_protocolName.Size = new System.Drawing.Size(533, 14);
            this.labelQ3_protocolName.TabIndex = 54;
            this.labelQ3_protocolName.Text = "label1";
            this.labelQ3_protocolName.Visible = false;
            // 
            // labelQ2_protocolName
            // 
            this.labelQ2_protocolName.BackColor = System.Drawing.Color.Transparent;
            this.labelQ2_protocolName.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelQ2_protocolName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.labelQ2_protocolName.Location = new System.Drawing.Point(84, 260);
            this.labelQ2_protocolName.Name = "labelQ2_protocolName";
            this.labelQ2_protocolName.Size = new System.Drawing.Size(533, 14);
            this.labelQ2_protocolName.TabIndex = 53;
            this.labelQ2_protocolName.Text = "label1";
            this.labelQ2_protocolName.Visible = false;
            // 
            // labelQ1_protocolName
            // 
            this.labelQ1_protocolName.BackColor = System.Drawing.Color.Transparent;
            this.labelQ1_protocolName.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelQ1_protocolName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.labelQ1_protocolName.Location = new System.Drawing.Point(84, 207);
            this.labelQ1_protocolName.Name = "labelQ1_protocolName";
            this.labelQ1_protocolName.Size = new System.Drawing.Size(533, 14);
            this.labelQ1_protocolName.TabIndex = 6;
            this.labelQ1_protocolName.Text = "label1";
            this.labelQ1_protocolName.Visible = false;
            // 
            // labelEstimated
            // 
            this.labelEstimated.AutoSize = false;
            this.labelEstimated.BackColor = System.Drawing.Color.Transparent;
            this.labelEstimated.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEstimated.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(38)))), ((int)(((byte)(131)))));
            this.labelEstimated.Location = new System.Drawing.Point(451, 138);
            this.labelEstimated.MaximumSize = new System.Drawing.Size(182, 20);
            this.labelEstimated.Name = "labelEstimated";
            this.labelEstimated.ScrollingText_Color1 = System.Drawing.Color.Black;
            this.labelEstimated.ScrollingTextColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.labelEstimated.Size = new System.Drawing.Size(175, 20);
            this.labelEstimated.MaximumSize = new System.Drawing.Size(180, 20);
            this.labelEstimated.TabIndex = 47;
            // 
            // labelElapsed
            // 
            this.labelElapsed.AutoSize = false;
            this.labelElapsed.BackColor = System.Drawing.Color.Transparent;
            this.labelElapsed.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelElapsed.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(38)))), ((int)(((byte)(131)))));
            this.labelElapsed.Location = new System.Drawing.Point(451, 119);
            this.labelElapsed.Name = "labelElapsed";
            this.labelElapsed.ScrollingText_Color1 = System.Drawing.Color.Black;
            this.labelElapsed.ScrollingTextColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.labelElapsed.Size = new System.Drawing.Size(175, 20);
            this.labelElapsed.MaximumSize = new System.Drawing.Size(180, 20);
            this.labelElapsed.TabIndex = 46;
            // 
            // labelQ4_previous
            // 
            this.labelQ4_previous.AutoSize = false;
            this.labelQ4_previous.BackColor = System.Drawing.Color.Transparent;
            this.labelQ4_previous.Font = new System.Drawing.Font("Arial Narrow", 12F);
            this.labelQ4_previous.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.labelQ4_previous.Location = new System.Drawing.Point(448, 346);
            this.labelQ4_previous.Name = "labelQ4_previous";
            this.labelQ4_previous.ScrollingText_Color1 = System.Drawing.Color.Black;
            this.labelQ4_previous.ScrollingTextColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.labelQ4_previous.Size = new System.Drawing.Size(175, 20);
            this.labelQ4_previous.MaximumSize = new System.Drawing.Size(180, 20);
            this.labelQ4_previous.TabIndex = 45;
            // 
            // labelQ4_current
            // 
            this.labelQ4_current.AutoSize = false;
            this.labelQ4_current.BackColor = System.Drawing.Color.Transparent;
            this.labelQ4_current.Font = new System.Drawing.Font("Arial Narrow", 12F);
            this.labelQ4_current.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.labelQ4_current.Location = new System.Drawing.Point(448, 327);
            this.labelQ4_current.Name = "labelQ4_current";
            this.labelQ4_current.ScrollingText_Color1 = System.Drawing.Color.Black;
            this.labelQ4_current.ScrollingTextColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.labelQ4_current.Size = new System.Drawing.Size(175, 20);
            this.labelQ4_current.MaximumSize = new System.Drawing.Size(180, 20);
            this.labelQ4_current.TabIndex = 44;
            // 
            // labelQ3_previous
            // 
            this.labelQ3_previous.AutoSize = false;
            this.labelQ3_previous.BackColor = System.Drawing.Color.Transparent;
            this.labelQ3_previous.Font = new System.Drawing.Font("Arial Narrow", 12F);
            this.labelQ3_previous.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.labelQ3_previous.Location = new System.Drawing.Point(448, 293);
            this.labelQ3_previous.Name = "labelQ3_previous";
            this.labelQ3_previous.ScrollingText_Color1 = System.Drawing.Color.Black;
            this.labelQ3_previous.ScrollingTextColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.labelQ3_previous.Size = new System.Drawing.Size(175, 20);
            this.labelQ3_previous.MaximumSize = new System.Drawing.Size(180, 20);
            this.labelQ3_previous.TabIndex = 43;
            // 
            // labelQ3_current
            // 
            this.labelQ3_current.AutoSize = false;
            this.labelQ3_current.BackColor = System.Drawing.Color.Transparent;
            this.labelQ3_current.Font = new System.Drawing.Font("Arial Narrow", 12F);
            this.labelQ3_current.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.labelQ3_current.Location = new System.Drawing.Point(448, 274);
            this.labelQ3_current.Name = "labelQ3_current";
            this.labelQ3_current.ScrollingText_Color1 = System.Drawing.Color.Black;
            this.labelQ3_current.ScrollingTextColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.labelQ3_current.Size = new System.Drawing.Size(175, 20);
            this.labelQ3_current.MaximumSize = new System.Drawing.Size(180, 20);
            this.labelQ3_current.TabIndex = 42;
            // 
            // labelQ2_previous
            // 
            this.labelQ2_previous.AutoSize = false;
            this.labelQ2_previous.BackColor = System.Drawing.Color.Transparent;
            this.labelQ2_previous.Font = new System.Drawing.Font("Arial Narrow", 12F);
            this.labelQ2_previous.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.labelQ2_previous.Location = new System.Drawing.Point(448, 240);
            this.labelQ2_previous.Name = "labelQ2_previous";
            this.labelQ2_previous.ScrollingText_Color1 = System.Drawing.Color.Black;
            this.labelQ2_previous.ScrollingTextColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.labelQ2_previous.Size = new System.Drawing.Size(175, 20);
            this.labelQ2_previous.MaximumSize = new System.Drawing.Size(180, 20);
            this.labelQ2_previous.TabIndex = 41;
            // 
            // labelQ2_current
            // 
            this.labelQ2_current.AutoSize = false;
            this.labelQ2_current.BackColor = System.Drawing.Color.Transparent;
            this.labelQ2_current.Font = new System.Drawing.Font("Arial Narrow", 12F);
            this.labelQ2_current.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.labelQ2_current.Location = new System.Drawing.Point(448, 221);
            this.labelQ2_current.Name = "labelQ2_current";
            this.labelQ2_current.ScrollingText_Color1 = System.Drawing.Color.Black;
            this.labelQ2_current.ScrollingTextColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.labelQ2_current.Size = new System.Drawing.Size(175, 20);
            this.labelQ2_current.MaximumSize = new System.Drawing.Size(180, 20);
            this.labelQ2_current.TabIndex = 40;
            // 
            // labelQ1_previous
            // 
            this.labelQ1_previous.AutoSize = false;
            this.labelQ1_previous.BackColor = System.Drawing.Color.Transparent;
            this.labelQ1_previous.Font = new System.Drawing.Font("Arial Narrow", 12F);
            this.labelQ1_previous.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.labelQ1_previous.Location = new System.Drawing.Point(448, 187);
            this.labelQ1_previous.Name = "labelQ1_previous";
            this.labelQ1_previous.ScrollingText_Color1 = System.Drawing.Color.Black;
            this.labelQ1_previous.ScrollingTextColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.labelQ1_previous.Size = new System.Drawing.Size(175, 20);
            this.labelQ1_previous.MaximumSize = new System.Drawing.Size(180, 20);
            this.labelQ1_previous.TabIndex = 39;
            // 
            // labelQ1_current
            // 
            this.labelQ1_current.AutoSize = false;
            this.labelQ1_current.BackColor = System.Drawing.Color.Transparent;
            this.labelQ1_current.Font = new System.Drawing.Font("Arial Narrow", 12F);
            this.labelQ1_current.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.labelQ1_current.Location = new System.Drawing.Point(448, 168);
            this.labelQ1_current.Name = "labelQ1_current";
            this.labelQ1_current.ScrollingText_Color1 = System.Drawing.Color.Black;
            this.labelQ1_current.ScrollingTextColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.labelQ1_current.Size = new System.Drawing.Size(175, 20);
            this.labelQ1_current.MaximumSize = new System.Drawing.Size(180, 20);
            this.labelQ1_current.TabIndex = 38;
            // 
            // ALLComplete
            // 
            this.ALLComplete.AutoSize = false;
            this.ALLComplete.BackColor = System.Drawing.Color.Transparent;
            this.ALLComplete.Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ALLComplete.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(38)))), ((int)(((byte)(131)))));
            this.ALLComplete.Location = new System.Drawing.Point(377, 127);
            this.ALLComplete.Name = "ALLComplete";
            this.ALLComplete.Size = new System.Drawing.Size(87, 23);
            this.ALLComplete.TabIndex = 52;
            this.ALLComplete.Text = "Complete!";
            this.ALLComplete.Visible = false;
            // 
            // lblQ4Complete
            // 
            this.lblQ4Complete.AutoSize = true;
            this.lblQ4Complete.BackColor = System.Drawing.Color.Transparent;
            this.lblQ4Complete.Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQ4Complete.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(38)))), ((int)(((byte)(131)))));
            this.lblQ4Complete.Location = new System.Drawing.Point(377, 336);
            this.lblQ4Complete.Name = "lblQ4Complete";
            this.lblQ4Complete.Size = new System.Drawing.Size(87, 23);
            this.lblQ4Complete.TabIndex = 51;
            this.lblQ4Complete.Text = "Complete!";
            this.lblQ4Complete.Visible = false;
            // 
            // lblQ3Complete
            // 
            this.lblQ3Complete.AutoSize = true;
            this.lblQ3Complete.BackColor = System.Drawing.Color.Transparent;
            this.lblQ3Complete.Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQ3Complete.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(38)))), ((int)(((byte)(131)))));
            this.lblQ3Complete.Location = new System.Drawing.Point(377, 283);
            this.lblQ3Complete.Name = "lblQ3Complete";
            this.lblQ3Complete.Size = new System.Drawing.Size(87, 23);
            this.lblQ3Complete.TabIndex = 50;
            this.lblQ3Complete.Text = "Complete!";
            this.lblQ3Complete.Visible = false;
            // 
            // lblQ2Complete
            // 
            this.lblQ2Complete.AutoSize = true;
            this.lblQ2Complete.BackColor = System.Drawing.Color.Transparent;
            this.lblQ2Complete.Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQ2Complete.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(38)))), ((int)(((byte)(131)))));
            this.lblQ2Complete.Location = new System.Drawing.Point(377, 230);
            this.lblQ2Complete.Name = "lblQ2Complete";
            this.lblQ2Complete.Size = new System.Drawing.Size(87, 23);
            this.lblQ2Complete.TabIndex = 49;
            this.lblQ2Complete.Text = "Complete!";
            this.lblQ2Complete.Visible = false;
            // 
            // Q4progress
            // 
            this.Q4progress.BackColor = System.Drawing.Color.DarkGray;
            this.Q4progress.ForeColor = System.Drawing.Color.White;
            this.Q4progress.Location = new System.Drawing.Point(81, 332);
            this.Q4progress.Name = "Q4progress";
            this.Q4progress.Size = new System.Drawing.Size(280, 30);
            this.Q4progress.TabIndex = 8;
            // 
            // ALLprogress
            // 
            this.ALLprogress.BackColor = System.Drawing.Color.DarkGray;
            this.ALLprogress.ForeColor = System.Drawing.Color.White;
            this.ALLprogress.Location = new System.Drawing.Point(81, 124);
            this.ALLprogress.Name = "ALLprogress";
            this.ALLprogress.Size = new System.Drawing.Size(280, 30);
            this.ALLprogress.TabIndex = 8;
            // 
            // estimated
            // 
            this.estimated.AutoSize = true;
            this.estimated.BackColor = System.Drawing.Color.Transparent;
            this.estimated.Font = new System.Drawing.Font("Arial Narrow", 12F);
            this.estimated.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(38)))), ((int)(((byte)(131)))));
            this.estimated.Location = new System.Drawing.Point(376, 138);
            this.estimated.Name = "estimated";
            this.estimated.Size = new System.Drawing.Size(72, 20);
            this.estimated.TabIndex = 37;
            this.estimated.Text = "Estimated:";
            // 
            // lblQ4previous
            // 
            this.lblQ4previous.AutoSize = true;
            this.lblQ4previous.BackColor = System.Drawing.Color.Transparent;
            this.lblQ4previous.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQ4previous.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.lblQ4previous.Location = new System.Drawing.Point(376, 346);
            this.lblQ4previous.MaximumSize = new System.Drawing.Size(241, 20);
            this.lblQ4previous.Name = "lblQ4previous";
            this.lblQ4previous.Size = new System.Drawing.Size(66, 20);
            this.lblQ4previous.TabIndex = 35;
            this.lblQ4previous.Text = "Previous:";
            // 
            // lblQ3previous
            // 
            this.lblQ3previous.AutoSize = true;
            this.lblQ3previous.BackColor = System.Drawing.Color.Transparent;
            this.lblQ3previous.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQ3previous.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.lblQ3previous.Location = new System.Drawing.Point(376, 293);
            this.lblQ3previous.MaximumSize = new System.Drawing.Size(241, 20);
            this.lblQ3previous.Name = "lblQ3previous";
            this.lblQ3previous.Size = new System.Drawing.Size(66, 20);
            this.lblQ3previous.TabIndex = 33;
            this.lblQ3previous.Text = "Previous:";
            // 
            // Q2progress
            // 
            this.Q2progress.BackColor = System.Drawing.Color.DarkGray;
            this.Q2progress.ForeColor = System.Drawing.Color.White;
            this.Q2progress.Location = new System.Drawing.Point(81, 226);
            this.Q2progress.Name = "Q2progress";
            this.Q2progress.Size = new System.Drawing.Size(280, 30);
            this.Q2progress.TabIndex = 8;
            // 
            // lblQ2previous
            // 
            this.lblQ2previous.AutoSize = true;
            this.lblQ2previous.BackColor = System.Drawing.Color.Transparent;
            this.lblQ2previous.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQ2previous.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.lblQ2previous.Location = new System.Drawing.Point(376, 240);
            this.lblQ2previous.MaximumSize = new System.Drawing.Size(241, 20);
            this.lblQ2previous.Name = "lblQ2previous";
            this.lblQ2previous.Size = new System.Drawing.Size(66, 20);
            this.lblQ2previous.TabIndex = 31;
            this.lblQ2previous.Text = "Previous:";
            // 
            // Q3progress
            // 
            this.Q3progress.BackColor = System.Drawing.Color.DarkGray;
            this.Q3progress.ForeColor = System.Drawing.Color.White;
            this.Q3progress.Location = new System.Drawing.Point(81, 279);
            this.Q3progress.Name = "Q3progress";
            this.Q3progress.Size = new System.Drawing.Size(280, 30);
            this.Q3progress.TabIndex = 8;
            // 
            // lblQ1Complete
            // 
            this.lblQ1Complete.AutoSize = true;
            this.lblQ1Complete.BackColor = System.Drawing.Color.Transparent;
            this.lblQ1Complete.Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQ1Complete.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(38)))), ((int)(((byte)(131)))));
            this.lblQ1Complete.Location = new System.Drawing.Point(377, 176);
            this.lblQ1Complete.Name = "lblQ1Complete";
            this.lblQ1Complete.Size = new System.Drawing.Size(87, 23);
            this.lblQ1Complete.TabIndex = 48;
            this.lblQ1Complete.Text = "Complete!";
            this.lblQ1Complete.Visible = false;
            // 
            // Q1progress
            // 
            this.Q1progress.BackColor = System.Drawing.Color.DarkGray;
            this.Q1progress.ForeColor = System.Drawing.Color.White;
            this.Q1progress.Location = new System.Drawing.Point(81, 173);
            this.Q1progress.Name = "Q1progress";
            this.Q1progress.Size = new System.Drawing.Size(280, 30);
            this.Q1progress.TabIndex = 8;
            // 
            // elapsed
            // 
            this.elapsed.AutoSize = true;
            this.elapsed.BackColor = System.Drawing.Color.Transparent;
            this.elapsed.Font = new System.Drawing.Font("Arial Narrow", 12F);
            this.elapsed.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(38)))), ((int)(((byte)(131)))));
            this.elapsed.Location = new System.Drawing.Point(376, 119);
            this.elapsed.Name = "elapsed";
            this.elapsed.Size = new System.Drawing.Size(63, 20);
            this.elapsed.TabIndex = 36;
            this.elapsed.Text = "Elapsed:";
            // 
            // AllQuadrants
            // 
            this.AllQuadrants.BackColor = System.Drawing.Color.Transparent;
            this.AllQuadrants.BackgroundImage = global::GUI_Console.Properties.Resources.RN_ICON01N_overall_progress;
            this.AllQuadrants.Location = new System.Drawing.Point(19, 119);
            this.AllQuadrants.Name = "AllQuadrants";
            this.AllQuadrants.Size = new System.Drawing.Size(40, 40);
            this.AllQuadrants.TabIndex = 29;
            this.AllQuadrants.TabStop = false;
            // 
            // Q4
            // 
            this.Q4.BackColor = System.Drawing.Color.Transparent;
            this.Q4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Q4.BackgroundImage")));
            this.Q4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Q4.Check = false;
            this.Q4.Enabled = false;
            this.Q4.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Q4.ForeColor = System.Drawing.Color.White;
            this.Q4.Location = new System.Drawing.Point(19, 333);
            this.Q4.Name = "Q4";
            this.Q4.NowInProgress = false;
            this.Q4.Size = new System.Drawing.Size(40, 42);
            this.Q4.TabIndex = 21;
            this.Q4.Text = "4";
            // 
            // Q3
            // 
            this.Q3.BackColor = System.Drawing.Color.Transparent;
            this.Q3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Q3.BackgroundImage")));
            this.Q3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Q3.Check = false;
            this.Q3.Enabled = false;
            this.Q3.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Q3.ForeColor = System.Drawing.Color.White;
            this.Q3.Location = new System.Drawing.Point(19, 280);
            this.Q3.Name = "Q3";
            this.Q3.NowInProgress = false;
            this.Q3.Size = new System.Drawing.Size(40, 42);
            this.Q3.TabIndex = 20;
            this.Q3.Text = "3";
            // 
            // Q2
            // 
            this.Q2.BackColor = System.Drawing.Color.Transparent;
            this.Q2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Q2.BackgroundImage")));
            this.Q2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Q2.Check = false;
            this.Q2.Enabled = false;
            this.Q2.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Q2.ForeColor = System.Drawing.Color.White;
            this.Q2.Location = new System.Drawing.Point(19, 227);
            this.Q2.Name = "Q2";
            this.Q2.NowInProgress = false;
            this.Q2.Size = new System.Drawing.Size(40, 42);
            this.Q2.TabIndex = 19;
            this.Q2.Text = "2";
            // 
            // Q1
            // 
            this.Q1.BackColor = System.Drawing.Color.Transparent;
            this.Q1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Q1.BackgroundImage")));
            this.Q1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Q1.Check = false;
            this.Q1.Enabled = false;
            this.Q1.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Q1.ForeColor = System.Drawing.Color.White;
            this.Q1.Location = new System.Drawing.Point(19, 174);
            this.Q1.Name = "Q1";
            this.Q1.NowInProgress = false;
            this.Q1.Size = new System.Drawing.Size(40, 42);
            this.Q1.TabIndex = 18;
            this.Q1.Text = "1";
            // 
            // lblQ2Current
            // 
            this.lblQ2Current.AutoSize = true;
            this.lblQ2Current.BackColor = System.Drawing.Color.Transparent;
            this.lblQ2Current.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQ2Current.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.lblQ2Current.Location = new System.Drawing.Point(376, 221);
            this.lblQ2Current.MaximumSize = new System.Drawing.Size(241, 20);
            this.lblQ2Current.Name = "lblQ2Current";
            this.lblQ2Current.Size = new System.Drawing.Size(55, 20);
            this.lblQ2Current.TabIndex = 30;
            this.lblQ2Current.Text = "Current:";
            // 
            // lblQ3Current
            // 
            this.lblQ3Current.AutoSize = true;
            this.lblQ3Current.BackColor = System.Drawing.Color.Transparent;
            this.lblQ3Current.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQ3Current.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.lblQ3Current.Location = new System.Drawing.Point(376, 274);
            this.lblQ3Current.MaximumSize = new System.Drawing.Size(241, 20);
            this.lblQ3Current.Name = "lblQ3Current";
            this.lblQ3Current.Size = new System.Drawing.Size(55, 20);
            this.lblQ3Current.TabIndex = 32;
            this.lblQ3Current.Text = "Current:";
            // 
            // lblQ4Current
            // 
            this.lblQ4Current.AutoSize = true;
            this.lblQ4Current.BackColor = System.Drawing.Color.Transparent;
            this.lblQ4Current.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQ4Current.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.lblQ4Current.Location = new System.Drawing.Point(376, 327);
            this.lblQ4Current.MaximumSize = new System.Drawing.Size(241, 20);
            this.lblQ4Current.Name = "lblQ4Current";
            this.lblQ4Current.Size = new System.Drawing.Size(55, 20);
            this.lblQ4Current.TabIndex = 34;
            this.lblQ4Current.Text = "Current:";
            // 
            // lblQ1previous
            // 
            this.lblQ1previous.AutoSize = true;
            this.lblQ1previous.BackColor = System.Drawing.Color.Transparent;
            this.lblQ1previous.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQ1previous.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.lblQ1previous.Location = new System.Drawing.Point(376, 187);
            this.lblQ1previous.MaximumSize = new System.Drawing.Size(241, 20);
            this.lblQ1previous.Name = "lblQ1previous";
            this.lblQ1previous.Size = new System.Drawing.Size(66, 20);
            this.lblQ1previous.TabIndex = 23;
            this.lblQ1previous.Text = "Previous:";
            // 
            // updateTimer
            // 
            this.updateTimer.Interval = 1000;
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // CWJ_Timer
            // 
            this.CWJ_Timer.Interval = 1000;
            this.CWJ_Timer.Tick += new System.EventHandler(this.CWJ_Timer_Tick);
            // 
            // PauseTimer
            // 
            this.PauseTimer.Interval = 200;
            this.PauseTimer.Tick += new System.EventHandler(this.PauseTimer_Tick);
            // 
            // panel2
            // 
            this.panel2.BackgroundImage = global::GUI_Console.Properties.Resources.home_header640x40_bg;
            this.panel2.Controls.Add(this.lblUsrProtocols);
            this.panel2.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel2.ForeColor = System.Drawing.Color.Black;
            this.panel2.Location = new System.Drawing.Point(2, 70);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(636, 40);
            this.panel2.TabIndex = 57;
            // 
            // lblUsrProtocols
            // 
            this.lblUsrProtocols.AutoSize = true;
            this.lblUsrProtocols.BackColor = System.Drawing.Color.Transparent;
            this.lblUsrProtocols.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsrProtocols.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblUsrProtocols.Location = new System.Drawing.Point(15, 8);
            this.lblUsrProtocols.Name = "lblUsrProtocols";
            this.lblUsrProtocols.Size = new System.Drawing.Size(124, 26);
            this.lblUsrProtocols.TabIndex = 32;
            this.lblUsrProtocols.Text = "Run Progress";
            // 
            // button_Abort
            // 
            this.button_Abort.BackColor = System.Drawing.Color.Transparent;
            this.button_Abort.BackgroundImage = global::GUI_Console.Properties.Resources.RN_BTN02L_abort_104x86_STD;
            this.button_Abort.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Abort.Check = false;
            this.button_Abort.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Abort.ForeColor = System.Drawing.Color.White;
            this.button_Abort.Location = new System.Drawing.Point(529, 389);
            this.button_Abort.Name = "button_Abort";
            this.button_Abort.Size = new System.Drawing.Size(104, 86);
            this.button_Abort.TabIndex = 59;
            this.button_Abort.Text = "button_Abort";
            this.button_Abort.Click += new System.EventHandler(this.button_Abort_Click);
            // 
            // button_Pause
            // 
            this.button_Pause.BackColor = System.Drawing.Color.Transparent;
            this.button_Pause.BackgroundImage = global::GUI_Console.Properties.Resources.RN_BTN01L_pause_STD;
            this.button_Pause.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Pause.Check = false;
            this.button_Pause.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Pause.ForeColor = System.Drawing.Color.White;
            this.button_Pause.Location = new System.Drawing.Point(419, 389);
            this.button_Pause.Name = "button_Pause";
            this.button_Pause.Size = new System.Drawing.Size(104, 86);
            this.button_Pause.TabIndex = 60;
            this.button_Pause.Text = "button_Pause";
            this.button_Pause.Click += new System.EventHandler(this.button_Pause_Click);
            // 
            // button_RemoteDeskTop
            // 
            this.button_RemoteDeskTop.BackColor = System.Drawing.Color.Transparent;
            this.button_RemoteDeskTop.BackgroundImage = global::GUI_Console.Properties.Resources.lock_STD;
            this.button_RemoteDeskTop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_RemoteDeskTop.Check = false;
            this.button_RemoteDeskTop.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_RemoteDeskTop.ForeColor = System.Drawing.Color.White;
            this.button_RemoteDeskTop.Location = new System.Drawing.Point(199, 389);
            this.button_RemoteDeskTop.Name = "button_RemoteDeskTop";
            this.button_RemoteDeskTop.Size = new System.Drawing.Size(104, 86);
            this.button_RemoteDeskTop.TabIndex = 63;
            this.button_RemoteDeskTop.Text = "  ";
            this.button_RemoteDeskTop.Visible = false;
            // 
            // UserNameHeader
            // 
            this.UserNameHeader.BackColor = System.Drawing.Color.Transparent;
            this.UserNameHeader.BackgroundImage = global::GUI_Console.Properties.Resources.User_HeaderSml2;
            this.UserNameHeader.Location = new System.Drawing.Point(285, 1);
            this.UserNameHeader.Name = "UserNameHeader";
            this.UserNameHeader.Size = new System.Drawing.Size(353, 37);
            this.UserNameHeader.TabIndex = 64;
            // 
            // lblQ1Current
            // 
            this.lblQ1Current.AutoSize = true;
            this.lblQ1Current.BackColor = System.Drawing.Color.Transparent;
            this.lblQ1Current.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQ1Current.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.lblQ1Current.Location = new System.Drawing.Point(376, 168);
            this.lblQ1Current.MaximumSize = new System.Drawing.Size(241, 20);
            this.lblQ1Current.Name = "lblQ1Current";
            this.lblQ1Current.Size = new System.Drawing.Size(55, 20);
            this.lblQ1Current.TabIndex = 65;
            this.lblQ1Current.Text = "Current:";
            // 
            // RoboSep_RunProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this.Controls.Add(this.lblQ1Current);
            this.Controls.Add(this.UserNameHeader);
            this.Controls.Add(this.button_RemoteDeskTop);
            this.Controls.Add(this.button_Pause);
            this.Controls.Add(this.button_Abort);
            this.Controls.Add(this.labelQ4_protocolName);
            this.Controls.Add(this.labelQ3_protocolName);
            this.Controls.Add(this.labelQ2_protocolName);
            this.Controls.Add(this.labelQ1_protocolName);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.ALLprogress);
            this.Controls.Add(this.Q4progress);
            this.Controls.Add(this.Q3progress);
            this.Controls.Add(this.Q2progress);
            this.Controls.Add(this.Q1progress);
            this.Controls.Add(this.Q1);
            this.Controls.Add(this.lblQ1previous);
            this.Controls.Add(this.lblQ4Current);
            this.Controls.Add(this.lblQ3Current);
            this.Controls.Add(this.lblQ2Current);
            this.Controls.Add(this.Q2);
            this.Controls.Add(this.labelEstimated);
            this.Controls.Add(this.Q3);
            this.Controls.Add(this.labelElapsed);
            this.Controls.Add(this.Q4);
            this.Controls.Add(this.labelQ4_previous);
            this.Controls.Add(this.AllQuadrants);
            this.Controls.Add(this.labelQ4_current);
            this.Controls.Add(this.elapsed);
            this.Controls.Add(this.labelQ3_previous);
            this.Controls.Add(this.labelQ3_current);
            this.Controls.Add(this.lblQ1Complete);
            this.Controls.Add(this.labelQ2_previous);
            this.Controls.Add(this.labelQ2_current);
            this.Controls.Add(this.lblQ2previous);
            this.Controls.Add(this.labelQ1_previous);
            this.Controls.Add(this.labelQ1_current);
            this.Controls.Add(this.lblQ3previous);
            this.Controls.Add(this.ALLComplete);
            this.Controls.Add(this.lblQ4previous);
            this.Controls.Add(this.lblQ4Complete);
            this.Controls.Add(this.estimated);
            this.Controls.Add(this.lblQ3Complete);
            this.Controls.Add(this.lblQ2Complete);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(38)))), ((int)(((byte)(131)))));
            this.Name = "RoboSep_RunProgress";
            this.Load += new System.EventHandler(this.RoboSep_RunProgress_Load);
            this.Controls.SetChildIndex(this.lblQ2Complete, 0);
            this.Controls.SetChildIndex(this.lblQ3Complete, 0);
            this.Controls.SetChildIndex(this.estimated, 0);
            this.Controls.SetChildIndex(this.lblQ4Complete, 0);
            this.Controls.SetChildIndex(this.lblQ4previous, 0);
            this.Controls.SetChildIndex(this.ALLComplete, 0);
            this.Controls.SetChildIndex(this.lblQ3previous, 0);
            this.Controls.SetChildIndex(this.labelQ1_current, 0);
            this.Controls.SetChildIndex(this.labelQ1_previous, 0);
            this.Controls.SetChildIndex(this.lblQ2previous, 0);
            this.Controls.SetChildIndex(this.labelQ2_current, 0);
            this.Controls.SetChildIndex(this.labelQ2_previous, 0);
            this.Controls.SetChildIndex(this.lblQ1Complete, 0);
            this.Controls.SetChildIndex(this.labelQ3_current, 0);
            this.Controls.SetChildIndex(this.labelQ3_previous, 0);
            this.Controls.SetChildIndex(this.elapsed, 0);
            this.Controls.SetChildIndex(this.labelQ4_current, 0);
            this.Controls.SetChildIndex(this.AllQuadrants, 0);
            this.Controls.SetChildIndex(this.labelQ4_previous, 0);
            this.Controls.SetChildIndex(this.Q4, 0);
            this.Controls.SetChildIndex(this.labelElapsed, 0);
            this.Controls.SetChildIndex(this.Q3, 0);
            this.Controls.SetChildIndex(this.labelEstimated, 0);
            this.Controls.SetChildIndex(this.Q2, 0);
            this.Controls.SetChildIndex(this.lblQ2Current, 0);
            this.Controls.SetChildIndex(this.lblQ3Current, 0);
            this.Controls.SetChildIndex(this.lblQ4Current, 0);
            this.Controls.SetChildIndex(this.lblQ1previous, 0);
            this.Controls.SetChildIndex(this.Q1, 0);
            this.Controls.SetChildIndex(this.Q1progress, 0);
            this.Controls.SetChildIndex(this.Q2progress, 0);
            this.Controls.SetChildIndex(this.Q3progress, 0);
            this.Controls.SetChildIndex(this.Q4progress, 0);
            this.Controls.SetChildIndex(this.ALLprogress, 0);
            this.Controls.SetChildIndex(this.panel2, 0);
            this.Controls.SetChildIndex(this.btn_home, 0);
            this.Controls.SetChildIndex(this.labelQ1_protocolName, 0);
            this.Controls.SetChildIndex(this.labelQ2_protocolName, 0);
            this.Controls.SetChildIndex(this.labelQ3_protocolName, 0);
            this.Controls.SetChildIndex(this.labelQ4_protocolName, 0);
            this.Controls.SetChildIndex(this.button_Abort, 0);
            this.Controls.SetChildIndex(this.button_Pause, 0);
            this.Controls.SetChildIndex(this.button_RemoteDeskTop, 0);
            this.Controls.SetChildIndex(this.UserNameHeader, 0);
            this.Controls.SetChildIndex(this.lblQ1Current, 0);
            ((System.ComponentModel.ISupportInitialize)(this.AllQuadrants)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GUI_Controls.Button_Quadrant Q1;
        private GUI_Controls.Button_Quadrant Q4;
        private GUI_Controls.Button_Quadrant Q3;
        private GUI_Controls.Button_Quadrant Q2;
        private System.Windows.Forms.Label lblQ1previous;
        private GUI_Controls.ProgressBar Q1progress;
        private GUI_Controls.ProgressBar ALLprogress;
        private GUI_Controls.ProgressBar Q4progress;
        private GUI_Controls.ProgressBar Q3progress;
        private GUI_Controls.ProgressBar Q2progress;
        private System.Windows.Forms.PictureBox AllQuadrants;
        private System.Windows.Forms.Label estimated;
        private System.Windows.Forms.Label elapsed;
        private System.Windows.Forms.Label lblQ4previous;
        private System.Windows.Forms.Label lblQ4Current;
        private System.Windows.Forms.Label lblQ3previous;
        private System.Windows.Forms.Label lblQ3Current;
        private System.Windows.Forms.Label lblQ2previous;
        private System.Windows.Forms.Label lblQ2Current;
        private GUI_Controls.TextBoxScroll labelQ1_previous;
        private GUI_Controls.TextBoxScroll labelQ1_current;
        private GUI_Controls.TextBoxScroll labelQ4_previous;
        private GUI_Controls.TextBoxScroll labelQ4_current;
        private GUI_Controls.TextBoxScroll labelQ3_previous;
        private GUI_Controls.TextBoxScroll labelQ3_current;
        private GUI_Controls.TextBoxScroll labelQ2_previous;
        private GUI_Controls.TextBoxScroll labelQ2_current;
        private GUI_Controls.TextBoxScroll labelEstimated;
        private GUI_Controls.TextBoxScroll labelElapsed;
        private System.Windows.Forms.Label lblQ1Complete;
        private System.Windows.Forms.Label ALLComplete;
        private System.Windows.Forms.Label lblQ4Complete;
        private System.Windows.Forms.Label lblQ3Complete;
        private System.Windows.Forms.Label lblQ2Complete;
        private System.Windows.Forms.Timer PauseTimer;
        public System.Windows.Forms.Timer updateTimer;
        public System.Windows.Forms.Timer CWJ_Timer;
        private System.Windows.Forms.Label labelQ4_protocolName;
        private System.Windows.Forms.Label labelQ3_protocolName;
        private System.Windows.Forms.Label labelQ2_protocolName;
        private System.Windows.Forms.Label labelQ1_protocolName;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblUsrProtocols;
        private GUI_Controls.Button_Circle button_Abort;
        private GUI_Controls.Button_Circle button_Pause;
        private GUI_Controls.GUIButton button_RemoteDeskTop;
        private GUI_Controls.nameHeader UserNameHeader;
        private System.Windows.Forms.Label lblQ1Current;
    }
}
