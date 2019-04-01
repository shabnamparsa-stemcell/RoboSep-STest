namespace GUI_Console
{
    partial class RoboSep_Logs
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoboSep_Logs));
            this.tabs1 = new GUI_Controls.Tabs(RoboSep_UserConsole.intCurrentLogTab);
            this.SuspendLayout();
            // 
            // tabs1
            // 
            this.tabs1.BackgroundImage = GUI_Controls.Properties.Resources.Tab_Top1;
            this.tabs1.Location = new System.Drawing.Point(573, 0);
            this.tabs1.Name = "tabs1";
            this.tabs1.Size = new System.Drawing.Size(67, 480);
            this.tabs1.TabIndex = 0;
            this.tabs1.BackgroundImageChanged += new System.EventHandler(this.tabs1_BackgroundImageChanged);
            // 
            // RoboSep_Logs
            // 
            this.Controls.Add(this.tabs1);
            this.Name = "RoboSep_Logs";
            this.Size = new System.Drawing.Size(640, 480);
            this.Load += new System.EventHandler(this.RoboSep_Logs_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private GUI_Controls.Tabs tabs1;

    }
}
