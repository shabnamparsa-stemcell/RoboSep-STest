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
            this.rectangleShape1 = new Microsoft.VisualBasic.PowerPacks.RectangleShape();
            this.shapeContainer1 = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label_WindowTitle = new System.Windows.Forms.Label();
            this.button_Cancel1 = new GUI_Controls.Button_Cancel();
            this.button1 = new GUI_Controls.Button_Rectangle();
            this.buttonQ = new GUI_Controls.Button_Rectangle();
            this.button2 = new GUI_Controls.Button_Rectangle();
            this.buttonW = new GUI_Controls.Button_Rectangle();
            this.buttonA = new GUI_Controls.Button_Rectangle();
            this.buttonZ = new GUI_Controls.Button_Rectangle();
            this.button3 = new GUI_Controls.Button_Rectangle();
            this.buttonE = new GUI_Controls.Button_Rectangle();
            this.buttonS = new GUI_Controls.Button_Rectangle();
            this.buttonX = new GUI_Controls.Button_Rectangle();
            this.button4 = new GUI_Controls.Button_Rectangle();
            this.buttonR = new GUI_Controls.Button_Rectangle();
            this.buttonD = new GUI_Controls.Button_Rectangle();
            this.buttonC = new GUI_Controls.Button_Rectangle();
            this.button5 = new GUI_Controls.Button_Rectangle();
            this.buttonT = new GUI_Controls.Button_Rectangle();
            this.buttonF = new GUI_Controls.Button_Rectangle();
            this.buttonV = new GUI_Controls.Button_Rectangle();
            this.button6 = new GUI_Controls.Button_Rectangle();
            this.buttonY = new GUI_Controls.Button_Rectangle();
            this.buttonG = new GUI_Controls.Button_Rectangle();
            this.buttonB = new GUI_Controls.Button_Rectangle();
            this.button7 = new GUI_Controls.Button_Rectangle();
            this.buttonU = new GUI_Controls.Button_Rectangle();
            this.buttonH = new GUI_Controls.Button_Rectangle();
            this.buttonN = new GUI_Controls.Button_Rectangle();
            this.button8 = new GUI_Controls.Button_Rectangle();
            this.buttonI = new GUI_Controls.Button_Rectangle();
            this.buttonJ = new GUI_Controls.Button_Rectangle();
            this.buttonM = new GUI_Controls.Button_Rectangle();
            this.button9 = new GUI_Controls.Button_Rectangle();
            this.buttonO = new GUI_Controls.Button_Rectangle();
            this.buttonK = new GUI_Controls.Button_Rectangle();
            this.buttonDot = new GUI_Controls.Button_Rectangle();
            this.button0 = new GUI_Controls.Button_Rectangle();
            this.buttonP = new GUI_Controls.Button_Rectangle();
            this.buttonL = new GUI_Controls.Button_Rectangle();
            this.button_ = new GUI_Controls.Button_Rectangle();
            this.button_space = new GUI_Controls.Button_Rectangle();
            this.buttonShift = new GUI_Controls.Button_Rectangle();
            this.buttonEnter = new GUI_Controls.Button_Rectangle();
            this.buttonDel = new GUI_Controls.Button_Rectangle();
            this.buttonCaps = new GUI_Controls.Button_Rectangle();
            this.buttonClear = new GUI_Controls.Button_Rectangle();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textbox_input
            // 
            this.textbox_input.AllowDrop = true;
            this.textbox_input.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.textbox_input.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.textbox_input.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textbox_input.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textbox_input.Location = new System.Drawing.Point(21, 54);
            this.textbox_input.Name = "textbox_input";
            this.textbox_input.Size = new System.Drawing.Size(433, 32);
            this.textbox_input.TabIndex = 0;
            this.textbox_input.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textbox_input_KeyDown);
            // 
            // rectangleShape1
            // 
            this.rectangleShape1.BorderWidth = 2;
            this.rectangleShape1.Location = new System.Drawing.Point(0, 0);
            this.rectangleShape1.Name = "rectangleShape1";
            this.rectangleShape1.Size = new System.Drawing.Size(588, 384);
            this.rectangleShape1.Click += new System.EventHandler(this.button_space_Click);
            // 
            // shapeContainer1
            // 
            this.shapeContainer1.Location = new System.Drawing.Point(0, 0);
            this.shapeContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.shapeContainer1.Name = "shapeContainer1";
            this.shapeContainer1.Shapes.AddRange(new Microsoft.VisualBasic.PowerPacks.Shape[] {
            this.rectangleShape1});
            this.shapeContainer1.Size = new System.Drawing.Size(589, 385);
            this.shapeContainer1.TabIndex = 98;
            this.shapeContainer1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::GUI_Controls.Properties.Resources.MessagePanelHeader_BG;
            this.panel1.Controls.Add(this.label_WindowTitle);
            this.panel1.Controls.Add(this.button_Cancel1);
            this.panel1.Location = new System.Drawing.Point(1, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(586, 37);
            this.panel1.TabIndex = 97;
            // 
            // label_WindowTitle
            // 
            this.label_WindowTitle.BackColor = System.Drawing.Color.Transparent;
            this.label_WindowTitle.Font = new System.Drawing.Font("Arial", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_WindowTitle.ForeColor = System.Drawing.Color.White;
            this.label_WindowTitle.Location = new System.Drawing.Point(6, 5);
            this.label_WindowTitle.Margin = new System.Windows.Forms.Padding(0);
            this.label_WindowTitle.Name = "label_WindowTitle";
            this.label_WindowTitle.Size = new System.Drawing.Size(188, 27);
            this.label_WindowTitle.TabIndex = 3;
            this.label_WindowTitle.Text = "Keyboard";
            this.label_WindowTitle.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // button_Cancel1
            // 
            this.button_Cancel1.AccessibleName = "  ";
            this.button_Cancel1.BackColor = System.Drawing.Color.Transparent;
            this.button_Cancel1.BackgroundImage = global::GUI_Controls.Properties.Resources.WindowClose_STD;
            this.button_Cancel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button_Cancel1.Check = false;
            this.button_Cancel1.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Cancel1.ForeColor = System.Drawing.Color.White;
            this.button_Cancel1.Location = new System.Drawing.Point(550, 3);
            this.button_Cancel1.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.button_Cancel1.Name = "button_Cancel1";
            this.button_Cancel1.Size = new System.Drawing.Size(33, 33);
            this.button_Cancel1.TabIndex = 96;
            this.button_Cancel1.Click += new System.EventHandler(this.button_Cancel1_Click);
            // 
            // button1
            // 
            this.button1.AccessibleName = "1";
            this.button1.BackColor = System.Drawing.Color.Transparent;
            this.button1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button1.BackgroundImage")));
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button1.Check = false;
            this.button1.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button1.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button1.Location = new System.Drawing.Point(6, 102);
            this.button1.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(48, 50);
            this.button1.TabIndex = 271;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonQ
            // 
            this.buttonQ.AccessibleName = "Q";
            this.buttonQ.BackColor = System.Drawing.Color.Transparent;
            this.buttonQ.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonQ.BackgroundImage")));
            this.buttonQ.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonQ.Check = false;
            this.buttonQ.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonQ.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonQ.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonQ.Location = new System.Drawing.Point(5, 157);
            this.buttonQ.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonQ.Name = "buttonQ";
            this.buttonQ.Size = new System.Drawing.Size(48, 50);
            this.buttonQ.TabIndex = 272;
            this.buttonQ.Click += new System.EventHandler(this.buttonQ_Click);
            // 
            // button2
            // 
            this.button2.AccessibleName = "2";
            this.button2.BackColor = System.Drawing.Color.Transparent;
            this.button2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button2.BackgroundImage")));
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button2.Check = false;
            this.button2.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button2.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button2.Location = new System.Drawing.Point(56, 102);
            this.button2.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(48, 50);
            this.button2.TabIndex = 267;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // buttonW
            // 
            this.buttonW.AccessibleName = "W";
            this.buttonW.BackColor = System.Drawing.Color.Transparent;
            this.buttonW.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonW.BackgroundImage")));
            this.buttonW.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonW.Check = false;
            this.buttonW.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonW.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonW.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonW.Location = new System.Drawing.Point(55, 157);
            this.buttonW.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonW.Name = "buttonW";
            this.buttonW.Size = new System.Drawing.Size(48, 50);
            this.buttonW.TabIndex = 268;
            this.buttonW.Click += new System.EventHandler(this.buttonW_Click);
            // 
            // buttonA
            // 
            this.buttonA.AccessibleName = "A";
            this.buttonA.BackColor = System.Drawing.Color.Transparent;
            this.buttonA.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonA.BackgroundImage")));
            this.buttonA.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonA.Check = false;
            this.buttonA.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonA.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonA.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonA.Location = new System.Drawing.Point(25, 212);
            this.buttonA.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonA.Name = "buttonA";
            this.buttonA.Size = new System.Drawing.Size(48, 50);
            this.buttonA.TabIndex = 269;
            this.buttonA.Click += new System.EventHandler(this.buttonA_Click);
            // 
            // buttonZ
            // 
            this.buttonZ.AccessibleName = "Z";
            this.buttonZ.BackColor = System.Drawing.Color.Transparent;
            this.buttonZ.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonZ.BackgroundImage")));
            this.buttonZ.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonZ.Check = false;
            this.buttonZ.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonZ.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonZ.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonZ.Location = new System.Drawing.Point(47, 267);
            this.buttonZ.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonZ.Name = "buttonZ";
            this.buttonZ.Size = new System.Drawing.Size(48, 50);
            this.buttonZ.TabIndex = 266;
            this.buttonZ.Click += new System.EventHandler(this.buttonZ_Click);
            // 
            // button3
            // 
            this.button3.AccessibleName = "3";
            this.button3.BackColor = System.Drawing.Color.Transparent;
            this.button3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button3.BackgroundImage")));
            this.button3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button3.Check = false;
            this.button3.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button3.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button3.Location = new System.Drawing.Point(106, 102);
            this.button3.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(48, 50);
            this.button3.TabIndex = 263;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // buttonE
            // 
            this.buttonE.AccessibleName = "E";
            this.buttonE.BackColor = System.Drawing.Color.Transparent;
            this.buttonE.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonE.BackgroundImage")));
            this.buttonE.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonE.Check = false;
            this.buttonE.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonE.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonE.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonE.Location = new System.Drawing.Point(105, 157);
            this.buttonE.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonE.Name = "buttonE";
            this.buttonE.Size = new System.Drawing.Size(48, 50);
            this.buttonE.TabIndex = 264;
            this.buttonE.Click += new System.EventHandler(this.buttonE_Click);
            // 
            // buttonS
            // 
            this.buttonS.AccessibleName = "S";
            this.buttonS.BackColor = System.Drawing.Color.Transparent;
            this.buttonS.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonS.BackgroundImage")));
            this.buttonS.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonS.Check = false;
            this.buttonS.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonS.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonS.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonS.Location = new System.Drawing.Point(75, 212);
            this.buttonS.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonS.Name = "buttonS";
            this.buttonS.Size = new System.Drawing.Size(48, 50);
            this.buttonS.TabIndex = 265;
            this.buttonS.Click += new System.EventHandler(this.buttonS_Click);
            // 
            // buttonX
            // 
            this.buttonX.AccessibleName = "X";
            this.buttonX.BackColor = System.Drawing.Color.Transparent;
            this.buttonX.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonX.BackgroundImage")));
            this.buttonX.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonX.Check = false;
            this.buttonX.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonX.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonX.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonX.Location = new System.Drawing.Point(97, 267);
            this.buttonX.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonX.Name = "buttonX";
            this.buttonX.Size = new System.Drawing.Size(48, 50);
            this.buttonX.TabIndex = 262;
            this.buttonX.Click += new System.EventHandler(this.buttonX_Click);
            // 
            // button4
            // 
            this.button4.AccessibleName = "4";
            this.button4.BackColor = System.Drawing.Color.Transparent;
            this.button4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button4.BackgroundImage")));
            this.button4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button4.Check = false;
            this.button4.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button4.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button4.Location = new System.Drawing.Point(156, 102);
            this.button4.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(48, 50);
            this.button4.TabIndex = 259;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // buttonR
            // 
            this.buttonR.AccessibleName = "R";
            this.buttonR.BackColor = System.Drawing.Color.Transparent;
            this.buttonR.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonR.BackgroundImage")));
            this.buttonR.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonR.Check = false;
            this.buttonR.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonR.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonR.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonR.Location = new System.Drawing.Point(155, 157);
            this.buttonR.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonR.Name = "buttonR";
            this.buttonR.Size = new System.Drawing.Size(48, 50);
            this.buttonR.TabIndex = 260;
            this.buttonR.Click += new System.EventHandler(this.buttonR_Click);
            // 
            // buttonD
            // 
            this.buttonD.AccessibleName = "D";
            this.buttonD.BackColor = System.Drawing.Color.Transparent;
            this.buttonD.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonD.BackgroundImage")));
            this.buttonD.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonD.Check = false;
            this.buttonD.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonD.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonD.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonD.Location = new System.Drawing.Point(125, 212);
            this.buttonD.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonD.Name = "buttonD";
            this.buttonD.Size = new System.Drawing.Size(48, 50);
            this.buttonD.TabIndex = 261;
            this.buttonD.Click += new System.EventHandler(this.buttonD_Click);
            // 
            // buttonC
            // 
            this.buttonC.AccessibleName = "C";
            this.buttonC.BackColor = System.Drawing.Color.Transparent;
            this.buttonC.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonC.BackgroundImage")));
            this.buttonC.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonC.Check = false;
            this.buttonC.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonC.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonC.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonC.Location = new System.Drawing.Point(147, 267);
            this.buttonC.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonC.Name = "buttonC";
            this.buttonC.Size = new System.Drawing.Size(48, 50);
            this.buttonC.TabIndex = 258;
            this.buttonC.Click += new System.EventHandler(this.buttonC_Click);
            // 
            // button5
            // 
            this.button5.AccessibleName = "5";
            this.button5.BackColor = System.Drawing.Color.Transparent;
            this.button5.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button5.BackgroundImage")));
            this.button5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button5.Check = false;
            this.button5.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button5.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button5.Location = new System.Drawing.Point(206, 102);
            this.button5.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(48, 50);
            this.button5.TabIndex = 255;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // buttonT
            // 
            this.buttonT.AccessibleName = "T";
            this.buttonT.BackColor = System.Drawing.Color.Transparent;
            this.buttonT.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonT.BackgroundImage")));
            this.buttonT.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonT.Check = false;
            this.buttonT.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonT.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonT.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonT.Location = new System.Drawing.Point(205, 157);
            this.buttonT.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonT.Name = "buttonT";
            this.buttonT.Size = new System.Drawing.Size(48, 50);
            this.buttonT.TabIndex = 256;
            this.buttonT.Click += new System.EventHandler(this.buttonT_Click);
            // 
            // buttonF
            // 
            this.buttonF.AccessibleName = "F";
            this.buttonF.BackColor = System.Drawing.Color.Transparent;
            this.buttonF.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonF.BackgroundImage")));
            this.buttonF.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonF.Check = false;
            this.buttonF.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonF.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonF.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonF.Location = new System.Drawing.Point(175, 212);
            this.buttonF.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonF.Name = "buttonF";
            this.buttonF.Size = new System.Drawing.Size(48, 50);
            this.buttonF.TabIndex = 257;
            this.buttonF.Click += new System.EventHandler(this.buttonF_Click);
            // 
            // buttonV
            // 
            this.buttonV.AccessibleName = "V";
            this.buttonV.BackColor = System.Drawing.Color.Transparent;
            this.buttonV.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonV.BackgroundImage")));
            this.buttonV.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonV.Check = false;
            this.buttonV.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonV.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonV.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonV.Location = new System.Drawing.Point(197, 267);
            this.buttonV.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonV.Name = "buttonV";
            this.buttonV.Size = new System.Drawing.Size(48, 50);
            this.buttonV.TabIndex = 254;
            this.buttonV.Click += new System.EventHandler(this.buttonV_Click);
            // 
            // button6
            // 
            this.button6.AccessibleName = "6";
            this.button6.BackColor = System.Drawing.Color.Transparent;
            this.button6.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button6.BackgroundImage")));
            this.button6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button6.Check = false;
            this.button6.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button6.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button6.Location = new System.Drawing.Point(256, 102);
            this.button6.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(48, 50);
            this.button6.TabIndex = 251;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // buttonY
            // 
            this.buttonY.AccessibleName = "Y";
            this.buttonY.BackColor = System.Drawing.Color.Transparent;
            this.buttonY.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonY.BackgroundImage")));
            this.buttonY.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonY.Check = false;
            this.buttonY.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonY.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonY.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonY.Location = new System.Drawing.Point(255, 157);
            this.buttonY.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonY.Name = "buttonY";
            this.buttonY.Size = new System.Drawing.Size(48, 50);
            this.buttonY.TabIndex = 252;
            this.buttonY.Click += new System.EventHandler(this.buttonY_Click);
            // 
            // buttonG
            // 
            this.buttonG.AccessibleName = "G";
            this.buttonG.BackColor = System.Drawing.Color.Transparent;
            this.buttonG.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonG.BackgroundImage")));
            this.buttonG.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonG.Check = false;
            this.buttonG.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonG.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonG.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonG.Location = new System.Drawing.Point(225, 212);
            this.buttonG.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonG.Name = "buttonG";
            this.buttonG.Size = new System.Drawing.Size(48, 50);
            this.buttonG.TabIndex = 253;
            this.buttonG.Click += new System.EventHandler(this.buttonG_Click);
            // 
            // buttonB
            // 
            this.buttonB.AccessibleName = "B";
            this.buttonB.BackColor = System.Drawing.Color.Transparent;
            this.buttonB.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonB.BackgroundImage")));
            this.buttonB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonB.Check = false;
            this.buttonB.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonB.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonB.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonB.Location = new System.Drawing.Point(247, 267);
            this.buttonB.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonB.Name = "buttonB";
            this.buttonB.Size = new System.Drawing.Size(48, 50);
            this.buttonB.TabIndex = 250;
            this.buttonB.Click += new System.EventHandler(this.buttonB_Click);
            // 
            // button7
            // 
            this.button7.AccessibleName = "7";
            this.button7.BackColor = System.Drawing.Color.Transparent;
            this.button7.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button7.BackgroundImage")));
            this.button7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button7.Check = false;
            this.button7.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button7.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button7.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button7.Location = new System.Drawing.Point(306, 102);
            this.button7.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(48, 50);
            this.button7.TabIndex = 247;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // buttonU
            // 
            this.buttonU.AccessibleName = "U";
            this.buttonU.BackColor = System.Drawing.Color.Transparent;
            this.buttonU.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonU.BackgroundImage")));
            this.buttonU.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonU.Check = false;
            this.buttonU.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonU.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonU.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonU.Location = new System.Drawing.Point(305, 157);
            this.buttonU.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonU.Name = "buttonU";
            this.buttonU.Size = new System.Drawing.Size(48, 50);
            this.buttonU.TabIndex = 248;
            this.buttonU.Click += new System.EventHandler(this.buttonU_Click);
            // 
            // buttonH
            // 
            this.buttonH.AccessibleName = "H";
            this.buttonH.BackColor = System.Drawing.Color.Transparent;
            this.buttonH.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonH.BackgroundImage")));
            this.buttonH.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonH.Check = false;
            this.buttonH.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonH.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonH.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonH.Location = new System.Drawing.Point(275, 212);
            this.buttonH.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonH.Name = "buttonH";
            this.buttonH.Size = new System.Drawing.Size(48, 50);
            this.buttonH.TabIndex = 249;
            this.buttonH.Click += new System.EventHandler(this.buttonH_Click);
            // 
            // buttonN
            // 
            this.buttonN.AccessibleName = "N";
            this.buttonN.BackColor = System.Drawing.Color.Transparent;
            this.buttonN.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonN.BackgroundImage")));
            this.buttonN.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonN.Check = false;
            this.buttonN.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonN.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonN.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonN.Location = new System.Drawing.Point(297, 267);
            this.buttonN.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonN.Name = "buttonN";
            this.buttonN.Size = new System.Drawing.Size(48, 50);
            this.buttonN.TabIndex = 246;
            this.buttonN.Click += new System.EventHandler(this.buttonN_Click);
            // 
            // button8
            // 
            this.button8.AccessibleName = "8";
            this.button8.BackColor = System.Drawing.Color.Transparent;
            this.button8.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button8.BackgroundImage")));
            this.button8.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button8.Check = false;
            this.button8.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button8.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button8.Location = new System.Drawing.Point(356, 102);
            this.button8.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(48, 50);
            this.button8.TabIndex = 243;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // buttonI
            // 
            this.buttonI.AccessibleName = "I";
            this.buttonI.BackColor = System.Drawing.Color.Transparent;
            this.buttonI.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonI.BackgroundImage")));
            this.buttonI.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonI.Check = false;
            this.buttonI.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonI.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonI.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonI.Location = new System.Drawing.Point(355, 157);
            this.buttonI.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonI.Name = "buttonI";
            this.buttonI.Size = new System.Drawing.Size(48, 50);
            this.buttonI.TabIndex = 244;
            this.buttonI.Click += new System.EventHandler(this.buttonI_Click);
            // 
            // buttonJ
            // 
            this.buttonJ.AccessibleName = "J";
            this.buttonJ.BackColor = System.Drawing.Color.Transparent;
            this.buttonJ.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonJ.BackgroundImage")));
            this.buttonJ.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonJ.Check = false;
            this.buttonJ.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonJ.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonJ.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonJ.Location = new System.Drawing.Point(325, 212);
            this.buttonJ.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonJ.Name = "buttonJ";
            this.buttonJ.Size = new System.Drawing.Size(48, 50);
            this.buttonJ.TabIndex = 245;
            this.buttonJ.Click += new System.EventHandler(this.buttonJ_Click);
            // 
            // buttonM
            // 
            this.buttonM.AccessibleName = "M";
            this.buttonM.BackColor = System.Drawing.Color.Transparent;
            this.buttonM.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonM.BackgroundImage")));
            this.buttonM.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonM.Check = false;
            this.buttonM.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonM.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonM.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonM.Location = new System.Drawing.Point(347, 267);
            this.buttonM.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonM.Name = "buttonM";
            this.buttonM.Size = new System.Drawing.Size(48, 50);
            this.buttonM.TabIndex = 242;
            this.buttonM.Click += new System.EventHandler(this.buttonM_Click);
            // 
            // button9
            // 
            this.button9.AccessibleName = "9";
            this.button9.BackColor = System.Drawing.Color.Transparent;
            this.button9.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button9.BackgroundImage")));
            this.button9.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button9.Check = false;
            this.button9.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button9.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button9.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button9.Location = new System.Drawing.Point(406, 102);
            this.button9.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(48, 50);
            this.button9.TabIndex = 239;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // buttonO
            // 
            this.buttonO.AccessibleName = "O";
            this.buttonO.BackColor = System.Drawing.Color.Transparent;
            this.buttonO.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonO.BackgroundImage")));
            this.buttonO.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonO.Check = false;
            this.buttonO.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonO.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonO.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonO.Location = new System.Drawing.Point(405, 157);
            this.buttonO.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonO.Name = "buttonO";
            this.buttonO.Size = new System.Drawing.Size(48, 50);
            this.buttonO.TabIndex = 240;
            this.buttonO.Click += new System.EventHandler(this.buttonO_Click);
            // 
            // buttonK
            // 
            this.buttonK.AccessibleName = "K";
            this.buttonK.BackColor = System.Drawing.Color.Transparent;
            this.buttonK.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonK.BackgroundImage")));
            this.buttonK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonK.Check = false;
            this.buttonK.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonK.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonK.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonK.Location = new System.Drawing.Point(375, 212);
            this.buttonK.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonK.Name = "buttonK";
            this.buttonK.Size = new System.Drawing.Size(48, 50);
            this.buttonK.TabIndex = 241;
            this.buttonK.Click += new System.EventHandler(this.buttonK_Click);
            // 
            // buttonDot
            // 
            this.buttonDot.AccessibleName = ".";
            this.buttonDot.BackColor = System.Drawing.Color.Transparent;
            this.buttonDot.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonDot.BackgroundImage")));
            this.buttonDot.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonDot.Check = false;
            this.buttonDot.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonDot.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDot.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonDot.Location = new System.Drawing.Point(397, 267);
            this.buttonDot.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonDot.Name = "buttonDot";
            this.buttonDot.Size = new System.Drawing.Size(48, 50);
            this.buttonDot.TabIndex = 238;
            this.buttonDot.Click += new System.EventHandler(this.buttonDot_Click);
            // 
            // button0
            // 
            this.button0.AccessibleName = "0";
            this.button0.BackColor = System.Drawing.Color.Transparent;
            this.button0.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button0.BackgroundImage")));
            this.button0.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button0.Check = false;
            this.button0.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button0.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button0.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button0.Location = new System.Drawing.Point(456, 102);
            this.button0.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.button0.Name = "button0";
            this.button0.Size = new System.Drawing.Size(48, 50);
            this.button0.TabIndex = 235;
            this.button0.Click += new System.EventHandler(this.button0_Click);
            // 
            // buttonP
            // 
            this.buttonP.AccessibleName = "P";
            this.buttonP.BackColor = System.Drawing.Color.Transparent;
            this.buttonP.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonP.BackgroundImage")));
            this.buttonP.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonP.Check = false;
            this.buttonP.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonP.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonP.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonP.Location = new System.Drawing.Point(455, 157);
            this.buttonP.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonP.Name = "buttonP";
            this.buttonP.Size = new System.Drawing.Size(48, 50);
            this.buttonP.TabIndex = 236;
            this.buttonP.Click += new System.EventHandler(this.buttonP_Click);
            // 
            // buttonL
            // 
            this.buttonL.AccessibleName = "L";
            this.buttonL.BackColor = System.Drawing.Color.Transparent;
            this.buttonL.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonL.BackgroundImage")));
            this.buttonL.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonL.Check = false;
            this.buttonL.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonL.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonL.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonL.Location = new System.Drawing.Point(425, 212);
            this.buttonL.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonL.Name = "buttonL";
            this.buttonL.Size = new System.Drawing.Size(48, 50);
            this.buttonL.TabIndex = 237;
            this.buttonL.Click += new System.EventHandler(this.buttonL_Click);
            // 
            // button_
            // 
            this.button_.AccessibleName = "_";
            this.button_.BackColor = System.Drawing.Color.Transparent;
            this.button_.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_.BackgroundImage")));
            this.button_.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button_.Check = false;
            this.button_.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button_.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button_.Location = new System.Drawing.Point(447, 267);
            this.button_.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.button_.Name = "button_";
            this.button_.Size = new System.Drawing.Size(48, 50);
            this.button_.TabIndex = 234;
            this.button_.Click += new System.EventHandler(this.button__Click);
            // 
            // button_space
            // 
            this.button_space.AccessibleName = "";
            this.button_space.BackColor = System.Drawing.Color.Transparent;
            this.button_space.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button_space.BackgroundImage")));
            this.button_space.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_space.Check = false;
            this.button_space.DialogResult = System.Windows.Forms.DialogResult.None;
            this.button_space.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_space.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button_space.Location = new System.Drawing.Point(78, 323);
            this.button_space.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.button_space.Name = "button_space";
            this.button_space.Size = new System.Drawing.Size(450, 50);
            this.button_space.TabIndex = 226;
            this.button_space.Click += new System.EventHandler(this.button_space_Click);
            // 
            // buttonShift
            // 
            this.buttonShift.AccessibleName = "Shift";
            this.buttonShift.BackColor = System.Drawing.Color.Transparent;
            this.buttonShift.BackgroundImage = global::GUI_Controls.Properties.Resources.btnShift_STD;
            this.buttonShift.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.buttonShift.Check = false;
            this.buttonShift.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonShift.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonShift.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonShift.Location = new System.Drawing.Point(497, 267);
            this.buttonShift.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonShift.Name = "buttonShift";
            this.buttonShift.Size = new System.Drawing.Size(83, 50);
            this.buttonShift.TabIndex = 186;
            this.buttonShift.Click += new System.EventHandler(this.buttonShift_Click);
            // 
            // buttonEnter
            // 
            this.buttonEnter.AccessibleName = "ENTER";
            this.buttonEnter.BackColor = System.Drawing.Color.Transparent;
            this.buttonEnter.BackgroundImage = global::GUI_Controls.Properties.Resources.btnEnter_STD;
            this.buttonEnter.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.buttonEnter.Check = false;
            this.buttonEnter.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonEnter.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonEnter.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonEnter.Location = new System.Drawing.Point(475, 212);
            this.buttonEnter.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonEnter.Name = "buttonEnter";
            this.buttonEnter.Size = new System.Drawing.Size(105, 50);
            this.buttonEnter.TabIndex = 185;
            this.buttonEnter.Click += new System.EventHandler(this.buttonEnter_Click);
            // 
            // buttonDel
            // 
            this.buttonDel.AccessibleName = "DEL";
            this.buttonDel.BackColor = System.Drawing.Color.Transparent;
            this.buttonDel.BackgroundImage = global::GUI_Controls.Properties.Resources.btnCaps_STD;
            this.buttonDel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.buttonDel.Check = false;
            this.buttonDel.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonDel.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonDel.Location = new System.Drawing.Point(507, 102);
            this.buttonDel.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonDel.Name = "buttonDel";
            this.buttonDel.Size = new System.Drawing.Size(72, 50);
            this.buttonDel.TabIndex = 183;
            this.buttonDel.Click += new System.EventHandler(this.buttonDel_Click);
            // 
            // buttonCaps
            // 
            this.buttonCaps.AccessibleName = "CAPS";
            this.buttonCaps.BackColor = System.Drawing.Color.Transparent;
            this.buttonCaps.BackgroundImage = global::GUI_Controls.Properties.Resources.btnCaps_STD;
            this.buttonCaps.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.buttonCaps.Check = false;
            this.buttonCaps.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonCaps.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCaps.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonCaps.Location = new System.Drawing.Point(507, 157);
            this.buttonCaps.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonCaps.Name = "buttonCaps";
            this.buttonCaps.Size = new System.Drawing.Size(72, 50);
            this.buttonCaps.TabIndex = 184;
            this.buttonCaps.Click += new System.EventHandler(this.buttonCaps_Click);
            // 
            // buttonClear
            // 
            this.buttonClear.AccessibleName = "CLEAR";
            this.buttonClear.BackColor = System.Drawing.Color.Transparent;
            this.buttonClear.BackgroundImage = global::GUI_Controls.Properties.Resources.btnEnter_STD;
            this.buttonClear.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.buttonClear.Check = false;
            this.buttonClear.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonClear.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonClear.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonClear.Location = new System.Drawing.Point(474, 46);
            this.buttonClear.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(105, 50);
            this.buttonClear.TabIndex = 187;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // Keyboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(589, 385);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonQ);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.buttonW);
            this.Controls.Add(this.buttonA);
            this.Controls.Add(this.buttonZ);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.buttonE);
            this.Controls.Add(this.buttonS);
            this.Controls.Add(this.buttonX);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.buttonR);
            this.Controls.Add(this.buttonD);
            this.Controls.Add(this.buttonC);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.buttonT);
            this.Controls.Add(this.buttonF);
            this.Controls.Add(this.buttonV);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.buttonY);
            this.Controls.Add(this.buttonG);
            this.Controls.Add(this.buttonB);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.buttonU);
            this.Controls.Add(this.buttonH);
            this.Controls.Add(this.buttonN);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.buttonI);
            this.Controls.Add(this.buttonJ);
            this.Controls.Add(this.buttonM);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.buttonO);
            this.Controls.Add(this.buttonK);
            this.Controls.Add(this.buttonDot);
            this.Controls.Add(this.button0);
            this.Controls.Add(this.buttonP);
            this.Controls.Add(this.buttonL);
            this.Controls.Add(this.button_);
            this.Controls.Add(this.button_space);
            this.Controls.Add(this.buttonShift);
            this.Controls.Add(this.buttonEnter);
            this.Controls.Add(this.buttonDel);
            this.Controls.Add(this.buttonCaps);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.textbox_input);
            this.Controls.Add(this.shapeContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Keyboard";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Keyboard";
            this.Load += new System.EventHandler(this.Keyboard_Load);
            this.Shown += new System.EventHandler(this.Keyboard_Shown);
            this.LocationChanged += new System.EventHandler(this.Keyboard_LocationChanged);
            this.VisibleChanged += new System.EventHandler(this.Keyboard_VisibleChanged);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textbox_input;
        private Button_Cancel button_Cancel1;
        private System.Windows.Forms.Panel panel1;
        protected System.Windows.Forms.Label label_WindowTitle;
        private Microsoft.VisualBasic.PowerPacks.RectangleShape rectangleShape1;
        private Microsoft.VisualBasic.PowerPacks.ShapeContainer shapeContainer1;
        private Button_Rectangle buttonShift;
        private Button_Rectangle buttonEnter;
        private Button_Rectangle buttonDel;
        private Button_Rectangle buttonCaps;
        private Button_Rectangle buttonClear;
        private Button_Rectangle button_space;
        private Button_Rectangle button_;
        private Button_Rectangle buttonM;
        private Button_Rectangle button9;
        private Button_Rectangle buttonO;
        private Button_Rectangle buttonK;
        private Button_Rectangle buttonDot;
        private Button_Rectangle button0;
        private Button_Rectangle buttonP;
        private Button_Rectangle buttonL;
        private Button_Rectangle buttonC;
        private Button_Rectangle button5;
        private Button_Rectangle buttonT;
        private Button_Rectangle buttonF;
        private Button_Rectangle buttonV;
        private Button_Rectangle button6;
        private Button_Rectangle buttonY;
        private Button_Rectangle buttonG;
        private Button_Rectangle buttonB;
        private Button_Rectangle button7;
        private Button_Rectangle buttonU;
        private Button_Rectangle buttonH;
        private Button_Rectangle buttonN;
        private Button_Rectangle button8;
        private Button_Rectangle buttonI;
        private Button_Rectangle buttonJ;
        private Button_Rectangle button1;
        private Button_Rectangle buttonQ;
        private Button_Rectangle button2;
        private Button_Rectangle buttonW;
        private Button_Rectangle buttonA;
        private Button_Rectangle buttonZ;
        private Button_Rectangle button3;
        private Button_Rectangle buttonE;
        private Button_Rectangle buttonS;
        private Button_Rectangle buttonX;
        private Button_Rectangle button4;
        private Button_Rectangle buttonR;
        private Button_Rectangle buttonD;
    }
}
