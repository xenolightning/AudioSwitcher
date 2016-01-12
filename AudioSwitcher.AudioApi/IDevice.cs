using System;
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

        double Volume { get; set; }

        IObservable<DeviceVolumeChangedArgs> VolumeChanged { get; }

        IObservable<DeviceMuteChangedArgs> MuteChanged { get; }

        IObservable<DevicePropertyChangedArgs> PropertyChanged { get; }

        IObservable<DefaultDeviceChangedArgs> DefaultChanged { get; }

        IObservable<DeviceStateChangedArgs> StateChanged { get; }

        IObservable<DevicePeakValueChangedArgs> PeakValueChanged { get; }

        bool SetAsDefault();

        Task<bool> SetAsDefaultAsync();

        bool SetAsDefaultCommunications();

        Task<bool> SetAsDefaultCommunicationsAsync();

        bool Mute(bool mute);

        Task<bool> MuteAsync(bool mute);

        bool ToggleMute();

        Task<bool> ToggleMuteAsync();

    }
}