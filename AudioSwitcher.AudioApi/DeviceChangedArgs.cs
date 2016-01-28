using System;

namespace AudioSwitcher.AudioApi
{
    public abstract class DeviceChangedArgs : EventArgs
    {
        public IDevice Device { get; private set; }

        public DeviceChangedType ChangedType { get; private set; }

        protected DeviceChangedArgs(IDevice dev, DeviceChangedType type)
        {
            Device = dev;
            ChangedType = type;
        }
    }
}