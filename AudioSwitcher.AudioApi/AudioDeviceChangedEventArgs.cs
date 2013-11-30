using System;

namespace AudioSwitcher.AudioApi
{
    /// <summary>
    ///     Event args passed back when an attribute on a device changes
    /// </summary>
    public class AudioDeviceChangedEventArgs : EventArgs
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="dev"></param>
        /// <param name="type"></param>
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
        ///     Get the change type
        /// </summary>
        public AudioDeviceEventType EventType { get; private set; }
    }
}