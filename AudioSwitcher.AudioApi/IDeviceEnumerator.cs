using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AudioSwitcher.AudioApi
{
    public interface IDeviceEnumerator
    {
        AudioController AudioController { get; set; }

        IDevice DefaultPlaybackDevice { get; }

        IDevice DefaultCommunicationsPlaybackDevice { get; }

        IDevice DefaultCaptureDevice { get; }

        IDevice DefaultCommunicationsCaptureDevice { get; }

        event AudioDeviceChangedHandler AudioDeviceChanged;

        IDevice GetDevice(Guid id);

        IDevice GetDefaultDevice(DeviceType deviceType, Role eRole);

        IEnumerable<IDevice> GetDevices(DeviceType deviceType, DeviceState state);

        Task<IDevice> GetDeviceAsync(Guid id);

        Task<IDevice> GetDefaultDeviceAsync(DeviceType deviceType, Role eRole);

        Task<IEnumerable<IDevice>> GetDevicesAsync(DeviceType deviceType, DeviceState state);

        bool SetDefaultDevice(IDevice dev);

        bool SetDefaultCommunicationsDevice(IDevice dev);

        Task<bool> SetDefaultDeviceAsync(IDevice dev);

        Task<bool> SetDefaultCommunicationsDeviceAsync(IDevice dev);
    }
}