#region Namespace Inclusions
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using EnterpriseDT.Net.Ftp;
using System.Threading;

using System.Diagnostics;
#endregion

namespace FTP_Client
{
    public class FTP_HE
    {
        #region Variables
        FTPConnection myFTP;
        string file_fullPath;   //Entire file path+name
        string file_name;       //Just the file name; name file is saved on server      
        volatile bool retrying; //true when retrying to upload, false otherwise
        #endregion

        #region Event Handlers
        public event EventHandler BeginRetrying, ResumingUpload;
        public event EventHandler Connected, Closed;
        public event EventHandler Uploading, Uploaded;
        public event EventHandler LoggedIn; //This replaces the "Connected" event
        public event EventHandler Connecting;
        #endregion

        #region Constructor
        public FTP_HE()
        {
            myFTP = new FTPConnection();
            retrying = false;

            //Default Values
            myFTP.ServerPort = 21;
            myFTP.ConnectMode = FTPConnectMode.PASV;
            
            //Default File Path
            file_fullPath = @"C:\Users\mluk\Desktop\FTP Client\Test Files\TestFile.zip";
            file_name = @"TestFile.zip";

            #region Event Handler
            //myFTP.Connected += new FTPConnectionEventHandler(myFTP_Connected);
            myFTP.Closed += new FTPConnectionEventHandler(myFTP_Closed);
            myFTP.Uploading += new FTPFileTransferEventHandler(myFTP_Uploading);
            myFTP.Uploaded += new FTPFileTransferEventHandler(myFTP_Uploaded);
            myFTP.LoggedIn += new FTPLogInEventHandler(myFTP_LoggedIn);
            myFTP.Connecting += new FTPConnectionEventHandler(myFTP_Connecting);
            #endregion
        }
        #endregion

        #region Methods: FTP Connection
        public void SetFTPSettings(string ip, string user, string pass, int port)
        {
            try
            {
                myFTP.ServerAddress = ip;
                myFTP.UserName = user;
                myFTP.Password = pass;
                myFTP.ServerPort = port;
            }
            catch (FTPException)
            {
                //Debug.WriteLine("FTPException: Already Connected 1");
                throw new FTPException("Already Connected!");
            }
        }

        public bool ConnectFTP()
        {
            bool bRet = false;
            try
            {
                myFTP.Connect();
                bRet = true;
            }
            catch (FTPAuthenticationException)
            {
                //NOTE: If connected, but wrong login information, client will
                //be automatically disconnected.
                //Debug.WriteLine("AuthExeption: Invalid Username/Password");
                CloseFTP();
                throw new Exception("Invalid Username/Password!");
            }
            catch (IOException)
            {
                //Could be invalid server IP or port number, or server offline
                //Debug.WriteLine("IOException: Failed to Connect");
                throw new Exception("Unable to connect!");
            }
            catch (ArgumentNullException)
            {
                //Client may still connect if there is empty field.
                //Debug.WriteLine("NullException: Empty Field");
                CloseFTP();
                throw new Exception("Empty Field!");
            }
            catch (System.Net.Sockets.SocketException)
            {
                //Debug.WriteLine("SocketException: No Response from Server");
                throw new Exception("No response from server!");
            }
            catch (FTPException)
            {
                //Debug.WriteLine("FTPException: Already Connected 2");
                throw new Exception("Unknown FTPException!");
            }
            return bRet;
        }

        public void CloseFTP()
        {
            try
            {
                myFTP.Close();
            }
            catch (FTPException)
            {
                //Debug.WriteLine("FTPException: No Connection to Close");
                throw new Exception("No Connection to Close!");
            }
            catch (ControlChannelIOException)
            {
                //Debug.WriteLine("ControlChannelIOException: Connection Already Aborted by Server");
                throw new Exception("Connection already aborted by server!");
            }
        }
        #endregion

        #region Methods: File Path
        /*public void GetFileList()
        {
            try
            {
                myFiles = myFTP.GetFiles();
            }
            catch (FTPException)
            {
                Debug.WriteLine("FTPException: Not Yet Connected to Server");
            }
        }*/

        public void SetFilePath(string fullPath)
        {
            file_fullPath = fullPath;
            file_name = Path.GetFileName(fullPath);

            //file_stream = File.OpenRead(path);
            //file_stream.Seek(0, SeekOrigin.Begin);
        }

        public string GetFilePath()
        {
            return file_fullPath;
        }
        #endregion

        #region Methods: File Upload
        public void UploadFile()
        {
            try
            {
                myFTP.UploadFile(file_fullPath, file_name);
            }
            catch (FTPException)
            {
                //Debug.WriteLine("FTPException: Upload Failed");
                throw new Exception("FTP Upload Failed!");
            }
            catch (IOException)
            {
                RetryUpload();
            }
        }

        //Used in UploadFile(). Attempts to resume upload if disconnected from server.
        public void RetryUpload()
        {
            retrying = true;
            if (this.BeginRetrying != null)
                this.BeginRetrying(this, new EventArgs());
            while(retrying)
            {
                try
                {
                    Debug.WriteLine("Attempting to Reconnect");
                    if (myFTP.IsConnected)
                    {
                        myFTP.Close();
                    }
                    myFTP.Connect();
                }
                catch
                {
                    Debug.WriteLine("Reconnect Fail. Retry again.");
                    Thread.Sleep(1000);
                    continue;
                }

                try
                {
                    if (this.ResumingUpload != null)
                        this.ResumingUpload(this, new EventArgs());
                    myFTP.ResumeTransfer();
                    myFTP.UploadFile(file_fullPath, file_name, true);
                    break;
                }
                catch
                {
                    Debug.WriteLine("Resume Upload Fail. Retry again.");
                    if (this.BeginRetrying != null)
                        this.BeginRetrying(this, new EventArgs());
                    Thread.Sleep(1000);
                }
            }
            retrying = false;
        }

        public void StopRetry()
        {
            retrying = false;
        }
        #endregion

        #region Event Triggers
        private void myFTP_Connected(object sender, FTPConnectionEventArgs e)
        {
            if (this.Connected != null && e.IsConnected)
                this.Connected(this, new EventArgs());
        }

        private void myFTP_Closed(object sender, FTPConnectionEventArgs e)
        {
            if (this.Closed != null && !retrying)
                this.Closed(this, new EventArgs());
        }

        private void myFTP_Uploading(object sender, FTPFileTransferEventArgs e)
        {
            if (this.Uploading != null)
                this.Uploading(this, new EventArgs());
        }

        private void myFTP_Uploaded(object sender, FTPFileTransferEventArgs e)
        {
            if (this.Uploaded != null && e.Succeeded)
                this.Uploaded(this, new EventArgs());
        }

        private void myFTP_LoggedIn(object sender, FTPLogInEventArgs e)
        {
            if (this.LoggedIn != null && e.HasLoggedIn)
                this.LoggedIn(this, new EventArgs());
        }

        private void myFTP_Connecting(object sender, FTPConnectionEventArgs e)
        {
            if (this.Connecting != null)
                this.Connecting(this, new EventArgs());
        }
        #endregion
    }
}
