namespace AudioSwitcher.AudioApi.Session
{
    public sealed class AudioSessionsChanged
    {
        public IAudioSession Session { get; private set; }
        public AudioSessionsChangedType ChangeType { get; private set; }

        public AudioSessionsChanged(IAudioSession session, AudioSessionsChangedType changeType)
        {
            Session = session;
            ChangeType = changeType;
        }
    }

    public enum AudioSessionsChangedType
    {
        Created,
        Disconnected
    }
}