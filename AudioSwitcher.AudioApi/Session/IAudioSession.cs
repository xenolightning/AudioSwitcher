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

        event SessionStateChangedEventHandler StateChanged;
        event SessionDisconnectedEventHandler Disconnected;
    }

    public delegate void SessionDisconnectedEventHandler(IAudioSession sender);

    public delegate void SessionStateChangedEventHandler(IAudioSession sender, AudioSessionState state);
}