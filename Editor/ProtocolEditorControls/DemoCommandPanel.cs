//----------------------------------------------------------------------------
// DemoCommandPanel
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
	public class DemoCommandPanel : Tesla.ProtocolEditorControls.CommandPanel
	{
        private System.Windows.Forms.TextBox txtIterationsCount;
		private System.Windows.Forms.Label lblIterationCount;
		private System.Windows.Forms.ErrorProvider errorIterationsCount;
		private System.ComponentModel.IContainer components = null;

		#region Construction/destruction

		public DemoCommandPanel()
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
			return base.IsContentValid()				&&
				txtIterationsCount.Text.Length > 0		&&
				myIterationsCount > 0;
		}

		private uint myIterationsCount = 0;

		public uint DemoCommandIterationCount
		{
			get
			{
				return myIterationsCount;
			}
			set
			{
				// Set the iterations count to the supplied value.  NOTE: the 
				// 'text changed' handler also checks to see if the new value
				// is valid.
				txtIterationsCount.Text = value.ToString();
			}
		}

		#endregion Properties

		#region Data Entry Error Indicators

		private void txtIterationsCount_TextChanged(object sender, System.EventArgs e)
		{
			ReportCommandDetailChanged();
			if (txtIterationsCount.TextLength == 0)
			{
				ShowIterationsCountError();
			}
			else
			{
				try
				{
					myIterationsCount = uint.Parse(txtIterationsCount.Text);
					if (myIterationsCount < 1)
					{
						ShowIterationsCountError();
					}
					else
					{
						ClearIterationsCountError();
					}
				}
				catch
				{
					myIterationsCount = 0;
				}
			}
		}

		private void ShowIterationsCountError()
		{
			errorIterationsCount.SetIconAlignment(
				txtIterationsCount, ErrorIconAlignment.MiddleLeft);
			errorIterationsCount.SetError(
				txtIterationsCount, "Iterations count must be >= 1");
		}

		private void ClearIterationsCountError()
		{
			errorIterationsCount.SetError(txtIterationsCount, string.Empty);
		}
		
		protected void DemoCommandPanel_VisibleChanged(object sender, System.EventArgs e)
		{
			if (this.Visible)
			{
				// Trigger re-evaluation of the error providers
				base.CommandPanel_VisibleChanged(sender, e);
				txtIterationsCount_TextChanged(sender, e);
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
            this.txtIterationsCount = new System.Windows.Forms.TextBox();
            this.lblIterationCount = new System.Windows.Forms.Label();
            this.errorIterationsCount = new System.Windows.Forms.ErrorProvider();
            ((System.ComponentModel.ISupportInitialize)(this.errorIterationsCount)).BeginInit();
            this.SuspendLayout();
            // 
            // txtIterationsCount
            // 
            this.txtIterationsCount.Location = new System.Drawing.Point(296, 56);
            this.txtIterationsCount.Name = "txtIterationsCount";
            this.txtIterationsCount.Size = new System.Drawing.Size(112, 20);
            this.txtIterationsCount.TabIndex = 10;
            this.txtIterationsCount.TextChanged += new System.EventHandler(this.txtIterationsCount_TextChanged);
            // 
            // lblIterationCount
            // 
            this.lblIterationCount.Location = new System.Drawing.Point(186, 59);
            this.lblIterationCount.Name = "lblIterationCount";
            this.lblIterationCount.Size = new System.Drawing.Size(64, 23);
            this.lblIterationCount.TabIndex = 9;
            this.lblIterationCount.Text = "Iterations";
            // 
            // errorIterationsCount
            // 
            this.errorIterationsCount.ContainerControl = this;
            // 
            // DemoCommandPanel
            // 
            this.Controls.Add(this.txtIterationsCount);
            this.Controls.Add(this.lblIterationCount);
            this.EnableExtension = true;
            this.Name = "DemoCommandPanel";
            this.Size = new System.Drawing.Size(532, 156);
            this.VisibleChanged += new System.EventHandler(this.DemoCommandPanel_VisibleChanged);
            this.Controls.SetChildIndex(this.lblIterationCount, 0);
            this.Controls.SetChildIndex(this.txtIterationsCount, 0);
            ((System.ComponentModel.ISupportInitialize)(this.errorIterationsCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion
	}
}

