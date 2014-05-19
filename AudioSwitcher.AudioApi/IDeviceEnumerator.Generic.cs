using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi
{
    [ComVisible(false)]
    public interface IDeviceEnumerator<T> : IDeviceEnumerator
        where T : Device
    {
        new T DefaultPlaybackDevice { get; }

        new T DefaultCommunicationsPlaybackDevice { get; }

        new T DefaultRecordingDevice { get; }

        new T DefaultCommunicationsRecordingDevice { get; }

        new T GetDevice(Guid id);

        new T GetDefaultDevice(DataFlow dataflow, Role eRole);

        new IEnumerable<T> GetDevices(DataFlow dataflow, DeviceState state);

        bool SetDefaultDevice(T dev);

        bool SetDefaultCommunicationsDevice(T dev);
    }
}