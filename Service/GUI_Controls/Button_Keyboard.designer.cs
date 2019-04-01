namespace GUI_Controls
{
    partial class Button_Keyboard
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
            this.EdgeL = new System.Windows.Forms.PictureBox();
            this.EdgeR = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.EdgeL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.EdgeR)).BeginInit();
            this.SuspendLayout();
            // 
            // EdgeL
            // 
            this.EdgeL.Image = global::GUI_Controls.Properties.Resources.KeybaordButton_L0;
            this.EdgeL.Location = new System.Drawing.Point(0, 0);
            this.EdgeL.Name = "EdgeL";
            this.EdgeL.Size = new System.Drawing.Size(6, 50);
            this.EdgeL.TabIndex = 0;
            this.EdgeL.TabStop = false;
            this.EdgeL.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Button_Keyboard_MouseDown);
            // 
            // EdgeR
            // 
            this.EdgeR.Image = global::GUI_Controls.Properties.Resources.KeybaordButton_R0;
            this.EdgeR.Location = new System.Drawing.Point(43, 0);
            this.EdgeR.Name = "EdgeR";
            this.EdgeR.Size = new System.Drawing.Size(7, 50);
            this.EdgeR.TabIndex = 1;
            this.EdgeR.TabStop = false;
            this.EdgeR.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Button_Keyboard_MouseDown);
            // 
            // Button_Keyboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::GUI_Controls.Properties.Resources.KeybaordButton_M0;
            this.Controls.Add(this.EdgeR);
            this.Controls.Add(this.EdgeL);
            this.Name = "Button_Keyboard";
            this.Size = new System.Drawing.Size(50, 50);
            this.Load += new System.EventHandler(this.Button_Keyboard_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Button_Keyboard_Paint);
            this.Leave += new System.EventHandler(this.Button_Keyboard_Leave);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Button_Keyboard_MouseDown);
            this.Resize += new System.EventHandler(this.Button_Keyboard_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.EdgeL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.EdgeR)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox EdgeL;
        private System.Windows.Forms.PictureBox EdgeR;
    }
}
