namespace AudioSwitcher.AudioApi.Session
{
    public sealed class AudioSessionChanged
    {
        public string SessionId { get; private set; }
        public AudioSessionChangedType ChangeType { get; private set; }

        public AudioSessionChanged(string sessionId, AudioSessionChangedType changeType)
        {
            SessionId = sessionId;
            ChangeType = changeType;
        }
    }

    public enum AudioSessionChangedType
    {
        Created,
        Disconnected
    }
}