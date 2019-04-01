//----------------------------------------------------------------------------
// AboutSubPage
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
// 05/24/05 - modified serial number string - bdr
//
//----------------------------------------------------------------------------
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using Tesla.Common.DrawingUtilities;
using Tesla.Common.ResourceManagement;

using System.Collections.Specialized;
using System.Configuration;

using Tesla.Common.OperatorConsole;

namespace Tesla.OperatorConsoleControls
{
	/// <summary>
	/// Summary description for AboutSubPage.
	/// </summary>
	public class AboutSubPage : Tesla.OperatorConsoleControls.RoboSepSubPage
	{
		private System.Windows.Forms.Panel pnlAboutBackground;
		private Tesla.OperatorConsoleControls.RoboSepButton btnExit;
		private System.Windows.Forms.GroupBox grpVersionInformation;
		private System.Windows.Forms.Label lblUiVersion;
		private System.Windows.Forms.Label lblIcVersion;
		private System.Windows.Forms.Label lblUiVersionNumber;
		private System.Windows.Forms.Label lblIcVersionNumber;
		private System.Windows.Forms.GroupBox grpInstrumentInformation;
		private System.Windows.Forms.Label txtSerialNumber;
		private System.Windows.Forms.Label txtSerialNum;
        private System.Windows.Forms.Label lblInstrument;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label txtServiceAddress;
        private System.Windows.Forms.Label txtServiceAddr;
		private Tesla.OperatorConsoleControls.RoboSepButton btnProductInfo;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#region Construction/destruction

		public AboutSubPage()
			: base(RoboSepSubPage.MdiChild.About)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// Initialise fixed text			
			this.Text = SeparatorResourceManager.GetSeparatorString(StringId.AboutText);
			btnExit.Text = SeparatorResourceManager.GetSeparatorString(StringId.ButtonTextExit);
			btnExit.Role = RoboSepButton.ButtonRole.OK;

			btnProductInfo.Text = "Product Info";//SeparatorResourceManager.GetSeparatorString(StringId.ButtonTextExit);
			btnProductInfo.Role = RoboSepButton.ButtonRole.OK;

			grpVersionInformation.Text = string.Empty;
                lblVersion.Text = SeparatorResourceManager.GetSeparatorString(
                StringId.AboutSoftwareGroupText);            
			lblUiVersion.Text = SeparatorResourceManager.GetSeparatorString(StringId.AboutSoftwareUiText);
			lblIcVersion.Text = SeparatorResourceManager.GetSeparatorString(StringId.AboutSoftareInstrumentControlText);

			grpInstrumentInformation.Text = string.Empty;
            lblInstrument.Text = SeparatorResourceManager.GetSeparatorString(
                StringId.AboutInstrumentGroupText);
			txtSerialNum.Text = SeparatorResourceManager.GetSeparatorString(
				StringId.AboutInstrumentSerialNumberText);
            txtServiceAddr.Text = SeparatorResourceManager.GetSeparatorString(
                StringId.AboutInstrumentServiceConnectionText);
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

		#region UserControl overrides

		private void AboutSubPage_Resize(object sender, System.EventArgs e)
		{
			base.ResizeHandler(sender, e);
		}

        GraphicsPath myBoundingPath = new GraphicsPath();

        private void pnlAboutBackground_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            // Apply subpage defaults
            DrawingUtilities.ApplyStandardContext(e.Graphics);
            DrawingUtilities.RoundedRectanglePath(grpInstrumentInformation.Bounds, 
                DrawingUtilities.RoundingRadius, out myBoundingPath);

            // Get a brush for drawing text
            SolidBrush textBrush = new SolidBrush(
                ColourScheme.GetColour(ColourSchemeItem.NamedAreaTextForeground));

            // Get a string format for left-aligned text, centered vertically
            StringFormat stringFormat = DrawingUtilities.NearCentreStringFormat;

            // Bounding Rectangle for "about" info
            Rectangle txtBounds = new Rectangle();

            // ----- Draw the Instrument info "group box" and associated information -----
            using (Pen borderPen = new Pen(
                       ColourScheme.GetColour(ColourSchemeItem.NamedAreaBoundary), 2.0f))
            {
                borderPen.Alignment = PenAlignment.Center;
                e.Graphics.DrawPath(borderPen, myBoundingPath);
            }          

            // Draw the group label
            txtBounds.Location = new Point(
                grpInstrumentInformation.Location.X + lblInstrument.Location.X,
                grpInstrumentInformation.Location.Y + lblInstrument.Location.Y);          
            txtBounds.Size = lblInstrument.Size;
            using (SolidBrush pnlFillBrush = new SolidBrush(pnlAboutBackground.BackColor))
            {
                SizeF textSize = e.Graphics.MeasureString(lblInstrument.Text, 
                    SeparatorResourceManager.GetFont(SeparatorResourceManager.FontId.SampleProcessingPageTitle),
                    lblInstrument.Width, 
                    stringFormat);
                Rectangle grpLabelArea = new Rectangle(txtBounds.Location,txtBounds.Size);                
                grpLabelArea.Width = (int)textSize.Width;
                e.Graphics.FillRectangle(pnlFillBrush, grpLabelArea);
            }
            e.Graphics.DrawString(lblInstrument.Text,
                SeparatorResourceManager.GetFont(SeparatorResourceManager.FontId.SampleProcessingPageTitle), 
                textBrush, 
                txtBounds,
                stringFormat);  

            if (txtSerialNum.Text != null && txtSerialNum.Text != string.Empty)
            {
                txtBounds.Location = new Point(
                    grpInstrumentInformation.Location.X + txtSerialNum.Location.X,
                    grpInstrumentInformation.Location.Y + txtSerialNum.Location.Y);
                txtBounds.Size     = txtSerialNum.Size;
#if (TESLA_UI_SHOW_BOUNDING_RECTANGLES)
				e.Graphics.DrawRectangle(Pens.CornflowerBlue, txtBounds.Bounds);					
#endif					
                // Add the Name string to the display
                e.Graphics.DrawString(txtSerialNum.Text,
                    SeparatorResourceManager.GetFont(SeparatorResourceManager.FontId.ApplicationDefault), 
                    textBrush, 
                    txtBounds,
                    stringFormat);               				
            }

            if (txtSerialNumber.Text != null && txtSerialNumber.Text != string.Empty)
            {
                txtBounds.Location = new Point(
                    grpInstrumentInformation.Location.X + txtSerialNumber.Location.X,
                    grpInstrumentInformation.Location.Y + txtSerialNumber.Location.Y);
                txtBounds.Size     = txtSerialNumber.Size;
#if (TESLA_UI_SHOW_BOUNDING_RECTANGLES)
				e.Graphics.DrawRectangle(Pens.CornflowerBlue, txtBounds.Bounds);					
#endif					
                // Add the Name string to the display
                e.Graphics.DrawString(txtSerialNumber.Text,
                    SeparatorResourceManager.GetFont(SeparatorResourceManager.FontId.ApplicationDefault), 
                    textBrush, 
                    txtBounds,
                    stringFormat);               				
            }

            if (txtServiceAddr.Text != null && txtServiceAddr.Text != string.Empty)
            {
                txtBounds.Location = new Point(
                    grpInstrumentInformation.Location.X + txtServiceAddr.Location.X,
                    grpInstrumentInformation.Location.Y + txtServiceAddr.Location.Y);
                txtBounds.Size     = txtServiceAddr.Size;
#if (TESLA_UI_SHOW_BOUNDING_RECTANGLES)
				e.Graphics.DrawRectangle(Pens.CornflowerBlue, txtBounds.Bounds);					
#endif					
                // Add the Name string to the display
                e.Graphics.DrawString(txtServiceAddr.Text,
                    SeparatorResourceManager.GetFont(SeparatorResourceManager.FontId.ApplicationDefault), 
                    textBrush, 
                    txtBounds,
                    stringFormat);               				
            }

            if (txtServiceAddress.Text != null)
            {
                // If the Service Connection address is unknown, display an error prompting
                // the user to connect a network cable and re-start.
                if (txtServiceAddress.Text == string.Empty)
                {                     
                    txtServiceAddress.Text = SeparatorResourceManager.GetSeparatorString(
                        StringId.AboutInstrumentConnectNetworkCableText);
                }
                txtBounds.Location = new Point(
                    grpInstrumentInformation.Location.X + txtServiceAddress.Location.X,
                    grpInstrumentInformation.Location.Y + txtServiceAddress.Location.Y);
                txtBounds.Size     = txtServiceAddress.Size;
#if (TESLA_UI_SHOW_BOUNDING_RECTANGLES)
				e.Graphics.DrawRectangle(Pens.CornflowerBlue, txtBounds.Bounds);					
#endif					
                // Add the Name string to the display
                e.Graphics.DrawString(txtServiceAddress.Text,
                    SeparatorResourceManager.GetFont(SeparatorResourceManager.FontId.ApplicationDefault), 
                    textBrush, 
                    txtBounds,
                    stringFormat);               				
            }

            // ----- Draw the Version info "group box" and associated information -----
            DrawingUtilities.RoundedRectanglePath(grpVersionInformation.Bounds, 
                DrawingUtilities.RoundingRadius, out myBoundingPath);
            using (Pen borderPen = new Pen(
                       ColourScheme.GetColour(ColourSchemeItem.NamedAreaBoundary), 2.0f))
            {
                borderPen.Alignment = PenAlignment.Center;
                e.Graphics.DrawPath(borderPen, myBoundingPath);
            }          

            // Draw the group label
            txtBounds.Location = new Point(
                grpVersionInformation.Location.X + lblVersion.Location.X,
                grpVersionInformation.Location.Y + lblVersion.Location.Y);          
            txtBounds.Size = lblVersion.Size;
            using (SolidBrush pnlFillBrush = new SolidBrush(pnlAboutBackground.BackColor))
            {
                SizeF textSize = e.Graphics.MeasureString(lblVersion.Text, 
                    SeparatorResourceManager.GetFont(SeparatorResourceManager.FontId.SampleProcessingPageTitle),
                    lblVersion.Width, 
                    stringFormat);
                Rectangle grpLabelArea = new Rectangle(txtBounds.Location,txtBounds.Size);                
                grpLabelArea.Width = (int)textSize.Width;
                e.Graphics.FillRectangle(pnlFillBrush, grpLabelArea);
            }
            e.Graphics.DrawString(lblVersion.Text,
                SeparatorResourceManager.GetFont(SeparatorResourceManager.FontId.SampleProcessingPageTitle), 
                textBrush, 
                txtBounds,
                stringFormat);  

            if (lblUiVersion.Text != null && lblUiVersion.Text != string.Empty)
            {
                txtBounds.Location = new Point(
                    grpVersionInformation.Location.X + lblUiVersion.Location.X,
                    grpVersionInformation.Location.Y + lblUiVersion.Location.Y);
                txtBounds.Size     = lblUiVersion.Size;
#if (TESLA_UI_SHOW_BOUNDING_RECTANGLES)
				e.Graphics.DrawRectangle(Pens.CornflowerBlue, txtBounds.Bounds);					
#endif					
                // Add the Name string to the display
                e.Graphics.DrawString(lblUiVersion.Text,
                    SeparatorResourceManager.GetFont(SeparatorResourceManager.FontId.ApplicationDefault), 
                    textBrush, 
                    txtBounds,
                    stringFormat);               				
            }


            if (lblUiVersionNumber.Text != null && lblUiVersionNumber.Text != string.Empty)
            {
                txtBounds.Location = new Point(
                    grpVersionInformation.Location.X + lblUiVersionNumber.Location.X,
                    grpVersionInformation.Location.Y + lblUiVersionNumber.Location.Y);
                txtBounds.Size     = lblUiVersionNumber.Size;
#if (TESLA_UI_SHOW_BOUNDING_RECTANGLES)
				e.Graphics.DrawRectangle(Pens.CornflowerBlue, txtBounds.Bounds);					
#endif					
                // Add the Name string to the display
                e.Graphics.DrawString(lblUiVersionNumber.Text,
                    SeparatorResourceManager.GetFont(SeparatorResourceManager.FontId.ApplicationDefault), 
                    textBrush, 
                    txtBounds,
                    stringFormat);               				
            }

            if (lblIcVersion.Text != null && lblIcVersion.Text != string.Empty)
            {
                txtBounds.Location = new Point(
                    grpVersionInformation.Location.X + lblIcVersion.Location.X,
                    grpVersionInformation.Location.Y + lblIcVersion.Location.Y);
                txtBounds.Size     = lblIcVersion.Size;
#if (TESLA_UI_SHOW_BOUNDING_RECTANGLES)
				e.Graphics.DrawRectangle(Pens.CornflowerBlue, txtBounds.Bounds);					
#endif					
                // Add the Name string to the display
                e.Graphics.DrawString(lblIcVersion.Text,
                    SeparatorResourceManager.GetFont(SeparatorResourceManager.FontId.ApplicationDefault), 
                    textBrush, 
                    txtBounds,
                    stringFormat);               				
            }

            if (lblIcVersionNumber.Text != null && lblIcVersionNumber.Text != string.Empty)
            {
                txtBounds.Location = new Point(
                    grpVersionInformation.Location.X + lblIcVersionNumber.Location.X,
                    grpVersionInformation.Location.Y + lblIcVersionNumber.Location.Y);
                txtBounds.Size     = lblIcVersionNumber.Size;
#if (TESLA_UI_SHOW_BOUNDING_RECTANGLES)
				e.Graphics.DrawRectangle(Pens.CornflowerBlue, txtBounds.Bounds);					
#endif					
                // Add the Name string to the display
                e.Graphics.DrawString(lblIcVersionNumber.Text,
                    SeparatorResourceManager.GetFont(SeparatorResourceManager.FontId.ApplicationDefault), 
                    textBrush, 
                    txtBounds,
                    stringFormat);               				
            }

            // Clean up graphics resources
            textBrush.Dispose();
        }

		#endregion UserControl overrides

		#region Control event handlers

		private void btnExit_Click(object sender, System.EventArgs e)
		{
			// Close the GUI (triggers an orderly shutdown of the application & instrument).
			((System.Windows.Forms.Form)this.TopLevelControl).Close();
		}

		private void btnProductInfo_Click(object sender, System.EventArgs e)
		{
			string url = null;

			NameValueCollection nvc = (NameValueCollection)	
				ConfigurationSettings.GetConfig("OperatorConsole/ConsoleConfiguration");
			try
			{
				url = nvc.Get("ProductInfoURL");
			}
			catch
			{
				url = "http://www.stemcell.com/";
			}

			System.Diagnostics.Process process = new System.Diagnostics.Process();
			process.StartInfo.FileName = "rundll32.exe";
			process.StartInfo.Arguments = "url.dll,FileProtocolHandler "+url;
			process.StartInfo.UseShellExecute = true;
			process.Start();
		}
		#endregion Control event handlers

		#region Separator Events

		public sealed override void RegisterForSeparatorEvents()
		{
			if (myEventSink == null)
			{
				base.RegisterForSeparatorEvents();
			}
			if (myEventSink != null)
			{
				myEventSink.ReportInstrumentInformation += new Tesla.Separator.ReportInstrumentInfoDelegate(AtReportInstrumentInformation);
			}
		}

		private void AtReportInstrumentInformation(string gatewayURL, 
            string gatewayInterfaceVersion, string serverUptime_seconds, 
            string instrumentControlVersion, string instrumentSerialNumber, 
            string serviceConnection)
		{
			// Update Instrument Control software version number
			lblIcVersionNumber.Text = instrumentControlVersion;

			// Show Instrument Serial Number
			txtSerialNumber.Text = "Robo-" + instrumentSerialNumber; // bdr

            // Show Service Connection address
            txtServiceAddress.Text = serviceConnection;

			// Show the GUI software version number (in MajorVersion.MinorVersion.BuildNumber format)
			// (so, remove the trailing .Revision number).
			string uiVersionNumber = Application.ProductVersion;
			int revisionPosition = uiVersionNumber.LastIndexOf(".");
			uiVersionNumber = uiVersionNumber.Substring(0, revisionPosition);
			lblUiVersionNumber.Text = uiVersionNumber;
		}

		#endregion SeparatorEvents

		#region Global MDI Support
		
		public override void EnableSelectionAccess(UiAccessMode accessMode)
		{
			btnExit.Enabled = true;
		}

		public override void DisableSelectionAccess(UiAccessMode accessMode)
		{	
			btnExit.Enabled = false;
		}

		#endregion Global MDI Support


		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.pnlAboutBackground = new System.Windows.Forms.Panel();
			this.btnProductInfo = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.grpVersionInformation = new System.Windows.Forms.GroupBox();
			this.lblVersion = new System.Windows.Forms.Label();
			this.lblIcVersionNumber = new System.Windows.Forms.Label();
			this.lblUiVersionNumber = new System.Windows.Forms.Label();
			this.lblIcVersion = new System.Windows.Forms.Label();
			this.lblUiVersion = new System.Windows.Forms.Label();
			this.btnExit = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.grpInstrumentInformation = new System.Windows.Forms.GroupBox();
			this.txtServiceAddress = new System.Windows.Forms.Label();
			this.txtServiceAddr = new System.Windows.Forms.Label();
			this.lblInstrument = new System.Windows.Forms.Label();
			this.txtSerialNumber = new System.Windows.Forms.Label();
			this.txtSerialNum = new System.Windows.Forms.Label();
			this.pnlAboutBackground.SuspendLayout();
			this.grpVersionInformation.SuspendLayout();
			this.grpInstrumentInformation.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlAboutBackground
			// 
			this.pnlAboutBackground.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.pnlAboutBackground.Controls.Add(this.btnProductInfo);
			this.pnlAboutBackground.Controls.Add(this.grpVersionInformation);
			this.pnlAboutBackground.Controls.Add(this.btnExit);
			this.pnlAboutBackground.Controls.Add(this.grpInstrumentInformation);
			this.pnlAboutBackground.Location = new System.Drawing.Point(0, 50);
			this.pnlAboutBackground.Name = "pnlAboutBackground";
			this.pnlAboutBackground.Size = new System.Drawing.Size(620, 360);
			this.pnlAboutBackground.TabIndex = 0;
			this.pnlAboutBackground.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlAboutBackground_Paint);
			// 
			// btnProductInfo
			// 
			this.btnProductInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnProductInfo.Location = new System.Drawing.Point(15, 308);
			this.btnProductInfo.Name = "btnProductInfo";
			this.btnProductInfo.Size = new System.Drawing.Size(120, 48);
			this.btnProductInfo.TabIndex = 5;
			this.btnProductInfo.Visible = false;
			this.btnProductInfo.Click += new System.EventHandler(this.btnProductInfo_Click);
			// 
			// grpVersionInformation
			// 
			this.grpVersionInformation.Controls.Add(this.lblVersion);
			this.grpVersionInformation.Controls.Add(this.lblIcVersionNumber);
			this.grpVersionInformation.Controls.Add(this.lblUiVersionNumber);
			this.grpVersionInformation.Controls.Add(this.lblIcVersion);
			this.grpVersionInformation.Controls.Add(this.lblUiVersion);
			this.grpVersionInformation.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.grpVersionInformation.Location = new System.Drawing.Point(24, 36);
			this.grpVersionInformation.Name = "grpVersionInformation";
			this.grpVersionInformation.Size = new System.Drawing.Size(276, 256);
			this.grpVersionInformation.TabIndex = 3;
			this.grpVersionInformation.TabStop = false;
			this.grpVersionInformation.Visible = false;
			// 
			// lblVersion
			// 
			this.lblVersion.Location = new System.Drawing.Point(12, -8);
			this.lblVersion.Name = "lblVersion";
			this.lblVersion.Size = new System.Drawing.Size(152, 23);
			this.lblVersion.TabIndex = 4;
			this.lblVersion.Text = "Software";
			this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblIcVersionNumber
			// 
			this.lblIcVersionNumber.Location = new System.Drawing.Point(12, 192);
			this.lblIcVersionNumber.Name = "lblIcVersionNumber";
			this.lblIcVersionNumber.Size = new System.Drawing.Size(248, 44);
			this.lblIcVersionNumber.TabIndex = 3;
			// 
			// lblUiVersionNumber
			// 
			this.lblUiVersionNumber.Location = new System.Drawing.Point(12, 76);
			this.lblUiVersionNumber.Name = "lblUiVersionNumber";
			this.lblUiVersionNumber.Size = new System.Drawing.Size(248, 44);
			this.lblUiVersionNumber.TabIndex = 2;
			// 
			// lblIcVersion
			// 
			this.lblIcVersion.Location = new System.Drawing.Point(12, 160);
			this.lblIcVersion.Name = "lblIcVersion";
			this.lblIcVersion.Size = new System.Drawing.Size(248, 23);
			this.lblIcVersion.TabIndex = 1;
			this.lblIcVersion.Text = "Instrument Control";
			// 
			// lblUiVersion
			// 
			this.lblUiVersion.Location = new System.Drawing.Point(12, 44);
			this.lblUiVersion.Name = "lblUiVersion";
			this.lblUiVersion.Size = new System.Drawing.Size(248, 23);
			this.lblUiVersion.TabIndex = 0;
			this.lblUiVersion.Text = "UI";
			// 
			// btnExit
			// 
			this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnExit.Location = new System.Drawing.Point(492, 308);
			this.btnExit.Name = "btnExit";
			this.btnExit.Size = new System.Drawing.Size(120, 48);
			this.btnExit.TabIndex = 2;
			this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
			// 
			// grpInstrumentInformation
			// 
			this.grpInstrumentInformation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.grpInstrumentInformation.Controls.Add(this.txtServiceAddress);
			this.grpInstrumentInformation.Controls.Add(this.txtServiceAddr);
			this.grpInstrumentInformation.Controls.Add(this.lblInstrument);
			this.grpInstrumentInformation.Controls.Add(this.txtSerialNumber);
			this.grpInstrumentInformation.Controls.Add(this.txtSerialNum);
			this.grpInstrumentInformation.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.grpInstrumentInformation.Location = new System.Drawing.Point(320, 36);
			this.grpInstrumentInformation.Name = "grpInstrumentInformation";
			this.grpInstrumentInformation.Size = new System.Drawing.Size(276, 256);
			this.grpInstrumentInformation.TabIndex = 4;
			this.grpInstrumentInformation.TabStop = false;
			this.grpInstrumentInformation.Visible = false;
			// 
			// txtServiceAddress
			// 
			this.txtServiceAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtServiceAddress.Location = new System.Drawing.Point(14, 192);
			this.txtServiceAddress.Name = "txtServiceAddress";
			this.txtServiceAddress.Size = new System.Drawing.Size(248, 44);
			this.txtServiceAddress.TabIndex = 5;
			// 
			// txtServiceAddr
			// 
			this.txtServiceAddr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtServiceAddr.Location = new System.Drawing.Point(14, 160);
			this.txtServiceAddr.Name = "txtServiceAddr";
			this.txtServiceAddr.Size = new System.Drawing.Size(248, 23);
			this.txtServiceAddr.TabIndex = 4;
			this.txtServiceAddr.Text = "ServiceAddress";
			// 
			// lblInstrument
			// 
			this.lblInstrument.Location = new System.Drawing.Point(12, -8);
			this.lblInstrument.Name = "lblInstrument";
			this.lblInstrument.Size = new System.Drawing.Size(152, 23);
			this.lblInstrument.TabIndex = 3;
			this.lblInstrument.Text = "Instrument";
			this.lblInstrument.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtSerialNumber
			// 
			this.txtSerialNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtSerialNumber.Location = new System.Drawing.Point(12, 76);
			this.txtSerialNumber.Name = "txtSerialNumber";
			this.txtSerialNumber.Size = new System.Drawing.Size(248, 44);
			this.txtSerialNumber.TabIndex = 2;
			// 
			// txtSerialNum
			// 
			this.txtSerialNum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtSerialNum.Location = new System.Drawing.Point(12, 44);
			this.txtSerialNum.Name = "txtSerialNum";
			this.txtSerialNum.Size = new System.Drawing.Size(248, 23);
			this.txtSerialNum.TabIndex = 0;
			this.txtSerialNum.Text = "Serial Number";
			// 
			// AboutSubPage
			// 
			this.Controls.Add(this.pnlAboutBackground);
			this.Name = "AboutSubPage";
			this.Size = new System.Drawing.Size(620, 410);
			this.Resize += new System.EventHandler(this.AboutSubPage_Resize);
			this.pnlAboutBackground.ResumeLayout(false);
			this.grpVersionInformation.ResumeLayout(false);
			this.grpInstrumentInformation.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion        
	}
}
