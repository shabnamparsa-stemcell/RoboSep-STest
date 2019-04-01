using System;
using System.Text.RegularExpressions;
namespace Tesla.ProtocolEditorControls
{
    partial class ProtocolTextbox
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            this.TextChanged += new EventHandler(ProtocolTextbox_TextChanged);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(ProtocolTextbox_KeyPress);
        }

        private string lastString;
        private void ProtocolTextbox_TextChanged(object sender, EventArgs e)
        {
            if (lastString == this.Text) return;

            bool isMatch = System.Text.RegularExpressions.Regex.IsMatch(this.Text, legalCharacters);

            bool isBadCharacter = !isMatch;

            //Regex regex = new Regex(illegalCharacters);
            //MatchCollection matches = regex.Matches(this.Text);
            if (isBadCharacter)//matches.Count > 0)
            {
                
                string list ="";
                foreach (char c in this.Text)
                {
                    isMatch = System.Text.RegularExpressions.Regex.IsMatch(c.ToString(), legalCharacters);
                    if (!isMatch)
                    {
                        list += c.ToString();
                    }
                }
                /*
                foreach(Match m in matches){
                   list += m.Value;
                }
                 */ 
                //tell the user
                System.Windows.Forms.MessageBox.Show(this.Text + " contains illegal characters. The following illgeal charcters were pasted into the textbox: "+list);
                
                //System.Windows.Forms.MessageBox.Show(this.Text + " contains illegal characters. ");
                
                this.Text = lastString;
            }
            else
            {
                lastString = this.Text;
            }
        }

        private void ProtocolTextbox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            bool isMatch = System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), legalCharacters);

            bool isBadCharacter = !isMatch;

            // Check for a naughty character in the KeyDown event.
            if (isBadCharacter)
            {
                // Stop the character from being entered into the control since it is illegal.
                e.Handled = isBadCharacter;
            }
        }
        #endregion
    }
}
