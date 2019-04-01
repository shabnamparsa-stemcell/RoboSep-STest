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
            this.textBox_value = new System.Windows.Forms.TextBox();
            this.theMin = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label_WindowTitle = new System.Windows.Forms.Label();
            this.rectangleShape1 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.text_instructions = new System.Windows.Forms.Label();
            this.button_9 = new GUI_Controls.Button_Rectangle();
            this.button_5 = new GUI_Controls.Button_Rectangle();
            this.button_3 = new GUI_Controls.Button_Rectangle();
            this.button_1 = new GUI_Controls.Button_Rectangle();
            this.button_2 = new GUI_Controls.Button_Rectangle();
            this.button_clear = new GUI_Controls.Button_Rectangle();
            this.button_4 = new GUI_Controls.Button_Rectangle();
            this.button_7 = new GUI_Controls.Button_Rectangle();
            this.button_0 = new GUI_Controls.Button_Rectangle();
            this.button_8 = new GUI_Controls.Button_Rectangle();
            this.button_cancel = new GUI_Controls.Button_Rectangle();
            this.button_point = new GUI_Controls.Button_Rectangle();
            this.button_6 = new GUI_Controls.Button_Rectangle();
            this.button_enter = new GUI_Controls.Button_Rectangle();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox_value
            // 
            this.textBox_value.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_value.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_value.Location = new System.Drawing.Point(32, 137);
            this.textBox_value.Name = "textBox_value";
            this.textBox_value.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.textBox_value.Size = new System.Drawing.Size(219, 35);
            this.textBox_value.TabIndex = 0;
            this.textBox_value.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBox_value.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_value_KeyPress);
            // 
            // theMin
            // 
            this.theMin.AutoSize = true;
            this.theMin.BackColor = System.Drawing.Color.Transparent;
            this.theMin.Font = new System.Drawing.Font("Arial", 10F);
            this.theMin.Location = new System.Drawing.Point(12, 117);
            this.theMin.Name = "theMin";
            this.theMin.Size = new System.Drawing.Size(30, 16);
            this.theMin.TabIndex = 42;
            this.theMin.Text = "min";
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.Controls.Add(this.label_WindowTitle);
            this.panel1.Location = new System.Drawing.Point(1, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(293, 37);
            this.panel1.TabIndex = 98;
            // 
            // label_WindowTitle
            // 
            this.label_WindowTitle.BackColor = System.Drawing.Color.Transparent;
            this.label_WindowTitle.Font = new System.Drawing.Font("Arial", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_WindowTitle.ForeColor = System.Drawing.Color.White;
            this.label_WindowTitle.Location = new System.Drawing.Point(6, 5);
            this.label_WindowTitle.Margin = new System.Windows.Forms.Padding(0);
            this.label_WindowTitle.Name = "label_WindowTitle";
            this.label_WindowTitle.Size = new System.Drawing.Size(188, 27);
            this.label_WindowTitle.TabIndex = 3;
            this.label_WindowTitle.Text = "Sample Volume";
            this.label_WindowTitle.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // rectangleShape1
            // 
            this.rectangleShape1.BorderWidth = 2;
            this.rectangleShape1.Location = new System.Drawing.Point(0, 0);
            this.rectangleShape1.Name = "rectangleShape1";
            this.rectangleShape1.Size = new System.Drawing.Size(295, 412);
            this.rectangleShape1.Click += new System.EventHandler(this.button_space_Click);
            // 
            // shapeContainer1
            // 
            this.shapeContainer1.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer1.Name = "shapeContainer1";
            this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.rectangleShape1});
            this.shapeContainer1.Size = new System.Drawing.Size(297, 413);
            this.shapeContainer1.TabIndex = 99;
            this.shapeContainer1.TabStop = false;
            // 
            // text_instructions
            // 
            this.text_instructions.BackColor = System.Drawing.Color.Transparent;
            this.text_instructions.Font = new System.Drawing.Font("Arial", 10F);
            this.text_instructions.Location = new System.Drawing.Point(12, 41);
            this.text_instructions.Name = "text_instructions";
            this.text_instructions.Size = new System.Drawing.Size(272, 76);
            this.text_instructions.TabIndex = 101;
            this.text_instructions.Text = "Write instructions here.";
            this.text_instructions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button_9
            // 
            this.button_9.BackColor = System.Drawing.Color.Transparent;
            this.button_9.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_9.BackgroundImage")));
            this.button_9.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_9.Check = false;
            this.button_9.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_9.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button_9.Location = new System.Drawing.Point(136, 178);
            this.button_9.Name = "button_9";
            this.button_9.Size = new System.Drawing.Size(48, 50);
            this.button_9.TabIndex = 29;
            this.button_9.Text = "9";
            this.button_9.Click += new System.EventHandler(this.button_9_Click);
            // 
            // button_5
            // 
            this.button_5.BackColor = System.Drawing.Color.Transparent;
            this.button_5.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_5.BackgroundImage")));
            this.button_5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_5.Check = false;
            this.button_5.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button_5.Location = new System.Drawing.Point(81, 234);
            this.button_5.Name = "button_5";
            this.button_5.Size = new System.Drawing.Size(48, 50);
            this.button_5.TabIndex = 31;
            this.button_5.Text = "5";
            this.button_5.Click += new System.EventHandler(this.button_5_Click);
            // 
            // button_3
            // 
            this.button_3.BackColor = System.Drawing.Color.Transparent;
            this.button_3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_3.BackgroundImage")));
            this.button_3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_3.Check = false;
            this.button_3.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button_3.Location = new System.Drawing.Point(136, 290);
            this.button_3.Name = "button_3";
            this.button_3.Size = new System.Drawing.Size(48, 50);
            this.button_3.TabIndex = 35;
            this.button_3.Text = "3";
            this.button_3.Click += new System.EventHandler(this.button_3_Click);
            // 
            // button_1
            // 
            this.button_1.BackColor = System.Drawing.Color.Transparent;
            this.button_1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_1.BackgroundImage")));
            this.button_1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_1.Check = false;
            this.button_1.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button_1.Location = new System.Drawing.Point(26, 290);
            this.button_1.Name = "button_1";
            this.button_1.Size = new System.Drawing.Size(48, 50);
            this.button_1.TabIndex = 33;
            this.button_1.Text = "1";
            this.button_1.Click += new System.EventHandler(this.button_1_Click);
            // 
            // button_2
            // 
            this.button_2.BackColor = System.Drawing.Color.Transparent;
            this.button_2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_2.BackgroundImage")));
            this.button_2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_2.Check = false;
            this.button_2.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button_2.Location = new System.Drawing.Point(81, 290);
            this.button_2.Name = "button_2";
            this.button_2.Size = new System.Drawing.Size(48, 50);
            this.button_2.TabIndex = 34;
            this.button_2.Text = "2";
            this.button_2.Click += new System.EventHandler(this.button_2_Click);
            // 
            // button_clear
            // 
            this.button_clear.BackColor = System.Drawing.Color.Transparent;
            this.button_clear.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_clear.BackgroundImage")));
            this.button_clear.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_clear.Check = false;
            this.button_clear.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_clear.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button_clear.Location = new System.Drawing.Point(191, 178);
            this.button_clear.Name = "button_clear";
            this.button_clear.Size = new System.Drawing.Size(72, 50);
            this.button_clear.TabIndex = 38;
            this.button_clear.Text = "Clear";
            this.button_clear.Click += new System.EventHandler(this.button_clear_Click);
            // 
            // button_4
            // 
            this.button_4.BackColor = System.Drawing.Color.Transparent;
            this.button_4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_4.BackgroundImage")));
            this.button_4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_4.Check = false;
            this.button_4.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button_4.Location = new System.Drawing.Point(26, 234);
            this.button_4.Name = "button_4";
            this.button_4.Size = new System.Drawing.Size(48, 50);
            this.button_4.TabIndex = 30;
            this.button_4.Text = "4";
            this.button_4.Click += new System.EventHandler(this.button_4_Click);
            // 
            // button_7
            // 
            this.button_7.BackColor = System.Drawing.Color.Transparent;
            this.button_7.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_7.BackgroundImage")));
            this.button_7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_7.Check = false;
            this.button_7.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_7.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button_7.Location = new System.Drawing.Point(26, 178);
            this.button_7.Name = "button_7";
            this.button_7.Size = new System.Drawing.Size(48, 50);
            this.button_7.TabIndex = 27;
            this.button_7.Text = "7";
            this.button_7.Click += new System.EventHandler(this.button_7_Click);
            // 
            // button_0
            // 
            this.button_0.BackColor = System.Drawing.Color.Transparent;
            this.button_0.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_0.BackgroundImage")));
            this.button_0.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_0.Check = false;
            this.button_0.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button_0.Location = new System.Drawing.Point(81, 346);
            this.button_0.Name = "button_0";
            this.button_0.Size = new System.Drawing.Size(48, 50);
            this.button_0.TabIndex = 36;
            this.button_0.Text = "0";
            this.button_0.Click += new System.EventHandler(this.button_0_Click);
            // 
            // button_8
            // 
            this.button_8.BackColor = System.Drawing.Color.Transparent;
            this.button_8.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_8.BackgroundImage")));
            this.button_8.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_8.Check = false;
            this.button_8.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button_8.Location = new System.Drawing.Point(81, 178);
            this.button_8.Name = "button_8";
            this.button_8.Size = new System.Drawing.Size(48, 50);
            this.button_8.TabIndex = 28;
            this.button_8.Text = "8";
            this.button_8.Click += new System.EventHandler(this.button_8_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.BackColor = System.Drawing.Color.Transparent;
            this.button_cancel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_cancel.BackgroundImage")));
            this.button_cancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_cancel.Check = false;
            this.button_cancel.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_cancel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button_cancel.Location = new System.Drawing.Point(191, 234);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(72, 50);
            this.button_cancel.TabIndex = 39;
            this.button_cancel.Text = "Cancel";
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // button_point
            // 
            this.button_point.BackColor = System.Drawing.Color.Transparent;
            this.button_point.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_point.BackgroundImage")));
            this.button_point.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_point.Check = false;
            this.button_point.Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_point.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button_point.Location = new System.Drawing.Point(136, 346);
            this.button_point.Margin = new System.Windows.Forms.Padding(5);
            this.button_point.Name = "button_point";
            this.button_point.Size = new System.Drawing.Size(50, 50);
            this.button_point.TabIndex = 37;
            this.button_point.Text = ".";
            this.button_point.Click += new System.EventHandler(this.button_point_Click);
            // 
            // button_6
            // 
            this.button_6.BackColor = System.Drawing.Color.Transparent;
            this.button_6.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_6.BackgroundImage")));
            this.button_6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_6.Check = false;
            this.button_6.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button_6.Location = new System.Drawing.Point(136, 234);
            this.button_6.Name = "button_6";
            this.button_6.Size = new System.Drawing.Size(48, 50);
            this.button_6.TabIndex = 32;
            this.button_6.Text = "6";
            this.button_6.Click += new System.EventHandler(this.button_6_Click);
            // 
            // button_enter
            // 
            this.button_enter.BackColor = System.Drawing.Color.Transparent;
            this.button_enter.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_enter.BackgroundImage")));
            this.button_enter.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_enter.Check = false;
            this.button_enter.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_enter.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button_enter.Location = new System.Drawing.Point(191, 290);
            this.button_enter.Name = "button_enter";
            this.button_enter.Size = new System.Drawing.Size(72, 106);
            this.button_enter.TabIndex = 43;
            this.button_enter.Text = "Enter";
            this.button_enter.Click += new System.EventHandler(this.button_enter_Click);
            // 
            // NumberPad
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(297, 413);
            this.Controls.Add(this.text_instructions);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button_9);
            this.Controls.Add(this.button_5);
            this.Controls.Add(this.button_3);
            this.Controls.Add(this.button_1);
            this.Controls.Add(this.button_2);
            this.Controls.Add(this.button_clear);
            this.Controls.Add(this.theMin);
            this.Controls.Add(this.button_4);
            this.Controls.Add(this.button_7);
            this.Controls.Add(this.button_0);
            this.Controls.Add(this.button_8);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.button_point);
            this.Controls.Add(this.button_6);
            this.Controls.Add(this.button_enter);
            this.Controls.Add(this.textBox_value);
            this.Controls.Add(this.shapeContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "NumberPad";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Deactivate += new System.EventHandler(this.NumberPad_Deactivate);
            this.Load += new System.EventHandler(this.NumberPad_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_value;
        private GUI_Controls.Button_Rectangle button_7;
        private GUI_Controls.Button_Rectangle button_8;
        private GUI_Controls.Button_Rectangle button_9;
        private GUI_Controls.Button_Rectangle button_4;
        private GUI_Controls.Button_Rectangle button_5;
        private GUI_Controls.Button_Rectangle button_6;
        private GUI_Controls.Button_Rectangle button_1;
        private GUI_Controls.Button_Rectangle button_2;
        private GUI_Controls.Button_Rectangle button_3;
        private GUI_Controls.Button_Rectangle button_0;
        private GUI_Controls.Button_Rectangle button_point;
        private GUI_Controls.Button_Rectangle button_clear;
        private GUI_Controls.Button_Rectangle button_cancel;
        private System.Windows.Forms.Label theMin;
        private Button_Rectangle button_enter;
        private System.Windows.Forms.Panel panel1;
        protected System.Windows.Forms.Label label_WindowTitle;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape1;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private System.Windows.Forms.Label text_instructions;
    }
}
