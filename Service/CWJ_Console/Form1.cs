using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Configuration;
using Tesla.DataAccess;

using GUI_Controls;

using System.IO.Compression;
using System.IO;

namespace GUI_Console
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int value = Convert.ToInt32(textBox4.Text);
            textBox3.Text = value.ToString();
        }

        RoboMessagePanel ProgressMSG;
        private void button1_Click(object sender, EventArgs e)
        {
            ProgressMSG = new RoboMessagePanel(this, "Progress...", true, true);
            ProgressMSG.Show();
        }

        private int count = 0;
        private void button2_Click(object sender, EventArgs e)
        {
            if (count < 100)
            {
            count += 10;
            ProgressMSG.setProgress(count);
            }
        }

        private void compressFiles()
        {

        }
       
    }
}
