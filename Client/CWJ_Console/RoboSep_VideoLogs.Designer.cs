namespace GUI_Console
{
    partial class RoboSep_VideoLogs
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoboSep_VideoLogs));
            this.logTabs = new GUI_Controls.HorizontalTabs();
            this.videoLogPanel = new System.Windows.Forms.TableLayoutPanel();
            this.videoErrorLogPanel = new System.Windows.Forms.TableLayoutPanel();
            this.copyButton = new GUI_Controls.Button_Rectangle();
            this.deleteButton = new GUI_Controls.Button_Rectangle();
            this.selectAllCheckBox = new GUI_Controls.CheckBox_Square();
            this.SuspendLayout();
            // 
            // logTabs
            // 
            this.logTabs.BackColor = System.Drawing.Color.Transparent;
            this.logTabs.Location = new System.Drawing.Point(0, 0);
            this.logTabs.Name = "logTabs";
            this.logTabs.Size = new System.Drawing.Size(640, 37);
            this.logTabs.Tab1 = "Video Error Log";
            this.logTabs.Tab2 = "Video Log";
            this.logTabs.Tab3 = null;
            this.logTabs.TabIndex = 46;
            // 
            // videoLogPanel
            // 
            this.videoLogPanel.AutoScroll = true;
            this.videoLogPanel.ColumnCount = 1;
            this.videoLogPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.videoLogPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.videoLogPanel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.videoLogPanel.Location = new System.Drawing.Point(0, 34);
            this.videoLogPanel.Name = "videoLogPanel";
            this.videoLogPanel.Padding = new System.Windows.Forms.Padding(30, 25, 0, 0);
            this.videoLogPanel.RowCount = 1;
            this.videoLogPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.videoLogPanel.Size = new System.Drawing.Size(640, 381);
            this.videoLogPanel.TabIndex = 58;
            this.videoLogPanel.Visible = false;
            // 
            // videoErrorLogPanel
            // 
            this.videoErrorLogPanel.AutoScroll = true;
            this.videoErrorLogPanel.ColumnCount = 1;
            this.videoErrorLogPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.videoErrorLogPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.videoErrorLogPanel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.videoErrorLogPanel.Location = new System.Drawing.Point(0, 34);
            this.videoErrorLogPanel.Name = "videoErrorLogPanel";
            this.videoErrorLogPanel.Padding = new System.Windows.Forms.Padding(30, 25, 0, 0);
            this.videoErrorLogPanel.RowCount = 1;
            this.videoErrorLogPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.videoErrorLogPanel.Size = new System.Drawing.Size(640, 381);
            this.videoErrorLogPanel.TabIndex = 58;
            this.videoErrorLogPanel.Visible = false;
            // 
            // copyButton
            // 
            this.copyButton.BackColor = System.Drawing.Color.Transparent;
            this.copyButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("copyButton.BackgroundImage")));
            this.copyButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.copyButton.Check = false;
            this.copyButton.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.copyButton.ForeColor = System.Drawing.Color.White;
            this.copyButton.Location = new System.Drawing.Point(300, 421);
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(156, 46);
            this.copyButton.TabIndex = 12;
            this.copyButton.Text = "Copy";
            this.copyButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.copyButtonClickHandler);
            // 
            // deleteButton
            // 
            this.deleteButton.BackColor = System.Drawing.Color.Transparent;
            this.deleteButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("deleteButton.BackgroundImage")));
            this.deleteButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.deleteButton.Check = false;
            this.deleteButton.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deleteButton.ForeColor = System.Drawing.Color.White;
            this.deleteButton.Location = new System.Drawing.Point(472, 421);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(154, 46);
            this.deleteButton.TabIndex = 59;
            this.deleteButton.Text = "Delete";
            this.deleteButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.deleteButtonClickHandler);
            // 
            // checkBox_Square1
            // 
            this.selectAllCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.selectAllCheckBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("checkBox_Square1.BackgroundImage")));
            this.selectAllCheckBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.selectAllCheckBox.Check = false;
            this.selectAllCheckBox.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectAllCheckBox.ForeColor = System.Drawing.Color.White;
            this.selectAllCheckBox.Location = new System.Drawing.Point(225, 428);
            this.selectAllCheckBox.Name = "checkBox_Square1";
            this.selectAllCheckBox.Size = new System.Drawing.Size(32, 32);
            this.selectAllCheckBox.TabIndex = 61;
            this.selectAllCheckBox.Text = "checkBox_Square1";
            this.selectAllCheckBox.Click += new System.EventHandler(this.selectAllClickHandler);
            // 
            // RoboSep_VideoLogs
            // 
            this.Controls.Add(this.selectAllCheckBox);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.copyButton);
            this.Controls.Add(this.videoErrorLogPanel);
            this.Controls.Add(this.videoLogPanel);
            this.Controls.Add(this.logTabs);
            this.Name = "RoboSep_VideoLogs";
            this.Controls.SetChildIndex(this.logTabs, 0);
            this.Controls.SetChildIndex(this.videoLogPanel, 0);
            this.Controls.SetChildIndex(this.videoErrorLogPanel, 0);
            this.Controls.SetChildIndex(this.copyButton, 0);
            this.Controls.SetChildIndex(this.deleteButton, 0);
            this.Controls.SetChildIndex(this.btn_home, 0);
            this.Controls.SetChildIndex(this.selectAllCheckBox, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private GUI_Controls.HorizontalTabs logTabs;
        private System.Windows.Forms.TableLayoutPanel videoLogPanel;
        private System.Windows.Forms.TableLayoutPanel videoErrorLogPanel;
        private GUI_Controls.Button_Rectangle copyButton;
        private GUI_Controls.Button_Rectangle deleteButton;
        private GUI_Controls.CheckBox_Square selectAllCheckBox;
    }
}
