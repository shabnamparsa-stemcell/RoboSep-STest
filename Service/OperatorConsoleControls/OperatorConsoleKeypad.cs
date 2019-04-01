//----------------------------------------------------------------------------
// OperatorConsoleKeypad
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
using System.Threading;
using System.Globalization;

using Tesla.Common;
using Tesla.Common.Separator;

namespace Tesla.OperatorConsoleControls
{
	#region Delegates

	/// <summary>
	/// Event signature for notification of newly entered sample volume
	/// </summary>
	public delegate void SampleVolumeDelegate(FluidVolume sampleVolume);

	#endregion Delegates

	/// <summary>
	/// Summary description for OperatorConsoleKeypad.
	/// </summary>
	public class OperatorConsoleKeypad : System.Windows.Forms.UserControl
	{		
		#region OperatorConsoleKeypad events

		/// <summary>
		/// Report a newly entered or updated sample volume
		/// </summary>
		public event SampleVolumeDelegate UpdateSampleVolume;

		#endregion OperatorConsoleKeypad events

		private Keypad.KeypadMode		myKeypadMode;
		private FluidVolume				myKeypadSampleVolume;

		private System.Windows.Forms.TextBox txtKeypad;
		private Tesla.OperatorConsoleControls.Keypad myKeypad;
		private System.Windows.Forms.Label lblKeypad;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public OperatorConsoleKeypad()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();			

			// For now, fix the operating mode to numeric
			// NOTE: future versions should allow for alpha/numeric operating mode and 
            // possibly an extender provider to allow design-time configuration.
			myKeypadMode = Keypad.KeypadMode.Numeric;

			// Create a fluid volume object to hold the entered sample volume
			myKeypadSampleVolume = new FluidVolume(0.0d, FluidVolumeUnit.MicroLitres);

			// Set the initial modal strings for the generic keypad's customisable keys
			// NOTE: in future versions (that allow alphabetic processing) store these
            // customised labels with the other resources in order to allow for 
            // internationalisation (and localisation).
			myKeypad.TextStarKeyNumeric = "mL / µL";
			myKeypad.TextStarKeyAlphabetic = "CAPS";
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

		#region UserControl Load

		private void OperatorConsoleKeypad_Load(object sender, System.EventArgs e)
		{			
			// Show selection highlight at all times when it's active
			txtKeypad.HideSelection = false;

			// Register for keypad events
			myKeypad.CharacterKeypress += new CharacterKeypressDelegate(WhenCharacterKeypress);
			myKeypad.BackspaceKeypress += new BackspaceKeypressDelegate(WhenBackspaceKeypress);
			myKeypad.EnterKeypress += new EnterKeypressDelegate(WhenEnterKeypress);
		}

		#endregion UserControl Load

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.txtKeypad = new System.Windows.Forms.TextBox();
			this.lblKeypad = new System.Windows.Forms.Label();
			this.myKeypad = new Tesla.OperatorConsoleControls.Keypad();
			this.SuspendLayout();
			// 
			// txtKeypad
			// 
			this.txtKeypad.Enabled = false;
			this.txtKeypad.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.txtKeypad.Location = new System.Drawing.Point(4, 8);
			this.txtKeypad.MaxLength = 15;
			this.txtKeypad.Name = "txtKeypad";
			this.txtKeypad.ReadOnly = true;
			this.txtKeypad.Size = new System.Drawing.Size(200, 26);
			this.txtKeypad.TabIndex = 153;
			this.txtKeypad.Text = "";
			this.txtKeypad.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtKeypad.ModifiedChanged += new System.EventHandler(this.txtKeypad_ModifiedChanged);
			// 
			// lblKeypad
			// 
			this.lblKeypad.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lblKeypad.Location = new System.Drawing.Point(220, 8);
			this.lblKeypad.Name = "lblKeypad";
			this.lblKeypad.Size = new System.Drawing.Size(56, 23);
			this.lblKeypad.TabIndex = 154;
			this.lblKeypad.Text = "µL";
			this.lblKeypad.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// myKeypad
			// 
			this.myKeypad.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.myKeypad.Location = new System.Drawing.Point(0, 40);
			this.myKeypad.Name = "myKeypad";
			this.myKeypad.Size = new System.Drawing.Size(280, 216);
			this.myKeypad.TabIndex = 155;
			// 
			// OperatorConsoleKeypad
			// 
			this.Controls.Add(this.myKeypad);
			this.Controls.Add(this.txtKeypad);
			this.Controls.Add(this.lblKeypad);
			this.Name = "OperatorConsoleKeypad";
			this.Size = new System.Drawing.Size(280, 256);
			this.Load += new System.EventHandler(this.OperatorConsoleKeypad_Load);
			this.ResumeLayout(false);

		}
		#endregion

		#region OperatorConsoleKeyboard behaviour

		public new bool Enabled
		{
			set
			{
				lock(this)
				{
					myKeypad.Enabled	= value;				
					txtKeypad.Enabled	= value;
					lblKeypad.Enabled	= value;
					base.Enabled		= value;
				}
			}
		}

		public new string Text
		{
			set
			{				
				try
				{
					lock(this)
					{
						if (myKeypadMode == Keypad.KeypadMode.Numeric)
						{	
							// Verify that the supplied string represents a double and so
							// is presentable in fixed decimal format
							double validatedNumber = double.Parse(value);							
							FormatNumericKeypadValue(ref validatedNumber);
							txtKeypad.Modified = false;
						}
						else if (myKeypadMode == Keypad.KeypadMode.Alphabetic)
						{
							// Verify that all characters in the string a alphanumeric
							foreach (char character in value)
							{
								if ( ! char.IsLetterOrDigit(character))
								{
									throw new ApplicationException();
								}
							}
							txtKeypad.Text = value;
						}
					}
				}
				catch
				{
					// Ignore supplied input if validation fails
				}
			}
		}

		public void Clear()
		{
			lock(this)
			{
				txtKeypad.Text = string.Empty;
				txtKeypad.Modified = false;
			}
		}

		private void FormatNumericKeypadValue(ref double keypadValue)
		{			
			// Format the supplied value to a given precision
			txtKeypad.Text = string.Format("{0:F3}", keypadValue);
			// Update the actual value to match the given precision
			keypadValue = double.Parse(txtKeypad.Text);
		}

		#endregion OperatorConsoleKeyboard behaviour

		#region Keypad event handlers

		private void WhenCharacterKeypress(char newChar)
		{
			
			if (myKeypadMode == Keypad.KeypadMode.Numeric)
			{
				// Allow only up to the 'max length' number of digits
				if (txtKeypad.Text.Length == txtKeypad.MaxLength)
				{
					return;
				}

				// Ignore numeric entries that start with zero unless the user is specifying
				// a valid decimal fraction
				if (txtKeypad.Text == "0" && newChar != '.')
				{
					return;
				}

				// NOTE: future versions should also check that we only ever allow one decimal 
                // point (allow it to be added once and reset the condition if the backspace 
                // passes back over the decimal position).
			}
			txtKeypad.Text += newChar;
			txtKeypad.Modified = true;
		}

		private void WhenBackspaceKeypress()
		{
			lock(this)
			{
				int textLength = txtKeypad.Text.Length;
				if (textLength > 0)
				{
					txtKeypad.Text = txtKeypad.Text.Substring(0, textLength-1);
					txtKeypad.Modified = true;
				}
			}
		}

		private void WhenEnterKeypress()
		{
			lock(this)
			{
				txtKeypad.Modified = false;
				switch (myKeypadMode)
				{
					case Keypad.KeypadMode.Numeric:					
						try
						{
							double amount = double.Parse(txtKeypad.Text, Thread.CurrentThread.CurrentCulture);
							myKeypadSampleVolume.Amount = amount;
							myKeypadSampleVolume.Unit   = FluidVolumeUnit.MicroLitres;							
						}
						catch
						{
							// Ignore any input that isn't parseable as a double							
							myKeypadSampleVolume.Amount = 0.0d;
						}

						// Reformat the entry to help show it's changed (note this may change
						// the precision of the entered value to round it off to a "sensible" 
						// number of digits).
						double keypadSampleVolume = myKeypadSampleVolume.Amount;
						FormatNumericKeypadValue(ref keypadSampleVolume);
						myKeypadSampleVolume.Amount = keypadSampleVolume;

						// Notify interested parties the sample volume has changed
						if (UpdateSampleVolume != null)
						{
							UpdateSampleVolume(myKeypadSampleVolume);
						}
						break;
				}
			}
		}

		#endregion Keypad event handlers

		private void txtKeypad_ModifiedChanged(object sender, System.EventArgs e)
		{			
			if (txtKeypad.Modified)
			{
				txtKeypad.SelectionLength = txtKeypad.Text.Length;
			}
			else
			{
				txtKeypad.SelectionLength = 0;
			}
			txtKeypad.Invalidate();
		}
	}
}
