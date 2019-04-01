using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GUI_Controls
{
    [Description("Select Flasher Interval")]
    public enum FlashIntervalSpeed { Slow = 0, Mid = 1, Fast = 2, BlipSlow = 3, BlipMid = 4, BlipFast = 5 }

    [Description("Flasher TextBox Control")]
    public partial class FlashTextBox : System.Windows.Forms.TextBox
    {
        protected const int m_iFlashIntervalMid = 500;
        protected const int m_iFlashIntervalFast = 200;
        protected const int m_iFlashIntervalSlow = 1000;
        protected const int m_iFlashIntervalBlipOn = 70;
        protected Color colorBackgroundOff = SystemColors.Control;
        protected Color colorBackgroundOn = Color.LightGreen;
        protected Color colorForegroundOn = Color.White;
        protected Color colorForegroundOff = Color.Black;

        protected bool m_bIsFlashEnabled = false;
        protected int iFlashPeriodON;
        protected int iFlashPeriodOFF;
        protected Timer timer;

        [Browsable(true), CategoryAttribute("Appearance"), 
        Description("Get/Set textbox background color while 'OFF' flash period or disabled"),System.ComponentModel.RefreshProperties(RefreshProperties.Repaint)]

        public Color FlasherTextBoxColorBackgroundOff { get { return colorBackgroundOff; } set { colorBackgroundOff = value; } }

        [Browsable(true), CategoryAttribute("Appearance"), 
        Description("Get/Set textbox foreground color while 'OFF' flash period or disabled"),System.ComponentModel.RefreshProperties(RefreshProperties.Repaint)]

        public Color FlasherTextBoxColorForegroundOff { get { return colorForegroundOff; } set { colorForegroundOff = value; } }

        [Browsable(true),CategoryAttribute("Appearance"),
        Description("Get/Set textbox background color while 'ON' flash period"), System.ComponentModel.RefreshProperties(RefreshProperties.Repaint)]

        public Color FlasherTextBoxColorBackgroundOn { get { return colorBackgroundOn; } set { colorBackgroundOn = value; base.BackColor = colorBackgroundOn; } }

        [Browsable(true), CategoryAttribute("Appearance"),
        Description("Get/Set textbox foreground color while 'ON' flash period"), System.ComponentModel.RefreshProperties(RefreshProperties.Repaint)]

        public Color FlasherTextBoxColorForegroundOn { get { return colorForegroundOn; } set { colorForegroundOn = value; base.ForeColor = colorForegroundOn; } }

        [Browsable(true),CategoryAttribute("Appearance"),
        Description("Get flasher status, True=flashing, False=disabled"), System.ComponentModel.RefreshProperties(RefreshProperties.Repaint)]

        public bool FlasherTextBoxStatus { get { return m_bIsFlashEnabled; } }       // True = flashing, false = inactive.

        public FlashTextBox()
        {
            InitializeComponent();
        }

        public FlashTextBox(Font font, Color colorBackgroundOn, Color colorBackgroundOff, Color colorForegroundOn, Color colorForegroundOff)
        {
            InitializeComponent();

            this.Font = font;
            this.BackColor = colorBackgroundOn;
            this.colorBackgroundOn = colorBackgroundOn;
            this.colorBackgroundOff = colorBackgroundOff;
            this.colorForegroundOn = colorForegroundOn;
            this.colorForegroundOff = colorForegroundOff;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            // TODO: Add custom paint code here

            // Calling the base class OnPaint
            base.OnPaint(pe);
        }

        [Browsable(true), CategoryAttribute("Appearance"),
        Description("Enable textbox flashing, select interval with standard / blip mode"), System.ComponentModel.RefreshProperties(RefreshProperties.Repaint)]

        public void FlasherTextBoxStart(FlashIntervalSpeed SelectFlashMode)
        {
            switch (SelectFlashMode)
            {
                case FlashIntervalSpeed.Slow:
                    iFlashPeriodON = m_iFlashIntervalSlow / 2;
                    iFlashPeriodOFF = iFlashPeriodON;
                    break;
                case FlashIntervalSpeed.Mid:
                    iFlashPeriodON = m_iFlashIntervalMid / 2;
                    iFlashPeriodOFF = iFlashPeriodON;
                    break;
                case FlashIntervalSpeed.Fast:
                    iFlashPeriodON = m_iFlashIntervalFast / 2;
                    iFlashPeriodOFF = iFlashPeriodON;
                    break;
                case FlashIntervalSpeed.BlipSlow:
                    iFlashPeriodON = m_iFlashIntervalBlipOn;
                    iFlashPeriodOFF = m_iFlashIntervalSlow - m_iFlashIntervalBlipOn;
                    break;
                case FlashIntervalSpeed.BlipMid:
                    iFlashPeriodON = m_iFlashIntervalBlipOn;
                    iFlashPeriodOFF = m_iFlashIntervalMid - m_iFlashIntervalBlipOn;
                    break;
                case FlashIntervalSpeed.BlipFast:
                    iFlashPeriodON = m_iFlashIntervalBlipOn;
                    iFlashPeriodOFF = m_iFlashIntervalFast - m_iFlashIntervalBlipOn;
                    break;
                default:
                    return;     // incorrect entry... ignore command.
            }
            if (m_bIsFlashEnabled == false)
            {
                m_bIsFlashEnabled = true;
                timer = new Timer();
                timer.Interval = iFlashPeriodON;
                base.BackColor = colorBackgroundOn;
                base.ForeColor = colorForegroundOn;
                timer.Tick += new EventHandler(TimerOnTick);
                timer.Start();
            }
        }
        [Description("Disable textbox flashing")]
        [Category("Layout")]
        [Browsable(true)]
        public void FlasherTextBoxStop()
        {
            if (timer != null)
            {
                base.BackColor = colorBackgroundOff;
                base.ForeColor = colorForegroundOff;

                timer.Stop();
                timer.Dispose();
            }
            m_bIsFlashEnabled = false;
        }
        [Description("Reset textbox")]
        [Category("Layout")]
        [Browsable(true)]
        public void FlasherTextBoxReset()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
            }
            base.BackColor = colorBackgroundOff;
            base.ForeColor = colorForegroundOff;
            m_bIsFlashEnabled = false;
        }

        protected void TimerOnTick(object obj, EventArgs e)
        {
            if (base.BackColor == colorBackgroundOff)
            {
                base.BackColor = colorBackgroundOn;
                base.ForeColor = colorForegroundOn;
                timer.Interval = iFlashPeriodON;
            }
            else
            {
                base.BackColor = colorBackgroundOff;
                base.ForeColor = colorForegroundOff;
                timer.Interval = iFlashPeriodOFF;
            }
            this.Invalidate();
        }

    }
}
