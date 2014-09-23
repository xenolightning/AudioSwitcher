using System;
using System.Threading.Tasks;

namespace AudioSwitcher.AudioApi
{
    public interface IDevice
    {
        IDeviceEnumerator Enumerator { get; }

        Guid Id { get; }

        string Description { get; }

        string ShortName { get; }

        string SystemName { get; }

        string FullName { get; }

        string IconPath { get; }

        bool IsDefaultDevice { get; }

        bool IsDefaultCommunicationsDevice { get; }

        DeviceState State { get; }

        DeviceType DeviceType { get; }

        bool IsPlaybackDevice { get; }

        bool IsCaptureDevice { get; }

        bool IsMuted { get; }

        int Volume { get; set; }

        bool SetAsDefault();

        Task<bool> SetAsDefaultAsync();

        bool SetAsDefaultCommunications();

        Task<bool> SetAsDefaultCommunicationsAsync();

        bool Mute();

        Task<bool> MuteAsync();

        bool UnMute();

        Task<bool> UnMuteAsync();

        bool ToggleMute();

        Task<bool> ToggleMuteAsync();

        event AudioDeviceChangedHandler VolumeChanged;
    }
}