using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Xml;
using System.Text;
using System.Windows.Forms;
using System.IO;

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
            //newKeyboard.Show();

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
            progressBar1.Start();
            uiLog.LOG(this, "Button1_Click", uiLog.LogLevel.EVENTS, "Button 1 click event activated, progress bar started");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string [] Lines = File.ReadAllLines("C:\\temp\\TestTxt.ini", System.Text.Encoding.Default);
            richTextBox1.Text = Lines[1];
        }

        private void button3_Click(object sender, EventArgs e)
        {
            RoboMessagePanel ProgressMSG = new RoboMessagePanel(this, "Progress...", true, true);
        }
    }
}
