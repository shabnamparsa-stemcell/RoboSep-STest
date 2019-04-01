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
	/// Summary description for RoboSepWaitForm.
	/// </summary>
	public class RoboSepWaitForm : System.Windows.Forms.Form
	{
		private Tesla.OperatorConsoleControls.RoboSepNamedPanel myBorder;
		private System.Windows.Forms.Label lblMessage;
		private System.Windows.Forms.Timer timerBlinkText;
		private System.ComponentModel.IContainer components;

		private string myLabel = "Please wait while RoboSep® refreshes this Protocol database. ";

		public RoboSepWaitForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
	
			myBorder.Text="Loading Profile";
			lblMessage.Text=myLabel;
			Color namedAreaStandardBackground = ColourScheme.GetColour(
				ColourSchemeItem.NamedAreaStandardBackground);
			myBorder.FillColor  = namedAreaStandardBackground;
			lblMessage.BackColor = namedAreaStandardBackground;

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
			this.components = new System.ComponentModel.Container();
			this.myBorder = new Tesla.OperatorConsoleControls.RoboSepNamedPanel();
			this.lblMessage = new System.Windows.Forms.Label();
			this.timerBlinkText = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// myBorder
			// 
			this.myBorder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.myBorder.BackColor = System.Drawing.SystemColors.Control;
			this.myBorder.FillColor = System.Drawing.Color.White;
			this.myBorder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
			this.myBorder.Location = new System.Drawing.Point(0, 0);
			this.myBorder.Name = "myBorder";
			this.myBorder.Size = new System.Drawing.Size(336, 248);
			this.myBorder.TabIndex = 0;
			// 
			// lblMessage
			// 
			this.lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
			this.lblMessage.Location = new System.Drawing.Point(16, 48);
			this.lblMessage.Name = "lblMessage";
			this.lblMessage.Size = new System.Drawing.Size(304, 168);
			this.lblMessage.TabIndex = 1;
			this.lblMessage.Text = "label1";
			this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// timerBlinkText
			// 
			this.timerBlinkText.Tick += new System.EventHandler(this.timerBlinkText_Tick);
			// 
			// RoboSepWaitForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(336, 248);
			this.Controls.Add(this.lblMessage);
			this.Controls.Add(this.myBorder);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "RoboSepWaitForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "RoboSepWaitForm";
			this.ResumeLayout(false);

		}
		#endregion

		public new DialogResult ShowDialog()
		{
			DialogResult result;
			SeparatorGateway.GetInstance().EventsApi.ReportStatus+= new ReportStatusDelegate(AtReportStatus);
			timerBlinkText.Interval = 500;
			lblMessage.Text=myLabel;
			//timerBlinkText.Start();
			result = base.ShowDialog();
			//timerBlinkText.Stop();
			SeparatorGateway.GetInstance().EventsApi.ReportStatus-= new ReportStatusDelegate(AtReportStatus);
			return result;
		}

		private void AtReportStatus(string statusCode, string[] statusMessageValues)
		{
			if(statusCode.Equals("TSC1002"))
			{
				if(statusMessageValues[0].StartsWith("Loading")) //Protocols loaded!!!!"))
				{
					lblMessage.Text=myLabel+'\n'+'\n'+statusMessageValues[0];
					lblMessage.Invalidate();
					
				}
				else
				{
					System.Diagnostics.Debug.WriteLine("---------------!!!"+ statusMessageValues[0]);
					lblMessage.Text=myLabel;
					lblMessage.Visible = true;
					lblMessage.Invalidate();
					DialogResult = DialogResult.OK;
					this.Refresh();
					
				}
			}
		}

		private void timerBlinkText_Tick(object sender, System.EventArgs e)
		{
			lblMessage.Visible = !lblMessage.Visible;
			lblMessage.Invalidate();
		}
	}
}
