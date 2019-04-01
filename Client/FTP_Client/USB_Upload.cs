#region Namespace Inclusions
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.IO;
#endregion

namespace FTP_Client
{
    public class USB_Upload
    {
        #region variables
        List<string> myDiskLetters, myDiskName;
        ManagementObjectSearcher myDisks;
        string file_name;
        string source_path;   //The location where the file to upload 
        string target_path;   //The location (USB) where file is to be copied to
        public bool usb_uploading { get; private set; }
        #endregion

        #region Event Delegate
        public delegate void USBUploadChangedHandler(
            object sender, USBUploadChangedEventArgs e);
        public event USBUploadChangedHandler USBUploadChanged;
        #endregion

        #region Constructor
        public USB_Upload()
        {
            myDisks = new ManagementObjectSearcher
                ("SELECT * FROM Win32_DiskDrive WHERE InterfaceType='USB'");
            myDiskLetters = new List<string>();
            myDiskName = new List<string>();
            usb_uploading = false;

            //Default Source File Path
            file_name = @"TestFile.zip";
            source_path = @"C:\Users\mluk\Desktop\Test Files\";
        }
        #endregion

        #region Methods: USB Disk Names
        public void setDiskLetters()
        {
            myDiskLetters.Clear();
            foreach (ManagementObject m in myDisks.Get())
            {
                myDiskName.Add((string)m["Caption"]);
                foreach (ManagementObject partition in new ManagementObjectSearcher(
                    "ASSOCIATORS OF {Win32_DiskDrive.DeviceID='" 
                    + m["DeviceID"]
                    + "'} WHERE AssocClass = Win32_DiskDriveToDiskPartition").Get())
                {
                    foreach (ManagementObject vol in new ManagementObjectSearcher(
                        "ASSOCIATORS OF {Win32_DiskPartition.DeviceID='"
                        + partition["DeviceID"]
                        + "'} WHERE AssocClass = Win32_LogicalDiskToPartition").Get())
                    {
                        myDiskLetters.Add((string)vol["Name"]);
                    }
                }
            }
        }

        public List<string> getDiskLetter()
        {
            return myDiskLetters;
        }
        #endregion

        #region Methods: File Copying
        public void SetSourcePath(string path)
        {
            source_path = path;
        }

        public void SetFileName(string name)
        {
            file_name = name;
        }

        public void SetSrcFromFullPath(string fullPath)
        {
            source_path = Path.GetDirectoryName(fullPath);
            file_name = Path.GetFileName(fullPath);
        }

        //Set target path
        public void SetTargetUSBPath(string letter)
        {
            target_path = letter[0] + @":\";
        }

        //Copy file from source to destination
        public void CopyToUSB()
        {
            USB_Uploading = true;
            if (Directory.Exists(source_path) && Directory.Exists(target_path))
            {
                string source_file = Path.Combine(source_path, file_name);
                string dest_file = Path.Combine(target_path, file_name);

                File.Copy(source_file, dest_file, true);
            }
            USB_Uploading = false;
        }
        #endregion

        #region Event Trigger
        public void OnUSBUploadChanged()
        {
            USBUploadChanged(this, new USBUploadChangedEventArgs(usb_uploading));
        }
        private bool USB_Uploading
        {
            set
            {
                usb_uploading = value;
                OnUSBUploadChanged();
            }
        }
        #endregion
    }

    #region Event Args
    public class USBUploadChangedEventArgs : EventArgs
    {
        private bool usb_uploading;

        public USBUploadChangedEventArgs(bool usb_upl)
        {
            usb_uploading = usb_upl;
        }

        public bool USB_Uploading
        {
            get { return usb_uploading; }
        }
    }
    #endregion
}
