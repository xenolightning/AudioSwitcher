using System;

namespace AudioSwitcher.AudioApi.CoreAudio.Interfaces
{
    internal struct AudioVolumeNotificationDataStruct
    {
        public Guid EventContext;
        public bool Muted;
        public float MasterVolume;
        public uint Channels;
        public float ChannelVolume;
    }
}