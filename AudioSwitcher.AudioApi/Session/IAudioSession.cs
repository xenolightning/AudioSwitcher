using System;

namespace AudioSwitcher.AudioApi.Session
{
    public interface IAudioSession : IDisposable
    {
        string Id { get; }

        int ProcessId { get; }

        string DisplayName { get; }

        string ExecutablePath { get; }

        bool IsSystemSession { get; }

        int Volume { get; set; }

        bool IsMuted { get; set; }

        AudioSessionState SessionState { get; }

        IObservable<AudioSessionStateChanged> StateChanged { get; }
        IObservable<AudioSessionDisconnected> Disconnected { get; }

    }
}