using System;
using System.Collections.Generic;
using System.Drawing;

namespace Winamp.Components
{
    public class HorizontalWinampTrackBarRenderer : WinampTrackBarRenderer
    {
        private WinampTrackBar _trackBar;
        private const int AcceptableSpaceBetweenScaleAndTrackBar = 3;
        private const int AcceptableSpaceBetweenTickAndTrackBar = 2;

        #region Constructor

        public HorizontalWinampTrackBarRenderer(WinampTrackBar trackBar) : base(trackBar)
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
                        return new Rectangle(0, offset, _trackBar.Width, _trackBar.ScaleFieldMaxHeight);                       
                    case WinampTrackBar.WinampTrackBarScaleFieldPosition.RightOrBottom:
                        return new Rectangle(0, _trackBar.Height - offset - _trackBar.ScaleFieldMaxHeight, _trackBar.Width, _trackBar.ScaleFieldMaxHeight);                        
                }
            }

            return Rectangle.Empty;
        }

        public override void RenderScaleFields(Graphics g)
        {
            int fieldCount = CalculateScaleFieldCount(_trackBar);
            Dictionary<int, int> fieldHeights = CalculateScaleFieldHeights(_trackBar, fieldCount);

            int totalScaleWidth = CalculateTotalScaleWidth(_trackBar, fieldCount);
            int scaleOffset = (_trackBar.Width - totalScaleWidth) / 2;

            foreach (var fieldHeight in fieldHeights)
            {
                int fieldNumber = fieldHeight.Key;

                int fieldValue = fieldHeights[fieldNumber];
                int fieldX = scaleOffset + (fieldNumber * (_trackBar.ScaleFieldWidth + _trackBar.ScaleFieldSpacing));
                int fieldY = 0;

                if (_trackBar.ScaleFieldPosition == WinampTrackBar.WinampTrackBarScaleFieldPosition.LeftOrTop)
                    fieldY = _trackBar.ScaleFieldMaxHeight - fieldValue;

                Rectangle fieldRectangle = new Rectangle((int)g.ClipBounds.X + fieldX, (int)g.ClipBounds.Y + fieldY, _trackBar.ScaleFieldWidth, fieldValue);

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

            int tickFieldLength = _trackBar.Width - 4;
            int lengthOffset = 0;

            if (_trackBar.IsSliderVisible)
            {
                tickFieldLength -= _trackBar.SliderButtonSize.Width;
                lengthOffset = _trackBar.SliderButtonSize.Width/2;
            }

            if (_trackBar.ScaleType == WinampTrackBar.WinampTrackBarScaleType.Ticks)
            {
                switch (position)
                {
                    case WinampTrackBar.WinampTrackBarTickPosition.LeftOrTop:
                        return new Rectangle(lengthOffset + 2, offset, tickFieldLength, tickFieldSize);
                        
                    case WinampTrackBar.WinampTrackBarTickPosition.RightOrBottom:
                        return new Rectangle(lengthOffset + 2, _trackBar.Height - offset - tickFieldSize - 1, tickFieldLength, tickFieldSize);
                        
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
                zeroRectangle = new Rectangle(pixelPos, (int)g.ClipBounds.Y, _trackBar.TickWidth, _trackBar.TickEmphasizedHeight);

                using (Brush zeroBrush = new SolidBrush(_trackBar.TickEmphasizedColor))
                {
                    g.FillRectangle(zeroBrush, zeroRectangle);
                }
            }
            else
            {
                zeroRectangle = new Rectangle(pixelPos, (int)g.ClipBounds.Y, _trackBar.TickWidth, _trackBar.TickHeight);

                using (Brush zeroBrush = new SolidBrush(_trackBar.TickColor))
                {
                    g.FillRectangle(zeroBrush, zeroRectangle);
                }
            }

            Rectangle tickRectangle;
            bool tickIsEmphasized;
            int tickHeight;
            int offsetY;

            if (_trackBar.Minimum == 0 || (_trackBar.Minimum < 0 && _trackBar.Maximum > 0))
            {
                //Paint ticks from 0 up to maximum

                int pixelPosCurrent = pixelPos;
                int pixelPosNext = pixelPosCurrent + _trackBar.TickWidth + _trackBar.TickSpacing;
                int nextTickMaxWidth = pixelPosNext + _trackBar.TickWidth;
                bool nextTickFits = nextTickMaxWidth <= (int) (g.ClipBounds.X + g.ClipBounds.Width) - 1;

                while (nextTickFits)
                {
                    pixelPosCurrent = pixelPosNext;
                    pixelPosNext = pixelPosCurrent + _trackBar.TickWidth + _trackBar.TickSpacing;
                    nextTickMaxWidth = pixelPosNext + _trackBar.TickWidth;
                    nextTickFits = nextTickMaxWidth <= (int)(g.ClipBounds.X + g.ClipBounds.Width) - 1;

                    tickIsEmphasized = _trackBar.TickEmphasizeMinMaxAndZero && !nextTickFits;

                    tickHeight = tickIsEmphasized ? _trackBar.TickEmphasizedHeight : _trackBar.TickHeight;
                    offsetY = 0;

                    if (!tickIsEmphasized)
                    {
                        if (_trackBar.TickAlignment == WinampTrackBar.WinampTrackBarTickAlignment.Center)
                        {
                            offsetY = (int)((g.ClipBounds.Height - tickHeight) / 2);
                        }
                        else if ((position == WinampTrackBar.WinampTrackBarTickPosition.LeftOrTop && _trackBar.TickAlignment == WinampTrackBar.WinampTrackBarTickAlignment.Far) ||
                                 (position == WinampTrackBar.WinampTrackBarTickPosition.RightOrBottom && _trackBar.TickAlignment == WinampTrackBar.WinampTrackBarTickAlignment.Near))
                        {
                            offsetY = 0;
                        }
                        else if ((position == WinampTrackBar.WinampTrackBarTickPosition.LeftOrTop && _trackBar.TickAlignment == WinampTrackBar.WinampTrackBarTickAlignment.Near) ||
                                 (position == WinampTrackBar.WinampTrackBarTickPosition.RightOrBottom && _trackBar.TickAlignment == WinampTrackBar.WinampTrackBarTickAlignment.Far))
                        {
                            offsetY = (int)g.ClipBounds.Height - tickHeight;
                        }
                    }

                    tickRectangle = new Rectangle(pixelPosCurrent, (int)g.ClipBounds.Y + offsetY, _trackBar.TickWidth, tickHeight);

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
                int pixelPosNext = pixelPosCurrent - _trackBar.TickWidth - _trackBar.TickSpacing;
                int nextTickMaxWidth = pixelPosNext - _trackBar.TickWidth;
                bool nextTickFits = nextTickMaxWidth >= (int)g.ClipBounds.X;

                while (nextTickFits)
                {
                    pixelPosCurrent = pixelPosNext;
                    pixelPosNext = pixelPosCurrent - _trackBar.TickWidth - _trackBar.TickSpacing;
                    nextTickMaxWidth = pixelPosNext - _trackBar.TickWidth;
                    nextTickFits = nextTickMaxWidth >= (int)g.ClipBounds.X;

                    tickIsEmphasized = _trackBar.TickEmphasizeMinMaxAndZero && !nextTickFits;

                    tickHeight = tickIsEmphasized ? _trackBar.TickEmphasizedHeight : _trackBar.TickHeight;
                    offsetY = 0;

                    if (!tickIsEmphasized)
                    {
                        if (_trackBar.TickAlignment == WinampTrackBar.WinampTrackBarTickAlignment.Center)
                        {
                            offsetY = (int)((g.ClipBounds.Height - tickHeight) / 2);
                        }
                        else if ((position == WinampTrackBar.WinampTrackBarTickPosition.LeftOrTop && _trackBar.TickAlignment == WinampTrackBar.WinampTrackBarTickAlignment.Far) ||
                                 (position == WinampTrackBar.WinampTrackBarTickPosition.RightOrBottom && _trackBar.TickAlignment == WinampTrackBar.WinampTrackBarTickAlignment.Near))
                        {
                            offsetY = 0;
                        }
                        else if ((position == WinampTrackBar.WinampTrackBarTickPosition.LeftOrTop && _trackBar.TickAlignment == WinampTrackBar.WinampTrackBarTickAlignment.Near) ||
                                 (position == WinampTrackBar.WinampTrackBarTickPosition.RightOrBottom && _trackBar.TickAlignment == WinampTrackBar.WinampTrackBarTickAlignment.Far))
                        {
                            offsetY = (int)g.ClipBounds.Height - tickHeight;
                        }
                    }

                    tickRectangle = new Rectangle(pixelPosCurrent, (int)g.ClipBounds.Y + offsetY, _trackBar.TickWidth, tickHeight);

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
            int trackOffset = (totalTrackThickness - trackThickness)/2;

            if (_trackBar.ScaleType == WinampTrackBar.WinampTrackBarScaleType.ScaleFields)
            {
                switch (_trackBar.ScaleFieldPosition)
                {
                    case WinampTrackBar.WinampTrackBarScaleFieldPosition.LeftOrTop:
                        return new Rectangle(0, offset + _trackBar.ScaleFieldMaxHeight + AcceptableSpaceBetweenScaleAndTrackBar + trackOffset , _trackBar.Width, trackThickness);
                        
                    case WinampTrackBar.WinampTrackBarScaleFieldPosition.RightOrBottom:
                        return new Rectangle(0, offset + trackOffset, _trackBar.Width, trackThickness);
                        
                }
            }
            else if (_trackBar.ScaleType == WinampTrackBar.WinampTrackBarScaleType.Ticks)
            {
                int tickHeight = GetTickFieldSize();

                switch (_trackBar.TickPosition)
                {
                    case WinampTrackBar.WinampTrackBarTickPosition.Both:
                    case WinampTrackBar.WinampTrackBarTickPosition.LeftOrTop:
                        return new Rectangle(0, offset + tickHeight + AcceptableSpaceBetweenTickAndTrackBar + trackOffset, _trackBar.Width, trackThickness);
                        
                    case WinampTrackBar.WinampTrackBarTickPosition.RightOrBottom:
                        return new Rectangle(0, offset + trackOffset, _trackBar.Width, trackThickness);
                        
                }
            }

            return new Rectangle(0, (_trackBar.Height - trackThickness) / 2, _trackBar.Width, trackThickness);
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
                    g.FillRectangle(trackUpperBrush, g.ClipBounds.X, g.ClipBounds.Y + 1, 1, 1);
                    g.FillRectangle(trackUpperBrush, g.ClipBounds.X + g.ClipBounds.Width - 1, g.ClipBounds.Y + 1, 1, 1);
                }
            }

            Color trackUpperInnerBorderColor = Color.FromArgb(35, 38, 41);

            using (Brush trackUpperInnerBrush = new SolidBrush(trackUpperInnerBorderColor))
            {
                using (Pen trackUpperInnerPen = new Pen(trackUpperInnerBrush))
                {
                    g.DrawLine(trackUpperInnerPen, g.ClipBounds.X + 1, g.ClipBounds.Y + 1, g.ClipBounds.X + g.ClipBounds.Width - 2, g.ClipBounds.Y + 1);
                }
            }

            Color trackLowerBorderColor = Color.FromArgb(60, 65, 66);

            using (Brush trackLowerBrush = new SolidBrush(trackLowerBorderColor))
            {
                using (Pen trackLowerPen = new Pen(trackLowerBrush))
                {
                    g.DrawLine(trackLowerPen, g.ClipBounds.X + 1, g.ClipBounds.Y + g.ClipBounds.Height - 1, g.ClipBounds.X + g.ClipBounds.Width - 2, g.ClipBounds.Y + g.ClipBounds.Height - 1);
                    g.FillRectangle(trackLowerBrush, g.ClipBounds.X, g.ClipBounds.Y + g.ClipBounds.Height - 3, 1, 2);
                    g.FillRectangle(trackLowerBrush, g.ClipBounds.X + g.ClipBounds.Width - 1, g.ClipBounds.Y + g.ClipBounds.Height - 3, 1, 2);
                }
            }

            Color innerFieldColor = Color.FromArgb(20, 21, 21);

            using (Brush innerFieldBrush = new SolidBrush(innerFieldColor))
            {
                g.FillRectangle(innerFieldBrush, g.ClipBounds.X + 1, g.ClipBounds.Y + 2, 1, 2);
                g.FillRectangle(innerFieldBrush, g.ClipBounds.X + g.ClipBounds.Width - 2, g.ClipBounds.Y + 2, 1, 2);
            }

            //Fill Empty Track
            Color emptyTrackColor = _trackBar.EmptyTrackColor;

            using (Brush emptyTrackBrush = new SolidBrush(emptyTrackColor))
            {
                g.FillRectangle(emptyTrackBrush, g.ClipBounds.X + 2, g.ClipBounds.Y + 2, g.ClipBounds.Width - 4, 2);
            }

            if (_trackBar.TrackStyle != WinampTrackBar.WinampTrackBarTrackStyle.None)
            {
                //Paint Track

                int startPixel = 0;
                int endPixel = 0;

                Rectangle trackRectangle = GetTrackLayoutRectangle();

                if (_trackBar.TrackStyle == WinampTrackBar.WinampTrackBarTrackStyle.FromLeftOrTop || (_trackBar.TrackStyle == WinampTrackBar.WinampTrackBarTrackStyle.FromZero && _trackBar.Minimum == 0))
                {
                    startPixel = 2;
                    endPixel = ValueToPixelValue(_trackBar.Value);
                }
                else if (_trackBar.TrackStyle == WinampTrackBar.WinampTrackBarTrackStyle.FromRightOrBottom || (_trackBar.TrackStyle == WinampTrackBar.WinampTrackBarTrackStyle.FromZero && _trackBar.Maximum == 0))
                {
                    startPixel = ValueToPixelValue(_trackBar.Value);
                    endPixel = trackRectangle.Width - 3;
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
                            g.DrawLine(upperTrackPen, g.ClipBounds.X + startPixel, g.ClipBounds.Y + 2, g.ClipBounds.X + endPixel, g.ClipBounds.Y + 2);
                        }
                    }

                    using (Brush lowerTrackBrush = new SolidBrush(_trackBar.TrackLowerColor))
                    {
                        using (Pen lowerTrackPen = new Pen(lowerTrackBrush))
                        {
                            g.DrawLine(lowerTrackPen, g.ClipBounds.X + startPixel, g.ClipBounds.Y + 3, g.ClipBounds.X + endPixel, g.ClipBounds.Y + 3);
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
                        return new Rectangle(2, offset + _trackBar.ScaleFieldMaxHeight + AcceptableSpaceBetweenScaleAndTrackBar , _trackBar.Width - 4, totalTrackThickness);
                        
                    case WinampTrackBar.WinampTrackBarScaleFieldPosition.RightOrBottom:
                        return new Rectangle(2, offset, _trackBar.Width - 4, totalTrackThickness);
                       
                }
            }
            else if (_trackBar.ScaleType == WinampTrackBar.WinampTrackBarScaleType.Ticks)
            {
                int tickHeight = GetTickFieldSize();

                switch (_trackBar.TickPosition)
                {
                    case WinampTrackBar.WinampTrackBarTickPosition.Both:
                    case WinampTrackBar.WinampTrackBarTickPosition.LeftOrTop:
                        return new Rectangle(2, offset + tickHeight + AcceptableSpaceBetweenTickAndTrackBar, _trackBar.Width - 4, totalTrackThickness);
                        
                    case WinampTrackBar.WinampTrackBarTickPosition.RightOrBottom:
                        return new Rectangle(2, offset, _trackBar.Width - 4, totalTrackThickness);
                        
                }
            }

            return new Rectangle(2, ((_trackBar.Height - totalTrackThickness) / 2), _trackBar.Width - 4, totalTrackThickness);
        }

        #endregion ClickArea

        #region Slider

        public override Rectangle GetSliderLayoutRectangle(int sliderValue)
        {
            if (!_trackBar.IsSliderVisible)
                return Rectangle.Empty;

            int sliderPixelValue = ValueToPixelValue(sliderValue);
            double dblPixelValue = ((double)sliderPixelValue - ((double)_trackBar.SliderButtonSize.Width / 2));
            int pixelValue = (int)dblPixelValue;

            int offset = GetOffset();
            
            if (_trackBar.ScaleType == WinampTrackBar.WinampTrackBarScaleType.ScaleFields)
            {
                switch (_trackBar.ScaleFieldPosition)
                {
                    case WinampTrackBar.WinampTrackBarScaleFieldPosition.LeftOrTop:
                        return new Rectangle(pixelValue, offset + _trackBar.ScaleFieldMaxHeight + AcceptableSpaceBetweenScaleAndTrackBar, _trackBar.SliderButtonSize.Width, _trackBar.SliderButtonSize.Height);
                        
                    case WinampTrackBar.WinampTrackBarScaleFieldPosition.RightOrBottom:
                        return new Rectangle(pixelValue, offset, _trackBar.SliderButtonSize.Width, _trackBar.SliderButtonSize.Height);
                        
                }    
            }
            else if (_trackBar.ScaleType == WinampTrackBar.WinampTrackBarScaleType.Ticks)
            {
                int tickHeight = GetTickFieldSize();

                switch (_trackBar.TickPosition)
                {
                    case WinampTrackBar.WinampTrackBarTickPosition.Both:
                    case WinampTrackBar.WinampTrackBarTickPosition.LeftOrTop:
                        return new Rectangle(pixelValue, offset + tickHeight + AcceptableSpaceBetweenTickAndTrackBar, _trackBar.SliderButtonSize.Width, _trackBar.SliderButtonSize.Height);
                        
                    case WinampTrackBar.WinampTrackBarTickPosition.RightOrBottom:
                        return new Rectangle(pixelValue, offset, _trackBar.SliderButtonSize.Width, _trackBar.SliderButtonSize.Height);
                        
                }
            }

            return new Rectangle(pixelValue, (_trackBar.Height - _trackBar.SliderButtonSize.Height) / 2, _trackBar.SliderButtonSize.Width, _trackBar.SliderButtonSize.Height);
        }

        #endregion Slider

        #region Helper Methods

        public override int GetTrackFieldSize()
        {
            return _trackBar.SliderButtonSize.Height;
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
            int offset = ((_trackBar.Height - totalSize) / 2);
            return offset;
        }

        #endregion Helper Methods

        #region Value Converters

        public override int ValueToPixelValue(int value)
        {
            Rectangle trackRectangle = GetTrackLayoutRectangle();
            int maxPixelCount = trackRectangle.Width - 4; //Adjust for track border
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
                int trackButtonPixelSize = _trackBar.SliderButtonSize.Width;
                double trackButtonOffset = (double)trackButtonPixelSize / 2;

                int adjustedMaxPixelValue = maxPixelValue - trackButtonPixelSize;

                factor = ((double)adjustedMaxPixelValue / (double)adjustedMaxValue);
                dblPixelValue = factor * (double)adjustedValue;
                dblPixelValue += (double)trackButtonOffset;
            }

            pixelValue = (int)Math.Round(dblPixelValue, 0);

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
            int maxPixelCount = trackRectangle.Width - 4; //Adjust for track border
            int maxPixelValue = maxPixelCount - 1; //Adjust for pixel array
            maxPixelValue = (int) ((int)((double) maxPixelValue/2)*2); //Make value equal number

            int valueOffset = _trackBar.Minimum;
            int adjustedMaxValue = _trackBar.Maximum - valueOffset;

            int returnValue = 0;
            double dblReturnValue = 0;
            double factor = 0;

            if (!_trackBar.IsSliderVisible)
            {
                factor = ((double)adjustedMaxValue / (double)maxPixelValue);
                dblReturnValue = factor * (double)pixelValue;
            }
            else
            {
                int trackButtonPixelSize = _trackBar.SliderButtonSize.Width;
                int trackButtonOffset = trackButtonPixelSize / 2;

                int adjustedMaxPixelValue = maxPixelValue - trackButtonPixelSize;
                pixelValue -= trackButtonOffset;

                factor = ((double)adjustedMaxValue / (double)adjustedMaxPixelValue);
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
