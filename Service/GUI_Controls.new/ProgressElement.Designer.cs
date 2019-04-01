using Tesla.Common;

namespace GUI_Controls
{
    partial class ProgressElement
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
        
        /*
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        */
        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.rectProgress = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.EstimatedProgressTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // shapeContainer1
            // 
            this.shapeContainer1.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer1.Name = "shapeContainer1";
            this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.rectProgress});
            this.shapeContainer1.Size = new System.Drawing.Size(48, 48);
            this.shapeContainer1.TabIndex = 0;
            this.shapeContainer1.TabStop = false;
            // 
            // rectProgress
            // 
            this.rectProgress.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
            this.rectProgress.BorderColor = System.Drawing.Color.Tomato;
            this.rectProgress.FillColor = System.Drawing.Color.Tomato;
            this.rectProgress.FillGradientColor = System.Drawing.Color.Violet;
            this.rectProgress.FillStyle = Microsoft.VisualBasic.PowerPacks.FillStyle.Solid;
            this.rectProgress.Location = new System.Drawing.Point(0, 0);
            this.rectProgress.Name = "rectProgress";
            this.rectProgress.Size = new System.Drawing.Size(0, 50);
            this.rectProgress.Visible = false;
            this.rectProgress.Paint += new System.Windows.Forms.PaintEventHandler(this.rectProgress_Paint);
            // 
            // EstimatedProgressTimer
            // 
            this.EstimatedProgressTimer.Interval = 1000;
            this.EstimatedProgressTimer.Tick += new System.EventHandler(this.EstimatedProgressTimer_Tick);
            // 
            // ProgressElement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.shapeContainer1);
            this.Name = "ProgressElement";
            this.Size = new System.Drawing.Size(48, 48);
            this.Load += new System.EventHandler(this.ProgressElement_Load);
            this.Resize += new System.EventHandler(this.ProgressElement_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectProgress;
        private System.Windows.Forms.Timer EstimatedProgressTimer;
    }
}
