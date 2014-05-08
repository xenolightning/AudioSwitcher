using System.Collections.Generic;
using System.IO;

namespace AudioSwitcher.AudioApi
{
    /// <summary>
    ///     Used to manage the preferred devices
    /// </summary>
    public abstract class PreferredDeviceManager : IPreferredDeviceManager
    {
        public AudioContext Context { get; set; }

        public abstract IEnumerable<AudioDevice> PreferredDevices { get; }

        public abstract AudioDevice NextPlaybackDevice();

        public abstract AudioDevice PreviousPlaybackDevice();

        public abstract AudioDevice NextRecordingDevice();

        public abstract AudioDevice PreviousRecordingDevice();

        public abstract void AddDevice(IAudioDevice ad, int position = 0);

        public abstract void RemoveDevice(IAudioDevice ad);

        public abstract bool IsPreferredDevice(IAudioDevice ad);

        public abstract void Save(Stream s);

        public abstract void Load(Stream s);
    }
}