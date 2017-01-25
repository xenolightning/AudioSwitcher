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
