namespace AudioSwitcher.AudioApi.Session
{
    public interface IAudioSessionEndpoint
    {

        bool IsSupported { get; }

        IAudioSessionController SessionController { get; }
    }
}