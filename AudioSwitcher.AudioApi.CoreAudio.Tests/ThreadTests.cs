using System;
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
            using (var controller = CreateTestController())
            {
                controller.AudioDeviceChanged.Subscribe(x =>
                {
                    Console.WriteLine(x.ToString());
                });

                for (var i = 0; i < 50; i++)
                {

                    controller.DefaultPlaybackDevice.SetAsDefault();
                }

                var newHandles = Process.GetCurrentProcess().HandleCount;
                Debug.WriteLine("Handles After: " + newHandles);

                //*15 for each device and the handles it requires
                //*3 because that should cater for at least 2 copies of each device
                var maxHandles = controller.GetDevices().Count()*20*3;

                //Ensure it doesn't blow out the handles
                Assert.True(newHandles - originalHandles < maxHandles);
            }
        }

        [Fact]
        public async Task CoreAudio_Attempted_Thread_Deadlock_Async()
        {
            var originalHandles = Process.GetCurrentProcess().HandleCount;
            Debug.WriteLine("Handles Before: " + originalHandles);
            using (var controller = CreateTestController())
            {
                var tasks = new List<Task>();

                controller.AudioDeviceChanged.Subscribe(x =>
                {
                    Console.WriteLine(x.ToString());
                });

                for (var i = 0; i < 50; i++)
                {

                    tasks.Add(controller.DefaultPlaybackDevice.SetAsDefaultAsync());
                }

                await Task.WhenAll(tasks.ToArray());

                var newHandles = Process.GetCurrentProcess().HandleCount;
                Debug.WriteLine("Handles After: " + newHandles);

                //*15 for each device and the handles it requires
                //*3 because that should cater for at least 2 copies of each device
                var maxHandles = controller.GetDevices().Count()*20*3;

                //Ensure it doesn't blow out the handles
                Assert.True(newHandles - originalHandles < maxHandles);
            }
        }

        [Fact]
        public async Task CoreAudio_SetDefaultPlayback()
        {
            using (var controller = CreateTestController())
            {

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
        }

        [Fact]
        public async Task CoreAudio_SetDefaultCapture()
        {
            using (var controller = CreateTestController())
            {
                var dev = controller.DefaultCaptureDevice;
                var devices = await controller.GetCaptureDevicesAsync();
                bool sucess;

                foreach (var d in devices)
                {
                    sucess = await d.SetAsDefaultAsync();
                    Assert.Equal(sucess, d.IsDefaultDevice);

                    if (dev.Id != d.Id && sucess)
                    {
                        Debug.WriteLine("Asserting Default Update");
                        Assert.False(dev.IsDefaultDevice);
                    }
                }

                sucess = await dev.SetAsDefaultAsync();
                Assert.Equal(sucess, dev.IsDefaultDevice);
            }
        }

    }
}
