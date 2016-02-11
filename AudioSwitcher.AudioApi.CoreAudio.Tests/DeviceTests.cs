using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AudioSwitcher.AudioApi.CoreAudio.Tests
{
    public class DeviceTests
    {
        private IAudioController CreateTestController()
        {
            return new CoreAudioController();
        }

        [Fact]
        public void Device_Set_Volume()
        {
            using (var controller = CreateTestController())
            {
                var currentVolume = controller.DefaultPlaybackDevice.Volume;

                controller.DefaultPlaybackDevice.Volume = 0;

                Assert.Equal(0, (int)controller.DefaultPlaybackDevice.Volume);

                controller.DefaultPlaybackDevice.Volume = currentVolume;
            }

        }

        [Fact]
        public async Task Device_Set_Volume_Async()
        {
            using (var controller = CreateTestController())
            {
                var currentVolume = controller.DefaultPlaybackDevice.Volume;

                await controller.DefaultPlaybackDevice.SetVolumeAsync(0);

                Assert.Equal(0, (int)controller.DefaultPlaybackDevice.Volume);

                await controller.DefaultPlaybackDevice.SetVolumeAsync(currentVolume);
            }

        }

        [Fact]
        public void Device_Set_Default()
        {
            using (var controller = CreateTestController())
            {
                controller.DefaultPlaybackDevice.SetAsDefault();

                Assert.True(controller.DefaultPlaybackDevice.IsDefaultDevice);
            }

        }

        [Fact]
        public async Task Device_Set_Default_Async()
        {
            using (var controller = CreateTestController())
            {
                await controller.DefaultPlaybackDevice.SetAsDefaultAsync();

                Assert.True(controller.DefaultPlaybackDevice.IsDefaultDevice);
            }

        }

        [Fact]
        public void Device_Set_Default_Communications()
        {
            using (var controller = CreateTestController())
            {
                controller.DefaultPlaybackDevice.SetAsDefaultCommunications();

                Assert.True(controller.DefaultPlaybackDevice.IsDefaultDevice);
            }
        }

        [Fact]
        public async Task Device_Set_Default_Communications_Async()
        {
            using (var controller = CreateTestController())
            {
                await controller.DefaultPlaybackDevice.SetAsDefaultCommunicationsAsync();

                Assert.True(controller.DefaultPlaybackDevice.IsDefaultDevice);
            }
        }

    }
}
