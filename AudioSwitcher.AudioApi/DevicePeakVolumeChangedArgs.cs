namespace AudioSwitcher.AudioApi
{
    public class DevicePeakVolumeChangedArgs : DeviceChangedArgs
    {

        public double Volume
        {
            get;
            private set;
        }

        public DevicePeakVolumeChangedArgs(IDevice device, double volume) 
            : base(device, DeviceChangedType.PeakVolumeChanged)
        {
            Volume = volume;
        }
    }
}