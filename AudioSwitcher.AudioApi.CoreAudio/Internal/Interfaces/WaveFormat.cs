using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi.CoreAudio.Interfaces
{
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi, Pack=2)]
    public class WaveFormat
    {
        protected WaveFormatEncoding waveFormatTag;
        protected short channels;
        protected int sampleRate;
        protected int averageBytesPerSecond;
        protected short blockAlign;
        protected short bitsPerSample;
        protected short extraSize;

        /// <summary>
        /// Creates a new PCM 44.1Khz stereo 16 bit format
        /// </summary>
        public WaveFormat() : this(44100,16,2)
        {

        }
        
        /// <summary>
        /// Creates a new 16 bit wave format with the specified sample
        /// rate and channel count
        /// </summary>
        /// <param name="sampleRate">Sample Rate</param>
        /// <param name="channels">Number of channels</param>
        public WaveFormat(int sampleRate, int channels)
            : this(sampleRate, 16, channels)
        {
        }

        public WaveFormat(int rate, int bits, int channels)
        {
            if (channels < 1)
            {
                throw new ArgumentOutOfRangeException("channels", "Channels must be 1 or greater");
            }
            // minimum 16 bytes, sometimes 18 for PCM
            waveFormatTag = WaveFormatEncoding.Pcm;
            this.channels = (short)channels;
            sampleRate = rate;
            bitsPerSample = (short)bits;
            extraSize = 0;
                   
            blockAlign = (short)(channels * (bits / 8));
            averageBytesPerSecond = sampleRate * blockAlign;
        }

        public override string ToString()
        {
            switch (waveFormatTag)
            {
                case WaveFormatEncoding.Pcm:
                case WaveFormatEncoding.Extensible:
                    // extensible just has some extra bits after the PCM header
                    return String.Format("{0} bit PCM: {1}kHz {2} channels",
                        bitsPerSample, sampleRate / 1000, channels);
                default:
                    return waveFormatTag.ToString();
            }
        }

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

    }
}
