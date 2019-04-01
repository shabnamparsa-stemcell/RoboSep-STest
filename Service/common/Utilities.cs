using System;

using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;

namespace Tesla.Common
{
	/// <summary>
	/// Summary description for Utilities.
	/// </summary>
	public class Utilities
	{
		public Utilities()
		{
		}

		[DllImport("kernel32.dll")]
		public static extern bool  Beep(int freq,int duration);

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
	}
}
