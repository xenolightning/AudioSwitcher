using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AudioSwitcher.AudioApi.CoreAudio.Tests
{
    public class ThreadTests
    {
        private AudioController CreateTestController()
        {
            return new CoreAudioController();
        }

        [Fact]
        public void CoreAudio_Attempted_Thread_Deadlock()
        {
            var originalHandles = Process.GetCurrentProcess().HandleCount;
            var controller = CreateTestController();

            for (int i = 0; i < 50; i++)
            {
                controller.AudioDeviceChanged += (sender, args) =>
                {
                    controller.GetAllDevices(DeviceState.Active).ToList();
                };

                controller.DefaultPlaybackDevice.SetAsDefault();
                controller.DefaultPlaybackDevice.SetAsDefault();

            }

            var newHandles = Process.GetCurrentProcess().HandleCount;

            //Ensure it doesn't blow out the handles
            //200 is enough to cover the overhead of devices
            Assert.True(newHandles - originalHandles < 200);
        }

        [Fact]
        public async Task CoreAudio_Attempted_Thread_Deadlock_Async()
        {
            var originalHandles = Process.GetCurrentProcess().HandleCount;
            var controller = CreateTestController();

            for (int i = 0; i < 50; i++)
            {
                controller.AudioDeviceChanged += (sender, args) =>
                {
                    controller.GetAllDevices(DeviceState.Active).ToList();
                };

                controller.DefaultPlaybackDevice.SetAsDefaultAsync();
                controller.DefaultPlaybackDevice.SetAsDefaultAsync();
            }

            var newHandles = Process.GetCurrentProcess().HandleCount;

            //Ensure it doesn't blow out the handles
            //200 is enough to cover the overhead of devices
            Assert.True(newHandles - originalHandles < 200);
        }

    }
}
