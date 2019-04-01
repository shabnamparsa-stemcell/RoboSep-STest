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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Protocol_SearchBar));
            this.textBox_search = new System.Windows.Forms.TextBox();
            this.lblSearch = new System.Windows.Forms.Label();
            this.button_negative = new GUI_Controls.Button_Filter();
            this.button_positive = new GUI_Controls.Button_Filter();
            this.button_mouse = new GUI_Controls.Button_Filter();
            this.button_human = new GUI_Controls.Button_Filter();
            this.button_mode = new GUI_Controls.Button_Circle();
            this.SuspendLayout();
            // 
            // textBox_search
            // 
            this.textBox_search.BackColor = System.Drawing.Color.White;
            this.textBox_search.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_search.Font = new System.Drawing.Font("Arial Narrow", 14F);
            this.textBox_search.Location = new System.Drawing.Point(66, 5);
            this.textBox_search.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_search.Name = "textBox_search";
            this.textBox_search.Size = new System.Drawing.Size(116, 29);
            this.textBox_search.TabIndex = 4;
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.BackColor = System.Drawing.Color.Transparent;
            this.lblSearch.Font = new System.Drawing.Font("Arial Narrow", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSearch.ForeColor = System.Drawing.SystemColors.Control;
            this.lblSearch.Location = new System.Drawing.Point(-2, 7);
            this.lblSearch.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(68, 25);
            this.lblSearch.TabIndex = 5;
            this.lblSearch.Text = "Search";
            // 
            // button_negative
            // 
            this.button_negative.BackColor = System.Drawing.Color.Transparent;
            this.button_negative.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_negative.BackgroundImage")));
            this.button_negative.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_negative.Check = false;
            this.button_negative.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_negative.ForeColor = System.Drawing.Color.White;
            this.button_negative.Location = new System.Drawing.Point(352, 2);
            this.button_negative.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.button_negative.Name = "button_negative";
            this.button_negative.Size = new System.Drawing.Size(36, 45);
            this.button_negative.TabIndex = 3;
            this.button_negative.Text = "  ";
            this.button_negative.Click += new System.EventHandler(this.button_negative_Click);
            // 
            // button_positive
            // 
            this.button_positive.BackColor = System.Drawing.Color.Transparent;
            this.button_positive.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_positive.BackgroundImage")));
            this.button_positive.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_positive.Check = false;
            this.button_positive.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_positive.ForeColor = System.Drawing.Color.White;
            this.button_positive.Location = new System.Drawing.Point(312, 2);
            this.button_positive.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.button_positive.Name = "button_positive";
            this.button_positive.Size = new System.Drawing.Size(36, 45);
            this.button_positive.TabIndex = 2;
            this.button_positive.Text = "  ";
            this.button_positive.Click += new System.EventHandler(this.button_positive_Click);
            // 
            // button_mouse
            // 
            this.button_mouse.BackColor = System.Drawing.Color.Transparent;
            this.button_mouse.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_mouse.BackgroundImage")));
            this.button_mouse.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_mouse.Check = false;
            this.button_mouse.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_mouse.ForeColor = System.Drawing.Color.White;
            this.button_mouse.Location = new System.Drawing.Point(272, 2);
            this.button_mouse.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.button_mouse.Name = "button_mouse";
            this.button_mouse.Size = new System.Drawing.Size(36, 45);
            this.button_mouse.TabIndex = 1;
            this.button_mouse.Text = "  ";
            this.button_mouse.Click += new System.EventHandler(this.button_mouse_Click);
            // 
            // button_human
            // 
            this.button_human.BackColor = System.Drawing.Color.Transparent;
            this.button_human.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_human.BackgroundImage")));
            this.button_human.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_human.Check = false;
            this.button_human.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_human.ForeColor = System.Drawing.Color.White;
            this.button_human.Location = new System.Drawing.Point(232, 2);
            this.button_human.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.button_human.Name = "button_human";
            this.button_human.Size = new System.Drawing.Size(36, 45);
            this.button_human.TabIndex = 0;
            this.button_human.Text = "  ";
            this.button_human.Click += new System.EventHandler(this.button_human_Click);
            // 
            // button_mode
            // 
            this.button_mode.BackColor = System.Drawing.Color.Transparent;
            this.button_mode.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_mode.BackgroundImage")));
            this.button_mode.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_mode.Check = false;
            this.button_mode.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_mode.ForeColor = System.Drawing.Color.White;
            this.button_mode.Location = new System.Drawing.Point(188, 2);
            this.button_mode.Name = "button_mode";
            this.button_mode.Size = new System.Drawing.Size(36, 45);
            this.button_mode.TabIndex = 6;
            this.button_mode.Text = "Mode";
            this.button_mode.Click += new System.EventHandler(this.button_mode_Click);
            // 
            // Protocol_SearchBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.Controls.Add(this.button_mode);
            this.Controls.Add(this.textBox_search);
            this.Controls.Add(this.button_negative);
            this.Controls.Add(this.button_positive);
            this.Controls.Add(this.button_mouse);
            this.Controls.Add(this.button_human);
            this.Controls.Add(this.lblSearch);
            this.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "Protocol_SearchBar";
            this.Size = new System.Drawing.Size(396, 39);
            this.Load += new System.EventHandler(this.Protocol_SearchBar_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public Button_Filter button_human;
        public Button_Filter button_mouse;
        public Button_Filter button_negative;
        public Button_Filter button_positive;
        public System.Windows.Forms.TextBox textBox_search;
        public System.Windows.Forms.Label lblSearch;
        public Button_Circle button_mode;
    }
}
