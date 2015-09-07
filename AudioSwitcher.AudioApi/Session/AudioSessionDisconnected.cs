namespace AudioSwitcher.AudioApi.Session
{
    public class AudioSessionDisconnected
    {

        public IAudioSession Session
        {
            get;
            private set;
        }

        public AudioSessionDisconnected(IAudioSession session)
        {
            Session = session;
        }
    }
}