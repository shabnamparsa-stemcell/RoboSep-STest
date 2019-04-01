//----------------------------------------------------------------------------
// QuadrantSelector
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
using System.Data;
using System.Windows.Forms;

using Tesla.Common.Separator;

namespace Tesla.OperatorConsoleControls
{
	#region Event delegates

    public delegate void StateChangedDelegate(QuadrantId selectedQuadrant,
        SeparatorState quadrantState);

	public delegate void SelectionChangedDelegate(QuadrantId selectedQuadrant);

	#endregion Event delegates

	/// <summary>
	/// Display carousel quadrant status and allow quadrant selection
	/// </summary>
	public class QuadrantSelector : System.Windows.Forms.UserControl
	{
        private QuadrantId          mySelectedQuadrant = QuadrantId.NoQuadrant;
        private SeparatorState[]    myQuadrantStates = new SeparatorState[(int)QuadrantId.NUM_QUADRANTS];

        private System.Windows.Forms.Button btnQ1;
        private System.Windows.Forms.Button btnQ2;
        private System.Windows.Forms.Button btnQ3;
        private System.Windows.Forms.Button btnQ4;
        private System.Windows.Forms.Label lblQ1;
        private System.Windows.Forms.Label lblQ4;
        private System.Windows.Forms.Label lblQ3;
        private System.Windows.Forms.Label lblQ2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        public event StateChangedDelegate		QuadrantStateChanged;

		public event SelectionChangedDelegate	QuadrantSelectionChanged;

		public QuadrantSelector()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

            // Initialise the quadrant states
            for (QuadrantId q = QuadrantId.Quadrant1;
                q < QuadrantId.NUM_QUADRANTS; ++q)
            {
                myQuadrantStates[(int)q] = SeparatorState.SeparatorUnloaded;
            }
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
					components.Dispose();
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Get or set the currently selected quadrant.
		/// </summary>
        public QuadrantId SelectedQuadrant
        {
            get
            {
                lock(this)
                {
                    return mySelectedQuadrant;
                }
            }
            set
            {
                SelectQuadrant(value);
            }
        }

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.btnQ1 = new System.Windows.Forms.Button();
            this.btnQ2 = new System.Windows.Forms.Button();
            this.btnQ3 = new System.Windows.Forms.Button();
            this.btnQ4 = new System.Windows.Forms.Button();
            this.lblQ1 = new System.Windows.Forms.Label();
            this.lblQ4 = new System.Windows.Forms.Label();
            this.lblQ3 = new System.Windows.Forms.Label();
            this.lblQ2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnQ1
            // 
            this.btnQ1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.btnQ1.Location = new System.Drawing.Point(148, 0);
            this.btnQ1.Name = "btnQ1";
            this.btnQ1.Size = new System.Drawing.Size(40, 40);
            this.btnQ1.TabIndex = 8;
            this.btnQ1.Text = "1";
            this.btnQ1.Click += new System.EventHandler(this.btnQ1_Click);
            // 
            // btnQ2
            // 
            this.btnQ2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.btnQ2.Location = new System.Drawing.Point(100, 0);
            this.btnQ2.Name = "btnQ2";
            this.btnQ2.Size = new System.Drawing.Size(40, 40);
            this.btnQ2.TabIndex = 9;
            this.btnQ2.Text = "2";
            this.btnQ2.Click += new System.EventHandler(this.btnQ2_Click);
            // 
            // btnQ3
            // 
            this.btnQ3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.btnQ3.Location = new System.Drawing.Point(100, 48);
            this.btnQ3.Name = "btnQ3";
            this.btnQ3.Size = new System.Drawing.Size(40, 40);
            this.btnQ3.TabIndex = 11;
            this.btnQ3.Text = "3";
            this.btnQ3.Click += new System.EventHandler(this.btnQ3_Click);
            // 
            // btnQ4
            // 
            this.btnQ4.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.btnQ4.Location = new System.Drawing.Point(148, 48);
            this.btnQ4.Name = "btnQ4";
            this.btnQ4.Size = new System.Drawing.Size(40, 40);
            this.btnQ4.TabIndex = 10;
            this.btnQ4.Text = "4";
            this.btnQ4.Click += new System.EventHandler(this.btnQ4_Click);
            // 
            // lblQ1
            // 
            this.lblQ1.Location = new System.Drawing.Point(192, 0);
            this.lblQ1.Name = "lblQ1";
            this.lblQ1.Size = new System.Drawing.Size(96, 40);
            this.lblQ1.TabIndex = 12;
            this.lblQ1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblQ4
            // 
            this.lblQ4.Location = new System.Drawing.Point(192, 48);
            this.lblQ4.Name = "lblQ4";
            this.lblQ4.Size = new System.Drawing.Size(96, 40);
            this.lblQ4.TabIndex = 13;
            this.lblQ4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblQ3
            // 
            this.lblQ3.Location = new System.Drawing.Point(0, 48);
            this.lblQ3.Name = "lblQ3";
            this.lblQ3.Size = new System.Drawing.Size(96, 40);
            this.lblQ3.TabIndex = 15;
            this.lblQ3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblQ2
            // 
            this.lblQ2.Location = new System.Drawing.Point(0, 0);
            this.lblQ2.Name = "lblQ2";
            this.lblQ2.Size = new System.Drawing.Size(96, 40);
            this.lblQ2.TabIndex = 14;
            this.lblQ2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // QuadrantSelector
            // 
            this.Controls.Add(this.lblQ3);
            this.Controls.Add(this.lblQ2);
            this.Controls.Add(this.lblQ4);
            this.Controls.Add(this.lblQ1);
            this.Controls.Add(this.btnQ3);
            this.Controls.Add(this.btnQ4);
            this.Controls.Add(this.btnQ2);
            this.Controls.Add(this.btnQ1);
            this.Name = "QuadrantSelector";
            this.Size = new System.Drawing.Size(288, 88);
            this.ResumeLayout(false);

        }
		#endregion

        private void btnQ1_Click(object sender, System.EventArgs e)
        {
            SelectQuadrant(QuadrantId.Quadrant1);
        }

        private void btnQ2_Click(object sender, System.EventArgs e)
        {
            SelectQuadrant(QuadrantId.Quadrant2);
        }

        private void btnQ3_Click(object sender, System.EventArgs e)
        {
            SelectQuadrant(QuadrantId.Quadrant3);
        }

        private void btnQ4_Click(object sender, System.EventArgs e)
        {
            SelectQuadrant(QuadrantId.Quadrant4);
        }

        public void SelectQuadrant(QuadrantId selectedQuadrant)
        {
            lock(this)
            {
                // Select the indicated quadrant; deselect the others
                for (QuadrantId q = QuadrantId.Quadrant1; q < QuadrantId.NUM_QUADRANTS; ++q)
                {
                    FlatStyle style = (q == selectedQuadrant) ?
                        FlatStyle.Flat : FlatStyle.Standard;

                    switch (q)
                    {
                        case QuadrantId.Quadrant1:
                            btnQ1.FlatStyle = style;
                            break;
                        case QuadrantId.Quadrant2:
                            btnQ2.FlatStyle = style;
                            break;
                        case QuadrantId.Quadrant3:
                            btnQ3.FlatStyle = style;
                            break;
                        case QuadrantId.Quadrant4:
                            btnQ4.FlatStyle = style;
                            break;
                    }
                }

                // Update local selection state variable
				QuadrantId previouslySelectedQuadrant = mySelectedQuadrant;
                mySelectedQuadrant = selectedQuadrant;

                // Notify interested parties of the selection change
                if (QuadrantSelectionChanged != null && 
					selectedQuadrant != QuadrantId.NoQuadrant)
                {
                    QuadrantSelectionChanged(selectedQuadrant);
                }

				// We must also notify interested parties if the change in selection
				// means we have changed quadrant state				
				if (QuadrantStateChanged != null)
				{
					if ((selectedQuadrant != QuadrantId.NoQuadrant &&
						previouslySelectedQuadrant == QuadrantId.NoQuadrant)	
						||
						(myQuadrantStates[(int)previouslySelectedQuadrant] != 
						myQuadrantStates[(int)selectedQuadrant]))				
					{
						QuadrantStateChanged(selectedQuadrant, 
							myQuadrantStates[(int)selectedQuadrant]);
					}
				}
            }
        }

        public void SetQuadrantState(QuadrantId quadrantId, SeparatorState state)
        {
            lock(this)
            {
                // Record the new quadrant state
                myQuadrantStates[(int)quadrantId] = state;
				
				string stateDescription = string.Empty;
				switch(state)
				{
					case SeparatorState.Configured:
					case SeparatorState.Selected:
						stateDescription = state.ToString();
						break;
					case SeparatorState.NoSample:
                        // NOTE: this state is largely a leftover from the CDP revision of
                        // the instrument and is not used in the beta GUI.  If this changes,
                        // get its state description strings from string services instead
                        // (that is, include this string in the common resources so it can
                        // be internationalised).
						stateDescription = "Volume?";
						break;
				}

                // Update the quadrant state display
                switch (quadrantId)
                {
                    case QuadrantId.Quadrant1:
                        lblQ1.Text = stateDescription;
                        break;
                    case QuadrantId.Quadrant2:
                        lblQ2.Text = stateDescription;
                        break;
                    case QuadrantId.Quadrant3:
                        lblQ3.Text = stateDescription;
                        break;
                    case QuadrantId.Quadrant4:
                        lblQ4.Text = stateDescription;
                        break;
                }

                // Notify interested parties of the change
                if (QuadrantStateChanged != null)
                {
                    QuadrantStateChanged(quadrantId, state);
                }
            }
        }

		public SeparatorState GetQuadrantState(QuadrantId quadrantId)
		{
			lock(this)
			{
				return myQuadrantStates[(int)quadrantId];
			}
		}
	}
}
