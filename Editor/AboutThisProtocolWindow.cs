using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Tesla.ProtocolEditorModel;
using ProtocolVersionManager;
using Tesla.ProtocolEditorControls;

namespace Tesla.ProtocolEditor
{
	/// <summary>
	/// Summary description for AboutThisProtocolWindow.
	/// </summary>
	public class AboutThisProtocolWindow : System.Windows.Forms.Form
	{
        private System.Windows.Forms.TextBox txtModificationDate;
        private System.Windows.Forms.TextBox txtCreationDate;
		private System.Windows.Forms.Label lblModificationDate;
		private System.Windows.Forms.Label lblCreationDate;
        private System.Windows.Forms.TextBox txtProtocolVersion;
		private System.Windows.Forms.Label lblProtocolVersion;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
        private System.Windows.Forms.TextBox ProtocolNum1;
		private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox ProtocolNum2;
        private ProtocolLabelDecriptionAndAuthorTextbox txtProtocolAuthor;
		private System.Windows.Forms.Label lblAuthor;
		private System.Windows.Forms.Label lblRoboSep;
		
		private static ProtocolModel	theProtocolModel;

		public AboutThisProtocolWindow()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			
			theProtocolModel = ProtocolModel.GetInstance();
			
			// set data in textboxes
			txtCreationDate.Text = "" + theProtocolModel.ProtocolCreationDate;

			if (theProtocolModel.ProtocolModificationDateIsSpecified)
			{
				txtModificationDate.Text = "" + theProtocolModel.ProtocolModificationDate;
			}
			else
			{
				txtModificationDate.Text = string.Empty;
			}
			txtProtocolVersion.Text = "" + theProtocolModel.ProtocolVersion;

			txtProtocolAuthor.Text = "" + theProtocolModel.ProtocolAuthor;
			ProtocolNum1.Text = "" + theProtocolModel.ProtocolNumber1;
			ProtocolNum2.Text = "" + theProtocolModel.ProtocolNumber2;

			VersionManager VerMgr = VersionManager.GetInstance();
			/*
			if ((VerMgr.InputVersions[0] == 0)||(VerMgr.InputVersions[1] == 0))
			{
				lblRoboSep.Text = "RoboSep Protocol Editor Ver: N/A";

			}
			else
			{
                lblRoboSep.Text = "RoboSep Protocol Editor Ver: " +
					VerMgr.InputVersions[0].ToString() + "." +
					VerMgr.InputVersions[1].ToString() + "." +
					VerMgr.InputVersions[2].ToString() + "." +
					VerMgr.InputVersions[3].ToString();
			}
             */
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutThisProtocolWindow));
            this.lblModificationDate = new System.Windows.Forms.Label();
            this.lblCreationDate = new System.Windows.Forms.Label();
            this.lblProtocolVersion = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblAuthor = new System.Windows.Forms.Label();
            this.lblRoboSep = new System.Windows.Forms.Label();
            this.ProtocolNum1 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.ProtocolNum2 = new System.Windows.Forms.TextBox();
            this.txtProtocolAuthor = new Tesla.ProtocolEditorControls.ProtocolLabelDecriptionAndAuthorTextbox();
            this.txtModificationDate = new System.Windows.Forms.TextBox();
            this.txtCreationDate = new System.Windows.Forms.TextBox();
            this.txtProtocolVersion = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblModificationDate
            // 
            this.lblModificationDate.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblModificationDate.Location = new System.Drawing.Point(16, 104);
            this.lblModificationDate.Name = "lblModificationDate";
            this.lblModificationDate.Size = new System.Drawing.Size(64, 23);
            this.lblModificationDate.TabIndex = 17;
            this.lblModificationDate.Text = "Modified";
            // 
            // lblCreationDate
            // 
            this.lblCreationDate.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCreationDate.Location = new System.Drawing.Point(16, 64);
            this.lblCreationDate.Name = "lblCreationDate";
            this.lblCreationDate.Size = new System.Drawing.Size(64, 23);
            this.lblCreationDate.TabIndex = 15;
            this.lblCreationDate.Text = "Created";
            // 
            // lblProtocolVersion
            // 
            this.lblProtocolVersion.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProtocolVersion.Location = new System.Drawing.Point(16, 24);
            this.lblProtocolVersion.Name = "lblProtocolVersion";
            this.lblProtocolVersion.Size = new System.Drawing.Size(64, 23);
            this.lblProtocolVersion.TabIndex = 13;
            this.lblProtocolVersion.Text = "Version";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 184);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 23);
            this.label1.TabIndex = 103;
            this.label1.Text = "Protocol Number";
            // 
            // lblAuthor
            // 
            this.lblAuthor.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAuthor.Location = new System.Drawing.Point(16, 144);
            this.lblAuthor.Name = "lblAuthor";
            this.lblAuthor.Size = new System.Drawing.Size(44, 23);
            this.lblAuthor.TabIndex = 100;
            this.lblAuthor.Text = "Author";
            // 
            // lblRoboSep
            // 
            this.lblRoboSep.Location = new System.Drawing.Point(16, 216);
            this.lblRoboSep.Name = "lblRoboSep";
            this.lblRoboSep.Size = new System.Drawing.Size(216, 23);
            this.lblRoboSep.TabIndex = 106;
            // 
            // ProtocolNum1
            // 
            this.ProtocolNum1.Enabled = false;
            this.ProtocolNum1.Location = new System.Drawing.Point(112, 184);
            this.ProtocolNum1.MaxLength = 5;
            this.ProtocolNum1.Name = "ProtocolNum1";
            this.ProtocolNum1.ReadOnly = true;
            this.ProtocolNum1.Size = new System.Drawing.Size(40, 20);
            this.ProtocolNum1.TabIndex = 104;
            this.ProtocolNum1.TextChanged += new System.EventHandler(this.ProtocolNum1_TextChanged);
            // 
            // textBox3
            // 
            this.textBox3.Enabled = false;
            this.textBox3.Font = new System.Drawing.Font("Arial", 8.25F);
            this.textBox3.Location = new System.Drawing.Point(152, 184);
            this.textBox3.MaxLength = 1;
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(24, 20);
            this.textBox3.TabIndex = 102;
            this.textBox3.Text = "–";
            this.textBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ProtocolNum2
            // 
            this.ProtocolNum2.Enabled = false;
            this.ProtocolNum2.Location = new System.Drawing.Point(176, 184);
            this.ProtocolNum2.MaxLength = 4;
            this.ProtocolNum2.Name = "ProtocolNum2";
            this.ProtocolNum2.ReadOnly = true;
            this.ProtocolNum2.Size = new System.Drawing.Size(36, 20);
            this.ProtocolNum2.TabIndex = 105;
            this.ProtocolNum2.TextChanged += new System.EventHandler(this.ProtocolNum2_TextChanged);
            // 
            // txtProtocolAuthor
            // 
            this.txtProtocolAuthor.Location = new System.Drawing.Point(80, 144);
            this.txtProtocolAuthor.Name = "txtProtocolAuthor";
            this.txtProtocolAuthor.Size = new System.Drawing.Size(148, 20);
            this.txtProtocolAuthor.TabIndex = 101;
            this.txtProtocolAuthor.TextChanged += new System.EventHandler(this.txtProtocolAuthor_TextChanged);
            // 
            // txtModificationDate
            // 
            this.txtModificationDate.Enabled = false;
            this.txtModificationDate.Location = new System.Drawing.Point(80, 104);
            this.txtModificationDate.Name = "txtModificationDate";
            this.txtModificationDate.ReadOnly = true;
            this.txtModificationDate.Size = new System.Drawing.Size(136, 20);
            this.txtModificationDate.TabIndex = 18;
            // 
            // txtCreationDate
            // 
            this.txtCreationDate.Enabled = false;
            this.txtCreationDate.Location = new System.Drawing.Point(80, 64);
            this.txtCreationDate.Name = "txtCreationDate";
            this.txtCreationDate.ReadOnly = true;
            this.txtCreationDate.Size = new System.Drawing.Size(136, 20);
            this.txtCreationDate.TabIndex = 16;
            // 
            // txtProtocolVersion
            // 
            this.txtProtocolVersion.Location = new System.Drawing.Point(80, 24);
            this.txtProtocolVersion.Name = "txtProtocolVersion";
            this.txtProtocolVersion.Size = new System.Drawing.Size(60, 20);
            this.txtProtocolVersion.TabIndex = 14;
            this.txtProtocolVersion.TextChanged += new System.EventHandler(this.txtProtocolVersion_TextChanged);
            // 
            // AboutThisProtocolWindow
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(250, 252);
            this.Controls.Add(this.lblRoboSep);
            this.Controls.Add(this.ProtocolNum1);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.ProtocolNum2);
            this.Controls.Add(this.txtProtocolAuthor);
            this.Controls.Add(this.txtModificationDate);
            this.Controls.Add(this.txtCreationDate);
            this.Controls.Add(this.txtProtocolVersion);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblAuthor);
            this.Controls.Add(this.lblModificationDate);
            this.Controls.Add(this.lblCreationDate);
            this.Controls.Add(this.lblProtocolVersion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "AboutThisProtocolWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About Current Protocol";
            this.Load += new System.EventHandler(this.AboutThisProtocolWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void ProtocolNum1_TextChanged(object sender, System.EventArgs e)
		{
			//switch to ProtocolNum2 editbox when this box is full.
			theProtocolModel.ProtocolNumber1 = ProtocolNum1.Text;
			if(ProtocolNum1.Text.Length == 5)
			{
				ProtocolNum2.Focus();
			}
		}

		private void ProtocolNum2_TextChanged(object sender, System.EventArgs e)
		{
			theProtocolModel.ProtocolNumber2 = ProtocolNum2.Text;
		}

		private void txtProtocolAuthor_TextChanged(object sender, System.EventArgs e)
		{
			theProtocolModel.ProtocolAuthor = txtProtocolAuthor.Text;
		}

		private void AboutThisProtocolWindow_Load(object sender, System.EventArgs e)
		{
		
		}

        private void txtProtocolVersion_TextChanged(object sender, EventArgs e)
        {
            if (txtProtocolVersion.Text == theProtocolModel.ProtocolVersion) return;
            int i;
            if (int.TryParse(txtProtocolVersion.Text, out i) && i > 0)
            {
                theProtocolModel.ProtocolVersion = txtProtocolVersion.Text;
            }
            else
            {
                txtProtocolVersion.Text = theProtocolModel.ProtocolVersion;
            }
        }
	}
}
