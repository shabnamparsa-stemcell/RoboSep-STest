using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Invetech.ApplicationLog;
namespace GUI_Controls
{
    public partial class ProgressBar : UserControl
    {
        private int numSteps;
        private int currentStep = 0;
        private int estTotalTime;
        private int[] stepTimes;
        private int[] stepColors;
        private const int BORDER = 0;
        private List<ProgressElement> progressBars;
        private int TimerCount = 0;
        private int secs;
        private int mins;
        private int hrs;
        private string elapsedTime = string.Empty; 
        private string percentCompleted = string.Empty;
        private bool OnlyUpdateTime = false;
        private bool ShowElapseTime = false;
        public bool ShowDetails = false;
        
        public enum Steps
        {
            Transport,
            Mix,
            Incubate,
            Dispense
        };

        private Color[,] colours = new Color[2, 12]; 

        public ProgressBar()
        {
            this.DoubleBuffered = true;
            InitializeComponent();
            //SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            numSteps = 20;
            estTotalTime = 0;
            stepTimes = new int[numSteps];
            stepColors = new int[numSteps];
            progressBars = new List<ProgressElement>();
            // generate progress elements = numSteps
            ResetProgress();
            OverlayRectangle.BringToFront();
        }

        public int NumStepsRemaining
        {
            get
            {
                int StepsRemaining = numSteps - (currentStep + 1);
                return StepsRemaining;
            }
        }

        public int Interval
        {
            set
            {
                ProgressTimer.Interval = value;
                for (int i = 0; i < progressBars.Count; i++)
                {
                    progressBars[i].Interval = value;
                }
            }
        }

        public int[] commands
        {
            get
            {
                return stepColors;
            }
        }

        public bool showTime
        {
            set
            {
                ShowElapseTime = value;
            }
        }

        public string elapse
        {
            get
            {
                return elapsedTime;
            }
        }

        public List<ProgressElement> elements
        {
            get
            {
                return progressBars;
            }
        }

        public int progress
        {
            get
            {
                if (percentCompleted != string.Empty)
                {
                    string[] pComplete = percentCompleted.Split('%');
                    int stringToDouble = (int)Convert.ToDouble(pComplete[0]);
                    return stringToDouble;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int elementProgress
        {
            get
            {
                return progressBars[currentStep].elementProgress;
            }
        }

        public void ResetTimer()
        {
            ProgressTimer.Stop();
            hrs = mins = secs = 0;
            currentStep = TimerCount = 0;
            percentCompleted = "0%";
            elapsedTime = "";
        }

        public void ResetProgress()
        {
            // LOG
            string name = this.GetType().Name;
            string logMSG = "Reset Progress on " + name;
            //GUI_Controls.uiLog.LOG(this, "ResetProgress", GUI_Controls.uiLog.LogLevel.DEBUG, logMSG);
            //  (logMSG);
            

            // clear current progress elements
            if (progressBars.Count > 0)
            {
                for (int i = 0; i < progressBars.Count; i++)
                {
                    this.Controls.Remove(progressBars[i]);
                }
                progressBars.Clear();
            }
            currentStep = 0;
            TimerCount = 0;
            

            // generate new progress elements
            if (stepTimes != null)
            {
                estTotalTime = 0;
                for (int i = 0; i < numSteps; i++)
                {
                    estTotalTime += stepTimes[i];
                }
            }
            else
            {
                stepTimes = new int[ numSteps ];
                stepColors = new int[ numSteps ];
                for (int i = 0; i < numSteps; i++)
                {
                    stepTimes[i] = 15;
                    estTotalTime += 15;
                    stepColors[i] = 0;
                }
            }

            double percentAtEndOfStep = 0.000;
            for (int i = 0; i < numSteps; i++)
            {
                //int stepColour = ShowDetails ? stepColors[i] : 0;
                int stepColour = stepColors[i];
                progressBars.Add(new ProgressElement(this, stepTimes[i], stepColour));
                // resize elements
                //int elementWidth = (this.Size.Width - 8 - (SpacingSize * (numSteps - 1))) / numSteps;
                double percentOfElement = (double)stepTimes[i] / (double)estTotalTime;
                percentAtEndOfStep += percentOfElement;
                double posOfEndOfElement = BORDER + ((double)this.Size.Width - BORDER * 2.00) * percentAtEndOfStep;

                if (!ShowDetails)
                {
                    progressBars[i].BorderStyle = BorderStyle.None;
                }

                int posX = BORDER;
                if (i != 0)
                    posX = progressBars[i - 1].Location.X + progressBars[i - 1].Size.Width - 1;
                progressBars[i].Location = new Point(posX, BORDER);

                int width = (int)posOfEndOfElement - progressBars[i].Location.X;
                if (i == numSteps - 1)
                    width = this.Size.Width - BORDER - progressBars[i].Location.X;
                progressBars[i].Size = new Size(width, this.Height - BORDER * 2);
            }

            for (int i = 0; i < numSteps; i++)
            {
                this.Controls.Add(progressBars[i]);
                progressBars[i].SendToBack();
            }

        }

        public void setProgress(int percentTotalComplete)
        {           
            Pause(false);
            //bool TimerSetting = OnlyUpdateTime;
            //OnlyUpdateTime = true;

            //System.Diagnostics.Debug.WriteLine(String.Format(" setProgress: percentTotalComplete={0} ", percentTotalComplete));




            double percent = (double)percentTotalComplete / 100;
            int cyclesToEqualPercent = (int)((double)estTotalTime * percent);
            int cycles = 0;


            //System.Diagnostics.Debug.WriteLine(String.Format(" percent={0}, cyclesToEqualPercent={1}, numSteps={2}.", percent, cyclesToEqualPercent, numSteps));


            for (int i = 0; i < numSteps; i++)
            {
                if ((cycles + stepTimes[i]) < cyclesToEqualPercent)
                {

                    //System.Diagnostics.Debug.WriteLine(String.Format(" cycles 1 ={0}.", cycles));


                    cycles += stepTimes[i];

                    //System.Diagnostics.Debug.WriteLine(String.Format("cycles 2 ={0}.", cycles));

                    progressBars[i].Finished();
                }
                else
                {

                    //System.Diagnostics.Debug.WriteLine(String.Format("cyclesToEqualPercent ={0}, cycles={1}", cyclesToEqualPercent, cycles));


                    int cyclesRemaining = cyclesToEqualPercent - cycles;


                    //System.Diagnostics.Debug.WriteLine(String.Format("cyclesRemaining  ={0}", cyclesRemaining));


                    int percentOfStep = (int)(100.00*(double)cyclesRemaining / (double)stepTimes[i]);

 
                    // set progress element progress status
                    currentStep = i;
                    progressBars[i].setProgress(percentOfStep);


                    //System.Diagnostics.Debug.WriteLine(String.Format("currentStep={0}, percentOfStep={1}", currentStep, percentOfStep));


                    // set tick counter to match percent complete
                    TimerCount = cyclesRemaining;

                    //System.Diagnostics.Debug.WriteLine(String.Format("TimerCount={0}", TimerCount));

                    break;
                }
            }
            Start();

            //OnlyUpdateTime = TimerSetting;
        }

        public void MoveToStep(int completed)
        {
            // LOG
            string name = this.GetType().Name;
            string logMSG = "Progress bar " + name + " to step " + (completed).ToString();
            //GUI_Controls.uiLog.LOG(this, "MoveToStep", GUI_Controls.uiLog.LogLevel.DEBUG, logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);            

            if (completed <= numSteps)
            {
                Pause(false);
                // make sure all steps before current
                // step are set to completed as "completed"
                // is updated to be complete
                if (completed != numSteps)
                {
                    for (int i = 0; i < completed; i++)
                    {
                        progressBars[i].Completed();
                    }
                    currentStep = completed;
                    TimerCount = 0;
                    hrs = mins = secs = 0;
                    progressBars[completed].Start();
                }
                Start();
            }
        }

        public void CompleteCurrentStep()
        {
            progressBars[currentStep].Completed();
        }

        public void AllCompleted()
        {
            //ProgressTimer.Stop();
            OnlyUpdateTime = true;

            //this.setProgress(100);
            for (int i = 0; i < numSteps; i++)
                progressBars[i].Completed();

            percentCompleted = "100%";
            this.Refresh();

            // LOG
            string name = this.GetType().Name;
            string logMSG = name + " COMPLETED! ";
            //GUI_Controls.uiLog.LOG(this, "AllCompleted", GUI_Controls.uiLog.LogLevel.DEBUG, logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);            
        }

        public void Stop()
        {
            OnlyUpdateTime = false;
            setProgress(0);
            currentStep = 0;
            TimerCount = 0;
            ProgressTimer.Stop();
        }

        public void Pause(bool stop)
        {
            //ProgressTimer.Stop();
            OnlyUpdateTime = true;
            if (stop)
                progressBars[currentStep].pause();
        }

        public void Start()
        {
            OnlyUpdateTime = false;
            if (!ProgressTimer.Enabled)
            {
            ProgressTimer.Start();            
            }
            progressBars[currentStep].Start();
        }

        public void setElements(int[] estTimes, int[] steps)
        {
            ResetTimer();
            this.Refresh();
            //Application.DoEvents();
            numSteps = estTimes.Length;
            stepTimes = estTimes;
            stepColors = steps;
            ResetProgress();

            // LOG
            string name = this.GetType().Name;
            string logMSG = name + " set " + estTimes.Length.ToString() + " steps";
            //GUI_Controls.uiLog.LOG(this, "setElements", GUI_Controls.uiLog.LogLevel.DEBUG, logMSG);
            LogFile.AddMessage(System.Diagnostics.TraceLevel.Verbose, logMSG);            
        }

        public void showProgressDetails(bool showDetails)
        {
            this.SuspendLayout();
            for (int elmnt = 0; elmnt < this.progressBars.Count; elmnt++)
            {
                progressBars[elmnt].showElementDetails(showDetails);
            }
            this.ResumeLayout();
        }

        private void ProgressBar_Load(object sender, EventArgs e)
        {
            
        }

        public void StartTimer()
        {
            OnlyUpdateTime = true;
            ProgressTimer.Start();
        }

        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            incrimentTime();

            if (!OnlyUpdateTime)
            {
                // update progress element
                progressBars[currentStep].tick();

                if (TimerCount == stepTimes[currentStep])
                {
                    if (currentStep < (numSteps - 1))
                    {
                        // step has been completed.
                    }
                }
                else
                {
                    TimerCount++;
                }

                // calculate % completed
                int CurrentlyCompleted = 0;
                for (int i = 0; i < currentStep; i++)
                    CurrentlyCompleted += stepTimes[i];
                CurrentlyCompleted += TimerCount;
                double percentComplete = (double)CurrentlyCompleted / (double)estTotalTime;
                string percentCompleteString = string.Format("{0:0}%", percentComplete * 100);

                percentCompleted = percentCompleteString;
            }
            this.Refresh();
        }

        private void incrimentTime()
        {
            secs++;
            if (secs == 60)
            {
                secs = 0;
                mins += 1;
            }

            if (mins == 60)
            {
                mins = 0;
                hrs += 1;
            }
            
            string updateElapsedTime = string.Empty;
            if (hrs > 0)
                updateElapsedTime = hrs.ToString() + ":";
            if (mins < 10)
                updateElapsedTime += "0";
            updateElapsedTime += mins.ToString() + ":";
            if (secs < 10)
                updateElapsedTime += "0";
            updateElapsedTime += secs.ToString();
            // update label;
            elapsedTime = updateElapsedTime;
            //this.SuspendLayout();
            //OverlayRectangle.BringToFront();
            //this.ResumeLayout();
        }

        private void ProgressBar_Resize(object sender, EventArgs e)
        {
            double percentAtEndOfStep = 0.000;
            for (int i = 0; i < numSteps; i++)
            {
                // resize elements
                double percentOfElement = (double)stepTimes[i] / (double)estTotalTime;
                percentAtEndOfStep += percentOfElement;
                double posOfEndOfElement = BORDER*1.000 + ((double)this.Size.Width - BORDER*2.000) * percentAtEndOfStep;

                int posX = BORDER;
                if (i != 0)
                    posX = progressBars[i - 1].Location.X + progressBars[i - 1].Size.Width -1;
                progressBars[i].Location = new Point(posX, BORDER);

                int width = (int)posOfEndOfElement - progressBars[i].Location.X;
                if (i == numSteps - 1)
                    width = this.Size.Width - BORDER - progressBars[i].Location.X;
                progressBars[i].Size = new Size(width, this.Height - BORDER*2);

            }
            OverlayRectangle.Size = this.Size;
        }

        private void OverlayRectangle_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);
            if (ShowDetails || true)
            {
                // format text
                StringFormat theFormat = new StringFormat();
                theFormat.Alignment = StringAlignment.Center;
                theFormat.LineAlignment = StringAlignment.Center;

                /*
                if (false)
                {
                    // draw shadow
                    e.Graphics.DrawString(percentCompleted, new Font("Arial", 12, FontStyle.Regular),
                        new SolidBrush(Color.Black), new Point((this.Size.Width / 2) + 4, (this.Size.Height / 2) + 4), theFormat);
                    theFormat.Alignment = StringAlignment.Near;
                }
                */

                if (ShowElapseTime)
                {
                    Font theFont = new Font("Arial", 12, FontStyle.Regular);
                    SolidBrush theBrush = new SolidBrush(Color.Black);
                    e.Graphics.DrawString(elapsedTime, theFont,
                        theBrush, new Point(21, (this.Size.Height / 2) + 4), theFormat);
                    //OverlayRectangle.BringToFront();
                    theFont.Dispose();
                    theBrush.Dispose();
                }

                // draw text
                Font theFont2 = new Font("Arial", 14, FontStyle.Regular);
                SolidBrush theBrush2 = new SolidBrush(this.ForeColor);
                theFormat.Alignment = StringAlignment.Center;
                e.Graphics.DrawString(percentCompleted, theFont2,
                    theBrush2, new Point((this.Size.Width / 2) + 3, (this.Size.Height / 2) + 3), theFormat);
                theFormat.Alignment = StringAlignment.Near;
                theFont2.Dispose();
                theBrush2.Dispose();

                if (ShowElapseTime)
                {
                    Font theFont3 = new Font("Arial", 12, FontStyle.Regular);
                    SolidBrush theBrush3 = new SolidBrush(this.ForeColor);
                    e.Graphics.DrawString(elapsedTime, theFont3, theBrush3
                        , new Point(20, (this.Size.Height / 2) + 3), theFormat);
                    //OverlayRectangle.BringToFront();
                    theFont3.Dispose();
                    theBrush3.Dispose();
                }

            }
        }
    }
}
