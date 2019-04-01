namespace GUI_Controls
{
    partial class GUIButton
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
            this.components = new System.ComponentModel.Container();
            this.tempTimer = new System.Windows.Forms.Timer(this.components);
            this.click_timer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // tempTimer
            // 
            this.tempTimer.Interval = 50;
            this.tempTimer.Tick += new System.EventHandler(this.tempTimer_Tick);
            // 
            // click_timer
            // 
            this.click_timer.Interval = 600;
            this.click_timer.Tick += new System.EventHandler(this.click_timer_Tick);
            // 
            // GUIButton
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BackgroundImage = global::GUI_Controls.Properties.Resources.BUTTON_HOMECANCEL0;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "GUIButton";
            this.Size = new System.Drawing.Size(52, 50);
            this.Load += new System.EventHandler(this.GUIButton_Load);
            this.Click += new System.EventHandler(this.GUIButton_Click);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.GUIButton_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GUIButton_MouseDown);
            this.MouseEnter += new System.EventHandler(this.GUIButton_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.GUIButton_MouseLeave);

            this.ResumeLayout(false);

        }

        #endregion

        protected System.Windows.Forms.Timer tempTimer;
        protected System.Windows.Forms.Timer click_timer;

    }
}
