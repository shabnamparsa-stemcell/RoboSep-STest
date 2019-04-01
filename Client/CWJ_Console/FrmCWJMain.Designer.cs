namespace CWJ_Console
{
    partial class FrmCWJMain
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
            this.m_Label_Main = new System.Windows.Forms.Label();
            this.m_Timer = new System.Windows.Forms.Timer(this.components);
            this.m_Button_Close = new System.Windows.Forms.Button();
            this.m_Label_InsVer = new System.Windows.Forms.Label();
            this.m_Label_InsSerial = new System.Windows.Forms.Label();
            this.m_Label_InsAdd = new System.Windows.Forms.Label();
            this.m_Label_GateVer = new System.Windows.Forms.Label();
            this.m_Label_URL = new System.Windows.Forms.Label();
            this.m_Label_SvrTime = new System.Windows.Forms.Label();
            this.m_Button_Scan = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_Label_Main
            // 
            this.m_Label_Main.AutoSize = true;
            this.m_Label_Main.Location = new System.Drawing.Point(12, 9);
            this.m_Label_Main.Name = "m_Label_Main";
            this.m_Label_Main.Size = new System.Drawing.Size(64, 13);
            this.m_Label_Main.TabIndex = 0;
            this.m_Label_Main.Text = "asdadsasda";
            // 
            // m_Timer
            // 
            this.m_Timer.Tick += new System.EventHandler(this.m_Timer_Tick);
            // 
            // m_Button_Close
            // 
            this.m_Button_Close.Location = new System.Drawing.Point(13, 235);
            this.m_Button_Close.Name = "m_Button_Close";
            this.m_Button_Close.Size = new System.Drawing.Size(201, 23);
            this.m_Button_Close.TabIndex = 1;
            this.m_Button_Close.Text = "Close RoboSep";
            this.m_Button_Close.UseVisualStyleBackColor = true;
            this.m_Button_Close.Click += new System.EventHandler(this.m_Button_Close_Click);
            // 
            // m_Label_InsVer
            // 
            this.m_Label_InsVer.AutoSize = true;
            this.m_Label_InsVer.Location = new System.Drawing.Point(12, 39);
            this.m_Label_InsVer.Name = "m_Label_InsVer";
            this.m_Label_InsVer.Size = new System.Drawing.Size(83, 13);
            this.m_Label_InsVer.TabIndex = 2;
            this.m_Label_InsVer.Text = "m_Label_InsVer";
            // 
            // m_Label_InsSerial
            // 
            this.m_Label_InsSerial.AutoSize = true;
            this.m_Label_InsSerial.Location = new System.Drawing.Point(12, 69);
            this.m_Label_InsSerial.Name = "m_Label_InsSerial";
            this.m_Label_InsSerial.Size = new System.Drawing.Size(93, 13);
            this.m_Label_InsSerial.TabIndex = 3;
            this.m_Label_InsSerial.Text = "m_Label_InsSerial";
            // 
            // m_Label_InsAdd
            // 
            this.m_Label_InsAdd.AutoSize = true;
            this.m_Label_InsAdd.Location = new System.Drawing.Point(12, 98);
            this.m_Label_InsAdd.Name = "m_Label_InsAdd";
            this.m_Label_InsAdd.Size = new System.Drawing.Size(86, 13);
            this.m_Label_InsAdd.TabIndex = 4;
            this.m_Label_InsAdd.Text = "m_Label_InsAdd";
            // 
            // m_Label_GateVer
            // 
            this.m_Label_GateVer.AutoSize = true;
            this.m_Label_GateVer.Location = new System.Drawing.Point(12, 127);
            this.m_Label_GateVer.Name = "m_Label_GateVer";
            this.m_Label_GateVer.Size = new System.Drawing.Size(92, 13);
            this.m_Label_GateVer.TabIndex = 5;
            this.m_Label_GateVer.Text = "m_Label_GateVer";
            // 
            // m_Label_URL
            // 
            this.m_Label_URL.AutoSize = true;
            this.m_Label_URL.Location = new System.Drawing.Point(12, 156);
            this.m_Label_URL.Name = "m_Label_URL";
            this.m_Label_URL.Size = new System.Drawing.Size(75, 13);
            this.m_Label_URL.TabIndex = 6;
            this.m_Label_URL.Text = "m_Label_URL";
            // 
            // m_Label_SvrTime
            // 
            this.m_Label_SvrTime.AutoSize = true;
            this.m_Label_SvrTime.Location = new System.Drawing.Point(12, 187);
            this.m_Label_SvrTime.Name = "m_Label_SvrTime";
            this.m_Label_SvrTime.Size = new System.Drawing.Size(92, 13);
            this.m_Label_SvrTime.TabIndex = 7;
            this.m_Label_SvrTime.Text = "m_Label_SvrTime";
            this.m_Label_SvrTime.VisibleChanged += new System.EventHandler(this.m_Label_SvrTime_VisibleChanged);
            // 
            // m_Button_Scan
            // 
            this.m_Button_Scan.Location = new System.Drawing.Point(232, 235);
            this.m_Button_Scan.Name = "m_Button_Scan";
            this.m_Button_Scan.Size = new System.Drawing.Size(171, 23);
            this.m_Button_Scan.TabIndex = 8;
            this.m_Button_Scan.Text = "Scan Vials";
            this.m_Button_Scan.UseVisualStyleBackColor = true;
            this.m_Button_Scan.Click += new System.EventHandler(this.m_Button_Scan_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(419, 22);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(99, 21);
            this.button1.TabIndex = 9;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(419, 49);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(99, 23);
            this.button2.TabIndex = 10;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(419, 78);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(99, 25);
            this.button3.TabIndex = 11;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(419, 109);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(99, 23);
            this.button4.TabIndex = 12;
            this.button4.Text = "button4";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(419, 146);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(99, 23);
            this.button5.TabIndex = 13;
            this.button5.Text = "button5";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // FrmCWJMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(543, 270);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.m_Button_Scan);
            this.Controls.Add(this.m_Label_SvrTime);
            this.Controls.Add(this.m_Label_URL);
            this.Controls.Add(this.m_Label_GateVer);
            this.Controls.Add(this.m_Label_InsAdd);
            this.Controls.Add(this.m_Label_InsSerial);
            this.Controls.Add(this.m_Label_InsVer);
            this.Controls.Add(this.m_Button_Close);
            this.Controls.Add(this.m_Label_Main);
            this.Name = "FrmCWJMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RoboSep Startup Test";
            this.Load += new System.EventHandler(this.FrmCWJMain_Load);
            this.Shown += new System.EventHandler(this.FrmCWJMain_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_Label_Main;
        private System.Windows.Forms.Timer m_Timer;
        private System.Windows.Forms.Button m_Button_Close;
        private System.Windows.Forms.Label m_Label_InsVer;
        private System.Windows.Forms.Label m_Label_InsSerial;
        private System.Windows.Forms.Label m_Label_InsAdd;
        private System.Windows.Forms.Label m_Label_GateVer;
        private System.Windows.Forms.Label m_Label_URL;
        private System.Windows.Forms.Label m_Label_SvrTime;
        private System.Windows.Forms.Button m_Button_Scan;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
    }
}

