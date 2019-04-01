using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Tesla.Common;


namespace GUI_Controls
{
    public partial class nameHeader : UserControl
    {
        private const char reservedChar = '^';
        private const char spaceChar = ' ';

        private static string UserName;
        private static List<nameHeader> controlsList = new List<nameHeader>();
        public event EventHandler CLICK_EVENT;

        public nameHeader()
        {
            InitializeComponent();
            controlsList.Add(this);
            if (UserName != null)
                this.label_Username.Text = UserName;
        }

        public nameHeader(bool ToBeDisposed)
        {
            InitializeComponent();
            if (!ToBeDisposed)
            {
                controlsList.Add(this);
                if (UserName != null)
                    this.label_Username.Text = UserName;
            }
        }

        public static string userName
        {
            get
            {
                return UserName;
            }
            set
            {
                UserName = value;
                string sDisplayName = UserName;
                int nIndex = sDisplayName.IndexOf(reservedChar);
                if (0 < nIndex)
                {
                    sDisplayName = sDisplayName.Replace(reservedChar, spaceChar);
                }
                for (int i = 0; i < controlsList.Count; i++)
                {
                    controlsList[i].label_Username.Text = sDisplayName;
                }
            }
        }

        public string Text
        {
            get
            {
                return label_Username.Text;
            }
            set
            {
                label_Username.Text = value;
            }
        }

        private void label_Username_Click(object sender, EventArgs e)
        {
            if (CLICK_EVENT != null)
                this.CLICK_EVENT.Invoke(sender, e);
        }

        private void label_Username_Paint(object sender, PaintEventArgs e)
        {
            label_Username.Text = Utilities.TruncatedString(label_Username.Font, label_Username.Text, label_Username.MaximumSize.Width, label_Username.Margin.Left, e.Graphics);
        }

        private void nameHeader_Click(object sender, EventArgs e)
        {
            if (CLICK_EVENT != null)
                this.CLICK_EVENT.Invoke(sender, e);
        }
    }
}
