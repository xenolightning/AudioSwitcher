using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

            for (int i = 0; i < 50; i++)
            {
                controller.GetDevices();
                var isDefault = controller.DefaultPlaybackDevice.SetAsDefault();
                Assert.True(isDefault);
                controller.GetPlaybackDevices();
            }

            var newHandles = Process.GetCurrentProcess().HandleCount;

            //*15 for each device and the handles it requires
            //*3 because that should cater for at least 2 copies of each device
            var maxHandles = controller.GetDevices().Count() * 15 * 3;

            //Ensure it doesn't blow out the handles
            Assert.True(newHandles - originalHandles < maxHandles);
        }

        [Fact]
        public async Task CoreAudio_NumberOfHandlesAreWithinAcceptableRange_Async()
        {
            var originalHandles = Process.GetCurrentProcess().HandleCount;
            var controller = CreateTestController();

            for (int i = 0; i < 50; i++)
            {
                await controller.GetCaptureDevicesAsync();
                var isDefault = await controller.DefaultPlaybackDevice.SetAsDefaultAsync();
                Assert.True(isDefault);
                await controller.GetPlaybackDevicesAsync();
            }

            var newHandles = Process.GetCurrentProcess().HandleCount;

            //*15 for each device and the handles it requires
            //*3 because that should cater for at least 2 copies of each device
            var maxHandles = controller.GetDevices().Count() * 15 * 3;

            //Ensure it doesn't blow out the handles
            Assert.True(newHandles - originalHandles < maxHandles);
        }

    }
}
