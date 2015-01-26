using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AudioSwitcher.AudioApi.CoreAudio.Tests
{
    public class ControllerTests
    {
        private IAudioController CreateTestController()
        {
            return new CoreAudioController();
        }

        [Fact]
        public void CoreAudio_NumberOfHandlesAreWithinAcceptableRange()
        {
            var originalHandles = Process.GetCurrentProcess().HandleCount;
            var controller = CreateTestController();

            for (int i = 0; i < 1000; i++)
            {
                var list = controller.GetAllDevices().ToList();
                var isDefault = controller.DefaultPlaybackDevice.SetAsDefault();
                Assert.True(isDefault);
                list = controller.GetPlaybackDevices().ToList();
            }

            var newHandles = Process.GetCurrentProcess().HandleCount;

            //Ensure it doesn't blow out the handles
            //200 is enough to cover the overhead of devices
            Assert.True(newHandles - originalHandles < 200);
        }

        [Fact]
        public async Task CoreAudio_NumberOfHandlesAreWithinAcceptableRange_Async()
        {
            var originalHandles = Process.GetCurrentProcess().HandleCount;
            var controller = CreateTestController();

            for (int i = 0; i < 1000; i++)
            {
                var list = await controller.GetCaptureDevicesAsync();
                var isDefault = await controller.DefaultPlaybackDevice.SetAsDefaultAsync();
                Assert.True(isDefault);
                list = await controller.GetPlaybackDevicesAsync();
            }

            var newHandles = Process.GetCurrentProcess().HandleCount;

            //Ensure it doesn't blow out the handles
            //200 is enough to cover the overhead of devices
            Assert.True(newHandles - originalHandles < 200);
        }

    }
}
