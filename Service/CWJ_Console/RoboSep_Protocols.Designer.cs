namespace GUI_Console
{
    partial class RoboSep_Protocols
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoboSep_Protocols));
            this.button_EditList = new GUI_Controls.Button_Rectangle();
            this.UserPanel = new System.Windows.Forms.Panel();
            this.textBox_UserName = new System.Windows.Forms.TextBox();
            this.dropDownMenu = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.Button_ProtocolEdit = new GUI_Controls.Button_SmallPink();
            this.button_FullList = new GUI_Controls.Button_Rectangle();
            this.button_WholeBlood = new GUI_Controls.Button_Rectangle();
            this.button_AllMouse = new GUI_Controls.Button_Rectangle();
            this.button_AllHuman = new GUI_Controls.Button_Rectangle();
            this.RoboPanel_ProtocolEditor = new GUI_Controls.RoboPanel();
            this.roboPanel2 = new GUI_Controls.RoboPanel();
            this.roboPanel1 = new GUI_Controls.RoboPanel();
            this.LoadUserTimer = new System.Windows.Forms.Timer(this.components);
            this.UserPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dropDownMenu)).BeginInit();
            this.RoboPanel_ProtocolEditor.SuspendLayout();
            this.roboPanel2.SuspendLayout();
            this.roboPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_EditList
            // 
            this.button_EditList.AccessibleName = "Edit List";
            this.button_EditList.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_EditList.BackgroundImage")));
            this.button_EditList.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_EditList.Check = false;
            this.button_EditList.Location = new System.Drawing.Point(23, 100);
            this.button_EditList.Name = "button_EditList";
            this.button_EditList.Size = new System.Drawing.Size(243, 74);
            this.button_EditList.TabIndex = 16;
            this.button_EditList.Click += new System.EventHandler(this.button_EditList_Click);
            // 
            // UserPanel
            // 
            this.UserPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.UserPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("UserPanel.BackgroundImage")));
            this.UserPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.UserPanel.Controls.Add(this.textBox_UserName);
            this.UserPanel.Controls.Add(this.dropDownMenu);
            this.UserPanel.Location = new System.Drawing.Point(80, 43);
            this.UserPanel.Name = "UserPanel";
            this.UserPanel.Size = new System.Drawing.Size(184, 51);
            this.UserPanel.TabIndex = 12;
            // 
            // textBox_UserName
            // 
            this.textBox_UserName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.textBox_UserName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.textBox_UserName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_UserName.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.5F);
            this.textBox_UserName.Location = new System.Drawing.Point(8, 13);
            this.textBox_UserName.Name = "textBox_UserName";
            this.textBox_UserName.Size = new System.Drawing.Size(139, 22);
            this.textBox_UserName.TabIndex = 19;
            this.textBox_UserName.Text = "User Name";
            this.textBox_UserName.TextChanged += new System.EventHandler(this.textBox_UserName_TextChanged);
            this.textBox_UserName.Enter += new System.EventHandler(this.textBox_UserName_Enter);
            // 
            // dropDownMenu
            // 
            this.dropDownMenu.BackColor = System.Drawing.Color.Transparent;
            this.dropDownMenu.Location = new System.Drawing.Point(144, 0);
            this.dropDownMenu.Name = "dropDownMenu";
            this.dropDownMenu.Size = new System.Drawing.Size(40, 51);
            this.dropDownMenu.TabIndex = 18;
            this.dropDownMenu.TabStop = false;
            this.dropDownMenu.Click += new System.EventHandler(this.dropDownMenu_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.label1.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 16F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.label1.Location = new System.Drawing.Point(24, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 26);
            this.label1.TabIndex = 17;
            this.label1.Text = "user:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.label7.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 16F, System.Drawing.FontStyle.Bold);
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.label7.Location = new System.Drawing.Point(33, 52);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(114, 26);
            this.label7.TabIndex = 16;
            this.label7.Text = "Protocol Editor";
            // 
            // Button_ProtocolEdit
            // 
            this.Button_ProtocolEdit.AccessibleName = "  ";
            this.Button_ProtocolEdit.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Button_ProtocolEdit.BackgroundImage")));
            this.Button_ProtocolEdit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Button_ProtocolEdit.Check = false;
            this.Button_ProtocolEdit.Location = new System.Drawing.Point(169, 17);
            this.Button_ProtocolEdit.Name = "Button_ProtocolEdit";
            this.Button_ProtocolEdit.Size = new System.Drawing.Size(97, 97);
            this.Button_ProtocolEdit.TabIndex = 12;
            // 
            // button_FullList
            // 
            this.button_FullList.AccessibleName = "Full List";
            this.button_FullList.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_FullList.BackgroundImage")));
            this.button_FullList.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_FullList.Check = false;
            this.button_FullList.Location = new System.Drawing.Point(15, 301);
            this.button_FullList.Name = "button_FullList";
            this.button_FullList.Size = new System.Drawing.Size(243, 74);
            this.button_FullList.TabIndex = 15;
            this.button_FullList.Click += new System.EventHandler(this.button_FullList_Click);
            // 
            // button_WholeBlood
            // 
            this.button_WholeBlood.AccessibleName = "Whole Blood";
            this.button_WholeBlood.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_WholeBlood.BackgroundImage")));
            this.button_WholeBlood.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_WholeBlood.Check = false;
            this.button_WholeBlood.Location = new System.Drawing.Point(15, 217);
            this.button_WholeBlood.Name = "button_WholeBlood";
            this.button_WholeBlood.Size = new System.Drawing.Size(243, 74);
            this.button_WholeBlood.TabIndex = 14;
            this.button_WholeBlood.Click += new System.EventHandler(this.button_WholeBlood_Click);
            // 
            // button_AllMouse
            // 
            this.button_AllMouse.AccessibleName = "All Mosue";
            this.button_AllMouse.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_AllMouse.BackgroundImage")));
            this.button_AllMouse.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_AllMouse.Check = false;
            this.button_AllMouse.Location = new System.Drawing.Point(15, 133);
            this.button_AllMouse.Name = "button_AllMouse";
            this.button_AllMouse.Size = new System.Drawing.Size(243, 74);
            this.button_AllMouse.TabIndex = 13;
            this.button_AllMouse.Click += new System.EventHandler(this.button_AllMouse_Click);
            // 
            // button_AllHuman
            // 
            this.button_AllHuman.AccessibleName = "All Human";
            this.button_AllHuman.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_AllHuman.BackgroundImage")));
            this.button_AllHuman.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_AllHuman.Check = false;
            this.button_AllHuman.Location = new System.Drawing.Point(15, 49);
            this.button_AllHuman.Name = "button_AllHuman";
            this.button_AllHuman.Size = new System.Drawing.Size(243, 74);
            this.button_AllHuman.TabIndex = 12;
            this.button_AllHuman.Click += new System.EventHandler(this.button_AllHuman_Click);
            // 
            // RoboPanel_ProtocolEditor
            // 
            this.RoboPanel_ProtocolEditor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.RoboPanel_ProtocolEditor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.RoboPanel_ProtocolEditor.Controls.Add(this.label7);
            this.RoboPanel_ProtocolEditor.Controls.Add(this.Button_ProtocolEdit);
            this.RoboPanel_ProtocolEditor.Location = new System.Drawing.Point(30, 232);
            this.RoboPanel_ProtocolEditor.Name = "RoboPanel_ProtocolEditor";
            this.RoboPanel_ProtocolEditor.Size = new System.Drawing.Size(285, 131);
            this.RoboPanel_ProtocolEditor.TabIndex = 17;
            // 
            // roboPanel2
            // 
            this.roboPanel2.AccessibleName = "User List";
            this.roboPanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.roboPanel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.roboPanel2.Controls.Add(this.button_EditList);
            this.roboPanel2.Controls.Add(this.UserPanel);
            this.roboPanel2.Controls.Add(this.label1);
            this.roboPanel2.Location = new System.Drawing.Point(30, 26);
            this.roboPanel2.Name = "roboPanel2";
            this.roboPanel2.Size = new System.Drawing.Size(285, 190);
            this.roboPanel2.TabIndex = 17;
            // 
            // roboPanel1
            // 
            this.roboPanel1.AccessibleName = "Preset List";
            this.roboPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.roboPanel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.roboPanel1.Controls.Add(this.button_AllHuman);
            this.roboPanel1.Controls.Add(this.button_AllMouse);
            this.roboPanel1.Controls.Add(this.button_WholeBlood);
            this.roboPanel1.Controls.Add(this.button_FullList);
            this.roboPanel1.Location = new System.Drawing.Point(336, 26);
            this.roboPanel1.Name = "roboPanel1";
            this.roboPanel1.Size = new System.Drawing.Size(272, 400);
            this.roboPanel1.TabIndex = 18;
            // 
            // LoadUserTimer
            // 
            this.LoadUserTimer.Interval = 10;
            this.LoadUserTimer.Tick += new System.EventHandler(this.LoadUserTimer_Tick);
            // 
            // RoboSep_Protocols
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.RoboPanel_ProtocolEditor);
            this.Controls.Add(this.roboPanel2);
            this.Controls.Add(this.roboPanel1);
            this.Name = "RoboSep_Protocols";
            this.Load += new System.EventHandler(this.RoboSep_Protocols_Load);
            this.Controls.SetChildIndex(this.roboPanel1, 0);
            this.Controls.SetChildIndex(this.roboPanel2, 0);
            this.Controls.SetChildIndex(this.RoboPanel_ProtocolEditor, 0);
            this.UserPanel.ResumeLayout(false);
            this.UserPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dropDownMenu)).EndInit();
            this.RoboPanel_ProtocolEditor.ResumeLayout(false);
            this.RoboPanel_ProtocolEditor.PerformLayout();
            this.roboPanel2.ResumeLayout(false);
            this.roboPanel2.PerformLayout();
            this.roboPanel1.ResumeLayout(false);
            this.roboPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private GUI_Controls.Button_SmallPink Button_ProtocolEdit;
        private GUI_Controls.Button_Rectangle button_FullList;
        private GUI_Controls.Button_Rectangle button_WholeBlood;
        private GUI_Controls.Button_Rectangle button_AllMouse;
        private GUI_Controls.Button_Rectangle button_AllHuman;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel UserPanel;
        private System.Windows.Forms.PictureBox dropDownMenu;
        private GUI_Controls.Button_Rectangle button_EditList;
        private GUI_Controls.RoboPanel RoboPanel_ProtocolEditor;
        private GUI_Controls.RoboPanel roboPanel2;
        private GUI_Controls.RoboPanel roboPanel1;
        private System.Windows.Forms.TextBox textBox_UserName;
        private System.Windows.Forms.Timer LoadUserTimer;
    }
}
