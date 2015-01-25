using System.Linq;
using AudioSwitcher.Tests.Common;
using Xunit;

namespace AudioSwitcher.AudioApi.Tests
{
    public partial class DeviceTests
    {
        private IAudioController CreateTestController()
        {
            return new TestAudioController(new TestDeviceEnumerator(2, 2));
        }


        [Fact]
        public void Device_DeviceType_PlaybackIsPlayback()
        {
            var controller = CreateTestController();
            Assert.True(DeviceType.Playback.HasFlag(controller.GetPlaybackDevices().First().DeviceType));
        }

        [Fact]
        public void Device_DeviceType_PlaybackIsAll()
        {
            var controller = CreateTestController();
            Assert.True(DeviceType.All.HasFlag(controller.GetPlaybackDevices().First().DeviceType));
        }

        [Fact]
        public void Device_DeviceType_CaptureIsCapture()
        {
            var controller = CreateTestController();
            Assert.True(DeviceType.Capture.HasFlag(controller.GetCaptureDevices().First().DeviceType));
        }

        [Fact]
        public void Device_DeviceType_CaptureIsAll()
        {
            var controller = CreateTestController();
            Assert.True(DeviceType.All.HasFlag(controller.GetCaptureDevices().First().DeviceType));
        }
    }
}
