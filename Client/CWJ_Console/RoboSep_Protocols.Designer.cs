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
            int i;
            for (i = 0; i < ilist1.Count; i++ )
            {
                ilist1[i].Dispose();
            }
            for (i = 0; i < ilist2.Count; i++)
            {
                ilist2[i].Dispose();
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
            this.textBox_UserName = new System.Windows.Forms.TextBox();
            this.dropDownMenu = new System.Windows.Forms.PictureBox();
            this.lblUser = new System.Windows.Forms.Label();
            this.lblProtocolEdit = new System.Windows.Forms.Label();
            this.Button_ProtocolEdit = new GUI_Controls.Button_SmallPink();
            this.button_FullList = new GUI_Controls.Button_Rectangle();
            this.button_WholeBlood = new GUI_Controls.Button_Rectangle();
            this.button_AllMouse = new GUI_Controls.Button_Rectangle();
            this.button_AllHuman = new GUI_Controls.Button_Rectangle();
            this.button_EditList = new GUI_Controls.Button_Rectangle();
            this.button_LoadUser = new GUI_Controls.Button_Rectangle();
            this.button_AddUser = new GUI_Controls.Button_Rectangle();
            this.button_RemoveUser = new GUI_Controls.Button_Rectangle();
            this.LoadUserTimer = new System.Windows.Forms.Timer(this.components);
            this.horizontalTabs1 = new GUI_Controls.HorizontalTabs();
            this.UserPanel = new System.Windows.Forms.Panel();
            this.lineShape1 = new Microsoft.VisualBasic.PowerPacks.LineShape();
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.UserIcon = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.UserNameHeader = new GUI_Controls.nameHeader();
            ((System.ComponentModel.ISupportInitialize)(this.dropDownMenu)).BeginInit();
            this.UserPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UserIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox_UserName
            // 
            this.textBox_UserName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.textBox_UserName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.textBox_UserName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_UserName.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_UserName.Location = new System.Drawing.Point(6, 7);
            this.textBox_UserName.Name = "textBox_UserName";
            this.textBox_UserName.Size = new System.Drawing.Size(233, 25);
            this.textBox_UserName.TabIndex = 19;
            this.textBox_UserName.Text = "User Name";
            this.textBox_UserName.Click += new System.EventHandler(this.textBox_UserName_Click);
            this.textBox_UserName.TextChanged += new System.EventHandler(this.textBox_UserName_TextChanged);
            this.textBox_UserName.Enter += new System.EventHandler(this.textBox_UserName_Enter);
            // 
            // dropDownMenu
            // 
            this.dropDownMenu.BackColor = System.Drawing.Color.Transparent;
            this.dropDownMenu.BackgroundImage = global::GUI_Console.Properties.Resources.btnDROPBOX_STD;
            this.dropDownMenu.Location = new System.Drawing.Point(469, 204);
            this.dropDownMenu.Name = "dropDownMenu";
            this.dropDownMenu.Size = new System.Drawing.Size(63, 39);
            this.dropDownMenu.TabIndex = 18;
            this.dropDownMenu.TabStop = false;
            this.dropDownMenu.Click += new System.EventHandler(this.dropDownMenu_Click);
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.BackColor = System.Drawing.Color.Transparent;
            this.lblUser.Font = new System.Drawing.Font("Arial Narrow", 16F, System.Drawing.FontStyle.Bold);
            this.lblUser.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.lblUser.Location = new System.Drawing.Point(212, 169);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(116, 26);
            this.lblUser.TabIndex = 17;
            this.lblUser.Text = "Select User:";
            // 
            // lblProtocolEdit
            // 
            this.lblProtocolEdit.AutoSize = true;
            this.lblProtocolEdit.BackColor = System.Drawing.Color.Transparent;
            this.lblProtocolEdit.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.lblProtocolEdit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.lblProtocolEdit.Location = new System.Drawing.Point(16, 3);
            this.lblProtocolEdit.Name = "lblProtocolEdit";
            this.lblProtocolEdit.Size = new System.Drawing.Size(170, 26);
            this.lblProtocolEdit.TabIndex = 16;
            this.lblProtocolEdit.Text = "Protocol Editor";
            this.lblProtocolEdit.Visible = false;
            // 
            // Button_ProtocolEdit
            // 
            this.Button_ProtocolEdit.BackColor = System.Drawing.Color.Transparent;
            this.Button_ProtocolEdit.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Button_ProtocolEdit.BackgroundImage")));
            this.Button_ProtocolEdit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Button_ProtocolEdit.Check = false;
            this.Button_ProtocolEdit.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Button_ProtocolEdit.ForeColor = System.Drawing.Color.White;
            this.Button_ProtocolEdit.Location = new System.Drawing.Point(123, 3);
            this.Button_ProtocolEdit.Name = "Button_ProtocolEdit";
            this.Button_ProtocolEdit.Size = new System.Drawing.Size(28, 28);
            this.Button_ProtocolEdit.TabIndex = 12;
            this.Button_ProtocolEdit.Text = "  ";
            this.Button_ProtocolEdit.Visible = false;
            this.Button_ProtocolEdit.Click += new System.EventHandler(this.Button_ProtocolEdit_Click);
            // 
            // button_FullList
            // 
            this.button_FullList.BackColor = System.Drawing.Color.Transparent;
            this.button_FullList.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_FullList.BackgroundImage")));
            this.button_FullList.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_FullList.Check = false;
            this.button_FullList.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_FullList.ForeColor = System.Drawing.Color.White;
            this.button_FullList.Location = new System.Drawing.Point(668, 318);
            this.button_FullList.Name = "button_FullList";
            this.button_FullList.Size = new System.Drawing.Size(243, 74);
            this.button_FullList.TabIndex = 15;
            this.button_FullList.Text = "  ";
            this.button_FullList.Click += new System.EventHandler(this.button_FullList_Click);
            // 
            // button_WholeBlood
            // 
            this.button_WholeBlood.BackColor = System.Drawing.Color.Transparent;
            this.button_WholeBlood.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_WholeBlood.BackgroundImage")));
            this.button_WholeBlood.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_WholeBlood.Check = false;
            this.button_WholeBlood.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_WholeBlood.ForeColor = System.Drawing.Color.White;
            this.button_WholeBlood.Location = new System.Drawing.Point(668, 234);
            this.button_WholeBlood.Name = "button_WholeBlood";
            this.button_WholeBlood.Size = new System.Drawing.Size(243, 74);
            this.button_WholeBlood.TabIndex = 14;
            this.button_WholeBlood.Text = "  ";
            this.button_WholeBlood.Click += new System.EventHandler(this.button_WholeBlood_Click);
            // 
            // button_AllMouse
            // 
            this.button_AllMouse.BackColor = System.Drawing.Color.Transparent;
            this.button_AllMouse.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_AllMouse.BackgroundImage")));
            this.button_AllMouse.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_AllMouse.Check = false;
            this.button_AllMouse.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_AllMouse.ForeColor = System.Drawing.Color.White;
            this.button_AllMouse.Location = new System.Drawing.Point(668, 150);
            this.button_AllMouse.Name = "button_AllMouse";
            this.button_AllMouse.Size = new System.Drawing.Size(243, 74);
            this.button_AllMouse.TabIndex = 13;
            this.button_AllMouse.Text = "  ";
            this.button_AllMouse.Click += new System.EventHandler(this.button_AllMouse_Click);
            // 
            // button_AllHuman
            // 
            this.button_AllHuman.BackColor = System.Drawing.Color.Transparent;
            this.button_AllHuman.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_AllHuman.BackgroundImage")));
            this.button_AllHuman.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_AllHuman.Check = false;
            this.button_AllHuman.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_AllHuman.ForeColor = System.Drawing.Color.White;
            this.button_AllHuman.Location = new System.Drawing.Point(668, 66);
            this.button_AllHuman.Name = "button_AllHuman";
            this.button_AllHuman.Size = new System.Drawing.Size(243, 74);
            this.button_AllHuman.TabIndex = 12;
            this.button_AllHuman.Text = "  ";
            this.button_AllHuman.Click += new System.EventHandler(this.button_AllHuman_Click);
            // 
            // button_EditList
            // 
            this.button_EditList.BackColor = System.Drawing.Color.Transparent;
            this.button_EditList.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_EditList.BackgroundImage")));
            this.button_EditList.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_EditList.Check = false;
            this.button_EditList.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_EditList.ForeColor = System.Drawing.Color.White;
            this.button_EditList.Location = new System.Drawing.Point(203, 414);
            this.button_EditList.Name = "button_EditList";
            this.button_EditList.Size = new System.Drawing.Size(188, 44);
            this.button_EditList.TabIndex = 22;
            this.button_EditList.Text = "Edit User List";
            this.button_EditList.Click += new System.EventHandler(this.button_EditList_Click);
            // 
            // button_LoadUser
            // 
            this.button_LoadUser.BackColor = System.Drawing.Color.Transparent;
            this.button_LoadUser.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_LoadUser.BackgroundImage")));
            this.button_LoadUser.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_LoadUser.Check = false;
            this.button_LoadUser.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_LoadUser.ForeColor = System.Drawing.Color.White;
            this.button_LoadUser.Location = new System.Drawing.Point(433, 414);
            this.button_LoadUser.Name = "button_LoadUser";
            this.button_LoadUser.Size = new System.Drawing.Size(188, 44);
            this.button_LoadUser.TabIndex = 23;
            this.button_LoadUser.Text = "Load User List";
            this.button_LoadUser.Click += new System.EventHandler(this.button_LoadUser_Click);
            // 
            // button_AddUser
            // 
            this.button_AddUser.BackColor = System.Drawing.Color.Transparent;
            this.button_AddUser.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_AddUser.BackgroundImage")));
            this.button_AddUser.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_AddUser.Check = false;
            this.button_AddUser.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_AddUser.ForeColor = System.Drawing.Color.White;
            this.button_AddUser.Location = new System.Drawing.Point(203, 263);
            this.button_AddUser.Name = "button_AddUser";
            this.button_AddUser.Size = new System.Drawing.Size(156, 44);
            this.button_AddUser.TabIndex = 19;
            this.button_AddUser.Text = "  Add User";
            this.button_AddUser.Click += new System.EventHandler(this.button_AddUser_Click);
            // 
            // button_RemoveUser
            // 
            this.button_RemoveUser.BackColor = System.Drawing.Color.Transparent;
            this.button_RemoveUser.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_RemoveUser.BackgroundImage")));
            this.button_RemoveUser.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_RemoveUser.Check = false;
            this.button_RemoveUser.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_RemoveUser.ForeColor = System.Drawing.Color.White;
            this.button_RemoveUser.Location = new System.Drawing.Point(379, 263);
            this.button_RemoveUser.Name = "button_RemoveUser";
            this.button_RemoveUser.Size = new System.Drawing.Size(156, 44);
            this.button_RemoveUser.TabIndex = 20;
            this.button_RemoveUser.Text = "Remove User";
            this.button_RemoveUser.Click += new System.EventHandler(this.button_RemoveUser_Click);
            // 
            // LoadUserTimer
            // 
            this.LoadUserTimer.Interval = 10;
            this.LoadUserTimer.Tick += new System.EventHandler(this.LoadUserTimer_Tick);
            // 
            // horizontalTabs1
            // 
            this.horizontalTabs1.BackColor = System.Drawing.Color.Transparent;
            this.horizontalTabs1.Enabled = false;
            this.horizontalTabs1.Location = new System.Drawing.Point(2, 55);
            this.horizontalTabs1.Name = "horizontalTabs1";
            this.horizontalTabs1.Size = new System.Drawing.Size(636, 38);
            this.horizontalTabs1.Tab1 = "User Profile";
            this.horizontalTabs1.Tab2 = null;
            this.horizontalTabs1.Tab3 = null;
            this.horizontalTabs1.TabIndex = 57;
            this.horizontalTabs1.Tab1_Click += new System.EventHandler(this.horizontalTabs1_Tab1_Click);
            // 
            // UserPanel
            // 
            this.UserPanel.BackColor = System.Drawing.Color.White;
            this.UserPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.UserPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.UserPanel.Controls.Add(this.textBox_UserName);
            this.UserPanel.Location = new System.Drawing.Point(203, 204);
            this.UserPanel.Name = "UserPanel";
            this.UserPanel.Size = new System.Drawing.Size(263, 39);
            this.UserPanel.TabIndex = 12;
            // 
            // lineShape1
            // 
            this.lineShape1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(146)))), ((int)(((byte)(108)))), ((int)(((byte)(175)))));
            this.lineShape1.BorderWidth = 3;
            this.lineShape1.Name = "lineShape1";
            this.lineShape1.X1 = 2;
            this.lineShape1.X2 = 638;
            this.lineShape1.Y1 = 376;
            this.lineShape1.Y2 = 376;
            // 
            // shapeContainer1
            // 
            this.shapeContainer1.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer1.Name = "shapeContainer1";
            this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.lineShape1});
            this.shapeContainer1.Size = new System.Drawing.Size(931, 480);
            this.shapeContainer1.TabIndex = 58;
            this.shapeContainer1.TabStop = false;
            // 
            // UserIcon
            // 
            this.UserIcon.BackColor = System.Drawing.Color.Transparent;
            this.UserIcon.Image = global::GUI_Console.Properties.Resources.DefaultUser_GREY;
            this.UserIcon.Location = new System.Drawing.Point(123, 198);
            this.UserIcon.Name = "UserIcon";
            this.UserIcon.Size = new System.Drawing.Size(50, 50);
            this.UserIcon.TabIndex = 59;
            this.UserIcon.TabStop = false;
            this.UserIcon.Click += new System.EventHandler(this.IconChange_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Arial Narrow", 16F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.label1.Location = new System.Drawing.Point(124, 169);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 26);
            this.label1.TabIndex = 60;
            this.label1.Text = "Icon";
            this.label1.Click += new System.EventHandler(this.IconChange_Click);
            // 
            // UserNameHeader
            // 
            this.UserNameHeader.BackColor = System.Drawing.Color.Transparent;
            this.UserNameHeader.BackgroundImage = global::GUI_Console.Properties.Resources.User_HeaderSml2;
            this.UserNameHeader.Location = new System.Drawing.Point(232, 1);
            this.UserNameHeader.Name = "UserNameHeader";
            this.UserNameHeader.Size = new System.Drawing.Size(353, 37);
            this.UserNameHeader.TabIndex = 61;
            // 
            // RoboSep_Protocols
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.UserNameHeader);
            this.Controls.Add(this.button_AllHuman);
            this.Controls.Add(this.button_FullList);
            this.Controls.Add(this.button_WholeBlood);
            this.Controls.Add(this.button_AllMouse);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.UserIcon);
            this.Controls.Add(this.horizontalTabs1);
            this.Controls.Add(this.dropDownMenu);
            this.Controls.Add(this.Button_ProtocolEdit);
            this.Controls.Add(this.lblProtocolEdit);
            this.Controls.Add(this.lblUser);
            this.Controls.Add(this.button_EditList);
            this.Controls.Add(this.button_RemoveUser);
            this.Controls.Add(this.button_LoadUser);
            this.Controls.Add(this.button_AddUser);
            this.Controls.Add(this.UserPanel);
            this.Controls.Add(this.shapeContainer1);
            this.Name = "RoboSep_Protocols";
            this.Size = new System.Drawing.Size(931, 480);
            this.Load += new System.EventHandler(this.RoboSep_Protocols_Load);
            this.Controls.SetChildIndex(this.shapeContainer1, 0);
            this.Controls.SetChildIndex(this.UserPanel, 0);
            this.Controls.SetChildIndex(this.button_AddUser, 0);
            this.Controls.SetChildIndex(this.button_LoadUser, 0);
            this.Controls.SetChildIndex(this.button_RemoveUser, 0);
            this.Controls.SetChildIndex(this.button_EditList, 0);
            this.Controls.SetChildIndex(this.lblUser, 0);
            this.Controls.SetChildIndex(this.lblProtocolEdit, 0);
            this.Controls.SetChildIndex(this.Button_ProtocolEdit, 0);
            this.Controls.SetChildIndex(this.dropDownMenu, 0);
            this.Controls.SetChildIndex(this.horizontalTabs1, 0);
            this.Controls.SetChildIndex(this.UserIcon, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.btn_home, 0);
            this.Controls.SetChildIndex(this.button_AllMouse, 0);
            this.Controls.SetChildIndex(this.button_WholeBlood, 0);
            this.Controls.SetChildIndex(this.button_FullList, 0);
            this.Controls.SetChildIndex(this.button_AllHuman, 0);
            this.Controls.SetChildIndex(this.UserNameHeader, 0);
            ((System.ComponentModel.ISupportInitialize)(this.dropDownMenu)).EndInit();
            this.UserPanel.ResumeLayout(false);
            this.UserPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UserIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GUI_Controls.Button_SmallPink Button_ProtocolEdit;
        private GUI_Controls.Button_Rectangle button_FullList;
        private GUI_Controls.Button_Rectangle button_WholeBlood;
        private GUI_Controls.Button_Rectangle button_AllMouse;
        private GUI_Controls.Button_Rectangle button_AllHuman;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Label lblProtocolEdit;
        private System.Windows.Forms.PictureBox dropDownMenu;
        private System.Windows.Forms.TextBox textBox_UserName;
        private System.Windows.Forms.Timer LoadUserTimer;
        private GUI_Controls.Button_Rectangle button_RemoveUser;
        private GUI_Controls.Button_Rectangle button_AddUser;
        private GUI_Controls.Button_Rectangle button_EditList;
        private GUI_Controls.Button_Rectangle button_LoadUser;
        private GUI_Controls.HorizontalTabs horizontalTabs1;
        private System.Windows.Forms.Panel UserPanel;
        private Microsoft.VisualBasic.PowerPacks.LineShape lineShape1;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private System.Windows.Forms.PictureBox UserIcon;
        private System.Windows.Forms.Label label1;
        private GUI_Controls.nameHeader UserNameHeader;
    }
}
