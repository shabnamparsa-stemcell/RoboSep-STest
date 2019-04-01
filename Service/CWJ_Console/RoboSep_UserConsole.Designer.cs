namespace GUI_Console
{
    partial class RoboSep_UserConsole
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoboSep_UserConsole));
            this.m_Timer = new System.Windows.Forms.Timer(this.components);
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.button_Refresh = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.errorTimer = new System.Windows.Forms.Timer(this.components);
            this.basePannel_Empty = new GUI_Console.BasePannel();
            this.SuspendLayout();
            // 
            // m_Timer
            // 
            this.m_Timer.Tick += new System.EventHandler(this.m_Timer_Tick);
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.textBox2.Location = new System.Drawing.Point(646, 74);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(209, 35);
            this.textBox2.TabIndex = 2;
            // 
            // textBox3
            // 
            this.textBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.textBox3.Location = new System.Drawing.Point(646, 115);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(209, 35);
            this.textBox3.TabIndex = 3;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.richTextBox1.Location = new System.Drawing.Point(646, 6);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(326, 62);
            this.richTextBox1.TabIndex = 5;
            this.richTextBox1.Text = "";
            // 
            // button_Refresh
            // 
            this.button_Refresh.Location = new System.Drawing.Point(646, 434);
            this.button_Refresh.Name = "button_Refresh";
            this.button_Refresh.Size = new System.Drawing.Size(111, 44);
            this.button_Refresh.TabIndex = 6;
            this.button_Refresh.Text = "Refresh All";
            this.button_Refresh.UseVisualStyleBackColor = true;
            this.button_Refresh.Click += new System.EventHandler(this.button_Refresh_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(646, 156);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(318, 277);
            this.listBox1.TabIndex = 7;
            // 
            // errorTimer
            // 
            this.errorTimer.Interval = 50;
            this.errorTimer.Tick += new System.EventHandler(this.errorTimer_Tick);
            // 
            // basePannel_Empty
            // 
            this.basePannel_Empty.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("basePannel_Empty.BackgroundImage")));
            this.basePannel_Empty.Location = new System.Drawing.Point(0, 0);
            this.basePannel_Empty.Name = "basePannel_Empty";
            this.basePannel_Empty.Size = new System.Drawing.Size(640, 480);
            this.basePannel_Empty.TabIndex = 0;
            // 
            // RoboSep_UserConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(976, 480);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button_Refresh);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.basePannel_Empty);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "RoboSep_UserConsole";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RoboSep";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RoboSep_UserConsole_FormClosing);
            this.Load += new System.EventHandler(this.RoboSep_UserConsole_Load);
            this.LocationChanged += new System.EventHandler(this.RoboSep_UserConsole_LocationChanged);
            this.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.RoboSep_UserConsole_ControlAdded);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer m_Timer;
        private BasePannel basePannel_Empty;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button button_Refresh;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Timer errorTimer;

    }
}

