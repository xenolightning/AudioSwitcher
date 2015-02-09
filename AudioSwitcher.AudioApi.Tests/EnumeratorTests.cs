using Xunit;

namespace AudioSwitcher.AudioApi.Tests
{
    public class EnumeratorTests
    {


        [Fact]
        public void DeviceState_HasFlag_All_Has_Active()
        {
            Assert.True(DeviceState.All.HasFlag(DeviceState.Active));
        }

        [Fact]
        public void DeviceState_HasFlag_All_Has_Disabled()
        {
            Assert.True(DeviceState.All.HasFlag(DeviceState.Disabled));
        }

        [Fact]
        public void DeviceState_HasFlag_All_Has_NotPresent()
        {
            Assert.True(DeviceState.All.HasFlag(DeviceState.NotPresent));
        }

        [Fact]
        public void DeviceState_HasFlag_All_Has_Unplugged()
        {
            Assert.True(DeviceState.All.HasFlag(DeviceState.Unplugged));
        }

        [Fact]
        public void DeviceType_HasFlag_All_Has_Playback()
        {
            Assert.True(DeviceType.All.HasFlag(DeviceType.Playback));
        }

        [Fact]
        public void DeviceType_HasFlag_All_Has_Capture()
        {
            Assert.True(DeviceType.All.HasFlag(DeviceType.Capture));
        }
    }
}
