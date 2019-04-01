using Tesla.Common;


namespace GUI_Controls
{
    partial class RoboPanel
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
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.Rect4 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.Rect3 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.Rect2 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.Rect1 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.label1 = new System.Windows.Forms.Label();
            this.Corner_BR = new System.Windows.Forms.PictureBox();
            this.Corner_TR = new System.Windows.Forms.PictureBox();
            this.Corner_BL = new System.Windows.Forms.PictureBox();
            this.Corner_TL = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.Corner_BR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Corner_TR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Corner_BL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Corner_TL)).BeginInit();
            this.SuspendLayout();
            // 
            // shapeContainer1
            // 
            this.shapeContainer1.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer1.Name = "shapeContainer1";
            this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.Rect4,
            this.Rect3,
            this.Rect2,
            this.Rect1});
            this.shapeContainer1.Size = new System.Drawing.Size(300, 400);
            this.shapeContainer1.TabIndex = 4;
            this.shapeContainer1.TabStop = false;
            // 
            // Rect4
            // 
            this.Rect4.BackColor = System.Drawing.Color.Purple;
            this.Rect4.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.Rect4.BorderStyle = System.Drawing.Drawing2D.DashStyle.Custom;
            this.Rect4.Location = new System.Drawing.Point(294, 19);
            this.Rect4.Name = "Rect4";
            this.Rect4.Size = new System.Drawing.Size(6, 362);
            // 
            // Rect3
            // 
            this.Rect3.BackColor = System.Drawing.Color.Purple;
            this.Rect3.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.Rect3.BorderStyle = System.Drawing.Drawing2D.DashStyle.Custom;
            this.Rect3.Location = new System.Drawing.Point(0, 19);
            this.Rect3.Name = "Rect3";
            this.Rect3.Size = new System.Drawing.Size(6, 362);
            // 
            // Rect2
            // 
            this.Rect2.BackColor = System.Drawing.Color.Purple;
            this.Rect2.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.Rect2.BorderStyle = System.Drawing.Drawing2D.DashStyle.Custom;
            this.Rect2.Location = new System.Drawing.Point(19, 394);
            this.Rect2.Name = "Rect2";
            this.Rect2.Size = new System.Drawing.Size(262, 6);
            // 
            // Rect1
            // 
            this.Rect1.BackColor = System.Drawing.Color.Purple;
            this.Rect1.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.Rect1.BorderStyle = System.Drawing.Drawing2D.DashStyle.Custom;
            this.Rect1.Location = new System.Drawing.Point(19, 0);
            this.Rect1.Name = "Rect1";
            this.Rect1.Size = new System.Drawing.Size(262, 6);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Agency FB", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(17, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 26);
            this.label1.TabIndex = 5;
            this.label1.Text = "label1";
            // 
            // Corner_BR
            // 
            this.Corner_BR.Image = global::GUI_Controls.Properties.Resources.RoboPANEL_CORNER_BOTTOMRIGHT;
            this.Corner_BR.Location = new System.Drawing.Point(281, 381);
            this.Corner_BR.Name = "Corner_BR";
            this.Corner_BR.Size = new System.Drawing.Size(19, 19);
            this.Corner_BR.TabIndex = 10;
            this.Corner_BR.TabStop = false;
            // 
            // Corner_TR
            // 
            this.Corner_TR.Image = global::GUI_Controls.Properties.Resources.RoboPANEL_CORNER_TOPRIGHT;
            this.Corner_TR.Location = new System.Drawing.Point(281, 0);
            this.Corner_TR.Name = "Corner_TR";
            this.Corner_TR.Size = new System.Drawing.Size(19, 19);
            this.Corner_TR.TabIndex = 9;
            this.Corner_TR.TabStop = false;
            // 
            // Corner_BL
            // 
            this.Corner_BL.Image = global::GUI_Controls.Properties.Resources.RoboPANEL_CORNER_BOTTOMLEFT;
            this.Corner_BL.Location = new System.Drawing.Point(0, 381);
            this.Corner_BL.Name = "Corner_BL";
            this.Corner_BL.Size = new System.Drawing.Size(19, 19);
            this.Corner_BL.TabIndex = 8;
            this.Corner_BL.TabStop = false;
            // 
            // Corner_TL
            // 
            this.Corner_TL.Image = global::GUI_Controls.Properties.Resources.RoboPANEL_CORNER_TOPLEFT;
            this.Corner_TL.Location = new System.Drawing.Point(0, 0);
            this.Corner_TL.Name = "Corner_TL";
            this.Corner_TL.Size = new System.Drawing.Size(19, 19);
            this.Corner_TL.TabIndex = 7;
            this.Corner_TL.TabStop = false;
            // 
            // RoboPanel
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Controls.Add(this.Corner_BR);
            this.Controls.Add(this.Corner_TR);
            this.Controls.Add(this.Corner_BL);
            this.Controls.Add(this.Corner_TL);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.shapeContainer1);
            this.Name = "RoboPanel";
            this.Size = new System.Drawing.Size(300, 400);
            this.RegionChanged += new System.EventHandler(this.RoboPanel_RegionChanged);
            this.SizeChanged += new System.EventHandler(this.RoboPanel_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.Corner_BR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Corner_TR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Corner_BL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Corner_TL)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape Rect3;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape Rect2;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape Rect1;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape Rect4;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.PictureBox Corner_TL;
        public System.Windows.Forms.PictureBox Corner_BL;
        public System.Windows.Forms.PictureBox Corner_BR;
        public System.Windows.Forms.PictureBox Corner_TR;

    }
}
