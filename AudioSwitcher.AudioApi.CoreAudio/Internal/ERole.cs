using System;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    [Flags]
    internal enum ERole : uint
    {
        Console = 0x00000000,
        Multimedia = 0x00000001,
        Communications = 0x00000010
    }
}