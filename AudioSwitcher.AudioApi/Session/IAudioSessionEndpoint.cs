namespace AudioSwitcher.AudioApi.Session
{
    public interface IAudioSessionEndpoint
    {
        IAudioSessionManager SessionManager { get; }
    }
}