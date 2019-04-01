namespace GUI_Console
{
    partial class Form_UserSelect
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_UserSelect));
            this.spacer = new System.Windows.Forms.ImageList(this.components);
            this.rectangleShape1 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label_WindowTitle = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button_Sort = new GUI_Controls.GUIButton();
            this.button_ScrollUp = new GUI_Controls.Button_Scroll();
            this.button_ScrollDown = new GUI_Controls.Button_Scroll();
            this.listView_users = new GUI_Controls.DragScrollListView();
            this.Column1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button_Cancel = new GUI_Controls.Button_Cancel();
            this.button_OK = new GUI_Controls.Button_Circle();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // spacer
            // 
            this.spacer.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.spacer.ImageSize = new System.Drawing.Size(1, 35);
            this.spacer.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // rectangleShape1
            // 
            this.rectangleShape1.BorderWidth = 2;
            this.rectangleShape1.Location = new System.Drawing.Point(1, 2);
            this.rectangleShape1.Name = "rectangleShape1";
            this.rectangleShape1.Size = new System.Drawing.Size(292, 397);
            // 
            // shapeContainer1
            // 
            this.shapeContainer1.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer1.Name = "shapeContainer1";
            this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.rectangleShape1});
            this.shapeContainer1.Size = new System.Drawing.Size(294, 400);
            this.shapeContainer1.TabIndex = 30;
            this.shapeContainer1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.Controls.Add(this.label_WindowTitle);
            this.panel1.Location = new System.Drawing.Point(2, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(290, 35);
            this.panel1.TabIndex = 29;
            // 
            // label_WindowTitle
            // 
            this.label_WindowTitle.BackColor = System.Drawing.Color.Transparent;
            this.label_WindowTitle.Font = new System.Drawing.Font("Arial", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_WindowTitle.ForeColor = System.Drawing.Color.White;
            this.label_WindowTitle.Location = new System.Drawing.Point(12, 3);
            this.label_WindowTitle.Margin = new System.Windows.Forms.Padding(0);
            this.label_WindowTitle.Name = "label_WindowTitle";
            this.label_WindowTitle.Size = new System.Drawing.Size(272, 27);
            this.label_WindowTitle.TabIndex = 3;
            this.label_WindowTitle.Text = "Window Title";
            this.label_WindowTitle.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button_Sort);
            this.panel2.Controls.Add(this.button_ScrollUp);
            this.panel2.Controls.Add(this.button_ScrollDown);
            this.panel2.Location = new System.Drawing.Point(3, 36);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(288, 63);
            this.panel2.TabIndex = 66;
            // 
            // button_Sort
            // 
            this.button_Sort.BackColor = System.Drawing.Color.Transparent;
            this.button_Sort.BackgroundImage = global::GUI_Console.Properties.Resources.GE_BTN11M_sort_ascend_STD;
            this.button_Sort.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Sort.Check = false;
            this.button_Sort.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Sort.ForeColor = System.Drawing.Color.White;
            this.button_Sort.Location = new System.Drawing.Point(0, 6);
            this.button_Sort.Name = "button_Sort";
            this.button_Sort.Size = new System.Drawing.Size(52, 52);
            this.button_Sort.TabIndex = 65;
            this.button_Sort.Text = "  ";
            this.button_Sort.Click += new System.EventHandler(this.button_Sort_Click);
            // 
            // button_ScrollUp
            // 
            this.button_ScrollUp.BackColor = System.Drawing.Color.Transparent;
            this.button_ScrollUp.BackgroundImage = global::GUI_Console.Properties.Resources.GE_BTN03M_up_arrow_STD;
            this.button_ScrollUp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_ScrollUp.Check = false;
            this.button_ScrollUp.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_ScrollUp.ForeColor = System.Drawing.Color.White;
            this.button_ScrollUp.Location = new System.Drawing.Point(231, 6);
            this.button_ScrollUp.Name = "button_ScrollUp";
            this.button_ScrollUp.Size = new System.Drawing.Size(52, 52);
            this.button_ScrollUp.TabIndex = 62;
            this.button_ScrollUp.Text = "  ";
            this.button_ScrollUp.Click += new System.EventHandler(this.button_ScrollUp_Click);
            this.button_ScrollUp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button_ScrollUp_MouseDown);
            this.button_ScrollUp.MouseUp += new System.Windows.Forms.MouseEventHandler(this.button_ScrollUp_MouseUp);
            // 
            // button_ScrollDown
            // 
            this.button_ScrollDown.BackColor = System.Drawing.Color.Transparent;
            this.button_ScrollDown.BackgroundImage = global::GUI_Console.Properties.Resources.GE_BTN04M_down_arrow_STD;
            this.button_ScrollDown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_ScrollDown.Check = false;
            this.button_ScrollDown.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_ScrollDown.ForeColor = System.Drawing.Color.White;
            this.button_ScrollDown.Location = new System.Drawing.Point(169, 6);
            this.button_ScrollDown.Name = "button_ScrollDown";
            this.button_ScrollDown.Size = new System.Drawing.Size(52, 52);
            this.button_ScrollDown.TabIndex = 63;
            this.button_ScrollDown.Text = "  ";
            this.button_ScrollDown.Click += new System.EventHandler(this.button_ScrollDown_Click);
            this.button_ScrollDown.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button_ScrollDown_MouseDown);
            this.button_ScrollDown.MouseUp += new System.Windows.Forms.MouseEventHandler(this.button_ScrollDown_MouseUp);
            // 
            // listView_users
            // 
            this.listView_users.BackColor = System.Drawing.SystemColors.Control;
            this.listView_users.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView_users.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Column1});
            this.listView_users.Font = new System.Drawing.Font("Arial Narrow", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listView_users.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.listView_users.FullRowSelect = true;
            this.listView_users.GridLines = true;
            this.listView_users.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView_users.HideSelection = false;
            this.listView_users.Location = new System.Drawing.Point(10, 101);
            this.listView_users.MultiSelect = false;
            this.listView_users.Name = "listView_users";
            this.listView_users.OwnerDraw = true;
            this.listView_users.Size = new System.Drawing.Size(275, 180);
            this.listView_users.SmallImageList = this.spacer;
            this.listView_users.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listView_users.TabIndex = 15;
            this.listView_users.TileSize = new System.Drawing.Size(1, 30);
            this.listView_users.UseCompatibleStateImageBehavior = false;
            this.listView_users.View = System.Windows.Forms.View.Details;
            this.listView_users.VisibleRow = 5;
            this.listView_users.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.listView_users_DrawColumnHeader);
            this.listView_users.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.listView_users_DrawItem);
            this.listView_users.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.listView_users_DrawSubItem);
            this.listView_users.SelectedIndexChanged += new System.EventHandler(listView_users_SelectedIndexChanged);
            // 
            // Column1
            // 
            this.Column1.Text = "Date / Time";
            this.Column1.Width = 278;
            // 
            // button_Cancel
            // 
            this.button_Cancel.BackColor = System.Drawing.Color.Transparent;
            this.button_Cancel.BackgroundImage = global::GUI_Console.Properties.Resources.L_104x86_single_arrow_left_STD;
            this.button_Cancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Cancel.Check = false;
            this.button_Cancel.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Cancel.ForeColor = System.Drawing.Color.White;
            this.button_Cancel.Location = new System.Drawing.Point(72, 306);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(104, 86);
            this.button_Cancel.TabIndex = 32;
            this.button_Cancel.Text = "button_Cancel2";
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // button_OK
            // 
            this.button_OK.BackColor = System.Drawing.Color.Transparent;
            this.button_OK.BackgroundImage = global::GUI_Console.Properties.Resources.GE_BTN20L_ok_STD;
            this.button_OK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_OK.Check = false;
            this.button_OK.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_OK.ForeColor = System.Drawing.Color.White;
            this.button_OK.Location = new System.Drawing.Point(182, 306);
            this.button_OK.Name = "button_OK";
            this.button_OK.Size = new System.Drawing.Size(104, 86);
            this.button_OK.TabIndex = 31;
            this.button_OK.Text = "OK";
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // Form_UserSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(294, 400);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.listView_users);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.shapeContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form_UserSelect";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form_UserSelect";
            this.Deactivate += new System.EventHandler(this.Form_UserSelect_Deactivate);
            this.Load += new System.EventHandler(this.Form_UserSelect_Load);
            this.LocationChanged += new System.EventHandler(this.Form_UserSelect_LocationChanged);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private GUI_Controls.DragScrollListView listView_users;
        private System.Windows.Forms.ColumnHeader Column1;
        private System.Windows.Forms.Panel panel1;
        protected System.Windows.Forms.Label label_WindowTitle;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape1;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private System.Windows.Forms.ImageList spacer;
        private GUI_Controls.Button_Circle button_OK;
        private GUI_Controls.Button_Cancel button_Cancel;
        private System.Windows.Forms.Panel panel2;
        private GUI_Controls.Button_Scroll button_ScrollUp;
        private GUI_Controls.Button_Scroll button_ScrollDown;
        private GUI_Controls.GUIButton button_Sort;

    }
}