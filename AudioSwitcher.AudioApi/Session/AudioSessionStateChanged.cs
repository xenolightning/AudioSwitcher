namespace AudioSwitcher.AudioApi.Session
{
    public class AudioSessionStateChanged
    {

        public IAudioSession Session
        {
            get;
            private set;
        }

        public AudioSessionState State
        {
            get;
            private set;
        }

        public AudioSessionStateChanged(IAudioSession session, AudioSessionState state)
        {
            Session = session;
            State = state;
        }
    }
}