namespace GUI_Controls
{
    partial class nameHeader
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
            this.label_Username = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label_Username
            // 
            this.label_Username.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.label_Username.ForeColor = System.Drawing.Color.White;
            this.label_Username.Location = new System.Drawing.Point(30, 6);
            this.label_Username.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label_Username.MaximumSize = new System.Drawing.Size(174, 26);
            this.label_Username.Name = "label_Username";
            this.label_Username.Size = new System.Drawing.Size(173, 26);
            this.label_Username.TabIndex = 0;
            this.label_Username.Text = "User Name";
            this.label_Username.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label_Username.Click += new System.EventHandler(this.label_Username_Click);
            this.label_Username.Paint += new System.Windows.Forms.PaintEventHandler(this.label_Username_Paint);
            // 
            // nameHeader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BackgroundImage = global::GUI_Controls.Properties.Resources.User_HeaderSml;
            this.Controls.Add(this.label_Username);
            this.Name = "nameHeader";
            this.Size = new System.Drawing.Size(353, 37);
            this.Click += new System.EventHandler(this.nameHeader_Click);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label_Username;


    }
}
