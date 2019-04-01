using Tesla.Common;


namespace GUI_Console
{
    partial class RoboMessagePanel6
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
            if (disposing)
            {
                Utilities.DisposeShapeContainer(shapeContainer1);
            }

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoboMessagePanel6));
            this.rectangleShape1 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label_WindowTitle = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.txtMessage = new System.Windows.Forms.Label();
            this.button_3 = new GUI_Controls.Button_Circle();
            this.button_2 = new GUI_Controls.Button_Circle();
            this.button_1 = new GUI_Controls.Button_Cancel();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // rectangleShape1
            // 
            this.rectangleShape1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.rectangleShape1.BorderWidth = 2;
            this.rectangleShape1.Location = new System.Drawing.Point(0, 0);
            this.rectangleShape1.Name = "rectangleShape1";
            this.rectangleShape1.Size = new System.Drawing.Size(396, 357);
            // 
            // shapeContainer1
            // 
            this.shapeContainer1.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer1.Name = "shapeContainer1";
            this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.rectangleShape1});
            this.shapeContainer1.Size = new System.Drawing.Size(396, 360);
            this.shapeContainer1.TabIndex = 0;
            this.shapeContainer1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.Controls.Add(this.label_WindowTitle);
            this.panel1.Location = new System.Drawing.Point(1, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(395, 34);
            this.panel1.TabIndex = 30;
            // 
            // label_WindowTitle
            // 
            this.label_WindowTitle.BackColor = System.Drawing.Color.Transparent;
            this.label_WindowTitle.Font = new System.Drawing.Font("Arial", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_WindowTitle.ForeColor = System.Drawing.Color.White;
            this.label_WindowTitle.Location = new System.Drawing.Point(8, 3);
            this.label_WindowTitle.Margin = new System.Windows.Forms.Padding(0);
            this.label_WindowTitle.Name = "label_WindowTitle";
            this.label_WindowTitle.Size = new System.Drawing.Size(378, 27);
            this.label_WindowTitle.TabIndex = 33;
            this.label_WindowTitle.Text = "Window Title";
            this.label_WindowTitle.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::GUI_Controls.Properties.Resources.Message_Information;
            this.pictureBox1.Location = new System.Drawing.Point(12, 58);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(52, 52);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 80;
            this.pictureBox1.TabStop = false;
            // 
            // txtMessage
            // 
            this.txtMessage.BackColor = System.Drawing.Color.Transparent;
            this.txtMessage.Font = new System.Drawing.Font("Arial Narrow", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMessage.Location = new System.Drawing.Point(79, 58);
            this.txtMessage.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(300, 173);
            this.txtMessage.TabIndex = 81;
            this.txtMessage.Text = "label1";
            // 
            // button_3
            // 
            this.button_3.BackColor = System.Drawing.Color.Transparent;
            this.button_3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_3.BackgroundImage")));
            this.button_3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_3.Check = false;
            this.button_3.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_3.ForeColor = System.Drawing.Color.White;
            this.button_3.Location = new System.Drawing.Point(57, 262);
            this.button_3.Name = "button_3";
            this.button_3.Size = new System.Drawing.Size(104, 86);
            this.button_3.TabIndex = 79;
            this.button_3.Text = "button_3";
            this.button_3.Visible = false;
            // 
            // button_2
            // 
            this.button_2.BackColor = System.Drawing.Color.Transparent;
            this.button_2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_2.BackgroundImage")));
            this.button_2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_2.Check = false;
            this.button_2.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_2.ForeColor = System.Drawing.Color.White;
            this.button_2.Location = new System.Drawing.Point(167, 262);
            this.button_2.Name = "button_2";
            this.button_2.Size = new System.Drawing.Size(104, 86);
            this.button_2.TabIndex = 78;
            this.button_2.Text = "button_2";
            this.button_2.Visible = false;
            // 
            // button_1
            // 
            this.button_1.BackColor = System.Drawing.Color.Transparent;
            this.button_1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_1.BackgroundImage")));
            this.button_1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_1.Check = false;
            this.button_1.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_1.ForeColor = System.Drawing.Color.White;
            this.button_1.Location = new System.Drawing.Point(278, 262);
            this.button_1.Name = "button_1";
            this.button_1.Size = new System.Drawing.Size(104, 86);
            this.button_1.TabIndex = 34;
            this.button_1.Text = "button_1";
            this.button_1.Visible = false;
            // 
            // RoboMessagePanel6
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(396, 360);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button_3);
            this.Controls.Add(this.button_2);
            this.Controls.Add(this.button_1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.shapeContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "RoboMessagePanel6";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "RoboMessagePanel6";
            this.Load += new System.EventHandler(this.RoboMessagePanel6_Load);
            this.SizeChanged += new System.EventHandler(this.RoboMessagePanel6_SizeChanged);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape1;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private System.Windows.Forms.Panel panel1;
        protected System.Windows.Forms.Label label_WindowTitle;
        private GUI_Controls.Button_Cancel button_1;
        private GUI_Controls.Button_Circle button_2;
        private GUI_Controls.Button_Circle button_3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label txtMessage;
    }
}