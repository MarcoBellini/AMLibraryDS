using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

/**********************************************************************************/
/*                         WinampTrackBar - Version  1.3                          */
/**********************************************************************************/
/* http://www.codeproject.com/Articles/997101/Custom-Winamp-Style-TrackBar-Slider */
/**********************************************************************************/

namespace Winamp.Components
{
    [DefaultValue("Value"), DefaultEvent("ValueChanged"), ToolboxBitmap(typeof(TrackBar))]
    public class WinampTrackBar : Control
    {
        private const int AcceptableSpaceBetweenScaleAndTrackBar = 3;
        private ToolTip _toolTip;

        #region Constructor

        public WinampTrackBar()
        {
            SetStyle(ControlStyles.ResizeRedraw |
                        ControlStyles.SupportsTransparentBackColor |
                        ControlStyles.AllPaintingInWmPaint |
                        ControlStyles.UserPaint |
                        ControlStyles.OptimizedDoubleBuffer |
                        ControlStyles.DoubleBuffer, true);

            BackColor = Color.FromArgb(56, 63, 67);

            _renderer = new HorizontalWinampTrackBarRenderer(this);
            _toolTip = new ToolTip();
        }

        #endregion Constructor

        #region Enums

        public enum WinampTrackBarShowSlider
        {
            Never,
            OnHover,
            Always
        }

        public enum WinampTrackBarTrackStyle
        {
            None,
            FromZero,
            FromLeftOrTop,
            FromRightOrBottom
        }

        public enum WinampTrackBarScaleFieldPosition
        {
            LeftOrTop,
            RightOrBottom
        }

        public enum WinampTrackBarTickPosition
        {
            LeftOrTop,
            RightOrBottom,
            Both
        }

        public enum WinampTrackBarTickAlignment
        {
            Near,
            Center,
            Far
        }

        public enum WinampTrackBarKeyChangeOption
        {
            NoKeyChange,
            LeftAndRightArrowKeys,
            UpAndDownArrowKeys
        }

        public enum WinampTrackBarValueChangeSource
        {
            Code,
            SliderChange,
            TrackClick,
            MouseWheel,
            KeyPress
        }

        public enum WinampTrackBarScaleType
        {
            None,
            ScaleFields,
            Ticks
        }

        #endregion Enums

        #region Events & Delegates

        public delegate void ValueChangingDelegate(Object sender, WinampTrackBarValueChangingEventArgs e);
        [Description("Raised before the TrackBar Value property changes")]
        public event ValueChangingDelegate ValueChanging;

        public delegate void ValueChangedDelegate(Object sender, WinampTrackBarValueChangedEventArgs e);
        [Description("Raised when the TrackBar Value property has changed")]
        public event ValueChangedDelegate ValueChanged;

        public delegate void ScrollDelegate(Object sender, EventArgs e);
        [Description("Raised when the TrackBar has scrolled.")]
        public event ScrollDelegate Scroll;

        public delegate void SeekingDelegate(Object sender, WinampTrackBarSeekEventArgs e);
        [Description("Raised when the TrackBar is seeking.")]
        public event SeekingDelegate Seeking;

        public delegate void SeekDoneDelegate(Object sender, WinampTrackBarSeekEventArgs e);
        [Description("Raised when the TrackBar seek is done.")]
        public event SeekDoneDelegate SeekDone;

        public delegate void MaximumChangedDelegate(Object sender, EventArgs e);
        [Description("Raised when the TrackBar Maximum changes.")]
        public event MaximumChangedDelegate MaximumChanged;

        public delegate void MinimumChangedDelegate(Object sender, EventArgs e);
        [Description("Raised when the TrackBar Minimum changes.")]
        public event MinimumChangedDelegate MinimumChanged;

        public delegate void SliderButtonDoubleClickDelegate(Object sender, MouseEventArgs e);
        [Description("Raised when the Slider Button of the TrackBar is double-clicked.")]
        public event SliderButtonDoubleClickDelegate SliderButtonDoubleClick;

        #endregion Events & Delegates

        #region Private Members

        private WinampTrackBarRenderer _renderer;

        private int _value;
        private int _minimum;
        private int _maximum = 100;

        private bool _isSeeking;
        private int _seekValue;
        private bool _isControlHovered;
        private bool _isSliderHovered;
        private int _mousePointerXOffset;
        private int _mousePointerYOffset;

        private bool _autoSize;
        private Orientation _orientation = Orientation.Horizontal;

        private Size _sliderButtonSize = new Size(38, 10);
        private bool _useSeeking = true;
        private bool _allowUserSlideChange = true;
        private WinampTrackBarShowSlider _showSlider = WinampTrackBarShowSlider.Always;
        private int _seekSliderTransparency = 200;
        private bool _useHoverEffect = true;

        private WinampTrackBarKeyChangeOption _keyChangeOption = WinampTrackBarKeyChangeOption.UpAndDownArrowKeys;
        private int _smallChange = 3;
        private int _largeChange = 10;
        private bool _allowMouseWheelChange = true;

        private WinampTrackBarScaleType _scaleType = WinampTrackBarScaleType.ScaleFields;

        private WinampTrackBarTickPosition _tickPosition = WinampTrackBarTickPosition.Both;
        private int _tickSpacing = 1;
        private int _tickWidth = 1;
        private int _tickHeight = 1;
        private int _tickEmphasizedHeight = 3;
        private WinampTrackBarTickAlignment _tickAlignment = WinampTrackBarTickAlignment.Center;
        private Color _tickColor = Color.FromArgb(72, 76, 79);
        private Color _tickEmphasizedColor = Color.FromArgb(72, 76, 79);
        private bool _tickEmphasizeMinMaxAndZero = true;

        private WinampTrackBarScaleFieldPosition _scaleFieldPosition = WinampTrackBarScaleFieldPosition.LeftOrTop;
        private int _scaleFieldWidth = 3;
        private int _scaleFieldMaxHeight = 10;
        private int _scaleFieldSpacing = 1;
        private Color _scaleFieldColor = Color.FromArgb(72, 76, 79);
        private bool _scaleFieldEqualizeHeights;

        private WinampTrackBarTrackStyle _trackStyle = WinampTrackBarTrackStyle.FromZero;
        private Color _trackUpperColor = Color.FromArgb(156, 169, 173);
        private Color _trackLowerColor = Color.FromArgb(88, 107, 113);
        private Color _emptyTrackColor = Color.Black;

        private string _toolTipText ="";
        private string _sliderToolTipText = "";

        #endregion Private Members

        #region Internal Properties

        [Browsable(false)]
        internal bool IsSeeking
        {
            get { return _isSeeking; }
            private set
            {
                _isSeeking = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        internal int SeekValue
        {
            get { return _seekValue; }
            private set
            {
                _seekValue = value;
                Invalidate();

                if (Seeking != null)
                {
                    WinampTrackBarSeekEventArgs eargs = new WinampTrackBarSeekEventArgs(_seekValue);
                    Seeking(this, eargs);
                }
            }
        }

        [Browsable(false)]
        internal bool IsSliderVisible
        {
            get
            {
                switch (_showSlider)
                {
                    case WinampTrackBarShowSlider.Always:
                        return true;
                        
                    case WinampTrackBarShowSlider.Never:
                        return false;
                        
                    case WinampTrackBarShowSlider.OnHover:
                        return _isControlHovered;
                        
                }

                return true;
            }
            set
            {
                if (_showSlider != WinampTrackBarShowSlider.Never)
                {
                    Invalidate();
                }
            }
        }
        
        [Browsable(false)]
        internal bool IsSliderHovered
        {
            get { return _isSliderHovered; }
            set
            {
                if (value != _isSliderHovered)
                {
                    _isSliderHovered = value;
                    Invalidate();
                }
            }
        }

        #endregion Internal Properties

        #region Public Properties

        [Bindable(true)]
        [DefaultValue(0)]
        [Category("Data")]
        [Description("Gets or sets the value of the TrackBar")]
        public int Value
        {
            get { return _value; }
            set
            {
                if (value != _value)
                {
                    if (value < Minimum)
                        value = Minimum;

                    if (value > Maximum)
                        value = Maximum;

                    if (ValueChanging != null)
                    {
                        WinampTrackBarValueChangingEventArgs args = new WinampTrackBarValueChangingEventArgs(value, WinampTrackBarValueChangeSource.Code);
                        ValueChanging(this, args);

                        if (args.Cancel)
                            return;
                    }

                    _value = value;
                    Invalidate();

                    if (ValueChanged != null)
                        ValueChanged(this, new WinampTrackBarValueChangedEventArgs(_value, WinampTrackBarValueChangeSource.Code));
                }
            }
        }

        [Bindable(false)]
        [DefaultValue(0)]
        [Category("Data")]
        [Description("Gets or sets the Minimum value of the TrackBar")]
        public int Minimum
        {
            get { return _minimum; }
            set
            {
                if (value != _minimum)
                {
                    _minimum = value;
                    Invalidate();

                    if (MinimumChanged != null)
                        MinimumChanged(this, new EventArgs());
                }
            }
        }

        [Bindable(false)]
        [DefaultValue(100)]
        [Category("Data")]
        [Description("Gets or sets the Maximum value of the TrackBar")]
        public int Maximum
        {
            get { return _maximum; }
            set
            {
                if (value != _maximum)
                {
                    _maximum = value;
                    Invalidate();

                    if (MaximumChanged != null)
                        MaximumChanged(this, new EventArgs());
                }
            }
        }

        [Bindable(false)]
        [DefaultValue(typeof(Size), "38, 10")]
        [Category("Appearance")]
        [Description("Gets or sets the size of the Slider Button")]
        public Size SliderButtonSize
        {
            get { return _sliderButtonSize; }
            set
            {
                if (value != _sliderButtonSize)
                {
                    _sliderButtonSize = value;
                    OnResize(null);
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Bindable(false)]
        [DefaultValue(false)]
        [Category("Behavior")]
        [Description("Gets or sets whether the control should autosize to fit a whole number of scale fields.")]
        public new bool AutoSize
        {
            get { return _autoSize; }
            set
            {
                if (value != _autoSize)
                {
                    _autoSize = value;
                    OnResize(null);
                    Invalidate();
                }
            }
        }

        [Bindable(false)]
        [DefaultValue(true)]
        [Category("Behavior")]
        [Description("If false, clicking the Track will change the calue to the selected value immediately. If true, the value will not be changed before the mouse capture is released.")]
        public bool UseSeeking
        {
            get { return _useSeeking; }
            set
            {
                if (value != _useSeeking)
                {
                    _useSeeking = value;
                    Invalidate();
                }
            }
        }

        [Bindable(false)]
        [DefaultValue(200)]
        [Category("Appearance")]
        [Description("Gets or sets how tranparent the seek slider should be. Only used if UseSeeking = true")]
        public int SeekSliderTransparency
        {
            get { return _seekSliderTransparency; }
            set { _seekSliderTransparency = value; }
        }

        [Bindable(false)]
        [DefaultValue(true)]
        [Category("Behavior")]
        [Description("If false, the user cannot change the value by dragging the Slider Button or clicking the Track. This can be useful if you just want to use the TrackBar to show some kind of progress.")]
        public bool AllowUserValueChange
        {
            get { return _allowUserSlideChange; }
            set
            {
                if (value != _allowUserSlideChange)
                {
                    _allowUserSlideChange = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Bindable(false)]
        [DefaultValue(true)]
        [Category("Behavior")]
        [Description("Gets or sets whether the slider should light up when the mouse hovers over it.")]
        public bool UseHoverEffect
        {
            get { return _useHoverEffect; }
            set
            {
                if (value != _useHoverEffect)
                {
                    _useHoverEffect = value;
                    OnResize(null);
                    Invalidate();
                }
            }
        }

        [Bindable(false)]
        [DefaultValue(typeof(Color), "156, 169, 173")]
        [Category("Appearance")]
        [Description("Gets or sets the upper color of the filled part of the track")]
        public Color TrackUpperColor
        {
            get { return _trackUpperColor; }
            set
            {
                if (value != _trackUpperColor)
                {
                    _trackUpperColor = value;
                    Invalidate();
                }
            }
        }

        [Bindable(false)]
        [DefaultValue(typeof(Color), "88, 107, 113")]
        [Category("Appearance")]
        [Description("Gets or sets the lower color of the filled part of the track")]
        public Color TrackLowerColor
        {
            get { return _trackLowerColor; }
            set
            {
                if (value != _trackLowerColor)
                {
                    _trackLowerColor = value;
                    Invalidate();
                }
            }
        }

        [Bindable(false)]
        [DefaultValue(typeof(Color), "72, 76, 79")]
        [Category("Appearance")]
        [Description("Gets or sets the color of the empty part of the track")]
        public Color EmptyTrackColor
        {
            get { return _emptyTrackColor; }
            set
            {
                if (value != _emptyTrackColor)
                {
                    _emptyTrackColor = value;
                    Invalidate();
                }
            }
        }
        
        [Bindable(false)]
        [DefaultValue(typeof(Orientation), "Horizontal")]
        [Category("Appearance")]
        [Description("Gets or sets the TrackBar orientation, Horizontal or Vertical")]
        public Orientation Orientation
        {
            get { return _orientation; }
            set
            {
                if (value != _orientation)
                {
                    _orientation = value;

                    switch (_orientation)
                    {
                        case Orientation.Horizontal:
                            _renderer = new HorizontalWinampTrackBarRenderer(this);
                            break;
                        case Orientation.Vertical:
                            _renderer = new VerticalWinampTrackBarRenderer(this);
                            break;
                    }    
                }

                Invalidate();
            }
        }

        [Bindable(false)]
        [DefaultValue(typeof(WinampTrackBarTrackStyle), "FromZero")]
        [Category("Behavior")]
        [Description("Gets or sets how the TrackBar track should be filled")]
        public WinampTrackBarTrackStyle TrackStyle
        {
            get { return _trackStyle; }
            set
            {
                if (value != _trackStyle)
                {
                    _trackStyle = value;
                    Invalidate();
                }
            }
        }

        [Bindable(false)]
        [DefaultValue(typeof(WinampTrackBarScaleType), "ScaleFields")]
        [Category("Appearance")]
        [Description("Gets or sets the TrackBar scale type")]
        public WinampTrackBarScaleType ScaleType
        {
            get { return _scaleType; }
            set
            {
                if (value != _scaleType)
                {
                    _scaleType = value;
                    OnResize(null);
                    Invalidate();
                }
            }
        }

        [Bindable(false)]
        [DefaultValue(typeof(WinampTrackBarTickPosition), "Both")]
        [Category("Appearance")]
        [Description("Gets or sets the position of the TrackBar ticks")]
        public WinampTrackBarTickPosition TickPosition
        {
            get { return _tickPosition; }
            set
            {
                if (value != _tickPosition)
                {
                    _tickPosition = value;
                    OnResize(null);
                    Invalidate();
                }
            }
        }

        [Bindable(false)]
        [DefaultValue(1)]
        [Category("Appearance")]
        [Description("Gets or sets the spacing in pixels between the TrackBar ticks")]
        public int TickSpacing
        {
            get { return _tickSpacing; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("TickSpacing cannot be negative!");

                if (value != _tickSpacing)
                {
                    _tickSpacing = value;
                    Invalidate();
                }
            }
        }
        
        [Bindable(false)]
        [DefaultValue(1)]
        [Category("Appearance")]
        [Description("Gets or sets the width in pixels of the normal TrackBar ticks (TickHeight for vertical orientation)")]
        public int TickWidth
        {
            get { return _tickWidth; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("TickWidth cannot be zero or negative!");

                if (value != _tickWidth)
                {
                    _tickWidth = value;
                    OnResize(null);
                    Invalidate();
                }
            }
        }

        [Bindable(false)]
        [DefaultValue(1)]
        [Category("Appearance")]
        [Description("Gets or sets the height in pixels of the normal TrackBar ticks (TickWidth for vertical orientation)")]
        public int TickHeight
        {
            get { return _tickHeight; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("TickHeight cannot be zero or negative!");

                if (value != _tickHeight)
                {
                    _tickHeight = value;
                    OnResize(null);
                    Invalidate();
                }
            }
        }

        [Bindable(false)]
        [DefaultValue(3)]
        [Category("Appearance")]
        [Description("Gets or sets the height in pixels of the emphasized TrackBar ticks (TickEmphasizedWidth for vertical orientation)")]
        public int TickEmphasizedHeight
        {
            get { return _tickEmphasizedHeight; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("TickEmphasizedHeight cannot be zero or negative!");

                if (value != _tickEmphasizedHeight)
                {
                    _tickEmphasizedHeight = value;
                    OnResize(null);
                    Invalidate();
                }
            }
        }


        [Bindable(false)]
        [DefaultValue(typeof(WinampTrackBarTickAlignment), "Center")]
        [Category("Appearance")]
        [Description("Gets or sets the alignment of the TrackBar ticks")]
        public WinampTrackBarTickAlignment TickAlignment
        {
            get { return _tickAlignment; }
            set
            {
                if (value != _tickAlignment)
                {
                    _tickAlignment = value;
                    Invalidate();
                }
            }
        }

        [Bindable(false)]
        [DefaultValue(typeof(Color), "72, 76, 79")]
        [Category("Appearance")]
        [Description("Gets or sets the color of the ticks")]
        public Color TickColor
        {
            get { return _tickColor; }
            set
            {
                if (value != _tickColor)
                {
                    _tickColor = value;
                    Invalidate();
                }
            }
        }

        [Bindable(false)]
        [DefaultValue(typeof(Color), "72, 76, 79")]
        [Category("Appearance")]
        [Description("Gets or sets the color of the emphasized ticks")]
        public Color TickEmphasizedColor
        {
            get { return _tickEmphasizedColor; }
            set
            {
                if (value != _tickEmphasizedColor)
                {
                    _tickEmphasizedColor = value;
                    Invalidate();
                }
            }
        }

        [Bindable(false)]
        [DefaultValue(true)]
        [Category("Appearance")]
        [Description("Gets or sets whether the Minimum value, the Maximum value and Zero should be emphasized with a larger ticks")]
        public bool TickEmphasizeMinMaxAndZero
        {
            get { return _tickEmphasizeMinMaxAndZero; }
            set
            {
                if (value != _tickEmphasizeMinMaxAndZero)
                {
                    _tickEmphasizeMinMaxAndZero = value;
                    OnResize(null);
                    Invalidate();
                }
            }
        }

        [Bindable(false)]
        [DefaultValue(typeof(WinampTrackBarScaleFieldPosition), "LeftOrTop")]
        [Category("Appearance")]
        [Description("Gets or sets the position of the TrackBar scale fields")]
        public WinampTrackBarScaleFieldPosition ScaleFieldPosition
        {
            get { return _scaleFieldPosition; }
            set
            {
                if (value != _scaleFieldPosition)
                {
                    _scaleFieldPosition = value;
                    OnResize(null);
                    Invalidate();    
                }
            }
        }

        [Bindable(false)]
        [DefaultValue(typeof(WinampTrackBarShowSlider), "Always")]
        [Category("Behavior")]
        [Description("Gets or sets When the slider should be shown")]
        public WinampTrackBarShowSlider ShowSlider
        {
            get { return _showSlider; }
            set
            {
                if (value != _showSlider)
                {
                    _showSlider = value;
                    OnResize(null);
                    Invalidate();
                }
            }
        }

        [Bindable(false)]
        [DefaultValue(typeof(Color), "72, 76, 79")]
        [Category("Appearance")]
        [Description("Gets or sets the color of the scale fields")]
        public Color ScaleFieldColor
        {
            get { return _scaleFieldColor; }
            set
            {
                if (value != _scaleFieldColor)
                {
                    _scaleFieldColor = value;
                    Invalidate();    
                }
            }
        }

        [Bindable(true)]
        [DefaultValue(3)]
        [Category("Appearance")]
        [Description("Gets or sets the width of the scale fields in Horizontal mode (will be field HEIGHT in Vertical mode)")]
        public int ScaleFieldWidth
        {
            get { return _scaleFieldWidth; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("ScaleFieldWidth cannot be zero or negative!");

                if (value != _scaleFieldWidth)
                {
                    _scaleFieldWidth = value;
                    OnResize(null);
                    Invalidate();    
                }
            }
        }

        [Bindable(true)]
        [DefaultValue(10)]
        [Category("Appearance")]
        [Description("Gets or sets the maximum height of the scale fields in Horizontal mode (will be maximum field WIDTH in Vertical mode)")]
        public int ScaleFieldMaxHeight
        {
            get { return _scaleFieldMaxHeight; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("ScaleFieldMaxHeight cannot be zero or negative!");

                if (value != _scaleFieldMaxHeight)
                {
                    _scaleFieldMaxHeight = value;
                    OnResize(null);
                    Invalidate();
                }
            }
        }

        [Bindable(true)]
        [DefaultValue(false)]
        [Category("Behavior")]
        [Description("Gets or sets if both ends or a dual scale should have the same height even if they have different values")]
        public bool ScaleFieldEqualizeHeights
        {
            get { return _scaleFieldEqualizeHeights; }
            set
            {
                if (value != _scaleFieldEqualizeHeights)
                {
                    _scaleFieldEqualizeHeights = value;
                    OnResize(null);
                    Invalidate();
                }
            }
        }

        [Bindable(true)]
        [DefaultValue(1)]
        [Category("Appearance")]
        [Description("Gets or sets the spacing in pixels between the scale fields")]
        public int ScaleFieldSpacing
        {
            get { return _scaleFieldSpacing; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("ScaleFieldSpacing cannot be negative!");

                if (value != _scaleFieldSpacing)
                {
                    _scaleFieldSpacing = value;
                    OnResize(null);
                    Invalidate();
                }
            }
        }

        [Bindable(false)]
        [DefaultValue(typeof(WinampTrackBarKeyChangeOption), "UpAndDownArrowKeys")]
        [Category("Behavior")]
        [Description("Gets or sets if the user should be able to change the trackbar value using the arrow keys")]
        public WinampTrackBarKeyChangeOption KeyChangeOption
        {
            get { return _keyChangeOption; }
            set { _keyChangeOption = value; }
        }

        [Bindable(false)]
        [DefaultValue(3)]
        [Category("Behavior")]
        [Description("Gets or sets the small value change")]
        public int SmallChange
        {
            get { return _smallChange; }
            set { _smallChange = value; }
        }

        [Bindable(false)]
        [DefaultValue(10)]
        [Category("Behavior")]
        [Description("Gets or sets the large value change")]
        public int LargeChange
        {
            get { return _largeChange; }
            set { _largeChange = value; }
        }

        [Bindable(false)]
        [DefaultValue(true)]
        [Category("Behavior")]
        [Description("Gets or sets if the user should be able to change the trackbar value using the mouse wheel")]
        public bool AllowMouseWheelChange
        {
            get { return _allowMouseWheelChange; }
            set { _allowMouseWheelChange = value; }
        }

        [Bindable(true)]
        [DefaultValue("")]
        [Category("Behavior")]
        [Description("Gets or sets the tooltip text that is shown when the mouse hovers over the control")]
        public string ToolTipText
        {
            get { return _toolTipText; }
            set { _toolTipText = value; }
        }

        [Bindable(true)]
        [DefaultValue("")]
        [Category("Behavior")]
        [Description("Gets or sets the tooltip text that is shown when the mouse hovers over the trackbar slider button")]
        public string ToolTipTextSliderButton
        {
            get { return _sliderToolTipText; }
            set { _sliderToolTipText = value; }
        }

        [Bindable(true)]
        [DefaultValue(true)]
        [Category("Behavior")]
        [Description("Determines if the ToolTip is active. A tip will only appear if the ToolTip has been activated.")]
        public bool ToolTipActive
        {
            get { return _toolTip.Active; }
            set { _toolTip.Active = value; }
        }

        [Bindable(true)]
        [DefaultValue(500)]
        [Category("Behavior")]
        [Description("Sets the values of AutoPopDelay, InitialDelay, and ReshowDelay to the appropriate values.")]
        public int ToolTipAutomaticDelay
        {
            get { return _toolTip.AutomaticDelay; }
            set { _toolTip.AutomaticDelay = value; }
        }

        [Bindable(true)]
        [DefaultValue(5000)]
        [Category("Behavior")]
        [Description("Determines the length of time the ToolTip window remains visible if the pointer is stationary inside a ToolTip region.")]
        public int ToolTipAutoPopDelay
        {
            get { return _toolTip.AutoPopDelay; }
            set { _toolTip.AutoPopDelay = value; }
        }

        [Bindable(true)]
        [DefaultValue(typeof(Color), "Info")]
        [Category("Behavior")]
        [Description("The background color of the ToolTip.")]
        public Color ToolTipBackColor
        {
            get { return _toolTip.BackColor; }
            set { _toolTip.BackColor = value; }
        }

        [Bindable(true)]
        [DefaultValue(typeof(Color), "InfoText")]
        [Category("Behavior")]
        [Description("The foreground color of the ToolTip.")]
        public Color ToolTipForeColor
        {
            get { return _toolTip.ForeColor; }
            set { _toolTip.ForeColor = value; }
        }

        [Bindable(true)]
        [DefaultValue(500)]
        [Category("Behavior")]
        [Description("Determines the length of time the pointer must remain stationary within a ToolTip region before the ToolTip window appears.")]
        public int ToolTipInitialDelay
        {
            get { return _toolTip.InitialDelay; }
            set { _toolTip.InitialDelay = value; }
        }

        [Bindable(true)]
        [DefaultValue(false)]
        [Category("Behavior")]
        [Description("Indicates whether the ToolTip will take on a balloon form.")]
        public bool ToolTipIsBalloon
        {
            get { return _toolTip.IsBalloon; }
            set { _toolTip.IsBalloon = value; }
        }

        [Bindable(true)]
        [DefaultValue(100)]
        [Category("Behavior")]
        [Description("Determines the length of time it takes for subsequent windows to appear as the pointer moves from one ToolTip region to another.")]
        public int ToolTipReshowDelay
        {
            get { return _toolTip.ReshowDelay; }
            set { _toolTip.ReshowDelay = value; }
        }

        [Bindable(true)]
        [DefaultValue(false)]
        [Category("Behavior")]
        [Description("Determines if the tool tip will be displayed always, even if the parent window is not active.")]
        public bool ToolTipStripAmpersands
        {
            get { return _toolTip.StripAmpersands; }
            set { _toolTip.StripAmpersands = value; }
        }

        [Bindable(true)]
        [DefaultValue(false)]
        [Category("Behavior")]
        [Description("When set to true, any ampersands in the Text property are not displayed.")]
        public bool ToolTipShowAlways
        {
            get { return _toolTip.ShowAlways; }
            set { _toolTip.ShowAlways = value; }
        }

        [Bindable(true)]
        [DefaultValue(typeof(ToolTipIcon), "None")]
        [Category("Behavior")]
        [Description("Determines the icon that is shown on the ToolTip.")]
        public ToolTipIcon ToolTipIcon
        {
            get { return _toolTip.ToolTipIcon; }
            set { _toolTip.ToolTipIcon = value; }
        }

        [Bindable(true)]
        [DefaultValue("")]
        [Category("Behavior")]
        [Description("Determines the title of the ToolTip.")]
        public string ToolTipTitle
        {
            get { return _toolTip.ToolTipTitle; }
            set { _toolTip.ToolTipTitle = value; }
        }

        [Bindable(true)]
        [DefaultValue(true)]
        [Category("Behavior")]
        [Description("When set to true, animations are used when the ToolTip is shown or hidden.")]
        public bool ToolTipUseAnimation
        {
            get { return _toolTip.UseAnimation; }
            set { _toolTip.UseAnimation = value; }
        }

        [Bindable(true)]
        [DefaultValue(true)]
        [Category("Behavior")]
        [Description("When set to true, a fade effect is used when the ToolTip is shown or hidden.")]
        public bool ToolTipUseFading
        {
            get { return _toolTip.UseFading; }
            set { _toolTip.UseFading = value; }
        }

        [Browsable(false)]
        public new string Text
        {
            get { return ""; }
            set { base.Text = ""; }
        }

        #endregion Public Properties

        #region Private Methods

        private void SetValueInternal(int value, bool raiseScroll, WinampTrackBarValueChangeSource source)
        {
            if (value > Maximum)
                value = Maximum;

            if (value < Minimum)
                value = Minimum;

            if (ValueChanging != null)
            {
                WinampTrackBarValueChangingEventArgs args = new WinampTrackBarValueChangingEventArgs(value, source);
                ValueChanging(this, args);

                if (args.Cancel)
                    return;
            }

            _value = value;

            Invalidate();

            if (raiseScroll && Scroll != null)
                Scroll(this, new EventArgs());

            if (ValueChanged != null)
                ValueChanged(this, new WinampTrackBarValueChangedEventArgs(_value, source));
        }

        #endregion Private Methods

        #region Overridden Base Class Methods

        protected override void OnPaint(PaintEventArgs e)
        {
            _renderer.RenderControl(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _isControlHovered = true;
            IsSliderVisible = true;

            if (_toolTip != null && _toolTipText != "")
                _toolTip.SetToolTip(this, _toolTipText);

            if (!AllowUserValueChange)
                return;

            base.OnMouseEnter(e);
        }

        protected override void OnMouseHover(EventArgs e)
        {
            _isControlHovered = true;

            if (!AllowUserValueChange)
                return;

            base.OnMouseHover(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isControlHovered = false;
            IsSliderVisible = false;

            if (_toolTip != null)
                _toolTip.SetToolTip(this, "");

            if (!AllowUserValueChange)
                return;

            base.OnMouseLeave(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            _isControlHovered = true;

            bool savedButtonHovered = IsSliderHovered;

            Rectangle sliderRectangle = _renderer.GetSliderLayoutRectangle(Value);
            IsSliderHovered = (sliderRectangle.Contains(e.Location));

            if (IsSliderHovered && savedButtonHovered != IsSliderHovered && _toolTip != null && _sliderToolTipText != "")
                _toolTip.SetToolTip(this, _sliderToolTipText);
            else if (!IsSliderHovered && savedButtonHovered != IsSliderHovered && _toolTip != null && _toolTipText != "")
                _toolTip.SetToolTip(this, _toolTipText);

            if (!AllowUserValueChange)
                return;

            if (IsSeeking)
            {
                int adjustedCoordinate = Orientation == Orientation.Horizontal ? e.X + _mousePointerXOffset : e.Y + _mousePointerYOffset;
                int currentValue = _renderer.PixelValueToValue(adjustedCoordinate);

                if (UseSeeking)
                    SeekValue = currentValue;
                else
                    SetValueInternal(currentValue, true, WinampTrackBarValueChangeSource.SliderChange);
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!AllowUserValueChange)
                return;

            if (e.Button != MouseButtons.Left)
                return;

            Rectangle sliderRectangle = _renderer.GetSliderLayoutRectangle(Value);
            Rectangle clickRectangle = _renderer.GetClickRectangle();

            if (sliderRectangle.Contains(e.Location))
            {
                IsSeeking = true;
                SeekValue = Value;

                int middleXValue = sliderRectangle.X + (sliderRectangle.Width / 2) - 1;
                _mousePointerXOffset = middleXValue - e.X;

                int middleYValue = sliderRectangle.Y + (sliderRectangle.Height / 2) - 1;
                _mousePointerYOffset = middleYValue - e.Y;
            }
            else if (clickRectangle.Contains(e.Location))
            {
                int currentValue = Orientation == Orientation.Horizontal ? _renderer.PixelValueToValue(e.X) : _renderer.PixelValueToValue(e.Y);
                SetValueInternal(currentValue, true, WinampTrackBarValueChangeSource.TrackClick);
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (!AllowUserValueChange)
                return;

            if (e.Button != MouseButtons.Left)
                return;

            base.OnMouseUp(e);

            if (UseSeeking && IsSeeking)
            {
                SetValueInternal(SeekValue, true, WinampTrackBarValueChangeSource.SliderChange);

                if (SeekDone != null)
                    SeekDone(this, new WinampTrackBarSeekEventArgs(SeekValue));
            }

            IsSeeking = false;
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (!AllowUserValueChange)
                return;

            Rectangle sliderRectangle = _renderer.GetSliderLayoutRectangle(Value);

            if (sliderRectangle.Contains(e.Location))
            {
                if (SliderButtonDoubleClick != null)
                    SliderButtonDoubleClick(this, e);
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (!AllowUserValueChange || !AllowMouseWheelChange)
                return;

            if (e.Delta > 0)
                SetValueInternal(Value + SmallChange, true, WinampTrackBarValueChangeSource.MouseWheel);

            if (e.Delta < 0)
                SetValueInternal(Value - SmallChange, true, WinampTrackBarValueChangeSource.MouseWheel);

            base.OnMouseWheel(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!AllowUserValueChange || _keyChangeOption == WinampTrackBarKeyChangeOption.NoKeyChange)
                return;

            if (_keyChangeOption == WinampTrackBarKeyChangeOption.LeftAndRightArrowKeys)
            {
                if (e.KeyCode == Keys.Left)
                {
                    SetValueInternal(Value - SmallChange, false, WinampTrackBarValueChangeSource.KeyPress);
                }
                else if (e.KeyCode == Keys.Right)
                {
                    SetValueInternal(Value + SmallChange, false, WinampTrackBarValueChangeSource.KeyPress);
                }
            }
            else
            {
                if (e.KeyCode == Keys.Up)
                {
                    SetValueInternal(Value + SmallChange, false, WinampTrackBarValueChangeSource.KeyPress);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    SetValueInternal(Value - SmallChange, false, WinampTrackBarValueChangeSource.KeyPress);
                }
            }

            if (e.KeyCode == Keys.PageUp)
            {
                SetValueInternal(Value + LargeChange, false, WinampTrackBarValueChangeSource.KeyPress);
            }
            else if (e.KeyCode == Keys.PageDown)
            {
                SetValueInternal(Value - LargeChange, false, WinampTrackBarValueChangeSource.KeyPress);
            }
            else if (e.KeyCode == Keys.Home)
            {
                SetValueInternal(Maximum, false, WinampTrackBarValueChangeSource.KeyPress);
            }
            else if (e.KeyCode == Keys.End)
            {
                SetValueInternal(Minimum, false, WinampTrackBarValueChangeSource.KeyPress);
            }

            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (!AllowUserValueChange || _keyChangeOption == WinampTrackBarKeyChangeOption.NoKeyChange)
                return;
            
            base.OnKeyUp(e);
        }

        protected override void OnResize(EventArgs e)
        {
            const int extraBorder = 1;

            if (AutoSize)
            {
                int fieldCount = _renderer.CalculateScaleFieldCount(this);
                int scaleWidth = _renderer.CalculateTotalScaleWidth(this, fieldCount);

                int tickFieldHeight = TickHeight;

                if (TickEmphasizeMinMaxAndZero && TickEmphasizedHeight > TickHeight)
                    tickFieldHeight = TickEmphasizedHeight;

                int tickCount = _renderer.CalculateTickCount(this);
                int tickScaleWidth = _renderer.CalculateTotalTickWidth(this, tickCount);

                if (Orientation == Orientation.Horizontal)
                {
                    switch (ScaleType)
                    {
                        case WinampTrackBarScaleType.None:
                            Height = SliderButtonSize.Height + (2*extraBorder);
                            break;
                        case WinampTrackBarScaleType.ScaleFields:
                            Height = ScaleFieldMaxHeight + AcceptableSpaceBetweenScaleAndTrackBar + SliderButtonSize.Height + (2*extraBorder);
                            Width = scaleWidth;
                            break;
                        case WinampTrackBarScaleType.Ticks:
                            Height = tickFieldHeight + AcceptableSpaceBetweenScaleAndTrackBar + SliderButtonSize.Height + (2 * extraBorder);

                            if (TickPosition == WinampTrackBarTickPosition.Both)
                                Height = Height + tickFieldHeight + AcceptableSpaceBetweenScaleAndTrackBar;

                            Width = tickScaleWidth;
                            break;
                    }
                }
                else
                {
                    switch (ScaleType)
                    {
                        case WinampTrackBarScaleType.None:
                            Width = SliderButtonSize.Width + (2 * extraBorder);
                            break;
                        case WinampTrackBarScaleType.ScaleFields:
                            Height = scaleWidth;
                            Width = ScaleFieldMaxHeight + AcceptableSpaceBetweenScaleAndTrackBar + SliderButtonSize.Width + (2 * extraBorder);
                            break;
                        case WinampTrackBarScaleType.Ticks:
                            Width = tickFieldHeight + AcceptableSpaceBetweenScaleAndTrackBar + SliderButtonSize.Width + (2 * extraBorder);

                            if (TickPosition == WinampTrackBarTickPosition.Both)
                                Width = Width + tickFieldHeight + AcceptableSpaceBetweenScaleAndTrackBar;

                            Height = tickScaleWidth;
                            break;
                    }
                }
            }

            base.OnResize(e);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Right:
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.PageUp:
                case Keys.PageDown:
                case Keys.Home:
                case Keys.End:
                    return true;
                //case Keys.Shift | Keys.Right:
                //case Keys.Shift | Keys.Left:
                //case Keys.Shift | Keys.Up:
                //case Keys.Shift | Keys.Down:
                //case Keys.Shift | Keys.PageUp:
                //case Keys.Shift | Keys.PageDown:
                //case Keys.Shift | Keys.Home:
                //case Keys.Shift | Keys.End:
                //    return true;
            }

            return base.IsInputKey(keyData);
        }

        #endregion Overridden Base Class Methods
    }
}
