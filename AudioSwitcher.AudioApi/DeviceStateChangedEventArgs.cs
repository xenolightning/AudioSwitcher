namespace AudioSwitcher.AudioApi
{
    public sealed class DeviceStateChangedEventArgs : DeviceChangedEventArgs
    {
        public DeviceStateChangedEventArgs(IDevice dev, DeviceState state)
            : base(dev, AudioDeviceEventType.StateChanged)
        {
            State = state;
        }

        public DeviceState State { get; private set; }
    }
}