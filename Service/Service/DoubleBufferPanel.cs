using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Tesla.Service
{
    public class DoubleBufferPanel : System.Windows.Forms.Panel
    {
        public DoubleBufferPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

    }
}
