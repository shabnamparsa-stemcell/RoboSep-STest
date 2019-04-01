using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
//using System.Xml;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Collections.Specialized;

namespace GUI_Console
{
    public partial class RoboSep_ServiceLogin : BasePannel
    {
        string[] Passwords;
        List<Image> ilist = new List<Image>();
        public RoboSep_ServiceLogin()
        {
            InitializeComponent();
        }

        private void RoboSep_ServiceLogin_Load(object sender, EventArgs e)
        {
            checkBox_standardUser.Check = true;
            // change graphics for Login Button

            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_PROMPTBUTTON0);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_PROMPTBUTTON0);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_PROMPTBUTTON0);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_PROMPTBUTTON3);
            button_Login.ChangeGraphics(ilist);

            // set passwords from config file
            Passwords = new string[3];
            try
            {
                // grab value from config file
                NameValueCollection nvc = (NameValueCollection)System.Configuration.ConfigurationManager.GetSection("OperatorConsole/ServiceLogin");
                Passwords[0] = nvc.Get("standardPassword");
                Passwords[1] = nvc.Get("maintenancePassword");
                Passwords[2] = nvc.Get("superPassword");                
            }
            catch
            {
                // set all passwords to "stemcell"
                for (int i = 0; i < Passwords.Length; i++)
                    Passwords[i] = "stemcell";
            }
        }

        private void label_superUser_Click(object sender, EventArgs e)
        {
            checkBox_standardUser.Check = false;
            checkBox_maintenanceUser.Check = false;
            checkBox_superUser.Check = true;
        }

        private void checkBox_superUser_Click(object sender, EventArgs e)
        {
            checkBox_standardUser.Check = false;
            checkBox_maintenanceUser.Check = false;
            checkBox_superUser.Check = true;
        }

        private void label_maintenanceUser_Click(object sender, EventArgs e)
        {
            checkBox_standardUser.Check = false;
            checkBox_maintenanceUser.Check = true;
            checkBox_superUser.Check = false;
        }

        private void checkBox_maintenanceUser_Click(object sender, EventArgs e)
        {
            checkBox_standardUser.Check = false;
            checkBox_maintenanceUser.Check = true;
            checkBox_superUser.Check = false;
        }

        private void label_standardUser_Click(object sender, EventArgs e)
        {
            checkBox_standardUser.Check = true;
            checkBox_maintenanceUser.Check = false;
            checkBox_superUser.Check = false;
        }

        private void checkBox_standardUser_Click(object sender, EventArgs e)
        {
            checkBox_standardUser.Check = true;
            checkBox_maintenanceUser.Check = false;
            checkBox_superUser.Check = false;
        }

        private bool validateUser()
        {
            string username = textBox_UserName.Text.ToUpper();
            if (username.Length < 5)
            {

                string sTitle = GUI_Controls.UserDetails.getValue("GUI", "headerLoginError3");
                string sMSG = GUI_Controls.UserDetails.getValue("GUI", "msgLoginError3");
                sMSG = sMSG == null ? "User Name entered is not long enough" : sMSG;
                GUI_Controls.RoboMessagePanel newPrompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(), GUI_Controls.MessageIcon.MBICON_ERROR,
                    sMSG, sTitle);
                RoboSep_UserConsole.getInstance().addForm(newPrompt, newPrompt.Offset);
                RoboSep_UserConsole.showOverlay();
                newPrompt.ShowDialog();
                newPrompt.Dispose();
                RoboSep_UserConsole.hideOverlay();
                //MessageBox.Show("User name not long enough");
                return false;
            }
            for (int i = 0; i < username.Length; i++)
            {
                if ( ! ((int)username[i] >= 65 && (int)username[i] <= 90 || (int)username[i] == 32))
                {
                    string sTitle = GUI_Controls.UserDetails.getValue("GUI", "headerLoginError");
                    string sMSG = GUI_Controls.UserDetails.getValue("GUI", "msgLoginError1");
                    string sButton =  GUI_Controls.UserDetails.getValue("GUI", "Ok");
                    sMSG = sMSG == null ? "User name should not include numbers, symbols, or punctuation" : sMSG;
                    GUI_Controls.RoboMessagePanel newPrompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(), GUI_Controls.MessageIcon.MBICON_ERROR,
                       sMSG, sTitle, sButton);
                    RoboSep_UserConsole.getInstance().addForm(newPrompt, newPrompt.Offset);
                    RoboSep_UserConsole.showOverlay();
                    newPrompt.ShowDialog();
                    newPrompt.Dispose();
                    RoboSep_UserConsole.hideOverlay();
                    //MessageBox.Show("User name should not include symbols or punctuation");
                    return false;
                }
                if (i < (username.Length - 2))
                {
                    if (username[i] == username[i+1] && username[i] == username[i+2])
                    {
                        string sTitle = GUI_Controls.UserDetails.getValue("GUI", "headerLoginError");
                        string sMSG = GUI_Controls.UserDetails.getValue("GUI", "msgLoginError2");
                        sMSG = sMSG == null ? "Incorrenctly typed User Name" : sMSG;
                        string sButton = GUI_Controls.UserDetails.getValue("GUI", "Ok");
                        GUI_Controls.RoboMessagePanel newPrompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(), GUI_Controls.MessageIcon.MBICON_ERROR, 
                        sMSG, sTitle, sButton);
                        RoboSep_UserConsole.getInstance().addForm(newPrompt, newPrompt.Offset);
                        RoboSep_UserConsole.showOverlay();
                        newPrompt.ShowDialog();
                        newPrompt.Dispose();
                        RoboSep_UserConsole.hideOverlay();
                        //MessageBox.Show("Incorrectly typed user name");
                        return false;
                    }
                }
            }
            return true;
        }

        private void Login()
        {
            if (validateUser())
            {
                bool[] userType = new bool[3] { checkBox_standardUser.Check, checkBox_maintenanceUser.Check, checkBox_superUser.Check };
                for (int i = 0; i < 3; i++)
                {
                    if (userType[i])
                    {
                        if (textBox_servicePassword.Text == Passwords[i])
                        {
                            GUI_Controls.RoboMessagePanel newPrompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(), GUI_Controls.MessageIcon.MBICON_INFORMATION, 
                                "Success, you have entered the right password!", "Password Correct");
                            RoboSep_UserConsole.getInstance().addForm(newPrompt, newPrompt.Offset);
                            RoboSep_UserConsole.showOverlay();
                            newPrompt.ShowDialog();
                            RoboSep_UserConsole.hideOverlay();
                            if (newPrompt.DialogResult == DialogResult.OK)
                            {
                                ActiveControl = textBox_servicePassword;
                            }
                            newPrompt.Dispose();
                        }
                        else
                        {
                            GUI_Controls.RoboMessagePanel newPrompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(), GUI_Controls.MessageIcon.MBICON_ERROR, 
                                "Incorrect password, Password is case sensetive", "Incorrect Password");
                            RoboSep_UserConsole.showOverlay();
                            newPrompt.ShowDialog();
                            RoboSep_UserConsole.hideOverlay();
                            if (newPrompt.DialogResult == DialogResult.OK)
                            {
                                ActiveControl = textBox_servicePassword;
                            }
                            newPrompt.Dispose();
                        }
                    }
                }
            }
        }

        private void textBox_servicePassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // submit password for login
                Login();
            }
        }

        private void button_Login_Click(object sender, EventArgs e)
        {
            // submit password for login
            Login();
        }

        private void textBox_UserName_MouseClick(object sender, MouseEventArgs e)
        {
            if (RoboSep_UserConsole.KeyboardEnabled)
            {
                // Show window overlay
                //RoboSep_UserConsole.showOverlay();
                //RoboSep_UserConsole.getInstance().frmOverlay.BringToFront();

                // Create keybaord control
                GUI_Controls.Keyboard newKeyboard =
                    GUI_Controls.Keyboard.getInstance(RoboSep_UserConsole.getInstance(),
                        textBox_UserName, null, RoboSep_UserConsole.getInstance().frmOverlay, false);
                newKeyboard.ShowDialog();

                // add keyboard control to user console "track form"
                RoboSep_UserConsole.getInstance().addForm(newKeyboard, newKeyboard.Offset);
                //RoboSep_UserConsole.myUserConsole.addForm(newKeyboard.overlay, newKeyboard.overlayOffset);
                newKeyboard.Dispose();
            }
        }

        private void textBox_servicePassword_MouseClick(object sender, MouseEventArgs e)
        {
            if (RoboSep_UserConsole.KeyboardEnabled)
            {
                // set text to "" so as not to show previous attempted password
                textBox_servicePassword.Text = "";

                // Show window overlay
                //RoboSep_UserConsole.showOverlay();
                //RoboSep_UserConsole.getInstance().frmOverlay.BringToFront();

                // Create keyboard
                GUI_Controls.Keyboard newKeyboard =
                    GUI_Controls.Keyboard.getInstance(RoboSep_UserConsole.getInstance(),
                        textBox_servicePassword, null, RoboSep_UserConsole.getInstance().frmOverlay, true);
                newKeyboard.ShowDialog();
                RoboSep_UserConsole.getInstance().addForm(newKeyboard, newKeyboard.Offset);
                //RoboSep_UserConsole.myUserConsole.addForm(newKeyboard.overlay, newKeyboard.overlayOffset);
                newKeyboard.Dispose();
            }
        }
    }
}
