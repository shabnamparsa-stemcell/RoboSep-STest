using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Tesla.Common.ResourceManagement;
using Tesla.OperatorConsoleControls;
using Tesla.Common.OperatorConsole;
using Tesla.Separator;

using Invetech.ApplicationLog;

namespace GUI_Console
{
    class ConsoleINI
    {
        string iniPath;
        string[] lines;


        public ConsoleINI()
        {
            iniPath = Environment.CurrentDirectory + "//GUI.ini";

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, "GUI.ini filepath: " + iniPath);            
            // grab data from ini file
            readINI();
        }

        private void readINI()
        {
            lines = System.IO.File.ReadAllLines(iniPath);

        }


    }
}
