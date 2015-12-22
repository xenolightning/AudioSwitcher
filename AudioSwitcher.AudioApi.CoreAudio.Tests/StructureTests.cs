using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;
using Xunit;

namespace AudioSwitcher.AudioApi.CoreAudio.Tests
{
    public class StructureTests
    {

        [Fact]
        public void WaveFormatExtensible_Size_Is_Correct()
        {
            var ext = new WaveFormatExtensible(SampleRate.R44100, BitDepth.B16, SpeakerConfiguration.Stereo);


            Assert.Equal(22, ext.ExtraSize);
            Assert.Equal(44100, ext.SampleRate);
            Assert.Equal(16, ext.BitsPerSample);
            Assert.Equal(2, ext.Channels);
        }

    }
}
