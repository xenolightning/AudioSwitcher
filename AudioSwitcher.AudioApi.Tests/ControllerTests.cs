using System.Linq;
using AudioSwitcher.Tests.Common;
using Xunit;

namespace AudioSwitcher.AudioApi.Tests
{
    public class ControllerTests
    {
        private IAudioController CreateTestController()
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

        [Fact]
        public void Controller_Capture_SetDefault_1()
        {
            var controller = CreateTestController();
            var device = controller.GetCaptureDevices().First(x => !x.IsDefaultDevice);

            Assert.True(device.SetAsDefault());
            Assert.Same(controller.DefaultCaptureDevice, device);
        }

        [Fact]
        public void Controller_Capture_SetDefault_2()
        {
            var controller = CreateTestController();
            var device = controller.GetCaptureDevices().First(x => !x.IsDefaultDevice);

            Assert.True(controller.SetDefaultDevice(device));
            Assert.Same(controller.DefaultCaptureDevice, device);
        }

        [Fact]
        public void Controller_Capture_SetDefault_3()
        {
            var controller = CreateTestController();
            var device = controller.GetCaptureDevices().First(x => !x.IsDefaultDevice);

            Assert.DoesNotThrow(() => controller.DefaultCaptureDevice = device);
            Assert.Same(controller.DefaultCaptureDevice, device);
        }

        [Fact]
        public void Controller_Playback_SetDefault_1()
        {
            var controller = CreateTestController();
            var device = controller.GetPlaybackDevices().First(x => !x.IsDefaultDevice);

            Assert.True(device.SetAsDefault());
            Assert.Same(controller.DefaultPlaybackDevice, device);
        }

        [Fact]
        public void Controller_Playback_SetDefault_2()
        {
            var controller = CreateTestController();
            var device = controller.GetPlaybackDevices().First(x => !x.IsDefaultDevice);

            Assert.True(controller.SetDefaultDevice(device));
            Assert.Same(controller.DefaultPlaybackDevice, device);
        }

        [Fact]
        public void Controller_Playback_SetDefault_3()
        {
            var controller = CreateTestController();
            var device = controller.GetPlaybackDevices().First(x => !x.IsDefaultDevice);

            Assert.DoesNotThrow(() => controller.DefaultPlaybackDevice = device);
            Assert.Same(controller.DefaultPlaybackDevice, device);
        }

        [Fact]
        public void Controller_Capture_SetDefaultComm_1()
        {
            var controller = CreateTestController();
            var device = controller.GetCaptureDevices().First(x => !x.IsDefaultDevice);

            Assert.True(device.SetAsDefaultCommunications());
            Assert.Same(controller.DefaultCaptureCommunicationsDevice, device);
        }

        [Fact]
        public void Controller_Capture_SetDefaultComm_2()
        {
            var controller = CreateTestController();
            var device = controller.GetCaptureDevices().First(x => !x.IsDefaultDevice);

            Assert.True(controller.SetDefaultCommunicationsDevice(device));
            Assert.Same(controller.DefaultCaptureCommunicationsDevice, device);
        }

        [Fact]
        public void Controller_Capture_SetDefaultComm_3()
        {
            var controller = CreateTestController();
            var device = controller.GetCaptureDevices().First(x => !x.IsDefaultDevice);

            Assert.DoesNotThrow(() => controller.DefaultCaptureCommunicationsDevice = device);
            Assert.Same(controller.DefaultCaptureCommunicationsDevice, device);
        }

        [Fact]
        public void Controller_Playback_SetDefaultComm_1()
        {
            var controller = CreateTestController();
            var device = controller.GetPlaybackDevices().First(x => !x.IsDefaultDevice);

            Assert.True(device.SetAsDefaultCommunications());
            Assert.Same(controller.DefaultPlaybackCommunicationsDevice, device);
        }

        [Fact]
        public void Controller_Playback_SetDefaultComm_2()
        {
            var controller = CreateTestController();
            var device = controller.GetPlaybackDevices().First(x => !x.IsDefaultDevice);

            Assert.True(controller.SetDefaultCommunicationsDevice(device));
            Assert.Same(controller.DefaultPlaybackCommunicationsDevice, device);
        }

        [Fact]
        public void Controller_Playback_SetDefaultComm_3()
        {
            var controller = CreateTestController();
            var device = controller.GetPlaybackDevices().First(x => !x.IsDefaultDevice);

            Assert.DoesNotThrow(() => controller.DefaultPlaybackCommunicationsDevice = device);
            Assert.Same(controller.DefaultPlaybackCommunicationsDevice, device);
        }
    }
}
