namespace AudioSwitcher.AudioApi.Session
{
    public interface IAudioSessionEndpoint
    {
        /// <summary>
        /// Returns null if sessions are not available
        /// </summary>
        IAudioSessionController SessionController { get; }
    }
}