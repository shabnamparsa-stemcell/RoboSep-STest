namespace GUI_Controls
{
    partial class NumberPad
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NumberPad));
            this.text_instructions = new System.Windows.Forms.RichTextBox();
            this.textBox_value = new System.Windows.Forms.TextBox();
            this.button_point = new GUI_Controls.Button_Keyboard();
            this.button_0 = new GUI_Controls.Button_Keyboard();
            this.button_clear = new GUI_Controls.Button_Keyboard();
            this.button_1 = new GUI_Controls.Button_Keyboard();
            this.button_5 = new GUI_Controls.Button_Keyboard();
            this.button_3 = new GUI_Controls.Button_Keyboard();
            this.button_2 = new GUI_Controls.Button_Keyboard();
            this.button_4 = new GUI_Controls.Button_Keyboard();
            this.button_cancel = new GUI_Controls.Button_Keyboard();
            this.button_enter = new GUI_Controls.Button_Rectangle();
            this.button_6 = new GUI_Controls.Button_Keyboard();
            this.button_9 = new GUI_Controls.Button_Keyboard();
            this.button_8 = new GUI_Controls.Button_Keyboard();
            this.button_7 = new GUI_Controls.Button_Keyboard();
            this.theMin = new System.Windows.Forms.Label();
            this.roboPanel1 = new GUI_Controls.RoboPanel();
            this.roboPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // text_instructions
            // 
            this.text_instructions.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.text_instructions.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.text_instructions.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.text_instructions.Location = new System.Drawing.Point(12, 12);
            this.text_instructions.Name = "text_instructions";
            this.text_instructions.Size = new System.Drawing.Size(276, 85);
            this.text_instructions.TabIndex = 40;
            this.text_instructions.Text = "Information will appear here to help the user.";
            this.text_instructions.Click += new System.EventHandler(this.text_instructions_select);
            this.text_instructions.Enter += new System.EventHandler(this.text_instructions_select);
            // 
            // textBox_value
            // 
            this.textBox_value.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_value.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_value.Location = new System.Drawing.Point(36, 119);
            this.textBox_value.Name = "textBox_value";
            this.textBox_value.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.textBox_value.Size = new System.Drawing.Size(219, 35);
            this.textBox_value.TabIndex = 0;
            this.textBox_value.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBox_value.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_value_KeyPress);
            // 
            // button_point
            // 
            this.button_point.AccessibleName = ".";
            this.button_point.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_point.BackgroundImage")));
            this.button_point.check = false;
            this.button_point.Location = new System.Drawing.Point(150, 333);
            this.button_point.Name = "button_point";
            this.button_point.Size = new System.Drawing.Size(60, 50);
            this.button_point.TabIndex = 37;
            this.button_point.Click += new System.EventHandler(this.button_point_Click);
            // 
            // button_0
            // 
            this.button_0.AccessibleName = "0";
            this.button_0.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_0.BackgroundImage")));
            this.button_0.check = false;
            this.button_0.Location = new System.Drawing.Point(84, 333);
            this.button_0.Name = "button_0";
            this.button_0.Size = new System.Drawing.Size(60, 50);
            this.button_0.TabIndex = 36;
            this.button_0.Click += new System.EventHandler(this.button_0_Click);
            // 
            // button_clear
            // 
            this.button_clear.AccessibleName = "Clear";
            this.button_clear.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_clear.BackgroundImage")));
            this.button_clear.check = false;
            this.button_clear.Location = new System.Drawing.Point(216, 165);
            this.button_clear.Name = "button_clear";
            this.button_clear.Size = new System.Drawing.Size(60, 50);
            this.button_clear.TabIndex = 38;
            this.button_clear.Click += new System.EventHandler(this.button_clear_Click);
            // 
            // button_1
            // 
            this.button_1.AccessibleName = "1";
            this.button_1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_1.BackgroundImage")));
            this.button_1.check = false;
            this.button_1.Location = new System.Drawing.Point(18, 277);
            this.button_1.Name = "button_1";
            this.button_1.Size = new System.Drawing.Size(60, 50);
            this.button_1.TabIndex = 33;
            this.button_1.Click += new System.EventHandler(this.button_1_Click);
            // 
            // button_5
            // 
            this.button_5.AccessibleName = "5";
            this.button_5.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_5.BackgroundImage")));
            this.button_5.check = false;
            this.button_5.Location = new System.Drawing.Point(84, 221);
            this.button_5.Name = "button_5";
            this.button_5.Size = new System.Drawing.Size(60, 50);
            this.button_5.TabIndex = 31;
            this.button_5.Click += new System.EventHandler(this.button_5_Click);
            // 
            // button_3
            // 
            this.button_3.AccessibleName = "3";
            this.button_3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_3.BackgroundImage")));
            this.button_3.check = false;
            this.button_3.Location = new System.Drawing.Point(150, 277);
            this.button_3.Name = "button_3";
            this.button_3.Size = new System.Drawing.Size(60, 50);
            this.button_3.TabIndex = 35;
            this.button_3.Click += new System.EventHandler(this.button_3_Click);
            // 
            // button_2
            // 
            this.button_2.AccessibleName = "2";
            this.button_2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_2.BackgroundImage")));
            this.button_2.check = false;
            this.button_2.Location = new System.Drawing.Point(84, 277);
            this.button_2.Name = "button_2";
            this.button_2.Size = new System.Drawing.Size(60, 50);
            this.button_2.TabIndex = 34;
            this.button_2.Click += new System.EventHandler(this.button_2_Click);
            // 
            // button_4
            // 
            this.button_4.AccessibleName = "4";
            this.button_4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_4.BackgroundImage")));
            this.button_4.check = false;
            this.button_4.Location = new System.Drawing.Point(18, 221);
            this.button_4.Name = "button_4";
            this.button_4.Size = new System.Drawing.Size(60, 50);
            this.button_4.TabIndex = 30;
            this.button_4.Click += new System.EventHandler(this.button_4_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.AccessibleName = "Cancel";
            this.button_cancel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_cancel.BackgroundImage")));
            this.button_cancel.check = false;
            this.button_cancel.Location = new System.Drawing.Point(216, 221);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(60, 50);
            this.button_cancel.TabIndex = 39;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // button_enter
            // 
            this.button_enter.AccessibleName = "Enter";
            this.button_enter.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_enter.BackgroundImage")));
            this.button_enter.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_enter.Check = false;
            this.button_enter.Location = new System.Drawing.Point(216, 277);
            this.button_enter.Name = "button_enter";
            this.button_enter.Size = new System.Drawing.Size(60, 106);
            this.button_enter.TabIndex = 43;
            this.button_enter.Click += new System.EventHandler(this.button_enter_Click);
            // 
            // button_6
            // 
            this.button_6.AccessibleName = "6";
            this.button_6.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_6.BackgroundImage")));
            this.button_6.check = false;
            this.button_6.Location = new System.Drawing.Point(150, 221);
            this.button_6.Name = "button_6";
            this.button_6.Size = new System.Drawing.Size(60, 50);
            this.button_6.TabIndex = 32;
            this.button_6.Click += new System.EventHandler(this.button_6_Click);
            // 
            // button_9
            // 
            this.button_9.AccessibleName = "9";
            this.button_9.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_9.BackgroundImage")));
            this.button_9.check = false;
            this.button_9.Location = new System.Drawing.Point(150, 165);
            this.button_9.Name = "button_9";
            this.button_9.Size = new System.Drawing.Size(60, 50);
            this.button_9.TabIndex = 29;
            this.button_9.Click += new System.EventHandler(this.button_9_Click);
            // 
            // button_8
            // 
            this.button_8.AccessibleName = "8";
            this.button_8.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_8.BackgroundImage")));
            this.button_8.check = false;
            this.button_8.Location = new System.Drawing.Point(84, 165);
            this.button_8.Name = "button_8";
            this.button_8.Size = new System.Drawing.Size(60, 50);
            this.button_8.TabIndex = 28;
            this.button_8.Click += new System.EventHandler(this.button_8_Click);
            // 
            // button_7
            // 
            this.button_7.AccessibleName = "7";
            this.button_7.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_7.BackgroundImage")));
            this.button_7.check = false;
            this.button_7.Location = new System.Drawing.Point(18, 165);
            this.button_7.Name = "button_7";
            this.button_7.Size = new System.Drawing.Size(60, 50);
            this.button_7.TabIndex = 27;
            this.button_7.Click += new System.EventHandler(this.button_7_Click);
            // 
            // theMin
            // 
            this.theMin.AutoSize = true;
            this.theMin.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.theMin.Location = new System.Drawing.Point(17, 94);
            this.theMin.Name = "theMin";
            this.theMin.Size = new System.Drawing.Size(30, 17);
            this.theMin.TabIndex = 42;
            this.theMin.Text = "min";
            // 
            // roboPanel1
            // 
            this.roboPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.roboPanel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.roboPanel1.Controls.Add(this.text_instructions);
            this.roboPanel1.Controls.Add(this.theMin);
            this.roboPanel1.Controls.Add(this.button_7);
            this.roboPanel1.Controls.Add(this.button_8);
            this.roboPanel1.Controls.Add(this.button_9);
            this.roboPanel1.Controls.Add(this.button_6);
            this.roboPanel1.Controls.Add(this.textBox_value);
            this.roboPanel1.Controls.Add(this.button_enter);
            this.roboPanel1.Controls.Add(this.button_point);
            this.roboPanel1.Controls.Add(this.button_cancel);
            this.roboPanel1.Controls.Add(this.button_0);
            this.roboPanel1.Controls.Add(this.button_4);
            this.roboPanel1.Controls.Add(this.button_clear);
            this.roboPanel1.Controls.Add(this.button_2);
            this.roboPanel1.Controls.Add(this.button_1);
            this.roboPanel1.Controls.Add(this.button_3);
            this.roboPanel1.Controls.Add(this.button_5);
            this.roboPanel1.Location = new System.Drawing.Point(0, 0);
            this.roboPanel1.Name = "roboPanel1";
            this.roboPanel1.Size = new System.Drawing.Size(300, 400);
            this.roboPanel1.TabIndex = 44;
            // 
            // NumberPad
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(300, 400);
            this.Controls.Add(this.roboPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "NumberPad";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Deactivate += new System.EventHandler(this.NumberPad_Deactivate);
            this.Load += new System.EventHandler(this.NumberPad_Load);
            this.roboPanel1.ResumeLayout(false);
            this.roboPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_value;
        private GUI_Controls.Button_Keyboard button_7;
        private GUI_Controls.Button_Keyboard button_8;
        private GUI_Controls.Button_Keyboard button_9;
        private GUI_Controls.Button_Keyboard button_4;
        private GUI_Controls.Button_Keyboard button_5;
        private GUI_Controls.Button_Keyboard button_6;
        private GUI_Controls.Button_Keyboard button_1;
        private GUI_Controls.Button_Keyboard button_2;
        private GUI_Controls.Button_Keyboard button_3;
        private GUI_Controls.Button_Keyboard button_0;
        private GUI_Controls.Button_Keyboard button_point;
        private GUI_Controls.Button_Keyboard button_clear;
        private GUI_Controls.Button_Keyboard button_cancel;
        private System.Windows.Forms.RichTextBox text_instructions;
        private System.Windows.Forms.Label theMin;
        private Button_Rectangle button_enter;
        private RoboPanel roboPanel1;
    }
}
