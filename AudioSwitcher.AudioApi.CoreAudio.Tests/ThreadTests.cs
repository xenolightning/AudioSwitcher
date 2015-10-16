using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi.Observables;
using Xunit;

namespace AudioSwitcher.AudioApi.CoreAudio.Tests
{
    [Collection("CoreAudio")]
    public class ThreadTests
    {
        private IAudioController CreateTestController()
        {
            return new CoreAudioController();
        }

        [Fact]
        public void CoreAudio_Attempted_Thread_Deadlock()
        {
            var originalHandles = Process.GetCurrentProcess().HandleCount;
            Debug.WriteLine("Handles Before: " + originalHandles);
            var controller = CreateTestController();

            for (var i = 0; i < 50; i++)
            {
                controller.AudioDeviceChanged.Subscribe(args =>
                {
                    controller.GetDevices(DeviceState.Active).ToList();
                });

                controller.DefaultPlaybackDevice.SetAsDefault();
                controller.DefaultPlaybackDevice.SetAsDefault();

            }

            var newHandles = Process.GetCurrentProcess().HandleCount;
            Debug.WriteLine("Handles After: " + newHandles);

            //*15 for each device and the handles it requires
            //*3 because that should cater for at least 2 copies of each device
            var maxHandles = controller.GetDevices().Count() * 15 * 3;

            //Ensure it doesn't blow out the handles
            Assert.True(newHandles - originalHandles < maxHandles);
        }

        [Fact]
        public async Task CoreAudio_Attempted_Thread_Deadlock_Async()
        {
            var originalHandles = Process.GetCurrentProcess().HandleCount;
            Debug.WriteLine("Handles Before: " + originalHandles);
            var controller = CreateTestController();
            var tasks = new List<Task>();

            for (var i = 0; i < 50; i++)
            {
                controller.AudioDeviceChanged.Subscribe(args =>
                {
                    controller.GetDevices(DeviceState.Active).ToList();
                });

                tasks.Add(controller.DefaultPlaybackDevice.SetAsDefaultAsync());
                tasks.Add(controller.DefaultPlaybackDevice.SetAsDefaultAsync());
            }

            Task.WaitAll(tasks.ToArray());

            var newHandles = Process.GetCurrentProcess().HandleCount;
            Debug.WriteLine("Handles After: " + newHandles);

            //*15 for each device and the handles it requires
            //*3 because that should cater for at least 2 copies of each device
            var maxHandles = controller.GetDevices().Count() * 15 * 3;

            //Ensure it doesn't blow out the handles
            Assert.True(newHandles - originalHandles < maxHandles);
        }

        [Fact]
        public async Task CoreAudio_SetDefaultPlayback()
        {
            var controller = CreateTestController();

            var dev = controller.DefaultPlaybackDevice;
            var devices = await controller.GetPlaybackDevicesAsync();

            foreach (var d in devices)
            {
                var isDefault = await d.SetAsDefaultAsync();
                Assert.Equal(isDefault, d.IsDefaultDevice);

                if (dev.Id != d.Id && isDefault)
                {
                    Debug.WriteLine("Asserting Default Update");
                    Assert.False(dev.IsDefaultDevice);
                }
            }

            await dev.SetAsDefaultAsync();
            Assert.True(dev.IsDefaultDevice);
        }

        [Fact]
        public async Task CoreAudio_SetDefaultCapture()
        {
            var controller = CreateTestController();

            var dev = controller.DefaultCaptureDevice;
            var devices = await controller.GetCaptureDevicesAsync();

            foreach (var d in devices)
            {
                var isDefault = await d.SetAsDefaultAsync();
                Assert.Equal(isDefault, d.IsDefaultDevice);

                if (dev.Id != d.Id && isDefault)
                {
                    Debug.WriteLine("Asserting Default Update");
                    Assert.False(dev.IsDefaultDevice);
                }
            }

            await dev.SetAsDefaultAsync();
            Assert.True(dev.IsDefaultDevice);
        }

    }
}
