namespace AudioSwitcher.AudioApi
{
    public sealed class DefaultDeviceChangedArgs : DeviceChangedArgs
    {
        public DefaultDeviceChangedArgs(IDevice dev)
            : base(dev, DeviceChangedType.DefaultChanged)
        {
            IsDefault = dev.IsDefaultDevice;
            IsDefaultCommunications = dev.IsDefaultCommunicationsDevice;
        }

        public bool IsDefault
        {
            get;
            private set;
        }

        public bool IsDefaultCommunications
        {
            get;
            private set;
        }
    }
}
