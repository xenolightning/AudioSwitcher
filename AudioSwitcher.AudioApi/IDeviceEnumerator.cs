using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi
{
    [ComVisible(false)]
    public interface IDeviceEnumerator
    {
        AudioDevice DefaultPlaybackDevice { get; }

        AudioDevice DefaultCommunicationsPlaybackDevice { get; }

        AudioDevice DefaultRecordingDevice { get; }

        AudioDevice DefaultCommunicationsRecordingDevice { get; }

        event AudioDeviceChangedHandler AudioDeviceChanged;

        AudioDevice GetAudioDevice(Guid id);

        AudioDevice GetDefaultAudioDevice(DataFlow dataflow, Role eRole);

        IEnumerable<AudioDevice> GetAudioDevices(DataFlow dataflow, DeviceState state);

        bool SetDefaultDevice(AudioDevice dev);

        bool SetDefaultCommunicationsDevice(AudioDevice dev);
    }

    [ComVisible(false)]
    public interface IDeviceEnumerator<T> : IDeviceEnumerator
        where T : AudioDevice
    {
        new T DefaultPlaybackDevice { get; }

        new T DefaultCommunicationsPlaybackDevice { get; }

        new T DefaultRecordingDevice { get; }

        new T DefaultCommunicationsRecordingDevice { get; }

        new T GetAudioDevice(Guid id);

        new T GetDefaultAudioDevice(DataFlow dataflow, Role eRole);

        new IEnumerable<T> GetAudioDevices(DataFlow dataflow, DeviceState state);

        bool SetDefaultDevice(T dev);

        bool SetDefaultCommunicationsDevice(T dev);
    }
}