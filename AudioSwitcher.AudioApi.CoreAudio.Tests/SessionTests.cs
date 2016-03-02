using System.Linq;
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
        public void CoreAudioSession_IsMuted_When_Device_Is_Muted()
        {
            using (var controller = new CoreAudioController())
            {
                var device = controller.DefaultPlaybackDevice;
                var session = device.GetCapability<IAudioSessionController>().First();

                var oldDMute = device.IsMuted;
                var oldSMute = session.IsMuted;

                session.IsMuted = false;
                device.Mute(true);

                Assert.True(session.IsMuted);

                device.Mute(oldDMute);
                session.IsMuted = oldSMute;
            }
        }

    }
}
