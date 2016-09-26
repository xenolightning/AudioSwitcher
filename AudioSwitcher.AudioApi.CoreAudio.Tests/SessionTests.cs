using System.Linq;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi.Session;
using Xunit;

namespace AudioSwitcher.AudioApi.CoreAudio.Tests
{
    [Collection("CoreAudio_Session")]
    public class SessionTests
    {

        [Fact]
        public void CoreAudioSessionController_Exists_As_Capability()
        {
            using (var controller = new CoreAudioController())
            {
                var device = controller.DefaultPlaybackDevice;
                Assert.NotNull(device.GetCapability<IAudioSessionController>());
            }
        }

        [Fact]
        public async Task CoreAudioSession_IsMuted_When_Device_Is_Muted()
        {
            using (var controller = new CoreAudioController())
            {
                var device = controller.DefaultPlaybackDevice;
                var session = device.GetCapability<IAudioSessionController>().First();

                var oldDMute = device.IsMuted;
                var oldSMute = session.IsMuted;

                await session.SetMuteAsync(false);
                await device.SetMuteAsync(true);

                Assert.True(session.IsMuted);

                await device.SetMuteAsync(oldDMute);
                await session.SetMuteAsync(oldSMute);
            }
        }

    }
}
