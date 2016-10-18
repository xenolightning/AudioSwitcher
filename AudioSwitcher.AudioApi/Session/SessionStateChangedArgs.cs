namespace AudioSwitcher.AudioApi.Session
{
    public class SessionStateChangedArgs
    {
        public IAudioSession Session { get; private set; }

        public AudioSessionState State { get; private set; }

        public SessionStateChangedArgs(IAudioSession session, AudioSessionState state)
        {
            Session = session;
            State = state;
        }
    }
}