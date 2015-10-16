namespace AudioSwitcher.AudioApi
{
    public sealed class DeviceAddedArgs : DeviceChangedArgs
    {
        public DeviceAddedArgs(IDevice dev)
            : base(dev, DeviceChangedType.DeviceAdded)
        {
        }
    }
}