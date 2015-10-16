namespace AudioSwitcher.AudioApi
{
    public sealed class DeviceVolumeChangedArgs
    {

        public IDevice Device
        {
            get;
            set;
        }

        public int Volume
        {
            get;
            private set;
        }

        public DeviceVolumeChangedArgs(IDevice device, int volume)
        {
            Device = device;
            Volume = volume;
        }
    }
}