using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Xml;
using System.Text;
using System.Windows.Forms;

namespace GUI_Controls
{
    public partial class TestButton : UserControl
    {
        List<Image> BackImage = new List<Image>();

        public TestButton()
        {
            InitializeComponent();
            BackImage.Add(Properties.Resources.Button_RUN0);
            BackImage.Add(Properties.Resources.Button_RUN2);
            BackImage.Add(Properties.Resources.Button_RUN3);
        }

        private void TestButton_MouseEnter(object sender, EventArgs e)
        {
            this.BackgroundImage = BackImage[1];
        }

        private void TestButton_MouseLeave(object sender, EventArgs e)
        {
            this.BackgroundImage = BackImage[0];
        }

        private void TestButton_Click(object sender, EventArgs e)
        {
            this.BackgroundImage = BackImage[2];
        }
    }
}
