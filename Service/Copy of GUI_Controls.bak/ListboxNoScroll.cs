﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GUI_Controls
{
    //http://stackoverflow.com/questions/13169900/hide-vertical-scroll-bar-in-listbox-control
    public class ListboxNoScroll : System.Windows.Forms.ListBox
    {
        private bool mShowScroll;
        protected override System.Windows.Forms.CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                if (!mShowScroll)
                    cp.Style = cp.Style & ~0x200000;
                return cp;
            }
        }
        public bool ShowScrollbar
        {
            get { return mShowScroll; }
            set
            {
                if (value == mShowScroll)
                    return;
                mShowScroll = value;
                if (Handle != IntPtr.Zero)
                    RecreateHandle();
            }
        }
    }
}
