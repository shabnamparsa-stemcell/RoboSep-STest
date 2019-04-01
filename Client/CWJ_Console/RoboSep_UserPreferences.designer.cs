using Tesla.Common;


namespace GUI_Console
{
    partial class RoboSep_UserPreferences
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoboSep_UserPreferences));
            this.panel2 = new System.Windows.Forms.Panel();
            this.button_ScrollUp = new GUI_Controls.Button_Scroll();
            this.button_ScrollDown = new GUI_Controls.Button_Scroll();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.UserNameHeader = new GUI_Controls.nameHeader();
            this.scrollIndicator1 = new GUI_Controls.ScrollIndicator();
            this.horizontalTabs1 = new GUI_Controls.HorizontalTabs();
            this.listView_UserPref = new GUI_Controls.DragScrollListView2();
            this.Column1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Column2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button_Cancel = new GUI_Controls.Button_Cancel();
            this.button_Save = new GUI_Controls.Button_Circle();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel2.BackgroundImage")));
            this.panel2.Controls.Add(this.button_ScrollUp);
            this.panel2.Controls.Add(this.button_ScrollDown);
            this.panel2.Location = new System.Drawing.Point(2, 95);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(636, 65);
            this.panel2.TabIndex = 72;
            // 
            // button_ScrollUp
            // 
            this.button_ScrollUp.BackColor = System.Drawing.Color.Transparent;
            this.button_ScrollUp.BackgroundImage = global::GUI_Console.Properties.Resources.GE_BTN03M_up_arrow_STD;
            this.button_ScrollUp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_ScrollUp.Check = false;
            this.button_ScrollUp.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_ScrollUp.ForeColor = System.Drawing.Color.White;
            this.button_ScrollUp.Location = new System.Drawing.Point(578, 7);
            this.button_ScrollUp.Name = "button_ScrollUp";
            this.button_ScrollUp.Size = new System.Drawing.Size(52, 52);
            this.button_ScrollUp.TabIndex = 74;
            this.button_ScrollUp.Text = "  ";
            this.button_ScrollUp.Click += new System.EventHandler(this.button_ScrollUp_Click);
            // 
            // button_ScrollDown
            // 
            this.button_ScrollDown.BackColor = System.Drawing.Color.Transparent;
            this.button_ScrollDown.BackgroundImage = global::GUI_Console.Properties.Resources.GE_BTN04M_down_arrow_STD;
            this.button_ScrollDown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_ScrollDown.Check = false;
            this.button_ScrollDown.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_ScrollDown.ForeColor = System.Drawing.Color.White;
            this.button_ScrollDown.Location = new System.Drawing.Point(515, 7);
            this.button_ScrollDown.Name = "button_ScrollDown";
            this.button_ScrollDown.Size = new System.Drawing.Size(52, 52);
            this.button_ScrollDown.TabIndex = 75;
            this.button_ScrollDown.Text = "  ";
            this.button_ScrollDown.Click += new System.EventHandler(this.button_ScrollDown_Click);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(13, 13);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // timer1
            // 
            this.timer1.Interval = 1500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // UserNameHeader
            // 
            this.UserNameHeader.BackColor = System.Drawing.Color.Transparent;
            this.UserNameHeader.BackgroundImage = global::GUI_Console.Properties.Resources.User_HeaderSml2;
            this.UserNameHeader.Location = new System.Drawing.Point(285, 1);
            this.UserNameHeader.Name = "UserNameHeader";
            this.UserNameHeader.Size = new System.Drawing.Size(353, 37);
            this.UserNameHeader.TabIndex = 76;
            // 
            // scrollIndicator1
            // 
            this.scrollIndicator1.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(194)))), ((int)(((byte)(164)))), ((int)(((byte)(217)))));
            this.scrollIndicator1.BarOuterColor = System.Drawing.Color.FromArgb(((int)(((byte)(194)))), ((int)(((byte)(164)))), ((int)(((byte)(217)))));
            this.scrollIndicator1.BarPenColor = System.Drawing.Color.Gainsboro;
            this.scrollIndicator1.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.scrollIndicator1.ColorSchema = GUI_Controls.ScrollIndicator.ColorSchemas.PerlPurple;
            this.scrollIndicator1.DrawFocusRectangle = true;
            this.scrollIndicator1.DrawSemitransparentThumb = true;
            this.scrollIndicator1.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(38)))), ((int)(((byte)(131)))));
            this.scrollIndicator1.ElapsedOuterColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(38)))), ((int)(((byte)(131)))));
            this.scrollIndicator1.LargeChange = 5;
            this.scrollIndicator1.Location = new System.Drawing.Point(617, 162);
            this.scrollIndicator1.Maximum = 100;
            this.scrollIndicator1.Minimum = 0;
            this.scrollIndicator1.Name = "scrollIndicator1";
            this.scrollIndicator1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.scrollIndicator1.Size = new System.Drawing.Size(18, 199);
            this.scrollIndicator1.SmallChange = 1;
            this.scrollIndicator1.TabIndex = 75;
            this.scrollIndicator1.Text = "scrollIndicator1";
            this.scrollIndicator1.ThumbCustomShape = null;
            this.scrollIndicator1.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(38)))), ((int)(((byte)(131)))));
            this.scrollIndicator1.ThumbOuterColor = System.Drawing.Color.White;
            this.scrollIndicator1.ThumbPenColor = System.Drawing.Color.Silver;
            this.scrollIndicator1.ThumbRoundRectSize = new System.Drawing.Size(8, 8);
            this.scrollIndicator1.ThumbSize = 15;
            this.scrollIndicator1.Value = 50;
            this.scrollIndicator1.Visible = false;
            // 
            // horizontalTabs1
            // 
            this.horizontalTabs1.BackColor = System.Drawing.Color.Transparent;
            this.horizontalTabs1.Location = new System.Drawing.Point(2, 58);
            this.horizontalTabs1.Name = "horizontalTabs1";
            this.horizontalTabs1.Size = new System.Drawing.Size(636, 38);
            this.horizontalTabs1.Tab1 = "User Preferences";
            this.horizontalTabs1.Tab2 = "Device Preferences";
            this.horizontalTabs1.Tab3 = null;
            this.horizontalTabs1.TabIndex = 73;
            this.horizontalTabs1.Tab1_Click += new System.EventHandler(this.horizontalTabs1_Tab1_Click);
            this.horizontalTabs1.Tab2_Click += new System.EventHandler(this.horizontalTabs1_Tab2_Click);
            // 
            // listView_UserPref
            // 
            this.listView_UserPref.BackColor = System.Drawing.SystemColors.Control;
            this.listView_UserPref.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView_UserPref.CheckBoxes = true;
            this.listView_UserPref.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Column1,
            this.Column2});
            this.listView_UserPref.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listView_UserPref.GridLines = true;
            this.listView_UserPref.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView_UserPref.HideSelection = false;
            this.listView_UserPref.LabelWrap = false;
            this.listView_UserPref.Location = new System.Drawing.Point(2, 162);
            this.listView_UserPref.MultiSelect = false;
            this.listView_UserPref.Name = "listView_UserPref";
            this.listView_UserPref.OwnerDraw = true;
            this.listView_UserPref.ShowGroups = false;
            this.listView_UserPref.Size = new System.Drawing.Size(609, 199);
            this.listView_UserPref.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listView_UserPref.TabIndex = 71;
            this.listView_UserPref.TileSize = new System.Drawing.Size(1, 45);
            this.listView_UserPref.UseCompatibleStateImageBehavior = false;
            this.listView_UserPref.View = System.Windows.Forms.View.Details;
            this.listView_UserPref.VisibleRow = 5;
            this.listView_UserPref.VScrollbar = null;
            this.listView_UserPref.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.listView_UserPref_DrawColumnHeader);
            this.listView_UserPref.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.listView_UserPref_DrawSubItem);
            this.listView_UserPref.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listView_UserPref_HandleItemChecked);
            this.listView_UserPref.Click += new System.EventHandler(this.listView_UserPref_Click);
            // 
            // Column1
            // 
            this.Column1.Text = "Settings";
            this.Column1.Width = 567;
            // 
            // Column2
            // 
            this.Column2.Text = "Help";
            this.Column2.Width = 40;
            // 
            // button_Cancel
            // 
            this.button_Cancel.BackColor = System.Drawing.Color.Transparent;
            this.button_Cancel.BackgroundImage = global::GUI_Console.Properties.Resources.L_104x86_single_arrow_left_STD;
            this.button_Cancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Cancel.Check = false;
            this.button_Cancel.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Cancel.ForeColor = System.Drawing.Color.White;
            this.button_Cancel.Location = new System.Drawing.Point(415, 388);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(104, 86);
            this.button_Cancel.TabIndex = 68;
            this.button_Cancel.Text = "Cancel";
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // button_Save
            // 
            this.button_Save.BackColor = System.Drawing.Color.Transparent;
            this.button_Save.BackgroundImage = global::GUI_Console.Properties.Resources.GE_BTN20L_ok_STD;
            this.button_Save.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Save.Check = false;
            this.button_Save.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Save.ForeColor = System.Drawing.Color.White;
            this.button_Save.Location = new System.Drawing.Point(526, 388);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(104, 86);
            this.button_Save.TabIndex = 65;
            this.button_Save.Text = "Save";
            this.button_Save.Click += new System.EventHandler(this.button_Save_Click);
            // 
            // RoboSep_UserPreferences
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this.Controls.Add(this.UserNameHeader);
            this.Controls.Add(this.scrollIndicator1);
            this.Controls.Add(this.horizontalTabs1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.listView_UserPref);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_Save);
            this.Name = "RoboSep_UserPreferences";
            this.Size = new System.Drawing.Size(640, 480);
            this.Load += new System.EventHandler(this.RoboSep_UserPreferences_Load);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private GUI_Controls.Button_Circle button_Save;
        private GUI_Controls.Button_Cancel button_Cancel;
        private GUI_Controls.HorizontalTabs horizontalTabs1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ColumnHeader Column1;
        private GUI_Controls.Button_Scroll button_ScrollUp;
        private GUI_Controls.Button_Scroll button_ScrollDown;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ColumnHeader Column2;
        private GUI_Controls.DragScrollListView2 listView_UserPref;
        private GUI_Controls.ScrollIndicator scrollIndicator1;
        private GUI_Controls.nameHeader UserNameHeader;
        private System.Windows.Forms.Timer timer1;
    }
}
