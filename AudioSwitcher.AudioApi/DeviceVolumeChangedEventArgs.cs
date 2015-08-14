namespace AudioSwitcher.AudioApi
{
    public sealed class DeviceVolumeChangedEventArgs : DeviceChangedEventArgs
    {
        public DeviceVolumeChangedEventArgs(IDevice dev, int volume)
            : base(dev, AudioDeviceEventType.Volume)
        {
            Volume = volume;
        }

        public int Volume { get; private set; }
    }
}