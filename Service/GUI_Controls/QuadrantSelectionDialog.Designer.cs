namespace GUI_Controls
{
    partial class QuadrantSelectionDialog
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
            this.Rect1 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.Rect2 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.Rect3 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.Rect4 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.txt_MessageBox = new System.Windows.Forms.RichTextBox();
            this.label_WindowTitle = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.Corner_BR = new System.Windows.Forms.PictureBox();
            this.Corner_BL = new System.Windows.Forms.PictureBox();
            this.Corner_TR = new System.Windows.Forms.PictureBox();
            this.Corner_TL = new System.Windows.Forms.PictureBox();
            this.button_Q3 = new GUI_Controls.GUIButton();
            this.button_Q4 = new GUI_Controls.GUIButton();
            this.button_Q2 = new GUI_Controls.GUIButton();
            this.button_Q1 = new GUI_Controls.GUIButton();
            this.roboPanel1 = new GUI_Controls.RoboPanel();
            ((System.ComponentModel.ISupportInitialize)(this.Corner_BR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Corner_BL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Corner_TR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Corner_TL)).BeginInit();
            this.SuspendLayout();
            // 
            // Rect1
            // 
            this.Rect1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(36)))), ((int)(((byte)(118)))));
            this.Rect1.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.Rect1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(36)))), ((int)(((byte)(118)))));
            this.Rect1.Location = new System.Drawing.Point(19, 0);
            this.Rect1.Name = "Rect1";
            this.Rect1.Size = new System.Drawing.Size(259, 26);
            // 
            // shapeContainer1
            // 
            this.shapeContainer1.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer1.Name = "shapeContainer1";
            this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.Rect2,
            this.Rect3,
            this.Rect4,
            this.Rect1});
            this.shapeContainer1.Size = new System.Drawing.Size(300, 370);
            this.shapeContainer1.TabIndex = 17;
            this.shapeContainer1.TabStop = false;
            // 
            // Rect2
            // 
            this.Rect2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(36)))), ((int)(((byte)(118)))));
            this.Rect2.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.Rect2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(36)))), ((int)(((byte)(118)))));
            this.Rect2.Location = new System.Drawing.Point(19, 363);
            this.Rect2.Name = "Rect2";
            this.Rect2.Size = new System.Drawing.Size(260, 6);
            // 
            // Rect3
            // 
            this.Rect3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(36)))), ((int)(((byte)(118)))));
            this.Rect3.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.Rect3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(36)))), ((int)(((byte)(118)))));
            this.Rect3.Location = new System.Drawing.Point(0, 35);
            this.Rect3.Name = "Rect3";
            this.Rect3.Size = new System.Drawing.Size(5, 315);
            // 
            // Rect4
            // 
            this.Rect4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(36)))), ((int)(((byte)(118)))));
            this.Rect4.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.Rect4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(36)))), ((int)(((byte)(118)))));
            this.Rect4.Location = new System.Drawing.Point(292, 34);
            this.Rect4.Name = "Rect4";
            this.Rect4.Size = new System.Drawing.Size(5, 316);
            // 
            // txt_MessageBox
            // 
            this.txt_MessageBox.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.txt_MessageBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.txt_MessageBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txt_MessageBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.txt_MessageBox.Font = new System.Drawing.Font("Arial Narrow", 13F);
            this.txt_MessageBox.Location = new System.Drawing.Point(30, 39);
            this.txt_MessageBox.Name = "txt_MessageBox";
            this.txt_MessageBox.ReadOnly = true;
            this.txt_MessageBox.Size = new System.Drawing.Size(244, 75);
            this.txt_MessageBox.TabIndex = 18;
            this.txt_MessageBox.Text = "To select which quadrants are to use the same reagent kit, ensure that all applic" +
                "able quadrants are highlighted.";
            // 
            // label_WindowTitle
            // 
            this.label_WindowTitle.AutoSize = true;
            this.label_WindowTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(36)))), ((int)(((byte)(118)))));
            this.label_WindowTitle.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 15F, System.Drawing.FontStyle.Bold);
            this.label_WindowTitle.ForeColor = System.Drawing.Color.White;
            this.label_WindowTitle.Location = new System.Drawing.Point(26, -2);
            this.label_WindowTitle.Name = "label_WindowTitle";
            this.label_WindowTitle.Size = new System.Drawing.Size(166, 24);
            this.label_WindowTitle.TabIndex = 19;
            this.label_WindowTitle.Text = "Quadrant Reagent Sharing";
            this.label_WindowTitle.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // button2
            // 
            this.button2.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 15F, System.Drawing.FontStyle.Bold);
            this.button2.ForeColor = System.Drawing.SystemColors.Window;
            this.button2.Image = global::GUI_Controls.Properties.Resources.BUTTON_PROMPTBUTTON0;
            this.button2.Location = new System.Drawing.Point(18, 321);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(86, 32);
            this.button2.TabIndex = 16;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Frutiger LT Std 55 Roman", 15F, System.Drawing.FontStyle.Bold);
            this.button1.ForeColor = System.Drawing.SystemColors.Window;
            this.button1.Image = global::GUI_Controls.Properties.Resources.BUTTON_PROMPTBUTTON0;
            this.button1.Location = new System.Drawing.Point(192, 321);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(86, 32);
            this.button1.TabIndex = 15;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Corner_BR
            // 
            this.Corner_BR.Image = global::GUI_Controls.Properties.Resources.RoboMessagePanel_BR;
            this.Corner_BR.Location = new System.Drawing.Point(279, 351);
            this.Corner_BR.Name = "Corner_BR";
            this.Corner_BR.Size = new System.Drawing.Size(19, 19);
            this.Corner_BR.TabIndex = 14;
            this.Corner_BR.TabStop = false;
            // 
            // Corner_BL
            // 
            this.Corner_BL.Image = global::GUI_Controls.Properties.Resources.RoboMessagePanel_BL;
            this.Corner_BL.Location = new System.Drawing.Point(0, 351);
            this.Corner_BL.Name = "Corner_BL";
            this.Corner_BL.Size = new System.Drawing.Size(19, 19);
            this.Corner_BL.TabIndex = 13;
            this.Corner_BL.TabStop = false;
            // 
            // Corner_TR
            // 
            this.Corner_TR.Image = global::GUI_Controls.Properties.Resources.RoboMessagePanel_TR;
            this.Corner_TR.Location = new System.Drawing.Point(279, 0);
            this.Corner_TR.Name = "Corner_TR";
            this.Corner_TR.Size = new System.Drawing.Size(19, 35);
            this.Corner_TR.TabIndex = 12;
            this.Corner_TR.TabStop = false;
            // 
            // Corner_TL
            // 
            this.Corner_TL.Image = global::GUI_Controls.Properties.Resources.RoboMessagePanel_TL;
            this.Corner_TL.Location = new System.Drawing.Point(0, 0);
            this.Corner_TL.Name = "Corner_TL";
            this.Corner_TL.Size = new System.Drawing.Size(19, 35);
            this.Corner_TL.TabIndex = 11;
            this.Corner_TL.TabStop = false;
            // 
            // button_Q3
            // 
            this.button_Q3.AccessibleName = "  ";
            this.button_Q3.BackgroundImage = global::GUI_Controls.Properties.Resources.Qselect30;
            this.button_Q3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Q3.Check = false;
            this.button_Q3.Location = new System.Drawing.Point(150, 222);
            this.button_Q3.Name = "button_Q3";
            this.button_Q3.Size = new System.Drawing.Size(83, 83);
            this.button_Q3.TabIndex = 23;
            this.button_Q3.Click += new System.EventHandler(this.button_Q3_Click);
            // 
            // button_Q4
            // 
            this.button_Q4.AccessibleName = "  ";
            this.button_Q4.BackgroundImage = global::GUI_Controls.Properties.Resources.Qselect40;
            this.button_Q4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Q4.Check = false;
            this.button_Q4.Location = new System.Drawing.Point(75, 222);
            this.button_Q4.Name = "button_Q4";
            this.button_Q4.Size = new System.Drawing.Size(75, 83);
            this.button_Q4.TabIndex = 22;
            this.button_Q4.Click += new System.EventHandler(this.button_Q4_Click);
            // 
            // button_Q2
            // 
            this.button_Q2.AccessibleName = "  ";
            this.button_Q2.BackgroundImage = global::GUI_Controls.Properties.Resources.Qselect20;
            this.button_Q2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Q2.Check = false;
            this.button_Q2.Location = new System.Drawing.Point(150, 147);
            this.button_Q2.Name = "button_Q2";
            this.button_Q2.Size = new System.Drawing.Size(83, 75);
            this.button_Q2.TabIndex = 21;
            this.button_Q2.Click += new System.EventHandler(this.button_Q2_Click);
            // 
            // button_Q1
            // 
            this.button_Q1.AccessibleName = "  ";
            this.button_Q1.BackgroundImage = global::GUI_Controls.Properties.Resources.Qselect10;
            this.button_Q1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Q1.Check = false;
            this.button_Q1.Location = new System.Drawing.Point(75, 147);
            this.button_Q1.Name = "button_Q1";
            this.button_Q1.Size = new System.Drawing.Size(75, 75);
            this.button_Q1.TabIndex = 20;
            this.button_Q1.Click += new System.EventHandler(this.button_Q1_Click);
            // 
            // roboPanel1
            // 
            this.roboPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.roboPanel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.roboPanel1.Location = new System.Drawing.Point(0, 0);
            this.roboPanel1.Name = "roboPanel1";
            this.roboPanel1.Size = new System.Drawing.Size(297, 370);
            this.roboPanel1.TabIndex = 24;
            this.roboPanel1.Visible = false;
            // 
            // QuadrantSelectionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 370);
            this.Controls.Add(this.button_Q3);
            this.Controls.Add(this.button_Q4);
            this.Controls.Add(this.button_Q2);
            this.Controls.Add(this.button_Q1);
            this.Controls.Add(this.label_WindowTitle);
            this.Controls.Add(this.txt_MessageBox);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.Corner_BR);
            this.Controls.Add(this.Corner_BL);
            this.Controls.Add(this.Corner_TR);
            this.Controls.Add(this.Corner_TL);
            this.Controls.Add(this.roboPanel1);
            this.Controls.Add(this.shapeContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "QuadrantSelectionDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Load += new System.EventHandler(this.QuadrantSelectionDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Corner_BR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Corner_BL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Corner_TR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Corner_TL)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox Corner_BR;
        private System.Windows.Forms.PictureBox Corner_BL;
        private System.Windows.Forms.PictureBox Corner_TR;
        private System.Windows.Forms.PictureBox Corner_TL;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape Rect1;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape Rect3;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape Rect4;
        private System.Windows.Forms.RichTextBox txt_MessageBox;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape Rect2;
        private System.Windows.Forms.Label label_WindowTitle;
        private GUIButton button_Q1;
        private GUIButton button_Q2;
        private GUIButton button_Q3;
        private GUIButton button_Q4;
        private RoboPanel roboPanel1;

    }
}
