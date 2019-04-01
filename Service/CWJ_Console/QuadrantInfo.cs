using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Tesla.Common.Protocol;

using GUI_Controls;

namespace GUI_Console
{
    class QuadrantInfo
    {
        public string QuadrantLabel;
        public string Quadrant_message;
        public double QuadrantVolume;
        public int QuadrantsRequired;
        public double volMin;
        public double volMax;
        public bool bQuadrantInUse;
        public bool bIsMaintenance = false;

        private TextBox Name;
        private Label Vol;
        private PictureBox Cancel;
        private Button_Quadrant Icon;
        public Panel Divider;


        public QuadrantInfo(TextBox tb, Label lbl, PictureBox pb, Button_Quadrant ic)
        {
            QuadrantLabel = "";
            Quadrant_message = "";
            QuadrantVolume = 0;
            QuadrantsRequired = 0;
            volMin = 0;
            volMax = 0;
            bQuadrantInUse = false;

            Name = tb;
            Vol = lbl;
            Cancel = pb;
            Icon = ic;
            Divider = null;
        }

        public void setProtocol(ISeparationProtocol protocol)
        {
            QuadrantLabel = protocol.Label;
            Quadrant_message = protocol.Description;
            QuadrantVolume = Convert.ToDouble(Vol.Text);
            volMax = protocol.MaximumSampleVolume / 1000;
            volMin = protocol.MinimumSampleVolume / 1000;
            bQuadrantInUse = true;
            QuadrantsRequired = protocol.QuadrantCount;
            
            Update();
        }

        public void Clear()
        {
            QuadrantLabel = "";
            Quadrant_message = "";
            QuadrantVolume = 0;
            QuadrantsRequired = 0;
            volMin = 0;
            volMax = 0;
            bQuadrantInUse = false;
            if (Divider != null)
                Divider.Visible = true;
            bIsMaintenance = false;
            Update();

            // LOG
            string logMSG = "Quadrant info Cleared";
            GUI_Controls.uiLog.LOG(this, "Clear", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);
        }

        public void Update()
        {
            Name.Text = QuadrantLabel;
            if (QuadrantVolume == 0)
            { Vol.Text = ""; }
            else
            { Vol.Text = string.Format("{0:0.00}", QuadrantVolume) + " mL"; }
            Cancel.Visible = bIsMaintenance || !(bQuadrantInUse && QuadrantVolume == 0) ? bQuadrantInUse: false;
            Icon.Check = !bQuadrantInUse;
        }

        public void updateVolume()
        {
            try
            {
                QuadrantVolume = Convert.ToDouble(Vol.Text);
            }
            catch
            {
            }
        }

        public void setDivider(Panel divider)
        {
            Divider = divider;
        }

        public Label GetVolumeLabel()
        {
            return Vol;
        }

    }
}
