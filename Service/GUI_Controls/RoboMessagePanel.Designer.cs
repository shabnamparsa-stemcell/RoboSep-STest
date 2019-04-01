namespace GUI_Controls
{
    partial class RoboMessagePanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoboMessagePanel));
            this.label_WindowTitle = new System.Windows.Forms.Label();
            this.ProgressPanel = new System.Windows.Forms.Panel();
            this.MSGprogress = new GUI_Controls.ProgressBar();
            this.txtMessage = new GUI_Controls.AlphaBlendTextBox();
            this.button2 = new GUI_Controls.Button_Rectangle();
            this.button1 = new GUI_Controls.Button_Rectangle();
            this.ProgressPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label_WindowTitle
            // 
            this.label_WindowTitle.AutoSize = true;
            this.label_WindowTitle.BackColor = System.Drawing.Color.Transparent;
            this.label_WindowTitle.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_WindowTitle.ForeColor = System.Drawing.Color.White;
            this.label_WindowTitle.Location = new System.Drawing.Point(25, -2);
            this.label_WindowTitle.Name = "label_WindowTitle";
            this.label_WindowTitle.Size = new System.Drawing.Size(149, 24);
            this.label_WindowTitle.TabIndex = 3;
            this.label_WindowTitle.Text = "Window Title";
            this.label_WindowTitle.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // ProgressPanel
            // 
            this.ProgressPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.ProgressPanel.Controls.Add(this.MSGprogress);
            this.ProgressPanel.Location = new System.Drawing.Point(39, 187);
            this.ProgressPanel.Name = "ProgressPanel";
            this.ProgressPanel.Size = new System.Drawing.Size(328, 48);
            this.ProgressPanel.TabIndex = 25;
            this.ProgressPanel.Visible = false;
            // 
            // MSGprogress
            // 
            this.MSGprogress.BackColor = System.Drawing.Color.DarkGray;
            this.MSGprogress.Location = new System.Drawing.Point(2, 2);
            this.MSGprogress.Name = "MSGprogress";
            this.MSGprogress.Size = new System.Drawing.Size(323, 44);
            this.MSGprogress.TabIndex = 8;
            // 
            // txtMessage
            // 
            this.txtMessage.BackAlpha = 0;
            this.txtMessage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.txtMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtMessage.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMessage.Location = new System.Drawing.Point(25, 35);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(350, 146);
            this.txtMessage.TabIndex = 11;
            this.txtMessage.Enter += new System.EventHandler(this.txtMessage_Enter);
            // 
            // button2
            // 
            this.button2.AccessibleName = "  ";
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button2.BackgroundImage")));
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button2.Check = false;
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.SystemColors.Window;
            this.button2.Location = new System.Drawing.Point(186, 246);
            this.button2.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(86, 32);
            this.button2.TabIndex = 10;
            this.button2.Click += new System.EventHandler(this.button_Click);
            // 
            // button1
            // 
            this.button1.AccessibleName = "  ";
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button1.BackgroundImage")));
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button1.Check = false;
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.SystemColors.Window;
            this.button1.Location = new System.Drawing.Point(296, 246);
            this.button1.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(86, 32);
            this.button1.TabIndex = 9;
            this.button1.Click += new System.EventHandler(this.button_Click);
            // 
            // RoboMessagePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.ClientSize = new System.Drawing.Size(400, 300);
            this.ControlBox = false;
            this.Controls.Add(this.ProgressPanel);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.label_WindowTitle);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "RoboMessagePanel";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.RoboMessagePanel_Load);
            this.RegionChanged += new System.EventHandler(this.RoboMessagePanel_RegionChanged);
            this.SizeChanged += new System.EventHandler(this.RoboMessagePanel_SizeChanged);
            this.ProgressPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_WindowTitle;
        private GUI_Controls.Button_Rectangle button1;
        private GUI_Controls.Button_Rectangle button2;
        private AlphaBlendTextBox txtMessage;
        private System.Windows.Forms.Panel ProgressPanel;
        private ProgressBar MSGprogress;
    }
}
