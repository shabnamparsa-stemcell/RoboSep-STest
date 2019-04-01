using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace GUI_Console
{
    // class to manage quadrant interaction / updating
    public partial class QuadrantProgress
    {
        public GUI_Controls.ProgressBar myProgressBar;
        private Rectangle myProgressBarInitialBounds;
        private GUI_Controls.TextBoxScroll myLabelCurrent;
        private GUI_Controls.TextBoxScroll myLabelPrevious;
        private Label P2;
        private Label C2;
        public Label myLabelCompleted;
        private Label myProtocolName;
        //
        public bool isActive;
        
        public string myDetailsCurrentStep;
        public string myDetailsPreviousStep;
        public string myBriefCurrentStep;
        public string myBriefPreviousStep;
        public int currentStepNum;
        public int trackedStepNum;
        private int[] CommandList;
        public int quadrantID;
        private string[] myProcessDescriptions;
        private float defaultFontSize;

        private Size maxSize;
        private bool showDetails = false;

        public enum ProtocolCommand
        {
            IncubateCommand = 0,                      
            MixCommand,
            TransportCommand,
            SeparateCommand,
            ResuspendVialCommand,
            TopUpVialCommand,
            FlushCommand,
            PrimeCommand,
            HomeAllCommand,
            DemoCommand,
            ParkCommand,
            PumpLifeCommand,
            MixTransCommand,
            TopUpMixTransCommand,
            ResusMixSepTransCommand,
            ResusMixCommand,
            TopUpTransCommand,
            TopUpTransSepTransCommand,
            TopUpMixTransSepTransCommand,
            NULL
        }

        public QuadrantProgress(GUI_Controls.ProgressBar pBar, GUI_Controls.TextBoxScroll c, Label c2, GUI_Controls.TextBoxScroll p, Label p2, Label complete, /*Panel back,*/ Label pName)
        {
            myDetailsCurrentStep = string.Empty;
            myDetailsPreviousStep = string.Empty;
            myBriefCurrentStep = string.Empty;
            myBriefPreviousStep = string.Empty;
            isActive = false;
            myProgressBar = pBar;
            if (myProgressBar != null)
            {
                myProgressBarInitialBounds = new Rectangle(pBar.Bounds.X, pBar.Bounds.Y, pBar.Bounds.Width, pBar.Bounds.Height);
            }

            myLabelCurrent = c;
            C2 = c2;
            myLabelPrevious = p;
            P2 = p2;
            myLabelCompleted = complete;
            //myPanel = back;
            currentStepNum = -1;
            trackedStepNum = 0;
            CommandList = myProgressBar.commands;
            myProtocolName = pName;
            defaultFontSize = myLabelCurrent.Font.Size;

            maxSize = new Size(180, 20);   // This is the maximum size of the label. Otherwise, it will extend outside the screen.


            // set label text based on language setting
            IniFile LanguageINI = GUI_Console.RoboSep_UserConsole.getInstance().LanguageINI;
            if (C2 != null && P2 != null & pName != null)
            {
                C2.Text = LanguageINI.GetString("lblCurrent");
                P2.Text = LanguageINI.GetString("lblPrevious");
            }

            if (pName == null)
            {
                C2.Text = LanguageINI.GetString("lblElapsed");
                P2.Text = LanguageINI.GetString("lblEstimated");
            }

            myLabelCompleted.Text = LanguageINI.GetString("lblComplete");
        }

        public string[] ProcessDescriptions
        {
            get
            {
                return myProcessDescriptions;
            }
            set
            {
                myProcessDescriptions = new string[ value.Length ];
                for (int i = 0; i < value.Length; i++)
                    myProcessDescriptions[i] = value[i];
            }
        }

        public string ProtocolName
        {
            get
            {
                if (myProtocolName != null)
                    return myProtocolName.Text;
                return null;
            }
            set
            {
                if (myProtocolName != null)
                {
                    myProtocolName.Visible = true;
                    myProtocolName.Text = value;
                }
            }
        }

        public bool ShowDetails
        {
            set
            {
                showDetails = value;
                myProgressBar.showProgressDetails(showDetails);
                myProgressBar.Refresh();
                Refresh();
            }
        }

        public void Clear()
        {
            myDetailsCurrentStep = string.Empty;
            myDetailsPreviousStep = string.Empty;
            myBriefCurrentStep = string.Empty;
            myBriefPreviousStep = string.Empty;

            if (myProtocolName != null)
            {
                myProtocolName.Text = string.Empty;
                myProtocolName.Visible = false;
            }
            myLabelCompleted.Visible = false;
            myLabelCurrent.Visible = true;
            myLabelPrevious.Visible = true;
            if (C2 != null && P2 != null)
            {
                C2.Visible = true;
                P2.Visible = true;
            }
            isActive = false;
            
            currentStepNum = -1;
            trackedStepNum = 0;
            myProcessDescriptions = null;

            if (myProgressBar != null)
            {
                myProgressBar.SetBounds(myProgressBarInitialBounds.X, myProgressBarInitialBounds.Y, myProgressBarInitialBounds.Width, myProgressBarInitialBounds.Height);
            }
            Refresh();
        }

        public void HideAll()
        {
            myDetailsCurrentStep = string.Empty;
            myDetailsPreviousStep = string.Empty;
            myBriefCurrentStep = string.Empty;
            myBriefPreviousStep = string.Empty;
            if (myProtocolName != null)
            {
                myProtocolName.Text = string.Empty;
                myProtocolName.Visible = false;
            }
            myLabelCompleted.Visible = false;
            myLabelCurrent.Visible = false;
            myLabelPrevious.Visible = false;
            if (C2 != null && P2 != null)
            {
                C2.Visible = false;
                P2.Visible = false;
            }
            isActive = false;

            currentStepNum = -1;
            trackedStepNum = 0;
            myProcessDescriptions = null;
            Refresh();
        }

        public void SetProtocolLabelVisibility(bool bVisible)
        {
            if (myProtocolName != null)
            {
                myProtocolName.Visible = bVisible;
            }
        }

        public void Refresh()
        {
            myLabelCurrent.Text = showDetails ? myDetailsCurrentStep : myBriefCurrentStep;
            myLabelPrevious.Text = showDetails ? myDetailsPreviousStep : myBriefPreviousStep;
            myProgressBar.Visible = isActive;
            if (!isComplete())
            {
                myLabelCurrent.Visible = isActive;
                myLabelPrevious.Visible = isActive;
                if (C2 != null && P2 != null)
                {
                    P2.Visible = isActive;
                    C2.Visible = isActive;
                }
                CommandList = myProgressBar.commands;
            }
        }

        public void Completed()
        {
            if (isActive)
            {
                myLabelCompleted.Visible = true;
                if (C2 != null && P2 != null)
                {
                    C2.Visible = false;
                    P2.Visible = false;
                }
                myLabelCurrent.Visible = false;
                myLabelPrevious.Visible = false;
                myProgressBar.AllCompleted();
            }
        }

        public bool isComplete()
        {
            return myLabelCompleted.Visible;
        }

        public void UpdateStep( int cmdNum )
        {
            //currentToPrevious();
            myBriefCurrentStep = intToCommand( CommandList[cmdNum] );
            myBriefPreviousStep = cmdNum > 0 ? intToCommand(CommandList[cmdNum-1]) : "Initializing";
            myProgressBar.MoveToStep( cmdNum );
            currentStepNum = cmdNum;
            Refresh();
        }

        public void UpdateStep2(int stepNum, int cmdNum)
        {
            if (myProcessDescriptions != null && myProcessDescriptions.Length > 0 && 0 <= stepNum && stepNum <= myProcessDescriptions.Length)
            {
                myDetailsCurrentStep = myProcessDescriptions[stepNum];
                myDetailsPreviousStep = stepNum > 0 ? myProcessDescriptions[stepNum - 1] : "Initializing";
            }

            myBriefCurrentStep = intToCommand(CommandList[cmdNum]);
            myBriefPreviousStep = cmdNum > 0 ? intToCommand(CommandList[cmdNum - 1]) : "Initializing";

            myProgressBar.MoveToStep(cmdNum);
            currentStepNum = cmdNum;
            Refresh();
        }

        public void CompleteStep(  )
        {
            // check if current step is incubate or separate
            // if so, do not complete..
            if (currentStepNum >= 0 &&  CommandList[currentStepNum] != (int)ProtocolCommand.IncubateCommand
                && CommandList[currentStepNum] != (int)ProtocolCommand.SeparateCommand)
            {
                myProgressBar.CompleteCurrentStep();
                //currentToPrevious();
                Refresh();
            }
        }

        public bool isNextStep( ProtocolCommand cmd)
        {
            if (currentStepNum + 1 < CommandList.Length)
            {
                if (CommandList[currentStepNum + 1] == (int)cmd || currentStepNum < 1)
                {
                    return true;
                }
                // if separated by a mix or transport command.  ignore trans or mix command
                else if (currentStepNum + 2 < CommandList.Length - 1)
                {
                    if ((CommandList[currentStepNum + 1] == (int)ProtocolCommand.TransportCommand ||
                     CommandList[currentStepNum + 1] == (int)ProtocolCommand.TransportCommand)
                    && CommandList[currentStepNum + 2] == (int)cmd)
                        return true;
                }
            }
            return false;
        }

        public bool isNextStepTransport(string command)
        {
            int nextStep = ConvertTointCommand(command);
            try
            {
                if (currentStepNum == 0 || CommandList[currentStepNum] == 2)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool onTimedAction()
        {
            if (currentStepNum > 1)
            {
                //int thisStep = currentStepNum != 0 ? currentStepNum - 1 : 0;
                if (CommandList[currentStepNum] == 0 || CommandList[currentStepNum] == 3)
                {
                    // if element progress is above 80 percent consider the event completed
                    if (myProgressBar.elementProgress < 85)
                        return true;
                }
            }
            return false;
        }

        public bool nextStepisCommand(string Command)
        {                        
            int commandInt = ConvertTointCommand(Command);
            for (int i = currentStepNum; i < currentStepNum + 1 && i < CommandList.Length; i++)
            {
                if (CommandList[i] == commandInt)
                    return true;
            }
            return false;
        }

        private int ConvertTointCommand(string command)
        {
            switch (command)
            {
                case "Incubate":
                    return 0;
                case "Mix":
                    return 1;
                case "Transport":
                    return 2;
                case "Separate":
                    return 3;
                case "Resuspend":
                    return 4;
                case "TopUp Vial":
                    return 5;
                case "Flush":
                    return 6;
                case "Prime":
                    return 7;
                case "Home All":
                    return 8;
                case "Demo":
                    return 9;
                case "Park":
                    return 10;
                case "PumpLife":
                    return 11;
                case "Mix Transport":
                    return 12;
                case "TopUp Mix Transport":
                    return 13;
                case "Resuspend Mix Separate Transport":
                    return 14;
                case "Resuspend Mix":
                    return 15;
                case "TopUp Transport":
                    return 16;
                case "TopUp Transport Separate Transport":
                    return 17;
                case "TopUp Mix Transport Separate Transport":
                    return 18;

            }
            return -1;
        }

        private string intToCommand(int cmd)
        {
            switch (cmd)
            {
                case 0:
                    return "Incubate";
                case 1:
                    return "Mix";
                case 2:
                    return "Transport";
                case 3:
                    return "Separate";
                case 4:
                    return "Resuspend";
                case 5:
                    return "TopUp Vial";
                case 6:
                    return "Flush";
                case 7:
                    return "Prime";
                case 8:
                    return "Home All";
                case 9:
                    return "Demo";
                case 10:
                    return "Park";
                case 11:
                    return "PumpLife";
                case 12:
                    return "Mix Transport";
                case 13:
                    return "TopUp Mix Transport";
                case 14:
                    return "Resuspend Mix Separate Transport";
                case 15:
                    return "Resuspend Mix";
                case 16:
                    return "TopUp Transport";
                case 17:
                    return "TopUp Transport Separate Transport";
                case 18:
                    return "TopUp Mix Transport Separate Transport";
            }
            return string.Empty;
        }

        public string[] getCommandList()
        {
            string[] cmdList = new string[CommandList.Length];

            for (int i = 0; i < cmdList.Length; i++)
            {
                ProtocolCommand mycmd = (ProtocolCommand)CommandList[i];
                string comand = mycmd.ToString();
                string temp = string.Empty;
                for (int j = 0; j < comand.Length - 7; j++)
                {
                    temp += comand[j];
                }
                cmdList[i] = temp;
            }
            return cmdList;
        }

        public static float BestFontSize(Font f, float defaultFontSize, string s, Size fitTo)
        {
            if (string.IsNullOrEmpty(s) || f == null)
                return -1.0f;

            TextFormatFlags format = TextFormatFlags.SingleLine;
            Size p = new Size(fitTo.Width, int.MaxValue);

            Size textSize;
            Font newFont;
            float fontsize;
            if (f.Size < defaultFontSize)
            {
                fontsize = defaultFontSize;
                newFont = new Font(f.FontFamily, fontsize, f.Style);
                textSize = TextRenderer.MeasureText(s, newFont, p, format);
                newFont.Dispose();
                newFont = null;
            }
            else
            {
                fontsize = f.Size;
                textSize = TextRenderer.MeasureText(s, f, p, format);
            }

            if (textSize.Width <= fitTo.Width)
                return fontsize;
            else
            {
                do
                {
                    fontsize -= 0.5f;
                    newFont = new Font(f.FontFamily, fontsize, f.Style);

                    //And check again
                    textSize = TextRenderer.MeasureText(s, newFont, p, format);

                    newFont.Dispose();
                    newFont = null;

                    if (textSize.Width <= fitTo.Width)
                        return fontsize;

                } while (fitTo.Width <= textSize.Width);

                return fontsize;
            }
        }
    }
}