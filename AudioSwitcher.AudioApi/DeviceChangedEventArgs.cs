using System;

namespace AudioSwitcher.AudioApi
{
    public abstract class DeviceChangedEventArgs : EventArgs
    {
        protected DeviceChangedEventArgs(IDevice dev, AudioDeviceEventType type)
        {
            Device = dev;
            EventType = type;
        }

        public IDevice Device { get; private set; }

        public AudioDeviceEventType EventType { get; private set; }
    }
}