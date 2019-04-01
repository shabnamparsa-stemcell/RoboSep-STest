using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Form_SplashScreen
{
    public partial class RoboSep_Splash : Form
    {
        private static RoboSep_Splash mySplashForm;
        //private Point Offset = new Point(4, 26);
        private Point Offset = new Point(0, 0);

        public RoboSep_Splash()
        {
            InitializeComponent();
            this.TopMost = true;
            this.DoubleBuffered = true;
        }

        private void Splash_Load(object sender, EventArgs e)
        {
        }

        public static RoboSep_Splash getInstance()
        {
            if (mySplashForm == null)
            {
                mySplashForm = new RoboSep_Splash();
            }
            return mySplashForm;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Form opening
            if (Opacity > 0.9D)
                Opacity -= 0.025D;
            else if (Opacity > 0.70D)
                Opacity -= 0.05D;
            else if (Opacity > 0.35D)
                Opacity -= 0.15D;
            else
                Opacity -= 0.175D;

            if (Opacity == 0.0D)
            {
                timer1.Stop();
                this.Close();
            }
        }


        private void Splash_Deactivate(object sender, EventArgs e)
        {
            this.BringToFront();
            this.Activate();
        }

        private void Splash_LocationChanged(object sender, EventArgs e)
        {
            this.BringToFront();
            this.Activate();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F3))
            {
                this.timer1.Start();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}