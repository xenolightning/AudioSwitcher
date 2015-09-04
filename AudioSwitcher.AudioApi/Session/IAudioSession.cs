using System;

namespace AudioSwitcher.AudioApi.Session
{
    public interface IAudioSession : IDisposable
    {
        string SessionId { get; }

        int ProcessId { get; }

        string DisplayName { get; }

        bool IsSystemSession { get; }

        int Volume { get; set; }

        AudioSessionState SessionState { get; }

        //IObservable<AudioSessionState> StateChanged { get; }
    }
}