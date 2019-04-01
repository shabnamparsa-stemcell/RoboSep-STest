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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QuadrantSelectionDialog));
            this.txt_MessageBox = new System.Windows.Forms.Label();
            this.button_Q3 = new GUI_Controls.GUIButton();
            this.button_Q4 = new GUI_Controls.GUIButton();
            this.button_Q2 = new GUI_Controls.GUIButton();
            this.button_Q1 = new GUI_Controls.GUIButton();
            this.button2 = new GUI_Controls.Button_Rectangle();
            this.button1 = new GUI_Controls.Button_Rectangle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label_WindowsTitle = new System.Windows.Forms.Label();
            this.rectangleShape1 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txt_MessageBox
            // 
            this.txt_MessageBox.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.txt_MessageBox.BackColor = System.Drawing.Color.Transparent;
            this.txt_MessageBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.txt_MessageBox.Font = new System.Drawing.Font("Arial Narrow", 13F);
            this.txt_MessageBox.Location = new System.Drawing.Point(11, 44);
            this.txt_MessageBox.Name = "txt_MessageBox";
            this.txt_MessageBox.Size = new System.Drawing.Size(244, 75);
            this.txt_MessageBox.TabIndex = 18;
            this.txt_MessageBox.Text = "To select which quadrants are to use the same reagent kit, ensure that all applic" +
                "able quadrants are highlighted.";
            // 
            // button_Q3
            // 
            this.button_Q3.BackColor = System.Drawing.Color.Transparent;
            this.button_Q3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Q3.BackgroundImage")));
            this.button_Q3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Q3.Check = false;
            this.button_Q3.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Q3.ForeColor = System.Drawing.Color.White;
            this.button_Q3.Location = new System.Drawing.Point(42, 234);
            this.button_Q3.Name = "button_Q3";
            this.button_Q3.Size = new System.Drawing.Size(188, 44);
            this.button_Q3.TabIndex = 23;
            this.button_Q3.Text = "Quadrant 3";
            this.button_Q3.Click += new System.EventHandler(this.button_Q3_Click);
            // 
            // button_Q4
            // 
            this.button_Q4.BackColor = System.Drawing.Color.Transparent;
            this.button_Q4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Q4.BackgroundImage")));
            this.button_Q4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Q4.Check = false;
            this.button_Q4.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Q4.ForeColor = System.Drawing.Color.White;
            this.button_Q4.Location = new System.Drawing.Point(42, 287);
            this.button_Q4.Name = "button_Q4";
            this.button_Q4.Size = new System.Drawing.Size(188, 44);
            this.button_Q4.TabIndex = 22;
            this.button_Q4.Text = "Quadrant 4";
            this.button_Q4.Click += new System.EventHandler(this.button_Q4_Click);
            // 
            // button_Q2
            // 
            this.button_Q2.BackColor = System.Drawing.Color.Transparent;
            this.button_Q2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Q2.BackgroundImage")));
            this.button_Q2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Q2.Check = false;
            this.button_Q2.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Q2.ForeColor = System.Drawing.Color.White;
            this.button_Q2.Location = new System.Drawing.Point(42, 181);
            this.button_Q2.Name = "button_Q2";
            this.button_Q2.Size = new System.Drawing.Size(188, 44);
            this.button_Q2.TabIndex = 21;
            this.button_Q2.Text = "Quadrant 2";
            this.button_Q2.Click += new System.EventHandler(this.button_Q2_Click);
            // 
            // button_Q1
            // 
            this.button_Q1.BackColor = System.Drawing.Color.Transparent;
            this.button_Q1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Q1.BackgroundImage")));
            this.button_Q1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Q1.Check = false;
            this.button_Q1.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Q1.ForeColor = System.Drawing.Color.White;
            this.button_Q1.Location = new System.Drawing.Point(42, 128);
            this.button_Q1.Name = "button_Q1";
            this.button_Q1.Size = new System.Drawing.Size(188, 44);
            this.button_Q1.TabIndex = 20;
            this.button_Q1.Text = "Quadrant 1";
            this.button_Q1.Click += new System.EventHandler(this.button_Q1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.BackColor = System.Drawing.Color.Transparent;
            this.button2.BackgroundImage = global::GUI_Controls.Properties.Resources.btnsmall_STD;
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button2.Check = false;
            this.button2.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Location = new System.Drawing.Point(98, 355);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(74, 35);
            this.button2.TabIndex = 30;
            this.button2.Text = "Cancel";
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.BackColor = System.Drawing.Color.Transparent;
            this.button1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button1.BackgroundImage")));
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button1.Check = false;
            this.button1.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(188, 355);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(74, 35);
            this.button1.TabIndex = 31;
            this.button1.Text = "OK";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.Controls.Add(this.label_WindowsTitle);
            this.panel1.Location = new System.Drawing.Point(1, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(290, 37);
            this.panel1.TabIndex = 32;
            // 
            // label_WindowsTitle
            // 
            this.label_WindowsTitle.BackColor = System.Drawing.Color.Transparent;
            this.label_WindowsTitle.Font = new System.Drawing.Font("Arial", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_WindowsTitle.ForeColor = System.Drawing.Color.White;
            this.label_WindowsTitle.Location = new System.Drawing.Point(6, 5);
            this.label_WindowsTitle.Margin = new System.Windows.Forms.Padding(0);
            this.label_WindowsTitle.Name = "label_WindowsTitle";
            this.label_WindowsTitle.Size = new System.Drawing.Size(188, 27);
            this.label_WindowsTitle.TabIndex = 3;
            this.label_WindowsTitle.Text = "Resource Sharing";
            this.label_WindowsTitle.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // rectangleShape1
            // 
            this.rectangleShape1.BorderWidth = 2;
            this.rectangleShape1.Location = new System.Drawing.Point(0, 0);
            this.rectangleShape1.Name = "rectangleShape1";
            this.rectangleShape1.Size = new System.Drawing.Size(271, 399);
            // 
            // shapeContainer1
            // 
            this.shapeContainer1.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer1.Name = "shapeContainer1";
            this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.rectangleShape1});
            this.shapeContainer1.Size = new System.Drawing.Size(272, 400);
            this.shapeContainer1.TabIndex = 33;
            this.shapeContainer1.TabStop = false;
            // 
            // QuadrantSelectionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(272, 400);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button_Q3);
            this.Controls.Add(this.button_Q4);
            this.Controls.Add(this.button_Q2);
            this.Controls.Add(this.button_Q1);
            this.Controls.Add(this.txt_MessageBox);
            this.Controls.Add(this.shapeContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "QuadrantSelectionDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Load += new System.EventHandler(this.QuadrantSelectionDialog_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label txt_MessageBox;
        private GUIButton button_Q1;
        private GUIButton button_Q2;
        private GUIButton button_Q3;
        private GUIButton button_Q4;
        protected Button_Rectangle button2;
        protected Button_Rectangle button1;
        private System.Windows.Forms.Panel panel1;
        protected System.Windows.Forms.Label label_WindowsTitle;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape1;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;

    }
}
