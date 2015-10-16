namespace AudioSwitcher.AudioApi
{
    public sealed class DeviceRemovedArgs : DeviceChangedArgs
    {
        public DeviceRemovedArgs(IDevice dev)
            : base(dev, DeviceChangedType.DeviceRemoved)
        {
        }
    }
}