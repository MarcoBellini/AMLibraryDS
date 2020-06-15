using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Winamp.Components
{
    [ToolboxBitmap(typeof(Panel))]
    public class WinampScalePanel : Panel
    {
        public enum ScaleFieldLocations
        {
            TopOrLeft,
            BottomOrRight
        }

        public enum ScaleFieldOrientations
        {
            RightToLeft,
            LeftToRight,
            HorizontalMiddleOut,
            TopToBottom,
            BottomToTop,
            VerticalMiddleOut
        }

        private int _minimumScaleFieldHeight = 2;

        private Color _scaleFieldColor = Color.FromArgb(72, 76, 79);
        [Bindable(true)]
        [DefaultValue(typeof (Color), "72, 76, 79")]
        [Category("Appearance")]
        [Description("The color of the scale fields")]
        public Color ScaleFieldColor
        {
            get { return _scaleFieldColor; }
            set
            {
                _scaleFieldColor = value;
                Invalidate();
            }
        }

        private int _scaleFieldSize = 3;
        [Bindable(true)]
        [DefaultValue(3)]
        [Category("Appearance")]
        [Description("Gets or sets the size of the scale fields")]
        public int ScaleFieldSize
        {
            get { return _scaleFieldSize; }
            set
            {
                _scaleFieldSize = value;
                Invalidate();
            }
        }

        private int _scaleFieldSpacing = 1;
        [Bindable(true)]
        [DefaultValue(1)]
        [Category("Appearance")]
        [Description("Gets or sets the spacing between the scale fields")]
        public int ScaleFieldSpacing
        {
            get { return _scaleFieldSpacing; }
            set
            {
                _scaleFieldSpacing = value;
                Invalidate();
            }
        }

        private ScaleFieldOrientations _scaleFieldOrientation = ScaleFieldOrientations.LeftToRight;
        [Bindable(true)]
        [DefaultValue(typeof(ScaleFieldOrientations), "LeftToRight")]
        [Category("Appearance")]
        [Description("Gets or sets the orientation of the scale fields")]
        public ScaleFieldOrientations ScaleFieldOrientation
        {
            get { return _scaleFieldOrientation; }
            set
            {
                _scaleFieldOrientation = value;
                Invalidate();
            }
        }

        private ScaleFieldLocations _scaleFieldLocation = ScaleFieldLocations.TopOrLeft;
        [Bindable(true)]
        [DefaultValue(typeof(ScaleFieldLocations), "TopOrLeft")]
        [Category("Appearance")]
        [Description("Gets or sets the location of the scale fields")]
        public ScaleFieldLocations ScaleFieldLocation
        {
            get { return _scaleFieldLocation; }
            set
            {
                _scaleFieldLocation = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            int scaleFieldCount;
            int scaleFieldOtherSizeEnd;
            int scaleFieldOtherSizeStart;
            int scaleFieldOffset = 0;
            bool evenScaleFieldCount = false;

            if (ScaleFieldOrientation == ScaleFieldOrientations.LeftToRight || ScaleFieldOrientation == ScaleFieldOrientations.RightToLeft || ScaleFieldOrientation == ScaleFieldOrientations.HorizontalMiddleOut)
            {
                scaleFieldCount = (int)Math.Floor(((double) Width + (double) ScaleFieldSpacing)/((double) ScaleFieldSize + (double) ScaleFieldSpacing));
                scaleFieldOtherSizeEnd = Height;
                scaleFieldOtherSizeStart = _minimumScaleFieldHeight;

                if (ScaleFieldOrientation == ScaleFieldOrientations.HorizontalMiddleOut)
                {
                    scaleFieldOffset = (int)Math.Floor((decimal) scaleFieldCount/2);
                    evenScaleFieldCount = ((2*scaleFieldOffset) == scaleFieldCount);
                }

                for (int x = 0; x < scaleFieldCount; x++)
                {
                    int scaleFieldOtherSize;

                    if (ScaleFieldOrientation != ScaleFieldOrientations.HorizontalMiddleOut)
                    {
                        scaleFieldOtherSize = scaleFieldOtherSizeStart + (int)Math.Floor(((double) scaleFieldOtherSizeEnd - (double) scaleFieldOtherSizeStart)*((double) x/(double) scaleFieldCount));
                    }
                    else
                    {
                        if (evenScaleFieldCount)
                        {
                            if (x < scaleFieldOffset)
                            {
                                scaleFieldOtherSize = scaleFieldOtherSizeStart + (int)Math.Floor(((double)scaleFieldOtherSizeEnd - (double)scaleFieldOtherSizeStart) * ((double)(2*(scaleFieldOffset - x)) / (double)scaleFieldCount));
                            }
                            else
                            {
                                scaleFieldOtherSize = scaleFieldOtherSizeStart + (int)Math.Floor(((double)scaleFieldOtherSizeEnd - (double)scaleFieldOtherSizeStart) * ((double)(2*(x - scaleFieldOffset + 1)) / (double)scaleFieldCount));
                            }
                        }
                        else
                        {
                            if (x < scaleFieldOffset)
                            {
                                scaleFieldOtherSize = scaleFieldOtherSizeStart + (int)Math.Floor(((double)scaleFieldOtherSizeEnd - (double)scaleFieldOtherSizeStart) * ((double)(2 * (scaleFieldOffset - x)) / (double)scaleFieldCount));
                            }
                            else if (x == scaleFieldOffset)
                            {
                                scaleFieldOtherSize = scaleFieldOtherSizeStart;
                            }
                            else
                            {
                                scaleFieldOtherSize = scaleFieldOtherSizeStart + (int)Math.Floor(((double)scaleFieldOtherSizeEnd - (double)scaleFieldOtherSizeStart) * ((double)(2 * (x - scaleFieldOffset)) / (double)scaleFieldCount));
                            }
                        }
                    }

                    Size scaleFieldSize = new Size(ScaleFieldSize, scaleFieldOtherSize);

                    int scaleFieldXPos;

                    if (ScaleFieldOrientation == ScaleFieldOrientations.LeftToRight || ScaleFieldOrientation == ScaleFieldOrientations.HorizontalMiddleOut)
                        scaleFieldXPos = x * (ScaleFieldSize + ScaleFieldSpacing);
                    else
                        scaleFieldXPos = Width - (x * (ScaleFieldSize + ScaleFieldSpacing) + ScaleFieldSize);

                    int scaleFieldYPos;

                    if (ScaleFieldLocation == ScaleFieldLocations.TopOrLeft)
                        scaleFieldYPos = scaleFieldOtherSizeEnd - scaleFieldOtherSize;
                    else
                        scaleFieldYPos = 0;

                    Point scaleFieldLocation = new Point(scaleFieldXPos, scaleFieldYPos);

                    Rectangle scaleFieldRectangle = new Rectangle(scaleFieldLocation, scaleFieldSize);

                    using (Brush scaleFieldBrush = new SolidBrush(ScaleFieldColor))
                    {
                        e.Graphics.FillRectangle(scaleFieldBrush, scaleFieldRectangle);    
                    }
                }
            }
            else
            {
                scaleFieldCount = (int)Math.Floor(((double)Height + (double)ScaleFieldSpacing) / ((double)ScaleFieldSize + (double)ScaleFieldSpacing));
                scaleFieldOtherSizeEnd = Width;
                scaleFieldOtherSizeStart = _minimumScaleFieldHeight;

                if (ScaleFieldOrientation == ScaleFieldOrientations.VerticalMiddleOut)
                {
                    scaleFieldOffset = (int)Math.Floor((decimal)scaleFieldCount / 2);
                    evenScaleFieldCount = ((2 * scaleFieldOffset) == scaleFieldCount);
                }

                for (int x = 0; x < scaleFieldCount; x++)
                {
                    int scaleFieldOtherSize;

                    if (ScaleFieldOrientation != ScaleFieldOrientations.VerticalMiddleOut)
                    {
                        scaleFieldOtherSize = scaleFieldOtherSizeStart + (int)Math.Floor(((double)scaleFieldOtherSizeEnd - (double)scaleFieldOtherSizeStart) * ((double)x / (double)scaleFieldCount));
                    }
                    else
                    {
                        if (evenScaleFieldCount)
                        {
                            if (x < scaleFieldOffset)
                            {
                                scaleFieldOtherSize = scaleFieldOtherSizeStart + (int)Math.Floor(((double)scaleFieldOtherSizeEnd - (double)scaleFieldOtherSizeStart) * ((double)(2 * (scaleFieldOffset - x)) / (double)scaleFieldCount));
                            }
                            else
                            {
                                scaleFieldOtherSize = scaleFieldOtherSizeStart + (int)Math.Floor(((double)scaleFieldOtherSizeEnd - (double)scaleFieldOtherSizeStart) * ((double)(2 * (x - scaleFieldOffset + 1)) / (double)scaleFieldCount));
                            }
                        }
                        else
                        {
                            if (x < scaleFieldOffset)
                            {
                                scaleFieldOtherSize = scaleFieldOtherSizeStart + (int)Math.Floor(((double)scaleFieldOtherSizeEnd - (double)scaleFieldOtherSizeStart) * ((double)(2 * (scaleFieldOffset - x)) / (double)scaleFieldCount));
                            }
                            else if (x == scaleFieldOffset)
                            {
                                scaleFieldOtherSize = scaleFieldOtherSizeStart;
                            }
                            else
                            {
                                scaleFieldOtherSize = scaleFieldOtherSizeStart + (int)Math.Floor(((double)scaleFieldOtherSizeEnd - (double)scaleFieldOtherSizeStart) * ((double)(2 * (x - scaleFieldOffset)) / (double)scaleFieldCount));
                            }
                        }
                    }

                    Size scaleFieldSize = new Size(scaleFieldOtherSize, ScaleFieldSize);

                    int scaleFieldXPos;

                    if (ScaleFieldLocation == ScaleFieldLocations.TopOrLeft)
                        scaleFieldXPos = scaleFieldOtherSizeEnd - scaleFieldOtherSize;
                    else
                        scaleFieldXPos = 0;

                    int scaleFieldYPos;

                    if (ScaleFieldOrientation == ScaleFieldOrientations.TopToBottom)
                        scaleFieldYPos = x * (ScaleFieldSize + ScaleFieldSpacing);
                    else
                        scaleFieldYPos = Height - (x * (ScaleFieldSize + ScaleFieldSpacing) + ScaleFieldSize);

                    Point scaleFieldLocation = new Point(scaleFieldXPos, scaleFieldYPos);

                    Rectangle scaleFieldRectangle = new Rectangle(scaleFieldLocation, scaleFieldSize);

                    using (Brush scaleFieldBrush = new SolidBrush(ScaleFieldColor))
                    {
                        e.Graphics.FillRectangle(scaleFieldBrush, scaleFieldRectangle);
                    }
                }
            }
        }
    }
}
