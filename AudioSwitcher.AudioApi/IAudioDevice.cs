using System;

namespace AudioSwitcher.AudioApi
{
    public interface IAudioDevice
    {
        Guid ID { get; }

        string Description { get; }

        string ShortName { get; }

        string SystemName { get; }

        string FullName { get; }

        string IconPath { get; }

        bool IsDefaultDevice { get; }

        bool IsDefaultCommunicationsDevice { get; }

        DeviceState State { get; }

        DataFlow DataFlow { get; }

        bool IsPlaybackDevice { get; }

        bool IsRecordingDevice { get; }

        bool IsMuted { get; }

        int Volume { get; set; }

        bool SetAsDefault();

        bool SetAsDefaultCommunications();

        bool Mute();

        bool UnMute();

        bool ToggleMute();

    }
}