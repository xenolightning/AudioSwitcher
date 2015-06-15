using System;

namespace AudioSwitcher.AudioApi.Hooking.ComObjects
{
    [Flags]
    public enum ERole : uint
    {
        Console = 0,
        Multimedia = 1,
        Communications = 2
    }
}