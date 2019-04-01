using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Tesla.Common.Protocol;
using Invetech.ApplicationLog;
using GUI_Controls;

namespace GUI_Console
{
    public class QuadrantInfo
    {
        public string QuadrantLabel;
        public string QuadrantLabel_Abbv;
        public string Quadrant_message;
        public double QuadrantVolume;
        public int QuadrantsRequired;
        public double volMin;
        public double volMax;
        public bool bQuadrantInUse;
        public bool bIsMaintenance = false;
        private Button_Quadrant2 Icon;
        private SizableLabel Name;
        private Label Vol;
        private Label SelectProtocol;
        public Panel Divider;
 
        public QuadrantInfo(SizableLabel tb, Label volLabel, Label selectLabel, Button_Quadrant2 ic)
        {
            QuadrantLabel = "";
            QuadrantLabel_Abbv = "";
            Quadrant_message = "";
            QuadrantVolume = 0;
            QuadrantsRequired = 0;
            volMin = 0;
            volMax = 0;
            bQuadrantInUse = false;

            Name = tb;
            Vol = volLabel;
            SelectProtocol = selectLabel;
            Icon = ic;
            Divider = null;
        }

        public QuadrantInfo(QuadrantInfo QI)
        {
            QuadrantLabel = QI.QuadrantLabel;
            QuadrantLabel_Abbv = QI.QuadrantLabel_Abbv;
            Quadrant_message = QI.Quadrant_message;
            QuadrantVolume = QI.QuadrantVolume;
            QuadrantsRequired = QI.QuadrantsRequired;
            volMin = QI.volMin;
            volMax = QI.volMax;
            bQuadrantInUse = QI.bQuadrantInUse;

            Name = null;
            Vol = null;
            SelectProtocol = null;
            Icon = null;
            Divider = null;
        }

        public void setProtocol(ISeparationProtocol protocol)
        {
            // Sunny to do 
            QuadrantLabel = protocol.Label;
            QuadrantLabel_Abbv = RoboSep_UserDB.getInstance().getProtocolAbbvNameFromList(protocol.Label);
            Quadrant_message = protocol.Description;
            QuadrantVolume = Convert.ToDouble(Vol.Text);
            volMax = protocol.MaximumSampleVolume / 1000;
            volMin = protocol.MinimumSampleVolume / 1000;                                                                                   
            bQuadrantInUse = true;
            SelectProtocol.Visible = false;
            QuadrantsRequired = protocol.QuadrantCount;
            
            
            Update();
        }

        public void Clear()
        {
            QuadrantLabel = "";
            QuadrantLabel_Abbv = "";
            Quadrant_message = "";
            QuadrantVolume = 0;
            QuadrantsRequired = 0;
            volMin = 0;
            volMax = 0;
            bQuadrantInUse = false;
            if (Divider != null)
                Divider.Visible = true;
            bIsMaintenance = false;
            if (SelectProtocol != null)
            {
                SelectProtocol.Visible = true;
            }
            Update();

            // LOG
            string logMSG = "Quadrant info Cleared";
            //GUI_Controls.uiLog.LOG(this, "Clear", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

            LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);                
        }

        public void Update()
        {
            if (Name != null)
            {
                bool DisplayProtocolsAbbv = RoboSep_UserDB.getInstance().UseAbbreviationsForProtocolNames;
                Name.Text = DisplayProtocolsAbbv ? ((!string.IsNullOrEmpty(QuadrantLabel_Abbv)) ? QuadrantLabel_Abbv : QuadrantLabel) : QuadrantLabel;
            }
            if (Vol != null)
            {
                if (QuadrantVolume == 0)
                { 
                    Vol.Text = ""; 
                }
                else
                { 
                    Vol.Text = string.Format("{0:0.00}", QuadrantVolume) + " mL"; 
                }
            }
      //      Cancel.Visible = bIsMaintenance || !(bQuadrantInUse && QuadrantVolume == 0) ? bQuadrantInUse: false;
            if (Icon != null)
                Icon.Check = !bQuadrantInUse;
        }

        public void RefreshDisplayName()
        {
            if (Name != null)
            {
                bool DisplayProtocolsAbbv = RoboSep_UserDB.getInstance().UseAbbreviationsForProtocolNames;
                Name.Text = DisplayProtocolsAbbv ? ((!string.IsNullOrEmpty(QuadrantLabel_Abbv)) ? QuadrantLabel_Abbv : QuadrantLabel) : QuadrantLabel;
            }
        }

        public bool QuadrantUsed
        {
            get
            {
                return bQuadrantInUse;
            }
            set
            {
                bQuadrantInUse = value;
                SelectProtocol.Visible = !value;
            }
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
