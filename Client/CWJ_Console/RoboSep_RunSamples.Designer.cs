namespace GUI_Console
{
    partial class RoboSep_RunSamples
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
            for (i = 0; i < iList.Count; i++)
                iList[i].Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoboSep_RunSamples));
            this.pnlQ1_vol = new System.Windows.Forms.Panel();
            this.Q1_Vol = new System.Windows.Forms.Label();
            this.label_volume = new System.Windows.Forms.Label();
            this.pnlQ1 = new System.Windows.Forms.Panel();
            this.Q1_lbl_SelectProtocol = new System.Windows.Forms.Label();
            this.Q1_protocolName = new GUI_Controls.SizableLabel();
            this.Q1 = new GUI_Controls.Button_Quadrant2();
            this.label_quadrant = new System.Windows.Forms.Label();
            this.label_name = new System.Windows.Forms.Label();
            this.divider3 = new System.Windows.Forms.Panel();
            this.divider2 = new System.Windows.Forms.Panel();
            this.divider1 = new System.Windows.Forms.Panel();
            this.pnlQ2_vol = new System.Windows.Forms.Panel();
            this.Q2_Vol = new System.Windows.Forms.Label();
            this.pnlQ3_vol = new System.Windows.Forms.Panel();
            this.Q3_Vol = new System.Windows.Forms.Label();
            this.pnlQ2 = new System.Windows.Forms.Panel();
            this.Q2_lbl_SelectProtocol = new System.Windows.Forms.Label();
            this.Q2_protocolName = new GUI_Controls.SizableLabel();
            this.Q2 = new GUI_Controls.Button_Quadrant2();
            this.pnlQ3 = new System.Windows.Forms.Panel();
            this.Q3_lbl_SelectProtocol = new System.Windows.Forms.Label();
            this.Q3_protocolName = new GUI_Controls.SizableLabel();
            this.Q3 = new GUI_Controls.Button_Quadrant2();
            this.pnlQ4_vol = new System.Windows.Forms.Panel();
            this.Q4_Vol = new System.Windows.Forms.Label();
            this.Q4 = new GUI_Controls.Button_Quadrant2();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button_CancelSelected = new GUI_Controls.Button_Circle();
            this.Q4_protocolName = new GUI_Controls.SizableLabel();
            this.Q4_lbl_SelectProtocol = new System.Windows.Forms.Label();
            this.pnlQ4 = new System.Windows.Forms.Panel();
            this.button_RunSamples = new GUI_Controls.GUIButton();
            this.button_RemoteDeskTop = new GUI_Controls.GUIButton();
            this.panel_header = new System.Windows.Forms.Panel();
            this.button_Tab1 = new GUI_Controls.Button_Tab();
            this.pnlQ1_vol.SuspendLayout();
            this.pnlQ1.SuspendLayout();
            this.pnlQ2_vol.SuspendLayout();
            this.pnlQ3_vol.SuspendLayout();
            this.pnlQ2.SuspendLayout();
            this.pnlQ3.SuspendLayout();
            this.pnlQ4_vol.SuspendLayout();
            this.panel2.SuspendLayout();
            this.pnlQ4.SuspendLayout();
            this.panel_header.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_home
            // 
            this.btn_home.Location = new System.Drawing.Point(8, 405);
            // 
            // pnlQ1_vol
            // 
            this.pnlQ1_vol.BackColor = System.Drawing.Color.Transparent;
            this.pnlQ1_vol.Controls.Add(this.Q1_Vol);
            this.pnlQ1_vol.Location = new System.Drawing.Point(101, 110);
            this.pnlQ1_vol.Name = "pnlQ1_vol";
            this.pnlQ1_vol.Size = new System.Drawing.Size(80, 55);
            this.pnlQ1_vol.TabIndex = 26;
            // 
            // Q1_Vol
            // 
            this.Q1_Vol.AutoSize = true;
            this.Q1_Vol.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.Q1_Vol.Location = new System.Drawing.Point(-3, 18);
            this.Q1_Vol.Name = "Q1_Vol";
            this.Q1_Vol.Size = new System.Drawing.Size(0, 24);
            this.Q1_Vol.TabIndex = 26;
            this.Q1_Vol.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Q1_Vol.Click += new System.EventHandler(this.Q1_Vol_Click);
            // 
            // label_volume
            // 
            this.label_volume.BackColor = System.Drawing.Color.Transparent;
            this.label_volume.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.label_volume.ForeColor = System.Drawing.Color.White;
            this.label_volume.Image = global::GUI_Console.Properties.Resources.HM_ICON02S_volume;
            this.label_volume.Location = new System.Drawing.Point(105, -3);
            this.label_volume.Name = "label_volume";
            this.label_volume.Size = new System.Drawing.Size(34, 46);
            this.label_volume.TabIndex = 16;
            // 
            // pnlQ1
            // 
            this.pnlQ1.BackColor = System.Drawing.Color.Transparent;
            this.pnlQ1.Controls.Add(this.Q1_lbl_SelectProtocol);
            this.pnlQ1.Controls.Add(this.Q1_protocolName);
            this.pnlQ1.Location = new System.Drawing.Point(180, 112);
            this.pnlQ1.Name = "pnlQ1";
            this.pnlQ1.Size = new System.Drawing.Size(447, 55);
            this.pnlQ1.TabIndex = 33;
            this.pnlQ1.Click += new System.EventHandler(this.Q1_Select);
            // 
            // Q1_lbl_SelectProtocol
            // 
            this.Q1_lbl_SelectProtocol.AutoSize = true;
            this.Q1_lbl_SelectProtocol.Enabled = false;
            this.Q1_lbl_SelectProtocol.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Q1_lbl_SelectProtocol.ForeColor = System.Drawing.SystemColors.GrayText;
            this.Q1_lbl_SelectProtocol.Location = new System.Drawing.Point(107, 15);
            this.Q1_lbl_SelectProtocol.Name = "Q1_lbl_SelectProtocol";
            this.Q1_lbl_SelectProtocol.Size = new System.Drawing.Size(193, 25);
            this.Q1_lbl_SelectProtocol.TabIndex = 27;
            this.Q1_lbl_SelectProtocol.Text = "< Select Protocol >";
            // 
            // Q1_protocolName
            // 
            this.Q1_protocolName.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Q1_protocolName.Location = new System.Drawing.Point(19, 4);
            this.Q1_protocolName.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.Q1_protocolName.Name = "Q1_protocolName";
            this.Q1_protocolName.Size = new System.Drawing.Size(405, 48);
            this.Q1_protocolName.TabIndex = 28;
            this.Q1_protocolName.Click += new System.EventHandler(this.Q1_Select);
            // 
            // Q1
            // 
            this.Q1.BackColor = System.Drawing.Color.Transparent;
            this.Q1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Q1.BackgroundImage")));
            this.Q1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Q1.Cancelled = false;
            this.Q1.Check = true;
            this.Q1.Enabled = false;
            this.Q1.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Q1.ForeColor = System.Drawing.Color.White;
            this.Q1.Location = new System.Drawing.Point(40, 120);
            this.Q1.Name = "Q1";
            this.Q1.Selected = false;
            this.Q1.Size = new System.Drawing.Size(40, 44);
            this.Q1.TabIndex = 17;
            this.Q1.Text = "1";
            this.Q1.Click += new System.EventHandler(this.Q1_Click);
            // 
            // label_quadrant
            // 
            this.label_quadrant.BackColor = System.Drawing.Color.Transparent;
            this.label_quadrant.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.label_quadrant.ForeColor = System.Drawing.Color.White;
            this.label_quadrant.Image = global::GUI_Console.Properties.Resources.HM_ICON01S_quad_number;
            this.label_quadrant.Location = new System.Drawing.Point(36, -3);
            this.label_quadrant.Name = "label_quadrant";
            this.label_quadrant.Size = new System.Drawing.Size(45, 46);
            this.label_quadrant.TabIndex = 26;
            // 
            // label_name
            // 
            this.label_name.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_name.BackColor = System.Drawing.Color.Transparent;
            this.label_name.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_name.ForeColor = System.Drawing.SystemColors.GrayText;
            this.label_name.Location = new System.Drawing.Point(194, 7);
            this.label_name.Name = "label_name";
            this.label_name.Size = new System.Drawing.Size(230, 29);
            this.label_name.TabIndex = 15;
            this.label_name.Text = "Protocol Name";
            // 
            // divider3
            // 
            this.divider3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(182)))), ((int)(((byte)(183)))), ((int)(((byte)(186)))));
            this.divider3.Location = new System.Drawing.Point(0, 298);
            this.divider3.Name = "divider3";
            this.divider3.Size = new System.Drawing.Size(640, 2);
            this.divider3.TabIndex = 5;
            // 
            // divider2
            // 
            this.divider2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(182)))), ((int)(((byte)(183)))), ((int)(((byte)(186)))));
            this.divider2.Location = new System.Drawing.Point(0, 235);
            this.divider2.Name = "divider2";
            this.divider2.Size = new System.Drawing.Size(640, 2);
            this.divider2.TabIndex = 4;
            // 
            // divider1
            // 
            this.divider1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(182)))), ((int)(((byte)(183)))), ((int)(((byte)(186)))));
            this.divider1.Location = new System.Drawing.Point(0, 171);
            this.divider1.Name = "divider1";
            this.divider1.Size = new System.Drawing.Size(640, 2);
            this.divider1.TabIndex = 3;
            // 
            // pnlQ2_vol
            // 
            this.pnlQ2_vol.BackColor = System.Drawing.Color.Transparent;
            this.pnlQ2_vol.Controls.Add(this.Q2_Vol);
            this.pnlQ2_vol.Location = new System.Drawing.Point(101, 175);
            this.pnlQ2_vol.Name = "pnlQ2_vol";
            this.pnlQ2_vol.Size = new System.Drawing.Size(80, 55);
            this.pnlQ2_vol.TabIndex = 27;
            // 
            // Q2_Vol
            // 
            this.Q2_Vol.AutoSize = true;
            this.Q2_Vol.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.Q2_Vol.Location = new System.Drawing.Point(-3, 18);
            this.Q2_Vol.Name = "Q2_Vol";
            this.Q2_Vol.Size = new System.Drawing.Size(0, 24);
            this.Q2_Vol.TabIndex = 26;
            this.Q2_Vol.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Q2_Vol.Click += new System.EventHandler(this.Q2_Vol_Click);
            // 
            // pnlQ3_vol
            // 
            this.pnlQ3_vol.BackColor = System.Drawing.Color.Transparent;
            this.pnlQ3_vol.Controls.Add(this.Q3_Vol);
            this.pnlQ3_vol.Location = new System.Drawing.Point(101, 237);
            this.pnlQ3_vol.Name = "pnlQ3_vol";
            this.pnlQ3_vol.Size = new System.Drawing.Size(80, 55);
            this.pnlQ3_vol.TabIndex = 35;
            // 
            // Q3_Vol
            // 
            this.Q3_Vol.AutoSize = true;
            this.Q3_Vol.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.Q3_Vol.Location = new System.Drawing.Point(-3, 18);
            this.Q3_Vol.Name = "Q3_Vol";
            this.Q3_Vol.Size = new System.Drawing.Size(0, 24);
            this.Q3_Vol.TabIndex = 26;
            this.Q3_Vol.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Q3_Vol.Click += new System.EventHandler(this.Q3_Vol_Click);
            // 
            // pnlQ2
            // 
            this.pnlQ2.BackColor = System.Drawing.Color.Transparent;
            this.pnlQ2.Controls.Add(this.Q2_lbl_SelectProtocol);
            this.pnlQ2.Controls.Add(this.Q2_protocolName);
            this.pnlQ2.Location = new System.Drawing.Point(180, 177);
            this.pnlQ2.Name = "pnlQ2";
            this.pnlQ2.Size = new System.Drawing.Size(447, 55);
            this.pnlQ2.TabIndex = 34;
            this.pnlQ2.Click += new System.EventHandler(this.Q2_Select);
            // 
            // Q2_lbl_SelectProtocol
            // 
            this.Q2_lbl_SelectProtocol.AutoSize = true;
            this.Q2_lbl_SelectProtocol.Enabled = false;
            this.Q2_lbl_SelectProtocol.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Q2_lbl_SelectProtocol.ForeColor = System.Drawing.SystemColors.GrayText;
            this.Q2_lbl_SelectProtocol.Location = new System.Drawing.Point(107, 16);
            this.Q2_lbl_SelectProtocol.Name = "Q2_lbl_SelectProtocol";
            this.Q2_lbl_SelectProtocol.Size = new System.Drawing.Size(193, 25);
            this.Q2_lbl_SelectProtocol.TabIndex = 28;
            this.Q2_lbl_SelectProtocol.Text = "< Select Protocol >";
            // 
            // Q2_protocolName
            // 
            this.Q2_protocolName.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Q2_protocolName.Location = new System.Drawing.Point(19, 3);
            this.Q2_protocolName.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.Q2_protocolName.Name = "Q2_protocolName";
            this.Q2_protocolName.Size = new System.Drawing.Size(405, 48);
            this.Q2_protocolName.TabIndex = 29;
            this.Q2_protocolName.Click += new System.EventHandler(this.Q2_Select);
            // 
            // Q2
            // 
            this.Q2.BackColor = System.Drawing.Color.Transparent;
            this.Q2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Q2.BackgroundImage")));
            this.Q2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Q2.Cancelled = false;
            this.Q2.Check = true;
            this.Q2.Enabled = false;
            this.Q2.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Q2.ForeColor = System.Drawing.Color.White;
            this.Q2.Location = new System.Drawing.Point(40, 184);
            this.Q2.Name = "Q2";
            this.Q2.Selected = false;
            this.Q2.Size = new System.Drawing.Size(40, 44);
            this.Q2.TabIndex = 26;
            this.Q2.Text = "2";
            this.Q2.Click += new System.EventHandler(this.Q2_Click);
            // 
            // pnlQ3
            // 
            this.pnlQ3.BackColor = System.Drawing.Color.Transparent;
            this.pnlQ3.Controls.Add(this.Q3_lbl_SelectProtocol);
            this.pnlQ3.Controls.Add(this.Q3_protocolName);
            this.pnlQ3.Location = new System.Drawing.Point(180, 239);
            this.pnlQ3.Name = "pnlQ3";
            this.pnlQ3.Size = new System.Drawing.Size(447, 55);
            this.pnlQ3.TabIndex = 37;
            this.pnlQ3.Click += new System.EventHandler(this.Q3_Select);
            // 
            // Q3_lbl_SelectProtocol
            // 
            this.Q3_lbl_SelectProtocol.AutoSize = true;
            this.Q3_lbl_SelectProtocol.Enabled = false;
            this.Q3_lbl_SelectProtocol.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Q3_lbl_SelectProtocol.ForeColor = System.Drawing.SystemColors.GrayText;
            this.Q3_lbl_SelectProtocol.Location = new System.Drawing.Point(107, 15);
            this.Q3_lbl_SelectProtocol.Name = "Q3_lbl_SelectProtocol";
            this.Q3_lbl_SelectProtocol.Size = new System.Drawing.Size(193, 25);
            this.Q3_lbl_SelectProtocol.TabIndex = 28;
            this.Q3_lbl_SelectProtocol.Text = "< Select Protocol >";
            // 
            // Q3_protocolName
            // 
            this.Q3_protocolName.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Q3_protocolName.Location = new System.Drawing.Point(19, 3);
            this.Q3_protocolName.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.Q3_protocolName.Name = "Q3_protocolName";
            this.Q3_protocolName.Size = new System.Drawing.Size(405, 48);
            this.Q3_protocolName.TabIndex = 30;
            this.Q3_protocolName.Click += new System.EventHandler(this.Q3_Select);
            // 
            // Q3
            // 
            this.Q3.BackColor = System.Drawing.Color.Transparent;
            this.Q3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Q3.BackgroundImage")));
            this.Q3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Q3.Cancelled = false;
            this.Q3.Check = true;
            this.Q3.Enabled = false;
            this.Q3.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Q3.ForeColor = System.Drawing.Color.White;
            this.Q3.Location = new System.Drawing.Point(40, 247);
            this.Q3.Name = "Q3";
            this.Q3.Selected = false;
            this.Q3.Size = new System.Drawing.Size(40, 44);
            this.Q3.TabIndex = 26;
            this.Q3.Text = "3";
            this.Q3.Click += new System.EventHandler(this.Q3_Click);
            // 
            // pnlQ4_vol
            // 
            this.pnlQ4_vol.BackColor = System.Drawing.Color.Transparent;
            this.pnlQ4_vol.Controls.Add(this.Q4_Vol);
            this.pnlQ4_vol.Location = new System.Drawing.Point(101, 300);
            this.pnlQ4_vol.Name = "pnlQ4_vol";
            this.pnlQ4_vol.Size = new System.Drawing.Size(80, 55);
            this.pnlQ4_vol.TabIndex = 38;
            // 
            // Q4_Vol
            // 
            this.Q4_Vol.AutoSize = true;
            this.Q4_Vol.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.Q4_Vol.Location = new System.Drawing.Point(-3, 18);
            this.Q4_Vol.Name = "Q4_Vol";
            this.Q4_Vol.Size = new System.Drawing.Size(0, 24);
            this.Q4_Vol.TabIndex = 26;
            this.Q4_Vol.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Q4_Vol.Click += new System.EventHandler(this.Q4_Vol_Click);
            // 
            // Q4
            // 
            this.Q4.BackColor = System.Drawing.Color.Transparent;
            this.Q4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Q4.BackgroundImage")));
            this.Q4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Q4.Cancelled = false;
            this.Q4.Check = true;
            this.Q4.Enabled = false;
            this.Q4.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Q4.ForeColor = System.Drawing.Color.White;
            this.Q4.Location = new System.Drawing.Point(40, 309);
            this.Q4.Name = "Q4";
            this.Q4.Selected = false;
            this.Q4.Size = new System.Drawing.Size(40, 44);
            this.Q4.TabIndex = 26;
            this.Q4.Text = "4";
            this.Q4.Click += new System.EventHandler(this.Q4_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(182)))), ((int)(((byte)(183)))), ((int)(((byte)(186)))));
            this.panel1.Location = new System.Drawing.Point(0, 363);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(640, 2);
            this.panel1.TabIndex = 41;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.BackgroundImage = global::GUI_Console.Properties.Resources.home_header640x40_bg;
            this.panel2.Controls.Add(this.label_name);
            this.panel2.Controls.Add(this.label_quadrant);
            this.panel2.Controls.Add(this.label_volume);
            this.panel2.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel2.Location = new System.Drawing.Point(0, 67);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(640, 40);
            this.panel2.TabIndex = 42;
            // 
            // button_CancelSelected
            // 
            this.button_CancelSelected.BackColor = System.Drawing.Color.Transparent;
            this.button_CancelSelected.BackgroundImage = global::GUI_Console.Properties.Resources.HM_BTN01L_delete_STD;
            this.button_CancelSelected.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_CancelSelected.Check = false;
            this.button_CancelSelected.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_CancelSelected.ForeColor = System.Drawing.Color.White;
            this.button_CancelSelected.Location = new System.Drawing.Point(118, 388);
            this.button_CancelSelected.Name = "button_CancelSelected";
            this.button_CancelSelected.Size = new System.Drawing.Size(104, 86);
            this.button_CancelSelected.TabIndex = 44;
            this.button_CancelSelected.Text = "X";
            this.button_CancelSelected.Click += new System.EventHandler(this.button_CancelSelected_Click);
            // 
            // Q4_protocolName
            // 
            this.Q4_protocolName.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Q4_protocolName.Location = new System.Drawing.Point(19, 4);
            this.Q4_protocolName.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.Q4_protocolName.Name = "Q4_protocolName";
            this.Q4_protocolName.Size = new System.Drawing.Size(405, 48);
            this.Q4_protocolName.TabIndex = 30;
            this.Q4_protocolName.Click += new System.EventHandler(this.Q4_Select);
            // 
            // Q4_lbl_SelectProtocol
            // 
            this.Q4_lbl_SelectProtocol.AutoSize = true;
            this.Q4_lbl_SelectProtocol.Enabled = false;
            this.Q4_lbl_SelectProtocol.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Q4_lbl_SelectProtocol.ForeColor = System.Drawing.SystemColors.GrayText;
            this.Q4_lbl_SelectProtocol.Location = new System.Drawing.Point(107, 15);
            this.Q4_lbl_SelectProtocol.Name = "Q4_lbl_SelectProtocol";
            this.Q4_lbl_SelectProtocol.Size = new System.Drawing.Size(193, 25);
            this.Q4_lbl_SelectProtocol.TabIndex = 28;
            this.Q4_lbl_SelectProtocol.Text = "< Select Protocol >";
            // 
            // pnlQ4
            // 
            this.pnlQ4.BackColor = System.Drawing.Color.Transparent;
            this.pnlQ4.Controls.Add(this.Q4_lbl_SelectProtocol);
            this.pnlQ4.Controls.Add(this.Q4_protocolName);
            this.pnlQ4.Location = new System.Drawing.Point(180, 303);
            this.pnlQ4.Name = "pnlQ4";
            this.pnlQ4.Size = new System.Drawing.Size(447, 55);
            this.pnlQ4.TabIndex = 40;
            this.pnlQ4.Click += new System.EventHandler(this.Q4_Select);
            // 
            // button_RunSamples
            // 
            this.button_RunSamples.BackColor = System.Drawing.Color.Transparent;
            this.button_RunSamples.BackgroundImage = global::GUI_Console.Properties.Resources.L_104x86_single_arrow_right_OVER;
            this.button_RunSamples.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_RunSamples.Check = false;
            this.button_RunSamples.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_RunSamples.ForeColor = System.Drawing.Color.White;
            this.button_RunSamples.Location = new System.Drawing.Point(528, 388);
            this.button_RunSamples.Name = "button_RunSamples";
            this.button_RunSamples.Size = new System.Drawing.Size(104, 86);
            this.button_RunSamples.TabIndex = 45;
            this.button_RunSamples.Text = "  ";
            // 
            // button_RemoteDeskTop
            // 
            this.button_RemoteDeskTop.BackColor = System.Drawing.Color.Transparent;
            this.button_RemoteDeskTop.BackgroundImage = global::GUI_Console.Properties.Resources.lock_STD;
            this.button_RemoteDeskTop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_RemoteDeskTop.Check = false;
            this.button_RemoteDeskTop.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_RemoteDeskTop.ForeColor = System.Drawing.Color.White;
            this.button_RemoteDeskTop.Location = new System.Drawing.Point(418, 388);
            this.button_RemoteDeskTop.Name = "button_RemoteDeskTop";
            this.button_RemoteDeskTop.Size = new System.Drawing.Size(104, 86);
            this.button_RemoteDeskTop.TabIndex = 47;
            this.button_RemoteDeskTop.Text = "  ";
            this.button_RemoteDeskTop.Visible = false;
            // 
            // panel_header
            // 
            this.panel_header.BackgroundImage = global::GUI_Console.Properties.Resources.User_HeaderSml2;
            this.panel_header.Controls.Add(this.button_Tab1);
            this.panel_header.Location = new System.Drawing.Point(285, 1);
            this.panel_header.Name = "panel_header";
            this.panel_header.Size = new System.Drawing.Size(353, 37);
            this.panel_header.TabIndex = 49;
            // 
            // button_Tab1
            // 
            this.button_Tab1.BackColor = System.Drawing.Color.Transparent;
            this.button_Tab1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Tab1.Check = false;
            this.button_Tab1.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Tab1.ForeColor = System.Drawing.SystemColors.GrayText;
            this.button_Tab1.Location = new System.Drawing.Point(0, 0);
            this.button_Tab1.Name = "button_Tab1";
            this.button_Tab1.Size = new System.Drawing.Size(227, 37);
            this.button_Tab1.TabIndex = 50;
            this.button_Tab1.Text = "button_Tab1";
            this.button_Tab1.Click += new System.EventHandler(this.button_Tab1_Click);
            // 
            // RoboSep_RunSamples
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this.Controls.Add(this.panel_header);
            this.Controls.Add(this.button_RemoteDeskTop);
            this.Controls.Add(this.button_RunSamples);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.Q4);
            this.Controls.Add(this.Q3);
            this.Controls.Add(this.Q2);
            this.Controls.Add(this.button_CancelSelected);
            this.Controls.Add(this.Q1);
            this.Controls.Add(this.divider2);
            this.Controls.Add(this.divider3);
            this.Controls.Add(this.pnlQ1);
            this.Controls.Add(this.divider1);
            this.Controls.Add(this.pnlQ4);
            this.Controls.Add(this.pnlQ4_vol);
            this.Controls.Add(this.pnlQ1_vol);
            this.Controls.Add(this.pnlQ2_vol);
            this.Controls.Add(this.pnlQ3);
            this.Controls.Add(this.pnlQ3_vol);
            this.Controls.Add(this.pnlQ2);
            this.Name = "RoboSep_RunSamples";
            this.Load += new System.EventHandler(this.RoboSep_RunSamples_Load);
            this.Controls.SetChildIndex(this.pnlQ2, 0);
            this.Controls.SetChildIndex(this.pnlQ3_vol, 0);
            this.Controls.SetChildIndex(this.pnlQ3, 0);
            this.Controls.SetChildIndex(this.pnlQ2_vol, 0);
            this.Controls.SetChildIndex(this.pnlQ1_vol, 0);
            this.Controls.SetChildIndex(this.pnlQ4_vol, 0);
            this.Controls.SetChildIndex(this.pnlQ4, 0);
            this.Controls.SetChildIndex(this.divider1, 0);
            this.Controls.SetChildIndex(this.pnlQ1, 0);
            this.Controls.SetChildIndex(this.divider3, 0);
            this.Controls.SetChildIndex(this.divider2, 0);
            this.Controls.SetChildIndex(this.Q1, 0);
            this.Controls.SetChildIndex(this.button_CancelSelected, 0);
            this.Controls.SetChildIndex(this.Q2, 0);
            this.Controls.SetChildIndex(this.Q3, 0);
            this.Controls.SetChildIndex(this.Q4, 0);
            this.Controls.SetChildIndex(this.panel1, 0);
            this.Controls.SetChildIndex(this.panel2, 0);
            this.Controls.SetChildIndex(this.button_RunSamples, 0);
            this.Controls.SetChildIndex(this.button_RemoteDeskTop, 0);
            this.Controls.SetChildIndex(this.panel_header, 0);
            this.Controls.SetChildIndex(this.btn_home, 0);
            this.pnlQ1_vol.ResumeLayout(false);
            this.pnlQ1_vol.PerformLayout();
            this.pnlQ1.ResumeLayout(false);
            this.pnlQ1.PerformLayout();
            this.pnlQ2_vol.ResumeLayout(false);
            this.pnlQ2_vol.PerformLayout();
            this.pnlQ3_vol.ResumeLayout(false);
            this.pnlQ3_vol.PerformLayout();
            this.pnlQ2.ResumeLayout(false);
            this.pnlQ2.PerformLayout();
            this.pnlQ3.ResumeLayout(false);
            this.pnlQ3.PerformLayout();
            this.pnlQ4_vol.ResumeLayout(false);
            this.pnlQ4_vol.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.pnlQ4.ResumeLayout(false);
            this.pnlQ4.PerformLayout();
            this.panel_header.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlQ1_vol;
        private System.Windows.Forms.Label label_volume;
        private System.Windows.Forms.Label Q1_Vol;
        private System.Windows.Forms.Panel pnlQ1;
        private System.Windows.Forms.Label label_name;
        private System.Windows.Forms.Panel divider3;
        private System.Windows.Forms.Panel divider2;
        private System.Windows.Forms.Panel divider1;
        private System.Windows.Forms.Panel pnlQ2_vol;
        private System.Windows.Forms.Label Q2_Vol;
        private System.Windows.Forms.Panel pnlQ3_vol;
        private System.Windows.Forms.Label Q3_Vol;
        private System.Windows.Forms.Panel pnlQ2;
        private System.Windows.Forms.Panel pnlQ3;
        private System.Windows.Forms.Panel pnlQ4_vol;
        private System.Windows.Forms.Label Q4_Vol;
        private GUI_Controls.Button_Quadrant2 Q1;
        private GUI_Controls.Button_Quadrant2 Q2;
        private GUI_Controls.Button_Quadrant2 Q3;
        private GUI_Controls.Button_Quadrant2 Q4;
        private System.Windows.Forms.Label label_quadrant;
        private System.Windows.Forms.Label Q1_lbl_SelectProtocol;
        private System.Windows.Forms.Label Q2_lbl_SelectProtocol;
        private System.Windows.Forms.Label Q3_lbl_SelectProtocol;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private GUI_Controls.SizableLabel Q1_protocolName;
        private GUI_Controls.SizableLabel Q2_protocolName;
        private GUI_Controls.SizableLabel Q3_protocolName;
        private GUI_Controls.Button_Circle button_CancelSelected;
        private GUI_Controls.SizableLabel Q4_protocolName;
        private System.Windows.Forms.Label Q4_lbl_SelectProtocol;
        private System.Windows.Forms.Panel pnlQ4;
        private GUI_Controls.GUIButton button_RunSamples;
        private GUI_Controls.GUIButton button_RemoteDeskTop;
        private System.Windows.Forms.Panel panel_header;
        private GUI_Controls.Button_Tab button_Tab1;
    }
}
