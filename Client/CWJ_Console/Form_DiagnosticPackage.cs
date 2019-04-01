using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GUI_Console
{
    public partial class Form_DiagnosticPackage : Form
    {


        private Point Offset;

        public Form_DiagnosticPackage()
        {
            InitializeComponent();

            SuspendLayout();

            List<Image> ilist = new List<Image>();
            ilist.Add(Properties.Resources.L_104x86_check_green);
            ilist.Add(Properties.Resources.GE_BTN20L_ok_OVER);
            ilist.Add(Properties.Resources.GE_BTN20L_ok_OVER);
            ilist.Add(Properties.Resources.GE_BTN20L_ok_CLICK);
            button_OK.ChangeGraphics(ilist);

            ilist.Clear();
            ilist.Add(Properties.Resources.GE_BTN13L_cancel_STD);
            ilist.Add(Properties.Resources.GE_BTN13L_cancel_OVER);
            ilist.Add(Properties.Resources.GE_BTN13L_cancel_OVER);
            ilist.Add(Properties.Resources.GE_BTN13L_cancel_CLICK);
            button_Cancel.ChangeGraphics(ilist);
        }


        private void Form_DiagnosticPackage_Load(object sender, EventArgs e)
        {
            IniFile LanguageINI = GUI_Console.RoboSep_UserConsole.getInstance().LanguageINI;

            label_WindowTitle.Text = LanguageINI.GetString("headerChooseDestination");
            label_USB.Text = LanguageINI.GetString("lblUSB");
            label_FTPServer.Text = LanguageINI.GetString("lblFTPServer");
            label_WindowEvent.Text = LanguageINI.GetString("lblIncludeWindowsEvents");


            
            checkBox_WindowsEvents.Check = false;
            checkBox_USB.Check = true;
            checkBox_FTPServer.Check = false;

            // add forms to form tracker
            Offset = new Point((RoboSep_UserConsole.getInstance().Size.Width - this.Size.Width) / 2,
                (RoboSep_UserConsole.getInstance().Size.Height - this.Size.Height) / 2);

            this.Location = new Point(Offset.X + RoboSep_UserConsole.getInstance().Location.X,
                Offset.Y + RoboSep_UserConsole.getInstance().Location.Y);

            RoboSep_UserConsole.getInstance().addForm(this, Offset);

        }

        public bool IncludeWindowsEventsLogs()
        {
            return checkBox_WindowsEvents.Check;
        }

        public enumSaveDestination Destination()
        {
            enumSaveDestination eDestination = enumSaveDestination.eDestinationUnknown;

            if (checkBox_USB.Check == true)
            {

                eDestination = enumSaveDestination.eDestinationUSB;
            }
            else if (checkBox_FTPServer.Check == true)
            {
                eDestination = enumSaveDestination.eDestinationFTP;
            }
            return eDestination;
        }

        private void checkBox_WindowsEvents_Click(object sender, EventArgs e)
        {
            
        }

        private void checkBox_USB_Click(object sender, EventArgs e)
        {
            checkBox_FTPServer.Check = !checkBox_USB.Check;
        }

        private void checkBox_FTPServer_Click(object sender, EventArgs e)
        {
            checkBox_USB.Check = !checkBox_FTPServer.Check;
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            RoboSep_UserConsole.hideOverlay();
            this.Close();

            RoboSep_UserConsole.getInstance().Activate();
            this.DialogResult = DialogResult.Cancel;
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            RoboSep_UserConsole.hideOverlay();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

    }



}
