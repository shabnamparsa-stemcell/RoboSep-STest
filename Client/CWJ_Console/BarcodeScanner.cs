using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Invetech.ApplicationLog;

using GUI_Controls;


namespace GUI_Console
{


    public class BarcodeScanner
    {
        public bool Initialized = false;

        public BarcodeScanner()
        {
            Initialized = false;
        }

        public int Init()
        {
            // if success in opening the COM ports and activating scanner, return value = 1
            // otherwise, return value = -1
            // (Note: on error, the server logs the event, but do not halt program operation).
            int nRet = RoboSep_UserConsole.getInstance().InitScanVialBarcode(false);
            if (nRet == -1)
            {
                // prompt user that barcode scanner initialization failed
                IniFile LanguageINI = RoboSep_UserConsole.getInstance().LanguageINI;
                string sMSG = LanguageINI.GetString("BarcodeInitFailed");
                RoboMessagePanel prompt = new RoboMessagePanel(RoboSep_UserConsole.getInstance(), MessageIcon.MBICON_ERROR, sMSG,
                    LanguageINI.GetString("headerBarcodeInit"), LanguageINI.GetString("Ok"));
                RoboSep_UserConsole.showOverlay();
                prompt.ShowDialog();
                prompt.Dispose();
                RoboSep_UserConsole.hideOverlay();
            }
            else if (nRet == 1)
            {
                string logMSG = "Barcode scanner initialization OK.";

                LogFile.AddMessage(System.Diagnostics.TraceLevel.Info, logMSG);
                Initialized = true;
            }
            return nRet;
        }

        public int FreeaxisBarcode()
        {
            return RoboSep_UserConsole.getInstance().InitScanVialBarcode(true);
        }

        public bool AbortScanVialBarcode()
        {
            return RoboSep_UserConsole.getInstance().AbortScanVialBarcode();
        }

        public bool IsBarcodeScanningDone()
        {
            return RoboSep_UserConsole.getInstance().IsBarcodeScanningDone();
        }

        public int ScanVialBarcodeAt(int Quadrant, int Vial, out bool bError)
        {
            // Quadrant:
            //          -1 : all barcodes in every quadrant/vial are initialized to “-“
            //           0 : all quadrants are selected (1,2,3,4)
            //          1-4: only selected quadrant is selected. 
            //
            // Vial:
            //          -1 : all barcodes in selected quadrant are initialized to “-“
            //           0 : all vials in selected quadrat are selected (1,2,3)
            //          1-3: only selected vial is selected
            //
            // Return:
            //            0: on success
            //            Quadrant:  |   4   |    3   |   2    |   1   |
            //            Vial:      00000WWW|00000XXX|00000YYY|00000ZZZ    eg. WWW, left to right => Vial 3, 2, 1 
            //            The vials that fail: if any barcode cannot be properly read. 
            //  (Note: expects integer values as input, does not check for other data types).

            bError = true;

            // Check input quadrant range
            if (Quadrant < -1 || Quadrant > 4)
                return 0;

            // Check input vial range
            if (Vial < -1 || Vial > 3)
                return 0;

            if (!Initialized)
                Init();

            if (!Initialized)
                return 0;

            bError = false;

            return RoboSep_UserConsole.getInstance().ScanVialBarcodeAt(Quadrant, Vial);
        }

        public string GetVialBarcodeAt(int Quadrant, int Vial)
        {
            // Quadrant:
            //          1-4: only selected quadrant is selected. 
            //
            // Vial:
            //          1-3: only selected vial is selected
            //
            // Return:
            //          the barcode (character string) of the quadrant and vial = barcode read on susccess
            //          "DEVICE_IO_ERROR" : if device error in communication
            //          "EMULATOR_DEFAULT" if in emulator mode
            //          "- " if barcode has not been scanned by ScanVialBarcodeAt
            //          " " if barcode cannot be read after 3 retires
            //  (Note: expects integer values as input, does not check for other data types)
            //  (Currently no error checking is employed for the input range, will come later)

            // Check input quadrant range
            if (Quadrant < 1 || Quadrant > 4)
                return null;

            // Check input vial range
            if (Vial < 1 || Vial > 3)
                return null;

            if (!Initialized)
                return null;

            return RoboSep_UserConsole.getInstance().GetVialBarcodeAt(Quadrant, Vial);
        }
    }
}
