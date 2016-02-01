namespace AudioSwitcher.AudioApi
{
    public class DeviceStateChangedArgs : DeviceChangedArgs
    {
        public DeviceState State { get; private set; }

        public DeviceStateChangedArgs(IDevice dev, DeviceState state)
            : base(dev, DeviceChangedType.StateChanged)
        {
            State = state;
        }
    }
}