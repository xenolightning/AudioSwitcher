namespace AudioSwitcher.AudioApi.Session
{
    public sealed class AudioSessionsChanged
    {
        public string SessionId { get; private set; }
        public AudioSessionsChangedType ChangeType { get; private set; }

        public AudioSessionsChanged(string sessionId, AudioSessionsChangedType changeType)
        {
            SessionId = sessionId;
            ChangeType = changeType;
        }
    }

    public enum AudioSessionsChangedType
    {
        Created,
        Disconnected
    }
}