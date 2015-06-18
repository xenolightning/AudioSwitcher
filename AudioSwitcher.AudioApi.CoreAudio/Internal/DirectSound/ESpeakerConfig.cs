using System;

namespace AudioSwitcher.AudioApi.CoreAudio.DirectSound
{
    [Flags]
    internal enum ESpeakerConfig : uint
    {
        DirectOut = 0x00000000,
        Headphone = 0x00000001,
        Mono = 0x00000002,
        Quad = 0x00000003,
        Stereo = 0x00000004,
        Surround = 0x00000005,
        Back5Point1 = 0x00000006,
        Wide7Point1 = 0x00000007,
        Surround7Point1 = 0x00000008,
        Surround5Point1 = 0x00000009,

        GeometryMin = 0x00050000,
        GeometryNarrow = 0x000A0000,
        GeometryWide = 0x00140000,
        GeometryMax = 0x00B40000
    }

    internal static class ESpeakerConfigExtensions
    {

        public static SpeakerConfiguration AsSpeakerConfiguration(this ESpeakerConfig config)
        {
            var configNoGeometry = (ESpeakerConfig)(((uint)config) & 0xFFFF);
            switch (configNoGeometry)
            {
                case ESpeakerConfig.DirectOut:
                    return SpeakerConfiguration.DirectOut;
                case ESpeakerConfig.Headphone:
                    return SpeakerConfiguration.Stereo;
                case ESpeakerConfig.Mono:
                    return SpeakerConfiguration.Mono;
                case ESpeakerConfig.Quad:
                    return SpeakerConfiguration.Quad;
                case ESpeakerConfig.Stereo:
                    return SpeakerConfiguration.Stereo;
                case ESpeakerConfig.Surround:
                    return SpeakerConfiguration.Surround;
                case ESpeakerConfig.Back5Point1:
                    return SpeakerConfiguration.FivePointOne;
                case ESpeakerConfig.Wide7Point1:
                    return SpeakerConfiguration.SevenPointOne;
                case ESpeakerConfig.Surround7Point1:
                    return SpeakerConfiguration.SevenPointOneSurround;
                case ESpeakerConfig.Surround5Point1:
                    return SpeakerConfiguration.FivePointOneSurround;
                default:
                    throw new ArgumentOutOfRangeException("config", config, null);
            }
        }

        public static ESpeakerConfig AsESpeakerConfig(this SpeakerConfiguration config)
        {
            switch (config)
            {
                case SpeakerConfiguration.DirectOut:
                    return ESpeakerConfig.DirectOut;
                case SpeakerConfiguration.Stereo:
                    return ESpeakerConfig.Stereo;
                case SpeakerConfiguration.Quad:
                    return ESpeakerConfig.Quad;
                case SpeakerConfiguration.Surround:
                    return ESpeakerConfig.Surround;
                case SpeakerConfiguration.FivePointOne:
                    return ESpeakerConfig.Back5Point1;
                case SpeakerConfiguration.FivePointOneSurround:
                    return ESpeakerConfig.Surround5Point1;
                case SpeakerConfiguration.SevenPointOne:
                    return ESpeakerConfig.Wide7Point1;
                case SpeakerConfiguration.SevenPointOneSurround:
                    return ESpeakerConfig.Surround7Point1;
                default:
                    throw new ArgumentOutOfRangeException("config", config, null);
            }
        }

    }
}