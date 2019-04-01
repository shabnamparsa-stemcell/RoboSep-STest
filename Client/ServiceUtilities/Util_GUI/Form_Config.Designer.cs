namespace Util_GUI
{
    partial class Form_Config
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Config));
            this.panel2 = new System.Windows.Forms.Panel();
            this.button_AllGroups = new GUI_Controls.GUIButton();
            this.lblAllSections = new System.Windows.Forms.Label();
            this.UserNameHeader = new GUI_Controls.nameHeader();
            this.scrollBar1 = new GUI_Controls.ScrollBar();
            this.configScrollPanel = new GUI_Controls.ScrollPanel();
            this.button_Cancel = new GUI_Controls.Button_Cancel();
            this.button_Save = new GUI_Controls.Button_Circle();
            this.loadingTimer = new System.Windows.Forms.Timer(this.components);
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button_AllGroups);
            this.panel2.Controls.Add(this.lblAllSections);
            this.panel2.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel2.ForeColor = System.Drawing.Color.Black;
            this.panel2.Location = new System.Drawing.Point(3, 59);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(562, 40);
            this.panel2.TabIndex = 95;
            // 
            // button_AllGroups
            // 
            this.button_AllGroups.BackColor = System.Drawing.Color.Transparent;
            this.button_AllGroups.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_AllGroups.BackgroundImage")));
            this.button_AllGroups.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_AllGroups.Check = false;
            this.button_AllGroups.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_AllGroups.ForeColor = System.Drawing.Color.White;
            this.button_AllGroups.Location = new System.Drawing.Point(7, 0);
            this.button_AllGroups.Name = "button_AllGroups";
            this.button_AllGroups.Size = new System.Drawing.Size(40, 40);
            this.button_AllGroups.TabIndex = 34;
            this.button_AllGroups.Text = "  ";
            this.button_AllGroups.Click += new System.EventHandler(this.button_AllGroups_Click);
            // 
            // lblAllSections
            // 
            this.lblAllSections.BackColor = System.Drawing.Color.Transparent;
            this.lblAllSections.Font = new System.Drawing.Font("Arial Narrow", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAllSections.ForeColor = System.Drawing.Color.DarkGray;
            this.lblAllSections.Location = new System.Drawing.Point(51, 9);
            this.lblAllSections.Name = "lblAllSections";
            this.lblAllSections.Size = new System.Drawing.Size(143, 25);
            this.lblAllSections.TabIndex = 32;
            this.lblAllSections.Text = "All Sections";
            this.lblAllSections.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // UserNameHeader
            // 
            this.UserNameHeader.BackColor = System.Drawing.Color.Transparent;
            this.UserNameHeader.BackgroundImage = global::GUI_Console.Properties.Resources.User_HeaderSml2;
            this.UserNameHeader.Location = new System.Drawing.Point(284, 2);
            this.UserNameHeader.Name = "UserNameHeader";
            this.UserNameHeader.Size = new System.Drawing.Size(353, 37);
            this.UserNameHeader.TabIndex = 96;
            // 
            // scrollBar1
            // 
            this.scrollBar1.ActiveBackColor = System.Drawing.Color.Gray;
            this.scrollBar1.LargeChange = 10;
            this.scrollBar1.Location = new System.Drawing.Point(569, 100);
            this.scrollBar1.Maximum = 99;
            this.scrollBar1.Minimum = 0;
            this.scrollBar1.Name = "scrollBar1";
            this.scrollBar1.Size = new System.Drawing.Size(67, 271);
            this.scrollBar1.SmallChange = 1;
            this.scrollBar1.TabIndex = 94;
            this.scrollBar1.Text = "scrollBar1";
            this.scrollBar1.ThumbStyle = GUI_Controls.ScrollBar.EnumThumbStyle.Auto;
            this.scrollBar1.Value = 0;
            // 
            // configScrollPanel
            // 
            this.configScrollPanel.AutoScroll = true;
            this.configScrollPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.configScrollPanel.Location = new System.Drawing.Point(3, 100);
            this.configScrollPanel.Name = "configScrollPanel";
            this.configScrollPanel.Padding = new System.Windows.Forms.Padding(0, 0, 17, 0);
            this.configScrollPanel.SingleItemOnlyExpansion = false;
            this.configScrollPanel.Size = new System.Drawing.Size(562, 270);
            this.configScrollPanel.TabIndex = 93;
            this.configScrollPanel.VScrollbar = null;
            this.configScrollPanel.WrapContents = false;
            // 
            // button_Cancel
            // 
            this.button_Cancel.BackColor = System.Drawing.Color.Transparent;
            this.button_Cancel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Cancel.BackgroundImage")));
            this.button_Cancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Cancel.Check = false;
            this.button_Cancel.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Cancel.ForeColor = System.Drawing.Color.White;
            this.button_Cancel.Location = new System.Drawing.Point(416, 393);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(104, 86);
            this.button_Cancel.TabIndex = 92;
            this.button_Cancel.Text = "Cancel";
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // button_Save
            // 
            this.button_Save.BackColor = System.Drawing.Color.Transparent;
            this.button_Save.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Save.BackgroundImage")));
            this.button_Save.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Save.Check = false;
            this.button_Save.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Save.ForeColor = System.Drawing.Color.White;
            this.button_Save.Location = new System.Drawing.Point(527, 393);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(104, 86);
            this.button_Save.TabIndex = 91;
            this.button_Save.Text = "Save";
            this.button_Save.Click += new System.EventHandler(this.button_Save_Click);
            // 
            // loadingTimer
            // 
            this.loadingTimer.Tick += new System.EventHandler(this.loadingTimer_Tick);
            // 
            // Form_Config
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this.ClientSize = new System.Drawing.Size(640, 480);
            this.Controls.Add(this.UserNameHeader);
            this.Controls.Add(this.scrollBar1);
            this.Controls.Add(this.configScrollPanel);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.button_Save);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form_Config";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form_Config_Load);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private GUI_Controls.nameHeader UserNameHeader;
        private GUI_Controls.ScrollBar scrollBar1;
        private GUI_Controls.ScrollPanel configScrollPanel;
        private GUI_Controls.Button_Cancel button_Cancel;
        private System.Windows.Forms.Panel panel2;
        private GUI_Controls.GUIButton button_AllGroups;
        private System.Windows.Forms.Label lblAllSections;
        private GUI_Controls.Button_Circle button_Save;
        private System.Windows.Forms.Timer loadingTimer;

    }
}

