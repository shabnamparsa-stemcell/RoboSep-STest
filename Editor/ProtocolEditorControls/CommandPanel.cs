
//     03/29/06 - pause command - RL 
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
using System.Data;
using System.Windows.Forms;

using Tesla.ProtocolEditorModel;
using Tesla.Common.Protocol;

namespace Tesla.ProtocolEditorControls
{

	/// <summary>
	/// Summary description for CommandPanel.
	/// </summary>
	public class CommandPanel : System.Windows.Forms.UserControl
	{
        private ProtocolCommandLabel txtCommandLabel;
		private System.Windows.Forms.Label lblCommandLabel;
		private System.Windows.Forms.Label lblCommandExtensionTime;
        private System.Windows.Forms.TextBox txtCommandExtensionTime;
        private System.Windows.Forms.ErrorProvider errorCommandLabel;
        private System.Windows.Forms.ErrorProvider errorCommandExtensionTime;
        private System.Windows.Forms.TextBox txtCommandType;
        private System.Windows.Forms.ToolTip tipCommandLabel;
		private System.ComponentModel.IContainer components;		

        #region Local Types

		public enum PanelType
		{
			CommandPanel = 0,				// 'Base' panel type, and used for HomeAllCommand
			DemoCommandPanel,
			IncubateCommandPanel,
			SeparateCommandPanel,
			PauseCommandPanel,
			VolumeCommandPanel,				// 'Base' type for Volume commands, and used for MixCommand
			WorkingVolumeCommandPanel,		// 'Base' type for Volume commands that specify free air dispense and optionally a working volume
			VolumeMaintenanceCommandPanel,	// 'Base' type for Volume Maintenance commands
            TopUpMixTransSepTransCommandPanel,
            TopUpMixTransCommandPanel, 
            TopUpTransSepTransCommandPanel,
            TopUpTransCommandPanel,
            ResusMixSepTransCommandPanel,
            ResusMixCommandPanel,
            MixTransCommandPanel, 
			NUM_COMMAND_PANEL_TYPES,
			START_TYPE	= CommandPanel,
		}

        #endregion Local Types

        #region Events

        public delegate void CommandDetailChangedDelegate();

        public static event CommandDetailChangedDelegate   CommandDetailChanged;

        protected void ReportCommandDetailChanged()
        {
            if (CommandDetailChanged != null)
            {
                CommandDetailChanged();
            }
        }

        #endregion Events

        #region Construction/destruction

		public CommandPanel()
		{
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

			//setup tool tip
    		tipCommandLabel.SetToolTip(this.lblCommandLabel, "Step description.");
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

		#region Properties

        public virtual bool IsContentValid()
        {
            return txtCommandLabel.TextLength > 0           &&
                    txtCommandExtensionTime.TextLength > 0  &&
                    myExtensionTime > -1;
        }

        public string CommandType
        {
            set
            {
                txtCommandType.Text = value;
            }
			get
			{
				return txtCommandType.Text;
			}
        }

      	public bool EnableExtension
		{
			set
			{
				txtCommandExtensionTime.Enabled = value;
				lblCommandExtensionTime.Enabled = value;
			}
			get
			{
				return txtCommandExtensionTime.Enabled && lblCommandExtensionTime.Enabled;
			}
		}

		// RL - pause command - 03/29/06
		// disable extension time
		public bool VisibleExtension
		{
			set
			{
				txtCommandExtensionTime.Visible = value;
				lblCommandExtensionTime.Visible = value;
			}
			get
			{
				return txtCommandExtensionTime.Visible && lblCommandExtensionTime.Visible;
			}
		}

		public string CommandLabel
		{
			get
			{
				return txtCommandLabel.Text;
			}
			set
			{
				txtCommandLabel.Text = value;
			}
		}

        private int myExtensionTime = -1;

		public uint CommandExtensionTime
		{
			get
			{
				return myExtensionTime < 0 ? (uint)0 : (uint)myExtensionTime;
			}
			set
			{
                // Set the extension time to the supplied value.  NOTE: the 
                // 'text changed' handler also checks to see if the new value
                // is valid.
				txtCommandExtensionTime.Text = value.ToString();
			}
		}

		public void SetUseBufferTip(bool flg)
		{
		}


		

		#endregion Properties

        #region Data Entry Error Indicators

        private void txtCommandLabel_TextChanged(object sender, System.EventArgs e)
        {
            ReportCommandDetailChanged();
            if (txtCommandLabel.TextLength == 0)
            {
                ShowCommandLabelError();
            }
            else
            {
                ClearCommandLabelError();
            }
        }

        private void ShowCommandLabelError()
        {
            errorCommandLabel.SetIconAlignment(txtCommandLabel, ErrorIconAlignment.MiddleLeft);
            errorCommandLabel.SetError(txtCommandLabel, "Command label required");
        }

        private void ClearCommandLabelError()
        {
            errorCommandLabel.SetError(txtCommandLabel, string.Empty);
        }

        private void txtCommandExtensionTime_TextChanged(object sender, System.EventArgs e)
        {
            ReportCommandDetailChanged();
            if (txtCommandExtensionTime.TextLength == 0)
            {
                ShowCommandExtensionTimeError();
            }
            else
            {
                try
                {
                    myExtensionTime = int.Parse(txtCommandExtensionTime.Text);
                    if (myExtensionTime < 0)
                    {
                        ShowCommandExtensionTimeError();
                    }
                    else
                    {
                        ClearCommandExtensionTimeError();
                    }
                }
                catch
                {
                    myExtensionTime = -1;
                }
            }
        }

        private void ShowCommandExtensionTimeError()
        {
            errorCommandExtensionTime.SetIconAlignment(
                txtCommandExtensionTime, ErrorIconAlignment.MiddleLeft);
            errorCommandExtensionTime.SetError(
                txtCommandExtensionTime, "Command extension time must be >= 0");
        }

        private void ClearCommandExtensionTimeError()
        {
            errorCommandExtensionTime.SetError(txtCommandExtensionTime, string.Empty);
        }

		protected void CommandPanel_VisibleChanged(object sender, System.EventArgs e)
		{
			if (this.Visible)		 
			{
				// Trigger re-evaluation of the error providers
				txtCommandLabel_TextChanged(sender, e);
				txtCommandExtensionTime_TextChanged(sender, e);
				this.ReportCommandDetailChanged();
			}
		}

        #endregion Data Entry Error Indicators

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.txtCommandLabel = new ProtocolCommandLabel();
            this.lblCommandLabel = new System.Windows.Forms.Label();
            this.lblCommandExtensionTime = new System.Windows.Forms.Label();
            this.txtCommandExtensionTime = new System.Windows.Forms.TextBox();
            this.errorCommandLabel = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorCommandExtensionTime = new System.Windows.Forms.ErrorProvider(this.components);
            this.txtCommandType = new System.Windows.Forms.TextBox();
            this.tipCommandLabel = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorCommandLabel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorCommandExtensionTime)).BeginInit();
            this.SuspendLayout();
            // 
            // txtCommandLabel
            // 
            this.txtCommandLabel.Location = new System.Drawing.Point(228, 4);
            this.txtCommandLabel.Name = "txtCommandLabel";
            this.txtCommandLabel.Size = new System.Drawing.Size(296, 20);
            this.txtCommandLabel.TabIndex = 3;
            this.txtCommandLabel.TextChanged += new System.EventHandler(this.txtCommandLabel_TextChanged);
            // 
            // lblCommandLabel
            // 
            this.lblCommandLabel.Location = new System.Drawing.Point(186, 4);
            this.lblCommandLabel.Name = "lblCommandLabel";
            this.lblCommandLabel.Size = new System.Drawing.Size(40, 20);
            this.lblCommandLabel.TabIndex = 2;
            this.lblCommandLabel.Text = "Label";
            this.lblCommandLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCommandExtensionTime
            // 
            this.lblCommandExtensionTime.Enabled = false;
            this.lblCommandExtensionTime.Location = new System.Drawing.Point(186, 30);
            this.lblCommandExtensionTime.Name = "lblCommandExtensionTime";
            this.lblCommandExtensionTime.Size = new System.Drawing.Size(104, 20);
            this.lblCommandExtensionTime.TabIndex = 4;
            this.lblCommandExtensionTime.Text = "Extension time (s)";
            this.lblCommandExtensionTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtCommandExtensionTime
            // 
            this.txtCommandExtensionTime.Enabled = false;
            this.txtCommandExtensionTime.Location = new System.Drawing.Point(296, 30);
            this.txtCommandExtensionTime.Name = "txtCommandExtensionTime";
            this.txtCommandExtensionTime.Size = new System.Drawing.Size(112, 20);
            this.txtCommandExtensionTime.TabIndex = 5;
            this.txtCommandExtensionTime.TextChanged += new System.EventHandler(this.txtCommandExtensionTime_TextChanged);
            // 
            // errorCommandLabel
            // 
            this.errorCommandLabel.ContainerControl = this;
            // 
            // errorCommandExtensionTime
            // 
            this.errorCommandExtensionTime.ContainerControl = this;
            // 
            // txtCommandType
            // 
            this.txtCommandType.Location = new System.Drawing.Point(16, 4);
            this.txtCommandType.Name = "txtCommandType";
            this.txtCommandType.ReadOnly = true;
            this.txtCommandType.Size = new System.Drawing.Size(148, 20);
            this.txtCommandType.TabIndex = 7;
            // 
            // CommandPanel
            // 
            this.Controls.Add(this.txtCommandType);
            this.Controls.Add(this.txtCommandExtensionTime);
            this.Controls.Add(this.lblCommandExtensionTime);
            this.Controls.Add(this.txtCommandLabel);
            this.Controls.Add(this.lblCommandLabel);
            this.Name = "CommandPanel";
            this.Size = new System.Drawing.Size(532, 156);
            this.VisibleChanged += new System.EventHandler(this.CommandPanel_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.errorCommandLabel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorCommandExtensionTime)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion
	}
}
