using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AudioSwitcher.AudioApi
{
    public interface IDeviceEnumerator : IDisposable
    {
        IAudioController AudioController { get; set; }

        IDevice DefaultPlaybackDevice { get; }

        IDevice DefaultCommunicationsPlaybackDevice { get; }

        IDevice DefaultCaptureDevice { get; }

        IDevice DefaultCommunicationsCaptureDevice { get; }

        event AudioDeviceChangedHandler AudioDeviceChanged;

        IDevice GetDevice(Guid id);

        IDevice GetDevice(Guid id, DeviceState state);

        IDevice GetDefaultDevice(DeviceType deviceType, Role eRole);

        IEnumerable<IDevice> GetDevices(DeviceType deviceType, DeviceState state);

        Task<IDevice> GetDeviceAsync(Guid id);

        Task<IDevice> GetDeviceAsync(Guid id, DeviceState state);

        Task<IDevice> GetDefaultDeviceAsync(DeviceType deviceType, Role eRole);

        Task<IEnumerable<IDevice>> GetDevicesAsync(DeviceType deviceType, DeviceState state);

        bool SetDefaultDevice(IDevice dev);

        bool SetDefaultCommunicationsDevice(IDevice dev);

        Task<bool> SetDefaultDeviceAsync(IDevice dev);

        Task<bool> SetDefaultCommunicationsDeviceAsync(IDevice dev);
    }
}