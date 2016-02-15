using System.Collections.Concurrent;
using System.Threading.Tasks;
using Xunit;

namespace AudioSwitcher.AudioApi.CoreAudio.Tests
{
    [Collection("CoreAudio")]
    public class DeviceTests
    {
        private CoreAudioController CreateTestController()
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
                var device = controller.DefaultPlaybackDevice;

                var isDefault = device.SetAsDefault();

                Assert.True(device.IsDefaultDevice);
                Assert.Equal(isDefault, device.IsDefaultDevice);
            }

        }

        [Fact]
        public async Task Device_Set_Default_Async()
        {
            using (var controller = CreateTestController())
            {
                var device = controller.DefaultPlaybackDevice;

                var isDefault = await device.SetAsDefaultAsync();

                Assert.True(device.IsDefaultDevice);
                Assert.Equal(isDefault, device.IsDefaultDevice);
            }

        }

        [Fact]
        public async Task Device_Set_Default_Async_Returns_In_Order()
        {
            using (var controller = CreateTestController())
            {
                var order = new ConcurrentQueue<int>();
                var device = controller.DefaultPlaybackDevice;

                var t1 = device.SetAsDefaultAsync().ContinueWith(x =>
                {
                    order.Enqueue(1);
                });

                await Task.Delay(5);
                var t2 = device.SetAsDefaultAsync().ContinueWith(x =>
                {
                    order.Enqueue(2);
                });

                await Task.Delay(5);
                var t3 = device.SetAsDefaultAsync().ContinueWith(x =>
                {
                    order.Enqueue(3);
                });

                await Task.Delay(5);
                var t4 = device.SetAsDefaultAsync().ContinueWith(x =>
                {
                    order.Enqueue(4);
                });

                await Task.WhenAll(t1, t2, t3, t4);

                int result;

                order.TryDequeue(out result);
                Assert.Equal(1, result);

                order.TryDequeue(out result);
                Assert.Equal(2, result);

                order.TryDequeue(out result);
                Assert.Equal(3, result);

                order.TryDequeue(out result);
                Assert.Equal(4, result);
            }

        }

        [Fact]
        public void Device_Set_Default_Communications()
        {
            using (var controller = CreateTestController())
            {
                var device = controller.DefaultPlaybackCommunicationsDevice;

                var isDefault = device.SetAsDefaultCommunications();

                Assert.True(device.IsDefaultCommunicationsDevice);
                Assert.Equal(isDefault, device.IsDefaultCommunicationsDevice);
            }
        }

        [Fact]
        public async Task Device_Set_Default_Communications_Async()
        {
            using (var controller = CreateTestController())
            {
                var device = controller.DefaultPlaybackCommunicationsDevice;

                var isDefault = await device.SetAsDefaultCommunicationsAsync();

                Assert.True(device.IsDefaultCommunicationsDevice);
                Assert.Equal(isDefault, device.IsDefaultCommunicationsDevice);
            }
        }

        [Fact]
        public async Task Device_Set_Default_Communications_Async_Returns_In_Order()
        {
            using (var controller = CreateTestController())
            {
                var order = new ConcurrentQueue<int>();
                var device = controller.DefaultPlaybackCommunicationsDevice;

                var t1 = device.SetAsDefaultCommunicationsAsync().ContinueWith(x =>
                {
                    order.Enqueue(1);
                });

                await Task.Delay(5);
                var t2 = device.SetAsDefaultCommunicationsAsync().ContinueWith(x =>
                {
                    order.Enqueue(2);
                });

                await Task.Delay(5);
                var t3 = device.SetAsDefaultCommunicationsAsync().ContinueWith(x =>
                {
                    order.Enqueue(3);
                });

                await Task.Delay(5);
                var t4 = device.SetAsDefaultCommunicationsAsync().ContinueWith(x =>
                {
                    order.Enqueue(4);
                });

                await Task.WhenAll(t1, t2, t3, t4);

                int result;

                order.TryDequeue(out result);
                Assert.Equal(1, result);

                order.TryDequeue(out result);
                Assert.Equal(2, result);

                order.TryDequeue(out result);
                Assert.Equal(3, result);

                order.TryDequeue(out result);
                Assert.Equal(4, result);
            }

        }

        [Fact]
        public void Device_Toggle_Mute()
        {
            using (var controller = CreateTestController())
            {
                var isMuted = controller.DefaultPlaybackDevice.IsMuted;

                var newMute = controller.DefaultPlaybackDevice.ToggleMute();

                Assert.NotEqual(isMuted, newMute);

                newMute = controller.DefaultPlaybackDevice.ToggleMute();

                Assert.Equal(isMuted, newMute);
            }
        }

        [Fact]
        public async Task Device_Toggle_Mute_Async()
        {
            using (var controller = CreateTestController())
            {
                var isMuted = controller.DefaultPlaybackDevice.IsMuted;

                var newMute = await controller.DefaultPlaybackDevice.ToggleMuteAsync();

                Assert.NotEqual(isMuted, newMute);

                newMute = await controller.DefaultPlaybackDevice.ToggleMuteAsync();

                Assert.Equal(isMuted, newMute);
            }
        }

        [Fact]
        public void Device_Mute()
        {
            using (var controller = CreateTestController())
            {
                var isMuted = controller.DefaultPlaybackDevice.IsMuted;

                var newMute = controller.DefaultPlaybackDevice.Mute(true);
                Assert.True(newMute);

                newMute = controller.DefaultPlaybackDevice.Mute(false);
                Assert.False(newMute);

                controller.DefaultPlaybackDevice.Mute(isMuted);
            }
        }

        [Fact]
        public async Task Device_Mute_Async()
        {
            using (var controller = CreateTestController())
            {
                var isMuted = controller.DefaultPlaybackDevice.IsMuted;

                var newMute = await controller.DefaultPlaybackDevice.MuteAsync(true);
                Assert.True(newMute);

                newMute = await controller.DefaultPlaybackDevice.MuteAsync(false);
                Assert.False(newMute);

                await controller.DefaultPlaybackDevice.MuteAsync(isMuted);
            }
        }

    }
}
