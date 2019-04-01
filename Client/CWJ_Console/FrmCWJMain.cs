using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;

using Invetech.ApplicationLog;

using Tesla.Common.ResourceManagement;
using Tesla.Common.DrawingUtilities;
using Tesla.Common.Separator;
using Tesla.Common.OperatorConsole;
using Tesla.Separator;
using Tesla.OperatorConsoleControls;

namespace CWJ_Console
{

    public partial class FrmCWJMain : Form
    {
        private SeparatorGateway            mySeparatorGateway;
        private CultureInfo                 myCultureInfo;
        private Thread                      mySeparatorConnectionThread;
        public  ISeparator                  mySeparator = null;
        public  SeparatorEventSink          myEventSink = null;

        private string m_sInsVersion = "";
        private string m_sInsSerial     = "";
        private string m_sInsAddress    = "";
        private string m_sGatewayURL    = "";
        private string m_sGatewayVer    = "";
        private string m_sSvrUptime     = "";

        private delegate void   INSTINFOUPDATE (string[] info);
        private INSTINFOUPDATE  m_FInstInfoUpdate = null;
        private void UpdateInfo(string[] info)
        {
            m_sInsVersion = info[0];
            m_sInsSerial  = info[1];
            m_sInsAddress = info[2];
            m_sGatewayVer = info[3];
            m_sGatewayURL = info[4];
            m_sSvrUptime  = info[5];

            m_Label_GateVer.Text   = m_sGatewayVer;
            m_Label_InsVer.Text    = m_sInsVersion;
            m_Label_InsSerial.Text = m_sInsSerial;
            m_Label_URL.Text       = m_sGatewayURL;
            m_Label_InsAdd.Text    = m_sInsAddress;
            m_Label_SvrTime.Text   = m_sSvrUptime;   

        }

        public FrmCWJMain()
        {
//            CheckForIllegalCrossThreadCalls = false; 
          
            mySeparatorGateway  = SeparatorGateway.GetInstance();
            myCultureInfo       = new CultureInfo("en-US", true);

            InitializeComponent();

            Thread.CurrentThread.CurrentCulture   = myCultureInfo;	// Set culture for formatting
            Thread.CurrentThread.CurrentUICulture = myCultureInfo;	// Set culture for UI resources

            m_Timer.Interval = 500;
            m_Timer.Enabled  = false;
        }

        private void AtReportInstrumentInformation(string gatewayURL,
                    string gatewayInterfaceVersion, string serverUptime_seconds,
                    string instrumentControlVersion, string instrumentSerialNumber,
                    string serviceConnection)
        {
#if true
            string[] info = new string[6];
            
            info[0]  = instrumentControlVersion;
            info[1]  = instrumentSerialNumber;
            info[2]  = serviceConnection;
            info[3]  = gatewayInterfaceVersion;
            info[4]  = gatewayURL;
            info[5]  = serverUptime_seconds;
            bool ans = this.m_Label_GateVer.InvokeRequired;
            if (ans)
            {
                m_FInstInfoUpdate = new INSTINFOUPDATE(UpdateInfo);
                Invoke(m_FInstInfoUpdate, new object[] {info});
            }
            else
            {
                m_Label_GateVer.Text = m_sGatewayVer;
                m_Label_InsVer.Text = m_sInsVersion;
                m_Label_InsSerial.Text = m_sInsSerial;
                m_Label_URL.Text = m_sGatewayURL;
                m_Label_InsAdd.Text = m_sInsAddress;
                m_Label_SvrTime.Text = m_sSvrUptime;               
            }
#else
            m_Label_GateVer.Text = gatewayInterfaceVersion;
            m_Label_InsVer.Text = instrumentControlVersion;
            m_Label_InsSerial.Text = instrumentSerialNumber;
            m_Label_URL.Text = gatewayURL;
            m_Label_InsAdd.Text = serviceConnection;
            m_Label_SvrTime.Text = serverUptime_seconds;             

#endif
        }

        private void AttemptSeparatorConnection()
        {
            try
            {
                // Initialise the Separator connection
                mySeparatorGateway.InitialiseConnection(myCultureInfo);

                mySeparator = SeparatorGateway.GetInstance().ControlApi;
                myEventSink = SeparatorGateway.GetInstance().EventsApi; 

                if (myEventSink != null)
                    myEventSink.ReportInstrumentInformation += new Tesla.Separator.ReportInstrumentInfoDelegate(AtReportInstrumentInformation);

                // Actually connect the UI to the Separator server
                mySeparatorGateway.Connect(true);
            }


            catch (ApplicationException /*aex*/)
            {
                // Allow application exceptions to propagate upwards.  That is, if the 
                // InitialiseConnection times out, we use the generated ApplicationException
                // to trigger application shutdown.
            }
            catch (Exception ex)
            {
                LogFile.LogException(System.Diagnostics.TraceLevel.Error, ex);
            }
        }

        private void FrmCWJMain_Shown(object sender, EventArgs e)
        {
            m_Label_Main.Text       = "RoboSep Start!";
            m_Timer.Enabled         = true;
            m_Label_InsVer.Text     = "";
            m_Label_InsSerial.Text  = "";
            m_Label_InsAdd.Text     = "";
            m_Label_GateVer.Text    = "";
            m_Label_URL.Text        = "";
            m_Label_SvrTime.Text    = "";

        }

        private void FrmCWJMain_Load(object sender, EventArgs e)
        {

            mySeparatorConnectionThread = new Thread(new ThreadStart(this.AttemptSeparatorConnection));
            mySeparatorConnectionThread.IsBackground = true;
            mySeparatorConnectionThread.Start();
        }

        private void m_Timer_Tick(object sender, EventArgs e)
        {
            if (mySeparatorGateway.IsInstrumentInitialised == true)
            {
                m_Timer.Enabled = false;
                m_Label_Main.Text = "RoboSep Ready to use!";
                /*
                m_Label_InsVer.Text     = m_sInsVersion;
                m_Label_InsSerial.Text  = m_sInsSerial;
                m_Label_InsAdd.Text     = m_sInsAddress;
                m_Label_GateVer.Text    = m_sGatewayVer;
                m_Label_URL.Text        = m_sGatewayURL;
                m_Label_SvrTime.Text    = m_sSvrUptime;
                */
            }
            else
            {
                m_Label_Main.Text = "RoboSep Initializing....";
            }
        }

        private void m_Button_Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void m_Label_SvrTime_VisibleChanged(object sender, EventArgs e)
        {
            m_Label_Main.Text = "RoboSep Initializing....";
            bool ans = this.m_Label_GateVer.InvokeRequired;   
        }

        private void m_Button_Scan_Click(object sender, EventArgs e)
        {
            string ans = "";

            mySeparator.InitScanVialBarcode(false);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    mySeparator.ScanVialBarcodeAt(i, j);
                    ans = mySeparator.GetVialBarcodeAt(i, j);
                    System.Diagnostics.Debug.WriteLine("Barcode " + ans);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool ans;
            ans = mySeparator.GetIgnoreLidSensor();
            if (ans == true)
                System.Diagnostics.Debug.WriteLine("Lid Sensor : True");
            else
                System.Diagnostics.Debug.WriteLine("Lid Sensor : False");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int ans;
            ans = mySeparator.SetIgnoreLidSensor(1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int ans;
            ans = mySeparator.SetIgnoreLidSensor(0);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            bool ans;
            ans = mySeparator.IsLidClosed();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int id, seq, sec;

            id  = mySeparator.GetCurrentID();
            seq = mySeparator.GetCurrentSeq();
            sec = mySeparator.GetCurrentSec();
        }
    }
}
