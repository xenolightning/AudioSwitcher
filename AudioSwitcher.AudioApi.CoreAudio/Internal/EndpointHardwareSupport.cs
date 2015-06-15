using System;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    [Flags]
    public enum EndpointHardwareSupport
    {
        Volume = 0x1,
        Mute = 0x2,
        Meter = 0x4
    }
}