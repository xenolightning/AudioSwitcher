using System;

namespace AudioSwitcher.AudioApi
{
    [Flags]
    public enum SpeakerConfiguration
    {
        NotSupported = -1,
        DirectOut = 0x0,
        FrontLeft = 0x1,
        FrontRight = 0x2,
        FrontCenter = 0x4,
        LowFrequency = 0x8,
        BackLeft = 0x10,
        BackRight = 0x20,
        FrontLeftOfCenter = 0x40,
        FrontRightOfCenter = 0x80,
        BackCenter = 0x100,
        SideLeft = 0x200,
        SideRight = 0x400,
        TopCenter = 0x800,
        TopFrontLeft = 0x1000,
        TopFrontCenter = 0x2000,
        TopFrontRight = 0x4000,
        TopBackLeft = 0x8000,
        TopBackCenter = 0x10000,
        TopBackRight = 0x20000,

        Mono = FrontCenter,
        Stereo = FrontLeft | FrontRight,
        Quad = Stereo | BackLeft | BackRight,
        Surround = Stereo | FrontCenter | BackCenter,
        FivePointOne = FrontCenter | Quad | LowFrequency,
        FivePointOneSurround = Stereo | FrontCenter | LowFrequency | SideLeft | SideRight,
        SevenPointOne = FivePointOne | FrontLeftOfCenter | FrontRightOfCenter,
        SevenPointOneSurround = FivePointOneSurround | BackLeft | BackRight
    }
}