using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi
{
    [ComVisible(false)]
    public interface IDeviceEnumerator
    {
        Controller Controller { get; set; }

        Device DefaultPlaybackDevice { get; }

        Device DefaultCommunicationsPlaybackDevice { get; }

        Device DefaultRecordingDevice { get; }

        Device DefaultCommunicationsRecordingDevice { get; }

        event AudioDeviceChangedHandler AudioDeviceChanged;

        Device GetDevice(Guid id);

        Device GetDefaultDevice(DataFlow dataflow, Role eRole);

        IEnumerable<Device> GetDevices(DataFlow dataflow, DeviceState state);

        bool SetDefaultDevice(Device dev);

        bool SetDefaultCommunicationsDevice(Device dev);
    }
}