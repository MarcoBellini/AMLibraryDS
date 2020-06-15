using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Winamp.Components
{
    public class VerticalWinampTrackBarRenderer : WinampTrackBarRenderer
    {
        private WinampTrackBar _trackBar;
        private const int AcceptableSpaceBetweenScaleAndTrackBar = 3;
        private const int AcceptableSpaceBetweenTickAndTrackBar = 2;

        #region Constructor

        public VerticalWinampTrackBarRenderer(WinampTrackBar trackBar) : base(trackBar)
        {
            if (trackBar == null)
                throw new Exception("You have to pass the TrackBar object in the constructor!");

            _trackBar = trackBar;
        }

        #endregion Constructor

        #region ScaleFields

        public override Rectangle GetScaleFieldLayoutRectangle()
        {
            if (_trackBar.ScaleType == WinampTrackBar.WinampTrackBarScaleType.ScaleFields)
            {
                int offset = GetOffset();

                switch (_trackBar.ScaleFieldPosition)
                {
                    case WinampTrackBar.WinampTrackBarScaleFieldPosition.LeftOrTop:
                        return new Rectangle(offset, 0, _trackBar.ScaleFieldMaxHeight, _trackBar.Height);
                        
                    case WinampTrackBar.WinampTrackBarScaleFieldPosition.RightOrBottom:
                        return new Rectangle(_trackBar.Width - offset - _trackBar.ScaleFieldMaxHeight, 0, _trackBar.ScaleFieldMaxHeight, _trackBar.Height);
                        
                }
            }

            return Rectangle.Empty;
        }

        public override void RenderScaleFields(Graphics g)
        {
            int fieldCount = CalculateScaleFieldCount(_trackBar);
            Dictionary<int, int> fieldHeights = CalculateScaleFieldHeights(_trackBar, fieldCount);

            int totalScaleHeight = CalculateTotalScaleWidth(_trackBar, fieldCount);
            int scaleOffset = (_trackBar.Height - totalScaleHeight) / 2;

            foreach (var fieldHeight in fieldHeights)
            {
                int fieldNumber = fieldCount - fieldHeight.Key - 1;

                int fieldValue = fieldHeights[fieldHeight.Key];
                int fieldX = 0;
                int fieldY = scaleOffset + (fieldNumber * (_trackBar.ScaleFieldWidth + _trackBar.ScaleFieldSpacing));

                if (_trackBar.ScaleFieldPosition == WinampTrackBar.WinampTrackBarScaleFieldPosition.LeftOrTop)
                    fieldX = _trackBar.ScaleFieldMaxHeight - fieldValue;

                Rectangle fieldRectangle = new Rectangle((int)g.ClipBounds.X + fieldX, (int)g.ClipBounds.Y + fieldY, fieldValue, _trackBar.ScaleFieldWidth);

                using (Brush fieldBrush = new SolidBrush(_trackBar.ScaleFieldColor))
                {
                    g.FillRectangle(fieldBrush, fieldRectangle);
                }
            }
        }

        #endregion ScaleFields

        #region Ticks

        public override Rectangle GetTickLayoutRectangle(WinampTrackBar.WinampTrackBarTickPosition position)
        {
            int tickFieldSize = GetTickFieldSize();
            int offset = GetOffset();

            int tickFieldLength = _trackBar.Height - 4;
            int lengthOffset = 0;

            if (_trackBar.IsSliderVisible)
            {
                tickFieldLength -= _trackBar.SliderButtonSize.Height;
                lengthOffset = _trackBar.SliderButtonSize.Height/2;
            }

            if (_trackBar.ScaleType == WinampTrackBar.WinampTrackBarScaleType.Ticks)
            {
                switch (position)
                {
                    case WinampTrackBar.WinampTrackBarTickPosition.LeftOrTop:
                        return new Rectangle(offset, lengthOffset + 2, tickFieldSize, tickFieldLength);
                        
                    case WinampTrackBar.WinampTrackBarTickPosition.RightOrBottom:
                        return new Rectangle(_trackBar.Width - offset - tickFieldSize - 1, lengthOffset + 2, tickFieldSize, tickFieldLength);
                        
                }
            }
            
            return Rectangle.Empty;
        }

        public override void RenderTicks(Graphics g, WinampTrackBar.WinampTrackBarTickPosition position)
        {
            int pixelValueZero = ValueToPixelValue(0);
            int pixelPos = pixelValueZero;

            if (_trackBar.TickWidth > 0)
                pixelPos = pixelPos - (_trackBar.TickWidth / 2);

            Rectangle zeroRectangle;

            if (_trackBar.TickEmphasizeMinMaxAndZero)
            {
                zeroRectangle = new Rectangle((int)g.ClipBounds.X, pixelPos, _trackBar.TickEmphasizedHeight, _trackBar.TickWidth);

                using (Brush zeroBrush = new SolidBrush(_trackBar.TickEmphasizedColor))
                {
                    g.FillRectangle(zeroBrush, zeroRectangle);
                }
            }
            else
            {
                zeroRectangle = new Rectangle((int)g.ClipBounds.X, pixelPos, _trackBar.TickHeight, _trackBar.TickWidth);

                using (Brush zeroBrush = new SolidBrush(_trackBar.TickColor))
                {
                    g.FillRectangle(zeroBrush, zeroRectangle);
                }
            }

            Rectangle tickRectangle;
            bool tickIsEmphasized;
            int tickHeight;
            int offsetX;

            if (_trackBar.Minimum == 0 || (_trackBar.Minimum < 0 && _trackBar.Maximum > 0))
            {
                //Paint ticks from 0 up to maximum

                int pixelPosCurrent = pixelPos;
                int pixelPosNext = pixelPosCurrent - _trackBar.TickWidth - _trackBar.TickSpacing;
                int nextTickMaxWidth = pixelPosNext - _trackBar.TickWidth;
                bool nextTickFits = nextTickMaxWidth >= (int)g.ClipBounds.Y;

                while (nextTickFits)
                {
                    pixelPosCurrent = pixelPosNext;
                    pixelPosNext = pixelPosCurrent - _trackBar.TickWidth - _trackBar.TickSpacing;
                    nextTickMaxWidth = pixelPosNext - _trackBar.TickWidth;
                    nextTickFits = nextTickMaxWidth >= (int)g.ClipBounds.Y;

                    tickIsEmphasized = _trackBar.TickEmphasizeMinMaxAndZero && !nextTickFits;

                    tickHeight = tickIsEmphasized ? _trackBar.TickEmphasizedHeight : _trackBar.TickHeight;
                    offsetX = 0;

                    if (!tickIsEmphasized)
                    {
                        if (_trackBar.TickAlignment == WinampTrackBar.WinampTrackBarTickAlignment.Center)
                        {
                            offsetX = (int)((g.ClipBounds.Width - tickHeight) / 2);
                        }
                        else if ((position == WinampTrackBar.WinampTrackBarTickPosition.LeftOrTop && _trackBar.TickAlignment == WinampTrackBar.WinampTrackBarTickAlignment.Far) ||
                                 (position == WinampTrackBar.WinampTrackBarTickPosition.RightOrBottom && _trackBar.TickAlignment == WinampTrackBar.WinampTrackBarTickAlignment.Near))
                        {
                            offsetX = 0;
                        }
                        else if ((position == WinampTrackBar.WinampTrackBarTickPosition.LeftOrTop && _trackBar.TickAlignment == WinampTrackBar.WinampTrackBarTickAlignment.Near) ||
                                 (position == WinampTrackBar.WinampTrackBarTickPosition.RightOrBottom && _trackBar.TickAlignment == WinampTrackBar.WinampTrackBarTickAlignment.Far))
                        {
                            offsetX = (int)g.ClipBounds.Width - tickHeight;
                        }
                    }

                    tickRectangle = new Rectangle((int)g.ClipBounds.X + offsetX, pixelPosCurrent, tickHeight, _trackBar.TickWidth);

                    if (tickIsEmphasized)
                    {
                        using (Brush tickBrush = new SolidBrush(_trackBar.TickEmphasizedColor))
                        {
                            g.FillRectangle(tickBrush, tickRectangle);
                        }
                    }
                    else
                    {
                        using (Brush tickBrush = new SolidBrush(_trackBar.TickColor))
                        {
                            g.FillRectangle(tickBrush, tickRectangle);
                        }
                    }
                }
            }


            if (_trackBar.Maximum == 0 || (_trackBar.Minimum < 0 && _trackBar.Maximum > 0))
            {
                //Paint ticks from 0 down to minimum

                int pixelPosCurrent = pixelPos;
                int pixelPosNext = pixelPosCurrent + _trackBar.TickWidth + _trackBar.TickSpacing;
                int nextTickMaxWidth = pixelPosNext + _trackBar.TickWidth;
                bool nextTickFits = nextTickMaxWidth <= (int)(g.ClipBounds.Y + g.ClipBounds.Height) - 1;

                while (nextTickFits)
                {
                    pixelPosCurrent = pixelPosNext;
                    pixelPosNext = pixelPosCurrent + _trackBar.TickWidth + _trackBar.TickSpacing;
                    nextTickMaxWidth = pixelPosNext + _trackBar.TickWidth;
                    nextTickFits = nextTickMaxWidth <= (int)(g.ClipBounds.Y + g.ClipBounds.Height) - 1;

                    tickIsEmphasized = _trackBar.TickEmphasizeMinMaxAndZero && !nextTickFits;

                    tickHeight = tickIsEmphasized ? _trackBar.TickEmphasizedHeight : _trackBar.TickHeight;
                    offsetX = 0;

                    if (!tickIsEmphasized)
                    {
                        if (_trackBar.TickAlignment == WinampTrackBar.WinampTrackBarTickAlignment.Center)
                        {
                            offsetX = (int)((g.ClipBounds.Width - tickHeight) / 2);
                        }
                        else if ((position == WinampTrackBar.WinampTrackBarTickPosition.LeftOrTop && _trackBar.TickAlignment == WinampTrackBar.WinampTrackBarTickAlignment.Far) ||
                                 (position == WinampTrackBar.WinampTrackBarTickPosition.RightOrBottom && _trackBar.TickAlignment == WinampTrackBar.WinampTrackBarTickAlignment.Near))
                        {
                            offsetX = 0;
                        }
                        else if ((position == WinampTrackBar.WinampTrackBarTickPosition.LeftOrTop && _trackBar.TickAlignment == WinampTrackBar.WinampTrackBarTickAlignment.Near) ||
                                 (position == WinampTrackBar.WinampTrackBarTickPosition.RightOrBottom && _trackBar.TickAlignment == WinampTrackBar.WinampTrackBarTickAlignment.Far))
                        {
                            offsetX = (int)g.ClipBounds.Width - tickHeight;
                        }
                    }

                    tickRectangle = new Rectangle((int)g.ClipBounds.X + offsetX, pixelPosCurrent, tickHeight, _trackBar.TickWidth);

                    if (tickIsEmphasized)
                    {
                        using (Brush tickBrush = new SolidBrush(_trackBar.TickEmphasizedColor))
                        {
                            g.FillRectangle(tickBrush, tickRectangle);
                        }
                    }
                    else
                    {
                        using (Brush tickBrush = new SolidBrush(_trackBar.TickColor))
                        {
                            g.FillRectangle(tickBrush, tickRectangle);
                        }
                    }
                }
            }
        }

        #endregion Ticks

        #region Track

        public override Rectangle GetTrackLayoutRectangle()
        {
            const int trackThickness = 5;

            int totalTrackThickness = GetTrackFieldSize();
            int offset = GetOffset();
            int trackOffset = (totalTrackThickness - trackThickness) / 2;

            if (_trackBar.ScaleType == WinampTrackBar.WinampTrackBarScaleType.ScaleFields)
            {
                switch (_trackBar.ScaleFieldPosition)
                {
                    case WinampTrackBar.WinampTrackBarScaleFieldPosition.LeftOrTop:
                        return new Rectangle(offset + _trackBar.ScaleFieldMaxHeight + AcceptableSpaceBetweenScaleAndTrackBar + trackOffset, 0, trackThickness, _trackBar.Height);
                        
                    case WinampTrackBar.WinampTrackBarScaleFieldPosition.RightOrBottom:
                        return new Rectangle(offset + trackOffset, 0, trackThickness, _trackBar.Height);
                        
                }
            }
            else if (_trackBar.ScaleType == WinampTrackBar.WinampTrackBarScaleType.Ticks)
            {
                int tickHeight = GetTickFieldSize();

                switch (_trackBar.TickPosition)
                {
                    case WinampTrackBar.WinampTrackBarTickPosition.Both:
                    case WinampTrackBar.WinampTrackBarTickPosition.LeftOrTop:
                        return new Rectangle(offset + tickHeight + AcceptableSpaceBetweenTickAndTrackBar + trackOffset, 0, trackThickness, _trackBar.Height);
                        
                    case WinampTrackBar.WinampTrackBarTickPosition.RightOrBottom:
                        return new Rectangle(offset + trackOffset, 0, trackThickness, _trackBar.Height);
                        
                }
            }

            return new Rectangle((_trackBar.Width - trackThickness) / 2, 0, trackThickness, _trackBar.Height);
        }

        public override void RenderTrack(Graphics g)
        {
            //Draw Track Border
            Color trackUpperBorderColor = Color.FromArgb(55, 60, 62);

            using (Brush trackUpperBrush = new SolidBrush(trackUpperBorderColor))
            {
                using (Pen trackUpperPen = new Pen(trackUpperBrush))
                {
                    g.DrawLine(trackUpperPen, g.ClipBounds.X + 1, g.ClipBounds.Y, g.ClipBounds.X + g.ClipBounds.Width - 2, g.ClipBounds.Y);
                }
            }

            Color trackUpperInnerBorderColor = Color.FromArgb(35, 38, 41);

            using (Brush trackUpperInnerBrush = new SolidBrush(trackUpperInnerBorderColor))
            {
                using (Pen trackUpperInnerPen = new Pen(trackUpperInnerBrush))
                {
                    g.DrawLine(trackUpperInnerPen, g.ClipBounds.X + 1, g.ClipBounds.Y + 1, g.ClipBounds.X + 1, g.ClipBounds.Y + g.ClipBounds.Height - 2);
                }
            }

            Color trackLowerBorderColor = Color.FromArgb(60, 65, 66);

            using (Brush trackLowerBrush = new SolidBrush(trackLowerBorderColor))
            {
                using (Pen trackLowerPen = new Pen(trackLowerBrush))
                {
                    g.DrawLine(trackLowerPen, g.ClipBounds.X + 1, g.ClipBounds.Y + g.ClipBounds.Height - 1, g.ClipBounds.X + g.ClipBounds.Width - 2, g.ClipBounds.Y + g.ClipBounds.Height - 1);
                }
            }

            RectangleF linearGradientRectangle = g.ClipBounds;
            linearGradientRectangle.Y += 2;
            linearGradientRectangle.Height -= 4;

            using (Brush trackSideBorderBrush = new LinearGradientBrush(linearGradientRectangle, trackUpperBorderColor, trackLowerBorderColor, LinearGradientMode.Vertical))
            {
                using (Pen trackSideBorderPen = new Pen(trackSideBorderBrush))
                {
                    g.DrawLine(trackSideBorderPen, g.ClipBounds.X, g.ClipBounds.Y + 1, g.ClipBounds.X, g.ClipBounds.Y + g.ClipBounds.Height - 2);
                    g.DrawLine(trackSideBorderPen, g.ClipBounds.X + g.ClipBounds.Width - 1, g.ClipBounds.Y + 1, g.ClipBounds.X + g.ClipBounds.Width - 1, g.ClipBounds.Y + g.ClipBounds.Height - 2);
                }
            }

            Color innerFieldColor = Color.FromArgb(20, 21, 21);
            
            using (Brush innerFieldBrush = new SolidBrush(innerFieldColor))
            {
                g.FillRectangle(innerFieldBrush, g.ClipBounds.X + 2, g.ClipBounds.Y + 1, 2, 1);
                g.FillRectangle(innerFieldBrush, g.ClipBounds.X + 2, g.ClipBounds.Y + g.ClipBounds.Height - 2, 2, 1);
            }

            //Fill Empty Track
            Color emptyTrackColor = _trackBar.EmptyTrackColor;

            using (Brush emptyTrackBrush = new SolidBrush(emptyTrackColor))
            {
                g.FillRectangle(emptyTrackBrush, g.ClipBounds.X + 2, g.ClipBounds.Y + 2, 2, g.ClipBounds.Height - 4);
            }

            if (_trackBar.TrackStyle != WinampTrackBar.WinampTrackBarTrackStyle.None)
            {
                //Paint Track

                int startPixel = 0;
                int endPixel = 0;

                Rectangle trackRectangle = GetTrackLayoutRectangle();

                if (_trackBar.TrackStyle == WinampTrackBar.WinampTrackBarTrackStyle.FromLeftOrTop || (_trackBar.TrackStyle == WinampTrackBar.WinampTrackBarTrackStyle.FromZero && _trackBar.Maximum == 0))
                {
                    startPixel = 2;
                    endPixel = ValueToPixelValue(_trackBar.Value);
                }
                else if (_trackBar.TrackStyle == WinampTrackBar.WinampTrackBarTrackStyle.FromRightOrBottom || (_trackBar.TrackStyle == WinampTrackBar.WinampTrackBarTrackStyle.FromZero && _trackBar.Minimum == 0))
                {
                    startPixel = ValueToPixelValue(_trackBar.Value);
                    endPixel = trackRectangle.Height - 3;
                }
                else if (_trackBar.TrackStyle == WinampTrackBar.WinampTrackBarTrackStyle.FromZero)
                {
                    startPixel = ValueToPixelValue(0);
                    endPixel = ValueToPixelValue(_trackBar.Value);
                }

                if (startPixel != endPixel)
                {
                    using (Brush upperTrackBrush = new SolidBrush(_trackBar.TrackUpperColor))
                    {
                        using (Pen upperTrackPen = new Pen(upperTrackBrush))
                        {
                            g.DrawLine(upperTrackPen, g.ClipBounds.X + 2, g.ClipBounds.Y + startPixel, g.ClipBounds.X + 2, g.ClipBounds.Y + endPixel);
                        }
                    }

                    using (Brush lowerTrackBrush = new SolidBrush(_trackBar.TrackLowerColor))
                    {
                        using (Pen lowerTrackPen = new Pen(lowerTrackBrush))
                        {
                            g.DrawLine(lowerTrackPen, g.ClipBounds.X + 3, g.ClipBounds.Y + startPixel, g.ClipBounds.X + 3, g.ClipBounds.Y + endPixel);
                        }
                    }
                }
            }
        }

        #endregion Track

        #region ClickArea

        public override Rectangle GetClickRectangle()
        {
            int totalTrackThickness = GetTrackFieldSize();
            int offset = GetOffset();

            if (_trackBar.ScaleType == WinampTrackBar.WinampTrackBarScaleType.ScaleFields)
            {
                switch (_trackBar.ScaleFieldPosition)
                {
                    case WinampTrackBar.WinampTrackBarScaleFieldPosition.LeftOrTop:
                        return new Rectangle(offset + _trackBar.ScaleFieldMaxHeight + AcceptableSpaceBetweenScaleAndTrackBar, 2, totalTrackThickness, _trackBar.Height - 4);
                        
                    case WinampTrackBar.WinampTrackBarScaleFieldPosition.RightOrBottom:
                        return new Rectangle(offset, 2, totalTrackThickness, _trackBar.Height - 4);
                        
                }
            }
            else if (_trackBar.ScaleType == WinampTrackBar.WinampTrackBarScaleType.Ticks)
            {
                int tickHeight = GetTickFieldSize();

                switch (_trackBar.TickPosition)
                {
                    case WinampTrackBar.WinampTrackBarTickPosition.Both:
                    case WinampTrackBar.WinampTrackBarTickPosition.LeftOrTop:
                        return new Rectangle(offset + tickHeight + AcceptableSpaceBetweenTickAndTrackBar, 2, totalTrackThickness, _trackBar.Height - 4);
                        
                    case WinampTrackBar.WinampTrackBarTickPosition.RightOrBottom:
                        return new Rectangle(offset, 2, totalTrackThickness, _trackBar.Height - 4);
                        
                }
            }

            return new Rectangle((_trackBar.Width - totalTrackThickness) / 2, 0, totalTrackThickness, _trackBar.Height - 4);
        }

        #endregion ClickArea

        #region Slider

        public override Rectangle GetSliderLayoutRectangle(int sliderValue)
        {
            if (!_trackBar.IsSliderVisible)
                return Rectangle.Empty;

            int sliderPixelValue = ValueToPixelValue(sliderValue);
            double dblPixelValue = ((double)sliderPixelValue - ((double)_trackBar.SliderButtonSize.Height / 2));
            int pixelValue = (int)dblPixelValue;

            int offset = GetOffset();

            if (_trackBar.ScaleType == WinampTrackBar.WinampTrackBarScaleType.ScaleFields)
            {
                switch (_trackBar.ScaleFieldPosition)
                {
                    case WinampTrackBar.WinampTrackBarScaleFieldPosition.LeftOrTop:
                        return new Rectangle(offset + _trackBar.ScaleFieldMaxHeight + AcceptableSpaceBetweenScaleAndTrackBar, pixelValue, _trackBar.SliderButtonSize.Width, _trackBar.SliderButtonSize.Height);
                        
                    case WinampTrackBar.WinampTrackBarScaleFieldPosition.RightOrBottom:
                        return new Rectangle(offset, pixelValue, _trackBar.SliderButtonSize.Width, _trackBar.SliderButtonSize.Height);
                        
                }
            }
            else if (_trackBar.ScaleType == WinampTrackBar.WinampTrackBarScaleType.Ticks)
            {
                int tickHeight = GetTickFieldSize();

                switch (_trackBar.TickPosition)
                {
                    case WinampTrackBar.WinampTrackBarTickPosition.Both:
                    case WinampTrackBar.WinampTrackBarTickPosition.LeftOrTop:
                        return new Rectangle(offset + tickHeight + AcceptableSpaceBetweenTickAndTrackBar, pixelValue, _trackBar.SliderButtonSize.Width, _trackBar.SliderButtonSize.Height);
                        
                    case WinampTrackBar.WinampTrackBarTickPosition.RightOrBottom:
                        return new Rectangle(offset, pixelValue, _trackBar.SliderButtonSize.Width, _trackBar.SliderButtonSize.Height);
                        
                }
            }

            return new Rectangle((_trackBar.Width - _trackBar.SliderButtonSize.Width) / 2, pixelValue, _trackBar.SliderButtonSize.Width, _trackBar.SliderButtonSize.Height);
        }

        #endregion Slider

        #region Helper Methods

        public override int GetTrackFieldSize()
        {
            return _trackBar.SliderButtonSize.Width;
        }

        public override int GetTickFieldSize()
        {
            int tickFieldSize = _trackBar.TickHeight;

            if (_trackBar.TickEmphasizeMinMaxAndZero && _trackBar.TickEmphasizedHeight > _trackBar.TickHeight)
                tickFieldSize = _trackBar.TickEmphasizedHeight;

            return tickFieldSize;
        }

        public override int GetTotalFieldSize()
        {
            int totalSize = GetTrackFieldSize();

            if (_trackBar.ScaleType == WinampTrackBar.WinampTrackBarScaleType.ScaleFields)
            {
                totalSize += AcceptableSpaceBetweenScaleAndTrackBar;
                totalSize += _trackBar.ScaleFieldMaxHeight;
            }
            else if (_trackBar.ScaleType == WinampTrackBar.WinampTrackBarScaleType.Ticks)
            {
                int tickFieldSize = GetTickFieldSize();

                totalSize += AcceptableSpaceBetweenTickAndTrackBar;
                totalSize += tickFieldSize;

                if (_trackBar.TickPosition == WinampTrackBar.WinampTrackBarTickPosition.Both)
                {
                    totalSize += AcceptableSpaceBetweenTickAndTrackBar;
                    totalSize += tickFieldSize;    
                }
            }

            return totalSize;
        }

        public override int GetOffset()
        {
            int totalSize = GetTotalFieldSize();
            return GetOffset(totalSize);
        }

        public override int GetOffset(int totalSize)
        {
            int offset = ((_trackBar.Width - totalSize) / 2);
            return offset;
        }

        #endregion Helper Methods

        #region Value Converters

        public override int ValueToPixelValue(int value)
        {
            Rectangle trackRectangle = GetTrackLayoutRectangle();
            int maxPixelCount = trackRectangle.Height - 4; //Adjust for track border
            int maxPixelValue = maxPixelCount - 1; //Adjust for pixel array

            int valueOffset = _trackBar.Minimum;
            int adjustedMaxValue = _trackBar.Maximum - valueOffset;

            int adjustedValue = value - valueOffset;

            int pixelValue = 0;
            double dblPixelValue = 0;
            double factor = 0;

            if (!_trackBar.IsSliderVisible)
            {
                factor = ((double)maxPixelValue / (double)adjustedMaxValue);
                dblPixelValue = factor * (double)adjustedValue;
            }
            else
            {
                int trackButtonPixelSize = _trackBar.SliderButtonSize.Height;
                double trackButtonOffset = (double)trackButtonPixelSize / 2;

                int adjustedMaxPixelValue = maxPixelValue - trackButtonPixelSize;

                factor = ((double)adjustedMaxPixelValue / (double)adjustedMaxValue);
                dblPixelValue = factor * (double)adjustedValue;
                dblPixelValue += (double)trackButtonOffset;
            }

            pixelValue = (int)((double)maxPixelValue - dblPixelValue); //Invert Y-axis

            if (pixelValue < 0)
                pixelValue = 0;

            if (pixelValue > maxPixelCount)
                pixelValue = maxPixelCount;

            pixelValue += 2; //Adjust for track border

            return pixelValue;
        }

        public override int PixelValueToValue(int pixelValue)
        {
            pixelValue -= 2; //Adjust for track border

            Rectangle trackRectangle = GetTrackLayoutRectangle();
            int maxPixelCount = trackRectangle.Height - 4; //Adjust for track border
            int maxPixelValue = maxPixelCount - 1; //Adjust for pixel array
            maxPixelValue = (int)((int)((double)maxPixelValue / 2) * 2); //Make value equal number

            int valueOffset = _trackBar.Minimum;
            int adjustedMaxValue = _trackBar.Maximum - valueOffset;

            int returnValue = 0;
            double dblReturnValue = 0;
            double factor = 0;

            pixelValue = maxPixelValue - pixelValue; //Invert Y-axis

            if (!_trackBar.IsSliderVisible)
            {
                factor = ((double)adjustedMaxValue / (double)maxPixelValue);
                dblReturnValue = factor * (double)pixelValue;
            }
            else
            {
                int trackButtonPixelSize = _trackBar.SliderButtonSize.Height;
                int trackButtonOffset = trackButtonPixelSize / 2;

                int adjustedMaxPixelCount = maxPixelValue - trackButtonPixelSize;
                pixelValue -= trackButtonOffset;

                factor = ((double)adjustedMaxValue / (double)adjustedMaxPixelCount);
                dblReturnValue = factor * (double)pixelValue;
            }

            returnValue = (int)Math.Round(dblReturnValue, 0);

            returnValue += valueOffset;

            if (returnValue < _trackBar.Minimum)
                returnValue = _trackBar.Minimum;

            if (returnValue > _trackBar.Maximum)
                returnValue = _trackBar.Maximum;

            return returnValue;
        }

        #endregion Value Converters
    }
}
