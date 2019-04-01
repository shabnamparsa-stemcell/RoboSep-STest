namespace GUI_Console
{
    partial class RoboSep_ScanSelection
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
            for (i = 0; i < IList.Count; i++)
                IList[i].Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoboSep_ScanSelection));
            this.roboPanel_CarouselScan = new GUI_Controls.RoboPanel();
            this.button_Cancel = new GUI_Controls.Button_Circle();
            this.button_Keyboard2 = new GUI_Controls.Button_Keyboard();
            this.button_Keyboard1 = new GUI_Controls.Button_Keyboard();
            this.panel2 = new System.Windows.Forms.Panel();
            this.listView_Protocols = new GUI_Controls.DragScrollListView();
            this.shapeContainer2 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.rectangleShape2 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBox_BarCode = new System.Windows.Forms.TextBox();
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.rectangleShape1 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.label1 = new System.Windows.Forms.Label();
            this.roboPanel_CarouselScan.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // roboPanel_CarouselScan
            // 
            this.roboPanel_CarouselScan.Text = "Carousel Scan Quadrant X";
            this.roboPanel_CarouselScan.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.roboPanel_CarouselScan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.roboPanel_CarouselScan.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.roboPanel_CarouselScan.Controls.Add(this.button_Cancel);
            this.roboPanel_CarouselScan.Controls.Add(this.button_Keyboard2);
            this.roboPanel_CarouselScan.Controls.Add(this.button_Keyboard1);
            this.roboPanel_CarouselScan.Controls.Add(this.panel2);
            this.roboPanel_CarouselScan.Controls.Add(this.label2);
            this.roboPanel_CarouselScan.Controls.Add(this.panel1);
            this.roboPanel_CarouselScan.Controls.Add(this.label1);
            this.roboPanel_CarouselScan.Location = new System.Drawing.Point(0, 0);
            this.roboPanel_CarouselScan.Name = "roboPanel_CarouselScan";
            this.roboPanel_CarouselScan.Size = new System.Drawing.Size(450, 470);
            this.roboPanel_CarouselScan.TabIndex = 1;
            // 
            // button_Cancel
            // 
            this.button_Cancel.Text = " ";
            this.button_Cancel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Cancel.BackgroundImage")));
            this.button_Cancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button_Cancel.Check = false;
            this.button_Cancel.Location = new System.Drawing.Point(386, 12);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(52, 50);
            this.button_Cancel.TabIndex = 25;
            // 
            // button_Keyboard2
            // 
            this.button_Keyboard2.Text = "Manual Selection";
            this.button_Keyboard2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Keyboard2.BackgroundImage")));
            this.button_Keyboard2.check = false;
            this.button_Keyboard2.Location = new System.Drawing.Point(158, 408);
            this.button_Keyboard2.Name = "button_Keyboard2";
            this.button_Keyboard2.Size = new System.Drawing.Size(135, 50);
            this.button_Keyboard2.TabIndex = 24;
            // 
            // button_Keyboard1
            // 
            this.button_Keyboard1.Text = "Accept";
            this.button_Keyboard1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Keyboard1.BackgroundImage")));
            this.button_Keyboard1.check = false;
            this.button_Keyboard1.Location = new System.Drawing.Point(303, 409);
            this.button_Keyboard1.Name = "button_Keyboard1";
            this.button_Keyboard1.Size = new System.Drawing.Size(135, 50);
            this.button_Keyboard1.TabIndex = 5;
            this.button_Keyboard1.Click += new System.EventHandler(this.button_Keyboard1_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.listView_Protocols);
            this.panel2.Controls.Add(this.shapeContainer2);
            this.panel2.Location = new System.Drawing.Point(12, 166);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(426, 240);
            this.panel2.TabIndex = 23;
            // 
            // listView_Protocols
            // 
            this.listView_Protocols.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listView_Protocols.Location = new System.Drawing.Point(8, 3);
            this.listView_Protocols.MultiSelect = false;
            this.listView_Protocols.Name = "listView_Protocols";
            this.listView_Protocols.Size = new System.Drawing.Size(410, 226);
            this.listView_Protocols.TabIndex = 4;
            this.listView_Protocols.UseCompatibleStateImageBehavior = false;
            this.listView_Protocols.View = System.Windows.Forms.View.List;
            this.listView_Protocols.DoubleClick += new System.EventHandler(this.listView_Protocols_DoubleClick);
            // 
            // shapeContainer2
            // 
            this.shapeContainer2.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer2.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer2.Name = "shapeContainer2";
            this.shapeContainer2.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.rectangleShape2});
            this.shapeContainer2.Size = new System.Drawing.Size(426, 240);
            this.shapeContainer2.TabIndex = 3;
            this.shapeContainer2.TabStop = false;
            // 
            // rectangleShape2
            // 
            this.rectangleShape2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.rectangleShape2.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.rectangleShape2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.rectangleShape2.Location = new System.Drawing.Point(12, 7);
            this.rectangleShape2.Name = "rectangleShape1";
            this.rectangleShape2.Size = new System.Drawing.Size(408, 225);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Agency FB", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 139);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(122, 24);
            this.label2.TabIndex = 22;
            this.label2.Text = "Possible Protocols:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textBox_BarCode);
            this.panel1.Controls.Add(this.shapeContainer1);
            this.panel1.Location = new System.Drawing.Point(19, 82);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(242, 49);
            this.panel1.TabIndex = 21;
            // 
            // textBox_BarCode
            // 
            this.textBox_BarCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_BarCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.textBox_BarCode.Location = new System.Drawing.Point(4, 7);
            this.textBox_BarCode.Name = "textBox_BarCode";
            this.textBox_BarCode.Size = new System.Drawing.Size(220, 30);
            this.textBox_BarCode.TabIndex = 2;
            this.textBox_BarCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_BarCode.TextChanged += new System.EventHandler(this.textBox_BarCode_TextChanged);
            // 
            // shapeContainer1
            // 
            this.shapeContainer1.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer1.Name = "shapeContainer1";
            this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.rectangleShape1});
            this.shapeContainer1.Size = new System.Drawing.Size(242, 49);
            this.shapeContainer1.TabIndex = 3;
            this.shapeContainer1.TabStop = false;
            // 
            // rectangleShape1
            // 
            this.rectangleShape1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.rectangleShape1.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.rectangleShape1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(6)))), ((int)(((byte)(69)))));
            this.rectangleShape1.Location = new System.Drawing.Point(6, 9);
            this.rectangleShape1.Name = "rectangleShape1";
            this.rectangleShape1.Size = new System.Drawing.Size(220, 30);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Agency FB", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "Antibody Barcode:";
            // 
            // RoboSep_ScanSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 470);
            this.Controls.Add(this.roboPanel_CarouselScan);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "RoboSep_ScanSelection";
            this.Text = "RoboSep_ScanSelection";
            this.Load += new System.EventHandler(this.RoboSep_ScanSelection_Load);
            this.roboPanel_CarouselScan.ResumeLayout(false);
            this.roboPanel_CarouselScan.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private GUI_Controls.RoboPanel roboPanel_CarouselScan;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private GUI_Controls.DragScrollListView listView_Protocols;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer2;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBox_BarCode;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape1;
        private GUI_Controls.Button_Keyboard button_Keyboard2;
        private GUI_Controls.Button_Keyboard button_Keyboard1;
        private GUI_Controls.Button_Circle button_Cancel;
    }
}