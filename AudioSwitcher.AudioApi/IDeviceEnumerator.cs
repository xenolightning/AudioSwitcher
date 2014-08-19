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

        IDevice GetDefaultDevice(DeviceType DeviceType, Role eRole);

        IEnumerable<IDevice> GetDevices(DeviceType DeviceType, DeviceState state);

        bool SetDefaultDevice(IDevice dev);

        bool SetDefaultCommunicationsDevice(IDevice dev);
    }
}