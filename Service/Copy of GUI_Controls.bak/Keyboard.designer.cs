namespace GUI_Controls
{
    partial class Keyboard
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Keyboard));
            this.textbox_input = new System.Windows.Forms.TextBox();
            this.buttonShift = new GUI_Controls.Button_Keyboard();
            this.buttonEnter = new GUI_Controls.Button_Keyboard();
            this.button_space = new GUI_Controls.Button_Keyboard();
            this.button_Keyboard1 = new GUI_Controls.Button_Keyboard();
            this.button2 = new GUI_Controls.Button_Keyboard();
            this.button3 = new GUI_Controls.Button_Keyboard();
            this.button4 = new GUI_Controls.Button_Keyboard();
            this.button5 = new GUI_Controls.Button_Keyboard();
            this.button6 = new GUI_Controls.Button_Keyboard();
            this.button7 = new GUI_Controls.Button_Keyboard();
            this.button8 = new GUI_Controls.Button_Keyboard();
            this.button9 = new GUI_Controls.Button_Keyboard();
            this.button0 = new GUI_Controls.Button_Keyboard();
            this.buttonDel = new GUI_Controls.Button_Keyboard();
            this.buttonQ = new GUI_Controls.Button_Keyboard();
            this.buttonW = new GUI_Controls.Button_Keyboard();
            this.buttonE = new GUI_Controls.Button_Keyboard();
            this.buttonR = new GUI_Controls.Button_Keyboard();
            this.buttonT = new GUI_Controls.Button_Keyboard();
            this.buttonY = new GUI_Controls.Button_Keyboard();
            this.buttonU = new GUI_Controls.Button_Keyboard();
            this.buttonI = new GUI_Controls.Button_Keyboard();
            this.buttonO = new GUI_Controls.Button_Keyboard();
            this.buttonP = new GUI_Controls.Button_Keyboard();
            this.buttonCaps = new GUI_Controls.Button_Keyboard();
            this.buttonA = new GUI_Controls.Button_Keyboard();
            this.buttonS = new GUI_Controls.Button_Keyboard();
            this.buttonD = new GUI_Controls.Button_Keyboard();
            this.buttonF = new GUI_Controls.Button_Keyboard();
            this.buttonG = new GUI_Controls.Button_Keyboard();
            this.buttonH = new GUI_Controls.Button_Keyboard();
            this.buttonJ = new GUI_Controls.Button_Keyboard();
            this.buttonK = new GUI_Controls.Button_Keyboard();
            this.buttonL = new GUI_Controls.Button_Keyboard();
            this.buttonZ = new GUI_Controls.Button_Keyboard();
            this.buttonX = new GUI_Controls.Button_Keyboard();
            this.buttonC = new GUI_Controls.Button_Keyboard();
            this.buttonV = new GUI_Controls.Button_Keyboard();
            this.buttonB = new GUI_Controls.Button_Keyboard();
            this.buttonN = new GUI_Controls.Button_Keyboard();
            this.buttonM = new GUI_Controls.Button_Keyboard();
            this.buttonDot = new GUI_Controls.Button_Keyboard();
            this.button_ = new GUI_Controls.Button_Keyboard();
            this.button10 = new GUI_Controls.Button_Keyboard();
            this.roboPanel1 = new GUI_Controls.RoboPanel();
            this.button_Cancel1 = new GUI_Controls.Button_Cancel();
            this.roboPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textbox_input
            // 
            this.textbox_input.AllowDrop = true;
            this.textbox_input.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.textbox_input.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.textbox_input.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textbox_input.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textbox_input.Location = new System.Drawing.Point(22, 21);
            this.textbox_input.Name = "textbox_input";
            this.textbox_input.Size = new System.Drawing.Size(453, 32);
            this.textbox_input.TabIndex = 0;
            this.textbox_input.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textbox_input_KeyDown);
            // 
            // buttonShift
            // 
            this.buttonShift.AccessibleName = "Shift";
            this.buttonShift.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonShift.BackgroundImage")));
            this.buttonShift.check = false;
            this.buttonShift.Location = new System.Drawing.Point(511, 217);
            this.buttonShift.Name = "buttonShift";
            this.buttonShift.Size = new System.Drawing.Size(83, 50);
            this.buttonShift.TabIndex = 93;
            this.buttonShift.Click += new System.EventHandler(this.buttonShift_Click);
            // 
            // buttonEnter
            // 
            this.buttonEnter.AccessibleName = "Enter";
            this.buttonEnter.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonEnter.BackgroundImage")));
            this.buttonEnter.check = false;
            this.buttonEnter.Location = new System.Drawing.Point(489, 166);
            this.buttonEnter.Name = "buttonEnter";
            this.buttonEnter.Size = new System.Drawing.Size(105, 50);
            this.buttonEnter.TabIndex = 83;
            this.buttonEnter.Enter += new System.EventHandler(this.buttonEnter_Click);
            // 
            // button_space
            // 
            this.button_space.AccessibleName = "   ";
            this.button_space.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_space.BackgroundImage")));
            this.button_space.check = false;
            this.button_space.Location = new System.Drawing.Point(81, 270);
            this.button_space.Name = "button_space";
            this.button_space.Size = new System.Drawing.Size(384, 50);
            this.button_space.TabIndex = 51;
            this.button_space.Click += new System.EventHandler(this.button_space_Click);
            // 
            // button_Keyboard1
            // 
            this.button_Keyboard1.AccessibleName = "1";
            this.button_Keyboard1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_Keyboard1.BackgroundImage")));
            this.button_Keyboard1.check = false;
            this.button_Keyboard1.Location = new System.Drawing.Point(12, 64);
            this.button_Keyboard1.Name = "button_Keyboard1";
            this.button_Keyboard1.Size = new System.Drawing.Size(50, 50);
            this.button_Keyboard1.TabIndex = 52;
            this.button_Keyboard1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.AccessibleName = "2";
            this.button2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button2.BackgroundImage")));
            this.button2.check = false;
            this.button2.Location = new System.Drawing.Point(63, 64);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(50, 50);
            this.button2.TabIndex = 53;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.AccessibleName = "3";
            this.button3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button3.BackgroundImage")));
            this.button3.check = false;
            this.button3.Location = new System.Drawing.Point(114, 64);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(50, 50);
            this.button3.TabIndex = 54;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.AccessibleName = "4";
            this.button4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button4.BackgroundImage")));
            this.button4.check = false;
            this.button4.Location = new System.Drawing.Point(165, 64);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(50, 50);
            this.button4.TabIndex = 55;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.AccessibleName = "5";
            this.button5.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button5.BackgroundImage")));
            this.button5.check = false;
            this.button5.Location = new System.Drawing.Point(216, 64);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(50, 50);
            this.button5.TabIndex = 56;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.AccessibleName = "6";
            this.button6.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button6.BackgroundImage")));
            this.button6.check = false;
            this.button6.Location = new System.Drawing.Point(267, 64);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(50, 50);
            this.button6.TabIndex = 57;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.AccessibleName = "7";
            this.button7.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button7.BackgroundImage")));
            this.button7.check = false;
            this.button7.Location = new System.Drawing.Point(318, 64);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(50, 50);
            this.button7.TabIndex = 58;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button8
            // 
            this.button8.AccessibleName = "8";
            this.button8.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button8.BackgroundImage")));
            this.button8.check = false;
            this.button8.Location = new System.Drawing.Point(369, 64);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(50, 50);
            this.button8.TabIndex = 59;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // button9
            // 
            this.button9.AccessibleName = "9";
            this.button9.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button9.BackgroundImage")));
            this.button9.check = false;
            this.button9.Location = new System.Drawing.Point(420, 64);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(50, 50);
            this.button9.TabIndex = 60;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button0
            // 
            this.button0.AccessibleName = "0";
            this.button0.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button0.BackgroundImage")));
            this.button0.check = false;
            this.button0.Location = new System.Drawing.Point(471, 64);
            this.button0.Name = "button0";
            this.button0.Size = new System.Drawing.Size(50, 50);
            this.button0.TabIndex = 61;
            this.button0.Click += new System.EventHandler(this.button0_Click);
            // 
            // buttonDel
            // 
            this.buttonDel.AccessibleName = "Del";
            this.buttonDel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonDel.BackgroundImage")));
            this.buttonDel.check = false;
            this.buttonDel.Location = new System.Drawing.Point(522, 64);
            this.buttonDel.Name = "buttonDel";
            this.buttonDel.Size = new System.Drawing.Size(73, 50);
            this.buttonDel.TabIndex = 62;
            this.buttonDel.Click += new System.EventHandler(this.buttonDel_Click);
            // 
            // buttonQ
            // 
            this.buttonQ.AccessibleName = "Q";
            this.buttonQ.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonQ.BackgroundImage")));
            this.buttonQ.check = false;
            this.buttonQ.Location = new System.Drawing.Point(11, 116);
            this.buttonQ.Name = "buttonQ";
            this.buttonQ.Size = new System.Drawing.Size(50, 50);
            this.buttonQ.TabIndex = 63;
            this.buttonQ.Click += new System.EventHandler(this.buttonQ_Click);
            // 
            // buttonW
            // 
            this.buttonW.AccessibleName = "W";
            this.buttonW.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonW.BackgroundImage")));
            this.buttonW.check = false;
            this.buttonW.Location = new System.Drawing.Point(62, 116);
            this.buttonW.Name = "buttonW";
            this.buttonW.Size = new System.Drawing.Size(50, 50);
            this.buttonW.TabIndex = 64;
            this.buttonW.Click += new System.EventHandler(this.buttonW_Click);
            // 
            // buttonE
            // 
            this.buttonE.AccessibleName = "E";
            this.buttonE.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonE.BackgroundImage")));
            this.buttonE.check = false;
            this.buttonE.Location = new System.Drawing.Point(113, 116);
            this.buttonE.Name = "buttonE";
            this.buttonE.Size = new System.Drawing.Size(50, 50);
            this.buttonE.TabIndex = 65;
            this.buttonE.Click += new System.EventHandler(this.buttonE_Click);
            // 
            // buttonR
            // 
            this.buttonR.AccessibleName = "R";
            this.buttonR.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonR.BackgroundImage")));
            this.buttonR.check = false;
            this.buttonR.Location = new System.Drawing.Point(164, 116);
            this.buttonR.Name = "buttonR";
            this.buttonR.Size = new System.Drawing.Size(50, 50);
            this.buttonR.TabIndex = 66;
            this.buttonR.Click += new System.EventHandler(this.buttonR_Click);
            // 
            // buttonT
            // 
            this.buttonT.AccessibleName = "T";
            this.buttonT.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonT.BackgroundImage")));
            this.buttonT.check = false;
            this.buttonT.Location = new System.Drawing.Point(215, 116);
            this.buttonT.Name = "buttonT";
            this.buttonT.Size = new System.Drawing.Size(50, 50);
            this.buttonT.TabIndex = 67;
            this.buttonT.Click += new System.EventHandler(this.buttonT_Click);
            // 
            // buttonY
            // 
            this.buttonY.AccessibleName = "Y";
            this.buttonY.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonY.BackgroundImage")));
            this.buttonY.check = false;
            this.buttonY.Location = new System.Drawing.Point(266, 116);
            this.buttonY.Name = "buttonY";
            this.buttonY.Size = new System.Drawing.Size(50, 50);
            this.buttonY.TabIndex = 68;
            this.buttonY.Click += new System.EventHandler(this.buttonY_Click);
            // 
            // buttonU
            // 
            this.buttonU.AccessibleName = "U";
            this.buttonU.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonU.BackgroundImage")));
            this.buttonU.check = false;
            this.buttonU.Location = new System.Drawing.Point(317, 116);
            this.buttonU.Name = "buttonU";
            this.buttonU.Size = new System.Drawing.Size(50, 50);
            this.buttonU.TabIndex = 69;
            this.buttonU.Click += new System.EventHandler(this.buttonU_Click);
            // 
            // buttonI
            // 
            this.buttonI.AccessibleName = "I";
            this.buttonI.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonI.BackgroundImage")));
            this.buttonI.check = false;
            this.buttonI.Location = new System.Drawing.Point(368, 116);
            this.buttonI.Name = "buttonI";
            this.buttonI.Size = new System.Drawing.Size(50, 50);
            this.buttonI.TabIndex = 70;
            this.buttonI.Click += new System.EventHandler(this.buttonI_Click);
            // 
            // buttonO
            // 
            this.buttonO.AccessibleName = "O";
            this.buttonO.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonO.BackgroundImage")));
            this.buttonO.check = false;
            this.buttonO.Location = new System.Drawing.Point(419, 116);
            this.buttonO.Name = "buttonO";
            this.buttonO.Size = new System.Drawing.Size(50, 50);
            this.buttonO.TabIndex = 71;
            this.buttonO.Click += new System.EventHandler(this.buttonO_Click);
            // 
            // buttonP
            // 
            this.buttonP.AccessibleName = "P";
            this.buttonP.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonP.BackgroundImage")));
            this.buttonP.check = false;
            this.buttonP.Location = new System.Drawing.Point(470, 116);
            this.buttonP.Name = "buttonP";
            this.buttonP.Size = new System.Drawing.Size(50, 50);
            this.buttonP.TabIndex = 72;
            this.buttonP.Click += new System.EventHandler(this.buttonP_Click);
            // 
            // buttonCaps
            // 
            this.buttonCaps.AccessibleName = "Caps";
            this.buttonCaps.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonCaps.BackgroundImage")));
            this.buttonCaps.check = false;
            this.buttonCaps.Location = new System.Drawing.Point(521, 116);
            this.buttonCaps.Name = "buttonCaps";
            this.buttonCaps.Size = new System.Drawing.Size(74, 50);
            this.buttonCaps.TabIndex = 73;
            this.buttonCaps.Click += new System.EventHandler(this.buttonCaps_Click);
            // 
            // buttonA
            // 
            this.buttonA.AccessibleName = "A";
            this.buttonA.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonA.BackgroundImage")));
            this.buttonA.check = false;
            this.buttonA.Location = new System.Drawing.Point(30, 166);
            this.buttonA.Name = "buttonA";
            this.buttonA.Size = new System.Drawing.Size(50, 50);
            this.buttonA.TabIndex = 74;
            this.buttonA.Click += new System.EventHandler(this.buttonA_Click);
            // 
            // buttonS
            // 
            this.buttonS.AccessibleName = "S";
            this.buttonS.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonS.BackgroundImage")));
            this.buttonS.check = false;
            this.buttonS.Location = new System.Drawing.Point(81, 166);
            this.buttonS.Name = "buttonS";
            this.buttonS.Size = new System.Drawing.Size(50, 50);
            this.buttonS.TabIndex = 75;
            this.buttonS.Click += new System.EventHandler(this.buttonS_Click);
            // 
            // buttonD
            // 
            this.buttonD.AccessibleName = "D";
            this.buttonD.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonD.BackgroundImage")));
            this.buttonD.check = false;
            this.buttonD.Location = new System.Drawing.Point(132, 166);
            this.buttonD.Name = "buttonD";
            this.buttonD.Size = new System.Drawing.Size(50, 50);
            this.buttonD.TabIndex = 76;
            this.buttonD.Click += new System.EventHandler(this.buttonD_Click);
            // 
            // buttonF
            // 
            this.buttonF.AccessibleName = "F";
            this.buttonF.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonF.BackgroundImage")));
            this.buttonF.check = false;
            this.buttonF.Location = new System.Drawing.Point(183, 166);
            this.buttonF.Name = "buttonF";
            this.buttonF.Size = new System.Drawing.Size(50, 50);
            this.buttonF.TabIndex = 77;
            this.buttonF.Click += new System.EventHandler(this.buttonF_Click);
            // 
            // buttonG
            // 
            this.buttonG.AccessibleName = "G";
            this.buttonG.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonG.BackgroundImage")));
            this.buttonG.check = false;
            this.buttonG.Location = new System.Drawing.Point(234, 166);
            this.buttonG.Name = "buttonG";
            this.buttonG.Size = new System.Drawing.Size(50, 50);
            this.buttonG.TabIndex = 78;
            this.buttonG.Click += new System.EventHandler(this.buttonG_Click);
            // 
            // buttonH
            // 
            this.buttonH.AccessibleName = "H";
            this.buttonH.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonH.BackgroundImage")));
            this.buttonH.check = false;
            this.buttonH.Location = new System.Drawing.Point(285, 166);
            this.buttonH.Name = "buttonH";
            this.buttonH.Size = new System.Drawing.Size(50, 50);
            this.buttonH.TabIndex = 79;
            this.buttonH.Click += new System.EventHandler(this.buttonH_Click);
            // 
            // buttonJ
            // 
            this.buttonJ.AccessibleName = "J";
            this.buttonJ.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonJ.BackgroundImage")));
            this.buttonJ.check = false;
            this.buttonJ.Location = new System.Drawing.Point(336, 166);
            this.buttonJ.Name = "buttonJ";
            this.buttonJ.Size = new System.Drawing.Size(50, 50);
            this.buttonJ.TabIndex = 80;
            this.buttonJ.Click += new System.EventHandler(this.buttonJ_Click);
            // 
            // buttonK
            // 
            this.buttonK.AccessibleName = "K";
            this.buttonK.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonK.BackgroundImage")));
            this.buttonK.check = false;
            this.buttonK.Location = new System.Drawing.Point(387, 166);
            this.buttonK.Name = "buttonK";
            this.buttonK.Size = new System.Drawing.Size(50, 50);
            this.buttonK.TabIndex = 81;
            this.buttonK.Click += new System.EventHandler(this.buttonK_Click);
            // 
            // buttonL
            // 
            this.buttonL.AccessibleName = "L";
            this.buttonL.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonL.BackgroundImage")));
            this.buttonL.check = false;
            this.buttonL.Location = new System.Drawing.Point(438, 166);
            this.buttonL.Name = "buttonL";
            this.buttonL.Size = new System.Drawing.Size(50, 50);
            this.buttonL.TabIndex = 82;
            this.buttonL.Click += new System.EventHandler(this.buttonL_Click);
            // 
            // buttonZ
            // 
            this.buttonZ.AccessibleName = "Z";
            this.buttonZ.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonZ.BackgroundImage")));
            this.buttonZ.check = false;
            this.buttonZ.Location = new System.Drawing.Point(52, 217);
            this.buttonZ.Name = "buttonZ";
            this.buttonZ.Size = new System.Drawing.Size(50, 50);
            this.buttonZ.TabIndex = 84;
            this.buttonZ.Click += new System.EventHandler(this.buttonZ_Click);
            // 
            // buttonX
            // 
            this.buttonX.AccessibleName = "X";
            this.buttonX.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonX.BackgroundImage")));
            this.buttonX.check = false;
            this.buttonX.Location = new System.Drawing.Point(103, 217);
            this.buttonX.Name = "buttonX";
            this.buttonX.Size = new System.Drawing.Size(50, 50);
            this.buttonX.TabIndex = 85;
            this.buttonX.Click += new System.EventHandler(this.buttonX_Click);
            // 
            // buttonC
            // 
            this.buttonC.AccessibleName = "C";
            this.buttonC.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonC.BackgroundImage")));
            this.buttonC.check = false;
            this.buttonC.Location = new System.Drawing.Point(154, 217);
            this.buttonC.Name = "buttonC";
            this.buttonC.Size = new System.Drawing.Size(50, 50);
            this.buttonC.TabIndex = 86;
            this.buttonC.Click += new System.EventHandler(this.buttonC_Click);
            // 
            // buttonV
            // 
            this.buttonV.AccessibleName = "V";
            this.buttonV.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonV.BackgroundImage")));
            this.buttonV.check = false;
            this.buttonV.Location = new System.Drawing.Point(205, 217);
            this.buttonV.Name = "buttonV";
            this.buttonV.Size = new System.Drawing.Size(50, 50);
            this.buttonV.TabIndex = 87;
            this.buttonV.Click += new System.EventHandler(this.buttonV_Click);
            // 
            // buttonB
            // 
            this.buttonB.AccessibleName = "B";
            this.buttonB.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonB.BackgroundImage")));
            this.buttonB.check = false;
            this.buttonB.Location = new System.Drawing.Point(256, 217);
            this.buttonB.Name = "buttonB";
            this.buttonB.Size = new System.Drawing.Size(50, 50);
            this.buttonB.TabIndex = 88;
            this.buttonB.Click += new System.EventHandler(this.buttonB_Click);
            // 
            // buttonN
            // 
            this.buttonN.AccessibleName = "N";
            this.buttonN.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonN.BackgroundImage")));
            this.buttonN.check = false;
            this.buttonN.Location = new System.Drawing.Point(307, 217);
            this.buttonN.Name = "buttonN";
            this.buttonN.Size = new System.Drawing.Size(50, 50);
            this.buttonN.TabIndex = 89;
            this.buttonN.Click += new System.EventHandler(this.buttonN_Click);
            // 
            // buttonM
            // 
            this.buttonM.AccessibleName = "M";
            this.buttonM.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonM.BackgroundImage")));
            this.buttonM.check = false;
            this.buttonM.Location = new System.Drawing.Point(358, 217);
            this.buttonM.Name = "buttonM";
            this.buttonM.Size = new System.Drawing.Size(50, 50);
            this.buttonM.TabIndex = 90;
            this.buttonM.Click += new System.EventHandler(this.buttonM_Click);
            // 
            // buttonDot
            // 
            this.buttonDot.AccessibleName = ".";
            this.buttonDot.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonDot.BackgroundImage")));
            this.buttonDot.check = false;
            this.buttonDot.Location = new System.Drawing.Point(409, 217);
            this.buttonDot.Name = "buttonDot";
            this.buttonDot.Size = new System.Drawing.Size(50, 50);
            this.buttonDot.TabIndex = 91;
            this.buttonDot.Click += new System.EventHandler(this.buttonDot_Click);
            // 
            // button_
            // 
            this.button_.AccessibleName = "_";
            this.button_.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_.BackgroundImage")));
            this.button_.check = false;
            this.button_.Location = new System.Drawing.Point(460, 217);
            this.button_.Name = "button_";
            this.button_.Size = new System.Drawing.Size(50, 50);
            this.button_.TabIndex = 92;
            this.button_.Click += new System.EventHandler(this.button__Click);
            // 
            // button10
            // 
            this.button10.AccessibleName = "Clear";
            this.button10.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button10.BackgroundImage")));
            this.button10.check = false;
            this.button10.Location = new System.Drawing.Point(489, 14);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(105, 50);
            this.button10.TabIndex = 94;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // roboPanel1
            // 
            this.roboPanel1.BackColor = System.Drawing.Color.Transparent;
            this.roboPanel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.roboPanel1.Controls.Add(this.button_Cancel1);
            this.roboPanel1.Location = new System.Drawing.Point(0, 0);
            this.roboPanel1.Name = "roboPanel1";
            this.roboPanel1.Size = new System.Drawing.Size(605, 334);
            this.roboPanel1.TabIndex = 95;
            // 
            // button_Cancel1
            // 
            this.button_Cancel1.AccessibleName = "  ";
            this.button_Cancel1.BackgroundImage = global::GUI_Controls.Properties.Resources.Button_QUADRANTCANCEL;
            this.button_Cancel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Cancel1.Check = false;
            this.button_Cancel1.Location = new System.Drawing.Point(561, 291);
            this.button_Cancel1.Name = "button_Cancel1";
            this.button_Cancel1.Size = new System.Drawing.Size(33, 33);
            this.button_Cancel1.TabIndex = 96;
            this.button_Cancel1.Click += new System.EventHandler(this.button_Cancel1_Click);
            // 
            // Keyboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(605, 334);
            this.Controls.Add(this.buttonShift);
            this.Controls.Add(this.buttonEnter);
            this.Controls.Add(this.button_space);
            this.Controls.Add(this.textbox_input);
            this.Controls.Add(this.button_Keyboard1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.button0);
            this.Controls.Add(this.buttonDel);
            this.Controls.Add(this.buttonQ);
            this.Controls.Add(this.buttonW);
            this.Controls.Add(this.buttonE);
            this.Controls.Add(this.buttonR);
            this.Controls.Add(this.buttonT);
            this.Controls.Add(this.buttonY);
            this.Controls.Add(this.buttonU);
            this.Controls.Add(this.buttonI);
            this.Controls.Add(this.buttonO);
            this.Controls.Add(this.buttonP);
            this.Controls.Add(this.buttonCaps);
            this.Controls.Add(this.buttonA);
            this.Controls.Add(this.buttonS);
            this.Controls.Add(this.buttonD);
            this.Controls.Add(this.buttonF);
            this.Controls.Add(this.buttonG);
            this.Controls.Add(this.buttonH);
            this.Controls.Add(this.buttonJ);
            this.Controls.Add(this.buttonK);
            this.Controls.Add(this.buttonL);
            this.Controls.Add(this.buttonZ);
            this.Controls.Add(this.buttonX);
            this.Controls.Add(this.buttonC);
            this.Controls.Add(this.buttonV);
            this.Controls.Add(this.buttonB);
            this.Controls.Add(this.buttonN);
            this.Controls.Add(this.buttonM);
            this.Controls.Add(this.buttonDot);
            this.Controls.Add(this.button_);
            this.Controls.Add(this.button10);
            this.Controls.Add(this.roboPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Keyboard";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Keyboard";
            this.TopMost = false;
            this.Deactivate += new System.EventHandler(this.Keyboard_Deactivate);
            this.Load += new System.EventHandler(this.Keyboard_Load);
            this.LocationChanged += new System.EventHandler(this.Keyboard_LocationChanged);
            this.roboPanel1.ResumeLayout(false);
            this.roboPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textbox_input;
        private Button_Keyboard button_space;
        private Button_Keyboard button_Keyboard1;
        private Button_Keyboard button2;
        private Button_Keyboard button3;
        private Button_Keyboard button4;
        private Button_Keyboard button5;
        private Button_Keyboard button6;
        private Button_Keyboard button7;
        private Button_Keyboard button8;
        private Button_Keyboard button9;
        private Button_Keyboard button0;
        private Button_Keyboard buttonDel;
        private Button_Keyboard buttonQ;
        private Button_Keyboard buttonW;
        private Button_Keyboard buttonE;
        private Button_Keyboard buttonR;
        private Button_Keyboard buttonT;
        private Button_Keyboard buttonY;
        private Button_Keyboard buttonU;
        private Button_Keyboard buttonI;
        private Button_Keyboard buttonO;
        private Button_Keyboard buttonP;
        private Button_Keyboard buttonCaps;
        private Button_Keyboard buttonA;
        private Button_Keyboard buttonS;
        private Button_Keyboard buttonD;
        private Button_Keyboard buttonF;
        private Button_Keyboard buttonG;
        private Button_Keyboard buttonH;
        private Button_Keyboard buttonJ;
        private Button_Keyboard buttonK;
        private Button_Keyboard buttonL;
        private Button_Keyboard buttonEnter;
        private Button_Keyboard buttonZ;
        private Button_Keyboard buttonX;
        private Button_Keyboard buttonC;
        private Button_Keyboard buttonV;
        private Button_Keyboard buttonB;
        private Button_Keyboard buttonN;
        private Button_Keyboard buttonM;
        private Button_Keyboard buttonDot;
        private Button_Keyboard button_;
        private Button_Keyboard buttonShift;
        private Button_Keyboard button10;
        private RoboPanel roboPanel1;
        private Button_Cancel button_Cancel1;
    }
}