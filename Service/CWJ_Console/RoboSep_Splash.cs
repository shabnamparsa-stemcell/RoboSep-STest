using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GUI_Console
{
    public partial class RoboSep_Splash : Form
    {
        private static RoboSep_Splash mySplashForm;
        //private Point Offset = new Point(4, 26);
        private Point Offset = new Point(0, 0);
        private bool closing = false;

        public RoboSep_Splash()
        {
            InitializeComponent();
            this.Location = new Point(RoboSep_UserConsole.getInstance().Location.X + Offset.X,
                RoboSep_UserConsole.getInstance().Location.Y + Offset.Y);
            this.Opacity = 1;

            // LOG
            string logMSG = "Initializing Splash Page";
            GUI_Controls.uiLog.LOG(this, "RoboSep_Splash", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        public static RoboSep_Splash getInstance()
        {
            if (mySplashForm == null)
            {
                mySplashForm = new RoboSep_Splash();
            }
            return mySplashForm;
        }

        private void Splash_Load(object sender, EventArgs e)
        {
            RoboSep_UserConsole.getInstance().addForm(this, Offset);
            timer1.Start();
        }

        private void Splash_Deactivate(object sender, EventArgs e)
        {
            RoboSep_UserConsole.getInstance().BringToFront();
            this.BringToFront();
            this.Activate();
        }

        private void Splash_LocationChanged(object sender, EventArgs e)
        {
            this.BringToFront();
            this.Activate();
        }

        public void close()
        {
            closing = true;
            
            // LOG
            string logMSG = "Closing Splash Page";
            GUI_Controls.uiLog.LOG(this, "close", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        public static void shutDown()
        {
            mySplashForm = new RoboSep_Splash();
            mySplashForm.Opacity = 0.1D;
            mySplashForm.closing = false;
            mySplashForm.timer1.Start();
            mySplashForm.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!closing)
            {
                if (Opacity < 1.0D)
                {
                    if (Opacity < 0.8D)
                        Opacity += 0.1D;
                    else
                        Opacity += 0.025D;
                }
            }
            else
            {
                if (Opacity > 0.9D)
                    Opacity -= 0.025D;
                else if (Opacity > 0.70D)
                    Opacity -= 0.05D;
                else if (Opacity > 0.35D)
                    Opacity -= 0.15D;
                else
                    Opacity -= 0.175D;
                if (Opacity == 0.0D)
                    timer1.Stop();
                    this.Close();
            }
        }
    }
}
