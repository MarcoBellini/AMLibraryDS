using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Winamp.Components
{
    public abstract class WinampTrackBarRenderer
    {
        private WinampTrackBar _trackBar;

        #region Constructor

        protected WinampTrackBarRenderer(WinampTrackBar trackBar)
        {
            if (trackBar == null)
                throw new Exception("You have to pass the TrackBar object in the constructor!");

            _trackBar = trackBar;
        }

        #endregion Constructor

        #region Render Methods

        public void RenderControl(PaintEventArgs e)
        {
            if (_trackBar.ScaleType == WinampTrackBar.WinampTrackBarScaleType.ScaleFields)
            {
                Rectangle scaleRectangle = GetScaleFieldLayoutRectangle();

                if (scaleRectangle != Rectangle.Empty)
                {
                    e.Graphics.SetClip(scaleRectangle);
                    RenderScaleFields(e.Graphics);
                    e.Graphics.ResetClip();
                }
            }
            else if (_trackBar.ScaleType == WinampTrackBar.WinampTrackBarScaleType.Ticks)
            {
                if (_trackBar.TickPosition == WinampTrackBar.WinampTrackBarTickPosition.LeftOrTop ||
                    _trackBar.TickPosition == WinampTrackBar.WinampTrackBarTickPosition.Both)
                {
                    Rectangle tickRectangle = GetTickLayoutRectangle(WinampTrackBar.WinampTrackBarTickPosition.LeftOrTop);

                    if (tickRectangle != Rectangle.Empty)
                    {
                        e.Graphics.SetClip(tickRectangle);
                        RenderTicks(e.Graphics, WinampTrackBar.WinampTrackBarTickPosition.LeftOrTop);
                        e.Graphics.ResetClip();
                    }
                }

                if (_trackBar.TickPosition == WinampTrackBar.WinampTrackBarTickPosition.RightOrBottom ||
                    _trackBar.TickPosition == WinampTrackBar.WinampTrackBarTickPosition.Both)
                {
                    Rectangle tickRectangle = GetTickLayoutRectangle(WinampTrackBar.WinampTrackBarTickPosition.RightOrBottom);

                    if (tickRectangle != Rectangle.Empty)
                    {
                        e.Graphics.SetClip(tickRectangle);
                        RenderTicks(e.Graphics, WinampTrackBar.WinampTrackBarTickPosition.RightOrBottom);
                        e.Graphics.ResetClip();
                    }
                }
            }

            Rectangle trackRectangle = GetTrackLayoutRectangle();

            e.Graphics.SetClip(trackRectangle);
            RenderTrack(e.Graphics);
            e.Graphics.ResetClip();

            if (_trackBar.IsSliderVisible)
            {
                Rectangle sliderRectangle = GetSliderLayoutRectangle(_trackBar.Value);
                sliderRectangle.Inflate(1, 1); //Shadow
                
                if (sliderRectangle != Rectangle.Empty)
                {
                    e.Graphics.SetClip(sliderRectangle);
                    RenderSlider(e.Graphics, 255, (_trackBar.IsSliderHovered && _trackBar.UseHoverEffect));
                    e.Graphics.ResetClip();
                }
            }

            if (_trackBar.IsSeeking && _trackBar.UseSeeking)
            {
                Rectangle seekSliderRectangle = GetSliderLayoutRectangle(_trackBar.SeekValue);
                seekSliderRectangle.Inflate(1, 1); //Shadow

                if (seekSliderRectangle != Rectangle.Empty)
                {
                    e.Graphics.SetClip(seekSliderRectangle);
                    RenderSlider(e.Graphics, _trackBar.SeekSliderTransparency, false);
                    e.Graphics.ResetClip();
                }
            }
        }

        public abstract Rectangle GetTickLayoutRectangle(WinampTrackBar.WinampTrackBarTickPosition position);
        public abstract void RenderTicks(Graphics g, WinampTrackBar.WinampTrackBarTickPosition position);

        public abstract Rectangle GetScaleFieldLayoutRectangle();
        public abstract void RenderScaleFields(Graphics g);

        public abstract Rectangle GetTrackLayoutRectangle();
        public abstract void RenderTrack(Graphics g);

        public abstract Rectangle GetClickRectangle();
        public abstract Rectangle GetSliderLayoutRectangle(int sliderValue);

        public abstract int ValueToPixelValue(int value);
        public abstract int PixelValueToValue(int pixelValue);

        public abstract int GetTrackFieldSize();
        public abstract int GetTickFieldSize();
        public abstract int GetTotalFieldSize();
        public abstract int GetOffset();
        public abstract int GetOffset(int totalSize);

        #endregion Render Methods

        #region Scale Fields

        public int CalculateScaleFieldCount(WinampTrackBar trackBar)
        {
            int width = trackBar.Orientation == Orientation.Horizontal ? trackBar.Width : trackBar.Height;

            return (int)Math.Floor(((double)width + (double)trackBar.ScaleFieldSpacing) / ((double)trackBar.ScaleFieldWidth + (double)trackBar.ScaleFieldSpacing));
        }

        public int CalculateTotalScaleWidth(WinampTrackBar trackBar, int fieldCount)
        {
            return (fieldCount * trackBar.ScaleFieldWidth) + ((fieldCount - 1) * trackBar.ScaleFieldSpacing);
        }

        public Dictionary<int, int> CalculateScaleFieldHeights(WinampTrackBar trackBar, int fieldCount)
        {
            Dictionary<int, int> heightList = new Dictionary<int, int>();

            if (fieldCount > 0)
            {
                double stepWidth;

                if (trackBar.Maximum >= trackBar.Minimum)
                {
                    //Normal

                    if ((trackBar.Maximum >= 0 && trackBar.Minimum >= 0) || (trackBar.Maximum <= 0 && trackBar.Minimum <= 0))
                    {
                        stepWidth = 100 / ((double)fieldCount - 1);

                        for (int i = 0; i < fieldCount; i++)
                        {
                            double stepValue = i * stepWidth;
                            int absoluteStepValue = (int)Math.Abs(stepValue);

                            double fraction = (double)absoluteStepValue / 100;
                            double height = (fraction * ((double)trackBar.ScaleFieldMaxHeight - 2)) + 2;

                            heightList.Add(i, (int)height);
                        }
                    }
                    else
                    {
                        stepWidth = ((double)trackBar.Maximum - (double)trackBar.Minimum) / ((double)fieldCount - 1);

                        for (int i = 0; i < fieldCount; i++)
                        {
                            double stepValue = (double)trackBar.Minimum + i * stepWidth;
                            int absoluteStepValue = (int)Math.Abs(stepValue);

                            double fraction;

                            if (trackBar.ScaleFieldEqualizeHeights)
                            {
                                if (stepValue < 0)
                                    fraction = (double)absoluteStepValue / (double)Math.Abs(trackBar.Minimum);
                                else
                                    fraction = (double)absoluteStepValue / (double)Math.Abs(trackBar.Maximum);
                            }
                            else
                            {
                                fraction = (double)absoluteStepValue / (double)Math.Max(Math.Abs(trackBar.Maximum), Math.Abs(trackBar.Minimum));
                            }

                            double height = (fraction * ((double)trackBar.ScaleFieldMaxHeight - 2)) + 2;

                            heightList.Add(i, (int)height);
                        }
                    }
                }
                else
                {
                    //Turn that thing around...

                    if ((trackBar.Maximum >= 0 && trackBar.Minimum >= 0) || (trackBar.Maximum <= 0 && trackBar.Minimum <= 0))
                    {
                        stepWidth = 100 / ((double)fieldCount - 1);

                        for (int i = 0; i < fieldCount; i++)
                        {
                            double stepValue = 100 - i * stepWidth;
                            int absoluteStepValue = (int)Math.Abs(stepValue);

                            double fraction = (double)absoluteStepValue / 100;
                            double height = (fraction * ((double)trackBar.ScaleFieldMaxHeight - 2)) + 2;

                            heightList.Add(i, (int)height);
                        }
                    }
                    else
                    {
                        stepWidth = ((double)trackBar.Minimum - (double)trackBar.Maximum) / ((double)fieldCount - 1);

                        for (int i = 0; i < fieldCount; i++)
                        {
                            double stepValue = (double)trackBar.Minimum - i * stepWidth;
                            int absoluteStepValue = (int)Math.Abs(stepValue);

                            double fraction;

                            if (trackBar.ScaleFieldEqualizeHeights)
                            {
                                if (stepValue < 0)
                                    fraction = (double)absoluteStepValue / (double)Math.Abs(trackBar.Maximum);
                                else
                                    fraction = (double)absoluteStepValue / (double)Math.Abs(trackBar.Minimum);
                            }
                            else
                            {
                                fraction = (double)absoluteStepValue / (double)Math.Max(Math.Abs(trackBar.Maximum), Math.Abs(trackBar.Minimum));
                            }

                            double height = (fraction * ((double)trackBar.ScaleFieldMaxHeight - 2)) + 2;

                            heightList.Add(i, (int)height);
                        }
                    }
                }
            }

            return heightList;
        }

        #endregion Scale Fields

        #region Ticks

        public int CalculateTickCount(WinampTrackBar trackBar)
        {
            int width = trackBar.Orientation == Orientation.Horizontal ? trackBar.Width : trackBar.Height;

            int tickCount = (int)Math.Floor(((double)width + (double)trackBar.TickSpacing) / ((double)trackBar.TickWidth + (double)trackBar.TickSpacing));

            if (trackBar.TickEmphasizeMinMaxAndZero && tickCount % 2 != 0)
            {
                tickCount -= 1;
            }

            return tickCount;
        }

        public int CalculateTotalTickWidth(WinampTrackBar trackBar, int tickCount)
        {
            return (tickCount * trackBar.TickWidth) + ((tickCount - 1) * trackBar.TickSpacing);
        }

        #endregion Ticks

        #region Slider

        public void RenderSlider(Graphics g, int transparency, bool isHovered)
        {
            //Draw double gradient slider center
            int gradientWidth = (int)g.ClipBounds.Width - 4;
            int gradientTotalHeight = (int)g.ClipBounds.Height - 4;

            int gradientLowerHeight = (int)Math.Round((double)gradientTotalHeight / 2, 0);
            int gradientUpperHeight = gradientTotalHeight - gradientLowerHeight;

            if (gradientWidth > 0 && gradientLowerHeight > 0)
            {
                Color lowerGradientColor1 = isHovered ? Color.FromArgb(transparency, 174, 174, 174) : Color.FromArgb(transparency, 169, 169, 169);
                Color lowerGradientColor2 = isHovered ? Color.FromArgb(transparency, 210, 210, 210) : Color.FromArgb(transparency, 186, 186, 186);

                Rectangle lowerGradientRectangle = new Rectangle((int)g.ClipBounds.X + 2, (int)g.ClipBounds.Y + gradientUpperHeight + 2, gradientWidth, gradientLowerHeight);
                RectangleF lowerBrushRectangle = new RectangleF(lowerGradientRectangle.Location, lowerGradientRectangle.Size);
                lowerBrushRectangle.Inflate(0, 1);

                using (Brush lowerGradientBrush = new LinearGradientBrush(lowerBrushRectangle, lowerGradientColor1, lowerGradientColor2, LinearGradientMode.Vertical))
                {
                    g.FillRectangle(lowerGradientBrush, lowerGradientRectangle);
                }
            }

            if (gradientWidth > 0 && gradientUpperHeight > 0)
            {
                Color upperGradientColor1 = isHovered ? Color.FromArgb(transparency, 249, 249, 249) : Color.FromArgb(transparency, 248, 248, 248);
                Color upperGradientColor2 = isHovered ? Color.FromArgb(transparency, 215, 215, 215) : Color.FromArgb(transparency, 200, 200, 200);

                RectangleF upperGradientRectangle = new RectangleF(g.ClipBounds.X + 2, g.ClipBounds.Y + 2, gradientWidth, gradientUpperHeight);
                RectangleF upperBrushRectangle = new RectangleF(upperGradientRectangle.Location, upperGradientRectangle.Size);
                upperBrushRectangle.Inflate(0, 1);

                using (Brush upperGradientBrush = new LinearGradientBrush(upperBrushRectangle, upperGradientColor1, upperGradientColor2, LinearGradientMode.Vertical))
                {
                    g.FillRectangle(upperGradientBrush, upperGradientRectangle);
                }
            }

            //Render shadow:
            int shadowTansparencyValue = transparency == 255 ? 30 : 15;
            Color shadowColor = Color.FromArgb(shadowTansparencyValue, Color.Black);

            using (Pen shadowPen = new Pen(shadowColor))
            {
                g.DrawLine(shadowPen, g.ClipBounds.X, g.ClipBounds.Y + 1, g.ClipBounds.X + g.ClipBounds.Width - 1, g.ClipBounds.Y + 1);
                g.DrawLine(shadowPen, g.ClipBounds.X + 1, g.ClipBounds.Y, g.ClipBounds.X + g.ClipBounds.Width - 2, g.ClipBounds.Y);

                g.DrawLine(shadowPen, g.ClipBounds.X, g.ClipBounds.Y + 2, g.ClipBounds.X, g.ClipBounds.Y + g.ClipBounds.Height - 3);
                g.DrawLine(shadowPen, g.ClipBounds.X + g.ClipBounds.Width - 1, g.ClipBounds.Y + 2, g.ClipBounds.X + g.ClipBounds.Width - 1, g.ClipBounds.Y + g.ClipBounds.Height - 3);

                g.DrawLine(shadowPen, g.ClipBounds.X, g.ClipBounds.Y + g.ClipBounds.Height - 2, g.ClipBounds.X + g.ClipBounds.Width - 1, g.ClipBounds.Y + g.ClipBounds.Height - 2);
                g.DrawLine(shadowPen, g.ClipBounds.X + 1, g.ClipBounds.Y + g.ClipBounds.Height - 1, g.ClipBounds.X + g.ClipBounds.Width - 2, g.ClipBounds.Y + g.ClipBounds.Height - 1);
            }

            //Draw slider border
            Color borderColor = isHovered ? Color.White : Color.FromArgb(transparency, 210, 210, 210);

            using (Pen borderPen = new Pen(borderColor))
            {
                g.DrawLine(borderPen, g.ClipBounds.X + 2, g.ClipBounds.Y + 1, g.ClipBounds.X + g.ClipBounds.Width - 3, g.ClipBounds.Y + 1);
                g.DrawLine(borderPen, g.ClipBounds.X + 1, g.ClipBounds.Y + 2, g.ClipBounds.X + 1, g.ClipBounds.Y + g.ClipBounds.Height - 3);
                g.DrawLine(borderPen, g.ClipBounds.X + g.ClipBounds.Width - 2, g.ClipBounds.Y + 2, g.ClipBounds.X + g.ClipBounds.Width - 2, g.ClipBounds.Y + g.ClipBounds.Height - 3);
                g.DrawLine(borderPen, g.ClipBounds.X + 2, g.ClipBounds.Y + g.ClipBounds.Height - 2, g.ClipBounds.X + g.ClipBounds.Width - 3, g.ClipBounds.Y + g.ClipBounds.Height - 2);
            }

            Color innerBorderColor = isHovered ? Color.FromArgb(transparency, 250, 250, 250) : Color.FromArgb(transparency, 208, 208, 208);

            using (Pen borderPen = new Pen(innerBorderColor))
            {
                g.DrawLine(borderPen, g.ClipBounds.X + 2, g.ClipBounds.Y + g.ClipBounds.Height - 3, g.ClipBounds.X + g.ClipBounds.Width - 3, g.ClipBounds.Y + g.ClipBounds.Height - 3);
            }
        }

        #endregion Slider
    }
}
