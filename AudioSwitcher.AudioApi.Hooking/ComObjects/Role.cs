using System;

namespace AudioSwitcher.AudioApi.Hooking.ComObjects
{
    [Flags]
    public enum Role : uint
    {
        Console = 0,
        Multimedia = 1,
        Communications = 2
    }
}