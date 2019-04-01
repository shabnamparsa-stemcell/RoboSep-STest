namespace GUI_Console
{
    partial class Form_BarcodeScan
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
            for (i = 0; i < headerImageList.Count; i++)
                    headerImageList[i].Dispose();
            for (i = 0; i < ilistGroupEmpty.Count; i++)
                ilistGroupEmpty[i].Dispose();
            for (i = 0; i < ilistGroupExpanded.Count; i++)
                ilistGroupExpanded[i].Dispose();
            for (i = 0; i < ilistGroupCollapsed.Count; i++)
                ilistGroupCollapsed[i].Dispose();
            for (i = 0; i < ilistSaveNormal.Count; i++)
                ilistSaveNormal[i].Dispose();
            for (i = 0; i < ilistSaveDirty.Count; i++)
                ilistSaveDirty[i].Dispose();
            for (i = 0; i < ilistEmbed.Count; i++)
                ilistEmbed[i].Dispose();
            for (i = 0; i < iListFlashTextBox.Count; i++)
            {
                iListFlashTextBox[i].Font.Dispose();
                iListFlashTextBox[i].Font = null;
                iListFlashTextBox[i].Dispose();
            }

            TextBoxFont.Dispose();
            TextBoxFont = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_BarcodeScan));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.button_AllGroups = new GUI_Controls.GUIButton();
            this.button_ScanAllQuadrants = new GUI_Controls.GUIButton();
            this.lblScanQuadrants = new System.Windows.Forms.Label();
            this.button_Abort = new GUI_Controls.Button_Circle();
            this.scrollBar1 = new GUI_Controls.ScrollBar();
            this.barcodeScrollPanel = new GUI_Controls.ScrollPanel();
            this.button_Cancel = new GUI_Controls.Button_Cancel();
            this.button_Save = new GUI_Controls.Button_Circle();
            this.UserNameHeader = new GUI_Controls.nameHeader();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(13, 13);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // panel2
            // 
            this.panel2.BackgroundImage = global::GUI_Console.Properties.Resources.home_header640x40_bg;
            this.panel2.Controls.Add(this.button_AllGroups);
            this.panel2.Controls.Add(this.button_ScanAllQuadrants);
            this.panel2.Controls.Add(this.lblScanQuadrants);
            this.panel2.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel2.ForeColor = System.Drawing.Color.Black;
            this.panel2.Location = new System.Drawing.Point(2, 57);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(562, 40);
            this.panel2.TabIndex = 82;
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
            // button_ScanAllQuadrants
            // 
            this.button_ScanAllQuadrants.BackColor = System.Drawing.Color.Transparent;
            this.button_ScanAllQuadrants.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_ScanAllQuadrants.BackgroundImage")));
            this.button_ScanAllQuadrants.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_ScanAllQuadrants.Check = false;
            this.button_ScanAllQuadrants.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_ScanAllQuadrants.ForeColor = System.Drawing.Color.White;
            this.button_ScanAllQuadrants.Location = new System.Drawing.Point(516, 2);
            this.button_ScanAllQuadrants.Name = "button_ScanAllQuadrants";
            this.button_ScanAllQuadrants.Size = new System.Drawing.Size(40, 40);
            this.button_ScanAllQuadrants.TabIndex = 33;
            this.button_ScanAllQuadrants.Text = "  ";
            this.button_ScanAllQuadrants.Click += new System.EventHandler(this.button_ScanAllQuadrants_Click);
            // 
            // lblScanQuadrants
            // 
            this.lblScanQuadrants.BackColor = System.Drawing.Color.Transparent;
            this.lblScanQuadrants.Font = new System.Drawing.Font("Arial Narrow", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblScanQuadrants.ForeColor = System.Drawing.Color.DarkGray;
            this.lblScanQuadrants.Location = new System.Drawing.Point(51, 9);
            this.lblScanQuadrants.Name = "lblScanQuadrants";
            this.lblScanQuadrants.Size = new System.Drawing.Size(143, 25);
            this.lblScanQuadrants.TabIndex = 32;
            this.lblScanQuadrants.Text = "All Quadrants";
            this.lblScanQuadrants.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button_Abort
            // 
            this.button_Abort.BackColor = System.Drawing.Color.Transparent;
            this.button_Abort.BackgroundImage = global::GUI_Console.Properties.Resources.RN_BTN02L_abort_104x86_STD;
            this.button_Abort.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Abort.Check = false;
            this.button_Abort.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Abort.ForeColor = System.Drawing.Color.White;
            this.button_Abort.Location = new System.Drawing.Point(305, 391);
            this.button_Abort.Name = "button_Abort";
            this.button_Abort.Size = new System.Drawing.Size(104, 86);
            this.button_Abort.TabIndex = 83;
            this.button_Abort.Text = "button_Circle1";
            this.button_Abort.Visible = false;
            this.button_Abort.Click += new System.EventHandler(this.button_Abort_Click);
            // 
            // scrollBar1
            // 
            this.scrollBar1.ActiveBackColor = System.Drawing.Color.Gray;
            this.scrollBar1.LargeChange = 10;
            this.scrollBar1.Location = new System.Drawing.Point(568, 98);
            this.scrollBar1.Maximum = 99;
            this.scrollBar1.Minimum = 0;
            this.scrollBar1.Name = "scrollBar1";
            this.scrollBar1.Size = new System.Drawing.Size(67, 271);
            this.scrollBar1.SmallChange = 1;
            this.scrollBar1.TabIndex = 81;
            this.scrollBar1.Text = "scrollBar1";
            this.scrollBar1.ThumbStyle = GUI_Controls.ScrollBar.EnumThumbStyle.Auto;
            this.scrollBar1.Value = 0;
            // 
            // barcodeScrollPanel
            // 
            this.barcodeScrollPanel.AutoScroll = true;
            this.barcodeScrollPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.barcodeScrollPanel.Location = new System.Drawing.Point(2, 98);
            this.barcodeScrollPanel.Name = "barcodeScrollPanel";
            this.barcodeScrollPanel.Padding = new System.Windows.Forms.Padding(0, 0, 17, 0);
            this.barcodeScrollPanel.SingleItemOnlyExpansion = false;
            this.barcodeScrollPanel.Size = new System.Drawing.Size(562, 270);
            this.barcodeScrollPanel.TabIndex = 80;
            this.barcodeScrollPanel.VScrollbar = null;
            this.barcodeScrollPanel.WrapContents = false;
            // 
            // button_Cancel
            // 
            this.button_Cancel.BackColor = System.Drawing.Color.Transparent;
            this.button_Cancel.BackgroundImage = global::GUI_Console.Properties.Resources.L_104x86_single_arrow_left_STD;
            this.button_Cancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Cancel.Check = false;
            this.button_Cancel.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Cancel.ForeColor = System.Drawing.Color.White;
            this.button_Cancel.Location = new System.Drawing.Point(415, 391);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(104, 86);
            this.button_Cancel.TabIndex = 76;
            this.button_Cancel.Text = "Cancel";
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // button_Save
            // 
            this.button_Save.BackColor = System.Drawing.Color.Transparent;
            this.button_Save.BackgroundImage = global::GUI_Console.Properties.Resources.GE_BTN10L_save_STD;
            this.button_Save.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Save.Check = false;
            this.button_Save.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Save.ForeColor = System.Drawing.Color.White;
            this.button_Save.Location = new System.Drawing.Point(526, 391);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(104, 86);
            this.button_Save.TabIndex = 75;
            this.button_Save.Text = "Save";
            this.button_Save.Click += new System.EventHandler(this.button_Save_Click);
            // 
            // UserNameHeader
            // 
            this.UserNameHeader.BackColor = System.Drawing.Color.Transparent;
            this.UserNameHeader.BackgroundImage = global::GUI_Console.Properties.Resources.User_HeaderSml2;
            this.UserNameHeader.Location = new System.Drawing.Point(285, 1);
            this.UserNameHeader.Name = "UserNameHeader";
            this.UserNameHeader.Size = new System.Drawing.Size(353, 37);
            this.UserNameHeader.TabIndex = 84;
            // 
            // Form_BarcodeScan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this.ClientSize = new System.Drawing.Size(640, 480);
            this.Controls.Add(this.UserNameHeader);
            this.Controls.Add(this.button_Abort);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.scrollBar1);
            this.Controls.Add(this.barcodeScrollPanel);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_Save);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form_BarcodeScan";
            this.Load += new System.EventHandler(this.Form_BarcodeScan_Load);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private GUI_Controls.Button_Cancel button_Cancel;
        private GUI_Controls.Button_Circle button_Save;
        private GUI_Controls.ScrollPanel barcodeScrollPanel;
        private GUI_Controls.ScrollBar scrollBar1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblScanQuadrants;
        private GUI_Controls.GUIButton button_ScanAllQuadrants;
        private GUI_Controls.GUIButton button_AllGroups;
        private GUI_Controls.Button_Circle button_Abort;
        private GUI_Controls.nameHeader UserNameHeader;

    }
}
