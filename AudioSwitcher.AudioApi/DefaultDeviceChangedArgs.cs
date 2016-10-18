namespace AudioSwitcher.AudioApi
{
    public sealed class DefaultDeviceChangedArgs : DeviceChangedArgs
    {
        public bool IsDefault { get; private set; }

        public bool IsDefaultCommunications { get; private set; }

        public DefaultDeviceChangedArgs(IDevice dev)
            : base(dev, DeviceChangedType.DefaultChanged)
        {
            IsDefault = dev.IsDefaultDevice;
            IsDefaultCommunications = dev.IsDefaultCommunicationsDevice;
        }
    }
}