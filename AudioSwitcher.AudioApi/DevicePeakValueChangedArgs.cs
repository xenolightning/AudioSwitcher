namespace AudioSwitcher.AudioApi
{
    public class DevicePeakValueChangedArgs : DeviceChangedArgs
    {
        public double PeakValue { get; private set; }

        public DevicePeakValueChangedArgs(IDevice device, double peakValue)
            : base(device, DeviceChangedType.PeakValueChanged)
        {
            PeakValue = peakValue;
        }
    }
}