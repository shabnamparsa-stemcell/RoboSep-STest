namespace GUI_Console
{
    partial class RoboSep_Help
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoboSep_Help));
            this.HelpTabs = new GUI_Controls.Tabs(RoboSep_UserConsole.intCurrentHelpTab);
            this.SuspendLayout();
            // 
            // HelpTabs
            // 
            this.HelpTabs.BackgroundImage = GUI_Controls.Properties.Resources.TAB_ROBOSEP1;
            this.HelpTabs.Location = new System.Drawing.Point(573, 0);
            this.HelpTabs.Name = "HelpTabs";
            this.HelpTabs.Size = new System.Drawing.Size(67, 480);
            this.HelpTabs.TabIndex = 0;
            this.HelpTabs.BackgroundImageChanged += new System.EventHandler(this.HelpTabs_BackgroundImageChanged);
            // 
            // RoboSep_Help
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.HelpTabs);
            this.Name = "RoboSep_Help";
            this.Size = new System.Drawing.Size(640, 480);
            this.Load += new System.EventHandler(this.RoboSep_Help_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private GUI_Controls.Tabs HelpTabs;
    }
}
