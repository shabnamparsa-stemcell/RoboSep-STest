using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace GUI_Controls
{
    public delegate void ScrollValueChangedDelegate(ICustomScrollbar sender, int newValue);

    public interface ICustomScrollbar
    {
        event ScrollValueChangedDelegate ValueChanged;

        int LargeChange { get; set; }
        int SmallChange { get; set; }

        int Maximum { get; set; }
        int Minimum { get; set; }
        int Value { get; set; }

    }

    public partial class ScrollBar :  Control, ICustomScrollbar
    {
        private enum EnumScrollAction
        {
            Down,
            Up,
            PageDown,
            PageUp
        }
        public enum EnumThumbStyle
        {
            Auto,
            Large,
            Small
        }

        private volatile int init = 0;
        private bool disableChangeEvents = false;
        protected Pen borderPen;
        protected Brush backBrush;
        protected Brush activeBackBrush;

        protected Painter.State upperButtonState = Painter.State.Normal;
        protected Painter upperButtonPainter;
        protected Painter.State lowerButtonState = Painter.State.Normal;
        protected Painter lowerButtonPainter;
        protected Painter largeThumbPainter;
        protected Painter smallThumbPainter;
        protected Painter.State thumbState = Painter.State.Normal;
        protected Painter.State beforeThumbState = Painter.State.Normal;
        protected Painter.State afterThumbState = Painter.State.Normal;

        protected Rectangle currentThumbPosition;
        protected Rectangle beforeThumb;
        protected Rectangle afterThumb;
        private Rectangle moveThumbRectStart;

        protected EnumThumbStyle thumbStyle = EnumThumbStyle.Auto;
        private EnumScrollAction timerAction;
        private Timer timer = new Timer();
        private Point? moveThumbStart = null;
        private int moveThumbValueStart;

        protected Color activeBackColor = Color.Gray;
        protected bool useCustomActiveBackColor = false;
        protected bool useCustomBackBrush = false;

        public event ScrollValueChangedDelegate ValueChanged;

        protected int _largeChange = 10;
        protected int _smallChange = 1;
        protected int maximum = 99;
        protected int minimum = 0;
        protected int currentValue = 0;

        public ScrollBar()
        {
           InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            PrepareBack();

            upperButtonPainter = new StackedPainters(
                new ButtonPainter(),
                new PainterFilterSize(
                    new SymbolPainter(SymbolPainter.SymbolEnum.TriangleUp, true, 1, Color.Black, Color.Black, Color.Black),
                    PainterFilterSize.Alignment.Center, PainterFilterSize.Alignment.Center, 0, 0, 0, 0, 10, 2));

            lowerButtonPainter = new StackedPainters(
                new ButtonPainter(),
                new PainterFilterSize(
                    new SymbolPainter(SymbolPainter.SymbolEnum.TriangleDown, true, 1, Color.Black, Color.Black, Color.Black),
                    PainterFilterSize.Alignment.Center, PainterFilterSize.Alignment.Center, 0, 0, 0, 0, 10, 2));

            smallThumbPainter = new ButtonPainter();
            largeThumbPainter = new StackedPainters(
                new ButtonPainter(),
                new PainterFilterSize(
                    new SymbolPainter(SymbolPainter.SymbolEnum.GripH, true, 1, Color.Black, Color.Black, Color.Black),
                    PainterFilterSize.Alignment.Center, PainterFilterSize.Alignment.Center, 0, 0, 0, 0, 10, 2)
                    );

            InvalidateThumbPosition();

            timer.Tick += new EventHandler(timer_Tick);

        }

        public void BeginInit()
        {
            init++;
        }

        public void EndInit()
        {
            if (init > 0)
                init--;

            if (init == 0)
                SetBounds(minimum, maximum);
        }

        public EnumThumbStyle ThumbStyle
        {
            get { return thumbStyle; }
            set { thumbStyle = value; }
        }

        public void SetUpperButtonPainter(Painter upperButtonPainter)
        {
            this.upperButtonPainter = upperButtonPainter;
        }

        public void SetLowerButtonPainter(Painter lowerButtonPainter)
        {
            this.lowerButtonPainter = lowerButtonPainter;
        }

        public void SetLargeThumbPainter(Painter largeThumbPainter)
        {
            this.largeThumbPainter = largeThumbPainter;
        }

        public void SetSmallThumbPainter(Painter smallThumbPainter)
        {
            this.smallThumbPainter = smallThumbPainter;
        }

        public override Color BackColor
        {
            get { return base.BackColor; }
            set
            {
                base.BackColor = value;
                PrepareBack();
            }
        }

        public Color ActiveBackColor
        {
            get { return activeBackColor; }
            set
            {
                activeBackColor = value;
                PrepareBack();
            }
        }

        public void SetCustomBackBrush(Brush brush, Brush activeBackBrush)
        {
            useCustomBackBrush = brush != null;
            backBrush = brush;

            useCustomActiveBackColor = activeBackBrush != null;
            this.activeBackBrush = activeBackBrush;
            PrepareBack();
        }

        protected void PrepareBack()
        {
            if (!useCustomBackBrush)
                backBrush = new SolidBrush(BackColor);

            if (!useCustomActiveBackColor)
                activeBackBrush = new SolidBrush(ActiveBackColor);
        }

        #region Painting methods
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            PaintBack(e);

            Rectangle upperButton = RectUpperButton();
            Rectangle lowerButton = RectLowerButton();
            upperButtonPainter.Paint(e.Graphics, upperButton, upperButtonState, "", null, Font, null);
            lowerButtonPainter.Paint(e.Graphics, lowerButton, lowerButtonState, "", null, Font, null);

            if (currentThumbPosition.Height > 0)
                GetThumbPainter().Paint(e.Graphics, currentThumbPosition, thumbState, "", null, Font, null);
        }

        protected void PaintBack(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(backBrush, this.ClientRectangle);

            if (beforeThumbState == Painter.State.Pressed)
                e.Graphics.FillRectangle(activeBackBrush, beforeThumb);
            if (afterThumbState == Painter.State.Pressed)
                e.Graphics.FillRectangle(activeBackBrush, afterThumb);
        }

        protected Rectangle RectUpperButton()
        {
            return new Rectangle(2, 2, ClientSize.Width - 4, 30);
        }

        protected Rectangle RectLowerButton()
        {
            return new Rectangle(2, ClientSize.Height - 32, ClientSize.Width - 4, 30);
        }

        protected Painter GetThumbPainter()
        {
            if (GetRealThumbStyle() == EnumThumbStyle.Small)
                return smallThumbPainter;
            else
                return largeThumbPainter;
        }

        protected EnumThumbStyle GetRealThumbStyle()
        {
            EnumThumbStyle myThumbStyle = thumbStyle;

            if (myThumbStyle == EnumThumbStyle.Auto && maximum - minimum >= 200)
                myThumbStyle = EnumThumbStyle.Small;
            else
                myThumbStyle = EnumThumbStyle.Large;

            return myThumbStyle;
        }
        #endregion

        #region Mouse events
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            //The thumb gets actively moved by the mouse
            if (moveThumbStart != null)
            {
                int offset = e.Y - moveThumbStart.Value.Y;
                //Find the nearest valid thumb position for Y + offset and translate it
                //to the corresponding value
                double barHeight = ClientSize.Height - 4 - 2 * 30 - moveThumbRectStart.Height;
                double smallChangeHeight = barHeight / (double)(maximum - minimum);


                double lastOffset = Math.Abs(offset);
                int currentValIncrease = 0;
                while (true)
                {
                    if (offset > 0)
                        currentValIncrease++;
                    else
                        currentValIncrease--;

                    double currentOffset = Math.Abs((double)smallChangeHeight * (double)currentValIncrease - (double)offset);
                    if (currentOffset > lastOffset || (currentOffset == 0 && lastOffset == 0))
                        break;

                    lastOffset = currentOffset;

                }
                if (currentValIncrease > 1)
                    SetValue(moveThumbValueStart + currentValIncrease - 1);
                else if (currentValIncrease < -1)
                    SetValue(moveThumbValueStart + currentValIncrease + 1);
                return;
            }

            if (AnyStateHasStatus(Painter.State.Pressed))
                return;


            if (RectUpperButton().Contains(e.Location) && upperButtonState != Painter.State.Hover)
            {
                ResetAllButtonState();
                upperButtonState = Painter.State.Hover;
                Invalidate();
            }
            else if (RectLowerButton().Contains(e.Location) && lowerButtonState != Painter.State.Hover)
            {
                ResetAllButtonState();
                lowerButtonState = Painter.State.Hover;
                Invalidate();
            }
            else if (currentThumbPosition.Contains(e.Location) && thumbState != Painter.State.Hover)
            {
                ResetAllButtonState();
                thumbState = Painter.State.Hover;
                Invalidate();
            }
            else if (beforeThumb.Contains(e.Location) && beforeThumbState != Painter.State.Hover)
            {
                ResetAllButtonState();
                beforeThumbState = Painter.State.Hover;
                Invalidate();
            }
            else if (afterThumb.Contains(e.Location) && afterThumbState != Painter.State.Hover)
            {
                ResetAllButtonState();
                afterThumbState = Painter.State.Hover;
                Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (RectUpperButton().Contains(e.Location) && upperButtonState != Painter.State.Pressed)
            {
                ScrollAndActivateTimer(EnumScrollAction.Up);
                ResetAllButtonState();
                upperButtonState = Painter.State.Pressed;
                Invalidate();
            }
            else if (RectLowerButton().Contains(e.Location) && lowerButtonState != Painter.State.Pressed)
            {
                ScrollAndActivateTimer(EnumScrollAction.Down);
                ResetAllButtonState();
                lowerButtonState = Painter.State.Pressed;
                Invalidate();
            }
            else if (currentThumbPosition.Contains(e.Location) && thumbState != Painter.State.Pressed)
            {
                ResetAllButtonState();
                thumbState = Painter.State.Pressed;
                moveThumbStart = e.Location;
                moveThumbValueStart = currentValue;
                moveThumbRectStart = currentThumbPosition;
                Invalidate();
            }
            else if (beforeThumb.Contains(e.Location) && beforeThumbState != Painter.State.Pressed)
            {
                ScrollAndActivateTimer(EnumScrollAction.PageUp);
                ResetAllButtonState();
                beforeThumbState = Painter.State.Pressed;
                Invalidate();
            }
            else if (afterThumb.Contains(e.Location) && afterThumbState != Painter.State.Pressed)
            {
                ScrollAndActivateTimer(EnumScrollAction.PageDown);
                ResetAllButtonState();
                afterThumbState = Painter.State.Pressed;
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            timer.Stop();
            base.OnMouseUp(e);

            ResetAllButtonState();
            Invalidate();
        }

        private void ResetAllButtonState()
        {
            moveThumbStart = null;
            thumbState = Painter.State.Normal;
            lowerButtonState = Painter.State.Normal;
            upperButtonState = Painter.State.Normal;
            beforeThumbState = Painter.State.Normal;
            afterThumbState = Painter.State.Normal;
        }

        private bool AnyStateHasStatus(Painter.State state)
        {
            return thumbState == state || lowerButtonState == state ||
                upperButtonState == state || beforeThumbState == state ||
                afterThumbState == state;
        }
        #endregion

        #region Resize
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            InvalidateThumbPosition();
        }
        #endregion

        #region Do the scroll
        private void Scroll(EnumScrollAction scrollAction)
        {
            int change = SmallChange;

            if (scrollAction == EnumScrollAction.Up)
                change = -SmallChange;
            else if (scrollAction == EnumScrollAction.PageDown)
                change = LargeChange;
            else if (scrollAction == EnumScrollAction.PageUp)
                change = -LargeChange;

            SetValue(currentValue + change);
        }

        private void ScrollAndActivateTimer(EnumScrollAction scrollAction)
        {
            Scroll(scrollAction);
            timerAction = scrollAction;
            timer.Interval = 500;
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timer.Interval = 100;
            Scroll(timerAction);
        }
        #endregion

        protected void InvalidateThumbPosition()
        {
            if (init > 0) return;

            if (minimum == maximum)
            {
                currentThumbPosition = new Rectangle(0, 0, 0, 0);
                return;
            }
            //Calculate the size of the thumb, thumb is positioned in origin
            if (GetRealThumbStyle() == EnumThumbStyle.Small)
                currentThumbPosition = new Rectangle(2, 32, ClientSize.Width - 4, 15);
            else
                currentThumbPosition = new Rectangle(2, 32, ClientSize.Width - 4, 30);

            //Move the thumb to the correct position (depending on the current scroll value)
            double barHeight = ClientSize.Height - 4 - 2 * 30 - currentThumbPosition.Height;
            double smallChangeHeight = barHeight / (double)(maximum - minimum);

            int thumbTopOffset = (int)(smallChangeHeight * (double)currentValue);
            currentThumbPosition.Offset(0, thumbTopOffset);


            beforeThumb = Rectangle.FromLTRB(2, RectUpperButton().Bottom, ClientSize.Width - 2, currentThumbPosition.Top);
            afterThumb = Rectangle.FromLTRB(2, currentThumbPosition.Bottom, ClientSize.Width - 2, RectLowerButton().Top);
            this.Invalidate();
        }

        protected void SetBounds(int minimum, int maximum)
        {
            if (init > 0) return;

            if (minimum > maximum)
                minimum = maximum;

            this.minimum = minimum;
            this.maximum = maximum;

            SetValue(currentValue);

        }
        protected void SetValue(int newValue)
        {
            if (init > 0) return;

            if (newValue < minimum)
                currentValue = minimum;
            else if (newValue > maximum)
                currentValue = maximum;
            else
                currentValue = newValue;

            InvalidateThumbPosition();

            if (ValueChanged != null)
                ValueChanged(this, currentValue);
        }

        #region ICustomScrollbar Members

        public int LargeChange
        {
            get { return _largeChange; }
            set { _largeChange = value; }
        }

        public int SmallChange
        {
            get { return _smallChange; }
            set { _smallChange = value; }
        }

        public int Maximum
        {
            get { return maximum; }
            set { SetBounds(minimum, value); }
        }

        public int Minimum
        {
            get { return minimum; }
            set { SetBounds(value, maximum); }
        }

        public int Value
        {
            get { return currentValue; }
            set { SetValue(value); }
        }

        private void scrollbar_ValueChanged(ICustomScrollbar sender, int newValue)
        {
            if (disableChangeEvents) return;

            disableChangeEvents = true;

            if (this != sender)
                Value = newValue;

            disableChangeEvents = false;
            currentValue = newValue;
            if (ValueChanged != null)
                ValueChanged(this, newValue);
        }

        #endregion
    }


    public abstract class Painter
    {
        public enum State
        {
            Normal,
            Pressed,
            Hover
        }

        public abstract void Paint(Graphics g, Rectangle position, State buttonState, string text, Image buttonImage, Font textFont, Rectangle? referencePosition);
    }

    public abstract class DoubleBrushPainter : Painter
    {

        protected abstract Color BorderColor(Painter.State state);
        protected abstract int BorderWidth(Painter.State state);
        protected abstract Brush UpperBrush(Painter.State state, Rectangle bounds);
        protected abstract Brush LowerBrush(Painter.State state, Rectangle bounds);
        protected abstract Color FontColor(Painter.State state);

        public override void Paint(Graphics g, Rectangle position, Painter.State buttonState, string text, Image buttonImage, Font textFont, Rectangle? referencePosition)
        {
            Rectangle upperRect;
            Rectangle lowerRect;

            if (buttonState != State.Pressed)
            {
                upperRect = new Rectangle(position.Left, position.Top, position.Width, position.Height / 2 - position.Height / 8);
                lowerRect = new Rectangle(position.Left, position.Top + position.Height / 2 - position.Height / 8, position.Width, position.Height / 2 + position.Height / 8);
            }
            else
            {
                upperRect = new Rectangle(position.Left, position.Top, position.Width, Math.Max(1, position.Height / 2));
                lowerRect = new Rectangle(position.Left, position.Top + position.Height / 2, position.Width, Math.Max(1, position.Height / 2));
            }

            g.FillRectangle(UpperBrush(buttonState, upperRect), upperRect);
            g.FillRectangle(LowerBrush(buttonState, lowerRect), lowerRect);

            Rectangle borderRect = new Rectangle(position.X, position.Y, position.Width - BorderWidth(buttonState), position.Height - BorderWidth(buttonState));

            g.DrawRectangle(new Pen(BorderColor(buttonState), BorderWidth(buttonState)), borderRect);

            Rectangle textBounds;

            if (buttonImage == null)
                textBounds = new Rectangle(
                   position.X + 2,
                   position.Y + 2,
                   position.Width - 4,
                   position.Height - 4);
            else
            {
                //There is an image, calculate the the width from the max height
                int imageHeight = position.Height - 10;

                double imageRatio = (double)buttonImage.Width / (double)buttonImage.Height;
                int imageWidth = (int)(imageRatio * (double)imageHeight);

                Rectangle imagePosition = new Rectangle(position.X + 5, position.Y + 5, imageWidth, imageHeight);
                textBounds = new Rectangle(imagePosition.Right + 2, position.Y + 2, position.Width - imagePosition.Width - 10, position.Height - 4);

                g.DrawImage(buttonImage, imagePosition, new Rectangle(0, 0, buttonImage.Width, buttonImage.Height), GraphicsUnit.Pixel);
            }

            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Center;
            format.FormatFlags = StringFormatFlags.NoClip
            | StringFormatFlags.FitBlackBox | StringFormatFlags.NoWrap;

            if (referencePosition != null)
            {

                double xRatio = (double)position.Width / (double)referencePosition.Value.Width;
                double yRatio = (double)position.Height / (double)referencePosition.Value.Height;
                textFont = ScaledFont(textFont, xRatio, yRatio, text, textBounds, g, format);
            }

            using (Brush frontBrush = new SolidBrush(FontColor(buttonState)))
            {
                g.DrawString(text, textFont, frontBrush,
                    textBounds, format);
            }
        }

        protected Font ScaledFont(Font referenceFont, double xRatio, double yRatio, string text, Rectangle fitTo, Graphics g, StringFormat format)
        {
            //Calculate scaled fonts based on the y-scale-factor
            float fontsize_YScaled = Math.Max(1, (int)(referenceFont.Size * yRatio));

            Font newFont = new Font(referenceFont.FontFamily, fontsize_YScaled, referenceFont.Style);
            //Check if text with y-scaled font fits
            SizeF textSize = g.MeasureString(text, newFont, fitTo.Width, format);

            if (textSize.Height <= fitTo.Height)
                return newFont;
            else
            {
                do
                {
                    newFont.Dispose();
                    newFont = null;

                    if (fontsize_YScaled <= 1)
                        return new Font(FontFamily.GenericSansSerif, 1, referenceFont.Style);

                    fontsize_YScaled -= 0.5f;
                    newFont = new Font(referenceFont.FontFamily, fontsize_YScaled, referenceFont.Style);
                    //And check again
                    textSize = g.MeasureString(text, newFont, fitTo.Width);

                    if (textSize.Height <= fitTo.Height)
                        return newFont;

                } while (textSize.Height <= fitTo.Height);
                return newFont;
            }
        }
    }

    public class ButtonPainter : Painter
    {
        protected Rectangle? lastPosition = null;
        protected Painter.State? lastState = null;
        protected GraphicsPath upperGradientPath = null;

        protected Rectangle upperGradientRect;
        protected Rectangle leftBounds;
        protected Rectangle middleBounds;
        protected Rectangle rightBounds;

        protected Brush upperGradientBrush = null;
        protected Brush leftBrush = null;
        protected Brush middleBrush = null;
        protected Brush rightBrush = null;
        protected Pen borderPen = null;

        protected virtual void RecalcBrushes(Rectangle position, Painter.State state)
        {
            if (lastPosition == null || lastPosition.Value != position || lastState == null || lastState.Value != state)
            {
                lastState = state;
                if (state == State.Hover)
                    borderPen = new Pen(Color.FromArgb(0x3c, 0x7f, 0xb1), 1);
                else if (state == State.Pressed)
                    borderPen = new Pen(Color.FromArgb(0x18, 0x59, 0x8a), 1);
                else
                    borderPen = new Pen(Color.FromArgb(0x9a, 0x9a, 0x9a), 1);

                leftBounds = new Rectangle(position.X, position.Y, 8, position.Height);
                leftBrush = CreateLeftBrush(leftBounds, state);

                middleBounds = new Rectangle(leftBounds.Right, leftBounds.Y, (int)((float)position.Width - 16), position.Height);
                middleBrush = CreateMiddleBrush(middleBounds, state);

                rightBounds = new Rectangle(middleBounds.Right, leftBounds.Y, 8, position.Height);
                rightBrush = CreateRightBrush(rightBounds, state);

                upperGradientPath = new GraphicsPath();
                upperGradientPath.AddLine(position.X + 8, position.Y + 1, position.X + 8, position.Y + 5);
                upperGradientPath.AddLine(position.X + 8, position.Y + 5, middleBounds.Right, position.Y + 5);
                upperGradientPath.AddLine(middleBounds.Right, position.Y + 5, rightBounds.Right - 1, position.Y + 1);
                upperGradientPath.CloseAllFigures();

                upperGradientRect = new Rectangle(position.X + 8, position.Y + 1, 10, 4);
                upperGradientBrush = new LinearGradientBrush(upperGradientRect, Color.FromArgb(237, 237, 237), Color.FromArgb(221, 221, 224), LinearGradientMode.Vertical);

                lastPosition = position;
            }
        }

        protected virtual Brush CreateLeftBrush(Rectangle bounds, Painter.State state)
        {
            if (state == State.Hover)
                return new LinearGradientBrush(bounds, Color.FromArgb(252, 226, 251), Color.FromArgb(252, 214, 251), LinearGradientMode.Horizontal); 
            else if (state == State.Pressed)
                return new LinearGradientBrush(bounds, Color.FromArgb(251, 206, 251), Color.FromArgb(248, 180, 245), LinearGradientMode.Horizontal);  
            else
                return new LinearGradientBrush(bounds, Color.FromArgb(245, 245, 245), Color.FromArgb(234, 234, 234), LinearGradientMode.Horizontal); ;
        }

        protected virtual Brush CreateMiddleBrush(Rectangle bounds, Painter.State state)
        {
            if (state == State.Hover)
                return new SolidBrush(Color.FromArgb(247, 168, 247));
            else if (state == State.Pressed)
                return new SolidBrush(Color.FromArgb(241, 109, 241));
            else
                return new SolidBrush(Color.FromArgb(221, 217, 221));
        }

        protected virtual Brush CreateRightBrush(Rectangle bounds, Painter.State state)
        {
            if (state == State.Hover)
                return new LinearGradientBrush(bounds, Color.FromArgb(247, 168, 247), Color.FromArgb(228, 154, 228), LinearGradientMode.Horizontal);
            else if (state == State.Pressed)
                return new LinearGradientBrush(bounds, Color.FromArgb(232, 109, 241), Color.FromArgb(220, 100, 223), LinearGradientMode.Horizontal);
            else
                return new LinearGradientBrush(bounds, Color.FromArgb(214, 214, 214), Color.FromArgb(193, 193, 193), LinearGradientMode.Horizontal);
        }

        public override void Paint(Graphics g, Rectangle position, Painter.State state, string text, Image buttonImage, Font textFont, Rectangle? referencePosition)
        {
            if (position.Height < 10 || position.Width < 20)
                return;

            RecalcBrushes(position, state);
            g.FillRectangle(leftBrush, leftBounds);
            g.FillRectangle(middleBrush, middleBounds);
            g.FillRectangle(rightBrush, rightBounds);
            DrawBorder(g, position, state);
            g.FillPath(upperGradientBrush, upperGradientPath);
        }

        public virtual void DrawBorder(Graphics g, Rectangle position, Painter.State state)
        {
            g.DrawRectangle(borderPen, new Rectangle(position.X, position.Y, (int)(position.Width - borderPen.Width), (int)(position.Height - borderPen.Width)));
        }
    }

    public class SymbolPainter : Painter
    {
        public enum SymbolEnum
        {
            TriangleDown,
            TriangleUp,
            GripH
        }

        private bool fill;
        private SymbolEnum symbol;
        private int penWidth;
        private Color color;
        private Pen pen;
        private Brush fillBrush;
        private Pen hoverPen;
        private Pen clickPen;

        private List<SymbolEnum> noFill = new List<SymbolEnum>(new SymbolEnum[]{ SymbolEnum.GripH });

        public static Painter Create(Painter backgroundPainter, SymbolEnum symbol, bool fill, int penWidth, Color forecolor, Color hoverColor, Color clickColor)
        {
            return new StackedPainters(
                new PainterFilterNoText(backgroundPainter),
                new SymbolPainter(symbol, fill, penWidth, forecolor, hoverColor, clickColor));
        }

        public SymbolPainter(SymbolEnum symbol, bool fill, int penWidth, Color forecolor, Color hoverPen, Color clickPen)
        {
            this.symbol = symbol;
            this.fill = fill;
            this.penWidth = penWidth;
            this.color = forecolor;
            this.hoverPen = new Pen(hoverPen, penWidth);
            this.clickPen = new Pen(clickPen, penWidth);
            this.pen = new Pen(color, penWidth);
            this.fillBrush = new SolidBrush(forecolor);
        }

        private GraphicsPath BuildTriangleDown(Rectangle bounds)
        {
            int xPadding = bounds.Width / 10;
            int yPadding = bounds.Height / 10;
            int triangleHalf = Math.Max(0, bounds.Width / 2 - xPadding);

            GraphicsPath triangle = new GraphicsPath();
            triangle.AddLine(bounds.Left + xPadding, bounds.Top + yPadding, bounds.Left + xPadding + 2 * triangleHalf, bounds.Top + yPadding);
            triangle.AddLine(bounds.Left + xPadding + 2 * triangleHalf, bounds.Top + yPadding, bounds.Left + xPadding + triangleHalf, bounds.Bottom - yPadding);
            triangle.CloseAllFigures();
            return triangle;
        }

        private GraphicsPath BuildTriangleUp(Rectangle bounds)
        {
            int xPadding = bounds.Width / 10;
            int yPadding = bounds.Height / 10;
            int triangleHalf = Math.Max(0, bounds.Width / 2 - xPadding);

            GraphicsPath triangle = new GraphicsPath();
            triangle.AddLine(bounds.Left + xPadding, bounds.Bottom - yPadding, bounds.Left + xPadding + 2 * triangleHalf, bounds.Bottom - yPadding);
            triangle.AddLine(bounds.Left + xPadding + 2 * triangleHalf, bounds.Bottom - yPadding, bounds.Left + xPadding + triangleHalf, bounds.Top + yPadding);
            triangle.CloseAllFigures();
            return triangle;
        }

        private GraphicsPath BuildGripH(Rectangle bounds)
        {
            int xPadding = bounds.Width / 10;
            int yPadding = bounds.Height / 10;

            int half = (int)((double)(bounds.Height - 2 * yPadding) / 2.0);

            GraphicsPath grip = new GraphicsPath();
            grip.AddLine(bounds.Left + xPadding, bounds.Top + yPadding, bounds.Right - xPadding, bounds.Top + yPadding);
            grip.StartFigure();
            grip.AddLine(bounds.Left + xPadding, bounds.Top + yPadding + half, bounds.Right - xPadding, bounds.Top + yPadding + half);
            grip.StartFigure();
            grip.AddLine(bounds.Left + xPadding, bounds.Top + yPadding + 2 * half, bounds.Right - xPadding, bounds.Top + yPadding + 2 * half);
            return grip;
        }

        public override void Paint(Graphics g, Rectangle position, Painter.State buttonState, string text, Image buttonImage, Font textFont, Rectangle? referencePosition)
        {
            GraphicsPath path;
            Pen myPen = pen;

            if (buttonState == State.Hover)
                myPen = hoverPen;
            else if (buttonState == State.Pressed)
                myPen = clickPen;

            if (symbol == SymbolEnum.TriangleDown)
                path = BuildTriangleDown(position);
            else if (symbol == SymbolEnum.TriangleUp)
                path = BuildTriangleUp(position);
            else if (symbol == SymbolEnum.GripH)
                path = BuildGripH(position);
            else
                throw new NotImplementedException("Symbol not implemented");

            if (fill && noFill.Contains(symbol) == false)
            {
                g.FillPath(fillBrush, path);

                if (buttonState == State.Hover)
                    g.DrawPath(myPen, path);
                else if (buttonState == State.Pressed)
                    g.DrawPath(myPen, path);
            }
            else
                g.DrawPath(myPen, path);
        }
    }

    public class StackedPainters : Painter
    {
        private List<Painter> lstPainters;

        public StackedPainters(params Painter[] painters)
        {
            lstPainters = new List<Painter>(painters);
        }

        public override void Paint(Graphics g, Rectangle position, Painter.State buttonState, string text, Image buttonImage, Font textFont, Rectangle? referencePosition)
        {
            foreach (Painter p in lstPainters)
                p.Paint(g, position, buttonState, text, buttonImage, textFont, referencePosition);
        }
    }

    public class PainterFilterSize : Painter
    {
        public enum Alignment
        {
            Center,
            Near,
            Far
        }

        private Alignment hAlign;
        private Alignment vAlign;
        private int paddingTop;
        private int paddingRight;
        private int paddingLeft;
        private int paddingBottom;
        private int maxWidth;

        // width/height = t
        private double targetRatio;
        private Painter subPainter;

        public PainterFilterSize(Painter subPainter, Alignment hAlign, Alignment vAlign,
            int paddingTop, int paddingLeft, int paddingRight, int paddingBottom,
            int maxWidth,
            double targetRatio)
        {
            this.hAlign = hAlign;
            this.vAlign = vAlign;
            this.paddingTop = paddingTop;
            this.paddingBottom = paddingBottom;
            this.paddingLeft = paddingLeft;
            this.paddingRight = paddingRight;

            this.maxWidth = maxWidth;
            this.targetRatio = targetRatio;
            this.subPainter = subPainter;
        }
        public override void Paint(Graphics g, Rectangle position, Painter.State buttonState, string text, Image buttonImage, Font textFont, Rectangle? referencePosition)
        {
            Rectangle layoutArea = Rectangle.FromLTRB(position.X + paddingLeft,
                position.Y + paddingTop,
                position.Right - paddingRight,
                position.Bottom - paddingBottom);

            if (layoutArea.Width <= 0 || layoutArea.Height <= 0)
                return;

            double layoutRatio = (double)layoutArea.Width / (double)layoutArea.Height;

            Rectangle targetRect;
            //maximize width
            if (layoutRatio < targetRatio)
            {
                int targetWidth = layoutArea.Width;

                if (maxWidth > 0)
                    targetWidth = Math.Min(layoutArea.Width, maxWidth);

                int targetHeight = (int)((double)targetWidth / targetRatio);

                targetRect = new Rectangle(layoutArea.X, layoutArea.Y, targetWidth, targetHeight);

            }
            else
            {
                //maximize height
                int targetWidth = (int)((double)layoutArea.Height * targetRatio);
                int targetHeight = layoutArea.Height;

                if (maxWidth > 0 && targetWidth > maxWidth)
                {
                    targetWidth = maxWidth;
                    targetHeight = (int)((double)targetHeight / targetRatio);
                }

                targetRect = new Rectangle(layoutArea.X, layoutArea.Y, targetWidth, targetHeight);
            }

            if (vAlign == Alignment.Far)
                targetRect = new Rectangle(targetRect.X, layoutArea.Bottom - targetRect.Height, targetRect.Width, targetRect.Height);
            else if (vAlign == Alignment.Center)
                targetRect = new Rectangle(targetRect.X, layoutArea.Top + (int)((double)layoutArea.Height / 2.0d - (double)targetRect.Height / 2.0d),
                    targetRect.Width, targetRect.Height);

            if (hAlign == Alignment.Far)
                targetRect = new Rectangle(layoutArea.Right - targetRect.Width, targetRect.Y, targetRect.Width, targetRect.Height);
            else if (hAlign == Alignment.Center)
                targetRect = new Rectangle(targetRect.X + (int)((double)layoutArea.Width / 2.0d - (double)targetRect.Width / 2.0d),
                    targetRect.Y, targetRect.Width, targetRect.Height);

            subPainter.Paint(g, targetRect, buttonState, text, buttonImage, textFont, referencePosition);
        }
    }

    public class PainterFilterNoText : Painter
    {
        private Painter subPainter;

        public PainterFilterNoText(Painter p)
        {
            subPainter = p;
        }

        public override void Paint(Graphics g, Rectangle position, Painter.State buttonState, string text, Image buttonImage, Font textFont, Rectangle? referencePosition)
        {
            subPainter.Paint(g, position, buttonState, "", buttonImage, textFont, referencePosition);
        }
    }

}
