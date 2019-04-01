using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace GUI_Controls
{
    public partial class QuadrantSelectionDialog : Form
    {
        // variable declaration
        private GUIButton[] btnQuadrants;
        private bool[] myQuadrantStatus;
        public Point Offset;
        private QuadrantSelectionState[] mySelectionState;
        public enum QuadrantSelectionState
        {
            ENABLED,
            SELECTED_ALWAYS,
            DISABLED
        };

        public QuadrantSelectionDialog(QuadrantSelectionState Q1,
			QuadrantSelectionState Q2,
			QuadrantSelectionState Q3,
			QuadrantSelectionState Q4)
        {
            InitializeComponent();

            SuspendLayout();

            myQuadrantStatus = new bool[4];
            btnQuadrants = new GUIButton[] {
                button_Q1,
                button_Q2,
                button_Q3,
                button_Q4};
            mySelectionState = new QuadrantSelectionState[4] { Q1, Q2, Q3, Q4};
            // disable quadrants with QuadrantSelectionState.DISABLE

            // change quadrant graphics
            List<Image> ilist = new List<Image>();
            // Quadrant buttons
            ilist.Add(Properties.Resources.btnLG_CLICK);
            ilist.Add(Properties.Resources.btnLG_CLICK);
            ilist.Add(Properties.Resources.btnLG_HIGHLIGHT);
            ilist.Add(Properties.Resources.btnLG_HIGHLIGHT);
            button_Q1.ChangeGraphics(ilist);
            button_Q2.ChangeGraphics(ilist);
            button_Q3.ChangeGraphics(ilist);
            button_Q4.ChangeGraphics(ilist);
            button_Q1.disableImage = Properties.Resources.btnLG_STD;
            button_Q2.disableImage = Properties.Resources.btnLG_STD;
            button_Q3.disableImage = Properties.Resources.btnLG_STD;
            button_Q4.disableImage = Properties.Resources.btnLG_STD;
            // ok and cancel buttons
            ilist.Clear();
            ilist.Add(Properties.Resources.btnsmall_STD);
            ilist.Add(Properties.Resources.btnsmall_STD);
            ilist.Add(Properties.Resources.btnsmall_STD);
            ilist.Add(Properties.Resources.btnsmall_CLICK);
            button1.ChangeGraphics(ilist);
            button2.ChangeGraphics(ilist);

            for (int i = 0; i < 4; i++)
            {
                // set quadrant buttons to toggle mode
                btnQuadrants[i].setToggle(true);

                // set quadrants to states based on their QuadrantSelectionState
                if (mySelectionState[i] == QuadrantSelectionState.DISABLED)
                {
                    disableQuadrant(i);
                }
                else if (mySelectionState[i] == QuadrantSelectionState.SELECTED_ALWAYS)
                {
                    btnQuadrants[i].Check = true;
                    btnQuadrants[i].Enabled = false;
                }
                myQuadrantStatus[i] = btnQuadrants[i].Check;
            }

            ResumeLayout();
        }

        public string[] Labels
        {
            set
            {
                if (value.Length == 3)
                {
                    label_WindowsTitle.Text = value[0];
                    button1.Text = value[1];
                    button2.Text = value[2];
                }
            }
        }

        private void QuadrantSelectionDialog_Load(object sender, EventArgs e)
        {
            
            int x = (640 - 300) / 2;
            int y = ((480 - 370) / 2);// +25;
            Offset = new Point(x, y);
            // store "current" location
            x = this.Location.X;
            y = this.Location.Y;
            this.Location = new Point(x + Offset.X, y + Offset.Y);
        }

        private void disableQuadrant(int Q)
        {
            btnQuadrants[Q].Check = false;
            //btnQuadrants[Q].BackgroundImage = btnQuadrants[Q].disableImage;
            //btnQuadrants[Q].Enabled = false;
            btnQuadrants[Q].disable(true);
        }

        public bool[] getQuadrantSelectionStatus()
        {
            return myQuadrantStatus;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void button_Q1_Click(object sender, EventArgs e)
        { QuadrantClick(0);}
        private void button_Q2_Click(object sender, EventArgs e)
        { QuadrantClick(1);}
        private void button_Q3_Click(object sender, EventArgs e)
        { QuadrantClick(2);}
        private void button_Q4_Click(object sender, EventArgs e)
        { QuadrantClick(3); }

        private void QuadrantClick(int Q)
        {
            // update quadrant status
            myQuadrantStatus[Q] = btnQuadrants[Q].Check;

            // enable / disable Ok button if no quadrants
            // have been selected
            int selectedCount = 0;
            for (int i = 0; i < 4; i++)
            {
                if (myQuadrantStatus[i])
                    selectedCount++;
            }
            button1.Enabled = selectedCount > 1 ? true : false;
        }
    }
}
