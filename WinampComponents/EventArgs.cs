using System;

namespace Winamp.Components
{
    public class WinampTrackBarSeekEventArgs : EventArgs
    {
        public int Value { get; private set; }

        public WinampTrackBarSeekEventArgs(int value)
        {
            Value = value;
        }
    }

    public class WinampTrackBarValueChangedEventArgs : EventArgs
    {
        public int Value { get; private set; }
        public WinampTrackBar.WinampTrackBarValueChangeSource ChangeSource { get; private set; }

        public WinampTrackBarValueChangedEventArgs(int value, WinampTrackBar.WinampTrackBarValueChangeSource changeSource)
        {
            Value = value;
            ChangeSource = changeSource;
        }
    }

    public class WinampTrackBarValueChangingEventArgs : EventArgs
    {
        public int Value { get; private set; }
        public WinampTrackBar.WinampTrackBarValueChangeSource ChangeSource { get; private set; }
        public bool Cancel { get; set; }

        public WinampTrackBarValueChangingEventArgs(int value, WinampTrackBar.WinampTrackBarValueChangeSource changeSource)
        {
            Value = value;
            ChangeSource = changeSource;
            Cancel = false;
        }
    }
}
