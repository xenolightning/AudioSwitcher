using System;
using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi.CoreAudio.Interfaces
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 2)]
    internal class WaveFormatExtensible : WaveFormatEx
    {
        private readonly short wValidBitsPerSample;
        private readonly int dwChannelMask;
        private readonly Guid subFormat;

        public short ValidBitsPerSample => wValidBitsPerSample;

        public SpeakerConfiguration ChannelMask => (SpeakerConfiguration)dwChannelMask;

        public Guid SubFormat => subFormat;

        /// <summary>
        /// Parameterless constructor for marshalling
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        private WaveFormatExtensible()
        {
        }

        /// <summary>
        /// Creates a new WaveFormatExtensible for PCM
        /// KSDATAFORMAT_SUBTYPE_PCM
        /// </summary>
        public WaveFormatExtensible(SampleRate rate, BitDepth bits, SpeakerConfiguration channelMask)
            : this(rate, bits, channelMask, new Guid("00000001-0000-0010-8000-00AA00389B71"))
        {
            wValidBitsPerSample = (short)bits;
            dwChannelMask = (int)channelMask;
        }

        public WaveFormatExtensible(SampleRate rate, BitDepth bits, SpeakerConfiguration channelMask, Guid subFormat)
            : base(rate, bits, channelMask, WaveFormatEncoding.Extensible, Marshal.SizeOf(typeof(WaveFormatExtensible)))
        {
            wValidBitsPerSample = (short)bits;
            dwChannelMask = (int)channelMask;

            this.subFormat = subFormat;
        }

        public override string ToString()
        {
            return
                $"{base.ToString()} wBitsPerSample:{wValidBitsPerSample} dwChannelMask:{dwChannelMask} subFormat:{subFormat} extraSize:{ExtraSize}";
        }
    }
}
