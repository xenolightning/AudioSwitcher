namespace AudioSwitcher.AudioApi
{
    public class DeviceRemovedArgs : DeviceChangedArgs
    {
        public DeviceRemovedArgs(IDevice dev)
            : base(dev, DeviceChangedType.DeviceRemoved)
        {
        }
    }
}