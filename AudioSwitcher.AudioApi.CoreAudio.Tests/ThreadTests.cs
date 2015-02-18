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

            //*15 for each device and the handles it requires
            //*3 because that should cater for at least 2 copies of each device
            var maxHandles = controller.GetAllDevices().Count() * 15 * 3;

            //Ensure it doesn't blow out the handles
            Assert.True(newHandles - originalHandles < maxHandles);
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

            //*15 for each device and the handles it requires
            //*3 because that should cater for at least 2 copies of each device
            var maxHandles = controller.GetAllDevices().Count() * 15 * 3;

            //Ensure it doesn't blow out the handles
            Assert.True(newHandles - originalHandles < maxHandles);
        }

    }
}
