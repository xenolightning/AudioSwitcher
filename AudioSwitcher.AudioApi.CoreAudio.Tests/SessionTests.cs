using System.Linq;
using Xunit;

namespace AudioSwitcher.AudioApi.CoreAudio.Tests
{
    [Collection("CoreAudio")]
    public class SessionTests
    {

        [Fact]
        public void CoreAudioSession_IsMuted_When_Device_Is_Muted()
        {
            using (var controller = new CoreAudioController())
            {
                var device = controller.DefaultPlaybackDevice;
                var session = device.SessionController.First();

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
