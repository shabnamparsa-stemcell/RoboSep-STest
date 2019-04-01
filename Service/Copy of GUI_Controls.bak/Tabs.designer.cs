namespace GUI_Controls
{
    partial class Tabs
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
            this.Tab1 = new System.Windows.Forms.PictureBox();
            this.Tab2 = new System.Windows.Forms.PictureBox();
            this.Tab3 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.Tab1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Tab2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Tab3)).BeginInit();
            this.SuspendLayout();
            // 
            // Tab1
            // 
            this.Tab1.BackColor = System.Drawing.Color.Transparent;
            this.Tab1.Location = new System.Drawing.Point(0, 0);
            this.Tab1.Name = "Tab1";
            this.Tab1.Size = new System.Drawing.Size(67, 165);
            this.Tab1.TabIndex = 0;
            this.Tab1.TabStop = false;
            this.Tab1.Click += new System.EventHandler(this.Tab1_Click);
            this.Tab1.MouseEnter += new System.EventHandler(this.Tab1_MouseEnter);
            this.Tab1.MouseLeave += new System.EventHandler(this.Tab1_MouseLeave);
            // 
            // Tab2
            // 
            this.Tab2.BackColor = System.Drawing.Color.Transparent;
            this.Tab2.Location = new System.Drawing.Point(0, 167);
            this.Tab2.Name = "Tab2";
            this.Tab2.Size = new System.Drawing.Size(67, 158);
            this.Tab2.TabIndex = 1;
            this.Tab2.TabStop = false;
            this.Tab2.Click += new System.EventHandler(this.Tab2_Click);
            this.Tab2.MouseEnter += new System.EventHandler(this.Tab2_MouseEnter);
            this.Tab2.MouseLeave += new System.EventHandler(this.Tab2_MouseLeave);
            // 
            // Tab3
            // 
            this.Tab3.BackColor = System.Drawing.Color.Transparent;
            this.Tab3.Location = new System.Drawing.Point(0, 327);
            this.Tab3.Name = "Tab3";
            this.Tab3.Size = new System.Drawing.Size(67, 153);
            this.Tab3.TabIndex = 2;
            this.Tab3.TabStop = false;
            this.Tab3.Click += new System.EventHandler(this.Tab3_Click);
            this.Tab3.MouseEnter += new System.EventHandler(this.Tab3_MouseEnter);
            this.Tab3.MouseLeave += new System.EventHandler(this.Tab3_MouseLeave);
            // 
            // Tabs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::GUI_Controls.Properties.Resources.TAB_RUNSTAT1;
            this.Controls.Add(this.Tab3);
            this.Controls.Add(this.Tab2);
            this.Controls.Add(this.Tab1);
            this.Name = "Tabs";
            this.Size = new System.Drawing.Size(67, 480);
            this.Load += new System.EventHandler(this.SideTabs_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Tab1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Tab2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Tab3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox Tab1;
        private System.Windows.Forms.PictureBox Tab2;
        private System.Windows.Forms.PictureBox Tab3;

    }
}
