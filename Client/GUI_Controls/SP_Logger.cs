using System;
using System.Collections.Generic;
using System.Text;
using pgmLog;

namespace GUI_Controls
{
    public class SP_Logger
    {
        // commented error levels are available in the logger but not used/activated within pgmLog
        public const int OFF = int.MaxValue;
        //public const int FATAL = 110000;
        //public const int ALERT = 100000;
        public const int CRITICAL = 90000;
        //public const int SEVERE = 80000;
        public const int ERROR = 70000;
        public const int WARN = 60000;
        public const int ID = 50000;
        public const int INFO = 40000;
        public const int DEBUG = 30000;
        //public const int TRACE = 20000;
        public const int VERBOSE = 10000;
        public const int ALL = int.MinValue;

        //private static SP_Logger myGuiLog;
        public static pgmLog.pLog log;

        public static void initializeLog(bool is32Bit)
        {
            if (log == null)
            {
                string fileName = "pgmlog_clt.stlog";
                int bufferCapacity = 5;

                int OSbit = UserDetails.getOSArchitecture();
                string syspath = OSbit == 32 || is32Bit ? @"C:\Program Files\STI\RoboSep\logs\"
                    : @"C:\Program Files (x86)\STI\RoboSep\logs\";
                fileName = syspath + GetTimestamp(DateTime.Now) + "_clt.stlog";

                // intialise using one of the 3 calls
                // using default levels: (filename, 64000, Level.Debug, Level.Info, Level.Error, Level.Off);
                //pgmLog.pgmLoggerInit.init(fileName);
                // using integer level definitions
                pgmLog.pgmLoggerInit.init(fileName, bufferCapacity, DEBUG, INFO, ERROR, OFF);
                // using string level definitions
                //pgmLog.pgmLoggerInit.init(fileName, bufferCapacity, "DEBUG", "INFO", "ERROR", "OFF");

                log = pgmLog.pgmLogManager.GetLogger("CLT_Logger");

                log.Critical("Log CLT_Logger initialized");
            }
        }
        public static void Close()
        {
            log.Critical("Log CLT_Logger closing...");
            pgmLog.pgmLoggerInit.close();

        }

        private static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMdd_HHmm");
        }
    }
}
