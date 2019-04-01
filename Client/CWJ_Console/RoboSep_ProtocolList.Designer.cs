namespace GUI_Console
{
    partial class RoboSep_ProtocolList
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
            int i;
            for (i = 0; i < ilistUserAdd.Count ;  i++)
            {
                ilistUserAdd[i].Dispose();
            }
            for (i = 0; i < ilistUserRemove.Count; i++ )
            {
                ilistUserRemove[i].Dispose();
            }
            for (i = 0; i < ilist1.Count; i++ )
            {
                ilist1[i].Dispose();
            }
            for (i = 0; i < ilist2.Count; i++)
            {
                ilist2[i].Dispose();
            }
            myProtocolList = null;
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoboSep_ProtocolList));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.btn_AddProtocols = new GUI_Controls.Button_Circle();
            this.btn_USBload = new GUI_Controls.Button_Circle();
            this.horizontalTabs1 = new GUI_Controls.HorizontalTabs();
            this.lvProtocolList = new GUI_Controls.DragScrollListView3();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.scrollIndicator1 = new GUI_Controls.ScrollIndicator();
            this.btn_RemoveProtocols = new GUI_Controls.Button_Circle();
            this.UserNameHeader = new GUI_Controls.nameHeader();
            this.protocol_SearchBar2 = new GUI_Controls.Protocol_SearchBar2();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_home
            // 
            this.btn_home.Location = new System.Drawing.Point(8, 405);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(96)))), ((int)(((byte)(98)))));
            this.panel2.BackgroundImage = global::GUI_Console.Properties.Resources.protocolHeaderBackground;
            this.panel2.Controls.Add(this.protocol_SearchBar2);
            this.panel2.Location = new System.Drawing.Point(2, 92);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(636, 69);
            this.panel2.TabIndex = 79;
            // 
            // btn_AddProtocols
            // 
            this.btn_AddProtocols.BackColor = System.Drawing.Color.Transparent;
            this.btn_AddProtocols.BackgroundImage = global::GUI_Console.Properties.Resources.PR_BTN08L_add_protocol_STD;
            this.btn_AddProtocols.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_AddProtocols.Check = false;
            this.btn_AddProtocols.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_AddProtocols.ForeColor = System.Drawing.Color.White;
            this.btn_AddProtocols.Location = new System.Drawing.Point(535, 388);
            this.btn_AddProtocols.Name = "btn_AddProtocols";
            this.btn_AddProtocols.Size = new System.Drawing.Size(104, 86);
            this.btn_AddProtocols.TabIndex = 65;
            this.btn_AddProtocols.Text = "  ";
            this.btn_AddProtocols.Visible = false;
            this.btn_AddProtocols.Click += new System.EventHandler(this.btn_AddProtocols_Click);
            // 
            // btn_USBload
            // 
            this.btn_USBload.BackColor = System.Drawing.Color.Transparent;
            this.btn_USBload.BackgroundImage = global::GUI_Console.Properties.Resources.US_BTN04L_usb_add_STD;
            this.btn_USBload.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_USBload.Check = false;
            this.btn_USBload.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_USBload.ForeColor = System.Drawing.Color.White;
            this.btn_USBload.Location = new System.Drawing.Point(105, 388);
            this.btn_USBload.Name = "btn_USBload";
            this.btn_USBload.Size = new System.Drawing.Size(104, 86);
            this.btn_USBload.TabIndex = 77;
            this.btn_USBload.Text = "USB";
            this.btn_USBload.Click += new System.EventHandler(this.btn_USBload_Click);
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
            this.horizontalTabs1.TabIndex = 63;
            this.horizontalTabs1.Tab1_Click += new System.EventHandler(this.horizontalTabs1_Tab1_Click);
            this.horizontalTabs1.Tab2_Click += new System.EventHandler(this.horizontalTabs1_Tab2_Click);
            // 
            // lvProtocolList
            // 
            this.lvProtocolList.BackColor = System.Drawing.Color.White;
            this.lvProtocolList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvProtocolList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvProtocolList.Font = new System.Drawing.Font("Arial Narrow", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvProtocolList.FullRowSelect = true;
            this.lvProtocolList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvProtocolList.HideSelection = false;
            this.lvProtocolList.LabelWrap = false;
            this.lvProtocolList.Location = new System.Drawing.Point(2, 163);
            this.lvProtocolList.MultiSelect = false;
            this.lvProtocolList.Name = "lvProtocolList";
            this.lvProtocolList.OwnerDraw = true;
            this.lvProtocolList.ShowGroups = false;
            this.lvProtocolList.Size = new System.Drawing.Size(615, 208);
            this.lvProtocolList.SmallImageList = this.imageList1;
            this.lvProtocolList.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvProtocolList.TabIndex = 32;
            this.lvProtocolList.UseCompatibleStateImageBehavior = false;
            this.lvProtocolList.View = System.Windows.Forms.View.Details;
            this.lvProtocolList.VisibleRow = 5;
            this.lvProtocolList.VScrollbar = null;
            this.lvProtocolList.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.lvProtocolList_DrawColumnHeader);
            this.lvProtocolList.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.lvProtocolList_DrawItem);
            this.lvProtocolList.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.lvProtocolList_DrawSubItem);
            this.lvProtocolList.SelectedIndexChanged += new System.EventHandler(this.lvProtocolList_MouseLeave);
            this.lvProtocolList.Click += new System.EventHandler(this.lvProtocolList_Click);
            this.lvProtocolList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvProtocolList_KeyDown);
            this.lvProtocolList.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lvProtocolList_KeyUp);
            this.lvProtocolList.MouseLeave += new System.EventHandler(this.lvProtocolList_MouseLeave);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Protocols";
            this.columnHeader1.Width = 493;
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
            this.scrollIndicator1.Location = new System.Drawing.Point(622, 163);
            this.scrollIndicator1.Maximum = 100;
            this.scrollIndicator1.Minimum = 0;
            this.scrollIndicator1.Name = "scrollIndicator1";
            this.scrollIndicator1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.scrollIndicator1.Size = new System.Drawing.Size(18, 208);
            this.scrollIndicator1.SmallChange = 1;
            this.scrollIndicator1.TabIndex = 78;
            this.scrollIndicator1.Text = "scrollIndicator1";
            this.scrollIndicator1.ThumbCustomShape = null;
            this.scrollIndicator1.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(38)))), ((int)(((byte)(131)))));
            this.scrollIndicator1.ThumbOuterColor = System.Drawing.Color.White;
            this.scrollIndicator1.ThumbPenColor = System.Drawing.Color.Silver;
            this.scrollIndicator1.ThumbRoundRectSize = new System.Drawing.Size(8, 8);
            this.scrollIndicator1.ThumbSize = 15;
            this.scrollIndicator1.Value = 50;
            // 
            // btn_RemoveProtocols
            // 
            this.btn_RemoveProtocols.BackColor = System.Drawing.Color.Transparent;
            this.btn_RemoveProtocols.BackgroundImage = global::GUI_Console.Properties.Resources.PR_BTN09L_remove_protocol_STD;
            this.btn_RemoveProtocols.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_RemoveProtocols.Check = false;
            this.btn_RemoveProtocols.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_RemoveProtocols.ForeColor = System.Drawing.Color.White;
            this.btn_RemoveProtocols.Location = new System.Drawing.Point(215, 388);
            this.btn_RemoveProtocols.Name = "btn_RemoveProtocols";
            this.btn_RemoveProtocols.Size = new System.Drawing.Size(104, 86);
            this.btn_RemoveProtocols.TabIndex = 80;
            this.btn_RemoveProtocols.Text = "remove";
            this.btn_RemoveProtocols.Click += new System.EventHandler(this.btn_RemoveProtocols_Click);
            // 
            // UserNameHeader
            // 
            this.UserNameHeader.BackColor = System.Drawing.Color.Transparent;
            this.UserNameHeader.BackgroundImage = global::GUI_Console.Properties.Resources.User_HeaderSml2;
            this.UserNameHeader.Location = new System.Drawing.Point(285, 1);
            this.UserNameHeader.Name = "UserNameHeader";
            this.UserNameHeader.Size = new System.Drawing.Size(353, 37);
            this.UserNameHeader.TabIndex = 81;
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
            // RoboSep_ProtocolList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this.Controls.Add(this.UserNameHeader);
            this.Controls.Add(this.btn_RemoveProtocols);
            this.Controls.Add(this.scrollIndicator1);
            this.Controls.Add(this.btn_USBload);
            this.Controls.Add(this.btn_AddProtocols);
            this.Controls.Add(this.horizontalTabs1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.lvProtocolList);
            this.Name = "RoboSep_ProtocolList";
            this.Load += new System.EventHandler(this.RoboSep_ProtocolList_Load);
            this.Controls.SetChildIndex(this.lvProtocolList, 0);
            this.Controls.SetChildIndex(this.panel2, 0);
            this.Controls.SetChildIndex(this.horizontalTabs1, 0);
            this.Controls.SetChildIndex(this.btn_AddProtocols, 0);
            this.Controls.SetChildIndex(this.btn_USBload, 0);
            this.Controls.SetChildIndex(this.scrollIndicator1, 0);
            this.Controls.SetChildIndex(this.btn_home, 0);
            this.Controls.SetChildIndex(this.btn_RemoveProtocols, 0);
            this.Controls.SetChildIndex(this.UserNameHeader, 0);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ColumnHeader columnHeader1;
        private GUI_Controls.DragScrollListView3 lvProtocolList;
        private GUI_Controls.HorizontalTabs horizontalTabs1;
        private GUI_Controls.Button_Circle btn_AddProtocols;
        private GUI_Controls.Button_Circle btn_USBload;
        private GUI_Controls.ScrollIndicator scrollIndicator1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ImageList imageList1;
        private GUI_Controls.Button_Circle btn_RemoveProtocols;
        private GUI_Controls.nameHeader UserNameHeader;
        private GUI_Controls.Protocol_SearchBar2 protocol_SearchBar2;
    }
}
