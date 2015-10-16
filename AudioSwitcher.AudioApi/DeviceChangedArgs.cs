using System;

namespace AudioSwitcher.AudioApi
{
    public abstract class DeviceChangedArgs : EventArgs
    {
        protected DeviceChangedArgs(IDevice dev, DeviceChangedType type)
        {
            Device = dev;
            ChangedType = type;
        }

        public IDevice Device { get; private set; }

        public DeviceChangedType ChangedType { get; private set; }
    }
}