namespace AudioSwitcher.AudioApi
{
    public sealed class DeviceVolumeChangedArgs : DeviceChangedArgs
    {

        public int Volume
        {
            get;
            private set;
        }

        public DeviceVolumeChangedArgs(IDevice device, int volume) 
            : base(device, DeviceChangedType.VolumeChanged)
        {
            Volume = volume;
        }
    }
}