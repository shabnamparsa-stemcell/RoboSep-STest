using System.Windows.Forms;


namespace GUI_Console
{
    partial class RoboSep_UserSelect
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoboSep_UserSelect));
            this.load_timer = new System.Windows.Forms.Timer(this.components);
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.lvUser = new GUI_Controls.DragScrollListView3();
            this.btn_home1 = new GUI_Controls.Button_Home();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button_Sort = new GUI_Controls.GUIButton();
            this.button_NewUser = new GUI_Controls.Button_Circle();
            this.button_CloneUser = new GUI_Controls.Button_Circle();
            this.button_DeleteUser = new GUI_Controls.Button_Circle();
            this.scrollBar_User = new GUI_Controls.ScrollBar();
            this.button_EditUser = new GUI_Controls.Button_Circle();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_home
            // 
            this.btn_home.Location = new System.Drawing.Point(10, 410);
            // 
            // load_timer
            // 
            this.load_timer.Interval = 600;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.BackgroundImage = global::GUI_Console.Properties.Resources.logo_mainscreen;
            this.pictureBox2.Location = new System.Drawing.Point(33, 28);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(172, 32);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox2.TabIndex = 60;
            this.pictureBox2.TabStop = false;
            // 
            // lvUser
            // 
            this.lvUser.BackColor = System.Drawing.SystemColors.Control;
            this.lvUser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvUser.FullRowSelect = true;
            this.lvUser.GridLines = true;
            this.lvUser.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvUser.Location = new System.Drawing.Point(39, 147);
            this.lvUser.MultiSelect = false;
            this.lvUser.Name = "lvUser";
            this.lvUser.OwnerDraw = true;
            this.lvUser.Size = new System.Drawing.Size(344, 210);
            this.lvUser.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvUser.TabIndex = 59;
            this.lvUser.UseCompatibleStateImageBehavior = false;
            this.lvUser.View = System.Windows.Forms.View.Details;
            this.lvUser.VisibleRow = 4;
            this.lvUser.VScrollbar = null;
            this.lvUser.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.listview_lvUser_DrawItem);
            this.lvUser.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.listview_lvUser_DrawSubItem);
            this.lvUser.SelectedIndexChanged += new System.EventHandler(this.listview_lvUser_SelectedIndexChanged);
            this.lvUser.Click += new System.EventHandler(this.listview_lvUser_Click);
            // 
            // btn_home1
            // 
            this.btn_home1.BackColor = System.Drawing.Color.Transparent;
            this.btn_home1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_home1.BackgroundImage")));
            this.btn_home1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_home1.Check = false;
            this.btn_home1.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_home1.ForeColor = System.Drawing.Color.White;
            this.btn_home1.Location = new System.Drawing.Point(0, 0);
            this.btn_home1.Name = "btn_home1";
            this.btn_home1.Size = new System.Drawing.Size(89, 89);
            this.btn_home1.TabIndex = 0;
            this.btn_home1.Text = "  ";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BackgroundImage = global::GUI_Console.Properties.Resources.User_header_bg;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.button_Sort);
            this.panel1.Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel1.Location = new System.Drawing.Point(39, 86);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(344, 60);
            this.panel1.TabIndex = 65;
            // 
            // button_Sort
            // 
            this.button_Sort.BackColor = System.Drawing.Color.Transparent;
            this.button_Sort.BackgroundImage = global::GUI_Console.Properties.Resources.GE_BTN11M_sort_ascend_STD;
            this.button_Sort.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Sort.Check = false;
            this.button_Sort.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Sort.ForeColor = System.Drawing.Color.White;
            this.button_Sort.Location = new System.Drawing.Point(3, 4);
            this.button_Sort.Name = "button_Sort";
            this.button_Sort.Size = new System.Drawing.Size(52, 52);
            this.button_Sort.TabIndex = 65;
            this.button_Sort.Text = "  ";
            this.button_Sort.Click += new System.EventHandler(this.button_Sort_Click);
            // 
            // button_NewUser
            // 
            this.button_NewUser.BackColor = System.Drawing.Color.Transparent;
            this.button_NewUser.BackgroundImage = global::GUI_Console.Properties.Resources.US_BTN02L_add_user_STD;
            this.button_NewUser.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_NewUser.Check = false;
            this.button_NewUser.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_NewUser.ForeColor = System.Drawing.Color.White;
            this.button_NewUser.Location = new System.Drawing.Point(418, 386);
            this.button_NewUser.Name = "button_NewUser";
            this.button_NewUser.Size = new System.Drawing.Size(104, 86);
            this.button_NewUser.TabIndex = 66;
            this.button_NewUser.Text = "New User";
            this.button_NewUser.Click += new System.EventHandler(this.button_NewUser_Click);
            // 
            // button_CloneUser
            // 
            this.button_CloneUser.BackColor = System.Drawing.Color.Transparent;
            this.button_CloneUser.BackgroundImage = global::GUI_Console.Properties.Resources.US_BTN01L_clone_user_STD;
            this.button_CloneUser.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_CloneUser.Check = false;
            this.button_CloneUser.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_CloneUser.ForeColor = System.Drawing.Color.White;
            this.button_CloneUser.Location = new System.Drawing.Point(198, 386);
            this.button_CloneUser.Name = "button_CloneUser";
            this.button_CloneUser.Size = new System.Drawing.Size(104, 86);
            this.button_CloneUser.TabIndex = 67;
            this.button_CloneUser.Text = "Clone User";
            this.button_CloneUser.Click += new System.EventHandler(this.button_CloneUser_Click);
            // 
            // button_DeleteUser
            // 
            this.button_DeleteUser.BackColor = System.Drawing.Color.Transparent;
            this.button_DeleteUser.BackgroundImage = global::GUI_Console.Properties.Resources.US_BTN03L_remove_user_STD;
            this.button_DeleteUser.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_DeleteUser.Check = false;
            this.button_DeleteUser.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_DeleteUser.ForeColor = System.Drawing.Color.White;
            this.button_DeleteUser.Location = new System.Drawing.Point(528, 386);
            this.button_DeleteUser.Name = "button_DeleteUser";
            this.button_DeleteUser.Size = new System.Drawing.Size(104, 86);
            this.button_DeleteUser.TabIndex = 68;
            this.button_DeleteUser.Text = "New User";
            this.button_DeleteUser.Click += new System.EventHandler(this.button_DeleteUser_Click);
            // 
            // scrollBar_User
            // 
            this.scrollBar_User.ActiveBackColor = System.Drawing.Color.Gray;
            this.scrollBar_User.LargeChange = 10;
            this.scrollBar_User.Location = new System.Drawing.Point(386, 147);
            this.scrollBar_User.Maximum = 99;
            this.scrollBar_User.Minimum = 0;
            this.scrollBar_User.Name = "scrollBar_User";
            this.scrollBar_User.Size = new System.Drawing.Size(75, 210);
            this.scrollBar_User.SmallChange = 1;
            this.scrollBar_User.TabIndex = 69;
            this.scrollBar_User.Text = "scrollBar1";
            this.scrollBar_User.ThumbStyle = GUI_Controls.ScrollBar.EnumThumbStyle.Auto;
            this.scrollBar_User.Value = 0;
            // 
            // button_EditUser
            // 
            this.button_EditUser.BackColor = System.Drawing.Color.Transparent;
            this.button_EditUser.BackgroundImage = global::GUI_Console.Properties.Resources.US_BTN05L_edit_user_STD;
            this.button_EditUser.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_EditUser.Check = false;
            this.button_EditUser.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_EditUser.ForeColor = System.Drawing.Color.White;
            this.button_EditUser.Location = new System.Drawing.Point(308, 386);
            this.button_EditUser.Name = "button_EditUser";
            this.button_EditUser.Size = new System.Drawing.Size(104, 86);
            this.button_EditUser.TabIndex = 70;
            this.button_EditUser.Text = "Edit User";
            this.button_EditUser.Click += new System.EventHandler(this.button_EditUser_Click);
            // 
            // RoboSep_UserSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.button_EditUser);
            this.Controls.Add(this.scrollBar_User);
            this.Controls.Add(this.button_DeleteUser);
            this.Controls.Add(this.button_CloneUser);
            this.Controls.Add(this.button_NewUser);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.lvUser);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.Name = "RoboSep_UserSelect";
            this.Size = new System.Drawing.Size(767, 496);
            this.Load += new System.EventHandler(this.RoboSep_UserSelect_Load);
            this.Controls.SetChildIndex(this.lvUser, 0);
            this.Controls.SetChildIndex(this.pictureBox2, 0);
            this.Controls.SetChildIndex(this.panel1, 0);
            this.Controls.SetChildIndex(this.button_NewUser, 0);
            this.Controls.SetChildIndex(this.btn_home, 0);
            this.Controls.SetChildIndex(this.button_CloneUser, 0);
            this.Controls.SetChildIndex(this.button_DeleteUser, 0);
            this.Controls.SetChildIndex(this.scrollBar_User, 0);
            this.Controls.SetChildIndex(this.button_EditUser, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer load_timer;
        private GUI_Controls.DragScrollListView3 lvUser;
        private System.Windows.Forms.PictureBox pictureBox2;
        private GUI_Controls.Button_Home btn_home1;
        private System.Windows.Forms.Panel panel1;
        private GUI_Controls.Button_Circle button_NewUser;
        private GUI_Controls.Button_Circle button_CloneUser;
        private GUI_Controls.Button_Circle button_DeleteUser;
        private GUI_Controls.GUIButton button_Sort;
        private GUI_Controls.ScrollBar scrollBar_User;
        private GUI_Controls.Button_Circle button_EditUser;
    }
}
