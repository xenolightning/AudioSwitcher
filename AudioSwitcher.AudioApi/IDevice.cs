using System;

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

        bool SetAsDefaultCommunications();

        bool Mute();

        bool UnMute();

        bool ToggleMute();

        event AudioDeviceChangedHandler VolumeChanged;
    }
}