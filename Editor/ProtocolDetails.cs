//----------------------------------------------------------------------------
// FrmProtocolDetails
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
//----------------------------------------------------------------------------
// ***MAJOR CHANGES***
//----------------------------------------------------------------------------
// IAT - Nov 2005 - Added new feature: editor determines if each transport,
//       mix, top up, and resuspend command is within specific limits before allowing
//       the user to save the protocol.  Limit logic:
//		-if source or destination is a 1mL vial (eg. antibody cocktail), TotalVolume must be < 1100. 
//		-if neither source nor destination is a 1mL vial TotalVolume < 5000mL
// Total volume is determined as follows:
//		-if Relative Proportion checkbox is checked, Total Volume = (Max Sample Volume)*(Relative Proportion)
//		-if Absolute Volume checkbox is checked, Total Volume - Absolute Volume
//		-if neither checkboxes are checked, Total Volume = Max Sample Volume
// PRIMARY METHODS: CalculatedVolumeTooHigh()
//					CheckAllCommands()
//				(both in CommandChecking Region)
//----------------------------------------------------------------------------
// IAT - Nov 2005 - Added new feature: copy button
// Copies currently selected (highlighted) command, and adds the copy to the Command List.
// PRIMARY METHODS: CopyCommandPanel()
//					btnCopy_Click(...)
//----------------------------------------------------------------------------
// IAT - Nov 2005 - Changed Protocol Type combobox to display Type instead of variable name:
//					eg. "TopUpVialCommand" is now displayed as "Top Up Vial"
// PRIMARY METHODS: FrmProtocolDetails_Load()
//
//
//     03/27/06 - delete commands prompt check protocol - RL
//     03/27/06 - adjust quad usage offset hack fix - RL
//     03/29/06 - pause command - RL (look for PauseCommand)
//     03/30/06 - Protocol Type Sync - RL
//	   09/05/07 - usebuffertip command - bdr
//     10/02/07 - check/bypass for maintenance protocols - bdr
//
//----------------------------------------------------------------------------
//
// 2011-09-05 to 2011-09-29 sp various changes
//     - provide support for use in smaller screen displays (support for scrollbar in other files)
//     - align and resize panels for more unify displays  
//     - compatibility issues for upgrading and operating in .NET 2.0 environement
//     - provide support for different volume error checking dialog  
//     - add checking for recommended volume levels and provide warnings
//     - add volume level thresholds (recommended and acceptable) using parameter file entries instead of fixed code  
//     - add checking of absolute volume levels 
//     - modify add command function from append to end of list to insert at current position
//     - fixed delete function such that it sets the current index to the position deleted instead of the beginning 
//
//----------------------------------------------------------------------------
//
//----------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

using Tesla.Common.Protocol;
using Tesla.Common.Separator;
using Tesla.Common.ProtocolCommand;
using Tesla.Common.ResourceManagement;
using Tesla.ProtocolEditorControls;
using Tesla.ProtocolEditorModel;
using System.Collections.Generic;
using Tesla.DataAccess;


namespace Tesla.ProtocolEditor
{
	/// <summary>
	/// Summary description for ProtocolDetails.
	/// </summary>
	public class FrmProtocolDetails : System.Windows.Forms.Form
	{
		private static ProtocolModel	 theProtocolModel;
        private static ProtocolCustomize myFrmProtocolCustomize;
        private static ProtocolReagentBarcodes myFrmProtocolReagentBarcodes;

		private System.Windows.Forms.Panel pnlCommandList;
		private System.Windows.Forms.ListBox lstCommands;
		private System.Windows.Forms.Panel pnlHeaderInfo;
		private System.Windows.Forms.Panel pnlCommandDetail;
		private System.Windows.Forms.Label lblHeader;
		private System.Windows.Forms.Label lblCommandList;
		private System.Windows.Forms.Label lblCommandDetails;
		
		private ProtocolEditorControls.CommandPanel[]	myCommandPanels;
		private ProtocolEditorControls.CommandPanel		myCurrentCommandPanel;
		private System.Windows.Forms.Button				btnMoveUp;
		private System.Windows.Forms.Button				btnMoveDown;		
		private System.Windows.Forms.Button				btnAddCommand;
		private System.Windows.Forms.Button				btnDeleteCommand;
		private System.Windows.Forms.ComboBox			cmbCommandTypes;
		private System.Windows.Forms.ComboBox			cmbProtocolType;
        private System.Windows.Forms.Label lblProtocolType;
		private System.Windows.Forms.Splitter			splCommands;

		private Tesla.ProtocolEditorControls.CommandPanel			myCommandPanel;
		private Tesla.ProtocolEditorControls.DemoCommandPanel		myDemoCommandPanel;
		private Tesla.ProtocolEditorControls.IncubateCommandPanel	myIncubateCommandPanel;
		private Tesla.ProtocolEditorControls.SeparateCommandPanel	mySeparateCommandPanel;
        private Tesla.ProtocolEditorControls.PauseCommandPanel myPauseCommandPanel;
        private Tesla.ProtocolEditorControls.MixCommandPanel myVolumeCommandPanel;
        private Tesla.ProtocolEditorControls.GenericMultiStepCommandPanel myTopUpMixTransSepTransCommandPanel;

		private Tesla.ProtocolEditorControls.VolumeMaintenanceCommandPanel myVolumeMaintenanceCommandPanel;
		private Tesla.ProtocolEditorControls.WorkingVolumeCommandPanel     myWorkingVolumeCommandPanel;
		private System.ComponentModel.IContainer components;


		#region Constants

		//private const int MAX_VOLUME_UL = 10000;
		private const int MIN_VOLUME_UL = 1; // IAT - set to one b/c 0 causes a runtime error.
        private bool copyFlag = false;
        private string LABEL_TOOLTIP_RSS = "Appears in protocol selection window. Should have format: <species> <cell type> <protocol type> <5 digit catalogID>-<optional 4 digit subID> - <short description> valid character sets  + - _ ( ) and upper and lower case alphanumerics";
        private string LABEL_TOOLTIP_RS16 = "Appears in protocol selection window. Should have format: <species> <cell type> <protocol type> <5 digit catalogID>-<4 digit subID> - <short description> valid character sets  + - _ ( ) and upper and lower case alphanumerics";
        private string LABEL_ICON_INFO_RSS = "Appears in protocol selection window. Should have format: <species> <cell type> <protocol type> <5 digit catalogID>-<optional 4 digit subID> - <short description> valid character sets  + - _ ( ) and upper and lower case alphanumerics";
        private string LABEL_ICON_INFO_RS16 = "Appears in protocol selection window. Should have format: <species> <cell type> <protocol type> <5 digit catalogID>-<4 digit subID> - <short description> valid character sets  + - _ ( ) and upper and lower case alphanumerics";

		#endregion Constants

		#region Types

		public enum OpeningMode
		{
			FileNew,
			FileOpen
		}
/*
		private enum ReagentVial
		{
			Cocktail = 0,
			Particle,
			Antibody,
			TOTAL
		}
*/
		private enum ReagentVial
		{
			Antibody = 0,
			Cocktail,
			Particle,
			TOTAL
		}

		#endregion Types

		#region Protocol Details events

		public delegate void EditModeChangedDelegate(bool isEditMode);

		public event EditModeChangedDelegate	ReportEditMode;

		private bool isFormDirty;
		private bool isFormChangedSinceLastSave = false;
		private System.Windows.Forms.ErrorProvider errorProtocolLabel;
		private System.Windows.Forms.ErrorProvider errorProtocolAuthor;
		private System.Windows.Forms.Button btnValidateProtocol;
		private System.Windows.Forms.Label lblValidateResult;
		private bool isEditModeEnabled = true;
		private System.Windows.Forms.Label lblSampleVolume;
		private System.Windows.Forms.Label lblSampleVolumeMin;
		private System.Windows.Forms.Label lblSampleVolumeMax;
        private System.Windows.Forms.TextBox txtSampleVolumeMin;
        private System.Windows.Forms.TextBox txtSampleVolumeMax;
		private System.Windows.Forms.ErrorProvider errorSampleVolumeMin;
		private System.Windows.Forms.ErrorProvider errorSampleVolumeMax;
		private System.Windows.Forms.ErrorProvider errorCommandSequence;
        private System.Windows.Forms.TextBox txtWorkingVolumeSampleThreshold;
		private System.Windows.Forms.Label lblWorkingVolumeSampleThreshold;
		private System.Windows.Forms.Label lblHighVolume;
		private System.Windows.Forms.Label lblLowVolume;
		private System.Windows.Forms.ErrorProvider errorWorkingVolumeThreshold;
		private System.Windows.Forms.ErrorProvider errorWorkingVolumeLow;
		private System.Windows.Forms.ErrorProvider errorWorkingVolumeHigh;
        private System.Windows.Forms.TextBox txtWorkingVolumeHigh;
        private System.Windows.Forms.TextBox txtWorkingVolumeLow;
		private System.Windows.Forms.Label lblProtocolDesc;
        public ProtocolLabelDecriptionAndAuthorTextbox txtProtocolDesc;
        //public System.Windows.Forms.TextBox txtProtocolLabel;
        public ProtocolLabelDecriptionAndAuthorTextbox txtProtocolLabel;
		private System.Windows.Forms.Label lblProtocolLabel;
		private System.Windows.Forms.Button btnHide;
		private System.Windows.Forms.Button btnShow;
		private System.Windows.Forms.ToolTip tipCheckProtocol;
		private System.Windows.Forms.ToolTip tipName;
		private System.Windows.Forms.ToolTip tipDescription;
		private System.Windows.Forms.ToolTip tipMaximumSample;
		private System.Windows.Forms.ToolTip tipWorking;
		private System.Windows.Forms.ToolTip tipWorkingLow;
		private System.Windows.Forms.ToolTip tipWorkingHigh;
		private System.Windows.Forms.ToolTip tipMinimumSample;		 
		private System.Windows.Forms.Button btnConfirmCommand;
		private System.Windows.Forms.ToolTip tipConfirmCommand;
		private int myQuadrantCount;      
		private System.Windows.Forms.Button btnCopy;
		private System.Windows.Forms.Button btnAutoFill;      
		private bool txtBoxEnabled = false;
        // 2011-09-19 sp
        // obtain constants from parameter file
        public VolumeLimits volumeLimits;

        private Splitter splHeader;
        private GenericMultiStepCommandPanel myTopUpMixTransCommandPanel;
        private GenericMultiStepCommandPanel myTopUpTransCommandPanel;
        private GenericMultiStepCommandPanel myTopUpTransSepTransCommandPanel;
        private GenericMultiStepCommandPanel myResusMixSepTransCommandPanel;
        private GenericMultiStepCommandPanel myResusMixCommandPanel;
        private GenericMultiStepCommandPanel2 myMixTransCommandPanel;
        private Label lblRobosepType;
        private Button btnLabelInfo;
        private Button btnProtocolSummary;
        
        private bool IsEditModeEnabled
		{
			get
			{
				return isEditModeEnabled;
			}
			set
			{
				isEditModeEnabled = value;
			}
		}

		private void EnterEditMode()
		{
			if (isEditModeEnabled)
			{
				isFormDirty = true;
				if ( ! isFormChangedSinceLastSave)
				{
					isFormChangedSinceLastSave = true;
				}

            	if (myWorkingVolumeCommandPanel.Visible == true) 
					myWorkingVolumeCommandPanel.CheckTransportSourceVial();

				lblValidateResult.Text = "´";   // Wingdings question mark
				DbgView("EnterEditMode - UpdateEditMode()");
				UpdateEditMode();
			}

		}

		private void ExitEditMode()
		{
			isFormDirty = false;
			UpdateEditMode();
		}

		private void UpdateEditMode()
		{
           	DbgView("UpdateEditMode");
			if (ReportEditMode != null)
			{
            	DbgView("UpdateEditMode - call ReportEditMode()");
			  	ReportEditMode(isFormDirty);
			}
		}

		#endregion Protocol Details events

		#region Construction/destruction

        //private int[] defaultEndResult = new int[2];        // 2011-11-09 sp -- support for storage of default end-result location
        private double[] WorkingSampleMinAddition = new double[4];
        private double[] WorkingSampleMaxAddition = new double[4];
        private double[] WorkingSeparationMinAddition = new double[4];
        private double[] WorkingSeparationMaxAddition = new double[4];
        private double[] maxCocktailNeeded = new double[4];
        private double[] maxParticlesNeeded = new double[4];
        private double[] maxAntibodyNeeded = new double[4];
        private double[] maxNegFractionVolume = new double[4];
        private double[] maxWasteVolume = new double[4];

		// compact debugmsg utility fn	  // bdr
		public void DbgView(string msg) 
        { 	
//			System.Diagnostics.Debug.WriteLine(msg); 
		}

		public FrmProtocolDetails()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            volumeLimits = VolumeLimits.GetInstance();

			//Create tool tips
			SetupToolTips();

			// Get a local reference to the protocol model. 
			theProtocolModel = ProtocolModel.GetInstance();


            //update panels (remove unused tabs)
            myTopUpMixTransCommandPanel.initForTopUpMixTransCommandPanel();
            myTopUpTransCommandPanel.initForTopUpTransCommandPanel();
            myTopUpTransSepTransCommandPanel.initForTopUpTransSepTransCommandPanel();
            myResusMixSepTransCommandPanel.initForResusMixSepTransCommandPanel();
            myResusMixCommandPanel.initForResusMixCommandPanel();

			// Create the various types of command detail panels that may be hosted by this
			// form.
			myCommandPanels = new CommandPanel[(int)CommandPanel.PanelType.NUM_COMMAND_PANEL_TYPES];
			for (CommandPanel.PanelType cp = CommandPanel.PanelType.START_TYPE;
				cp < CommandPanel.PanelType.NUM_COMMAND_PANEL_TYPES; ++cp)
			{
				switch (cp)
				{
					case CommandPanel.PanelType.CommandPanel:
						myCommandPanels[(int)CommandPanel.PanelType.CommandPanel] = myCommandPanel;
						break;
					case CommandPanel.PanelType.DemoCommandPanel:
						myCommandPanels[(int)CommandPanel.PanelType.DemoCommandPanel] = myDemoCommandPanel;
						break;
					case CommandPanel.PanelType.IncubateCommandPanel:
						myCommandPanels[(int)CommandPanel.PanelType.IncubateCommandPanel] = myIncubateCommandPanel;
						break;
					case CommandPanel.PanelType.SeparateCommandPanel:
						myCommandPanels[(int)CommandPanel.PanelType.SeparateCommandPanel] = mySeparateCommandPanel;
						break;
					case CommandPanel.PanelType.VolumeCommandPanel:
						myCommandPanels[(int)CommandPanel.PanelType.VolumeCommandPanel] = myVolumeCommandPanel;
						break;
					case CommandPanel.PanelType.WorkingVolumeCommandPanel:
						myCommandPanels[(int)CommandPanel.PanelType.WorkingVolumeCommandPanel] = myWorkingVolumeCommandPanel;
						break;
					case CommandPanel.PanelType.VolumeMaintenanceCommandPanel:
						myCommandPanels[(int)CommandPanel.PanelType.VolumeMaintenanceCommandPanel] = myVolumeMaintenanceCommandPanel;
                        break;
                    case CommandPanel.PanelType.PauseCommandPanel:
                        myCommandPanels[(int)CommandPanel.PanelType.PauseCommandPanel] = myPauseCommandPanel;
                        break;
                    case CommandPanel.PanelType.TopUpMixTransSepTransCommandPanel:
                        myCommandPanels[(int)CommandPanel.PanelType.TopUpMixTransSepTransCommandPanel] = myTopUpMixTransSepTransCommandPanel;
                        break;
                    case CommandPanel.PanelType.TopUpMixTransCommandPanel:
                        myCommandPanels[(int)CommandPanel.PanelType.TopUpMixTransCommandPanel] = myTopUpMixTransCommandPanel;
                        break;
                    case CommandPanel.PanelType.TopUpTransSepTransCommandPanel:
                        myCommandPanels[(int)CommandPanel.PanelType.TopUpTransSepTransCommandPanel] = myTopUpTransSepTransCommandPanel;
                        break;
                    case CommandPanel.PanelType.TopUpTransCommandPanel:
                        myCommandPanels[(int)CommandPanel.PanelType.TopUpTransCommandPanel] = myTopUpTransCommandPanel;
                        break;
                    case CommandPanel.PanelType.ResusMixSepTransCommandPanel:
                        myCommandPanels[(int)CommandPanel.PanelType.ResusMixSepTransCommandPanel] = myResusMixSepTransCommandPanel;
                        break;
                    case CommandPanel.PanelType.ResusMixCommandPanel:
                        myCommandPanels[(int)CommandPanel.PanelType.ResusMixCommandPanel] = myResusMixCommandPanel;
                        break;
                    case CommandPanel.PanelType.MixTransCommandPanel:
                        myCommandPanels[(int)CommandPanel.PanelType.MixTransCommandPanel] = myMixTransCommandPanel;
                        break;
                    
				}
			}																			

			// Position each command panel  
			for (int i = 0; i < (int)CommandPanel.PanelType.NUM_COMMAND_PANEL_TYPES; ++i)
			{				
				myCommandPanels[i].Visible = false;				
				myCommandPanels[i].Parent = pnlCommandDetail;
				myCommandPanels[i].Location = new Point(0, pnlCommandDetail.Height - myCommandPanels[i].Height);		

			}

			// register for the CommandDetailChanged event from the command panels
			CommandPanel.CommandDetailChanged += new Tesla.ProtocolEditorControls.CommandPanel.CommandDetailChangedDelegate(WhenCommandDetailChanged);
		}

		private void SetupToolTips()
		{

            tipCheckProtocol.SetToolTip(this.btnValidateProtocol, "Needed prior to saving protocol.");
            //tipName.SetToolTip(this.lblProtocolLabel, "Appears in protocol selection window. Should be similar to file name.");
            tipName.SetToolTip(this.lblProtocolLabel, SeparatorResourceManager.isPlatformRS16() ? LABEL_TOOLTIP_RS16 : LABEL_TOOLTIP_RSS);
		tipDescription.SetToolTip(this.lblProtocolDesc, "Description of application.");
		tipMinimumSample.SetToolTip(this.lblSampleVolumeMin, "Minimum sample volume.");
		tipMaximumSample.SetToolTip(this.lblSampleVolumeMax, "Maximum sample volume.");
		tipWorking.SetToolTip(this.lblWorkingVolumeSampleThreshold, "Sample volume at which top-up/resuspend switch point occurs from low to high volume.");
		tipWorkingLow.SetToolTip(this.lblLowVolume, "Low top-up/resuspend volume.");
		tipWorkingHigh.SetToolTip(this.lblHighVolume, "High top-up/resuspend volume.");
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


		//----------------------------------------------------------------------------------- 
		// Description:
		//   Protocol types are contained in the ProtocolClass enum in Common\ ProtocolTypes.cs 	  	
		//     Positive,
		//     Negative,
		//     HumanPositive,
		//     HumanNegative,
		//     MousePositive,
		//     MouseNegative,
		//     WholeBloodPositive,
		//     WholeBloodNegative,
		//     Maintenance,
		//     Shutdown,
		//     Undefined
		//
		// Setup the protocol type list and command types list
		//----------------------------------------------------------------------------------- 

        private string[] commandDisplayNames;
		private void FrmProtocolDetails_Load(object sender, System.EventArgs e)
		{
			// initialise the list of Protocol types 			
			this.cmbProtocolType.SelectedIndexChanged -= new System.EventHandler(this.cmbProtocolType_SelectedIndexChanged);

			// removed: cmbProtocolType.Items.AddRange(Enum.GetNames(typeof(Tesla.Common.Protocol.ProtocolClass)));

			// !! hard coded enum list - may need to change..bdr
			// adding to types to dropdown list: THIS IS ORDER SPECIFIC

			// RL - Protocol Type Sync - 03/30/06
			// match order in protocol types AND RoboSepProtocol.xsd
			cmbProtocolType.Items.Add("Positive");  			         
			cmbProtocolType.Items.Add("Negative");  			         
			cmbProtocolType.Items.Add("Human Positive");  			     
			cmbProtocolType.Items.Add("Mouse Positive");  			     
			cmbProtocolType.Items.Add("Human Negative");  			     
			cmbProtocolType.Items.Add("Mouse Negative");  			     
			cmbProtocolType.Items.Add("Whole Blood Positive");           
			cmbProtocolType.Items.Add("Whole Blood Negative");           
			cmbProtocolType.Items.Add("Undefined");  			         
			cmbProtocolType.Items.Add("Maintenance");  			         
			cmbProtocolType.Items.Add("Shutdown");  			         

			cmbProtocolType.MaxDropDownItems = cmbProtocolType.Items.Count;
			cmbProtocolType.SelectedIndex = 1;	
			
            string dbg = string.Format("Types combobox ttl items = {0}", cmbProtocolType.Items.Count);
            DbgView(dbg);
            
			this.cmbProtocolType.SelectedIndexChanged += new System.EventHandler(this.cmbProtocolType_SelectedIndexChanged);
			cmbProtocolType.SelectedIndex = 0;      

			// initialise the list of Command types			
			string[] commandNames = Enum.GetNames(typeof(SubCommand));
			//foreach (string name in commandNames) 
            for(int i=0; i<commandNames.GetLength(0);i++)
			{
                string name = commandNames[i];
				// remove the "Command" suffix
				string commandName = name.Remove(name.Length - 7, 7);

				// RL pause command (DISABLE)
				//if(commandName == "Pause")
				//	continue;

				// insert spaces for multi-word names
				string cmdDisplayName = commandName;
				for (int index = commandName.Length - 1; index > 0; --index) 
				{
					if (char.IsUpper(commandName[index])) 
					{
						cmdDisplayName = cmdDisplayName.Insert(index, " ");
					}
				}

                commandNames[i] = cmdDisplayName;

                //skip if enable user add combo command is false
                if (i < (int)(SubCommand.TopUpMixTransSepTransCommand) || EnableUserAddComboCommands())
                {
                    // add the command name to the display list
                    cmbCommandTypes.Items.Add(cmdDisplayName);
                }
			}
            commandDisplayNames = commandNames;
			cmbCommandTypes.MaxDropDownItems = commandNames.GetLength(0);
			cmbCommandTypes.SelectedIndex = 0;	// By default display the first list item

		}
		private void FrmProtocolDetails_EnabledChanged(object sender, System.EventArgs e)
		{
			if (this.Enabled)
			{
                btnProtocolSummary.Visible = true;  // 2011-10-03 sp -- added
                btnValidateProtocol.Visible = true;
                lblValidateResult.Visible = true;

				btnHide.Visible = true;

				// Trigger display of the error providers in the case of a new protocol
				if (myCommandSequence == null || myCommandSequence.GetLength(0) == 0)
				{
					ShowCommandSequenceError();
					if (txtProtocolLabel.TextLength == 0)
					{
						ShowProtocolLabelError();
					}
					if (txtSampleVolumeMin.TextLength == 0)
					{
						ShowSampleVolumeMinError();
					}
					if (txtSampleVolumeMax.TextLength == 0)
					{
						ShowSampleVolumeMaxError(0);
					}
				}
			}
			else
			{
                btnProtocolSummary.Visible = false;  // 2011-10-03 sp -- added
                btnValidateProtocol.Visible = false;
				lblValidateResult.Visible = false;
			}
		}
				
		#endregion Form events

        // 2011-09-29 sp
        // added provision to insert command after current selection
        private void insertCommandIntoSequence(object commandToInsert)
        {
            // Transpose this current command sequence entry with the one above it.
            ArrayList commandSequence = new ArrayList();
            commandSequence.AddRange(myCommandSequence);
            int commandToMoveIndex = myCurrentCommandIndex + 1;
            commandSequence.Insert(commandToMoveIndex, commandToInsert);
            myCommandSequence = (IProtocolCommand[])commandSequence.ToArray(typeof(IProtocolCommand));
            RecalculateCommandSequence();
            lstCommands.DataSource = myCommandSequence;
            lstCommands.SelectedIndex = commandToMoveIndex;
            myCurrentCommandIndex = commandToMoveIndex;
        }

		#region Control events


		private void btnMoveUp_Click(object sender, System.EventArgs e)
		{
			// Transpose this current command sequence entry with the one above it.
			ArrayList commandSequence = new ArrayList();
			commandSequence.AddRange(myCommandSequence);
			object commandToMove = commandSequence[myCurrentCommandIndex];
			int commandToMoveIndex = myCurrentCommandIndex-1;
			commandSequence.RemoveAt(myCurrentCommandIndex);
			commandSequence.Insert(commandToMoveIndex, commandToMove);
			myCommandSequence = (IProtocolCommand[]) commandSequence.ToArray(typeof(IProtocolCommand));
			RecalculateCommandSequence();
			lstCommands.DataSource = myCommandSequence;
			lstCommands.SelectedIndex = commandToMoveIndex;
		}

		private void btnMoveDown_Click(object sender, System.EventArgs e)
		{
			// Transpose this current command sequence entry with the one below it.
			ArrayList commandSequence = new ArrayList();
			commandSequence.AddRange(myCommandSequence);
			object commandToMove = commandSequence[myCurrentCommandIndex];
			int commandToMoveIndex = myCurrentCommandIndex+1;
			int cmdToMoveIndex = myCurrentCommandIndex+1;
			commandSequence.RemoveAt(myCurrentCommandIndex);
			commandSequence.Insert(commandToMoveIndex, commandToMove);
			myCommandSequence = (IProtocolCommand[]) commandSequence.ToArray(typeof(IProtocolCommand));
			RecalculateCommandSequence();
			lstCommands.DataSource = myCommandSequence;
			lstCommands.SelectedIndex = commandToMoveIndex; 
		}

		private void btnAddCommand_Click(object sender, System.EventArgs e)
		{
			// Prevent changes to the command sequence (other than delete!) while we are
			// processing this add request.  NOTE: we do not use SetAddingMode(true) until
			// we have actually added the new item to the command sequence, otherwise the
			// command detail will not be shown/updated properly.

			btnConfirmCommand.Visible = true;
			btnAddCommand.Enabled = false;
			btnMoveDown.Enabled = false;
			btnMoveUp.Enabled = false;
			btnCopy.Enabled = false;
			cmbCommandTypes.Enabled = false;      

			/*CR - if and else statement*/
			bool enabled;
			CommandPanel.PanelType panelType;

			if (myCommandSequence.Length==0)
			{
				enabled = txtBoxEnabled;
			}
			else
			{
				try
				{
					panelType = CommandPanelTypeForCommandSubtype(myCurrentCommandIndex);
					enabled = myCommandPanels[(int)panelType].EnableExtension;
				}
				catch(Exception)
				{
					enabled = false;
				}
			}

			SubCommand concreteCommandType;
			
			     
			// if btnCopy was pressed (as opposed to the Add button), new command is assigned 
			// selected command's values.
			if(copyFlag)
			{
				concreteCommandType = (SubCommand)myCommandSequence[myCurrentCommandIndex].CommandSubtype;
			}
				
				// Create a new blank command of the type specified by the current
				// cmbCommandTypes selection, after the current command sequence item.
			else
			{
				concreteCommandType = (SubCommand)cmbCommandTypes.SelectedIndex;
			}

			IProtocolCommand concreteCommand = null;
			
			switch(concreteCommandType)
			{
				default:
				case SubCommand.HomeAllCommand:
					concreteCommand = new ProtocolHomeAllCommand();
					myCommandPanel.CommandType = "Home All";      
					break;
				case SubCommand.DemoCommand:
					concreteCommand = new ProtocolDemoCommand();
					concreteCommand.CommandExtensionTime = 0;      
					myDemoCommandPanel.CommandType = "Demo";      
					break;
				case SubCommand.PumpLifeCommand:
					concreteCommand = new ProtocolPumpLifeCommand();
					concreteCommand.CommandExtensionTime = 0;      
					myDemoCommandPanel.CommandType = "PumpLife";      
					break;
				case SubCommand.IncubateCommand:
					concreteCommand = new ProtocolIncubateCommand();
					concreteCommand.CommandExtensionTime = 215;      
					myIncubateCommandPanel.CommandType = "Incubate";      
					break;
				case SubCommand.SeparateCommand:
					concreteCommand = new ProtocolSeparateCommand();
					concreteCommand.CommandExtensionTime = 215;      
					mySeparateCommandPanel.CommandType = "Separate";
					break;
				case SubCommand.MixCommand:	
					concreteCommand = new ProtocolMixCommand();
					concreteCommand.CommandExtensionTime = 0;      
					myVolumeCommandPanel.CommandType = "Mix";      
					break;
				case SubCommand.TransportCommand:
					concreteCommand = new ProtocolTransportCommand();
					concreteCommand.CommandExtensionTime = 0;      
					myWorkingVolumeCommandPanel.CommandType = "Transport";      
					myWorkingVolumeCommandPanel.CheckTransportDestVial();      
					break;
				case SubCommand.TopUpVialCommand:
					concreteCommand = new ProtocolTopUpVialCommand();
					concreteCommand.CommandExtensionTime = 120;      
					myWorkingVolumeCommandPanel.CommandType = "Top Up Vial";      
					myWorkingVolumeCommandPanel.CheckTransportDestVial();      
					break;
				case SubCommand.ResuspendVialCommand:
					concreteCommand = new ProtocolResuspendVialCommand();
					concreteCommand.CommandExtensionTime = 120;      
					myWorkingVolumeCommandPanel.CommandType = "Resuspend Vial";      
					myWorkingVolumeCommandPanel.CheckTransportDestVial();      
					break;
				case SubCommand.FlushCommand:
					concreteCommand = new ProtocolFlushCommand();
					myVolumeMaintenanceCommandPanel.CommandType = "Flush";
					break;
				case SubCommand.PrimeCommand:
					concreteCommand = new ProtocolPrimeCommand();
					myVolumeMaintenanceCommandPanel.CommandType = "Prime";
					break;
				case SubCommand.PauseCommand:
					concreteCommand = new ProtocolPauseCommand();
					concreteCommand.CommandExtensionTime = 1;      
					myPauseCommandPanel.CommandType = "Pause";
					break;

                case SubCommand.TopUpMixTransSepTransCommand:
                    concreteCommand = new ProtocolTopUpMixTransSepTransCommand();
                    concreteCommand.CommandExtensionTime = 120;
                    myTopUpMixTransSepTransCommandPanel.CommandType = "TopUpMixTransSepTrans";
                    //myTopUpMixTransSepTransCommandPanel.CheckCommandForError();      
                    break;
                case SubCommand.TopUpMixTransCommand:
                    concreteCommand = new ProtocolTopUpMixTransCommand();
                    concreteCommand.CommandExtensionTime = 120;
                    myTopUpMixTransCommandPanel.CommandType = "TopUpMixTrans";
                    //myTopUpMixTransCommandPanel.CheckCommandForError();      
                    break;
                case SubCommand.TopUpTransSepTransCommand:
                    concreteCommand = new ProtocolTopUpTransSepTransCommand();
                    concreteCommand.CommandExtensionTime = 120;
                    myTopUpTransSepTransCommandPanel.CommandType = "TopUpTransSepTrans";
                    //myTopUpTransSepTransCommandPanel.CheckCommandForError();      
                    break;
                case SubCommand.TopUpTransCommand:
                    concreteCommand = new ProtocolTopUpTransCommand();
                    concreteCommand.CommandExtensionTime = 120;
                    myTopUpTransCommandPanel.CommandType = "TopUpTrans";
                    //myTopUpTransCommandPanel.CheckCommandForError();      
                    break;

                case SubCommand.ResusMixSepTransCommand:
                    concreteCommand = new ProtocolResusMixSepTransCommand();
                    concreteCommand.CommandExtensionTime = 120;
                    myResusMixSepTransCommandPanel.CommandType = "ResusMixSepTrans";
                    //myResusMixSepTransCommandPanel.CheckCommandForError();      
                    break;
                case SubCommand.ResusMixCommand:
                    concreteCommand = new ProtocolResusMixCommand();
                    concreteCommand.CommandExtensionTime = 120;
                    myResusMixCommandPanel.CommandType = "ResusMix";
                    //myResusMixCommandPanel.CheckCommandForError();      
                    break;
                case SubCommand.MixTransCommand:
                    concreteCommand = new ProtocolMixTransCommand();
                    concreteCommand.CommandExtensionTime = 120;
                    myMixTransCommandPanel.CommandType = "MixTrans";
                    //myMixTransCommandPanel.CheckCommandForError();      
                    break;
			}
			
			int copyIndex = myCurrentCommandIndex;      

			// Add the new command to the command sequence
			ArrayList commandSequence = new ArrayList();
			if (myCommandSequence != null && myCommandSequence.GetLength(0) > 0)
			{
				commandSequence.AddRange(myCommandSequence);
			}

            // added 2011-09-06 sp
            // provide support for volume checking for each command
            if (copyFlag)
            {
                concreteCommand.CommandCheckStatus = VolumeCheck.VOLUMES_UNCHECKED;
                concreteCommand.CommandStatus = "";
            }
            else
            {
                concreteCommand.CommandCheckStatus = VolumeCheck.VOLUMES_WARNINGS;
                concreteCommand.CommandStatus = "missing parameters";
            }

            // 2011-09-29 sp
            // replace append function with insert function
            // commandSequence.Add(concreteCommand);
			// myCommandSequence = (IProtocolCommand[]) commandSequence.ToArray(typeof(IProtocolCommand));
            insertCommandIntoSequence(concreteCommand);
			
			//RL moved from the bottom to here... IAT's bug (ignore default value)
			//bug: copy -> add -> reset panel -> isCurrentCommandDirty = true -> lstCommands_SelectedIndexChanged 
			// current (new) command invalid -> index jumps back to old command...
			//fix: copy panel before lstCommands_SelectedIndexChanged
			//well... u can never add while adding so the rest of the code is ok although it doesn't make sense
			
			ResetCommandPanel(concreteCommandType);      
			if(copyFlag)
			{
				CopyCommandPanel(concreteCommandType, copyIndex);     
				copyFlag = false; // reset flag
			}
			else
			{
                for (int i = 0; i < myCommandPanels.GetLength(0); ++i)
                {
                    VolumeCommandPanel panel = myCommandPanels[i] as VolumeCommandPanel;
                    if (panel != null)
                    {
                        panel.CheckTipRack = false;		    //CWJ ADD	
                        panel.TipRackSpecified = false;		//CWJ ADD
                        panel.TipRack = 1;					//CWJ ADD
                    }
                }    
			}

			RecalculateCommandSequence();

            // 2011-09-29 sp
            // replace append function with insert function; command index is not incremented
            /*
			// Explicitly update the current command index.  Normally this is done only
			// after we've checked that the current command is valid (that is we allow
			// the user to change position in the list only if the current command has
			// been validated).  In this case we know the new command hasn't been 
			// validated yet, so we want to restrict movement until it has been (or is
			// deleted).
			int newCommandSequenceCount = myCommandSequence.GetLength(0);
			myCurrentCommandIndex = newCommandSequenceCount-1;
			// Now, re-bind the command list to the new command sequence 
			lstCommands.DataSource = myCommandSequence;
			lstCommands.DisplayMember = "CommandSummary";      
			lstCommands.SelectedIndex = newCommandSequenceCount-1;
             */
            // Repeat the relevant flags/state to indicate the new item is 'dirty' (yet 
			// to be validated)
			SetAddingMode(true);

			/*CR - show and bring panel to front*/
			panelType = 
				CommandPanelTypeForCommandSubtype(myCurrentCommandIndex);
			myCommandPanels[(int)panelType].Visible = true;
			myCommandPanels[(int)panelType].BringToFront();

			      
			myCommandPanels[(int)panelType].EnableExtension = enabled;

            // 2011-09-29 sp
            // Enable command sequence item actions if appropriate
            // EnableCommandSequenceItemActions(newCommandSequenceCount);
            EnableCommandSequenceItemActions(myCommandSequence.GetLength(0));
            
		}

		      	private void ResetCommandPanel(SubCommand concreteCommandType)
				{
					switch (concreteCommandType)
					{
						default:
						case SubCommand.HomeAllCommand:	
							myCommandPanel.CommandLabel = "";
							myCommandPanel.CommandExtensionTime = 0;
							break;

						case SubCommand.DemoCommand:
						case SubCommand.PumpLifeCommand:
							myDemoCommandPanel.DemoCommandIterationCount = 0;
							myDemoCommandPanel.CommandLabel = "";
							myDemoCommandPanel.CommandExtensionTime = 0;
							break;

						case SubCommand.IncubateCommand:
							myIncubateCommandPanel.WaitCommandTimeDuration = 0;
							myIncubateCommandPanel.CommandLabel = "";
							myIncubateCommandPanel.CommandExtensionTime = 215;
							break;

						case SubCommand.SeparateCommand:
							mySeparateCommandPanel.WaitCommandTimeDuration = 0;
							mySeparateCommandPanel.CommandLabel = "";
							mySeparateCommandPanel.CommandExtensionTime = 215;
							break;

                        case SubCommand.MixCommand:
                            myVolumeCommandPanel.cmbDestinationVial.Enabled = false;
                            myVolumeCommandPanel.SetVolumeCommandPanelParams(AbsoluteResourceLocation.TPC0001, AbsoluteResourceLocation.TPC0001,
                               VolumeType.NotSpecified, 0, 0,
                               1, false, "", 0);

							myVolumeCommandPanel.IsVolumeSpecificationRequired = true; // Ensure the relative/absolute value is set before setting this flag.	
							myVolumeCommandPanel.MixCycles = 3;
							myVolumeCommandPanel.TipTubeBottomGap = 0;
							myVolumeCommandPanel.txtTipTubeBottomGap.Enabled = false;
							break;

                        case SubCommand.TransportCommand:
                        case SubCommand.TopUpVialCommand:
                        case SubCommand.ResuspendVialCommand:
                            uint extentionTime = (uint)((concreteCommandType == SubCommand.TransportCommand) ? 0 : 120);
                            myWorkingVolumeCommandPanel.SetVolumeCommandPanelParams(AbsoluteResourceLocation.TPC0001, AbsoluteResourceLocation.TPC0001,
                               VolumeType.NotSpecified, 0, 0,
                               1, false, "", extentionTime);

                            myWorkingVolumeCommandPanel.cmbSourceVial.Enabled = (concreteCommandType == SubCommand.TransportCommand);   
                            myWorkingVolumeCommandPanel.IsChangeAllowed=!(concreteCommandType == SubCommand.ResuspendVialCommand); //set before VolumeTypeSpecifier
                            myWorkingVolumeCommandPanel.IsThresholdStyle=!(concreteCommandType == SubCommand.TransportCommand); //set before VolumeTypeSpecifier
                            
							myWorkingVolumeCommandPanel.IsVolumeSpecificationRequired = true;        // Ensure the relative/absolute value is set before setting this flag.
							myWorkingVolumeCommandPanel.FreeAirDispense  = false;
                            myWorkingVolumeCommandPanel.UseBufferTip = false;
							myWorkingVolumeCommandPanel.CheckTransportDestVial();   
							break;

						case SubCommand.FlushCommand:
						case SubCommand.PrimeCommand:

                            myVolumeMaintenanceCommandPanel.SetVolumeCommandPanelParams(AbsoluteResourceLocation.TPC0101, AbsoluteResourceLocation.TPC0101,
                                    VolumeType.NotSpecified, -1.0f, -1,
                                    1, false, "", 0);
							myVolumeMaintenanceCommandPanel.HomeFlag = false;
							myVolumeMaintenanceCommandPanel.IsVolumeSpecificationRequired = false;    
							break;

						case SubCommand.PauseCommand:
							myPauseCommandPanel.CommandLabel = "";
							myPauseCommandPanel.CommandExtensionTime = 1;
							break;

                        case SubCommand.TopUpMixTransSepTransCommand:
                        case SubCommand.TopUpMixTransCommand:
                        case SubCommand.TopUpTransSepTransCommand:
                        case SubCommand.TopUpTransCommand:
                        case SubCommand.ResusMixSepTransCommand:
                        case SubCommand.ResusMixCommand:
                            {
                                GenericMultiStepCommandPanel panel = myTopUpMixTransSepTransCommandPanel;

                                switch (concreteCommandType)
                                {
                                    case SubCommand.TopUpMixTransSepTransCommand:
                                        panel = myTopUpMixTransSepTransCommandPanel;
                                        break;
                                    case SubCommand.TopUpMixTransCommand:
                                        panel = myTopUpMixTransCommandPanel;
                                        break;
                                    case SubCommand.TopUpTransSepTransCommand:
                                        panel = myTopUpTransSepTransCommandPanel;
                                        break;
                                    case SubCommand.TopUpTransCommand:
                                        panel = myTopUpTransCommandPanel;
                                        break;

                                    case SubCommand.ResusMixSepTransCommand:
                                        panel = myResusMixSepTransCommandPanel;
                                        break;
                                    case SubCommand.ResusMixCommand:
                                        panel = myResusMixCommandPanel;
                                        break;
                                }

                                panel.CommandLabel = "";
                                panel.SetVolumeCommandPanelParams(AbsoluteResourceLocation.TPC0001, AbsoluteResourceLocation.TPC0001,
                                        VolumeType.NotSpecified, 0, 0,
                                        1, false, "", 0);

                                panel.SetVolumeCommandPanelParams2(AbsoluteResourceLocation.TPC0001, AbsoluteResourceLocation.TPC0001,
                                        VolumeType.NotSpecified, 0, 0);
                                panel.SetVolumeCommandPanelParams3(AbsoluteResourceLocation.TPC0001, AbsoluteResourceLocation.TPC0001,
                                                VolumeType.NotSpecified, 0, 0);
                                panel.SetVolumeCommandPanelParamsMix(AbsoluteResourceLocation.TPC0001, AbsoluteResourceLocation.TPC0001,
                                        VolumeType.NotSpecified, 0, 0, 3, 0);
                            }
                            break;

                        case SubCommand.MixTransCommand:
                            {
                                GenericMultiStepCommandPanel2 panel = myMixTransCommandPanel;
                                panel.CommandLabel = "";
                                panel.SetVolumeCommandPanelParams(AbsoluteResourceLocation.TPC0001, AbsoluteResourceLocation.TPC0001,
                                        VolumeType.NotSpecified, 0, 0,
                                        1, false, "", 0);

                                panel.SetVolumeCommandPanelParams2(AbsoluteResourceLocation.TPC0001, AbsoluteResourceLocation.TPC0001,
                                        VolumeType.NotSpecified, 0, 0);
                                panel.MixCycles = 3;
                                panel.TipTubeBottomGap = 0;

                                panel.txtTipTubeBottomGap.Enabled = false;
                            }
                            break;
                       
					}	
				}

		     
		private void CopyCommandPanel(SubCommand concreteCommandType, int copyIndex) 
		{
			switch (concreteCommandType)
			{
				default:
				case SubCommand.HomeAllCommand:	
					myCommandPanel.CommandLabel = ""+myCommandSequence[copyIndex].CommandLabel;
					myCommandPanel.CommandExtensionTime = myCommandSequence[copyIndex].CommandExtensionTime;
					break;

				case SubCommand.DemoCommand:
					ProtocolDemoCommand demoCommand = 
						(ProtocolDemoCommand)myCommandSequence[copyIndex];
					myDemoCommandPanel.DemoCommandIterationCount = demoCommand.IterationCount;
					myCommandPanel.CommandLabel = demoCommand.CommandLabel;
					myCommandPanel.CommandExtensionTime = demoCommand.CommandExtensionTime;
					break;

				case SubCommand.PumpLifeCommand:
					ProtocolPumpLifeCommand pumpLifeCommand = 
						(ProtocolPumpLifeCommand)myCommandSequence[copyIndex];
					myDemoCommandPanel.DemoCommandIterationCount = pumpLifeCommand.IterationCount;
					myCommandPanel.CommandLabel = pumpLifeCommand.CommandLabel;
					myCommandPanel.CommandExtensionTime = pumpLifeCommand.CommandExtensionTime;
					break;

				case SubCommand.IncubateCommand:
					ProtocolIncubateCommand incubateCommand = 
						(ProtocolIncubateCommand)myCommandSequence[copyIndex];
					myIncubateCommandPanel.WaitCommandTimeDuration = incubateCommand.WaitCommandTimeDuration;
					myIncubateCommandPanel.CommandLabel = incubateCommand.CommandLabel;
					myIncubateCommandPanel.CommandExtensionTime = incubateCommand.CommandExtensionTime;
					break;

				case SubCommand.SeparateCommand:
					ProtocolSeparateCommand separateCommand = 
						(ProtocolSeparateCommand)myCommandSequence[copyIndex];
					mySeparateCommandPanel.WaitCommandTimeDuration = separateCommand.WaitCommandTimeDuration;
					mySeparateCommandPanel.CommandLabel = separateCommand.CommandLabel;
					mySeparateCommandPanel.CommandExtensionTime = separateCommand.CommandExtensionTime;
					break;

				case SubCommand.MixCommand:
					ProtocolMixCommand mixCommand = 
						(ProtocolMixCommand)myCommandSequence[copyIndex];

                    myVolumeCommandPanel.cmbDestinationVial.Enabled = false;

                    myVolumeCommandPanel.SetVolumeCommandPanelParams(mixCommand.SourceVial, mixCommand.SourceVial,
                                mixCommand.VolumeTypeSpecifier, mixCommand.RelativeVolumeProportion, mixCommand.AbsoluteVolume_uL,
                                mixCommand.TipRack, mixCommand.TipRackSpecified, mixCommand.CommandLabel, mixCommand.CommandExtensionTime);

                    myVolumeCommandPanel.IsVolumeSpecificationRequired = true;        // Ensure the relative/absolute value is set before setting this flag.

					myVolumeCommandPanel.MixCycles = mixCommand.MixCycles;
					myVolumeCommandPanel.TipTubeBottomGap = mixCommand.TipTubeBottomGap_uL;
					break;

				case SubCommand.TransportCommand:
					ProtocolTransportCommand transportCommand = 
						(ProtocolTransportCommand)myCommandSequence[copyIndex];
					myWorkingVolumeCommandPanel.cmbSourceVial.Enabled = true; 
					myWorkingVolumeCommandPanel.IsChangeAllowed=true; //set before VolumeTypeSpecifier
					myWorkingVolumeCommandPanel.IsThresholdStyle=false; //set before VolumeTypeSpecifier

                    myWorkingVolumeCommandPanel.SetVolumeCommandPanelParams(transportCommand.SourceVial, transportCommand.DestinationVial,
                                transportCommand.VolumeTypeSpecifier, transportCommand.RelativeVolumeProportion, transportCommand.AbsoluteVolume_uL,
                                transportCommand.TipRack, transportCommand.TipRackSpecified, transportCommand.CommandLabel, transportCommand.CommandExtensionTime);

					myWorkingVolumeCommandPanel.IsVolumeSpecificationRequired = true;  // Ensure the relative/absolute value is set before setting this flag.

                    myWorkingVolumeCommandPanel.UseBufferTip = transportCommand.UseBufferTip;
					myWorkingVolumeCommandPanel.FreeAirDispense  = transportCommand.FreeAirDispense;

					myWorkingVolumeCommandPanel.CheckTransportDestVial(); 
					myWorkingVolumeCommandPanel.CheckTransportSourceVial();
					break;				

				case SubCommand.TopUpVialCommand:
					ProtocolTopUpVialCommand topUpVialCommand = 
						(ProtocolTopUpVialCommand)myCommandSequence[copyIndex];

                    myWorkingVolumeCommandPanel.cmbSourceVial.Enabled = false;  
					myWorkingVolumeCommandPanel.IsChangeAllowed=true; //set before VolumeTypeSpecifier
					myWorkingVolumeCommandPanel.IsThresholdStyle=true; //set before VolumeTypeSpecifier


                    myWorkingVolumeCommandPanel.SetVolumeCommandPanelParams(AbsoluteResourceLocation.TPC0001, topUpVialCommand.DestinationVial,
                               topUpVialCommand.VolumeTypeSpecifier, topUpVialCommand.RelativeVolumeProportion, topUpVialCommand.AbsoluteVolume_uL,
                               topUpVialCommand.TipRack, topUpVialCommand.TipRackSpecified, topUpVialCommand.CommandLabel, topUpVialCommand.CommandExtensionTime);

					
					myWorkingVolumeCommandPanel.IsVolumeSpecificationRequired = true;        // Ensure the relative/absolute value is set before setting this flag.     
					myWorkingVolumeCommandPanel.UseBufferTip = false;
                    myWorkingVolumeCommandPanel.FreeAirDispense  = topUpVialCommand.FreeAirDispense;

					myWorkingVolumeCommandPanel.CheckTransportDestVial(); 
					break;

				case SubCommand.ResuspendVialCommand:
					ProtocolResuspendVialCommand resuspendVialCommand = 
						(ProtocolResuspendVialCommand)myCommandSequence[copyIndex];  
					myWorkingVolumeCommandPanel.cmbSourceVial.Enabled = false;    
					myWorkingVolumeCommandPanel.IsChangeAllowed=false; //set before IsVolumeSpecificationRequired
					myWorkingVolumeCommandPanel.IsThresholdStyle=true; //set before IsVolumeSpecificationRequired  

                    myWorkingVolumeCommandPanel.SetVolumeCommandPanelParams(AbsoluteResourceLocation.TPC0001, resuspendVialCommand.DestinationVial,
                              resuspendVialCommand.VolumeTypeSpecifier, resuspendVialCommand.RelativeVolumeProportion, resuspendVialCommand.AbsoluteVolume_uL,
                              resuspendVialCommand.TipRack, resuspendVialCommand.TipRackSpecified, resuspendVialCommand.CommandLabel, resuspendVialCommand.CommandExtensionTime);

					myWorkingVolumeCommandPanel.IsVolumeSpecificationRequired = true;        // Ensure the relative/absolute value is set before setting this flag.
					myWorkingVolumeCommandPanel.FreeAirDispense  = resuspendVialCommand.FreeAirDispense;
					myWorkingVolumeCommandPanel.CheckTransportDestVial();      
					break;

				case SubCommand.FlushCommand:
					ProtocolFlushCommand flushCommand = 
						(ProtocolFlushCommand)myCommandSequence[copyIndex];
                    myVolumeMaintenanceCommandPanel.SetVolumeCommandPanelParams(flushCommand.SourceVial, flushCommand.DestinationVial,
                              VolumeType.NotSpecified, -1.0f, -1,
                              flushCommand.TipRack, flushCommand.TipRackSpecified, flushCommand.CommandLabel, flushCommand.CommandExtensionTime);

					myVolumeMaintenanceCommandPanel.HomeFlag = flushCommand.HomeFlag;
					myVolumeMaintenanceCommandPanel.IsVolumeSpecificationRequired = false;      
					break;

				case SubCommand.PrimeCommand:
					ProtocolPrimeCommand primeCommand = 
						(ProtocolPrimeCommand)myCommandSequence[copyIndex];
                    myVolumeMaintenanceCommandPanel.SetVolumeCommandPanelParams(primeCommand.SourceVial, primeCommand.DestinationVial,
                              VolumeType.NotSpecified, -1.0f, -1,
                              primeCommand.TipRack, primeCommand.TipRackSpecified, primeCommand.CommandLabel, primeCommand.CommandExtensionTime);

					myVolumeMaintenanceCommandPanel.HomeFlag = primeCommand.HomeFlag;
					myVolumeMaintenanceCommandPanel.IsVolumeSpecificationRequired = false;      
					break;

				case SubCommand.PauseCommand:
					ProtocolPauseCommand pauseCommand = 
						(ProtocolPauseCommand)myCommandSequence[copyIndex];
					myPauseCommandPanel.CommandLabel = pauseCommand.CommandLabel;
					myPauseCommandPanel.CommandExtensionTime = pauseCommand.CommandExtensionTime;
					break;


                case SubCommand.TopUpMixTransSepTransCommand:
                    {
                        ProtocolTopUpMixTransSepTransCommand cmd =
                            (ProtocolTopUpMixTransSepTransCommand)myCommandSequence[copyIndex];
                        GenericMultiStepCommandPanel panel = myTopUpMixTransSepTransCommandPanel;
                        panel.CommandLabel = cmd.CommandLabel;
                        panel.cmbSourceVial.Enabled = false;
                        panel.IsChangeAllowed = true; //set before IsVolumeSpecificationRequired
                        panel.IsThresholdStyle = true; //set before IsVolumeSpecificationRequired  

                        panel.SetVolumeCommandPanelParams(AbsoluteResourceLocation.TPC0001, cmd.DestinationVial,
                                  cmd.VolumeTypeSpecifier, cmd.RelativeVolumeProportion, cmd.AbsoluteVolume_uL,
                                  cmd.TipRack, cmd.TipRackSpecified, cmd.CommandLabel, cmd.CommandExtensionTime);

                        panel.SetVolumeCommandPanelParamsMix(cmd.DestinationVial, cmd.DestinationVial,
                                  cmd.VolumeTypeSpecifier2, cmd.RelativeVolumeProportion2, cmd.AbsoluteVolume_uL2,
                                  cmd.MixCycles, cmd.TipTubeBottomGap_uL);
                        panel.SetVolumeCommandPanelParams2(cmd.SourceVial2, cmd.DestinationVial2,
                                  cmd.VolumeTypeSpecifier3, cmd.RelativeVolumeProportion3, cmd.AbsoluteVolume_uL3);
                        panel.SetVolumeCommandPanelParams3(cmd.SourceVial3, cmd.DestinationVial3,
                                  cmd.VolumeTypeSpecifier4, cmd.RelativeVolumeProportion4, cmd.AbsoluteVolume_uL4);

                        panel.WaitCommandTimeDuration = cmd.WaitCommandTimeDuration;

                        panel.IsVolumeSpecificationRequired = true;        // Ensure the relative/absolute value is set before setting this flag.
                        panel.CheckTransportDestVial();
                        panel.CheckTransportSourceVial();
                    }

                    break;
                case SubCommand.TopUpMixTransCommand:
                    {
                        ProtocolTopUpMixTransCommand cmd =
                            (ProtocolTopUpMixTransCommand)myCommandSequence[copyIndex];
                        GenericMultiStepCommandPanel panel = myTopUpMixTransCommandPanel;
                        panel.CommandLabel = cmd.CommandLabel;
                        panel.cmbSourceVial.Enabled = false;
                        panel.IsChangeAllowed = true; //set before IsVolumeSpecificationRequired
                        panel.IsThresholdStyle = true; //set before IsVolumeSpecificationRequired  

                        panel.SetVolumeCommandPanelParams(AbsoluteResourceLocation.TPC0001, cmd.DestinationVial,
                                  cmd.VolumeTypeSpecifier, cmd.RelativeVolumeProportion, cmd.AbsoluteVolume_uL,
                                  cmd.TipRack, cmd.TipRackSpecified, cmd.CommandLabel, cmd.CommandExtensionTime);

                        panel.SetVolumeCommandPanelParamsMix(cmd.DestinationVial, cmd.DestinationVial,
                                  cmd.VolumeTypeSpecifier2, cmd.RelativeVolumeProportion2, cmd.AbsoluteVolume_uL2,
                                  cmd.MixCycles, cmd.TipTubeBottomGap_uL);

                        panel.SetVolumeCommandPanelParams2(cmd.SourceVial2, cmd.DestinationVial2,
                                  cmd.VolumeTypeSpecifier3, cmd.RelativeVolumeProportion3, cmd.AbsoluteVolume_uL3);


                        panel.IsVolumeSpecificationRequired = true;        // Ensure the relative/absolute value is set before setting this flag.
                        panel.CheckTransportDestVial();
                        panel.CheckTransportSourceVial();
                    }

                    break;
                case SubCommand.TopUpTransSepTransCommand:
                    {
                        ProtocolTopUpTransSepTransCommand cmd =
                            (ProtocolTopUpTransSepTransCommand)myCommandSequence[copyIndex];
                        GenericMultiStepCommandPanel panel = myTopUpTransSepTransCommandPanel;
                        panel.CommandLabel = cmd.CommandLabel;
                        panel.cmbSourceVial.Enabled = false;
                        panel.IsChangeAllowed = true; //set before IsVolumeSpecificationRequired
                        panel.IsThresholdStyle = true; //set before IsVolumeSpecificationRequired  

                        panel.SetVolumeCommandPanelParams(AbsoluteResourceLocation.TPC0001, cmd.DestinationVial,
                                  cmd.VolumeTypeSpecifier, cmd.RelativeVolumeProportion, cmd.AbsoluteVolume_uL,
                                  cmd.TipRack, cmd.TipRackSpecified, cmd.CommandLabel, cmd.CommandExtensionTime);

                        panel.SetVolumeCommandPanelParams2(cmd.SourceVial2, cmd.DestinationVial2,
                                  cmd.VolumeTypeSpecifier2, cmd.RelativeVolumeProportion2, cmd.AbsoluteVolume_uL2);
                        panel.SetVolumeCommandPanelParams3(cmd.SourceVial3, cmd.DestinationVial3,
                                  cmd.VolumeTypeSpecifier3, cmd.RelativeVolumeProportion3, cmd.AbsoluteVolume_uL3);

                        panel.WaitCommandTimeDuration = cmd.WaitCommandTimeDuration;

                        panel.IsVolumeSpecificationRequired = true;        // Ensure the relative/absolute value is set before setting this flag.
                        panel.CheckTransportDestVial();
                        panel.CheckTransportSourceVial();
                    }

                    break;
                case SubCommand.TopUpTransCommand:
                    {
                        ProtocolTopUpTransCommand cmd =
                            (ProtocolTopUpTransCommand)myCommandSequence[copyIndex];
                        GenericMultiStepCommandPanel panel = myTopUpTransCommandPanel;
                        panel.CommandLabel = cmd.CommandLabel;
                        panel.cmbSourceVial.Enabled = false;
                        panel.IsChangeAllowed = true; //set before IsVolumeSpecificationRequired
                        panel.IsThresholdStyle = true; //set before IsVolumeSpecificationRequired  

                        panel.SetVolumeCommandPanelParams(AbsoluteResourceLocation.TPC0001, cmd.DestinationVial,
                                  cmd.VolumeTypeSpecifier, cmd.RelativeVolumeProportion, cmd.AbsoluteVolume_uL,
                                  cmd.TipRack, cmd.TipRackSpecified, cmd.CommandLabel, cmd.CommandExtensionTime);

                        panel.SetVolumeCommandPanelParams2(cmd.SourceVial2, cmd.DestinationVial2,
                                  cmd.VolumeTypeSpecifier2, cmd.RelativeVolumeProportion2, cmd.AbsoluteVolume_uL2);

                        panel.IsVolumeSpecificationRequired = true;        // Ensure the relative/absolute value is set before setting this flag.
                        panel.CheckTransportDestVial();
                        panel.CheckTransportSourceVial();
                    }

                    break;
                case SubCommand.ResusMixSepTransCommand:
                    {
                        ProtocolResusMixSepTransCommand cmd =
                            (ProtocolResusMixSepTransCommand)myCommandSequence[copyIndex];
                        GenericMultiStepCommandPanel panel = myResusMixSepTransCommandPanel;
                        panel.CommandLabel = cmd.CommandLabel;
                        panel.cmbSourceVial.Enabled = false;
                        panel.IsChangeAllowed = false; //set before IsVolumeSpecificationRequired
                        panel.IsThresholdStyle = true; //set before IsVolumeSpecificationRequired  

                        panel.SetVolumeCommandPanelParams(AbsoluteResourceLocation.TPC0001, cmd.DestinationVial,
                                  cmd.VolumeTypeSpecifier, cmd.RelativeVolumeProportion, cmd.AbsoluteVolume_uL,
                                  cmd.TipRack, cmd.TipRackSpecified, cmd.CommandLabel, cmd.CommandExtensionTime);

                        panel.SetVolumeCommandPanelParamsMix(cmd.DestinationVial, cmd.DestinationVial,
                                  cmd.VolumeTypeSpecifier2, cmd.RelativeVolumeProportion2, cmd.AbsoluteVolume_uL2,
                                  cmd.MixCycles, cmd.TipTubeBottomGap_uL);

                        panel.SetVolumeCommandPanelParams2(cmd.SourceVial2, cmd.DestinationVial2,
                                  cmd.VolumeTypeSpecifier3, cmd.RelativeVolumeProportion3, cmd.AbsoluteVolume_uL3);

                        panel.WaitCommandTimeDuration = cmd.WaitCommandTimeDuration;

                        panel.IsVolumeSpecificationRequired = true;        // Ensure the relative/absolute value is set before setting this flag.
                        panel.CheckTransportDestVial();
                        panel.CheckTransportSourceVial();
                    }

                    break;
                case SubCommand.ResusMixCommand:
                    {
                        ProtocolResusMixCommand cmd =
                            (ProtocolResusMixCommand)myCommandSequence[copyIndex];
                        GenericMultiStepCommandPanel panel = myResusMixCommandPanel;
                        panel.CommandLabel = cmd.CommandLabel;
                        panel.cmbSourceVial.Enabled = false;
                        panel.IsChangeAllowed = false; //set before IsVolumeSpecificationRequired
                        panel.IsThresholdStyle = true; //set before IsVolumeSpecificationRequired  

                        panel.SetVolumeCommandPanelParams(AbsoluteResourceLocation.TPC0001, cmd.DestinationVial,
                                  cmd.VolumeTypeSpecifier, cmd.RelativeVolumeProportion, cmd.AbsoluteVolume_uL,
                                  cmd.TipRack, cmd.TipRackSpecified, cmd.CommandLabel, cmd.CommandExtensionTime);

                        panel.SetVolumeCommandPanelParamsMix(cmd.DestinationVial, cmd.DestinationVial,
                                  cmd.VolumeTypeSpecifier2, cmd.RelativeVolumeProportion2, cmd.AbsoluteVolume_uL2,
                                  cmd.MixCycles, cmd.TipTubeBottomGap_uL);

                        panel.IsVolumeSpecificationRequired = true;        // Ensure the relative/absolute value is set before setting this flag.
                        panel.CheckTransportDestVial();
                        panel.CheckTransportSourceVial();
                    }

                    break;
                case SubCommand.MixTransCommand:
                    {
                        ProtocolMixTransCommand cmd = (ProtocolMixTransCommand)myCommandSequence[copyIndex];
                        GenericMultiStepCommandPanel2 panel = myMixTransCommandPanel;

                        panel.cmbDestinationVial.Enabled = false;

                        panel.SetVolumeCommandPanelParams(cmd.SourceVial, cmd.SourceVial,
                                    cmd.VolumeTypeSpecifier, cmd.RelativeVolumeProportion, cmd.AbsoluteVolume_uL,
                                    cmd.TipRack, cmd.TipRackSpecified, cmd.CommandLabel, cmd.CommandExtensionTime);

                        panel.IsVolumeSpecificationRequired = true;        // Ensure the relative/absolute value is set before setting this flag.

                        panel.MixCycles = cmd.MixCycles;
                        panel.TipTubeBottomGap = cmd.TipTubeBottomGap_uL;

                        panel.SetVolumeCommandPanelParams2(cmd.SourceVial2, cmd.DestinationVial2,
                                    cmd.VolumeTypeSpecifier2, cmd.RelativeVolumeProportion2, cmd.AbsoluteVolume_uL2);


                        panel.CheckTransportDestVial();
                        panel.CheckTransportSourceVial();
                    }
                    break;				
			}//switch
		}

		      	private void btnHide_Click(object sender, System.EventArgs e)
				{
					pnlHeaderInfo.Visible = false;
					btnShow.Visible = true;
				}
		      	private void btnShow_Click(object sender, System.EventArgs e)
				{
					pnlHeaderInfo.Visible = true;
					btnShow.Visible = false;
				}
		private void EnableCommandSequenceItemActions(int sequenceItemCount)
		{
			// Disable the Delete button and MoveUp/MoveDown buttons if there are no further 
			// command sequence items.
			btnDeleteCommand.Enabled = (sequenceItemCount > 0);
			btnMoveUp.Enabled		 = CanMoveUp() && ( ! isAddingInProgress);
			btnMoveDown.Enabled		 = CanMoveDown() && ( ! isAddingInProgress);
			btnCopy.Enabled			 = (sequenceItemCount > 0) && (!isAddingInProgress);      

		}

		private void btnDeleteCommand_Click(object sender, System.EventArgs e)
		{
			//RL - delete commands prompt check protocol - 03/27/06
			EnterEditMode();
			if (myCommandSequence.Length == 1)
			{
				CommandPanel.PanelType panelType = 
					CommandPanelTypeForCommandSubtype(myCurrentCommandIndex);
				txtBoxEnabled = myCommandPanels[(int)panelType].EnableExtension;
			}

			// Delete the current command sequence item.  After deletion, set the
			// current item to the 'previous' item, if it exists.
			int previousCommandIndex = myCurrentCommandIndex;
            // 2011-09-29 sp
            // keep current commandIndex the same unless it is the last command
            // if (myCurrentCommandIndex > 0)
            if (myCurrentCommandIndex == myCommandSequence.GetLength(0) - 1)
			{
                --previousCommandIndex;
            }
			ArrayList commandSequence = new ArrayList();
			commandSequence.AddRange(myCommandSequence);
			commandSequence.RemoveAt(myCurrentCommandIndex);
			myCommandSequence = (IProtocolCommand[]) commandSequence.ToArray(typeof(IProtocolCommand));
			RecalculateCommandSequence();
			// Explicitly update the current command index.  We need to avoid attempting 
			// to validate the command we're deleting when we re-bind the command 
			// sequence.
			int newCommandSequenceCount = myCommandSequence.GetLength(0);
			myCurrentCommandIndex = newCommandSequenceCount-1;
			// Now, re-bind the command list to the new command sequence
			int commandSequenceCount = myCommandSequence.GetLength(0);
			if (commandSequenceCount > 0)
			{
                // 2011-09-29 sp
                // fix problem of reseting the current command counter to 0 instead of the previous command
                /*
				myCurrentCommandIndex = previousCommandIndex;
				UpdateCommandDetailView();
				lstCommands.DataSource = myCommandSequence;
				lstCommands.SelectedIndex = myCurrentCommandIndex;
				lstCommands.Invalidate();
                 */
                lstCommands.DataSource = myCommandSequence;
                myCurrentCommandIndex = previousCommandIndex;
                lstCommands.SelectedIndex = myCurrentCommandIndex;
                UpdateCommandDetailView();
                lstCommands.Invalidate();
            }
			else
			{
				// Since there are no items, set the current command "panel" to show a blank 
				// command details display.
				lstCommands.DataSource = myCommandSequence;         
				lstCommands.DisplayMember = "CommandSummary";       			           
				myCurrentCommandPanel = null;
				UpdateCommandDetailView();
			}
			// Re-enable command sequence item actions, as we're finished adding the new item
			SetAddingMode(false);
			EnableCommandSequenceItemActions(commandSequenceCount);

			//RL - delete commands prompt check protocol - 03/27/06
			TestAndSetCommandSequenceError();

			if (!IsACommand()) btnConfirmCommand.Visible = false;
			else btnConfirmCommand.Visible = true;

		}

		private int myCurrentCommandIndex = -1;

		private void lstCommands_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			// if not chnanging index, don't bother doing anything
            if (myCurrentCommandIndex == lstCommands.SelectedIndex)
			{
				return;
			}
			if (lstCommands.Items.Count > 0) btnConfirmCommand.Visible = true;
			else btnConfirmCommand.Visible = false;

			TestAndSetCommandSequenceError();

			// Verify that the current command is valid before allowing the user
			// to move to a different command.
			if (isCurrentCommandDirty || isAddingInProgress)
			{
				bool isCurrentCommandValid = ValidateCurrentCommand();
				if (isCurrentCommandValid)
				{
					// Since we're about to move places in the command list,
					// persist changes from the current panel back to the sequence.
					PersistCommandDetailView();
					isCurrentCommandDirty = false;
					// Re-enable Add/MoveUp/MoveDown now the current command is validated.
					SetAddingMode(false);
				}
				else
				{
					// The current command is not valid, so do not allow the user
					// to move the position in the command list until the necessary
					// corrections are made or the current command is deleted.
                    lstCommands.SelectedIndex = myCurrentCommandIndex;
                    lstCommands.Invalidate();
					return;
				}   
			}

			// If we've reached here, we know the current command is valid and we
			// can allow the selected index to change and the relevant command data to 
			// be updated.
			if (myCurrentCommandIndex == lstCommands.SelectedIndex)
				PersistCommandDetailView();

			myCurrentCommandIndex = lstCommands.SelectedIndex;


			if (myCurrentCommandIndex != -1)
			{
				// Update the data displayed in the command view.  Note, however, that
				// we must ignore any 'data changed' events in the process as we know
				// it's an internal update and not the user changing the content.
				CommandPanel.CommandDetailChanged -= new Tesla.ProtocolEditorControls.CommandPanel.CommandDetailChangedDelegate(WhenCommandDetailChanged);
				UpdateCommandDetailView();	
				
				//4.5.X - 2.4.3
				//Disable working vol if there is atleast 1 topup/resuspend and	they are all relative
				//lstCommands.SelectedIndexChanged -= new System.EventHandler(this.lstCommands_SelectedIndexChanged);
				CheckCmdSeqForWorkingVolumeUse();
				
				//lstCommands.SelectedIndexChanged += new System.EventHandler(this.lstCommands_SelectedIndexChanged);
				CommandPanel.CommandDetailChanged += new Tesla.ProtocolEditorControls.CommandPanel.CommandDetailChangedDelegate(WhenCommandDetailChanged);

				// Enable/disable the move-item up/down buttons as appropriate
				btnMoveUp.Enabled = CanMoveUp();
				btnMoveDown.Enabled = CanMoveDown();
			}
      		this.Invalidate(true);
			myWorkingVolumeCommandPanel.CheckTransportDestVial(); 
			this.Invalidate(true);
		}

        // added 2011-09-05 sp 
        // provide highlights to specific lines in protocol commands
        private void lstCommands_DrawItem(object sender, DrawItemEventArgs e)
        {

            Brush myBrush;
            string delimiterStr = " ###:";

            int index = e.Index;
            if (index >= 0 && index < lstCommands.Items.Count)
            {
                IProtocolCommand thisCommand = (IProtocolCommand)lstCommands.Items[index];
                string text = thisCommand.CommandSummary;
                string warningText = text + delimiterStr + thisCommand.CommandStatus;
                Font newFont = e.Font;
                if (index != lstCommands.SelectedIndex)
                {
                    switch( thisCommand.CommandCheckStatus )
                    {
                        case VolumeCheck.VOLUMES_WARNINGS:
                            myBrush = Brushes.DarkRed;
                            text = warningText;
                            // add emphasis as light text color has lower contrast
                            newFont = new Font(newFont, FontStyle.Bold);
                            break;
                        case VolumeCheck.VOLUMES_INVALID:
                            myBrush = Brushes.Red;
                            text = warningText;
                            // add emphasis as light text color has lower contrast
                            newFont = new Font(newFont, FontStyle.Bold);
                            break;
                        case VolumeCheck.VOLUMES_INFO:
                            text = warningText;
                            myBrush = Brushes.Black;
                            // add emphasis as light text color has lower contrast
                            newFont = new Font(newFont, FontStyle.Bold);
                            break;
                        case VolumeCheck.VOLUMES_UNCHECKED:
                        case VolumeCheck.VOLUMES_VALID:
                        default:
                            myBrush = Brushes.Black;//
                            break;
                    }
                }
                else
                {
                    switch( thisCommand.CommandCheckStatus )
                    {
                        case VolumeCheck.VOLUMES_WARNINGS:
                            myBrush = Brushes.DarkRed;//Brushes.Pink;
                            text = warningText;
                            newFont = new Font(newFont, FontStyle.Bold);
                            break;
                        case VolumeCheck.VOLUMES_INVALID:
                            myBrush = Brushes.Red;//Brushes.Tomato;
                            text = warningText;
                            newFont = new Font(newFont, FontStyle.Bold);
                            break;
                        case VolumeCheck.VOLUMES_INFO:
                            text = warningText;
                            //myBrush = Brushes.LightGray;
                            myBrush = Brushes.Black;//Brushes.Gray;
                            newFont = new Font(newFont, FontStyle.Bold);
                            break;
                        case VolumeCheck.VOLUMES_UNCHECKED:
                        case VolumeCheck.VOLUMES_VALID:
                        default:
                            //myBrush = Brushes.LightGray;
                            myBrush = Brushes.Black;//Brushes.Gray;
                            break;
                    }
                }


                //if the item state is selected them change the back color 
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                    e = new DrawItemEventArgs(e.Graphics,
                                              newFont,
                                              e.Bounds,
                                              e.Index,
                                              e.State ^ DrawItemState.Selected,
                                              new Pen(myBrush).Color,
                                              Color.LightBlue);//Choose the color


                e.DrawBackground();

                //text too long, cut middle
                int max = 69;
                if (text.Length > max)
                {
                    int warning_start_idx = text.LastIndexOf(delimiterStr);
                    if (warning_start_idx >0)
                    {
                        int warning_len = text.Length - warning_start_idx;
                        string s1 = text.Substring(0,max-warning_len);
                        string s2 = text.Substring(warning_start_idx);
                        text = s1 +"... "+s2;
                    }
                }


                // Print text
                StringFormat strFormat = StringFormat.GenericDefault;
                float[] tabStops = { 15f, 140f, 0f };
                strFormat.SetTabStops(4, tabStops);
                e.Graphics.DrawString(text, newFont, myBrush, e.Bounds, strFormat);
            }

            e.DrawFocusRectangle();
        }


		private bool CanMoveUp()
		{
			return myCurrentCommandIndex != 0 && myCommandSequence.GetLength(0) > 0;
		}

		private bool CanMoveDown()
		{
			int commandSequenceItemCount = myCommandSequence.GetLength(0);
			return commandSequenceItemCount > 1 && 
				myCurrentCommandIndex < commandSequenceItemCount-1;
		}

		#region Data Entry Error Indicators 

		private void TestAndSetCommandSequenceError()
		{
			if (myCommandSequence == null || myCommandSequence.GetLength(0) == 0)
			{
				ShowCommandSequenceError();
			}
			else
			{
				ClearCommandSequenceError();
			}
		}

		private void ShowCommandSequenceError()
		{
			errorCommandSequence.SetIconAlignment(btnAddCommand, ErrorIconAlignment.MiddleLeft);
			errorCommandSequence.SetError(btnAddCommand, "Define at least 1 command");
		}

		private void ClearCommandSequenceError()
		{
			errorCommandSequence.SetError(btnAddCommand, string.Empty);
		}

		private void cmbProtocolType_SelectedIndexChanged(object sender, System.EventArgs e)
		{
            UpdateVialComboboxes();

			// clear custom names array
			DbgView("Protocol Details - combo box - clear custom names");
			theProtocolModel.InitCustomNames();		
			
			EnterEditMode();
			SetAddingMode(false);
		}

		public void EnableProtocolType(bool state)
		{
			cmbProtocolType.Enabled = state;
		}

		private void txtProtocolLabel_TextChanged(object sender, System.EventArgs e)
		{
			EnterEditMode();
			if (txtProtocolLabel.TextLength == 0)
			{
				ShowProtocolLabelError();
			}
			else
			{
				ClearProtocolLabelError();
			}
		}

		private void ShowProtocolLabelError()
		{
			errorProtocolLabel.SetIconAlignment(txtProtocolLabel, ErrorIconAlignment.MiddleLeft);
			errorProtocolLabel.SetError(txtProtocolLabel, "Protocol label required");
		}

		private void ClearProtocolLabelError()
		{
			errorProtocolLabel.SetError(txtProtocolLabel, string.Empty);
		}

		      	private void txtProtocolDesc_TextChanged(object sender, System.EventArgs e)
				{
					theProtocolModel.ProtocolDesc = txtProtocolDesc.Text;
					EnterEditMode();
				}


		


		public void WhenCommandDetailChanged()
		{
			// Mark the command as 'dirty' as well as the form
			if (isEditModeEnabled)
			{
				isCurrentCommandDirty = true;
				EnterEditMode();
			}
		}

		private bool isAddingInProgress;

		private void SetAddingMode(bool isAdding)
		{
			isAddingInProgress = isAdding;
			if (isAddingInProgress)
			{
				btnAddCommand.Enabled = false;
				btnMoveDown.Enabled = false;
				btnMoveUp.Enabled = false;
				btnCopy.Enabled = false;
				cmbCommandTypes.Enabled = false;      
			}
			else
			{
				btnAddCommand.Enabled = true;
				btnMoveDown.Enabled = true;
				btnMoveUp.Enabled = true;
				btnCopy.Enabled = true;
				cmbCommandTypes.Enabled = true;      
				// In order that the label for the new command is shown in the list,
				// we seem to have to explicitly null the list's data source and then
				// re-bind it (doing the latter isn't enough to show the command label).
				lstCommands.DataSource = null;
				lstCommands.DataSource = myCommandSequence;
				lstCommands.DisplayMember = "CommandSummary";      

			}
		}

		private int myWorkingVolumeSampleThreshold_uL = 1;  // IAT - changed from 0 to 1
		private int myWorkingVolumeLow_uL = 1;				// IAT - changed from 0 to 1
		private int myWorkingVolumeHigh_uL = 1;				// IAT - changed from 0 to 1

		private void txtWorkingVolumeSampleThreshold_TextChanged(object sender, System.EventArgs e)
		{
			EnterEditMode();
			if (txtWorkingVolumeSampleThreshold.TextLength == 0)
			{
				// The working volume is optional
				myWorkingVolumeSampleThreshold_uL = 1;
				ClearWorkingVolumeSampleThresholdError();
			}
			else
			{
				try
				{
					myWorkingVolumeSampleThreshold_uL = int.Parse(txtWorkingVolumeSampleThreshold.Text);
					if ((IsMaintenanceProtocol == false) && 
//					if ((theProtocolModel.ProtocolClass != ProtocolClass.Maintenance || 
//						 theProtocolModel.ProtocolClass != ProtocolClass.Shutdown) &&
						((myWorkingVolumeSampleThreshold_uL < mySampleVolumeMin_uL	||
						 myWorkingVolumeSampleThreshold_uL > mySampleVolumeMax_uL) && 
						 txtWorkingVolumeSampleThreshold.Enabled))
					{
						ShowWorkingVolumeSampleThresholdError();
					}
					else
					{
						ClearWorkingVolumeSampleThresholdError();
					}
				}
				catch
				{
					myWorkingVolumeSampleThreshold_uL = 1;       
				}
			}
		}

		private void ShowWorkingVolumeSampleThresholdError()
		{
			// Display the sample threshold error
			errorWorkingVolumeThreshold.SetIconAlignment(
				txtWorkingVolumeSampleThreshold, ErrorIconAlignment.MiddleLeft);
			errorWorkingVolumeThreshold.SetError(
				txtWorkingVolumeSampleThreshold, "Sample threshold must be >= minimum sample volume and <= maximum sample volume");
			// Disable the working volume low/high areas until the sample threshold error is fixed.
			txtWorkingVolumeLow.Enabled = false;
			txtWorkingVolumeHigh.Enabled = false;
		}

		private void ClearWorkingVolumeSampleThresholdError()
		{
			// Clear the error display
			errorWorkingVolumeThreshold.SetError(txtWorkingVolumeSampleThreshold, string.Empty);
			// Enable the working volume high/low areas now the sample threshold error is fixed.
			txtWorkingVolumeLow.Enabled = true;
			txtWorkingVolumeHigh.Enabled = true;
		}

		private void txtWorkingVolumeLow_TextChanged(object sender, System.EventArgs e)
		{
			EnterEditMode();
			if (txtWorkingVolumeLow.TextLength == 0)
			{
				// The working volume is optional, so default the working volume (low) to the 
				// minimum sample volume.
				myWorkingVolumeLow_uL = mySampleVolumeMin_uL;
				ClearWorkingVolumeLowError();
			}
			else
			{
				try
				{
					myWorkingVolumeLow_uL = int.Parse(txtWorkingVolumeLow.Text);
					if ((IsMaintenanceProtocol == false) && 
//					if ((theProtocolModel.ProtocolClass != ProtocolClass.Maintenance || 
//						 theProtocolModel.ProtocolClass != ProtocolClass.Shutdown) &&
						(IsWorkingVolumeLowError()&& txtWorkingVolumeLow.Enabled))
					{
						ShowWorkingVolumeLowError();
					}
					else
					{
						ClearWorkingVolumeLowError();
						// Also clear the working volume (high) error if suitable
						if ( ! IsWorkingVolumeHighError())
						{
							ClearWorkingVolumeHighError();
						}
					}
				}
				catch
				{
					myWorkingVolumeLow_uL = 1;  // changed from 0 to 1
				}
			}
		}

		private void ShowWorkingVolumeLowError()
		{
			errorWorkingVolumeLow.SetIconAlignment(
				txtWorkingVolumeLow, ErrorIconAlignment.MiddleLeft);
			errorWorkingVolumeLow.SetError(
				txtWorkingVolumeLow, "Working volume (low) must be >= minimum sample volume and <= working volume (high)");
		}

		private void ClearWorkingVolumeLowError()
		{
			errorWorkingVolumeLow.SetError(txtWorkingVolumeLow, string.Empty);
		}

		private bool IsWorkingVolumeLowError()
		{
			return myWorkingVolumeLow_uL < mySampleVolumeMin_uL	||
				myWorkingVolumeLow_uL > myWorkingVolumeHigh_uL ||
				myWorkingVolumeLow_uL < MIN_VOLUME_UL;				     
		}

		private bool IsWorkingVolumeHighError()
		{
            return myWorkingVolumeHigh_uL > SeparatorResourceManager.GetMaxVolume() ||
				myWorkingVolumeHigh_uL < mySampleVolumeMin_uL	 ||
				myWorkingVolumeHigh_uL < myWorkingVolumeLow_uL;
		}

		private void txtWorkingVolumeHigh_TextChanged(object sender, System.EventArgs e)
		{
			EnterEditMode();
			if (txtWorkingVolumeHigh.TextLength == 0)
			{
				// The working volume is optional, so default the working volume (high) to the 
				// maximum sample volume.
                myWorkingVolumeHigh_uL = SeparatorResourceManager.GetMaxVolume();
				ClearWorkingVolumeHighError();
			}
			else
			{
				try
				{
					myWorkingVolumeHigh_uL = int.Parse(txtWorkingVolumeHigh.Text);
					if ((IsMaintenanceProtocol == false) && 
//					if ((theProtocolModel.ProtocolClass != ProtocolClass.Maintenance || 
//						 theProtocolModel.ProtocolClass != ProtocolClass.Shutdown) &&
						(IsWorkingVolumeHighError()&& txtWorkingVolumeHigh.Enabled))
					{
						ShowWorkingVolumeHighError();
					}
					else
					{
						ClearWorkingVolumeHighError();
						// Also clear the working volume (low) error if suitable
						if ( ! IsWorkingVolumeLowError())
						{
							ClearWorkingVolumeLowError();
						}
					}
				}
				catch
				{
					myWorkingVolumeHigh_uL = 1;  // changed from 0 to 1
				}
			}
		}

		private void ShowWorkingVolumeHighError()
		{
			errorWorkingVolumeHigh.SetIconAlignment(
				txtWorkingVolumeHigh, ErrorIconAlignment.MiddleLeft);
            errorWorkingVolumeHigh.SetError(
                txtWorkingVolumeHigh, "Working volume (high) must be >= working volume (low) and <= " + SeparatorResourceManager.GetMaxVolume() + "uL");//10000uL");
		}

		private void ClearWorkingVolumeHighError()
		{
			errorWorkingVolumeHigh.SetError(txtWorkingVolumeHigh, string.Empty);
		}

		private int mySampleVolumeMin_uL = 1;		
		private int mySampleVolumeMax_uL = 1;		

		private void txtSampleVolumeMin_TextChanged(object sender, System.EventArgs e)
		{
			EnterEditMode();
			if (txtSampleVolumeMin.TextLength == 0)
			{
				ShowSampleVolumeMinError();
			}
			else
			{
				try
				{
					mySampleVolumeMin_uL = int.Parse(txtSampleVolumeMin.Text);
					if ((IsMaintenanceProtocol == false) && 
//					if ((theProtocolModel.ProtocolClass != ProtocolClass.Maintenance || 
//						 theProtocolModel.ProtocolClass != ProtocolClass.Shutdown) &&
						(mySampleVolumeMin_uL < 1 || mySampleVolumeMin_uL > mySampleVolumeMax_uL))
					{
						ShowSampleVolumeMinError();
					}
					else
					{
						ClearSampleVolumeMinError();
						if (txtSampleVolumeMax.TextLength > 0)
						{
							ClearSampleVolumeMaxError();
						}
					}
				}
				catch
				{
					mySampleVolumeMin_uL = 1;		//     changed from 0 to 1
				}
			}

			if(txtWorkingVolumeSampleThreshold.Enabled)
			{
				// Trigger updates of the working volume error indicators so their error state
				// is recalculated in accordance with the new sample volume data.
				txtWorkingVolumeSampleThreshold_TextChanged(this, new System.EventArgs());
				txtWorkingVolumeLow_TextChanged(this, new System.EventArgs());
				txtWorkingVolumeHigh_TextChanged(this, new System.EventArgs());
			}
		}

		private void ShowSampleVolumeMinError()
		{
			errorSampleVolumeMin.SetIconAlignment(
				txtSampleVolumeMin, ErrorIconAlignment.MiddleLeft);
			errorSampleVolumeMin.SetError(
				txtSampleVolumeMin, "Minimum Sample Volume must be >= 1 and <= Maximum Sample Volume");
		}

		private void ClearSampleVolumeMinError()
		{
			errorSampleVolumeMin.SetError(txtSampleVolumeMin, string.Empty);
		}

		private void txtSampleVolumeMax_TextChanged(object sender, System.EventArgs e)
		{
			EnterEditMode();
			if (txtSampleVolumeMax.TextLength == 0)
			{
				ShowSampleVolumeMaxError(0);
			}
			else
			{
				try
				{
					mySampleVolumeMax_uL = int.Parse(txtSampleVolumeMax.Text);
					if ((IsMaintenanceProtocol == false) && 
//					if ((theProtocolModel.ProtocolClass != ProtocolClass.Maintenance || 
//						 theProtocolModel.ProtocolClass != ProtocolClass.Shutdown) &&
						(mySampleVolumeMax_uL < 1 || mySampleVolumeMax_uL < mySampleVolumeMin_uL ||
                         mySampleVolumeMax_uL > SeparatorResourceManager.GetMaxVolume()))
					{
						ShowSampleVolumeMaxError(0);
					}
					else
					{
						ClearSampleVolumeMaxError();
						if (txtSampleVolumeMin.TextLength > 0)
						{
							ClearSampleVolumeMinError();
						}
					}
				}
				catch
				{
					mySampleVolumeMax_uL = 1;			//     changed from 0 to 1
				}
			}
			
			if(txtWorkingVolumeSampleThreshold.Enabled)
			{
				// Trigger updates of the working volume error indicators so their error state
				// is recalculated in accordance with the new sample volume data.
				txtWorkingVolumeSampleThreshold_TextChanged(this, new System.EventArgs());
				txtWorkingVolumeLow_TextChanged(this, new System.EventArgs());
				txtWorkingVolumeHigh_TextChanged(this, new System.EventArgs());
			}
		}
		
		// IAT - added second error type
		private void ShowSampleVolumeMaxError(int e)
		{
			errorSampleVolumeMax.SetIconAlignment(
				txtSampleVolumeMax, ErrorIconAlignment.MiddleLeft);
			if(e == 0)
			{
				errorSampleVolumeMax.SetError(
                    txtSampleVolumeMax, "Maximum Sample Volume must be >= Minimum Sample Volume and <= " + SeparatorResourceManager.GetMaxVolume() + "uL");// 10000uL");
			}
			else if(e == 1)      
			{
				errorSampleVolumeMax.SetError(
					txtSampleVolumeMax, "Maxium Sample Volume must be <= 1100uL if 1mL vials \nare being used, and <= 5000uL if 5mL vials are used.");
			}
		}

		private void ClearSampleVolumeMaxError()
		{
			errorSampleVolumeMax.SetError(txtSampleVolumeMax, string.Empty);
		}

		#endregion Data Entry Error Indicators

		#endregion Control events

		#region Data access

		private IProtocolCommand[]	myCommandSequence;
		private bool                isCurrentCommandDirty;
		private bool				isNewProtocol;
		private bool				isMaintenanceProtocol;

		public bool IsMaintenanceProtocol // bdr
		{
			get
			{
				return isMaintenanceProtocol;
			}
			set
			{
				isMaintenanceProtocol = value;
			}
		}

		public bool IsNewProtocol
		{
			get
			{
				return isNewProtocol;
			}
			set
			{
				isNewProtocol = value;
				SetAddingMode(false);
			}
		}

		      	public bool EnableExtensionTime
				{
					set
					{
						myDemoCommandPanel.EnableExtension = value;
						myIncubateCommandPanel.EnableExtension = value;
						mySeparateCommandPanel.EnableExtension = value;
						myWorkingVolumeCommandPanel.EnableExtension = value;
						myVolumeMaintenanceCommandPanel.EnableExtension = value;
						myVolumeCommandPanel.EnableExtension = value;
                        myCommandPanel.EnableExtension = value;
                        myPauseCommandPanel.EnableExtension = value;
                        myTopUpMixTransSepTransCommandPanel.EnableExtension = value;
					}
					get
					{
						switch (myCommandSequence[myCurrentCommandIndex].CommandSubtype)
						{
							case SubCommand.DemoCommand:
							case SubCommand.PumpLifeCommand:
								return myDemoCommandPanel.EnableExtension;
							case SubCommand.IncubateCommand:
								return myIncubateCommandPanel.EnableExtension;
							case SubCommand.SeparateCommand:
								return mySeparateCommandPanel.EnableExtension;
							case SubCommand.ResuspendVialCommand:
							case SubCommand.TopUpVialCommand:
							case SubCommand.TransportCommand:
								return myWorkingVolumeCommandPanel.EnableExtension;
							case SubCommand.FlushCommand:
							case SubCommand.PrimeCommand:
								return myVolumeMaintenanceCommandPanel.EnableExtension;
							case SubCommand.MixCommand:
								return myVolumeCommandPanel.EnableExtension;
							case SubCommand.HomeAllCommand:
                                return myCommandPanel.EnableExtension;
                            case SubCommand.PauseCommand:
                                return myPauseCommandPanel.EnableExtension;
                            case SubCommand.TopUpMixTransSepTransCommand:
                                return myTopUpMixTransSepTransCommandPanel.EnableExtension;
                            case SubCommand.TopUpMixTransCommand:
                                return myTopUpMixTransCommandPanel.EnableExtension;
                            case SubCommand.TopUpTransSepTransCommand:
                                return myTopUpTransSepTransCommandPanel.EnableExtension;
                            case SubCommand.TopUpTransCommand:
                                return myTopUpTransCommandPanel.EnableExtension;
                            case SubCommand.ResusMixSepTransCommand:
                                return myResusMixSepTransCommandPanel.EnableExtension;
                            case SubCommand.ResusMixCommand:
                                return myResusMixCommandPanel.EnableExtension;
                            case SubCommand.MixTransCommand:
                                return myMixTransCommandPanel.EnableExtension;
							default:
								return false;
						}
					}
				}

		      	public bool IsACommand()//is there atleast one command in the sequence?
				{
					if (lstCommands.Items.Count > 0)
						return true;
					return false;
				}

		public void ClearForm()
		{
			theProtocolModel.ClearModel();
			TestAndSetCommandSequenceError();
			myCurrentCommandIndex = -1;
			btnConfirmCommand.Visible = false;
			//cmbCommandTypes.SelectedIndex = 0;      
		}

		public void PopulateDetailsFields()
		{
			// Disable form edit mode triggers for the duration of the bulk update
			IsEditModeEnabled = false;
            isCurrentCommandDirty = false;

			// Get the commands sequence.  
			theProtocolModel.GetCommandSequence(out myCommandSequence);
			if (myCommandSequence == null)
			{
				myCommandSequence = new IProtocolCommand[0];
			}
			// Ignore any command sequence numbers specified in the file.  
			// (That is, we always set the sequence numbers programmatically,
			// since the sequence numbers read from file represent de-normalised or 
			// redundant data.  The command sequence numbers are explicitly represented
			// in the protocol file just to aid human readability of the protocol 
			// definition if examining the raw XML.)
			RecalculateCommandSequence();

			// Enable command sequence item actions if appropriate
			EnableCommandSequenceItemActions(myCommandSequence.GetLength(0));

			// Get and display the header information
			//			if ((theProtocolModel.ProtocolClass == ProtocolClass.Positive)||(theProtocolModel.ProtocolClass == ProtocolClass.Negative))
			//				cmbProtocolType.SelectedIndex = (int)ProtocolClass.Undefined;	
			//			else
			cmbProtocolType.SelectedIndex = (int) theProtocolModel.ProtocolClass;
			txtProtocolLabel.Text = theProtocolModel.ProtocolLabel;
			      		txtProtocolDesc.Text = theProtocolModel.ProtocolDesc;

			// set general maintenance type ... ignore some volume checking
			if (theProtocolModel.ProtocolClass == ProtocolClass.Maintenance || 
			    theProtocolModel.ProtocolClass == ProtocolClass.Shutdown)
				 IsMaintenanceProtocol = true;
			else IsMaintenanceProtocol = false;
				
			if (IsNewProtocol == true)
				theProtocolModel.ProtocolDescManualFill = false;

			txtProtocolDesc.Enabled = true;
			if (!theProtocolModel.ProtocolDescManualFill)
				txtProtocolDesc.Enabled = false;


			// The following was moved to AboutThisProtocolWidow
			//			txtCreationDate.Text = theProtocolModel.ProtocolCreationDate.ToString(
			//				CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
			//
			//			if (theProtocolModel.ProtocolModificationDateIsSpecified)
			//			{
			//				txtModificationDate.Text = theProtocolModel.ProtocolModificationDate.ToString(
			//					CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
			//			}
			//			else
			//			{
			//				txtModificationDate.Text = string.Empty;
			//			}

			// Get the constraints information
			txtSampleVolumeMin.Text = theProtocolModel.SampleVolumeMinimum.ToString();
			txtSampleVolumeMax.Text = theProtocolModel.SampleVolumeMaximum.ToString();
			myWorkingVolumeSampleThreshold_uL = theProtocolModel.WorkingVolumeSampleThreshold;
			txtWorkingVolumeSampleThreshold.Text = myWorkingVolumeSampleThreshold_uL.ToString();
			myWorkingVolumeLow_uL = theProtocolModel.WorkingVolumeLowVolume;
			txtWorkingVolumeLow.Text = myWorkingVolumeLow_uL.ToString();
			myWorkingVolumeHigh_uL= theProtocolModel.WorkingVolumeHighVolume;
			txtWorkingVolumeHigh.Text = myWorkingVolumeHigh_uL.ToString();

			// Bind UI fields to the new information
			lstCommands.DataSource = myCommandSequence;
			lstCommands.DisplayMember = "CommandSummary";      

			if (myCommandSequence.GetLength(0) > 0)
			{
				myCurrentCommandIndex = 0;
				lstCommands.SelectedIndex = myCurrentCommandIndex;
			}

			// Now the Command list is in place, because we're using data binding currently,
			// the first list item (command) has just been selected but the data binding
			// messes with the normal "list index changed" mechanism.  Therefore, manually
			// inspect the currently selected command type and trigger display of the
			// corresponding Command detail.
			UpdateCommandDetailView();

			// Update the 'is protocol valid' indicator (we expect this to indicate a
			// valid protocol since we've just loaded validated information!).  This also
			// triggers calculation of the quadrant count.
//			btnValidateProtocol_Click(this, new System.EventArgs()); //CWJ ADD
			RunValidation();										 //CWJ ADD

			// Re-enable form edit mode triggers now the bulk update is complete
			IsEditModeEnabled = true;
			this.Invalidate(true);
		}

		public bool WriteBackDetailsFields(int DisableVerUpdate)
		{
			bool isWriteBackComplete = false;

			// In theory this should only be called once the protocol has been validated,
			// but re-check just to confirm...
			bool isProtocolValid = ValidateProtocol();
			if ( ! isProtocolValid)
			{
				DialogResult invalidProtocolOverride = MessageBox.Show(this, 
					"Protocol definition is not valid.  Save anyway?", 
					"Save Protocol",
					MessageBoxButtons.OKCancel,
					MessageBoxIcon.Error);
				if (invalidProtocolOverride != DialogResult.OK)
				{
					return isWriteBackComplete;
				}
			}

			// Write changed data and computed fields back to the model
			// --- Write the command sequence ---
			RecalculateCommandSequence();
			theProtocolModel.SetCommandSequence(myCommandSequence);

			// --- Write the Constraints information ---
			// Write the number of quadrants
			theProtocolModel.QuadrantCount = myQuadrantCount;
			// Write the sample min/max volume
			theProtocolModel.SampleVolumeMinimum = mySampleVolumeMin_uL;
			theProtocolModel.SampleVolumeMaximum = mySampleVolumeMax_uL;
			// Write the working volume details...
			// Write the sample threshold.  If it's not specified, then also set the threshold
			// low/high values to zero.
			if (myWorkingVolumeSampleThreshold_uL == 1)
			{
				myWorkingVolumeLow_uL  = 1; //     changed from 0 to 1
				myWorkingVolumeHigh_uL = 1; //     changed from 0 to 1
			}
			theProtocolModel.WorkingVolumeSampleThreshold = myWorkingVolumeSampleThreshold_uL;
			theProtocolModel.WorkingVolumeLowVolume	 = myWorkingVolumeLow_uL;
			theProtocolModel.WorkingVolumeHighVolume = myWorkingVolumeHigh_uL;

			// --- Write the Header information ---
			// Write the protocol label
			theProtocolModel.ProtocolLabel = txtProtocolLabel.Text;
			// Write the protocol desc
			      		theProtocolModel.ProtocolDesc = txtProtocolDesc.Text;
			
			// Write the protocol author
			theProtocolModel.ProtocolAuthor = theProtocolModel.ProtocolAuthor+"";
			// Write the protocol number 
			pasreProtcolNumbers();


			// Write the creation/modification date
			if (DisableVerUpdate == 0)
			{
				if (isNewProtocol)
					theProtocolModel.ProtocolCreationDate = DateTime.Now;
				else
					theProtocolModel.ProtocolModificationDate = DateTime.Now;
			}
			else
			{
				theProtocolModel.ProtocolCreationDate		= DateTime.Now;
				theProtocolModel.ProtocolModificationDate	= DateTime.Now;
			}
			
			// Increment the Protocol definition version number
			int protocolVersion = 0;
			try
			{
				protocolVersion = int.Parse(theProtocolModel.ProtocolVersion);
			}
			catch
			{
				// The protocol version may be empty if we're defining a new protocol
				if (!isNewProtocol)
					return isWriteBackComplete;
			}

			if (DisableVerUpdate == 0) 
			{
				if (isFormChangedSinceLastSave || isNewProtocol)
					++protocolVersion;				
			}
			else
			{
				protocolVersion=1;							
			}

			theProtocolModel.ProtocolVersion = protocolVersion.ToString();
			// txtProtocolVersion.Text = protocolVersion.ToString();

            theProtocolModel.SetVersionInfoProtocolEditorVersion(Application.ProductVersion);

            string ini_version = Tesla.Common.Utilities.GetSoftwareConfigString("Editor_Options", "ini_version", "");
            theProtocolModel.SetVersionInfoProtocolEditorIniVersion(ini_version);


			// Write the protocol 'class'
			theProtocolModel.ProtocolClass = 
				(Tesla.Common.Protocol.ProtocolClass)cmbProtocolType.SelectedIndex;

			// ?? theProtocolModel.ProtocolDescManualFill = theProtocolModel.ProtocolDescManualFill;

            //theProtocolModel.ProtocolRoboSepType = SeparatorResourceManager.RobosepTypeToString(ProtocolRobosepTypes.RoboSepS);
            //theProtocolModel.ProtocolHackAbsoluteVolumeMultipler = "0";

			// All fields are written, so exit indicating success.
			isWriteBackComplete = true;
			isFormChangedSinceLastSave = false;
			return isWriteBackComplete;
		}
		
		public void pasreProtcolNumbers()
		{
			//get label
			string label = txtProtocolLabel.Text;

			if (label == null)
				return;
			int idx = 0;
			//18xxx-0yyy or 19xxx-0yyy (old)
			//18xxx-yyyy or 19xxx-yyyy or 18xxx- or 19xxx-
			while ((idx = label.IndexOf("-",idx))!=-1)
			{
				//check bounds
				if(idx-5<0 || //idx+4>=label.Length || 
					(idx-5>0 && label[idx-6]!=' ') ) //|| (idx+5<label.Length && label[idx+5]!=' '))
				{
					++idx;
					continue;
				}

				//if(label[idx-5]=='1' && (label[idx-4]=='8' || label[idx-4]=='9') &&
                    //label[idx+1]=='0' &&
                 if(label[idx - 5] >= '0' && label[idx - 5] <= '9' &&
                    label[idx - 4] >= '0' && label[idx - 4] <= '9' &&
					label[idx-3] >= '0' && label[idx-3]<='9' &&
					label[idx-2] >= '0' && label[idx-2]<='9' &&
					label[idx-1] >= '0' && label[idx-1]<='9')
				{
					theProtocolModel.ProtocolNumber1 = label.Substring(idx-5,5);
					if(idx+4>=label.Length || (idx+5<label.Length && label[idx+5]!=' '))
					{
						break;
					}
					if(label[idx+1] >= '0' && label[idx+2]<='9' &&
						label[idx+2] >= '0' && label[idx+2]<='9' &&
						label[idx+3] >= '0' && label[idx+3]<='9' &&
						label[idx+4] >= '0' && label[idx+4]<='9' )
					{
						theProtocolModel.ProtocolNumber2 = label.Substring(idx+1,4);
					}
					
					break;
				}
				++idx;
			}
		}

		private void UpdateCommandDetailView()
		{	
			UpdateCommandDetailViewEx(true);
		}
		private void UpdateCommandDetailViewEx(bool isShow)
		{	
			// Clear any previous display
			CommandPanel nextPanel = null;
			if (myCommandSequence.GetLength(0) == 0)
			{
				for (int i = 0; i < myCommandPanels.GetLength(0); ++i) {
					myCommandPanels[i].Visible = false;
				}
			}

			// Update the current panel instance and set instance-specific data            
			int selectedIndex = myCurrentCommandIndex;
			if (selectedIndex < 0 || selectedIndex >= myCommandSequence.GetLength(0))
				return;

			switch ((SubCommand)myCommandSequence[selectedIndex].CommandSubtype)
			{
				default:
				case SubCommand.HomeAllCommand:
					nextPanel = myCommandPanel;
                    nextPanel.CommandType = commandDisplayNames[(int)myCommandSequence[selectedIndex].CommandSubtype];
					break;

				case SubCommand.DemoCommand:
					nextPanel = myDemoCommandPanel;
                    nextPanel.CommandType = commandDisplayNames[(int)myCommandSequence[selectedIndex].CommandSubtype];
					ProtocolDemoCommand demoCommand = 
						(ProtocolDemoCommand)myCommandSequence[selectedIndex];
					myDemoCommandPanel.DemoCommandIterationCount = demoCommand.IterationCount;
					break;

				case SubCommand.PumpLifeCommand:
					nextPanel = myDemoCommandPanel;
                    nextPanel.CommandType = commandDisplayNames[(int)myCommandSequence[selectedIndex].CommandSubtype];
					ProtocolPumpLifeCommand pumpLifeCommand = 
						(ProtocolPumpLifeCommand)myCommandSequence[selectedIndex];
					myDemoCommandPanel.DemoCommandIterationCount = pumpLifeCommand.IterationCount;
					break;

				case SubCommand.IncubateCommand:
					nextPanel = myIncubateCommandPanel;
                    nextPanel.CommandType = commandDisplayNames[(int)myCommandSequence[selectedIndex].CommandSubtype];
					ProtocolIncubateCommand incubateCommand = 
						(ProtocolIncubateCommand)myCommandSequence[selectedIndex];
					myIncubateCommandPanel.WaitCommandTimeDuration = incubateCommand.WaitCommandTimeDuration;
					break;

				case SubCommand.SeparateCommand:
					nextPanel = mySeparateCommandPanel;
                    nextPanel.CommandType = commandDisplayNames[(int)myCommandSequence[selectedIndex].CommandSubtype];
					ProtocolSeparateCommand separateCommand =
						(ProtocolSeparateCommand)myCommandSequence[selectedIndex];
					mySeparateCommandPanel.WaitCommandTimeDuration = separateCommand.WaitCommandTimeDuration;
					break;

				case SubCommand.MixCommand:
					nextPanel = myVolumeCommandPanel;
                    nextPanel.CommandType = commandDisplayNames[(int)myCommandSequence[selectedIndex].CommandSubtype];
					ProtocolMixCommand mixCommand =
						(ProtocolMixCommand)myCommandSequence[selectedIndex];
					myVolumeCommandPanel.SourceVial = mixCommand.SourceVial;
					myVolumeCommandPanel.cmbDestinationVial.Enabled = false;      
					myVolumeCommandPanel.DestinationVial = mixCommand.SourceVial;      
					myVolumeCommandPanel.VolumeTypeSpecifier = mixCommand.VolumeTypeSpecifier;

					myVolumeCommandPanel.txtTipTubeBottomGap.Enabled = false;
			
					if (mixCommand.VolumeTypeSpecifier == VolumeType.Relative)
					{
						myVolumeCommandPanel.RelativeVolumeProportion = mixCommand.RelativeVolumeProportion;
					}
					else if (mixCommand.VolumeTypeSpecifier == VolumeType.Absolute)
					{
						myVolumeCommandPanel.AbsoluteVolume_uL = mixCommand.AbsoluteVolume_uL;
						myVolumeCommandPanel.txtTipTubeBottomGap.Enabled = true;
						myVolumeCommandPanel.TipTubeBottomGap = mixCommand.TipTubeBottomGap_uL;
					}                    
					myVolumeCommandPanel.IsVolumeSpecificationRequired = true;  // Ensure the relative/absolute value is set before setting this flag.
					
					if (!mixCommand.TipRackSpecified)
					{
						myVolumeCommandPanel.TipRack = mixCommand.TipRack;
						myVolumeCommandPanel.CheckTipRack = false;
					}
					else
					{
						myVolumeCommandPanel.TipRack = mixCommand.TipRack;
						myVolumeCommandPanel.CheckTipRack = true;
					}
					myVolumeCommandPanel.MixCycles = mixCommand.MixCycles;
					break;

				case SubCommand.TransportCommand:
					nextPanel = myWorkingVolumeCommandPanel;
                    nextPanel.CommandType = commandDisplayNames[(int)myCommandSequence[selectedIndex].CommandSubtype];
					ProtocolTransportCommand transportCommand =
						(ProtocolTransportCommand)myCommandSequence[selectedIndex];
					myWorkingVolumeCommandPanel.SourceVial = transportCommand.SourceVial;
					myWorkingVolumeCommandPanel.cmbSourceVial.Enabled = true;      
					myWorkingVolumeCommandPanel.DestinationVial = transportCommand.DestinationVial;	
					myWorkingVolumeCommandPanel.IsChangeAllowed=true; //set before VolumeTypeSpecifier
					myWorkingVolumeCommandPanel.IsThresholdStyle=false; //set before VolumeTypeSpecifier 
					myWorkingVolumeCommandPanel.VolumeTypeSpecifier = transportCommand.VolumeTypeSpecifier;				
					if (transportCommand.VolumeTypeSpecifier == VolumeType.Relative)
					{
						myWorkingVolumeCommandPanel.RelativeVolumeProportion = transportCommand.RelativeVolumeProportion;
					}
					else if (transportCommand.VolumeTypeSpecifier == VolumeType.Absolute)
					{
						myWorkingVolumeCommandPanel.AbsoluteVolume_uL = transportCommand.AbsoluteVolume_uL;
					}                    
					myWorkingVolumeCommandPanel.IsVolumeSpecificationRequired = true;        // Ensure the relative/absolute value is set before setting this flag.
					myWorkingVolumeCommandPanel.UseBufferTip = transportCommand.UseBufferTip;
					myWorkingVolumeCommandPanel.FreeAirDispense  = transportCommand.FreeAirDispense;
					if (!transportCommand.TipRackSpecified)
					{
						myWorkingVolumeCommandPanel.TipRack = transportCommand.TipRack;
						myWorkingVolumeCommandPanel.CheckTipRack = false;
					}
					else
					{
						myWorkingVolumeCommandPanel.TipRack = transportCommand.TipRack;
						myWorkingVolumeCommandPanel.CheckTipRack = true;
					}
					//MCHERE					myWorkingVolumeCommandPanel.CheckTransportDestVial();      
					break;				

				case SubCommand.TopUpVialCommand:
					nextPanel = myWorkingVolumeCommandPanel;
                    nextPanel.CommandType = commandDisplayNames[(int)myCommandSequence[selectedIndex].CommandSubtype];
					ProtocolTopUpVialCommand topUpVialCommand =
						(ProtocolTopUpVialCommand)myCommandSequence[selectedIndex];
					myWorkingVolumeCommandPanel.SourceVial = AbsoluteResourceLocation.TPC0001;      
					myWorkingVolumeCommandPanel.cmbSourceVial.Enabled = false;      
					myWorkingVolumeCommandPanel.DestinationVial = topUpVialCommand.DestinationVial;	
					
					myWorkingVolumeCommandPanel.IsChangeAllowed=true; //set before VolumeTypeSpecifier
					myWorkingVolumeCommandPanel.IsThresholdStyle=true; //set before VolumeTypeSpecifier

					myWorkingVolumeCommandPanel.VolumeTypeSpecifier = topUpVialCommand.VolumeTypeSpecifier;				
					if (topUpVialCommand.VolumeTypeSpecifier == VolumeType.Relative)
					{
						myWorkingVolumeCommandPanel.RelativeVolumeProportion = topUpVialCommand.RelativeVolumeProportion;
					}
					else if (topUpVialCommand.VolumeTypeSpecifier == VolumeType.Absolute)
					{
						myWorkingVolumeCommandPanel.AbsoluteVolume_uL = topUpVialCommand.AbsoluteVolume_uL;
					}

					myWorkingVolumeCommandPanel.IsVolumeSpecificationRequired = true;        // Ensure the relative/absolute value is set before setting this flag.
					myWorkingVolumeCommandPanel.FreeAirDispense  = topUpVialCommand.FreeAirDispense;
					if (!topUpVialCommand.TipRackSpecified)
					{
						myWorkingVolumeCommandPanel.TipRack = topUpVialCommand.TipRack;
						myWorkingVolumeCommandPanel.CheckTipRack = false;
					}
					else
					{
						myWorkingVolumeCommandPanel.TipRack = topUpVialCommand.TipRack;
						myWorkingVolumeCommandPanel.CheckTipRack = true;
					}
					//MCHERE					myWorkingVolumeCommandPanel.CheckTransportDestVial();      
					myWorkingVolumeCommandPanel.Invalidate();
					break;

				case SubCommand.ResuspendVialCommand:
					nextPanel = myWorkingVolumeCommandPanel;
                    nextPanel.CommandType = commandDisplayNames[(int)myCommandSequence[selectedIndex].CommandSubtype];
					ProtocolResuspendVialCommand resuspendVialCommand =
						(ProtocolResuspendVialCommand)myCommandSequence[selectedIndex];
					myWorkingVolumeCommandPanel.SourceVial = AbsoluteResourceLocation.TPC0001;      
					myWorkingVolumeCommandPanel.cmbSourceVial.Enabled = false;      
					myWorkingVolumeCommandPanel.DestinationVial = resuspendVialCommand.DestinationVial;	
					
					myWorkingVolumeCommandPanel.IsChangeAllowed=false; //set before VolumeTypeSpecifier
					myWorkingVolumeCommandPanel.IsThresholdStyle=true; //set before VolumeTypeSpecifier

					myWorkingVolumeCommandPanel.VolumeTypeSpecifier = resuspendVialCommand.VolumeTypeSpecifier;				
					if (resuspendVialCommand.VolumeTypeSpecifier == VolumeType.Relative)
					{
						myWorkingVolumeCommandPanel.RelativeVolumeProportion = resuspendVialCommand.RelativeVolumeProportion;
					}
					else if (resuspendVialCommand.VolumeTypeSpecifier == VolumeType.Absolute)
					{
						myWorkingVolumeCommandPanel.AbsoluteVolume_uL = resuspendVialCommand.AbsoluteVolume_uL;
					}

					myWorkingVolumeCommandPanel.IsVolumeSpecificationRequired = true;        // Ensure the relative/absolute value is set before setting this flag.
					myWorkingVolumeCommandPanel.FreeAirDispense  = resuspendVialCommand.FreeAirDispense;
					if (!resuspendVialCommand.TipRackSpecified)
					{
						myWorkingVolumeCommandPanel.TipRack = resuspendVialCommand.TipRack;
						myWorkingVolumeCommandPanel.CheckTipRack = false;
					}
					else
					{
						myWorkingVolumeCommandPanel.TipRack = resuspendVialCommand.TipRack;
						myWorkingVolumeCommandPanel.CheckTipRack = true;
					}
					//MCHERE					myWorkingVolumeCommandPanel.CheckTransportDestVial();      
					myWorkingVolumeCommandPanel.Invalidate();
					break;

				case SubCommand.FlushCommand:
					nextPanel = myVolumeMaintenanceCommandPanel;
                    nextPanel.CommandType = commandDisplayNames[(int)myCommandSequence[selectedIndex].CommandSubtype];
					ProtocolFlushCommand flushCommand =
						(ProtocolFlushCommand)myCommandSequence[selectedIndex];
					myVolumeMaintenanceCommandPanel.SourceVial = flushCommand.SourceVial;
					myVolumeMaintenanceCommandPanel.DestinationVial = flushCommand.DestinationVial;
					myVolumeMaintenanceCommandPanel.HomeFlag = flushCommand.HomeFlag;
					if (!flushCommand.TipRackSpecified)
					{
						myVolumeMaintenanceCommandPanel.TipRack = flushCommand.TipRack;
						myVolumeMaintenanceCommandPanel.CheckTipRack = false;
					}
					else
					{
						myVolumeMaintenanceCommandPanel.TipRack = flushCommand.TipRack;
						myVolumeMaintenanceCommandPanel.CheckTipRack = true;
					}
					myVolumeMaintenanceCommandPanel.IsVolumeSpecificationRequired = false;      
					break;

				case SubCommand.PrimeCommand:
					nextPanel = myVolumeMaintenanceCommandPanel;
                    nextPanel.CommandType = commandDisplayNames[(int)myCommandSequence[selectedIndex].CommandSubtype];
					ProtocolPrimeCommand primeCommand =
						(ProtocolPrimeCommand)myCommandSequence[selectedIndex];
					myVolumeMaintenanceCommandPanel.SourceVial = primeCommand.SourceVial;
					myVolumeMaintenanceCommandPanel.DestinationVial = primeCommand.DestinationVial;
					myVolumeMaintenanceCommandPanel.HomeFlag = primeCommand.HomeFlag;
					if (!primeCommand.TipRackSpecified)
					{
						myVolumeMaintenanceCommandPanel.TipRack = primeCommand.TipRack;
						myVolumeMaintenanceCommandPanel.CheckTipRack = false;
					}
					else
					{
						myVolumeMaintenanceCommandPanel.TipRack = primeCommand.TipRack;
						myVolumeMaintenanceCommandPanel.CheckTipRack = true;
					}
					myVolumeMaintenanceCommandPanel.IsVolumeSpecificationRequired = false;      
					break;

				case SubCommand.PauseCommand:
					nextPanel = myPauseCommandPanel;
                    nextPanel.CommandType = commandDisplayNames[(int)myCommandSequence[selectedIndex].CommandSubtype];
					ProtocolPauseCommand pauseCommand =
						(ProtocolPauseCommand)myCommandSequence[selectedIndex];
					break;

                case SubCommand.TopUpMixTransSepTransCommand:
                    {
                        nextPanel = myTopUpMixTransSepTransCommandPanel;
                        nextPanel.CommandType = commandDisplayNames[(int)myCommandSequence[selectedIndex].CommandSubtype];
                        ProtocolTopUpMixTransSepTransCommand cmd =
                            (ProtocolTopUpMixTransSepTransCommand)myCommandSequence[selectedIndex];
                        GenericMultiStepCommandPanel panel = myTopUpMixTransSepTransCommandPanel;

                        panel.SourceVial = AbsoluteResourceLocation.TPC0001;
                        panel.cmbSourceVial.Enabled = false;
                        panel.IsChangeAllowed = true; //set before IsVolumeSpecificationRequired
                        panel.IsThresholdStyle = true; //set before IsVolumeSpecificationRequired  

                        panel.SetVolumeCommandPanelParams(AbsoluteResourceLocation.TPC0001, cmd.DestinationVial,
                                  cmd.VolumeTypeSpecifier, cmd.RelativeVolumeProportion, cmd.AbsoluteVolume_uL,
                                  cmd.TipRack, cmd.TipRackSpecified, cmd.CommandLabel, cmd.CommandExtensionTime);

                        panel.SetVolumeCommandPanelParamsMix(cmd.DestinationVial, cmd.DestinationVial,
                                  cmd.VolumeTypeSpecifier2, cmd.RelativeVolumeProportion2, cmd.AbsoluteVolume_uL2,
                                  cmd.MixCycles, cmd.TipTubeBottomGap_uL);
                        panel.SetVolumeCommandPanelParams2(cmd.SourceVial2, cmd.DestinationVial2,
                                  cmd.VolumeTypeSpecifier3, cmd.RelativeVolumeProportion3, cmd.AbsoluteVolume_uL3);
                        panel.SetVolumeCommandPanelParams3(cmd.SourceVial3, cmd.DestinationVial3,
                                  cmd.VolumeTypeSpecifier4, cmd.RelativeVolumeProportion4, cmd.AbsoluteVolume_uL4);

                        panel.WaitCommandTimeDuration = cmd.WaitCommandTimeDuration;

                        panel.IsVolumeSpecificationRequired = true;        // Ensure the relative/absolute value is set before setting this flag.
                        panel.CheckTransportDestVial();
                        panel.CheckTransportSourceVial();
                    }

                    break;
                case SubCommand.TopUpMixTransCommand:
                    {
                        nextPanel = myTopUpMixTransCommandPanel;
                        nextPanel.CommandType = commandDisplayNames[(int)myCommandSequence[selectedIndex].CommandSubtype];
                        ProtocolTopUpMixTransCommand cmd =
                            (ProtocolTopUpMixTransCommand)myCommandSequence[selectedIndex];
                        GenericMultiStepCommandPanel panel = myTopUpMixTransCommandPanel;

                        panel.SourceVial = AbsoluteResourceLocation.TPC0001;
                        panel.cmbSourceVial.Enabled = false;
                        panel.IsChangeAllowed = true; //set before IsVolumeSpecificationRequired
                        panel.IsThresholdStyle = true; //set before IsVolumeSpecificationRequired  

                        panel.SetVolumeCommandPanelParams(AbsoluteResourceLocation.TPC0001, cmd.DestinationVial,
                                  cmd.VolumeTypeSpecifier, cmd.RelativeVolumeProportion, cmd.AbsoluteVolume_uL,
                                  cmd.TipRack, cmd.TipRackSpecified, cmd.CommandLabel, cmd.CommandExtensionTime);

                        panel.SetVolumeCommandPanelParamsMix(cmd.DestinationVial, cmd.DestinationVial,
                                  cmd.VolumeTypeSpecifier2, cmd.RelativeVolumeProportion2, cmd.AbsoluteVolume_uL2,
                                  cmd.MixCycles, cmd.TipTubeBottomGap_uL);
                        panel.SetVolumeCommandPanelParams2(cmd.SourceVial2, cmd.DestinationVial2,
                                  cmd.VolumeTypeSpecifier3, cmd.RelativeVolumeProportion3, cmd.AbsoluteVolume_uL3);



                        panel.IsVolumeSpecificationRequired = true;        // Ensure the relative/absolute value is set before setting this flag.
                        panel.CheckTransportDestVial();
                        panel.CheckTransportSourceVial();
                    }

                    break;
                case SubCommand.TopUpTransSepTransCommand:
                    {
                        nextPanel = myTopUpTransSepTransCommandPanel;
                        nextPanel.CommandType = commandDisplayNames[(int)myCommandSequence[selectedIndex].CommandSubtype];
                        ProtocolTopUpTransSepTransCommand cmd =
                            (ProtocolTopUpTransSepTransCommand)myCommandSequence[selectedIndex];
                        GenericMultiStepCommandPanel panel = myTopUpTransSepTransCommandPanel;

                        panel.SourceVial = AbsoluteResourceLocation.TPC0001;
                        panel.cmbSourceVial.Enabled = false;
                        panel.IsChangeAllowed = true; //set before IsVolumeSpecificationRequired
                        panel.IsThresholdStyle = true; //set before IsVolumeSpecificationRequired  

                        panel.SetVolumeCommandPanelParams(AbsoluteResourceLocation.TPC0001, cmd.DestinationVial,
                                  cmd.VolumeTypeSpecifier, cmd.RelativeVolumeProportion, cmd.AbsoluteVolume_uL,
                                  cmd.TipRack, cmd.TipRackSpecified, cmd.CommandLabel, cmd.CommandExtensionTime);

                        panel.SetVolumeCommandPanelParams2(cmd.SourceVial2, cmd.DestinationVial2,
                                  cmd.VolumeTypeSpecifier2, cmd.RelativeVolumeProportion2, cmd.AbsoluteVolume_uL2);
                        panel.SetVolumeCommandPanelParams3(cmd.SourceVial3, cmd.DestinationVial3,
                                  cmd.VolumeTypeSpecifier3, cmd.RelativeVolumeProportion3, cmd.AbsoluteVolume_uL3);

                        panel.WaitCommandTimeDuration = cmd.WaitCommandTimeDuration;

                        panel.IsVolumeSpecificationRequired = true;        // Ensure the relative/absolute value is set before setting this flag.
                        panel.CheckTransportDestVial();
                        panel.CheckTransportSourceVial();
                    }

                    break;
                case SubCommand.TopUpTransCommand:
                    {
                        nextPanel = myTopUpTransCommandPanel;
                        nextPanel.CommandType = commandDisplayNames[(int)myCommandSequence[selectedIndex].CommandSubtype];
                        ProtocolTopUpTransCommand cmd =
                            (ProtocolTopUpTransCommand)myCommandSequence[selectedIndex];
                        GenericMultiStepCommandPanel panel = myTopUpTransCommandPanel;

                        panel.SourceVial = AbsoluteResourceLocation.TPC0001;
                        panel.cmbSourceVial.Enabled = false;
                        panel.IsChangeAllowed = true; //set before IsVolumeSpecificationRequired
                        panel.IsThresholdStyle = true; //set before IsVolumeSpecificationRequired  

                        panel.SetVolumeCommandPanelParams(AbsoluteResourceLocation.TPC0001, cmd.DestinationVial,
                                  cmd.VolumeTypeSpecifier, cmd.RelativeVolumeProportion, cmd.AbsoluteVolume_uL,
                                  cmd.TipRack, cmd.TipRackSpecified, cmd.CommandLabel, cmd.CommandExtensionTime);

                        panel.SetVolumeCommandPanelParams2(cmd.SourceVial2, cmd.DestinationVial2,
                                  cmd.VolumeTypeSpecifier2, cmd.RelativeVolumeProportion2, cmd.AbsoluteVolume_uL2);


                        panel.IsVolumeSpecificationRequired = true;        // Ensure the relative/absolute value is set before setting this flag.
                        panel.CheckTransportDestVial();
                        panel.CheckTransportSourceVial();
                    }

                    break;
                case SubCommand.ResusMixSepTransCommand:
                    {
                        nextPanel = myResusMixSepTransCommandPanel;
                        nextPanel.CommandType = commandDisplayNames[(int)myCommandSequence[selectedIndex].CommandSubtype];
                        ProtocolResusMixSepTransCommand cmd =
                            (ProtocolResusMixSepTransCommand)myCommandSequence[selectedIndex];
                        GenericMultiStepCommandPanel panel = myResusMixSepTransCommandPanel;

                        panel.SourceVial = AbsoluteResourceLocation.TPC0001;
                        panel.cmbSourceVial.Enabled = false;
                        panel.IsChangeAllowed = false; //set before IsVolumeSpecificationRequired
                        panel.IsThresholdStyle = true; //set before IsVolumeSpecificationRequired  

                        panel.SetVolumeCommandPanelParams(AbsoluteResourceLocation.TPC0001, cmd.DestinationVial,
                                  cmd.VolumeTypeSpecifier, cmd.RelativeVolumeProportion, cmd.AbsoluteVolume_uL,
                                  cmd.TipRack, cmd.TipRackSpecified, cmd.CommandLabel, cmd.CommandExtensionTime);

                        panel.SetVolumeCommandPanelParamsMix(cmd.DestinationVial, cmd.DestinationVial,
                                  cmd.VolumeTypeSpecifier2, cmd.RelativeVolumeProportion2, cmd.AbsoluteVolume_uL2,
                                  cmd.MixCycles, cmd.TipTubeBottomGap_uL);
                        panel.SetVolumeCommandPanelParams2(cmd.SourceVial2, cmd.DestinationVial2,
                                  cmd.VolumeTypeSpecifier3, cmd.RelativeVolumeProportion3, cmd.AbsoluteVolume_uL3);

                        panel.WaitCommandTimeDuration = cmd.WaitCommandTimeDuration;

                        panel.IsVolumeSpecificationRequired = true;        // Ensure the relative/absolute value is set before setting this flag.
                        panel.CheckTransportDestVial();
                        panel.CheckTransportSourceVial();
                    }

                    break;
                case SubCommand.ResusMixCommand:
                    {
                        nextPanel = myResusMixCommandPanel;
                        nextPanel.CommandType = commandDisplayNames[(int)myCommandSequence[selectedIndex].CommandSubtype];
                        ProtocolResusMixCommand cmd =
                            (ProtocolResusMixCommand)myCommandSequence[selectedIndex];
                        GenericMultiStepCommandPanel panel = myResusMixCommandPanel;

                        panel.SourceVial = AbsoluteResourceLocation.TPC0001;
                        panel.cmbSourceVial.Enabled = false;
                        panel.IsChangeAllowed = false; //set before IsVolumeSpecificationRequired
                        panel.IsThresholdStyle = true; //set before IsVolumeSpecificationRequired  

                        panel.SetVolumeCommandPanelParams(AbsoluteResourceLocation.TPC0001, cmd.DestinationVial,
                                  cmd.VolumeTypeSpecifier, cmd.RelativeVolumeProportion, cmd.AbsoluteVolume_uL,
                                  cmd.TipRack, cmd.TipRackSpecified, cmd.CommandLabel, cmd.CommandExtensionTime);

                        panel.SetVolumeCommandPanelParamsMix(cmd.DestinationVial, cmd.DestinationVial,
                                  cmd.VolumeTypeSpecifier2, cmd.RelativeVolumeProportion2, cmd.AbsoluteVolume_uL2,
                                  cmd.MixCycles, cmd.TipTubeBottomGap_uL);


                        panel.IsVolumeSpecificationRequired = true;        // Ensure the relative/absolute value is set before setting this flag.
                        panel.CheckTransportDestVial();
                        panel.CheckTransportSourceVial();
                    }

                    break;
                case SubCommand.MixTransCommand:
                    {
                        nextPanel = myMixTransCommandPanel;
                        nextPanel.CommandType = commandDisplayNames[(int)myCommandSequence[selectedIndex].CommandSubtype];

                        ProtocolMixTransCommand cmd =
                            (ProtocolMixTransCommand)myCommandSequence[selectedIndex];
                        GenericMultiStepCommandPanel2 panel = myMixTransCommandPanel;


                        panel.SetVolumeCommandPanelParams(cmd.SourceVial, cmd.DestinationVial,
                                  cmd.VolumeTypeSpecifier, cmd.RelativeVolumeProportion, cmd.AbsoluteVolume_uL,
                                  cmd.TipRack, cmd.TipRackSpecified, cmd.CommandLabel, cmd.CommandExtensionTime);

                        panel.SetVolumeCommandPanelParams2(cmd.SourceVial2, cmd.DestinationVial2,
                                  cmd.VolumeTypeSpecifier2, cmd.RelativeVolumeProportion2, cmd.AbsoluteVolume_uL2);

                        panel.txtTipTubeBottomGap.Enabled = (panel.VolumeTypeSpecifier == VolumeType.Absolute);

                        panel.MixCycles = cmd.MixCycles;
                        panel.TipTubeBottomGap = cmd.TipTubeBottomGap_uL;
                        panel.IsVolumeSpecificationRequired = true;        // Ensure the relative/absolute value is set before setting this flag.
                        panel.CheckTransportDestVial();
                        panel.CheckTransportSourceVial();
                    }
                    break;

                
			}		

			// Set common panel data
            //CommandType MUST BE SET CORRECTLY BEFORE VOLUME CHANGES!!!
            //nextPanel.CommandType = commandDisplayNames[(int)myCommandSequence[selectedIndex].CommandSubtype];
				//cmbCommandTypes.Items[(int)myCommandSequence[selectedIndex].CommandSubtype].ToString();
			nextPanel.CommandLabel = myCommandSequence[selectedIndex].CommandLabel;
			nextPanel.CommandExtensionTime = myCommandSequence[selectedIndex].CommandExtensionTime;
			
			// Update the command details display
			if (nextPanel != myCurrentCommandPanel || ( ! nextPanel.Visible))
			{											
				if (myCurrentCommandPanel == null)
				{					
					nextPanel.Location = myCommandPanels[(int)CommandPanel.PanelType.CommandPanel].Location;											
				}
				else
				{
					nextPanel.Location = myCurrentCommandPanel.Location;
					
					myCurrentCommandPanel.SendToBack();
					myCurrentCommandPanel.Visible = false;
				}				

				nextPanel.BringToFront();
				nextPanel.Visible = true & isShow;
				myCurrentCommandPanel = nextPanel;

			}	
		}

		private void RecalculateCommandSequence()
		{
			// Update the command sequence numbers (using 'natural' numbers, that is one-based)
			// to match the current command sequence.  
			// NOTE: we ignore any sequence numbers read from file just
			// in case they do not match the actual list order.
			for (int i = 0; i < myCommandSequence.GetLength(0); ++i)
			{
				myCommandSequence[i].CommandSequenceNumber = (uint)(i+1);
			}
		}

		private void PersistCommandDetailView()
		{
			// Store the current panel instance data back in the command sequence 
			// (that is, persist on-screen changes to the command sequence).  This
			// routine assumes that the data has already been validated.
			CommandPanel currentPanel = null;
			int selectedIndex = myCurrentCommandIndex;
			switch ((SubCommand)myCommandSequence[selectedIndex].CommandSubtype)
			{
				default:
				case SubCommand.HomeAllCommand:
					currentPanel = myCommandPanel;	                    
					break;

				case SubCommand.DemoCommand:
					currentPanel = myDemoCommandPanel;
					ProtocolDemoCommand demoCommand = 
						(ProtocolDemoCommand)myCommandSequence[selectedIndex];
					demoCommand.IterationCount = myDemoCommandPanel.DemoCommandIterationCount;
					break;

				case SubCommand.PumpLifeCommand:
					currentPanel = myDemoCommandPanel;
					ProtocolPumpLifeCommand pumpLifeCommand = 
						(ProtocolPumpLifeCommand)myCommandSequence[selectedIndex];
					pumpLifeCommand.IterationCount = myDemoCommandPanel.DemoCommandIterationCount;
					break;

				case SubCommand.IncubateCommand:
					currentPanel = myIncubateCommandPanel;
					ProtocolIncubateCommand incubateCommand = 
						(ProtocolIncubateCommand)myCommandSequence[selectedIndex];
					incubateCommand.WaitCommandTimeDuration = myIncubateCommandPanel.WaitCommandTimeDuration;
					break;

				case SubCommand.SeparateCommand:
					currentPanel = mySeparateCommandPanel;
					ProtocolSeparateCommand separateCommand =
						(ProtocolSeparateCommand)myCommandSequence[selectedIndex];
					separateCommand.WaitCommandTimeDuration = mySeparateCommandPanel.WaitCommandTimeDuration;
					break;

				case SubCommand.MixCommand:
					currentPanel = myVolumeCommandPanel;
					ProtocolMixCommand mixCommand =
						(ProtocolMixCommand)myCommandSequence[selectedIndex];
					mixCommand.SourceVial = myVolumeCommandPanel.SourceVial;
					mixCommand.DestinationVial = myVolumeCommandPanel.DestinationVial;
					//myVolumeCommandPanel.IsVolumeSpecificationRequired = false;      
					mixCommand.VolumeTypeSpecifier = myVolumeCommandPanel.VolumeTypeSpecifier;
					if (mixCommand.VolumeTypeSpecifier == VolumeType.Relative)
					{
						mixCommand.RelativeVolumeProportion = myVolumeCommandPanel.RelativeVolumeProportion;
					}
					else if (mixCommand.VolumeTypeSpecifier == VolumeType.Absolute)
					{
						mixCommand.AbsoluteVolume_uL = myVolumeCommandPanel.AbsoluteVolume_uL;
					}
					mixCommand.VolumeTypeSpecifier = myVolumeCommandPanel.VolumeTypeSpecifier;
					mixCommand.TipRack = myVolumeCommandPanel.TipRack;      
					mixCommand.TipRackSpecified = myVolumeCommandPanel.TipRackSpecified;      

					mixCommand.MixCycles= myVolumeCommandPanel.MixCycles;
					mixCommand.TipTubeBottomGap_uL = myVolumeCommandPanel.TipTubeBottomGap;
					break;

				case SubCommand.TransportCommand:
					currentPanel = myWorkingVolumeCommandPanel;
					ProtocolTransportCommand transportCommand =
						(ProtocolTransportCommand)myCommandSequence[selectedIndex];
					transportCommand.SourceVial = myWorkingVolumeCommandPanel.SourceVial;
					transportCommand.DestinationVial = myWorkingVolumeCommandPanel.DestinationVial;
					//myWorkingVolumeCommandPanel.IsChangeAllowed=true; //set before IsVolumeSpecificationRequired
					//myWorkingVolumeCommandPanel.IsThresholdStyle=false; //set before IsVolumeSpecificationRequired
					//myWorkingVolumeCommandPanel.IsVolumeSpecificationRequired = true;      
					transportCommand.VolumeTypeSpecifier = myWorkingVolumeCommandPanel.VolumeTypeSpecifier;
					if (transportCommand.VolumeTypeSpecifier == VolumeType.Relative)
					{
						transportCommand.RelativeVolumeProportion = myWorkingVolumeCommandPanel.RelativeVolumeProportion;
					}
					else if (transportCommand.VolumeTypeSpecifier == VolumeType.Absolute)
					{
						transportCommand.AbsoluteVolume_uL = myWorkingVolumeCommandPanel.AbsoluteVolume_uL;
					}
					transportCommand.UseBufferTip = myWorkingVolumeCommandPanel.UseBufferTip;
					transportCommand.FreeAirDispense  = myWorkingVolumeCommandPanel.FreeAirDispense;
					transportCommand.TipRack = myWorkingVolumeCommandPanel.TipRack;      
					transportCommand.TipRackSpecified = myWorkingVolumeCommandPanel.TipRackSpecified;      
					break;			
						
				case SubCommand.TopUpVialCommand:
					currentPanel = myWorkingVolumeCommandPanel;
					ProtocolTopUpVialCommand topUpVialCommand =
						(ProtocolTopUpVialCommand)myCommandSequence[selectedIndex];
					topUpVialCommand.SourceVial = AbsoluteResourceLocation.TPC0001;      
					topUpVialCommand.DestinationVial = myWorkingVolumeCommandPanel.DestinationVial;
					
					//myWorkingVolumeCommandPanel.IsChangeAllowed=true; //set before IsVolumeSpecificationRequired
					//myWorkingVolumeCommandPanel.IsThresholdStyle=true; //set before IsVolumeSpecificationRequired

					//myWorkingVolumeCommandPanel.IsVolumeSpecificationRequired = true;      
					topUpVialCommand.VolumeTypeSpecifier = myWorkingVolumeCommandPanel.VolumeTypeSpecifier;
					if (topUpVialCommand.VolumeTypeSpecifier == VolumeType.Relative)
					{
						topUpVialCommand.RelativeVolumeProportion = myWorkingVolumeCommandPanel.RelativeVolumeProportion;
					}
					else if (topUpVialCommand.VolumeTypeSpecifier == VolumeType.Absolute)
					{
						topUpVialCommand.AbsoluteVolume_uL = myWorkingVolumeCommandPanel.AbsoluteVolume_uL;
					}
					topUpVialCommand.FreeAirDispense  = myWorkingVolumeCommandPanel.FreeAirDispense;                    
					topUpVialCommand.TipRack = myWorkingVolumeCommandPanel.TipRack;      
					topUpVialCommand.TipRackSpecified = myWorkingVolumeCommandPanel.TipRackSpecified;      
					break;

				case SubCommand.ResuspendVialCommand:
					currentPanel = myWorkingVolumeCommandPanel;
					ProtocolResuspendVialCommand resuspendVialCommand =
						(ProtocolResuspendVialCommand)myCommandSequence[selectedIndex];
					
					//myWorkingVolumeCommandPanel.IsChangeAllowed=false; //set before IsVolumeSpecificationRequired
					//myWorkingVolumeCommandPanel.IsThresholdStyle=true; //set before IsVolumeSpecificationRequired

					//myWorkingVolumeCommandPanel.IsVolumeSpecificationRequired = true;      
					resuspendVialCommand.SourceVial = AbsoluteResourceLocation.TPC0001;      
					resuspendVialCommand.DestinationVial = myWorkingVolumeCommandPanel.DestinationVial;
					resuspendVialCommand.VolumeTypeSpecifier = myWorkingVolumeCommandPanel.VolumeTypeSpecifier;
					if (resuspendVialCommand.VolumeTypeSpecifier == VolumeType.Relative)
					{
						resuspendVialCommand.RelativeVolumeProportion = myWorkingVolumeCommandPanel.RelativeVolumeProportion;
					}
					else if (resuspendVialCommand.VolumeTypeSpecifier == VolumeType.Absolute)
					{
						resuspendVialCommand.AbsoluteVolume_uL = myWorkingVolumeCommandPanel.AbsoluteVolume_uL;
					}
					resuspendVialCommand.FreeAirDispense  = myWorkingVolumeCommandPanel.FreeAirDispense;
					resuspendVialCommand.TipRack = myWorkingVolumeCommandPanel.TipRack;      
					resuspendVialCommand.TipRackSpecified = myWorkingVolumeCommandPanel.TipRackSpecified;      
					break;

				case SubCommand.FlushCommand:
					currentPanel = myVolumeMaintenanceCommandPanel;
					ProtocolFlushCommand flushCommand =
						(ProtocolFlushCommand)myCommandSequence[selectedIndex];
					flushCommand.SourceVial = myVolumeMaintenanceCommandPanel.SourceVial;
					flushCommand.DestinationVial = myVolumeMaintenanceCommandPanel.DestinationVial;
					flushCommand.HomeFlag = myVolumeMaintenanceCommandPanel.HomeFlag;
					myVolumeMaintenanceCommandPanel.IsVolumeSpecificationRequired = false;      
					flushCommand.TipRack = myVolumeMaintenanceCommandPanel.TipRack;      
					flushCommand.TipRackSpecified = myVolumeMaintenanceCommandPanel.TipRackSpecified;      
					break;

				case SubCommand.PrimeCommand:
					currentPanel = myVolumeMaintenanceCommandPanel;
					ProtocolPrimeCommand primeCommand =
						(ProtocolPrimeCommand)myCommandSequence[selectedIndex];
					primeCommand.SourceVial = myVolumeMaintenanceCommandPanel.SourceVial; 
					primeCommand.DestinationVial = myVolumeMaintenanceCommandPanel.DestinationVial;
					primeCommand.HomeFlag = myVolumeMaintenanceCommandPanel.HomeFlag;
					myVolumeMaintenanceCommandPanel.IsVolumeSpecificationRequired = false;      
					primeCommand.TipRack = myVolumeMaintenanceCommandPanel.TipRack;      
					primeCommand.TipRackSpecified = myVolumeMaintenanceCommandPanel.TipRackSpecified;      
					break;
				
				case SubCommand.PauseCommand:
					currentPanel = myPauseCommandPanel;
					ProtocolPauseCommand pauseCommand =
						(ProtocolPauseCommand)myCommandSequence[selectedIndex];
					break;

                case SubCommand.TopUpMixTransSepTransCommand:
                    {
                        currentPanel = myTopUpMixTransSepTransCommandPanel;
                        ProtocolTopUpMixTransSepTransCommand cmd =
                            (ProtocolTopUpMixTransSepTransCommand)myCommandSequence[selectedIndex];
                        GenericMultiStepCommandPanel panel = myTopUpMixTransSepTransCommandPanel;
                        cmd.SourceVial = panel.SourceVial;
                        cmd.SourceVial2 = panel.SourceVial2;
                        cmd.SourceVial3 = panel.SourceVial3;
                        cmd.DestinationVial = panel.DestinationVial;
                        cmd.DestinationVial2 = panel.DestinationVial2;
                        cmd.DestinationVial3 = panel.DestinationVial3;
                        cmd.TipRack = panel.TipRack;
                        cmd.TipRackSpecified = panel.TipRackSpecified;
                        cmd.VolumeTypeSpecifier = panel.VolumeTypeSpecifier;
                        if (cmd.VolumeTypeSpecifier == VolumeType.Relative)
                        {
                            cmd.RelativeVolumeProportion = panel.RelativeVolumeProportion;
                        }
                        else if (cmd.VolumeTypeSpecifier == VolumeType.Absolute)
                        {
                            cmd.AbsoluteVolume_uL = panel.AbsoluteVolume_uL;
                        }

                        cmd.VolumeTypeSpecifier2 = panel.VolumeTypeSpecifierMix;
                        if (cmd.VolumeTypeSpecifier2 == VolumeType.Relative)
                        {
                            cmd.RelativeVolumeProportion2 = panel.RelativeVolumeProportionMix;
                        }
                        else if (cmd.VolumeTypeSpecifier2 == VolumeType.Absolute)
                        {
                            cmd.AbsoluteVolume_uL2 = panel.AbsoluteVolume_uLMix;
                        }

                        cmd.VolumeTypeSpecifier3 = panel.VolumeTypeSpecifier2;
                        if (cmd.VolumeTypeSpecifier3 == VolumeType.Relative)
                        {
                            cmd.RelativeVolumeProportion3 = panel.RelativeVolumeProportion2;
                        }
                        else if (cmd.VolumeTypeSpecifier3 == VolumeType.Absolute)
                        {
                            cmd.AbsoluteVolume_uL3 = panel.AbsoluteVolume_uL2;
                        }
                        cmd.VolumeTypeSpecifier4 = panel.VolumeTypeSpecifier3;
                        if (cmd.VolumeTypeSpecifier4 == VolumeType.Relative)
                        {
                            cmd.RelativeVolumeProportion4 = panel.RelativeVolumeProportion3;
                        }
                        else if (cmd.VolumeTypeSpecifier4 == VolumeType.Absolute)
                        {
                            cmd.AbsoluteVolume_uL4 = panel.AbsoluteVolume_uL3;
                        }

                        cmd.MixCycles = panel.MixCycles;
                        cmd.TipTubeBottomGap_uL = panel.TipTubeBottomGap;
                        cmd.WaitCommandTimeDuration = panel.WaitCommandTimeDuration;
                    }
                    break;
                case SubCommand.TopUpMixTransCommand:
                    {
                        currentPanel = myTopUpMixTransCommandPanel;
                        ProtocolTopUpMixTransCommand cmd =
                            (ProtocolTopUpMixTransCommand)myCommandSequence[selectedIndex];
                        GenericMultiStepCommandPanel panel = myTopUpMixTransCommandPanel;
                        cmd.SourceVial = panel.SourceVial;
                        cmd.SourceVial2 = panel.SourceVial2;
                        cmd.DestinationVial = panel.DestinationVial;
                        cmd.DestinationVial2 = panel.DestinationVial2;
                        cmd.TipRack = panel.TipRack;
                        cmd.TipRackSpecified = panel.TipRackSpecified;
                        cmd.VolumeTypeSpecifier = panel.VolumeTypeSpecifier;
                        if (cmd.VolumeTypeSpecifier == VolumeType.Relative)
                        {
                            cmd.RelativeVolumeProportion = panel.RelativeVolumeProportion;
                        }
                        else if (cmd.VolumeTypeSpecifier == VolumeType.Absolute)
                        {
                            cmd.AbsoluteVolume_uL = panel.AbsoluteVolume_uL;
                        }
                        cmd.VolumeTypeSpecifier2 = panel.VolumeTypeSpecifierMix;
                        if (cmd.VolumeTypeSpecifier2 == VolumeType.Relative)
                        {
                            cmd.RelativeVolumeProportion2 = panel.RelativeVolumeProportionMix;
                        }
                        else if (cmd.VolumeTypeSpecifier2 == VolumeType.Absolute)
                        {
                            cmd.AbsoluteVolume_uL2 = panel.AbsoluteVolume_uLMix;
                        }
                        cmd.VolumeTypeSpecifier3 = panel.VolumeTypeSpecifier2;
                        if (cmd.VolumeTypeSpecifier3 == VolumeType.Relative)
                        {
                            cmd.RelativeVolumeProportion3 = panel.RelativeVolumeProportion2;
                        }
                        else if (cmd.VolumeTypeSpecifier3 == VolumeType.Absolute)
                        {
                            cmd.AbsoluteVolume_uL3 = panel.AbsoluteVolume_uL2;
                        }

                        cmd.MixCycles = panel.MixCycles;
                        cmd.TipTubeBottomGap_uL = panel.TipTubeBottomGap;
                    }
                    break;
                case SubCommand.TopUpTransSepTransCommand:
                    {
                        currentPanel = myTopUpTransSepTransCommandPanel;
                        ProtocolTopUpTransSepTransCommand cmd =
                            (ProtocolTopUpTransSepTransCommand)myCommandSequence[selectedIndex];
                        GenericMultiStepCommandPanel panel = myTopUpTransSepTransCommandPanel;
                        cmd.SourceVial = panel.SourceVial;
                        cmd.SourceVial2 = panel.SourceVial2;
                        cmd.SourceVial3 = panel.SourceVial3;
                        cmd.DestinationVial = panel.DestinationVial;
                        cmd.DestinationVial2 = panel.DestinationVial2;
                        cmd.DestinationVial3 = panel.DestinationVial3;
                        cmd.TipRack = panel.TipRack;
                        cmd.TipRackSpecified = panel.TipRackSpecified;
                        cmd.VolumeTypeSpecifier = panel.VolumeTypeSpecifier;
                        if (cmd.VolumeTypeSpecifier == VolumeType.Relative)
                        {
                            cmd.RelativeVolumeProportion = panel.RelativeVolumeProportion;
                        }
                        else if (cmd.VolumeTypeSpecifier == VolumeType.Absolute)
                        {
                            cmd.AbsoluteVolume_uL = panel.AbsoluteVolume_uL;
                        }
                        cmd.VolumeTypeSpecifier2 = panel.VolumeTypeSpecifier2;
                        if (cmd.VolumeTypeSpecifier2 == VolumeType.Relative)
                        {
                            cmd.RelativeVolumeProportion2 = panel.RelativeVolumeProportion2;
                        }
                        else if (cmd.VolumeTypeSpecifier2 == VolumeType.Absolute)
                        {
                            cmd.AbsoluteVolume_uL2 = panel.AbsoluteVolume_uL2;
                        }
                        cmd.VolumeTypeSpecifier3 = panel.VolumeTypeSpecifier3;
                        if (cmd.VolumeTypeSpecifier3 == VolumeType.Relative)
                        {
                            cmd.RelativeVolumeProportion3 = panel.RelativeVolumeProportion3;
                        }
                        else if (cmd.VolumeTypeSpecifier3 == VolumeType.Absolute)
                        {
                            cmd.AbsoluteVolume_uL3 = panel.AbsoluteVolume_uL3;
                        }
                        
                        cmd.WaitCommandTimeDuration = panel.WaitCommandTimeDuration;
                    }
                    break;
                case SubCommand.TopUpTransCommand:
                    {
                        currentPanel = myTopUpTransCommandPanel;
                        ProtocolTopUpTransCommand cmd =
                            (ProtocolTopUpTransCommand)myCommandSequence[selectedIndex];
                        GenericMultiStepCommandPanel panel = myTopUpTransCommandPanel;
                        cmd.SourceVial = panel.SourceVial;
                        cmd.SourceVial2 = panel.SourceVial2;
                        cmd.DestinationVial = panel.DestinationVial;
                        cmd.DestinationVial2 = panel.DestinationVial2;
                        cmd.TipRack = panel.TipRack;
                        cmd.TipRackSpecified = panel.TipRackSpecified;
                        cmd.VolumeTypeSpecifier = panel.VolumeTypeSpecifier;
                        if (cmd.VolumeTypeSpecifier == VolumeType.Relative)
                        {
                            cmd.RelativeVolumeProportion = panel.RelativeVolumeProportion;
                        }
                        else if (cmd.VolumeTypeSpecifier == VolumeType.Absolute)
                        {
                            cmd.AbsoluteVolume_uL = panel.AbsoluteVolume_uL;
                        }
                        cmd.VolumeTypeSpecifier2 = panel.VolumeTypeSpecifier2;
                        if (cmd.VolumeTypeSpecifier2 == VolumeType.Relative)
                        {
                            cmd.RelativeVolumeProportion2 = panel.RelativeVolumeProportion2;
                        }
                        else if (cmd.VolumeTypeSpecifier2 == VolumeType.Absolute)
                        {
                            cmd.AbsoluteVolume_uL2 = panel.AbsoluteVolume_uL2;
                        }
                    }
                    break;
                case SubCommand.ResusMixSepTransCommand:
                    {
                        currentPanel = myResusMixSepTransCommandPanel;
                        ProtocolResusMixSepTransCommand cmd =
                            (ProtocolResusMixSepTransCommand)myCommandSequence[selectedIndex];
                        GenericMultiStepCommandPanel panel = myResusMixSepTransCommandPanel;
                        cmd.SourceVial = panel.SourceVial;
                        cmd.SourceVial2 = panel.SourceVial2;
                        cmd.DestinationVial = panel.DestinationVial;
                        cmd.DestinationVial2 = panel.DestinationVial2;
                        cmd.TipRack = panel.TipRack;
                        cmd.TipRackSpecified = panel.TipRackSpecified;
                        cmd.VolumeTypeSpecifier = panel.VolumeTypeSpecifier;
                        if (cmd.VolumeTypeSpecifier == VolumeType.Relative)
                        {
                            cmd.RelativeVolumeProportion = panel.RelativeVolumeProportion;
                        }
                        else if (cmd.VolumeTypeSpecifier == VolumeType.Absolute)
                        {
                            cmd.AbsoluteVolume_uL = panel.AbsoluteVolume_uL;
                        }
                        cmd.VolumeTypeSpecifier2 = panel.VolumeTypeSpecifierMix;
                        if (cmd.VolumeTypeSpecifier2 == VolumeType.Relative)
                        {
                            cmd.RelativeVolumeProportion2 = panel.RelativeVolumeProportionMix;
                        }
                        else if (cmd.VolumeTypeSpecifier2 == VolumeType.Absolute)
                        {
                            cmd.AbsoluteVolume_uL2 = panel.AbsoluteVolume_uLMix;
                        }
                        cmd.VolumeTypeSpecifier3 = panel.VolumeTypeSpecifier2;
                        if (cmd.VolumeTypeSpecifier3 == VolumeType.Relative)
                        {
                            cmd.RelativeVolumeProportion3 = panel.RelativeVolumeProportion2;
                        }
                        else if (cmd.VolumeTypeSpecifier3 == VolumeType.Absolute)
                        {
                            cmd.AbsoluteVolume_uL3 = panel.AbsoluteVolume_uL2;
                        }

                        cmd.MixCycles = panel.MixCycles;
                        cmd.TipTubeBottomGap_uL = panel.TipTubeBottomGap;
                        cmd.WaitCommandTimeDuration = panel.WaitCommandTimeDuration;
                    }
                    break;
                case SubCommand.ResusMixCommand:
                    {
                        currentPanel = myResusMixCommandPanel;
                        ProtocolResusMixCommand cmd =
                            (ProtocolResusMixCommand)myCommandSequence[selectedIndex];
                        GenericMultiStepCommandPanel panel = myResusMixCommandPanel;
                        cmd.SourceVial = panel.SourceVial;
                        cmd.DestinationVial = panel.DestinationVial;
                        cmd.TipRack = panel.TipRack;
                        cmd.TipRackSpecified = panel.TipRackSpecified;
                        cmd.VolumeTypeSpecifier = panel.VolumeTypeSpecifier;
                        if (cmd.VolumeTypeSpecifier == VolumeType.Relative)
                        {
                            cmd.RelativeVolumeProportion = panel.RelativeVolumeProportion;
                        }
                        else if (cmd.VolumeTypeSpecifier == VolumeType.Absolute)
                        {
                            cmd.AbsoluteVolume_uL = panel.AbsoluteVolume_uL;
                        }
                        cmd.VolumeTypeSpecifier2 = panel.VolumeTypeSpecifierMix;
                        if (cmd.VolumeTypeSpecifier2 == VolumeType.Relative)
                        {
                            cmd.RelativeVolumeProportion2 = panel.RelativeVolumeProportionMix;
                        }
                        else if (cmd.VolumeTypeSpecifier2 == VolumeType.Absolute)
                        {
                            cmd.AbsoluteVolume_uL2 = panel.AbsoluteVolume_uLMix;
                        }

                        cmd.MixCycles = panel.MixCycles;
                        cmd.TipTubeBottomGap_uL = panel.TipTubeBottomGap;
                    }
                    break;
                case SubCommand.MixTransCommand:
                    {
                        currentPanel = myMixTransCommandPanel;

                        ProtocolMixTransCommand cmd =
                            (ProtocolMixTransCommand)myCommandSequence[selectedIndex];
                        GenericMultiStepCommandPanel2 panel = myMixTransCommandPanel;
                        cmd.SourceVial = panel.SourceVial;
                        cmd.SourceVial2 = panel.SourceVial2;
                        cmd.DestinationVial = panel.DestinationVial;
                        cmd.DestinationVial2 = panel.DestinationVial2;
                        cmd.TipRack = panel.TipRack;
                        cmd.TipRackSpecified = panel.TipRackSpecified;
                        cmd.VolumeTypeSpecifier = panel.VolumeTypeSpecifier;

                        if (cmd.VolumeTypeSpecifier == VolumeType.Relative)
                        {
                            cmd.RelativeVolumeProportion = panel.RelativeVolumeProportion;
                        }
                        else if (cmd.VolumeTypeSpecifier == VolumeType.Absolute)
                        {
                            cmd.AbsoluteVolume_uL = panel.AbsoluteVolume_uL;
                        }
                        cmd.VolumeTypeSpecifier2 = panel.VolumeTypeSpecifier2;
                        if (cmd.VolumeTypeSpecifier2 == VolumeType.Relative)
                        {
                            cmd.RelativeVolumeProportion2 = panel.RelativeVolumeProportion2;
                        }
                        else if (cmd.VolumeTypeSpecifier2 == VolumeType.Absolute)
                        {
                            cmd.AbsoluteVolume_uL2 = panel.AbsoluteVolume_uL2;
                        }

                        cmd.MixCycles = panel.MixCycles;
                        cmd.TipTubeBottomGap_uL = panel.TipTubeBottomGap;
                    }
                    break;
			
						
			}

			// Store common panel data
			myCommandSequence[selectedIndex].CommandLabel = currentPanel.CommandLabel;
			myCommandSequence[selectedIndex].CommandExtensionTime = currentPanel.CommandExtensionTime;
        
		}

		private CommandPanel.PanelType CommandPanelTypeForCommandSubtype(int selectedIndex)
		{
            if (selectedIndex < 0) selectedIndex =0;
			CommandPanel.PanelType panelType = CommandPanel.PanelType.START_TYPE;
			switch ((SubCommand)myCommandSequence[selectedIndex].CommandSubtype)
			{
				default:
				case SubCommand.HomeAllCommand:
					panelType = CommandPanel.PanelType.CommandPanel;		
					break;

				case SubCommand.DemoCommand:
					panelType = CommandPanel.PanelType.DemoCommandPanel;
					break;

				case SubCommand.PumpLifeCommand:
					panelType = CommandPanel.PanelType.DemoCommandPanel;
					break;

				case SubCommand.IncubateCommand:
					panelType = CommandPanel.PanelType.IncubateCommandPanel;
					break;

				case SubCommand.SeparateCommand:
					panelType = CommandPanel.PanelType.SeparateCommandPanel;
					break;

				case SubCommand.MixCommand:
					panelType = CommandPanel.PanelType.VolumeCommandPanel;
					break;

				case SubCommand.TransportCommand:
					panelType = CommandPanel.PanelType.WorkingVolumeCommandPanel;
					break;	
								
				case SubCommand.TopUpVialCommand:
					panelType = CommandPanel.PanelType.WorkingVolumeCommandPanel;
					break;

				case SubCommand.ResuspendVialCommand:
					panelType = CommandPanel.PanelType.WorkingVolumeCommandPanel;
					break;

				case SubCommand.FlushCommand:
					panelType = CommandPanel.PanelType.VolumeMaintenanceCommandPanel;
					break;

				case SubCommand.PrimeCommand:
					panelType = CommandPanel.PanelType.VolumeMaintenanceCommandPanel;
					break;

                case SubCommand.PauseCommand:
                    panelType = CommandPanel.PanelType.PauseCommandPanel;
                    break;
                case SubCommand.TopUpMixTransSepTransCommand:
                    panelType = CommandPanel.PanelType.TopUpMixTransSepTransCommandPanel;
                    break;
                case SubCommand.TopUpMixTransCommand:
                    panelType = CommandPanel.PanelType.TopUpMixTransCommandPanel;
                    break;
                case SubCommand.TopUpTransSepTransCommand:
                    panelType = CommandPanel.PanelType.TopUpTransSepTransCommandPanel;
                    break;
                case SubCommand.TopUpTransCommand:
                    panelType = CommandPanel.PanelType.TopUpTransCommandPanel;
                    break;
                case SubCommand.ResusMixSepTransCommand:
                    panelType = CommandPanel.PanelType.ResusMixSepTransCommandPanel;
                    break;
                case SubCommand.ResusMixCommand:
                    panelType = CommandPanel.PanelType.ResusMixCommandPanel;
                    break;
                case SubCommand.MixTransCommand:
                    panelType = CommandPanel.PanelType.MixTransCommandPanel;
                    break;
			}
			return panelType;
		}

		#endregion Data access

		#region Data Validation

		private void btnValidateProtocol_Click(object sender, System.EventArgs e)
		{				
			// Save changes to the current command if they haven't already been saved.
			if (isCurrentCommandDirty || isAddingInProgress)
			{
				bool isCurrentCommandValid = ValidateCurrentCommand();
                if (isCurrentCommandValid)
                {
                    // Since we're about to move places in the command list,
                    // persist changes from the current panel back to the sequence.
                    PersistCommandDetailView();
                    isCurrentCommandDirty = false;
                    // Re-enable Add/MoveUp/MoveDown now the current command is validated.
                    SetAddingMode(false);
                }
                else
                {
                    return;
                }
			}
			//4.5.X - 2.4.3
			//Disable working vol if there is atleast 1 topup/resuspend and
			// they are all relative
			CheckCmdSeqForWorkingVolumeUse();
			
			/*CR - moved quad count to here*/
			// Adjust quadrant usage, if required, to ensure the definition uses contiguous
			// quadrants.
			myQuadrantCount = AdjustAndCountUsedQuadrants();

			// Validate user changes - was: && CheckAllCommands() && ((ProtocolClass)(cmbProtocolType.SelectedIndex)!=ProtocolClass.Undefined);            
			bool isProtocolValid = ValidateProtocol();

			theProtocolModel.ValidProtocol = isProtocolValid;

			// previous
			// if (isProtocolValid && !txtProtocolDesc.Enabled)
			//     UpdateProtocolDescription();
			if (isProtocolValid && !theProtocolModel.ProtocolDescManualFill) //txtProtocolDesc.Enabled)
			{
				UpdateProtocolDescription();				
			}

			if (isProtocolValid == true)
			{
                // 2011-09-12 sp
                // rename from CheckProtocolVolumes to more generic name
                isProtocolValid = CheckProtocolCommands( false );//CWJ ADD
			}
			else
			{
                // 2011-09-12 sp
                // rename from CheckProtocolVolumes to more generic name
                CheckProtocolCommands( false );
			}

			if ((ProtocolClass)(cmbProtocolType.SelectedIndex) == ProtocolClass.Undefined)
				MessageBox.Show(this,"Please define the Protocol Type.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			
			// set wingdings tick or cross
			lblValidateResult.Text = isProtocolValid ? "þ" : "ý";

			// if validation successful, reset the 'page dirty' flag
			if (( ! isFormDirty) || isProtocolValid)
			{
				ExitEditMode();		
			}

			if (isProtocolValid)
			{
				SetAddingMode(false);
			}
		}

        // 2011-11-03 sp -- added to provide status of volumes
        private void btnProtocolSummary_Click(object sender, System.EventArgs e)
        {
            bool isProtocolValid = ValidateProtocol() & CheckProtocolCommands(true);

            // set wingdings tick or cross
            lblValidateResult.Text = isProtocolValid ? "þ" : "ý";

            if ((!isFormDirty) || isProtocolValid)
            {
                ExitEditMode();
            }

            if (isProtocolValid)
            {
                SetAddingMode(false);
            }
        }

        public static bool isInQuadrant1(AbsoluteResourceLocation vial)
        {
            return (vial >= AbsoluteResourceLocation.TPC0101 && vial < AbsoluteResourceLocation.TPC0201);
        }
        public static bool isInQuadrant2(AbsoluteResourceLocation vial)
        {
            return (vial >= AbsoluteResourceLocation.TPC0201 && vial < AbsoluteResourceLocation.TPC0301);
        }
        public static bool isInQuadrant3(AbsoluteResourceLocation vial)
        {
            return (vial >= AbsoluteResourceLocation.TPC0301 && vial < AbsoluteResourceLocation.TPC0401);
        }
        public static bool isInQuadrant4(AbsoluteResourceLocation vial)
        {
            return (vial >= AbsoluteResourceLocation.TPC0401 && vial < AbsoluteResourceLocation.NUM_LOCATIONS);
        }
        public static bool isVialInQuadrant(QuadrantId currentQuadrant, AbsoluteResourceLocation vial)
        {
            bool boResult = false;
            switch (currentQuadrant)
            {
                case QuadrantId.Quadrant1:
                    boResult = isInQuadrant1(vial);
                    break;
                case QuadrantId.Quadrant2:
                    boResult = isInQuadrant2(vial);
                    break;
                case QuadrantId.Quadrant3:
                    boResult = isInQuadrant3(vial);
                    break;
                case QuadrantId.Quadrant4:
                    boResult = isInQuadrant4(vial);
                    break;
            }
            return boResult;
        }
        public static bool isVialInGreaterOrEqualToQuadrant(QuadrantId currentQuadrant, AbsoluteResourceLocation vial)
        {
            bool boResult = false;
            switch (currentQuadrant)
            {
                case QuadrantId.Quadrant1:
                    boResult = isInQuadrant1(vial) || isInQuadrant2(vial) || isInQuadrant3(vial) || isInQuadrant4(vial);
                    break;
                case QuadrantId.Quadrant2:
                    boResult = isInQuadrant2(vial) || isInQuadrant3(vial) || isInQuadrant4(vial);
                    break;
                case QuadrantId.Quadrant3:
                    boResult = isInQuadrant3(vial) || isInQuadrant4(vial);
                    break;
                case QuadrantId.Quadrant4:
                    boResult = isInQuadrant4(vial);
                    break;
            }
            return boResult;
        }
        private int quadrantIDToUsage(QuadrantId currentQuadrant, int usageQ1, int usageQ2, int usageQ3, int usageQ4)
        {
            int quadrantUsage = 0;
            switch (currentQuadrant)
            {
                case QuadrantId.Quadrant1:
                    quadrantUsage = usageQ1;
                    break;
                case QuadrantId.Quadrant2:
                    quadrantUsage = usageQ2;
                    break;
                case QuadrantId.Quadrant3:
                    quadrantUsage = usageQ3;
                    break;
                case QuadrantId.Quadrant4:
                    quadrantUsage = usageQ4;
                    break;
            }
            return quadrantUsage;
        }

        public void quadrantUsageCount(out int usageQ1, out int usageQ2, out int usageQ3, out int usageQ4)
        {
            usageQ1 = usageQ2 = usageQ3 = usageQ4 = 0; 
            IEnumerator iter = myCommandSequence == null ? null : myCommandSequence.GetEnumerator();
            while (iter != null && iter.MoveNext())
            {

                // check to see if the source, destination or tiprack are in differing quadrants...
                IVolumeCommand volumeCommand = iter.Current as IVolumeCommand;
                if (volumeCommand != null)
                {
                    AbsoluteResourceLocation srcVial = volumeCommand.SourceVial;
                    AbsoluteResourceLocation destVial = volumeCommand.DestinationVial;

                    //note: tipRack check was removed
                    usageQ1 += (isInQuadrant1(srcVial) || isInQuadrant1(destVial)) ? 1 : 0;
                    usageQ2 += (isInQuadrant2(srcVial) || isInQuadrant2(destVial)) ? 1 : 0;
                    usageQ3 += (isInQuadrant3(srcVial) || isInQuadrant3(destVial)) ? 1 : 0;
                    usageQ4 += (isInQuadrant4(srcVial) || isInQuadrant4(destVial)) ? 1 : 0;
                }

                IMultiSrcDestCommand multiSrcDestCmd = iter.Current as IMultiSrcDestCommand;
                if (multiSrcDestCmd != null)
                {
                    AbsoluteResourceLocation srcVial = multiSrcDestCmd.SourceVial;
                    AbsoluteResourceLocation destVial = multiSrcDestCmd.DestinationVial;
                    AbsoluteResourceLocation srcVial2 = multiSrcDestCmd.SourceVial2;
                    AbsoluteResourceLocation destVial2 = multiSrcDestCmd.DestinationVial2;
                    AbsoluteResourceLocation srcVial3 = multiSrcDestCmd.SourceVial3;
                    AbsoluteResourceLocation destVial3 = multiSrcDestCmd.DestinationVial3;

                    //note: tipRack check was removed
                    usageQ1 += (isInQuadrant1(srcVial) || isInQuadrant1(destVial) ||
                                isInQuadrant1(srcVial2) || isInQuadrant1(destVial2) ||
                                isInQuadrant1(srcVial3) || isInQuadrant1(destVial3)) ? 1 : 0;
                    usageQ2 += (isInQuadrant2(srcVial) || isInQuadrant2(destVial) ||
                                isInQuadrant2(srcVial2) || isInQuadrant2(destVial2) ||
                                isInQuadrant2(srcVial3) || isInQuadrant2(destVial3)) ? 1 : 0;
                    usageQ3 += (isInQuadrant3(srcVial) || isInQuadrant3(destVial) ||
                                isInQuadrant3(srcVial2) || isInQuadrant3(destVial2) ||
                                isInQuadrant3(srcVial3) || isInQuadrant3(destVial3)) ? 1 : 0;
                    usageQ4 += (isInQuadrant4(srcVial) || isInQuadrant4(destVial) ||
                                isInQuadrant4(srcVial2) || isInQuadrant4(destVial2) ||
                                isInQuadrant4(srcVial3) || isInQuadrant4(destVial3)) ? 1 : 0;
                }
            }
        }
        private AbsoluteResourceLocation shiftVialIfVialInGreaterOrEqualToQuadrant(AbsoluteResourceLocation vial, QuadrantId nextQuadrant, QuadrantId currentQuadrant)
        {
            // Translate the location if applicable else return original
            if (isVialInGreaterOrEqualToQuadrant(nextQuadrant, vial))
            {
                return vial - ((int)nextQuadrant - (int)currentQuadrant) *
                                        (int)(RelativeQuadrantLocation.NUM_RELATIVE_QUADRANT_LOCATIONS - 2);
            }
            return vial;
        }
        private void shiftVialsInCommands(QuadrantId nextQuadrant, QuadrantId currentQuadrant)
        {
            for (int i = 0; i < myCommandSequence.GetLength(0); ++i)
            {
                IVolumeCommand volumeCommand = myCommandSequence[i] as IVolumeCommand;
                if (volumeCommand != null)
                {
                    volumeCommand.SourceVial = shiftVialIfVialInGreaterOrEqualToQuadrant(volumeCommand.SourceVial, nextQuadrant, currentQuadrant);
                    volumeCommand.DestinationVial = shiftVialIfVialInGreaterOrEqualToQuadrant(volumeCommand.DestinationVial, nextQuadrant, currentQuadrant);
                }

                IMultiSrcDestCommand multiSrcDestCmd = myCommandSequence[i] as IMultiSrcDestCommand;
                if (multiSrcDestCmd != null)
                {
                    multiSrcDestCmd.SourceVial = shiftVialIfVialInGreaterOrEqualToQuadrant(multiSrcDestCmd.SourceVial, nextQuadrant, currentQuadrant);
                    multiSrcDestCmd.DestinationVial = shiftVialIfVialInGreaterOrEqualToQuadrant(multiSrcDestCmd.DestinationVial, nextQuadrant, currentQuadrant);
                    multiSrcDestCmd.SourceVial2 = shiftVialIfVialInGreaterOrEqualToQuadrant(multiSrcDestCmd.SourceVial2, nextQuadrant, currentQuadrant);
                    multiSrcDestCmd.DestinationVial2 = shiftVialIfVialInGreaterOrEqualToQuadrant(multiSrcDestCmd.DestinationVial2, nextQuadrant, currentQuadrant);
                    multiSrcDestCmd.SourceVial3 = shiftVialIfVialInGreaterOrEqualToQuadrant(multiSrcDestCmd.SourceVial3, nextQuadrant, currentQuadrant);
                    multiSrcDestCmd.DestinationVial3 = shiftVialIfVialInGreaterOrEqualToQuadrant(multiSrcDestCmd.DestinationVial3, nextQuadrant, currentQuadrant);
                
                }
            }
        }

        

		// check the quadrant usage implied in the commands that are specified in the protocol being designed
		// and increase the quadrant count accordingly - tip racks outside the main quadrant are also considered
        
		public int AdjustAndCountUsedQuadrants()
		{
			// Adjust the specified quadrant resource locations to ensure that quadrant
			// usage is contiguous.

			// Count the number of quadrant resources specified for each quadrant.
			int usageQ1, usageQ2, usageQ3, usageQ4;
            quadrantUsageCount(out usageQ1, out usageQ2, out usageQ3, out usageQ4);
			

			// If necessary, adjust the quadrant resources to remove any "gap" in quadrant usage,
			// to ensure that the definition uses contiguous quadrants.
			for (QuadrantId currentQuadrant = QuadrantId.Quadrant1;
				currentQuadrant < QuadrantId.NUM_QUADRANTS && myCommandSequence != null;
				++currentQuadrant)
			{
                if (quadrantIDToUsage(currentQuadrant, usageQ1, usageQ2, usageQ3, usageQ4) > 0)
				{
					continue;
				}
				else // The current quadrant is not used, so shuffle down other quadrant resources
				{
					for (QuadrantId nextQuadrant = currentQuadrant+1;nextQuadrant < QuadrantId.NUM_QUADRANTS;++nextQuadrant)
					{
						int quadrantUsage = quadrantIDToUsage(nextQuadrant, usageQ1, usageQ2, usageQ3, usageQ4);
						if (quadrantUsage > 0)
						{
                            shiftVialsInCommands(nextQuadrant, currentQuadrant);

                            quadrantUsageCount(out usageQ1, out usageQ2, out usageQ3, out usageQ4); //recount after shift
                            break;
						}
					}
				}
			}

			// Return the quadrant usage count
			int quadrantCount = 0;
			quadrantCount = usageQ1 > 0 ? quadrantCount+1 : quadrantCount;
			quadrantCount = usageQ2 > 0 ? quadrantCount+1 : quadrantCount;
			quadrantCount = usageQ3 > 0 ? quadrantCount+1 : quadrantCount;
			quadrantCount = usageQ4 > 0 ? quadrantCount+1 : quadrantCount;

			return quadrantCount;
		}

        /*
        private static void AddAbsoluteResourceLocationToList(List<AbsoluteResourceLocation> result, AbsoluteResourceLocation sLoc)
        {
            //try
            {
                //sLoc = (AbsoluteResourceLocation)Enum.Parse(typeof(AbsoluteResourceLocation), srcVial);
                if (!result.Contains(sLoc))
                {
                    result.Add(sLoc);
                }
            }
            //catch (Exception) { }
            return;
        }
        internal void UpdateForReagentCustomVialUse()
        {
            bool[] quadrantUsed = new bool[]{false,false,false,false};
            List<AbsoluteResourceLocation> lstUsed = theProtocolModel.GetListOfReagentVialsUsedInProtocol();
            for (int i = 0; i < lstUsed.Count; i++)
            {
                switch (lstUsed[i])
                {
                    case AbsoluteResourceLocation.TPC0103:
                    case AbsoluteResourceLocation.TPC0105:
                    case AbsoluteResourceLocation.TPC0104:
                        quadrantUsed[0] = true;
                        break;

                    case AbsoluteResourceLocation.TPC0203:
                    case AbsoluteResourceLocation.TPC0205:
                    case AbsoluteResourceLocation.TPC0204:
                        quadrantUsed[1] = true;
                        break;

                    case AbsoluteResourceLocation.TPC0303:
                    case AbsoluteResourceLocation.TPC0305:
                    case AbsoluteResourceLocation.TPC0304:
                        quadrantUsed[2] = true;
                        break;

                    case AbsoluteResourceLocation.TPC0403:
                    case AbsoluteResourceLocation.TPC0405:
                    case AbsoluteResourceLocation.TPC0404:
                        quadrantUsed[3] = true;
                        break;

                }
            }

            theProtocolModel.UpdateVialBarcodeQuadrant(quadrantUsed);
        }
        
        */
        /*
        private List<AbsoluteResourceLocation> GetListOfReagentVialsUsedInProtocol()
        {
            List<AbsoluteResourceLocation> result = new List<AbsoluteResourceLocation>();
            for (int i = 0; i < myCommandSequence.GetLength(0); ++i)
            {
                IVolumeCommand volumeCommand = myCommandSequence[i] as IVolumeCommand;
                if (volumeCommand != null)
                {
                    AddAbsoluteResourceLocationToList(result, volumeCommand.SourceVial);
                    AddAbsoluteResourceLocationToList(result, volumeCommand.DestinationVial);
                }

                IMultiSrcDestCommand multiSrcDestCmd = myCommandSequence[i] as IMultiSrcDestCommand;
                if (multiSrcDestCmd != null)
                {
                    AddAbsoluteResourceLocationToList(result, multiSrcDestCmd.SourceVial);
                    AddAbsoluteResourceLocationToList(result, multiSrcDestCmd.DestinationVial);
                    AddAbsoluteResourceLocationToList(result, multiSrcDestCmd.SourceVial2);
                    AddAbsoluteResourceLocationToList(result, multiSrcDestCmd.DestinationVial2);
                    AddAbsoluteResourceLocationToList(result, multiSrcDestCmd.SourceVial3);
                    AddAbsoluteResourceLocationToList(result, multiSrcDestCmd.DestinationVial3);
                }
            }
            return result;
        }
        */

		private bool ValidateProtocol()
		{
			// Validate the header information and the commands in the command
			// sequence.  Before doing the latter, we must check the current command
			// details as they may have changed.
			bool result = ValidateHeaderAndConstraints() && ValidateCurrentCommand() && 
				ValidateCommandSequence() && ValidateCurrentTipRackQuadrant() &&
				CheckAllCommands() && 
				((ProtocolClass)(cmbProtocolType.SelectedIndex)!=ProtocolClass.Undefined); //RL

			return result;
		
		}

		      	private bool ValidateCurrentTipRackQuadrant()
				{
					bool valid = true;

					if (myCommandSequence != null && myCommandSequence.GetLength(0) > 0)
					{
                        VolumeCommandPanel p = null;
						switch (myCommandSequence[myCurrentCommandIndex].CommandSubtype)
						{
							case SubCommand.MixCommand:
                                p = myVolumeCommandPanel;
								break;
							case SubCommand.TransportCommand:
							case SubCommand.TopUpVialCommand:
							case SubCommand.ResuspendVialCommand:
                                p = myWorkingVolumeCommandPanel;
								break;
                            case SubCommand.FlushCommand:
                            case SubCommand.PrimeCommand:
                                p = myVolumeMaintenanceCommandPanel;
                                break;
                            case SubCommand.TopUpMixTransSepTransCommand:
                                p = myTopUpMixTransSepTransCommandPanel;
                                break;
                            case SubCommand.TopUpMixTransCommand:
                                p = myTopUpMixTransCommandPanel;
                                break;
                            case SubCommand.TopUpTransSepTransCommand:
                                p = myTopUpTransSepTransCommandPanel;
                                break;
                            case SubCommand.TopUpTransCommand:
                                p = myTopUpTransCommandPanel;
                                break;
                            case SubCommand.ResusMixSepTransCommand:
                                p = myResusMixSepTransCommandPanel;
                                break;
                            case SubCommand.ResusMixCommand:
                                p = myResusMixCommandPanel;
                                break;
                            case SubCommand.MixTransCommand:
                                p = myMixTransCommandPanel;
                                break;
						}
                        if (p != null && p.TipRackSpecified)
                        {
                            if (p.TipRack > myQuadrantCount)
                            {
                                valid = false;
                                p.TipRack = myQuadrantCount;
                            }
                        }
					}
					return valid;
				}

		private bool ValidateHeaderAndConstraints()
		{
			bool isContentValid = true;
			// Check if the protocol label and author have been provided (mandatory)
			isContentValid &= txtProtocolLabel.TextLength > 0;

			// do not check volumes for maintenance protocols ... bdr
			if (IsMaintenanceProtocol == true) 
//			if (theProtocolModel.ProtocolClass != ProtocolClass.Maintenance || 
//				theProtocolModel.ProtocolClass != ProtocolClass.Shutdown)
				return isContentValid;
			
			// Check if the sample volume minimum & maximum have been provided (mandatory)
			isContentValid &= txtSampleVolumeMin.TextLength > 0 &&
				txtSampleVolumeMax.TextLength > 0				&&
				mySampleVolumeMin_uL >= 1						&&		//     changed from 0 to 1
				mySampleVolumeMax_uL >= 1						&&		//     changed from 0 to 1
				mySampleVolumeMax_uL >= mySampleVolumeMin_uL;

			// Check if the working volume threshold and working volume low/high values
			// have been provided (optional)
			if (txtWorkingVolumeSampleThreshold.Enabled &&
				txtWorkingVolumeSampleThreshold.TextLength > 0)
			{				
				isContentValid &= myWorkingVolumeSampleThreshold_uL >= 1		&&
					txtWorkingVolumeLow.TextLength > 0							&&
					txtWorkingVolumeHigh.TextLength > 0							&&
					// Check the sample threshold is in the sample volume range
					myWorkingVolumeSampleThreshold_uL >= mySampleVolumeMin_uL	&&
					myWorkingVolumeSampleThreshold_uL <= mySampleVolumeMax_uL	&&
					// Check the working volume 'low' and 'high' hold true with 
					// respect to each other.
					( ! IsWorkingVolumeLowError())								&&
					( ! IsWorkingVolumeHighError());
			}
			
			return isContentValid;
		}

		private bool ValidateCurrentCommand()
		{
			bool isContentValid = false;
            if (myCommandSequence != null && myCommandSequence.GetLength(0) > 0 && myCurrentCommandIndex !=-1)
			{

				CommandPanel.PanelType panelType = 
					CommandPanelTypeForCommandSubtype(myCurrentCommandIndex);
				isContentValid = myCommandPanels[(int)panelType].IsContentValid();
			}
			return isContentValid;
		}

		private bool ValidateCommandSequence()
		{
			// Currently there are no dependencies between commands.  Individual command
			// validation is done by the validation code in each individual command panel.
			// So, currently, if each individual commmand is valid, then the sequence is valid.
			return myCommandSequence != null && myCommandSequence.GetLength(0) > 0;
		}

		#endregion Data Validation

		#region Navigation

		private void btnChange_Click(object sender, System.EventArgs e)
		{
			// Return "change page" result code
			((FrmProtocolEditor)MdiParent).ChangePage(DialogResult.OK);
		}

		#endregion Navigation

        // 2011-09-08 to 2011-09-16 sp various changes
        //     - provide support for use in smaller screen displays (support for scrollbar in other files)
        //     - align and resize panels for more unify displays  
        #region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.pnlCommandList = new System.Windows.Forms.Panel();
            this.splHeader = new System.Windows.Forms.Splitter();
            this.btnCopy = new System.Windows.Forms.Button();
            this.splCommands = new System.Windows.Forms.Splitter();
            this.cmbCommandTypes = new System.Windows.Forms.ComboBox();
            this.btnDeleteCommand = new System.Windows.Forms.Button();
            this.btnAddCommand = new System.Windows.Forms.Button();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.btnMoveUp = new System.Windows.Forms.Button();
            this.lblCommandList = new System.Windows.Forms.Label();
            this.lstCommands = new System.Windows.Forms.ListBox();
            this.btnShow = new System.Windows.Forms.Button();
            this.pnlHeaderInfo = new System.Windows.Forms.Panel();
            this.btnLabelInfo = new System.Windows.Forms.Button();
            this.lblRobosepType = new System.Windows.Forms.Label();
            this.btnProtocolSummary = new System.Windows.Forms.Button();
            this.btnAutoFill = new System.Windows.Forms.Button();
            this.lblProtocolDesc = new System.Windows.Forms.Label();
            this.txtProtocolDesc = new Tesla.ProtocolEditorControls.ProtocolLabelDecriptionAndAuthorTextbox();
            this.lblLowVolume = new System.Windows.Forms.Label();
            this.txtWorkingVolumeHigh = new System.Windows.Forms.TextBox();
            this.txtWorkingVolumeLow = new System.Windows.Forms.TextBox();
            this.lblHighVolume = new System.Windows.Forms.Label();
            this.txtWorkingVolumeSampleThreshold = new System.Windows.Forms.TextBox();
            this.lblWorkingVolumeSampleThreshold = new System.Windows.Forms.Label();
            this.txtSampleVolumeMax = new System.Windows.Forms.TextBox();
            this.txtSampleVolumeMin = new System.Windows.Forms.TextBox();
            this.lblSampleVolumeMax = new System.Windows.Forms.Label();
            this.lblSampleVolumeMin = new System.Windows.Forms.Label();
            this.lblSampleVolume = new System.Windows.Forms.Label();
            this.lblValidateResult = new System.Windows.Forms.Label();
            this.btnValidateProtocol = new System.Windows.Forms.Button();
            this.lblProtocolType = new System.Windows.Forms.Label();
            this.cmbProtocolType = new System.Windows.Forms.ComboBox();
            this.lblHeader = new System.Windows.Forms.Label();
            this.txtProtocolLabel = new Tesla.ProtocolEditorControls.ProtocolLabelDecriptionAndAuthorTextbox();
            this.lblProtocolLabel = new System.Windows.Forms.Label();
            this.btnHide = new System.Windows.Forms.Button();
            this.pnlCommandDetail = new System.Windows.Forms.Panel();
            this.myMixTransCommandPanel = new Tesla.ProtocolEditorControls.GenericMultiStepCommandPanel2();
            this.myResusMixSepTransCommandPanel = new Tesla.ProtocolEditorControls.GenericMultiStepCommandPanel();
            this.myResusMixCommandPanel = new Tesla.ProtocolEditorControls.GenericMultiStepCommandPanel();
            this.myTopUpTransCommandPanel = new Tesla.ProtocolEditorControls.GenericMultiStepCommandPanel();
            this.myTopUpTransSepTransCommandPanel = new Tesla.ProtocolEditorControls.GenericMultiStepCommandPanel();
            this.myTopUpMixTransCommandPanel = new Tesla.ProtocolEditorControls.GenericMultiStepCommandPanel();
            this.btnConfirmCommand = new System.Windows.Forms.Button();
            this.lblCommandDetails = new System.Windows.Forms.Label();
            this.myIncubateCommandPanel = new Tesla.ProtocolEditorControls.IncubateCommandPanel();
            this.myPauseCommandPanel = new Tesla.ProtocolEditorControls.PauseCommandPanel();
            this.mySeparateCommandPanel = new Tesla.ProtocolEditorControls.SeparateCommandPanel();
            this.myDemoCommandPanel = new Tesla.ProtocolEditorControls.DemoCommandPanel();
            this.myCommandPanel = new Tesla.ProtocolEditorControls.CommandPanel();
            this.errorProtocolLabel = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorProtocolAuthor = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorWorkingVolumeThreshold = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorSampleVolumeMin = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorSampleVolumeMax = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorCommandSequence = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorWorkingVolumeLow = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorWorkingVolumeHigh = new System.Windows.Forms.ErrorProvider(this.components);
            this.tipCheckProtocol = new System.Windows.Forms.ToolTip(this.components);
            this.tipName = new System.Windows.Forms.ToolTip(this.components);
            this.tipDescription = new System.Windows.Forms.ToolTip(this.components);
            this.tipMinimumSample = new System.Windows.Forms.ToolTip(this.components);
            this.tipMaximumSample = new System.Windows.Forms.ToolTip(this.components);
            this.tipWorking = new System.Windows.Forms.ToolTip(this.components);
            this.tipWorkingLow = new System.Windows.Forms.ToolTip(this.components);
            this.tipWorkingHigh = new System.Windows.Forms.ToolTip(this.components);
            this.tipConfirmCommand = new System.Windows.Forms.ToolTip(this.components);
            this.myWorkingVolumeCommandPanel = new Tesla.ProtocolEditorControls.WorkingVolumeCommandPanel();
            this.myVolumeMaintenanceCommandPanel = new Tesla.ProtocolEditorControls.VolumeMaintenanceCommandPanel();
            this.myVolumeCommandPanel = new Tesla.ProtocolEditorControls.MixCommandPanel();
            this.myTopUpMixTransSepTransCommandPanel = new Tesla.ProtocolEditorControls.GenericMultiStepCommandPanel();
            this.pnlCommandList.SuspendLayout();
            this.pnlHeaderInfo.SuspendLayout();
            this.pnlCommandDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProtocolLabel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProtocolAuthor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorWorkingVolumeThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorSampleVolumeMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorSampleVolumeMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorCommandSequence)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorWorkingVolumeLow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorWorkingVolumeHigh)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlCommandList
            // 
            this.pnlCommandList.AutoSize = true;
            this.pnlCommandList.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlCommandList.BackColor = System.Drawing.SystemColors.Control;
            this.pnlCommandList.Controls.Add(this.splHeader);
            this.pnlCommandList.Controls.Add(this.btnCopy);
            this.pnlCommandList.Controls.Add(this.splCommands);
            this.pnlCommandList.Controls.Add(this.cmbCommandTypes);
            this.pnlCommandList.Controls.Add(this.btnDeleteCommand);
            this.pnlCommandList.Controls.Add(this.btnAddCommand);
            this.pnlCommandList.Controls.Add(this.btnMoveDown);
            this.pnlCommandList.Controls.Add(this.btnMoveUp);
            this.pnlCommandList.Controls.Add(this.lblCommandList);
            this.pnlCommandList.Controls.Add(this.lstCommands);
            this.pnlCommandList.Controls.Add(this.btnShow);
            this.pnlCommandList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCommandList.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlCommandList.Location = new System.Drawing.Point(0, 162);
            this.pnlCommandList.MinimumSize = new System.Drawing.Size(532, 162);
            this.pnlCommandList.Name = "pnlCommandList";
            this.pnlCommandList.Size = new System.Drawing.Size(532, 164);
            this.pnlCommandList.TabIndex = 7;
            this.pnlCommandList.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlCommandList_Paint);
            // 
            // splHeader
            // 
            this.splHeader.BackColor = System.Drawing.Color.Red;
            this.splHeader.Cursor = System.Windows.Forms.Cursors.Default;
            this.splHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.splHeader.Location = new System.Drawing.Point(0, 0);
            this.splHeader.Margin = new System.Windows.Forms.Padding(0);
            this.splHeader.Name = "splHeader";
            this.splHeader.Size = new System.Drawing.Size(532, 2);
            this.splHeader.TabIndex = 30;
            this.splHeader.TabStop = false;
            // 
            // btnCopy
            // 
            this.btnCopy.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCopy.Location = new System.Drawing.Point(416, 30);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(50, 23);
            this.btnCopy.TabIndex = 29;
            this.btnCopy.Text = "Copy";
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // splCommands
            // 
            this.splCommands.BackColor = System.Drawing.Color.Red;
            this.splCommands.Cursor = System.Windows.Forms.Cursors.Default;
            this.splCommands.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splCommands.Location = new System.Drawing.Point(0, 162);
            this.splCommands.Margin = new System.Windows.Forms.Padding(0);
            this.splCommands.Name = "splCommands";
            this.splCommands.Size = new System.Drawing.Size(532, 2);
            this.splCommands.TabIndex = 7;
            this.splCommands.TabStop = false;
            // 
            // cmbCommandTypes
            // 
            this.cmbCommandTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCommandTypes.Location = new System.Drawing.Point(238, 30);
            this.cmbCommandTypes.Name = "cmbCommandTypes";
            this.cmbCommandTypes.Size = new System.Drawing.Size(172, 22);
            this.cmbCommandTypes.TabIndex = 1;
            // 
            // btnDeleteCommand
            // 
            this.btnDeleteCommand.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnDeleteCommand.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDeleteCommand.Location = new System.Drawing.Point(472, 30);
            this.btnDeleteCommand.Name = "btnDeleteCommand";
            this.btnDeleteCommand.Size = new System.Drawing.Size(50, 23);
            this.btnDeleteCommand.TabIndex = 4;
            this.btnDeleteCommand.Text = "Delete";
            this.btnDeleteCommand.Click += new System.EventHandler(this.btnDeleteCommand_Click);
            // 
            // btnAddCommand
            // 
            this.btnAddCommand.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddCommand.Location = new System.Drawing.Point(182, 30);
            this.btnAddCommand.Name = "btnAddCommand";
            this.btnAddCommand.Size = new System.Drawing.Size(50, 23);
            this.btnAddCommand.TabIndex = 3;
            this.btnAddCommand.Text = "Add";
            this.btnAddCommand.Click += new System.EventHandler(this.btnAddCommand_Click);
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Location = new System.Drawing.Point(90, 30);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(72, 23);
            this.btnMoveDown.TabIndex = 6;
            this.btnMoveDown.Text = "Move Down";
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.Location = new System.Drawing.Point(16, 30);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(72, 23);
            this.btnMoveUp.TabIndex = 5;
            this.btnMoveUp.Text = "Move Up";
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // lblCommandList
            // 
            this.lblCommandList.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCommandList.Location = new System.Drawing.Point(12, 4);
            this.lblCommandList.Name = "lblCommandList";
            this.lblCommandList.Size = new System.Drawing.Size(200, 23);
            this.lblCommandList.TabIndex = 0;
            this.lblCommandList.Text = "Command Sequence";
            // 
            // lstCommands
            // 
            this.lstCommands.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lstCommands.Cursor = System.Windows.Forms.Cursors.Default;
            this.lstCommands.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lstCommands.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstCommands.IntegralHeight = false;
            this.lstCommands.ItemHeight = 14;
            this.lstCommands.Location = new System.Drawing.Point(16, 58);
            this.lstCommands.Name = "lstCommands";
            this.lstCommands.Size = new System.Drawing.Size(506, 100);
            this.lstCommands.TabIndex = 2;
            this.lstCommands.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lstCommands_DrawItem);
            this.lstCommands.SelectedIndexChanged += new System.EventHandler(this.lstCommands_SelectedIndexChanged);
            // 
            // btnShow
            // 
            this.btnShow.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShow.Location = new System.Drawing.Point(436, 4);
            this.btnShow.Name = "btnShow";
            this.btnShow.Size = new System.Drawing.Size(90, 16);
            this.btnShow.TabIndex = 28;
            this.btnShow.Text = "Show Header";
            this.btnShow.Visible = false;
            this.btnShow.Click += new System.EventHandler(this.btnShow_Click);
            // 
            // pnlHeaderInfo
            // 
            this.pnlHeaderInfo.Controls.Add(this.btnLabelInfo);
            this.pnlHeaderInfo.Controls.Add(this.lblRobosepType);
            this.pnlHeaderInfo.Controls.Add(this.btnProtocolSummary);
            this.pnlHeaderInfo.Controls.Add(this.btnAutoFill);
            this.pnlHeaderInfo.Controls.Add(this.lblProtocolDesc);
            this.pnlHeaderInfo.Controls.Add(this.txtProtocolDesc);
            this.pnlHeaderInfo.Controls.Add(this.lblLowVolume);
            this.pnlHeaderInfo.Controls.Add(this.txtWorkingVolumeHigh);
            this.pnlHeaderInfo.Controls.Add(this.txtWorkingVolumeLow);
            this.pnlHeaderInfo.Controls.Add(this.lblHighVolume);
            this.pnlHeaderInfo.Controls.Add(this.txtWorkingVolumeSampleThreshold);
            this.pnlHeaderInfo.Controls.Add(this.lblWorkingVolumeSampleThreshold);
            this.pnlHeaderInfo.Controls.Add(this.txtSampleVolumeMax);
            this.pnlHeaderInfo.Controls.Add(this.txtSampleVolumeMin);
            this.pnlHeaderInfo.Controls.Add(this.lblSampleVolumeMax);
            this.pnlHeaderInfo.Controls.Add(this.lblSampleVolumeMin);
            this.pnlHeaderInfo.Controls.Add(this.lblSampleVolume);
            this.pnlHeaderInfo.Controls.Add(this.lblValidateResult);
            this.pnlHeaderInfo.Controls.Add(this.btnValidateProtocol);
            this.pnlHeaderInfo.Controls.Add(this.lblProtocolType);
            this.pnlHeaderInfo.Controls.Add(this.cmbProtocolType);
            this.pnlHeaderInfo.Controls.Add(this.lblHeader);
            this.pnlHeaderInfo.Controls.Add(this.txtProtocolLabel);
            this.pnlHeaderInfo.Controls.Add(this.lblProtocolLabel);
            this.pnlHeaderInfo.Controls.Add(this.btnHide);
            this.pnlHeaderInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeaderInfo.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlHeaderInfo.Location = new System.Drawing.Point(0, 0);
            this.pnlHeaderInfo.MaximumSize = new System.Drawing.Size(532, 162);
            this.pnlHeaderInfo.MinimumSize = new System.Drawing.Size(532, 162);
            this.pnlHeaderInfo.Name = "pnlHeaderInfo";
            this.pnlHeaderInfo.Size = new System.Drawing.Size(532, 162);
            this.pnlHeaderInfo.TabIndex = 6;
            this.pnlHeaderInfo.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlHeaderInfo_Paint);
            // 
            // btnLabelInfo
            // 
            this.btnLabelInfo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnLabelInfo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnLabelInfo.FlatAppearance.BorderSize = 0;
            this.btnLabelInfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLabelInfo.Image = global::Tesla.ProtocolEditor.Properties.Resources.Button_Info_icon1;
            this.btnLabelInfo.Location = new System.Drawing.Point(57, 60);
            this.btnLabelInfo.Name = "btnLabelInfo";
            this.btnLabelInfo.Size = new System.Drawing.Size(22, 20);
            this.btnLabelInfo.TabIndex = 31;
            this.btnLabelInfo.UseVisualStyleBackColor = true;
            this.btnLabelInfo.Click += new System.EventHandler(this.btnLabelInfo_Click);
            // 
            // lblRobosepType
            // 
            this.lblRobosepType.AutoSize = true;
            this.lblRobosepType.Location = new System.Drawing.Point(127, 9);
            this.lblRobosepType.Name = "lblRobosepType";
            this.lblRobosepType.Size = new System.Drawing.Size(90, 14);
            this.lblRobosepType.TabIndex = 30;
            this.lblRobosepType.Text = "RoboSep Type:   ";
            // 
            // btnProtocolSummary
            // 
            this.btnProtocolSummary.Location = new System.Drawing.Point(266, 29);
            this.btnProtocolSummary.Name = "btnProtocolSummary";
            this.btnProtocolSummary.Size = new System.Drawing.Size(111, 23);
            this.btnProtocolSummary.TabIndex = 29;
            this.btnProtocolSummary.Text = "View Summary";
            this.tipCheckProtocol.SetToolTip(this.btnProtocolSummary, "Provide summary of protocol.");
            this.btnProtocolSummary.Visible = false;
            this.btnProtocolSummary.Click += new System.EventHandler(this.btnProtocolSummary_Click);
            // 
            // btnAutoFill
            // 
            this.btnAutoFill.Location = new System.Drawing.Point(463, 112);
            this.btnAutoFill.Name = "btnAutoFill";
            this.btnAutoFill.Size = new System.Drawing.Size(56, 23);
            this.btnAutoFill.TabIndex = 28;
            this.btnAutoFill.Text = "Auto Fill";
            this.btnAutoFill.Visible = false;
            this.btnAutoFill.Click += new System.EventHandler(this.btnAutoFill_Click);
            // 
            // lblProtocolDesc
            // 
            this.lblProtocolDesc.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProtocolDesc.Location = new System.Drawing.Point(15, 91);
            this.lblProtocolDesc.Name = "lblProtocolDesc";
            this.lblProtocolDesc.Size = new System.Drawing.Size(64, 23);
            this.lblProtocolDesc.TabIndex = 27;
            this.lblProtocolDesc.Text = "Description";
            // 
            // txtProtocolDesc
            // 
            this.txtProtocolDesc.Location = new System.Drawing.Point(95, 86);
            this.txtProtocolDesc.MaxLength = 256;
            this.txtProtocolDesc.Name = "txtProtocolDesc";
            this.txtProtocolDesc.Size = new System.Drawing.Size(424, 20);
            this.txtProtocolDesc.TabIndex = 26;
            this.txtProtocolDesc.TextChanged += new System.EventHandler(this.txtProtocolDesc_TextChanged);
            // 
            // lblLowVolume
            // 
            this.lblLowVolume.Location = new System.Drawing.Point(255, 140);
            this.lblLowVolume.Name = "lblLowVolume";
            this.lblLowVolume.Size = new System.Drawing.Size(40, 23);
            this.lblLowVolume.TabIndex = 20;
            this.lblLowVolume.Text = "Low";
            // 
            // txtWorkingVolumeHigh
            // 
            this.txtWorkingVolumeHigh.Location = new System.Drawing.Point(407, 137);
            this.txtWorkingVolumeHigh.Name = "txtWorkingVolumeHigh";
            this.txtWorkingVolumeHigh.Size = new System.Drawing.Size(44, 20);
            this.txtWorkingVolumeHigh.TabIndex = 23;
            this.txtWorkingVolumeHigh.TextChanged += new System.EventHandler(this.txtWorkingVolumeHigh_TextChanged);
            // 
            // txtWorkingVolumeLow
            // 
            this.txtWorkingVolumeLow.Location = new System.Drawing.Point(311, 137);
            this.txtWorkingVolumeLow.Name = "txtWorkingVolumeLow";
            this.txtWorkingVolumeLow.Size = new System.Drawing.Size(44, 20);
            this.txtWorkingVolumeLow.TabIndex = 21;
            this.txtWorkingVolumeLow.TextChanged += new System.EventHandler(this.txtWorkingVolumeLow_TextChanged);
            // 
            // lblHighVolume
            // 
            this.lblHighVolume.Location = new System.Drawing.Point(373, 140);
            this.lblHighVolume.Name = "lblHighVolume";
            this.lblHighVolume.Size = new System.Drawing.Size(40, 23);
            this.lblHighVolume.TabIndex = 22;
            this.lblHighVolume.Text = "High";
            // 
            // txtWorkingVolumeSampleThreshold
            // 
            this.txtWorkingVolumeSampleThreshold.Location = new System.Drawing.Point(191, 137);
            this.txtWorkingVolumeSampleThreshold.Name = "txtWorkingVolumeSampleThreshold";
            this.txtWorkingVolumeSampleThreshold.Size = new System.Drawing.Size(44, 20);
            this.txtWorkingVolumeSampleThreshold.TabIndex = 19;
            this.txtWorkingVolumeSampleThreshold.TextChanged += new System.EventHandler(this.txtWorkingVolumeSampleThreshold_TextChanged);
            // 
            // lblWorkingVolumeSampleThreshold
            // 
            this.lblWorkingVolumeSampleThreshold.Location = new System.Drawing.Point(15, 140);
            this.lblWorkingVolumeSampleThreshold.Name = "lblWorkingVolumeSampleThreshold";
            this.lblWorkingVolumeSampleThreshold.Size = new System.Drawing.Size(168, 23);
            this.lblWorkingVolumeSampleThreshold.TabIndex = 18;
            this.lblWorkingVolumeSampleThreshold.Text = "Working Volume Threshold  (uL)";
            // 
            // txtSampleVolumeMax
            // 
            this.txtSampleVolumeMax.Location = new System.Drawing.Point(311, 113);
            this.txtSampleVolumeMax.Name = "txtSampleVolumeMax";
            this.txtSampleVolumeMax.Size = new System.Drawing.Size(44, 20);
            this.txtSampleVolumeMax.TabIndex = 17;
            this.txtSampleVolumeMax.TextChanged += new System.EventHandler(this.txtSampleVolumeMax_TextChanged);
            // 
            // txtSampleVolumeMin
            // 
            this.txtSampleVolumeMin.Location = new System.Drawing.Point(191, 113);
            this.txtSampleVolumeMin.Name = "txtSampleVolumeMin";
            this.txtSampleVolumeMin.Size = new System.Drawing.Size(44, 20);
            this.txtSampleVolumeMin.TabIndex = 15;
            this.txtSampleVolumeMin.TextChanged += new System.EventHandler(this.txtSampleVolumeMin_TextChanged);
            // 
            // lblSampleVolumeMax
            // 
            this.lblSampleVolumeMax.Location = new System.Drawing.Point(255, 116);
            this.lblSampleVolumeMax.Name = "lblSampleVolumeMax";
            this.lblSampleVolumeMax.Size = new System.Drawing.Size(60, 23);
            this.lblSampleVolumeMax.TabIndex = 16;
            this.lblSampleVolumeMax.Text = "Maximum";
            // 
            // lblSampleVolumeMin
            // 
            this.lblSampleVolumeMin.Location = new System.Drawing.Point(137, 116);
            this.lblSampleVolumeMin.Name = "lblSampleVolumeMin";
            this.lblSampleVolumeMin.Size = new System.Drawing.Size(56, 23);
            this.lblSampleVolumeMin.TabIndex = 14;
            this.lblSampleVolumeMin.Text = "Minimum";
            // 
            // lblSampleVolume
            // 
            this.lblSampleVolume.Location = new System.Drawing.Point(15, 116);
            this.lblSampleVolume.Name = "lblSampleVolume";
            this.lblSampleVolume.Size = new System.Drawing.Size(108, 23);
            this.lblSampleVolume.TabIndex = 13;
            this.lblSampleVolume.Text = "Sample Volume (uL)";
            // 
            // lblValidateResult
            // 
            this.lblValidateResult.Font = new System.Drawing.Font("Wingdings", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.lblValidateResult.Location = new System.Drawing.Point(493, 29);
            this.lblValidateResult.Name = "lblValidateResult";
            this.lblValidateResult.Size = new System.Drawing.Size(32, 28);
            this.lblValidateResult.TabIndex = 25;
            this.lblValidateResult.Text = "´";
            this.lblValidateResult.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblValidateResult.Visible = false;
            this.lblValidateResult.Click += new System.EventHandler(this.lblValidateResult_Click);
            // 
            // btnValidateProtocol
            // 
            this.btnValidateProtocol.Location = new System.Drawing.Point(378, 29);
            this.btnValidateProtocol.Name = "btnValidateProtocol";
            this.btnValidateProtocol.Size = new System.Drawing.Size(111, 23);
            this.btnValidateProtocol.TabIndex = 24;
            this.btnValidateProtocol.Text = "Check Protocol";
            this.tipCheckProtocol.SetToolTip(this.btnValidateProtocol, "Needed prior to saving protocol.");
            this.btnValidateProtocol.Visible = false;
            this.btnValidateProtocol.Click += new System.EventHandler(this.btnValidateProtocol_Click);
            // 
            // lblProtocolType
            // 
            this.lblProtocolType.Location = new System.Drawing.Point(15, 36);
            this.lblProtocolType.Name = "lblProtocolType";
            this.lblProtocolType.Size = new System.Drawing.Size(80, 23);
            this.lblProtocolType.TabIndex = 1;
            this.lblProtocolType.Text = "Protocol Type";
            // 
            // cmbProtocolType
            // 
            this.cmbProtocolType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProtocolType.Location = new System.Drawing.Point(95, 32);
            this.cmbProtocolType.Name = "cmbProtocolType";
            this.cmbProtocolType.Size = new System.Drawing.Size(142, 22);
            this.cmbProtocolType.TabIndex = 2;
            this.cmbProtocolType.SelectedIndexChanged += new System.EventHandler(this.cmbProtocolType_SelectedIndexChanged);
            // 
            // lblHeader
            // 
            this.lblHeader.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.Location = new System.Drawing.Point(12, 4);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(336, 23);
            this.lblHeader.TabIndex = 0;
            this.lblHeader.Text = "Description";
            this.lblHeader.Click += new System.EventHandler(this.lblHeader_Click);
            // 
            // txtProtocolLabel
            // 
            this.txtProtocolLabel.Location = new System.Drawing.Point(95, 60);
            this.txtProtocolLabel.Name = "txtProtocolLabel";
            this.txtProtocolLabel.Size = new System.Drawing.Size(424, 20);
            this.txtProtocolLabel.TabIndex = 4;
            this.txtProtocolLabel.TextChanged += new System.EventHandler(this.txtProtocolLabel_TextChanged);
            // 
            // lblProtocolLabel
            // 
            this.lblProtocolLabel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProtocolLabel.Location = new System.Drawing.Point(15, 63);
            this.lblProtocolLabel.Name = "lblProtocolLabel";
            this.lblProtocolLabel.Size = new System.Drawing.Size(36, 23);
            this.lblProtocolLabel.TabIndex = 3;
            this.lblProtocolLabel.Text = "Label";
            // 
            // btnHide
            // 
            this.btnHide.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHide.Location = new System.Drawing.Point(436, 4);
            this.btnHide.Name = "btnHide";
            this.btnHide.Size = new System.Drawing.Size(90, 16);
            this.btnHide.TabIndex = 8;
            this.btnHide.Text = "Hide Header";
            this.btnHide.Visible = false;
            this.btnHide.Click += new System.EventHandler(this.btnHide_Click);
            // 
            // pnlCommandDetail
            // 
            this.pnlCommandDetail.Controls.Add(this.myMixTransCommandPanel);
            this.pnlCommandDetail.Controls.Add(this.myResusMixSepTransCommandPanel);
            this.pnlCommandDetail.Controls.Add(this.myResusMixCommandPanel);
            this.pnlCommandDetail.Controls.Add(this.myTopUpTransCommandPanel);
            this.pnlCommandDetail.Controls.Add(this.myTopUpTransSepTransCommandPanel);
            this.pnlCommandDetail.Controls.Add(this.myTopUpMixTransCommandPanel);
            this.pnlCommandDetail.Controls.Add(this.btnConfirmCommand);
            this.pnlCommandDetail.Controls.Add(this.lblCommandDetails);
            this.pnlCommandDetail.Controls.Add(this.myIncubateCommandPanel);
            this.pnlCommandDetail.Controls.Add(this.myPauseCommandPanel);
            this.pnlCommandDetail.Controls.Add(this.mySeparateCommandPanel);
            this.pnlCommandDetail.Controls.Add(this.myDemoCommandPanel);
            this.pnlCommandDetail.Controls.Add(this.myCommandPanel);
            this.pnlCommandDetail.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlCommandDetail.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlCommandDetail.Location = new System.Drawing.Point(0, 326);
            this.pnlCommandDetail.MaximumSize = new System.Drawing.Size(532, 188);
            this.pnlCommandDetail.MinimumSize = new System.Drawing.Size(532, 188);
            this.pnlCommandDetail.Name = "pnlCommandDetail";
            this.pnlCommandDetail.Size = new System.Drawing.Size(532, 188);
            this.pnlCommandDetail.TabIndex = 8;
            // 
            // myMixTransCommandPanel
            // 
            this.myMixTransCommandPanel.AbsoluteVolume_uL = 0;
            this.myMixTransCommandPanel.AbsoluteVolume_uL2 = 0;
            this.myMixTransCommandPanel.CommandExtensionTime = ((uint)(0u));
            this.myMixTransCommandPanel.CommandLabel = "";
            this.myMixTransCommandPanel.CommandType = "";
            this.myMixTransCommandPanel.DestinationVial = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myMixTransCommandPanel.DestinationVial2 = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myMixTransCommandPanel.EnableExtension = true;
            this.myMixTransCommandPanel.IsChangeAllowed = true;
            this.myMixTransCommandPanel.IsThresholdStyle = false;
            this.myMixTransCommandPanel.IsVolumeSpecificationRequired = true;
            this.myMixTransCommandPanel.IsVolumeSpecificationRequired2 = true;
            this.myMixTransCommandPanel.Location = new System.Drawing.Point(0, 32);
            this.myMixTransCommandPanel.MixCycles = 1;
            this.myMixTransCommandPanel.Name = "myMixTransCommandPanel";
            this.myMixTransCommandPanel.RelativeVolumeProportion = 0D;
            this.myMixTransCommandPanel.RelativeVolumeProportion2 = 0D;
            this.myMixTransCommandPanel.Size = new System.Drawing.Size(532, 156);
            this.myMixTransCommandPanel.SourceVial = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myMixTransCommandPanel.SourceVial2 = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myMixTransCommandPanel.TabIndex = 15;
            this.myMixTransCommandPanel.TipRack = 1;
            this.myMixTransCommandPanel.TipRackSpecified = false;
            this.myMixTransCommandPanel.TipTubeBottomGap = 0;
            this.myMixTransCommandPanel.UseBufferTip = false;
            this.myMixTransCommandPanel.VisibleExtension = true;
            this.myMixTransCommandPanel.VolumeTypeSpecifier = Tesla.Common.ProtocolCommand.VolumeType.NotSpecified;
            this.myMixTransCommandPanel.VolumeTypeSpecifier2 = Tesla.Common.ProtocolCommand.VolumeType.NotSpecified;
            // 
            // myResusMixSepTransCommandPanel
            // 
            this.myResusMixSepTransCommandPanel.AbsoluteVolume_uL = 0;
            this.myResusMixSepTransCommandPanel.AbsoluteVolume_uL2 = 0;
            this.myResusMixSepTransCommandPanel.AbsoluteVolume_uL3 = 0;
            this.myResusMixSepTransCommandPanel.AbsoluteVolume_uLMix = 0;
            this.myResusMixSepTransCommandPanel.CommandExtensionTime = ((uint)(0u));
            this.myResusMixSepTransCommandPanel.CommandLabel = "";
            this.myResusMixSepTransCommandPanel.CommandType = "";
            this.myResusMixSepTransCommandPanel.DestinationVial = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myResusMixSepTransCommandPanel.DestinationVial2 = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myResusMixSepTransCommandPanel.DestinationVial3 = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myResusMixSepTransCommandPanel.DestinationVialMix = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myResusMixSepTransCommandPanel.EnableExtension = true;
            this.myResusMixSepTransCommandPanel.IsChangeAllowed = true;
            this.myResusMixSepTransCommandPanel.IsThresholdStyle = false;
            this.myResusMixSepTransCommandPanel.IsVolumeSpecificationRequired = true;
            this.myResusMixSepTransCommandPanel.IsVolumeSpecificationRequired2 = true;
            this.myResusMixSepTransCommandPanel.IsVolumeSpecificationRequired3 = true;
            this.myResusMixSepTransCommandPanel.IsVolumeSpecificationRequiredMix = true;
            this.myResusMixSepTransCommandPanel.Location = new System.Drawing.Point(0, 32);
            this.myResusMixSepTransCommandPanel.MixCycles = 1;
            this.myResusMixSepTransCommandPanel.Name = "myResusMixSepTransCommandPanel";
            this.myResusMixSepTransCommandPanel.RelativeVolumeProportion = 0D;
            this.myResusMixSepTransCommandPanel.RelativeVolumeProportion2 = 0D;
            this.myResusMixSepTransCommandPanel.RelativeVolumeProportion3 = 0D;
            this.myResusMixSepTransCommandPanel.RelativeVolumeProportionMix = 0D;
            this.myResusMixSepTransCommandPanel.Size = new System.Drawing.Size(532, 156);
            this.myResusMixSepTransCommandPanel.SourceVial = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myResusMixSepTransCommandPanel.SourceVial2 = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myResusMixSepTransCommandPanel.SourceVial3 = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myResusMixSepTransCommandPanel.SourceVialMix = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myResusMixSepTransCommandPanel.TabIndex = 14;
            this.myResusMixSepTransCommandPanel.TipRack = 1;
            this.myResusMixSepTransCommandPanel.TipRackSpecified = false;
            this.myResusMixSepTransCommandPanel.TipTubeBottomGap = 0;
            this.myResusMixSepTransCommandPanel.UseBufferTip = false;
            this.myResusMixSepTransCommandPanel.VisibleExtension = true;
            this.myResusMixSepTransCommandPanel.VolumeTypeSpecifier = Tesla.Common.ProtocolCommand.VolumeType.NotSpecified;
            this.myResusMixSepTransCommandPanel.VolumeTypeSpecifier2 = Tesla.Common.ProtocolCommand.VolumeType.NotSpecified;
            this.myResusMixSepTransCommandPanel.VolumeTypeSpecifier3 = Tesla.Common.ProtocolCommand.VolumeType.NotSpecified;
            this.myResusMixSepTransCommandPanel.VolumeTypeSpecifierMix = Tesla.Common.ProtocolCommand.VolumeType.NotSpecified;
            this.myResusMixSepTransCommandPanel.WaitCommandTimeDuration = ((uint)(0u));
            // 
            // myResusMixCommandPanel
            // 
            this.myResusMixCommandPanel.AbsoluteVolume_uL = 0;
            this.myResusMixCommandPanel.AbsoluteVolume_uL2 = 0;
            this.myResusMixCommandPanel.AbsoluteVolume_uL3 = 0;
            this.myResusMixCommandPanel.AbsoluteVolume_uLMix = 0;
            this.myResusMixCommandPanel.CommandExtensionTime = ((uint)(0u));
            this.myResusMixCommandPanel.CommandLabel = "";
            this.myResusMixCommandPanel.CommandType = "";
            this.myResusMixCommandPanel.DestinationVial = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myResusMixCommandPanel.DestinationVial2 = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myResusMixCommandPanel.DestinationVial3 = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myResusMixCommandPanel.DestinationVialMix = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myResusMixCommandPanel.EnableExtension = true;
            this.myResusMixCommandPanel.IsChangeAllowed = true;
            this.myResusMixCommandPanel.IsThresholdStyle = false;
            this.myResusMixCommandPanel.IsVolumeSpecificationRequired = true;
            this.myResusMixCommandPanel.IsVolumeSpecificationRequired2 = true;
            this.myResusMixCommandPanel.IsVolumeSpecificationRequired3 = true;
            this.myResusMixCommandPanel.IsVolumeSpecificationRequiredMix = true;
            this.myResusMixCommandPanel.Location = new System.Drawing.Point(0, 32);
            this.myResusMixCommandPanel.MixCycles = 1;
            this.myResusMixCommandPanel.Name = "myResusMixCommandPanel";
            this.myResusMixCommandPanel.RelativeVolumeProportion = 0D;
            this.myResusMixCommandPanel.RelativeVolumeProportion2 = 0D;
            this.myResusMixCommandPanel.RelativeVolumeProportion3 = 0D;
            this.myResusMixCommandPanel.RelativeVolumeProportionMix = 0D;
            this.myResusMixCommandPanel.Size = new System.Drawing.Size(532, 156);
            this.myResusMixCommandPanel.SourceVial = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myResusMixCommandPanel.SourceVial2 = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myResusMixCommandPanel.SourceVial3 = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myResusMixCommandPanel.SourceVialMix = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myResusMixCommandPanel.TabIndex = 13;
            this.myResusMixCommandPanel.TipRack = 1;
            this.myResusMixCommandPanel.TipRackSpecified = false;
            this.myResusMixCommandPanel.TipTubeBottomGap = 0;
            this.myResusMixCommandPanel.UseBufferTip = false;
            this.myResusMixCommandPanel.VisibleExtension = true;
            this.myResusMixCommandPanel.VolumeTypeSpecifier = Tesla.Common.ProtocolCommand.VolumeType.NotSpecified;
            this.myResusMixCommandPanel.VolumeTypeSpecifier2 = Tesla.Common.ProtocolCommand.VolumeType.NotSpecified;
            this.myResusMixCommandPanel.VolumeTypeSpecifier3 = Tesla.Common.ProtocolCommand.VolumeType.NotSpecified;
            this.myResusMixCommandPanel.VolumeTypeSpecifierMix = Tesla.Common.ProtocolCommand.VolumeType.NotSpecified;
            this.myResusMixCommandPanel.WaitCommandTimeDuration = ((uint)(0u));
            // 
            // myTopUpTransCommandPanel
            // 
            this.myTopUpTransCommandPanel.AbsoluteVolume_uL = 0;
            this.myTopUpTransCommandPanel.AbsoluteVolume_uL2 = 0;
            this.myTopUpTransCommandPanel.AbsoluteVolume_uL3 = 0;
            this.myTopUpTransCommandPanel.AbsoluteVolume_uLMix = 0;
            this.myTopUpTransCommandPanel.CommandExtensionTime = ((uint)(0u));
            this.myTopUpTransCommandPanel.CommandLabel = "";
            this.myTopUpTransCommandPanel.CommandType = "";
            this.myTopUpTransCommandPanel.DestinationVial = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myTopUpTransCommandPanel.DestinationVial2 = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myTopUpTransCommandPanel.DestinationVial3 = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myTopUpTransCommandPanel.DestinationVialMix = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myTopUpTransCommandPanel.EnableExtension = true;
            this.myTopUpTransCommandPanel.IsChangeAllowed = true;
            this.myTopUpTransCommandPanel.IsThresholdStyle = false;
            this.myTopUpTransCommandPanel.IsVolumeSpecificationRequired = true;
            this.myTopUpTransCommandPanel.IsVolumeSpecificationRequired2 = true;
            this.myTopUpTransCommandPanel.IsVolumeSpecificationRequired3 = true;
            this.myTopUpTransCommandPanel.IsVolumeSpecificationRequiredMix = true;
            this.myTopUpTransCommandPanel.Location = new System.Drawing.Point(0, 32);
            this.myTopUpTransCommandPanel.MixCycles = 1;
            this.myTopUpTransCommandPanel.Name = "myTopUpTransCommandPanel";
            this.myTopUpTransCommandPanel.RelativeVolumeProportion = 0D;
            this.myTopUpTransCommandPanel.RelativeVolumeProportion2 = 0D;
            this.myTopUpTransCommandPanel.RelativeVolumeProportion3 = 0D;
            this.myTopUpTransCommandPanel.RelativeVolumeProportionMix = 0D;
            this.myTopUpTransCommandPanel.Size = new System.Drawing.Size(532, 156);
            this.myTopUpTransCommandPanel.SourceVial = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myTopUpTransCommandPanel.SourceVial2 = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myTopUpTransCommandPanel.SourceVial3 = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myTopUpTransCommandPanel.SourceVialMix = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myTopUpTransCommandPanel.TabIndex = 12;
            this.myTopUpTransCommandPanel.TipRack = 1;
            this.myTopUpTransCommandPanel.TipRackSpecified = false;
            this.myTopUpTransCommandPanel.TipTubeBottomGap = 0;
            this.myTopUpTransCommandPanel.UseBufferTip = false;
            this.myTopUpTransCommandPanel.VisibleExtension = true;
            this.myTopUpTransCommandPanel.VolumeTypeSpecifier = Tesla.Common.ProtocolCommand.VolumeType.NotSpecified;
            this.myTopUpTransCommandPanel.VolumeTypeSpecifier2 = Tesla.Common.ProtocolCommand.VolumeType.NotSpecified;
            this.myTopUpTransCommandPanel.VolumeTypeSpecifier3 = Tesla.Common.ProtocolCommand.VolumeType.NotSpecified;
            this.myTopUpTransCommandPanel.VolumeTypeSpecifierMix = Tesla.Common.ProtocolCommand.VolumeType.NotSpecified;
            this.myTopUpTransCommandPanel.WaitCommandTimeDuration = ((uint)(0u));
            // 
            // myTopUpTransSepTransCommandPanel
            // 
            this.myTopUpTransSepTransCommandPanel.AbsoluteVolume_uL = 0;
            this.myTopUpTransSepTransCommandPanel.AbsoluteVolume_uL2 = 0;
            this.myTopUpTransSepTransCommandPanel.AbsoluteVolume_uL3 = 0;
            this.myTopUpTransSepTransCommandPanel.AbsoluteVolume_uLMix = 0;
            this.myTopUpTransSepTransCommandPanel.CommandExtensionTime = ((uint)(0u));
            this.myTopUpTransSepTransCommandPanel.CommandLabel = "";
            this.myTopUpTransSepTransCommandPanel.CommandType = "";
            this.myTopUpTransSepTransCommandPanel.DestinationVial = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myTopUpTransSepTransCommandPanel.DestinationVial2 = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myTopUpTransSepTransCommandPanel.DestinationVial3 = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myTopUpTransSepTransCommandPanel.DestinationVialMix = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myTopUpTransSepTransCommandPanel.EnableExtension = true;
            this.myTopUpTransSepTransCommandPanel.IsChangeAllowed = true;
            this.myTopUpTransSepTransCommandPanel.IsThresholdStyle = false;
            this.myTopUpTransSepTransCommandPanel.IsVolumeSpecificationRequired = true;
            this.myTopUpTransSepTransCommandPanel.IsVolumeSpecificationRequired2 = true;
            this.myTopUpTransSepTransCommandPanel.IsVolumeSpecificationRequired3 = true;
            this.myTopUpTransSepTransCommandPanel.IsVolumeSpecificationRequiredMix = true;
            this.myTopUpTransSepTransCommandPanel.Location = new System.Drawing.Point(0, 32);
            this.myTopUpTransSepTransCommandPanel.MixCycles = 1;
            this.myTopUpTransSepTransCommandPanel.Name = "myTopUpTransSepTransCommandPanel";
            this.myTopUpTransSepTransCommandPanel.RelativeVolumeProportion = 0D;
            this.myTopUpTransSepTransCommandPanel.RelativeVolumeProportion2 = 0D;
            this.myTopUpTransSepTransCommandPanel.RelativeVolumeProportion3 = 0D;
            this.myTopUpTransSepTransCommandPanel.RelativeVolumeProportionMix = 0D;
            this.myTopUpTransSepTransCommandPanel.Size = new System.Drawing.Size(532, 156);
            this.myTopUpTransSepTransCommandPanel.SourceVial = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myTopUpTransSepTransCommandPanel.SourceVial2 = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myTopUpTransSepTransCommandPanel.SourceVial3 = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myTopUpTransSepTransCommandPanel.SourceVialMix = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myTopUpTransSepTransCommandPanel.TabIndex = 11;
            this.myTopUpTransSepTransCommandPanel.TipRack = 1;
            this.myTopUpTransSepTransCommandPanel.TipRackSpecified = false;
            this.myTopUpTransSepTransCommandPanel.TipTubeBottomGap = 0;
            this.myTopUpTransSepTransCommandPanel.UseBufferTip = false;
            this.myTopUpTransSepTransCommandPanel.VisibleExtension = true;
            this.myTopUpTransSepTransCommandPanel.VolumeTypeSpecifier = Tesla.Common.ProtocolCommand.VolumeType.NotSpecified;
            this.myTopUpTransSepTransCommandPanel.VolumeTypeSpecifier2 = Tesla.Common.ProtocolCommand.VolumeType.NotSpecified;
            this.myTopUpTransSepTransCommandPanel.VolumeTypeSpecifier3 = Tesla.Common.ProtocolCommand.VolumeType.NotSpecified;
            this.myTopUpTransSepTransCommandPanel.VolumeTypeSpecifierMix = Tesla.Common.ProtocolCommand.VolumeType.NotSpecified;
            this.myTopUpTransSepTransCommandPanel.WaitCommandTimeDuration = ((uint)(0u));
            // 
            // myTopUpMixTransCommandPanel
            // 
            this.myTopUpMixTransCommandPanel.AbsoluteVolume_uL = 0;
            this.myTopUpMixTransCommandPanel.AbsoluteVolume_uL2 = 0;
            this.myTopUpMixTransCommandPanel.AbsoluteVolume_uL3 = 0;
            this.myTopUpMixTransCommandPanel.AbsoluteVolume_uLMix = 0;
            this.myTopUpMixTransCommandPanel.CommandExtensionTime = ((uint)(0u));
            this.myTopUpMixTransCommandPanel.CommandLabel = "";
            this.myTopUpMixTransCommandPanel.CommandType = "";
            this.myTopUpMixTransCommandPanel.DestinationVial = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myTopUpMixTransCommandPanel.DestinationVial2 = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myTopUpMixTransCommandPanel.DestinationVial3 = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myTopUpMixTransCommandPanel.DestinationVialMix = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myTopUpMixTransCommandPanel.EnableExtension = true;
            this.myTopUpMixTransCommandPanel.IsChangeAllowed = true;
            this.myTopUpMixTransCommandPanel.IsThresholdStyle = false;
            this.myTopUpMixTransCommandPanel.IsVolumeSpecificationRequired = true;
            this.myTopUpMixTransCommandPanel.IsVolumeSpecificationRequired2 = true;
            this.myTopUpMixTransCommandPanel.IsVolumeSpecificationRequired3 = true;
            this.myTopUpMixTransCommandPanel.IsVolumeSpecificationRequiredMix = true;
            this.myTopUpMixTransCommandPanel.Location = new System.Drawing.Point(0, 32);
            this.myTopUpMixTransCommandPanel.MixCycles = 1;
            this.myTopUpMixTransCommandPanel.Name = "myTopUpMixTransCommandPanel";
            this.myTopUpMixTransCommandPanel.RelativeVolumeProportion = 0D;
            this.myTopUpMixTransCommandPanel.RelativeVolumeProportion2 = 0D;
            this.myTopUpMixTransCommandPanel.RelativeVolumeProportion3 = 0D;
            this.myTopUpMixTransCommandPanel.RelativeVolumeProportionMix = 0D;
            this.myTopUpMixTransCommandPanel.Size = new System.Drawing.Size(532, 156);
            this.myTopUpMixTransCommandPanel.SourceVial = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myTopUpMixTransCommandPanel.SourceVial2 = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myTopUpMixTransCommandPanel.SourceVial3 = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myTopUpMixTransCommandPanel.SourceVialMix = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myTopUpMixTransCommandPanel.TabIndex = 10;
            this.myTopUpMixTransCommandPanel.TipRack = 1;
            this.myTopUpMixTransCommandPanel.TipRackSpecified = false;
            this.myTopUpMixTransCommandPanel.TipTubeBottomGap = 0;
            this.myTopUpMixTransCommandPanel.UseBufferTip = false;
            this.myTopUpMixTransCommandPanel.VisibleExtension = true;
            this.myTopUpMixTransCommandPanel.VolumeTypeSpecifier = Tesla.Common.ProtocolCommand.VolumeType.NotSpecified;
            this.myTopUpMixTransCommandPanel.VolumeTypeSpecifier2 = Tesla.Common.ProtocolCommand.VolumeType.NotSpecified;
            this.myTopUpMixTransCommandPanel.VolumeTypeSpecifier3 = Tesla.Common.ProtocolCommand.VolumeType.NotSpecified;
            this.myTopUpMixTransCommandPanel.VolumeTypeSpecifierMix = Tesla.Common.ProtocolCommand.VolumeType.NotSpecified;
            this.myTopUpMixTransCommandPanel.WaitCommandTimeDuration = ((uint)(0u));
            // 
            // btnConfirmCommand
            // 
            this.btnConfirmCommand.Location = new System.Drawing.Point(414, 4);
            this.btnConfirmCommand.Name = "btnConfirmCommand";
            this.btnConfirmCommand.Size = new System.Drawing.Size(108, 23);
            this.btnConfirmCommand.TabIndex = 9;
            this.btnConfirmCommand.Text = "Confirm Command";
            this.tipConfirmCommand.SetToolTip(this.btnConfirmCommand, "Confirm command choice and add to command window.");
            this.btnConfirmCommand.Visible = false;
            this.btnConfirmCommand.Click += new System.EventHandler(this.btnConfirmCommand_Click);
            // 
            // lblCommandDetails
            // 
            this.lblCommandDetails.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCommandDetails.Location = new System.Drawing.Point(12, 4);
            this.lblCommandDetails.Name = "lblCommandDetails";
            this.lblCommandDetails.Size = new System.Drawing.Size(168, 23);
            this.lblCommandDetails.TabIndex = 0;
            this.lblCommandDetails.Text = "Command Details";
            // 
            // myIncubateCommandPanel
            // 
            this.myIncubateCommandPanel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.myIncubateCommandPanel.CommandExtensionTime = ((uint)(0u));
            this.myIncubateCommandPanel.CommandLabel = "";
            this.myIncubateCommandPanel.CommandType = "";
            this.myIncubateCommandPanel.EnableExtension = false;
            this.myIncubateCommandPanel.Location = new System.Drawing.Point(0, 26);
            this.myIncubateCommandPanel.Margin = new System.Windows.Forms.Padding(0);
            this.myIncubateCommandPanel.Name = "myIncubateCommandPanel";
            this.myIncubateCommandPanel.Size = new System.Drawing.Size(532, 156);
            this.myIncubateCommandPanel.TabIndex = 5;
            this.myIncubateCommandPanel.VisibleExtension = true;
            this.myIncubateCommandPanel.WaitCommandTimeDuration = ((uint)(0u));
            // 
            // myPauseCommandPanel
            // 
            this.myPauseCommandPanel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.myPauseCommandPanel.CommandExtensionTime = ((uint)(0u));
            this.myPauseCommandPanel.CommandLabel = "";
            this.myPauseCommandPanel.CommandType = "";
            this.myPauseCommandPanel.EnableExtension = false;
            this.myPauseCommandPanel.Location = new System.Drawing.Point(0, 26);
            this.myPauseCommandPanel.Margin = new System.Windows.Forms.Padding(0);
            this.myPauseCommandPanel.Name = "myPauseCommandPanel";
            this.myPauseCommandPanel.Size = new System.Drawing.Size(532, 156);
            this.myPauseCommandPanel.TabIndex = 6;
            this.myPauseCommandPanel.VisibleExtension = false;
            // 
            // mySeparateCommandPanel
            // 
            this.mySeparateCommandPanel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.mySeparateCommandPanel.CommandExtensionTime = ((uint)(0u));
            this.mySeparateCommandPanel.CommandLabel = "";
            this.mySeparateCommandPanel.CommandType = "";
            this.mySeparateCommandPanel.EnableExtension = false;
            this.mySeparateCommandPanel.Location = new System.Drawing.Point(0, 26);
            this.mySeparateCommandPanel.Margin = new System.Windows.Forms.Padding(0);
            this.mySeparateCommandPanel.Name = "mySeparateCommandPanel";
            this.mySeparateCommandPanel.Size = new System.Drawing.Size(532, 156);
            this.mySeparateCommandPanel.TabIndex = 6;
            this.mySeparateCommandPanel.VisibleExtension = true;
            this.mySeparateCommandPanel.WaitCommandTimeDuration = ((uint)(0u));
            this.mySeparateCommandPanel.Load += new System.EventHandler(this.mySeparateCommandPanel_Load);
            // 
            // myDemoCommandPanel
            // 
            this.myDemoCommandPanel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.myDemoCommandPanel.CommandExtensionTime = ((uint)(0u));
            this.myDemoCommandPanel.CommandLabel = "";
            this.myDemoCommandPanel.CommandType = "";
            this.myDemoCommandPanel.DemoCommandIterationCount = ((uint)(0u));
            this.myDemoCommandPanel.EnableExtension = false;
            this.myDemoCommandPanel.Location = new System.Drawing.Point(0, 26);
            this.myDemoCommandPanel.Margin = new System.Windows.Forms.Padding(0);
            this.myDemoCommandPanel.Name = "myDemoCommandPanel";
            this.myDemoCommandPanel.Size = new System.Drawing.Size(532, 156);
            this.myDemoCommandPanel.TabIndex = 4;
            this.myDemoCommandPanel.VisibleExtension = true;
            this.myDemoCommandPanel.Load += new System.EventHandler(this.myDemoCommandPanel_Load);
            // 
            // myCommandPanel
            // 
            this.myCommandPanel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.myCommandPanel.CommandExtensionTime = ((uint)(0u));
            this.myCommandPanel.CommandLabel = "";
            this.myCommandPanel.CommandType = "";
            this.myCommandPanel.EnableExtension = false;
            this.myCommandPanel.Location = new System.Drawing.Point(0, 26);
            this.myCommandPanel.Margin = new System.Windows.Forms.Padding(0);
            this.myCommandPanel.Name = "myCommandPanel";
            this.myCommandPanel.Size = new System.Drawing.Size(532, 156);
            this.myCommandPanel.TabIndex = 3;
            this.myCommandPanel.VisibleExtension = true;
            // 
            // errorProtocolLabel
            // 
            this.errorProtocolLabel.ContainerControl = this;
            // 
            // errorProtocolAuthor
            // 
            this.errorProtocolAuthor.ContainerControl = this;
            // 
            // errorWorkingVolumeThreshold
            // 
            this.errorWorkingVolumeThreshold.ContainerControl = this;
            // 
            // errorSampleVolumeMin
            // 
            this.errorSampleVolumeMin.ContainerControl = this;
            // 
            // errorSampleVolumeMax
            // 
            this.errorSampleVolumeMax.ContainerControl = this;
            // 
            // errorCommandSequence
            // 
            this.errorCommandSequence.ContainerControl = this;
            // 
            // errorWorkingVolumeLow
            // 
            this.errorWorkingVolumeLow.ContainerControl = this;
            // 
            // errorWorkingVolumeHigh
            // 
            this.errorWorkingVolumeHigh.ContainerControl = this;
            // 
            // myWorkingVolumeCommandPanel
            // 
            this.myWorkingVolumeCommandPanel.AbsoluteVolume_uL = 0;
            this.myWorkingVolumeCommandPanel.CommandExtensionTime = ((uint)(0u));
            this.myWorkingVolumeCommandPanel.CommandLabel = "";
            this.myWorkingVolumeCommandPanel.CommandType = "";
            this.myWorkingVolumeCommandPanel.DestinationVial = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myWorkingVolumeCommandPanel.EnableExtension = false;
            this.myWorkingVolumeCommandPanel.FreeAirDispense = false;
            this.myWorkingVolumeCommandPanel.IsChangeAllowed = true;
            this.myWorkingVolumeCommandPanel.IsThresholdStyle = false;
            this.myWorkingVolumeCommandPanel.IsVolumeSpecificationRequired = true;
            this.myWorkingVolumeCommandPanel.Location = new System.Drawing.Point(0, 32);
            this.myWorkingVolumeCommandPanel.Name = "myWorkingVolumeCommandPanel";
            this.myWorkingVolumeCommandPanel.RelativeVolumeProportion = 0D;
            this.myWorkingVolumeCommandPanel.Size = new System.Drawing.Size(538, 156);
            this.myWorkingVolumeCommandPanel.SourceVial = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myWorkingVolumeCommandPanel.TabIndex = 5;
            this.myWorkingVolumeCommandPanel.TipRack = 1;
            this.myWorkingVolumeCommandPanel.TipRackSpecified = false;
            this.myWorkingVolumeCommandPanel.UseBufferTip = false;
            this.myWorkingVolumeCommandPanel.VisibleExtension = true;
            this.myWorkingVolumeCommandPanel.VolumeTypeSpecifier = Tesla.Common.ProtocolCommand.VolumeType.NotSpecified;
            // 
            // myVolumeMaintenanceCommandPanel
            // 
            this.myVolumeMaintenanceCommandPanel.AbsoluteVolume_uL = 0;
            this.myVolumeMaintenanceCommandPanel.CommandExtensionTime = ((uint)(0u));
            this.myVolumeMaintenanceCommandPanel.CommandLabel = "";
            this.myVolumeMaintenanceCommandPanel.CommandType = "";
            this.myVolumeMaintenanceCommandPanel.DestinationVial = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myVolumeMaintenanceCommandPanel.EnableExtension = false;
            this.myVolumeMaintenanceCommandPanel.HomeFlag = false;
            this.myVolumeMaintenanceCommandPanel.IsChangeAllowed = true;
            this.myVolumeMaintenanceCommandPanel.IsThresholdStyle = false;
            this.myVolumeMaintenanceCommandPanel.IsVolumeSpecificationRequired = true;
            this.myVolumeMaintenanceCommandPanel.Location = new System.Drawing.Point(0, 32);
            this.myVolumeMaintenanceCommandPanel.Name = "myVolumeMaintenanceCommandPanel";
            this.myVolumeMaintenanceCommandPanel.RelativeVolumeProportion = 0D;
            this.myVolumeMaintenanceCommandPanel.Size = new System.Drawing.Size(532, 156);
            this.myVolumeMaintenanceCommandPanel.SourceVial = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myVolumeMaintenanceCommandPanel.TabIndex = 8;
            this.myVolumeMaintenanceCommandPanel.TipRack = 1;
            this.myVolumeMaintenanceCommandPanel.TipRackSpecified = false;
            this.myVolumeMaintenanceCommandPanel.UseBufferTip = false;
            this.myVolumeMaintenanceCommandPanel.VisibleExtension = true;
            this.myVolumeMaintenanceCommandPanel.VolumeTypeSpecifier = Tesla.Common.ProtocolCommand.VolumeType.NotSpecified;
            // 
            // myVolumeCommandPanel
            // 
            this.myVolumeCommandPanel.AbsoluteVolume_uL = 0;
            this.myVolumeCommandPanel.CommandExtensionTime = ((uint)(0u));
            this.myVolumeCommandPanel.CommandLabel = "";
            this.myVolumeCommandPanel.CommandType = "";
            this.myVolumeCommandPanel.DestinationVial = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myVolumeCommandPanel.EnableExtension = false;
            this.myVolumeCommandPanel.IsChangeAllowed = true;
            this.myVolumeCommandPanel.IsThresholdStyle = false;
            this.myVolumeCommandPanel.IsVolumeSpecificationRequired = true;
            this.myVolumeCommandPanel.Location = new System.Drawing.Point(0, 32);
            this.myVolumeCommandPanel.MixCycles = 1;
            this.myVolumeCommandPanel.Name = "myVolumeCommandPanel";
            this.myVolumeCommandPanel.RelativeVolumeProportion = 0D;
            this.myVolumeCommandPanel.Size = new System.Drawing.Size(532, 156);
            this.myVolumeCommandPanel.SourceVial = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myVolumeCommandPanel.TabIndex = 7;
            this.myVolumeCommandPanel.TipRack = 1;
            this.myVolumeCommandPanel.TipRackSpecified = false;
            this.myVolumeCommandPanel.TipTubeBottomGap = 0;
            this.myVolumeCommandPanel.UseBufferTip = false;
            this.myVolumeCommandPanel.VisibleExtension = true;
            this.myVolumeCommandPanel.VolumeTypeSpecifier = Tesla.Common.ProtocolCommand.VolumeType.NotSpecified;
            // 
            // myTopUpMixTransSepTransCommandPanel
            // 
            this.myTopUpMixTransSepTransCommandPanel.AbsoluteVolume_uL = 0;
            this.myTopUpMixTransSepTransCommandPanel.AbsoluteVolume_uL2 = 0;
            this.myTopUpMixTransSepTransCommandPanel.AbsoluteVolume_uL3 = 0;
            this.myTopUpMixTransSepTransCommandPanel.AbsoluteVolume_uLMix = 0;
            this.myTopUpMixTransSepTransCommandPanel.CommandExtensionTime = ((uint)(0u));
            this.myTopUpMixTransSepTransCommandPanel.CommandLabel = "";
            this.myTopUpMixTransSepTransCommandPanel.CommandType = "";
            this.myTopUpMixTransSepTransCommandPanel.DestinationVial = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myTopUpMixTransSepTransCommandPanel.DestinationVial2 = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myTopUpMixTransSepTransCommandPanel.DestinationVial3 = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myTopUpMixTransSepTransCommandPanel.DestinationVialMix = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myTopUpMixTransSepTransCommandPanel.EnableExtension = true;
            this.myTopUpMixTransSepTransCommandPanel.IsChangeAllowed = true;
            this.myTopUpMixTransSepTransCommandPanel.IsThresholdStyle = false;
            this.myTopUpMixTransSepTransCommandPanel.IsVolumeSpecificationRequired = true;
            this.myTopUpMixTransSepTransCommandPanel.IsVolumeSpecificationRequired2 = true;
            this.myTopUpMixTransSepTransCommandPanel.IsVolumeSpecificationRequired3 = true;
            this.myTopUpMixTransSepTransCommandPanel.IsVolumeSpecificationRequiredMix = true;
            this.myTopUpMixTransSepTransCommandPanel.Location = new System.Drawing.Point(0, 0);
            this.myTopUpMixTransSepTransCommandPanel.MixCycles = 1;
            this.myTopUpMixTransSepTransCommandPanel.Name = "myTopUpMixTransSepTransCommandPanel";
            this.myTopUpMixTransSepTransCommandPanel.RelativeVolumeProportion = 0D;
            this.myTopUpMixTransSepTransCommandPanel.RelativeVolumeProportion2 = 0D;
            this.myTopUpMixTransSepTransCommandPanel.RelativeVolumeProportion3 = 0D;
            this.myTopUpMixTransSepTransCommandPanel.RelativeVolumeProportionMix = 0D;
            this.myTopUpMixTransSepTransCommandPanel.Size = new System.Drawing.Size(532, 156);
            this.myTopUpMixTransSepTransCommandPanel.SourceVial = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myTopUpMixTransSepTransCommandPanel.SourceVial2 = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myTopUpMixTransSepTransCommandPanel.SourceVial3 = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myTopUpMixTransSepTransCommandPanel.SourceVialMix = Tesla.Common.Separator.AbsoluteResourceLocation.TPC0001;
            this.myTopUpMixTransSepTransCommandPanel.TabIndex = 0;
            this.myTopUpMixTransSepTransCommandPanel.TipRack = 1;
            this.myTopUpMixTransSepTransCommandPanel.TipRackSpecified = false;
            this.myTopUpMixTransSepTransCommandPanel.TipTubeBottomGap = 0;
            this.myTopUpMixTransSepTransCommandPanel.UseBufferTip = false;
            this.myTopUpMixTransSepTransCommandPanel.VisibleExtension = true;
            this.myTopUpMixTransSepTransCommandPanel.VolumeTypeSpecifier = Tesla.Common.ProtocolCommand.VolumeType.NotSpecified;
            this.myTopUpMixTransSepTransCommandPanel.VolumeTypeSpecifier2 = Tesla.Common.ProtocolCommand.VolumeType.NotSpecified;
            this.myTopUpMixTransSepTransCommandPanel.VolumeTypeSpecifier3 = Tesla.Common.ProtocolCommand.VolumeType.NotSpecified;
            this.myTopUpMixTransSepTransCommandPanel.VolumeTypeSpecifierMix = Tesla.Common.ProtocolCommand.VolumeType.NotSpecified;
            this.myTopUpMixTransSepTransCommandPanel.WaitCommandTimeDuration = ((uint)(0u));
            // 
            // FrmProtocolDetails
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(0, 326);
            this.ClientSize = new System.Drawing.Size(532, 514);
            this.Controls.Add(this.pnlCommandList);
            this.Controls.Add(this.pnlHeaderInfo);
            this.Controls.Add(this.pnlCommandDetail);
            this.Enabled = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmProtocolDetails";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FrmProtocolDetails_Load);
            this.EnabledChanged += new System.EventHandler(this.FrmProtocolDetails_EnabledChanged);
            this.pnlCommandList.ResumeLayout(false);
            this.pnlHeaderInfo.ResumeLayout(false);
            this.pnlHeaderInfo.PerformLayout();
            this.pnlCommandDetail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProtocolLabel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProtocolAuthor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorWorkingVolumeThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorSampleVolumeMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorSampleVolumeMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorCommandSequence)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorWorkingVolumeLow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorWorkingVolumeHigh)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        private void btnConfirmCommand_Click(object sender, System.EventArgs e)
		{
			//btnValidateProtocol_Click(sender, e);
			bool isCurrentCommandValid;

            // 2011-11-10 sp -- added support for volume checkinh; reset volume status
            myCommandSequence[myCurrentCommandIndex].CommandCheckStatus = VolumeCheck.VOLUMES_VALID;
            myCommandSequence[myCurrentCommandIndex].CommandStatus = "";
            ResetTubeVolumes();

			// Save changes to the current command if they haven't already been saved.
			if (isCurrentCommandDirty || isAddingInProgress)
			{
				isCurrentCommandValid = ValidateCurrentCommand();
				if (isCurrentCommandValid)
				{
					// Since we're about to move places in the command list,
					// persist changes from the current panel back to the sequence.
					PersistCommandDetailView();
					isCurrentCommandDirty = false;

					DbgView("btnConfirmCmdclk - SetAddingMode");
					// Re-enable Add/MoveUp/MoveDown now the current command is validated.
					SetAddingMode(false);
				}
                // 2011-11-10 sp -- added settings to act on warnings
                else 
				{
                    myCommandSequence[myCurrentCommandIndex].CommandCheckStatus = VolumeCheck.VOLUMES_WARNINGS;
                    myCommandSequence[myCurrentCommandIndex].CommandStatus = "invalid volumes";
                    return;
				}
			}
            // Adjust quadrant usage, if required, to ensure the definition uses contiguous
            // quadrants.
            DbgView("btnConfirmCmdclk - Adjustquads");
            myQuadrantCount = AdjustAndCountUsedQuadrants();

            // Validate tip rack choice - may be auto corrected
            isCurrentCommandValid = ValidateCurrentCommand() && ValidateCurrentTipRackQuadrant();

            if ((!isFormDirty) || isCurrentCommandValid)
            {
                DbgView("btnConfirmCmdclk - exitedit");
                ExitEditMode();
            }
            if (isCurrentCommandValid)
            {
                DbgView("btnConfirmCmdclk - setaddmode2");
                SetAddingMode(false);
            }
            //4.5.X - 2.4.3
            //Disable working vol if there is atleast 1 topup/resuspend and
            // they are all relative
            CheckCmdSeqForWorkingVolumeUse();

            // Error will show automatically, so do nothing.
            DbgView("btnConfirmCmdclk - calcvolhigh call");

            // added 2011-09-06 sp
            // call routine to check for volumes (allow the same routine to be called from elsewhere
            CheckCommandVolumeLevels(myCurrentCommandPanel);

        }

        private void ResetTubeVolumes()
        {
            Array.Clear(WorkingSampleMinAddition, 0, WorkingSampleMinAddition.Length);
            Array.Clear(WorkingSampleMaxAddition, 0, WorkingSampleMaxAddition.Length);
            Array.Clear(WorkingSeparationMinAddition, 0, WorkingSeparationMinAddition.Length);
            Array.Clear(WorkingSeparationMaxAddition, 0, WorkingSeparationMaxAddition.Length);
            Array.Clear(maxCocktailNeeded, 0, maxCocktailNeeded.Length);
            Array.Clear(maxParticlesNeeded, 0, maxParticlesNeeded.Length);
            Array.Clear(maxAntibodyNeeded, 0, maxAntibodyNeeded.Length);
            Array.Clear(maxNegFractionVolume, 0, maxNegFractionVolume.Length);
            Array.Clear(maxWasteVolume, 0, maxWasteVolume.Length);

        }
        
        // 2011-09-09 sp
        // added routine to check volume levels for high or below recommended levels
        public bool CheckCommandVolumeLevels(CommandPanel currentPanel)
        {
            bool acceptableTransportVolume = true;
            ProtocolReagentVolumeCheck volcheck = ProtocolReagentVolumeCheck.GetInstance();

            feature featureCustomVials = theProtocolModel.FindFeature("RSSCustVial");
            if (featureCustomVials != null)
            {
                List<string> ArgumentData = new List<string>(featureCustomVials.inputData.Split(';'));
                int customVialVolume = 1400;
                if (ArgumentData.Count > 0)
                {
                    int.TryParse(ArgumentData[0], out customVialVolume);
                }
                //MUST SET volumeLimits.customVialVolume BEFORE ACTIVATE
                volumeLimits.setFeatureCustomVolumeVials(true, customVialVolume);
            }
            else
            {
                volumeLimits.UpateLiquidVolumeLimits();
            }

            myCommandSequence[myCurrentCommandIndex].CommandCheckStatus = VolumeCheck.VOLUMES_VALID;
            myCommandSequence[myCurrentCommandIndex].CommandStatus = "";
            /*
            if (CalculatedVolumeBelowRecommended())
            {
                myCommandSequence[myCurrentCommandIndex].CommandCheckStatus = VolumeCheck.VOLUMES_WARNINGS;
                if (myWorkingVolumeCommandPanel.Visible)
                    myCommandSequence[myCurrentCommandIndex].CommandStatus = "volume < recommended " + volumeLimits.minimumRecommended_ReagentTipVolume_ul.ToString() +
                    "(" + volumeLimits.minimumRecommended_SampleTipVolume_ul.ToString() + ") uL";
                else if (myVolumeCommandPanel.Visible)
                    myCommandSequence[myCurrentCommandIndex].CommandStatus = "volume < recommended " + volumeLimits.minimumRecommended_mixVolume_ul.ToString() + " uL";
            }
            if (CalculatedVolumeAboveRecommended())
            {
                myCommandSequence[myCurrentCommandIndex].CommandCheckStatus = VolumeCheck.VOLUMES_WARNINGS;
                if (myVolumeCommandPanel.Visible)
                    myCommandSequence[myCurrentCommandIndex].CommandStatus = "volume > recommended " + volumeLimits.maximumRecommended_mixVolume_ul.ToString() + " uL";
            }

            if (CalculatedVolumeTooHigh())
            {
                myCommandSequence[myCurrentCommandIndex].CommandCheckStatus = VolumeCheck.VOLUMES_INVALID;
                myCommandSequence[myCurrentCommandIndex].CommandStatus = "volume exceeded " + volumeLimits.maximumAllowable_ReagentTipVolume_ul.ToString() +
                    "(" + volumeLimits.maximumAllowable_TubeVolume_ul.ToString() + ") uL";
                acceptableTransportVolume = false;
            }

            acceptableTransportVolume = acceptableTransportVolume && !CheckForVolumeTooLow(myCommandSequence[myCurrentCommandIndex]);
            */
            VolumeError err = myCommandSequence[myCurrentCommandIndex].validateCommandAndUpdateCommandStatus(volumeLimits,mySampleVolumeMin_uL,mySampleVolumeMax_uL);

            string selectedSrc, selectedDst;
            VolumeCommandPanel volPanel = currentPanel as VolumeCommandPanel;
            if (err != VolumeError.NO_ERROR && volPanel!=null)
            {
                volPanel.ShowwVolumeError(err);
            }
            else if (myCommandSequence[myCurrentCommandIndex].CommandCheckStatus == VolumeCheck.VOLUMES_VALID)
            {
                if ((SubCommand)myCommandSequence[myCurrentCommandIndex].CommandSubtype == SubCommand.TransportCommand)
                {
                    selectedSrc = (string)myWorkingVolumeCommandPanel.cmbSourceVial.SelectedItem;
                    selectedDst = (string)myWorkingVolumeCommandPanel.cmbDestinationVial.SelectedItem;
                    acceptableTransportVolume = CheckCommandVolumeLevels_helper(acceptableTransportVolume, selectedSrc, selectedDst);
                }
                else if ((SubCommand)myCommandSequence[myCurrentCommandIndex].CommandSubtype == SubCommand.TopUpMixTransSepTransCommand ||
                    (SubCommand)myCommandSequence[myCurrentCommandIndex].CommandSubtype == SubCommand.TopUpTransSepTransCommand)
                {
                    //trans1 trans2
                    GenericMultiStepCommandPanel panel = myTopUpMixTransSepTransCommandPanel;
                    if((SubCommand)myCommandSequence[myCurrentCommandIndex].CommandSubtype == SubCommand.TopUpTransSepTransCommand)
                        panel = myTopUpTransSepTransCommandPanel;

                    selectedSrc = (string)panel.cmbSourceVial2.SelectedItem;
                    selectedDst = (string)panel.cmbDestinationVial2.SelectedItem;
                    acceptableTransportVolume = CheckCommandVolumeLevels_helper(acceptableTransportVolume, selectedSrc, selectedDst);

                    selectedSrc = (string)panel.cmbSourceVial3.SelectedItem;
                    selectedDst = (string)panel.cmbDestinationVial3.SelectedItem;
                    acceptableTransportVolume = CheckCommandVolumeLevels_helper(acceptableTransportVolume, selectedSrc, selectedDst);
                }
                else if ((SubCommand)myCommandSequence[myCurrentCommandIndex].CommandSubtype == SubCommand.TopUpMixTransCommand ||
                    (SubCommand)myCommandSequence[myCurrentCommandIndex].CommandSubtype == SubCommand.TopUpTransCommand)
                {
                    //trans1
                    GenericMultiStepCommandPanel panel = myTopUpMixTransCommandPanel;
                    if ((SubCommand)myCommandSequence[myCurrentCommandIndex].CommandSubtype == SubCommand.TopUpTransCommand)
                        panel = myTopUpTransCommandPanel;
                    selectedSrc = (string)panel.cmbSourceVial2.SelectedItem;
                    selectedDst = (string)panel.cmbDestinationVial2.SelectedItem;
                    acceptableTransportVolume = CheckCommandVolumeLevels_helper(acceptableTransportVolume, selectedSrc, selectedDst);
                }
                else if ((SubCommand)myCommandSequence[myCurrentCommandIndex].CommandSubtype == SubCommand.ResusMixCommand)
                {
                    //no op for this case, put here for completeness
                }
                else if ((SubCommand)myCommandSequence[myCurrentCommandIndex].CommandSubtype == SubCommand.ResusMixSepTransCommand)
                { 
                    //trans1
                    GenericMultiStepCommandPanel panel = myResusMixSepTransCommandPanel;
                    selectedSrc = (string)panel.cmbSourceVial2.SelectedItem;
                    selectedDst = (string)panel.cmbDestinationVial2.SelectedItem;
                    acceptableTransportVolume = CheckCommandVolumeLevels_helper(acceptableTransportVolume, selectedSrc, selectedDst);
                
                }
                else if ((SubCommand)myCommandSequence[myCurrentCommandIndex].CommandSubtype == SubCommand.MixTransCommand)
                {
                    //trans1
                    GenericMultiStepCommandPanel2 panel = myMixTransCommandPanel;
                    selectedSrc = (string)panel.cmbSourceVial2.SelectedItem;
                    selectedDst = (string)panel.cmbDestinationVial2.SelectedItem;
                    acceptableTransportVolume = CheckCommandVolumeLevels_helper(acceptableTransportVolume, selectedSrc, selectedDst);
                }
            }



            DbgView("btnConfirmCmdclk - calcvolhigh DONE");
            // refresh list after checks
            pnlCommandList.Refresh();

            return acceptableTransportVolume;
        }
        /*
        private bool CheckForVolumeTooLow(IProtocolCommand iProtocolCommand)
        {
            bool CalculatedVolumeBelowFlag = false;
            string tSrc = "", tDst = "", tType = "";
            int absVolume = 0;
            double relProportion = 0;
            bool absVolumeSpecified = false,
                relProportionSpecified = false;

           
            if (CalculatedVolumeTooLow())
            {
                myCommandSequence[myCurrentCommandIndex].CommandCheckStatus = VolumeCheck.VOLUMES_INVALID;
                myCommandSequence[myCurrentCommandIndex].CommandStatus = "volume below " + minimumAllowable_ReagentTipVolume_uL.ToString() +
                    "(" + minimumAllowable_SampleTipVolume_uL.ToString() + ") uL";
                acceptableTransportVolume = false;
            }
             

            if (myWorkingVolumeCommandPanel.Visible)
            {
                tSrc = myWorkingVolumeCommandPanel.SourceVial.ToString();// get source
                tDst = myWorkingVolumeCommandPanel.DestinationVial.ToString(); // get destination
                absVolume = myWorkingVolumeCommandPanel.AbsoluteVolume_uL; // get absolute volume
                relProportion = myWorkingVolumeCommandPanel.RelativeVolumeProportion; // get relativeVolume
                absVolumeSpecified = myWorkingVolumeCommandPanel.cbAbsoluteSpecified.Checked;
                relProportionSpecified = myWorkingVolumeCommandPanel.cbRelativeSpecified.Checked;
                tType = myWorkingVolumeCommandPanel.CommandType.ToString(); 
                return CalculatedVolumeTooLow_helper(tSrc, tDst, (tType == "Transport"), absVolume, relProportion, absVolumeSpecified, relProportionSpecified);
        
            }
            else if (myVolumeCommandPanel.Visible)
            {
                tSrc = myVolumeCommandPanel.SourceVial.ToString();// get source
                tDst = myVolumeCommandPanel.DestinationVial.ToString(); // get destination
                absVolume = myVolumeCommandPanel.AbsoluteVolume_uL; // get absolute volume
                relProportion = myVolumeCommandPanel.RelativeVolumeProportion; // get relativeVolume
                absVolumeSpecified = myVolumeCommandPanel.cbAbsoluteSpecified.Checked;
                relProportionSpecified = myVolumeCommandPanel.cbRelativeSpecified.Checked;
                tType = myVolumeCommandPanel.CommandType.ToString(); 
                return CalculatedVolumeTooLow_helper(tSrc, tDst, (tType == "Transport"), absVolume, relProportion, absVolumeSpecified, relProportionSpecified);
        
            }
            else if (myTopUpMixTransSepTransCommandPanel.Visible)
            {
                tType = myTopUpMixTransSepTransCommandPanel.CommandType.ToString(); //todo: need to distinguish type later


                //mix (dest of topup
                tSrc = myTopUpMixTransSepTransCommandPanel.DestinationVial.ToString();
                absVolume = myTopUpMixTransSepTransCommandPanel.AbsoluteVolume_uLMix;
                relProportion = myTopUpMixTransSepTransCommandPanel.RelativeVolumeProportionMix;
                absVolumeSpecified = myTopUpMixTransSepTransCommandPanel.cbAbsoluteSpecifiedMix.Checked;
                relProportionSpecified = myTopUpMixTransSepTransCommandPanel.cbRelativeSpecifiedMix.Checked;
                CalculatedVolumeBelowFlag = CalculatedVolumeBelowFlag | CalculatedVolumeTooLow_helper(tSrc, tSrc,false, absVolume, relProportion,
                                                                                    absVolumeSpecified, relProportionSpecified);
                //trans1
                tSrc = myTopUpMixTransSepTransCommandPanel.SourceVial2.ToString();
                tDst = myTopUpMixTransSepTransCommandPanel.SourceVial3.ToString();
                absVolume = myTopUpMixTransSepTransCommandPanel.AbsoluteVolume_uL2;
                relProportion = myTopUpMixTransSepTransCommandPanel.RelativeVolumeProportion2;
                absVolumeSpecified = myTopUpMixTransSepTransCommandPanel.cbAbsoluteSpecified2.Checked;
                relProportionSpecified = myTopUpMixTransSepTransCommandPanel.cbRelativeSpecified2.Checked;
                CalculatedVolumeBelowFlag = CalculatedVolumeBelowFlag | CalculatedVolumeTooLow_helper(tSrc, tDst, true, absVolume, relProportion,
                                                                                    absVolumeSpecified, relProportionSpecified);
                //trans2
                tSrc = myTopUpMixTransSepTransCommandPanel.SourceVial3.ToString();
                tDst = myTopUpMixTransSepTransCommandPanel.DestinationVial3.ToString();
                absVolume = myTopUpMixTransSepTransCommandPanel.AbsoluteVolume_uL3;
                relProportion = myTopUpMixTransSepTransCommandPanel.RelativeVolumeProportion3;
                absVolumeSpecified = myTopUpMixTransSepTransCommandPanel.cbAbsoluteSpecified3.Checked;
                relProportionSpecified = myTopUpMixTransSepTransCommandPanel.cbRelativeSpecified3.Checked;
                CalculatedVolumeBelowFlag = CalculatedVolumeBelowFlag | CalculatedVolumeTooLow_helper(tSrc, tDst, true, absVolume, relProportion,
                                                                                    absVolumeSpecified, relProportionSpecified);
                return CalculatedVolumeBelowFlag;
            }
            else
            {
                // only care if about transport or mix
                return false;
            }
        }
        */
        private bool CheckCommandVolumeLevels_helper(bool acceptableTransportVolume, string selectedSrc, string selectedDst)
        {
            int quadrantIndex = 0;
            double maxSampleVolume = 0;
            double maxSeparationVolume = 0;
            try
            {
                quadrantIndex = (int)Convert.ToDecimal(selectedSrc.Substring(1, 1)) - 1;
                if (selectedSrc.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.SampleTube)) != -1)
                {
                    maxSampleVolume = WorkingSampleMaxAddition[quadrantIndex];
                }
                else if (selectedSrc.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.SeparationTube)) != -1)
                {
                    maxSeparationVolume = WorkingSeparationMaxAddition[quadrantIndex];
                }
                else if (selectedSrc.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.VialB)) != -1 ||
                    selectedSrc.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.VialBpos)) != -1 ||
                    selectedSrc.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.VialBneg)) != -1)
                {
                    AbsoluteResourceLocation SourceVial = AbsoluteResourceLocation.TPC0103;
                    int acceptableVolSrc = volumeLimits.getCapacity(SourceVial) - volumeLimits.getDeadVolume(SourceVial);
                    if (maxCocktailNeeded[quadrantIndex] > acceptableVolSrc)
                    {
                        myCommandSequence[myCurrentCommandIndex].CommandCheckStatus = VolumeCheck.VOLUMES_INVALID;
                        myCommandSequence[myCurrentCommandIndex].CommandStatus = "short " + (maxCocktailNeeded[quadrantIndex] - acceptableVolSrc)
                                                            + "uL from Q" + (quadrantIndex + 1) + " cocktail vial";
                        acceptableTransportVolume = false;
                    }
                }
                else if (selectedSrc.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.VialA)) != -1)
                {
                    AbsoluteResourceLocation SourceVial = AbsoluteResourceLocation.TPC0104;
                    int acceptableVolSrc = volumeLimits.getCapacity(SourceVial) - volumeLimits.getDeadVolume(SourceVial);
                    if (maxParticlesNeeded[quadrantIndex] > acceptableVolSrc)
                    {
                        myCommandSequence[myCurrentCommandIndex].CommandCheckStatus = VolumeCheck.VOLUMES_INVALID;
                        myCommandSequence[myCurrentCommandIndex].CommandStatus = "short " + (maxParticlesNeeded[quadrantIndex] - acceptableVolSrc)
                                                            + "uL from Q" + (quadrantIndex + 1) + " particle vial";
                        acceptableTransportVolume = false;
                    }
                }
                else if (selectedSrc.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.VialC)) != -1)
                {
                    AbsoluteResourceLocation SourceVial = AbsoluteResourceLocation.TPC0105;
                    int acceptableVolSrc = volumeLimits.getCapacity(SourceVial) - volumeLimits.getDeadVolume(SourceVial);
                    if (maxAntibodyNeeded[quadrantIndex] > acceptableVolSrc)
                    {
                        myCommandSequence[myCurrentCommandIndex].CommandCheckStatus = VolumeCheck.VOLUMES_INVALID;
                        myCommandSequence[myCurrentCommandIndex].CommandStatus = "short " + (maxAntibodyNeeded[quadrantIndex] - acceptableVolSrc)
                                                            + "uL from Q" + (quadrantIndex + 1) + " antibody vial";
                        acceptableTransportVolume = false;
                    }
                }
            }
            catch (FormatException) { }

            try
            {
                quadrantIndex = (int)Convert.ToDecimal(selectedDst.Substring(1, 1)) - 1;
                if (selectedDst.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.SampleTube)) != -1)
                {
                    maxSampleVolume = WorkingSampleMaxAddition[quadrantIndex];
                }
                else if (selectedDst.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.SeparationTube)) != -1)
                {
                    maxSeparationVolume = WorkingSeparationMaxAddition[quadrantIndex];
                }
                else if (selectedDst.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.NegativeFractionTube)) != -1)
                {
                    if (maxNegFractionVolume[quadrantIndex] > volumeLimits.maximumCapacity_NegFractionTube_ul)
                    {
                        myCommandSequence[myCurrentCommandIndex].CommandCheckStatus = VolumeCheck.VOLUMES_INVALID;
                        myCommandSequence[myCurrentCommandIndex].CommandStatus = "excess " + (maxNegFractionVolume[quadrantIndex] - volumeLimits.maximumCapacity_NegFractionTube_ul)
                                                            + "uL in Q" + (quadrantIndex + 1) + " negative fraction tube";
                        acceptableTransportVolume = false;
                    }
                }
                else if (selectedDst.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.WasteTube)) != -1)
                {
                    if (maxWasteVolume[quadrantIndex] > volumeLimits.maximumCapacity_WasteTube_ul)
                    {
                        myCommandSequence[myCurrentCommandIndex].CommandCheckStatus = VolumeCheck.VOLUMES_INVALID;
                        myCommandSequence[myCurrentCommandIndex].CommandStatus = "excess " + (maxWasteVolume[quadrantIndex] - volumeLimits.maximumCapacity_WasteTube_ul)
                                                            + " uL in Q" + (quadrantIndex + 1) + " waste tube";
                        acceptableTransportVolume = false;
                    }
                }
            }
            catch (FormatException) { }


            if (maxSampleVolume > volumeLimits.maximumCapacity_SampleTube_uL)
            {
                myCommandSequence[myCurrentCommandIndex].CommandCheckStatus = VolumeCheck.VOLUMES_INVALID;
                myCommandSequence[myCurrentCommandIndex].CommandStatus = "excess " + (maxSampleVolume - volumeLimits.maximumCapacity_SampleTube_uL) + "uL in sample";
            }
            else if (maxSeparationVolume > volumeLimits.maximumCapacity_SeparationTube_uL)
            {
                myCommandSequence[myCurrentCommandIndex].CommandCheckStatus = VolumeCheck.VOLUMES_INVALID;
                myCommandSequence[myCurrentCommandIndex].CommandStatus = "excess " + (maxSeparationVolume - volumeLimits.maximumCapacity_SeparationTube_uL) + "uL in separation";
            }
            // 2011-11-17 sp -- disable checking of transport higher than specified in tube
            //else if (maxSampleVolume < 0)
            //{
            //    if( myCommandSequence[myCurrentCommandIndex].CommandCheckStatus != VolumeCheck.VOLUMES_INVALID )
            //    {
            //        myCommandSequence[myCurrentCommandIndex].CommandCheckStatus = VolumeCheck.VOLUMES_INFO;
            //        myCommandSequence[myCurrentCommandIndex].CommandStatus = "short " + -maxSampleVolume + "uL in sample";
            //    }
            //}
            //else if (maxSeparationVolume < 0)
            //{
            //    if (myCommandSequence[myCurrentCommandIndex].CommandCheckStatus != VolumeCheck.VOLUMES_INVALID)
            //    {
            //        myCommandSequence[myCurrentCommandIndex].CommandCheckStatus = VolumeCheck.VOLUMES_INFO;
            //        myCommandSequence[myCurrentCommandIndex].CommandStatus = "short " + -maxSeparationVolume + "uL in separation";
            //    }
            //}
            return acceptableTransportVolume;
        }
        
		public void CreateCustomizeWindow()
		{
			myFrmProtocolCustomize = new ProtocolCustomize();
			myFrmProtocolCustomize.SetProtocolClass = (Tesla.Common.Protocol.ProtocolClass)(cmbProtocolType.SelectedIndex);

			DbgView("CreateCustomWindow - setup custom names");

			myFrmProtocolCustomize.SetupCustomNames(3);
			myFrmProtocolCustomize.SetupCustomNames(2);
			myFrmProtocolCustomize.SetupCustomNames(1);
			myFrmProtocolCustomize.SetupCustomNames(0);
			myFrmProtocolCustomize.SetupListBox();

			myQuadrantCount = AdjustAndCountUsedQuadrants();
			myFrmProtocolCustomize.SetEnabledTotalQuad(myQuadrantCount);

			myFrmProtocolCustomize.ShowDialog();
		}

        public void CreateReagentBarcodesWindow()
        {
            myFrmProtocolReagentBarcodes = new ProtocolReagentBarcodes();
            myFrmProtocolReagentBarcodes.SetProtocolClass = (Tesla.Common.Protocol.ProtocolClass)(cmbProtocolType.SelectedIndex);
            
            DbgView("CreateReagentBarcodeWindow - setup barcode");
            List<AbsoluteResourceLocation> lstUsed = theProtocolModel.GetListOfReagentVialsUsedInProtocol();

            theProtocolModel.NormalizeVialBarcodes();

            myFrmProtocolReagentBarcodes.SetUsedList(lstUsed);
            myFrmProtocolReagentBarcodes.SetupVialBarcodes(3);
            myFrmProtocolReagentBarcodes.SetupVialBarcodes(2);
            myFrmProtocolReagentBarcodes.SetupVialBarcodes(1);
            myFrmProtocolReagentBarcodes.SetupVialBarcodes(0);
            myFrmProtocolReagentBarcodes.SetupListBox();

            myQuadrantCount = AdjustAndCountUsedQuadrants();
            myFrmProtocolReagentBarcodes.SetEnabledTotalQuad(myQuadrantCount);

            myFrmProtocolReagentBarcodes.ShowDialog();
        }

		private void pnlCommandList_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
		}

		private void pnlHeaderInfo_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
		}


		     
		private void btnCopy_Click(object sender, System.EventArgs e)
		{
			copyFlag = true;
			// if no items, cannot use copy utility.  - Also, this prevents double-click during adding process
			if(myCommandSequence.Length == 0 || myCurrentCommandIndex < 0 || !btnCopy.Enabled)
			{
				copyFlag = false;
				return;
			}
			btnAddCommand_Click(sender, e);
		}

		#region CommandChecking

		private bool CheckAllCommands()
		{
			// save current index
			int currentPlaceHolder = myCurrentCommandIndex;
			bool isValid = true;
			double lastRelativeTopUpProportion = 0;
			isEditModeEnabled=false;
			if(myCommandSequence.Length > 0) 
			{
				
				// move through entire sequence looking for errors
				for(int i = 0; i < myCommandSequence.Length; i++) 
				{
					myCurrentCommandIndex=i;
                    // 2011-11-10 sp -- modified to fix bug for incorrectly updating the first entry transports 
                    //          when myCurrentCommandIndex is tranporting at threshold levels
                    UpdateCommandDetailViewEx(false);
                    //UpdateCommandDetailViewEx(true);

					if((SubCommand)myCommandSequence[i].CommandSubtype == 
						SubCommand.TransportCommand)
					{
                        //isValid = !CalculatedVolumeTooHigh() && myWorkingVolumeCommandPanel.IsContentValid();
                        isValid =  myWorkingVolumeCommandPanel.IsContentValid();
					}
					else if((SubCommand)myCommandSequence[i].CommandSubtype == 
						SubCommand.TopUpVialCommand)
					{
						ProtocolTopUpVialCommand topUpVialCommand =
							(ProtocolTopUpVialCommand)myCommandSequence[i];
						if (topUpVialCommand.VolumeTypeSpecifier == VolumeType.Relative)
						{
							lastRelativeTopUpProportion = topUpVialCommand.RelativeVolumeProportion;
                            // 2011-11-10 sp -- moved checking; no need to check threshold absolute values
                            isValid = myWorkingVolumeCommandPanel.IsContentValid();
						}
                        //isValid=myWorkingVolumeCommandPanel.IsContentValid();
					}
					else if((SubCommand)myCommandSequence[i].CommandSubtype == 
						SubCommand.ResuspendVialCommand)
					{
						ProtocolResuspendVialCommand resuspendVialCommand =
							(ProtocolResuspendVialCommand)myCommandSequence[i];
						if (resuspendVialCommand.VolumeTypeSpecifier == VolumeType.Relative)
						{
							resuspendVialCommand.RelativeVolumeProportion = lastRelativeTopUpProportion;
                            // 2011-11-10 sp -- moved checking; no need to check threshold absolute values
                            isValid = myWorkingVolumeCommandPanel.IsContentValid();
                        }
                        //isValid=myWorkingVolumeCommandPanel.IsContentValid();
					}
                    else if ((SubCommand)myCommandSequence[i].CommandSubtype ==
                        SubCommand.TopUpMixTransSepTransCommand)
                    {
                        ProtocolTopUpMixTransSepTransCommand cmd = (ProtocolTopUpMixTransSepTransCommand)myCommandSequence[i];
                        GenericMultiStepCommandPanel panel = myTopUpMixTransSepTransCommandPanel;
                        if (cmd.VolumeTypeSpecifier == VolumeType.Relative)
                        {
                            lastRelativeTopUpProportion = cmd.RelativeVolumeProportion;
                            //no need to check threshold absolute values
                            isValid = panel.IsContentValid();
                        }
                    }
                    else if ((SubCommand)myCommandSequence[i].CommandSubtype ==
                        SubCommand.TopUpMixTransCommand)
                    {
                        ProtocolTopUpMixTransCommand cmd = (ProtocolTopUpMixTransCommand)myCommandSequence[i];
                        GenericMultiStepCommandPanel panel = myTopUpMixTransCommandPanel;
                        if (cmd.VolumeTypeSpecifier == VolumeType.Relative)
                        {
                            lastRelativeTopUpProportion = cmd.RelativeVolumeProportion;
                            //no need to check threshold absolute values
                            isValid = panel.IsContentValid();
                        }
                    }
                    else if ((SubCommand)myCommandSequence[i].CommandSubtype ==
                        SubCommand.TopUpTransSepTransCommand)
                    {
                        ProtocolTopUpTransSepTransCommand cmd = (ProtocolTopUpTransSepTransCommand)myCommandSequence[i];
                        GenericMultiStepCommandPanel panel = myTopUpTransSepTransCommandPanel;
                        if (cmd.VolumeTypeSpecifier == VolumeType.Relative)
                        {
                            lastRelativeTopUpProportion = cmd.RelativeVolumeProportion;
                            //no need to check threshold absolute values
                            isValid = panel.IsContentValid();
                        }
                    }
                    else if ((SubCommand)myCommandSequence[i].CommandSubtype ==
                        SubCommand.TopUpTransCommand)
                    {
                        ProtocolTopUpTransCommand cmd = (ProtocolTopUpTransCommand)myCommandSequence[i];
                        GenericMultiStepCommandPanel panel = myTopUpTransCommandPanel;
                        if (cmd.VolumeTypeSpecifier == VolumeType.Relative)
                        {
                            lastRelativeTopUpProportion = cmd.RelativeVolumeProportion;
                            //no need to check threshold absolute values
                            isValid = panel.IsContentValid();
                        }
                    }
                    else if ((SubCommand)myCommandSequence[i].CommandSubtype ==
                        SubCommand.ResusMixSepTransCommand)
                    {
                        ProtocolResusMixSepTransCommand cmd =
                            (ProtocolResusMixSepTransCommand)myCommandSequence[i];
                        GenericMultiStepCommandPanel panel = myResusMixSepTransCommandPanel;
                        if (cmd.VolumeTypeSpecifier == VolumeType.Relative)
                        {
                            cmd.RelativeVolumeProportion = lastRelativeTopUpProportion;
                            //no need to check threshold absolute values
                            isValid = panel.IsContentValid();
                        }
                    }
                    else if ((SubCommand)myCommandSequence[i].CommandSubtype ==
                        SubCommand.ResusMixCommand)
                    {
                        ProtocolResusMixCommand cmd =
                            (ProtocolResusMixCommand)myCommandSequence[i];
                        GenericMultiStepCommandPanel panel = myResusMixCommandPanel;
                        if (cmd.VolumeTypeSpecifier == VolumeType.Relative)
                        {
                            cmd.RelativeVolumeProportion = lastRelativeTopUpProportion;
                            //no need to check threshold absolute values
                            isValid = panel.IsContentValid();
                        }
                    }
                    else if ((SubCommand)myCommandSequence[i].CommandSubtype ==
                        SubCommand.MixTransCommand)
                    {
                        GenericMultiStepCommandPanel2 panel = myMixTransCommandPanel;
                        //isValid = !CalculatedVolumeTooHigh() && panel.IsContentValid();
                        isValid = panel.IsContentValid();
                    }

					if(!isValid)
					{
						lstCommands.SelectedIndex = myCurrentCommandIndex;
						lstCommands.Invalidate();
						isEditModeEnabled=true;
						return isValid;
					}
				}
				// restore current index
				myCurrentCommandIndex = currentPlaceHolder;
				UpdateCommandDetailViewEx(false);
			}
			isEditModeEnabled=true;
			return isValid;
		}


        //---------------------------------------------------------------------------- 
        // Description:
        //  Test the volume to be moved out of or into a container and determine 
        //  whether the container can hold the proposed volume
        //  Refers to enums found in Common\SeparatorTypes.cs
        //      TPC0101, //WasteVial
        //      TPC0102, //LysisVial
        //      TPC0103, //CocktailVial
        //      TPC0104, //ParticleVial
        //      TPC0105, //AntibodyVial
        //      TPC0106, //SampleVial
        //      TPC0107, //SeparationVial
        // 
        // Returns: true if volume is too high 
        //---------------------------------------------------------------------------- 
        /*
        private bool CalculatedVolumeTooHigh()
        {
            bool CalculatedVolumeTooHighFlag = false;


            string tSrc = "", tDst = "", tType = "";
            int absVolume = 0;
            double relProportion = 0;
            bool absVolumeSpecified = false,
                relProportionSpecified = false;

            if (myTopUpMixTransSepTransCommandPanel.Visible)
            {
                //check trans1
                tSrc = myTopUpMixTransSepTransCommandPanel.SourceVial2.ToString();
                tDst = myTopUpMixTransSepTransCommandPanel.DestinationVial2.ToString(); 
                absVolume = myTopUpMixTransSepTransCommandPanel.AbsoluteVolume_uL2; 
                relProportion = myTopUpMixTransSepTransCommandPanel.RelativeVolumeProportion2; 
                absVolumeSpecified = myTopUpMixTransSepTransCommandPanel.cbAbsoluteSpecified2.Checked;
                relProportionSpecified = myTopUpMixTransSepTransCommandPanel.cbRelativeSpecified2.Checked;
                CalculatedVolumeTooHigh_helper(tSrc, tDst, absVolume, relProportion, absVolumeSpecified, relProportionSpecified,
                myTopUpMixTransSepTransCommandPanel.ClearAbsoluteVolumeError2, myTopUpMixTransSepTransCommandPanel.ShowCantTransportError2,
                myTopUpMixTransSepTransCommandPanel.ClearAbsoluteVolumeError2, myTopUpMixTransSepTransCommandPanel.ShowAbsoluteVolumeError2,
                myTopUpMixTransSepTransCommandPanel.ClearRelativeVolumeProportion2Error, myTopUpMixTransSepTransCommandPanel.ShowRelativeVolumeProportion2Error);
        
                //check trans2
                tSrc = myTopUpMixTransSepTransCommandPanel.SourceVial3.ToString();// get source
                tDst = myTopUpMixTransSepTransCommandPanel.DestinationVial3.ToString(); // get destination
                absVolume = myTopUpMixTransSepTransCommandPanel.AbsoluteVolume_uL3; // get absolute volume
                relProportion = myTopUpMixTransSepTransCommandPanel.RelativeVolumeProportion3; // get relativeVolume
                absVolumeSpecified = myTopUpMixTransSepTransCommandPanel.cbAbsoluteSpecified3.Checked;
                relProportionSpecified = myTopUpMixTransSepTransCommandPanel.cbRelativeSpecified3.Checked;
                CalculatedVolumeTooHigh_helper(tSrc, tDst, absVolume, relProportion, absVolumeSpecified, relProportionSpecified,
                myTopUpMixTransSepTransCommandPanel.ClearCantTransportError3, myTopUpMixTransSepTransCommandPanel.ShowCantTransportError3,
                myTopUpMixTransSepTransCommandPanel.ClearAbsoluteVolumeError3, myTopUpMixTransSepTransCommandPanel.ShowAbsoluteVolumeError3,
                myTopUpMixTransSepTransCommandPanel.ClearRelativeVolumeProportion3Error, myTopUpMixTransSepTransCommandPanel.ShowRelativeVolumeProportion3Error);
        
            }


            if (myWorkingVolumeCommandPanel.Visible != true)
                return false;

            tSrc = myWorkingVolumeCommandPanel.SourceVial.ToString();// get source
            tDst = myWorkingVolumeCommandPanel.DestinationVial.ToString(); // get destination
            absVolume = myWorkingVolumeCommandPanel.AbsoluteVolume_uL; // get absolute volume
            relProportion = myWorkingVolumeCommandPanel.RelativeVolumeProportion; // get relativeVolume
            absVolumeSpecified = myWorkingVolumeCommandPanel.cbAbsoluteSpecified.Checked;
            relProportionSpecified = myWorkingVolumeCommandPanel.cbRelativeSpecified.Checked;
            tType = myWorkingVolumeCommandPanel.CommandType.ToString();

            // only care if command is a transport command
            if (tType != "Transport")
                return false;


            return CalculatedVolumeTooHigh_helper(tSrc, tDst, absVolume, relProportion, absVolumeSpecified, relProportionSpecified,
                myWorkingVolumeCommandPanel.ClearCantTransportError, myWorkingVolumeCommandPanel.ShowCantTransportError,
                myWorkingVolumeCommandPanel.ClearAbsoluteVolumeError,myWorkingVolumeCommandPanel.ShowAbsoluteVolumeError,
                myWorkingVolumeCommandPanel.ClearRelativeVolumeProportionError,myWorkingVolumeCommandPanel.ShowRelativeVolumeProportionError);
        }

        
        public delegate void ClearTransportError_delegate();
        public delegate void ShowTransportError_delegate();
        public delegate void ClearVolumeError_delegate();
        public delegate void ShowVolumeError_delegate(VolumeError verr);
        private bool CalculatedVolumeTooHigh_helper(string tSrc, string tDst, int absVolume, double relProportion, bool absVolumeSpecified, bool relProportionSpecified,
            ClearTransportError_delegate clearErr, ShowTransportError_delegate showErr,
            ClearVolumeError_delegate clearAbsErr, ShowVolumeError_delegate showAbsErr,
            ClearVolumeError_delegate clearRelErr, ShowVolumeError_delegate showRelErr

            )
        {
            bool CalculatedVolumeTooHighFlag = false;

            //KC - check to make sure destination vial isnt a small vial:
            if (isSmallVial(tDst))
            {
                showErr();
                return true;
            }
            else
            {
                clearErr();
            }
            //KC - end of added code

            // if at least one 1mL vial is used (id ends with 03, 04 or 05) as source or 
            // destination vial, then TOTAL volume to be transferred must be less than 
            // 1100mL...if only 5mL vials is used, TOTAL volume must be less than 5000mL

            // modify 2011-09-06 sp
            // obtain values from configuration INI file
            // int maxVolume = 14000; // should be adequate for all large vials and bottles
            int maxVolume = volumeLimits.maximumAllowable_TubeVolume_ul;

            //KC - took this out destination since I handled bad destination vials above
            // check for small vials
            if (isSmallVial(tSrc))
            //  tDst.EndsWith("03") || tDst.EndsWith("04") || tDst.EndsWith("05"))
            {
                maxVolume = volumeLimits.maximumAllowable_ReagentTipVolume_ul;
            }


            // Three cases:
            // if Absolute Volume checkbox ticked, Total Volume = Absolute Volume
            if (absVolumeSpecified)
            {
                if (absVolume > maxVolume)
                {
                    CalculatedVolumeTooHighFlag = true;
                    showAbsErr(VolumeError.ABSOLUTE_TUBE_AMOUNT);
                }
                else
                {
                    clearAbsErr();
                }
            }

                // if the Relative Proportion checkbox has been ticked, Total Volume = 
            // (Max Volume)*(Relative Proportion)
            else if (relProportionSpecified)
            {
                if ((relProportion * mySampleVolumeMax_uL) > maxVolume)
                {
                    CalculatedVolumeTooHighFlag = true;
                    showRelErr(VolumeError.ABSOLUTE_TUBE_AMOUNT);
                }
                else
                {
                    clearRelErr();
                }
            }

            // if neither box checked, Total Volume = Max Volume
            else if (mySampleVolumeMax_uL > maxVolume)
            {
                CalculatedVolumeTooHighFlag = true;
                ShowSampleVolumeMaxError(1);
            }
            else
            {
                ClearSampleVolumeMaxError();
            }
            return CalculatedVolumeTooHighFlag;
        }
        */

        public static bool isSmallVial(string s)
        { 
            return (s.EndsWith("03") || s.EndsWith("04") || s.EndsWith("05"));
        }


        //---------------------------------------------------------------------------- 
        // 2011-09-09 sp
        // added volume level check for acceptable but below recommended levels
        //
        //  Description:
        //  Test the volume to be moved out of or into a container and determine 
        //  whether the container can hold the proposed volume
        //  Refers to enums found in Common\SeparatorTypes.cs
        //      TPC0101, //WasteVial
        //      TPC0102, //LysisVial
        //      TPC0103, //CocktailVial
        //      TPC0104, //ParticleVial
        //      TPC0105, //AntibodyVial
        //      TPC0106, //SampleVial
        //      TPC0107, //SeparationVial
        // 
        // Returns: true if volume is below recommended levels 
        //---------------------------------------------------------------------------- 

        /*
        private bool CalculatedVolumeBelowRecommended()
        {
            bool CalculatedVolumeBelowFlag = false;


            string tSrc = "", tDst = "", tType = "";
            int absVolume = 0;
            double relProportion = 0;
            bool absVolumeSpecified = false,
                relProportionSpecified = false;

            if (myWorkingVolumeCommandPanel.Visible)
            {
                tSrc = myWorkingVolumeCommandPanel.SourceVial.ToString();// get source
                tDst = myWorkingVolumeCommandPanel.DestinationVial.ToString(); // get destination
                absVolume = myWorkingVolumeCommandPanel.AbsoluteVolume_uL; // get absolute volume
                relProportion = myWorkingVolumeCommandPanel.RelativeVolumeProportion; // get relativeVolume
                absVolumeSpecified = myWorkingVolumeCommandPanel.cbAbsoluteSpecified.Checked;
                relProportionSpecified = myWorkingVolumeCommandPanel.cbRelativeSpecified.Checked;
                tType = myWorkingVolumeCommandPanel.CommandType.ToString();
            }
            else if (myVolumeCommandPanel.Visible)
            {
                tSrc = myVolumeCommandPanel.SourceVial.ToString();// get source
                tDst = myVolumeCommandPanel.DestinationVial.ToString(); // get destination
                absVolume = myVolumeCommandPanel.AbsoluteVolume_uL; // get absolute volume
                relProportion = myVolumeCommandPanel.RelativeVolumeProportion; // get relativeVolume
                absVolumeSpecified = myVolumeCommandPanel.cbAbsoluteSpecified.Checked;
                relProportionSpecified = myVolumeCommandPanel.cbRelativeSpecified.Checked;
                tType = myVolumeCommandPanel.CommandType.ToString();
            }
            else if (myTopUpMixTransSepTransCommandPanel.Visible)
            {
                tType = myTopUpMixTransSepTransCommandPanel.CommandType.ToString(); //todo: need to distinguish type later


                //mix (dest of topup
                tSrc = myTopUpMixTransSepTransCommandPanel.DestinationVial.ToString();
                absVolume = myTopUpMixTransSepTransCommandPanel.AbsoluteVolume_uLMix;
                relProportion = myTopUpMixTransSepTransCommandPanel.RelativeVolumeProportionMix;
                absVolumeSpecified = myTopUpMixTransSepTransCommandPanel.cbAbsoluteSpecifiedMix.Checked;
                relProportionSpecified = myTopUpMixTransSepTransCommandPanel.cbRelativeSpecifiedMix.Checked;
                CalculatedVolumeBelowFlag = CalculatedVolumeBelowFlag | CalculatedVolumeBelowRecommended_helper(tSrc,tSrc, false, absVolume, relProportion,
                                                                                    absVolumeSpecified, relProportionSpecified);
                //trans1
                tSrc = myTopUpMixTransSepTransCommandPanel.SourceVial2.ToString();
                tDst = myTopUpMixTransSepTransCommandPanel.SourceVial3.ToString();
                absVolume = myTopUpMixTransSepTransCommandPanel.AbsoluteVolume_uL2;
                relProportion = myTopUpMixTransSepTransCommandPanel.RelativeVolumeProportion2;
                absVolumeSpecified = myTopUpMixTransSepTransCommandPanel.cbAbsoluteSpecified2.Checked;
                relProportionSpecified = myTopUpMixTransSepTransCommandPanel.cbRelativeSpecified2.Checked;
                CalculatedVolumeBelowFlag = CalculatedVolumeBelowFlag | CalculatedVolumeBelowRecommended_helper(tSrc,tDst, true, absVolume, relProportion,
                                                                                    absVolumeSpecified, relProportionSpecified);
                //trans2
                tSrc = myTopUpMixTransSepTransCommandPanel.SourceVial3.ToString();
                tDst = myTopUpMixTransSepTransCommandPanel.DestinationVial3.ToString();
                absVolume = myTopUpMixTransSepTransCommandPanel.AbsoluteVolume_uL3;
                relProportion = myTopUpMixTransSepTransCommandPanel.RelativeVolumeProportion3;
                absVolumeSpecified = myTopUpMixTransSepTransCommandPanel.cbAbsoluteSpecified3.Checked;
                relProportionSpecified = myTopUpMixTransSepTransCommandPanel.cbRelativeSpecified3.Checked;
                CalculatedVolumeBelowFlag = CalculatedVolumeBelowFlag | CalculatedVolumeBelowRecommended_helper(tSrc,tDst, true, absVolume, relProportion,
                                                                                    absVolumeSpecified, relProportionSpecified);
                return CalculatedVolumeBelowFlag;
            }
            else
            {
                return false;
            }

            // only care if command is a transport command
            if (!(tType == "Transport" || tType == "Mix"))
                return false;

            CalculatedVolumeBelowFlag = CalculatedVolumeBelowRecommended_helper(tSrc, tDst, (tType == "Transport"), absVolume, relProportion, absVolumeSpecified, relProportionSpecified);
            return CalculatedVolumeBelowFlag;
        }
         */
        /*
        private bool CalculatedVolumeTooLow_helper(string tSrc, string tDst, bool boTransport, int absVolume, double relProportion, bool absVolumeSpecified, bool relProportionSpecified)
        {
            return CalculatedVolumeBelow_helper(tSrc, tDst, boTransport, absVolume, relProportion, absVolumeSpecified, relProportionSpecified,
                volumeLimits.minimumAllowable_ReagentTipVolume_uL, volumeLimits.minimumAllowable_SampleTipVolume_uL);
        }
         * */

        /*
        private bool CalculatedVolumeBelowRecommended_helper(string tSrc, string tDst, bool boTransport, int absVolume, double relProportion, bool absVolumeSpecified, bool relProportionSpecified)
        {
            return CalculatedVolumeBelow_helper(tSrc, tDst, boTransport, absVolume, relProportion, absVolumeSpecified, relProportionSpecified,
                volumeLimits.minimumRecommended_ReagentTipVolume_ul, volumeLimits.minimumRecommended_SampleTipVolume_ul);
        }
        private bool CalculatedVolumeBelow_helper(string tSrc, string tDst, bool boTransport, int absVolume, double relProportion, bool absVolumeSpecified, bool relProportionSpecified,
            int TipVolume1ml, int TipVolume5ml)
        {
            bool CalculatedVolumeBelowFlag = false;

            // if at least one 1mL vial is used (id ends with 03, 04 or 05) as source  
            //  vial, then TOTAL volume to be transferred must be above the acceptable amounts
            int acceptableVol = volumeLimits.minimumRecommended_mixVolume_ul;

            if (boTransport)
            {
                if (isSmallVial(tSrc))
                {
                    acceptableVol = volumeLimits.minimumRecommended_ReagentTipVolume_ul;
                }
                else
                {
                    acceptableVol = volumeLimits.minimumRecommended_SampleTipVolume_ul;
                }
            }

            // Three cases:
            // if Absolute Volume checkbox ticked, Total Volume = Absolute Volume
            if (absVolumeSpecified)
            {
                if (absVolume < acceptableVol)
                {
                    CalculatedVolumeBelowFlag = true;
                }
            }

                // if the Relative Proportion checkbox has been ticked, Total Volume = 
            // (Max Volume)*(Relative Proportion)
            else if (relProportionSpecified)
            {
                if ((relProportion * mySampleVolumeMin_uL) < acceptableVol)
                {
                    CalculatedVolumeBelowFlag = true;
                }
            }

            // if neither box checked, Total Volume = Max Volume
            else if (mySampleVolumeMin_uL < acceptableVol)
            {
                CalculatedVolumeBelowFlag = true;
            }
            return CalculatedVolumeBelowFlag;
        }
        */

        //---------------------------------------------------------------------------- 
        // 2011-09-09 sp
        // added volume level check for acceptable but below recommended levels
        //
        //  Description:
        //  Test the volume to be moved out of or into a container and determine 
        //  whether the container can hold the proposed volume
        //  Refers to enums found in Common\SeparatorTypes.cs
        //      TPC0101, //WasteVial
        //      TPC0102, //LysisVial
        //      TPC0103, //CocktailVial
        //      TPC0104, //ParticleVial
        //      TPC0105, //AntibodyVial
        //      TPC0106, //SampleVial
        //      TPC0107, //SeparationVial
        // 
        // Returns: true if volume is below recommended levels 
        //---------------------------------------------------------------------------- 
        /*
        private bool CalculatedVolumeAboveRecommended()
        {
            bool CalculatedVolumeAboveFlag = false;


            string tSrc = "", tDst = "", tType = "";
            int absVolume = 0;
            double relProportion = 0;
            bool absVolumeSpecified = false,
                relProportionSpecified = false;

            if (myVolumeCommandPanel.Visible)
            {
                tSrc = myVolumeCommandPanel.SourceVial.ToString();// get source
                tDst = myVolumeCommandPanel.DestinationVial.ToString(); // get destination
                absVolume = myVolumeCommandPanel.AbsoluteVolume_uL; // get absolute volume
                relProportion = myVolumeCommandPanel.RelativeVolumeProportion; // get relativeVolume
                absVolumeSpecified = myVolumeCommandPanel.cbAbsoluteSpecified.Checked;
                relProportionSpecified = myVolumeCommandPanel.cbRelativeSpecified.Checked;
                tType = myVolumeCommandPanel.CommandType.ToString();
            }
            else if (myTopUpMixTransSepTransCommandPanel.Visible)
            {
                tType = myTopUpMixTransSepTransCommandPanel.CommandType.ToString(); //todo: need to distinguish type later

                //mix (dest of topup
                tSrc = myTopUpMixTransSepTransCommandPanel.DestinationVial.ToString();
                absVolume = myTopUpMixTransSepTransCommandPanel.AbsoluteVolume_uLMix;
                relProportion = myTopUpMixTransSepTransCommandPanel.RelativeVolumeProportionMix;
                absVolumeSpecified = myTopUpMixTransSepTransCommandPanel.cbAbsoluteSpecifiedMix.Checked;
                relProportionSpecified = myTopUpMixTransSepTransCommandPanel.cbRelativeSpecifiedMix.Checked;
            }
            else
            {
                return false;
            }

            if (tType != "Mix")
                return false;

            int acceptableVol = volumeLimits.maximumRecommended_mixVolume_ul;

            // Three cases:
            // if Absolute Volume checkbox ticked, Total Volume = Absolute Volume
            if (absVolumeSpecified)
            {
                if (absVolume > acceptableVol)
                {
                    CalculatedVolumeAboveFlag = true;
                }
            }

                // if the Relative Proportion checkbox has been ticked, Total Volume = 
            // (Max Volume)*(Relative Proportion)
            else if (relProportionSpecified)
            {
                if ((relProportion * mySampleVolumeMin_uL) > acceptableVol)
                {
                    CalculatedVolumeAboveFlag = true;
                }
            }

            // if neither box checked, Total Volume = Max Volume
            else if (mySampleVolumeMin_uL > acceptableVol)
            {
                CalculatedVolumeAboveFlag = true;
            }
            return CalculatedVolumeAboveFlag;
        }
        */
        #endregion CommandChecking


		public string getProtocolLabel()
		{
			return txtProtocolLabel.Text;
		}

		// UpdateProtocolDescription auto creates a description for the protocol
		private void UpdateProtocolDescription()
		{
			string result = GetProtocolDescriptionString();
			txtProtocolDesc.Text = result;
		}

		// only automatically build a protocol description string if certain commands 
		// using certain resources are contained in the protocol - otherwise the auto
		// descriptor is BLANK
		private string GetProtocolDescriptionString()
		{
			int i;
			string tube="", addition="", result="";
			if(myCommandSequence.Length <= 0) 
				return result;

			int [] absArray = new int[(int)ReagentVial.TOTAL]; 
			int [] relArray = new int[(int)ReagentVial.TOTAL];

			string incubateString ="";
			string seperateString ="";

			int currentPlaceHolder = myCurrentCommandIndex;

			//init arrays;
			for(i=0;i<(int)ReagentVial.TOTAL;++i)
			{
				relArray[i]=0;
				absArray[i]=0;
			}

			// move through entire sequence - add a descriptor for tranport, incubate
			// or separation commands
			for(i = 0; i < myCommandSequence.Length; ++i) 
			{
                string src;
                int absVolume;
                double relProportion;

				myCurrentCommandIndex=i;
				UpdateCommandDetailViewEx(false);
                if ((SubCommand)myCommandSequence[i].CommandSubtype ==
                    SubCommand.TransportCommand)
                {
                    src = (string)myWorkingVolumeCommandPanel.cmbSourceVial.SelectedItem;
                    absVolume = myWorkingVolumeCommandPanel.AbsoluteVolume_uL; // get absolute volume
                    relProportion = myWorkingVolumeCommandPanel.RelativeVolumeProportion; // get relativeVolume

                    GetProtocolDescriptionString_Transport_helper(absArray, relArray, src, absVolume, relProportion);
                }
                else if ((SubCommand)myCommandSequence[i].CommandSubtype ==
                    SubCommand.IncubateCommand)
                {
                    if (incubateString != "") incubateString += ", ";
                    incubateString += (Math.Round((myIncubateCommandPanel.WaitCommandTimeDuration) / 60.0));
                    //+myIncubateCommandPanel.CommandExtensionTime
                }
                else if ((SubCommand)myCommandSequence[i].CommandSubtype ==
                    SubCommand.SeparateCommand)
                {
                    if (seperateString != "") seperateString += ", ";
                    seperateString += (Math.Round((mySeparateCommandPanel.WaitCommandTimeDuration) / 60.0));
                    //+mySeparateCommandPanel.CommandExtensionTime
                }

                else if ((SubCommand)myCommandSequence[i].CommandSubtype ==
                    SubCommand.TopUpMixTransSepTransCommand)
                {
                    //trans1 trans2
                    GenericMultiStepCommandPanel panel = myTopUpMixTransSepTransCommandPanel;
                    src = (string)panel.cmbSourceVial2.SelectedItem;
                    absVolume = panel.AbsoluteVolume_uL2; // get absolute volume
                    relProportion = panel.RelativeVolumeProportion2; // get relativeVolume

                    GetProtocolDescriptionString_Transport_helper(absArray, relArray, src, absVolume, relProportion);

                    src = (string)panel.cmbSourceVial3.SelectedItem;
                    absVolume = panel.AbsoluteVolume_uL3; // get absolute volume
                    relProportion = panel.RelativeVolumeProportion3; // get relativeVolume

                    GetProtocolDescriptionString_Transport_helper(absArray, relArray, src, absVolume, relProportion);

                    //sep
                    if (seperateString != "") seperateString += ", ";
                    seperateString += (Math.Round((panel.WaitCommandTimeDuration) / 60.0));
                }
                else if ((SubCommand)myCommandSequence[i].CommandSubtype ==
                    SubCommand.TopUpMixTransCommand)
                {
                    //trans1 
                    GenericMultiStepCommandPanel panel = myTopUpMixTransCommandPanel;
                    src = (string)panel.cmbSourceVial2.SelectedItem;
                    absVolume = panel.AbsoluteVolume_uL2; // get absolute volume
                    relProportion = panel.RelativeVolumeProportion2; // get relativeVolume

                    GetProtocolDescriptionString_Transport_helper(absArray, relArray, src, absVolume, relProportion);
                }
                else if ((SubCommand)myCommandSequence[i].CommandSubtype ==
                    SubCommand.TopUpTransSepTransCommand)
                {
                    //trans1 trans2
                    GenericMultiStepCommandPanel panel = myTopUpTransSepTransCommandPanel;
                    src = (string)panel.cmbSourceVial2.SelectedItem;
                    absVolume = panel.AbsoluteVolume_uL2; // get absolute volume
                    relProportion = panel.RelativeVolumeProportion2; // get relativeVolume

                    GetProtocolDescriptionString_Transport_helper(absArray, relArray, src, absVolume, relProportion);

                    src = (string)panel.cmbSourceVial3.SelectedItem;
                    absVolume = panel.AbsoluteVolume_uL3; // get absolute volume
                    relProportion = panel.RelativeVolumeProportion3; // get relativeVolume

                    GetProtocolDescriptionString_Transport_helper(absArray, relArray, src, absVolume, relProportion);

                    //sep
                    if (seperateString != "") seperateString += ", ";
                    seperateString += (Math.Round((panel.WaitCommandTimeDuration) / 60.0));
                }
                else if ((SubCommand)myCommandSequence[i].CommandSubtype ==
                    SubCommand.TopUpTransCommand)
                {
                    //trans1 
                    GenericMultiStepCommandPanel panel = myTopUpTransCommandPanel;
                    src = (string)panel.cmbSourceVial2.SelectedItem;
                    absVolume = panel.AbsoluteVolume_uL2; // get absolute volume
                    relProportion = panel.RelativeVolumeProportion2; // get relativeVolume

                    GetProtocolDescriptionString_Transport_helper(absArray, relArray, src, absVolume, relProportion);
                }
                else if ((SubCommand)myCommandSequence[i].CommandSubtype ==
                    SubCommand.ResusMixSepTransCommand)
                {
                    //trans1
                    GenericMultiStepCommandPanel panel = myResusMixSepTransCommandPanel;
                    src = (string)panel.cmbSourceVial2.SelectedItem;
                    absVolume = panel.AbsoluteVolume_uL2; // get absolute volume
                    relProportion = panel.RelativeVolumeProportion2; // get relativeVolume

                    GetProtocolDescriptionString_Transport_helper(absArray, relArray, src, absVolume, relProportion);


                    //sep
                    if (seperateString != "") seperateString += ", ";
                    seperateString += (Math.Round((panel.WaitCommandTimeDuration) / 60.0));
                }
                else if ((SubCommand)myCommandSequence[i].CommandSubtype ==
                    SubCommand.ResusMixCommand)
                {
                    //no op here, added for completeness
                }
                else if ((SubCommand)myCommandSequence[i].CommandSubtype ==
                    SubCommand.MixTransCommand)
                {
                    //trans1
                    GenericMultiStepCommandPanel2 panel = myMixTransCommandPanel;
                    src = (string)panel.cmbSourceVial2.SelectedItem;
                    absVolume = panel.AbsoluteVolume_uL2; // get absolute volume
                    relProportion = panel.RelativeVolumeProportion2; // get relativeVolume

                    GetProtocolDescriptionString_Transport_helper(absArray, relArray, src, absVolume, relProportion);

                }
			}
			// restore current index
			myCurrentCommandIndex = currentPlaceHolder;
			UpdateCommandDetailView();

			
			result="";
			for (i = 0; i< (int)ReagentVial.TOTAL; ++i)
			{
				addition="";
				
				if(relArray[i] > 0) addition += relArray[i] + " uL/mL";
				if(absArray[i] > 0)
				{
					if (addition!="") addition+=", ";
					addition+=absArray[i]+" uL";
				}
				if (addition!="")
				{
					tube = ((ReagentVial)i).ToString();
					addition+=" " + tube.ToLower(); // +" addition(s)";
					if (result!="") result += ", ";
					result+=addition;
				}
			}
			if(incubateString!="")
			{
				if (result!="") result += ", ";
				result += incubateString + " min incubation(s)";
			}

			if(seperateString!="")
			{
				if (result!="") result += ", ";
				result+=seperateString + " min separation(s)";
			}


            //add features to description
            feature[] protocolFeature = theProtocolModel.getFeatureList();
            if (protocolFeature != null)
            {
                foreach (DataAccess.feature f in protocolFeature)
                {
                    if (result != "") result += ", ";
                    result += f.name+": "+f.inputData;
                }
            }
			return result;
		}

        private void GetProtocolDescriptionString_Transport_helper(int[] absArray, int[] relArray, string src, int absVolume, double relProportion)
        {
            ReagentVial currentVial = ReagentVial.TOTAL;
            string selected = src;

            //cocktail
            if (selected.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.VialB)) != -1 ||
                selected.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.VialBpos)) != -1 ||
                selected.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.VialBneg)) != -1)
                currentVial = ReagentVial.Cocktail;
            //particle
            else if (selected.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.VialA)) != -1)
                currentVial = ReagentVial.Particle;
            //antibody
            else if (selected.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.VialC)) != -1)
                currentVial = ReagentVial.Antibody;
            else
                return;

            if (myWorkingVolumeCommandPanel.cbRelativeSpecified.Checked)
            {
                relArray[(int)currentVial] += (int)Math.Round(relProportion * 1000);
            }
            else if (myWorkingVolumeCommandPanel.cbAbsoluteSpecified.Checked)
            {
                absArray[(int)currentVial] += absVolume;
            }
        }


		private void btnAutoFill_Click(object sender, System.EventArgs e)
		{
			btnValidateProtocol_Click(sender, e);
			if (!isCurrentCommandDirty)
			{
				UpdateProtocolDescription();
			}
		}
		
		//4.5.X - 2.4.3
		//Disable working vol if there is atleast 1 topup/resuspend and
		// they are all relative
		private void CheckCmdSeqForWorkingVolumeUse()
		{
			bool oldIsEnableEditMode = IsEditModeEnabled;
			bool atleastOne = false;
			bool absUsed = false;

			IsEditModeEnabled = false;

			// save current index
			int currentPlaceHolder = myCurrentCommandIndex;
			if(myCommandSequence.Length > 0) 
			{
				// move through entire sequence looking for topup/resuspend
				for(int i = 0; i < myCommandSequence.Length; i++) 
				{
					myCurrentCommandIndex=i;
					UpdateCommandDetailViewEx(false);

					if((SubCommand)myCommandSequence[i].CommandSubtype == 
						SubCommand.TopUpVialCommand)
					{
						atleastOne = true;
						ProtocolTopUpVialCommand topUpVialCommand =
							(ProtocolTopUpVialCommand)myCommandSequence[i];
						if (topUpVialCommand.VolumeTypeSpecifier == VolumeType.Absolute)
						{
							absUsed=true;
						}
					}
					else if((SubCommand)myCommandSequence[i].CommandSubtype == 
						SubCommand.ResuspendVialCommand)
					{
						atleastOne = true;
						ProtocolResuspendVialCommand resuspendVialCommand =
							(ProtocolResuspendVialCommand)myCommandSequence[i];
						if (resuspendVialCommand.VolumeTypeSpecifier == VolumeType.Absolute)
						{
							absUsed=true;
						}
					}
				}
				// restore current index
				myCurrentCommandIndex = currentPlaceHolder;
				UpdateCommandDetailView();
			}

			if(atleastOne && !absUsed)
			{
				ClearWorkingVolumeSampleThresholdError();
				ClearWorkingVolumeLowError();
				ClearWorkingVolumeHighError();
				txtWorkingVolumeSampleThreshold.Enabled = false;
				txtWorkingVolumeLow.Enabled = false;
				txtWorkingVolumeHigh.Enabled = false;
			}
			else
			{
				txtWorkingVolumeSampleThreshold.Enabled = true;
				txtWorkingVolumeLow.Enabled = true;
				txtWorkingVolumeHigh.Enabled = true;
				txtWorkingVolumeSampleThreshold_TextChanged(this, new System.EventArgs());
				txtWorkingVolumeLow_TextChanged(this, new System.EventArgs());
				txtWorkingVolumeHigh_TextChanged(this, new System.EventArgs());
			}

			IsEditModeEnabled = oldIsEnableEditMode;
			return;
		}

	
		public ProtocolClass GetProtocolClass
		{
			get
			{
				return (Tesla.Common.Protocol.ProtocolClass)(cmbProtocolType.SelectedIndex);
			}
		}

		private void RunValidation()		//CWJ Add
		{				
			// Save changes to the current command if they haven't already been saved.
			if (isCurrentCommandDirty || isAddingInProgress)
			{
				bool isCurrentCommandValid = ValidateCurrentCommand();
				if (isCurrentCommandValid)
				{
					// Since we're about to move places in the command list,
					// persist changes from the current panel back to the sequence.
					PersistCommandDetailView();
					isCurrentCommandDirty = false;
					// Re-enable Add/MoveUp/MoveDown now the current command is validated.
					SetAddingMode(false);
				}
			}
			//4.5.X - 2.4.3
			//Disable working vol if there is atleast 1 topup/resuspend and
			// they are all relative
			CheckCmdSeqForWorkingVolumeUse();
			
			/*CR - moved quad count to here*/
			// Adjust quadrant usage, if required, to ensure the definition uses contiguous
			// quadrants.
			myQuadrantCount = AdjustAndCountUsedQuadrants();

			// Validate user changes - was: && CheckAllCommands() && ((ProtocolClass)(cmbProtocolType.SelectedIndex)!=ProtocolClass.Undefined);            
			bool isProtocolValid = ValidateProtocol();

			theProtocolModel.ValidProtocol = isProtocolValid;

			// previous
			// if (isProtocolValid && !txtProtocolDesc.Enabled)
			//     UpdateProtocolDescription();
			if (isProtocolValid && !theProtocolModel.ProtocolDescManualFill) //txtProtocolDesc.Enabled)
				UpdateProtocolDescription();
			      
			if ((ProtocolClass)(cmbProtocolType.SelectedIndex) == ProtocolClass.Undefined)
				MessageBox.Show(this,"Please define the Protocol Type.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			
			// set wingdings tick or cross
			lblValidateResult.Text = isProtocolValid ? "þ" : "ý";

			// if validation successful, reset the 'page dirty' flag
			if (( ! isFormDirty) || isProtocolValid)
			{
				ExitEditMode();		
			}

			if (isProtocolValid)
			{
				SetAddingMode(false);
			}
		}

        private void SetRelVolume(int vial, int vol, int quadrant)  //CWJ Add   // sp added quadrant
        {
            ProtocolReagentVolumeCheck volcheck = ProtocolReagentVolumeCheck.GetInstance();//CWJ Add

            // 2011-11-03 sp -- use quadrant specified instead of using auto-increment
            volcheck.SetQuadrantIndex = quadrant;

            switch (vial)
            {
                case (int)ReagentVial.Cocktail:
                    volcheck.SetCocktailRelVol = vol;
                    break;
                case (int)ReagentVial.Particle:
                    volcheck.SetParticleRelVol = vol;
                    break;
                case (int)ReagentVial.Antibody:
                    volcheck.SetAntibodyRelVol = vol;
                    break;
            }
        }

        // 2011-09-09 sp
        // re-introduced top keep track of total voulmes
        //*
        private void SetAbsVolume(int vial, int vol, int quadrant)  //CWJ Add   // sp added quadrant
        {
            ProtocolReagentVolumeCheck volcheck = ProtocolReagentVolumeCheck.GetInstance();//CWJ Add		

            // 2011-11-03 sp -- use quadrant specified instead of using auto-increment
            volcheck.SetQuadrantIndex = quadrant;

            switch (vial)
            {
                case (int)ReagentVial.Cocktail:
                    volcheck.SetCocktailAbsVol = vol;
                    break;
                case (int)ReagentVial.Particle:
                    volcheck.SetParticleAbsVol = vol;
                    break;
                case (int)ReagentVial.Antibody:
                    volcheck.SetAntibodyAbsVol = vol;
                    break;
            }

        }

        // 2011-11-10 sp
        // added to support volume additions
        private void AddRelVolume(int vial, int vol, int quadrant)  
        {
            ProtocolReagentVolumeCheck volcheck = ProtocolReagentVolumeCheck.GetInstance();
            volcheck.SetQuadrantIndex = quadrant;

            switch (vial)
            {
                case (int)ReagentVial.Cocktail:
                    volcheck.SetCocktailRelVol += vol;
                    break;
                case (int)ReagentVial.Particle:
                    volcheck.SetParticleRelVol += vol;
                    break;
                case (int)ReagentVial.Antibody:
                    volcheck.SetAntibodyRelVol += vol;
                    break;
            }
        }

        // 2011-11-10 sp
        // added to support volume additions
        //*
        private void AddAbsVolume(int vial, int vol, int quadrant)  
        {
            ProtocolReagentVolumeCheck volcheck = ProtocolReagentVolumeCheck.GetInstance();		
            volcheck.SetQuadrantIndex = quadrant;

            switch (vial)
            {
                case (int)ReagentVial.Cocktail:
                    volcheck.SetCocktailAbsVol += vol;
                    break;
                case (int)ReagentVial.Particle:
                    volcheck.SetParticleAbsVol += vol;
                    break;
                case (int)ReagentVial.Antibody:
                    volcheck.SetAntibodyAbsVol += vol;
                    break;
            }

        }
        //*/

        private void SetMaxMinVolume(int Max, int Min)  //CWJ Add
        {
            ProtocolReagentVolumeCheck volcheck = ProtocolReagentVolumeCheck.GetInstance();//CWJ Add
            volcheck.SampMaxVol = Max;
            volcheck.SampMinVol = Min;
        }
        private void ClearVolumeCheck()  //CWJ Add
		{
			ProtocolReagentVolumeCheck volcheck = ProtocolReagentVolumeCheck.GetInstance();//CWJ Add
			volcheck.ClearAllValues();
		}


        // 2011-11-07 sp -- added selection for default end-result based on destination string
        //someone called defaultEndResult[1] = -1;    // forced to no valid tube, i.e. no tube selected
        //so makes the function useless so comment out
        /*
        private void setDefaultEndResult( bool endResultSelected, String selectedDst, int []defaultEndResult )
        {
            if (!endResultSelected)
            {
                try
                {
                    defaultEndResult[0] = (int)Convert.ToDecimal(selectedDst.Substring(1, 1)) - 1;
                }
                catch (FormatException e) { }

                if (selectedDst.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.VialA)) != -1)
                    defaultEndResult[1] = 4;
                else if (selectedDst.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.VialB)) != -1)
                    defaultEndResult[1] = 3;
                else if (selectedDst.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.VialC)) != -1)
                    defaultEndResult[1] = 5;
                else if (selectedDst.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.WasteTube)) != -1)
                    defaultEndResult[1] = 1;
                else if (selectedDst.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.NegativeFractionTube)) != -1)
                    defaultEndResult[1] = 2;
                else if (selectedDst.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.SampleTube)) != -1)
                    defaultEndResult[1] = 6;
                else if (selectedDst.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.SeparationTube)) != -1)
                    defaultEndResult[1] = 7;

                defaultEndResult[1] = -1;    // forced to no valid tube, i.e. no tube selected
            }
        }
		*/

        // 2011-09-12 sp
        // rename from CheckProtocolVolumes to more generic name
		public bool CheckProtocolCommands( bool forceDisplay )
		{
			int i;
			int result = 0;										//CWJ ADD
			ProtocolVolWarning dlg = new ProtocolVolWarning();	//CWJ ADD

			ProtocolReagentVolumeCheck volcheck = ProtocolReagentVolumeCheck.GetInstance();//CWJ Add

			int [] absArray = new int[(int)ReagentVial.TOTAL]; 
			int [] relArray = new int[(int)ReagentVial.TOTAL];
			int    absVolume = 0;
			double    relProportion = 0;

            double TotalIncubationTime = 0f;
            double TotalSeparationTime = 0f;
            double TotalExtensionTime = 0f;
            int NumberOfSeparations = 0;
            int NumberOfIncubations = 0;

            //defaultEndResult[0] = -1;
            //defaultEndResult[1] = -1;

			int currentPlaceHolder = myCurrentCommandIndex;
			
			int	SampleMinVol = mySampleVolumeMin_uL;		//CWJ ADD
			int SampleMaxVol = mySampleVolumeMax_uL;		//CWJ ADD
			SetMaxMinVolume(SampleMaxVol, SampleMinVol);	//CWJ ADD

			//init arrays;
			for(i=0;i<(int)ReagentVial.TOTAL;++i)
			{
				relArray[i]=0;
				absArray[i]=0;
			}

            // 2011-11-03 sp -- initialize maximum in sample tube volume
            ResetTubeVolumes();
            maxWasteVolume[0] = volumeLimits.initialPumpPrimeVol_WasteTube_ul;
            WorkingSampleMinAddition[0] = mySampleVolumeMin_uL;
            WorkingSampleMaxAddition[0] = mySampleVolumeMax_uL;
            bool individualCommand_OK = true;

            // 2011-11-07 sp -- added check if end-result is selected
            ProtocolModel theProtocolModel = ProtocolModel.GetInstance();
            bool endResultSelected = false;
            bool[] resultVialChecks;

            try
            {
                bool[] quadrantsUsed = theProtocolModel.GetQuadrantUse();
                for (int quadrant = 0; quadrant < quadrantsUsed.Length; quadrant++)
                {
                    if (!quadrantsUsed[quadrant]) continue;
                    theProtocolModel.GetResultVialChecks(quadrant, out resultVialChecks);
                    if (resultVialChecks != null)
                    {
                        for (int idx = 0; idx < 8; idx++)
                        {
                            endResultSelected |= resultVialChecks[idx];
                        }
                    }
                }
            }
            catch (Exception) { }

            // 2011-11-16 sp
            isEditModeEnabled = false;
            // move through entire sequence - add a descriptor for tranport, incubate
			// or separation commands
			for(i = 0; i < myCommandSequence.Length; ++i) 
			{
				myCurrentCommandIndex=i;
                // 2011-09021 sp
                // enable screen updates such that values can be obtained for volume checks without having to rewrite volume checking routine
				UpdateCommandDetailViewEx(true);
				if((SubCommand)myCommandSequence[i].CommandSubtype == 
					SubCommand.TransportCommand)
				{
                    // 2011-09-22 sp
                    // added checking for destination tube; renamed variable selected to selectedSrc
					string selectedSrc = (string)myWorkingVolumeCommandPanel.cmbSourceVial.SelectedItem;
					string selectedDst = (string)myWorkingVolumeCommandPanel.cmbDestinationVial.SelectedItem;
                    bool boRel = myWorkingVolumeCommandPanel.cbRelativeSpecified.Checked;
                    bool boAbs = myWorkingVolumeCommandPanel.cbAbsoluteSpecified.Checked;
					absVolume = myWorkingVolumeCommandPanel.AbsoluteVolume_uL; // get absolute volume
					relProportion = myWorkingVolumeCommandPanel.RelativeVolumeProportion; // get relativeVolume
                    // 2011-09-21 sp
                    // added to avoid bypass by other checks by continue statement
                    CheckProtocolCommands_Transport_helper(absArray, relArray, absVolume, relProportion,
                        endResultSelected, selectedSrc, selectedDst, boRel, boAbs);
                }
                // 2011-09-12 sp
                // added parsing of separation cycles
                else if ((SubCommand)myCommandSequence[i].CommandSubtype ==
                    SubCommand.IncubateCommand)
                {
                    NumberOfIncubations++;
                    TotalIncubationTime += (double) (Math.Round((double)(myIncubateCommandPanel.WaitCommandTimeDuration) / 60.0, 1));
                }
                // 2011-09-12 sp
                // added parsing of separation times 
                else if ((SubCommand)myCommandSequence[i].CommandSubtype ==
                    SubCommand.SeparateCommand)
                {
                    NumberOfSeparations++;
                    TotalSeparationTime += (double)(Math.Round((double)(mySeparateCommandPanel.WaitCommandTimeDuration) / 60.0, 1));
                }
                // 2011-09-22 sp
                // added sample and separation tube checking 
                else if ((SubCommand)myCommandSequence[i].CommandSubtype == SubCommand.TopUpVialCommand || 
                    (SubCommand)myCommandSequence[i].CommandSubtype == SubCommand.ResuspendVialCommand)
                {
                    string selectedSrc = (string)myWorkingVolumeCommandPanel.cmbSourceVial.SelectedItem;
                    string selectedDst = (string)myWorkingVolumeCommandPanel.cmbDestinationVial.SelectedItem;
                    bool boRel = myWorkingVolumeCommandPanel.cbRelativeSpecified.Checked;
                    bool boAbs = myWorkingVolumeCommandPanel.cbAbsoluteSpecified.Checked;
                    absVolume = myWorkingVolumeCommandPanel.AbsoluteVolume_uL; // get absolute volume
                    int absLowVolume = myWorkingVolumeCommandPanel.AbsoluteVolume_uL; 
                    relProportion = myWorkingVolumeCommandPanel.RelativeVolumeProportion; // get relativeVolume

                    CheckProtocolCommands_TopUpResus_helper(absVolume, relProportion, endResultSelected, selectedSrc, selectedDst, boRel, boAbs, ref absLowVolume);
                }

                else if ((SubCommand)myCommandSequence[i].CommandSubtype ==
                    SubCommand.TopUpMixTransSepTransCommand)
                {
                    GenericMultiStepCommandPanel panel = myTopUpMixTransSepTransCommandPanel;

                    //topup
                    string selectedSrc = (string)panel.cmbSourceVial.SelectedItem;
                    string selectedDst = (string)panel.cmbDestinationVial.SelectedItem;
                    bool boRel = panel.cbRelativeSpecified.Checked;
                    bool boAbs = panel.cbAbsoluteSpecified.Checked;
                    absVolume = panel.AbsoluteVolume_uL; // get absolute volume
                    int absLowVolume = panel.AbsoluteVolume_uL;
                    relProportion = panel.RelativeVolumeProportion; // get relativeVolume

                    CheckProtocolCommands_TopUpResus_helper(absVolume, relProportion, endResultSelected, selectedSrc, selectedDst, boRel, boAbs, ref absLowVolume);

                    //trans1
                    selectedSrc = (string)panel.cmbSourceVial2.SelectedItem;
                    selectedDst = (string)panel.cmbDestinationVial2.SelectedItem;
                    boRel = panel.cbRelativeSpecified2.Checked;
                    boAbs = panel.cbAbsoluteSpecified2.Checked;
                    absVolume = panel.AbsoluteVolume_uL2; // get absolute volume
                    relProportion = panel.RelativeVolumeProportion2; // get relativeVolume

                    CheckProtocolCommands_Transport_helper(absArray, relArray, absVolume, relProportion,
                        endResultSelected, selectedSrc, selectedDst, boRel, boAbs);

                    //sep
                    NumberOfSeparations++;
                    TotalSeparationTime += (double)(Math.Round((double)(panel.WaitCommandTimeDuration) / 60.0, 1));

                    //trans2
                    selectedSrc = (string)panel.cmbSourceVial3.SelectedItem;
                    selectedDst = (string)panel.cmbDestinationVial3.SelectedItem;
                    boRel = panel.cbRelativeSpecified3.Checked;
                    boAbs = panel.cbAbsoluteSpecified3.Checked;
                    absVolume = panel.AbsoluteVolume_uL3; // get absolute volume
                    relProportion = panel.RelativeVolumeProportion3; // get relativeVolume

                    CheckProtocolCommands_Transport_helper(absArray, relArray, absVolume, relProportion,
                        endResultSelected, selectedSrc, selectedDst, boRel, boAbs);
                }
                else if ((SubCommand)myCommandSequence[i].CommandSubtype ==
                    SubCommand.TopUpMixTransCommand)
                {
                    GenericMultiStepCommandPanel panel = myTopUpMixTransCommandPanel;

                    //topup
                    string selectedSrc = (string)panel.cmbSourceVial.SelectedItem;
                    string selectedDst = (string)panel.cmbDestinationVial.SelectedItem;
                    bool boRel = panel.cbRelativeSpecified.Checked;
                    bool boAbs = panel.cbAbsoluteSpecified.Checked;
                    absVolume = panel.AbsoluteVolume_uL; // get absolute volume
                    int absLowVolume = panel.AbsoluteVolume_uL;
                    relProportion = panel.RelativeVolumeProportion; // get relativeVolume

                    CheckProtocolCommands_TopUpResus_helper(absVolume, relProportion, endResultSelected, selectedSrc, selectedDst, boRel, boAbs, ref absLowVolume);

                    //trans1
                    selectedSrc = (string)panel.cmbSourceVial2.SelectedItem;
                    selectedDst = (string)panel.cmbDestinationVial2.SelectedItem;
                    boRel = panel.cbRelativeSpecified2.Checked;
                    boAbs = panel.cbAbsoluteSpecified2.Checked;
                    absVolume = panel.AbsoluteVolume_uL2; // get absolute volume
                    relProportion = panel.RelativeVolumeProportion2; // get relativeVolume

                    CheckProtocolCommands_Transport_helper(absArray, relArray, absVolume, relProportion,
                        endResultSelected, selectedSrc, selectedDst, boRel, boAbs);

                    
                }
                else if ((SubCommand)myCommandSequence[i].CommandSubtype ==
                   SubCommand.TopUpTransSepTransCommand)
                {
                    GenericMultiStepCommandPanel panel = myTopUpTransSepTransCommandPanel;

                    //topup
                    string selectedSrc = (string)panel.cmbSourceVial.SelectedItem;
                    string selectedDst = (string)panel.cmbDestinationVial.SelectedItem;
                    bool boRel = panel.cbRelativeSpecified.Checked;
                    bool boAbs = panel.cbAbsoluteSpecified.Checked;
                    absVolume = panel.AbsoluteVolume_uL; // get absolute volume
                    int absLowVolume = panel.AbsoluteVolume_uL;
                    relProportion = panel.RelativeVolumeProportion; // get relativeVolume

                    CheckProtocolCommands_TopUpResus_helper(absVolume, relProportion, endResultSelected, selectedSrc, selectedDst, boRel, boAbs, ref absLowVolume);

                    //trans1
                    selectedSrc = (string)panel.cmbSourceVial2.SelectedItem;
                    selectedDst = (string)panel.cmbDestinationVial2.SelectedItem;
                    boRel = panel.cbRelativeSpecified2.Checked;
                    boAbs = panel.cbAbsoluteSpecified2.Checked;
                    absVolume = panel.AbsoluteVolume_uL2; // get absolute volume
                    relProportion = panel.RelativeVolumeProportion2; // get relativeVolume

                    CheckProtocolCommands_Transport_helper(absArray, relArray, absVolume, relProportion,
                        endResultSelected, selectedSrc, selectedDst, boRel, boAbs);

                    //sep
                    NumberOfSeparations++;
                    TotalSeparationTime += (double)(Math.Round((double)(panel.WaitCommandTimeDuration) / 60.0, 1));

                    //trans2
                    selectedSrc = (string)panel.cmbSourceVial3.SelectedItem;
                    selectedDst = (string)panel.cmbDestinationVial3.SelectedItem;
                    boRel = panel.cbRelativeSpecified3.Checked;
                    boAbs = panel.cbAbsoluteSpecified3.Checked;
                    absVolume = panel.AbsoluteVolume_uL3; // get absolute volume
                    relProportion = panel.RelativeVolumeProportion3; // get relativeVolume

                    CheckProtocolCommands_Transport_helper(absArray, relArray, absVolume, relProportion,
                        endResultSelected, selectedSrc, selectedDst, boRel, boAbs);
                }
                else if ((SubCommand)myCommandSequence[i].CommandSubtype ==
                    SubCommand.TopUpTransCommand)
                {
                    GenericMultiStepCommandPanel panel = myTopUpTransCommandPanel;

                    //topup
                    string selectedSrc = (string)panel.cmbSourceVial.SelectedItem;
                    string selectedDst = (string)panel.cmbDestinationVial.SelectedItem;
                    bool boRel = panel.cbRelativeSpecified.Checked;
                    bool boAbs = panel.cbAbsoluteSpecified.Checked;
                    absVolume = panel.AbsoluteVolume_uL; // get absolute volume
                    int absLowVolume = panel.AbsoluteVolume_uL;
                    relProportion = panel.RelativeVolumeProportion; // get relativeVolume

                    CheckProtocolCommands_TopUpResus_helper(absVolume, relProportion, endResultSelected, selectedSrc, selectedDst, boRel, boAbs, ref absLowVolume);

                    //trans1
                    selectedSrc = (string)panel.cmbSourceVial2.SelectedItem;
                    selectedDst = (string)panel.cmbDestinationVial2.SelectedItem;
                    boRel = panel.cbRelativeSpecified2.Checked;
                    boAbs = panel.cbAbsoluteSpecified2.Checked;
                    absVolume = panel.AbsoluteVolume_uL2; // get absolute volume
                    relProportion = panel.RelativeVolumeProportion2; // get relativeVolume

                    CheckProtocolCommands_Transport_helper(absArray, relArray, absVolume, relProportion,
                        endResultSelected, selectedSrc, selectedDst, boRel, boAbs);


                }
                else if ((SubCommand)myCommandSequence[i].CommandSubtype ==
                    SubCommand.ResusMixSepTransCommand)
                {
                    GenericMultiStepCommandPanel panel = myResusMixSepTransCommandPanel;

                    //resus
                    string selectedSrc = (string)panel.cmbSourceVial.SelectedItem;
                    string selectedDst = (string)panel.cmbDestinationVial.SelectedItem;
                    bool boRel = panel.cbRelativeSpecified.Checked;
                    bool boAbs = panel.cbAbsoluteSpecified.Checked;
                    absVolume = panel.AbsoluteVolume_uL; // get absolute volume
                    int absLowVolume = panel.AbsoluteVolume_uL;
                    relProportion = panel.RelativeVolumeProportion; // get relativeVolume

                    CheckProtocolCommands_TopUpResus_helper(absVolume, relProportion, endResultSelected, selectedSrc, selectedDst, boRel, boAbs, ref absLowVolume);

                    //trans1
                    selectedSrc = (string)panel.cmbSourceVial2.SelectedItem;
                    selectedDst = (string)panel.cmbDestinationVial2.SelectedItem;
                    boRel = panel.cbRelativeSpecified2.Checked;
                    boAbs = panel.cbAbsoluteSpecified2.Checked;
                    absVolume = panel.AbsoluteVolume_uL2; // get absolute volume
                    relProportion = panel.RelativeVolumeProportion2; // get relativeVolume

                    CheckProtocolCommands_Transport_helper(absArray, relArray, absVolume, relProportion,
                        endResultSelected, selectedSrc, selectedDst, boRel, boAbs);

                    //sep
                    NumberOfSeparations++;
                    TotalSeparationTime += (double)(Math.Round((double)(panel.WaitCommandTimeDuration) / 60.0, 1));

                }
                else if ((SubCommand)myCommandSequence[i].CommandSubtype ==
                    SubCommand.ResusMixCommand)
                {
                    GenericMultiStepCommandPanel panel = myResusMixCommandPanel;

                    //resus
                    string selectedSrc = (string)panel.cmbSourceVial.SelectedItem;
                    string selectedDst = (string)panel.cmbDestinationVial.SelectedItem;
                    bool boRel = panel.cbRelativeSpecified.Checked;
                    bool boAbs = panel.cbAbsoluteSpecified.Checked;
                    absVolume = panel.AbsoluteVolume_uL; // get absolute volume
                    int absLowVolume = panel.AbsoluteVolume_uL;
                    relProportion = panel.RelativeVolumeProportion; // get relativeVolume

                    CheckProtocolCommands_TopUpResus_helper(absVolume, relProportion, endResultSelected, selectedSrc, selectedDst, boRel, boAbs, ref absLowVolume);


                }
                else if ((SubCommand)myCommandSequence[i].CommandSubtype ==
                   SubCommand.MixTransCommand)
                {
                    GenericMultiStepCommandPanel2 panel = myMixTransCommandPanel;

                    //trans1
                    string selectedSrc = (string)panel.cmbSourceVial2.SelectedItem;
                    string selectedDst = (string)panel.cmbDestinationVial2.SelectedItem;
                    bool boRel = panel.cbRelativeSpecified2.Checked;
                    bool boAbs = panel.cbAbsoluteSpecified2.Checked;
                    absVolume = panel.AbsoluteVolume_uL2; // get absolute volume
                    relProportion = panel.RelativeVolumeProportion2; // get relativeVolume

                    CheckProtocolCommands_Transport_helper(absArray, relArray, absVolume, relProportion,
                        endResultSelected, selectedSrc, selectedDst, boRel, boAbs);
                }

                // 2011-09-12 sp
                // added extension times summary and volume checking of individual commands
                TotalExtensionTime += (double)(Math.Round((double)(myCommandSequence[i].CommandExtensionTime) / 60.0, 1));
                individualCommand_OK &= CheckCommandVolumeLevels(null);     // keep track of invalid volume checks
            }

            //if ( !endResultSelected && defaultEndResult[0] >= 0 )
            //{
            //    bool [] resultVialChecked = new bool[ 8 ];
            //    for( int idx = 0; idx < 8; idx++ )
            //        resultVialChecked[idx] = false;
            //    resultVialChecked[defaultEndResult[1]] = true;
            //    theProtocolModel.UpdateResultVialChecks(defaultEndResult[0], resultVialChecked);
            //}
               
            // restore current index
			myCurrentCommandIndex = currentPlaceHolder;
            UpdateCommandDetailViewEx(true);
            // 2011-11-16 sp
            isEditModeEnabled = true;

			result = volcheck.VerifyVolumes();
            if ( !individualCommand_OK )
                result |= 0x80;
            if (forceDisplay || result != 0 || !endResultSelected)				//CWJ ADD  // 2011-11-03 sp -- added force display
			{
                // 2011-11-03 sp -- replace with quadrant specified by protocol instead of auto-increment
                //dlg.SetDefaultEndResult = defaultEndResult;
                dlg.SetMaxQuadrantIndex = volcheck.GetMaxQuadrantIndex; 
                //dlg.NumOfCocktailProps = volcheck.NumOfCocktailProps;
                //dlg.NumOfParticleProps = volcheck.NumOfParticleProps;
                //dlg.NumOfAntibodyProps = volcheck.NumOfAntibodyProps;

				dlg.SetTotalMaxLimit = volcheck.MaxSampleValue;
				dlg.SetTotalMinLimit = volcheck.MinLimitValue;
                dlg.SetRelCock = volcheck.GetCocktailRelVol;
                dlg.SetRelPart = volcheck.GetParticleRelVol;
                dlg.SetRelAnti = volcheck.GetAntibodyRelVol;
                // 2010-09-09 sp added absolute volume reporting
                dlg.SetAbsCock = volcheck.GetCocktailAbsVol;
                dlg.SetAbsPart = volcheck.GetParticleAbsVol;
                dlg.SetAbsAnti = volcheck.GetAntibodyAbsVol;

                // 2011-09-12 sp
                // removed; obsolete values
                //dlg.SetSumMax = volcheck.SumMaxValues;
                //dlg.SetSumMin		 = volcheck.SumMinValues;

				dlg.SetSampleMaxVol  = volcheck.SampMaxVol;
				dlg.SetSampleMinVol	 = volcheck.SampMinVol;
				dlg.SetMaxReagCock   = volcheck.MaxReagentCockValues;
				dlg.SetMinReagCock   = volcheck.MinReagentCockValues;
				dlg.SetMaxReagPart   = volcheck.MaxReagentPartValues;
				dlg.SetMinReagPart   = volcheck.MinReagentPartValues;
				dlg.SetMaxReagAnti   = volcheck.MaxReagentAntiValues;
				dlg.SetMinReagAnti   = volcheck.MinReagentAntiValues;

                // 2011-09-14 sp
                // provide support for error reporting flags
                dlg.SetMaxReagCockFlags = volcheck.GetMaxReagentCockFlags;
                dlg.SetMinReagCockFlags = volcheck.GetMinReagentCockFlags;
                dlg.SetMaxReagPartFlags = volcheck.GetMaxReagentPartFlags;
                dlg.SetMinReagPartFlags = volcheck.GetMinReagentPartFlags;
                dlg.SetMaxReagAntiFlags = volcheck.GetMaxReagentAntiFlags;
                dlg.SetMinReagAntiFlags = volcheck.GetMinReagentAntiFlags;


                // 2011-09-14 sp
                // provide support for working volume checks and error reporting flags
                dlg.SetMinWorkingSample = volcheck.GetMinSampleWorkingVol;
                dlg.SetMinWorkingSeparation = volcheck.GetMinSeparationWorkingVol;
                dlg.SetMaxWorkingSample = volcheck.GetMaxSampleWorkingVol;
                dlg.SetMaxWorkingSeparation = volcheck.GetMaxSeparationWorkingVol;
                dlg.SetMinWorkingSampleFlags = volcheck.GetMinSampleWorkingVolFlags;
                dlg.SetMinWorkingSeparationFlags = volcheck.GetMinSeparationWorkingVolFlags;
                dlg.SetMaxWorkingSampleFlags = volcheck.GetMaxSampleWorkingVolFlags;
                dlg.SetMaxWorkingSeparationFlags = volcheck.GetMaxSeparationWorkingVolFlags;

                // 2011-09-12 sp
                // provide support for incubation and separation time reporting
                dlg.SetTotalIncubationTime = TotalIncubationTime;
                dlg.SetTotalSeparationTime = TotalSeparationTime;
                dlg.SetNumberOfSeparations = NumberOfSeparations;
                dlg.SetNumberOfIncubations = NumberOfIncubations;
                dlg.SetTotalExtensionTime = TotalExtensionTime;
                // 2011-09-12 sp
                // rename SetVolumeErrorColor to more generic display function
                dlg.SetCheckProtocolDlgContents = result;
               
				dlg.ShowDialog(this);		//CWJ ADD

                // 2011-11-10 sp
                // override errors in protocol to allow it to be accepted
                if (dlg.acceptProtocolOverride)
                    result = 0;

				dlg.Dispose();				//CWJ ADD	

					
			}
			ClearVolumeCheck();				//CWJ ADD	
            
			// 2011-09-12 sp
		    // clear warning flags
            result &= 0xFFFE;
			return (result != 0)?false:true;
		}

        private void CheckProtocolCommands_TopUpResus_helper(int absVolume, double relProportion, bool endResultSelected, string selectedSrc, string selectedDst, bool boRel, bool boAbs, ref int absLowVolume)
        {
            if (relProportion == 0 || absVolume == 0)
            {
                absLowVolume = myWorkingVolumeLow_uL;
                absVolume = myWorkingVolumeHigh_uL;
            }
            absLowVolume = Math.Min(absLowVolume, myWorkingVolumeLow_uL);
            absVolume = Math.Min(absVolume, myWorkingVolumeHigh_uL);

            updateTubeVolumes(selectedSrc, selectedDst, relProportion, absLowVolume, absVolume, true, boRel, boAbs);
            //setDefaultEndResult(endResultSelected, selectedDst, defaultEndResult);
        }

        private ReagentVial CheckProtocolCommands_Transport_helper(int[] absArray, int[] relArray, int absVolume, double relProportion, bool endResultSelected, string selectedSrc, string selectedDst, bool boRel, bool boAbs)
        {
            ReagentVial currentVial = ReagentVial.TOTAL;
            bool isCocktailVial = true;
            //cocktail
            if (selectedSrc.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.VialB)) != -1 ||
                selectedSrc.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.VialBpos)) != -1 ||
                selectedSrc.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.VialBneg)) != -1)
                currentVial = ReagentVial.Cocktail;
            //particle
            else if (selectedSrc.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.VialA)) != -1)
                currentVial = ReagentVial.Particle;
            //antibody
            else if (selectedSrc.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.VialC)) != -1)
                currentVial = ReagentVial.Antibody;
            else
            // 2011-09-21 sp
            // added section to check for tube volumes and flag to avoid bypass by other checks by continue statement
            // continue;
            {
                isCocktailVial = false;
            }

            if (isCocktailVial)
            {
                //  2011-11-03 sp -- get quadrant for addition to destination tube
                int dstQuadrantIndex = 0;
                try
                {
                    dstQuadrantIndex = (int)Convert.ToDecimal(selectedDst.Substring(1, 1)) - 1;
                }
                catch (FormatException) { }

                if (boRel)
                {
                    relArray[(int)currentVial] += (int)Math.Round(relProportion * 1000);
                    //                            SetRelVolume((int)currentVial, (int)Math.Round(relProportion * 1000), dstQuadrantIndex);	//CWJ Add
                    AddRelVolume((int)currentVial, (int)Math.Round(relProportion * 1000), dstQuadrantIndex);	//CWJ Add
                }
                else if (boAbs)
                {
                    absArray[(int)currentVial] += absVolume;
                    // 2011-09-09 sp
                    // re-introduced to keep track of total volumes
                    //                            SetAbsVolume((int)currentVial, absVolume, dstQuadrantIndex);								//CWJ Add
                    AddAbsVolume((int)currentVial, absVolume, dstQuadrantIndex);								//CWJ Add
                }
            }
            updateTubeVolumes(selectedSrc, selectedDst, relProportion, absVolume, absVolume, false, boRel, boAbs);
            //setDefaultEndResult(endResultSelected, selectedDst, defaultEndResult);
            return currentVial;
        }


        // 2011-11-09 sp
        // if end-result selection has not been defined, save location for the last tube accessed
        /*
        public void setIfNotCheckedEndResult()
        {
            if (defaultEndResult[0] >= 0 && defaultEndResult[1] >= 0)
            {
                bool[] resultVialChecks;
                try
                {
                    theProtocolModel.InitResultVialChecks();
                    theProtocolModel.GetResultVialChecks(defaultEndResult[0], out resultVialChecks);
                    if (resultVialChecks != null)//set checks 
                    {
                        resultVialChecks[defaultEndResult[1]] = true;
                        theProtocolModel.UpdateResultVialChecks(defaultEndResult[0], resultVialChecks);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        */

        // 2011-11-03 sp -- added volume updating of sample and separation tubes
        private void updateTubeVolumes(String selectedSrc, String selectedDst, double relProportion, int absLowVolume, int absHighVolume, bool replace, bool boRel, bool boAbs)
        {
			ProtocolReagentVolumeCheck volcheck = ProtocolReagentVolumeCheck.GetInstance();
            int SampleMaxVol = mySampleVolumeMax_uL;
            int SampleMinVol = mySampleVolumeMin_uL;

            double workingVolume = 0;
            int srcQuadrantIndex = 0;
            int dstQuadrantIndex = 0;
            // get quadrant of source and destination tubes
            try
            {
                srcQuadrantIndex = (int)Convert.ToDecimal(selectedSrc.Substring(1, 1)) - 1;
            }
            catch (FormatException) { }
            try
            {
                dstQuadrantIndex = (int)Convert.ToDecimal(selectedDst.Substring(1, 1)) - 1;
            }
            catch (FormatException) { }

            double volumeTransportedMax = 0;
            double volumeTransportedMin = 0;
            // get volumes
            if (boRel)
            {
                volumeTransportedMin = (int)Math.Round(relProportion * SampleMinVol);
                volumeTransportedMax = (int)Math.Round(relProportion * SampleMaxVol);
            }
            else if (boAbs)
            {
                volumeTransportedMin = absLowVolume;
                volumeTransportedMax = absHighVolume;
            }

            if (selectedSrc.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.SampleTube)) != -1)
            {
                workingVolume = WorkingSampleMinAddition[srcQuadrantIndex] - volumeTransportedMin;
                volumeTransportedMin = Math.Min(volumeTransportedMin, WorkingSampleMinAddition[srcQuadrantIndex]);
                WorkingSampleMinAddition[srcQuadrantIndex] = workingVolume;

                workingVolume = WorkingSampleMaxAddition[srcQuadrantIndex] - volumeTransportedMax;
                volumeTransportedMax = Math.Min(volumeTransportedMax, WorkingSampleMaxAddition[srcQuadrantIndex]);
                WorkingSampleMaxAddition[srcQuadrantIndex] = workingVolume;
            }
            else if (selectedSrc.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.SeparationTube)) != -1)
            {
                workingVolume = WorkingSeparationMinAddition[srcQuadrantIndex] - volumeTransportedMin;
                volumeTransportedMin = Math.Min(volumeTransportedMin, WorkingSeparationMinAddition[srcQuadrantIndex]);
                WorkingSeparationMinAddition[srcQuadrantIndex] = workingVolume;

                workingVolume = WorkingSeparationMaxAddition[srcQuadrantIndex] - volumeTransportedMax;
                volumeTransportedMax = Math.Min(volumeTransportedMax, WorkingSeparationMaxAddition[srcQuadrantIndex]);
                WorkingSeparationMaxAddition[srcQuadrantIndex] = workingVolume;
            }
            else if (selectedSrc.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.VialB)) != -1 ||
                selectedSrc.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.VialBpos)) != -1 ||
                selectedSrc.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.VialBneg)) != -1)
            {
                maxCocktailNeeded[srcQuadrantIndex] += volumeTransportedMax;
            }
            else if (selectedSrc.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.VialA)) != -1)
            {
                maxParticlesNeeded[srcQuadrantIndex] += volumeTransportedMax;
            }
            else if (selectedSrc.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.VialC)) != -1)
            {
                maxAntibodyNeeded[srcQuadrantIndex] += volumeTransportedMax;
            }
 
            if (selectedDst.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.SampleTube)) != -1)
            {
                if (replace)
                {
                    WorkingSampleMinAddition[dstQuadrantIndex] = Math.Max( WorkingSampleMinAddition[dstQuadrantIndex], volumeTransportedMin );
                    WorkingSampleMaxAddition[dstQuadrantIndex] = Math.Max( WorkingSampleMaxAddition[dstQuadrantIndex], volumeTransportedMax );
                }
                else
                {
                    WorkingSampleMinAddition[dstQuadrantIndex] += volumeTransportedMin;
                    WorkingSampleMaxAddition[dstQuadrantIndex] += volumeTransportedMax;
                }
                volcheck.SetQuadrantIndex = dstQuadrantIndex;
                volcheck.SetMinSampleWorkingVol = Math.Max(volcheck.GetMinSampleTubeVol[dstQuadrantIndex], WorkingSampleMinAddition[dstQuadrantIndex]);
                volcheck.SetMaxSampleWorkingVol = Math.Max(volcheck.GetMaxSampleTubeVol[dstQuadrantIndex], WorkingSampleMaxAddition[dstQuadrantIndex]);
            }
            else if (selectedDst.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.SeparationTube)) != -1)
            {
                if (replace)
                {
                    WorkingSeparationMinAddition[dstQuadrantIndex] =  Math.Max( WorkingSeparationMinAddition[dstQuadrantIndex], volumeTransportedMin);
                    WorkingSeparationMaxAddition[dstQuadrantIndex] =  Math.Max( WorkingSeparationMaxAddition[dstQuadrantIndex], volumeTransportedMax);
                }
                else
                {
                    WorkingSeparationMinAddition[dstQuadrantIndex] += volumeTransportedMin;
                    WorkingSeparationMaxAddition[dstQuadrantIndex] += volumeTransportedMax;
                }
                volcheck.SetQuadrantIndex = dstQuadrantIndex;
                volcheck.SetMinSeparationWorkingVol = Math.Max(volcheck.GetMinSeparationTubeVol[dstQuadrantIndex], WorkingSeparationMinAddition[dstQuadrantIndex]);
                volcheck.SetMaxSeparationWorkingVol = Math.Max(volcheck.GetMaxSeparationTubeVol[dstQuadrantIndex], WorkingSeparationMaxAddition[dstQuadrantIndex]);
            }
            else if (selectedDst.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.NegativeFractionTube)) != -1)
            {
                if (replace)
                    maxNegFractionVolume[dstQuadrantIndex] = Math.Max(maxNegFractionVolume[dstQuadrantIndex], volumeTransportedMax);
                else
                    maxNegFractionVolume[dstQuadrantIndex] += volumeTransportedMax;
            }
            else if (selectedDst.IndexOf(SeparatorResourceManager.GetSeparatorString(StringId.WasteTube)) != -1)
            {
                if (replace)
                    maxWasteVolume[dstQuadrantIndex] = Math.Max(maxWasteVolume[dstQuadrantIndex], volumeTransportedMax);
                else
                    maxWasteVolume[dstQuadrantIndex] += volumeTransportedMax;
            }
        }

		public void SetValidCheckMark(bool result)
		{
			lblValidateResult.Text = result ? "þ" : "ý";
			// if validation successful, reset the 'page dirty' flag
			if (( ! isFormDirty) || result)
			{
				ExitEditMode();		
			}

			if (result)
			{
				SetAddingMode(false);
			}
		}

		private void lblHeader_Click(object sender, System.EventArgs e)
		{
		
		}

        private void lblValidateResult_Click(object sender, EventArgs e)
        {

        }

        private void mySeparateCommandPanel_Load(object sender, EventArgs e)
        {

        }

        private void myDemoCommandPanel_Load(object sender, EventArgs e)
        {

        }

        public void ThingsToDoWhenProtocolFormatChanges()
        {
            SetupToolTips();
            UpdateMaxMixCycle();
            UpdateVialComboboxes();
            SetRobosepTypeLabel("RoboSep Type: " + SeparatorResourceManager.ProtocolFormatToUserDisplayString(SeparatorResourceManager.GetProtocolFormat()));

            //change max min limits
            UpateLiquidVolumeLimits();
            ProtocolReagentVolumeCheck.GetInstance().UpateLiquidVolumeLimits();
            UpdatePanelLiquidVolumeLimits();

        }
        
        public void UpdateMaxMixCycle()
        {
            myVolumeCommandPanel.UpdateMaxMixCycle();
            myTopUpMixTransCommandPanel.UpdateMaxMixCycle();
            myResusMixSepTransCommandPanel.UpdateMaxMixCycle();
            myResusMixCommandPanel.UpdateMaxMixCycle();
            myMixTransCommandPanel.UpdateMaxMixCycle();
        }

        public void UpdateVialComboboxes()
        {
            for (int i = 0; i < myCommandPanels.GetLength(0); ++i)
            {
                VolumeCommandPanel panel = myCommandPanels[i] as VolumeCommandPanel;
                if (panel != null)
                    panel.UpdateComboBoxes((Tesla.Common.Protocol.ProtocolClass)(cmbProtocolType.SelectedIndex));
            }     
        }

        public void UpdatePanelLiquidVolumeLimits()
        {
            for (int i = 0; i < myCommandPanels.GetLength(0); ++i)
            {
                VolumeCommandPanel panel = myCommandPanels[i] as VolumeCommandPanel;
                if (panel != null)
                    panel.UpateLiquidVolumeLimits();
            }
        }


        public void SetRobosepTypeLabel(string s)
        {
            lblRobosepType.Text = s;
        }

        public bool EnableUserAddComboCommands()
        {
            int iResult = Tesla.Common.Utilities.GetSoftwareConfigInt("Editor_Options", "enable_user_add_combo_commands", 0);
            return iResult == 1;
        }

        public void UpateLiquidVolumeLimits()
        {
            volumeLimits.UpateLiquidVolumeLimits();
        }


        internal string GetLabel()
        {
            return txtProtocolLabel.Text;
        }

        private void btnLabelInfo_Click(object sender, EventArgs e)
        {
            MessageBox.Show(SeparatorResourceManager.isPlatformRS16()?LABEL_ICON_INFO_RS16:LABEL_ICON_INFO_RSS,
                "Label Format Recommendation");
		
        }
    }


	public class ProtocolReagentVolumeCheck
    {
        private const int errorFlag = 0x2;
        private const int warningFlag = 0x1;

		private static ProtocolReagentVolumeCheck volcheck = null;
		// 2011-09-09 sp
        // added configuration file parameters
       private double myMaxSampleValue;
        private double TotalMinVol = 0;  // disable checking for total reagents and sample (checking already in place for individual additions
        private double MinRecommendedReagentVol;

        private VolumeLimits volumeLimits;

//      private const double	TotalMaxVol   = 10000f;
//		private const double	TotalMinVol   = 12.5f;
//		private const double MaxReagentVol = 1000f;

		private int   SampleMinVolume;
		private int	  SampleMaxVolume;
		private int   idx_cocktail;
		private int   idx_particle;
        private int   idx_antibody;

        private int[] CocktailVol = new int[4];
        private int[] ParticleVol = new int[4];
        private int[] AntibodyVol = new int[4];
        // 2011-09-09 sp
        // added support for absolute cocktail volumes
        private int[] AbsCocktailVol = new int[4];
        private int[] AbsParticleVol = new int[4];
        private int[] AbsAntibodyVol = new int[4];
        // 2011-09-22 sp
        // added support for max volumes in sample and separation tubes
        private int maxQuadrantIndex;
        private int quadrantIndex;
        private int srcTubeIndex;
        private int dstTubeIndex;
        private double[] MaxWorkingSampleMin = new double[4];
        private double[] MaxWorkingSampleMax = new double[4];
        private double[] MaxWorkingSeparationMin = new double[4];
        private double[] MaxWorkingSeparationMax = new double[4];
        //private double[] MaxWorkingSample = new double[4];
        //private double[] MaxWorkingSeparation = new double[4];
        
		private double[] SumMax			  = new double[4];
		private double[] SumMin			  = new double[4];
		private double[] MaxReagentCock    = new double[4];
		private double[] MaxReagentPart    = new double[4];
		private double[] MaxReagentAnti    = new double[4];
		private double[] MinReagentCock    = new double[4];
		private double[] MinReagentPart    = new double[4];
		private double[] MinReagentAnti    = new double[4];

 		private int[] MaxReagentCockFlags    = new int[4];
 		private int[] MinReagentCockFlags    = new int[4];
 		private int[] MaxReagentPartFlags    = new int[4];
 		private int[] MinReagentPartFlags    = new int[4];
 		private int[] MaxReagentAntiFlags    = new int[4];
 		private int[] MinReagentAntiFlags    = new int[4];
 		private int[] MinSampleWorkingVolFlags    = new int[4];
 		private int[] MinSeparationWorkingVolFlags    = new int[4];
 		private int[] MaxSampleWorkingVolFlags    = new int[4];
 		private int[] MaxSeparationWorkingVolFlags    = new int[4];

		public int NumOfCocktailProps
		{
			get
			{
				return (idx_cocktail + 1);
			}
		}
		
		public int NumOfParticleProps
		{
			get
			{
				return (idx_particle + 1);
			}
		
		}

		public int NumOfAntibodyProps
		{
			get
			{
				return (idx_antibody + 1);
			}		
		}

		public double[] MaxReagentCockValues
		{
			get
			{
				return MaxReagentCock;
			}
		}
		public double[] MinReagentCockValues
		{
			get
			{
				return MinReagentCock;
			}
		}

		public double[] MaxReagentPartValues
		{
			get
			{
				return MaxReagentPart;
			}
		}
		public double[] MinReagentPartValues
		{
			get
			{
				return MinReagentPart;
			}
		}
		public double[] MaxReagentAntiValues
		{
			get
			{
				return MaxReagentAnti;
			}
		}
		public double[] MinReagentAntiValues
		{
			get
			{
				return MinReagentAnti;
			}
		}

		public int[] GetMaxReagentCockFlags
		{
			get
			{
				return MaxReagentCockFlags;
			}
		}

		public int[] GetMinReagentCockFlags
		{
			get
			{
				return MinReagentCockFlags;
			}
		}

		public int[] GetMaxReagentPartFlags
		{
			get
			{
				return MaxReagentPartFlags;
			}
		}

		public int[] GetMinReagentPartFlags
		{
			get
			{
				return MinReagentPartFlags;
			}
		}

		public int[] GetMaxReagentAntiFlags
		{
			get
			{
				return MaxReagentAntiFlags;
			}
		}

		public int[] GetMinReagentAntiFlags
		{
			get
			{
				return MinReagentAntiFlags;
			}
		}

		public int[] GetMinSampleWorkingVolFlags
		{
			get
			{
				return MinSampleWorkingVolFlags;
			}
		}

		public int[] GetMinSeparationWorkingVolFlags
		{
			get
			{
				return MinSeparationWorkingVolFlags;
			}
		}

		public int[] GetMaxSampleWorkingVolFlags
		{
			get
			{
				return MaxSampleWorkingVolFlags;
			}
		}

		public int[] GetMaxSeparationWorkingVolFlags
		{
			get
			{
				return MaxSeparationWorkingVolFlags;
			}
		}


        public double[] MaxWorkingSampleValues
        {
            get
            {
                return MaxWorkingSampleMax;
            }
        }


        public double[] MaxWorkingSeparationValues
        {
            get
            {
                return MaxWorkingSeparationMax;
            }
        }


		public double[] SumMaxValues 
		{
			get
			{
				return SumMax;
			}
		}

		public double[] SumMinValues 
		{
			get
			{
				return SumMin;
			}
		}

        // 2011-11-18 sp -- volume checks for indivual transports and vial volumes already in place
        //public double MaxReagentValue 
        //{
        //    get
        //    {
        //        return MaxReagentVol;
        //    }
        //}



		public double MaxSampleValue 
		{
			get
			{
				return myMaxSampleValue;
			}
		}

		public double MinLimitValue 
		{
			get
			{
				return TotalMinVol;
			}
		}


        public int SetCocktailRelVol
        {
            set
            {
                // 2011-11-03 sp -- replace with specified quadrant instead of auto-increment
                CocktailVol[ quadrantIndex ] = value;
                //if (idx_cocktail < 4)
                //{
                //   CocktailVol[++idx_cocktail] = value;
                //}
                //else
                //{
                //    string msg;
                //    msg = "idx_cocktail - bigger than 4!!";
                //    CWJDbgView(msg);
                //}
            }
            // 2011-11-10 sp -- return current value
            get
            {
                return CocktailVol[quadrantIndex];
            }
        }
        public int SetParticleRelVol
        {
            set
            {
                // 2011-11-03 sp -- replace with specified quadrant instead of auto-increment
                ParticleVol[ quadrantIndex ] = value;
                //if (idx_particle < 4)
                //{
                //    ParticleVol[++idx_particle] = value;
                //}
                //else
                //{
                //    string msg;
                //    msg = "idx_particle - bigger than 4!!";
                //    CWJDbgView(msg);
                //}
            }
            // 2011-11-10 sp -- return current value
            get
            {
                return ParticleVol[quadrantIndex];
            }
        }
        public int SetAntibodyRelVol
        {
            set
            {
                // 2011-11-03 sp -- replace with specified quadrant instead of auto-increment
                AntibodyVol[quadrantIndex] = value;
                //if (idx_antibody < 4)
                //{
                //    AntibodyVol[++idx_antibody] = value;
                //}
                //else
                //{
                //    string msg;
                //    msg = "idx_antibody - bigger than 4!!";
                //    CWJDbgView(msg);
                //}
            }
            // 2011-11-10 sp -- return current value
            get
            {
                return AntibodyVol[quadrantIndex];
            }
        }


        // 2011-09-09 sp
        // added for total volume checking
        public int SetCocktailAbsVol
        {
            set
            {
                // 2011-11-03 sp -- replace with specified quadrant instead of auto-increment
                AbsCocktailVol[quadrantIndex] = value;
                //if (idx_cocktail < 4)
                //{
                //    AbsCocktailVol[++idx_cocktail] = value;
                //}
                //else
                //{
                //    string msg;
                //    msg = "idx_cocktail - bigger than 4!!";
                //    CWJDbgView(msg);
                //}
            }
            // 2011-11-10 sp -- return current value
            get
            {
                return AbsCocktailVol[quadrantIndex];
            }
        }
        // 2011-09-09 sp
        // added for total volume checking
        public int SetParticleAbsVol
        {
            set
            {
                // 2011-11-03 sp -- replace with specified quadrant instead of auto-increment
                AbsParticleVol[quadrantIndex] = value;
                //if (idx_particle < 4)
                //{
                //    AbsParticleVol[++idx_particle] = value;
                //}
                //else
                //{
                //    string msg;
                //    msg = "idx_particle - bigger than 4!!";
                //    CWJDbgView(msg);
                //}
            }
            // 2011-11-10 sp -- return current value
            get
            {
                return AbsParticleVol[quadrantIndex];
            }
        }
        // 2011-09-09 sp
        // added for total volume checking
        public int SetAntibodyAbsVol
        {
            set
            {
                // 2011-11-03 sp -- replace with specified quadrant instead of auto-increment
                AbsAntibodyVol[quadrantIndex] = value;
                //if (idx_antibody < 4)
                //{
                //    AbsAntibodyVol[++idx_antibody] = value;
                //}
                //else
                //{
                //    string msg;
                //    msg = "idx_antibody - bigger than 4!!";
                //    CWJDbgView(msg);
                //}
            }
            // 2011-11-10 sp -- return current value
            get
            {
                return AbsAntibodyVol[quadrantIndex];
            }
        }
        // 2011-09-22 sp
        // added for separation volume checking
        public double[] GetMinSeparationWorkingVol
        {
            get
            {
                return MaxWorkingSeparationMin;
            }
        }

        // 2011-09-22 sp
        // added for sample volume checking
        public double[] GetMinSampleWorkingVol
        {
            get
            {
                return MaxWorkingSampleMin;
            }
        }

        // 2011-09-22 sp
        // added for separation volume checking
        public double[] GetMaxSeparationWorkingVol
        {
            get
            {
                return MaxWorkingSeparationMax;
            }
        }

        // 2011-09-22 sp
        // added for sample volume checking
        public double[] GetMaxSampleWorkingVol
        {
            get
            {
                return MaxWorkingSampleMax;
            }
        }


        // 2011-09-22 sp
        // added for sample volume checking
        public double SetMinSampleWorkingVol
        {
            set
            {
                if (quadrantIndex < 4)
                {
                    MaxWorkingSampleMin[quadrantIndex] = value;
                }
            }
        }
        // 2011-09-22 sp
        // added for separation volume checking
        public double SetMinSeparationWorkingVol
        {
            set
            {
                if (quadrantIndex < 4)
                {
                    MaxWorkingSeparationMin[quadrantIndex] = value;
                }
            }
        }

        // 2011-09-22 sp
        // added for sample volume checking
        public double SetMaxSampleWorkingVol
        {
            set
            {
                if (quadrantIndex < 4)
                {
                    MaxWorkingSampleMax[quadrantIndex] = value;
                }
            }
        }
        // 2011-09-22 sp
        // added for separation volume checking
        public double SetMaxSeparationWorkingVol
        {
            set
            {
                if (quadrantIndex < 4)
                {
                    MaxWorkingSeparationMax[quadrantIndex] = value;
                }
            }
        }


		public int[] GetCocktailRelVol 
		{
			get
			{
				return CocktailVol;
			}
		}
		public int[] GetParticleRelVol 
		{
			get
			{
				return ParticleVol;
			}
		}
		public int[] GetAntibodyRelVol
		{
			get
			{
				return AntibodyVol;
			}
		}

        // 2011-09-09 sp
        // added for total volume checking
        public int[] GetCocktailAbsVol
        {
            get
            {
                return AbsCocktailVol;
            }
        }
        // 2011-09-09 sp
        // added for total volume checking
        public int[] GetParticleAbsVol
        {
            get
            {
                return AbsParticleVol;
            }
        }
        // 2011-09-09 sp
        // added for total volume checking
        public int[] GetAntibodyAbsVol
        {
            get
            {
                return AbsAntibodyVol;
            }
        }
        // 2011-09-22 sp
        // added for sample volume checking
        public double[] GetMinSampleTubeVol
        {
            get
            {
                return MaxWorkingSampleMin;
            }
        }
        // 2011-09-22 sp
        // added for separation volume checking
        public double[] GetMinSeparationTubeVol
        {
            get
            {
                return MaxWorkingSeparationMin;
            }
        }
        // 2011-09-22 sp
        // added for sample volume checking
        public double[] GetMaxSampleTubeVol
        {
            get
            {
                return MaxWorkingSampleMax;
            }
        }
        // 2011-09-22 sp
        // added for separation volume checking
        public double[] GetMaxSeparationTubeVol
        {
            get
            {
                return MaxWorkingSeparationMax;
            }
        }
        // 2011-09-22 sp
        // added for tube index
        public int GetMaxQuadrantIndex
        {
            get
            {
                return maxQuadrantIndex;
            }
        }
        // 2011-09-09 sp
        // added for setting quadrant to add volumes
        public int SetQuadrantIndex
        {
            set
            {
                //if (quadrantIndex >= 0 && quadrantIndex < 4) //what???
                if (value >= 0 && value < 4) 
                {
                    quadrantIndex = value;
                    maxQuadrantIndex = Math.Max( maxQuadrantIndex, quadrantIndex );
                }

            }
        }
        // 2011-09-22 sp
        // added for tube index
        public int GetSrcQuadrantIndexVol
        {
            get
            {
                return srcTubeIndex;
            }
        }
        // 2011-09-22 sp
        // added for tube index
        public int SetSrcQuadrantIndexVol
        {
            set
            {
                srcTubeIndex = value;
            }
        }
        // 2011-09-22 sp
        // added for tube index
        public int GetDstQuadrantIndexVol
        {
            get
            {
                return dstTubeIndex;
            }
        }
        // 2011-09-22 sp
        // added for tube index
        public int SetDstQuadrantIndexVol
        {
            set
            {
                dstTubeIndex = value;
            }
        }

		public int SampMinVol
		{
			set
			{
				SampleMinVolume = value;
				string msg;
				msg = "SampleMinVolume: " +SampleMinVolume.ToString();
				CWJDbgView(msg);
			}
			get
			{
				return SampleMinVolume;
			}

		}
		public int SampMaxVol
		{
			set
			{
				SampleMaxVolume = value;
				string msg;
				msg = "SampleMaxVolume: " +SampleMaxVolume.ToString();
				CWJDbgView(msg);
			}
			get
			{
				return SampleMaxVolume;
			}
		}


		public void ClearAllValues()
		{
			SampleMinVolume = 0;
			SampleMaxVolume = 0;

			idx_cocktail	= -1;
			idx_particle	= -1;
			idx_antibody	= -1;
								
			Array.Clear(SumMax, 0, SumMax.Length);
			Array.Clear(SumMin, 0, SumMin.Length);
			Array.Clear(MaxReagentCock, 0, MaxReagentCock.Length);
			Array.Clear(MaxReagentPart, 0, MaxReagentPart.Length);
			Array.Clear(MaxReagentAnti, 0, MaxReagentAnti.Length);
			Array.Clear(MinReagentCock, 0, MinReagentCock.Length);
			Array.Clear(MinReagentPart, 0, MinReagentPart.Length);
			Array.Clear(MinReagentAnti, 0, MinReagentAnti.Length);
            Array.Clear(CocktailVol, 0, CocktailVol.Length);
            Array.Clear(ParticleVol, 0, ParticleVol.Length);
            Array.Clear(AntibodyVol, 0, AntibodyVol.Length);
            // 2011-09-09 sp
            // added absolute, sample and separation volumes for checking totals
            maxQuadrantIndex = -1;
            Array.Clear(AbsCocktailVol, 0, AbsCocktailVol.Length);
            Array.Clear(AbsParticleVol, 0, AbsParticleVol.Length);
            Array.Clear(AbsAntibodyVol, 0, AbsAntibodyVol.Length);
            Array.Clear(MaxWorkingSampleMax, 0, MaxWorkingSampleMax.Length);
            Array.Clear(MaxWorkingSeparationMax, 0, MaxWorkingSeparationMax.Length);
            Array.Clear(MaxReagentCockFlags, 0, MaxReagentCockFlags.Length);
            Array.Clear(MinReagentCockFlags, 0, MinReagentCockFlags.Length);
            Array.Clear(MaxReagentPartFlags, 0, MaxReagentPartFlags.Length);
            Array.Clear(MinReagentPartFlags, 0, MinReagentPartFlags.Length);
            Array.Clear(MaxReagentAntiFlags, 0, MaxReagentAntiFlags.Length);
            Array.Clear(MinReagentAntiFlags, 0, MinReagentAntiFlags.Length);
            Array.Clear(MinSampleWorkingVolFlags, 0, MinSampleWorkingVolFlags.Length);
            Array.Clear(MinSeparationWorkingVolFlags, 0, MinSeparationWorkingVolFlags.Length);
            Array.Clear(MaxSampleWorkingVolFlags, 0, MaxSampleWorkingVolFlags.Length);
            Array.Clear(MaxSeparationWorkingVolFlags, 0, MaxSeparationWorkingVolFlags.Length);

            volumeLimits = VolumeLimits.GetInstance();
            volumeLimits.UpateLiquidVolumeLimits();

			CWJDbgView("VolCheck: All Values are Cleared!");
		}

		private ProtocolReagentVolumeCheck()
		{
			ClearAllValues();
		}

		public static ProtocolReagentVolumeCheck GetInstance()
		{
			if (volcheck == null)
				volcheck = new ProtocolReagentVolumeCheck();

			return volcheck;
		}

		public void CWJDbgView(string msg) 
		{ 	
			System.Diagnostics.Debug.WriteLine(msg); 			
		}

		/*
				public bool VerifyVolume()
				{
					bool	ans = true;
					double	max = 0 , min = 0;
			
					MaxReagentCock = (double)SampleMaxVolume*(double)CocktailVol[0]/1000f;
					MaxReagentPart = (double)SampleMaxVolume*(double)ParticleVol[0]/1000f;
					MaxReagentAnti = (double)SampleMaxVolume*(double)AntibodyVol[0]/1000f;
					MinReagentCock = (double)SampleMinVolume*(double)CocktailVol[0]/1000f;
					MinReagentPart = (double)SampleMinVolume*(double)ParticleVol[0]/1000f;
					MinReagentAnti = (double)SampleMinVolume*(double)AntibodyVol[0]/1000f;

					max = (double)SampleMaxVolume+MaxReagentCock+MaxReagentPart+MaxReagentAnti;
					min = (double)SampleMinVolume+MinReagentCock+MinReagentPart+MinReagentAnti;

					SumMax = max;
					SumMin = min;

					if ((max < TotalMaxVol)&&(min>TotalMinVol))
					{
						ans = true;
					}
					else
					{
						ans = false;
					}
			
					return ans;
				}
		*/		
		public int VerifyVolumes()
		{
			int	ans = 0, i, PropSum = 0;
            /*
            //  2011-09-20 sp removed
			double[]	max = new double[4] , min = new double[4];
			
			Array.Clear(max, 0, 4);
			Array.Clear(min, 0, 4);
			//*/
	
			if ((SampleMaxVolume == 0)||(SampleMinVolume == 0))
				return ans;

			for(i = 0 ; i < 4 ; i++)
				PropSum += CocktailVol[i];

			for(i = 0 ; i < 4 ; i++)
				PropSum += AntibodyVol[i];

			for(i = 0 ; i < 4 ; i++)
				PropSum += ParticleVol[i];

			if (PropSum == 0)
				return ans;

			for(i = 0 ; i < 4 ; i++)
			{
                MaxReagentCock[i] = SampleMaxVolume * CocktailVol[i] / 1000 + AbsCocktailVol[i];
                MaxReagentPart[i] = SampleMaxVolume * ParticleVol[i] / 1000 + AbsParticleVol[i];
                MaxReagentAnti[i] = SampleMaxVolume * AntibodyVol[i] / 1000 + AbsAntibodyVol[i];
                MinReagentCock[i] = SampleMinVolume * CocktailVol[i] / 1000 + AbsCocktailVol[i];
                MinReagentPart[i] = SampleMinVolume * ParticleVol[i] / 1000 + AbsParticleVol[i];
                MinReagentAnti[i] = SampleMinVolume * AntibodyVol[i] / 1000 + AbsAntibodyVol[i];
			}

            // 2011-11-09 sp
            // remove from error report; volume calculations does not capture all events (under-estimates)
            //for (i = 0; i < 4; i++)
            //{
            //    SumMax[i] = (double)SampleMaxVolume + MaxReagentCock[i] + MaxReagentPart[i] + MaxReagentAnti[i];
            //    SumMin[i] = (double)SampleMinVolume + MinReagentCock[i] + MinReagentPart[i] + MinReagentAnti[i];
            //}

            // 2011-11-09 sp
            // remove from error report; replace by other volume checks
            //for (i = 0; i < 4; i++)
            //{
            //    if (SumMax[i] > TotalMaxVol)
            //    {
            //        ans |= 1; 
            //        break;
            //    }	
            //}
            // 2011-09-09 sp added minimum volume check
            //for (i = 0; i < 4; i++)
            //{
            //    if (SumMin[i] > SampleMinVolume && SumMin[i] < TotalMinVol)
            //    {
            //        ans |= 2; 
            //        break;
            //    }
            //}

            // 2011-09-12 sp
            // added flag codes for warning levels
            for (i = 0; i < 4; i++)
            {
                if (MinReagentAnti[i] > 0 && MinReagentAnti[i] < MinRecommendedReagentVol)
                {
                    ans |= warningFlag;
                    MinReagentAntiFlags[i] |= warningFlag;
                    //break;
                }
            }
            // 2011-09-12 sp
            // added flag codes for warning levels
            for (i = 0; i < 4; i++)
            {
                if (MinReagentCock[i] > 0 && MinReagentCock[i] < MinRecommendedReagentVol)
                {
                    ans |= warningFlag;
                    MinReagentCockFlags[i] |= warningFlag;
                    //break;
                }
            }
            // 2011-09-12 sp
            // added flag codes for warning levels
            for (i = 0; i < 4; i++)
            {
                if (MinReagentPart[i] > 0 && MinReagentPart[i] < MinRecommendedReagentVol)
                {
                    ans |= warningFlag;
                    MinReagentPartFlags[i] |= warningFlag;
                    //break;
                }
            }

           

           
            // 2011-09-12 sp
            // added flag codes for warning levels
            for (i = 0; i < 4; i++)
            {
                if ((MaxWorkingSampleMin[i] > myMaxSampleValue) ||
                                (MaxWorkingSampleMin[i] > 0 && MaxWorkingSampleMin[i] < TotalMinVol))
                {
                    ans |= errorFlag;
                    MinSampleWorkingVolFlags[i] |= errorFlag;
                    //break;
                }
            }
            // 2011-09-12 sp
            // added flag codes for warning levels
            for (i = 0; i < 4; i++)
            {
                if ((MaxWorkingSampleMax[i] > myMaxSampleValue) ||
                                (MaxWorkingSampleMax[i] > 0 && MaxWorkingSampleMax[i] < TotalMinVol))
                {
                    ans |= errorFlag;
                    MaxSampleWorkingVolFlags[i] |= errorFlag;
                    //break;
                }
            }
            // 2011-09-12 sp
            // added flag codes for warning levels
            for (i = 0; i < 4; i++)
            {
                if ((MaxWorkingSeparationMin[i] > myMaxSampleValue) ||
                                (MaxWorkingSeparationMin[i] > 0 && MaxWorkingSeparationMin[i] < TotalMinVol))
                {
                    ans |= errorFlag;
                    MinSeparationWorkingVolFlags[i] |= errorFlag;
                    //break;
                }
            }
            // 2011-09-12 sp
            // added flag codes for warning levels
            for (i = 0; i < 4; i++)
            {
                if ((MaxWorkingSeparationMax[i] > myMaxSampleValue) ||
                                (MaxWorkingSeparationMax[i] > 0 && MaxWorkingSeparationMax[i] < TotalMinVol))
                {
                    ans |= errorFlag;
                    MaxSeparationWorkingVolFlags[i] |= errorFlag;
                    //break;
                }
            }

			return ans;
		}

        public void UpateLiquidVolumeLimits()
        {
            volumeLimits.UpateLiquidVolumeLimits();
            myMaxSampleValue = (double)volumeLimits.maximumCapacity_SampleTube_uL;
            MinRecommendedReagentVol = (double)volumeLimits.minimumRecommended_ReagentTipVolume_ul;
         
        }
    }  
} 

    
