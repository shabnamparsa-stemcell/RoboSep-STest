using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace FTP_Client
{
    class MyCommand : ICommand
    {
        private readonly Action execute;
        public event EventHandler CanExecuteChanged;

        public MyCommand(Action execute)
        {
            this.execute = execute;
        }

        public bool CanExecute(object param)
        {
            return true;
        }

        public void Execute(object param)
        {
            execute();
        }
    }
}
