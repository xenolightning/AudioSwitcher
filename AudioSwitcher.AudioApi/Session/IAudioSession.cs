using System;

namespace AudioSwitcher.AudioApi.Session
{
    public interface IAudioSession : 
        IObservable<AudioSessionStateChanged>, 
        IObservable<AudioSessionDisconnected>, 
        IDisposable
    {
        string Id { get; }

        int ProcessId { get; }

        string DisplayName { get; }

        bool IsSystemSession { get; }

        int Volume { get; set; }

        bool IsMuted { get; set; }

        AudioSessionState SessionState { get; }

    }
}