using System;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    [Flags]
    public enum EndpointHardwareSupport
    {
        Volume = 0x0001,
        Mute = 0x0002,
        Meter = 0x0004
    }
}