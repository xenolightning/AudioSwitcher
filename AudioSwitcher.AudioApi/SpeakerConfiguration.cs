namespace AudioSwitcher.AudioApi
{
    public enum SpeakerConfiguration : uint
    {
        SpeakerDirectOut = 0x0,
        SpeakerFrontLeft = 0x1,
        SpeakerFrontRight = 0x2,
        SpeakerFrontCenter = 0x4,
        SpeakerLowFrequency = 0x8,
        SpeakerBackLeft = 0x10,
        SpeakerBackRight = 0x20,
        SpeakerFrontLeftOfCenter = 0x40,
        SpeakerFrontRightOfCenter = 0x80,
        SpeakerBackCenter = 0x100,
        SpeakerSideLeft = 0x200,
        SpeakerSideRight = 0x400,
        SpeakerTopCenter = 0x800,
        SpeakerTopFrontLeft = 0x1000,
        SpeakerTopFrontCenter = 0x2000,
        SpeakerTopFrontRight = 0x4000,
        SpeakerTopBackLeft = 0x8000,
        SpeakerTopBackCenter = 0x10000,
        SpeakerTopBackRight = 0x20000,

        Mono = (SpeakerFrontCenter),
        Stereo = (SpeakerFrontLeft | SpeakerFrontRight),
        Quad = (Stereo | SpeakerBackLeft | SpeakerBackRight),
        Surround = (Stereo | SpeakerFrontCenter | SpeakerBackCenter),
        FivePointOne = (Quad | SpeakerLowFrequency),
        FivePointOneSurround = (Stereo | SpeakerFrontCenter | SpeakerLowFrequency | SpeakerSideLeft | SpeakerSideRight),
        SevenPointOne = (FivePointOne | SpeakerFrontLeftOfCenter | SpeakerFrontRightOfCenter),
        SevenPointOneSurround = (FivePointOneSurround | SpeakerBackLeft | SpeakerBackRight)
    }
}
