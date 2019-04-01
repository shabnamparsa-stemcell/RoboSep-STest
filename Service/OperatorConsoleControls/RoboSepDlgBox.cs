using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Tesla.Common.DrawingUtilities;
using Tesla.Common.ResourceManagement;
using Tesla.Separator;

namespace Tesla.OperatorConsoleControls
{
	/// <summary>
	/// Summary description for RoboSepDlgBox.
	/// </summary>
	public class RoboSepDlgBox : System.Windows.Forms.Form
	{
		private Tesla.OperatorConsoleControls.RoboSepNamedPanel myBorder;
		private System.Windows.Forms.Label lblMessage;
		private Tesla.OperatorConsoleControls.RoboSepButton btnCancel;
		private Tesla.OperatorConsoleControls.RoboSepButton btnOK;
		private System.ComponentModel.IContainer components;


		public RoboSepDlgBox(string title, string caption)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
	
			myBorder.Text=title;
			lblMessage.Text=caption;
			Color namedAreaStandardBackground = ColourScheme.GetColour(
				ColourSchemeItem.NamedAreaStandardBackground);
			myBorder.FillColor  = namedAreaStandardBackground;
			lblMessage.BackColor = namedAreaStandardBackground;
			btnOK.BackColor = namedAreaStandardBackground;	
			btnCancel.BackColor = namedAreaStandardBackground;

			btnOK.Text = "OK";
			btnOK.Role = RoboSepButton.ButtonRole.OK;
			btnCancel.Text = "Cancel";
			btnCancel.Role = RoboSepButton.ButtonRole.Error;
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
			this.myBorder = new Tesla.OperatorConsoleControls.RoboSepNamedPanel();
			this.lblMessage = new System.Windows.Forms.Label();
			this.btnCancel = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.btnOK = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.SuspendLayout();
			// 
			// myBorder
			// 
			this.myBorder.BackColor = System.Drawing.SystemColors.Control;
			this.myBorder.FillColor = System.Drawing.Color.White;
			this.myBorder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
			this.myBorder.Location = new System.Drawing.Point(0, 0);
			this.myBorder.Name = "myBorder";
			this.myBorder.Size = new System.Drawing.Size(336, 184);
			this.myBorder.TabIndex = 0;
			// 
			// lblMessage
			// 
			this.lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
			this.lblMessage.Location = new System.Drawing.Point(16, 48);
			this.lblMessage.Name = "lblMessage";
			this.lblMessage.Size = new System.Drawing.Size(304, 56);
			this.lblMessage.TabIndex = 1;
			this.lblMessage.Text = "label1";
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
			this.btnCancel.Location = new System.Drawing.Point(208, 128);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(120, 48);
			this.btnCancel.TabIndex = 14;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnOK
			// 
			this.btnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
			this.btnOK.Location = new System.Drawing.Point(8, 128);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(120, 48);
			this.btnOK.TabIndex = 15;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// RoboSepDlgBox
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(336, 184);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.lblMessage);
			this.Controls.Add(this.myBorder);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "RoboSepDlgBox";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "RoboSepDlgBox";
			this.ResumeLayout(false);

		}
		#endregion

		public new DialogResult ShowDialog()
		{
			DialogResult result;
			result = base.ShowDialog();
			return result;
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
		DialogResult = DialogResult.Cancel;
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
		DialogResult = DialogResult.OK;
		}

	}
}
