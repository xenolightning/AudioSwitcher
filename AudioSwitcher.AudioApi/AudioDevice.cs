using System;
using System.Drawing;
using NAudio.CoreAudioApi;

namespace AudioSwitcher.AudioApi
{
    public abstract class AudioDevice : IAudioDevice
    {
        public abstract Guid ID { get; }

        public abstract string Description { get; }

        public abstract string ShortName { get; }

        public abstract string SystemName { get; }

        public abstract string FullName { get; }

        public abstract bool IsDefaultDevice { get; }

        public abstract bool IsDefaultCommunicationsDevice { get; }

        public abstract DeviceState State { get; }

        public abstract DataFlow DataFlow { get; }

        public abstract bool IsPlaybackDevice { get; }

        public abstract bool IsRecordingDevice { get; }

        public abstract bool IsMuted { get; }

        public abstract int Volume { get; set; }

        public abstract bool SetAsDefault();

        public abstract bool SetAsDefaultCommunications();

        public abstract bool Mute();

        public abstract bool UnMute();

        public abstract bool ToggleMute();

        public abstract Icon GetIcon();

        public abstract Icon GetIcon(int width, int height);
    }
}