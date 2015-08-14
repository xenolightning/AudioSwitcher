using System;

namespace AudioSwitcher.AudioApi.Session
{
    public interface IAudioSession
    {
        Guid SessionId { get; }

        int ProcessId { get; }

        int ProcessName { get; }

        bool IsSystemSession { get; }

        int Volume { get; set; }

        AudioSessionState SessionState { get; }
    }
}