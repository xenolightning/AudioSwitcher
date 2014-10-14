using System.Linq;
using AudioSwitcher.Tests.Common;
using Xunit;

namespace AudioSwitcher.AudioApi.Tests
{
    public class ControllerTests
    {
        private AudioController CreateTestController()
        {
            return new TestAudioController(new TestDeviceEnumerator(2, 2));
        }

        [Fact]
        public void Create_Controller()
        {
            var controller = CreateTestController();
            Assert.NotNull(controller);
        }

        [Fact]
        public void Controller_GetAllDevices()
        {
            var controller = CreateTestController();
            Assert.NotEmpty(controller.GetAllDevices());
            Assert.Equal(4, controller.GetAllDevices().Count());
        }

        [Fact]
        public void Controller_GetAllPlaybackDevices()
        {
            var controller = CreateTestController();
            Assert.NotEmpty(controller.GetPlaybackDevices());
            Assert.Equal(2, controller.GetPlaybackDevices().Count());
            Assert.True(controller.GetPlaybackDevices().All(x => x.IsPlaybackDevice));
        }

        [Fact]
        public void Controller_GetAllCaptureDevices()
        {
            var controller = CreateTestController();
            Assert.NotEmpty(controller.GetCaptureDevices());
            Assert.Equal(2, controller.GetCaptureDevices().Count());
            Assert.True(controller.GetCaptureDevices().All(x => x.IsCaptureDevice));
        }

        [Fact]
        public void Controller_GetAudioDevice_Playback_1()
        {
            var controller = CreateTestController();
            var device = controller.GetPlaybackDevices().ToList()[0];

            Assert.NotNull(device);

            var sameDevice = controller.GetAudioDevice(device.Id);
            var sameDevice2 = controller.GetAudioDevice(device.Id, device.State);

            Assert.Same(device, sameDevice);
            Assert.Same(device, sameDevice2);
        }

        [Fact]
        public void Controller_GetAudioDevice_Playback_2()
        {
            var controller = CreateTestController();
            var device = controller.GetPlaybackDevices().ToList()[1];

            Assert.NotNull(device);

            var sameDevice = controller.GetAudioDevice(device.Id);
            var sameDevice2 = controller.GetAudioDevice(device.Id, device.State);

            Assert.Same(device, sameDevice);
            Assert.Same(device, sameDevice2);
        }

        [Fact]
        public void Controller_GetAudioDevice_Capture_1()
        {
            var controller = CreateTestController();
            var device = controller.GetCaptureDevices().ToList()[0];

            Assert.NotNull(device);

            var sameDevice = controller.GetAudioDevice(device.Id);
            var sameDevice2 = controller.GetAudioDevice(device.Id, device.State);

            Assert.Same(device, sameDevice);
            Assert.Same(device, sameDevice2);
        }

        [Fact]
        public void Controller_GetAudioDevice_Capture_2()
        {
            var controller = CreateTestController();
            var device = controller.GetCaptureDevices().ToList()[1];

            Assert.NotNull(device);

            var sameDevice = controller.GetAudioDevice(device.Id);
            var sameDevice2 = controller.GetAudioDevice(device.Id, device.State);

            Assert.Same(device, sameDevice);
            Assert.Same(device, sameDevice2);
        }

    }
}
