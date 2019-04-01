using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Util_GUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form_Config formConfig = new Form_Config();
            Application.Run(formConfig);
            DialogResult nRetCode = formConfig.DialogResult;
            return (int)nRetCode;
        }
    }
}
