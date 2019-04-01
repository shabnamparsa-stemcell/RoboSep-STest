//----------------------------------------------------------------------------
// IncubateCommandPanel
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
//
// 2011-09-05 to 2011-09-16 sp various changes
//     - provide support for use in smaller screen displays (support for scrollbar in other files)
//     - align and resize panels for more unify displays  
//
//----------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Tesla.ProtocolEditorModel;
using Tesla.Common.Protocol;

namespace Tesla.ProtocolEditorControls
{
	public class IncubateCommandPanel : ProtocolEditorControls.CommandPanel
	{
		private System.Windows.Forms.Label lblIncubateProcessingTime;
        private System.Windows.Forms.TextBox txtIncubateProcessingTime;
		private System.Windows.Forms.ErrorProvider errorIncubateProcessingTime;
		private System.ComponentModel.IContainer components = null;

		#region Construction/destruction

		public IncubateCommandPanel()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#endregion Construction/destruction

		#region Properties

		public override bool IsContentValid()
		{
			return base.IsContentValid()					&&
				txtIncubateProcessingTime.Text.Length > 0	&&
				myIncubateProcessingTime > -1;
		}

		private int myIncubateProcessingTime = -1;

		public uint WaitCommandTimeDuration
		{
			get
			{
				return myIncubateProcessingTime < 0 ? 
					(uint)0 : (uint)myIncubateProcessingTime;
			}
			set
			{
				// Set the incubation time to the supplied value.  NOTE: the 
				// 'text changed' handler also checks to see if the new value
				// is valid.
				txtIncubateProcessingTime.Text = value.ToString();
			}
		}

		#endregion Properties

		#region Data Entry Error Indicators

		private void txtIncubateProcessingTime_TextChanged(object sender, System.EventArgs e)
		{
			ReportCommandDetailChanged();
			if (txtIncubateProcessingTime.TextLength == 0)
			{
				ShowIncubateProcessingTimeError();
			}
			else
			{
				try
				{
					myIncubateProcessingTime = int.Parse(txtIncubateProcessingTime.Text);
					if (myIncubateProcessingTime < 0)
					{
						ShowIncubateProcessingTimeError();
					}
					else
					{
						ClearIncubateProcessingTimeError();
					}
				}
				catch
				{
					myIncubateProcessingTime = -1;
				}
			}
		}

		private void ShowIncubateProcessingTimeError()
		{
			errorIncubateProcessingTime.SetIconAlignment(
				txtIncubateProcessingTime, ErrorIconAlignment.MiddleLeft);
			errorIncubateProcessingTime.SetError(
				txtIncubateProcessingTime, "Incubate processing time must be >= 0");
		}

		private void ClearIncubateProcessingTimeError()
		{
			errorIncubateProcessingTime.SetError(txtIncubateProcessingTime, string.Empty);
		}

		private void IncubateCommandPanel_VisibleChanged(object sender, System.EventArgs e)
		{
			if (this.Visible)
			{
				// Trigger re-evaluation of the error providers
				base.CommandPanel_VisibleChanged(sender, e);
				txtIncubateProcessingTime_TextChanged(sender ,e);
			}
		}

		#endregion Data Entry Error Indicators

        // 2011-09-08 to 2011-09-16 sp various changes
        //     - provide support for use in smaller screen displays (support for scrollbar in other files)
        //     - align and resize panels for more unify displays  
        #region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.lblIncubateProcessingTime = new System.Windows.Forms.Label();
            this.txtIncubateProcessingTime = new System.Windows.Forms.TextBox();
            this.errorIncubateProcessingTime = new System.Windows.Forms.ErrorProvider();
            ((System.ComponentModel.ISupportInitialize)(this.errorIncubateProcessingTime)).BeginInit();
            this.SuspendLayout();
            // 
            // lblIncubateProcessingTime
            // 
            this.lblIncubateProcessingTime.Location = new System.Drawing.Point(186, 56);
            this.lblIncubateProcessingTime.Name = "lblIncubateProcessingTime";
            this.lblIncubateProcessingTime.Size = new System.Drawing.Size(108, 23);
            this.lblIncubateProcessingTime.TabIndex = 4;
            this.lblIncubateProcessingTime.Text = "Incubation Time (s)";
            this.lblIncubateProcessingTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtIncubateProcessingTime
            // 
            this.txtIncubateProcessingTime.Location = new System.Drawing.Point(296, 56);
            this.txtIncubateProcessingTime.Name = "txtIncubateProcessingTime";
            this.txtIncubateProcessingTime.Size = new System.Drawing.Size(112, 20);
            this.txtIncubateProcessingTime.TabIndex = 6;
            this.txtIncubateProcessingTime.TextChanged += new System.EventHandler(this.txtIncubateProcessingTime_TextChanged);
            // 
            // errorIncubateProcessingTime
            // 
            this.errorIncubateProcessingTime.ContainerControl = this;
            // 
            // IncubateCommandPanel
            // 
            this.Controls.Add(this.txtIncubateProcessingTime);
            this.Controls.Add(this.lblIncubateProcessingTime);
            this.EnableExtension = true;
            this.Name = "IncubateCommandPanel";
            this.Size = new System.Drawing.Size(532, 156);
            this.VisibleChanged += new System.EventHandler(this.IncubateCommandPanel_VisibleChanged);
            this.Controls.SetChildIndex(this.lblIncubateProcessingTime, 0);
            this.Controls.SetChildIndex(this.txtIncubateProcessingTime, 0);
            ((System.ComponentModel.ISupportInitialize)(this.errorIncubateProcessingTime)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion
	}
}

