//----------------------------------------------------------------------------
// SeparateCommandPanel
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

namespace Tesla.ProtocolEditorControls
{
	public class SeparateCommandPanel : Tesla.ProtocolEditorControls.CommandPanel
	{
        private System.Windows.Forms.TextBox txtSeparationProcessingTime;
		private System.Windows.Forms.Label lblSeparateProcessingTime;
		private System.Windows.Forms.ErrorProvider errorSeparationProcessingTime;
		private System.ComponentModel.IContainer components = null;

		#region Construction/destruction

		public SeparateCommandPanel()
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
				txtSeparationProcessingTime.Text.Length > 0	&&
				mySeparationProcessingTime > -1;
		}

		private int mySeparationProcessingTime = -1;

		public uint WaitCommandTimeDuration
		{
			get
			{
				return mySeparationProcessingTime < 0 ? 
					(uint)0 : (uint)mySeparationProcessingTime;
			}
			set
			{
				// Set the separation time to the supplied value.  NOTE: the 
				// 'text changed' handler also checks to see if the new value
				// is valid.
				txtSeparationProcessingTime.Text = value.ToString();
			}
		}

		#endregion Properties

		#region Data Entry Error Indicators

		private void txtSeparationProcessingTime_TextChanged(object sender, System.EventArgs e)
		{
			ReportCommandDetailChanged();
			if (txtSeparationProcessingTime.TextLength == 0)
			{
				ShowSeparationProcessingTimeError();
			}
			else
			{
				try
				{
					mySeparationProcessingTime = int.Parse(txtSeparationProcessingTime.Text);
					if (mySeparationProcessingTime < 0)
					{
						ShowSeparationProcessingTimeError();
					}
					else
					{
						ClearSeparationProcessingTimeError();
					}
				}
				catch
				{
					mySeparationProcessingTime = -1;
				}
			}
		}

		private void ShowSeparationProcessingTimeError()
		{
			errorSeparationProcessingTime.SetIconAlignment(
				txtSeparationProcessingTime, ErrorIconAlignment.MiddleLeft);
			errorSeparationProcessingTime.SetError(
				txtSeparationProcessingTime, "Separation processing time must be >= 0");
		}

		private void ClearSeparationProcessingTimeError()
		{
			errorSeparationProcessingTime.SetError(txtSeparationProcessingTime, string.Empty);
		}

		private void SeparateCommandPanel_VisibleChanged(object sender, System.EventArgs e)
		{
			if (this.Visible)
			{
				// Trigger re-evaluation of the error providers
				base.CommandPanel_VisibleChanged(sender, e);
				txtSeparationProcessingTime_TextChanged(sender, e);
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
            this.components = new System.ComponentModel.Container();
            this.txtSeparationProcessingTime = new System.Windows.Forms.TextBox();
            this.lblSeparateProcessingTime = new System.Windows.Forms.Label();
            this.errorSeparationProcessingTime = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorSeparationProcessingTime)).BeginInit();
            this.SuspendLayout();
            // 
            // txtSeparationProcessingTime
            // 
            this.txtSeparationProcessingTime.Location = new System.Drawing.Point(296, 56);
            this.txtSeparationProcessingTime.Name = "txtSeparationProcessingTime";
            this.txtSeparationProcessingTime.Size = new System.Drawing.Size(112, 20);
            this.txtSeparationProcessingTime.TabIndex = 6;
            this.txtSeparationProcessingTime.TextChanged += new System.EventHandler(this.txtSeparationProcessingTime_TextChanged);
            // 
            // lblSeparateProcessingTime
            // 
            this.lblSeparateProcessingTime.Location = new System.Drawing.Point(186, 59);
            this.lblSeparateProcessingTime.Name = "lblSeparateProcessingTime";
            this.lblSeparateProcessingTime.Size = new System.Drawing.Size(108, 23);
            this.lblSeparateProcessingTime.TabIndex = 7;
            this.lblSeparateProcessingTime.Text = "Separation Time (s)";
            // 
            // errorSeparationProcessingTime
            // 
            this.errorSeparationProcessingTime.ContainerControl = this;
            // 
            // SeparateCommandPanel
            // 
            this.Controls.Add(this.txtSeparationProcessingTime);
            this.Controls.Add(this.lblSeparateProcessingTime);
            this.EnableExtension = true;
            this.Name = "SeparateCommandPanel";
            this.Size = new System.Drawing.Size(532, 156);
            this.VisibleChanged += new System.EventHandler(this.SeparateCommandPanel_VisibleChanged);
            this.Controls.SetChildIndex(this.lblSeparateProcessingTime, 0);
            this.Controls.SetChildIndex(this.txtSeparationProcessingTime, 0);
            ((System.ComponentModel.ISupportInitialize)(this.errorSeparationProcessingTime)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion
	}
}

