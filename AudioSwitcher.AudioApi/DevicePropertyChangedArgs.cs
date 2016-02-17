namespace AudioSwitcher.AudioApi
{
    public class DevicePropertyChangedArgs : DeviceChangedArgs
    {
        public string PropertyName { get; private set; }

        public DevicePropertyChangedArgs(IDevice dev, string propertyName = null)
            : base(dev, DeviceChangedType.PropertyChanged)
        {
            PropertyName = propertyName;
        }
    }
}