using System;
using System.Threading;
using System.Threading.Tasks;

namespace AudioSwitcher.AudioApi.Session
{
    public interface IAudioSession
    {
        string Id { get; }

        int ProcessId { get; }

        string DisplayName { get; }

        string IconPath { get; }

        string ExecutablePath { get; }

        bool IsSystemSession { get; }

        double Volume { get; }

        bool IsMuted { get; }

        AudioSessionState SessionState { get; }

        IDevice Device { get; }

        IObservable<SessionVolumeChangedArgs> VolumeChanged { get; }

        IObservable<SessionPeakValueChangedArgs> PeakValueChanged { get; }

        IObservable<SessionMuteChangedArgs> MuteChanged { get; }

        IObservable<SessionStateChangedArgs> StateChanged { get; }

        IObservable<SessionDisconnectedArgs> Disconnected { get; }

        Task<double> GetVolumeAsync();
        Task<double> GetVolumeAsync(CancellationToken cancellationToken);

        Task<double> SetVolumeAsync(double volume);
        Task<double> SetVolumeAsync(double volume, CancellationToken cancellationToken);

        Task<bool> GetMuteAsync();
        Task<bool> GetMuteAsync(CancellationToken cancellationToken);

        Task<bool> SetMuteAsync(bool muted);
        Task<bool> SetMuteAsync(bool muted, CancellationToken cancellationToken);

    }
}