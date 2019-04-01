using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Threading;
using System.Xml.Linq;
using System.IO;

namespace FTP_Client
{
    public class FTPClient_ViewModel : INotifyPropertyChanged
    {
        //Set default full file path here - this is the file that will be sent 
        //to FTP server or USB************************************************
        private string DEFAULT_FILE = @"C:\TestFile.zip";

        private HE HE;
        private ICommand cnctCmd, discnctCmd, ftpUploadCmd, usbGetDriveCmd,
            usbUploadCmd;

        #region Variables
        private string display_msg;
        private bool allowSrcChange, allowFTPConnection, allowFTPUpload,
                allowFTPDisconnect, allowUSBUpload, allowRefreshUSB;    //For use with what actions users are allowed to perform.
        private string ftpServer, ftpUsername, ftpPassword, ftpPort;
        private string srcFilePath;
        private string selectedUSB;
        private ObservableCollection<string> usbDriveList;
        #endregion   
        
        #region Constructor
        public FTPClient_ViewModel()
        {
            HE = new HE();
            ToState(State_Initial);
            SrcFilePath = DEFAULT_FILE;

            /*string xml_path = Directory.GetParent(
                System.Reflection.Assembly.GetExecutingAssembly().Location).ToString();
            xml_path += @"\properties.xml";
            if (!File.Exists(xml_path))
            {
                CreateXML();
            }

            //Read default file path from properties.xml
            try
            {
                XDocument xdoc = XDocument.Load("properties.xml");
                SrcFilePath = xdoc.Root.Element("Default_File_Path").Value;
            }
            catch (System.IO.FileNotFoundException)
            {
                SrcFilePath = HE.file_fullPath;
                Display_MSG += "WARNING - properties.xml file not found!\r\n";
            }
            catch (System.ArgumentException)
            {
                SrcFilePath = "ERROR - properties.xml not properly written.";
            }*/

            #region Event Handling
            HE.FTPRetryingUpload += new EventHandler(this.FTPRetryingUpload);
            HE.FTPResumingUpload += new EventHandler(this.FTPResumingUpload);
            //HE.FTPConnected += new EventHandler(this.FTPConnected); //DONT NEED THIS.
            HE.FTPClosed += new EventHandler(this.FTPClosed);
            HE.FTPUploading += new EventHandler(this.FTPUploading);
            HE.FTPUploaded += new EventHandler(this.FTPUploaded);
            HE.USBUploading += new EventHandler(this.USBUploading);
            HE.USBUploaded += new EventHandler(this.USBUploaded);
            HE.FTPLoggedIn += new EventHandler(this.FTPLoggedIn);
            HE.FTPConnecting += new EventHandler(this.FTPConnecting);
            #endregion
        }
        #endregion

        #region XML
        /*private void CreateXML()
        {
            XDocument xdoc = new XDocument(
                new XElement("CONFIG",
                    new XElement("Default_File_Path", 
                        HE.file_fullPath)
                )
            );
            xdoc.Save("properties.xml");
        }*/
        #endregion

        #region States
        Action Curr_State;  //To keep track of current state
        private void SetAllowedFTPActions(bool src_c, bool ftp_c, bool ftp_u, 
            bool ftp_d)
        {
            AllowSrcChange      = src_c;
            AllowFTPConnection  = ftp_c; 
            AllowFTPUpload      = ftp_u;
            AllowFTPDisconnect  = ftp_d;
        }

        private void ToState(Action state)
        {
            if (Curr_State != state)
            {
                Curr_State = state;
                state();
            }
        }

        private void State_Initial()
        {
            SetAllowedFTPActions(true, true, false, false);
            AllowUSBUpload = true;
            AllowRefreshUSB = true;
        }

        //Not used
        private void State_SelectFile()
        {
            SetAllowedFTPActions(true, false, false, false);
            AllowUSBUpload = false;
        }

        private void State_FTPDisconnect()
        {
            SetAllowedFTPActions(true,true,false,false);
            Display_MSG += "FTP Disconnected!\r\n";
        }

        private void State_FTPConnecting()
        {
            SetAllowedFTPActions(false,false,false,false);
            Display_MSG += "FTP Connecting...\r\n";
        }

        private void State_FTPConnect()
        {
            SetAllowedFTPActions(true,false,true,true);
            Display_MSG += "FTP Connected!\r\n";
        }

        private void State_FTPUploading()
        {
            AllowSrcChange      = false;
            AllowFTPDisconnect  = false;
            AllowFTPUpload      = false;

            Display_MSG += "FTP Uploading...\r\n";
        }

        private void State_FTPUploadRetry()
        {
            Display_MSG += "Connection Lost. Attempting to Reconnect...\r\n";
        }

        private void State_FTPUploadResume()
        {
            Display_MSG += "Reconnected. Upload Resumed...\r\n";
        }

        private void State_FTPUploadDone()
        {
            AllowSrcChange      = true;
            AllowFTPDisconnect  = true;
            AllowFTPUpload      = true;

            Display_MSG += "FTP Upload Done!\r\n";
        }

        private void State_USBUploading()
        {
            AllowSrcChange  = false;
            AllowUSBUpload  = false;

            Display_MSG += "USB Uploading...\r\n";
        }

        private void State_USBUploadDone()
        {
            AllowSrcChange  = true;
            AllowUSBUpload  = true;

            Display_MSG += "USB Upload Done!\r\n";
        }
        #endregion

        #region State Change Events
        public void FTPRetryingUpload(object sender, EventArgs e)
        {
            ToState(State_FTPUploadRetry);
        }

        public void FTPResumingUpload(object sender, EventArgs e)
        {
            ToState(State_FTPUploadResume);
        }

        public void FTPConnected(object sender, EventArgs e)
        {
            ToState(State_FTPConnect);
        }

        public void FTPClosed(object sender, EventArgs e)
        {
            ToState(State_FTPDisconnect);
        }

        public void FTPUploading(object sender, EventArgs e)
        {
            ToState(State_FTPUploading);
        }

        public void FTPUploaded(object sender, EventArgs e)
        {
            ToState(State_FTPUploadDone);
        }

        public void USBUploading(object sender, EventArgs e)
        {
            ToState(State_USBUploading);
        }

        public void USBUploaded(object sender, EventArgs e)
        {
            ToState(State_USBUploadDone);
        }

        public void FTPLoggedIn(object sender, EventArgs e)
        {
            //ToState(State_FTPLoggedIn);
            ToState(State_FTPConnect);
        }

        public void FTPConnecting(object sender, EventArgs e)
        {
            ToState(State_FTPConnecting);
        }
        #endregion

        #region ICommands
        public ICommand CnctCmd
        {
            get
            {
                if (cnctCmd == null)
                    cnctCmd = new MyCommand(Exec_CnctCmd);
                return cnctCmd;
            }
            set { cnctCmd = value; }
        }

        public ICommand DiscnctCmd
        {
            get
            {
                if (discnctCmd == null)
                    discnctCmd = new MyCommand(Exec_DiscnctCmd);
                return discnctCmd;
            }
            set { discnctCmd = value; }
        }

        public ICommand FTPUploadCmd
        {
            get
            {
                if (ftpUploadCmd == null)
                    ftpUploadCmd = new MyCommand(Exec_FTPUploadCmd);
                return ftpUploadCmd;
            }
            set { ftpUploadCmd = value; }
        }

        public ICommand USBGetDriveCmd
        {
            get
            {
                if (usbGetDriveCmd == null)
                    usbGetDriveCmd = new MyCommand(Exec_USBGetDriveCmd);
                return usbGetDriveCmd;
            }
            set { usbGetDriveCmd = value; }
        }

        public ICommand USBUploadCmd
        {
            get
            {
                if (usbUploadCmd == null)
                    usbUploadCmd = new MyCommand(Exec_USBUploadCmd);
                return usbUploadCmd;
            }
            set { usbUploadCmd = value; }
        }
        #endregion

        #region On Error
        private void OnError(Exception ex)
        {
            //Display_MSG += "ERROR: "+ex.Message+"\r\n";
            OnError(ex.Message);
        }

        private void OnError(string msg)
        {
            Display_MSG += "ERROR: " + msg + "\r\n";
        }

        private void SourceFileError()
        {
            OnError("Source File Not Set!");
        }
        #endregion

        #region Methods
        private void ConnectFTPServer(object stateinfo)
        {
            try
            {
                int port = Convert.ToInt32(FTPPort);
                HE.ConnectServer(FTPServer, FTPUsername, FTPPassword, port);
            }
            catch (FormatException)
            {
                OnError("Check Port Number or IP!");
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

        private void DisconnectFTP(object stateinfo)
        {
            HE.DisconnectServer();
        }

        private void UploadToFTP(object stateinfo)
        {
            if (!File.Exists(SrcFilePath))
            {
                SourceFileError();
                return;
            }
            HE.UploadToFTP();
        }

        private void GetUSBDriveList(object stateinfo)
        {
            AllowRefreshUSB = false;
            USBDriveList = new ObservableCollection<string>(HE.GetUSBDrives());
            AllowRefreshUSB = true;
        }

        private void UploadToUSB(object stateinfo)
        {
            if (!File.Exists(SrcFilePath))
            {
                SourceFileError();
                return;
            }
            if (SelectedUSB != null)
                HE.UploadToUSB(SelectedUSB);
        }
        #endregion

        #region Command Execution
        //All model commands in separate worker threads
        private void Exec_CnctCmd()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(ConnectFTPServer));
        }

        private void Exec_DiscnctCmd()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(DisconnectFTP));
        }

        private void Exec_FTPUploadCmd()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(UploadToFTP));
        }

        private void Exec_USBGetDriveCmd()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(GetUSBDriveList));
        }

        private void Exec_USBUploadCmd()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(UploadToUSB));
        }
        #endregion

        #region On Variable Changes
        public ObservableCollection<string> USBDriveList
        {
            get { return usbDriveList; }
            set
            {
                if (usbDriveList != value)
                {
                    usbDriveList = value;
                    OnPropertyChanged("USBDriveList");
                }
            }
        }

        public bool AllowSrcChange
        {
            get { return allowSrcChange; }
            set
            {
                if (allowSrcChange != value)
                {
                    allowSrcChange = value;
                    OnPropertyChanged("AllowSrcChange");
                }
            }
        }

        public bool AllowFTPConnection
        {
            get { return allowFTPConnection; }
            set
            {
                if (allowFTPConnection != value)
                {
                    allowFTPConnection = value;
                    OnPropertyChanged("AllowFTPConnection");
                }
            }
        }

        public bool AllowFTPUpload
        {
            get { return allowFTPUpload; }
            set
            {
                if (allowFTPUpload != value)
                {
                    allowFTPUpload = value;
                    OnPropertyChanged("AllowFTPUpload");
                }
            }
        }

        public bool AllowFTPDisconnect
        {
            get { return allowFTPDisconnect; }
            set
            {
                if (allowFTPDisconnect != value)
                {
                    allowFTPDisconnect = value;
                    OnPropertyChanged("AllowFTPDisconnect");
                }
            }
        }

        public bool AllowUSBUpload
        {
            get { return allowUSBUpload; }
            set
            {
                if (allowUSBUpload != value)
                {
                    allowUSBUpload = value;
                    OnPropertyChanged("AllowUSBUpload");
                }
            }
        }

        public bool AllowRefreshUSB
        {
            get { return allowRefreshUSB; }
            set
            {
                if (allowRefreshUSB != value)
                {
                    allowRefreshUSB = value;
                    OnPropertyChanged("AllowRefreshUSB");
                }
            }
        }

        public string Display_MSG
        {
            get { return display_msg; }
            set
            {
                if (display_msg != value)
                {
                    display_msg = value;
                    OnPropertyChanged("Display_MSG");
                }
            }
        }

        public string FTPServer
        {
            get { return ftpServer; }
            set
            {
                if (ftpServer != value)
                {
                    ftpServer = value;
                    OnPropertyChanged("FTPServer");
                }
            }
        }


        public string FTPUsername
        {
            get { return ftpUsername; }
            set
            {
                if (ftpUsername != value)
                {
                    ftpUsername = value;
                    OnPropertyChanged("FTPUsername");
                }
            }
        }

        public string FTPPassword
        {
            get { return ftpPassword; }
            set
            {
                if (ftpPassword != value)
                {
                    ftpPassword = value;
                    OnPropertyChanged("FTPPassword");
                }
            }
        }

        public string FTPPort
        {
            get { return ftpPort; }
            set
            {
                if (ftpPort != value)
                {
                    ftpPort = value;
                    OnPropertyChanged("FTPPort");
                }
            }
        }

        public string SrcFilePath
        {
            get { return srcFilePath; }
            set
            {
                if (srcFilePath != value)
                {
                    if (!File.Exists(value))
                    {
                        //ToState(State_SelectFile);
                        return;
                    }
                    srcFilePath = value;
                    HE.SetSourcePath(value);
                    Display_MSG += "Source Path Set!\r\n";

                    OnPropertyChanged("SrcFilePath");
                }
            }
        }

        public string SelectedUSB
        {
            get { return selectedUSB; }
            set
            {
                if (selectedUSB != value)
                {
                    selectedUSB = value;
                    OnPropertyChanged("SelectedUSB");
                }
            }
        }
        #endregion

        #region OnPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
