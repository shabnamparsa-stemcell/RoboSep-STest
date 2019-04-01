//
// 2011-09-05 to 2011-09-22 sp various changes
//     - provide support for different volume error checking dialog  
//     - add reporting of incubation and separation status
//     - add checking for recommended volume levels and provide warnings
//     - add checking of absolute volume levels  
//
//----------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Tesla.ProtocolEditorModel;
using Tesla.DataAccess;
using Tesla.Common.ResourceManagement;

namespace Tesla.ProtocolEditor
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class ProtocolVolWarning : System.Windows.Forms.Form
	{
        private System.Windows.Forms.Button btnExitClick;

        // 2011-09-12 sp
        // added report for incubation and separation status
        private double TotalIncubationTime, TotalSeparationTime, TotalExtensionTime;
        private int NumberOfIncubations, NumberOfSeparations;

		private int SampleMaxVol, SampleMinVol;
        private int[] RelCock, RelPart, RelAnti;
        // 2010-09-09 sp added absolute volume reporting
        private int[] AbsCock, AbsPart, AbsAnti;
        private double TotalMaxLimit, TotalMinLimit;
		private double[] MaxReagCock, MaxReagPart, MaxReagAnti;
		private double[] MinReagCock, MinReagPart, MinReagAnti;
        private double[] MinWorkingSample;
        private double[] MaxWorkingSample;
        private double[] MinWorkingSeparation;
        private double[] MaxWorkingSeparation;
        private double[] SumMax, SumMin;
        //private int[] defaultEndResult;
        private bool endResultSelected;

        private int[] MaxReagCockFlags;
        private int[] MinReagCockFlags;
        private int[] MaxReagPartFlags;
        private int[] MinReagPartFlags;
        private int[] MaxReagAntiFlags;
        private int[] MinReagAntiFlags;
        private int[] MinWorkingSampleFlags;
        private int[] MinWorkingSeparationFlags;
        private int[] MaxWorkingSampleFlags;
        private int[] MaxWorkingSeparationFlags;
        private Color warningTextColor = Color.DarkRed;
        private Color errorTextColor = Color.Red;
        private Color warningBackColor = Color.LightYellow;
        private Color errorBackColor = Color.LightYellow;
        private Color defaultTextColor = Color.Black;

		private System.Windows.Forms.ListView MsgListView;
		private System.Windows.Forms.ColumnHeader Param;
		
		private int CocktailProps;		
		private int ParticleProps;
		private int AntibodyProps;
		private int nItems;
		private System.Windows.Forms.ColumnHeader Q1;
		private System.Windows.Forms.ColumnHeader Q2;
		private System.Windows.Forms.ColumnHeader Q3;
        private System.Windows.Forms.ColumnHeader Q4;
//        private ListBox MsgBox;
        private RichTextBox rTextBox;
        private CheckBox cbAcceptAsIs;
        public bool acceptProtocolOverride = false;

        private const string NOT_SPECIFIED_TEXT = @"   END RESULT NOT SPECIFIED";

        // 2011-11-03 sp -- added to use specified quadrants instead of auto-incemented value
        public bool SetEndResultSelected
        {
            set
            {
                endResultSelected = value;
            }
        }

        // 2011-11-03 sp -- added to use specified quadrants instead of auto-incemented value
        /*
        public int[] SetDefaultEndResult
        {
            set
            {
                defaultEndResult = value;
            }
        }
        */

        // 2011-11-03 sp -- added to use specified quadrants instead of auto-incemented value
        public int SetMaxQuadrantIndex
        {
            set
            {
                nItems = value + 1;
            }
        }
        
        public int NumOfCocktailProps
		{
			set
			{
				CocktailProps = value;

				if (CocktailProps > nItems)
					nItems = CocktailProps;
			}
		}
		
		public int NumOfParticleProps
		{
			set
			{
				ParticleProps = value;
				
				if (ParticleProps > nItems)
					nItems = ParticleProps;
			}
		
		}

		public int NumOfAntibodyProps
		{
			set
			{
				AntibodyProps = value;

				if (AntibodyProps > nItems)
					nItems = AntibodyProps;
			}		
		}


		public int SetSampleMaxVol
		{
			set
			{
				SampleMaxVol = value;
			}
		}

		public int SetSampleMinVol
		{
			set
			{
				SampleMinVol = value;
			}
		}

		public double SetTotalMaxLimit
		{
			set
			{
				TotalMaxLimit = value;
			}
		}

		public double SetTotalMinLimit
		{
			set
			{
				TotalMinLimit = value;
			}
		}

        public int[] SetRelCock
        {
            set
            {
                RelCock = value;
            }
        }

        public int[] SetRelPart
        {
            set
            {
                RelPart = value;
            }
        }

        public int[] SetRelAnti
        {
            set
            {
                RelAnti = value;
            }
        }

        // 2010-09-09 sp added absolute volume reporting
        public int[] SetAbsCock
        {
            set
            {
                AbsCock = value;
            }
        }

        // 2010-09-09 sp added absolute volume reporting
        public int[] SetAbsPart
        {
            set
            {
                AbsPart = value;
            }
        }

        // 2010-09-09 sp added absolute volume reporting
        public int[] SetAbsAnti
        {
            set
            {
                AbsAnti = value;
            }
        }

        public double[] SetSumMax
		{
			set
			{
				SumMax = value;
			}
		}

		public double[] SetSumMin
		{
			set
			{
				SumMin = value;
			}
		}
		public double[] SetMaxReagCock
		{
			set
			{
				MaxReagCock = value;
			}
		}
		public double[] SetMinReagCock
		{
			set
			{
				MinReagCock = value;
			}
		}
		public double[] SetMaxReagPart
		{
			set
			{
				MaxReagPart = value;
			}
		}
		public double[] SetMinReagPart
		{
			set
			{
				MinReagPart = value;
			}
		}
		public double[] SetMaxReagAnti
		{
			set
			{
				MaxReagAnti = value;
			}
		}
        public double[] SetMinReagAnti
        {
            set
            {
                MinReagAnti = value;
            }
        }

        public int[] SetMaxReagCockFlags
        {
            set
            {
                MaxReagCockFlags = value;
            }
        }
        public int[] SetMinReagCockFlags
        {
            set
            {
                MinReagCockFlags = value;
            }
        }
        public int[] SetMaxReagPartFlags
        {
            set
            {
                MaxReagPartFlags = value;
            }
        }
        public int[] SetMinReagPartFlags
        {
            set
            {
                MinReagPartFlags = value;
            }
        }
        public int[] SetMaxReagAntiFlags
        {
            set
            {
                MaxReagAntiFlags = value;
            }
        }
        public int[] SetMinReagAntiFlags
        {
            set
            {
                MinReagAntiFlags = value;
            }
        }
        public int[] SetMinWorkingSampleFlags
        {
            set
            {
                MinWorkingSampleFlags = value;
            }
        }
        public int[] SetMinWorkingSeparationFlags
        {
            set
            {
                MinWorkingSeparationFlags = value;
            }
        }
        public int[] SetMaxWorkingSampleFlags
        {
            set
            {
                MaxWorkingSampleFlags = value;
            }
        }
        public int[] SetMaxWorkingSeparationFlags
        {
            set
            {
                MaxWorkingSeparationFlags = value;
            }
        }


        public double[] SetMinWorkingSample
        {
            set
            {
                MinWorkingSample = value;
            }
        }

        public double[] SetMinWorkingSeparation
        {
            set
            {
                MinWorkingSeparation = value;
            }
        }


        public double[] SetMaxWorkingSample
        {
            set
            {
                MaxWorkingSample = value;
            }
        }

        public double[] SetMaxWorkingSeparation
        {
            set
            {
                MaxWorkingSeparation = value;
            }
        }
        
        
        // 2011-09-12 sp
        // support for incubation time
        public double SetTotalIncubationTime
        {
            set
            {
                TotalIncubationTime = value;
            }
        }
        // 2011-09-12 sp
        // support for separation time
        public double SetTotalSeparationTime
        {
            set
            {
                TotalSeparationTime = value;
            }
        }
        // 2011-09-12 sp
        // support for number of separations
        public int SetNumberOfSeparations
        {
            set
            {
                NumberOfSeparations = value;
            }
        }
        // 2011-09-12 sp
        // support for number of incubations
        public int SetNumberOfIncubations
        {
            set
            {
                NumberOfIncubations = value;
            }
        }

        // 2011-09-12 sp
        // support for separation time
        public double SetTotalExtensionTime
        {
            set
            {
                TotalExtensionTime = value;
            }
        }
        // 2011-09-12 sp
        // rename VolumeErrorColor and SetVolumeErrorColor to more generic display function
        public int SetCheckProtocolDlgContents
		{
			set
			{
				SetCheckProtocolDisplayContents(value);
			}
		}

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ProtocolVolWarning()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            // 2011-11-08 sp
            // resize window if the screen resolution is smaller than the specify dialog size
            Rectangle screenArea = System.Windows.Forms.Screen.PrimaryScreen.Bounds;      // get size of display
            //           MessageBox.Show("Screen area: " + screenArea.Width + " x " + screenArea.Height + " ; Panel size:" +
            //               this.Size.Width + " x " + this.Size.Height);
            // reset panel to fit within the screen height with some padding
            // No adjustments are made to the display width as horizontal scrolling is not provided
            int padding = 40;
            this.SetBounds(0, 0, this.Size.Width, Math.Min(screenArea.Height - padding, this.Size.Height));

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			RelCock		= new int[4];
			RelPart		= new int[4];
			RelAnti		= new int[4];
            // 2010-09-09 sp added absolute volume reporting
            AbsCock = new int[4];
            AbsPart = new int[4];
            AbsAnti = new int[4];
            MaxReagCock = new double[4];
			MaxReagPart = new double[4];
			MaxReagAnti = new double[4];			
			MinReagCock = new double[4];
			MinReagPart = new double[4];
			MinReagAnti = new double[4];
            MinWorkingSample = new double[4];
            MaxWorkingSample = new double[4];
            MinWorkingSeparation = new double[4];
            MaxWorkingSeparation = new double[4];
			SumMax		= new double[4];
			SumMin      = new double[4];		

			CocktailProps = 0;		
			ParticleProps = 0;
			AntibodyProps = 0;
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

        // 2011-09-08 to 2011-09-16 sp various changes
        //     - provide support for use in smaller screen displays (support for scrollbar in other files)
        //     - align and resize panels for more unify displays  
        #region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "Absolute Antibody Volume to add",
            "",
            "",
            "",
            ""}, -1, System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, null);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] {
            "Absolute Coctail Volume to add",
            "",
            "",
            "",
            ""}, -1, System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, null);
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem(new string[] {
            "Absolute Particle Volume to add",
            "",
            "",
            "",
            ""}, -1, System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.Window, null);
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem(new string[] {
            "Rel. Antibody Proportion to add",
            "",
            "",
            "",
            ""}, -1, System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.ControlLight, null);
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem(new string[] {
            "Rel. Coctail Proportion to add",
            "",
            "",
            "",
            ""}, -1, System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.ControlLight, null);
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem(new string[] {
            "Rel. Particle Proportion to add",
            "",
            "",
            "",
            ""}, -1, System.Drawing.SystemColors.WindowText, System.Drawing.SystemColors.ControlLight, null);
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem(new string[] {
            "Min Antibody added",
            "",
            "",
            "",
            ""}, -1, System.Drawing.Color.Empty, System.Drawing.SystemColors.Window, null);
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem(new string[] {
            "Min Cocktail added",
            "",
            "",
            "",
            ""}, -1, System.Drawing.Color.Empty, System.Drawing.SystemColors.Window, null);
            System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem(new string[] {
            "Min Particle added",
            "",
            "",
            "",
            ""}, -1, System.Drawing.Color.Empty, System.Drawing.SystemColors.Window, null);
            System.Windows.Forms.ListViewItem listViewItem10 = new System.Windows.Forms.ListViewItem(new string[] {
            "Working (Sample with Min added )",
            "",
            "",
            "",
            ""}, -1, System.Drawing.Color.Empty, System.Drawing.SystemColors.ControlLight, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem11 = new System.Windows.Forms.ListViewItem(new string[] {
            "Working (Separation with Min added)",
            "",
            "",
            "",
            ""}, -1, System.Drawing.Color.Empty, System.Drawing.SystemColors.ControlLight, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem12 = new System.Windows.Forms.ListViewItem(new string[] {
            "Max Antibody added",
            "",
            "",
            "",
            ""}, -1, System.Drawing.Color.Empty, System.Drawing.SystemColors.Window, null);
            System.Windows.Forms.ListViewItem listViewItem13 = new System.Windows.Forms.ListViewItem(new string[] {
            "Max Cocktail added",
            "",
            "",
            "",
            ""}, -1, System.Drawing.Color.Empty, System.Drawing.SystemColors.Window, null);
            System.Windows.Forms.ListViewItem listViewItem14 = new System.Windows.Forms.ListViewItem(new string[] {
            "Max Particle added",
            "",
            "",
            "",
            ""}, -1, System.Drawing.Color.Empty, System.Drawing.SystemColors.Window, null);
            System.Windows.Forms.ListViewItem listViewItem15 = new System.Windows.Forms.ListViewItem(new string[] {
            "Working (Sample with Max added)",
            "",
            "",
            "",
            ""}, -1, System.Drawing.Color.Empty, System.Drawing.SystemColors.ControlLight, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem16 = new System.Windows.Forms.ListViewItem(new string[] {
            "Working (Separation with Max added)",
            "",
            "",
            "",
            ""}, -1, System.Drawing.Color.Empty, System.Drawing.SystemColors.ControlLight, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem17 = new System.Windows.Forms.ListViewItem(new string[] {
            "End-Result Locations",
            "",
            "",
            "",
            ""}, -1, System.Drawing.Color.Empty, System.Drawing.SystemColors.Window, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem18 = new System.Windows.Forms.ListViewItem(new string[] {
            "   Buffer Bottle",
            "",
            "",
            "",
            ""}, -1);
            System.Windows.Forms.ListViewItem listViewItem19 = new System.Windows.Forms.ListViewItem(new string[] {
            "   Buffer Bottle 3/4",
            "",
            "",
            "",
            ""}, -1);
            System.Windows.Forms.ListViewItem listViewItem20 = new System.Windows.Forms.ListViewItem(new string[] {
            "   Buffer Bottle 5/6",
            "",
            "",
            "",
            ""}, -1);
            System.Windows.Forms.ListViewItem listViewItem21 = new System.Windows.Forms.ListViewItem(new string[] {
            "   Waste Tube",
            "",
            "",
            "",
            ""}, -1);
            System.Windows.Forms.ListViewItem listViewItem22 = new System.Windows.Forms.ListViewItem(new string[] {
            "   Negative Fraction Tube",
            "",
            "",
            "",
            ""}, -1, System.Drawing.Color.Empty, System.Drawing.SystemColors.Window, null);
            System.Windows.Forms.ListViewItem listViewItem23 = new System.Windows.Forms.ListViewItem(new string[] {
            "   Sample Tube",
            "",
            "",
            "",
            ""}, -1, System.Drawing.Color.Empty, System.Drawing.SystemColors.Window, null);
            System.Windows.Forms.ListViewItem listViewItem24 = new System.Windows.Forms.ListViewItem(new string[] {
            "   Separation Tube",
            "",
            "",
            "",
            ""}, -1, System.Drawing.Color.Empty, System.Drawing.SystemColors.Window, null);
            System.Windows.Forms.ListViewItem listViewItem25 = new System.Windows.Forms.ListViewItem(new string[] {
            "   END RESULT NOT SPECIFIED",
            "",
            "",
            "",
            ""}, -1);
            this.btnExitClick = new System.Windows.Forms.Button();
            this.MsgListView = new System.Windows.Forms.ListView();
            this.Param = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Q1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Q2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Q3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Q4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.rTextBox = new System.Windows.Forms.RichTextBox();
            this.cbAcceptAsIs = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnExitClick
            // 
            this.btnExitClick.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExitClick.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnExitClick.Location = new System.Drawing.Point(391, 444);
            this.btnExitClick.Name = "btnExitClick";
            this.btnExitClick.Size = new System.Drawing.Size(72, 37);
            this.btnExitClick.TabIndex = 1;
            this.btnExitClick.Text = "EXIT";
            this.btnExitClick.Click += new System.EventHandler(this.btnExitClicked);
            // 
            // MsgListView
            // 
            this.MsgListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.MsgListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Param,
            this.Q1,
            this.Q2,
            this.Q3,
            this.Q4});
            this.MsgListView.GridLines = true;
            listViewItem1.StateImageIndex = 0;
            listViewItem2.StateImageIndex = 0;
            listViewItem3.StateImageIndex = 0;
            listViewItem4.StateImageIndex = 0;
            listViewItem5.StateImageIndex = 0;
            listViewItem6.StateImageIndex = 0;
            listViewItem7.StateImageIndex = 0;
            listViewItem8.StateImageIndex = 0;
            listViewItem9.StateImageIndex = 0;
            listViewItem10.StateImageIndex = 0;
            listViewItem11.StateImageIndex = 0;
            listViewItem12.StateImageIndex = 0;
            listViewItem13.StateImageIndex = 0;
            listViewItem14.StateImageIndex = 0;
            listViewItem15.StateImageIndex = 0;
            listViewItem16.StateImageIndex = 0;
            listViewItem17.StateImageIndex = 0;
            listViewItem22.IndentCount = 5;
            listViewItem22.StateImageIndex = 0;
            listViewItem23.StateImageIndex = 0;
            listViewItem24.StateImageIndex = 0;
            listViewItem25.StateImageIndex = 0;
            this.MsgListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6,
            listViewItem7,
            listViewItem8,
            listViewItem9,
            listViewItem10,
            listViewItem11,
            listViewItem12,
            listViewItem13,
            listViewItem14,
            listViewItem15,
            listViewItem16,
            listViewItem17,
            listViewItem18,
            listViewItem19,
            listViewItem20,
            listViewItem21,
            listViewItem22,
            listViewItem23,
            listViewItem24,
            listViewItem25});
            this.MsgListView.Location = new System.Drawing.Point(1, 1);
            this.MsgListView.MinimumSize = new System.Drawing.Size(430, 96);
            this.MsgListView.Name = "MsgListView";
            this.MsgListView.Size = new System.Drawing.Size(462, 378);
            this.MsgListView.TabIndex = 5;
            this.MsgListView.UseCompatibleStateImageBehavior = false;
            this.MsgListView.View = System.Windows.Forms.View.Details;
            this.MsgListView.SelectedIndexChanged += new System.EventHandler(this.MsgListView_SelectedIndexChanged);
            this.MsgListView.SizeChanged += new System.EventHandler(this.MsgListView_SizeChanged);
            // 
            // Param
            // 
            this.Param.Text = "Volumes (uL) ";
            this.Param.Width = 200;
            // 
            // Q1
            // 
            this.Q1.Text = " Quad 1";
            this.Q1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Q1.Width = 58;
            // 
            // Q2
            // 
            this.Q2.Text = " Quad 2";
            this.Q2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Q3
            // 
            this.Q3.Text = " Quad 3";
            this.Q3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Q4
            // 
            this.Q4.Text = " Quad 4";
            this.Q4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // rTextBox
            // 
            this.rTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.rTextBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.rTextBox.ForeColor = System.Drawing.Color.Black;
            this.rTextBox.Location = new System.Drawing.Point(0, 385);
            this.rTextBox.MaximumSize = new System.Drawing.Size(390, 96);
            this.rTextBox.MinimumSize = new System.Drawing.Size(390, 96);
            this.rTextBox.Name = "rTextBox";
            this.rTextBox.ReadOnly = true;
            this.rTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.rTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rTextBox.Size = new System.Drawing.Size(390, 96);
            this.rTextBox.TabIndex = 6;
            this.rTextBox.Text = "";
            this.rTextBox.WordWrap = false;
            // 
            // cbAcceptAsIs
            // 
            this.cbAcceptAsIs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbAcceptAsIs.Location = new System.Drawing.Point(396, 396);
            this.cbAcceptAsIs.Name = "cbAcceptAsIs";
            this.cbAcceptAsIs.Size = new System.Drawing.Size(67, 37);
            this.cbAcceptAsIs.TabIndex = 7;
            this.cbAcceptAsIs.Text = "Accept As Is...";
            this.cbAcceptAsIs.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbAcceptAsIs.UseVisualStyleBackColor = true;
            this.cbAcceptAsIs.Visible = false;
            this.cbAcceptAsIs.Click += new System.EventHandler(this.cbAcceptAsIsClicked);
            // 
            // ProtocolVolWarning
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(472, 481);
            this.Controls.Add(this.cbAcceptAsIs);
            this.Controls.Add(this.rTextBox);
            this.Controls.Add(this.MsgListView);
            this.Controls.Add(this.btnExitClick);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(480, 800);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(480, 300);
            this.Name = "ProtocolVolWarning";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Protocol Summary";
            this.TopMost = true;
            this.Activated += new System.EventHandler(this.ProtocolVolWarning_Activated);
            this.Deactivate += new System.EventHandler(this.ProtocolVolWarning_Deactivate);
            this.Load += new System.EventHandler(this.ProtocolVolWarning_Load);
            this.ResumeLayout(false);

		}
		#endregion

        private void btnExitClicked(object sender, System.EventArgs e)
        {
            acceptProtocolOverride = this.cbAcceptAsIs.Checked;
        }

        private void cbAcceptAsIsClicked(object sender, System.EventArgs e)
        {
            DialogResult result;
            if (this.cbAcceptAsIs.Checked)
            {
                result = MessageBox.Show("Protocol has inconsistencies.  Please confirm override and acceptance of this protocol under these conditions, or reject using the CANCEL button.",
                                           "Override protocol checking", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Cancel)
                    this.cbAcceptAsIs.Checked = false;
            }
        }

		private void button2_Click(object sender, System.EventArgs e)
		{


		}

		private void ProtocolVolWarning_Activated(object sender, System.EventArgs e)
		{
			/*
						double  num;

						MsgListView.Items[0].SubItems[1].Text = TotalMaxLimit.ToString();
						MsgListView.Items[1].SubItems[1].Text = TotalMinLimit.ToString();
			
						num = (double)RelCock/1000;
						MsgListView.Items[2].SubItems[1].Text = num.ToString();
						num = (double)RelPart/1000;			
						MsgListView.Items[3].SubItems[1].Text = num.ToString();
						num = (double)RelAnti/1000;			
						MsgListView.Items[4].SubItems[1].Text = num.ToString();

						MsgListView.Items[5].SubItems[1].Text = SampleMaxVol.ToString();
						MsgListView.Items[6].SubItems[1].Text = SampleMinVol.ToString();

						MsgListView.Items[7].SubItems[1].Text  = MaxReagCock.ToString();
						MsgListView.Items[8].SubItems[1].Text  = MaxReagPart.ToString();
						MsgListView.Items[9].SubItems[1].Text  = MaxReagAnti.ToString();
						MsgListView.Items[10].SubItems[1].Text = MinReagCock.ToString();
						MsgListView.Items[11].SubItems[1].Text = MinReagPart.ToString();
						MsgListView.Items[12].SubItems[1].Text = MinReagAnti.ToString();

						MsgListView.Items[13].SubItems[1].Text = SumMax.ToString();
						MsgListView.Items[14].SubItems[1].Text = SumMin.ToString();			
			*/

		}

        // 2011-11-08 sp
        // appends a line to the text with the specified scaleFactor applied to the current font size
        private void richTextBox_AppendTextLine( RichTextBox rTextBox, string msg, float FontSizeFactor, float lineSpacingFactor, Color fontColor )
        {
            System.Drawing.Font currentFont = rTextBox.SelectionFont;
            rTextBox.SelectionColor = fontColor;
            rTextBox.SelectionFont = new Font(currentFont.FontFamily, FontSizeFactor * currentFont.Size, FontStyle.Regular);
            rTextBox.AppendText("  " + msg + Environment.NewLine);
            if (lineSpacingFactor > 0)
            {
                rTextBox.SelectionFont = new Font(currentFont.FontFamily, lineSpacingFactor * currentFont.Size, FontStyle.Regular);
                rTextBox.AppendText(Environment.NewLine);
            }
            rTextBox.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, FontStyle.Regular);
            rTextBox.SelectionColor = Color.Black;
        }


        // 2011-09-12 sp
        // rename SetVolumeErrorColor to more generic display function
        private void SetCheckProtocolDisplayContents(int code)
        {
            int decode = 0, i;
            string msg;
            int rowOffset = 6;


            // 2011-09-09 sp
            // change display format
//            MsgBox.Items.Clear();
            //            MsgBox.Items.Add("Please review the volumes below:");
            //MsgBox.Items.Add("    *( yellow = below recommended;  red = exceed acceptable )");
            //MsgBox.Items.Add("");

            float defaultFontSizeFactor = 1.0f;
            float legendTextFactor = 0.9f;
            float extraLineSpacingFactor = 0.2f;
            System.Drawing.Font currentFont = rTextBox.SelectionFont;
            rTextBox.SelectionFont = new Font(currentFont.FontFamily, legendTextFactor * currentFont.Size, FontStyle.Regular);
            rTextBox.SelectionColor = defaultTextColor;
            rTextBox.AppendText("     *( ");
            rTextBox.SelectionColor = warningTextColor;
            rTextBox.AppendText("maroon = exceed recommended;");
            rTextBox.SelectionColor = errorTextColor;
            rTextBox.AppendText("  red = beyond acceptable");
            rTextBox.SelectionColor = defaultTextColor;
            rTextBox.AppendText(" )");
            rTextBox.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, FontStyle.Regular);
            richTextBox_AppendTextLine(rTextBox, "", legendTextFactor, extraLineSpacingFactor, defaultTextColor);

            msg = "Desired sample range: [ " + SampleMinVol.ToString() + ", " + SampleMaxVol.ToString() + " ] uL.";
            richTextBox_AppendTextLine(rTextBox, msg, defaultFontSizeFactor, extraLineSpacingFactor, defaultTextColor);
            msg = "Estimated duration: ~" + (TotalIncubationTime + TotalSeparationTime + TotalExtensionTime).ToString() + " min(m)";
            //MsgBox.Items.Add(msg);
//            rTextBox.AppendText(Environment.NewLine + msg);
            richTextBox_AppendTextLine(rTextBox, msg, defaultFontSizeFactor, 0.0f, defaultTextColor);
            msg = "   " + NumberOfIncubations + " incubations ~" + TotalIncubationTime.ToString() + " m;   "
                + NumberOfSeparations.ToString() + " separations ~" + TotalSeparationTime.ToString() + " m;   and "
                + "extensions ~" + TotalExtensionTime.ToString() + " m.";
            //MsgBox.Items.Add(msg);
//            rTextBox.AppendText(Environment.NewLine + msg);
            richTextBox_AppendTextLine(rTextBox, msg, defaultFontSizeFactor, extraLineSpacingFactor, defaultTextColor);

            /*			
                        msg = "Relative Cocktail Proportion:";
                        MsgBox.Items.Add(msg);
                        for (i = 0 ; i < 4 ; i++)
                        {
                            num = (double)RelCock[i]/1000f;
                            msg = "[Quad"+ (i+1).ToString()+"]" + "  " + num.ToString();
                            MsgBox.Items.Add(msg);
                        }
                        msg = "Relative Particle Proportion:";
                        MsgBox.Items.Add(msg);
                        for (i = 0 ; i < 4 ; i++)
                        {
                            num = (double)RelPart[i]/1000f;
                            msg = "[Quad"+ (i+1).ToString()+"]" + "  " + num.ToString();
                            MsgBox.Items.Add(msg);
                        }
                        msg = "Relative Antibody Proportion:";
                        MsgBox.Items.Add(msg);
                        for (i = 0 ; i < 4 ; i++)
                        {
                            num = (double)RelAnti[i]/1000f;
                            msg = "[Quad"+ (i+1).ToString()+"]" + "  " + num.ToString();
                            MsgBox.Items.Add(msg);
                        }
            */

            //			MsgBox.Items.Add("");

            // 2011-09-09 sp
            // reorder to match description/protocol order
            String numberFormat = "{0,6}";

            for (i = 0; i < nItems; i++)
            {
                int index = 0;
                int item = i + 1;
                //                MsgListView.Items[index++].SubItems[i + 1].Text = ((double)RelAnti[i] / 1000f).ToString();
                MsgListView.Items[index++].SubItems[item].Text = String.Format(numberFormat, AbsAnti[i]);
                MsgListView.Items[index++].SubItems[item].Text = String.Format(numberFormat, AbsCock[i]);
                MsgListView.Items[index++].SubItems[item].Text = String.Format(numberFormat, AbsPart[i]);
                MsgListView.Items[index++].SubItems[item].Text = String.Format(numberFormat, (double)RelAnti[i] / 1000f);
                MsgListView.Items[index++].SubItems[item].Text = String.Format(numberFormat, (double)RelCock[i] / 1000f);
                MsgListView.Items[index++].SubItems[item].Text = String.Format(numberFormat, (double)RelPart[i] / 1000f);
                /*
                MsgListView.Items[0].SubItems[i+1].Text = MaxReagCock[i].ToString();
				MsgListView.Items[1].SubItems[i+1].Text = MaxReagPart[i].ToString();
				MsgListView.Items[2].SubItems[i+1].Text = MaxReagAnti[i].ToString();
				MsgListView.Items[3].SubItems[i+1].Text = MinReagCock[i].ToString();
				MsgListView.Items[4].SubItems[i+1].Text = MinReagPart[i].ToString();
				MsgListView.Items[5].SubItems[i+1].Text = MinReagAnti[i].ToString();
                */
                index = rowOffset + 6;
                MsgListView.Items[index].UseItemStyleForSubItems = false;
                MsgListView.Items[index].SubItems[item].Text = String.Format(numberFormat, MaxReagCock[i]);
                setItemColorFromFlag(MsgListView.Items[index], item, MaxReagCockFlags[i]);
                index = rowOffset + 7;
                MsgListView.Items[index].UseItemStyleForSubItems = false;
                MsgListView.Items[index].SubItems[item].Text = String.Format(numberFormat, MaxReagPart[i]);
                setItemColorFromFlag(MsgListView.Items[index], item, MaxReagPartFlags[i]);
                index = rowOffset + 5;
                MsgListView.Items[index].UseItemStyleForSubItems = false;
                MsgListView.Items[index].SubItems[item].Text = String.Format(numberFormat, MaxReagAnti[i]);
                setItemColorFromFlag(MsgListView.Items[index], item, MaxReagAntiFlags[i]);
                index = rowOffset + 1;
                MsgListView.Items[index].UseItemStyleForSubItems = false;
                MsgListView.Items[index].SubItems[item].Text = String.Format(numberFormat, MinReagCock[i]);
                setItemColorFromFlag(MsgListView.Items[index], item, MinReagCockFlags[i]);
                index = rowOffset + 2;
                MsgListView.Items[index].UseItemStyleForSubItems = false;
                MsgListView.Items[index].SubItems[item].Text = String.Format(numberFormat, MinReagPart[i]);
                setItemColorFromFlag(MsgListView.Items[index], item, MinReagPartFlags[i]);
                index = rowOffset + 0;
                MsgListView.Items[index].UseItemStyleForSubItems = false;
                MsgListView.Items[index].SubItems[item].Text = String.Format(numberFormat, MinReagAnti[i]);
                setItemColorFromFlag(MsgListView.Items[index], item, MinReagAntiFlags[i]);
                index = rowOffset + 8;
                MsgListView.Items[index].UseItemStyleForSubItems = false;
                MsgListView.Items[index].SubItems[item].Text = String.Format(numberFormat, MaxWorkingSample[i]);
                setItemColorFromFlag(MsgListView.Items[index], item, MaxWorkingSampleFlags[i]);
                index = rowOffset + 3;
                MsgListView.Items[index].UseItemStyleForSubItems = false;
                MsgListView.Items[index].SubItems[item].Text = String.Format(numberFormat, MinWorkingSample[i]);
                setItemColorFromFlag(MsgListView.Items[index], item, MinWorkingSampleFlags[i]);
                index = rowOffset + 9;
                MsgListView.Items[index].UseItemStyleForSubItems = false;
                MsgListView.Items[index].SubItems[item].Text = String.Format(numberFormat, MaxWorkingSeparation[i]);
                setItemColorFromFlag(MsgListView.Items[index], item, MaxWorkingSeparationFlags[i]);
                index = rowOffset + 4;
                MsgListView.Items[index].UseItemStyleForSubItems = false;
                MsgListView.Items[index].SubItems[item].Text = String.Format(numberFormat, MinWorkingSeparation[i]);
                setItemColorFromFlag(MsgListView.Items[index], item, MinWorkingSeparationFlags[i]);

            }

            // 2011-09-12 sp
            // added resolution for warning limit levels
            decode = (code & 0xFFFE);
            if (decode != 0)
            {
                richTextBox_AppendTextLine(rTextBox, "** Volume transfers exceed acceptable limits in at least one transport step.", defaultFontSizeFactor,
                            0.0f, errorTextColor);
                richTextBox_AppendTextLine(rTextBox, "** See errors above and in protocol Command Sequence list.", defaultFontSizeFactor,
                            extraLineSpacingFactor, errorTextColor);
            }

            //decode = (code & 0x1);
            //if (decode != 0)
            //    MsgListView.Items[0 + rowOffset].ForeColor = warningTextColor;

            //decode = (code & 0x2);
            //if (decode != 0)
            //    MsgListView.Items[1 + rowOffset].ForeColor = warningTextColor;

            //decode = (code & 0x4);
            //if (decode != 0)
            //    MsgListView.Items[2 + rowOffset].ForeColor = warningTextColor;

            //decode = (code & 0x10);
            //if (decode != 0)
            //    MsgListView.Items[0 + rowOffset].ForeColor = errorTextColor;

            //decode = (code & 0x20);
            //if (decode != 0)
            //    MsgListView.Items[1 + rowOffset].ForeColor = errorTextColor;

            //decode = (code & 0x40);
            //if (decode != 0)
            //    MsgListView.Items[2 + rowOffset].ForeColor = errorTextColor;

            //decode = (code & 0x100);
            //if (decode != 0)
            //    MsgListView.Items[5 + rowOffset].ForeColor = errorTextColor;

            //decode = (code & 0x200);
            //if (decode != 0)
            //    MsgListView.Items[6 + rowOffset].ForeColor = errorTextColor;

            //decode = (code & 0x400);
            //if (decode != 0)
            //    MsgListView.Items[7 + rowOffset].ForeColor = errorTextColor;

            //decode = (code & 0x1000);
            //if (decode != 0)
            //    MsgListView.Items[3 + rowOffset].ForeColor = errorTextColor;

            //decode = (code & 0x2000);
            //if (decode != 0)
            //    MsgListView.Items[8 + rowOffset].ForeColor = errorTextColor;

            //decode = (code & 0x4000);
            //if (decode != 0)
            //    MsgListView.Items[4 + rowOffset].ForeColor = errorTextColor;

            //decode = (code & 0x8000);
            //if (decode != 0)
            //    MsgListView.Items[9 + rowOffset].ForeColor = errorTextColor;

            // 2011-09-12
            // remove upper warning flage bits
            //code &= 0xFF;

            //if (code >= 32)
            //{
            //    //MsgBox.Items.Add("This protocol has different reletive proportion values.");
            //    rTextBox.AppendText(Environment.NewLine + "This protocol has different reletive proportion values.");
            //    richTextBox_AppendTextLine(rTextBox, msg, defaultFontSizeFactor, extraLineSpacingFactor, warningTextColor);
            //}

            //decode = (code & 32);
            //if (decode != 0)
            //{
            //    //MsgBox.Items.Add("Check your Cocktail reletive proportion values!");
            //    rTextBox.AppendText(Environment.NewLine + "Check your Cocktail reletive proportion values!");
            //    richTextBox_AppendTextLine(rTextBox, msg, defaultFontSizeFactor, extraLineSpacingFactor, warningTextColor);
            //}

            // 2011-11-07 sp -- added en-result selection marker
            ProtocolModel theProtocolModel = ProtocolModel.GetInstance();
            rowOffset = 17;


            string checkedStr = "   X";
            bool boEndResultSelected = false;
            bool boRS16 = SeparatorResourceManager.isPlatformRS16();
            int not_specified_idx_offset = boRS16?7:5;
            MsgListView.Items[not_specified_idx_offset + rowOffset].SubItems[0].Text = "";// empty NOT_SPECIFIED_TEXT
            try 
            {
                
                resultVialChecks results;
                string sIndent ="   ";
                bool[] quadrantsUsed = theProtocolModel.GetQuadrantUse();
                for (int quadrant = 0; quadrant < quadrantsUsed.Length; quadrant++)
                {
                    if (!quadrantsUsed[quadrant]) continue;
                    int resultIdx = 0;
                    //theProtocolModel.GetResultVialChecks(quadrant, out resultVialChecks);
                    results = theProtocolModel.GetResultVialChecks(quadrant);
                    MsgListView.Items[resultIdx + rowOffset].SubItems[0].Text = sIndent + SeparatorResourceManager.GetSeparatorString(StringId.QuadrantBuffer);
                    if (results.bufferBottle) MsgListView.Items[resultIdx + rowOffset].SubItems[quadrant + 1].Text = checkedStr;
                    resultIdx++;
                    if (boRS16)
                    {
                        MsgListView.Items[resultIdx + rowOffset].SubItems[0].Text = sIndent + SeparatorResourceManager.GetSeparatorString(StringId.QuadrantBuffer34);
                        if (results.bufferBottle34) MsgListView.Items[resultIdx + rowOffset].SubItems[quadrant + 1].Text = checkedStr;
                        resultIdx++;
                        MsgListView.Items[resultIdx + rowOffset].SubItems[0].Text = sIndent + SeparatorResourceManager.GetSeparatorString(StringId.QuadrantBuffer56);
                        if (results.bufferBottle56) MsgListView.Items[resultIdx + rowOffset].SubItems[quadrant + 1].Text = checkedStr;
                        resultIdx++;
                    } 
                    MsgListView.Items[resultIdx + rowOffset].SubItems[0].Text = sIndent + SeparatorResourceManager.GetSeparatorString(StringId.WasteTube);
                    if (results.wasteTube) MsgListView.Items[resultIdx + rowOffset].SubItems[quadrant + 1].Text = checkedStr;
                    resultIdx++;
                    MsgListView.Items[resultIdx + rowOffset].SubItems[0].Text = sIndent + SeparatorResourceManager.GetSeparatorString(StringId.NegativeFractionTube);
                    if (results.lysisBufferTube) MsgListView.Items[resultIdx + rowOffset].SubItems[quadrant + 1].Text = checkedStr;
                    resultIdx++;
                    MsgListView.Items[resultIdx + rowOffset].SubItems[0].Text = sIndent + SeparatorResourceManager.GetSeparatorString(StringId.SampleTube);
                    if (results.sampleTube) MsgListView.Items[resultIdx + rowOffset].SubItems[quadrant + 1].Text = checkedStr;
                    resultIdx++;
                    MsgListView.Items[resultIdx + rowOffset].SubItems[0].Text = sIndent + SeparatorResourceManager.GetSeparatorString(StringId.SeparationTube);
                    if (results.separationTube) MsgListView.Items[resultIdx + rowOffset].SubItems[quadrant + 1].Text = checkedStr;
                    resultIdx++;

                    MsgListView.Items[resultIdx + rowOffset].SubItems[0].Text = "";
                    resultIdx++;
                    if (!boRS16)
                    {
                        MsgListView.Items[resultIdx + rowOffset].SubItems[0].Text = "";
                        resultIdx++;
                        MsgListView.Items[resultIdx + rowOffset].SubItems[0].Text = "";
                        resultIdx++;
                    }
                    boEndResultSelected = boEndResultSelected || results.bufferBottle || 
                        results.bufferBottle34 || results.bufferBottle56 || 
                        results.wasteTube || results.lysisBufferTube || 
                        results.sampleTube || results.separationTube;
                }
            }
            catch (Exception) { }
            /*
            if (defaultEndResult[0] >= 0)
            {
                if (defaultEndResult[1] >= 0)
                {
                    int quadrant = defaultEndResult[0];
                    if (defaultEndResult[1] == NegFractionIndex)
                        MsgListView.Items[0 + rowOffset].SubItems[quadrant + 1].Text = checkedStr;
                    else if (defaultEndResult[1] == sampleTubeIndex)
                        MsgListView.Items[1 + rowOffset].SubItems[quadrant + 1].Text = checkedStr;
                    else if (defaultEndResult[1] == separationTubeIndex)
                        MsgListView.Items[2 + rowOffset].SubItems[quadrant + 1].Text = checkedStr;


                    // 2011-11-17 sp -- removed waste tube display 
                    //else if (defaultEndResult[1] == wasteTubeIndex)
                    //    MsgListView.Items[3 + rowOffset].SubItems[quadrant + 1].Text = checkedStr;

                    MsgListView.Items[3 + rowOffset].SubItems[0].Text = NOT_SPECIFIED_TEXT;
                    MsgListView.Items[3 + rowOffset].SubItems[quadrant+1].Text = checkedStr;
                    richTextBox_AppendTextLine(rTextBox, "** No end-result/product specified; default location ( last tube ) selected.", defaultFontSizeFactor,
                                extraLineSpacingFactor, warningTextColor);
                }
                else
                {
                    richTextBox_AppendTextLine(rTextBox, "** No end-result/product specified.", defaultFontSizeFactor,
                                extraLineSpacingFactor, warningTextColor);
                    for (int rr = -1; rr < 4; rr++)
                    {
                        MsgListView.Items[rowOffset + rr].ForeColor = warningTextColor;
                        MsgListView.Items[rowOffset + rr].BackColor = warningBackColor;
                    }

                    MsgListView.Items[3 + rowOffset].SubItems[0].Text = NOT_SPECIFIED_TEXT;
                    MsgListView.Items[3 + rowOffset].SubItems[1].Text = checkedStr;
                }
            }
            */
            if (!boEndResultSelected)
            {
                richTextBox_AppendTextLine(rTextBox, "** No end-result/product specified.", defaultFontSizeFactor,
                            extraLineSpacingFactor, warningTextColor);
                for (int rr = -1; rr < not_specified_idx_offset+1; rr++)
                {
                    MsgListView.Items[rowOffset + rr].ForeColor = warningTextColor;
                    MsgListView.Items[rowOffset + rr].BackColor = warningBackColor;
                }

                MsgListView.Items[not_specified_idx_offset + rowOffset].SubItems[0].Text = NOT_SPECIFIED_TEXT;
                MsgListView.Items[not_specified_idx_offset + rowOffset].SubItems[1].Text = checkedStr;
            }

            if ((code & 0xFFFE) != 0)
                cbAcceptAsIs.Visible = true;
        }

        private Color getItemFlagColor(int flag )
        {
            if( flag >= 2 )
                return errorTextColor;
            else if( flag == 1 )
                return warningTextColor;
            else
                return defaultTextColor;
        }

        private void setItemColorFromFlag(ListViewItem listItem, int item, int flag)
        {
            if (flag >= 2)
            {
                listItem.SubItems[item].ForeColor = errorTextColor;
                listItem.SubItems[item].BackColor = errorBackColor;
            }
            else if (flag == 1)
            {
                listItem.SubItems[item].ForeColor = warningTextColor;
                listItem.SubItems[item].BackColor = warningBackColor;
            }
            else
            {
                listItem.SubItems[item].ForeColor = defaultTextColor;
                listItem.SubItems[item].BackColor = listItem.SubItems[0].BackColor;
            }
        }


		private void ProtocolVolWarning_Deactivate(object sender, System.EventArgs e)
		{
	
		}

		private void ProtocolVolWarning_Load(object sender, System.EventArgs e)
		{
            MsgListView.EnsureVisible(MsgListView.Items.Count - 1);
        }

		private void MsgBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
		
		}

		private void MsgListView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
		
		}
		protected override void OnClosed(System.EventArgs e)
		{
			int i;
            //MsgBox.Items.Clear();
            rTextBox.Clear();
			SampleMaxVol	= SampleMinVol = 0;
			TotalMaxLimit	= TotalMinLimit = 0f;
			nItems			= 0;

            // 2011-09-12 sp
            // added report for incubation and separation status
            TotalIncubationTime = 0f;
            TotalSeparationTime = 0f;
            NumberOfSeparations = 0;
            NumberOfIncubations = 0;
            TotalExtensionTime = 0f;

			MaxReagCock = new double[4];
			MaxReagPart = new double[4];
			MaxReagAnti = new double[4];			
			MinReagCock = new double[4];
			MinReagPart = new double[4];
			MinReagAnti = new double[4];
			SumMax		= new double[4];
			SumMin      = new double[4];

            Array.Clear(RelCock, 0, 4);
            Array.Clear(RelPart, 0, 4);
            Array.Clear(RelAnti, 0, 4);
            // 2011-09-09 sp added absolute volume reporting
            Array.Clear(AbsCock, 0, 4);
            Array.Clear(AbsPart, 0, 4);
            Array.Clear(AbsAnti, 0, 4);
            Array.Clear(MaxReagCock, 0, 4);
			Array.Clear( MaxReagPart, 0, 4);	
			Array.Clear( MaxReagAnti, 0, 4);	
			Array.Clear( MinReagCock, 0, 4);
			Array.Clear( MinReagPart, 0, 4);	
			Array.Clear( MinReagAnti, 0, 4);
            Array.Clear(SumMax, 0, 4);
            Array.Clear(SumMin, 0, 4);
            Array.Clear(MinWorkingSample, 0, 4);
            Array.Clear(MaxWorkingSample, 0, 4);
            Array.Clear(MinWorkingSeparation, 0, 4);
            Array.Clear(MaxWorkingSeparation, 0, 4);

			for ( i = 0 ; i < 10 ; i++)
				MsgListView.Items[i].SubItems[1].Text = "";

			base.OnClosed(e);
		}

        private void MsgListView_SizeChanged(object sender, EventArgs e)
        {
            MsgListView.EnsureVisible(MsgListView.Items.Count-1);
        }

	}
}
