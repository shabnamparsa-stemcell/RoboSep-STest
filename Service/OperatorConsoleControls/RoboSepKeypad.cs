//----------------------------------------------------------------------------
// RoboSepKeypad
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

using Tesla.Common.ResourceManagement;

namespace Tesla.OperatorConsoleControls
{
	/// <summary>
	/// Simple numeric keypad including dialog OK/Cancel semantics.
	/// </summary>
	public class RoboSepKeypad : System.Windows.Forms.UserControl
	{
        #region Delegates

        /// <summary>
        /// Event signature for notification of new characters
        /// </summary>
        public delegate void CharacterKeypressDelegate(char newChar);

        /// <summary>
        /// Event signature for notification of Clear keypress
        /// </summary>
        public delegate void ClearKeypressDelegate();

        /// <summary>
        /// Event signature for notification of Cancel keypress
        /// </summary>
        public delegate void CancelKeypressDelegate();

        /// <summary>
        /// Event signature for notification of Enter keypress
        /// </summary>
        public delegate void EnterKeypressDelegate();

        #endregion Delegates

        #region Keypad events

        /// <summary>
        /// Report character keypress events
        /// </summary>
        public event CharacterKeypressDelegate	CharacterKeypress;

        /// <summary>
        /// Report clear keypress events
        /// </summary>
        public event ClearKeypressDelegate	    ClearKeypress;

        /// <summary>
        /// Report cancel keypress events
        /// </summary>
        public event CancelKeypressDelegate	    CancelKeypress;

        /// <summary>
        /// Report enter keypress events
        /// </summary>
        public event EnterKeypressDelegate		EnterKeypress;
		
        #endregion Keypad events

        #region Enums

        public enum KeypadMode
        {
            Numeric,
            // Future support may be added for 'Alphabetic' mode (similar to a mobile phone keypad)
            NUM_KEYPAD_MODES
        }

        public enum CustomButtons
        {
            Star,
            Hash,
            NUM_CUSTOM_BUTTONS
        }

        #endregion Enums

        // Default to numeric operation
        private KeypadMode	myKeypadMode = KeypadMode.Numeric;

		private bool myEnabled = true;
		// When in Numeric+Integer mode, disable the '.' (decimal point or equivalent
		// separation character), restricting number entry to integers
		private bool myIntegerMode = false;

        // Display strings for customisable buttons -- one string per button per mode
        private string[,]	myCustomButtonStrings;

        private RoboSepButton btn0;
        private RoboSepButton btn1;
        private RoboSepButton btn2;
        private RoboSepButton btn3;
        private RoboSepButton btn4;
        private RoboSepButton btn5;
        private RoboSepButton btn6;
        private RoboSepButton btn7;
        private RoboSepButton btn8;
        private RoboSepButton btn9;
        private RoboSepButton btnStar;
        private RoboSepButton btnHash;
        private RoboSepButton btnClear;
        private RoboSepButton btnCancel;
        private RoboSepButton btnEnter;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#region Construction/destruction

        public RoboSepKeypad()
        {
            KeypadCommonConstructor();
        }

        public RoboSepKeypad(string textStarKeyNumeric, string textStarKeyAlphabetic,
            string textHashKeyNumeric, string textHashKeyAlphabetic)
        {
            KeypadCommonConstructor();
            myCustomButtonStrings[(int)KeypadMode.Numeric, (int)CustomButtons.Star] = textStarKeyNumeric;
            myCustomButtonStrings[(int)KeypadMode.Numeric, (int)CustomButtons.Hash] = textHashKeyNumeric;
        }

        private void KeypadCommonConstructor()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // Allocate space for custom button text
            myCustomButtonStrings = new string[(int)KeypadMode.NUM_KEYPAD_MODES, (int)CustomButtons.NUM_CUSTOM_BUTTONS];

			// Get localised strings for the alphabetic cases
			btnCancel.Text = SeparatorResourceManager.GetSeparatorString(StringId.KeypadCancelText);
			btnClear.Text = SeparatorResourceManager.GetSeparatorString(StringId.KeypadClearText);
			btnEnter.Text = SeparatorResourceManager.GetSeparatorString(StringId.KeypadEnterText);
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

        public new bool Enabled
        {
            set
            {
                lock(this)
                {
                    myEnabled = value;

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
                    btnHash.Enabled = myIntegerMode;
                    btnClear.Enabled = value;
                    btnCancel.Enabled = value;
                    btnEnter.Enabled = value;

                    // Special cases (for now, pending further implementation)...
                    btnStar.Enabled	= false;	// NOTE: reconsider if an alphbetic mode is implemented
                }
            }
        }

		private Color myButtonColour;

		public Color ButtonColor
		{
			get
			{
				return myButtonColour;
			}
			set
			{
				lock (this)
				{
					myButtonColour = value;
					btn0.BackColor = value;
					btn1.BackColor = value;
					btn2.BackColor = value;
					btn3.BackColor = value;
					btn4.BackColor = value;
					btn5.BackColor = value;
					btn6.BackColor = value;
					btn7.BackColor = value;
					btn8.BackColor = value;
					btn9.BackColor = value;					
					btnStar.BackColor = value;
					btnHash.BackColor = value;
					btnClear.BackColor = value;
					btnCancel.BackColor = value;
					btnEnter.BackColor = value;
				}
			}
		}

		public bool IntegerModeNumeric
		{
			set
			{
				myIntegerMode = value;
				if (myEnabled)
				{
					btnHash.Enabled = ! value;
				}
			}
		}

        public string TextStarKeyNumeric
        {
            set
            {
                myCustomButtonStrings[(int)KeypadMode.Numeric, (int)CustomButtons.Star] = value;
				btnStar.Text = value;
				btnStar.Enabled = (value != string.Empty);

            }
        }

// NOTE: reconsider if an alphbetic mode is implemented
//        public string TextStarKeyAlphabetic
//        {
//            set
//            {
//                myCustomButtonStrings[(int)KeypadMode.Alphabetic, (int)CustomButtons.Star] = value;
//				  btnStar.Text = value;
//            }
//        }

        public string TextHashKeyNumeric
        {
            set
            {
                myCustomButtonStrings[(int)KeypadMode.Numeric, (int)CustomButtons.Star] = value;
				btnHash.Text = value;
				btnHash.Enabled = (value != string.Empty);
            }
        }

// NOTE: reconsider if an alphbetic mode is implemented
//        public string TextHashKeyAlphabetic
//        {
//            set
//            {
//                myCustomButtonStrings[(int)KeypadMode.Alphabetic, (int)CustomButtons.Hash] = value;
//                btnHash.Text = value;
//            }
//        }

		#endregion Properties

		#region UserControl overrides

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
// NOTE: reconsider if an alphbetic mode is implemented
//                case KeypadMode.Alphabetic:
//                    break;
            }
        }

		#endregion UserControl overrides        

		#region Event handlers

		private void FireCharacterKeypress(char keyChar)
		{
			if (CharacterKeypress != null)
			{
				CharacterKeypress(keyChar);
			}
		}

		private void FireClearKeypress()
		{
			if (ClearKeypress != null)
			{
				ClearKeypress();
			}
		}

		private void FireCancelKeypress()
		{
			if (CancelKeypress != null)
			{
				CancelKeypress();
			}
		}

		private void FireEnterKeypress()
		{
			if (EnterKeypress != null)
			{
				EnterKeypress();
			}
		}

		private void CharacterButtonKeypress(RoboSepButton button)
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
                    // NOTE: reconsider if an alphbetic mode is implemented...
					//                case KeypadMode.Alphabetic:
					//                    // Count successive clicks from any one button, modulo the
					//                    // number of alphabetic characters in the button text.  When the next 
					//                    // (different) button click arrives, "accept" the last entry for the
					//                    // alphabetic character.  Note this is similar to alphabetic character
					//                    // entry schemes typical of mobile phones, but slightly different.
					//                    break;
			}
		}

		private void btn0_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			CharacterButtonKeypress(btn0);
		}	

		private void btn1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			CharacterButtonKeypress(btn1);
		}

		private void btn2_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			CharacterButtonKeypress(btn2);
		}

        private void btn3_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            CharacterButtonKeypress(btn3);
        }

		private void btn4_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			CharacterButtonKeypress(btn4);
		}

		private void btn5_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			CharacterButtonKeypress(btn5);
		}

		private void btn6_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			CharacterButtonKeypress(btn6);
		}

		private void btn7_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			CharacterButtonKeypress(btn7);
		}

		private void btn8_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			CharacterButtonKeypress(btn8);
		}

		private void btn9_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			CharacterButtonKeypress(btn9);
		}

		private void btnStar_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			// NOTE: reconsider if an alphbetic mode is implemented
		}	

		private void btnHash_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			CharacterButtonKeypress(btnHash);
		}

		private void btnClear_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			FireClearKeypress();
		}

		private void btnCancel_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			FireCancelKeypress();
		}

		private void btnEnter_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			FireEnterKeypress();
		}			

		#endregion Event handlers

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btn0 = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.btn9 = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.btn4 = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.btn8 = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.btn7 = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.btn1 = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.btn3 = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.btn6 = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.btn5 = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.btn2 = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.btnStar = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.btnHash = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.btnEnter = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.btnClear = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.btnCancel = new Tesla.OperatorConsoleControls.RoboSepButton();
			this.SuspendLayout();
			// 
			// btn0
			// 
			this.btn0.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btn0.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn0.Location = new System.Drawing.Point(72, 168);
			this.btn0.Name = "btn0";
			this.btn0.Size = new System.Drawing.Size(56, 48);
			this.btn0.TabIndex = 173;
			this.btn0.Text = "0";
			this.btn0.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn0_MouseDown);
			// 
			// btn9
			// 
			this.btn9.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btn9.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn9.Location = new System.Drawing.Point(144, 112);
			this.btn9.Name = "btn9";
			this.btn9.Size = new System.Drawing.Size(56, 48);
			this.btn9.TabIndex = 172;
			this.btn9.Text = "9";
			this.btn9.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn9_MouseDown);
			// 
			// btn4
			// 
			this.btn4.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btn4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn4.Location = new System.Drawing.Point(0, 56);
			this.btn4.Name = "btn4";
			this.btn4.Size = new System.Drawing.Size(56, 48);
			this.btn4.TabIndex = 167;
			this.btn4.Text = "4";
			this.btn4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn4_MouseDown);
			// 
			// btn8
			// 
			this.btn8.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btn8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn8.Location = new System.Drawing.Point(72, 112);
			this.btn8.Name = "btn8";
			this.btn8.Size = new System.Drawing.Size(56, 48);
			this.btn8.TabIndex = 171;
			this.btn8.Text = "8";
			this.btn8.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn8_MouseDown);
			// 
			// btn7
			// 
			this.btn7.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btn7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn7.Location = new System.Drawing.Point(0, 112);
			this.btn7.Name = "btn7";
			this.btn7.Size = new System.Drawing.Size(56, 48);
			this.btn7.TabIndex = 170;
			this.btn7.Text = "7";
			this.btn7.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn7_MouseDown);
			// 
			// btn1
			// 
			this.btn1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btn1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn1.Location = new System.Drawing.Point(0, 0);
			this.btn1.Name = "btn1";
			this.btn1.Size = new System.Drawing.Size(56, 48);
			this.btn1.TabIndex = 164;
			this.btn1.Text = "1";
			this.btn1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn1_MouseDown);
			// 
			// btn3
			// 
			this.btn3.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btn3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn3.Location = new System.Drawing.Point(144, 0);
			this.btn3.Name = "btn3";
			this.btn3.Size = new System.Drawing.Size(56, 48);
			this.btn3.TabIndex = 166;
			this.btn3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn3_MouseDown);
			this.btn3.Text = "3";            
			// 
			// btn6
			// 
			this.btn6.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btn6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn6.Location = new System.Drawing.Point(144, 56);
			this.btn6.Name = "btn6";
			this.btn6.Size = new System.Drawing.Size(56, 48);
			this.btn6.TabIndex = 169;
			this.btn6.Text = "6";
			this.btn6.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn6_MouseDown);
			// 
			// btn5
			// 
			this.btn5.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btn5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn5.Location = new System.Drawing.Point(72, 56);
			this.btn5.Name = "btn5";
			this.btn5.Size = new System.Drawing.Size(56, 48);
			this.btn5.TabIndex = 168;
			this.btn5.Text = "5";
			this.btn5.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn5_MouseDown);
			// 
			// btn2
			// 
			this.btn2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btn2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn2.Location = new System.Drawing.Point(72, 0);
			this.btn2.Name = "btn2";
			this.btn2.Size = new System.Drawing.Size(56, 48);
			this.btn2.TabIndex = 165;
			this.btn2.Text = "2";
			this.btn2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn2_MouseDown);
			// 
			// btnStar
			// 
			this.btnStar.Enabled = false;
			this.btnStar.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btnStar.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnStar.Location = new System.Drawing.Point(0, 168);
			this.btnStar.Name = "btnStar";
			this.btnStar.Size = new System.Drawing.Size(56, 48);
			this.btnStar.TabIndex = 175;
			this.btnStar.Text = "A / 1";
			this.btnStar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnStar_MouseDown);
			// 
			// btnHash
			// 
			this.btnHash.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btnHash.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnHash.Location = new System.Drawing.Point(144, 168);
			this.btnHash.Name = "btnHash";
			this.btnHash.Size = new System.Drawing.Size(56, 48);
			this.btnHash.TabIndex = 174;
			this.btnHash.Text = ".";
			this.btnHash.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnHash_MouseDown);
			// 
			// btnEnter
			// 
			this.btnEnter.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btnEnter.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnEnter.Location = new System.Drawing.Point(224, 112);
			this.btnEnter.Name = "btnEnter";
			this.btnEnter.Size = new System.Drawing.Size(72, 104);
			this.btnEnter.TabIndex = 177;
			this.btnEnter.Text = "Enter";
			this.btnEnter.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnEnter_MouseDown);
			// 
			// btnClear
			// 
			this.btnClear.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btnClear.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnClear.Location = new System.Drawing.Point(224, 56);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(72, 48);
			this.btnClear.TabIndex = 176;
			this.btnClear.Text = "Clear";
			this.btnClear.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnClear_MouseDown);
			// 
			// btnCancel
			// 
			this.btnCancel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.btnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnCancel.Location = new System.Drawing.Point(224, 0);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(72, 48);
			this.btnCancel.TabIndex = 178;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnCancel_MouseDown);
			// 
			// RoboSepKeypad
			// 
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnEnter);
			this.Controls.Add(this.btnClear);
			this.Controls.Add(this.btn0);
			this.Controls.Add(this.btn9);
			this.Controls.Add(this.btn4);
			this.Controls.Add(this.btn8);
			this.Controls.Add(this.btn7);
			this.Controls.Add(this.btn1);
			this.Controls.Add(this.btn3);
			this.Controls.Add(this.btn6);
			this.Controls.Add(this.btn5);
			this.Controls.Add(this.btn2);
			this.Controls.Add(this.btnStar);
			this.Controls.Add(this.btnHash);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Name = "RoboSepKeypad";
			this.Size = new System.Drawing.Size(296, 216);
			this.ResumeLayout(false);

		}

		#endregion

	}
}
