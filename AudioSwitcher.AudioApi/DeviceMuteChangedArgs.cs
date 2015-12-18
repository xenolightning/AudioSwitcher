namespace AudioSwitcher.AudioApi
{
    public sealed class DeviceMuteChangedArgs
    {

        public IDevice Device
        {
            get;
            private set;
        }

        public bool IsMuted
        {
            get;
            private set;
        }

        public DeviceMuteChangedArgs(IDevice device, bool isMuted)
        {
            Device = device;
            IsMuted = isMuted;
        }
    }
}