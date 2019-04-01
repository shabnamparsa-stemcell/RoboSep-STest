using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

//for test file generation
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Serialization;
using GUI_Console;
using System.Configuration;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Collections;

namespace Tesla.Service
{
    public partial class ServiceLogin : Form
    {
        GUI_Controls.Keyboard touchKeyboard;
        string[] Passwords;
        
        public static string STANDARD_USER = "StandardUser";
        public static string MAINTENANCE_USER = "MaintenanceUser";
        public static string SUPER_USER = "SuperUser";

        public static string VERSION = "v1.0.0.9";//"v4.6.0.12";
        //replace AssemblyVersion("4.7.4")
        //replace AssemblyFileVersion("4.7.4")

        public static string rsaKey = "<RSAKeyValue><Modulus>yXCnyqwaxlf9hwy62VF3leHhjS7I1QKp5dlQTtSDM4rSSyyDfseXTHgB1c3Nwb3h</Modulus><Exponent>AQAB</Exponent><P>/+n6AIKzciaOLbxO3W9fWSa1GKiQxJf3</P><Q>yYH9rx44N1CExe02hRx+sZQCoaSw7NLn</Q><DP>TSEPoCfEPZsxLseaXVK7wfrQieYD+7xx</DP><DQ>h8X8pofYHP012R7yQ1Jl00UFWODdDVU1</DQ><InverseQ>djPYkO3Tb4rLvfmENfX8n0OzVW/QIop2</InverseQ><D>Rw3q0c2lYCM3dYSi//cBlKfplJBVHPXj8KsNYJKlOHpAuxCkq7bRYQPw+QR942A1</D></RSAKeyValue>";
        public static int rsaKeysize = 384;
        //string test = @"GCclxhIcTKxDbsUv7ktUxKpyclF6o28bQ8SDwtjj31/GBUZ+4BvyUy2F0RxIsIK+VcEUjtywqr9fLauLbPZm82hRckqNY3y33ujYwb0hhxmHPOoMGoHJVD2nrqRA9/gV7BXXvVrQy+8pIF9LzbQ4WWzoSeiS3yjURD3RP00BcNUuXb4mZ3z3NL0HfXixTUBvWjfIm2Q/aSX9lL2tk/yxrpU8oG6baGqI5aCGKshO1Jwi2+zlGZVxFhY1RZdC0U4JH4hyZLgAETpo+za3bljRMtophfS7CUmf+b4MS331HstI/8zqz3Co7glHZjJ2L62pWlD6DXWUmsHjy5gKNEd3CgmjGC0tUH6k99wiwJEmItK5s0HvufOaeq6PtSmbK752";
        //string encString = EncryptString("Test", keysize, key);
        //string decString = DecryptString(test, keysize, key);

        public ServiceLogin()
        {
            InitializeComponent();

            string logMSG = "Creating new ServiceLogin";
            GUI_Controls.uiLog.SERVICE_LOG(this, "ServiceLogin", GUI_Controls.uiLog.LogLevel.EVENTS, logMSG);

            //btnTest.Text = "Protocol List";
            //btnTest.Role = OperatorConsoleControls.RoboSepButton.ButtonRole.Warning;
            //btnTest.Enabled = false;

            label1.Text = VERSION;
            this.TopMost = true;
            textBox_UserName.Text = "user";
            textBox_servicePassword.Text = "";
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            new ServiceDialog(sender.ToString()).ShowDialog();
        }

        //for generating test config file with original scripts
        private void button1_Click(object sender, EventArgs e)
        {
            /*
            RoboSepService service = new RoboSepService();
            service.calibrationScripts = new calibrationScripts();
            List<script> scripts = new List<script>();
            List<command> commands = new List<command>();

            script s = new script();
            command c = new command();
            commands.Clear();
            s.name = "Theta Axis";
            #region Theta Axis
            c.IsMessageOnly = true;
            c.Description = "Locate the calibration jig on quadrant 1 of the carousel.";
            commands.Add(c);
            c = new command();
            c.Description = "Enter calibration mode.";
            c.ServerCommand = "Subsystem: Calibration startCalibration;";
            commands.Add(c);
            c = new command();
            c.Description = "Home all axes.";
            c.ServerCommand = "Subsystem: Instrument HomeAll 0;";
            commands.Add(c);
            c = new command();
            c.Description = "Locate the robot above the outer radius etch line on the calibration jig.";
            c.ServerCommand = "Subsystem: TeslaPlatform MoveTo 1 'antibodyvial_degrees' 135;";
            commands.Add(c);
            c = new command();
            c.Description = "Turn off power to the carousel.";
            c.ServerCommand = "Subsystem: TeslaPlatform powerDownCarousel;";
            commands.Add(c);
            
            c = new command();
            c.Description = "Set Robot_ZAxis defaults";
            c.AxisName = "Robot_ZAxis";
            c.AxisCoarse = 2;
            c.AxisFine = 0.5;
            c.AxisUnits = "mm";
            commands.Add(c);
            c = new command();
            c.Description = "Set Robot_ThetaAxis defaults";
            c.AxisName = "Robot_ThetaAxis";
            c.AxisCoarse = 2;
            c.AxisFine = 0.25;
            c.AxisUnits = "deg";
            commands.Add(c);

            c = new command();
            c.Description = "Adjust the Z axis so that the tip is 1 mm above the calibration jig using the 'Manual Axis Controls'." +
                            "Move the theta axis using the 'Manual Axis Controls' so that robot tip is centred directly above the #1 (outer) radius etch line on the calibration jig. You may move the carousel by hand to help confirm the alignment.";
            c.IsManualStep = true;
            commands.Add(c);
            c = new command();
            c.Description = "Store the current theta angle to temp var m_reagentVialAngle";
            c.ServerCommand = "Subsystem: Calibration calibrateReagentVialAngle;";
            commands.Add(c);
            c = new command();
            c.Description = "Move the theta axis using the 'Manual Axis Controls' so that robot tip is centred directly above the #2 radius etch line on the calibration jig. You may move the carousel by hand to help confirm the alignment.";
            c.IsManualStep = true;
            commands.Add(c);
            c = new command();
            c.Description = "Store the current theta angle to temp var m_sampleVialAngle";
            c.ServerCommand = "Subsystem: Calibration calibrateSampleVialAngle;";
            commands.Add(c);
            c = new command();
            c.Description = "Move the theta axis using the 'Manual Axis Controls' so that robot tip is centred directly above the #3 radius etch line on the calibration jig. You may move the carousel by hand to help confirm the alignment.";
            c.IsManualStep = true;
            commands.Add(c);
            c = new command();
            c.Description = "Store the current theta angle to temp var m_5mlTipAngle";
            c.ServerCommand = "Subsystem: Calibration calibrate5mlTipAngle;";
            commands.Add(c);
            c = new command();
            c.Description = "Move the theta axis using the 'Manual Axis Controls' so that robot tip is centred directly above the #4 radius etch line on the calibration jig. You may move the carousel by hand to help confirm the alignment.";
            c.IsManualStep = true;
            commands.Add(c);
            c = new command();
            c.Description = "Store the current theta angle to temp var m_1mlTipAngle";
            c.ServerCommand = "Subsystem: Calibration calibrate1mlTipAngle;";
            commands.Add(c);
            c = new command();
            c.Description = "Move the theta axis using the 'Manual Axis Controls' so that robot tip is centred directly above the #5 radius etch line on the calibration jig. You may move the carousel by hand to help confirm the alignment.";
            c.IsManualStep = true;
            commands.Add(c);
            c = new command();
            c.Description = "Store the current theta angle to temp var m_lysisVialAngle";
            c.ServerCommand = "Subsystem: Calibration calibrateLysisVialAngle;";
            commands.Add(c);
            c = new command();
            c.Description = "Move the theta axis using the 'Manual Axis Controls' so that robot tip is centred directly above the #6 radius etch line on the calibration jig. You may move the carousel by hand to help confirm the alignment.";          
            c.IsManualStep = true;
            commands.Add(c);
            c = new command();
            c.Description = "Store the current theta angle to temp var m_separationVialAngle";
            c.ServerCommand = "Subsystem: Calibration calibrateSeparationVialAngle;";
            commands.Add(c);

            c = new command();
            c.Description = "Set Robot_ThetaAxis defaults";
            c.AxisName = "Robot_ThetaAxis";
            c.AxisCoarse = 5;
            c.AxisFine = 1;
            c.AxisUnits = "deg";
            commands.Add(c);
            c = new command();
            c.Description = "Move the theta axis using the 'Manual Axis Controls' so that robot tip is centred directly above the #7 (inner) radius etch line on the calibration jig. You may move the carousel by hand to help confirm the alignment.";
            c.IsManualStep = true;
            commands.Add(c);
            c = new command();
            c.Description = "Store the current theta angle to temp var m_wasteVialAngle";
            c.ServerCommand = "Subsystem: Calibration calibrateWasteVialAngle;";
            commands.Add(c);
            c = new command();
            c.Description = "Resets the carousel power back to the default.";
            c.ServerCommand = "Device: Carousel ResetPower;";
            commands.Add(c);
            c = new command();
            c.Description = "Home all axes.";
            c.ServerCommand = "Subsystem: Instrument HomeAll 0;";
            commands.Add(c);
            c = new command();
            c.Description = "Exit calibration mode. NOTE: This will save the new calibration settings to file. The new calibration will not take effect until the instrument is restarted.";
            c.ServerCommand = "Subsystem: Calibration endCalibration;";
            commands.Add(c);
            c = new command();
            c.Description = "Reset Instrument";
            c.ServerCommand = "Centre reInitInstrument;";
            commands.Add(c);
           
            s.command = commands.ToArray();
            scripts.Add(s);
            #endregion

            s = new script();
            c = new command();
            commands.Clear();
            s.name = "Carousel";
            #region Carousel
            c.IsMessageOnly = true;
            c.Description = "Locate the calibration jig on quadrant 1 of the carousel. Ensure that the theta axis is calibrated before commencing calibration of the carousel.";
            commands.Add(c);
            c = new command();
            c.Description = "Enter calibration mode.";
            c.ServerCommand = "Subsystem: Calibration startCalibration;";
            commands.Add(c);
            c = new command();
            c.Description = "Home all axes.";
            c.ServerCommand = "Subsystem: Instrument HomeAll 0;";
            commands.Add(c);
            c = new command();
            c.Description = "Locate the robot tip head above the carousel calibration slot.";
            c.ServerCommand = "Subsystem: TeslaPlatform MoveTo 1 'calibrationjigslot_degrees' 135;";
            commands.Add(c);
            
            c = new command();
            c.Description = "Set Robot_ZAxis defaults";
            c.AxisName = "Robot_ZAxis";
            c.AxisCoarse = 5;
            c.AxisFine = 1;
            c.AxisUnits = "mm";
            commands.Add(c);
            c = new command();
            c.Description = "Set Robot_ThetaAxis defaults";
            c.AxisName = "Robot_ThetaAxis";
            c.AxisCoarse = 2.5;
            c.AxisFine = 0.5;
            c.AxisUnits = "deg";
            commands.Add(c);
            c = new command();
            c.Description = "Set Carousel defaults";
            c.AxisName = "Carousel";
            c.AxisCoarse = 2.5;
            c.AxisFine = 0.25;
            c.AxisUnits = "deg";
            commands.Add(c);

	
            c = new command();
            c.Description = "Move the theta axis using the 'Manual Axis Controls' so that robot tip is centred directly above the slot on the calibration jig." +
"Adjust the Z axis so that the tip is 2 mm lower than the lower face of the calibration jig top plate using the 'Manual Axis Controls'." +
"Rotate the carousel CLOCKWISE (negative direction) using the 'Manual Axis Controls' so that the tip head kisses the end of the calibration slot.";
            c.IsManualStep = true;
            commands.Add(c);
            
            c = new command();
            c.Description = "Store the current carousel angle to temp var m_jigCarouselAngle1";
            c.ServerCommand = "Subsystem: Calibration calibrateCarouselAngle1;";
            commands.Add(c);
            c = new command();
            c.Description = "Move the carousel to the other end of the calibration slot.";
            c.ServerCommand = "Device: Carousel IncrementTheta 18;";
            commands.Add(c);
            c = new command();
            c.Description = "Rotate the carousel ANTI-CLOCKWISE (positive direction) anti-clockwise using the 'Manual Axis Controls' so that the tip head kisses the end of the calibration slot.";
            c.IsManualStep = true;
            commands.Add(c);
            c = new command();
            c.Description = "Store the current carousel angle to temp var m_jigCarouselAngle2";
            c.ServerCommand = "Subsystem: Calibration calibrateCarouselAngle2;";
            commands.Add(c);
            	
            c = new command();
            c.Description = "Home all axes.";
            c.ServerCommand = "Subsystem: Instrument HomeAll 0;";
            commands.Add(c);
            c = new command();
            c.Description = "Exit calibration mode. NOTE: This will save the new calibration settings to file. The new calibration will not take effect until the instrument is restarted.";
            c.ServerCommand = "Subsystem: Calibration endCalibration;";
            commands.Add(c);
            c = new command();
            c.Description = "Reset Instrument";
            c.ServerCommand = "Centre reInitInstrument;";
            commands.Add(c);

            s.command = commands.ToArray();
            scripts.Add(s);
            #endregion

            
            s = new script();
            c = new command();
            commands.Clear();
            s.name = "Z Axis";
            #region Z Axis
            c.IsMessageOnly = true;
            c.Description = "Locate the calibration jig on quadrant 1 of the carousel. Ensure that the theta axis and carousel are calibrated before commencing calibration of the Z axis.";
            commands.Add(c);
            c = new command();
            c.Description = "Enter calibration mode.";
            c.ServerCommand = "Subsystem: Calibration startCalibration;";
            commands.Add(c);
            c = new command();
            c.Description = "Home all axes.";
            c.ServerCommand = "Subsystem: Instrument HomeAll 0;";
            commands.Add(c);
            c = new command();
            c.Description = "Rotate the robot theta axis and the carousel to above the Z calibration feature.";
            c.ServerCommand = "Subsystem: TeslaPlatform MoveTo 1 'calibrationjigzaxisfeature_degrees' 130;";
            commands.Add(c);
            
            c = new command();
            c.Description = "Set Robot_ZAxis defaults";
            c.AxisName = "Robot_ZAxis";
            c.AxisCoarse = 2.5;
            c.AxisFine = 0.5;
            c.AxisUnits = "mm";
            commands.Add(c);
            c = new command();
            c.Description = "Set Robot_ThetaAxis defaults";
            c.AxisName = "Robot_ThetaAxis";
            c.AxisCoarse = 2;
            c.AxisFine = 0.5;
            c.AxisUnits = "deg";
            commands.Add(c);
            c = new command();
            c.Description = "Set Carousel defaults";
            c.AxisName = "Carousel";
            c.AxisCoarse = 2;
            c.AxisFine = 0.15;
            c.AxisUnits = "deg";
            commands.Add(c);

	
            c = new command();
            c.Description = "Adjust the theta axis using the 'Manual Axis Controls' so that robot tip is centred directly above the Z calibration feature."+
"Rotate the carousel using the 'Manual Axis Controls' so that robot tip is centred directly above the Z calibration feature."+
"Adjust the Z axis so that the lower face of the tip head is now level with the top face of the Z axis calibration feature in the calibration jig using the 'Manual Axis Controls'.";
           c.IsManualStep = true;
            commands.Add(c);
            
            c = new command();
            c.Description = "Stores the current z height to temp var m_jigZHeight";
            c.ServerCommand = "Subsystem: Calibration calibrateZHeights;";
            commands.Add(c);
            	
            c = new command();
            c.Description = "Home all axes.";
            c.ServerCommand = "Subsystem: Instrument HomeAll 0;";
            commands.Add(c);
            c = new command();
            c.Description = "Exit calibration mode. NOTE: This will save the new calibration settings to file. The new calibration will not take effect until the instrument is restarted.";
            c.ServerCommand = "Subsystem: Calibration endCalibration;";
            commands.Add(c);
            c = new command();
            c.Description = "Reset Instrument";
            c.ServerCommand = "Centre reInitInstrument;";
            commands.Add(c);

            s.command = commands.ToArray();
            scripts.Add(s);
            #endregion

              
            s = new script();
            c = new command();
            commands.Clear();
            s.name = "Tip Stripper";
            #region Tip Stripper
            c.IsMessageOnly = true;
            c.Description = "Ensure that the theta axis and carousel are calibrated before commencing calibration of the tip stripper.";
            commands.Add(c);
            c = new command();
            c.Description = "Enter calibration mode.";
            c.ServerCommand = "Subsystem: Calibration startCalibration;";
            commands.Add(c);
            c = new command();
            c.Description = "Home all axes.";
            c.ServerCommand = "Subsystem: Instrument HomeAll 0;";
            commands.Add(c);
            c = new command();
            c.Description = "Script moves robot and carousel theta axis to tip 1 strip position.";
            c.ServerCommand = "Subsystem: TeslaPlatform MoveTo 1 'tip_1ml_position1_degrees' 0;";
            commands.Add(c);
            c = new command();
            c.Description = "Engage Tip Stripper";
            c.ServerCommand = "Device: TipStripper Engage;";
            commands.Add(c);
            c = new command();
            c.Description = "Script moves robot down to the strip position";
            c.ServerCommand = "Subsystem: ZThetaRobot SetZPosition 85;";
            commands.Add(c);
            
            c = new command();
            c.Description = "Set Robot_ZAxis defaults";
            c.AxisName = "Robot_ZAxis";
            c.AxisCoarse = 5;
            c.AxisFine = 0.5;
            c.AxisUnits = "mm";
            commands.Add(c);
            c = new command();
            c.Description = "Set Robot_ThetaAxis defaults";
            c.AxisName = "Robot_ThetaAxis";
            c.AxisCoarse = 2;
            c.AxisFine = 0.25;
            c.AxisUnits = "deg";
            commands.Add(c);

	
            c = new command();
            c.Description = "Move the theta axis using the 'Manual Axis Controls' so that the robot tip head is centred within the 1100uL tip stripper jaws."+
"Adjust the Z axis so that the tip head is inside of the tip stripper slot using the 'Manual Axis Controls'. Drive the tip head down as far as possible, then back it off 1 mm."; 
            c.IsManualStep = true;
            commands.Add(c);
            
            c = new command();
            c.Description = "Store the current theta angle to temp var m_1mlTipStripAngle";
            c.ServerCommand = "Subsystem: Calibration calibrate1mlTipStripPosition;";
            commands.Add(c);
            c = new command();
            c.Description = "Script moves robot and carousel theta axis to tip 4 strip position.";
            c.ServerCommand = "Subsystem: TeslaPlatform MoveTo 1 'tip_5ml_position4_degrees' 0;";
            commands.Add(c);
            c = new command();
            c.Description = "Engage Tip Stripper";
            c.ServerCommand = "Device: TipStripper Engage;";
            commands.Add(c);
            c = new command();
            c.Description = "Script moves robot down to the strip position";
            c.ServerCommand = "Subsystem: ZThetaRobot SetZPosition 85;";
            commands.Add(c);
            	
            c = new command();
            c.Description = "Move the theta axis using the 'Manual Axis Controls' so that the robot tip head is centred within the 5000uL tip stripper jaws."+
"Adjust the Z axis so that the tip head is inside of the tip stripper slot using the 'Manual Axis Controls'. Drive the tip head down as far as possible, then back it off 1.5mm."; 
            c.IsManualStep = true;
            commands.Add(c);
            
            c = new command();
            c.Description = "Store the current theta angle to temp var m_5mlTipStripAngle";
            c.ServerCommand = "Subsystem: Calibration calibrate5mlTipStripPosition;";
            commands.Add(c);
            	
	

            c = new command();
            c.Description = "Home all axes.";
            c.ServerCommand = "Subsystem: Instrument HomeAll 0;";
            commands.Add(c);
            c = new command();
            c.Description = "Exit calibration mode. NOTE: This will save the new calibration settings to file. The new calibration will not take effect until the instrument is restarted.";
            c.ServerCommand = "Subsystem: Calibration endCalibration;";
            commands.Add(c);
            c = new command();
            c.Description = "Reset Instrument";
            c.ServerCommand = "Centre reInitInstrument;";
            commands.Add(c);

            s.command = commands.ToArray();
            scripts.Add(s);
            #endregion

            s = new script();
            c = new command();
            commands.Clear();
            s.name = "Adjust Tips";
            #region Adjust Tips
            c.IsMessageOnly = true;
            c.Description = "Load quadrant number 1 with a tip box. Note that any previous instrument calibration changes will not take effect until after the instrument is restarted.";
            commands.Add(c);
            c = new command();
            c.Description = "Enter calibration mode.";
            c.ServerCommand = "Subsystem: Calibration startCalibration;";
            commands.Add(c);
            c = new command();
            c.Description = "Home all axes.";
            c.ServerCommand = "Subsystem: Instrument HomeAll 0;";
            commands.Add(c);
            c = new command();
            c.Description = "Move to just above the tip 1 pickup position";
            c.ServerCommand = "Subsystem: TeslaPlatform MoveTo 1 'tip_1ml_position1_degrees' 124;";
            commands.Add(c);
            
            c = new command();
            c.Description = "Set Robot_ZAxis defaults";
            c.AxisName = "Robot_ZAxis";
            c.AxisCoarse = 1;
            c.AxisFine = 0.2;
            c.AxisUnits = "mm";
            commands.Add(c);
            c = new command();
            c.Description = "Set Robot_ThetaAxis defaults";
            c.AxisName = "Robot_ThetaAxis";
            c.AxisCoarse = 0.4;
            c.AxisFine = 0.2;
            c.AxisUnits = "deg";
            commands.Add(c);
            c = new command();
            c.Description = "Set Carousel defaults";
            c.AxisName = "Carousel";
            c.AxisCoarse = 0.4;
            c.AxisFine = 0.1;
            c.AxisUnits = "deg";
            commands.Add(c);

	
            c = new command();
            c.Description = "Adjust the Z axis so that the tip head is just above the tip using the 'Manual Axis Controls'"+
"Rotate the carousel using the 'Manual Axis Controls' so that robot tip is centred directly above the 1100uL tip (tip 1)."+
"Rotate the robot theta axis using the 'Manual Axis Controls' so that robot tip is centred directly above the 1100uL tip (tip 1).";
            c.IsManualStep = true;
            commands.Add(c);
            
            c = new command();
            c.Description = "Store the new position.";
            c.ServerCommand = "Subsystem: Calibration setCalibrationAngles 'ReferencePoints' 'tip_1ml_position1_degrees';";
            commands.Add(c);

            c = new command();
            c.Description = "Move to just above the tip 2 pickup position";
            c.ServerCommand = "Subsystem: TeslaPlatform MoveTo 1 'tip_1ml_position2_degrees' 124;";
            commands.Add(c);
            c = new command();
            c.Description = "Adjust the Z axis so that the tip head is just above the tip using the 'Manual Axis Controls'"+
"Rotate the carousel using the 'Manual Axis Controls' so that robot tip is centred directly above the 1100uL tip (tip 2)."+
"Rotate the robot theta axis using the 'Manual Axis Controls' so that robot tip is centred directly above the 1100uL tip (tip 2).";
            c.IsManualStep = true;
            commands.Add(c);
            c = new command();
            c.Description = "Store the new position.";
            c.ServerCommand = "Subsystem: Calibration setCalibrationAngles 'ReferencePoints' 'tip_1ml_position2_degrees';";
            commands.Add(c);
            	
            c = new command();
            c.Description = "Move to just above the tip 3 pickup position";
            c.ServerCommand = "Subsystem: TeslaPlatform MoveTo 1 'tip_1ml_position3_degrees' 124;";
            commands.Add(c);
            c = new command();
            c.Description = "Adjust the Z axis so that the tip head is just above the tip using the 'Manual Axis Controls'"+
"Rotate the carousel using the 'Manual Axis Controls' so that robot tip is centred directly above the 1100uL tip (tip 3)."+
"Rotate the robot theta axis using the 'Manual Axis Controls' so that robot tip is centred directly above the 1100uL tip (tip 3).";
            c.IsManualStep = true;
            commands.Add(c);
            c = new command();
            c.Description = "Store the new position.";
            c.ServerCommand = "Subsystem: Calibration setCalibrationAngles 'ReferencePoints' 'tip_1ml_position3_degrees';";
            commands.Add(c);
            
            c = new command();
            c.Description = "Move to just above the tip 4 pickup position";
            c.ServerCommand = "Subsystem: TeslaPlatform MoveTo 1 'tip_5ml_position4_degrees' 119;";
            commands.Add(c);
            c = new command();
            c.Description = "Adjust the Z axis so that the tip head is just above the tip using the 'Manual Axis Controls'"+
"Rotate the carousel using the 'Manual Axis Controls' so that robot tip is centred directly above the 5000uL tip (tip 4)."+
"Rotate the robot theta axis using the 'Manual Axis Controls' so that robot tip is centred directly above the 5000uL tip (tip 4).";
            c.IsManualStep = true;
            commands.Add(c);
            c = new command();
            c.Description = "Store the new position.";
            c.ServerCommand = "Subsystem: Calibration setCalibrationAngles 'ReferencePoints' 'tip_5ml_position4_degrees';";
            commands.Add(c);

            c = new command();
            c.Description = "Move to just above the tip 5 pickup position";
            c.ServerCommand = "Subsystem: TeslaPlatform MoveTo 1 'tip_5ml_position5_degrees' 119;";
            commands.Add(c);
            c = new command();
            c.Description = "Adjust the Z axis so that the tip head is just above the tip using the 'Manual Axis Controls'"+
"Rotate the carousel using the 'Manual Axis Controls' so that robot tip is centred directly above the 5000uL tip (tip 5)."+
"Rotate the robot theta axis using the 'Manual Axis Controls' so that robot tip is centred directly above the 5000uL tip (tip 5).";
            c.IsManualStep = true;
            commands.Add(c);
            c = new command();
            c.Description = "Store the new position.";
            c.ServerCommand = "Subsystem: Calibration setCalibrationAngles 'ReferencePoints' 'tip_5ml_position5_degrees';";
            commands.Add(c);


            
            c = new command();
            c.Description = "Exit calibration mode. NOTE: This will save the new calibration settings to file. The new calibration will not take effect until the instrument is restarted.";
            c.ServerCommand = "Subsystem: Calibration endCalibration;";
            commands.Add(c);
            c = new command();
            c.Description = "Home all axes.";
            c.ServerCommand = "Subsystem: Instrument HomeAll 0;";
            commands.Add(c);
            c = new command();
            c.Description = "Reset Instrument";
            c.ServerCommand = "Centre reInitInstrument;";
            commands.Add(c);

            s.command = commands.ToArray();
            scripts.Add(s);
            #endregion
            
            s = new script();
            c = new command();
            commands.Clear();
            s.name = "BCCM";
            #region BCCM
            c.IsMessageOnly = true;
            c.Description = "Load quadrant number 1 with a tip box. Remove all items from the carousel, including the calibration jig.";
            commands.Add(c);
            c = new command();
            c.Description = "Enter calibration mode.";
            c.ServerCommand = "Subsystem: Calibration startCalibration;";
            commands.Add(c);
            c = new command();
            c.Description = "Home all axes.";
            c.ServerCommand = "Subsystem: Instrument HomeAll 0;";
            commands.Add(c);
            c = new command();
            c.Description = "Pickup tip #4.";
            c.ServerCommand = "Subsystem: TeslaPlatform PickupTip 1 4;";
            commands.Add(c);
            c = new command();
            c.Description = "Script moves robot to above the BCCM bottle";
            c.ServerCommand = "Subsystem: TeslaPlatform MoveTo 0 'bccm_bulkcontainer_degrees' 0;";
            commands.Add(c);

            c = new command();
            c.Description = "Set Robot_ZAxis defaults";
            c.AxisName = "Robot_ZAxis";
            c.AxisCoarse = 10;
            c.AxisFine = 1;
            c.AxisUnits = "mm";
            commands.Add(c);
            c = new command();
            c.Description = "Set Robot_ThetaAxis defaults";
            c.AxisName = "Robot_ThetaAxis";
            c.AxisCoarse = 5;
            c.AxisFine = 0.5;
            c.AxisUnits = "deg";
            commands.Add(c);
	
            c = new command();
            c.Description = "Move the theta axis using the 'Manual Axis Controls' so that the tip is centred above the BCCM bottle."+
"Adjust the Z axis so that the tip is just touching the bottom of the BCCM bottle using the 'Manual Axis Controls'. Drive the tip head down as far as possible, then back off 1mm.";
            c.IsManualStep = true;
            commands.Add(c);
            
            c = new command();
            c.Description = "Store the current theta and Z locations to temp vars m_bccmAngle and m_bccmZHeight";
            c.ServerCommand = "Subsystem: Calibration calibrateBCCMPosition;";
            commands.Add(c);
            
            c = new command();
            c.Description = "Strip the current tip.";
            c.ServerCommand = "Subsystem: TeslaPlatform StripTip;";
            commands.Add(c);
            c = new command();
            c.Description = "Home the Theta axis.";
            c.ServerCommand = "Subsystem: ZThetaRobot HomeTheta;";
            commands.Add(c);
            		
            c = new command();
            c.Description = "Home all axes.";
            c.ServerCommand = "Subsystem: Instrument HomeAll 0;";
            commands.Add(c);
            c = new command();
            c.Description = "Exit calibration mode. NOTE: This will save the new calibration settings to file. The new calibration will not take effect until the instrument is restarted.";
            c.ServerCommand = "Subsystem: Calibration endCalibration;";
            commands.Add(c);
            c = new command();
            c.Description = "Reset Instrument";
            c.ServerCommand = "Centre reInitInstrument;";
            commands.Add(c);

            s.command = commands.ToArray();
            scripts.Add(s);
            #endregion

            
            s = new script();
            c = new command();
            commands.Clear();
            s.name = "Adjust Vials";
            #region Adjust Vials
            c.IsMessageOnly = true;
            c.Description = "Load quadrant number 1 with a tip box and all vials. Note that any previous instrument calibration changes will not take effect until after the instrument is restarted. If you have adjusted the tip pickup positions, it is recommended that you restart the instrument.";
            commands.Add(c);
            c = new command();
            c.Description = "Enter calibration mode.";
            c.ServerCommand = "Subsystem: Calibration startCalibration;";
            commands.Add(c);
            c = new command();
            c.Description = "Home all axes.";
            c.ServerCommand = "Subsystem: Instrument HomeAll 0;";
            commands.Add(c);
            c = new command();
            c.Description = "Pickup tip #3.";
            c.ServerCommand = "Subsystem: TeslaPlatform PickupTip 1 3;";
            commands.Add(c);
            c = new command();
            c.Description = "Move to the antibody vial.";
            c.ServerCommand = "Subsystem: Instrument setContainerVolume 1 'AntibodyVial' 100;Subsystem: Instrument MoveToAspirate self.ContainerAt(1,'AntibodyVial') 100;";
            commands.Add(c);

            	

            c = new command();
            c.Description = "Set Robot_ThetaAxis defaults";
            c.AxisName = "Robot_ThetaAxis";
            c.AxisCoarse = 0.4;
            c.AxisFine = 0.2;
            c.AxisUnits = "deg";
            commands.Add(c);
            c = new command();
            c.Description = "Set Carousel defaults";
            c.AxisName = "Carousel";
            c.AxisCoarse = 0.4;
            c.AxisFine = 0.1;
            c.AxisUnits = "deg";
            commands.Add(c);

	
            c = new command();
            c.Description = "Rotate the carousel using the 'Manual Axis Controls' so that robot tip is centred in the carousel hole."+
"Rotate the robot theta axis using the 'Manual Axis Controls' so that robot tip is centred in the carousel hole.";
            c.IsManualStep = true;
            commands.Add(c);
            
            c = new command();
            c.Description = "Store the new position.";
            c.ServerCommand = "Subsystem: Calibration setCalibrationAngles 'ReferencePoints' 'antibodyvial_degrees';";
            commands.Add(c);
            c = new command();
            c.Description = "Move to the cocktail vial.";
            c.ServerCommand = "Subsystem: Instrument setContainerVolume 1 'CocktailVial' 100;Subsystem: Instrument MoveToAspirate self.ContainerAt(1,'CocktailVial') 100;";
            commands.Add(c);

            
            c = new command();
            c.Description = "Rotate the carousel using the 'Manual Axis Controls' so that robot tip is centred in the carousel hole."+
"Rotate the robot theta axis using the 'Manual Axis Controls' so that robot tip is centred in the carousel hole.";
            c.IsManualStep = true;
            commands.Add(c);
            
            c = new command();
            c.Description = "Store the new position.";
            c.ServerCommand = "Subsystem: Calibration setCalibrationAngles 'ReferencePoints' 'cocktailvial_degrees';";
            commands.Add(c);
            c = new command();
            c.Description = "Move to the bead vial.";
            c.ServerCommand = "Subsystem: Instrument setContainerVolume 1 'ParticleVial' 100;Subsystem: Instrument MoveToAspirate self.ContainerAt(1,'ParticleVial') 100;";
            commands.Add(c);

            c = new command();
            c.Description = "Rotate the carousel using the 'Manual Axis Controls' so that robot tip is centred in the carousel hole."+
"Rotate the robot theta axis using the 'Manual Axis Controls' so that robot tip is centred in the carousel hole.";
            c.IsManualStep = true;
            commands.Add(c);
            
            c = new command();
            c.Description = "Store the new position.";
            c.ServerCommand = "Subsystem: Calibration setCalibrationAngles 'ReferencePoints' 'particlevial_degrees';";
            commands.Add(c);
            c = new command();
            c.Description = "Strip the current tip.";
            c.ServerCommand = "Subsystem: TeslaPlatform StripTip;";
            commands.Add(c);
            c = new command();
            c.Description = "Pickup tip #5.";
            c.ServerCommand = "Subsystem: TeslaPlatform PickupTip 1 5;";
            commands.Add(c);
            c = new command();
            c.Description = "Move to the sample vial.";
            c.ServerCommand = "Subsystem: Instrument setContainerVolume 1 'SampleVial' 100;Subsystem: Instrument MoveToAspirate self.ContainerAt(1,'SampleVial') 100;";
            commands.Add(c);

            c = new command();
            c.Description = "Rotate the carousel using the 'Manual Axis Controls' so that robot tip is centred in the carousel hole."+
"Rotate the robot theta axis using the 'Manual Axis Controls' so that robot tip is centred in the carousel hole.";
            c.IsManualStep = true;
            commands.Add(c);
            
            c = new command();
            c.Description = "Store the new position.";
            c.ServerCommand = "Subsystem: Calibration setCalibrationAngles 'ReferencePoints' 'samplevial_14ml_degrees';";
            commands.Add(c);
            c = new command();
            c.Description = "Move to the supernatent vial.";
            c.ServerCommand = "Subsystem: Instrument setContainerVolume 1 'SeparationVial' 100;Subsystem: Instrument MoveToAspirate self.ContainerAt(1,'SeparationVial') 100;";
            commands.Add(c);

            c = new command();
            c.Description = "Rotate the carousel using the 'Manual Axis Controls' so that robot tip is centred in the carousel hole."+
"Rotate the robot theta axis using the 'Manual Axis Controls' so that robot tip is centred in the carousel hole.";
            c.IsManualStep = true;
            commands.Add(c);
            
            c = new command();
            c.Description = "Store the new position.";
            c.ServerCommand = "Subsystem: Calibration setCalibrationAngles 'ReferencePoints' 'separationvial_14ml_degrees';";
            commands.Add(c);
            c = new command();
            c.Description = "Strip the current tip.";
            c.ServerCommand = "Subsystem: TeslaPlatform StripTip;";
            commands.Add(c);
            
            
            c = new command();
            c.Description = "Exit calibration mode. NOTE: This will save the new calibration settings to file. The new calibration will not take effect until the instrument is restarted.";
            c.ServerCommand = "Subsystem: Calibration endCalibration;";
            commands.Add(c);
            c = new command();
            c.Description = "Home all axes.";
            c.ServerCommand = "Subsystem: Instrument HomeAll 0;";
            commands.Add(c);
            c = new command();
            c.Description = "Reset Instrument";
            c.ServerCommand = "Centre reInitInstrument;";
            commands.Add(c);

            s.command = commands.ToArray();
            scripts.Add(s);
            #endregion

            
            s = new script();
            c = new command();
            commands.Clear();
            s.name = "Adjust Z-Cal";
            #region Adjust Z-Cal
            c.IsMessageOnly = true;
            c.Description = "Fully load quadrant number 1 with all empty reagents, consumables and a tip box. Note that instrument calibration changes will not take effect until after the instrument is restarted.";
            commands.Add(c);
            c = new command();
            c.Description = "Enter calibration mode.";
            c.ServerCommand = "Subsystem: Calibration startCalibration;";
            commands.Add(c);
            c = new command();
            c.Description = "Home all axes.";
            c.ServerCommand = "Subsystem: Instrument HomeAll 0;";
            commands.Add(c);
            c = new command();
            c.Description = "Pickup tip #2.";
            c.ServerCommand = "Subsystem: TeslaPlatform PickupTip 1 2;";
            commands.Add(c);
            c = new command();
            c.Description = "Move to the cocktail vial.";
            c.ServerCommand = "Subsystem: Instrument setContainerVolume 1 'CocktailVial' 100;Subsystem: Instrument MoveToAspirate self.ContainerAt(1,'CocktailVial') 100;";
            commands.Add(c);
            
            c = new command();
            c.Description = "Set Robot_ZAxis defaults";
            c.AxisName = "Robot_ZAxis";
            c.AxisCoarse = 1;
            c.AxisFine = 0.2;
            c.AxisUnits = "mm";
            commands.Add(c);

	
            c = new command();
            c.Description = "Adjust the Z axis so that the tip is down as far as possible using the 'Manual Axis Controls', then back it off 1 mm.";
            c.IsManualStep = true;
            commands.Add(c);
            
            c = new command();
            c.Description = "Stores tip height for small vials";
            c.ServerCommand = "Subsystem: Calibration calibrateSetSmallVialTipHeight;";
            commands.Add(c);
            c = new command();
            c.Description = "Strip the current tip.";
            c.ServerCommand = "Subsystem: TeslaPlatform StripTip;";
            commands.Add(c);
            c = new command();
            c.Description = "Pickup tip #4.";
            c.ServerCommand = "Subsystem: TeslaPlatform PickupTip 1 4;";
            commands.Add(c);
            c = new command();
            c.Description = "Move to the sample tube";
            c.ServerCommand = "Subsystem: Instrument setContainerVolume 1 'SampleVial' 100;Subsystem: Instrument MoveToAspirate self.ContainerAt(1,'SampleVial') 100;";
            commands.Add(c);
            	
            
            
            c = new command();
            c.Description = "Set Robot_ZAxis defaults";
            c.AxisName = "Robot_ZAxis";
            c.AxisCoarse = 1;
            c.AxisFine = 0.5;
            c.AxisUnits = "mm";
            commands.Add(c);

	
            c = new command();
            c.Description = "Adjust the Z axis so that the tip is down as far as possible using the 'Manual Axis Controls', then back it off 2 mm.";
            c.IsManualStep = true;
            commands.Add(c);
            
            c = new command();
            c.Description = "Stores tip height for 14mL tubes";
            c.ServerCommand = "Subsystem: Calibration calibrateSet14mLTubeTipHeight;";
            commands.Add(c);
            c = new command();
            c.Description = "Move to the lysis tube.";
            c.ServerCommand = "Subsystem: Instrument setContainerVolume 1 'LysisVial' 100;Subsystem: Instrument MoveToAspirate self.ContainerAt(1,'LysisVial') 100;";
            commands.Add(c);
            c = new command();
            c.Description = "Adjust the Z axis so that the tip is down as far as possible using the 'Manual Axis Controls', then back it off 2 mm.";
            c.IsManualStep = true;
            commands.Add(c);
            
            c = new command();
            c.Description = "Stores tip height for 50mL tubes";
            c.ServerCommand = "Subsystem: Calibration calibrateSet50mLTubeTipHeight;";
            commands.Add(c);
            c = new command();
            c.Description = "Strip the current tip.";
            c.ServerCommand = "Subsystem: TeslaPlatform StripTip;";
            commands.Add(c);


            c = new command();
            c.Description = "Home all axes.";
            c.ServerCommand = "Subsystem: Instrument HomeAll 0;";
            commands.Add(c);
            c = new command();
            c.Description = "Exit calibration mode. NOTE: This will save the new calibration settings to file. The new calibration will not take effect until the instrument is restarted.";
            c.ServerCommand = "Subsystem: Calibration endCalibration;";
            commands.Add(c);
            c = new command();
            c.Description = "Reset Instrument";
            c.ServerCommand = "Centre reInitInstrument;";
            commands.Add(c);

            s.command = commands.ToArray();
            scripts.Add(s);
            #endregion


            s = new script();
            c = new command();
            commands.Clear();
            s.name = "Verification";
            #region Verification
            c.IsMessageOnly = true;
            c.Description = "Fully load quadrant number 1 with all empty reagents, consumables and a tip box. Note that instrument calibration changes will not take effect until after the instrument is restarted.";
            commands.Add(c);
            c = new command();
            c.Description = "Home all axes.";
            c.ServerCommand = "Subsystem: Instrument HomeAll 0;";
            commands.Add(c);
            	
            c = new command();
            c.Description = "Pickup tip #1.";
            c.ServerCommand = "Subsystem: TeslaPlatform PickupTip 1 1;";
            commands.Add(c);
#region strip current tip
            c = new command();
            c.Description = "Strip the current tip.";
            c.ServerCommand = "Subsystem: TeslaPlatform StripTip;";
            commands.Add(c);
#endregion
            c = new command();
            c.Description = "Pickup tip #2.";
            c.ServerCommand = "Subsystem: TeslaPlatform PickupTip 1 2;";
            commands.Add(c);
#region strip current tip
            c = new command();
            c.Description = "Strip the current tip.";
            c.ServerCommand = "Subsystem: TeslaPlatform StripTip;";
            commands.Add(c);
#endregion
            c = new command();
            c.Description = "Pickup tip #3.";
            c.ServerCommand = "Subsystem: TeslaPlatform PickupTip 1 3;";
            commands.Add(c);

            c = new command();
            c.Description = "Perform a 100uL aspirate from the antibody vial.";
            c.ServerCommand = "Subsystem: Instrument setContainerVolume 1 'AntibodyVial' 100;Subsystem: Instrument MoveToAspirate self.ContainerAt(1,'AntibodyVial') 100;Subsystem: Calibration wait 2000;";
            commands.Add(c);
            c = new command();
            c.Description = "Perform a 100uL aspirate from the cocktail vial.";
            c.ServerCommand = "Subsystem: Instrument setContainerVolume 1 'CocktailVial' 100;Subsystem: Instrument MoveToAspirate self.ContainerAt(1,'CocktailVial') 100;Subsystem: Calibration wait 2000;";
            commands.Add(c);
            c = new command();
            c.Description = "Perform a 100uL aspirate from the bead vial.";
            c.ServerCommand = "Subsystem: Instrument setContainerVolume 1 'ParticleVial' 100;Subsystem: Instrument MoveToAspirate self.ContainerAt(1,'ParticleVial') 100;Subsystem: Calibration wait 2000;";
            commands.Add(c);
            c = new command();
            c.Description = "Perform a 100uL aspirate from the sample vial.";
            c.ServerCommand = "Subsystem: Instrument setContainerVolume 1 'SampleVial' 100;Subsystem: Instrument MoveToAspirate self.ContainerAt(1,'SampleVial') 100;Subsystem: Calibration wait 2000;";
            commands.Add(c);
            c = new command();
            c.Description = "Perform a 100uL aspirate from the supernatent vial.";
            c.ServerCommand = "Subsystem: Instrument setContainerVolume 1 'SeparationVial' 100;Subsystem: Instrument MoveToAspirate self.ContainerAt(1,'SeparationVial') 100;Subsystem: Calibration wait 2000;";
            commands.Add(c);
            
#region strip current tip
            c = new command();
            c.Description = "Strip the current tip.";
            c.ServerCommand = "Subsystem: TeslaPlatform StripTip;";
            commands.Add(c);
#endregion
            c = new command();
            c.Description = "Pickup tip #4.";
            c.ServerCommand = "Subsystem: TeslaPlatform PickupTip 1 4;";
            commands.Add(c);
#region strip current tip
            c = new command();
            c.Description = "Strip the current tip.";
            c.ServerCommand = "Subsystem: TeslaPlatform StripTip;";
            commands.Add(c);
#endregion
            c = new command();
            c.Description = "Pickup tip #5.";
            c.ServerCommand = "Subsystem: TeslaPlatform PickupTip 1 5;";
            commands.Add(c);
            
            c = new command();
            c.Description = "Perform a 100uL aspirate from the sample vial.";
            c.ServerCommand = "Subsystem: Instrument setContainerVolume 1 'SampleVial' 100;Subsystem: Instrument MoveToAspirate self.ContainerAt(1,'SampleVial') 100;Subsystem: Calibration wait 2000;";
            commands.Add(c);
            c = new command();
            c.Description = "Perform a 100uL aspirate from the supernatent vial.";
            c.ServerCommand = "Subsystem: Instrument setContainerVolume 1 'SeparationVial' 100;Subsystem: Instrument MoveToAspirate self.ContainerAt(1,'SeparationVial') 100;Subsystem: Calibration wait 2000;";
            commands.Add(c);
            c = new command();
            c.Description = "Perform a 100uL aspirate from the lysis vial.";
            c.ServerCommand = "Subsystem: Instrument setContainerVolume 1 'LysisVial' 100;Subsystem: Instrument MoveToAspirate self.ContainerAt(1,'LysisVial') 100;Subsystem: Calibration wait 2000;";
            commands.Add(c);
            c = new command();
            c.Description = "Perform a 100uL dispense into the waste vial.";
            c.ServerCommand = "Subsystem: Instrument emptyWasteContainers;Subsystem: Instrument MoveToDispense self.ContainerAt(1,'WasteVial') 100 False;Subsystem: Calibration wait 2000;";
            commands.Add(c);
            c = new command();
            c.Description = "Perform a 1000uL aspirate from the bccm.";
            c.ServerCommand = "Subsystem: Instrument setContainerVolume 0 'BCCMContainer' 1000;Subsystem: Instrument MoveToAspirate self.ContainerAt(0,'BCCMContainer') 1000;Subsystem: Calibration wait 2000;";
            commands.Add(c);
#region strip current tip
            c = new command();
            c.Description = "Strip the current tip.";
            c.ServerCommand = "Subsystem: TeslaPlatform StripTip;";
            commands.Add(c);
#endregion

            
            c = new command();
            c.Description = "Perform a free air dispense in to the sample vial.";
            c.ServerCommand = "Subsystem: Instrument MoveToDispense self.ContainerAt(1,'SampleVial') 10 True;Subsystem: Calibration wait 2000;";
            commands.Add(c);
            c = new command();
            c.Description = "Perform a free air dispense in to the supernatent vial.";
            c.ServerCommand = "Subsystem: Instrument MoveToDispense self.ContainerAt(1,'SeparationVial') 10 True;Subsystem: Calibration wait 2000;";
            commands.Add(c);
            c = new command();
            c.Description = "Perform a free air dispense in to the waste vial.";
            c.ServerCommand = "Subsystem: Instrument MoveToDispense self.ContainerAt(1,'WasteVial') 10 True;Subsystem: Calibration wait 2000;";
            commands.Add(c);
            c = new command();
            c.Description = "Perform a free air dispense in to the lysis vial.";
            c.ServerCommand = "Subsystem: Instrument MoveToDispense self.ContainerAt(1,'LysisVial') 10 True;Subsystem: Calibration wait 2000;";
            commands.Add(c);
            c = new command();
            c.Description = "Perform a free air dispense in to the bccm.";
            c.ServerCommand = "Subsystem: Instrument MoveToDispense self.ContainerAt(0,'BCCMContainer') 10 True;Subsystem: Calibration wait 2000;";
            commands.Add(c);
            
            c = new command();
            c.Description = "Home all axes.";
            c.ServerCommand = "Subsystem: Instrument HomeAll 0;";
            commands.Add(c);

            s.command = commands.ToArray();
            scripts.Add(s);
		
            #endregion


            service.calibrationScripts.script = scripts.ToArray();

            
		    XmlSerializer	myXmlSerializer = new XmlSerializer(typeof(RoboSepService));
		    string myUserConfigPath = Application.StartupPath+"\\..\\config\\RoboSepService.config";
            using (FileStream fs = new FileStream(myUserConfigPath, FileMode.Create))
            {
                XmlTextWriter writer = new XmlTextWriter(fs, new UTF8Encoding());
                writer.Formatting = Formatting.Indented;
                myXmlSerializer.Serialize(writer, service);
            }
             * */
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ServiceLogin_Load(object sender, EventArgs e)
        {
            checkBox_standardUser.Check = true;
            // change graphics for Login Button
            List<Image> ilist = new List<Image>();
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_PROMPTBUTTON0);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_PROMPTBUTTON0);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_PROMPTBUTTON0);
            ilist.Add(GUI_Controls.Properties.Resources.BUTTON_PROMPTBUTTON3);
            button_Login.ChangeGraphics(ilist);

            button_Cancel.ChangeGraphics(ilist);


            string LOGmsg = "Service Login Form loaded";
            GUI_Controls.uiLog.SERVICE_LOG(this, "ServiceLogin", GUI_Controls.uiLog.LogLevel.EVENTS, LOGmsg);
            this.Region = roboPanel2.Region;
        }
        private void label_superUser_Click(object sender, EventArgs e)
        {
            textBox_UserName.Text = "super";
            checkBox_standardUser.Check = false;
            checkBox_maintenanceUser.Check = false;
            checkBox_superUser.Check = true;
        }

        private void checkBox_superUser_Click(object sender, EventArgs e)
        {
            textBox_UserName.Text = "super";
            checkBox_standardUser.Check = false;
            checkBox_maintenanceUser.Check = false;
            checkBox_superUser.Check = true;
        }

        private void label_maintenanceUser_Click(object sender, EventArgs e)
        {
            textBox_UserName.Text = "maint";
            checkBox_standardUser.Check = false;
            checkBox_maintenanceUser.Check = true;
            checkBox_superUser.Check = false;
        }

        private void checkBox_maintenanceUser_Click(object sender, EventArgs e)
        {
            textBox_UserName.Text = "maint";
            checkBox_standardUser.Check = false;
            checkBox_maintenanceUser.Check = true;
            checkBox_superUser.Check = false;
        }

        private void label_standardUser_Click(object sender, EventArgs e)
        {
            textBox_UserName.Text = "user";
            checkBox_standardUser.Check = true;
            checkBox_maintenanceUser.Check = false;
            checkBox_superUser.Check = false;
        }

        private void checkBox_standardUser_Click(object sender, EventArgs e)
        {
            textBox_UserName.Text = "user";
            checkBox_standardUser.Check = true;
            checkBox_maintenanceUser.Check = false;
            checkBox_superUser.Check = false;
        }

        private bool validateUser()
        {
            string username = textBox_UserName.Text.ToUpper();
            if (username.Length < 5)
            {
                string sMSG = GUI_Controls.fromINI.getValue("GUI", "msgLoginError3");
                sMSG = sMSG == null ? "User Name entered is not long enough" : sMSG;
                GUI_Controls.RoboMessagePanel newPrompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(),
                    sMSG);
                RoboSep_UserConsole.getInstance().addForm(newPrompt, newPrompt.Offset);
                RoboSep_UserConsole.getInstance().frmOverlay.Show();
                newPrompt.ShowDialog();
                RoboSep_UserConsole.getInstance().frmOverlay.Hide();
                //MessageBox.Show("User name not long enough");
                return false;
            }
            for (int i = 0; i < username.Length; i++)
            {
                if (!((int)username[i] >= 65 && (int)username[i] <= 90 || (int)username[i] == 32))
                {
                    string sMSG = GUI_Controls.fromINI.getValue("GUI", "msgLoginError1");
                    sMSG = sMSG == null ? "User name should not include numbers, symbols, or punctuation" : sMSG;
                    GUI_Controls.RoboMessagePanel newPrompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(),
                       sMSG);
                    RoboSep_UserConsole.getInstance().addForm(newPrompt, newPrompt.Offset);
                    RoboSep_UserConsole.getInstance().frmOverlay.Show();
                    newPrompt.ShowDialog();
                    RoboSep_UserConsole.getInstance().frmOverlay.Hide();
                    //MessageBox.Show("User name should not include symbols or punctuation");
                    return false;
                }
                if (i < (username.Length - 2))
                {
                    if (username[i] == username[i + 1] && username[i] == username[i + 2])
                    {
                        string sMSG = GUI_Controls.fromINI.getValue("GUI", "msgLoginError2");
                        sMSG = sMSG == null ? "Incorrenctly typed User Name" : sMSG;
                        GUI_Controls.RoboMessagePanel newPrompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(),
                        sMSG);
                        RoboSep_UserConsole.getInstance().addForm(newPrompt, newPrompt.Offset);
                        RoboSep_UserConsole.getInstance().frmOverlay.Show();
                        newPrompt.ShowDialog();
                        RoboSep_UserConsole.getInstance().frmOverlay.Hide();
                        //MessageBox.Show("Incorrectly typed user name");
                        return false;
                    }
                }
            }
            return true;
        }

        private void Login()
        {
            GUI_Controls.uiLog.SERVICE_LOG(this, "ServiceLogin", GUI_Controls.uiLog.LogLevel.EVENTS, "Attempt login");
            //if (validateUser())
            {
                string[] userTypeString = new string[3] { STANDARD_USER, MAINTENANCE_USER, SUPER_USER };
                bool[] userType = new bool[3] { checkBox_standardUser.Check, checkBox_maintenanceUser.Check, checkBox_superUser.Check };

                int userLevel = 99;
                string currentUserType = "";
                bool validLogin = false;
                string errorMsg = "Invalid username or password, username and password are case sensetive";


                for (int i = 0; i < 3; i++)
                {
                    if (userType[i])
                    {
                        GUI_Controls.uiLog.SERVICE_LOG(this, "ServiceLogin", GUI_Controls.uiLog.LogLevel.EVENTS, "Login as " + userTypeString[i]);
                        currentUserType = userTypeString[i];
                        userLevel = i + 1;
                        break;
                    }
                }

                try
                {
                    // grab value from config file

                    NameValueCollection nvc = (NameValueCollection)
                        ConfigurationSettings.GetConfig("Service/ServiceLogin");
                    string user_password_level = DecryptString(nvc.Get(textBox_UserName.Text), rsaKeysize, rsaKey);
                    string[] user_password_level_arr = user_password_level.Split(':');
                    if (user_password_level_arr.Length != 3)
                    {
                        errorMsg = "Corrupted user info in config file. Please contact support.";
                    }
                    else if (textBox_UserName.Text != user_password_level_arr[0])
                    {
                        errorMsg = "Username does not match user info in config file. Please contact support.";
                    }
                    else if (textBox_servicePassword.Text != user_password_level_arr[1])
                    {
                        errorMsg = "Incorrect password. Please try again with the correct case sensitive password.";
                    }
                    else if (int.Parse(user_password_level_arr[2]) < userLevel)
                    {
                        errorMsg = "You do not have permission to login with selected user level. Please select a lower user level and try again.";
                    }
                    else
                    {
                        validLogin = true;
                    }
                }
                catch
                {
                    //check for defaults if can't find user
                    if (textBox_UserName.Text == "stemcell" && textBox_servicePassword.Text == "stemcell")
                    {
                        validLogin = true;
                    }
                }


                if (validLogin)
                {
                    GUI_Controls.uiLog.SERVICE_LOG(this, "ServiceLogin", GUI_Controls.uiLog.LogLevel.EVENTS, "Login successful");
                    btnLogin_Click(currentUserType, null);
                }
                else
                {
                    GUI_Controls.RoboMessagePanel newPrompt = new GUI_Controls.RoboMessagePanel(RoboSep_UserConsole.getInstance(),
                        errorMsg);
                    RoboSep_UserConsole.getInstance().frmOverlay.Show();

                    this.TopMost = false;
                    newPrompt.ShowDialog();

                    this.TopMost = true;
                    RoboSep_UserConsole.getInstance().frmOverlay.Hide();
                    if (newPrompt.DialogResult == DialogResult.OK)
                    {
                        ActiveControl = textBox_servicePassword;
                    }
                }
            
            }
        }

        private void textBox_servicePassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // submit password for login
                Login();
            }
        }

        private void button_Login_Click(object sender, EventArgs e)
        {
            // submit password for login
            Login();
        }

        private void textBox_UserName_MouseClick(object sender, MouseEventArgs e)
        {
            if (RoboSep_UserConsole.KeyboardEnabled)
            {
                // Show window overlay
                RoboSep_UserConsole.getInstance().frmOverlay.Show();
                RoboSep_UserConsole.getInstance().frmOverlay.BringToFront();

                // Create keybaord control
                GUI_Controls.Keyboard newKeyboard =
                    new GUI_Controls.Keyboard(this,
                        textBox_UserName, null,RoboSep_UserConsole.getInstance().frmOverlay,false);

                newKeyboard.Show();

                // add keyboard control to user console "track form"
                RoboSep_UserConsole.getInstance().addForm(newKeyboard, newKeyboard.Offset);
                //RoboSep_UserConsole.myUserConsole.addForm(newKeyboard.overlay, newKeyboard.overlayOffset);
            }
        }

        private void textBox_servicePassword_MouseClick(object sender, MouseEventArgs e)
        {
            if (RoboSep_UserConsole.KeyboardEnabled)
            {
                // set text to "" so as not to show previous attempted password
                textBox_servicePassword.Text = "";

                // Show window overlay
                RoboSep_UserConsole.getInstance().frmOverlay.Show();
                RoboSep_UserConsole.getInstance().frmOverlay.BringToFront();

                // Create keyboard
                GUI_Controls.Keyboard newKeyboard =
                    new GUI_Controls.Keyboard(this,
                        textBox_servicePassword, null,RoboSep_UserConsole.getInstance().frmOverlay,true);
                
                newKeyboard.Show();
                RoboSep_UserConsole.getInstance().addForm(newKeyboard, newKeyboard.Offset);
                //RoboSep_UserConsole.myUserConsole.addForm(newKeyboard.overlay, newKeyboard.overlayOffset);
            }
        }

        #region password encrypt
        public string EncryptString(string inputString, int dwKeySize,
                             string xmlString)
        {
            // TODO: Add Proper Exception Handlers
            RSACryptoServiceProvider rsaCryptoServiceProvider =
                                          new RSACryptoServiceProvider(dwKeySize);
            rsaCryptoServiceProvider.FromXmlString(xmlString);
            int keySize = dwKeySize / 8;
            byte[] bytes = Encoding.UTF32.GetBytes(inputString);
            // The hash function in use by the .NET RSACryptoServiceProvider here 
            // is SHA1
            // int maxLength = ( keySize ) - 2 - 
            //              ( 2 * SHA1.Create().ComputeHash( rawBytes ).Length );
            int maxLength = keySize - 42;
            int dataLength = bytes.Length;
            int iterations = dataLength / maxLength;
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i <= iterations; i++)
            {
                byte[] tempBytes = new byte[
                        (dataLength - maxLength * i > maxLength) ? maxLength :
                                                      dataLength - maxLength * i];
                Buffer.BlockCopy(bytes, maxLength * i, tempBytes, 0,
                                  tempBytes.Length);
                byte[] encryptedBytes = rsaCryptoServiceProvider.Encrypt(tempBytes,
                                                                          true);
                // Be aware the RSACryptoServiceProvider reverses the order of 
                // encrypted bytes. It does this after encryption and before 
                // decryption. If you do not require compatibility with Microsoft 
                // Cryptographic API (CAPI) and/or other vendors. Comment out the 
                // next line and the corresponding one in the DecryptString function.
                Array.Reverse(encryptedBytes);
                // Why convert to base 64?
                // Because it is the largest power-of-two base printable using only 
                // ASCII characters
                stringBuilder.Append(Convert.ToBase64String(encryptedBytes));
            }
            return stringBuilder.ToString();
        }

        public string DecryptString(string inputString, int dwKeySize,
                                     string xmlString)
        {
            // TODO: Add Proper Exception Handlers
            RSACryptoServiceProvider rsaCryptoServiceProvider
                                     = new RSACryptoServiceProvider(dwKeySize);
            rsaCryptoServiceProvider.FromXmlString(xmlString);
            int base64BlockSize = ((dwKeySize / 8) % 3 != 0) ?
              (((dwKeySize / 8) / 3) * 4) + 4 : ((dwKeySize / 8) / 3) * 4;
            int iterations = inputString.Length / base64BlockSize;
            ArrayList arrayList = new ArrayList();
            for (int i = 0; i < iterations; i++)
            {
                byte[] encryptedBytes = Convert.FromBase64String(
                     inputString.Substring(base64BlockSize * i, base64BlockSize));
                // Be aware the RSACryptoServiceProvider reverses the order of 
                // encrypted bytes after encryption and before decryption.
                // If you do not require compatibility with Microsoft Cryptographic 
                // API (CAPI) and/or other vendors.
                // Comment out the next line and the corresponding one in the 
                // EncryptString function.
                Array.Reverse(encryptedBytes);
                arrayList.AddRange(rsaCryptoServiceProvider.Decrypt(
                                    encryptedBytes, true));
            }
            return Encoding.UTF32.GetString(arrayList.ToArray(
                                      Type.GetType("System.Byte")) as byte[]);
        }
        #endregion
    }
}
