namespace AudioSwitcher.AudioApi.Session
{
    public interface IAudioSessionEndpoint
    {
        IAudioSessionController SessionController { get; }
    }
}