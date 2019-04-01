using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GUI_Console
{
    // class to manage quadrant interaction / updating
    public partial class QuadrantProgress
    {
        public GUI_Controls.ProgressBar myProgressBar;
        private Label myLabelCurrent;
        private Label myLabelPrevious;
        private Label P2;
        private Label C2;
        public Label myLabelCompleted;
        private Panel myPanel;
        //
        public bool isActive;
        public string myCurrentStep;
        public string myPreviousStep;
        public int currentStepNum;
        public int trackedStepNum;
        private int[] CommandList;
        public int quadrantID;
        private string[] myProcessDescriptions;

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
            NULL
        }


        public QuadrantProgress(GUI_Controls.ProgressBar pBar, Label c, Label c2, Label p, Label p2, Label complete, Panel back)
        {
            myCurrentStep = string.Empty;
            myPreviousStep = string.Empty;
            isActive = false;
            myProgressBar = pBar;
            myLabelCurrent = c;
            C2 = c2;
            myLabelPrevious = p;
            P2 = p2;
            myLabelCompleted = complete;
            myPanel = back;
            currentStepNum = -1;
            trackedStepNum = 0;
            CommandList = myProgressBar.commands;
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

        public void Clear()
        {
            myCurrentStep = string.Empty;
            myPreviousStep = string.Empty;
            myLabelCompleted.Visible = false;
            myLabelCurrent.Visible = true;
            myLabelPrevious.Visible = true;
            C2.Visible = true;
            P2.Visible = true;
            isActive = false;
            currentStepNum = -1;
            trackedStepNum = 0;
            myProcessDescriptions = null;
            Refresh();
        }

        public void Refresh()
        {
            myLabelCurrent.Text = myCurrentStep;
            myLabelPrevious.Text = myPreviousStep;
            myPanel.Visible = isActive;
            myProgressBar.Visible = isActive;
            if (!isComplete())
            {
                myLabelCurrent.Visible = isActive;
                myLabelPrevious.Visible = isActive;
                P2.Visible = isActive;
                C2.Visible = isActive;
                CommandList = myProgressBar.commands;
            }
        }

        public void Completed()
        {
            if (isActive)
            {
                myLabelCompleted.Visible = true;
                C2.Visible = false;
                P2.Visible = false;
                myLabelCurrent.Visible = false;
                myLabelPrevious.Visible = false;
                myProgressBar.AllCompleted();
            }
        }

        public bool isComplete()
        {
            return myLabelCompleted.Visible;
        }

        public void NextStep(string Command, int Step)
        {
            if (myCurrentStep != string.Empty)
                myPreviousStep = myCurrentStep;
            myCurrentStep = Command;
            Refresh();

            currentStepNum = Step;
            myProgressBar.MoveToStep(currentStepNum);
        }

        public bool ProgressToNextStep(string Command, int Step)
        {
            // check if next step is transport step
            // if it is, possibly skip over it to
            // actualnext step
            bool iterateStep = true;
            int ThisCommand = ConvertTointCommand(Command);
            
            // find next of given command in command list
            int range = 3;
            if (ThisCommand == (int)ProtocolCommand.IncubateCommand || ThisCommand == (int)ProtocolCommand.SeparateCommand)
                range = 8;
            if (ThisCommand == (int)ProtocolCommand.MixCommand || ThisCommand == (int)ProtocolCommand.TransportCommand)
            {
                range = 2;
                if (currentStepNum < CommandList.Length)
                {
                    if (CommandList[currentStepNum + 1] == (int)ProtocolCommand.MixCommand ||
                        CommandList[currentStepNum + 1] == (int)ProtocolCommand.TransportCommand)
                        range++;
                }
            }

            for (int i = currentStepNum+1; i < (currentStepNum + range) && i < CommandList.Length; i++)
            {
                iterateStep = false;
                if (ThisCommand == CommandList[i])
                {
                    if (currentStepNum > -1)
                    {
                        // LOG
                        string LogString = "Progress to element:'" + i.ToString() + "' from:'" + currentStepNum + "'";
                        LogString += " cmdGiven: '" + Command.ToString() + "' elementCmdType: '" + ((ProtocolCommand)(CommandList[i])).ToString() + "'";
                        GUI_Controls.uiLog.LOG(this,"ProgressToNextStep" , GUI_Controls.uiLog.LogLevel.DEBUG, LogString);
                    }

                    currentStepNum = i;

                    myProgressBar.MoveToStep(currentStepNum);
                    //currentStepNum++;
                    trackedStepNum++;
                    
                    // update current / previous
                    if (myCurrentStep != string.Empty)
                        myPreviousStep = myCurrentStep;
                    myCurrentStep = Command;
                    iterateStep = true;
                    break;
                }
            }
            // update information
            Refresh();

            if (!iterateStep)
            {
                // LOG
                string logString = "Progress to element: '" + currentStepNum + "'";
                logString += " cmdGiven: '" + Command.ToString() + "' cmdExecuted: 'NONE'";
                GUI_Controls.uiLog.LOG(this, "ProgressToNextStep",  GUI_Controls.uiLog.LogLevel.DEBUG, logString);
            }

            return iterateStep;
        }

        public void currentToPrevious()
        {
            if (myCurrentStep != string.Empty)
                myPreviousStep = myCurrentStep;
            myCurrentStep = string.Empty;
            Refresh();
        }

        public void UpdateStep( int cmdNum )
        {
            //currentToPrevious();
            myCurrentStep = intToCommand( CommandList[cmdNum] );
            myPreviousStep = cmdNum > 0 ? intToCommand(CommandList[cmdNum-1]) : "Initializing";
            myProgressBar.MoveToStep( cmdNum );
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
    }
}