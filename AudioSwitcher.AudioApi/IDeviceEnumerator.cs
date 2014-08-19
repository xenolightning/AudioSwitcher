using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi
{
    [ComVisible(false)]
    public interface IDeviceEnumerator
    {
        AudioController AudioController { get; set; }

        IDevice DefaultPlaybackDevice { get; }

        IDevice DefaultCommunicationsPlaybackDevice { get; }

        IDevice DefaultCaptureDevice { get; }

        IDevice DefaultCommunicationsCaptureDevice { get; }

        event AudioDeviceChangedHandler AudioDeviceChanged;

        IDevice GetDevice(Guid id);

        IDevice GetDefaultDevice(DataFlow dataflow, Role eRole);

        IEnumerable<IDevice> GetDevices(DataFlow dataflow, DeviceState state);

        bool SetDefaultDevice(IDevice dev);

        bool SetDefaultCommunicationsDevice(IDevice dev);
    }
}