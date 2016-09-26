using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AudioSwitcher.AudioApi
{
    public interface IDevice
    {
        IAudioController Controller { get; }

        Guid Id { get; }

        string Name { get; }

        string InterfaceName { get; }

        string FullName { get; }

        DeviceIcon Icon { get; }

        string IconPath { get; }

        bool IsDefaultDevice { get; }

        bool IsDefaultCommunicationsDevice { get; }

        DeviceState State { get; }

        DeviceType DeviceType { get; }

        bool IsPlaybackDevice { get; }

        bool IsCaptureDevice { get; }

        bool IsMuted { get; }

        double Volume { get; }

        IObservable<DeviceVolumeChangedArgs> VolumeChanged { get; }

        IObservable<DeviceMuteChangedArgs> MuteChanged { get; }

        IObservable<DevicePropertyChangedArgs> PropertyChanged { get; }

        IObservable<DefaultDeviceChangedArgs> DefaultChanged { get; }

        IObservable<DeviceStateChangedArgs> StateChanged { get; }

        IObservable<DevicePeakValueChangedArgs> PeakValueChanged { get; }

        bool SetAsDefault();
        bool SetAsDefault(CancellationToken cancellationToken);

        Task<bool> SetAsDefaultAsync();
        Task<bool> SetAsDefaultAsync(CancellationToken cancellationToken);

        bool SetAsDefaultCommunications();
        bool SetAsDefaultCommunications(CancellationToken cancellationToken);

        Task<bool> SetAsDefaultCommunicationsAsync();
        Task<bool> SetAsDefaultCommunicationsAsync(CancellationToken cancellationToken);

        Task<bool> SetMuteAsync(bool mute);
        Task<bool> SetMuteAsync(bool mute, CancellationToken cancellationToken);

        Task<bool> ToggleMuteAsync();
        Task<bool> ToggleMuteAsync(CancellationToken cancellationToken);

        Task<double> GetVolumeAsync();
        Task<double> GetVolumeAsync(CancellationToken cancellationToken);

        Task<double> SetVolumeAsync(double volume);
        Task<double> SetVolumeAsync(double volume, CancellationToken cancellationToken);

        bool HasCapability<TCapability>() where TCapability : IDeviceCapability;

        TCapability GetCapability<TCapability>() where TCapability : IDeviceCapability;

        IEnumerable<IDeviceCapability> GetAllCapabilities();
    }
}