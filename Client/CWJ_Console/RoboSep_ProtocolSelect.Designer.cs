using Tesla.Common;


namespace GUI_Console
{
    partial class RoboSep_ProtocolSelect
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        /// 

        
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            int i;
            for (i = 0; i < ilist1.Count; i++)
                ilist1[i].Dispose();

            for (i = 0; i < ilist2.Count; i++)
                ilist2[i].Dispose();
            
            for (i = 0; i < ilist3.Count; i++)
                ilist3[i].Dispose();
            
            theProtocolSelect = null;
            
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoboSep_ProtocolSelect));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.LoadProtocolTimer = new System.Windows.Forms.Timer(this.components);
            this.SelectProtocolTimer = new System.Windows.Forms.Timer(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.protocol_SearchBar2 = new GUI_Controls.Protocol_SearchBar2();
            this.UserNameHeader = new GUI_Controls.nameHeader();
            this.btn_home2 = new GUI_Controls.btn_home2();
            this.scrollIndicator1 = new GUI_Controls.ScrollIndicator();
            this.btn_AddProtocols = new GUI_Controls.Button_Circle();
            this.btn_SelectProtocol = new GUI_Controls.Button_Circle();
            this.btn_RemoveProtocols = new GUI_Controls.Button_Circle();
            this.horizontalTabs1 = new GUI_Controls.HorizontalTabs();
            this.listView_UserProtocols = new GUI_Controls.DragScrollListView3();
            this.Column1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // LoadProtocolTimer
            // 
            this.LoadProtocolTimer.Interval = 150;
            this.LoadProtocolTimer.Tick += new System.EventHandler(this.LoadProtocolTimer_Tick);
            // 
            // SelectProtocolTimer
            // 
            this.SelectProtocolTimer.Interval = 200;
            this.SelectProtocolTimer.Tick += new System.EventHandler(this.SelectProtocolTimer_Tick);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.panel2.BackgroundImage = global::GUI_Console.Properties.Resources.protocolHeaderBackground;
            this.panel2.Controls.Add(this.protocol_SearchBar2);
            this.panel2.Location = new System.Drawing.Point(2, 92);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(636, 69);
            this.panel2.TabIndex = 52;
            // 
            // protocol_SearchBar2
            // 
            this.protocol_SearchBar2.BackColor = System.Drawing.SystemColors.Control;
            this.protocol_SearchBar2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("protocol_SearchBar2.BackgroundImage")));
            this.protocol_SearchBar2.Location = new System.Drawing.Point(-2, 0);
            this.protocol_SearchBar2.Name = "protocol_SearchBar2";
            this.protocol_SearchBar2.Size = new System.Drawing.Size(640, 69);
            this.protocol_SearchBar2.TabIndex = 0;
            // 
            // UserNameHeader
            // 
            this.UserNameHeader.BackColor = System.Drawing.Color.Transparent;
            this.UserNameHeader.BackgroundImage = global::GUI_Console.Properties.Resources.User_HeaderSml2;
            this.UserNameHeader.Location = new System.Drawing.Point(285, 1);
            this.UserNameHeader.Name = "UserNameHeader";
            this.UserNameHeader.Size = new System.Drawing.Size(353, 37);
            this.UserNameHeader.TabIndex = 64;
            // 
            // btn_home2
            // 
            this.btn_home2.BackColor = System.Drawing.Color.Transparent;
            this.btn_home2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_home2.BackgroundImage")));
            this.btn_home2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_home2.Check = false;
            this.btn_home2.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_home2.ForeColor = System.Drawing.Color.White;
            this.btn_home2.Location = new System.Drawing.Point(11, 408);
            this.btn_home2.Name = "btn_home2";
            this.btn_home2.Size = new System.Drawing.Size(51, 51);
            this.btn_home2.TabIndex = 63;
            this.btn_home2.Text = "btn_home2";
            this.btn_home2.Click += new System.EventHandler(this.btn_home2_Click);
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
            this.scrollIndicator1.Location = new System.Drawing.Point(622, 162);
            this.scrollIndicator1.Maximum = 100;
            this.scrollIndicator1.Minimum = 0;
            this.scrollIndicator1.Name = "scrollIndicator1";
            this.scrollIndicator1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.scrollIndicator1.Size = new System.Drawing.Size(18, 208);
            this.scrollIndicator1.SmallChange = 1;
            this.scrollIndicator1.TabIndex = 62;
            this.scrollIndicator1.Text = "scrollIndicator1";
            this.scrollIndicator1.ThumbCustomShape = null;
            this.scrollIndicator1.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(38)))), ((int)(((byte)(131)))));
            this.scrollIndicator1.ThumbOuterColor = System.Drawing.Color.White;
            this.scrollIndicator1.ThumbPenColor = System.Drawing.Color.Silver;
            this.scrollIndicator1.ThumbRoundRectSize = new System.Drawing.Size(8, 8);
            this.scrollIndicator1.ThumbSize = 15;
            this.scrollIndicator1.Value = 50;
            // 
            // btn_AddProtocols
            // 
            this.btn_AddProtocols.BackColor = System.Drawing.Color.Transparent;
            this.btn_AddProtocols.BackgroundImage = global::GUI_Console.Properties.Resources.PR_BTN08L_add_protocol_STD;
            this.btn_AddProtocols.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_AddProtocols.Check = false;
            this.btn_AddProtocols.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_AddProtocols.ForeColor = System.Drawing.Color.White;
            this.btn_AddProtocols.Location = new System.Drawing.Point(308, 388);
            this.btn_AddProtocols.Name = "btn_AddProtocols";
            this.btn_AddProtocols.Size = new System.Drawing.Size(104, 86);
            this.btn_AddProtocols.TabIndex = 60;
            this.btn_AddProtocols.Text = "Add";
            this.btn_AddProtocols.Click += new System.EventHandler(this.btn_AddProtocols_Click);
            // 
            // btn_SelectProtocol
            // 
            this.btn_SelectProtocol.BackColor = System.Drawing.Color.Transparent;
            this.btn_SelectProtocol.BackgroundImage = global::GUI_Console.Properties.Resources.L_104x86_single_arrow_right_STD;
            this.btn_SelectProtocol.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_SelectProtocol.Check = false;
            this.btn_SelectProtocol.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_SelectProtocol.ForeColor = System.Drawing.Color.White;
            this.btn_SelectProtocol.Location = new System.Drawing.Point(530, 388);
            this.btn_SelectProtocol.Name = "btn_SelectProtocol";
            this.btn_SelectProtocol.Size = new System.Drawing.Size(104, 86);
            this.btn_SelectProtocol.TabIndex = 59;
            this.btn_SelectProtocol.Text = "  ";
            // 
            // btn_RemoveProtocols
            // 
            this.btn_RemoveProtocols.BackColor = System.Drawing.Color.Transparent;
            this.btn_RemoveProtocols.BackgroundImage = global::GUI_Console.Properties.Resources.PR_BTN09L_remove_protocol_STD;
            this.btn_RemoveProtocols.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_RemoveProtocols.Check = false;
            this.btn_RemoveProtocols.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_RemoveProtocols.ForeColor = System.Drawing.Color.White;
            this.btn_RemoveProtocols.Location = new System.Drawing.Point(419, 388);
            this.btn_RemoveProtocols.Name = "btn_RemoveProtocols";
            this.btn_RemoveProtocols.Size = new System.Drawing.Size(104, 86);
            this.btn_RemoveProtocols.TabIndex = 57;
            this.btn_RemoveProtocols.Text = "button_Circle1";
            this.btn_RemoveProtocols.Click += new System.EventHandler(this.btn_RemoveProtocols_Click);
            // 
            // horizontalTabs1
            // 
            this.horizontalTabs1.BackColor = System.Drawing.Color.Transparent;
            this.horizontalTabs1.Location = new System.Drawing.Point(2, 55);
            this.horizontalTabs1.Name = "horizontalTabs1";
            this.horizontalTabs1.Size = new System.Drawing.Size(636, 38);
            this.horizontalTabs1.Tab1 = "All Protocols";
            this.horizontalTabs1.Tab2 = "My Protocols";
            this.horizontalTabs1.Tab3 = null;
            this.horizontalTabs1.TabIndex = 53;
            this.horizontalTabs1.Tab1_Click += new System.EventHandler(this.horizontalTabs1_Tab1_Click);
            this.horizontalTabs1.Tab2_Click += new System.EventHandler(this.horizontalTabs1_Tab2_Click);
            // 
            // listView_UserProtocols
            // 
            this.listView_UserProtocols.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listView_UserProtocols.BackColor = System.Drawing.SystemColors.Control;
            this.listView_UserProtocols.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView_UserProtocols.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Column1});
            this.listView_UserProtocols.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listView_UserProtocols.FullRowSelect = true;
            this.listView_UserProtocols.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView_UserProtocols.HideSelection = false;
            this.listView_UserProtocols.LabelWrap = false;
            this.listView_UserProtocols.Location = new System.Drawing.Point(2, 162);
            this.listView_UserProtocols.MultiSelect = false;
            this.listView_UserProtocols.Name = "listView_UserProtocols";
            this.listView_UserProtocols.OwnerDraw = true;
            this.listView_UserProtocols.ShowGroups = false;
            this.listView_UserProtocols.Size = new System.Drawing.Size(615, 208);
            this.listView_UserProtocols.SmallImageList = this.imageList1;
            this.listView_UserProtocols.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listView_UserProtocols.TabIndex = 15;
            this.listView_UserProtocols.TileSize = new System.Drawing.Size(1, 45);
            this.listView_UserProtocols.UseCompatibleStateImageBehavior = false;
            this.listView_UserProtocols.View = System.Windows.Forms.View.Details;
            this.listView_UserProtocols.VisibleRow = 5;
            this.listView_UserProtocols.VScrollbar = null;
            this.listView_UserProtocols.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.listView_UserProtocols_DrawColumnHeader);
            this.listView_UserProtocols.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.listView_UserProtocols_DrawItem);
            this.listView_UserProtocols.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.listView_UserProtocols_DrawSubItem);
            this.listView_UserProtocols.Click += new System.EventHandler(this.listView_UserProtocols_Click);
            // 
            // Column1
            // 
            this.Column1.Text = "Protocol Name";
            this.Column1.Width = 2000;
            // 
            // RoboSep_ProtocolSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this.Controls.Add(this.UserNameHeader);
            this.Controls.Add(this.btn_home2);
            this.Controls.Add(this.scrollIndicator1);
            this.Controls.Add(this.btn_AddProtocols);
            this.Controls.Add(this.btn_SelectProtocol);
            this.Controls.Add(this.btn_RemoveProtocols);
            this.Controls.Add(this.horizontalTabs1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.listView_UserProtocols);
            this.Name = "RoboSep_ProtocolSelect";
            this.Size = new System.Drawing.Size(640, 480);
            this.Load += new System.EventHandler(this.RoboSep_ProtocolSelect_Load);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ColumnHeader Column1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Timer LoadProtocolTimer;
        private GUI_Controls.DragScrollListView3 listView_UserProtocols;
        private System.Windows.Forms.Panel panel2;
        private GUI_Controls.HorizontalTabs horizontalTabs1;
        private GUI_Controls.Button_Circle btn_RemoveProtocols;
        private GUI_Controls.Button_Circle btn_SelectProtocol;
        private System.Windows.Forms.Timer SelectProtocolTimer;
        private GUI_Controls.Button_Circle btn_AddProtocols;
        private GUI_Controls.ScrollIndicator scrollIndicator1;
        private GUI_Controls.btn_home2 btn_home2;
        private GUI_Controls.nameHeader UserNameHeader;
        private GUI_Controls.Protocol_SearchBar2 protocol_SearchBar2;
    }
}
