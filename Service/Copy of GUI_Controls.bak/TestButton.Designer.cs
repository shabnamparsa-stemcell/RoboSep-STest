namespace GUI_Controls
{
    partial class TestButton
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
            this.SuspendLayout();
            // 
            // TestButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::GUI_Controls.Properties.Resources.Button_RUN0;
            this.Name = "TestButton";
            this.Size = new System.Drawing.Size(159, 106);
            this.Click += new System.EventHandler(this.TestButton_Click);
            this.MouseEnter += new System.EventHandler(this.TestButton_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.TestButton_MouseLeave);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
