using AudioSwitcher.AudioApi.Session;
using Xunit;
using Moq;

namespace AudioSwitcher.AudioApi.Tests
{
    public class SessionTests
    {

        [Fact]
        public void SessionDisconnectedArgs_Sets_Session()
        {
            var mockAudioSession = new Mock<IAudioSession>();

            var disconnectedArgs = new SessionDisconnectedArgs(mockAudioSession.Object);

            Assert.NotNull(disconnectedArgs);
            Assert.NotNull(disconnectedArgs.Session);
            Assert.Equal(mockAudioSession.Object, disconnectedArgs.Session);
        }

        [Fact]
        public void SessionMuteChangedArgs_Sets_Session_Muted_True()
        {
            var mockAudioSession = new Mock<IAudioSession>();

            var args = new SessionMuteChangedArgs(mockAudioSession.Object, true);

            Assert.NotNull(args);
            Assert.NotNull(args.Session);
            Assert.Equal(mockAudioSession.Object, args.Session);

            Assert.True(args.IsMuted);
        }

        [Fact]
        public void SessionMuteChangedArgs_Sets_Session_Muted_False()
        {
            var mockAudioSession = new Mock<IAudioSession>();

            var args = new SessionMuteChangedArgs(mockAudioSession.Object, false);

            Assert.NotNull(args);
            Assert.NotNull(args.Session);
            Assert.Equal(mockAudioSession.Object, args.Session);

            Assert.False(args.IsMuted);
        }

        [Fact]
        public void SessionStateChangedArgs_Sets_Session_State_Inactive()
        {
            var mockAudioSession = new Mock<IAudioSession>();
            const AudioSessionState state = AudioSessionState.Inactive;

            var args = new SessionStateChangedArgs(mockAudioSession.Object, state);

            Assert.NotNull(args);
            Assert.NotNull(args.Session);
            Assert.Equal(mockAudioSession.Object, args.Session);

            Assert.Equal(state, args.State);
        }

        [Fact]
        public void SessionStateChangedArgs_Sets_Session_State_Active()
        {
            var mockAudioSession = new Mock<IAudioSession>();
            const AudioSessionState state = AudioSessionState.Active;

            var args = new SessionStateChangedArgs(mockAudioSession.Object, state);

            Assert.NotNull(args);
            Assert.NotNull(args.Session);
            Assert.Equal(mockAudioSession.Object, args.Session);

            Assert.Equal(state, args.State);
        }

        [Fact]
        public void SessionStateChangedArgs_Sets_Session_State_Expired()
        {
            var mockAudioSession = new Mock<IAudioSession>();
            const AudioSessionState state = AudioSessionState.Expired;

            var args = new SessionStateChangedArgs(mockAudioSession.Object, state);

            Assert.NotNull(args);
            Assert.NotNull(args.Session);
            Assert.Equal(mockAudioSession.Object, args.Session);

            Assert.Equal(state, args.State);
        }

        [Fact]
        public void SessionVolumeChangedArgs_Sets_Session_Volume()
        {
            var mockAudioSession = new Mock<IAudioSession>();
            const double volume = 57;

            var args = new SessionVolumeChangedArgs(mockAudioSession.Object, volume);

            Assert.NotNull(args);
            Assert.NotNull(args.Session);
            Assert.Equal(mockAudioSession.Object, args.Session);

            Assert.Equal(volume, args.Volume);
        }

        [Fact]
        public void SessionPeakValueChangedArgs_Sets_Session_Volume()
        {
            var mockAudioSession = new Mock<IAudioSession>();
            const double volume = 57;

            var args = new SessionPeakValueChangedArgs(mockAudioSession.Object, volume);

            Assert.NotNull(args);
            Assert.NotNull(args.Session);
            Assert.Equal(mockAudioSession.Object, args.Session);

            Assert.Equal(volume, args.PeakValue);
        }
    }

}
