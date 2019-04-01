using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Invetech.ApplicationLog;
namespace GUI_Console
{
    public class StopWatch : IDisposable
    {
        private Stopwatch internalStopwatch;
        private string name;

        // Initializes the StopWatch instance
        public StopWatch(string name)
        {
            this.name = name;
            internalStopwatch = new Stopwatch();
            internalStopwatch.Start();
        }

        // Disposes the StopWatch instance and log the amount of time used on the operation.
        public void Dispose()
        {
            internalStopwatch.Stop();
            string logMSG = String.Format("{0} takes {1}.", name, internalStopwatch.Elapsed);
            //  (logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);             
        }
    }
}
