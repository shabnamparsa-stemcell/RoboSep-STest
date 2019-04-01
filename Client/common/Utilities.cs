using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;


using Microsoft.VisualBasic.PowerPacks;

namespace Tesla.Common
{

	/// <summary>
	/// Summary description for Utilities.
	/// </summary>
    /// 
	public class Utilities
	{
        public enum BeepTypes : long
        {
            MB_OK = 0x0,
            MB_ICONHAND = 0x00000010,
            MB_ICONQUESTION = 0x00000020,
            MB_ICONEXCLAMATION = 0x00000030,
            MB_ICONASTERISK = 0x00000040,
            SIMPLE_BEEP = 0xffffffff
        };

		public Utilities()
		{
		}

        public static int getOsInfo()
        {
            int DataBit = -1;
            try
            {
                string programeFiles = ProgramFiles();
                if (programeFiles.Contains("x86"))
                {
                    DataBit = 64;
                }
                else
                {
                    DataBit = 32;
                }
            }
            catch (Exception)
            {
                return 32;
            }
            return DataBit;
        }

        public bool Is64bitOS
        {
            get { return (Environment.GetEnvironmentVariable("ProgramFiles(x86)") != null); }
        }

        public static string ProgramFiles()
        {
            string programFiles = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            if (programFiles == null)
            {
                programFiles = Environment.GetEnvironmentVariable("ProgramFiles");
            }

            return programFiles;
        }

        public static string GetRoboSepSTIpath()
        {
            string RoboSepSTIpath = ProgramFiles();
            RoboSepSTIpath += "\\STI\\";
            return RoboSepSTIpath;
        }


        public static string GetRoboSepSysPath()
        {
            string RoboSepSysPath = ProgramFiles();
            RoboSepSysPath += "\\STI\\RoboSep\\";
            return RoboSepSysPath;
        }

        public static string GetTempFileFolder()
        {
            StringBuilder temp = new StringBuilder();
            string systemPath = GetRoboSepSysPath();
            temp.Append(systemPath);
            temp.Append("temp\\");
            temp.Append(GetCurrentTimeStamp());
            return temp.ToString();
        }

        public static bool RemoveTempFileDirectory(string folderName)
        {
            if (string.IsNullOrEmpty(folderName))
                return false;

            string sysTempPath = GetRoboSepSysPath();
            sysTempPath += "temp";

            // Check if the folder name contains the temp folder
            string temp = folderName;
            if (!temp.Contains(sysTempPath))
                return false;

            temp = temp.Replace(sysTempPath, string.Empty);
            if (string.IsNullOrEmpty(temp))
                return false;

            string tempDirectory = "";
            string[] tokens = temp.Split('\\');

            if (tokens == null || tokens.Length == 0)
                return false;

            foreach (string token in tokens)
            {
                if (string.IsNullOrEmpty(token))
                    continue;

                tempDirectory = token.Trim();
                break;
            }

            if (string.IsNullOrEmpty(tempDirectory))
                return false;

            string tempFileBaseFolder = sysTempPath;
            tempFileBaseFolder += "\\";
            tempFileBaseFolder += tempDirectory;
            bool bRet = true;
            try
            {
                if (Directory.Exists(tempFileBaseFolder))
                {
                    RemoveFilesInDirectory(tempFileBaseFolder);
                    Directory.Delete(tempFileBaseFolder, true);
                    return true;
                }
            }
            catch (Exception ex)
            {
                bRet = false;
                string sErrMsg = String.Format("Failed to remove temp file directory. Exception: {0}", ex.Message);
                throw (new IOException(sErrMsg));
            }
            return bRet;
        }

        private static void RemoveFilesInDirectory(string pPath)
        {
            try
            {
                // Loop to explore all the directories under pPath
                foreach (string dirName in Directory.GetDirectories(pPath))
                {
                    // Access the file system to get the directory size
                    DirectoryInfo dirInfo = new DirectoryInfo(dirName);

                    RemoveFilesInSubDirectory(dirInfo);

                    foreach (string fileName in Directory.GetFiles(dirName))
                    {
                        try
                        {
                            File.Delete(fileName);

                        }
                        // Catch any exception if a file cannot be accessed
                        // e.g. due to security restriction
                        catch (Exception) { }
                    }

                }
            }
            // Catch any exception if a folder cannot be accessed
            // e.g. due to security restriction
            catch (Exception)
            {

            }
        }

        // A recursive method to remove files in subdirectory 
        private static bool RemoveFilesInSubDirectory(DirectoryInfo pDirInfo)
        {
            bool bRet = true;

            FileInfo[] fileInfos = pDirInfo.GetFiles();
            foreach (FileInfo fileInfo in fileInfos)
            {

                try
                {
                    fileInfo.Delete();
                }

                // Catch any exception if a file cannot be accessed
                // e.g. due to security restriction
                catch (Exception)
                {
                    bRet = false;
                }
            }

            DirectoryInfo[] dirInfos = pDirInfo.GetDirectories();
            foreach (DirectoryInfo dirInfo in dirInfos)
            {
                try
                {
                    RemoveFilesInSubDirectory(dirInfo);
                }
                // Catch any exception if a folder cannot be accessed
                // e.g. due to security restriction
                catch (Exception)
                {
                    bRet = false;
                }
            }
            return bRet;
        }

        public static string RemoveIllegalCharsInDirectory(string directory)
        {
            if (string.IsNullOrEmpty(directory))
                return null;

            int index = 0;
            string temp = directory;

            index = temp.LastIndexOf('\"');
            if (0 <= index)
                temp = temp.Substring(0, index);

            index = temp.IndexOf('\"');
            if (0 <= index)
                temp = temp.Remove(index, 1);

            index = temp.IndexOf('\\');
            if (0 == index)
                temp = temp.Remove(index, 1);

            index = temp.LastIndexOf('\\');
            if (index == temp.Length - 1)
                temp = temp.Substring(0, index);

            return temp;
        }

        public static List<string> GetUSBDrives()
        {
            List<string> lstUSBDrives = new List<string>();

            foreach (DriveInfo driveInfo in DriveInfo.GetDrives())
            {
                if (driveInfo.DriveType == DriveType.Removable && driveInfo.IsReady == true)
                {
                    lstUSBDrives.Add(driveInfo.Name);
                }
            }
            return lstUSBDrives;
        }

        public static void DisableCamera(string userName, bool bDisable)
        {
            if (string.IsNullOrEmpty(userName))
                return;

            // Enable/Disable camera
            IntPtr handle = FindWindow("TApplication", "RoboCam");
            if (handle != null)
            {
                int nMessage = bDisable ? ROBO_REMOTE_DISABLE_CAM : ROBO_REMOTE_ENABLE_CAM;

                IntPtr mem = Marshal.StringToHGlobalUni(userName);
                SendMessage(handle, nMessage, (IntPtr)0, mem);
                Marshal.FreeHGlobal(mem);
            }
        }

        public static void DeleteVideoLogFiles(string userName, bool bEnable)
        {
            if (string.IsNullOrEmpty(userName) || bEnable == false)
                return;

            string sSTIPath = Utilities.GetRoboSepSTIpath();

            
            // video logs files path
            string RoboVideoLogSubDirectory = "RoboSep\\logs\\videolog";

            // Delete video log files
            string sVideoLogPath = sSTIPath + RoboVideoLogSubDirectory;
            if (Directory.Exists(sVideoLogPath))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(sVideoLogPath);
                if (dirInfo != null)
                {
                    FileInfo[] fileInfos = dirInfo.GetFiles();
                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        if (fileInfo == null)
                            continue;
                        try
                        {
                            fileInfo.Delete();
                        }
                        // Catch any exception if a file cannot be accessed
                        // e.g. due to security restriction
                        catch (Exception ex)
                        {
                            string sErrMsg = String.Format("Failed to delete video log files. Exception: {0}", ex.Message); 
                            throw (new IOException(sErrMsg));
                        }
                    }
                }
            }

            // Delete video error log
            string RoboVideoErrorLogSubDirectory = "RoboSep\\logs\\videoerrorlog";

            // Delete video error log files
            string sVideoErrorLogPath = sSTIPath + RoboVideoErrorLogSubDirectory;
            if (Directory.Exists(sVideoErrorLogPath))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(sVideoErrorLogPath);
                if (dirInfo != null)
                {
                    FileInfo[] fileInfos = dirInfo.GetFiles();
                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        if (fileInfo == null)
                            continue;
                        try
                        {
                            fileInfo.Delete();
                        }
                        // Catch any exception if a file cannot be accessed
                        // e.g. due to security restriction
                        catch (Exception ex)
                        {
                            string sErrMsg = String.Format("Failed to delete video error log files. Exception: {0}", ex.Message);
                            throw (new IOException(sErrMsg));
                        }
                    }
                }
            }
        }  

        public static Bitmap RotateImage(Image image, float angle)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            //create a new empty bitmap to hold rotated image
            Bitmap rotatedBmp = new Bitmap(image.Width, image.Height);
            rotatedBmp.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(rotatedBmp);

            //Put the rotation point in the center of the image
            PointF offset = new PointF((float)image.Width / 2, (float)image.Height / 2);
            g.TranslateTransform(offset.X, offset.Y);

            //rotate the image
            g.RotateTransform(angle);

            //move the image back
            g.TranslateTransform(-offset.X, -offset.Y);

            //draw passed in image onto graphics object
            g.DrawImage(image, new PointF(0, 0));

            g.Dispose();
            return rotatedBmp;
        }

        public static Rectangle DrawSortIndicator(Graphics g, Rectangle r, SortOrder order, Brush brush)
        {
            //  draw a triangle at the right edge of the rectangle
            const int triangleHeight = 20;
            const int triangleWidth = 20;
            const int midX = triangleWidth / 2;
            const int midY = (triangleHeight / 2) - 1;
            const int deltaX = midX - 2;
            const int deltaY = deltaX / 2;

            Point triangleLocation = new Point(r.Right - triangleWidth - 2, r.Top + (r.Height - triangleHeight) / 2);
            Point[] pts = new Point[] { triangleLocation, triangleLocation, triangleLocation };

            if (order == SortOrder.Ascending)
            {
                pts[0].Offset(midX - deltaX, midY + deltaY);
                pts[1].Offset(midX, midY - deltaY - 1);
                pts[2].Offset(midX + deltaX, midY + deltaY);
            }
            else if (order == SortOrder.Descending)
            {
                pts[0].Offset(midX - deltaX, midY - deltaY);
                pts[1].Offset(midX, midY + deltaY);
                pts[2].Offset(midX + deltaX, midY - deltaY);
            }
            else
            {
                // do nothing
                return r;
            }

            g.FillPolygon(brush, pts);
            r.Width = r.Width - triangleWidth;
            return r;
        }

        public static void DrawCancelledMarker(int ImageWidth, int ImageHeight, Graphics g)
        {
            if (ImageWidth <= 0 || ImageHeight <= 0 || g == null)
                return;

            Rectangle rc = new Rectangle();
            rc.Height = ImageHeight;
            rc.Width = ImageWidth;


            // Rectangle rc = this.ClientRectangle;
            int X = rc.Width / 2;
            int Y = rc.Height / 2;

            List<Point> pointList = new List<Point>();
            pointList.Add(new Point(X, Y)); // This is the center location

            int distance = rc.Height / 3;
            double cornerBetweenPoints = (2 * Math.PI) / (5 - 1);

            // Draw connections between the points
            g.SmoothingMode = SmoothingMode.AntiAlias;
            for (int i = 0; i < 4; i++)
            {
                // Location of the other points
                Point p = new Point((int)(X - (distance * Math.Cos((Math.PI / 4) + (cornerBetweenPoints * i)))),
                                    (int)(Y - (distance * Math.Sin((Math.PI / 4) + (cornerBetweenPoints * i)))));
                pointList.Add(p);
                var pen = new Pen(Brushes.Red);
                pen.Width = 3;
                g.DrawLine(pen, pointList[0], p); // Draw a connection from center to a point on the circumference
                pen.Dispose();

            }
        }

        public static void DrawPlusMarker(int ImageWidth, int ImageHeight, Graphics g)
        {
            if (ImageWidth <= 0 || ImageHeight <= 0 || g == null)
                return;

            Rectangle rc = new Rectangle();
            rc.Height = ImageHeight;
            rc.Width = ImageWidth;


            // Rectangle rc = this.ClientRectangle;
            int X = rc.Width / 2;
            int Y = rc.Height / 2;

            List<Point> pointList = new List<Point>();
            pointList.Add(new Point(X, Y)); // This is the center location

            int distance = rc.Height / 3;
            double cornerBetweenPoints = (2 * Math.PI) / (5 - 1);

            // Draw connections between the points
            g.SmoothingMode = SmoothingMode.AntiAlias;
            for (int i = 0; i < 4; i++)
            {
                // Location of the other points
                Point p = new Point((int)(X - (distance * Math.Cos((0) + (cornerBetweenPoints * i)))),
                                    (int)(Y - (distance * Math.Sin((0) + (cornerBetweenPoints * i)))));
                pointList.Add(p);
                var pen = new Pen(Brushes.Gold);
                pen.Width = 10;
                g.DrawLine(pen, pointList[0], p); // Draw a connection from center to a point on the circumference
                pen.Dispose();

            }
        }


        public static GraphicsPath GetRoundedRect(Rectangle rect, float diameter)
        {
            GraphicsPath path = new GraphicsPath();

            if (diameter > 0)
            {
                RectangleF arc = new RectangleF(rect.X, rect.Y, diameter, diameter);
                path.AddArc(arc, 180, 90);
                arc.X = rect.Right - diameter;
                path.AddArc(arc, 270, 90);
                arc.Y = rect.Bottom - diameter;
                path.AddArc(arc, 0, 90);
                arc.X = rect.Left;
                path.AddArc(arc, 90, 90);
                path.CloseFigure();
            }
            else
            {
                path.AddRectangle(rect);
            }
            return path;
        }

        // Calculate the graphics path that representing the figure in the bitmap excluding the transparent color which is the top left pixel.
        public static GraphicsPath CalculateControlGraphicsPath(Bitmap bitmap)
        {
            // Create GraphicsPath for our bitmap calculation
            GraphicsPath graphicsPath = new GraphicsPath();

            // Use the top left pixel as our transparent color
            Color colorTransparent = bitmap.GetPixel(0, 0);

            // This is to store the column value where an opaque pixel is first found.
            // This value will determine where we start scanning for trailing opaque pixels.
            int colOpaquePixel = 0;

            // Go through all rows (Y axis)
            for (int row = 0; row < bitmap.Height; row++)
            {
                // Reset value
                colOpaquePixel = 0;

                // Go through all columns (X axis)
                for (int col = 0; col < bitmap.Width; col++)
                {
                    // If this is an opaque pixel, mark it and search for anymore trailing behind
                    if (bitmap.GetPixel(col, row) != colorTransparent)
                    {
                        // Opaque pixel found, mark current position
                        colOpaquePixel = col;

                        // Create another variable to set the current pixel position
                        int colNext = col;

                        // Starting from current found opaque pixel, search for anymore opaque pixels 
                        // trailing behind, until a transparent pixel is found or minimum width is reached
                        for (colNext = colOpaquePixel; colNext < bitmap.Width; colNext++)
                            if (bitmap.GetPixel(colNext, row) == colorTransparent)
                                break;

                        // Form a rectangle for line of opaque pixels found and add it to our graphics path
                        graphicsPath.AddRectangle(new Rectangle(colOpaquePixel, row, colNext - colOpaquePixel, 1));

                        // No need to scan the line of opaque pixels just found
                        col = colNext;
                    }
                }
            }

            // Return calculated graphics path
            return graphicsPath;
        }

        public static bool UnorderedEqual<T>(ICollection<T> a, ICollection<T> b)
        {
            // 1
            // Require that the counts are equal
            if (a.Count != b.Count)
            {
                return false;
            }
            // 2
            // Initialize new Dictionary of the type
            Dictionary<T, int> d = new Dictionary<T, int>();

            // 3
            // Add each key's frequency from collection A to the Dictionary
            foreach (T item in a)
            {
                int c;
                if (d.TryGetValue(item, out c))
                {
                    d[item] = c + 1;
                }
                else
                {
                    d.Add(item, 1);
                }
            }
            // 4
            // Add each key's frequency from collection B to the Dictionary
            // Return early if we detect a mismatch
            foreach (T item in b)
            {
                int c;
                if (d.TryGetValue(item, out c))
                {
                    if (c == 0)
                    {
                        return false;
                    }
                    else
                    {
                        d[item] = c - 1;
                    }
                }
                else
                {
                    // Not in dictionary
                    return false;
                }
            }
            // 5
            // Verify that all frequencies are zero
            foreach (int v in d.Values)
            {
                if (v != 0)
                {
                    return false;
                }
            }
            // 6
            // We know the collections are equal
            return true;
        }


        public static void DisposeShapeContainer(Microsoft.VisualBasic.PowerPacks.ShapeContainer AShapeContainer)
        {
            if (AShapeContainer != null)
            {
                if (AShapeContainer.Shapes != null && AShapeContainer.Shapes.Owner != null && AShapeContainer.Shapes.Count > 0)
                {
                    List<Microsoft.VisualBasic.PowerPacks.Shape> tshapes = new List<Microsoft.VisualBasic.PowerPacks.Shape>();
                    foreach (Microsoft.VisualBasic.PowerPacks.Shape tshape in AShapeContainer.Shapes)
                    {
                        tshapes.Add(tshape);
                    }
                    AShapeContainer.Shapes.Clear();
                    AShapeContainer.Shapes.Dispose();

                    foreach (Microsoft.VisualBasic.PowerPacks.Shape tshape in tshapes)
                    {
                        tshape.Dispose();
                    }
                }
                AShapeContainer.Dispose();
            }
        }

        public static String GetCurrentTimeStamp()
        {
            DateTime value = DateTime.Now;
            return value.ToString("yyMMdd_HHmmss");
        }

        public static void SendCtrlEscKeys()
        {
#if false
            //press the control key
            keybd_event(VK_CONTROL, 0x45, 0, (UIntPtr)0);

            //press the escape key
            keybd_event(VK_ESCAPE, 0x45, 0, (UIntPtr)0);

            //release the escape key
            keybd_event(VK_ESCAPE, 0x45, KEYEVENTF_KEYUP, (UIntPtr)0);

            //release the control key
            keybd_event(VK_CONTROL, 0x45, KEYEVENTF_KEYUP, (UIntPtr)0);
#endif
        }

        public static string GetUserName(string sName, char charToBeReplaced, char charForReplacement)
        {
            if (string.IsNullOrEmpty(sName) || charToBeReplaced == null || charForReplacement == null)
                return String.Empty;

            string sDisplayName = sName;
            int nIndex = 0;
            nIndex = sDisplayName.IndexOf(charForReplacement, nIndex);
            while (0 < nIndex)
            {
                sDisplayName = sDisplayName.Replace(charForReplacement, charToBeReplaced);
                nIndex = sDisplayName.IndexOf(charForReplacement, ++nIndex);
            }
            return sDisplayName;
        }

        public static string TruncatedString(Font referenceFont, string s, int width, int offset, Graphics g)
        {
            if (referenceFont == null || g == null || string.IsNullOrEmpty(s))
                return null;

            string sTemp, sResult = s;
            int sWidth;
            int i;
            SizeF strSize;

            try
            {
                strSize = g.MeasureString(s, referenceFont);
                sWidth = ((int)strSize.Width);

                if (width < sWidth)
                {
                    i = s.Length;
                    sTemp = "..." + s;

                    for (i = s.Length; i > 0 && sWidth > width - offset; i--)
                    {
                        strSize = g.MeasureString(sTemp.Substring(0, i), referenceFont);
                        sWidth = ((int)strSize.Width);
                    }

                    if (i < s.Length)
                    {
                        if (i - 3 <= 0)
                            sResult = s.Substring(0, 1) + "...";
                        else
                            sResult = s.Substring(0, i - 3) + "...";
                    }
                    else
                        sResult = s.Substring(0, i);
                }
            }
            catch
            {
            }

            return sResult;
        }

        public static string TruncatedString(string s, Rectangle rcText, Font referenceFont, TextFormatFlags flags)
        {
            if (referenceFont == null || string.IsNullOrEmpty(s))
                return null;

            string sTemp, sResult = s;
            int sWidth;
            int i;

            Size txtSize = new Size(rcText.Width, rcText.Height);
            try
            {
                txtSize = TextRenderer.MeasureText(sResult, referenceFont, txtSize, flags);
                sWidth = ((int)txtSize.Width);

                if (rcText.Width < sWidth)
                {
                    i = s.Length;
                    sTemp = "..." + s;

                    for (i = s.Length; i > 0 && sWidth > rcText.Width; i--)
                    {
                        txtSize = TextRenderer.MeasureText(sTemp.Substring(0, i), referenceFont, txtSize, flags);
                        sWidth = ((int)txtSize.Width);
                    }

                    if (i < s.Length)
                    {
                        if (i - 3 <= 0)
                            sResult = s.Substring(0, 1) + "...";
                        else
                            sResult = s.Substring(0, i - 3) + "...";
                    }
                    else
                        sResult = s.Substring(0, i);
                }
            }
            catch
            {
            }

            return sResult;
        }

        public static string EncryptString(string Message, string Passphrase)
        {
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below
            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the encoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            // Step 4. Convert the input string to a byte[]
            byte[] DataToEncrypt = UTF8.GetBytes(Message);

            // Step 5. Attempt to encrypt the string
            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            // Step 6. Return the encrypted string as a base64 encoded string
            return Convert.ToBase64String(Results);
        }

        public static string DecryptString(string Message, string Passphrase)
        {
            byte[] Results = null;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the decoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            // Step 4. Convert the input string to a byte[]
            byte[] DataToDecrypt = Convert.FromBase64String(Message);

            // Step 5. Attempt to decrypt the string
            try
            {
                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
            }
            catch (CryptographicException ex)
            {
                Console.WriteLine("Exception: {0}", ex.Message);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }
            // Step 6. Return the decrypted string in UTF8 format
            string s = String.Empty;

            if (Results != null)
                s = UTF8.GetString(Results);

            return s;
        }


        public static int GetWindowLong(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 4)
                return (int)GetWindowLong32(hWnd, nIndex);
            else
                return (int)(long)GetWindowLongPtr64(hWnd, nIndex);
        }

        public static int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong)
        {
            if (IntPtr.Size == 4)
                return (int)SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
            else
                return (int)(long)SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
        }

        [DllImport("kernel32.dll")]
        public static extern bool Beep(int freq, int duration);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool MessageBeep(int type);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern IntPtr PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "PostMessage")]
        private static extern IntPtr PostMessage(IntPtr hwnd, long wMsg, long wParam, long lParam);

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        private const byte VK_ESCAPE = 0x1B; // escape key
        private const byte VK_CONTROL = 0x11; // control key
        private const int KEYEVENTF_KEYUP = 0x02;

        private const int WM_USER = 0x0400;
        static int ROBO_REMOTE_ENABLE_CAM = WM_USER + 14;   // Enable CAM program
        static int ROBO_REMOTE_DISABLE_CAM = WM_USER + 15; // Disable Cam Program

	}
}
