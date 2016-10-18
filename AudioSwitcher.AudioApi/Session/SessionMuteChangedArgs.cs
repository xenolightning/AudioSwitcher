namespace AudioSwitcher.AudioApi.Session
{
    public class SessionMuteChangedArgs
    {
        public IAudioSession Session { get; private set; }

        public bool IsMuted { get; private set; }

        public SessionMuteChangedArgs(IAudioSession session, bool isMuted)
        {
            Session = session;
            IsMuted = isMuted;
        }
    }
}