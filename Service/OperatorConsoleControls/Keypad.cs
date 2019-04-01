//----------------------------------------------------------------------------
// Keypad
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

namespace Tesla.OperatorConsoleControls
{
	#region Delegates

	/// <summary>
	/// Event signature for notification of new characters
	/// </summary>
	public delegate void CharacterKeypressDelegate(char newChar);

	/// <summary>
	/// Event signature for notification of backspace keypress
	/// </summary>
	public delegate void BackspaceKeypressDelegate();

	/// <summary>
	/// Event signature for notification of Enter keypress
	/// </summary>
	public delegate void EnterKeypressDelegate();

	#endregion Delegates

	/// <summary>
	/// Summary description for Keypad.
	/// </summary>
	public class Keypad : System.Windows.Forms.UserControl
	{	
		#region Keypad events

		/// <summary>
		/// Report character keypress events
		/// </summary>
		public event CharacterKeypressDelegate	CharacterKeypress;

		/// <summary>
		/// Report backspace keypress events
		/// </summary>
		public event BackspaceKeypressDelegate	BackspaceKeypress;

		/// <summary>
		/// Report enter keypress events
		/// </summary>
		public event EnterKeypressDelegate		EnterKeypress;
		
		#endregion Keypad events

		#region enums

		// NOTE: for future versions, consider defining an extender provider so that the 
        // alpha/numeric choice can be made at design-time.
		public enum KeypadMode
		{
			Numeric,
			Alphabetic,
			NUM_KEYPAD_MODES
		}

		public enum CustomButtons
		{
			Star,
			Hash,
			NUM_CUSTOM_BUTTONS
		}

		#endregion enums

		// Default to numeric operation
		private KeypadMode	myKeypadMode = KeypadMode.Numeric;

		// Display strings for customisable buttons -- one string per button per mode
		private string[,]	myCustomButtonStrings;


		private System.Windows.Forms.Button btn0;
		private System.Windows.Forms.Button btn9;
		private System.Windows.Forms.Button btn4;
		private System.Windows.Forms.Button btn8;
		private System.Windows.Forms.Button btn7;
		private System.Windows.Forms.Button btn1;
		private System.Windows.Forms.Button btn3;
		private System.Windows.Forms.Button btn6;
		private System.Windows.Forms.Button btn5;
		private System.Windows.Forms.Button btnBackspace;
		private System.Windows.Forms.Button btnEnter;
		private System.Windows.Forms.Button btnAlphNumToggle;
		private System.Windows.Forms.Button btn2;
		private System.Windows.Forms.Button btnStar;
		private System.Windows.Forms.Button btnHash;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Keypad()
		{
			KeypadCommonConstructor();
		}

		public Keypad(string textStarKeyNumeric, string textStarKeyAlphabetic,
			string textHashKeyNumeric, string textHashKeyAlphabetic)
		{
			KeypadCommonConstructor();
			myCustomButtonStrings[(int)KeypadMode.Numeric, (int)CustomButtons.Star] = textStarKeyNumeric;
			myCustomButtonStrings[(int)KeypadMode.Numeric, (int)CustomButtons.Hash] = textHashKeyNumeric;
			myCustomButtonStrings[(int)KeypadMode.Alphabetic, (int)CustomButtons.Star] = textStarKeyAlphabetic;			
			myCustomButtonStrings[(int)KeypadMode.Alphabetic, (int)CustomButtons.Hash] = textHashKeyAlphabetic;
		}

		private void KeypadCommonConstructor()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// Allocate space for custom button text
			myCustomButtonStrings = new string[(int)KeypadMode.NUM_KEYPAD_MODES, (int)CustomButtons.NUM_CUSTOM_BUTTONS];
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

		public new bool Enabled
		{
			set
			{
				lock(this)
				{
					btn0.Enabled = value;
					btn1.Enabled = value;
					btn2.Enabled = value;
					btn3.Enabled = value;
					btn4.Enabled = value;
					btn5.Enabled = value;
					btn6.Enabled = value;
					btn7.Enabled = value;
					btn8.Enabled = value;
					btn9.Enabled = value;					
					btnHash.Enabled = value;
					btnBackspace.Enabled = value;
					btnEnter.Enabled = value;

					// Special cases (for now, pending further implementation)...
					btnStar.Enabled	= false;	// NOTE: reconsider this if units support is implemented
					btnAlphNumToggle.Enabled = false; // NOTE: reconsider this if alphbetic mode is implemented
				}
			}
		}

		public string TextStarKeyNumeric
		{
			set
			{
				myCustomButtonStrings[(int)KeypadMode.Numeric, (int)CustomButtons.Star] = value;
			}
		}

		public string TextStarKeyAlphabetic
		{
			set
			{
				myCustomButtonStrings[(int)KeypadMode.Alphabetic, (int)CustomButtons.Star] = value;
			}
		}

		public string TextHashKeyNumeric
		{
			set
			{
				myCustomButtonStrings[(int)KeypadMode.Alphabetic, (int)CustomButtons.Star] = value;
			}
		}

		public string TextHashKeyAlphabetic
		{
			set
			{
				myCustomButtonStrings[(int)KeypadMode.Alphabetic, (int)CustomButtons.Hash] = value;
			}
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btn0 = new System.Windows.Forms.Button();
			this.btn9 = new System.Windows.Forms.Button();
			this.btn4 = new System.Windows.Forms.Button();
			this.btn8 = new System.Windows.Forms.Button();
			this.btn7 = new System.Windows.Forms.Button();
			this.btn1 = new System.Windows.Forms.Button();
			this.btn3 = new System.Windows.Forms.Button();
			this.btn6 = new System.Windows.Forms.Button();
			this.btn5 = new System.Windows.Forms.Button();
			this.btnBackspace = new System.Windows.Forms.Button();
			this.btnEnter = new System.Windows.Forms.Button();
			this.btnAlphNumToggle = new System.Windows.Forms.Button();
			this.btn2 = new System.Windows.Forms.Button();
			this.btnStar = new System.Windows.Forms.Button();
			this.btnHash = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btn0
			// 
			this.btn0.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn0.Location = new System.Drawing.Point(72, 168);
			this.btn0.Name = "btn0";
			this.btn0.Size = new System.Drawing.Size(56, 48);
			this.btn0.TabIndex = 161;
			this.btn0.Text = "0";
			this.btn0.Click += new System.EventHandler(this.btn0_Click);
			// 
			// btn9
			// 
			this.btn9.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn9.Location = new System.Drawing.Point(144, 112);
			this.btn9.Name = "btn9";
			this.btn9.Size = new System.Drawing.Size(56, 48);
			this.btn9.TabIndex = 160;
			this.btn9.Text = "9";
			this.btn9.Click += new System.EventHandler(this.btn9_Click);
			// 
			// btn4
			// 
			this.btn4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn4.Location = new System.Drawing.Point(0, 56);
			this.btn4.Name = "btn4";
			this.btn4.Size = new System.Drawing.Size(56, 48);
			this.btn4.TabIndex = 155;
			this.btn4.Text = "4";
			this.btn4.Click += new System.EventHandler(this.btn4_Click);
			// 
			// btn8
			// 
			this.btn8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn8.Location = new System.Drawing.Point(72, 112);
			this.btn8.Name = "btn8";
			this.btn8.Size = new System.Drawing.Size(56, 48);
			this.btn8.TabIndex = 159;
			this.btn8.Text = "8";
			this.btn8.Click += new System.EventHandler(this.btn8_Click);
			// 
			// btn7
			// 
			this.btn7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn7.Location = new System.Drawing.Point(0, 112);
			this.btn7.Name = "btn7";
			this.btn7.Size = new System.Drawing.Size(56, 48);
			this.btn7.TabIndex = 158;
			this.btn7.Text = "7";
			this.btn7.Click += new System.EventHandler(this.btn7_Click);
			// 
			// btn1
			// 
			this.btn1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn1.Location = new System.Drawing.Point(0, 0);
			this.btn1.Name = "btn1";
			this.btn1.Size = new System.Drawing.Size(56, 48);
			this.btn1.TabIndex = 152;
			this.btn1.Text = "1";
			this.btn1.Click += new System.EventHandler(this.btn1_Click);
			// 
			// btn3
			// 
			this.btn3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn3.Location = new System.Drawing.Point(144, 0);
			this.btn3.Name = "btn3";
			this.btn3.Size = new System.Drawing.Size(56, 48);
			this.btn3.TabIndex = 154;
			this.btn3.Text = "3";
			this.btn3.Click += new System.EventHandler(this.btn3_Click);
			// 
			// btn6
			// 
			this.btn6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn6.Location = new System.Drawing.Point(144, 56);
			this.btn6.Name = "btn6";
			this.btn6.Size = new System.Drawing.Size(56, 48);
			this.btn6.TabIndex = 157;
			this.btn6.Text = "6";
			this.btn6.Click += new System.EventHandler(this.btn6_Click);
			// 
			// btn5
			// 
			this.btn5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn5.Location = new System.Drawing.Point(72, 56);
			this.btn5.Name = "btn5";
			this.btn5.Size = new System.Drawing.Size(56, 48);
			this.btn5.TabIndex = 156;
			this.btn5.Text = "5";
			this.btn5.Click += new System.EventHandler(this.btn5_Click);
			// 
			// btnBackspace
			// 
			this.btnBackspace.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold);
			this.btnBackspace.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnBackspace.Location = new System.Drawing.Point(224, 0);
			this.btnBackspace.Name = "btnBackspace";
			this.btnBackspace.Size = new System.Drawing.Size(56, 48);
			this.btnBackspace.TabIndex = 166;
			this.btnBackspace.Text = "←";
			this.btnBackspace.Click += new System.EventHandler(this.btnBackspace_Click);
			// 
			// btnEnter
			// 
			this.btnEnter.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnEnter.Location = new System.Drawing.Point(224, 112);
			this.btnEnter.Name = "btnEnter";
			this.btnEnter.Size = new System.Drawing.Size(56, 104);
			this.btnEnter.TabIndex = 165;
			this.btnEnter.Text = "Enter";
			this.btnEnter.Click += new System.EventHandler(this.btnEnter_Click);
			// 
			// btnAlphNumToggle
			// 
			this.btnAlphNumToggle.Enabled = false;
			this.btnAlphNumToggle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnAlphNumToggle.Location = new System.Drawing.Point(224, 56);
			this.btnAlphNumToggle.Name = "btnAlphNumToggle";
			this.btnAlphNumToggle.Size = new System.Drawing.Size(56, 48);
			this.btnAlphNumToggle.TabIndex = 164;
			this.btnAlphNumToggle.Text = "A <-> 1";
			// 
			// btn2
			// 
			this.btn2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn2.Location = new System.Drawing.Point(72, 0);
			this.btn2.Name = "btn2";
			this.btn2.Size = new System.Drawing.Size(56, 48);
			this.btn2.TabIndex = 153;
			this.btn2.Text = "2";
			this.btn2.Click += new System.EventHandler(this.btn2_Click);
			// 
			// btnStar
			// 
			this.btnStar.Enabled = false;
			this.btnStar.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnStar.Location = new System.Drawing.Point(0, 168);
			this.btnStar.Name = "btnStar";
			this.btnStar.Size = new System.Drawing.Size(56, 48);
			this.btnStar.TabIndex = 163;
			this.btnStar.Text = "mL / μL";
			this.btnStar.Click += new System.EventHandler(this.btnStar_Click);
			// 
			// btnHash
			// 
			this.btnHash.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btnHash.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnHash.Location = new System.Drawing.Point(144, 168);
			this.btnHash.Name = "btnHash";
			this.btnHash.Size = new System.Drawing.Size(56, 48);
			this.btnHash.TabIndex = 162;
			this.btnHash.Text = ".";
			this.btnHash.Click += new System.EventHandler(this.btnHash_Click);
			// 
			// Keypad
			// 
			this.Controls.Add(this.btn0);
			this.Controls.Add(this.btn9);
			this.Controls.Add(this.btn4);
			this.Controls.Add(this.btn8);
			this.Controls.Add(this.btn7);
			this.Controls.Add(this.btn1);
			this.Controls.Add(this.btn3);
			this.Controls.Add(this.btn6);
			this.Controls.Add(this.btn5);
			this.Controls.Add(this.btnBackspace);
			this.Controls.Add(this.btnEnter);
			this.Controls.Add(this.btnAlphNumToggle);
			this.Controls.Add(this.btn2);
			this.Controls.Add(this.btnStar);
			this.Controls.Add(this.btnHash);
			this.Name = "Keypad";
			this.Size = new System.Drawing.Size(280, 216);
			this.Load += new System.EventHandler(this.Keypad_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void Keypad_Load(object sender, System.EventArgs e)
		{
			switch (myKeypadMode)
			{
				case KeypadMode.Numeric:
					btn0.Text = "0";
					btn1.Text = "1";
					btn2.Text = "2";
					btn3.Text = "3";
					btn4.Text = "4";
					btn5.Text = "5";
					btn6.Text = "6";
					btn7.Text = "7";
					btn8.Text = "8";
					btn9.Text = "9";
					btnStar.Text = myCustomButtonStrings[(int)KeypadMode.Numeric, (int)CustomButtons.Star];
					btnHash.Text = myCustomButtonStrings[(int)KeypadMode.Numeric, (int)CustomButtons.Hash];					 
					break;
				case KeypadMode.Alphabetic:
					break;
			}
		}

		private void FireCharacterKeypress(char keyChar)
		{
			if (CharacterKeypress != null)
			{
				CharacterKeypress(keyChar);
			}
		}

		private void FireBackspaceKeypress()
		{
			if (BackspaceKeypress != null)
			{
				BackspaceKeypress();
			}
		}

		private void FireEnterKeypress()
		{
			if (EnterKeypress != null)
			{
				EnterKeypress();
			}
		}

		private void CharacterButtonKeypress(System.Windows.Forms.Button button)
		{
			if (button.Text == string.Empty)
			{
				return;
			}

			switch(myKeypadMode)
			{
				case KeypadMode.Numeric:
					FireCharacterKeypress(button.Text[0]);					
					break;
				case KeypadMode.Alphabetic:
					// NOTE: To be considered if alphabetic support is ever required.
                    // Count successive clicks from any one button, modulo the
					// number of alphabetic characters in the button text.  When the next 
					// (different) button click arrives, "accept" the last entry for the
					// alphabetic character.  Note this is similar to alphabetic character
					// entry schemes typical of mobile phones, but slightly different.
					break;
			}
		}

		private void btn0_Click(object sender, System.EventArgs e)
		{
			CharacterButtonKeypress(btn0);
		}	

		private void btn1_Click(object sender, System.EventArgs e)
		{
			CharacterButtonKeypress(btn1);
		}

		private void btn2_Click(object sender, System.EventArgs e)
		{
			CharacterButtonKeypress(btn2);
		}

		private void btn3_Click(object sender, System.EventArgs e)
		{
			CharacterButtonKeypress(btn3);
		}

		private void btn4_Click(object sender, System.EventArgs e)
		{
			CharacterButtonKeypress(btn4);
		}

		private void btn5_Click(object sender, System.EventArgs e)
		{
			CharacterButtonKeypress(btn5);
		}

		private void btn6_Click(object sender, System.EventArgs e)
		{
			CharacterButtonKeypress(btn6);
		}

		private void btn7_Click(object sender, System.EventArgs e)
		{
			CharacterButtonKeypress(btn7);
		}

		private void btn8_Click(object sender, System.EventArgs e)
		{
			CharacterButtonKeypress(btn8);
		}

		private void btn9_Click(object sender, System.EventArgs e)
		{
			CharacterButtonKeypress(btn9);
		}

		private void btnStar_Click(object sender, System.EventArgs e)
		{
			// NOTE: implement if alphabetic support is ever required.
		}	

		private void btnHash_Click(object sender, System.EventArgs e)
		{
			CharacterButtonKeypress(btnHash);
		}

		private void btnBackspace_Click(object sender, System.EventArgs e)
		{
			FireBackspaceKeypress();
		}

		private void btnEnter_Click(object sender, System.EventArgs e)
		{
			FireEnterKeypress();
		}			
	}
}
