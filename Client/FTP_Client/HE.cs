#region Namespace Inclusions
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
//using System.ComponentModel;
#endregion

namespace FTP_Client
{
    public class HE
    {
        #region Class Objects
        private FTP_HE FTP_HE;
        private USB_Upload USB_HE;
        #endregion

        #region Variables
        public string file_fullPath { get; private set; }
        #endregion

        #region Event Handlers
        public event EventHandler FTPRetryingUpload,FTPResumingUpload;
        public event EventHandler FTPConnected, FTPClosed;
        public event EventHandler FTPUploading, FTPUploaded;
        public event EventHandler USBUploading, USBUploaded;
        public event EventHandler FTPLoggedIn, FTPConnecting;
        #endregion

        #region Constructor
        public HE()
        {
            FTP_HE = new FTP_HE();
            USB_HE = new USB_Upload();

            //Default File Path
            file_fullPath = @"C:\Users\mluk\Desktop\FTP Client\Test Files\TestFile.zip";
            this.SetSourcePath(file_fullPath);

            #region Event Handler
            USB_HE.USBUploadChanged += new USB_Upload.USBUploadChangedHandler(
                this.USBUploadChanged);
            //FTP_HE.Connected += new EventHandler(ftp_connected);
            FTP_HE.Closed += new EventHandler(ftp_closed);
            FTP_HE.Uploading += new EventHandler(ftp_uploading);
            FTP_HE.Uploaded += new EventHandler(ftp_uploaded);
            FTP_HE.BeginRetrying += new EventHandler(ftp_beginRetrying);
            FTP_HE.ResumingUpload += new EventHandler(ftp_resumingUpload);
            FTP_HE.LoggedIn += new EventHandler(ftp_loggedIn);
            FTP_HE.Connecting += new EventHandler(ftp_connecting);
            #endregion
        }
        #endregion

        #region Event Triggers
        public void ftp_beginRetrying(object sender, EventArgs e)
        {
            if (this.FTPRetryingUpload != null)
                this.FTPRetryingUpload(this, new EventArgs());
        }

        public void ftp_resumingUpload(object sender, EventArgs e)
        {
            if (this.FTPResumingUpload != null)
                this.FTPResumingUpload(this, new EventArgs());
        }

        public void ftp_connected(object sender, EventArgs e)
        {
            if (this.FTPConnected != null)
                this.FTPConnected(this, new EventArgs());
        }

        public void ftp_closed(object sender, EventArgs e)
        {
            if (this.FTPClosed != null)
                this.FTPClosed(this, new EventArgs());
        }

        public void ftp_uploading(object sender, EventArgs e)
        {
            if (this.FTPUploading != null)
                this.FTPUploading(this, new EventArgs());
        }

        public void ftp_uploaded(object sender, EventArgs e)
        {
            if (this.FTPUploaded != null)
                this.FTPUploaded(this, new EventArgs());
        }

        public void USBUploadChanged(object sender, USBUploadChangedEventArgs e)
        {
            if (e.USB_Uploading)
            {
                if (this.USBUploading != null)
                    this.USBUploading(this, new EventArgs());
            }
            else
            {
                if (this.USBUploaded != null)
                    this.USBUploaded(this, new EventArgs());
            }
        }

        public void ftp_loggedIn(object sender, EventArgs e)
        {
            if (this.FTPLoggedIn != null)
                this.FTPLoggedIn(this, new EventArgs());
        }

        public void ftp_connecting(object sender, EventArgs e)
        {
            if (this.FTPConnecting != null)
                this.FTPConnecting(this, new EventArgs());
        }
        #endregion

        #region Other Methods
        //Use this to set source path for both USB and FTP to be identical
        public void SetSourcePath(string fullPath)
        {
            file_fullPath = fullPath;
            SetSourcePathFTP(fullPath);
            SetSourcePathUSB(fullPath);
        }
        #endregion

        #region Methods: FTP
        public void ConnectServer(string ip,string user,string password, int port)
        {
            FTP_HE.SetFTPSettings(ip, user, password, port);

            FTP_HE.ConnectFTP();
        }

        public void DisconnectServer()
        {
            FTP_HE.CloseFTP();
        }

        public void UploadToFTP()
        {
            FTP_HE.UploadFile();
        }

        private void SetSourcePathFTP(string fullPath)
        {
            FTP_HE.SetFilePath(fullPath);
        }

        public void StopUploadRetry()
        {
            FTP_HE.StopRetry();
        }
        #endregion

        #region Methods: USB
        private void SetSourcePathUSB(string fullPath)
        {
            USB_HE.SetSrcFromFullPath(fullPath);
        }

        public void UploadToUSB(string letter)
        {
            USB_HE.SetTargetUSBPath(letter);
            USB_HE.CopyToUSB();
        }

        //Returns a list of attached USB drives
        public List<string> GetUSBDrives()
        {
            USB_HE.setDiskLetters();
            return USB_HE.getDiskLetter();
        }
        #endregion

        
    }

}
