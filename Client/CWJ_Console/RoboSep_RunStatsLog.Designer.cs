namespace GUI_Console
{
    partial class RoboSep_Reports
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
            for (i = 0; i < ilist1.Count; i++)
                ilist1[i].Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoboSep_Reports));
            this.button_ViewReport = new GUI_Controls.Button_Rectangle();
            this.button_SaveReports = new GUI_Controls.Button_Rectangle();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.header_devide = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.listView_robostat = new GUI_Controls.DragScrollListView3();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.horizontalTabs1 = new GUI_Controls.HorizontalTabs();
            this.lblRunInfo = new System.Windows.Forms.Label();
            this.lblUser = new System.Windows.Forms.Label();
            this.lblDateTime = new System.Windows.Forms.Label();
            this.scrollBar_Report = new GUI_Controls.ScrollBar();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label_SelectAll = new GUI_Controls.SizableLabel();
            this.checkBox_SelectAll = new GUI_Controls.CheckBox();
            this.UserNameHeader = new GUI_Controls.nameHeader();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_ViewReport
            // 
            this.button_ViewReport.BackColor = System.Drawing.Color.Transparent;
            this.button_ViewReport.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_ViewReport.BackgroundImage")));
            this.button_ViewReport.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_ViewReport.Check = false;
            this.button_ViewReport.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_ViewReport.ForeColor = System.Drawing.Color.White;
            this.button_ViewReport.Location = new System.Drawing.Point(203, 421);
            this.button_ViewReport.Name = "button_ViewReport";
            this.button_ViewReport.Size = new System.Drawing.Size(156, 44);
            this.button_ViewReport.TabIndex = 1;
            this.button_ViewReport.Text = "View Report";
            this.button_ViewReport.Click += new System.EventHandler(this.button_ViewReport_Click);
            // 
            // button_SaveReports
            // 
            this.button_SaveReports.BackColor = System.Drawing.Color.Transparent;
            this.button_SaveReports.BackgroundImage = global::GUI_Console.Properties.Resources.btnLG_STD;
            this.button_SaveReports.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_SaveReports.Check = false;
            this.button_SaveReports.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_SaveReports.ForeColor = System.Drawing.Color.White;
            this.button_SaveReports.Location = new System.Drawing.Point(397, 421);
            this.button_SaveReports.Name = "button_SaveReports";
            this.button_SaveReports.Size = new System.Drawing.Size(188, 44);
            this.button_SaveReports.TabIndex = 2;
            this.button_SaveReports.Text = "Save Reports to USB";
            this.button_SaveReports.Click += new System.EventHandler(this.button_SaveReports_Click);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(1, 26);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // header_devide
            // 
            this.header_devide.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.header_devide.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.header_devide.BorderColor = System.Drawing.Color.Transparent;
            this.header_devide.FillGradientColor = System.Drawing.Color.White;
            this.header_devide.Location = new System.Drawing.Point(23, 82);
            this.header_devide.Name = "header_devide";
            this.header_devide.Size = new System.Drawing.Size(500, 2);
            // 
            // listView_robostat
            // 
            this.listView_robostat.AutoArrange = false;
            this.listView_robostat.BackColor = System.Drawing.SystemColors.Control;
            this.listView_robostat.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView_robostat.CheckBoxes = true;
            this.listView_robostat.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listView_robostat.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listView_robostat.FullRowSelect = true;
            this.listView_robostat.GridLines = true;
            this.listView_robostat.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView_robostat.HideSelection = false;
            this.listView_robostat.Location = new System.Drawing.Point(4, 133);
            this.listView_robostat.Name = "listView_robostat";
            this.listView_robostat.OwnerDraw = true;
            this.listView_robostat.Size = new System.Drawing.Size(589, 244);
            this.listView_robostat.SmallImageList = this.imageList1;
            this.listView_robostat.TabIndex = 33;
            this.listView_robostat.UseCompatibleStateImageBehavior = false;
            this.listView_robostat.View = System.Windows.Forms.View.Details;
            this.listView_robostat.VisibleRow = 6;
            this.listView_robostat.VScrollbar = null;
            this.listView_robostat.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.listView_robostat_DrawColumnHeader);
            this.listView_robostat.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.listView_robostat_DrawItem);
            this.listView_robostat.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.listView_robostat_DrawSubItem);
            this.listView_robostat.SelectedIndexChanged += new System.EventHandler(this.listView_robostat_SelectedIndexChanged);
            this.listView_robostat.Click += new System.EventHandler(this.listView_robostat_Click);
            this.listView_robostat.DoubleClick += new System.EventHandler(this.button_ViewReport_Click);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 168;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Width = 153;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Width = 291;
            // 
            // horizontalTabs1
            // 
            this.horizontalTabs1.BackColor = System.Drawing.Color.Transparent;
            this.horizontalTabs1.Location = new System.Drawing.Point(2, 55);
            this.horizontalTabs1.Name = "horizontalTabs1";
            this.horizontalTabs1.Size = new System.Drawing.Size(636, 38);
            this.horizontalTabs1.Tab1 = "Reports";
            this.horizontalTabs1.Tab2 = null;
            this.horizontalTabs1.Tab3 = null;
            this.horizontalTabs1.TabIndex = 29;
            // 
            // lblRunInfo
            // 
            this.lblRunInfo.AutoSize = true;
            this.lblRunInfo.BackColor = System.Drawing.Color.Transparent;
            this.lblRunInfo.Font = new System.Drawing.Font("Arial Narrow", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRunInfo.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblRunInfo.Location = new System.Drawing.Point(337, 8);
            this.lblRunInfo.Name = "lblRunInfo";
            this.lblRunInfo.Size = new System.Drawing.Size(84, 25);
            this.lblRunInfo.TabIndex = 53;
            this.lblRunInfo.Text = "Run Info";
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.BackColor = System.Drawing.Color.Transparent;
            this.lblUser.Font = new System.Drawing.Font("Arial Narrow", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUser.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblUser.Location = new System.Drawing.Point(196, 8);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(51, 25);
            this.lblUser.TabIndex = 33;
            this.lblUser.Text = "User";
            // 
            // lblDateTime
            // 
            this.lblDateTime.AutoSize = true;
            this.lblDateTime.BackColor = System.Drawing.Color.Transparent;
            this.lblDateTime.Font = new System.Drawing.Font("Arial Narrow", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDateTime.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblDateTime.Location = new System.Drawing.Point(40, 8);
            this.lblDateTime.Name = "lblDateTime";
            this.lblDateTime.Size = new System.Drawing.Size(106, 25);
            this.lblDateTime.TabIndex = 32;
            this.lblDateTime.Text = "Date / Time";
            // 
            // scrollBar_Report
            // 
            this.scrollBar_Report.ActiveBackColor = System.Drawing.Color.Gray;
            this.scrollBar_Report.LargeChange = 10;
            this.scrollBar_Report.Location = new System.Drawing.Point(597, 133);
            this.scrollBar_Report.Maximum = 99;
            this.scrollBar_Report.Minimum = 0;
            this.scrollBar_Report.Name = "scrollBar_Report";
            this.scrollBar_Report.Size = new System.Drawing.Size(39, 244);
            this.scrollBar_Report.SmallChange = 1;
            this.scrollBar_Report.TabIndex = 55;
            this.scrollBar_Report.Text = "scrollBar1";
            this.scrollBar_Report.ThumbStyle = GUI_Controls.ScrollBar.EnumThumbStyle.Auto;
            this.scrollBar_Report.Value = 0;
            // 
            // panel2
            // 
            this.panel2.BackgroundImage = global::GUI_Console.Properties.Resources.home_header640x40_bg;
            this.panel2.Controls.Add(this.label_SelectAll);
            this.panel2.Controls.Add(this.checkBox_SelectAll);
            this.panel2.Controls.Add(this.lblRunInfo);
            this.panel2.Controls.Add(this.lblDateTime);
            this.panel2.Controls.Add(this.lblUser);
            this.panel2.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel2.ForeColor = System.Drawing.Color.Black;
            this.panel2.Location = new System.Drawing.Point(2, 92);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(636, 40);
            this.panel2.TabIndex = 58;
            // 
            // label_SelectAll
            // 
            this.label_SelectAll.AutoSize = true;
            this.label_SelectAll.BackColor = System.Drawing.Color.Transparent;
            this.label_SelectAll.Font = new System.Drawing.Font("Arial Narrow", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_SelectAll.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(38)))), ((int)(((byte)(131)))));
            this.label_SelectAll.Location = new System.Drawing.Point(573, 8);
            this.label_SelectAll.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.label_SelectAll.Name = "label_SelectAll";
            this.label_SelectAll.Size = new System.Drawing.Size(50, 25);
            this.label_SelectAll.TabIndex = 57;
            this.label_SelectAll.Text = "All";
            // 
            // checkBox_SelectAll
            // 
            this.checkBox_SelectAll.AutoSize = true;
            this.checkBox_SelectAll.BackColor = System.Drawing.Color.Transparent;
            this.checkBox_SelectAll.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("checkBox_SelectAll.BackgroundImage")));
            this.checkBox_SelectAll.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.checkBox_SelectAll.Check = false;
            this.checkBox_SelectAll.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_SelectAll.ForeColor = System.Drawing.Color.White;
            this.checkBox_SelectAll.Location = new System.Drawing.Point(543, 11);
            this.checkBox_SelectAll.Name = "checkBox_SelectAll";
            this.checkBox_SelectAll.Size = new System.Drawing.Size(27, 21);
            this.checkBox_SelectAll.TabIndex = 56;
            this.checkBox_SelectAll.Text = "  ";
            this.checkBox_SelectAll.Click += new System.EventHandler(this.checkBox_SelectAll_Click);
            // 
            // UserNameHeader
            // 
            this.UserNameHeader.BackColor = System.Drawing.Color.Transparent;
            this.UserNameHeader.BackgroundImage = global::GUI_Console.Properties.Resources.User_HeaderSml2;
            this.UserNameHeader.Location = new System.Drawing.Point(285, 1);
            this.UserNameHeader.Name = "UserNameHeader";
            this.UserNameHeader.Size = new System.Drawing.Size(353, 37);
            this.UserNameHeader.TabIndex = 59;
            // 
            // RoboSep_Reports
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this.Controls.Add(this.UserNameHeader);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.scrollBar_Report);
            this.Controls.Add(this.listView_robostat);
            this.Controls.Add(this.horizontalTabs1);
            this.Controls.Add(this.button_ViewReport);
            this.Controls.Add(this.button_SaveReports);
            this.Name = "RoboSep_Reports";
            this.Controls.SetChildIndex(this.btn_home, 0);
            this.Controls.SetChildIndex(this.button_SaveReports, 0);
            this.Controls.SetChildIndex(this.button_ViewReport, 0);
            this.Controls.SetChildIndex(this.horizontalTabs1, 0);
            this.Controls.SetChildIndex(this.listView_robostat, 0);
            this.Controls.SetChildIndex(this.scrollBar_Report, 0);
            this.Controls.SetChildIndex(this.panel2, 0);
            this.Controls.SetChildIndex(this.UserNameHeader, 0);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private GUI_Controls.Button_Rectangle button_ViewReport;
        private GUI_Controls.Button_Rectangle button_SaveReports;
        private System.Windows.Forms.ImageList imageList1;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape header_devide;
        private /*System.Windows.Forms.ListView*/ GUI_Controls.DragScrollListView3 listView_robostat;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private GUI_Controls.HorizontalTabs horizontalTabs1;
        private System.Windows.Forms.Label lblDateTime;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Label lblRunInfo;
        private GUI_Controls.ScrollBar scrollBar_Report;
        private System.Windows.Forms.Panel panel2;
        private GUI_Controls.CheckBox checkBox_SelectAll;
        private GUI_Controls.SizableLabel label_SelectAll;
        private GUI_Controls.nameHeader UserNameHeader;
    }
}
