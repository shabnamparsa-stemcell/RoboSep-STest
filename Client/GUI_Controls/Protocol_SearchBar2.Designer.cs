namespace GUI_Controls
{
    partial class Protocol_SearchBar2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Protocol_SearchBar2));
            this.textBox_search = new System.Windows.Forms.TextBox();
            this.btn_ScrollUp = new GUI_Controls.Button_Scroll();
            this.btn_ScrollDown = new GUI_Controls.Button_Scroll();
            this.btn_ScrollLeft = new GUI_Controls.Button_Scroll();
            this.btn_ScrollRight = new GUI_Controls.Button_Scroll();
            this.SuspendLayout();
            // 
            // textBox_search
            // 
            this.textBox_search.BackColor = System.Drawing.Color.White;
            this.textBox_search.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_search.Font = new System.Drawing.Font("Arial Narrow", 14F);
            this.textBox_search.Location = new System.Drawing.Point(14, 21);
            this.textBox_search.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_search.Name = "textBox_search";
            this.textBox_search.Size = new System.Drawing.Size(198, 29);
            this.textBox_search.TabIndex = 11;
            // 
            // btn_ScrollUp
            // 
            this.btn_ScrollUp.BackColor = System.Drawing.Color.Transparent;
            this.btn_ScrollUp.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_ScrollUp.BackgroundImage")));
            this.btn_ScrollUp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_ScrollUp.Check = false;
            this.btn_ScrollUp.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_ScrollUp.ForeColor = System.Drawing.Color.White;
            this.btn_ScrollUp.Location = new System.Drawing.Point(582, 8);
            this.btn_ScrollUp.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btn_ScrollUp.Name = "btn_ScrollUp";
            this.btn_ScrollUp.Size = new System.Drawing.Size(52, 52);
            this.btn_ScrollUp.TabIndex = 21;
            this.btn_ScrollUp.Text = "  ";
            // 
            // btn_ScrollDown
            // 
            this.btn_ScrollDown.BackColor = System.Drawing.Color.Transparent;
            this.btn_ScrollDown.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_ScrollDown.BackgroundImage")));
            this.btn_ScrollDown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_ScrollDown.Check = false;
            this.btn_ScrollDown.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_ScrollDown.ForeColor = System.Drawing.Color.White;
            this.btn_ScrollDown.Location = new System.Drawing.Point(519, 8);
            this.btn_ScrollDown.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btn_ScrollDown.Name = "btn_ScrollDown";
            this.btn_ScrollDown.Size = new System.Drawing.Size(52, 52);
            this.btn_ScrollDown.TabIndex = 20;
            this.btn_ScrollDown.Text = "  ";
            // 
            // btn_ScrollLeft
            // 
            this.btn_ScrollLeft.BackColor = System.Drawing.Color.Transparent;
            this.btn_ScrollLeft.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_ScrollLeft.BackgroundImage")));
            this.btn_ScrollLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_ScrollLeft.Check = false;
            this.btn_ScrollLeft.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_ScrollLeft.ForeColor = System.Drawing.Color.White;
            this.btn_ScrollLeft.Location = new System.Drawing.Point(393, 8);
            this.btn_ScrollLeft.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btn_ScrollLeft.Name = "btn_ScrollLeft";
            this.btn_ScrollLeft.Size = new System.Drawing.Size(52, 52);
            this.btn_ScrollLeft.TabIndex = 19;
            this.btn_ScrollLeft.Text = "  ";
            // 
            // btn_ScrollRight
            // 
            this.btn_ScrollRight.BackColor = System.Drawing.Color.Transparent;
            this.btn_ScrollRight.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_ScrollRight.BackgroundImage")));
            this.btn_ScrollRight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_ScrollRight.Check = false;
            this.btn_ScrollRight.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_ScrollRight.ForeColor = System.Drawing.Color.White;
            this.btn_ScrollRight.Location = new System.Drawing.Point(456, 8);
            this.btn_ScrollRight.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btn_ScrollRight.Name = "btn_ScrollRight";
            this.btn_ScrollRight.Size = new System.Drawing.Size(52, 52);
            this.btn_ScrollRight.TabIndex = 18;
            this.btn_ScrollRight.Text = "  ";
            // 
            // Protocol_SearchBar2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.Controls.Add(this.btn_ScrollUp);
            this.Controls.Add(this.btn_ScrollDown);
            this.Controls.Add(this.btn_ScrollLeft);
            this.Controls.Add(this.btn_ScrollRight);
            this.Controls.Add(this.textBox_search);
            this.Name = "Protocol_SearchBar2";
            this.Size = new System.Drawing.Size(640, 69);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox textBox_search;
        public Button_Scroll btn_ScrollUp;
        public Button_Scroll btn_ScrollDown;
        public Button_Scroll btn_ScrollLeft;
        public Button_Scroll btn_ScrollRight;

    }
}
