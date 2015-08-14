namespace AudioSwitcher.AudioApi
{
    public sealed class DefaultDeviceChangedEventArgs : DeviceChangedEventArgs
    {
        public DefaultDeviceChangedEventArgs(IDevice dev, bool isCommunications = false)
            : base(
                dev,
                isCommunications ? AudioDeviceEventType.DefaultCommunicationsDevice : AudioDeviceEventType.DefaultDevice
                )
        {
            IsDefaultEvent = !isCommunications;
            IsDefaultCommunicationsEvent = isCommunications;
        }

        public bool IsDefaultEvent { get; set; }
        public bool IsDefaultCommunicationsEvent { get; set; }
    }
}