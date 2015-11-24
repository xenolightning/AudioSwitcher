using System;
using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi.CoreAudio.Interfaces
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 2)]
    public class WaveFormatExtensible : WaveFormat
    {
        private short wValidBitsPerSample;
        private int dwChannelMask;
        private Guid subFormat;

        /// <summary>
        /// Parameterless constructor for marshalling
        /// </summary>
        private WaveFormatExtensible()
        {
        }

        /// <summary>
        /// Creates a new WaveFormatExtensible for PCM or IEEE
        /// </summary>
        public WaveFormatExtensible(int rate, int bits, int channelMask)
            : base(rate, bits, channelMask)
        {
            waveFormatTag = WaveFormatEncoding.Extensible;
            extraSize = 22;
            wValidBitsPerSample = (short)bits;
            dwChannelMask = channelMask;

            //TODO, probably should handle non PCM?
            //if (bits == 32)
            //{
            //    // KSDATAFORMAT_SUBTYPE_IEEE_FLOAT
            //    subFormat = new Guid("00000003-0000-0010-8000-00aa00389b71");
            //}
            //else
            //{
                // KSDATAFORMAT_SUBTYPE_PCM
                subFormat = new Guid("00000001-0000-0010-8000-00aa00389b71");
            //}

        }

        public Guid SubFormat { get { return subFormat; } }

        public override string ToString()
        {
            return String.Format("{0} wBitsPerSample:{1} dwChannelMask:{2} subFormat:{3} extraSize:{4}",
                base.ToString(),
                wValidBitsPerSample,
                dwChannelMask,
                subFormat,
                extraSize);
        }
    }
}
