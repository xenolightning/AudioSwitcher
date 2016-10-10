using System.Collections.Concurrent;
using System.Threading.Tasks;
using Xunit;

namespace AudioSwitcher.AudioApi.CoreAudio.Tests
{
    [Collection("CoreAudio_Device")]
    public class DeviceTests
    {
        private CoreAudioController CreateTestController()
        {
            return new CoreAudioController();
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
        public async Task Device_Set_Volume_Async_2()
        {
            using (var controller = CreateTestController())
            {
                var currentVolume = controller.DefaultPlaybackDevice.Volume;

                await controller.DefaultPlaybackDevice.SetVolumeAsync(20);

                Assert.Equal(20, (int)controller.DefaultPlaybackDevice.Volume);

                await controller.DefaultPlaybackDevice.SetVolumeAsync(currentVolume);
            }

        }

        [Fact]
        public async Task Device_Set_Volume_Async_Negative_Is_Zero()
        {
            using (var controller = CreateTestController())
            {
                var currentVolume = controller.DefaultPlaybackDevice.Volume;

                await controller.DefaultPlaybackDevice.SetVolumeAsync(-5);

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
        public void Device_Set_All_Default()
        {
            using (var controller = CreateTestController())
            {
                var originalCommDevice = controller.DefaultPlaybackDevice;

                foreach (var device in controller.GetPlaybackDevices(DeviceState.Active))
                {
                    try
                    {
                        var isDefault = device.SetAsDefault();
                        Assert.Equal(isDefault, device.IsDefaultDevice);
                    }
                    catch (ComInteropTimeoutException)
                    {
                        //Can't set the default, don't fail the test
                    }
                }

                var originalIsDefault = originalCommDevice.SetAsDefault();
                Assert.True(originalIsDefault);
            }
        }

        [Fact]
        public async Task Device_Set_All_Default_Async()
        {
            using (var controller = CreateTestController())
            {
                var originalCommDevice = controller.DefaultPlaybackDevice;

                foreach (var device in await controller.GetPlaybackDevicesAsync(DeviceState.Active))
                {
                    try
                    {
                        var isDefault = await device.SetAsDefaultAsync();
                        Assert.Equal(isDefault, device.IsDefaultDevice);
                    }
                    catch (ComInteropTimeoutException)
                    {
                        //Can't set the default, don't fail the test
                    }
                }

                var originalIsDefault = await originalCommDevice.SetAsDefaultAsync();
                Assert.True(originalIsDefault);
            }
        }

        [Fact(Skip = "The order is not guranteed, so skipping for now")]
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
        public void Device_Set_All_Default_Communications()
        {
            using (var controller = CreateTestController())
            {
                var originalCommDevice = controller.DefaultPlaybackCommunicationsDevice;

                foreach (var device in controller.GetPlaybackDevices(DeviceState.Active))
                {
                    try
                    {
                        var isDefault = device.SetAsDefaultCommunications();
                        Assert.Equal(isDefault, device.IsDefaultCommunicationsDevice);
                    }
                    catch (ComInteropTimeoutException)
                    {
                        //Can't set the default, don't fail the test
                    }
                }

                var originalIsDefault = originalCommDevice.SetAsDefaultCommunications();
                Assert.True(originalIsDefault);
            }
        }

        [Fact]
        public async Task Device_Set_All_Default_Communications_Async()
        {
            using (var controller = CreateTestController())
            {
                var originalCommDevice = controller.DefaultPlaybackCommunicationsDevice;

                foreach (var device in await controller.GetPlaybackDevicesAsync(DeviceState.Active))
                {
                    try
                    {
                        var isDefault = await device.SetAsDefaultCommunicationsAsync();
                        Assert.Equal(isDefault, device.IsDefaultCommunicationsDevice);
                    }
                    catch (ComInteropTimeoutException)
                    {
                        //Can't set the default, don't fail the test
                    }
                }

                var originalIsDefault = await originalCommDevice.SetAsDefaultCommunicationsAsync();
                Assert.True(originalIsDefault);
            }
        }

        [Fact(Skip = "The order is not guranteed, so skipping for now")]
        public async Task Device_Set_Default_Communications_Async_Returns_In_Order()
        {
            using (var controller = CreateTestController())
            {
                var order = new ConcurrentQueue<int>();
                var device = controller.DefaultPlaybackCommunicationsDevice;
                var current = 1;

                var t1 = device.SetAsDefaultCommunicationsAsync().ContinueWith(x =>
                {
                    order.Enqueue(current++);
                });

                var t2 = device.SetAsDefaultCommunicationsAsync().ContinueWith(x =>
                {
                    order.Enqueue(current++);
                });

                var t3 = device.SetAsDefaultCommunicationsAsync().ContinueWith(x =>
                {
                    order.Enqueue(current++);
                });

                var t4 = device.SetAsDefaultCommunicationsAsync().ContinueWith(x =>
                {
                    order.Enqueue(current++);
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
        public async Task Device_Mute_Async()
        {
            using (var controller = CreateTestController())
            {
                var isMuted = controller.DefaultPlaybackDevice.IsMuted;

                var newMute = await controller.DefaultPlaybackDevice.SetMuteAsync(true);
                Assert.True(newMute);

                newMute = await controller.DefaultPlaybackDevice.SetMuteAsync(false);
                Assert.False(newMute);

                await controller.DefaultPlaybackDevice.SetMuteAsync(isMuted);
            }
        }

    }
}
