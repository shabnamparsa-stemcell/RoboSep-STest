namespace Tesla.Service
{
    partial class RoboSepServiceManualStepControl
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
            this.lblAccept = new System.Windows.Forms.Label();
            this.lblRestore = new System.Windows.Forms.Label();
            this.lblBack = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblAbort = new System.Windows.Forms.Label();
            this.lblBarcode = new System.Windows.Forms.Label();
            this.robosepModelButton1 = new Tesla.Service.RobosepModelButton();
            this.button_Barcode = new GUI_Controls.Button_Rectangle();
            this.button_Back = new GUI_Controls.Button_Rectangle();
            this.button_abort = new GUI_Controls.Button_Rectangle();
            this.panel1 = new Tesla.Service.DoubleBufferPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.txtUnits = new System.Windows.Forms.TextBox();
            this.txtCoarseFine = new System.Windows.Forms.TextBox();
            this.button_Accept = new GUI_Controls.Button_Rectangle();
            this.button_CoarseFine = new GUI_Controls.Button_Rectangle();
            this.button_Restore = new GUI_Controls.Button_Rectangle();
            this.button_AxisFreeEnable = new GUI_Controls.Button_Rectangle();
            this.lblAxisFreeEnable = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblAccept
            // 
            this.lblAccept.AutoSize = true;
            this.lblAccept.BackColor = System.Drawing.Color.Transparent;
            this.lblAccept.Font = new System.Drawing.Font("Frutiger LT Std 45 Light", 9.749999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAccept.ForeColor = System.Drawing.Color.Black;
            this.lblAccept.Location = new System.Drawing.Point(180, 53);
            this.lblAccept.Name = "lblAccept";
            this.lblAccept.Size = new System.Drawing.Size(59, 15);
            this.lblAccept.TabIndex = 41;
            this.lblAccept.Text = "Changes";
            this.lblAccept.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblRestore
            // 
            this.lblRestore.AutoSize = true;
            this.lblRestore.BackColor = System.Drawing.Color.Transparent;
            this.lblRestore.Font = new System.Drawing.Font("Frutiger LT Std 45 Light", 9.749999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRestore.ForeColor = System.Drawing.Color.Black;
            this.lblRestore.Location = new System.Drawing.Point(103, 53);
            this.lblRestore.Name = "lblRestore";
            this.lblRestore.Size = new System.Drawing.Size(57, 15);
            this.lblRestore.TabIndex = 42;
            this.lblRestore.Text = "Position";
            this.lblRestore.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblBack
            // 
            this.lblBack.AutoSize = true;
            this.lblBack.BackColor = System.Drawing.Color.Transparent;
            this.lblBack.Font = new System.Drawing.Font("Frutiger LT Std 45 Light", 9.749999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBack.ForeColor = System.Drawing.Color.Black;
            this.lblBack.Location = new System.Drawing.Point(265, 53);
            this.lblBack.Name = "lblBack";
            this.lblBack.Size = new System.Drawing.Size(41, 15);
            this.lblBack.TabIndex = 43;
            this.lblBack.Text = "BACK";
            this.lblBack.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(34, 2);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 19);
            this.label2.TabIndex = 48;
            this.label2.Text = "Coarse";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label2.Click += new System.EventHandler(this.label3_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(35, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 19);
            this.label3.TabIndex = 49;
            this.label3.Text = "Fine";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // lblAbort
            // 
            this.lblAbort.AutoSize = true;
            this.lblAbort.BackColor = System.Drawing.Color.Transparent;
            this.lblAbort.Font = new System.Drawing.Font("Frutiger LT Std 45 Light", 9.749999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAbort.ForeColor = System.Drawing.Color.Black;
            this.lblAbort.Location = new System.Drawing.Point(334, 53);
            this.lblAbort.Name = "lblAbort";
            this.lblAbort.Size = new System.Drawing.Size(49, 15);
            this.lblAbort.TabIndex = 52;
            this.lblAbort.Text = "ABORT";
            this.lblAbort.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblBarcode
            // 
            this.lblBarcode.AutoSize = true;
            this.lblBarcode.BackColor = System.Drawing.Color.Transparent;
            this.lblBarcode.Font = new System.Drawing.Font("Frutiger LT Std 45 Light", 9.749999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBarcode.ForeColor = System.Drawing.Color.Black;
            this.lblBarcode.Location = new System.Drawing.Point(105, 137);
            this.lblBarcode.Name = "lblBarcode";
            this.lblBarcode.Size = new System.Drawing.Size(56, 15);
            this.lblBarcode.TabIndex = 54;
            this.lblBarcode.Text = "Barcode";
            this.lblBarcode.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // robosepModelButton1
            // 
            this.robosepModelButton1.BackColor = System.Drawing.Color.Transparent;
            this.robosepModelButton1.IsStripperArmEngaged = false;
            this.robosepModelButton1.Location = new System.Drawing.Point(0, 87);
            this.robosepModelButton1.Name = "robosepModelButton1";
            this.robosepModelButton1.Size = new System.Drawing.Size(378, 285);
            this.robosepModelButton1.TabIndex = 46;
            this.robosepModelButton1.CarouselEnable_Click += new System.EventHandler(this.btnCarouselEnable_Click);
            this.robosepModelButton1.ThetaEnable_Click += new System.EventHandler(this.btnThetaEnable_Click);
            this.robosepModelButton1.VertArmEnable_Click += new System.EventHandler(this.btnZEnable_Click);
            this.robosepModelButton1.CarouselInc_Click += new System.EventHandler(this.btnCarouselInc_Click);
            this.robosepModelButton1.CarouselDec_Click += new System.EventHandler(this.btnCarouselDec_Click);
            this.robosepModelButton1.VertArmInc_Click += new System.EventHandler(this.btnZInc_Click);
            this.robosepModelButton1.VertArmDec_Click += new System.EventHandler(this.btnZDec_Click);
            this.robosepModelButton1.ThetaDec_Click += new System.EventHandler(this.btnThetaDec_Click);
            this.robosepModelButton1.ThetaInc_Click += new System.EventHandler(this.btnThetaInc_Click);
            this.robosepModelButton1.Stripper_Click += new System.EventHandler(this.button_StripperArm_Click);
            this.robosepModelButton1.StripperEnable_Click += new System.EventHandler(this.button_StripperArmEnable_Click);
            // 
            // button_Barcode
            // 
            this.button_Barcode.AccessibleName = "  ";
            this.button_Barcode.BackgroundImage = global::Tesla.Service.Properties.Resources.barcode_0;
            this.button_Barcode.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Barcode.Check = false;
            this.button_Barcode.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button_Barcode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.button_Barcode.Location = new System.Drawing.Point(102, 71);
            this.button_Barcode.Name = "button_Barcode";
            this.button_Barcode.Size = new System.Drawing.Size(61, 63);
            this.button_Barcode.TabIndex = 53;
            this.button_Barcode.Click += new System.EventHandler(this.button_Barcode_Click);
            // 
            // button_Back
            // 
            this.button_Back.AccessibleName = "  ";
            this.button_Back.BackgroundImage = global::Tesla.Service.Properties.Resources.BackButton0;
            this.button_Back.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Back.Check = false;
            this.button_Back.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button_Back.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.button_Back.Location = new System.Drawing.Point(250, -7);
            this.button_Back.Name = "button_Back";
            this.button_Back.Size = new System.Drawing.Size(76, 63);
            this.button_Back.TabIndex = 40;
            this.button_Back.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // button_abort
            // 
            this.button_abort.AccessibleName = "  ";
            this.button_abort.BackgroundImage = global::Tesla.Service.Properties.Resources.AbortButton0;
            this.button_abort.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_abort.Check = false;
            this.button_abort.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button_abort.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.button_abort.Location = new System.Drawing.Point(333, 0);
            this.button_abort.Name = "button_abort";
            this.button_abort.Size = new System.Drawing.Size(53, 53);
            this.button_abort.TabIndex = 51;
            this.button_abort.Click += new System.EventHandler(this.button_abort_Click);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::Tesla.Service.Properties.Resources.stepSize;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtUnits);
            this.panel1.Controls.Add(this.txtCoarseFine);
            this.panel1.Location = new System.Drawing.Point(3, 60);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(82, 62);
            this.panel1.TabIndex = 37;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(3, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 19);
            this.label1.TabIndex = 47;
            this.label1.Text = "Step Size:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // txtUnits
            // 
            this.txtUnits.BackColor = System.Drawing.Color.White;
            this.txtUnits.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtUnits.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUnits.Location = new System.Drawing.Point(50, 32);
            this.txtUnits.Name = "txtUnits";
            this.txtUnits.ReadOnly = true;
            this.txtUnits.Size = new System.Drawing.Size(25, 15);
            this.txtUnits.TabIndex = 24;
            this.txtUnits.TabStop = false;
            this.txtUnits.Text = " ";
            this.txtUnits.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtUnits.MouseClick += new System.Windows.Forms.MouseEventHandler(this.txtUnits_MouseClick);
            // 
            // txtCoarseFine
            // 
            this.txtCoarseFine.BackColor = System.Drawing.Color.White;
            this.txtCoarseFine.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtCoarseFine.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCoarseFine.Location = new System.Drawing.Point(7, 29);
            this.txtCoarseFine.Name = "txtCoarseFine";
            this.txtCoarseFine.Size = new System.Drawing.Size(42, 20);
            this.txtCoarseFine.TabIndex = 23;
            this.txtCoarseFine.Text = "0";
            this.txtCoarseFine.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCoarseFine.WordWrap = false;
            this.txtCoarseFine.MouseClick += new System.Windows.Forms.MouseEventHandler(this.txtCoarseFine_MouseClick);
            // 
            // button_Accept
            // 
            this.button_Accept.AccessibleName = "  ";
            this.button_Accept.BackgroundImage = global::Tesla.Service.Properties.Resources.AcceptButton0;
            this.button_Accept.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Accept.Check = false;
            this.button_Accept.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button_Accept.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.button_Accept.Location = new System.Drawing.Point(167, -8);
            this.button_Accept.Name = "button_Accept";
            this.button_Accept.Size = new System.Drawing.Size(95, 63);
            this.button_Accept.TabIndex = 38;
            this.button_Accept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // button_CoarseFine
            // 
            this.button_CoarseFine.AccessibleName = "  ";
            this.button_CoarseFine.BackgroundImage = global::Tesla.Service.Properties.Resources.CourseFine1;
            this.button_CoarseFine.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_CoarseFine.Check = false;
            this.button_CoarseFine.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button_CoarseFine.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.button_CoarseFine.Location = new System.Drawing.Point(3, -1);
            this.button_CoarseFine.Name = "button_CoarseFine";
            this.button_CoarseFine.Size = new System.Drawing.Size(72, 61);
            this.button_CoarseFine.TabIndex = 2;
            this.button_CoarseFine.Click += new System.EventHandler(this.btnCoarseFine_Click);
            // 
            // button_Restore
            // 
            this.button_Restore.AccessibleName = "  ";
            this.button_Restore.BackgroundImage = global::Tesla.Service.Properties.Resources.RestoreButton0;
            this.button_Restore.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Restore.Check = false;
            this.button_Restore.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button_Restore.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.button_Restore.Location = new System.Drawing.Point(90, -7);
            this.button_Restore.Name = "button_Restore";
            this.button_Restore.Size = new System.Drawing.Size(88, 63);
            this.button_Restore.TabIndex = 39;
            this.button_Restore.Click += new System.EventHandler(this.btnResetPosition_Click);
            // 
            // button_AxisFreeEnable
            // 
            this.button_AxisFreeEnable.AccessibleName = "  ";
            this.button_AxisFreeEnable.BackgroundImage = global::Tesla.Service.Properties.Resources.BackButton0;
            this.button_AxisFreeEnable.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_AxisFreeEnable.Check = false;
            this.button_AxisFreeEnable.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button_AxisFreeEnable.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.button_AxisFreeEnable.Location = new System.Drawing.Point(-10, 293);
            this.button_AxisFreeEnable.Name = "button_AxisFreeEnable";
            this.button_AxisFreeEnable.Size = new System.Drawing.Size(76, 63);
            this.button_AxisFreeEnable.TabIndex = 55;
            this.button_AxisFreeEnable.Visible = false;
            this.button_AxisFreeEnable.Click += new System.EventHandler(this.button_AxisFreeEnable_Click);
            // 
            // lblAxisFreeEnable
            // 
            this.lblAxisFreeEnable.AutoSize = true;
            this.lblAxisFreeEnable.BackColor = System.Drawing.Color.Transparent;
            this.lblAxisFreeEnable.Font = new System.Drawing.Font("Frutiger LT Std 45 Light", 9.749999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAxisFreeEnable.ForeColor = System.Drawing.Color.Black;
            this.lblAxisFreeEnable.Location = new System.Drawing.Point(5, 353);
            this.lblAxisFreeEnable.Name = "lblAxisFreeEnable";
            this.lblAxisFreeEnable.Size = new System.Drawing.Size(33, 15);
            this.lblAxisFreeEnable.TabIndex = 56;
            this.lblAxisFreeEnable.Text = "Free";
            this.lblAxisFreeEnable.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblAxisFreeEnable.Visible = false;
            // 
            // RoboSepServiceManualStepControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Controls.Add(this.button_AxisFreeEnable);
            this.Controls.Add(this.lblAxisFreeEnable);
            this.Controls.Add(this.lblBarcode);
            this.Controls.Add(this.button_Barcode);
            this.Controls.Add(this.lblAbort);
            this.Controls.Add(this.button_Back);
            this.Controls.Add(this.button_abort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button_Accept);
            this.Controls.Add(this.lblAccept);
            this.Controls.Add(this.button_CoarseFine);
            this.Controls.Add(this.lblRestore);
            this.Controls.Add(this.button_Restore);
            this.Controls.Add(this.lblBack);
            this.Controls.Add(this.robosepModelButton1);
            this.DoubleBuffered = true;
            this.Name = "RoboSepServiceManualStepControl";
            this.Size = new System.Drawing.Size(403, 370);
            this.EnabledChanged += new System.EventHandler(this.RoboSepServiceManualStepControl_EnabledChanged);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtCoarseFine;
        private DoubleBufferPanel panel1;
        private GUI_Controls.Button_Rectangle button_CoarseFine;
        private GUI_Controls.Button_Rectangle button_Accept;
        private GUI_Controls.Button_Rectangle button_Back;
        private System.Windows.Forms.Label lblAccept;
        private System.Windows.Forms.Label lblRestore;
        private System.Windows.Forms.Label lblBack;
        private GUI_Controls.Button_Rectangle button_Restore;
        private System.Windows.Forms.TextBox txtUnits;
        private RobosepModelButton robosepModelButton1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private GUI_Controls.Button_Rectangle button_abort;
        private System.Windows.Forms.Label lblAbort;
        private GUI_Controls.Button_Rectangle button_Barcode;
        private System.Windows.Forms.Label lblBarcode;
        private GUI_Controls.Button_Rectangle button_AxisFreeEnable;
        private System.Windows.Forms.Label lblAxisFreeEnable;
    }
}
