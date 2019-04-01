using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using System.Runtime.InteropServices;



namespace GUI_Controls
{
    public class BarcodeListView : DragScrollListView2
    {
        #region MEMBER DECLARATIONS

        private const int IMAGELIST_WIDTH = 40;
        private const int IMAGELIST_HEIGHT = 40;

        public int headerHeight = HEADER_HEIGHT;


        #endregion // DELEGATES AND EVENT DEFINITIONS


        #region LOCAL STATIC MEMBERS

        // Text strings used as Image keys for the expanded/Collapsed image in the 
        // left-most columnHeader:
        static string COLLAPSED_IMAGE_KEY = "CollapsedImage";
        static string EXPANDED_IMAGE_KEY = "ExpandedImageKey";
        static string EMPTY_IMAGE_KEY = "EmptyImageKey";

        static string QUADRANT_IMAGE_KEY = "QuadrantImageKey";
        static string MAGNETIC_PARTICLE_EMPTY_IMAGE_KEY = "MagneticParticlesEmptyImageKey";
        static string MAGNETIC_PARTICLE_IMAGE_KEY = "MagneticParticlesImageKey";
        static string COCKTAIL_EMPTY_IMAGE_KEY = "CockTailEmptyImageKey";
        static string COCKTAIL_IMAGE_KEY = "CockTailImageKey";
        static string ANTIBODY_EMPTY_IMAGE_KEY = "AntiBodyEmptyImageKey";
        static string ANTIBODY_IMAGE_KEY = "AntiBodyImageKey";
        static string SAMPLE_TUBE_EMPTY_IMAGE_KEY = "SampleTubeEmptyImageKey";
        static string SAMPLE_TUBE_IMAGE_KEY = "SampleTubeImageKey";


        // "Magic number" approximates the height of the List View Column Header:
 
        static int HEADER_HEIGHT = 40;

        #endregion // LOCAL STATIC MEMBERS


        #region CONSTRUCTOR

        public BarcodeListView() : base()
        {
            this.SetAutoSizeMode(AutoSizeMode.GrowOnly);

            this.MaximumSize = new System.Drawing.Size(640, 480);

            // The Imagelist is used to hold images for the expanded and contracted icons in the
            // Left-most columnheader:
            ImageList imglist = new ImageList();
            this.SmallImageList = imglist;
            this.LargeImageList = imglist;

            this.SmallImageList.ImageSize = new System.Drawing.Size(IMAGELIST_WIDTH, IMAGELIST_HEIGHT);

            this.SmallImageList.Images.Add(MAGNETIC_PARTICLE_EMPTY_IMAGE_KEY, Properties.Resources.RE_BTN01N_magnetic);
            this.SmallImageList.Images.Add(MAGNETIC_PARTICLE_IMAGE_KEY, Properties.Resources.RE_BTN01N_magnetic_1);
            this.SmallImageList.Images.Add(COCKTAIL_EMPTY_IMAGE_KEY, Properties.Resources.RE_BTN02N_cocktail);
            this.SmallImageList.Images.Add(COCKTAIL_IMAGE_KEY, Properties.Resources.RE_BTN02N_cocktail_1);
            this.SmallImageList.Images.Add(ANTIBODY_EMPTY_IMAGE_KEY, Properties.Resources.RE_BTN10N_antibody);
            this.SmallImageList.Images.Add(ANTIBODY_IMAGE_KEY, Properties.Resources.RE_BTN10N_antibody_1);
            this.SmallImageList.Images.Add(SAMPLE_TUBE_EMPTY_IMAGE_KEY, Properties.Resources.RE_BTN03N_sample_tube);
            this.SmallImageList.Images.Add(SAMPLE_TUBE_IMAGE_KEY, Properties.Resources.RE_BTN03N_sample_tube_1);

            Color textColour = Color.FromArgb(95, 96, 98);
            Font textFont = this.Font;
            StringFormat textFormat = new StringFormat();
            textFormat.Alignment = StringAlignment.Center;
            textFormat.LineAlignment = StringAlignment.Center;
            
            Bitmap bmQuadrantSelected = new Bitmap(Properties.Resources.QuadrantNumber_SELECTED);
            Bitmap bm1;
           
            for (int i = 0; i <= 3; i++)
            {
                int number = i + 1;

                bm1 = new Bitmap(bmQuadrantSelected);
                Graphics graphicsSelected = Graphics.FromImage(bm1);

                graphicsSelected.DrawString(number.ToString(), textFont, new SolidBrush(textColour),
                        new Rectangle(0, 0, IMAGELIST_WIDTH, IMAGELIST_HEIGHT), textFormat);

                string quadrant = QUADRANT_IMAGE_KEY;
                quadrant += i.ToString();
                this.SmallImageList.Images.Add(quadrant, new Bitmap(bm1));
            }


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

        #region CONTROL EXPANSION/COLLAPSE

        private int PreferredControlHeight()
        {
            int output = HEADER_HEIGHT;
            int rowHeight = 0;

            // determine the height of an individual list item:
            if(this.Items.Count > 0)
                rowHeight = this.Items[0].Bounds.Height;

            // Increase the height of the control to accomodate the Columnheader, all of the current items, 
            // and the value of the horizontal scroll bar (if present):
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

        public bool HasBarcodeVials()
        {
            bool bHasBarcodeVials = false;
            for (int i = 0; i < Items.Count; i++)
            {
                ListViewItem lvItem = Items[i];
                //if (lvItem.ImageKey == 


            }


            return bHasBarcodeVials;
        }


        #endregion CONTROL EXPANSION/COLLAPSE
    }

}
