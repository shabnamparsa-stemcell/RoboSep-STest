using Tesla.Common;

namespace GUI_Controls
{
    partial class RoboMessagePanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoboMessagePanel));
            this.label_WindowTitle = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.rectangleShape1 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.MSGprogress = new System.Windows.Forms.ProgressBar();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.txtMessage = new System.Windows.Forms.Label();
            this.button_2 = new GUI_Controls.Button_Rectangle();
            this.button_1 = new GUI_Controls.Button_Rectangle();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label_WindowTitle
            // 
            this.label_WindowTitle.BackColor = System.Drawing.Color.Transparent;
            this.label_WindowTitle.Font = new System.Drawing.Font("Arial", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_WindowTitle.ForeColor = System.Drawing.Color.White;
            this.label_WindowTitle.Location = new System.Drawing.Point(6, 5);
            this.label_WindowTitle.Margin = new System.Windows.Forms.Padding(0);
            this.label_WindowTitle.Name = "label_WindowTitle";
            this.label_WindowTitle.Size = new System.Drawing.Size(354, 27);
            this.label_WindowTitle.TabIndex = 3;
            this.label_WindowTitle.Text = "Window Title";
            this.label_WindowTitle.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.Controls.Add(this.label_WindowTitle);
            this.panel1.Location = new System.Drawing.Point(1, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(378, 37);
            this.panel1.TabIndex = 28;
            // 
            // shapeContainer1
            // 
            this.shapeContainer1.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer1.Name = "shapeContainer1";
            this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.rectangleShape1});
            this.shapeContainer1.Size = new System.Drawing.Size(380, 306);
            this.shapeContainer1.TabIndex = 29;
            this.shapeContainer1.TabStop = false;
            // 
            // rectangleShape1
            // 
            this.rectangleShape1.BorderWidth = 2;
            this.rectangleShape1.Location = new System.Drawing.Point(0, 0);
            this.rectangleShape1.Name = "rectangleShape1";
            this.rectangleShape1.Size = new System.Drawing.Size(378, 303);
            // 
            // MSGprogress
            // 
            this.MSGprogress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.MSGprogress.BackColor = System.Drawing.Color.DarkGray;
            this.MSGprogress.ForeColor = System.Drawing.Color.White;
            this.MSGprogress.Location = new System.Drawing.Point(22, 218);
            this.MSGprogress.Name = "MSGprogress";
            this.MSGprogress.Size = new System.Drawing.Size(333, 44);
            this.MSGprogress.TabIndex = 8;
            this.MSGprogress.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 58);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(52, 52);
            this.pictureBox1.TabIndex = 30;
            this.pictureBox1.TabStop = false;
            // 
            // txtMessage
            // 
            this.txtMessage.BackColor = System.Drawing.Color.Transparent;
            this.txtMessage.Font = new System.Drawing.Font("Arial Narrow", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMessage.Location = new System.Drawing.Point(79, 58);
            this.txtMessage.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(282, 183);
            this.txtMessage.TabIndex = 31;
            this.txtMessage.Text = "label1";
            // 
            // button_2
            // 
            this.button_2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_2.AutoSize = true;
            this.button_2.BackColor = System.Drawing.Color.Transparent;
            this.button_2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_2.BackgroundImage")));
            this.button_2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_2.Check = false;
            this.button_2.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_2.ForeColor = System.Drawing.Color.White;
            this.button_2.Location = new System.Drawing.Point(170, 259);
            this.button_2.Name = "button_2";
            this.button_2.Size = new System.Drawing.Size(91, 35);
            this.button_2.TabIndex = 26;
            this.button_2.Text = "Cancel";
            this.button_2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button_1
            // 
            this.button_1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_1.AutoSize = true;
            this.button_1.BackColor = System.Drawing.Color.Transparent;
            this.button_1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_1.BackgroundImage")));
            this.button_1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_1.Check = false;
            this.button_1.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_1.ForeColor = System.Drawing.Color.White;
            this.button_1.Location = new System.Drawing.Point(270, 259);
            this.button_1.Name = "button_1";
            this.button_1.Size = new System.Drawing.Size(91, 35);
            this.button_1.TabIndex = 27;
            this.button_1.Text = "OK";
            this.button_1.Click += new System.EventHandler(this.button1_Click);
            // 
            // RoboMessagePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(380, 306);
            this.ControlBox = false;
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button_2);
            this.Controls.Add(this.button_1);
            this.Controls.Add(this.MSGprogress);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.shapeContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "RoboMessagePanel";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Load += new System.EventHandler(this.RoboMessagePanel_Load);
            this.RegionChanged += new System.EventHandler(this.RoboMessagePanel_RegionChanged);
            this.SizeChanged += new System.EventHandler(this.RoboMessagePanel_SizeChanged);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar MSGprogress;
        protected Button_Rectangle button_2;
        protected Button_Rectangle button_1;
        protected System.Windows.Forms.Label label_WindowTitle;
        private System.Windows.Forms.Panel panel1;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label txtMessage;
    }
}
