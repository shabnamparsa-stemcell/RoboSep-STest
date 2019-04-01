using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Windows.Forms;

namespace GUI_Console
{
    class Program
    {
        static void Main()
        {
            Form1 newForm1 = new Form1();
            Application.Run(newForm1);
//            RoboSep_UserConsole.getInstance();
            //Application.Run(RoboSep_UserConsole.myUserConsole);
        }
    }
}
