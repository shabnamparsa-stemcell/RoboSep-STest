namespace GUI_Console
{
    partial class BasePannel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BasePannel));
            this.btn_home = new GUI_Controls.Button_Home();
            this.SuspendLayout();
            // 
            // btn_home
            // 
            this.btn_home.AccessibleName = " ";
            this.btn_home.BackgroundImage = GUI_Controls.Properties.Resources.Button_Home0;
            this.btn_home.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_home.Check = false;
            this.btn_home.Location = new System.Drawing.Point(4, 383);
            this.btn_home.Name = "btn_home";
            this.btn_home.Size = new System.Drawing.Size(91, 91);
            this.btn_home.TabIndex = 0;
            this.btn_home.Click += new System.EventHandler(this.btn_home_Click);
            // 
            // BasePannel
            // 
            this.BackgroundImage = GUI_Controls.Properties.Resources.Frame_Background;
            this.Controls.Add(this.btn_home);
            this.Name = "BasePannel";
            this.Size = new System.Drawing.Size(640, 480);
            this.ResumeLayout(false);
            this.Resize += new System.EventHandler(this.BasePanel1_Resize);

        }

        #endregion

        private GUI_Controls.Button_Home btn_home;
    }
}
