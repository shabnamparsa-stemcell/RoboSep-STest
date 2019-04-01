using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Xml;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Drawing2D;

namespace GUI_Controls
{
    public partial class Form1 : Form
    {
        // Form Offsets
        List<Point> Offsets = new List<Point>();

        public Form1()
        {
            InitializeComponent();
        }

        private void button_Cancel1_EnabledChanged(object sender, EventArgs e)
        {
            //this.Enabled = true;
            //this.Close();
        }

        private void button_Cancel1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_MouseClick(object sender, MouseEventArgs e)
        {   
            //Keyboard newKeyboard = new Keyboard(this, textBox1);
            //newKeyboard.ShowDialog();

        }

        private void textBox2_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void control_SearchBar_PaddingChanged(object sender, EventArgs e)
        {
            // refilter list
           
        }

        private void HelpTabs_BackgroundImageChanged(object sender, EventArgs e)
        {
            // check if form.currentTab == Tabs.currentTab
            // if not, change to new tab
            // change form.currentTab to... what ever
        }

        private void Form1_LocationChanged(object sender, EventArgs e)
        {
            // check if # of offsets matches # of windows
            if (Offsets.Count != Application.OpenForms.Count)
            {
                Offsets.Clear();
                for (int i = 0; i < Application.OpenForms.Count; i++)
                {
                    // find difference between this.location and other forms.location
                    int offsetX = Application.OpenForms[i].Location.X - this.Location.X;
                    int offsetY = Application.OpenForms[i].Location.Y - this.Location.Y;
                    Offsets.Add(new Point(offsetX, offsetY));
                }
            }
            
            // get list of open forms
            for (int i = 0; i < Application.OpenForms.Count; i++)
            {
                if (Application.OpenForms[i] != this)
                {
                        int newX = this.Location.X + Offsets[i].X;
                        int newY = this.Location.Y + Offsets[i].Y;
                        Application.OpenForms[i].Location = new Point(newX, newY);
                }
            }
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            Offsets.Clear();
            // get list of open forms
            // store current offsets 
            for (int i = 0; i < Application.OpenForms.Count; i++)
            {
                // find difference between this.location and other forms.location
                int offsetX = Application.OpenForms[i].Location.X - this.Location.X;
                int offsetY = Application.OpenForms[i].Location.Y - this.Location.Y;
                Offsets.Add(new Point(offsetX, offsetY));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            uiLog.LOG(this, "Button1_Click", uiLog.LogLevel.EVENTS, "Button 1 click event activated, progress bar started");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string [] Lines = File.ReadAllLines("C:\\temp\\TestTxt.ini", System.Text.Encoding.Default);
        }

        private void button3_Click(object sender, EventArgs e)
        {
        }

        private void btnMP2_Click(object sender, EventArgs e)
        {
            RoboMessagePanel prompt = new RoboMessagePanel(this, MessageIcon.MBICON_INFORMATION, "this is a 2 button message pannel.  See how there are 2 option buttons at the bottom....",
                "2 buttons", "Excellent", "Meh");
            prompt.ShowDialog();
            prompt.Dispose();
        }

        private void btnMP1_Click(object sender, EventArgs e)
        {
            RoboMessagePanel prompt = new RoboMessagePanel(this, MessageIcon.MBICON_INFORMATION, "this is a 1 button message pannel.  See how there is only 1 option button at the bottom....", "1 button");
            prompt.ShowDialog();
            prompt.Dispose();
        }

        private void btn_logInit_Click(object sender, EventArgs e)
        {
            
        }

        int tickCount = 0;
        private void btn_Log_Click(object sender, EventArgs e)
        {
            tickCount++;

            Graphics mygraphics = this.CreateGraphics();
            Image img = Properties.Resources.HEX_SAMPLE0;

            int seqNum = tickCount % 10;
            float rotationAngle = seqNum * 9;
            Matrix myMatrix = new Matrix();
            //myMatrix.RotateAt(rotationAngle, new PointF(img.Width / 2, img.Height / 2), MatrixOrder.Prepend);
            myMatrix.RotateAt(rotationAngle, new PointF(img.Width/2, img.Height/2), MatrixOrder.Prepend);
            myMatrix.Translate(273, 12, MatrixOrder.Append);

            mygraphics.Transform = myMatrix;

            try
            {
                // draw on picturebox content using transformed points
                mygraphics.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));
            }
            finally
            {
                mygraphics.Dispose();
            }
        }

        private void horizontalTabs1_Tab1_Click(object sender, EventArgs e)
        {
        }

        private void horizontalTabs1_Tab2_Click(object sender, EventArgs e)
        {
        }

        private void horizontalTabs1_Tab3_Click(object sender, EventArgs e)
        {
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Keyboard newKeyboard = Keyboard.getInstance(this, textBox1, null, null, false);
            newKeyboard.ShowDialog();
            newKeyboard.Dispose();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            this.Refresh();
        }
    }
}
