using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Tesla.Common;



namespace GUI_Controls
{
    public partial class DragScrollListView : ListView
    {
        // declare scroll constants:
        private const int WM_HSCROLL = 276; // Horizontal scroll
        private const int WM_VSCROLL = 277; // Vertical scroll
        private const int SB_LINEUP = 0; // Scrolls one line up
        private const int SB_LINELEFT = 0;// Scrolls one cell left
        private const int SB_LINEDOWN = 1; // Scrolls one line down
        private const int SB_LINERIGHT = 1;// Scrolls one cell right
        private const int SB_PAGEUP = 2; // Scrolls one page up
        private const int SB_PAGELEFT = 2;// Scrolls one page left
        private const int SB_PAGEDOWN = 3; // Scrolls one page down
        private const int SB_PAGERIGTH = 3; // Scrolls one page right
        private const int SB_PAGETOP = 6; // Scrolls to the upper left
        private const int SB_LEFT = 6; // Scrolls to the left
        private const int SB_PAGEBOTTOM = 7; // Scrolls to the upper right
        private const int SB_RIGHT = 7; // Scrolls to the right
        private const int SB_ENDSCROLL = 8; // Ends scroll


        private const int GWL_STYLE = -16;
        private const int WS_VSCROLL = 0x00200000;
        private const int WS_HSCROLL = 0x00100000;

        // sends navigation commands
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

       [DllImport("user32.dll", EntryPoint = "SendMessage")]
       private static extern IntPtr SendMessage(IntPtr hwnd, long wMsg, long wParam, long lParam);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(HandleRef hwnd, out RECT lpRect);

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        // ListView messages
        private const int LVM_FIRST = 0x1000;
        const long LVM_GETHEADER = (LVM_FIRST + 31);


        // define constants
        public int TIMER_CUT_OFF = 200;
        public int VERTICAL_SCROLL_CUTOFF = 6;
        public double SCROLL_MOMENTUM = 0.75;
        public double VERTICAL_SPEED = 1.0;
        public int LINE_SIZE = 10;
        public int INTERVAL = 30;
        public int NUM_VISIBLE_ELEMENTS = 7;
        public int VERTICAL_PAGE_SIZE = 4;

        // define variables
        private Point mouseHoldRecent;
        private Point[] mouseDownPoints;
        private DateTime[] mouseDownTimes;
        private int TopItemIndex;
  

        // speed variables
        private double speed = 0.0;
        private double speed1 = 0.0;
        private double speed2 = 0.0;
        private double mouseScrollTime;
        private double mouseScrollVertical;
        private bool DirectionDown;
        private volatile bool inPageScrollTimer = false;
        private bool startPageScrolling = false;

        //timers
        private Timer mouseSpeedTimer = new Timer();
        private Timer scrollTimer = new Timer();
        private Timer pageScrollTimer = new Timer();

        public DragScrollListView()
        {
            InitializeComponent();
            // set scroll bars off
            //this.Scrollable = false;
            this.View = View.List;

            mouseDownPoints = new Point[4];
            mouseDownTimes = new DateTime[4];

            // add mouse down event
            this.MouseClick += new MouseEventHandler(MouseClickEvent);
            mouseSpeedTimer.Tick += new EventHandler(SpeedTimerTick);
            mouseSpeedTimer.Interval = INTERVAL;
            scrollTimer.Tick += new EventHandler(ScrollTimerTick);
            scrollTimer.Interval = 200;
            this.Scrollable = true;

            NUM_VISIBLE_ELEMENTS = this.VisibleRow;

            pageScrollTimer.Tick += new EventHandler(PageScrollTimerTick);
            pageScrollTimer.Interval = 1000;

            // add single click and double click event
            this.Click += new EventHandler(SingleClick);
            //this.DoubleClick += new EventHandler(DoubleClick);
        }

        public int VisibleRow
        {
            get { return NUM_VISIBLE_ELEMENTS; }
            set { NUM_VISIBLE_ELEMENTS = value; }
        }

        private void MouseClickEvent(object sender, MouseEventArgs e)
        {
            mouseSpeedTimer.Stop();
            scrollTimer.Stop();
        }

        private void SpeedTimerTick(object sender, EventArgs e)
        {
            // update points (push older points to later in the array)
            if (mouseDownPoints[1] != new Point(0, 0)
            && mouseDownPoints[2] != new Point(0, 0)
            && mouseDownPoints[3] != new Point(0, 0))
            {
                for (int i = 0; i < 3; i++)
                {
                    if (mouseDownPoints[i + 1] != new Point(0, 0))
                    {
                        mouseDownPoints[i] = mouseDownPoints[i + 1];
                        mouseDownTimes[i] = mouseDownTimes[i + 1];
                    }
                }
                mouseDownPoints[3] = MousePosition;
                mouseDownTimes[3] = DateTime.Now;

                EstimateScrollSpeed();

                // if vertical travel is less than "VERTICAL SCROLL CUTOFF", turn off mouse speed timer
                if (Math.Abs(mouseScrollVertical) < VERTICAL_SCROLL_CUTOFF)
                    mouseSpeedTimer.Stop();
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    if (mouseDownPoints[i] == new Point(0, 0))
                    {
                        mouseDownPoints[i] = MousePosition;
                        mouseDownTimes[i] = DateTime.Now;
                        break;
                    }
                }
            }
         }


        public void StartScrolling()
        {
            if (Items.Count == 0 || this.TopItem == null)
                return;
            
            speed1 = 0.0;
            speed2 = 0.0;

            mouseHoldRecent = MousePosition;

            // clear point and time arrays
            for (int i = 0; i < 4; i++)
            {
                mouseDownPoints[i] = new Point(0, 0);
                mouseDownTimes[i] = new DateTime();
            }

            mouseDownPoints[0] = MousePosition;
            mouseDownTimes[0] = DateTime.Now;

            // start timer to get additional points
            mouseSpeedTimer.Start();
            scrollTimer.Start();

            TopItemIndex = this.TopItem.Index;
        }


        public void StartScrollingUp()
        {
            DirectionDown = false;
            pageScrollTimer.Start();
        }

        public void StopScrollingUp()
        {
            pageScrollTimer.Stop();
            mouseSpeedTimer.Stop();
            scrollTimer.Stop();
            startPageScrolling = false;
        }

        public void StartScrollingDown()
        {
            DirectionDown = true;
            pageScrollTimer.Start();
        }

        public void StopScrollingDown()
        {
            pageScrollTimer.Stop();
            mouseSpeedTimer.Stop();
            scrollTimer.Stop();

            startPageScrolling = false;
        }

        private void EstimateScrollSpeed()
        {
            if (mouseDownTimes[1] != mouseDownTimes[0]
                && mouseDownTimes[2] != mouseDownTimes[0]
                && mouseDownTimes[3] != mouseDownTimes[0])
            {
                // Case 1: estimate speed based on point1 and point3
                mouseScrollTime = mouseDownTimes[3].Subtract(mouseDownTimes[1]).TotalSeconds;
                mouseScrollVertical = mouseDownPoints[3].Y - mouseDownPoints[1].Y;

                if (mouseScrollTime != 0 && mouseScrollVertical != 0)
                    speed1 = mouseScrollVertical / mouseScrollTime;

                // Case 2: estimate speed based on mousedown point
                mouseScrollTime = mouseDownTimes[3].Subtract(mouseDownTimes[0]).TotalSeconds;
                mouseScrollVertical = mouseDownPoints[3].Y - mouseDownPoints[0].Y;

                if (mouseScrollTime != 0 && mouseScrollVertical != 0)
                    speed2 = mouseScrollVertical / mouseScrollTime;

                // Pick the largest of speed 1 and 2
                speed = Math.Abs(speed1) > Math.Abs(speed2) ? speed1 : speed2;
                // modify speed based on size of list
                // speed is greater when list is larger
                // speed is lower when list is shorter
                double SizeModifier = (double)this.Items.Count / 40.00;
                SizeModifier = SizeModifier > 1 ? (SizeModifier / 7.5) + 1 : ((SizeModifier + 1.0) / 2.0);
                SizeModifier = Math.Round(SizeModifier, 2);

                DirectionDown = speed > 0 ? true : false;

                if (speed != 0)
                {
                    // based on speed, set scroll timer interval
                    double interval = Math.Abs(7000 / (speed * SizeModifier * VERTICAL_SPEED));
                    scrollTimer.Interval = (int)interval > 225 ? 225 : (int)interval < 5 ? 5 : (int)interval;
                }
            }
        }

        private void ScrollTimerTick(object sender, EventArgs e)
        {
            // Case 1: mouse speed timer is on, move direction
            // indicated and modify timer
            if (!mouseSpeedTimer.Enabled)
            {
                // stop timer and movement if interval is > 800 ms
                if (scrollTimer.Interval > TIMER_CUT_OFF)
                {
                    scrollTimer.Stop();
                }
                else
                {
                    double interval = (double)scrollTimer.Interval;
                    interval = interval * (1 / SCROLL_MOMENTUM);
                    scrollTimer.Interval = (int)interval;
                }

                // scroll
                if (DirectionDown)
                {
                    LineDown();
                }
                else
                {
                    LineUp();
                }
            }

            // Case 2: Mouse speed timer is off, base position
            // on mouseHoldRecent point position

            else
            {
                int numLines = 0;
                Point currentPosition = MousePosition;
                int VertDisplacement = currentPosition.Y - mouseHoldRecent.Y;

                if (Math.Abs(VertDisplacement) > LINE_SIZE)
                {
                    double temp = (double)VertDisplacement / LINE_SIZE * 1.000;
                    numLines = (int)Math.Round(temp);
                }

                if (DirectionDown)
                {
                    mouseHoldRecent = MousePosition;

                    for (int i = 0; i < numLines; i++)
                        LineDown();
                }
                else
                {
                    mouseHoldRecent = MousePosition;
                    for (int i = 0; i < numLines; i++)
                        LineUp();
                }
            }
        }

        private void PageScrollTimerTick(object sender, EventArgs e)
        {
             if (inPageScrollTimer)
                return;

            inPageScrollTimer = true;
            for (int i = 0; i < VERTICAL_PAGE_SIZE; i++)
            {
                if (DirectionDown)
                {
                    LineDown();
                }
                else
                {
                    LineUp();
                }
            }

            startPageScrolling = true;
            inPageScrollTimer = false;
        }

        public void LineDown()
        {
            if (Items.Count == 0 || this.TopItem == null)
            {
                return;
            }

            if (startPageScrolling && !inPageScrollTimer)
            {
                return;
            }

            if (this.TopItem.Index <= (this.Items.Count - NUM_VISIBLE_ELEMENTS + 1))
            {
                TopItemIndex += 1;
                if (TopItemIndex > (this.Items.Count - NUM_VISIBLE_ELEMENTS + 1))
                    TopItemIndex = this.Items.Count - NUM_VISIBLE_ELEMENTS + 1;

                int FocusItemNum = TopItemIndex + (NUM_VISIBLE_ELEMENTS / 2);
                if (FocusItemNum > 0 && FocusItemNum < this.Items.Count)
                {
                    FocusedItem = Items[FocusItemNum];
                }

                SendMessage(this.Handle, WM_VSCROLL, (IntPtr)SB_LINEDOWN, IntPtr.Zero);
            }
        }

        public void LineUp()
        {
            if (Items.Count == 0 || this.TopItem == null)
                return;

            if (startPageScrolling && !inPageScrollTimer)
                return;

            if (this.TopItem.Index >= 0)
            {
                TopItemIndex -= 1;
                if (TopItemIndex < 0)
                    TopItemIndex = 0;

                int FocusItemNum = TopItemIndex + (NUM_VISIBLE_ELEMENTS / 2);
                if (FocusItemNum >= Items.Count)
                    return;

                if ((0 <= FocusItemNum)  && (FocusItemNum < this.Items.Count))
                {
                    FocusedItem = Items[FocusItemNum];
                }

                SendMessage(this.Handle, WM_VSCROLL, (IntPtr)SB_LINEUP, IntPtr.Zero);
            }
        }

        public void PageUp()
        {
            for (int i = 0; i < VERTICAL_PAGE_SIZE; i++)
            {
                LineUp();
            }
        }

        public void PageDown()
        {
            for (int i = 0; i < VERTICAL_PAGE_SIZE; i++)
            {
                LineDown();
            }
        }
 
        private void SingleClick(object sender, EventArgs e)
        {

        }

        private new void DoubleClick(object sender, EventArgs e)
        {

        }

        public void ResizeColumnWidth(ColumnHeader header, StringFormat theFormat, int minColumnWidth)
        {
            int minWidth = minColumnWidth;
            PointF ulCorner = new PointF(0.0f, 0.0f);
            int temp = 0;
            for (int i = 0; i < this.Items.Count; i++ )
            {
                ListViewItem lvItem = Items[i];
                Bitmap bm = new Bitmap(32, 32);
                Graphics g = Graphics.FromImage(bm);
                SizeF sizeF = g.MeasureString(lvItem.Text, this.Font, ulCorner, theFormat);
                temp = (int)Math.Ceiling((double)sizeF.Width);

                if (minWidth < temp)
                    minWidth = temp;

                bm.Dispose();

            }

            // set column header width
            if (header.Width != minWidth)
                header.Width = minWidth;
        }

        public void SizeLastColumn()
        {
            int nClientWidth = ClientRectangle.Width;
            int nColWidth = 0;
            int nLastColWidth = 0;
            for (int i = 0; i < Columns.Count; i++)
            {
                nColWidth += Columns[i].Width;

                // last column cannnot be resized since the sum of the column widths before the last 
                // column already has exceeded the client width
                if (nColWidth > nClientWidth && i < (Columns.Count - 1))
                    break;

                if ((i == Columns.Count - 1))
                {
                    int nLastColExtraWidth = nClientWidth - nColWidth;
                    Columns[i].Width += nLastColExtraWidth;
                    nLastColWidth = Columns[i].Width;
                }
            }
            this.SuspendLayout();

            Columns[Columns.Count - 1].Width = -2;
            this.Refresh();
            if (Columns[Columns.Count - 1].Width < nLastColWidth)
            {
                Columns[Columns.Count - 1].Width = nLastColWidth;
            }
            this.ResumeLayout(true);
        }

        public void ResizeVerticalHeight(bool addMargins)
        {
            int headerHeight = 0;
            if (HeaderStyle != ColumnHeaderStyle.None)
            {
                RECT rch = new RECT();

                IntPtr hwnd = SendMessage(this.Handle, LVM_GETHEADER, 0, 0);
                 if (hwnd != null)
                {
                    if (GetWindowRect(new HandleRef(null, hwnd), out rch))
                    {
                        headerHeight = rch.Bottom - rch.Top;
                    }
                }
            }
            if (this.Items.Count > 0 )
            {
                int nMargins = addMargins ? 2 : 0;
                Rectangle rcItem = GetItemRect(0);
                Rectangle t = ClientRectangle;
                this.ClientSize = new Size(t.Width, VisibleRow * rcItem.Height + headerHeight + nMargins);
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x83: // WM_NCCALCSIZE
                    int style = (int)Utilities.GetWindowLong(this.Handle, GWL_STYLE);
                    if ((style & WS_VSCROLL) == WS_VSCROLL)
                    {
                        style &= ~WS_VSCROLL;
                        Utilities.SetWindowLong(this.Handle, GWL_STYLE, style);
                    }
                    if ((style & WS_HSCROLL) == WS_HSCROLL)
                    {
                        style &= ~WS_HSCROLL;
                        Utilities.SetWindowLong(this.Handle, GWL_STYLE, style);
                    }
                    base.WndProc(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }
}