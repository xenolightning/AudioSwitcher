using System;

namespace AudioSwitcher.AudioApi.Hooking.ComObjects
{
    [Flags]
    public enum EDeviceState : uint
    {
        Active = 0x1,
        Disabled = 0x2,
        NotPresent = 0x4,
        Unplugged = 0x8,
        All = 0xF
    }
}