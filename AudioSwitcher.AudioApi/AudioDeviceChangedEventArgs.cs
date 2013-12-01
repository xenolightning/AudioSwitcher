using System;
using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi
{
    /// <summary>
    ///     Event args passed back when an attribute on a device changes
    /// </summary>
    [ComVisible(false)]
    public class AudioDeviceChangedEventArgs : EventArgs
    {
        public AudioDeviceChangedEventArgs(AudioDevice dev, AudioDeviceEventType type)
        {
            Device = dev;
            EventType = type;
        }

        /// <summary>
        ///     Device that fired this event
        /// </summary>
        public AudioDevice Device { get; private set; }

        /// <summary>
        ///     The type of event
        /// </summary>
        public AudioDeviceEventType EventType { get; private set; }
    }
}