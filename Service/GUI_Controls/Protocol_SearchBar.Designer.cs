namespace GUI_Controls
{
    partial class Protocol_SearchBar
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
            this.textBox_search = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button_negative = new GUI_Controls.Button_Filter();
            this.button_positive = new GUI_Controls.Button_Filter();
            this.button_mouse = new GUI_Controls.Button_Filter();
            this.button_human = new GUI_Controls.Button_Filter();
            this.SuspendLayout();
            // 
            // textBox_search
            // 
            this.textBox_search.BackColor = System.Drawing.SystemColors.Control;
            this.textBox_search.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_search.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_search.Location = new System.Drawing.Point(21, 21);
            this.textBox_search.Name = "textBox_search";
            this.textBox_search.Size = new System.Drawing.Size(93, 27);
            this.textBox_search.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(18, -2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 24);
            this.label1.TabIndex = 5;
            this.label1.Text = "search:";
            // 
            // button_negative
            // 
            this.button_negative.AccessibleName = "  ";
            this.button_negative.BackgroundImage = global::GUI_Controls.Properties.Resources.BUTTON_NEGATIVE0;
            this.button_negative.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_negative.Check = false;
            this.button_negative.Location = new System.Drawing.Point(302, 2);
            this.button_negative.Name = "button_negative";
            this.button_negative.Size = new System.Drawing.Size(46, 47);
            this.button_negative.TabIndex = 3;
            this.button_negative.Click += new System.EventHandler(this.button_negative_Click);
            // 
            // button_positive
            // 
            this.button_positive.AccessibleName = "  ";
            this.button_positive.BackgroundImage = global::GUI_Controls.Properties.Resources.BUTTON_POSITIVE0;
            this.button_positive.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_positive.Check = false;
            this.button_positive.Location = new System.Drawing.Point(250, 2);
            this.button_positive.Name = "button_positive";
            this.button_positive.Size = new System.Drawing.Size(46, 47);
            this.button_positive.TabIndex = 2;
            this.button_positive.Click += new System.EventHandler(this.button_positive_Click);
            // 
            // button_mouse
            // 
            this.button_mouse.AccessibleName = "  ";
            this.button_mouse.BackgroundImage = global::GUI_Controls.Properties.Resources.BUTTON_MOUSE0;
            this.button_mouse.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_mouse.Check = false;
            this.button_mouse.Location = new System.Drawing.Point(181, 2);
            this.button_mouse.Name = "button_mouse";
            this.button_mouse.Size = new System.Drawing.Size(46, 47);
            this.button_mouse.TabIndex = 1;
            this.button_mouse.Click += new System.EventHandler(this.button_mouse_Click);
            // 
            // button_human
            // 
            this.button_human.AccessibleName = "  ";
            this.button_human.BackgroundImage = global::GUI_Controls.Properties.Resources.BUTTON_HUMAN0;
            this.button_human.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_human.Check = false;
            this.button_human.Location = new System.Drawing.Point(127, 2);
            this.button_human.Name = "button_human";
            this.button_human.Size = new System.Drawing.Size(46, 47);
            this.button_human.TabIndex = 0;
            this.button_human.Click += new System.EventHandler(this.button_human_Click);
            // 
            // Protocol_SearchBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::GUI_Controls.Properties.Resources.BG_SearchBar;
            this.Controls.Add(this.textBox_search);
            this.Controls.Add(this.button_negative);
            this.Controls.Add(this.button_positive);
            this.Controls.Add(this.button_mouse);
            this.Controls.Add(this.button_human);
            this.Controls.Add(this.label1);
            this.Name = "Protocol_SearchBar";
            this.Size = new System.Drawing.Size(351, 51);
            this.Load += new System.EventHandler(this.Protocol_SearchBar_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        public Button_Filter button_human;
        public Button_Filter button_mouse;
        public Button_Filter button_negative;
        public Button_Filter button_positive;
        public System.Windows.Forms.TextBox textBox_search;
    }
}
