using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace GUI_Controls
{
    public class uiLog
    {
        private static uiLog myLogFile;
        private string  strLogPath;
        private LogLevel outLevel;
        private string[] history;   // remembers the previous 10 lines to be posted when error occurs

        public enum LogLevel
        {
            GENERAL,
            ERROR,
            WARNING,
            EVENTS,
            INFO,
            DEBUG
        };

        private uiLog()
        {
            do
            {
                strLogPath = GetstrLogPath();
            } while (strLogPath == string.Empty);
            
            // set standard log level
            outLevel = LogLevel.DEBUG;
            history = new string[10];
            for (int i = 0; i < 10; i++) { history[i] = string.Empty; }

            // initialize file with header
            InitializeFile( strLogPath );
        }

        public static uiLog getInstance()
        {
            if (myLogFile == null)
                myLogFile = new uiLog();
            return myLogFile;
        }

        private string GetstrLogPath()
        {
            try
            {
                // Check the "protocols" directory exists, otherwise create it
                string startupPath = Application.StartupPath;
                int lastBackslashIndex = startupPath.LastIndexOf('\\');  // must escape the '\'
                startupPath = startupPath.Remove(lastBackslashIndex, startupPath.Length - lastBackslashIndex);
                string strLogPath = startupPath + @"\logs\";
                if (!Directory.Exists(strLogPath))
                {
                    // We're not running from the application installation directory,
                    // (probably because we're running from development directories)
                    // so create a  "protocols" directory.
                    Directory.CreateDirectory(strLogPath);
                }

                // now generate log file and return name and path of file
                DateTime GenerationTime = DateTime.Now;
                string logGenTime = string.Format("{0:yyMMdd_HHmmss}", GenerationTime);
                strLogPath += logGenTime + "_UI" + ".log";

                // create ne log file
                //File.Create(strLogPath);

                return strLogPath;
            }
            catch
            {
                return string.Empty;
            }
        }

        private void InitializeFile(string logPath)
        {
            string header1 = "RoboSep UI Log File";
            string header2 = "Created: " + LogTime();

            try
            {
                // write log line to current log file
                using (StreamWriter sw = File.CreateText(logPath))
                {
                    sw.WriteLine(header1);
                    sw.WriteLine(header2);
                    sw.WriteLine("");
                }
            }
            catch
            {
                // get prompt message
                string msg = GUI_Controls.UserDetails.getValue("GUI", "msgLogWriteFail");
                msg = msg == null ? "Unable to write to log file '" : msg;
                msg += logPath + "'";

                string title = "File Write Error";

                RoboMessagePanel err = new RoboMessagePanel(Application.OpenForms[0], MessageIcon.MBICON_ERROR, msg, title);
                err.ShowDialog();
            }
        }

        private string LogTime()
        {
            string LTIME = string.Empty;
            DateTime dt = DateTime.Now;
            LTIME = string.Format("{0:yyyyMMdd_HHmmss.fff}", dt);
            return LTIME;
        }

        private string Origin( object LogOrigin )
        {
            Type objType = LogOrigin.GetType();
            return objType.Name;
        }

        private string lvl(LogLevel LVL)
        {
            string level = string.Empty;
            switch (LVL)
            {
                case LogLevel.DEBUG:
                    level = "Debug";
                    break;
                case LogLevel.INFO:
                    level = "Info";
                    break;
                case LogLevel.WARNING:
                    level = "Warning";
                    break;
                case LogLevel.GENERAL:
                    level = "General";
                    break;
                case LogLevel.EVENTS:
                    level = "Info";
                    break;
                case LogLevel.ERROR:
                    level = "Error";
                    break;
            }
            return level;
        }

        public static void LOG(object origin, string FTN, LogLevel logLVL, string msg)
        {
            // get current Log file
            uiLog mylog = uiLog.getInstance();
            
            
            string TIME = mylog.LogTime();
            string LEVEL = mylog.lvl(logLVL);
            string ORIGIN = mylog.Origin(origin);

            string logLine = TIME + "\tclt\t" + ORIGIN + "." + FTN + "()" + "\t" + LEVEL + "\t" + ORIGIN + "\t" + msg;

            // check if log level is a higher or equal to output level
            if ( logLVL == LogLevel.ERROR)
                mylog.logError();
            if ((int)logLVL <= (int)mylog.outLevel)
            {
                try
                {
                    // write log line to current log file
                    using (StreamWriter sw = File.AppendText(mylog.strLogPath))
                    {
                        sw.WriteLine(logLine);
                    }
                }
                catch
                {
                    // do nothing
                }
            }
            // write log line indeterminate of output level
            Debug.WriteLine( logLine );
            // update history
            List<string> tempHistory = new List<string>();
            tempHistory.AddRange(mylog.history);
            mylog.history[0] = logLine;
            for (int i = 1; i < mylog.history.Length; i++)
                mylog.history[i] = tempHistory[i - 1];
        }

        private void logError()
        {
            // get entire log file lines
            string[] allLines = File.ReadAllLines( strLogPath );
            int EntryLine = -1 ;
            // find first matching line
            for (int i = 0; i < history.Length; i++)
            {
                // look at last line first, and work back
                for (int j = allLines.Length-1; j > (allLines.Length-(history.Length +1)); j--)
                {
                    if (j > 0)
                    {
                        if (history[i] == allLines[j])
                        {
                            // remember number of matching line
                            EntryLine = j;
                            break;
                        }
                    }
                }
                if (EntryLine != -1)
                    break;
            }
            if (EntryLine == -1)
                EntryLine = allLines.Length;

            // replace matching line and all lines pas that with history
            List<string> IncHistory = new List<string>();
            for (int i = 0; i < EntryLine; i++)
                IncHistory.Add(allLines[i]);
            for (int i = 0; i < history.Length; i++)
                IncHistory.Add(history[i]);
            allLines = new string[IncHistory.Count];
            for (int i = 0; i < IncHistory.Count; i++)
                allLines[i] = IncHistory[i];
            File.Delete(strLogPath);
            File.WriteAllLines(strLogPath, allLines);
        }

        public static void outputLevel(LogLevel setLevel)
        {
            // get current Log file
            uiLog mylog = uiLog.getInstance();

            // set log level
            mylog.outLevel = setLevel;
        }
    }
}
