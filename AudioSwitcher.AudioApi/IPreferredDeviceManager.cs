using System.Collections.Generic;
using System.IO;

namespace AudioSwitcher.AudioApi
{
    public interface IPreferredDeviceManager
    {
        AudioContext Context { get; set; }

        IEnumerable<AudioDevice> PreferredDevices { get; }

        AudioDevice NextPlaybackDevice();

        AudioDevice PreviousPlaybackDevice();

        AudioDevice NextRecordingDevice();

        AudioDevice PreviousRecordingDevice();

        void AddDevice(IAudioDevice ad, int position = 0);

        void RemoveDevice(IAudioDevice ad);

        bool IsPreferredDevice(IAudioDevice ad);

        void Save(Stream s);

        void Load(Stream s);
    }
}