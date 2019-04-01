using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Invetech.ApplicationLog;
using System.IO;

namespace Tesla.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            LogFile.GetInstance().Initialise(GetLogsPath(), "robosep_service.log");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ServiceLogin());
        }

        private static string GetLogsPath()
        {
            // Check the "logs" sub-directory exists, otherwise create it
            string startupPath = Application.StartupPath;
            int lastBackslashIndex = startupPath.LastIndexOf('\\');  // must escape the '\'
            startupPath = startupPath.Remove(lastBackslashIndex, startupPath.Length - lastBackslashIndex);
            string logsPath = startupPath + @"\logs";
            if (!Directory.Exists(logsPath))
            {
                // We're not running from the application installation directory,
                // (probably because we're running from development directories)
                // so create a "logs" directory.
                Directory.CreateDirectory(logsPath);
            }
            return logsPath;
        }
    }
}
