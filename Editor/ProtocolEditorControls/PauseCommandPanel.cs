
//     03/29/06 - pause command - RL (look for PauseCommand)
//
// 2011-09-05 to 2011-09-16 sp various changes
//     - provide support for use in smaller screen displays (support for scrollbar in other files)
//     - align and resize panels for more unify displays  
//
//----------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Tesla.ProtocolEditorControls
{
	/// <summary>
	/// Summary description for PauseCommandPanel.
	/// </summary>
	public class PauseCommandPanel : Tesla.ProtocolEditorControls.CommandPanel
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PauseCommandPanel()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			VisibleExtension=false;
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
            this.SuspendLayout();
            // 
            // PauseCommandPanel
            // 
            this.EnableExtension = true;
            this.Name = "PauseCommandPanel";
            this.Size = new System.Drawing.Size(532, 156);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion
	}
}
