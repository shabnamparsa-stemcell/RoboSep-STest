using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Layout;


namespace GUI_Controls
{
    public partial class ConfigListView : DragScrollListView2
    {
        private const int IMAGELIST_WIDTH = 40;
        private const int IMAGELIST_HEIGHT = 40;
        private static int HEADER_HEIGHT = 40;
        public int headerHeight = HEADER_HEIGHT;


        #region CONSTRUCTOR

        public ConfigListView() : base()
        {
            this.SetAutoSizeMode(AutoSizeMode.GrowOnly);

            this.MaximumSize = new System.Drawing.Size(640, 480);

            // The Imagelist is used to hold images for the expanded and contracted icons in the
            // Left-most columnheader:
            ImageList imglist = new ImageList();
            this.SmallImageList = imglist;
            this.LargeImageList = imglist;

            this.SmallImageList.ImageSize = new System.Drawing.Size(IMAGELIST_WIDTH, IMAGELIST_HEIGHT);

            Color textColour = Color.FromArgb(95, 96, 98);
            Font textFont = this.Font;
            StringFormat textFormat = new StringFormat();
            textFormat.Alignment = StringAlignment.Center;
            textFormat.LineAlignment = StringAlignment.Center;


            // Default configuration 
            this.View = System.Windows.Forms.View.Details;
            this.FullRowSelect = false;
            this.GridLines = true;
            this.Margin = new Padding(0);
            this.OwnerDraw = false;

            // The stateImageList is used as a hack method to allow larger Row Heights:
            this.StateImageList = new ImageList();
            this.StateImageList.ImageSize = new System.Drawing.Size(IMAGELIST_WIDTH, IMAGELIST_HEIGHT);
       }

        #endregion // CONSTRUCTOR
  
        public int RowHeight
        {
            get
            {
                int nRowHeight = this.StateImageList.ImageSize.Height;
                if (this.Items.Count > 0)
                {
                    nRowHeight = this.Items[0].Bounds.Height;
                }
                return nRowHeight;
            }
            set
            {
                this.StateImageList.ImageSize = new System.Drawing.Size(value, value);
            }
        }

        private int PreferredControlHeight()
        {
            int output = HEADER_HEIGHT;
            int rowHeight = 0;

            // determine the height of an individual list item:
            if(this.Items.Count > 0)
                rowHeight = this.Items[0].Bounds.Height;

            // Increase the height of the control to accomodate all the items and the header 
            output = HEADER_HEIGHT + (this.Items.Count) * rowHeight + this.Groups.Count * HEADER_HEIGHT;

            return output;
        }

        public void SetControlHeight()
        {
            if (this.Items.Count != 0)
            {
                int headerHeight = HEADER_HEIGHT;
                if (this.HeaderStyle == ColumnHeaderStyle.None)
                {
                    headerHeight = 0;
                }
                int nHeight = headerHeight + RowHeight * this.Items.Count;

                Rectangle rc = this.Bounds;
                this.SetBounds(rc.X, rc.Y, rc.Width, nHeight);
            }
        }
    }
}
