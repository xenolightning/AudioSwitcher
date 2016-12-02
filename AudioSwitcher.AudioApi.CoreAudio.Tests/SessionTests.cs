using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi.Observables;
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

        [Fact]
        public async Task CoreAudioSession_SetMute_True()
        {
            using (var controller = new CoreAudioController())
            {
                var device = controller.DefaultPlaybackDevice;
                var session = device.GetCapability<IAudioSessionController>().First();

                var oldDMute = device.IsMuted;
                var oldSMute = session.IsMuted;

                await device.SetMuteAsync(false);
                await session.SetMuteAsync(true);

                Assert.True(session.IsMuted);

                await device.SetMuteAsync(oldDMute);
                await session.SetMuteAsync(oldSMute);
            }
        }

        [Fact]
        public async Task CoreAudioSession_SetMute_False()
        {
            using (var controller = new CoreAudioController())
            {
                var device = controller.DefaultPlaybackDevice;
                var session = device.GetCapability<IAudioSessionController>().First();

                var oldDMute = device.IsMuted;
                var oldSMute = session.IsMuted;

                await device.SetMuteAsync(false);
                await session.SetMuteAsync(false);

                Assert.False(session.IsMuted);

                await device.SetMuteAsync(oldDMute);
                await session.SetMuteAsync(oldSMute);
            }
        }

        [Fact]
        public async Task CoreAudioSession_SetVolume_Zero()
        {
            using (var controller = new CoreAudioController())
            {
                var device = controller.DefaultPlaybackDevice;
                var session = device.GetCapability<IAudioSessionController>().First();

                var oldVol = session.Volume;

                await session.SetVolumeAsync(-1);

                Assert.Equal(0, session.Volume);

                await session.SetVolumeAsync(oldVol);
            }
        }

        [Fact]
        public async Task CoreAudioSession_SetVolume_Greater_Than_100()
        {
            using (var controller = new CoreAudioController())
            {
                var device = controller.DefaultPlaybackDevice;
                var session = device.GetCapability<IAudioSessionController>().First();

                var oldVol = session.Volume;

                await session.SetVolumeAsync(110);

                Assert.Equal(100, session.Volume);

                await session.SetVolumeAsync(oldVol);
            }
        }

        [Fact]
        public async Task CoreAudioSession_SetVolume_Valid_value()
        {
            using (var controller = new CoreAudioController())
            {
                var device = controller.DefaultPlaybackDevice;
                var session = device.GetCapability<IAudioSessionController>().First();

                var oldVol = session.Volume;

                await session.SetVolumeAsync(60);

                Assert.Equal(60, Math.Round(session.Volume, 0));

                await session.SetVolumeAsync(oldVol);
            }
        }

        [Fact]
        public async Task CoreAudioSession_SetVolume_Raises_Event()
        {
            using (var controller = new CoreAudioController())
            {
                var device = controller.DefaultPlaybackDevice;
                var session = device.GetCapability<IAudioSessionController>().First();

                var oldVol = session.Volume;
                var resetEvent = new ManualResetEvent(false);

                session.VolumeChanged.Subscribe(x =>
                {
                    resetEvent.Set();
                });

                await session.SetVolumeAsync(60);
                await session.SetVolumeAsync(50);

                resetEvent.WaitOne();

                await session.SetVolumeAsync(oldVol);
            }
        }

        [Fact]
        public async Task CoreAudioSession_SetMute_Raises_Event()
        {
            using (var controller = new CoreAudioController())
            {
                var device = controller.DefaultPlaybackDevice;
                var session = device.GetCapability<IAudioSessionController>().First();

                var oldMute = session.IsMuted;
                var resetEvent = new ManualResetEvent(false);

                session.MuteChanged.Subscribe(x =>
                {
                    resetEvent.Set();
                });

                await session.SetMuteAsync(false);
                await session.SetMuteAsync(true);

                resetEvent.WaitOne();

                await session.SetMuteAsync(oldMute);
            }
        }

        [Fact]
        public async Task CoreAudioSession_Can_Subscribe_To_PeakValueChanged()
        {
            using (var controller = new CoreAudioController())
            {
                var device = controller.DefaultPlaybackDevice;
                var session = device.GetCapability<IAudioSessionController>().First();

                var sub = session.PeakValueChanged.Subscribe(x =>
                {
                    
                });

                sub.Dispose();
            }
        }

        [Fact]
        public async Task CoreAudioSession_Can_Subscribe_To_MuteChanged()
        {
            using (var controller = new CoreAudioController())
            {
                var device = controller.DefaultPlaybackDevice;
                var session = device.GetCapability<IAudioSessionController>().First();

                var sub = session.MuteChanged.Subscribe(x =>
                {

                });

                sub.Dispose();
            }
        }

        [Fact]
        public async Task CoreAudioSession_Can_Subscribe_To_StateChanged()
        {
            using (var controller = new CoreAudioController())
            {
                var device = controller.DefaultPlaybackDevice;
                var session = device.GetCapability<IAudioSessionController>().First();

                var sub = session.StateChanged.Subscribe(x =>
                {

                });

                sub.Dispose();
            }
        }

        [Fact]
        public async Task CoreAudioSession_Can_Subscribe_To_VolumeChanged()
        {
            using (var controller = new CoreAudioController())
            {
                var device = controller.DefaultPlaybackDevice;
                var session = device.GetCapability<IAudioSessionController>().First();

                var sub = session.VolumeChanged.Subscribe(x =>
                {

                });

                sub.Dispose();
            }
        }
    }
}
