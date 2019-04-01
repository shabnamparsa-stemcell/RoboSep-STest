using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using Invetech.ApplicationLog;
using GUI_Controls;
using Tesla.Common;

namespace GUI_Console
{
    public partial class Form_UserLoginNew : Form
    {
        private Point   Offset;
        private IniFile LanguageINI;
        private IniFile UserINI;
        private IniFile GUIini;
        private NewUser tempUser;
        private string TempImageIconFilePrefix = "$img";
        private string ImageIconFilePosfix = "_icon";
        private string DefaultUserHDImageDirectory = "c:\\Share Data\\";
        private int maxUserLoginStringLength = 20;
        private int maxTempImageIconFile = 10;
        private int maxSameUserLoginIDImageIconFile = 100;
        private const string UserInfoFileName = "UserInfo.ini";
        private const char reservedChar = '^';
        private const char spaceChar = ' ';

        private Dictionary<string, bool> dictUserPreferences = null;
        private Dictionary<string, bool> dictDevicePreferences = null;

        private List<Image> ilistSaveNormal = new List<Image>();
        private List<Image> ilistSaveDirty = new List<Image>();

        private List<string> slistUserNames;
        private string systemPath;
        private string NewLoginID;
        private string DefaultLoginID = string.Empty;
        private bool bCreateNewUser = true;

        private const string DefaultImage = "Default2";
        private RoboSep_UserPreferences userPreferences = null;
        private System.EventHandler evTextBoxClicked= null;

        public Form_UserLoginNew(List<string> UserNames, string sDefaultLoginID, bool bCreateNew)
        {
            InitializeComponent();

            SuspendLayout();

            DefaultLoginID = sDefaultLoginID;
            bCreateNewUser = bCreateNew;

            //change toggle button graphics
            List<Image> ilist = new List<Image>();
            ilist.Add(Properties.Resources.GE_BTN14M_preferences_STD);
            ilist.Add(Properties.Resources.GE_BTN14M_preferences_OVER);
            ilist.Add(Properties.Resources.GE_BTN14M_preferences_OVER);
            ilist.Add(Properties.Resources.GE_BTN14M_preferences_CLICK);
            button_User_Settings.ChangeGraphics(ilist);

            ilist.Clear();
            ilist.Add(Properties.Resources.US_BTN04L_usb_STD);
            ilist.Add(Properties.Resources.US_BTN04L_usb_OVER);
            ilist.Add(Properties.Resources.US_BTN04L_usb_OVER);
            ilist.Add(Properties.Resources.US_BTN04L_usb_CLICK);
            button_LoadImageUSB.ChangeGraphics(ilist);

            ilist.Clear();
            ilist.Add(Properties.Resources.US_BTN04L_hard_drive_STD);
            ilist.Add(Properties.Resources.US_BTN04L_hard_drive_OVER);
            ilist.Add(Properties.Resources.US_BTN04L_hard_drive_OVER);
            ilist.Add(Properties.Resources.US_BTN04L_hard_drive_CLICK);
            button_LoadImageHD.ChangeGraphics(ilist);

            ilist.Clear();
            ilist.Add(Properties.Resources.L_104x86_single_arrow_left_STD);
            ilist.Add(Properties.Resources.L_104x86_single_arrow_left_OVER);
            ilist.Add(Properties.Resources.L_104x86_single_arrow_left_OVER);
            ilist.Add(Properties.Resources.L_104x86_single_arrow_left_CLICK);
            button_Cancel.ChangeGraphics(ilist);

            ilistSaveDirty.Add(Properties.Resources.L_104x86_save_green);
            ilistSaveDirty.Add(Properties.Resources.GE_BTN10L_save_OVER);
            ilistSaveDirty.Add(Properties.Resources.GE_BTN10L_save_OVER);
            ilistSaveDirty.Add(Properties.Resources.GE_BTN10L_save_CLICK);

            ilistSaveNormal.Add(Properties.Resources.GE_BTN10L_save_STD);
            ilistSaveNormal.Add(Properties.Resources.GE_BTN10L_save_OVER);
            ilistSaveNormal.Add(Properties.Resources.GE_BTN10L_save_OVER);
            ilistSaveNormal.Add(Properties.Resources.GE_BTN10L_save_CLICK);
            button_Save.ChangeGraphics(ilistSaveNormal);

            slistUserNames = UserNames;

            // LOG
            string LOGmsg = "New user login screen";

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);

            UserINI = new IniFile(IniFile.iniFile.User);
            GUIini = new IniFile(IniFile.iniFile.GUI);

            // get language file
            LanguageINI = GUI_Console.RoboSep_UserConsole.getInstance().LanguageINI;
  
            systemPath = RoboSep_UserDB.getInstance().sysPath;
            tempUser = new NewUser();

            ResumeLayout();
        }

        private void Form_UserLoginNew_Load(object sender, EventArgs e)
        {
            // add forms to form tracker
            Offset = new Point((RoboSep_UserConsole.getInstance().Size.Width - this.Size.Width) / 2,
                (RoboSep_UserConsole.getInstance().Size.Height - this.Size.Height) / 2);

            this.Location = new Point(Offset.X + RoboSep_UserConsole.getInstance().Location.X,
                Offset.Y + RoboSep_UserConsole.getInstance().Location.Y);

            RoboSep_UserConsole.getInstance().addForm(this, Offset);

            bool bAddEvent = true;
            if (!bCreateNewUser && !string.IsNullOrEmpty(DefaultLoginID))
            {
                textBox_NewUserLoginID.Text = Utilities.GetUserName(DefaultLoginID, spaceChar, reservedChar);
                if (RoboSep_UserDB.getInstance().IsPresetUser(DefaultLoginID))
                {
                    textBox_NewUserLoginID.ReadOnly = true;
                    bAddEvent = false;
                }
            }
 
            if (bAddEvent)
            {
                evTextBoxClicked = new System.EventHandler(this.textBox_NewUserLoginID_Click);
                this.textBox_NewUserLoginID.Click += evTextBoxClicked;
            }
            label_User.Select();
        }

        public bool RenameUserNameInUserInfoINI(string sOldUserLoginID, string sNewUserLoginID)
        {
            if (string.IsNullOrEmpty(sOldUserLoginID) || string.IsNullOrEmpty(sNewUserLoginID))
                return false;

            string fullPathUserFileName = RoboSep_UserDB.getInstance().sysPath;

            fullPathUserFileName += "protocols\\";
            fullPathUserFileName += UserInfoFileName;

            if (!File.Exists(fullPathUserFileName))
            {
                return false;
            }

            // read values
            string[] lines = File.ReadAllLines(fullPathUserFileName, System.Text.Encoding.UTF8);
            List<string> lst = new List<string>();
            lst.AddRange(lines);
           
            int index;
            string temp;
            // rename section
            string sSection = String.Format("[{0}]", sOldUserLoginID);
            index = lst.FindIndex(x => { return (!string.IsNullOrEmpty(x) && x.Length == sSection.Length && x.ToLower() == (sSection.ToLower())); });
            if (0 <= index)
            {
                temp = String.Format("[{0}]", sNewUserLoginID);
                lst[index] = lst[index].Replace(sSection, temp);
            }

            // rename image path
            string sImageDefaultPath = String.Format("{0}Image", sOldUserLoginID);
            index = lst.FindLastIndex(x => { return (!string.IsNullOrEmpty(x) && x.Length >= sImageDefaultPath.Length && x.ToLower().Contains(sImageDefaultPath.ToLower()) && x.Trim().Substring(0, sImageDefaultPath.Length) == sImageDefaultPath); });
            if (0 <= index)
            {
                temp = String.Format("{0}Image", sNewUserLoginID);
                lst[index] = lst[index].Replace(sImageDefaultPath, temp);
            }

            File.WriteAllLines(fullPathUserFileName, lst.ToArray());
            return true;
        }

        private void Form_UserLoginNew_Deactivate(object sender, EventArgs e)
        {
            RoboSep_UserConsole.getInstance().frmOverlay.BringToFront();
            this.BringToFront();
            this.Activate();
        }

        private void button_LoadImageUSB_Click(object sender, EventArgs e)
        {
            bool LoadSuccessful = LoadUSBPicture();
            if (LoadSuccessful && tempUser.ImageIcon != null)
            {
                // Update picture
                pictureBox_User.Image = tempUser.ImageIcon;
                pictureBox_User.Invalidate();
            }
        }

        private void button_LoadImageHD_Click(object sender, EventArgs e)
        {
            bool LoadSuccessful = LoadHDPicture();
            if (LoadSuccessful && tempUser.ImageIcon != null)
            {
                // Update picture
                pictureBox_User.Image = tempUser.ImageIcon;
                pictureBox_User.Invalidate();
            }
            this.BringToFront();
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            string sTempUserLoginID = textBox_NewUserLoginID.Text.Trim();
            if (string.IsNullOrEmpty(sTempUserLoginID))
            {
                // Sunny to do 
                string sMSG = LanguageINI.GetString("lblUserName");
                string sMSG1 = LanguageINI.GetString("lblIsEmpty");
                string sMSG2 = LanguageINI.GetString("lblEnterUserName");
                string sOK = LanguageINI.GetString("Ok");
                string sMSG3 = String.Format("{0} {1}", sMSG, sMSG1);
                string sMSG4 = sMSG2;
              
                GUI_Controls.RoboMessagePanel prompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_ERROR,
                    sMSG3, sMSG4, sOK);
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                RoboSep_UserConsole.hideOverlay();
                prompt.Dispose();
                return;
            }
            else if (0 <= sTempUserLoginID.IndexOf(reservedChar))
            {
                string sHeader = LanguageINI.GetString("headerReservedChar");
                string sMSG = LanguageINI.GetString("lblUserName");
                string sMSG1 = LanguageINI.GetString("lblContainsIllegalChars");
                string sMSG2 = LanguageINI.GetString("lblEnterUserName");
                sMSG += " ";
                sMSG += sMSG1;
                string sMSG3 = String.Format(sMSG, reservedChar.ToString());
                sMSG3 += sMSG2;

                GUI_Controls.RoboMessagePanel prompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_ERROR,
                    sMSG3, sHeader, LanguageINI.GetString("Ok"));
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                RoboSep_UserConsole.hideOverlay();
                prompt.Dispose();
                return;
            }
            else if (sTempUserLoginID.Length > maxUserLoginStringLength)
            {
                // Sunny to do 
                string sMSG = LanguageINI.GetString("lblUserName");
                string sMSG1 = LanguageINI.GetString("lblIsTooLong");
                string sTemp = LanguageINI.GetString("lblUserNameMaxLength");
                string sMSG2 = String.Format(sTemp, maxUserLoginStringLength);
                string sMSG4 = sMSG2 + LanguageINI.GetString("lblEnterUserName");
                string sMSG5 = sMSG +" " + sMSG1;
 
                GUI_Controls.RoboMessagePanel prompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_ERROR,
                    sMSG4, sMSG5, LanguageINI.GetString("Ok"));
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                RoboSep_UserConsole.hideOverlay();
                prompt.Dispose();
                return;

            }
            
            int nIndex = sTempUserLoginID.IndexOf(spaceChar);
            if (0 < nIndex)
            {
                sTempUserLoginID = sTempUserLoginID.Replace(spaceChar, reservedChar);
            }
            string sUserLoginID = sTempUserLoginID;
            if (ValidateName(sUserLoginID) == false)
            {
                return;
            }

            string destination = DefaultImage;
            if (File.Exists(tempUser.TempImageIconPath))
            {
                // Copy image from temporary folder to image folder
                string temp = tempUser.TempImageIconPath;
                int fileExtensionIndex = temp.LastIndexOf('.');
                string fileExtension = temp.Substring(fileExtensionIndex);

                int lastBackslashIndex = temp.LastIndexOf('\\');  // must escape the '\'
                string tempDirectory = temp.Remove(lastBackslashIndex, temp.Length - lastBackslashIndex);

                string fileName = sUserLoginID;
                fileName += ImageIconFilePosfix;
                fileName += fileExtension;

                string targetDestination = systemPath + "images\\";
                if (!Directory.Exists(targetDestination))
                    Directory.CreateDirectory(targetDestination);

                destination = targetDestination + fileName;
                bool bContinue = false;

                for (int i = 0; i < maxSameUserLoginIDImageIconFile; i++)
                {
                    if (!File.Exists(destination))
                    {
                        bContinue = true;
                        break;
                    }
                    destination = string.Format("{0}{1}{2}{3}{4}", targetDestination, sUserLoginID, i, ImageIconFilePosfix, fileExtension);
                }

                if (bContinue != true)
                {
                    // LOG
                    string LOGmsg = "New user login screen: failed to save user info. File '";
                    LOGmsg += "' aready exists.";

                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, LOGmsg);
                    return;
                }

                File.Copy(tempUser.TempImageIconPath, destination);

                // Remove temporary folder
                Directory.Delete(tempDirectory, true);
            }
            NewLoginID = sUserLoginID;

            // Add visual effect
            string title = LanguageINI.GetString("headerSavingInProgress");
            RoboMessagePanel4 saving = new GUI_Controls.RoboMessagePanel4(RoboSep_UserConsole.getInstance(), title, 500, 20);
            RoboSep_UserConsole.showOverlay();
            saving.ShowDialog();

            RoboSep_UserConsole.hideOverlay();

            if (bCreateNewUser)
            {
                // create new user info in INI file
                string strTemp = sUserLoginID + "Image";
                UserINI.IniWriteValue(sUserLoginID, strTemp, destination);
            }
            else
            {
                // rename the userlogin
                RenameUserNameInUserInfoINI(DefaultLoginID, NewLoginID);

                // rename protocols and MRUs
                RoboSep_UserDB.getInstance().RenameProtocolsAndMRUs(DefaultLoginID, NewLoginID);
            }

            // update user preferences
            string UserToBeUpdated = NewLoginID;
            if (dictUserPreferences == null && dictDevicePreferences == null)
            {
                // User preferences are not set
                dictUserPreferences = new Dictionary<string, bool>();
                dictDevicePreferences = new Dictionary<string, bool>();

                if (bCreateNewUser)
                {
                     // Use default user and device preferences for new user.
                    RoboSep_UserDB.getInstance().getDefaultUserAndDevicePreferences(dictUserPreferences, dictDevicePreferences);
                }
                else
                {
                    RoboSep_UserDB.getInstance().getUserAndDevicePreferences(DefaultLoginID, dictUserPreferences, dictDevicePreferences);
                    if (dictUserPreferences.Count == 0 || dictDevicePreferences.Count == 0)
                    {
                        // Use default user and device preferences if the user preferences file not existed.
                        RoboSep_UserDB.getInstance().getDefaultUserAndDevicePreferences(dictUserPreferences, dictDevicePreferences);
                    }
                }
            }

            string UserLogin = bCreateNewUser ? NewLoginID : DefaultLoginID;

            // save user preferences
            UserDetails.SaveUserPreferences(UserLogin, dictUserPreferences, dictDevicePreferences);

            if (!bCreateNewUser)
            {
                // rename the user preferences file
                UserDetails.RenameUserPreferencesFile(DefaultLoginID, NewLoginID);
            }

            saving.Dispose();

            // remove Control            
            this.Close();
            DialogResult = DialogResult.OK;
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            // Sunny to do
            // Remove temperory files
            if (tempUser.TempImageIconPath != null)
            {
                try
                {
                    // delete temp folder 
                    Utilities.RemoveTempFileDirectory(tempUser.TempImageIconPath);
                }
                catch (Exception ex)
                {
                    // LOG
                    LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);
                }
               
                tempUser.Clear();
            }

            RoboSep_UserConsole.hideOverlay();

            this.DialogResult = DialogResult.Cancel;

            this.Close();
            //RoboSep_UserConsole.UserConsole.removeForm(overlay);
 
            RoboSep_UserConsole.getInstance().Activate();

            // LOG
            string logMSG = "User Login window closed without creating new user";

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
        }

        private void textBox_NewUserLoginID_Click(object sender, EventArgs e)
        {
            if (RoboSep_UserConsole.KeyboardEnabled)
                createKeybaord();
        }

        private void textBox_NewUserLoginID_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox_NewUserLoginID.Text))
            {
                button_User_Settings.Visible = false;
            }
            else
            {
                button_User_Settings.Visible = true;
            }

            ChangeSaveButtonGraphics();
        }

        private void mouseActivateTextbox(object sender, MouseEventArgs e)
        {
            if (RoboSep_UserConsole.KeyboardEnabled)
                createKeybaord();
        }

        private void ChangeSaveButtonGraphics()
        {
            bool bDirty = false;
            if (bCreateNewUser)
            {
                if (!string.IsNullOrEmpty(textBox_NewUserLoginID.Text))
                {
                    bDirty = true;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(textBox_NewUserLoginID.Text))
                {
                    string strName = Utilities.GetUserName(textBox_NewUserLoginID.Text.Trim(), reservedChar, spaceChar);
                    if (strName != DefaultLoginID)
                    {
                        bDirty = true;
                    }
                }
            }
            button_Save.ChangeGraphics(bDirty ? ilistSaveDirty : ilistSaveNormal);
        }

        private void createKeybaord()
        {
            // Create keybaord control
            GUI_Controls.Keyboard newKeyboard =
                GUI_Controls.Keyboard.getInstance(RoboSep_UserConsole.getInstance(),
                   textBox_NewUserLoginID, null, RoboSep_UserConsole.getInstance().frmOverlay, false);
            newKeyboard.ShowDialog();
            newKeyboard.Dispose();


            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "Generating touch keybaord");
        }
        public string NewUserLoginID
        {
            get
            {
                return NewLoginID;
            }
        }
        private bool ValidateName(string iLoginID)
        {
            if (string.IsNullOrEmpty(iLoginID))
                return false;
           
            string newLoginID = iLoginID.Trim();
            char[] invalidCharacters = new char[] { '~', '[', ']', '{', '}', '<', '>', '&', '/', '\\', '\'', '=', '~' };
            int nIndex = -1;

            // Check for duplicate
            if (slistUserNames.Contains(newLoginID))
            {
                // prompt user
                string sMsg = LanguageINI.GetString("msgUserOccupied");
                string sHeader = LanguageINI.GetString("headerUserInvalid");
                string sButton = LanguageINI.GetString("Ok");
                GUI_Controls.RoboMessagePanel prompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_ERROR, sMsg, sHeader, sButton);
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                prompt.Dispose();                
                RoboSep_UserConsole.hideOverlay();
                return false;
            }
            else if ((nIndex = newLoginID.IndexOfAny(invalidCharacters)) >= 0)
            {
                string sInvalidCharacter= newLoginID.Substring(nIndex, 1);
                
                // prompt user
                string sTemp = LanguageINI.GetString("msgUserInvalidCharacters");
                string sMsg = String.Format(sTemp, sInvalidCharacter);
                string sHeader = LanguageINI.GetString("headerUserInvalid");
                string sButton = LanguageINI.GetString("Ok");
                GUI_Controls.RoboMessagePanel prompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_ERROR, sMsg, sHeader, sButton);
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                prompt.Dispose();
                RoboSep_UserConsole.hideOverlay();
                return false;
            }
            else
            {
                string[] aPresetNames = RoboSep_UserDB.getInstance().GetProtocolPresetNames();
                if (aPresetNames != null && aPresetNames.Length > 0)
                {
                    string sName = Array.Find(aPresetNames, (x => { return (!string.IsNullOrEmpty(x) && x.ToLower() == newLoginID.ToLower()); }));
                    if (!string.IsNullOrEmpty(sName))
                    {
                        // prompt user
                        string sMsg = LanguageINI.GetString("msgNameUsedByPresets");
                        string sHeader = LanguageINI.GetString("headerUserInvalid");
                        string sButton = LanguageINI.GetString("Ok");
                        GUI_Controls.RoboMessagePanel prompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_ERROR, sMsg, sHeader, sButton);
                        RoboSep_UserConsole.showOverlay(); ;
                        prompt.ShowDialog();
                        prompt.Dispose();
                        RoboSep_UserConsole.hideOverlay();
                        return false;
                    }
                }
            }
            return true;
        }

        private bool LoadHDPicture()
        {
            // get all the local drives
            string[] drives = Environment.GetLogicalDrives();
            return LoadPicture(drives);
        }


        private bool LoadUSBPicture()
        {
            List<string> lstUSBDrives = AskUserToInsertUSBDrive();

            if (lstUSBDrives == null || lstUSBDrives.Count == 0)
            {
                return false;
            }

            return LoadPicture(lstUSBDrives.ToArray());
        }

        private List<string> AskUserToInsertUSBDrive()
        {
            List<string> lstUSBDrives = Utilities.GetUSBDrives();
            if (lstUSBDrives == null)
                return null;

            if (lstUSBDrives.Count > 0)
                return lstUSBDrives;


            // prompt if user to insert a USB drive
            string msg = LanguageINI.GetString("msgInsertUSB");
            RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_INFORMATION, msg,
                LanguageINI.GetString("headerInsertUSB"), LanguageINI.GetString("Ok"), LanguageINI.GetString("Cancel"));
            RoboSep_UserConsole.showOverlay();
            prompt.ShowDialog();
            RoboSep_UserConsole.hideOverlay();

            if (prompt.DialogResult != DialogResult.OK)
            {
                prompt.Dispose();
                return null;
            }
            prompt.Dispose();
            return AskUserToInsertUSBDrive();
        }

        private bool LoadPicture(string[] drives)
        {
            if (drives == null)
            {
                return false;
            }


            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "Report SysPath: " + systemPath);
            //systemPath = @"C:\Program Files (x86)\STI\RoboSep\";

            string IconsPath = systemPath + "images\\";
            if (!Directory.Exists(IconsPath))
                Directory.CreateDirectory(IconsPath);

            string TempPath = systemPath + "temp\\";
            if (!Directory.Exists(TempPath))
                Directory.CreateDirectory(TempPath);

            // Check if there is any previous images
            if (tempUser.ImageIcon != null)
            {

                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "User has already load an image");
                if (tempUser.TempImageIconPath != null)
                {

                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "Temporary image file location: " + tempUser.TempImageIconPath);
                }
 
                // Sunny to do
                // set up message prompt
                string sMSG = LanguageINI.GetString("lblPreviousImageLoaded"); ;
                string sMSG2 = LanguageINI.GetString("lblOverwritePreviousImage");
                sMSG += sMSG2;
                RoboMessagePanel prompt = new RoboMessagePanel(this, MessageIcon.MBICON_WARNING, sMSG,
                    LanguageINI.GetString("Warning"), LanguageINI.GetString("Yes"), LanguageINI.GetString("No"));
                RoboSep_UserConsole.showOverlay();

                //prompt user
                prompt.ShowDialog();
                RoboSep_UserConsole.hideOverlay();

                if (prompt.DialogResult != DialogResult.OK)
                {
                    prompt.Dispose();
                    return false;
                }
                prompt.Dispose();
                try
                {
                    // Remove the temporary directory and its contents
                    Utilities.RemoveTempFileDirectory(tempUser.TempImageIconPath);
                }
                catch (Exception ex)
                {
                    // LOG
                    LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);
                }

                // Clear the previous image info
                tempUser.TempImageIconPath = "";
                tempUser.ImageIcon = null;
            }

            // get the default image directory
            string sDefaultImgDirectory = String.Empty;
            string sTempImgDirectory = GUIini.IniReadValue("General", "DefaultUserHDImageDirectory", DefaultUserHDImageDirectory);
            sTempImgDirectory = Utilities.RemoveIllegalCharsInDirectory(sTempImgDirectory);

            DriveInfo d = new DriveInfo(sTempImgDirectory);
            if (d.IsReady)
            {
                string sName = d.Name;
                List<string> lstDrive = new List<string>(drives);
                string sDrive = lstDrive.Find(x => { return (!string.IsNullOrEmpty(x) && x.ToLower() == sName.ToLower()); });
                if (!string.IsNullOrEmpty(sDrive))
                {
                    if (Directory.Exists(sTempImgDirectory))
                    {
                        sDefaultImgDirectory = sTempImgDirectory;
                    }
                }
            }

            if (string.IsNullOrEmpty(sDefaultImgDirectory))
            {
                // search for directory
                string[] dirs = drives;
                int dirIndex = -1;
                for (int i = 0; i < dirs.Length; i++)
                {
                    if (Directory.Exists(dirs[i]))
                    {
                        dirIndex = i;
                        sDefaultImgDirectory = dirs[dirIndex];
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(sDefaultImgDirectory))
            {
                string sTitle = LanguageINI.GetString("headerSelectImageFile");
                FileBrowser fb = new FileBrowser(RoboSep_UserConsole.getInstance(), sTitle, sDefaultImgDirectory, IconsPath,
                    new string[] { ".jpg", ".jpeg", ".png", ".tga", ".bmp" }, FileBrowser.BrowseResult.SelectFile);
                fb.ShowDialog();
                // make sure that file browser finished properly
                if (fb.DialogResult == DialogResult.OK)
                {

                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "File Browser result: OK!!");        
                    // store target file path
                    string fileTarget = fb.Target;

                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "File target = " + fileTarget);        
                    // generate icon graphic
                    string IconPath = GenerateProfileIcon(fileTarget);

                    LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "Temorary Icon Path : " + IconPath);    
                 }
                fb.Dispose();
                return true;
            }
            else
            {

                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "Failed to locate drive");    
            }
            return false;
        }

        private bool removeImageIconTempDirectory(string fileName)
        {
            if (fileName == null && fileName == string.Empty)
                return false;

            // Remove the temporary directory and its contents
            string temp = fileName;
            int lastBackslashIndex = temp.LastIndexOf('\\');  // must escape the '\'

            if (lastBackslashIndex < 0)
                return false;

            string tempFileName = temp.Substring(lastBackslashIndex + 1);

            string tempImageIconDirectory = temp.Remove(lastBackslashIndex, temp.Length - lastBackslashIndex);
            
            // Double check to be sure
            lastBackslashIndex = tempImageIconDirectory.LastIndexOf('\\');  // must escape the '\'
            temp = tempImageIconDirectory;
            string tempDirectory = temp.Remove(lastBackslashIndex, temp.Length - lastBackslashIndex);
            string sysTempPath = systemPath + "temp";
            if (tempDirectory == sysTempPath)
            {
                Directory.Delete(tempImageIconDirectory, true);
                return true;
            }
            return false;
        }

        private string getFullImageIconTempFileName(string folderPath, string extension)
        {
            if (folderPath == string.Empty)
            {
                return null;
            }

            StringBuilder temp = new StringBuilder();
            temp.Append(folderPath);
            temp.Append("\\");

            string tempFilename = TempImageIconFilePrefix + "000";

            if (Directory.Exists(folderPath))
            {
                string tempFileNamePath = temp.ToString() + TempImageIconFilePrefix;
                string tempFullFileName;
                for (int i = 0; i < maxTempImageIconFile; i++)
                {
                    tempFilename = TempImageIconFilePrefix + i.ToString("000");

                    tempFullFileName = tempFileNamePath;
                    tempFullFileName += tempFilename;
                    tempFullFileName += extension;

                    // Check if temp file exists
                    if (!File.Exists(tempFullFileName))
                    {
                        break;
                    }
                }
            }

            temp.Append(tempFilename);
            temp.Append(extension);

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "Get a temporary folder for user icon: " + temp.ToString());    
            return temp.ToString();
        }

        private void OnHandleUserPreferences(object sender, EventArgs e)
        {
            
        }


        private void button_LoadImageDefault_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox_User_Click(object sender, EventArgs e)
        {
            button_LoadImageUSB.Visible = true;
            button_LoadImageHD.Visible = true;
            lineShape2.Visible = true;
            lineShape3.Visible = true;
        }

        private void button_User_Settings_Click(object sender, EventArgs e)
        {
            string sUserLoginID;
            if (bCreateNewUser)
            {
                // New user
                sUserLoginID = textBox_NewUserLoginID.Text;
            }
            else
            {
                // edit user
                sUserLoginID = DefaultLoginID;
            }
            if (string.IsNullOrEmpty(sUserLoginID) || sUserLoginID.Trim().Length == 0)
                return;

            if (userPreferences != null)
            {
                // let garbage collector to dispose this item
                userPreferences.Dispose();
                userPreferences = null;
            }

            userPreferences = new RoboSep_UserPreferences(sUserLoginID, bCreateNewUser? true : false, false);
            userPreferences.GetUserPreferences += new EventHandler(GetUserPrefernces);
            userPreferences.ClosingUserPreferencesApp += new EventHandler(HandleClosingUserPreferencesApp);
            userPreferences.Parent = this;

            userPreferences.Show();
            userPreferences.BringToFront();
            userPreferences.Focus();
        }

        private void GetUserPrefernces(object sender, EventArgs e)
        {
            RoboSep_UserPreferences userPref = (RoboSep_UserPreferences)sender;

            Dictionary<string, bool>dictUser = userPref.UserPreferences;
            Dictionary<string, bool>dictDevice = userPref.DevicePreferences;

            if (dictUser.Count > 0)
            {
                if (dictUserPreferences == null)
                {
                    dictUserPreferences = new Dictionary<string, bool>();
                    copyPreferences(dictUser, dictUserPreferences);
                }
            }

            if (dictDevice.Count > 0)
            {
                if (dictDevicePreferences == null)
                {
                    dictDevicePreferences = new Dictionary<string, bool>();
                    copyPreferences(dictDevice, dictDevicePreferences);
                }
            }
        }

        private void copyPreferences(Dictionary<string, bool> dictSettings, Dictionary<string, bool> dictPreferences)
        {
            if (dictSettings == null || dictPreferences == null)
                return;

            dictPreferences.Clear();

            IDictionaryEnumerator aEnumerator = dictSettings.GetEnumerator();
            while (aEnumerator.MoveNext())
            {
               dictPreferences.Add((string)aEnumerator.Key, (bool)aEnumerator.Value);
            }
        }

        private void HandleClosingUserPreferencesApp(object sender, EventArgs e)
        {
            this.Visible = true;
            this.Show();
            this.BringToFront();
            this.Activate();
        }

        #region GRAPHIC_EDITING

        private string GenerateProfileIcon(string imgPath)
        {
            if (imgPath == null)
            {

                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "Generating Profile Icon returns. imgpath is null");    
                return null;
            }

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "Generating Profile Icon");                
            Image source = Image.FromFile(imgPath);

            Image workingImage = CropImage(source);

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "Cropped Image");                

            workingImage = scaleImage(workingImage, 48);

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "Scaled Image");                

            workingImage = frameImage(workingImage);

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "Framed Image");                

            string tempPath = Utilities.GetTempFileFolder();

            //string framedPath = getTempFileName(tempPath, (UserName + "_framed"), ".png");

            string framedPath = getFullImageIconTempFileName(tempPath, ".png");
            try
            {
                if (!Directory.Exists(tempPath))
                {
                    Directory.CreateDirectory(tempPath);
                }

                // fails when saving over file it is currently using
                savePNG(framedPath, (Bitmap)workingImage, 100L);

                // set flag to valid
                if (tempUser != null)
                {
                    tempUser.ImageIcon = workingImage;
                    tempUser.TempImageIconPath = framedPath;
                }
                workingImage.Dispose();
            }
            catch
            {

                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "Fail to save profile icon. ");                
            }


            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "Profile Icon Saved");                

            return framedPath;
        }
        private string changeFileName(string originalPath, string newFileName, string extension)
        {
            StringBuilder temp = new StringBuilder();
            temp.Append(systemPath);
            temp.Append("images\\");
            temp.Append(newFileName);
            temp.Append(extension);

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, "Save User Icon: " + temp.ToString());                
            return temp.ToString();
        }


        private void saveJPG(string path, Bitmap img, long quality)
        {
            // set image quality
            EncoderParameter jpgQuality = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

            // set image compression codec
            ImageCodecInfo jpgCodec = getCodecInfo("image/jpeg");

            if (jpgCodec == null)
                return;

            EncoderParameters eParams = new EncoderParameters();
            eParams.Param[0] = jpgQuality;

            img.Save(path, jpgCodec, eParams);
        }

        private void savePNG(string path, Bitmap img, long quality)
        {
            // set image quality
            EncoderParameter pngQuality = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

            // set image compression codec
            ImageCodecInfo pngCodec = getCodecInfo("image/png");

            if (pngCodec == null)
                return;

            EncoderParameters eParams = new EncoderParameters();
            eParams.Param[0] = pngQuality;

            if (File.Exists(path))
            {

            }
            img.Save(path, pngCodec, eParams);
        }

        private ImageCodecInfo getCodecInfo(string type)
        {
            // get all codecs
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // find jpg encoder
            for (int i = 0; i < codecs.Length; i++)
            {
                if (codecs[i].MimeType == type)
                    return codecs[i];
            }
            return null;
        }

        private Image CropImage(Image img)
        {
            Bitmap imgCopy = new Bitmap(img);
            int X = imgCopy.Size.Width;
            int Y = imgCopy.Size.Height;
            int min;
            bool portrait;
            int centeringOffset;
            Rectangle sqCrop;

            if (X < Y)
            {
                min = X;
                portrait = true;
                // centre crop
                centeringOffset = (int)((double)(Y - X) / 2.0);
            }
            else
            {
                min = Y;
                portrait = false;
                // centre crop
                centeringOffset = (int)((double)(X - Y) / 2.0);
            }

            if (portrait)
            {
                sqCrop = new Rectangle(0, centeringOffset, min, min);
            }
            else
            {
                sqCrop = new Rectangle(centeringOffset, 0, min, min);
            }

            Bitmap imgCropped = imgCopy.Clone(sqCrop, imgCopy.PixelFormat);
            imgCopy.Dispose();
            return (Image)(imgCropped);
        }

        private Image scaleImage(Image img, int pixelDimension)
        {

            Bitmap imgResized = new Bitmap(pixelDimension, pixelDimension);
            Graphics g = Graphics.FromImage((Image)imgResized);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            g.DrawImage(img, 0, 0, pixelDimension, pixelDimension);
            g.Dispose();

            return (Image)imgResized;
        }

        private Image frameImage(Image img)
        {
            Image frame = Properties.Resources.CustomUserPic_Border; //Image.FromFile(systemPath + "\\images\\Frame.png");
            Bitmap framedImage = new Bitmap(frame.Size.Width, frame.Size.Height);

            Graphics g = Graphics.FromImage(framedImage);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            //g.DrawImage(img, new Rectangle(0, 0, frame.Size.Width, frame.Size.Height), 
            //    new Rectangle(0, 0, frame.Size.Width, frame.Size.Height), GraphicsUnit.Pixel);
            g.DrawImage(img, new Point(0, 0));
            g.DrawImage(frame, new Rectangle(new Point(0, 0), frame.Size));


            g.Save();
            g.Dispose();
            return (Image)framedImage;
        }

        #endregion



        class NewUser
        {
            public NewUser() { }

            public void Clear()
            {
                name = "";
                tempImageIconPath = "";
              }

            public string Name
            {
                get { return name; }
                set { name = value; }
            }
            private string name;

            public string TempImageIconPath
            {
                get { return tempImageIconPath; }
                set { tempImageIconPath = value; }
            }
            private string tempImageIconPath;

            public Image ImageIcon
            {
                get { return imageIcon; }
                set { imageIcon = value; }
            }
            private Image imageIcon;
        }
    }
}
