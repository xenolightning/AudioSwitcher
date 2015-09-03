namespace AudioSwitcher.AudioApi.Session
{
    public interface IAudioSessionEndpoint
    {

        bool IsSessionEndpoint { get; }

        IAudioSessionController SessionController { get; }
    }
}