using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;



namespace GUI_Controls
{
    public partial class ScrollIndicator : Control, ICustomScrollbar
    {
        private bool disableChangeEvents = false;
        public event ScrollValueChangedDelegate ValueChanged;
        private Rectangle barRect; //bounding rectangle of bar area
        private Rectangle barHalfRect;
        private Rectangle thumbHalfRect;
        private Rectangle elapsedRect; //bounding rectangle of elapsed area
        private Rectangle thumbRect; //bounding rectangle of thumb area
        private GraphicsPath thumbCustomShape = null;
        private int thumbSize = 15;
        private Size thumbRoundRectSize = new Size(8, 8);

        private Size borderRoundRectSize = new Size(8, 8);
        private Orientation barOrientation = Orientation.Vertical;
        private int trackerValue = 50;
        private int barMinimum = 0;
        private int barMaximum = 100;
        private int smallChange = 1;
        private int largeChange = 5;
        private bool drawFocusRectangle = true;
        private bool drawSemitransparentThumb = true;
        private Color thumbOuterColor = Color.White;
        private Color thumbInnerColor = Color.FromArgb(78, 38, 131);
        private Color thumbPenColor = Color.Silver;
        private Color barOuterColor = Color.SkyBlue;
        private Color barInnerColor = Color.DarkSlateBlue;
        private Color barPenColor = Color.Gainsboro;
        private Color elapsedOuterColor = Color.DarkGreen;
        private Color elapsedInnerColor = Color.Chartreuse;


        public ScrollIndicator()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        public Rectangle ThumbRect
        {
            get { return thumbRect; }
        }

        public int ThumbSize
        {
            get { return thumbSize; }
            set
            {
                if (value > 0 &
                    value < (barOrientation == Orientation.Horizontal ? ClientRectangle.Width : ClientRectangle.Height))
                    thumbSize = value;
                else
                    throw new ArgumentOutOfRangeException(
                        "TrackSize has to be greather than zero and lower than half of Slider width");
                Invalidate();
            }
        }

        public GraphicsPath ThumbCustomShape
        {
            get { return thumbCustomShape; }
            set
            {
                if (value != null)
                {
                    thumbCustomShape = value;
                    thumbSize = (int)(barOrientation == Orientation.Horizontal ? value.GetBounds().Width : value.GetBounds().Height) + 1;
                    Invalidate();
                }
            }
        }

        public Size ThumbRoundRectSize
        {
            get { return thumbRoundRectSize; }
            set
            {
                int h = value.Height, w = value.Width;
                if (h <= 0) h = 1;
                if (w <= 0) w = 1;
                thumbRoundRectSize = new Size(w, h);
                Invalidate();
            }
        }

        public Size BorderRoundRectSize
        {
            get { return borderRoundRectSize; }
            set
            {
                int h = value.Height, w = value.Width;
                if (h <= 0) h = 1;
                if (w <= 0) w = 1;
                borderRoundRectSize = new Size(w, h);
                Invalidate();
            }
        }

        public Orientation Orientation
        {
            get { return barOrientation; }
            set
            {
                if (barOrientation != value)
                {
                    barOrientation = value;
                    int temp = Width;
                    Width = Height;
                    Height = temp;
                    if (thumbCustomShape != null)
                    {
                        thumbSize =
                            (int)
                            (barOrientation == Orientation.Horizontal
                                    ? thumbCustomShape.GetBounds().Width
                                    : thumbCustomShape.GetBounds().Height) + 1;
                    }
                    Invalidate();
                }
            }
        }

        #region ICustomScrollbar Members

        private void scrollbar_ValueChanged(ICustomScrollbar sender, int newValue)
        {
            if (disableChangeEvents) 
                return;

            disableChangeEvents = true;

            if (this != sender)
                Value = newValue;

            disableChangeEvents = false;
            trackerValue = newValue;

            if (ValueChanged != null)
                ValueChanged(this, newValue);
        }

        public int Value
        {
            get { return trackerValue; }
            set
            {
                if (value >= barMinimum & value <= barMaximum)
                {
                    trackerValue = value;
                    if (ValueChanged != null) 
                        ValueChanged(this, trackerValue);

                    Invalidate();
                }
                else throw new ArgumentOutOfRangeException("Value is outside appropriate range (min, max)");
            }
        }

        public int Minimum
        {
            get { return barMinimum; }
            set
            {
                if (value <= barMaximum)
                {
                      barMinimum = value;
                    if (trackerValue < barMinimum)
                    {
                        trackerValue = barMinimum;

                        if (ValueChanged != null) 
                            ValueChanged(this, trackerValue);
                    }
                    Invalidate();
                }
                else throw new ArgumentOutOfRangeException("Minimal value is greather than maximal one");
            }
        }

         public int Maximum
        {
            get { return barMaximum; }
            set
            {
                if (value >= barMinimum)
                {
                    barMaximum = value;
                    if (trackerValue > barMaximum)
                    {
                        trackerValue = barMaximum;

                        if (ValueChanged != null) 
                            ValueChanged(this, trackerValue);
                    }
                    if (barMaximum == 0)
                        barMaximum = 1;

                    Invalidate();
                }
                else throw new ArgumentOutOfRangeException("Maximal value is lower than minimal one");
            }
        }

        public int SmallChange
        {
            get { return smallChange; }
            set { smallChange = value; }
        }

        public int LargeChange
        {
            get { return largeChange; }
            set { largeChange = value; }
        }

        #endregion


        public bool DrawFocusRectangle
        {
            get { return drawFocusRectangle; }
            set
            {
                drawFocusRectangle = value;
                Invalidate();
            }
        }

        public bool DrawSemitransparentThumb
        {
            get { return drawSemitransparentThumb; }
            set
            {
                drawSemitransparentThumb = value;
                Invalidate();
            }
        }
   
        public Color ThumbOuterColor
        {
            get { return thumbOuterColor; }
            set
            {
                thumbOuterColor = value;
                Invalidate();
            }
        }
   
        public Color ThumbInnerColor
        {
            get { return thumbInnerColor; }
            set
            {
                thumbInnerColor = value;
                Invalidate();
            }
        }

        public Color ThumbPenColor
        {
            get { return thumbPenColor; }
            set
            {
                thumbPenColor = value;
                Invalidate();
            }
        }

        public Color BarOuterColor
        {
            get { return barOuterColor; }
            set
            {
                barOuterColor = value;
                Invalidate();
            }
        }

        public Color BarInnerColor
        {
            get { return barInnerColor; }
            set
            {
                barInnerColor = value;
                Invalidate();
            }
        }

        public Color BarPenColor
        {
            get { return barPenColor; }
            set
            {
                barPenColor = value;
                Invalidate();
            }
        }

        public Color ElapsedOuterColor
        {
            get { return elapsedOuterColor; }
            set
            {
                elapsedOuterColor = value;
                Invalidate();
            }
        }

        public Color ElapsedInnerColor
        {
            get { return elapsedInnerColor; }
            set
            {
                elapsedInnerColor = value;
                Invalidate();
            }
        }

        //define color schemas
        private Color[,] aColorSchema = new Color[,]
            {
                {
                    Color.White, Color.Purple, Color.Silver, Color.MediumPurple, Color.Purple, Color.Gainsboro,
                    Color.DarkGreen, Color.Chartreuse
                },
                {
                    Color.White, Color.Gainsboro, Color.Silver, Color.Red, Color.DarkRed, Color.Gainsboro, Color.Coral,
                    Color.LightCoral
                },
                {
                    Color.White, Color.Gainsboro, Color.Silver, Color.GreenYellow, Color.Yellow, Color.Gold, Color.Orange,
                    Color.OrangeRed
                },
                {
                    Color.White, Color.Gainsboro, Color.Silver, Color.Red, Color.Crimson, Color.Gainsboro, Color.DarkViolet
                    , Color.Violet
                }
            };

        public enum ColorSchemas
        {
            PerlPurpleGreen,
            PerlRedCoral,
            PerlGold,
            PerlRoyalColors
        }

        private ColorSchemas colorSchema = ColorSchemas.PerlPurpleGreen;
        public ColorSchemas ColorSchema
        {
            get { return colorSchema; }
            set
            {
                colorSchema = value;
                byte sn = (byte)value;
                thumbOuterColor = aColorSchema[sn, 0];
                thumbInnerColor = aColorSchema[sn, 1];
                thumbPenColor = aColorSchema[sn, 2];
                barOuterColor = aColorSchema[sn, 3];
                barInnerColor = aColorSchema[sn, 4];
                barPenColor = aColorSchema[sn, 5];
                elapsedOuterColor = aColorSchema[sn, 6];
                elapsedInnerColor = aColorSchema[sn, 7];

                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!Enabled)
            {
                Color[] desaturatedColors = DesaturateColors(thumbOuterColor, thumbInnerColor, thumbPenColor,
                                                                barOuterColor, barInnerColor, barPenColor,
                                                                elapsedOuterColor, elapsedInnerColor);
                DrawColorSlider(e, desaturatedColors[0], desaturatedColors[1], desaturatedColors[2],
                                desaturatedColors[3],
                                desaturatedColors[4], desaturatedColors[5], desaturatedColors[6], desaturatedColors[7]);
            }
            else
            {
                DrawColorSlider(e, thumbOuterColor, thumbInnerColor, thumbPenColor,
                                barOuterColor, barInnerColor, barPenColor,
                                elapsedOuterColor, elapsedInnerColor);
            }
        }

        private void DrawColorSlider(PaintEventArgs e, Color thumbOuterColorPaint, Color thumbInnerColorPaint,
                                        Color thumbPenColorPaint, Color barOuterColorPaint, Color barInnerColorPaint,
                                        Color barPenColorPaint, Color elapsedOuterColorPaint, Color elapsedInnerColorPaint)
        {
            try
            {
                //set up thumbRect
                int nRange = barMaximum - barMinimum;
                if (nRange == 0)
                    nRange = 1;

                if (barOrientation == Orientation.Horizontal)
                {
                    int TrackX = (((trackerValue - barMinimum) * (ClientRectangle.Width - thumbSize)) / (barMaximum - barMinimum));
                    thumbRect = new Rectangle(TrackX, 1, thumbSize - 1, ClientRectangle.Height - 3);
                }
                else
                {
                    int TrackY = (((trackerValue - barMinimum) * (ClientRectangle.Height - thumbSize)) / (barMaximum - barMinimum));
                    thumbRect = new Rectangle(ClientRectangle.Width/2, TrackY, ClientRectangle.Width - 2, thumbSize - 1);
                }

                //adjust drawing rects
                barRect = ClientRectangle;
                thumbHalfRect = thumbRect;
                LinearGradientMode gradientOrientation;

                if (barOrientation == Orientation.Horizontal)
                {
                    barRect.Inflate(-1, -barRect.Height / 3);
                    barHalfRect = barRect;
                    barHalfRect.Height /= 2;
                    gradientOrientation = LinearGradientMode.Vertical;
                    thumbHalfRect.Height /= 2;
                    elapsedRect = barRect;
                    elapsedRect.Width = thumbRect.Left + thumbSize / 2;
                }
                else
                {
                    barRect.Inflate(-1, -1);
                    barRect.Width /= 2;
                    barHalfRect = barRect;
                    barHalfRect.Width /= 2;
                    gradientOrientation = LinearGradientMode.Horizontal;
                    thumbHalfRect.Width /= 2;
                    elapsedRect = barRect;
                    elapsedRect.Height = thumbRect.Top + thumbSize / 2;
                }
                //get thumb shape path 
                GraphicsPath thumbPath;
                if (thumbCustomShape == null)
                    thumbPath = CreateRoundRectPath(thumbRect, thumbRoundRectSize);
                else
                {
                    thumbPath = thumbCustomShape;
                    Matrix m = new Matrix();
                    m.Translate(thumbRect.Left - thumbPath.GetBounds().Left, thumbRect.Top - thumbPath.GetBounds().Top);
                    thumbPath.Transform(m);
                }

                //draw bar
                using (
                    LinearGradientBrush lgbBar =
                        new LinearGradientBrush(barHalfRect, barOuterColorPaint, barInnerColorPaint, gradientOrientation)
                    )
                {
                    lgbBar.WrapMode = WrapMode.TileFlipXY;
                    e.Graphics.FillRectangle(lgbBar, barRect);
                    //draw elapsed bar
                    using (
                        LinearGradientBrush lgbElapsed =
                            new LinearGradientBrush(barHalfRect, elapsedOuterColorPaint, elapsedInnerColorPaint,
                                                    gradientOrientation))
                    {
                        lgbElapsed.WrapMode = WrapMode.TileFlipXY;
                        if (Capture && drawSemitransparentThumb)
                        {
                            Region elapsedReg = new Region(elapsedRect);
                            elapsedReg.Exclude(thumbPath);
                            e.Graphics.FillRegion(lgbElapsed, elapsedReg);
                        }
                        else
                            e.Graphics.FillRectangle(lgbElapsed, elapsedRect);
                    }
                    //draw bar band                    
                    using (Pen barPen = new Pen(barPenColorPaint, 0.5f))
                    {
                       e.Graphics.DrawRectangle(barPen, barRect);
                    }
                }

                //draw thumb
                Color newthumbOuterColorPaint = thumbOuterColorPaint, newthumbInnerColorPaint = thumbInnerColorPaint;
                using (
                    LinearGradientBrush lgbThumb =
                        new LinearGradientBrush(thumbHalfRect, newthumbOuterColorPaint, newthumbInnerColorPaint,
                                                gradientOrientation))
                {
                    lgbThumb.WrapMode = WrapMode.TileFlipXY;
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.FillPath(lgbThumb, thumbPath);
                    //draw thumb band
                    Color newThumbPenColor = thumbPenColorPaint;
                    using (Pen thumbPen = new Pen(newThumbPenColor))
                    {
                        e.Graphics.DrawPath(thumbPen, thumbPath);
                    }
                }

                //draw focusing rectangle
                if (Focused & drawFocusRectangle)
                    using (Pen p = new Pen(Color.FromArgb(200, barPenColorPaint)))
                    {
                        p.DashStyle = DashStyle.Dot;
                        Rectangle r = ClientRectangle;
                        r.Width -= 2;
                        r.Height--;
                        r.X++;
                      
                        using (GraphicsPath gpBorder = CreateRoundRectPath(r, borderRoundRectSize))
                        {
                            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                            e.Graphics.DrawPath(p, gpBorder);
                        }
                    }
            }
            catch (Exception Err)
            {
                // Log message
                string LOGmsg = String.Format("DrawBackGround Error: Exception = {0}", Err.Message);
                GUI_Controls.SP_Logger.log.Info(LOGmsg);
            }
            finally
            {
            }
        }
      
        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Invalidate();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate();
        }

        public static GraphicsPath CreateRoundRectPath(Rectangle rect, Size size)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddLine(rect.Left + size.Width / 2, rect.Top, rect.Right - size.Width / 2, rect.Top);
            gp.AddArc(rect.Right - size.Width, rect.Top, size.Width, size.Height, 270, 90);

            gp.AddLine(rect.Right, rect.Top + size.Height / 2, rect.Right, rect.Bottom - size.Width / 2);
            gp.AddArc(rect.Right - size.Width, rect.Bottom - size.Height, size.Width, size.Height, 0, 90);

            gp.AddLine(rect.Right - size.Width / 2, rect.Bottom, rect.Left + size.Width / 2, rect.Bottom);
            gp.AddArc(rect.Left, rect.Bottom - size.Height, size.Width, size.Height, 90, 90);

            gp.AddLine(rect.Left, rect.Bottom - size.Height / 2, rect.Left, rect.Top + size.Height / 2);
            gp.AddArc(rect.Left, rect.Top, size.Width, size.Height, 180, 90);
            return gp;
        }

        public static Color[] DesaturateColors(params Color[] colorsToDesaturate)
        {
            Color[] colorsToReturn = new Color[colorsToDesaturate.Length];
            for (int i = 0; i < colorsToDesaturate.Length; i++)
            {
                //use NTSC weighted avarage
                int gray =
                    (int)(colorsToDesaturate[i].R * 0.3 + colorsToDesaturate[i].G * 0.6 + colorsToDesaturate[i].B * 0.1);
                colorsToReturn[i] = Color.FromArgb(-0x010101 * (255 - gray) - 1);
            }
            return colorsToReturn;
        }

        public static Color[] LightenColors(params Color[] colorsToLighten)
        {
            Color[] colorsToReturn = new Color[colorsToLighten.Length];
            for (int i = 0; i < colorsToLighten.Length; i++)
            {
                colorsToReturn[i] = ControlPaint.Light(colorsToLighten[i]);
            }
            return colorsToReturn;
        }

        public void SetValue(int val)
        {
            if (val < barMinimum) Value = barMinimum;
            else if (val > barMaximum) Value = barMaximum;
            else Value = val;
        }

        private static bool IsPointInRect(Point pt, Rectangle rect)
        {
            if (pt.X > rect.Left & pt.X < rect.Right & pt.Y > rect.Top & pt.Y < rect.Bottom)
                return true;
            else return false;
        }



    }
}
