namespace AudioSwitcher.AudioApi
{
    public sealed class DeviceVolumeChangedEventArgs : DeviceChangedEventArgs
    {
        public int Volume { get; private set; }
        public bool IsMuted { get; private set; }

        public DeviceVolumeChangedEventArgs(IDevice dev, int volume, bool isMuted)
            : base(dev, AudioDeviceEventType.Volume)
        {
            Volume = volume;
            IsMuted = isMuted;
        }
    }
}
