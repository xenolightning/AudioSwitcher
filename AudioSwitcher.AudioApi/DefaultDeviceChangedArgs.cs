namespace AudioSwitcher.AudioApi
{
    public sealed class DefaultDeviceChangedArgs : DeviceChangedArgs
    {
        public DefaultDeviceChangedArgs(IDevice dev, bool isCommunications = false)
            : base(
                dev,
                isCommunications ? DeviceChangedType.DefaultCommunicationsDevice : DeviceChangedType.DefaultDevice
                )
        {
            IsDefaultEvent = !isCommunications;
            IsDefaultCommunicationsEvent = isCommunications;
        }

        public bool IsDefaultEvent
        {
            get;
            private set;
        }

        public bool IsDefaultCommunicationsEvent
        {
            get;
            private set;
        }
    }
}