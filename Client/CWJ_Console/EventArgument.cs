using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.Linq;


using Tesla.Common.Protocol;


namespace GUI_Console
{
 
    public class IntegerEventArgs : EventArgs
    {
        public IntegerEventArgs(int intValue)
        {
            this.myInteger = intValue;
        }

        private int myInteger;
        public int MyInteger
        {
            get
            {
                return myInteger; 
            }
            set
            {
                myInteger = value;
            }
        }
    }

    public class StringEventArgs : EventArgs
    {
        public StringEventArgs(string sValue)
        {
            this.myString = sValue;
        }

        private string myString;
        public string MyString
        {
            get
            {
                return myString;
            }
            set
            {
                myString = value;
            }
        }
    }


    public class ProtocolEventArgs : EventArgs
    {
        public ProtocolEventArgs(IProtocol[] pProtocols )
        {
            if (pProtocols != null)
            {
                myProtocols = new IProtocol[pProtocols.Length];
                pProtocols.CopyTo(myProtocols, 0);
            }
        }

        private IProtocol[] myProtocols;
        public IProtocol[] MyProtocols
        {
            get
            {
                return myProtocols;
            }
            set
            {
                myProtocols = value;
            }
        }
    }

    public class CopyFileEventArgs : EventArgs    
    {
        private Dictionary<string, bool> dictFileNames = new Dictionary<string,bool>();
        public int TotalFiles { get; set; }
        public int FileNumber { get; set; }  

        public CopyFileEventArgs(string[] FileNames)
        {
            dictFileNames.Clear();

            if (FileNames == null || FileNames.Length == 0)
                return;

            for (int i = 0; i < FileNames.Length; i++)
            {
                if (!dictFileNames.ContainsKey(FileNames[i]))
                    dictFileNames.Add(FileNames[i], false);
            }
        }

        public Dictionary<string, bool> DictFileNames
        {
            get { return dictFileNames; }
            set
            {
                dictFileNames.Clear();
                dictFileNames = value.ToDictionary(entry => entry.Key,
                                               entry => entry.Value);
            }
        }
    }


    public delegate void ReloadProtocolsFinishedDeledate(object sender, EventArgs e);
    public delegate void SeparatorIsReadyDelegate(object sender, IntegerEventArgs e);
    public delegate void ChosenProtocolsFinishedDelegate(object sender, ProtocolEventArgs e);


}
