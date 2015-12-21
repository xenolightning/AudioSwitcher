namespace AudioSwitcher.AudioApi
{
    public class DeviceStateChangedArgs : DeviceChangedArgs
    {
        public DeviceStateChangedArgs(IDevice dev, DeviceState state)
            : base(dev, DeviceChangedType.StateChanged)
        {
            State = state;
        }

        public DeviceState State { get; private set; }
    }
}