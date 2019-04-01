//----------------------------------------------------------------------------
// FrmProtocolValidation
//
// Invetech Pty Ltd
// Victoria, 3149, Australia
// Phone:   (+61) 3 9211 7700
// Fax:     (+61) 3 9211 7701
//
// The copyright to the computer program(s) herein is the property of 
// Invetech Pty Ltd, Australia.
// The program(s) may be used and/or copied only with the written permission 
// of Invetech Pty Ltd or in accordance with the terms and conditions 
// stipulated in the agreement/contract under which the program(s)
// have been supplied.
// 
// Copyright © 2004. All Rights Reserved.
//
//----------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Tesla.ProtocolEditorControls;

namespace Tesla.ProtocolEditor
{
	/// <summary>
	/// Summary description for FrmProtocolValidation.
	/// </summary>
	public class FrmProtocolValidation : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnChange;
        private System.Windows.Forms.TextBox txtValidationErrors;
		private System.Windows.Forms.Label lblValidationErrors;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#region Construction/destruction

		public FrmProtocolValidation()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
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

		#endregion Construction/destruction

		#region Form events

		private void FrmProtocolValidation_Load(object sender, System.EventArgs e)
		{		 
		}

		#endregion Form events

		#region Navigation

		private void btnChange_Click(object sender, System.EventArgs e)
		{
			// Return "change page" result code
			((FrmProtocolEditor)MdiParent).ChangePage(DialogResult.OK);
		}

		#endregion Navigation

		#region Validation 

		private ArrayList myValidationErrorLines = new ArrayList();

		public void ClearValidationErrors()
		{
			myValidationErrorLines.Clear();
			UpdateValidationErrorDisplay();
		}

		public void DisplayValidationError(string validationError)
		{
			// Append a new line to the errors display.  
			myValidationErrorLines.Add(validationError);
			UpdateValidationErrorDisplay();						
		}

		private void UpdateValidationErrorDisplay()
		{
			txtValidationErrors.Lines = (string[])
				myValidationErrorLines.ToArray(typeof(string));
		}

		#endregion Validation

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnChange = new System.Windows.Forms.Button();
            this.txtValidationErrors = new System.Windows.Forms.TextBox();
			this.lblValidationErrors = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btnChange
			// 
			this.btnChange.Location = new System.Drawing.Point(456, 8);
			this.btnChange.Name = "btnChange";
			this.btnChange.TabIndex = 12;
			this.btnChange.Text = "Change";
			this.btnChange.Visible = false;
			this.btnChange.Click += new System.EventHandler(this.btnChange_Click);
			// 
			// txtValidationErrors
			// 
			this.txtValidationErrors.Location = new System.Drawing.Point(49, 72);
			this.txtValidationErrors.Multiline = true;
			this.txtValidationErrors.Name = "txtValidationErrors";
			this.txtValidationErrors.ReadOnly = true;
			this.txtValidationErrors.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtValidationErrors.Size = new System.Drawing.Size(440, 432);
			this.txtValidationErrors.TabIndex = 13;
			this.txtValidationErrors.Text = "";
			// 
			// lblValidationErrors
			// 
			this.lblValidationErrors.Location = new System.Drawing.Point(49, 48);
			this.lblValidationErrors.Name = "lblValidationErrors";
			this.lblValidationErrors.Size = new System.Drawing.Size(319, 23);
			this.lblValidationErrors.TabIndex = 14;
			this.lblValidationErrors.Text = "The protocol definition contains the following errors:";
			// 
			// FrmProtocolValidation
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(538, 540);
			this.Controls.Add(this.lblValidationErrors);
			this.Controls.Add(this.txtValidationErrors);
			this.Controls.Add(this.btnChange);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "FrmProtocolValidation";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Load += new System.EventHandler(this.FrmProtocolValidation_Load);
			this.ResumeLayout(false);

		}
		#endregion		
	}
}
