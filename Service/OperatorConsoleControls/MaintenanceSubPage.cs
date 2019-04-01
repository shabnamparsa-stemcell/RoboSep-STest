//----------------------------------------------------------------------------
// MaintenanceSubPage
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
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

using Invetech.ApplicationLog;

using Tesla.Common.DrawingUtilities;
using Tesla.Common.ResourceManagement;
using Tesla.Common.OperatorConsole;
using Tesla.Common.Protocol;
using Tesla.Common.Separator;

using Tesla.Separator;

using Tesla.DataAccess;
using System.Data;

namespace Tesla.OperatorConsoleControls
{
    public class MaintenanceSubPage : Tesla.OperatorConsoleControls.RoboSepSubPage
    {
        private System.Windows.Forms.Panel pnlMaintenanceBackground;
        private Tesla.OperatorConsoleControls.RoboSepNamedPanel myShutdownPanel;
        private Tesla.OperatorConsoleControls.RoboSepNamedPanel myMaintenancePanel;
        private Tesla.OperatorConsoleControls.RoboSepItemList myMaintenanceItems;
        private Tesla.OperatorConsoleControls.RoboSepItemList myShutdownItems;
		private Tesla.OperatorConsoleControls.RoboSepButton btnService;
		private Tesla.OperatorConsoleControls.RoboSepButton btnPackage;
        private System.ComponentModel.IContainer components = null;

        #region Construction/destruction

        public MaintenanceSubPage()
            : base(RoboSepSubPage.MdiChild.Maintenance)
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            // Initialise colour scheme
			Color activeSubPageBackground= ColourScheme.GetColour(ColourSchemeItem.ActiveSubPageBackground);
			myMaintenancePanel.BackColor = activeSubPageBackground;
			myShutdownPanel.BackColor	 = activeSubPageBackground;

            myMaintenancePanel.FillColor = ColourScheme.GetColour(ColourSchemeItem.NamedAreaStandardBackground);
            myMaintenanceItems.BackColor = myMaintenancePanel.FillColor;
            myShutdownPanel.FillColor = myMaintenancePanel.FillColor;					
            myShutdownItems.BackColor = myMaintenancePanel.FillColor;

			btnService.Text = "Service";
			btnService.Role = RoboSepButton.ButtonRole.Warning;
			btnPackage.Text = "Package for Diagnosis";
			btnPackage.Role = RoboSepButton.ButtonRole.Warning;
			SeparatorGateway.GetInstance().UpdateChosenProtocolTable += new SampleTableDelegate(AtUpdateChosenProtocolTableUpdate);
			

            // Initialise fixed text
            this.Text = SeparatorResourceManager.GetSeparatorString(
                StringId.MaintenanceText);
            myMaintenancePanel.Text = SeparatorResourceManager.GetSeparatorString(
                StringId.MaintenancePanelText);
            myShutdownPanel.Text = SeparatorResourceManager.GetSeparatorString(
                StringId.ShutdownPanelText);

            // Register for control events
            myMaintenanceItems.InvokeItemAction += new Tesla.OperatorConsoleControls.RoboSepItemList.InvokeItemActionDelegate(InvokeMaintenanceAction);
            myShutdownItems.InvokeItemAction += new Tesla.OperatorConsoleControls.RoboSepItemList.InvokeItemActionDelegate(InvokeShutdownAction);			
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

		#region Global MDI Support
		
		public override void EnableSelectionAccess(UiAccessMode accessMode)
		{
			// Enable interaction with the Maintenance and Shutdown ItemLists
			myMaintenanceItems.Enabled = true;
			myShutdownItems.Enabled = true;
		}

		public override void DisableSelectionAccess(UiAccessMode accessMode)
		{	
			// Disable interaction with the Maintenance and Shutdown ItemLists
			myMaintenanceItems.Enabled = false;
			myShutdownItems.Enabled = false;
		}

		private void AtUpdateChosenProtocolTableUpdate(DataTable chosenProtocols, ProtocolMix protocolMix)
		{
			//check if table empty...	
			//enable if empty else disable
			if(chosenProtocols.Rows.Count > 0)
			{
				btnService.Enabled = false;
			}
			else
			{
				btnService.Enabled = true;
			}
		}
		#endregion Global MDI Support
		
		#region Events

		public event UiPageJumpDelegate			JumpToUiPage;

		public event ShutdownInitiatedDelegate	ReportShutdownInitiated;

		#endregion Events

        #region Control event handlers	

        private void MaintenanceSubPage_Resize(object sender, System.EventArgs e)
        {
            base.ResizeHandler(sender, e);		
			myMaintenancePanel.Left = 2 * DrawingUtilities.MinimumTabMargin;
			myMaintenancePanel.Width = (pnlMaintenanceBackground.Width - 3 * 
                DrawingUtilities.MinimumTabMargin)/2;			
            myShutdownPanel.Width = myMaintenancePanel.Width;
            myShutdownPanel.Left = myMaintenancePanel.Width + 
                2 * DrawingUtilities.MinimumTabMargin;
        }		

        private void InvokeMaintenanceAction(object actionContext)
        {
            IMaintenanceProtocol maintenanceProtocol = actionContext as IMaintenanceProtocol;
            if (maintenanceProtocol != null)
            {
                // NOTE: Currently, we're treating Separation and Maintenance protocols as mutually
                // exclusive.

                // Select the appropriate maintenance protocol.
                // NOTE: deselection of any previous Separation protocols to effect the 
                // mutual exclusion described above is handled in by the "SelectProtocol" 
				// routine.							
                mySeparator.SelectProtocol(QuadrantId.Quadrant1, 
                    maintenanceProtocol);
				// Schedule immediately
				mySeparator.ScheduleRun(QuadrantId.Quadrant1);
				LogFile.AddMessage(TraceLevel.Info, "Selected " + maintenanceProtocol.Label);

				// Jump to the Sample Processing/Run page
				if (JumpToUiPage != null)
				{
					JumpToUiPage(UiPage.Run);
				}
            }
        }

        private void InvokeShutdownAction(object actionContext)
        {
            IShutdownProtocol shutdownProtocol = actionContext as IShutdownProtocol;
            if (shutdownProtocol != null)
            {				
				// Select & immediately run the shutdown protocol.
                mySeparator.Shutdown(shutdownProtocol);
				LogFile.AddMessage(TraceLevel.Info, "Invoked " + shutdownProtocol.Label);

				// Alert interested parties that shutdown has been initiated.  We use an
				// event here to prevent circular dependency between the OperatorConsole
				// and OperatorConsoleControls projects (as essentially we need to call
				// OperatorConsole to process the shutdown correctly).
				if (ReportShutdownInitiated != null)
				{
					ReportShutdownInitiated();
				}

				// Jump to the Sample Processing/Run page (not essential but just for
				// consistency since we are now running the shutdown protocol).
				if (JumpToUiPage != null)
				{
					JumpToUiPage(UiPage.Run);
				}
            }
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
                myEventSink.UpdateShutdownProtocolList += new ShutdownProtocolListDelegate(AtShutdownProtocolListUpdate);
                myEventSink.UpdateMaintenanceProtocolList += new MaintenanceProtocolListDelegate(AtMaintenanceProtocolListUpdate);
            }
        }

        private void AtShutdownProtocolListUpdate(IShutdownProtocol[] shutdownProtocols)
        {
            try
            {
                if (myShutdownPanel.InvokeRequired)
                {
                    ShutdownProtocolListDelegate eh = new ShutdownProtocolListDelegate(this.AtShutdownProtocolListUpdate);
                    this.Invoke(eh, new object[]{shutdownProtocols});
                }
                else
                {					
                    // Assume required shutdown protocols are not defined, then enable their
                    // use if they are actually present in the configured list of shutdown
                    // protocols.
                    myShutdownItems.Clear();

                    int shutdownProtocolCount = shutdownProtocols.GetLength(0);
                    if (shutdownProtocols == null || shutdownProtocolCount <= 0)
                    {
                        // Instrument must always be configured with at least one shutdown 
                        // protocol.
                        throw new ApplicationException(
                            SeparatorResourceManager.GetSeparatorString(
                            StringId.ExMissingMandatoryProtocol));
                    }
                    else 
                    {                        
                        // Add supplied Shutdown protocols to the display
                        RoboSepListItem[] listItems = new RoboSepListItem[shutdownProtocolCount];
                        for (int i = 0; i < shutdownProtocolCount; ++i)								
                        {
                            IShutdownProtocol protocol  = shutdownProtocols[i];
                            listItems[i] = new RoboSepListItem();
                            listItems[i].Text = protocol.Label;
                            listItems[i].Tag  = protocol;
                        }
                        myShutdownItems.AddRange(listItems);
                    }
                }
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
        }

        private void AtMaintenanceProtocolListUpdate(IMaintenanceProtocol[] maintenanceProtocols)
        {
            try
            {
                if (myMaintenancePanel.InvokeRequired)
                {
                    MaintenanceProtocolListDelegate eh = new MaintenanceProtocolListDelegate(this.AtMaintenanceProtocolListUpdate);
                    this.Invoke(eh, new object[]{maintenanceProtocols});
                }
                else
                {					
                    // Assume required maintenance protocols are not defined, then enable their
                    // use if they are actually present in the configured list of maintenance
                    // protocols.
                    myMaintenanceItems.Clear();

                    int maintenanceProtocolCount = maintenanceProtocols.GetLength(0);
                    if (maintenanceProtocols != null && maintenanceProtocolCount > 0)
                    {
                        // Check that all required Maintenance protocols are supplied

                        // Add supplied Shutdown protocols to the display
                        RoboSepListItem[] listItems = new RoboSepListItem[maintenanceProtocolCount];
                        for (int i = 0; i < maintenanceProtocolCount; ++i)								
                        {
                            IMaintenanceProtocol protocol  = maintenanceProtocols[i];
                            listItems[i] = new RoboSepListItem();
                            listItems[i].Text = protocol.Label;
                            listItems[i].Tag  = protocol;
                        }
                        myMaintenanceItems.AddRange(listItems);
                    }
                }
            }
            catch (Exception ex)
            {
                LogFile.LogException(TraceLevel.Error, ex);
            }
        }

        #endregion SeparatorEvents

        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.pnlMaintenanceBackground = new System.Windows.Forms.Panel();
			this.btnService = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.myShutdownPanel = new Tesla.OperatorConsoleControls.RoboSepNamedPanel();
			this.myShutdownItems = new Tesla.OperatorConsoleControls.RoboSepItemList();
			this.myMaintenancePanel = new Tesla.OperatorConsoleControls.RoboSepNamedPanel();
			this.myMaintenanceItems = new Tesla.OperatorConsoleControls.RoboSepItemList();
			this.btnPackage = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.pnlMaintenanceBackground.SuspendLayout();
			this.myShutdownPanel.SuspendLayout();
			this.myMaintenancePanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlMaintenanceBackground
			// 
			this.pnlMaintenanceBackground.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.pnlMaintenanceBackground.Controls.Add(this.btnPackage);
			this.pnlMaintenanceBackground.Controls.Add(this.btnService);
			this.pnlMaintenanceBackground.Controls.Add(this.myShutdownPanel);
			this.pnlMaintenanceBackground.Controls.Add(this.myMaintenancePanel);
			this.pnlMaintenanceBackground.Location = new System.Drawing.Point(0, 50);
			this.pnlMaintenanceBackground.Name = "pnlMaintenanceBackground";
			this.pnlMaintenanceBackground.Size = new System.Drawing.Size(620, 360);
			this.pnlMaintenanceBackground.TabIndex = 4;
			// 
			// btnService
			// 
			this.btnService.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnService.Location = new System.Drawing.Point(492, 308);
			this.btnService.Name = "btnService";
			this.btnService.Size = new System.Drawing.Size(120, 48);
			this.btnService.TabIndex = 3;
			this.btnService.Click += new System.EventHandler(this.btnService_Click);
			// 
			// myShutdownPanel
			// 
			this.myShutdownPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
			this.myShutdownPanel.BackColor = System.Drawing.SystemColors.Control;
			this.myShutdownPanel.Controls.Add(this.myShutdownItems);
			this.myShutdownPanel.FillColor = System.Drawing.Color.White;
			this.myShutdownPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
			this.myShutdownPanel.Location = new System.Drawing.Point(315, 10);
			this.myShutdownPanel.Name = "myShutdownPanel";
			this.myShutdownPanel.Size = new System.Drawing.Size(300, 272);
			this.myShutdownPanel.TabIndex = 0;
			// 
			// myShutdownItems
			// 
			this.myShutdownItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.myShutdownItems.BackColor = System.Drawing.Color.SandyBrown;
			this.myShutdownItems.IsMultiSelect = false;
			this.myShutdownItems.Location = new System.Drawing.Point(18, 25);
			this.myShutdownItems.Name = "myShutdownItems";
			this.myShutdownItems.Size = new System.Drawing.Size(268, 234);
			this.myShutdownItems.TabIndex = 2;
			// 
			// myMaintenancePanel
			// 
			this.myMaintenancePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
			this.myMaintenancePanel.BackColor = System.Drawing.SystemColors.Control;
			this.myMaintenancePanel.Controls.Add(this.myMaintenanceItems);
			this.myMaintenancePanel.FillColor = System.Drawing.Color.White;
			this.myMaintenancePanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
			this.myMaintenancePanel.Location = new System.Drawing.Point(10, 10);
			this.myMaintenancePanel.Name = "myMaintenancePanel";
			this.myMaintenancePanel.Size = new System.Drawing.Size(300, 272);
			this.myMaintenancePanel.TabIndex = 1;
			// 
			// myMaintenanceItems
			// 
			this.myMaintenanceItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.myMaintenanceItems.BackColor = System.Drawing.Color.SandyBrown;
			this.myMaintenanceItems.IsMultiSelect = false;
			this.myMaintenanceItems.Location = new System.Drawing.Point(18, 25);
			this.myMaintenanceItems.Name = "myMaintenanceItems";
			this.myMaintenanceItems.Size = new System.Drawing.Size(264, 234);
			this.myMaintenanceItems.TabIndex = 2;
			// 
			// btnPackage
			// 
			this.btnPackage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnPackage.Location = new System.Drawing.Point(16, 308);
			this.btnPackage.Name = "btnPackage";
			this.btnPackage.Size = new System.Drawing.Size(120, 48);
			this.btnPackage.TabIndex = 4;
			this.btnPackage.Click += new System.EventHandler(this.btnPackage_Click);
			// 
			// MaintenanceSubPage
			// 
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.pnlMaintenanceBackground);
			this.Name = "MaintenanceSubPage";
			this.Size = new System.Drawing.Size(620, 410);
			this.Resize += new System.EventHandler(this.MaintenanceSubPage_Resize);
			this.pnlMaintenanceBackground.ResumeLayout(false);
			this.myShutdownPanel.ResumeLayout(false);
			this.myMaintenancePanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
        #endregion

		private void btnService_Click(object sender, System.EventArgs e)
		{
			//string url = "C:\\Program Files\\STI\\RoboSep\\Service\\Service.exe";
			System.Diagnostics.Process process = new System.Diagnostics.Process();
			process.StartInfo.FileName = "C:\\Program Files\\STI\\RoboSep\\Service\\Service.exe"; //"rundll32.exe";
			process.StartInfo.WorkingDirectory = "C:\\Program Files\\STI\\RoboSep\\Service\\";
			//process.StartInfo.Arguments = "url.dll,FileProtocolHandler "+url;
			process.StartInfo.UseShellExecute = true;
			process.Start();
		}

		private void btnPackage_Click(object sender, System.EventArgs e)
		{
			//prompt for destination

			DateTime t =  DateTime.Now; 
			string sDestFile;
			sDestFile ="DiagnosticData-"+t.Year+"-"+t.Month+"-"+t.Day+"-"+t.Hour+"-"+t.Minute+"-"+t.Second;

			SaveFileDialog saveDialog = new SaveFileDialog();
			saveDialog.FileName=sDestFile;
			saveDialog.Filter=@"Package Files (*.pk)|*.pk|All Files (*.*)|*.*||";
			saveDialog.FilterIndex=1;
			saveDialog.RestoreDirectory=true;

			if (saveDialog.ShowDialog() != DialogResult.OK )
				return;

			sDestFile="\""+saveDialog.FileName+"\"";

			//int result=system("C:/Program Files/STI/RoboSep/Service/package.bat "+sDestFile);
			//MessageBox("Package Completed.","Service");

			//return;
			
			string serviceDir="C:\\Program Files\\STI\\RoboSep\\Service";

			try
			{
				StreamReader re = File.OpenText(serviceDir+"\\packagelist.txt");
				string line = null;
				string param="";
				while((line =re.ReadLine())!=null)
				{
					//OutputDebugString("AAA"+line);
					param+="\""+line+"\" ";
				}
				re.Close();

				//Package files and directories here.
				System.Diagnostics.Process process = new System.Diagnostics.Process();
				process.StartInfo.FileName = serviceDir+"\\zip"; //"rundll32.exe";
				process.StartInfo.WorkingDirectory = serviceDir;
				process.StartInfo.Arguments = " -9r "+sDestFile+" "+param;;
				process.StartInfo.UseShellExecute = true;
				process.Start();

				process.WaitForExit();
				MessageBox.Show("Package Completed.","Service");
			}
			catch
			{
				MessageBox.Show("Source files not specified.");
			}

			
		}
			
    }
}