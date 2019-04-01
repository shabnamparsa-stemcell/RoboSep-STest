using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GUI_Console
{
    class ConsoleINI
    {
        string iniPath;
        string[] lines;


        public ConsoleINI()
        {
            iniPath = Environment.CurrentDirectory + "//GUI.ini";
            // grab data from ini file
            readINI();
        }

        private void readINI()
        {
            lines = System.IO.File.ReadAllLines(iniPath);
        }


    }
}
