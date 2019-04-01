//----------------------------------------------------------------------------
// MessageLogSubPage
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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;
using System.Globalization;

using Invetech.ApplicationLog;

using Tesla.Common.DrawingUtilities;
using Tesla.Common.ResourceManagement;

using Tesla.Separator;

namespace Tesla.OperatorConsoleControls
{
	public class MessageLogSubPage : Tesla.OperatorConsoleControls.RoboSepSubPage
	{
		private System.Windows.Forms.Panel pnlMessageLogBackground;
		private Tesla.OperatorConsoleControls.RoboSepGrid myMessageLog;
		private System.Windows.Forms.Panel pnlScrollControl;
		private System.Windows.Forms.Button btnUp;
		private System.Windows.Forms.Button btnDown;
		private Tesla.OperatorConsoleControls.RoboSepButton btnLogs;
		private Tesla.OperatorConsoleControls.RoboSepButton btnReports;
		private System.ComponentModel.IContainer components = null;

		#region Construction/destruction

		public MessageLogSubPage()
			: base(RoboSepSubPage.MdiChild.MessageLog)
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();	

			// Initialise fixed text			
			this.Text = SeparatorResourceManager.GetSeparatorString(
				StringId.MessageLogText);

			// Set colour scheme
			Color activeSubPageBackground = ColourScheme.GetColour(ColourSchemeItem.ActiveSubPageBackground);
			btnUp.BackColor = activeSubPageBackground;
			btnDown.BackColor = activeSubPageBackground;

			btnLogs.Text = "Logs";
			btnLogs.Role = RoboSepButton.ButtonRole.OK;
			btnReports.Text = "Reports";
			btnReports.Role = RoboSepButton.ButtonRole.OK;

			// Configure Samples grid (configure settings that cannot be set via the
			// DataGridTableStyle or need to be set on the DataGrid itself).
			pnlScrollControl.BackColor = activeSubPageBackground;
			myMessageLog.BorderStyle = BorderStyle.None;
			myMessageLog.BackgroundColor = activeSubPageBackground;			
			myMessageLog.BackColor = activeSubPageBackground;
			myMessageLog.AlternatingBackColor = activeSubPageBackground;
			myMessageLog.GridLineColor = ColourScheme.GetColour(ColourSchemeItem.GridLine);
			myMessageLog.ReadOnly = true;
			myMessageLog.ColumnHeadersVisible = true;
			myMessageLog.RowHeadersVisible = false;
			myMessageLog.AllowSorting = false;
			myMessageLog.CaptionVisible = false;
			myMessageLog.HeaderBackColor = activeSubPageBackground;
			myMessageLog.BackgroundColor = activeSubPageBackground;			
			myMessageLog.Font = SeparatorResourceManager.GetFont(SeparatorResourceManager.FontId.ApplicationDefault);
			myMessageLog.HeaderFont = myMessageLog.Font;
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

		#region Control event handlers

		private void MessageLogSubPage_Resize(object sender, System.EventArgs e)
		{
			base.ResizeHandler(sender, e);
		}

		#endregion Control event handlers	

		#region Control overrides

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);

			// Re-load the table style for the Protocol Selection grid (to re-size the
			// columns).
			DefineMessageLogGridStyle();
		}

		#endregion Control overrides

		#region Separator Events

		public sealed override void RegisterForSeparatorEvents()
		{
			if (myEventSink == null)
			{
				base.RegisterForSeparatorEvents();
			}
			if (myEventSink != null)
			{				
				// Register for ISeparatorEvents here (if/as required)
			}

			// Register for SeparatorGateway-specific events
			SeparatorGateway.GetInstance().UpdateMessageLogTable += new MessageLogTableDelegate(AtMessageLogTableUpdate);
		}

		#endregion SeparatorEvents

		#region Message Log

		private void btnUp_Click(object sender, System.EventArgs e)
		{
			myMessageLog.ScrollUp();
			UpdateScrollButtonState();
		}

		private void btnDown_Click(object sender, System.EventArgs e)
		{
			myMessageLog.ScrollDown();
			UpdateScrollButtonState();
		}

		private void UpdateScrollButtonState()
		{
			bool isAtTop = myMessageLog.IsScrollAtTop();
			bool isAtBottom = myMessageLog.IsScrollAtBottom();
			btnUp.Visible = ( ! isAtTop);
			btnDown.Visible = ( ! isAtBottom);
		}	

		/// <summary>
		/// Helper class for TableStyle definition
		/// </summary>
		private class ReadOnlyNoActiveCellColumn : DataGridTextBoxColumn  
		{  
			public ReadOnlyNoActiveCellColumn()
			{
				this.ReadOnly = true;
			}
 
			protected override void Edit(System.Windows.Forms.CurrencyManager source, 
				int rowNum, Rectangle bounds, bool readOnly, 
				string instantText, bool cellIsVisible)  
			{  
				// Do nothing - to avoid the cell textbox area 'flashing' when it
				// receives edit focus (prior to the row then being selected by other code)
			}  

			protected override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, Brush backBrush, Brush foreBrush, bool alignToRight)
			{
				// Apply our standard Graphics settings (so we can get anti-aliasing on the text font)
				DrawingUtilities.ApplyStandardContext(g);

				// Continue with the standard behaviour
				base.Paint(g, bounds, source, rowNum, backBrush, foreBrush, alignToRight);				
			}
		}

		private void DefineMessageLogGridStyle()
		{
			if ( ! myMessageLog.TableStyles.Contains("MessageLogInfo"))
			{
                // NOTE: future versions may need to customise the column width multipliers per 
                // culture/display language.
				Color activeSubPageBackground = ColourScheme.GetColour(ColourSchemeItem.ActiveSubPageBackground);
				Color namedAreaStandardBackground = ColourScheme.GetColour(ColourSchemeItem.NamedAreaStandardBackground);

				DataGridTableStyle dgTableStyle = new DataGridTableStyle();
				dgTableStyle.MappingName = "MessageLogInfo";
				dgTableStyle.PreferredRowHeight = 50;
				dgTableStyle.RowHeadersVisible = false;
				dgTableStyle.ColumnHeadersVisible = true;
				dgTableStyle.AllowSorting = false;

				dgTableStyle.BackColor = activeSubPageBackground;
				dgTableStyle.AlternatingBackColor = activeSubPageBackground;
				dgTableStyle.HeaderBackColor = activeSubPageBackground;
				dgTableStyle.ForeColor = ColourScheme.GetColour(ColourSchemeItem.GridTextForeground);
				dgTableStyle.HeaderForeColor = namedAreaStandardBackground;

				dgTableStyle.GridLineColor = ColourScheme.GetColour(ColourSchemeItem.GridLine);
				dgTableStyle.SelectionBackColor = ColourScheme.GetColour(ColourSchemeItem.ApplicationBackground);
				dgTableStyle.SelectionForeColor = namedAreaStandardBackground;				

				dgTableStyle.ReadOnly = true;								

				DataGridTextBoxColumn TextCol = new ReadOnlyNoActiveCellColumn();
				int cumulativeColumnWidth = 0;
				int columnWidth = 0;

				// Add a dummy first column
				TextCol = new ReadOnlyNoActiveCellColumn();
				TextCol.Width = columnWidth;
				dgTableStyle.GridColumnStyles.Add(TextCol);
			
				TextCol = new ReadOnlyNoActiveCellColumn();				
				TextCol.MappingName = "Time";
				TextCol.HeaderText = SeparatorResourceManager.GetSeparatorString(
					StringId.MessageLogTimeColumnText);
				columnWidth = Convert.ToInt32(myMessageLog.Width*0.30);		
				TextCol.Width = columnWidth;
				TextCol.Format = "g";	// General data/time format (short date and short time pattern)
				TextCol.FormatInfo = DateTimeFormatInfo.CurrentInfo;	// Localise the date/time format
				dgTableStyle.GridColumnStyles.Add(TextCol);
				cumulativeColumnWidth += columnWidth;

				TextCol = new ReadOnlyNoActiveCellColumn();				
				TextCol.MappingName = "Severity";
				TextCol.HeaderText = SeparatorResourceManager.GetSeparatorString(
					StringId.MessageLogSeverityColumnText);
				columnWidth = Convert.ToInt32(myMessageLog.Width*0.15);
				TextCol.Width = columnWidth;
				dgTableStyle.GridColumnStyles.Add(TextCol);
				cumulativeColumnWidth += columnWidth;

				TextCol = new ReadOnlyNoActiveCellColumn();				
				TextCol.MappingName = "Text";
				TextCol.HeaderText = SeparatorResourceManager.GetSeparatorString(
					StringId.MessageLogMessageColumnText);
				TextCol.Width = Convert.ToInt32(myMessageLog.Width - cumulativeColumnWidth);
				dgTableStyle.GridColumnStyles.Add(TextCol);							
					
				myMessageLog.TableStyles.Add(dgTableStyle);						
			}
		}

		// Declare a data view for the message log
		RoboSepDataView myMessageLogDataView;

		private void AtMessageLogTableUpdate(System.Data.DataTable runLogMessages)
		{
			try
			{
				if (myMessageLog.InvokeRequired)
				{
					MessageLogTableDelegate eh = new MessageLogTableDelegate(this.AtMessageLogTableUpdate);
					this.Invoke(eh, new object[]{runLogMessages}); 
				}
				else
				{ 										
					// Remove current entries
					myMessageLog.DataBindings.Clear();

					// Create DataView object based on the supplied table.
					myMessageLogDataView = new RoboSepDataView(myMessageLog, runLogMessages);					
					myMessageLogDataView.Sort = "Time DESC";

					// Bind the grid to the data view
					myMessageLog.SetDataBinding(myMessageLogDataView, string.Empty);
					UpdateMessageLogScrollButtonState();
				}
			}
			catch (Exception ex)
			{
				LogFile.LogException(TraceLevel.Error, ex);
			}
		}

		private void UpdateMessageLogScrollButtonState()
		{
			bool isAtTop = myMessageLog.IsScrollAtTop();
			bool isAtBottom = myMessageLog.IsScrollAtBottom();
			btnUp.Visible = ( ! isAtTop);
			btnDown.Visible = ( ! isAtBottom);
		}	

		#endregion Message Log

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.pnlMessageLogBackground = new System.Windows.Forms.Panel();
			this.btnReports = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.btnLogs = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.pnlScrollControl = new System.Windows.Forms.Panel();
			this.btnUp = new System.Windows.Forms.Button();
			this.btnDown = new System.Windows.Forms.Button();
			this.myMessageLog = new Tesla.OperatorConsoleControls.RoboSepGrid();
			this.pnlMessageLogBackground.SuspendLayout();
			this.pnlScrollControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.myMessageLog)).BeginInit();
			this.SuspendLayout();
			// 
			// pnlMessageLogBackground
			// 
			this.pnlMessageLogBackground.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.pnlMessageLogBackground.Controls.Add(this.btnReports);
			this.pnlMessageLogBackground.Controls.Add(this.btnLogs);
			this.pnlMessageLogBackground.Controls.Add(this.pnlScrollControl);
			this.pnlMessageLogBackground.Controls.Add(this.myMessageLog);
			this.pnlMessageLogBackground.Location = new System.Drawing.Point(0, 50);
			this.pnlMessageLogBackground.Name = "pnlMessageLogBackground";
			this.pnlMessageLogBackground.Size = new System.Drawing.Size(620, 360);
			this.pnlMessageLogBackground.TabIndex = 0;
			// 
			// btnReports
			// 
			this.btnReports.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnReports.Location = new System.Drawing.Point(15, 308);
			this.btnReports.Name = "btnReports";
			this.btnReports.Size = new System.Drawing.Size(120, 48);
			this.btnReports.TabIndex = 0;
			this.btnReports.Click += new System.EventHandler(this.btnReports_Click);
			// 
			// btnLogs
			// 
			this.btnLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnLogs.Location = new System.Drawing.Point(492, 308);
			this.btnLogs.Name = "btnLogs";
			this.btnLogs.Size = new System.Drawing.Size(120, 48);
			this.btnLogs.TabIndex = 3;
			this.btnLogs.Click += new System.EventHandler(this.btnLogs_Click);
			// 
			// pnlScrollControl
			// 
			this.pnlScrollControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pnlScrollControl.BackColor = System.Drawing.Color.SandyBrown;
			this.pnlScrollControl.Controls.Add(this.btnUp);
			this.pnlScrollControl.Controls.Add(this.btnDown);
			this.pnlScrollControl.Location = new System.Drawing.Point(491, 8);
			this.pnlScrollControl.Name = "pnlScrollControl";
			this.pnlScrollControl.Size = new System.Drawing.Size(120, 48);
			this.pnlScrollControl.TabIndex = 1;
			// 
			// btnUp
			// 
			this.btnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnUp.BackColor = System.Drawing.Color.SandyBrown;
			this.btnUp.Font = new System.Drawing.Font("Webdings", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(2)));
			this.btnUp.Location = new System.Drawing.Point(8, 9);
			this.btnUp.Name = "btnUp";
			this.btnUp.Size = new System.Drawing.Size(48, 31);
			this.btnUp.TabIndex = 9;
			this.btnUp.Text = "5";
			this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
			// 
			// btnDown
			// 
			this.btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDown.BackColor = System.Drawing.Color.SandyBrown;
			this.btnDown.Font = new System.Drawing.Font("Webdings", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(2)));
			this.btnDown.Location = new System.Drawing.Point(64, 9);
			this.btnDown.Name = "btnDown";
			this.btnDown.Size = new System.Drawing.Size(48, 31);
			this.btnDown.TabIndex = 8;
			this.btnDown.Text = "6";
			this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
			// 
			// myMessageLog
			// 
			this.myMessageLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.myMessageLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.myMessageLog.CaptionFont = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.myMessageLog.ColumnHeaderHeight = 18;
			this.myMessageLog.DataMember = "";
			this.myMessageLog.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.myMessageLog.Location = new System.Drawing.Point(19, 56);
			this.myMessageLog.Name = "myMessageLog";
			this.myMessageLog.Size = new System.Drawing.Size(592, 232);
			this.myMessageLog.TabIndex = 0;
			// 
			// MessageLogSubPage
			// 
			this.Controls.Add(this.pnlMessageLogBackground);
			this.Name = "MessageLogSubPage";
			this.Size = new System.Drawing.Size(620, 410);
			this.Resize += new System.EventHandler(this.MessageLogSubPage_Resize);
			this.pnlMessageLogBackground.ResumeLayout(false);
			this.pnlScrollControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.myMessageLog)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion		

		private void btnReports_Click(object sender, System.EventArgs e)
		{
			string url = "C:\\Program Files\\STI\\RoboSep\\reports\\";
			System.Diagnostics.Process process = new System.Diagnostics.Process();
			process.StartInfo.FileName = "rundll32.exe";
			process.StartInfo.Arguments = "url.dll,FileProtocolHandler "+url;
			process.StartInfo.UseShellExecute = true;
			process.Start();
		}

		private void btnLogs_Click(object sender, System.EventArgs e)
		{
			string url = "C:\\Program Files\\STI\\RoboSep\\logs\\";
			System.Diagnostics.Process process = new System.Diagnostics.Process();
			process.StartInfo.FileName = "rundll32.exe";
			process.StartInfo.Arguments = "url.dll,FileProtocolHandler "+url;
			process.StartInfo.UseShellExecute = true;
			process.Start();
		}
	}
}

