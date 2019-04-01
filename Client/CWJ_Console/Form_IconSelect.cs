using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using GUI_Controls;

namespace GUI_Console
{
    public partial class Form_IconSelect : Form
    {
        public string ImgPath = string.Empty;
        IniFile UserINI;
        string UserName;
        string userImgKey;

        // Language file
        private IniFile LanguageINI = GUI_Console.RoboSep_UserConsole.getInstance().LanguageINI;

        public Form_IconSelect(string User)
        {
            InitializeComponent();

            Point UCpoint = RoboSep_UserConsole.getInstance().Location;
            int X = UCpoint.X + (640 - this.Size.Width) / 2;
            int Y = UCpoint.Y + (480 - this.Size.Height) / 2;

            this.Location = new Point(X, Y);

            // change graphics for Cancel button
            List<Image> ilist = new List<Image>();
            ilist.Add(Properties.Resources.WindowClose_STD);
            ilist.Add(Properties.Resources.WindowClose_STD);
            ilist.Add(Properties.Resources.WindowClose_STD);
            ilist.Add(Properties.Resources.WindowClose_CLICK);
            button_Cancel1.ChangeGraphics(ilist);

            UserINI = new IniFile(IniFile.iniFile.User);
            UserName = User;
            userImgKey = UserName + "Image";
        }

        private void button_Cancel1_Click(object sender, EventArgs e)
        {
            ImgPath = string.Empty;
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        private void DefaultIcon1_Click(object sender, EventArgs e)
        {
            ImgPath = "Default1";
            UserINI.IniWriteValue(UserName, userImgKey, ImgPath);
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void DefaultIcon2_Click(object sender, EventArgs e)
        {
            ImgPath = "Default2";
            UserINI.IniWriteValue(UserName, userImgKey, ImgPath);
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void DefaultIcon3_Click(object sender, EventArgs e)
        {
            ImgPath = "Default3";
            UserINI.IniWriteValue(UserName, userImgKey, ImgPath);
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void DefaultIcon4_Click(object sender, EventArgs e)
        {
            ImgPath = "Default4";
            UserINI.IniWriteValue(UserName, userImgKey, ImgPath);
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void FromUSB_Click(object sender, EventArgs e)
        {
            RoboSep_UserSelect myUserSelect = new RoboSep_UserSelect();
            bool GraphicCreated = myUserSelect.LoadUSBPicture(UserName);

            if (GraphicCreated)
            {
                this.DialogResult = DialogResult.OK;
                this.Hide();
            }
        }
    }
}
