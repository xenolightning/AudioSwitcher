using System.Collections.Generic;
using System.IO;

namespace AudioSwitcher.AudioApi
{
    public interface IPreferredDeviceManager
    {
        AudioContext Context { get; }

        IEnumerable<AudioDevice> PreferredDevices { get; }

        AudioDevice NextPlaybackDevice();

        AudioDevice PreviousPlaybackDevice();

        AudioDevice NextRecordingDevice();

        AudioDevice PreviousRecordingDevice();

        void AddDevice(AudioDevice ad, int position = 0);

        void RemoveDevice(AudioDevice ad);

        bool IsPreferredDevice(AudioDevice ad);

        void Save(Stream s);

        void Load(Stream s);
    }
}