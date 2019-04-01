//----------------------------------------------------------------------------
// RoboSepButton
//
// Invetech Pty Ltd
// Victoria, 3149, Australia
// Phone:   (+61) 3 9211 7700
// Fax:     (+61) 3 9211 7701
//
// The copyright to the computer program(s) herein is the property of 
// Invetech Pty Ltd, Australia.
// The program(s) may be used and/or copied only with the written permission 
// of Invetech Pty Ltd or in accordance with the terms and conditions 
// stipulated in the agreement/contract under which the program(s)
// have been supplied.
// 
// Copyright © 2004. All Rights Reserved.
//
//----------------------------------------------------------------------------
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Data;
using System.Windows.Forms;
using System.Resources;
using System.Threading;

using Tesla.Common.ResourceManagement;
using Tesla.Common.DrawingUtilities;

namespace Tesla.OperatorConsoleControls
{
	/// <summary>
	/// UI button with settings appropriate for use on touch-screens.
	/// </summary>
    public class RoboSepButton : System.Windows.Forms.UserControl 
    {
        #region Types

        public enum ButtonRole
        {
			General,
            OK,
            Warning,
            Error,
			OK_BIG,
			Warning_LEFT,
			Warning_RIGHT,
            NUM_BUTTON_ROLES
        }

        private enum ButtonState
        {
            Up,
            Hit,
            NUM_BUTTON_STATES
        }

        #endregion Types

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        // Storage for button state images
        private static readonly string[,]   theButtonStateResourceIdTable;
        private static Image[,]             theButtonImages;
        private static bool                 theButtonImagesLoaded;
        private static ImageAttributes      theImageAttributes;
        private static readonly ColorMatrix theDisabledColorMatrix;

        #region Construction/destruction

        public RoboSepButton()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();        
 
            // Set appropriate control styles
            this.SetStyle(ControlStyles.DoubleBuffer    |
                ControlStyles.AllPaintingInWmPaint      |
                ControlStyles.UserPaint                 |
                ControlStyles.ResizeRedraw				|
                ControlStyles.StandardClick,
                true); 
            this.UpdateStyles();

            // Load the button state images 
            ResourceManager localResources = new ResourceManager(
                "Tesla.OperatorConsoleControls.OperatorConsoleControlsResources", this.GetType().Assembly);
            if (( ! theButtonImagesLoaded) && localResources != null)
            {
                for (ButtonRole role = ButtonRole.OK; role < ButtonRole.NUM_BUTTON_ROLES; 
                    ++role)
                {
                    for (ButtonState state = ButtonState.Up; state < ButtonState.NUM_BUTTON_STATES;
                        ++state)
                    {
						Bitmap buttonImage = (role == ButtonRole.General) ? null :                        
                            (Bitmap)localResources.GetObject(
                            theButtonStateResourceIdTable[(int)role, (int)state]);
						theButtonImages[(int)role, (int)state] = buttonImage;
                    }
                }
                theButtonImagesLoaded = true;
            }

            // Initialise the image attributes used to produce disabled ("greyed-out") 
            // images.
            theImageAttributes = new ImageAttributes();
            theImageAttributes.SetColorMatrix(theDisabledColorMatrix, 
                ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
        }

        static RoboSepButton()
        {
            theButtonStateResourceIdTable = 
                new string[(int)ButtonRole.NUM_BUTTON_ROLES, (int)ButtonState.NUM_BUTTON_STATES]
            {
                // "Up" image Id				"Hit" image Id
                // ---------------------------------------------------
				// ButtonRole.General
				{"",							""},
                // ButtonRole.OK
                {"IMG_BUTTON_GREEN_UP",         "IMG_BUTTON_GREEN_HIT"},
                // ButtonRole.Warning
                {"IMG_BUTTON_YELLOW_UP",        "IMG_BUTTON_YELLOW_HIT"},
                // ButtonRole.Error
				{"IMG_BUTTON_RED_UP",           "IMG_BUTTON_RED_HIT"},

				{"IMG_BUTTON_GREEN_BIG_UP",         "IMG_BUTTON_GREEN_BIG_HIT"},
				{"IMG_BUTTON_YELLOW_ARROW_LEFT_UP",        "IMG_BUTTON_YELLOW_ARROW_LEFT_HIT"},
				{"IMG_BUTTON_YELLOW_ARROW_RIGHT_UP",        "IMG_BUTTON_YELLOW_ARROW_RIGHT_HIT"},
            };

            theButtonImages = 
                new Image[(int)ButtonRole.NUM_BUTTON_ROLES, (int)ButtonState.NUM_BUTTON_STATES];

            // Initialize the "color" matrix (used for greying-out images - notice the 
            // value in the fourth row, fourth column, which controls the alpha blend).
            float[,] disableMatrix = new float[5,5]
            {
                {1.0f, 0.0f, 0.0f, 0.0f, 0.0f},
                {0.0f, 1.0f, 0.0f, 0.0f, 0.0f},
                {0.0f, 0.0f, 1.0f, 0.0f, 0.0f},
                {0.0f, 0.0f, 0.0f, 0.4f, 0.0f},
                {0.0f, 0.0f, 0.0f, 0.0f, 1.0f}
            };
            theDisabledColorMatrix = new ColorMatrix();
            for (int row = 0; row < 5; ++row)
            {
                for (int col = 0; col < 5; ++col)
                {
                    theDisabledColorMatrix[row, col] = disableMatrix[row, col];
                }
            }
        }

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#endregion Construction/destruction

        private ButtonRole  myRole;
        private ButtonState myState;

        public ButtonRole Role
        {
            set
            {
                myRole = value;
                Refresh();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown (e);
            if (theButtonImages[(int)myRole, (int)myState] != null)
            {
                myState = ButtonState.Hit;
                Refresh();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp (e);            
            if (theButtonImages[(int)myRole, (int)myState] != null)
            {
                myState = ButtonState.Up;
                Refresh();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Perform default painting behaviour
            base.OnPaint(e);

            // Create a new temporary Graphics area on which to compose the image.
            Image rawImage = theButtonImages[(int)myRole, (int)myState];
			int btnWidth, btnHeight;
			if (rawImage == null)
			{
				btnWidth = this.Width;
				btnHeight= this.Height;
			}
			else
			{
				btnWidth = rawImage.Width;
				btnHeight= rawImage.Height;
			}
            Bitmap btnImage = new Bitmap(btnWidth, btnHeight);           
            Graphics btnGraphics = Graphics.FromImage(btnImage);

            // Apply standard Graphics settings
            DrawingUtilities.ApplyStandardContext(e.Graphics);
            DrawingUtilities.ApplyStandardContext(btnGraphics);

            // Draw a solid background (the same size as the as a base.  We do this so 
            // that transparent images are always blended with the background from the 
            // same starting point (otherwise, features such as drop shadows will 
            // increasingly darken when the control is clicked multiple times and is hence 
            // redrawn).
            SolidBrush backgroundBrush = new SolidBrush(this.BackColor);
            btnGraphics.FillRectangle(backgroundBrush, 0, 0,
                btnImage.Width, btnImage.Height);

            // Draw the button image according to the button role & state
			if (rawImage != null)
			{
				btnGraphics.DrawImage(rawImage, 0, 0, rawImage.Width, rawImage.Height);
			}

            // Draw the button text
            SolidBrush textBrush = new SolidBrush(
                ColourScheme.GetColour(ColourSchemeItem.ButtonTextForeground));        

            if (this.Text != null && this.Text != string.Empty)
            {
				DrawingUtilities.ApplyStandardContext(e.Graphics);
				StringFormat stringFormat = DrawingUtilities.CentreCentreStringFormat;
				Font font = SeparatorResourceManager.GetFont(SeparatorResourceManager.FontId.MaintenanceListItem);
				
				if(rawImage!= null && rawImage.Width > 120)
					font = SeparatorResourceManager.GetFont(SeparatorResourceManager.FontId.Smaller);

                btnGraphics.DrawString(this.Text,
                    font, 
                    textBrush, 
                    this.ClientRectangle,
                    stringFormat);
            }
			textBrush.Dispose();   
      
            // Finally, draw the composed image as the button image (adjusting the 
            // show it "disabled" ("greyed-out") if necessary).
            if (this.Enabled)
            {
                e.Graphics.DrawImage(btnImage, 0, 0, btnImage.Width, btnImage.Height);
            }
            else    // "greyed-out"
            {
                // "Grey out" (disable) the button image by applying a semitransparent
                // effect.
                int iWidth = btnImage.Width;
                int iHeight = btnImage.Height;
                e.Graphics.DrawImage(
                    btnImage, 
                    new Rectangle(0, 0, iWidth, iHeight),   // Destination rectangle
                    0,                                  // Source rectangle X 
                    0,                                  // Source rectangle Y
                    iWidth,                             // Source rectangle width
                    iHeight,                            // Source rectangle height
                    GraphicsUnit.Pixel, 
                    theImageAttributes);
            }  
        }

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            // 
            // RoboSepButton
            // 
            this.Name = "RoboSepButton";
            this.Size = new System.Drawing.Size(120, 48);

        }
		#endregion
	}
}
