namespace AudioSwitcher.AudioApi
{
    public class DeviceVolumeChangedArgs : DeviceChangedArgs
    {
        public double Volume { get; private set; }

        public DeviceVolumeChangedArgs(IDevice device, double volume)
            : base(device, DeviceChangedType.VolumeChanged)
        {
            Volume = volume;
        }
    }
}