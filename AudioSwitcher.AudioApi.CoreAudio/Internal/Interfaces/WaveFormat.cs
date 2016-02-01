using System;
using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi.CoreAudio.Interfaces
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 2)]
    public class WaveFormat
    {
        private readonly int averageBytesPerSecond;
        private readonly short bitsPerSample;
        private readonly short blockAlign;
        private readonly short channels;
        private readonly short extraSize;
        private readonly WaveFormatEncoding waveFormatTag;
        private int sampleRate;

        public WaveFormatEncoding Encoding
        {
            get
            {
                return waveFormatTag;
            }
        }

        public int Channels
        {
            get
            {
                return channels;
            }
        }

        public int SampleRate
        {
            get
            {
                return sampleRate;
            }
        }

        public int AverageBytesPerSecond
        {
            get
            {
                return averageBytesPerSecond;
            }
        }

        public virtual int BlockAlign
        {
            get
            {
                return blockAlign;
            }
        }

        public int BitsPerSample
        {
            get
            {
                return bitsPerSample;
            }
        }

        public int ExtraSize
        {
            get
            {
                return extraSize;
            }
        }

        protected WaveFormat()
        {
            
        }

        public WaveFormat(SampleRate rate, BitDepth bits, SpeakerConfiguration channelMask)
            :this(rate, bits, channelMask, WaveFormatEncoding.Pcm, Marshal.SizeOf(typeof(WaveFormat)))
        {
            
        }

        protected WaveFormat(SampleRate rate, BitDepth bits, SpeakerConfiguration channelMask, WaveFormatEncoding formatTag, int totalSize)
        {
            channels = ChannelsFromMask((int)channelMask);

            if (channels < 1)
            {
                throw new ArgumentOutOfRangeException("channelMask", "Channels must be 1 or greater");
            }
            // minimum 16 bytes, sometimes 18 for PCM
            waveFormatTag = WaveFormatEncoding.Pcm;

            sampleRate = (int)rate;
            bitsPerSample = (short)bits;
            extraSize = 0;

            blockAlign = (short)(channels * (bitsPerSample / 8));
            averageBytesPerSecond = sampleRate * blockAlign;

            waveFormatTag = formatTag;
            extraSize = (short)(totalSize - Marshal.SizeOf(typeof(WaveFormat)));
        }

        private short ChannelsFromMask(int channelMask)
        {
            short count = 0;

            // until all bits are zero
            while (channelMask > 0)
            {
                // check lower bit
                if ((channelMask & 1) == 1)
                    count++;

                // shift bits, removing lower bit
                channelMask >>= 1;
            }

            return count;
        }

        public override string ToString()
        {
            switch (waveFormatTag)
            {
                case WaveFormatEncoding.Pcm:
                case WaveFormatEncoding.Extensible:
                    // formatTag just has some extra bits after the PCM header
                    return String.Format("{0} bit PCM: {1}kHz {2} channels",
                        bitsPerSample, sampleRate / 1000, channels);
                default:
                    return waveFormatTag.ToString();
            }
        }
    }
}
