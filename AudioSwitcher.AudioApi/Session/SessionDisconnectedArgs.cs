namespace AudioSwitcher.AudioApi.Session
{
    public sealed class SessionDisconnectedArgs
    {

        public IAudioSession Session
        {
            get;
            private set;
        }

        public SessionDisconnectedArgs(IAudioSession session)
        {
            Session = session;
        }
    }
}